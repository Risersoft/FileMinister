Imports Infragistics.Win.UltraWinGrid
Imports System.Drawing
Imports Syncfusion.Windows.Forms.HTMLUI

Public Class frmPreview
    Inherits frmMax
   Dim fd As frmDWGView, fp As frmPDFView
    Public Sub initForm()

        If Not Me.DesignMode Then
            fd = New frmDWGView
            fd.AddtoTab(Me.UltraTabControl1, "dwgview", True)

            fp = New frmPDFView
            fp.AddtoTab(Me.UltraTabControl1, "pdfview", True)

     
            Me.UltraTabControl1.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard
        End If
    End Sub
    
    Public Sub UnloadPreviews()
        fp.CloseFile()
        fd.CloseFile()
        PreviewPane1.UnloadPreview()
    End Sub
    Public Sub HandleSelectionPreview(path As String)
        If myUtils.cStrTN(path).Trim.Length > 0 Then
            Select Case System.IO.Path.GetExtension(path).Trim.ToLower
                Case ".pdf"
                    Me.UltraTabControl1.Tabs("pdfview").Selected = True
                    fp.LoadFile(path)
                Case ".dwg"
                    Me.UltraTabControl1.Tabs("dwgview").Selected = True
                    fd.OpenFile(path)
                    fd.zoom_extents()
                Case Else
                    PreviewPane1.LoadPreview(path)
                    Me.UltraTabControl1.Tabs("winview").Selected = True
            End Select
        End If
    End Sub
End Class
