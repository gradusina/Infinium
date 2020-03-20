using Infinium.Modules.Marketing.MutualSettlements;

using System;
using System.Collections;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MutualSettlementsFilterMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public CashReportParameters CashReportParameters;
        MutualSettlements MutualSettlementsManager;
        int FormEvent = 0;
        string ReportName = string.Empty;

        Form MainForm = null;

        public MutualSettlementsFilterMenu(Form tMainForm, MutualSettlements tMutualSettlementsManager)
        {
            MainForm = tMainForm;
            MutualSettlementsManager = tMutualSettlementsManager;
            InitializeComponent();
            dgvClients.DataSource = tMutualSettlementsManager.FilterClientsBS;
            if (dgvClients.Columns.Contains("ClientID"))
                dgvClients.Columns["ClientID"].Visible = false;
            dgvClients.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClients.Columns["Check"].Width = 60;
            dgvClients.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvClients.Columns["ClientName"].MinimumWidth = 100;
            int DisplayIndex = 0;
            dgvClients.Columns["Check"].DisplayIndex = DisplayIndex++;
            dgvClients.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            MutualSettlementsManager.CheckAllClients(false);
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

        private void btnOKReport_Click(object sender, EventArgs e)
        {
            CashReportParameters.Clients = new ArrayList();
            CashReportParameters.Cancel = false;
            CashReportParameters.AllClients = cbAllClients.Checked;
            for (int i = 0; i < dgvClients.Rows.Count; i++)
            {
                if (dgvClients.Rows[i].Cells["Check"].Value == DBNull.Value || !Convert.ToBoolean(dgvClients.Rows[i].Cells["Check"].Value))
                    continue;
                CashReportParameters.Clients.Add(Convert.ToInt32(dgvClients.Rows[i].Cells["ClientID"].Value));
            }
            CashReportParameters.Date1 = CalendarFrom.SelectionStart;
            CashReportParameters.Date2 = CalendarTo.SelectionStart;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelReport_Click(object sender, EventArgs e)
        {
            CashReportParameters.Cancel = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void cbAllClients_CheckedChanged(object sender, EventArgs e)
        {
            MutualSettlementsManager.CheckAllClients(cbAllClients.Checked);
        }
    }
}
