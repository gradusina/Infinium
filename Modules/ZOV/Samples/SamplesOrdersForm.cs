using Infinium.Modules.ZOV.Samples;

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SamplesOrdersForm : Form
    {
        const int iAdmin = 79;
        const int iMarketing = 78;
        const int iDirector = 75;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedRefresh = false;

        int CurrentRowIndex = -1;
        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        DataTable RolePermissionsDataTable;

        public OrdersManager OrdersManager;
        public DecorCatalogOrder DecorCatalogOrder;

        //RoleTypes RoleType = RoleTypes.Ordinary;
        public enum RoleTypes
        {
            Ordinary = 0,
            Admin = 1,
            Marketing = 2,
            Director = 3
        }

        public SamplesOrdersForm()
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            RolePermissionsDataTable = OrdersManager.GetPermissions(Security.CurrentUserID, this.Name);

            //if (!PermissionGranted(iMarketing) && !PermissionGranted(iDirector) && !PermissionGranted(iAdmin))
            //{
            //    tableLayoutPanel1.Height = this.Height - NavigatePanel.Height - 10;
            //}
            //if (PermissionGranted(iMarketing))
            //{
            //    RoleType = RoleTypes.Marketing;
            //}
            //if (PermissionGranted(iAdmin))
            //{
            //    RoleType = RoleTypes.Admin;
            //}
            //if (PermissionGranted(iDirector))
            //{
            //    RoleType = RoleTypes.Director;
            //}
        }

        public SamplesOrdersForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            RolePermissionsDataTable = OrdersManager.GetPermissions(Security.CurrentUserID, this.Name);

            //if (!PermissionGranted(iMarketing) && !PermissionGranted(iDirector) && !PermissionGranted(iAdmin))
            //{
            //    tableLayoutPanel1.Height = this.Height - NavigatePanel.Height - 10;
            //}
            //if (PermissionGranted(iMarketing))
            //{
            //    RoleType = RoleTypes.Marketing;
            //}
            //if (PermissionGranted(iAdmin))
            //{
            //    RoleType = RoleTypes.Admin;
            //}
            //if (PermissionGranted(iDirector))
            //{
            //    RoleType = RoleTypes.Director;
            //}

            while (!SplashForm.bCreated)
            {
            }
        }

        private bool PermissionGranted(int RoleID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("RoleID = " + RoleID);

            return Rows.Count() > 0;
        }

        private void SamplesOrdersForm_Shown(object sender, EventArgs e)
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
                        this.Close();
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
                        this.Close();
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

        private void Initialize()
        {
            DecorCatalogOrder = new DecorCatalogOrder();

            OrdersManager = new OrdersManager(ref dgvMFrontsOrders, ref dgvZFrontsOrders, ref MDecorTabControl, ref ZDecorTabControl, ref DecorCatalogOrder);

            dgvMClients.DataSource = OrdersManager.MClientsBindingSource;
            dgvMClients.Columns["ClientGroupID"].Visible = false;
            dgvMClients.Columns["ClientID"].Visible = false;
            dgvMClients.Columns["Check"].DisplayIndex = 0;
            dgvMClients.Columns["ClientName"].DisplayIndex = 1;
            dgvMClients.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMClients.Columns["Check"].Width = 50;
            dgvMClients.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMClients.Columns["ClientName"].Visible = true;

            dgvMClientsGroups.DataSource = OrdersManager.MClientsGroupsBindingSource;
            dgvMClientsGroups.Columns["ClientGroupID"].Visible = false;
            dgvMClientsGroups.Columns["Check"].DisplayIndex = 0;
            dgvMClientsGroups.Columns["ClientGroupName"].DisplayIndex = 1;
            dgvMClientsGroups.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMClientsGroups.Columns["Check"].Width = 50;
            dgvMClientsGroups.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMClientsGroups.Columns["ClientGroupName"].Visible = true;

            dgvZClients.DataSource = OrdersManager.ZClientsBindingSource;
            dgvZClients.Columns["ClientGroupID"].Visible = false;
            dgvZClients.Columns["ClientID"].Visible = false;
            dgvZClients.Columns["Check"].DisplayIndex = 0;
            dgvZClients.Columns["ClientName"].DisplayIndex = 1;
            dgvZClients.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvZClients.Columns["Check"].Width = 50;
            dgvZClients.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvZClients.Columns["ClientName"].Visible = true;

            dgvZClientsGroups.DataSource = OrdersManager.ZClientsGroupsBindingSource;
            dgvZClientsGroups.Columns["ClientGroupID"].Visible = false;
            dgvZClientsGroups.Columns["Check"].DisplayIndex = 0;
            dgvZClientsGroups.Columns["ClientGroupName"].DisplayIndex = 1;
            dgvZClientsGroups.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvZClientsGroups.Columns["Check"].Width = 50;
            dgvZClientsGroups.Columns["ClientGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvZClientsGroups.Columns["ClientGroupName"].Visible = true;

            dgvMainOrders.DataSource = OrdersManager.MainOrdersBindingSource;
            MainGridSetting();
        }

        private void MainGridSetting()
        {
            foreach (DataGridViewColumn Column in dgvMainOrders.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMainOrders.Columns.Contains("FirmType"))
                dgvMainOrders.Columns["FirmType"].Visible = false;
            if (dgvMainOrders.Columns.Contains("ClientID"))
                dgvMainOrders.Columns["ClientID"].Visible = false;
            if (dgvMainOrders.Columns.Contains("MainOrderID"))
                dgvMainOrders.Columns["MainOrderID"].Visible = false;

            dgvMainOrders.Columns["ClientName"].HeaderText = "Клиент";
            dgvMainOrders.Columns["OrderNumber"].HeaderText = "№ заказа";
            dgvMainOrders.Columns["MainOrderID"].HeaderText = "№ п\\п";
            dgvMainOrders.Columns["CreateDate"].HeaderText = "Дата\r\nсоздания";
            dgvMainOrders.Columns["DispDate"].HeaderText = "Дата\r\nотгрузки";
            dgvMainOrders.Columns["Description"].HeaderText = "Описание\r\nпродукции";
            dgvMainOrders.Columns["Cost"].HeaderText = "Стоимость, евро";
            dgvMainOrders.Columns["Square"].HeaderText = "Квадратура, м.кв.";
            dgvMainOrders.Columns["ShopAddresses"].HeaderText = "Адреса салонов";
            dgvMainOrders.Columns["Foto"].HeaderText = "Фото";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 3
            };
            dgvMainOrders.Columns["Cost"].DefaultCellStyle.Format = "C";
            dgvMainOrders.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            dgvMainOrders.Columns["Square"].DefaultCellStyle.Format = "C";
            dgvMainOrders.Columns["Square"].DefaultCellStyle.FormatProvider = nfi2;

            dgvMainOrders.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMainOrders.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["Cost"].Width = 130;
            dgvMainOrders.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMainOrders.Columns["OrderNumber"].MinimumWidth = 125;
            dgvMainOrders.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["MainOrderID"].Width = 80;
            dgvMainOrders.Columns["CreateDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["CreateDate"].Width = 115;
            dgvMainOrders.Columns["DispDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["DispDate"].Width = 115;
            dgvMainOrders.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMainOrders.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["Square"].Width = 130;
            dgvMainOrders.Columns["ShopAddresses"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMainOrders.Columns["Foto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainOrders.Columns["Foto"].Width = 50;

            dgvMainOrders.AutoGenerateColumns = false;
            dgvMainOrders.Columns["Description"].ReadOnly = false;
            dgvMainOrders.Columns["ShopAddresses"].DefaultCellStyle.ForeColor = Color.Blue;
            dgvMainOrders.Columns["ShopAddresses"].DefaultCellStyle.Font = new System.Drawing.Font("SEGOE UI", 13.0F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
        }

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OrdersManager != null)
            {
                pcbxFoto.Image = null;
                bool FrontsVisible = false;
                bool DecorVisible = false;
                if (OrdersManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        int MainOrderID = Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["MainOrderID"]);
                        if (Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["FirmType"]) == 1)
                        {
                            dgvMFrontsOrders.Visible = true;
                            dgvZFrontsOrders.Visible = false;
                            MDecorTabControl.Visible = true;
                            ZDecorTabControl.Visible = false;
                            OrdersManager.FilterProductByMainOrder(false, cbMDecor.Checked, cbZDecor.Checked, MainOrderID, ref FrontsVisible, ref DecorVisible);
                        }
                        if (Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["FirmType"]) == 0)
                        {
                            dgvMFrontsOrders.Visible = false;
                            dgvZFrontsOrders.Visible = true;
                            MDecorTabControl.Visible = false;
                            ZDecorTabControl.Visible = true;
                            OrdersManager.FilterProductByMainOrder(true, cbMDecor.Checked, cbZDecor.Checked, MainOrderID, ref FrontsVisible, ref DecorVisible);
                        }

                        if (MainOrderID > 0)
                        {
                            if (MainOrdersTabControl.TabPages[0].PageVisible != FrontsVisible)
                                MainOrdersTabControl.TabPages[0].PageVisible = FrontsVisible;
                            if (MainOrdersTabControl.TabPages[1].PageVisible != DecorVisible)
                                MainOrdersTabControl.TabPages[1].PageVisible = DecorVisible;
                        }

                        if (MainOrdersTabControl.TabPages[0].PageVisible && MainOrdersTabControl.TabPages[1].PageVisible)
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];
                        MainOrdersTabControl.TabPages[2].PageVisible = Convert.ToBoolean(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["Foto"]);

                        if (Convert.ToBoolean(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["Foto"]))
                        {
                            if (Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["FirmType"]) == 1)
                                pcbxFoto.Image = OrdersManager.GetMFoto(MainOrderID);
                            if (Convert.ToInt32(((DataRowView)OrdersManager.MainOrdersBindingSource.Current)["FirmType"]) == 0)
                                pcbxFoto.Image = OrdersManager.GetZFoto(MainOrderID);

                            //if (pcbxFoto.Image.Height > pcbxFoto.Height | pcbxFoto.Image.Width > pcbxFoto.Width)
                            //    pcbxFoto.SizeMode = PictureBoxSizeMode.Zoom;
                            //else
                            //    pcbxFoto.SizeMode = PictureBoxSizeMode.CenterImage;
                            //if (pcbxFoto.Image == null)
                            //    pcbxFoto.Cursor = Cursors.Default;
                            //else
                            //    pcbxFoto.Cursor = Cursors.Hand;
                        }
                    }
                }
                else
                {
                    OrdersManager.FilterProductByMainOrder(true, false, false, -1, ref FrontsVisible, ref DecorVisible);
                    MainOrdersTabControl.TabPages[2].PageVisible = false;
                    OrdersManager.FilterProductByMainOrder(false, false, false, -1, ref FrontsVisible, ref DecorVisible);
                }
            }
        }

        private void MainOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                dgvMainOrders.Refresh();
                NeedRefresh = false;
            }
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

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void CurrencyTodayButton_Click(object sender, EventArgs e)
        {
            bool bMarketing = cbMarketing.Checked;
            bool bMClients = cbMClients.Checked;
            bool bMCreateDate = cbMCreateDate.Checked;
            object MCreateDateFrom = dtpMCreateDateFrom.Value;
            object MCreateDateTo = dtpMCreateDateTo.Value;
            bool bMDispDate = cbMDispDate.Checked;
            object MDispDateFrom = dtpMDispDateFrom.Value;
            object MDispDateTo = dtpMDispDateTo.Value;

            bool bZOV = cbZOV.Checked;
            bool bZClients = cbZClients.Checked;
            bool bZCreateDate = cbZCreateDate.Checked;
            object ZCreateDateFrom = dtpZCreateDateFrom.Value;
            object ZCreateDateTo = dtpZCreateDateTo.Value;
            bool bZDispDate = cbZDispDate.Checked;
            object ZDispDateFrom = dtpZDispDateFrom.Value;
            object ZDispDateTo = dtpZDispDateTo.Value;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            OrdersManager.FilterOrders(
                bMarketing, bMClients, bMCreateDate, MCreateDateFrom, MCreateDateTo, bMDispDate, MDispDateFrom, MDispDateTo,
                bZOV, bZClients, bZCreateDate, ZCreateDateFrom, ZCreateDateTo, bZDispDate, ZDispDateFrom, ZDispDateTo);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MainOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void cbMarketing_CheckedChanged(object sender, EventArgs e)
        {
            panel3.Enabled = cbMarketing.Checked;
        }

        private void cbZOV_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = cbZOV.Checked;
        }

        private void cbMClients_CheckedChanged(object sender, EventArgs e)
        {
            dgvMClientsGroups.Enabled = cbMClients.Checked;
            dgvMClients.Enabled = cbMClients.Checked;
            btnCheckAllMClients.Enabled = cbMClients.Checked;
            btnUncheckAllMClients.Enabled = cbMClients.Checked;
        }

        private void cbZClients_CheckedChanged(object sender, EventArgs e)
        {
            dgvZClientsGroups.Enabled = cbZClients.Checked;
            dgvZClients.Enabled = cbZClients.Checked;
            btnCheckAllZClients.Enabled = cbZClients.Checked;
            btnUncheckAllZClients.Enabled = cbZClients.Checked;
            btnCheckAllZClientsGroups.Enabled = cbZClients.Checked;
            btnUncheckAllZClientsGroups.Enabled = cbZClients.Checked;
        }

        private void cbMCreateDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpMCreateDateFrom.Enabled = cbMCreateDate.Checked;
            dtpMCreateDateTo.Enabled = cbMCreateDate.Checked;
        }

        private void cbMDispDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpMDispDateFrom.Enabled = cbMDispDate.Checked;
            dtpMDispDateTo.Enabled = cbMDispDate.Checked;
        }

        private void cbZCreateDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpZCreateDateFrom.Enabled = cbZCreateDate.Checked;
            dtpZCreateDateTo.Enabled = cbZCreateDate.Checked;
        }

        private void cbZDispDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpZDispDateFrom.Enabled = cbZDispDate.Checked;
            dtpZDispDateTo.Enabled = cbZDispDate.Checked;
        }

        private void btnCheckAllMClients_Click(object sender, EventArgs e)
        {
            int ClientGroupID = 0;
            if (dgvMClientsGroups.SelectedRows.Count > 0)
                ClientGroupID = Convert.ToInt32(dgvMClientsGroups.SelectedRows[0].Cells["ClientGroupID"].Value);
            OrdersManager.CheckMClients(true, ClientGroupID);
            OrdersManager.CheckMClientGroups(true, ClientGroupID);
        }

        private void btnUncheckAllMClients_Click(object sender, EventArgs e)
        {
            int ClientGroupID = 0;
            if (dgvMClientsGroups.SelectedRows.Count > 0)
                ClientGroupID = Convert.ToInt32(dgvMClientsGroups.SelectedRows[0].Cells["ClientGroupID"].Value);
            OrdersManager.CheckMClients(false, ClientGroupID);
            OrdersManager.CheckMClientGroups(false, ClientGroupID);
        }

        private void btnCheckAllZClientsGroups_Click(object sender, EventArgs e)
        {
            OrdersManager.CheckZClientGroups(true);
            OrdersManager.CheckZClients(true);
        }

        private void btnUncheckAllZClientsGroups_Click(object sender, EventArgs e)
        {
            OrdersManager.CheckZClientGroups(false);
            OrdersManager.CheckZClients(false);
        }

        private void btnCheckAllZClients_Click(object sender, EventArgs e)
        {
            int ClientGroupID = 0;
            if (dgvZClientsGroups.SelectedRows.Count > 0)
                ClientGroupID = Convert.ToInt32(dgvZClientsGroups.SelectedRows[0].Cells["ClientGroupID"].Value);
            OrdersManager.CheckZClients(true, ClientGroupID);
            OrdersManager.CheckZClientGroups(true, ClientGroupID);
        }

        private void btnUncheckAllZClients_Click(object sender, EventArgs e)
        {
            int ClientGroupID = 0;
            if (dgvZClientsGroups.SelectedRows.Count > 0)
                ClientGroupID = Convert.ToInt32(dgvZClientsGroups.SelectedRows[0].Cells["ClientGroupID"].Value);
            OrdersManager.CheckZClients(false, ClientGroupID);
            OrdersManager.CheckZClientGroups(false, ClientGroupID);
        }

        private void dgvMClientsGroups_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMClientsGroups.Columns[e.ColumnIndex].Name == "Check" && e.RowIndex != -1)
            {
                bool Checked = Convert.ToBoolean(dgvMClientsGroups.Rows[e.RowIndex].Cells["Check"].Value);
                int ClientGroupID = Convert.ToInt32(dgvMClientsGroups.Rows[e.RowIndex].Cells["ClientGroupID"].Value);
                OrdersManager.CheckMClients(Checked, ClientGroupID);
            }
        }

        private void dgvZClientsGroups_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvZClientsGroups.Columns[e.ColumnIndex].Name == "Check" && e.RowIndex != -1)
            {
                bool Checked = Convert.ToBoolean(dgvZClientsGroups.Rows[e.RowIndex].Cells["Check"].Value);
                int ClientGroupID = Convert.ToInt32(dgvZClientsGroups.Rows[e.RowIndex].Cells["ClientGroupID"].Value);
                OrdersManager.CheckZClients(Checked, ClientGroupID);
            }
        }

        private void dgvMClientsGroups_SelectionChanged(object sender, EventArgs e)
        {
            int ClientGroupID = 0;
            if (dgvMClientsGroups.SelectedRows.Count > 0)
                ClientGroupID = Convert.ToInt32(dgvMClientsGroups.SelectedRows[0].Cells["ClientGroupID"].Value);
            OrdersManager.FilterMClients(ClientGroupID);
        }

        private void dgvZClientsGroups_SelectionChanged(object sender, EventArgs e)
        {
            int ClientGroupID = 0;
            if (dgvZClientsGroups.SelectedRows.Count > 0)
                ClientGroupID = Convert.ToInt32(dgvZClientsGroups.SelectedRows[0].Cells["ClientGroupID"].Value);
            OrdersManager.FilterZClients(ClientGroupID);
        }

        private void tbMNotes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tbMNotes.Text.Length > 0)
                OrdersManager.SearchPartMNotes(tbMNotes.Text);
        }

        private void tbZNotes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tbZNotes.Text.Length > 0)
                OrdersManager.SearchPartZNotes(tbZNotes.Text);
        }

        private void tbSearchMDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tbSearchMDoc.Text.Length > 0)
                OrdersManager.SearchPartMDocNumber(tbSearchMDoc.Text);
        }

        private void tbSearchZDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tbSearchZDoc.Text.Length > 0)
                OrdersManager.SearchPartZDocNumber(tbSearchZDoc.Text);
        }

        private void btnSaveDescription_Click(object sender, EventArgs e)
        {
            int MainOrderID = -1;
            if (dgvMainOrders.SelectedRows.Count > 0 && dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
            bool bMarketing = cbMarketing.Checked;
            bool bMClients = cbMClients.Checked;
            bool bMCreateDate = cbMCreateDate.Checked;
            object MCreateDateFrom = dtpMCreateDateFrom.Value;
            object MCreateDateTo = dtpMCreateDateTo.Value;
            bool bMDispDate = false;
            object MDispDateFrom = dtpMDispDateFrom.Value;
            object MDispDateTo = dtpMDispDateTo.Value;

            bool bZOV = cbZOV.Checked;
            bool bZClients = cbZClients.Checked;
            bool bZCreateDate = cbZCreateDate.Checked;
            object ZCreateDateFrom = dtpZCreateDateFrom.Value;
            object ZCreateDateTo = dtpZCreateDateTo.Value;
            bool bZDispDate = cbZDispDate.Checked;
            object ZDispDateFrom = dtpZDispDateFrom.Value;
            object ZDispDateTo = dtpZDispDateTo.Value;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            OrdersManager.SaveMDescription();
            OrdersManager.SaveZDescription();
            OrdersManager.MoveToMainOrder(MainOrderID);
            OrdersManager.FilterOrders(
                bMarketing, bMClients, bMCreateDate, MCreateDateFrom, MCreateDateTo, bMDispDate, MDispDateFrom, MDispDateTo,
                bZOV, bZClients, bZCreateDate, ZCreateDateFrom, ZCreateDateTo, bZDispDate, ZDispDateFrom, ZDispDateTo);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void openFileDialog4_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            string sFileName = Path.GetFileNameWithoutExtension(openFileDialog4.FileName);
            string sExtension = fileInfo.Extension;
            string sPath = openFileDialog4.FileName;


            bool bMarketing = cbMarketing.Checked;
            bool bMClients = cbMClients.Checked;
            bool bMCreateDate = cbMCreateDate.Checked;
            object MCreateDateFrom = dtpMCreateDateFrom.Value;
            object MCreateDateTo = dtpMCreateDateTo.Value;
            bool bMDispDate = false;
            object MDispDateFrom = dtpMDispDateFrom.Value;
            object MDispDateTo = dtpMDispDateTo.Value;

            bool bZOV = cbZOV.Checked;
            bool bZClients = cbZClients.Checked;
            bool bZCreateDate = cbZCreateDate.Checked;
            object ZCreateDateFrom = dtpZCreateDateFrom.Value;
            object ZCreateDateTo = dtpZCreateDateTo.Value;
            bool bZDispDate = cbZDispDate.Checked;
            object ZDispDateFrom = dtpZDispDateFrom.Value;
            object ZDispDateTo = dtpZDispDateTo.Value;
            int MainOrderID = -1;
            if (dgvMainOrders.SelectedRows.Count > 0 && dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            if (Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FirmType"].Value) == 1)
                OrdersManager.AttachMFoto(sExtension, sFileName, sPath, MainOrderID);
            if (Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FirmType"].Value) == 0)
                OrdersManager.AttachZFoto(sExtension, sFileName, sPath, MainOrderID);
            OrdersManager.FilterOrders(
                bMarketing, bMClients, bMCreateDate, MCreateDateFrom, MCreateDateTo, bMDispDate, MDispDateFrom, MDispDateTo,
                bZOV, bZClients, bZCreateDate, ZCreateDateFrom, ZCreateDateTo, bZDispDate, ZDispDateFrom, ZDispDateTo);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnAttachFoto_Click(object sender, EventArgs e)
        {
            if (dgvMainOrders.SelectedRows.Count == 0)
                return;

            openFileDialog4.ShowDialog();
        }

        private void btnDetachFoto_Click(object sender, EventArgs e)
        {
            bool bMarketing = cbMarketing.Checked;
            bool bMClients = cbMClients.Checked;
            bool bMCreateDate = cbMCreateDate.Checked;
            object MCreateDateFrom = dtpMCreateDateFrom.Value;
            object MCreateDateTo = dtpMCreateDateTo.Value;
            bool bMDispDate = false;
            object MDispDateFrom = dtpMDispDateFrom.Value;
            object MDispDateTo = dtpMDispDateTo.Value;

            bool bZOV = cbZOV.Checked;
            bool bZClients = cbZClients.Checked;
            bool bZCreateDate = cbZCreateDate.Checked;
            object ZCreateDateFrom = dtpZCreateDateFrom.Value;
            object ZCreateDateTo = dtpZCreateDateTo.Value;
            bool bZDispDate = cbZDispDate.Checked;
            object ZDispDateFrom = dtpZDispDateFrom.Value;
            object ZDispDateTo = dtpZDispDateTo.Value;
            int MainOrderID = -1;
            if (dgvMainOrders.SelectedRows.Count > 0 && dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            if (Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FirmType"].Value) == 1)
                OrdersManager.DetachMFoto(MainOrderID);
            if (Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FirmType"].Value) == 0)
                OrdersManager.DetachZFoto(MainOrderID);
            OrdersManager.FilterOrders(
                bMarketing, bMClients, bMCreateDate, MCreateDateFrom, MCreateDateTo, bMDispDate, MDispDateFrom, MDispDateTo,
                bZOV, bZClients, bZCreateDate, ZCreateDateFrom, ZCreateDateTo, bZDispDate, ZDispDateFrom, ZDispDateTo);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void pcbxFoto_Click(object sender, EventArgs e)
        {
            if (pcbxFoto.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxFoto.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void dgvMainOrders_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid dataGridView = (PercentageDataGrid)sender;
            if (e.ColumnIndex > -1 && e.RowIndex > -1 && dataGridView.Columns[e.ColumnIndex].Name == "ShopAddresses" && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void dgvMainOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dgvMainOrders.Columns[e.ColumnIndex].Name == "ShopAddresses"
                                && dgvMainOrders.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null
                                && dgvMainOrders.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value
                                && dgvMainOrders.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
            {
                int FirmType = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["FirmType"].Value);
                if (FirmType == 1)
                {
                    PhantomForm PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();
                    MarketShopAddressesForm ShopAddressesForm = new MarketShopAddressesForm(this, OrdersManager,
                        FirmType, Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["ClientID"].Value));
                    TopForm = ShopAddressesForm;
                    ShopAddressesForm.ShowDialog();
                    PhantomForm.Close();
                    PhantomForm.Dispose();
                    ShopAddressesForm.Dispose();
                    TopForm = null;
                }
                if (FirmType == 0)
                {
                    int ClientID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["ClientID"].Value);
                    int MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);

                    PhantomForm PhantomForm = new Infinium.PhantomForm();
                    PhantomForm.Show();
                    ZOVShopAddressesForm ShopAddressesForm = new ZOVShopAddressesForm(this, ClientID, MainOrderID);
                    TopForm = ShopAddressesForm;
                    ShopAddressesForm.ShowDialog();
                    PhantomForm.Close();
                    PhantomForm.Dispose();
                    ShopAddressesForm.Dispose();
                    TopForm = null;
                }
            }
        }

        private void ReportCreate()
        {
            bool bZOV = cbZOV.Checked;
            bool bZClients = cbZClients.Checked;
            bool bZCreateDate = cbZCreateDate.Checked;
            object ZCreateDateFrom = dtpZCreateDateFrom.Value;
            object ZCreateDateTo = dtpZCreateDateTo.Value;
            bool bZDispDate = cbZDispDate.Checked;
            object ZDispDateFrom = dtpZDispDateFrom.Value;
            object ZDispDateTo = dtpZDispDateTo.Value;
            string FileName = "Образцы";
            samplesReport samplesReport = new samplesReport();
            samplesReport.GetSamples(OrdersManager.GetData());
            samplesReport.GetSamplesFronts(OrdersManager.GetFrontsDataTable(bZOV, bZClients, bZCreateDate, ZCreateDateFrom, ZCreateDateTo, bZDispDate, ZDispDateFrom, ZDispDateTo));
            samplesReport.Report(FileName);
        }

        private void btnExcelExport_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            ReportCreate();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
