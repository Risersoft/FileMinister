Imports System.Timers
Imports risersoft.shared.portable.Model

Public Class TimerPrivider
    Private timers As New Dictionary(Of String, FileMinisterTimer)
    Public Shared Property Instance As New TimerPrivider()

    Public Event TimeElapsed As TimeElapsed

    Private Sub New()

    End Sub

    Public Sub ConfigureTimers(shares As IList(Of LocalConfigInfo))
        For Each share In shares
            Dim key As String = share.UserAccountId.ToString() + "::" + share.FileShareId.ToString
            If (Not String.IsNullOrWhiteSpace(share.User.AccessToken)) Then
                If Not timers.ContainsKey(key) Then
                    AddTimer(share)
                End If
            ElseIf timers.ContainsKey(key) Then
                Dim timer = timers(key)
                timer.Stop()
                timer.IsStarted = False
                timers.Remove(key)
            End If
        Next
    End Sub

    Private Sub AddTimer(share As LocalConfigInfo)
        Dim timer As New FileMinisterTimer(share)
        timers.Add(share.UserAccountId.ToString() + "::" + share.FileShareId.ToString, timer)

        AddHandler timer.Elapsed, AddressOf Elapsed

    End Sub

    Public Sub Start()
        For Each timer In timers.Values
            If Not timer.IsStarted Then
                timer.Start()
                timer.IsStarted = True
            End If
        Next
    End Sub

    Public Sub [Stop]()
        For Each timer In timers.Values
            timer.Stop()
            timer.IsStarted = False
        Next
    End Sub

    Private Sub Elapsed(sender As Object, e As ElapsedEventArgs)
        Dim reEnable = True
        Dim timer = CType(sender, FileMinisterTimer)
        Try
            timer.Enabled = False
            RaiseEvent TimeElapsed(timer)

        Catch ex As Exception
            If ("Share Missing".Equals(ex.Message)) Then
                reEnable = False
            End If
        Finally
            If reEnable Then
                timer.Enabled = True
            End If
        End Try
    End Sub
End Class

Public Delegate Sub TimeElapsed(timer As FileMinisterTimer)
