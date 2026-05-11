using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class MyResultsForm : Form
    {
        private string userEmail;
        private DataGridView dgvResults;
        private Label lblInfo;

        public MyResultsForm(string email)
        {
            userEmail = email;
            this.Text = "Мои результаты";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "Мои результаты участия в марафонах",
                Location = new Point(20, 20),
                Size = new Size(700, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Информация о бегуне
            lblInfo = new Label
            {
                Text = "Загрузка данных...",
                Location = new Point(20, 60),
                Size = new Size(700, 40),
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(lblInfo);

            // Таблица результатов
            dgvResults = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(740, 320),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvResults);

            // Кнопка "Показать все результаты"
            Button btnAllResults = new Button
            {
                Text = "Показать все результаты",
                Location = new Point(300, 450),
                Size = new Size(180, 40),
                BackColor = Color.LightBlue
            };
            btnAllResults.Click += (s, e) => MessageBox.Show("Все результаты марафонов - в разработке", "Инфо");
            this.Controls.Add(btnAllResults);

            // Кнопка "Закрыть"
            Button btnClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(500, 450),
                Size = new Size(100, 40),
                BackColor = Color.LightCoral
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            LoadResults();
        }

        private void LoadResults()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Получаем информацию о бегуне
                    string runnerInfoQuery = @"
                        SELECT u.FirstName, u.LastName, r.Gender, r.DateOfBirth, c.CountryName
                        FROM Runner r
                        JOIN [User] u ON r.Email = u.Email
                        JOIN Country c ON r.CountryCode = c.CountryCode
                        WHERE r.Email = @Email";

                    SqlCommand cmdInfo = new SqlCommand(runnerInfoQuery, conn);
                    cmdInfo.Parameters.AddWithValue("@Email", userEmail);
                    SqlDataReader reader = cmdInfo.ExecuteReader();

                    if (reader.Read())
                    {
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        string gender = reader["Gender"].ToString();
                        DateTime dob = Convert.ToDateTime(reader["DateOfBirth"]);
                        string country = reader["CountryName"].ToString();

                        int age = DateTime.Now.Year - dob.Year;
                        if (dob > DateTime.Now.AddYears(-age)) age--;

                        string ageCategory = GetAgeCategory(age);

                        lblInfo.Text = $"{firstName} {lastName} | Пол: {gender} | Страна: {country} | Возраст: {age} лет | Категория: {ageCategory}";
                    }
                    reader.Close();

                    // Получаем результаты забегов
                    string resultsQuery = @"
                        SELECT 
                            m.MarathonName AS 'Марафон',
                            e.EventName AS 'Событие',
                            CONVERT(varchar, DATEADD(SECOND, re.RaceTime, 0), 108) AS 'Время',
                            re.BibNumber AS 'Номер участника'
                        FROM RegistrationEvent re
                        JOIN Registration reg ON re.RegistrationId = reg.RegistrationId
                        JOIN Runner r ON reg.RunnerId = r.RunnerId
                        JOIN [Event] e ON re.EventId = e.EventId
                        JOIN Marathon m ON e.MarathonId = m.MarathonId
                        WHERE r.Email = @Email
                        ORDER BY m.YearHeld DESC";

                    SqlCommand cmdResults = new SqlCommand(resultsQuery, conn);
                    cmdResults.Parameters.AddWithValue("@Email", userEmail);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmdResults);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    adapter.Fill(dt);

                    dgvResults.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("У вас пока нет результатов участия в марафонах.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetAgeCategory(int age)
        {
            if (age < 18) return "до 18";
            if (age <= 29) return "18-29";
            if (age <= 39) return "30-39";
            if (age <= 55) return "40-55";
            if (age <= 70) return "56-70";
            return "более 70";
        }
    }
}