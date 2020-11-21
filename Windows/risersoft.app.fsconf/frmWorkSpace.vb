Imports risersoft.app.mxent
Imports risersoft.app.mxform
Imports risersoft.app.mxform.eto

Public Class frmWorkSpace
    Inherits frmMax

    Public Sub initForm()
        myView.SetGrid(Me.UltraGridLocal)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Me.FormPrepared = False
        Dim objModel As frmWorkSpaceModel = Me.InitData("frmWorkSpaceModel", oview, prepMode, prepIdx, strXML)
        If Me.BindModel(objModel, oview) Then
            Me.FormPrepared = True
        End If
        Return Me.FormPrepared
    End Function

    Public Overrides Function BindModel(NewModel As clsFormDataModel, oView As clsView) As Boolean
        If MyBase.BindModel(NewModel, oView) Then
            myView.PrepEdit(Me.Model.GridViews("Local"),, Me.btnDel)
            myWinSQL.AssignCmb(Me.dsCombo, "User", "", Me.cmb_UserDomainID)
            Return True
        End If
        Return False
    End Function

    Public Overrides Function VSave() As Boolean
        Me.InitError()
        VSave = False
        cm.EndCurrentEdit()
        If Me.ValidateData() Then
            If Me.SaveModel() Then
                Return True
            End If
        Else
            Me.SetError()
        End If
        Me.Refresh()
    End Function

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If Me.frmMode = EnumfrmMode.acEditM Then
            Dim f As New frmLocalShare
            f.fMat = Me
            If f.PrepForm(myView, EnumfrmMode.acAddM, "", "") Then f.ShowDialog()
        Else
            MsgBox("You need to save first before proceeding ..", MsgBoxStyle.Information, myWinApp.Vars("AppName"))
        End If
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Dim f As New frmLocalShare
        f.fMat = Me
        myView.ContextRow = Nothing
        If f.PrepForm(Me.myView, EnumfrmMode.acEditM, "") Then f.ShowDialog()
    End Sub
End Class


