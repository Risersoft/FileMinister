Imports risersoft.shared.portable.Model
Imports System.Net
Imports System.IO
Imports FileMinister.Client.Common
Imports System.Security.Authentication
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.cloud

Public Class History

#Region "Declaration"

    Public Property fileId As Guid
    Public Property shareId As Integer
    Public Property file As FileEntryInfo
    Public Property accountId As Guid = AuthData.User.AccountId

    Dim userId As Integer = 0
    Dim userName As String
    Dim password As String
    Dim isDeleteVisible = True

#End Region

#Region "Constructor And Page Load And Other Events"

    Public Sub New(fileParam As FileEntryInfo)

        file = fileParam
        fileId = fileParam.FileEntryId
        shareId = fileParam.FileShareId

        InitializeComponent()

    End Sub

    Private Sub History_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Bind()
        DisableButton()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        DisableButton()

        If (dgvHistory.Rows.Count() > 1) Then
            If (dgvHistory.SelectedRows.Count() > 0) Then

                Dim sCell = dgvHistory.SelectedRows(0).Cells("FileSystemEntryVersionId")
                Dim Id = New Guid(sCell.Value.ToString())

                Using proxy = New Common.HistoryClient()
                    If (proxy.DeleteFileVersion(Id).Data = True) Then
                        Using proxyC = New Common.LocalHistoryClient()
                            proxyC.DeleteFileVersion(Id)
                        End Using
                    Else
                        MessageBoxHelper.ShowErrorMessage("Error while deleting version on server")
                    End If
                End Using
            Else
                MessageBoxHelper.ShowInfoMessage("No selection is made, Please select a row")
            End If

        Else
            MessageBoxHelper.ShowErrorMessage("Can not delete all versions of the file")
        End If
        Bind()
    End Sub

    Private Async Sub btnDlink_Click(sender As Object, e As EventArgs) Handles btnDlink.Click
        Dim selFileId As Guid = New Guid(dgvHistory.SelectedRows(0).Cells("FileSystemEntryId").Value().ToString())
        Using proxy = New Common.FileClient()
            Dim result = Await proxy.RemoveFileLinkAsync(selFileId)
            btnDlink.Enabled = False
            Bind()
        End Using
    End Sub

    'Private Sub dgvPermissions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvHistory.CellContentClick
    '    Dim senderGrid = DirectCast(sender, DataGridView)
    '    If e.RowIndex >= 0 Then
    '        If (e.RowIndex = 0) Then
    '            DisableButton()
    '            btnSaveAs.Enabled = True
    '        Else
    '            EnableButton()
    '        End If

    '        If (dgvHistory.Rows(e.RowIndex).Cells("showDelink").Value.ToString() = "1") Then
    '            btnDlink.Enabled = True
    '        Else
    '            btnDlink.Enabled = False
    '        End If

    '    End If
    'End Sub

    Private Sub dgvHistory_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvHistory.CellClick
        If (file.FileEntryTypeId = FileType.Folder) Then
            Return
        End If

        Dim senderGrid = DirectCast(sender, DataGridView)
        If e.RowIndex >= 0 Then
            If (e.RowIndex = 0) Then
                DisableButton()
                btnSaveAs.Enabled = True
            Else
                EnableButton()
            End If

            If (dgvHistory.Rows(e.RowIndex).Cells("showDelink").Value.ToString() = "1") Then
                btnDlink.Enabled = True
            Else
                btnDlink.Enabled = False
            End If
        Else
            DisableButton()
            dgvHistory.ClearSelection()
        End If
    End Sub

    Private Sub dgvHistory_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgvHistory.DataBindingComplete
        Dim gridView As DataGridView
        gridView = CType(sender, DataGridView)
        gridView.ClearSelection()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        Dim bytesIn As Double = Double.Parse(e.BytesReceived.ToString())
        Dim totalBytes As Double = Double.Parse(e.TotalBytesToReceive.ToString())
        Dim percentage As Double = bytesIn / totalBytes * 100

        ProgressBar1.Visible = True
        ProgressBar1.Value = Int32.Parse(Math.Truncate(percentage).ToString())
    End Sub

    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        MessageBoxHelper.ShowInfoMessage("Download Complete")
        ProgressBar1.Value = 0
        ProgressBar1.Visible = False


        'EnableButton()

        'If dgvHistory.SelectedRows(0).Index >= 0 Then
        '    If (dgvHistory.SelectedRows(0).Index = 0) Then
        '        DisableButton()
        '        btnSaveAs.Enabled = True
        '    Else
        '        EnableButton()
        '    End If

        '    If (dgvHistory.Rows(dgvHistory.SelectedRows(0).Index).Cells("showDelink").Value.ToString() = "1") Then
        '        btnDlink.Enabled = True
        '    Else
        '        btnDlink.Enabled = False
        '    End If

        'End If

    End Sub

    'TODO - NEED TO TEST DOWNLOAD FILE FROM AZURE
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        DisableButton()

        If (dgvHistory.SelectedRows.Count() > 0) Then

            Dim sCellServerFileSystemEntryName = dgvHistory.SelectedRows(0).Cells("ServerFileSystemEntryName")
            Dim ServerFileSystemEntryName = New Guid(sCellServerFileSystemEntryName.Value.ToString())

            Dim sCellFileShareId = dgvHistory.SelectedRows(0).Cells("FileShareId")
            Dim FileShareId = CType(sCellFileShareId.Value.ToString(), Integer)

            Dim sCellFileSystemEntryRelativePath = dgvHistory.SelectedRows(0).Cells("FileSystemEntryRelativePath")
            Dim FileSystemEntryRelativePath = sCellFileSystemEntryRelativePath.Value.ToString()

            Dim versionCell = dgvHistory.SelectedRows(0).Cells("VersionNumber")
            Dim version = versionCell.Value.ToString()




            Dim async = New AsyncProvider()
            async.AddMethod("GetSharedAccessSignatureUrl", Function() New Common.SyncClient().GetSharedAccessSignatureUrl(FileShareId, ServerFileSystemEntryName, 0, file.FileEntryId))


            async.OnCompletion = Sub(list As IDictionary)
                                     Dim result = CType(list("GetSharedAccessSignatureUrl"), ResultInfo(Of Uri, Status))
                                     If (ValidateResponse(result)) Then
                                         Dim fileUri = result.Data

                                         Dim sDomain As String = ""
                                         Dim sUser As String = ""
                                         Dim sPass As String = ""


                                         If (String.IsNullOrEmpty(userName) = False) Then
                                             Dim delimiter As Char = "\"
                                             Dim substrings() As String = userName.Split(delimiter)

                                             sDomain = substrings(0)
                                             sUser = substrings(1)
                                             sPass = CommonUtils.Helper.Decrypt(password)
                                         End If

                                         Try
                                             Using CommonUtils.Helper.Impersonate(sUser, sDomain, sPass)
                                                 SaveFile(fileUri, FileSystemEntryRelativePath, version)
                                             End Using
                                         Catch ex As AuthenticationException
                                             MessageBoxHelper.ShowErrorMessage("Invaid username or password." & vbNewLine & "Please go to share configuration option to validate the user")
                                         Catch ex As Exception
                                             MessageBoxHelper.ShowErrorMessage(ex.Message)
                                         End Try

                                     End If
                                 End Sub

            async.Execute()
        Else
            MessageBoxHelper.ShowInfoMessage("No selection is made, Please select a row")
        End If

    End Sub

    'TODO - NEED TO TEST DOWNLOAD FILE FROM AZURE
    Private Sub btnSaveAs_Click(sender As Object, e As EventArgs) Handles btnSaveAs.Click

        DisableButton()

        If (dgvHistory.SelectedRows.Count() > 0) Then

            Dim sCellServerFileSystemEntryName = dgvHistory.SelectedRows(0).Cells("ServerFileSystemEntryName")
            Dim ServerFileSystemEntryName = New Guid(sCellServerFileSystemEntryName.Value.ToString())

            Dim sCellFileShareId = dgvHistory.SelectedRows(0).Cells("FileShareId")
            Dim FileShareId = CType(sCellFileShareId.Value.ToString(), Integer)

            Dim sCellFileSystemEntryRelativePath = dgvHistory.SelectedRows(0).Cells("FileSystemEntryRelativePath")
            Dim FileSystemEntryRelativePath = sCellFileSystemEntryRelativePath.Value.ToString()

            Dim versionCell = dgvHistory.SelectedRows(0).Cells("VersionNumber")
            Dim version = versionCell.Value.ToString()

            Dim async = New AsyncProvider()
            async.AddMethod("GetSharedAccessSignatureUrl", Function() New Common.SyncClient().GetSharedAccessSignatureUrl(FileShareId, ServerFileSystemEntryName, 0, file.FileEntryId))


            async.OnCompletion = Sub(list As IDictionary)
                                     Dim result = CType(list("GetSharedAccessSignatureUrl"), ResultInfo(Of Uri, Status))
                                     If (ValidateResponse(result)) Then
                                         Dim fileUri = result.Data

                                         Dim fileNameWithExtension = Path.GetFileName(FileSystemEntryRelativePath)
                                         Dim fileExtension = Path.GetExtension(FileSystemEntryRelativePath)

                                         SaveFileDialog1.DefaultExt = "*" + fileExtension
                                         SaveFileDialog1.Filter = "|*" + fileExtension

                                         SaveFileDialog1.FileName = ""

                                         dgvHistory.ClearSelection()

                                         If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                                             Dim client As WebClient = New WebClient

                                             AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
                                             AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted

                                             client.DownloadFileAsync(fileUri, SaveFileDialog1.FileName)

                                         End If
                                     End If
                                 End Sub

            async.Execute()
        Else
            MessageBoxHelper.ShowInfoMessage("No selection is made, Please select a row")
        End If

    End Sub

