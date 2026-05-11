using System;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class LoginForm : Form
    {
        private TextBox txtEmail, txtPassword;
        private Label lblStatus;

        public LoginForm()
        {
            this.Text = "Авторизация";
            this.Size = new Size(450, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 50;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "Вход в систему",
                Location = new Point(150, 20),
                Size = new Size(150, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Email
            Label lblEmail = new Label { Text = "Email:", Location = new Point(50, y), Size = new Size(80, 30), Font = new Font("Arial", 11) };
            txtEmail = new TextBox { Location = new Point(140, y), Size = new Size(220, 30) };
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            y += 50;

            // Пароль
            Label lblPass = new Label { Text = "Пароль:", Location = new Point(50, y), Size = new Size(80, 30), Font = new Font("Arial", 11) };
            txtPassword = new TextBox { Location = new Point(140, y), Size = new Size(220, 30), PasswordChar = '*', UseSystemPasswordChar = true };
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);
            y += 60;

            // Статус
            lblStatus = new Label
            {
                Text = "",
                Location = new Point(50, y),
                Size = new Size(350, 30),
                ForeColor = Color.Red,
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(lblStatus);
            y += 40;

            // Кнопки
            Button btnLogin = new Button { Text = "Войти", Location = new Point(140, y), Size = new Size(100, 40), BackColor = Color.LightGreen, Font = new Font("Arial", 11) };
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 40) };
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnCancel);

            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Введите email и пароль!";
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT RoleId, FirstName, LastName FROM [User] WHERE Email = @Email AND [Password] = @Password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string role = reader["RoleId"].ToString();
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        reader.Close();

                        MessageBox.Show($"Добро пожаловать, {firstName} {lastName}!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();

                        // Открываем меню в зависимости от роли
                        if (role == "R")
                        {
                            RunnerMenuForm runnerMenu = new RunnerMenuForm(email, firstName);
                            runnerMenu.ShowDialog();
                        }
                        else if (role == "C")
                        {
                            CoordinatorMenuForm coordMenu = new CoordinatorMenuForm(email, firstName);
                            coordMenu.ShowDialog();
                        }
                        else if (role == "A")
                        {
                            AdminMenuForm adminMenu = new AdminMenuForm(email, firstName);
                            adminMenu.ShowDialog();
                        }

                        this.Close();
                    }
                    else
                    {
                        lblStatus.Text = "Неверный email или пароль!";
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Ошибка: " + ex.Message;
            }
        }
    }
}