Imports risersoft.app.shared

Public Class frmAboutBox
    Inherits frmAboutRSBase

    Protected Overrides Sub AboutBox1_Load(sender As Object, e As System.EventArgs)
        MyBase.AboutBox1_Load(sender, e)
        Me.PictureBox1.Image = My.Resources.logo_simpleedms
        Me.PictureBox2.Image = Me.RiserLogo
    End Sub

    Private Sub InitializeComponent()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'frmAboutBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.ClientSize = New System.Drawing.Size(647, 433)
        Me.Name = "frmAboutBox"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class