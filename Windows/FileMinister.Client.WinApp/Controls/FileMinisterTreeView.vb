Public Class FileMinisterTreeView
    Inherits TreeView
    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        'MessageHelper.SetWindowTheme(Me.Handle, "explorer", Nothing)
    End Sub
End Class



