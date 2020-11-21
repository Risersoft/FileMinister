Imports System.Configuration
Imports log4net
Imports System.ComponentModel
Imports System.Timers
Imports System.Data.SqlClient
Imports System.IO
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Model
Imports Microsoft.WindowsAzure.Storage.Blob
Imports System.Security.Cryptography
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.cloud

Public Class FileSyncModule
    Private Shared ReadOnly LoggerFileSync As ILog = LogManager.GetLogger(name:="FileSyncLogger")
    Private ReadOnly TimerFileSyncDownload As Timer
    Private Shared ReadOnly FileSyncDownloadServiceIntervalInMilliSeconds As Double = Double.Parse(ConfigurationManager.AppSettings(name:="FileSyncDownloadServiceIntervalInMilliSeconds"))
    Private Shared ReadOnly FileSyncCheckInServiceIntervalInMilliSeconds As Double = Double.Parse(ConfigurationManager.AppSettings(name:="FileSyncCheckInServiceIntervalInMilliSeconds"))
    Private Shared ReadOnly FileSyncCheckOutServiceIntervalInMilliSeconds As Double = Double.Parse(ConfigurationManager.AppSettings(name:="FileSyncCheckOutServiceIntervalInMilliSeconds"))
    Private Shared ReadOnly FileSyncUploadServiceIntervalInMilliSeconds As Double = Double.Parse(ConfigurationManager.AppSettings(name:="FileSyncUploadServiceIntervalInMilliSeconds"))
    Private Shared ReadOnly FileSyncConflictUploadServiceIntervalInMilliSeconds As Double = Double.Parse(ConfigurationManager.AppSettings(name:="FileSyncConflictUploadServiceIntervalInMilliSeconds"))

    Private Shared ReadOnly CommandTimeoutInSeconds As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="CommandTimeoutInSeconds"))
    Private ReadOnly TimerFileSyncCheckIn As Timer
    Private ReadOnly TimerFileSyncCheckOut As Timer
    Private ReadOnly TimerFileSyncUpload As Timer
    Private ReadOnly TimerFileSyncConflictUpload As Timer
    Private Shared ReadOnly LoggerCheckInProcess As ILog = LogManager.GetLogger(name:="CheckInProcessLogger")
    Private Shared fileSyncModule As FileSyncModule

    Shared Sub New()
        fileSyncModule = New FileSyncModule()
    End Sub

    Private Sub New()
        Me.TimerFileSyncDownload = New Timer(interval:=FileSyncDownloadServiceIntervalInMilliSeconds)
        With Me.TimerFileSyncDownload
            .AutoReset = False
            .Enabled = False
            AddHandler .Elapsed, AddressOf Me.TimerFileSyncDownload_Elapsed
        End With
        Me.TimerFileSyncCheckIn = New Timer(interval:=FileSyncCheckInServiceIntervalInMilliSeconds)
        With Me.TimerFileSyncCheckIn
            .AutoReset = False
            .Enabled = False
            AddHandler .Elapsed, AddressOf Me.TimerFileSyncCheckIn_Elapsed
        End With

        Me.TimerFileSyncCheckOut = New Timer(interval:=FileSyncCheckOutServiceIntervalInMilliSeconds)
        With Me.TimerFileSyncCheckOut
            .AutoReset = False
            .Enabled = False
            AddHandler .Elapsed, AddressOf Me.TimerFileSyncCheckOut_Elapsed
        End With

        Me.TimerFileSyncUpload = New Timer(interval:=FileSyncUploadServiceIntervalInMilliSeconds)
        With Me.TimerFileSyncUpload
            .AutoReset = False
            .Enabled = False
            AddHandler .Elapsed, AddressOf Me.TimerFileSyncUpload_Elapsed
        End With

        Me.TimerFileSyncConflictUpload = New Timer(interval:=FileSyncConflictUploadServiceIntervalInMilliSeconds)
        With Me.TimerFileSyncConflictUpload
            .AutoReset = False
            .Enabled = False
            AddHandler .Elapsed, AddressOf Me.TimerFileSyncConflictUpload_Elapsed
        End With

    End Sub

    Public Shared Function GetInstance() As FileSyncModule
        Return fileSyncModule
    End Function

    Public Sub StartService()
        Me.TimerFileSyncDownload.Start()

        'Me.TimerFileSyncMoveDownloaded.Start()

        Me.TimerFileSyncCheckIn.Start()

        Me.TimerFileSyncCheckOut.Start()

        Me.TimerFileSyncUpload.Start()

        Me.TimerFileSyncConflictUpload.Start()
    End Sub

    Public Sub StopService()

        Me.TimerFileSyncDownload.Stop()

        'Me.TimerFileSyncMoveDownloaded.Stop()

        Me.TimerFileSyncCheckIn.Stop()

        Me.TimerFileSyncCheckOut.Stop()

        Me.TimerFileSyncUpload.Stop()

        Me.TimerFileSyncConflictUpload.Stop()

    End Sub

    Public Function isEnabled() As Boolean
        Return Me.TimerFileSyncDownload.Enabled OrElse Me.TimerFileSyncCheckIn.Enabled OrElse Me.TimerFileSyncCheckOut.Enabled OrElse Me.TimerFileSyncUpload.Enabled OrElse Me.TimerFileSyncConflictUpload.Enabled
    End Function

    Public Sub StartOnDemandSync(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
        Me.TimerFileSyncDownload_Elapsed(Sender:=Nothing, ElapsedEventArgs:=Nothing, OnDemandSyncFileSystemEntryId:=FileSystemEntryId)
        Me.TimerFileSyncUpload_Elapsed(Sender:=Nothing, ElapsedEventArgs:=Nothing, OnDemandSyncFileSystemEntryId:=FileSystemEntryId)

    End Sub

#Region "File Sync"
    Public Event FileCheckedOut(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid, IsCheckedOutByDifferentUser As Boolean)
    Public Event FileDownloaded(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
    Public Event FileDownloadFinishedForShare(UserAccountId As Integer, ShareId As Integer)
    Public Event FileMoveDownloadedFinishedForShare(UserAccountId As Integer, ShareId As Integer)
    Public Event FileUploaded(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
    Public Event FileUploadFinishedForShare(UserAccountId As Integer, ShareId As Integer)

    Private Shared ReadOnly FileSyncMaximumConcurrentFileDownloadBatchSize As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="FileSyncMaximumConcurrentFileDownloadBatchSize"))

    Private Shared ReadOnly MetadataKeyIsUploadFinished As String = ConfigurationManager.AppSettings(name:="MetadataKeyIsUploadFinished")
    Private Shared ReadOnly MaximumPutBlockRetryCount As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="MaximumPutBlockRetryCount"))
    Private Shared ReadOnly DownloadBlockSizeInBytes As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="DownloadBlockSizeInBytes"))
    Private Shared ReadOnly UploadBlockSizeInBytes As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="UploadBlockSizeInBytes"))
    Private ReadOnly BackgroundWorkersFileSyncDownload As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)
    'Private ReadOnly BackgroundWorkersFileSyncMoveDownloaded As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)
    Private ReadOnly BackgroundWorkersFileSyncCheckIn As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)
    Private ReadOnly BackgroundWorkersFileSyncCheckOut As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)
    Private ReadOnly BackgroundWorkersFileSyncUpload As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)
    Private ReadOnly BackgroundWorkersFileSyncConflictUpload As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)

    Private ReadOnly TasksFileSyncDownload As Dictionary(Of KeyValuePair(Of Guid, Integer), Task) = New Dictionary(Of KeyValuePair(Of Guid, Integer), Task)
    Private ReadOnly TasksFileSyncUpload As Dictionary(Of KeyValuePair(Of Guid, Integer), Task) = New Dictionary(Of KeyValuePair(Of Guid, Integer), Task)

    Private Sub TimerFileSyncDownload_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs, Optional OnDemandSyncFileSystemEntryId As Guid? = Nothing)
        Dim DataTableUserAccounts As DataTable = Nothing
        Dim DataTableUserShares As DataTable = Nothing

        FileMinisterService.GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerFileSync)

        If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
            If DataTableUserAccounts.Rows.Count = 0 Then
                LoggerFileSync.Debug("Unable to find last logged-in User.")
            Else
                If DataTableUserShares.Rows.Count = 0 Then
                    LoggerFileSync.Debug("Unable to find any mapped share.")
                Else
                    For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
                        Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

                        If (DataRowsUserShares.Length > 0) Then
                            For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
                                If DataRowsUserSharesByUserId.Count() > 0 Then
                                    For Each DataRow As DataRow In DataRowsUserSharesByUserId
                                        Dim BackgroundWorkerFileSyncDownloadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=New Guid(DataRowsUserSharesByUserId.Key.ToString), ShareId:=CInt(DataRow(columnName:="ShareId")), OnDemandSyncFileSystemEntryId:=OnDemandSyncFileSystemEntryId)

                                        SyncLock Me.BackgroundWorkersFileSyncDownload
                                            If Not Me.BackgroundWorkersFileSyncDownload.ContainsKey(key:=BackgroundWorkerFileSyncDownloadKey) Then
                                                Dim BackgroundWorkerFileSyncDownload As BackgroundWorker = New BackgroundWorker()

                                                With BackgroundWorkerFileSyncDownload
                                                    AddHandler .DoWork, AddressOf Me.BackgroundWorkerFileSyncDownload_DoWork
                                                    AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerFileSyncDownload_RunWorkerCompleted
                                                End With

                                                Me.BackgroundWorkersFileSyncDownload.Add(key:=BackgroundWorkerFileSyncDownloadKey, value:=BackgroundWorkerFileSyncDownload)

                                                BackgroundWorkerFileSyncDownload.RunWorkerAsync(argument:=New FileBackgroundWorkerArguments With {
                                                                                        .IsRunInServiceMode = ((Not Sender Is Nothing) AndAlso OnDemandSyncFileSystemEntryId Is Nothing),
                                                                                        .UserAccountId = CInt(DataRowUserAccounts(columnName:="UserAccountId")),
                                                                                        .AccountId = New Guid(DataRowUserAccounts(columnName:="AccountId").ToString),
                                                                                        .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
                                                                                        .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
                                                                                        .UserId = New Guid(DataRowsUserSharesByUserId.Key.ToString),
                                                                                        .ShareId = CInt(DataRow(columnName:="ShareId")),
                                                                                        .SharePath = DataRow(columnName:="SharePath").ToString(),
                                                                                        .WindowsUser = If(DataRow.IsNull("WindowsUser"), Nothing, DataRow(columnName:="WindowsUser").ToString()),
                                                                                        .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1)),
                                                                                        .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0)),
                                                                                        .Password = CommonUtils.Helper.Decrypt(If(DataRow.IsNull("Password"), Nothing, DataRow(columnName:="Password").ToString())),
                                                                                        .OnDemandSyncFileSystemEntryId = OnDemandSyncFileSystemEntryId,
                                                                                        .RoleId = CInt(DataRowUserAccounts(columnName:="RoleId")),
                                                                                        .CloudSyncServiceURL = DataRowUserAccounts(columnName:="CloudSyncServiceURL").ToString()
                                                                                    })
                                            End If
                                        End SyncLock
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        If Not Sender Is Nothing Then
            Me.TimerFileSyncDownload.Start()
        ElseIf Not OnDemandSyncFileSystemEntryId Is Nothing Then
            While True
                Dim BackgroundWorkersOnDemandFileSyncDownloadCount As Integer = 0

                SyncLock Me.BackgroundWorkersFileSyncDownload
                    BackgroundWorkersOnDemandFileSyncDownloadCount = Me.BackgroundWorkersFileSyncDownload.Where(Function(x) x.Key.EndsWith(String.Format(format:=", OnDemandSyncFileSystemEntryId={0}", arg0:=OnDemandSyncFileSystemEntryId.Value.ToString()))).Count()
                End SyncLock

                If BackgroundWorkersOnDemandFileSyncDownloadCount > 0 Then
                    Threading.Thread.Sleep(millisecondsTimeout:=CInt(FileSyncDownloadServiceIntervalInMilliSeconds))
                Else
                    Exit While
                End If
            End While
        End If
    End Sub

    Private Sub BackgroundWorkerFileSyncDownload_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim FileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim BackgroundWorkerFileSyncDownloadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, UserId:=FileBackgroundWorkerArguments.UserId, ShareId:=FileBackgroundWorkerArguments.ShareId, OnDemandSyncFileSystemEntryId:=FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId)

        LoggerFileSync.Info(String.Format(format:="Starting Sync for '{0}'...", arg0:=BackgroundWorkerFileSyncDownloadKey))

        Dim DataTableFileSystemEntryVersionsWithEffectiveUserPermissions As DataTable = Nothing

        'ASHISH: Files that can be downloaded:
        '1. Latest File Version of non server deleted (!File.IsDeleted) if no local modified file
        '2. If local modifed file or (local deleted but not server deleted): conflict case
        '3. in case of admin all the conflicts
        'all others insert delta.status =0 for null delta records

        '1. File of prev version should not be ask to be downloaded. create delta record of status 0 for them.
        '2. in case of conflict, save file as ## and client modified record status should change to conflict
        '3. We need to reset the status flag for downloading/uploading files, if they are not reqd to be uploaded/downloaded now.

        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
            Using DataSetClient As DataSet = New DataSet()
                Using SqlDataAdapterClient As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                               "DELETE D FROM [DeltaOperations] D JOIN FileSystemEntryVersions FV ON D.FileSystemEntryVersionId=FV.FileSystemEntryVersionId WHERE VersionNumber=1 AND [FileSystemEntryStatusId] = " & FileEntryStatus.TempCreated & " AND IsConflicted=0;" &
                                                               "" &
                                                               " SELECT" &
                                                               "    [FileSystemEntries].[FileSystemEntryTypeId]" &
                                                               "    , [FileSystemEntries].[CurrentVersionNumber]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryVersionId]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                               "    , [FileSystemEntryVersions].[VersionNumber]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryRelativePath] AS [FileSystemEntryPathRelativeToShare]" &
                                                               "    , [FileSystemEntryVersions].[ServerFileSystemEntryName]" &
                                                               "    , [DeltaOperations].[DeltaOperationId]" &
                                                               "    , [DeltaOperations].[FileSystemEntryStatusId]" &
                                                               "    , [DeltaOperations].[IsConflicted]" &
                                                               "    , [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryVersions].[FileSystemEntryId]) AS [EffectiveUserPermissions]" &
                                                               "    , [dbo].[GetLatestFileSystemEntryVersionId]([FileSystemEntryVersions].[FileSystemEntryId]) AS [LatestFileSystemEntryVersionIdOfFileSystemEntry]" &
                                                               " FROM" &
                                                               "    [FileSystemEntryVersions]" &
                                                               "    INNER JOIN [FileSystemEntries] ON [FileSystemEntries].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                               "    LEFT OUTER JOIN [DeltaOperations] ON /* [DeltaOperations].[IsDeleted] = 0 AND */ [DeltaOperations].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId] AND [DeltaOperations].[FileSystemEntryVersionId] = [FileSystemEntryVersions].[FileSystemEntryVersionId]" &
                                                               "    LEFT OUTER JOIN [RelativePathsExcludedFromSync] E ON E.ShareId = [FileSystemEntries].ShareId AND FileSystemEntryVersions.[FileSystemEntryRelativePath] LIKE CONCAT(E.RelativePath,'%')" &
                                                               " WHERE" &
                                                               "    [FileSystemEntries].[ShareId] = " & FileBackgroundWorkerArguments.ShareId.ToString() &
                                                               "    AND E.ShareId IS NULL" &
                                                               "    AND [FileSystemEntries].[IsDeleted] = 0 " &
                                                               "    AND [FileSystemEntryVersions].[IsDeleted] = 0 " &
                                                               If(FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId Is Nothing, String.Empty,
                                                               "    AND (" &
                                                               "        (" &
                                                               "            (SELECT [FileSystemEntryTypeId] FROM [FileSystemEntries] WHERE [FileSystemEntryId] = @FileSystemEntryId) = 3" &
                                                               "            AND [FileSystemEntries].[FileSystemEntryId] = @FileSystemEntryId" &
                                                               "        )" &
                                                               "        OR (" &
                                                               "            (SELECT [FileSystemEntryTypeId] FROM [FileSystemEntries] WHERE [FileSystemEntryId] = @FileSystemEntryId) <> 3" &
                                                               "            AND [FileSystemEntries].[FileSystemEntryId] IN (SELECT [FileSystemEntryId] FROM [dbo].[GetLatestFileSystemEntryVersionsChildrenHierarchy](@FileSystemEntryId))" &
                                                               "        )" &
                                                               "    )").ToString() &
                                                               "    AND LTRIM(RTRIM([FileSystemEntryVersions].[FileSystemEntryRelativePath]))<>''" &
                                                               "    AND (" &
                                                               "        [DeltaOperations].[DeltaOperationId] IS NULL" &
                                                               "        OR (" &
                                                               "            [DeltaOperations].[FileSystemEntryStatusId] <> " & FileEntryStatus.NoActionRequired &
                                                               "            AND [DeltaOperations].[FileSystemEntryStatusId] <> " & FileEntryStatus.NoDownloadRequired &
                                                               "        )" &
                                                               "    )" &
                                                               "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryVersions].[FileSystemEntryId]) > 0" &
                                                               "    AND [FileSystemEntryVersions].[VersionNumber] IS NOT NULL" &
                                                               " ORDER BY" &
                                                               "    [FileSystemEntryPathRelativeToShare]", selectConnection:=SqlConnectionClient)
                    Try
                        With SqlDataAdapterClient
                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds

                            If Not FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId Is Nothing Then
                                .SelectCommand.Parameters.Add(value:=New SqlParameter(parameterName:="@FileSystemEntryId", dbType:=SqlDbType.UniqueIdentifier))
                                .SelectCommand.Parameters(0).Value = FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId
                            End If

                            .Fill(dataSet:=DataSetClient)
                        End With

                        DataTableFileSystemEntryVersionsWithEffectiveUserPermissions = DataSetClient.Tables(0)
                    Catch Exception As Exception
                        LoggerFileSync.Error("Unable to connect to client database.", Exception)
                    End Try
                End Using
            End Using
        End Using

        If (Not DataTableFileSystemEntryVersionsWithEffectiveUserPermissions Is Nothing) AndAlso DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Rows.Count > 0 Then
            'START - Directory Sync
            'Me.DirectorySync(FileBackgroundWorkerArguments:=FileBackgroundWorkerArguments)
            'END - Directory Sync

            'START - File Sync
            Dim DataRowsFileFileSystemEntryVersionsWithoutDeltaOperationAndZeroCurrentVersionNumberAndWithEffectiveUserPermissions As DataRow() = DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Select(filterExpression:="[DeltaOperationId] IS NULL AND [CurrentVersionNumber] = 0 AND [FileSystemEntryTypeId] = " & FileType.File)
            If DataRowsFileFileSystemEntryVersionsWithoutDeltaOperationAndZeroCurrentVersionNumberAndWithEffectiveUserPermissions.Length > 0 Then
                Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                    Using DataSetDeltaOperations As DataSet = New DataSet()
                        Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                "SELECT TOP 0" &
                                                                                "      [DeltaOperationId]" &
                                                                                "    , [FileSystemEntryId]" &
                                                                                "    , [FileSystemEntryVersionId]" &
                                                                                "    , [FileSystemEntryStatusId]" &
                                                                                "    , [LocalFileSystemEntryName]" &
                                                                                "    , [LocalFileSystemEntryExtension]" &
                                                                                "    , [LocalCreatedOnUTC]" &
                                                                                "    , [IsOpen]" &
                                                                                "    , [IsConflicted]" &
                                                                                " FROM" &
                                                                                "    [DeltaOperations]", selectConnection:=SqlConnectionClient)
                            Try
                                With SqlDataAdapterDeltaOperations
                                    .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                    .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                    .Fill(dataSet:=DataSetDeltaOperations)
                                End With

                                Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
                            Catch Exception As Exception
                                LoggerFileSync.Error("Unable to connect to client database.", Exception)
                            End Try

                            If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
                                'Mark Temp Created files
                                Try
                                    With DataSetDeltaOperations.Tables(0)
                                        .BeginLoadData()

                                        For Each drFileVersionWithoutDeltaZeroVersion As DataRow In DataRowsFileFileSystemEntryVersionsWithoutDeltaOperationAndZeroCurrentVersionNumberAndWithEffectiveUserPermissions
                                            Dim FilePath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=drFileVersionWithoutDeltaZeroVersion(columnName:="FileSystemEntryPathRelativeToShare").ToString())
                                            Dim isTempFileCreated = False
                                            Try
                                                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                                                    If Not File.Exists(path:=FilePath) AndAlso Directory.GetParent(path:=FilePath).Exists Then
                                                        File.Create(path:=FilePath).Dispose()
                                                        isTempFileCreated = True
                                                    Else
                                                        isTempFileCreated = True
                                                    End If
                                                End Using
                                            Catch
                                            End Try

                                            If isTempFileCreated Then
                                                drFileVersionWithoutDeltaZeroVersion(columnName:="DeltaOperationId") = Guid.NewGuid()
                                                drFileVersionWithoutDeltaZeroVersion(columnName:="IsConflicted") = False
                                                drFileVersionWithoutDeltaZeroVersion(columnName:="FileSystemEntryStatusId") = FileEntryStatus.TempCreated

                                                .LoadDataRow(values:={
                                                             drFileVersionWithoutDeltaZeroVersion(columnName:="DeltaOperationId"),
                                                             drFileVersionWithoutDeltaZeroVersion(columnName:="FileSystemEntryId"),
                                                             drFileVersionWithoutDeltaZeroVersion(columnName:="FileSystemEntryVersionId"),
                                                             drFileVersionWithoutDeltaZeroVersion(columnName:="FileSystemEntryStatusId"),
                                                             FilePath,
                                                             Nothing,
                                                             Nothing,
                                                             False,
                                                             False
                                                         }, fAcceptChanges:=False)

                                            End If
                                        Next

                                        .EndLoadData()
                                    End With

                                    SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)
                                Catch Exception As Exception
                                    LoggerFileSync.Error(String.Format(format:="Unable to create file 'DeltaOperation' record(s) for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey), Exception)
                                End Try
                            End If
                        End Using
                    End Using
                End Using
            End If

            'Get files without delta operation or having download related status
            Dim DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions As List(Of DataRow) = DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Select(filterExpression:="([DeltaOperationId] IS NULL OR [FileSystemEntryStatusId] = " & FileEntryStatus.PendingDownload & " OR [FileSystemEntryStatusId] = " & FileEntryStatus.Downloading & " OR [FileSystemEntryStatusId] = " & FileEntryStatus.Downloaded & " OR ([FileSystemEntryStatusId] = " & FileEntryStatus.TempCreated & " AND VersionNumber=1)) AND [FileSystemEntryTypeId] = " & FileType.File).ToList()
            If DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Count > 0 Then
                'Get file ids having previous conflicts
                Dim ConflictedFileSystemEntryIds As List(Of Guid) = New List(Of Guid)()
                Dim DataRowsFileConflictedFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions As List(Of DataRow) = DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Rows.Cast(Of DataRow).Where(Function(x) (Not IsDBNull(x(columnName:="DeltaOperationId"))) AndAlso CType(x(columnName:="IsConflicted"), Boolean) = True AndAlso CType(x(columnName:="FileSystemEntryTypeId"), FileType) = FileType.File).ToList()
                If DataRowsFileConflictedFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Count > 0 Then
                    ConflictedFileSystemEntryIds.AddRange(DataRowsFileConflictedFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Select(Function(x) CType(x(columnName:="FileSystemEntryId"), Guid)).Distinct())
                End If

                Dim DataRowsDownloadableFileLatestFileSystemEntryVersionsWithEffectiveUserPermissions As DataRow() = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) x(columnName:="FileSystemEntryVersionId").Equals(x(columnName:="LatestFileSystemEntryVersionIdOfFileSystemEntry"))).ToArray()

                'Get file versions having conflict or beyond conflict
                Dim DataRowsDownloadableFileConflictedOrConflictLaterFileSystemEntryVersionsWithEffectiveUserPermissions As List(Of DataRow) = New List(Of DataRow)()
                For Each ConflictedFileSystemEntryId As Guid In ConflictedFileSystemEntryIds
                    Dim ConflictedVersionNumber As Integer = 1

                    Dim DataRowsFileSpecificConflictedFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions As List(Of DataRow) = DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Rows.Cast(Of DataRow).Where(Function(x) (Not IsDBNull(x(columnName:="DeltaOperationId"))) AndAlso CType(x(columnName:="IsConflicted"), Boolean) = True AndAlso CType(x(columnName:="FileSystemEntryId"), Guid).Equals(g:=ConflictedFileSystemEntryId) AndAlso CType(x(columnName:="FileSystemEntryTypeId"), FileType) = FileType.File).ToList()

                    If DataRowsFileSpecificConflictedFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Count > 0 Then
                        ConflictedVersionNumber = DataRowsFileSpecificConflictedFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Select(Function(x) CInt(x(columnName:="VersionNumber"))).Max()
                    End If

                    DataRowsDownloadableFileConflictedOrConflictLaterFileSystemEntryVersionsWithEffectiveUserPermissions.AddRange(collection:=DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) CType(x(columnName:="FileSystemEntryId"), Guid).Equals(g:=ConflictedFileSystemEntryId) AndAlso CInt(x(columnName:="VersionNumber")) >= ConflictedVersionNumber))
                Next

                Dim IsLatestOrConflictedOrConflictLaterFileSystemEntryVersionsSelected As Boolean = False
                If DataRowsDownloadableFileLatestFileSystemEntryVersionsWithEffectiveUserPermissions.Length > 0 OrElse DataRowsDownloadableFileConflictedOrConflictLaterFileSystemEntryVersionsWithEffectiveUserPermissions.Count > 0 Then
                    Dim DataRowsDownloadableFileFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions As List(Of DataRow) = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) (Not IsDBNull(x(columnName:="DeltaOperationId")))).ToList()

                    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                        Using DataSetDeltaOperations As DataSet = New DataSet()
                            Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                    "SELECT" & If(DataRowsDownloadableFileFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Count = 0, " TOP 0", String.Empty).ToString() &
                                                                                    "    [DeltaOperationId]" &
                                                                                    "    , [FileSystemEntryId]" &
                                                                                    "    , [FileSystemEntryVersionId]" &
                                                                                    "    , [FileSystemEntryStatusId]" &
                                                                                    "    , [LocalFileSystemEntryName]" &
                                                                                    "    , [LocalFileSystemEntryExtension]" &
                                                                                    "    , [LocalCreatedOnUTC]" &
                                                                                    "    , [IsOpen]" &
                                                                                    "    , [IsConflicted]" &
                                                                                    " FROM" &
                                                                                    "    [DeltaOperations]" &
                                                                                    If(DataRowsDownloadableFileFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Count = 0, String.Empty, " WHERE /* [IsDeleted] = 0 AND */ [DeltaOperationId] IN (" & String.Join(separator:=", ", values:=DataRowsDownloadableFileFileSystemEntryVersionsWithDeltaOperationWithEffectiveUserPermissions.Select(Function(x) "'" & x(columnName:="DeltaOperationId").ToString() & "'")) & ")").ToString(), selectConnection:=SqlConnectionClient)
                                Try
                                    With SqlDataAdapterDeltaOperations
                                        .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                        .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                        .Fill(dataSet:=DataSetDeltaOperations)
                                    End With

                                    Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
                                Catch Exception As Exception
                                    LoggerFileSync.Error("Unable to connect to client database.", Exception)
                                End Try

                                If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
                                    Dim Index As Integer = 0

                                    While Index < DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Count
                                        Dim drDownloadableFileVersion As DataRow = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions(index:=Index)

                                        'If latest file version is not required to be downloaded or is one of conflicted or conflict later version
                                        If (Not DataRowsDownloadableFileLatestFileSystemEntryVersionsWithEffectiveUserPermissions.Contains(value:=drDownloadableFileVersion)) AndAlso (Not DataRowsDownloadableFileConflictedOrConflictLaterFileSystemEntryVersionsWithEffectiveUserPermissions.Contains(value:=drDownloadableFileVersion)) Then
                                            If (IsDBNull(drDownloadableFileVersion(columnName:="DeltaOperationId"))) Then
                                                'Insert no download required delta operation record for file version
                                                With DataSetDeltaOperations.Tables(0)
                                                    .BeginLoadData()

                                                    .LoadDataRow(values:={
                                                                 Guid.NewGuid(),
                                                                 drDownloadableFileVersion(columnName:="FileSystemEntryId"),
                                                                 drDownloadableFileVersion(columnName:="FileSystemEntryVersionId"),
                                                                 FileEntryStatus.NoDownloadRequired,
                                                                 Nothing,
                                                                 Nothing,
                                                                 Nothing,
                                                                 False,
                                                                 False
                                                             }, fAcceptChanges:=False)

                                                    .EndLoadData()
                                                End With
                                            Else
                                                'Update no download required status to delta operation record for file version
                                                Dim DataRowsDeltaOperation As DataRow() = DataSetDeltaOperations.Tables(0).Select(filterExpression:="[DeltaOperationId] = '" & drDownloadableFileVersion(columnName:="DeltaOperationId").ToString() & "'")

                                                If DataRowsDeltaOperation.Length > 0 Then
                                                    DataRowsDeltaOperation(0)(columnName:="FileSystemEntryStatusId") = FileEntryStatus.NoDownloadRequired
                                                    DataRowsDeltaOperation(0)(columnName:="IsConflicted") = False
                                                End If
                                            End If

                                            DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.RemoveAt(index:=Index)
                                        Else
                                            Index += 1
                                        End If
                                    End While

                                    Try
                                        SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)

                                        IsLatestOrConflictedOrConflictLaterFileSystemEntryVersionsSelected = True
                                    Catch Exception As Exception
                                        LoggerFileSync.Error(String.Format(format:="Unable to create or update file 'DeltaOperation' record(s) for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey), Exception)
                                    End Try
                                End If
                            End Using
                        End Using
                    End Using
                Else
                    IsLatestOrConflictedOrConflictLaterFileSystemEntryVersionsSelected = True
                End If

                If IsLatestOrConflictedOrConflictLaterFileSystemEntryVersionsSelected Then
                    'Prepare batch of downloadable files
                    Dim DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions As List(Of DataRow) = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) (Not IsDBNull(x(columnName:="DeltaOperationId"))) AndAlso CType(x(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.Downloading).Take(count:=FileSyncMaximumConcurrentFileDownloadBatchSize).ToList()

                    If DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Count < FileSyncMaximumConcurrentFileDownloadBatchSize Then
                        'Get pending downloadable file versions & set status to downloading
                        Dim drsPendingDownloadFileVersions As DataRow() = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) (Not IsDBNull(x(columnName:="DeltaOperationId"))) AndAlso CType(x(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.PendingDownload).ToArray()
                        If drsPendingDownloadFileVersions.Length > 0 Then
                            Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                                Using DataSetDeltaOperations As DataSet = New DataSet()
                                    Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                            "SELECT TOP " & (FileSyncMaximumConcurrentFileDownloadBatchSize - DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Count) &
                                                                                            "    [DeltaOperationId]" &
                                                                                            "    , [FileSystemEntryId]" &
                                                                                            "    , [FileSystemEntryVersionId]" &
                                                                                            "    , [FileSystemEntryStatusId]" &
                                                                                            "    , [LocalFileSystemEntryName]" &
                                                                                            "    , [LocalFileSystemEntryExtension]" &
                                                                                            "    , [LocalCreatedOnUTC]" &
                                                                                            "    , [IsOpen]" &
                                                                                            "    , [IsConflicted]" &
                                                                                            " FROM" &
                                                                                            "    [DeltaOperations]" &
                                                                                            " WHERE" &
                                                                                            "    /* [IsDeleted] = 0" &
                                                                                            "    AND */ [FileSystemEntryStatusId] = " & FileEntryStatus.PendingDownload &
                                                                                            "    AND [DeltaOperationId] IN (" & String.Join(separator:=", ", values:=drsPendingDownloadFileVersions.Select(Function(x) "'" & x(columnName:="DeltaOperationId").ToString() & "'")) & ")" &
                                                                                            "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryId]) > 0", selectConnection:=SqlConnectionClient)
                                        Try
                                            With SqlDataAdapterDeltaOperations
                                                .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                                .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                                .Fill(dataSet:=DataSetDeltaOperations)
                                            End With

                                            Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
                                        Catch Exception As Exception
                                            LoggerFileSync.Error("Unable to connect to client database.", Exception)
                                        End Try

                                        If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
                                            Try
                                                For Each DataRowDeltaOperation As DataRow In DataSetDeltaOperations.Tables(0).Rows
                                                    DataRowDeltaOperation(columnName:="FileSystemEntryStatusId") = FileEntryStatus.Downloading
                                                Next

                                                SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)

                                                Dim DataRowsToChangeStatusFromPendingDownloadToDownloading As DataRow() = drsPendingDownloadFileVersions.Where(Function(x) DataSetDeltaOperations.Tables(0).Rows.Cast(Of DataRow)().Select(Function(y) y(columnName:="DeltaOperationId")).Contains(value:=x(columnName:="DeltaOperationId"))).ToArray()

                                                drsPendingDownloadFileVersions = drsPendingDownloadFileVersions.Where(Function(x) Not DataRowsToChangeStatusFromPendingDownloadToDownloading.Select(Function(y) y(columnName:="DeltaOperationId")).Contains(value:=x(columnName:="DeltaOperationId"))).ToArray()

                                                For Each DataRowToChangeStatusFromPendingDownloadToDownloading As DataRow In DataRowsToChangeStatusFromPendingDownloadToDownloading
                                                    DataRowToChangeStatusFromPendingDownloadToDownloading(columnName:="FileSystemEntryStatusId") = FileEntryStatus.Downloading
                                                Next

                                                DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.AddRange(collection:=DataRowsToChangeStatusFromPendingDownloadToDownloading)
                                            Catch Exception As Exception
                                                LoggerFileSync.Error(String.Format(format:="Unable to update file 'DeltaOperation' record(s) status from '{0}' to '{1}' for '{2}'.", arg0:=FileEntryStatus.PendingDownload.ToString(), arg1:=FileEntryStatus.Downloading.ToString(), arg2:=BackgroundWorkerFileSyncDownloadKey), Exception)
                                            End Try
                                        End If
                                    End Using
                                End Using
                            End Using
                        End If
                    End If

                    'Add delta operation records with pending download status for files having version
                    Dim drsFileVersionsWithoutDeltaOperationAndNonZeroVersion As DataRow() = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) IsDBNull(x(columnName:="DeltaOperationId")) AndAlso CInt(x(columnName:="CurrentVersionNumber")) <> 0).ToArray()
                    If drsFileVersionsWithoutDeltaOperationAndNonZeroVersion.Length > 0 Then
                        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                            Using DataSetDeltaOperations As DataSet = New DataSet()
                                Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                        "SELECT TOP 0" &
                                                                                        "    [DeltaOperationId]" &
                                                                                        "    , [FileSystemEntryId]" &
                                                                                        "    , [FileSystemEntryVersionId]" &
                                                                                        "    , [FileSystemEntryStatusId]" &
                                                                                        "    , [LocalFileSystemEntryName]" &
                                                                                        "    , [LocalFileSystemEntryExtension]" &
                                                                                        "    , [LocalCreatedOnUTC]" &
                                                                                        "    , [IsOpen]" &
                                                                                        "    , [IsConflicted]" &
                                                                                        " FROM" &
                                                                                        "    [DeltaOperations]", selectConnection:=SqlConnectionClient)
                                    Try
                                        With SqlDataAdapterDeltaOperations
                                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                            .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                            .Fill(dataSet:=DataSetDeltaOperations)
                                        End With

                                        Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
                                    Catch Exception As Exception
                                        LoggerFileSync.Error("Unable to connect to client database.", Exception)
                                    End Try

                                    If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
                                        Try
                                            With DataSetDeltaOperations.Tables(0)
                                                .BeginLoadData()

                                                For Each drFileVersionWithoutDeltaOperationAndNonZeroVersion As DataRow In drsFileVersionsWithoutDeltaOperationAndNonZeroVersion
                                                    drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="DeltaOperationId") = Guid.NewGuid()

                                                    If DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Count < FileSyncMaximumConcurrentFileDownloadBatchSize Then
                                                        drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="FileSystemEntryStatusId") = FileEntryStatus.Downloading

                                                        DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Add(item:=drFileVersionWithoutDeltaOperationAndNonZeroVersion)
                                                    Else
                                                        drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="FileSystemEntryStatusId") = FileEntryStatus.PendingDownload
                                                    End If

                                                    .LoadDataRow(values:={
                                                                 drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="DeltaOperationId"),
                                                                 drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="FileSystemEntryId"),
                                                                 drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="FileSystemEntryVersionId"),
                                                                 drFileVersionWithoutDeltaOperationAndNonZeroVersion(columnName:="FileSystemEntryStatusId"),
                                                                 Nothing,
                                                                 Nothing,
                                                                 Nothing,
                                                                 False,
                                                                 False
                                                             }, fAcceptChanges:=False)
                                                Next

                                                .EndLoadData()
                                            End With

                                            SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)
                                        Catch Exception As Exception
                                            DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.RemoveAll(Function(x) DataSetDeltaOperations.Tables(0).Rows.Cast(Of DataRow)().Select(Function(y) y(columnName:="DeltaOperationId")).Contains(value:=x(columnName:="DeltaOperationId")))

                                            LoggerFileSync.Error(String.Format(format:="Unable to create file 'DeltaOperation' record(s) for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey), Exception)
                                        End Try
                                    End If
                                End Using
                            End Using
                        End Using
                    End If

                    'Check existance of temporary path
                    Dim TempPath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=Enums.Constants.TEMP_FOLDER_NAME)
                    Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                        Try
                            Dim DirectoryInfo As DirectoryInfo = New DirectoryInfo(path:=TempPath)

                            If Not DirectoryInfo.Exists Then
                                If Directory.GetParent(path:=TempPath).Exists Then
                                    DirectoryInfo = Directory.CreateDirectory(path:=TempPath)

                                    DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                                Else
                                    Throw New Exception(message:="Parent folder of TempPath '" & TempPath & "' does not exists.")
                                End If
                            ElseIf (DirectoryInfo.Attributes And FileAttributes.Hidden) = 0 Then
                                Try
                                    DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                                Catch Exception As Exception
                                    LoggerFileSync.Error("Unable to set hidden attribute to TempPath '" & TempPath & "'.", Exception)
                                End Try
                            End If
                        Catch Exception As Exception
                            LoggerFileSync.Error("Unable to create TempPath '" & TempPath & "'.", Exception)
                        End Try
                    End Using
                    'Mark downloaded files for moving
                    Dim DataRowsDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions As List(Of DataRow) = DataRowsDownloadableFileFileSystemEntryVersionsWithEffectiveUserPermissions.Where(Function(x) (Not IsDBNull(x(columnName:="DeltaOperationId"))) AndAlso CType(x(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.Downloaded).ToList()
                    If DataRowsDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Count > 0 Then
                        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                            Using DataSetDeltaOperations As DataSet = New DataSet()
                                Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                        "SELECT" &
                                                                                        "    [DeltaOperationId]" &
                                                                                        "    , [FileSystemEntryId]" &
                                                                                        "    , [FileSystemEntryVersionId]" &
                                                                                        "    , [FileSystemEntryStatusId]" &
                                                                                        "    , [LocalFileSystemEntryName]" &
                                                                                        "    , [LocalFileSystemEntryExtension]" &
                                                                                        "    , [LocalCreatedOnUTC]" &
                                                                                        "    , [IsOpen]" &
                                                                                        "    , [IsConflicted]" &
                                                                                        " FROM" &
                                                                                        "    [DeltaOperations]" &
                                                                                        " WHERE" &
                                                                                        "    /* [IsDeleted] = 0" &
                                                                                        "    AND */ [FileSystemEntryStatusId] = " & FileEntryStatus.Downloaded &
                                                                                        "    AND [DeltaOperationId] IN (" & String.Join(separator:=", ", values:=DataRowsDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Select(Function(x) "'" & x(columnName:="DeltaOperationId").ToString() & "'")) & ")" &
                                                                                        "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryId]) > 0", selectConnection:=SqlConnectionClient)
                                    Try
                                        With SqlDataAdapterDeltaOperations
                                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                            .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                            .Fill(dataSet:=DataSetDeltaOperations)
                                        End With

                                        Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
                                    Catch Exception As Exception
                                        LoggerFileSync.Error(String.Format(format:="Unable to connect to client database while fetching DeltaOperations records with '{0}' status.", arg0:=FileEntryStatus.Downloaded.ToString()), Exception)
                                    End Try

                                    If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
                                        Try
                                            For Each DataRowDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions As DataRow In DataRowsDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions
                                                Dim DataRowsDeltaOperations As DataRow() = DataSetDeltaOperations.Tables(0).Select(filterExpression:="[DeltaOperationId] = '" & DataRowDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions(columnName:="DeltaOperationId").ToString() & "'")

                                                If DataRowsDeltaOperations.Length > 0 Then
                                                    Try
                                                        Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                                                            If File.Exists(path:=Path.Combine(path1:=TempPath, path2:=DataRowDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions(columnName:="ServerFileSystemEntryName").ToString())) Then
                                                                DataRowsDeltaOperations(0)(columnName:="FileSystemEntryStatusId") = FileEntryStatus.MovingDownloaded
                                                            Else
                                                                DataRowsDeltaOperations(0)(columnName:="FileSystemEntryStatusId") = FileEntryStatus.Downloading
                                                            End If
                                                        End Using
                                                    Catch
                                                    End Try
                                                End If
                                            Next

                                            SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)

                                            DataRowsDownloadedDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.RemoveAll(Function(x) DataSetDeltaOperations.Tables(0).Select(filterExpression:="[FileSystemEntryStatusId] = " & FileEntryStatus.MovingDownloaded).Select(Function(y) y(columnName:="DeltaOperationId")).Contains(value:=x(columnName:="DeltaOperationId")))
                                        Catch Exception As Exception
                                            LoggerFileSync.Error(String.Format(format:="Unable to update file 'DeltaOperation' record(s) status from '{0}' to '{1}' for '{2}'.", arg0:=FileEntryStatus.Downloaded.ToString(), arg1:=FileEntryStatus.MovingDownloaded.ToString(), arg2:=BackgroundWorkerFileSyncDownloadKey), Exception)
                                        End Try
                                    End If
                                End Using
                            End Using
                        End Using
                    End If

                    'Start download thread for files
                    If DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions.Count > 0 Then
                        Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)

                            If Directory.Exists(TempPath) Then
                                SyncLock Me.TasksFileSyncDownload
                                    For Each drBatchDownloadingFileVersions As DataRow In DataRowsBatchDownloadingDownloadableFileFileSystemEntryVersionsWithDeltaOperationAndWithEffectiveUserPermissions
                                        Dim BlobName As Guid = CType(drBatchDownloadingFileVersions(columnName:="ServerFileSystemEntryName"), Guid)
                                        Dim fileSystemEntryId As Guid = CType(drBatchDownloadingFileVersions(columnName:="FileSystemEntryId"), Guid)

                                        Dim IsCreateTaskFileSyncDownload As Boolean = True

                                        If Me.TasksFileSyncDownload.LongCount(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = FileBackgroundWorkerArguments.ShareId) > 0 Then
                                            Dim TaskFileSyncDownload As Task = Me.TasksFileSyncDownload.First(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = FileBackgroundWorkerArguments.ShareId).Value

                                            If TaskFileSyncDownload.Status = TaskStatus.Canceled OrElse TaskFileSyncDownload.Status = TaskStatus.Faulted Then
                                                Me.TasksFileSyncDownload.Remove(key:=Me.TasksFileSyncDownload.First(Function(x) x.Value.Equals(obj:=TaskFileSyncDownload)).Key)
                                            Else
                                                IsCreateTaskFileSyncDownload = False
                                            End If
                                        End If

                                        If IsCreateTaskFileSyncDownload Then
                                            Me.TasksFileSyncDownload.Add(key:=New KeyValuePair(Of Guid, Integer)(key:=BlobName, value:=FileBackgroundWorkerArguments.ShareId), value:=Task.Factory.StartNew(action:=Sub() Me.TaskFileSyncDownload_DoTask(AccountId:=FileBackgroundWorkerArguments.AccountId, UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, AccessToken:=FileBackgroundWorkerArguments.AccessToken, UserId:=FileBackgroundWorkerArguments.UserId, DeltaOperationId:=CType(drBatchDownloadingFileVersions(columnName:="DeltaOperationId"), Guid), LocalDatabaseName:=FileBackgroundWorkerArguments.LocalDatabaseName, ShareId:=FileBackgroundWorkerArguments.ShareId, SharePath:=FileBackgroundWorkerArguments.SharePath, BlobName:=BlobName, RoleId:=FileBackgroundWorkerArguments.RoleId, fileSystemEntryId:=fileSystemEntryId, serviceUrl:=FileBackgroundWorkerArguments.CloudSyncServiceURL, windowsUserName:=FileBackgroundWorkerArguments.WindowsUserName, domainName:=FileBackgroundWorkerArguments.DomainName, password:=FileBackgroundWorkerArguments.Password)))
                                        End If
                                    Next
                                End SyncLock

                                If Not FileBackgroundWorkerArguments.IsRunInServiceMode Then
                                    Threading.Thread.Sleep(millisecondsTimeout:=CInt(FileSyncDownloadServiceIntervalInMilliSeconds))

                                    Me.BackgroundWorkerFileSyncDownload_DoWork(Sender:=Sender, DoWorkEventArgs:=DoWorkEventArgs)
                                End If
                            Else
                                LoggerFileSync.Error("Unable to sync files as TempPath '" & TempPath & "' does not exists or not accessible.")
                            End If
                        End Using
                    End If
                Else
                    LoggerFileSync.Error(String.Format("Could not select latest ot conflicted or conflict later versions for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey))
                End If
            End If
            'END - File Sync
        End If

        If FileBackgroundWorkerArguments.IsRunInServiceMode Then
            Dim TasksFileSyncDownloadCount As Long = 0

            SyncLock Me.TasksFileSyncDownload
                TasksFileSyncDownloadCount = Me.TasksFileSyncDownload.LongCount(Function(x) x.Key.Value = FileBackgroundWorkerArguments.ShareId)
            End SyncLock

            If TasksFileSyncDownloadCount = 0 Then
                RaiseEvent FileDownloadFinishedForShare(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, ShareId:=FileBackgroundWorkerArguments.ShareId)
            End If
        End If

        LoggerFileSync.Info(String.Format(format:="Finished Sync for '{0}'...", arg0:=BackgroundWorkerFileSyncDownloadKey))
    End Sub

    Public Sub DirectorySync(FileBackgroundWorkerArguments As FileBackgroundWorkerArguments)
        Dim BackgroundWorkerFileSyncDownloadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, UserId:=FileBackgroundWorkerArguments.UserId, ShareId:=FileBackgroundWorkerArguments.ShareId, OnDemandSyncFileSystemEntryId:=FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId)

        Dim DataTableFileSystemEntryVersionsWithEffectiveUserPermissions As DataTable = Nothing

        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
            Using DataSetClient As DataSet = New DataSet()
                Using SqlDataAdapterClient As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                               "/*DELETE FROM [DeltaOperations] WHERE [FileSystemEntryStatusId] = " & FileEntryStatus.TempCreated & ";" &
                                                               "*/" &
                                                               " SELECT" &
                                                               "    [FileSystemEntries].[FileSystemEntryTypeId]" &
                                                               "    , [FileSystemEntries].[CurrentVersionNumber]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryVersionId]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                               "    , [FileSystemEntryVersions].[VersionNumber]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryRelativePath] AS [FileSystemEntryPathRelativeToShare]" &
                                                               "    , [FileSystemEntryVersions].[ServerFileSystemEntryName]" &
                                                               "    , [DeltaOperations].[DeltaOperationId]" &
                                                               "    , [DeltaOperations].[FileSystemEntryStatusId]" &
                                                               "    , [DeltaOperations].[IsConflicted]" &
                                                               "    , [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryVersions].[FileSystemEntryId]) AS [EffectiveUserPermissions]" &
                                                               "    , [dbo].[GetLatestFileSystemEntryVersionId]([FileSystemEntryVersions].[FileSystemEntryId]) AS [LatestFileSystemEntryVersionIdOfFileSystemEntry]" &
                                                               " FROM" &
                                                               "    [FileSystemEntryVersions]" &
                                                               "    INNER JOIN [FileSystemEntries] ON [FileSystemEntries].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                               "    LEFT OUTER JOIN [DeltaOperations] ON /* [DeltaOperations].[IsDeleted] = 0 AND */ [DeltaOperations].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId] AND [DeltaOperations].[FileSystemEntryVersionId] = [FileSystemEntryVersions].[FileSystemEntryVersionId]" &
                                                               " WHERE" &
                                                               "    [FileSystemEntries].[ShareId] = " & FileBackgroundWorkerArguments.ShareId.ToString() &
                                                               "    AND [FileSystemEntries].[IsDeleted] = 0 " &
                                                               "    AND [FileSystemEntryVersions].[IsDeleted] = 0 " &
                                                               If(FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId Is Nothing, String.Empty,
                                                               "    AND (" &
                                                               "        (" &
                                                               "            (SELECT [FileSystemEntryTypeId] FROM [FileSystemEntries] WHERE [FileSystemEntryId] = @FileSystemEntryId) = 3" &
                                                               "            AND [FileSystemEntries].[FileSystemEntryId] = @FileSystemEntryId" &
                                                               "        )" &
                                                               "        OR (" &
                                                               "            (SELECT [FileSystemEntryTypeId] FROM [FileSystemEntries] WHERE [FileSystemEntryId] = @FileSystemEntryId) <> 3" &
                                                               "            AND [FileSystemEntries].[FileSystemEntryId] IN (SELECT [FileSystemEntryId] FROM [dbo].[GetLatestFileSystemEntryVersionsChildrenHierarchy](@FileSystemEntryId))" &
                                                               "        )" &
                                                               "    )").ToString() &
                                                               "    AND LTRIM(RTRIM([FileSystemEntryVersions].[FileSystemEntryRelativePath]))<>''" &
                                                               "    AND (" &
                                                               "        [DeltaOperations].[DeltaOperationId] IS NULL" &
                                                               "        OR (" &
                                                               "            [DeltaOperations].[FileSystemEntryStatusId] <> " & FileEntryStatus.NoActionRequired &
                                                               "            AND [DeltaOperations].[FileSystemEntryStatusId] <> " & FileEntryStatus.NoDownloadRequired &
                                                               "        )" &
                                                               "    )" &
                                                               "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryVersions].[FileSystemEntryId]) > 0" &
                                                               "    AND [FileSystemEntryVersions].[VersionNumber] IS NOT NULL" &
                                                               " ORDER BY" &
                                                               "    [FileSystemEntryPathRelativeToShare]", selectConnection:=SqlConnectionClient)
                    Try
                        With SqlDataAdapterClient
                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds

                            If Not FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId Is Nothing Then
                                .SelectCommand.Parameters.Add(value:=New SqlParameter(parameterName:="@FileSystemEntryId", dbType:=SqlDbType.UniqueIdentifier))
                                .SelectCommand.Parameters(0).Value = FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId
                            End If

                            .Fill(dataSet:=DataSetClient)
                        End With

                        DataTableFileSystemEntryVersionsWithEffectiveUserPermissions = DataSetClient.Tables(0)
                    Catch Exception As Exception
                        LoggerFileSync.Error("Unable to connect to client database.", Exception)
                    End Try
                End Using
            End Using
        End Using

        For Each a In DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Select(filterExpression:="[DeltaOperationId] IS NULL AND [FileSystemEntryTypeId] = " & FileType.Folder).OrderBy(Function(p) p("FileSystemEntryPathRelativeToShare")).GroupBy(Function(p) p("FileSystemEntryId"))
            Dim folderVersions = a.OrderBy(Function(p) p("VersionNumber"))
            Dim versionNumber = CInt(folderVersions.First()("VersionNumber"))
            Dim DirectoryLatestFileSystemEntryVersionPath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=folderVersions.Last()("FileSystemEntryPathRelativeToShare").ToString())
            Try
                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                    If Directory.GetParent(path:=DirectoryLatestFileSystemEntryVersionPath).Exists Then
                        If Directory.Exists(path:=DirectoryLatestFileSystemEntryVersionPath) Then
                            ' rename this folder to ## and delete all file records from DB
                            Dim parentDir = Directory.GetParent(path:=DirectoryLatestFileSystemEntryVersionPath).FullName
                            Dim filename = Path.GetFileName(DirectoryLatestFileSystemEntryVersionPath)
                            Dim i = 0
                            Dim newPath = Path.Combine(parentDir, "##" & filename)
                            While Directory.Exists(newPath)
                                i = i + 1
                                newPath = Path.Combine(parentDir, "##_" & i & filename)
                            End While

                            Directory.Move(DirectoryLatestFileSystemEntryVersionPath, newPath)
                            'remove all file ids for the DirectoryLatestFileSystemEntryVersionPath and its children
                            Using client As New LocalFileClient()
                                client.DeleteFiles(DirectoryLatestFileSystemEntryVersionPath, FileBackgroundWorkerArguments.ShareId)
                            End Using
                        End If
                    End If
                End Using
                Dim isDeleted As Boolean = False
                Dim isActionTaken = False
                If (versionNumber = 1) Then
                    'Try
                    Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                        If Directory.GetParent(path:=DirectoryLatestFileSystemEntryVersionPath).Exists Then
                            Directory.CreateDirectory(path:=DirectoryLatestFileSystemEntryVersionPath)
                            isActionTaken = True
                        End If
                    End Using
                    'Catch Exception As Exception
                    '    LoggerFileSync.Error(String.Format(format:="Unable to create folder '{0}' for '{1}'.", arg0:=DirectoryLatestFileSystemEntryVersionPath, arg1:=BackgroundWorkerFileSyncDownloadKey), Exception)
                    'End Try
                Else
                    Dim prevVersionFolderPath As String = ""
                    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                        Using DataSetClient As DataSet = New DataSet()
                            Using SqlDataAdapterClient As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                   " SELECT [FileSystemEntryVersions].[FileSystemEntryVersionId]" &
                                                                   "    , [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                                   "    , [FileSystemEntryVersions].[VersionNumber]" &
                                                                   "    , [FileSystemEntryVersions].[FileSystemEntryRelativePath] AS [FileSystemEntryPathRelativeToShare]" &
                                                                   "    , [FileSystemEntryVersions].[ServerFileSystemEntryName]" &
                                                                   " FROM" &
                                                                   "    [FileSystemEntryVersions]" &
                                                                   " WHERE" &
                                                                   "    [FileSystemEntryVersions].FileSystemEntryId = '" & a.Key.ToString() & "'" &
                                                                   "    AND [FileSystemEntryVersions].[VersionNumber] = " & (versionNumber - 1).ToString &
                                                                   "", selectConnection:=SqlConnectionClient)
                                Try
                                    With SqlDataAdapterClient
                                        .SelectCommand.CommandTimeout = CommandTimeoutInSeconds

                                        .Fill(dataSet:=DataSetClient)
                                    End With

                                    Dim prevVersionDataTable As DataTable = DataSetClient.Tables(0)
                                    If (prevVersionDataTable IsNot Nothing AndAlso prevVersionDataTable.Rows IsNot Nothing AndAlso prevVersionDataTable.Rows.Count > 0) Then
                                        prevVersionFolderPath = prevVersionDataTable.Rows(0).Item("FileSystemEntryPathRelativeToShare").ToString()
                                    End If
                                Catch Exception As Exception
                                    LoggerFileSync.Error("Unable to connect to client database.", Exception)
                                End Try

                            End Using
                        End Using
                    End Using
                    'Dim prevVersionRecord = DataTableFileSystemEntryVersionsWithEffectiveUserPermissions.Select("FileSystemEntryId = '" & a.Key.ToString() & "' AND VersionNumber = " & versionNumber - 1).FirstOrDefault()
                    If Not String.IsNullOrWhiteSpace(prevVersionFolderPath) Then
                        Dim prevDirectoryPath = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=prevVersionFolderPath)
                        Dim isMoved = False
                        Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                            If (Directory.Exists(prevDirectoryPath)) Then
                                Try
                                    Directory.Move(prevDirectoryPath, DirectoryLatestFileSystemEntryVersionPath)
                                    isMoved = True
                                    isActionTaken = True
                                Catch ex As Exception
                                    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                                        If (SqlConnectionClient.State <> ConnectionState.Open) Then
                                            SqlConnectionClient.Open()
                                        End If
                                        Using sqlCommand As SqlCommand = New SqlCommand("INSERT INTO RelativePathsExcludedFromSyncs (ShareId, RelativePath) values (" +
                                                                                        FileBackgroundWorkerArguments.ShareId.ToString() +
                                                                                        ", '" + folderVersions.Last()("FileSystemEntryPathRelativeToShare").ToString() +
                                                                                        "'),(" + FileBackgroundWorkerArguments.ShareId.ToString() + ", '" + prevVersionFolderPath + "')", SqlConnectionClient)
                                            sqlCommand.ExecuteNonQuery()
                                        End Using
                                    End Using
                                End Try
                            Else
                                'create delta with mark as deleted
                                isDeleted = True
                                isActionTaken = True
                            End If
                        End Using
                        If isMoved Then
                            Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                                If (SqlConnectionClient.State <> ConnectionState.Open) Then
                                    SqlConnectionClient.Open()
                                End If
                                Using sqlCommand As SqlCommand = New SqlCommand("UPDATE [dbo].[FileSystemEntryVersions] SET [FileSystemEntryRelativePath] = '" +
                                                                                folderVersions.Last()("FileSystemEntryPathRelativeToShare").ToString() +
                                                                                "' WHERE [FileSystemEntryId]='" +
                                                                                a.Key.ToString() +
                                                                                "' AND [VersionNumber]  IS NULL; " +
                                                                                "UPDATE D SET D.[LocalFileSystemEntryName] = '" +
                                                                                DirectoryLatestFileSystemEntryVersionPath +
                                                                                "' FROM [dbo].[DeltaOperations] D JOIN [dbo].[FileSystemEntryVersions] FSEV ON D.FileSystemEntryVersionId = FSEV.FileSystemEntryVersionId WHERE FSEV.[FileSystemEntryId]='" +
                                                                                a.Key.ToString() + "' AND FSEV.[VersionNumber]  IS NULL;" +
                                                                                "DELETE FROM RelativePathsExcludedFromSync WHERE ShareId=" +
                                                                                FileBackgroundWorkerArguments.ShareId.ToString() +
                                                                                " AND RelativePath in ('" + folderVersions.Last()("FileSystemEntryPathRelativeToShare").ToString() +
                                                                                "','" + prevVersionFolderPath + "')", SqlConnectionClient)
                                    sqlCommand.ExecuteNonQuery()
                                End Using
                            End Using
                        End If
                    End If
                End If
                If isActionTaken Then
                    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                        Using DataSetDeltaOperations As DataSet = New DataSet()
                            Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                    "SELECT TOP 0" &
                                                                                    "    [DeltaOperationId]" &
                                                                                    "    , [FileSystemEntryId]" &
                                                                                    "    , [FileSystemEntryVersionId]" &
                                                                                    "    , [FileSystemEntryStatusId]" &
                                                                                    "    , [LocalFileSystemEntryName]" &
                                                                                    "    , [LocalFileSystemEntryExtension]" &
                                                                                    "    , [LocalCreatedOnUTC]" &
                                                                                    "    , [IsOpen]" &
                                                                                    "    , [IsConflicted]" &
                                                                                    "    , [IsDeleted]" &
                                                                                    "    , [DeletedOnUTC]" &
                                                                                    "    , [DeletedByUserId]" &
                                                                                    " FROM" &
                                                                                    "    [DeltaOperations]", selectConnection:=SqlConnectionClient)
                                Try
                                    With SqlDataAdapterDeltaOperations
                                        .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                        .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                        .Fill(dataSet:=DataSetDeltaOperations)
                                    End With

                                    Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
                                Catch Exception As Exception
                                    LoggerFileSync.Error("Unable to connect to client database.", Exception)
                                End Try

                                If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
                                    Try
                                        With DataSetDeltaOperations.Tables(0)
                                            .BeginLoadData()

                                            For Each folderVersion In folderVersions
                                                Dim DirectoryInfo As DirectoryInfo = New DirectoryInfo(path:=Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=folderVersion(columnName:="FileSystemEntryPathRelativeToShare").ToString()))
                                                Dim deletedTime As Object = Nothing
                                                Dim deletingUser As Object = Nothing
                                                If isDeleted Then
                                                    deletedTime = DateTime.UtcNow
                                                    deletingUser = FileBackgroundWorkerArguments.UserId
                                                End If
                                                Dim dirPath As Object = Nothing
                                                Dim creationTime As Object = Nothing
                                                If (DirectoryInfo.Exists) Then
                                                    dirPath = DirectoryInfo.FullName
                                                    creationTime = DirectoryInfo.CreationTimeUtc
                                                End If
                                                .LoadDataRow(values:={
                                                         Guid.NewGuid(),
                                                         a.Key,
                                                         folderVersion(columnName:="FileSystemEntryVersionId"),
                                                         If(isDeleted, FileEntryStatus.PendingFileSystemEntryDelete, FileEntryStatus.NoActionRequired),
                                                         dirPath,
                                                         Nothing,
                                                         creationTime,
                                                         False,
                                                         False,
                                                         isDeleted,
                                                         deletedTime,
                                                         deletingUser
                                                }, fAcceptChanges:=False)
                                            Next

                                            .EndLoadData()
                                        End With

                                        SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)
                                    Catch Exception As Exception
                                        LoggerFileSync.Error(String.Format(format:="Unable to create directory 'DeltaOperation' record(s) for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey), Exception)
                                    End Try
                                End If
                            End Using
                        End Using
                    End Using
                End If
            Catch ex As Exception
                LoggerFileSync.Error(String.Format(format:="Unable to create directory 'DeltaOperation' record(s) for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey), ex)
            End Try
        Next
        'If drDirFileVersion.Length > 0 Then
        '    Dim dtDirLatestFileVersion As DataRow() = drDirFileVersion.Where(Function(x) x(columnName:="FileSystemEntryVersionId").Equals(x(columnName:="LatestFileSystemEntryVersionIdOfFileSystemEntry"))).ToArray()
        '    If dtDirLatestFileVersion.Length > 0 Then
        '        Dim lstDirLatestFileVersionRelativePath As List(Of String) = New List(Of String)

        '        For Each dirLatestFileVersionRelativePath As String In dtDirLatestFileVersion.Select(Function(x) x(columnName:="FileSystemEntryPathRelativeToShare"))
        '            If Not lstDirLatestFileVersionRelativePath.Exists(Function(x) x.StartsWith(value:=dirLatestFileVersionRelativePath & Path.DirectorySeparatorChar, comparisonType:=StringComparison.InvariantCultureIgnoreCase)) Then
        '                lstDirLatestFileVersionRelativePath.Add(dirLatestFileVersionRelativePath)
        '            End If
        '        Next

        '        For Each dirLatestFileVersionRelativePath As String In lstDirLatestFileVersionRelativePath
        '            Dim DirectoryLatestFileSystemEntryVersionPath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=dirLatestFileVersionRelativePath)

        '            Try

        '                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
        '                    If Directory.GetParent(path:=DirectoryLatestFileSystemEntryVersionPath).Exists Then
        '                        Directory.CreateDirectory(path:=DirectoryLatestFileSystemEntryVersionPath)
        '                    End If
        '                End Using
        '            Catch Exception As Exception
        '                LoggerFileSync.Error(String.Format(format:="Unable to create folder '{0}' for '{1}'.", arg0:=DirectoryLatestFileSystemEntryVersionPath, arg1:=BackgroundWorkerFileSyncDownloadKey), Exception)
        '            End Try
        '        Next
        '    End If

        '    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
        '        Using DataSetDeltaOperations As DataSet = New DataSet()
        '            Using SqlDataAdapterDeltaOperations As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
        '                                                                    "SELECT TOP 0" &
        '                                                                    "    [DeltaOperationId]" &
        '                                                                    "    , [FileSystemEntryId]" &
        '                                                                    "    , [FileSystemEntryVersionId]" &
        '                                                                    "    , [FileSystemEntryStatusId]" &
        '                                                                    "    , [LocalFileSystemEntryName]" &
        '                                                                    "    , [LocalFileSystemEntryExtension]" &
        '                                                                    "    , [LocalCreatedOnUTC]" &
        '                                                                    "    , [IsOpen]" &
        '                                                                    "    , [IsConflicted]" &
        '                                                                    " FROM" &
        '                                                                    "    [DeltaOperations]", selectConnection:=SqlConnectionClient)
        '                Try
        '                    With SqlDataAdapterDeltaOperations
        '                        .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
        '                        .MissingSchemaAction = MissingSchemaAction.AddWithKey
        '                        .Fill(dataSet:=DataSetDeltaOperations)
        '                    End With

        '                    Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterDeltaOperations)
        '                Catch Exception As Exception
        '                    LoggerFileSync.Error("Unable to connect to client database.", Exception)
        '                End Try

        '                If DataSetDeltaOperations.Tables.Count = 1 AndAlso (Not DataSetDeltaOperations.Tables(0) Is Nothing) Then
        '                    Try
        '                        With DataSetDeltaOperations.Tables(0)
        '                            .BeginLoadData()

        '                            For Each DataRowDirectoryFileSystemEntryVersionsWithoutDeltaOperationAndWithEffectiveUserPermissions In drDirFileVersion
        '                                Dim DirectoryInfo As DirectoryInfo = New DirectoryInfo(path:=Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=DataRowDirectoryFileSystemEntryVersionsWithoutDeltaOperationAndWithEffectiveUserPermissions(columnName:="FileSystemEntryPathRelativeToShare").ToString()))

        '                                .LoadDataRow(values:={
        '                                             Guid.NewGuid(),
        '                                             DataRowDirectoryFileSystemEntryVersionsWithoutDeltaOperationAndWithEffectiveUserPermissions(columnName:="FileSystemEntryId"),
        '                                             DataRowDirectoryFileSystemEntryVersionsWithoutDeltaOperationAndWithEffectiveUserPermissions(columnName:="FileSystemEntryVersionId"),
        '                                             FileSystemEntryStatus.NoActionRequired,
        '                                             If(DirectoryInfo.Exists, DirectoryInfo.FullName, Nothing),
        '                                             Nothing,
        '                                             If(DirectoryInfo.Exists, DirectoryInfo.CreationTimeUtc, Nothing),
        '                                             False,
        '                                             False
        '                                         }, fAcceptChanges:=False)
        '                            Next

        '                            .EndLoadData()
        '                        End With

        '                        SqlDataAdapterDeltaOperations.Update(dataSet:=DataSetDeltaOperations)
        '                    Catch Exception As Exception
        '                        LoggerFileSync.Error(String.Format(format:="Unable to create directory 'DeltaOperation' record(s) for '{0}'.", arg0:=BackgroundWorkerFileSyncDownloadKey), Exception)
        '                    End Try
        '                End If
        '            End Using
        '        End Using
        '    End Using
        'End If
    End Sub

    Private Sub BackgroundWorkerFileSyncDownload_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
        SyncLock Me.BackgroundWorkersFileSyncDownload
            Dim KeyValuePairBackgroundWorkerFileSyncDownload As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersFileSyncDownload.First(Function(x) x.Value.Equals(obj:=Sender))

            If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
                LoggerFileSync.Error(String.Format(format:="Error occurred while syncing files for '{0}'.", arg0:=KeyValuePairBackgroundWorkerFileSyncDownload.Key), RunWorkerCompletedEventArgs.Error)
            End If

            Me.BackgroundWorkersFileSyncDownload.Remove(key:=KeyValuePairBackgroundWorkerFileSyncDownload.Key)

            KeyValuePairBackgroundWorkerFileSyncDownload.Value.Dispose()
        End SyncLock
    End Sub

    Private Sub TaskFileSyncDownload_DoTask(AccountId As Guid, UserAccountId As Integer, AccessToken As String, UserId As Guid, DeltaOperationId As Guid, LocalDatabaseName As String, ShareId As Integer, SharePath As String, BlobName As Guid, RoleId As Integer, fileSystemEntryId As Guid, serviceUrl As String, windowsUserName As String, domainName As String, password As String)
        Try
            If Me.TaskFileSyncDownload_DoTask(AccountId:=AccountId, UserAccountId:=UserAccountId, AccessToken:=AccessToken, ShareId:=ShareId, SharePath:=SharePath, BlobName:=BlobName, LocalDatabaseName:=LocalDatabaseName, DeltaOperationId:=DeltaOperationId, UserId:=UserId, IsStartFromScratchComparingHash:=False, RoleId:=RoleId, fileSystemEntryId:=fileSystemEntryId, serviceUrl:=serviceUrl, windowsUserName:=windowsUserName, domainName:=domainName, password:=password) Then
                Try
                    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=LocalDatabaseName))
                        Using SqlCommand As SqlCommand = New SqlCommand(cmdText:=
                                                         "UPDATE" &
                                                         "    [DeltaOperations]" &
                                                         " SET" &
                                                         "    [FileSystemEntryStatusId] = " & FileEntryStatus.Downloaded &
                                                         " WHERE" &
                                                         "    /* [IsDeleted] = 0" &
                                                         "    AND */ [FileSystemEntryStatusId] = " & FileEntryStatus.Downloading &
                                                         "    AND [DeltaOperationId] = '" & DeltaOperationId.ToString() & "'" &
                                                         "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & UserId.ToString() & ", [FileSystemEntryId]) > 0", connection:=SqlConnectionClient)
                            SqlConnectionClient.Open()

                            With SqlCommand
                                .CommandTimeout = CommandTimeoutInSeconds

                                .ExecuteNonQuery()
                            End With
                        End Using
                    End Using

                    LoggerFileSync.Debug(String.Format(format:="Downloaded blob '{0}'.", arg0:=BlobName.ToString()))
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to update file 'DeltaOperation' record(s) status from '{0}' to '{1}' for blob '{2}'.", arg0:=FileEntryStatus.Downloading.ToString(), arg1:=FileEntryStatus.Downloaded.ToString(), arg2:=BlobName.ToString()), innerException:=Exception)
                End Try

                SyncLock Me.TasksFileSyncDownload
                    If Me.TasksFileSyncDownload.LongCount(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = ShareId) > 0 Then
                        Me.TasksFileSyncDownload.Remove(key:=Me.TasksFileSyncDownload.First(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = ShareId).Key)
                    End If
                End SyncLock
            Else
                Throw New Exception(message:=String.Format(format:="Unable to download blob '{0}'.", arg0:=BlobName.ToString()))
            End If
        Catch Exception As Exception
            LoggerFileSync.Error(String.Format(format:="Unable to download blob '{0}'.", arg0:=BlobName.ToString()), Exception)

            Throw Exception
        End Try
    End Sub

    Private Function TaskFileSyncDownload_DoTask(AccountId As Guid, UserAccountId As Integer, AccessToken As String, ShareId As Integer, SharePath As String, BlobName As Guid, LocalDatabaseName As String, DeltaOperationId As Guid, UserId As Guid, IsStartFromScratchComparingHash As Boolean, RoleId As Integer, fileSystemEntryId As Guid, serviceUrl As String, windowsUserName As String, domainName As String, password As String) As Boolean
        'Dim FileSyncServiceClient As FileSyncServiceReference.FileSyncServiceClient = New FileSyncServiceReference.FileSyncServiceClient(endpointConfigurationName:="WSHttpBinding_IFileSyncService", remoteAddress:=FileMinisterFileSyncServiceURL)
        Dim user As LocalWorkSpaceInfo = New LocalWorkSpaceInfo() With {.AccessToken = AccessToken, .AccountId = AccountId, .UserId = UserId, .UserAccountId = UserAccountId, .RoleId = CType(RoleId, Role), .CloudSyncServiceUrl = serviceUrl}
        Dim SyncClient As SyncClient = New SyncClient(user)

        Dim CloudBlockBlob As CloudBlockBlob = Nothing

        Try
            'Dim FileMinisterProxyWrapper As New FileMinisterProxyWrapper(Of FileSyncServiceReference.FileSyncServiceClient, FileSyncServiceReference.IFileSyncService)(
            '    client:=FileSyncServiceClient,
            '    user:=New UserInfo() With {.AccountId = AccountId, .UserAccountId = UserAccountId, .AccessToken = AccessToken},
            '    invokeMethod:=Sub(ServiceClient)
            '                      CloudBlockBlob = New CloudBlockBlob(blobAbsoluteUri:=ServiceClient.GetSharedAccessSignatureUrl(ShareId:=ShareId, BlobName:=BlobName, SharedAccessSignatureType:=FileSyncServiceReference.SharedAccessSignatureType.Download))
            '                  End Sub)

            'CloudBlockBlob = New CloudBlockBlob(blobAbsoluteUri:=SyncClient.GetSharedAccessSignatureUrl(ShareId:=ShareId, BlobName:=BlobName, SharedAccessSignatureType:=FileSyncServiceReference.SharedAccessSignatureType.Download, fileSystemEntryId:=fileSystemEntryId).Data)
            Dim res = SyncClient.GetSharedAccessSignatureUrl(ShareId:=ShareId, BlobName:=BlobName, SharedAccessSignatureType:=SharedAccessSignatureType.Download, fileSystemEntryId:=fileSystemEntryId)
            If (res.Status = 200 AndAlso res.Data IsNot Nothing) Then
                CloudBlockBlob = New CloudBlockBlob(blobAbsoluteUri:=res.Data)
            Else
                Throw New Exception(res.Message)
            End If
        Catch Exception As Exception
            Throw New Exception(message:=String.Format(format:="Unable to download blob '{0}'.", arg0:=BlobName.ToString()), innerException:=Exception)
        End Try

        If (Not CloudBlockBlob Is Nothing) Then
            If Not CloudBlockBlob.Exists() Then
                Try
                    Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=LocalDatabaseName))
                        Using SqlCommand As SqlCommand = New SqlCommand(cmdText:=
                                                         "UPDATE" &
                                                         "    [DeltaOperations]" &
                                                         " SET" &
                                                         "    [FileSystemEntryStatusId] = " & FileEntryStatus.NotFoundOnAzureStorage &
                                                         " WHERE" &
                                                         "    /* [IsDeleted] = 0" &
                                                         "    AND */ [FileSystemEntryStatusId] = " & FileEntryStatus.Downloading &
                                                         "    AND [DeltaOperationId] = '" & DeltaOperationId.ToString() & "'" &
                                                         "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & UserId.ToString() & ", [FileSystemEntryId]) > 0", connection:=SqlConnectionClient)
                            SqlConnectionClient.Open()

                            With SqlCommand
                                .CommandTimeout = CommandTimeoutInSeconds

                                .ExecuteNonQuery()
                            End With
                        End Using
                    End Using

                    LoggerFileSync.Error(String.Format(format:="Blob '{0}' not found on Azure Storage.", arg0:=BlobName.ToString()))
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to update file 'DeltaOperation' record(s) status from '{0}' to '{1}' for blob '{2}'.", arg0:=FileEntryStatus.Downloading.ToString(), arg1:=FileEntryStatus.NotFoundOnAzureStorage.ToString(), arg2:=BlobName.ToString()), innerException:=Exception)
                End Try

                IsStartFromScratchComparingHash = True
            Else
                Dim CloudBlockBlobMetadata As Dictionary(Of String, String) = CType(CloudBlockBlob.Metadata(), Dictionary(Of String, String))

                If CloudBlockBlobMetadata.ContainsKey(key:=MetadataKeyIsUploadFinished) AndAlso CloudBlockBlobMetadata(key:=MetadataKeyIsUploadFinished).Equals(value:=Boolean.TrueString, comparisonType:=StringComparison.InvariantCultureIgnoreCase) Then
                    Dim CloudBlockBlobDownloadException As Exception = Nothing

                    Dim TempPath As String = Path.Combine(path1:=SharePath, path2:=Enums.Constants.TEMP_FOLDER_NAME)
                    Try
                        Using CommonUtils.Helper.Impersonate(windowsUserName, domainName, password)

                            Dim DirectoryInfo As DirectoryInfo = New DirectoryInfo(path:=TempPath)

                            If Not DirectoryInfo.Exists Then
                                If Directory.GetParent(path:=TempPath).Exists Then
                                    DirectoryInfo = Directory.CreateDirectory(path:=TempPath)

                                    DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                                Else
                                    Throw New Exception(message:="Parent folder of TempPath '" & TempPath & "' does not exists.")
                                End If
                            Else
                                Try
                                    DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                                Catch Exception As Exception
                                    LoggerFileSync.Error("Unable to set hidden attribute to TempPath '" & TempPath & "'.", Exception)
                                End Try
                            End If
                        End Using
                    Catch Exception As Exception
                        LoggerFileSync.Error("Unable to set hidden attribute to TempPath '" & TempPath & "'.", Exception)
                    End Try

                    Try
                        Dim FilePath As String = Path.Combine(path1:=TempPath, path2:=BlobName.ToString())
                        Using CommonUtils.Helper.Impersonate(windowsUserName, domainName, password)
                            Using FileStream As FileStream = New FileStream(path:=FilePath, mode:=FileMode.OpenOrCreate, access:=FileAccess.ReadWrite, share:=FileShare.None)
                                Dim CloudBlockBlobLength As Long = CloudBlockBlob.Properties.Length
                                Dim CloudBlockBlobHash As String = CloudBlockBlob.Properties.ContentMD5
                                Dim FileSize As Single = FileStream.Length
                                Dim BlockNumber As Integer = 0
                                Dim TotalBytesRead As Long = 0

                                If (Not IsStartFromScratchComparingHash) AndAlso FileSize > 0 Then
                                    If Not String.IsNullOrWhiteSpace(CloudBlockBlobHash) Then
                                        Dim FileHash As String = Convert.ToBase64String(inArray:=MD5CryptoServiceProvider.Create().ComputeHash(inputStream:=FileStream))

                                        FileStream.Position = 0

                                        If CloudBlockBlobHash.Equals(value:=FileHash, comparisonType:=StringComparison.InvariantCulture) Then
                                            Return True
                                        End If
                                    End If

                                    BlockNumber = CInt(Math.Truncate(d:=(FileSize / DownloadBlockSizeInBytes)))

                                    TotalBytesRead = BlockNumber * DownloadBlockSizeInBytes

                                    FileStream.Position = TotalBytesRead
                                End If

                                Do
                                    Dim Bytes(DownloadBlockSizeInBytes - 1) As Byte
                                    Dim BytesRead As Integer? = Nothing

                                    BlockNumber += 1

                                    Try
                                        BytesRead = CloudBlockBlob.DownloadRangeToByteArray(target:=Bytes, index:=0, blobOffset:=((BlockNumber - 1) * DownloadBlockSizeInBytes), length:=DownloadBlockSizeInBytes)
                                    Catch Exception As Exception
                                        CloudBlockBlobDownloadException = New Exception(message:=String.Format(format:="Unable to download blob '{0}' as exception thrown from Storage.", arg0:=BlobName.ToString()), innerException:=Exception)
                                    End Try

                                    If (Not BytesRead Is Nothing) AndAlso CloudBlockBlobDownloadException Is Nothing Then
                                        FileStream.Write(array:=Bytes, offset:=0, count:=BytesRead.Value)

                                        If IsStartFromScratchComparingHash AndAlso (Not String.IsNullOrWhiteSpace(CloudBlockBlobHash)) Then
                                            Dim FileStreamPosition As Long = FileStream.Position

                                            FileStream.Position = 0

                                            Dim FileHash As String = Convert.ToBase64String(inArray:=MD5CryptoServiceProvider.Create().ComputeHash(inputStream:=FileStream))

                                            FileStream.Position = FileStreamPosition

                                            If CloudBlockBlobHash.Equals(value:=FileHash, comparisonType:=StringComparison.InvariantCulture) Then
                                                Return True
                                            End If
                                        End If

                                        TotalBytesRead += BytesRead.Value
                                    Else
                                        Exit Do
                                    End If
                                Loop While TotalBytesRead < CloudBlockBlobLength

                                If TotalBytesRead >= CloudBlockBlobLength Then
                                    If Not String.IsNullOrWhiteSpace(CloudBlockBlobHash) Then
                                        FileStream.Position = 0

                                        Dim FileHash As String = Convert.ToBase64String(inArray:=MD5CryptoServiceProvider.Create().ComputeHash(inputStream:=FileStream))

                                        If CloudBlockBlobHash.Equals(value:=FileHash, comparisonType:=StringComparison.InvariantCulture) Then
                                            Return True
                                        End If
                                    Else
                                        Return True
                                    End If
                                End If
                            End Using
                        End Using
                    Catch Exception As Exception
                        Throw New Exception(message:=String.Format(format:="Unable to download blob '{0}' as TempPath '{1}' is not accessible.", arg0:=BlobName.ToString(), arg1:=TempPath), innerException:=Exception)
                    End Try

                    If Not CloudBlockBlobDownloadException Is Nothing Then
                        Throw CloudBlockBlobDownloadException
                    End If
                Else
                    Throw New Exception(message:=String.Format(format:="Unable to download blob '{0}' as blob does not exists.", arg0:=BlobName.ToString()))
                End If
            End If
        Else
            Throw New Exception(message:=String.Format(format:="Unable to download blob '{0}' as either blob does not exists or access is not granted.", arg0:=BlobName.ToString()))
        End If

        If Not IsStartFromScratchComparingHash Then
            Return Me.TaskFileSyncDownload_DoTask(AccountId:=AccountId, UserAccountId:=UserAccountId, AccessToken:=AccessToken, ShareId:=ShareId, SharePath:=SharePath, BlobName:=BlobName, LocalDatabaseName:=LocalDatabaseName, DeltaOperationId:=DeltaOperationId, UserId:=UserId, IsStartFromScratchComparingHash:=True, RoleId:=RoleId, fileSystemEntryId:=fileSystemEntryId, serviceUrl:=serviceUrl, windowsUserName:=windowsUserName, domainName:=domainName, password:=password)
        Else
            Return False
        End If
    End Function

    'Public Sub TimerFileSyncMoveDownloaded_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs)
    '    Dim DataTableUserAccounts As DataTable = Nothing
    '    Dim DataTableUserShares As DataTable = Nothing

    '    Me.GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerFileSync)

    '    If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
    '        If DataTableUserAccounts.Rows.Count = 0 Then
    '            LoggerFileSync.Debug("Unable to find last logged-in User.")
    '        Else
    '            If DataTableUserShares.Rows.Count = 0 Then
    '                LoggerFileSync.Debug("Unable to find any mapped share.")
    '            Else
    '                For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
    '                    Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

    '                    If (DataRowsUserShares.Length > 0) Then
    '                        For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
    '                            If DataRowsUserSharesByUserId.Count() > 0 Then
    '                                For Each DataRow As DataRow In DataRowsUserSharesByUserId
    '                                    Dim BackgroundWorkerFileSyncMoveDownloadedKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=CInt(DataRowsUserSharesByUserId.Key), ShareId:=CInt(DataRow(columnName:="ShareId")))

    '                                    SyncLock Me.BackgroundWorkersFileSyncMoveDownloaded
    '                                        If Not Me.BackgroundWorkersFileSyncMoveDownloaded.ContainsKey(key:=BackgroundWorkerFileSyncMoveDownloadedKey) Then
    '                                            Dim BackgroundWorkerFileSyncMoveDownloaded As BackgroundWorker = New BackgroundWorker()

    '                                            With BackgroundWorkerFileSyncMoveDownloaded
    '                                                AddHandler .DoWork, AddressOf Me.BackgroundWorkerFileSyncMoveDownloaded_DoWork
    '                                                AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerFileSyncMoveDownloaded_RunWorkerCompleted
    '                                            End With

    '                                            Me.BackgroundWorkersFileSyncMoveDownloaded.Add(key:=BackgroundWorkerFileSyncMoveDownloadedKey, value:=BackgroundWorkerFileSyncMoveDownloaded)

    '                                            BackgroundWorkerFileSyncMoveDownloaded.RunWorkerAsync(argument:=New FileBackgroundWorkerArguments With {
    '                                                                                    .IsRunInServiceMode = True,
    '                                                                                    .UserAccountId = CInt(DataRowUserAccounts(columnName:="UserAccountId")),
    '                                                                                    .AccountId = CInt(DataRowUserAccounts(columnName:="AccountId")),
    '                                                                                    .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
    '                                                                                    .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
    '                                                                                    .WorkSpaceId = New Guid(DataRowUserAccounts(columnName:="WorkSpaceId").ToString()),
    '                                                                                    .UserId = CInt(DataRowsUserSharesByUserId.Key),
    '                                                                                    .ShareId = CInt(DataRow(columnName:="ShareId")),
    '                                                                                    .SharePath = DataRow(columnName:="SharePath").ToString(),
    '                                                                                    .WindowsUser = If(DataRow.IsNull("WindowsUser"), Nothing, DataRow(columnName:="WindowsUser").ToString()),
    '                                                                                    .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1)),
    '                                                                                    .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0)),
    '                                                                                    .Password = CommonUtils.Helper.Decrypt(If(DataRow.IsNull("Password"), Nothing, DataRow(columnName:="Password").ToString()))
    '                                                                                })
    '                                        End If
    '                                    End SyncLock
    '                                Next
    '                            End If
    '                        Next
    '                    End If
    '                Next
    '            End If
    '        End If
    '    End If

    '    Me.TimerFileSyncMoveDownloaded.Start()
    'End Sub

    Public Sub BackgroundWorkerFileSyncMoveDownloaded_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim FileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim BackgroundWorkerFileSyncMoveDownloadedKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, UserId:=FileBackgroundWorkerArguments.UserId, ShareId:=FileBackgroundWorkerArguments.ShareId)

        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
            Using DataSetClient As DataSet = New DataSet()
                ''1. Just Delta Operation To Move
                ''2. Just File EntryVersion To Move
                ''3. Last Local File Version
                Using SqlDataAdapterClient As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                               "DECLARE @DeltaOperationIdsMovingDownloaded TABLE" &
                                                               " (" &
                                                               "    [DeltaOperationId] [UNIQUEIDENTIFIER] NOT NULL" &
                                                               ")" &
                                                               "" &
                                                               " INSERT INTO @DeltaOperationIdsMovingDownloaded" &
                                                               "    SELECT" &
                                                               "        [DeltaOperations].[DeltaOperationId]" &
                                                               "    FROM" &
                                                               "        [DeltaOperations]" &
                                                               "        INNER JOIN [FileSystemEntryVersions] ON [FileSystemEntryVersions].[FileSystemEntryId] = [DeltaOperations].[FileSystemEntryId] AND [FileSystemEntryVersions].[FileSystemEntryVersionId] = [DeltaOperations].[FileSystemEntryVersionId]" &
                                                               "        INNER JOIN [FileSystemEntries] ON [FileSystemEntries].[FileSystemEntryId] = [DeltaOperations].[FileSystemEntryId]" &
                                                               "        LEFT OUTER JOIN [RelativePathsExcludedFromSync] E ON E.ShareId = [FileSystemEntries].ShareId AND FileSystemEntryVersions.[FileSystemEntryRelativePath] LIKE CONCAT(E.RelativePath,'%')" &
                                                               "    WHERE" &
                                                               "        /* [DeltaOperations].[IsDeleted] = 0" &
                                                               "        AND */ [DeltaOperations].[IsConflicted] = 0" &
                                                               "        AND E.ShareId IS NULL" &
                                                               "        AND [DeltaOperations].[FileSystemEntryStatusId] = " & FileEntryStatus.MovingDownloaded &
                                                               "        AND LTRIM(RTRIM([FileSystemEntryVersions].[FileSystemEntryRelativePath]))<>''" &
                                                               "        AND [FileSystemEntries].[ShareId] = " & FileBackgroundWorkerArguments.ShareId.ToString() &
                                                               "        AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [DeltaOperations].[FileSystemEntryId]) > 0" &
                                                               "" &
                                                               " SELECT" &
                                                               "    [DeltaOperationId]" &
                                                               "    , [FileSystemEntryId]" &
                                                               "    , [FileSystemEntryVersionId]" &
                                                               "    , [FileSystemEntryStatusId]" &
                                                               "    , [LocalFileSystemEntryName]" &
                                                               "    , [LocalFileSystemEntryExtension]" &
                                                               "    , [LocalCreatedOnUTC]" &
                                                               "    , [IsOpen]" &
                                                               "    , [IsConflicted]" &
                                                               " FROM" &
                                                               "    [DeltaOperations]" &
                                                               " WHERE" &
                                                               "    [DeltaOperationId] IN (SELECT [DeltaOperationId] FROM @DeltaOperationIdsMovingDownloaded)" &
                                                               "" &
                                                               " SELECT" &
                                                               "    [DeltaOperations].[DeltaOperationId]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                               "    , [FileSystemEntryVersions].[VersionNumber]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryRelativePath]" &
                                                               "    , [FileSystemEntryVersions].[ServerFileSystemEntryName]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntrySize]" &
                                                               "    , [FileSystemEntryVersions].[FileSystemEntryHash]" &
                                                               "    , [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryVersions].[FileSystemEntryId]) AS [EffectiveUserPermissions]" &
                                                               " FROM" &
                                                               "    [DeltaOperations]" &
                                                               "    INNER JOIN [FileSystemEntryVersions] ON [FileSystemEntryVersions].[FileSystemEntryId] = [DeltaOperations].[FileSystemEntryId] AND [FileSystemEntryVersions].[FileSystemEntryVersionId] = [DeltaOperations].[FileSystemEntryVersionId]" &
                                                               " WHERE" &
                                                               "    [DeltaOperations].[DeltaOperationId] IN (SELECT [DeltaOperationId] FROM @DeltaOperationIdsMovingDownloaded)" &
                                                               "" &
                                                               " SELECT DISTINCT" &
                                                               "    [DO1].[FileSystemEntryId]" &
                                                               "    , [V].[FileSystemEntryStatusId]" &
                                                               "    , [V].[LocalCreatedOnUTC]" &
                                                               "    , [V].PreviousRelativePath" &
                                                               "    , [V].[VersionNumber]" &
                                                               "    , [V].[PreviousVersionNumber]" &
                                                               "    , [V].[FileSystemEntryHash]" &
                                                               "    , (SELECT CASE WHEN ISNULL(COUNT(*), 0) > 0 THEN 1 ELSE 0 END FROM [DeltaOperations] WHERE [FileSystemEntryId] = [DO1].[FileSystemEntryId] AND [IsDeleted] = 1) AS [AnyDeltaOperationsHasIsDeleted]" &
                                                               " FROM" &
                                                               "    [DeltaOperations] AS [DO1]" &
                                                               "    CROSS APPLY (" &
                                                               "        SELECT TOP 1" &
                                                               "            [DO2].[FileSystemEntryStatusId]" &
                                                               "            , [DO2].[LocalCreatedOnUTC]" &
                                                               "            , [FileSystemEntryVersions].FileSystemEntryRelativePath As PreviousRelativePath" &
                                                               "            , [DO2].[IsConflicted]" &
                                                               "            , [FileSystemEntryVersions].[VersionNumber]" &
                                                               "            , (SELECT [FSEV].[VersionNumber] FROM [FileSystemEntryVersions] AS [FSEV] WHERE [FSEV].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId] AND [FSEV].[FileSystemEntryVersionId] = [FileSystemEntryVersions].[PreviousFileSystemEntryVersionId]) AS [PreviousVersionNumber]" &
                                                               "            , [FileSystemEntryVersions].[FileSystemEntryHash]" &
                                                               "        FROM" &
                                                               "            [DeltaOperations] AS [DO2]" &
                                                               "            INNER JOIN [FileSystemEntryVersions] ON [FileSystemEntryVersions].[FileSystemEntryVersionId] = [DO2].[FileSystemEntryVersionId]" &
                                                               "        WHERE" &
                                                               "            [DO2].[FileSystemEntryId] = [DO1].[FileSystemEntryId]" &
                                                               "            AND [DO2].FileSystemEntryStatusId<>4" &
                                                               "        ORDER BY" &
                                                               "            [DO2].[LocalCreatedOnUTC] DESC" &
                                                               "    ) AS [V]" &
                                                               " WHERE" &
                                                               "    [DO1].[DeltaOperationId] IN (SELECT [DeltaOperationId] FROM @DeltaOperationIdsMovingDownloaded)" &
                                                               "    /*AND [V].[IsConflicted] = 0*/" &
                                                               "    AND [V].[LocalCreatedOnUTC] IS NOT NULL", selectConnection:=SqlConnectionClient)
                    Try
                        With SqlDataAdapterClient
                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                            .MissingSchemaAction = MissingSchemaAction.AddWithKey
                            .Fill(dataSet:=DataSetClient)
                        End With

                        Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterClient)
                    Catch Exception As Exception
                        LoggerFileSync.Error("Unable to connect to client database.", Exception)
                    End Try

                    If DataSetClient.Tables.Count = 3 AndAlso (Not DataSetClient.Tables(0) Is Nothing) AndAlso (Not DataSetClient.Tables(1) Is Nothing) AndAlso (Not DataSetClient.Tables(2) Is Nothing) AndAlso DataSetClient.Tables(0).Rows.Count > 0 Then
                        'If DataSetConflictClient.Tables.Count = 1 AndAlso (Not DataSetConflictClient.Tables(0) Is Nothing) Then
                        Dim DataTableMovingDownloadedDeltaOperationsWithEffectiveUserPermissions As DataTable = DataSetClient.Tables(0)
                        Dim DataRowsMovingDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions As DataRow() = DataSetClient.Tables(1).Rows.Cast(Of DataRow)().ToArray()
                        Dim drsToMoveFileSystemEntriesLatestLocalCreated As DataRow() = DataSetClient.Tables(2).Rows.Cast(Of DataRow)().ToArray()

                        For Each drToMoveDeltaOperations As DataRow In DataTableMovingDownloadedDeltaOperationsWithEffectiveUserPermissions.Rows
                            Dim drToMoveFileSystemEntryVersion As DataRow = DataRowsMovingDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions.FirstOrDefault(Function(x) x(columnName:="DeltaOperationId").Equals(obj:=drToMoveDeltaOperations(columnName:="DeltaOperationId")))

                            Using DataSetConflictClient As DataSet = New DataSet()
                                Using SqlDataAdapterConflictClient As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                               "SELECT TOP 1" &
                                                                               "    [FileSystemEntryVersionConflictId]" &
                                                                               "    , [FileSystemEntryId]" &
                                                                               "    , [FileSystemEntryVersionId]" &
                                                                               "    , [FileSystemEntryVersionConflictTypeId]" &
                                                                               "    , [UserId]" &
                                                                               "    , [WorkSpaceId]" &
                                                                               "    , [FileSystemEntryPath]" &
                                                                               "    , [FileSystemEntryNameAndExtension]" &
                                                                               "    , [IsResolved]" &
                                                                               "    , [IsActionTaken]" &
                                                                               "    , [ResolvedByUserId]" &
                                                                               "    , [ResolvedOnUTC]" &
                                                                               "    , [ResolvedType]" &
                                                                               "    , [CreatedOnUTC]" &
                                                                               " FROM" &
                                                                               "    [FileSystemEntryVersionConflicts]" &
                                                                               " WHERE FileSystemEntryId='" & drToMoveDeltaOperations(columnName:="FileSystemEntryId").ToString() & "' AND IsResolved=0 And UserId=" & FileBackgroundWorkerArguments.UserId.ToString() &
                                                                               " And WorkSpaceId='" & FileBackgroundWorkerArguments.WorkSpaceId.ToString() & "'", selectConnection:=SqlConnectionClient)
                                    Try
                                        With SqlDataAdapterConflictClient
                                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                            .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                            .Fill(dataSet:=DataSetConflictClient)
                                        End With

                                        Dim SqlCommandBuilder As SqlCommandBuilder = New SqlCommandBuilder(adapter:=SqlDataAdapterConflictClient)
                                    Catch Exception As Exception
                                        LoggerFileSync.Error("Unable to connect to client database.", Exception)
                                    End Try
                                    Dim DataTableFileSystemEntryVersionConflicts As DataTable = DataSetConflictClient.Tables(0)

                                    If Not drToMoveFileSystemEntryVersion Is Nothing Then
                                        Dim drToMoveFileSystemEntriesLatestLocalCreated As DataRow = drsToMoveFileSystemEntriesLatestLocalCreated.FirstOrDefault(Function(x) x(columnName:="FileSystemEntryId").Equals(obj:=drToMoveDeltaOperations(columnName:="FileSystemEntryId")))

                                        Try
                                            Dim LocalReplicaFilePath As String = Path.Combine(path1:=Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=Enums.Constants.TEMP_FOLDER_NAME), path2:=drToMoveFileSystemEntryVersion(columnName:="ServerFileSystemEntryName").ToString())

                                            Dim fileExists As Boolean = False
                                            Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                                                fileExists = File.Exists(path:=LocalReplicaFilePath)
                                            End Using

                                            If fileExists Then
                                                Dim destinationFileRelativePath = drToMoveFileSystemEntryVersion(columnName:="FileSystemEntryRelativePath").ToString()
                                                Dim destinationFilePath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=destinationFileRelativePath)
                                                Dim oldFileRelativePath As String = String.Empty
                                                Dim oldFilePath As String = String.Empty
                                                Dim fileRenamed = False
                                                If drToMoveFileSystemEntriesLatestLocalCreated IsNot Nothing Then
                                                    oldFileRelativePath = drToMoveFileSystemEntriesLatestLocalCreated(columnName:="PreviousRelativePath").ToString
                                                    oldFilePath = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=oldFileRelativePath)
                                                    If (Not oldFilePath.Equals(destinationFilePath)) Then
                                                        fileRenamed = True
                                                    End If
                                                End If


                                                Dim ConflictType? As FileVersionConflictType = Nothing
                                                Dim IsMoveFile As Boolean = False

                                                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                                                    Dim newFileInfo As IO.FileInfo = New IO.FileInfo(fileName:=destinationFilePath)
                                                    Dim oldFileInfo = newFileInfo
                                                    If (fileRenamed) Then
                                                        oldFileInfo = New IO.FileInfo(fileName:=oldFilePath)
                                                    End If
                                                    If oldFileInfo.Exists Then
                                                        If (Not fileRenamed) AndAlso oldFileInfo.Length = 0 Then
                                                            'in case of new file and its length is zero
                                                            IsMoveFile = True
                                                        ElseIf drToMoveFileSystemEntriesLatestLocalCreated Is Nothing And newFileInfo.Exists Then
                                                            If (CType(drToMoveFileSystemEntryVersion(columnName:="EffectiveUserPermissions"), PermissionType) And PermissionType.Write) <> PermissionType.Write Then
                                                                'if only read permission then move dest to temp and download server file
                                                                Dim tempdestfilename = Path.Combine(path1:=Path.GetDirectoryName(path:=destinationFilePath), path2:="##_Self_" & If(String.IsNullOrEmpty(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="VersionNumber").ToString()), drToMoveFileSystemEntriesLatestLocalCreated(columnName:="PreviousVersionNumber").ToString(), drToMoveFileSystemEntriesLatestLocalCreated(columnName:="VersionNumber").ToString()).ToString() & "_" & Path.GetFileName(path:=destinationFilePath))
                                                                If (Not File.Exists(tempdestfilename)) Then
                                                                    File.Move(sourceFileName:=destinationFilePath, destFileName:=tempdestfilename)
                                                                End If
                                                            Else
                                                                If Not (newFileInfo.Length = CLng(drToMoveFileSystemEntryVersion(columnName:="FileSystemEntrySize")) AndAlso HashCalculator.hash_generator(hash_type:="md5", file_name:=newFileInfo.FullName) = drToMoveFileSystemEntryVersion(columnName:="FileSystemEntryHash").ToString) Then
                                                                    'no record and diff file exist so its in the way conflict
                                                                    ConflictType = FileVersionConflictType.InTheWay
                                                                End If
                                                            End If
                                                            IsMoveFile = True
                                                        ElseIf IsDBNull(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="VersionNumber")) OrElse CType(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.NewModified OrElse CType(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.PendingUpload OrElse CType(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.Uploading OrElse CType(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.PendingCheckInAfterUploading Then
                                                            'version is being uploaded means modified comflict
                                                            If Not fileRenamed Then
                                                                'if oldpath and new path is same
                                                                ConflictType = FileVersionConflictType.Modified
                                                            Else
                                                                ConflictType = FileVersionConflictType.ClientModifyServerRename
                                                            End If

                                                            IsMoveFile = True
                                                        ElseIf Math.Abs(value:=oldFileInfo.LastWriteTimeUtc.Subtract(value:=CDate(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="LocalCreatedOnUTC"))).TotalMilliseconds) < 1000 OrElse drToMoveFileSystemEntriesLatestLocalCreated(columnName:="FileSystemEntryHash").ToString().Equals(value:=HashCalculator.hash_generator(hash_type:="md5", file_name:=oldFileInfo.FullName), comparisonType:=StringComparison.InvariantCulture) Then
                                                            ' time diff < 1 sec or hash is matching
                                                            IsMoveFile = True
                                                        ElseIf (CType(drToMoveFileSystemEntryVersion(columnName:="EffectiveUserPermissions"), PermissionType) And PermissionType.Write) <> PermissionType.Write Then
                                                            'if only read permission then move dest to temp and download server file
                                                            File.Move(sourceFileName:=destinationFilePath, destFileName:=Path.Combine(path1:=Path.GetDirectoryName(path:=destinationFilePath), path2:="##_Self_" & If(String.IsNullOrEmpty(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="VersionNumber").ToString()), drToMoveFileSystemEntriesLatestLocalCreated(columnName:="PreviousVersionNumber").ToString(), drToMoveFileSystemEntriesLatestLocalCreated(columnName:="VersionNumber").ToString()).ToString() & "_" & Path.GetFileName(path:=destinationFilePath)))
                                                            IsMoveFile = True
                                                        End If
                                                    ElseIf (Not drToMoveFileSystemEntriesLatestLocalCreated Is Nothing) AndAlso CInt(drToMoveFileSystemEntriesLatestLocalCreated(columnName:="AnyDeltaOperationsHasIsDeleted")) > 0 Then
                                                        ConflictType = FileVersionConflictType.ClientDelete

                                                        IsMoveFile = True
                                                    Else
                                                        IsMoveFile = True
                                                    End If
                                                End Using

                                                If Not ConflictType Is Nothing Then
                                                    Dim diffConflict = False
                                                    If (DataTableFileSystemEntryVersionConflicts.Rows.Count > 0) Then
                                                        Dim oldconflictType = CType(DataTableFileSystemEntryVersionConflicts.Rows(0)("FileSystemEntryVersionConflictTypeId").ToString(), Byte)
                                                        If oldconflictType <> ConflictType.Value Then
                                                            DataTableFileSystemEntryVersionConflicts.Rows(0)("IsResolved") = True
                                                            DataTableFileSystemEntryVersionConflicts.Rows(0)("ResolvedByUserId") = FileBackgroundWorkerArguments.UserId
                                                            DataTableFileSystemEntryVersionConflicts.Rows(0)("ResolvedOnUTC") = DateTime.UtcNow
                                                            DataTableFileSystemEntryVersionConflicts.Rows(0)("ResolvedType") = Enums.Constants.NEW_CONFLICT_RESOLVETYPE
                                                            diffConflict = True
                                                        End If
                                                    End If

                                                    If (DataTableFileSystemEntryVersionConflicts.Rows.Count = 0 OrElse diffConflict) Then
                                                        With DataTableFileSystemEntryVersionConflicts
                                                            .BeginLoadData()

                                                            .LoadDataRow(values:={
                                                                         Guid.NewGuid(),
                                                                         drToMoveDeltaOperations(columnName:="FileSystemEntryId"),
                                                                         drToMoveDeltaOperations(columnName:="FileSystemEntryVersionId"),
                                                                         ConflictType.Value,
                                                                         FileBackgroundWorkerArguments.UserId,
                                                                         FileBackgroundWorkerArguments.WorkSpaceId,
                                                                         destinationFilePath,
                                                                         Path.GetFileName(path:=destinationFilePath),
                                                                         False,
                                                                         False,
                                                                         Nothing,
                                                                         Nothing,
                                                                         Nothing,
                                                                         DateTime.UtcNow
                                                                     }, fAcceptChanges:=False)

                                                            .EndLoadData()
                                                        End With
                                                    End If
                                                    Try
                                                        Using SqlCommand As SqlCommand = New SqlCommand(cmdText:=
                                                                                             "UPDATE" &
                                                                                             "    [DeltaOperations]" &
                                                                                             " SET" &
                                                                                             "    [IsConflicted] = 1" &
                                                                                             " WHERE" &
                                                                                             "    [DeltaOperationId] IN (" &
                                                                                             "        SELECT" &
                                                                                             "            [DeltaOperationId]" &
                                                                                             "        FROM" &
                                                                                             "            [DeltaOperations]" &
                                                                                             "            INNER JOIN [FileSystemEntryVersions] ON [FileSystemEntryVersions].[FileSystemEntryId] = [DeltaOperations].[FileSystemEntryId] AND [FileSystemEntryVersions].[FileSystemEntryVersionId] = [DeltaOperations].[FileSystemEntryVersionId]" &
                                                                                             "        WHERE" &
                                                                                             "            [FileSystemEntryVersions].[VersionNumber] IS NULL" &
                                                                                             "            AND [DeltaOperations].[FileSystemEntryId] ='" & drToMoveDeltaOperations(columnName:="FileSystemEntryId").ToString() & "'" &
                                                                                             "    )", connection:=SqlConnectionClient)
                                                            '"            AND [DeltaOperations].[FileSystemEntryId] IN (" & String.Join(separator:=", ", values:=DataTableFileSystemEntryVersionConflicts.Rows.Cast(Of DataRow).Select(Function(x) "'" & x(columnName:="FileSystemEntryId").ToString() & "'")) & ")" &
                                                            SqlConnectionClient.Open()

                                                            With SqlCommand
                                                                .CommandTimeout = CommandTimeoutInSeconds

                                                                .ExecuteNonQuery()
                                                            End With
                                                        End Using
                                                        SqlDataAdapterConflictClient.Update(dataTable:=DataTableFileSystemEntryVersionConflicts)
                                                    Catch Exception As Exception
                                                        LoggerFileSync.Error(String.Format(format:="Unable to insert conflict records for '{0}' file {1}.", arg0:=BackgroundWorkerFileSyncMoveDownloadedKey, arg1:=drToMoveDeltaOperations(columnName:="FileSystemEntryId").ToString()), Exception)
                                                        Continue For
                                                    End Try
                                                    'DestinationFilePath = Path.Combine(path1:=Path.GetDirectoryName(path:=DestinationFilePath), path2:="##_Server_" & If(DataRowMovingDownloadedFileSystemEntryLatestLocalCreatedOnUTCWithEffectiveUserPermissions Is Nothing, DataRowMovingDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions(columnName:="VersionNumber").ToString(), If(String.IsNullOrEmpty(DataRowMovingDownloadedFileSystemEntryLatestLocalCreatedOnUTCWithEffectiveUserPermissions(columnName:="VersionNumber").ToString()), DataRowMovingDownloadedFileSystemEntryLatestLocalCreatedOnUTCWithEffectiveUserPermissions(columnName:="PreviousVersionNumber").ToString(), DataRowMovingDownloadedFileSystemEntryLatestLocalCreatedOnUTCWithEffectiveUserPermissions(columnName:="VersionNumber").ToString()).ToString()).ToString() & "_" & Path.GetFileName(path:=DestinationFilePath))
                                                    destinationFilePath = Path.Combine(path1:=Path.GetDirectoryName(path:=destinationFilePath), path2:="##_Server_" & drToMoveFileSystemEntryVersion(columnName:="VersionNumber").ToString() & "_" & Path.GetFileName(path:=destinationFilePath))
                                                End If

                                                If IsMoveFile Then
                                                    Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                                                        If Directory.GetParent(path:=destinationFilePath).Exists Then
                                                            File.Copy(sourceFileName:=LocalReplicaFilePath, destFileName:=destinationFilePath, overwrite:=True)
                                                            File.Delete(path:=LocalReplicaFilePath)
                                                            Dim FileInfo As IO.FileInfo = New IO.FileInfo(fileName:=destinationFilePath)

                                                            If ConflictType IsNot Nothing Then
                                                                drToMoveDeltaOperations(columnName:="IsConflicted") = True
                                                            Else
                                                                If (fileRenamed) Then
                                                                    File.Delete(oldFilePath)
                                                                End If
                                                                drToMoveDeltaOperations(columnName:="IsConflicted") = False
                                                                drToMoveDeltaOperations(columnName:="FileSystemEntryStatusId") = FileEntryStatus.NoActionRequired
                                                            End If

                                                            drToMoveDeltaOperations(columnName:="LocalFileSystemEntryName") = destinationFilePath
                                                            drToMoveDeltaOperations(columnName:="LocalFileSystemEntryExtension") = Path.GetExtension(path:=destinationFilePath)
                                                            drToMoveDeltaOperations(columnName:="LocalCreatedOnUTC") = FileInfo.LastWriteTimeUtc
                                                        End If
                                                    End Using

                                                End If
                                            Else
                                                drToMoveDeltaOperations(columnName:="FileSystemEntryStatusId") = FileEntryStatus.Downloading
                                            End If
                                        Catch
                                        End Try
                                    End If
                                End Using
                            End Using
                        Next

                        'Try
                        '    If DataTableFileSystemEntryVersionConflicts.Rows.Count > 0 Then
                        '        Using SqlCommand As SqlCommand = New SqlCommand(cmdText:=
                        '                                         "UPDATE" &
                        '                                         "    [DeltaOperations]" &
                        '                                         " SET" &
                        '                                         "    [IsConflicted] = 1" &
                        '                                         " WHERE" &
                        '                                         "    [DeltaOperationId] IN (" &
                        '                                         "        SELECT" &
                        '                                         "            [DeltaOperationId]" &
                        '                                         "        FROM" &
                        '                                         "            [DeltaOperations]" &
                        '                                         "            INNER JOIN [FileSystemEntryVersions] ON [FileSystemEntryVersions].[FileSystemEntryId] = [DeltaOperations].[FileSystemEntryId] AND [FileSystemEntryVersions].[FileSystemEntryVersionId] = [DeltaOperations].[FileSystemEntryVersionId]" &
                        '                                         "        WHERE" &
                        '                                         "            [FileSystemEntryVersions].[VersionNumber] IS NULL" &
                        '                                         "            AND [DeltaOperations].[FileSystemEntryId] IN (" & String.Join(separator:=", ", values:=DataTableFileSystemEntryVersionConflicts.Rows.Cast(Of DataRow).Select(Function(x) "'" & x(columnName:="FileSystemEntryId").ToString() & "'")) & ")" &
                        '                                         "    )", connection:=SqlConnectionClient)
                        '            SqlConnectionClient.Open()

                        '            With SqlCommand
                        '                .CommandTimeout = CommandTimeoutInSeconds

                        '                .ExecuteNonQuery()
                        '            End With
                        '        End Using

                        '        SqlDataAdapterConflictClient.Update(dataTable:=DataTableFileSystemEntryVersionConflicts)
                        '    End If


                        Try
                            SqlDataAdapterClient.Update(dataTable:=DataTableMovingDownloadedDeltaOperationsWithEffectiveUserPermissions)

                            For Each DataRowMovedDownloadedDeltaOperationsWithEffectiveUserPermissions As DataRow In DataTableMovingDownloadedDeltaOperationsWithEffectiveUserPermissions.Rows.Cast(Of DataRow)().Where(Function(x) CType(x(columnName:="FileSystemEntryStatusId"), FileEntryStatus) = FileEntryStatus.NoActionRequired)
                                Dim DataRowMovedDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions As DataRow = DataRowsMovingDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions.FirstOrDefault(Function(x) x(columnName:="DeltaOperationId").Equals(obj:=DataRowMovedDownloadedDeltaOperationsWithEffectiveUserPermissions(columnName:="DeltaOperationId")))

                                If Not DataRowMovedDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions Is Nothing Then
                                    RaiseEvent FileDownloaded(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, ShareId:=FileBackgroundWorkerArguments.ShareId, FileSystemEntryId:=CType(DataRowMovedDownloadedFileSystemEntryVersionsWithEffectiveUserPermissions(columnName:="FileSystemEntryId"), Guid))
                                End If
                            Next
                        Catch Exception As Exception
                            LoggerFileSync.Error(String.Format(format:="Unable to update file 'DeltaOperation' record(s) status from '{0}' to '{1}' for '{2}'.", arg0:=FileEntryStatus.MovingDownloaded.ToString(), arg1:=FileEntryStatus.NoActionRequired.ToString(), arg2:=BackgroundWorkerFileSyncMoveDownloadedKey), Exception)
                        End Try
                        'Catch Exception As Exception
                        '    LoggerFileSync.Error(String.Format(format:="Unable to insert conflict records for '{0}'.", arg0:=BackgroundWorkerFileSyncMoveDownloadedKey), Exception)
                        'End Try
                    End If
                End Using
            End Using
            'End If
        End Using

        If FileBackgroundWorkerArguments.IsRunInServiceMode Then
            Try
                Dim CountOfDownloadableFiles As Integer = -1

                Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                    Using SqlCommand As SqlCommand = New SqlCommand(cmdText:=
                                                     "SELECT" &
                                                     "    COUNT(*)" &
                                                     " FROM" &
                                                     "    [FileSystemEntryVersions]" &
                                                     "    INNER JOIN [FileSystemEntries] ON [FileSystemEntries].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId]" &
                                                     "    LEFT OUTER JOIN [DeltaOperations] ON /* [DeltaOperations].[IsDeleted] = 0 AND */ [DeltaOperations].[FileSystemEntryId] = [FileSystemEntryVersions].[FileSystemEntryId] AND [DeltaOperations].[FileSystemEntryVersionId] = [FileSystemEntryVersions].[FileSystemEntryVersionId]" &
                                                     "    LEFT OUTER JOIN [RelativePathsExcludedFromSync] E ON E.ShareId = [FileSystemEntries].ShareId AND FileSystemEntryVersions.[FileSystemEntryRelativePath] LIKE CONCAT(E.RelativePath,'%')" &
                                                     " WHERE" &
                                                     "    [FileSystemEntries].[ShareId] = " & FileBackgroundWorkerArguments.ShareId.ToString() &
                                                     "    AND E.ShareId IS NULL" &
                                                     "    AND [FileSystemEntries].[IsDeleted] = 0 " &
                                                     "    AND [FileSystemEntryVersions].[IsDeleted] = 0 " &
                                                     "    AND LTRIM(RTRIM([FileSystemEntryVersions].[FileSystemEntryRelativePath]))<>''" &
                                                     "    AND (" &
                                                     "        [DeltaOperations].[DeltaOperationId] IS NULL" &
                                                     "        OR (" &
                                                     "            [DeltaOperations].[IsConflicted] = 0" &
                                                     "            AND (" &
                                                     "                [DeltaOperations].[FileSystemEntryStatusId] = " & FileEntryStatus.PendingDownload &
                                                     "                OR [DeltaOperations].[FileSystemEntryStatusId] = " & FileEntryStatus.Downloading &
                                                     "                OR [DeltaOperations].[FileSystemEntryStatusId] = " & FileEntryStatus.Downloaded &
                                                     "                OR [DeltaOperations].[FileSystemEntryStatusId] = " & FileEntryStatus.MovingDownloaded &
                                                     "            )" &
                                                     "        )" &
                                                     "    )" &
                                                     "    AND [dbo].[GetEffectiveUserPermissionsOnFileSystemEntry](" & FileBackgroundWorkerArguments.UserId.ToString() & ", [FileSystemEntryVersions].[FileSystemEntryId]) > 0", connection:=SqlConnectionClient)
                        SqlConnectionClient.Open()

                        With SqlCommand
                            .CommandTimeout = CommandTimeoutInSeconds

                            CountOfDownloadableFiles = CInt(.ExecuteScalar())
                        End With
                    End Using
                End Using

                If CountOfDownloadableFiles = 0 Then
                    RaiseEvent FileMoveDownloadedFinishedForShare(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, ShareId:=FileBackgroundWorkerArguments.ShareId)
                End If
            Catch Exception As Exception
                LoggerFileSync.Error("Unable to fetch count of Downloadable files.", Exception)
            End Try
        End If
    End Sub

    'Private Sub BackgroundWorkerFileSyncMoveDownloaded_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
    '    SyncLock Me.BackgroundWorkersFileSyncMoveDownloaded
    '        Dim KeyValuePairBackgroundWorkerFileSyncMoveDownloaded As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersFileSyncMoveDownloaded.First(Function(x) x.Value.Equals(obj:=Sender))

    '        If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
    '            LoggerFileSync.Error(String.Format(format:="Error occurred while syncing files for '{0}'.", arg0:=KeyValuePairBackgroundWorkerFileSyncMoveDownloaded.Key), RunWorkerCompletedEventArgs.Error)
    '        End If

    '        Me.BackgroundWorkersFileSyncMoveDownloaded.Remove(key:=KeyValuePairBackgroundWorkerFileSyncMoveDownloaded.Key)

    '        KeyValuePairBackgroundWorkerFileSyncMoveDownloaded.Value.Dispose()
    '    End SyncLock
    'End Sub

    Private Sub TimerFileSyncCheckIn_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs)
        Dim DataTableUserAccounts As DataTable = Nothing
        Dim DataTableUserShares As DataTable = Nothing

        FileMinisterService.GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerFileSync)

        If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
            If DataTableUserAccounts.Rows.Count = 0 Then
                LoggerFileSync.Debug("Unable to find last logged-in User.")
            Else
                If DataTableUserShares.Rows.Count = 0 Then
                    LoggerFileSync.Debug("Unable to find any mapped share.")
                Else
                    For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
                        Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

                        If (DataRowsUserShares.Length > 0) Then
                            For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
                                If DataRowsUserSharesByUserId.Count() > 0 Then
                                    For Each DataRow As DataRow In DataRowsUserSharesByUserId
                                        Dim BackgroundWorkerFileSyncCheckInKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=New Guid(DataRowsUserSharesByUserId.Key.ToString), ShareId:=CInt(DataRow(columnName:="ShareId")))

                                        SyncLock Me.BackgroundWorkersFileSyncCheckIn
                                            If Not Me.BackgroundWorkersFileSyncCheckIn.ContainsKey(key:=BackgroundWorkerFileSyncCheckInKey) Then
                                                Dim BackgroundWorkerFileSyncCheckIn As BackgroundWorker = New BackgroundWorker()

                                                With BackgroundWorkerFileSyncCheckIn
                                                    AddHandler .DoWork, AddressOf Me.BackgroundWorkerFileSyncCheckIn_DoWork
                                                    AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerFileSyncCheckIn_RunWorkerCompleted
                                                End With

                                                Me.BackgroundWorkersFileSyncCheckIn.Add(key:=BackgroundWorkerFileSyncCheckInKey, value:=BackgroundWorkerFileSyncCheckIn)

                                                BackgroundWorkerFileSyncCheckIn.RunWorkerAsync(argument:=New FileBackgroundWorkerArguments With {
                                                                                        .UserAccountId = CInt(DataRowUserAccounts(columnName:="UserAccountId")),
                                                                                        .AccountId = New Guid(DataRowUserAccounts(columnName:="AccountId").ToString),
                                                                                        .CloudSyncServiceURL = DataRowUserAccounts(columnName:="CloudSyncServiceURL").ToString(),
                                                                                        .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
                                                                                        .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
                                                                                        .WindowsUserSID = DataRowUserAccounts(columnName:="WindowsUserSID").ToString(),
                                                                                        .WorkSpaceId = New Guid(DataRowUserAccounts(columnName:="WorkSpaceId").ToString()),
                                                                                        .UserId = New Guid(DataRowsUserSharesByUserId.Key.ToString),
                                                                                        .ShareId = CInt(DataRow(columnName:="ShareId")),
                                                                                        .SharePath = DataRow(columnName:="SharePath").ToString(),
                                                                                        .WindowsUser = If(DataRow.IsNull("WindowsUser"), Nothing, DataRow(columnName:="WindowsUser").ToString()),
                                                                                        .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1)),
                                                                                        .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0)),
                                                                                        .Password = CommonUtils.Helper.Decrypt(If(DataRow.IsNull("Password"), Nothing, DataRow(columnName:="Password").ToString())),
                                                                                        .RoleId = CInt(DataRowUserAccounts(columnName:="RoleId"))
                                                                                    })
                                            End If
                                        End SyncLock
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        Me.TimerFileSyncCheckIn.Start()
    End Sub

    Private Sub BackgroundWorkerFileSyncCheckIn_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim FileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim UploadProcess As UploadProcess = New UploadProcess(ShareId:=FileBackgroundWorkerArguments.ShareId, ConnectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName), oUserInfo:=(New LocalWorkSpaceInfo() With {.UserId = FileBackgroundWorkerArguments.UserId, .AccessToken = FileBackgroundWorkerArguments.AccessToken, .UserAccountId = FileBackgroundWorkerArguments.UserAccountId, .AccountId = FileBackgroundWorkerArguments.AccountId, .WindowsUserSId = FileBackgroundWorkerArguments.WindowsUserSID, .RoleId = CType(FileBackgroundWorkerArguments.RoleId, Role), .CloudSyncServiceUrl = FileBackgroundWorkerArguments.CloudSyncServiceURL, .WorkSpaceId = FileBackgroundWorkerArguments.WorkSpaceId}))

        Try
            'Get Files that hve to be checked In
            Dim FilesForCheckIn As DataTable = UploadProcess.GetFilesforCheckIn()

            If (Not FilesForCheckIn Is Nothing AndAlso FilesForCheckIn.Rows.Count > 0) Then

                For Each dr As DataRow In FilesForCheckIn.Rows
                    Dim result As ResultInfo(Of Boolean, Status) = Nothing
                    Dim msg As String = String.Empty
                    Dim oFileInfo As FileEntryInfo = UploadProcess.MapFileSystemEntryInfo(dr:=dr)

                    Dim fileSystemEntryStatusId As Byte = Convert.ToByte(dr(columnName:="FileSystemEntryStatusId").ToString())
                    Dim strVersionNumber As String = dr(columnName:="VersionNumber").ToString()

                    If Not String.IsNullOrEmpty(strVersionNumber) Then
                        Dim VersionNumber As Integer = Convert.ToInt32(strVersionNumber)

                        If fileSystemEntryStatusId = FileEntryStatus.MoveOrRename Then

                            ' Case of File Moved or Renamd - Call AddVersionWithoutUpload API
                            Try
                                result = UploadProcess.AddVersionWithoutUpload(oFileInfo, oFileInfo.FileVersion)

                                If Not result.Data Then
                                    msg = String.Format("Error While CheckingIn a File (in case of Rename/Move) (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                    msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                    LoggerCheckInProcess.Error(msg)
                                End If
                            Catch ex As Exception
                                LoggerCheckInProcess.Error(ex.Message, ex)
                            End Try

                        ElseIf fileSystemEntryStatusId = FileEntryStatus.PendingCheckInAfterUploading Then
                            ' Case of File normal CheckIn - Call CheckIn API
                            Try
                                result = UploadProcess.CheckInFile(oFileVersionInfo:=oFileInfo.FileVersion, versionNumber:=VersionNumber)

                                If Not result.Data Then
                                    msg = String.Format("Error While CheckingIn a File (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                    msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                    LoggerCheckInProcess.Error(msg)
                                Else
                                    RaiseEvent FileUploaded(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, ShareId:=FileBackgroundWorkerArguments.ShareId, FileSystemEntryId:=oFileInfo.FileEntryId)
                                End If
                            Catch ex As Exception
                                LoggerCheckInProcess.Error(ex.Message, ex)
                            End Try

                        ElseIf fileSystemEntryStatusId = FileEntryStatus.PendingFileSystemEntryDelete Then
                            ' Case of file Delete - Call Soft Delete Server API
                            Try
                                result = UploadProcess.SoftDelete(oFileInfo.FileEntryId, VersionNumber)

                                If Not result.Data Then
                                    msg = String.Format("Error While SoftDeleting a File (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                                    msg = String.Format(msg & "Error msg: {0}", arg0:=result.Message)
                                    LoggerCheckInProcess.Error(msg)
                                End If

                            Catch ex As Exception
                                LoggerCheckInProcess.Error(ex.Message, ex)
                            End Try
                        End If

                    Else
                        msg = String.Format("CurrentVersion Not returned, Could Not CheckIn (name: '{0}', fileId: '{1}', versionId: '{2}') ", arg0:=oFileInfo.FileVersion.FileEntryNameWithExtension, arg1:=oFileInfo.FileEntryId.ToString(), arg2:=oFileInfo.FileVersion.FileVersionId.ToString())
                        LoggerCheckInProcess.Error(msg)
                    End If
                Next
            End If

        Catch ex As Exception
            LoggerCheckInProcess.Error("Error in CheckIn Process", ex)
        End Try


    End Sub

    Private Sub BackgroundWorkerFileSyncCheckIn_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
        SyncLock Me.BackgroundWorkersFileSyncCheckIn
            Dim KeyValuePairBackgroundWorkerFileSyncCheckIn As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersFileSyncCheckIn.First(Function(x) x.Value.Equals(obj:=Sender))

            If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
                LoggerFileSync.Error(String.Format(format:="Error occurred while syncing files for '{0}'.", arg0:=KeyValuePairBackgroundWorkerFileSyncCheckIn.Key), RunWorkerCompletedEventArgs.Error)
            End If

            Me.BackgroundWorkersFileSyncCheckIn.Remove(key:=KeyValuePairBackgroundWorkerFileSyncCheckIn.Key)

            KeyValuePairBackgroundWorkerFileSyncCheckIn.Value.Dispose()
        End SyncLock
    End Sub

    Private Sub TimerFileSyncCheckOut_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs)
        Dim DataTableUserAccounts As DataTable = Nothing
        Dim DataTableUserShares As DataTable = Nothing

        FileMinisterService.GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerFileSync)

        If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
            If DataTableUserAccounts.Rows.Count = 0 Then
                LoggerFileSync.Debug("Unable to find last logged-in User.")
            Else
                If DataTableUserShares.Rows.Count = 0 Then
                    LoggerFileSync.Debug("Unable to find any mapped share.")
                Else
                    For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
                        Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

                        If (DataRowsUserShares.Length > 0) Then
                            For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
                                If DataRowsUserSharesByUserId.Count() > 0 Then
                                    For Each DataRow As DataRow In DataRowsUserSharesByUserId
                                        Dim BackgroundWorkerFileSyncCheckOutKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=New Guid(DataRowsUserSharesByUserId.Key.ToString), ShareId:=CInt(DataRow(columnName:="ShareId")))

                                        SyncLock Me.BackgroundWorkersFileSyncCheckOut
                                            If Not Me.BackgroundWorkersFileSyncCheckOut.ContainsKey(key:=BackgroundWorkerFileSyncCheckOutKey) Then
                                                Dim BackgroundWorkerFileSyncCheckOut As BackgroundWorker = New BackgroundWorker()

                                                With BackgroundWorkerFileSyncCheckOut
                                                    AddHandler .DoWork, AddressOf Me.BackgroundWorkerFileSyncCheckOut_DoWork
                                                    AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerFileSyncCheckOut_RunWorkerCompleted
                                                End With

                                                Me.BackgroundWorkersFileSyncCheckOut.Add(key:=BackgroundWorkerFileSyncCheckOutKey, value:=BackgroundWorkerFileSyncCheckOut)

                                                BackgroundWorkerFileSyncCheckOut.RunWorkerAsync(argument:=New FileBackgroundWorkerArguments With {
                                                                                        .UserAccountId = CInt(DataRowUserAccounts(columnName:="UserAccountId")),
                                                                                        .AccountId = New Guid(DataRowUserAccounts(columnName:="AccountId").ToString),
                                                                                        .CloudSyncServiceURL = DataRowUserAccounts(columnName:="CloudSyncServiceURL").ToString(),
                                                                                        .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
                                                                                        .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
                                                                                        .WindowsUserSID = DataRowUserAccounts(columnName:="WindowsUserSID").ToString(),
                                                                                        .WorkSpaceId = New Guid(DataRowUserAccounts(columnName:="WorkSpaceId").ToString()),
                                                                                        .UserId = New Guid(DataRowsUserSharesByUserId.Key.ToString),
                                                                                        .ShareId = CInt(DataRow(columnName:="ShareId")),
                                                                                        .SharePath = DataRow(columnName:="SharePath").ToString(),
                                                                                        .WindowsUser = If(DataRow.IsNull("WindowsUser"), Nothing, DataRow(columnName:="WindowsUser").ToString()),
                                                                                        .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1)),
                                                                                        .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0)),
                                                                                        .Password = CommonUtils.Helper.Decrypt(If(DataRow.IsNull("Password"), Nothing, DataRow(columnName:="Password").ToString())),
                                                                                        .RoleId = CInt(DataRowUserAccounts(columnName:="RoleId"))
                                                                                    })
                                            End If
                                        End SyncLock
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        Me.TimerFileSyncCheckOut.Start()
    End Sub

    Private Sub BackgroundWorkerFileSyncCheckOut_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim FileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim UploadProcess As UploadProcess = New UploadProcess(ShareId:=FileBackgroundWorkerArguments.ShareId, ConnectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName), oUserInfo:=(New LocalWorkSpaceInfo() With {.UserId = FileBackgroundWorkerArguments.UserId, .AccessToken = FileBackgroundWorkerArguments.AccessToken, .UserAccountId = FileBackgroundWorkerArguments.UserAccountId, .AccountId = FileBackgroundWorkerArguments.AccountId, .WindowsUserSId = FileBackgroundWorkerArguments.WindowsUserSID, .RoleId = CType(FileBackgroundWorkerArguments.RoleId, Role), .CloudSyncServiceUrl = FileBackgroundWorkerArguments.CloudSyncServiceURL, .WorkSpaceId = FileBackgroundWorkerArguments.WorkSpaceId}))

        AddHandler UploadProcess.FileCheckedOut, AddressOf OnFileCheckedOut

        UploadProcess.CheckOutProcess()
    End Sub

    Private Sub BackgroundWorkerFileSyncCheckOut_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
        SyncLock Me.BackgroundWorkersFileSyncCheckOut
            Dim KeyValuePairBackgroundWorkerFileSyncCheckOut As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersFileSyncCheckOut.First(Function(x) x.Value.Equals(obj:=Sender))

            If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
                LoggerFileSync.Error(String.Format(format:="Error occurred while syncing files for '{0}'.", arg0:=KeyValuePairBackgroundWorkerFileSyncCheckOut.Key), RunWorkerCompletedEventArgs.Error)
            End If

            Me.BackgroundWorkersFileSyncCheckOut.Remove(key:=KeyValuePairBackgroundWorkerFileSyncCheckOut.Key)

            KeyValuePairBackgroundWorkerFileSyncCheckOut.Value.Dispose()
        End SyncLock
    End Sub

    Private Sub TimerFileSyncUpload_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs, Optional OnDemandSyncFileSystemEntryId As Guid? = Nothing)
        Dim DataTableUserAccounts As DataTable = Nothing
        Dim DataTableUserShares As DataTable = Nothing

        FileMinisterService.GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerFileSync)

        If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
            If DataTableUserAccounts.Rows.Count = 0 Then
                LoggerFileSync.Debug("Unable to find last logged-in User.")
            Else
                If DataTableUserShares.Rows.Count = 0 Then
                    LoggerFileSync.Debug("Unable to find any mapped share.")
                Else
                    For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
                        Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

                        If (DataRowsUserShares.Length > 0) Then
                            For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
                                If DataRowsUserSharesByUserId.Count() > 0 Then
                                    For Each DataRow As DataRow In DataRowsUserSharesByUserId
                                        Dim BackgroundWorkerFileSyncUploadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=New Guid(DataRowsUserSharesByUserId.Key.ToString), ShareId:=CInt(DataRow(columnName:="ShareId")), OnDemandSyncFileSystemEntryId:=OnDemandSyncFileSystemEntryId)

                                        SyncLock Me.BackgroundWorkersFileSyncUpload
                                            If Not Me.BackgroundWorkersFileSyncUpload.ContainsKey(key:=BackgroundWorkerFileSyncUploadKey) Then
                                                Dim BackgroundWorkerFileSyncUpload As BackgroundWorker = New BackgroundWorker()

                                                With BackgroundWorkerFileSyncUpload
                                                    AddHandler .DoWork, AddressOf Me.BackgroundWorkerFileSyncUpload_DoWork
                                                    AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerFileSyncUpload_RunWorkerCompleted
                                                End With

                                                Me.BackgroundWorkersFileSyncUpload.Add(key:=BackgroundWorkerFileSyncUploadKey, value:=BackgroundWorkerFileSyncUpload)

                                                BackgroundWorkerFileSyncUpload.RunWorkerAsync(argument:=New FileBackgroundWorkerArguments With {
                                                                                        .IsRunInServiceMode = ((Not Sender Is Nothing) AndAlso OnDemandSyncFileSystemEntryId Is Nothing),
                                                                                        .UserAccountId = CInt(DataRowUserAccounts(columnName:="UserAccountId")),
                                                                                        .AccountId = New Guid(DataRowUserAccounts(columnName:="AccountId").ToString),
                                                                                        .CloudSyncServiceURL = DataRowUserAccounts(columnName:="CloudSyncServiceURL").ToString(),
                                                                                        .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
                                                                                        .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
                                                                                        .WindowsUserSID = DataRowUserAccounts(columnName:="WindowsUserSID").ToString(),
                                                                                        .WorkSpaceId = New Guid(DataRowUserAccounts(columnName:="WorkSpaceId").ToString()),
                                                                                        .UserId = New Guid(DataRowsUserSharesByUserId.Key.ToString),
                                                                                        .ShareId = CInt(DataRow(columnName:="ShareId")),
                                                                                        .SharePath = DataRow(columnName:="SharePath").ToString(),
                                                                                        .WindowsUser = If(DataRow.IsNull("WindowsUser"), Nothing, DataRow(columnName:="WindowsUser").ToString()),
                                                                                        .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1)),
                                                                                        .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0)),
                                                                                        .Password = CommonUtils.Helper.Decrypt(If(DataRow.IsNull("Password"), Nothing, DataRow(columnName:="Password").ToString())),
                                                                                        .RoleId = CInt(DataRowUserAccounts(columnName:="RoleId")),
                                                                                        .OnDemandSyncFileSystemEntryId = OnDemandSyncFileSystemEntryId
                                                                                    })
                                            End If
                                        End SyncLock
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        If Not Sender Is Nothing Then
            Me.TimerFileSyncUpload.Start()
        ElseIf Not OnDemandSyncFileSystemEntryId Is Nothing Then
            While True
                Dim BackgroundWorkersOnDemandFileSyncUploadCount As Integer = 0

                SyncLock Me.BackgroundWorkersFileSyncUpload
                    BackgroundWorkersOnDemandFileSyncUploadCount = Me.BackgroundWorkersFileSyncUpload.Where(Function(x) x.Key.EndsWith(String.Format(format:=", OnDemandSyncFileSystemEntryId={0}", arg0:=OnDemandSyncFileSystemEntryId.Value.ToString()))).Count()
                End SyncLock

                If BackgroundWorkersOnDemandFileSyncUploadCount > 0 Then
                    Threading.Thread.Sleep(millisecondsTimeout:=CInt(FileSyncUploadServiceIntervalInMilliSeconds))
                Else
                    Exit While
                End If
            End While
        End If
    End Sub

    Private Sub BackgroundWorkerFileSyncUpload_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim FileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim BackgroundWorkerFileSyncUploadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, UserId:=FileBackgroundWorkerArguments.UserId, ShareId:=FileBackgroundWorkerArguments.ShareId, OnDemandSyncFileSystemEntryId:=FileBackgroundWorkerArguments.OnDemandSyncFileSystemEntryId)

        Try
            Dim user As LocalWorkSpaceInfo = New LocalWorkSpaceInfo() With {.UserId = FileBackgroundWorkerArguments.UserId, .AccessToken = FileBackgroundWorkerArguments.AccessToken, .AccountId = FileBackgroundWorkerArguments.AccountId, .WindowsUserSId = FileBackgroundWorkerArguments.WindowsUserSID, .RoleId = CType(FileBackgroundWorkerArguments.RoleId, Role), .CloudSyncServiceUrl = FileBackgroundWorkerArguments.CloudSyncServiceURL, .WorkSpaceId = FileBackgroundWorkerArguments.WorkSpaceId, .UserAccountId = FileBackgroundWorkerArguments.UserAccountId}
            Dim UploadProcess = New UploadProcess(ShareId:=FileBackgroundWorkerArguments.ShareId, ConnectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName), oUserInfo:=user)
            Dim DataTableFilesForUpload As DataTable = UploadProcess.GetFilesforUpload()
            If (Not DataTableFilesForUpload Is Nothing) AndAlso DataTableFilesForUpload.Rows.Count > 0 Then
                Dim TempPath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=Enums.Constants.TEMP_FOLDER_NAME)
                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)

                    Try
                        Dim DirectoryInfo As DirectoryInfo = New DirectoryInfo(path:=TempPath)

                        If Not DirectoryInfo.Exists Then
                            If Directory.GetParent(path:=TempPath).Exists Then
                                DirectoryInfo = Directory.CreateDirectory(path:=TempPath)

                                DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                            Else
                                Throw New Exception(message:="Parent folder of TempPath '" & TempPath & "' does not exists.")
                            End If
                        Else
                            Try
                                DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                            Catch Exception As Exception
                                LoggerFileSync.Error("Unable to set hidden attribute to TempPath '" & TempPath & "'.", Exception)
                            End Try
                        End If
                    Catch Exception As Exception
                        LoggerFileSync.Error("Unable to create TempPath '" & TempPath & "'.", Exception)
                    End Try

                    If Directory.Exists(TempPath) Then
                        SyncLock Me.TasksFileSyncUpload
                            For Each DataRowFileForUpload As DataRow In DataTableFilesForUpload.Rows
                                Dim BlobName As Guid = CType(DataRowFileForUpload(columnName:="ServerFileSystemEntryName"), Guid)
                                Dim fileSystemEntryVersionId As Guid = New Guid(DataRowFileForUpload(columnName:="FileSystemEntryVersionId").ToString())
                                Dim fileSystemEntryId As Guid = New Guid(DataRowFileForUpload(columnName:="FileSystemEntryId").ToString())
                                Dim status As FileEntryStatus = CType(Convert.ToByte(DataRowFileForUpload(columnName:="FileSystemEntryStatusId").ToString()), FileEntryStatus)
                                Dim IsCreateTaskFileSyncUpload As Boolean = True

                                If Me.TasksFileSyncUpload.LongCount(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = FileBackgroundWorkerArguments.ShareId) > 0 Then
                                    Dim TaskFileSyncUpload As Task = Me.TasksFileSyncUpload.First(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = FileBackgroundWorkerArguments.ShareId).Value

                                    If TaskFileSyncUpload.Status = TaskStatus.Canceled OrElse TaskFileSyncUpload.Status = TaskStatus.Faulted Then
                                        Me.TasksFileSyncUpload.Remove(key:=Me.TasksFileSyncUpload.First(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = FileBackgroundWorkerArguments.ShareId).Key)
                                    Else
                                        IsCreateTaskFileSyncUpload = False
                                    End If
                                End If

                                If IsCreateTaskFileSyncUpload Then
                                    Dim LocalReplicaFilePath As String = Path.Combine(path1:=TempPath, path2:=BlobName.ToString())

                                    If Not File.Exists(LocalReplicaFilePath) Then
                                        Dim FileForUploadPath As String = Path.Combine(path1:=FileBackgroundWorkerArguments.SharePath, path2:=DataRowFileForUpload(columnName:="FileSystemEntryRelativePath").ToString())

                                        Try
                                            If File.Exists(path:=FileForUploadPath) Then
                                                File.Copy(sourceFileName:=FileForUploadPath, destFileName:=LocalReplicaFilePath)
                                            End If
                                        Catch Exception As Exception
                                            LoggerFileSync.Error(String.Format(format:="Error occurred while copying file '{0}' to TempPath '{1}' to start upload process for '{2}'.", arg0:=FileForUploadPath, arg1:=LocalReplicaFilePath, arg2:=BackgroundWorkerFileSyncUploadKey), Exception)
                                        End Try
                                    End If

                                    If File.Exists(LocalReplicaFilePath) Then
                                        ' Set Status to Uploading if not already set
                                        If Not (status = FileEntryStatus.Uploading) Then
                                            Dim result As ResultInfo(Of Boolean, Status) = UploadProcess.UpdateFileVersionStatus(fileSystemEntryVersionId, FileEntryStatus.Uploading)

                                            If Not result.Data Then
                                                Throw New Exception(String.Format(format:="Error occurred while Chnaging the status to Uploading for version '{0}': msg: {1}.", arg0:=fileSystemEntryVersionId, arg1:=result.Message))
                                            End If
                                        End If
                                        Dim fileInfo As IO.FileInfo = New IO.FileInfo(LocalReplicaFilePath)
                                        Me.TasksFileSyncUpload.Add(key:=New KeyValuePair(Of Guid, Integer)(key:=BlobName, value:=FileBackgroundWorkerArguments.ShareId), value:=Task.Factory.StartNew(action:=Sub() Me.TaskFileSyncUpload_DoTask(AccountId:=FileBackgroundWorkerArguments.AccountId, UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, AccessToken:=FileBackgroundWorkerArguments.AccessToken, ShareId:=FileBackgroundWorkerArguments.ShareId, SharePath:=FileBackgroundWorkerArguments.SharePath, BlobName:=BlobName, UploadProcess:=UploadProcess, FileSystemEntryVersionId:=fileSystemEntryVersionId, user:=user, fileSystemEntryId:=fileSystemEntryId, filesize:=fileInfo.Length, windowsUserName:=FileBackgroundWorkerArguments.WindowsUserName, domainName:=FileBackgroundWorkerArguments.DomainName, password:=FileBackgroundWorkerArguments.Password)))
                                    End If
                                End If
                            Next

                        End SyncLock

                        If Not FileBackgroundWorkerArguments.IsRunInServiceMode Then
                            Threading.Thread.Sleep(millisecondsTimeout:=CInt(FileSyncUploadServiceIntervalInMilliSeconds))

                            Me.BackgroundWorkerFileSyncUpload_DoWork(Sender:=Sender, DoWorkEventArgs:=DoWorkEventArgs)
                        End If
                    End If
                End Using
            End If
        Catch Exception As Exception
            LoggerFileSync.Error(String.Format(format:="Error occurred while fetching list of files to upload for '{0}'.", arg0:=BackgroundWorkerFileSyncUploadKey), Exception)
        End Try

        If FileBackgroundWorkerArguments.IsRunInServiceMode Then
            Dim TasksFileSyncUploadCount As Long = 0

            SyncLock Me.TasksFileSyncUpload
                TasksFileSyncUploadCount = Me.TasksFileSyncUpload.LongCount(Function(x) x.Key.Value = FileBackgroundWorkerArguments.ShareId)
            End SyncLock

            If TasksFileSyncUploadCount = 0 Then
                RaiseEvent FileUploadFinishedForShare(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, ShareId:=FileBackgroundWorkerArguments.ShareId)
            End If
        End If
    End Sub

    Private Sub BackgroundWorkerFileSyncUpload_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
        SyncLock Me.BackgroundWorkersFileSyncUpload
            Dim KeyValuePairBackgroundWorkerFileSyncUpload As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersFileSyncUpload.First(Function(x) x.Value.Equals(obj:=Sender))

            If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
                LoggerFileSync.Error(String.Format(format:="Error occurred while syncing files for '{0}'.", arg0:=KeyValuePairBackgroundWorkerFileSyncUpload.Key), RunWorkerCompletedEventArgs.Error)
            End If

            Me.BackgroundWorkersFileSyncUpload.Remove(key:=KeyValuePairBackgroundWorkerFileSyncUpload.Key)

            KeyValuePairBackgroundWorkerFileSyncUpload.Value.Dispose()
        End SyncLock
    End Sub

    Private Sub TaskFileSyncUpload_DoTask(AccountId As Guid, UserAccountId As Integer, AccessToken As String, ShareId As Integer, SharePath As String, BlobName As Guid, UploadProcess As UploadProcess, FileSystemEntryVersionId As Guid, user As LocalWorkSpaceInfo, fileSystemEntryId As Guid, filesize As Long, windowsUserName As String, domainName As String, password As String)
        Try
            If Me.TaskFileSyncUpload_DoTask(AccountId:=AccountId, UserAccountId:=UserAccountId, AccessToken:=AccessToken, ShareId:=ShareId, SharePath:=SharePath, BlobName:=BlobName, user:=user, fileSystemEntryId:=fileSystemEntryId, filesize:=filesize, windowsUserName:=windowsUserName, domainName:=domainName, password:=password) Then

                'Change status from Uploading to PendingCheckInAfterUploading
                Dim result As ResultInfo(Of Boolean, Status) = UploadProcess.UpdateFileVersionStatus(FileSystemEntryVersionId, FileEntryStatus.PendingCheckInAfterUploading)

                If Not result.Data Then
                    Throw New Exception(String.Format(format:="Error occurred while Changing the status to PendingCheckInAfterUploading for version '{0}': msg: {1}.", arg0:=FileSystemEntryVersionId.ToString(), arg1:=result.Message))
                End If

                Dim FilePath As String = Path.Combine(path1:=Path.Combine(path1:=SharePath, path2:=Enums.Constants.TEMP_FOLDER_NAME), path2:=BlobName.ToString())
                Try
                    Using CommonUtils.Helper.Impersonate(windowsUserName, domainName, password)
                        File.Delete(path:=FilePath)
                    End Using
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to delete TempPath '{0}' after uploading as a blob '{1}'.", arg0:=FilePath, arg1:=BlobName.ToString()), innerException:=Exception)
                End Try

                SyncLock Me.TasksFileSyncUpload
                    If Me.TasksFileSyncUpload.LongCount(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = ShareId) > 0 Then
                        Me.TasksFileSyncUpload.Remove(key:=Me.TasksFileSyncUpload.First(Function(x) x.Key.Key.Equals(g:=BlobName) AndAlso x.Key.Value = ShareId).Key)
                    End If
                End SyncLock
            Else
                Throw New Exception(message:=String.Format(format:="Unable to upload blob '{0}'.", arg0:=BlobName.ToString()))
            End If
        Catch Exception As Exception
            LoggerFileSync.Error(String.Format(format:="Unable to upload blob '{0}'.", arg0:=BlobName.ToString()), Exception)

            Throw Exception
        End Try
    End Sub

    Private Function TaskFileSyncUpload_DoTask(AccountId As Guid, UserAccountId As Integer, AccessToken As String, ShareId As Integer, SharePath As String, BlobName As Guid, user As LocalWorkSpaceInfo, fileSystemEntryId As Guid, filesize As Long, windowsUserName As String, domainName As String, password As String) As Boolean
        'Dim FileSyncServiceClient As FileSyncServiceReference.FileSyncServiceClient = New FileSyncServiceReference.FileSyncServiceClient(endpointConfigurationName:="WSHttpBinding_IFileSyncService", remoteAddress:=FileMinisterFileSyncServiceURL)
        Dim CloudBlockBlob As CloudBlockBlob = Nothing
        Dim syncClient = New SyncClient(user)
        Try
            'Dim FileMinisterProxyWrapper As New FileMinisterProxyWrapper(Of FileSyncServiceReference.FileSyncServiceClient, FileSyncServiceReference.IFileSyncService)(
            '    client:=FileSyncServiceClient,
            '    user:=New UserInfo() With {.AccountId = AccountId, .UserAccountId = UserAccountId, .AccessToken = AccessToken},
            '    invokeMethod:=Sub(ServiceClient)
            '                      CloudBlockBlob = New CloudBlockBlob(blobAbsoluteUri:=ServiceClient.GetSharedAccessSignatureUrl(ShareId:=ShareId, BlobName:=BlobName, SharedAccessSignatureType:=FileSyncServiceReference.SharedAccessSignatureType.Upload))
            '                  End Sub)
            Dim res = syncClient.GetSharedAccessSignatureUrl(ShareId:=ShareId, BlobName:=BlobName, SharedAccessSignatureType:=SharedAccessSignatureType.Upload, fileSystemEntryId:=fileSystemEntryId, filesize:=filesize)
            If (res.Status = 200 AndAlso res.Data IsNot Nothing) Then
                CloudBlockBlob = New CloudBlockBlob(blobAbsoluteUri:=res.Data)
            Else
                Throw New Exception(res.Message)
            End If
        Catch Exception As Exception
            Throw New Exception(message:=String.Format(format:="Unable to upload blob '{0}'.", arg0:=BlobName.ToString()), innerException:=Exception)
        End Try

        ' Pallav: Commented this condition as we still have to try and upload even if file exists on server. 
        'If (Not CloudBlockBlob Is Nothing) AndAlso (Not CloudBlockBlob.Exists()) Then
        Dim CloudBlockBlobMetadata As Dictionary(Of String, String) = CType(CloudBlockBlob.Metadata(), Dictionary(Of String, String))

        If (Not CloudBlockBlobMetadata.ContainsKey(key:=MetadataKeyIsUploadFinished)) OrElse (Not CloudBlockBlobMetadata(key:=MetadataKeyIsUploadFinished).Equals(value:=Boolean.TrueString, comparisonType:=StringComparison.InvariantCultureIgnoreCase)) Then
            Dim CloudBlockBlobBlockList As List(Of ListBlockItem) = Nothing

            Try
                CloudBlockBlobBlockList = CType(CloudBlockBlob.DownloadBlockList(blockListingFilter:=BlockListingFilter.All), List(Of ListBlockItem))
            Catch
            End Try

            If CloudBlockBlobBlockList Is Nothing Then
                CloudBlockBlobBlockList = New List(Of ListBlockItem)
            End If

            Dim FilePath As String = Path.Combine(path1:=Path.Combine(path1:=SharePath, path2:=Enums.Constants.TEMP_FOLDER_NAME), path2:=BlobName.ToString())
            Dim BlockIDs As List(Of String) = New List(Of String)
            Dim BlockCount As Integer? = Nothing
            Dim CloudBlockBlobPutBlockFailureCount As Integer = 0
            Dim FileHash As String = String.Empty

            Try
                Using CommonUtils.Helper.Impersonate(windowsUserName, domainName, password)
                    Using FileStream As FileStream = New FileStream(path:=FilePath, mode:=FileMode.Open, access:=FileAccess.Read, share:=FileShare.Read)
                        Dim InFileSize As Single = FileStream.Length

                        If InFileSize = 0 Then
                            Try
                                CloudBlockBlob.UploadFromStream(source:=FileStream)
                            Catch Exception As Exception
                                Throw New Exception(message:=String.Format(format:="Unable to uploaded zero byte blob '{0}'.", arg0:=BlobName.ToString()), innerException:=Exception)
                            End Try

                            FileStream.Position = 0

                            FileHash = Convert.ToBase64String(inArray:=MD5CryptoServiceProvider.Create().ComputeHash(inputStream:=FileStream))

                            Try
                                CloudBlockBlob.Properties.ContentMD5 = FileHash

                                CloudBlockBlob.SetProperties()
                            Catch Exception As Exception
                                Throw New Exception(message:=String.Format(format:="Unable to set MD5 checksum for the uploaded blob '{0}'.", arg0:=BlobName.ToString()), innerException:=Exception)
                            End Try
                        Else
                            Dim BlockNumber As Integer = 0
                            Dim BytesLeft As Long = Convert.ToInt64(value:=InFileSize)

                            BlockCount = CInt(Math.Truncate(d:=(InFileSize / UploadBlockSizeInBytes)) + 1)

                            Do
                                BlockNumber += 1

                                Dim BytesToRead As Integer
                                If BytesLeft >= UploadBlockSizeInBytes Then
                                    BytesToRead = UploadBlockSizeInBytes
                                Else
                                    BytesToRead = CInt(BytesLeft)
                                End If

                                Dim Bytes(BytesToRead - 1) As Byte

                                FileStream.Read(array:=Bytes, offset:=0, count:=BytesToRead)

                                Dim BlockHash As String = Convert.ToBase64String(inArray:=MD5CryptoServiceProvider.Create().ComputeHash(buffer:=Bytes, offset:=0, count:=BytesToRead))

                                Dim BlockId As String = Convert.ToBase64String(inArray:=Text.ASCIIEncoding.ASCII.GetBytes(String.Format(format:="BlockId={0},BlockHash={1}", arg0:=BlockNumber.ToString("0000000000"), arg1:=BlockHash)))

                                Dim ListBlockItem As ListBlockItem = CloudBlockBlobBlockList.FirstOrDefault(Function(x) x.Name.Equals(value:=BlockId, comparisonType:=StringComparison.InvariantCulture))

                                If ListBlockItem Is Nothing OrElse ListBlockItem.Length <> BytesToRead Then
                                    Try
                                        CloudBlockBlob.PutBlock(blockId:=BlockId, blockData:=New MemoryStream(Bytes), contentMD5:=BlockHash)

                                        BlockIDs.Add(item:=BlockId)
                                    Catch
                                        CloudBlockBlobPutBlockFailureCount += 1

                                        If CloudBlockBlobPutBlockFailureCount > MaximumPutBlockRetryCount Then
                                            Exit Do
                                        End If
                                    End Try
                                Else
                                    BlockIDs.Add(item:=BlockId)
                                End If

                                BytesLeft -= BytesToRead
                            Loop While BytesLeft > 0

                            If BytesLeft = 0 Then
                                Try
                                    FileStream.Position = 0

                                    FileHash = Convert.ToBase64String(inArray:=MD5CryptoServiceProvider.Create().ComputeHash(inputStream:=FileStream))
                                Catch
                                End Try
                            End If
                        End If
                    End Using
                End Using
            Catch Exception As Exception
                Throw New Exception(message:=String.Format(format:="Unable to upload blob '{0}' as TempPath '{1}' is not accessible.", arg0:=BlobName.ToString(), arg1:=FilePath), innerException:=Exception)
            End Try

            If CloudBlockBlobPutBlockFailureCount >= MaximumPutBlockRetryCount Then
                Throw New Exception(message:=String.Format(format:="Unable to upload blob '{0}' as maximum PutBlock failure count has reached.", arg0:=BlobName.ToString()))
            End If

            If (Not BlockCount Is Nothing) AndAlso BlockIDs.Count = BlockCount.GetValueOrDefault() AndAlso (Not String.IsNullOrWhiteSpace(FileHash)) Then
                Try
                    CloudBlockBlob.PutBlockList(blockList:=BlockIDs)
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to reassemble the uploaded blob '{0}'.", arg0:=BlobName.ToString()), innerException:=Exception)
                End Try

                Try
                    CloudBlockBlob.Properties.ContentMD5 = FileHash

                    CloudBlockBlob.SetProperties()
                Catch Exception As Exception
                    Throw New Exception(message:=String.Format(format:="Unable to set MD5 checksum for the uploaded blob '{0}'.", arg0:=BlobName.ToString()), innerException:=Exception)
                End Try
            End If
            'Else 
            'Pallav - Comented this condition as we should not throw an exception if he file is already uploaded on the server and upload is complete
            '    Throw New Exception(message:=String.Format(format:="Unable to upload blob '{0}' as either blob already exists or access is not granted.", arg0:=BlobName.ToString()))
        End If
        'Else
        'Throw New Exception(message:=String.Format(format:="Unable to upload blob '{0}' as blob already exists.", arg0:=BlobName.ToString()))
        'End If

        Return True
    End Function

    Private Sub TimerFileSyncConflictUpload_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs)
        Dim DataTableUserAccounts As DataTable = Nothing
        Dim DataTableUserShares As DataTable = Nothing

        FileMinisterService.GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerFileSync)

        If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
            If DataTableUserAccounts.Rows.Count = 0 Then
                LoggerFileSync.Debug("Unable to find last logged-in User.")
            Else
                If DataTableUserShares.Rows.Count = 0 Then
                    LoggerFileSync.Debug("Unable to find any mapped share.")
                Else
                    For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
                        Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

                        If (DataRowsUserShares.Length > 0) Then
                            For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
                                If DataRowsUserSharesByUserId.Count() > 0 Then
                                    For Each DataRow As DataRow In DataRowsUserSharesByUserId
                                        Dim BackgroundWorkerFileSyncConflictUploadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=New Guid(DataRowsUserSharesByUserId.Key.ToString), ShareId:=CInt(DataRow(columnName:="ShareId")))

                                        SyncLock Me.BackgroundWorkersFileSyncConflictUpload
                                            If Not Me.BackgroundWorkersFileSyncConflictUpload.ContainsKey(key:=BackgroundWorkerFileSyncConflictUploadKey) Then
                                                Dim BackgroundWorkerFileSyncConflictUpload As BackgroundWorker = New BackgroundWorker()

                                                With BackgroundWorkerFileSyncConflictUpload
                                                    AddHandler .DoWork, AddressOf Me.BackgroundWorkerFileSyncConflictUpload_DoWork
                                                    AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerFileSyncConflictUpload_RunWorkerCompleted
                                                End With

                                                Me.BackgroundWorkersFileSyncConflictUpload.Add(key:=BackgroundWorkerFileSyncConflictUploadKey, value:=BackgroundWorkerFileSyncConflictUpload)

                                                BackgroundWorkerFileSyncConflictUpload.RunWorkerAsync(argument:=New FileBackgroundWorkerArguments With {
                                                                                        .UserAccountId = CInt(DataRowUserAccounts(columnName:="UserAccountId")),
                                                                                        .AccountId = New Guid(DataRowUserAccounts(columnName:="AccountId").ToString),
                                                                                        .CloudSyncServiceURL = DataRowUserAccounts(columnName:="CloudSyncServiceURL").ToString(),
                                                                                        .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
                                                                                        .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
                                                                                        .WindowsUserSID = DataRowUserAccounts(columnName:="WindowsUserSID").ToString(),
                                                                                        .WorkSpaceId = New Guid(DataRowUserAccounts(columnName:="WorkSpaceId").ToString()),
                                                                                        .UserId = New Guid(DataRowsUserSharesByUserId.Key.ToString),
                                                                                        .ShareId = CInt(DataRow(columnName:="ShareId")),
                                                                                        .SharePath = DataRow(columnName:="SharePath").ToString(),
                                                                                        .WindowsUser = If(DataRow.IsNull("WindowsUser"), Nothing, DataRow(columnName:="WindowsUser").ToString()),
                                                                                        .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1)),
                                                                                        .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0)),
                                                                                        .Password = CommonUtils.Helper.Decrypt(If(DataRow.IsNull("Password"), Nothing, DataRow(columnName:="Password").ToString())),
                                                                                        .RoleId = CInt(DataRowUserAccounts(columnName:="RoleId"))
                                                                                    })
                                            End If
                                        End SyncLock
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If
        Me.TimerFileSyncConflictUpload.Start()
    End Sub

    Private Sub BackgroundWorkerFileSyncConflictUpload_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim FileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim BackgroundWorkerFileSyncConflictUploadKey As String = Me.GetBackgroundWorkerFileSyncKey(UserAccountId:=FileBackgroundWorkerArguments.UserAccountId, UserId:=FileBackgroundWorkerArguments.UserId, ShareId:=FileBackgroundWorkerArguments.ShareId)

        Try
            Dim userInfo = New LocalWorkSpaceInfo()
            With userInfo
                .UserId = FileBackgroundWorkerArguments.UserId
                .AccessToken = FileBackgroundWorkerArguments.AccessToken
                .AccountId = FileBackgroundWorkerArguments.AccountId
                .WindowsUserSId = FileBackgroundWorkerArguments.WindowsUserSID
                .RoleId = CType(FileBackgroundWorkerArguments.RoleId, Role)
                .CloudSyncServiceUrl = FileBackgroundWorkerArguments.CloudSyncServiceURL
                .WorkSpaceId = FileBackgroundWorkerArguments.WorkSpaceId
                .UserAccountId = FileBackgroundWorkerArguments.UserAccountId
            End With

            Dim FileClient = New FileClient(userInfo)
            Dim result = FileClient.GetConflictFilePendingUpload(userInfo.WorkSpaceId)

            If result.Data IsNot Nothing AndAlso result.Data.Count > 0 Then
                Dim TempPath As String = Path.Combine(FileBackgroundWorkerArguments.SharePath, Enums.Constants.TEMP_FOLDER_NAME)
                Try

                    Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                        Dim DirectoryInfo As DirectoryInfo = New DirectoryInfo(TempPath)
                        If Not DirectoryInfo.Exists Then
                            If Directory.GetParent(path:=TempPath).Exists Then
                                DirectoryInfo = Directory.CreateDirectory(path:=TempPath)

                                DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                            Else
                                Throw New Exception(message:="Parent folder of TempPath '" & TempPath & "' does not exists.")
                            End If
                        Else
                            Try
                                If (DirectoryInfo.Attributes And FileAttributes.Hidden) <> FileAttributes.Hidden Then
                                    DirectoryInfo.Attributes = DirectoryInfo.Attributes Or FileAttributes.Hidden
                                End If
                            Catch Exception As Exception
                                LoggerFileSync.Error("Unable to set hidden attribute to TempPath '" & TempPath & "'.", Exception)
                            End Try
                        End If
                    End Using
                Catch Exception As Exception
                    LoggerFileSync.Error("Unable to create TempPath '" & TempPath & "'.", Exception)
                End Try

                Dim dirExist As Boolean
                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                    dirExist = Directory.Exists(TempPath)
                End Using
                If dirExist Then
                    For Each BlobName As Guid In result.Data
                        Dim LocalReplicaFilePath As String = Path.Combine(TempPath, BlobName.ToString())
                        Dim fileSystemEntryId As Guid
                        Dim fileExist As Boolean
                        Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                            fileExist = File.Exists(LocalReplicaFilePath)
                        End Using
                        If Not fileExist Then
                            'Get Relative path of the file on client
                            Dim FileForUploadPath As String = String.Empty
                            Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=FileBackgroundWorkerArguments.LocalDatabaseName))
                                Dim dt As DataTable = New DataTable()

                                Using FileRelativePathClient As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                                   "SELECT TOP 1" &
                                                                                   "    V.[FileSystemEntryRelativePath]" &
                                                                                   "    , C.[FileSystemEntryId]" &
                                                                                   " FROM [dbo].[FileSystemEntryVersionConflicts] C" &
                                                                                   " JOIN [dbo].[FileSystemEntryVersions] V" &
                                                                                   "    ON C.[FileSystemEntryId] = V.[FileSystemEntryId]" &
                                                                                   " WHERE C.[FileSystemEntryVersionConflictId] = '" & BlobName.ToString() & "'" &
                                                                                   "    AND ISNULL(V.[VersionNumber], 0) = 0" &
                                                                                   " ORDER BY V.[CreatedOnUTC] DESC", selectConnection:=SqlConnectionClient)
                                    Try
                                        With FileRelativePathClient
                                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                                            .MissingSchemaAction = MissingSchemaAction.AddWithKey
                                            .Fill(dt)
                                        End With
                                    Catch Exception As Exception
                                        LoggerFileSync.Error(String.Format("Unable to connect to client database for getting the relativepath of local file for conflict upload of {0}", BackgroundWorkerFileSyncConflictUploadKey), Exception)
                                    End Try

                                    If (dt.Rows.Count = 1) Then
                                        Dim dr = dt.Rows.Cast(Of DataRow).FirstOrDefault()
                                        FileForUploadPath = Path.Combine(FileBackgroundWorkerArguments.SharePath, dr("FileSystemEntryRelativePath").ToString())
                                        fileSystemEntryId = New Guid(dr("FileSystemEntryId").ToString())
                                    Else
                                        LoggerFileSync.Error(String.Format("File Relative Path not found in Client database for conflict upload of {0}", BackgroundWorkerFileSyncConflictUploadKey))
                                    End If

                                End Using
                            End Using

                            Try
                                Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                                    If File.Exists(FileForUploadPath) Then
                                        File.Copy(FileForUploadPath, LocalReplicaFilePath)
                                    End If
                                End Using
                            Catch Exception As Exception
                                LoggerFileSync.Error(String.Format(format:="Error occurred while copying file '{0}' to TempPath '{1}' to start conflict upload process for '{2}'.", arg0:=FileForUploadPath, arg1:=LocalReplicaFilePath, arg2:=BackgroundWorkerFileSyncConflictUploadKey), Exception)
                            End Try
                        End If
                        'Upload the file
                        Using CommonUtils.Helper.Impersonate(FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password)
                            If File.Exists(LocalReplicaFilePath) Then
                                Dim fileInfo = New IO.FileInfo(LocalReplicaFilePath)
                                If Me.TaskFileSyncUpload_DoTask(FileBackgroundWorkerArguments.AccountId, FileBackgroundWorkerArguments.UserAccountId, FileBackgroundWorkerArguments.AccessToken, FileBackgroundWorkerArguments.ShareId, FileBackgroundWorkerArguments.SharePath, BlobName, userInfo, fileSystemEntryId, fileInfo.Length, FileBackgroundWorkerArguments.WindowsUserName, FileBackgroundWorkerArguments.DomainName, FileBackgroundWorkerArguments.Password) Then
                                    ' Update the Status to Uploaded on server
                                    Dim statusresult = FileClient.UpdateConflictUploadStatus(BlobName, fileInfo.Length, HashCalculator.hash_generator("md5", LocalReplicaFilePath))

                                    If Not statusresult.Data Then
                                        LoggerFileSync.Error(String.Format(format:="Error occurred while Changing the status to Uploaded for requested conflict '{0}': msg: {1}.", arg0:=BlobName.ToString(), arg1:=statusresult.Message))
                                    Else
                                        ' Delete the file from temp folder
                                        Try
                                            fileInfo.Delete()
                                        Catch Exception As Exception
                                            LoggerFileSync.Error(String.Format(format:="Unable to delete the file from TempPath '{0}' after uploading the conflicted blob '{1}'.", arg0:=LocalReplicaFilePath, arg1:=BlobName.ToString()), Exception)
                                        End Try
                                    End If
                                Else
                                    LoggerFileSync.Error(String.Format(format:="Unable to upload blob for conflict request '{0}'.", arg0:=BlobName.ToString()))
                                End If
                            End If
                        End Using
                    Next

                End If
            End If
        Catch Exception As Exception
            LoggerFileSync.Error(String.Format(format:="Error occurred while fetching list of files for conflict upload for '{0}'.", arg0:=BackgroundWorkerFileSyncConflictUploadKey), Exception)
        End Try

    End Sub

    Private Sub BackgroundWorkerFileSyncConflictUpload_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
        SyncLock Me.BackgroundWorkersFileSyncConflictUpload
            Dim KeyValuePairBackgroundWorkerFileSyncConflictUpload As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersFileSyncConflictUpload.First(Function(x) x.Value.Equals(obj:=Sender))

            If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
                LoggerFileSync.Error(String.Format(format:="Error occurred while syncing files for '{0}'.", arg0:=KeyValuePairBackgroundWorkerFileSyncConflictUpload.Key), RunWorkerCompletedEventArgs.Error)
            End If

            Me.BackgroundWorkersFileSyncConflictUpload.Remove(key:=KeyValuePairBackgroundWorkerFileSyncConflictUpload.Key)

            KeyValuePairBackgroundWorkerFileSyncConflictUpload.Value.Dispose()
        End SyncLock
    End Sub

    Private Function GetBackgroundWorkerFileSyncKey(UserAccountId As Integer, UserId As Guid, ShareId As Integer, Optional OnDemandSyncFileSystemEntryId As Guid? = Nothing) As String
        Return String.Format("UserAccountId={0}, UserId={1}, ShareId={2}{3}", UserAccountId.ToString(), UserId.ToString(), ShareId.ToString(), If(OnDemandSyncFileSystemEntryId Is Nothing, String.Empty, String.Format(format:=", OnDemandSyncFileSystemEntryId={0}", arg0:=OnDemandSyncFileSystemEntryId.GetValueOrDefault().ToString())))
    End Function

    Private Sub OnFileCheckedOut(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid, IsCheckedOutByDifferentUser As Boolean)
        RaiseEvent FileCheckedOut(UserAccountId, ShareId, FileSystemEntryId, IsCheckedOutByDifferentUser)
    End Sub
#End Region
End Class
