using Infinium.Modules.TechnologyCatalog;

using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewToolsForm : Form
    {
        int ToolsID, ToolsSubTypeID;
        public bool Ok = false;
        bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        DataTable ValueParametrsDT;

        public NewToolsForm(int tToolsID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = false;

            TechStoreManager = tTechStoreManager;

            ToolsID = tToolsID;

            DataRow Row = TechStoreManager.ToolsDT.Select("ToolsID = " + ToolsID)[0];

            ToolsNameTextBox.Text = Row["ToolsName"].ToString();
            ToolsSubTypeID = Convert.ToInt32(Row["ToolsSubTypeID"]);
            Initialize();
        }

        public NewToolsForm(ref TechStoreManager tTechStoreManager, int tToolsSubTypeID)
        {
            InitializeComponent();

            IsNew = true;
            ToolsSubTypeID = tToolsSubTypeID;

            TechStoreManager = tTechStoreManager;
            Initialize();
        }

        private void Initialize()
        {
            if (!IsNew)
                ValueParametrsDT = TechStoreManager.ReadValueTable(TechStoreManager.ToolsDT.Select("ToolsID = " + ToolsID)[0]["ValueParametrs"].ToString(), TechStoreManager.ToolsSubTypeDT.Select("ToolsSubTypeID = " + ToolsSubTypeID)[0]["Parametrs"].ToString());
            else
                ValueParametrsDT = TechStoreManager.ReadValueTable("", TechStoreManager.ToolsSubTypeDT.Select("ToolsSubTypeID = " + ToolsSubTypeID)[0]["Parametrs"].ToString());

            ValueParametrsGrid.DataSource = ValueParametrsDT;

            ValueParametrsGrid.ReadOnly = false;
            ValueParametrsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;

            DataGridViewComboBoxColumn ParametrName = new DataGridViewComboBoxColumn()
            {
                DataSource = TechStoreManager.CurrentParametrsDT,
                DisplayMember = "ParametrName",
                ValueMember = "ParametrID",
                DataPropertyName = "ParametrID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ValueParametrsGrid.Columns.Add(ParametrName);

            ValueParametrsGrid.Columns["ParametrID"].Visible = false;
            ParametrName.DisplayIndex = 0;
            ParametrName.ReadOnly = true;

            ValueParametrsGrid.Columns["Value"].ReadOnly = false;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ToolsNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название!",
                                    "Ошибка");
                return;
            }

            string ValueParametrsDTstring;
            using (StringWriter SW = new StringWriter())
            {
                ValueParametrsDT.WriteXml(SW);
                ValueParametrsDTstring = SW.ToString();
            }

            if (IsNew)
                TechStoreManager.AddTools(ToolsSubTypeID, ToolsNameTextBox.Text, ValueParametrsDTstring);
            else
                TechStoreManager.EditTools(ToolsID, ToolsNameTextBox.Text, ValueParametrsDTstring);

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

        private void ValueParametrsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!TechStoreManager.ParametrCanBeApply(Convert.ToInt32(ValueParametrsGrid.Rows[e.RowIndex].Cells["ParametrID"].Value), ValueParametrsGrid.Rows[e.RowIndex].Cells["Value"].Value.ToString()))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное значение параметра!",
                                    "Ошибка");

                ValueParametrsGrid.Rows[e.RowIndex].Cells["Value"].Value = 0;
            }
        }
    }
}
