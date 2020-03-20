using Infinium.Modules.DyeingAssignments;

using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DyeingAssignmentsCreateForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;

        int FormEvent = 0;

        Form MainForm;
        Form TopForm = null;
        DateTime From = DateTime.Now;
        DateTime To = DateTime.Now;

        ControlAssignments ControlAssignmentsManager;
        FrontsOrdersManager FrontsOrdersManager;
        PrintDyeingAssignments PrintDyeingAssignmentsManager;

        public DyeingAssignmentsCreateForm(Form tMainForm, ref ControlAssignments tControlAssignmentsManager, ref PrintDyeingAssignments tPrintDyeingAssignmentsManager, DateTime dFrom, DateTime dTo)
        {
            InitializeComponent();
            MainForm = tMainForm;
            From = dFrom;
            To = dTo;
            ControlAssignmentsManager = tControlAssignmentsManager;
            PrintDyeingAssignmentsManager = tPrintDyeingAssignmentsManager;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            ShowButtons();
            while (!SplashForm.bCreated) ;
        }

        public void ShowButtons()
        {
            int GroupType = 0;
            int GroupType1 = 0;
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["GroupType"].Value);
            if (cbGroupTypes.SelectedItem != null && cbGroupTypes.SelectedItem != DBNull.Value)
                GroupType1 = Convert.ToInt32(((DataRowView)cbGroupTypes.SelectedItem).Row["ID"]);

            btnAddMarketingRetailColumn.Visible = false;
            btnSaveMarketingRetail.Visible = false;
            btnSaveZOV.Visible = false;
            btnAddMarketingWholeColumn.Visible = false;
            btnSaveMarketingWhole.Visible = false;
            btnAddMarketingWholeColumn1.Visible = false;
            btnSaveMarketingWhole1.Visible = false;

            switch (GroupType1)
            {
                case 0:
                    btnSaveZOV.Visible = true;
                    break;
                case 1:
                    btnAddMarketingRetailColumn.Visible = true;
                    btnSaveMarketingRetail.Visible = true;
                    break;
                case 2:
                    btnAddMarketingWholeColumn.Visible = true;
                    btnSaveMarketingWhole.Visible = true;
                    btnAddMarketingWholeColumn1.Visible = true;
                    btnSaveMarketingWhole1.Visible = true;
                    break;
                case 3:
                    btnSaveZOV.Visible = true;
                    btnAddMarketingRetailColumn.Visible = true;
                    btnSaveMarketingRetail.Visible = true;
                    break;
                case 4:
                    btnSaveZOV.Visible = true;
                    break;
                default:
                    break;
            }

            if (GroupType1 == 3 && GroupType == 1)
            {
                btnSaveZOV.Visible = false;
                btnAddMarketingRetailColumn.Visible = true;
                btnSaveMarketingRetail.Visible = true;
            }
            if (GroupType1 == 3 && GroupType == 0)
            {
                btnSaveZOV.Visible = true;
                btnAddMarketingRetailColumn.Visible = false;
                btnSaveMarketingRetail.Visible = false;
            }

            //if (ControlAssignmentsManager.NewAssignment)
            //{
            //    btnAddMarketingRetailColumn.Visible = true;
            //    btnSaveMarketingRetail.Visible = true;
            //    btnSaveZOV.Visible = true;
            //    btnAddMarketingWholeColumn.Visible = true;
            //    btnSaveMarketingWhole.Visible = true;
            //    btnAddMarketingWholeColumn1.Visible = true;
            //    btnSaveMarketingWhole1.Visible = true;
            //}
            //else
            //{
            //    if (ControlAssignmentsManager.GroupType == 0)
            //    {
            //        btnAddMarketingRetailColumn.Visible = false;
            //        btnSaveMarketingRetail.Visible = false;
            //        btnSaveZOV.Visible = true;
            //        btnAddMarketingWholeColumn.Visible = false;
            //        btnSaveMarketingWhole.Visible = false;
            //        btnAddMarketingWholeColumn1.Visible = false;
            //        btnSaveMarketingWhole1.Visible = false;
            //    }
            //    if (ControlAssignmentsManager.GroupType == 1)
            //    {
            //        btnAddMarketingRetailColumn.Visible = true;
            //        btnSaveMarketingRetail.Visible = true;
            //        btnSaveZOV.Visible = false;
            //        btnAddMarketingWholeColumn.Visible = false;
            //        btnSaveMarketingWhole.Visible = false;
            //        btnAddMarketingWholeColumn1.Visible = false;
            //        btnSaveMarketingWhole1.Visible = false;
            //    }
            //    if (ControlAssignmentsManager.GroupType == 2)
            //    {
            //        btnAddMarketingRetailColumn.Visible = false;
            //        btnSaveMarketingRetail.Visible = false;
            //        btnSaveZOV.Visible = false;
            //        btnAddMarketingWholeColumn.Visible = true;
            //        btnSaveMarketingWhole.Visible = true;
            //        btnAddMarketingWholeColumn1.Visible = true;
            //        btnSaveMarketingWhole1.Visible = true;
            //    }
            //}
        }

        private void DyeingAssignmentsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            NeedSplash = true;
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
                        MainForm.Activate();
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
                        MainForm.Activate();
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
            DataTable GroupTypesDT = new DataTable();
            GroupTypesDT.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
            GroupTypesDT.Columns.Add(new DataColumn("GroupType", Type.GetType("System.String")));

            {
                DataRow NewRow = GroupTypesDT.NewRow();
                NewRow["ID"] = 3;
                NewRow["GroupType"] = "Комбинированная";
                GroupTypesDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = GroupTypesDT.NewRow();
                NewRow["ID"] = 0;
                NewRow["GroupType"] = "ЗОВ";
                GroupTypesDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = GroupTypesDT.NewRow();
                NewRow["ID"] = 1;
                NewRow["GroupType"] = "Маркетинг розница";
                GroupTypesDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = GroupTypesDT.NewRow();
                NewRow["ID"] = 2;
                NewRow["GroupType"] = "Маркетинг опт";
                GroupTypesDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = GroupTypesDT.NewRow();
                NewRow["ID"] = 4;
                NewRow["GroupType"] = "ДОЗ";
                GroupTypesDT.Rows.Add(NewRow);
            }

            cbGroupTypes.DataSource = GroupTypesDT.DefaultView;
            cbGroupTypes.ValueMember = "ID";
            cbGroupTypes.DisplayMember = "GroupType";

            if (!ControlAssignmentsManager.NewAssignment)
            {
                cbGroupTypes.Enabled = false;
                cbGroupTypes.SelectedValue = ControlAssignmentsManager.GroupType;
            }
            else
            {
                if (cbGroupTypes.SelectedItem != null && cbGroupTypes.SelectedItem != DBNull.Value)
                    ControlAssignmentsManager.GroupType = Convert.ToInt32(((DataRowView)cbGroupTypes.SelectedItem).Row["ID"]);
            }

            btnAddMarketingRetailColumn.Visible = false;
            btnSaveMarketingRetail.Visible = false;
            btnSaveZOV.Visible = false;
            btnAddMarketingWholeColumn.Visible = false;
            btnSaveMarketingWhole.Visible = false;
            btnAddMarketingWholeColumn1.Visible = false;
            btnSaveMarketingWhole1.Visible = false;

            switch (ControlAssignmentsManager.GroupType)
            {
                case 0:
                    btnSaveZOV.Visible = true;
                    break;
                case 1:
                    btnAddMarketingRetailColumn.Visible = true;
                    btnSaveMarketingRetail.Visible = true;
                    break;
                case 2:
                    btnAddMarketingWholeColumn.Visible = true;
                    btnSaveMarketingWhole.Visible = true;
                    btnAddMarketingWholeColumn1.Visible = true;
                    btnSaveMarketingWhole1.Visible = true;
                    break;
                case 3:
                    btnSaveZOV.Visible = true;
                    btnAddMarketingRetailColumn.Visible = true;
                    btnSaveMarketingRetail.Visible = true;
                    break;
                case 4:
                    btnSaveZOV.Visible = true;
                    break;
                default:
                    break;
            }
            ControlAssignmentsManager.GetSavedMainOrders();

            FrontsOrdersManager = new FrontsOrdersManager();
            FrontsOrdersManager.Initialize();
            FrontsOrdersManager.ClearTables();

            dgvBatchFrontsOrders.DataSource = FrontsOrdersManager.BatchFrontsList;
            dgvDyeingFrontsOrders.DataSource = FrontsOrdersManager.DyeingFrontsList;
            dgvBatchMainOrders.DataSource = ControlAssignmentsManager.BatchMainOrdersList;
            dgvBatches.DataSource = ControlAssignmentsManager.BatchesList;
            dgvMegaBatches.DataSource = ControlAssignmentsManager.MegaBatchesList;

            BatchFrontsOrdersGridSetting(ref dgvBatchFrontsOrders);
            DyeingFrontsOrdersGridSetting(ref dgvDyeingFrontsOrders);
            BatchMainOrdersGridSetting(ref dgvBatchMainOrders);
            BatchesGridSetting(ref dgvBatches);
            MegaBatchesGridSetting(ref dgvMegaBatches);
        }

        private void BatchesGridSetting(ref PercentageDataGrid grid)
        {
            if (grid.Columns.Contains("ProfilConfirmDateTime"))
                grid.Columns["ProfilConfirmDateTime"].Visible = false;
            if (grid.Columns.Contains("TPSConfirmDateTime"))
                grid.Columns["TPSConfirmDateTime"].Visible = false;
            if (grid.Columns.Contains("ProfilCloseDateTime"))
                grid.Columns["ProfilCloseDateTime"].Visible = false;
            if (grid.Columns.Contains("TPSCloseDateTime"))
                grid.Columns["TPSCloseDateTime"].Visible = false;
            grid.Columns["ProfilEnabled"].Visible = false;
            grid.Columns["TPSEnabled"].Visible = false;
            grid.Columns["MegaBatchID"].Visible = false;
            if (grid.Columns.Contains("ProfilConfirm"))
                grid.Columns["ProfilConfirm"].Visible = false;
            if (grid.Columns.Contains("TPSConfirm"))
                grid.Columns["TPSConfirm"].Visible = false;
            if (grid.Columns.Contains("ProfilCloseUserID"))
                grid.Columns["ProfilCloseUserID"].Visible = false;
            if (grid.Columns.Contains("TPSCloseUserID"))
                grid.Columns["TPSCloseUserID"].Visible = false;
            if (grid.Columns.Contains("ProfilConfirmUserID"))
                grid.Columns["ProfilConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("TPSConfirmUserID"))
                grid.Columns["TPSConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("ProfilWorkAssignmentID"))
                grid.Columns["ProfilWorkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("TPSWorkAssignmentID"))
                grid.Columns["TPSWorkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;
            if (grid.Columns.Contains("ProfilName"))
                grid.Columns["ProfilName"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["BatchID"].HeaderText = "№";
            grid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            grid.Columns["ProfilName"].HeaderText = "Название";
            grid.Columns["TPSName"].HeaderText = "Название";

            grid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FrontsSquare"].Width = 95;
            grid.Columns["BatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["BatchID"].Width = 55;
            grid.Columns["ProfilName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ProfilName"].MinimumWidth = 155;
            grid.Columns["TPSName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TPSName"].MinimumWidth = 155;
        }

        private void BatchMainOrdersGridSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("BatchID"))
                grid.Columns["BatchID"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            grid.Columns["OrderNumber"].HeaderText = "№ заказа\\документа";
            grid.Columns["MainOrderID"].HeaderText = "№ подзаказа";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ClientName"].MinimumWidth = 55;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 95;
            grid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FrontsSquare"].Width = 95;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["OrderNumber"].Width = 155;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 55;
        }

        private void MegaBatchesGridSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            grid.Columns["TPSEntryDateTime"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;
            if (grid.Columns.Contains("ProfilCloseUserID"))
                grid.Columns["ProfilCloseUserID"].Visible = false;
            if (grid.Columns.Contains("TPSCloseUserID"))
                grid.Columns["TPSCloseUserID"].Visible = false;

            if (grid.Columns.Contains("Group"))
            {
                grid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["Group"].Width = 45;
                grid.Columns["Group"].DisplayIndex = 0;
                grid.Columns["Group"].HeaderText = string.Empty;
            }

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TPSEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ProfilEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            grid.Columns["MegaBatchID"].HeaderText = "№";
            grid.Columns["CreateDateTime"].HeaderText = "Дата создания";
            grid.Columns["ProfilEntryDateTime"].HeaderText = "Вход на пр-во";
            grid.Columns["TPSEntryDateTime"].HeaderText = "Вход на пр-во";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["MegaBatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaBatchID"].Width = 55;
            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CreateDateTime"].MinimumWidth = 55;
            grid.Columns["ProfilEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ProfilEntryDateTime"].MinimumWidth = 55;
            grid.Columns["TPSEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TPSEntryDateTime"].MinimumWidth = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["Notes"].MinimumWidth = 155;
        }

        private void BatchFrontsOrdersGridSetting(ref PercentageDataGrid grid)
        {
            if (!grid.Columns.Contains("FrontsColumn"))
                grid.Columns.Add(FrontsOrdersManager.FrontsColumn);
            if (!grid.Columns.Contains("FrameColorsColumn"))
                grid.Columns.Add(FrontsOrdersManager.FrameColorsColumn);
            if (!grid.Columns.Contains("PatinaColumn"))
                grid.Columns.Add(FrontsOrdersManager.PatinaColumn);
            if (!grid.Columns.Contains("InsetTypesColumn"))
                grid.Columns.Add(FrontsOrdersManager.InsetTypesColumn);
            if (!grid.Columns.Contains("InsetColorsColumn"))
                grid.Columns.Add(FrontsOrdersManager.InsetColorsColumn);

            grid.Columns["FrontID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["InsetColorID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["InsetTypeID"].Visible = false;
            grid.Columns["TechnoProfileID"].Visible = false;
            grid.Columns["TechnoColorID"].Visible = false;
            grid.Columns["TechnoInsetTypeID"].Visible = false;
            grid.Columns["TechnoInsetColorID"].Visible = false;
            grid.Columns["FrontConfigID"].Visible = false;
            if (grid.Columns.Contains("ImpostMargin"))
                grid.Columns["ImpostMargin"].Visible = false;
            grid.Columns["FrontsOrdersID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;

            if (grid.Columns.Contains("CreateDateTime"))
            {
                grid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                grid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["CreateDateTime"].Width = 100;
            }
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            if (grid.Columns.Contains("DyeingAssignmentID"))
                grid.Columns["DyeingAssignmentID"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;
            if (grid.Columns.Contains("WorkAssignmentID"))
                grid.Columns["WorkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("MegaBatchID"))
                grid.Columns["MegaBatchID"].Visible = false;
            if (grid.Columns.Contains("BatchID"))
                grid.Columns["BatchID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            int DisplayIndex = 0;
            grid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["IsNonStandard"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].HeaderText = "Высота";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Square"].HeaderText = "Площадь";
            grid.Columns["IsNonStandard"].HeaderText = "Н\\С";

            grid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].Width = 85;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].Width = 85;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count"].Width = 85;
            grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Square"].Width = 85;
            grid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["IsNonStandard"].Width = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["Notes"].MinimumWidth = 185;
        }

        private void BatchColumnDyeingCartSetting(ref PercentageDataGrid grid)
        {
            int DyeingCartColumnNumber = 1;
            int DisplayIndex = 0;
            grid.Columns["BatchID"].DisplayIndex = DisplayIndex++;
            grid.Columns["ProfilName"].DisplayIndex = DisplayIndex++;
            grid.Columns["TPSName"].DisplayIndex = DisplayIndex++;
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                string ColumnName = grid.Columns[i].Name;
                if (ColumnName.Contains("ColumnDyeingCart"))
                {
                    //string s = ColumnName.Substring(16, ColumnName.Length - 16);
                    if (grid.Columns.Contains(ColumnName))
                    {
                        grid.Columns[ColumnName].ReadOnly = false;
                        grid.Columns[ColumnName].HeaderText = "Тележка " + DyeingCartColumnNumber++;
                        grid.Columns[ColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        grid.Columns[ColumnName].Width = 85;
                        grid.Columns[ColumnName].DisplayIndex = DisplayIndex++;
                    }
                }
            }
        }

        private void ColumnDyeingCartSetting(ref PercentageDataGrid grid)
        {
            int DyeingCartColumnNumber = 1;
            int DisplayIndex = 0;

            if (!grid.Columns.Contains("FrontsColumn"))
                grid.Columns.Add(FrontsOrdersManager.FrontsColumn);
            if (!grid.Columns.Contains("FrameColorsColumn"))
                grid.Columns.Add(FrontsOrdersManager.FrameColorsColumn);
            if (!grid.Columns.Contains("PatinaColumn"))
                grid.Columns.Add(FrontsOrdersManager.PatinaColumn);
            if (!grid.Columns.Contains("InsetTypesColumn"))
                grid.Columns.Add(FrontsOrdersManager.InsetTypesColumn);
            if (!grid.Columns.Contains("InsetColorsColumn"))
                grid.Columns.Add(FrontsOrdersManager.InsetColorsColumn);
            grid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;

            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            if (grid.Columns.Contains("PlanCount"))
                grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            if (grid.Columns.Contains("Count"))
                grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["IsNonStandard"].DisplayIndex = DisplayIndex++;
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                string ColumnName = grid.Columns[i].Name;
                if (ColumnName.Contains("ColumnDyeingCart"))
                {
                    //string s = ColumnName.Substring(16, ColumnName.Length - 16);
                    grid.Columns[ColumnName].ReadOnly = false;
                    grid.Columns[ColumnName].HeaderText = "Тележка " + DyeingCartColumnNumber++;
                    grid.Columns[ColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    grid.Columns[ColumnName].Width = 85;
                    grid.Columns[ColumnName].DisplayIndex = DisplayIndex++;
                }
            }
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        private void DyeingFrontsOrdersGridSetting(ref PercentageDataGrid grid)
        {
            if (!grid.Columns.Contains("FrontsColumn"))
                grid.Columns.Add(FrontsOrdersManager.FrontsColumn);
            if (!grid.Columns.Contains("FrameColorsColumn"))
                grid.Columns.Add(FrontsOrdersManager.FrameColorsColumn);
            if (!grid.Columns.Contains("PatinaColumn"))
                grid.Columns.Add(FrontsOrdersManager.PatinaColumn);
            if (!grid.Columns.Contains("InsetTypesColumn"))
                grid.Columns.Add(FrontsOrdersManager.InsetTypesColumn);
            if (!grid.Columns.Contains("InsetColorsColumn"))
                grid.Columns.Add(FrontsOrdersManager.InsetColorsColumn);

            grid.Columns["FrontID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["InsetColorID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["InsetTypeID"].Visible = false;
            grid.Columns["TechnoProfileID"].Visible = false;
            grid.Columns["TechnoColorID"].Visible = false;
            grid.Columns["TechnoInsetTypeID"].Visible = false;
            grid.Columns["TechnoInsetColorID"].Visible = false;
            grid.Columns["FrontConfigID"].Visible = false;
            if (grid.Columns.Contains("ImpostMargin"))
                grid.Columns["ImpostMargin"].Visible = false;
            grid.Columns["FrontsOrdersID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;

            if (grid.Columns.Contains("CreateDateTime"))
            {
                grid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                grid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["CreateDateTime"].Width = 100;
            }
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            if (grid.Columns.Contains("FactCount"))
                grid.Columns["FactCount"].Visible = false;
            if (grid.Columns.Contains("DefectCount"))
                grid.Columns["DefectCount"].Visible = false;

            if (grid.Columns.Contains("DyeingAssignmentID"))
                grid.Columns["DyeingAssignmentID"].Visible = false;
            if (grid.Columns.Contains("DyeingAssignmentDetailID"))
                grid.Columns["DyeingAssignmentDetailID"].Visible = false;
            if (grid.Columns.Contains("DyeingCartID"))
                grid.Columns["DyeingCartID"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;
            if (grid.Columns.Contains("WorkAssignmentID"))
                grid.Columns["WorkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("MegaBatchID"))
                grid.Columns["MegaBatchID"].Visible = false;
            if (grid.Columns.Contains("BatchID"))
                grid.Columns["BatchID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            int DisplayIndex = 0;
            grid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["IsNonStandard"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].HeaderText = "Высота";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["PlanCount"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Square"].HeaderText = "Площадь";
            grid.Columns["IsNonStandard"].HeaderText = "Н\\С";

            grid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].Width = 85;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].Width = 85;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].Width = 85;
            grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Square"].Width = 85;
            grid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["IsNonStandard"].Width = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["Notes"].MinimumWidth = 185;
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

        private void cmiOpenAssingnment_Click(object sender, EventArgs e)
        {
            pnlAssignmentPage.BringToFront();
        }

        private void dgvBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (FrontsOrdersManager == null || ControlAssignmentsManager == null)
                return;

            int MegaBatchID = 0;
            int BatchID = 0;
            int FactoryID = ControlAssignmentsManager.FactoryID;
            int WorkAssignmentID = ControlAssignmentsManager.WorkAssignmentID;

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);

            int GroupType = 0;
            int GroupType1 = 0;
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["GroupType"].Value);
            if (cbGroupTypes.SelectedItem != null && cbGroupTypes.SelectedItem != DBNull.Value)
                GroupType1 = Convert.ToInt32(((DataRowView)cbGroupTypes.SelectedItem).Row["ID"]);

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                FrontsOrdersManager.GetSavedFrontsOrders(BatchID);
                ControlAssignmentsManager.FilterBatchMainOrders(GroupType, BatchID);
                BatchColumnDyeingCartSetting(ref dgvBatches);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                FrontsOrdersManager.GetSavedFrontsOrders(BatchID);
                ControlAssignmentsManager.FilterBatchMainOrders(GroupType, BatchID);
                BatchColumnDyeingCartSetting(ref dgvBatches);
            }
            ShowButtons();
        }

        private void dgvMegaBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (ControlAssignmentsManager == null)
                return;
            int GroupType = 0;
            int MegaBatchID = 0;
            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["GroupType"].Value);

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                //ControlAssignmentsManager.FilterBatchesByMegaBatch();
                ControlAssignmentsManager.FilterBatchesByMegaBatch(GroupType, MegaBatchID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                //ControlAssignmentsManager.FilterBatchesByMegaBatch();
                ControlAssignmentsManager.FilterBatchesByMegaBatch(GroupType, MegaBatchID);
        }

        private void dgvBatchMainOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (FrontsOrdersManager == null)
                return;
            int MegaBatchID = 0;
            int BatchID = 0;
            int GroupType = 0;
            int WorkAssignmentID = ControlAssignmentsManager.WorkAssignmentID;

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["GroupType"].Value);

            int ArrayCount = 1;
            if (dgvBatchMainOrders.SelectedRows.Count > 0)
                ArrayCount = dgvBatchMainOrders.SelectedRows.Count;
            int[] MainOrders = new int[ArrayCount];
            for (int i = 0; i < dgvBatchMainOrders.SelectedRows.Count; i++)
            {
                MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value);
            }
            if (MainOrders.Count() == 0)
                MainOrders[0] = 0;

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;

                if (ControlAssignmentsManager.NewAssignment)
                {
                    pnlBatchFronts.BringToFront();
                    FrontsOrdersManager.FilterBatchFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
                }
                else
                {
                    pnlDyeingFronts.BringToFront();
                    FrontsOrdersManager.FilterDyeingFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, ControlAssignmentsManager.GroupType, MainOrders);
                    ColumnDyeingCartSetting(ref dgvDyeingFrontsOrders);
                }

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                if (ControlAssignmentsManager.NewAssignment)
                {
                    pnlBatchFronts.BringToFront();
                    FrontsOrdersManager.FilterBatchFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
                }
                else
                {
                    pnlDyeingFronts.BringToFront();
                    FrontsOrdersManager.FilterDyeingFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, ControlAssignmentsManager.GroupType, MainOrders);
                    ColumnDyeingCartSetting(ref dgvDyeingFrontsOrders);
                }
            }
        }

        private void dgvBatchMainOrders_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows.Count == 0)
                return;

            int MainOrderID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["MainOrderID"].Value);

            if (ControlAssignmentsManager.IsMainOrderSaved(MainOrderID))
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void btnAddMarketingWholeColumn_Click(object sender, EventArgs e)
        {
            FrontsOrdersManager.AddDyeingCartColumn(ControlAssignmentsManager.DyeingCartID);
            DyeingFrontsOrdersGridSetting(ref dgvDyeingFrontsOrders);
            ColumnDyeingCartSetting(ref dgvDyeingFrontsOrders);
        }

        private void btnSaveMarketingWhole_Click(object sender, EventArgs e)
        {
            if (dgvBatchMainOrders.SelectedRows.Count == 0)
                return;

            ControlAssignmentsManager.GroupType = 2;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            int[] MainOrders = new int[dgvBatchMainOrders.SelectedRows.Count];

            int MegaBatchID = 0;
            int BatchID = 0;
            int GroupType = 0;
            int WorkAssignmentID = ControlAssignmentsManager.WorkAssignmentID;

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["GroupType"].Value);

            for (int i = 0; i < dgvBatchMainOrders.SelectedRows.Count; i++)
            {
                MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value);
                if (ControlAssignmentsManager.IsMainOrderSaved(Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value)))
                {
                    //ControlAssignmentsManager.NewAssignment = false;
                }
            }

            //FrontsOrdersManager.MarketingReatailFrontOrders(GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
            FrontsOrdersManager.MarketingReatailFrontOrders(ControlAssignmentsManager.NewAssignment);

            DyeingAssignmentsMaterialForm NeedPaintingMaterialForm = new DyeingAssignmentsMaterialForm(this, ref ControlAssignmentsManager, ref FrontsOrdersManager, From, To);

            TopForm = NeedPaintingMaterialForm;

            NeedPaintingMaterialForm.ShowDialog();

            NeedPaintingMaterialForm.Close();
            NeedPaintingMaterialForm.Dispose();

            TopForm = null;
            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            //ControlAssignmentsManager.NewAssignment = true;
            ControlAssignmentsManager.GetSavedMainOrders();
            FrontsOrdersManager.GetSavedFrontsOrders(BatchID);
            ShowButtons();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnAddMarketingWholeColumn1_Click(object sender, EventArgs e)
        {
            FrontsOrdersManager.AddDyeingCartColumn1();
            BatchFrontsOrdersGridSetting(ref dgvBatchFrontsOrders);
            ColumnDyeingCartSetting(ref dgvBatchFrontsOrders);
        }

        private void btnAddMarketingRetailColumn_Click(object sender, EventArgs e)
        {
            ControlAssignmentsManager.AddDyeingCartColumn1();
            BatchesGridSetting(ref dgvBatches);
            BatchColumnDyeingCartSetting(ref dgvBatches);
        }

        private void btnSaveMarketingRetail_Click(object sender, EventArgs e)
        {
            if (dgvBatchMainOrders.SelectedRows.Count == 0)
                return;

            ControlAssignmentsManager.GroupType = 1;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;
            int MegaBatchID = 0;
            int BatchID = 0;
            int GroupType = 0;
            int WorkAssignmentID = ControlAssignmentsManager.WorkAssignmentID;

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["GroupType"].Value);

            int[] MainOrders = new int[dgvBatchMainOrders.Rows.Count];

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["GroupType"].Value);

            for (int i = 0; i < dgvBatchMainOrders.Rows.Count; i++)
            {
                MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.Rows[i].Cells["MainOrderID"].Value);
                if (ControlAssignmentsManager.IsMainOrderSaved(Convert.ToInt32(dgvBatchMainOrders.Rows[i].Cells["MainOrderID"].Value)))
                {
                    //ControlAssignmentsManager.NewAssignment = false;
                }
            }
            FrontsOrdersManager.FilterBatchFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
            //FrontsOrdersManager.MarketingReatailFrontOrders(GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
            FrontsOrdersManager.MarketingReatailFrontOrders(ControlAssignmentsManager.NewAssignment);
            //if (ControlAssignmentsManager.NewAssignment)
            //{
            //    FrontsOrdersManager.FilterBatchFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
            //    FrontsOrdersManager.MarketingReatailFrontOrders(MainOrders);
            //}
            //else
            //{
            //    FrontsOrdersManager.FilterDyeingFrontsByMainOrder(ControlAssignmentsManager.DyeingAssignmentID, ControlAssignmentsManager.GroupType, MainOrders);
            //    FrontsOrdersManager.MarketingReatailFrontOrders(MainOrders);
            //}

            DyeingAssignmentsMaterialForm NeedPaintingMaterialForm = new DyeingAssignmentsMaterialForm(this, ref ControlAssignmentsManager, ref FrontsOrdersManager, From, To);

            TopForm = NeedPaintingMaterialForm;

            NeedPaintingMaterialForm.ShowDialog();

            NeedPaintingMaterialForm.Close();
            NeedPaintingMaterialForm.Dispose();

            TopForm = null;

            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            int DyeingAssignmentID = ControlAssignmentsManager.DyeingAssignmentID;
            //ControlAssignmentsManager.NewAssignment = true;
            ControlAssignmentsManager.GetSavedMainOrders();
            FrontsOrdersManager.GetSavedFrontsOrders(BatchID);
            ShowButtons();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnSaveZOV_Click(object sender, EventArgs e)
        {
            if (dgvBatchMainOrders.SelectedRows.Count == 0)
                return;
            int MegaBatchID = 0;
            int BatchID = 0;
            int GroupType = 0;
            int WorkAssignmentID = ControlAssignmentsManager.WorkAssignmentID;

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["GroupType"].Value);

            ControlAssignmentsManager.GroupType = 0;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            if (cbGroupTypes.SelectedItem != null && cbGroupTypes.SelectedItem != DBNull.Value)
                ControlAssignmentsManager.GroupType = Convert.ToInt32(((DataRowView)cbGroupTypes.SelectedItem).Row["ID"]);

            if (ControlAssignmentsManager.GroupType == 0 || ControlAssignmentsManager.GroupType == 3)
            {
                int[] MainOrders = new int[dgvBatchMainOrders.SelectedRows.Count];
                ControlAssignmentsManager.MainOrders.Clear();
                for (int i = 0; i < dgvBatchMainOrders.SelectedRows.Count; i++)
                {
                    ControlAssignmentsManager.MainOrders.Add(Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value));
                    MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value);
                }

                FrontsOrdersManager.ZOVFrontOrders(ControlAssignmentsManager.NewAssignment, GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
            }
            if (ControlAssignmentsManager.GroupType == 4)
            {
                if (ControlAssignmentsManager.NewAssignment)
                {
                    int[] FrontsOrders = new int[dgvBatchFrontsOrders.SelectedRows.Count];
                    for (int i = 0; i < dgvBatchFrontsOrders.SelectedRows.Count; i++)
                    {
                        FrontsOrders[i] = Convert.ToInt32(dgvBatchFrontsOrders.SelectedRows[i].Cells["FrontsOrdersID"].Value);
                    }
                    FrontsOrdersManager.ReOrdersFrontOrders(ControlAssignmentsManager.NewAssignment, FrontsOrders);
                }
                else
                {
                    int[] FrontsOrders = new int[dgvDyeingFrontsOrders.SelectedRows.Count];
                    for (int i = 0; i < dgvDyeingFrontsOrders.SelectedRows.Count; i++)
                    {
                        FrontsOrders[i] = Convert.ToInt32(dgvDyeingFrontsOrders.SelectedRows[i].Cells["FrontsOrdersID"].Value);
                    }
                    FrontsOrdersManager.ReOrdersFrontOrders(ControlAssignmentsManager.NewAssignment, FrontsOrders);
                }
            }
            //if (ControlAssignmentsManager.NewAssignment)
            //{
            //    if (ControlAssignmentsManager.GroupType == 0)
            //    {
            //        int[] MainOrders = new int[dgvBatchMainOrders.SelectedRows.Count];
            //        ControlAssignmentsManager.MainOrders.Clear();
            //        for (int i = 0; i < dgvBatchMainOrders.SelectedRows.Count; i++)
            //        {
            //            ControlAssignmentsManager.MainOrders.Add(Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value));
            //            MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value);
            //        }

            //        FrontsOrdersManager.ZOVFrontOrders(GroupType, WorkAssignmentID, MegaBatchID, BatchID, MainOrders);
            //    }
            //    if (ControlAssignmentsManager.GroupType == 4)
            //    {
            //        int[] FrontsOrders = new int[dgvBatchFrontsOrders.SelectedRows.Count];
            //        for (int i = 0; i < dgvBatchFrontsOrders.SelectedRows.Count; i++)
            //        {
            //            FrontsOrders[i] = Convert.ToInt32(dgvBatchFrontsOrders.SelectedRows[i].Cells["FrontsOrdersID"].Value);
            //        }
            //        FrontsOrdersManager.ReOrdersFrontOrders(FrontsOrders);
            //    }
            //}
            //else
            //{
            //    int[] MainOrders = new int[dgvBatchMainOrders.SelectedRows.Count];

            //    for (int i = 0; i < dgvBatchMainOrders.SelectedRows.Count; i++)
            //    {
            //        MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value);
            //        if (ControlAssignmentsManager.IsMainOrderSaved(Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value)))
            //        {
            //            ControlAssignmentsManager.NewAssignment = false;
            //        }
            //    }

            //    int DyeingAssignmentID = ControlAssignmentsManager.FindDyeingAssignmentID(MainOrders[0]);
            //    FrontsOrdersManager.GetTempFrontOrders(true, DyeingAssignmentID, MainOrders);
            //}
            DyeingAssignmentsMaterialForm NeedPaintingMaterialForm = new DyeingAssignmentsMaterialForm(this, ref ControlAssignmentsManager, ref FrontsOrdersManager, From, To);

            TopForm = NeedPaintingMaterialForm;

            NeedPaintingMaterialForm.ShowDialog();

            NeedPaintingMaterialForm.Close();
            NeedPaintingMaterialForm.Dispose();

            TopForm = null;
            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            //ControlAssignmentsManager.NewAssignment = true;
            ControlAssignmentsManager.GetSavedMainOrders();
            FrontsOrdersManager.GetSavedFrontsOrders(BatchID);
            ShowButtons();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnSaveMarketingWhole1_Click(object sender, EventArgs e)
        {
            if (dgvBatchMainOrders.SelectedRows.Count == 0)
                return;

            ControlAssignmentsManager.GroupType = 2;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            int GroupType = 0;
            int[] MainOrders = new int[dgvBatchMainOrders.SelectedRows.Count];

            if (dgvMegaBatches.SelectedRows.Count != 0 && dgvMegaBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvMegaBatches.SelectedRows[0].Cells["GroupType"].Value);
            for (int i = 0; i < dgvBatchMainOrders.SelectedRows.Count; i++)
            {
                MainOrders[i] = Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value);
                if (ControlAssignmentsManager.IsMainOrderSaved(Convert.ToInt32(dgvBatchMainOrders.SelectedRows[i].Cells["MainOrderID"].Value)))
                {
                    //ControlAssignmentsManager.NewAssignment = false;
                }
            }

            FrontsOrdersManager.MarketingWholeFrontOrders(ControlAssignmentsManager.NewAssignment);
            //if (ControlAssignmentsManager.NewAssignment)
            //    FrontsOrdersManager.MarketingWholeFrontOrders(MainOrders);
            //else
            //    FrontsOrdersManager.GetTempFrontOrders(ZOV, ControlAssignmentsManager.DyeingAssignmentID, MainOrders);

            DyeingAssignmentsMaterialForm NeedPaintingMaterialForm = new DyeingAssignmentsMaterialForm(this, ref ControlAssignmentsManager, ref FrontsOrdersManager, From, To);

            TopForm = NeedPaintingMaterialForm;

            NeedPaintingMaterialForm.ShowDialog();

            NeedPaintingMaterialForm.Close();
            NeedPaintingMaterialForm.Dispose();

            TopForm = null;
            Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T1.Start();

            while (!SplashWindow.bSmallCreated) ;

            //ControlAssignmentsManager.NewAssignment = true;
            ControlAssignmentsManager.GetSavedMainOrders();
            int BatchID = 0;
            if (dgvBatches.SelectedRows.Count != 0 && dgvBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
            FrontsOrdersManager.GetSavedFrontsOrders(BatchID);
            ShowButtons();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cbGroupTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int GroupType = 0;
            if (cbGroupTypes.SelectedItem != null && cbGroupTypes.SelectedItem != DBNull.Value)
                GroupType = Convert.ToInt32(((DataRowView)cbGroupTypes.SelectedItem).Row["ID"]);
            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                ControlAssignmentsManager.FilterMegaBatches(GroupType);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                ControlAssignmentsManager.FilterMegaBatches(GroupType);
            ShowButtons();
        }

        private void dgvBatchFrontsOrders_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows.Count == 0 || ControlAssignmentsManager.GroupType != 4)
                return;

            int FrontsOrdersID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["FrontsOrdersID"].Value);

            if (FrontsOrdersManager.IsFrontOrderSaved(FrontsOrdersID))
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void dgvDyeingFrontsOrders_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows.Count == 0 || ControlAssignmentsManager.GroupType != 4)
                return;

            int FrontsOrdersID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["FrontsOrdersID"].Value);

            if (FrontsOrdersManager.IsFrontOrderSaved(FrontsOrdersID))
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    }
}
