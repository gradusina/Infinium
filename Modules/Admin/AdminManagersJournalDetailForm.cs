using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminManagersJournalDetailForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        AdminManagersJournalDetail AdminManagersJournalDetail;

        Bitmap OnTopBitmap;

        public AdminManagersJournalDetailForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            OnTopBitmap = new Bitmap(Properties.Resources.OnTop);

            OnlinerTimer_Tick(null, null);

            while (!SplashForm.bCreated) ;
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
            AdminManagersJournalDetail = new AdminManagersJournalDetail(ref ClientsDataGrid, ref LoginJournalDataGrid, ref ComputerParamsDataGrid,
                                                        ref ModulesJornalDataGrid, ref MessagesDataGrid, ref ClientEventsJournalDataGrid);

            DateFromPicker.Value = DateTime.Now;
            DateToPicker.Value = DateTime.Now;

            Filter();
            AdminManagersJournalDetail.FillMessages(MessagesFromDateTimePicker.Value, MessagesToDateTimePicker.Value);
            ClientsDataGrid.AddColumns();
            AdminManagersJournalDetail.SetGrid();
        }

        private void Filter()
        {
            AdminManagersJournalDetail.FillClients(DateFromPicker.Value, DateToPicker.Value);

            if (AdminManagersJournalDetail.ManagersBindingSource.Count > 0)
                AdminManagersJournalDetail.ManagersBindingSource.Position = 0;

            //int ClientID = -1;

            //if (SelectedUserCheckBox.Checked)
            //    ClientID = Convert.ToInt32(UsersComboBox.SelectedValue);

            //AdminJournalDetail.FillJournal(ClientID, DateFromPicker.Value, DateToPicker.Value, CodersCheckBox.Checked, OCheckBox.Checked);
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
            AdminManagersJournalDetail.FillClients(DateFromPicker.Value, DateToPicker.Value);
            AdminManagersJournalDetail.CheckOnline();
            AdminManagersJournalDetail.ClearTopMostAndTopModuleAndIdle();
            ClientsDataGrid.SuspendLayout();
            ClientsDataGrid.Refresh();
            ClientsDataGrid.ResumeLayout();

            TotalUsersLabel.Text = "Всего: " + AdminManagersJournalDetail.ManagersBindingSource.Count;
            OnlineLabel.Text = "Онлайн: " + AdminManagersJournalDetail.GetOnlineUsersCount();
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (AdminManagersJournalDetail == null)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "UsersFilterOnline")
                AdminManagersJournalDetail.ManagersBindingSource.Sort = "OnlineStatus DESC";

            if (kryptonCheckSet1.CheckedButton.Name == "UsersFilterAlpha")
                AdminManagersJournalDetail.ManagersBindingSource.Sort = "Name ASC";

            if (AdminManagersJournalDetail.ManagersBindingSource.Count > 0)
                AdminManagersJournalDetail.ManagersBindingSource.Position = 0;
        }

        private void UsersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminManagersJournalDetail.ManagersBindingSource.Count > 0)
                AdminManagersJournalDetail.FillLoginJournal(Convert.ToInt32(((DataRowView)AdminManagersJournalDetail.ManagersBindingSource.Current)["ManagerID"]), DateFromPicker.Value, DateToPicker.Value);
        }

        private void LoginJournalDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminManagersJournalDetail.LoginJournalBindingSource.Count > 0)
            {
                AdminManagersJournalDetail.FillComputerParams(Convert.ToInt32(((DataRowView)AdminManagersJournalDetail.LoginJournalBindingSource.Current)["ManagersLoginJournalID"]));
                AdminManagersJournalDetail.FillModulesJournal(Convert.ToInt32(((DataRowView)AdminManagersJournalDetail.LoginJournalBindingSource.Current)["ManagersLoginJournalID"]));
                AdminManagersJournalDetail.FillEventsJournal(Convert.ToInt32(((DataRowView)AdminManagersJournalDetail.LoginJournalBindingSource.Current)["ManagersLoginJournalID"]));
            }
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet2.CheckedButton.Name == "FullButton")
                panel5.BringToFront();

            if (kryptonCheckSet2.CheckedButton.Name == "ClientEventsButton")
                panel8.BringToFront();
        }

        private void UsersDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == ClientsDataGrid.Columns["IdleTime"].Index)
            {
                if (ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString() != "-" && ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString() != "")
                {
                    int Total = Convert.ToDateTime(ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString()).Hour * 3600 +
                                Convert.ToDateTime(ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString()).Minute * 60 +
                                Convert.ToDateTime(ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].FormattedValue.ToString()).Second;

                    if (Total < 30)
                        ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].Style.BackColor = Color.FromArgb(103, 202, 79);
                    else
                        ClientsDataGrid.Rows[e.RowIndex].Cells["IdleTime"].Style.BackColor = Color.White;
                }

                return;
            }

            if (e.ColumnIndex == ClientsDataGrid.Columns["OnTop"].Index)
            {
                if (Convert.ToBoolean(ClientsDataGrid.Rows[e.RowIndex].Cells["TopMost"].Value) == true)
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
            AdminManagersJournalDetail.FillMessages(MessagesFromDateTimePicker.Value, MessagesToDateTimePicker.Value);
        }

        private void AdminManagersJournalDetailForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }
    }
}
