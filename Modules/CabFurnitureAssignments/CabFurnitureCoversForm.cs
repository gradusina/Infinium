using Infinium.Modules.CabFurnitureAssignments;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CabFurnitureCoversForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool bb = true;
        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        AssignmentsManager AssignmentsManager;
        CoversManager CoversManager;

        public CabFurnitureCoversForm(Form tMainForm, AssignmentsManager tAssignmentsManager)
        {
            MainForm = tMainForm;
            AssignmentsManager = tAssignmentsManager;
            InitializeComponent();
            if (CoversManager == null)
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
            CoversManager = new CoversManager();
            dgvCovers.DataSource = CoversManager.CoversBS;
            dgvCovers.Columns.Add(AssignmentsManager.TechStoreNameColumn);
            dgvCovers.Columns.Add(AssignmentsManager.InsetColorColumn);
            dgvCovers.Columns.Add(AssignmentsManager.CoverColumn1);
            dgvCovers.Columns.Add(AssignmentsManager.CoverColumn2);
            dgvCovers.Columns.Add(AssignmentsManager.PatinaColumn1);
            dgvCovers.Columns.Add(AssignmentsManager.PatinaColumn2);
            dgvCovers.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in dgvCovers.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dgvCovers.Columns.Contains("TechStoreSubGroupName"))
                dgvCovers.Columns["TechStoreSubGroupName"].Visible = false;
            if (dgvCovers.Columns.Contains("CreationUserID"))
                dgvCovers.Columns["CreationUserID"].Visible = false;
            if (dgvCovers.Columns.Contains("CreationDateTime"))
                dgvCovers.Columns["CreationDateTime"].Visible = false;
            if (dgvCovers.Columns.Contains("CabFurnitureCoverID"))
                dgvCovers.Columns["CabFurnitureCoverID"].Visible = false;
            if (dgvCovers.Columns.Contains("TechStoreID"))
                dgvCovers.Columns["TechStoreID"].Visible = false;
            if (dgvCovers.Columns.Contains("CoverID1"))
                dgvCovers.Columns["CoverID1"].Visible = false;
            if (dgvCovers.Columns.Contains("CoverID2"))
                dgvCovers.Columns["CoverID2"].Visible = false;
            if (dgvCovers.Columns.Contains("PatinaID1"))
                dgvCovers.Columns["PatinaID1"].Visible = false;
            if (dgvCovers.Columns.Contains("PatinaID2"))
                dgvCovers.Columns["PatinaID2"].Visible = false;
            if (dgvCovers.Columns.Contains("InsetColorID"))
                dgvCovers.Columns["InsetColorID"].Visible = false;

            int DisplayIndex = 0;
            dgvCovers.Columns["CoverColumn1"].DisplayIndex = DisplayIndex++;
            dgvCovers.Columns["PatinaColumn1"].DisplayIndex = DisplayIndex++;
            dgvCovers.Columns["InsetColorColumn"].DisplayIndex = DisplayIndex++;
            dgvCovers.Columns["TechStoreNameColumn"].DisplayIndex = DisplayIndex++;
            dgvCovers.Columns["CoverColumn2"].DisplayIndex = DisplayIndex++;
            dgvCovers.Columns["PatinaColumn2"].DisplayIndex = DisplayIndex++;

            cbTechStore.DataSource = AssignmentsManager.BasicTechStoreBS;
            cbTechStore.DisplayMember = "TechStoreName";
            cbTechStore.ValueMember = "TechStoreID";
            cbTechStoreSubGroups.DataSource = AssignmentsManager.BasicTechStoreSubGroupsBS;
            cbTechStoreSubGroups.DisplayMember = "TechStoreSubGroupName";
            cbTechStoreSubGroups.ValueMember = "TechStoreSubGroupID";
            cbTechStoreGroups.DataSource = AssignmentsManager.BasicTechStoreGroupsBS;
            cbTechStoreGroups.DisplayMember = "TechStoreGroupName";
            cbTechStoreGroups.ValueMember = "TechStoreGroupID";
            cbCovers1.DataSource = AssignmentsManager.BasicCoversBS1;
            cbCovers1.DisplayMember = "CoverName";
            cbCovers1.ValueMember = "CoverID";
            cbCovers2.DataSource = AssignmentsManager.BasicCoversBS2;
            cbCovers2.DisplayMember = "CoverName";
            cbCovers2.ValueMember = "CoverID";
            cbPatina1.DataSource = AssignmentsManager.BasicPatinaBS1;
            cbPatina1.DisplayMember = "PatinaName";
            cbPatina1.ValueMember = "PatinaID";
            cbPatina2.DataSource = AssignmentsManager.BasicPatinaBS2;
            cbPatina2.DisplayMember = "PatinaName";
            cbPatina2.ValueMember = "PatinaID";

            cbInsetColors.DataSource = AssignmentsManager.BasicInsetColorsBS;
            cbInsetColors.DisplayMember = "InsetColorName";
            cbInsetColors.ValueMember = "InsetColorID";
        }

        private void btnAddCover_Click(object sender, EventArgs e)
        {
            int CoverID1 = 0;
            int CoverID2 = 0;
            int TechStoreID = 0;
            int TechStoreSubGroupID = 0;
            int InsetColorID = 0;
            int PatinaID1 = 0;
            int PatinaID2 = 0;
            if (cbInsetColors.SelectedItem != null && ((DataRowView)cbInsetColors.SelectedItem).Row["InsetColorID"] != DBNull.Value)
                InsetColorID = Convert.ToInt32(((DataRowView)cbInsetColors.SelectedItem).Row["InsetColorID"]);
            if (cbCovers1.SelectedItem != null && ((DataRowView)cbCovers1.SelectedItem).Row["CoverID"] != DBNull.Value)
                CoverID1 = Convert.ToInt32(((DataRowView)cbCovers1.SelectedItem).Row["CoverID"]);
            if (cbCovers2.SelectedItem != null && ((DataRowView)cbCovers2.SelectedItem).Row["CoverID"] != DBNull.Value)
                CoverID2 = Convert.ToInt32(((DataRowView)cbCovers2.SelectedItem).Row["CoverID"]);
            if (cbTechStore.SelectedItem != null && ((DataRowView)cbTechStore.SelectedItem).Row["TechStoreID"] != DBNull.Value)
                TechStoreID = Convert.ToInt32(((DataRowView)cbTechStore.SelectedItem).Row["TechStoreID"]);
            if (cbTechStoreSubGroups.SelectedItem != null && ((DataRowView)cbTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"] != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(((DataRowView)cbTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"]);
            if (cbPatina1.SelectedItem != null && ((DataRowView)cbPatina1.SelectedItem).Row["PatinaID"] != DBNull.Value)
                PatinaID1 = Convert.ToInt32(((DataRowView)cbPatina1.SelectedItem).Row["PatinaID"]);
            if (cbPatina2.SelectedItem != null && ((DataRowView)cbPatina2.SelectedItem).Row["PatinaID"] != DBNull.Value)
                PatinaID2 = Convert.ToInt32(((DataRowView)cbPatina2.SelectedItem).Row["PatinaID"]);

            if (cbAddSubGroup.Checked)
            {
                CoversManager.AddCovers(CoverID1, PatinaID1, InsetColorID, TechStoreSubGroupID, CoverID2, PatinaID2);
                CoversManager.MoveToLast();
            }
            else
            {
                if (!CoversManager.IsCoverExist(CoverID1, PatinaID1, InsetColorID, TechStoreID, CoverID2, PatinaID2))
                {
                    CoversManager.AddCover(CoverID1, PatinaID1, InsetColorID, TechStoreID, CoverID2, PatinaID2);
                    CoversManager.MoveToLast();
                }
                else
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Такая позиция уже добавлена", "Добавление");
                }
            }
        }

        private void btnDeleteCover_Click(object sender, EventArgs e)
        {
            bb = false;
            if (dgvCovers.SelectedRows.Count != 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Удалить позицию?",
                        "Удаление");

                if (OKCancel)
                {
                    if (dgvCovers.SelectedRows[0].Cells["CabFurnitureCoverID"].Value == DBNull.Value)
                    {
                        CoversManager.DeleteCover();
                    }
                    else
                    {
                        int[] CabFurnitureCoverID = new int[dgvCovers.SelectedRows.Count];
                        for (int i = 0; i < dgvCovers.SelectedRows.Count; i++)
                        {
                            if (dgvCovers.SelectedRows[i].Cells["CabFurnitureCoverID"].Value != DBNull.Value)
                                CabFurnitureCoverID[i] = Convert.ToInt32(dgvCovers.SelectedRows[i].Cells["CabFurnitureCoverID"].Value);
                        }
                        CoversManager.DeleteCovers(CabFurnitureCoverID);
                    }
                }
            }
            bb = true;
        }

        private void btnSaveCovers_Click(object sender, EventArgs e)
        {
            bb = false;
            CoversManager.UpdateCovers();
            CoversManager.RefreshCovers();
            bb = true;
        }

        private void cbTechStoreGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AssignmentsManager == null)
                return;
            int TechStoreGroupID = 0;
            if (cbTechStoreGroups.SelectedItem != null && ((DataRowView)cbTechStoreGroups.SelectedItem).Row["TechStoreGroupID"] != DBNull.Value)
                TechStoreGroupID = Convert.ToInt32(((DataRowView)cbTechStoreGroups.SelectedItem).Row["TechStoreGroupID"]);
            AssignmentsManager.FilterBasicTechStoreSubGroups(TechStoreGroupID);
            cbTechStoreSubGroups_SelectedIndexChanged(null, null);
        }

        private void cbTechStoreSubGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AssignmentsManager == null)
                return;
            int TechStoreSubGroupID = 0;
            if (cbTechStoreSubGroups.SelectedItem != null && ((DataRowView)cbTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"] != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(((DataRowView)cbTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"]);
            AssignmentsManager.FilterBasicTechStore(TechStoreSubGroupID);
        }

        private void CabFurnitureCoversForm_Shown(object sender, EventArgs e)
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
                StoreName = AssignmentsManager.StoreName(TechDecorID);
                cell.ToolTipText = StoreName;
            }
        }

        private void cbAddSubGroup_CheckedChanged(object sender, EventArgs e)
        {
            cbTechStore.Enabled = !cbAddSubGroup.Checked;
        }
    }
}
