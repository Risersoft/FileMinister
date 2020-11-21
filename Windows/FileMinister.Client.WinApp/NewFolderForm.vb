Imports risersoft.shared.portable.Model
Imports risersoft.shared.cloud

Public Class NewFolderForm

    Dim share As ConfigInfo
    Public folderPath As String
    Public Result As Boolean

    Public Sub New(share As ConfigInfo, folderPath As String)
        InitializeComponent()
        Me.share = share
        Me.folderPath = folderPath
    End Sub

    Private Sub NewFolderForm_Load(sender As Object, e As EventArgs)
        txtfolderName.Focus()
    End Sub

    Private Sub btnNewFolderCreate_Click(sender As Object, e As EventArgs) Handles btnNewFolderCreate.Click

        If share Is Nothing Then
            MessageBoxHelper.ShowErrorMessage("Share not found")
            Return
        End If

        Result = False
        If String.IsNullOrWhiteSpace(txtfolderName.Text) Then
            ErrorProvider1.SetError(txtfolderName, "Name cannot be blank")
            txtfolderName.Focus()
            Return
        End If

        Using CommonUtils.Helper.Impersonate(share)
            Dim path = System.IO.Path.Combine(folderPath, txtfolderName.Text)
            Dim folderName = txtfolderName.Text.Trim()
            If System.IO.Directory.Exists(path) Then
                MessageBoxHelper.ShowErrorMessage(String.Format("Folder {0} already exists", folderName))
                txtfolderName.Focus()
                Return
            End If

            Try
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(folderPath, txtfolderName.Text))
                Result = True
                Me.Close()
            Catch ex As Exception
                MessageBoxHelper.ShowErrorMessage(ex.Message)
                txtfolderName.Focus()
            End Try
        End Using
    End Sub
End Class