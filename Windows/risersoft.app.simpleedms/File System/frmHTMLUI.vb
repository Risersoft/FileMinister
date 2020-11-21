Imports Infragistics.Win.UltraWinGrid
Imports System.Drawing
Imports Syncfusion.Windows.Forms.HTMLUI

Public Class frmHTMLUI
    Inherits frmMax
    Dim htmlelements As Hashtable = New Hashtable()
    Dim mDic As clsCollection(Of String, Image)
    Public Sub initForm()


    End Sub
    Public Sub SetHTML(str1 As String, dic As clsCollection(Of String, Image))
        mDic = dic
        Me.HtmluiControl1.LoadFromString(str1)
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
    Private Sub HtmluiControl1_LinkClicked(sender As Object, e As Syncfusion.Windows.Forms.HTMLUI.LinkForwardEventArgs) Handles HtmluiControl1.LinkClicked
        Dim str1 As String = e.Path


        If System.IO.File.Exists(str1) Then
            Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(str1))

        End If
    End Sub

End Class
