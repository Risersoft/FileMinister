Imports Infragistics.Win.UltraWinGrid
Imports System.Xml
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class frmFileShare
    Inherits frmMax

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Add any initialization after the InitializeComponent() call

        Me.initForm()


    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraTab15 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab7 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab6 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab5 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim Appearance15 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance16 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance17 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance18 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance19 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance20 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance21 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance22 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab4 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab3 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Me.UltraTabPageControl25 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraLabel4 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_ShareContainerName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraTabPageControl5 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.cmb_MaintainVersionHistory = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.UltraLabel7 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_maxVersionFileSizeMB = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel6 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_MaxVersionMonths = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel5 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_VersionDiffMinutes = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel3 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraTabPageControl7 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmb_BackupShareID = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.UltraGroupBox2 = New Infragistics.Win.Misc.UltraGroupBox()
        Me.cmb_IncrBackupPeriodType = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.txt_IncrBackupPeriodValue = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel10 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel9 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_BackupRootPathIncr = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraGroupBox1 = New Infragistics.Win.Misc.UltraGroupBox()
        Me.chk_UseSubDir = New Infragistics.Win.UltraWinEditors.UltraCheckEditor()
        Me.UltraLabel11 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_BackupRootPathFull = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.cmb_FullBackupPeriodType = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.txt_FullBackupPeriodValue = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel12 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraTabPageControl6 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.txt_EmailTo = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraLabel13 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel14 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_Emailcc = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraTabPageControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridDir = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.Panel4 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnEditDir = New Infragistics.Win.Misc.UltraButton()
        Me.btnDelDir = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddDir = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabPageControl2 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridAgent = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.UltraPanel2 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnDelAgent = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddAgent = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabPageControl4 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraTabControl5 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage5 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.UltraTabPageControl3 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGrid1 = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.UltraPanel3 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnEditWS = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddWS = New Infragistics.Win.Misc.UltraButton()
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.cmb_ShareType = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.cmb_IsBackup = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.UltraLabel8 = New Infragistics.Win.Misc.UltraLabel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_Description = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.txt_ShareName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.Panel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.btnRunFull = New Infragistics.Win.Misc.UltraButton()
        Me.btnRunIncr = New Infragistics.Win.Misc.UltraButton()
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl25.SuspendLayout()
        CType(Me.txt_ShareContainerName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl5.SuspendLayout()
        CType(Me.cmb_MaintainVersionHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_maxVersionFileSizeMB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_MaxVersionMonths, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_VersionDiffMinutes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl7.SuspendLayout()
        CType(Me.cmb_BackupShareID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraGroupBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraGroupBox2.SuspendLayout()
        CType(Me.cmb_IncrBackupPeriodType, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_IncrBackupPeriodValue, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_BackupRootPathIncr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraGroupBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraGroupBox1.SuspendLayout()
        CType(Me.chk_UseSubDir, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_BackupRootPathFull, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmb_FullBackupPeriodType, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_FullBackupPeriodValue, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl6.SuspendLayout()
        CType(Me.txt_EmailTo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_Emailcc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl1.SuspendLayout()
        CType(Me.UltraGridDir, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.ClientArea.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.UltraTabPageControl2.SuspendLayout()
        CType(Me.UltraGridAgent, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraPanel2.ClientArea.SuspendLayout()
        Me.UltraPanel2.SuspendLayout()
        Me.UltraTabPageControl4.SuspendLayout()
        CType(Me.UltraTabControl5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl5.SuspendLayout()
        Me.UltraTabPageControl3.SuspendLayout()
        CType(Me.UltraGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraPanel3.ClientArea.SuspendLayout()
        Me.UltraPanel3.SuspendLayout()
        Me.UltraPanel1.ClientArea.SuspendLayout()
        Me.UltraPanel1.SuspendLayout()
        CType(Me.cmb_ShareType, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmb_IsBackup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_Description, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_ShareName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.ClientArea.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'UltraTabPageControl25
        '
        Me.UltraTabPageControl25.Controls.Add(Me.UltraLabel4)
        Me.UltraTabPageControl25.Controls.Add(Me.txt_ShareContainerName)
        Me.UltraTabPageControl25.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl25.Name = "UltraTabPageControl25"
        Me.UltraTabPageControl25.Size = New System.Drawing.Size(516, 268)
        '
        'UltraLabel4
        '
        Appearance1.FontData.BoldAsString = "False"
        Appearance1.TextHAlignAsString = "Right"
        Appearance1.TextVAlignAsString = "Middle"
        Me.UltraLabel4.Appearance = Appearance1
        Me.UltraLabel4.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel4.Location = New System.Drawing.Point(42, 31)
        Me.UltraLabel4.Name = "UltraLabel4"
        Me.UltraLabel4.Size = New System.Drawing.Size(166, 16)
        Me.UltraLabel4.TabIndex = 2
        Me.UltraLabel4.Text = "Container Name"
        '
        'txt_ShareContainerName
        '
        Me.txt_ShareContainerName.Location = New System.Drawing.Point(214, 28)
        Me.txt_ShareContainerName.Name = "txt_ShareContainerName"
        Me.txt_ShareContainerName.Size = New System.Drawing.Size(160, 21)
        Me.txt_ShareContainerName.TabIndex = 3
        Me.txt_ShareContainerName.Text = "UltraTextEditor1"
        '
        'UltraTabPageControl5
        '
        Me.UltraTabPageControl5.Controls.Add(Me.cmb_MaintainVersionHistory)
        Me.UltraTabPageControl5.Controls.Add(Me.UltraLabel7)
        Me.UltraTabPageControl5.Controls.Add(Me.txt_maxVersionFileSizeMB)
        Me.UltraTabPageControl5.Controls.Add(Me.UltraLabel6)
        Me.UltraTabPageControl5.Controls.Add(Me.txt_MaxVersionMonths)
        Me.UltraTabPageControl5.Controls.Add(Me.UltraLabel5)
        Me.UltraTabPageControl5.Controls.Add(Me.txt_VersionDiffMinutes)
        Me.UltraTabPageControl5.Controls.Add(Me.UltraLabel3)
        Me.UltraTabPageControl5.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl5.Name = "UltraTabPageControl5"
        Me.UltraTabPageControl5.Size = New System.Drawing.Size(516, 268)
        '
        'cmb_MaintainVersionHistory
        '
        Me.cmb_MaintainVersionHistory.Location = New System.Drawing.Point(181, 26)
        Me.cmb_MaintainVersionHistory.Name = "cmb_MaintainVersionHistory"
        Me.cmb_MaintainVersionHistory.Size = New System.Drawing.Size(160, 21)
        Me.cmb_MaintainVersionHistory.TabIndex = 26
        Me.cmb_MaintainVersionHistory.Text = "UltraComboEditor1"
        '
        'UltraLabel7
        '
        Appearance2.FontData.BoldAsString = "False"
        Appearance2.TextHAlignAsString = "Right"
        Appearance2.TextVAlignAsString = "Middle"
        Me.UltraLabel7.Appearance = Appearance2
        Me.UltraLabel7.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel7.Location = New System.Drawing.Point(38, 119)
        Me.UltraLabel7.Name = "UltraLabel7"
        Me.UltraLabel7.Size = New System.Drawing.Size(137, 16)
        Me.UltraLabel7.TabIndex = 24
        Me.UltraLabel7.Text = "Max File Size MB"
        '
        'txt_maxVersionFileSizeMB
        '
        Me.txt_maxVersionFileSizeMB.Location = New System.Drawing.Point(181, 114)
        Me.txt_maxVersionFileSizeMB.Name = "txt_maxVersionFileSizeMB"
        Me.txt_maxVersionFileSizeMB.Size = New System.Drawing.Size(160, 21)
        Me.txt_maxVersionFileSizeMB.TabIndex = 25
        Me.txt_maxVersionFileSizeMB.Text = "UltraTextEditor1"
        '
        'UltraLabel6
        '
        Appearance3.FontData.BoldAsString = "False"
        Appearance3.TextHAlignAsString = "Right"
        Appearance3.TextVAlignAsString = "Middle"
        Me.UltraLabel6.Appearance = Appearance3
        Me.UltraLabel6.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel6.Location = New System.Drawing.Point(38, 89)
        Me.UltraLabel6.Name = "UltraLabel6"
        Me.UltraLabel6.Size = New System.Drawing.Size(137, 16)
        Me.UltraLabel6.TabIndex = 22
        Me.UltraLabel6.Text = "Max Months"
        '
        'txt_MaxVersionMonths
        '
        Me.txt_MaxVersionMonths.Location = New System.Drawing.Point(181, 84)
        Me.txt_MaxVersionMonths.Name = "txt_MaxVersionMonths"
        Me.txt_MaxVersionMonths.Size = New System.Drawing.Size(160, 21)
        Me.txt_MaxVersionMonths.TabIndex = 23
        Me.txt_MaxVersionMonths.Text = "UltraTextEditor1"
        '
        'UltraLabel5
        '
        Appearance4.FontData.BoldAsString = "False"
        Appearance4.TextHAlignAsString = "Right"
        Appearance4.TextVAlignAsString = "Middle"
        Me.UltraLabel5.Appearance = Appearance4
        Me.UltraLabel5.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel5.Location = New System.Drawing.Point(38, 59)
        Me.UltraLabel5.Name = "UltraLabel5"
        Me.UltraLabel5.Size = New System.Drawing.Size(137, 16)
        Me.UltraLabel5.TabIndex = 20
        Me.UltraLabel5.Text = "Min Differential Minutes"
        '
        'txt_VersionDiffMinutes
        '
        Me.txt_VersionDiffMinutes.Location = New System.Drawing.Point(181, 54)
        Me.txt_VersionDiffMinutes.Name = "txt_VersionDiffMinutes"
        Me.txt_VersionDiffMinutes.Size = New System.Drawing.Size(160, 21)
        Me.txt_VersionDiffMinutes.TabIndex = 21
        Me.txt_VersionDiffMinutes.Text = "UltraTextEditor1"
        '
        'UltraLabel3
        '
        Appearance5.FontData.BoldAsString = "False"
        Appearance5.TextHAlignAsString = "Right"
        Appearance5.TextVAlignAsString = "Middle"
        Me.UltraLabel3.Appearance = Appearance5
        Me.UltraLabel3.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel3.Location = New System.Drawing.Point(38, 29)
        Me.UltraLabel3.Name = "UltraLabel3"
        Me.UltraLabel3.Size = New System.Drawing.Size(137, 16)
        Me.UltraLabel3.TabIndex = 19
        Me.UltraLabel3.Text = "Maintain History"
        '
        'UltraTabPageControl7
        '
        Me.UltraTabPageControl7.Controls.Add(Me.Label1)
        Me.UltraTabPageControl7.Controls.Add(Me.cmb_BackupShareID)
        Me.UltraTabPageControl7.Controls.Add(Me.UltraGroupBox2)
        Me.UltraTabPageControl7.Controls.Add(Me.UltraGroupBox1)
        Me.UltraTabPageControl7.Location = New System.Drawing.Point(94, 2)
        Me.UltraTabPageControl7.Name = "UltraTabPageControl7"
        Me.UltraTabPageControl7.Size = New System.Drawing.Size(516, 268)
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(46, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(98, 16)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Backup Share"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmb_BackupShareID
        '
        Appearance6.FontData.BoldAsString = "False"
        Me.cmb_BackupShareID.Appearance = Appearance6
        Me.cmb_BackupShareID.Location = New System.Drawing.Point(150, 12)
        Me.cmb_BackupShareID.Name = "cmb_BackupShareID"
        Me.cmb_BackupShareID.Size = New System.Drawing.Size(315, 22)
        Me.cmb_BackupShareID.TabIndex = 11
        Me.cmb_BackupShareID.Text = "UltraCombo5"
        '
        'UltraGroupBox2
        '
        Me.UltraGroupBox2.Controls.Add(Me.btnRunIncr)
        Me.UltraGroupBox2.Controls.Add(Me.cmb_IncrBackupPeriodType)
        Me.UltraGroupBox2.Controls.Add(Me.txt_IncrBackupPeriodValue)
        Me.UltraGroupBox2.Controls.Add(Me.UltraLabel10)
        Me.UltraGroupBox2.Controls.Add(Me.UltraLabel9)
        Me.UltraGroupBox2.Controls.Add(Me.txt_BackupRootPathIncr)
        Me.UltraGroupBox2.Location = New System.Drawing.Point(34, 156)
        Me.UltraGroupBox2.Name = "UltraGroupBox2"
        Me.UltraGroupBox2.Size = New System.Drawing.Size(431, 87)
        Me.UltraGroupBox2.TabIndex = 1
        Me.UltraGroupBox2.Text = "Incremental "
        '
        'cmb_IncrBackupPeriodType
        '
        Appearance7.FontData.BoldAsString = "False"
        Me.cmb_IncrBackupPeriodType.Appearance = Appearance7
        Me.cmb_IncrBackupPeriodType.Location = New System.Drawing.Point(219, 18)
        Me.cmb_IncrBackupPeriodType.Name = "cmb_IncrBackupPeriodType"
        Me.cmb_IncrBackupPeriodType.Size = New System.Drawing.Size(160, 22)
        Me.cmb_IncrBackupPeriodType.TabIndex = 56
        Me.cmb_IncrBackupPeriodType.Text = "UltraCombo5"
        '
        'txt_IncrBackupPeriodValue
        '
        Me.txt_IncrBackupPeriodValue.Location = New System.Drawing.Point(116, 19)
        Me.txt_IncrBackupPeriodValue.Name = "txt_IncrBackupPeriodValue"
        Me.txt_IncrBackupPeriodValue.Size = New System.Drawing.Size(97, 21)
        Me.txt_IncrBackupPeriodValue.TabIndex = 55
        Me.txt_IncrBackupPeriodValue.Text = "UltraTextEditor1"
        '
        'UltraLabel10
        '
        Appearance8.FontData.BoldAsString = "False"
        Appearance8.TextHAlignAsString = "Right"
        Appearance8.TextVAlignAsString = "Middle"
        Me.UltraLabel10.Appearance = Appearance8
        Me.UltraLabel10.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel10.Location = New System.Drawing.Point(36, 22)
        Me.UltraLabel10.Name = "UltraLabel10"
        Me.UltraLabel10.Size = New System.Drawing.Size(74, 16)
        Me.UltraLabel10.TabIndex = 54
        Me.UltraLabel10.Text = "Period"
        '
        'UltraLabel9
        '
        Appearance9.FontData.BoldAsString = "False"
        Appearance9.TextHAlignAsString = "Right"
        Appearance9.TextVAlignAsString = "Middle"
        Me.UltraLabel9.Appearance = Appearance9
        Me.UltraLabel9.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel9.Location = New System.Drawing.Point(6, 51)
        Me.UltraLabel9.Name = "UltraLabel9"
        Me.UltraLabel9.Size = New System.Drawing.Size(104, 16)
        Me.UltraLabel9.TabIndex = 52
        Me.UltraLabel9.Text = "Relative Root Path"
        '
        'txt_BackupRootPathIncr
        '
        Me.txt_BackupRootPathIncr.Location = New System.Drawing.Point(116, 46)
        Me.txt_BackupRootPathIncr.Name = "txt_BackupRootPathIncr"
        Me.txt_BackupRootPathIncr.Size = New System.Drawing.Size(160, 21)
        Me.txt_BackupRootPathIncr.TabIndex = 53
        Me.txt_BackupRootPathIncr.Text = "UltraTextEditor1"
        '
        'UltraGroupBox1
        '
        Me.UltraGroupBox1.Controls.Add(Me.btnRunFull)
        Me.UltraGroupBox1.Controls.Add(Me.chk_UseSubDir)
        Me.UltraGroupBox1.Controls.Add(Me.UltraLabel11)
        Me.UltraGroupBox1.Controls.Add(Me.txt_BackupRootPathFull)
        Me.UltraGroupBox1.Controls.Add(Me.cmb_FullBackupPeriodType)
        Me.UltraGroupBox1.Controls.Add(Me.txt_FullBackupPeriodValue)
        Me.UltraGroupBox1.Controls.Add(Me.UltraLabel12)
        Me.UltraGroupBox1.Location = New System.Drawing.Point(34, 40)
        Me.UltraGroupBox1.Name = "UltraGroupBox1"
        Me.UltraGroupBox1.Size = New System.Drawing.Size(431, 110)
        Me.UltraGroupBox1.TabIndex = 0
        Me.UltraGroupBox1.Text = "Full"
        '
        'chk_UseSubDir
        '
        Me.chk_UseSubDir.Location = New System.Drawing.Point(116, 74)
        Me.chk_UseSubDir.Name = "chk_UseSubDir"
        Me.chk_UseSubDir.Size = New System.Drawing.Size(219, 21)
        Me.chk_UseSubDir.TabIndex = 56
        Me.chk_UseSubDir.Text = "Use Sub Directory Wise Replication"
        '
        'UltraLabel11
        '
        Appearance10.FontData.BoldAsString = "False"
        Appearance10.TextHAlignAsString = "Right"
        Appearance10.TextVAlignAsString = "Middle"
        Me.UltraLabel11.Appearance = Appearance10
        Me.UltraLabel11.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel11.Location = New System.Drawing.Point(6, 52)
        Me.UltraLabel11.Name = "UltraLabel11"
        Me.UltraLabel11.Size = New System.Drawing.Size(104, 16)
        Me.UltraLabel11.TabIndex = 54
        Me.UltraLabel11.Text = "Relative Root Path"
        '
        'txt_BackupRootPathFull
        '
        Me.txt_BackupRootPathFull.Location = New System.Drawing.Point(116, 47)
        Me.txt_BackupRootPathFull.Name = "txt_BackupRootPathFull"
        Me.txt_BackupRootPathFull.Size = New System.Drawing.Size(160, 21)
        Me.txt_BackupRootPathFull.TabIndex = 55
        Me.txt_BackupRootPathFull.Text = "UltraTextEditor4"
        '
        'cmb_FullBackupPeriodType
        '
        Me.cmb_FullBackupPeriodType.Location = New System.Drawing.Point(219, 19)
        Me.cmb_FullBackupPeriodType.Name = "cmb_FullBackupPeriodType"
        Me.cmb_FullBackupPeriodType.Size = New System.Drawing.Size(160, 22)
        Me.cmb_FullBackupPeriodType.TabIndex = 46
        Me.cmb_FullBackupPeriodType.Text = "UltraCombo5"
        '
        'txt_FullBackupPeriodValue
        '
        Me.txt_FullBackupPeriodValue.Location = New System.Drawing.Point(116, 19)
        Me.txt_FullBackupPeriodValue.Name = "txt_FullBackupPeriodValue"
        Me.txt_FullBackupPeriodValue.Size = New System.Drawing.Size(97, 21)
        Me.txt_FullBackupPeriodValue.TabIndex = 45
        Me.txt_FullBackupPeriodValue.Text = "UltraTextEditor1"
        '
        'UltraLabel12
        '
        Appearance11.FontData.BoldAsString = "False"
        Appearance11.TextHAlignAsString = "Right"
        Appearance11.TextVAlignAsString = "Middle"
        Me.UltraLabel12.Appearance = Appearance11
        Me.UltraLabel12.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel12.Location = New System.Drawing.Point(36, 22)
        Me.UltraLabel12.Name = "UltraLabel12"
        Me.UltraLabel12.Size = New System.Drawing.Size(74, 16)
        Me.UltraLabel12.TabIndex = 43
        Me.UltraLabel12.Text = "Period"
        '
        'UltraTabPageControl6
        '
        Me.UltraTabPageControl6.Controls.Add(Me.txt_EmailTo)
        Me.UltraTabPageControl6.Controls.Add(Me.UltraLabel13)
        Me.UltraTabPageControl6.Controls.Add(Me.UltraLabel14)
        Me.UltraTabPageControl6.Controls.Add(Me.txt_Emailcc)
        Me.UltraTabPageControl6.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl6.Name = "UltraTabPageControl6"
        Me.UltraTabPageControl6.Size = New System.Drawing.Size(516, 268)
        '
        'txt_EmailTo
        '
        Me.txt_EmailTo.Location = New System.Drawing.Point(137, 31)
        Me.txt_EmailTo.Name = "txt_EmailTo"
        Me.txt_EmailTo.Size = New System.Drawing.Size(357, 21)
        Me.txt_EmailTo.TabIndex = 59
        Me.txt_EmailTo.Text = "UltraTextEditor1"
        '
        'UltraLabel13
        '
        Appearance12.FontData.BoldAsString = "False"
        Appearance12.TextHAlignAsString = "Right"
        Appearance12.TextVAlignAsString = "Middle"
        Me.UltraLabel13.Appearance = Appearance12
        Me.UltraLabel13.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel13.Location = New System.Drawing.Point(57, 34)
        Me.UltraLabel13.Name = "UltraLabel13"
        Me.UltraLabel13.Size = New System.Drawing.Size(74, 16)
        Me.UltraLabel13.TabIndex = 58
        Me.UltraLabel13.Text = "To"
        '
        'UltraLabel14
        '
        Appearance13.FontData.BoldAsString = "False"
        Appearance13.TextHAlignAsString = "Right"
        Appearance13.TextVAlignAsString = "Middle"
        Me.UltraLabel14.Appearance = Appearance13
        Me.UltraLabel14.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel14.Location = New System.Drawing.Point(57, 63)
        Me.UltraLabel14.Name = "UltraLabel14"
        Me.UltraLabel14.Size = New System.Drawing.Size(74, 16)
        Me.UltraLabel14.TabIndex = 56
        Me.UltraLabel14.Text = "Cc"
        '
        'txt_Emailcc
        '
        Me.txt_Emailcc.Location = New System.Drawing.Point(137, 58)
        Me.txt_Emailcc.Name = "txt_Emailcc"
        Me.txt_Emailcc.Size = New System.Drawing.Size(357, 21)
        Me.txt_Emailcc.TabIndex = 57
        Me.txt_Emailcc.Text = "UltraTextEditor6"
        '
        'UltraTabPageControl1
        '
        Me.UltraTabPageControl1.Controls.Add(Me.UltraGridDir)
        Me.UltraTabPageControl1.Controls.Add(Me.Panel4)
        Me.UltraTabPageControl1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl1.Name = "UltraTabPageControl1"
        Me.UltraTabPageControl1.Size = New System.Drawing.Size(612, 272)
        '
        'UltraGridDir
        '
        Me.UltraGridDir.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridDir.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridDir.Name = "UltraGridDir"
        Me.UltraGridDir.Size = New System.Drawing.Size(612, 244)
        Me.UltraGridDir.TabIndex = 0
        '
        'Panel4
        '
        '
        'Panel4.ClientArea
        '
        Me.Panel4.ClientArea.Controls.Add(Me.btnEditDir)
        Me.Panel4.ClientArea.Controls.Add(Me.btnDelDir)
        Me.Panel4.ClientArea.Controls.Add(Me.btnAddDir)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 244)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(612, 28)
        Me.Panel4.TabIndex = 1
        '
        'btnEditDir
        '
        Me.btnEditDir.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnEditDir.Location = New System.Drawing.Point(389, 0)
        Me.btnEditDir.Name = "btnEditDir"
        Me.btnEditDir.Size = New System.Drawing.Size(70, 28)
        Me.btnEditDir.TabIndex = 4
        Me.btnEditDir.Text = "Edit"
        '
        'btnDelDir
        '
        Me.btnDelDir.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDelDir.Location = New System.Drawing.Point(459, 0)
        Me.btnDelDir.Name = "btnDelDir"
        Me.btnDelDir.Size = New System.Drawing.Size(83, 28)
        Me.btnDelDir.TabIndex = 2
        Me.btnDelDir.Text = "Delete"
        '
        'btnAddDir
        '
        Me.btnAddDir.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddDir.Location = New System.Drawing.Point(542, 0)
        Me.btnAddDir.Name = "btnAddDir"
        Me.btnAddDir.Size = New System.Drawing.Size(70, 28)
        Me.btnAddDir.TabIndex = 3
        Me.btnAddDir.Text = "Add New"
        '
        'UltraTabPageControl2
        '
        Me.UltraTabPageControl2.Controls.Add(Me.UltraGridAgent)
        Me.UltraTabPageControl2.Controls.Add(Me.UltraPanel2)
        Me.UltraTabPageControl2.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl2.Name = "UltraTabPageControl2"
        Me.UltraTabPageControl2.Size = New System.Drawing.Size(612, 272)
        '
        'UltraGridAgent
        '
        Me.UltraGridAgent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridAgent.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridAgent.Name = "UltraGridAgent"
        Me.UltraGridAgent.Size = New System.Drawing.Size(612, 244)
        Me.UltraGridAgent.TabIndex = 0
        '
        'UltraPanel2
        '
        '
        'UltraPanel2.ClientArea
        '
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnDelAgent)
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnAddAgent)
        Me.UltraPanel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.UltraPanel2.Location = New System.Drawing.Point(0, 244)
        Me.UltraPanel2.Name = "UltraPanel2"
        Me.UltraPanel2.Size = New System.Drawing.Size(612, 28)
        Me.UltraPanel2.TabIndex = 1
        '
        'btnDelAgent
        '
        Me.btnDelAgent.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDelAgent.Location = New System.Drawing.Point(459, 0)
        Me.btnDelAgent.Name = "btnDelAgent"
        Me.btnDelAgent.Size = New System.Drawing.Size(83, 28)
        Me.btnDelAgent.TabIndex = 0
        Me.btnDelAgent.Text = "Delete"
        '
        'btnAddAgent
        '
        Me.btnAddAgent.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddAgent.Location = New System.Drawing.Point(542, 0)
        Me.btnAddAgent.Name = "btnAddAgent"
        Me.btnAddAgent.Size = New System.Drawing.Size(70, 28)
        Me.btnAddAgent.TabIndex = 1
        Me.btnAddAgent.Text = "Add New"
        '
        'UltraTabPageControl4
        '
        Me.UltraTabPageControl4.Controls.Add(Me.UltraTabControl5)
        Me.UltraTabPageControl4.Location = New System.Drawing.Point(2, 24)
        Me.UltraTabPageControl4.Name = "UltraTabPageControl4"
        Me.UltraTabPageControl4.Size = New System.Drawing.Size(612, 272)
        '
        'UltraTabControl5
        '
        Me.UltraTabControl5.Controls.Add(Me.UltraTabSharedControlsPage5)
        Me.UltraTabControl5.Controls.Add(Me.UltraTabPageControl25)
        Me.UltraTabControl5.Controls.Add(Me.UltraTabPageControl5)
        Me.UltraTabControl5.Controls.Add(Me.UltraTabPageControl6)
        Me.UltraTabControl5.Controls.Add(Me.UltraTabPageControl7)
        Me.UltraTabControl5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraTabControl5.Location = New System.Drawing.Point(0, 0)
        Me.UltraTabControl5.Name = "UltraTabControl5"
        Appearance14.FontData.BoldAsString = "True"
        Me.UltraTabControl5.SelectedTabAppearance = Appearance14
        Me.UltraTabControl5.SharedControlsPage = Me.UltraTabSharedControlsPage5
        Me.UltraTabControl5.Size = New System.Drawing.Size(612, 272)
        Me.UltraTabControl5.TabIndex = 1
        Me.UltraTabControl5.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop
        Me.UltraTabControl5.TabPadding = New System.Drawing.Size(20, 10)
        UltraTab15.Key = "c"
        UltraTab15.TabPage = Me.UltraTabPageControl25
        UltraTab15.Text = "Cloud"
        UltraTab7.Key = "f"
        UltraTab7.TabPage = Me.UltraTabPageControl5
        UltraTab7.Text = "Version"
        UltraTab6.Key = "Backup"
        UltraTab6.TabPage = Me.UltraTabPageControl7
        UltraTab6.Text = "Backup"
        UltraTab5.TabPage = Me.UltraTabPageControl6
        UltraTab5.Text = "Notify"
        Me.UltraTabControl5.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab15, UltraTab7, UltraTab6, UltraTab5})
        Me.UltraTabControl5.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal
        '
        'UltraTabSharedControlsPage5
        '
        Me.UltraTabSharedControlsPage5.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage5.Name = "UltraTabSharedControlsPage5"
        Me.UltraTabSharedControlsPage5.Size = New System.Drawing.Size(516, 268)
        '
        'UltraTabPageControl3
        '
        Me.UltraTabPageControl3.Controls.Add(Me.UltraGrid1)
        Me.UltraTabPageControl3.Controls.Add(Me.UltraPanel3)
        Me.UltraTabPageControl3.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl3.Name = "UltraTabPageControl3"
        Me.UltraTabPageControl3.Size = New System.Drawing.Size(612, 272)
        '
        'UltraGrid1
        '
        Me.UltraGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGrid1.Location = New System.Drawing.Point(0, 0)
        Me.UltraGrid1.Name = "UltraGrid1"
        Me.UltraGrid1.Size = New System.Drawing.Size(612, 244)
        Me.UltraGrid1.TabIndex = 3
        '
        'UltraPanel3
        '
        '
        'UltraPanel3.ClientArea
        '
        Me.UltraPanel3.ClientArea.Controls.Add(Me.btnEditWS)
        Me.UltraPanel3.ClientArea.Controls.Add(Me.btnAddWS)
        Me.UltraPanel3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.UltraPanel3.Location = New System.Drawing.Point(0, 244)
        Me.UltraPanel3.Name = "UltraPanel3"
        Me.UltraPanel3.Size = New System.Drawing.Size(612, 28)
        Me.UltraPanel3.TabIndex = 2
        '
        'btnEditWS
        '
        Me.btnEditWS.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnEditWS.Location = New System.Drawing.Point(472, 0)
        Me.btnEditWS.Name = "btnEditWS"
        Me.btnEditWS.Size = New System.Drawing.Size(70, 28)
        Me.btnEditWS.TabIndex = 4
        Me.btnEditWS.Text = "Edit"
        '
        'btnAddWS
        '
        Me.btnAddWS.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddWS.Location = New System.Drawing.Point(542, 0)
        Me.btnAddWS.Name = "btnAddWS"
        Me.btnAddWS.Size = New System.Drawing.Size(70, 28)
        Me.btnAddWS.TabIndex = 3
        Me.btnAddWS.Text = "Add New"
        '
        'UltraPanel1
        '
        '
        'UltraPanel1.ClientArea
        '
        Me.UltraPanel1.ClientArea.Controls.Add(Me.cmb_ShareType)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.cmb_IsBackup)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel8)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.Label9)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel2)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel1)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_Description)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_ShareName)
        Me.UltraPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.UltraPanel1.Location = New System.Drawing.Point(0, 0)
        Me.UltraPanel1.Name = "UltraPanel1"
        Me.UltraPanel1.Size = New System.Drawing.Size(616, 119)
        Me.UltraPanel1.TabIndex = 0
        '
        'cmb_ShareType
        '
        Appearance15.FontData.BoldAsString = "False"
        Me.cmb_ShareType.Appearance = Appearance15
        Me.cmb_ShareType.Location = New System.Drawing.Point(107, 71)
        Me.cmb_ShareType.Name = "cmb_ShareType"
        Me.cmb_ShareType.Size = New System.Drawing.Size(168, 22)
        Me.cmb_ShareType.TabIndex = 63
        Me.cmb_ShareType.Text = "UltraCombo5"
        '
        'cmb_IsBackup
        '
        Me.cmb_IsBackup.Location = New System.Drawing.Point(397, 69)
        Me.cmb_IsBackup.Name = "cmb_IsBackup"
        Me.cmb_IsBackup.Size = New System.Drawing.Size(160, 21)
        Me.cmb_IsBackup.TabIndex = 28
        Me.cmb_IsBackup.Text = "UltraComboEditor1"
        '
        'UltraLabel8
        '
        Appearance16.FontData.BoldAsString = "False"
        Appearance16.TextHAlignAsString = "Right"
        Appearance16.TextVAlignAsString = "Middle"
        Me.UltraLabel8.Appearance = Appearance16
        Me.UltraLabel8.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel8.Location = New System.Drawing.Point(305, 72)
        Me.UltraLabel8.Name = "UltraLabel8"
        Me.UltraLabel8.Size = New System.Drawing.Size(86, 16)
        Me.UltraLabel8.TabIndex = 27
        Me.UltraLabel8.Text = "Usage"
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(3, 73)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(96, 16)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "Type"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'UltraLabel2
        '
        Appearance17.FontData.BoldAsString = "False"
        Appearance17.TextHAlignAsString = "Right"
        Appearance17.TextVAlignAsString = "Middle"
        Me.UltraLabel2.Appearance = Appearance17
        Me.UltraLabel2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel2.Location = New System.Drawing.Point(15, 47)
        Me.UltraLabel2.Name = "UltraLabel2"
        Me.UltraLabel2.Size = New System.Drawing.Size(86, 16)
        Me.UltraLabel2.TabIndex = 2
        Me.UltraLabel2.Text = "Description"
        '
        'UltraLabel1
        '
        Appearance18.FontData.BoldAsString = "False"
        Appearance18.TextHAlignAsString = "Right"
        Appearance18.TextVAlignAsString = "Middle"
        Me.UltraLabel1.Appearance = Appearance18
        Me.UltraLabel1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel1.Location = New System.Drawing.Point(15, 17)
        Me.UltraLabel1.Name = "UltraLabel1"
        Me.UltraLabel1.Size = New System.Drawing.Size(86, 16)
        Me.UltraLabel1.TabIndex = 0
        Me.UltraLabel1.Text = "Name"
        '
        'txt_Description
        '
        Me.txt_Description.Location = New System.Drawing.Point(107, 42)
        Me.txt_Description.Name = "txt_Description"
        Me.txt_Description.Size = New System.Drawing.Size(450, 21)
        Me.txt_Description.TabIndex = 3
        Me.txt_Description.Text = "UltraTextEditor1"
        '
        'txt_ShareName
        '
        Appearance19.FontData.SizeInPoints = 10.0!
        Me.txt_ShareName.Appearance = Appearance19
        Me.txt_ShareName.Location = New System.Drawing.Point(107, 12)
        Me.txt_ShareName.Name = "txt_ShareName"
        Me.txt_ShareName.Size = New System.Drawing.Size(160, 24)
        Me.txt_ShareName.TabIndex = 1
        Me.txt_ShareName.Text = "UltraTextEditor1"
        '
        'Panel1
        '
        '
        'Panel1.ClientArea
        '
        Me.Panel1.ClientArea.Controls.Add(Me.btnSave)
        Me.Panel1.ClientArea.Controls.Add(Me.btnCancel)
        Me.Panel1.ClientArea.Controls.Add(Me.btnOK)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 417)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(616, 33)
        Me.Panel1.TabIndex = 2
        '
        'btnSave
        '
        Appearance20.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance20
        Me.btnSave.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnSave.Location = New System.Drawing.Point(412, 0)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(68, 33)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save"
        '
        'btnCancel
        '
        Appearance21.FontData.BoldAsString = "True"
        Me.btnCancel.Appearance = Appearance21
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnCancel.Location = New System.Drawing.Point(480, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(68, 33)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        '
        'btnOK
        '
        Appearance22.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance22
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnOK.Location = New System.Drawing.Point(548, 0)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(68, 33)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        '
        'UltraTabControl1
        '
        Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl1)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl2)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl4)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl3)
        Me.UltraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraTabControl1.Location = New System.Drawing.Point(0, 119)
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        Me.UltraTabControl1.Size = New System.Drawing.Size(616, 298)
        Me.UltraTabControl1.TabIndex = 1
        UltraTab1.Key = "dir"
        UltraTab1.TabPage = Me.UltraTabPageControl1
        UltraTab1.Text = "Directory"
        UltraTab2.Key = "agent"
        UltraTab2.TabPage = Me.UltraTabPageControl2
        UltraTab2.Text = "Agents"
        UltraTab4.Key = "conf"
        UltraTab4.TabPage = Me.UltraTabPageControl4
        UltraTab4.Text = "Config"
        UltraTab3.Key = "local"
        UltraTab3.TabPage = Me.UltraTabPageControl3
        UltraTab3.Text = "Local"
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2, UltraTab4, UltraTab3})
        '
        'UltraTabSharedControlsPage1
        '
        Me.UltraTabSharedControlsPage1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        Me.UltraTabSharedControlsPage1.Size = New System.Drawing.Size(612, 272)
        '
        'btnRunFull
        '
        Me.btnRunFull.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRunFull.Location = New System.Drawing.Point(361, 81)
        Me.btnRunFull.Name = "btnRunFull"
        Me.btnRunFull.Size = New System.Drawing.Size(70, 29)
        Me.btnRunFull.TabIndex = 57
        Me.btnRunFull.Text = "Run Now"
        '
        'btnRunIncr
        '
        Me.btnRunIncr.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRunIncr.Location = New System.Drawing.Point(361, 58)
        Me.btnRunIncr.Name = "btnRunIncr"
        Me.btnRunIncr.Size = New System.Drawing.Size(70, 29)
        Me.btnRunIncr.TabIndex = 58
        Me.btnRunIncr.Text = "Run Now"
        '
        'frmFileShare
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "File Share"
        Me.ClientSize = New System.Drawing.Size(616, 450)
        Me.Controls.Add(Me.UltraTabControl1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.UltraPanel1)
        Me.Name = "frmFileShare"
        Me.Text = "File Share"
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl25.ResumeLayout(False)
        Me.UltraTabPageControl25.PerformLayout()
        CType(Me.txt_ShareContainerName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl5.ResumeLayout(False)
        Me.UltraTabPageControl5.PerformLayout()
        CType(Me.cmb_MaintainVersionHistory, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_maxVersionFileSizeMB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_MaxVersionMonths, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_VersionDiffMinutes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl7.ResumeLayout(False)
        Me.UltraTabPageControl7.PerformLayout()
        CType(Me.cmb_BackupShareID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraGroupBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraGroupBox2.ResumeLayout(False)
        Me.UltraGroupBox2.PerformLayout()
        CType(Me.cmb_IncrBackupPeriodType, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_IncrBackupPeriodValue, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_BackupRootPathIncr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraGroupBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraGroupBox1.ResumeLayout(False)
        Me.UltraGroupBox1.PerformLayout()
        CType(Me.chk_UseSubDir, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_BackupRootPathFull, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmb_FullBackupPeriodType, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_FullBackupPeriodValue, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl6.ResumeLayout(False)
        Me.UltraTabPageControl6.PerformLayout()
        CType(Me.txt_EmailTo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_Emailcc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl1.ResumeLayout(False)
        CType(Me.UltraGridDir, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ClientArea.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.UltraTabPageControl2.ResumeLayout(False)
        CType(Me.UltraGridAgent, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraPanel2.ClientArea.ResumeLayout(False)
        Me.UltraPanel2.ResumeLayout(False)
        Me.UltraTabPageControl4.ResumeLayout(False)
        CType(Me.UltraTabControl5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl5.ResumeLayout(False)
        Me.UltraTabPageControl3.ResumeLayout(False)
        CType(Me.UltraGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraPanel3.ClientArea.ResumeLayout(False)
        Me.UltraPanel3.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.PerformLayout()
        Me.UltraPanel1.ResumeLayout(False)
        CType(Me.cmb_ShareType, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmb_IsBackup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_Description, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_ShareName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ClientArea.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents txt_Description As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_ShareName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Panel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl1 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridDir As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents Panel4 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDelDir As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddDir As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraTabPageControl2 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridAgent As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents UltraPanel2 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDelAgent As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddAgent As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Label9 As Label
    Friend WithEvents UltraTabPageControl4 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraTabControl5 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage5 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl25 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraTabPageControl5 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraLabel4 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_ShareContainerName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraTabPageControl6 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraTabPageControl7 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents cmb_MaintainVersionHistory As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents UltraLabel7 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_maxVersionFileSizeMB As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel6 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_MaxVersionMonths As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel5 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_VersionDiffMinutes As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel3 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents cmb_IsBackup As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents UltraLabel8 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraGroupBox2 As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents UltraGroupBox1 As Infragistics.Win.Misc.UltraGroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cmb_BackupShareID As UltraCombo
    Friend WithEvents txt_FullBackupPeriodValue As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel12 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel9 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_BackupRootPathIncr As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents cmb_FullBackupPeriodType As UltraCombo
    Friend WithEvents cmb_IncrBackupPeriodType As UltraCombo
    Friend WithEvents txt_IncrBackupPeriodValue As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel10 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents chk_UseSubDir As Infragistics.Win.UltraWinEditors.UltraCheckEditor
    Friend WithEvents UltraLabel11 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_BackupRootPathFull As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_EmailTo As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel13 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel14 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_Emailcc As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents btnEditDir As Infragistics.Win.Misc.UltraButton
    Friend WithEvents cmb_ShareType As UltraCombo
    Friend WithEvents UltraTabPageControl3 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraPanel3 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnEditWS As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddWS As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraGrid1 As UltraGrid
    Friend WithEvents btnRunIncr As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnRunFull As Infragistics.Win.Misc.UltraButton

#End Region
End Class

