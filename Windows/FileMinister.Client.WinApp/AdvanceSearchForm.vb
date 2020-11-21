Public Class AdvanceSearchForm

    Public isSearchCanceled As Boolean = True

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub AdvanceSearchForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BindMetadata()
    End Sub

    Private Sub BindMetadata()

        'Set AutoGenerateColumns False
        dgvMetadata.AutoGenerateColumns = False
        'Set Columns Count
        dgvMetadata.ColumnCount = 2
        'Add Columns
        dgvMetadata.Columns(0).Name = "Key"
        dgvMetadata.Columns(0).HeaderText = "Key"
        dgvMetadata.Columns(0).DataPropertyName = "TagName"
        dgvMetadata.Columns(1).Name = "Value"
        dgvMetadata.Columns(1).HeaderText = "Value"
        dgvMetadata.Columns(1).DataPropertyName = "TagValue"

    End Sub



    Private Sub BtnOk_Click(sender As Object, e As EventArgs) Handles BtnOk.Click
        Me.isSearchCanceled = False
        Me.Close()
    End Sub

End Class