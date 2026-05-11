using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class CoordinatorMenuForm : Form
    {
        private string userEmail;
        private string userFirstName;

        // КОНСТРУКТОР
        public CoordinatorMenuForm(string email, string firstName)
        {
            userEmail = email;
            userFirstName = firstName;

            this.Text = "Меню координатора";
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = $"Добро пожаловать, координатор {firstName}!",
                Location = new Point(50, 30),
                Size = new Size(550, 40),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblTitle);

            // Информация об email
            Label lblEmail = new Label
            {
                Text = $"Email: {email}",
                Location = new Point(50, 80),
                Size = new Size(400, 25),
                Font = new Font("Arial", 11)
            };
            this.Controls.Add(lblEmail);

            // КНОПКА 1: Управление бегунами (НОВАЯ)
            Button btnManageRunners = new Button
            {
                Text = "👥 Управление бегунами",
                Location = new Point(50, 140),
                Size = new Size(220, 45),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            btnManageRunners.Click += (s, e) => new ManageRunnersForm().ShowDialog();
            this.Controls.Add(btnManageRunners);

            // КНОПКА 2: Регистрация на марафон
            Button btnRegisterEvent = new Button
            {
                Text = "📝 Регистрация на марафон",
                Location = new Point(50, 200),
                Size = new Size(220, 45),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 11)
            };
            btnRegisterEvent.Click += (s, e) => new RegisterForEventForm().ShowDialog();
            this.Controls.Add(btnRegisterEvent);

            // КНОПКА 3: Мои результаты
            Button btnMyResults = new Button
            {
                Text = "🏆 Мои результаты",
                Location = new Point(50, 260),
                Size = new Size(220, 45),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 11)
            };
            btnMyResults.Click += (s, e) => new MyResultsForm(userEmail).ShowDialog();
            this.Controls.Add(btnMyResults);

            // КНОПКА 4: Редактировать профиль
            Button btnEditProfile = new Button
            {
                Text = "✏️ Редактировать профиль",
                Location = new Point(50, 320),
                Size = new Size(220, 45),
                BackColor = Color.LightYellow,
                Font = new Font("Arial", 11)
            };
            btnEditProfile.Click += (s, e) => { new EditProfileForm(userEmail).ShowDialog(); };
            this.Controls.Add(btnEditProfile);

            // КНОПКА 5: BMI калькулятор
            Button btnBMI = new Button
            {
                Text = "⚖️ BMI калькулятор",
                Location = new Point(50, 380),
                Size = new Size(220, 45),
                BackColor = Color.LightCyan,
                Font = new Font("Arial", 11)
            };
            btnBMI.Click += (s, e) => new FormBMI().ShowDialog();
            this.Controls.Add(btnBMI);

            // КНОПКА 6: Выход
            Button btnLogout = new Button
            {
                Text = "🚪 Выход",
                Location = new Point(50, 450),
                Size = new Size(120, 40),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 11)
            };
            btnLogout.Click += (s, e) => { this.Close(); new MainForm().Show(); };
            this.Controls.Add(btnLogout);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "CoordinatorMenuForm";
            this.Load += new System.EventHandler(this.CoordinatorMenuForm_Load);
            this.ResumeLayout(false);
        }

        private void CoordinatorMenuForm_Load(object sender, EventArgs e)
        {
        }
    }
}