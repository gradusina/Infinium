using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DateCorrectForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        public const int dStartDay = 0;
        public const int dBreakDay = 1;
        public const int dContinueDay = 2;
        public const int dEndDay = 3;

        bool IsOverdued = false;

        DateTime OverduedDateTime = DateTime.Now;

        Form TopForm;

        public DateCorrectForm(ref Form tTopForm, int DayEventType)
        {
            InitializeComponent();

            TopForm = tTopForm;

            if (DayEventType == dBreakDay)
            {
                label2.Text = "Пойти на перерыв";
                BreakButton.BringToFront();
                BreakButtonChanged.BringToFront();

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    xtraTabPage1.PageEnabled = false;
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }
            }
            if (DayEventType == dContinueDay)
            {
                label2.Text = "Продолжить рабочий день";
                ContinueButton.BringToFront();
                ContinueButtonChanged.BringToFront();

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    xtraTabPage1.PageEnabled = false;
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }
            }
            if (DayEventType == dStartDay)
            {
                label2.Text = "Начать рабочий день";
                StartButton.BringToFront();
                StartButtonChanged.BringToFront();
            }
            if (DayEventType == dEndDay)
            {
                label2.Text = "Завершить рабочий день";
                StopButton.BringToFront();
                StopButtonChanged.BringToFront();

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    xtraTabPage1.PageEnabled = false;
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }
            }

            label1.Text = Security.GetCurrentDate().ToString("HH:mm");
            timeEdit1.EditValue = Security.GetCurrentDate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (FormEvent == eClose || FormEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        this.Hide();
                    }

                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    return;
                }

            }

            if (FormEvent == eClose || FormEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        this.Hide();
                    }
                }

                return;
            }


            if (FormEvent == eShow || FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                }

                return;
            }
        }

        private void NewsCommentsForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            LightWorkDay.StartWorkDay(Security.CurrentUserID);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void StartButtonChanged_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
                return;

            LightWorkDay.StartWorkDay(Security.CurrentUserID, Convert.ToDateTime(timeEdit1.EditValue), richTextBox1.Text);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void BreakButton_Click(object sender, EventArgs e)
        {
            LightWorkDay.BreakStartWorkDay(Security.CurrentUserID);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void BreakButtonChanged_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
                return;

            DateTime D;

            if (IsOverdued)
            {
                D = Convert.ToDateTime(OverduedDateTime.ToShortDateString() + " " + timeEdit1.Time.ToString("HH:mm:ss.fff"));
                LightWorkDay.BreakStartWorkDay(Security.CurrentUserID, D, richTextBox1.Text, true);
            }
            else
            {
                D = Convert.ToDateTime(timeEdit1.EditValue);
                LightWorkDay.BreakStartWorkDay(Security.CurrentUserID, D, richTextBox1.Text);
            }



            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {

            LightWorkDay.EndWorkDay(Security.CurrentUserID);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void StopButtonChanged_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
                return;

            //if (Convert.ToDateTime(timeEdit1.EditValue) > DateTime.Now)
            //{
            //    MessageBox.Show("«Не торопитесь жить!» — © А.В.Литвиненко");
            //    return;
            //}

            DateTime D;

            if (IsOverdued)
            {
                D = Convert.ToDateTime(OverduedDateTime.ToShortDateString() + " " + timeEdit1.Time.ToString("HH:mm:ss.fff"));
                LightWorkDay.EndWorkDay(Security.CurrentUserID, D, richTextBox1.Text, true);
            }
            else
            {
                D = Convert.ToDateTime(timeEdit1.EditValue);
                LightWorkDay.EndWorkDay(Security.CurrentUserID, D, richTextBox1.Text);
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            LightWorkDay.ContinueWorkDay(Security.CurrentUserID);

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ContinueButtonChanged_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
                return;

            DateTime D;

            if (IsOverdued)
            {
                D = Convert.ToDateTime(OverduedDateTime.ToShortDateString() + " " + timeEdit1.Time.ToString("HH:mm:ss.fff"));
                LightWorkDay.ContinueWorkDay(Security.CurrentUserID, D, richTextBox1.Text, true);
            }
            else
            {
                D = Convert.ToDateTime(timeEdit1.EditValue);
                LightWorkDay.ContinueWorkDay(Security.CurrentUserID, D, richTextBox1.Text);
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
