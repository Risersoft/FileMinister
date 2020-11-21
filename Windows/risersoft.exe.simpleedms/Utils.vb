Imports risersoft.app.shared
Imports risersoft.app.shared.mxengg
Imports risersoft.app.simpleedms
Imports risersoft.app2.shared
Imports risersoft.shared.win
Imports risersoft.shared.DotnetFx
Imports risersoft.shared
Public Class Utils

    Public Shared Sub Main(args() As String)

        'Explorer and FSConfig are separate appcodes in FSConfig
        DotnetFx.Globals.myApp = New clsRSWinCloudApp(New clsExtendSEDMS)
        myWinApp.CheckInitConsole(args)
        Dim fMain As frmMax = AppStarter.StartWinFormApp(args)
        If Not fMain Is Nothing Then
            Application.Run(fMain)
        End If

    End Sub
    Private Shared Sub CalculateFolder()
        Dim sql As String = "select * from pidunit where len(fileroot)>0"
        Dim dt1 As DataTable = myWinApp.objSQLHelper.ExecuteDataset(CommandType.Text, sql).Tables(0)
        For Each r1 As DataRow In dt1.Select
            r1("foldername") = myPathUtils.RelativePathTo("\\dse1\file", r1("fileroot"))
        Next
        SQLHelper.SaveResults(dt1, sql)

    End Sub
    Private Shared Sub CalculateHash()
        Dim sql As String = "select * from filestoredir where dirtype <> 'wot'"
        Dim dt1 As DataTable = myWinApp.objSQLHelper.ExecuteDataset(CommandType.Text, sql).Tables(0)
        For Each r1 As DataRow In dt1.Select
            r1("relativepathhash") = EncryptionUtilsBase.Md5FromString(r1("relativepath"))
        Next
        SQLHelper.SaveResults(dt1, sql)

    End Sub
    Private Shared Sub populateFSDir()
        Dim sql As String = "select * from filestoredir"
        Dim dt1 As DataTable = myWinApp.objSQLHelper.ExecuteDataset(CommandType.Text, sql).Tables(0)
        For Each str1 As String In New String() {"CTRL", "DOCS", "DOCS\CUSTOMER", "DOCS\PRODUCTION", "DOCS\OFFICE FILE", "CHARTS", "DRG", "DRG\Layout", "DRG\Customer", "DRG\Production", "DRG\Internal", "DRG\Fittings", "DRG\PDF", "NC", "CNC\Insulation", "CNC\Fabrication"}
            Dim nr As DataRow = myTables.AddNewRow(dt1)
            nr("filestoreid") = 1
            nr("dirtype") = "WOT"
            nr("pidunittype1") = "WO"
            nr("pidunittype2") = "RO"
            nr("relativepath") = str1
            nr("relativepathhash") = EncryptionUtilsBase.Md5FromString(str1)
            AssignPermissions(nr, str1, "WO")
        Next
        For Each str1 As String In New String() {"CTRL", "DOCS", "DRG", "DRG\Layout", "DRG\Customer", "DRG\Fittings", "DRG\PDF"}
            Dim nr As DataRow = myTables.AddNewRow(dt1)
            nr("filestoreid") = 1
            nr("dirtype") = "WOT"
            nr("pidunittype1") = "TE"
            nr("relativepath") = str1
            nr("relativepathhash") = EncryptionUtilsBase.Md5FromString(str1)
            AssignPermissions(nr, str1, "TE")
        Next
        myWinApp.objSQLHelper.SaveResults(dt1, sql)

    End Sub
    Private Shared Sub AssignPermissions(nr As DataRow, str1 As String, pidunittype As String)
        Dim strGrp As String = ""
        Select Case str1.Trim.ToLower
            Case "docs", "charts"
                If myUtils.IsInList(pidunittype, "te") Then
                    strGrp = "KANOHAR\Tender"
                Else
                    strGrp = "KANOHAR\FILEDOCS"
                End If
            Case "cnc\insulation", "cnc\fabrication"
                strGrp = "KANOHAR\CNCDRG"
            Case "ctrl"
                strGrp = "KANOHAR\CTRL"
            Case "drg\customer", "drg\production", "drg\fittings", "drg\pdf"
                If myUtils.IsInList(pidunittype, "te") Then
                    strGrp = "KANOHAR\Drawing"
                Else
                    nr("allowwotpidu") = True
                End If
            Case "drg\internal"
                strGrp = "KANOHAR\InternalDrg"
        End Select
        If strGrp.Trim.Length > 0 Then nr("allowwotstd") = strGrp
        If Not myUtils.IsInList(str1, "drg", "docs") Then
            nr("obsoletedirname") = "Obsolete"
        End If
    End Sub
End Class
