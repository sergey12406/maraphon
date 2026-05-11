using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class MainForm : Form
    {

        private DateTime raceStart = new DateTime(2017, 11, 24, 6, 0, 0);
        private Timer countdownTimer;
        private Label lblCountdown;

        public MainForm()
        {

            this.Text = "Marathon Skills 2016";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Button btnLogin = new Button { Text = "Вход", Location = new Point(50, 50), Size = new Size(250, 50) };
            Button btnCheckRunner = new Button { Text = "Проверка бегуна", Location = new Point(50, 120), Size = new Size(250, 50) };
            Button btnInfo = new Button { Text = "О марафоне", Location = new Point(50, 190), Size = new Size(250, 50) };
            Button btnAbout = new Button { Text = "О системе", Location = new Point(50, 260), Size = new Size(250, 50) };

            btnLogin.Click += BtnLogin_Click;
            btnCheckRunner.Click += BtnCheckRunner_Click;
            btnInfo.Click += BtnInfo_Click;
            btnAbout.Click += BtnAbout_Click;

            this.Controls.Add(btnLogin);
            this.Controls.Add(btnCheckRunner);
            this.Controls.Add(btnInfo);
            this.Controls.Add(btnAbout);

            lblCountdown = new Label
            {
                Location = new Point(50, 350),
                Size = new Size(800, 40),
                Font = new Font("Arial", 14),
                ForeColor = Color.Red
            };
            this.Controls.Add(lblCountdown);

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
            UpdateCountdown();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            UpdateCountdown();
        }

        private void UpdateCountdown()
        {
            TimeSpan remaining = raceStart - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
                lblCountdown.Text = "МАРАФОН НАЧАЛСЯ!";
            else
                lblCountdown.Text = $"До старта: {remaining.Days} дн. {remaining.Hours} ч. {remaining.Minutes} мин. {remaining.Seconds} сек.";
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.ShowDialog();
        }

        private void BtnCheckRunner_Click(object sender, EventArgs e)
        {
            CheckRunnerForm check = new CheckRunnerForm();
            check.ShowDialog();
        }

        private void BtnInfo_Click(object sender, EventArgs e)
        {
            InfoForm info = new InfoForm();
            info.ShowDialog();
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


    }      
}
    
