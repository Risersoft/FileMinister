Imports risersoft.app.mxent
Imports risersoft.app.mxform
Imports risersoft.app.mxform.eto
Imports risersoft.shared.Extensions

Public Class frmFileAgent
    Inherits frmMax
    Dim myVueWork As New clsWinView

    Public Sub initForm()
        myView.SetGrid(Me.UltraGridShare)
        myVueWork.SetGrid(Me.UltraGridWork)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Me.FormPrepared = False
        Dim objModel As frmFileAgentModel = Me.InitData("frmFileAgentModel", oview, prepMode, prepIdx, strXML)
        If Me.BindModel(objModel, oview) Then
            Me.FormPrepared = True
        End If
        Return Me.FormPrepared
    End Function

    Public Overrides Function BindModel(NewModel As clsFormDataModel, oView As clsView) As Boolean
        If MyBase.BindModel(NewModel, oView) Then
            myView.PrepEdit(Me.Model.GridViews("Share"))
            myVueWork.PrepEdit(Me.Model.GridViews("Work"))
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

    Private Sub btnAddShare_Click(sender As Object, e As EventArgs) Handles btnAddShare.Click
        Dim Params As New List(Of clsSQLParam), r1, r2, r3, rr() As DataRow
        Params.Add(New clsSQLParam("@FileAgentID", frmIDX, GetType(Guid), True))
        Params.Add(New clsSQLParam("@shareidcsv", myUtils.MakeCSV(myView.mainGrid.myDv.Table.Select("isDeleted = 0"), "FileShareID"), GetType(Integer), True))
        rr = Me.AdvancedSelect("share", Params)
        If (Not rr Is Nothing) AndAlso rr.Length > 0 Then
            For Each r1 In rr
                If r1("isdeleted") = True Then
                    r1("isdeleted") = 0
                    r1("deletedonutc") = DBNull.Value
                    r1("deletedbyuserid") = DBNull.Value
                    r3 = myUtils.CopyOneRow(r1, myView.mainGrid.myDS.Tables(0))
                Else
                    r2 = myUtils.CopyOneRow(r1, myView.mainGrid.myDS.Tables(0))
                End If
            Next
        End If
    End Sub

    Private Sub btnDelShare_Click(sender As Object, e As EventArgs) Handles btnDelShare.Click
        If MsgBox("Are you sure you want to delete this Share", MsgBoxStyle.YesNo, myWinApp.Vars("appname")) = MsgBoxResult.Yes Then
            Dim rr As DataRow() = New DataRow() {myWinUtils.DataRowFromGridRow(myView.mainGrid.myGrid.ActiveRow)}
            rr(0)("isDeleted") = 1
            rr(0)("deletedonutc") = Now.Date
            rr(0)("deletedbyuserid") = Me.Controller.Police.UniqueUserID
        End If
    End Sub

    Private Sub btnAddWS_Click(sender As Object, e As EventArgs) Handles btnAddWS.Click
        If Me.frmMode = EnumfrmMode.acEditM Then
            Dim f As New frmWorkSpace
            If f.PrepForm(myVueWork, EnumfrmMode.acAddM, "", "<PARAMS FileAgentID=""" & frmIDX & """/>") Then
                If f.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    Dim Params As New List(Of clsSQLParam)
                    Params.Add(New clsSQLParam("@FileAgentID", frmIDX, GetType(Guid), True))
                    Dim oRet As clsProcOutput = Me.GenerateParamsOutput("workspace", Params)
                    If oRet.Success Then
                        Me.UpdateViewData(myVueWork, oRet)
                    Else
                        MsgBox(oRet.Message, MsgBoxStyle.Information, myWinApp.Vars("appname"))
                    End If
                End If
            End If
        Else
            MsgBox("You need to save first before proceeding ..", MsgBoxStyle.Information, myWinApp.Vars("AppName"))
        End If
    End Sub

    Private Sub btnEditWS_Click(sender As Object, e As EventArgs) Handles btnEditWS.Click
        If Not Me.myVueWork.mainGrid.myGrid.ActiveRow Is Nothing Then
            Dim frm As New frmWorkSpace
            If frm.PrepForm(Me.myVueWork, EnumfrmMode.acEditM, Me.myVueWork.mainGrid.myGrid.ActiveRow.Cells("WorkSpaceID").Value.ToString) Then
                If frm.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    Dim Params As New List(Of clsSQLParam)
                    Params.Add(New clsSQLParam("@FileAgentID", frmIDX, GetType(Guid), True))
                    Dim oRet As clsProcOutput = Me.GenerateParamsOutput("workspace", Params)
                    If oRet.Success Then
                        Me.UpdateViewData(myVueWork, oRet)
                    Else
                        MsgBox(oRet.Message, MsgBoxStyle.Information, myWinApp.Vars("appname"))
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub btnDelWS_Click(sender As Object, e As EventArgs) Handles btnDelWS.Click
        If MsgBox("Are you sure you want to delete this WorkSpace", MsgBoxStyle.YesNo, myWinApp.Vars("appname")) = MsgBoxResult.Yes Then
            Dim rr As DataRow() = New DataRow() {myWinUtils.DataRowFromGridRow(myVueWork.mainGrid.myGrid.ActiveRow)}
            rr(0)("isDeleted") = 1
            rr(0)("deletedonutc") = Now.Date
            rr(0)("deletedbyuserid") = Me.Controller.Police.UniqueUserID
        End If
    End Sub
End Class
