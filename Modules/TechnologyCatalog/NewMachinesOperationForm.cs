using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewMachinesOperationForm : Form
    {
        int MachineID;
        int MachinesOperationID;
        public bool Ok = false;
        bool Edit;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        BindingSource MachinesOperationsBS;

        public NewMachinesOperationForm(int tMachineID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            TechStoreManager = tTechStoreManager;

            MachineID = tMachineID;

            Initialize("MachineID IS NULL");

            Edit = false;
        }

        public NewMachinesOperationForm(int tMachineID, int tMachinesOperationID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            AddMachineButton.Visible = false;
            EditMachineButton.Visible = false;
            RemoveMachineButton.Visible = false;

            TechStoreManager = tTechStoreManager;

            MachineID = tMachineID;

            MachinesOperationID = tMachinesOperationID;

            Initialize("MachinesOperationID = " + MachinesOperationID);

            Edit = true;
        }

        private void Initialize(string Filter)
        {
            MachinesOperationsBS = new BindingSource()
            {
                DataSource = TechStoreManager.MachinesOperationsDT.Copy(),
                Filter = Filter
            };
            MachinesOperationsGrid.DataSource = MachinesOperationsBS;

            MeasureComboBox.DataSource = TechStoreManager.MeasuresDT;
            MeasureComboBox.ValueMember = "MeasureID";
            MeasureComboBox.DisplayMember = "Measure";

            cbPositions.DataSource = TechStoreManager.PositionsList;
            cbPositions.ValueMember = "PositionID";
            cbPositions.DisplayMember = "Position";

            cbPositions2.DataSource = TechStoreManager.PositionsList;
            cbPositions2.ValueMember = "PositionID";
            cbPositions2.DisplayMember = "Position";

            cbDocTypes.DataSource = TechStoreManager.DocTypesList;
            cbDocTypes.ValueMember = "CabFurDocTypeID";
            cbDocTypes.DisplayMember = "DocName";

            cbAlgorithms.DataSource = TechStoreManager.AlgorithmsList;
            cbAlgorithms.ValueMember = "CabFurAlgorithmID";
            cbAlgorithms.DisplayMember = "Algorithm";

            MachinesOperationsGrid.Columns["CabFurDocTypeID"].Visible = false;
            MachinesOperationsGrid.Columns["CabFurAlgorithmID"].Visible = false;
            MachinesOperationsGrid.Columns["PreparatoryNorm"].Visible = false;
            MachinesOperationsGrid.Columns["MachinesOperationID"].Visible = false;
            MachinesOperationsGrid.Columns["MachineID"].Visible = false;
            MachinesOperationsGrid.Columns["Norm"].Visible = false;
            MachinesOperationsGrid.Columns["MeasureID"].Visible = false;
            MachinesOperationsGrid.Columns["Article"].Visible = false;
            MachinesOperationsGrid.Columns["Rank"].Visible = false;
            MachinesOperationsGrid.Columns["Rank2"].Visible = false;
            MachinesOperationsGrid.Columns["PositionID"].Visible = false;
            MachinesOperationsGrid.Columns["PositionID2"].Visible = false;


            tbRank.Text = "1";
            tbRank2.Text = "1";
            cbPositions.SelectedValue = 0;
            cbPositions2.SelectedValue = 0;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (Edit)
            {
                if (MachinesOperationsGrid.SelectedRows.Count == 0)
                    return;

                if (String.IsNullOrWhiteSpace(MachinesOperationNameTextBox.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                                        "Некорректное название!",
                                        "Ошибка");
                    return;
                }

                decimal Norm;
                decimal PreparatoryNorm;

                try
                {
                    Norm = Convert.ToDecimal(NormTextBox.Text);
                }
                catch
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                                        "Некорректное значение нормы в час!",
                                        "Ошибка");
                    return;
                }
                try
                {
                    PreparatoryNorm = Convert.ToDecimal(tbPreparatoryNorm.Text);
                }
                catch
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                                        "Некорректное значение доп. нормы в час!",
                                        "Ошибка");
                    return;
                }

                TechStoreManager.EditMachinesOperation(MachinesOperationID, MachinesOperationNameTextBox.Text, Norm, PreparatoryNorm,
                    Convert.ToInt32(MeasureComboBox.SelectedValue), ArticleTextBox.Text, Convert.ToInt32(cbPositions.SelectedValue), Convert.ToInt32(tbRank.Text),
                    Convert.ToInt32(cbPositions2.SelectedValue), Convert.ToInt32(tbRank2.Text), Convert.ToInt32(cbDocTypes.SelectedValue), Convert.ToInt32(cbAlgorithms.SelectedValue));
            }
            else
            {
                if (MachinesOperationsGrid.SelectedRows.Count == 0)
                    return;

                TechStoreManager.AddMachinesOperationToMachine(MachineID, Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value));
            }

            Ok = true;
            this.Close();
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

        private void MachinesOperationsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            MachinesOperationNameTextBox.Text = MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationName"].Value.ToString();
            NormTextBox.Text = MachinesOperationsGrid.SelectedRows[0].Cells["Norm"].Value.ToString();
            tbPreparatoryNorm.Text = MachinesOperationsGrid.SelectedRows[0].Cells["PreparatoryNorm"].Value.ToString();
            ArticleTextBox.Text = MachinesOperationsGrid.SelectedRows[0].Cells["Article"].Value.ToString();
            tbRank.Text = MachinesOperationsGrid.SelectedRows[0].Cells["Rank"].Value.ToString();
            tbRank2.Text = MachinesOperationsGrid.SelectedRows[0].Cells["Rank2"].Value.ToString();

            if (MachinesOperationsGrid.SelectedRows[0].Cells["MeasureID"].Value != DBNull.Value)
                MeasureComboBox.SelectedValue = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MeasureID"].Value);
            if (MachinesOperationsGrid.SelectedRows[0].Cells["PositionID"].Value != DBNull.Value)
                cbPositions.SelectedValue = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["PositionID"].Value);
            if (MachinesOperationsGrid.SelectedRows[0].Cells["PositionID"].Value != DBNull.Value)
                cbPositions2.SelectedValue = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["PositionID2"].Value);
            if (MachinesOperationsGrid.SelectedRows[0].Cells["CabFurDocTypeID"].Value != DBNull.Value)
                cbDocTypes.SelectedValue = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["CabFurDocTypeID"].Value);
            if (MachinesOperationsGrid.SelectedRows[0].Cells["CabFurAlgorithmID"].Value != DBNull.Value)
                cbAlgorithms.SelectedValue = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["CabFurAlgorithmID"].Value);
        }

        private void AddMachineButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(MachinesOperationNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Некорректное название!", "Ошибка");
                return;
            }
            decimal Norm;
            decimal PreparatoryNorm;

            try
            {
                Norm = Convert.ToDecimal(NormTextBox.Text);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное значение нормы в час!",
                                    "Ошибка");
                return;
            }
            try
            {
                PreparatoryNorm = Convert.ToDecimal(tbPreparatoryNorm.Text);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное значение доп. нормы в час!",
                                    "Ошибка");
                return;
            }

            TechStoreManager.AddMachinesOperation(MachinesOperationNameTextBox.Text, Norm, PreparatoryNorm,
                Convert.ToInt32(MeasureComboBox.SelectedValue), ArticleTextBox.Text, Convert.ToInt32(cbPositions.SelectedValue), Convert.ToInt32(tbRank.Text),
                Convert.ToInt32(cbPositions2.SelectedValue), Convert.ToInt32(tbRank2.Text), Convert.ToInt32(cbDocTypes.SelectedValue), Convert.ToInt32(cbAlgorithms.SelectedValue));

            MachinesOperationsBS.DataSource = TechStoreManager.MachinesOperationsDT.Copy();
            MachinesOperationsBS.MoveLast();
        }

        private void EditMachineButton_Click(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            if (String.IsNullOrWhiteSpace(MachinesOperationNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название!",
                                    "Ошибка");
                return;
            }

            decimal Norm;
            decimal PreparatoryNorm;

            try
            {
                Norm = Convert.ToDecimal(NormTextBox.Text);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное значение нормы в час!",
                                    "Ошибка");
                return;
            }
            try
            {
                PreparatoryNorm = Convert.ToDecimal(tbPreparatoryNorm.Text);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное значение доп. нормы в час!",
                                    "Ошибка");
                return;
            }

            int MachinesOperationID = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value);

            TechStoreManager.EditMachinesOperation(MachinesOperationID, MachinesOperationNameTextBox.Text, Norm, PreparatoryNorm,
                Convert.ToInt32(MeasureComboBox.SelectedValue), ArticleTextBox.Text, Convert.ToInt32(cbPositions.SelectedValue), Convert.ToInt32(tbRank.Text),
                 Convert.ToInt32(cbPositions2.SelectedValue), Convert.ToInt32(tbRank2.Text), Convert.ToInt32(cbDocTypes.SelectedValue), Convert.ToInt32(cbAlgorithms.SelectedValue));

            MachinesOperationsBS.DataSource = TechStoreManager.MachinesOperationsDT.Copy();
            MachinesOperationsBS.Position = MachinesOperationsBS.Find("MachinesOperationID", MachinesOperationID);
        }

        private void RemoveMachineButton_Click(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            int MachinesOperationID = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value);

            if (TechStoreManager.OperationsUseInCatalog(MachinesOperationID))
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Операция используется в каталоге. Вы уверены, что хотите удалить?",
                    "Удаление");

                if (!OKCancel)
                    return;
            }

            TechStoreManager.RemoveMachinesOperation(MachinesOperationID);

            MachinesOperationsBS.DataSource = TechStoreManager.MachinesOperationsDT.Copy();

            if (MachinesOperationsGrid.Rows.Count == 0)
            {
                MachinesOperationNameTextBox.Text = "";
                NormTextBox.Text = "";
                ArticleTextBox.Text = "";
                MeasureComboBox.SelectedValue = 1;
            }
        }
    }
}
