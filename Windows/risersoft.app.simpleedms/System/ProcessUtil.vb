Imports System.IO
Imports System.Runtime.InteropServices

Public Class ProcessUtil
    Public Shared Function ProcessesLockingFile(path As String) As String
        Dim tool As Process = New Process, strLocks As String = ""

        If myUtils.cStrTN(path).Trim.Length > 0 Then
            Dim ur1 As Uri = New Uri(path)
            tool.StartInfo.FileName = Environment.SystemDirectory & "\openfiles.exe"
            tool.StartInfo.Arguments = " /query /v /fo csv /s " & ur1.Host
            tool.StartInfo.UseShellExecute = False
            tool.StartInfo.CreateNoWindow = True
            tool.StartInfo.RedirectStandardOutput = True
            tool.Start()
            Dim outputTool As String = tool.StandardOutput.ReadToEnd()

            'string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            'foreach(Match match in Regex.Matches(outputTool, matchPattern))
            '{
            '    Process.GetProcessById(int.Parse(match.Value)).Kill();
            '}

            Dim dt1 As DataTable = DataTableFromCSV(outputTool)
            If dt1.Columns.Contains("hostname") Then
                dt1.Columns(dt1.Columns.Count - 1).ColumnName = "FileName"
                For Each r1 As DataRow In dt1.Select("len(filename)>0")
                    If System.IO.Path.GetFileName(r1("filename")).Trim.ToLower = System.IO.Path.GetFileName(path).Trim.ToLower Then
                        'currently matching is only by filename
                        'TODO: do it using shares etc
                        strLocks = strLocks & "<br/>" & r1("accessed by") & " - " & r1("open mode")
                    End If
                Next
            End If
        End If
        If strLocks.Trim.Length = 0 Then strLocks = "<BR/>No Locks Found"

        Return strLocks
    End Function

    Public Shared Function DataTableFromCSV(str1 As String) As DataTable


        Dim arr1() As String = Split(str1, vbCrLf)
        Dim dt1 As New DataTable, nr As DataRow = Nothing
        For i As Integer = 0 To arr1.Length - 1
            Dim arr2() As String = Split(arr1(i), ",")
            If i > 0 Then nr = dt1.NewRow
            For j As Integer = 0 To arr2.Length - 1
                If i > 0 Then
                    'data
                    nr(dt1.Columns(j)) = Replace(arr2(j), """", "")
                Else
                    'title
                    dt1.Columns.Add(Replace(arr2(j), """", ""))
                End If
            Next
            If i > 0 Then dt1.Rows.Add(nr)
        Next
        Return dt1
    End Function
    Public Shared Function GetFileTypeDescription(fileNameOrExtension As String) As String
        Dim shfi As SHFILEINFO
        If IntPtr.Zero <> SHGetFileInfo(fileNameOrExtension, FILE_ATTRIBUTE_NORMAL, shfi, CUInt(Marshal.SizeOf(GetType(SHFILEINFO))), SHGFI_USEFILEATTRIBUTES Or SHGFI_TYPENAME) Then
            Return shfi.szTypeName
        End If
        Return Nothing
    End Function

    <DllImport("shell32")> _
    Private Shared Function SHGetFileInfo(pszPath As String, dwFileAttributes As UInteger, ByRef psfi As SHFILEINFO, cbFileInfo As UInteger, flags As UInteger) As IntPtr
    End Function

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure SHFILEINFO
        Public hIcon As IntPtr
        Public iIcon As Integer
        Public dwAttributes As UInteger
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> _
        Public szDisplayName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)> _
        Public szTypeName As String
    End Structure

    Private Const FILE_ATTRIBUTE_NORMAL As UInteger = &H80
    Private Const SHGFI_TYPENAME As UInteger = &H400
   Private Const SHGFI_USEFILEATTRIBUTES As UInteger = &H10
   
End Class
