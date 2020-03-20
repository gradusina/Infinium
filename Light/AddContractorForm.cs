using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddContractorForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm;

        Contractors Contractors;

        public bool bCanceled = false;
        bool bNeedSplash = true;
        bool bC = false;

        public int ContractorID = -1;
        public bool bNewCategory = false;

        public DataTable CityDT;
        public DataTable SubCategoriesDT;
        public DataTable ContactsDataTable;

        public AddContractorForm(ref Form tTopForm, ref Contractors tContractors)
        {
            InitializeComponent();

            Contractors = tContractors;

            TopForm = tTopForm;

            CountryComboBox.DataSource = Contractors.CountriesDataTable;
            CountryComboBox.DisplayMember = "Name";
            CountryComboBox.ValueMember = "CountryID";

            CityDT = new DataTable();
            CityDT = Contractors.CitiesDataTable.Copy();

            CityComboBox.DataSource = CityDT.DefaultView;
            CityComboBox.DisplayMember = "Name";
            CityComboBox.ValueMember = "CityID";

            CategoriesComboBox.DataSource = Contractors.CategoriesDataTable;
            CategoriesComboBox.DisplayMember = "Name";
            CategoriesComboBox.ValueMember = "ContractorCategoryID";

            SubCategoriesDT = new DataTable();
            SubCategoriesDT = Contractors.SubCategoriesDataTable.Copy();

            SubCategoriesComboBox.DataSource = SubCategoriesDT.DefaultView;
            SubCategoriesComboBox.DisplayMember = "Name";
            SubCategoriesComboBox.ValueMember = "ContractorSubCategoryID";

            CountryComboBox_SelectedValueChanged(null, null);
            CategoriesComboBox_SelectedValueChanged(null, null);

            ContactsDataTable = new DataTable();
            ContactsDataTable = Contractors.ContactsDataTable.Clone();
        }

        public AddContractorForm(ref Form tTopForm, ref Contractors tContractors, int iContractorID)
        {
            InitializeComponent();

            Contractors = tContractors;
            ContractorID = iContractorID;
            TopForm = tTopForm;

            CreateButton.Text = "Сохранить";

            CountryComboBox.DataSource = Contractors.CountriesDataTable;
            CountryComboBox.DisplayMember = "Name";
            CountryComboBox.ValueMember = "CountryID";

            CityDT = new DataTable();
            CityDT = Contractors.CitiesDataTable.Copy();

            CityComboBox.DataSource = CityDT.DefaultView;
            CityComboBox.DisplayMember = "Name";
            CityComboBox.ValueMember = "CityID";

            CategoriesComboBox.DataSource = Contractors.CategoriesDataTable;
            CategoriesComboBox.DisplayMember = "Name";
            CategoriesComboBox.ValueMember = "ContractorCategoryID";

            SubCategoriesDT = new DataTable();
            SubCategoriesDT = Contractors.SubCategoriesDataTable.Copy();

            SubCategoriesComboBox.DataSource = SubCategoriesDT.DefaultView;
            SubCategoriesComboBox.DisplayMember = "Name";
            SubCategoriesComboBox.ValueMember = "ContractorSubCategoryID";

            string sName = "";
            string Email = "";
            string Website = "";
            string Address = "";
            string Facebook = "";
            string Skype = "";
            int CountryID = -1;
            int CityID = -1;
            string UNN = "";
            string Description = "";
            int CategoryID = -1;
            int SubCategoryID = -1;

            Contractors.GetEditContractor(ContractorID, ref sName, ref Email, ref Website, ref Address, ref Facebook, ref Skype,
                                          ref CountryID, ref CityID, ref UNN, ref Description, ref CategoryID, ref SubCategoryID);

            ContactsDataTable = Contractors.CurrentContactsDataTable;

            NameTextBox.Text = sName;
            EmailTextBox.Text = Email;
            WEBSiteTextBox.Text = Website;
            AddressTextBox.Text = Address;
            FacebookTextBox.Text = Facebook;
            SkypeTextBox.Text = Skype;

            CountryComboBox.SelectedValue = CountryID;
            CountryComboBox_SelectedValueChanged(null, null);

            CityComboBox.SelectedValue = CityID;
            UNNTextBox.Text = UNN;
            DescriptionTextBox.Text = Description;

            CategoriesComboBox.SelectedValue = CategoryID;
            CategoriesComboBox_SelectedValueChanged(null, null);

            SubCategoriesComboBox.SelectedValue = SubCategoryID;

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

        private void AddNewsForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }


        private void Cover()
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(this.Top, this.Left, this.Height, this.Width);
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


        private void CancelMessagesButton_Click(object sender, EventArgs e)
        {
            bCanceled = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }


        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (NameTextBox.Text.Length == 0)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Введите название", 2600);
                return;
            }

            if (ContractorID > -1)
            {
                Contractors.EditContractor(ContractorID, NameTextBox.Text, EmailTextBox.Text, WEBSiteTextBox.Text, AddressTextBox.Text, FacebookTextBox.Text,
                                          SkypeTextBox.Text, Convert.ToInt32(CountryComboBox.SelectedValue), Convert.ToInt32(CityComboBox.SelectedValue),
                                          UNNTextBox.Text, DescriptionTextBox.Text, Convert.ToInt32(CategoriesComboBox.SelectedValue),
                                          Convert.ToInt32(SubCategoriesComboBox.SelectedValue), ContactsDataTable);
            }
            else
            {
                if (Contractors.AddContractor(NameTextBox.Text, EmailTextBox.Text, WEBSiteTextBox.Text, AddressTextBox.Text, FacebookTextBox.Text,
                                              SkypeTextBox.Text, Convert.ToInt32(CountryComboBox.SelectedValue), Convert.ToInt32(CityComboBox.SelectedValue),
                                              UNNTextBox.Text, DescriptionTextBox.Text, Convert.ToInt32(CategoriesComboBox.SelectedValue),
                                              Convert.ToInt32(SubCategoriesComboBox.SelectedValue), ContactsDataTable) == false)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Контрагент с таким именем уже существует", 4600);
                    return;
                }
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CountryComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (CityDT == null)
                return;

            if (Convert.ToInt32(CountryComboBox.SelectedValue) == -1)
                return;

            CityDT.DefaultView.RowFilter = "CountryID = " + Convert.ToInt32(CountryComboBox.SelectedValue);
        }

        private void CategoriesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SubCategoriesDT == null)
                return;

            if (Convert.ToInt32(CategoriesComboBox.SelectedValue) == -1)
                return;

            SubCategoriesDT.DefaultView.RowFilter = "ContractorCategoryID = " + Convert.ToInt32(CategoriesComboBox.SelectedValue);
        }

        private void CategoryNoListLabel_Click(object sender, EventArgs e)
        {
            AddItemForm AddItemForm = new AddItemForm(ref TopForm, "Введите название новой категории")
            {
                StartPosition = FormStartPosition.CenterParent
            };
            AddItemForm.ShowDialog();

            if (AddItemForm.bCanceled)
                return;

            Cover();

            Contractors.AddCategory(AddItemForm.sText);
            Contractors.RefillCategories();
            bNewCategory = true;

            Uncover();
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

        private void SubCategoryNoListLabel_Click(object sender, EventArgs e)
        {
            AddItemForm AddItemForm = new AddItemForm(ref TopForm, "Введите название новой подкатегории")
            {
                StartPosition = FormStartPosition.CenterParent
            };
            AddItemForm.ShowDialog();

            if (AddItemForm.bCanceled)
                return;

            Cover();

            Contractors.AddSubCategory(AddItemForm.sText, Convert.ToInt32(CategoriesComboBox.SelectedValue));
            Contractors.RefillSubCategories();
            bNewCategory = true;

            SubCategoriesDT.Clear();
            SubCategoriesDT = Contractors.SubCategoriesDataTable.Copy();
            SubCategoriesComboBox.DataSource = SubCategoriesDT.DefaultView;
            SubCategoriesDT.DefaultView.RowFilter = "ContractorCategoryID = " + Convert.ToInt32(CategoriesComboBox.SelectedValue);

            Uncover();
        }

        private void NoCountryLabel_Click(object sender, EventArgs e)
        {
            AddItemForm AddItemForm = new AddItemForm(ref TopForm, "Введите название новой страны")
            {
                StartPosition = FormStartPosition.CenterParent
            };
            AddItemForm.ShowDialog();

            if (AddItemForm.bCanceled)
                return;

            Cover();

            Contractors.AddCountry(AddItemForm.sText);
            Contractors.RefillCountries();

            Uncover();
        }

        private void NoCityLabel_Click(object sender, EventArgs e)
        {
            AddItemForm AddItemForm = new AddItemForm(ref TopForm, "Введите название нового города")
            {
                StartPosition = FormStartPosition.CenterParent
            };
            AddItemForm.ShowDialog();

            if (AddItemForm.bCanceled)
                return;

            Cover();

            Contractors.AddCity(AddItemForm.sText, Convert.ToInt32(CountryComboBox.SelectedValue));
            Contractors.RefillCities();

            CityDT.Clear();
            CityDT = Contractors.CitiesDataTable.Copy();
            CityComboBox.DataSource = CityDT.DefaultView;
            CityDT.DefaultView.RowFilter = "CountryID = " + Convert.ToInt32(CountryComboBox.SelectedValue);

            Uncover();
        }

        private void ContactsLabel_Click(object sender, EventArgs e)
        {
            ContractorContactsForm ContractorContactsForm = new ContractorContactsForm(ref TopForm, ContactsDataTable, ContractorID)
            {
                StartPosition = FormStartPosition.CenterParent
            };
            ContractorContactsForm.ShowDialog();

            if (ContractorContactsForm.bCanceled)
                return;
        }
    }
}
