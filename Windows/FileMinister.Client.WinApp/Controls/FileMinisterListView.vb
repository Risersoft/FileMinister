Public Class FileMinisterListView
    Inherits ListView
    Const LVM_FIRST As Integer = &H1000
    Const LVM_SETICONSPACING As Integer = LVM_FIRST + 53

    Public Sub New()
        SetSpacing(100, 120)
        'this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

        Me.DoubleBuffered = True
    End Sub

    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        ' MessageHelper.SetWindowTheme(Me.Handle, "explorer", Nothing)
    End Sub

    Private Sub SetSpacing(x As Integer, y As Integer)
        'MessageHelper.SendMessage(Me.Handle, LVM_SETICONSPACING, DirectCast(0, IntPtr), DirectCast(x * 65536 + y, IntPtr))

    End Sub
End Class


