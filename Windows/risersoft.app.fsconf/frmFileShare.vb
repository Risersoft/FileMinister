Imports Infragistics.Win.UltraWinGrid
Imports System.Windows.Forms
Imports risersoft.app.mxform.eto
Imports risersoft.shared.Extensions
Imports risersoft.shared.agent

Public Class frmFileShare
    Inherits frmMax
    Dim myVueAgent As New clsWinView
    Dim myVueLocal As New clsWinView

    Public Sub initForm()
        myView.SetGrid(Me.UltraGridDir)
        myVueAgent.SetGrid(Me.UltraGridAgent)
        myVueLocal.SetGrid(Me.UltraGrid1)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Me.FormPrepared = False
        Dim objModel As frmFileShareModel = Me.InitData("frmFileShareModel", oView, prepMode, prepIdx, strXML)
        If Me.BindModel(objModel, oView) Then

            Me.cmb_IsBackup.ValueList = Me.Model.ValueLists("IsBackup").ComboList
            Me.cmb_MaintainVersionHistory.ValueList = Me.Model.ValueLists("MaintainVersionHistory").ComboList
            myWinSQL.AssignCmb(Me.dsCombo, "ShareType", "", Me.cmb_ShareType)
            myWinSQL.AssignCmb(Me.dsCombo, "BackUp", "", Me.cmb_BackupShareID)
            myWinSQL.AssignCmb(Me.dsCombo, "FullBackupPeriodType", "", Me.cmb_FullBackupPeriodType)
            myWinSQL.AssignCmb(Me.dsCombo, "IncrBackupPeriodType", "", Me.cmb_IncrBackupPeriodType)

            If myUtils.cBoolTN(myRow("IsBackup")) Then
                Me.UltraTabControl5.Tabs("Backup").Visible = False
            End If

            If (frmMode = EnumfrmMode.acEditM) Then
                Me.txt_ShareName.ReadOnly = True
                Me.txt_ShareContainerName.ReadOnly = True
                Me.cmb_ShareType.ReadOnly = True
            End If

            Me.FormPrepared = True
        End If
        Return Me.FormPrepared
    End Function

    Public Overrides Function BindModel(NewModel As clsFormDataModel, oView As clsView) As Boolean
        If MyBase.BindModel(NewModel, oView) Then
            myView.PrepEdit(Me.Model.GridViews("Dir"),, Me.btnDelDir)
            myVueAgent.PrepEdit(Me.Model.GridViews("Agent"))
            myVueLocal.PrepEdit(Me.Model.GridViews("local"))
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

    Private Sub btnAddAgent_Click(sender As Object, e As EventArgs) Handles btnAddAgent.Click
        Dim Params As New List(Of clsSQLParam), r1, r2, r3, rr() As DataRow
        Params.Add(New clsSQLParam("@FileShareID", frmIDX, GetType(Integer), False))
        Dim str1 As String = myUtils.MakeCSV(myVueAgent.mainGrid.myDv.Table.Select("isDeleted = 0"), "FileAgentId", ",", "'00000000-0000-0000-0000-000000000000'", True)
        Params.Add(New clsSQLParam("@agentidcsv", str1, GetType(Guid), True))
        rr = Me.AdvancedSelect("agent", Params)
        If (Not rr Is Nothing) AndAlso rr.Length > 0 Then
            For Each r1 In rr
                If r1("isdeleted") = True Then
                    r1("isdeleted") = 0
                    r1("deletedonutc") = DBNull.Value
                    r1("deletedbyuserid") = DBNull.Value
                    r3 = myUtils.CopyOneRow(r1, myVueAgent.mainGrid.myDS.Tables(0))
                Else
                    r2 = myUtils.CopyOneRow(r1, myVueAgent.mainGrid.myDS.Tables(0))
                End If
            Next
        End If
    End Sub

    Private Sub btnDelAgent_Click(sender As Object, e As EventArgs) Handles btnDelAgent.Click
        If MsgBox("Are you sure you want to delete this Agent", MsgBoxStyle.YesNo, myWinApp.Vars("appname")) = MsgBoxResult.Yes Then
            Dim rr As DataRow() = New DataRow() {myWinUtils.DataRowFromGridRow(myVueAgent.mainGrid.myGrid.ActiveRow)}
            rr(0)("isDeleted") = 1
            rr(0)("deletedonutc") = Now.Date
            rr(0)("deletedbyuserid") = Me.Controller.Police.UniqueUserID
        End If
    End Sub

    Private Sub btnAddDir_Click(sender As Object, e As EventArgs) Handles btnAddDir.Click
        If Me.frmMode = EnumfrmMode.acEditM Then
            Dim f As New frmFileShareDir
            f.fMat = Me
            If f.PrepForm(myView, EnumfrmMode.acAddM, "", "") Then f.ShowDialog()
        Else
            MsgBox("You need to save first before proceeding ..", MsgBoxStyle.Information, myWinApp.Vars("AppName"))
        End If
    End Sub

    Private Sub btnEditDir_Click(sender As Object, e As EventArgs) Handles btnEditDir.Click
        Dim f As New frmFileShareDir
        f.fMat = Me
        myView.ContextRow = myView.mainGrid.ActiveRow
        If Not myView.ContextRow Is Nothing Then
            If f.PrepForm(Me.myView, EnumfrmMode.acEditM, "") Then f.ShowDialog()
        End If
    End Sub

    Private Sub cmb_IsBackup_Leave(sender As Object, e As EventArgs) Handles cmb_IsBackup.Leave, cmb_IsBackup.AfterCloseUp
        If cmb_IsBackup.Value = True Then
            Me.UltraTabControl5.Tabs("Backup").Visible = False
        Else
            Me.UltraTabControl5.Tabs("Backup").Visible = True
        End If
    End Sub

    Private Sub btnAddWS_Click(sender As Object, e As EventArgs) Handles btnAddWS.Click
        If Me.frmMode = EnumfrmMode.acEditM Then
            Dim f As New frmWorkSpace
            If f.PrepForm(myView, EnumfrmMode.acAddM, "", "") Then f.ShowDialog()
        Else
            MsgBox("You need to save first before proceeding ..", MsgBoxStyle.Information, myWinApp.Vars("AppName"))
        End If
    End Sub

    Private Sub btnEditWS_Click(sender As Object, e As EventArgs) Handles btnEditWS.Click
        myVueLocal.ContextRow = myVueLocal.mainGrid.ActiveRow
        If Not myVueLocal.ContextRow Is Nothing Then
            Dim f As New frmWorkSpace
            If f.PrepForm(myView, EnumfrmMode.acEditM, myVueLocal.ContextRow.CellValue("workspaceid").ToString, "") Then f.ShowDialog()
        Else
            MsgBox("You need to save first before proceeding ..", MsgBoxStyle.Information, myWinApp.Vars("AppName"))
        End If
    End Sub

    Private Async Sub btnRunFull_Click(sender As Object, e As EventArgs) Handles btnRunFull.Click
        If Me.VSave Then
            Dim Params As New List(Of clsSQLParam)
            Params.Add(New clsSQLParam("@ActionType", "'BF'", GetType(String), False))
            Dim ds As DataSet = Me.Controller.DataProvider.LoadAppData(Me.oApp.Info, "dbschedtask", Params, False).Result.Data
            Dim rr() = ds.Tables(0).Select("backuptype='f'")
            If rr.Length > 0 Then
                Dim Params2 As New List(Of clsSQLParam)
                Params2.Add(New clsSQLParam("@Fileshareid", myRow("fileshareid"), GetType(Integer), False))

                Dim scheduler = New clsTaskScheduler(oApp, True)
                Dim oRet2 = Await scheduler.ExecuteLocalTask(rr(0)("dbschedtaskid"), Params2)

                MsgBox(oRet2.Message)
            End If

        End If
    End Sub
End Class
