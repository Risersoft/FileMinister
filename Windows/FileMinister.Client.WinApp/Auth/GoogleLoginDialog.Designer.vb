<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GoogleLoginDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GoogleBrowser = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'GoogleBrowser
        '
        Me.GoogleBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GoogleBrowser.Location = New System.Drawing.Point(0, 0)
        Me.GoogleBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.GoogleBrowser.Name = "GoogleBrowser"
        Me.GoogleBrowser.ScriptErrorsSuppressed = True
        Me.GoogleBrowser.Size = New System.Drawing.Size(784, 356)
        Me.GoogleBrowser.TabIndex = 1
        '
        'GoogleLoginDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 356)
        Me.Controls.Add(Me.GoogleBrowser)
        Me.Name = "GoogleLoginDialog"
        Me.Text = "GoogleLoginDialog"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GoogleBrowser As System.Windows.Forms.WebBrowser
End Class
