using Infinium.Modules.Marketing.Clients;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CountriesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;

        Clients Clients = null;

        public CountriesForm(Form tMainForm, Clients tClients)
        {
            MainForm = tMainForm;
            Clients = tClients;

            InitializeComponent();

            Initialize();
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
            Clients.GetCountries();
            dgvCountries.DataSource = Clients.NewCountriesBindingSource;

            dgvCountries.Columns["CountryID"].Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnSaveCountries_Click(object sender, EventArgs e)
        {
            Clients.SaveCountries();
            Clients.UpdateCountries();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CountriesForm_Load(object sender, EventArgs e)
        {
        }

        private void btnAddCountry_Click(object sender, EventArgs e)
        {
            if (tbNewCountry.Text.Length > 0)
            {
                Clients.AddCountry(tbNewCountry.Text);
                tbNewCountry.Clear();
            }
        }
    }
}
