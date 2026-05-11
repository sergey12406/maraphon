using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class ManageRunnerDetailForm : Form
    {
        private int registrationEventId;
        private int userId;
        private string runnerName;
        private string eventName;

        private CheckBox chkConfirmed;
        private CheckBox chkGotKit;
        private ComboBox cmbStatus;
        private Button btnSave, btnPrintBadge, btnClose;
        private Label lblRunnerInfo, lblEventInfo;

        private string connectionString = "Server=LAPTOP-Q3TD6VOU;Database=MarathonSkills2016;Integrated Security=True;"; // Замените

        public ManageRunnerDetailForm(int regEventId, int userId, string name, string eventName)
        {
            this.registrationEventId = regEventId;
            this.userId = userId;
            this.runnerName = name;
            this.eventName = eventName;

            this.Text = "Управление бегуном";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Информация о бегуне
            lblRunnerInfo = new Label()
            {
                Text = $"Бегун: {runnerName}",
                Location = new Point(30, 30),
                Size = new Size(380, 30),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            lblEventInfo = new Label()
            {
                Text = $"Забег: {eventName}",
                Location = new Point(30, 65),
                Size = new Size(380, 25),
                Font = new Font("Arial", 10)
            };

            // Статус регистрации
            Label lblStatus = new Label()
            {
                Text = "Статус регистрации:",
                Location = new Point(30, 110),
                Size = new Size(130, 25)
            };

            cmbStatus = new ComboBox()
            {
                Location = new Point(170, 108),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new string[] { "Registered", "Confirmed", "DNF", "DNS" });

            // Галочка "Получил комплект"
            chkGotKit = new CheckBox()
            {
                Text = "Получил гоночный комплект",
                Location = new Point(30, 150),
                Size = new Size(200, 25)
            };

            // Галочка "Подтверждён" (дублирует статус, но по заданию)
            chkConfirmed = new CheckBox()
            {
                Text = "Подтверждён",
                Location = new Point(30, 180),
                Size = new Size(150, 25)
            };
            chkConfirmed.CheckedChanged += (s, e) =>
            {
                if (chkConfirmed.Checked && cmbStatus.SelectedItem?.ToString() != "Confirmed")
                    cmbStatus.SelectedItem = "Confirmed";
            };

            // Кнопки
            btnSave = new Button()
            {
                Text = "Сохранить",
                Location = new Point(30, 230),
                Size = new Size(100, 40),
                BackColor = Color.LightGreen
            };
            btnSave.Click += BtnSave_Click;

            btnPrintBadge = new Button()
            {
                Text = "Печать бейджа",
                Location = new Point(150, 230),
                Size = new Size(120, 40),
                BackColor = Color.LightBlue
            };
            btnPrintBadge.Click += BtnPrintBadge_Click;

            btnClose = new Button()
            {
                Text = "Закрыть",
                Location = new Point(290, 230),
                Size = new Size(100, 40),
                BackColor = Color.LightCoral
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblRunnerInfo, lblEventInfo, lblStatus, cmbStatus,
                                                   chkGotKit, chkConfirmed, btnSave, btnPrintBadge, btnClose });

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT re.Status, re.GotKit 
                        FROM RegistrationEvent re 
                        WHERE re.RegistrationEventID = @RegEventId";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@RegEventId", registrationEventId);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        string status = dr["Status"].ToString();
                        cmbStatus.SelectedItem = status;
                        chkConfirmed.Checked = (status == "Confirmed");

                        int gotKit = Convert.ToInt32(dr["GotKit"]);
                        chkGotKit.Checked = (gotKit == 1);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE RegistrationEvent 
                        SET Status = @Status, GotKit = @GotKit 
                        WHERE RegistrationEventID = @RegEventId";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Status", cmbStatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@GotKit", chkGotKit.Checked ? 1 : 0);
                    cmd.Parameters.AddWithValue("@RegEventId", registrationEventId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Сохранено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void BtnPrintBadge_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 18, FontStyle.Bold);
            Font textFont = new Font("Arial", 12);
            Font nameFont = new Font("Arial", 16, FontStyle.Bold);
            Brush brush = Brushes.Black;

            // Рамка бейджа
            g.DrawRectangle(Pens.Black, 50, 50, 250, 150);

            // Логотип (простой текст вместо картинки)
            g.DrawString("MARATHON", titleFont, brush, 80, 65);
            g.DrawString("SKILLS 2017", new Font("Arial", 12, FontStyle.Bold), brush, 100, 95);

            // Линия разделения
            g.DrawLine(Pens.Black, 60, 115, 290, 115);

            // Имя бегуна
            g.DrawString(runnerName, nameFont, brush, 70, 130);

            // Забег
            g.DrawString(eventName, textFont, brush, 70, 160);

            // Штрихкод (имитация)
            for (int i = 0; i < 20; i++)
            {
                g.DrawLine(Pens.Black, 70 + i * 10, 185, 70 + i * 10, 195);
            }
            g.DrawString(userId.ToString(), new Font("Arial", 8), brush, 140, 185);
        }
    }
}