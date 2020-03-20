using Infinium.Modules.ZOV.Complaints;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVComplaintsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        ComplaintsManager ComplaintsManager;

        public ZOVComplaintsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            while (!SplashForm.bCreated) ;
        }

        private void ZOVComplaintsForm_Shown(object sender, EventArgs e)
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

        private void Initialize()
        {
            ComplaintsManager = new ComplaintsManager();

            dgvComplaints.DataSource = ComplaintsManager.ComplaintsBS;
            dgvComplaintsSettings();
            dgvFrontsOrders.DataSource = ComplaintsManager.FrontsOrdersBS;
            dgvFrontsOrdersSettings();
            dgvDecorOrders.DataSource = ComplaintsManager.DecorOrdersBS;
            dgvDecorOrderSettings();
        }

        private void dgvComplaintsSettings()
        {
            dgvComplaints.Columns.Add(ComplaintsManager.ClientsColumn);
            dgvComplaints.Columns.Add(ComplaintsManager.ComplaintTypesColumn);
            foreach (DataGridViewColumn Column in dgvComplaints.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvComplaints.Columns.Contains("ComplaintID"))
                dgvComplaints.Columns["ComplaintID"].Visible = false;
            if (dgvComplaints.Columns.Contains("ClientID"))
                dgvComplaints.Columns["ClientID"].Visible = false;
            if (dgvComplaints.Columns.Contains("ComplaintTypeID"))
                dgvComplaints.Columns["ComplaintTypeID"].Visible = false;
            if (dgvComplaints.Columns.Contains("ReportStatusID"))
                dgvComplaints.Columns["ReportStatusID"].Visible = false;
            if (dgvComplaints.Columns.Contains("CreateDateTime"))
                dgvComplaints.Columns["CreateDateTime"].Visible = false;
            if (dgvComplaints.Columns.Contains("CreateUserID"))
                dgvComplaints.Columns["CreateUserID"].Visible = false;

            dgvComplaints.Columns["DispatchDate"].DefaultCellStyle.Format = "dd.MM.yyyy";
            dgvComplaints.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            dgvComplaints.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
            dgvComplaints.Columns["MainOrderID"].HeaderText = "№ п\\п";
            dgvComplaints.Columns["DocNumber"].HeaderText = "№ заказа";
            dgvComplaints.Columns["ReorderDocNumber"].HeaderText = "№ перезаказа";
            dgvComplaints.Columns["WayBill"].HeaderText = "№ ТТН";
            dgvComplaints.Columns["Notes"].HeaderText = "Причина";
            dgvComplaints.Columns["DocNumber"].HeaderText = "№ заказа";
            dgvComplaints.Columns["PresentingPercentage"].HeaderText = "Выставленная\r\nпретензия, %";
            dgvComplaints.Columns["ConfirmPercentage"].HeaderText = "Принятая\r\nпретензия, %";
            dgvComplaints.Columns["ReportType"].HeaderText = "Тип отчета";
            dgvComplaints.Columns["ConfirmComplaint"].HeaderText = "Подтверждена";
            dgvComplaints.Columns["Disable"].HeaderText = "Закрыта";
            dgvComplaints.Columns["CreateDateTime"].HeaderText = "Дата\r\nсоздания";

            dgvComplaints.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvComplaints.Columns["DispatchDate"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["ClientsColumn"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["ComplaintTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["ReportType"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["PresentingPercentage"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["ConfirmPercentage"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["ConfirmComplaint"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["Disable"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["DocNumber"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["ReorderDocNumber"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["WayBill"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
            dgvComplaints.Columns["Notes"].DisplayIndex = DisplayIndex++;

            dgvComplaints.Columns["ReportType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvComplaints.Columns["ReportType"].MinimumWidth = 125;
            dgvComplaints.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvComplaints.Columns["DispatchDate"].MinimumWidth = 125;
            dgvComplaints.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvComplaints.Columns["DocNumber"].MinimumWidth = 125;
            dgvComplaints.Columns["ReorderDocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvComplaints.Columns["ReorderDocNumber"].MinimumWidth = 125;
            dgvComplaints.Columns["WayBill"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["WayBill"].Width = 125;
            dgvComplaints.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["CreateDateTime"].Width = 125;
            dgvComplaints.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvComplaints.Columns["Notes"].MinimumWidth = 120;
            dgvComplaints.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["MainOrderID"].Width = 75;
            dgvComplaints.Columns["Disable"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["Disable"].Width = 125;
            dgvComplaints.Columns["ConfirmComplaint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["ConfirmComplaint"].Width = 125;
            dgvComplaints.Columns["PresentingPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["PresentingPercentage"].Width = 125;
            dgvComplaints.Columns["ConfirmPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvComplaints.Columns["ConfirmPercentage"].Width = 125;
        }

        private void dgvFrontsOrdersSettings()
        {
            dgvFrontsOrders.AutoGenerateColumns = false;

            //добавление столбцов
            dgvFrontsOrders.Columns.Add(ComplaintsManager.FrontsColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.FrameColorsColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.TechnoProfilesColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.PatinaColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.InsetTypesColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.InsetColorsColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.TechnoFrameColorsColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.TechnoInsetTypesColumn);
            dgvFrontsOrders.Columns.Add(ComplaintsManager.TechnoInsetColorsColumn);

            //убирание лишних столбцов
            if (dgvFrontsOrders.Columns.Contains("CreateDateTime"))
            {
                dgvFrontsOrders.Columns["CreateDateTime"].HeaderText = "Добавлено";
                dgvFrontsOrders.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgvFrontsOrders.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvFrontsOrders.Columns["CreateDateTime"].Width = 100;
            }
            if (dgvFrontsOrders.Columns.Contains("CreateUserID"))
                dgvFrontsOrders.Columns["CreateUserID"].Visible = false;
            dgvFrontsOrders.Columns["FrontsOrdersID"].Visible = false;
            dgvFrontsOrders.Columns["MainOrderID"].Visible = false;
            dgvFrontsOrders.Columns["FrontID"].Visible = false;
            dgvFrontsOrders.Columns["ColorID"].Visible = false;
            dgvFrontsOrders.Columns["PatinaID"].Visible = false;
            dgvFrontsOrders.Columns["InsetTypeID"].Visible = false;
            dgvFrontsOrders.Columns["InsetColorID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoProfileID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoColorID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoInsetTypeID"].Visible = false;
            dgvFrontsOrders.Columns["TechnoInsetColorID"].Visible = false;
            dgvFrontsOrders.Columns["FactoryID"].Visible = false;

            if (dgvFrontsOrders.Columns.Contains("AlHandsSize"))
                dgvFrontsOrders.Columns["AlHandsSize"].Visible = false;
            if (dgvFrontsOrders.Columns.Contains("FrontDrillTypeID"))
                dgvFrontsOrders.Columns["FrontDrillTypeID"].Visible = false;
            if (dgvFrontsOrders.Columns.Contains("ImpostMargin"))
                dgvFrontsOrders.Columns["ImpostMargin"].Visible = false;
            dgvFrontsOrders.Columns["FrontPrice"].Visible = false;
            dgvFrontsOrders.Columns["InsetPrice"].Visible = false;
            dgvFrontsOrders.Columns["Cost"].Visible = false;

            dgvFrontsOrders.ScrollBars = ScrollBars.Both;



            dgvFrontsOrders.Columns["Debt"].Visible = false;

            dgvFrontsOrders.Columns["ItemWeight"].Visible = false;
            dgvFrontsOrders.Columns["Weight"].Visible = false;
            dgvFrontsOrders.Columns["FrontConfigID"].Visible = false;

            int DisplayIndex = 0;
            dgvFrontsOrders.Columns["Reason"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            dgvFrontsOrders.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in dgvFrontsOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            dgvFrontsOrders.Columns["CupboardString"].HeaderText = "Шкаф";
            dgvFrontsOrders.Columns["Height"].HeaderText = "Высота";
            dgvFrontsOrders.Columns["Width"].HeaderText = "Ширина";
            dgvFrontsOrders.Columns["Count"].HeaderText = "Кол-во";
            dgvFrontsOrders.Columns["Notes"].HeaderText = "Примечание";
            dgvFrontsOrders.Columns["Square"].HeaderText = "Площадь";
            dgvFrontsOrders.Columns["IsNonStandard"].HeaderText = "Н\\С";
            dgvFrontsOrders.Columns["FrontPrice"].HeaderText = "Цена за\r\n  фасад";
            dgvFrontsOrders.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            dgvFrontsOrders.Columns["Cost"].HeaderText = "Стоимость";
            dgvFrontsOrders.Columns["Reason"].HeaderText = "Причина";

            dgvFrontsOrders.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["CupboardString"].Width = 165;
            dgvFrontsOrders.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Height"].Width = 85;
            dgvFrontsOrders.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Width"].Width = 85;
            dgvFrontsOrders.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Count"].Width = 85;
            dgvFrontsOrders.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Cost"].Width = 120;
            dgvFrontsOrders.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["Square"].Width = 100;
            dgvFrontsOrders.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["FrontPrice"].Width = 85;
            dgvFrontsOrders.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["InsetPrice"].Width = 85;
            dgvFrontsOrders.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvFrontsOrders.Columns["IsNonStandard"].Width = 85;
            dgvFrontsOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["Notes"].MinimumWidth = 75;
            dgvFrontsOrders.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFrontsOrders.Columns["Reason"].MinimumWidth = 75;
            dgvFrontsOrders.CellFormatting += FrontsOrdersDataGrid_CellFormatting;
        }

        void FrontsOrdersDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns.Contains("PatinaColumn") && (e.ColumnIndex == grid.Columns["PatinaColumn"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int PatinaID = -1;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["PatinaID"].Value != DBNull.Value)
                {
                    PatinaID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PatinaID"].Value);
                    DisplayName = ComplaintsManager.PatinaDisplayName(PatinaID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        private void dgvDecorOrderSettings()
        {
            dgvDecorOrders.Columns["Price"].Visible = false;
            dgvDecorOrders.Columns["Cost"].Visible = false;

            dgvDecorOrders.Columns.Add(ComplaintsManager.DecorItemColumn);
            dgvDecorOrders.Columns.Add(ComplaintsManager.DecorColorColumn);
            dgvDecorOrders.Columns.Add(ComplaintsManager.DecorPatinaColumn);

            if (dgvDecorOrders.Columns.Contains("CreateDateTime"))
            {
                dgvDecorOrders.Columns["CreateDateTime"].HeaderText = "Добавлено";
                dgvDecorOrders.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgvDecorOrders.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvDecorOrders.Columns["CreateDateTime"].Width = 100;
            }
            if (dgvDecorOrders.Columns.Contains("CreateUserID"))
                dgvDecorOrders.Columns["CreateUserID"].Visible = false;
            dgvDecorOrders.Columns["DecorOrderID"].Visible = false;
            dgvDecorOrders.Columns["MainOrderID"].Visible = false;
            dgvDecorOrders.Columns["ProductID"].Visible = false;
            dgvDecorOrders.Columns["DecorID"].Visible = false;
            dgvDecorOrders.Columns["ColorID"].Visible = false;
            dgvDecorOrders.Columns["PatinaID"].Visible = false;
            dgvDecorOrders.Columns["Debt"].Visible = false;
            dgvDecorOrders.Columns["DecorConfigID"].Visible = false;
            dgvDecorOrders.Columns["FactoryID"].Visible = false;
            dgvDecorOrders.Columns["ItemWeight"].Visible = false;
            dgvDecorOrders.Columns["Weight"].Visible = false;

            dgvDecorOrders.Columns["Reason"].HeaderText = "Причина";
            dgvDecorOrders.Columns["Price"].HeaderText = "Цена";
            dgvDecorOrders.Columns["Cost"].HeaderText = "Стоимость";

            for (int j = 2; j < dgvDecorOrders.Columns.Count; j++)
            {
                if (dgvDecorOrders.Columns[j].HeaderText == "Height")
                {
                    dgvDecorOrders.Columns[j].HeaderText = "Высота";
                }
                if (dgvDecorOrders.Columns[j].HeaderText == "Length")
                {
                    dgvDecorOrders.Columns[j].HeaderText = "Длина";
                }
                if (dgvDecorOrders.Columns[j].HeaderText == "Width")
                {
                    dgvDecorOrders.Columns[j].HeaderText = "Ширина";
                }
                if (dgvDecorOrders.Columns[j].HeaderText == "Count")
                {
                    dgvDecorOrders.Columns[j].HeaderText = "Кол-во";
                }
                if (dgvDecorOrders.Columns[j].HeaderText == "Notes")
                {
                    dgvDecorOrders.Columns[j].HeaderText = "Примечание";
                }
            }

            foreach (DataGridViewColumn Column in dgvDecorOrders.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgvDecorOrders.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ItemColumn"].MinimumWidth = 75;
            dgvDecorOrders.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["ColorsColumn"].MinimumWidth = 75;
            dgvDecorOrders.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["PatinaColumn"].MinimumWidth = 75;
            dgvDecorOrders.Columns["Height"].MinimumWidth = 90;
            dgvDecorOrders.Columns["Length"].MinimumWidth = 90;
            dgvDecorOrders.Columns["Width"].MinimumWidth = 90;
            dgvDecorOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["Notes"].MinimumWidth = 90;
            dgvDecorOrders.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvDecorOrders.Columns["Reason"].MinimumWidth = 70;
            dgvDecorOrders.Columns["Count"].MinimumWidth = 90;

            int DisplayIndex = 0;
            dgvDecorOrders.Columns["Reason"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Length"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Height"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Width"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Count"].DisplayIndex = DisplayIndex++;
            dgvDecorOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;
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

        private void MinimizeButton_Click(object sender, EventArgs e)
        {

        }

        private void dgvComplaints_SelectionChanged(object sender, EventArgs e)
        {
            if (ComplaintsManager == null)
                return;
            int ComplaintID = 0;
            int MainOrderID = 0;
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value != DBNull.Value)
                ComplaintID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value);
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["MainOrderID"].Value);

            MainOrdersTabControl.TabPages[0].PageVisible = ComplaintsManager.FilterFrontsOrdersByMainOrderID(ComplaintID, MainOrderID);
            MainOrdersTabControl.TabPages[1].PageVisible = ComplaintsManager.FilterDecorOrdersByMainOrderID(ComplaintID, MainOrderID);
        }

        private void ZOVComplaintsForm_Load(object sender, EventArgs e)
        {
            Initialize();
            ComplaintsManager.GetComplaints();
            bool bWithReturn = cbWithReturn.Checked;
            bool bWithoutReturn = cbWithoutReturn.Checked;
            bool bNotFullyDispatch = cbNotFullyDispatch.Checked;
            bool bConfirm = cbNotConfirm.Checked;
            bool bNotConfirm = cbNotConfirm.Checked;

            ComplaintsManager.FilterComplaints(bWithReturn, bWithoutReturn, bNotFullyDispatch, bConfirm, bNotConfirm);
        }

        private void FilterComplaints()
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bool bWithReturn = cbWithReturn.Checked;
            bool bWithoutReturn = cbWithoutReturn.Checked;
            bool bNotFullyDispatch = cbNotFullyDispatch.Checked;
            bool bConfirm = cbConfirm.Checked;
            bool bNotConfirm = cbNotConfirm.Checked;

            ComplaintsManager.FilterComplaints(bWithReturn, bWithoutReturn, bNotFullyDispatch, bConfirm, bNotConfirm);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbNotFullyDispatch_CheckedChanged(object sender, EventArgs e)
        {
            FilterComplaints();
        }

        private void cbWithoutReturn_CheckedChanged(object sender, EventArgs e)
        {
            FilterComplaints();
        }

        private void cbWithReturn_CheckedChanged(object sender, EventArgs e)
        {
            FilterComplaints();
        }

        private void cbConfirm_CheckedChanged(object sender, EventArgs e)
        {
            FilterComplaints();
        }

        private void cbNotConfirm_CheckedChanged(object sender, EventArgs e)
        {
            FilterComplaints();
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvComplaints.SelectedRows.Count == 0)
                return;

            bool bOkCancel = LightMessageBox.Show(ref TopForm, true,
                "Подтвердить?",
                "Подтверждение рекламации");
            if (!bOkCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Подтверждение рекламации.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ComplaintID = 0;
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value != DBNull.Value)
                ComplaintID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value);
            ComplaintsManager.ConfirmComplaint(ComplaintID, true, 0);
            ComplaintsManager.GetComplaints();
            ComplaintsManager.MoveToComplaint(ComplaintID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            if (dgvComplaints.SelectedRows.Count == 0)
                return;

            bool bOkCancel = LightMessageBox.Show(ref TopForm, true,
                "Отменить подтверждение?",
                "Подтверждение рекламации");
            if (!bOkCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Отмена подтверждения рекламации.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ComplaintID = 0;
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value != DBNull.Value)
                ComplaintID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value);
            ComplaintsManager.ConfirmComplaint(ComplaintID, false, 0);
            ComplaintsManager.GetComplaints();
            ComplaintsManager.MoveToComplaint(ComplaintID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvComplaints_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ComplaintsManager.ComplaintsBS.Position = e.RowIndex;
                bool ConfirmComplaint = false;
                if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ConfirmComplaint"].Value != DBNull.Value)
                    ConfirmComplaint = Convert.ToBoolean(dgvComplaints.SelectedRows[0].Cells["ConfirmComplaint"].Value);
                if (ConfirmComplaint)
                {
                    kryptonContextMenuItem1.Visible = false;
                    kryptonContextMenuItem2.Visible = true;
                }
                else
                {
                    kryptonContextMenuItem1.Visible = true;
                    kryptonContextMenuItem2.Visible = false;
                }
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            if (dgvComplaints.SelectedRows.Count == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ReportToDBF DBFReport = new ReportToDBF();
            object DispatchDate = dgvComplaints.SelectedRows[0].Cells["DispatchDate"].Value;
            int ComplaintID = 0;
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value != DBNull.Value)
                ComplaintID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value);
            DBFReport.CreateReport(Convert.ToDateTime(DispatchDate), ComplaintID, 4);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            if (dgvComplaints.SelectedRows.Count == 0)
                return;

            bool bOkCancel = LightMessageBox.Show(ref TopForm, true,
                "Подтвердить?",
                "Подтверждение рекламации");
            if (!bOkCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Подтверждение рекламации.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ComplaintID = 0;
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value != DBNull.Value)
                ComplaintID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value);
            ComplaintsManager.ConfirmComplaint(ComplaintID, true, 50);
            ComplaintsManager.GetComplaints();
            ComplaintsManager.MoveToComplaint(ComplaintID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            if (dgvComplaints.SelectedRows.Count == 0)
                return;

            bool bOkCancel = LightMessageBox.Show(ref TopForm, true,
                "Подтвердить?",
                "Подтверждение рекламации");
            if (!bOkCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Подтверждение рекламации.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int ComplaintID = 0;
            if (dgvComplaints.SelectedRows.Count > 0 && dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value != DBNull.Value)
                ComplaintID = Convert.ToInt32(dgvComplaints.SelectedRows[0].Cells["ComplaintID"].Value);
            ComplaintsManager.ConfirmComplaint(ComplaintID, true, 100);
            ComplaintsManager.GetComplaints();
            ComplaintsManager.MoveToComplaint(ComplaintID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

    }
}
