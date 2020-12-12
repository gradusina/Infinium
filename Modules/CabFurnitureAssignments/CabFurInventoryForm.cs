using Infinium.Modules.CabFurnitureAssignments;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CabFurInventoryForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int workshopId = -1;

        bool bb = true;
        int FormEvent = 0;

        Form MainForm = null;

        AssignmentsManager assignmentsStoreManager;
        StoragePackagesManager storagePackagesManager;

        public CabFurInventoryForm(Form tMainForm, AssignmentsManager AM, StoragePackagesManager SM, int iworkshopId)
        {
            MainForm = tMainForm;
            assignmentsStoreManager = AM;
            storagePackagesManager = SM;
            workshopId = iworkshopId;

            InitializeComponent();

            Initialize();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
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

        private void Initialize()
        {
            dgvStoragePackagesLabels.DataSource = storagePackagesManager.InvPackageLabelsBS;
            dgvStoragePackagesDetails.DataSource = storagePackagesManager.InvPackageDetailsBS;
            dgvStoragePackagesLabelsSetting(ref dgvStoragePackagesLabels);
            dgvPackagesDetailsSetting(ref dgvStoragePackagesDetails);
            storagePackagesManager.ClearInvTables();
        }

        private void dgvStoragePackagesLabelsSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("PackagesCount"))
                grid.Columns["PackagesCount"].Visible = false;
            if (grid.Columns.Contains("TechStoreSubGroupID"))
                grid.Columns["TechStoreSubGroupID"].Visible = false;
            if (grid.Columns.Contains("CabFurAssignmentDetailID"))
                grid.Columns["CabFurAssignmentDetailID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("QualityControlInUserID"))
                grid.Columns["QualityControlInUserID"].Visible = false;
            if (grid.Columns.Contains("QualityControlOutUserID"))
                grid.Columns["QualityControlOutUserID"].Visible = false;
            if (grid.Columns.Contains("CellID"))
                grid.Columns["CellID"].Visible = false;
            if (grid.Columns.Contains("WorkshopID"))
                grid.Columns["WorkshopID"].Visible = false;

            grid.Columns["CabFurniturePackageID"].HeaderText = "ID";
            grid.Columns["PackNumber"].HeaderText = "№ упаковки";
            grid.Columns["AddToStorageDateTime"].HeaderText = "Принято на склад";
            grid.Columns["RemoveFromStorageDateTime"].HeaderText = "Списано со склада";
            grid.Columns["QualityControlInDateTime"].HeaderText = "Отправлено на ОТК";
            grid.Columns["QualityControlOutDateTime"].HeaderText = "Принято с ОТК";
            grid.Columns["QualityControl"].HeaderText = "ОТК";
            grid.Columns["WorkshopName"].HeaderText = "Цех";
            grid.Columns["RackName"].HeaderText = "Стеллаж";
            grid.Columns["CellName"].HeaderText = "Ячейка";

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            int DisplayIndex = 0;
            grid.Columns["CabFurniturePackageID"].DisplayIndex = DisplayIndex++;
            grid.Columns["WorkshopName"].DisplayIndex = DisplayIndex++;
            grid.Columns["RackName"].DisplayIndex = DisplayIndex++;
            grid.Columns["CellName"].DisplayIndex = DisplayIndex++;
            grid.Columns["AddToStorageDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["RemoveFromStorageDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["QualityControl"].DisplayIndex = DisplayIndex++;
            grid.Columns["QualityControlInDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["QualityControlOutDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["PackNumber"].DisplayIndex = DisplayIndex++;

            grid.Columns["CabFurniturePackageID"].Width = 50;
        }

        private void dgvPackagesDetailsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(assignmentsStoreManager.CTechStoreNameColumn);
            grid.Columns.Add(assignmentsStoreManager.TechStoreNameColumn);
            grid.Columns.Add(assignmentsStoreManager.CoverColumn);
            grid.Columns.Add(assignmentsStoreManager.PatinaColumn);
            grid.Columns.Add(assignmentsStoreManager.InsetColorColumn);
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

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
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

                if (!storagePackagesManager.IsPackageExist(CabFurniturePackageID))
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

                if (storagePackagesManager.GetInvPackagesLabels(CabFurniturePackageID))
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

                    BarcodeLabel.Text = "Упаковки не существует";
                }
            }

        }

        private void BarcodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void BindPackagesToCellForm_Shown(object sender, EventArgs e)
        {

        }

        private void dgvCovers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            if (!bb)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            int TechDecorID = -1;
            string StoreName = string.Empty;
            if (grid.Rows[e.RowIndex].Cells["TechStoreID"].Value != DBNull.Value)
            {
                TechDecorID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["TechStoreID"].Value);
                //StoreName = AssignmentsManager.StoreName(TechDecorID);
                cell.ToolTipText = StoreName;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStoragePackagesLabels.Rows.Count == 0)
                return;

            bb = false;
            int Index = dgvStoragePackagesLabels.CurrentRow.Index;
            dgvStoragePackagesLabels.Rows.RemoveAt(Index);
            Index--;

            if (Index == -1)
                Index = 0;

            if (dgvStoragePackagesLabels.Rows.Count == 0)
                return;
            dgvStoragePackagesLabels.Rows[Index].Selected = true;
            bb = false;
        }

        private void dgvStoragePackagesLabels_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int WorkshopID = -1;
            if (grid.Rows[e.RowIndex].Cells["WorkshopID"].Value != DBNull.Value)
                WorkshopID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["WorkshopID"].Value);

            if (WorkshopID == -1)//упаковка еще не привязана к ячейке
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Blue;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Blue;
            }
            if (WorkshopID != -1 && WorkshopID != workshopId)//упаковка привязана к ячейку из другого цеха
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }

        private void dgvStoragePackagesLabels_SelectionChanged(object sender, EventArgs e)
        {
            if (storagePackagesManager == null)
                return;
            int cabFurniturePackageID = 0;
            if (dgvStoragePackagesLabels.SelectedRows.Count != 0 && dgvStoragePackagesLabels.SelectedRows[0].Cells["CabFurniturePackageID"].Value != DBNull.Value)
                cabFurniturePackageID = Convert.ToInt32(dgvStoragePackagesLabels.SelectedRows[0].Cells["CabFurniturePackageID"].Value);
            storagePackagesManager.FilterInvPackagesDetails(cabFurniturePackageID);
        }

        private void btnStopScan_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void btnBackToScan_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }
    }
}
