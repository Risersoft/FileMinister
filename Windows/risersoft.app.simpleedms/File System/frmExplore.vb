Imports Infragistics.Win.UltraWinGrid
Imports System.Drawing
Imports LogicNP.FileViewControl
Imports Syncfusion.Windows.Forms.HTMLUI
Imports LogicNP.FolderViewControl
Imports System.IO
Imports risersoft.app.mxengg
Imports risersoft.shared.dotnetfx

Public Class frmExplore
    Inherits frmMax
    Dim fp As frmPreview, fn As frmNotify
    Dim fhInfo, fhPerm, fhBackup, fhDetails As frmHTMLUI
    Dim dic As clsCollection(Of String, Image), dsFile As DataSet, currpath As String
    Dim perleft, perright As Single, notedone As Boolean = False, ViewDone As New clsCollecString(Of Boolean)
    Public Sub initForm()
        Dim sql, strh As String, r As DataRow


        fldrView.FileView = flView

        If Not Me.DesignMode Then


            Me.PopulateImages()
            strh = myAssy.GetString(System.Reflection.Assembly.GetExecutingAssembly.GetName.Name, "Info.htm")


            For Each str1 As String In New String() {"info", "perm", "act", "backup", "preview"}
                Dim fm As frmMax
                Select Case str1.Trim.ToLower
                    Case "preview"
                        fp = New frmPreview
                        fm = fp
                    Case "info"
                        fhInfo = New frmHTMLUI
                        fm = fhInfo
                        fhInfo.SetHTML(strh, dic)
                    Case "perm"
                        fhPerm = New frmHTMLUI
                        fm = fhPerm
                        fhPerm.SetHTML(strh, dic)
                    Case "act"
                        fn = New frmNotify
                        fm = fn
                        fn.SetHTML(strh, dic)
                    Case "backup"
                        fhBackup = New frmHTMLUI
                        fm = fhBackup
                        fhBackup.SetHTML(strh, dic)
                End Select
                If Not fm Is Nothing Then
                    fm.MakeDownLevel()
                    Me.UltraExplorerBar1.Groups(str1).Container.Controls.Add(fm)
                    fm.Dock = DockStyle.Fill
                    fm.Visible = True
                End If

                fhDetails = New frmHTMLUI
                fhDetails.AddToPanel(Me.PanelDetails, True, DockStyle.Fill)
                fhDetails.SetHTML(strh, dic)
            Next

            dsFile = Me.FileStoreDataSet(True)
        End If
    End Sub
    Public Function FileStoreDataSet(forcefresh As Boolean) As DataSet
        Dim sql As String
        If myFuncsPB.dsFile Is Nothing OrElse forcefresh Then
            Dim dic As New clsCollecString(Of String)
            dic.Add("share", "select * from fileshare")
            dic.Add("dir", "select * from filesharedir")
            dic.Add("pidu", "select * from deslistpidunittot()")
            dic.Add("docu", "select * from deslistprodocu()")
            dic.Add("std", "select * from desliststddrg()")
            dic.Add("type", "Select * from pidunittype")
            Dim ds As DataSet = Me.Controller.DataProvider.objSQLHelper.ExecuteDataset(CommandType.Text, dic)
            myUtils.AddTable(ds, Me.Controller.Tables.Decrossnum(ds.Tables("dir"), "pidunittype", "index"), "dir")
            myFuncsPB.dsFile = ds
        End If
        Return myFuncsPB.dsFile
    End Function

    Public Sub InitViewPIDU(pidunitid As Integer)
        Dim cnt As Integer = 0, newRoot As FOVTreeNode
        Dim rr() As DataRow, WOPath As String

        rr = dsFile.Tables("pidu").Select("pidunitid=" & pidunitid)
        If rr.Length > 0 Then
            Me.Text = Me.Text & " for " & rr(0)("woinfo")

            If myUtils.cStrTN(rr(0)("foldername")).Trim.Length > 0 Then
                For Each r1 As DataRow In dsFile.Tables("store").Select("isnull(isbackup,0)=0")
                    If dsFile.Tables("dir").Select("filestoreid=" & r1("filestoreid")).Length > 0 Then
                        Dim strPath As String = System.IO.Path.Combine(r1("uncpath"), rr(0)("foldername"))
                        If cnt = 0 Then
                            fldrView.RootFolder = strPath
                            newRoot = fldrView.GetFirstNode
                        Else
                            newRoot = fldrView.AddRootNode(strPath, -1)
                        End If
                        If Not newRoot Is Nothing Then newRoot.Text = r1("storecode")
                        cnt = cnt + 1
                    End If
                Next

                If fldrView.GetFirstNode IsNot Nothing Then
                    fldrView.GetFirstNode.Selected = True
                    flView.CurrentFolder = fldrView.SelectedNode.Path
                End If
            Else
                Me.fldrView.Hide()
                Me.flView.Hide()
            End If
        Else
            'work order not found.
            Me.fldrView.Hide()
            Me.flView.Hide()
        End If



    End Sub

    Public Sub InitViewServer()
        Dim cnt As Integer = 0, newRoot As FOVTreeNode

        For Each r1 As DataRow In dsFile.Tables("store").Select("isnull(isbackup,0)=0")
            If dsFile.Tables("dir").Select("filestoreid=" & r1("filestoreid")).Length > 0 Then
                If cnt = 0 Then
                    fldrView.RootFolder = r1("uncpath")
                    newRoot = fldrView.GetFirstNode
                Else
                    newRoot = fldrView.AddRootNode(r1("uncpath"), -1)
                End If
                newRoot.Text = r1("storecode")
                cnt = cnt + 1
            End If
        Next
        If Not newRoot Is Nothing Then
            fldrView.GetFirstNode.Selected = True
            flView.CurrentFolder = fldrView.SelectedNode.Path
        End If

    End Sub
