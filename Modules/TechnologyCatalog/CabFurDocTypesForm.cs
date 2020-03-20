using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CabFurDocTypesForm : Form
    {
        int CabFurDocTypeID;
        int MachinesOperationID;
        public bool Ok = false;
        //bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        public CabFurDocTypesForm(int tCabFurDocTypeID, int tMachinesOperationID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            //IsNew = false;

            TechStoreManager = tTechStoreManager;
            Initialize();

            TechStoreManager.SaveCabFurDocTypes();
            CabFurDocTypeID = tCabFurDocTypeID;
            MachinesOperationID = tMachinesOperationID;
        }

        private void Initialize()
        {
            dgvDocTypes.DataSource = TechStoreManager.CabFurnitureDocumentTypesList;

            dgvDocTypes.Columns["CabFurDocTypeID"].Visible = false;

            int DisplayIndex = 0;
            dgvDocTypes.Columns["DocName"].DisplayIndex = DisplayIndex++;
            //dgvDocTypes.Columns["SaveColumn"].DisplayIndex = DisplayIndex++;
            //dgvDocTypes.Columns["DeleteColumn"].DisplayIndex = DisplayIndex++;

        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            TechStoreManager.EditCabFurDocType(MachinesOperationID, Convert.ToInt32(dgvDocTypes.SelectedRows[0].Cells["CabFurDocTypeID"].Value));
            TechStoreManager.UpdateMachinesOperations();
            TechStoreManager.RefreshMachinesOperations();
            TechStoreManager.MoveToMachinesOperation(MachinesOperationID);
            CabFurDocTypeID = Convert.ToInt32(dgvDocTypes.SelectedRows[0].Cells["CabFurDocTypeID"].Value);
        }

        private void CancelDateButton_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void dgvDocTypes_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            //if (e.RowIndex < 0)
            //    return;
            //PercentageDataGrid grid = (PercentageDataGrid)sender;
            //if (grid.Rows[e.RowIndex].IsNewRow)
            //    return;
            //int iCabFurDocTypeID = 0;
            //if (grid.Rows[e.RowIndex].Cells["CabFurDocTypeID"].Value != DBNull.Value)
            //    iCabFurDocTypeID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["CabFurDocTypeID"].Value);

            //if (iCabFurDocTypeID == CabFurDocTypeID && iCabFurDocTypeID != 1)
            //{
            //    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Green;
            //}
            //else
            //{
            //    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            //    grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            //}
        }

        private void dgvDocTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            var d = senderGrid.Columns[e.ColumnIndex].GetType();
            if (senderGrid.Columns[e.ColumnIndex] is ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn &&
                e.RowIndex >= 1)
            {
                int CabFurDocTypeID = 0;
                if (senderGrid.Columns[e.ColumnIndex].Name == "DeleteColumn")
                {
                    if (senderGrid.Rows.Count > 0 && senderGrid.Rows[e.RowIndex].Cells["CabFurDocTypeID"].Value != DBNull.Value)
                        CabFurDocTypeID = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells["CabFurDocTypeID"].Value);
                    TechStoreManager.DeleteCabFurDocType(CabFurDocTypeID);
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "SaveColumn")
                    TechStoreManager.SaveCabFurDocTypes();
            }
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            int CabFurDocTypeID = 0;
            if (dgvDocTypes.SelectedRows.Count > 0 && dgvDocTypes.SelectedRows[0].Cells["CabFurDocTypeID"].Value != DBNull.Value)
                CabFurDocTypeID = Convert.ToInt32(dgvDocTypes.SelectedRows[0].Cells["CabFurDocTypeID"].Value);
            TechStoreManager.DeleteCabFurDocType(CabFurDocTypeID);
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            TechStoreManager.SaveCabFurDocTypes();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            TechStoreManager.EditCabFurDocType(MachinesOperationID, 1);
            TechStoreManager.UpdateMachinesOperations();
            TechStoreManager.RefreshMachinesOperations();
            TechStoreManager.MoveToMachinesOperation(MachinesOperationID);
            CabFurDocTypeID = 1;
        }
    }
}
