using Infinium.Modules.Marketing.Clients;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TechnicalSpecificationForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool EditGroups = false;
        public bool PressOK = false;

        int FormEvent = 0;

        MachinesCatalog MachinesCatalogManager;
        Form MainForm = null;

        public TechnicalSpecificationForm(bool bEditGroups, Form tMainForm, ref MachinesCatalog tMachinesCatalogManager)
        {
            EditGroups = bEditGroups;
            MainForm = tMainForm;
            MachinesCatalogManager = tMachinesCatalogManager;
            InitializeComponent();
            if (bEditGroups)
            {
                panel9.Height = 314;
                panel2.Visible = false;
            }
            Initialize();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (EditGroups)
                MachinesCatalogManager.PreSaveTechnicalSpecification(true, string.Empty);
            else
            {
                if (dgvGroups.SelectedRows.Count > 0 &&
                    dgvGroups.SelectedRows[0].Cells["Group"].Value != DBNull.Value)
                    MachinesCatalogManager.PreSaveTechnicalSpecification(false, dgvGroups.SelectedRows[0].Cells["Group"].Value.ToString());
            }
            //MachinesCatalogManager.CurrentTechnicalSpecification = MachinesCatalogManager.GetTechnicalSpecification();
            //MachinesCatalogManager.SaveMachines();
            PressOK = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
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
            MachinesCatalogManager.F();
            dgvGroups.DataSource = MachinesCatalogManager.TempTechnicalSpecificationList;

            if (dgvGroups.Columns.Contains("GroupNumber"))
                dgvGroups.Columns["GroupNumber"].Visible = false;
            dgvGroups.Columns["Group"].HeaderText = "Подраздел";
            dgvGroups.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnAddTechnicalSpecification_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbGroupName.Text))
                return;
            MachinesCatalogManager.AddTecnhicalRow(tbGroupName.Text);
        }
    }
}
