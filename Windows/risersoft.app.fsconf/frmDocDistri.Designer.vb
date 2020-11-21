Imports Infragistics.Win.UltraWinGrid
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmDocDistri
    Inherits frmMax

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        InitForm()
        'Add any initialization after the InitializeComponent() call

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
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmb_MatDepID As Infragistics.Win.UltraWinGrid.UltraCombo
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_DistriCode As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl1 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraLabel17 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents cmb_DocTypeCode As Infragistics.Win.UltraWinEditors.UltraComboEditor
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance29 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance30 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance31 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance45 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance39 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance40 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraTab5 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim Appearance35 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.UltraTabPageControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.cmb_DocTypeCode = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.UltraLabel17 = New Infragistics.Win.Misc.UltraLabel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmb_MatDepID = New Infragistics.Win.UltraWinGrid.UltraCombo()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_DistriCode = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        Me.UGridDep = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.cmb_MfgTypeCode = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl1.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.cmb_DocTypeCode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmb_MatDepID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_DistriCode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()
        CType(Me.UGridDep, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmb_MfgTypeCode, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraTabPageControl1
        '
        Me.UltraTabPageControl1.Controls.Add(Me.UGridDep)
        Me.UltraTabPageControl1.Location = New System.Drawing.Point(1, 23)
        Me.UltraTabPageControl1.Name = "UltraTabPageControl1"
        Me.UltraTabPageControl1.Size = New System.Drawing.Size(502, 294)
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.btnSave)
        Me.Panel4.Controls.Add(Me.btnCancel)
        Me.Panel4.Controls.Add(Me.btnOK)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 512)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(506, 48)
        Me.Panel4.TabIndex = 3
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance29.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance29
        Me.btnSave.Location = New System.Drawing.Point(218, 8)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(88, 32)
        Me.btnSave.TabIndex = 2
        Me.btnSave.Text = "&Save"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance30.FontData.BoldAsString = "True"
        Me.btnCancel.Appearance = Appearance30
        Me.btnCancel.Location = New System.Drawing.Point(314, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(88, 32)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "&Cancel"
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance31.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance31
        Me.btnOK.Location = New System.Drawing.Point(410, 8)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(88, 32)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "&OK"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.cmb_MfgTypeCode)
        Me.Panel1.Controls.Add(Me.UltraLabel1)
        Me.Panel1.Controls.Add(Me.cmb_DocTypeCode)
        Me.Panel1.Controls.Add(Me.UltraLabel17)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.cmb_MatDepID)
        Me.Panel1.Controls.Add(Me.UltraLabel2)
        Me.Panel1.Controls.Add(Me.txt_DistriCode)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(506, 192)
        Me.Panel1.TabIndex = 121
        '
        'cmb_DocTypeCode
        '
        Me.cmb_DocTypeCode.Location = New System.Drawing.Point(178, 98)
        Me.cmb_DocTypeCode.Name = "cmb_DocTypeCode"
        Me.cmb_DocTypeCode.Size = New System.Drawing.Size(224, 21)
        Me.cmb_DocTypeCode.TabIndex = 158
        Me.cmb_DocTypeCode.Text = "UltraComboEditor1"
        '
        'UltraLabel17
        '
        Appearance45.TextHAlignAsString = "Right"
        Me.UltraLabel17.Appearance = Appearance45
        Me.UltraLabel17.Location = New System.Drawing.Point(51, 98)
        Me.UltraLabel17.Name = "UltraLabel17"
        Me.UltraLabel17.Size = New System.Drawing.Size(119, 16)
        Me.UltraLabel17.TabIndex = 153
        Me.UltraLabel17.Text = "Document Type Code"
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.Label2.Location = New System.Drawing.Point(106, 60)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 16)
        Me.Label2.TabIndex = 116
        Me.Label2.Text = "Shop"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmb_MatDepID
        '
        Me.cmb_MatDepID.Location = New System.Drawing.Point(178, 60)
        Me.cmb_MatDepID.Name = "cmb_MatDepID"
        Me.cmb_MatDepID.Size = New System.Drawing.Size(224, 22)
        Me.cmb_MatDepID.TabIndex = 115
        Me.cmb_MatDepID.Text = "UltraCombo4"
        '
        'UltraLabel2
        '
        Appearance39.FontData.BoldAsString = "True"
        Appearance39.FontData.SizeInPoints = 9.0!
        Appearance39.TextHAlignAsString = "Right"
        Me.UltraLabel2.Appearance = Appearance39
        Me.UltraLabel2.Location = New System.Drawing.Point(53, 22)
        Me.UltraLabel2.Name = "UltraLabel2"
        Me.UltraLabel2.Size = New System.Drawing.Size(113, 16)
        Me.UltraLabel2.TabIndex = 112
        Me.UltraLabel2.Text = "Distribution Code"
        '
        'txt_DistriCode
        '
        Appearance40.FontData.BoldAsString = "True"
        Appearance40.FontData.ItalicAsString = "False"
        Appearance40.FontData.Name = "Arial"
        Appearance40.FontData.SizeInPoints = 9.0!
        Appearance40.FontData.StrikeoutAsString = "False"
        Appearance40.FontData.UnderlineAsString = "False"
        Me.txt_DistriCode.Appearance = Appearance40
        Me.txt_DistriCode.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_DistriCode.Location = New System.Drawing.Point(174, 22)
        Me.txt_DistriCode.Name = "txt_DistriCode"
        Me.txt_DistriCode.ReadOnly = True
        Me.txt_DistriCode.Size = New System.Drawing.Size(269, 22)
        Me.txt_DistriCode.TabIndex = 111
        '
        'UltraTabControl1
        '
        Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
        Me.UltraTabControl1.Controls.Add(Me.UltraTabPageControl1)
        Me.UltraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraTabControl1.Location = New System.Drawing.Point(0, 192)
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        Me.UltraTabControl1.Size = New System.Drawing.Size(506, 320)
        Me.UltraTabControl1.TabIndex = 122
        UltraTab5.TabPage = Me.UltraTabPageControl1
        UltraTab5.Text = "Details"
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab5})
        '
        'UltraTabSharedControlsPage1
        '
        Me.UltraTabSharedControlsPage1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        Me.UltraTabSharedControlsPage1.Size = New System.Drawing.Size(502, 294)
        '
        'UGridDep
        '
        Me.UGridDep.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UGridDep.Location = New System.Drawing.Point(0, 0)
        Me.UGridDep.Name = "UGridDep"
        Me.UGridDep.Size = New System.Drawing.Size(502, 294)
        Me.UGridDep.TabIndex = 119
        Me.UGridDep.Text = "Departments"
        '
        'cmb_MfgTypeCode
        '
        Me.cmb_MfgTypeCode.Location = New System.Drawing.Point(178, 134)
        Me.cmb_MfgTypeCode.Name = "cmb_MfgTypeCode"
        Me.cmb_MfgTypeCode.Size = New System.Drawing.Size(224, 21)
        Me.cmb_MfgTypeCode.TabIndex = 160
        Me.cmb_MfgTypeCode.Text = "UltraComboEditor1"
        '
        'UltraLabel1
        '
        Appearance35.TextHAlignAsString = "Right"
        Me.UltraLabel1.Appearance = Appearance35
        Me.UltraLabel1.Location = New System.Drawing.Point(51, 134)
        Me.UltraLabel1.Name = "UltraLabel1"
        Me.UltraLabel1.Size = New System.Drawing.Size(119, 16)
        Me.UltraLabel1.TabIndex = 159
        Me.UltraLabel1.Text = "Mfg Type Code"
        '
        'frmDocDistri
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "Design Document"
        Me.ClientSize = New System.Drawing.Size(506, 560)
        Me.Controls.Add(Me.UltraTabControl1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel4)
        Me.Name = "frmDocDistri"
        Me.Text = "Design Document"
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl1.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.cmb_DocTypeCode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmb_MatDepID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_DistriCode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)
        CType(Me.UGridDep, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmb_MfgTypeCode, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UGridDep As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents cmb_MfgTypeCode As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel

#End Region
End Class

