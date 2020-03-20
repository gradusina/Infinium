using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewToolsTypeForm : Form
    {
        int ToolsTypeID;
        public bool Ok = false;
        bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        public NewToolsTypeForm(int tToolsTypeID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = false;

            TechStoreManager = tTechStoreManager;
            Initialize();

            ToolsTypeID = tToolsTypeID;

            ToolsTypeNameTextBox.Text = TechStoreManager.ToolsTypeDT.Select("ToolsTypeID = " + ToolsTypeID)[0]["ToolsTypeName"].ToString();
        }

        public NewToolsTypeForm(ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = true;

            TechStoreManager = tTechStoreManager;
            Initialize();
        }

        private void Initialize()
        {
            ToolsGroupGrid.DataSource = TechStoreManager.ToolsGroupsBS;

            ToolsGroupGrid.Columns["ToolsGroupID"].Visible = false;
            ToolsGroupGrid.Columns["FactoryID"].Visible = false;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ToolsTypeNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название типа!",
                                    "Ошибка");
                return;
            }

            if (IsNew)
                TechStoreManager.AddToolsType(Convert.ToInt32(ToolsGroupGrid.SelectedRows[0].Cells["ToolsGroupID"].Value), ToolsTypeNameTextBox.Text);
            else
                TechStoreManager.EditToolsType(ToolsTypeID, Convert.ToInt32(ToolsGroupGrid.SelectedRows[0].Cells["ToolsGroupID"].Value), ToolsTypeNameTextBox.Text);

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
    }
}
