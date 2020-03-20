using Infinium.Modules.DyeingAssignments;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DyeingAssignmentsMaterialForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;
        //decimal Square = 0;
        int FormEvent = 0;

        //ArrayList FrontConfigID;

        Form MainForm;
        Form TopForm = null;
        DateTime From = DateTime.Now;
        DateTime To = DateTime.Now;

        ControlAssignments ControlAssignmentsManager;
        FrontsOrdersManager FrontsOrdersManager;
        NeedPaintingMaterial NeedPaintingMaterialManager;

        public DyeingAssignmentsMaterialForm(Form tMainForm, ref ControlAssignments tControlAssignmentsManager, ref FrontsOrdersManager tFrontsOrdersManager,
            DateTime dFrom, DateTime dTo)
        {
            InitializeComponent();
            MainForm = tMainForm;
            From = dFrom;
            To = dTo;
            ControlAssignmentsManager = tControlAssignmentsManager;
            FrontsOrdersManager = tFrontsOrdersManager;
            //FrontConfigID = aFrontConfigID;
            //Square = dSquare;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void NeedPaintingMaterialForm_Shown(object sender, EventArgs e)
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
            NeedPaintingMaterialManager = new NeedPaintingMaterial()
            {
                FrontConfigID = FrontsOrdersManager.DinstinctFrontsConfig(),
                PatinaID = FrontsOrdersManager.DinstinctFrontsPatina()
            };
            NeedPaintingMaterialManager.Initialize();
            //if (!ControlAssignmentsManager.CanSave)
            //    NeedPaintingMaterialManager.UpdateTables(ControlAssignmentsManager.DyeingAssignmentID);
            //else
            NeedPaintingMaterialManager.UpdateTables();
            NeedPaintingMaterialManager.FindTerms();
            decimal Square = 0;
            int Count = 0;
            FrontsOrdersManager.GetSquare(ControlAssignmentsManager.NewAssignment, ref Square, ref Count);
            NeedPaintingMaterialManager.CalcOperationDetailConsumption(Square, Count);
            NeedPaintingMaterialManager.CalcSumStoreDetailConsumption();

            dgvStore.DataSource = NeedPaintingMaterialManager.StoreList;
            dgvStoreDetail.DataSource = NeedPaintingMaterialManager.StoreDetailList;
            dgvOperationsDetail.DataSource = NeedPaintingMaterialManager.OperationsDetailList;
            dgvOperationsGroups.DataSource = NeedPaintingMaterialManager.OperationsGroupsList;
            dgvSummaryStoreDetail.DataSource = NeedPaintingMaterialManager.SummaryStoreDetailList;

            dgvStoreSetting(ref dgvStore);
            dgvStoreDetailSetting(ref dgvStoreDetail);
            dgvSummaryStoreDetailSetting(ref dgvSummaryStoreDetail);
            dgvOperationsDetailSetting(ref dgvOperationsDetail);
            dgvOperationsGroupsSetting(ref dgvOperationsGroups);
        }

        private void dgvOperationsGroupsSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechCatalogOperationsGroupID"))
                grid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("GroupNumber"))
                grid.Columns["GroupNumber"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["GroupName"].HeaderText = "Группа операций";
            grid.Columns["GroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["GroupName"].MinimumWidth = 155;
        }

        private void dgvOperationsDetailSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("MeasureID"))
                grid.Columns["MeasureID"].Visible = false;
            if (grid.Columns.Contains("TechCatalogOperationsDetailID"))
                grid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            if (grid.Columns.Contains("MachinesOperationID"))
                grid.Columns["MachinesOperationID"].Visible = false;
            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            if (grid.Columns.Contains("IsPerform"))
                grid.Columns["IsPerform"].Visible = false;
            if (grid.Columns.Contains("TechCatalogOperationsGroupID"))
                grid.Columns["TechCatalogOperationsGroupID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.Columns["Consumption"].HeaderText = "Плановое\r\n   время";
            grid.Columns["MachinesOperationName"].HeaderText = "Операция";
            grid.Columns["Measure"].HeaderText = "Ед.изм.";
            grid.Columns["Norm"].HeaderText = "Норма";
            grid.Columns["Norm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Norm"].Width = 65;
            grid.Columns["Consumption"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Consumption"].Width = 95;
            grid.Columns["MachinesOperationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["MachinesOperationName"].MinimumWidth = 55;
            grid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Measure"].Width = 70;

            grid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            grid.Columns["MachinesOperationName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Norm"].DisplayIndex = DisplayIndex++;
            grid.Columns["Consumption"].DisplayIndex = DisplayIndex++;
            grid.Columns["Measure"].DisplayIndex = DisplayIndex++;
        }

        private void dgvStoreSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("StoreID"))
                grid.Columns["StoreID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";
            grid.Columns["TechStoreName"].HeaderText = "Наименование";
            grid.Columns["Measure"].HeaderText = "Ед.изм.";
            grid.Columns["Produced"].HeaderText = "Произведено";
            grid.Columns["BestBefore"].HeaderText = "Срок годности";
            grid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Measure"].Width = 70;
            grid.Columns["Produced"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Produced"].Width = 115;
            grid.Columns["BestBefore"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["BestBefore"].Width = 115;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 95;
            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 95;
        }

        private void dgvStoreDetailSetting(ref PercentageDataGrid grid)
        {
            if (grid.Columns.Contains("MeasureID"))
                grid.Columns["MeasureID"].Visible = false;
            if (grid.Columns.Contains("TechCatalogOperationsGroupID"))
                grid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("TechCatalogOperationsDetailID"))
                grid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            if (grid.Columns.Contains("TechCatalogStoreDetailID"))
                grid.Columns["TechCatalogStoreDetailID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.Columns["Count"].HeaderText = "Удельный\r\n   расход";
            grid.Columns["Consumption"].HeaderText = "Общий\r\nрасход";
            grid.Columns["TechStoreName"].HeaderText = "Наименование";
            grid.Columns["Measure"].HeaderText = "Ед.изм.";
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Count"].Width = 95;
            grid.Columns["Consumption"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Consumption"].Width = 95;
            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 95;
            grid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Measure"].Width = 70;

            grid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Consumption"].DisplayIndex = DisplayIndex++;
            grid.Columns["Measure"].DisplayIndex = DisplayIndex++;
        }

        private void dgvSummaryStoreDetailSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechCatalogOperationsGroupID"))
                grid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.Columns["Consumption"].HeaderText = "Расход";
            grid.Columns["TechStoreName"].HeaderText = "Наименование";
            grid.Columns["Measure"].HeaderText = "Ед.изм.";
            grid.Columns["Consumption"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Consumption"].Width = 95;
            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 95;
            grid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Measure"].Width = 70;
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

        private void btnBackToMainPage_Click(object sender, EventArgs e)
        {

        }

        private void dgvWorkAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            //{
            //    kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            //}
        }

        private void dgvOperationsDetail_SelectionChanged(object sender, EventArgs e)
        {
            if (NeedPaintingMaterialManager == null)
                return;

            int TechCatalogOperationsDetailID = 0;
            if (dgvOperationsDetail.SelectedRows.Count != 0 && dgvOperationsDetail.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value != DBNull.Value)
                TechCatalogOperationsDetailID = Convert.ToInt32(dgvOperationsDetail.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value);

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedPaintingMaterialManager.FilterStoreDetail(TechCatalogOperationsDetailID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                NeedPaintingMaterialManager.FilterStoreDetail(TechCatalogOperationsDetailID);
            }
        }

        private void dgvOperationsGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (NeedPaintingMaterialManager == null)
                return;
            int TechCatalogOperationsGroupID = 0;
            if (dgvOperationsGroups.SelectedRows.Count != 0 && dgvOperationsGroups.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                TechCatalogOperationsGroupID = Convert.ToInt32(dgvOperationsGroups.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedPaintingMaterialManager.FilterOperationsDetail(TechCatalogOperationsGroupID);
                NeedPaintingMaterialManager.FilterSummaryStoreDetail(TechCatalogOperationsGroupID);
                NeedPaintingMaterialManager.GetStore(TechCatalogOperationsGroupID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
            {
                NeedPaintingMaterialManager.FilterOperationsDetail(TechCatalogOperationsGroupID);
                NeedPaintingMaterialManager.FilterSummaryStoreDetail(TechCatalogOperationsGroupID);
                NeedPaintingMaterialManager.GetStore(TechCatalogOperationsGroupID);
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {

        }

        private void cmiPrintBatches_Click(object sender, EventArgs e)
        {
        }

        private void cmiPrintBatchMainOrders_Click(object sender, EventArgs e)
        {
        }

        private void dgvBatches_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            //{
            //    kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            //}
        }

        private void dgvBatchMainOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            //{
            //    kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            //}
        }

        private void dgvOperationsGroups_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (!ControlAssignmentsManager.CanSave)
            //    return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                NeedPaintingMaterialManager.MoveToOperationsGroupPos(e.RowIndex);
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiSaveDyeingAssignments_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            int TechCatalogOperationsGroupID = 0;
            if (dgvOperationsGroups.SelectedRows.Count != 0 && dgvOperationsGroups.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                TechCatalogOperationsGroupID = Convert.ToInt32(dgvOperationsGroups.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            ControlAssignmentsManager.TechCatalogOperationsGroupID = TechCatalogOperationsGroupID;
            if (ControlAssignmentsManager.NewAssignment)
            {
                switch (ControlAssignmentsManager.GroupType)
                {
                    case 0:
                        foreach (int item in ControlAssignmentsManager.MainOrders)
                        {
                            int MainOrderID = item;
                            ControlAssignmentsManager.CreateDyeingAssignment(TechCatalogOperationsGroupID);
                            ControlAssignmentsManager.SaveDyeingAssignments();
                            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
                            FrontsOrdersManager.SaveZOV(ControlAssignmentsManager, MainOrderID);
                            ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                            ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                            ControlAssignmentsManager.RemoveDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID);
                            for (int i = 0; i < dgvOperationsDetail.Rows.Count; i++)
                            {
                                int TechCatalogOperationsDetailID = Convert.ToInt32(dgvOperationsDetail.Rows[i].Cells["TechCatalogOperationsDetailID"].Value);
                                ControlAssignmentsManager.CreateDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID);
                            }
                            ControlAssignmentsManager.SaveDyeingAssignmentBarcodes();
                            ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        }
                        break;
                    case 1:
                        ControlAssignmentsManager.CreateDyeingAssignment(TechCatalogOperationsGroupID);
                        ControlAssignmentsManager.SaveDyeingAssignments();
                        ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
                        ControlAssignmentsManager.SaveDyeingBatch();
                        FrontsOrdersManager.SaveMarketingReatailFronts(ControlAssignmentsManager);
                        ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.RemoveDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID);
                        for (int i = 0; i < dgvOperationsDetail.Rows.Count; i++)
                        {
                            int TechCatalogOperationsDetailID = Convert.ToInt32(dgvOperationsDetail.Rows[i].Cells["TechCatalogOperationsDetailID"].Value);
                            ControlAssignmentsManager.CreateDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID);
                        }
                        ControlAssignmentsManager.SaveDyeingAssignmentBarcodes();
                        ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        break;
                    case 2:
                        ControlAssignmentsManager.CreateDyeingAssignment(TechCatalogOperationsGroupID);
                        ControlAssignmentsManager.SaveDyeingAssignments();
                        ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
                        FrontsOrdersManager.SaveBatchFronts(ControlAssignmentsManager);
                        ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.RemoveDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID);
                        for (int i = 0; i < dgvOperationsDetail.Rows.Count; i++)
                        {
                            int TechCatalogOperationsDetailID = Convert.ToInt32(dgvOperationsDetail.Rows[i].Cells["TechCatalogOperationsDetailID"].Value);
                            ControlAssignmentsManager.CreateDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID);
                        }
                        ControlAssignmentsManager.SaveDyeingAssignmentBarcodes();
                        ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        break;
                    case 3:

                        break;
                    case 4:
                        ControlAssignmentsManager.CreateDyeingAssignment(TechCatalogOperationsGroupID);
                        ControlAssignmentsManager.SaveDyeingAssignments();
                        ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
                        FrontsOrdersManager.SaveReOrders(ControlAssignmentsManager);
                        ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                        ControlAssignmentsManager.RemoveDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID);
                        for (int i = 0; i < dgvOperationsDetail.Rows.Count; i++)
                        {
                            int TechCatalogOperationsDetailID = Convert.ToInt32(dgvOperationsDetail.Rows[i].Cells["TechCatalogOperationsDetailID"].Value);
                            ControlAssignmentsManager.CreateDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID);
                        }
                        ControlAssignmentsManager.SaveDyeingAssignmentBarcodes();
                        ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                        break;
                    default:
                        break;
                }
                //ControlAssignmentsManager.NewAssignment = false;

            }
            else
            {
                if (ControlAssignmentsManager.GroupType == 1)
                {
                    ControlAssignmentsManager.SaveDyeingBatch();
                }
                FrontsOrdersManager.SaveDyeingFronts(ControlAssignmentsManager);

                ControlAssignmentsManager.ChangeOperationsGroup(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID);

                ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
                ControlAssignmentsManager.UpdateDyeingCarts(ControlAssignmentsManager.DyeingAssignmentID);
                ControlAssignmentsManager.RemoveDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID);
                for (int i = 0; i < dgvOperationsDetail.Rows.Count; i++)
                {
                    int TechCatalogOperationsDetailID = Convert.ToInt32(dgvOperationsDetail.Rows[i].Cells["TechCatalogOperationsDetailID"].Value);
                    ControlAssignmentsManager.CreateDyeingAssignmentBarcode(ControlAssignmentsManager.DyeingAssignmentID, TechCatalogOperationsGroupID, TechCatalogOperationsDetailID);
                }
                ControlAssignmentsManager.SaveDyeingAssignmentBarcodes();
                ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(ControlAssignmentsManager.DyeingAssignmentID);
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvOperationsGroups_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Rows.Count == 0)
                return;

            int TechCatalogOperationsGroupID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["TechCatalogOperationsGroupID"].Value);

            if (!ControlAssignmentsManager.NewAssignment && TechCatalogOperationsGroupID == ControlAssignmentsManager.TechCatalogOperationsGroupID)
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
