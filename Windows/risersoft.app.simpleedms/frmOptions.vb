Imports ug = Infragistics.Win.UltraWinGrid
Imports risersoft.shared.db
Imports risersoft.app.shared

Public Class frmOptions
    Inherits frmMax


    Public Sub InitForm()

        WinFormUtils.SetButtonConf(Me.btnOK, Me.btnCancel, Me.btnSave)
        Me.myView.SetGrid(, Me)
        Me.chk_ShowLog.Checked = True
    End Sub
    Private Sub reCalc()
        Me.LabelDB.Text = mySQL.DBInfo(myApp)
        If (myApp.dicSettings IsNot Nothing) AndAlso myApp.dicSettings.Exists("fileserver") Then Me.LabelFile.Text = String.Format("Path = {0}", myApp.dicSettings("fileserver")) Else Me.LabelFile.Text = "Not Selected"

    End Sub
    Public Sub reCalcSystemOptions()

        If myApp.objAppExtender IsNot Nothing Then
            Me.TextCompanyName.Text = myApp.objAppExtender.SystemOptions("companyname")
            Me.TextCompanyAddress.Text = myApp.objAppExtender.SystemOptions("companyaddress")
            Me.GroupBoxFileServer.Visible = myApp.objAppExtender.FileServerRequired
        Else
            Me.TextCompanyName.Text = ""
            Me.TextCompanyAddress.Text = ""
            Me.GroupBoxFileServer.Visible = True
        End If

    End Sub
    Public Overrides Function PrepForm(oView As clsWinView, ByVal prepMode As EnumfrmMode, ByVal prepIdx As String, Optional ByVal strXML As String = "") As Boolean

        Me.reCalc()
        Me.reCalcSystemOptions()
        Me.pView = oView
        PrepForm = True

    End Function
    Public Overrides Function VSave() As Boolean
        VSave = False
        Me.InitError()

        If Me.CanSave Then
            myRiserApp.ActivateOptions(False)
            myXMLUtils.WriteFile(myRiserApp.objAppExtender.SettingsFilePath, myApp.dicSettings, myApp.dicMRU)
            Dim oApp As clsExtendRSAppBase = CType(myApp.objAppExtender, clsExtendRSAppBase)
            If Not myApp.dbConn Is Nothing Then
                Try
                    oApp.GetSystemOptionsTable(True)        'database may have been changed.
                    Dim nr1 As DataRow
                    If oApp.dtSystemOptions.Rows.Count = 0 Then
                        nr1 = oApp.dtSystemOptions.NewRow
                        oApp.dtSystemOptions.Rows.Add(nr1)
                    Else
                        nr1 = oApp.dtSystemOptions.Rows(0)
                    End If
                    If Me.TextCompanyName.Text.Trim.Length > 0 Then
                        'If text boxes are left blank, existing database options will be used.
                        nr1("companyname") = Me.TextCompanyName.Text
                        nr1("companyaddress") = Me.TextCompanyAddress.Text
                    End If
                    SQLHelper2.SaveResults(oApp.dtSystemOptions, "select * from systemoptions")
                Catch ex As Exception
                    Debug.WriteLine(ex.Message)
                End Try
            End If
            myRiserApp.CopySamples(myRiserApp.FileServerPath)
            If pView IsNot Nothing Then pView.Refresh()
            VSave = True
        End If
        Me.Refresh()

    End Function

    Private Sub btnChangeDB_Click(sender As System.Object, e As System.EventArgs) Handles btnChangeDB.Click
        Dim newdb As Boolean, str2 As String = ""
        If (Not myApp.dbConn Is Nothing) Then str2 = myApp.Model.DBConnString 'so that security info goes !
        Dim str1 As String = myDBUtils.SetupDB(str2, myRiserApp.objAppExtender.DacPacFilePath, myRiserApp.objAppExtender.DBFilePath, myRiserApp.objAppExtender.DBScriptAppCode, newdb, Me.chk_ShowLog.Checked)
        If str1.Trim.Length > 0 Then
            myXMLUtils.SetConnectionString(myApp.dicSettings, str1)
            myApp.dbConn = myApp.OpenConnFromString(str1)
            Me.LabelDB.Text = mySQL.DBInfo(myApp)
        End If
    End Sub

    Private Sub btnChangeFile_Click(sender As System.Object, e As System.EventArgs) Handles btnChangeFile.Click
        Dim str1 As String = myWinApp.SetupFileServer("")
        If str1.Trim.Length > 0 Then
            myXMLUtils.SetFileServer(myApp.dicSettings, str1)
            Me.LabelFile.Text = String.Format("Path = {0}", str1)
        End If

    End Sub
End Class
