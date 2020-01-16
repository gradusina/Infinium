using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingNewDispatchMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool ChangeDispatchDate = false;
        public bool PressOK = false;
        public int ClientID = 0;
        public object DispatchDate = null;

        int FormEvent = 0;

        DataTable ClientsDT;
        Form MainForm = null;
        Form TopForm = null;

        public MarketingNewDispatchMenu(Form tMainForm, DataTable tClientsDT, bool bChangeDispatchDate)
        {
            MainForm = tMainForm;
            ClientsDT = tClientsDT;
            ChangeDispatchDate = bChangeDispatchDate;
            InitializeComponent();
            Initialize();
            if (ChangeDispatchDate)
            {
                label1.Visible = false;
                cbClients.Visible = false;
                panel2.Left = this.Width / 2 - panel2.Width / 2;
                panel2.Top = this.Height / 2 - panel2.Height / 2;
            }
        }

        private void btnPressOK_Click(object sender, EventArgs e)
        {
            if (!ChangeDispatchDate)
                ClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            DispatchDate = mcDispatchDate.SelectionStart;
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
            if (!ChangeDispatchDate)
            {
                cbClients.DataSource = new DataView(ClientsDT);
                cbClients.DisplayMember = "ClientName";
                cbClients.ValueMember = "ClientID";
            }
        }
    }
}
