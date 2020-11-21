Public Class MessageBoxHelper

    Public Shared Function ShowInfoMessage(message As String, Optional title As String = "Cloud Sync") As DialogResult
        Dim result = MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Return result
    End Function

    Public Shared Function ShowWarningMessage(message As String, Optional title As String = "Cloud Sync") As DialogResult
        Dim result = MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Return result
    End Function

    Public Shared Function ShowErrorMessage(message As String, Optional title As String = "Cloud Sync") As DialogResult
        Dim result = MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return result
    End Function

    Public Shared Function ShowQuestion(message As String, Optional title As String = "Cloud Sync") As DialogResult
        Dim result = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
        Return result
    End Function

End Class
