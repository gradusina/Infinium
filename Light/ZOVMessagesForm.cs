using System;
using System.Data;
using System.Windows.Forms;
namespace Infinium
{
    public partial class ZOVMessagesForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;


        Form TopForm = null;

        ZOVMessages ZOVMessages;

        public ZOVMessagesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();

            messagesContainer1.CurrentUserName = Security.GetUserNameByID(Security.CurrentUserID);
            messagesContainer1.MessagesDataTable = ZOVMessages.MessagesDataTable;

            UsersListDataGrid.DataSource = ZOVMessages.ManagersBindingSource;
            UsersListDataGrid.Columns["ManagerID"].Visible = false;
            //UsersListDataGrid.Columns["ClientID"].Visible = false;

            //SelectedUsersGrid.Columns["ClientID"].Visible = false;
            SelectedUsersGrid.Columns["ManagerID"].Visible = false;
            SelectedUsersGrid.Columns["UpdatesCount"].Visible = false;

            GetNewMessages();

            OnlineTimer_Tick(null, null);

            kryptonCheckSet1_CheckedButtonChanged(null, null);

            while (!SplashForm.bCreated) ;
        }

        //private bool PermissionGranted(int PermissionID)
        //{
        //    DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

        //    if (Rows.Count() > 0)
        //    {
        //        return Convert.ToBoolean(Rows[0]["Granted"]);
        //    }

        //    return false;
        //}

        private void MessagesForm_Shown(object sender, EventArgs e)
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
            ZOVMessages = new ZOVMessages(ref SelectedUsersGrid, ref UsersListDataGrid);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            SelectedUsersGrid.bDrawOnlineImage = !SelectedUsersGrid.bDrawOnlineImage;
            SelectedUsersGrid.Refresh();
        }

        private void OnlineTimer_Tick(object sender, EventArgs e)
        {
            ZOVMessages.CheckOnline();
            UsersListDataGrid.SuspendLayout();
            UsersListDataGrid.Refresh();
            UsersListDataGrid.ResumeLayout();

            SelectedUsersGrid.SuspendLayout();
            SelectedUsersGrid.Refresh();
            SelectedUsersGrid.ResumeLayout();
        }

        private void UsersListDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectedUsersGrid.Rows.Count == 0)
            {
                NameLabel.Text = "";
                messagesContainer1.Clear();
                return;
            }
        }

        private void UsersListDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ZOVMessages.AddUserToSelected(UsersListDataGrid.SelectedRows[0].Cells["ManagerID"].Value.ToString());
        }

        private void SelectedUsersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((SelectedUsersGrid.Columns[e.ColumnIndex].GetType().Equals(typeof(DataGridViewImageColumn))))
            {
                ZOVMessages.RemoveCurrent();
                messagesContainer1.ClearCurrent();
            }

            SelectedUsersGrid.Cursor = Cursors.Default;
        }

        private void SelectedUsersGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            SelectedUsersGrid.Cursor = Cursors.Default;
        }

        private void SelectedUsersGrid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (SelectedUsersGrid.Columns[e.ColumnIndex].GetType().Equals(typeof(DataGridViewImageColumn)))
                SelectedUsersGrid.Cursor = Cursors.Hand;
            else
                SelectedUsersGrid.Cursor = Cursors.Default;
        }

        private void SelectedUsersGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectedUsersGrid.Rows.Count == 0)
            {
                NameLabel.Text = "";
                messagesContainer1.Clear();
                return;
            }

            ZOVMessages.ClearCurrentUpdates();

            ZOVMessages.FillMessages(Convert.ToInt32(((DataRowView)ZOVMessages.SelectedUsersBindingSource.Current)["ManagerID"]));
            NameLabel.Text = ((DataRowView)ZOVMessages.SelectedUsersBindingSource.Current)["Name"].ToString();

            messagesContainer1.AddDataClient(Convert.ToInt32(((DataRowView)ZOVMessages.SelectedUsersBindingSource.Current)["ManagerID"]));

            richTextBox1.Focus();
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && e.Control)
            {
                SendButton_Click(sender, null);
                e.Handled = true;
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (SelectedUsersGrid.Rows.Count == 0)
                return;

            if (ZOVMessages.IsEmptyMessage(richTextBox1.Text))
                return;

            ZOVMessages.AddMessage((Convert.ToInt32(((DataRowView)ZOVMessages.SelectedUsersBindingSource.Current)["ManagerID"])), richTextBox1.Text.TrimEnd());
            messagesContainer1.AddDataClient((Convert.ToInt32(((DataRowView)ZOVMessages.SelectedUsersBindingSource.Current)["ManagerID"])));
            richTextBox1.Clear();
        }

        private void messagesContainer1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void ClientsMessagesForm_ANSUpdate(object sender)
        {
            GetNewMessages();
        }

        public void GetNewMessages()
        {
            ZOVMessages.GetNewMessages();

            SelectedUsersGrid_SelectionChanged(null, null);

            SelectedUsersGrid.Refresh();

            ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ZOVMessages == null)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "UsersFilterOnline")
                ZOVMessages.ManagersBindingSource.Sort = "OnlineStatus DESC";

            if (kryptonCheckSet1.CheckedButton.Name == "UsersFilterAlpha")
                ZOVMessages.ManagersBindingSource.Sort = "Name ASC";
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }
    }
}
