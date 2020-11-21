Imports System.Configuration.Install
Imports System.Reflection
Imports System.IO

Public Class InstallerClass

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add initialization code after the call to InitializeComponent

        AddHandler Me.Committed, AddressOf MyInstaller_Committed
        ' Attach the 'Committed' event.
        ' Attach the 'Committing' event.
        AddHandler Me.Committing, AddressOf MyInstaller_Committing

    End Sub

    Private Sub MyInstaller_Committing _
        (ByVal sender As Object, ByVal e As InstallEventArgs)
        'Console.WriteLine("");
        'Console.WriteLine("Committing Event occurred.");
        'Console.WriteLine("");
    End Sub

    ' Event handler for 'Committed' event.
    Private Sub MyInstaller_Committed(ByVal sender As Object, _
    ByVal e As InstallEventArgs)
        Try
            Directory.SetCurrentDirectory _
        (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            Process.Start(Path.GetDirectoryName _
        (Assembly.GetExecutingAssembly().Location) + "\FileMinister.Client.TrayApp.exe")
            ' Do nothing... 
        Catch
        End Try
    End Sub

    Public Overrides Sub Install(stateSaver As IDictionary)
        MyBase.Install(stateSaver)
    End Sub


    Public Overloads Overrides Sub Commit(ByVal savedState As IDictionary)
        MyBase.Commit(savedState)
    End Sub

    ' Override the 'Rollback' method.
    Public Overloads Overrides Sub Rollback(ByVal savedState As IDictionary)
        MyBase.Rollback(savedState)
    End Sub

End Class
