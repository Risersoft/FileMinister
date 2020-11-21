Imports risersoft.shared.portable.Model
Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.portable
Imports risersoft.shared.cloud

Public Class Properties

#Region "Declaration"

    Dim cLeft As Integer = 1
    Dim textBoxes As List(Of TextBox)
    Dim deleteButtons As List(Of Button)
    Dim rowIndex As Integer = 1
    Dim allTagID As New List(Of TagInfo)

    Public Property fileId As Guid
    Public Property fileType As Integer
    Public Property shareId As Integer
    Public Property file As FileEntryInfo
    Public Property accountName As String = AuthData.User.AccountName
    Public Property AllUserAndGroupList As List(Of UserGroupInfo)
    Public Property DeletedUserAndGroupList As New List(Of UserGroupInfo)()
    Public Property CanChangeTag = True
    Public Property CanChangePermission = True

    Dim oldUserGroup As UserGroupInfo
    Dim changedPermissions As New Dictionary(Of UserGroupInfo, UserFilePermissionInfo)

#End Region

#Region "Constructor And Page Load And Other Events"

    Public Sub New(fileParam As FileEntryInfo)

        file = fileParam
        fileId = fileParam.FileEntryId
        fileType = fileParam.FileEntryTypeId
        shareId = fileParam.FileShareId

        InitializeComponent()

    End Sub

    Private Sub csProperties_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        BindMetadata()

        Dim prmsnprovider As PermissionProvider = New PermissionProvider(file)
        If (prmsnprovider.CanViewPermission()) Then
            BindGroupsAndUsers()
        Else
            CanChangePermission = False
            tbCtrlProperties.TabPages.Remove(tbPagePermissions)
        End If

        If (prmsnprovider.CanChangeTag() = False) Then
            CanChangeTag = False
            dgvMetadata.Enabled = False
        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub


#End Region

