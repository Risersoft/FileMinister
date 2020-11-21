<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NewFolderForm
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
        Me.components = New System.ComponentModel.Container()
        Me.lblNewFolder = New System.Windows.Forms.Label()
        Me.txtfolderName = New System.Windows.Forms.TextBox()
        Me.btnNewFolderCreate = New System.Windows.Forms.Button()
        Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblNewFolder
        '
        Me.lblNewFolder.AutoSize = True
        Me.lblNewFolder.Location = New System.Drawing.Point(22, 26)
        Me.lblNewFolder.Name = "lblNewFolder"
        Me.lblNewFolder.Size = New System.Drawing.Size(35, 13)
        Me.lblNewFolder.TabIndex = 0
        Me.lblNewFolder.Text = "Name"
        '
        'txtfolderName
        '
        Me.txtfolderName.Location = New System.Drawing.Point(91, 23)
        Me.txtfolderName.Name = "txtfolderName"
        Me.txtfolderName.Size = New System.Drawing.Size(175, 20)
        Me.txtfolderName.TabIndex = 1
        '
        'btnNewFolderCreate
        '
        Me.btnNewFolderCreate.Location = New System.Drawing.Point(314, 23)
        Me.btnNewFolderCreate.Name = "btnNewFolderCreate"
        Me.btnNewFolderCreate.Size = New System.Drawing.Size(75, 23)
        Me.btnNewFolderCreate.TabIndex = 2
        Me.btnNewFolderCreate.Text = "Create"
        Me.btnNewFolderCreate.UseVisualStyleBackColor = True
        '
        'ErrorProvider1
        '
        Me.ErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.ErrorProvider1.ContainerControl = Me
        '
        'NewFolderForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(440, 65)
        Me.Controls.Add(Me.btnNewFolderCreate)
        Me.Controls.Add(Me.txtfolderName)
        Me.Controls.Add(Me.lblNewFolder)
        Me.Name = "NewFolderForm"
        Me.Text = "New Folder"
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblNewFolder As System.Windows.Forms.Label
    Friend WithEvents txtfolderName As System.Windows.Forms.TextBox
    Friend WithEvents btnNewFolderCreate As System.Windows.Forms.Button
    Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
End Class
