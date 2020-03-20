using Infinium.Modules.Marketing.ClientMessages;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MenuClientsMessagesForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        DataTable TableUsers;
        ConnectClient ConnectClient;
        Form TopForm = null;

        public MenuClientsMessagesForm(ref Form tTopForm)
        {
            TableUsers = new DataTable();

            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            ClientGrid.DataSource = ConnectClient.ClientBindingSource;
            ClientGrid.Columns["ClientID"].Visible = false;
            ClientGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ClientGrid.Columns["ClientName"].Width = 300;
            ClientGrid.Columns["ClientName"].HeaderText = "Клиенты";

            //***************************************************************************************************

            DataGridViewComboBoxColumn UsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "Blogger",
                DataSource = TableUsers,
                DisplayMember = "Name",
                ValueMember = "UserID",
                DataPropertyName = "UserID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            MessagersGrid.Columns.Add(UsersColumn);
            MessagersGrid.Columns["Blogger"].HeaderText = "Блоггер";
            MessagersGrid.Columns["Blogger"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //     MessagersGrid.Columns["Blogger"].Width = 400;

            MessagersGrid.Columns["ChatPermissionID"].Visible = false;
            MessagersGrid.Columns["ClientID"].Visible = false;
            MessagersGrid.Columns["UserID"].Visible = false;

            while (!SplashForm.bCreated) ;
        }

        private void MessagesForm_Shown(object sender, EventArgs e)
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
            ConnectClient = new ConnectClient(ref ClientGrid, ref MessagersGrid);
            TableUsers = ConnectClient.TableUsers();

            FIOComboBox.DataSource = TableUsers;
            FIOComboBox.DisplayMember = "Name";
            FIOComboBox.ValueMember = "UserID";
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

        private void AddButton_Click(object sender, EventArgs e)
        {
            ConnectClient.AddChat(ClientGrid.SelectedRows[0].Cells["ClientID"].Value.ToString(), FIOComboBox.SelectedValue.ToString());
            ConnectClient.UpdateMessagersGrid(ClientGrid.SelectedRows[0].Cells["ClientID"].Value.ToString());
        }

        private void ClientGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ConnectClient != null && ClientGrid.SelectedRows.Count == 1)
            {
                ConnectClient.UpdateMessagersGrid(ClientGrid.SelectedRows[0].Cells["ClientID"].Value.ToString());
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ConnectClient != null && MessagersGrid.SelectedRows.Count == 1)
            {
                ConnectClient.DeleteBlogger();
                ConnectClient.UpdateMessagersGrid(ClientGrid.SelectedRows[0].Cells["ClientID"].Value.ToString());
            }
        }
    }
}
