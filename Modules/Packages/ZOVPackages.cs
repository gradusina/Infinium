using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Printing;
using System.Collections;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

namespace Infinium.Modules.Packages.ZOV
{
    public class SplitStruct
    {
        public int PackagesCount;
        public int FCount;
        public int SCount;
        public bool IsSplit;
        public bool IsEqual;
        public int FirstPosition;

        public SplitStruct()
        {
            PackagesCount = 1;
            FCount = 0;
            SCount = 0;
            IsSplit = false;
            IsEqual = false;
        }
    }





    #region Упаковка
    public class ZOVPackagesAllocFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrderID = -1;
        int FactoryID = 1;

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

        //private SqlDataAdapter PackagesDataAdapter = null;
        //private SqlCommandBuilder PackagesSqlCommandBuilder = null;

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

        public ZOVPackagesAllocFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;

            Initialize();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders WHERE FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
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
            FrontsOrdersDataGrid.Columns["Debt"].Visible = false;
            FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Visible = false;

            if (FrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                FrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                FrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;

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
            FrontsOrdersDataGrid.Columns["CupboardString"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["PackNumber"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

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
            FrontsOrdersDataGrid.Columns["PackNumber"].HeaderText = "Номер\r\nупаковки";
            FrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";

            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Height"].Width = 85;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Width"].Width = 85;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Count"].Width = 65;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 105;
            FrontsOrdersDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["PackNumber"].Width = 95;
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

        private int GetPackageID(int MainOrderID, int PackNumber)
        {
            int PackageID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0" +
                " AND FactoryID=" + FactoryID + " AND PackNumber = " + PackNumber, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                    }
                }
            }

