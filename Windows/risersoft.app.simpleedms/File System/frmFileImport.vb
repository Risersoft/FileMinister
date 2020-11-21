Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabControl
Imports IWshRuntimeLibrary
Imports risersoft.shared.dotnetfx

Public Class frmFileImport
    Inherits frmMax
    Public myVueProDocu As New clsWinView, myVueAll As New clsWinView
    Public dtStd, dtFile, dtDocu As DataTable
    Dim arrOld As New ArrayList, serverPathFile As String = "\\dse1\file"
    Dim objPIDU As clsPIDUFolder
    Public Sub initForm()
        Dim sql As String, r As DataRow, i As Integer, dt As DataTable

        myView.SetGrid(Me.UltraGrid1)
        myVueProDocu.SetGrid(Me.UltraGrid2)
        myVueAll.SetGrid(Me.UltraGrid3)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)

        objPIDU = New clsPIDUFolder(Me.Controller)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Dim sql, str1 As String, i As Integer, dt As DataTable, r As DataRow
        Dim dt1 As New DataTable, dt2 As New DataTable, rr() As DataRow, c As DataColumn, sum As Decimal

        dt1.Columns.Add("FileName", GetType(String))
        dt1.Columns.Add("Path", GetType(String))
        dt1.Columns.Add("LastModified", GetType(DateTime))
        dt1.Columns.Add("Sheetnum", GetType(Integer))
        dt1.Columns.Add("stddrgid", GetType(Integer))

        myView.mainGrid.MainConf("showrownumber") = True
        myView.mainGrid.BindView(myUtils.MakeDSfromTable(dt1, False))
        myView.mainGrid.QuickConf("", True, "1-2-1-1-1", True, "Standard Drawings")

        dt2.Columns.Add("FileName", GetType(String))
        dt2.Columns.Add("Path", GetType(String))
        dt2.Columns.Add("LastModified", GetType(DateTime))
        dt2.Columns.Add("FileNum", GetType(String))
        dt2.Columns.Add("DrgNum", GetType(String))
        dt2.Columns.Add("WONUM", GetType(String))
        dt2.Columns.Add("Sheetnum", GetType(Integer))
        dt2.Columns.Add("prodocuid", GetType(Integer))

        myVueProDocu.mainGrid.MainConf("showrownumber") = True
        myVueProDocu.mainGrid.BindView(myUtils.MakeDSfromTable(dt2, False))
        myVueProDocu.mainGrid.QuickConf("", True, "1-2-1-1-1-1-1-1", True, "File Drawings")

        sql = "select pidunitid,wonum,filenum,wodate,pidinfo,username,fileroot, iscompleted,isnull(nodesign,0) as nodesign,prodfiledate, " &
                " dbo.split(wonum,'/',0)  + '-0' + convert(varchar,year(wodate)-2000) as wonum1, " &
                " dbo.split(wonum,'/',0)  + '-' + convert(varchar,year(wodate)) as wonum2, " &
                " (select count(prodocuid) from prodocu where prodocu.pidunitid = deslistpidunit.pidunitid) as cntdoc, " &
                " (select count(pidctlid) from pidctl where pidctl.pidunitid = deslistpidunit.pidunitid) as cntmat " &
                " from deslistpidunittot() where isnull(filenum,'')<>'' and wostatusid <> 12"

        dtStd = SQLHelper.ExecuteDataset(CommandType.Text, "select stddrgid, stdseries, dbo.val(groupnum) as groupnum, dbo.val(drgnum) as drgnum from stddrg").Tables(0)
        'dtFile = SqlHelper.ExecuteDataset( CommandType.Text, "select * from deslistpidunittot() where isnull(filenum,'')<>''").Tables(0)
        dtFile = SQLHelper.ExecuteDataset(CommandType.Text, sql).Tables(0)
        dtDocu = SQLHelper.ExecuteDataset(CommandType.Text, "select * from deslistprodocu()").Tables(0)

        arrOld.Add("\\dse1\file\OLD\K_DRAWING2\DRAWINGS\DRAW8_USER_DRG")
        arrOld.Add("\\dse1\file\OLD\K_DRAWING2\DRAWINGS\OLD_DRG")
        arrOld.Add("\\dse1\file\OLD\T_DRAW4\OLTC\OLD DRG")
        arrOld.Add("\\dse1\file\OLD\K_DRAWING2\DRAWINGS\USER_DRG\RDP-1\OLD-RDP-1")
        PrepForm = True
    End Function

    Public Overrides Function VSave() As Boolean
        Dim i As Integer

        Me.InitError()

        If Me.CanSave Then
            VSave = True
        End If

    End Function

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim lvi As ListViewItem

        If Me.FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            lvi = New ListViewItem(Me.FolderBrowserDialog1.SelectedPath)
            Me.ListView1.Items.Add(lvi)
        End If
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Me.ListView1.Items.Clear()
    End Sub

    Private Sub btnProc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProc.Click
        Me.DoImport()
    End Sub

    Private Sub DoImport()
        Dim lvi As ListViewItem, str1, str2 As String

        myView.mainGrid.myDS.Tables(0).Rows.Clear()
        myVueProDocu.mainGrid.myDS.Tables(0).Rows.Clear()
        Application.DoEvents()
        For Each lvi In Me.ListView1.Items
            str1 = lvi.Text
            If System.IO.Directory.Exists(str1) Then myIndexSR.doImportFolder(Me.Controller, myView.mainGrid.myDS.Tables(0), myVueProDocu.mainGrid.myDS.Tables(0), Me.dtStd, str1)
        Next
    End Sub
    Public Sub AppendText(ByVal txt As String)
        myWinUtils.AppendText(Me.UltraTextEditor1, txt)
    End Sub

    Private Sub btnFill_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFill.Click
        Dim str1 As String = "", sql As String, dt As DataTable

        str1 = myUtils.CombineWhere(False, str1, " filename like '%dwg'")
        myVueAll.mainGrid.MainConf("showrownumber") = True

        sql = "select filename, write,Directory FROM scope() where " & str1
        dt = myIndexSR.PopulateIndexTable(Me.Controller, sql, "dse1", "file")


        dt.Columns("write").ColumnName = "LastModified"
        dt.Columns.Add("FILENUM", GetType(String))
        dt.Columns.Add("WONUM", GetType(String))
        dt.Columns.Add("PIDUNITID", GetType(Integer))
        dt.Columns.Add("STDDRGID", GetType(Integer))
        dt.Columns.Add("NumDup", GetType(Integer))
        dt.Columns.Add("CTRL", GetType(Integer))
        myVueAll.mainGrid.MainConf("defaultwidfact") = 1
        myVueAll.mainGrid.MainConf("allowgroupby") = True
        myVueAll.mainGrid.BindView(dt.DataSet)
        myVueAll.mainGrid.QuickConf("", False, "1-1-1-1", True, "Files")

    End Sub

    Private Sub btnProcAllFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcAllFiles.Click
        Dim rr(), rLast As DataRow, numdup As Integer, arr As New ArrayList, arrolddir() As String

        arrolddir = arrOld.ToArray(GetType(String))
        For Each r1 As DataRow In myVueAll.mainGrid.myDS.Tables(0).Select("", "filename")
            If InStr(myPathUtils.RelativePathTo(serverPathFile & "\old", r1("directory")).Trim, "old ") > 0 Then
                r1("WONUM") = "Old Folder"
            ElseIf myPathUtils.IsInTreeList(r1("directory"), arrolddir) Then
                r1("WONUM") = "Discarded Folder"
            Else
                If (Not rLast Is Nothing) Then
                    If myUtils.cStrTN(rLast("filename")).Trim.ToLower = myUtils.cStrTN(r1("filename")).Trim.ToLower Then
                        numdup = numdup + 1
                        arr.Add(rLast)
                    ElseIf numdup > 0 Then
                        arr.Add(rLast)
                        For Each r2 As DataRow In arr
                            r2("numdup") = numdup
                        Next
                        numdup = 0
                        arr.Clear()
                    End If
                End If
                myIndexSR.ProcessRow(r1, dtStd, dtFile, dtDocu)
                Application.DoEvents()
                rLast = r1
            End If
        Next
    End Sub

    Private Sub btnProcRow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcRow.Click
        Dim r1 As DataRow
        Dim rr(), rLast As DataRow, numdup As Integer, arr As New ArrayList, arrolddir() As String

        arrolddir = arrOld.ToArray(GetType(String))
        r1 = myWinUtils.DataRowFromGridRow(myVueAll.mainGrid.myGrid.ActiveRow)
        If myPathUtils.IsInTreeList(r1("directory"), arrolddir) Then
            r1("WONUM") = "Discarded Folder"
        Else
            myIndexSR.ProcessRow(r1, dtStd, dtFile, dtDocu)
            Application.DoEvents()
        End If
    End Sub

    Private Sub btnMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMove.Click
        Dim dt As DataTable, rr() As DataRow, cnt As Integer = 0, str1, str2 As String
        Dim oPIDU As New clsPIDUFolder(Me.Controller)
        Dim dsFile As DataSet = oPIDU.FileShareDataset(False)
        dt = myWinData.SelectDistinct(myVueAll.mainGrid.myDS.Tables(0), "pidunitid", , , "pidunitid is not null and isnull(numdup,0)=0")
        For Each r1 As DataRow In dt.Select
            rr = Me.dtFile.Select("pidunitid=" & r1("pidunitid"))
            If rr.Length > 0 Then
                Dim strFileRoot As String = ""
                objPIDU.EnsureWODirectoryStructure(dsFile, rr(0), strFileRoot, "")
                rr(0)("fileroot") = strFileRoot
                For Each r2 As DataRow In myVueAll.mainGrid.myDS.Tables(0).Select("pidunitid=" & r1("pidunitid") & " and isnull(numdup,0)=0")
                    cnt = cnt + 1
                    If myUtils.cValTN(r2("ctrl")) > 0 Then
                        'move to control
                        str1 = rr(0)("fileroot") & "\CTRL"
                    Else
                        'move to drg
                        str1 = rr(0)("fileroot") & "\DRG"
                    End If
                    str2 = str1 & "\" & r2("filename")
                    Me.AppendText("Moving file " & cnt & " -- " & r2("filename") & " --> " & str2)
                    Application.DoEvents()
                    System.IO.File.Move(r2("directory") & "\" & r2("filename"), str2)
                    Me.CreateShortCut(r2("filename"), r2("directory"), str2, str1, "", 0)
                Next
            End If
        Next
        Debug.WriteLine(Me.UltraTextEditor1.Text)
        SQLHelper.SaveResults(Me.dtFile, "select pidunitid,fileroot from pidunit")
    End Sub
    Private Shared Function CreateShortCut(ByVal shortcutName As String, ByVal creationDir As String, ByVal targetFullpath As String, ByVal workingDir As String, ByVal iconFile As String, ByVal iconNumber As Integer) As Boolean
        Dim wShell As WshShell

        Try
            wShell = New WshShell
            If Not IO.Directory.Exists(creationDir) Then
                Dim retVal As DialogResult = MsgBox(creationDir & " does not exist. Do you wish to create it?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo)
                If retVal = DialogResult.Yes Then
                    IO.Directory.CreateDirectory(creationDir)
                Else
                    Return False
                End If
            End If
            Dim shortCut As IWshRuntimeLibrary.IWshShortcut
            shortCut = CType(wShell.CreateShortcut(creationDir & "\" & shortcutName & ".lnk"), IWshRuntimeLibrary.IWshShortcut)
            shortCut.TargetPath = targetFullpath
            shortCut.WindowStyle = 1
            shortCut.Description = shortcutName
            shortCut.WorkingDirectory = workingDir
            shortCut.IconLocation = iconFile & ", " & iconNumber
            shortCut.Save()
            Return True
        Catch ex As System.Exception
            Return False
        End Try
    End Function

    Private Sub btnWOPerm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWOPerm.Click
        Dim oPIDU As New clsPIDUFolder(Me.Controller)
        Dim dsFile As DataSet = oPIDU.FileShareDataset(False)
        For Each r1 As DataRow In dtFile.Select("fileroot is not null and iscompleted=0")
            objPIDU.EnsureWODirectoryStructure(dsFile, r1, "", "")
        Next
    End Sub
End Class
