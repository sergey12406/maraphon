using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarathonFinal
{
    public class CheckRunnerForm : Form
    {
        public CheckRunnerForm()
        {
            this.Text = "Проверка бегуна";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblQuestion = new Label
            {
                Text = "Вы уже регистрировались на марафон ранее?",
                Location = new Point(50, 50),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnNew = new Button
            {
                Text = "Я НОВЫЙ бегун",
                Location = new Point(50, 120),
                Size = new Size(280, 50),
                BackColor = Color.LightBlue
            };

            Button btnExisting = new Button
            {
                Text = "Я УЖЕ регистрировался",
                Location = new Point(50, 190),
                Size = new Size(280, 50),
                BackColor = Color.LightGreen
            };

            // ЭТО ГЛАВНОЕ - кнопка открывает форму регистрации
            btnNew.Click += (s, e) => { new RegisterRunnerForm().ShowDialog(); };

            btnExisting.Click += (s, e) => { new LoginForm().ShowDialog(); this.Close(); };

            this.Controls.Add(lblQuestion);
            this.Controls.Add(btnNew);
            this.Controls.Add(btnExisting);
        }
    }
}