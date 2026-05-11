using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class ManageRunnersForm : Form
    {
        private DataGridView dgvRunners;
        private TextBox txtSearch;
        private ComboBox cmbEvent;
        private ComboBox cmbStatus;
        private Button btnSearch, btnExportCSV, btnExportEmail, btnClose;
        private Button btnEditProfile, btnCertificate;
        private string connectionString = "Server=LAPTOP-Q3TD6VOU;Database=MarathonSkills2016;Integrated Security=True;";

        public ManageRunnersForm()
        {
            this.Text = "Управление бегунами - Координатор";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Панель фильтров
            Label lblSearch = new Label() { Text = "Поиск (ФИО):", Location = new Point(20, 20), Size = new Size(100, 25) };
            txtSearch = new TextBox() { Location = new Point(130, 18), Size = new Size(150, 25) };

            Label lblEvent = new Label() { Text = "Забег:", Location = new Point(310, 20), Size = new Size(60, 25) };
            cmbEvent = new ComboBox() { Location = new Point(370, 18), Size = new Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEvent.Items.Add("Все забеги");
            cmbEvent.SelectedIndex = 0;

            Label lblStatus = new Label() { Text = "Статус:", Location = new Point(580, 20), Size = new Size(60, 25) };
            cmbStatus = new ComboBox() { Location = new Point(640, 18), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new string[] { "Все", "Registered", "Confirmed", "DNF", "DNS" });
            cmbStatus.SelectedIndex = 0;

            btnSearch = new Button() { Text = "🔍 Поиск", Location = new Point(790, 16), Size = new Size(100, 30), BackColor = Color.LightBlue };
            btnSearch.Click += BtnSearch_Click;

            // Таблица
            dgvRunners = new DataGridView()
            {
                Location = new Point(20, 60),
                Size = new Size(1050, 480),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvRunners.CellDoubleClick += DgvRunners_CellDoubleClick;

            // Кнопки экспорта
            btnExportCSV = new Button() { Text = "📁 Выгрузить CSV", Location = new Point(20, 550), Size = new Size(150, 40), BackColor = Color.LightGreen };
            btnExportCSV.Click += BtnExportCSV_Click;

            btnExportEmail = new Button() { Text = "📧 Выгрузить Email", Location = new Point(190, 550), Size = new Size(150, 40), BackColor = Color.LightYellow };
            btnExportEmail.Click += BtnExportEmail_Click;

            btnEditProfile = new Button()
            {
                Text = "✏️ Редактировать профиль",
                Location = new Point(360, 550),
                Size = new Size(160, 40),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnEditProfile.Click += BtnEditProfile_Click;

            btnCertificate = new Button()
            {
                Text = "🏆 Сертификат",
                Location = new Point(540, 550),
                Size = new Size(160, 40),
                BackColor = Color.LightGoldenrodYellow,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnCertificate.Click += BtnCertificate_Click;

            btnClose = new Button() { Text = "Закрыть", Location = new Point(920, 550), Size = new Size(150, 40), BackColor = Color.LightCoral };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, lblEvent, cmbEvent, lblStatus, cmbStatus,
                btnSearch, dgvRunners, btnExportCSV, btnExportEmail,
                btnEditProfile, btnCertificate, btnClose
            });

            LoadEvents();
            LoadRunners();
        }

        private void LoadEvents()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT EventId, EventName FROM Event", conn);
                    SqlDataReader dr = cmd.ExecuteReader();

                    cmbEvent.Items.Clear();
                    cmbEvent.Items.Add("Все забеги");

                    while (dr.Read())
                    {
                        int eventId = Convert.ToInt32(dr["EventId"]);
                        string eventName = dr["EventName"].ToString();
                        cmbEvent.Items.Add(new KeyValuePair<int, string>(eventId, eventName));
                    }
                    dr.Close();
                    cmbEvent.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки забегов: " + ex.Message);
            }
        }

        private void LoadRunners()
        {
            try
            {
                string sql = @"
                    SELECT 
                        re.RegistrationEventId,
                        u.Email,
                        r.FirstName,
                        r.LastName,
                        r.Gender,
                        r.DateOfBirth,
                        e.EventName,
                        e.StartDateTime,
                        re.Status AS RegistrationStatus,
                        re.GotKit,
                        rs.ResultTime,
                        rs.Place
                    FROM Registration reg
                    JOIN [User] u ON reg.Email = u.Email
                    JOIN Runner r ON u.Email = r.Email
                    JOIN RegistrationEvent re ON reg.RegistrationId = re.RegistrationId
                    JOIN Event e ON re.EventId = e.EventId
                    LEFT JOIN Result rs ON re.RegistrationEventId = rs.RegistrationEventId
                    WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    sql += $" AND (r.FirstName LIKE '%{txtSearch.Text}%' OR r.LastName LIKE '%{txtSearch.Text}%' OR u.Email LIKE '%{txtSearch.Text}%')";
                }

                if (cmbEvent.SelectedItem is KeyValuePair<int, string> eventItem)
                {
                    sql += $" AND e.EventId = {eventItem.Key}";
                }

                if (cmbStatus.SelectedIndex > 0)
                {
                    string status = cmbStatus.SelectedItem.ToString();
                    sql += $" AND re.Status = '{status}'";
                }

                sql += " ORDER BY e.StartDateTime, r.LastName, r.FirstName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRunners.DataSource = dt;

                    // Настройка колонок
                    if (dgvRunners.Columns.Contains("RegistrationEventId"))
                        dgvRunners.Columns["RegistrationEventId"].Visible = false;
                    if (dgvRunners.Columns.Contains("GotKit"))
                        dgvRunners.Columns["GotKit"].Visible = false;

                    if (dgvRunners.Columns.Contains("FirstName"))
                        dgvRunners.Columns["FirstName"].HeaderText = "Имя";
                    if (dgvRunners.Columns.Contains("LastName"))
                        dgvRunners.Columns["LastName"].HeaderText = "Фамилия";
                    if (dgvRunners.Columns.Contains("Email"))
                        dgvRunners.Columns["Email"].HeaderText = "Email";
                    if (dgvRunners.Columns.Contains("Gender"))
                        dgvRunners.Columns["Gender"].HeaderText = "Пол";
                    if (dgvRunners.Columns.Contains("DateOfBirth"))
                        dgvRunners.Columns["DateOfBirth"].HeaderText = "Дата рождения";
                    if (dgvRunners.Columns.Contains("EventName"))
                        dgvRunners.Columns["EventName"].HeaderText = "Забег";
                    if (dgvRunners.Columns.Contains("StartDateTime"))
                        dgvRunners.Columns["StartDateTime"].HeaderText = "Дата старта";
                    if (dgvRunners.Columns.Contains("RegistrationStatus"))
                        dgvRunners.Columns["RegistrationStatus"].HeaderText = "Статус";
                    if (dgvRunners.Columns.Contains("ResultTime"))
                        dgvRunners.Columns["ResultTime"].HeaderText = "Результат";
                    if (dgvRunners.Columns.Contains("Place"))
                        dgvRunners.Columns["Place"].HeaderText = "Место";

                    // Подсветка строк
                    foreach (DataGridViewRow row in dgvRunners.Rows)
                    {
                        string status = row.Cells["RegistrationStatus"].Value?.ToString();
                        if (status == "Confirmed")
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                        else if (status == "Registered")
                            row.DefaultCellStyle.BackColor = Color.LightYellow;
                        else if (status == "DNF" || status == "DNS")
                            row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadRunners();
        }

        // ВСПОМОГАТЕЛЬНЫЙ МЕТОД: получаем UserId по Email
        private int GetUserIdByEmail(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT UserId FROM [User] WHERE Email = @Email", conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка получения UserId: " + ex.Message);
                return 0;
            }
        }

        // ДВОЙНОЙ КЛИК
        private void DgvRunners_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvRunners.Rows[e.RowIndex];

                int registrationEventId = Convert.ToInt32(row.Cells["RegistrationEventId"].Value);
                string email = row.Cells["Email"].Value.ToString();
                int userId = GetUserIdByEmail(email);
                string runnerName = row.Cells["FirstName"].Value.ToString() + " " + row.Cells["LastName"].Value.ToString();
                string eventName = row.Cells["EventName"].Value.ToString();

                var detailForm = new ManageRunnerDetailForm(registrationEventId, userId, runnerName, eventName);
                detailForm.ShowDialog();
                LoadRunners();
            }
        }

        // РЕДАКТИРОВАНИЕ ПРОФИЛЯ
        private void BtnEditProfile_Click(object sender, EventArgs e)
        {
            if (dgvRunners.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите бегуна для редактирования!");
                return;
            }

            DataGridViewRow row = dgvRunners.SelectedRows[0];
            string email = row.Cells["Email"].Value.ToString();
            int userId = GetUserIdByEmail(email);

            var editForm = new EditRunnerByCoordinatorForm(userId);
            editForm.ShowDialog();
            LoadRunners();
        }

        // СЕРТИФИКАТ
        private void BtnCertificate_Click(object sender, EventArgs e)
        {
            if (dgvRunners.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите бегуна для показа сертификата!");
                return;
            }

            DataGridViewRow row = dgvRunners.SelectedRows[0];

            string runnerName = row.Cells["FirstName"].Value.ToString() + " " + row.Cells["LastName"].Value.ToString();
            string eventName = row.Cells["EventName"].Value.ToString();
            string resultTime = row.Cells["ResultTime"].Value?.ToString() ?? "00:00:00";
            int place = row.Cells["Place"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["Place"].Value) : 1;
            DateTime eventDate = row.Cells["StartDateTime"].Value != DBNull.Value ? Convert.ToDateTime(row.Cells["StartDateTime"].Value) : DateTime.Now;

            var certForm = new CertificateForm(runnerName, eventName, resultTime, place, eventDate);
            certForm.ShowDialog();
        }

        // ВЫГРУЗКА CSV
        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            if (dgvRunners.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "CSV файлы (*.csv)|*.csv",
                FileName = $"Runners_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();

                // Заголовки
                for (int i = 0; i < dgvRunners.Columns.Count; i++)
                {
                    if (dgvRunners.Columns[i].Visible && dgvRunners.Columns[i].Name != "RegistrationEventId")
                    {
                        sb.Append(dgvRunners.Columns[i].HeaderText);
                        if (i < dgvRunners.Columns.Count - 1) sb.Append(";");
                    }
                }
                sb.AppendLine();

                // Данные
                foreach (DataGridViewRow row in dgvRunners.Rows)
                {
                    for (int i = 0; i < dgvRunners.Columns.Count; i++)
                    {
                        if (dgvRunners.Columns[i].Visible && dgvRunners.Columns[i].Name != "RegistrationEventId")
                        {
                            string value = row.Cells[i].Value?.ToString() ?? "";
                            if (value.Contains(";") || value.Contains("\""))
                            {
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            }
                            sb.Append(value);
                            if (i < dgvRunners.Columns.Count - 1) sb.Append(";");
                        }
                    }
                    sb.AppendLine();
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Данные выгружены в файл:\n{sfd.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ВЫГРУЗКА EMAIL
        private void BtnExportEmail_Click(object sender, EventArgs e)
        {
            if (dgvRunners.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StringBuilder emails = new StringBuilder();
            foreach (DataGridViewRow row in dgvRunners.Rows)
            {
                string email = row.Cells["Email"].Value?.ToString();
                if (!string.IsNullOrEmpty(email))
                {
                    emails.AppendLine(email);
                }
            }

            Form frmEmails = new Form()
            {
                Text = "Список email-адресов",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterScreen
            };

            TextBox txtEmails = new TextBox()
            {
                Multiline = true,
                Height = 300,
                Width = 450,
                Text = emails.ToString(),
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 20),
                Font = new Font("Consolas", 10)
            };

            Button btnCopy = new Button()
            {
                Text = "Копировать в буфер",
                Location = new Point(20, 330),
                Size = new Size(150, 30)
            };
            btnCopy.Click += (s, args) =>
            {
                Clipboard.SetText(emails.ToString());
                MessageBox.Show("Email-адреса скопированы в буфер обмена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            Button btnCloseEmail = new Button()
            {
                Text = "Закрыть",
                Location = new Point(350, 330),
                Size = new Size(100, 30)
            };
            btnCloseEmail.Click += (s, args) => frmEmails.Close();

            frmEmails.Controls.Add(txtEmails);
            frmEmails.Controls.Add(btnCopy);
            frmEmails.Controls.Add(btnCloseEmail);
            frmEmails.ShowDialog();
        }
    }
}