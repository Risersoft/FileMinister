Imports risersoft.shared.portable.Model
Imports System.Net
Imports System.IO
Imports risersoft.shared.portable.Enums
Imports risersoft.shared.cloud

Public Class UnResolvedConflicts

#Region "Declaration"
    Public Property fileId As Guid
    Public Property shareId As Integer
    Public Property file As FileEntryInfo
    Public Property accountId As Guid = AuthData.User.AccountId

#End Region

#Region "Constructor And Page Load And Other Events"

    Public Sub New(fileParam As FileEntryInfo)

        file = fileParam
        fileId = fileParam.FileEntryId
        shareId = fileParam.FileShareId

        InitializeComponent()

    End Sub

    Private Async Sub UnResolvedConflicts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'btnSaveAs.Enabled = False
        Await BindAsync()
    End Sub

    Private Sub dgvConflicts_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvConflicts.CellMouseDown

        Me.dgvConflicts.ClearSelection()

        cmsMain.Items.Clear()

        If e.Button = Windows.Forms.MouseButtons.Right AndAlso e.RowIndex >= 0 Then

            Me.dgvConflicts.Rows(e.RowIndex).Selected = True

            Dim ds = CType(Me.dgvConflicts.DataSource, List(Of risersoft.shared.portable.Model.FileVersionConflictInfo))
            Dim rw = ds(e.RowIndex)

            Dim permission As Common.PermissionProvider = New Common.PermissionProvider(rw.FileEntryId)

            If (Common.ConflictPermissionProvider.CanRequestConflictFileUpload(rw, permission.GetPermissions())) Then
                Dim tsmi1 As New ToolStripMenuItem("Request User File")
                tsmi1.Tag = CommandType.RequestUserfile
                cmsMain.Items.Add(tsmi1)
            End If
            If (Common.ConflictPermissionProvider.CanResolveOtherConflictUsingServer(rw, permission.GetPermissions())) Then
                Dim tsmi2 As New ToolStripMenuItem("Resolve Conflict Using Server")
                tsmi2.Tag = CommandType.ResolveUsingServer
                cmsMain.Items.Add(tsmi2)
            End If
            If (Common.ConflictPermissionProvider.CanResolveOtherConflictUsingUser(rw, permission.GetPermissions())) Then
                Dim tsmi3 As New ToolStripMenuItem("Resolve Conflict Using User")
                tsmi3.Tag = CommandType.ResolveUsingUser
                cmsMain.Items.Add(tsmi3)
            End If

            Dim selrw = dgvConflicts.Rows(e.RowIndex)
            Dim FileSystemEntryVersionConflictRequestStatusId = selrw.Cells("FileSystemEntryVersionConflictRequestStatusId").Value()
            If (FileSystemEntryVersionConflictRequestStatusId IsNot Nothing AndAlso FileSystemEntryVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded) Then
                Dim tsmi4 As New ToolStripMenuItem("Save As")
                tsmi4.Tag = CommandType.SaveUploadedConflict
                cmsMain.Items.Add(tsmi4)
            End If

            'Me.rowIndex = e.RowIndex
            Me.dgvConflicts.CurrentCell = Me.dgvConflicts.Rows(e.RowIndex).Cells("UserName")
            Me.cmsMain.Show(Me.dgvConflicts, e.Location)
            cmsMain.Show(Cursor.Position)

        End If
    End Sub

    Private Async Sub CmsMain_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles cmsMain.ItemClicked
        Dim commandType__1 = If(e.ClickedItem.Tag IsNot Nothing, DirectCast(e.ClickedItem.Tag, CommandType), CommandType.None)
        Select Case commandType__1
            Case CommandType.RequestUserfile
                Await RequestUserFileAsync()
                Await BindAsync()
                Exit Select
            Case CommandType.ResolveUsingServer
                Await ResolveOthersConflictUsingServerAsync()
                Await BindAsync()
                Exit Select
            Case CommandType.ResolveUsingUser
                Await ResolveOthersConflictUsingOthersAsync()
                Await BindAsync()
                Exit Select
            Case CommandType.SaveUploadedConflict
                Await SaveFileAs()
                Exit Select
        End Select
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

        'If (dgvConflicts.SelectedRows.Count > 0) Then
        '    btnSaveAs.Enabled = True
        'Else
        '    btnSaveAs.Enabled = False
        'End If

    End Sub


#End Region

