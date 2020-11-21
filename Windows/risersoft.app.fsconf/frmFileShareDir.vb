Imports risersoft.shared.Extensions
Imports risersoft.app.mxent

    Public Class frmFileShareDir
        Inherits frmMax
        Dim WithEvents ItemCodeSys As New clsCodeSystem
    Friend fMat As frmFileShare

    Private Sub InitForm()

        End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        myWinSQL.AssignCmb(fMat.dsCombo, "PidunitType", "", Me.cmb_PidUnitType1)
        myWinSQL.AssignCmb(fMat.dsCombo, "PidunitType", "", Me.cmb_PidUnitType2)
        myWinSQL.AssignCmb(fMat.dsCombo, "DirType", "", Me.cmb_DirType)
        Me.cmb_AllowWOTPIDU.ValueList = fMat.Model.ValueLists("AllowWOTPIDU").ComboList
        If Me.GetSoftData(oView, prepMode, prepIDX) Then
            PrepForm = True
        End If
    End Function

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click, btnCancel.Click
        Me.InitError()
        If Me.CanSave Then
            cm.EndCurrentEdit()
        End If
        myRow("PidUnitType1") = Me.cmb_PidUnitType1.Text
        myRow("PidUnitType2") = Me.cmb_PidUnitType2.Text
        Me.SaveSoftData()
    End Sub

End Class
