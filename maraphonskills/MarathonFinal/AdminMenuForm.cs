using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class AdminMenuForm : Form
    {
        public AdminMenuForm(string email, string firstName)
        {
            this.Text = "Меню администратора";
            this.Size = new Size(650, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label
            {
                Text = $"Добро пожаловать, администратор {firstName}!",
                Location = new Point(50, 30),
                Size = new Size(500, 40),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblTitle);

            Button btnManageUsers = new Button
            {
                Text = "Управление пользователями",
                Location = new Point(50, 140),
                Size = new Size(220, 45),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 11)
            };
            btnManageUsers.Click += (s, e) => MessageBox.Show("Форма будет в Сессии 3", "В разработке");
            this.Controls.Add(btnManageUsers);

            Button btnLogout = new Button
            {
                Text = "Выход",
                Location = new Point(50, 340),
                Size = new Size(120, 40),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 11)
            };
            btnLogout.Click += (s, e) => { this.Close(); new MainForm().Show(); };
            this.Controls.Add(btnLogout);
        }
    }
}