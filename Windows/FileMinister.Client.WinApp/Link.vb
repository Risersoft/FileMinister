Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud
Imports risersoft.shared.portable.Enums

Public Class Link

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

    Private Sub Link_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Bind()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (dgvFiles.SelectedRows.Count() > 0) Then
            Using proxy = New Common.FileClient()
                Dim sCell = dgvFiles.SelectedRows(0).Cells("FileSystemEntryId")
                Dim linkedFileId = New Guid(sCell.Value.ToString())

                proxy.AddFileLink(fileId, linkedFileId)

                Me.Close()
            End Using
        Else
            MessageBoxHelper.ShowInfoMessage("No selection is made, Please select a row")
        End If
    End Sub


    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub Bind()
        Using proxy = New Common.FileClient()

            Me.dgvFiles.DataSource = Nothing
            Me.dgvFiles.ClearSelection()
            Me.dgvFiles.Refresh()
            Me.dgvFiles.Rows.Clear()
            Me.dgvFiles.Columns.Clear()


            Dim async = New AsyncProvider()
            async.AddMethod("FileLinkInfo", Function() New Common.FileClient().GetAllFilesForLinking(fileId, shareId))
            async.OnCompletion = Sub(list As IDictionary)
                                     Dim result = CType(list("FileLinkInfo"), ResultInfo(Of List(Of FileEntryLinkInfo), Status))
                                     If (ValidateResponse(result)) Then

                                         Me.dgvFiles.DataSource = result.Data

                                         For Each col In dgvFiles.Columns
                                             col.Visible = False
                                         Next

                                         Me.dgvFiles.Columns("LatestFileSystemEntryVersionFileSystemEntryPathRelativeToShare").Visible = True

                                         Me.dgvFiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                                         dgvFiles.RowsDefaultCellStyle.SelectionBackColor = Color.LightGray

                                         dgvFiles.MultiSelect = False
                                         dgvFiles.ClearSelection()

                                         dgvFiles.DefaultCellStyle.SelectionBackColor = dgvFiles.DefaultCellStyle.BackColor
                                         dgvFiles.DefaultCellStyle.SelectionForeColor = dgvFiles.DefaultCellStyle.ForeColor

                                         dgvFiles.ReadOnly = True

                                         dgvFiles.AllowUserToResizeColumns = False

                                         dgvFiles.Columns("FileSystemEntryId").Width = 10
                                         dgvFiles.Columns("LatestFileSystemEntryVersionFileSystemEntryPathRelativeToShare").Width = 380
                                         dgvFiles.Columns("LatestFileSystemEntryVersionFileSystemEntryPathRelativeToShare").HeaderText = "Path"
                                     End If
                                 End Sub
            async.Execute()

        End Using
    End Sub

#End Region

End Class