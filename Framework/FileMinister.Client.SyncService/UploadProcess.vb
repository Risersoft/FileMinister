Option Strict Off
Imports System.Configuration
Imports System.Data.SqlClient
Imports log4net
Imports risersoft.shared.portable.Model
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable

Public Class UploadProcess
    Private Shared ReadOnly _CommandTimeoutInSeconds As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="CommandTimeoutInSeconds"))
    Private Shared ReadOnly LoggerCheckOutProcess As ILog = LogManager.GetLogger(name:="CheckOutProcessLogger")
    Private Shared ReadOnly LoggerCheckInProcess As ILog = LogManager.GetLogger(name:="CheckInProcessLogger")
    Private Shared ReadOnly LoggerFileSync As ILog = LogManager.GetLogger(name:="FileSyncLogger")

    Public Event FileCheckedOut(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid, IsCheckedOutByDifferentUser As Boolean)

    Private ReadOnly _ShareId As Integer
    Private ReadOnly _ConnectionString As String
    Private ReadOnly _UserInfo As LocalWorkSpaceInfo

    Public Sub New(ShareId As Integer, oUserInfo As LocalWorkSpaceInfo, Optional ConnectionString As String = Nothing)
        _ShareId = ShareId
        _ConnectionString = ConnectionString
        _UserInfo = oUserInfo
    End Sub

    Public Sub CheckOutProcess()
        Try
            'Get Files that hve to be checked out
            Dim FilesForCheckOut As DataTable = GetFilesforCheckOut()
            Dim msg As String = String.Empty

            If (Not FilesForCheckOut Is Nothing) Then
                If (FilesForCheckOut.Rows.Count > 0) Then

                    For Each dr As DataRow In FilesForCheckOut.Rows
                        Dim isSuccessful As Boolean = False
                        Dim isCheckedOutByDifferentUser As Boolean = False
                        Dim FileSystemEntryId As Guid
                        Dim PrevFileSystemEntryVersionId As Guid = Guid.Empty
                        'Dim fileSystemEntryStatusId As Byte = Convert.ToByte(dr("FileSystemEntryStatusId").ToString())
                        Dim operationType As String = dr("OperationType").ToString()

                        If (Guid.TryParse(input:=dr("FileSystemEntryId").ToString(), result:=FileSystemEntryId)) Then

                            Dim result As ResultInfo(Of Boolean, Status) = Nothing

                            If (operationType.ToLower() = "c") Then

                                If Not String.IsNullOrEmpty(value:=dr("PreviousFileSystemEntryVersionId").ToString()) Then
                                    PrevFileSystemEntryVersionId = Guid.Parse(input:=dr("PreviousFileSystemEntryVersionId").ToString())
                                End If

                                Dim strVersionNumber As String = dr(columnName:="VersionNumber").ToString()
                                Dim oFileInfo As FileEntryInfo = MapFileSystemEntryInfo(dr:=dr)

                                If (PrevFileSystemEntryVersionId = Guid.Empty AndAlso (String.IsNullOrEmpty(strVersionNumber) OrElse strVersionNumber = "0")) Then
                                    'Case of Add and Checkout or Add in case of Folder

                                    If oFileInfo.FileEntryTypeId = FileType.Folder Then
                                        'Case of folder
                                        Try
                                            result = AddFolder(oFileInfo, oFileInfo.FileVersion)

                                            If Not result.Data Then
                                                msg = String.Format("Error While Adding a Folder (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                                msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                                LoggerCheckOutProcess.Error(msg)
                                            End If
                                        Catch ex As Exception
                                            LoggerCheckOutProcess.Error(ex.Message, ex)
                                        End Try

                                    Else
                                        'case of file
                                        Try
                                            result = AddAndCheckoutFile(oFileInfo, oFileInfo.FileVersion)

                                            If Not result.Data Then
                                                msg = String.Format("Error While Adding and CheckingOut a File (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                                msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                                LoggerCheckOutProcess.Error(msg)
                                            End If
                                        Catch ex As Exception
                                            LoggerCheckOutProcess.Error(ex.Message, ex)
                                        End Try
                                        isCheckedOutByDifferentUser = If(result.Status = Status.FileCheckedOutByDifferentUser, True, False)
                                        isSuccessful = result.Data
                                    End If
                                Else
                                    'Case of normal checkout
                                    If Not String.IsNullOrEmpty(strVersionNumber) Then
                                        Dim VersionNumber As Integer = Convert.ToInt32(strVersionNumber)
                                        ' Case of File Checkout - First check whether Server delete Type of conflict is resolved (with Client Win) for this file by the current user. If yes, the call undelete API on server and then Call Checkout API
                                        Try
                                            Dim conflictId As String = dr("FileSystemEntryVersionConflictId").ToString()

                                            If (Not String.IsNullOrEmpty(conflictId)) Then
                                                ' Call Server Undelete API and the Checkout API
                                                Dim proxy As FileClient = New FileClient(_UserInfo)
                                                result = proxy.UndoDelete(FileSystemEntryId)

                                                If result.Data Then
                                                    'call checkout
                                                    result = CheckoutFile(fileSystemEntryId:=FileSystemEntryId, versionNumber:=VersionNumber)
                                                Else
                                                    msg = String.Format("Error While UnDeleting a File/Folder (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                                    msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                                    LoggerCheckOutProcess.Error(msg)
                                                End If
                                            Else
                                                ' case of no conflict - just call the checkout API
                                                result = CheckoutFile(fileSystemEntryId:=FileSystemEntryId, versionNumber:=VersionNumber)

                                            End If
                                            isCheckedOutByDifferentUser = If(result.Status = Status.FileCheckedOutByDifferentUser, True, False)
                                            isSuccessful = result.Data

                                            If Not result.Data Then
                                                msg = String.Format("Error While CheckingOut a File/Folder (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                                msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                                LoggerCheckOutProcess.Error(msg)
                                            End If

                                        Catch ex As Exception
                                            LoggerCheckOutProcess.Error(ex.Message, ex)
                                        End Try

                                    Else
                                        msg = String.Format("CurrentVersion Not returned, Could Not CheckOut (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                        LoggerCheckOutProcess.Error(msg)
                                    End If

                                End If

                            ElseIf (operationType.ToLower() = "u") Then
                                'Case of UnCheckout - Call the API
                                Try
                                    result = UndoCheckoutFile(FileSystemEntryId)

                                    If Not result.Data Then
                                        msg = String.Format("Error While Undoing a CheckOut of a File (fileId: '{0}') ", FileSystemEntryId.ToString())
                                        msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                        LoggerCheckOutProcess.Error(msg)
                                    End If
                                Catch ex As Exception
                                    LoggerCheckOutProcess.Error(ex.Message, ex)
                                End Try

                            End If

                        Else
                            If (operationType.ToLower() = "c") Then
                                msg = "FileSystemEntryId Not returned, Could Not CheckOut"
                            Else
                                msg = "FileSystemEntryId Not returned, Could Not perform Undo CheckOut"
                            End If

                            LoggerCheckOutProcess.Error(msg)

                        End If

                        If isSuccessful Then
                            RaiseEvent FileCheckedOut(_UserInfo.UserAccountId, _ShareId, FileSystemEntryId, isCheckedOutByDifferentUser)
                        End If

                    Next
                End If

            End If
        Catch ex As Exception
            LoggerCheckOutProcess.Error("Error in CheckOut Process", ex)
        End Try

    End Sub

    Public Function AddFolder(oFileInfo As FileEntryInfo, oFileVersionInfo As FileVersionInfo) As ResultInfo(Of Boolean, Status)

        ' Call Server API AddFolder API
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)
        Dim result As ResultInfo(Of Boolean, Status) = proxy.AddFolder(FileEntryInfo:=oFileInfo, FileVersionsInfo:=oFileVersionInfo)

        If result.Data Then
            ' On Success Checkin the record on the client
            ' Send the version number as 1 as this is the first version which being created
            result = clientproxy.CheckIn(oFileVersionInfo.FileVersionId, 1)
        ElseIf (result.Status = Status.AlreadyExists OrElse result.Status = Status.PathNotValid) Then
            'Delete Folder, Version and Delta Records from Local Client
            Dim delresult = clientproxy.DeleteFile(oFileInfo.FileEntryId)

            If (Not delresult.Data) Then
                result.Message = "Folder already exists on server or Path not valid so cannot add. Failed to delete the folder record on client. Error: " + delresult.Message
                result.Status = delresult.Status
            Else
                result.Message = "Folder already exists on server so cannot add. Filder Record sucessfully deleted on Client"
            End If
        End If

        Return result
    End Function

    Public Function AddAndCheckoutFile(oFileInfo As FileEntryInfo, oFileVersionInfo As FileVersionInfo) As ResultInfo(Of Boolean, Status)

        ' Call Server API AddAndCheckOutFile
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)

        Dim result = proxy.[AddFileAndCheckOut](oFileInfo, oFileVersionInfo, _UserInfo.WorkSpaceId)

        If result.Data Then
            ' On Success CheckOut the file on Client
            result = clientproxy.CheckOut(oFileInfo.FileEntryId, _UserInfo.WorkSpaceId)
        ElseIf (result.Status = Status.AlreadyExists OrElse result.Status = Status.PathNotValid) Then
            'Delete File, Version and Delta Records from Local Client
            Dim delresult = clientproxy.DeleteFile(oFileInfo.FileEntryId)

            If (Not delresult.Data) Then
                result.Message = "File already exists or Path not valid on server so cannot add. Failed to delete the file record on client. Error: " + delresult.Message
                result.Status = delresult.Status
            Else
                result.Message = "File already exists or Path not valid on server so cannot add. File Record sucessfully deleted on Client"
            End If
        End If

        Return result
    End Function

    Public Function AddVersionWithoutUpload(oFileInfo As FileEntryInfo, oFileVersionInfo As FileVersionInfo) As ResultInfo(Of Boolean, Status)

        ' Call Server API AddAndCheckOutFile
        Dim clientResult As ResultInfo(Of Boolean, Status) = Nothing
        Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)

        Dim linkResult = clientproxy.GetFileLinks(oFileInfo.FileEntryId)
        Dim links As List(Of FileEntryLinkInfo) = linkResult.Data

        If (links IsNot Nothing AndAlso links.Count > 0) Then
            Dim proxy As FileClient = New FileClient(_UserInfo)
            Dim result As ResultInfo(Of Integer, Status) = proxy.AddVersionWithoutUpload(oFileInfo, oFileInfo.FileVersion, links)

            If result.Status = Status.Success AndAlso result.Data <> -1 Then
                ' On Success CheckOut the file on Client
                clientResult = clientproxy.CheckIn(oFileVersionInfo.FileVersionId, result.Data)
            ElseIf (result.Status = Status.AlreadyExists OrElse result.Status = Status.PathNotValid) Then
                'Delete File, Version and Delta Records from Local Client
                clientResult = clientproxy.DeleteFile(oFileInfo.FileEntryId)

                If (Not clientResult.Data) Then
                    clientResult.Message = "File already exists or Path not valid on server so cannot add. Failed to delete the file record on client. Error: " + clientResult.Message
                Else
                    clientResult.Message = "File already exists or Path not valid on server so cannot add. File Record sucessfully deleted on Client"
                    clientResult.Data = False
                End If
            Else

                clientResult = New ResultInfo(Of Boolean, Status)
                clientResult.Data = False
                clientResult.Message = result.Message
                clientResult.Status = result.Status
            End If
        Else
            clientResult = New ResultInfo(Of Boolean, Status)
            clientResult.Data = False
            clientResult.Message = "Links not found on Client"
            clientResult.Status = Status.NotFound

        End If
        Return clientResult
    End Function

    Public Function CheckoutFile(fileSystemEntryId As Guid, versionNumber As Integer) As ResultInfo(Of Boolean, Status)

        ' Call server API for Checking out the file on server
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim result = proxy.CheckOut(fileSystemEntryId, versionNumber, _UserInfo.WorkSpaceId)

        If result.Data Then
            ' On Success Checkout the file in client DB
            Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)
            result = clientproxy.CheckOut(fileSystemEntryId, _UserInfo.WorkSpaceId)

        End If

        Return result
    End Function

    Public Function SoftDelete(fileSystemEntryId As Guid, versionNumber As Integer) As ResultInfo(Of Boolean, Status)
        Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)
        Dim result As ResultInfo(Of Boolean, Status) = Nothing

        ' Call server API for SoftDelete the file on server
        Dim proxy As FileClient = New FileClient(_UserInfo)
        result = proxy.SoftDelete(fileSystemEntryId:=fileSystemEntryId, localFileVersionNumber:=versionNumber)

        If result.Data Then
            ' On Success Delete Delta Operations for file
            result = clientproxy.DeleteDeltaOperationsForFile(fileSystemEntryId)
        ElseIf (result.Status = Status.NotFound) Then
            result = clientproxy.DeleteFile(fileSystemEntryId)
            result.Message = String.Format("File Does not exist on server. Trying to delete the file record physically. {0}", arg0:=result.Message)
        End If

        Return result
    End Function

    Public Function UndoDelete(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)

        ' Call server API for SoftDelete the file on server
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim result = proxy.UndoDelete(fileSystemEntryId)

        If result.Data Then
            ' On Success Checkout the file in client DB
            Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)
            result = clientproxy.UndoDelete(fileSystemEntryId)

        End If

        Return result
    End Function

    Public Function AddAndDelete(fileSystemEntryInfo As FileEntryInfo, FileVersionInfo As FileVersionInfo) As ResultInfo(Of Boolean, Status)

        ' Call server API for SoftDelete the file on server
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim result = proxy.AddAndDelete(fileSystemEntryInfo, FileVersionInfo)

        Return result
    End Function

    Public Function UndoCheckoutFile(fileSystemEntryId As Guid) As ResultInfo(Of Boolean, Status)

        ' Call server API for Checking out the file on server
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim result = proxy.UndoCheckOut(fileSystemEntryId:=fileSystemEntryId)

        If result.Data Then
            ' On Success Checkout the file in client DB
            Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)
            result = clientproxy.UndoCheckOut(fileSystemEntryId)

        End If

        Return result
    End Function

    Public Function CheckInFile(oFileVersionInfo As FileVersionInfo, versionNumber As Int32) As ResultInfo(Of Boolean, Status)

        ' Call Server API AddAndCheckOutFile
        Dim proxy As FileClient = New FileClient(_UserInfo)
        Dim result As ResultInfo(Of Integer, Status) = proxy.CheckIn(FileVersionsInfo:=oFileVersionInfo, localFileVersionNumber:=versionNumber)

        Dim clientResult As ResultInfo(Of Boolean, Status) = Nothing

        If result.Status = Status.Success AndAlso result.Data <> -1 Then
            ' On Success CheckOut the file on Client
            Dim clientproxy As LocalFileClient = New LocalFileClient(_UserInfo)
            clientResult = clientproxy.CheckIn(oFileVersionInfo.FileVersionId, result.Data)
        ElseIf (result.Status = Status.AccessDenied) Then
            'Call UndoCheckout API
            clientResult = UndoCheckoutFile(oFileVersionInfo.FileEntryId)

            If Not clientResult.Data Then
                clientResult.Message = "Access Denied on Server, failed while undoing the CheckOut. Error : " + clientResult.Message
            Else
                clientResult.Message = "Access Denied on Server, UndoCheckout Successful"
                clientResult.Status = result.Status
            End If
            clientResult.Data = False

        Else
            clientResult = New ResultInfo(Of Boolean, Status)
            clientResult.Data = False
            clientResult.Message = result.Message
            clientResult.Status = result.Status
        End If

        Return clientResult
    End Function

    Public Function [UpdateFileVersionStatus](fileSystemEntryVersionId As Guid, fileSystemEntryStatus As Enums.FileEntryStatus) As ResultInfo(Of Boolean, Status)

        Dim proxy As LocalFileVersionClient = New LocalFileVersionClient(_UserInfo)
        Return proxy.UpdateFileVersionStatus(fileSystemEntryVersionId, fileSystemEntryStatus)
    End Function

    Private Function GetFilesforCheckOut() As DataTable
        Dim FilesForCheckOut As DataTable = New DataTable()
        Dim recordNo As Int32 = 10

        Using conn As SqlConnection = New SqlConnection(connectionString:=_ConnectionString)
            Using adapter As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                            "SELECT" &
                                                "  [FileSystemEntryId]" &
                                                ", [FileSystemEntryTypeId]" &
                                                ", [ShareId]" &
                                                ", [FileSystemEntryVersionId]" &
                                                ", [ParentFileSystemEntryId]" &
                                                ", [PreviousFileSystemEntryVersionId]" &
                                                ", [FileSystemEntryName]" &
                                                ", [FileSystemEntryExtension]" &
                                                ", [FileSystemEntryHash]" &
                                                ", [FileSystemEntryRelativePath]" &
                                                ", [FileSystemEntrySize]" &
                                                ", [ServerFileSystemEntryName]" &
                                                ", [CreatedByUserId]" &
                                                ", [CreatedOnUTC]" &
                                                ", [VersionNumber]" &
                                                ", [FileSystemEntryStatusId]" &
                                                ", [OperationType]" &
                                                ", [FileSystemEntryVersionConflictId]" &
                                              " FROM [dbo].[GetFileSystemEntryForCheckout](" &
                                              _ShareId.ToString() &
                                              ", " & recordNo.ToString() &
                                              ", " & _UserInfo.UserId.ToString() &
                                              ", '" & _UserInfo.WorkSpaceId.ToString() & "')", selectConnection:=conn)

                Try
                    With adapter
                        .SelectCommand.CommandTimeout = _CommandTimeoutInSeconds
                        .Fill(FilesForCheckOut)
                    End With

                Catch ex As Exception
                    LoggerCheckOutProcess.Error(ex)
                End Try
            End Using
        End Using
        Return FilesForCheckOut
    End Function

    Public Function GetFilesforCheckIn() As DataTable
        Dim FilesForCheckIn As DataTable = New DataTable()
        Dim recordNo As Int32 = 10
        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=_ConnectionString)
            If (SqlConnectionClient.State <> ConnectionState.Open) Then
                SqlConnectionClient.Open()
            End If
            Using sqlCommand As SqlCommand = New SqlCommand("UPDATE D " &
                                                            " SET D.FileSystemEntryStatusId = 0" &
                                                            " FROM DeltaOperations D" &
                                                            " JOIN FileSystemEntryVersions V ON D.FileSystemEntryVersionId = V.FileSystemEntryVersionId" &
                                                            " WHERE D.FileSystemEntryStatusId IN (5,6,7,8)" &
                                                            " AND V.VersionNumber > 0", SqlConnectionClient)
                sqlCommand.ExecuteNonQuery()
            End Using
        End Using

        Using conn As SqlConnection = New SqlConnection(connectionString:=_ConnectionString)
            Using adapter As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                            "SELECT" &
                                                "  [FileSystemEntryId]" &
                                                ", [FileSystemEntryTypeId]" &
                                                ", [ShareId]" &
                                                ", [FileSystemEntryVersionId]" &
                                                ", [ParentFileSystemEntryId]" &
                                                ", [PreviousFileSystemEntryVersionId]" &
                                                ", [FileSystemEntryName]" &
                                                ", [FileSystemEntryExtension]" &
                                                ", [FileSystemEntryHash]" &
                                                ", [FileSystemEntryRelativePath]" &
                                                ", [FileSystemEntrySize]" &
                                                ", [ServerFileSystemEntryName]" &
                                                ", [CreatedByUserId]" &
                                                ", [CreatedOnUTC]" &
                                                ", [VersionNumber]" &
                                                ", [FileSystemEntryStatusId]" &
                                              " FROM [dbo].[GetFileSystemEntryVersionForCheckIn](" &
                                              _ShareId.ToString() & ", " & _UserInfo.UserId.ToString() & ", " &
                                              recordNo.ToString() & ")", selectConnection:=conn)

                Try
                    With adapter
                        .SelectCommand.CommandTimeout = _CommandTimeoutInSeconds
                        .Fill(dataTable:=FilesForCheckIn)
                    End With

                Catch ex As Exception
                    LoggerCheckInProcess.Error(ex)
                End Try
            End Using
        End Using
        Return FilesForCheckIn
    End Function

    Public Function GetFilesforUpload() As DataTable
        Dim FilesForUpload As DataTable = New DataTable()
        Dim recordNo As Int32 = 10
        Dim timeOffset As Int32 = Integer.Parse(ConfigurationManager.AppSettings(name:="CheckInOffsetTimeInSecs"))
        Dim OffsetDateTime As String = DateTime.UtcNow().AddSeconds(-1 * timeOffset).ToString("yyyy/MM/dd HH:mm:ss")

        Using conn As SqlConnection = New SqlConnection(connectionString:=_ConnectionString)
            Using adapter As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                            "SELECT" &
                                                "  [FileSystemEntryVersionId]" &
                                                ", [FileSystemEntryRelativePath]" &
                                                ", [ServerFileSystemEntryName]" &
                                                ", [FileSystemEntryStatusId]" &
                                                ", [FileSystemEntryId]" &
                                              " FROM [dbo].[GetFileSystemEntryVersionForUpload](" &
                                              _ShareId.ToString() & ", '" & OffsetDateTime & "', " & _UserInfo.UserId.ToString() &
                                              ", " & recordNo.ToString() & ")", selectConnection:=conn)

                Try
                    With adapter
                        .SelectCommand.CommandTimeout = _CommandTimeoutInSeconds
                        .Fill(dataTable:=FilesForUpload)
                    End With

                Catch ex As Exception
                    LoggerFileSync.Error(ex)
                End Try
            End Using
        End Using
        Return FilesForUpload
    End Function


    Public Function MapFileSystemEntryInfo(dr As DataRow) As FileEntryInfo
        Dim obj As FileEntryInfo = New FileEntryInfo()

        obj.FileEntryId = Guid.Parse(dr(columnName:="FileSystemEntryId").ToString())
        obj.FileEntryTypeId = Convert.ToByte(dr(columnName:="FileSystemEntryTypeId").ToString())
        obj.FileShareId = _ShareId
        obj.IsDeleted = 0
        obj.IsPermanentlyDeleted = 0

        obj.FileVersion = New FileVersionInfo()
        obj.FileVersion.FileEntryId = obj.FileEntryId
        obj.FileVersion.FileVersionId = Guid.Parse(dr(columnName:="FileSystemEntryVersionId").ToString())
        obj.FileVersion.ParentFileEntryId = Guid.Parse(dr(columnName:="ParentFileSystemEntryId").ToString())
        obj.FileVersion.FileEntryName = dr(columnName:="FileSystemEntryName").ToString()
        obj.FileVersion.FileEntryExtension = dr(columnName:="FileSystemEntryExtension").ToString()
        obj.FileVersion.FileEntryRelativePath = dr(columnName:="FileSystemEntryRelativePath").ToString()
        obj.FileVersion.FileEntryHash = dr(columnName:="FileSystemEntryHash").ToString()
        obj.FileVersion.FileEntrySize = Convert.ToInt64(dr(columnName:="FileSystemEntrySize").ToString())

        Dim serverFileNameStr = dr(columnName:="ServerFileSystemEntryName").ToString()

        If (Not String.IsNullOrEmpty(serverFileNameStr)) Then
            obj.FileVersion.ServerFileName = Guid.Parse(serverFileNameStr)
        End If

        Dim previousFileSystemEntryVersionIdstr = dr(columnName:="PreviousFileSystemEntryVersionId").ToString()

        If (Not String.IsNullOrEmpty(previousFileSystemEntryVersionIdstr)) Then
            obj.FileVersion.PreviousFileVersionId = Guid.Parse(previousFileSystemEntryVersionIdstr)
        End If

        obj.FileVersion.CreatedOnUTC = Convert.ToDateTime(dr(columnName:="CreatedOnUTC").ToString())

        Return obj
    End Function

End Class
