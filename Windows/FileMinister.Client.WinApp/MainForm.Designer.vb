<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits frmMaxApi

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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.DirectorySearcher1 = New System.DirectoryServices.DirectorySearcher()
        Me.cmsMain = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ssMain = New System.Windows.Forms.StatusStrip()
        Me.tsslConnectionStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsProgressBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.tsslStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.txtbreadcrum = New System.Windows.Forms.TextBox()
        Me.txt_search = New System.Windows.Forms.TextBox()
        Me.btnAdvancedSearch = New System.Windows.Forms.Button()
        Me.btnsearch = New System.Windows.Forms.Button()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.pnlNav = New System.Windows.Forms.Panel()
        Me.btnConfigureShare = New System.Windows.Forms.Button()
        Me.lblMessageLeft = New System.Windows.Forms.Label()
        Me.tvDriveExplorer = New FileMinister.Client.WinApp.FileMinisterTreeView()
        Me.cmsTree = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnlDetailTop = New System.Windows.Forms.Panel()
        Me.lblAddAgent = New System.Windows.Forms.Label()
        Me.lblAddShare = New System.Windows.Forms.Label()
        Me.lblMessageRight = New System.Windows.Forms.Label()
        Me.lvItems = New FileMinister.Client.WinApp.FileMinisterListView()
        Me.msMain = New System.Windows.Forms.MenuStrip()
        Me.tsmiHome = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiRefresh = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiNewFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiCheckIn = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiCheckout = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiUndoCheckout = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiConfig = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiHistory = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.HistoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiLink = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiClipboard = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiCut = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiAdmin = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiShares = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiAgents = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiReports = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiUser = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiSwitchUser = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiSwitchAccount = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiAddAgent = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogOutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ssMain.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        Me.pnlNav.SuspendLayout()
        Me.cmsTree.SuspendLayout()
        Me.pnlDetailTop.SuspendLayout()
        Me.msMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'DirectorySearcher1
        '
        Me.DirectorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01")
        Me.DirectorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01")
        Me.DirectorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01")
        '
        'cmsMain
        '
        Me.cmsMain.Name = "cmsMain"
        Me.cmsMain.Size = New System.Drawing.Size(61, 4)
        '
        'ssMain
        '
        Me.ssMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsslConnectionStatus, Me.ToolStripStatusLabel1, Me.tsProgressBar, Me.tsslStatus})
        Me.ssMain.Location = New System.Drawing.Point(0, 643)
        Me.ssMain.Name = "ssMain"
        Me.ssMain.Size = New System.Drawing.Size(1027, 23)
        Me.ssMain.TabIndex = 14
        '
        'tsslConnectionStatus
        '
        Me.tsslConnectionStatus.Name = "tsslConnectionStatus"
        Me.tsslConnectionStatus.Size = New System.Drawing.Size(0, 18)
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(910, 18)
        Me.ToolStripStatusLabel1.Spring = True
        '
        'tsProgressBar
        '
        Me.tsProgressBar.Name = "tsProgressBar"
        Me.tsProgressBar.Size = New System.Drawing.Size(100, 17)
        Me.tsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'tsslStatus
        '
        Me.tsslStatus.Name = "tsslStatus"
        Me.tsslStatus.Size = New System.Drawing.Size(0, 18)
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 41.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.SplitContainer3, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnAdvancedSearch, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnsearch, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SplitContainer4, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 24)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1027, 619)
        Me.TableLayoutPanel1.TabIndex = 15
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.txtbreadcrum)
        Me.SplitContainer3.Panel1MinSize = 10
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.txt_search)
        Me.SplitContainer3.Panel2MinSize = 10
        Me.SplitContainer3.Size = New System.Drawing.Size(855, 23)
        Me.SplitContainer3.SplitterDistance = 573
        Me.SplitContainer3.SplitterWidth = 1
        Me.SplitContainer3.TabIndex = 0
        '
        'txtbreadcrum
        '
        Me.txtbreadcrum.BackColor = System.Drawing.Color.White
        Me.txtbreadcrum.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtbreadcrum.Location = New System.Drawing.Point(0, 0)
        Me.txtbreadcrum.Name = "txtbreadcrum"
        Me.txtbreadcrum.ReadOnly = True
        Me.txtbreadcrum.Size = New System.Drawing.Size(573, 20)
        Me.txtbreadcrum.TabIndex = 0
        '
        'txt_search
        '
        Me.txt_search.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txt_search.Location = New System.Drawing.Point(0, 0)
        Me.txt_search.Name = "txt_search"
        Me.txt_search.Size = New System.Drawing.Size(281, 20)
        Me.txt_search.TabIndex = 0
        '
        'btnAdvancedSearch
        '
        Me.btnAdvancedSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnAdvancedSearch.Location = New System.Drawing.Point(905, 3)
        Me.btnAdvancedSearch.Name = "btnAdvancedSearch"
        Me.btnAdvancedSearch.Size = New System.Drawing.Size(119, 23)
        Me.btnAdvancedSearch.TabIndex = 2
        Me.btnAdvancedSearch.Text = "Advanced Search"
        Me.btnAdvancedSearch.UseVisualStyleBackColor = True
        '
        'btnsearch
        '
        Me.btnsearch.AutoSize = True
        Me.btnsearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnsearch.Image = CType(resources.GetObject("btnsearch.Image"), System.Drawing.Image)
        Me.btnsearch.Location = New System.Drawing.Point(864, 3)
        Me.btnsearch.Name = "btnsearch"
        Me.btnsearch.Size = New System.Drawing.Size(35, 23)
        Me.btnsearch.TabIndex = 1
        Me.btnsearch.UseVisualStyleBackColor = True
        '
        'SplitContainer4
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.SplitContainer4, 3)
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.Location = New System.Drawing.Point(3, 32)
        Me.SplitContainer4.Name = "SplitContainer4"
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.pnlNav)
        Me.SplitContainer4.Panel1.Controls.Add(Me.tvDriveExplorer)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.pnlDetailTop)
        Me.SplitContainer4.Panel2.Controls.Add(Me.lvItems)
        Me.SplitContainer4.Size = New System.Drawing.Size(1021, 584)
        Me.SplitContainer4.SplitterDistance = 193
        Me.SplitContainer4.SplitterWidth = 1
        Me.SplitContainer4.TabIndex = 1
        '
        'pnlNav
        '
        Me.pnlNav.Controls.Add(Me.btnConfigureShare)
        Me.pnlNav.Controls.Add(Me.lblMessageLeft)
        Me.pnlNav.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNav.Location = New System.Drawing.Point(0, 0)
        Me.pnlNav.Name = "pnlNav"
        Me.pnlNav.Size = New System.Drawing.Size(193, 584)
        Me.pnlNav.TabIndex = 8
        Me.pnlNav.Visible = False
        '
        'btnConfigureShare
        '
        Me.btnConfigureShare.BackColor = System.Drawing.Color.White
        Me.btnConfigureShare.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnConfigureShare.FlatAppearance.BorderSize = 0
        Me.btnConfigureShare.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConfigureShare.Location = New System.Drawing.Point(0, 0)
        Me.btnConfigureShare.Name = "btnConfigureShare"
        Me.btnConfigureShare.Size = New System.Drawing.Size(193, 584)
        Me.btnConfigureShare.TabIndex = 10
        Me.btnConfigureShare.Text = "Çonfigure Share"
        Me.btnConfigureShare.UseVisualStyleBackColor = False
        '
        'lblMessageLeft
        '
        Me.lblMessageLeft.BackColor = System.Drawing.Color.White
        Me.lblMessageLeft.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMessageLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblMessageLeft.Location = New System.Drawing.Point(0, 0)
        Me.lblMessageLeft.Name = "lblMessageLeft"
        Me.lblMessageLeft.Size = New System.Drawing.Size(193, 584)
        Me.lblMessageLeft.TabIndex = 9
        Me.lblMessageLeft.Text = "Loading..."
        Me.lblMessageLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tvDriveExplorer
        '
        Me.tvDriveExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tvDriveExplorer.ContextMenuStrip = Me.cmsTree
        Me.tvDriveExplorer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvDriveExplorer.FullRowSelect = True
        Me.tvDriveExplorer.HideSelection = False
        Me.tvDriveExplorer.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.tvDriveExplorer.Location = New System.Drawing.Point(0, 0)
        Me.tvDriveExplorer.Name = "tvDriveExplorer"
        Me.tvDriveExplorer.ShowNodeToolTips = True
        Me.tvDriveExplorer.Size = New System.Drawing.Size(193, 584)
        Me.tvDriveExplorer.TabIndex = 6
        '
        'cmsTree
        '
        Me.cmsTree.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RefreshToolStripMenuItem})
        Me.cmsTree.Name = "cmsTree"
        Me.cmsTree.Size = New System.Drawing.Size(114, 26)
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(113, 22)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'pnlDetailTop
        '
        Me.pnlDetailTop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlDetailTop.AutoSize = True
        Me.pnlDetailTop.BackColor = System.Drawing.Color.White
        Me.pnlDetailTop.ContextMenuStrip = Me.cmsMain
        Me.pnlDetailTop.Controls.Add(Me.lblAddAgent)
        Me.pnlDetailTop.Controls.Add(Me.lblAddShare)
        Me.pnlDetailTop.Controls.Add(Me.lblMessageRight)
        Me.pnlDetailTop.Location = New System.Drawing.Point(7, 38)
        Me.pnlDetailTop.Name = "pnlDetailTop"
        Me.pnlDetailTop.Size = New System.Drawing.Size(853, 108)
        Me.pnlDetailTop.TabIndex = 8
        Me.pnlDetailTop.Visible = False
        '
        'lblAddAgent
        '
        Me.lblAddAgent.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblAddAgent.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAddAgent.Location = New System.Drawing.Point(373, 70)
        Me.lblAddAgent.Name = "lblAddAgent"
        Me.lblAddAgent.Size = New System.Drawing.Size(115, 14)
        Me.lblAddAgent.TabIndex = 2
        Me.lblAddAgent.Text = "Select Agent"
        Me.lblAddAgent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblAddShare
        '
        Me.lblAddShare.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblAddShare.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAddShare.Location = New System.Drawing.Point(370, 39)
        Me.lblAddShare.Name = "lblAddShare"
        Me.lblAddShare.Size = New System.Drawing.Size(118, 14)
        Me.lblAddShare.TabIndex = 1
        Me.lblAddShare.Text = "Add Share"
        Me.lblAddShare.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblMessageRight
        '
        Me.lblMessageRight.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblMessageRight.Location = New System.Drawing.Point(0, 0)
        Me.lblMessageRight.Name = "lblMessageRight"
        Me.lblMessageRight.Size = New System.Drawing.Size(853, 30)
        Me.lblMessageRight.TabIndex = 0
        Me.lblMessageRight.Text = "This folder is empty"
        Me.lblMessageRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lvItems
        '
        Me.lvItems.AllowColumnReorder = True
        Me.lvItems.AllowDrop = True
        Me.lvItems.BackColor = System.Drawing.Color.White
        Me.lvItems.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lvItems.ContextMenuStrip = Me.cmsMain
        Me.lvItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvItems.FullRowSelect = True
        Me.lvItems.HideSelection = False
        Me.lvItems.Location = New System.Drawing.Point(0, 0)
        Me.lvItems.Name = "lvItems"
        Me.lvItems.ShowItemToolTips = True
        Me.lvItems.Size = New System.Drawing.Size(827, 584)
        Me.lvItems.TabIndex = 7
        Me.lvItems.UseCompatibleStateImageBehavior = False
        Me.lvItems.View = System.Windows.Forms.View.Details
        '
        'msMain
        '
        Me.msMain.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.msMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiHome, Me.tsmiHistory, Me.tsmiClipboard, Me.tsmiAdmin, Me.tsmiReports, Me.tsmiUser})
        Me.msMain.Location = New System.Drawing.Point(0, 0)
        Me.msMain.Name = "msMain"
        Me.msMain.Size = New System.Drawing.Size(1027, 24)
        Me.msMain.TabIndex = 16
        Me.msMain.Text = "MenuStrip1"
        '
        'tsmiHome
        '
        Me.tsmiHome.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiRefresh, Me.tsmiNewFolder, Me.tsmiCheckIn, Me.tsmiCheckout, Me.tsmiUndoCheckout, Me.tsmiConfig})
        Me.tsmiHome.Name = "tsmiHome"
        Me.tsmiHome.Size = New System.Drawing.Size(52, 20)
        Me.tsmiHome.Text = "&Home"
        '
        'tsmiRefresh
        '
        Me.tsmiRefresh.Name = "tsmiRefresh"
        Me.tsmiRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.tsmiRefresh.Size = New System.Drawing.Size(177, 22)
        Me.tsmiRefresh.Text = "&Refresh"
        '
        'tsmiNewFolder
        '
        Me.tsmiNewFolder.Name = "tsmiNewFolder"
        Me.tsmiNewFolder.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.tsmiNewFolder.Size = New System.Drawing.Size(177, 22)
        Me.tsmiNewFolder.Text = "&New Folder"
        '
        'tsmiCheckIn
        '
        Me.tsmiCheckIn.Name = "tsmiCheckIn"
        Me.tsmiCheckIn.Size = New System.Drawing.Size(177, 22)
        Me.tsmiCheckIn.Text = "Check&in"
        Me.tsmiCheckIn.Visible = False
        '
        'tsmiCheckout
        '
        Me.tsmiCheckout.Name = "tsmiCheckout"
        Me.tsmiCheckout.Size = New System.Drawing.Size(177, 22)
        Me.tsmiCheckout.Text = "Check&out"
        Me.tsmiCheckout.Visible = False
        '
        'tsmiUndoCheckout
        '
        Me.tsmiUndoCheckout.Name = "tsmiUndoCheckout"
        Me.tsmiUndoCheckout.Size = New System.Drawing.Size(177, 22)
        Me.tsmiUndoCheckout.Text = "&Undo Checkout"
        '
        'tsmiConfig
        '
        Me.tsmiConfig.Name = "tsmiConfig"
        Me.tsmiConfig.Size = New System.Drawing.Size(177, 22)
        Me.tsmiConfig.Text = "Confi&g"
        '
        'tsmiHistory
        '
        Me.tsmiHistory.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiProperties, Me.HistoryToolStripMenuItem, Me.tsmiLink})
        Me.tsmiHistory.Name = "tsmiHistory"
        Me.tsmiHistory.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.H), System.Windows.Forms.Keys)
        Me.tsmiHistory.Size = New System.Drawing.Size(44, 20)
        Me.tsmiHistory.Text = "&View"
        '
        'tsmiProperties
        '
        Me.tsmiProperties.Name = "tsmiProperties"
        Me.tsmiProperties.ShortcutKeys = System.Windows.Forms.Keys.F4
        Me.tsmiProperties.Size = New System.Drawing.Size(155, 22)
        Me.tsmiProperties.Text = "&Properties"
        '
        'HistoryToolStripMenuItem
        '
        Me.HistoryToolStripMenuItem.Name = "HistoryToolStripMenuItem"
        Me.HistoryToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.H), System.Windows.Forms.Keys)
        Me.HistoryToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.HistoryToolStripMenuItem.Text = "&History"
        '
        'tsmiLink
        '
        Me.tsmiLink.Name = "tsmiLink"
        Me.tsmiLink.Size = New System.Drawing.Size(155, 22)
        Me.tsmiLink.Text = "&Link"
        '
        'tsmiClipboard
        '
        Me.tsmiClipboard.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiCut, Me.tsmiCopy, Me.tsmiPaste, Me.tsmiDelete})
        Me.tsmiClipboard.Name = "tsmiClipboard"
        Me.tsmiClipboard.Size = New System.Drawing.Size(71, 20)
        Me.tsmiClipboard.Text = "Cli&pboard"
        '
        'tsmiCut
        '
        Me.tsmiCut.Name = "tsmiCut"
        Me.tsmiCut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.tsmiCut.Size = New System.Drawing.Size(152, 22)
        Me.tsmiCut.Text = "&Cut"
        '
        'tsmiCopy
        '
        Me.tsmiCopy.Name = "tsmiCopy"
        Me.tsmiCopy.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.tsmiCopy.Size = New System.Drawing.Size(152, 22)
        Me.tsmiCopy.Text = "C&opy"
        '
        'tsmiPaste
        '
        Me.tsmiPaste.Name = "tsmiPaste"
        Me.tsmiPaste.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.tsmiPaste.Size = New System.Drawing.Size(152, 22)
        Me.tsmiPaste.Text = "&Paste"
        '
        'tsmiDelete
        '
        Me.tsmiDelete.Name = "tsmiDelete"
        Me.tsmiDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.tsmiDelete.Size = New System.Drawing.Size(152, 22)
        Me.tsmiDelete.Text = "&Delete"
        '
        'tsmiAdmin
        '
        Me.tsmiAdmin.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiShares, Me.tsmiAgents})
        Me.tsmiAdmin.Name = "tsmiAdmin"
        Me.tsmiAdmin.Size = New System.Drawing.Size(55, 20)
        Me.tsmiAdmin.Text = "&Admin"
        '
        'tsmiShares
        '
        Me.tsmiShares.Name = "tsmiShares"
        Me.tsmiShares.Size = New System.Drawing.Size(152, 22)
        Me.tsmiShares.Text = "&Shares"
        '
        'tsmiAgents
        '
        Me.tsmiAgents.Name = "tsmiAgents"
        Me.tsmiAgents.Size = New System.Drawing.Size(152, 22)
        Me.tsmiAgents.Text = "&Agents"
        '
        'tsmiReports
        '
        Me.tsmiReports.Name = "tsmiReports"
        Me.tsmiReports.Size = New System.Drawing.Size(59, 20)
        Me.tsmiReports.Text = "&Reports"
        '
        'tsmiUser
        '
        Me.tsmiUser.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tsmiUser.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiSwitchUser, Me.tsmiSwitchAccount, Me.tsmiAddAgent, Me.LogOutToolStripMenuItem})
        Me.tsmiUser.Image = CType(resources.GetObject("tsmiUser.Image"), System.Drawing.Image)
        Me.tsmiUser.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.tsmiUser.Name = "tsmiUser"
        Me.tsmiUser.Size = New System.Drawing.Size(65, 20)
        Me.tsmiUser.Text = "Guest"
        '
        'tsmiSwitchUser
        '
        Me.tsmiSwitchUser.Name = "tsmiSwitchUser"
        Me.tsmiSwitchUser.Size = New System.Drawing.Size(157, 22)
        Me.tsmiSwitchUser.Text = "Switch &User"
        '
        'tsmiSwitchAccount
        '
        Me.tsmiSwitchAccount.Name = "tsmiSwitchAccount"
        Me.tsmiSwitchAccount.Size = New System.Drawing.Size(157, 22)
        Me.tsmiSwitchAccount.Text = "Switch &Account"
        '
        'tsmiAddAgent
        '
        Me.tsmiAddAgent.Name = "tsmiAddAgent"
        Me.tsmiAddAgent.Size = New System.Drawing.Size(157, 22)
        Me.tsmiAddAgent.Text = "Select A&gent"
        '
        'LogOutToolStripMenuItem
        '
        Me.LogOutToolStripMenuItem.Name = "LogOutToolStripMenuItem"
        Me.LogOutToolStripMenuItem.Size = New System.Drawing.Size(157, 22)
        Me.LogOutToolStripMenuItem.Text = "Log Out"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Caption = "Cloud Sync"
        Me.ClientSize = New System.Drawing.Size(1027, 666)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.ssMain)
        Me.Controls.Add(Me.msMain)
        Me.Name = "MainForm"
        Me.Text = "Cloud Sync"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.eBag, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ssMain.ResumeLayout(False)
        Me.ssMain.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.PerformLayout()
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        Me.SplitContainer4.Panel2.PerformLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.pnlNav.ResumeLayout(False)
        Me.cmsTree.ResumeLayout(False)
        Me.pnlDetailTop.ResumeLayout(False)
        Me.msMain.ResumeLayout(False)
        Me.msMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DirectorySearcher1 As System.DirectoryServices.DirectorySearcher
    Friend WithEvents cmsMain As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ssMain As StatusStrip
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents txtbreadcrum As TextBox
    Friend WithEvents txt_search As TextBox
    Friend WithEvents btnAdvancedSearch As Button
    Friend WithEvents btnsearch As Button
    Friend WithEvents SplitContainer4 As SplitContainer
    Friend WithEvents tvDriveExplorer As FileMinisterTreeView
    Friend WithEvents lvItems As FileMinisterListView
    Friend WithEvents pnlNav As Panel
    Friend WithEvents lblMessageLeft As Label
    Friend WithEvents btnConfigureShare As Button
    Friend WithEvents tsProgressBar As ToolStripProgressBar
    Friend WithEvents pnlDetailTop As Panel
    Friend WithEvents lblMessageRight As Label
    Friend WithEvents lblAddAgent As Label
    Friend WithEvents lblAddShare As Label
    Friend WithEvents msMain As MenuStrip
    Friend WithEvents tsmiHome As ToolStripMenuItem
    Friend WithEvents tsmiRefresh As ToolStripMenuItem
    Friend WithEvents tsmiNewFolder As ToolStripMenuItem
    Friend WithEvents tsmiHistory As ToolStripMenuItem
    Friend WithEvents tsmiProperties As ToolStripMenuItem
    Friend WithEvents HistoryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents tsmiLink As ToolStripMenuItem
    Friend WithEvents tsmiCheckIn As ToolStripMenuItem
    Friend WithEvents tsmiCheckout As ToolStripMenuItem
    Friend WithEvents tsmiConfig As ToolStripMenuItem
    Friend WithEvents tsmiUndoCheckout As ToolStripMenuItem
    Friend WithEvents tsmiClipboard As ToolStripMenuItem
    Friend WithEvents tsmiCut As ToolStripMenuItem
    Friend WithEvents tsmiAdmin As ToolStripMenuItem
    Friend WithEvents tsmiCopy As ToolStripMenuItem
    Friend WithEvents tsmiPaste As ToolStripMenuItem
    Friend WithEvents tsmiDelete As ToolStripMenuItem
    Friend WithEvents tsmiShares As ToolStripMenuItem
    Friend WithEvents tsmiAgents As ToolStripMenuItem
    Friend WithEvents tsmiReports As ToolStripMenuItem
    Friend WithEvents tsmiUser As ToolStripMenuItem
    Friend WithEvents tsmiSwitchUser As ToolStripMenuItem
    Friend WithEvents tsmiSwitchAccount As ToolStripMenuItem
    Friend WithEvents tsmiAddAgent As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents tsslStatus As ToolStripStatusLabel
    Friend WithEvents cmsTree As ContextMenuStrip
    Friend WithEvents RefreshToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LogOutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents tsslConnectionStatus As ToolStripStatusLabel
End Class
