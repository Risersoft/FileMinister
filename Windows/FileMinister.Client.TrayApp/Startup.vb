Module Startup
    Public Sub Main()

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Dim frm As New Form1
        frm.ShowInTaskbar = False
        'frm.Size = New Size(0, 0)
        frm.Opacity = 0
        Application.Run(frm)

    End Sub
End Module
