Imports System.Runtime.InteropServices
Imports System.Security.Permissions

Public Class FileMinisterWebBrowser
    Inherits WebBrowser

    Private cookie As AxHost.ConnectionPointCookie
    Private helper As FileMinisterWebBrowserEventHelper

    <PermissionSetAttribute(SecurityAction.LinkDemand,
    Name:="FullTrust")> Protected Overrides Sub CreateSink()

        MyBase.CreateSink()

        ' Create an instance of the client that will handle the event
        ' and associate it with the underlying ActiveX control.
        helper = New FileMinisterWebBrowserEventHelper(Me)
        cookie = New AxHost.ConnectionPointCookie(
            Me.ActiveXInstance, helper, GetType(DWebBrowserEvents2))
    End Sub

    <PermissionSetAttribute(SecurityAction.LinkDemand,
    Name:="FullTrust")> Protected Overrides Sub DetachSink()

        ' Disconnect the client that handles the event
        ' from the underlying ActiveX control.
        If cookie IsNot Nothing Then
            cookie.Disconnect()
            cookie = Nothing
        End If
        MyBase.DetachSink()

    End Sub

    Public Event NavigateError As WebBrowserNavigateErrorEventHandler

    ' Raises the NavigateError event.
    Protected Overridable Sub OnNavigateError(
        ByVal e As WebBrowserNavigateErrorEventArgs)

        RaiseEvent NavigateError(Me, e)

    End Sub

    ' Handles the NavigateError event from the underlying ActiveX 
    ' control by raising the NavigateError event defined in this class.
    Private Class FileMinisterWebBrowserEventHelper
        Inherits StandardOleMarshalObject
        Implements DWebBrowserEvents2

        Private parent As FileMinisterWebBrowser

        Public Sub New(ByVal parent As FileMinisterWebBrowser)
            Me.parent = parent
        End Sub

        Public Sub NavigateError(ByVal pDisp As Object,
            ByRef URL As Object, ByRef frame As Object,
            ByRef statusCode As Object, ByRef cancel As Boolean) _
            Implements DWebBrowserEvents2.NavigateError

            ' Raise the NavigateError event.
            Me.parent.OnNavigateError(
                New WebBrowserNavigateErrorEventArgs(
                CStr(URL), CStr(frame), CInt(statusCode), cancel))

        End Sub
    End Class
End Class

' Represents the method that will handle the WebBrowser2.NavigateError event.
Public Delegate Sub WebBrowserNavigateErrorEventHandler(ByVal sender As Object,
        ByVal e As WebBrowserNavigateErrorEventArgs)

' Provides data for the WebBrowser2.NavigateError event.
Public Class WebBrowserNavigateErrorEventArgs
    Inherits EventArgs

    Private urlValue As String
    Private frameValue As String
    Private statusCodeValue As Int32
    Private cancelValue As Boolean

    Public Sub New(
            ByVal url As String, ByVal frame As String,
            ByVal statusCode As Int32, ByVal cancel As Boolean)

        Me.urlValue = url
        Me.frameValue = frame
        Me.statusCodeValue = statusCode
        Me.cancelValue = cancel

    End Sub

    Public Property Url() As String
        Get
            Return urlValue
        End Get
        Set(ByVal value As String)
            urlValue = value
        End Set
    End Property

    Public Property Frame() As String
        Get
            Return frameValue
        End Get
        Set(ByVal value As String)
            frameValue = value
        End Set
    End Property

    Public Property StatusCode() As Int32
        Get
            Return statusCodeValue
        End Get
        Set(ByVal value As Int32)
            statusCodeValue = value
        End Set
    End Property

    Public Property Cancel() As Boolean
        Get
            Return cancelValue
        End Get
        Set(ByVal value As Boolean)
            cancelValue = value
        End Set
    End Property

End Class

' Imports the NavigateError method from the OLE DWebBrowserEvents2 
' interface. 
<ComImport(), Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    TypeLibType(TypeLibTypeFlags.FHidden)>
Public Interface DWebBrowserEvents2

    <DispId(271)> Sub NavigateError(
            <InAttribute(), MarshalAs(UnmanagedType.IDispatch)>
            ByVal pDisp As Object,
            <InAttribute()> ByRef URL As Object,
            <InAttribute()> ByRef frame As Object,
            <InAttribute()> ByRef statusCode As Object,
            <InAttribute(), OutAttribute()> ByRef cancel As Boolean)

End Interface