#Region "Private Methods"

    Private Async Function BindAsync() As Task

        Me.dgvConflicts.DataSource = Nothing
        Me.dgvConflicts.ClearSelection()
        Me.dgvConflicts.Refresh()
        Me.dgvConflicts.Rows.Clear()
        Me.dgvConflicts.Columns.Clear()

        Using Client As New Common.FileClient()
            Dim result = Await Client.GetOtherUsersUnresolvedConflictsAsync(fileId)
            If (ValidateResponse(result)) Then

                Me.dgvConflicts.DataSource = result.Data

                For Each col In dgvConflicts.Columns
                    col.Visible = False
                Next

                Me.dgvConflicts.Columns("UserName").Visible = True
                Me.dgvConflicts.Columns("VersionNumber").Visible = True
                Me.dgvConflicts.Columns("FileSystemEntryVersionConflictType").Visible = True
                Me.dgvConflicts.Columns("FileSystemEntryVersionConflictRequestStatus").Visible = True

                Me.dgvConflicts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                dgvConflicts.RowsDefaultCellStyle.SelectionBackColor = Color.LightGray

                dgvConflicts.MultiSelect = False
                dgvConflicts.ClearSelection()

                dgvConflicts.DefaultCellStyle.SelectionBackColor = dgvConflicts.DefaultCellStyle.BackColor
                dgvConflicts.DefaultCellStyle.SelectionForeColor = dgvConflicts.DefaultCellStyle.ForeColor

                dgvConflicts.ReadOnly = True

                dgvConflicts.AllowUserToResizeColumns = False

                '
                'dgvConflicts.Columns("LatestFileSystemEntryVersionFileSystemEntryPathRelativeToShare").Width = 380

                dgvConflicts.Columns("FileSystemEntryVersionConflictRequestStatus").Width = 150
                dgvConflicts.Columns("FileSystemEntryVersionConflictType").Width = 150
                dgvConflicts.Columns("UserName").Width = 150
                dgvConflicts.Columns("VersionNumber").Width = 100


                dgvConflicts.Columns("FileSystemEntryVersionConflictRequestStatus").HeaderText = "User File Upload Status"
                dgvConflicts.Columns("FileSystemEntryVersionConflictType").HeaderText = "Conflict Type"
                dgvConflicts.Columns("UserName").HeaderText = "User"
                dgvConflicts.Columns("VersionNumber").HeaderText = "Version"

            End If
        End Using


        'Dim async = New AsyncProvider()
        'async.AddMethod("GetOtherUsersUnresolvedConflicts", Function() New Common.FileClient().GetOtherUsersUnresolvedConflicts(fileId))
        'async.OnCompletion = Sub(list As IDictionary)
        '                         Dim result = CType(list("GetOtherUsersUnresolvedConflicts"), ResultInfo(Of List(Of FileSystemEntryVersionConflictInfo)))
        '                         If (ValidateResponse(result)) Then

        '                             Me.dgvConflicts.DataSource = result.Data

        '                             For Each col In dgvConflicts.Columns
        '                                 col.Visible = False
        '                             Next

        '                             Me.dgvConflicts.Columns("UserName").Visible = True
        '                             Me.dgvConflicts.Columns("VersionNumber").Visible = True
        '                             Me.dgvConflicts.Columns("FileSystemEntryVersionConflictType").Visible = True
        '                             Me.dgvConflicts.Columns("FileSystemEntryVersionConflictRequestStatus").Visible = True

        '                             Me.dgvConflicts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        '                             dgvConflicts.RowsDefaultCellStyle.SelectionBackColor = Color.LightGray

        '                             dgvConflicts.MultiSelect = False
        '                             dgvConflicts.ClearSelection()

        '                             dgvConflicts.DefaultCellStyle.SelectionBackColor = dgvConflicts.DefaultCellStyle.BackColor
        '                             dgvConflicts.DefaultCellStyle.SelectionForeColor = dgvConflicts.DefaultCellStyle.ForeColor

        '                             dgvConflicts.ReadOnly = True

        '                             dgvConflicts.AllowUserToResizeColumns = False

        '                             '
        '                             'dgvConflicts.Columns("LatestFileSystemEntryVersionFileSystemEntryPathRelativeToShare").Width = 380

        '                             dgvConflicts.Columns("FileSystemEntryVersionConflictRequestStatus").Width = 150
        '                             dgvConflicts.Columns("FileSystemEntryVersionConflictType").Width = 150
        '                             dgvConflicts.Columns("UserName").Width = 150
        '                             dgvConflicts.Columns("VersionNumber").Width = 100


        '                             dgvConflicts.Columns("FileSystemEntryVersionConflictRequestStatus").HeaderText = "User File Upload Status"
        '                             dgvConflicts.Columns("FileSystemEntryVersionConflictType").HeaderText = "Conflict Type"
        '                             dgvConflicts.Columns("UserName").HeaderText = "User"
        '                             dgvConflicts.Columns("VersionNumber").HeaderText = "Version"

        '                         End If
        '                     End Sub
        'async.Execute()

    End Function

    Private Async Function RequestUserFileAsync() As Task
        Using proxy = New Common.FileClient()
            Dim selrw = dgvConflicts.SelectedRows(0)
            Dim FileSystemEntryVersionConflictId = selrw.Cells("FileSystemEntryVersionConflictId").Value()
            Dim FileSystemEntryId = selrw.Cells("FileSystemEntryId").Value()
            Await proxy.RequestConflictFileUploadAsync(FileSystemEntryId, FileSystemEntryVersionConflictId)
        End Using
    End Function

    Private Async Function ResolveOthersConflictUsingOthersAsync() As Task
        Using proxy = New Common.FileClient()
            Dim selrw = dgvConflicts.SelectedRows(0)
            Dim FileSystemEntryVersionConflictId = selrw.Cells("FileSystemEntryVersionConflictId").Value()
            Dim FileSystemEntryId = selrw.Cells("FileSystemEntryId").Value()
            Await proxy.ResolveOthersConflictUsingOthersAsync(shareId, FileSystemEntryId, FileSystemEntryVersionConflictId)
        End Using
    End Function

    Private Async Function ResolveOthersConflictUsingServerAsync() As Task
        Using proxy = New Common.FileClient()
            Dim selrw = dgvConflicts.SelectedRows(0)
            Dim FileSystemEntryVersionConflictId = selrw.Cells("FileSystemEntryVersionConflictId").Value()
            Dim FileSystemEntryId = selrw.Cells("FileSystemEntryId").Value()
            Await proxy.ResolveOthersConflictUsingServerAsync(shareId, FileSystemEntryId, FileSystemEntryVersionConflictId)
        End Using
    End Function

    Private Async Function SaveFileAs() As Task
        If (dgvConflicts.SelectedRows.Count() > 0) Then

            Dim sCellServerFileSystemEntryName = dgvConflicts.SelectedRows(0).Cells("FileSystemEntryVersionConflictId")
            Dim ServerFileSystemEntryName = New Guid(sCellServerFileSystemEntryName.Value.ToString())

            Dim FileShareId = shareId

            Dim FileSystemEntryRelativePath = file.FileVersion.FileEntryRelativePath

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
    End Function

