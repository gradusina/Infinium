using Infinium.Store;

using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SellerForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;


        Form TopForm = null;
        LightStartForm LightStartForm;
        SellersManager SellersManager;

        DataTable RolePermissionsDataTable;

        public SellerForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, this.Name);
            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private bool PermissionGranted(int PermissionID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

            if (Rows.Count() > 0)
            {
                return Convert.ToBoolean(Rows[0]["Granted"]);
            }

            return false;
        }

        private void SellerForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            if (!PermissionGranted(Convert.ToInt32(AddGroupButton.Tag)))
            {
                SellerGroupTextBox.Visible = false;
                AddGroupButton.Visible = false;
                EditGroupButton.Visible = false;
                RemoveGroupButton.Visible = false;

                SellerSubGroupTextBox.Visible = false;
                AddSubGroupButton.Visible = false;
                EditSubGroupButton.Visible = false;
                RemoveSubGroupButton.Visible = false;

                AddItemButton.Visible = false;
                EditItemButton.Visible = false;
                RemoveItemButton.Visible = false;

                panel4.Height = panel12.Height - 2;
                panel5.Height = panel13.Height - 2;
                panel1.Height = panel14.Height - 2;
            }
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
            SellersManager = new SellersManager(
                ref SellersDataGrid,
                ref SellerGroupsDataGrid,
                ref SellerSubGroupsDataGrid,
                ref SellerInfoDataGrid);
            SellerGroupTextBox.DataBindings.Add("Text", SellerGroupsDataGrid.DataSource, "SellerGroup");
            SellerSubGroupTextBox.DataBindings.Add("Text", SellerSubGroupsDataGrid.DataSource, "SellerSubGroup");
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

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            if (SellersManager == null || SellersManager.SubGroupsCount == 0)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            NewSellerForm NewSellerForm = new NewSellerForm(ref SellersManager, -1);

            TopForm = NewSellerForm;

            NewSellerForm.ShowDialog();

            NewSellerForm.Close();
            NewSellerForm.Dispose();

            TopForm = null;
        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            if (SellersDataGrid.SelectedRows.Count > 0)
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
                    UNNLabel.Text = string.Empty;
                    SellersManager.RemoveSeller();
                    SellersManager.GetSellersInfo();
                }
            }
        }

        private void EditItemButton_Click(object sender, EventArgs e)
        {
            if (SellersDataGrid.SelectedRows.Count > 0)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                NewSellerForm NewSellerForm = new NewSellerForm(ref SellersManager, 0);

                TopForm = NewSellerForm;

                NewSellerForm.ShowDialog();

                NewSellerForm.Close();
                NewSellerForm.Dispose();

                TopForm = null;
            }
        }

        private void SellersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (SellersManager == null)
                return;

            if (SellersManager.SellersCount == 0)
            {
                NameLabel.Text = string.Empty;
                CountryLabel.Text = string.Empty;
                AddressLabel.Text = string.Empty;
                ContractDocNumberLabel.Text = string.Empty;
                EmailLabel.Text = string.Empty;
                SiteLabel.Text = string.Empty;
                NotesTextBox.Text = string.Empty;
                UNNLabel.Text = string.Empty;
            }
            else
            {
                NameLabel.Text = SellersManager.CurrentSellerName;
                CountryLabel.Text = SellersManager.CurrentSellerCountry;
                AddressLabel.Text = SellersManager.CurrentSellerAddress;
                ContractDocNumberLabel.Text = SellersManager.CurrentContractDocNumber;
                EmailLabel.Text = SellersManager.CurrentSellerEmail;
                SiteLabel.Text = SellersManager.CurrentSellerSite;
                UNNLabel.Text = SellersManager.CurrentSellerUNN;
                NotesTextBox.Text = SellersManager.CurrentSellerNotes;

                SellersManager.GetCurrentSeller();
            }
            SellersManager.GetSellersInfo();
        }

        private void SiteLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;
            if (SiteLabel.Text != null && (SiteLabel.Text.StartsWith("www.") | SiteLabel.Text.StartsWith("http:") | SiteLabel.Text.StartsWith("https:")))
                url = SiteLabel.Text;
            else
                url = "www." + SiteLabel.Text;
            System.Diagnostics.Process.Start(url);
        }

        private void EditGroupButton_Click(object sender, EventArgs e)
        {
            if (SellerGroupsDataGrid.SelectedRows.Count == 1 && SellerGroupTextBox.Text != string.Empty)
            {
                SellersManager.EditSellerGroup(SellerGroupTextBox.Text);
            }
        }

        private void SellerGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (SellersManager == null)
                return;

            SellersManager.FilterSellerSubGroups();
            //SellerGroupTextBox.Text = SellersManager.CurrentSellerGroupName;
        }

        private void RemoveGroupButton_Click(object sender, EventArgs e)
        {
            //if (SellerGroupsDataGrid.SelectedRows.Count > 0)
            //{
            //    bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Вы уверены, что хотите удалить группу поставщиков?",
            //        "Удаление поставщика");

            //    if (OKCancel)
            //    {
            //        SellersManager.RemoveSellerGroup();
            //    }
            //}
        }

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            if (SellerGroupTextBox.Text.Length == 0)
                return;

            SellersManager.AddSellerGroup(SellerGroupTextBox.Text);
        }

        private void SellerSubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (SellersManager == null)
                return;

            SellersManager.FilterSellers();
            //SellerSubGroupTextBox.Text = SellersManager.CurrentSellerSubGroupName;
        }

        private void EditSubGroupButton_Click(object sender, EventArgs e)
        {
            if (SellerSubGroupsDataGrid.SelectedRows.Count == 1 && SellerSubGroupTextBox.Text != string.Empty)
            {
                SellersManager.EditSellerSubGroup(SellerSubGroupTextBox.Text);
            }
        }

        private void AddSubGroupButton_Click(object sender, EventArgs e)
        {
            if (SellerSubGroupTextBox.Text.Length == 0)
                return;

            SellersManager.AddSellerSubGroup(SellerSubGroupTextBox.Text);
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void RemoveSubGroupButton_Click(object sender, EventArgs e)
        {
            if (SellerSubGroupsDataGrid.SelectedRows.Count > 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы уверены, что хотите удалить подгруппу поставщиков?",
                    "Удаление подгруппы");

                if (OKCancel)
                {
                    SellersManager.RemoveSellerSubGroup();
                }
            }
        }
    }
}
