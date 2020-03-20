using Infinium.Modules.TechnologyCatalog;

using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewToolsSubTypeForm : Form
    {
        int ToolsSubTypeID;
        public bool Ok = false;
        bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;
        DataTable ParametrsDT;

        public NewToolsSubTypeForm(int tToolsSubTypeID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = false;

            TechStoreManager = tTechStoreManager;
            Initialize();

            ToolsSubTypeID = tToolsSubTypeID;

            DataRow Row = TechStoreManager.ToolsSubTypeDT.Select("ToolsSubTypeID = " + ToolsSubTypeID)[0];

            ToolsSubTypeNameTextBox.Text = Row["ToolsSubTypeName"].ToString();

            string ParametrsXML = Row["Parametrs"].ToString();
            if (ParametrsXML.Length == 0)
                return;

            using (StringReader SR = new StringReader(ParametrsXML))
            {
                ParametrsDT.ReadXml(SR);
            }

            if (ParametrsDT.Rows.Count != 0)
                ParametrsDT.Columns["ParametrID"].AutoIncrementSeed = Convert.ToInt64(ParametrsDT.Rows[ParametrsDT.Rows.Count - 1]["ParametrID"]);
            else
                ParametrsDT.Columns["ParametrID"].AutoIncrementSeed = 1;
        }

        public NewToolsSubTypeForm(ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = true;

            TechStoreManager = tTechStoreManager;
            Initialize();
        }

        private void Initialize()
        {
            ParametrsTypeComboBox.SelectedIndex = 0;

            ToolsTypeGrid.DataSource = TechStoreManager.ToolsTypesBS;

            ToolsTypeGrid.Columns["ToolsGroupID"].Visible = false;
            ToolsTypeGrid.Columns["ToolsTypeID"].Visible = false;

            ParametrsDT = TechStoreManager.CreateTableParametrs();
            ParametrsGrid.DataSource = ParametrsDT;
            ParametrsGrid.Columns["ParametrID"].Visible = false;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ToolsSubTypeNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название подтипа!",
                                    "Ошибка");
                return;
            }

            string ParametrsDTstring;
            using (StringWriter SW = new StringWriter())
            {
                ParametrsDT.WriteXml(SW);
                ParametrsDTstring = SW.ToString();
            }

            if (IsNew)
                TechStoreManager.AddToolsSubType(Convert.ToInt32(ToolsTypeGrid.SelectedRows[0].Cells["ToolsTypeID"].Value), ToolsSubTypeNameTextBox.Text, ParametrsDTstring);
            else
                TechStoreManager.EditToolsSubType(ToolsSubTypeID, Convert.ToInt32(ToolsTypeGrid.SelectedRows[0].Cells["ToolsTypeID"].Value), ToolsSubTypeNameTextBox.Text, ParametrsDTstring);

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

        private void ParametrsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ParametrsGrid.SelectedRows.Count == 0)
                return;

            ParametrsTextBox.Text = ParametrsGrid.SelectedRows[0].Cells["ParametrName"].Value.ToString();
            ParametrsTypeComboBox.SelectedItem = ParametrsGrid.SelectedRows[0].Cells["Type"].Value;
        }

        private void AddParametrsButton_Click(object sender, EventArgs e)
        {
            DataRow NewRow = ParametrsDT.NewRow();
            NewRow["ParametrName"] = ParametrsTextBox.Text;
            NewRow["Type"] = ParametrsTypeComboBox.SelectedItem;
            ParametrsDT.Rows.Add(NewRow);
        }

        private void EditParametrsButton_Click(object sender, EventArgs e)
        {
            if (ParametrsGrid.SelectedRows.Count == 0)
                return;

            DataRow EditRow = ParametrsDT.Rows[ParametrsGrid.SelectedRows[0].Index];
            EditRow["ParametrName"] = ParametrsTextBox.Text;
            if (IsNew)
                EditRow["Type"] = ParametrsTypeComboBox.SelectedItem;
        }

        private void RemoveParametrsButton_Click(object sender, EventArgs e)
        {
            if (ParametrsGrid.SelectedRows.Count == 0)
                return;

            ParametrsDT.Rows[ParametrsGrid.SelectedRows[0].Index].Delete();

            if (ParametrsGrid.SelectedRows.Count == 0)
            {
                ParametrsTextBox.Text = "";
                ParametrsTypeComboBox.SelectedIndex = 0;
            }
        }
    }
}
