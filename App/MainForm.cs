namespace App
{
    using static Data.AutoKeylight;
    using static Data.KeylightApi;

    public partial class MainForm : Form
    {
        private readonly AutoKeylight autoKeylight;
        private static MainForm? instance;

        public MainForm()
        {
            InitializeComponent();
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 450, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            autoKeylight = new AutoKeylight();
            autoKeylight.OnCamEnable += new EventHandler<object>(OnCamEnable);
            autoKeylight.OnCamDisable += new EventHandler<object>(OnCamDisable);
            autoKeylight.Start();
            instance = this;
            tbKeylightIp.Text = (string)Properties.Settings.Default["KeylightIp"];
        }

        private void Unminimize()
        {
            Show();
            Focus();
            TopMost = true;
        }

        private void Minimize()
        {
            TopMost = false;

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            Hide();
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Unminimize();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Minimize();
        }
        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            Minimize();
        }

        private static void TrySwitchKeylight(bool on)
        {
            if (instance == null) return;
            if (instance.tbKeylightIp.Text == "")
            {
                instance.Invoke(new MethodInvoker(delegate ()
                {
                    instance.lbError.Text = "Please enter Keylight IP.";
                    instance.Unminimize();
                }));
            }
            else 
            {
                try
                {
                    switchKeylight(instance.tbKeylightIp.Text, on);
                }
                catch
                {
                    instance.Invoke(new MethodInvoker(delegate ()
                    {
                        instance.lbError.Text = "Unable to reach Keylight, please check the IP.";
                        instance.Unminimize();
                    }));
                }
            }
        }

        private static void OnCamEnable(object? sender, object _)
        {
            if (instance == null) return;

            instance.Invoke(new MethodInvoker(delegate ()
            {
                instance.lbWebcamStatus.Text = "Enabled";
                instance.lbWebcamStatus.ForeColor = Color.Green;
            }));

            TrySwitchKeylight(true);
        }

        private static void OnCamDisable(object? sender, object _)
        {
            if (instance == null) return;

            instance.Invoke(new MethodInvoker(delegate ()
            {
                instance.lbWebcamStatus.Text = "Disabled";
                instance.lbWebcamStatus.ForeColor = Color.Red;
            }));

            TrySwitchKeylight(false);
        }

        private void tbKeylightIp_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["KeylightIp"] = tbKeylightIp.Text;
            Properties.Settings.Default.Save();
            lbError.Text = "";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon.Visible = false;
        }
    }
}