using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingDispatchLabelCheckForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        ArrayList DispatchIDs;
        int FormEvent = 0;

        bool CanAction = false;
        int UserID = 0;
        Form TopForm;
        Form MainForm;
        public Modules.Dispatch.MarketingDispatchCheckLabel CheckLabel;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public MarketingDispatchLabelCheckForm(Form tMainForm, ArrayList aDispatchIDs)
        {
            InitializeComponent();

            DispatchIDs = aDispatchIDs;
            MainForm = tMainForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void LabelCheckProfilForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            panel3.BringToFront();

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ReAutorizationForm ReAutorizationForm = new ReAutorizationForm(this);

            TopForm = ReAutorizationForm;
            ReAutorizationForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            bool PressOK = ReAutorizationForm.PressOK;
            UserID = ReAutorizationForm.UserID;
            ReAutorizationForm.Dispose();
            TopForm = null;

            if (PressOK)
            {
                CheckLabel.UserID = UserID;
                CanAction = true;
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
            CheckLabel = new Modules.Dispatch.MarketingDispatchCheckLabel(ref FrontsPackContentDataGrid, ref DecorPackContentDataGrid, ref PackagesDataGrid,
                ref dgvScanPackages, ref dgvNotScanPackages, ref dgvWrongPackages)
            {
                CurrentDispatchIDs = DispatchIDs
            };
            OrderDateLabel.Text = string.Empty;
            GroupLabel.Text = string.Empty;
            lblPackageCount.Text = string.Empty;
        }

        private void BarcodeTextBox_Leave(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
            CheckTimer.Enabled = true;
        }

        private void NavigateMenuCloseButton_MouseUp(object sender, MouseEventArgs e)
        {
            BarcodeTextBox.Focus();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!CanAction)
                return;
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!CanAction)
                return;
            if (e.KeyCode == Keys.Enter)
            {
                ErrorPackLabel.Visible = false;
                BarcodeLabel.Text = "";
                CheckPicture.Visible = false;

                CheckLabel.Clear();

                if (BarcodeTextBox.Text.Length < 12)
                {
                    BarcodeTextBox.Clear();
                    OrderDateLabel.Text = "";
                    GroupLabel.Text = "";
                    lblPackageCount.Text = string.Empty;
                    return;
                }

                BarcodeLabel.Text = BarcodeTextBox.Text;

                BarcodeTextBox.Clear();

                string Message = string.Empty;
                string Prefix = BarcodeLabel.Text.Substring(0, 3);

                if (CheckLabel.IsPackageLabel(BarcodeLabel.Text))
                {
                    int PackageID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));
                    if (CheckLabel.HasPackages(BarcodeLabel.Text))
                    {
                        CheckLabel.GetPackageInfo(PackageID);
                        CheckLabel.FillPackagesByPackage(BarcodeLabel.Text);
                        CheckLabel.AddPackageToTempTable();

                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.OK;
                        BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                        GroupLabel.Text = CheckLabel.LabelInfo.Group;
                        lblPackageCount.Text = CheckLabel.LabelInfo.PackageCount.ToString();
                        OrderDateLabel.Text = CheckLabel.LabelInfo.OrderDate;
                    }
                    else
                    {
                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        GroupLabel.Text = "";
                        lblPackageCount.Text = string.Empty;
                        OrderDateLabel.Text = "";
                    }
                }
                else
                {
                    if (CheckLabel.IsTrayLabel(BarcodeLabel.Text))
                    {
                        int TrayID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                        //CheckOrdersStatus.SetStatusZOV(TrayID, true);
                        //CheckOrdersStatus.SetStatusMarketingForTray(TrayID);
                        if (CheckLabel.HasTray(BarcodeLabel.Text))
                        {
                            CheckLabel.FillPackagesByTray(BarcodeLabel.Text);
                            CheckLabel.AddPackageToTempTable();
                            CheckLabel.AddTrayToTempTable(TrayID);

                            CheckPicture.Visible = true;
                            CheckPicture.Image = Properties.Resources.OK;
                            BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                            GroupLabel.Text = CheckLabel.LabelInfo.Group;
                            lblPackageCount.Text = CheckLabel.LabelInfo.PackageCount.ToString();
                            OrderDateLabel.Text = CheckLabel.LabelInfo.OrderDate;
                        }
                        else
                        {
                            CheckPicture.Visible = true;
                            CheckPicture.Image = Properties.Resources.cancel;
                            BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                            GroupLabel.Text = "";
                            lblPackageCount.Text = string.Empty;
                            OrderDateLabel.Text = "";
                        }
                    }
                    else
                    {
                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);
                        OrderDateLabel.Text = "";
                        lblPackageCount.Text = string.Empty;
                        GroupLabel.Text = "";
                        CheckLabel.Clear();
                        return;
                    }
                }
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

        private void PackagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (CheckLabel == null || CheckLabel.IsPackagesTableEmpty)
                return;

            if (PackagesDataGrid.SelectedRows.Count == 0)
                return;

            int PackageID = Convert.ToInt32(PackagesDataGrid.SelectedRows[0].Cells["PackageID"].Value);
            MainOrdersTabControl.TabPages[0].PageVisible = CheckLabel.FillFrontsPackContent(PackageID);
            MainOrdersTabControl.TabPages[1].PageVisible = CheckLabel.FillDecorPackContent(PackageID);
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
            CheckOrdersStatus.SetStatusMarketingForDispatch(DispatchIDs);

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

    }
}
