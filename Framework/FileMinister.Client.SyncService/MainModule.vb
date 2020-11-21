Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection
Imports System.Timers
Imports log4net
Imports Microsoft.Synchronization
Imports Microsoft.Synchronization.Data
Imports Microsoft.Synchronization.Data.SqlServer
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Model
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.cloud
Imports risersoft.shared.web
Imports risersoft.shared.sync
Imports risersoft.shared

Public Class FileMinisterService
    Private ReadOnly LoggerSyncService As ILog = LogManager.GetLogger(name:="SyncServiceLogger")

    Private Shared ReadOnly CommandTimeoutInSeconds As Integer = Integer.Parse(ConfigurationManager.AppSettings(name:="CommandTimeoutInSeconds"))

    Private Shared ReadOnly DatabaseSyncServiceIntervalInMilliSeconds As Double = Double.Parse(ConfigurationManager.AppSettings(name:="DatabaseSyncServiceIntervalInMilliSeconds"))

    Private Shared ReadOnly LoggerDatabaseSync As ILog = LogManager.GetLogger(name:="DatabaseSyncLogger")
    Private Shared ReadOnly FileMinisterServiceInstance As FileMinisterService

    Private ReadOnly TimerDatabaseSync As Timer

    Private Shared fileSyncService As FileSyncModule

    Shared Sub New()
        FileMinisterServiceInstance = New FileMinisterService()
        fileSyncService = FileSyncModule.GetInstance
    End Sub

    Private Sub New()
        Me.TimerDatabaseSync = New Timer(interval:=DatabaseSyncServiceIntervalInMilliSeconds)
        With Me.TimerDatabaseSync
            .AutoReset = False
            .Enabled = False
            AddHandler .Elapsed, AddressOf Me.TimerDatabaseSync_Elapsed
        End With

        'Me.TimerFileSyncMoveDownloaded = New Timer(interval:=FileSyncMoveDownloadedServiceIntervalInMilliSeconds)
        'With Me.TimerFileSyncMoveDownloaded
        '    .AutoReset = False
        '    .Enabled = False
        '    AddHandler .Elapsed, AddressOf Me.TimerFileSyncMoveDownloaded_Elapsed
        'End With

    End Sub

    Public Shared Function GetInstance() As FileMinisterService
        Return FileMinisterServiceInstance
    End Function

    Public Sub StartService()
        LoggerSyncService.Info("Starting Service...")

        Me.TimerDatabaseSync_Elapsed(Me.TimerDatabaseSync, Nothing)

        LoggerSyncService.Info("Service started.")
    End Sub

    Public Sub StopService()
        LoggerSyncService.Info("Stopping Service...")

        Me.TimerDatabaseSync.Stop()
        fileSyncService.StopService()

        LoggerSyncService.Info("Service stopped.")
    End Sub

    Public Event OnDemandSyncFinished(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
    Public Sub StartOnDemandSync(UserAccountId As Integer, ShareId As Integer, FileSystemEntryId As Guid)
        Me.TimerDatabaseSync_Elapsed(Sender:=Nothing, ElapsedEventArgs:=Nothing, OnDemandSyncFileSystemEntryId:=FileSystemEntryId)
        fileSyncService.StartOnDemandSync(UserAccountId, ShareId, FileSystemEntryId)

        RaiseEvent OnDemandSyncFinished(UserAccountId, ShareId, FileSystemEntryId:=FileSystemEntryId)
    End Sub

#Region "Database Sync"
    Private Shared ReadOnly SyncObjectPrefix As String = ConfigurationManager.AppSettings(name:="SyncObjectPrefix")

    Private ReadOnly BackgroundWorkersDatabaseSync As Dictionary(Of String, BackgroundWorker) = New Dictionary(Of String, BackgroundWorker)

    Private IsInitialDatabaseSync As Boolean = True

    Private Sub TimerDatabaseSync_Elapsed(Sender As Object, ElapsedEventArgs As ElapsedEventArgs, Optional OnDemandSyncFileSystemEntryId As Guid? = Nothing)
        Dim IsBackgroundWorkerDatabaseSyncCreated = False
        Dim DataTableUserAccounts As DataTable = Nothing
        Dim DataTableUserShares As DataTable = Nothing

        GetUserAccountsAndShares(DataTableUserAccounts:=DataTableUserAccounts, DataTableUserShares:=DataTableUserShares, Logger:=LoggerDatabaseSync)

        If Not DataTableUserAccounts Is Nothing AndAlso Not DataTableUserShares Is Nothing Then
            If DataTableUserAccounts.Rows.Count = 0 Then
                LoggerDatabaseSync.Debug("Unable to find last logged-in User.")
            Else
                If DataTableUserShares.Rows.Count = 0 Then
                    LoggerDatabaseSync.Debug("Unable to find any mapped share.")
                Else
                    For Each DataRowUserAccounts As DataRow In DataTableUserAccounts.Rows
                        Dim DataRowsUserShares As DataRow() = DataTableUserShares.Select(filterExpression:="[UserAccountId] = " & DataRowUserAccounts(columnName:="UserAccountId").ToString())

                        If (DataRowsUserShares.Length > 0) Then
                            For Each DataRowsUserSharesByUserId In DataRowsUserShares.GroupBy(Function(x) x(columnName:="UserId"))
                                If DataRowsUserSharesByUserId.Count() > 0 Then
                                    Try
                                        If Me.IsInitialDatabaseSync OrElse fileSyncService.isEnabled Then 'OrElse Me.TimerFileSyncMoveDownloaded.Enabled 
                                            Dim BackgroundWorkerDatabaseSyncKey As String = Me.GetBackgroundWorkerDatabaseSyncKey(UserAccountId:=CInt(DataRowUserAccounts(columnName:="UserAccountId")), UserId:=New Guid(DataRowsUserSharesByUserId.Key.ToString))

                                            SyncLock Me.BackgroundWorkersDatabaseSync
                                                If Not Me.BackgroundWorkersDatabaseSync.ContainsKey(key:=BackgroundWorkerDatabaseSyncKey) Then
                                                    Dim BackgroundWorkerDatabaseSync As BackgroundWorker = New BackgroundWorker()

                                                    With BackgroundWorkerDatabaseSync
                                                        AddHandler .DoWork, AddressOf Me.BackgroundWorkerDatabaseSync_DoWork
                                                        AddHandler .RunWorkerCompleted, AddressOf Me.BackgroundWorkerDatabaseSync_RunWorkerCompleted
                                                    End With

                                                    Me.BackgroundWorkersDatabaseSync.Add(key:=BackgroundWorkerDatabaseSyncKey, value:=BackgroundWorkerDatabaseSync)

                                                    Dim shareCredentials As New Dictionary(Of Integer, ShareCredentials)
                                                    Dim userAcctId As Integer = CInt(DataRowUserAccounts(columnName:="UserAccountId"))
                                                    For Each drShare In DataRowsUserSharesByUserId.Where(Function(p) CInt(p("UserAccountId")) = userAcctId)
                                                        Dim shareId = CInt(drShare(columnName:="ShareId"))
                                                        If (Not shareCredentials.ContainsKey(shareId)) Then
                                                            Dim cred = New ShareCredentials
                                                            With cred
                                                                .WindowsUser = If(drShare.IsNull("WindowsUser"), Nothing, drShare(columnName:="WindowsUser").ToString())
                                                                .WindowsUserName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(1))
                                                                .DomainName = If(.WindowsUser Is Nothing, Nothing, .WindowsUser.Split(CChar("\"))(0))
                                                                .Password = If(drShare.IsNull("Password"), Nothing, drShare(columnName:="Password").ToString())
                                                                .ShareName = drShare(columnName:="ShareName").ToString
                                                                .SharePath = drShare(columnName:="SharePath").ToString
                                                            End With
                                                            shareCredentials.Add(shareId, cred)
                                                        End If
                                                    Next
                                                    BackgroundWorkerDatabaseSync.RunWorkerAsync(argument:=New DatabaseBackgroundWorkerArguments With {
                                                                                                .UserAccountId = userAcctId,
                                                                                                .AccountId = New Guid(DataRowUserAccounts(columnName:="AccountId").ToString),
                                                                                                .AccountName = DataRowUserAccounts(columnName:="AccountName").ToString,
                                                                                                .FileMinisterDatabaseSyncServiceURL = (New Uri(baseUri:=New Uri(uriString:=DataRowUserAccounts(columnName:="CloudSyncSyncServiceURL").ToString()), relativeUri:="DbSyncService.svc")).ToString(),
                                                                                                .CloudSyncServiceURL = DataRowUserAccounts(columnName:="CloudSyncServiceURL").ToString(),
                                                                                                .LocalDatabaseName = DataRowUserAccounts(columnName:="LocalDatabaseName").ToString(),
                                                                                                .AccessToken = CommonUtils.Helper.Decrypt(DataRowUserAccounts(columnName:="AccessToken").ToString()),
                                                                                                .UserId = New Guid(DataRowsUserSharesByUserId.Key.ToString),
                                                                                                .WorkSpaceId = New Guid(DataRowUserAccounts(columnName:="WorkSpaceId").ToString()),
                                                                                                .UserMappedShareIds = JsonConvert.SerializeObject(value:=DataRowsUserSharesByUserId.Select(Function(x) x(columnName:="ShareId")).ToArray()),
                                                                                                .ShareCredentials = shareCredentials,
                                                                                                .WindowsUserSID = DataRowUserAccounts(columnName:="WindowsUserSID").ToString
                                                                                            })

                                                    IsBackgroundWorkerDatabaseSyncCreated = True
                                                End If
                                            End SyncLock
                                        Else
                                            Dim BackgroundWorkersDatabaseSyncCount As Integer = 0

                                            SyncLock Me.BackgroundWorkersDatabaseSync
                                                BackgroundWorkersDatabaseSyncCount = Me.BackgroundWorkersDatabaseSync.Count
                                            End SyncLock

                                            If BackgroundWorkersDatabaseSyncCount = 0 Then
                                                fileSyncService.StartService()
                                            End If
                                        End If
                                    Catch ex As Exception
                                        LoggerDatabaseSync.Info("Error in main process.", ex)
                                    End Try
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        If IsBackgroundWorkerDatabaseSyncCreated AndAlso Me.IsInitialDatabaseSync Then
            Me.IsInitialDatabaseSync = False
        End If

        If Not Sender Is Nothing Then
            Me.TimerDatabaseSync.Start()
        ElseIf Not OnDemandSyncFileSystemEntryId Is Nothing Then
            While True
                Dim BackgroundWorkersDatabaseSyncCount As Integer = 0

                SyncLock Me.BackgroundWorkersDatabaseSync
                    BackgroundWorkersDatabaseSyncCount = Me.BackgroundWorkersDatabaseSync.Count
                End SyncLock

                If BackgroundWorkersDatabaseSyncCount > 0 Then
                    Threading.Thread.Sleep(millisecondsTimeout:=CInt(DatabaseSyncServiceIntervalInMilliSeconds))
                Else
                    Exit While
                End If
            End While
        End If
    End Sub

    Public Sub BackgroundWorkerDatabaseSync_DoWork(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim DatabaseBackgroundWorkerArguments As DatabaseBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, DatabaseBackgroundWorkerArguments)
        Dim BackgroundWorkerDatabaseSyncKey As String = Me.GetBackgroundWorkerDatabaseSyncKey(UserAccountId:=DatabaseBackgroundWorkerArguments.UserAccountId, UserId:=DatabaseBackgroundWorkerArguments.UserId)

        LoggerDatabaseSync.Info(String.Format(format:="Starting Sync for '{0}'...", arg0:=BackgroundWorkerDatabaseSyncKey))

        Dim SyncScopes As ReadOnlyDictionary(Of String, SyncScope) = JsonConvert.DeserializeObject(Of ReadOnlyDictionary(Of String, SyncScope))(value:=ConfigurationManager.AppSettings(name:="SyncScopes"))
        For Each SyncScope As KeyValuePair(Of String, SyncScope) In SyncScopes.Where(Function(x) (Not x.Value.FilterParameters Is Nothing AndAlso x.Value.FilterParameters.Count > 0))
            For Each FilterParameter As FilterParameter In SyncScope.Value.FilterParameters
                Dim PropertyInfo As PropertyInfo = DoWorkEventArgs.Argument.GetType().GetProperty(name:=FilterParameter.Name)

                If Not PropertyInfo Is Nothing Then
                    FilterParameter.Value = PropertyInfo.GetValue(obj:=DatabaseBackgroundWorkerArguments, invokeAttr:=BindingFlags.GetProperty, binder:=Nothing, index:=Nothing, culture:=Nothing).ToString()
                End If
            Next
        Next

        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=DatabaseBackgroundWorkerArguments.LocalDatabaseName))
            For Each SyncScope As KeyValuePair(Of String, SyncScope) In SyncScopes
                Using ClientSqlSyncProvider As ClientSqlSyncProvider = New ClientSqlSyncProvider(BackgroundWorkerDatabaseSyncKey:=BackgroundWorkerDatabaseSyncKey, ScopeName:=SyncScope.Key, SqlConnection:=SqlConnectionClient, SyncObjectPrefix:=SyncObjectPrefix)
                    With ClientSqlSyncProvider
                        .CommandTimeout = CommandTimeoutInSeconds
                        AddHandler .ApplyChangeFailed, AddressOf ClientSqlSyncProvider_ApplyChangeFailed
                    End With

                    Dim ServerDatabaseSyncProviderProxy As ServerDatabaseSyncProviderProxy = New ServerDatabaseSyncProviderProxy(AccountName:=DatabaseBackgroundWorkerArguments.AccountName, AccessToken:=DatabaseBackgroundWorkerArguments.AccessToken, BackgroundWorkerDatabaseSyncKey:=BackgroundWorkerDatabaseSyncKey, ScopeName:=SyncScope.Key, FilterParameters:=SyncScope.Value.FilterParameters, FileMinisterDatabaseSyncServiceURL:=DatabaseBackgroundWorkerArguments.FileMinisterDatabaseSyncServiceURL)

                    AddHandler ServerDatabaseSyncProviderProxy.DestinationCallbacks.ItemConflicting, AddressOf ServerSyncProviderProxy_ItemConflicting

                    Dim SyncOrchestrator As SyncOrchestrator = New SyncOrchestrator()
                    With SyncOrchestrator
                        .LocalProvider = ClientSqlSyncProvider
                        .RemoteProvider = ServerDatabaseSyncProviderProxy
                        .Direction = CType(SyncScope.Value.SyncDirection, SyncDirectionOrder)
                    End With

                    Try
                        LoggerDatabaseSync.Info(String.Format(format:="Starting Sync for '{0}' for '{1}' scope...", arg0:=BackgroundWorkerDatabaseSyncKey, arg1:=SyncScope.Key))

                        Dim SyncOperationStatistics As SyncOperationStatistics = SyncOrchestrator.Synchronize()

                        LoggerDatabaseSync.Debug(
                            "Sync Start Time: " & SyncOperationStatistics.SyncStartTime & vbCrLf &
                            "Sync End Time: " & SyncOperationStatistics.SyncEndTime & vbCrLf &
                            "Sync Time Taken: " & SyncOperationStatistics.SyncEndTime.Subtract(SyncOperationStatistics.SyncStartTime).ToString() & vbCrLf &
                            "Sync Total Changes Downloaded: " & SyncOperationStatistics.DownloadChangesTotal & vbCrLf &
                            "Sync Total Changes Uploaded: " & SyncOperationStatistics.UploadChangesTotal
                        )

                        LoggerDatabaseSync.Info(String.Format(format:="Finished Sync for '{0}' for '{1}' scope.", arg0:=BackgroundWorkerDatabaseSyncKey, arg1:=SyncScope.Key))
                    Catch Exception As Exception
                        With LoggerDatabaseSync
                            .Error(String.Format(format:="Unable to sync for '{0}' for '{1}' scope.", arg0:=BackgroundWorkerDatabaseSyncKey, arg1:=SyncScope.Key), Exception)
                            .Debug(String.Format(format:="Synchronization failed for '{0}' for '{1}' scope.", arg0:=BackgroundWorkerDatabaseSyncKey, arg1:=SyncScope.Key))
                        End With
                    End Try

                    Try
                        ServerDatabaseSyncProviderProxy.CloseSession()
                    Catch
                    End Try
                End Using
            Next
            LoggerDatabaseSync.Info(String.Format(format:="Finished Sync for '{0}'...", arg0:=BackgroundWorkerDatabaseSyncKey))

            For Each shareID In DatabaseBackgroundWorkerArguments.ShareCredentials.Keys
                Dim shareCred = DatabaseBackgroundWorkerArguments.ShareCredentials(shareID)
                Dim LocalConfigInfo As New LocalConfigInfo With {
                            .UserAccountId = DatabaseBackgroundWorkerArguments.UserAccountId,
                            .AccountId = DatabaseBackgroundWorkerArguments.AccountId,
                            .FileShareId = shareID,
                            .ShareName = shareCred.ShareName,
                            .SharePath = shareCred.SharePath,
                            .WindowsUser = shareCred.WindowsUser,
                            .Password = shareCred.Password,
                            .User = New LocalWorkSpaceInfo With {
                                .UserId = DatabaseBackgroundWorkerArguments.UserId,
                                .WorkSpaceId = DatabaseBackgroundWorkerArguments.WorkSpaceId,
                                .AccountId = DatabaseBackgroundWorkerArguments.AccountId,
                                .AccountName = DatabaseBackgroundWorkerArguments.AccountName,
                                .UserAccountId = DatabaseBackgroundWorkerArguments.UserAccountId,
                                .AccessToken = CommonUtils.Helper.Decrypt(DatabaseBackgroundWorkerArguments.AccessToken),
                                .LocalDatabaseName = DatabaseBackgroundWorkerArguments.LocalDatabaseName,
                                .CloudSyncServiceUrl = DatabaseBackgroundWorkerArguments.CloudSyncServiceURL,
                                .CloudSyncSyncServiceUrl = DatabaseBackgroundWorkerArguments.FileMinisterDatabaseSyncServiceURL,
                                .WindowsUserSId = DatabaseBackgroundWorkerArguments.WindowsUserSID
                            }
                         }
                Dim fileBackgroundWorkerArguments = New FileBackgroundWorkerArguments With {
                                                                                                                .IsRunInServiceMode = True,
                                                                                                                .UserAccountId = LocalConfigInfo.UserAccountId,
                                                                                                                .AccountId = LocalConfigInfo.AccountId,
                                                                                                                .LocalDatabaseName = LocalConfigInfo.User.LocalDatabaseName,
                                                                                                                .AccessToken = LocalConfigInfo.User.AccessToken,
                                                                                                                .WorkSpaceId = LocalConfigInfo.User.WorkSpaceId,
                                                                                                                .UserId = LocalConfigInfo.User.UserId,
                                                                                                                .ShareId = LocalConfigInfo.FileShareId,
                                                                                                                .SharePath = LocalConfigInfo.SharePath,
                                                                                                                .WindowsUser = LocalConfigInfo.WindowsUser,
                                                                                                                .WindowsUserName = If(String.IsNullOrWhiteSpace(.WindowsUser), Nothing, .WindowsUser.Split(CChar("\"))(1)),
                                                                                                                .DomainName = If(String.IsNullOrWhiteSpace(.WindowsUser), Nothing, .WindowsUser.Split(CChar("\"))(0)),
                                                                                                                .Password = CommonUtils.Helper.Decrypt(LocalConfigInfo.Password),
                                                                                                                .OnDemandSyncFileSystemEntryId = Nothing,
                                                                                                                .RoleId = CInt(LocalConfigInfo.User.RoleId),
                                                                                                                .CloudSyncServiceURL = LocalConfigInfo.User.CloudSyncSyncServiceUrl
                                                                                                            }
                Dim DoWorkEventArgs1 = New DoWorkEventArgs(argument:=fileBackgroundWorkerArguments)

                Using CommonUtils.Helper.Impersonate(LocalConfigInfo)
                    If Directory.Exists(LocalConfigInfo.SharePath) Then
                        Try
                            fileSyncService.DirectorySync(FileBackgroundWorkerArguments:=fileBackgroundWorkerArguments)
                        Catch ex As Exception
                            LoggerDatabaseSync.Info("error in dir sync", ex)
                        End Try
                        Try
                            FileMinisterService.GetInstance().ResolveConflictsResolvedOnServer(Nothing, DoWorkEventArgs1)
                        Catch ex As Exception
                            LoggerDatabaseSync.Info("error in resolve conflict", ex)
                        End Try
                    End If
                End Using
                Using client As New LocalSyncClient(LocalConfigInfo.User)
                    Try
                        Dim result = client.UpdateFileSystemCache(LocalConfigInfo, 10)
                        If (Not result.Data AndAlso result.Status = Status.PathNotValid) Then
                            Continue For
                        End If
                    Catch ex As Exception
                        LoggerDatabaseSync.Info("error in updating file watcher", ex)
                    End Try
                End Using

                Try
                    fileSyncService.BackgroundWorkerFileSyncMoveDownloaded_DoWork(Sender:=Nothing, DoWorkEventArgs:=DoWorkEventArgs1)
                Catch ex As Exception
                    LoggerDatabaseSync.Info("error in moving files", ex)
                End Try
            Next

        End Using
    End Sub

    Public Sub ResolveConflictsResolvedOnServer(Sender As Object, DoWorkEventArgs As DoWorkEventArgs)
        Dim fileBackgroundWorkerArguments As FileBackgroundWorkerArguments = CType(DoWorkEventArgs.Argument, FileBackgroundWorkerArguments)
        Dim BackgroundWorkerDatabaseSyncKey As String = Me.GetBackgroundWorkerDatabaseSyncKey(UserAccountId:=fileBackgroundWorkerArguments.UserAccountId, UserId:=fileBackgroundWorkerArguments.UserId)
        LoggerDatabaseSync.Info(String.Format(format:="Checking conflict resolved on server for '{0}'...", arg0:=BackgroundWorkerDatabaseSyncKey))
        Using SqlConnectionClient As SqlConnection = New SqlConnection(connectionString:=String.Format(format:=ConfigurationManager.ConnectionStrings(name:="FileMinisterClientConnectionString").ConnectionString, arg0:=fileBackgroundWorkerArguments.LocalDatabaseName))
            Dim conflictDataset As New DataSet
            Try
                'get conflicts that are resolved on server and not handled @ local user
                Using sqlDataAdapterClient As SqlDataAdapter = New SqlDataAdapter("Select C.*, E.ShareId from [FileSystemEntryVersionConflicts] C JOIN [FileSystemEntries] E ON C.FileSystemEntryId=E.FileSystemEntryId WHERE E.ShareId = " + fileBackgroundWorkerArguments.ShareId.ToString() + " AND C.[UserId] = " + fileBackgroundWorkerArguments.UserId.ToString() + " And C.WorkSpaceId='" + fileBackgroundWorkerArguments.WorkSpaceId.ToString() + "' AND C.IsResolved = 1 AND C.ResolvedByUserId<>" + fileBackgroundWorkerArguments.UserId.ToString() + " AND C.IsActionTaken= 0", SqlConnectionClient)
                    With sqlDataAdapterClient
                        .Fill(conflictDataset)
                    End With
                End Using
                For Each drConflict As DataRow In conflictDataset.Tables(0).Rows
                    Try
                        'Dim cred As ShareCredentials = DatabaseBackgroundWorkerArguments.ShareCredentials.Item(CInt(drConflict("ShareId")))
                        'remove delta and null version records
                        Dim deltaDataset As New DataSet
                        Using sqlDataAdapterClient As SqlDataAdapter = New SqlDataAdapter("Select * from [DeltaOperations] where FileSystemEntryId = '" + drConflict.Item("FileSystemEntryId").ToString() + "'", SqlConnectionClient)
                            With sqlDataAdapterClient
                                .Fill(deltaDataset)
                            End With
                        End Using

                        Try
                            Dim filesToBeDeleted = deltaDataset.Tables(0).Rows.Cast(Of DataRow).Where(Function(p) Not String.IsNullOrWhiteSpace(p("LocalFileSystemEntryName").ToString())).Select(Function(p) p("LocalFileSystemEntryName").ToString()).Distinct().ToList()
                            'For Each drDelta As DataRow In deltaDataset.Tables(0).Rows
                            '    Dim filePath = drDelta.Item("LocalFileSystemEntryName").ToString()
                            '    If (File.Exists(filePath)) Then
                            '        File.Delete(filePath)
                            '    End If
                            'Next
                            If (SqlConnectionClient.State <> ConnectionState.Open) Then
                                SqlConnectionClient.Open()
                            End If

                            If (CType(Enums.FileVersionConflictType.ClientDelete, Byte).ToString.Equals(drConflict.Item("FileSystemEntryVersionConflictTypeId").ToString()) AndAlso drConflict.Item("ResolvedType").ToString().Equals(Enums.Constants.SERVER_WIN_RESOLVETYPE)) Then
                                Using sqlCommand As SqlCommand = New SqlCommand("UPDATE FSEV SET [ServerFileSystemEntryName] = NEWID() from FileSystemEntryVersions FSEV join DeltaOperations D On D.FileSystemEntryVersionId = FSEV.FileSystemEntryVersionId JOIN [dbo].[FileSystemEntryLinks] L ON L.FileSystemEntryId = D.FileSystemEntryId Where L.PreviousFileSystemEntryId = '" +
                                    drConflict.Item("FileSystemEntryId").ToString() + "'; UPDATE D SET D.[FileSystemEntryStatusId] = 5 from DeltaOperations D JOIN [dbo].[FileSystemEntryLinks] L ON L.FileSystemEntryId = D.FileSystemEntryId Where L.PreviousFileSystemEntryId = '" +
                                    drConflict.Item("FileSystemEntryId").ToString() + "'", SqlConnectionClient)
                                    '" + drConflict.Item("FileSystemEntryId").ToString() + "'; delete from [FileSystemEntryVersions] where FileSystemEntryId = '" + drConflict.Item("FileSystemEntryId").ToString() + "'; ", SqlConnectionClient)
                                    sqlCommand.ExecuteNonQuery()
                                End Using

                                Using sqlCommand As SqlCommand = New SqlCommand("DELETE FROM [FileSystemEntryLinks] where [PreviousFileSystemEntryId] = '" + drConflict.Item("FileSystemEntryId").ToString() + "'", SqlConnectionClient)
                                    sqlCommand.ExecuteNonQuery()
                                End Using
                            ElseIf (CType(Enums.FileVersionConflictType.ServerDelete, Byte).ToString.Equals(drConflict.Item("FileSystemEntryVersionConflictTypeId").ToString()) AndAlso drConflict.Item("ResolvedType").ToString().Equals(Enums.Constants.CLIENT_WIN_RESOLVETYPE)) Then
                                Using sqlCommand As SqlCommand = New SqlCommand("DELETE FROM [FileSystemEntryLinks] where [PreviousFileSystemEntryId] = '" + drConflict.Item("FileSystemEntryId").ToString() + "'", SqlConnectionClient)
                                    sqlCommand.ExecuteNonQuery()
                                End Using
                            End If

                            Using sqlCommand As SqlCommand = New SqlCommand("DELETE FROM [DeltaOperations] where FileSystemEntryId = '" + drConflict.Item("FileSystemEntryId").ToString() + "'; delete from [FileSystemEntryVersions] where FileSystemEntryId = '" + drConflict.Item("FileSystemEntryId").ToString() + "' AND (VersionNumber is NULL or VersionNumber=0)", SqlConnectionClient)
                                sqlCommand.ExecuteNonQuery()
                            End Using
                            Using CommonUtils.Helper.Impersonate(fileBackgroundWorkerArguments.WindowsUserName, fileBackgroundWorkerArguments.DomainName, fileBackgroundWorkerArguments.Password)
                                For Each filePath In filesToBeDeleted
                                    If (File.Exists(filePath)) Then
                                        File.Delete(filePath)
                                    End If
                                Next
                            End Using
                            Using sqlCommand As SqlCommand = New SqlCommand("Update FileSystemEntryVersionConflicts Set IsActionTaken=1 where [FileSystemEntryVersionConflictId] = '" + drConflict.Item("FileSystemEntryVersionConflictId").ToString() + "'", SqlConnectionClient)
                                sqlCommand.ExecuteNonQuery()
                            End Using
                        Finally
                            If SqlConnectionClient.State <> ConnectionState.Closed Then
                                SqlConnectionClient.Close()
                            End If
                        End Try

                    Catch ex As Exception
                        LoggerDatabaseSync.Error(String.Format(format:="error resolving conflict resolved on server for '{0}' and conflictid...", arg0:=BackgroundWorkerDatabaseSyncKey, arg1:=drConflict.Item("FileSystemEntryVersionConflictId").ToString()), ex)
                    End Try
                Next
            Catch ex As Exception
                LoggerDatabaseSync.Error(String.Format(format:="error resolving conflict resolved on server for '{0}'...", arg0:=BackgroundWorkerDatabaseSyncKey), ex)
            End Try
        End Using
    End Sub

    Private Sub BackgroundWorkerDatabaseSync_RunWorkerCompleted(Sender As Object, RunWorkerCompletedEventArgs As RunWorkerCompletedEventArgs)
        SyncLock Me.BackgroundWorkersDatabaseSync
            Dim KeyValuePairBackgroundWorkerDatabaseSync As KeyValuePair(Of String, BackgroundWorker) = Me.BackgroundWorkersDatabaseSync.First(Function(x) x.Value.Equals(obj:=Sender))

            If Not RunWorkerCompletedEventArgs.Error Is Nothing Then
                LoggerDatabaseSync.Error(String.Format(format:="Error occurred while Syncing database for '{0}'.", arg0:=KeyValuePairBackgroundWorkerDatabaseSync.Key), RunWorkerCompletedEventArgs.Error)
            End If

            Me.BackgroundWorkersDatabaseSync.Remove(key:=KeyValuePairBackgroundWorkerDatabaseSync.Key)

            KeyValuePairBackgroundWorkerDatabaseSync.Value.Dispose()
        End SyncLock
    End Sub

    Private Sub ClientSqlSyncProvider_ApplyChangeFailed(Sender As Object, DbApplyChangeFailedEventArgs As DbApplyChangeFailedEventArgs)
        LoggerDatabaseSync.Debug(String.Format("Conflict of type '{0}' was detected at local database for '{1}' while '{2}' for table '{3}' with primary key '{4}'{5}", DbApplyChangeFailedEventArgs.Conflict.Type.ToString(), CType(Sender, ClientSqlSyncProvider).BackgroundWorkerDatabaseSyncKey, DbApplyChangeFailedEventArgs.Conflict.Stage.ToString(), DbApplyChangeFailedEventArgs.Conflict.RemoteChange.TableName, String.Join(separator:=", ", values:=DbApplyChangeFailedEventArgs.Conflict.RemoteChange.PrimaryKey.Select(Function(x) "[" & x.ColumnName & "]=[" & DbApplyChangeFailedEventArgs.Conflict.RemoteChange.Rows(index:=0)(columnName:=x.ColumnName).ToString() & "]")), If(String.IsNullOrWhiteSpace(DbApplyChangeFailedEventArgs.Conflict.ErrorMessage), ".", " with following message: " & DbApplyChangeFailedEventArgs.Conflict.ErrorMessage)), DbApplyChangeFailedEventArgs.Error)

        If DbApplyChangeFailedEventArgs.Conflict.Type <> DbConflictType.ErrorsOccurred Then
            DbApplyChangeFailedEventArgs.Action = ApplyAction.RetryWithForceWrite
        End If
    End Sub

    Private Sub ServerSyncProviderProxy_ItemConflicting(Sender As Object, ItemConflictingEventArgs As ItemConflictingEventArgs)
        'ToDo: ASHISH: CType(Sender, ServerSyncProviderProxy).BackgroundWorkerDatabaseSyncKey
    End Sub

    Private Class ClientSqlSyncProvider
        Inherits SqlSyncProvider

        Private _BackgroundWorkerDatabaseSyncKey As String

        Public ReadOnly Property BackgroundWorkerDatabaseSyncKey As String
            Get
                Return Me._BackgroundWorkerDatabaseSyncKey
            End Get
        End Property

        Public Sub New(BackgroundWorkerDatabaseSyncKey As String, ScopeName As String, SqlConnection As SqlConnection, SyncObjectPrefix As String)
            MyBase.New(scopeName:=ScopeName, connection:=SqlConnection, objectPrefix:=SyncObjectPrefix)

            Me._BackgroundWorkerDatabaseSyncKey = BackgroundWorkerDatabaseSyncKey
        End Sub
    End Class

    Private Class ServerDatabaseSyncProviderProxy
        Inherits KnowledgeSyncProvider
        Implements IServiceAuthorizer

        Private UserInfo As LocalWorkSpaceInfo
        Private _BackgroundWorkerDatabaseSyncKey As String
        Private ScopeName As String
        Private FilterParameters As List(Of FilterParameter)
        Private DatabaseSyncServiceClient As IDatabaseSyncService
        Private SyncIdFormatGroup As SyncIdFormatGroup = Nothing

        Public ReadOnly Property BackgroundWorkerDatabaseSyncKey As String
            Get
                Return Me._BackgroundWorkerDatabaseSyncKey
            End Get
        End Property

        Public Sub New(AccountName As String, AccessToken As String, BackgroundWorkerDatabaseSyncKey As String, ScopeName As String, FilterParameters As List(Of FilterParameter), FileMinisterDatabaseSyncServiceURL As String)
            Me.UserInfo = New LocalWorkSpaceInfo() With {.AccountName = AccountName, .AccessToken = AccessToken}
            Dim client = ChannelProxyFactory.CreateWS(Of IDatabaseSyncService)(FileMinisterDatabaseSyncServiceURL, Me)

            With Me
                ._BackgroundWorkerDatabaseSyncKey = BackgroundWorkerDatabaseSyncKey
                .ScopeName = ScopeName
                .FilterParameters = FilterParameters
                .DatabaseSyncServiceClient = client
            End With
        End Sub

        Public Overrides ReadOnly Property IdFormats As SyncIdFormatGroup
            Get
                If Me.SyncIdFormatGroup Is Nothing Then
                    Me.SyncIdFormatGroup = New SyncIdFormatGroup()

                    With Me.SyncIdFormatGroup
                        .ChangeUnitIdFormat.IsVariableLength = False
                        .ChangeUnitIdFormat.Length = 1
                        .ReplicaIdFormat.IsVariableLength = False
                        .ReplicaIdFormat.Length = 16
                        .ItemIdFormat.IsVariableLength = True
                        .ItemIdFormat.Length = 10240
                    End With
                End If

                Return Me.SyncIdFormatGroup
            End Get
        End Property

        Public Overrides Sub BeginSession(SyncProviderPosition As SyncProviderPosition, SyncSessionContext As SyncSessionContext)
            Me.DatabaseSyncServiceClient.BeginSession(Me.ScopeName, Me.FilterParameters)
        End Sub

        Public Overrides Sub GetSyncBatchParameters(ByRef BatchSize As UInteger, ByRef SyncKnowledge As SyncKnowledge)
            Me.DatabaseSyncServiceClient.GetSyncBatchParameters(BatchSize, SyncKnowledge)
        End Sub

        Public Overrides Function GetChangeBatch(BatchSize As UInteger, SyncKnowledgeDestination As SyncKnowledge, ByRef ChangeDataRetriever As Object) As ChangeBatch
            Return Me.DatabaseSyncServiceClient.GetChangeBatch(BatchSize, SyncKnowledgeDestination, ChangeDataRetriever)
        End Function

        Public Overrides Sub ProcessChangeBatch(ConflictResolutionPolicy As ConflictResolutionPolicy, ChangeBatchSource As ChangeBatch, ChangeDataRetriever As Object, SyncCallbacks As SyncCallbacks, SyncSessionStatistics As SyncSessionStatistics)
            Me.DatabaseSyncServiceClient.ProcessChangeBatch(ConflictResolutionPolicy, ChangeBatchSource, ChangeDataRetriever, SyncSessionStatistics)
        End Sub

        Public Overrides Sub EndSession(SyncSessionContext As SyncSessionContext)
            Me.DatabaseSyncServiceClient.EndSession()
        End Sub

        Public Sub CloseSession()
            Me.DatabaseSyncServiceClient.CloseSession()
        End Sub

        Public Overrides Function GetFullEnumerationChangeBatch(BatchSize As UInteger, SyncId As SyncId, SyncKnowledgeForDataRetrieval As SyncKnowledge, ByRef ChangeDataRetriever As Object) As FullEnumerationChangeBatch
            Throw New NotImplementedException(message:="Function 'GetFullEnumerationChangeBatch' has not been implemented on ServerSyncProviderProxy.")
        End Function

        Public Overrides Sub ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy As ConflictResolutionPolicy, FullEnumerationChangeBatch As FullEnumerationChangeBatch, ChangeDataRetriever As Object, SyncCallbacks As SyncCallbacks, SyncSessionStatistics As SyncSessionStatistics)
            Throw New NotImplementedException(message:="Method 'ProcessFullEnumerationChangeBatch' has not been implemented on ServerSyncProviderProxy.")
        End Sub

        Public Function GetAuthorizationHeader() As Dictionary(Of String, String) Implements IServiceAuthorizer.GetAuthorizationHeader
            Dim dic As New clsCollecString(Of String)
            dic.AddUpd("Authorization", "Bearer " & UserInfo.AccessToken)
            dic.AddUpd("AccountName", UserInfo.AccountName)
            Return dic
        End Function
    End Class

    Private Function GetBackgroundWorkerDatabaseSyncKey(UserAccountId As Integer, UserId As Guid) As String
        Return String.Format(format:="UserAccountId={0}, UserId={1}", arg0:=UserAccountId.ToString(), arg1:=UserId.ToString())
    End Function
#End Region

    Public Shared Sub GetUserAccountsAndShares(ByRef DataTableUserAccounts As DataTable, ByRef DataTableUserShares As DataTable, Logger As ILog)
        Dim dbPath As String = System.IO.Path.Combine(Globals.myApp.objAppExtender.CommonAppDataPath, "FileMinisterClientCommon.mdf")
        Dim strConn As String = String.Format(ConfigurationManager.ConnectionStrings("FileMinisterClientCommonConnectionString").ConnectionString, dbPath)
        Using SqlConnectionClientCommon As SqlConnection = New SqlConnection(strConn)
            Using DataSetClientCommon As DataSet = New DataSet()
                Using SqlDataAdapterClientCommon As SqlDataAdapter = New SqlDataAdapter(selectCommandText:=
                                                                     "/*DECLARE @WindowsUserSID [NVARCHAR](50);" &
                                                                     "SELECT TOP 1" &
                                                                     "    @WindowsUserSID = [UserAccounts].[WindowsUserSID]" &
                                                                     " FROM" &
                                                                     "    [UserAccounts]" &
                                                                     " ORDER BY" &
                                                                     "    [UserAccounts].[LastLoggedInUTC] DESC;*/" &
                                                                     "SELECT" &
                                                                     "    [UserAccounts].[UserAccountId]" &
                                                                     "    , [UserAccounts].[AccountId]" &
                                                                     "    , [UserAccounts].[AccountName]" &
                                                                     "    , [UserAccounts].[CloudSyncServiceURL]" &
                                                                     "    , [UserAccounts].[CloudSyncSyncServiceURL]" &
                                                                     "    , [UserAccounts].[LocalDatabaseName]" &
                                                                     "    , [UserAccounts].[AccessToken]" &
                                                                     "    , [UserAccounts].[WindowsUserSID]" &
                                                                     "    , [UserAccounts].[RoleId]" &
                                                                     "    , [UserAccounts].[WorkSpaceId]" &
                                                                     " FROM" &
                                                                     "    [UserAccounts]" &
                                                                     " WHERE [UserAccounts].[AccessToken] IS NOT NULL;" &
                                                                     " /*WHERE" &
                                                                     "    [UserAccounts].[WindowsUserSID] = @WindowsUserSID;*/" &
                                                                     "SELECT" &
                                                                     "      UA.[UserAccountId]" &
                                                                     "    , UA.[UserId]" &
                                                                     "    , US.[ShareId]" &
                                                                     "    , ACS.ShareName" &
                                                                     "    , US.[SharePath]" &
                                                                     "    , US.[WindowsUser]" &
                                                                     "    , US.[Password]" &
                                                                     " FROM" &
                                                                     "    [dbo].[UserAccounts] UA" &
                                                                     "    INNER JOIN [dbo].[UserShares] US ON UA.[UserAccountId] = US.[UserAccountId]" &
                                                                     "    INNER JOIN dbo. AccountShares ACS ON ACS.ShareId=US.ShareId AND ACS.AccountId=UA.AccountId" &
                                                                     " /*WHERE" &
                                                                     "    [UserAccounts].[WindowsUserSID] = @WindowsUserSID" &
                                                                     "    AND [UserShares].[WindowsUser] = @WindowsUserSID;*/", selectConnection:=SqlConnectionClientCommon)
                    Try
                        With SqlDataAdapterClientCommon
                            .SelectCommand.CommandTimeout = CommandTimeoutInSeconds
                            .Fill(dataSet:=DataSetClientCommon)
                        End With

                        With DataSetClientCommon
                            DataTableUserAccounts = .Tables(0)
                            DataTableUserShares = .Tables(1)
                        End With
                    Catch Exception As Exception
                        Logger.Error("Unable to connect to common database.", Exception)
                    End Try
                End Using
            End Using
        End Using
    End Sub

    Private Class SyncScope
        <JsonConverter(GetType(StringEnumConverter))> Public Property SyncDirection As SyncDirection
        Public Property FilterParameters As List(Of FilterParameter)
    End Class

    Private Enum SyncDirection As Byte
        DownloadAndUpload = 1
        Download = 3
    End Enum
End Class
