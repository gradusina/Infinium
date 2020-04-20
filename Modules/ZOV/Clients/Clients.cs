using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Infinium.Modules.ZOV.Clients
{
    public class Clients
    {
        PercentageDataGrid ClientsDataGrid = null;
        PercentageDataGrid ContactsDataGrid = null;
        PercentageDataGrid ShopAddressesDataGrid = null;

        DataTable ClientsGroupsDataTable = null;
        DataTable ClientsDataTable = null;
        DataTable ManagersDataTable = null;

        public bool NewClient = false;

        public DataTable ContactsDataTable = null;
        public DataTable NewContactsDataTable = null;

        public DataTable ShopAddressesDataTable = null;
        public DataTable NewShopAddressesDataTable = null;

        public SqlDataAdapter ClientsGroupsDataAdapter = null;
        public SqlDataAdapter ClientsDataAdapter = null;
        public SqlDataAdapter ShopAddressesDataAdapter = null;

        public SqlCommandBuilder ClientsGroupsCommandBuilder = null;
        public SqlCommandBuilder ClientsCommandBuilder = null;
        public SqlCommandBuilder ShopAddressesCommandBuilder = null;

        public BindingSource ContactsBindingSource = null;
        public BindingSource NewContactsBindingSource = null;
        public BindingSource ClientsGroupsBindingSource = null;
        public BindingSource ClientsBindingSource = null;
        public BindingSource ManagersBindingSource = null;
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
            ClientsGroupsDataTable = new DataTable();
            ClientsDataTable = new DataTable();
            ManagersDataTable = new DataTable();
            ShopAddressesDataTable = new DataTable();

            ContactsBindingSource = new BindingSource();
            NewContactsBindingSource = new BindingSource();
            ClientsGroupsBindingSource = new BindingSource();
            ClientsBindingSource = new BindingSource();
            ManagersBindingSource = new BindingSource();
            ShopAddressesBindingSource = new BindingSource();
            NewShopAddressesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, Name FROM Managers ORDER BY Name", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ManagersDataTable);
            }

            ClientsGroupsDataAdapter = new SqlDataAdapter("SELECT * FROM ClientsGroups ORDER BY ClientGroupName",
                ConnectionStrings.ZOVReferenceConnectionString);
            ClientsGroupsCommandBuilder = new SqlCommandBuilder(ClientsGroupsDataAdapter);
            ClientsGroupsDataAdapter.Fill(ClientsGroupsDataTable);

            ClientsDataAdapter = new SqlDataAdapter("SELECT * FROM Clients ORDER BY ClientName",
                ConnectionStrings.ZOVReferenceConnectionString);
            ClientsCommandBuilder = new SqlCommandBuilder(ClientsDataAdapter);
            ClientsDataAdapter.Fill(ClientsDataTable);

            ShopAddressesDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM ShopAddresses ORDER BY Address",
                ConnectionStrings.ZOVReferenceConnectionString);
            ShopAddressesCommandBuilder = new SqlCommandBuilder(ShopAddressesDataAdapter);
            ShopAddressesDataAdapter.Fill(ShopAddressesDataTable);
        }

        private void Binding()
        {
            ClientsGroupsBindingSource.DataSource = ClientsGroupsDataTable;
            ClientsBindingSource.DataSource = ClientsDataTable;
            ManagersBindingSource.DataSource = ManagersDataTable;
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
            ClientsDataGrid.Columns.Add(ClientsGroupsColumn);
            ClientsDataGrid.Columns.Add(ManagerColumn);

            foreach (DataGridViewColumn Column in ClientsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            ClientsDataGrid.Columns["ClientGroupID"].Visible = false;
            ClientsDataGrid.Columns["ManagerID"].Visible = false;
            ClientsDataGrid.Columns["Contacts"].Visible = false;
            ClientsDataGrid.Columns["MoveOk"].Visible = false;

            ClientsDataGrid.Columns["ClientID"].HeaderText = "ID";
            ClientsDataGrid.Columns["ClientName"].HeaderText = "Название организации";

            ClientsDataGrid.AutoGenerateColumns = false;

            ClientsDataGrid.Columns["ClientID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsDataGrid.Columns["ClientID"].Width = 50;
            //ClientsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //ClientsDataGrid.Columns["ClientName"].MinimumWidth = 150;
            //ClientsDataGrid.Columns["ManagerColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //ClientsDataGrid.Columns["ManagerColumn"].MinimumWidth = 150;
            //ClientsDataGrid.Columns["ClientsGroupsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //ClientsDataGrid.Columns["ClientsGroupsColumn"].MinimumWidth = 150;

            int DisplayIndex = 0;
            ClientsDataGrid.Columns["ClientID"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["ClientsGroupsColumn"].DisplayIndex = DisplayIndex++;
            ClientsDataGrid.Columns["ManagerColumn"].DisplayIndex = DisplayIndex++;
        }

        private DataGridViewComboBoxColumn ManagerColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ManagerColumn",
                    HeaderText = "Менеджер",
                    DataPropertyName = "ManagerID",

                    DataSource = new DataView(ManagersDataTable),
                    ValueMember = "ManagerID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    ReadOnly = true
                };
                return Column;
            }
        }

        private DataGridViewComboBoxColumn ClientsGroupsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientsGroupsColumn",
                    HeaderText = "Группа клиентов",
                    DataPropertyName = "ClientGroupID",

                    DataSource = new DataView(ClientsGroupsDataTable),
                    ValueMember = "ClientGroupID",
                    DisplayMember = "ClientGroupName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    ReadOnly = true
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
            ShopAddressesDataGrid.Columns["Address"].HeaderText = "Адреса салонов";

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
                ConnectionStrings.ZOVReferenceConnectionString);
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
            tShopAddressesDataGrid.Columns["Address"].HeaderText = "Адреса салонов";

            tShopAddressesDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //tShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
            //tShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.Font = new System.Drawing.Font("SEGOE UI", 13.0F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
        }

        public void AddClient(string Name, int ClientGroupID, int ManagerID)
        {
            string Contacts = GetContactsXML(NewContactsDataTable);

            DataRow Row = ClientsDataTable.NewRow();
            Row["ManagerID"] = ManagerID;
            Row["ClientName"] = Name;
            Row["ClientGroupID"] = ClientGroupID;
            Row["Contacts"] = Contacts;

            ClientsDataTable.Rows.Add(Row);
            ClientsDataAdapter.Update(ClientsDataTable);
            ClientsDataTable.Clear();
            ClientsDataAdapter.Fill(ClientsDataTable);
        }

        public void EditClient(ref string ClientsName, ref int ClientGroupID, ref int ManagerID)
        {
            ClientsName = ((DataRowView)ClientsBindingSource.Current)["ClientName"].ToString();
            ClientGroupID = Convert.ToInt32(((DataRowView)ClientsBindingSource.Current)["ClientGroupID"]);
            ManagerID = Convert.ToInt32(((DataRowView)ClientsBindingSource.Current)["ManagerID"]);

            NewContactsDataTable.Clear();
            NewContactsDataTable = ContactsDataTable.Copy();
            NewContactsBindingSource.DataSource = NewContactsDataTable;

            NewShopAddressesDataTable.Clear();
            NewShopAddressesDataTable = ShopAddressesDataTable.Copy();
            NewShopAddressesBindingSource.DataSource = NewShopAddressesDataTable;
        }

        public void SaveClient(string Name, int ClientGroupID, int ManagerID, int ClientID)
        {
            string Contacts = GetContactsXML(NewContactsDataTable);

            int index = ClientsBindingSource.Position;

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Clients WHERE ClientID = " + ClientID + " ORDER BY ClientName",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0]["ClientName"] = Name;
                            DT.Rows[0]["ClientGroupID"] = ClientGroupID;

                            DT.Rows[0]["Contacts"] = Contacts;
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

        public void SaveShopAddresses()
        {
            ShopAddressesDataAdapter.Update(NewShopAddressesDataTable);
            ShopAddressesDataTable.Clear();
            ShopAddressesDataAdapter.Fill(ShopAddressesDataTable);
        }
    }
}
