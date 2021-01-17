using Infinium.Modules.CabFurnitureAssignments;

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CabFurDispatchForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        bool CanAction = true;

        int ClientID = 1;
        int OrderNumber = 1;

        Form TopForm;
        Form MainForm;
        public Modules.Dispatch.ZOVDispatchCheckLabel CheckLabel;
        DispatchPackagesManager dispatchPackagesManager;
        AssignmentsManager assignmentsManager;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public CabFurDispatchForm(Form tMainForm, AssignmentsManager tAssignmentsManager, int iClientID, int iOrderNumber)
        {
            InitializeComponent();

            ClientID = iClientID;
            OrderNumber = iOrderNumber;

            MainForm = tMainForm;
            assignmentsManager = tAssignmentsManager;
            Initialize();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            while (!SplashForm.bCreated) ;
        }

        private void CabFurDispatchForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            panel3.BringToFront();

            //PhantomForm PhantomForm = new Infinium.PhantomForm();
            //PhantomForm.Show();

            //ReAutorizationForm ReAutorizationForm = new ReAutorizationForm(this);

            //TopForm = ReAutorizationForm;
            //ReAutorizationForm.ShowDialog();

            //PhantomForm.Close();

            //PhantomForm.Dispose();
            //bool PressOK = ReAutorizationForm.PressOK;
            //UserID = ReAutorizationForm.UserID;
            //ReAutorizationForm.Dispose();
            //TopForm = null;

            //if (PressOK)
            //{
            //    CheckLabel.UserID = UserID;
            //    CanAction = true;
            //}
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

        private void Initialize()
        {
            dispatchPackagesManager = new DispatchPackagesManager();

            dgvStoragePackagesLabels.DataSource = dispatchPackagesManager.PackageLabelsBS;
            dgvStoragePackagesDetails.DataSource = dispatchPackagesManager.PackageDetailsBS;
            dgvScanedStoragePackagesDetails.DataSource = dispatchPackagesManager.ScanedPackageDetailsBS;

            dgvPackagesLabelsSetting(ref dgvStoragePackagesLabels);
            dgvPackagesDetailsSetting(ref dgvStoragePackagesDetails);
            dgvPackagesDetailsSetting(ref dgvScanedStoragePackagesDetails);
            dispatchPackagesManager.Clear();
            dispatchPackagesManager.GetPackagesLabels(ClientID, OrderNumber);
        }

        private void dgvPackagesLabelsSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("CabFurnitureComplementID"))
                grid.Columns["CabFurnitureComplementID"].Visible = false;
            if (grid.Columns.Contains("TechCatalogOperationsDetailID"))
                grid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            if (grid.Columns.Contains("TechStoreSubGroupID"))
                grid.Columns["TechStoreSubGroupID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;
            if (grid.Columns.Contains("PatinaID"))
                grid.Columns["PatinaID"].Visible = false;
            if (grid.Columns.Contains("InsetColorID"))
                grid.Columns["InsetColorID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("Notes"))
                grid.Columns["Notes"].Visible = false;
            if (grid.Columns.Contains("Scan"))
                grid.Columns["Scan"].Visible = false;

            if (grid.Columns.Contains("PackNumber"))
                grid.Columns["PackNumber"].HeaderText = "№ упаковки";
            if (grid.Columns.Contains("Index"))
                grid.Columns["Index"].HeaderText = "№ п/п";

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            int DisplayIndex = 0;
            if (grid.Columns.Contains("Index"))
                grid.Columns["Index"].DisplayIndex = DisplayIndex++;
            if (grid.Columns.Contains("PackNumber"))
                grid.Columns["PackNumber"].DisplayIndex = DisplayIndex++;
        }

        private void dgvPackagesDetailsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(assignmentsManager.CTechStoreNameColumn);
            grid.Columns.Add(assignmentsManager.TechStoreNameColumn);
            grid.Columns.Add(assignmentsManager.CoverColumn);
            grid.Columns.Add(assignmentsManager.PatinaColumn);
            grid.Columns.Add(assignmentsManager.InsetColorColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("TechStoreSubGroupID"))
                grid.Columns["TechStoreSubGroupID"].Visible = false;
            if (grid.Columns.Contains("CTechStoreID"))
                grid.Columns["CTechStoreID"].Visible = false;
            if (grid.Columns.Contains("CoverID"))
                grid.Columns["CoverID"].Visible = false;
            if (grid.Columns.Contains("PatinaID"))
                grid.Columns["PatinaID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID"))
                grid.Columns["TechStoreID"].Visible = false;
            if (grid.Columns.Contains("InsetColorID"))
                grid.Columns["InsetColorID"].Visible = false;
            if (grid.Columns.Contains("CabFurniturePackageDetailID"))
                grid.Columns["CabFurniturePackageDetailID"].Visible = false;
            if (grid.Columns.Contains("CabFurniturePackageID"))
                grid.Columns["CabFurniturePackageID"].Visible = false;
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("PackNumber"))
                grid.Columns["PackNumber"].Visible = false;
            if (grid.Columns.Contains("CNotes"))
                grid.Columns["CNotes"].Visible = false;
            if (grid.Columns.Contains("PackagesCount"))
                grid.Columns["PackagesCount"].Visible = false;

            grid.Columns["CreateDateTime"].HeaderText = "Дата создания";
            grid.Columns["PackNumber"].HeaderText = "№ упак.";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Length"].HeaderText = "Длина, мм";
            grid.Columns["Height"].HeaderText = "Высота, мм";
            grid.Columns["Width"].HeaderText = "Ширина, мм";
            grid.Columns["Count"].HeaderText = "Кол-во";

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            int DisplayIndex = 0;
            grid.Columns["CTechStoreNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoverColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetColorColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechStoreNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
        }

        private void BarcodeTextBox_Leave(object sender, EventArgs e)
        {
            CheckTimer.Enabled = true;
        }

        private void NavigateMenuCloseButton_MouseUp(object sender, MouseEventArgs e)
        {
            BarcodeTextBox.Focus();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!CanAction)
                return;

            if (e.KeyCode == Keys.Enter)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                BarcodeLabel.Text = "";
                CheckPicture.Visible = false;

                BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);

                if (BarcodeTextBox.Text.Length < 12)
                {
                    BarcodeTextBox.Clear();
                    BarcodeLabel.Text = "Неверный штрихкод";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }

                string Prefix = BarcodeTextBox.Text.Substring(0, 3);

                if (Prefix != "021")
                {
                    BarcodeTextBox.Clear();
                    BarcodeLabel.Text = "Это не штрихкод упаковки";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }

                int CabFurniturePackageID = Convert.ToInt32(BarcodeTextBox.Text.Substring(3, 9));
                
                if (dispatchPackagesManager.IsPackageScan(CabFurniturePackageID))
                {
                    CabFurniturePackageID = -1;
                    BarcodeTextBox.Clear();
                    BarcodeLabel.Text = "Уже отсканировано";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }
                if (!dispatchPackagesManager.IsPackageExist(CabFurniturePackageID))
                {
                    CabFurniturePackageID = -1;
                    BarcodeTextBox.Clear();
                    BarcodeLabel.Text = "Упаковки не существует";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }

                BarcodeLabel.Text = BarcodeTextBox.Text;
                BarcodeTextBox.Clear();

                if (dispatchPackagesManager.ScanPackage(CabFurniturePackageID))
                {
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.OK;
                    BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                }
                else
                {
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);

                    BarcodeLabel.Text = "Упаковки не соответствует заказу";
                }
                lbScaned.Text = dispatchPackagesManager.ScanedPackages;
                lbRackName.Text = dispatchPackagesManager.RackName;
                lbCellName.Text = dispatchPackagesManager.CellName;
            }

        }

        private void BarcodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CanAction)
                return;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                if (BarcodeTextBox.Text.Length >= 12 && e.KeyChar != (char)8)
                    e.Handled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
        }

        private void OKInvButton_Click(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
            CheckLabel.GetNotScanedPackages();

            int AllPackagesInDispatchCount = CheckLabel.AllPackagesInDispatchCount();
            int AllScanedPackagesCount = CheckLabel.ScanedPackagesCount + CheckLabel.WrongPackagesCount;
            int NotScanedPackagesCount = CheckLabel.NotScanedPackagesCount;
            int WrongPackagesCount = CheckLabel.WrongPackagesCount;
            label11.Text = AllScanedPackagesCount + " шт.";
            label15.Text = AllPackagesInDispatchCount + " шт.";
            if (NotScanedPackagesCount > 0)
            {
                cbtnNotScanedPackages.Visible = true;
                panel11.Visible = true;
            }
            else
            {
                cbtnNotScanedPackages.Visible = false;
                panel11.Visible = false;
            }
            if (WrongPackagesCount > 0)
            {
                cbtnWrongPackages.Visible = true;
                panel11.Visible = true;
                panel9.Visible = true;
                label13.Text = WrongPackagesCount + " шт.";
            }
            else
            {
                cbtnWrongPackages.Visible = false;
                panel11.Visible = false;
                panel9.Visible = false;
            }

            panel5.BringToFront();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            CheckLabel.DispatchPackages();
            CheckLabel.DispatchTrays();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvCheckPackages_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //PercentageDataGrid grid = (PercentageDataGrid)sender;
            //bool bNeedPaint = CheckLabel.IsCorrectPackage(Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PackageID"].Value));

            //if (bNeedPaint)
            //{
            //    int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
            //    Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
            //        grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvScanPackages.HorizontalScrollingOffset + 1, e.RowBounds.Height);

            //    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            //}
            //else
            //{
            //    int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
            //    Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
            //        grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvScanPackages.HorizontalScrollingOffset + 1, e.RowBounds.Height);

            //    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            //}
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (CheckLabel == null)
                return;
            if (kryptonCheckSet1.CheckedButton == cbtnScanedPackages)
            {
                panel7.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnNotScanedPackages)
            {
                panel6.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnWrongPackages)
            {
                panel11.BringToFront();
            }
        }

        private void dgvPackagesLabels_SelectionChanged(object sender, EventArgs e)
        {
            if (dispatchPackagesManager == null)
                return;
            int cabFurniturePackageID = 0;
            if (dgvStoragePackagesLabels.SelectedRows.Count != 0 && dgvStoragePackagesLabels.SelectedRows[0].Cells["CabFurnitureComplementID"].Value != DBNull.Value)
                cabFurniturePackageID = Convert.ToInt32(dgvStoragePackagesLabels.SelectedRows[0].Cells["CabFurnitureComplementID"].Value);
            dispatchPackagesManager.FilterPackagesDetails(cabFurniturePackageID);
        }
    }
}
