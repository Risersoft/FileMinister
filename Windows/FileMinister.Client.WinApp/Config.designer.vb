<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Config
    Inherits frmMaxApi
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Config))
        Me.fbDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.epWarning = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.epTick = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.ttDelete = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.epWarning, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.epTick, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'epWarning
        '
        Me.epWarning.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epWarning.ContainerControl = Me
        '
        'epTick
        '
        Me.epTick.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epTick.ContainerControl = Me
        Me.epTick.Icon = CType(resources.GetObject("epTick.Icon"), System.Drawing.Icon)
        '
        'Config
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(662, 298)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MinimizeBox = False
        Me.Name = "Config"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Cloud Sync Config"
        CType(Me.epWarning, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.epTick, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents fbDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents epWarning As System.Windows.Forms.ErrorProvider
    Friend WithEvents epTick As System.Windows.Forms.ErrorProvider
    Friend WithEvents ttDelete As System.Windows.Forms.ToolTip
End Class
