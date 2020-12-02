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

        bool bb = true;
        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        StoragePackagesManager storagePackagesManager;

        public BindPackagesToCellForm(Form tMainForm, StoragePackagesManager SM)
        {
            MainForm = tMainForm;
            storagePackagesManager = SM;
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

            storagePackagesManager.Clear();
            dgvPackages.DataSource = storagePackagesManager.BindPackageLabelsBS;
            //dgvPackages.AutoGenerateColumns = false;

            //foreach (DataGridViewColumn Column in dgvPackages.Columns)
            //{
            //    Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //}

            //if (dgvPackages.Columns.Contains("TechStoreSubGroupName"))
            //    dgvPackages.Columns["TechStoreSubGroupName"].Visible = false;
            //if (dgvPackages.Columns.Contains("CreationUserID"))
            //    dgvPackages.Columns["CreationUserID"].Visible = false;
            //if (dgvPackages.Columns.Contains("CreationDateTime"))
            //    dgvPackages.Columns["CreationDateTime"].Visible = false;
            //if (dgvPackages.Columns.Contains("CabFurnitureCoverID"))
            //    dgvPackages.Columns["CabFurnitureCoverID"].Visible = false;
            //if (dgvPackages.Columns.Contains("TechStoreID"))
            //    dgvPackages.Columns["TechStoreID"].Visible = false;
            //if (dgvPackages.Columns.Contains("CoverID1"))
            //    dgvPackages.Columns["CoverID1"].Visible = false;
            //if (dgvPackages.Columns.Contains("CoverID2"))
            //    dgvPackages.Columns["CoverID2"].Visible = false;
            //if (dgvPackages.Columns.Contains("PatinaID1"))
            //    dgvPackages.Columns["PatinaID1"].Visible = false;
            //if (dgvPackages.Columns.Contains("PatinaID2"))
            //    dgvPackages.Columns["PatinaID2"].Visible = false;
            //if (dgvPackages.Columns.Contains("InsetColorID"))
            //    dgvPackages.Columns["InsetColorID"].Visible = false;

            //int DisplayIndex = 0;
            //dgvPackages.Columns["CoverColumn1"].DisplayIndex = DisplayIndex++;
            //dgvPackages.Columns["PatinaColumn1"].DisplayIndex = DisplayIndex++;
            //dgvPackages.Columns["InsetColorColumn"].DisplayIndex = DisplayIndex++;
            //dgvPackages.Columns["TechStoreNameColumn"].DisplayIndex = DisplayIndex++;
            //dgvPackages.Columns["CoverColumn2"].DisplayIndex = DisplayIndex++;
            //dgvPackages.Columns["PatinaColumn2"].DisplayIndex = DisplayIndex++;
        }
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
        }

        private int GetChar(KeyEventArgs e)
        {
            int c = -1;

            if (e.KeyCode == Keys.NumPad1 || e.KeyCode == Keys.NumPad2 || e.KeyCode == Keys.NumPad3 || e.KeyCode == Keys.NumPad4 ||
                e.KeyCode == Keys.NumPad5 || e.KeyCode == Keys.NumPad6 || e.KeyCode == Keys.NumPad7 || e.KeyCode == Keys.NumPad8 ||
                e.KeyCode == Keys.NumPad9 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.D0 || e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 || e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 || e.KeyCode == Keys.D9 || e.KeyCode == Keys.D0)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad1:
                        { c = 1; }
                        break;
                    case Keys.NumPad2:
                        { c = 2; }
                        break;
                    case Keys.NumPad3:
                        { c = 3; }
                        break;
                    case Keys.NumPad4:
                        { c = 4; }
                        break;
                    case Keys.NumPad5:
                        { c = 5; }
                        break;
                    case Keys.NumPad6:
                        { c = 6; }
                        break;
                    case Keys.NumPad7:
                        { c = 7; }
                        break;
                    case Keys.NumPad8:
                        { c = 8; }
                        break;
                    case Keys.NumPad9:
                        { c = 9; }
                        break;
                    case Keys.NumPad0:
                        { c = 0; }
                        break;


                    case Keys.D1:
                        { c = 1; }
                        break;
                    case Keys.D2:
                        { c = 2; }
                        break;
                    case Keys.D3:
                        { c = 3; }
                        break;
                    case Keys.D4:
                        { c = 4; }
                        break;
                    case Keys.D5:
                        { c = 5; }
                        break;
                    case Keys.D6:
                        { c = 6; }
                        break;
                    case Keys.D7:
                        { c = 7; }
                        break;
                    case Keys.D8:
                        { c = 8; }
                        break;
                    case Keys.D9:
                        { c = 9; }
                        break;
                    case Keys.D0:
                        { c = 0; }
                        break;
                }


            }
            return c;
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
                    SetGridColor(false);

                    return;
                }

                string Prefix = BarcodeTextBox.Text.Substring(0, 3);

                if (Prefix != "022")
                {
                    BarcodeTextBox.Clear();
                    BarcodeLabel.Text = "Неверный штрихкод";
                    SetGridColor(false);

                    return;
                }

                BarcodeLabel.Text = BarcodeTextBox.Text;
                BarcodeTextBox.Clear();

                //упаковка
                if (Prefix == "022")
                {
                    int CabFurniturePackageID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                    if (storagePackagesManager.GetPackagesLabels(CabFurniturePackageID))
                    {
                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.OK;
                        BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);

                        //if (CheckLabel.lInfo.RemoveFromStorage)
                        //    BarcodeLabel.Text = "Списано со склада ранее";
                        SetGridColor(true);
                    }
                    else
                    {
                        CheckPicture.Visible = true;
                        CheckPicture.Image = Properties.Resources.cancel;
                        BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);

                        BarcodeLabel.Text = "Упаковки не существует";
                        SetGridColor(false);
                        ClearControls();
                    }
                }
            }

        }

        public void SetGridColor(bool IsAccept)
        {
            //if (IsAccept)
            //{
            //    dgvScan.StateCommon.Background.Color1 = Color.FromArgb(82, 169, 24);
            //    dgvScan.StateCommon.Background.Color2 = Color.Transparent;
            //    dgvScan.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
            //    dgvScan.StateCommon.DataCell.Back.Color1 = Color.FromArgb(82, 169, 24);
            //    dgvScan.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;
            //}
            //else
            //{
            //    dgvScan.StateCommon.Background.Color1 = Color.Red;
            //    dgvScan.StateCommon.Background.Color2 = Color.Transparent;
            //    dgvScan.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
            //    dgvScan.StateCommon.DataCell.Back.Color1 = Color.Red;
            //    dgvScan.StateCommon.DataCell.Content.Color1 = Color.White;
            //}
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
    }
}
