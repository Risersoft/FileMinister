Imports risersoft.shared.Extensions
Imports risersoft.app.mxent

Public Class frmLocalShare
    Inherits frmMax
    Dim WithEvents ItemCodeSys As New clsCodeSystem
    Friend fMat As frmWorkSpace

    Private Sub InitForm()
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        myWinSQL.AssignCmb(fMat.dsCombo, "Share", "", Me.cmb_FileShareID)
        If Me.GetSoftData(oView, prepMode, prepIDX) Then
            PrepForm = True
        End If
    End Function

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click, btnCancel.Click
        Me.InitError()
        If myUtils.NullNot(Me.cmb_FileShareID.Value) Then WinFormUtils.AddError(Me.cmb_FileShareID, "Enter a File Share")
        If Me.CanSave Then
            cm.EndCurrentEdit()
            myRow("CreatedOnUTC") = Now.Date
            Me.SaveSoftData()
        End If
    End Sub

End Class
