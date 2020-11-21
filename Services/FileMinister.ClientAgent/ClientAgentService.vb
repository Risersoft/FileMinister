Imports System.Timers
Imports System.Diagnostics
Imports System.Configuration
Imports FileMinister.Models.Sync
Imports FileMinister.ServiceProxy
Imports risersoft.shared.portable
Imports System.IO
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports FileMinister.Models.Enums

Public Class ClientAgentService
    Inherits System.ServiceProcess.ServiceBase

    Private Property IsServiceStopping As Boolean = False
    Private Property IsFirstRun As Boolean = True

    Dim timer As New Timer()
    Dim tokenHelper As ServiceTokenHelper
    Dim appSettingsHT As Hashtable
    Dim prevOpenHandles As List(Of FileHandleInfo)
    Dim workSpace As WorkSpaceInfo
    Dim shares As List(Of LocalShareInfo)
    Dim userHT As Hashtable
    Dim accountAdminSIDs As New List(Of String)
    Dim fileMinisterSIDs As New List(Of String)

    Public Sub New()

        InitializeComponent()

        timer.Interval = Int32.Parse(ConfigurationManager.AppSettings("TimerInterval"))
        AddHandler timer.Elapsed, New System.Timers.ElapsedEventHandler(AddressOf Me.OnTimer)

    End Sub
    Private Sub TestStartupAndStop(args() As String)
        Me.IsFirstRun = True
        Me.RunProcess()
        timer.Start()
        Console.ReadKey()
        'Me.OnStop()
    End Sub

    Protected Sub OnTimer(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        timer.Stop()

        RunProcess()

        timer.Start()
    End Sub

    Private Sub RunProcess()
        Try
            If Me.IsFirstRun Then
                ValidateAndLoadInitialData()
                Me.IsFirstRun = False
            End If

            'Validate and refresh Tokens
            tokenHelper.ValidateAndRefreshToken()

            'Manage open File Handles
            ManageOpenFileHandles()

            ' Manage Share Permiions
            ManagePermissions()

        Catch ex As Exception
            Me.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            Me.IsServiceStopping = False
            Me.IsFirstRun = True
            timer.Start()

        Catch ex As Exception
            Me.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Try
            Me.IsServiceStopping = True
            appSettingsHT = Nothing
            timer.Stop()

        Catch ex As Exception
            Me.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    ''' <summary>
    ''' This sub does the following things: 
    ''' 1. reads the config file
    ''' 2. validates the AgentId, SecretKey, Mac Address and gets the worspaceInfo
    ''' 3. Get Local Share Info for the workspace
    ''' 4. Get All Users for workspace Domain and store it in a HashTable 
    ''' 5. Get Open File handles for the workspace
    ''' </summary>
    Private Sub ValidateAndLoadInitialData()
        'Set Config settings
        ReadConfigSettingsFile()

        'Create ServiceTokenHelper object and get tokens
        tokenHelper = New ServiceTokenHelper(appSettingsHT(Constants.LOGIN_SERVICE_URL).ToString(), appSettingsHT(Constants.CLIENT_ID), appSettingsHT(Constants.CLIENT_SECRET))
        tokenHelper.SetTokens()

        'Validate Agent and Get WorkspaceId
        Dim agentClient As New FileAgentClient(appSettingsHT(Constants.SERVICE_URL).ToString(), String.Empty, tokenHelper.ServerAccessToken)
        Dim valResult = agentClient.ValidateAgentWithMAC(New Guid(appSettingsHT(Constants.AGENT_ID).ToString()), appSettingsHT(Constants.AGENT_SECRET).ToString(), Helper.GetMacAddresses())

        If (valResult.Status = Models.Enums.Status.Success AndAlso valResult.Data IsNot Nothing) Then
            If (valResult.Data.WorkSpaceId = New Guid(appSettingsHT(Constants.WORK_SPACE_ID).ToString())) Then
                workSpace = valResult.Data
            Else
                Throw New Exception(String.Format("WorkSpaceID from config file and from AgentName does not match. WorkspaceID in Config file: {0}, WorkspaceID from AgentName: {1}", appSettingsHT(Constants.WORK_SPACE_ID).ToString(), valResult.Data.WorkSpaceId.ToString()))
            End If

        Else
            Throw New Exception(String.Format("Validation Failed for Agent Name, Agent Secret and MacAddress. StatusCode: {0}", valResult.Status.ToString()))
        End If

        'Get Shares for the Agent/Workspace
        Dim shareResult = agentClient.GetLocalShares(workSpace.WorkSpaceId)

        If (shareResult.Status = Models.Enums.Status.Success AndAlso shareResult.Data IsNot Nothing) Then
            shares = shareResult.Data
        Else
            Throw New Exception(String.Format("Error while fetching Shares for the workspace. StatusCode: {0}", shareResult.Status.ToString()))
        End If

        ' Populate user Hash Table
        PopulateUserHT()

        'Get Handles from database for the workspace
        Dim fileHandleClient As New FileHandleClient(appSettingsHT(Constants.SERVICE_URL).ToString(), String.Empty, tokenHelper.ServerAccessToken)
        Dim result = fileHandleClient.GetOpenFileHandlesForWorkspace(workSpace.WorkSpaceId)

        If result.Status = Models.Enums.Status.Success Then
            prevOpenHandles = result.Data
        Else
            Throw New Exception(String.Format("Could Not Get Open File Handles from DB. StatusCode: {0}", result.Status.ToString()))

        End If
    End Sub

    Private Sub ReadConfigSettingsFile()
        Dim configPath As String = ConfigurationManager.AppSettings("ConfigFilePath")

        If (Not String.IsNullOrWhiteSpace(configPath)) Then

            Using fs As FileStream = File.Open(configPath, FileMode.Open, FileAccess.Read)
                Dim doc As XDocument = XDocument.Load(fs)

                appSettingsHT = New Hashtable()
                For Each el As XElement In doc.Elements("ROOT").Elements()
                    appSettingsHT.Add(el.Name.ToString(), el.Value.ToString())
                Next

            End Using
        Else
            Throw New Exception("Config File Path not set in app.config file.")
        End If

    End Sub

    Private Sub PopulateUserHT()
        If (workSpace IsNot Nothing AndAlso workSpace.UserDomainId > 0) Then
            'Dim currentUser = WindowsIdentity.GetCurrent.User.ToString()
            'If Not accountAdminSIDs.Exists(Function(p) p = currentUser) Then
            '    accountAdminSIDs.Add(currentUser)
            'End If

            Using userclient As UserClient = New UserClient(appSettingsHT(Constants.SERVICE_URL).ToString(), String.Empty, tokenHelper.ServerAccessToken)
                Dim result = userclient.GetDomainUsers(workSpace.UserDomainId)

                If result.Status = Status.Success AndAlso result.Data IsNot Nothing Then
                    userHT = New Hashtable()
                    Dim accountAdminRole = CType(UserRole.AccountAdmin, Integer)

                    For Each obj In result.Data
                        If Not String.IsNullOrWhiteSpace(obj.SID) Then


                            userHT.Add(obj.SID, obj)
                            fileMinisterSIDs.Add(obj.SID)
                            If obj.RoleId = accountAdminRole AndAlso Not accountAdminSIDs.Exists(Function(p) p = obj.SID) Then
                                accountAdminSIDs.Add(obj.SID)
                            End If
                        End If
                    Next
                End If

            End Using
        End If
    End Sub

    Private Sub ManageOpenFileHandles()
        Using fileHandleClient = New FileHandleClient(appSettingsHT(Constants.SERVICE_URL).ToString(), String.Empty, tokenHelper.ServerAccessToken)

            'Get Open File Handles
            Dim curOpenHandles = GetOpenFiles()
            Dim handlesToOpen As List(Of FileHandleInfo) = Nothing

            ' Get Handles to Close
            If prevOpenHandles IsNot Nothing AndAlso prevOpenHandles.Count > 0 Then


                Dim handlesToClose = prevOpenHandles.Except(curOpenHandles).ToList()
                handlesToOpen = curOpenHandles.Except(prevOpenHandles).ToList()

                'close Handles
                For Each close In handlesToClose
                    'Set User Email in the client object
                    If close.UserSID IsNot Nothing AndAlso userHT(close.UserSID) IsNot Nothing Then
                        Dim curUser As UserInfo = CType(userHT(close.UserSID), UserInfo)
                        fileHandleClient.UserEmail = curUser.UserName
                    End If

                    'Get File Size and Timestamp
                    Dim fileSize As Long = 0
                    Dim fileTimeStamp As Date = DateTime.MinValue
                    Dim share = shares.FirstOrDefault(Function(p) p.ShareId = close.FileShareId)
                    GetFileSizeAndTimeStamp(Path.Combine(share.SharePathLocal, close.FileRelativePath), fileSize, fileTimeStamp)

                    'API call to close File Handle
                    fileHandleClient.CloseFile(close.FileRelativePath, close.FileShareId, workSpace.WorkSpaceId, fileSize, fileTimeStamp)

                Next
            Else
                handlesToOpen = curOpenHandles
            End If

            'Open Handles
            For Each open In handlesToOpen
                'Set User Email in the client object
                If open.UserSID IsNot Nothing AndAlso userHT(open.UserSID) IsNot Nothing Then
                    Dim curUser As UserInfo = CType(userHT(open.UserSID), UserInfo)
                    fileHandleClient.UserEmail = curUser.UserName
                End If

                'Get File Size and Timestamp
                Dim fileSize As Long = 0
                Dim fileTimeStamp As Date = DateTime.MinValue
                Dim share = shares.FirstOrDefault(Function(p) p.ShareId = open.FileShareId)
                GetFileSizeAndTimeStamp(Path.Combine(share.SharePathLocal, open.FileRelativePath), fileSize, fileTimeStamp)
                open.ServerFileSizeAtClose = fileSize
                open.ServerFileTimeAtClose = fileTimeStamp
                'API call to Open File Handle
                fileHandleClient.OpenFileHandle(open.FileRelativePath, open.FileShareId, workSpace.WorkSpaceId, open.ServerFileSizeAtClose, open.ServerFileTimeAtClose)

            Next

            prevOpenHandles = curOpenHandles
        End Using
    End Sub

    Private Function GetOpenFiles() As List(Of FileHandleInfo)
        Dim openFileHandles As New List(Of FileHandleInfo)
        Dim fileStartReached As Boolean = False
        Dim hash As New HashSet(Of String)

        Dim p As New ProcessStartInfo
        p.FileName = "openfiles.exe"
        p.Arguments = "/query /fo CSV /nh"
        p.CreateNoWindow = True
        p.UseShellExecute = False
        p.RedirectStandardOutput = True
        p.RedirectStandardError = True

        Try
            ' Start the process with the info we specified.
            ' Call WaitForExit and then the using-statement will close.
            Using process As Process = Process.Start(p)

                Using reader As StreamReader = process.StandardOutput
                    While Not reader.EndOfStream()
                        Dim line = reader.ReadLine().ToLower().Replace(Chr(34), "")

                        If Not fileStartReached OrElse String.IsNullOrWhiteSpace(line) Then
                            If line.StartsWith("files opened remotely via local share points:") Then
                                fileStartReached = True
                            End If
                            Continue While
                        End If

                        Dim values = line.Split(",")
                        ' values(0) = "ID", values(1) = "User", Values(2) = "Type", values(3) = "FilePath"
                        If values.Length = 4 Then
                            Dim share = shares.FirstOrDefault(Function(x) values(3).StartsWith(x.SharePathLocal.ToLower()))

                            ' check whether we find a matching share we find a matching share
                            If share IsNot Nothing Then
                                ' Check whether the path represents a file
                                If File.Exists(values(3)) Then
                                    Dim relativePath = values(3).Replace(share.SharePathLocal.ToLower() + "\", "")

                                    Dim key = String.Format("{0}|{1}", relativePath, values(1))

                                    If Not hash.Contains(key) Then

                                        'Get User Details
                                        Dim user As UserInfo = Nothing
                                        If Not String.IsNullOrWhiteSpace(values(1)) Then
                                            user = FindUser(values(1))
                                        End If

                                        'Add File Handle Object
                                        openFileHandles.Add(New FileHandleInfo With {
                                            .FileRelativePath = relativePath,
                                            .FileShareId = share.ShareId,
                                            .UserId = If(user Is Nothing, Nothing, user.UserId),
                                            .UserSID = If(user Is Nothing, Nothing, user.SID)
                                                            })
                                        hash.Add(key)
                                    End If

                                End If
                            End If
                        End If

                    End While
                End Using
                process.WaitForExit()
            End Using
        Catch ex As Exception

        End Try
        Return openFileHandles
    End Function

    Private Function FindUser(userName As String) As UserInfo
        If Not String.IsNullOrWhiteSpace(userName) Then

            Dim strArray As String() = userName.Split("\\")

            If (strArray.Count = 2) Then
                'Get the User SID
                Dim sId = Helper.GetUserSID(strArray(1), strArray(0))

                If userHT(sId) IsNot Nothing Then
                    Return CType(userHT(sId), UserInfo)
                Else
                    PopulateUserHT()

                    If userHT(sId) IsNot Nothing Then
                        Return CType(userHT(sId), UserInfo)
                    End If
                End If

            End If
        End If

        Return Nothing
    End Function

    Private Sub ManagePermissions()
        Dim userPermissionClient As New UserFilePermissionClient(appSettingsHT(Constants.SERVICE_URL).ToString(), String.Empty, tokenHelper.ServerAccessToken)
        Dim groupPermissionClient As New GroupFilePermissionClient(appSettingsHT(Constants.SERVICE_URL).ToString(), String.Empty, tokenHelper.ServerAccessToken)
        Dim shareAdmin As Integer = CType(PermissionType.ShareAdmin, Integer)

        For Each share In shares
            Dim permissions As New List(Of FilePermissionInfo)

            'API Call to get all User permisisons on the Share
            Dim userResult = userPermissionClient.GetUserFilePermissionsForShare(share.ShareId)

            If userResult.Status = Status.Success AndAlso userResult.Data IsNot Nothing AndAlso userResult.Data.Count > 0 Then
                permissions.AddRange(userResult.Data)
            End If

            'API Call to get all Group permisisons on the Share
            Dim groupResult = groupPermissionClient.GetGroupFilePermissionsForShare(share.ShareId)

            If groupResult.Status = Status.Success AndAlso groupResult.Data IsNot Nothing AndAlso groupResult.Data.Count > 0 Then
                permissions.AddRange(groupResult.Data)
            End If

            Dim adminPermissions = accountAdminSIDs.ConvertAll(Of FilePermissionInfo)(Function(p) New FilePermissionInfo With
                {
                .RelativePath = share.SharePathLocal,
                .ExclusiveAllow = shareAdmin,
                .ExclusiveDeny = 0,
                .SID = p
                })

            If permissions.Exists(Function(p) (p.ExclusiveAllow And shareAdmin) = shareAdmin) Then
                adminPermissions.AddRange(permissions.Where(Function(p) ((p.ExclusiveAllow And shareAdmin) = shareAdmin) AndAlso Not adminPermissions.Any(Function(q) q.SID = p.SID)).Select(Of FilePermissionInfo)(Function(p) New FilePermissionInfo With
                {
                .RelativePath = share.SharePathLocal,
                .ExclusiveAllow = shareAdmin,
                .ExclusiveDeny = 0,
                .SID = p.SID
                }))
            End If

            'Manage Account Admin and Share Admin Permission at the share root
            Dim d = New DirectoryInfo(share.SharePathLocal)
            ' Manage Account Admin and Share Admin Permissions at the Root
            ManageFolderPermission(d, adminPermissions)
            'Manage file permissions at the root
            'Get all files within the folder
            Dim rootFiles = d.GetFiles()

            'Iterate files to Manage exclusive permissions
            For Each file In rootFiles
                'Get DB Permissions for the file
                Dim filePermissions = permissions.Where(Function(p) Path.Combine(share.SharePathLocal, p.RelativePath) = file.FullName).ToList()
                'Manage file Permissions
                ManageFilePermission(file, filePermissions)
            Next


            ' Traverse the whole folder structure of the share to check for permissions and set/delete them if needed
            Dim folders = d.GetDirectories("*", SearchOption.AllDirectories).OrderBy(Of Integer)(Function(p) Len(p.FullName))

            ' Iterate folders and Manage Exculsive Permissions
            For Each folder In folders
                ' get DB permissions for this folder
                Dim folderPermissions = permissions.Where(Function(p) Path.Combine(share.SharePathLocal, p.RelativePath) = folder.FullName).ToList()
                'Manage folder permissions
                ManageFolderPermission(folder, folderPermissions)

                'Get all files within the folder
                Dim files = folder.GetFiles()

                'Iterate files to Manage exclusive permissions
                For Each file In files
                    'Get DB Permissions for the file
                    Dim filePermissions = permissions.Where(Function(p) Path.Combine(share.SharePathLocal, p.RelativePath) = file.FullName).ToList()
                    'Manage file Permissions
                    ManageFilePermission(file, filePermissions)
                Next
            Next
        Next
    End Sub

    Private Sub ManageFolderPermission(folder As DirectoryInfo, permissions As List(Of FilePermissionInfo))
        Dim secColl As DirectorySecurity = folder.GetAccessControl()
        Dim accessRules = secColl.GetAccessRules(True, False, Type.GetType("System.Security.Principal.SecurityIdentifier")).OfType(Of FileSystemAccessRule)

        Dim permissionsRemove As New List(Of String)
        Dim change As Boolean = False

        If (permissions IsNot Nothing AndAlso permissions.Count > 0) Then
            '    ' Since Explicit Permissions exist on the Folder, turn off Inheritance at this folder
            '    'secColl.SetAccessRuleProtection(True, False)

            For Each permission In permissions

                Dim allowRule = accessRules.FirstOrDefault(Function(p) p.IdentityReference.ToString() = permission.SID AndAlso p.AccessControlType = AccessControlType.Allow)
                Dim denyRule = accessRules.FirstOrDefault(Function(p) p.IdentityReference.ToString() = permission.SID AndAlso p.AccessControlType = AccessControlType.Deny)

                ' Manage Allow Rule
                If allowRule IsNot Nothing OrElse permission.ExclusiveAllow > 0 Then
                    If permission.ExclusiveAllow = 0 Then
                        change = True
                        secColl.RemoveAccessRule(allowRule)
                    Else
                        Dim transPermissionAllow = TranslatePermission(permission.ExclusiveAllow)
                        If allowRule Is Nothing OrElse Not (allowRule.FileSystemRights = transPermissionAllow) Then
                            change = True
                            secColl.ResetAccessRule(New FileSystemAccessRule(New SecurityIdentifier(permission.SID), transPermissionAllow, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
                        End If
                    End If
                End If

                ' Manage Deny Rule
                If denyRule IsNot Nothing OrElse permission.ExclusiveDeny > 0 Then
                    If permission.ExclusiveDeny = 0 Then
                        change = True
                        secColl.RemoveAccessRule(denyRule)
                    Else
                        Dim transPermissionDeny = TranslatePermission(permission.ExclusiveDeny)
                        If denyRule Is Nothing OrElse Not (denyRule.FileSystemRights = transPermissionDeny) Then
                            change = True
                            secColl.ResetAccessRule(New FileSystemAccessRule(New SecurityIdentifier(permission.SID), transPermissionDeny, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Deny))
                        End If
                    End If
                End If
            Next

            permissionsRemove.AddRange(accessRules.Where(Function(p) Not permissions.Any(Function(q) q.SID = p.IdentityReference.ToString()) AndAlso fileMinisterSIDs.Exists(Function(r) r = p.IdentityReference.ToString())).Select(Function(p) p.IdentityReference.ToString()))
        Else
            'Get the SID which needs to be removed from this folder
            If accessRules IsNot Nothing AndAlso accessRules.Count > 0 Then
                permissionsRemove.AddRange(accessRules.Where(Function(p) fileMinisterSIDs.Exists(Function(q) q = p.IdentityReference.ToString())).Select(Function(p) p.IdentityReference.ToString()))
            End If
        End If

        ' IF there are SIDs that needs to be removed, purge the rules
        If permissionsRemove IsNot Nothing AndAlso permissionsRemove.Count > 0 Then
            ' Since Explicit Permissions exist on the File, turn off Inheritance at this file so that explicit permissions can be set
            'secColl.SetAccessRuleProtection(True, False)
            change = True
            For Each p In permissionsRemove
                secColl.PurgeAccessRules(New SecurityIdentifier(p))
            Next
        End If

        If change Then
            folder.SetAccessControl(secColl)
        End If

    End Sub

    Private Sub ManageFilePermission(file As IO.FileInfo, permissions As List(Of FilePermissionInfo))
        Dim secColl As FileSecurity = file.GetAccessControl()
        Dim accessRules = secColl.GetAccessRules(True, False, Type.GetType("System.Security.Principal.SecurityIdentifier")).OfType(Of FileSystemAccessRule)
        Dim permissionsRemove As New List(Of String)
        Dim change As Boolean = False

        If (permissions IsNot Nothing AndAlso permissions.Count > 0) Then
            ' Since Explicit Permissions exist on the File, turn off Inheritance at this file so that explicit permissions can be set
            'secColl.SetAccessRuleProtection(True, False)

            For Each permission In permissions

                Dim allowRule = accessRules.FirstOrDefault(Function(p) p.IdentityReference.ToString() = permission.SID AndAlso p.AccessControlType = AccessControlType.Allow)
                Dim denyRule = accessRules.FirstOrDefault(Function(p) p.IdentityReference.ToString() = permission.SID AndAlso p.AccessControlType = AccessControlType.Deny)

                ' Manage Allow Rule
                If allowRule IsNot Nothing OrElse permission.ExclusiveAllow > 0 Then
                    If permission.ExclusiveAllow = 0 Then
                        change = True
                        secColl.RemoveAccessRule(allowRule)
                    Else
                        Dim transPermissionAllow = TranslatePermission(permission.ExclusiveAllow)
                        If allowRule Is Nothing OrElse Not (denyRule.FileSystemRights = transPermissionAllow) Then
                            change = True
                            secColl.ResetAccessRule(New FileSystemAccessRule(New SecurityIdentifier(permission.SID), transPermissionAllow, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
                        End If
                    End If
                End If

                ' Manage Deny Rule
                If denyRule IsNot Nothing OrElse permission.ExclusiveDeny > 0 Then
                    If permission.ExclusiveDeny = 0 Then
                        change = True
                        secColl.RemoveAccessRule(denyRule)
                    Else
                        Dim transPermissionDeny = TranslatePermission(permission.ExclusiveDeny)
                        If denyRule Is Nothing OrElse Not (denyRule.FileSystemRights = transPermissionDeny) Then
                            change = True
                            secColl.ResetAccessRule(New FileSystemAccessRule(New SecurityIdentifier(permission.SID), transPermissionDeny, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Deny))
                        End If
                    End If
                End If

            Next
            permissionsRemove.AddRange(accessRules.Where(Function(p) Not permissions.Any(Function(q) q.SID = p.IdentityReference.ToString()) AndAlso fileMinisterSIDs.Exists(Function(r) r = p.IdentityReference.ToString())).Select(Function(p) p.IdentityReference.ToString()))
        Else
            'Get the SID which needs to be removed from this folder
            If accessRules IsNot Nothing AndAlso accessRules.Count > 0 Then
                permissionsRemove.AddRange(accessRules.Where(Function(p) fileMinisterSIDs.Exists(Function(q) q = p.IdentityReference.ToString())).Select(Function(p) p.IdentityReference.ToString()))
            End If
        End If

        ' IF there are SIDs that needs to be removed, purge the rules
        If permissionsRemove IsNot Nothing AndAlso permissionsRemove.Count > 0 Then
            ' Since Explicit Permissions exist on the File, turn off Inheritance at this file so that explicit permissions can be set
            'secColl.SetAccessRuleProtection(True, False)
            change = True
            For Each p In permissionsRemove
                secColl.PurgeAccessRules(New SecurityIdentifier(p))
            Next
        End If

        If change Then
            file.SetAccessControl(secColl)
        End If

    End Sub

    Public Function TranslatePermission(permission As Integer) As FileSystemRights
        If (permission And PermissionType.ShareAdmin) = PermissionType.ShareAdmin Then
            Return FileSystemRights.FullControl
        ElseIf (permission And PermissionType.Write) = PermissionType.Write Then
            Return FileSystemRights.Modify
        ElseIf (permission And PermissionType.Read) = PermissionType.Read Then
            Return FileSystemRights.ReadAndExecute
        Else
            Return FileSystemRights.ExecuteFile
        End If

    End Function

    Private Sub GetFileSizeAndTimeStamp(filePath As String, ByRef fileSize As Long, ByRef fileTimeStamp As Date)
        Dim file As New System.IO.FileInfo(filePath)

        If file IsNot Nothing Then
            fileSize = file.Length
            fileTimeStamp = file.LastWriteTimeUtc
        End If

    End Sub

End Class
