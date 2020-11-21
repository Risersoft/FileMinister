Imports System.IO
Imports IWshRuntimeLibrary
Imports System.Reflection

Public Class Installer1

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add initialization code after the call to InitializeComponent

    End Sub


#Region "Overridable Methods"

    Public Overrides Sub Install(stateSaver As IDictionary)
        MyBase.Install(stateSaver)


        Const DESKTOP_SHORTCUT_PARAM As String = "DESKTOP_SHORTCUT_PARAM"
        'Const QUICKLAUNCH_SHORTCUT_PARAM As String = "QUICKLAUNCH_SHORTCUT"
        'Const ALLUSERS_PARAM As String = "ALLUSERS"


        If Not Context.Parameters.ContainsKey(DESKTOP_SHORTCUT_PARAM) Then
            Throw New Exception(String.Format("The {0} parameter has not been provided for the {1} class.", DESKTOP_SHORTCUT_PARAM, Me.[GetType]()))
        End If

        'If Not Context.Parameters.ContainsKey(ALLUSERS_PARAM) Then
        '    Throw New Exception(String.Format("The {0} parameter has not been provided for the {1} class.", ALLUSERS_PARAM, Me.[GetType]()))
        'End If

        'If Not Context.Parameters.ContainsKey(QUICKLAUNCH_SHORTCUT_PARAM) Then
        '    Throw New Exception(String.Format("The {0} parameter has not been provided for the {1} class.", QUICKLAUNCH_SHORTCUT_PARAM, Me.[GetType]()))
        'End If



        Dim installDesktopShortcut As Boolean = Context.Parameters(DESKTOP_SHORTCUT_PARAM) <> String.Empty

        Dim allusers As Boolean = False
        'Dim allusers As Boolean = Context.Parameters(ALLUSERS_PARAM) <> String.Empty
        'Dim installQuickLaunchShortcut As Boolean = Context.Parameters(QUICKLAUNCH_SHORTCUT_PARAM) <> String.Empty

        If installDesktopShortcut Then
            ' If this is an All Users install then we need to install the desktop shortcut for 
            ' all users.  .Net does not give us access to the All Users Desktop special folder,
            ' but we can get this using the Windows Scripting Host.
            Dim desktopFolder As String = Nothing

            If allusers Then
                Try
                    ' This is in a Try block in case AllUsersDesktop is not supported
                    Dim allUsersDesktop As Object = "AllUsersDesktop"
                    Dim shell As WshShell = New WshShell()
                    desktopFolder = shell.SpecialFolders.Item(allUsersDesktop).ToString()
                Catch
                End Try
            End If
            If desktopFolder Is Nothing Then
                desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            End If

            CreateShortcut(desktopFolder, ShortcutName, ShortcutTarget, ShortcutDescription)
        End If

        'If installQuickLaunchShortcut Then
        '    CreateShortcut(QuickLaunchFolder, ShortcutName, ShortcutTarget, ShortcutDescription)
        'End If


    End Sub

    Public Overrides Sub Uninstall(savedState As IDictionary)
        MyBase.Uninstall(savedState)
        DeleteShortcuts()
    End Sub

    Public Overrides Sub Rollback(savedState As IDictionary)
        MyBase.Rollback(savedState)
        DeleteShortcuts()
    End Sub

#End Region

#Region "Private Instance Variables"

    Private _location As String = Nothing
    Private _name As String = Nothing
    Private _description As String = Nothing

#End Region

#Region "Private Properties"

    Private ReadOnly Property QuickLaunchFolder() As String
        Get
            Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Internet Explorer\Quick Launch"
        End Get
    End Property


    Private ReadOnly Property ShortcutTarget() As String
        Get
            If _location Is Nothing Then
                _location = Assembly.GetExecutingAssembly().Location
            End If
            Return _location
        End Get
    End Property


    Private ReadOnly Property ShortcutName() As String
        Get
            If _name Is Nothing Then
                Dim myAssembly As Assembly = Assembly.GetExecutingAssembly()

                Try
                    Dim titleAttribute As Object = myAssembly.GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0)
                    _name = DirectCast(titleAttribute, AssemblyTitleAttribute).Title
                Catch
                End Try

                If (_name Is Nothing) OrElse (_name.Trim() = String.Empty) Then
                    _name = myAssembly.GetName().Name
                End If
            End If
            Return _name
        End Get
    End Property


    Private ReadOnly Property ShortcutDescription() As String
        Get
            If _description Is Nothing Then
                Dim myAssembly As Assembly = Assembly.GetExecutingAssembly()

                Try
                    Dim descriptionAttribute As Object = myAssembly.GetCustomAttributes(GetType(AssemblyDescriptionAttribute), False)(0)
                    _description = DirectCast(descriptionAttribute, AssemblyDescriptionAttribute).Description
                Catch
                End Try

                If (_description Is Nothing) OrElse (_description.Trim() = String.Empty) Then
                    _description = "Launch " + ShortcutName
                End If
            End If
            Return _description
        End Get
    End Property


#End Region

#Region "Private Helper Methods"

    Private Sub CreateShortcut(folder As String, name As String, target As String, description As String)
        Dim shortcutFullName As String = Path.Combine(folder, name & Convert.ToString(".lnk"))

        Try
            Dim shell As WshShell = New WshShell()
            Dim link As IWshShortcut = DirectCast(shell.CreateShortcut(shortcutFullName), IWshShortcut)
            link.TargetPath = target
            link.Description = description
            link.Save()
        Catch ex As Exception
            MessageBox.Show(String.Format("The shortcut ""{0}"" could not be created." & vbLf & vbLf & "{1}", shortcutFullName, ex.ToString()), "Create Shortcut", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub


    Private Sub DeleteShortcuts()

        'Dim file As System.IO.StreamWriter
        'file = My.Computer.FileSystem.OpenTextFileWriter("D:\test.txt", True)
        'file.WriteLine("DELETE SHORTCUTS 1 ")
        'file.Close()

        ' Just try and delete all possible shortcuts that may have been
        ' created during install

        Try
            ' This is in a Try block in case AllUsersDesktop is not supported
            Dim allUsersDesktop As Object = "AllUsersDesktop"
            Dim shell As WshShell = New WshShell()
            Dim desktopFolder As String = shell.SpecialFolders.Item(allUsersDesktop).ToString()
            DeleteShortcut(desktopFolder, ShortcutName)
        Catch
        End Try

        Try
            DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), ShortcutName)
        Catch ex As Exception

        End Try

        Try
            DeleteShortcut(QuickLaunchFolder, ShortcutName)
        Catch ex As Exception

        End Try




    End Sub


    Private Sub DeleteShortcut(folder As String, name As String)
        Dim shortcutFullName As String = Path.Combine(folder, name & Convert.ToString(".lnk"))
        Dim shortcut As New FileInfo(shortcutFullName)
        If shortcut.Exists Then
            Try
                shortcut.Delete()
            Catch ex As Exception
                MessageBox.Show(String.Format("The shortcut ""{0}"" could not be deleted." & vbLf & vbLf & "{1}", shortcutFullName, ex.ToString()), "Delete Shortcut", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Try
        End If
    End Sub


#End Region

End Class
