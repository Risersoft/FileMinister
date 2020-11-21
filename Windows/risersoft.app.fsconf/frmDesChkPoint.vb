Imports Microsoft.Office.Interop
Imports risersoft.app.mxform.eto

Public Class frmDesChkPoint
    Inherits frmMax
    Dim oDoc As Word.Document

    Public Sub InitForm()
        myView.SetGrid(Me.UltraGrid1)
        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
    End Sub

    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean
        Me.FormPrepared = False
        Dim copyOK As Boolean = False
        Dim objModel As frmDesChkPointModel = Me.InitData("frmDesChkPointModel", oview, prepMode, prepIdx, strXML)
        If Me.BindModel(objModel, oview) Then
            'Me.oFramer1.CreateNew("Word.Document.8")
            'oDoc = Me.oFramer1.ActiveDocument
            If myUtils.cStrTN(myRow("contentxml")).Trim.Length > 0 Then oDoc.Content.InsertXML(myRow("contentxml"))
            'Me.oFramer1.Titlebar = False
            Me.FormPrepared = True
        End If
        Return Me.FormPrepared
    End Function

    Public Overrides Function BindModel(NewModel As clsFormDataModel, oView As clsView) As Boolean
        If MyBase.BindModel(NewModel, oView) Then
            Me.myView.PrepEdit(Me.Model.GridViews("Group"))
            Me.myView.mainGrid.HighlightRows()
            Return True
        End If
        Return False
    End Function

    Protected Friend Function HandleDocument()
        If oDoc Is Nothing Then
            myRow("contentxml") = DBNull.Value
            myRow("pdfsource") = DBNull.Value
        Else
            myRow("contentxml") = oDoc.Content.XML()
            Dim str1 As String = myWinFtp.GetTempLocalFileWithExt("temp.pdf")
            oDoc.Content.ExportAsFixedFormat(str1, Word.WdExportFormat.wdExportFormatPDF)
            If System.IO.File.Exists(str1) Then
                Dim str2 As String = myWinFtp.UpLoad(str1, "PDF_" & myRow("deschkpointid") & ".pdf", "/TRN/CHKPNT/")
                myRow("pdfsource") = str2
                myWinFtp.DeleteLocalFile(str1)
            End If
        End If
        Return True
    End Function

    Public Overrides Function VSave() As Boolean
        Me.InitError()
        VSave = False
        cm.EndCurrentEdit()
        If Me.ValidateData() Then
            If Me.SaveModel() Then
                frmMode = EnumfrmMode.acEditM
                HandleDocument()
                If Me.SaveModel() Then
                    Return True
                End If
            End If
        Else
            Me.SetError()
        End If
        Me.Refresh()
    End Function

    Private Sub frmClamp_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        'Me.oFramer1.Close()
    End Sub

    Private Sub btnAddGroup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddGroup.Click
        Dim f As New frmDesCPGroup
        If f.PrepForm(myView, EnumfrmMode.acAddM, "") Then
            If f.ShowDialog Then
                myUtils.CopyOneRow(f.myRow.Row, myView.mainGrid.myDS.Tables(0))
            End If
        End If
    End Sub

    Private Sub btnEditGroup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditGroup.Click
        myView.ContextRow = myView.mainGrid.ActiveRow
        If Not myView.ContextRow Is Nothing Then
            Dim f As New frmDesCPGroup
            If f.PrepForm(myView, EnumfrmMode.acEditM, myUtils.cValTN(myView.ContextRow.CellValue("descpgroupid"))) Then
                If f.ShowDialog Then
                    myUtils.MergeDataRow(f.myRow.Row, myUtils.DataRowFromGridRow(myView.ContextRow))
                End If
            End If
        End If
    End Sub
End Class