#Region "Menu Commands"
    Private Sub fldrView_ContextMenuHint(ByVal sender As System.Object, ByVal e As LogicNP.FolderViewControl.FolderViewContextMenuHintEventArgs) Handles fldrView.ContextMenuHint
        formStatusBar.Panels(0).Text = e.Hint
    End Sub

    Private Sub flView_ContextMenuHint(ByVal sender As System.Object, ByVal e As LogicNP.FileViewControl.ContextMenuHintEventArgs) Handles flView.ContextMenuHint
        formStatusBar.Panels(0).Text = e.Hint
    End Sub
    Private Sub formToolBar_ButtonClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolBarButtonClickEventArgs) Handles formToolBar.ButtonClick
        If e.Button Is GoUp Then
            flView.GoUp()
        Else
            If e.Button Is Cut Then
                ExecuteShellCommand(LogicNP.FileViewControl.ShellCommands.Cut)
            Else
                If e.Button Is Copy Then
                    ExecuteShellCommand(LogicNP.FileViewControl.ShellCommands.Copy)
                Else
                    If e.Button Is Paste Then
                        ExecuteShellCommand(LogicNP.FileViewControl.ShellCommands.Paste)
                    Else
                        If e.Button Is Delete Then
                            ExecuteShellCommand(LogicNP.FileViewControl.ShellCommands.Delete)
                        Else
                            If e.Button Is ViewStyles Then
                                If (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon) Then
                                    flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails
                                ElseIf (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails) Then
                                    flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List
                                ElseIf (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List) Then
                                    flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
                                ElseIf (flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report) Then
                                    flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub ExecuteShellCommand(ByVal cmd As LogicNP.FileViewControl.ShellCommands)
        If fldrView.Focused Then
            fldrView.SelectedNode.ExecuteShellCommand(CType(cmd, LogicNP.FolderViewControl.ShellCommands))
        Else
            If (cmd = LogicNP.FileViewControl.ShellCommands.Paste) Then
                flView.ExecuteCmdForFolder(cmd)
            Else
                flView.ExecuteCmdForAllSelected(cmd)
            End If
        End If
    End Sub
    Private Sub menuLargeIcons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuLargeIcons.Click
        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.LargeIcon
    End Sub

    Private Sub menuThumbnails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuThumbnails.Click
        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Thumbnails
    End Sub

    Private Sub menuList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuList.Click
        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.List
    End Sub

    Private Sub menuReport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReport.Click
        flView.ViewStyle = LogicNP.FileViewControl.ViewStyles.Report
    End Sub
