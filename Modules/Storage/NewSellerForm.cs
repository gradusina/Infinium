using Infinium.Store;

using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewSellerForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        SellersManager SellersManager;
        DataTable SellerInfoDataTable;
        int Index;

        public NewSellerForm(ref SellersManager tSellersManager, int tIndex)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            Index = tIndex;
            if (Index == -1)
            {
                SellersManager = tSellersManager;
                SellerInfoDataTable = SellersManager.InfoDataTableClone;
            }
            else
            {
                SellersManager = tSellersManager;

                NameTextBox.Text = SellersManager.CurrentSellerName;
                CountryTextBox.Text = SellersManager.CurrentSellerCountry;
                AddressTextBox.Text = SellersManager.CurrentSellerAddress;
                ContractDocNumberTextBox.Text = SellersManager.CurrentContractDocNumber;
                EmailTextBox.Text = SellersManager.CurrentSellerEmail;
                SiteTextBox.Text = SellersManager.CurrentSellerSite;
                UNNTextBox.Text = SellersManager.CurrentSellerUNN;
                NotesTextBox.Text = SellersManager.CurrentSellerNotes;

                SellerInfoDataTable = SellersManager.InfoDataTableCopy;
            }
            SellerInfoDataGrid.DataSource = SellerInfoDataTable;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            SellerInfoDataGrid.Columns["Name"].MinimumWidth = 150;
            SellerInfoDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SellerInfoDataGrid.Columns["Position"].MinimumWidth = 150;
            SellerInfoDataGrid.Columns["Position"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SellerInfoDataGrid.Columns["Phone"].MinimumWidth = 150;
            SellerInfoDataGrid.Columns["Phone"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SellerInfoDataGrid.Columns["Email"].MinimumWidth = 150;
            SellerInfoDataGrid.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SellerInfoDataGrid.Columns["ICQ"].MinimumWidth = 150;
            SellerInfoDataGrid.Columns["ICQ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SellerInfoDataGrid.Columns["Skype"].MinimumWidth = 150;
            SellerInfoDataGrid.Columns["Skype"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            SellerInfoDataGrid.Columns["Name"].HeaderText = "Имя";
            SellerInfoDataGrid.Columns["Position"].HeaderText = "Должность";
            SellerInfoDataGrid.Columns["Phone"].HeaderText = "Телефон";
            SellerInfoDataGrid.Columns["Email"].HeaderText = "E-mail";
            SellerInfoDataGrid.Columns["ICQ"].HeaderText = "ICQ";
            SellerInfoDataGrid.Columns["Skype"].HeaderText = "Skype";
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

        private void NewSellersForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (NameTextBox.Text != "")
                using (StringWriter SW = new StringWriter())
                {
                    SellerInfoDataTable.WriteXml(SW);

                    if (Index == -1)
                    {
                        SellersManager.AddSeller(NameTextBox.Text, CountryTextBox.Text, AddressTextBox.Text,
                              ContractDocNumberTextBox.Text, EmailTextBox.Text, SiteTextBox.Text, SW.ToString(),
                              NotesTextBox.Text, UNNTextBox.Text);
                    }
                    else
                    {
                        SellersManager.EditSeller(NameTextBox.Text, CountryTextBox.Text, AddressTextBox.Text,
                           ContractDocNumberTextBox.Text, EmailTextBox.Text, SiteTextBox.Text, SW.ToString(),
                           NotesTextBox.Text, UNNTextBox.Text);
                    }
                }
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
