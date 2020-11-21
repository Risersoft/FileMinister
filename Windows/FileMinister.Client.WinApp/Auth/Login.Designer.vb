<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Login
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
        Me.gpAuthorisation = New System.Windows.Forms.GroupBox()
        Me.pnlPassword = New System.Windows.Forms.Panel()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.pnlOTP = New System.Windows.Forms.Panel()
        Me.btnOtpVerify = New System.Windows.Forms.Button()
        Me.txtOTP = New System.Windows.Forms.TextBox()
        Me.lblOTP = New System.Windows.Forms.Label()
        Me.txtUserName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnAzureAd = New System.Windows.Forms.Button()
        Me.btnGoogle = New System.Windows.Forms.Button()
        Me.btnLinkedIn = New System.Windows.Forms.Button()
        Me.btnFaceBook = New System.Windows.Forms.Button()
        Me.pnlLogin = New System.Windows.Forms.Panel()
        Me.btnAuthCheck = New System.Windows.Forms.Button()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.btnLogin = New System.Windows.Forms.Button()
        Me.pnlAccount = New System.Windows.Forms.Panel()
        Me.btnAccountNext = New System.Windows.Forms.Button()
        Me.cbAccounts = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pnlAgentDetail = New System.Windows.Forms.Panel()
        Me.lbMACAddresses = New System.Windows.Forms.ListBox()
        Me.lblMACAddressesLabel = New System.Windows.Forms.Label()
        Me.txtSecretKey = New System.Windows.Forms.TextBox()
        Me.txtAgent = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnNextAgentDetail = New System.Windows.Forms.Button()
        Me.gpAuthorisation.SuspendLayout()
        Me.pnlPassword.SuspendLayout()
        Me.pnlOTP.SuspendLayout()
        Me.pnlLogin.SuspendLayout()
        Me.pnlAccount.SuspendLayout()
        Me.pnlAgentDetail.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpAuthorisation
        '
        Me.gpAuthorisation.Controls.Add(Me.pnlPassword)
        Me.gpAuthorisation.Controls.Add(Me.pnlOTP)
        Me.gpAuthorisation.Controls.Add(Me.txtUserName)
        Me.gpAuthorisation.Controls.Add(Me.Label1)
        Me.gpAuthorisation.Controls.Add(Me.btnAzureAd)
        Me.gpAuthorisation.Controls.Add(Me.btnGoogle)
        Me.gpAuthorisation.Controls.Add(Me.btnLinkedIn)
        Me.gpAuthorisation.Controls.Add(Me.btnFaceBook)
        Me.gpAuthorisation.Controls.Add(Me.pnlLogin)
        Me.gpAuthorisation.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpAuthorisation.Location = New System.Drawing.Point(10, 10)
        Me.gpAuthorisation.Name = "gpAuthorisation"
        Me.gpAuthorisation.Size = New System.Drawing.Size(428, 230)
        Me.gpAuthorisation.TabIndex = 8
        Me.gpAuthorisation.TabStop = False
        Me.gpAuthorisation.Text = "Authorisation"
        '
        'pnlPassword
        '
        Me.pnlPassword.Controls.Add(Me.txtPassword)
        Me.pnlPassword.Controls.Add(Me.lblPassword)
        Me.pnlPassword.Location = New System.Drawing.Point(77, 50)
        Me.pnlPassword.Name = "pnlPassword"
        Me.pnlPassword.Size = New System.Drawing.Size(340, 27)
        Me.pnlPassword.TabIndex = 17
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(90, 2)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(159, 20)
        Me.txtPassword.TabIndex = 4
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Location = New System.Drawing.Point(4, 6)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(61, 13)
        Me.lblPassword.TabIndex = 3
        Me.lblPassword.Text = "Password"
        '
        'pnlOTP
        '
        Me.pnlOTP.Controls.Add(Me.btnOtpVerify)
        Me.pnlOTP.Controls.Add(Me.txtOTP)
        Me.pnlOTP.Controls.Add(Me.lblOTP)
        Me.pnlOTP.Location = New System.Drawing.Point(77, 77)
        Me.pnlOTP.Name = "pnlOTP"
        Me.pnlOTP.Size = New System.Drawing.Size(340, 28)
        Me.pnlOTP.TabIndex = 16
        '
        'btnOtpVerify
        '
        Me.btnOtpVerify.Location = New System.Drawing.Point(255, 2)
        Me.btnOtpVerify.Name = "btnOtpVerify"
        Me.btnOtpVerify.Size = New System.Drawing.Size(75, 23)
        Me.btnOtpVerify.TabIndex = 14
        Me.btnOtpVerify.Text = "Verify"
        Me.btnOtpVerify.UseVisualStyleBackColor = True
        Me.btnOtpVerify.Visible = False
        '
        'txtOTP
        '
        Me.txtOTP.Location = New System.Drawing.Point(90, 4)
        Me.txtOTP.Name = "txtOTP"
        Me.txtOTP.Size = New System.Drawing.Size(159, 20)
        Me.txtOTP.TabIndex = 13
        '
        'lblOTP
        '
        Me.lblOTP.AutoSize = True
        Me.lblOTP.Location = New System.Drawing.Point(3, 7)
        Me.lblOTP.Name = "lblOTP"
        Me.lblOTP.Size = New System.Drawing.Size(32, 13)
        Me.lblOTP.TabIndex = 12
        Me.lblOTP.Text = "OTP"
        '
        'txtUserName
        '
        Me.txtUserName.Location = New System.Drawing.Point(167, 26)
        Me.txtUserName.Name = "txtUserName"
        Me.txtUserName.Size = New System.Drawing.Size(159, 20)
        Me.txtUserName.TabIndex = 15
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(78, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(69, 13)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "User Name"
        '
        'btnAzureAd
        '
        Me.btnAzureAd.Location = New System.Drawing.Point(338, 169)
        Me.btnAzureAd.Name = "btnAzureAd"
        Me.btnAzureAd.Size = New System.Drawing.Size(75, 23)
        Me.btnAzureAd.TabIndex = 10
        Me.btnAzureAd.Text = "Azure AD"
        Me.btnAzureAd.UseVisualStyleBackColor = True
        Me.btnAzureAd.Visible = False
        '
        'btnGoogle
        '
        Me.btnGoogle.Location = New System.Drawing.Point(257, 169)
        Me.btnGoogle.Name = "btnGoogle"
        Me.btnGoogle.Size = New System.Drawing.Size(75, 23)
        Me.btnGoogle.TabIndex = 9
        Me.btnGoogle.Text = "Google"
        Me.btnGoogle.UseVisualStyleBackColor = True
        Me.btnGoogle.Visible = False
        '
        'btnLinkedIn
        '
        Me.btnLinkedIn.Location = New System.Drawing.Point(172, 169)
        Me.btnLinkedIn.Name = "btnLinkedIn"
        Me.btnLinkedIn.Size = New System.Drawing.Size(75, 23)
        Me.btnLinkedIn.TabIndex = 8
        Me.btnLinkedIn.Text = "Linkedin"
        Me.btnLinkedIn.UseVisualStyleBackColor = True
        Me.btnLinkedIn.Visible = False
        '
        'btnFaceBook
        '
        Me.btnFaceBook.Location = New System.Drawing.Point(86, 169)
        Me.btnFaceBook.Name = "btnFaceBook"
        Me.btnFaceBook.Size = New System.Drawing.Size(75, 23)
        Me.btnFaceBook.TabIndex = 7
        Me.btnFaceBook.Text = "Facebook"
        Me.btnFaceBook.UseVisualStyleBackColor = True
        Me.btnFaceBook.Visible = False
        '
        'pnlLogin
        '
        Me.pnlLogin.BackColor = System.Drawing.SystemColors.ControlDark
        Me.pnlLogin.Controls.Add(Me.btnAuthCheck)
        Me.pnlLogin.Controls.Add(Me.btnEnd)
        Me.pnlLogin.Controls.Add(Me.btnLogin)
        Me.pnlLogin.Location = New System.Drawing.Point(62, 115)
        Me.pnlLogin.Name = "pnlLogin"
        Me.pnlLogin.Size = New System.Drawing.Size(299, 32)
        Me.pnlLogin.TabIndex = 6
        '
        'btnAuthCheck
        '
        Me.btnAuthCheck.Location = New System.Drawing.Point(24, 4)
        Me.btnAuthCheck.Name = "btnAuthCheck"
        Me.btnAuthCheck.Size = New System.Drawing.Size(75, 23)
        Me.btnAuthCheck.TabIndex = 7
        Me.btnAuthCheck.Text = "Next"
        Me.btnAuthCheck.UseVisualStyleBackColor = True
        '
        'btnEnd
        '
        Me.btnEnd.Location = New System.Drawing.Point(195, 3)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(75, 23)
        Me.btnEnd.TabIndex = 6
        Me.btnEnd.Text = "Cancel"
        Me.btnEnd.UseVisualStyleBackColor = True
        Me.btnEnd.Visible = False
        '
        'btnLogin
        '
        Me.btnLogin.Location = New System.Drawing.Point(110, 3)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(75, 23)
        Me.btnLogin.TabIndex = 5
        Me.btnLogin.Text = "Login"
        Me.btnLogin.UseVisualStyleBackColor = True
        Me.btnLogin.Visible = False
        '
        'pnlAccount
        '
        Me.pnlAccount.Controls.Add(Me.btnAccountNext)
        Me.pnlAccount.Controls.Add(Me.cbAccounts)
        Me.pnlAccount.Controls.Add(Me.Label2)
        Me.pnlAccount.Location = New System.Drawing.Point(12, 11)
        Me.pnlAccount.Name = "pnlAccount"
        Me.pnlAccount.Size = New System.Drawing.Size(442, 221)
        Me.pnlAccount.TabIndex = 13
        Me.pnlAccount.Visible = False
        '
        'btnAccountNext
        '
        Me.btnAccountNext.Location = New System.Drawing.Point(336, 180)
        Me.btnAccountNext.Name = "btnAccountNext"
        Me.btnAccountNext.Size = New System.Drawing.Size(75, 23)
        Me.btnAccountNext.TabIndex = 2
        Me.btnAccountNext.Text = "Next"
        Me.btnAccountNext.UseVisualStyleBackColor = True
        '
        'cbAccounts
        '
        Me.cbAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAccounts.FormattingEnabled = True
        Me.cbAccounts.Location = New System.Drawing.Point(111, 33)
        Me.cbAccounts.Name = "cbAccounts"
        Me.cbAccounts.Size = New System.Drawing.Size(240, 21)
        Me.cbAccounts.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(51, 36)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(47, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Account"
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
        'Login
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(466, 243)
        Me.Controls.Add(Me.gpAuthorisation)
        Me.Controls.Add(Me.pnlAccount)
        Me.Controls.Add(Me.pnlAgentDetail)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Login"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Login"
        Me.gpAuthorisation.ResumeLayout(False)
        Me.gpAuthorisation.PerformLayout()
        Me.pnlPassword.ResumeLayout(False)
        Me.pnlPassword.PerformLayout()
        Me.pnlOTP.ResumeLayout(False)
        Me.pnlOTP.PerformLayout()
        Me.pnlLogin.ResumeLayout(False)
        Me.pnlAccount.ResumeLayout(False)
        Me.pnlAccount.PerformLayout()
        Me.pnlAgentDetail.ResumeLayout(False)
        Me.pnlAgentDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpAuthorisation As System.Windows.Forms.GroupBox
    Friend WithEvents btnAzureAd As System.Windows.Forms.Button
    Friend WithEvents btnGoogle As System.Windows.Forms.Button
    Friend WithEvents btnLinkedIn As System.Windows.Forms.Button
    Friend WithEvents btnFaceBook As System.Windows.Forms.Button
    Friend WithEvents pnlLogin As System.Windows.Forms.Panel
    Friend WithEvents btnAuthCheck As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnLogin As System.Windows.Forms.Button
    Friend WithEvents pnlAccount As Panel
    Friend WithEvents btnAccountNext As Button
    Friend WithEvents cbAccounts As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents pnlAgentDetail As Panel
    Friend WithEvents btnNextAgentDetail As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtSecretKey As TextBox
    Friend WithEvents txtAgent As TextBox
    Friend WithEvents lblMACAddressesLabel As Label
    Friend WithEvents lbMACAddresses As ListBox
    Friend WithEvents pnlPassword As Panel
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents lblPassword As Label
    Friend WithEvents pnlOTP As Panel
    Friend WithEvents btnOtpVerify As Button
    Friend WithEvents txtOTP As TextBox
    Friend WithEvents lblOTP As Label
    Friend WithEvents txtUserName As TextBox
    Friend WithEvents Label1 As Label
End Class
