Imports Infragistics.Win.UltraWinGrid
Imports System.Drawing
Imports Syncfusion.Windows.Forms.HTMLUI
Imports System.IO

Public Class frmNotify
    Inherits frmMax
    Dim img1 As IHTMLElement, htmlelements As Hashtable = New Hashtable(), txt1 As IHTMLElement
    Dim mDic As clsCollection(Of String, Image), m_watcher As FileSystemWatcher
    Dim donecount As Integer = 0, listupdates As New List(Of String), strOrigHTML As String
    Public Sub initForm()

        AddHandler Timer1.Tick, AddressOf tmrEditNotify_Tick
        Me.Timer1.Enabled = False
    End Sub
    Public Sub SetHTML(str1 As String, dic As clsCollection(Of String, Image))
        mDic = dic
        strOrigHTML = str1
        Me.HtmluiControl1.LoadFromString(str1)
    End Sub
    Private Sub htmluiControl1_LoadFinished(ByVal sender As Object, ByVal e As System.EventArgs) Handles HtmluiControl1.LoadFinished

        Me.img1 = Me.HtmluiControl1.Document.GetElementByUserId("img1")
        If Not img1 Is Nothing Then Me.img1.Parent.Children.Clear()

    End Sub


    Private Sub HtmluiControl1_PreRenderDocument(sender As Object, e As Syncfusion.Windows.Forms.HTMLUI.PreRenderDocumentArgs) Handles HtmluiControl1.PreRenderDocument
        Me.htmlelements = e.Document.GetElementsByNameHash()
        Dim imgs As ArrayList = CType(IIf(TypeOf Me.htmlelements("img") Is ArrayList, Me.htmlelements("img"), Nothing), ArrayList)
        If Not imgs Is Nothing Then
            For Each elem As IMGElementImpl In imgs
                Dim src As String = elem.Attributes("src").Value
                If mDic.ContainsKey(src) Then
                    CType(elem, IMGElementImpl).Image = mDic(src)

                End If

            Next elem
        End If
    End Sub
    Public Sub StopWatching()
        Me.Timer1.Enabled = False
        donecount = 0
        listupdates.Clear()
        If Not m_watcher Is Nothing Then
            m_watcher.EnableRaisingEvents = False
            m_watcher.Dispose()
            Me.HtmluiControl1.LoadFromString(strOrigHTML)
        End If
    End Sub
    Public Sub BeginWatching(path As String)
        m_watcher = New System.IO.FileSystemWatcher()
        m_watcher.Filter = "*.*"
        m_watcher.Path = path & "\"
        m_watcher.IncludeSubdirectories = True

        m_watcher.NotifyFilter = NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName Or NotifyFilters.Size
        AddHandler m_watcher.Changed, AddressOf OnChanged
        AddHandler m_watcher.Created, AddressOf OnChanged
        AddHandler m_watcher.Deleted, AddressOf OnChanged
        AddHandler m_watcher.Renamed, AddressOf OnRenamed
        m_watcher.EnableRaisingEvents = True
        Me.Timer1.Enabled = True
    End Sub
    Private Sub OnChanged(sender As Object, e As FileSystemEventArgs)
        Dim str1 As String

        str1 = String.Format("{0} {1} on {2}", e.FullPath, e.ChangeType.ToString, DateTime.Now.ToString)
        Me.AddString(str1)
    End Sub

    Private Sub OnRenamed(sender As Object, e As RenamedEventArgs)
        Dim str1 As String

        str1 = String.Format("{0} {1} to {2} on {3}", e.OldFullPath, e.ChangeType.ToString, e.Name, DateTime.Now.ToString)
        Me.AddString(str1)
    End Sub
    Private Sub AddString(str1 As String)
        listupdates.Add(str1)
    End Sub
 
 

    Private Sub tmrEditNotify_Tick(sender As Object, e As EventArgs)
        For i As Integer = donecount To listupdates.Count - 1
            img1.Parent.InnerHTML = listupdates(i) & "<BR/>" & img1.Parent.InnerHTML
        Next
        donecount = listupdates.Count
    End Sub
End Class
