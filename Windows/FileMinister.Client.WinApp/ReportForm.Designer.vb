<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReportForm
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
        Me.DataTable1BindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CheckinDataSet = New FileMinister.Client.WinApp.CheckinDataSet()
        Me.CheckinTableBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Users = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.FromTime = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ToTime = New System.Windows.Forms.DateTimePicker()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Shares = New System.Windows.Forms.ComboBox()
        Me.Filterbtn = New System.Windows.Forms.Button()
        Me.ReportTypes = New System.Windows.Forms.ComboBox()
        Me.PanelReport = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.PanelFilter = New System.Windows.Forms.Panel()
        Me.EmailReport_btn = New System.Windows.Forms.Button()
        CType(Me.DataTable1BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CheckinDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CheckinTableBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFilter.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataTable1BindingSource
        '
        Me.DataTable1BindingSource.DataMember = "DataTable1"
        Me.DataTable1BindingSource.DataSource = Me.CheckinDataSet
        '
        'CheckinDataSet
        '
        Me.CheckinDataSet.DataSetName = "CheckinDataSet"
        Me.CheckinDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(21, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(72, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Select Report"
        '
        'Users
        '
        Me.Users.FormattingEnabled = True
        Me.Users.Location = New System.Drawing.Point(99, 51)
        Me.Users.Name = "Users"
        Me.Users.Size = New System.Drawing.Size(190, 21)
        Me.Users.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(21, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "User"
        '
        'FromTime
        '
        Me.FromTime.Location = New System.Drawing.Point(99, 89)
        Me.FromTime.Name = "FromTime"
        Me.FromTime.Size = New System.Drawing.Size(189, 20)
        Me.FromTime.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(21, 89)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "From"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(349, 89)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(20, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "To"
        '
        'ToTime
        '
        Me.ToTime.Location = New System.Drawing.Point(420, 83)
        Me.ToTime.Name = "ToTime"
        Me.ToTime.Size = New System.Drawing.Size(190, 20)
        Me.ToTime.TabIndex = 7
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(349, 48)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(35, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Share"
        '
        'Shares
        '
        Me.Shares.FormattingEnabled = True
        Me.Shares.Location = New System.Drawing.Point(420, 48)
        Me.Shares.Name = "Shares"
        Me.Shares.Size = New System.Drawing.Size(189, 21)
        Me.Shares.TabIndex = 9
        '
        'Filterbtn
        '
        Me.Filterbtn.Location = New System.Drawing.Point(543, 122)
        Me.Filterbtn.Name = "Filterbtn"
        Me.Filterbtn.Size = New System.Drawing.Size(66, 23)
        Me.Filterbtn.TabIndex = 10
        Me.Filterbtn.Text = "Apply"
        Me.Filterbtn.UseVisualStyleBackColor = True
        '
        'ReportTypes
        '
        Me.ReportTypes.FormattingEnabled = True
        Me.ReportTypes.Location = New System.Drawing.Point(99, 15)
        Me.ReportTypes.Name = "ReportTypes"
        Me.ReportTypes.Size = New System.Drawing.Size(190, 21)
        Me.ReportTypes.TabIndex = 18
        '
        'PanelReport
        '
        Me.PanelReport.AutoScroll = True
        Me.PanelReport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.PanelReport.Location = New System.Drawing.Point(12, 181)
        Me.PanelReport.Name = "PanelReport"
        Me.PanelReport.Size = New System.Drawing.Size(860, 261)
        Me.PanelReport.TabIndex = 19
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(444, 122)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 20
        Me.btnClose.Text = "Cancel"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'PanelFilter
        '
        Me.PanelFilter.Controls.Add(Me.EmailReport_btn)
        Me.PanelFilter.Controls.Add(Me.Label1)
        Me.PanelFilter.Controls.Add(Me.btnClose)
        Me.PanelFilter.Controls.Add(Me.Filterbtn)
        Me.PanelFilter.Controls.Add(Me.ReportTypes)
        Me.PanelFilter.Controls.Add(Me.Label5)
        Me.PanelFilter.Controls.Add(Me.Label2)
        Me.PanelFilter.Controls.Add(Me.Users)
        Me.PanelFilter.Controls.Add(Me.FromTime)
        Me.PanelFilter.Controls.Add(Me.Label3)
        Me.PanelFilter.Controls.Add(Me.ToTime)
        Me.PanelFilter.Controls.Add(Me.Shares)
        Me.PanelFilter.Controls.Add(Me.Label4)
        Me.PanelFilter.Location = New System.Drawing.Point(12, 12)
        Me.PanelFilter.Name = "PanelFilter"
        Me.PanelFilter.Size = New System.Drawing.Size(860, 163)
        Me.PanelFilter.TabIndex = 21
        '
        'EmailReport_btn
        '
        Me.EmailReport_btn.Location = New System.Drawing.Point(747, 121)
        Me.EmailReport_btn.Name = "EmailReport_btn"
        Me.EmailReport_btn.Size = New System.Drawing.Size(75, 23)
        Me.EmailReport_btn.TabIndex = 21
        Me.EmailReport_btn.Text = "Email"
        Me.EmailReport_btn.UseVisualStyleBackColor = True
        '
        'ReportForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(882, 505)
        Me.Controls.Add(Me.PanelFilter)
        Me.Controls.Add(Me.PanelReport)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ReportForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Report"
        CType(Me.DataTable1BindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CheckinDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CheckinTableBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFilter.ResumeLayout(False)
        Me.PanelFilter.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CheckinTableBindingSource As System.Windows.Forms.BindingSource
    'Friend WithEvents DataSetCheckin As FileMinister.Client.WinApp.DataSetCheckin
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Users As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents FromTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ToTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Shares As System.Windows.Forms.ComboBox
    Friend WithEvents Filterbtn As System.Windows.Forms.Button
    Friend WithEvents DataTable1BindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents CheckinDataSet As FileMinister.Client.WinApp.CheckinDataSet
    Friend WithEvents ReportTypes As System.Windows.Forms.ComboBox
    Friend WithEvents PanelReport As System.Windows.Forms.Panel
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents PanelFilter As System.Windows.Forms.Panel
    Friend WithEvents EmailReport_btn As System.Windows.Forms.Button
End Class
