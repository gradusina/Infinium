using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewSubSectorForm : Form
    {
        int SubSectorID;
        public bool Ok = false;
        bool IsNew;

        Form TopForm = null;
        TechStoreManager TechStoreManager;

        public NewSubSectorForm(int tSubSectorID, ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = false;

            TechStoreManager = tTechStoreManager;
            Initialize();

            SubSectorID = tSubSectorID;

            SubSectorNameTextBox.Text = TechStoreManager.SubSectorsDT.Select("SubSectorID = " + SubSectorID)[0]["SubSectorName"].ToString();
        }

        public NewSubSectorForm(ref TechStoreManager tTechStoreManager)
        {
            InitializeComponent();

            IsNew = true;

            TechStoreManager = tTechStoreManager;
            Initialize();
        }

        private void Initialize()
        {
            SectorsGrid.DataSource = TechStoreManager.SectorsBS;

            SectorsGrid.Columns["SectorID"].Visible = false;
            SectorsGrid.Columns["FactoryID"].Visible = false;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SubSectorNameTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название подучастка!",
                                    "Ошибка");
                return;
            }

            if (IsNew)
                TechStoreManager.AddSubSector(Convert.ToInt32(SectorsGrid.SelectedRows[0].Cells["SectorID"].Value), SubSectorNameTextBox.Text);
            else
                TechStoreManager.EditSubSector(SubSectorID, Convert.ToInt32(SectorsGrid.SelectedRows[0].Cells["SectorID"].Value), SubSectorNameTextBox.Text);

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
