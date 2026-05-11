using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class RegisterForEventForm : Form
    {
        private FlowLayoutPanel flpEvents;
        private List<CheckBox> eventCheckBoxes;
        private List<decimal> eventCosts;
        private ComboBox cbRaceKit;
        private ComboBox cbCharity;
        private TextBox txtSponsorship;
        private Label lblTotalCost;
        private Label lblConnectionStatus;

        private List<RaceKit> raceKits;
        private List<Charity> charities;
        private List<Event> events;

        private class RaceKit { public string Id { get; set; } public string Name { get; set; } public decimal Cost { get; set; } }
        private class Charity { public int Id { get; set; } public string Name { get; set; } public string Description { get; set; } }
        private class Event { public string Id { get; set; } public string Name { get; set; } public decimal Cost { get; set; } }

        public RegisterForEventForm()
        {
            this.Text = "Регистрация на марафон";
            this.Size = new Size(650, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // Статус подключения
            lblConnectionStatus = new Label { Location = new Point(20, 10), Size = new Size(600, 25), Font = new Font("Arial", 9), ForeColor = Color.Green };
            this.Controls.Add(lblConnectionStatus);

            // Загрузка данных
            LoadDataFromDB();

            int y = 50;

            Label lblEvents = new Label { Text = "Выберите дистанции (минимум 1):", Location = new Point(20, y), Size = new Size(250, 25), Font = new Font("Arial", 10, FontStyle.Bold) };
            y += 30;
            this.Controls.Add(lblEvents);

            flpEvents = new FlowLayoutPanel { Location = new Point(20, y), Size = new Size(580, 150), AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };
            y += 160;
            this.Controls.Add(flpEvents);

            eventCheckBoxes = new List<CheckBox>();
            eventCosts = new List<decimal>();

            foreach (var ev in events)
            {
                CheckBox ch = new CheckBox { Text = $"{ev.Name} (${ev.Cost:F2})", Width = 260, Height = 25 };
                ch.CheckedChanged += (s, e) => CalculateTotal();
                eventCheckBoxes.Add(ch);
                eventCosts.Add(ev.Cost);
                flpEvents.Controls.Add(ch);
            }

            // Гоночный комплект
            Label lblKit = new Label { Text = "Гоночный комплект:", Location = new Point(20, y), Size = new Size(150, 25) };
            cbRaceKit = new ComboBox { Location = new Point(180, y), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var kit in raceKits) cbRaceKit.Items.Add($"{kit.Name} (${kit.Cost:F2})");
            if (cbRaceKit.Items.Count > 0) cbRaceKit.SelectedIndex = 0;
            cbRaceKit.SelectedIndexChanged += (s, e) => CalculateTotal();
            y += 35;
            this.Controls.Add(lblKit);
            this.Controls.Add(cbRaceKit);

            // Благотворительность
            Label lblCharity = new Label { Text = "Благотворительная организация:", Location = new Point(20, y), Size = new Size(190, 25) };
            cbCharity = new ComboBox { Location = new Point(220, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in charities) cbCharity.Items.Add(c.Name);
            if (cbCharity.Items.Count > 0) cbCharity.SelectedIndex = 0;
            Button btnCharityInfo = new Button { Text = "i", Location = new Point(510, y), Width = 30, Height = 23 };
            btnCharityInfo.Click += (s, e) =>
            {
                if (cbCharity.SelectedIndex >= 0)
                    MessageBox.Show(charities[cbCharity.SelectedIndex].Description, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            y += 35;
            this.Controls.Add(lblCharity);
            this.Controls.Add(cbCharity);
            this.Controls.Add(btnCharityInfo);

            // Спонсорство
            Label lblSponsor = new Label { Text = "Сумма взноса (спонсорство): $", Location = new Point(20, y), Size = new Size(210, 25) };
            txtSponsorship = new TextBox { Location = new Point(240, y), Width = 120, Text = "0" };
            txtSponsorship.TextChanged += (s, e) => CalculateTotal();
            y += 40;
            this.Controls.Add(lblSponsor);
            this.Controls.Add(txtSponsorship);

            // Итог
            Label lblTotal = new Label { Text = "Итоговая стоимость: $", Location = new Point(20, y), Size = new Size(150, 30), Font = new Font("Arial", 12, FontStyle.Bold) };
            lblTotalCost = new Label { Text = "0.00", Location = new Point(180, y), Size = new Size(150, 30), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Green };
            y += 50;
            this.Controls.Add(lblTotal);
            this.Controls.Add(lblTotalCost);

            // Кнопки
            Button btnRegister = new Button { Text = "Зарегистрироваться", Location = new Point(180, y), Size = new Size(160, 45), BackColor = Color.LightGreen, Font = new Font("Arial", 11) };
            btnRegister.Click += BtnRegister_Click;
            Button btnCancel = new Button { Text = "Отмена", Location = new Point(360, y), Size = new Size(100, 45), BackColor = Color.LightCoral };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnCancel);

            CalculateTotal();
        }

        private void LoadDataFromDB()
        {
            events = new List<Event>();
            raceKits = new List<RaceKit>();
            charities = new List<Charity>();

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    lblConnectionStatus.Text = "✓ Подключено к базе данных. Загрузка...";
                    lblConnectionStatus.ForeColor = Color.Green;

                    // Загрузка событий
                    SqlCommand cmdEvents = new SqlCommand("SELECT EventId, EventName, Cost FROM [Event] WHERE MarathonId = 5", conn);
                    SqlDataReader reader = cmdEvents.ExecuteReader();
                    while (reader.Read())
                    {
                        events.Add(new Event
                        {
                            Id = reader["EventId"].ToString(),
                            Name = reader["EventName"].ToString(),
                            Cost = Convert.ToDecimal(reader["Cost"])
                        });
                    }
                    reader.Close();

                    // Загрузка комплектов
                    SqlCommand cmdKits = new SqlCommand("SELECT RaceKitOptionId, RaceKitOption, Cost FROM RaceKitOption", conn);
                    reader = cmdKits.ExecuteReader();
                    while (reader.Read())
                    {
                        raceKits.Add(new RaceKit
                        {
                            Id = reader["RaceKitOptionId"].ToString(),
                            Name = reader["RaceKitOption"].ToString(),
                            Cost = Convert.ToDecimal(reader["Cost"])
                        });
                    }
                    reader.Close();

                    // Загрузка благотворительных организаций
                    SqlCommand cmdCharity = new SqlCommand("SELECT CharityId, CharityName, CharityDescription FROM Charity", conn);
                    reader = cmdCharity.ExecuteReader();
                    while (reader.Read())
                    {
                        charities.Add(new Charity
                        {
                            Id = Convert.ToInt32(reader["CharityId"]),
                            Name = reader["CharityName"].ToString(),
                            Description = reader["CharityDescription"]?.ToString() ?? "Нет описания"
                        });
                    }
                    reader.Close();

                    lblConnectionStatus.Text = "✓ Данные успешно загружены";
                }
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "✗ Ошибка: " + ex.Message;
                lblConnectionStatus.ForeColor = Color.Red;

                // Тестовые данные на случай ошибки
                events.Add(new Event { Id = "1", Name = "Полный марафон (42 км)", Cost = 145 });
                events.Add(new Event { Id = "2", Name = "Полумарафон (21 км)", Cost = 75 });
                events.Add(new Event { Id = "3", Name = "Забег 5 км", Cost = 20 });

                raceKits.Add(new RaceKit { Id = "A", Name = "Базовый (номер + браслет)", Cost = 0 });
                raceKits.Add(new RaceKit { Id = "B", Name = "Стандарт (+ кепка + бутылка)", Cost = 20 });
                raceKits.Add(new RaceKit { Id = "C", Name = "Премиум (+ футболка + буклет)", Cost = 45 });

                charities.Add(new Charity { Id = 1, Name = "Красный Крест", Description = "Помощь в кризисных ситуациях" });
                charities.Add(new Charity { Id = 2, Name = "Спасём детей", Description = "Помощь детям" });
            }
        }

        private void CalculateTotal()
        {
            decimal total = 0;
            for (int i = 0; i < eventCheckBoxes.Count; i++)
                if (eventCheckBoxes[i].Checked) total += eventCosts[i];

            if (cbRaceKit.SelectedIndex >= 0 && cbRaceKit.SelectedIndex < raceKits.Count)
                total += raceKits[cbRaceKit.SelectedIndex].Cost;

            if (decimal.TryParse(txtSponsorship.Text, out decimal sponsorship) && sponsorship > 0)
                total += sponsorship;

            lblTotalCost.Text = total.ToString("F2");
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            bool atLeastOneEvent = false;
            foreach (var ch in eventCheckBoxes)
                if (ch.Checked) { atLeastOneEvent = true; break; }

            if (!atLeastOneEvent)
            {
                MessageBox.Show("Выберите хотя бы один забег!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtSponsorship.Text, out decimal sponsorship) || sponsorship < 0)
            {
                MessageBox.Show("Введите корректную сумму взноса!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Регистрация на марафон успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}