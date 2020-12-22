<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadTgaTextureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.out_tb = New System.Windows.Forms.TextBox()
        Me.PB1 = New System.Windows.Forms.Panel()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.LOADSRTToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.LoadTgaTextureToolStripMenuItem, Me.LOADSRTToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(871, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(76, 20)
        Me.OpenToolStripMenuItem.Text = "LOAD STM"
        '
        'LoadTgaTextureToolStripMenuItem
        '
        Me.LoadTgaTextureToolStripMenuItem.Name = "LoadTgaTextureToolStripMenuItem"
        Me.LoadTgaTextureToolStripMenuItem.Size = New System.Drawing.Size(113, 20)
        Me.LoadTgaTextureToolStripMenuItem.Text = "LOAD TGA IMAGE"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.out_tb)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.Black
        Me.SplitContainer1.Panel2.Controls.Add(Me.PB1)
        Me.SplitContainer1.Size = New System.Drawing.Size(871, 479)
        Me.SplitContainer1.SplitterDistance = 233
        Me.SplitContainer1.TabIndex = 1
        '
        'out_tb
        '
        Me.out_tb.BackColor = System.Drawing.Color.Black
        Me.out_tb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.out_tb.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.out_tb.ForeColor = System.Drawing.Color.White
        Me.out_tb.Location = New System.Drawing.Point(0, 0)
        Me.out_tb.Multiline = True
        Me.out_tb.Name = "out_tb"
        Me.out_tb.Size = New System.Drawing.Size(233, 479)
        Me.out_tb.TabIndex = 0
        '
        'PB1
        '
        Me.PB1.BackColor = System.Drawing.Color.Black
        Me.PB1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PB1.Location = New System.Drawing.Point(0, 0)
        Me.PB1.Name = "PB1"
        Me.PB1.Size = New System.Drawing.Size(634, 479)
        Me.PB1.TabIndex = 0
        '
        'Timer1
        '
        Me.Timer1.Interval = 200
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'LOADSRTToolStripMenuItem
        '
        Me.LOADSRTToolStripMenuItem.Name = "LOADSRTToolStripMenuItem"
        Me.LOADSRTToolStripMenuItem.Size = New System.Drawing.Size(71, 20)
        Me.LOADSRTToolStripMenuItem.Text = "LOAD SRT"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(871, 503)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmMain"
        Me.Text = "SRT loader"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents glControl_main As OpenTK.GLControl
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents out_tb As System.Windows.Forms.TextBox
    Friend WithEvents PB1 As System.Windows.Forms.Panel
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents LoadTgaTextureToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LOADSRTToolStripMenuItem As ToolStripMenuItem
End Class
