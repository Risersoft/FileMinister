<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LinkedInLoginDialog
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
        Me.linkedInBrowser = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'linkedInBrowser
        '
        Me.linkedInBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linkedInBrowser.Location = New System.Drawing.Point(0, 0)
        Me.linkedInBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.linkedInBrowser.Name = "linkedInBrowser"
        Me.linkedInBrowser.ScriptErrorsSuppressed = True
        Me.linkedInBrowser.Size = New System.Drawing.Size(584, 261)
        Me.linkedInBrowser.TabIndex = 0
        '
        'LinkedInLoginDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 261)
        Me.Controls.Add(Me.linkedInBrowser)
        Me.Name = "LinkedInLoginDialog"
        Me.Text = "LinkedIn Login"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents linkedInBrowser As System.Windows.Forms.WebBrowser
End Class
