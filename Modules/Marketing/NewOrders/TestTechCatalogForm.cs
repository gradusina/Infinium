using Infinium.Modules.TechnologyCatalog;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TestTechCatalogForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        //bool NeedSplash = false;
        public bool bPrint = false;

        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        TestTechCatalog TestTechCatalogManager;

        public TestTechCatalogForm(Form tMainForm, ref TestTechCatalog tTestTechCatalogManager)
        {
            InitializeComponent();
            MainForm = tMainForm;
            TestTechCatalogManager = tTestTechCatalogManager;

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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        //NeedSplash = false;
                        MainForm.Activate();
                        this.Hide();
                    }

                    return;
                }

                if (FormEvent == eShow)
                {
                    //NeedSplash = true;
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
                        //NeedSplash = false;
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
                    //NeedSplash = true;
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                }

                return;
            }
        }

        private void TestTechCatalogForm_Shown(object sender, EventArgs e)
        {
            //NeedSplash = true;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void dgvMainOrders_SelectionChanged(object sender, EventArgs e)
        {
            //if (TestTechCatalogManager == null)
            //    return;
            //int GroupType = 0;
            //int MainOrderID = 0;
            //if (dgvMaterials.SelectedRows.Count != 0 && dgvMaterials.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
            //    GroupType = Convert.ToInt32(dgvMaterials.SelectedRows[0].Cells["GroupType"].Value);
            //if (dgvMaterials.SelectedRows.Count != 0 && dgvMaterials.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
            //    MainOrderID = Convert.ToInt32(dgvMaterials.SelectedRows[0].Cells["MainOrderID"].Value);

            //if (NeedSplash)
            //{
            //    NeedSplash = false;
            //    Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            //    T.Start();
            //    while (!SplashWindow.bSmallCreated) ;
            //    //TafelManager.FilterOrdersByMainOrder(GroupType, MainOrderID);
            //    while (SplashWindow.bSmallCreated)
            //        SmallWaitForm.CloseS = true;
            //    NeedSplash = true;
            //}
            //else
            //TafelManager.FilterOrdersByMainOrder(GroupType, MainOrderID);
        }

        private void TestTechCatalogForm_Load(object sender, EventArgs e)
        {
            dgvResult.DataSource = TestTechCatalogManager.ResultDV;
            dgvResultSetting(ref dgvResult);
        }

        private void dgvMaterialsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns["GroupType"].Visible = false;

            grid.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
            }

            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["OrderNumber"].HeaderText = "№ док.";
            grid.Columns["MainOrderID"].HeaderText = "№ п\\п";

            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ClientName"].MinimumWidth = 105;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderNumber"].MinimumWidth = 55;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MainOrderID"].MinimumWidth = 55;
        }

        private void dgvResultSetting(ref PercentageDataGrid grid)
        {
            //if (grid.Columns.Contains("TechCatalogOperationsGroupID"))
            //    grid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            if (grid.Columns.Contains("hide"))
                grid.Columns["hide"].Visible = false;
            if (grid.Columns.Contains("PrevTechCatalogOperationsDetailID"))
                grid.Columns["PrevTechCatalogOperationsDetailID"].Visible = false;
            //if (grid.Columns.Contains("TechCatalogOperationsDetailID"))
            //    grid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("TechStoreName"))
                grid.Columns["TechStoreName"].Visible = false;
            if (grid.Columns.Contains("Cost"))
                grid.Columns["Cost"].Visible = false;
            if (grid.Columns.Contains("Measure"))
                grid.Columns["Measure"].Visible = false;

            grid.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
            }

            //grid.Columns["SerialNumber"].HeaderText = "№ п/п";
            //grid.Columns["GroupName"].HeaderText = "Группа операций";
            //grid.Columns["MachinesOperationName"].HeaderText = "Операция";
            //grid.Columns["TechStoreName"].HeaderText = "Материал";
            //grid.Columns["Cost"].HeaderText = "Расход";
            //grid.Columns["Measure"].HeaderText = "Ед.изм.";
            grid.Columns["check"].HeaderText = string.Empty;

            //grid.Columns["SerialNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["GroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["MachinesOperationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //grid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["check"].Width = 35;

            //grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Height"].Width = 85;
            //grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Width"].Width = 85;
            //grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Count"].Width = 85;
            //grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Square"].Width = 85;
            //grid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //grid.Columns["IsNonStandard"].MinimumWidth = 55;
            //grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["ClientName"].MinimumWidth = 185;
            //grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Notes"].MinimumWidth = 185;
            //grid.Columns["GroupNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["GroupNumber"].Width = 85;
            grid.Columns["check"].ReadOnly = false;

            //grid.AutoGenerateColumns = false;

            //int DisplayIndex = 0;
            //grid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            //grid.Columns["GroupNumber"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            //grid.Columns["IsNonStandard"].DisplayIndex = DisplayIndex++;
            //grid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        private void btnPrintTafel_Click(object sender, EventArgs e)
        {
            bPrint = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void dgvFrontOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {

        }

    }
}
