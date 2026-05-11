using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class RegisterRunnerForm : Form
    {
        private TextBox txtFirstName, txtLastName, txtEmail, txtPassword, txtConfirmPassword;
        private ComboBox cbGender, cbCountry;
        private DateTimePicker dtpBirthDate;
        private PictureBox pbPhoto;
        private string photoPath = "";

        // ЭТО КОНСТРУКТОР - НЕ ДОЛЖНО БЫТЬ void или других модификаторов
        public RegisterRunnerForm()
        {
            this.Text = "Регистрация бегуна";
            this.Size = new Size(650, 750);
            this.StartPosition = FormStartPosition.CenterParent;

            int y = 30;
            int labelWidth = 120;
            int controlWidth = 280;
            int xLabel = 40;
            int xControl = 170;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "Регистрация нового бегуна",
                Location = new Point(150, 10),
                Size = new Size(350, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Поля
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Имя:*", out txtFirstName);
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Фамилия:*", out txtLastName);
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Email:*", out txtEmail);
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Пароль:*", out txtPassword);
            txtPassword.PasswordChar = '*';
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Повтор пароля:*", out txtConfirmPassword);
            txtConfirmPassword.PasswordChar = '*';

            // Пол
            Label lblGender = new Label { Text = "Пол:*", Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            cbGender = new ComboBox { Location = new Point(xControl, y), Size = new Size(controlWidth, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cbGender.Items.AddRange(new string[] { "Male", "Female" });
            this.Controls.Add(lblGender);
            this.Controls.Add(cbGender);
            y += 45;

            // Страна
            Label lblCountry = new Label { Text = "Страна:*", Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            cbCountry = new ComboBox { Location = new Point(xControl, y), Size = new Size(controlWidth, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cbCountry.Items.AddRange(new string[] { "Russia", "USA", "Germany", "France", "Brazil", "China", "Japan", "UK", "Italy", "Spain" });
            this.Controls.Add(lblCountry);
            this.Controls.Add(cbCountry);
            y += 45;

            // Дата рождения
            Label lblBirthDate = new Label { Text = "Дата рождения:*", Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            dtpBirthDate = new DateTimePicker { Location = new Point(xControl, y), Size = new Size(controlWidth, 30), Format = DateTimePickerFormat.Short };
            this.Controls.Add(lblBirthDate);
            this.Controls.Add(dtpBirthDate);
            y += 55;

            // Фото
            Label lblPhoto = new Label { Text = "Фото:*", Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            pbPhoto = new PictureBox { Location = new Point(xControl, y), Size = new Size(100, 100), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.LightGray };
            Button btnLoadPhoto = new Button { Text = "Загрузить фото", Location = new Point(xControl + 110, y + 35), Size = new Size(120, 30) };
            this.Controls.Add(lblPhoto);
            this.Controls.Add(pbPhoto);
            this.Controls.Add(btnLoadPhoto);
            y += 110;

            // Кнопки
            Button btnRegister = new Button { Text = "Зарегистрироваться", Location = new Point(150, y), Size = new Size(160, 45), BackColor = Color.LightGreen, Font = new Font("Arial", 11) };
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(330, y), Size = new Size(100, 45), BackColor = Color.LightCoral };
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnCancel);

            btnLoadPhoto.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    photoPath = ofd.FileName;
                    pbPhoto.Image = Image.FromFile(photoPath);
                    pbPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            };

            btnRegister.Click += (s, e) =>
            {
                if (ValidateForm())
                {
                    SaveToDatabase();
                }
            };

            btnCancel.Click += (s, e) => this.Close();
        }

        private void AddField(ref int y, int xLabel, int xControl, int labelWidth, int controlWidth, string labelText, out TextBox textBox)
        {
            Label lbl = new Label { Text = labelText, Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            textBox = new TextBox { Location = new Point(xControl, y), Size = new Size(controlWidth, 30) };
            this.Controls.Add(lbl);
            this.Controls.Add(textBox);
            y += 45;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text)) { MessageBox.Show("Введите имя!"); return false; }
            if (string.IsNullOrWhiteSpace(txtLastName.Text)) { MessageBox.Show("Введите фамилию!"); return false; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) { MessageBox.Show("Введите Email!"); return false; }
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { MessageBox.Show("Введите корректный Email!"); return false; }

            string password = txtPassword.Text;
            if (password.Length < 6) { MessageBox.Show("Пароль: минимум 6 символов"); return false; }
            if (!Regex.IsMatch(password, @"[A-Z]")) { MessageBox.Show("Пароль: минимум 1 заглавная буква"); return false; }
            if (!Regex.IsMatch(password, @"[0-9]")) { MessageBox.Show("Пароль: минимум 1 цифра"); return false; }
            if (!Regex.IsMatch(password, @"[!@#$%^]")) { MessageBox.Show("Пароль: минимум 1 спецсимвол (!@#$%^)"); return false; }
            if (password != txtConfirmPassword.Text) { MessageBox.Show("Пароли не совпадают!"); return false; }

            int age = DateTime.Now.Year - dtpBirthDate.Value.Year;
            if (dtpBirthDate.Value > DateTime.Now.AddYears(-age)) age--;
            if (age < 10) { MessageBox.Show("Возраст должен быть не менее 10 лет!"); return false; }

            if (cbGender.SelectedIndex == -1) { MessageBox.Show("Выберите пол!"); return false; }
            if (cbCountry.SelectedIndex == -1) { MessageBox.Show("Выберите страну!"); return false; }
            if (string.IsNullOrEmpty(photoPath)) { MessageBox.Show("Загрузите фото!"); return false; }

            return true;
        }

        private void SaveToDatabase()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Проверка: существует ли пользователь с таким Email
                    string checkUser = "SELECT COUNT(*) FROM [User] WHERE Email = @Email";
                    SqlCommand cmdCheck = new SqlCommand(checkUser, conn);
                    cmdCheck.Parameters.AddWithValue("@Email", txtEmail.Text);
                    int userExists = (int)cmdCheck.ExecuteScalar();

                    if (userExists > 0)
                    {
                        MessageBox.Show("Пользователь с таким Email уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Добавляем пользователя
                    string insertUser = "INSERT INTO [User] (Email, [Password], FirstName, LastName, RoleId) VALUES (@Email, @Password, @FirstName, @LastName, 'R')";
                    SqlCommand cmdUser = new SqlCommand(insertUser, conn);
                    cmdUser.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmdUser.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmdUser.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmdUser.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmdUser.ExecuteNonQuery();

                    // Добавляем бегуна
                    string countryName = cbCountry.SelectedItem.ToString();
                    string countryCode = countryName.Substring(0, 3).ToUpper();

                    string insertRunner = "INSERT INTO Runner (Email, Gender, DateOfBirth, CountryCode) VALUES (@Email, @Gender, @DateOfBirth, @CountryCode)";
                    SqlCommand cmdRunner = new SqlCommand(insertRunner, conn);
                    cmdRunner.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmdRunner.Parameters.AddWithValue("@Gender", cbGender.SelectedItem.ToString());
                    cmdRunner.Parameters.AddWithValue("@DateOfBirth", dtpBirthDate.Value);
                    cmdRunner.Parameters.AddWithValue("@CountryCode", countryCode);
                    cmdRunner.ExecuteNonQuery();

                    MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}