using Infinium.Modules.Marketing.Clients;

using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MachinesSummaryForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public bool PressOK = false;

        int FormEvent = 0;

        MachinesCatalog MachinesCatalogManager;
        Form MainForm = null;

        public MachinesSummaryForm(Form tMainForm, ref MachinesCatalog tMachinesCatalogManager)
        {
            MainForm = tMainForm;
            MachinesCatalogManager = tMachinesCatalogManager;
            InitializeComponent();
            Initialize();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < dgvMachines.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvMachines.Rows[i].Cells["Check"].Value))
                    array.Add(Convert.ToInt32(dgvMachines.Rows[i].Cells["MachineID"].Value));
            }
            if (array.Count > 0)
                MachinesCatalogManager.MachinesSumming(array.OfType<int>().ToArray());
            else
                MachinesCatalogManager.ClearMachinesSummary();
            //PressOK = true;
            //FormEvent = eClose;
            //AnimateTimer.Enabled = true;
        }

        private void CancelButtonButton_Click(object sender, EventArgs e)
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

        private void AddCheckBoxColumn()
        {
            ComponentFactory.Krypton.Toolkit.KryptonCheckBox checkboxHeader = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox()
            {
                PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black,
                Name = "checkboxHeader",
                Text = string.Empty,
                Size = new Size(18, 18),
                Location = new Point(19, 12)
            };
            checkboxHeader.CheckedChanged += new EventHandler(checkboxHeader_CheckedChanged);

            dgvMachines.Controls.Add(checkboxHeader);
        }

        private void checkboxHeader_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvMachines.RowCount; i++)
            {
                dgvMachines["Check", i].Value = ((ComponentFactory.Krypton.Toolkit.KryptonCheckBox)dgvMachines.Controls.Find("checkboxHeader", true)[0]).Checked;
            }
            dgvMachines.EndEdit();
        }

        private void Initialize()
        {
            AddCheckBoxColumn();
            MachinesCatalogManager.CreateTempMachines();

            dgvMachinesSummary.DataSource = MachinesCatalogManager.MachinesSummaryList;
            dgvMachines.DataSource = MachinesCatalogManager.TempMachinesList;
            MachinesCatalogManager.ClearMachinesSummary();

            dgvMachines.Columns["Check"].HeaderText = string.Empty;
            dgvMachines.Columns["MachineName"].HeaderText = "Станок";
            dgvMachines.AutoGenerateColumns = false;
            dgvMachines.Columns["MachineID"].Visible = false;
            dgvMachines.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMachines.Columns["Check"].Width = 50;
            dgvMachines.Columns["MachineName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMachines.Columns["Check"].DisplayIndex = 1;
            dgvMachines.Columns["MachineName"].DisplayIndex = 2;
            dgvMachinesSummary.Columns["Name"].ReadOnly = true;
            dgvMachinesSummary.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMachinesSummary.Columns["Name"].Width = 220;
            dgvMachinesSummary.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
    }
}
