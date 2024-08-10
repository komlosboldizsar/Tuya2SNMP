using BToolbox.GUI.Helpers;
using BToolbox.GUI.Tables;
using BToolbox.Model;
using Tuya2SNMP.Logger;

namespace Tuya2SNMP
{
    internal partial class MainForm : Form
    {

        private readonly Config config;
        private readonly string parsingError;
        private readonly bool hideOnStartup;
        private bool hidingOnStartup;

        public MainForm() => InitializeComponent();

        public MainForm(Config config, string parsingError, bool hideOnStartup)
        {
            InitializeComponent();
            LogDispatcher.NewLogMessage += newLogMessageHandler;
            this.config = config;
            this.parsingError = parsingError;
            this.hideOnStartup = hideOnStartup;
            lastWindowStateNotMinimized = WindowState;
            Resize += MainForm_Resize;
            if (hideOnStartup)
            {
                hidingOnStartup = true;
                ShowInTaskbar = false;
                Opacity = 0;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            reloadLogMessages();
            string errorToShow = parsingError;
            if ((errorToShow == null) && (config == null))
                errorToShow = "Couldn't load configuration, reason unknown.";
            if (errorToShow != null)
            {
                MessageBox.Show(errorToShow, "Initialization error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            initDevicesTable();
            if (hideOnStartup)
            {
                Hide();
                Opacity = 1;
                ShowInTaskbar = true;
                hidingOnStartup = false;
            }
        }

        #region Devices table
        private void initDevicesTable()
        {
            Padding devicesTableMargin = devicesTable.Margin;
            mainTableLayoutPanel.Controls.Remove(devicesTable);
            if (!devicesTable.IsDisposed)
                devicesTable.Dispose();

            CustomDataGridView<Device> devicesTableT = new();
            devicesTable = devicesTableT;
            mainTableLayoutPanel.Controls.Add(devicesTable, 0, 0);
            devicesTable.Dock = DockStyle.Fill;
            devicesTable.Margin = devicesTableMargin;
            //devicesTable.InvokeSafe = true;

            CustomDataGridViewColumnDescriptorBuilder<Device> builder;

            builder = new();
            builder.Type(DataGridViewColumnType.TextBox);
            builder.Header("Index");
            builder.Width(100);
            builder.UpdaterMethod((item, cell) => { cell.Value = item.Index; });
            builder.BuildAndAdd(devicesTableT);

            builder = new();
            builder.Type(DataGridViewColumnType.TextBox);
            builder.Header("Name");
            builder.Width(150);
            builder.UpdaterMethod((item, cell) => { cell.Value = item.Name; });
            builder.BuildAndAdd(devicesTableT);

            builder = new();
            builder.Type(DataGridViewColumnType.TextBox);
            builder.Header("Type");
            builder.Width(250);
            builder.UpdaterMethod((item, cell) => { cell.Value = item.Type; });
            builder.BuildAndAdd(devicesTableT);

            builder = new();
            builder.Type(DataGridViewColumnType.TextBox);
            builder.Header("IP address");
            builder.Width(150);
            builder.UpdaterMethod((item, cell) => { cell.Value = item.IP; });
            builder.BuildAndAdd(devicesTableT);

            ObservableList<Device> configDevicesListProxy = new();
            configDevicesListProxy.AddRange(config.Devices);
            devicesTableT.BoundCollection = configDevicesListProxy;

        }
        #endregion

        #region Windows state handlers
        protected override bool ShowWithoutActivation => hidingOnStartup;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*if (oneInstanceMode && !closingFromTrayMenu)
            {*/
            e.Cancel = true;
            Hide();
            //}
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
            => Show();

        private void trayMenuExit_Click(object sender, EventArgs e)
        {
            closingFromTrayMenu = true;
            Close();
        }

        private bool closingFromTrayMenu = false;

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                lastWindowStateNotMinimized = WindowState;
        }

        private FormWindowState lastWindowStateNotMinimized;
        #endregion

        #region Logging
        private void newLogMessageHandler(DateTime Timestamp, LogMessageSeverity severity, string message)
            => logTextBox.InvokeIfRequired(() => addLogMessage(Timestamp, severity, message));

        private void addLogMessage(DateTime timestamp, LogMessageSeverity severity, string message)
        {
            if ((severity > MAX_LOG_MSG_SEVERITY_VERBOSE) || (!_verboseLog && (severity > MAX_LOG_MSG_SEVERITY_NORMAL)))
                return;
            string textToAdd = $"[{timestamp:HH:mm:ss}] {message}\r\n";
            logTextBox.AppendText(textToAdd);
            int textLength = logTextBox.TextLength;
            int selectionLength = textToAdd.Length;
            int selectionStart = textLength - selectionLength + 1;
            if (selectionStart < 0)
            {
                selectionStart = 0;
                selectionLength = 0;
            }
            logTextBox.Select(selectionStart, selectionLength);
            logTextBox.SelectionColor = logColors[severity];
            logTextBoxScrollToEnd();
        }

        private const LogMessageSeverity MAX_LOG_MSG_SEVERITY_NORMAL = LogMessageSeverity.Info;
        private const LogMessageSeverity MAX_LOG_MSG_SEVERITY_VERBOSE = LogMessageSeverity.Verbose;

        private void logTextBox_Resize(object sender, EventArgs e)
            => logTextBoxScrollToEnd();

        private void logTextBoxScrollToEnd()
        {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private bool _verboseLog = false;
        private void verboseLogSwitchLabel_Click(object sender, EventArgs e)
        {
            _verboseLog = !_verboseLog;
            verboseLogSwitchLabel.Text = "verbose log: " + (_verboseLog ? "on" : "off");
            verboseLogSwitchLabel.ForeColor = _verboseLog ? Color.Red : SystemColors.ControlText;
            reloadLogMessages();
        }

        private void reloadLogMessages()
        {
            logTextBox.Text = string.Empty;
            List<LogMessage> messages = new(LogDispatcher.Messages);
            foreach (LogMessage logMessage in messages)
                addLogMessage(logMessage.Timestamp, logMessage.Severity, logMessage.Message);
        }

        private static readonly Dictionary<LogMessageSeverity, Color> logColors = new()
        {
            { LogMessageSeverity.Error, Color.Red },
            { LogMessageSeverity.Warning, Color.Orange },
            { LogMessageSeverity.Info, Color.Black },
            { LogMessageSeverity.Verbose, Color.Blue },
            { LogMessageSeverity.VerbosePlus, Color.BlueViolet }
        };
        #endregion

        #region Statusbar
        private static void openUrl(string url)
            => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });

        private void githubLink_Click(object sender, EventArgs e) => openUrl(URL_GITHUB);
        private void toolStripStatusLabel1_Click(object sender, EventArgs e) => openUrl(URL_GITHUB_ISSUEREPORT);

        private const string URL_GITHUB = @"http://github.com/komlosboldizsar/Tuya2SNMP";
        private const string URL_GITHUB_ISSUEREPORT = @"http://github.com/komlosboldizsar/Tuya2SNMP/issues/new";
        #endregion

    }
}