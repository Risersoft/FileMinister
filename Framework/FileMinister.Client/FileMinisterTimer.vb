Imports System.Timers
Imports risersoft.shared.portable.Model

Public Class FileMinisterTimer
    Inherits Timer

    Public Property ShareInfo As LocalConfigInfo

    Public Property IsStarted As Boolean

    Public Sub New(share As LocalConfigInfo)
        Me.New(30 * 1000, share)
    End Sub

    Public Sub New(interval As Double, share As LocalConfigInfo)
        MyBase.New(interval)
        ShareInfo = share
    End Sub
End Class
