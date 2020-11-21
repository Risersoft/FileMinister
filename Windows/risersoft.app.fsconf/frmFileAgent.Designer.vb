Imports Infragistics.Win.UltraWinGrid
Imports System.Xml
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class frmFileAgent
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
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Me.UltraTabPageControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridShare = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.Panel4 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnDelShare = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddShare = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabPageControl2 = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        Me.UltraGridWork = New Infragistics.Win.UltraWinGrid.UltraGrid()
        Me.UltraPanel2 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnEditWS = New Infragistics.Win.Misc.UltraButton()
        Me.btnDelWS = New Infragistics.Win.Misc.UltraButton()
        Me.btnAddWS = New Infragistics.Win.Misc.UltraButton()
        Me.UltraPanel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.UltraLabel2 = New Infragistics.Win.Misc.UltraLabel()
        Me.UltraLabel1 = New Infragistics.Win.Misc.UltraLabel()
        Me.txt_SecretKey = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.txt_AgentName = New Infragistics.Win.UltraWinEditors.UltraTextEditor()
        Me.Panel1 = New Infragistics.Win.Misc.UltraPanel()
        Me.btnSave = New Infragistics.Win.Misc.UltraButton()
        Me.btnCancel = New Infragistics.Win.Misc.UltraButton()
        Me.btnOK = New Infragistics.Win.Misc.UltraButton()
        Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabPageControl1.SuspendLayout()
        CType(Me.UltraGridShare, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.ClientArea.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.UltraTabPageControl2.SuspendLayout()
        CType(Me.UltraGridWork, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraPanel2.ClientArea.SuspendLayout()
        Me.UltraPanel2.SuspendLayout()
        Me.UltraPanel1.ClientArea.SuspendLayout()
        Me.UltraPanel1.SuspendLayout()
        CType(Me.txt_SecretKey, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txt_AgentName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.ClientArea.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.UltraTabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'UltraTabPageControl1
        '
        Me.UltraTabPageControl1.Controls.Add(Me.UltraGridShare)
        Me.UltraTabPageControl1.Controls.Add(Me.Panel4)
        Me.UltraTabPageControl1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabPageControl1.Name = "UltraTabPageControl1"
        Me.UltraTabPageControl1.Size = New System.Drawing.Size(689, 270)
        '
        'UltraGridShare
        '
        Me.UltraGridShare.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridShare.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridShare.Name = "UltraGridShare"
        Me.UltraGridShare.Size = New System.Drawing.Size(689, 242)
        Me.UltraGridShare.TabIndex = 0
        '
        'Panel4
        '
        '
        'Panel4.ClientArea
        '
        Me.Panel4.ClientArea.Controls.Add(Me.btnDelShare)
        Me.Panel4.ClientArea.Controls.Add(Me.btnAddShare)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 242)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(689, 28)
        Me.Panel4.TabIndex = 1
        '
        'btnDelShare
        '
        Me.btnDelShare.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDelShare.Location = New System.Drawing.Point(536, 0)
        Me.btnDelShare.Name = "btnDelShare"
        Me.btnDelShare.Size = New System.Drawing.Size(83, 28)
        Me.btnDelShare.TabIndex = 2
        Me.btnDelShare.Text = "Delete"
        '
        'btnAddShare
        '
        Me.btnAddShare.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddShare.Location = New System.Drawing.Point(619, 0)
        Me.btnAddShare.Name = "btnAddShare"
        Me.btnAddShare.Size = New System.Drawing.Size(70, 28)
        Me.btnAddShare.TabIndex = 3
        Me.btnAddShare.Text = "Add New"
        '
        'UltraTabPageControl2
        '
        Me.UltraTabPageControl2.Controls.Add(Me.UltraGridWork)
        Me.UltraTabPageControl2.Controls.Add(Me.UltraPanel2)
        Me.UltraTabPageControl2.Location = New System.Drawing.Point(1, 23)
        Me.UltraTabPageControl2.Name = "UltraTabPageControl2"
        Me.UltraTabPageControl2.Size = New System.Drawing.Size(689, 270)
        '
        'UltraGridWork
        '
        Me.UltraGridWork.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraGridWork.Location = New System.Drawing.Point(0, 0)
        Me.UltraGridWork.Name = "UltraGridWork"
        Me.UltraGridWork.Size = New System.Drawing.Size(689, 242)
        Me.UltraGridWork.TabIndex = 0
        '
        'UltraPanel2
        '
        '
        'UltraPanel2.ClientArea
        '
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnEditWS)
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnDelWS)
        Me.UltraPanel2.ClientArea.Controls.Add(Me.btnAddWS)
        Me.UltraPanel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.UltraPanel2.Location = New System.Drawing.Point(0, 242)
        Me.UltraPanel2.Name = "UltraPanel2"
        Me.UltraPanel2.Size = New System.Drawing.Size(689, 28)
        Me.UltraPanel2.TabIndex = 1
        '
        'btnEditWS
        '
        Me.btnEditWS.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnEditWS.Location = New System.Drawing.Point(453, 0)
        Me.btnEditWS.Name = "btnEditWS"
        Me.btnEditWS.Size = New System.Drawing.Size(83, 28)
        Me.btnEditWS.TabIndex = 5
        Me.btnEditWS.Text = "Edit"
        '
        'btnDelWS
        '
        Me.btnDelWS.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnDelWS.Location = New System.Drawing.Point(536, 0)
        Me.btnDelWS.Name = "btnDelWS"
        Me.btnDelWS.Size = New System.Drawing.Size(83, 28)
        Me.btnDelWS.TabIndex = 0
        Me.btnDelWS.Text = "Delete"
        '
        'btnAddWS
        '
        Me.btnAddWS.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnAddWS.Location = New System.Drawing.Point(619, 0)
        Me.btnAddWS.Name = "btnAddWS"
        Me.btnAddWS.Size = New System.Drawing.Size(70, 28)
        Me.btnAddWS.TabIndex = 1
        Me.btnAddWS.Text = "Add New"
        '
        'UltraPanel1
        '
        '
        'UltraPanel1.ClientArea
        '
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel2)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.UltraLabel1)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_SecretKey)
        Me.UltraPanel1.ClientArea.Controls.Add(Me.txt_AgentName)
        Me.UltraPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.UltraPanel1.Location = New System.Drawing.Point(0, 0)
        Me.UltraPanel1.Name = "UltraPanel1"
        Me.UltraPanel1.Size = New System.Drawing.Size(693, 86)
        Me.UltraPanel1.TabIndex = 0
        '
        'UltraLabel2
        '
        Appearance1.FontData.BoldAsString = "False"
        Appearance1.TextHAlignAsString = "Right"
        Appearance1.TextVAlignAsString = "Middle"
        Me.UltraLabel2.Appearance = Appearance1
        Me.UltraLabel2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel2.Location = New System.Drawing.Point(15, 47)
        Me.UltraLabel2.Name = "UltraLabel2"
        Me.UltraLabel2.Size = New System.Drawing.Size(86, 16)
        Me.UltraLabel2.TabIndex = 2
        Me.UltraLabel2.Text = "Secret Key"
        '
        'UltraLabel1
        '
        Appearance2.FontData.BoldAsString = "False"
        Appearance2.TextHAlignAsString = "Right"
        Appearance2.TextVAlignAsString = "Middle"
        Me.UltraLabel1.Appearance = Appearance2
        Me.UltraLabel1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UltraLabel1.Location = New System.Drawing.Point(15, 17)
        Me.UltraLabel1.Name = "UltraLabel1"
        Me.UltraLabel1.Size = New System.Drawing.Size(86, 16)
        Me.UltraLabel1.TabIndex = 0
        Me.UltraLabel1.Text = "Name"
        '
        'txt_SecretKey
        '
        Appearance3.FontData.SizeInPoints = 10.0!
        Me.txt_SecretKey.Appearance = Appearance3
        Me.txt_SecretKey.Location = New System.Drawing.Point(107, 42)
        Me.txt_SecretKey.Name = "txt_SecretKey"
        Me.txt_SecretKey.Size = New System.Drawing.Size(458, 24)
        Me.txt_SecretKey.TabIndex = 3
        Me.txt_SecretKey.Text = "UltraTextEditor1"
        '
        'txt_AgentName
        '
        Appearance4.FontData.SizeInPoints = 10.0!
        Me.txt_AgentName.Appearance = Appearance4
        Me.txt_AgentName.Location = New System.Drawing.Point(107, 12)
        Me.txt_AgentName.Name = "txt_AgentName"
        Me.txt_AgentName.Size = New System.Drawing.Size(160, 24)
        Me.txt_AgentName.TabIndex = 1
        Me.txt_AgentName.Text = "UltraTextEditor1"
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
        Me.Panel1.Location = New System.Drawing.Point(0, 382)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(693, 33)
        Me.Panel1.TabIndex = 2
        '
        'btnSave
        '
        Appearance5.FontData.BoldAsString = "True"
        Me.btnSave.Appearance = Appearance5
        Me.btnSave.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnSave.Location = New System.Drawing.Point(489, 0)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(68, 33)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save"
        '
        'btnCancel
        '
        Appearance6.FontData.BoldAsString = "True"
        Me.btnCancel.Appearance = Appearance6
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnCancel.Location = New System.Drawing.Point(557, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(68, 33)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        '
        'btnOK
        '
        Appearance7.FontData.BoldAsString = "True"
        Me.btnOK.Appearance = Appearance7
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnOK.Location = New System.Drawing.Point(625, 0)
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
        Me.UltraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UltraTabControl1.Location = New System.Drawing.Point(0, 86)
        Me.UltraTabControl1.Name = "UltraTabControl1"
        Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
        Me.UltraTabControl1.Size = New System.Drawing.Size(693, 296)
        Me.UltraTabControl1.TabIndex = 1
        UltraTab1.TabPage = Me.UltraTabPageControl1
        UltraTab1.Text = "Shares"
        UltraTab2.TabPage = Me.UltraTabPageControl2
        UltraTab2.Text = "Workspaces"
        Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2})
        '
        'UltraTabSharedControlsPage1
        '
        Me.UltraTabSharedControlsPage1.Location = New System.Drawing.Point(-10000, -10000)
        Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
        Me.UltraTabSharedControlsPage1.Size = New System.Drawing.Size(689, 270)
        '
        'frmFileAgent
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "File Agent"
        Me.ClientSize = New System.Drawing.Size(693, 415)
        Me.Controls.Add(Me.UltraTabControl1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.UltraPanel1)
        Me.Name = "frmFileAgent"
        Me.Text = "File Agent"
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabPageControl1.ResumeLayout(False)
        CType(Me.UltraGridShare, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ClientArea.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.UltraTabPageControl2.ResumeLayout(False)
        CType(Me.UltraGridWork, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraPanel2.ClientArea.ResumeLayout(False)
        Me.UltraPanel2.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.ResumeLayout(False)
        Me.UltraPanel1.ClientArea.PerformLayout()
        Me.UltraPanel1.ResumeLayout(False)
        CType(Me.txt_SecretKey, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txt_AgentName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ClientArea.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.UltraTabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents UltraPanel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents txt_SecretKey As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents txt_AgentName As Infragistics.Win.UltraWinEditors.UltraTextEditor
    Friend WithEvents UltraLabel2 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents UltraLabel1 As Infragistics.Win.Misc.UltraLabel
    Friend WithEvents btnSave As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnCancel As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnOK As Infragistics.Win.Misc.UltraButton
    Friend WithEvents Panel1 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents UltraTabPageControl1 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridShare As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents Panel4 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDelShare As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddShare As Infragistics.Win.Misc.UltraButton
    Friend WithEvents UltraTabPageControl2 As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents UltraGridWork As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents UltraPanel2 As Infragistics.Win.Misc.UltraPanel
    Friend WithEvents btnDelWS As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnAddWS As Infragistics.Win.Misc.UltraButton
    Friend WithEvents btnEditWS As Infragistics.Win.Misc.UltraButton

#End Region
End Class

