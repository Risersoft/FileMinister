Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.ServiceProcess
Imports Microsoft.Win32
Imports System.DirectoryServices

Public Class ProjectInstaller

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Dim file As System.IO.StreamWriter
        'file = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
        'file.WriteLine("WINDOW INSTALLER 1 " + Environment.UserDomainName + " " + Environment.MachineName)
        'file.Close()

        'Shell("net user /add FileMinister FileMinister!@", AppWinStyle.Hide, True)
        'Shell("net localgroup administrators FileMinister /add", AppWinStyle.Hide, True)


        Me.ServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem

        'Me.ServiceProcessInstaller1.Username = Environment.MachineName + "\FileMinister"
        'Me.ServiceProcessInstaller1.Password = "FileMinister!@"

        'Add initialization code after the call to InitializeComponent

    End Sub

    Private Sub ServiceInstaller1_AfterInstall(sender As Object, e As InstallEventArgs) Handles ServiceInstaller1.AfterInstall

        'Dim domainname As String = Environment.UserDomainName

        'Dim command As String = "sc.exe config FileMinisterService obj=" + domainname + "\FileMinister password=FileMinister!@"

        'Shell(command, AppWinStyle.Hide, True)

        'Dim file As System.IO.StreamWriter
        'file = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
        'file.WriteLine("WINDOW INSTALLER 2 ")
        'file.Close()

        Dim sc As ServiceController = New ServiceController("FileMinisterService")
        sc.Start()

    End Sub

End Class
