using ComponentFactory.Krypton.Toolkit;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Infinium
{
    public class CheckLabel
    {
        int iUserID = 0;

        PercentageDataGrid FrontsPackContentDataGrid = null;
        PercentageDataGrid DecorPackContentDataGrid = null;

        DataTable FrontsPackContentDataTable = null;
        DataTable DecorPackContentDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        DataTable ZOVClientsDataTable;
        DataTable MarktClientsDataTable;

        DataTable DecorDataTable;
        DataTable DecorProductsDataTable;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public BindingSource FrontsPackContentBindingSource = null;
        public BindingSource DecorPackContentBindingSource = null;

        public struct Labelinfo
        {
            public string CurrentPackNumber;
            public string PackedToTotal;
            public string ClientName;
            public string DispatchDate;
            public string OrderDate;
            public string Factory;
            public string MegaOrderNumber;
            public string MainOrderNumber;
            public int MainOrderID;
            public int MegaOrderID;
            public string ProductType;
            public string Group;
            public Color DispatchDateColor;
            public Color TotalLabelColor;
        }

        public int UserID
        {
            get { return iUserID; }
            set { iUserID = value; }
        }

        public Labelinfo LabelInfo;

        public CheckLabel(ref PercentageDataGrid tFrontsPackContentDataGrid, ref PercentageDataGrid tDecorPackContentDataGrid)
        {
            FrontsPackContentDataGrid = tFrontsPackContentDataGrid;
            DecorPackContentDataGrid = tDecorPackContentDataGrid;

            Initialize();
        }

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();
            FrontsPackContentDataTable = new DataTable();
            DecorPackContentDataTable = new DataTable();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow[] rows = FrameColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = FrameColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            FrameColorsDataTable.Rows.Add(NewRow);
                        }
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }

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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            MarktClientsDataTable = new DataTable();
            ZOVClientsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MarktClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZOVClientsDataTable);
            }
        }

        private void Binding()
        {
            FrontsPackContentBindingSource = new BindingSource()
            {
                DataSource = FrontsPackContentDataTable
            };
            FrontsPackContentDataGrid.DataSource = FrontsPackContentBindingSource;

            DecorPackContentBindingSource = new BindingSource()
            {
                DataSource = DecorPackContentDataTable
            };
            DecorPackContentDataGrid.DataSource = DecorPackContentBindingSource;
        }

        private void CreateColumns()
        {
            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = new DataView(FrontsDataTable),
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = new DataView(PatinaDataTable),
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = new DataView(InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = new DataView(InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoProfilesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(TechnoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoFrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(TechnoInsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(TechnoInsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrontsPackContentDataGrid.Columns.Add(FrontsColumn);
            FrontsPackContentDataGrid.Columns.Add(FrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(PatinaColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetColorsColumn);

            DecorPackContentDataGrid.Columns.Add(ProductColumn);
            DecorPackContentDataGrid.Columns.Add(ItemColumn);
            DecorPackContentDataGrid.Columns.Add(DecorPatinaColumn);
            DecorPackContentDataGrid.Columns.Add(ColorColumn);
        }

        public DataGridViewComboBoxColumn ProductColumn
        {
            get
            {
                DataGridViewComboBoxColumn ProductColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ProductColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "ProductID",

                    DataSource = new DataView(DecorProductsDataTable),
                    ValueMember = "ProductID",
                    DisplayMember = "ProductName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ProductColumn;
            }
        }

        public DataGridViewComboBoxColumn ItemColumn
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

        public DataGridViewComboBoxColumn ColorColumn
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

        public DataGridViewComboBoxColumn DecorPatinaColumn
        {
            get
            {
                DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn",
                    HeaderText = "Патина",
                    DataPropertyName = "PatinaID",

                    DataSource = new DataView(PatinaDataTable),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return PatinaColumn;
            }
        }

        private void FrontsGridSettings()
        {
            foreach (DataGridViewColumn Column in FrontsPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsPackContentDataGrid.Columns["FrontID"].Visible = false;
            FrontsPackContentDataGrid.Columns["ColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            int DisplayIndex = 0;
            FrontsPackContentDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;

            FrontsPackContentDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Height"].Width = 90;
            FrontsPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Width"].Width = 90;
            FrontsPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void DecorGridSettings()
        {
            foreach (DataGridViewColumn Column in DecorPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorPackContentDataGrid.Columns["ProductID"].Visible = false;
            DecorPackContentDataGrid.Columns["DecorID"].Visible = false;
            DecorPackContentDataGrid.Columns["ColorID"].Visible = false;
            DecorPackContentDataGrid.Columns["PatinaID"].Visible = false;


            DecorPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorPackContentDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            DecorPackContentDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            DecorPackContentDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;

            DecorPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Height"].Width = 90;
            DecorPackContentDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Length"].Width = 90;
            DecorPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Width"].Width = 90;
            DecorPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            FrontsGridSettings();
            DecorGridSettings();
        }

        private void FillFrontsPackContent(int PackageID, int Group)
        {
            FrontsPackContentDataTable.Clear();


            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }

            if (Group == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID,  FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }
        }

        private void FillDecorPackContent(int PackageID, int Group)
        {
            DecorPackContentDataTable.Clear();


            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (Group == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (DecorPackContentDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in DecorPackContentDataTable.Rows)
                {
                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }
        }

        public void GetLabelInfo(string Barcode)
        {
            string BarType = Barcode.Substring(0, 3);

            if (BarType == "001" || BarType == "002")//ЗОВ
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, DocNumber, ClientID, ProfilPackCount, TPSPackCount, DocDateTime FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)) + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {

                        DA.Fill(DT);

                        using (SqlDataAdapter MegaDA = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate FROM MegaOrders WHERE MegaOrderID = " + DT.Rows[0]["MegaOrderID"], ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            using (DataTable MegaDT = new DataTable())
                            {
                                MegaDA.Fill(MegaDT);


                                LabelInfo.ClientName = ZOVClientsDataTable.Select("ClientID = " + DT.Rows[0]["ClientID"])[0]["ClientName"].ToString();

                                if (BarType == "001")//profil markt
                                    LabelInfo.Factory = "Профиль";
                                else
                                    LabelInfo.Factory = "ТПС";

                                int TotalPackCount = 0;

                                SetPacked(Barcode, "ЗОВ");

                                if (MegaDT.Rows[0]["DispatchDate"] != DBNull.Value)
                                    LabelInfo.DispatchDate = Convert.ToDateTime(MegaDT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");

                                if (LabelInfo.Factory == "Профиль")
                                {
                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]);

                                    int PackedCount = GetPackedCount(1, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "ЗОВ");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }
                                else
                                {
                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);

                                    int PackedCount = GetPackedCount(2, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "ЗОВ");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }


                                if (LabelInfo.DispatchDate.Length > 0)
                                    if (Convert.ToDateTime(LabelInfo.DispatchDate) < Convert.ToDateTime(GetCurrentDate.ToString("dd.MM.yyyy")))
                                        LabelInfo.DispatchDateColor = Color.Red;
                                    else
                                        LabelInfo.DispatchDateColor = Color.FromArgb(82, 169, 24);

                                LabelInfo.OrderDate = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]).ToString("dd.MM.yyyy");

                                LabelInfo.MegaOrderNumber = MegaDT.Rows[0]["MegaOrderID"].ToString();

                                LabelInfo.MainOrderNumber = DT.Rows[0]["DocNumber"].ToString();

                                LabelInfo.MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                                LabelInfo.MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                                LabelInfo.Group = "ЗОВ";



                                using (SqlDataAdapter pDA = new SqlDataAdapter("SELECT PackNumber FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                                {
                                    using (DataTable pDT = new DataTable())
                                    {
                                        pDA.Fill(pDT);
                                        LabelInfo.CurrentPackNumber = pDT.Rows[0]["PackNumber"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }


                if (CheckProductType(Barcode, LabelInfo.Group) == 0)
                {
                    LabelInfo.ProductType = "Фасады";
                    FillFrontsPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 1);
                    FrontsPackContentDataGrid.BringToFront();
                }
                else
                {
                    LabelInfo.ProductType = "Погонаж и декор";
                    FillDecorPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 1);
                    DecorPackContentDataGrid.BringToFront();
                }

                return;
            }


            if (BarType == "003" || BarType == "004")//Маркетинг
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, ProfilPackCount, TPSPackCount FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)) + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {

                        DA.Fill(DT);

                        using (SqlDataAdapter MegaDA = new SqlDataAdapter("SELECT MegaOrderID, ClientID, ProfilDispatchDate, TPSDispatchDate, OrderDate, OrderNumber FROM MegaOrders WHERE MegaOrderID = " + DT.Rows[0]["MegaOrderID"], ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            using (DataTable MegaDT = new DataTable())
                            {
                                MegaDA.Fill(MegaDT);


                                LabelInfo.ClientName = MarktClientsDataTable.Select("ClientID = " + MegaDT.Rows[0]["ClientID"])[0]["ClientName"].ToString();

                                if (BarType == "003")//profil markt
                                    LabelInfo.Factory = "Профиль";
                                else
                                    LabelInfo.Factory = "ТПС";

                                int TotalPackCount = 0;

                                SetPacked(Barcode, "Маркетинг");

                                if (LabelInfo.Factory == "Профиль")
                                {
                                    if (MegaDT.Rows[0]["ProfilDispatchDate"] != DBNull.Value)
                                        LabelInfo.DispatchDate = Convert.ToDateTime(MegaDT.Rows[0]["ProfilDispatchDate"]).ToString("dd.MM.yyyy");

                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]);

                                    int PackedCount = GetPackedCount(1, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "Маркетинг");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }
                                else
                                {
                                    if (MegaDT.Rows[0]["TPSDispatchDate"] != DBNull.Value)
                                        LabelInfo.DispatchDate = Convert.ToDateTime(MegaDT.Rows[0]["TPSDispatchDate"]).ToString("dd.MM.yyyy");

                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);

                                    int PackedCount = GetPackedCount(2, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "Маркетинг");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }

                                if (!string.IsNullOrEmpty(LabelInfo.DispatchDate) && Convert.ToDateTime(LabelInfo.DispatchDate) < Convert.ToDateTime(GetCurrentDate.ToString("dd.MM.yyyy")))
                                    LabelInfo.DispatchDateColor = Color.Red;
                                else
                                    LabelInfo.DispatchDateColor = Color.FromArgb(82, 169, 24);

                                LabelInfo.OrderDate = Convert.ToDateTime(MegaDT.Rows[0]["OrderDate"]).ToString("dd.MM.yyyy");

                                LabelInfo.MegaOrderNumber = MegaDT.Rows[0]["OrderNumber"].ToString();

                                LabelInfo.MainOrderNumber = DT.Rows[0]["MainOrderID"].ToString();
                                LabelInfo.MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                                LabelInfo.MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                                LabelInfo.Group = "Маркетинг";



                                using (SqlDataAdapter pDA = new SqlDataAdapter("SELECT PackNumber FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                                {
                                    using (DataTable pDT = new DataTable())
                                    {
                                        pDA.Fill(pDT);
                                        LabelInfo.CurrentPackNumber = pDT.Rows[0]["PackNumber"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }


                if (CheckProductType(Barcode, LabelInfo.Group) == 0)
                {
                    LabelInfo.ProductType = "Фасады";
                    FillFrontsPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 2);
                    FrontsPackContentDataGrid.BringToFront();
                }
                else
                {
                    LabelInfo.ProductType = "Погонаж и декор";
                    FillDecorPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 2);
                    DecorPackContentDataGrid.BringToFront();
                }

                return;
            }
        }

        public bool CheckBarcode(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        private void SetPacked(string Barcode, string Group)
        {
            string ConnectionString;

            if (Group == "Маркетинг")
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, PackingDateTime, PackUserID FROM Packages WHERE PackageID = " +
                Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            if (Group == "Маркетинг")
                            {
                                //нельзя принять на упаковку, если уже огружено
                                if (DT.Rows[0]["PackageStatusID"] != DBNull.Value
                                    && Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) == 3)
                                    return;
                                DT.Rows[0]["PackageStatusID"] = 1;
                                if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                    DT.Rows[0]["PackingDateTime"] = GetCurrentDate;

                                if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                                    DT.Rows[0]["PackUserID"] = iUserID;
                            }
                            else
                            {
                                using (SqlDataAdapter DA1 = new SqlDataAdapter("SELECT TOP 1 * FROM PackageDetails WHERE PackageID = " +
                                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionString))
                                {
                                    using (DataTable DT1 = new DataTable())
                                    {
                                        DA1.Fill(DT1);
                                        if (DT1.Rows.Count > 0)
                                        {
                                            int OrderID = Convert.ToInt32(DT1.Rows[0]["OrderID"]);
                                            using (SqlDataAdapter DA2 = new SqlDataAdapter("SELECT * FROM PackagesEvents WHERE OrderID = " + OrderID, ConnectionString))
                                            {
                                                using (DataTable DT2 = new DataTable())
                                                {
                                                    DA2.Fill(DT2);
                                                    if (DT2.Rows.Count > 0)
                                                    {
                                                        //нельзя принять на упаковку, если уже огружено
                                                        if (DT.Rows[0]["PackageStatusID"] != DBNull.Value && Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) == 3)
                                                            return;
                                                        DT.Rows[0]["PackageStatusID"] = 1;
                                                        if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                                            DT.Rows[0]["PackingDateTime"] = DT2.Rows[0]["PackingDateTime"];

                                                        if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                                                            DT.Rows[0]["PackUserID"] = DT2.Rows[0]["PackUserID"];
                                                    }
                                                    else
                                                    {
                                                        if (DT.Rows[0]["PackageStatusID"] != DBNull.Value
                                                            && Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) == 3)
                                                            return;
                                                        DT.Rows[0]["PackageStatusID"] = 1;
                                                        if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                                            DT.Rows[0]["PackingDateTime"] = GetCurrentDate;

                                                        if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                                                            DT.Rows[0]["PackUserID"] = iUserID;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public int GetPackedCount(int FactoryID, int MainOrderID, string Group)
        {
            string ConnectionString;

            if (Group == "Маркетинг")
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT PackageID FROM Packages WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID + " AND (PackageStatusID = 1 OR PackageStatusID = 2)",
                ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    return DA.Fill(DT);
                }
            }
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        public int CheckProductType(string Barcode, string Group)
        {
            string ConnectionString;

            if (Group == "Маркетинг")
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProductType FROM Packages WHERE PackageID = " + Barcode.Substring(3, 9), ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToInt32(DT.Rows[0]["ProductType"]);
                }
            }
        }

        public void Clear()
        {
            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            LabelInfo.ClientName = "";
            LabelInfo.CurrentPackNumber = "";
            LabelInfo.DispatchDate = "";
            LabelInfo.DispatchDateColor = Color.White;
            LabelInfo.Factory = "";
            LabelInfo.Group = "";
            LabelInfo.MainOrderNumber = "";
            LabelInfo.MainOrderID = 0;
            LabelInfo.MegaOrderNumber = "";
            LabelInfo.OrderDate = "";
            LabelInfo.PackedToTotal = "";
            LabelInfo.ProductType = "";
            LabelInfo.TotalLabelColor = Color.White;
        }

        private int GetMegaOrderID(int MainOrderID)
        {
            int MegaOrderID = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["MegaOrderID"] != DBNull.Value)
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                    return MegaOrderID;
                }
            }
        }

        public void SetGridColor(string ProductType, bool IsAccept)
        {
            if (IsAccept)
            {
                if (ProductType == "Фасады")
                {
                    FrontsPackContentDataGrid.StateCommon.Background.Color1 = Color.FromArgb(82, 169, 24);
                    FrontsPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                    FrontsPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                    FrontsPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.FromArgb(82, 169, 24);
                    FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;
                }

                if (ProductType == "Погонаж и декор")
                {
                    DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.FromArgb(82, 169, 24);
                    DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                    DecorPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                    DecorPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.FromArgb(82, 169, 24);
                    DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;
                }
            }
            else
            {
                FrontsPackContentDataGrid.StateCommon.Background.Color1 = Color.Red;
                FrontsPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                FrontsPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = Color.FromArgb(240, 0, 0);

                DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.Red;
                DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                DecorPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = Color.FromArgb(240, 0, 0);
            }
        }
    }






    public class StorageCheckLabel
    {
        int iUserID = 0;

        PercentageDataGrid FrontsPackContentDataGrid = null;
        PercentageDataGrid DecorPackContentDataGrid = null;

        DataTable FrontsPackContentDataTable = null;
        DataTable DecorPackContentDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        DataTable ZOVClientsDataTable;
        DataTable MarktClientsDataTable;

        DataTable DecorDataTable;
        DataTable DecorProductsDataTable;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public BindingSource FrontsPackContentBindingSource = null;
        public BindingSource DecorPackContentBindingSource = null;

        public struct Labelinfo
        {
            public string CurrentPackNumber;
            public string PackedToTotal;
            public string ClientName;
            public string DispatchDate;
            public string OrderDate;
            public string Factory;
            public string MegaOrderNumber;
            public string MainOrderNumber;
            public int MainOrderID;
            public int MegaOrderID;
            public string ProductType;
            public string Group;
            public Color DispatchDateColor;
            public Color TotalLabelColor;
        }

        public Labelinfo LabelInfo;

        public int UserID
        {
            get { return iUserID; }
            set { iUserID = value; }
        }

        public StorageCheckLabel(ref PercentageDataGrid tFrontsPackContentDataGrid, ref PercentageDataGrid tDecorPackContentDataGrid)
        {
            FrontsPackContentDataGrid = tFrontsPackContentDataGrid;
            DecorPackContentDataGrid = tDecorPackContentDataGrid;

            Initialize();
        }

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            FrontsPackContentDataTable = new DataTable();
            DecorPackContentDataTable = new DataTable();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow[] rows = FrameColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = FrameColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            FrameColorsDataTable.Rows.Add(NewRow);
                        }
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID,  FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            MarktClientsDataTable = new DataTable();
            ZOVClientsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MarktClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZOVClientsDataTable);
            }
        }

        private void Binding()
        {
            FrontsPackContentBindingSource = new BindingSource()
            {
                DataSource = FrontsPackContentDataTable
            };
            FrontsPackContentDataGrid.DataSource = FrontsPackContentBindingSource;

            DecorPackContentBindingSource = new BindingSource()
            {
                DataSource = DecorPackContentDataTable
            };
            DecorPackContentDataGrid.DataSource = DecorPackContentBindingSource;
        }

        private void CreateColumns()
        {
            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = new DataView(FrontsDataTable),
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = new DataView(PatinaDataTable),
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = new DataView(InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = new DataView(InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoProfilesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(TechnoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoFrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(TechnoInsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(TechnoInsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrontsPackContentDataGrid.Columns.Add(FrontsColumn);
            FrontsPackContentDataGrid.Columns.Add(FrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(PatinaColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetColorsColumn);

            DecorPackContentDataGrid.Columns.Add(ProductColumn);
            DecorPackContentDataGrid.Columns.Add(ItemColumn);
            DecorPackContentDataGrid.Columns.Add(ColorColumn);
            DecorPackContentDataGrid.Columns.Add(DecorPatinaColumn);
        }

        private DataGridViewComboBoxColumn ProductColumn
        {
            get
            {
                DataGridViewComboBoxColumn ProductColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ProductColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "ProductID",

                    DataSource = new DataView(DecorProductsDataTable),
                    ValueMember = "ProductID",
                    DisplayMember = "ProductName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ProductColumn;
            }
        }

        private DataGridViewComboBoxColumn ItemColumn
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

        private DataGridViewComboBoxColumn ColorColumn
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

        private DataGridViewComboBoxColumn DecorPatinaColumn
        {
            get
            {
                DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn",
                    HeaderText = "Патина",
                    DataPropertyName = "PatinaID",

                    DataSource = new DataView(PatinaDataTable),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return PatinaColumn;
            }
        }

        private void FrontsGridSettings()
        {
            foreach (DataGridViewColumn Column in FrontsPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsPackContentDataGrid.Columns["FrontID"].Visible = false;
            FrontsPackContentDataGrid.Columns["ColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            int DisplayIndex = 0;
            FrontsPackContentDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;

            FrontsPackContentDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Height"].Width = 90;
            FrontsPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Width"].Width = 90;
            FrontsPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void DecorGridSettings()
        {
            foreach (DataGridViewColumn Column in DecorPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorPackContentDataGrid.Columns["ProductID"].Visible = false;
            DecorPackContentDataGrid.Columns["DecorID"].Visible = false;
            DecorPackContentDataGrid.Columns["ColorID"].Visible = false;
            DecorPackContentDataGrid.Columns["PatinaID"].Visible = false;


            DecorPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorPackContentDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            DecorPackContentDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            DecorPackContentDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;

            DecorPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Height"].Width = 90;
            DecorPackContentDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Length"].Width = 90;
            DecorPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Width"].Width = 90;
            DecorPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            FrontsGridSettings();
            DecorGridSettings();
        }

        private void FillFrontsPackContent(int PackageID, int Group)
        {
            FrontsPackContentDataTable.Clear();


            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders WHERE " +
                                                              " FrontsOrdersID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }

            if (Group == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders WHERE " +
                                                              " FrontsOrdersID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }
        }

        private void FillDecorPackContent(int PackageID, int Group)
        {
            DecorPackContentDataTable.Clear();


            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (Group == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (DecorPackContentDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in DecorPackContentDataTable.Rows)
                {
                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }
        }

        public void GetLabelInfo(ref DataTable EventsDataTable, string Barcode)
        {
            string BarType = Barcode.Substring(0, 3);

            if (BarType == "001" || BarType == "002")//ЗОВ
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, DocNumber, ClientID, ProfilPackCount, TPSPackCount, DocDateTime FROM MainOrders" +
                    " WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)) + ")",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {

                        DA.Fill(DT);

                        using (SqlDataAdapter MegaDA = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate FROM MegaOrders WHERE MegaOrderID = " + DT.Rows[0]["MegaOrderID"],
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            using (DataTable MegaDT = new DataTable())
                            {
                                MegaDA.Fill(MegaDT);


                                LabelInfo.ClientName = ZOVClientsDataTable.Select("ClientID = " + DT.Rows[0]["ClientID"])[0]["ClientName"].ToString();

                                if (BarType == "001")//profil zov
                                    LabelInfo.Factory = "Профиль";
                                else
                                    LabelInfo.Factory = "ТПС";

                                int TotalPackCount = 0;

                                SetPacked(ref EventsDataTable, false, Barcode);

                                if (MegaDT.Rows[0]["DispatchDate"] != DBNull.Value)
                                    LabelInfo.DispatchDate = Convert.ToDateTime(MegaDT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");

                                if (LabelInfo.Factory == "Профиль")
                                {
                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]);

                                    int PackedCount = GetPackedCount(1, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "ЗОВ");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }
                                else
                                {
                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);

                                    int PackedCount = GetPackedCount(2, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "ЗОВ");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }


                                if (LabelInfo.DispatchDate.Length > 0)
                                    if (Convert.ToDateTime(LabelInfo.DispatchDate) < Convert.ToDateTime(Security.GetCurrentDate().ToString("dd.MM.yyyy")))
                                        LabelInfo.DispatchDateColor = Color.Red;
                                    else
                                        LabelInfo.DispatchDateColor = Color.FromArgb(82, 169, 24);

                                LabelInfo.OrderDate = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]).ToString("dd.MM.yyyy");

                                LabelInfo.MegaOrderNumber = MegaDT.Rows[0]["MegaOrderID"].ToString();

                                LabelInfo.MainOrderNumber = DT.Rows[0]["DocNumber"].ToString();

                                LabelInfo.MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                                LabelInfo.MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                                LabelInfo.Group = "ЗОВ";



                                using (SqlDataAdapter pDA = new SqlDataAdapter("SELECT PackNumber FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                                    ConnectionStrings.ZOVOrdersConnectionString))
                                {
                                    using (DataTable pDT = new DataTable())
                                    {
                                        pDA.Fill(pDT);
                                        LabelInfo.CurrentPackNumber = pDT.Rows[0]["PackNumber"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }


                if (CheckProductType(Barcode, LabelInfo.Group) == 0)
                {
                    LabelInfo.ProductType = "Фасады";
                    FillFrontsPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 1);
                    FrontsPackContentDataGrid.BringToFront();
                }
                else
                {
                    LabelInfo.ProductType = "Погонаж и декор";
                    FillDecorPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 1);
                    DecorPackContentDataGrid.BringToFront();
                }

                return;
            }


            if (BarType == "003" || BarType == "004")//Маркетинг
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, ProfilPackCount, TPSPackCount FROM MainOrders" +
                    " WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)) + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {

                        DA.Fill(DT);

                        using (SqlDataAdapter MegaDA = new SqlDataAdapter("SELECT MegaOrderID, ClientID, ProfilDispatchDate, TPSDispatchDate, OrderDate, OrderNumber" +
                            " FROM MegaOrders WHERE MegaOrderID = " + DT.Rows[0]["MegaOrderID"], ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            using (DataTable MegaDT = new DataTable())
                            {
                                MegaDA.Fill(MegaDT);


                                LabelInfo.ClientName = MarktClientsDataTable.Select("ClientID = " + MegaDT.Rows[0]["ClientID"])[0]["ClientName"].ToString();

                                if (BarType == "003")//profil markt
                                    LabelInfo.Factory = "Профиль";
                                else
                                    LabelInfo.Factory = "ТПС";

                                int TotalPackCount = 0;

                                SetPacked(ref EventsDataTable, true, Barcode);

                                if (LabelInfo.Factory == "Профиль")
                                {
                                    if (MegaDT.Rows[0]["ProfilDispatchDate"] != DBNull.Value)
                                        LabelInfo.DispatchDate = Convert.ToDateTime(MegaDT.Rows[0]["ProfilDispatchDate"]).ToString("dd.MM.yyyy");

                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]);

                                    int PackedCount = GetPackedCount(1, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "Маркетинг");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }
                                else
                                {
                                    if (MegaDT.Rows[0]["TPSDispatchDate"] != DBNull.Value)
                                        LabelInfo.DispatchDate = Convert.ToDateTime(MegaDT.Rows[0]["TPSDispatchDate"]).ToString("dd.MM.yyyy");

                                    TotalPackCount = Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);

                                    int PackedCount = GetPackedCount(2, Convert.ToInt32(DT.Rows[0]["MainOrderID"]), "Маркетинг");

                                    LabelInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();

                                    if (PackedCount == TotalPackCount)
                                        LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                                    else
                                        LabelInfo.TotalLabelColor = Color.White;
                                }

                                if (!string.IsNullOrEmpty(LabelInfo.DispatchDate) &&
                                    Convert.ToDateTime(LabelInfo.DispatchDate) < Convert.ToDateTime(Security.GetCurrentDate().ToString("dd.MM.yyyy")))
                                    LabelInfo.DispatchDateColor = Color.Red;
                                else
                                    LabelInfo.DispatchDateColor = Color.FromArgb(82, 169, 24);

                                LabelInfo.OrderDate = Convert.ToDateTime(MegaDT.Rows[0]["OrderDate"]).ToString("dd.MM.yyyy");

                                LabelInfo.MegaOrderNumber = MegaDT.Rows[0]["OrderNumber"].ToString();

                                LabelInfo.MainOrderNumber = DT.Rows[0]["MainOrderID"].ToString();
                                LabelInfo.MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                                LabelInfo.MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                                LabelInfo.Group = "Маркетинг";



                                using (SqlDataAdapter pDA = new SqlDataAdapter("SELECT PackNumber FROM Packages WHERE PackageID = " +
                                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                                {
                                    using (DataTable pDT = new DataTable())
                                    {
                                        pDA.Fill(pDT);
                                        LabelInfo.CurrentPackNumber = pDT.Rows[0]["PackNumber"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }


                if (CheckProductType(Barcode, LabelInfo.Group) == 0)
                {
                    LabelInfo.ProductType = "Фасады";
                    FillFrontsPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 2);
                    FrontsPackContentDataGrid.BringToFront();
                }
                else
                {
                    LabelInfo.ProductType = "Погонаж и декор";
                    FillDecorPackContent(Convert.ToInt32(Barcode.Substring(3, 9)), 2);
                    DecorPackContentDataGrid.BringToFront();
                }

                return;
            }
        }

        public bool CheckBarcode(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        private void SetPacked(ref DataTable EventsDataTable, bool IsMarketing, string Barcode)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            string ConnectionString = ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!IsMarketing)
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID," +
                " PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime, PackUserID, StoreUserID, ExpUserID, DispUserID FROM Packages WHERE PackageID = " +
                Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (IsMarketing)
                        {
                            //статус упаковки Склад
                            //если упаковка не была отсканировна упаковщиками, то выставляем дату Упаковки PackingDateTime
                            //выставляем дату Принятия на склад
                            //убираем дату Дату экспедиции
                            //убираем дату Отгрузки
                            DT.Rows[0]["PackageStatusID"] = 2;
                            if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                DT.Rows[0]["PackingDateTime"] = CurrentDate;
                            if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
                                DT.Rows[0]["StorageDateTime"] = CurrentDate;

                            if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                                DT.Rows[0]["PackUserID"] = iUserID;
                            if (DT.Rows[0]["StoreUserID"] == DBNull.Value)
                                DT.Rows[0]["StoreUserID"] = iUserID;
                        }
                        else
                        {
                            using (SqlDataAdapter DA1 = new SqlDataAdapter("SELECT TOP 1 * FROM PackageDetails WHERE PackageID = " +
                                Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionString))
                            {
                                using (DataTable DT1 = new DataTable())
                                {
                                    DA1.Fill(DT1);
                                    if (DT1.Rows.Count > 0)
                                    {
                                        int OrderID = Convert.ToInt32(DT1.Rows[0]["OrderID"]);
                                        using (SqlDataAdapter DA2 = new SqlDataAdapter("SELECT * FROM PackagesEvents WHERE OrderID = " + OrderID, ConnectionString))
                                        {
                                            using (DataTable DT2 = new DataTable())
                                            {
                                                DA2.Fill(DT2);
                                                if (DT2.Rows.Count > 0)
                                                {
                                                    if (Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) != 3 && Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) != 4)
                                                        DT.Rows[0]["PackageStatusID"] = 2;
                                                    if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                                        DT.Rows[0]["PackingDateTime"] = DT2.Rows[0]["PackingDateTime"];
                                                    if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
                                                        DT.Rows[0]["StorageDateTime"] = DT2.Rows[0]["StorageDateTime"];
                                                    //DT.Rows[0]["ExpeditionDateTime"] = DBNull.Value;
                                                    //DT.Rows[0]["DispatchDateTime"] = DBNull.Value;

                                                    if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                                                        DT.Rows[0]["PackUserID"] = DT2.Rows[0]["PackUserID"];
                                                    if (DT.Rows[0]["StoreUserID"] == DBNull.Value)
                                                        DT.Rows[0]["StoreUserID"] = DT2.Rows[0]["StoreUserID"];
                                                    //DT.Rows[0]["ExpUserID"] = DBNull.Value;
                                                    //DT.Rows[0]["DispUserID"] = DBNull.Value;
                                                }
                                                else
                                                {
                                                    if (Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) != 3 && Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) != 4)
                                                        DT.Rows[0]["PackageStatusID"] = 2;
                                                    if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                                        DT.Rows[0]["PackingDateTime"] = CurrentDate;
                                                    if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
                                                        DT.Rows[0]["StorageDateTime"] = CurrentDate;
                                                    //DT.Rows[0]["ExpeditionDateTime"] = DBNull.Value;
                                                    //DT.Rows[0]["DispatchDateTime"] = DBNull.Value;

                                                    if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                                                        DT.Rows[0]["PackUserID"] = iUserID;
                                                    if (DT.Rows[0]["StoreUserID"] == DBNull.Value)
                                                        DT.Rows[0]["StoreUserID"] = iUserID;
                                                    //DT.Rows[0]["ExpUserID"] = DBNull.Value;
                                                    //DT.Rows[0]["DispUserID"] = DBNull.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public int GetPackedCount(int FactoryID, int MainOrderID, string Group)
        {
            string ConnectionString;

            if (Group == "Маркетинг")
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages" +
                " WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID + " AND (PackageStatusID = 1 OR PackageStatusID = 2)", ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    return DA.Fill(DT);
                }
            }
        }

        public int CheckProductType(string Barcode, string Group)
        {
            string ConnectionString;

            if (Group == "Маркетинг")
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProductType FROM Packages WHERE PackageID = " + Barcode.Substring(3, 9), ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToInt32(DT.Rows[0]["ProductType"]);
                }
            }
        }

        public void Clear()
        {
            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            LabelInfo.ClientName = "";
            LabelInfo.CurrentPackNumber = "";
            LabelInfo.DispatchDate = "";
            LabelInfo.DispatchDateColor = Color.White;
            LabelInfo.Factory = "";
            LabelInfo.Group = "";
            LabelInfo.MainOrderNumber = "";
            LabelInfo.MainOrderID = 0;
            LabelInfo.MegaOrderNumber = "";
            LabelInfo.OrderDate = "";
            LabelInfo.PackedToTotal = "";
            LabelInfo.ProductType = "";
            LabelInfo.TotalLabelColor = Color.White;
        }

        public void SetGridColor(string ProductType, bool IsAccept)
        {
            if (IsAccept)
            {
                if (ProductType == "Фасады")
                {
                    FrontsPackContentDataGrid.StateCommon.Background.Color1 = Color.FromArgb(82, 169, 24);
                    FrontsPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                    FrontsPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                    FrontsPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.FromArgb(82, 169, 24);
                    FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;
                }

                if (ProductType == "Погонаж и декор")
                {
                    DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.FromArgb(82, 169, 24);
                    DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                    DecorPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                    DecorPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.FromArgb(82, 169, 24);
                    DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;
                }
            }
            else
            {
                FrontsPackContentDataGrid.StateCommon.Background.Color1 = Color.Red;
                FrontsPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                FrontsPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = Color.FromArgb(240, 0, 0);

                DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.Red;
                DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                DecorPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = Color.FromArgb(240, 0, 0);
            }
        }
    }





    public class ExpeditionCheckLabel
    {
        int iUserID = 0;
        int CurrentGroup = 0;
        int CurrentProductType = 0;
        int CurrentClientID = 0;
        int CurrentFactoryID = 0;
        public int CurrentMainOrderID = 0;
        public int CurrentMegaOrderID = 0;

        PercentageDataGrid FrontsPackContentDataGrid = null;
        PercentageDataGrid DecorPackContentDataGrid = null;
        PercentageDataGrid PackagesDataGrid = null;

        DataTable FrontsPackContentDataTable = null;
        DataTable DecorPackContentDataTable = null;
        DataTable PackagesDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;

        DataTable ZOVClientsDataTable;
        DataTable MarktClientsDataTable;
        DataTable PackageStatusesDataTable;

        DataTable DecorDataTable;
        DataTable DecorProductsDataTable;

        DataTable PackageDetailsDT;
        DataTable DecorAssignmentsDT;
        DataTable StoreDT;
        DataTable ManufactureStoreDT;
        DataTable ReadyStoreDT;
        DataTable MovementInvoicesDT;
        DataTable MovementInvoiceDetailsDT;

        SqlDataAdapter ManufactureStoreDA;
        SqlCommandBuilder ManufactureStoreCB;
        SqlDataAdapter ReadyStoreDA;
        SqlCommandBuilder ReadyStoreCB;
        SqlDataAdapter MovementInvoicesDA;
        SqlCommandBuilder MovementInvoicesCB;
        SqlDataAdapter MovementInvoiceDetailsDA;
        SqlCommandBuilder MovementInvoiceDetailsCB;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        DataGridViewComboBoxColumn PackageStatusesColumn = null;

        public BindingSource FrontsPackContentBindingSource = null;
        public BindingSource DecorPackContentBindingSource = null;
        public BindingSource PackagesBindingSource = null;
        public BindingSource PackageStatusesBindingSource = null;

        public struct Labelinfo
        {
            public int PackageCount;
            public string OrderDate;
            public string Group;
        }

        public Labelinfo LabelInfo;

        public int UserID
        {
            get { return iUserID; }
            set { iUserID = value; }
        }

        public ExpeditionCheckLabel(ref PercentageDataGrid tFrontsPackContentDataGrid, ref PercentageDataGrid tDecorPackContentDataGrid,
            ref PercentageDataGrid tPackagesDataGrid)
        {
            FrontsPackContentDataGrid = tFrontsPackContentDataGrid;
            DecorPackContentDataGrid = tDecorPackContentDataGrid;
            PackagesDataGrid = tPackagesDataGrid;

            Initialize();
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            FrontsGridSettings();
            DecorGridSettings();
            PackagesGridSetting();
        }

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            FrontsPackContentDataTable = new DataTable();
            DecorPackContentDataTable = new DataTable();
            PackagesDataTable = new DataTable();
            PackageStatusesDataTable = new DataTable();

            PackageDetailsDT = new DataTable();
            DecorAssignmentsDT = new DataTable();
            StoreDT = new DataTable();
            ManufactureStoreDT = new DataTable();
            ManufactureStoreDA = new SqlDataAdapter("SELECT TOP 0 * FROM ManufactureStore",
                ConnectionStrings.StorageConnectionString);
            ManufactureStoreCB = new SqlCommandBuilder(ManufactureStoreDA);
            ManufactureStoreDA.Fill(ManufactureStoreDT);

            ReadyStoreDT = new DataTable();
            ReadyStoreDA = new SqlDataAdapter("SELECT TOP 0 * FROM ReadyStore",
                ConnectionStrings.StorageConnectionString);
            ReadyStoreCB = new SqlCommandBuilder(ReadyStoreDA);
            ReadyStoreDA.Fill(ReadyStoreDT);

            MovementInvoicesDT = new DataTable();
            MovementInvoicesDA = new SqlDataAdapter("SELECT TOP 1 * FROM MovementInvoices ORDER BY MovementInvoiceID DESC",
                ConnectionStrings.StorageConnectionString);
            MovementInvoicesCB = new SqlCommandBuilder(MovementInvoicesDA);
            MovementInvoicesDA.Fill(MovementInvoicesDT);

            MovementInvoiceDetailsDT = new DataTable();
            MovementInvoiceDetailsDA = new SqlDataAdapter("SELECT TOP 0 * FROM MovementInvoiceDetails",
                ConnectionStrings.StorageConnectionString);
            MovementInvoiceDetailsCB = new SqlCommandBuilder(MovementInvoiceDetailsDA);
            MovementInvoiceDetailsDA.Fill(MovementInvoiceDetailsDT);
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow[] rows = FrameColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = FrameColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            FrameColorsDataTable.Rows.Add(NewRow);
                        }
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails" +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDataTable);
            }

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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL WHERE Enabled=1",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaRALDataTable);
            }
            foreach (DataRow item in PatinaRALDataTable.Rows)
            {
                DataRow NewRow = PatinaDataTable.NewRow();
                NewRow["PatinaID"] = item["PatinaRALID"];
                NewRow["PatinaName"] = item["PatinaRAL"];
                NewRow["DisplayName"] = item["DisplayName"];
                PatinaDataTable.Rows.Add(NewRow);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            MarktClientsDataTable = new DataTable();
            ZOVClientsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MarktClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZOVClientsDataTable);
            }

            string SelectionCommand = "SELECT TOP 0 Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " CAST(MegaOrders.OrderNumber AS CHAR) AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }

            PackagesDataTable.Columns.Add(new DataColumn("Group", Type.GetType("System.Int32")));
        }

        private void Binding()
        {
            PackageStatusesBindingSource = new BindingSource()
            {
                DataSource = PackageStatusesDataTable
            };
            FrontsPackContentBindingSource = new BindingSource()
            {
                DataSource = FrontsPackContentDataTable
            };
            FrontsPackContentDataGrid.DataSource = FrontsPackContentBindingSource;

            DecorPackContentBindingSource = new BindingSource()
            {
                DataSource = DecorPackContentDataTable
            };
            DecorPackContentDataGrid.DataSource = DecorPackContentBindingSource;

            PackagesBindingSource = new BindingSource()
            {
                DataSource = PackagesDataTable
            };
            PackagesDataGrid.DataSource = PackagesBindingSource;
        }

        #region GridSettings

        private void CreateColumns()
        {
            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = new DataView(FrontsDataTable),
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = new DataView(PatinaDataTable),
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = new DataView(InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = new DataView(InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoProfilesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoProfilesColumn",
                HeaderText = "Тип\r\nпрофиля-2",
                DataPropertyName = "TechnoProfileID",
                DataSource = new DataView(TechnoProfilesDataTable),
                ValueMember = "TechnoProfileID",
                DisplayMember = "TechnoProfileName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoFrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoFrameColorsColumn",
                HeaderText = "Цвет профиля-2",
                DataPropertyName = "TechnoColorID",
                DataSource = new DataView(FrameColorsDataTable),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(TechnoInsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(TechnoInsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PackageStatusesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PackageStatusesColumn",
                HeaderText = "Статус",
                DataPropertyName = "PackageStatusID",
                DataSource = PackageStatusesBindingSource,
                ValueMember = "PackageStatusID",
                DisplayMember = "PackageStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            PackagesDataGrid.Columns.Add(PackageStatusesColumn);

            FrontsPackContentDataGrid.Columns.Add(FrontsColumn);
            FrontsPackContentDataGrid.Columns.Add(FrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(PatinaColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(InsetColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsPackContentDataGrid.Columns.Add(TechnoInsetColorsColumn);

            DecorPackContentDataGrid.Columns.Add(ProductColumn);
            DecorPackContentDataGrid.Columns.Add(ItemColumn);
            DecorPackContentDataGrid.Columns.Add(ColorColumn);
            DecorPackContentDataGrid.Columns.Add(DecorPatinaColumn);
        }

        public DataGridViewComboBoxColumn ProductColumn
        {
            get
            {
                DataGridViewComboBoxColumn ProductColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "ProductColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "ProductID",

                    DataSource = new DataView(DecorProductsDataTable),
                    ValueMember = "ProductID",
                    DisplayMember = "ProductName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ProductColumn;
            }
        }

        public DataGridViewComboBoxColumn ItemColumn
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

        public DataGridViewComboBoxColumn ColorColumn
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

        public DataGridViewComboBoxColumn DecorPatinaColumn
        {
            get
            {
                DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn",
                    HeaderText = "Патина",
                    DataPropertyName = "PatinaID",

                    DataSource = new DataView(PatinaDataTable),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return PatinaColumn;
            }
        }

        private void FrontsGridSettings()
        {
            foreach (DataGridViewColumn Column in FrontsPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsPackContentDataGrid.Columns["FrontID"].Visible = false;
            FrontsPackContentDataGrid.Columns["ColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            int DisplayIndex = 0;
            FrontsPackContentDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;

            FrontsPackContentDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Height"].Width = 90;
            FrontsPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Width"].Width = 90;
            FrontsPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void DecorGridSettings()
        {
            foreach (DataGridViewColumn Column in DecorPackContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorPackContentDataGrid.Columns["ProductID"].Visible = false;
            DecorPackContentDataGrid.Columns["DecorID"].Visible = false;
            DecorPackContentDataGrid.Columns["ColorID"].Visible = false;
            DecorPackContentDataGrid.Columns["PatinaID"].Visible = false;


            DecorPackContentDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorPackContentDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorPackContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorPackContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            DecorPackContentDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            DecorPackContentDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            DecorPackContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;

            DecorPackContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Height"].Width = 90;
            DecorPackContentDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Length"].Width = 90;
            DecorPackContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Width"].Width = 90;
            DecorPackContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorPackContentDataGrid.Columns["Count"].Width = 90;
        }

        private void PackagesGridSetting()
        {
            foreach (DataGridViewColumn Column in PackagesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            PackagesDataGrid.Columns["PackageStatusID"].Visible = false;
            PackagesDataGrid.Columns["MainOrderID"].Visible = false;
            PackagesDataGrid.Columns["PrintDateTime"].Visible = false;
            PackagesDataGrid.Columns["PrintedCount"].Visible = false;
            PackagesDataGrid.Columns["MainOrderID"].Visible = false;
            PackagesDataGrid.Columns["DispatchDateTime"].Visible = false;
            PackagesDataGrid.Columns["PalleteID"].Visible = false;
            PackagesDataGrid.Columns["Group"].Visible = false;

            if (PackagesDataGrid.Columns.Contains("PalleteID"))
                PackagesDataGrid.Columns["PalleteID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("DispatchID"))
                PackagesDataGrid.Columns["DispatchID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("PackUserID"))
                PackagesDataGrid.Columns["PackUserID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("StoreUserID"))
                PackagesDataGrid.Columns["StoreUserID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("ExpUserID"))
                PackagesDataGrid.Columns["ExpUserID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("DispUserID"))
                PackagesDataGrid.Columns["DispUserID"].Visible = false;

            if (PackagesDataGrid.Columns.Contains("ProductType"))
                PackagesDataGrid.Columns["ProductType"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("PackingDateTime"))
                PackagesDataGrid.Columns["PackingDateTime"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("TrayID"))
                PackagesDataGrid.Columns["TrayID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("FactoryID"))
                PackagesDataGrid.Columns["FactoryID"].Visible = false;

            PackagesDataGrid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            PackagesDataGrid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            PackagesDataGrid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            PackagesDataGrid.Columns["PackageID"].HeaderText = "ID";
            PackagesDataGrid.Columns["FactoryName"].HeaderText = "Участок";
            PackagesDataGrid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            PackagesDataGrid.Columns["Notes"].HeaderText = "Примечание";
            PackagesDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            PackagesDataGrid.Columns["Group"].HeaderText = "Группа";
            PackagesDataGrid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";

            PackagesDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackNumber"].Width = 70;
            PackagesDataGrid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageStatusesColumn"].Width = 140;
            PackagesDataGrid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["StorageDateTime"].Width = 170;
            PackagesDataGrid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["ExpeditionDateTime"].Width = 170;
            PackagesDataGrid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageID"].Width = 100;
            PackagesDataGrid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["FactoryName"].Width = 100;
            PackagesDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PackagesDataGrid.Columns["ClientName"].MinimumWidth = 190;
            PackagesDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PackagesDataGrid.Columns["OrderNumber"].MinimumWidth = 150;
            PackagesDataGrid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["Group"].Width = 70;
            PackagesDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PackagesDataGrid.Columns["Notes"].MinimumWidth = 100;

            PackagesDataGrid.AutoGenerateColumns = false;

            PackagesDataGrid.Columns["ClientName"].DisplayIndex = 0;
            PackagesDataGrid.Columns["OrderNumber"].DisplayIndex = 1;
            PackagesDataGrid.Columns["Notes"].DisplayIndex = 2;
            PackagesDataGrid.Columns["PackageStatusesColumn"].DisplayIndex = 3;
            PackagesDataGrid.Columns["FactoryName"].DisplayIndex = 4;
            PackagesDataGrid.Columns["StorageDateTime"].DisplayIndex = 5;
            PackagesDataGrid.Columns["ExpeditionDateTime"].DisplayIndex = 6;
            PackagesDataGrid.Columns["Group"].DisplayIndex = 7;
            PackagesDataGrid.Columns["PackageID"].DisplayIndex = 8;
        }

        #endregion

        #region Fill

        public bool FillFrontsPackContent(int PackageID)
        {
            FrontsPackContentDataTable.Clear();

            //if (CurrentProductType == 1)
            //    return false;

            if (CurrentGroup == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }

            if (CurrentGroup == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN FrontsOrders ON PackageDetails.OrderID=FrontsOrders.FrontsOrdersID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }

            return FrontsPackContentDataTable.Rows.Count > 0;
        }

        public bool FillDecorPackContent(int PackageID)
        {
            DecorPackContentDataTable.Clear();

            //if (CurrentProductType == 0)
            //    return false;

            if (CurrentGroup == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (CurrentGroup == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count FROM PackageDetails " +
                    " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (DecorPackContentDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in DecorPackContentDataTable.Rows)
                {
                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                    //    Row["ColorID"] = 0;
                }
            }

            return DecorPackContentDataTable.Rows.Count > 0;
        }


        public bool HasPackages(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    CurrentGroup = 1;
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    CurrentGroup = 2;
                }
            }
            return DT.Rows.Count > 0;
        }

        public bool FilterByPackageID(string Barcode)
        {
            int Group = 1;

            string Prefix = Barcode.Substring(0, 3);

            PackagesDataGrid.DataSource = null;

            PackagesDataTable.Clear();

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "001" || Prefix == "002")
            {
                SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                   " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                   " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                   " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                   " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                   " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                   " WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9));

                Group = 1;
                LabelInfo.Group = "ЗОВ";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                Group = 2;
                LabelInfo.Group = "Маркетинг";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            LabelInfo.PackageCount = PackagesDataTable.Rows.Count;

            if (PackagesDataTable.Rows.Count > 0)
                PackagesDataTable.Rows[0]["Group"] = Group;

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        public bool HasTray(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    CurrentGroup = 1;
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    CurrentGroup = 2;
                }
            }
            return DT.Rows.Count > 0;
        }

        public bool FillPackages(string Barcode)
        {
            int Group = 1;

            string Prefix = Barcode.Substring(0, 3);

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " CAST(MegaOrders.OrderNumber AS CHAR) AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

            PackagesDataGrid.DataSource = null;

            PackagesDataTable.Clear();

            if (Prefix == "005")
            {
                Group = 1;
                LabelInfo.Group = "ЗОВ";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                SelectionCommand = "SELECT Packages.*, infiniu2_zovreference.dbo.Clients.ClientName," +
                    " MainOrders.DocNumber AS OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                    " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                    " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                    " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9));

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            if (Prefix == "006")
            {
                Group = 2;
                LabelInfo.Group = "Маркетинг";
                LabelInfo.OrderDate = GetOrderDate(Barcode);

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(PackagesDataTable);
                }
            }

            LabelInfo.PackageCount = PackagesDataTable.Rows.Count;

            for (int i = 0; i < PackagesDataTable.Rows.Count; i++)
            {
                PackagesDataTable.Rows[i]["Group"] = Group;
            }

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// true, если таблица упаковок Packages пуста
        /// </summary>
        public bool IsPackagesTableEmpty
        {
            get { return PackagesDataTable.Rows.Count == 0; }
        }

        public void GetPackageInfo(int PackageID)
        {
            if (CurrentGroup == 2)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductType, Packages.FactoryID, Packages.MainOrderID, MainOrders.MegaOrderID, MegaOrders.ClientID FROM Packages
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID WHERE PackageID = " + PackageID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        CurrentFactoryID = Convert.ToInt32(DT.Rows[0]["FactoryID"]);
                        CurrentClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                        CurrentProductType = Convert.ToInt32(DT.Rows[0]["ProductType"]);
                        CurrentMainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                        CurrentMegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                    }
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ProductType, Packages.MainOrderID, MainOrders.MegaOrderID FROM Packages
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID WHERE PackageID = " + PackageID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        CurrentProductType = Convert.ToInt32(DT.Rows[0]["ProductType"]);
                        CurrentMainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                        CurrentMegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                    }
                }
            }

            return;
        }

        #endregion

        #region Set

        //public void ОТГРУЗКАДОЛГОВ()
        //{
        //    int NoReorder = 0;
        //    int FoundReorder = 0;
        //    int NotFoundReorder = 0;
        //    int DispatchEntirely = 0;
        //    int DispatchNotEntirely = 0;
        //    int DebtMainOrderID = -1;
        //    int MainOrderID = 0;
        //    int DebtTypeID = 0;
        //    int FactoryID = 0;
        //    int ProfilProductionStatusID = 0;
        //    int ProfilStorageStatusID = 0;
        //    int ProfilDispatchStatusID = 0;
        //    int TPSProductionStatusID = 0;
        //    int TPSStorageStatusID = 0;
        //    int TPSDispatchStatusID = 0;
        //    string DocNumber = string.Empty;
        //    string DebtDocNumber = string.Empty;
        //    string ReorderDocNumber = string.Empty;

        //    DataTable DoNotDispatchDT = new DataTable();
        //    DataTable ReordersDT = new DataTable();
        //    SqlDataAdapter DoNotDispatchDA = new SqlDataAdapter("SELECT MainOrderID, DocNumber, DebtDocNumber, ReorderDocNumber, DocDateTime, DebtTypeID, FactoryID, ProfilProductionStatusID, ProfilStorageStatusID," +
        //        " ProfilDispatchStatusID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID FROM MainOrders" +
        //        " WHERE ReorderDocNumber IS NOT NULL AND FactoryID = 2", ConnectionStrings.ZOVOrdersConnectionString);
        //    SqlCommandBuilder DoNotDispatchCB = new SqlCommandBuilder(DoNotDispatchDA);
        //    DoNotDispatchDA.Fill(DoNotDispatchDT);

        //    for (int i = 0; i < DoNotDispatchDT.Rows.Count; i++)
        //    {
        //        MainOrderID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["MainOrderID"]);
        //        //FactoryID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["FactoryID"]);
        //        //DebtTypeID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["DebtTypeID"]);
        //        //ProfilProductionStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["ProfilProductionStatusID"]);
        //        //ProfilStorageStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["ProfilStorageStatusID"]);
        //        //ProfilDispatchStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["ProfilDispatchStatusID"]);
        //        //TPSProductionStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["TPSProductionStatusID"]);
        //        //TPSStorageStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["TPSStorageStatusID"]);
        //        //TPSDispatchStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["TPSDispatchStatusID"]);
        //        DocNumber = DoNotDispatchDT.Rows[i]["DocNumber"].ToString();
        //        DebtDocNumber = DoNotDispatchDT.Rows[i]["DebtDocNumber"].ToString();
        //        ReorderDocNumber = DoNotDispatchDT.Rows[i]["ReorderDocNumber"].ToString();

        //        if (DoNotDispatchDT.Rows[i]["ReorderDocNumber"] == DBNull.Value
        //            || ReorderDocNumber.Length < 2)
        //        {
        //            NoReorder++;
        //            continue;
        //        }
        //        DebtMainOrderID = GetReorderMainOrderID(ReorderDocNumber);
        //        //DebtMainOrderID = GetDebtMainOrderID(DebtDocNumber);
        //        if (DebtMainOrderID != -1)
        //        {
        //            FoundReorder++;
        //            if (IsDebtDispatchEntirely(DebtMainOrderID))
        //            {
        //                DispatchEntirely++;
        //                DispatchDebt(MainOrderID);
        //            }
        //            else
        //                DispatchNotEntirely++;
        //        }
        //        else
        //            NotFoundReorder++;
        //    }
        //}

        public int GetReorderMainOrderID(string ReorderDocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocNumber FROM MainOrders" +
                " WHERE DocNumber = '" + ReorderDocNumber + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    else
                        return -1;
                }
            }
        }

        //public void ОТГРУЗКАДОЛГОВ()
        //{
        //    int NoReorder = 0;
        //    int FoundReorder = 0;
        //    int NotFoundReorder = 0;
        //    int DispatchEntirely = 0;
        //    int DispatchNotEntirely = 0;
        //    int DebtMainOrderID = -1;
        //    int MainOrderID = 0;
        //    //int DebtTypeID = 0;
        //    //int FactoryID = 0;
        //    //int ProfilProductionStatusID = 0;
        //    //int ProfilStorageStatusID = 0;
        //    //int ProfilDispatchStatusID = 0;
        //    //int TPSProductionStatusID = 0;
        //    //int TPSStorageStatusID = 0;
        //    //int TPSDispatchStatusID = 0;
        //    string DocNumber = string.Empty;
        //    string DebtDocNumber = string.Empty;
        //    string ReorderDocNumber = string.Empty;
        //    //            SELECT        SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Expr1, SUM(dbo.FrontsOrders.Cost) AS Expr2, 
        //    //                         dbo.MegaOrders.DispatchDate, dbo.FrontsOrders.MainOrderID
        //    //FROM            dbo.PackageDetails INNER JOIN
        //    //                         dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
        //    //                         dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND dbo.Packages.PackageStatusID <> 3 AND 
        //    //                         dbo.Packages.ProductType = 0 INNER JOIN
        //    //                         dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DoNotDispatch = 1 INNER JOIN
        //    //                         infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
        //    //                         dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID
        //    //GROUP BY dbo.MegaOrders.DispatchDate, dbo.FrontsOrders.MainOrderID
        //    //ORDER BY dbo.MegaOrders.DispatchDate
        //    DataTable DoNotDispatchDT = new DataTable();
        //    DataTable ReordersDT = new DataTable();
        //    SqlDataAdapter DoNotDispatchDA = new SqlDataAdapter("SELECT MainOrderID, DocNumber, DebtDocNumber, ReorderDocNumber, DocDateTime, DebtTypeID, FactoryID, ProfilProductionStatusID, ProfilStorageStatusID," +
        //        " ProfilDispatchStatusID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID FROM MainOrders" +
        //        " WHERE DebtTypeID <> 0 AND FactoryID = 2", ConnectionStrings.ZOVOrdersConnectionString);
        //    SqlCommandBuilder DoNotDispatchCB = new SqlCommandBuilder(DoNotDispatchDA);
        //    DoNotDispatchDA.Fill(DoNotDispatchDT);

        //    for (int i = 0; i < DoNotDispatchDT.Rows.Count; i++)
        //    {
        //        MainOrderID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["MainOrderID"]);
        //        //FactoryID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["FactoryID"]);
        //        //DebtTypeID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["DebtTypeID"]);
        //        //ProfilProductionStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["ProfilProductionStatusID"]);
        //        //ProfilStorageStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["ProfilStorageStatusID"]);
        //        //ProfilDispatchStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["ProfilDispatchStatusID"]);
        //        //TPSProductionStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["TPSProductionStatusID"]);
        //        //TPSStorageStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["TPSStorageStatusID"]);
        //        //TPSDispatchStatusID = Convert.ToInt32(DoNotDispatchDT.Rows[i]["TPSDispatchStatusID"]);
        //        DocNumber = DoNotDispatchDT.Rows[i]["DocNumber"].ToString();
        //        DebtDocNumber = DoNotDispatchDT.Rows[i]["DebtDocNumber"].ToString();
        //        //ReorderDocNumber = DoNotDispatchDT.Rows[i]["ReorderDocNumber"].ToString();

        //        if (DoNotDispatchDT.Rows[i]["DebtDocNumber"] == DBNull.Value
        //            || DebtDocNumber.Length < 2)
        //        {
        //            NoReorder++;
        //            continue;
        //        }
        //        DebtMainOrderID = GetDebtMainOrderID(DebtDocNumber);
        //        if (DebtMainOrderID != -1)
        //        {
        //            FoundReorder++;
        //            if (IsDebtDispatchEntirely(DebtMainOrderID))
        //            {
        //                DispatchEntirely++;
        //                DispatchDebt(MainOrderID);
        //            }
        //            else
        //                DispatchNotEntirely++;
        //        }
        //        else
        //            NotFoundReorder++;
        //    }
        //}

        public bool IsDebtDispatchEntirely(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, FactoryID, PackageStatusID FROM Packages" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int AllCount = DT.Rows.Count;
                        int DispCount = DT.Select("PackageStatusID = 3").Count();
                        return AllCount == DispCount;
                    }
                    else
                        return false;
                }
            }
        }

        public int GetDebtMainOrderID(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocNumber FROM MainOrders" +
                " WHERE DocNumber IN (SELECT DebtDocNumber FROM MainOrders" +
                " WHERE MainOrderID = " + MainOrderID + ")", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    else
                        return -1;
                }
            }
        }

        public int GetDebtMainOrderID(string DebtDocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocNumber FROM MainOrders" +
                " WHERE DocNumber = '" + DebtDocNumber + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    else
                        return -1;
                }
            }
        }

        #endregion

        public bool CanPackageExp(int PackageID, ref string Message)
        {
            string ConnectionString;

            if (CurrentGroup == 1)//ZOV
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            int DispatchID = 0;
            object PrepareDispatchDateTime = null;
            using (SqlDataAdapter DA1 = new SqlDataAdapter("SELECT DispatchID FROM Packages" +
                " WHERE DispatchID IS NOT NULL AND PackageID = " + PackageID, ConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA1.Fill(DT1) > 0)
                    {
                        DispatchID = Convert.ToInt32(DT1.Rows[0]["DispatchID"]);
                        using (SqlDataAdapter DA2 = new SqlDataAdapter("SELECT DispatchID, PrepareDispatchDateTime, ConfirmExpDateTime FROM Dispatch" +
                            " WHERE DispatchID = " + DispatchID, ConnectionString))
                        {
                            using (DataTable DT2 = new DataTable())
                            {
                                if (DA2.Fill(DT2) > 0)
                                {
                                    PrepareDispatchDateTime = DT2.Rows[0]["PrepareDispatchDateTime"];
                                    if (PrepareDispatchDateTime != null)
                                        PrepareDispatchDateTime = Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy");
                                    if (DT2.Rows[0]["ConfirmExpDateTime"] != DBNull.Value)
                                        return true;
                                    else
                                        Message = "Экспедиция не разрешена. Дата отгрузки " + PrepareDispatchDateTime;
                                }
                                else
                                    Message = "Отгрузка №" + DispatchID + " не существует";
                            }
                        }
                    }
                    else
                        Message = "Упаковка не включена в отгрузку";
                }
            }
            return false;
        }

        public bool CanTrayExp(int TrayID, ref string Message)
        {
            string ConnectionString;

            if (CurrentGroup == 1)//ZOV
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            int DispatchID = 0;
            object PrepareDispatchDateTime = null;
            using (SqlDataAdapter DA1 = new SqlDataAdapter("SELECT DispatchID FROM Packages" +
                " WHERE DispatchID IS NOT NULL AND TrayID = " + TrayID, ConnectionString))
            {
                using (DataTable DT1 = new DataTable())
                {
                    if (DA1.Fill(DT1) > 0)
                    {
                        DataTable DT = new DataTable();
                        using (DataView DV = new DataView(DT1))
                        {
                            DT = DV.ToTable(true, new string[] { "DispatchID" });
                        }
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            DispatchID = Convert.ToInt32(DT.Rows[i]["DispatchID"]);
                            using (SqlDataAdapter DA2 = new SqlDataAdapter("SELECT DispatchID, PrepareDispatchDateTime, ConfirmExpDateTime FROM Dispatch" +
                                " WHERE DispatchID = " + DispatchID, ConnectionString))
                            {
                                using (DataTable DT2 = new DataTable())
                                {
                                    if (DA2.Fill(DT2) > 0)
                                    {
                                        PrepareDispatchDateTime = DT2.Rows[0]["PrepareDispatchDateTime"];
                                        if (PrepareDispatchDateTime != null)
                                            PrepareDispatchDateTime = Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy");
                                        if (DT2.Rows[0]["ConfirmExpDateTime"] != DBNull.Value)
                                            return true;
                                        else
                                            Message = "Отгрузка не разрешена. Дата отгрузки " + PrepareDispatchDateTime;
                                    }
                                    else
                                        Message = "Отгрузка №" + DispatchID + " не существует";
                                }
                            }
                        }
                    }
                    else
                        Message = "Упаковка не включена в отгрузку";
                }
            }
            return false;
        }

        public void SetExpDebt(int MainOrderID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpeditionDateTime FROM Packages" +
                " WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                //если долг уже отгружен
                                if (Convert.ToInt32(DT.Rows[i]["PackageStatusID"]) == 4)
                                    continue;
                                DT.Rows[i]["PackageStatusID"] = 3;
                                if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PackingDateTime"] = CurrentDate;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            int FactoryID = Convert.ToInt32(DT.Rows[0]["FactoryID"]);

                            if (FactoryID == 1)
                            {
                                //если долг уже отгружен
                                if (Convert.ToInt32(DT.Rows[0]["ProfilDispatchStatusID"]) == 2)
                                    return;
                                DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 2;
                            }

                            if (FactoryID == 2)
                            {
                                if (Convert.ToInt32(DT.Rows[0]["TPSDispatchStatusID"]) == 2)
                                    return;
                                DT.Rows[0]["TPSProductionStatusID"] = 1;
                                DT.Rows[0]["TPSStorageStatusID"] = 1;
                                DT.Rows[0]["TPSDispatchStatusID"] = 2;
                            }

                            if (FactoryID == 0)
                            {
                                if (Convert.ToInt32(DT.Rows[0]["ProfilDispatchStatusID"]) == 2
                                    && Convert.ToInt32(DT.Rows[0]["TPSDispatchStatusID"]) == 2)
                                    return;
                                DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 2;
                                DT.Rows[0]["TPSProductionStatusID"] = 1;
                                DT.Rows[0]["TPSStorageStatusID"] = 1;
                                DT.Rows[0]["TPSDispatchStatusID"] = 2;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void RemovePackageFromTray(int PackageID, ref int TrayID)
        {
            string ConnectionString;

            if (CurrentGroup == 1)//ZOV
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, DispatchID, TrayID FROM Packages" +
                " WHERE PackageID = " + PackageID, ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0]["TrayID"] != DBNull.Value)
                                TrayID = Convert.ToInt32(DT.Rows[0]["TrayID"]);
                            DT.Rows[0]["TrayID"] = DBNull.Value;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        //        public bool PreExpPackage(bool IsMarketing, int PackageID, ref string Message)
        //        {
        //            string ConnectionString;

        //            if (CurrentGroup == 1)//ZOV
        //                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
        //            else
        //                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

        //            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageID, PackageStatusID, 
        //                PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime FROM Packages" +
        //                " WHERE PackageID = " + PackageID, ConnectionString))
        //            {
        //                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
        //                {
        //                    using (DataTable DT = new DataTable())
        //                    {
        //                        if (DA.Fill(DT) > 0)
        //                        {
        //                            if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
        //                            {
        //                                Message = "Упаковка №" + DT.Rows[0]["PackageID"].ToString() + " не была принята на склад";
        //                                return false;
        //                            }
        //                            else
        //                                return true;
        //                        }
        //                        else
        //                        {
        //                            Message = "Упаковка №" + PackageID + " не существует";
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        public bool PreExpTray(bool IsMarketing, int TrayID, ref string Message)
        //        {
        //            string ConnectionString;

        //            if (CurrentGroup == 1)//ZOV
        //                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
        //            else
        //                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

        //            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageID, PackageStatusID, 
        //                PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime FROM Packages" +
        //                " WHERE TrayID = " + TrayID, ConnectionString))
        //            {
        //                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
        //                {
        //                    using (DataTable DT = new DataTable())
        //                    {
        //                        if (DA.Fill(DT) > 0)
        //                        {
        //                            for (int i = 0; i < DT.Rows.Count; i++)
        //                            {
        //                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
        //                                {
        //                                    Message = "Упаковка №" + DT.Rows[i]["PackageID"].ToString() + " не была принята на склад";
        //                                    return false;
        //                                }
        //                            }
        //                            return true;
        //                        }
        //                        else
        //                        {
        //                            Message = "На поддоне нет упаковок";
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        /// <summary>
        /// Выставляет статус "Экспедиция" для одной упаковки
        /// </summary>
        public bool SetExp(ref DataTable EventsDataTable, bool IsMarketing, int PackageID, ref string Message)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            string ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!IsMarketing)//ZOV
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, MainOrderID, PackageStatusID, PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime," +
                " PackUserID, StoreUserID, ExpUserID, DispUserID FROM Packages WHERE PackageID = " + PackageID, ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
                        {
                            Message = "Упаковка №" + PackageID + " не была принята на склад";
                            return false;
                        }

                        if (Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) != 3)
                        {
                            //DT.Rows[0]["DispUserID"] = DBNull.Value;
                            //DT.Rows[0]["DispatchDateTime"] = DBNull.Value;
                            DT.Rows[0]["PackageStatusID"] = 4;
                        }
                        if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                            DT.Rows[0]["PackingDateTime"] = CurrentDate;
                        if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
                            DT.Rows[0]["StorageDateTime"] = CurrentDate;
                        if (DT.Rows[0]["ExpeditionDateTime"] == DBNull.Value)
                            DT.Rows[0]["ExpeditionDateTime"] = CurrentDate;

                        if (DT.Rows[0]["PackUserID"] == DBNull.Value)
                            DT.Rows[0]["PackUserID"] = iUserID;
                        if (DT.Rows[0]["StoreUserID"] == DBNull.Value)
                            DT.Rows[0]["StoreUserID"] = iUserID;
                        if (DT.Rows[0]["ExpUserID"] == DBNull.Value)
                            DT.Rows[0]["ExpUserID"] = iUserID;

                        DA.Update(DT);
                        return true;
                    }
                }
            }

        }

        /// <summary>
        /// Выставляет статус "Экспедиция" для всех упаковок, находящихся на поддоне
        /// </summary>
        /// <param name="TrayID"></param>
        public void SetExp(string Barcode)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            string ConnectionString;

            if (CurrentGroup == 1)//ZOV
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime," +
                " PackUserID, StoreUserID, ExpUserID, DispUserID FROM Packages" +
                " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["PackageStatusID"] = 4;
                                if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PackingDateTime"] = CurrentDate;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;

                                if (DT.Rows[i]["PackUserID"] == DBNull.Value)
                                    DT.Rows[i]["PackUserID"] = iUserID;
                                if (DT.Rows[i]["StoreUserID"] == DBNull.Value)
                                    DT.Rows[i]["StoreUserID"] = iUserID;
                                if (DT.Rows[i]["ExpUserID"] == DBNull.Value)
                                    DT.Rows[i]["ExpUserID"] = iUserID;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID, TrayStatusID, StorageDateTime, ExpeditionDateTime FROM Trays" +
                " WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["TrayStatusID"] = 3;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public string GetOrderDate(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            string DateTime = string.Empty;

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Packages WHERE PackageID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Trays WHERE TrayID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StorageDateTime FROM Trays WHERE TrayID = " +
                    Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (DT.Rows[0][0] != DBNull.Value)
                                DateTime = Convert.ToDateTime(DT.Rows[0][0]).ToString("dd.MM.yyyy");
                        }

                    }
                }
            }

            return DateTime;
        }

        public bool IsPackageLabel(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public bool IsTrayLabel(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();
            PackagesDataTable.Clear();
        }

        public void WriteOffFromStore(int PackageID)
        {
            if (CurrentProductType == 0)
                return;
            if (CurrentGroup == 1)//ZOV
                return;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            MovementInvoiceDetailsDT.Clear();
            string SelectCommand = @"SELECT * FROM PackageDetails WHERE PackageID = " + PackageID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                PackageDetailsDT.Clear();
                DA.Fill(PackageDetailsDT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE DecorOrderID IN (SELECT OrderID FROM infiniu2_marketingorders.dbo.PackageDetails WHERE PackageID = " + PackageID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DecorAssignmentsDT.Clear();
                DA.Fill(DecorAssignmentsDT);
            }
            SelectCommand = @"SELECT * FROM Store WHERE DecorAssignmentID IN (SELECT DecorAssignmentID FROM DecorAssignments WHERE DecorOrderID IN (SELECT OrderID FROM infiniu2_marketingorders.dbo.PackageDetails WHERE PackageID = " + PackageID + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                StoreDT.Clear();
                DA.Fill(StoreDT);
            }
            SelectCommand = @"SELECT * FROM ManufactureStore WHERE CurrentCount<>0 AND DecorAssignmentID IN (SELECT DecorAssignmentID FROM DecorAssignments WHERE DecorOrderID IN (SELECT OrderID FROM infiniu2_marketingorders.dbo.PackageDetails WHERE PackageID = " + PackageID + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                ManufactureStoreDT.Clear();
                DA.Fill(ManufactureStoreDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM ReadyStore";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                ReadyStoreDT.Clear();
                DA.Fill(ReadyStoreDT);
            }

            //НАЙТИ ID КЛИЕНТА
            int RecipientStoreAllocID = 10;
            int SellerStoreAllocID = 3;
            if (CurrentFactoryID == 2)
            {
                SellerStoreAllocID = 4;
                RecipientStoreAllocID = 11;
            }
            int MovementInvoiceID = SaveMovementInvoices(SellerStoreAllocID, RecipientStoreAllocID, 0, iUserID, Security.CurrentUserShortName, iUserID, CurrentClientID, 0, string.Empty, "Экспедиция");
            DateTime CreateDateTime = Security.GetCurrentDate();
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(PackageDetailsDT))
            {
                DT = DV.ToTable(true, "OrderID");
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                int OrderID = 0;
                if (DT.Rows[i]["OrderID"] != DBNull.Value)
                    OrderID = Convert.ToInt32(DT.Rows[i]["OrderID"]);
                DataRow[] Rows = PackageDetailsDT.Select("OrderID = " + OrderID);
                decimal Count = 0;
                decimal ConstCount = 0;
                decimal InvoiceCount = 0;
                foreach (DataRow item in Rows)
                    ConstCount += Convert.ToDecimal(item["Count"]);
                for (int j = 0; j < DecorAssignmentsDT.Rows.Count; j++)
                {
                    Count = ConstCount;
                    int StoreItemID = Convert.ToInt32(DecorAssignmentsDT.Rows[j]["TechStoreID2"]);
                    DataRow[] Rows1 = ManufactureStoreDT.Select("StoreItemID = " + StoreItemID);
                    foreach (var item in Rows1)
                    {
                        int ManufactureStoreID = 0;
                        if (item["ManufactureStoreID"] != DBNull.Value)
                            ManufactureStoreID = Convert.ToInt32(item["ManufactureStoreID"]);
                        InvoiceCount = MoveToReadyStore(MovementInvoiceID, CreateDateTime, ManufactureStoreID, ref Count);
                        AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, ManufactureStoreID, InvoiceCount);
                        if (Count <= 0)
                            break;
                    }
                }
            }
            ManufactureStoreDA.Update(ManufactureStoreDT);
            ReadyStoreDA.Update(ReadyStoreDT);
            FillReadyStoreMovementInvoiceDetails(MovementInvoiceID);
            MovementInvoiceDetailsDA.Update(MovementInvoiceDetailsDT);

            sw.Stop();
            double t = 0;
            t = sw.Elapsed.TotalMilliseconds;
        }

        public void AddMovementInvoiceDetail(int MovementInvoiceID, DateTime CreateDateTime, int StoreIDFrom, decimal Count)
        {
            DataRow[] Rows = MovementInvoiceDetailsDT.Select("StoreIDFrom = " + StoreIDFrom);
            if (Rows.Count() > 0)
            {
                Rows[0]["Count"] = Convert.ToDecimal(Rows[0]["Count"]) + Count;
            }
            else
            {
                DataRow NewRow = MovementInvoiceDetailsDT.NewRow();
                if (MovementInvoiceDetailsDT.Columns.Contains("CreateDateTime"))
                    NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["MovementInvoiceID"] = MovementInvoiceID;
                NewRow["StoreIDFrom"] = StoreIDFrom;
                NewRow["StoreIDTo"] = 0;
                NewRow["Count"] = Count;
                MovementInvoiceDetailsDT.Rows.Add(NewRow);
            }

        }

        public void FillReadyStoreMovementInvoiceDetails(int MovementInvoiceID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ReadyStore" +
                   " WHERE MovementInvoiceID = " + MovementInvoiceID,
                   ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    int j = 0;

                    for (int i = 0; i < MovementInvoiceDetailsDT.Rows.Count; i++)
                    {
                        if (MovementInvoiceDetailsDT.Rows[i].RowState == DataRowState.Deleted)
                        {
                            continue;
                        }

                        int StoreID = Convert.ToInt32(MovementInvoiceDetailsDT.Rows[i]["StoreIDTo"]);

                        if (StoreID == 0)
                        {
                            MovementInvoiceDetailsDT.Rows[i]["StoreIDTo"] = DT.Rows[j++]["ReadyStoreID"];
                        }
                        else
                        {
                            DataRow[] Rows = DT.Select("ReadyStoreID = " + StoreID);
                            if (Rows.Count() > 0)
                            {
                                j++;
                            }
                            else
                                MovementInvoiceDetailsDT.Rows[i].Delete();
                        }
                    }
                }
            }
        }

        public int SaveMovementInvoices(
            int SellerStoreAllocID,
            int RecipientStoreAllocID, int RecipientSectorID,
            int PersonID, string PersonName, int StoreKeeperID,
            int ClientID, int SellerID,
            string ClientName, string Notes)
        {
            int LastMovementInvoiceID = 0;
            DateTime CurrentDate = Security.GetCurrentDate();

            DataRow NewRow = MovementInvoicesDT.NewRow();

            NewRow["DateTime"] = CurrentDate;
            NewRow["SellerStoreAllocID"] = SellerStoreAllocID;
            NewRow["RecipientStoreAllocID"] = RecipientStoreAllocID;
            NewRow["RecipientSectorID"] = RecipientSectorID;
            NewRow["PersonID"] = PersonID;
            NewRow["PersonName"] = PersonName;
            NewRow["StoreKeeperID"] = StoreKeeperID;
            NewRow["ClientName"] = ClientName;
            NewRow["ClientID"] = ClientID;
            NewRow["SellerID"] = SellerID;
            NewRow["Notes"] = Notes;

            MovementInvoicesDT.Rows.Add(NewRow);

            MovementInvoicesDA.Update(MovementInvoicesDT);
            MovementInvoicesDT.Clear();
            MovementInvoicesDA.Fill(MovementInvoicesDT);
            if (MovementInvoicesDT.Rows.Count > 0)
                LastMovementInvoiceID = Convert.ToInt32(MovementInvoicesDT.Rows[MovementInvoicesDT.Rows.Count - 1]["MovementInvoiceID"]);

            return LastMovementInvoiceID;
        }

        public decimal MoveToReadyStore(int MovementInvoiceID, DateTime CreateDateTime, int ManufactureStoreID, ref decimal Count)
        {
            DataRow[] Rows = ManufactureStoreDT.Select("ManufactureStoreID = " + ManufactureStoreID);
            decimal CurrentCount = 0;
            decimal InvoiceCount = 0;
            if (Rows.Count() > 0)
            {
                CurrentCount = Convert.ToDecimal(Rows[0]["CurrentCount"]);
                InvoiceCount = CurrentCount - Count;

                if (CurrentCount - Count < 0)
                {
                    Rows[0]["CurrentCount"] = 0;
                    InvoiceCount = CurrentCount;
                }
                else
                {
                    Rows[0]["CurrentCount"] = CurrentCount - Count;
                    InvoiceCount = Count;
                }

                DataRow NewRow = ReadyStoreDT.NewRow();
                if (ReadyStoreDT.Columns.Contains("CreateDateTime"))
                    NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["StoreItemID"] = Rows[0]["StoreItemID"];
                NewRow["Length"] = Rows[0]["Length"];
                NewRow["Width"] = Rows[0]["Width"];
                NewRow["Height"] = Rows[0]["Height"];
                NewRow["Thickness"] = Rows[0]["Thickness"];
                NewRow["Diameter"] = Rows[0]["Diameter"];
                NewRow["Admission"] = Rows[0]["Admission"];
                NewRow["Capacity"] = Rows[0]["Capacity"];
                NewRow["Weight"] = Rows[0]["Weight"];
                NewRow["ColorID"] = Rows[0]["ColorID"];
                NewRow["CoverID"] = Rows[0]["CoverID"];
                NewRow["PatinaID"] = Rows[0]["PatinaID"];
                NewRow["InvoiceCount"] = InvoiceCount;
                NewRow["CurrentCount"] = InvoiceCount;
                NewRow["MovementInvoiceID"] = MovementInvoiceID;
                NewRow["FactoryID"] = Rows[0]["FactoryID"];
                NewRow["Notes"] = Rows[0]["Notes"];
                NewRow["DecorAssignmentID"] = Rows[0]["DecorAssignmentID"];
                ReadyStoreDT.Rows.Add(NewRow);
                Count = Count - CurrentCount;
            }
            return InvoiceCount;
        }

    }



    public enum PalleteTypeAction
    {
        PackingPallete,
        DispatchPallete
    }


    public class PalleteCheckLabel
    {
        DataTable DecorPackContentDT = null;

        public BindingSource DecorPackContentBindingSource = null;

        public struct Labelinfo
        {
            public string PalleteName;
            public string UserName;
            public int WeekNumber;
            public int MegaBatchID;
            public string DocDateTime;
            public string DispatchDate;
            public string BarcodeNumber;
            public string Notes;
            public DataTable OrderData;
            public int NumberOfChange;
            public int ProductType;
            public int FactoryType;
            public string GroupType;
        }

        public Labelinfo LabelInfo;

        public PalleteCheckLabel()
        {
            Initialize();
        }


        private void Create()
        {
            DecorPackContentDT = new DataTable();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 Product.ProductName, Decor.Name, Color.ColorName, Height, Length, Width, PackageDetails.Count FROM PackageDetails" +
            " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID AND PackageDetails.PackageID IN (SELECT PackageID FROM Packages)" +
            " INNER JOIN infiniu2_catalog.dbo.DecorProducts AS Product ON DecorOrders.ProductID=Product.ProductID" +
            " INNER JOIN infiniu2_catalog.dbo.Decor AS Decor ON DecorOrders.DecorID=Decor.DecorID" +
            " INNER JOIN infiniu2_catalog.dbo.Colors AS Color ON DecorOrders.ColorID=Color.ColorID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorPackContentDT);
            }
        }

        private void Binding()
        {
            DecorPackContentBindingSource = new BindingSource()
            {
                DataSource = DecorPackContentDT
            };
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public void FillDecorPackContent(string Barcode)
        {
            int PalleteID = Convert.ToInt32(Barcode.Substring(3, 9));
            string BarType = Barcode.Substring(0, 3);
            string ConnectionString = string.Empty;

            DecorPackContentDT.Clear();

            if (BarType == "009" || BarType == "010")
            {
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            }
            if (BarType == "011" || BarType == "012")
            {
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Product.ProductName, Decor.Name, Color.ColorName, Height, Length, Width, PackageDetails.Count FROM PackageDetails" +
            " INNER JOIN DecorOrders ON PackageDetails.OrderID=DecorOrders.DecorOrderID AND PackageDetails.PackageID IN (SELECT PackageID FROM Packages WHERE PalleteID = " + PalleteID + ")" +
            " INNER JOIN infiniu2_catalog.dbo.DecorProducts AS Product ON DecorOrders.ProductID=Product.ProductID" +
            " INNER JOIN infiniu2_catalog.dbo.Decor AS Decor ON DecorOrders.DecorID=Decor.DecorID" +
            " INNER JOIN infiniu2_catalog.dbo.Colors AS Color ON DecorOrders.ColorID=Color.ColorID", ConnectionString))
            {
                DA.Fill(DecorPackContentDT);
            }
        }

        public object GetMegaBatchDate(int MegaBatchID)
        {
            object obj = null;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT CreationDateTime FROM MegaBatch WHERE MegaBatchID = " + MegaBatchID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        obj = DT.Rows[0]["CreationDateTime"];
                    }
                }
            }
            return obj;
        }

        public int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        public void GetPalleteParameters(string Barcode)
        {
            DataTable DT = new DataTable();
            int PalleteID = Convert.ToInt32(Barcode.Substring(3, 9));
            string BarType = Barcode.Substring(0, 3);
            string ConnectionString = string.Empty;

            if (BarType == "009" || BarType == "010")
            {
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
                LabelInfo.GroupType = "ЗОВ";
            }
            if (BarType == "011" || BarType == "012")
            {
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
                LabelInfo.GroupType = "Маркетинг";
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Pallets WHERE PalleteID = " + PalleteID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DT);
                if (DT.Rows.Count > 0)
                {
                    if (DT.Rows[0]["MegaBatchID"] != DBNull.Value)
                        LabelInfo.MegaBatchID = Convert.ToInt32(DT.Rows[0]["MegaBatchID"]);
                    if (DT.Rows[0]["Name"] != DBNull.Value)
                        LabelInfo.PalleteName = DT.Rows[0]["Name"].ToString();
                    if (DT.Rows[0]["UserName"] != DBNull.Value)
                        LabelInfo.UserName = DT.Rows[0]["UserName"].ToString();
                    if (DT.Rows[0]["FactoryID"] != DBNull.Value)
                        LabelInfo.FactoryType = Convert.ToInt32(DT.Rows[0]["FactoryID"]);
                    if (DT.Rows[0]["NumberOfChange"] != DBNull.Value)
                        LabelInfo.NumberOfChange = Convert.ToInt32(DT.Rows[0]["NumberOfChange"]);
                    object obj = GetMegaBatchDate(LabelInfo.MegaBatchID);
                    if (obj != null)
                        LabelInfo.WeekNumber = GetWeekNumber(Convert.ToDateTime(obj));
                    if (DT.Rows[0]["ProductionDateTime"] != DBNull.Value)
                        LabelInfo.DocDateTime = Convert.ToDateTime(obj).ToString("dd.MM.yyyy HH:mm");
                }
            }
        }


        public bool CheckBarcode(string Barcode)
        {
            int PalleteID = Convert.ToInt32(Barcode.Substring(3, 9));
            string BarType = Barcode.Substring(0, 3);

            DecorPackContentDT.Clear();

            if (BarType == "009" || BarType == "010")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT PackageID FROM Packages WHERE PalleteID = " + PalleteID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (BarType == "011" || BarType == "012")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT PackageID FROM Packages WHERE PalleteID = " + PalleteID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public void SetPacked(string Barcode)
        {
            int PalleteID = Convert.ToInt32(Barcode.Substring(3, 9));
            string BarType = Barcode.Substring(0, 3);
            string ConnectionString = string.Empty;

            if (BarType == "009" || BarType == "010")
            {
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            }
            if (BarType == "011" || BarType == "012")
            {
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpeditionDateTime FROM Packages" +
                " WHERE PalleteID = " + PalleteID, ConnectionString))
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
                                //нельзя принять на упаковку, если уже огружено
                                if (DT.Rows[i]["PackageStatusID"] != DBNull.Value
                                    && Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) == 3)
                                    continue;
                                DT.Rows[i]["PackageStatusID"] = 2;
                                if (DT.Rows[i]["PackingDateTime"] != DBNull.Value)
                                    DT.Rows[i]["PackingDateTime"] = Security.GetCurrentDate();
                                if (DT.Rows[i]["StorageDateTime"] != DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = Security.GetCurrentDate();
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID FROM Packages" +
                " WHERE PalleteID = " + PalleteID + ")", ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            CheckOrdersStatus.SetMegaOrderStatus(Convert.ToInt32(DT.Rows[0]["MegaOrderID"]));
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetDispatched(string Barcode)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            int PalleteID = Convert.ToInt32(Barcode.Substring(3, 9));
            string BarType = Barcode.Substring(0, 3);
            string ConnectionString = string.Empty;

            if (BarType == "009" || BarType == "010")
            {
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            }
            if (BarType == "011" || BarType == "012")
            {
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime FROM Packages" +
                " WHERE PalleteID = " + PalleteID, ConnectionString))
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
                                DT.Rows[i]["PackageStatusID"] = 3;
                                if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PackingDateTime"] = CurrentDate;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;
                                if (DT.Rows[i]["DispatchDateTime"] == DBNull.Value)
                                    DT.Rows[i]["DispatchDateTime"] = CurrentDate;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID FROM Packages" +
                " WHERE PalleteID = " + PalleteID + ")", ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            CheckOrdersStatus.SetMegaOrderStatus(Convert.ToInt32(DT.Rows[0]["MegaOrderID"]));
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void Clear()
        {
            DecorPackContentDT.Clear();

            LabelInfo.PalleteName = string.Empty;
            LabelInfo.MegaBatchID = 0;
            LabelInfo.WeekNumber = 0;
            LabelInfo.DocDateTime = string.Empty;
            LabelInfo.DispatchDate = string.Empty;
            LabelInfo.BarcodeNumber = string.Empty;
            LabelInfo.Notes = string.Empty;
            LabelInfo.OrderData = null;
            LabelInfo.NumberOfChange = 0;
            LabelInfo.ProductType = 0;
            LabelInfo.FactoryType = 0;
            LabelInfo.GroupType = string.Empty;
        }

        private int GetMegaOrderID(int MainOrderID)
        {
            int MegaOrderID = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["MegaOrderID"] != DBNull.Value)
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                    return MegaOrderID;
                }
            }
        }
    }



    //public class MarketingOrdersEvents
    //{
    //    public MarketingOrdersEvents()
    //    {

    //    }

    //    public static void SaveEvents(int MarketingOrdersEventID)
    //    {
    //        using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MarketingOrdersEventsJournal",
    //            ConnectionStrings.MarketingReferenceConnectionString))
    //        {
    //            using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
    //            {
    //                using (DataTable EventsDataTable = new DataTable())
    //                {
    //                    DA.Fill(EventsDataTable);
    //                    DataRow NewRow = EventsDataTable.NewRow();
    //                    NewRow["UserID"] = iUserID;
    //                    NewRow["MarketingOrdersEventID"] = MarketingOrdersEventID;
    //                    NewRow["EventDateTime"] = Security.GetCurrentDate();
    //                    EventsDataTable.Rows.Add(NewRow);
    //                    DA.Update(EventsDataTable);
    //                }
    //            }
    //        }
    //    }
    //}


    public class ScanEvents
    {
        static TimeSpan DeltaTime;

        public ScanEvents()
        {

        }

        public static void SetEventsDataTable(DataTable DT)
        {
            DeltaTime = Security.GetCurrentDate() - DateTime.Now;

            DT.Columns.Add(new DataColumn("UserID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("LoginJournalID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("GroupType", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("ModuleID", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Event", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("EventDateTime", Type.GetType("System.DateTime")));
            DT.Columns.Add(new DataColumn("OrderStatusInfo", Type.GetType("System.String")));
        }

        public static void AddEvent(DataTable EventsDataTable, string Event, int GroupType, int iUserID)
        {
            DataRow NewRow = EventsDataTable.NewRow();
            NewRow["UserID"] = iUserID;
            NewRow["ModuleID"] = Security.CurrentModuleID;
            NewRow["LoginJournalID"] = Security.CurrentLoginJournalID;
            NewRow["GroupType"] = GroupType;
            NewRow["Event"] = Event;
            NewRow["EventDateTime"] = DateTime.Now + DeltaTime;
            EventsDataTable.Rows.Add(NewRow);
        }

        public static void SaveEvents(DataTable EventsDataTable)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ScanEventsJournal",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in EventsDataTable.Rows)
                        {
                            DT.ImportRow(Row);
                        }

                        DA.Update(DT);
                    }
                }
            }
        }
    }

}
