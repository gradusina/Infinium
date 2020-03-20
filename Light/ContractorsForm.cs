using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ContractorsForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        Contractors Contractors;

        bool bC = false;

        bool bNeedSplash = false;

        public ContractorsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void LightNewsForm_Shown(object sender, EventArgs e)
        {
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
                    bNeedSplash = true;

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


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    bNeedSplash = true;

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

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }


        private void Initialize()
        {
            Contractors = new Contractors();

            CategoriesMenu.ItemsDataTable = Contractors.CategoriesDataTable;
            CategoriesMenu.InitializeItems();

            if (Contractors.CategoriesDataTable.Rows.Count > 0)
                CategoriesMenu.Selected = 0;

            SubCategoriesMenu.ItemsDataTable = Contractors.SubCategoriesDataTable;
            SubCategoriesMenu.InitializeItems();

            ContractorsList.ItemsDataTable = Contractors.ContractorsDataTable;
            ContractorsList.CitiesDataTable = Contractors.CitiesDataTable;
            ContractorsList.CountriesDataTable = Contractors.CountriesDataTable;
            ContractorsList.ContactsDataTable = Contractors.ContactsDataTable;

            DataTable PermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, "ContractorsForm");

            if (PermissionsDataTable.Rows.Count > 0)
            {
                ContractorsList.bCanEdit = true;
                EditButtonsPanel.Visible = true;
            }

            if (Contractors.SubCategoriesDataTable.Rows.Count > 0)
            {
                Contractors.FillContractors(SubCategoriesMenu.SelectedContractorSubCategoryID);
                ContractorsList.InitializeItems();
            }
        }

        private void CoverUpdatePanel()
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }
        }

        private void CoverForm()
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(this.Top, this.Left,
                                                   this.Height, this.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }
        }

        private void Uncover()
        {
            if (bNeedSplash)
                bC = true;
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


        private void ProjectsForm_ANSUpdate(object sender)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ContractorsList.VerticalScroll.Left.ToString() + " " + ContractorsList.Width.ToString());
        }

        private void CategoriesMenu_SelectedChanged(object sender, string Name, int ContractorCategoryID)
        {
            Contractors.SubCategoriesDataTable.DefaultView.RowFilter = "ContractorCategoryID = " + ContractorCategoryID;

            SubCategoriesMenu.ItemsColor = CategoriesMenu.SelectedColor;
            SubCategoriesMenu.InitializeItems();

            if (Contractors.SubCategoriesDataTable.DefaultView.Count == 0)
            {
                SubCategoriesMenu_SelectedChanged(null, "", -1);
            }
        }

        private void SubCategoriesMenu_SelectedChanged(object sender, string Name, int index)
        {
            CoverUpdatePanel();

            Contractors.FillContractors(SubCategoriesMenu.SelectedContractorSubCategoryID);
            ContractorsList.ItemColor = SubCategoriesMenu.ItemsColor;
            ContractorsList.InitializeItems();

            Uncover();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddContractorForm AddContractorForm = new AddContractorForm(ref TopForm, ref Contractors);

            TopForm = AddContractorForm;

            AddContractorForm.ShowDialog();

            if (AddContractorForm.bNewCategory)
            {
                CategoriesMenu.InitializeItems();
                SubCategoriesMenu.InitializeItems();
            }

            if (AddContractorForm.bCanceled == false)
            {
                CoverUpdatePanel();

                Contractors.FillContractors(SubCategoriesMenu.SelectedContractorSubCategoryID);
                ContractorsList.ItemColor = SubCategoriesMenu.ItemsColor;
                ContractorsList.InitializeItems();

                Uncover();
            }

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void ContractorsList_EditClicked(object sender, int iContractorID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddContractorForm AddContractorForm = new AddContractorForm(ref TopForm, ref Contractors, iContractorID);

            TopForm = AddContractorForm;

            AddContractorForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddContractorForm.bNewCategory)
            {
                // CoverForm();

                CategoriesMenu.InitializeItems();
                SubCategoriesMenu.InitializeItems();

                // Uncover();
            }

            if (AddContractorForm.bCanceled == false)
            {
                CoverUpdatePanel();

                Contractors.FillContractors(SubCategoriesMenu.SelectedContractorSubCategoryID);
                ContractorsList.ItemColor = SubCategoriesMenu.ItemsColor;
                ContractorsList.InitializeItems();

                Uncover();
            }

        }

        private void ContractorsList_ReadDescription(object sender, string sText)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ReadForm ReadForm = new ReadForm(ref TopForm, sText);

            TopForm = ReadForm;

            ReadForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

    }
}