#Region "Events Of PROPERTIES"

    Private Sub lstGrouporUsers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstGroupUsers.SelectedIndexChanged
        If (oldUserGroup IsNot Nothing) Then
            Dim permissions = GetSetPermissions(oldUserGroup)
            If (permissions IsNot Nothing) Then
                changedPermissions(oldUserGroup) = permissions
            End If
        End If
        BindPermissions()

        If (lstGroupUsers.DataSource IsNot Nothing) Then
            oldUserGroup = CType(lstGroupUsers.SelectedItem, UserGroupInfo)
            Dim curItem As String = oldUserGroup.Name
            Dim curItemValue As Guid = oldUserGroup.Id
            Dim curItemType As Integer = oldUserGroup.Type
            If (changedPermissions.ContainsKey(oldUserGroup)) Then
                Dim permissions = changedPermissions(oldUserGroup)
                If (permissions IsNot Nothing) Then
                    SetPermissions(permissions)
                End If
            Else
                Dim async = New AsyncProvider()
                If curItemType = 1 Then
                    async.AddMethod("permission", Function() New Common.UserFilePermissionClient().GetPermissionByFileAndUser(fileId, curItemValue))
                Else
                    async.AddMethod("permission", Function() New Common.GroupFilePermissionClient().GetPermissionByFileAndGroup(fileId, curItemValue))
                End If
                async.OnCompletion = Sub(list As IDictionary)
                                         Dim result = CType(list("permission"), ResultInfo(Of UserFilePermissionInfo, Status))
                                         If ValidateResponse(result) Then
                                             If result.Data IsNot Nothing Then
                                                 SetPermissions(result.Data)
                                             End If
                                         End If
                                     End Sub
                async.Execute()
            End If
            lblPermissions.Text = "Permissions for " & curItem
        End If
    End Sub

    Private Sub btnRemoveUser_Click(sender As Object, e As EventArgs) Handles btnRemoveUser.Click

        If (lstGroupUsers.Items.Count() > 0 AndAlso lstGroupUsers.SelectedItem IsNot Nothing) Then

            Dim selIndex = lstGroupUsers.SelectedIndex
            Dim selectedUserGroup = CType(lstGroupUsers.SelectedItem, UserGroupInfo)
            Dim curItem As String = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Name
            Dim curItemValue As Guid = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Id
            Dim curItemType As Integer = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Type

            'Dim EffectiveAllow As Integer = 0
            'Dim EffectiveDeny As Integer = 0
            'Dim ExclusiveAllow As Integer = 0
            'Dim ExclusiveDeny As Integer = 0

            If curItemType = 1 Then
                Dim async = New AsyncProvider()
                async.AddMethod("permission", Function() New Common.UserFilePermissionClient().GetPermissionByFileAndUser(fileId, curItemValue))
                async.OnCompletion = Sub(list As IDictionary)
                                         Dim result = CType(list("permission"), ResultInfo(Of UserFilePermissionInfo, Status))
                                         If ValidateResponse(result) Then
                                             Dim Data = result.Data
                                             If Data IsNot Nothing Then
                                                 RemoveUser(selectedUserGroup, Data)
                                             End If
                                         End If
                                     End Sub
                async.Execute()
            Else
                Dim async = New AsyncProvider()
                async.AddMethod("permission", Function() New Common.GroupFilePermissionClient().GetPermissionByFileAndGroup(fileId, curItemValue))
                async.OnCompletion = Sub(list As IDictionary)
                                         Dim result = CType(list("permission"), ResultInfo(Of UserFilePermissionInfo, Status))
                                         If ValidateResponse(result) Then
                                             Dim Data = result.Data
                                             If Data IsNot Nothing Then
                                                 RemoveUser(selectedUserGroup, Data)
                                             End If
                                         End If
                                     End Sub
                async.Execute()
            End If
        Else
            MessageBoxHelper.ShowInfoMessage("Either there is no associated group and user or no selection is made")
        End If
    End Sub

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs) Handles btnAddUser.Click
        Dim myForm As New SelectUser(Me.accountName)
        myForm.ShowDialog()
        Dim row = myForm.SelectedValue
        If (row IsNot Nothing) Then
            Dim ugId As Guid = New Guid(row.Cells(0).Value.ToString)
            Dim ugName As String = row.Cells(1).Value
            Dim ugType As Integer = Convert.ToInt32(row.Cells(2).Value)
            Dim rowFound = False

            For Each UGInfo In AllUserAndGroupList
                If (UGInfo.Type = ugType And UGInfo.Id = ugId) Then
                    rowFound = True
                End If
            Next


            If (rowFound = False) Then

                lstGroupUsers.BeginUpdate()

                Dim obj As UserGroupInfo = New UserGroupInfo()
                obj.Id = ugId
                obj.Name = ugName
                obj.Type = ugType

                If (DeletedUserAndGroupList IsNot Nothing) Then

                    Dim indexDel As Integer = -1
                    For Each element In DeletedUserAndGroupList
                        If (element.Id = obj.Id) Then
                            indexDel = DeletedUserAndGroupList.IndexOf(element)
                        End If
                    Next
                    If (indexDel > -1) Then
                        DeletedUserAndGroupList.RemoveAt(indexDel)
                    End If

                End If


                AllUserAndGroupList.Add(obj)

                lstGroupUsers.DataSource = Nothing
                lstGroupUsers.Items.Clear()

                lstGroupUsers.DisplayMember = "Name"
                lstGroupUsers.ValueMember = "Id"

                lstGroupUsers.DataSource = AllUserAndGroupList


                lstGroupUsers.EndUpdate()

                lstGroupUsers.SelectedIndex = lstGroupUsers.Items.Count() - 1

            End If
        End If

    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        Dim result As Boolean = True
        If (CanChangeTag) Then
            result = SaveMetadata()
        End If

        If (result AndAlso CanChangePermission) Then
            If (oldUserGroup IsNot Nothing) Then
                Dim permissions = GetSetPermissions(oldUserGroup)
                If (permissions IsNot Nothing) Then
                    changedPermissions(oldUserGroup) = permissions
                End If
            End If
            Using proxy = New Common.UserFilePermissionClient()
                Dim changedPermissionList As New List(Of UserFilePermissionInfo)
                changedPermissionList.AddRange(changedPermissions.Values)
                Dim callresult = proxy.UpdateFilePermission(fileId, DeletedUserAndGroupList, changedPermissionList)
                If (Not callresult.Data) Then
                    MsgBox("Error in saving permissions")
                End If
            End Using
            'RemoveUsersGroups()

            '    If (lstGroupUsers.SelectedItem IsNot Nothing) Then

            '        Dim curItem As String = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Name
            '        Dim curItemValue As Integer = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Id
            '        Dim curItemType As Integer = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Type

            '        Dim allowedPermission = 0
            '        Dim deniedPermission = 0

            '        For Each row As DataGridViewRow In dgvPermissions.Rows
            '            Dim masterPermissionId As Integer = Convert.ToInt32(row.Cells("PermissionId").Value)
            '            For Each cell In row.Cells
            '                If TypeOf cell Is DataGridViewCheckBoxCell Then
            '                    If Convert.ToBoolean(cell.Value) = True Then
            '                        If cell.ReadOnly = False Then
            '                            If dgvPermissions.Columns(cell.ColumnIndex).Name = "Allow" Then
            '                                allowedPermission = allowedPermission + masterPermissionId
            '                            Else
            '                                deniedPermission = deniedPermission + masterPermissionId
            '                            End If
            '                        End If
            '                    End If
            '                End If
            '            Next
            '        Next

            '        If (allowedPermission = deniedPermission) Then
            '            MessageBoxHelper.ShowErrorMessage("Invalid Permissions")
            '            'Exit Sub
            '        Else
            '            Using ugProxy = New Common.UserGroupClient()
            '                If curItemType = 1 Then
            '                    Dim async = New AsyncProvider()
            '                    async.AddMethod("Users", Function() New Common.OrganizationClient().GetMyAccountUser(curItemValue, accountId))
            '                    async.OnCompletion = Sub(list As IDictionary)
            '                                             Dim result = CType(list("Users"), ResultInfo(Of Dictionary(Of String, Object)))
            '                                             If ValidateResponse(result) Then
            '                                                 Dim userData = result.Data
            '                                                 ugProxy.DeleteByUser(curItemValue)
            '                                                 Dim ugaList As List(Of UserGroupAssignmentsInfo) = New List(Of UserGroupAssignmentsInfo)()
            '                                                 Dim groupList = userData("userGroupList")
            '                                                 For Each item In groupList
            '                                                     Dim groupid = Convert.ToInt32(item("groupId"))
            '                                                     Dim uga As UserGroupAssignmentsInfo = New UserGroupAssignmentsInfo()
            '                                                     uga.UserId = curItemValue
            '                                                     uga.GroupId = groupid
            '                                                     ugaList.Add(uga)
            '                                                 Next
            '                                                 ugProxy.AddAll(ugaList)
            '                                             End If
            '                                         End Sub
            '                    async.Execute()

            '                    Using proxy = New Common.UserFilePermissionClient()
            '                        proxy.UpdatePermissionByFileAndUser(fileId, curItemValue, allowedPermission, deniedPermission)
            '                    End Using
            '                Else

            '                    Dim async = New AsyncProvider()
            '                    async.AddMethod("Groups", Function() New Common.OrganizationClient().GetMyGroup(curItemValue, accountId))
            '                    async.OnCompletion = Sub(list As IDictionary)
            '                                             Dim result = CType(list("Groups"), ResultInfo(Of Dictionary(Of String, Object)))
            '                                             If ValidateResponse(result) Then
            '                                                 Dim groupData = result.Data
            '                                                 ugProxy.DeleteByGroup(curItemValue)
            '                                                 Dim ugaList As List(Of UserGroupAssignmentsInfo) = New List(Of UserGroupAssignmentsInfo)()
            '                                                 Dim userList = groupData("userList")
            '                                                 For Each item In userList
            '                                                     Dim userid = Convert.ToInt32(item("userId"))
            '                                                     Dim uga As UserGroupAssignmentsInfo = New UserGroupAssignmentsInfo()
            '                                                     uga.UserId = userid
            '                                                     uga.GroupId = curItemValue
            '                                                     ugaList.Add(uga)
            '                                                 Next
            '                                                 ugProxy.AddAll(ugaList)
            '                                             End If
            '                                         End Sub
            '                    async.Execute()
            '                    Using proxy = New Common.GroupFilePermissionClient()
            '                        proxy.UpdatePermissionByFileAndGroup(fileId, curItemValue, allowedPermission, deniedPermission)
            '                    End Using
            '                End If
            '            End Using
            '            Me.Close()
            '        End If
            '    End If
        End If
        Me.Close()
    End Sub


    Private Function GetSetPermissions(userGroup As UserGroupInfo) As UserFilePermissionInfo
        Dim obj As UserFilePermissionInfo = Nothing
        If (userGroup IsNot Nothing) Then
            obj = New UserFilePermissionInfo
            'Dim curItem As String = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Name
            'Dim curItemValue As Integer = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Id
            'Dim curItemType As Integer = CType(lstGroupUsers.SelectedItem, UserGroupInfo).Type
            obj.Id = userGroup.Id
            obj.Type = userGroup.Type
            Dim allowedPermission = 0
            Dim deniedPermission = 0
            Dim inheritedAllowedPermission = 0
            Dim inheritedDeniedPermission = 0

            For Each row As DataGridViewRow In dgvPermissions.Rows
                Dim masterPermissionId As Integer = Convert.ToInt32(row.Cells("PermissionId").Value)
                For Each cell In row.Cells
                    If TypeOf cell Is DataGridViewCheckBoxCell Then
                        If Convert.ToBoolean(cell.Value) = True Then
                            If cell.ReadOnly = False Then
                                If dgvPermissions.Columns(cell.ColumnIndex).Name = "Allow" Then
                                    allowedPermission = allowedPermission + masterPermissionId
                                Else
                                    deniedPermission = deniedPermission + masterPermissionId
                                End If
                            Else
                                If dgvPermissions.Columns(cell.ColumnIndex).Name = "Allow" Then
                                    inheritedAllowedPermission = inheritedAllowedPermission + masterPermissionId
                                Else
                                    inheritedDeniedPermission = inheritedDeniedPermission + masterPermissionId
                                End If
                            End If
                        End If
                    End If
                Next
            Next
            obj.ExclusiveAllow = allowedPermission
            obj.ExclusiveDeny = deniedPermission
            obj.EffectiveAllow = inheritedAllowedPermission
            obj.EffectiveDeny = inheritedDeniedPermission
        End If
        Return obj
    End Function

    Private Sub EnableAllAllow(e As DataGridViewCellEventArgs)
        For Each rw As DataGridViewRow In dgvPermissions.Rows
            If (rw.Index < e.RowIndex) Then
                For Each cell In rw.Cells
                    If TypeOf cell Is DataGridViewCheckBoxCell Then
                        If dgvPermissions.Columns(cell.ColumnIndex).Name = "Allow" Then
                            If (cell.ReadOnly = False) Then
                                cell.Value = True
                            End If
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub DisableAllAllow(e As DataGridViewCellEventArgs)
        For Each rw As DataGridViewRow In dgvPermissions.Rows
            If (rw.Index > e.RowIndex) Then
                For Each cell In rw.Cells
                    If TypeOf cell Is DataGridViewCheckBoxCell Then
                        If dgvPermissions.Columns(cell.ColumnIndex).Name = "Allow" Then
                            If (cell.ReadOnly = False) Then
                                cell.Value = False
                            End If
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub EnableAllDeny(e As DataGridViewCellEventArgs)
        For Each rw As DataGridViewRow In dgvPermissions.Rows
            If (rw.Index > e.RowIndex) Then
                For Each cell In rw.Cells
                    If TypeOf cell Is DataGridViewCheckBoxCell Then
                        If dgvPermissions.Columns(cell.ColumnIndex).Name = "Deny" Then
                            If (cell.ReadOnly = False) Then
                                cell.Value = True
                            End If
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub DisableAllDeny(e As DataGridViewCellEventArgs)
        For Each rw As DataGridViewRow In dgvPermissions.Rows
            If (rw.Index < e.RowIndex) Then
                For Each cell In rw.Cells
                    If TypeOf cell Is DataGridViewCheckBoxCell Then
                        If dgvPermissions.Columns(cell.ColumnIndex).Name = "Deny" Then
                            If (cell.ReadOnly = False) Then
                                cell.Value = False
                            End If
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub dgvPermissions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPermissions.CellContentClick
        dgvPermissions.CommitEdit(DataGridViewDataErrorContexts.Commit)

        If (e.RowIndex < 0) Then
            Return
        End If

        Dim row = dgvPermissions.Rows(e.RowIndex)
        Dim cell = dgvPermissions.Rows(e.RowIndex).Cells(e.ColumnIndex)


        If TypeOf cell Is DataGridViewCheckBoxCell Then
            If (cell.ReadOnly = False) Then

                Dim cellIndexToValidate As String = ""
                If (dgvPermissions.Columns(e.ColumnIndex).Name = "Allow") Then
                    cellIndexToValidate = "Deny"
                Else
                    cellIndexToValidate = "Allow"
                End If

                If Convert.ToBoolean(cell.Value) = True Then
                    Dim nextCell = dgvPermissions.Rows(e.RowIndex).Cells(cellIndexToValidate)
                    If nextCell.ReadOnly = False Then
                        nextCell.Value = False
                    End If

                    If (dgvPermissions.Columns(e.ColumnIndex).Name = "Allow") Then
                        EnableAllAllow(e)
                        DisableAllDeny(e)
                    Else
                        EnableAllDeny(e)
                        DisableAllAllow(e)
                    End If
                Else
                    If (dgvPermissions.Columns(e.ColumnIndex).Name = "Allow") Then
                        DisableAllAllow(e)
                    Else
                        DisableAllDeny(e)
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Private Methods Of PROPERTIES"

    Private Sub BindGroupsAndUsers()
        'GET ALL GROUPS AND USERS AGAINST A FILE
        Dim async1 = New AsyncProvider()
        async1.AddMethod("UsersAndGroups", Function() New Common.UserFilePermissionClient().GetAllUsersAndGroups(fileId))
        async1.OnCompletion = Sub(list As IDictionary)
                                  Dim result = CType(list("UsersAndGroups"), ResultInfo(Of List(Of UserGroupInfo), Status))
                                  If ValidateResponse(result) Then
                                      AllUserAndGroupList = result.Data
                                      lstGroupUsers.DisplayMember = "Name"
                                      lstGroupUsers.ValueMember = "Id"
                                      lstGroupUsers.DataSource = AllUserAndGroupList
                                      'For Each UGInfo In AllUserAndGroupList.ToList()
                                      '    If (UGInfo.Type = UserGroupType.User) Then
                                      '        Dim async = New AsyncProvider()
                                      '        async.AddMethod("Users", Function() New Common.OrganizationClient().GetMyAccountUser(UGInfo.Id, accountId))
                                      '        async.OnCompletion = Sub(userList As IDictionary)
                                      '                                 Dim userResult = CType(userList("Users"), ResultInfo(Of Dictionary(Of String, Object)))
                                      '                                 If ValidateResponse(userResult) Then
                                      '                                     UGInfo.Name = userResult.Data("userName")
                                      '                                 End If
                                      '                                 If Object.ReferenceEquals(UGInfo, AllUserAndGroupList.Last()) Then
                                      '                                     lstGroupUsers.DisplayMember = "Name"
                                      '                                     lstGroupUsers.ValueMember = "Id"
                                      '                                     lstGroupUsers.DataSource = AllUserAndGroupList
                                      '                                 End If
                                      '                             End Sub
                                      '        async.Execute()
                                      '    Else
                                      '        Dim async = New AsyncProvider()
                                      '        async.AddMethod("Groups", Function() New Common.OrganizationClient().GetMyAccountGroup(UGInfo.Id))
                                      '        async.OnCompletion = Sub(groupList As IDictionary)
                                      '                                 Dim groupResult = CType(groupList("Groups"), ResultInfo(Of Dictionary(Of String, Object)))
                                      '                                 If ValidateResponse(groupResult) Then
                                      '                                     UGInfo.Name = groupResult.Data("groupName")
                                      '                                     If Object.ReferenceEquals(UGInfo, AllUserAndGroupList.Last()) Then
                                      '                                         lstGroupUsers.DisplayMember = "Name"
                                      '                                         lstGroupUsers.ValueMember = "Id"
                                      '                                         lstGroupUsers.DataSource = AllUserAndGroupList
                                      '                                     End If
                                      '                                 End If
                                      '                             End Sub
                                      '        async.Execute()
                                      '    End If
                                      'Next
                                  End If
                              End Sub
        async1.Execute()

    End Sub

    Private Sub BindPermissions()

        Me.dgvPermissions.DataSource = Nothing
        Me.dgvPermissions.Refresh()
        Me.dgvPermissions.Rows.Clear()
        Me.dgvPermissions.Columns.Clear()

        'Dim items As Array
        Dim enumValues As Array = System.Enum.GetValues(GetType(PermissionType))

        Dim list As ArrayList = New ArrayList()
        For Each item In enumValues
            Dim description = Helper.GetEnumDescription(item)
            list.Add(New KeyValuePair(Of [Enum], String)(item, description))
        Next


        Dim data As List(Of PermissionInfo) = New List(Of PermissionInfo)()

        For Each item In list
            If (Convert.ToInt32(item.key) = PermissionType.Read OrElse Convert.ToInt32(item.key) = PermissionType.Write OrElse Convert.ToInt32(item.key) = PermissionType.Tag) Then
                Dim obj As PermissionInfo = New PermissionInfo()
                obj.PermissionId = Convert.ToInt32(item.key)
                obj.PermissionName = item.value.ToString()
                data.Add(obj)
            Else
                If (Convert.ToInt32(item.key) = PermissionType.ShareAdmin And fileType = Enums.FileType.Share) Then
                    If (PermissionProvider.CanChangeShareAdminPermission() = True) Then
                        Dim obj As PermissionInfo = New PermissionInfo()
                        obj.PermissionId = Convert.ToInt32(item.key)
                        obj.PermissionName = item.value.ToString()
                        data.Add(obj)
                    End If
                End If
            End If
        Next

        Me.dgvPermissions.DataSource = data

        For Each col In dgvPermissions.Columns
            col.Visible = False
        Next
        Me.dgvPermissions.Columns("PermissionName").Visible = True


        Me.dgvPermissions.Columns("PermissionName").HeaderText = ""
        dgvPermissions.DefaultCellStyle.SelectionBackColor = dgvPermissions.DefaultCellStyle.BackColor
        dgvPermissions.DefaultCellStyle.SelectionForeColor = dgvPermissions.DefaultCellStyle.ForeColor

        Dim doWork As New DataGridViewCheckBoxColumn()
        doWork.Name = "Allow"
        doWork.HeaderText = "Allow"
        dgvPermissions.Columns.Insert(dgvPermissions.Columns.Count, doWork)

        Dim doWork1 As New DataGridViewCheckBoxColumn()
        doWork1.Name = "Deny"
        doWork1.HeaderText = "Deny"
        dgvPermissions.Columns.Insert(dgvPermissions.Columns.Count, doWork1)

        For Each row As DataGridViewRow In dgvPermissions.Rows
            Dim masterPermissionId As Integer = Convert.ToInt32(row.Cells("PermissionId").Value)
            If (masterPermissionId = PermissionType.ShareAdmin) Then
                For Each cell In row.Cells
                    If TypeOf cell Is DataGridViewCheckBoxCell Then
                        If dgvPermissions.Columns(cell.ColumnIndex).Name = "Deny" Then
                            cell.ReadOnly = True
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub SetPermissions(permissions As UserFilePermissionInfo)
        For Each row As DataGridViewRow In dgvPermissions.Rows
            Dim masterPermissionId As Integer = Convert.ToInt32(row.Cells(0).Value)
            Dim resultEffDeny As Boolean = masterPermissionId And permissions.EffectiveDeny
            If resultEffDeny = True Then
                row.Cells("Allow").ReadOnly = True
                row.Cells("Deny").Value = True
                row.Cells("Deny").ReadOnly = True
            Else
                Dim resultEffAllow As Boolean = masterPermissionId And permissions.EffectiveAllow
                If resultEffAllow = True Then
                    row.Cells("Allow").Value = True
                    row.Cells("Allow").ReadOnly = True
                End If

                Dim resultExAllow As Boolean = masterPermissionId And permissions.ExclusiveAllow
                If resultExAllow = True Then
                    row.Cells("Allow").Value = True
                    row.Cells("Allow").ReadOnly = False
                End If

                Dim resultExDeny As Boolean = masterPermissionId And permissions.ExclusiveDeny
                If resultExDeny = True Then
                    row.Cells("Deny").Value = True
                    row.Cells("Deny").ReadOnly = False
                End If
            End If
        Next
    End Sub

    Private Sub RemoveUser(selUserGroup As UserGroupInfo, selUserGroupPermission As UserFilePermissionInfo)
        If (selUserGroupPermission.EffectiveAllow > 0 OrElse selUserGroupPermission.EffectiveDeny > 0) Then
            MessageBoxHelper.ShowErrorMessage("Can not delete the user having inherited permissions")
        Else
            'If (DeletedUserAndGroupList Is Nothing) Then
            '    DeletedUserAndGroupList = New List(Of UserGroupInfo)
            'End If


            Dim indexDel As Integer = -1
            For Each element In DeletedUserAndGroupList
                If (element.Id = selUserGroup.Id) Then
                    indexDel = DeletedUserAndGroupList.IndexOf(element)
                    Exit For
                End If
            Next
            If (indexDel <= -1) Then
                DeletedUserAndGroupList.Add(selUserGroup)
            End If

            Dim index As Integer = -1
            For Each element In AllUserAndGroupList
                If (element.Id = selUserGroup.Id) Then
                    index = AllUserAndGroupList.IndexOf(element)
                    Exit For
                End If
            Next
            If (index > -1) Then
                AllUserAndGroupList.RemoveAt(index)
            End If


            changedPermissions.Remove(selUserGroup)
            oldUserGroup = Nothing

            lstGroupUsers.BeginUpdate()
            lstGroupUsers.DataSource = Nothing
            lstGroupUsers.Items.Clear()
            lstGroupUsers.DataSource = AllUserAndGroupList
            lstGroupUsers.DisplayMember = "Name"
            lstGroupUsers.ValueMember = "Id"
            lstGroupUsers.EndUpdate()

            If (lstGroupUsers.Items.Count() > 0) Then
                lstGroupUsers.SelectedIndex = 0
            End If

        End If
    End Sub

    'Private Sub RemoveUsersGroups()
    '    If (DeletedUserAndGroupList IsNot Nothing) Then
    '        For Each item In DeletedUserAndGroupList
    '            If (item.Type = UserGroupType.Group) Then
    '                Using proxy = New Common.GroupFilePermissionClient()
    '                    proxy.DeletePermissionByFileAndGroup(fileId, item.Id)
    '                End Using
    '            Else
    '                Using proxy = New Common.UserFilePermissionClient()
    '                    proxy.DeletePermissionByFileAndUser(fileId, item.Id)
    '                End Using
    '            End If
    '        Next
    '    End If
    'End Sub