#End Region

#Region "Private Methods"

    Private Sub SaveFile(ByVal serverPath As Uri, fileSystemEntryRelativePath As String, version As String)


        Dim fileNameWithExtension = Path.GetFileName(fileSystemEntryRelativePath)

        Dim destinationFolder = Path.GetDirectoryName(fileSystemEntryRelativePath)

        Dim finalFileName = "##_" + version + "_" + fileNameWithExtension

        Dim destinationPath = Path.Combine(destinationFolder, finalFileName)


        Dim client As WebClient = New WebClient


        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted


        client.DownloadFileAsync(serverPath, destinationPath)

        DisableButton()


    End Sub

    Private Sub Bind()
        dgvHistory.DataSource = Nothing
        dgvHistory.Refresh()
        dgvHistory.Rows.Clear()
        dgvHistory.Columns.Clear()
        dgvHistory.ClearSelection()

        Dim async = New AsyncProvider()
        async.AddMethod("FileVersions", Function() New Common.LocalHistoryClient().GetAllFileVersions(fileId))
        'async.AddMethod("File", Function() New Common.LocalFileClient().Get(Of FileSystemEntryInfo)(fileId.ToString()))
        async.AddMethod("UserShare", Function() New Common.LocalUserShareClient().Get(Of UserShareInfo)(shareId))
        async.OnCompletion = Sub(list As IDictionary)
                                 Dim result = CType(list("FileVersions"), ResultInfo(Of List(Of FileVersionInfo), Status))
                                 If (ValidateResponse(result)) Then
                                     Dim data = result.Data
                                     Me.dgvHistory.DataSource = data

                                     For Each col In dgvHistory.Columns
                                         col.Visible = False
                                     Next

                                     Me.dgvHistory.Columns("VersionNumber").Visible = True
                                     Me.dgvHistory.Columns("FileSystemEntryRelativePath").Visible = True
                                     Me.dgvHistory.Columns("CreatedOnUTC").Visible = True
                                     Me.dgvHistory.Columns("User").Visible = True

                                     Me.dgvHistory.Columns("VersionNumber").HeaderText = "Version"
                                     Me.dgvHistory.Columns("FileSystemEntryRelativePath").HeaderText = "Path"
                                     Me.dgvHistory.Columns("CreatedOnUTC").HeaderText = "Date"
                                     Me.dgvHistory.Columns("User").HeaderText = "User"

                                     Me.dgvHistory.Columns("FileSystemEntryRelativePath").Width = 210
                                     Me.dgvHistory.Columns("CreatedOnUTC").Width = 160
                                     Me.dgvHistory.Columns("User").Width = 160

                                     'dgvHistory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells

                                     'Me.dgvHistory.Columns("FileSystemEntryRelativePath").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
                                     'Me.dgvHistory.Columns("VersionNumber").AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                                     'Me.dgvHistory.Columns("CreatedOnUTC").AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                                     'Me.dgvHistory.Columns("User").AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells

                                     'dgvHistory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
                                     'Me.dgvHistory.Columns("FileSystemEntryRelativePath").DefaultCellStyle.WrapMode = DataGridViewTriState.True
                                     'dgvHistory.DefaultCellStyle.WrapMode = DataGridViewTriState.True
                                     'dgvHistory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells

                                     dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                                     dgvHistory.RowsDefaultCellStyle.SelectionBackColor = Color.LightGray
                                     dgvHistory.MultiSelect = False
                                     dgvHistory.ClearSelection()
                                     dgvHistory.DefaultCellStyle.SelectionBackColor = dgvHistory.DefaultCellStyle.BackColor
                                     dgvHistory.DefaultCellStyle.SelectionForeColor = dgvHistory.DefaultCellStyle.ForeColor
                                     dgvHistory.ReadOnly = True
                                     dgvHistory.AllowUserToResizeColumns = True




                                     Dim resultUserShare = CType(list("UserShare"), ResultInfo(Of UserShareInfo, Status))
                                     If (ValidateResponse(resultUserShare)) Then
                                         Dim userShareObj As UserShareInfo = resultUserShare.Data
                                         If (userShareObj IsNot Nothing) Then
                                             userName = userShareObj.WindowsUser
                                             password = userShareObj.Password
                                         End If
                                     End If

                                 End If
                             End Sub
        async.Execute()

    End Sub

    Private Sub EnableButton()
        If (file.FileEntryTypeId = FileType.Folder) Then
            Return
        End If
        Dim prmsnprovider As PermissionProvider = New PermissionProvider(file)
        If (prmsnprovider.CanDeleteFileVersion() = True) Then
            btnDelete.Enabled = True
        End If
        btnSave.Enabled = True
        btnSaveAs.Enabled = True
        btnDlink.Enabled = False
    End Sub

    Private Sub DisableButton()
        btnDelete.Enabled = False
        btnSave.Enabled = False
        btnSaveAs.Enabled = False
        btnDlink.Enabled = False
    End Sub

