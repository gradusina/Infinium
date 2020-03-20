using ComponentFactory.Krypton.Toolkit;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.Packages.Trays
{
    public class MarketingFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        public DataTable FrontsOrdersDataTable = null;
        public DataTable FrontsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;

        public BindingSource FrontsOrdersBindingSource = null;
        private BindingSource FrontsBindingSource = null;
        private BindingSource PatinaBindingSource = null;
        private BindingSource InsetTypesBindingSource = null;
        private BindingSource FrameColorsBindingSource = null;
        private BindingSource InsetColorsBindingSource = null;
        private BindingSource TechnoFrameColorsBindingSource = null;
        private BindingSource TechnoInsetTypesBindingSource = null;
        private BindingSource TechnoInsetColorsBindingSource = null;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public MarketingFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;

            Initialize();
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            FrontsOrdersBindingSource = new BindingSource();
            FrontsBindingSource = new BindingSource();
            FrameColorsBindingSource = new BindingSource();
            PatinaBindingSource = new BindingSource();
            InsetTypesBindingSource = new BindingSource();
            InsetColorsBindingSource = new BindingSource();
            TechnoFrameColorsBindingSource = new BindingSource();
            TechnoInsetTypesBindingSource = new BindingSource();
            TechnoInsetColorsBindingSource = new BindingSource();
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            FrontsOrdersDataTable.Columns.Add(new DataColumn("PackNumber", Type.GetType("System.Int32")));
        }

        private void Binding()
        {
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            FrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;

            FrontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns()
        {
            if (FrontsColumn != null)
                return;

            //создание столбцов
            FrontsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrontsColumn",
                HeaderText = "Фасад",
                DataPropertyName = "FrontID",
                DataSource = FrontsBindingSource,
                ValueMember = "FrontID",
                DisplayMember = "FrontName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            FrameColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FrameColorsColumn",
                HeaderText = "Цвет\r\nпрофиля",
                DataPropertyName = "ColorID",
                DataSource = FrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = InsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = InsetColorsBindingSource,
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
                DataSource = TechnoFrameColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypesColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = TechnoInsetTypesBindingSource,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorsColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = TechnoInsetColorsBindingSource,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
        }

        private void GridSetting()
        {
            //добавление столбцов
            FrontsOrdersDataGrid.Columns.Add(FrontsColumn);
            FrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
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
            FrontsOrdersDataGrid.Columns["PackNumber"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("TotalDiscount"))
                FrontsOrdersDataGrid.Columns["TotalDiscount"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("OriginalCost"))
                FrontsOrdersDataGrid.Columns["OriginalCost"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CurrencyCost"))
                FrontsOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CurrencyTypeID"))
                FrontsOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;

            FrontsOrdersDataGrid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
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
            //MainOrdersFrontsOrdersDataGrid.Columns["PackNumber"].DisplayIndex = 10;


            FrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            foreach (DataGridViewColumn Column in FrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            //названия столбцов
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";

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
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 75;
            FrontsOrdersDataGrid.CellFormatting += FrontsOrdersDataGrid_CellFormatting;
        }

        void FrontsOrdersDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (grid.Columns.Contains("PatinaColumn") && (e.ColumnIndex == grid.Columns["PatinaColumn"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int PatinaID = -1;
                string DisplayName = string.Empty;
                if (grid.Rows[e.RowIndex].Cells["PatinaID"].Value != DBNull.Value)
                {
                    PatinaID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PatinaID"].Value);
                    DisplayName = PatinaDisplayName(PatinaID);
                }
                cell.ToolTipText = DisplayName;
            }
        }

        public string PatinaDisplayName(int PatinaID)
        {
            DataRow[] rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            GridSetting();
        }

        private int[] GetPackageIDs(int MainOrderID)
        {
            int[] PackageIDs = { 0 };

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageIDs = new int[DT.Rows.Count];
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            PackageIDs[i] = Convert.ToInt32(DT.Rows[i]["PackageID"]);
                        }
                    }
                }
            }

            return PackageIDs;
        }

        public bool Filter(int PackageID)
        {
            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            OriginalFrontsOrdersDataTable = FrontsOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE FrontsOrdersID IN " +
                "(SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID = " + PackageID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["FrontsOrdersID"] = ORow[0]["FrontsOrdersID"];
                            NewRow["FrontID"] = ORow[0]["FrontID"];
                            NewRow["PatinaID"] = ORow[0]["PatinaID"];
                            NewRow["InsetTypeID"] = ORow[0]["InsetTypeID"];
                            NewRow["ColorID"] = ORow[0]["ColorID"];
                            NewRow["InsetColorID"] = ORow[0]["InsetColorID"];
                            NewRow["TechnoColorID"] = ORow[0]["TechnoColorID"];
                            NewRow["TechnoInsetTypeID"] = ORow[0]["TechnoInsetTypeID"];
                            NewRow["TechnoInsetColorID"] = ORow[0]["TechnoInsetColorID"];
                            NewRow["Height"] = ORow[0]["Height"];
                            NewRow["Width"] = ORow[0]["Width"];
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();
            FrontsOrdersBindingSource.MoveFirst();
            return FrontsOrdersDataTable.Rows.Count > 0;
        }
    }







    public class MarketingDecorOrders
    {
        private PercentageDataGrid MainOrdersDecorOrdersDataGrid = null;

        private DataTable ColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;

        public DataTable DecorOrdersDataTable = null;

        public BindingSource DecorOrdersBindingSource = null;

        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;

        //конструктор
        public MarketingDecorOrders(ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid)
        {
            MainOrdersDecorOrdersDataGrid = tMainOrdersDecorOrdersDataGrid;
            Initialize();
        }

        private void Create()
        {
            DecorOrdersDataTable = new DataTable();

            ColorsDataTable = new DataTable();
            ProductsDataTable = new DataTable();
            DecorDataTable = new DataTable();

            DecorOrdersBindingSource = new BindingSource();
        }

        private void GetColorsDT()
        {
            ColorsDataTable = new DataTable();
            ColorsDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDataTable.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    {
                        DataRow NewRow = ColorsDataTable.NewRow();
                        NewRow["ColorID"] = -1;
                        NewRow["ColorName"] = "-";
                        ColorsDataTable.Rows.Add(NewRow);
                    }
                    {
                        DataRow NewRow = ColorsDataTable.NewRow();
                        NewRow["ColorID"] = 0;
                        NewRow["ColorName"] = "на выбор";
                        ColorsDataTable.Rows.Add(NewRow);
                    }
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDataTable.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDataTable.Rows.Add(NewRow);
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
                        DataRow[] rows = ColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = ColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            ColorsDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
        }

        private void Fill()
        {
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersDataTable.Columns.Add(new DataColumn("PackNumber", Type.GetType("System.Int32")));

            string SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1  ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            GetColorsDT();
            PatinaDataTable = new DataTable();
            SelectCommand = @"SELECT * FROM Patina ORDER BY PatinaName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
        }

        private void Binding()
        {
            DecorOrdersBindingSource.DataSource = DecorOrdersDataTable;
            MainOrdersDecorOrdersDataGrid.DataSource = DecorOrdersBindingSource;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();

            GridSettings();
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

                    DataSource = new DataView(ProductsDataTable),
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

                    DataSource = new DataView(ColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ColorsColumn;
            }
        }

        private DataGridViewComboBoxColumn PatinaColumn
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

        private void GridSettings()
        {
            MainOrdersDecorOrdersDataGrid.ReadOnly = true;

            MainOrdersDecorOrdersDataGrid.Columns.Add(ProductColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(ItemColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(ColorColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(PatinaColumn);
            //MainOrdersDecorOrdersDataGrid.RowPostPaint += new DataGridViewRowPostPaintEventHandler(MainOrdersDecorOrdersDataGrid_RowPostPaint);
            //убирание лишних столбцов
            if (MainOrdersDecorOrdersDataGrid.Columns.Contains("CreateDateTime"))
            {
                MainOrdersDecorOrdersDataGrid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                MainOrdersDecorOrdersDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                MainOrdersDecorOrdersDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                MainOrdersDecorOrdersDataGrid.Columns["CreateDateTime"].Width = 100;
            }
            if (MainOrdersDecorOrdersDataGrid.Columns.Contains("CreateUserID"))
                MainOrdersDecorOrdersDataGrid.Columns["CreateUserID"].Visible = false;
            if (MainOrdersDecorOrdersDataGrid.Columns.Contains("CreateUserTypeID"))
                MainOrdersDecorOrdersDataGrid.Columns["CreateUserTypeID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ProductID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["DecorID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ColorID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Price"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["DecorConfigID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Cost"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Weight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["OriginalCost"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["TotalDiscount"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["DiscountVolume"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
            if (MainOrdersDecorOrdersDataGrid.Columns.Contains("PriceWithTransport"))
                MainOrdersDecorOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            if (MainOrdersDecorOrdersDataGrid.Columns.Contains("CostWithTransport"))
                MainOrdersDecorOrdersDataGrid.Columns["CostWithTransport"].Visible = false;

            //русские названия полей

            MainOrdersDecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            MainOrdersDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            MainOrdersDecorOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            MainOrdersDecorOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            MainOrdersDecorOrdersDataGrid.Columns["Length"].HeaderText = "Длина";
            MainOrdersDecorOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            MainOrdersDecorOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";

            int DisplayIndex = 0;
            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Height"].Width = 85;
            MainOrdersDecorOrdersDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Length"].Width = 85;
            MainOrdersDecorOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Width"].Width = 85;
            MainOrdersDecorOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Count"].Width = 85;
            MainOrdersDecorOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["IsSample"].Width = 85;

            foreach (DataGridViewColumn Column in MainOrdersDecorOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private int[] GetPackageIDs(int MainOrderID)
        {
            int[] PackageIDs = { 0 };

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageIDs = new int[DT.Rows.Count];
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            PackageIDs[i] = Convert.ToInt32(DT.Rows[i]["PackageID"]);
                        }
                    }
                }
            }

            return PackageIDs;
        }

        public bool Filter(int PackageID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return DecorOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = DecorOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE DecorOrderID IN " +
                "(SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID = " + PackageID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["DecorOrderID"] = ORow[0]["DecorOrderID"];
                            NewRow["ProductID"] = ORow[0]["ProductID"];
                            NewRow["DecorID"] = ORow[0]["DecorID"];

                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];

                            NewRow["Height"] = ORow[0]["Height"];
                            NewRow["Length"] = ORow[0]["Length"];
                            NewRow["Width"] = ORow[0]["Width"];
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();
            DecorOrdersBindingSource.MoveFirst();
            return DecorOrdersDataTable.Rows.Count > 0;
        }


        //public void SetColor(int packNumber)
        //{
        //    for (int i = 0; i < MainOrdersDecorOrdersDataGrid.Rows.Count; i++)
        //    {
        //        if (MainOrdersDecorOrdersDataGrid.Rows[i].Cells["PackNumber"].Value != DBNull.Value)
        //        {
        //            int DecorOrderPackNumber = Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[i].Cells["PackNumber"].Value);
        //            PackNumber = packNumber;

        //            if (DecorOrderPackNumber == PackNumber)
        //            {
        //                MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(31, 158, 0);
        //                MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.White;
        //            }
        //            else
        //            {
        //                MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Security.GridsBackColor;
        //                MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
        //            }
        //        }
        //    }
        //}

        //public void MoveToDecorOrder(int PackNumber)
        //{
        //    DataRow[] Rows = DecorOrdersDataTable.Select("PackNumber = " + PackNumber);
        //    if (Rows.Count() < 1)
        //        return;

        //    int DecorOrderID = Convert.ToInt32(Rows[0]["DecorOrderID"]);
        //    DecorOrdersDataTable.DefaultView.Sort = "PackNumber, DecorOrderID";
        //    using (DataView DV = new DataView(DecorOrdersDataTable))
        //    {
        //        DV.Sort = "PackNumber, DecorOrderID";
        //        object[] obj = new object[] { PackNumber, DecorOrderID };

        //        MainOrdersDecorOrdersDataGrid.FirstDisplayedScrollingRowIndex = DV.Find(obj);
        //    }
        //}

        //private void MainOrdersDecorOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    if (MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value == DBNull.Value)
        //        return;

        //    if (Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) == PackNumber)
        //    {
        //        // Calculate the bounds of the row 
        //        int rowHeaderWidth = MainOrdersDecorOrdersDataGrid.RowHeadersVisible ?
        //                             MainOrdersDecorOrdersDataGrid.RowHeadersWidth : 0;
        //        Rectangle rowBounds = new Rectangle(
        //            rowHeaderWidth,
        //            e.RowBounds.Top,
        //            MainOrdersDecorOrdersDataGrid.Columns.GetColumnsWidth(
        //                    DataGridViewElementStates.Visible) -
        //                    MainOrdersDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
        //           e.RowBounds.Height);

        //        MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
        //                                             Color.FromArgb(31, 158, 0);
        //        MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
        //                                             Color.White;
        //    }
        //    if (Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) != PackNumber)
        //    {
        //        // Calculate the bounds of the row 
        //        int rowHeaderWidth = MainOrdersDecorOrdersDataGrid.RowHeadersVisible ?
        //                             MainOrdersDecorOrdersDataGrid.RowHeadersWidth : 0;
        //        Rectangle rowBounds = new Rectangle(
        //            rowHeaderWidth,
        //            e.RowBounds.Top,
        //            MainOrdersDecorOrdersDataGrid.Columns.GetColumnsWidth(
        //                    DataGridViewElementStates.Visible) -
        //                    MainOrdersDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
        //           e.RowBounds.Height);

        //        MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
        //                                             Security.GridsBackColor;
        //        MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
        //                                             Color.Black;
        //    }
        //}
    }








    public class MarketingTraysManager
    {
        public MarketingFrontsOrders PackedMainOrdersFrontsOrders = null;
        public MarketingDecorOrders PackedMainOrdersDecorOrders = null;

        public PercentageDataGrid PackagesDataGrid = null;
        public PercentageDataGrid TraysDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        private DataTable PackagesDataTable = null;
        private DataTable PackageStatusesDataTable = null;
        private DataTable FactoryTypesDataTable = null;
        private DataTable TraysDataTable = null;
        private DataTable TrayStatusesDataTable = null;

        public BindingSource PackagesBindingSource = null;
        public BindingSource PackageStatusesBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource TraysBindingSource = null;
        public BindingSource TrayStatusesBindingSource = null;

        private DataGridViewComboBoxColumn PackageStatusesColumn = null;
        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn TrayStatusesColumn = null;

        public MarketingTraysManager(ref PercentageDataGrid tPackagesDataGrid,
            ref PercentageDataGrid tTraysDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl)
        {
            PackagesDataGrid = tPackagesDataGrid;
            TraysDataGrid = tTraysDataGrid;
            OrdersTabControl = tOrdersTabControl;

            PackedMainOrdersFrontsOrders = new MarketingFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);

            PackedMainOrdersDecorOrders = new MarketingDecorOrders(ref tMainOrdersDecorOrdersDataGrid);
        }

        #region Properties

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
        #endregion

        #region Initialize

        private void Create()
        {
            PackagesDataTable = new DataTable();
            FactoryTypesDataTable = new DataTable();
            PackageStatusesDataTable = new DataTable();
            TraysDataTable = new DataTable();
            TrayStatusesDataTable = new DataTable();

            FactoryTypesBindingSource = new BindingSource();
            PackagesBindingSource = new BindingSource();
            PackageStatusesBindingSource = new BindingSource();
            TraysBindingSource = new BindingSource();
            TrayStatusesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TrayStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TrayStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Trays", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TraysDataTable);
            }
        }

        private void Binding()
        {
            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;
            PackagesBindingSource.DataSource = PackagesDataTable;
            PackageStatusesBindingSource.DataSource = PackageStatusesDataTable;
            TraysBindingSource.DataSource = TraysDataTable;

            PackagesDataGrid.DataSource = PackagesBindingSource;
            TraysDataGrid.DataSource = TraysBindingSource;
            TrayStatusesBindingSource.DataSource = TrayStatusesDataTable;
        }

        private void CreateColumns()
        {
            TrayStatusesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TrayStatusColumn",
                HeaderText = "Статус",
                DataPropertyName = "TrayStatusID",
                DataSource = TrayStatusesBindingSource,
                ValueMember = "TrayStatusID",
                DisplayMember = "TrayStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
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
            FactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryTypeColumn",
                HeaderText = "Тип\r\nпроизводства",
                DataPropertyName = "FactoryID",
                DataSource = FactoryTypesBindingSource,
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            PackagesDataGrid.Columns.Add(PackageStatusesColumn);
            TraysDataGrid.Columns.Add(TrayStatusesColumn);
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
            if (PackagesDataGrid.Columns.Contains("TrayID"))
                PackagesDataGrid.Columns["TrayID"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("FactoryID"))
                PackagesDataGrid.Columns["FactoryID"].Visible = false;

            PackagesDataGrid.Columns["PackingDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            PackagesDataGrid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            PackagesDataGrid.Columns["PackageID"].HeaderText = "ID";
            PackagesDataGrid.Columns["FactoryName"].HeaderText = "Участок";
            PackagesDataGrid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            PackagesDataGrid.Columns["Notes"].HeaderText = "Примечание";
            PackagesDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            PackagesDataGrid.Columns["PackingDateTime"].HeaderText = "Дата\r\nупаковки";
            PackagesDataGrid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            PackagesDataGrid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";

            PackagesDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackNumber"].Width = 70;
            PackagesDataGrid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageStatusesColumn"].Width = 140;
            PackagesDataGrid.Columns["PackingDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackingDateTime"].Width = 150;
            PackagesDataGrid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["StorageDateTime"].Width = 150;
            PackagesDataGrid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["ExpeditionDateTime"].Width = 150;
            PackagesDataGrid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageID"].Width = 100;
            PackagesDataGrid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["FactoryName"].Width = 100;
            PackagesDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PackagesDataGrid.Columns["ClientName"].MinimumWidth = 290;
            PackagesDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["OrderNumber"].Width = 70;
            PackagesDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PackagesDataGrid.Columns["Notes"].MinimumWidth = 100;

            PackagesDataGrid.AutoGenerateColumns = false;

            PackagesDataGrid.Columns["ClientName"].DisplayIndex = 0;
            PackagesDataGrid.Columns["OrderNumber"].DisplayIndex = 1;
            PackagesDataGrid.Columns["Notes"].DisplayIndex = 2;
            PackagesDataGrid.Columns["PackageStatusesColumn"].DisplayIndex = 3;
            PackagesDataGrid.Columns["FactoryName"].DisplayIndex = 4;
            PackagesDataGrid.Columns["PackingDateTime"].DisplayIndex = 5;
            PackagesDataGrid.Columns["StorageDateTime"].DisplayIndex = 6;
            PackagesDataGrid.Columns["ExpeditionDateTime"].DisplayIndex = 7;
            PackagesDataGrid.Columns["PackNumber"].DisplayIndex = 8;
            PackagesDataGrid.Columns["PackageID"].DisplayIndex = 9;
        }

        private void TraysGridSetting()
        {
            foreach (DataGridViewColumn Column in TraysDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            TraysDataGrid.Columns["TrayStatusID"].Visible = false;
            TraysDataGrid.Columns["ExpeditionDateTime"].Visible = false;

            TraysDataGrid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            TraysDataGrid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            TraysDataGrid.Columns["DispatchDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            TraysDataGrid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            TraysDataGrid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";
            TraysDataGrid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            TraysDataGrid.Columns["TrayID"].HeaderText = "№\r\nпод.";

            TraysDataGrid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TraysDataGrid.Columns["TrayID"].Width = 50;
            TraysDataGrid.Columns["TrayStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TraysDataGrid.Columns["TrayStatusColumn"].Width = 140;
            TraysDataGrid.Columns["DispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TraysDataGrid.Columns["DispatchDateTime"].Width = 130;
            TraysDataGrid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TraysDataGrid.Columns["ExpeditionDateTime"].Width = 130;
            TraysDataGrid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TraysDataGrid.Columns["StorageDateTime"].Width = 130;

            TraysDataGrid.AutoGenerateColumns = false;

            TraysDataGrid.Columns["TrayID"].DisplayIndex = 0;
            TraysDataGrid.Columns["TrayStatusColumn"].DisplayIndex = 1;
            TraysDataGrid.Columns["StorageDateTime"].DisplayIndex = 2;
            TraysDataGrid.Columns["ExpeditionDateTime"].DisplayIndex = 3;
            TraysDataGrid.Columns["DispatchDateTime"].DisplayIndex = 4;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            PackagesGridSetting();
            TraysGridSetting();
        }
        #endregion


        #region Filter

        public bool FilterTrays(bool NotFormed, bool Formed, bool Dispatched)
        {
            string FilterString = "TrayStatusID = -1";

            if (NotFormed)
                FilterString = "TrayStatusID = 0";

            if (Formed)
                if (FilterString.Length > 0)
                    FilterString += " OR TrayStatusID = 1";
                else
                    FilterString = "TrayStatusID = 1";

            if (Dispatched)
                if (FilterString.Length > 0)
                    FilterString += " OR TrayStatusID = 2";
                else
                    FilterString = "TrayStatusID = 2";

            TraysBindingSource.Filter = FilterString;

            return TraysBindingSource.Count > 0;
        }

        public void FilterPackages(int TrayID)
        {
            PackagesDataTable.Clear();

            string SelectionCommand = "SELECT Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE TrayID = " + TrayID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }
            //PackagesDataTable.DefaultView.Sort = "PackNumber ASC";
            PackagesBindingSource.MoveFirst();
        }

        public void Filter(int PackageID, int ProductType)
        {
            if (ProductType == 0)
            {
                OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.Filter(PackageID);
                OrdersTabControl.TabPages[1].PageVisible = false;
            }
            if (ProductType == 1)
            {
                OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.Filter(PackageID);
                OrdersTabControl.TabPages[0].PageVisible = false;
            }
        }
        #endregion

        public void AddTray()
        {
            DataRow NewRow = TraysDataTable.NewRow();
            TraysDataTable.Rows.Add(NewRow);
            SaveTrays();
        }

        public void RemoveTray(int TrayID)
        {
            if (TraysBindingSource == null || TraysBindingSource.Count < 1)
                return;

            DataRow[] Rows = TraysDataTable.Select("TrayID = " + TrayID);

            if (Rows.Count() > 0)
            {
                Rows[0]["TrayStatusID"] = 0;
                Rows[0]["ExpeditionDateTime"] = DBNull.Value;
                Rows[0]["StorageDateTime"] = DBNull.Value;
                Rows[0]["DispatchDateTime"] = DBNull.Value;
            }

            ClearTrayPackages(TrayID);
            SaveTrays();
        }

        public void ClearTrayPackages(int TrayID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("UPDATE Packages SET TrayID = NULL WHERE TrayID = " + TrayID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void ClearPackagesGrid()
        {
            PackagesDataTable.Clear();
        }

        public void ClearOrdersGrid()
        {
            PackedMainOrdersFrontsOrders.FrontsOrdersDataTable.Clear();
            PackedMainOrdersDecorOrders.DecorOrdersDataTable.Clear();
        }

        public void SaveTrays()
        {
            int Pos = TraysBindingSource.Position;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Trays", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TraysDataTable);
                    TraysDataTable.Clear();
                    DA.Fill(TraysDataTable);
                }
            }

            //остается на позиции удаленного
            if (TraysBindingSource.Count > 0)
                if (Pos >= TraysBindingSource.Count)
                {
                    TraysBindingSource.MoveLast();
                    TraysDataGrid.Rows[TraysDataGrid.Rows.Count - 1].Selected = true;
                }
                else
                    TraysBindingSource.Position = Pos;
        }

        public int[] GetTrayIDs()
        {
            int[] rows = new int[TraysDataGrid.SelectedRows.Count];

            for (int i = 0; i < TraysDataGrid.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(TraysDataGrid.SelectedRows[i].Cells["TrayID"].Value);

            return rows;
        }
    }






    public class CheckTray
    {
        public int CurrentClientID = 0;
        public string CurrentClientName = string.Empty;
        public int CurrentTrayID = 0;
        public int CurrentGroupType = 1;

        public bool IsNewTray = true;

        PercentageDataGrid PackagesDataGrid = null;
        PercentageDataGrid FrontsContentDataGrid = null;
        PercentageDataGrid DecorContentDataGrid = null;
        PercentageDataGrid FrontsPackContentDataGrid = null;
        PercentageDataGrid DecorPackContentDataGrid = null;

        DataTable PackageInfoDT = null;
        DataTable OrderStatusInfoDT = null;
        DataTable ClientsDataTable = null;
        DataTable MarketPackagesDataTable = null;
        DataTable ZOVPackagesDataTable = null;
        DataTable PackagesDataTable = null;

        DataTable FrontsContentDataTable = null;
        DataTable DecorContentDataTable = null;
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
        DataTable MarketClientsDataTable;

        DataTable DecorDataTable;
        DataTable DecorProductsDataTable;
        DataTable TempDataTable;

        public BindingSource ClientsBindingSource = null;
        public BindingSource PackagesBindingSource = null;
        public BindingSource FrontsContentBindingSource = null;
        public BindingSource DecorContentBindingSource = null;
        public BindingSource FrontsPackContentBindingSource = null;
        public BindingSource DecorPackContentBindingSource = null;

        public DataTable EventsDataTable;

        static TimeSpan DeltaTime;

        public struct PackLabelInfo
        {
            public string Group;
            public string ClientName;
            public string Factory;
            public string MegaOrderNumber;
            public string MainOrderNumber;
            public string OrderDate;
            public string DispatchDate;
            public string CurrentPackNumber;
            public string ProductType;
            public string PackedToTotal;
            public Color DispatchDateColor;
            public Color TotalLabelColor;

            public int ClientID;
            public int MegaOrderID;
            public int MainOrderID;
            public object Dispatch;
            public DateTime DocDateTime;
            public int Product;
        }

        public struct TrayLabelInfo
        {
            public string CurrentPackNumber;
            public string PackedToTotal;
            public string ClientName;
            public string DispatchDate;
            public string Group;
            public int TrayID;
            public Color DispatchDateColor;
            public Color TotalLabelColor;
        }

        public PackLabelInfo LabelInfo;

        public CheckTray(ref PercentageDataGrid tPackagesDataGrid, ref PercentageDataGrid tFrontsPackContentDataGrid, ref PercentageDataGrid tDecorPackContentDataGrid,
            ref PercentageDataGrid tFrontsContentDataGrid, ref PercentageDataGrid tDecorContentDataGrid)
        {
            PackagesDataGrid = tPackagesDataGrid;
            FrontsContentDataGrid = tFrontsContentDataGrid;
            DecorContentDataGrid = tDecorContentDataGrid;

            FrontsPackContentDataGrid = tFrontsPackContentDataGrid;
            DecorPackContentDataGrid = tDecorPackContentDataGrid;

            Initialize();
        }

        #region Properties

        public int ScanPackgesCount
        {
            get { return TempDataTable.Rows.Count; }
        }

        #endregion

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            PackagesGridSetting();
            FrontsGridSettings();
            DecorGridSettings();
        }

        private void Create()
        {
            DeltaTime = Security.GetCurrentDate() - DateTime.Now;
            EventsDataTable = new DataTable();

            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            MarketPackagesDataTable = new DataTable();
            ZOVPackagesDataTable = new DataTable();

            FrontsContentDataTable = new DataTable();
            DecorContentDataTable = new DataTable();
            FrontsPackContentDataTable = new DataTable();
            DecorPackContentDataTable = new DataTable();

            PackagesDataTable = new DataTable();
            PackagesDataTable.Columns.Add(new DataColumn("PackageID", Type.GetType("System.Int32")));
            PackagesDataTable.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            PackagesDataTable.Columns.Add(new DataColumn("OrderNumber", Type.GetType("System.String")));
            PackagesDataTable.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            PackagesDataTable.Columns.Add(new DataColumn("Group", Type.GetType("System.String")));
            PackagesDataTable.Columns.Add(new DataColumn("GroupType", Type.GetType("System.Int32")));
            PackagesDataTable.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            PackagesDataTable.Columns.Add(new DataColumn("TrueGroup", Type.GetType("System.Boolean")));

            TempDataTable = new DataTable();
            TempDataTable.Columns.Add(new DataColumn("Group", Type.GetType("System.Int32")));//1 - ЗОВ, 2 - Маркетинг
            TempDataTable.Columns.Add(new DataColumn("PackageID", Type.GetType("System.Int32")));
            TempDataTable.Columns.Add(new DataColumn("TrueGroup", Type.GetType("System.Boolean")));

            OrderStatusInfoDT = new DataTable()
            {
                TableName = "OrderStatusInfo"
            };
            OrderStatusInfoDT.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("FactoryID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilProductionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilStorageStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("ProfilDispatchStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSProductionStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSStorageStatusID", Type.GetType("System.Int32")));
            OrderStatusInfoDT.Columns.Add(new DataColumn("TPSDispatchStatusID", Type.GetType("System.Int32")));

            PackageInfoDT = new DataTable()
            {
                TableName = "PackageInfo"
            };
            PackageInfoDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            PackageInfoDT.Columns.Add(new DataColumn("MegaOrderID", Type.GetType("System.Int32")));
            PackageInfoDT.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.Int32")));
            PackageInfoDT.Columns.Add(new DataColumn("DispatchDate", Type.GetType("System.DateTime")));
            PackageInfoDT.Columns.Add(new DataColumn("OrderDate", Type.GetType("System.DateTime")));
            PackageInfoDT.Columns.Add(new DataColumn("ProductType", Type.GetType("System.Int32")));
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TrayEventsJournal",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(EventsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 Packages.*, infiniu2_marketingreference.dbo.Clients.ClientName," +
                " MegaOrders.OrderNumber, MainOrders.Notes, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MarketPackagesDataTable);
            }

            string fltrstr = "SELECT TOP 0 FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(fltrstr, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsContentDataTable);
            }

            fltrstr = "SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count FROM DecorOrders";

            using (SqlDataAdapter DA = new SqlDataAdapter(fltrstr, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorContentDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsPackContentDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count FROM DecorOrders", ConnectionStrings.MarketingOrdersConnectionString))
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

            MarketClientsDataTable = new DataTable();
            ZOVClientsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
                DA.Fill(MarketClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ZOVClientsDataTable);
            }
        }

        private void Binding()
        {
            ClientsBindingSource = new BindingSource()
            {
                DataSource = ClientsDataTable
            };
            PackagesBindingSource = new BindingSource()
            {
                DataSource = PackagesDataTable
            };
            PackagesDataGrid.DataSource = PackagesBindingSource;

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

            FrontsContentBindingSource = new BindingSource()
            {
                DataSource = FrontsContentDataTable
            };
            FrontsContentDataGrid.DataSource = FrontsContentBindingSource;

            DecorContentBindingSource = new BindingSource()
            {
                DataSource = DecorContentDataTable
            };
            DecorContentDataGrid.DataSource = DecorContentBindingSource;
        }

        private DataGridViewComboBoxColumn FrontsColumn
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

        private DataGridViewComboBoxColumn FrameColorsColumn
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

        private DataGridViewComboBoxColumn PatinaColumn
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

        private DataGridViewComboBoxColumn InsetTypesColumn
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

        private DataGridViewComboBoxColumn InsetColorsColumn
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

        private DataGridViewComboBoxColumn TechnoProfilesColumn
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
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn
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

        private DataGridViewComboBoxColumn TechnoInsetTypesColumn
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

        private DataGridViewComboBoxColumn TechnoInsetColorsColumn
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

        private void CreateColumns()
        {
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

            FrontsContentDataGrid.Columns.Add(FrontsColumn);
            FrontsContentDataGrid.Columns.Add(FrameColorsColumn);
            FrontsContentDataGrid.Columns.Add(PatinaColumn);
            FrontsContentDataGrid.Columns.Add(InsetTypesColumn);
            FrontsContentDataGrid.Columns.Add(InsetColorsColumn);
            FrontsContentDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsContentDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsContentDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsContentDataGrid.Columns.Add(TechnoInsetColorsColumn);

            DecorContentDataGrid.Columns.Add(ProductColumn);
            DecorContentDataGrid.Columns.Add(ItemColumn);
            DecorContentDataGrid.Columns.Add(DecorPatinaColumn);
            DecorContentDataGrid.Columns.Add(ColorColumn);
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

        #region Settings
        private void PackagesGridSetting()
        {
            foreach (DataGridViewColumn Column in PackagesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            DataGridViewImageColumn ImageColumn = new DataGridViewImageColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "ImageColumn",
                Image = global::Infinium.Properties.Resources.CancelGrid
            };
            ImageColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PackagesDataGrid.Columns.Insert(0, ImageColumn);

            PackagesDataGrid.Columns["GroupType"].Visible = false;
            PackagesDataGrid.Columns["TrueGroup"].Visible = false;

            PackagesDataGrid.Columns["MainOrderID"].HeaderText = "№\r\nподзаказа";
            PackagesDataGrid.Columns["ImageColumn"].HeaderText = "";
            PackagesDataGrid.Columns["PackageID"].HeaderText = "ID";
            PackagesDataGrid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            PackagesDataGrid.Columns["Notes"].HeaderText = "Примечание";
            PackagesDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            PackagesDataGrid.Columns["Group"].HeaderText = "Группа";

            PackagesDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["MainOrderID"].Width = 120;
            PackagesDataGrid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageID"].Width = 100;
            PackagesDataGrid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["Group"].Width = 100;
            PackagesDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PackagesDataGrid.Columns["ClientName"].MinimumWidth = 190;
            PackagesDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["OrderNumber"].Width = 170;
            PackagesDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PackagesDataGrid.Columns["Notes"].MinimumWidth = 100;
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
            FrontsPackContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsPackContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsPackContentDataGrid.Columns["InsetTypeID"].Visible = false;
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

            foreach (DataGridViewColumn Column in FrontsContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            FrontsContentDataGrid.Columns["FrontID"].Visible = false;
            FrontsContentDataGrid.Columns["ColorID"].Visible = false;
            FrontsContentDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsContentDataGrid.Columns["PatinaID"].Visible = false;
            FrontsContentDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsContentDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsContentDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsContentDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsContentDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            FrontsContentDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            DisplayIndex = 0;
            FrontsContentDataGrid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsPackContentDataGrid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsContentDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;

            FrontsContentDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsContentDataGrid.Columns["Height"].Width = 90;
            FrontsContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsContentDataGrid.Columns["Width"].Width = 90;
            FrontsContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsContentDataGrid.Columns["Count"].Width = 90;

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

            foreach (DataGridViewColumn Column in DecorContentDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            DecorContentDataGrid.Columns["ProductID"].Visible = false;
            DecorContentDataGrid.Columns["DecorID"].Visible = false;
            DecorContentDataGrid.Columns["ColorID"].Visible = false;
            DecorContentDataGrid.Columns["PatinaID"].Visible = false;

            DecorContentDataGrid.Columns["Height"].HeaderText = "Высота";
            DecorContentDataGrid.Columns["Length"].HeaderText = "Длина";
            DecorContentDataGrid.Columns["Width"].HeaderText = "Ширина";
            DecorContentDataGrid.Columns["Count"].HeaderText = "Кол-во";

            DecorContentDataGrid.AutoGenerateColumns = false;
            DisplayIndex = 0;
            DecorContentDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            DecorContentDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            DecorContentDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            DecorContentDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;

            DecorContentDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorContentDataGrid.Columns["Height"].Width = 90;
            DecorContentDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorContentDataGrid.Columns["Length"].Width = 90;
            DecorContentDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorContentDataGrid.Columns["Width"].Width = 90;
            DecorContentDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorContentDataGrid.Columns["Count"].Width = 90;
        }

        #endregion

        #region Filters

        private void FillFrontsContent(int PackageID, int Group)
        {
            FrontsContentDataTable.Clear();

            string fltrstr = "SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID,  FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders" +
                " WHERE FrontsOrdersID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")";

            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(fltrstr, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(FrontsContentDataTable);
                }
            }

            if (Group == 2)//Market
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(fltrstr, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(FrontsContentDataTable);
                }
            }
        }

        private void FillDecorContent(int PackageID, int Group)
        {
            DecorContentDataTable.Clear();

            string fltrstr = "SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count FROM DecorOrders" +
                " WHERE DecorOrderID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")";

            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(fltrstr, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DecorContentDataTable);
                }
            }

            if (Group == 2)//Market
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(fltrstr, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DecorContentDataTable);
                }
            }
        }

        private void FillFrontsPackContent(int PackageID, int Group)
        {
            FrontsPackContentDataTable.Clear();

            if (Group == 1)//ZOV
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders WHERE " +
                                                              " FrontsOrdersID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(FrontsPackContentDataTable);
                }
            }

            if (Group == 2)//Market
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrders.FrontID, FrontsOrders.ColorID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.InsetColorID, FrontsOrders.TechnoProfileID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, Height, Width, Count FROM FrontsOrders WHERE " +
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
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count FROM DecorOrders WHERE " +
                                                              " DecorOrderID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DecorPackContentDataTable);
                }
            }

            if (Group == 2)//Markt
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,  DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count FROM DecorOrders WHERE " +
                                                              " DecorOrderID IN (SELECT OrderID FROM PackageDetails WHERE PackageID = " + PackageID + ")", ConnectionStrings.MarketingOrdersConnectionString))
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

        public void FillPackages()
        {
            PackagesDataTable.Clear();

            DataRow[] MRows = TempDataTable.Select("Group = 2");

            foreach (DataRow row in MRows)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, MegaOrders.ClientID, infiniu2_marketingreference.dbo.Clients.ClientName," +
                    " MegaOrders.OrderNumber, MainOrders.Notes, MainOrders.MainOrderID FROM Packages" +
                    " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                    " WHERE PackageID = " + row["PackageID"],
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = PackagesDataTable.NewRow();
                        NewRow["PackageID"] = row["PackageID"];
                        NewRow["MainOrderID"] = DT.Rows[0]["MainOrderID"];
                        NewRow["GroupType"] = 2;
                        NewRow["Group"] = "Маркетинг";
                        NewRow["ClientName"] = DT.Rows[0]["ClientName"];
                        NewRow["OrderNumber"] = DT.Rows[0]["OrderNumber"];
                        NewRow["Notes"] = DT.Rows[0]["Notes"];
                        if (CurrentGroupType == 2)
                            NewRow["TrueGroup"] = true;
                        if (CurrentGroupType == 1)
                            NewRow["TrueGroup"] = false;
                        if (CurrentClientID != Convert.ToInt32(DT.Rows[0]["ClientID"]))
                            NewRow["TrueGroup"] = false;
                        PackagesDataTable.Rows.Add(NewRow);
                    }
                }
            }

            DataRow[] ZRows = TempDataTable.Select("Group = 1");

            foreach (DataRow row in ZRows)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, infiniu2_zovreference.dbo.Clients.ClientName," +
                    " MainOrders.DocNumber, MainOrders.Notes, MainOrders.MainOrderID FROM Packages" +
                    " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                    " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                    " WHERE PackageID = " + row["PackageID"],
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = PackagesDataTable.NewRow();
                        NewRow["PackageID"] = row["PackageID"];
                        NewRow["MainOrderID"] = DT.Rows[0]["MainOrderID"];
                        NewRow["GroupType"] = 1;
                        NewRow["Group"] = "ЗОВ";
                        NewRow["ClientName"] = DT.Rows[0]["ClientName"];
                        NewRow["OrderNumber"] = DT.Rows[0]["DocNumber"];
                        NewRow["Notes"] = DT.Rows[0]["Notes"];
                        if (CurrentGroupType == 1)
                            NewRow["TrueGroup"] = true;
                        if (CurrentGroupType == 2)
                            NewRow["TrueGroup"] = false;
                        PackagesDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        public bool FillTrayPackages(string Barcode)
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

            for (int i = 0; i < PackagesDataTable.Rows.Count; i++)
            {
                PackagesDataTable.Rows[i]["Group"] = Group;
            }

            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.MoveFirst();

            return PackagesDataTable.Rows.Count > 0;
        }

        public void FilterPackages(int PackageID, string Group)
        {
            int GroupID = 1;
            if (Group == "Маркетинг")
                GroupID = 2;

            if (CheckProductType(PackageID, Group) == 0)
            {
                FillFrontsContent(PackageID, GroupID);
                FrontsContentDataGrid.BringToFront();
            }
            else
            {
                FillDecorContent(PackageID, GroupID);
                DecorContentDataGrid.BringToFront();
            }
        }

        #endregion

        #region Check
        public bool CheckPackBarcode(string Barcode)
        {
            int FactoryID = 0;
            string Prefix = Barcode.Substring(0, 3);

            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            if (Prefix == "001" || Prefix == "002")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count == 0)
                        {
                            if (Prefix == "001")
                                FactoryID = 1;
                            if (Prefix == "002")
                                FactoryID = 2;
                            AddEvent(true, 1, FactoryID, -1, Convert.ToInt32(Barcode.Substring(3, 9)), string.Empty, string.Empty, "В таблице Packages нет упаковки с таким номером!");
                        }

                        return DT.Rows.Count > 0;
                    }
                }
            }

            if (Prefix == "003" || Prefix == "004")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE PackageID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count == 0)
                        {
                            if (Prefix == "003")
                                FactoryID = 1;
                            if (Prefix == "004")
                                FactoryID = 2;
                            AddEvent(true, 2, FactoryID, -1, Convert.ToInt32(Barcode.Substring(3, 9)), string.Empty, string.Empty, "В таблице Packages нет упаковки с таким номером!");
                        }

                        return DT.Rows.Count > 0;
                    }
                }
            }

            return false;
        }

        public bool CheckTrayBarcode(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Trays WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)),
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count == 0)
                        {
                            AddEvent(true, 1, -1, Convert.ToInt32(Barcode.Substring(3, 9)), -1, string.Empty, string.Empty, "В таблице Trays нет поддона с таким номером!");
                        }

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
                        DA.Fill(DT);
                        if (DT.Rows.Count == 0)
                        {
                            AddEvent(true, 2, -1, Convert.ToInt32(Barcode.Substring(3, 9)), -1, string.Empty, string.Empty, "В таблице Trays нет поддона с таким номером!");
                        }

                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public bool IsTrayNotEmpty(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            if (Prefix == "005")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Packages WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            if (Prefix == "006")
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Packages WHERE TrayID = " + Convert.ToInt32(Barcode.Substring(3, 9)), ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        return DA.Fill(DT) > 0;
                    }
                }
            }

            return false;
        }

        public int CheckProductType(int PackageID, string Group)
        {
            string ConnectionString;

            if (Group == "Маркетинг")
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;
            else
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProductType FROM Packages" +
                " WHERE PackageID = " + PackageID, ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToInt32(DT.Rows[0]["ProductType"]);
                }
            }
        }

        public bool IsNotPacked(int GroupType, int PackageID)
        {
            string ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (GroupType == 1)
            {
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageStatusID FROM Packages" +
                " WHERE PackageID = " + PackageID, ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (Convert.ToInt32(DT.Rows[0]["PackageStatusID"]) == 0)
                        return true;
                }
            }

            return false;
        }

        public int IsPackageOnTray(int GroupType, int PackageID)
        {
            int TrayID = -1;
            string ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (GroupType == 1)
            {
                ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID FROM Packages" +
                " WHERE PackageID = " + PackageID, ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 &&
                        DT.Rows[0]["TrayID"] != DBNull.Value)
                        TrayID = Convert.ToInt32(DT.Rows[0]["TrayID"]);
                }
            }

            return TrayID;
        }

        public bool IsWrongClient(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
                " WHERE MegaOrderID = (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (Convert.ToInt32(DT.Rows[0]["ClientID"]) != CurrentClientID)
                        return true;
                }
            }
            return false;
        }

        private bool IsPackageScan(int GroupType, int PackageID)
        {
            DataColumn[] dcolPk = new DataColumn[2];
            dcolPk[0] = TempDataTable.Columns["Group"];
            dcolPk[1] = TempDataTable.Columns["PackageID"];
            TempDataTable.PrimaryKey = dcolPk;

            object[] key = new object[] { GroupType, PackageID };
            bool B = TempDataTable.Rows.Contains(key);

            return B;
        }

        public bool ExcessGroup
        {
            get
            {
                DataRow[] Rows = PackagesDataTable.Select("TrueGroup = false");

                return Rows.Count() > 0;
            }
        }
        #endregion

        #region Set

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
                FrontsPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.Red;
                FrontsPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;

                DecorPackContentDataGrid.StateCommon.Background.Color1 = Color.Red;
                DecorPackContentDataGrid.StateCommon.Background.Color2 = Color.Transparent;
                DecorPackContentDataGrid.StateCommon.Background.ColorStyle = PaletteColorStyle.Solid;
                DecorPackContentDataGrid.StateCommon.DataCell.Back.Color1 = Color.Red;
                DecorPackContentDataGrid.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.White;
            }
        }

        public void SetTotalLabel(int TotalPackCount)
        {
            LabelInfo.PackedToTotal = ScanPackgesCount.ToString() + "/" + TotalPackCount.ToString();

            if (ScanPackgesCount == TotalPackCount)
                LabelInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
            if (ScanPackgesCount > TotalPackCount)
                LabelInfo.TotalLabelColor = Color.Red;
            if (ScanPackgesCount < TotalPackCount)
                LabelInfo.TotalLabelColor = Color.Black;
        }
        #endregion

        #region Get

        public string GetOrderDate(string Barcode)
        {
            string Prefix = Barcode.Substring(0, 3);

            string DateTime = "";

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

        public void GetClientID(int TrayID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
                " WHERE MegaOrderID = (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID = " +
                " (SELECT TOP 1 MainOrderID FROM Packages WHERE TrayID = " + TrayID + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        CurrentClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }
        }

        public string GetMarketClientName(int ClientID)
        {
            string ClientName = string.Empty;

            DataRow[] Rows = MarketClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetZOVClientName(int ClientID)
        {
            string ClientName = string.Empty;

            DataRow[] Rows = ZOVClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public void GetPackLabelInfo(string Barcode)
        {
            string BarType = Barcode.Substring(0, 3);
            int PackageID = Convert.ToInt32(Barcode.Substring(3, 9));

            int GroupType = -1;
            int FactoryID = -1;

            string OrderStatusInfo = string.Empty;
            string PackageInfo = string.Empty;

            if (BarType == "001")//profil zov
            {
                GroupType = 1;
                LabelInfo.Group = "ЗОВ";
                FactoryID = 1;
                LabelInfo.Factory = "Профиль";
            }
            if (BarType == "002")
            {
                GroupType = 1;
                LabelInfo.Group = "ЗОВ";
                FactoryID = 2;
                LabelInfo.Factory = "ТПС";
            }
            if (BarType == "003")//profil market
            {
                GroupType = 2;
                LabelInfo.Group = "Маркетинг";
                FactoryID = 1;
                LabelInfo.Factory = "Профиль";
            }
            if (BarType == "004")
            {
                GroupType = 2;
                LabelInfo.Group = "Маркетинг";
                FactoryID = 2;
                LabelInfo.Factory = "ТПС";
            }

            if (GroupType == 1)//ЗОВ
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrders.MainOrderID, MainOrders.MegaOrderID, DocNumber, ClientID," +
                    " MainOrders.ProfilPackCount, MainOrders.TPSPackCount, DocDateTime, DispatchDate, PackNumber FROM MainOrders" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN Packages ON MainOrders.MainOrderID = Packages.MainOrderID AND Packages.PackageID = " + PackageID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count == 0)
                        {
                            AddEvent(true, GroupType, -1, -1, PackageID, string.Empty, string.Empty, "Упаковка не найдена!");
                            return;
                        }

                        LabelInfo.ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                        LabelInfo.DocDateTime = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]);
                        LabelInfo.MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                        LabelInfo.MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                        LabelInfo.ClientName = GetZOVClientName(LabelInfo.ClientID);
                        LabelInfo.CurrentPackNumber = DT.Rows[0]["PackNumber"].ToString();

                        if (DT.Rows[0]["DispatchDate"] != DBNull.Value)
                        {
                            LabelInfo.Dispatch = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]);
                            LabelInfo.DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                            if (Convert.ToDateTime(DT.Rows[0]["DispatchDate"]) < Convert.ToDateTime(Security.GetCurrentDate().ToString("dd.MM.yyyy")))
                                LabelInfo.DispatchDateColor = Color.Red;
                            else
                                LabelInfo.DispatchDateColor = Color.FromArgb(82, 169, 24);
                        }

                        LabelInfo.OrderDate = Convert.ToDateTime(DT.Rows[0]["DocDateTime"]).ToString("dd.MM.yyyy");
                        LabelInfo.MegaOrderNumber = DT.Rows[0]["MegaOrderID"].ToString();
                        LabelInfo.MainOrderNumber = DT.Rows[0]["DocNumber"].ToString();

                        //SetTotalLabel(TotalPackCount);
                    }
                }

                if (CheckProductType(PackageID, LabelInfo.Group) == 0)
                {
                    LabelInfo.Product = 1;
                    LabelInfo.ProductType = "Фасады";
                    FillFrontsPackContent(PackageID, 1);
                    FrontsPackContentDataGrid.BringToFront();
                }
                else
                {
                    LabelInfo.Product = 2;
                    LabelInfo.ProductType = "Погонаж и декор";
                    FillDecorPackContent(PackageID, 1);
                    DecorPackContentDataGrid.BringToFront();
                }

                return;
            }


            if (GroupType == 2)//Маркетинг
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrders.MainOrderID, MainOrders.MegaOrderID," +
                    " MainOrders.ProfilPackCount, MainOrders.TPSPackCount, Packages.PackNumber, " +
                    " MegaOrders.ClientID, MegaOrders.ProfilDispatchDate, MegaOrders.TPSDispatchDate, MegaOrders.OrderDate, MegaOrders.OrderNumber FROM MainOrders" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN Packages ON MainOrders.MainOrderID = Packages.MainOrderID AND Packages.PackageID = " + PackageID,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count == 0)
                        {
                            AddEvent(true, GroupType, -1, -1, PackageID, string.Empty, string.Empty, "Упаковка не найдена!");
                            return;
                        }

                        LabelInfo.ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                        LabelInfo.DocDateTime = Convert.ToDateTime(DT.Rows[0]["OrderDate"]);
                        LabelInfo.MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                        LabelInfo.MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);

                        LabelInfo.ClientName = GetMarketClientName(LabelInfo.ClientID);
                        LabelInfo.CurrentPackNumber = DT.Rows[0]["PackNumber"].ToString();

                        if (FactoryID == 1)
                        {
                            if (DT.Rows[0]["ProfilDispatchDate"] != DBNull.Value)
                                LabelInfo.DispatchDate = Convert.ToDateTime(DT.Rows[0]["ProfilDispatchDate"]).ToString("dd.MM.yyyy");
                        }
                        if (FactoryID == 2)
                        {
                            if (DT.Rows[0]["TPSDispatchDate"] != DBNull.Value)
                                LabelInfo.DispatchDate = Convert.ToDateTime(DT.Rows[0]["TPSDispatchDate"]).ToString("dd.MM.yyyy");
                        }

                        if (!string.IsNullOrEmpty(LabelInfo.DispatchDate)
                            && Convert.ToDateTime(LabelInfo.DispatchDate) < Convert.ToDateTime(Security.GetCurrentDate().ToString("dd.MM.yyyy")))
                            LabelInfo.DispatchDateColor = Color.Red;
                        else
                            LabelInfo.DispatchDateColor = Color.FromArgb(82, 169, 24);

                        LabelInfo.OrderDate = Convert.ToDateTime(DT.Rows[0]["OrderDate"]).ToString("dd.MM.yyyy");
                        LabelInfo.MegaOrderNumber = DT.Rows[0]["OrderNumber"].ToString();
                        LabelInfo.MainOrderNumber = DT.Rows[0]["MainOrderID"].ToString();

                        //SetTotalLabel(TotalPackCount);
                    }
                }

                if (CheckProductType(PackageID, LabelInfo.Group) == 0)
                {
                    LabelInfo.Product = 1;
                    LabelInfo.ProductType = "Фасады";
                    FillFrontsPackContent(PackageID, 2);
                    FrontsPackContentDataGrid.BringToFront();
                }
                else
                {
                    LabelInfo.Product = 2;
                    LabelInfo.ProductType = "Погонаж и декор";
                    FillDecorPackContent(PackageID, 2);
                    DecorPackContentDataGrid.BringToFront();
                }
            }

            return;
        }

        public void AddToTray(int GroupType, int FactoryID, int PackageID)
        {
            if (!IsPackageScan(CurrentGroupType, PackageID))
            {
                DataRow NewRow = TempDataTable.NewRow();
                NewRow["Group"] = GroupType;
                NewRow["PackageID"] = PackageID;
                NewRow["TrueGroup"] = true;
                TempDataTable.Rows.Add(NewRow);
            }
            else
            {
                AddEvent(true, GroupType, FactoryID, -1, PackageID, string.Empty, string.Empty, "Упаковка уже на поддоне");
            }
        }

        #endregion

        public void Clear()
        {
            FrontsPackContentDataTable.Clear();
            DecorPackContentDataTable.Clear();

            LabelInfo.ClientName = string.Empty;
            LabelInfo.CurrentPackNumber = string.Empty;
            LabelInfo.DispatchDate = string.Empty;
            LabelInfo.DispatchDateColor = Color.White;
            LabelInfo.Factory = string.Empty;
            LabelInfo.Group = string.Empty;
            LabelInfo.MainOrderNumber = string.Empty;
            LabelInfo.MegaOrderNumber = string.Empty;
            LabelInfo.OrderDate = string.Empty;
            LabelInfo.PackedToTotal = string.Empty;
            LabelInfo.ProductType = string.Empty;
            LabelInfo.TotalLabelColor = Color.White;
        }

        public void CleareTables()
        {
            Clear();
            FrontsContentDataTable.Clear();
            DecorContentDataTable.Clear();
            TempDataTable.Clear();
        }

        public void RemoveCurrent()
        {
            int Pos = PackagesBindingSource.Position;
            int PackageID = Convert.ToInt32(((DataRowView)PackagesBindingSource.Current).Row["PackageID"]);
            string Group = ((DataRowView)PackagesBindingSource.Current).Row["Group"].ToString();
            int GroupType = 1;

            if (Group == "Маркетинг")
                GroupType = 2;
            PackagesBindingSource.RemoveCurrent();

            //остается на позиции удаленного
            if (PackagesBindingSource.Count > 0)
                if (Pos >= PackagesBindingSource.Count)
                {
                    PackagesBindingSource.MoveLast();
                    PackagesDataGrid.Rows[PackagesDataGrid.Rows.Count - 1].Selected = true;
                }
                else
                    PackagesBindingSource.Position = Pos;

            ((DataTable)PackagesBindingSource.DataSource).AcceptChanges();

            DataRow[] Rows = TempDataTable.Select("PackageID = " + PackageID + " AND Group = " + GroupType);

            if (Rows.Count() > 0)
            {
                Rows[0].Delete();
                TempDataTable.AcceptChanges();
            }

            AddEvent(false, GroupType, -1, -1, PackageID, string.Empty, string.Empty, "Упаковка удалена с поддона");
        }

        public bool SavePackages(string Barcode)
        {
            int TrayID = Convert.ToInt32(Barcode.Substring(3, 9));

            string ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            if (CurrentGroupType == 2)
                ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            int[] PackageIDs = new int[PackagesDataTable.Rows.Count];

            for (int i = 0; i < PackagesDataTable.Rows.Count; i++)
                PackageIDs[i] = Convert.ToInt32(PackagesDataTable.Rows[i]["PackageID"]);

            //for (int i = 93243, j=0; i <= 93342; i++, j++)
            //    PackageIDs[j] = i;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID," +
                " PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime, PackUserID, StoreUserID, ExpUserID, DispUserID, TrayID FROM Packages" +
                " WHERE PackageID IN (" + string.Join(",", PackageIDs) + ")", ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            DA.Fill(DT);

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (IsNewTray)
                                    DT.Rows[i]["TrayID"] = TrayID;
                                else
                                    DT.Rows[i]["TrayID"] = CurrentTrayID;
                                if (DT.Rows[i]["PackageStatusID"] != DBNull.Value && Convert.ToInt32(DT.Rows[i]["PackageStatusID"]) != 3)
                                    DT.Rows[i]["PackageStatusID"] = 2;
                                if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PackingDateTime"] = Security.GetCurrentDate();
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = Security.GetCurrentDate();
                                //DT.Rows[i]["ExpeditionDateTime"] = DBNull.Value;
                                //DT.Rows[i]["DispatchDateTime"] = DBNull.Value;

                                if (DT.Rows[i]["PackUserID"] == DBNull.Value)
                                    DT.Rows[i]["PackUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["StoreUserID"] == DBNull.Value)
                                    DT.Rows[i]["StoreUserID"] = Security.CurrentUserID;
                                //DT.Rows[i]["ExpUserID"] = DBNull.Value;
                                //DT.Rows[i]["DispUserID"] = DBNull.Value;

                            }

                            DA.Update(DT);
                            IsNewTray = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            IsNewTray = false;
                            return false;
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TrayID, TrayStatusID, StorageDateTime FROM Trays WHERE TrayID = " + TrayID, ConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            DA.Fill(DT);

                            DT.Rows[0]["TrayStatusID"] = 1;
                            if (DT.Rows[0]["StorageDateTime"] == DBNull.Value)
                                DT.Rows[0]["StorageDateTime"] = Security.GetCurrentDate();

                            DA.Update(DT);
                        }
                        catch (Exception ex)
                        {
                            AddEvent(true, CurrentGroupType, -1, -1, -1, string.Empty, string.Empty, "Сохранение выполнено, поддон сформирован; ScanTrayPanel; SuccessPanel.BringToFront()");
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public string SetPackageInfo(
            int ClientID,
            int MegaOrderID,
            int MainOrderID,
            object DispatchDate,
            DateTime OrderDate,
            int ProductType)
        {
            string PackageInfo = string.Empty;

            PackageInfoDT.Clear();

            DataRow NewRow = PackageInfoDT.NewRow();
            if (ClientID > -1)
                NewRow["ClientID"] = ClientID;
            if (MegaOrderID > -1)
                NewRow["MegaOrderID"] = MegaOrderID;
            if (MainOrderID > -1)
                NewRow["MainOrderID"] = MainOrderID;
            if (DispatchDate != null)
                NewRow["DispatchDate"] = DispatchDate;
            NewRow["OrderDate"] = OrderDate;
            NewRow["ProductType"] = ProductType;
            PackageInfoDT.Rows.Add(NewRow);

            using (System.IO.StringWriter SW = new System.IO.StringWriter())
            {
                PackageInfoDT.WriteXml(SW);
                PackageInfo = SW.ToString();
            }

            return PackageInfo;
        }

        public void AddEvent(
            bool IsError,
            int GroupType,
            int FactoryID,
            int TrayID,
            int PackageID,
            string PackageInfo,
            string OrderStatusInfo,
            string Event)
        {
            DataRow NewRow = EventsDataTable.NewRow();
            NewRow["UserID"] = Security.CurrentUserID;
            NewRow["LoginJournalID"] = Security.CurrentLoginJournalID;
            NewRow["IsError"] = IsError;
            if (GroupType > -1)
                NewRow["GroupType"] = GroupType;
            if (FactoryID > -1)
                NewRow["FactoryID"] = FactoryID;
            if (TrayID > -1)
                NewRow["TrayID"] = TrayID;
            if (PackageID > -1)
                NewRow["PackageID"] = PackageID;
            if (PackageInfo.Length > 0)
                NewRow["PackageInfo"] = PackageInfo;
            if (OrderStatusInfo.Length > 0)
                NewRow["OrderStatusInfo"] = OrderStatusInfo;
            NewRow["Event"] = Event;
            NewRow["EventDateTime"] = DateTime.Now + DeltaTime;
            EventsDataTable.Rows.Add(NewRow);
        }

        public void SaveEvents()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TrayEventsJournal",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(EventsDataTable);
                }
            }
        }
    }







    public class Barcode
    {
        BarcodeLib.Barcode Barcod;

        SolidBrush FontBrush;

        public enum BarcodeLength { Short, Medium, Long };

        public Barcode()
        {
            Barcod = new BarcodeLib.Barcode();

            FontBrush = new System.Drawing.SolidBrush(Color.Black);
        }

        public void DrawBarcodeText(BarcodeLength BarcodeLength, Graphics Graphics, string Text, int X, int Y)
        {
            int CharOffset = 0;
            int CharWidth = 0;
            float FontSize = 0;

            if (BarcodeLength == Barcode.BarcodeLength.Short)
            {
                CharWidth = 8;
                CharOffset = 7;
                FontSize = 8.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Medium)
            {
                CharWidth = 20;
                CharOffset = 5;
                FontSize = 14.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                CharWidth = 27;
                CharOffset = 3;
                FontSize = 20.0f;
            }

            Font F = new Font("Arial", FontSize, FontStyle.Bold);

            for (int i = 0; i < Text.Length; i++)
            {
                Graphics.DrawString(Text[i].ToString(), F, FontBrush, i * CharWidth + CharOffset + X, Y + 2);
            }

            F.Dispose();
        }

        public Image GetBarcode(BarcodeLength BarcodeLength, int BarcodeHeight, string Text)
        {
            //set length and height
            if (BarcodeLength == Barcode.BarcodeLength.Short)
            {
                Barcod.Width = 101 + 12;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Medium)
            {
                Barcod.Width = 202 + 12;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                Barcod.Width = 303 + 12;
            }

            Barcod.Height = BarcodeHeight;


            //create area
            Bitmap B = new Bitmap(Barcod.Width, BarcodeHeight);
            Graphics G = Graphics.FromImage(B);
            G.Clear(Color.White);


            //create barcode
            Image Bar = Barcod.Encode(BarcodeLib.TYPE.CODE128C, Text);
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            G.DrawImage(Bar, 0, 2);


            //create text
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;


            Bar.Dispose();
            G.Dispose();

            GC.Collect();

            return B;
        }
    }





    public struct Info
    {
        public int TrayID;
        public int TotalPackCount;
        public string BarcodeNumber;
        public string GroupType;
    }




    public class TrayLabel
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font DocFont;
        Font GroupFont;
        Font NumFont;

        Pen Pen;

        public ArrayList LabelInfo;

        public TrayLabel()
        {
            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            DocFont = new Font("Arial", 20.0f, FontStyle.Bold);
            GroupFont = new Font("Arial", 32.0f, FontStyle.Bold);
            NumFont = new Font("Arial", 32.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref Info LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;

            ev.Graphics.Clear(Color.White);

            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            ev.Graphics.DrawString("№", DocFont, FontBrush, 8, 45);
            ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).TrayID.ToString(), NumFont, FontBrush, 37, 38);
            ev.Graphics.DrawLine(Pen, 11, 96, 497, 96);

            ev.Graphics.DrawLine(Pen, 220, 29, 220, 96);

            ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).GroupType, GroupFont, FontBrush, 241, 38);

            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Long, 95, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 101, 168);

            Barcode.DrawBarcodeText(Barcode.BarcodeLength.Long, ev.Graphics, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 100, 266);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }

        public string GetBarcodeNumber(int BarcodeType, int TrayID)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (TrayID.ToString().Length == 1)
                Number = "00000000" + TrayID.ToString();
            if (TrayID.ToString().Length == 2)
                Number = "0000000" + TrayID.ToString();
            if (TrayID.ToString().Length == 3)
                Number = "000000" + TrayID.ToString();
            if (TrayID.ToString().Length == 4)
                Number = "00000" + TrayID.ToString();
            if (TrayID.ToString().Length == 5)
                Number = "0000" + TrayID.ToString();
            if (TrayID.ToString().Length == 6)
                Number = "000" + TrayID.ToString();
            if (TrayID.ToString().Length == 7)
                Number = "00" + TrayID.ToString();
            if (TrayID.ToString().Length == 8)
                Number = "0" + TrayID.ToString();

            StringBuilder BarcodeNumber = new StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

    }


}
