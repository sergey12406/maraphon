using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class FormBMI : Form
    {
        private TrackBar trackHeight;
        private TrackBar trackWeight;
        private Label lblHeightVal;
        private Label lblWeightVal;
        private Label lblBMIVal;
        private Label lblCategory;
        private PictureBox pbScale;

        public FormBMI()
        {
            this.Text = "BMI Калькулятор";
            this.Size = new Size(500, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Рост
            Label lblHeight = new Label()
            {
                Text = "Рост (см):",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = new Font("Arial", 10)
            };

            trackHeight = new TrackBar()
            {
                Minimum = 100,
                Maximum = 250,
                Value = 170,
                Location = new Point(130, 15),
                Size = new Size(250, 45),
                TickFrequency = 10
            };
            trackHeight.ValueChanged += RecalcBMI;

            lblHeightVal = new Label()
            {
                Text = "170",
                Location = new Point(400, 20),
                Size = new Size(50, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            // Вес
            Label lblWeight = new Label()
            {
                Text = "Вес (кг):",
                Location = new Point(20, 70),
                Size = new Size(100, 25),
                Font = new Font("Arial", 10)
            };

            trackWeight = new TrackBar()
            {
                Minimum = 30,
                Maximum = 200,
                Value = 70,
                Location = new Point(130, 65),
                Size = new Size(250, 45),
                TickFrequency = 10
            };
            trackWeight.ValueChanged += RecalcBMI;

            lblWeightVal = new Label()
            {
                Text = "70",
                Location = new Point(400, 70),
                Size = new Size(50, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            // Результат BMI
            Label lblBMI = new Label()
            {
                Text = "Ваш BMI:",
                Location = new Point(20, 130),
                Size = new Size(100, 30),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            lblBMIVal = new Label()
            {
                Text = "0.0",
                Location = new Point(130, 128),
                Size = new Size(100, 35),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Blue
            };

            lblCategory = new Label()
            {
                Text = "",
                Location = new Point(20, 165),
                Size = new Size(450, 25),
                Font = new Font("Arial", 11),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Цветная шкала
            pbScale = new PictureBox()
            {
                Location = new Point(20, 210),
                Size = new Size(440, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка закрытия
            Button btnClose = new Button()
            {
                Text = "Закрыть",
                Location = new Point(200, 330),
                Size = new Size(100, 35),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10)
            };
            btnClose.Click += (s, e) => this.Close();

            // Добавляем все элементы
            this.Controls.AddRange(new Control[] {
                lblHeight, trackHeight, lblHeightVal,
                lblWeight, trackWeight, lblWeightVal,
                lblBMI, lblBMIVal, lblCategory, pbScale, btnClose
            });

            // Рисуем шкалу и считаем BMI
            DrawBMIScale();
            RecalcBMI(null, null);
        }

        private void DrawBMIScale()
        {
            Bitmap bmp = new Bitmap(438, 48);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Рисуем цветные зоны
                for (int bmi = 10; bmi <= 45; bmi++)
                {
                    int x = (bmi - 10) * 12;
                    if (x >= 438) break;

                    Color zoneColor;
                    if (bmi < 18.5) zoneColor = Color.LightBlue;
                    else if (bmi < 25) zoneColor = Color.LightGreen;
                    else if (bmi < 30) zoneColor = Color.Yellow;
                    else zoneColor = Color.LightCoral;

                    using (Brush brush = new SolidBrush(zoneColor))
                    {
                        g.FillRectangle(brush, x, 0, 13, 48);
                    }
                }

                // Рисуем подписи
                g.DrawString("16", new Font("Arial", 8), Brushes.Black, 70, 35);
                g.DrawString("18.5", new Font("Arial", 8), Brushes.Black, 100, 35);
                g.DrawString("25", new Font("Arial", 8), Brushes.Black, 180, 35);
                g.DrawString("30", new Font("Arial", 8), Brushes.Black, 240, 35);
                g.DrawString("40", new Font("Arial", 8), Brushes.Black, 360, 35);
            }
            pbScale.Image = bmp;
        }

        private void RecalcBMI(object sender, EventArgs e)
        {
            double heightM = trackHeight.Value / 100.0;
            double weight = trackWeight.Value;
            double bmi = weight / (heightM * heightM);

            lblHeightVal.Text = trackHeight.Value.ToString();
            lblWeightVal.Text = trackWeight.Value.ToString();
            lblBMIVal.Text = bmi.ToString("F1");

            string category;
            Color categoryColor;

            if (bmi < 18.5)
            {
                category = "Недостаточный вес";
                categoryColor = Color.Blue;
            }
            else if (bmi < 25)
            {
                category = "Нормальный вес";
                categoryColor = Color.Green;
            }
            else if (bmi < 30)
            {
                category = "Избыточный вес";
                categoryColor = Color.Orange;
            }
            else
            {
                category = "Ожирение";
                categoryColor = Color.Red;
            }

            lblCategory.Text = $"Категория: {category}";
            lblCategory.ForeColor = categoryColor;

            // Обновляем маркер на шкале
            int markerX = (int)((bmi - 10) * 12);
            if (markerX >= 0 && markerX <= 438)
            {
                Bitmap bmp = new Bitmap(pbScale.Image);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (Pen redPen = new Pen(Color.Red, 3))
                    {
                        g.DrawLine(redPen, markerX, 5, markerX, 43);
                    }
                }
                pbScale.Image = bmp;
            }
        }
    }
}