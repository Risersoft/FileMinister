<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ExternalLoginDialog
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
        Me.wbExternalLogin = New FileMinisterWebBrowser()
        Me.SuspendLayout()
        '
        'wbExternalLogin
        '
        Me.wbExternalLogin.Dock = System.Windows.Forms.DockStyle.Fill
        Me.wbExternalLogin.Location = New System.Drawing.Point(0, 0)
        Me.wbExternalLogin.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbExternalLogin.Name = "wbExternalLogin"
        Me.wbExternalLogin.ScriptErrorsSuppressed = True
        Me.wbExternalLogin.Size = New System.Drawing.Size(664, 659)
        Me.wbExternalLogin.TabIndex = 0
        '
        'ExternalLoginDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(664, 659)
        Me.Controls.Add(Me.wbExternalLogin)
        Me.Name = "ExternalLoginDialog"
        Me.Text = "Login"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents wbExternalLogin As FileMinisterWebBrowser
End Class
