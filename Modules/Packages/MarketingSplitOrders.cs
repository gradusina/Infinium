using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Infinium.Modules.Packages
{
    public class SplitOrders
    {
        public int TotalCount;
        public int EqualCount;
        public int LastCount;
        public int OrdersCount;
        public bool IsSplit;
        public bool IsEqual;

        public SplitOrders()
        {
            TotalCount = 0;
            EqualCount = 0;
            LastCount = 0;
            OrdersCount = 0;
            IsSplit = false;
            IsEqual = false;
        }
    }





    public class MarketingSplitMainOrders
    {
        private int CurrentMegaOrderID = -1;
        private int CurrentMainOrderID = -1;
        private int NewMainOrderID = -1;
        private int FactoryID = -1;

        private DataTable FrontsOrdersDT = null;
        private DataTable FrontsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;

        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public BindingSource FrontsOrdersBindingSource = null;
        public BindingSource DecorOrdersBindingSource = null;

        private PercentageDataGrid FrontsOrdersDataGrid = null;
        private PercentageDataGrid DecorOrdersDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        public MarketingSplitMainOrders(ref PercentageDataGrid tFrontsOrdersDataGrid,
            ref PercentageDataGrid tDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl)
        {
            FrontsOrdersDataGrid = tFrontsOrdersDataGrid;
            DecorOrdersDataGrid = tDecorOrdersDataGrid;
            OrdersTabControl = tOrdersTabControl;

            Initialize();
        }

        public int Factory
        {
            set { FactoryID = value; }
        }

        public int MegaOrder
        {
            set { CurrentMegaOrderID = value; }
        }

        public int MainOrder
        {
            set { CurrentMainOrderID = value; }
        }

        public int NewMainOrder
        {
            get { return NewMainOrderID; }
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            FrontsOrdersGridSetting();
            DecorOrdersGridSettings();
        }

        private void Create()
        {
            FrontsOrdersDT = new DataTable();
            FrontsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsOrdersBindingSource = new BindingSource();
            DecorOrdersBindingSource = new BindingSource();
        }

        private void GetColorsDT()
        {
            FrameColorsDataTable = new DataTable();
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            FrameColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = FrameColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        FrameColorsDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDataTable);
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDataTable.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDataTable.Rows.Add(NewRow);
                }

            }

        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechnoProfilesDataTable = new DataTable();
                DA.Fill(TechnoProfilesDataTable);

                DataRow NewRow = TechnoProfilesDataTable.NewRow();
                NewRow["TechnoProfileID"] = -1;
                NewRow["TechnoProfileName"] = "-";
                TechnoProfilesDataTable.Rows.InsertAt(NewRow, 0);
            }

            GetColorsDT();
            GetInsetColorsDT();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }

            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }
            FrontsOrdersDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }
            DecorOrdersDataTable.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
        }

        private void Binding()
        {
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            DecorOrdersBindingSource.DataSource = DecorOrdersDataTable;

            FrontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
            DecorOrdersDataGrid.DataSource = DecorOrdersBindingSource;
        }

        #region Создание столбцов DataGridViewComboBoxColumn

        public DataGridViewComboBoxColumn FrontsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "FrontsColumn",
                    HeaderText = "Фасад",
                    DataPropertyName = "FrontID",
                    DataSource = new DataView(FrontsDataTable),
                    ValueMember = "FrontID",
                    DisplayMember = "FrontName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn FrameColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "FrameColorsColumn",
                    HeaderText = "Цвет\r\nпрофиля",
                    DataPropertyName = "ColorID",
                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PatinaColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn",
                    HeaderText = "Патина",
                    DataPropertyName = "PatinaID",
                    DataSource = new DataView(PatinaDataTable),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn InsetTypesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "InsetTypesColumn",
                    HeaderText = "Тип\r\nнаполнителя",
                    DataPropertyName = "InsetTypeID",
                    DataSource = new DataView(InsetTypesDataTable),
                    ValueMember = "InsetTypeID",
                    DisplayMember = "InsetTypeName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn InsetColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "InsetColorsColumn",
                    HeaderText = "Цвет\r\nнаполнителя",
                    DataPropertyName = "InsetColorID",
                    DataSource = new DataView(InsetColorsDataTable),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoProfilesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoProfilesColumn",
                    HeaderText = "Тип\r\nпрофиля-2",
                    DataPropertyName = "TechnoProfileID",
                    DataSource = new DataView(TechnoProfilesDataTable),
                    ValueMember = "TechnoProfileID",
                    DisplayMember = "TechnoProfileName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }
        public DataGridViewComboBoxColumn TechnoFrameColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoFrameColorsColumn",
                    HeaderText = "Цвет профиля-2",
                    DataPropertyName = "TechnoColorID",
                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoInsetTypesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoInsetTypesColumn",
                    HeaderText = "Тип наполнителя-2",
                    DataPropertyName = "TechnoInsetTypeID",
                    DataSource = new DataView(TechnoInsetTypesDataTable),
                    ValueMember = "InsetTypeID",
                    DisplayMember = "InsetTypeName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnoInsetColorsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnoInsetColorsColumn",
                    HeaderText = "Цвет наполнителя-2",
                    DataPropertyName = "TechnoInsetColorID",
                    DataSource = new DataView(TechnoInsetColorsDataTable),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        private DataGridViewComboBoxColumn CreateProductColumn
        {
            get
            {
                DataGridViewComboBoxColumn ProductColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ProductColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "ProductID",

                    DataSource = ProductsDataTable,
                    ValueMember = "ProductID",
                    DisplayMember = "ProductName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ProductColumn;
            }
        }

        private DataGridViewComboBoxColumn CreateColorColumn
        {
            get
            {
                DataGridViewComboBoxColumn ColorsColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ColorsColumn",
                    HeaderText = "Цвет",
                    DataPropertyName = "ColorID",

                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ColorsColumn;
            }
        }

        private DataGridViewComboBoxColumn CreateItemColumn
        {
            get
            {
                DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ItemColumn",
                    HeaderText = "Название",
                    DataPropertyName = "DecorID",

                    DataSource = new DataView(DecorDataTable),
                    ValueMember = "DecorID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ItemColumn;
            }
        }

        #endregion

        #region Настройка гридов

        private void FrontsOrdersGridSetting()
        {
            //добавление столбцов
            FrontsOrdersDataGrid.Columns.Add(FrontsColumn);
            FrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetColorsColumn);


            if (FrontsOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                FrontsOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                FrontsOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                FrontsOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                FrontsOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (FrontsOrdersDataGrid.Columns.Contains("CreateUserID"))
                FrontsOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                FrontsOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("PriceWithTransport"))
                FrontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CostWithTransport"))
                FrontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("OriginalPrice"))
                FrontsOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            FrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            FrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;
            FrontsOrdersDataGrid.Columns["Square"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Visible = false;

            FrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            foreach (DataGridViewColumn Column in FrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";

            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Height"].Width = 85;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Width"].Width = 85;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Count"].Width = 85;
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Square"].Width = 100;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsSample"].Width = 85;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 75;
            FrontsOrdersDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Check"].Width = 100;

            FrontsOrdersDataGrid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            FrontsOrdersDataGrid.Columns["Check"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            FrontsOrdersDataGrid.Columns["Check"].ReadOnly = false;
        }

        private void DecorOrdersGridSettings()
        {
            DecorOrdersDataGrid.Columns.Add(this.CreateProductColumn);
            DecorOrdersDataGrid.Columns.Add(this.CreateColorColumn);
            DecorOrdersDataGrid.Columns["ColorID"].Visible = false;

            //ItemColumn
            DecorOrdersDataGrid.Columns.Add(CreateItemColumn);

            //убирание лишних столбцов
            if (DecorOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                DecorOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                DecorOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                DecorOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                DecorOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (DecorOrdersDataGrid.Columns.Contains("CreateUserID"))
                DecorOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (DecorOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                DecorOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            DecorOrdersDataGrid.Columns["DecorOrderID"].Visible = false;
            DecorOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            DecorOrdersDataGrid.Columns["ProductID"].Visible = false;
            DecorOrdersDataGrid.Columns["DecorID"].Visible = false;
            DecorOrdersDataGrid.Columns["Price"].Visible = false;
            DecorOrdersDataGrid.Columns["DecorConfigID"].Visible = false;
            DecorOrdersDataGrid.Columns["Cost"].Visible = false;
            DecorOrdersDataGrid.Columns["FactoryID"].Visible = false;
            DecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            DecorOrdersDataGrid.Columns["Weight"].Visible = false;

            DecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            DecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

            DecorOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorOrdersDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            DecorOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            DecorOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";

            DecorOrdersDataGrid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorOrdersDataGrid.Columns["ProductColumn"].MinimumWidth = 120;
            DecorOrdersDataGrid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorOrdersDataGrid.Columns["ItemColumn"].MinimumWidth = 120;
            DecorOrdersDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DecorOrdersDataGrid.Columns["ColorsColumn"].MinimumWidth = 145;
            DecorOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorOrdersDataGrid.Columns["Height"].Width = 85;
            DecorOrdersDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorOrdersDataGrid.Columns["Length"].Width = 85;
            DecorOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorOrdersDataGrid.Columns["Width"].Width = 85;
            DecorOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorOrdersDataGrid.Columns["Count"].Width = 85;
            DecorOrdersDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorOrdersDataGrid.Columns["Check"].Width = 100;
            DecorOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorOrdersDataGrid.Columns["IsSample"].Width = 85;

            foreach (DataGridViewColumn Column in DecorOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorOrdersDataGrid.AutoGenerateColumns = false;

            DecorOrdersDataGrid.Columns["Check"].DisplayIndex = 0;
            DecorOrdersDataGrid.Columns["ProductColumn"].DisplayIndex = 1;
            DecorOrdersDataGrid.Columns["ItemColumn"].DisplayIndex = 2;
            DecorOrdersDataGrid.Columns["ColorsColumn"].DisplayIndex = 3;
            DecorOrdersDataGrid.Columns["Height"].DisplayIndex = 4;
            DecorOrdersDataGrid.Columns["Length"].DisplayIndex = 5;
            DecorOrdersDataGrid.Columns["Width"].DisplayIndex = 6;
            DecorOrdersDataGrid.Columns["Count"].DisplayIndex = 7;
            DecorOrdersDataGrid.Columns["Notes"].DisplayIndex = 8;

            DecorOrdersDataGrid.Columns["Check"].ReadOnly = false;
        }

        #endregion

        #region Замена MainOrderID в заказах

        public void ChangeFrontsMainOrder()
        {
            int[] FrontsOrdersIDs = GetCheckedFrontsOrders();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrdersID, MainOrderID FROM FrontsOrders WHERE FrontsOrdersID IN (" +
                string.Join(",", FrontsOrdersIDs) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["MainOrderID"] = NewMainOrderID;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ChangeDecorMainOrder()
        {
            int[] DecorOrdersIDs = GetCheckedDecorOrders();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrderID, MainOrderID FROM DecorOrders WHERE DecorOrderID IN (" +
                string.Join(",", DecorOrdersIDs) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["MainOrderID"] = NewMainOrderID;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Создаёт новый подзаказ и копирует в него все значения из исходного подзаказа
        /// </summary>
        /// <param name="MegaOrderID"></param>
        public void CreateNewMainOrder()
        {
            DataTable TempDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID = " + CurrentMainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(TempDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow Row = DT.NewRow();

                        Row.ItemArray = TempDataTable.Rows[0].ItemArray;

                        DT.Rows.Add(Row);

                        DA.Update(DT);
                    }
                }
            }

            NewMainOrderID = GetNewMainOrderID();

            TempDataTable.Dispose();
        }

        public void CreateNewFrontsOrder(Infinium.Modules.Packages.SplitOrders SS, int FrontsOrdersID)
        {
            DataTable TempDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE FrontsOrdersID = " + FrontsOrdersID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(TempDataTable);

                    if (TempDataTable.Rows.Count > 0)
                    {
                        TempDataTable.Rows[0]["Count"] = SS.EqualCount;

                        DA.Update(TempDataTable);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (SS.IsEqual)
                        {
                            for (int i = 0; i < SS.OrdersCount - 1; i++)
                            {
                                DataRow Row = DT.NewRow();

                                Row.ItemArray = TempDataTable.Rows[0].ItemArray;
                                Row["Count"] = SS.EqualCount;

                                DT.Rows.Add(Row);
                            }

                            if (SS.LastCount != 0)
                            {
                                DataRow TRow = DT.NewRow();

                                TRow.ItemArray = TempDataTable.Rows[0].ItemArray;
                                TRow["Count"] = SS.LastCount;

                                DT.Rows.Add(TRow);
                            }
                        }
                        else
                        {
                            DataRow Row = DT.NewRow();

                            Row.ItemArray = TempDataTable.Rows[0].ItemArray;
                            Row["Count"] = SS.LastCount;

                            DT.Rows.Add(Row);
                        }

                        DA.Update(DT);
                    }
                }
            }
            TempDataTable.Dispose();
        }

        public void CreateNewDecorOrder(Infinium.Modules.Packages.SplitOrders SS, int DecorOrderID)
        {
            DataTable TempDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE DecorOrderID = " + DecorOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(TempDataTable);

                    if (TempDataTable.Rows.Count > 0)
                    {
                        TempDataTable.Rows[0]["Count"] = SS.EqualCount;

                        DA.Update(TempDataTable);
                    }
                }
            }


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (SS.IsEqual)
                        {
                            for (int i = 0; i < SS.OrdersCount - 1; i++)
                            {
                                DataRow Row = DT.NewRow();

                                Row.ItemArray = TempDataTable.Rows[0].ItemArray;
                                Row["Count"] = SS.EqualCount;

                                DT.Rows.Add(Row);
                            }

                            if (SS.LastCount != 0)
                            {
                                DataRow TRow = DT.NewRow();

                                TRow.ItemArray = TempDataTable.Rows[0].ItemArray;
                                TRow["Count"] = SS.LastCount;

                                DT.Rows.Add(TRow);
                            }
                        }
                        else
                        {
                            DataRow Row = DT.NewRow();

                            Row.ItemArray = TempDataTable.Rows[0].ItemArray;
                            Row["Count"] = SS.LastCount;

                            DT.Rows.Add(Row);
                        }

                        DA.Update(DT);
                    }
                }
            }
            TempDataTable.Dispose();
        }


        #region Фильтры

        public void Filter()
        {
            OrdersTabControl.TabPages[0].PageVisible = FilterFrontsOrders();
            OrdersTabControl.TabPages[1].PageVisible = FilterDecorOrders();

            //if (OrdersTabControl.TabPages[0].PageVisible == false && OrdersTabControl.TabPages[1].PageVisible == false)
            //    OrdersTabControl.Visible = false;
            //else
            //    OrdersTabControl.Visible = true;
        }

        private bool FilterFrontsOrders()
        {
            string SelectionCommand = "SELECT * FROM FrontsOrders WHERE MainOrderID = " + CurrentMainOrderID + " AND FactoryID=" + FactoryID;

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            for (int i = 0; i < FrontsOrdersDataTable.Rows.Count; i++)
            {
                FrontsOrdersDataTable.Rows[i]["Check"] = false;
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders()
        {
            string SelectionCommand = "SELECT * FROM DecorOrders WHERE MainOrderID = " + CurrentMainOrderID + " AND FactoryID=" + FactoryID;

            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in DecorOrdersDataTable.Rows)
                {
                    Row["Check"] = false;

                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        #endregion

        /// <summary>
        /// возвращает массив значений FrontsOrdersID, отмеченных для перемещения в новый подзаказ
        /// </summary>
        /// <returns></returns>
        private int[] GetCheckedFrontsOrders()
        {
            int[] rows = new int[FrontsOrdersDataGrid.Rows.Count];

            for (int i = 0; i < FrontsOrdersDataGrid.Rows.Count; i++)
            {
                if (FrontsOrdersDataGrid.Rows[i].Cells["Check"].Value != DBNull.Value && Convert.ToBoolean(FrontsOrdersDataGrid.Rows[i].Cells["Check"].Value))
                    rows[i] = Convert.ToInt32(FrontsOrdersDataGrid.Rows[i].Cells["FrontsOrdersID"].Value);
            }
            return rows;
        }

        /// <summary>
        /// возвращает массив значений DecorOrderID, отмеченных для перемещения в новый подзаказ
        /// </summary>
        /// <returns></returns>
        private int[] GetCheckedDecorOrders()
        {
            int[] rows = new int[DecorOrdersDataGrid.Rows.Count];

            for (int i = 0; i < DecorOrdersDataGrid.Rows.Count; i++)
            {
                if (DecorOrdersDataGrid.Rows[i].Cells["Check"].Value != DBNull.Value && Convert.ToBoolean(DecorOrdersDataGrid.Rows[i].Cells["Check"].Value))
                    rows[i] = Convert.ToInt32(DecorOrdersDataGrid.Rows[i].Cells["DecorOrderID"].Value);
            }
            return rows;
        }

        /// <summary>
        /// Возвращает количество фасадов в отмеченных позициях
        /// </summary>
        public int GetCheckedFrontsCount
        {
            get
            {
                int Count = 0;

                for (int i = 0; i < FrontsOrdersDataGrid.Rows.Count; i++)
                {
                    if (FrontsOrdersDataGrid.Rows[i].Cells["Check"].Value != DBNull.Value && Convert.ToBoolean(FrontsOrdersDataGrid.Rows[i].Cells["Check"].Value))
                        Count += Convert.ToInt32(FrontsOrdersDataGrid.Rows[i].Cells["Count"].Value);
                }
                return Count;
            }
        }

        /// <summary>
        /// Возвращает количество декора в отмеченных позициях
        /// </summary>
        public int GetCheckedDecorCount
        {
            get
            {
                int Count = 0;

                for (int i = 0; i < DecorOrdersDataGrid.Rows.Count; i++)
                {
                    if (DecorOrdersDataGrid.Rows[i].Cells["Check"].Value != DBNull.Value && Convert.ToBoolean(DecorOrdersDataGrid.Rows[i].Cells["Check"].Value))
                        Count += Convert.ToInt32(DecorOrdersDataGrid.Rows[i].Cells["Count"].Value);
                }
                return Count;
            }
        }

        /// <summary>
        /// возвращает MainOrderID нового созданного подзаказа
        /// </summary>
        private int GetNewMainOrderID()
        {
            int NewID = -1;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MAX(MainOrderID) AS MainOrderID FROM MainOrders WHERE MegaOrderID = " + CurrentMegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["MainOrderID"] == DBNull.Value)
                        return 1;

                    NewID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                }
            }

            return NewID;
        }

        //true, если хотя бы одна упаковка отмечена
        public bool AreFrontsChecked
        {
            get
            {
                foreach (DataGridViewRow row in FrontsOrdersDataGrid.Rows)
                {
                    if (row.Cells["Check"].Value != DBNull.Value && Convert.ToBoolean(row.Cells["Check"].Value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool AreDecorChecked
        {
            get
            {
                foreach (DataGridViewRow row in DecorOrdersDataGrid.Rows)
                {
                    if (row.Cells["Check"].Value != DBNull.Value && Convert.ToBoolean(row.Cells["Check"].Value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void SetMainOrderFactory(int MainOrderID)
        {
            int FactoryID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID FROM FrontsOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        FactoryID = Convert.ToInt32(DT.Rows[0]["FactoryID"]);

                        foreach (DataRow row in DT.Rows)
                        {
                            if (Convert.ToInt32(row["FactoryID"]) != FactoryID)
                            {
                                FactoryID = 0;
                                break;
                            }
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID FROM DecorOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        FactoryID = Convert.ToInt32(DT.Rows[0]["FactoryID"]);

                        foreach (DataRow row in DT.Rows)
                        {
                            if (Convert.ToInt32(row["FactoryID"]) != FactoryID)
                            {
                                FactoryID = 0;
                                break;
                            }
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID FROM MainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["FactoryID"] = FactoryID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }
    }
}
