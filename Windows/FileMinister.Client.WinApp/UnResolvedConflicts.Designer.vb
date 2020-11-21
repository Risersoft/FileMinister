<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UnResolvedConflicts
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
        Me.dgvConflicts = New System.Windows.Forms.DataGridView()
        Me.cmsMain = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        CType(Me.dgvConflicts, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvConflicts
        '
        Me.dgvConflicts.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvConflicts.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvConflicts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvConflicts.GridColor = System.Drawing.SystemColors.Window
        Me.dgvConflicts.Location = New System.Drawing.Point(1, 32)
        Me.dgvConflicts.MultiSelect = False
        Me.dgvConflicts.Name = "dgvConflicts"
        Me.dgvConflicts.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvConflicts.RowHeadersVisible = False
        Me.dgvConflicts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvConflicts.Size = New System.Drawing.Size(555, 150)
        Me.dgvConflicts.TabIndex = 0
        '
        'cmsMain
        '
        Me.cmsMain.Name = "cmsMain"
        Me.cmsMain.Size = New System.Drawing.Size(61, 4)
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(1, 3)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(555, 23)
        Me.ProgressBar1.TabIndex = 17
        Me.ProgressBar1.Visible = False
        '
        'UnResolvedConflicts
        '
        Me.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(557, 186)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.dgvConflicts)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UnResolvedConflicts"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Other's UnResolved Conflicts"
        CType(Me.dgvConflicts, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvConflicts As System.Windows.Forms.DataGridView
    Friend WithEvents cmsMain As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
End Class
