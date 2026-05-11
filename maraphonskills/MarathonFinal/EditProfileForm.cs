using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace MarathonFinal
{
    public class EditProfileForm : Form
    {
        private TextBox txtFirstName, txtLastName, txtPassword, txtConfirmPassword;
        private ComboBox cbGender, cbCountry;
        private DateTimePicker dtpBirthDate;
        private PictureBox pbPhoto;
        private string photoPath = "";
        private string currentPhotoPath = "";
        private string userEmail;

        public EditProfileForm(string email)
        {
            userEmail = email;
            this.Text = "Редактирование профиля";
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
                Text = "Редактирование профиля бегуна",
                Location = new Point(150, 10),
                Size = new Size(350, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Имя
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Имя:*", out txtFirstName);
            // Фамилия
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Фамилия:*", out txtLastName);
            y += 45;

            // Email (только для чтения)
            Label lblEmail = new Label { Text = "Email:", Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            TextBox txtEmail = new TextBox { Location = new Point(xControl, y), Size = new Size(controlWidth, 30), Text = email, ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            y += 45;

            // Пароль
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Новый пароль:", out txtPassword);
            txtPassword.PasswordChar = '*';
            // Подтверждение пароля
            AddField(ref y, xLabel, xControl, labelWidth, controlWidth, "Повтор пароля:", out txtConfirmPassword);
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
            Label lblPhoto = new Label { Text = "Фото:", Location = new Point(xLabel, y), Size = new Size(labelWidth, 30) };
            pbPhoto = new PictureBox { Location = new Point(xControl, y), Size = new Size(100, 100), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.LightGray };
            Button btnLoadPhoto = new Button { Text = "Загрузить новое фото", Location = new Point(xControl + 110, y + 35), Size = new Size(130, 30) };
            this.Controls.Add(lblPhoto);
            this.Controls.Add(pbPhoto);
            this.Controls.Add(btnLoadPhoto);
            y += 110;

            // Кнопки
            Button btnSave = new Button { Text = "Сохранить изменения", Location = new Point(150, y), Size = new Size(160, 45), BackColor = Color.LightGreen, Font = new Font("Arial", 11) };
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(330, y), Size = new Size(100, 45), BackColor = Color.LightCoral };
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            // Загрузка данных пользователя
            LoadUserData();

            // Загрузка нового фото
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

            // Сохранение изменений
            btnSave.Click += (s, e) =>
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

        private void LoadUserData()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Загрузка данных из User
                    string queryUser = "SELECT FirstName, LastName, [Password] FROM [User] WHERE Email = @Email";
                    SqlCommand cmdUser = new SqlCommand(queryUser, conn);
                    cmdUser.Parameters.AddWithValue("@Email", userEmail);
                    SqlDataReader reader = cmdUser.ExecuteReader();

                    if (reader.Read())
                    {
                        txtFirstName.Text = reader["FirstName"].ToString();
                        txtLastName.Text = reader["LastName"].ToString();
                        string oldPassword = reader["Password"].ToString();
                        txtPassword.Text = oldPassword;
                        txtConfirmPassword.Text = oldPassword;
                    }
                    reader.Close();

                    // Загрузка данных из Runner
                    string queryRunner = "SELECT Gender, DateOfBirth, CountryCode FROM Runner WHERE Email = @Email";
                    SqlCommand cmdRunner = new SqlCommand(queryRunner, conn);
                    cmdRunner.Parameters.AddWithValue("@Email", userEmail);
                    reader = cmdRunner.ExecuteReader();

                    if (reader.Read())
                    {
                        string gender = reader["Gender"].ToString();
                        if (gender == "Male") cbGender.SelectedIndex = 0;
                        else if (gender == "Female") cbGender.SelectedIndex = 1;

                        dtpBirthDate.Value = Convert.ToDateTime(reader["DateOfBirth"]);

                        string countryCode = reader["CountryCode"].ToString();
                        // Упрощённый поиск страны по коду
                        for (int i = 0; i < cbCountry.Items.Count; i++)
                        {
                            if (cbCountry.Items[i].ToString().StartsWith(countryCode))
                            {
                                cbCountry.SelectedIndex = i;
                                break;
                            }
                        }
                        if (cbCountry.SelectedIndex == -1) cbCountry.SelectedIndex = 0;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            { MessageBox.Show("Введите имя!"); return false; }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            { MessageBox.Show("Введите фамилию!"); return false; }

            string password = txtPassword.Text;
            if (password.Length < 6)
            { MessageBox.Show("Пароль: минимум 6 символов"); return false; }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            { MessageBox.Show("Пароль: минимум 1 заглавная буква"); return false; }
            if (!Regex.IsMatch(password, @"[0-9]"))
            { MessageBox.Show("Пароль: минимум 1 цифра"); return false; }
            if (!Regex.IsMatch(password, @"[!@#$%^]"))
            { MessageBox.Show("Пароль: минимум 1 спецсимвол (!@#$%^)"); return false; }
            if (password != txtConfirmPassword.Text)
            { MessageBox.Show("Пароли не совпадают!"); return false; }

            int age = DateTime.Now.Year - dtpBirthDate.Value.Year;
            if (dtpBirthDate.Value > DateTime.Now.AddYears(-age)) age--;
            if (age < 10)
            { MessageBox.Show("Возраст должен быть не менее 10 лет!"); return false; }

            if (cbGender.SelectedIndex == -1)
            { MessageBox.Show("Выберите пол!"); return false; }
            if (cbCountry.SelectedIndex == -1)
            { MessageBox.Show("Выберите страну!"); return false; }

            return true;
        }

        private void SaveToDatabase()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Обновление User
                    string updateUser = "UPDATE [User] SET FirstName = @FirstName, LastName = @LastName, [Password] = @Password WHERE Email = @Email";
                    SqlCommand cmdUser = new SqlCommand(updateUser, conn);
                    cmdUser.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmdUser.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmdUser.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmdUser.Parameters.AddWithValue("@Email", userEmail);
                    cmdUser.ExecuteNonQuery();

                    // Обновление Runner
                    string countryName = cbCountry.SelectedItem.ToString();
                    string countryCode = countryName.Substring(0, 3).ToUpper();

                    string updateRunner = "UPDATE Runner SET Gender = @Gender, DateOfBirth = @DateOfBirth, CountryCode = @CountryCode WHERE Email = @Email";
                    SqlCommand cmdRunner = new SqlCommand(updateRunner, conn);
                    cmdRunner.Parameters.AddWithValue("@Gender", cbGender.SelectedItem.ToString());
                    cmdRunner.Parameters.AddWithValue("@DateOfBirth", dtpBirthDate.Value);
                    cmdRunner.Parameters.AddWithValue("@CountryCode", countryCode);
                    cmdRunner.Parameters.AddWithValue("@Email", userEmail);
                    cmdRunner.ExecuteNonQuery();

                    MessageBox.Show("Профиль успешно обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}