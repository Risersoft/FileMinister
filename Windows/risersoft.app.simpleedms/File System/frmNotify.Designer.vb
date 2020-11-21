Imports Infragistics.Win.UltraWinGrid
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmNotify
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
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.HtmluiControl1 = New Syncfusion.Windows.Forms.HTMLUI.HTMLUIControl()
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.HtmluiControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'HtmluiControl1
        '
        Me.HtmluiControl1.AutoScroll = True
        Me.HtmluiControl1.AutoScrollMinSize = New System.Drawing.Size(655, 471)
        Me.HtmluiControl1.DefaultFormat.BackgroundColor = System.Drawing.SystemColors.Control
        Me.HtmluiControl1.DefaultFormat.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HtmluiControl1.DefaultFormat.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HtmluiControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HtmluiControl1.Location = New System.Drawing.Point(0, 0)
        Me.HtmluiControl1.Name = "HtmluiControl1"
        Me.HtmluiControl1.ShowTitle = False
        Me.HtmluiControl1.Size = New System.Drawing.Size(672, 486)
        Me.HtmluiControl1.TabIndex = 1
        Me.HtmluiControl1.Text = "HtmluiControl1"
        '
        'frmNotify
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.Caption = "Document template"
        Me.ClientSize = New System.Drawing.Size(672, 486)
        Me.Controls.Add(Me.HtmluiControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmNotify"
        Me.Text = "Document template"
        CType(Me.dsForm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsCombo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.HtmluiControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents HtmluiControl1 As Syncfusion.Windows.Forms.HTMLUI.HTMLUIControl

#End Region
End Class