#End Region

#Region "OLD CODE"
    'Dim fileId As Guid = New Guid("32DE5F6A-BC54-45DE-91B9-F3FCD4EDB526")
    'Dim nextFileId As Guid = New Guid("32DE5F6A-BC54-45DE-91B9-F3FCD4EDB526")
    'Dim accountId As Integer = 21
    'Dim shareId As Integer = 0



    'Private Sub BindOld()

    '    Me.dgvHistory.DataSource = Nothing
    '    Me.dgvHistory.Refresh()
    '    Me.dgvHistory.Rows.Clear()
    '    Me.dgvHistory.Columns.Clear()

    '    Using proxy = New Common.HistoryClient()
    '        Dim data = proxy.GetAllFileVersions(fileId).Data
    '        For Each item In data
    '            Using userproxy = New Common.OrganizationClient()
    '                Dim userdata = userproxy.GetMyAccountUser(item.CreatedByUserId, accountId).Data
    '                item.User = userdata("userName")
    '            End Using

    '        Next

    '        Me.dgvHistory.DataSource = data


    '        Me.dgvHistory.Columns(0).Visible = False
    '        Me.dgvHistory.Columns(3).Visible = False

    '        For Each col In dgvHistory.Columns
    '            If (col.index > 5) Then
    '                col.Visible = False
    '            End If
    '        Next
    '    End Using


    '    Using proxyFileClient = New Common.FileClient()
    '        Dim file As FileSystemEntryInfo = proxyFileClient.Get(Of FileSystemEntryInfo)(fileId.ToString()).Data
    '        shareId = file.ShareId
    '    End Using

    '    If (shareId > 0) Then
    '        Using proxyUserShareClient = New Common.UserShareClient()
    '            Dim userShareObj As UserShareInfo = proxyUserShareClient.Get(Of UserShareInfo)(shareId).Data
    '            If (userShareObj IsNot Nothing) Then
    '                userName = userShareObj.WindowsUser
    '                password = userShareObj.Password
    '            End If
    '        End Using
    '    End If



    '    Me.dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
    '    dgvHistory.RowsDefaultCellStyle.SelectionBackColor = Color.LightGray

    '    dgvHistory.MultiSelect = False
    '    dgvHistory.ClearSelection()

    '    dgvHistory.DefaultCellStyle.SelectionBackColor = dgvHistory.DefaultCellStyle.BackColor
    '    dgvHistory.DefaultCellStyle.SelectionForeColor = dgvHistory.DefaultCellStyle.ForeColor

    '    dgvHistory.ReadOnly = True

    '    dgvHistory.AllowUserToResizeColumns = False

    '    dgvHistory.Columns(0).Width = 10
    '    dgvHistory.Columns(1).Width = 50
    '    dgvHistory.Columns(2).Width = 120
    '    dgvHistory.Columns(3).Width = 10
    '    dgvHistory.Columns(4).Width = 120
    '    dgvHistory.Columns(5).Width = 250

    'End Sub

    'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

    '    If (dgvHistory.Rows.Count() > 1) Then
    '        If (dgvHistory.SelectedRows.Count() > 0) Then

    '            Dim sCell = dgvHistory.SelectedRows(0).Cells(0)
    '            Dim Id = New Guid(sCell.Value.ToString())

    '            Using proxy = New Common.HistoryServerClient()
    '                If (proxy.DeleteFileVersion(Id).Data = True) Then
    '                    Using proxyC = New Common.HistoryClient()
    '                        proxyC.DeleteFileVersion(Id)
    '                    End Using
    '                Else
    '                    MessageBox.Show("Error while deleting version on server")
    '                End If
    '            End Using
    '        Else
    '            MessageBox.Show("No selection is made, Please select a row")
    '        End If

    '    Else
    '        MessageBox.Show("Can not delete all versions of a file")
    '    End If

    'End Sub

    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    '    If (dgvHistory.SelectedRows.Count() > 0) Then

    '        Dim pathCell = dgvHistory.SelectedRows(0).Cells(5)
    '        Dim fPath = pathCell.Value.ToString()

    '        If (File.Exists(fPath)) Then

    '            If (String.IsNullOrEmpty(userName) = False) Then
    '                Dim delimiter As Char = "\"
    '                Dim substrings() As String = userName.Split(delimiter)

    '                Dim sDomain As String = substrings(0)
    '                Dim sUser As String = substrings(1)
    '                Dim sPass As String = password

    '                Try
    '                    Using New Impersonator(sUser, sDomain, sPass)
    '                        SaveFile(fPath)
    '                    End Using
    '                Catch ex As AuthenticationException
    '                    MessageBox.Show("Invaid username or password." & vbNewLine & "Please go to share configuration option to validate the user", "Cloud Sync", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '                Catch ex As Exception
    '                    MessageBox.Show(ex.Message, "Cloud Sync", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '                End Try

    '                'Dim Impersonator As New MessageHelper
    '                '    If Impersonator.impersonateValidUser(sUser, sDomain, sPass) Then
    '                '        SaveFile(fPath)
    '                '        Impersonator.undoImpersonation()
    '                '    Else
    '                '        SaveFile(fPath)
    '                '    End If
    '                '    Else
    '                '    SaveFile(fPath)
    '            End If
    '        Else
    '            MessageBox.Show("Selected file does not exist")
    '        End If
    '    Else
    '        MessageBox.Show("No selection is made, Please select a row")
    '    End If

    'End Sub

    'Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    '    If (dgvHistory.SelectedRows.Count() > 0) Then


    '        Dim pathCell = dgvHistory.SelectedRows(0).Cells(5)
    '        Dim fPath = pathCell.Value.ToString()

    '        If (File.Exists(fPath)) Then

    '            Dim versionCell = dgvHistory.SelectedRows(0).Cells(1)
    '            Dim version = versionCell.Value.ToString()

    '            Dim fileNameWithExtension = Path.GetFileName(fPath)
    '            Dim fileExtension = Path.GetExtension(fPath)

    '            SaveFileDialog1.DefaultExt = "*" + fileExtension
    '            SaveFileDialog1.Filter = "|*" + fileExtension


    '            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
    '                Dim client As WebClient = New WebClient

    '                AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
    '                AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted


    '                client.DownloadFileAsync(New Uri(fPath), SaveFileDialog1.FileName)

    '                DisableButton()

    '            End If

    '        Else
    '            MessageBox.Show("Selected file does not exist")
    '        End If
    '    Else
    '        MessageBox.Show("No selection is made, Please select a row")
    '    End If


    'End Sub

    'Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
    '    Dim selFileId As Guid = New Guid(dgvHistory.SelectedRows(0).Cells(8).Value().ToString())
    '    Using proxy = New Common.FileClient()
    '        proxy.RemoveFileLink(selFileId)
    '        'nextFileId = fileId
    '    End Using
    '    Button4.Enabled = False
    '    Bind()
    'End Sub



    'Private Sub SaveFile(ByVal fPath As String)

    '    Dim versionCell = dgvHistory.SelectedRows(0).Cells(1)
    '    Dim version = versionCell.Value.ToString()

    '    Dim fileNameWithExtension = Path.GetFileName(fPath)

    '    Dim destinationFolder = Path.GetDirectoryName(fPath)

    '    Dim finalFileName = "##_" + version + "_" + fileNameWithExtension

    '    Dim destinationPath = Path.Combine(destinationFolder, finalFileName)


    '    Dim client As WebClient = New WebClient


    '    AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
    '    AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted


    '    client.DownloadFileAsync(New Uri(fPath), destinationPath)

    '    DisableButton()


    'End Sub

    'Private Sub DisableButton()
    '    Button1.Enabled = False
    '    Button2.Enabled = False
    '    Button3.Enabled = False
    '    Button4.Enabled = False
    'End Sub

    'Private Sub EnableButton()
    '    Button1.Enabled = True
    '    If (PermissionProvider.CanDeleteFileVersion(fileId) = False) Then
    '        Button2.Enabled = True
    '    End If
    '    Button3.Enabled = True
    '    Button4.Enabled = False
    'End Sub

    'Private Sub dgvPermissions_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPermissions.CellClick
    '    Dim rIndex = e.RowIndex
    '    If (rIndex = 0) Then
    '        DisableButton()
    '        Button3.Enabled = True
    '    End If
    'End Sub

    'Private Sub dgvPermissions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvHistory.CellContentClick
    '    Dim senderGrid = DirectCast(sender, DataGridView)
    '    'If TypeOf senderGrid.Columns(e.ColumnIndex) Is DataGridViewButtonColumn AndAlso
    '    '   e.RowIndex >= 0 Then
    '    If e.RowIndex >= 0 Then
    '        'Dim selFileId As Guid = New Guid(dgvPermissions.Rows(e.RowIndex).Cells(7).Value().ToString())
    '        'Using proxy = New FileClient()
    '        '    proxy.RemoveFileLink(selFileId)
    '        '    nextFileId = fileId
    '        'End Using
    '        'Bind()

    '        'nextFileId = New Guid(dgvPermissions.Rows(e.RowIndex).Cells(7).Value.ToString())

    '        If (e.RowIndex = 0) Then
    '            DisableButton()
    '            Button3.Enabled = True

    '        Else
    '            EnableButton()

    '        End If

    '        If (dgvHistory.Rows(e.RowIndex).Cells(6).Value.ToString() = "1") Then
    '            Button4.Enabled = True
    '        Else
    '            Button4.Enabled = False
    '        End If

    '    End If
    'End Sub


#End Region


End Class