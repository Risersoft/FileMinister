Imports risersoft.app.mxform.eto
Imports risersoft.shared.Extensions
Public Class frmDocDistri
    Inherits frmMax
    Private Sub InitForm()
        myView.SetGrid(Me.UGridDep)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Me.FormPrepared = False
        Dim objModel As frmDocDistriModel = Me.InitData("frmDocDistriModel", oview, prepMode, prepIdx, strXML)
        If Me.BindModel(objModel, oview) Then
            Me.FormPrepared = True
        End If
        Return Me.FormPrepared
    End Function

    Public Overrides Function BindModel(NewModel As clsFormDataModel, oView As clsView) As Boolean
        If MyBase.BindModel(NewModel, oView) Then
            Me.myView.PrepEdit(Me.Model.GridViews("Doc"))
            Me.myView.mainGrid.HighlightRows()

            myWinSQL.AssignCmb(Me.dsCombo, "deps", "", Me.cmb_MatDepID)
            Me.cmb_MfgTypeCode.ValueList = Me.Model.ValueLists("MfgTypeCode").ComboList
            Me.cmb_DocTypeCode.ValueList = Me.Model.ValueLists("DocTypeCode").ComboList
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
End Class
