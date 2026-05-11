using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            this.Text = "О системе";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lbl = new Label
            {
                Text = "Marathon Skills 2016\n\nСистема для организаторов марафона\n\nДемонстрационный экзамен\nWorldSkills Russia",
                Location = new Point(50, 50),
                Size = new Size(400, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12)
            };

            Button btnClose = new Button { Text = "Закрыть", Location = new Point(200, 300), Size = new Size(100, 40) };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lbl);
            this.Controls.Add(btnClose);
        }
    }
}