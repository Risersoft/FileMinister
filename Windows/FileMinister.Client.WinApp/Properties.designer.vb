<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Properties
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.btnApply = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tbCtrlProperties = New System.Windows.Forms.TabControl()
        Me.tbPagePermissions = New System.Windows.Forms.TabPage()
        Me.dgvPermissions = New System.Windows.Forms.DataGridView()
        Me.lblPermissions = New System.Windows.Forms.Label()
        Me.btnRemoveUser = New System.Windows.Forms.Button()
        Me.btnAddUser = New System.Windows.Forms.Button()
        Me.lblGroups = New System.Windows.Forms.Label()
        Me.lstGroupUsers = New System.Windows.Forms.ListBox()
        Me.tbPageMetadata = New System.Windows.Forms.TabPage()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.dgvMetadata = New System.Windows.Forms.DataGridView()
        Me.mnuContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteRowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tbCtrlProperties.SuspendLayout()
        Me.tbPagePermissions.SuspendLayout()
        CType(Me.dgvPermissions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbPageMetadata.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.dgvMetadata, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mnuContextMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnApply
        '
        Me.btnApply.Location = New System.Drawing.Point(280, 446)
        Me.btnApply.Name = "btnApply"
        Me.btnApply.Size = New System.Drawing.Size(75, 23)
        Me.btnApply.TabIndex = 6
        Me.btnApply.Text = "OK"
        Me.btnApply.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(199, 446)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Tag = ""
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'tbCtrlProperties
        '
        Me.tbCtrlProperties.Controls.Add(Me.tbPagePermissions)
        Me.tbCtrlProperties.Controls.Add(Me.tbPageMetadata)
        Me.tbCtrlProperties.Location = New System.Drawing.Point(12, 9)
        Me.tbCtrlProperties.Name = "tbCtrlProperties"
        Me.tbCtrlProperties.SelectedIndex = 0
        Me.tbCtrlProperties.Size = New System.Drawing.Size(343, 431)
        Me.tbCtrlProperties.TabIndex = 0
        '
        'tbPagePermissions
        '
        Me.tbPagePermissions.Controls.Add(Me.dgvPermissions)
        Me.tbPagePermissions.Controls.Add(Me.lblPermissions)
        Me.tbPagePermissions.Controls.Add(Me.btnRemoveUser)
        Me.tbPagePermissions.Controls.Add(Me.btnAddUser)
        Me.tbPagePermissions.Controls.Add(Me.lblGroups)
        Me.tbPagePermissions.Controls.Add(Me.lstGroupUsers)
        Me.tbPagePermissions.Location = New System.Drawing.Point(4, 22)
        Me.tbPagePermissions.Name = "tbPagePermissions"
        Me.tbPagePermissions.Padding = New System.Windows.Forms.Padding(3)
        Me.tbPagePermissions.Size = New System.Drawing.Size(335, 405)
        Me.tbPagePermissions.TabIndex = 1
        Me.tbPagePermissions.Text = "Permissions"
        Me.tbPagePermissions.UseVisualStyleBackColor = True
        '
        'dgvPermissions
        '
        Me.dgvPermissions.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvPermissions.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.dgvPermissions.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvPermissions.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvPermissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPermissions.GridColor = System.Drawing.SystemColors.Window
        Me.dgvPermissions.Location = New System.Drawing.Point(10, 180)
        Me.dgvPermissions.Name = "dgvPermissions"
        Me.dgvPermissions.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvPermissions.RowHeadersVisible = False
        Me.dgvPermissions.Size = New System.Drawing.Size(307, 169)
        Me.dgvPermissions.TabIndex = 8
        '
        'lblPermissions
        '
        Me.lblPermissions.AutoSize = True
        Me.lblPermissions.Location = New System.Drawing.Point(7, 201)
        Me.lblPermissions.Name = "lblPermissions"
        Me.lblPermissions.Size = New System.Drawing.Size(62, 13)
        Me.lblPermissions.TabIndex = 5
        Me.lblPermissions.Text = "Permissions"
        '
        'btnRemoveUser
        '
        Me.btnRemoveUser.Location = New System.Drawing.Point(242, 130)
        Me.btnRemoveUser.Name = "btnRemoveUser"
        Me.btnRemoveUser.Size = New System.Drawing.Size(75, 23)
        Me.btnRemoveUser.TabIndex = 3
        Me.btnRemoveUser.Text = "Remove"
        Me.btnRemoveUser.UseVisualStyleBackColor = True
        '
        'btnAddUser
        '
        Me.btnAddUser.Location = New System.Drawing.Point(161, 130)
        Me.btnAddUser.Name = "btnAddUser"
        Me.btnAddUser.Size = New System.Drawing.Size(75, 23)
        Me.btnAddUser.TabIndex = 2
        Me.btnAddUser.Text = "Add..."
        Me.btnAddUser.UseVisualStyleBackColor = True
        '
        'lblGroups
        '
        Me.lblGroups.AutoSize = True
        Me.lblGroups.Location = New System.Drawing.Point(7, 12)
        Me.lblGroups.Name = "lblGroups"
        Me.lblGroups.Size = New System.Drawing.Size(105, 13)
        Me.lblGroups.TabIndex = 1
        Me.lblGroups.Text = "Group or user names"
        '
        'lstGroupUsers
        '
        Me.lstGroupUsers.FormattingEnabled = True
        Me.lstGroupUsers.Location = New System.Drawing.Point(10, 28)
        Me.lstGroupUsers.Name = "lstGroupUsers"
        Me.lstGroupUsers.Size = New System.Drawing.Size(307, 95)
        Me.lstGroupUsers.TabIndex = 0
        '
        'tbPageMetadata
        '
        Me.tbPageMetadata.Controls.Add(Me.Panel1)
        Me.tbPageMetadata.Location = New System.Drawing.Point(4, 22)
        Me.tbPageMetadata.Name = "tbPageMetadata"
        Me.tbPageMetadata.Padding = New System.Windows.Forms.Padding(3)
        Me.tbPageMetadata.Size = New System.Drawing.Size(335, 405)
        Me.tbPageMetadata.TabIndex = 0
        Me.tbPageMetadata.Text = "Metadata"
        Me.tbPageMetadata.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.dgvMetadata)
        Me.Panel1.Location = New System.Drawing.Point(6, 18)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(323, 381)
        Me.Panel1.TabIndex = 0
        '
        'dgvMetadata
        '
        Me.dgvMetadata.AllowUserToResizeRows = False
        Me.dgvMetadata.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMetadata.BackgroundColor = System.Drawing.SystemColors.Control
        Me.dgvMetadata.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvMetadata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMetadata.Location = New System.Drawing.Point(3, 3)
        Me.dgvMetadata.Name = "dgvMetadata"
        Me.dgvMetadata.RowHeadersWidth = 25
        Me.dgvMetadata.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvMetadata.Size = New System.Drawing.Size(317, 375)
        Me.dgvMetadata.TabIndex = 0
        '
        'mnuContextMenu
        '
        Me.mnuContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteRowToolStripMenuItem})
        Me.mnuContextMenu.Name = "ContextMenuStrip"
        Me.mnuContextMenu.Size = New System.Drawing.Size(134, 26)
        '
        'DeleteRowToolStripMenuItem
        '
        Me.DeleteRowToolStripMenuItem.Name = "DeleteRowToolStripMenuItem"
        Me.DeleteRowToolStripMenuItem.Size = New System.Drawing.Size(133, 22)
        Me.DeleteRowToolStripMenuItem.Text = "Delete Row"
        '
        'Properties
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(367, 479)
        Me.Controls.Add(Me.tbCtrlProperties)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Properties"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Properties"
        Me.tbCtrlProperties.ResumeLayout(False)
        Me.tbPagePermissions.ResumeLayout(False)
        Me.tbPagePermissions.PerformLayout()
        CType(Me.dgvPermissions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbPageMetadata.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.dgvMetadata, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mnuContextMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents btnApply As System.Windows.Forms.Button
    Private WithEvents btnCancel As System.Windows.Forms.Button
    Private WithEvents tbCtrlProperties As System.Windows.Forms.TabControl
    Private WithEvents tbPageMetadata As System.Windows.Forms.TabPage
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents dgvMetadata As System.Windows.Forms.DataGridView
    Friend WithEvents tbPagePermissions As System.Windows.Forms.TabPage
    Friend WithEvents lstGroupUsers As System.Windows.Forms.ListBox
    Friend WithEvents lblGroups As System.Windows.Forms.Label
    Friend WithEvents btnRemoveUser As System.Windows.Forms.Button
    Friend WithEvents btnAddUser As System.Windows.Forms.Button
    Friend WithEvents lblPermissions As System.Windows.Forms.Label
    Friend WithEvents mnuContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents DeleteRowToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents dgvPermissions As System.Windows.Forms.DataGridView
End Class