#End Region

    Private Sub flView_AfterSelectionChange(sender As System.Object, e As LogicNP.FileViewControl.FileViewEventArgs) Handles flView.AfterSelectionChange
        Try

            If currpath <> e.Item.Path Then
                Me.formStatusBar.Panels(0).Text = ""
                currpath = e.Item.Path
                Me.UnloadOnNewSelection()
                Me.HandleSelectionInfo(e.Item.Path)
                Me.HandleSelectionDetails(e.Item.Path)
                ViewDone.Clear()
                If Me.UltraExplorerBar1.ActiveGroup IsNot Nothing Then Me.HandleActiveGroup(Me.UltraExplorerBar1.ActiveGroup.Key)
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub
    Private Sub UnloadOnNewSelection()
        fp.UnloadPreviews()
        fn.StopWatching()
    End Sub
    Private Sub HandleActiveGroup(groupkey As String)
        If Not ViewDone.Exists(groupkey) Then
            Select Case groupkey.Trim.ToLower
                Case "preview"
                    fp.HandleSelectionPreview(currpath)
                Case "perm"
                    Me.HandleSelectionLocks(currpath)
                Case "act"
                    fn.BeginWatching(System.IO.Path.GetDirectoryName(currpath))
                Case "backup"
                    Me.HandleSelectionBackup(currpath)
            End Select
            ViewDone.AddUpd(groupkey, True)
        End If
    End Sub

    Private Sub PopulateImages()
        If dic Is Nothing Then
            dic = New clsCollection(Of String, Image)
            Dim img As Image = myBMP.ScaleImageWidth(My.Resources.logo, 300)
            dic.Add("logo.png", img)
        End If
    End Sub
    Private Sub HandleSelectionDetails(path As String)
        Dim str1 As String = "", str2 As String

        Me.PopulateImages()
        dic.AddUpd("icon.png", flView.FirstSelectedItem.GetShellIcon(LogicNP.FileViewControl.ShellIconTypes.Large).ToBitmap)
        str1 = myAssy.GetString(System.Reflection.Assembly.GetExecutingAssembly.GetName.Name, "Info2.htm")


        Dim info As FileSystemInfo
        If flView.FirstSelectedItem.IsFolder Then info = New DirectoryInfo(path) Else info = New FileInfo(path)
        Dim spanstyle As String = "<span style=""color:#999999;"">"

        str2 = "<table border=""0"">"
        str2 = str2 & "<tr><td rowspan=""2""><img src=""icon.png""  /></td>"
        str2 = str2 & "<td>" & System.IO.Path.GetFileName(path) & "</td>"
        str2 = str2 & "<td width=""10""/><td>" & spanstyle & "Date Modified:</span> " & info.LastWriteTime.ToShortDateString & " " & info.LastWriteTime.ToShortTimeString & "</td>"
        str2 = str2 & "<td width=""10""/><td>" & spanstyle & "Date Created:</span> " & info.CreationTime.ToShortDateString & " " & info.CreationTime.ToShortTimeString & "</td>"
        If info.GetType Is GetType(FileInfo) Then
            str2 = str2 & "</tr><tr><td>" & ProcessUtil.GetFileTypeDescription(System.IO.Path.GetExtension(path)) & "</td>"
            str2 = str2 & "<td width=""10""/><td>" & spanstyle & "Size:</span> " & CType(info, FileInfo).Length / 1000 & " KB</td>"
        Else
            str2 = str2 & "</tr><tr><td>" & "Folder" & "</td>"
        End If
        str2 = str2 & "</tr></table>"



        Dim index As Integer = InStr(str1.ToUpper, "</BODY>")
        str1 = str1.Insert(index - 1, str2)

        fhDetails.SetHTML(str1, dic)
    End Sub
    Private Sub HandleSelectionBackup(path As String)
        Dim str1 As String = ""

        Me.PopulateImages()
        'Throw New Exception("Exception Occured")
        str1 = myAssy.GetString(System.Reflection.Assembly.GetExecutingAssembly.GetName.Name, "Info.htm")
        Dim str2 As String = "Backup Lists"

        If myUtils.cStrTN(path).Trim.Length > 0 Then

            For Each r1 As DataRow In dsFile.Tables("store").Select
                'For Each r2 As DataRow In dsFile.Tables("dir").Select(myUtils.CombineWhere(False, "filestoreid=" & r1("filestoreid"), "dirtype='wor'"))
                '    Dim rootpath1 As String = System.IO.Path.Combine(r1("uncpath"), r2("relativepath"))

                'above was commented because we can find backup history for any file, not just  on root wo folder
                If myPathUtils.LastCommonRootPath(r1("uncpath"), path).Trim.ToLower = r1("uncpath").Trim.ToLower Then
                    'found the filestore entry.
                    Dim rr() As DataRow = dsFile.Tables("store").Select("filestoreid=" & myUtils.cValTN(r1("backupstoreid")))
                    If rr.Length > 0 Then
                        Dim path2 As String = System.IO.Path.Combine(rr(0)("uncpath"), r1("backuprootpathincr"))
                        str2 = "<h3>Backup Versions for " & System.IO.Path.GetFileName(path) & "</h3><br/>"
                        str2 = str2 & "<table border=""1"" cellpadding=""1""><tr><th>Date</th><th>Size</th><th>Open</th></tr>"

                        Dim relpath As String = myPathUtils.RelativePathTo(r1("uncpath"), System.IO.Path.GetDirectoryName(path))
                        Dim dic As New clsCollection(Of DateTime, FileInfo)
                        'My.Computer.FileSystem.GetDirectories(path2, FileIO.SearchOption.SearchAllSubDirectories, relpath)
                        For Each monthdir As String In My.Computer.FileSystem.GetDirectories(path2, FileIO.SearchOption.SearchTopLevelOnly)
                            For Each datedir As String In My.Computer.FileSystem.GetDirectories(monthdir, FileIO.SearchOption.SearchTopLevelOnly)
                                Dim founddir As String = System.IO.Path.Combine(datedir, relpath)
                                If System.IO.Directory.Exists(founddir) Then
                                    For Each foundfile As String In My.Computer.FileSystem.GetFiles(founddir, FileIO.SearchOption.SearchTopLevelOnly, System.IO.Path.GetFileName(path))
                                        Dim finfo As FileInfo = New FileInfo(foundfile)
                                        dic.Add(finfo.LastWriteTime, finfo)

                                    Next
                                End If
                            Next
                        Next

                        For Each finfo As FileInfo In dic.OrderByDescending(Function(s) s.Key).Select(Function(s) s.Value)
                            str2 = str2 & "<tr><td>" & finfo.LastWriteTime.ToShortDateString & " " & finfo.LastWriteTime.ToShortTimeString & "<br/></td><td>" & finfo.Length / 1000 & " KB</td>"
                            str2 = str2 & "<td><a href=""" & finfo.FullName & """>Open</a></td></tr>"

                        Next
                        str2 = str2 & "</table>"
                    End If
                    Exit For
                End If
            Next
        End If

        Dim index As Integer = InStr(str1.ToUpper, "</BODY>")
        str1 = str1.Insert(index - 1, str2)

        fhBackup.SetHTML(str1, dic)

    End Sub
    Private Sub HandleSelectionLocks(path As String)
        Dim str1 As String = ""

        Me.PopulateImages()
        'Throw New Exception("Exception Occured")
        str1 = myAssy.GetString(System.Reflection.Assembly.GetExecutingAssembly.GetName.Name, "Info.htm")

        Dim str2 As String = ProcessUtil.ProcessesLockingFile(path)
        Dim index As Integer = InStr(str1.ToUpper, "</BODY>")
        str1 = str1.Insert(index - 1, str2)

        fhPerm.SetHTML(str1, dic)

    End Sub
    Private Sub HandleSelectionInfo(path As String)
        Dim str1 As String = ""

        Me.PopulateImages()
        'Throw New Exception("Exception Occured")
        str1 = myAssy.GetString(System.Reflection.Assembly.GetExecutingAssembly.GetName.Name, "Info.htm")

        Dim str2 As String = Me.BuildSelectionHTML(path)
        Dim index As Integer = InStr(str1.ToUpper, "</BODY>")
        str1 = str1.Insert(index - 1, str2)

        fhInfo.SetHTML(str1, dic)

    End Sub
    Private Function BuildSelectionHTML(path As String) As String
        Dim str1 As String = ""
        For Each r1 As DataRow In dsFile.Tables("store").Select
            For Each r2 As DataRow In dsFile.Tables("dir").Select(myUtils.CombineWhere(False, "filestoreid=" & r1("filestoreid"), "dirtype='wor'"))
                Dim rootpath1 As String = System.IO.Path.Combine(r1("uncpath"), r2("relativepath"))
                If myPathUtils.LastCommonRootPath(rootpath1, path).Trim.ToLower = rootpath1.Trim.ToLower Then
                    For Each r3 As DataRow In dsFile.Tables("pidu").Select(myUtils.CombineWhere(False, "len(foldername)>0", "pidunittype='" & r2("pidunittype") & "'"))
                        Dim rootpath2 As String = System.IO.Path.Combine(r1("uncpath"), r3("foldername"))
                        If myPathUtils.LastCommonRootPath(rootpath2, path).Trim.ToLower = rootpath2.Trim.ToLower Then
                            'work order row r3 found.
                            str1 = "<h3>Work Order Information</h3><br/>"
                            str1 = str1 & "<table border=""1"" cellpadding=""1""><tr><th>Field</th><th>Value</th></tr>"
                            For Each str2 As String In New String() {"WONum", "Dated", "Customer", "Descrip", "Rating", "FileNumComp", "DesignDate", "Remarks"}
                                If r3.Table.Columns.Contains(str2) Then str1 = str1 & "<tr><td>" & str2 & "</td><td>" & r3(str2).ToString & "</td></tr>"
                            Next
                            str1 = str1 & "</table>"

                            For Each r4 As DataRow In dsFile.Tables("docu").Select(myUtils.CombineWhere(False, "len(Drawing)>0", "pidunitid=" & r3("pidunitid")), "desdocgrpid,serialnum")
                                Dim filename As String = Me.CompareString(System.IO.Path.GetFileNameWithoutExtension(path))
                                Dim docnum As String = Me.CompareString(r4("drawing"))
                                If docnum.Trim.ToLower = filename.Trim.ToLower Then
                                    'document found
                                    str1 = str1 & "</br><h3>Document Information</h3><br/>"
                                    str1 = str1 & "<table border=""1"" cellpadding=""1""><tr><th>Field</th><th>Value</th></tr>"
                                    For Each str2 As String In New String() {"Serial", "Drawing", "Revision", "Document", "DistriDescrip", "NumSheets", "Remarks"}
                                        str1 = str1 & "<tr><td>" & str2 & "</td><td>" & r4(str2).ToString & "</td></tr>"
                                    Next
                                    str1 = str1 & "</table>"
                                    Exit For
                                End If
                            Next
                            Exit For
                        End If
                    Next

                End If

            Next
            For Each r2 As DataRow In dsFile.Tables("dir").Select(myUtils.CombineWhere(False, "filestoreid=" & r1("filestoreid"), "dirtype='std'"))
                Dim rootpath1 As String = System.IO.Path.Combine(r1("uncpath"), r2("relativepath"))
                If myPathUtils.LastCommonRootPath(rootpath1, path).Trim.ToLower = rootpath1.Trim.ToLower Then
                    'std folder
                    For Each r3 As DataRow In dsFile.Tables("std").Select(myUtils.CombineWhere(False, "len(completenum)>0"))
                        Dim filename As String = Me.CompareString(System.IO.Path.GetFileNameWithoutExtension(path))
                        Dim docnum As String = Me.CompareString(r3("completenum"))
                        If docnum.Trim.ToLower = filename.Trim.ToLower Then
                            'document found
                            str1 = str1 & "<h3>Standard Document Information</h3><br/>"
                            str1 = str1 & "<table border=""1"" cellpadding=""1""><tr><th>Field</th><th>Value</th></tr>"
                            For Each str2 As String In New String() {"CompleteNum", "DrgName", "Descrip", "RevNum", "RevDate"}
                                str1 = str1 & "<tr><td>" & str2 & "</td><td>" & r3(str2).ToString & "</td></tr>"
                            Next
                            str1 = str1 & "</table>"
                            Exit For
                        End If
                    Next
                End If

            Next
        Next

        Return str1

    End Function
    Private Function CompareString(str1 As String) As String
        Dim str2 As String = str1
        'remove space
        For Each str3 As String In New String() {" "}
            str2 = Replace(str2, str3, "")
        Next

        '--if any letter is coming after the / or \, make it blank----------------------
        Dim arr() As Char = str2.ToCharArray
        str2 = ""
        For i As Integer = 0 To arr.Length - 1
            Dim c As Char = arr(i)
            If i < arr.Length - 1 AndAlso myUtils.IsInList(c.ToString, "\", "/") AndAlso System.Text.RegularExpressions.Regex.Match(arr(i + 1), "[a-zA-Z]").Success Then
                'ignore / \
            Else
                str2 = str2 & c
            End If
        Next
        '--------------------------------------------------------------------------------

        'replace / \ _ by -
        For Each str3 As String In New String() {"/", "\", "_"}
            str2 = Replace(str2, str3, "-")
        Next
        Return str2
    End Function

    Private Sub UltraExplorerBar1_ActiveGroupChanged(sender As Object, e As Infragistics.Win.UltraWinExplorerBar.GroupEventArgs) Handles UltraExplorerBar1.ActiveGroupChanged
        Me.HandleActiveGroup(e.Group.Key)
    End Sub

    Private Sub frmExplore_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.NoteWidthRatios()
    End Sub

    Private Sub frmExplore_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        If notedone Then
            Me.fldrView.Width = Me.Width * perleft
            Me.UltraExplorerBar1.Width = Me.Width * perright
        End If
    End Sub


    Private Sub NoteWidthRatios()
        perleft = Me.fldrView.Width / Me.Width
        perright = Me.UltraExplorerBar1.Width / Me.Width
        notedone = True
    End Sub

    Private Sub UltraSplitter2_CollapsedChanged(sender As Object, e As System.EventArgs) Handles UltraSplitter2.CollapsedChanged
        Me.NoteWidthRatios()
    End Sub



    Private Sub UltraSplitter2_SplitterDragCompleted(sender As Object, e As Infragistics.Win.Misc.SplitterDragCompletedEventArgs) Handles UltraSplitter2.SplitterDragCompleted
        Me.NoteWidthRatios()
    End Sub

    Private Sub UltraSplitter3_CollapsedChanged(sender As Object, e As System.EventArgs) Handles UltraSplitter3.CollapsedChanged
        Me.NoteWidthRatios()
    End Sub

    Private Sub UltraSplitter3_SplitterDragCompleted(sender As Object, e As Infragistics.Win.Misc.SplitterDragCompletedEventArgs) Handles UltraSplitter3.SplitterDragCompleted
        Me.NoteWidthRatios()
    End Sub

End Class
