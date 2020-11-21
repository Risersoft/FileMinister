Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabControl
Imports risersoft.shared.dotnetfx
Imports System.Security
Public Class frmSearchDWG
    Inherits frmMax
    Dim fp As frmPreview, rStore As DataRow

    Public Sub initForm()
        Dim sql As String, r As DataRow, i As Integer, dt As DataTable

        myView.SetGrid(Me.UltraGrid1)
        WinFormUtils.SetButtonConf(, , Me.btnClose)

        fp = New frmPreview
        fp.AddToPanel(Me.SplitContainer1.Panel2, True, DockStyle.Fill)


        sql = "select filestoreid, StoreCode, IndexServerName, IndexServerCatalog, IndexServerUsername, IndexServerPassword,localpath,uncpath from filestore where len(indexservername)>0 order by storecode"
        myWinSQL.AssignCmb(Me.dsCombo, "store", sql, Me.cmb_filestoreid)
        If Me.dsCombo.Tables("store").Rows.Count > 0 Then Me.cmb_filestoreid.Value = Me.dsCombo.Tables("store").Rows(0)("filestoreid")

        Me.cmb_subject.Items.Clear()
        Me.cmb_subject.Items.Add("BSG", "Bushing")
        Me.cmb_subject.Items.Add("CHL", "Channel")
        Me.cmb_subject.Items.Add("CL", "Clamp")
        Me.cmb_subject.Items.Add("Coil", "Coil Assy")
        Me.cmb_subject.Items.Add("CPack", "Core Packing")
        Me.cmb_subject.Items.Add("CoreAss", "Core Assy")
        Me.cmb_subject.Items.Add("CBox", "Cable Box")
        Me.cmb_subject.Items.Add("DisCh", "Disconnecting Chamber / Bus Trunking")
        Me.cmb_subject.Items.Add("fplan", "Foundation Plan")
        Me.cmb_subject.Items.Add("ga", "General Arrangement")
        Me.cmb_subject.Items.Add("header", "Header")
        Me.cmb_subject.Items.Add("hvlead", "HV Connection")
        Me.cmb_subject.Items.Add("ins", "Insulation")
        Me.cmb_subject.Items.Add("lock", "Locking")
        Me.cmb_subject.Items.Add("lvlead", "LV Connection")
        Me.cmb_subject.Items.Add("misc", "Miscallenous")
        Me.cmb_subject.Items.Add("mbox", "Marshalling Box")
        Me.cmb_subject.Items.Add("npock", "Neutral Turret")
        Me.cmb_subject.Items.Add("oltc", "OLTC Control")
        Me.cmb_subject.Items.Add("pack", "Packing")
        Me.cmb_subject.Items.Add("rdp", "Rating & Diagram Plate")
        Me.cmb_subject.Items.Add("rtcc", "RTCC")
        Me.cmb_subject.Items.Add("tankd", "Tank Details")
        Me.cmb_subject.Items.Add("tapsw", "Tap Switch")
        Me.cmb_subject.Items.Add("trod", "Tie Rod / Core Stud")
        Me.cmb_subject.Items.Add("vdiag", "Valve Diagram Plate")


        Me.dt_ModAfter.Value = Nothing

    End Sub
    Public Function QueryIDX(servername As String, catalogname As String)
        Dim sql, str1 As String, dt As New DataTable, str2 As String = "", str3 As String = ""


        ' execute the data query against the remote indexing server and obtain 
        ' the data result 
        If myUtils.cStrTN(Me.cmb_subject.Value).Trim.Length > 0 Then str1 = "docsubject like '%" & Me.cmb_subject.Value & "%'" Else str1 = ""
        If myUtils.NullNot(Me.dt_ModAfter.Value) Then str2 = "" Else str2 = "write >= '" & Format(Me.dt_ModAfter.Value, "yy-MM-dd") & "'"
        If myUtils.cStrTN(Me.txt_FileName.Value).Trim.Length > 0 Then str3 = "filename like '%" & Me.txt_FileName.Text & "%'" Else str3 = ""
        'str3= " filename like '%xps'"
        str1 = myUtils.CombineWhere(False, str1, str2, str3)
        sql = "select FileName, Doctitle, Path,Write,DocAuthor FROM scope() where " & str1

        dt = myIndexSR.PopulateIndexTable(Me.Controller, sql, servername, catalogname)

        Return dt

    End Function
    Private Function QueryIDX() As DataTable
        Return Me.QueryIDX(rStore("indexservername"), rStore("indexservercatalog"))
    End Function
    Public Function PopulateDT(ByVal dt As DataTable) As DataTable

        Dim sql, str1 As String, str2 As String = "", str3 As String = ""
        Try
            dt.Columns.Add("Ext", GetType(String))
            For Each r1 As DataRow In dt.Select
                Try
                    str1 = r1("filename")
                    str2 = r1("path")
                    str3 = System.IO.Path.GetExtension(str1).Trim.ToLower
                    r1("ext") = str3
                Catch ex As Exception
                    Debug.WriteLine(ex.Message)
                End Try
            Next
            myView.mainGrid.MainConf("autorowht") = True
            myUtils.DeleteRows(dt.Select("ext in ('.bak','.db','.log','.tmp')"))
            myView.mainGrid.MainConf("sortcolsasc") = "write desc,docauthor"
            myView.mainGrid.MainConf("hidecols") = "backup"
            myView.mainGrid.BindView(dt.DataSet)
            myView.mainGrid.QuickConf("", True, "1-0-2-1-0-1", True, "Matching Files")

        Catch ex As Exception

            ' show any connection/querying error 
            MessageBox.Show(Me, ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
        Return dt
    End Function

    Private Sub btnGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGo.Click
        If Me.cmb_filestoreid.SelectedRow Is Nothing Then
            MsgBox("Select a file store", MsgBoxStyle.Information, myWinApp.Vars("appname"))
        Else
            rStore = myWinUtils.DataRowFromGridRow(Me.cmb_filestoreid.SelectedRow)
            Dim dt1 As DataTable = Me.QueryIDX()
            Me.PopulateDT(dt1)
        End If

    End Sub

    Private Sub UltraGrid1_AfterRowActivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles UltraGrid1.AfterRowActivate
        Dim str1, str3 As String

        fp.UnloadPreviews()
        Dim gRow As UltraGridRow = Me.UltraGrid1.ActiveRow
        str1 = ""
        If (Not gRow Is Nothing) Then
            str1 = myUtils.cStrTN(gRow.Cells("path").Value)
            If myUtils.cStrTN(rStore("localpath")).Trim.Length > 0 Then
                str1 = Replace(str1, rStore("localpath"), rStore("uncpath"))
            End If
        End If

        If str1.Trim.Length > 0 Then fp.HandleSelectionPreview(str1)
    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Me.Controller.PrintingPress.GenerateAndShowReport(myView, EnumPrintWhat.acMMR, "<MMR SERIAL=""TRUE""><GROUP FIELD=""DocAuthor"" TYPE=""2""/></MMR>", False)
    End Sub
End Class
