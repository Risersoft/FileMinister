Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabControl
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Partial Class frmSearchDWG
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
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents cmb_subject As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents UltraLabel10 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraGrid1 As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents btnClose As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnGo As Infragistics.Win.Misc.UltraButton
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnClose = New Infragistics.Win.Misc.UltraButton()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_FileName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.btnPrint = New Infragistics.Win.Misc.UltraButton()
        Me.dt_ModAfter = New Infragistics.Win.UltraWinEditors.UltraDateTimeEditor()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.btnGo = New Infragistics.Win.Misc.UltraButton()
        Me.cmb_subject = New Infragistics.Win.UltraWinEditors.UltraComboEditor()
        Me.UltraLabel10 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraGrid1 = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.cmb_filestoreid = New Infragistics.Win.UltraWinGrid.UltraCombo()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.txt_FileName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dt_ModAfter, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmb_subject, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UltraGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.cmb_filestoreid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.btnClose)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(0, 590)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(840, 48)
        Me.Panel2.TabIndex = 2
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Appearance1.FontData.BoldAsString = "True"
        Me.btnClose.Appearance = Appearance1
        Me.btnClose.Location = New System.Drawing.Point(744, 8)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(88, 32)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "&Close"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label12)
        Me.Panel1.Controls.Add(Me.cmb_filestoreid)
        Me.Panel1.Controls.Add(Me.UltraLabel1)
        Me.Panel1.Controls.Add(Me.txt_FileName)
        Me.Panel1.Controls.Add(Me.btnPrint)
        Me.Panel1.Controls.Add(Me.dt_ModAfter)
        Me.Panel1.Controls.Add(Me.Label20)
        Me.Panel1.Controls.Add(Me.btnGo)
        Me.Panel1.Controls.Add(Me.cmb_subject)
        Me.Panel1.Controls.Add(Me.UltraLabel10)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(840, 130)
        Me.Panel1.TabIndex = 3
        '
        'UltraLabel1
        '
        Appearance4.TextHAlignAsString = "Right"
        Me.UltraLabel1.Appearance = Appearance4
        Me.UltraLabel1.Location = New System.Drawing.Point(64, 44)
        Me.UltraLabel1.Name = "UltraLabel1"
        Me.UltraLabel1.Size = New System.Drawing.Size(104, 16)
        Me.UltraLabel1.TabIndex = 439
        Me.UltraLabel1.Text = "FileName"
        '
        'txt_FileName
        '
        Appearance10.FontData.BoldAsString = "False"
        Appearance10.FontData.ItalicAsString = "False"
        Appearance10.FontData.Name = "Arial"
        Appearance10.FontData.SizeInPoints = 8.25!
        Appearance10.FontData.StrikeoutAsString = "False"
        Appearance10.FontData.UnderlineAsString = "False"
        Me.txt_FileName.Appearance = Appearance10
        Me.txt_FileName.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.txt_FileName.Location = New System.Drawing.Point(176, 40)
        Me.txt_FileName.Name = "txt_FileName"
        Me.txt_FileName.Size = New System.Drawing.Size(224, 21)
        Me.txt_FileName.TabIndex = 438
        '
        'btnPrint
        '
        Appearance2.FontData.BoldAsString = "True"
        Me.btnPrint.Appearance = Appearance2
        Me.btnPrint.Location = New System.Drawing.Point(513, 83)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(88, 32)
        Me.btnPrint.TabIndex = 437
        Me.btnPrint.Text = "Print"
        '
        'dt_ModAfter
        '
        Me.dt_ModAfter.FormatString = "dddd dd MMM yyyy"
        Me.dt_ModAfter.Location = New System.Drawing.Point(176, 94)
        Me.dt_ModAfter.Name = "dt_ModAfter"
        Me.dt_ModAfter.NullText = "Not Defined"
        Me.dt_ModAfter.Size = New System.Drawing.Size(224, 21)
        Me.dt_ModAfter.TabIndex = 436
        '
        'Label20
        '
        Me.Label20.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.Label20.Location = New System.Drawing.Point(60, 94)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(108, 16)
        Me.Label20.TabIndex = 435
        Me.Label20.Text = "Modified After"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnGo
        '
        Appearance5.FontData.BoldAsString = "True"
        Me.btnGo.Appearance = Appearance5
        Me.btnGo.Location = New System.Drawing.Point(419, 83)
        Me.btnGo.Name = "btnGo"
        Me.btnGo.Size = New System.Drawing.Size(88, 32)
        Me.btnGo.TabIndex = 434
        Me.btnGo.Text = "Search"
        '
        'cmb_subject
        '
        Me.cmb_subject.Location = New System.Drawing.Point(176, 67)
        Me.cmb_subject.Name = "cmb_subject"
        Me.cmb_subject.Size = New System.Drawing.Size(224, 21)
        Me.cmb_subject.TabIndex = 432
        '
        'UltraLabel10
        '
        Appearance7.TextHAlignAsString = "Right"
        Me.UltraLabel10.Appearance = Appearance7
        Me.UltraLabel10.Location = New System.Drawing.Point(64, 71)
        Me.UltraLabel10.Name = "UltraLabel10"
        Me.UltraLabel10.Size = New System.Drawing.Size(104, 16)
        Me.UltraLabel10.TabIndex = 431
        Me.UltraLabel10.Text = "Subject"
        '
        'UltraGrid1
        '
        Me.UltraGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGrid1.Location = New System.Drawing.Point(0, 0)
        Me.UltraGrid1.Name = "UltraGrid1"
        Me.UltraGrid1.Size = New System.Drawing.Size(400, 460)
        Me.UltraGrid1.TabIndex = 18
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 130)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.UltraGrid1)
        Me.SplitContainer1.Size = New System.Drawing.Size(840, 460)
        Me.SplitContainer1.SplitterDistance = 400
        Me.SplitContainer1.TabIndex = 20
        '
        'Label12
        '
        Me.Label12.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.Label12.Location = New System.Drawing.Point(70, 15)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(100, 16)
        Me.Label12.TabIndex = 441
        Me.Label12.Text = "Server"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmb_filestoreid
        '
        Me.cmb_filestoreid.Location = New System.Drawing.Point(176, 12)
        Me.cmb_filestoreid.Name = "cmb_filestoreid"
        Me.cmb_filestoreid.Size = New System.Drawing.Size(224, 22)
        Me.cmb_filestoreid.TabIndex = 440
        '
        'frmSearchDWG
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "Search Files"
        Me.ClientSize = New System.Drawing.Size(840, 638)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Name = "frmSearchDWG"
        Me.Text = "Search Files"
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.txt_FileName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dt_ModAfter, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmb_subject, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UltraGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.cmb_filestoreid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dt_ModAfter As Infragistics.Win.UltraWinEditors.UltraDateTimeEditor
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents btnPrint As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents txt_FileName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents cmb_filestoreid As Infragistics.Win.UltraWinGrid.UltraCombo

#End Region
End Class

