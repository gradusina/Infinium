using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ToolsSellerForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;


        Form TopForm = null;
        LightStartForm LightStartForm;
        ToolsSellersManager ToolsSellersManager;

        public ToolsSellerForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void StorageForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }


        private void Initialize()
        {
            ToolsSellersManager = new ToolsSellersManager(
                ref ToolsSellersDataGrid,
                ref ToolsSellerInfoDataGrid,
                ref ToolsSellersGroupsDataGrid,
                ref ToolsSellersSubGroupsDataGrid);
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

        private void AddSellerButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellersManager == null || ToolsSellersManager.ToolsSubGroupsCount == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewToolsSellerForm NewToolsSellerForm = new NewToolsSellerForm(ref ToolsSellersManager, -1);

            TopForm = NewToolsSellerForm;

            NewToolsSellerForm.ShowDialog();

            NewToolsSellerForm.Close();
            NewToolsSellerForm.Dispose();

            TopForm = null;
        }

        private void RemoveSellerButton_Click(object sender, EventArgs e)
        {

            if (ToolsSellersDataGrid.SelectedRows.Count > 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы уверены, что хотите удалить поставщика?",
                    "Удаление поставщика");

                if (OKCancel)
                {
                    NameLabel.Text = string.Empty;
                    CountryLabel.Text = string.Empty;
                    AddressLabel.Text = string.Empty;
                    ContractDocNumberLabel.Text = string.Empty;
                    EmailLabel.Text = string.Empty;
                    SiteLabel.Text = string.Empty;
                    NotesTextBox.Text = string.Empty;
                    ToolsSellersManager.RemoveSeller();
                    ToolsSellersManager.GetToolsSellerInfo();
                }
            }
        }

        private void EditSellerButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellersDataGrid.SelectedRows.Count > 0
                && ToolsSellersGroupsDataGrid.SelectedRows.Count > 0)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                NewToolsSellerForm NewToolsSellerForm = new NewToolsSellerForm(ref ToolsSellersManager, 0);

                TopForm = NewToolsSellerForm;

                NewToolsSellerForm.ShowDialog();

                NewToolsSellerForm.Close();
                NewToolsSellerForm.Dispose();

                TopForm = null;
            }
        }

        private void ToolsSellersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ToolsSellersManager == null)
                return;

            if (ToolsSellersManager.ToolsSellersCount == 0)
            {
                NameLabel.Text = string.Empty;
                CountryLabel.Text = string.Empty;
                AddressLabel.Text = string.Empty;
                ContractDocNumberLabel.Text = string.Empty;
                EmailLabel.Text = string.Empty;
                SiteLabel.Text = string.Empty;
                NotesTextBox.Text = string.Empty;
            }
            else
            {
                NameLabel.Text = ToolsSellersManager.CurrentToolsSellerName;
                CountryLabel.Text = ToolsSellersManager.CurrentToolsSellerCountry;
                AddressLabel.Text = ToolsSellersManager.CurrentToolsSellerAddress;
                ContractDocNumberLabel.Text = ToolsSellersManager.CurrentContractDocNumber;
                EmailLabel.Text = ToolsSellersManager.CurrentToolsSellerEmail;
                SiteLabel.Text = ToolsSellersManager.CurrentToolsSellerSite;
                NotesTextBox.Text = ToolsSellersManager.CurrentToolsSellerNotes;

                ToolsSellersManager.GetCurrentSeller();
            }

            ToolsSellersManager.GetToolsSellerInfo();
        }

        private void SiteLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;
            if (SiteLabel.Text != null
                && (SiteLabel.Text.StartsWith("www.") | SiteLabel.Text.StartsWith("http:") | SiteLabel.Text.StartsWith("https:")))
                url = SiteLabel.Text;
            else
                url = "www." + SiteLabel.Text;
            System.Diagnostics.Process.Start(url);
        }

        private void ToolsSellersGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ToolsSellersManager == null)
                return;

            ToolsSellersManager.FilterSellerSubGroups();
            ToolsSellerGroupTextBox.Text = ToolsSellersManager.CurrentToolsSellerGroupName;
        }

        private void ToolsSellersSubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ToolsSellersManager == null)
                return;

            ToolsSellersManager.FilterSellers();
            ToolsSellerSubGroupTextBox.Text = ToolsSellersManager.CurrentToolsSellerSubGroupName;
        }

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellerGroupTextBox.Text.Length == 0)
                return;

            ToolsSellersManager.AddSellerGroup(ToolsSellerGroupTextBox.Text);
        }

        private void EditGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellersGroupsDataGrid.SelectedRows.Count == 1
                && ToolsSellerGroupTextBox.Text != string.Empty)
            {
                ToolsSellersManager.EditSellerGroup(ToolsSellerGroupTextBox.Text);
            }
        }

        private void AddSubGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellerSubGroupTextBox.Text.Length == 0)
                return;

            ToolsSellersManager.AddSellerSubGroup(ToolsSellerSubGroupTextBox.Text);
        }

        private void EditSubGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellersSubGroupsDataGrid.SelectedRows.Count == 1
                && ToolsSellerSubGroupTextBox.Text != string.Empty)
            {
                ToolsSellersManager.EditSellerSubGroup(ToolsSellerSubGroupTextBox.Text);
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void RemoveSubGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellersSubGroupsDataGrid.SelectedRows.Count > 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы уверены, что хотите удалить подгруппу?",
                    "Удаление подгруппы");

                if (OKCancel)
                {
                    NameLabel.Text = string.Empty;
                    CountryLabel.Text = string.Empty;
                    AddressLabel.Text = string.Empty;
                    ContractDocNumberLabel.Text = string.Empty;
                    EmailLabel.Text = string.Empty;
                    SiteLabel.Text = string.Empty;
                    NotesTextBox.Text = string.Empty;
                    ToolsSellersManager.RemoveSellerSubGroup();
                    ToolsSellersManager.GetToolsSellerInfo();
                }
            }
        }

        private void RemoveGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsSellersGroupsDataGrid.SelectedRows.Count > 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы уверены, что хотите удалить группу?",
                    "Удаление группы");

                if (OKCancel)
                {
                    NameLabel.Text = string.Empty;
                    CountryLabel.Text = string.Empty;
                    AddressLabel.Text = string.Empty;
                    ContractDocNumberLabel.Text = string.Empty;
                    EmailLabel.Text = string.Empty;
                    SiteLabel.Text = string.Empty;
                    NotesTextBox.Text = string.Empty;
                    ToolsSellersManager.RemoveSellerGroup();
                    ToolsSellersManager.GetToolsSellerInfo();
                }
            }
        }
    }
}
