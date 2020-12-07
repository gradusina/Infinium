using Infinium.Modules.CabFurnitureAssignments;

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class BindPackagesToCellForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int cellId = -1;

        bool bb = true;
        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        StoragePackagesManager storagePackagesManager;

        public BindPackagesToCellForm(Form tMainForm, StoragePackagesManager SM, int iCellId)
        {
            MainForm = tMainForm;
            storagePackagesManager = SM;
            cellId = iCellId;
            InitializeComponent();

            Initialize();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (dgvPackages.Rows.Count == 0 || cellId == -1)
                return;

            int[] packageID = new int[dgvPackages.Rows.Count];

            for (int i = 0; i < dgvPackages.Rows.Count; i++)
            {
                packageID[i] = Convert.ToInt32(dgvPackages.Rows[i].Cells["CabFurniturePackageID"].Value);
            }
            storagePackagesManager.BindPackagesToCell(cellId, packageID);
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
            storagePackagesManager.Clear();
            dgvPackages.DataSource = storagePackagesManager.BindPackageLabelsBS;

            dgvPackages.AutoGenerateColumns = false;

            if (dgvPackages.Columns.Contains("PackagesCount"))
                dgvPackages.Columns["PackagesCount"].Visible = false;
            if (dgvPackages.Columns.Contains("TechStoreSubGroupID"))
                dgvPackages.Columns["TechStoreSubGroupID"].Visible = false;
            if (dgvPackages.Columns.Contains("CabFurAssignmentDetailID"))
                dgvPackages.Columns["CabFurAssignmentDetailID"].Visible = false;
            if (dgvPackages.Columns.Contains("MainOrderID"))
                dgvPackages.Columns["MainOrderID"].Visible = false;
            if (dgvPackages.Columns.Contains("QualityControlInUserID"))
                dgvPackages.Columns["QualityControlInUserID"].Visible = false;
            if (dgvPackages.Columns.Contains("QualityControlOutUserID"))
                dgvPackages.Columns["QualityControlOutUserID"].Visible = false;
            if (dgvPackages.Columns.Contains("CellID"))
                dgvPackages.Columns["CellID"].Visible = false;
            if (dgvPackages.Columns.Contains("PackNumber"))
                dgvPackages.Columns["PackNumber"].Visible = false;

            dgvPackages.Columns["CabFurniturePackageID"].HeaderText = "ID";
            dgvPackages.Columns["AddToStorageDateTime"].HeaderText = "Принято на склад";
            dgvPackages.Columns["RemoveFromStorageDateTime"].HeaderText = "Списано со склада";
            dgvPackages.Columns["Name"].HeaderText = "Ячейка";
            dgvPackages.Columns["QualityControlInDateTime"].HeaderText = "Отправлено на ОТК";
            dgvPackages.Columns["QualityControlOutDateTime"].HeaderText = "Принято с ОТК";
            dgvPackages.Columns["QualityControl"].HeaderText = "ОТК";

            foreach (DataGridViewColumn Column in dgvPackages.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            int DisplayIndex = 0;
            dgvPackages.Columns["CabFurniturePackageID"].DisplayIndex = DisplayIndex++;
            dgvPackages.Columns["AddToStorageDateTime"].DisplayIndex = DisplayIndex++;
            dgvPackages.Columns["RemoveFromStorageDateTime"].DisplayIndex = DisplayIndex++;
            dgvPackages.Columns["Name"].DisplayIndex = DisplayIndex++;
            dgvPackages.Columns["QualityControl"].DisplayIndex = DisplayIndex++;
            dgvPackages.Columns["QualityControlInDateTime"].DisplayIndex = DisplayIndex++;
            dgvPackages.Columns["QualityControlOutDateTime"].DisplayIndex = DisplayIndex++;

            dgvPackages.Columns["CabFurniturePackageID"].Width = 50;
        }
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
        }

        private void ClearControls()
        {

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

                ClearControls();

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

                if (storagePackagesManager.GetPackagesLabels(CabFurniturePackageID))
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
                    ClearControls();
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
            if (dgvPackages.Rows.Count == 0)
                return;

            bb = false;
            int Index = dgvPackages.CurrentRow.Index;
            dgvPackages.Rows.RemoveAt(Index);
            Index--;

            if (Index == -1)
                Index = 0;

            if (dgvPackages.Rows.Count == 0)
                return;
            dgvPackages.Rows[Index].Selected = true;
            bb = false;
        }

        private void dgvPackages_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int CellID = -1;
            if (grid.Rows[e.RowIndex].Cells["CellID"].Value != DBNull.Value)
                CellID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["CellID"].Value);

            if (CellID != -1)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
        }
    }
}
