using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Infinium.Modules.Marketing.Clients
{
    public class Clients
    {
        PercentageDataGrid ClientsDataGrid = null;
        PercentageDataGrid ContactsDataGrid = null;
        PercentageDataGrid ShopAddressesDataGrid = null;

        public DataTable ClientRatesDataTable = null;
        public DataTable ClientsDataTable = null;
        public DataTable NewCountriesDataTable = null;
        public DataTable CountriesDataTable = null;
        public DataTable ManagersDataTable = null;
        public DataTable GetManagersDataTable = null;
        public DataTable ClientGroupsDataTable = null;

        public bool NewClient = false;

        private DataTable FrontsPriceGroupsDataTable = null;
        private DataTable DecorPriceGroupsDataTable = null;
        public DataTable ContactsDataTable = null;
        public DataTable NewContactsDataTable = null;

        public DataTable ShopAddressesDataTable = null;
        public DataTable NewShopAddressesDataTable = null;

        public SqlDataAdapter CountriesDataAdapter = null;
        public SqlDataAdapter ClientRatesDataAdapter = null;
        public SqlDataAdapter ClientsDataAdapter = null;
        public SqlDataAdapter ShopAddressesDataAdapter = null;

        public SqlCommandBuilder CountriesCommandBuilder = null;
        public SqlCommandBuilder ClientRatesCommandBuilder = null;
        public SqlCommandBuilder ClientsCommandBuilder = null;
        public SqlCommandBuilder ShopAddressesCommandBuilder = null;

        public BindingSource FrontsPriceGroupsBindingSource = null;
        public BindingSource DecorPriceGroupsBindingSource = null;
        public BindingSource ContactsBindingSource = null;
        public BindingSource NewContactsBindingSource = null;
        public BindingSource ClientRatesBindingSource = null;
        public BindingSource ClientsBindingSource = null;
        public BindingSource NewCountriesBindingSource = null;
        public BindingSource CountriesBindingSource = null;
        public BindingSource UsersBindingSource = null;
        public BindingSource ClientGroupsBindingSource = null;
        public BindingSource ShopAddressesBindingSource = null;
        public BindingSource NewShopAddressesBindingSource = null;

        public string ClientsBindingSourceDisplayMember = null;

        public string ClientsBindingSourceValueMember = null;


        public Clients(ref PercentageDataGrid tClientsDataGrid, ref PercentageDataGrid tContactsDataGrid, ref PercentageDataGrid tShopAddressesDataGrid)
        {
            ClientsDataGrid = tClientsDataGrid;
            ContactsDataGrid = tContactsDataGrid;
            ShopAddressesDataGrid = tShopAddressesDataGrid;

            Initialize();
        }

        private void Create()
        {
            prepareTranslit();
            FrontsPriceGroupsDataTable = new DataTable();
            DecorPriceGroupsDataTable = new DataTable();
            ClientRatesDataTable = new DataTable();
            ClientsDataTable = new DataTable();
            ClientGroupsDataTable = new DataTable();
            CountriesDataTable = new DataTable();
            ManagersDataTable = new DataTable();
            GetManagersDataTable = new DataTable();
            ShopAddressesDataTable = new DataTable();

            FrontsPriceGroupsBindingSource = new BindingSource();
            DecorPriceGroupsBindingSource = new BindingSource();
            ContactsBindingSource = new BindingSource();
            NewContactsBindingSource = new BindingSource();
            ClientRatesBindingSource = new BindingSource();
            ClientsBindingSource = new BindingSource();
            CountriesBindingSource = new BindingSource();
            UsersBindingSource = new BindingSource();
            ClientGroupsBindingSource = new BindingSource();
            ShopAddressesBindingSource = new BindingSource();
            NewShopAddressesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsPriceGroups",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(FrontsPriceGroupsDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorPriceGroups",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DecorPriceGroupsDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, Name, ShortName FROM ClientsManagers ORDER BY Name", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ManagersDataTable);
                {
                    DataRow NewRow = ManagersDataTable.NewRow();
                    NewRow["ManagerID"] = 0;
                    NewRow["Name"] = "-";
                    NewRow["ShortName"] = "-";
                    ManagersDataTable.Rows.InsertAt(NewRow, 0);
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsManagers ORDER BY Name", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(GetManagersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Countries ORDER BY Name",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CountriesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientGroups ORDER BY ClientGroupName",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientGroupsDataTable);
            }

            ClientRatesDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM ClientRates ORDER BY Date",
                ConnectionStrings.MarketingReferenceConnectionString);
            ClientRatesCommandBuilder = new SqlCommandBuilder(ClientRatesDataAdapter);
            ClientRatesDataAdapter.Fill(ClientRatesDataTable);

            ClientsDataAdapter = new SqlDataAdapter("SELECT * FROM Clients ORDER BY ClientName",
                ConnectionStrings.MarketingReferenceConnectionString);
            ClientsCommandBuilder = new SqlCommandBuilder(ClientsDataAdapter);
            ClientsDataAdapter.Fill(ClientsDataTable);

            ShopAddressesDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM ShopAddresses ORDER BY Address",
                ConnectionStrings.MarketingReferenceConnectionString);
            ShopAddressesCommandBuilder = new SqlCommandBuilder(ShopAddressesDataAdapter);
            ShopAddressesDataAdapter.Fill(ShopAddressesDataTable);
        }

        private void Binding()
        {
            FrontsPriceGroupsBindingSource.DataSource = FrontsPriceGroupsDataTable;
            DecorPriceGroupsBindingSource.DataSource = DecorPriceGroupsDataTable;

            ClientRatesBindingSource.DataSource = ClientRatesDataTable;
            ClientsBindingSource.DataSource = ClientsDataTable;
            CountriesBindingSource.DataSource = CountriesDataTable;
            UsersBindingSource.DataSource = ManagersDataTable;
            ClientGroupsBindingSource.DataSource = ClientGroupsDataTable;
            ContactsBindingSource.DataSource = ContactsDataTable;
            ShopAddressesBindingSource.DataSource = ShopAddressesDataTable;

            ClientsDataGrid.DataSource = ClientsBindingSource;
            ContactsDataGrid.DataSource = ContactsBindingSource;
            ShopAddressesDataGrid.DataSource = ShopAddressesBindingSource;


            ClientsBindingSourceDisplayMember = "ClientName";

            ClientsBindingSourceValueMember = "ClientID";
        }

        private void SetClientsDataGrid()
        {
            ClientsDataGrid.Columns.Add(CountryColumn);
            ClientsDataGrid.Columns.Add(ManagerColumn);
            ClientsDataGrid.Columns.Add(ClientGroupColumn);

            foreach (DataGridViewColumn Column in ClientsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            ClientsDataGrid.Columns["CountryID"].Visible = false;
            ClientsDataGrid.Columns["ManagerID"].Visible = false;
            ClientsDataGrid.Columns["Login"].Visible = false;
            ClientsDataGrid.Columns["Password"].Visible = false;
            ClientsDataGrid.Columns["Contacts"].Visible = false;
            ClientsDataGrid.Columns["ClientGroupID"].Visible = false;
            ClientsDataGrid.Columns["Online"].Visible = false;
            ClientsDataGrid.Columns["OnlineRefreshDateTime"].Visible = false;
            ClientsDataGrid.Columns["IdleTime"].Visible = false;
            ClientsDataGrid.Columns["TopMost"].Visible = false;
            ClientsDataGrid.Columns["TopModule"].Visible = false;
            ClientsDataGrid.Columns["Accountant"].Visible = false;
            ClientsDataGrid.Columns["Director"].Visible = false;
            ClientsDataGrid.Columns["DischargeAddress"].Visible = false;
            ClientsDataGrid.Columns["ClientFolderName"].Visible = false;

            ClientsDataGrid.Columns["Enabled"].HeaderText = "Активен";
            ClientsDataGrid.Columns["PriceGroup"].HeaderText = "Ценовая\r\nгруппа";
            ClientsDataGrid.Columns["DelayOfPayment"].HeaderText = "Отсрочка";
            ClientsDataGrid.Columns["ClientID"].HeaderText = "ID";
            ClientsDataGrid.Columns["ClientName"].HeaderText = "Название организации";
            ClientsDataGrid.Columns["Login"].HeaderText = "Логин";
            //ClientsDataGrid.Columns["Country"].HeaderText = "Страна";
            ClientsDataGrid.Columns["City"].HeaderText = "Город";
            ClientsDataGrid.Columns["Email"].HeaderText = "E-mail";
            ClientsDataGrid.Columns["Site"].HeaderText = "Сайт";
            ClientsDataGrid.Columns["UNN"].HeaderText = "УНН";
            ClientsDataGrid.Columns["NonStandard"].HeaderText = "Учет\r\nнестандарта";

            ClientsDataGrid.AutoGenerateColumns = false;

            ClientsDataGrid.Columns["PriceGroup"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["DelayOfPayment"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["Enabled"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["ClientID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsDataGrid.Columns["ClientID"].Width = 50;
            ClientsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["ClientName"].MinimumWidth = 150;
            ClientsDataGrid.Columns["ManagerColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["ManagerColumn"].MinimumWidth = 150;
            ClientsDataGrid.Columns["ClientGroupColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["ClientGroupColumn"].MinimumWidth = 150;
            ClientsDataGrid.Columns["CountryColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["CountryColumn"].MinimumWidth = 100;
            ClientsDataGrid.Columns["City"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["City"].MinimumWidth = 100;
            ClientsDataGrid.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["Email"].MinimumWidth = 100;
            ClientsDataGrid.Columns["UNN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["UNN"].MinimumWidth = 100;
            ClientsDataGrid.Columns["Site"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["Site"].MinimumWidth = 70;
            //ClientsDataGrid.Columns["Login"].MinimumWidth = 60;
            ClientsDataGrid.Columns["NonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsDataGrid.Columns["NonStandard"].Width = 120;

            int DisplayIndex = 0;
            ClientsDataGrid.Columns["ClientID"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["ClientGroupColumn"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["ManagerColumn"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["Login"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["CountryColumn"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["City"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["UNN"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["NonStandard"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["DelayOfPayment"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["PriceGroup"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["Enabled"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["Site"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["Email"].DisplayIndex = DisplayIndex++;

            ClientsDataGrid.Columns["ManagerColumn"].ReadOnly = true;
            ClientsDataGrid.Columns["ClientGroupColumn"].ReadOnly = true;
            ClientsDataGrid.Columns["ClientID"].ReadOnly = true;
            ClientsDataGrid.Columns["ClientName"].ReadOnly = true;
            ClientsDataGrid.Columns["Login"].ReadOnly = true;
            ClientsDataGrid.Columns["CountryColumn"].ReadOnly = true;
            ClientsDataGrid.Columns["City"].ReadOnly = true;
            ClientsDataGrid.Columns["Email"].ReadOnly = true;
            ClientsDataGrid.Columns["UNN"].ReadOnly = true;
            ClientsDataGrid.Columns["Site"].ReadOnly = true;
            ClientsDataGrid.Columns["NonStandard"].ReadOnly = true;
            ClientsDataGrid.Columns["DelayOfPayment"].ReadOnly = true;
            //ClientsDataGrid.Columns["ManagerColumn"].ReadOnly = false;
        }

        private DataGridViewComboBoxColumn ManagerColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ManagerColumn",
                    HeaderText = "Кто ведёт",
                    DataPropertyName = "ManagerID",

                    DataSource = new DataView(ManagersDataTable),
                    ValueMember = "ManagerID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        private DataGridViewComboBoxColumn ClientGroupColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientGroupColumn",
                    HeaderText = "Группа",
                    DataPropertyName = "ClientGroupID",

                    DataSource = new DataView(ClientGroupsDataTable),
                    ValueMember = "ClientGroupID",
                    DisplayMember = "ClientGroupName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        private DataGridViewComboBoxColumn CountryColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CountryColumn",
                    HeaderText = "Страна",
                    DataPropertyName = "CountryID",

                    DataSource = new DataView(CountriesDataTable),
                    ValueMember = "CountryID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        private void SetContactsDataGrid()
        {
            ContactsDataGrid.Columns["Name"].HeaderText = "Имя";
            ContactsDataGrid.Columns["Position"].HeaderText = "Должность";
            ContactsDataGrid.Columns["Phone"].HeaderText = "Телефон";
            ContactsDataGrid.Columns["Email"].HeaderText = "E-mail";
            ContactsDataGrid.Columns["ICQ"].HeaderText = "ICQ";
            ContactsDataGrid.Columns["Notes"].HeaderText = "Примечание";

            ContactsDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            ShopAddressesDataGrid.Columns["ShopAddressID"].Visible = false;
            ShopAddressesDataGrid.Columns["ClientID"].Visible = false;
            ShopAddressesDataGrid.Columns["Address"].HeaderText = "Адрес";
            ShopAddressesDataGrid.Columns["City"].HeaderText = "Город";
            ShopAddressesDataGrid.Columns["Country"].HeaderText = "Страна";
            ShopAddressesDataGrid.Columns["Lat"].HeaderText = "Широта";
            ShopAddressesDataGrid.Columns["Long"].HeaderText = "Долгота";
            ShopAddressesDataGrid.Columns["Worktime"].HeaderText = "Время работы";
            ShopAddressesDataGrid.Columns["Name"].HeaderText = "Название";
            ShopAddressesDataGrid.Columns["Site"].HeaderText = "Сайт";
            ShopAddressesDataGrid.Columns["IsFronts"].HeaderText = "Фасады";
            ShopAddressesDataGrid.Columns["IsProfile"].HeaderText = "Погонаж";
            ShopAddressesDataGrid.Columns["IsFurniture"].HeaderText = "Мебель";
            ShopAddressesDataGrid.Columns["Email"].HeaderText = "Email";
            ShopAddressesDataGrid.Columns["Phone"].HeaderText = "Телефон";
            ShopAddressesDataGrid.Columns["WebSite"].HeaderText = "Сайт";

            ShopAddressesDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
            ShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.Font = new System.Drawing.Font("SEGOE UI", 13.0F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
        }

        private void Initialize()
        {
            Create();
            Fill();
            CreateContactsDataTable();
            Binding();
            SetClientsDataGrid();
            SetContactsDataGrid();
        }

        private string GetContactsXML(DataTable DT)
        {
            StringWriter SW = new StringWriter();
            DT.WriteXml(SW);

            return SW.ToString();
        }

        public void FillContactsDataTable(int ClientID)
        {
            ContactsDataTable.Clear();

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);

            if (Rows[0]["Contacts"].ToString().Length == 0)
                return;

            string ContactXML = Rows[0]["Contacts"].ToString();

            using (StringReader SR = new StringReader(ContactXML))
            {
                ContactsDataTable.ReadXml(SR);
            }
        }

        public void FillShopAddressesDataTable(int ClientID)
        {
            ShopAddressesDataAdapter.Dispose();
            ShopAddressesCommandBuilder.Dispose();
            ShopAddressesDataAdapter = new SqlDataAdapter("SELECT * FROM ShopAddresses WHERE ClientID=" + ClientID + " ORDER BY Address",
                ConnectionStrings.MarketingReferenceConnectionString);
            ShopAddressesCommandBuilder = new SqlCommandBuilder(ShopAddressesDataAdapter);
            ShopAddressesDataTable.Clear();
            ShopAddressesDataAdapter.Fill(ShopAddressesDataTable);
        }

        private void CreateContactsDataTable()
        {
            ContactsDataTable = new DataTable()
            {
                TableName = "ContactsDataTable"
            };
            ContactsDataTable.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ContactsDataTable.Columns.Add(new DataColumn("Position", Type.GetType("System.String")));
            ContactsDataTable.Columns.Add(new DataColumn("Phone", Type.GetType("System.String")));
            ContactsDataTable.Columns.Add(new DataColumn("Email", Type.GetType("System.String")));
            ContactsDataTable.Columns.Add(new DataColumn("ICQ", Type.GetType("System.String")));
            ContactsDataTable.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
        }

        private bool GetManagerInfo(int ManagerID, ref string Name, ref string Phone, ref string Email, ref string Skype)
        {
            DataRow[] Rows = GetManagersDataTable.Select("ManagerID = " + ManagerID);
            if (Rows.Count() > 0)
            {
                Name = Rows[0]["Name"].ToString();
                Phone = Rows[0]["Phone"].ToString();
                Email = Rows[0]["Email"].ToString();
                Skype = Rows[0]["Skype"].ToString();
                if (Phone.Count() == 0 && Email.Count() == 0 && Skype.Count() == 0)
                    return false;
                return true;
            }
            else
                return false;
        }


        public string GetClientName(int ClientID)
        {
            DataRow[] Row = ClientsDataTable.Select("ClientID = " + ClientID);

            return Row[0]["ClientName"].ToString();
        }

        public void CreateNewContactsDataTable(ref PercentageDataGrid DataGrid, ref PercentageDataGrid tShopAddressesDataGrid)
        {
            if (NewContactsDataTable == null)
            {
                NewContactsDataTable = new DataTable();
                NewContactsDataTable = ContactsDataTable.Clone();
            }
            else
                NewContactsDataTable.Clear();

            NewContactsBindingSource.DataSource = NewContactsDataTable;
            DataGrid.DataSource = NewContactsBindingSource;

            DataGrid.Columns["Name"].HeaderText = "Имя";
            DataGrid.Columns["Position"].HeaderText = "Должность";
            DataGrid.Columns["Phone"].HeaderText = "Телефон";
            DataGrid.Columns["Email"].HeaderText = "E-mail";
            DataGrid.Columns["ICQ"].HeaderText = "ICQ";
            DataGrid.Columns["Notes"].HeaderText = "Примечание";

            DataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            if (NewShopAddressesDataTable == null)
            {
                NewShopAddressesDataTable = new DataTable();
                NewShopAddressesDataTable = ShopAddressesDataTable.Clone();
            }
            else
                NewShopAddressesDataTable.Clear();

            NewShopAddressesBindingSource.DataSource = NewShopAddressesDataTable;
            tShopAddressesDataGrid.DataSource = NewShopAddressesBindingSource;

            tShopAddressesDataGrid.Columns["ShopAddressID"].Visible = false;
            tShopAddressesDataGrid.Columns["ClientID"].Visible = false;
            tShopAddressesDataGrid.Columns["Address"].HeaderText = "Адрес";
            tShopAddressesDataGrid.Columns["City"].HeaderText = "Город";
            tShopAddressesDataGrid.Columns["Country"].HeaderText = "Страна";
            tShopAddressesDataGrid.Columns["Lat"].HeaderText = "Широта";
            tShopAddressesDataGrid.Columns["Long"].HeaderText = "Долгота";
            tShopAddressesDataGrid.Columns["Worktime"].HeaderText = "Время работы";
            tShopAddressesDataGrid.Columns["Name"].HeaderText = "Название";
            tShopAddressesDataGrid.Columns["Site"].HeaderText = "Сайт";
            tShopAddressesDataGrid.Columns["IsFronts"].HeaderText = "Фасады";
            tShopAddressesDataGrid.Columns["IsProfile"].HeaderText = "Погонаж";
            tShopAddressesDataGrid.Columns["IsFurniture"].HeaderText = "Мебель";
            tShopAddressesDataGrid.Columns["Email"].HeaderText = "Email";
            tShopAddressesDataGrid.Columns["Phone"].HeaderText = "Телефон";
            tShopAddressesDataGrid.Columns["WebSite"].HeaderText = "Сайт";

            tShopAddressesDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //tShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
            //tShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.Font = new System.Drawing.Font("SEGOE UI", 13.0F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
        }


        public void AddClient(string Name, int CountryID, string City, int ClientGroupID, string Site, string Email, int ManagerID, string UNN,
                              int NonStandard, decimal PriceGroup, int DelayOfPayment, bool Enabled)
        {
            string Contacts = GetContactsXML(NewContactsDataTable);

            string Login = GetTranslit(Name);
            DataRow Row = ClientsDataTable.NewRow();
            Row["ManagerID"] = ManagerID;
            Row["Login"] = Login;
            Row["Password"] = "b59c67bf196a4758191e42f76670ceba";
            Row["ClientName"] = Name;
            Row["ClientFolderName"] = Name.Replace('"', ' ');
            Row["CountryID"] = CountryID;
            Row["ClientGroupID"] = ClientGroupID;
            Row["City"] = City;
            Row["Token"] = GenAccessToken(Name);

            if (Email.Length > 0)
                Row["Email"] = Email;

            if (Contacts.Length > 0)
                Row["Site"] = Site;

            if (UNN != string.Empty)
                Row["UNN"] = UNN;
            else
                Row["UNN"] = DBNull.Value;

            Row["Contacts"] = Contacts;
            Row["NonStandard"] = NonStandard;
            Row["PriceGroup"] = PriceGroup;
            Row["DelayOfPayment"] = DelayOfPayment;
            Row["Enabled"] = Enabled;

            ClientsDataTable.Rows.Add(Row);
            ClientsDataAdapter.Update(ClientsDataTable);
            ClientsDataTable.Clear();
            ClientsDataAdapter.Fill(ClientsDataTable);
        }

        public void EditClient(ref string ClientsName, ref int ClientsCountryID, ref string ClientsCity, ref int ClientGroupID,
                               ref string ClientsEmail, ref string ClientsSite, ref string UNN, ref int ManagerID, ref decimal PriceGroup, ref int DelayOfPayment, ref bool Enabled)
        {
            ClientsName = ((DataRowView)ClientsBindingSource.Current)["ClientName"].ToString();
            ClientsCountryID = Convert.ToInt32(((DataRowView)ClientsBindingSource.Current)["CountryID"]);
            ClientGroupID = Convert.ToInt32(((DataRowView)ClientsBindingSource.Current)["ClientGroupID"]);
            ClientsCity = ((DataRowView)ClientsBindingSource.Current)["City"].ToString();
            ClientsEmail = ((DataRowView)ClientsBindingSource.Current)["Email"].ToString();
            ClientsSite = ((DataRowView)ClientsBindingSource.Current)["Site"].ToString();
            UNN = ((DataRowView)ClientsBindingSource.Current)["UNN"].ToString();
            ManagerID = Convert.ToInt32(((DataRowView)ClientsBindingSource.Current)["ManagerID"]);
            PriceGroup = Convert.ToDecimal(((DataRowView)ClientsBindingSource.Current)["PriceGroup"]);
            DelayOfPayment = Convert.ToInt32(((DataRowView)ClientsBindingSource.Current)["DelayOfPayment"]);
            Enabled = Convert.ToBoolean(((DataRowView)ClientsBindingSource.Current)["Enabled"]);

            NewContactsDataTable.Clear();
            NewContactsDataTable = ContactsDataTable.Copy();

            NewContactsBindingSource.DataSource = NewContactsDataTable;

            NewShopAddressesDataTable.Clear();
            NewShopAddressesDataTable = ShopAddressesDataTable.Copy();
            NewShopAddressesBindingSource.DataSource = NewShopAddressesDataTable;
        }

        public void SaveClient(string Name, int CountryID, string City, int ClientGroupID, string Site, string Email, int ManagerID, string UNN,
                               int NonStandard, decimal PriceGroup, int DelayOfPayment, bool Enabled, int ClientID)
        {
            string Contacts = GetContactsXML(NewContactsDataTable);

            int index = ClientsBindingSource.Position;

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Clients WHERE ClientID = " + ClientID + " ORDER BY ClientName",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows[0]["Token"] == DBNull.Value)
                                DT.Rows[0]["Token"] = GenAccessToken(Name);

                            DT.Rows[0]["ClientName"] = Name;
                            DT.Rows[0]["CountryID"] = CountryID;
                            DT.Rows[0]["ClientGroupID"] = ClientGroupID;
                            DT.Rows[0]["City"] = City;

                            if (Email.Length > 0)
                                DT.Rows[0]["Email"] = Email;

                            if (Contacts.Length > 0)
                                DT.Rows[0]["Site"] = Site;
                            if (UNN != string.Empty)
                                DT.Rows[0]["UNN"] = UNN;
                            else
                                DT.Rows[0]["UNN"] = DBNull.Value;

                            DT.Rows[0]["Contacts"] = Contacts;
                            DT.Rows[0]["NonStandard"] = NonStandard;
                            DT.Rows[0]["PriceGroup"] = PriceGroup;
                            DT.Rows[0]["DelayOfPayment"] = DelayOfPayment;
                            DT.Rows[0]["Enabled"] = Enabled;

                            //int OldManagerID = 0;
                            //if (DT.Rows[0]["ManagerID"] != DBNull.Value)
                            //{
                            //    OldManagerID = Convert.ToInt32(DT.Rows[0]["ManagerID"]);
                            //    if (OldManagerID != ManagerID && ManagerID != 0)
                            //    {
                            //    }
                            //}
                            DT.Rows[0]["ManagerID"] = ManagerID;
                            DA.Update(DT);
                            ClientsDataTable.Clear();
                            ClientsDataAdapter.Fill(ClientsDataTable);
                            ClientsBindingSource.Position = ClientsBindingSource.Find("ClientID", ClientID);
                        }
                    }
                }
            }
        }

        private string SendNotifyToEmail(string ClientEmail, string Name, string Phone, string Email, string Skype)
        {
            string result = "Уведомление на почту клиента отправлено";
            string AccountPassword = "3699PassWord14772588";
            string SenderEmail = "infiniumdevelopers@gmail.com";


            string to = ClientEmail;
            string from = SenderEmail;

            if (to.Length == 0)
            {
                result = "У клиента не указан Email. Отправка отчета невозможна";
                MessageBox.Show(result);
                return result;
            }

            string messageBody = "Уважаемые клиенты! Сообщаем Вам о назначении нового менеджера, курирующего Вашу компанию - " + Name + ".\r\n";
            if (Phone.Count() > 0 || Email.Count() > 0 || Skype.Count() > 0)
                messageBody += "Обращайтесь по следующим контактам:\r\n";
            if (Phone.Count() > 0)
                messageBody += "Тел: " + Phone + "\r\n";
            if (Email.Count() > 0)
                messageBody += "Эл. почта: " + Email + "\r\n";
            if (Skype.Count() > 0)
                messageBody += "Скайп: " + Skype;

            using (MailMessage message = new MailMessage(from, to))
            {
                message.Subject = "О смене ответственного лица";
                message.Body = messageBody;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SenderEmail, AccountPassword)
                };
                try
                {
                    client.Send(message);
                }

                catch (ArgumentException ex)
                {
                    MessageBox.Show("ArgumentException\r\n" + ex.Message);
                    result = "ArgumentException\r\n" + ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("InvalidOperationException\r\n" + ex.Message);
                    result = "InvalidOperationException\r\n" + ex.Message;
                }
                catch (SmtpException ex)
                {
                    MessageBox.Show("SmtpException\r\n" + ex.Message);
                    result = "SmtpException\r\n" + ex.Message;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception\r\n" + ex.Message);
                    result = "Exception\r\n" + ex.Message;
                }

                client.Dispose();
            }
            return result;
        }

        public string NotifyClient(string ClientEmail, int ManagerID)
        {
            string ManagerName = string.Empty;
            string ManagerPhone = string.Empty;
            string ManagerEmail = string.Empty;
            string ManagerSkype = string.Empty;
            string result = string.Empty;
            if (!GetManagerInfo(ManagerID, ref ManagerName, ref ManagerPhone, ref ManagerEmail, ref ManagerSkype))
                result = "Нет сведений о менеджере. Заполните информацию в модуле \r\n" +
                        "\"Редактор менеджеров\" и повторите отправку уведомления";
            else
                result = SendNotifyToEmail(ClientEmail, ManagerName, ManagerPhone, ManagerEmail, ManagerSkype);

            return result;
        }

        private string GetMD5(string text)
        {
            using (System.Security.Cryptography.MD5 Hasher = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = Hasher.ComputeHash(Encoding.Default.GetBytes(text));

                StringBuilder sBuilder = new StringBuilder();

                //преобразование в HEX
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public string GenAccessToken(string Name)
        {
            string AccessToken = string.Empty;
            if (Name.Length > 0)
            {
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                AccessToken = GetMD5(Name) + GetMD5(GuidString);
            }
            return AccessToken;
        }

        public void SaveShopAddresses()
        {
            ShopAddressesDataAdapter.Update(NewShopAddressesDataTable);
            ShopAddressesDataTable.Clear();
            ShopAddressesDataAdapter.Fill(ShopAddressesDataTable);
        }

        public void GetClientRates(int ClientID)
        {
            ClientRatesDataTable.Clear();
            ClientRatesDataAdapter = new SqlDataAdapter("SELECT * FROM ClientRates WHERE ClientID=" + ClientID + " ORDER BY Date",
                ConnectionStrings.MarketingReferenceConnectionString);
            ClientRatesCommandBuilder = new SqlCommandBuilder(ClientRatesDataAdapter);
            ClientRatesDataTable.Clear();
            ClientRatesDataAdapter.Fill(ClientRatesDataTable);
        }

        public void SaveClientRates()
        {
            ClientRatesDataAdapter.Update(ClientRatesDataTable);
            ClientRatesDataTable.Clear();
            ClientRatesDataAdapter.Fill(ClientRatesDataTable);
        }

        public void GetCountries()
        {
            if (NewCountriesDataTable == null)
            {
                NewCountriesDataTable = new DataTable();
                NewCountriesBindingSource = new BindingSource
                {
                    DataSource = NewCountriesDataTable
                };
            }
            NewCountriesDataTable.Clear();
            CountriesDataAdapter = new SqlDataAdapter("SELECT * FROM Countries ORDER BY Name",
                ConnectionStrings.CatalogConnectionString);
            CountriesCommandBuilder = new SqlCommandBuilder(CountriesDataAdapter);
            CountriesDataAdapter.Fill(NewCountriesDataTable);
        }

        public void UpdateCountries()
        {
            CountriesDataTable.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Countries ORDER BY Name",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CountriesDataTable);
            }
        }

        public void SaveCountries()
        {
            CountriesDataAdapter.Update(NewCountriesDataTable);
            NewCountriesDataTable.Clear();
            CountriesDataAdapter.Fill(NewCountriesDataTable);
        }

        public void AddCountry(string Name)
        {
            DataRow Row = NewCountriesDataTable.NewRow();
            Row["Name"] = Name;
            NewCountriesDataTable.Rows.Add(Row);
        }

        private Dictionary<string, string> transliter = new Dictionary<string, string>();

        private void prepareTranslit()
        {
            transliter.Add("а", "a");
            transliter.Add("б", "b");
            transliter.Add("в", "v");
            transliter.Add("г", "g");
            transliter.Add("д", "d");
            transliter.Add("е", "e");
            transliter.Add("ё", "yo");
            transliter.Add("ж", "zh");
            transliter.Add("з", "z");
            transliter.Add("и", "i");
            transliter.Add("й", "j");
            transliter.Add("к", "k");
            transliter.Add("л", "l");
            transliter.Add("м", "m");
            transliter.Add("н", "n");
            transliter.Add("о", "o");
            transliter.Add("п", "p");
            transliter.Add("р", "r");
            transliter.Add("с", "s");
            transliter.Add("т", "t");
            transliter.Add("у", "u");
            transliter.Add("ф", "f");
            transliter.Add("х", "h");
            transliter.Add("ц", "c");
            transliter.Add("ч", "ch");
            transliter.Add("ш", "sh");
            transliter.Add("щ", "sch");
            transliter.Add("ъ", "j");
            transliter.Add("ы", "i");
            transliter.Add("ь", "j");
            transliter.Add("э", "e");
            transliter.Add("ю", "yu");
            transliter.Add("я", "ya");
            transliter.Add("А", "A");
            transliter.Add("Б", "B");
            transliter.Add("В", "V");
            transliter.Add("Г", "G");
            transliter.Add("Д", "D");
            transliter.Add("Е", "E");
            transliter.Add("Ё", "Yo");
            transliter.Add("Ж", "Zh");
            transliter.Add("З", "Z");
            transliter.Add("И", "I");
            transliter.Add("Й", "J");
            transliter.Add("К", "K");
            transliter.Add("Л", "L");
            transliter.Add("М", "M");
            transliter.Add("Н", "N");
            transliter.Add("О", "O");
            transliter.Add("П", "P");
            transliter.Add("Р", "R");
            transliter.Add("С", "S");
            transliter.Add("Т", "T");
            transliter.Add("У", "U");
            transliter.Add("Ф", "F");
            transliter.Add("Х", "H");
            transliter.Add("Ц", "C");
            transliter.Add("Ч", "Ch");
            transliter.Add("Ш", "Sh");
            transliter.Add("Щ", "Sch");
            transliter.Add("Ъ", "J");
            transliter.Add("Ы", "I");
            transliter.Add("Ь", "J");
            transliter.Add("Э", "E");
            transliter.Add("Ю", "Yu");
            transliter.Add("Я", "Ya");
        }

        public string GetTranslit(string sourceText)
        {
            StringBuilder ans = new StringBuilder();
            for (int i = 0; i < sourceText.Length; i++)
            {
                if (transliter.ContainsKey(sourceText[i].ToString()))
                {
                    ans.Append(transliter[sourceText[i].ToString()]);
                }
                else
                {
                    ans.Append(sourceText[i].ToString());
                }
            }
            return ans.ToString();
        }

        public void Send(string ClientEmail, string Login)
        {
            string AccountPassword = "3699PassWord14772588";
            string SenderEmail = "infiniumdevelopers@gmail.com";


            string to = ClientEmail;
            string from = SenderEmail;

            if (to.Length == 0)
            {
                MessageBox.Show("У клиента не указан Email. Отправка отчета невозможна");
                return;
            }

            using (MailMessage message = new MailMessage(from, to))
            {
                message.Subject = "InfiniumAgent";
                message.Body = @"Здравствуйте. Отдел разработки программного обеспечения СООО «ЗОВ-Профиль» предлагает Вам установить десктоп-приложение Infinium.Agent, которое является частью корпоративной системы управления ресурсами предприятия Infinium. В этой программе Вы сможете видеть состояние заказов Ваших клиентов на нашей фабрике в производстве, на складе и отгрузке. Кроме того, можете обмениваться сообщениями с нашими сотрудниками, читать и комментировать новости нашего предприятия. В программе также имеется актуальный каталог продукции. База данных находится на облачном хостинге, поэтому работает круглосуточно и вероятность сбоев сведена к минимуму. Скачать приложение можете по ссылке:
https://drive.google.com/file/d/0BzOa6U366p2pOFRESEdUN2hQSGc/view?usp=sharing
Для входа в программу используйте логин " + Login + @" и пароль 1111. После авторизации рекомендуем сменить данные для входа.

--
С уважением, отдел разработки программного обеспечения СООО «ЗОВ - Профиль», Республика Беларусь, г. Гродно
infiniumdevelopers@gmail.com";
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SenderEmail, AccountPassword)
                };
                try
                {
                    client.Send(message);
                }

                catch (ArgumentException ex)
                {
                    MessageBox.Show("ArgumentException\r\n" + ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("InvalidOperationException\r\n" + ex.Message);
                }
                catch (SmtpException ex)
                {
                    MessageBox.Show("SmtpException\r\n" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception\r\n" + ex.Message);
                }

                client.Dispose();
            }
        }

        public void GetFixedPaymentRate(int ClientID, DateTime ConfirmDateTime, ref bool FixedPaymentRate, ref decimal USD, ref decimal RUB, ref decimal BYN)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ClientRates WHERE CAST(Date AS Date) <= 
                    '" + ConfirmDateTime.ToString("yyyy-MM-dd") + "' AND ClientID = " + ClientID + " ORDER BY Date DESC",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        if (DT.Rows[0]["USD"] == DBNull.Value || DT.Rows[0]["RUB"] == DBNull.Value || DT.Rows[0]["BYN"] == DBNull.Value)
                            FixedPaymentRate = false;
                        else
                        {
                            FixedPaymentRate = true;
                            USD = Convert.ToDecimal(DT.Rows[0]["USD"]);
                            RUB = Convert.ToDecimal(DT.Rows[0]["RUB"]);
                            BYN = Convert.ToDecimal(DT.Rows[0]["BYN"]);
                        }
                    }
                    else
                        FixedPaymentRate = false;
                }
            }
            return;
        }

        public void GetDateRates(DateTime date, ref decimal EURBYRCurrency)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DateRates WHERE CAST(Date AS Date) = 
                    '" + date.ToString("yyyy-MM-dd") + "'",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        EURBYRCurrency = Convert.ToDecimal(DT.Rows[0]["BYN"]);
                    }
                }
            }
            return;
        }

        //public bool NBRBDailyRates(DateTime date, ref decimal EURBYRCurrency)
        //{
        //    string EuroXML = "";
        //    string url = "http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM/dd/yyyy");

        //    HttpWebRequest myHttpWebRequest;
        //    HttpWebResponse myHttpWebResponse;

        //    try
        //    {
        //          ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        //        myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        //        myHttpWebRequest.KeepAlive = false;
        //        myHttpWebRequest.AllowAutoRedirect = true;
        //        CookieContainer cookieContainer = new CookieContainer();
        //        myHttpWebRequest.CookieContainer = cookieContainer;
        //        myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    XmlTextReader reader = new XmlTextReader(myHttpWebResponse.GetResponseStream());
        //    //XmlTextReader reader = new XmlTextReader("http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM/dd/yyyy"));
        //    try
        //    {
        //        while (reader.Read())
        //        {
        //            switch (reader.NodeType)
        //            {
        //                case XmlNodeType.Element:
        //                    if (reader.Name == "Currency")
        //                    {
        //                        if (reader.HasAttributes)
        //                        {
        //                            while (reader.MoveToNextAttribute())
        //                            {
        //                                if (reader.Name == "Id")
        //                                {
        //                                    if (reader.Value == "292")
        //                                    {
        //                                        reader.MoveToElement();
        //                                        EuroXML = reader.ReadOuterXml();
        //                                    }
        //                                }
        //                                if (reader.Name == "Id")
        //                                {
        //                                    if (reader.Value == "19")
        //                                    {
        //                                        reader.MoveToElement();
        //                                        EuroXML = reader.ReadOuterXml();
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    break;
        //            }
        //        }
        //        XmlDocument euroXmlDocument = new XmlDocument();
        //        euroXmlDocument.LoadXml(EuroXML);
        //        XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Currency/Rate");
        //        bool b = decimal.TryParse(xmlNode.InnerText, out EURBYRCurrency);
        //        if (!b)
        //            EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
        //        else
        //            EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText);
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message + " . КУРСЫ МОЖНО БУДЕТ ВВЕСТИ ВРУЧНУЮ");
        //        return false;
        //    }
        //    return true;
        //}

        public bool CBRDailyRates(DateTime date, ref decimal EURRUBCurrency, ref decimal USDRUBCurrency)
        {
            string EuroXML = "";
            string USDXml = "";

            string url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date.ToString("dd/MM/yyyy");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.AllowAutoRedirect = true;
                CookieContainer cookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer = cookieContainer;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            XmlTextReader reader1 = new XmlTextReader(myHttpWebResponse.GetResponseStream());

            try
            {
                while (reader1.Read())
                {
                    switch (reader1.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader1.Name == "Valute")
                            {
                                if (reader1.HasAttributes)
                                {
                                    while (reader1.MoveToNextAttribute())
                                    {
                                        if (reader1.Name == "ID")
                                        {
                                            if (reader1.Value == "R01239")
                                            {
                                                reader1.MoveToElement();
                                                EuroXML = reader1.ReadOuterXml();
                                            }
                                        }
                                        if (reader1.Name == "ID")
                                        {
                                            if (reader1.Value == "R01235")
                                            {
                                                reader1.MoveToElement();
                                                USDXml = reader1.ReadOuterXml();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                XmlDocument usdXmlDocument = new XmlDocument();
                usdXmlDocument.LoadXml(USDXml);

                XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Valute/Value");
                EURRUBCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
                xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");
                USDRUBCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message + " . КУРСЫ МОЖНО БУДЕТ ВВЕСТИ ВРУЧНУЮ");
                return false;
            }
            return true;
        }

        public void FixOrderEvent(int ClientID, string Event)
        {
            string SelectCommand = @"SELECT TOP 0 * FROM NewMegaOrdersEvents";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["MegaOrderID"] = ClientID;
                        NewRow["Event"] = Event;
                        NewRow["EventDate"] = Security.GetCurrentDate();
                        NewRow["EventUserID"] = Security.CurrentUserID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public void RemoveClient(int ClientID)
        {
            using (var da = new SqlDataAdapter("DELETE FROM Clients" +
                " WHERE ClientID = " + ClientID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (DataTable dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
        }

        public void RemoveClientOrders(int ClientID)
        {
            using (var da = new SqlDataAdapter("DELETE FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID =" + ClientID + ")))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }

            using (var da = new SqlDataAdapter("DELETE FROM NewFrontsOrders WHERE MainOrderID IN (SELECT MainOrderID FROM NewMainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMegaOrders WHERE ClientID =" + ClientID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
            using (var da = new SqlDataAdapter("DELETE FROM FrontsOrders WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID =" + ClientID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
            using (var da = new SqlDataAdapter("DELETE FROM NewDecorOrders WHERE MainOrderID IN (SELECT MainOrderID FROM NewMainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMegaOrders WHERE ClientID =" + ClientID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
            using (var da = new SqlDataAdapter("DELETE FROM DecorOrders WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID =" + ClientID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }

            using (var da = new SqlDataAdapter("DELETE FROM BatchDetails WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID =" + ClientID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
            using (var da = new SqlDataAdapter("DELETE FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID =" + ClientID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }

            using (var da = new SqlDataAdapter("DELETE FROM NewMainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM NewMegaOrders WHERE ClientID =" + ClientID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
            using (var da = new SqlDataAdapter("DELETE FROM MainOrders" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID =" + ClientID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }

            using (var da = new SqlDataAdapter("DELETE FROM NewMegaOrders" +
                " WHERE ClientID = " + ClientID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (DataTable dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
            using (var da = new SqlDataAdapter("DELETE FROM MegaOrders" +
                " WHERE ClientID = " + ClientID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (var dtTable = new DataTable())
                    {
                        da.Fill(dtTable);
                    }
                }
            }
        }

        DataTable dtRolePermissions;

        public void GetPermissions(int UserID, string FormName)
        {
            if (dtRolePermissions == null)
                dtRolePermissions = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + UserID +
                " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                " (SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'))", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(dtRolePermissions);
            }
        }

        public bool PermissionGranted(int RoleID)
        {
            DataRow[] Rows = dtRolePermissions.Select("RoleID = " + RoleID);
            return Rows.Count() > 0;
        }

    }

}
