using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class EditRunnerByCoordinatorForm : Form
    {
        private int userId;
        private string connectionString = "ваша_строка_подключения"; // Замените

        private TextBox txtFirstName, txtLastName, txtEmail, txtPhone, txtCountry, txtEmergencyContact;
        private ComboBox cmbGender;
        private DateTimePicker dtpDateOfBirth;
        private PictureBox pbPhoto;
        private Button btnSave, btnCancel, btnLoadPhoto;
        private string currentPhotoPath = "";

        public EditRunnerByCoordinatorForm(int userId)
        {
            this.userId = userId;
            this.Text = "Редактирование профиля бегуна (Координатор)";
            this.Size = new Size(550, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Заголовок
            Label lblTitle = new Label()
            {
                Text = "✏️ Редактирование профиля бегуна",
                Location = new Point(150, 15),
                Size = new Size(300, 35),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Фото
            pbPhoto = new PictureBox()
            {
                Location = new Point(350, 60),
                Size = new Size(150, 150),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            btnLoadPhoto = new Button()
            {
                Text = "Загрузить фото",
                Location = new Point(350, 220),
                Size = new Size(150, 30),
                BackColor = Color.LightYellow
            };
            btnLoadPhoto.Click += BtnLoadPhoto_Click;

            // Поля ввода
            int y = 60;
            int x = 30;
            int spacing = 40;

            AddField("Имя:", ref y, spacing, x);
            txtFirstName = new TextBox() { Location = new Point(120, y - 23), Size = new Size(200, 25) };

            AddField("Фамилия:", ref y, spacing, x);
            txtLastName = new TextBox() { Location = new Point(120, y - 23), Size = new Size(200, 25) };

            AddField("Email:", ref y, spacing, x);
            txtEmail = new TextBox() { Location = new Point(120, y - 23), Size = new Size(200, 25) };
            txtEmail.ReadOnly = true; // Email нельзя менять

            AddField("Пол:", ref y, spacing, x);
            cmbGender = new ComboBox() { Location = new Point(120, y - 23), Size = new Size(80, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGender.Items.AddRange(new string[] { "M", "F" });

            AddField("Дата рождения:", ref y, spacing, x);
            dtpDateOfBirth = new DateTimePicker() { Location = new Point(120, y - 23), Size = new Size(150, 25), Format = DateTimePickerFormat.Short };
            dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-10); // Минимум 10 лет

            AddField("Страна:", ref y, spacing, x);
            txtCountry = new TextBox() { Location = new Point(120, y - 23), Size = new Size(150, 25) };

            AddField("Телефон:", ref y, spacing, x);
            txtPhone = new TextBox() { Location = new Point(120, y - 23), Size = new Size(150, 25) };

            AddField("Контакт для экстр. случаев:", ref y, spacing, x);
            txtEmergencyContact = new TextBox() { Location = new Point(200, y - 23), Size = new Size(200, 25) };

            // Кнопки
            btnSave = new Button()
            {
                Text = "💾 Сохранить",
                Location = new Point(100, 480),
                Size = new Size(120, 40),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button()
            {
                Text = "❌ Отмена",
                Location = new Point(280, 480),
                Size = new Size(120, 40),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnCancel.Click += (s, e) => this.Close();

            // Добавляем элементы
            this.Controls.AddRange(new Control[] { lblTitle, pbPhoto, btnLoadPhoto,
                txtFirstName, txtLastName, txtEmail, cmbGender, dtpDateOfBirth,
                txtCountry, txtPhone, txtEmergencyContact, btnSave, btnCancel });

            LoadRunnerData();
        }

        private void AddField(string labelText, ref int y, int spacing, int x)
        {
            Label lbl = new Label()
            {
                Text = labelText,
                Location = new Point(x, y),
                Size = new Size(150, 25),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(lbl);
            y += spacing;
        }

        private void LoadRunnerData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT u.Email, r.FirstName, r.LastName, r.Gender, r.DateOfBirth, 
                               r.Country, r.Phone, r.EmergencyContact, r.Photo
                        FROM [User] u
                        JOIN Runner r ON u.UserID = r.UserID
                        WHERE u.UserID = @UserId";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        txtEmail.Text = dr["Email"].ToString();
                        txtFirstName.Text = dr["FirstName"].ToString();
                        txtLastName.Text = dr["LastName"].ToString();
                        cmbGender.SelectedItem = dr["Gender"].ToString();
                        dtpDateOfBirth.Value = Convert.ToDateTime(dr["DateOfBirth"]);
                        txtCountry.Text = dr["Country"]?.ToString() ?? "";
                        txtPhone.Text = dr["Phone"]?.ToString() ?? "";
                        txtEmergencyContact.Text = dr["EmergencyContact"]?.ToString() ?? "";
                        currentPhotoPath = dr["Photo"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(currentPhotoPath) && File.Exists(currentPhotoPath))
                        {
                            pbPhoto.Image = Image.FromFile(currentPhotoPath);
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void BtnLoadPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите фото бегуна"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Копируем фото в папку проекта
                string fileName = Path.GetFileName(ofd.FileName);
                string destPath = Path.Combine(Application.StartupPath, "Photos", $"runner_{userId}_{fileName}");

                if (!Directory.Exists(Path.Combine(Application.StartupPath, "Photos")))
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Photos"));

                File.Copy(ofd.FileName, destPath, true);
                currentPhotoPath = destPath;
                pbPhoto.Image = Image.FromFile(destPath);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию!");
                return;
            }

            // Проверка возраста
            int age = DateTime.Now.Year - dtpDateOfBirth.Value.Year;
            if (dtpDateOfBirth.Value > DateTime.Now.AddYears(-age)) age--;
            if (age < 10)
            {
                MessageBox.Show("Бегун должен быть не младше 10 лет!");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE Runner 
                        SET FirstName = @FirstName, LastName = @LastName, 
                            Gender = @Gender, DateOfBirth = @DateOfBirth,
                            Country = @Country, Phone = @Phone, 
                            EmergencyContact = @EmergencyContact, Photo = @Photo
                        WHERE UserID = @UserId";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem?.ToString() ?? "M");
                    cmd.Parameters.AddWithValue("@DateOfBirth", dtpDateOfBirth.Value);
                    cmd.Parameters.AddWithValue("@Country", string.IsNullOrEmpty(txtCountry.Text) ? (object)DBNull.Value : txtCountry.Text);
                    cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text);
                    cmd.Parameters.AddWithValue("@EmergencyContact", string.IsNullOrEmpty(txtEmergencyContact.Text) ? (object)DBNull.Value : txtEmergencyContact.Text);
                    cmd.Parameters.AddWithValue("@Photo", string.IsNullOrEmpty(currentPhotoPath) ? (object)DBNull.Value : currentPhotoPath);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Профиль успешно обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }
    }
}