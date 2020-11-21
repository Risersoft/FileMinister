<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SelectUser
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
        Me.rtbSelectUser = New System.Windows.Forms.RichTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnCheckNames = New System.Windows.Forms.Button()
        Me.lblType = New System.Windows.Forms.Label()
        Me.dgvUG = New System.Windows.Forms.DataGridView()
        Me.lblName = New System.Windows.Forms.Label()
        CType(Me.dgvUG, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rtbSelectUser
        '
        Me.rtbSelectUser.Location = New System.Drawing.Point(12, 30)
        Me.rtbSelectUser.Name = "rtbSelectUser"
        Me.rtbSelectUser.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.rtbSelectUser.Size = New System.Drawing.Size(229, 66)
        Me.rtbSelectUser.TabIndex = 0
        Me.rtbSelectUser.Text = ""
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(162, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Enter the object names to select:"
        '
        'btnOK
        '
        Me.btnOK.Enabled = False
        Me.btnOK.Location = New System.Drawing.Point(180, 359)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(261, 359)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnCheckNames
        '
        Me.btnCheckNames.Enabled = False
        Me.btnCheckNames.Location = New System.Drawing.Point(247, 28)
        Me.btnCheckNames.Name = "btnCheckNames"
        Me.btnCheckNames.Size = New System.Drawing.Size(89, 23)
        Me.btnCheckNames.TabIndex = 4
        Me.btnCheckNames.Text = "Check Names"
        Me.btnCheckNames.UseVisualStyleBackColor = True
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(167, 115)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(31, 13)
        Me.lblType.TabIndex = 13
        Me.lblType.Text = "Type"
        '
        'dgvUG
        '
        Me.dgvUG.AllowUserToAddRows = False
        Me.dgvUG.AllowUserToDeleteRows = False
        Me.dgvUG.AllowUserToResizeColumns = False
        Me.dgvUG.AllowUserToResizeRows = False
        Me.dgvUG.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight
        Me.dgvUG.CausesValidation = False
        Me.dgvUG.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.dgvUG.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvUG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvUG.ColumnHeadersVisible = False
        Me.dgvUG.EnableHeadersVisualStyles = False
        Me.dgvUG.Location = New System.Drawing.Point(12, 131)
        Me.dgvUG.MultiSelect = False
        Me.dgvUG.Name = "dgvUG"
        Me.dgvUG.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvUG.RowHeadersVisible = False
        Me.dgvUG.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgvUG.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.dgvUG.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvUG.ShowCellToolTips = False
        Me.dgvUG.ShowEditingIcon = False
        Me.dgvUG.ShowRowErrors = False
        Me.dgvUG.Size = New System.Drawing.Size(229, 212)
        Me.dgvUG.TabIndex = 12
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(12, 115)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(35, 13)
        Me.lblName.TabIndex = 11
        Me.lblName.Text = "Name"
        '
        'SelectUser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(344, 394)
        Me.Controls.Add(Me.lblType)
        Me.Controls.Add(Me.dgvUG)
        Me.Controls.Add(Me.lblName)
        Me.Controls.Add(Me.btnCheckNames)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.rtbSelectUser)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SelectUser"
        Me.ShowInTaskbar = False
        Me.Text = "Select Users or Groups"
        CType(Me.dgvUG, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rtbSelectUser As System.Windows.Forms.RichTextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnCheckNames As System.Windows.Forms.Button
    Friend WithEvents lblType As System.Windows.Forms.Label
    Friend WithEvents dgvUG As System.Windows.Forms.DataGridView
    Friend WithEvents lblName As System.Windows.Forms.Label
End Class
