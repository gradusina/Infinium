using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewToolsGroupForm : Form
    {
        int ToolsGroupID;
        public bool Ok = false;
        bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        public NewToolsGroupForm(string Factory, int tToolsGroupID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = false;

            TechStoreManager = tTechStoreManager;

            ToolsGroupID = tToolsGroupID;
            FactoryComboBox.SelectedItem = Factory;

            ToolsGroupNameTextBox.Text = TechStoreManager.ToolsGroupDT.Select("ToolsGroupID = " + ToolsGroupID)[0]["ToolsGroupName"].ToString();
        }

        public NewToolsGroupForm(string Factory, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = true;

            TechStoreManager = tTechStoreManager;

            FactoryComboBox.SelectedItem = Factory;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ToolsGroupNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название группы инструмента!",
                                    "Ошибка");
                return;
            }

            int FactoryID;

            if (FactoryComboBox.SelectedItem.ToString() == "Профиль")
                FactoryID = 1;
            else
                FactoryID = 2;

            if (!IsNew)
                TechStoreManager.EditToolsGroup(FactoryID, ToolsGroupID, ToolsGroupNameTextBox.Text);
            else
                TechStoreManager.AddToolsGroup(FactoryID, ToolsGroupNameTextBox.Text);

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
