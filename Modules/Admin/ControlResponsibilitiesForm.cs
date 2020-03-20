using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ControlResponsibilitiesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;
        AdminWorkDays AdminWorkDays;

        DataTable TableUsers;

        Form TopForm = null;


        public ControlResponsibilitiesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void AdminModulesForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            AdminWorkDays = new AdminWorkDays(ref UserFunctionsDataGrid);

            TableUsers = new DataTable();
            TableUsers = AdminWorkDays.TableUsers();

            UserComboBox.DataSource = TableUsers;
            UserComboBox.DisplayMember = "Name";
            UserComboBox.ValueMember = "UserID";

            if (UserComboBox.SelectedValue != null && UserComboBox.ValueMember != "")
            {
                AdminWorkDays.UpdateUserFunctionsDataGrid(Convert.ToInt32(UserComboBox.SelectedValue));
            }

            if (AdminWorkDays.Comments == "")
            {
                CommentsResponsibleRichTextBox.Visible = false;
                label30.Visible = false;
            }
            else
            {
                CommentsResponsibleRichTextBox.Visible = true;
                label30.Visible = true;
                CommentsResponsibleRichTextBox.Text = AdminWorkDays.Comments;
            }

            if (DayRadioButton.Checked)
                DayRadioButton_CheckedChanged(null, null);
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

        private void UserComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserComboBox.SelectedValue != null && UserComboBox.ValueMember != "")
            {
                AdminWorkDays.UpdateUserFunctionsDataGrid(Convert.ToInt32(UserComboBox.SelectedValue));
                AdminWorkDays.UpdatePercentageColumns();
                CommentsResponsibleRichTextBox.Text = "";
                TotalLabel.Text = "";
                ClearLabel();
                if (DayRadioButton.Checked)
                {
                    CalendarFrom_DateChanged(null, null);
                }

                if (PeriodRadioButton.Checked)
                {
                    FilterButton_Click(null, null);
                }
            }
        }

        public void ClearLabel()
        {
            DayStartResponsibleLabel.Text = "";
            BreakStartResponsibleLabel.Text = "";
            BreakEndResponsibleLabel.Text = "";
            DayEndResponsibleLabel.Text = "";
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            AdminWorkDays.FilterMinutes(Convert.ToInt32(UserComboBox.SelectedValue), CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, DayRadioButton.Checked);
            AdminWorkDays.FillColumnMinutes();

            if (AdminWorkDays.Comments == "")
            {
                CommentsResponsibleRichTextBox.Visible = false;
                label30.Visible = false;
            }
            else
            {
                CommentsResponsibleRichTextBox.Visible = true;
                label30.Visible = true;
                CommentsResponsibleRichTextBox.Text = AdminWorkDays.Comments;
            }

            int min, hours;
            min = 0;
            hours = 0;
            hours = Convert.ToInt32(AdminWorkDays.TotalTime) / 60;
            min = Convert.ToInt32(AdminWorkDays.TotalTime) - hours * 60;
            TotalLabel.Text = hours.ToString() + " ч " + min.ToString() + " мин";

            if (DayRadioButton.Checked)
            {
                AdminWorkDays.DescriptionWorkDay(Convert.ToInt32(UserComboBox.SelectedValue), CalendarFrom.SelectionEnd);

                if (AdminWorkDays.DescriptionWorkDayDataTable.Rows.Count == 1)
                {
                    DayStartResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayStartDateTime"].ToString();
                    BreakStartResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayBreakStartDateTime"].ToString();
                    BreakEndResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayBreakEndDateTime"].ToString();
                    DayEndResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayEndDateTime"].ToString();
                }
                else
                {
                    ClearLabel();
                }
            }
        }

        private void CalendarFrom_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (DayRadioButton.Checked)
            {
                AdminWorkDays.FilterMinutes(Convert.ToInt32(UserComboBox.SelectedValue), CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, DayRadioButton.Checked);
                AdminWorkDays.FillColumnMinutes();

                if (AdminWorkDays.Comments == "")
                {
                    CommentsResponsibleRichTextBox.Visible = false;
                    label30.Visible = false;
                }
                else
                {
                    CommentsResponsibleRichTextBox.Visible = true;
                    label30.Visible = true;
                    CommentsResponsibleRichTextBox.Text = AdminWorkDays.Comments;
                }

                int min, hours;
                min = 0;
                hours = 0;
                hours = Convert.ToInt32(AdminWorkDays.TotalTime) / 60;
                min = Convert.ToInt32(AdminWorkDays.TotalTime) - hours * 60;
                TotalLabel.Text = hours.ToString() + " ч " + min.ToString() + " мин";

                if (DayRadioButton.Checked)
                {
                    AdminWorkDays.DescriptionWorkDay(Convert.ToInt32(UserComboBox.SelectedValue), CalendarFrom.SelectionEnd);

                    if (AdminWorkDays.DescriptionWorkDayDataTable.Rows.Count == 1)
                    {
                        DayStartResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayStartDateTime"].ToString();
                        BreakStartResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayBreakStartDateTime"].ToString();
                        BreakEndResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayBreakEndDateTime"].ToString();
                        DayEndResponsibleLabel.Text = AdminWorkDays.DescriptionWorkDayDataTable.Rows[0]["DayEndDateTime"].ToString();
                    }
                    else
                    {
                        ClearLabel();
                    }
                }
            }
            else
            {
                AdminWorkDays.FilterMinutes(Convert.ToInt32(UserComboBox.SelectedValue), CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, DayRadioButton.Checked);
                AdminWorkDays.FillColumnMinutes();

                int min, hours;
                min = 0;
                hours = 0;
                hours = Convert.ToInt32(AdminWorkDays.TotalTime) / 60;
                min = Convert.ToInt32(AdminWorkDays.TotalTime) - hours * 60;
                TotalLabel.Text = hours.ToString() + " ч " + min.ToString() + " мин";
            }
        }

        private void DayRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DayRadioButton.Checked)
            {
                CalendarTo.Visible = false;
                FilterButton.Top = 341;

                label23.Visible = true;
                label25.Visible = true;
                label27.Visible = true;
                label28.Visible = true;
                DayStartResponsibleLabel.Visible = true;
                BreakStartResponsibleLabel.Visible = true;
                BreakEndResponsibleLabel.Visible = true;
                DayEndResponsibleLabel.Visible = true;

                CommentsResponsibleRichTextBox.Visible = true;
                label30.Visible = true;

                ClearLabel();
            }
        }

        private void PeriodRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            CalendarTo.Visible = true;
            FilterButton.Top = 534;

            label23.Visible = false;
            label25.Visible = false;
            label27.Visible = false;
            label28.Visible = false;
            DayStartResponsibleLabel.Visible = false;
            BreakStartResponsibleLabel.Visible = false;
            BreakEndResponsibleLabel.Visible = false;
            DayEndResponsibleLabel.Visible = false;

            CommentsResponsibleRichTextBox.Visible = false;
            label30.Visible = false;

            ClearLabel();
        }
    }
}
