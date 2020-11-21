Imports System.IO

Public Class OpenHandle
    Public Shared Function GetOpenFiles(basePath As String) As List(Of String)
        ' One file parameter to the executable
        Dim sourceName As String = basePath
        ' The second file parameter to the executable
        Dim targetName As String = "C:\ProgramData\FileMinister\handlerresult.txt"

        ' New ProcessStartInfo created
        Dim p As New ProcessStartInfo

        ' Specify the location of the binary
        p.FileName = "C:\ProgramData\FileMinister\Handle.exe"

        ' Use these arguments for the process
        'p.Arguments = "/accepteula " + sourceName + " > " + targetName
        p.Arguments = "/accepteula " + sourceName
        ' Use a hidden window
        'p.WindowStyle = ProcessWindowStyle.Hidden
        p.CreateNoWindow = True

        p.UseShellExecute = False

        p.RedirectStandardOutput = True
        p.RedirectStandardError = True

        Dim fileList As New List(Of String)
        ' Start the process
        Try
            ' Start the process with the info we specified.
            ' Call WaitForExit and then the using-statement will close.
            Using exeProcess As Process = Process.Start(p)
                exeProcess.WaitForExit()
                Using reader As StreamReader = exeProcess.StandardOutput
                    While Not reader.EndOfStream
                        Dim line = reader.ReadLine().ToLower()
                        If (line.Contains(basePath.ToLower())) Then
                            'pick all these from appsettings TSVNCache.exe, cmd.exe, Handle.exe, Handle64.exe
                            If Not line.StartsWith("explorer.exe") AndAlso Not line.StartsWith("TSVNCache.exe") AndAlso Not line.StartsWith("cmd.exe") AndAlso Not line.StartsWith("Handle64.exe") AndAlso Not line.StartsWith("Handle.exe") Then
                                fileList.Add(line.Substring(line.IndexOf(basePath.ToLower())))
                            End If
                        End If
                    End While
                End Using
            End Using
        Catch

            ' Log error.
        End Try

        'Dim fileList As New List(Of String)
        'If File.Exists(targetName) Then
        '    For Each data As String In File.ReadLines(targetName)
        '        Dim line = data.ToLower()
        '        If (line.Contains(basePath.ToLower())) Then
        '            'todo: pick all these from appsettings TSVNCache.exe, cmd.exe, Handle.exe, Handle64.exe
        '            If Not line.StartsWith("explorer.exe") Then
        '                fileList.Add(line.Substring(line.IndexOf(basePath.ToLower())))
        '            End If
        '        End If
        '    Next
        'End If
        Return fileList
    End Function

End Class
