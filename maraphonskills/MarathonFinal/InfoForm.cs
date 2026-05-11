using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class InfoForm : Form
    {
        public InfoForm()
        {
            this.Text = "О марафоне";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            RichTextBox rtb = new RichTextBox
            {
                Location = new Point(20, 20),
                Size = new Size(540, 400),
                ReadOnly = true,
                Font = new Font("Arial", 11),
                Text = @"MARATHON SKILLS 2016

Место проведения: Сан-Паоло, Бразилия
Дата: 24 ноября 2017 года, старт в 6:00

Дистанции:
• Полный марафон (42 км) - старт в 6:00
• Полумарафон (21 км) - старт в 7:00
• Забег для новичков (5 км) - старт в 15:00

Добро пожаловать!"
            };

            Button btnClose = new Button { Text = "Закрыть", Location = new Point(240, 440), Size = new Size(100, 40) };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(rtb);
            this.Controls.Add(btnClose);
        }
    }
}