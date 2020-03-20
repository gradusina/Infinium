using Infinium.Modules.Marketing.Clients;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientsManagersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        ClientsManagers ClientsManagers;

        public ClientsManagersForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            while (!SplashForm.bCreated) ;
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

        private void ClientsManagersForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ClientsManagersForm_Load(object sender, EventArgs e)
        {
            ClientsManagers = new ClientsManagers();
            dgvManagers.DataSource = ClientsManagers.ManagersBS;
            dgvUsers.DataSource = ClientsManagers.UsersBS;
            dgvGridsSettings();
        }

        private void dgvGridsSettings()
        {
            dgvManagers.Columns["ManagerID"].Visible = false;
            dgvManagers.Columns["Name"].Visible = false;
            dgvManagers.Columns["Password"].Visible = false;
            dgvManagers.Columns["UserID"].Visible = false;

            dgvManagers.Columns["ShortName"].HeaderText = "ФИО";
            dgvManagers.Columns["Phone"].HeaderText = "Телефон";
            dgvManagers.Columns["Email"].HeaderText = "E-mail";
            dgvManagers.Columns["Skype"].HeaderText = "Skype";

            dgvManagers.Columns["ShortName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvManagers.Columns["ShortName"].MinimumWidth = 150;
            dgvManagers.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvManagers.Columns["Phone"].MinimumWidth = 70;
            dgvManagers.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvManagers.Columns["Email"].MinimumWidth = 70;
            dgvManagers.Columns["Skype"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvManagers.Columns["Skype"].MinimumWidth = 70;

            dgvUsers.Columns["UserID"].Visible = false;
            dgvUsers.Columns["Name"].Visible = false;
            dgvUsers.Columns["Password"].Visible = false;
        }

        private void dgvDiscountVolume_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            int ManagerID = 0;
            if (dgvManagers.SelectedRows.Count != 0 && dgvManagers.SelectedRows[0].Cells["ManagerID"].Value != DBNull.Value)
                ManagerID = Convert.ToInt32(dgvManagers.SelectedRows[0].Cells["ManagerID"].Value);
            if (ManagerID != 0 && ClientsManagers.IsUserClientManager(ManagerID))
            {
                LightMessageBox.Show(ref TopForm, false, "Удаление запрещено, так как у менеджера есть клиенты. Для удаления открепите менеджера от всех клиентов", "Отмена");
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Удалить позицию?",
                    "Удаление");

            if (OKCancel)
            {
                ClientsManagers.RemoveManager();
            }
        }

        private void btnSaveManagers_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                ClientsManagers.SaveManagers();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
            }
        }

        private void btnAddManager_Click(object sender, EventArgs e)
        {
            string Name = string.Empty;
            string ShortName = string.Empty;
            string Password = string.Empty;
            string Phone = string.Empty;
            string Email = string.Empty;
            string Skype = string.Empty;

            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                Name = dgvUsers.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["ShortName"].Value != DBNull.Value)
                ShortName = dgvUsers.SelectedRows[0].Cells["ShortName"].Value.ToString();
            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["Password"].Value != DBNull.Value)
                Password = dgvUsers.SelectedRows[0].Cells["Password"].Value.ToString();
            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["Phone"].Value != DBNull.Value)
                Phone = dgvUsers.SelectedRows[0].Cells["Phone"].Value.ToString();
            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["Email"].Value != DBNull.Value)
                Email = dgvUsers.SelectedRows[0].Cells["Email"].Value.ToString();
            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["Skype"].Value != DBNull.Value)
                Skype = dgvUsers.SelectedRows[0].Cells["Skype"].Value.ToString();

            int UserID = 0;
            if (dgvUsers.SelectedRows.Count != 0 && dgvUsers.SelectedRows[0].Cells["UserID"].Value != DBNull.Value)
                UserID = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);

            ClientsManagers.AddManager(UserID, Name, ShortName, Password, Phone, Email, Skype);
        }
    }
}
