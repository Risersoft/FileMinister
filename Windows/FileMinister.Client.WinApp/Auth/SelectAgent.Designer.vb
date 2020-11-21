<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SelectAgent
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlAgentDetail = New System.Windows.Forms.Panel()
        Me.lbMACAddresses = New System.Windows.Forms.ListBox()
        Me.lblMACAddressesLabel = New System.Windows.Forms.Label()
        Me.txtSecretKey = New System.Windows.Forms.TextBox()
        Me.txtAgent = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnNextAgentDetail = New System.Windows.Forms.Button()
        Me.pnlAgentDetail.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlAgentDetail
        '
        Me.pnlAgentDetail.Controls.Add(Me.lbMACAddresses)
        Me.pnlAgentDetail.Controls.Add(Me.lblMACAddressesLabel)
        Me.pnlAgentDetail.Controls.Add(Me.txtSecretKey)
        Me.pnlAgentDetail.Controls.Add(Me.txtAgent)
        Me.pnlAgentDetail.Controls.Add(Me.Label4)
        Me.pnlAgentDetail.Controls.Add(Me.Label3)
        Me.pnlAgentDetail.Controls.Add(Me.btnNextAgentDetail)
        Me.pnlAgentDetail.Location = New System.Drawing.Point(12, 11)
        Me.pnlAgentDetail.Name = "pnlAgentDetail"
        Me.pnlAgentDetail.Size = New System.Drawing.Size(442, 221)
        Me.pnlAgentDetail.TabIndex = 15
        Me.pnlAgentDetail.Visible = False
        '
        'lbMACAddresses
        '
        Me.lbMACAddresses.FormattingEnabled = True
        Me.lbMACAddresses.Location = New System.Drawing.Point(141, 103)
        Me.lbMACAddresses.Name = "lbMACAddresses"
        Me.lbMACAddresses.Size = New System.Drawing.Size(208, 56)
        Me.lbMACAddresses.TabIndex = 9
        '
        'lblMACAddressesLabel
        '
        Me.lblMACAddressesLabel.AutoSize = True
        Me.lblMACAddressesLabel.Location = New System.Drawing.Point(36, 103)
        Me.lblMACAddressesLabel.Name = "lblMACAddressesLabel"
        Me.lblMACAddressesLabel.Size = New System.Drawing.Size(82, 13)
        Me.lblMACAddressesLabel.TabIndex = 8
        Me.lblMACAddressesLabel.Text = "MAC Addresses"
        Me.lblMACAddressesLabel.Visible = False
        '
        'txtSecretKey
        '
        Me.txtSecretKey.Location = New System.Drawing.Point(141, 61)
        Me.txtSecretKey.MaxLength = 50
        Me.txtSecretKey.Name = "txtSecretKey"
        Me.txtSecretKey.Size = New System.Drawing.Size(208, 20)
        Me.txtSecretKey.TabIndex = 7
        '
        'txtAgent
        '
        Me.txtAgent.Location = New System.Drawing.Point(141, 22)
        Me.txtAgent.MaxLength = 50
        Me.txtAgent.Name = "txtAgent"
        Me.txtAgent.Size = New System.Drawing.Size(208, 20)
        Me.txtAgent.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(36, 64)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(59, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Secret Key"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(36, 25)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(35, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Agent"
        '
        'btnNextAgentDetail
        '
        Me.btnNextAgentDetail.Location = New System.Drawing.Point(349, 175)
        Me.btnNextAgentDetail.Name = "btnNextAgentDetail"
        Me.btnNextAgentDetail.Size = New System.Drawing.Size(75, 23)
        Me.btnNextAgentDetail.TabIndex = 3
        Me.btnNextAgentDetail.Text = "Next"
        Me.btnNextAgentDetail.UseVisualStyleBackColor = True
        '
        'SelectAgent
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(466, 243)
        Me.Controls.Add(Me.pnlAgentDetail)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SelectAgent"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Agent"
        Me.pnlAgentDetail.ResumeLayout(False)
        Me.pnlAgentDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlAgentDetail As Panel
    Friend WithEvents btnNextAgentDetail As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtSecretKey As TextBox
    Friend WithEvents txtAgent As TextBox
    Friend WithEvents lblMACAddressesLabel As Label
    Friend WithEvents lbMACAddresses As ListBox
End Class