#End Region

#Region "Old Code"

    'Private Sub btnSaveAs_Click(sender As Object, e As EventArgs) Handles btnSaveAs.Click


    '    If (dgvConflicts.SelectedRows.Count() > 0) Then

    '        Dim sCellServerFileSystemEntryName = dgvConflicts.SelectedRows(0).Cells("FileSystemEntryVersionConflictId")
    '        Dim ServerFileSystemEntryName = New Guid(sCellServerFileSystemEntryName.Value.ToString())

    '        Dim FileShareId = shareId

    '        Dim FileSystemEntryRelativePath = file.FileSystemEntryVersion.FileSystemEntryRelativePath

    '        Dim async = New AsyncProvider()
    '        async.AddMethod("GetSharedAccessSignatureUrl", Function() New Common.SyncClient().GetSharedAccessSignatureUrl(FileShareId, ServerFileSystemEntryName, 0, file.FileSystemEntryId))


    '        async.OnCompletion = Sub(list As IDictionary)
    '                                 Dim result = CType(list("GetSharedAccessSignatureUrl"), ResultInfo(Of Uri))
    '                                 If (ValidateResponse(result)) Then
    '                                     Dim fileUri = result.Data

    '                                     Dim fileNameWithExtension = Path.GetFileName(FileSystemEntryRelativePath)
    '                                     Dim fileExtension = Path.GetExtension(FileSystemEntryRelativePath)

    '                                     SaveFileDialog1.DefaultExt = "*" + fileExtension
    '                                     SaveFileDialog1.Filter = "|*" + fileExtension

    '                                     SaveFileDialog1.FileName = ""

    '                                     If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
    '                                         Dim client As WebClient = New WebClient

    '                                         AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
    '                                         AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted

    '                                         client.DownloadFileAsync(fileUri, SaveFileDialog1.FileName)

    '                                     End If
    '                                 End If
    '                             End Sub

    '        async.Execute()
    '    Else
    '        MessageBoxHelper.ShowInfoMessage("No selection is made, Please select a row")
    '    End If

    'End Sub

    'Private Sub dgvConflicts_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvConflicts.CellContentClick
    '    Dim senderGrid = DirectCast(sender, DataGridView)
    '    If e.RowIndex >= 0 Then
    '        Dim selrw = dgvConflicts.Rows(e.RowIndex)
    '        Dim FileSystemEntryVersionConflictRequestStatusId = selrw.Cells("FileSystemEntryVersionConflictRequestStatusId").Value()
    '        If (FileSystemEntryVersionConflictRequestStatusId IsNot Nothing AndAlso FileSystemEntryVersionConflictRequestStatusId = ConflictUploadStatus.Uploaded) Then
    '            btnSaveAs.Enabled = True
    '            Return
    '        End If
    '    End If
    '    btnSaveAs.Enabled = False
    'End Sub

#End Region


End Class