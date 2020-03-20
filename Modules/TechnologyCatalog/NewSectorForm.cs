using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewSectorForm : Form
    {
        int SectorID;
        public bool Ok = false;
        bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        public NewSectorForm(string Factory, int tSectorID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = false;

            TechStoreManager = tTechStoreManager;

            SectorID = tSectorID;
            FactoryComboBox.SelectedItem = Factory;

            SectorNameTextBox.Text = TechStoreManager.SectorsDT.Select("SectorID = " + SectorID)[0]["SectorName"].ToString();
        }

        public NewSectorForm(string Factory, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = true;

            TechStoreManager = tTechStoreManager;

            FactoryComboBox.SelectedItem = Factory;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SectorNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название участка!",
                                    "Ошибка");
                return;
            }

            int FactoryID;

            if (FactoryComboBox.SelectedItem.ToString() == "Профиль")
                FactoryID = 1;
            else
                FactoryID = 2;

            if (!IsNew)
                TechStoreManager.EditSector(SectorID, FactoryID, SectorNameTextBox.Text);
            else
                TechStoreManager.AddSector(FactoryID, SectorNameTextBox.Text);

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
