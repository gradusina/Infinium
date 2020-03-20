using Infinium.Modules.Marketing.Clients;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewMachineForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool EditMachine = false;
        public bool PressOK = false;

        int FormEvent = 0;
        int CurrentFactoryID = -1;
        int CurrentSectorID = -1;
        int CurrentSubSectorID = -1;
        int MachineID = -1;
        string MachineName = string.Empty;

        Form MainForm = null;
        Form TopForm = null;
        MachinesCatalog MachinesCatalogManager;

        public NewMachineForm(Form tMainForm, ref MachinesCatalog tMachinesCatalogManager)
        {
            MainForm = tMainForm;
            MachinesCatalogManager = tMachinesCatalogManager;
            InitializeComponent();
            Initialize();
        }

        public NewMachineForm(Form tMainForm, ref MachinesCatalog tMachinesCatalogManager,
            int iMachineID, string sMachineName)
        {
            EditMachine = true;
            MainForm = tMainForm;
            MachinesCatalogManager = tMachinesCatalogManager;
            MachineID = iMachineID;
            MachineName = sMachineName;
            CurrentSubSectorID = MachinesCatalogManager.GetSubSectorID(MachineID);
            CurrentSectorID = MachinesCatalogManager.GetSectorID(CurrentSubSectorID);
            CurrentFactoryID = MachinesCatalogManager.GetFactoryID(CurrentSectorID);
            InitializeComponent();
            Initialize();
            tbMachineName.Text = MachineName;
        }

        private void btnPressOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbMachineName.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название!",
                                    "Ошибка");
                return;
            }

            if (!EditMachine)
                MachinesCatalogManager.AddMachine(Convert.ToInt32(dgvSubSectors.SelectedRows[0].Cells["SubSectorID"].Value), tbMachineName.Text);
            else
                MachinesCatalogManager.EditMachine(MachineID, Convert.ToInt32(dgvSubSectors.SelectedRows[0].Cells["SubSectorID"].Value), tbMachineName.Text);
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
            PressOK = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnPressCancel_Click(object sender, EventArgs e)
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void Initialize()
        {
            cbFactory.DataSource = MachinesCatalogManager.FactoryList;
            cbFactory.DisplayMember = "FactoryName";
            cbFactory.ValueMember = "FactoryID";

            dgvSectors.SelectionChanged -= dgvSectors_SelectionChanged;
            dgvSectors.DataSource = MachinesCatalogManager.SectorsList;
            dgvSectors.Columns["SectorID"].Visible = false;
            dgvSectors.Columns["FactoryID"].Visible = false;
            dgvSectors.SelectionChanged += dgvSectors_SelectionChanged;

            dgvSubSectors.DataSource = MachinesCatalogManager.SubSectorsList;
            dgvSubSectors.Columns["SubSectorID"].Visible = false;
            dgvSubSectors.Columns["SectorID"].Visible = false;
        }

        private void dgvSectors_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;

            int SectorID = 0;

            if (dgvSectors.SelectedRows.Count != 0 && dgvSectors.SelectedRows[0].Cells["SectorID"].Value != DBNull.Value)
                SectorID = Convert.ToInt32(dgvSectors.SelectedRows[0].Cells["SectorID"].Value);

            MachinesCatalogManager.FilterSubSectors(SectorID);
        }

        private void cbFactory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;

            MachinesCatalogManager.FilterSectors(Convert.ToInt32(((DataRowView)cbFactory.SelectedItem).Row["FactoryID"]));
        }

        private void NewMachineForm_Load(object sender, EventArgs e)
        {
            MachinesCatalogManager.MoveToFactory(CurrentFactoryID);
            MachinesCatalogManager.MoveToSector(CurrentSectorID);
            MachinesCatalogManager.MoveToSubSector(CurrentSubSectorID);
        }

        private void NewMachineForm_Shown(object sender, EventArgs e)
        {

        }

    }
}