#End Region

#Region "Events Of METADATA"

    Private Sub dgvMetadata_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvMetadata.CellMouseUp
        If e.Button = MouseButtons.Right Then
            Me.dgvMetadata.Rows(e.RowIndex).Selected = True
            Me.rowIndex = e.RowIndex
            Me.dgvMetadata.CurrentCell = Me.dgvMetadata.Rows(e.RowIndex).Cells(1)
            Me.mnuContextMenu.Show(Me.dgvMetadata, e.Location)
            mnuContextMenu.Show(Cursor.Position)
        End If
    End Sub

    Private Sub mnuContextMenu_Click(sender As Object, e As EventArgs) Handles mnuContextMenu.Click
        If Not Me.dgvMetadata.Rows(Me.rowIndex).IsNewRow Then
            Me.dgvMetadata.Rows.RemoveAt(Me.rowIndex)
        End If
        dgvMetadata.ClearSelection()
    End Sub

    Private Sub dgvMetadata_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvMetadata.CellValidating

        Dim cell As DataGridViewCell = dgvMetadata.Rows(e.RowIndex).Cells(e.ColumnIndex)
        cell.ErrorText = String.Empty

    End Sub

#End Region

#Region "Private Methods Of METADATA"

    Private Sub BindMetadata()
        Dim async = New AsyncProvider()
        async.AddMethod("Tags", Function() New Common.LocalTagClient().GetFileTags(fileId))
        async.OnCompletion = Sub(list As IDictionary)
                                 Dim result = CType(list("Tags"), ResultInfo(Of List(Of TagInfo), Status))
                                 If ValidateResponse(result) Then
                                     allTagID = result.Data
                                     Dim dt As New DataTable()
                                     dt = ConvertToDataTable(allTagID)

                                     'Set AutoGenerateColumns False
                                     dgvMetadata.AutoGenerateColumns = False
                                     'Set Columns Count
                                     dgvMetadata.ColumnCount = 3
                                     'Add Columns
                                     dgvMetadata.Columns(0).Name = "Key"
                                     dgvMetadata.Columns(0).HeaderText = "Key"
                                     dgvMetadata.Columns(0).DataPropertyName = "TagName"
                                     dgvMetadata.Columns(1).Name = "Value"
                                     dgvMetadata.Columns(1).HeaderText = "Value"
                                     dgvMetadata.Columns(1).DataPropertyName = "TagValue"
                                     dgvMetadata.Columns(2).Name = "TagId"
                                     dgvMetadata.Columns(2).HeaderText = "TagId"
                                     dgvMetadata.Columns(2).DataPropertyName = "TagId"
                                     dgvMetadata.Columns(2).Visible = False
                                     dgvMetadata.DataSource = dt
                                 End If
                             End Sub
        async.Execute()
    End Sub

    Private Function SaveMetadata() As Boolean
        Dim isValid As Boolean = DgvValidator()
        Dim activeTagID As New List(Of String)()
        Dim maxDisplay As Integer = 0
        If allTagID.Count > 0 Then
            maxDisplay = allTagID.Max(Function(o) o.DisplayOrder)
        End If

        If isValid Then

            'check for duplicate tag name
            Dim duplicateDictionary As New Dictionary(Of String, Integer) 'value, count
            For Each row As DataGridViewRow In dgvMetadata.Rows
                If Not row.Cells(0).Value Is Nothing Then
                    Dim count As Integer = 0
                    Dim value As String = row.Cells(0).Value.ToString.ToLower
                    duplicateDictionary.TryGetValue(value, count)
                    duplicateDictionary(value) = count + 1
                End If
            Next
            For Each kv As KeyValuePair(Of String, Integer) In duplicateDictionary
                If kv.Value > 1 Then 'we have a duplicate
                    MsgBox(kv.Key & " is a duplicated value, encountered " & kv.Value & " times")
                    Return False
                End If
            Next

            For i As Integer = 0 To dgvMetadata.Rows.Count
                If dgvMetadata.Rows(i).Cells("Key").Value = "" OrElse dgvMetadata.Rows(i).Cells("Value").Value = "" Then
                    Exit For

                ElseIf dgvMetadata.Rows(i).Cells("TagId").Value Is Nothing OrElse dgvMetadata.Rows(i).Cells("TagId").Value Is DBNull.Value Then

                    Dim tagKey As String = dgvMetadata.Rows(i).Cells("Key").Value.ToString()
                    Dim tagValue As String = dgvMetadata.Rows(i).Cells("Value").Value.ToString()
                    maxDisplay = maxDisplay + 1

                    'Client side request
                    Using proxy = New Common.LocalTagClient()
                        Dim tag = New TagInfo()
                        tag.TagId = Guid.NewGuid()
                        tag.FileEntryId = fileId
                        tag.TagTypeId = 1
                        tag.TagName = tagKey
                        tag.TagValue = tagValue
                        tag.DisplayOrder = maxDisplay
                        tag.CreatedOnUTC = DateTime.UtcNow
                        Dim callresult = proxy.Post(Of TagInfo, Boolean)(tag)
                        If (Not callresult.Data) Then
                            MsgBox("Error in saving Tag")
                            Return False
                        End If
                    End Using

                Else
                    Dim TagId As String = dgvMetadata.Rows(i).Cells("TagId").Value.ToString()
                    Dim tagKey As String = dgvMetadata.Rows(i).Cells("Key").Value.ToString()
                    Dim tagValue As String = dgvMetadata.Rows(i).Cells("Value").Value.ToString()
                    activeTagID.Add(TagId)

                    'Client side request
                    Using proxy = New Common.LocalTagClient()
                        Dim tag = New TagInfo()
                        tag.TagId = New Guid(TagId)
                        tag.TagName = tagKey
                        tag.TagValue = tagValue
                        Dim callresult = proxy.Put(Of TagInfo, Boolean)(TagId, tag)
                        If (Not callresult.Data) Then
                            MsgBox("Error in saving Tag")
                            Return False
                        End If
                    End Using

                End If
            Next

            For Each obj In allTagID
                'Dim customerWithID1 As TagInfo = metaData.Find(Function(p) p.TagId = New Guid(num))
                Dim TagId As String = obj.TagId.ToString()
                Dim val As String = activeTagID.IndexOf(TagId)
                If val = -1 Then
                    Using proxy = New Common.LocalTagClient()
                        Dim callresult = proxy.Delete(TagId)
                        If (Not callresult.Data) Then
                            MsgBox("Error in saving Tag")
                            Return False
                        End If
                    End Using
                End If
            Next

        End If
        Return True
    End Function

    Private Function DgvValidator()
        Dim isValid As Boolean = True
        For Each row As DataGridViewRow In dgvMetadata.Rows
            If DBNull.Value Is row.Cells(0).Value OrElse DBNull.Value Is row.Cells(1).Value Then
                If DBNull.Value Is row.Cells(1).Value Then
                    row.Cells(1).ErrorText = "ERROR!"
                    isValid = False
                ElseIf DBNull.Value Is row.Cells(0).Value Then
                    row.Cells(0).ErrorText = "ERROR!"
                    isValid = False
                End If
            End If
        Next
        Return isValid
    End Function

    Public Function ConvertToDataTable(Of T)(ByVal list As IList(Of T)) As DataTable
        Dim table As New DataTable()
        If Not list.Any Then
            'don't know schema ....
            Return table
        End If
        Dim fields() = list.First.GetType.GetProperties
        For Each field In fields
            If field.PropertyType.Name <> Nothing Then
                table.Columns.Add(field.Name, If(Nullable.GetUnderlyingType(field.PropertyType), field.PropertyType))
            End If

        Next
        For Each item In list
            Dim row As DataRow = table.NewRow()
            For Each field In fields
                row(field.Name) = If(field.GetValue(item) <> Nothing, field.GetValue(item), DBNull.Value)
            Next
            table.Rows.Add(row)
        Next
        Return table
    End Function

#End Region

End Class