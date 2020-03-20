using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class EditTimeForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;
        int DayEventType;

        public const int dStartDay = 0;
        public const int dBreakDay = 1;
        public const int dContinueDay = 2;
        public const int dEndDay = 3;

        bool IsOverdued = false;

        DateTime OverduedDateTime = DateTime.Now;

        Form TopForm;

        public EditTimeForm(ref Form tTopForm, int tDayEventType, ref DayFactStatus DayFactStatus, ref DayStatus DayStatus)
        {
            InitializeComponent();

            DayEventType = tDayEventType;

            TopForm = tTopForm;

            if (DayEventType == dBreakDay)
            {
                label2.Text = "Изменить время начала перерыва";

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }

                timeEdit1.EditValue = DayStatus.BreakStarted;
                FactTimeLabel.Text = "с " + DayFactStatus.BreakFactStarted.ToString("HH:mm") + " на";
                richTextBox1.Text = DayFactStatus.DayBreakStartNotes;
            }
            if (DayEventType == dContinueDay)
            {
                label2.Text = "Изменить время окончания перерыва";

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }

                timeEdit1.EditValue = DayStatus.BreakEnded;
                FactTimeLabel.Text = "с " + DayFactStatus.BreakFactEnded.ToString("HH:mm") + " на";
                richTextBox1.Text = DayFactStatus.DayContinueNotes;
            }
            if (DayEventType == dStartDay)
            {
                label2.Text = "Изменить время начала рабочего дня";

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }

                timeEdit1.EditValue = DayStatus.DayStarted;
                FactTimeLabel.Text = "с " + DayFactStatus.DayFactStarted.ToString("HH:mm") + " на";
                richTextBox1.Text = DayFactStatus.DayStartNotes;
            }
            if (DayEventType == dEndDay)
            {
                label2.Text = "Изменить время завершения рабочего дня";

                if (LightWorkDay.IsDayOverdued(Security.CurrentUserID, ref OverduedDateTime))
                {
                    OverduedDateLabel.Visible = true;
                    OverduedDateLabel.Text = OverduedDateTime.ToString("dd.MM.yyyy");
                    IsOverdued = true;
                }

                timeEdit1.EditValue = DayStatus.DayEnded;
                FactTimeLabel.Text = "с " + DayFactStatus.DayFactEnded.ToString("HH:mm") + " на";
                richTextBox1.Text = DayFactStatus.DayEndNotes;
            }

            if (((DateTime)timeEdit1.EditValue).ToString("dd.MM.yyyy") == "01.01.0001")
                timeEdit1.EditValue = OverduedDateTime;
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

        private void EditTimeForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
                return;

            DateTime D;

            if (IsOverdued)
            {
                D = Convert.ToDateTime(OverduedDateTime.ToShortDateString() + " " + timeEdit1.Time.ToString("HH:mm:ss.fff"));
            }
            else
            {
                D = Convert.ToDateTime(timeEdit1.EditValue);
            }

            if (DayEventType == dStartDay)
            {
                LightWorkDay.ChangeStartTime(Security.CurrentUserID, D, richTextBox1.Text);
            }
            if (DayEventType == dEndDay)
            {
                LightWorkDay.ChangeEndTime(Security.CurrentUserID, D, richTextBox1.Text);
            }
            if (DayEventType == dBreakDay)
            {
                LightWorkDay.ChangeBreakStartTime(Security.CurrentUserID, D, richTextBox1.Text);
            }
            if (DayEventType == dContinueDay)
            {
                LightWorkDay.ChangeBreakEndTime(Security.CurrentUserID, D, richTextBox1.Text);
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
