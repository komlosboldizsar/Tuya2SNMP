namespace Tuya2SNMP
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            mainTableLayoutPanel = new TableLayoutPanel();
            logTextBox = new RichTextBox();
            devicesTable = new DataGridView();
            statusStrip = new StatusStrip();
            githubLink = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            statusStripSpring = new ToolStripStatusLabel();
            verboseLogSwitchLabel = new ToolStripStatusLabel();
            trayIcon = new NotifyIcon(components);
            trayMenu = new ContextMenuStrip(components);
            trayMenuExit = new ToolStripMenuItem();
            panel1.SuspendLayout();
            mainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)devicesTable).BeginInit();
            statusStrip.SuspendLayout();
            trayMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(mainTableLayoutPanel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 424);
            panel1.TabIndex = 0;
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 1;
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            mainTableLayoutPanel.Controls.Add(logTextBox, 0, 1);
            mainTableLayoutPanel.Controls.Add(devicesTable, 0, 0);
            mainTableLayoutPanel.Dock = DockStyle.Fill;
            mainTableLayoutPanel.Location = new Point(0, 0);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowCount = 2;
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayoutPanel.Size = new Size(800, 424);
            mainTableLayoutPanel.TabIndex = 0;
            // 
            // logTextBox
            // 
            logTextBox.Dock = DockStyle.Fill;
            logTextBox.Location = new Point(10, 222);
            logTextBox.Margin = new Padding(10);
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.Size = new Size(780, 192);
            logTextBox.TabIndex = 0;
            logTextBox.Text = "";
            logTextBox.Resize += logTextBox_Resize;
            // 
            // devicesTable
            // 
            devicesTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            devicesTable.Dock = DockStyle.Fill;
            devicesTable.Location = new Point(10, 10);
            devicesTable.Margin = new Padding(10, 10, 10, 0);
            devicesTable.Name = "devicesTable";
            devicesTable.RowHeadersWidth = 51;
            devicesTable.RowTemplate.Height = 29;
            devicesTable.Size = new Size(780, 202);
            devicesTable.TabIndex = 1;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { githubLink, toolStripStatusLabel1, statusStripSpring, verboseLogSwitchLabel });
            statusStrip.Location = new Point(0, 424);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 26);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // githubLink
            // 
            githubLink.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            githubLink.ForeColor = Color.Blue;
            githubLink.Name = "githubLink";
            githubLink.Size = new Size(284, 20);
            githubLink.Text = "github.com/komlosboldizsar/Tuya2SNMP";
            githubLink.Click += githubLink_Click;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
            toolStripStatusLabel1.ForeColor = Color.Blue;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Padding = new Padding(5, 0, 0, 0);
            toolStripStatusLabel1.Size = new Size(91, 20);
            toolStripStatusLabel1.Text = "report issue";
            toolStripStatusLabel1.Click += toolStripStatusLabel1_Click;
            // 
            // statusStripSpring
            // 
            statusStripSpring.Name = "statusStripSpring";
            statusStripSpring.Size = new Size(258, 20);
            statusStripSpring.Spring = true;
            // 
            // verboseLogSwitchLabel
            // 
            verboseLogSwitchLabel.Name = "verboseLogSwitchLabel";
            verboseLogSwitchLabel.Size = new Size(113, 20);
            verboseLogSwitchLabel.Text = "verbose log: off";
            verboseLogSwitchLabel.Click += verboseLogSwitchLabel_Click;
            // 
            // trayIcon
            // 
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Text = "Tuya2SNMP";
            trayIcon.Visible = true;
            trayIcon.DoubleClick += trayIcon_DoubleClick;
            // 
            // trayMenu
            // 
            trayMenu.ImageScalingSize = new Size(20, 20);
            trayMenu.Items.AddRange(new ToolStripItem[] { trayMenuExit });
            trayMenu.Name = "trayMenu";
            trayMenu.Size = new Size(103, 28);
            // 
            // trayMenuExit
            // 
            trayMenuExit.Name = "trayMenuExit";
            trayMenuExit.Size = new Size(102, 24);
            trayMenuExit.Text = "Exit";
            trayMenuExit.Click += trayMenuExit_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(statusStrip);
            Name = "MainForm";
            Text = "Tuya2SNMP";
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            panel1.ResumeLayout(false);
            mainTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)devicesTable).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            trayMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private TableLayoutPanel mainTableLayoutPanel;
        private RichTextBox logTextBox;
        private DataGridView devicesTable;
        private StatusStrip statusStrip;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem trayMenuExit;
        private ToolStripStatusLabel githubLink;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel statusStripSpring;
        private ToolStripStatusLabel verboseLogSwitchLabel;
    }
}