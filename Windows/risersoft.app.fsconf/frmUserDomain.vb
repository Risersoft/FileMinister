Imports risersoft.app.mxform.eto
Imports Infragistics.Win.UltraWinGrid
Imports risersoft.shared.Extensions
Imports risersoft.app.shared
Imports risersoft.app.mxform
Imports risersoft.shared.cloud

Public Class frmUserDomain
    Inherits frmMax
    Dim myVueUser As New clsWinView

    Private Sub InitForm()
        myView.SetGrid(Me.UltraGridGroupMap)
        myVueUser.SetGrid(Me.UltraGridUserMap)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Me.FormPrepared = False
        Dim objModel As frmUserDomainModel = Me.InitData("frmUserDomainModel", oview, prepMode, prepIdx, strXML)
        If Me.BindModel(objModel, oview) Then
            Me.FormPrepared = True
        End If
        Return Me.FormPrepared
    End Function

    Public Overrides Function BindModel(NewModel As clsFormDataModel, oView As clsView) As Boolean
        If MyBase.BindModel(NewModel, oView) Then
            myView.PrepEdit(Me.Model.GridViews("Group"))
            myVueUser.PrepEdit(Me.Model.GridViews("User"))
            Return True
        End If
        Return False
    End Function

    Public Overrides Function VSave() As Boolean
        Me.InitError()
        VSave = False
        cm.EndCurrentEdit()
        For Each gr As UltraGridRow In myView.mainGrid.myGrid.Rows
            Dim r1 As DataRow = myWinUtils.DataRowFromGridRow(gr)
            If Not myUtils.cBoolTN(r1("isdeleted")) Then
                Dim LoginName As String = If(myUtils.cStrTN(r1("loginname")).Trim.Length = 0, gr.Cells("groupid").Text, r1("loginname"))
                Dim sid = WinAdHelper.getSidByGroupName(myRow("domainname"), LoginName)
                If sid IsNot Nothing Then r1("sid") = sid.Value
            End If
        Next
        For Each gr As UltraGridRow In myVueUser.mainGrid.myGrid.Rows
            Dim r1 As DataRow = myWinUtils.DataRowFromGridRow(gr)
            If Not myUtils.cBoolTN(r1("isdeleted")) Then
                Dim LoginName As String = If(myUtils.cStrTN(r1("loginname")).Trim.Length = 0, gr.Cells("userid").Text, r1("loginname"))
                Dim sid = WinAdHelper.getSidByUserName(myRow("domainname"), LoginName)
                If sid IsNot Nothing Then r1("sid") = sid.Value

            End If
        Next

        If Me.ValidateData() Then
            If Me.SaveModel() Then
                Return True
            End If
        Else
            Me.SetError()
        End If
        Me.Refresh()
    End Function

    Private Sub btnDelUser_Click(sender As Object, e As EventArgs) Handles btnDelUser.Click
        If MsgBox("Are you sure you want to delete this User", MsgBoxStyle.YesNo, myWinApp.Vars("appname")) = MsgBoxResult.Yes Then
            Dim rr As DataRow() = New DataRow() {myWinUtils.DataRowFromGridRow(myVueUser.mainGrid.myGrid.ActiveRow)}
            rr(0)("isDeleted") = 1
            rr(0)("deletedonutc") = Now.Date
            rr(0)("deletedbyuserid") = Me.Controller.Police.UniqueUserID
        End If
    End Sub

    Private Sub btnDelGroup_Click(sender As Object, e As EventArgs) Handles btnDelGroup.Click
        If MsgBox("Are you sure you want to delete this Group", MsgBoxStyle.YesNo, myWinApp.Vars("appname")) = MsgBoxResult.Yes Then
            Dim rr As DataRow() = New DataRow() {myWinUtils.DataRowFromGridRow(myView.mainGrid.myGrid.ActiveRow)}
            rr(0)("isDeleted") = 1
            rr(0)("deletedonutc") = Now.Date
            rr(0)("deletedbyuserid") = Me.Controller.Police.UniqueUserID
        End If
    End Sub

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs) Handles btnAddUser.Click
        Dim r1, r2, r3, rr() As DataRow, Params As New List(Of clsSQLParam)
        Params.Add(New clsSQLParam("@UserMappingID", frmIDX, GetType(Integer), False))
        Dim str1 As String = myUtils.MakeCSV(myVueUser.mainGrid.myDv.Table.Select("isDeleted = 0"), "UserID", ", ", "'00000000-0000-0000-0000-000000000000'", True)
                Params.Add(New clsSQLParam("@userIDCSV", str1, GetType(Guid), True))
        rr = Me.AdvancedSelect("users", Params)
        If (Not rr Is Nothing) AndAlso rr.Length > 0 Then
            For Each r1 In rr
                If r1("isdeleted") = True Then
                    r1("isdeleted") = 0
                    r1("deletedonutc") = DBNull.Value
                    r1("deletedbyuserid") = DBNull.Value
                    r3 = myUtils.CopyOneRow(r1, myVueUser.mainGrid.myDS.Tables(0))
                Else
                    r2 = myUtils.CopyOneRow(r1, myVueUser.mainGrid.myDS.Tables(0))
                End If
            Next
        End If
    End Sub

    Private Sub btnAddGroup_Click(sender As Object, e As EventArgs) Handles btnAddGroup.Click
        Dim r1, r2, r3, rr() As DataRow, Params As New List(Of clsSQLParam)
        Params.Add(New clsSQLParam("@UserMappingID", frmIDX, GetType(Integer), False))
        Dim str1 As String = myUtils.MakeCSV(myView.mainGrid.myDv.Table.Select("isDeleted = 0"), "GroupID", ",", "'00000000-0000-0000-0000-000000000000'", True)
        Params.Add(New clsSQLParam("@grpIDCSV", str1, GetType(Guid), True))
        rr = Me.AdvancedSelect("groups", Params)
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

    Private Sub frmUserDomain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.UltraTabControl1.Height = (Me.Height - Me.UltraPanel3.Height - Me.Panel4.Height) / 2
    End Sub
End Class

