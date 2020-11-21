Imports FileMinister.Client.Common
Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud
Imports risersoft.app
Public Class Config

    Dim agentId As Guid
    Dim m_IsConfigured As Boolean

    'Shared _theform As Config

    Public ReadOnly Property IsConfigured As Boolean
        Get
            Return m_IsConfigured
        End Get
    End Property

    'Public Shared ReadOnly Property TheForm() As Config
    '    Get
    '        If _theform Is Nothing Then
    '            _theform = New Config()
    '        End If

    '        Return _theform
    '    End Get
    'End Property

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        agentId = Guid.Empty
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(agentId)
        Me.New()
        Me.agentId = agentId
    End Sub

    Private Async Sub Config_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await LoadSharesAsync()
    End Sub

    Private Async Function LoadSharesAsync() As Task
        'Dim async = New AsyncProvider()
        'async.AddMethod("Share", Function() New Common.LocalShareClient().ShareByAgentAsync(agentId))
        'async.OnCompletion = Sub(list As IDictionary)
        '                         Dim result = CType(list("Share"), ResultInfo(Of List(Of ConfigInfo)))
        '                         If (ValidateResponse(result)) Then
        '                             LoadConfig(result.Data)
        '                         End If
        '                     End Sub
        'async.Execute()

        Using client As New LocalShareClient()
            Dim result = Await client.GetSharesAsync
            If (ValidateResponse(result)) Then
                LoadConfig(result.Data)
            End If
        End Using

    End Function

    Private Sub LoadConfig(data As List(Of ConfigInfo))

        Me.Controls.Clear()


        Dim i As Integer = 0
        Dim p As New Helper



        Dim Table As TableLayoutPanel = New TableLayoutPanel()
        Table.Location = New Point(5, 5)
        Table.AutoSize = True
        Table.Name = "MainTableLayout"
        Table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Table.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddRows
        Table.Dock = DockStyle.Fill

        Me.Controls.Add(Table)

        Dim btnRefresh As New Button()
        btnRefresh.Name = "btnRefresh"
        btnRefresh.Text = "Refresh"
        Table.Controls.Add(btnRefresh, 0, i)
        AddHandler btnRefresh.Click, AddressOf btnRefresh_Click

        i = i + 1

        Dim lblMapPath As New Label()
        lblMapPath.Text = "Map Path"
        lblMapPath.AutoSize = True
        Table.Controls.Add(lblMapPath, 1, i)

        Dim lblUsers As New Label()
        lblUsers.Text = "Domain name\ User name"
        lblUsers.AutoSize = True
        Table.Controls.Add(lblUsers, 4, i)

        Dim lblPwd As New Label()
        lblPwd.Text = "Password"
        lblPwd.AutoSize = True
        Table.Controls.Add(lblPwd, 5, i)

        i = i + 1

        For Each obj In data
            Dim lblshareName As New Label()
            lblshareName.Text = obj.ShareName
            lblshareName.Name = "lblShare" + i.ToString
            lblshareName.AutoSize = True
            Table.Controls.Add(lblshareName, 0, i)

            Dim txtShare As New TextBox()
            txtShare.Width = 200
            txtShare.Name = "tbShare" + i.ToString
            txtShare.Text = obj.SharePath
            txtShare.ReadOnly = If(obj.SharePath Is Nothing, False, True)
            AddHandler txtShare.TextChanged, AddressOf Me.txtShare_TextChanged
            Table.Controls.Add(txtShare, 1, i)

            Dim lblID As New Label()
            lblID.Name = "lblShareID" + i.ToString()
            lblID.Text = obj.FileShareId.ToString()
            lblID.Visible = False
            Table.Controls.Add(lblID, 2, i)


            Dim btn As New Button()
            btn.Text = "..."
            btn.Width = 40
            btn.Name = "btnShare" + i.ToString
            btn.Enabled = If(obj.SharePath Is Nothing, True, False)
            AddHandler btn.Click, AddressOf Me.btnShare_Click
            Table.Controls.Add(btn, 3, i)

            Dim txtUser As New TextBox()
            txtUser.Width = 140
            txtUser.Text = If(obj.WindowsUser Is Nothing, Nothing, obj.WindowsUser)
            txtUser.Name = "tbUser" + i.ToString
            AddHandler txtUser.TextChanged, AddressOf Me.txtUser_TextChanged
            Table.Controls.Add(txtUser, 4, i)

            Dim txtPwd As New TextBox()
            txtPwd.Width = 140
            txtPwd.Text = If(obj.Password Is Nothing, Nothing, CommonUtils.Helper.Decrypt(obj.Password))
            txtPwd.Name = "tbPwd" + i.ToString
            txtPwd.PasswordChar = "*"
            AddHandler txtPwd.TextChanged, AddressOf Me.txtPwd_TextChanged
            Table.Controls.Add(txtPwd, 5, i)


            Dim btnTest As New Button()
            btnTest.Text = "Test"
            btnTest.Width = 50
            btnTest.Name = "btnTest" + i.ToString
            AddHandler btnTest.Click, AddressOf Me.btnTest_Click
            Table.Controls.Add(btnTest, 6, i)

            Dim btnDelete As New Button()
            btnDelete.Image = fsconfig.My.Resources.action_Cancel_16xLG
            btnDelete.Width = 50
            btnDelete.Name = "btnDelete" + i.ToString
            btnDelete.Visible = If(obj.SharePath Is Nothing, False, True)
            AddHandler btnDelete.Click, AddressOf Me.btnDelete_Click
            Table.Controls.Add(btnDelete, 7, i)

            i = i + 1

            If (txtShare.ReadOnly) Then
                epTick.SetError(txtPwd, "Correct")
                epWarning.SetError(txtPwd, "")
            Else
                epTick.SetError(txtPwd, "")
                epWarning.SetError(txtPwd, "")
            End If
        Next



        Dim btnCancel As New Button()
        btnCancel.Text = "Cancel"
        Table.Controls.Add(btnCancel, 0, i)
        AddHandler btnCancel.Click, AddressOf Me.btnCancel_Click

        Dim btnNext As New Button()
        btnNext.Name = "btnNext"
        btnNext.Text = "Next"
        Table.Controls.Add(btnNext, 0, i)
        AddHandler btnNext.Click, AddressOf Me.btnNext_Click




        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160.0F))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))

    End Sub

    Protected Sub txtShare_TextChanged(sender As Object, e As EventArgs)

        Dim t As TextBox = sender
        Dim b = t.Name.Replace("tbShare", "")
        Dim tbPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)

        epTick.SetError(tbPwd, "")
        epWarning.SetError(tbPwd, "")

    End Sub

    Protected Sub txtUser_TextChanged(sender As Object, e As EventArgs)

        Dim t As TextBox = sender
        Dim b = t.Name.Replace("tbUser", "")
        Dim tbPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)

        epTick.SetError(tbPwd, "")
        epWarning.SetError(tbPwd, "")

    End Sub

    Protected Sub txtPwd_TextChanged(sender As Object, e As EventArgs)

        Dim t As TextBox = sender
        Dim b = t.Name.Replace("tbPwd", "")
        Dim tbPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)

        epTick.SetError(tbPwd, "")
        epWarning.SetError(tbPwd, "")

    End Sub

    Protected Sub btnShare_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim shareButton As Button = CType(sender, Button)

        If fbDialog.ShowDialog = DialogResult.OK Then
            Dim b = shareButton.Name.Replace("btnShare", "")
            Dim tbx As TextBox = TryCast(Me.Controls.Find("tbShare" & b, True).FirstOrDefault(), TextBox)
            tbx.Text = fbDialog.SelectedPath
        End If

    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs)

        Dim testButton As Button = CType(sender, Button)
        Dim b = testButton.Name.Replace("btnDelete", "")

        Dim lblID As Label = TryCast(Me.Controls.Find("lblShareID" & b, True).FirstOrDefault(), Label)

        Using proxy = New LocalShareClient()
            Dim result As Boolean = proxy.DeleteShareMapping(lblID.Text).Data

            If result Then
                Dim txtPath As TextBox = TryCast(Me.Controls.Find("tbShare" & b, True).FirstOrDefault(), TextBox)
                Dim btnShare As Button = TryCast(Me.Controls.Find("btnShare" & b, True).FirstOrDefault(), Button)

                Dim txtUser As TextBox = TryCast(Me.Controls.Find("tbUser" & b, True).FirstOrDefault(), TextBox)
                Dim txtPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)

                Dim btnDelete As Button = TryCast(Me.Controls.Find("btnDelete" & b, True).FirstOrDefault(), Button)

                txtPath.ReadOnly = False
                txtPath.Text = String.Empty
                btnShare.Enabled = True

                txtUser.Text = String.Empty
                txtPwd.Text = String.Empty

                btnDelete.Visible = False

            End If

        End Using

    End Sub

    Protected Sub btnTest_Click(ByVal sender As Object, ByVal e As EventArgs)

        Dim testButton As Button = CType(sender, Button)
        Dim b = testButton.Name.Replace("btnTest", "")

        Dim tbUser As TextBox = TryCast(Me.Controls.Find("tbUser" & b, True).FirstOrDefault(), TextBox)
        Dim tbShare As TextBox = TryCast(Me.Controls.Find("tbShare" & b, True).FirstOrDefault(), TextBox)
        Dim tbPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)

        Dim delimiter As Char = "\"
        Dim substrings() As String = tbUser.Text.Split(delimiter)

        Dim pwd = tbPwd.Text.Trim()

        Dim alreadyExists = False
        Dim cntrl = TryCast(Me.Controls.Find("MainTableLayout", True).FirstOrDefault(), TableLayoutPanel)
        Dim boxes = cntrl.Controls.OfType(Of TextBox)()
        For Each box In boxes
            If box.Name.Contains("tbShare") And box.Text <> Nothing Then
                Dim bx = box.Name.Replace("tbShare", "")
                If (bx >= 0 AndAlso bx <> b) Then
                    Dim txtPath As TextBox = TryCast(Me.Controls.Find("tbShare" & bx, True).FirstOrDefault(), TextBox)
                    If (tbShare.Text.StartsWith(txtPath.Text)) Then
                        alreadyExists = True
                        Exit For
                    End If
                End If
            End If
        Next

        If (alreadyExists = True) Then
            epWarning.SetError(tbPwd, "Share path already mapped.")
            epTick.SetError(tbPwd, "")
            Return
        Else
            If (tbShare.ReadOnly = False) Then
                If (tbShare.Text = String.Empty) Then
                    epWarning.SetError(tbPwd, "Map path required.")
                    epTick.SetError(tbPwd, "")
                    Return
                ElseIf (Not System.IO.Directory.Exists(tbShare.Text)) Then
                    epWarning.SetError(tbPwd, "Folder not exist.")
                    epTick.SetError(tbPwd, "")
                    Return
                ElseIf (System.IO.Directory.GetDirectories(tbShare.Text).Length > 0 OrElse System.IO.Directory.GetFiles(tbShare.Text).Length > 0) Then
                    epWarning.SetError(tbPwd, "Folder not blank.")
                    epTick.SetError(tbPwd, "")
                    Return
                ElseIf Not String.IsNullOrEmpty(tbUser.Text) AndAlso substrings.Count <> 2 Then
                    epWarning.SetError(tbPwd, "Invalid Domain or User name.")
                    epTick.SetError(tbPwd, "")
                    Return
                ElseIf Not String.IsNullOrEmpty(tbUser.Text) AndAlso (String.IsNullOrEmpty(substrings(0)) Or String.IsNullOrEmpty(substrings(1))) Then
                    epWarning.SetError(tbPwd, "Invalid Domain or User name.")
                    epTick.SetError(tbPwd, "")
                    Return
                ElseIf Not String.IsNullOrEmpty(tbUser.Text) AndAlso String.IsNullOrEmpty(tbPwd.Text) Then
                    epWarning.SetError(tbPwd, "Invalid Password.")
                    epTick.SetError(tbPwd, "")
                    Return
                ElseIf Not String.IsNullOrEmpty(tbPwd.Text) AndAlso String.IsNullOrEmpty(tbUser.Text) Then
                    epWarning.SetError(tbPwd, "Invalid Domain or User name.")
                    epTick.SetError(tbPwd, "")
                    Return
                Else
                    Using proxy = New LocalShareClient()
                        Dim sharePath = tbShare.Text.Trim()
                        Dim lst As List(Of String) = New List(Of String)()
                        lst.Add(sharePath)
                        Dim res = proxy.IsAnyPathExists(lst, AuthData.User.AccountId)
                        If (res.Data = True) Then
                            epWarning.SetError(tbPwd, "Share path already mapped with some different account or with different user.")
                            epTick.SetError(tbPwd, "")
                            Return
                        Else
                            epTick.SetError(tbPwd, "")
                            epWarning.SetError(tbPwd, "")
                        End If
                    End Using
                End If
            End If
        End If



        Dim sDomain As String = Nothing
        Dim sUser As String = Nothing
        Dim sPass As String = Nothing

        If (substrings.Length = 2) Then
            sDomain = substrings(0)
            sUser = substrings(1)
            sPass = pwd
        End If


        Dim isAllowedAccess As Boolean = False
        Try
            Using CommonUtils.Helper.Impersonate(sUser, sDomain, sPass)

                Dim testFilePath = tbShare.Text & "\" & Guid.NewGuid().ToString()
                IO.Directory.CreateDirectory(testFilePath)
                IO.Directory.Delete(testFilePath)
                isAllowedAccess = True


                ''Insert your code that runs under the security context of a specific user here.
                'Dim isAllowedAccess As Boolean
                'Try

                '    Dim accessControlList = System.IO.Directory.GetAccessControl(tbShare.Text)
                '    'Dim accessControlList1 = System.IO.Directory.GetAccessControl(tbShare.Text, System.Security.AccessControl.AccessControlSections.All)

                '    For Each rule As System.Security.AccessControl.FileSystemAccessRule In accessControlList.GetAccessRules(True, True, GetType(System.Security.Principal.NTAccount))

                '        If isAllowedAccess Then
                '            Exit For
                '        End If


                '        If rule.IdentityReference.Value.Equals(tbUser.Text, StringComparison.InvariantCultureIgnoreCase) Then
                '            'And rule.FileSystemRights.Equals(System.Security.AccessControl.FileSystemRights.Write) 
                '            Dim rights() As String = rule.FileSystemRights.ToString().Split(",")
                '            If rule.AccessControlType.Equals(System.Security.AccessControl.AccessControlType.Allow) Then
                '                For Each right As String In [Enum].GetNames(GetType(AccessControl.FileSystemRights))
                '                    If rights.ToList().IndexOf(right) <> -1 Then
                '                        isAllowedAccess = True
                '                        Exit For
                '                    End If
                '                Next
                '            End If
                '        End If
                '    Next

                'Catch ex As UnauthorizedAccessException

                'Catch ex As Exception
                '    MessageBox.Show(ex.Message)
                'End Try

            End Using
        Catch ex As UnauthorizedAccessException
            MessageBoxHelper.ShowErrorMessage("Unable to access " + tbShare.Text)
        Catch ex As Exception
            MessageBoxHelper.ShowErrorMessage(ex.Message)
        End Try


        If isAllowedAccess Then
            epTick.SetError(tbPwd, "Correct")
            epWarning.SetError(tbPwd, "")
        End If

        Dim isValid As Boolean

        For Each box In boxes
            If box.Name.Contains("tbPwd") Then
                If epWarning.GetError(box) = "" And epTick.GetError(box) = "Correct" Then
                    isValid = True
                    Exit For
                Else
                    isValid = False
                End If
            End If
        Next
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.Close()
    End Sub

    Protected Sub btnNext_Click(ByVal sender As Object, ByVal e As EventArgs)

        Dim cntrl = TryCast(Me.Controls.Find("MainTableLayout", True).FirstOrDefault(), TableLayoutPanel)
        Dim boxes = cntrl.Controls.OfType(Of TextBox)()

        Dim isValid As Boolean = True

        For Each box In boxes
            If box.Name.Contains("tbShare") And box.Text <> Nothing Then
                Dim b = box.Name.Replace("tbShare", "")
                Dim tbPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)
                Dim ep As String = epTick.GetError(tbPwd)
                If (Not ep.Contains("Correct")) Then
                    MessageBoxHelper.ShowWarningMessage("Please test share path first!")
                    isValid = False
                    Return
                End If
            End If
        Next

        If isValid Then
            Dim lst As New List(Of ConfigInfo)
            Using proxy = New LocalShareClient()
                For Each box In boxes
                    If box.Name.Contains("tbShare") And box.Text <> Nothing Then
                        Dim b = box.Name.Replace("tbShare", "")
                        Dim lblID As Label = TryCast(Me.Controls.Find("lblShareID" & b, True).FirstOrDefault(), Label)
                        Dim lblShare As Label = TryCast(Me.Controls.Find("lblShare" & b, True).FirstOrDefault(), Label)
                        Dim tbUser As TextBox = TryCast(Me.Controls.Find("tbUser" & b, True).FirstOrDefault(), TextBox)
                        Dim tbPwd As TextBox = TryCast(Me.Controls.Find("tbPwd" & b, True).FirstOrDefault(), TextBox)

                        Dim id As Short = lblID.Text
                        Dim config = New ConfigInfo()
                        'config.UserId = 1
                        config.FileShareId = id
                        config.ShareName = lblShare.Text
                        config.SharePath = box.Text
                        If (Not String.IsNullOrEmpty(tbUser.Text.Trim())) Then
                            config.WindowsUser = tbUser.Text
                        End If
                        If (Not String.IsNullOrEmpty(tbPwd.Text.Trim())) Then
                            config.Password = CommonUtils.Helper.Encrypt(tbPwd.Text.Trim())
                        End If
                        config.CreatedOnUTC = System.DateTime.UtcNow
                        lst.Add(config)
                    End If
                Next

                Dim result = proxy.AddAll(lst).Data
                If result Then
                    m_IsConfigured = True
                Else
                    m_IsConfigured = False
                End If

            End Using

        End If
        If m_IsConfigured Then
            Me.Close()
        End If
    End Sub

    Protected Async Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
        Using localAgent As New LocalAgentClient()
            Await localAgent.SyncSharesAsync(Guid.Empty)
        End Using


        'Dim async = New AsyncProvider()
        'async.AddMethod("Share", Function() New Common.LocalAgentClient().SyncShares(agentId))
        'async.OnCompletion = Sub(list As IDictionary)
        '                         Dim result = CType(list("Share"), ResultInfo(Of Boolean))
        '                         If (ValidateResponse(result)) Then
        '                             LoadShares()
        '                         End If
        '                     End Sub
        'async.Execute()

        Using client As New LocalAgentClient()
            Dim result = Await client.SyncSharesAsync(agentId)
            If (ValidateResponse(result)) Then
                Await LoadSharesAsync()
            End If
        End Using

    End Sub

    Private Function ValidateFolder(ByVal path As String) As String
        Dim errorMessage As String = String.Empty
        If (Not System.IO.Directory.Exists(path)) Then
            errorMessage = "Folder doesn't exist."
        End If
        Return errorMessage
    End Function


End Class