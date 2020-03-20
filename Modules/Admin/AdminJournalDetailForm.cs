using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminJournalDetailForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        AdminJournalDetail AdminJournalDetail;

        Bitmap OnTopBitmap;

        public AdminJournalDetailForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            OnTopBitmap = new Bitmap(Properties.Resources.OnTop);



            OnlinerTimer_Tick(null, null);

            while (!SplashForm.bCreated) ;
        }


        private void AdminJournalDetailForm_Shown(object sender, EventArgs e)
        {
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
            AdminJournalDetail = new AdminJournalDetail(ref UsersDataGrid, ref LoginJournalDataGrid, ref ComputerParamsDataGrid,
                                                        ref ModulesJornalDataGrid, ref RichTextBox, ref MessagesDataGrid);

            DateFromPicker.Value = DateTime.Now;
            DateToPicker.Value = DateTime.Now;

            Filter();
            AdminJournalDetail.FillMessages(MessagesFromDateTimePicker.Value, MessagesToDateTimePicker.Value);
            UsersDataGrid.AddColumns();
            AdminJournalDetail.SetGrid();
        }

        private void Filter()
        {
            AdminJournalDetail.FillUsers(DateFromPicker.Value, DateToPicker.Value);

            if (AdminJournalDetail.UsersBindingSource.Count > 0)
                AdminJournalDetail.UsersBindingSource.Position = 0;

            //int UserID = -1;

            //if (SelectedUserCheckBox.Checked)
            //    UserID = Convert.ToInt32(UsersComboBox.SelectedValue);

            //AdminJournalDetail.FillJournal(UserID, DateFromPicker.Value, DateToPicker.Value, CodersCheckBox.Checked, OCheckBox.Checked);
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

        private void DateFromPicker_ValueChanged(object sender, EventArgs e)
        {
            Filter();
            kryptonCheckSet1_CheckedButtonChanged(null, null);
        }

        private void DateToPicker_ValueChanged(object sender, EventArgs e)
        {
            Filter();
            kryptonCheckSet1_CheckedButtonChanged(null, null);
        }

        private void OnlinerTimer_Tick(object sender, EventArgs e)
        {
            AdminJournalDetail.FillUsers(DateFromPicker.Value, DateToPicker.Value);
            AdminJournalDetail.CheckOnline();
            AdminJournalDetail.ClearTopMostAndTopModuleAndIdle();
            UsersDataGrid.SuspendLayout();
            UsersDataGrid.Refresh();
            UsersDataGrid.ResumeLayout();

            TotalUsersLabel.Text = "Всего: " + AdminJournalDetail.UsersBindingSource.Count;
            OnlineLabel.Text = "Онлайн: " + AdminJournalDetail.GetOnlineUsersCount();

            AdminJournalDetail.FillRichTextBox();

        }

        private void OnlineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (AdminJournalDetail == null)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "UsersFilterOnline")
                AdminJournalDetail.UsersBindingSource.Sort = "OnlineStatus DESC";

            if (kryptonCheckSet1.CheckedButton.Name == "UsersFilterAlpha")
                AdminJournalDetail.UsersBindingSource.Sort = "ShortName ASC";

            if (AdminJournalDetail.UsersBindingSource.Count > 0)
                AdminJournalDetail.UsersBindingSource.Position = 0;
        }

        private void UsersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminJournalDetail.UsersBindingSource.Count > 0)
                AdminJournalDetail.FillLoginJournal(Convert.ToInt32(((DataRowView)AdminJournalDetail.UsersBindingSource.Current)["UserID"]), DateFromPicker.Value, DateToPicker.Value);
        }

        private void LoginJournalDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminJournalDetail.LoginJournalBindingSource.Count > 0)
            {
                AdminJournalDetail.FillComputerParams(Convert.ToInt32(((DataRowView)AdminJournalDetail.LoginJournalBindingSource.Current)["LoginJournalID"]));
                AdminJournalDetail.FillModulesJournal(Convert.ToInt32(((DataRowView)AdminJournalDetail.LoginJournalBindingSource.Current)["LoginJournalID"]));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void AdminJournalDetailForm_Load(object sender, EventArgs e)
        {

        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet2.CheckedButton.Name == "SimpleButton")
                CurrentJournalPanel.BringToFront();

            if (kryptonCheckSet2.CheckedButton.Name == "FullButton")
                panel5.BringToFront();
        }

        private void UsersDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == UsersDataGrid.Columns["IdleTime"].Index)
            {
                if (UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString() != "-" && UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString() != "")
                {
                    int Total = Convert.ToDateTime(UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString()).Hour * 3600 +
                                Convert.ToDateTime(UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString()).Minute * 60 +
                                Convert.ToDateTime(UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString()).Second;

                    if (Total < 30)
                        UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].Style.BackColor = Color.FromArgb(103, 202, 79);
                    else
                        UsersDataGrid.Rows[e.RowIndex].Cells["IdleTime"].Style.BackColor = Color.White;
                }

                return;
            }

            if (e.ColumnIndex == UsersDataGrid.Columns["OnTop"].Index)
            {
                if (Convert.ToBoolean(UsersDataGrid.Rows[e.RowIndex].Cells["TopMost"].Value) == true)
                {
                    e.Graphics.DrawImage(OnTopBitmap, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnTopBitmap.Width - 5) / 2) - 2,
                                                      e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnTopBitmap.Height - 5) / 2) - 1,
                                                      OnTopBitmap.Width - 5,
                                                      OnTopBitmap.Height - 5);
                }
            }
        }

        private void MessagesFromDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            AdminJournalDetail.FillMessages(MessagesFromDateTimePicker.Value, MessagesToDateTimePicker.Value);
        }
    }
}