            return PackageID;
        }

        private int GetPackageID(int MainOrderID)
        {
            int PackageID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0" +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                    }
                }
            }

            return PackageID;
        }

        private int[] GetPackageIDs(int MainOrderID)
        {
            int[] PackageIDs = { 0 };

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0" +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
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

        public bool Filter(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return false;

            int[] PackageIDs = GetPackageIDs(MainOrderID);

            CurrentMainOrderID = MainOrderID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            OriginalFrontsOrdersDataTable = FrontsOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID=" + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0" +
                " AND FactoryID=" + FactoryID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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

                        foreach (DataRow Row in OriginalFrontsOrdersDataTable.Rows)
                        {
                            DataRow[] PackRow = DT.Select("OrderID = " + Row["FrontsOrdersID"] +
                                " AND PackageID IN (" + string.Join(",", PackageIDs) + ")");

                            if (PackRow.Count() == 0)
                            {
                                DataRow NewRow = FrontsOrdersDataTable.NewRow();
                                NewRow.ItemArray = Row.ItemArray;
                                FrontsOrdersDataTable.Rows.Add(NewRow);
                            }
                        }
                    }
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                            " WHERE MainOrderID = " + MainOrderID +
                            " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(FrontsOrdersDataTable);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            if (FrontsOrdersDataTable.Rows.Count > 0)
                FrontsOrdersBindingSource.MoveFirst();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        /// <summary>
        /// Возвращает таблицу с упорядоченными номерами упаковок
        /// </summary>
        public DataTable PackagesSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        /// <summary>
        /// Возвращает количество упакованных позиций
        /// </summary>
        public int AllPackCount
        {
            get
            {
                int Count = 0;
                foreach (DataRow Row in FrontsOrdersDataTable.Rows)
                {

                    if (Row["PackNumber"] != DBNull.Value && Convert.ToInt32(Row["PackNumber"]) > 0)
                    {
                        Count++;
                    }

                }

                return Count;
            }
        }

        /// <summary>
        /// Возвращает true, если в упаковке не больше 12 фасадов
        /// </summary>
        public bool IsOverflow
        {
            get
            {
                foreach (DataRow Row in PackagesSequence.Rows)
                {
                    DataRow[] Rows = FrontsOrdersDataTable.Select("PackNumber = " + Row["PackNumber"]);

                    if (Rows.Count() > 6)
                    {
                        DataTable TempDataTable = new DataTable();

                        TempDataTable.Columns.Add(new DataColumn("FrontID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("PatinaID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("InsetTypeID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("InsetColorID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("TechnoColorID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("TechnoInsetTypeID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("TechnoInsetColorID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));

                        using (DataView DV = new DataView(FrontsOrdersDataTable))
                        {
                            DV.RowFilter = "PackNumber = " + Row["PackNumber"];
                            TempDataTable = DV.ToTable(true,
                                new string[] { "FrontID", "PatinaID", "InsetTypeID", "ColorID", "InsetColorID", "TechnoColorID", "TechnoInsetTypeID", "TechnoInsetColorID", "Height", "Width" });
                        }

                        int Count = TempDataTable.Rows.Count;

                        if (Count > 6)
                            return false;

                        TempDataTable.Dispose();
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Возвращает false, если в упаковке есть и гнутые, и негнутые фасады
        /// </summary>
        public bool IsSimpleAndCurved
        {
            get
            {
                int SimpleFronts = 0;
                int CurvedFronts = 0;

                foreach (DataRow Row in PackagesSequence.Rows)
                {
                    DataRow[] Rows = FrontsOrdersDataTable.Select("PackNumber = " + Row["PackNumber"]);

                    if (Rows.Count() > 0)
                    {
                        SimpleFronts = 0;
                        CurvedFronts = 0;

                        for (int i = 0; i < Rows.Count(); i++)
                        {
                            if (Convert.ToInt32(Rows[i]["Width"]) == -1)
                            {
                                CurvedFronts++;
                            }
                            else
                                SimpleFronts++;
                        }

                        if (SimpleFronts != 0 && CurvedFronts != 0)
                            return false;
                    }
                }
                return true;
            }
        }

        public int DifferentPackCount
        {
            get
            {
                int Count = 0;
                foreach (DataRow Row in PackagesSequence.Rows)
                {
                    Count++;
                }

                return Count;
            }
        }

        public int[] GetPackNumbers()
        {
            int[] rows = new int[PackagesSequence.Rows.Count];

            for (int i = 0; i < PackagesSequence.Rows.Count; i++)
                rows[i] = Convert.ToInt32(PackagesSequence.Rows[i]["PackNumber"]);

            return rows;
        }

        public void ClearPackage()
        {
            if (FrontsOrdersDataTable.Rows.Count == 0)
                return;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE MainOrderID = " + CurrentMainOrderID +
                " AND ProductType = 0 AND FactoryID = " + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ClearPackageDetails()
        {
            if (FrontsOrdersDataTable.Rows.Count == 0)
                return;

            DataRow[] Rows = FrontsOrdersDataTable.Select("MainOrderID = " + CurrentMainOrderID);
            foreach (DataRow Row in Rows)
            {
                Row["PackNumber"] = DBNull.Value;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE FactoryID = " + FactoryID + " AND ProductType = 0 AND MainOrderID = " + CurrentMainOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        private void SavePackages()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE MainOrderID = " + CurrentMainOrderID +
                " AND ProductType = 0 AND FactoryID = " + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        //if (DT.Rows.Count > 0)
                        //{
                        //    foreach (DataRow Row in DT.Rows)
                        //        Row.Delete();
                        //}

                        if (AllPackCount > 0)
                        {
                            foreach (DataRow Row in PackagesSequence.Rows)
                            {
                                DataRow[] Rows = DT.Select("PackNumber = " + Row["PackNumber"]);
                                if (Rows.Count() == 0)
                                {
                                    DataRow NewRow = DT.NewRow();
                                    NewRow["MainOrderID"] = CurrentMainOrderID;
                                    NewRow["PackNumber"] = Row["PackNumber"];
                                    NewRow["FactoryID"] = FactoryID;
                                    NewRow["ProductType"] = 0;
                                    DT.Rows.Add(NewRow);
                                }
                            }
                        }

                        for (int i = DT.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow[] Rows = PackagesSequence.Select("PackNumber = " + DT.Rows[i]["PackNumber"]);
                            if (Rows.Count() == 0)
                            {
                                DT.Rows[i].Delete();
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        private void SavePackageDetails()
        {
            if (FrontsOrdersDataTable.Rows.Count == 0)
                return;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE FactoryID = " + FactoryID + " AND ProductType = 0 AND MainOrderID = " + CurrentMainOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();
                        }

                        SavePackages();

                        if (AllPackCount > 0)
                        {
                            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
                            {
                                if (Row["PackNumber"] != DBNull.Value)
                                {
                                    DataRow NewRow = DT.NewRow();
                                    NewRow["PackageID"] = GetPackageID(CurrentMainOrderID, Convert.ToInt32(Row["PackNumber"]));
                                    NewRow["PackNumber"] = Row["PackNumber"];
                                    NewRow["OrderID"] = Row["FrontsOrdersID"];
                                    NewRow["Count"] = Row["Count"];
                                    DT.Rows.Add(NewRow);
                                }
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveOneMainOrder()
        {
            SavePackageDetails();
        }

        /// <summary>
        /// разделяет текущую позицию
        /// </summary>
        /// <param name="SS"></param>
        public void SplitCurrentItem(SplitStruct SS)
        {
            //если выбрано разделение на равное количество в упаковках
            if (SS.IsEqual)
            {
                if (SS.SCount == 0)
                {
                    ((DataRowView)FrontsOrdersBindingSource.Current).Row["Count"] = SS.FCount;
                    ((DataRowView)FrontsOrdersBindingSource.Current).Row["PackNumber"] = SS.FirstPosition++;
                    for (int i = 0; i < SS.PackagesCount - 1; i++)
                    {
                        DataRow Row = FrontsOrdersDataTable.NewRow();

                        Row.ItemArray = ((DataRowView)FrontsOrdersBindingSource.Current).Row.ItemArray;

                        Row["Count"] = SS.FCount;
                        Row["PackNumber"] = SS.FirstPosition + i;

                        FrontsOrdersDataTable.Rows.Add(Row);
                    }
                }
                else
                {
                    ((DataRowView)FrontsOrdersBindingSource.Current).Row["Count"] = SS.FCount;
                    ((DataRowView)FrontsOrdersBindingSource.Current).Row["PackNumber"] = SS.FirstPosition++;
                    for (int i = 0; i < SS.PackagesCount - 1; i++)
                    {
                        DataRow Row = FrontsOrdersDataTable.NewRow();

                        Row.ItemArray = ((DataRowView)FrontsOrdersBindingSource.Current).Row.ItemArray;

                        Row["Count"] = SS.FCount;
                        Row["PackNumber"] = SS.FirstPosition + i;

                        FrontsOrdersDataTable.Rows.Add(Row);
                    }

                    DataRow Row1 = FrontsOrdersDataTable.NewRow();

                    Row1.ItemArray = ((DataRowView)FrontsOrdersBindingSource.Current).Row.ItemArray;

                    Row1["Count"] = SS.SCount;
                    Row1["PackNumber"] = SS.PackagesCount + SS.FirstPosition - 1;

                    FrontsOrdersDataTable.Rows.Add(Row1);
                }
            }
            else
            {
                DataRow Row = FrontsOrdersDataTable.NewRow();

                ((DataRowView)FrontsOrdersBindingSource.Current).Row["Count"] = SS.FCount;

                Row.ItemArray = ((DataRowView)FrontsOrdersBindingSource.Current).Row.ItemArray;

                Row["Count"] = SS.SCount;
                Row["PackNumber"] = DBNull.Value;

                FrontsOrdersDataTable.Rows.Add(Row);
            }
            FrontsOrdersBindingSource.Sort = "FrontsOrdersID";
        }

        public int OrdersCount
        {
            get { return FrontsOrdersDataTable.Rows.Count; }
        }

        public int PackageAllocStatus
        {
            get
            {
                if (AllPackCount == 0)
                    return 0;
                if (AllPackCount == FrontsOrdersDataTable.Rows.Count)
                    return 2;
                return 1;
            }
        }

        /// <summary>
        /// Возвращает true, если подзаказ полностью упакован
        /// </summary>
        public bool IsPacked
        {
            get
            {
                return AllPackCount == FrontsOrdersDataTable.Rows.Count;
            }
        }

        public int DebtPack(int MainOrderID)
        {
            int PackNumber = 1;

            DataTable PackagesDT = new DataTable();
            SqlDataAdapter PackagesDA = new SqlDataAdapter("SELECT TOP 0 * FROM Packages",
                ConnectionStrings.ZOVOrdersConnectionString);
            SqlCommandBuilder PackagesCB = new SqlCommandBuilder(PackagesDA);
            PackagesDA.Fill(PackagesDT);

            DataTable PackageDetailsDT = new DataTable();
            SqlDataAdapter PackageDetailsDA = new SqlDataAdapter("SELECT TOP 0 * FROM PackageDetails",
                ConnectionStrings.ZOVOrdersConnectionString);
            SqlCommandBuilder PackageDetailsCB = new SqlCommandBuilder(PackageDetailsDA);
            PackageDetailsDA.Fill(PackageDetailsDT);
                                                              
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber,
            FrontsOrders.FrontID, FrontsOrders.PatinaID, FrontsOrders.InsetTypeID, FrontsOrders.ColorID, FrontsOrders.InsetColorID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, 
            FrontsOrders.Height, FrontsOrders.Width, FrontsOrders.Count, PackageDetails.Count
            FROM PackageDetails 
            INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
            WHERE PackageDetails.PackageID IN
            (SELECT PackageID FROM Packages WHERE ProductType = 0 AND MainOrderID = " + MainOrderID + @")
            ORDER BY PackageDetails.PackNumber, FrontsOrders.Height, FrontsOrders.Width",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) < 1)
                        return 1;

                    DT.Columns.Add(new DataColumn("Ready", Type.GetType("System.Boolean")));
                    foreach (DataRow Row in DT.Rows)
                    {
                        Row["Ready"] = false;
                    }

                    int FrontID = 0;
                    int PatinaID = 0;
                    int InsetTypeID = 0;
                    int ColorID = 0;
                    int InsetColorID = 0;
                    int AdditionalColorID = 0;
                    int Height = 0;
                    int Width = 0;

                    int OldPackNumber = 1;

                    foreach (DataRow Row in DT.Select("Ready = false"))
                    {
                        FrontID = Convert.ToInt32(Row["FrontID"]);
                        PatinaID = Convert.ToInt32(Row["PatinaID"]);
                        InsetTypeID = Convert.ToInt32(Row["InsetTypeID"]);
                        ColorID = Convert.ToInt32(Row["ColorID"]);
                        InsetColorID = Convert.ToInt32(Row["InsetColorID"]);
                        AdditionalColorID = Convert.ToInt32(Row["AdditionalColorID"]);
                        Height = Convert.ToInt32(Row["Height"]);
                        Width = Convert.ToInt32(Row["Width"]);

                        DataRow[] PRows = FrontsOrdersDataTable.Select("FrontID = " + FrontID + " AND PatinaID = " + PatinaID + " AND InsetTypeID = " + InsetTypeID +
                            " AND ColorID = " + ColorID + " AND InsetColorID = " + InsetColorID + " AND AdditionalColorID = " + AdditionalColorID +
                            " AND Height = " + Height + " AND Width = " + Width + " AND PackNumber IS NULL");

                        if (PRows.Count() < 1)
                        {
                            Row["Ready"] = true;
                            continue;
                        }

                        if (OldPackNumber != Convert.ToInt32(Row["PackNumber"]))
                            PackNumber++;
                        OldPackNumber = Convert.ToInt32(Row["PackNumber"]);

                        PRows[0]["PackNumber"] = PackNumber;
                        DataRow NewRow = PackageDetailsDT.NewRow();
                        NewRow["PackNumber"] = PackNumber;
                        NewRow["OrderID"] = PRows[0]["FrontsOrdersID"];
                        NewRow["Count"] = Row["Count"];
                        PackageDetailsDT.Rows.Add(NewRow);

                        Row["Ready"] = true;
                    }
                }
            }
            PackageDetailsDT.Dispose();

            return PackNumber;
        }
    }







    public class ZOVPackagesAllocDecorOrders
    {
        int CurrentMainOrderID = -1;
        int FactoryID = 1;

        private PercentageDataGrid MainOrdersFrontsOrdersDataGrid = null;
        private PercentageDataGrid MainOrdersDecorOrdersDataGrid = null;

        public DataTable DecorOrdersDataTable = null;

        private DataTable ColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;

        public BindingSource DecorOrdersBindingSource = null;

        //public SqlDataAdapter DecorOrdersDataAdapter = null;
        //public SqlCommandBuilder DecorOrdersCommandBuilder = null;

        //конструктор
        public ZOVPackagesAllocDecorOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid)
        {
            MainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
            MainOrdersDecorOrdersDataGrid = tMainOrdersDecorOrdersDataGrid;
            Initialize();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }
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
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
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
            MainOrdersDecorOrdersDataGrid.Columns["Debt"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Weight"].Visible = false;

            //русские названия полей

            MainOrdersDecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            MainOrdersDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].HeaderText = "  Номер\r\nупаковки";

            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].Width = 110;

            //MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].DisplayIndex = 10;

            MainOrdersDecorOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            MainOrdersDecorOrdersDataGrid.Columns["Length"].HeaderText = "Длина";
            MainOrdersDecorOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            MainOrdersDecorOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";

            int DisplayIndex = 0;
            MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Length"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in MainOrdersDecorOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private int GetPackageID(int MainOrderID, int PackNumber)
        {
            int PackageID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" +
                " AND FactoryID=" + FactoryID + " AND PackNumber = " + PackNumber, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                    }
                }
            }

            return PackageID;
        }

        private int GetPackageID(int MainOrderID)
        {
            int PackageID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                    }
                }
            }

            return PackageID;
        }

        private int[] GetPackageIDs(int MainOrderID)
        {
            int[] PackageIDs = { 0 };

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
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

        public bool Filter(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return false;

            int[] PackageIDs = GetPackageIDs(MainOrderID);

            CurrentMainOrderID = MainOrderID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = DecorOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID=" + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" +
                " AND FactoryID=" + FactoryID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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
                        foreach (DataRow Row in OriginalDecorOrdersDataTable.Rows)
                        {
                            DataRow[] PackRow = DT.Select("OrderID = " + Row["DecorOrderID"] +
                                " AND PackageID IN (" + string.Join(",", PackageIDs) + ")");

                            if (PackRow.Count() == 0)
                            {
                                DataRow NewRow = DecorOrdersDataTable.NewRow();
                                NewRow.ItemArray = Row.ItemArray;
                                DecorOrdersDataTable.Rows.Add(NewRow);
                            }
                        }
                    }
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID = " + MainOrderID +
                            " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(DecorOrdersDataTable);

                            if (DecorOrdersDataTable.Rows.Count > 0)
                            {
                                foreach (DataRow Row in DecorOrdersDataTable.Rows)
                                {
                                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                                    //    Row["ColorID"] = 0;
                                }
                            }
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            if (DecorOrdersDataTable.Rows.Count > 0)
                DecorOrdersBindingSource.MoveFirst();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        /// <summary>
        /// Возвращает таблицу с упорядоченными номерами упаковок
        /// </summary>
        public DataTable PackagesSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        /// <summary>
        /// Возвращает количество упакованных позиций
        /// </summary>
        public int AllPackCount
        {
            get
            {
                int Count = 0;
                foreach (DataRow Row in DecorOrdersDataTable.Rows)
                {

                    if (Row["PackNumber"] != DBNull.Value && Convert.ToInt32(Row["PackNumber"]) > 0)
                    {
                        Count++;
                    }

                }

                return Count;
            }
        }

        /// <summary>
        /// Возвращает true, если в упаковке не больше 12 позиций
        /// </summary>
        public bool IsOverflow
        {
            get
            {
                foreach (DataRow Row in PackagesSequence.Rows)
                {
                    DataRow[] Rows = DecorOrdersDataTable.Select("PackNumber = " + Row["PackNumber"]);

                    if (Rows.Count() > 6)
                    {
                        DataTable TempDataTable = new DataTable();

                        TempDataTable.Columns.Add(new DataColumn("ProductID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("DecorID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
                        TempDataTable.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));

                        using (DataView DV = new DataView(DecorOrdersDataTable))
                        {
                            DV.RowFilter = "PackNumber = " + Row["PackNumber"];
                            TempDataTable = DV.ToTable(true,
                                new string[] { "ProductID", "DecorID", "ColorID", "Length", "Height", "Width" });
                        }

                        int Count = TempDataTable.Rows.Count;

                        if (Count > 6)
                            return false;

                        TempDataTable.Dispose();
                    }
                }

                return true;
            }
        }

        public int[] GetPackNumbers()
        {
            int[] rows = new int[PackagesSequence.Rows.Count];

            for (int i = 0; i < PackagesSequence.Rows.Count; i++)
                rows[i] = Convert.ToInt32(PackagesSequence.Rows[i]["PackNumber"]);

            return rows;
        }

        public void ClearPackage()
        {
            if (DecorOrdersDataTable.Rows.Count == 0)
                return;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE MainOrderID = " + CurrentMainOrderID +
                " AND ProductType = 1 AND FactoryID = " + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ClearPackageDetails()
        {
            if (DecorOrdersDataTable.Rows.Count == 0)
                return;

            DataRow[] Rows = DecorOrdersDataTable.Select("MainOrderID = " + CurrentMainOrderID);
            foreach (DataRow Row in Rows)
            {
                Row["PackNumber"] = DBNull.Value;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE FactoryID = " + FactoryID + " AND ProductType = 1 AND MainOrderID = " + CurrentMainOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        private void SavePackages()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE MainOrderID = " + CurrentMainOrderID +
                " AND ProductType = 1 AND FactoryID = " + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        //if (DT.Rows.Count > 0)
                        //{
                        //    foreach (DataRow Row in DT.Rows)
                        //        Row.Delete();
                        //}

                        if (AllPackCount > 0)
                        {
                            foreach (DataRow Row in PackagesSequence.Rows)
                            {
                                DataRow[] Rows = DT.Select("PackNumber = " + Row["PackNumber"]);
                                if (Rows.Count() == 0)
                                {
                                    DataRow NewRow = DT.NewRow();
                                    NewRow["MainOrderID"] = CurrentMainOrderID;
                                    NewRow["PackNumber"] = Row["PackNumber"];
                                    NewRow["FactoryID"] = FactoryID;
                                    NewRow["ProductType"] = 1;
                                    DT.Rows.Add(NewRow);
                                }
                            }
                        }
                        for (int i = DT.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow[] Rows = PackagesSequence.Select("PackNumber = " + DT.Rows[i]["PackNumber"]);
                            if (Rows.Count() == 0)
                            {
                                DT.Rows[i].Delete();
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        private void SavePackageDetails()
        {
            if (DecorOrdersDataTable.Rows.Count == 0)
                return;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN " +
                " (SELECT PackageID FROM Packages WHERE MainOrderID = " + CurrentMainOrderID +
                " AND ProductType = 1 AND FactoryID=" + FactoryID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                                Row.Delete();
                        }

                        SavePackages();

                        if (AllPackCount > 0)
                        {
                            foreach (DataRow Row in DecorOrdersDataTable.Rows)
                            {
                                if (Row["PackNumber"] != DBNull.Value)
                                {
                                    DataRow NewRow = DT.NewRow();
                                    NewRow["PackageID"] = GetPackageID(CurrentMainOrderID, Convert.ToInt32(Row["PackNumber"]));
                                    NewRow["PackNumber"] = Row["PackNumber"];
                                    NewRow["OrderID"] = Row["DecorOrderID"];
                                    NewRow["Count"] = Row["Count"];
                                    DT.Rows.Add(NewRow);
                                }
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveOneMainOrder()
        {
            SavePackageDetails();
        }

        /// <summary>
        /// разделяет текущую позицию
        /// </summary>
        /// <param name="SS"></param>
        public void SplitCurrentItem(SplitStruct SS)
        {
            if (SS.IsEqual)
            {
                if (SS.SCount == 0)
                {
                    ((DataRowView)DecorOrdersBindingSource.Current).Row["Count"] = SS.FCount;
                    ((DataRowView)DecorOrdersBindingSource.Current).Row["PackNumber"] = SS.FirstPosition++;
                    for (int i = 0; i < SS.PackagesCount - 1; i++)
                    {
                        DataRow Row = DecorOrdersDataTable.NewRow();

                        Row.ItemArray = ((DataRowView)DecorOrdersBindingSource.Current).Row.ItemArray;

                        Row["Count"] = SS.FCount;
                        Row["PackNumber"] = SS.FirstPosition + i;

                        DecorOrdersDataTable.Rows.Add(Row);
                    }
                }
                else
                {
                    ((DataRowView)DecorOrdersBindingSource.Current).Row["Count"] = SS.FCount;
                    ((DataRowView)DecorOrdersBindingSource.Current).Row["PackNumber"] = SS.FirstPosition++;
                    for (int i = 0; i < SS.PackagesCount - 1; i++)
                    {
                        DataRow Row = DecorOrdersDataTable.NewRow();

                        Row.ItemArray = ((DataRowView)DecorOrdersBindingSource.Current).Row.ItemArray;

                        Row["Count"] = SS.FCount;
                        Row["PackNumber"] = SS.FirstPosition + i;

                        DecorOrdersDataTable.Rows.Add(Row);
                    }

                    DataRow Row1 = DecorOrdersDataTable.NewRow();

                    Row1.ItemArray = ((DataRowView)DecorOrdersBindingSource.Current).Row.ItemArray;

                    Row1["Count"] = SS.SCount;
                    Row1["PackNumber"] = SS.FirstPosition + SS.PackagesCount - 1;

                    DecorOrdersDataTable.Rows.Add(Row1);
                }
            }
            else
            {
                DataRow Row = DecorOrdersDataTable.NewRow();

                ((DataRowView)DecorOrdersBindingSource.Current).Row["Count"] = SS.FCount;

                Row.ItemArray = ((DataRowView)DecorOrdersBindingSource.Current).Row.ItemArray;

                Row["Count"] = SS.SCount;
                Row["PackNumber"] = DBNull.Value;

                DecorOrdersDataTable.Rows.Add(Row);
            }
            DecorOrdersBindingSource.Sort = "DecorOrderID";

        }

        public int OrdersCount
        {
            get { return DecorOrdersDataTable.Rows.Count; }
        }

        public int PackageAllocStatus
        {
            get
            {
                if (AllPackCount == 0)
                    return 0;
                if (AllPackCount == DecorOrdersDataTable.Rows.Count)
                    return 2;
                return 1;
            }
        }

        /// <summary>
        /// Возвращает true, если подзаказ полностью упакован
        /// </summary>
        public bool IsPacked
        {
            get
            {
                return AllPackCount == DecorOrdersDataTable.Rows.Count;
            }
        }

        public void DebtPack(int PackNumber, int MainOrderID)
        {
            DataTable PackagesDT = new DataTable();
            SqlDataAdapter PackagesDA = new SqlDataAdapter("SELECT TOP 0 * FROM Packages",
                ConnectionStrings.ZOVOrdersConnectionString);
            SqlCommandBuilder PackagesCB = new SqlCommandBuilder(PackagesDA);
            PackagesDA.Fill(PackagesDT);

            DataTable PackageDetailsDT = new DataTable();
            SqlDataAdapter PackageDetailsDA = new SqlDataAdapter("SELECT TOP 0 * FROM PackageDetails",
                ConnectionStrings.ZOVOrdersConnectionString);
            SqlCommandBuilder PackageDetailsCB = new SqlCommandBuilder(PackageDetailsDA);
            PackageDetailsDA.Fill(PackageDetailsDT);

//            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber,
//            DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, 
//            DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count, PackageDetails.Count
//            FROM PackageDetails 
//            INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
//            WHERE PackageDetails.PackageID IN
//            (SELECT PackageID FROM Packages WHERE ProductType = 1 AND MainOrderID IN
//            (SELECT MainOrderID FROM MainOrders WHERE DocNumber = '" + DebtDocNumber + @"'))
//            ORDER BY PackageDetails.PackNumber, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width",
//            ConnectionStrings.ZOVOrdersConnectionString))

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.PackNumber,
            DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID, 
             DecorOrders.PatinaID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count, PackageDetails.Count
            FROM PackageDetails 
            INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
            WHERE PackageDetails.PackageID IN
            (SELECT PackageID FROM Packages WHERE ProductType = 1 AND MainOrderID = " + MainOrderID + @")
            ORDER BY PackageDetails.PackNumber, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) < 1)
                        return;

                    DT.Columns.Add(new DataColumn("Ready", Type.GetType("System.Boolean")));
                    
                    foreach (DataRow Row in DT.Rows)
                    {
                        //if (Convert.ToInt32(Row["ColorID"]) == -1)
                        //    Row["ColorID"] = 0;
                        Row["Ready"] = false;
                    }

                    int ProductID = 0;
                    int DecorID = 0;
                    int ColorID = 0;

                    int Height = 0;
                    int Length = 0;
                    int Width = 0;

                    int OldPackNumber = 1;

                    foreach (DataRow Row in DT.Select("Ready = false"))
                    {
                        ProductID = Convert.ToInt32(Row["ProductID"]);
                        DecorID = Convert.ToInt32(Row["DecorID"]);
                        ColorID = Convert.ToInt32(Row["ColorID"]);

                        Height = Convert.ToInt32(Row["Height"]);
                        Length = Convert.ToInt32(Row["Length"]);
                        Width = Convert.ToInt32(Row["Width"]);

                        DataRow[] PRows = DecorOrdersDataTable.Select("ProductID = " + ProductID + " AND DecorID = " + DecorID + " AND ColorID = " + ColorID +
                            " AND Height = " + Height + " AND Length = " + Length + " AND Width = " + Width + " AND PackNumber IS NULL");

                        if (PRows.Count() < 1)
                        {
                            Row["Ready"] = true;
                            continue;
                        }

                        if (OldPackNumber != Convert.ToInt32(Row["PackNumber"]))
                            PackNumber++;
                        OldPackNumber = Convert.ToInt32(Row["PackNumber"]);

                        PRows[0]["PackNumber"] = PackNumber;
                        DataRow NewRow = PackageDetailsDT.NewRow();
                        //NewRow["PackageID"] = GetPackageID(CurrentMainOrderID, Convert.ToInt32(Row["PackNumber"]));
                        NewRow["PackNumber"] = PackNumber;
                        NewRow["OrderID"] = PRows[0]["DecorOrderID"];
                        NewRow["Count"] = Row["Count"];
                        PackageDetailsDT.Rows.Add(NewRow);

                        Row["Ready"] = true;
                    }
                }
            }
            PackageDetailsDT.Dispose();
        }
    }







    public class ZOVPackagesAllocManager
    {
        public int CurrentMainOrderID = -1;
        public int CurrentMegaOrderID = -1;

        private int FactoryID = 1;
        private string PackAllocStatusID = "ProfilPackAllocStatusID";
        private string PackCount = "ProfilPackCount";

        public ZOVPackagesAllocFrontsOrders PackagesMainOrdersFrontsOrders = null;
        public ZOVPackagesAllocDecorOrders PackagesMainOrdersDecorOrders = null;

        public PercentageDataGrid MainOrdersDataGrid = null;
        public PercentageDataGrid MegaOrdersDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        public DataTable ClientsDataTable = null;
        public DataTable MainOrdersDataTable = null;
        private DataTable DocNumbersDataTable = null;
        private DataTable FactoryTypesDataTable = null;
        private DataTable PackAllocStatusesDataTable = null;
        private DataTable PackageStatusesDataTable = null;
        private DataTable SquareMainOrdersDataTable = null;
        private DataTable WeightMainFrontsOrdersDataTable = null;
        private DataTable WeightMainDecorOrdersDataTable = null;
        private DataTable SquareMegaOrdersDataTable = null;
        private DataTable WeightMegaFrontsOrdersDataTable = null;
        private DataTable WeightMegaDecorOrdersDataTable = null;
        public DataTable MegaOrdersDataTable = null;

        private BindingSource ClientsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;
        public BindingSource DocNumbersBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource PackAllocStatusesBindingSource = null;
        public BindingSource MegaOrdersBindingSource = null;

        private SqlDataAdapter MainOrdersDataAdapter = null;
        private SqlCommandBuilder MainOrdersSqlCommandBuilder = null;

        private SqlDataAdapter MegaOrdersDataAdapter = null;
        private SqlCommandBuilder MegaOrdersSqlCommandBuilder = null;

        private DataGridViewComboBoxColumn ClientsColumn = null;
        //private DataGridViewComboBoxColumn OrderStatusColumn = null;
        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn PackAllocStatusColumn = null;

        public ZOVPackagesAllocManager(ref PercentageDataGrid tMainOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMegaOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl,
            int iFactory)
        {
            MainOrdersDataGrid = tMainOrdersDataGrid;
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            OrdersTabControl = tOrdersTabControl;
            FactoryID = iFactory;

            PackagesMainOrdersFrontsOrders = new ZOVPackagesAllocFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid)
            {
                Factory = FactoryID
            };
            PackagesMainOrdersDecorOrders = new ZOVPackagesAllocDecorOrders(ref tMainOrdersFrontsOrdersDataGrid, ref tMainOrdersDecorOrdersDataGrid)
            {
                Factory = FactoryID
            };
            if (FactoryID == 2)
            {
                PackAllocStatusID = "TPSPackAllocStatusID";
                PackCount = "TPSPackCount";
            }

            Initialize();
        }

        private void Create()
        {
            MainOrdersDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            PackageStatusesDataTable = new DataTable();
            DocNumbersDataTable = new DataTable();
            FactoryTypesDataTable = new DataTable();
            PackAllocStatusesDataTable = new DataTable();

            SquareMegaOrdersDataTable = new DataTable();
            WeightMegaFrontsOrdersDataTable = new DataTable();
            WeightMegaDecorOrdersDataTable = new DataTable();
            SquareMainOrdersDataTable = new DataTable();
            WeightMainFrontsOrdersDataTable = new DataTable();
            WeightMainDecorOrdersDataTable = new DataTable();

            ClientsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();
            DocNumbersBindingSource = new BindingSource();
            FactoryTypesBindingSource = new BindingSource();
            PackAllocStatusesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders", ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(MainOrdersDataTable);
            //}
            MainOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 MainOrderID, MegaOrderID, MainOrders.ClientID, DocNumber, DebtDocNumber, DebtTypeID, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID, " + PackAllocStatusID + ", " + PackCount + ", " +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID", ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersSqlCommandBuilder = new SqlCommandBuilder(MainOrdersDataAdapter);
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);

            DateTime CurrentDate = GetCurrentDate.AddDays(-8);
            string DispatchDate = CurrentDate.ToString("yyyy-MM-dd");

            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
            //    " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
            //    " WHERE (MegaOrderID = 0) OR (DispatchDate >='" + DispatchDate + "' AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
            //    " WHERE (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
            //    " AND ((MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
            //    " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + "))" +
            //    " AND MainOrderID NOT IN (SELECT MainOrderID FROM Packages WHERE PackageStatusID = 1 AND FactoryID = " + FactoryID + "))))" +
            //    " ORDER BY DispatchDate", ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(MegaOrdersDataTable);
            //}

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Square) As Square, MainOrderID FROM FrontsOrders WHERE FactoryID = " + FactoryID +
                " GROUP BY MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SquareMainOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight, MainOrderID FROM FrontsOrders WHERE FactoryID = " + FactoryID +
                " GROUP BY MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMainFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight, MainOrderID FROM DecorOrders WHERE FactoryID = " + FactoryID +
                " GROUP BY MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMainDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(FrontsOrders.Square) As Square, MegaOrders.MegaOrderID" +
                " FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID + " GROUP BY MegaOrders.MegaOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SquareMegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(FrontsOrders.Weight) As Weight, MegaOrders.MegaOrderID" +
                " FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID + " GROUP BY MegaOrders.MegaOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMegaFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(DecorOrders.Weight) As Weight, MegaOrders.MegaOrderID" +
                " FROM DecorOrders" +
                " INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID + " GROUP BY MegaOrders.MegaOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMegaDecorOrdersDataTable);
            }

            MegaOrdersDataAdapter = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate, Weight, Square, " + PackAllocStatusID + ", " + PackCount + "," +
                " infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE (MegaOrderID = 0) OR (DispatchDate >='" + DispatchDate + "' AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                " AND ((MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + "))" +
                " AND MainOrderID NOT IN (SELECT MainOrderID FROM Packages WHERE FactoryID = " + FactoryID + "))))" +
                " ORDER BY DispatchDate", ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersSqlCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                MegaOrdersDataTable.Rows[i]["Square"] = GetSquareMegaOrder(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
                MegaOrdersDataTable.Rows[i]["Weight"] = GetWeightMegaOrder(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
            }

            FactoryTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }
            PackAllocStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackAllocStatuses",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackAllocStatusesDataTable);
            }
            PackageStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }
            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DocNumber) FROM MainOrders" +
                " WHERE (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + "))" +
                //" OR MainOrderID IN (SELECT MainOrderID FROM Packages WHERE FactoryID = " + FactoryID + "))" +
                " AND (MegaOrderID=0 OR MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE DispatchDate>='2012-12-01'))" +
                " ORDER BY DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDataTable);
            }
        }

        private void Binding()
        {
            ClientsBindingSource.DataSource = ClientsDataTable;

            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;
            PackAllocStatusesBindingSource.DataSource = PackAllocStatusesDataTable;

            DocNumbersBindingSource.DataSource = DocNumbersDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
            MainOrdersBindingSource.DataSource = MainOrdersDataTable;

            MainOrdersDataGrid.DataSource = MainOrdersBindingSource;
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;
        }

        private void CreateColumns()
        {
            ClientsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ClientsColumn",
                HeaderText = "Клиент",
                DataPropertyName = "ClientID",
                DataSource = ClientsBindingSource,
                ValueMember = "ClientID",
                DisplayMember = "ClientName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            //OrderStatusColumn = new DataGridViewComboBoxColumn();
            //OrderStatusColumn.Name = "OrderStatusColumn";
            //OrderStatusColumn.HeaderText = "Статус заказа";
            //OrderStatusColumn.DataPropertyName = "OrderStatusID";
            //OrderStatusColumn.DataSource = OrderStatusesBindingSource;
            //OrderStatusColumn.ValueMember = "OrderStatusID";
            //OrderStatusColumn.DisplayMember = "OrderStatus";
            //OrderStatusColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //OrderStatusColumn.SortMode = DataGridViewColumnSortMode.Automatic;

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
            PackAllocStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PackAllocStatusColumn",
                HeaderText = "Упаковано",
                DataPropertyName = PackAllocStatusID,
                DataSource = PackAllocStatusesBindingSource,
                ValueMember = "PackAllocStatusID",
                DisplayMember = "PackAllocStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            //MegaOrdersDataGrid.Columns.Add(OrderStatusColumn);
            MainOrdersDataGrid.Columns.Add(FactoryTypeColumn);
            MainOrdersDataGrid.Columns.Add(ClientsColumn);
            MainOrdersDataGrid.Columns.Add(PackAllocStatusColumn);
        }

        private void MainGridSetting()
        {
            foreach (DataGridViewColumn Column in MainOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //MainOrdersDataGrid.Columns["DebtDocNumber"].Visible = false;
            //MainOrdersDataGrid.Columns["ReorderDocNumber"].Visible = false;
            //MainOrdersDataGrid.Columns["FrontsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["DecorCost"].Visible = false;
            //MainOrdersDataGrid.Columns["OrderCost"].Visible = false;
            //MainOrdersDataGrid.Columns["DispatchedCost"].Visible = false;
            //MainOrdersDataGrid.Columns["DispatchedDebtCost"].Visible = false;
            //MainOrdersDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
            //MainOrdersDataGrid.Columns["CalcDebtCost"].Visible = false;
            MainOrdersDataGrid.Columns["ClientID"].Visible = false;
            MainOrdersDataGrid.Columns["MegaOrderID"].Visible = false;
            MainOrdersDataGrid.Columns["DebtTypeID"].Visible = false;

            if (MainOrdersDataGrid.Columns.Contains("ProfilProductionStatusID"))
                MainOrdersDataGrid.Columns["ProfilProductionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilStorageStatusID"))
                MainOrdersDataGrid.Columns["ProfilStorageStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilDispatchStatusID"))
                MainOrdersDataGrid.Columns["ProfilDispatchStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSProductionStatusID"))
                MainOrdersDataGrid.Columns["TPSProductionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSStorageStatusID"))
                MainOrdersDataGrid.Columns["TPSStorageStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSDispatchStatusID"))
                MainOrdersDataGrid.Columns["TPSDispatchStatusID"].Visible = false;

            //MainOrdersDataGrid.Columns["WriteOffDebtCost"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffDefectsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["TotalWriteOffCost"].Visible = false;
            //MainOrdersDataGrid.Columns["IncomeCost"].Visible = false;
            //MainOrdersDataGrid.Columns["ProfitCost"].Visible = false;
            //MainOrdersDataGrid.Columns["NeedCalculate"].Visible = false;
            //MainOrdersDataGrid.Columns["DocDateTime"].Visible = false;

            //MainOrdersDataGrid.Columns["WillPercentID"].Visible = false;
            //MainOrdersDataGrid.Columns["DebtTypeID"].Visible = false;
            //MainOrdersDataGrid.Columns["PriceTypeID"].Visible = false;
            //MainOrdersDataGrid.Columns["IsPrepared"].Visible = false;
            //MainOrdersDataGrid.Columns["FactoryID"].Visible = false;
            //MainOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;

            //MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            //MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;

            //if (FactoryID == 1)
            //{
            //    MainOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            //}
            //if (FactoryID == 2)
            //{
            //    MainOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            //}

            MainOrdersDataGrid.Columns["MainOrderID"].HeaderText = "№ п\\п";
            MainOrdersDataGrid.Columns["DocNumber"].HeaderText = "№ документа";
            MainOrdersDataGrid.Columns["IsSample"].HeaderText = "Образец";
            MainOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            MainOrdersDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            MainOrdersDataGrid.Columns["DocDateTime"].HeaderText = "Дата создания";
            MainOrdersDataGrid.Columns["AllocPackDateTime"].HeaderText = "Дата\r\nраспределения";
            MainOrdersDataGrid.Columns["DebtDocNumber"].HeaderText = "№ документа\r\nдолга";
            MainOrdersDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            MainOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MainOrdersDataGrid.Columns[PackCount].HeaderText = "Кол-во\r\nупаковок";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["MainOrderID"].Width = 65;
            MainOrdersDataGrid.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["DocDateTime"].Width = 150;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].Width = 140;
            MainOrdersDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MainOrdersDataGrid.Columns["ClientName"].MinimumWidth = 160;
            MainOrdersDataGrid.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MainOrdersDataGrid.Columns["DocNumber"].MinimumWidth = 150;
            MainOrdersDataGrid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["FrontsSquare"].Width = 120;
            MainOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["IsSample"].Width = 100;
            MainOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            MainOrdersDataGrid.Columns["Notes"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["Weight"].Width = 70;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].MinimumWidth = 125;
            MainOrdersDataGrid.Columns["PackAllocStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            MainOrdersDataGrid.Columns["PackAllocStatusColumn"].MinimumWidth = 140;
            MainOrdersDataGrid.Columns[PackCount].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns[PackCount].Width = 100;
            MainOrdersDataGrid.Columns["DebtDocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            MainOrdersDataGrid.AutoGenerateColumns = false;
            //DocDateTime
            MainOrdersDataGrid.Columns["MainOrderID"].DisplayIndex = 1;
            MainOrdersDataGrid.Columns["DocDateTime"].DisplayIndex = 2;
            MainOrdersDataGrid.Columns["ClientName"].DisplayIndex = 3;
            MainOrdersDataGrid.Columns["DocNumber"].DisplayIndex = 4;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].DisplayIndex = 5;
            MainOrdersDataGrid.Columns[PackCount].DisplayIndex = 6;
            MainOrdersDataGrid.Columns["PackAllocStatusColumn"].DisplayIndex = 7;
            MainOrdersDataGrid.Columns["FrontsSquare"].DisplayIndex = 8;
            MainOrdersDataGrid.Columns["Weight"].DisplayIndex = 9;
            MainOrdersDataGrid.Columns["IsSample"].DisplayIndex = 10;
            MainOrdersDataGrid.Columns["Notes"].DisplayIndex = 11;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].DisplayIndex = 12;
            

            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns[PackCount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void MegaGridSetting()
        {
            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //MegaOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;
            //MegaOrdersDataGrid.Columns["TotalCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["DispatchedCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["DispatchedDebtCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcDebtCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcDefectsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["WriteOffDebtCost"].Visible = false;

            //MegaOrdersDataGrid.Columns["WriteOffDefectsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["TotalWriteOffCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["TotalCalcWriteOffCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["IncomeCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["ProfitCost"].Visible = false;

            //MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            //MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;

            MegaOrdersDataGrid.Columns[PackAllocStatusID].Visible = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["DispatchDate"].HeaderText = "Дата\r\nотгрузки";
            MegaOrdersDataGrid.Columns["PackAllocStatus"].HeaderText = "Упaковано";
            MegaOrdersDataGrid.Columns[PackCount].HeaderText = "Кол-во\r\nупаковок";
            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = "Площадь\r\nфасадов, м.кв.";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["DispatchDate"].DisplayIndex = 1;
            MegaOrdersDataGrid.Columns[PackCount].DisplayIndex = 2;
            MegaOrdersDataGrid.Columns["PackAllocStatus"].DisplayIndex = 3;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = 4;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = 5;
            
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MegaOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Square"].Width = 130;
            MegaOrdersDataGrid.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["DispatchDate"].Width = 120;
            MegaOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Weight"].Width = 70;
            MegaOrdersDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaOrderID"].Width = 80;
            MegaOrdersDataGrid.Columns["PackAllocStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaOrdersDataGrid.Columns["PackAllocStatus"].MinimumWidth = 170;
            MegaOrdersDataGrid.Columns[PackCount].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns[PackCount].Width = 110;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            MainGridSetting();
            MegaGridSetting();
        }

        private decimal GetSquareMegaOrder(int MegaOrderID)
        {
            decimal Square = 0;

            DataRow[] Rows = SquareMegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
            if (Rows.Count() > 0)
                Square = Convert.ToDecimal(Rows[0]["Square"]);
            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);

            return Square;
        }

        private decimal GetSquareMainOrder(int MainOrderID)
        {
            decimal Square = 0;

            DataRow[] Rows = SquareMainOrdersDataTable.Select("MainOrderID = " + MainOrderID);

            if (Rows.Count() > 0)
                Square = Convert.ToDecimal(Rows[0]["Square"]);

            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);

            return Square;
        }

        private decimal GetWeightMegaOrder(int MegaOrderID)
        {
            decimal Weight = 0;

            DataRow[] FRows = WeightMegaFrontsOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
            if (FRows.Count() > 0)
                Weight += Convert.ToDecimal(FRows[0]["Weight"]);
            DataRow[] DRows = WeightMegaDecorOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
            if (DRows.Count() > 0)
                Weight += Convert.ToDecimal(DRows[0]["Weight"]);
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        private decimal GetWeightMainOrder(int MainOrderID)
        {
            decimal Weight = 0;

            DataRow[] FRows = WeightMainFrontsOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            if (FRows.Count() > 0)
                Weight += Convert.ToDecimal(FRows[0]["Weight"]);
            DataRow[] DRows = WeightMainDecorOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            if (DRows.Count() > 0)
                Weight += Convert.ToDecimal(DRows[0]["Weight"]);
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        public void FilterMainOrders(int MegaOrderID)
        {
            if (CurrentMegaOrderID == MegaOrderID)
                return;

            if (MegaOrdersBindingSource.Count == 0)
                return;

            MainOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, MainOrders.ClientID, DocNumber, DebtDocNumber, DebtTypeID, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID, " + PackAllocStatusID + ", " + PackCount + ", " + 
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE MegaOrderID=" + MegaOrderID +
                " AND (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                " AND ((MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + ")))" +
                " ORDER BY ClientName, DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                MainOrdersDataTable.Rows[i]["FrontsSquare"] = GetSquareMainOrder(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
                MainOrdersDataTable.Rows[i]["Weight"] = GetWeightMainOrder(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
            }

            CurrentMegaOrderID = MegaOrderID;
        }

        public void FilterMainOrdersCurrent()
        {
            int MegaOrderID = -1;
            int MainOrderID = -1;

            if (MegaOrdersBindingSource.Count == 0)
                return;

            if (MegaOrdersBindingSource.Count > 0)
                MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            if (MainOrdersBindingSource.Count > 0)
                MainOrderID = Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current).Row["MainOrderID"]);

            MainOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, DocNumber, DebtDocNumber, DebtTypeID, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID, " + PackAllocStatusID + ", " + PackCount + "," +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE MegaOrderID = " + MegaOrderID +
                " AND (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                " AND ((MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + ")))" +
                " ORDER BY ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                MainOrdersDataTable.Rows[i]["FrontsSquare"] = GetSquareMainOrder(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
                MainOrdersDataTable.Rows[i]["Weight"] = GetWeightMainOrder(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
            }

            MoveToMainOrder(MainOrderID);
            CurrentMegaOrderID = MegaOrderID;
        }

        public void Filter(int MainOrderID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return;

            CurrentMainOrderID = MainOrderID;

            OrdersTabControl.TabPages[0].PageVisible = PackagesMainOrdersFrontsOrders.Filter(MainOrderID);
            OrdersTabControl.TabPages[1].PageVisible = PackagesMainOrdersDecorOrders.Filter(MainOrderID);

            //if (OrdersTabControl.TabPages[0].PageVisible == false && OrdersTabControl.TabPages[1].PageVisible == false)
            //    OrdersTabControl.Visible = false;
            //else
            //    OrdersTabControl.Visible = true;
        }

        public void FilterMegaOrders(bool bsIsPacked, bool bsIsNonPacked)
        {
            string Filter = null;

            int MegaOrderID = CurrentMegaOrderID;
            //запакован
            if (bsIsPacked)
                Filter += PackAllocStatusID + "= 2";

            //не запакован
            if (bsIsNonPacked)
                if (Filter != null)
                    Filter += " OR (" + PackAllocStatusID + "= 0 OR " + PackAllocStatusID + "= 1)";
                else
                    Filter += PackAllocStatusID + "= 0 OR " + PackAllocStatusID + "= 1";

            if (!bsIsPacked && !bsIsNonPacked)
                Filter = PackAllocStatusID + " = -1";

            MegaOrdersDataTable.Clear();
            MegaOrdersDataAdapter.Dispose();
            MegaOrdersSqlCommandBuilder.Dispose();

            DateTime CurrentDate = GetCurrentDate.AddDays(-24);
            string DispatchDate = CurrentDate.ToString("yyyy-MM-dd");

            MegaOrdersDataAdapter = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate, Weight, Square," + PackAllocStatusID + ", " + PackCount + "," +
                " infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE (MegaOrderID = 0) OR (DispatchDate >='" + DispatchDate + "' AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE (FactoryID=0 OR FactoryID=" + FactoryID + ") AND (" + Filter +
                ") AND ((MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + ")))))" +
                " ORDER BY DispatchDate", ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersSqlCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                MegaOrdersDataTable.Rows[i]["Square"] = GetSquareMegaOrder(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
                MegaOrdersDataTable.Rows[i]["Weight"] = GetWeightMegaOrder(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DocNumber) FROM MainOrders" +
                " WHERE (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + "))" +
                //" OR MainOrderID NOT IN (SELECT MainOrderID FROM Packages WHERE FactoryID = " + FactoryID + "))" +
                " AND (MegaOrderID=0 OR MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE DispatchDate>='2012-12-01'))" +
                " ORDER BY DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DocNumbersDataTable.Clear();
                DA.Fill(DocNumbersDataTable);
            }
            MoveToMegaOrder(MegaOrderID);
        }

        public void FilterMainOrders(bool bsIsPacked, bool bsIsNonPacked)
        {
            string Filter = null;
            MainOrdersBindingSource.RemoveFilter();

            //запакован
            if (bsIsPacked)
                Filter += PackAllocStatusID + "= 2";

            //не запакован
            if (bsIsNonPacked)
                if (Filter != null)
                    Filter += " OR (" + PackAllocStatusID + "= 0 OR " + PackAllocStatusID + "= 1)";
                else
                    Filter += PackAllocStatusID + "= 0 OR " + PackAllocStatusID + "= 1";

            if (!bsIsPacked && !bsIsNonPacked)
                Filter = PackAllocStatusID + " = -1";

            CurrentMegaOrderID = -1;

            MainOrdersBindingSource.Filter = Filter;
        }

        private void MoveToMainOrder(int MainOrderID)
        {
            MainOrdersBindingSource.Position = MainOrdersBindingSource.Find("MainOrderID", MainOrderID);
        }

        public void MoveToMegaOrder(int MegaOrderID)
        {
            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        /// <summary>
        /// Возвращает true, если в подзаказе больше 12 позиций, т.е. он переполнен
        /// </summary>
        public bool IsOverflow
        {
            get
            {
                if (!PackagesMainOrdersFrontsOrders.IsOverflow || !PackagesMainOrdersDecorOrders.IsOverflow)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Возвращает true, если в подзаказе номера упаковок идут по порядку (1, 2, 3; но не 1, 4, 5)
        /// </summary>
        public bool IsConsequence
        {
            get
            {
                DataTable FrontsDT = PackagesMainOrdersFrontsOrders.PackagesSequence.Copy();
                DataTable DecorDT = PackagesMainOrdersDecorOrders.PackagesSequence.Copy();

                DataTable DT = new DataTable();
                DT.Columns.Add(new DataColumn("PackNumber", Type.GetType("System.Int32")));

                foreach (DataRow Row in FrontsDT.Rows)
                {
                    if (Row["PackNumber"].ToString().Length > 0)
                        DT.ImportRow(Row);
                }

                foreach (DataRow Row in DecorDT.Rows)
                {
                    if (Row["PackNumber"].ToString().Length > 0)
                        DT.ImportRow(Row);
                }


                using (DataView DV = new DataView(DT.Copy()))
                {
                    DT.Clear();

                    DV.Sort = "PackNumber ASC";
                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }



                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    if (Convert.ToInt32(DT.Rows[i]["PackNumber"]) != MainOrderPackCount + i + 1)
                    {
                        DT.Dispose();
                        return false;
                    }
                }

                DT.Dispose();

                return true;
            }
        }

        /// <summary>
        /// Возвращает true, если фасады и декор находятся в разных упаковках
        /// </summary>
        public bool IsDifferentPackNumbers
        {
            get
            {
                DataTable FrontsDT = PackagesMainOrdersFrontsOrders.PackagesSequence.Copy();
                DataTable DecorDT = PackagesMainOrdersDecorOrders.PackagesSequence.Copy();

                for (int i = 0; i < FrontsDT.Rows.Count; i++)
                {
                    for (int j = 0; j < DecorDT.Rows.Count; j++)
                    {
                        if (Convert.ToInt32(FrontsDT.Rows[i]["PackNumber"]) == Convert.ToInt32(DecorDT.Rows[j]["PackNumber"]))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Возвращает количество введенных упаковок
        /// </summary>
        public int GetPackCount
        {
            get
            {
                int Count = 0;
                DataTable FrontsDT = PackagesMainOrdersFrontsOrders.PackagesSequence.Copy();
                DataTable DecorDT = PackagesMainOrdersDecorOrders.PackagesSequence.Copy();

                DataTable DT = new DataTable();
                DT.Columns.Add(new DataColumn("PackNumber", Type.GetType("System.Int32")));

                foreach (DataRow Row in FrontsDT.Rows)
                {
                    if (Row["PackNumber"].ToString().Length > 0)
                        DT.ImportRow(Row);
                }

                foreach (DataRow Row in DecorDT.Rows)
                {
                    if (Row["PackNumber"].ToString().Length > 0)
                        DT.ImportRow(Row);
                }


                using (DataView DV = new DataView(DT.Copy()))
                {
                    DT.Clear();

                    DV.Sort = "PackNumber ASC";
                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }



                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    Count++;
                }

                DT.Dispose();

                return Count;
            }
        }

        /// <summary>
        /// Возвращает количество упаковок на другой фирме
        /// </summary>
        public int MainOrderPackCount
        {
            get
            {
                int FirmPackCount = 0;
                string PackCount = "TPSPackCount";
                if (FactoryID == 2)
                    PackCount = "ProfilPackCount";

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT " + PackCount + " FROM MainOrders WHERE MainOrderID = " + CurrentMainOrderID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            if (DA.Fill(DT) > 0)
                            {
                                FirmPackCount = Convert.ToInt32(DT.Rows[0][PackCount]);
                            }
                        }
                    }
                }

                return FirmPackCount;
            }
        }

        private void SetMainOrderPackStatus()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, AllocPackDateTime," + PackAllocStatusID + ", " + PackCount +
                " FROM MainOrders WHERE MainOrderID = " + CurrentMainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0][PackCount] = GetPackCount;
                            DT.Rows[0]["AllocPackDateTime"] = GetCurrentDate;

                            if (PackagesMainOrdersDecorOrders.OrdersCount > 0 && PackagesMainOrdersFrontsOrders.OrdersCount > 0)
                            {
                                if (PackagesMainOrdersFrontsOrders.PackageAllocStatus == 2 && PackagesMainOrdersDecorOrders.PackageAllocStatus == 2)
                                    DT.Rows[0][PackAllocStatusID] = 2;

                                if (PackagesMainOrdersFrontsOrders.PackageAllocStatus != 2 && PackagesMainOrdersDecorOrders.PackageAllocStatus == 2)
                                    DT.Rows[0][PackAllocStatusID] = 1;

                                if (PackagesMainOrdersFrontsOrders.PackageAllocStatus == 2 && PackagesMainOrdersDecorOrders.PackageAllocStatus != 2)
                                    DT.Rows[0][PackAllocStatusID] = 1;

                                if (PackagesMainOrdersFrontsOrders.PackageAllocStatus == 0 && PackagesMainOrdersDecorOrders.PackageAllocStatus == 0)
                                    DT.Rows[0][PackAllocStatusID] = 0;

                                if (PackagesMainOrdersFrontsOrders.PackageAllocStatus == 1 || PackagesMainOrdersDecorOrders.PackageAllocStatus == 1)
                                    DT.Rows[0][PackAllocStatusID] = 1;
                            }
                            if (PackagesMainOrdersDecorOrders.OrdersCount < 1 && PackagesMainOrdersFrontsOrders.OrdersCount > 0)
                            {
                                DT.Rows[0][PackAllocStatusID] = PackagesMainOrdersFrontsOrders.PackageAllocStatus;
                            }

                            if (PackagesMainOrdersDecorOrders.OrdersCount > 0 && PackagesMainOrdersFrontsOrders.OrdersCount < 1)
                            {
                                DT.Rows[0][PackAllocStatusID] = PackagesMainOrdersDecorOrders.PackageAllocStatus;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }

            //FilterMainOrdersCurrent();
        }

        private void SetMegaOrderPackStatus()
        {
            DataTable MainOrdersDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, " + PackAllocStatusID + ", " + PackCount +
                " FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + ") AND MegaOrderID = " + CurrentMegaOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, " + PackAllocStatusID + ", " + PackCount +
                " FROM MegaOrders WHERE MegaOrderID = " + CurrentMegaOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        int StatusID = 1;
                        int Count = 0;
                        int NotPacked = 0;
                        int AllPacked = 0;

                        foreach (DataRow PRow in MainOrdersDT.Rows)
                        {
                            if (Convert.ToInt32(PRow[PackAllocStatusID]) == 0)
                                NotPacked++;

                            if (Convert.ToInt32(PRow[PackAllocStatusID]) == 2)
                                AllPacked++;

                            Count += Convert.ToInt32(PRow[PackCount]);
                        }

                        if (NotPacked == MainOrdersDT.Rows.Count)
                            StatusID = 0;

                        if (AllPacked == MainOrdersDT.Rows.Count)
                            StatusID = 2;

                        if (Count == 0)
                            StatusID = 0;

                        DT.Rows[0][PackAllocStatusID] = StatusID;
                        DT.Rows[0][PackCount] = Count;

                        DA.Update(DT);
                    }
                }
            }
            MainOrdersDT.Dispose();
        }

        public bool IsSimpleAndCurved
        {
            get
            {
                return PackagesMainOrdersFrontsOrders.IsSimpleAndCurved;
            }
        }

        public void SetPackStatus()
        {
            SetMainOrderPackStatus();
            SetMegaOrderPackStatus();
            UpdateOrders();
        }

        public void UpdateOrders()
        {
            int CurrentMegaIndex = MegaOrdersBindingSource.Position;
            int CurrentMainIndex = MainOrdersBindingSource.Position;
            CurrentMegaOrderID = -1;

            MegaOrdersDataTable.Clear();
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);
            MegaOrdersBindingSource.Position = CurrentMegaIndex;

            //MainOrdersDataTable.Clear();
            //FilterMainOrdersCurrent();

            MainOrdersBindingSource.Position = CurrentMainIndex;
        }

        private void ClearMainOrderPackCount()
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, " + PackCount + " FROM MainOrders WHERE MainOrderID = " + CurrentMainOrderID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0][PackCount] = 0;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ClearPackage()
        {
            PackagesMainOrdersFrontsOrders.ClearPackageDetails();
            PackagesMainOrdersFrontsOrders.ClearPackage();

            PackagesMainOrdersDecorOrders.ClearPackageDetails();
            PackagesMainOrdersDecorOrders.ClearPackage();
            ClearMainOrderPackCount();
        }

        public void SavePackageDetails()
        {
            PackagesMainOrdersFrontsOrders.SaveOneMainOrder();
            PackagesMainOrdersDecorOrders.SaveOneMainOrder();
        }

        public int[] GetSelectedMainOrders()
        {
            int[] rows = new int[MainOrdersDataGrid.SelectedRows.Count];

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);
            Array.Sort(rows);

            return rows;
        }

        /// <summary>
        /// Возвращает true, если подзаказ упакован
        /// </summary>
        /// <param name="MainOrderID">ID подзаказа</param>
        /// <returns></returns>
        private bool IsMainOrderPacked(int MainOrderID)
        {
            string PackAllocStatus = "ProfilPackAllocStatusID";

            if (FactoryID == 2)
                PackAllocStatus = "TPSPackAllocStatusID";

            DataRow[] Rows = MainOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() < 0)
                return false;

            if (Convert.ToInt32(Rows[0][PackAllocStatus]) == 2)
                return true;

            return false;
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
        
        private int[] GetPackageIDs(int MainOrderID, int ProductType)
        {
            int[] PackageIDs = { 0 };

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = " + ProductType +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
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

        public void CheckPackages()
        {
            int MainOrderID = 0;
            int MegaOrderID = 0;

            int FrontsPackCount = 0;
            int DecorPackCount = 0;
            int FrontsOrdersCount = 0;
            int DecorOrdersCount = 0;
            int FrontsPackageAllocStatus = 1;
            int DecorPackageAllocStatus = 0;

            int MainOrderAllocStatusID = 1;
            int MegaOrderAllocStatusID = 1;
            int MainOrdersPackCount = 0;
            int MegaOrdersPackCount = 0;
            int MegaOrderNotPacked = 0;
            int MegaOrderAllPacked = 0;

            string PackAllocStatusID = "ProfilPackAllocStatusID";

            DateTime CurrentDate = GetCurrentDate.AddDays(-8);
            string DispatchDate = CurrentDate.ToString("yyyy-MM-dd");

            DataTable MegaOrdersDT = new DataTable();
            DataTable MainOrdersDT = new DataTable();
            DataTable FrontsOrdersDT = new DataTable();
            DataTable FrontsPackagesDT = new DataTable();
            DataTable DecorOrdersDT = new DataTable();
            DataTable DecorPackagesDT = new DataTable();
            DataTable MainOrdersPackDT = new DataTable();

            SqlDataAdapter MainOrdersDA = null;
            SqlDataAdapter MegaOrdersDA = null;
            SqlCommandBuilder MainOrdersCB = null;
            SqlCommandBuilder MegaOrdersCB = null;

            if (FactoryID == 2)
                PackAllocStatusID = "TPSPackAllocStatusID";

            try
            {
                MainOrdersDA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, " + PackAllocStatusID +
                    ", " + PackCount + " FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                    " AND (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                    " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + "))",
                    ConnectionStrings.ZOVOrdersConnectionString);
                MainOrdersCB = new SqlCommandBuilder(MainOrdersDA);

                MainOrdersDT.Clear();
                MainOrdersDA.Fill(MainOrdersDT);

                MegaOrdersDA = new SqlDataAdapter("SELECT MegaOrderID, " + PackAllocStatusID + ", " + PackCount +
                    " FROM MegaOrders WHERE (MegaOrderID = 0) OR (DispatchDate >='" + DispatchDate + "' AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                    " WHERE (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                    " AND (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FactoryID=" + FactoryID + ")" +
                    " OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE FactoryID=" + FactoryID + "))))",
                    ConnectionStrings.ZOVOrdersConnectionString);
                MegaOrdersCB = new SqlCommandBuilder(MegaOrdersDA);
                MegaOrdersDA.Fill(MegaOrdersDT);

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Sum(Count) AS FrontsCount," +
                    " Packages.MainOrderID FROM PackageDetails" +
                    " INNER JOIN Packages ON (PackageDetails.PackageID = Packages.PackageID" +
                    " AND Packages.PackageID IN (SELECT PackageID FROM Packages" +
                    " WHERE ProductType = 0 AND FactoryID = " + FactoryID + " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + "))))" +
                    " GROUP BY Packages.MainOrderID ORDER BY Packages.MainOrderID",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    FrontsPackagesDT.Clear();
                    DA.Fill(FrontsPackagesDT);
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Sum(Count) AS FrontsCount," +
                    " MainOrderID FROM FrontsOrders" +
                    " WHERE FactoryID = " + FactoryID + " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + "))" +
                    " GROUP BY MainOrderID ORDER BY MainOrderID",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    FrontsOrdersDT.Clear();
                    DA.Fill(FrontsOrdersDT);
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Sum(Count) AS DecorCount," +
                    " Packages.MainOrderID FROM PackageDetails" +
                    " INNER JOIN Packages ON (PackageDetails.PackageID = Packages.PackageID" +
                    " AND Packages.PackageID IN (SELECT PackageID FROM Packages" +
                    " WHERE ProductType = 1 AND FactoryID = " + FactoryID + " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + "))))" +
                    " GROUP BY Packages.MainOrderID ORDER BY Packages.MainOrderID",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DecorPackagesDT.Clear();
                    DA.Fill(DecorPackagesDT);
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Sum(Count) AS DecorCount," +
                    " MainOrderID FROM DecorOrders" +
                    " WHERE FactoryID = " + FactoryID + " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + "))" +
                    " GROUP BY MainOrderID ORDER BY MainOrderID",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DecorOrdersDT.Clear();
                    DA.Fill(DecorOrdersDT);
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Packages.MainOrderID, COUNT(DISTINCT PackageDetails.PackNumber) AS Count FROM PackageDetails" +
                    " INNER JOIN Packages ON (PackageDetails.PackageID = Packages.PackageID" +
                    " AND Packages.PackageID IN" +
                    " (SELECT PackageID FROM Packages" +
                    " WHERE FactoryID=" + FactoryID + " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM MainOrders WHERE (FactoryID=0 OR FactoryID=" + FactoryID + "))))" +
                    " GROUP BY Packages.MainOrderID" +
                    " ORDER BY Packages.MainOrderID",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(MainOrdersPackDT);
                }

                foreach (DataRow MegaRow in MegaOrdersDT.Rows)
                {
                    MegaOrdersPackCount = 0;

                    MegaOrderID = Convert.ToInt32(MegaRow["MegaOrderID"]);

                    MegaOrderAllocStatusID = 1;//статус упаковки заказа: 0 - не запакован; 1 - частично; 2 - запакован

                    MegaOrderNotPacked = 0;
                    MegaOrderAllPacked = 0;

                    DataRow[] MainRows = MainOrdersDT.Select("MegaOrderID = " + MegaOrderID);

                    foreach (DataRow MainRow in MainRows)
                    {
                        FrontsPackCount = 0;//кол-во запакованных позиций
                        DecorPackCount = 0;
                        FrontsOrdersCount = 0;//общее кол-во позиций
                        DecorOrdersCount = 0;
                        MainOrdersPackCount = 0;//кол-во запакованных позиций
                        MainOrderAllocStatusID = 1;//статус упаковки заказа: 0 - не запакован; 1 - частично; 2 - запакован
                        FrontsPackageAllocStatus = 1;//статус упаковки подзаказа: 0 - не запакован; 2 - запакован
                        DecorPackageAllocStatus = 1;

                        MainOrderID = Convert.ToInt32(MainRow["MainOrderID"]);

                        DataRow[] FPRows = FrontsPackagesDT.Select("MainOrderID = " + MainOrderID);

                        if (FPRows.Count() > 0)
                            FrontsPackCount = Convert.ToInt32(FPRows[0]["FrontsCount"]);

                        DataRow[] FORows = FrontsOrdersDT.Select("MainOrderID = " + MainOrderID);

                        if (FORows.Count() > 0)
                            FrontsOrdersCount = Convert.ToInt32(FORows[0]["FrontsCount"]);

                        if (FrontsPackCount == 0)
                            FrontsPackageAllocStatus = 0;
                        if (FrontsPackCount == FrontsOrdersCount)
                            FrontsPackageAllocStatus = 2;

                        if (FrontsPackCount == 0)
                            FrontsPackageAllocStatus = 0;
                        else
                        {
                            if (FrontsPackCount == FrontsOrdersCount)
                                FrontsPackageAllocStatus = 2;
                            else
                                FrontsPackageAllocStatus = 1;
                        }

                        DataRow[] DPRows = DecorPackagesDT.Select("MainOrderID = " + MainOrderID);

                        if (DPRows.Count() > 0)
                            DecorPackCount = Convert.ToInt32(DPRows[0]["DecorCount"]);

                        DataRow[] DORows = DecorOrdersDT.Select("MainOrderID = " + MainOrderID);

                        if (DORows.Count() > 0)
                            DecorOrdersCount = Convert.ToInt32(DORows[0]["DecorCount"]);

                        if (DecorPackCount == 0)
                            DecorPackageAllocStatus = 0;
                        else
                        {
                            if (DecorPackCount == DecorOrdersCount)
                                DecorPackageAllocStatus = 2;
                            else
                                DecorPackageAllocStatus = 1;
                        }

                        // фасады	декор	общий
                        // 0	        0	    0

                        // 0	        1	    1
                        // 1	        0	    1
                        // 1	        1	    1
                        // 1	        2	    1
                        // 2	        1	    1

                        // 0	        2	    2
                        // 2	        0	    2
                        // 2	        2	    2

                        if (FrontsPackageAllocStatus == 2)
                        {
                            if (DecorOrdersCount > 0)
                            {
                                if (DecorPackageAllocStatus == 0 || DecorPackageAllocStatus == 1)
                                    MainOrderAllocStatusID = 1;
                                else
                                    MainOrderAllocStatusID = 2;
                            }
                            else
                                MainOrderAllocStatusID = 2;
                        }

                        if (DecorPackageAllocStatus == 2)
                        {
                            if (FrontsOrdersCount > 0)
                            {
                                if (FrontsPackageAllocStatus == 0 || FrontsPackageAllocStatus == 1)
                                    MainOrderAllocStatusID = 1;
                                else
                                    MainOrderAllocStatusID = 2;
                            }
                            else
                                MainOrderAllocStatusID = 2;
                        }

                        if (FrontsPackageAllocStatus == 0 && DecorPackageAllocStatus == 0)
                            MainOrderAllocStatusID = 0;

                        if (FrontsPackageAllocStatus == 1 || DecorPackageAllocStatus == 1)
                            MainOrderAllocStatusID = 1;

                        DataRow[] MainPRows = MainOrdersPackDT.Select("MainOrderID = " + MainOrderID);
                        if (MainPRows.Count() > 0)
                            MainOrdersPackCount = Convert.ToInt32(MainPRows[0]["Count"]);

                        MainRow[PackAllocStatusID] = MainOrderAllocStatusID;
                        MainRow[PackCount] = MainOrdersPackCount;

                        if (MainOrderAllocStatusID == 0)
                            MegaOrderNotPacked++;

                        if (MainOrderAllocStatusID == 2)
                            MegaOrderAllPacked++;

                        MegaOrdersPackCount += MainOrdersPackCount;
                    }

                    if (MegaOrderNotPacked == MainRows.Count())
                        MegaOrderAllocStatusID = 0;

                    if (MegaOrderAllPacked == MainRows.Count())
                        MegaOrderAllocStatusID = 2;

                    if (MegaOrdersPackCount == 0)
                        MegaOrderAllocStatusID = 0;

                    MegaRow[PackAllocStatusID] = MegaOrderAllocStatusID;
                    MegaRow[PackCount] = MegaOrdersPackCount;
                }

                MainOrdersDA.Update(MainOrdersDT);

                DataTable DT = MegaOrdersDT.GetChanges();
                MegaOrdersDA.Update(MegaOrdersDT);

            }
            catch
            {

            }

            MegaOrdersDT.Dispose();
            MainOrdersDT.Dispose();
            FrontsOrdersDT.Dispose();
            FrontsPackagesDT.Dispose();
            DecorOrdersDT.Dispose();
            DecorPackagesDT.Dispose();
            MainOrdersPackDT.Dispose();

            MainOrdersDA.Dispose();
            MegaOrdersDA.Dispose();
            MainOrdersCB.Dispose();
            MegaOrdersCB.Dispose();
        }

        public void MovePackageID()
        {
            DataTable PackagesDT = new DataTable();
            DataTable DetailsDT = new DataTable();

            using (SqlDataAdapter PackagesDA = new SqlDataAdapter("SELECT * FROM Packages WHERE FactoryID = 1", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder PackagesCB = new SqlCommandBuilder(PackagesDA))
                {
                    PackagesDA.Fill(PackagesDT);

                    using (SqlDataAdapter DetailsDA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE FactoryID = 1", ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        using (SqlCommandBuilder DetailsCB = new SqlCommandBuilder(DetailsDA))
                        {
                            DetailsDA.Fill(DetailsDT);

                            foreach (DataRow PRow in PackagesDT.Rows)
                            {
                                DataRow[] DRows = DetailsDT.Select("MainOrderID = " + PRow["MainOrderID"] + "AND PackNumber = " + PRow["PackNumber"] + " AND FactoryID = 1");

                                foreach (DataRow DRow in DRows)
                                {
                                    DRow["PackageID"] = Convert.ToInt32(PRow["PackageID"]);
                                }
                            }

                            DetailsDA.Update(DetailsDT);
                        }
                    }
                }
            }
        }

        public void MoveProductType()
        {
            DataTable PackagesDT = new DataTable();
            DataTable DetailsDT = new DataTable();

            using (SqlDataAdapter DetailsDA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE FactoryID = 1", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder DetailsCB = new SqlCommandBuilder(DetailsDA))
                {
                    DetailsDA.Fill(DetailsDT);

                    using (SqlDataAdapter PackagesDA = new SqlDataAdapter("SELECT * FROM Packages WHERE FactoryID = 1", ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        using (SqlCommandBuilder PackagesCB = new SqlCommandBuilder(PackagesDA))
                        {
                            PackagesDA.Fill(PackagesDT);

                            foreach (DataRow DRow in DetailsDT.Rows)
                            {
                                DataRow[] PRows = PackagesDT.Select("MainOrderID = " + DRow["MainOrderID"] + " AND PackNumber = " + DRow["PackNumber"] + " AND FactoryID =1");

                                foreach (DataRow PRow in PRows)
                                {
                                    PRow["ProductType"] = Convert.ToInt32(DRow["ProductType"]);
                                }
                            }

                            PackagesDA.Update(PackagesDT);
                        }
                    }
                }
            }
        }

        public void FindDocNumber(string DocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate, Weight, Square," + PackAllocStatusID + ", " + PackCount + "," +
                " infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE MegaOrderID = (SELECT MegaOrderID FROM MainOrders WHERE DocNumber = '" + DocNumber + "')",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                MegaOrdersDataTable.Clear();
                DA.Fill(MegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, MainOrders.ClientID, DocNumber, DebtDocNumber, DebtTypeID, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID," + PackAllocStatusID + ", " + PackCount + ", " +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                MainOrdersDataTable.Clear();
                DA.Fill(MainOrdersDataTable);
            }
        }

        public bool IsDebt
        {
            get
            {
                if (((DataRowView)MainOrdersBindingSource.Current != null
                    && ((DataRowView)MainOrdersBindingSource.Current).Row["DebtTypeID"] != DBNull.Value
                    && Convert.ToInt32(((DataRowView)MainOrdersBindingSource.Current).Row["DebtTypeID"]) == 1))
                    return true;
                return false;
            }
        }

        public string DebtDocNumber
        {
            get
            {
                if (((DataRowView)MainOrdersBindingSource.Current != null
                    && ((DataRowView)MainOrdersBindingSource.Current).Row["DebtDocNumber"] != DBNull.Value))
                    return ((DataRowView)MainOrdersBindingSource.Current).Row["DebtDocNumber"].ToString();
                return string.Empty;
            }
        }

        public int DebtMainOrder(string DebtDocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE DocNumber = '" + DebtDocNumber + "'",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) < 1)
                        return 0;

                    return Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                }
            }
        }

        public bool IsDebtExist(string DebtDocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE DocNumber = '" + DebtDocNumber + "'",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DT.Rows.Count > 0;
                }
            }
        }

        public void FixOrderEvent(int MainOrderID, string Event)
        {
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT Packages.*, PackageDetails.OrderID FROM PackageDetails
                INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM PackagesEvents";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (TempDT.Rows.Count > 0)
                        {
                            for (int i = 0; i < TempDT.Rows.Count; i++)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                NewRow["Event"] = Event;
                                NewRow["EventDate"] = Security.GetCurrentDate();
                                NewRow["EventUserID"] = Security.CurrentUserID;
                                DT.Rows.Add(NewRow);
                            }
                            DA.Update(DT);
                        }
                        else
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow["MainOrderID"] = MainOrderID;
                            NewRow["Event"] = "Заказа не существует";
                            NewRow["EventDate"] = Security.GetCurrentDate();
                            NewRow["EventUserID"] = Security.CurrentUserID;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

    }
    #endregion





    #region Печать
    public class ZOVPackagesPrintFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrderID = -1;
        int FactoryID = 1;
        int PackNumber = 1;

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

        public ZOVPackagesPrintFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;

            Initialize();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders WHERE FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
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
            FrontsOrdersDataGrid.Columns.Add(PatinaColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(FrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(InsetColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoProfilesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoFrameColorsColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetTypesColumn);
            FrontsOrdersDataGrid.Columns.Add(TechnoInsetColorsColumn);

            FrontsOrdersDataGrid.RowPostPaint += new DataGridViewRowPostPaintEventHandler(MainOrdersFrontsOrdersDataGrid_RowPostPaint);

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
            if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
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
            FrontsOrdersDataGrid.Columns["Debt"].Visible = false;
            FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Visible = false;
            FrontsOrdersDataGrid.Columns["PackNumber"].Visible = false;

            if (FrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                FrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                FrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;

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
            FrontsOrdersDataGrid.Columns["CupboardString"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Height"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Width"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Count"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Square"].DisplayIndex = DisplayIndex++;
            FrontsOrdersDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

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
            //MainOrdersFrontsOrdersDataGrid.Columns["PackNumber"].HeaderText = "  Номер\r\nупаковки";
            FrontsOrdersDataGrid.Columns["CupboardString"].HeaderText = "Шкаф";

            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Height"].Width = 85;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Width"].Width = 85;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Count"].Width = 65;
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Square"].Width = 100;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 105;
            FrontsOrdersDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["PackNumber"].Width = 65;
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

        private int GetPackageID(int MainOrderID)
        {
            int PackageID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0" +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                    }
                }
            }

            return PackageID;
        }

        //private int[] GetPackageIDs(int MainOrderID)
        //{
        //    int[] PackageIDs = { 0 };

        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
        //        " AND ProductType = 0" +
        //        " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
        //    {
        //        using (DataTable DT = new DataTable())
        //        {
        //            if (DA.Fill(DT) > 0)
        //            {
        //                PackageIDs = new int[DT.Rows.Count];
        //                for (int i = 0; i < DT.Rows.Count; i++)
        //                {
        //                    PackageIDs[i] = Convert.ToInt32(DT.Rows[i]["PackageID"]);
        //                }
        //            }
        //        }
        //    }

        //    return PackageIDs;
        //}

        public bool Filter(int MainOrderID, ArrayList FrontIDs)
        {
            if (CurrentMainOrderID == MainOrderID)
                return FrontsOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            if (FrontIDs.Count < 1)
                return false;

            int[] IDs = FrontIDs.OfType<int>().ToArray();

            string SelectionCommand = "SELECT * FROM FrontsOrders WHERE FrontID IN (" + string.Join(",", IDs) + ")" +
                    " AND MainOrderID = " + MainOrderID + " AND FactoryID=" + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            OriginalFrontsOrdersDataTable = FrontsOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0" +
                " AND FactoryID=" + FactoryID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(FrontsOrdersDataTable);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public void SetColor(int PackNum)
        {
            for (int i = 0; i < FrontsOrdersDataGrid.Rows.Count; i++)
            {
                int FrontOrderPackNumber = Convert.ToInt32(FrontsOrdersDataGrid.Rows[i].Cells["PackNumber"].Value);
                PackNumber = PackNum;

                if (FrontOrderPackNumber == PackNumber)
                {
                    FrontsOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(31, 158, 0);
                    FrontsOrdersDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    FrontsOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Security.GridsBackColor;
                    FrontsOrdersDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        public void MoveToFrontOrder(int PackNumber)
        {
            DataRow[] Rows = FrontsOrdersDataTable.Select("PackNumber = " + PackNumber);
            if (Rows.Count() < 1)
                return;

            int FrontsOrdersID = Convert.ToInt32(Rows[0]["FrontsOrdersID"]);
            FrontsOrdersDataTable.DefaultView.Sort = "PackNumber, FrontsOrdersID";
            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                DV.Sort = "PackNumber, FrontsOrdersID";
                object[] obj = new object[] { PackNumber, FrontsOrdersID };

                FrontsOrdersDataGrid.FirstDisplayedScrollingRowIndex = DV.Find(obj);
            }
        }

        private void MainOrdersFrontsOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value == DBNull.Value)
                return;

            if (Convert.ToInt32(FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) == PackNumber)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = FrontsOrdersDataGrid.RowHeadersVisible ?
                                     FrontsOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    FrontsOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            FrontsOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                FrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Color.FromArgb(31, 158, 0);
                FrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.White;
            }
            if (Convert.ToInt32(FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) != PackNumber)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = FrontsOrdersDataGrid.RowHeadersVisible ?
                                     FrontsOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    FrontsOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            FrontsOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                FrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Security.GridsBackColor;
                FrontsOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.Black;
            }
        }
    }







    public class ZOVPackagesPrintDecorOrders
    {
        int CurrentMainOrderID = -1;
        int FactoryID = 1;
        int PackNumber = 1;

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
        public ZOVPackagesPrintDecorOrders(ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid)
        {
            MainOrdersDecorOrdersDataGrid = tMainOrdersDecorOrdersDataGrid;
            
            Initialize();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
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
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.ZOVOrdersConnectionString);
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

            MainOrdersDecorOrdersDataGrid.RowPostPaint += new DataGridViewRowPostPaintEventHandler(MainOrdersDecorOrdersDataGrid_RowPostPaint);

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
            MainOrdersDecorOrdersDataGrid.Columns["Debt"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Weight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].Visible = false;

            //русские названия полей

            MainOrdersDecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            MainOrdersDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            //MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].HeaderText = "  Номер\r\nупаковки";

            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].MinimumWidth = 110;

            //MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].DisplayIndex = 10;

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

            foreach (DataGridViewColumn Column in MainOrdersDecorOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private int GetPackageID(int MainOrderID)
        {
            int PackageID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" +
                " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                    }
                }
            }

            return PackageID;
        }

        //private int[] GetPackageIDs(int MainOrderID)
        //{
        //    int[] PackageIDs = { 0 };

        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
        //        " AND ProductType = 1" +
        //        " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
        //    {
        //        using (DataTable DT = new DataTable())
        //        {
        //            if (DA.Fill(DT) > 0)
        //            {
        //                PackageIDs = new int[DT.Rows.Count];
        //                for (int i = 0; i < DT.Rows.Count; i++)
        //                {
        //                    PackageIDs[i] = Convert.ToInt32(DT.Rows[i]["PackageID"]);
        //                }
        //            }
        //        }
        //    }

        //    return PackageIDs;
        //}

        public bool Filter(int MainOrderID, ArrayList DecorIDs)
        {
            if (CurrentMainOrderID == MainOrderID)
                return DecorOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            //CurrentMainOrderID = MainOrderID;
            if (DecorIDs.Count < 1)
                return false;

            int[] IDs = DecorIDs.OfType<int>().ToArray();

            string SelectionCommand = "SELECT * FROM DecorOrders WHERE ProductID IN (" + string.Join(",", IDs) + ")" +
                    " AND MainOrderID = " + MainOrderID + " AND FactoryID=" + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = DecorOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1" +
                " AND FactoryID=" + FactoryID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID = " + MainOrderID +
                            " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(DecorOrdersDataTable);

                            if (DecorOrdersDataTable.Rows.Count > 0)
                            {
                                foreach (DataRow Row in DecorOrdersDataTable.Rows)
                                {
                                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                                    //    Row["ColorID"] = 0;
                                }
                            }
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        public void SetColor(int packNumber)
        {
            for (int i = 0; i < MainOrdersDecorOrdersDataGrid.Rows.Count; i++)
            {
                int DecorOrderPackNumber = Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[i].Cells["PackNumber"].Value);
                PackNumber = packNumber;

                if (DecorOrderPackNumber == PackNumber)
                {
                    MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(31, 158, 0);
                    MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Security.GridsBackColor;
                    MainOrdersDecorOrdersDataGrid.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        public void MoveToDecorOrder(int PackNumber)
        {
            DataRow[] Rows = DecorOrdersDataTable.Select("PackNumber = " + PackNumber);
            if (Rows.Count() < 1)
                return;

            int DecorOrderID = Convert.ToInt32(Rows[0]["DecorOrderID"]);
            DecorOrdersDataTable.DefaultView.Sort = "PackNumber, DecorOrderID";
            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                DV.Sort = "PackNumber, DecorOrderID";
                object[] obj = new object[] { PackNumber, DecorOrderID };

                MainOrdersDecorOrdersDataGrid.FirstDisplayedScrollingRowIndex = DV.Find(obj);
            }
        }

        private void MainOrdersDecorOrdersDataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value == DBNull.Value)
                return;

            if (Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) == PackNumber)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = MainOrdersDecorOrdersDataGrid.RowHeadersVisible ?
                                     MainOrdersDecorOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    MainOrdersDecorOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            MainOrdersDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Color.FromArgb(31, 158, 0);
                MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.White;
            }
            if (Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) != PackNumber)
            {
                // Calculate the bounds of the row 
                int rowHeaderWidth = MainOrdersDecorOrdersDataGrid.RowHeadersVisible ?
                                     MainOrdersDecorOrdersDataGrid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(
                    rowHeaderWidth,
                    e.RowBounds.Top,
                    MainOrdersDecorOrdersDataGrid.Columns.GetColumnsWidth(
                            DataGridViewElementStates.Visible) -
                            MainOrdersDecorOrdersDataGrid.HorizontalScrollingOffset + 1,
                   e.RowBounds.Height);

                MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                                                     Security.GridsBackColor;
                MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor =
                                                     Color.Black;
            }
        }
    }







    public class ZOVPackagesPrintManager
    {
        private int CurrentMegaOrderID = -1;

        private int FactoryID = 1;

        private string PackAllocStatusID = "ProfilPackAllocStatusID";
        private string PackCount = "ProfilPackCount";
        string ProductionDate = "ProfilProductionDate";
        string ProductionStatus = "ProfilProductionStatusID";
        string StorageStatus = "ProfilStorageStatusID";
        string DispatchStatus = "ProfilDispatchStatusID";

        public ZOVPackagesPrintFrontsOrders PackedMainOrdersFrontsOrders = null;
        public ZOVPackagesPrintDecorOrders PackedMainOrdersDecorOrders = null;

        public PercentageDataGrid MainOrdersDataGrid = null;
        public PercentageDataGrid MegaOrdersDataGrid = null;
        public PercentageDataGrid PackagesDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        private DataTable ClientsDataTable = null;
        public DataTable MainOrdersDataTable = null;
        private DataTable PackAllocStatusesDataTable = null;
        public DataTable PackagesDataTable = null;
        private DataTable PackageStatusesDataTable = null;

        private DataTable DocNumbersDataTable = null;
        public DataTable MegaOrdersDataTable = null;
        private DataTable FactoryTypesDataTable = null;
        private DataTable ProductionStatusesDataTable = null;
        private DataTable StorageStatusesDataTable = null;
        private DataTable DispatchStatusesDataTable = null;
        private DataTable SquareMainOrdersDataTable = null;
        private DataTable WeightMainFrontsOrdersDataTable = null;
        private DataTable WeightMainDecorOrdersDataTable = null;
        private DataTable SquareMegaOrdersDataTable = null;
        private DataTable WeightMegaFrontsOrdersDataTable = null;
        private DataTable WeightMegaDecorOrdersDataTable = null;

        public BindingSource ClientsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;
        public BindingSource PackAllocStatusesBindingSource = null;
        public BindingSource MegaOrdersBindingSource = null;
        public BindingSource PackagesBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource PackageStatusesBindingSource = null;
        public BindingSource DocNumbersBindingSource = null;

        private SqlDataAdapter MainOrdersDataAdapter = null;
        private SqlCommandBuilder MainOrdersSqlCommandBuilder = null;

        private SqlDataAdapter MegaOrdersDataAdapter = null;
        private SqlCommandBuilder MegaOrdersSqlCommandBuilder = null;

        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn ClientsColumn = null;
        private DataGridViewComboBoxColumn PackAllocStatusColumn = null;
        private DataGridViewComboBoxColumn PackageStatusesColumn = null;
        private DataGridViewComboBoxColumn ProductionStatusColumn = null;
        private DataGridViewComboBoxColumn StorageStatusColumn = null;
        private DataGridViewComboBoxColumn DispatchStatusColumn = null;

        public ZOVPackagesPrintManager(ref PercentageDataGrid tMegaOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDataGrid,
            ref PercentageDataGrid tPackagesDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl,
            int iFactoryID)
        {
            MainOrdersDataGrid = tMainOrdersDataGrid;
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            PackagesDataGrid = tPackagesDataGrid;
            OrdersTabControl = tOrdersTabControl;
            FactoryID = iFactoryID;

            if (FactoryID == 2)
            {
                PackAllocStatusID = "TPSPackAllocStatusID";
                PackCount = "TPSPackCount";
                ProductionDate = "TPSProductionDate";
                ProductionStatus = "TPSProductionStatusID";
                StorageStatus = "TPSStorageStatusID";
                DispatchStatus = "TPSDispatchStatusID";
            }

            PackedMainOrdersFrontsOrders = new ZOVPackagesPrintFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid)
            {
                Factory = FactoryID
            };
            PackedMainOrdersDecorOrders = new ZOVPackagesPrintDecorOrders(ref tMainOrdersDecorOrdersDataGrid)
            {
                Factory = FactoryID
            };
            Initialize();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
        }

        #region Initialize

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            MainGridSetting();
            MegaGridSetting();
            PackagesGridSetting();
        }

        private void Create()
        {
            DocNumbersDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            PackagesDataTable = new DataTable();
            PackAllocStatusesDataTable = new DataTable();
            PackageStatusesDataTable = new DataTable();
            FactoryTypesDataTable = new DataTable();
            ProductionStatusesDataTable = new DataTable();
            StorageStatusesDataTable = new DataTable();
            DispatchStatusesDataTable = new DataTable();

            SquareMegaOrdersDataTable = new DataTable();
            WeightMegaFrontsOrdersDataTable = new DataTable();
            WeightMegaDecorOrdersDataTable = new DataTable();
            SquareMainOrdersDataTable = new DataTable();
            WeightMainFrontsOrdersDataTable = new DataTable();
            WeightMainDecorOrdersDataTable = new DataTable();

            DocNumbersBindingSource = new BindingSource();
            ClientsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();
            PackAllocStatusesBindingSource = new BindingSource();
            PackagesBindingSource = new BindingSource();
            PackageStatusesBindingSource = new BindingSource();
            FactoryTypesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            ArrayList FrontIDs = new ArrayList();
            ArrayList ProductIDs = new ArrayList();

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        FrontIDs.Add(Convert.ToInt32(DT.Rows[i]["FrontID"]));
                    }
                }
            }
            int[] Fronts = FrontIDs.OfType<int>().ToArray();
            string OrdersProductionStatus = string.Empty;

            if (FactoryID == 1)
                OrdersProductionStatus = " NOT (ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=2) AND ";
            if (FactoryID == 2)
                OrdersProductionStatus = " NOT (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=2) AND ";

            string SellectionCommand = "SELECT MegaOrderID, DispatchDate, Weight, Square," +
            " ProfilPackAllocStatusID, TPSPackAllocStatusID, " + PackCount + ", infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
            " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
            " WHERE (MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
            " WHERE " + OrdersProductionStatus + " (FactoryID=0 OR FactoryID=" + FactoryID +
            ") AND " + PackAllocStatusID + "<>0 AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE FactoryID = " + FactoryID + " AND ProductType = 0) AND MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FrontID IN (" +
            string.Join(",", Fronts) + ")" +
            " AND FactoryID=" + FactoryID + ")))" +
            " ORDER BY DispatchDate";
            
            MegaOrdersDataAdapter = new SqlDataAdapter(SellectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersSqlCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            MainOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 MainOrders.*, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID",
                ConnectionStrings.ZOVOrdersConnectionString);
            MainOrdersSqlCommandBuilder = new SqlCommandBuilder(MainOrdersDataAdapter);
            MainOrdersDataAdapter.Fill(MainOrdersDataTable);
            
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackAllocStatuses",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackAllocStatusesDataTable);
            }
            PackageStatusesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }
            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM Packages", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Square) As Square, MainOrderID FROM FrontsOrders WHERE FactoryID = " + FactoryID +
                " GROUP BY MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SquareMainOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight, MainOrderID FROM FrontsOrders WHERE FactoryID = " + FactoryID +
                " GROUP BY MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMainFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(Weight) As Weight, MainOrderID FROM DecorOrders WHERE FactoryID = " + FactoryID +
                " GROUP BY MainOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMainDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(FrontsOrders.Square) As Square, MegaOrders.MegaOrderID" +
                " FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID + " GROUP BY MegaOrders.MegaOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SquareMegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(FrontsOrders.Weight) As Weight, MegaOrders.MegaOrderID" +
                " FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID + " GROUP BY MegaOrders.MegaOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMegaFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SUM(DecorOrders.Weight) As Weight, MegaOrders.MegaOrderID" +
                " FROM DecorOrders" +
                " INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID + " GROUP BY MegaOrders.MegaOrderID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(WeightMegaDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DocNumber) FROM MainOrders" +
                " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ")" +
                " ORDER BY DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDataTable);
            }
        }

        private void Binding()
        {
            DocNumbersBindingSource.DataSource = DocNumbersDataTable;
            ClientsBindingSource.DataSource = ClientsDataTable;

            PackAllocStatusesBindingSource.DataSource = PackAllocStatusesDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
            MainOrdersBindingSource.DataSource = MainOrdersDataTable;
            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;
            MainOrdersDataGrid.DataSource = MainOrdersBindingSource;

            PackagesBindingSource.DataSource = PackagesDataTable;
            PackagesDataGrid.DataSource = PackagesBindingSource;
            PackageStatusesBindingSource.DataSource = PackageStatusesDataTable;
        }

        private void CreateColumns()
        {
            ProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProductionStatusColumn",
                HeaderText = "Производство",
                DataPropertyName = ProductionStatus,
                DataSource = new DataView(ProductionStatusesDataTable),
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            StorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "StorageStatusColumn",
                HeaderText = " Склад",
                DataPropertyName = StorageStatus,
                DataSource = new DataView(StorageStatusesDataTable),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            DispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DispatchStatusColumn",
                HeaderText = "Отгрузка",
                DataPropertyName = DispatchStatus,
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ClientsColumn",
                HeaderText = "Клиент",
                DataPropertyName = "ClientID",
                DataSource = ClientsBindingSource,
                ValueMember = "ClientID",
                DisplayMember = "ClientName",
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
            PackAllocStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PackAllocStatusColumn",
                HeaderText = "Упаковано",
                DataPropertyName = PackAllocStatusID,
                DataSource = PackAllocStatusesBindingSource,
                ValueMember = "PackAllocStatusID",
                DisplayMember = "PackAllocStatus",
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
            MainOrdersDataGrid.Columns.Add(FactoryTypeColumn);
            MainOrdersDataGrid.Columns.Add(PackAllocStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(StorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(DispatchStatusColumn);

            PackagesDataGrid.Columns.Add(PackageStatusesColumn);
        }

        private void MainGridSetting()
        {
            foreach (DataGridViewColumn Column in MainOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (MainOrdersDataGrid.Columns.Contains("ProfilProductionStatusID"))
                MainOrdersDataGrid.Columns["ProfilProductionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilStorageStatusID"))
                MainOrdersDataGrid.Columns["ProfilStorageStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("ProfilDispatchStatusID"))
                MainOrdersDataGrid.Columns["ProfilDispatchStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSProductionStatusID"))
                MainOrdersDataGrid.Columns["TPSProductionStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSStorageStatusID"))
                MainOrdersDataGrid.Columns["TPSStorageStatusID"].Visible = false;
            if (MainOrdersDataGrid.Columns.Contains("TPSDispatchStatusID"))
                MainOrdersDataGrid.Columns["TPSDispatchStatusID"].Visible = false;

            MainOrdersDataGrid.Columns["DebtDocNumber"].Visible = false;
            MainOrdersDataGrid.Columns["ReorderDocNumber"].Visible = false;
            MainOrdersDataGrid.Columns["FrontsCost"].Visible = false;
            MainOrdersDataGrid.Columns["DecorCost"].Visible = false;
            MainOrdersDataGrid.Columns["OrderCost"].Visible = false;
            MainOrdersDataGrid.Columns["DispatchedCost"].Visible = false;
            MainOrdersDataGrid.Columns["DispatchedDebtCost"].Visible = false;
            MainOrdersDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
            MainOrdersDataGrid.Columns["CalcDebtCost"].Visible = false;
            MainOrdersDataGrid.Columns["ClientID"].Visible = false;
            MainOrdersDataGrid.Columns["MegaOrderID"].Visible = false;

            MainOrdersDataGrid.Columns["WriteOffDebtCost"].Visible = false;
            MainOrdersDataGrid.Columns["WriteOffDefectsCost"].Visible = false;
            MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].Visible = false;
            MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].Visible = false;
            MainOrdersDataGrid.Columns["TotalWriteOffCost"].Visible = false;
            MainOrdersDataGrid.Columns["IncomeCost"].Visible = false;
            MainOrdersDataGrid.Columns["ProfitCost"].Visible = false;
            MainOrdersDataGrid.Columns["NeedCalculate"].Visible = false;
            MainOrdersDataGrid.Columns["DocDateTime"].Visible = false;
            MainOrdersDataGrid.Columns["WillPercentID"].Visible = false;
            MainOrdersDataGrid.Columns["DebtTypeID"].Visible = false;
            MainOrdersDataGrid.Columns["PriceTypeID"].Visible = false;
            MainOrdersDataGrid.Columns["IsPrepared"].Visible = false;
            MainOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["DoNotDispatch"].Visible = false;
            MainOrdersDataGrid.Columns["TechDrilling"].Visible = false;
            MainOrdersDataGrid.Columns["QuicklyOrder"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;

            if (FactoryID == 1)
            {
                MainOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
                MainOrdersDataGrid.Columns["TPSProductionDate"].Visible = false;
            }
            else
            {
                MainOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilProductionDate"].Visible = false;
            }

            MainOrdersDataGrid.Columns[ProductionDate].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            MainOrdersDataGrid.Columns["MainOrderID"].HeaderText = "№ п\\п";
            MainOrdersDataGrid.Columns["DocNumber"].HeaderText = "№ документа";
            MainOrdersDataGrid.Columns["DocDateTime"].HeaderText = "Дата\r\nсоздания";
            MainOrdersDataGrid.Columns["AllocPackDateTime"].HeaderText = "Дата\r\nраспределения";
            MainOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            MainOrdersDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            MainOrdersDataGrid.Columns["IsSample"].HeaderText = "Образец";
            MainOrdersDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            MainOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MainOrdersDataGrid.Columns[PackCount].HeaderText = "Кол-во\r\nупаковок";

            MainOrdersDataGrid.Columns[ProductionDate].HeaderText = "Дата входа\r\n впр-во";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["MainOrderID"].Width = 65;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].Width = 140;
            MainOrdersDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MainOrdersDataGrid.Columns["ClientName"].MinimumWidth = 160;
            MainOrdersDataGrid.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MainOrdersDataGrid.Columns["DocNumber"].MinimumWidth = 150;
            MainOrdersDataGrid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["FrontsSquare"].Width = 120;
            MainOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["IsSample"].Width = 100;
            MainOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            MainOrdersDataGrid.Columns["Notes"].MinimumWidth = 110;
            MainOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["Weight"].Width = 70;
            MainOrdersDataGrid.Columns["PackAllocStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            MainOrdersDataGrid.Columns["PackAllocStatusColumn"].MinimumWidth = 140;
            MainOrdersDataGrid.Columns[PackCount].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns[PackCount].Width = 100;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].Width = 140;

            MainOrdersDataGrid.Columns[ProductionDate].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns[ProductionDate].MinimumWidth = 135;

            MainOrdersDataGrid.Columns["ProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProductionStatusColumn"].Width = 150;
            MainOrdersDataGrid.Columns["StorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["StorageStatusColumn"].Width = 110;
            MainOrdersDataGrid.Columns["DispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["DispatchStatusColumn"].Width = 110;

            MainOrdersDataGrid.AutoGenerateColumns = false;

            MainOrdersDataGrid.Columns["MainOrderID"].DisplayIndex = 1;
            MainOrdersDataGrid.Columns["ClientName"].DisplayIndex = 2;
            MainOrdersDataGrid.Columns["DocNumber"].DisplayIndex = 3;
            MainOrdersDataGrid.Columns[PackCount].DisplayIndex = 4;
            MainOrdersDataGrid.Columns["PackAllocStatusColumn"].DisplayIndex = 5;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].DisplayIndex = 6;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].DisplayIndex = 7;

            MainOrdersDataGrid.Columns[ProductionDate].DisplayIndex = 8;
            MainOrdersDataGrid.Columns["ProductionStatusColumn"].DisplayIndex = 9;
            MainOrdersDataGrid.Columns["StorageStatusColumn"].DisplayIndex = 10;
            MainOrdersDataGrid.Columns["DispatchStatusColumn"].DisplayIndex = 11;

            MainOrdersDataGrid.Columns["FrontsSquare"].DisplayIndex = 12;
            MainOrdersDataGrid.Columns["Weight"].DisplayIndex = 13;
            MainOrdersDataGrid.Columns["IsSample"].DisplayIndex = 14;
            MainOrdersDataGrid.Columns["Notes"].DisplayIndex = 15;

            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns[PackCount].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void MegaGridSetting()
        {
            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //MegaOrdersDataGrid.Columns["DispatchStatusID"].Visible = false;
            //MegaOrdersDataGrid.Columns["TotalCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["DispatchedCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["DispatchedDebtCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcDebtCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcDefectsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcProductionErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["CalcZOVErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["SamplesWriteOffCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["WriteOffDebtCost"].Visible = false;

            //MegaOrdersDataGrid.Columns["WriteOffDefectsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["TotalWriteOffCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["TotalCalcWriteOffCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["IncomeCost"].Visible = false;
            //MegaOrdersDataGrid.Columns["ProfitCost"].Visible = false;

            MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;


            //MegaOrdersDataGrid.Columns[PackAllocStatusID].Visible = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["DispatchDate"].HeaderText = "Дата\r\nотгрузки";
            MegaOrdersDataGrid.Columns["PackAllocStatus"].HeaderText = "Упaковано";
            MegaOrdersDataGrid.Columns[PackCount].HeaderText = "Кол-во\r\nупаковок";
            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = "Площадь\r\nфасадов, м.кв.";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["DispatchDate"].DisplayIndex = 1;
            MegaOrdersDataGrid.Columns[PackCount].DisplayIndex = 2;
            MegaOrdersDataGrid.Columns["PackAllocStatus"].DisplayIndex = 3;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = 4;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = 5;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MegaOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Square"].Width = 130;
            MegaOrdersDataGrid.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["DispatchDate"].Width = 120;
            MegaOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Weight"].Width = 70;
            MegaOrdersDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaOrderID"].Width = 80;
            MegaOrdersDataGrid.Columns["PackAllocStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaOrdersDataGrid.Columns["PackAllocStatus"].MinimumWidth = 170;
            MegaOrdersDataGrid.Columns[PackCount].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns[PackCount].Width = 110;
        }

        private void PackagesGridSetting()
        {
            foreach (DataGridViewColumn Column in PackagesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //PackagesDataGrid.Columns["PackageID"].Visible = false;
            PackagesDataGrid.Columns["PackageStatusID"].Visible = false;
            PackagesDataGrid.Columns["MainOrderID"].Visible = false;

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

            PackagesDataGrid.Columns["PrintDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["PackingDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["DispatchDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["PackNumber"].HeaderText = "  №\r\nупак.";
            PackagesDataGrid.Columns["PrintedCount"].HeaderText = "    Кол-во\r\nраспечаток";
            PackagesDataGrid.Columns["PrintDateTime"].HeaderText = "  Дата\r\nпечати";
            PackagesDataGrid.Columns["PackingDateTime"].HeaderText = "   Дата\r\nупаковки";
            PackagesDataGrid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            PackagesDataGrid.Columns["ExpeditionDateTime"].HeaderText = "      Дата\r\nэкспедиции";
            PackagesDataGrid.Columns["PackageID"].HeaderText = "ID";
            PackagesDataGrid.Columns["DispatchDateTime"].HeaderText = "    Дата\r\nотгрузки";

            PackagesDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackNumber"].Width = 70;
            PackagesDataGrid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageStatusesColumn"].Width = 140;
            PackagesDataGrid.Columns["PrintedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PrintedCount"].Width = 140;
            PackagesDataGrid.Columns["PrintDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PrintDateTime"].Width = 150;
            PackagesDataGrid.Columns["PackingDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackingDateTime"].Width = 150;
            PackagesDataGrid.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["StorageDateTime"].Width = 150;
            PackagesDataGrid.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["ExpeditionDateTime"].Width = 150;
            PackagesDataGrid.Columns["DispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["DispatchDateTime"].Width = 150;
            PackagesDataGrid.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageID"].Width = 100;

            PackagesDataGrid.AutoGenerateColumns = false;

            PackagesDataGrid.Columns["PackNumber"].DisplayIndex = 0;
            PackagesDataGrid.Columns["PackageStatusesColumn"].DisplayIndex = 1;
            PackagesDataGrid.Columns["PrintedCount"].DisplayIndex = 2;
            PackagesDataGrid.Columns["PrintDateTime"].DisplayIndex = 3;
            PackagesDataGrid.Columns["PackingDateTime"].DisplayIndex = 4;
            PackagesDataGrid.Columns["StorageDateTime"].DisplayIndex = 5;
            PackagesDataGrid.Columns["ExpeditionDateTime"].DisplayIndex = 6;
            PackagesDataGrid.Columns["DispatchDateTime"].DisplayIndex = 7;
            //PackagesDataGrid.Columns["PackageID"].DisplayIndex = 6;
        }

        #endregion


        #region Filter
        private decimal GetSquareMegaOrder(int MegaOrderID)
        {
            decimal Square = 0;

            DataRow[] Rows = SquareMegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
            if (Rows.Count() > 0)
                Square = Convert.ToDecimal(Rows[0]["Square"]);
            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);

            return Square;
        }

        private decimal GetSquareMainOrder(int MainOrderID)
        {
            decimal Square = 0;

            DataRow[] Rows = SquareMainOrdersDataTable.Select("MainOrderID = " + MainOrderID);

            if (Rows.Count() > 0)
                Square = Convert.ToDecimal(Rows[0]["Square"]);

            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);

            return Square;
        }

        private decimal GetWeightMegaOrder(int MegaOrderID)
        {
            decimal Weight = 0;

            DataRow[] FRows = WeightMegaFrontsOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
            if (FRows.Count() > 0)
                Weight += Convert.ToDecimal(FRows[0]["Weight"]);
            DataRow[] DRows = WeightMegaDecorOrdersDataTable.Select("MegaOrderID = " + MegaOrderID);
            if (DRows.Count() > 0)
                Weight += Convert.ToDecimal(DRows[0]["Weight"]);
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        private decimal GetWeightMainOrder(int MainOrderID)
        {
            decimal Weight = 0;
            DataRow[] FRows = WeightMainFrontsOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            if (FRows.Count() > 0)
                Weight += Convert.ToDecimal(FRows[0]["Weight"]);
            DataRow[] DRows = WeightMainDecorOrdersDataTable.Select("MainOrderID = " + MainOrderID);
            if (DRows.Count() > 0)
                Weight += Convert.ToDecimal(DRows[0]["Weight"]);
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        public void FillOrders(ArrayList FrontIDs, ArrayList ProductIDs, bool Dispatched, bool NotPrinted)
        {
            int[] Fronts = FrontIDs.OfType<int>().ToArray();
            int[] Products = ProductIDs.OfType<int>().ToArray();

            MegaOrdersDataTable.Clear();

            DateTime CurrentDate = GetCurrentDate.AddDays(-78);
            string DispatchDate = CurrentDate.ToString("yyyy-MM-dd");
            string OrdersProductionStatus = string.Empty;
            string PrintedCountFilter = string.Empty;
            CurrentMegaOrderID = -1;

            if (Dispatched)
            {
                if (FactoryID == 1)
                    OrdersProductionStatus = " NOT (ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=2) AND ";
                if (FactoryID == 2)
                    OrdersProductionStatus = " NOT (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=2) AND ";
            }
            if (NotPrinted)
            {
                PrintedCountFilter = " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PrintedCount = 0 AND FactoryID = " + FactoryID + ")";
            }

            string SellectionCommand = "SELECT MegaOrderID, DispatchDate, Weight, Square," +
                " ProfilPackAllocStatusID, TPSPackAllocStatusID, " + PackCount + ", infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE " + OrdersProductionStatus + " (FactoryID=0 OR FactoryID=" + FactoryID +
                ") AND " + PackAllocStatusID + "<>0" + PrintedCountFilter + " AND (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FrontID IN (" +
                string.Join(",", Fronts) + ")" +
                " AND FactoryID=" + FactoryID + ") OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE ProductID IN (" +
                string.Join(",", Products) + ")" +
                " AND FactoryID=" + FactoryID + ")))" +
                " ORDER BY DispatchDate";

            if (Fronts.Count() < 1)
            {
                if (NotPrinted)
                {
                    PrintedCountFilter = " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PrintedCount = 0 AND FactoryID = " + FactoryID + " AND ProductType = 1)";
                }
                SellectionCommand = "SELECT MegaOrderID, DispatchDate, Weight, Square," +
                " ProfilPackAllocStatusID, TPSPackAllocStatusID, " + PackCount + ", infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE (MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE " + OrdersProductionStatus + " (FactoryID=0 OR FactoryID=" + FactoryID +
                ") AND " + PackAllocStatusID + "<>0" + PrintedCountFilter + " AND MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE ProductID IN (" +
                string.Join(",", Products) + ")" +
                " AND FactoryID=" + FactoryID + ")))" +
                " ORDER BY DispatchDate";
            }

            if (Products.Count() < 1)
            {
                if (NotPrinted)
                {
                    PrintedCountFilter = " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PrintedCount = 0 AND FactoryID = " + FactoryID + " AND ProductType = 0)";
                }
                SellectionCommand = "SELECT MegaOrderID, DispatchDate, Weight, Square," +
                " ProfilPackAllocStatusID, TPSPackAllocStatusID, " + PackCount + ", infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE (MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE " + OrdersProductionStatus + " (FactoryID=0 OR FactoryID=" + FactoryID +
                ") AND " + PackAllocStatusID + "<>0" + PrintedCountFilter + " AND MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FrontID IN (" +
                string.Join(",", Fronts) + ")" +
                " AND FactoryID=" + FactoryID + ")))" +
                " ORDER BY DispatchDate";
            }

            if (Fronts.Count() < 1 && Products.Count() < 1)
            {
                SellectionCommand = "SELECT TOP 0 MegaOrderID, DispatchDate, Weight, Square," +
                   " ProfilPackAllocStatusID, TPSPackAllocStatusID, " + PackCount + ", infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                   " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID";
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            
            MegaOrdersDataAdapter = new SqlDataAdapter(SellectionCommand, ConnectionStrings.ZOVOrdersConnectionString);
            MegaOrdersSqlCommandBuilder = new SqlCommandBuilder(MegaOrdersDataAdapter);
            MegaOrdersDataAdapter.Fill(MegaOrdersDataTable);

            sw.Stop();
            double G = sw.Elapsed.TotalSeconds;

            sw.Reset();
            sw.Start();

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                MegaOrdersDataTable.Rows[i]["Square"] = GetSquareMegaOrder(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
                MegaOrdersDataTable.Rows[i]["Weight"] = GetWeightMegaOrder(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
            }

            sw.Stop();
            double G1 = sw.Elapsed.TotalSeconds;
        }

        public bool FilterMainOrders(int MegaOrderID, ArrayList FrontIDs, ArrayList ProductIDs, bool Dispatched, bool NotPrinted)
        {
            if (MegaOrdersBindingSource.Count == 0)
                return true;

            int[] Fronts = FrontIDs.OfType<int>().ToArray();
            int[] Products = ProductIDs.OfType<int>().ToArray();
            string OrdersProductionStatus = string.Empty;
            string PrintedCountFilter = string.Empty;

            if (Dispatched)
            {
                if (FactoryID == 1)
                    OrdersProductionStatus = " NOT (ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=2) AND ";
                if (FactoryID == 2)
                    OrdersProductionStatus = " NOT (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=2) AND ";
            }
            if (NotPrinted)
            {
                PrintedCountFilter = " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PrintedCount = 0 AND FactoryID = " + FactoryID + ")";
            }

            MainOrdersDataTable.Clear();

            string SelectionCommand = "SELECT MainOrderID, MegaOrderID, DocNumber, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID," + PackCount + "," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID , ProfilStorageStatusID , ProfilDispatchStatusID, " +
                " TPSProductionDate, TPSProductionStatusID , TPSStorageStatusID , TPSDispatchStatusID, " +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE " + OrdersProductionStatus + " MegaOrderID=" + MegaOrderID +
                " AND (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                " AND " + PackAllocStatusID + "<>0" + PrintedCountFilter + " AND (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FrontConfigID IN" +
                " (SELECT FrontConfigID FROM infiniu2_catalog.dbo.FrontsConfig WHERE FrontID IN (" + string.Join(",", Fronts) + "))" +
                " AND FactoryID=" + FactoryID + ") OR MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE DecorConfigID IN" +
                " (SELECT DecorConfigID FROM infiniu2_catalog.dbo.DecorConfig WHERE ProductID IN (" + string.Join(",", Products) + ")) AND FactoryID=" + FactoryID + "))" +
                " ORDER BY ClientName";

            if (Fronts.Count() < 1)
            {
                if (NotPrinted)
                {
                    PrintedCountFilter = " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PrintedCount = 0 AND FactoryID = " + FactoryID + " AND ProductType = 1)";
                }
                SelectionCommand = "SELECT MainOrderID, MegaOrderID, DocNumber, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID," + PackCount + "," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID , ProfilStorageStatusID , ProfilDispatchStatusID, " +
                " TPSProductionDate, TPSProductionUserID, TPSProductionUserID, TPSProductionStatusID , TPSStorageStatusID , TPSDispatchStatusID, " +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE " + OrdersProductionStatus + " MegaOrderID=" + MegaOrderID +
                " AND (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                " AND " + PackAllocStatusID + "<>0" + PrintedCountFilter + " AND (MainOrderID IN (SELECT MainOrderID FROM DecorOrders WHERE DecorConfigID IN" +
                " (SELECT DecorConfigID FROM infiniu2_catalog.dbo.DecorConfig WHERE ProductID IN (" + string.Join(",", Products) + ")) AND FactoryID=" + FactoryID + "))" +
                " ORDER BY ClientName";
            }

            if (Products.Count() < 1)
            {
                if (NotPrinted)
                {
                    PrintedCountFilter = " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PrintedCount = 0 AND FactoryID = " + FactoryID + " AND ProductType = 0)";
                }
                SelectionCommand = "SELECT MainOrderID, MegaOrderID, DocNumber, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID," + PackCount + "," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID , ProfilStorageStatusID , ProfilDispatchStatusID, " +
                " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID , TPSStorageStatusID , TPSDispatchStatusID, " +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE " + OrdersProductionStatus + " MegaOrderID=" + MegaOrderID +
                " AND (FactoryID=0 OR FactoryID=" + FactoryID + ")" +
                " AND " + PackAllocStatusID + "<>0" + PrintedCountFilter + " AND (MainOrderID IN (SELECT MainOrderID FROM FrontsOrders WHERE FrontConfigID IN" +
                " (SELECT FrontConfigID FROM infiniu2_catalog.dbo.FrontsConfig WHERE FrontID IN (" + string.Join(",", Fronts) + "))" +
                " AND FactoryID=" + FactoryID + "))" +
                " ORDER BY ClientName";
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            sw.Stop();
            double G = sw.Elapsed.TotalMilliseconds;

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                MainOrdersDataTable.Rows[i]["FrontsSquare"] = GetSquareMainOrder(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
                MainOrdersDataTable.Rows[i]["Weight"] = GetWeightMainOrder(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
            }

            CurrentMegaOrderID = MegaOrderID;

            return MainOrdersDataTable.Rows.Count > 0;
        }

        public void FilterPackages(int MainOrderID, ArrayList FrontIDs, ArrayList ProductIDs)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            PackagesDataTable.Clear();

            if (MainOrderID < 0)
                return;

            int[] Fronts = FrontIDs.OfType<int>().ToArray();
            int[] Decor = ProductIDs.OfType<int>().ToArray();

            string SelectionCommand = "SELECT * FROM Packages" +
                " WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackageID IN (SELECT PackageID FROM PackageDetails WHERE (OrderID IN (SELECT FrontsOrdersID FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND FrontID IN (" + string.Join(",", Fronts) + "))" +
                " OR OrderID IN (SELECT DecorOrderID FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND ProductID IN (" + string.Join(",", Decor) + "))))";

            if (FrontIDs.Count > 0 && ProductIDs.Count < 1)
            {
                SelectionCommand = "SELECT * FROM Packages" +
                " WHERE ProductType = 0 AND FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackageID IN (SELECT PackageID FROM PackageDetails WHERE OrderID IN (SELECT FrontsOrdersID FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND FrontID IN (" + string.Join(",", Fronts) + ")))";
            }

            if (ProductIDs.Count > 0 && FrontIDs.Count < 1)
            {
                SelectionCommand = "SELECT * FROM Packages" +
                " WHERE ProductType = 1 AND FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackageID IN (SELECT PackageID FROM PackageDetails WHERE OrderID IN (SELECT DecorOrderID FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND ProductID IN (" + string.Join(",", Decor) + ")))";
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }
            PackagesDataTable.DefaultView.Sort = "PackNumber ASC";

            sw.Stop();
            double G = sw.Elapsed.TotalSeconds;

        }

        public DataTable TempPackages(int MainOrderID, ArrayList FrontIDs, ArrayList ProductIDs)
        {
            DataTable DT = PackagesDataTable.Clone();

            int[] Fronts = FrontIDs.OfType<int>().ToArray();
            int[] Products = ProductIDs.OfType<int>().ToArray();

            string SelectionCommand = "SELECT * FROM Packages" +
                " WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackNumber IN (SELECT PackNumber FROM PackageDetails WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND (OrderID IN (SELECT FrontsOrdersID FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND FrontID IN (" + string.Join(",", Fronts) + "))" +
                " OR OrderID IN (SELECT DecorOrderID FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND ProductID IN (" + string.Join(",", Products) + "))))";

            if (FrontIDs.Count > 0 && ProductIDs.Count < 1)
            {
                SelectionCommand = "SELECT * FROM Packages" +
                " WHERE ProductType=0 AND FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackNumber IN (SELECT PackNumber FROM PackageDetails WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND OrderID IN (SELECT FrontsOrdersID FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND FrontID IN (" + string.Join(",", Fronts) + ")))";
            }

            if (ProductIDs.Count > 0 && FrontIDs.Count < 1)
            {
                SelectionCommand = "SELECT * FROM Packages" +
                " WHERE ProductType=1 AND FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackNumber IN (SELECT PackNumber FROM PackageDetails WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND OrderID IN (SELECT DecorOrderID FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND ProductID IN (" + string.Join(",", Products) + ")))";
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            return (DataTable)DT;
        }

        public void Filter(int MainOrderID, ArrayList FrontIDs, ArrayList ProductIDs)
        {
            //if (CurrentMainOrderID == MainOrderID || MainOrderID < 0)
            //    return;
            //CurrentMainOrderID = MainOrderID;
            OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.Filter(MainOrderID, FrontIDs);
            OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.Filter(MainOrderID, ProductIDs);
        }

        public void FilterMegaOrders(bool bsIsPacked, bool bsIsNonPacked)
        {
            string Filter = null;
            MegaOrdersBindingSource.RemoveFilter();

            //запакован
            if (bsIsPacked)
                Filter += PackAllocStatusID + " = 2";

            //не запакован
            if (bsIsNonPacked)
                if (Filter != null)
                    Filter += " OR (" + PackAllocStatusID + "= 0 OR " + PackAllocStatusID + "= 1)";
                else
                    Filter += PackAllocStatusID + "= 0 OR " + PackAllocStatusID + "= 1";

            if (!bsIsPacked && !bsIsNonPacked)
                Filter = PackAllocStatusID + " = -1";

            CurrentMegaOrderID = -1;

            MegaOrdersBindingSource.Filter = Filter;
        }
        #endregion

        public int[] GetSelectedMainOrders()
        {
            int[] rows = new int[MainOrdersDataGrid.SelectedRows.Count];

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);
            Array.Sort(rows);

            return rows;
        }

        public int[] GetSelectedPackages()
        {
            int[] rows = new int[PackagesDataGrid.SelectedRows.Count];

            for (int i = 0; i < PackagesDataGrid.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(PackagesDataGrid.SelectedRows[i].Cells["PackageID"].Value);
            Array.Sort(rows);

            return rows;
        }

        public int[] GetMainOrders()
        {
            int[] rows = new int[MainOrdersDataTable.Rows.Count];

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);

            return rows;
        }

        public void PrintedCountUp(int PackageID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PrintedCount, PrintDateTime FROM Packages WHERE PackageID=" + PackageID,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["PrintedCount"] = Convert.ToInt32(DT.Rows[0]["PrintedCount"]) + 1;
                        DT.Rows[0]["PrintDateTime"] = GetCurrentDate;
                        DA.Update(DT);
                    }
                }
            }
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает true, если подзаказ полностью распределён на Профиле и на ТПС
        /// </summary>
        /// <param name="MainOrderID">ID заказа</param>
        /// <returns></returns>
        public bool IsMainOrderPacked(int MainOrderID)
        {
            int ProfilPackAllocStatusID = 0;
            int TPSPackAllocStatusID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID" +
                " FROM MainOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        //Если подзаказ и на Профиле, и на ТПС
                        if (Convert.ToInt32(DT.Rows[0]["FactoryID"]) == 0)
                        {
                            ProfilPackAllocStatusID = Convert.ToInt32(DT.Rows[0]["ProfilPackAllocStatusID"]);
                            TPSPackAllocStatusID = Convert.ToInt32(DT.Rows[0]["TPSPackAllocStatusID"]);

                            //Можно распечатать, только если на обеих фирмах подзаказ распределён полностью
                            if (ProfilPackAllocStatusID == 2 && TPSPackAllocStatusID == 2)
                                return true;
                            else
                                return false;
                        }
                        else
                            return true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// возвращает true, если заказ полностью распределён на Профиле и на ТПС
        /// </summary>
        /// <param name="MegaOrderID">ID заказа</param>
        /// <param name="MainOrderID">ID подзаказа, который не полностью распределён</param>
        /// <returns></returns>
        public bool IsMegaOrderPacked(int MegaOrderID, ref int MainOrderID)
        {
            int ProfilPackAllocStatusID = 0;
            int TPSPackAllocStatusID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID" +
                " FROM MainOrders WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            //Если подзаказ и на Профиле, и на ТПС
                            if (Convert.ToInt32(DT.Rows[i]["FactoryID"]) == 0)
                            {
                                ProfilPackAllocStatusID = Convert.ToInt32(DT.Rows[i]["ProfilPackAllocStatusID"]);
                                TPSPackAllocStatusID = Convert.ToInt32(DT.Rows[i]["TPSPackAllocStatusID"]);

                                //Можно распечатать, только если на обеих фирмах подзаказ распределён полностью
                                if (ProfilPackAllocStatusID != 2 || TPSPackAllocStatusID != 2)
                                {
                                    MainOrderID = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void FindDocNumber(string DocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, DispatchDate, Weight, Square," + PackAllocStatusID + ", " + PackCount + "," +
                " infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatus FROM MegaOrders" +
                " INNER JOIN infiniu2_catalog.dbo.PackAllocStatuses ON MegaOrders." + PackAllocStatusID + " = infiniu2_catalog.dbo.PackAllocStatuses.PackAllocStatusID" +
                " WHERE MegaOrderID = (SELECT MegaOrderID FROM MainOrders WHERE DocNumber = '" + DocNumber + "')",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                MegaOrdersDataTable.Clear();
                DA.Fill(MegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID, MainOrders.ClientID, DocNumber, FrontsSquare," +
                " Weight, DocDateTime, IsSample, Notes, " + PackAllocStatusID + ", " + PackCount + ", " +
                " AllocPackDateTime, infiniu2_zovreference.dbo.Clients.ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                MainOrdersDataTable.Clear();
                DA.Fill(MainOrdersDataTable);
            }
        }
    }
    #endregion








    public class PackingList : IAllFrontParameterName, IIsMarsel
    {
        private int FactoryID = 1;

        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public PackingList()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
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

        private void GetColorsDT()
        {
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

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();

            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            GetColorsDT();
            GetInsetColorsDT();
            SelectCommand = @"SELECT * FROM Patina";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
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
            SelectCommand = @"SELECT * FROM InsetTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
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

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }

        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add("Product", Type.GetType("System.String"));
            DecorResultDataTable.Columns.Add("Color", Type.GetType("System.String"));
            DecorResultDataTable.Columns.Add("Height", Type.GetType("System.Int32"));
            DecorResultDataTable.Columns.Add("Width", Type.GetType("System.Int32"));
            DecorResultDataTable.Columns.Add("Count", Type.GetType("System.Int32"));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
        }

        #region Реализация интерфейса IReference
                
        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        private bool IsDoNotDispatch(int MainOrderID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DoNotDispatch  FROM MainOrders" +
                    " WHERE MainOrderID=" + MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]);
                }
            }
            return false;
        }

        public string GetOrderNumber(int MainOrderID)
        {
            string DocNumber = "";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber FROM MainOrders" +
                    " WHERE MainOrderID=" +  MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        DocNumber = DT.Rows[0]["DocNumber"].ToString();
                }
            }
            return DocNumber;
        }

        public string GetClientName(int MainOrderID)
        {
            int ClientID = 0;
            string ClientName = "";

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MainOrders" +
                    " WHERE MainOrderID=" +  MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetDispatchDate(int MainOrderID, int FactoryID)
        {
            string DispatchDate = "";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDispatchDate(int MegaOrderID)
        {
            string DispatchDate = "";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }
        #endregion

        #region Реализация интерфейса

        public bool IsMarsel3(int FrontID)
        {
            return FrontID == 3630;
        }

        public bool IsMarsel4(int FrontID)
        {
            return FrontID == 15003;
        }
        public bool IsImpost(int TechnoProfileID)
        {
            return TechnoProfileID == -1;
        }
        public string GetFrontName(int FrontID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }

        public string GetFront2Name(int TechnoProfileID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + TechnoProfileID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }
        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
                ColorName = Rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetPatinaName(int PatinaID)
        {
            string PatinaName = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetType = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetType = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetType;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }
        #endregion

        private decimal GetSquare(DataTable DT)
        {
            decimal Square = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    Square += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return Square;
        }

        private int GetCount(DataTable DT)
        {
            int Count = 0;

            foreach (DataRow Row in DT.Rows)
            {
                Count += Convert.ToInt32(Row["Count"]);
            }

            return Count;
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

        public DataTable PackageFrontsSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        public DataTable PackageDecorSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        private bool FillFronts()
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if ((Convert.ToInt32(Row["FrontID"]) == 3630 || Convert.ToInt32(Row["FrontID"]) == 15003) && Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) == Convert.ToInt32(Row["ColorID"]))
                    Front = Front + " ИМП";
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }
                NewRow["Front"] = Front;
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["Square"] = Row["Square"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FillDecor()
        {
            DecorResultDataTable.Clear();

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                DataRow NewRow = DecorResultDataTable.NewRow();

                NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                //if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                //{
                //    if (Convert.ToInt32(Row["Height"]) != -1)
                //        NewRow["Height"] = Row["Height"];
                //    if (Convert.ToInt32(Row["Length"]) != -1)
                //        NewRow["Height"] = Row["Length"];
                //}
                //if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                //{
                //    if (Convert.ToInt32(Row["Length"]) != -1)
                //        NewRow["Height"] = Row["Length"];
                //    if (Convert.ToInt32(Row["Height"]) != -1)
                //        NewRow["Height"] = Row["Height"];
                //}
                if (Convert.ToInt32(Row["Height"]) != -1)
                    NewRow["Height"] = Row["Height"];
                if (Convert.ToInt32(Row["Length"]) != -1)
                    NewRow["Height"] = Row["Length"];
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    NewRow["Width"] = Row["Width"];

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        public int[] GetFrontsPackageIDs(int MainOrderID)
        {
            int[] PackageIDs = { 0 };

            string ProductType = "ProductType = 0";

            string SelectionCommand = "SELECT * FROM Packages" +
                " WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackNumber IN (SELECT PackNumber FROM PackageDetails WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND " + ProductType + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
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
            //PackagesDataTable.DefaultView.Sort = "PackNumber ASC";
        }

        public int[] GetDecorPackageIDs(int MainOrderID)
        {
            int[] PackageIDs = { 0 };

            string ProductType = "ProductType = 1";

            string SelectionCommand = "SELECT * FROM Packages" +
                " WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID +
                " AND PackNumber IN (SELECT PackNumber FROM PackageDetails WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID = " + FactoryID + " AND " + ProductType + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
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
            //PackagesDataTable.DefaultView.Sort = "PackNumber ASC";
        }

        private bool FilterFrontsOrders(int MainOrderID, ArrayList FrontIDs)
        {
            FrontsOrdersDataTable.Clear();

            int[] Fronts = FrontIDs.OfType<int>().ToArray();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID=" + FactoryID + " AND FrontID IN (" + string.Join(",", Fronts) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" + string.Join(",", GetFrontsPackageIDs(MainOrderID)) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                            " WHERE MainOrderID = " + MainOrderID +
                            " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(FrontsOrdersDataTable);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(int MainOrderID, ArrayList ProductIDs)
        {
            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();

            int[] Products = ProductIDs.OfType<int>().ToArray();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID=" + FactoryID + " AND ProductID IN (" + string.Join(",", Products) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" + string.Join(",", GetDecorPackageIDs(MainOrderID)) + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID = " + MainOrderID +
                            " AND FactoryID=" + FactoryID, ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(DecorOrdersDataTable);

                            if (DecorOrdersDataTable.Rows.Count > 0)
                            {
                                foreach (DataRow Row in DecorOrdersDataTable.Rows)
                                {
                                    //if (Convert.ToInt32(Row["ColorID"]) == -1)
                                    //    Row["ColorID"] = 0;
                                }
                            }
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        public void CreateReport(int[] MainOrders, ArrayList FrontIDs, ArrayList ProductIDs)
        {
            string DispatchDate = GetDispatchDate(MainOrders[0], FactoryID);
            string FileName = string.Empty;
            string Firm = " Профиль";

            if (FactoryID == 2)
                Firm = "ТПС";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            if (FrontIDs.Count > 0)
            {
                CreateFrontsExcel(ref hssfworkbook, MainOrders, FrontIDs);

                //FileName = GetFileName(MainOrders, DispatchDate, "Фасады");

                if (DispatchDate.Length < 1)
                {
                    string DocumentNumber = GetOrderNumber(MainOrders[0]);
                    string ClientName = GetClientName(MainOrders[0]);

                    ClientName = ClientName.Replace('/', '-');
                    ClientName = ClientName.Replace('\"', '\'');
                    DocumentNumber = DocumentNumber.Replace('/', '-');

                    FileName = "Предварилово " + Firm + " Фасады";
                }
                else
                {
                    FileName = DispatchDate + " " + Firm + " Фасады";
                }

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                System.Diagnostics.Process.Start(file.FullName);
            }

            if (ProductIDs.Count > 0)
            {
                CreateDecorExcel(ref hssfworkbook, MainOrders, ProductIDs);

                if (DispatchDate.Length < 1)
                {
                    string DocumentNumber = GetOrderNumber(MainOrders[0]);
                    string ClientName = GetClientName(MainOrders[0]);

                    ClientName = ClientName.Replace('/', '-');
                    ClientName = ClientName.Replace('\"', '\'');
                    DocumentNumber = DocumentNumber.Replace('/', '-');

                    FileName = "Предварилово " + Firm + " Декор";
                }
                else
                {
                    FileName = DispatchDate + " " + Firm + " Декор";
                }

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }
                //FileName = GetFileName(MainOrders, DispatchDate, "Декор");
                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                System.Diagnostics.Process.Start(file.FullName);
            }
        }

        //private string GetFileName(int[] MainOrders, string DispatchDate, string Product)
        //{
        //    string ReportFilePath = ReadReportFilePath("ZOVPackReportPathProfil.config");

        //    string Firm = " Профиль";
        //    string FileName = "";

        //    if (FactoryID == 2)
        //    {
        //        ReportFilePath = ReadReportFilePath("ZOVPackReportPathTPS.config");
        //        Firm = "ТПС";
        //    }

        //    if (DispatchDate.Length < 1)
        //    {
        //        ReportFilePath = ReadReportFilePath("PreviousReportPath.config");

        //        if (!(Directory.Exists(ReportFilePath)))
        //        {
        //            Directory.CreateDirectory(ReportFilePath);
        //        }

        //        string DocumentNumber = GetOrderNumber(MainOrders[0]);
        //        string ClientName = GetClientName(MainOrders[0]);

        //        ClientName = ClientName.Replace('/', '-');
        //        ClientName = ClientName.Replace('\"', '\'');
        //        DocumentNumber = DocumentNumber.Replace('/', '-');

        //        if (!(Directory.Exists(ReportFilePath)))
        //        {
        //            Directory.CreateDirectory(ReportFilePath);
        //        }

        //        FileName = ClientName + " " + DocumentNumber + " " + Firm + " " + Product + ".xls";

        //        FileName = Path.Combine(ReportFilePath, FileName);

        //        int DocNumber = 1;
        //        while (File.Exists(FileName))
        //        {
        //            FileName = ClientName + " " + DocumentNumber + " " + Firm + " " + Product + "(" + DocNumber++ + ").xls";
        //            FileName = Path.Combine(ReportFilePath, FileName);
        //        }

        //    }

        //    else
        //    {
        //        ReportFilePath = Path.Combine(ReportFilePath, DispatchDate);

        //        if (!(Directory.Exists(ReportFilePath)))
        //        {
        //            Directory.CreateDirectory(ReportFilePath);
        //        }

        //        FileName = Firm + " " + Product + ".xls";

        //        FileName = Path.Combine(ReportFilePath, FileName);

        //        int DocNumber = 1;
        //        while (File.Exists(FileName))
        //        {
        //            FileName = Firm + " " + Product + "(" + DocNumber++ + ").xls";
        //            FileName = Path.Combine(ReportFilePath, FileName);
        //        }

        //    }
        //    return FileName;
        //}

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, ArrayList FrontIDs)
        {
            string DispatchDate = "";

            string DocNumber = "";
            string ClientName = "";

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Фасады");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 4 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 18 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 7 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 9 * 256);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);

            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 6, "Утверждаю...............");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            int PackCount = 0;
            int RowIndex = 3;

            int TopRowFront = 1;
            int BottomRowFront = 1;

            string MainOrderNote = "";

            bool IsFronts = false;
            bool DoNotDispatch = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterFrontsOrders(MainOrders[i], FrontIDs);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                DispatchDate = GetDispatchDate(MainOrders[i], FactoryID);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " № " + DocNumber);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к подзаказу: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к подзаказу: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }
                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;

                TopRowFront = RowIndex;
                BottomRowFront = FrontsResultDataTable.Rows.Count + RowIndex;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                        {
                            Type t = FrontsResultDataTable.Rows[x][y].GetType();

                            if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                cell.CellStyle = PackNumberStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == FrontsResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == FRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == FrontsResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                continue;
                            }

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

                                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                cellStyle.SetFont(SimpleFont);
                                cell.CellStyle = cellStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == FrontsResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == FRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == FrontsResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                cell.CellStyle = SimpleCellStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == FrontsResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == FRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == FrontsResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(FRows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == FrontsResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == FRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == FrontsResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                continue;
                            }
                        }
                        
                        RowIndex++;
                    }
                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 8, Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;

                
                RowIndex++;
            }

                for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, ArrayList ProductIDs)
        {
            string DispatchDate = "";

            string DocNumber = "";
            string ClientName = "";

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Декор");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 5 * 256);
            sheet1.SetColumnWidth(1, 25 * 256);
            sheet1.SetColumnWidth(2, 20 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 7 * 256);
            sheet1.SetColumnWidth(5, 6 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);

            int PackCount = 0;
            int RowIndex = 3;

            int TopRowDecor = 1;
            int BottomRowDecor = 1;

            string MainOrderNote = "";

            bool IsDecor = false;
            bool DoNotDispatch = false;

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFCellStyle PackNumberStyle = hssfworkbook.CreateCellStyle();
            PackNumberStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.BottomBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.LeftBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.RightBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            PackNumberStyle.TopBorderColor = HSSFColor.BLACK.index;
            PackNumberStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            PackNumberStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            PackNumberStyle.SetFont(PackNumberFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            #region границы между упаковками

            HSSFCellStyle BottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle BottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle BottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            BottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            BottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            BottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            BottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle LeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            LeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            LeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            LeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            LeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            LeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            LeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle RightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            RightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            RightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            RightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            RightMediumBorderCellStyle.SetFont(SimpleFont);

            #endregion

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 4, "Утверждаю...............");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                FilterDecorOrders(MainOrders[i], ProductIDs);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                DocNumber = GetOrderNumber(MainOrders[i]);
                ClientName = GetClientName(MainOrders[i]);
                DispatchDate = GetDispatchDate(MainOrders[i], FactoryID);
                MainOrderNote = GetMainOrderNotes(MainOrders[i]);
                DoNotDispatch = IsDoNotDispatch(MainOrders[i]);
                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName + " № " + DocNumber);
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к подзаказу: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к подзаказу: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Название");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Цвет");
                cell3.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Длина\\Высота");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Ширина");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 5, "Кол.");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "Прим.");
                cell7.CellStyle = HeaderStyle;

                TopRowDecor = RowIndex;
                BottomRowDecor = DecorResultDataTable.Rows.Count + RowIndex;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count; y++)
                        {
                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                cell.CellStyle = PackNumberStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == DecorResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == DRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == DecorResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                continue;
                            }

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

                                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                cellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
                                cellStyle.BottomBorderColor = HSSFColor.BLACK.index;
                                cellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
                                cellStyle.LeftBorderColor = HSSFColor.BLACK.index;
                                cellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
                                cellStyle.RightBorderColor = HSSFColor.BLACK.index;
                                cellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
                                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                                cellStyle.SetFont(SimpleFont);
                                cell.CellStyle = cellStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == DecorResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == DRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == DecorResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                if (DRows[x][y] != DBNull.Value)
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));
                                cell.CellStyle = SimpleCellStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == DecorResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == DRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == DecorResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(DRows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;

                                if (y == 0)
                                {
                                    cell.CellStyle = LeftMediumBorderCellStyle;
                                }
                                if (y == DecorResultDataTable.Columns.Count - 1)
                                {
                                    cell.CellStyle = RightMediumBorderCellStyle;
                                }

                                if (x == DRows.Count() - 1)
                                {
                                    cell.CellStyle = BottomMediumBorderCellStyle;

                                    if (y == 0)
                                    {
                                        cell.CellStyle = BottomMediumLeftBorderCellStyle;
                                    }

                                    if (y == DecorResultDataTable.Columns.Count - 1)
                                    {
                                        cell.CellStyle = BottomMediumRightBorderCellStyle;
                                    }
                                }

                                continue;
                            }
                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            
                    for (int y = 0; y < DecorResultDataTable.Columns.Count; y++)
                    {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell8.CellStyle = TotalStyle;
            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell9.CellStyle = TotalStyle;
        }

        //private string ReadReportFilePath(string FileName)
        //{
        //    string ReportFilePath = "";
        //    using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName, Encoding.Default))
        //    {
        //        ReportFilePath = sr.ReadToEnd();
        //    }
        //    return ReportFilePath;
        //}
    }




    public class PrintBarCode : IAllFrontParameterName, IIsMarsel
    {
        private int FactoryID = 1;

        private DataTable ClientsDataTable = null;

        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public PrintBarCode()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
        }

        public int Factory
        {
            set { FactoryID = value; }
            get { return FactoryID; }
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

        private void GetColorsDT()
        {
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

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();

            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            GetColorsDT();
            GetInsetColorsDT();
            SelectCommand = @"SELECT * FROM Patina";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
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
            SelectCommand = @"SELECT * FROM InsetTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1   ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.String")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add("Product", Type.GetType("System.String"));
            DecorResultDataTable.Columns.Add("Color", Type.GetType("System.String"));
            DecorResultDataTable.Columns.Add("Height", Type.GetType("System.String"));
            DecorResultDataTable.Columns.Add("Width", Type.GetType("System.String"));
            DecorResultDataTable.Columns.Add("Count", Type.GetType("System.String"));
        }

        #region Реализация интерфейса IReference
                
        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetOrderNumber(int MainOrderID)
        {
            string DocNumber = "";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocNumber FROM MainOrders" +
                    " WHERE MainOrderID=" +  MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        DocNumber = DT.Rows[0]["DocNumber"].ToString();
                }
            }
            return DocNumber;
        }

        public string GetClientName(int MainOrderID)
        {
            int ClientID = 0;
            string ClientName = "";

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MainOrders" +
                    " WHERE MainOrderID=" +  MainOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                }
            }

            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
                ClientName = Rows[0]["ClientName"].ToString();

            return ClientName;
        }

        public string GetDispatchDate(int MainOrderID, int FactoryID)
        {
            string DispatchDate = "";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID + ")", ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }

        public string GetDispatchDate(int MegaOrderID)
        {
            string DispatchDate = "";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DispatchDate FROM MegaOrders" +
                    " WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0 && !string.IsNullOrEmpty(DT.Rows[0]["DispatchDate"].ToString()))
                        DispatchDate = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]).ToString("dd.MM.yyyy");
                }
            }
            return DispatchDate;
        }
        #endregion

        public int GetPackageID(int PackNumber, int ProductType, int MainOrderID)
        {
            int PackageID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = " + ProductType + " AND PackNumber = " + PackNumber + " AND FactoryID = " + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return 0;

                    PackageID = Convert.ToInt32(DT.Rows[0]["PackageID"]);
                }
            }
            return PackageID;
        }

        public int GetPackNumberCount(int MainOrderID, int FactoryID, ref int PackAllocStatusID)
        {
            int Count = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProfilPackCount, TPSPackCount, ProfilPackAllocStatusID, TPSPackAllocStatusID FROM MainOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return 0;

                    Count = Convert.ToInt32(DT.Rows[0]["ProfilPackCount"]) + Convert.ToInt32(DT.Rows[0]["TPSPackCount"]);
                    if (FactoryID == 1)
                        PackAllocStatusID = Convert.ToInt32(DT.Rows[0]["ProfilPackAllocStatusID"]);
                    if (FactoryID == 2)
                        PackAllocStatusID = Convert.ToInt32(DT.Rows[0]["TPSPackAllocStatusID"]);
                }
            }
            return Count;
        }

        #region Реализация интерфейса

        public bool IsMarsel3(int FrontID)
        {
            return FrontID == 3630;
        }

        public bool IsMarsel4(int FrontID)
        {
            return FrontID == 15003;
        }
        public bool IsImpost(int TechnoProfileID)
        {
            return TechnoProfileID == -1;
        }
        public string GetFrontName(int FrontID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }

        public string GetFront2Name(int TechnoProfileID)
        {
            string FrontName = "";
            try
            {
                DataRow[] Rows = FrontsDataTable.Select("FrontID = " + TechnoProfileID);
                FrontName = Rows[0]["FrontName"].ToString();
            }
            catch
            {
                return "";
            }
            return FrontName;
        }
        public string GetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = FrameColorsDataTable.Select("ColorID = " + ColorID);
                ColorName = Rows[0]["ColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetPatinaName(int PatinaID)
        {
            string PatinaName = string.Empty;
            try
            {
                DataRow[] Rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
                PatinaName = Rows[0]["PatinaName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return PatinaName;
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            string InsetType = string.Empty;
            try
            {
                DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
                InsetType = Rows[0]["InsetTypeName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return InsetType;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDataTable.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }
        #endregion

        public string GetBarcodeNumber(int BarcodeType, int PackNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (PackNumber.ToString().Length == 1)
                Number = "00000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 2)
                Number = "0000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 3)
                Number = "000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 4)
                Number = "00000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 5)
                Number = "0000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 6)
                Number = "000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 7)
                Number = "00" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 8)
                Number = "0" + PackNumber.ToString();

            StringBuilder BarcodeNumber = new StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        public bool TechnoInset
        {
            get
            {
                foreach (DataRow Row in FrontsOrdersDataTable.Rows)
                {
                    if (Convert.ToInt32(Row["TechnoInsetTypeID"]) != -1)
                        return true;
                }
                return false;
            }
        }

        public DataTable FillFrontsDataTable()
        {
            FrontsResultDataTable.Clear();

            string Front = "";
            string FrameColor = "";
            string InsetColor = "";
            string TechnoInsetType = "";
            string TechnoInsetColor = "";

            int Height = 0;
            int Width = 0;
            int Count = 0;

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                Front = "";
                FrameColor = "";
                InsetColor = "";
                TechnoInsetType = "";
                TechnoInsetColor = "";
                Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }

                InsetColor = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                TechnoInsetType = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));
                TechnoInsetColor = GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));

                Height = Convert.ToInt32(Row["Height"]);
                Width = Convert.ToInt32(Row["Width"]);
                Count = Convert.ToInt32(Row["Count"]);

                string str = "Front = '" + Front.ToString() +
                    "' AND InsetType = '" + InsetType.ToString() + "' AND FrameColor = '" + FrameColor + "' AND InsetColor = '" + InsetColor +
                    "' AND TechnoInsetType = '" + TechnoInsetType.ToString() + "' AND TechnoInsetColor = '" + TechnoInsetColor.ToString() + "' AND Height = '" + Height.ToString() + "' AND Width = '" + Width.ToString() + "'";

                DataRow[] fRow = FrontsResultDataTable.Select(str);
                if (fRow.Count() == 0)
                {
                    DataRow NewRow = FrontsResultDataTable.NewRow();

                    NewRow["FrameColor"] = FrameColor;
                    NewRow["InsetType"] = InsetType;
                    NewRow["InsetColor"] = InsetColor;
                    NewRow["TechnoInsetType"] = TechnoInsetType;
                    NewRow["TechnoInsetColor"] = TechnoInsetColor;
                    NewRow["Front"] = Front;
                    NewRow["Height"] = Height;
                    NewRow["Width"] = Width;
                    NewRow["Count"] = Count;

                    FrontsResultDataTable.Rows.Add(NewRow);
                }
                else
                {
                    fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Count;
                }
            }
            return FrontsResultDataTable;
        }

        public DataTable FillDecorDataTable()
        {
            DecorResultDataTable.Clear();

            string filter = "";
            string Product = "";
            string Decor = "";
            string Color = "";

            int Height = 0;
            int Width = 0;
            int Count = 0;

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                Product = GetProductName(Convert.ToInt32(Row["ProductID"]));
                Decor = GetDecorName(Convert.ToInt32(Row["DecorID"]));
                Count = Convert.ToInt32(Row["Count"]);

                filter = "Product = '" + Product + " " + Decor + "'";

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                {
                    Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                    if (Convert.ToInt32(Row["PatinaID"]) != -1)
                        Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                    filter += " AND Color = '" + Color.ToString() + "'";
                }

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                {
                    Height = Convert.ToInt32(Row["Height"]);
                    filter += " AND Height = '" + Height.ToString() + "'";
                }

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                {
                    Height = Convert.ToInt32(Row["Length"]);
                    filter += " AND Height = '" + Height.ToString() + "'";
                }

                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                {
                    Width = Convert.ToInt32(Row["Width"]);
                    filter += " AND Width = '" + Width.ToString() + "'";
                }

                DataRow[] dRow = DecorResultDataTable.Select(filter);

                if (dRow.Count() == 0)
                {
                    DataRow NewRow = DecorResultDataTable.NewRow();

                    NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                    //if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                    //{
                    //    if (Convert.ToInt32(Row["Height"]) != -1)
                    //        NewRow["Height"] = Row["Height"];
                    //    if (Convert.ToInt32(Row["Length"]) != -1)
                    //        NewRow["Height"] = Row["Length"];
                    //}
                    //if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
                    //{
                    //    if (Convert.ToInt32(Row["Length"]) != -1)
                    //        NewRow["Height"] = Row["Length"];
                    //    if (Convert.ToInt32(Row["Height"]) != -1)
                    //        NewRow["Height"] = Row["Height"];
                    //}
                    if (Convert.ToInt32(Row["Height"]) != -1)
                        NewRow["Height"] = Row["Height"];
                    if (Convert.ToInt32(Row["Length"]) != -1)
                        NewRow["Height"] = Row["Length"];
                    if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                        NewRow["Width"] = Row["Width"];

                    if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                        NewRow["Color"] = Color;

                    NewRow["Count"] = Row["Count"];

                    DecorResultDataTable.Rows.Add(NewRow);
                }
                else
                {
                    dRow[0]["Count"] = Convert.ToDecimal(dRow[0]["Count"]) + Count;
                }

            }

            return DecorResultDataTable;
        }

        public bool FilterFrontsOrders(int MainOrderID, int PackageID)
        {
            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID=" + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID = " + PackageID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public bool FilterDecorOrders(int MainOrderID, int PackageID)
        {
            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID +
                " AND FactoryID=" + FactoryID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID = " + PackageID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"] +
                                " AND FactoryID=" + FactoryID);

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

            return DecorOrdersDataTable.Rows.Count > 0;
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
                CharWidth = 18;
                CharOffset = 5;
                FontSize = 12.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                CharWidth = 26;
                CharOffset = 5;
                FontSize = 14.0f;
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
        public string ClientName;
        public string DocNumber;
        public int PackNumber;
        public int TotalPackCount;
        public int PackAllocStatusID;
        public string DocDateTime;
        public string DispatchDate;
        public string BarcodeNumber;
        public string Notes;
        public DataTable OrderData;
        public int ProductType;
        public bool TechnoInset;
        public int FactoryType;
        public string GroupType;
    }





    public class PackageLabel
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font ClientFont;
        Font DocFont;
        Font InfoFont;
        Font NotesFont;
        Font HeaderFont;
        Font FrontOrderFont;
        Font DecorOrderFont;
        Font DispatchFont;

        Pen Pen;


        Image ZTTPS;
        Image ZTProfil;
        Image STB;
        Image RST;

        public ArrayList LabelInfo;




        public PackageLabel()
        {
            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            ZTProfil = new Bitmap(Properties.Resources.ZOVPROFIL);
            STB = new Bitmap(Properties.Resources.STB);
            RST = new Bitmap(Properties.Resources.RST);
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
            ClientFont = new Font("Arial", 18.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 16.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        private void DrawTable(PrintPageEventArgs ev)
        {
            float HeaderTopY = 151;
            float OrderTopY = 164;
            float TopLineY = 144;
            float BottomLineY = 315;

            float VertLine1 = 11;
            float VertLine3 = 160;
            float VertLine4 = 247;
            float VertLine6 = 379;
            float VertLine7 = 409;
            float VertLine8 = 439;
            float VertLine9 = 467;

            float factor = 1;

            if (((Info)LabelInfo[CurrentLabelNumber]).ProductType == 0)//фасады
            {
                ev.Graphics.DrawLine(Pen, 11, 113, 467, 113);
                ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["Front"].ToString(), DocFont, FontBrush, 7, 114);
                //header

                int MaxStringLenth1 = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["FrameColor"].ToString().Length;
                int MaxStringLenth2 = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["InsetType"].ToString().Length;
                int MaxStringLenth3 = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["InsetColor"].ToString().Length + ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["TechnoInsetColor"].ToString().Length + 1;
                for (int i = 0, p = 10; i < ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["FrameColor"].ToString().Length > MaxStringLenth1)
                        MaxStringLenth1 = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["FrameColor"].ToString().Length;
                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetType"].ToString().Length > MaxStringLenth2)
                        MaxStringLenth2 = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetType"].ToString().Length;
                    if ((((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetColor"].ToString().Length
                        + ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["TechnoInsetColor"].ToString().Length + 1) > MaxStringLenth3)
                        MaxStringLenth3 = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetColor"].ToString().Length + ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["TechnoInsetColor"].ToString().Length + 1;
                }
                factor = (VertLine6 - VertLine1) / (MaxStringLenth1 + MaxStringLenth2 + MaxStringLenth3);
                VertLine3 = VertLine1 + factor * MaxStringLenth1;
                if (VertLine3 > 180)
                    VertLine3 = 180;
                VertLine4 = VertLine3 + factor * MaxStringLenth2;
                if (VertLine4 > 280)
                    VertLine4 = 280;
                for (int i = 0, p = 10; i < ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["FrameColor"].ToString(), FrontOrderFont, FontBrush, VertLine1 + 1, OrderTopY + p);

                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetType"].ToString(),
                        FrontOrderFont, FontBrush, VertLine3 + 1, OrderTopY + p);

                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetColor"].ToString() + "/" + ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["TechnoInsetColor"].ToString(),
                        FrontOrderFont, FontBrush, VertLine4 + 1, OrderTopY + p);

                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(),
                        FrontOrderFont, FontBrush, VertLine6 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(),
                        FrontOrderFont, FontBrush, VertLine7 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Count"].ToString(),
                        FrontOrderFont, FontBrush, VertLine8 + 1, OrderTopY + p);
                }

                ev.Graphics.DrawString("Профиль", HeaderFont, FontBrush, VertLine1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Вставка", HeaderFont, FontBrush, VertLine3 + 1, HeaderTopY);
                ev.Graphics.DrawString("Цвет наполнителя", HeaderFont, FontBrush, VertLine4 + 1, HeaderTopY);
                ev.Graphics.DrawString("Выс", HeaderFont, FontBrush, VertLine6 + 1, HeaderTopY);
                ev.Graphics.DrawString("Шир", HeaderFont, FontBrush, VertLine7 + 1, HeaderTopY);
                ev.Graphics.DrawString("Кол", HeaderFont, FontBrush, VertLine8 + 1, HeaderTopY);

                ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine6, TopLineY, VertLine6, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine7, TopLineY, VertLine7, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine8, TopLineY, VertLine8, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

                for (int i = 0, p = 168; i <= ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (i != 6)
                        ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                }
            }
            else
            {
                ev.Graphics.DrawLine(Pen, 11, 113, 467, 113);
                string Product = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["Product"].ToString();
                Product = Product.Substring(0, Product.IndexOf(' '));
                ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, 114);
                //header
                ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, 12, HeaderTopY);
                ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, 186, HeaderTopY);
                ev.Graphics.DrawString("Длин", HeaderFont, FontBrush, 353, HeaderTopY);
                ev.Graphics.DrawString("Шир", HeaderFont, FontBrush, 393, HeaderTopY);
                ev.Graphics.DrawString("Кол", HeaderFont, FontBrush, 430, HeaderTopY);


                ev.Graphics.DrawLine(Pen, 11, TopLineY, 11, BottomLineY);
                ev.Graphics.DrawLine(Pen, 185, TopLineY, 185, BottomLineY);
                ev.Graphics.DrawLine(Pen, 353, TopLineY, 353, BottomLineY);
                ev.Graphics.DrawLine(Pen, 392, TopLineY, 392, BottomLineY);
                ev.Graphics.DrawLine(Pen, 429, TopLineY, 429, BottomLineY);
                ev.Graphics.DrawLine(Pen, 467, TopLineY, 467, BottomLineY);

                for (int i = 0, p = 168; i <= ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (i != 6)
                        ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                }

                for (int i = 0, p = 10; i < ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Product"].ToString().Length > 24)
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Product"].ToString().Substring(0, 24), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Product"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);

                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Length > 22)
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Substring(0, 22), DecorOrderFont, FontBrush, 186, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 186, OrderTopY + p);

                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, 353, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, 393, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, 430, OrderTopY + p);

                    //ev.Graphics.DrawString("Профиль Спинка кровати П-036/0", DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    //ev.Graphics.DrawString("ППДубок/ППКант", DecorOrderFont, FontBrush, 236, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 353, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 393, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 430, OrderTopY + p);
                }
            }
        }

        private void DrawTableWithTechnoFront(PrintPageEventArgs ev)
        {
            float HeaderTopY = 151;
            float OrderTopY = 164;
            float TopLineY = 144;
            float BottomLineY = 315;

            float VertLine1 = 11;
            float VertLine3 = 160;
            float VertLine5 = 247;
            float VertLine6 = 379;
            float VertLine7 = 409;
            float VertLine8 = 439;
            float VertLine9 = 467;

            if (((Info)LabelInfo[CurrentLabelNumber]).ProductType == 0)//фасады
            {
                ev.Graphics.DrawLine(Pen, 11, 113, 467, 113);
                ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["Front"].ToString(), DocFont, FontBrush, 7, 114);
                //header
                ev.Graphics.DrawString("Профиль", HeaderFont, FontBrush, VertLine1 + 1, HeaderTopY);
                ev.Graphics.DrawString("Вставка", HeaderFont, FontBrush, VertLine3 + 1, HeaderTopY);
                ev.Graphics.DrawString("Цвет наполнителя", HeaderFont, FontBrush, VertLine5 + 1, HeaderTopY);
                ev.Graphics.DrawString("Выс", HeaderFont, FontBrush, VertLine6 + 1, HeaderTopY);
                ev.Graphics.DrawString("Шир", HeaderFont, FontBrush, VertLine7 + 1, HeaderTopY);
                ev.Graphics.DrawString("Кол", HeaderFont, FontBrush, VertLine8 + 1, HeaderTopY);

                ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine5, TopLineY, VertLine5, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine6, TopLineY, VertLine6, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine7, TopLineY, VertLine7, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine8, TopLineY, VertLine8, BottomLineY);
                ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

                for (int i = 0, p = 168; i <= ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (i != 6)
                        ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                }

                for (int i = 0, p = 10; i < ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    //if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["FrameColor"].ToString().Length > 30)
                    //    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["FrameColor"].ToString().Substring(0, 30),
                    //        FrontOrderFont, FontBrush, VertLine1 + 1, OrderTopY + p);
                    //else
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["FrameColor"].ToString(),
                        FrontOrderFont, FontBrush, VertLine1 + 1, OrderTopY + p);

                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetType"].ToString().Length > 20)
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetType"].ToString().Substring(0, 20),
                            FrontOrderFont, FontBrush, VertLine3 + 1, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetType"].ToString(),
                            FrontOrderFont, FontBrush, VertLine3 + 1, OrderTopY + p);

                    //if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["TechnoInset"].ToString().Length > 20)
                    //    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["TechnoInset"].ToString().Substring(0, 20),
                    //        FrontOrderFont, FontBrush, VertLine5 + 1, OrderTopY + p);
                    //else
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["InsetColor"].ToString() + "/" + ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["TechnoInsetColor"].ToString(),
                        FrontOrderFont, FontBrush, VertLine5 + 1, OrderTopY + p);

                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(),
                        FrontOrderFont, FontBrush, VertLine6 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(),
                        FrontOrderFont, FontBrush, VertLine7 + 1, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Count"].ToString(),
                        FrontOrderFont, FontBrush, VertLine8 + 1, OrderTopY + p);
                }
            }
            else
            {
                ev.Graphics.DrawLine(Pen, 11, 113, 467, 113);
                string Product = ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[0]["Product"].ToString();
                Product = Product.Substring(0, Product.IndexOf(' '));
                ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, 114);
                //header
                ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, 12, HeaderTopY);
                ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, 186, HeaderTopY);
                ev.Graphics.DrawString("Длин", HeaderFont, FontBrush, 353, HeaderTopY);
                ev.Graphics.DrawString("Шир", HeaderFont, FontBrush, 393, HeaderTopY);
                ev.Graphics.DrawString("Кол", HeaderFont, FontBrush, 430, HeaderTopY);


                ev.Graphics.DrawLine(Pen, 11, TopLineY, 11, BottomLineY);
                ev.Graphics.DrawLine(Pen, 185, TopLineY, 185, BottomLineY);
                ev.Graphics.DrawLine(Pen, 353, TopLineY, 353, BottomLineY);
                ev.Graphics.DrawLine(Pen, 392, TopLineY, 392, BottomLineY);
                ev.Graphics.DrawLine(Pen, 429, TopLineY, 429, BottomLineY);
                ev.Graphics.DrawLine(Pen, 467, TopLineY, 467, BottomLineY);

                for (int i = 0, p = 168; i <= ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (i != 6)
                        ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                }

                for (int i = 0, p = 10; i < ((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 24)
                {
                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Product"].ToString().Length > 24)
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Product"].ToString().Substring(0, 24), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Product"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);

                    if (((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Length > 22)
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString().Substring(0, 22), DecorOrderFont, FontBrush, 186, OrderTopY + p);
                    else
                        ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 186, OrderTopY + p);

                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, 353, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, 393, OrderTopY + p);
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, 430, OrderTopY + p);

                    //ev.Graphics.DrawString("Профиль Спинка кровати П-036/0", DecorOrderFont, FontBrush, 12, OrderTopY + p);
                    //ev.Graphics.DrawString("ППДубок/ППКант", DecorOrderFont, FontBrush, 236, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 353, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 393, OrderTopY + p);
                    //ev.Graphics.DrawString("9999", DecorOrderFont, FontBrush, 430, OrderTopY + p);
                }
            }
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

            if (((Info)LabelInfo[CurrentLabelNumber]).ClientName.Length > 30)
                ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).ClientName.Substring(0, 30), ClientFont, FontBrush, 9, 6);
            else
                ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).ClientName, ClientFont, FontBrush, 9, 6);

            ev.Graphics.DrawLine(Pen, 11, 33, 467, 33);
            ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).DocNumber, DocFont, FontBrush, 8, 37);
            ev.Graphics.DrawLine(Pen, 11, 67, 467, 67);

            if (((Info)LabelInfo[CurrentLabelNumber]).Notes != null)
            {
                ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 70);

                if (((Info)LabelInfo[CurrentLabelNumber]).Notes.Length > 37)
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, 84);
                else
                    ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).Notes, NotesFont, FontBrush, 7, 84);
            }

            ev.Graphics.DrawLine(Pen, 11, 144, 467, 144);

            if (((Info)LabelInfo[CurrentLabelNumber]).PackAllocStatusID == 2)
                ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).PackNumber.ToString() + "(" + 
                ((Info)LabelInfo[CurrentLabelNumber]).TotalPackCount.ToString() + ")", DocFont, FontBrush, 371, 34);
            else
                ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).PackNumber.ToString(), DocFont, FontBrush, 388, 34);
            ev.Graphics.DrawLine(Pen, 371, 33, 371, 67);


            //OrderData
            //if (Convert.ToBoolean(((Info)LabelInfo[CurrentLabelNumber]).TechnoInset))
                DrawTableWithTechnoFront(ev);
            //else
            //    DrawTable(ev);


            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 46, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);

            Barcode.DrawBarcodeText(Barcode.BarcodeLength.Medium, ev.Graphics, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);



            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Short, 15, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 342, 69, 130, 15);

            if (((Info)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            {
                ev.Graphics.DrawImage(ZTTPS, 249, 320, 37, 45);
            }
            else
            {
                ev.Graphics.DrawImage(ZTProfil, 249, 320, 37, 45);
            }

            ev.Graphics.DrawImage(STB, 418, 319, 39, 27);
            ev.Graphics.DrawImage(RST, 423, 357, 34, 27);

            ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
            ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);

            ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, 320);

            if (((Info)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 332);
            else
                ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 332);

            ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 344);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 356);
            ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 368);
            ev.Graphics.DrawString("Изготовлено: " + ((Info)LabelInfo[CurrentLabelNumber]).DocDateTime, InfoFont, FontBrush, 305, 380);
            ev.Graphics.DrawString("Группа: " + ((Info)LabelInfo[CurrentLabelNumber]).GroupType, InfoFont, FontBrush, 250, 364);
            ev.Graphics.DrawString(((Info)LabelInfo[CurrentLabelNumber]).DispatchDate, DispatchFont, FontBrush, 237, 374);

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



    }
}
