using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class CertificateForm : Form
    {
        private string runnerName;
        private string eventName;
        private string resultTime;
        private int place;
        private DateTime eventDate;

        public CertificateForm(string runnerName, string eventName, string resultTime, int place, DateTime eventDate)
        {
            this.runnerName = runnerName;
            this.eventName = eventName;
            this.resultTime = resultTime;
            this.place = place;
            this.eventDate = eventDate;

            this.Text = "Сертификат участника";
            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Панель для предпросмотра
            Panel panelPreview = new Panel()
            {
                Location = new Point(20, 20),
                Size = new Size(780, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Рисуем сертификат на панели
            panelPreview.Paint += PanelPreview_Paint;

            // Кнопка печати
            Button btnPrint = new Button()
            {
                Text = "🖨️ Печать сертификата",
                Location = new Point(300, 540),
                Size = new Size(200, 50),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            btnPrint.Click += BtnPrint_Click;

            // Кнопка закрытия
            Button btnClose = new Button()
            {
                Text = "Закрыть",
                Location = new Point(600, 540),
                Size = new Size(150, 40),
                BackColor = Color.LightCoral
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { panelPreview, btnPrint, btnClose });
        }

        private void PanelPreview_Paint(object sender, PaintEventArgs e)
        {
            DrawCertificate(e.Graphics, new Rectangle(20, 20, 740, 460));
        }

        private void DrawCertificate(Graphics g, Rectangle rect)
        {
            // Рамка
            g.DrawRectangle(new Pen(Color.Gold, 5), rect);
            g.DrawRectangle(new Pen(Color.DarkGoldenrod, 2), rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10);

            // Фоновый узор (простая штриховка)
            for (int i = 0; i < 10; i++)
            {
                g.DrawLine(new Pen(Color.LightGray, 1), rect.X + i * 50, rect.Y, rect.X + i * 50, rect.Y + rect.Height);
                g.DrawLine(new Pen(Color.LightGray, 1), rect.X, rect.Y + i * 30, rect.X + rect.Width, rect.Y + i * 30);
            }

            // Логотип (золотая медаль)
            Rectangle logoRect = new Rectangle(rect.X + rect.Width / 2 - 40, rect.Y + 20, 80, 80);
            g.FillEllipse(Brushes.Gold, logoRect);
            g.DrawEllipse(new Pen(Color.DarkGoldenrod, 3), logoRect);
            g.DrawString("★", new Font("Arial", 36, FontStyle.Bold), Brushes.DarkGoldenrod,
                rect.X + rect.Width / 2 - 20, rect.Y + 35);

            // Заголовок
            Font titleFont = new Font("Arial", 24, FontStyle.Bold);
            g.DrawString("MARATHON SKILLS 2017", titleFont, Brushes.DarkBlue,
                rect.X + rect.Width / 2 - 150, rect.Y + 110);

            // Подзаголовок
            Font subFont = new Font("Arial", 14, FontStyle.Italic);
            g.DrawString("Сертификат участника", subFont, Brushes.Gray,
                rect.X + rect.Width / 2 - 100, rect.Y + 155);

            // Имя бегуна
            Font nameFont = new Font("Arial", 18, FontStyle.Bold);
            SizeF nameSize = g.MeasureString(runnerName, nameFont);
            g.DrawString(runnerName, nameFont, Brushes.DarkGreen,
                rect.X + rect.Width / 2 - nameSize.Width / 2, rect.Y + 210);

            // Текст сертификата
            Font textFont = new Font("Arial", 12);
            g.DrawString("успешно финишировал(а) в забеге", textFont, Brushes.Black,
                rect.X + rect.Width / 2 - 150, rect.Y + 260);

            // Название забега
            Font eventFont = new Font("Arial", 16, FontStyle.Bold);
            SizeF eventSize = g.MeasureString(eventName, eventFont);
            g.DrawString(eventName, eventFont, Brushes.DarkRed,
                rect.X + rect.Width / 2 - eventSize.Width / 2, rect.Y + 300);

            // Результат и место
            g.DrawString($"с результатом {resultTime}", textFont, Brushes.Black,
                rect.X + rect.Width / 2 - 100, rect.Y + 350);

            g.DrawString($"заняв {place} место", textFont, Brushes.Black,
                rect.X + rect.Width / 2 - 80, rect.Y + 385);

            // Дата
            g.DrawString($"Дата: {eventDate:dd MMMM yyyy}", new Font("Arial", 10), Brushes.Gray,
                rect.X + 50, rect.Y + rect.Height - 60);

            // Подпись
            g.DrawString("Директор марафона", new Font("Arial", 10, FontStyle.Italic), Brushes.Black,
                rect.X + rect.Width - 180, rect.Y + rect.Height - 80);
            g.DrawLine(new Pen(Brushes.Black, 1), rect.X + rect.Width - 180, rect.Y + rect.Height - 60,
                rect.X + rect.Width - 50, rect.Y + rect.Height - 60);

            // Номер сертификата
            string certNumber = $"№ {DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
            g.DrawString(certNumber, new Font("Arial", 8), Brushes.Gray,
                rect.X + rect.Width - 150, rect.Y + 20);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
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
            Rectangle rect = new Rectangle(50, 50, 700, 500);
            DrawCertificate(e.Graphics, rect);
        }
    }
}