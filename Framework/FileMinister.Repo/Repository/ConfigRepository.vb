Imports risersoft.shared.portable.Model
Imports Microsoft.WindowsAzure.Storage.Blob
Imports Microsoft.WindowsAzure.Storage
Imports risersoft.shared.portable.Enums
Imports Microsoft.WindowsAzure.Storage.File

''' <summary>
''' Config Repository
''' </summary>
''' <remarks></remarks>
Public Class ConfigRepository
    Inherits ServerRepositoryBase(Of ConfigInfo, Short)
    Implements IConfigRepository

    ''' <summary>
    ''' Get All FileShares Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAll() As ResultInfo(Of List(Of ConfigInfo), Status)
        Try
            Using service = GetServerEntity()
                Dim result = service.FileShares.Where(Function(p) p.IsDeleted = False).Select(Function(s) New ConfigInfo With {
                    .FileShareId = s.FileShareId,
                    .ShareName = s.ShareName,
                    .SharePath = s.ShareContainerName
                 }).ToList()
                Return BuildResponse(result)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of ConfigInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get FileShare Details
    ''' </summary>
    ''' <param name="id">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function [Get](id As Short) As ResultInfo(Of ConfigInfo, Status)
        Try
            Using service = GetServerEntity()
                Dim FileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = id)
                Dim obj = New ConfigInfo
                If FileShare IsNot Nothing Then
                    obj = MapToObject(FileShare)
                End If
                Return BuildResponse(obj)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of ConfigInfo)(ex)
        End Try

    End Function

    ''' <summary>
    ''' Add FileShare
    ''' </summary>
    ''' <param name="data">FileShare Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Add(data As ConfigInfo) As ResultInfo(Of Boolean, Status)
        If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin) Then
            Try
                Using service = GetServerEntity()
                    'check for duplicate FileShare name
                    Dim isShareExist = service.FileShares.Where(Function(p) p.ShareName = data.ShareName).Any()
                    If (Not isShareExist) Then

                        Dim shareContainerConnectionString = Me.User.Account.StorageAccount

                        If String.IsNullOrWhiteSpace(shareContainerConnectionString) Then
                            Throw New ArgumentNullException("FileShare container connection string not found")
                        End If

                        Dim containerName = Guid.NewGuid().ToString().ToLower()

                        Util.Helper.CreateAzureStorageContainer(shareContainerConnectionString, containerName)

                        Dim FileShare = MapFromObject(data)

                        FileShare.ShareContainerName = containerName
                        FileShare.CreatedByUserId = Me.User.UserId
                        FileShare.CreatedOnUTC = DateTime.UtcNow

                        ''file
                        Dim fileObj = New FileEntry()
                        fileObj.FileEntryId = Guid.NewGuid()
                        fileObj.FileEntryTypeId = FileType.Share
                        fileObj.CurrentVersionNumber = 0
                        fileObj.IsCheckedOut = 0
                        fileObj.IsDeleted = 0
                        FileShare.FileEntries.Add(fileObj)

                        'FileAgent FileShare
                        For Each FileAgentShare As FileAgentShareInfo In data.AgentShares
                            Dim oAgentShareInfo = New FileAgentShare()
                            oAgentShareInfo.FileAgentId = FileAgentShare.FileAgentId
                            oAgentShareInfo.CreatedByUserId = Me.User.UserId
                            oAgentShareInfo.CreatedOnUTC = DateTime.UtcNow
                            FileShare.FileAgentShares.Add(oAgentShareInfo)
                        Next


                        service.FileShares.Add(FileShare)
                        service.SaveChanges()
                        Return BuildResponse(True)
                    Else
                        Return BuildResponse(False, Status.AlreadyExists, "FileShare name already exists") 'duplcate FileShare name
                    End If
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        Else
            Return BuildResponse(False, Status.AccessDenied)
        End If
    End Function

    ''' <summary>
    ''' Update FileShare
    ''' </summary>
    ''' <param name="id">FileShareId</param>
    ''' <param name="data">FileShare Details</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update(id As Short, data As ConfigInfo) As ResultInfo(Of Boolean, Status)
        If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin) Then
            Try
                Using service = GetServerEntity()
                    Dim FileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = id)

                    If FileShare IsNot Nothing Then
                        MapFromObject(data, FileShare)
                        Dim agentArr = data.AgentShares.Where(Function(p) p.FileAgentShareId <> Nothing).Select(Function(p) p.FileAgentId).ToList()

                        Dim missingShareData = service.FileAgentShares.Where(Function(p) Not agentArr.Contains(p.FileAgentId) And p.FileShareId = data.FileShareId And p.IsDeleted = False)
                        For Each ama In missingShareData
                            ama.IsDeleted = True
                            ama.DeletedByUserId = Me.User.UserId
                            ama.DeletedOnUTC = DateTime.UtcNow
                        Next

                        For Each newShare In data.AgentShares.Where(Function(p) p.FileAgentShareId = 0)
                            Dim oAgentShare = New FileAgentShare()
                            oAgentShare.FileAgentId = newShare.FileAgentId
                            oAgentShare.FileShareId = newShare.FileShareId
                            oAgentShare.CreatedByUserId = Me.User.UserId
                            oAgentShare.CreatedOnUTC = DateTime.UtcNow
                            oAgentShare.IsDeleted = False
                            FileShare.FileAgentShares.Add(oAgentShare)
                        Next
                        service.SaveChanges()
                    End If

                    Return BuildResponse(True)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        Else
            Return BuildResponse(False, Status.AccessDenied)
        End If
    End Function

    ''' <summary>
    ''' Delete FileShare
    ''' </summary>
    ''' <param name="id">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Delete(id As Short) As ResultInfo(Of Boolean, Status)
        If (Me.User.UserAccount.UserTypeId = Role.AccountAdmin) Then
            Try
                Using service = GetServerEntity()
                    Dim FileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = id)
                    If FileShare IsNot Nothing Then
                        FileShare.IsDeleted = True
                        FileShare.DeletedByUserId = Me.User.UserId
                        FileShare.DeletedOnUTC = DateTime.UtcNow()

                        For Each item In FileShare.FileAgentShares
                            item.IsDeleted = True
                            item.DeletedByUserId = Me.User.UserId
                            item.DeletedOnUTC = DateTime.UtcNow()

                        Next

                        service.SaveChanges()
                    End If
                    Return BuildResponse(True)
                End Using
            Catch ex As Exception
                Return BuildExceptionResponse(Of Boolean)(ex)
            End Try
        Else
            Return BuildResponse(False, Status.AccessDenied)
        End If
    End Function

    ''' <summary>
    ''' Map FileShare Details to FileShare Model
    ''' </summary>
    ''' <param name="s">FileShare Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapToObject(s As FileShare) As ConfigInfo
        Dim t = New ConfigInfo With {
               .FileShareId = s.FileShareId,
                .ShareName = s.ShareName,
                .SharePath = s.ShareContainerName
             }

        If (s.FileAgentShares IsNot Nothing And s.FileAgentShares.Count > 0) Then
            For Each item In s.FileAgentShares
                If (item.IsDeleted = False) Then
                    Dim a As FileAgentShareInfo = New FileAgentShareInfo()
                    a.FileAgentShareId = item.FileAgentShareId
                    a.FileShareId = item.FileShareId
                    a.ShareName = item.FileShare.ShareName
                    a.FileAgentId = item.FileAgentId
                    t.AgentShares.Add(a)
                End If
            Next
        End If

        Return t
    End Function

    ''' <summary>
    ''' Map FileShare Model to FileShare Entity
    ''' </summary>
    ''' <param name="s">FileShare Model</param>
    ''' <param name="t">FileShare Entity</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function MapFromObject(s As ConfigInfo, Optional t As FileShare = Nothing) As FileShare
        Dim tt = t
        If tt Is Nothing Then
            tt = New FileShare()
        End If
        '   tt.FileShareId = s.FileShareId
        If s.ShareName <> Nothing Then
            tt.ShareName = s.ShareName
        End If
        If s.SharePath <> Nothing Then
            tt.ShareContainerName = s.SharePath
        End If

        If s.CreatedOnUTC <> DateTime.MinValue Then
            tt.CreatedOnUTC = DateTime.UtcNow
        End If

        If s.CreatedByUserId.Equals(Guid.Empty) Then
            tt.CreatedByUserId = Me.User.UserId
        End If
        tt.IsDeleted = s.IsDeleted

        Return tt
    End Function

    ''' <summary>
    ''' Get All FileShare FileAgent Map Details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [ShareMapDetails]() As ResultInfo(Of List(Of AgentShareMappingInfo), Status) Implements IConfigRepository.ShareMapDetails

        Try
            Using service = GetServerEntity()
                Dim agentShareAggregate = New List(Of AgentShareMappingInfo)

                Dim agentShares = New List(Of AgentShareMappingInfo)
                Dim data = service.FileShares.Where(Function(p) p.IsDeleted = False).ToList()
                For Each dt In data
                    Dim cnt = dt.FileAgentShares.Where(Function(p) p.IsDeleted = False).Count()
                    agentShares.Add(New AgentShareMappingInfo With {.AgentCount = cnt, .FileShareId = dt.FileShareId, .ShareName = dt.ShareName})
                Next

                Return BuildResponse(agentShares)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of List(Of AgentShareMappingInfo))(ex)
        End Try
    End Function

    ''' <summary>
    ''' Update FileShare Size
    ''' </summary>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateShareBlobSize(FileShareId As Short) As ResultInfo(Of Boolean, Status) Implements IConfigRepository.UpdateShareBlobSize
        Try
            'Get FileShare Blob size
            Dim getresult = GetBlobSizeForShare(FileShareId)

            Dim getFileSize = GetFileSizeForShare(FileShareId)

            If (getresult.Status = Status.Success) Then
                Using service = GetServerEntity()
                    Dim FileShare = service.FileShares.FirstOrDefault(Function(p) p.FileShareId = FileShareId)
                    FileShare.DiskSpaceMB = Util.Helper.ConvertBytesToMb(getresult.Data + getFileSize.Data)

                    service.SaveChanges()

                    Return BuildResponse(True, Status.Success, ServiceConstants.OPERATION_SUCCESSFUL)
                End Using
            Else
                Return BuildResponse(False, Status.None, "Unable to get FileShare blob size")
            End If
        Catch ex As Exception
            Return BuildExceptionResponse(Of Boolean)(ex)
        End Try

    End Function

    ''' <summary>
    ''' Get FileShare Size
    ''' </summary>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBlobSizeForShare(FileShareId As Short) As ResultInfo(Of Long, Status) Implements IConfigRepository.GetBlobSizeForShare
        Try
            Using Service = GetServerEntity()
                Dim Size As ULong = 0
                Dim FileShare = Service.FileShares.FirstOrDefault(Function(p) p.FileShareId = FileShareId)

                Dim CloudBlobClient As CloudBlobClient = CloudStorageAccount.Parse(Me.User.Account.StorageAccount).CreateCloudBlobClient()
                Dim CloudBlobContainer As CloudBlobContainer = CloudBlobClient.GetContainerReference(containerName:=FileShare.ShareContainerName)

                If CloudBlobContainer.Exists() Then

                    Size = (From blob As CloudBlockBlob In CloudBlobContainer.ListBlobs(useFlatBlobListing:=True, blobListingDetails:=BlobListingDetails.All)
                            Select blob.Properties.Length).Sum()
                End If

                Return BuildResponse(Of Long)(Size)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Long)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get FileShare Size from Azure File
    ''' </summary>
    ''' <param name="FileShareId">FileShareId</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFileSizeForShare(FileShareId As Short) As ResultInfo(Of Long, Status) Implements IConfigRepository.GetFileSizeForShare
        Try
            Using Service = GetServerEntity()
                Dim size As ULong = 0
                Dim FileShare = Service.FileShares.FirstOrDefault(Function(p) p.FileShareId = FileShareId)

                Dim cloudFileClient As File.CloudFileClient = CloudStorageAccount.Parse(Me.User.Account.StorageAccount).CreateCloudFileClient()
                Dim cloudFileShare As File.CloudFileShare = cloudFileClient.GetShareReference(shareName:=FileShare.ShareContainerName)

                If cloudFileShare.Exists() Then
                    'GetStats will give value in GB, but we need to convert to bytes
                    'size = cloudFileShare.GetStats().Usage * 1024 * 1024
                    Dim rootDir = cloudFileShare.GetRootDirectoryReference()
                    FileShareByteCount(rootDir, size)

                End If

                Return BuildResponse(Of Long)(size)
            End Using
        Catch ex As Exception
            Return BuildExceptionResponse(Of Long)(ex)
        End Try
    End Function

    ''' <summary>
    ''' Get the FileShare Byte count from Azure Folder/Files
    ''' </summary>
    ''' <param name="dir">Cloud File Directory</param>
    ''' <param name="bytesCount">return byte counts</param>
    Private Sub FileShareByteCount(ByVal dir As CloudFileDirectory, ByRef bytesCount As Long)
        Dim continuationToken As FileContinuationToken = Nothing
        Dim resultSegment As FileResultSegment = Nothing
        Do
            resultSegment = dir.ListFilesAndDirectoriesSegmented(100, continuationToken, Nothing, Nothing)
            If resultSegment.Results.Count() > 0 Then
                For Each item In resultSegment.Results

                    If item.[GetType]() = GetType(CloudFileDirectory) Then
                        Dim cloudFileDirectory = TryCast(item, CloudFileDirectory)
                        FileShareByteCount(cloudFileDirectory, bytesCount)
                    ElseIf item.[GetType]() = GetType(CloudFile) Then
                        Dim cloudFile = TryCast(item, CloudFile)
                        bytesCount += cloudFile.Properties.Length
                    End If

                Next
            End If
        Loop While continuationToken IsNot Nothing
    End Sub


End Class
