using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.WeeklyPlanning
{

    public class MainOrdersFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrderID = -1;

        public DataTable FrontsOrdersDataTable = null;
        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;

        public BindingSource FrontsOrdersBindingSource = null;
        private BindingSource FrontsBindingSource = null;
        private BindingSource FrameColorsBindingSource = null;
        private BindingSource PatinaBindingSource = null;
        private BindingSource InsetTypesBindingSource = null;
        private BindingSource InsetColorsBindingSource = null;
        private BindingSource TechnoFrameColorsBindingSource = null;
        private BindingSource TechnoInsetTypesBindingSource = null;
        private BindingSource TechnoInsetColorsBindingSource = null;

        private DataGridViewComboBoxColumn FrontsColumn = null;
        private DataGridViewComboBoxColumn FrameColorsColumn = null;
        private DataGridViewComboBoxColumn PatinaColumn = null;
        private DataGridViewComboBoxColumn InsetTypesColumn = null;
        private DataGridViewComboBoxColumn InsetColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoProfilesColumn = null;
        private DataGridViewComboBoxColumn TechnoFrameColorsColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetTypesColumn = null;
        private DataGridViewComboBoxColumn TechnoInsetColorsColumn = null;

        public MainOrdersFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            FrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
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
        }

        private void Binding()
        {
            FrontsOrdersBindingSource.DataSource = FrontsOrdersDataTable;
            FrontsBindingSource.DataSource = FrontsDataTable;
            FrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            PatinaBindingSource.DataSource = PatinaDataTable;
            InsetTypesBindingSource.DataSource = InsetTypesDataTable;
            InsetColorsBindingSource.DataSource = InsetColorsDataTable;
            TechnoFrameColorsBindingSource.DataSource = new DataView(FrameColorsDataTable);
            TechnoInsetTypesBindingSource.DataSource = TechnoInsetTypesDataTable;
            TechnoInsetColorsBindingSource.DataSource = TechnoInsetColorsDataTable;

            FrontsOrdersDataGrid.DataSource = FrontsOrdersBindingSource;
        }

        private void CreateColumns(bool ShowPrice)
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
            FrontsOrdersDataGrid.AutoGenerateColumns = false;

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

            //убирание лишних столбцов
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
            if (FrontsOrdersDataGrid.Columns.Contains("ImpostMargin"))
                FrontsOrdersDataGrid.Columns["ImpostMargin"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontsOrdersID"].Visible = false;
            FrontsOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["PatinaID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["InsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoProfileID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorID"].Visible = false;
            FrontsOrdersDataGrid.Columns["FactoryID"].Visible = false;
            FrontsOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            FrontsOrdersDataGrid.Columns["Weight"].Visible = false;
            FrontsOrdersDataGrid.Columns["FrontConfigID"].Visible = false;
            FrontsOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            FrontsOrdersDataGrid.Columns["CurrencyCost"].Visible = false;
            FrontsOrdersDataGrid.Columns["CupboardString"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("OriginalInsetPrice"))
                FrontsOrdersDataGrid.Columns["OriginalInsetPrice"].Visible = false;

            if (!Security.PriceAccess || !ShowPrice)
            {
                FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["TotalDiscount"].Visible = false;
                FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
                FrontsOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["OriginalCost"].Visible = false;
                FrontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
                FrontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            }
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

            FrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            foreach (DataGridViewColumn Column in FrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //названия столбцов
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            FrontsOrdersDataGrid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            FrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\nфасад";
            FrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            FrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            FrontsOrdersDataGrid.Columns["OriginalPrice"].HeaderText = "Цена\r\n(оригинал)";
            FrontsOrdersDataGrid.Columns["OriginalCost"].HeaderText = "Стоимость\r\n(оригинал)";
            FrontsOrdersDataGrid.Columns["CostWithTransport"].HeaderText = "Стоимость\r\n(с транспортом)";
            FrontsOrdersDataGrid.Columns["PriceWithTransport"].HeaderText = "Цена\r\n(с транспортом)";
            FrontsOrdersDataGrid.Columns["CurrencyCost"].HeaderText = "Стоимость\r\nв расчете";
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образцы";
            FrontsOrdersDataGrid.Columns["TotalDiscount"].HeaderText = "Общая\r\nскидка, %";

            FrontsOrdersDataGrid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CurrencyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["OriginalPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["OriginalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["CostWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["PriceWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["TotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsOrdersDataGrid.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //FrontsOrdersDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
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

        public void ShowCalculation(bool Show)
        {
            if (FrontsOrdersDataGrid.Columns.Contains("PriceWithTransport"))
                FrontsOrdersDataGrid.Columns["PriceWithTransport"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("CostWithTransport"))
                FrontsOrdersDataGrid.Columns["CostWithTransport"].Visible = false;
            FrontsOrdersDataGrid.Columns["OriginalPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["Cost"].Visible = Show;
            FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = Show;
            FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = Show;
            FrontsOrdersDataGrid.Columns["Square"].Visible = Show;
            FrontsOrdersDataGrid.Columns["Notes"].Visible = !Show;
        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();
            CreateColumns(ShowPrice);
        }

        public bool Filter(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return FrontsOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public void ClearOrders()
        {
            FrontsOrdersDataTable.Clear();
            CurrentMainOrderID = -1;
        }

        public void DeleteOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM FrontsOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }
    }

    public class MainOrdersDecorOrders
    {
        int CurrentClientID = -1;
        int CurrentMainOrderID = -1;
        int SelectedGridIndex = -1;

        private DevExpress.XtraTab.XtraTabControl DecorTabControl = null;

        public Infinium.Modules.Marketing.NewOrders.DecorCatalogOrder DecorCatalogOrder = null;

        public DataTable DecorOrdersDataTable = null;
        public DataTable[] DecorItemOrdersDataTables = null;

        public BindingSource[] DecorItemOrdersBindingSources = null;

        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;

        public PercentageDataGrid[] DecorItemOrdersDataGrids = null;

        public PercentageDataGrid MainOrdersFrontsOrdersDataGrid = null;

        //private ComponentFactory.Krypton.Toolkit.KryptonContextMenu kryptonContextMenu1;
        //private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        //private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem cmiAddToRequest;

        //конструктор
        public MainOrdersDecorOrders(ref DevExpress.XtraTab.XtraTabControl tDecorTabControl,
            ref Infinium.Modules.Marketing.NewOrders.DecorCatalogOrder tDecorCatalogOrder,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
        {
            DecorTabControl = tDecorTabControl;
            DecorCatalogOrder = tDecorCatalogOrder;

            MainOrdersFrontsOrdersDataGrid = tMainOrdersFrontsOrdersDataGrid;
        }

        public int ClientID
        {
            get { return CurrentClientID; }
            set { CurrentClientID = value; }
        }

        private void Create()
        {
            //cmiAddToRequest = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            //cmiAddToRequest.ImageTransparentColor = System.Drawing.Color.Transparent;
            //cmiAddToRequest.Text = "Добавить в заявку";
            //cmiAddToRequest.Click += new System.EventHandler(cmiAddToRequest_Click);

            //kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            //kryptonContextMenuItems1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            //cmiAddToRequest});

            //kryptonContextMenu1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            //kryptonContextMenu1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            //kryptonContextMenuItems1});
            //kryptonContextMenu1.Tag = "18";

            DecorOrdersDataTable = new DataTable();
            DecorItemOrdersDataTables = new DataTable[DecorCatalogOrder.DecorProductsCount];
            DecorItemOrdersBindingSources = new BindingSource[DecorCatalogOrder.DecorProductsCount];
            DecorItemOrdersDataGrids = new PercentageDataGrid[DecorCatalogOrder.DecorProductsCount];

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = new DataTable();
                DecorItemOrdersBindingSources[i] = new BindingSource();
            }
        }

        private void Fill()
        {
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
        }

        private void Binding()
        {

        }

        public void Initialize(bool ShowPrice)
        {
            Create();
            Fill();
            Binding();

            SplitDecorOrdersTables();
            GridSettings(ShowPrice);
        }

        private DataGridViewComboBoxColumn CreateInsetTypesColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypesColumn",
                HeaderText = "Тип\r\nнаполнителя",
                DataPropertyName = "InsetTypeID",

                DataSource = new DataView(DecorCatalogOrder.InsetTypesDataTable),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return ItemColumn;
        }

        private DataGridViewComboBoxColumn CreateInsetColorsColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorsColumn",
                HeaderText = "Цвет\r\nнаполнителя",
                DataPropertyName = "InsetColorID",

                DataSource = new DataView(DecorCatalogOrder.InsetColorsDataTable),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return ItemColumn;
        }

        private DataGridViewComboBoxColumn CreateColorColumn()
        {
            DataGridViewComboBoxColumn ColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",

                DataSource = DecorCatalogOrder.ColorsBindingSource,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return ColorsColumn;
        }

        private DataGridViewComboBoxColumn CreatePatinaColumn()
        {
            DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",

                DataSource = DecorCatalogOrder.PatinaBindingSource,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            return PatinaColumn;
        }

        private DataGridViewComboBoxColumn CreateItemColumn()
        {
            DataGridViewComboBoxColumn ItemColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ItemColumn",
                HeaderText = "Название",
                DataPropertyName = "DecorID",

                DataSource = new DataView(DecorCatalogOrder.DecorDataTable),
                ValueMember = "DecorID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 1
            };
            return ItemColumn;
        }

        private void SplitDecorOrdersTables()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i] = DecorOrdersDataTable.Clone();
                DecorItemOrdersBindingSources[i].DataSource = DecorItemOrdersDataTables[i];
            }
        }

        private void GridSettings(bool ShowPrice)
        {
            DecorTabControl.AppearancePage.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            DecorTabControl.AppearancePage.Header.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            DecorTabControl.AppearancePage.Header.BorderColor = System.Drawing.Color.Black;
            DecorTabControl.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                ((byte)(204)));
            DecorTabControl.AppearancePage.Header.Options.UseBackColor = true;
            DecorTabControl.AppearancePage.Header.Options.UseBorderColor = true;
            DecorTabControl.AppearancePage.Header.Options.UseFont = true;
            DecorTabControl.LookAndFeel.SkinName = "Office 2010 Black";
            DecorTabControl.LookAndFeel.UseDefaultLookAndFeel = false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorTabControl.TabPages.Add(DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString());
                DecorTabControl.TabPages[i].PageVisible = false;
                DecorTabControl.TabPages[i].Text = DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductName"].ToString();

                DecorItemOrdersDataGrids[i] = new PercentageDataGrid()
                {
                    Parent = DecorTabControl.TabPages[i],
                    DataSource = DecorItemOrdersBindingSources[i],
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Black
                };
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = Color.White;
                DecorItemOrdersDataGrids[i].AllowUserToAddRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToDeleteRows = false;
                DecorItemOrdersDataGrids[i].AllowUserToResizeRows = false;
                DecorItemOrdersDataGrids[i].RowHeadersVisible = false;
                DecorItemOrdersDataGrids[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                DecorItemOrdersDataGrids[i].StateCommon.Background.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.Background.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.Background.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateCommon.Background.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Back.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Font = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.DataCell.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Content.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.ColorStyle = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.ColorStyle;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Border.Color1;
                DecorItemOrdersDataGrids[i].RowTemplate.Height = MainOrdersFrontsOrdersDataGrid.RowTemplate.Height;
                DecorItemOrdersDataGrids[i].ColumnHeadersHeight = MainOrdersFrontsOrdersDataGrid.ColumnHeadersHeight;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Back.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Border.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Border.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Font = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Font;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.Color1 = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.Color1;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLine = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLine;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.MultiLineH = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.MultiLineH;
                DecorItemOrdersDataGrids[i].StateCommon.HeaderColumn.Content.TextH = MainOrdersFrontsOrdersDataGrid.StateCommon.HeaderColumn.Content.TextH;
                DecorItemOrdersDataGrids[i].StateSelected.DataCell.Back.Color1 = MainOrdersFrontsOrdersDataGrid.StateSelected.DataCell.Back.Color1;
                DecorItemOrdersDataGrids[i].SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DecorItemOrdersDataGrids[i].SelectedColorStyle = PercentageDataGrid.ColorStyle.Green;
                DecorItemOrdersDataGrids[i].ReadOnly = true;
                DecorItemOrdersDataGrids[i].Tag = i;
                DecorItemOrdersDataGrids[i].UseCustomBackColor = true;
                DecorItemOrdersDataGrids[i].StandardStyle = false;
                DecorItemOrdersDataGrids[i].MultiSelect = true;
                DecorItemOrdersDataGrids[i].CellMouseDown += new DataGridViewCellMouseEventHandler(MainOrdersDecorOrders_CellMouseDown);

                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetColorsColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateInsetTypesColumn());
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateColorColumn());
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreatePatinaColumn());
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns.Add(CreateItemColumn());
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].MinimumWidth = 120;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].MinimumWidth = 60;

                DecorItemOrdersDataGrids[i].Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DecorItemOrdersDataGrids[i].Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //убирание лишних столбцов
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateDateTime"))
                {
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].HeaderText = "Добавлено";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    DecorItemOrdersDataGrids[i].Columns["CreateDateTime"].Width = 100;
                }
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserID"].Visible = false;
                if (DecorItemOrdersDataGrids[i].Columns.Contains("CreateUserTypeID"))
                    DecorItemOrdersDataGrids[i].Columns["CreateUserTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["MainOrderID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ProductID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ColorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["InsetColorID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["DecorConfigID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["FactoryID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["ItemWeight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Weight"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["CurrencyTypeID"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["NeedCalcPrice"].Visible = false;

                if (!Security.PriceAccess)
                {
                    DecorItemOrdersDataGrids[i].Columns["Price"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["Cost"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["OriginalCost"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].Visible = false;
                    DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].Visible = false;
                }

                //русские названия полей

                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].HeaderText = "Цена\r\nначальная";
                DecorItemOrdersDataGrids[i].Columns["Price"].HeaderText = "Цена\r\nконечная";
                DecorItemOrdersDataGrids[i].Columns["Cost"].HeaderText = "Стоимость\r\nконечная";
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].HeaderText = "Цена\r\n(оригинал)";
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].HeaderText = "Стоимость\r\n(оригинал)";
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].HeaderText = "Стоимость\r\n(с транспортом)";
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].HeaderText = "Цена\r\n(с транспортом)";
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].HeaderText = "Общая\r\nскидка, %";
                DecorItemOrdersDataGrids[i].Columns["CurrencyCost"].HeaderText = "Стоимость\r\nв расчете";
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].HeaderText = "Объемный\r\nкоэф-нт";
                DecorItemOrdersDataGrids[i].Columns["IsSample"].HeaderText = "Образцы";

                for (int j = 2; j < DecorItemOrdersDataGrids[i].Columns.Count; j++)
                {
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Height")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Высота";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Length")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Длина";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Width")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Ширина";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Count")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Кол-во";
                    }
                    if (DecorItemOrdersDataGrids[i].Columns[j].HeaderText == "Notes")
                    {
                        DecorItemOrdersDataGrids[i].Columns[j].HeaderText = "Примечание";
                    }
                }

                foreach (DataGridViewColumn Column in DecorItemOrdersDataGrids[i].Columns)
                {
                    Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                DecorItemOrdersDataGrids[i].AutoGenerateColumns = false;
                int DisplayIndex = 0;
                DecorItemOrdersDataGrids[i].Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Length"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Height"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Width"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Count"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["OriginalPrice"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["DiscountVolume"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["TotalDiscount"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Price"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["PriceWithTransport"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["OriginalCost"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["Cost"].DisplayIndex = DisplayIndex++;
                DecorItemOrdersDataGrids[i].Columns["CostWithTransport"].DisplayIndex = DisplayIndex++;
            }
        }

        void MainOrdersDecorOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            SelectedGridIndex = Convert.ToInt32(grid.Tag);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DecorItemOrdersBindingSources[SelectedGridIndex].Position = e.RowIndex;
                //kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        public bool HasRows()
        {
            int ItemsCount = 0;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                for (int r = 0; r < DecorItemOrdersDataTables[i].Rows.Count; r++)
                    if (DecorItemOrdersDataTables[i].Rows[r].RowState != DataRowState.Deleted)
                        ItemsCount += DecorItemOrdersDataTables[i].Rows.Count;
            }

            return ItemsCount > 0;
        }

        private bool ShowTabs()
        {
            int IsOrder = 0;

            for (int i = 0; i < DecorTabControl.TabPages.Count; i++)
            {
                if (DecorItemOrdersDataTables[i].Rows.Count > 0)
                {
                    IsOrder++;
                    DecorTabControl.TabPages[i].PageVisible = true;
                }
                else
                    DecorTabControl.TabPages[i].PageVisible = false;
            }

            if (IsOrder > 0)
                return true;
            else
                return false;
        }

        public void ClearOrders()
        {
            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                for (int r = 0; r < DecorItemOrdersDataTables[i].Rows.Count; r++)
                    DecorItemOrdersDataTables[r].Clear();
            }
            CurrentMainOrderID = -1;
        }

        public bool Filter(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return DecorOrdersDataTable.Rows.Count > 0;

            CurrentMainOrderID = MainOrderID;

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();
            DecorOrdersDataTable.AcceptChanges();

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataTables[i].Clear();
                DecorItemOrdersDataTables[i].AcceptChanges();
                DecorTabControl.TabPages[i].PageVisible = false;
            }

            DecorOrdersCommandBuilder.Dispose();
            DecorOrdersDataAdapter.Dispose();

            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);

            if (DecorOrdersDataTable.Rows.Count == 0)
                return false;

            for (int i = 0; i < DecorCatalogOrder.DecorProductsCount; i++)
            {
                DecorItemOrdersDataGrids[i].Columns["DecorOrderID"].Visible = false;
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalogOrder.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;
                bool ShowColor = false;
                bool ShowPatina = false;
                bool ShowLength = false;
                bool ShowHeight = false;
                bool ShowWidth = false;
                for (int r = 0; r < Rows.Count(); r++)
                {
                    if (!ShowColor)
                        if (Convert.ToInt32(Rows[r]["ColorID"]) != -1)
                            ShowColor = true;
                    if (!ShowPatina)
                        if (Convert.ToInt32(Rows[r]["PatinaID"]) != -1)
                            ShowPatina = true;
                    if (!ShowLength)
                        if (Convert.ToInt32(Rows[r]["Length"]) != -1)
                            ShowLength = true;
                    if (!ShowHeight)
                        if (Convert.ToInt32(Rows[r]["Height"]) != -1)
                            ShowHeight = true;
                    if (!ShowWidth)
                        if (Convert.ToInt32(Rows[r]["Width"]) != -1)
                            ShowWidth = true;
                    DecorItemOrdersDataTables[i].ImportRow(Rows[r]);
                }
                DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Length"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Height"].Visible = false;
                DecorItemOrdersDataGrids[i].Columns["Width"].Visible = false;
                if (ShowColor)
                    DecorItemOrdersDataGrids[i].Columns["ColorsColumn"].Visible = true;
                if (ShowPatina)
                    DecorItemOrdersDataGrids[i].Columns["PatinaColumn"].Visible = true;
                if (ShowLength)
                    DecorItemOrdersDataGrids[i].Columns["Length"].Visible = true;
                if (ShowHeight)
                    DecorItemOrdersDataGrids[i].Columns["Height"].Visible = true;
                if (ShowWidth)
                    DecorItemOrdersDataGrids[i].Columns["Width"].Visible = true;
            }

            ShowTabs();

            return true;
        }

        public void DeleteOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM DecorOrders WHERE MainOrderID = " + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }

        public DataTable WriteOffDecor()
        {
            DataTable DT = DecorOrdersDataTable.Copy();
            DT.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));

            return DT;
        }

        public void DeleteDecorAssignmentByMainOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM DecorAssignments WHERE MainOrderID = " + MainOrderID, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }

        public void AddMegaOrderToDecorAssignment(int MegaOrderID)
        {
            int MainOrderID = 0;
            int DecorOrderID = 0;
            int DecorConfigID = 0;
            int PlanCount = 0;
            int Height = -1;
            int Length = -1;
            int Width = -1;
            string SelectCommand = @"SELECT DecorOrders.FactoryID, DecorOrders.MainOrderID, DecorOrders.DecorConfigID, DecorOrders.DecorOrderID, DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count FROM DecorOrders 
                WHERE DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID=" + MegaOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (DT.Rows[i]["FactoryID"] != DBNull.Value)
                            {
                                if (Convert.ToInt32(DT.Rows[i]["FactoryID"]) == 2)
                                    continue;
                            }
                            if (DT.Rows[i]["MainOrderID"] != DBNull.Value)
                                MainOrderID = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
                            if (DT.Rows[i]["DecorOrderID"] != DBNull.Value)
                                DecorOrderID = Convert.ToInt32(DT.Rows[i]["DecorOrderID"]);
                            if (DT.Rows[i]["DecorConfigID"] != DBNull.Value)
                                DecorConfigID = Convert.ToInt32(DT.Rows[i]["DecorConfigID"]);
                            if (DT.Rows[i]["Count"] != DBNull.Value)
                                PlanCount = Convert.ToInt32(DT.Rows[i]["Count"]);
                            if (DT.Rows[i]["Height"] != DBNull.Value)
                                Height = Convert.ToInt32(DT.Rows[i]["Height"]);
                            if (DT.Rows[i]["Length"] != DBNull.Value)
                                Length = Convert.ToInt32(DT.Rows[i]["Length"]);
                            if (DT.Rows[i]["Width"] != DBNull.Value)
                                Width = Convert.ToInt32(DT.Rows[i]["Width"]);
                            AddToRequest(CurrentClientID, MegaOrderID, MainOrderID, DecorOrderID, DecorConfigID, PlanCount, Height, Length, Width);
                        }
                    }
                }
            }
        }

        private void AddToRequest(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int DecorConfigID, int PlanCount,
            int Height = -1, int Length = -1, int Width = -1)
        {
            int TechStoreSubGroupID = -1;
            int TechStoreID = -1;
            int CoverID = -1;
            decimal MinBalanceOnStorage = 0;
            decimal BalanceOnMainStorage = 0;
            decimal BalanceOnManufactureStorage = 0;

            string SelectCommand = @"SELECT TechStoreSubGroupID, DecorID, ColorID, MinBalanceOnStorage FROM DecorConfig WHERE DecorConfigID=" + DecorConfigID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["TechStoreSubGroupID"] != DBNull.Value)
                            TechStoreSubGroupID = Convert.ToInt32(DT.Rows[0]["TechStoreSubGroupID"]);
                        if (DT.Rows[0]["DecorID"] != DBNull.Value)
                            TechStoreID = Convert.ToInt32(DT.Rows[0]["DecorID"]);
                        if (DT.Rows[0]["ColorID"] != DBNull.Value)
                            CoverID = Convert.ToInt32(DT.Rows[0]["ColorID"]);
                        if (DT.Rows[0]["MinBalanceOnStorage"] != DBNull.Value)
                            MinBalanceOnStorage = Convert.ToInt32(DT.Rows[0]["MinBalanceOnStorage"]);
                    }
                }
            }

            //Проверить основной склад и склад производства 
            //ЕСЛИ ЕСТЬ ОБЛИЦОВКА, ТО ЕЁ УЧИТЫВАТЬ
            //SelectCommand = @"SELECT StoreID, Length, CurrentCount FROM Store WHERE DecorAssignmentID=0 AND CurrentCount > 0 AND StoreItemID=" + TechStoreID + " AND CoverID=" + CoverID;
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        if (DA.Fill(DT) > 0)
            //        {
            //            for (int i = 0; i < DT.Rows.Count; i++)
            //            {
            //                if (DT.Rows[i]["CurrentCount"] != DBNull.Value && DT.Rows[i]["Length"] != DBNull.Value)
            //                    BalanceOnMainStorage += Convert.ToInt32(DT.Rows[i]["Length"]) * Convert.ToInt32(DT.Rows[i]["CurrentCount"]);
            //            }
            //        }
            //    }
            //}
            //SelectCommand = @"SELECT Length, CurrentCount FROM ManufactureStore WHERE DecorAssignmentID=0 AND CurrentCount > 0 AND StoreItemID=" + TechStoreID + " AND CoverID=" + CoverID;
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        if (DA.Fill(DT) > 0)
            //        {
            //            for (int i = 0; i < DT.Rows.Count; i++)
            //            {
            //                if (DT.Rows[i]["CurrentCount"] != DBNull.Value && DT.Rows[i]["Length"] != DBNull.Value)
            //                    BalanceOnManufactureStorage += Convert.ToInt32(DT.Rows[i]["Length"]) * Convert.ToInt32(DT.Rows[i]["CurrentCount"]);
            //            }
            //        }
            //    }
            //}

            int BalanceStatus = 0;
            if (Length > -1)
            {
                if (BalanceOnMainStorage + BalanceOnManufactureStorage - (Length * PlanCount) < 0)
                {
                    BalanceStatus = 0;
                }
                else
                {
                    if (BalanceOnMainStorage + BalanceOnManufactureStorage - (Length * PlanCount) > MinBalanceOnStorage)
                        return;
                    if (BalanceOnMainStorage + BalanceOnManufactureStorage > MinBalanceOnStorage)
                    {
                        if (BalanceOnMainStorage + BalanceOnManufactureStorage - (Length * PlanCount) < MinBalanceOnStorage)
                            BalanceStatus = 1;
                    }
                }
            }
            else
            {
                if (Height > -1 && Width > -1)
                {

                }
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE ClientID=" + ClientID + " AND MainOrderID=" + MainOrderID + " AND DecorOrderID=" + DecorOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count == 0)
                        {
                            int ProductType = 0;

                            if (TechStoreSubGroupID == 30)
                            {
                                ProductType = 1;
                            }
                            if (TechStoreSubGroupID == 22)
                            {
                                ProductType = 2;
                            }
                            if (TechStoreSubGroupID == 91 || TechStoreSubGroupID == 232)
                            {
                                ProductType = 3;
                            }
                            if (TechStoreSubGroupID == 9)
                            {
                                ProductType = 5;
                            }
                            if (ProductType == 0)
                                return;
                            DataRow NewRow = DT.NewRow();
                            NewRow["BalanceStatus"] = BalanceStatus;
                            NewRow["ClientID"] = ClientID;
                            NewRow["MegaOrderID"] = MegaOrderID;
                            NewRow["MainOrderID"] = MainOrderID;
                            NewRow["DecorOrderID"] = DecorOrderID;
                            if (BalanceStatus == 0)
                                NewRow["PlanCount"] = PlanCount;

                            if (TechStoreID != -1)
                                NewRow["TechStoreID2"] = TechStoreID;
                            if (CoverID != -1)
                                NewRow["CoverID2"] = CoverID;
                            if (Height != -1)
                                NewRow["Length2"] = Height;
                            if (Length != -1)
                                NewRow["Length2"] = Length;
                            if (Width != -1)
                                NewRow["Width2"] = Width;
                            NewRow["CreationUserID"] = Security.CurrentUserID;
                            NewRow["CreationDateTime"] = Security.GetCurrentDate();
                            NewRow["ProductType"] = ProductType;

                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        private void cmiAddToRequest_Click(object sender, EventArgs e)
        {
            int MainOrderID = -1;
            int DecorOrderID = -1;
            int DecorConfigID = -1;
            int PlanCount = -1;
            int Height = -1;
            int Length = -1;
            int Width = -1;

            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["MainOrderID"] != DBNull.Value)
                MainOrderID = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["MainOrderID"]);
            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["DecorOrderID"] != DBNull.Value)
                DecorOrderID = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["DecorOrderID"]);
            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["DecorConfigID"] != DBNull.Value)
                DecorConfigID = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["DecorConfigID"]);
            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Count"] != DBNull.Value)
                PlanCount = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Count"]);
            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Height"] != DBNull.Value)
                Height = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Height"]);
            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Length"] != DBNull.Value)
                Length = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Length"]);
            if (((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Width"] != DBNull.Value)
                Width = Convert.ToInt32(((DataRowView)(DecorItemOrdersBindingSources[SelectedGridIndex]).Current).Row["Width"]);
            AddToRequest(CurrentClientID, MainOrderID, DecorOrderID, DecorConfigID, PlanCount, Height, Length, Width);
        }
    }

    public class BatchManager : IAllFrontParameterName
    {
        public bool NewBatch;
        public bool OldBatch;
        public bool CancelMovement;

        public int CurrentMainOrderID = -1;
        public int CurrentMegaOrderID = -1;
        public int CurrentBatchMainOrderID = -1;
        public int CurrentBatchMegaOrderID = -1;
        public int CurrentBatchID = -1;
        public int CurrentMegaBatchID = -1;
        int CurrentFrontID = 0;
        int CurrentFrameColorID = 0;
        int CurrentPatinaID = 0;

        public MainOrdersFrontsOrders MainOrdersFrontsOrders = null;
        public MainOrdersDecorOrders MainOrdersDecorOrders = null;
        public MainOrdersFrontsOrders BatchMainOrdersFrontsOrders = null;
        public MainOrdersDecorOrders BatchMainOrdersDecorOrders = null;

        public PercentageDataGrid BatchDataGrid = null;
        public PercentageDataGrid MegaBatchDataGrid = null;
        public PercentageDataGrid MainOrdersDataGrid = null;
        public PercentageDataGrid MegaOrdersDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl;
        public PercentageDataGrid BatchMainOrdersDataGrid = null;
        public PercentageDataGrid BatchMegaOrdersDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl BatchOrdersTabControl;

        PercentageDataGrid FrontsDataGrid = null;
        PercentageDataGrid FrameColorsDataGrid = null;
        PercentageDataGrid TechnoColorsDataGrid = null;
        PercentageDataGrid InsetTypesDataGrid = null;
        PercentageDataGrid InsetColorsDataGrid = null;
        PercentageDataGrid TechnoInsetTypesDataGrid = null;
        PercentageDataGrid TechnoInsetColorsDataGrid = null;
        PercentageDataGrid SizesDataGrid = null;

        PercentageDataGrid PreFrontsDataGrid = null;
        PercentageDataGrid PreFrameColorsDataGrid = null;
        PercentageDataGrid PreTechnoColorsDataGrid = null;
        PercentageDataGrid PreInsetTypesDataGrid = null;
        PercentageDataGrid PreInsetColorsDataGrid = null;
        PercentageDataGrid PreTechnoInsetTypesDataGrid = null;
        PercentageDataGrid PreTechnoInsetColorsDataGrid = null;
        PercentageDataGrid PreSizesDataGrid = null;

        private PercentageDataGrid DecorProductsDataGrid = null;
        private PercentageDataGrid DecorItemsDataGrid = null;
        private PercentageDataGrid DecorColorsDataGrid = null;
        private PercentageDataGrid DecorSizesDataGrid = null;

        private PercentageDataGrid PreDecorProductsDataGrid = null;
        private PercentageDataGrid PreDecorItemsDataGrid = null;
        private PercentageDataGrid PreDecorColorsDataGrid = null;
        private PercentageDataGrid PreDecorSizesDataGrid = null;

        private DataTable FirmOrderStatusesDataTable = null;
        private DataTable NotInProductionDataTable = null;
        private DataTable InBatchDataTable = null;
        private DataTable BatchDataTable = null;
        private DataTable MegaBatchDataTable = null;
        public DataTable ClientsDataTable = null;
        public DataTable MainOrdersDataTable = null;
        public DataTable BatchMainOrdersDataTable = null;
        private DataTable OrderStatusesDataTable = null;
        private DataTable AgreementStatusesDataTable = null;
        public DataTable ProductionStatusesDataTable = null;
        public DataTable StorageStatusesDataTable = null;
        public DataTable ExpeditionStatusesDataTable = null;
        public DataTable DispatchStatusesDataTable = null;
        public DataTable FactoryTypesDataTable = null;
        public DataTable ClientsMegaOrdersDataTable = null;
        public DataTable MegaOrdersDataTable = null;
        public DataTable BatchMegaOrdersDataTable = null;
        private DataTable BatchDetailsDataTable = null;

        DataTable UsersDT = null;

        DataTable FrontsSummaryDataTable = null;
        DataTable FrameColorsSummaryDataTable = null;
        DataTable TechnoColorsSummaryDataTable = null;
        DataTable InsetTypesSummaryDataTable = null;
        DataTable InsetColorsSummaryDataTable = null;
        DataTable TechnoInsetTypesSummaryDataTable = null;
        DataTable TechnoInsetColorsSummaryDataTable = null;
        DataTable SizesSummaryDataTable = null;

        DataTable PreFrontsSummaryDataTable = null;
        DataTable PreFrameColorsSummaryDataTable = null;
        DataTable PreTechnoColorsSummaryDataTable = null;
        DataTable PreInsetTypesSummaryDataTable = null;
        DataTable PreInsetColorsSummaryDataTable = null;
        DataTable PreTechnoInsetTypesSummaryDataTable = null;
        DataTable PreTechnoInsetColorsSummaryDataTable = null;
        DataTable PreSizesSummaryDataTable = null;

        private DataTable DecorProductsSummaryDataTable = null;
        private DataTable DecorItemsSummaryDataTable = null;
        private DataTable DecorColorsSummaryDataTable = null;
        private DataTable DecorSizesSummaryDataTable = null;
        private DataTable PreDecorProductsSummaryDataTable = null;
        private DataTable PreDecorItemsSummaryDataTable = null;
        private DataTable PreDecorColorsSummaryDataTable = null;
        private DataTable PreDecorSizesSummaryDataTable = null;

        private DataTable FrontsDataTable = null;
        private DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable TechStoreDataTable = null;
        private DataTable ColorsDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable DecorProductsDataTable = null;
        private DataTable DecorDataTable = null;

        public BindingSource BatchBindingSource = null;
        public BindingSource MegaBatchBindingSource = null;
        public BindingSource ClientsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;
        public BindingSource BatchMainOrdersBindingSource = null;
        public BindingSource OrderStatusesBindingSource = null;
        public BindingSource AgreementStatusesBindingSource = null;
        public BindingSource ProductionStatusesBindingSource = null;
        public BindingSource StorageStatusesBindingSource = null;
        public BindingSource ExpeditionStatusesBindingSource = null;
        public BindingSource DispatchStatusesBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource MegaOrdersBindingSource = null;
        public BindingSource BatchMegaOrdersBindingSource = null;
        public BindingSource ClientsMegaOrdersBindingSource = null;

        public BindingSource FrontsSummaryBindingSource = null;
        public BindingSource FrameColorsSummaryBindingSource = null;
        public BindingSource TechnoColorsSummaryBindingSource = null;
        public BindingSource InsetTypesSummaryBindingSource = null;
        public BindingSource InsetColorsSummaryBindingSource = null;
        public BindingSource TechnoInsetTypesSummaryBindingSource = null;
        public BindingSource TechnoInsetColorsSummaryBindingSource = null;
        public BindingSource SizesSummaryBindingSource = null;

        public BindingSource PreFrontsSummaryBindingSource = null;
        public BindingSource PreFrameColorsSummaryBindingSource = null;
        public BindingSource PreTechnoColorsSummaryBindingSource = null;
        public BindingSource PreInsetTypesSummaryBindingSource = null;
        public BindingSource PreInsetColorsSummaryBindingSource = null;
        public BindingSource PreTechnoInsetTypesSummaryBindingSource = null;
        public BindingSource PreTechnoInsetColorsSummaryBindingSource = null;
        public BindingSource PreSizesSummaryBindingSource = null;

        public BindingSource DecorProductsSummaryBindingSource = null;
        public BindingSource DecorItemsSummaryBindingSource = null;
        public BindingSource DecorColorsSummaryBindingSource = null;
        public BindingSource DecorSizesSummaryBindingSource = null;

        public BindingSource PreDecorProductsSummaryBindingSource = null;
        public BindingSource PreDecorItemsSummaryBindingSource = null;
        public BindingSource PreDecorColorsSummaryBindingSource = null;
        public BindingSource PreDecorSizesSummaryBindingSource = null;

        private DataGridViewComboBoxColumn ProfilOrderStatusColumn = null;
        private DataGridViewComboBoxColumn TPSOrderStatusColumn = null;
        private DataGridViewComboBoxColumn ClientsColumn = null;
        private DataGridViewComboBoxColumn OrderStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilProductionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilStorageStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn TPSProductionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSStorageStatusColumn = null;
        private DataGridViewComboBoxColumn TPSExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn MegaFactoryTypeColumn = null;
        private DataGridViewComboBoxColumn AgreementStatusColumn = null;

        private DataGridViewComboBoxColumn BatchProfilOrderStatusColumn = null;
        private DataGridViewComboBoxColumn BatchTPSOrderStatusColumn = null;
        private DataGridViewComboBoxColumn BatchOrderStatusColumn = null;
        private DataGridViewComboBoxColumn BatchProfilProductionStatusColumn = null;
        private DataGridViewComboBoxColumn BatchProfilStorageStatusColumn = null;
        private DataGridViewComboBoxColumn BatchProfilExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn BatchProfilDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn BatchTPSProductionStatusColumn = null;
        private DataGridViewComboBoxColumn BatchTPSStorageStatusColumn = null;
        private DataGridViewComboBoxColumn BatchTPSExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn BatchTPSDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn BatchFactoryTypeColumn = null;
        private DataGridViewComboBoxColumn BatchMegaFactoryTypeColumn = null;
        private DataGridViewComboBoxColumn BatchAgreementStatusColumn = null;

        public BatchManager(ref PercentageDataGrid tMegaBatchDataGrid,
            ref PercentageDataGrid tBatchDataGrid,
            ref PercentageDataGrid tMainOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMegaOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tMainOrdersDecorTabControl,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl,
            ref PercentageDataGrid tBatchMainOrdersDataGrid,
            ref PercentageDataGrid tBatchFrontsOrdersDataGrid,
            ref PercentageDataGrid tBatchMegaOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tBatchDecorTabControl,
            ref DevExpress.XtraTab.XtraTabControl tBatchOrdersTabControl,
            ref Infinium.Modules.Marketing.NewOrders.DecorCatalogOrder DecorCatalogOrder,
            ref PercentageDataGrid tFrontsDataGrid,
            ref PercentageDataGrid tFrameColorsDataGrid,
            ref PercentageDataGrid tTechnoColorsDataGrid,
            ref PercentageDataGrid tInsetTypesDataGrid,
            ref PercentageDataGrid tInsetColorsDataGrid,
            ref PercentageDataGrid tTechnoInsetTypesDataGrid,
            ref PercentageDataGrid tTechnoInsetColorsDataGrid,
            ref PercentageDataGrid tSizesDataGrid,
            ref PercentageDataGrid tDecorProductsDataGrid,
            ref PercentageDataGrid tDecorItemsDataGrid,
            ref PercentageDataGrid tDecorColorsDataGrid,
            ref PercentageDataGrid tDecorSizesDataGrid,
            ref PercentageDataGrid tPreFrontsDataGrid,
            ref PercentageDataGrid tPreFrameColorsDataGrid,
            ref PercentageDataGrid tPreTechnoColorsDataGrid,
            ref PercentageDataGrid tPreInsetTypesDataGrid,
            ref PercentageDataGrid tPreInsetColorsDataGrid,
            ref PercentageDataGrid tPreTechnoInsetTypesDataGrid,
            ref PercentageDataGrid tPreTechnoInsetColorsDataGrid,
            ref PercentageDataGrid tPreSizesDataGrid,
            ref PercentageDataGrid tPreDecorProductsDataGrid,
            ref PercentageDataGrid tPreDecorItemsDataGrid,
            ref PercentageDataGrid tPreDecorColorsDataGrid,
            ref PercentageDataGrid tPreDecorSizesDataGrid)
        {
            BatchDataGrid = tBatchDataGrid;
            MegaBatchDataGrid = tMegaBatchDataGrid;
            MainOrdersDataGrid = tMainOrdersDataGrid;
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            OrdersTabControl = tOrdersTabControl;

            BatchMainOrdersDataGrid = tBatchMainOrdersDataGrid;
            BatchMegaOrdersDataGrid = tBatchMegaOrdersDataGrid;
            BatchOrdersTabControl = tBatchOrdersTabControl;

            FrontsDataGrid = tFrontsDataGrid;
            FrameColorsDataGrid = tFrameColorsDataGrid;
            TechnoColorsDataGrid = tTechnoColorsDataGrid;
            InsetTypesDataGrid = tInsetTypesDataGrid;
            InsetColorsDataGrid = tInsetColorsDataGrid;
            TechnoInsetTypesDataGrid = tTechnoInsetTypesDataGrid;
            TechnoInsetColorsDataGrid = tTechnoInsetColorsDataGrid;
            SizesDataGrid = tSizesDataGrid;

            DecorProductsDataGrid = tDecorProductsDataGrid;
            DecorItemsDataGrid = tDecorItemsDataGrid;
            DecorColorsDataGrid = tDecorColorsDataGrid;
            DecorSizesDataGrid = tDecorSizesDataGrid;

            PreFrontsDataGrid = tPreFrontsDataGrid;
            PreFrameColorsDataGrid = tPreFrameColorsDataGrid;
            PreTechnoColorsDataGrid = tPreTechnoColorsDataGrid;
            PreInsetTypesDataGrid = tPreInsetTypesDataGrid;
            PreInsetColorsDataGrid = tPreInsetColorsDataGrid;
            PreTechnoInsetTypesDataGrid = tPreTechnoInsetTypesDataGrid;
            PreTechnoInsetColorsDataGrid = tPreTechnoInsetColorsDataGrid;
            PreSizesDataGrid = tPreSizesDataGrid;

            PreDecorProductsDataGrid = tPreDecorProductsDataGrid;
            PreDecorItemsDataGrid = tPreDecorItemsDataGrid;
            PreDecorColorsDataGrid = tPreDecorColorsDataGrid;
            PreDecorSizesDataGrid = tPreDecorSizesDataGrid;

            MainOrdersFrontsOrders = new MainOrdersFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);
            MainOrdersFrontsOrders.Initialize(false);

            MainOrdersDecorOrders = new MainOrdersDecorOrders(ref tMainOrdersDecorTabControl,
                ref DecorCatalogOrder, ref tMainOrdersFrontsOrdersDataGrid);
            MainOrdersDecorOrders.Initialize(false);

            BatchMainOrdersFrontsOrders = new MainOrdersFrontsOrders(ref tBatchFrontsOrdersDataGrid);
            BatchMainOrdersFrontsOrders.Initialize(false);

            BatchMainOrdersDecorOrders = new MainOrdersDecorOrders(ref tBatchDecorTabControl,
                ref DecorCatalogOrder, ref tBatchFrontsOrdersDataGrid);
            BatchMainOrdersDecorOrders.Initialize(false);

            Initialize();
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

        private void Create()
        {
            NotInProductionDataTable = new DataTable();
            NotInProductionDataTable.Columns.Add(new DataColumn("MegaOrderID", Type.GetType("System.Int32")));

            FirmOrderStatusesDataTable = new DataTable();
            InBatchDataTable = new DataTable();
            BatchDataTable = new DataTable();
            MegaBatchDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            BatchMainOrdersDataTable = new DataTable();
            BatchMegaOrdersDataTable = new DataTable();
            ClientsMegaOrdersDataTable = new DataTable();
            OrderStatusesDataTable = new DataTable();
            ProductionStatusesDataTable = new DataTable();
            StorageStatusesDataTable = new DataTable();
            ExpeditionStatusesDataTable = new DataTable();
            DispatchStatusesDataTable = new DataTable();
            FactoryTypesDataTable = new DataTable();
            AgreementStatusesDataTable = new DataTable();

            BatchDetailsDataTable = new DataTable();

            MegaBatchBindingSource = new BindingSource();
            BatchBindingSource = new BindingSource();
            ClientsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();
            BatchMainOrdersBindingSource = new BindingSource();
            BatchMegaOrdersBindingSource = new BindingSource();
            OrderStatusesBindingSource = new BindingSource();
            ProductionStatusesBindingSource = new BindingSource();
            StorageStatusesBindingSource = new BindingSource();
            ExpeditionStatusesBindingSource = new BindingSource();
            DispatchStatusesBindingSource = new BindingSource();
            FactoryTypesBindingSource = new BindingSource();
            ClientsMegaOrdersBindingSource = new BindingSource();
            AgreementStatusesBindingSource = new BindingSource();

            FrontsSummaryBindingSource = new BindingSource();
            FrameColorsSummaryBindingSource = new BindingSource();
            TechnoColorsSummaryBindingSource = new BindingSource();
            InsetTypesSummaryBindingSource = new BindingSource();
            InsetColorsSummaryBindingSource = new BindingSource();
            TechnoInsetTypesSummaryBindingSource = new BindingSource();
            TechnoInsetColorsSummaryBindingSource = new BindingSource();
            SizesSummaryBindingSource = new BindingSource();

            DecorProductsSummaryBindingSource = new BindingSource();
            DecorItemsSummaryBindingSource = new BindingSource();
            DecorColorsSummaryBindingSource = new BindingSource();
            DecorSizesSummaryBindingSource = new BindingSource();

            PreFrontsSummaryBindingSource = new BindingSource();
            PreFrameColorsSummaryBindingSource = new BindingSource();
            PreTechnoColorsSummaryBindingSource = new BindingSource();
            PreInsetTypesSummaryBindingSource = new BindingSource();
            PreInsetColorsSummaryBindingSource = new BindingSource();
            PreTechnoInsetTypesSummaryBindingSource = new BindingSource();
            PreTechnoInsetColorsSummaryBindingSource = new BindingSource();
            PreSizesSummaryBindingSource = new BindingSource();

            PreDecorProductsSummaryBindingSource = new BindingSource();
            PreDecorItemsSummaryBindingSource = new BindingSource();
            PreDecorColorsSummaryBindingSource = new BindingSource();
            PreDecorSizesSummaryBindingSource = new BindingSource();

            FrontsSummaryDataTable = new DataTable();
            FrontsSummaryDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            FrontsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            FrameColorsSummaryDataTable = new DataTable();
            FrameColorsSummaryDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrameColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            FrameColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            FrameColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            FrameColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrameColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            TechnoColorsSummaryDataTable = new DataTable();
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColor"), System.Type.GetType("System.String")));
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            TechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            InsetTypesSummaryDataTable = new DataTable();
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            InsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            InsetColorsSummaryDataTable = new DataTable();
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            InsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            TechnoInsetTypesSummaryDataTable = new DataTable();
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetType"), System.Type.GetType("System.String")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetTypeID"), System.Type.GetType("System.Int32")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            TechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            TechnoInsetColorsSummaryDataTable = new DataTable();
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetColor"), System.Type.GetType("System.String")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetTypeID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetColorID"), System.Type.GetType("System.Int32")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            TechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            SizesSummaryDataTable = new DataTable();
            SizesSummaryDataTable.Columns.Add(new DataColumn(("Size"), System.Type.GetType("System.String")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetTypeID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetColorID"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            SizesSummaryDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));

            PreFrontsSummaryDataTable = new DataTable();
            PreFrontsSummaryDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            PreFrontsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreFrontsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreFrontsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreFrameColorsSummaryDataTable = new DataTable();
            PreFrameColorsSummaryDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            PreFrameColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreFrameColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreFrameColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreFrameColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreFrameColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreTechnoColorsSummaryDataTable = new DataTable();
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColor"), System.Type.GetType("System.String")));
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreTechnoColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreInsetTypesSummaryDataTable = new DataTable();
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreInsetColorsSummaryDataTable = new DataTable();
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreTechnoInsetTypesSummaryDataTable = new DataTable();
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetType"), System.Type.GetType("System.String")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetTypeID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreTechnoInsetTypesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreTechnoInsetColorsSummaryDataTable = new DataTable();
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetColor"), System.Type.GetType("System.String")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetTypeID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetColorID"), System.Type.GetType("System.Int32")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreTechnoInsetColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));

            PreSizesSummaryDataTable = new DataTable();
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("Size"), System.Type.GetType("System.String")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("PatinaID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("TechnoColorID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("InsetColorID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetTypeID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("TechnoInsetColorID"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            PreSizesSummaryDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));

            DecorProductsSummaryDataTable = new DataTable();
            DecorProductsSummaryDataTable.Columns.Add(new DataColumn(("DecorProduct"), System.Type.GetType("System.String")));
            DecorProductsSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            DecorProductsSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            DecorProductsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            DecorProductsSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            DecorItemsSummaryDataTable = new DataTable();
            DecorItemsSummaryDataTable.Columns.Add(new DataColumn(("DecorItem"), System.Type.GetType("System.String")));
            DecorItemsSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            DecorItemsSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            DecorItemsSummaryDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            DecorItemsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            DecorItemsSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            DecorColorsSummaryDataTable = new DataTable();
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            DecorColorsSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            DecorSizesSummaryDataTable = new DataTable();
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Size"), System.Type.GetType("System.String")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Length"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            DecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            PreDecorProductsSummaryDataTable = new DataTable();
            PreDecorProductsSummaryDataTable.Columns.Add(new DataColumn(("DecorProduct"), System.Type.GetType("System.String")));
            PreDecorProductsSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            PreDecorProductsSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            PreDecorProductsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            PreDecorProductsSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            PreDecorItemsSummaryDataTable = new DataTable();
            PreDecorItemsSummaryDataTable.Columns.Add(new DataColumn(("DecorItem"), System.Type.GetType("System.String")));
            PreDecorItemsSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            PreDecorItemsSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            PreDecorItemsSummaryDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            PreDecorItemsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            PreDecorItemsSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            PreDecorColorsSummaryDataTable = new DataTable();
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            PreDecorColorsSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            PreDecorSizesSummaryDataTable = new DataTable();
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("ColorID"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Size"), System.Type.GetType("System.String")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Length"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            PreDecorSizesSummaryDataTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
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
            GetColorsDT();
            GetInsetColorsDT();
            UsersDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ShortName FROM Users",
                ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            FrontsDataTable = new DataTable();

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }

            PatinaDataTable = new DataTable();
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
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }

            TechStoreDataTable = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStore",
            //    ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(TechStoreDataTable);
            //}
            TechStoreDataTable = TablesManager.TechStoreDataTable;

            SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1  ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM BatchDetails", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(InBatchDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MegaBatch", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MegaBatchDataTable);
            }
            MegaBatchDataTable.Columns.Add(new DataColumn(("WeekNumber"), System.Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM Batch", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 MainOrders.*, BatchDetails.BatchID FROM MainOrders" +
                " LEFT JOIN BatchDetails ON MainOrders.MainOrderID = BatchDetails.MainOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchMainOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " ORDER BY ClientName, OrderNumber", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchMegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " ORDER BY ClientName, OrderNumber", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FirmOrderStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(FirmOrderStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MegaOrders", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(ClientsMegaOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OrderStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(OrderStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM AgreementStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(AgreementStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ExpeditionStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ExpeditionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDataTable);
            }

            FactoryTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDataTable);
            }

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID," +
                " BatchDetails.MainOrderID, MainOrders.MegaOrderID FROM BatchDetails" +
                " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID" +
                " ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDataTable);
            }

            MainOrdersDataTable.Columns.Add(new DataColumn("BatchNumber", Type.GetType("System.String")));
        }

        private void Binding()
        {
            BatchBindingSource.DataSource = BatchDataTable;
            BatchDataGrid.DataSource = BatchBindingSource;

            MegaBatchBindingSource.DataSource = MegaBatchDataTable;
            MegaBatchDataGrid.DataSource = MegaBatchBindingSource;

            ClientsBindingSource.DataSource = ClientsDataTable;

            ProductionStatusesBindingSource.DataSource = ProductionStatusesDataTable;
            StorageStatusesBindingSource.DataSource = StorageStatusesDataTable;
            ExpeditionStatusesBindingSource.DataSource = ExpeditionStatusesDataTable;
            DispatchStatusesBindingSource.DataSource = DispatchStatusesDataTable;
            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;

            OrderStatusesBindingSource.DataSource = OrderStatusesDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
            MainOrdersBindingSource.DataSource = MainOrdersDataTable;
            BatchMegaOrdersBindingSource.DataSource = BatchMegaOrdersDataTable;
            BatchMainOrdersBindingSource.DataSource = BatchMainOrdersDataTable;

            BatchDataGrid.DataSource = BatchBindingSource;
            MainOrdersDataGrid.DataSource = MainOrdersBindingSource;
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;
            BatchMainOrdersDataGrid.DataSource = BatchMainOrdersBindingSource;
            BatchMegaOrdersDataGrid.DataSource = BatchMegaOrdersBindingSource;

            ClientsMegaOrdersBindingSource.DataSource = ClientsMegaOrdersDataTable;

            AgreementStatusesBindingSource.DataSource = AgreementStatusesDataTable;

            FrontsSummaryBindingSource.DataSource = FrontsSummaryDataTable;
            FrontsDataGrid.DataSource = FrontsSummaryBindingSource;

            FrameColorsSummaryBindingSource.DataSource = FrameColorsSummaryDataTable;
            FrameColorsDataGrid.DataSource = FrameColorsSummaryBindingSource;

            TechnoColorsSummaryBindingSource.DataSource = TechnoColorsSummaryDataTable;
            TechnoColorsDataGrid.DataSource = TechnoColorsSummaryBindingSource;

            InsetTypesSummaryBindingSource.DataSource = InsetTypesSummaryDataTable;
            InsetTypesDataGrid.DataSource = InsetTypesSummaryBindingSource;

            InsetColorsSummaryBindingSource.DataSource = InsetColorsSummaryDataTable;
            InsetColorsDataGrid.DataSource = InsetColorsSummaryBindingSource;

            TechnoInsetTypesSummaryBindingSource.DataSource = TechnoInsetTypesSummaryDataTable;
            TechnoInsetTypesDataGrid.DataSource = TechnoInsetTypesSummaryBindingSource;

            TechnoInsetColorsSummaryBindingSource.DataSource = TechnoInsetColorsSummaryDataTable;
            TechnoInsetColorsDataGrid.DataSource = TechnoInsetColorsSummaryBindingSource;

            SizesSummaryBindingSource.DataSource = SizesSummaryDataTable;
            SizesDataGrid.DataSource = SizesSummaryBindingSource;

            PreFrontsSummaryBindingSource.DataSource = PreFrontsSummaryDataTable;
            PreFrontsDataGrid.DataSource = PreFrontsSummaryBindingSource;

            PreFrameColorsSummaryBindingSource.DataSource = PreFrameColorsSummaryDataTable;
            PreFrameColorsDataGrid.DataSource = PreFrameColorsSummaryBindingSource;

            PreTechnoColorsSummaryBindingSource.DataSource = PreTechnoColorsSummaryDataTable;
            PreTechnoColorsDataGrid.DataSource = PreTechnoColorsSummaryBindingSource;

            PreInsetTypesSummaryBindingSource.DataSource = PreInsetTypesSummaryDataTable;
            PreInsetTypesDataGrid.DataSource = PreInsetTypesSummaryBindingSource;

            PreInsetColorsSummaryBindingSource.DataSource = PreInsetColorsSummaryDataTable;
            PreInsetColorsDataGrid.DataSource = PreInsetColorsSummaryBindingSource;

            PreTechnoInsetTypesSummaryBindingSource.DataSource = PreTechnoInsetTypesSummaryDataTable;
            PreTechnoInsetTypesDataGrid.DataSource = PreTechnoInsetTypesSummaryBindingSource;

            PreTechnoInsetColorsSummaryBindingSource.DataSource = PreTechnoInsetColorsSummaryDataTable;
            PreTechnoInsetColorsDataGrid.DataSource = PreTechnoInsetColorsSummaryBindingSource;

            PreSizesSummaryBindingSource.DataSource = PreSizesSummaryDataTable;
            PreSizesDataGrid.DataSource = PreSizesSummaryBindingSource;

            DecorProductsSummaryBindingSource.DataSource = DecorProductsSummaryDataTable;
            DecorProductsDataGrid.DataSource = DecorProductsSummaryBindingSource;

            DecorItemsSummaryBindingSource.DataSource = DecorItemsSummaryDataTable;
            DecorItemsDataGrid.DataSource = DecorItemsSummaryBindingSource;

            DecorColorsSummaryBindingSource.DataSource = DecorColorsSummaryDataTable;
            DecorColorsDataGrid.DataSource = DecorColorsSummaryBindingSource;

            DecorSizesSummaryBindingSource.DataSource = DecorSizesSummaryDataTable;
            DecorSizesDataGrid.DataSource = DecorSizesSummaryBindingSource;

            PreDecorProductsSummaryBindingSource.DataSource = PreDecorProductsSummaryDataTable;
            PreDecorProductsDataGrid.DataSource = PreDecorProductsSummaryBindingSource;

            PreDecorItemsSummaryBindingSource.DataSource = PreDecorItemsSummaryDataTable;
            PreDecorItemsDataGrid.DataSource = PreDecorItemsSummaryBindingSource;

            PreDecorColorsSummaryBindingSource.DataSource = PreDecorColorsSummaryDataTable;
            PreDecorColorsDataGrid.DataSource = PreDecorColorsSummaryBindingSource;

            PreDecorSizesSummaryBindingSource.DataSource = PreDecorSizesSummaryDataTable;
            PreDecorSizesDataGrid.DataSource = PreDecorSizesSummaryBindingSource;

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            SetMegaBatchEnable();
            CreateColumns();
            CreateBatchColumns();

            ProductsGridSetting(
                ref FrontsDataGrid,
                ref FrameColorsDataGrid,
                ref TechnoColorsDataGrid,
                ref InsetTypesDataGrid,
                ref InsetColorsDataGrid,
                ref TechnoInsetTypesDataGrid,
                ref TechnoInsetColorsDataGrid,
                ref SizesDataGrid,
                ref DecorProductsDataGrid,
                ref DecorItemsDataGrid,
                ref DecorColorsDataGrid,
                ref DecorSizesDataGrid);

            ProductsGridSetting(
                ref PreFrontsDataGrid,
                ref PreFrameColorsDataGrid,
                ref PreTechnoColorsDataGrid,
                ref PreInsetTypesDataGrid,
                ref PreInsetColorsDataGrid,
                ref PreTechnoInsetTypesDataGrid,
                ref PreTechnoInsetColorsDataGrid,
                ref PreSizesDataGrid,
                ref PreDecorProductsDataGrid,
                ref PreDecorItemsDataGrid,
                ref PreDecorColorsDataGrid,
                ref PreDecorSizesDataGrid);

            BatchGridSetting();
            MegaBatchGridSetting();
            MainGridSetting(ref MainOrdersDataGrid);
            MegaGridSetting(ref MegaOrdersDataGrid);
            MainGridSetting(ref BatchMainOrdersDataGrid);
            MegaGridSetting(ref BatchMegaOrdersDataGrid);
            ShowMainOrdersColumns(ref MainOrdersDataGrid, 1);
            ShowMainOrdersColumns(ref BatchMainOrdersDataGrid, 1);
            ShowMegaOrdersColumns(ref MegaOrdersDataGrid, 1);
            ShowMegaOrdersColumns(ref BatchMegaOrdersDataGrid, 1);
        }

        #region Grid settings

        public DataGridViewComboBoxColumn ProfilConfirmUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ProfilConfirmUserColumn",
                    HeaderText = "Утвердил",
                    DataPropertyName = "ProfilConfirmUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TPSConfirmUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TPSConfirmUserColumn",
                    HeaderText = "Утвердил",
                    DataPropertyName = "TPSConfirmUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ProfilCloseUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ProfilCloseUserColumn",
                    HeaderText = "Закрыл",
                    DataPropertyName = "ProfilCloseUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TPSCloseUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TPSCloseUserColumn",
                    HeaderText = "Закрыл",
                    DataPropertyName = "TPSCloseUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewImageColumn ProfilEnabledColumn
        {
            get
            {
                DataGridViewImageColumn Column = new DataGridViewImageColumn()
                {
                    Name = "ProfilEnabledColumn",
                    HeaderText = string.Empty,
                    DataPropertyName = "ProfilEnabledImage",
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewImageColumn TPSEnabledColumn
        {
            get
            {
                DataGridViewImageColumn Column = new DataGridViewImageColumn()
                {
                    Name = "TPSEnabledColumn",
                    HeaderText = string.Empty,
                    DataPropertyName = "TPSEnabledImage",
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        private void CreateColumns()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn BestBeforeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                CalendarTodayDate = DateTime.Now,
                Checked = false,
                DataPropertyName = "BestBefore",
                HeaderText = "Срок годности",
                Name = "BestBeforeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
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
            OrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "OrderStatusColumn",
                HeaderText = "Статус заказа",
                DataPropertyName = "OrderStatusID",
                DataSource = new DataView(OrderStatusesDataTable),
                ValueMember = "OrderStatusID",
                DisplayMember = "OrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            AgreementStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "AgreementStatusColumn",
                HeaderText = "Статус\r\nсогласования",
                DataPropertyName = "AgreementStatusID",
                DataSource = new DataView(AgreementStatusesDataTable),
                ValueMember = "AgreementStatusID",
                DisplayMember = "AgreementStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilOrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilOrderStatusColumn",
                HeaderText = "Статус заказа\n\rПрофиль",
                DataPropertyName = "ProfilOrderStatusID",
                DataSource = new DataView(FirmOrderStatusesDataTable),
                ValueMember = "FirmOrderStatusID",
                DisplayMember = "FirmOrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilProductionStatusColumn",
                HeaderText = "Пр-во\n\rПрофиль",
                DataPropertyName = "ProfilProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilStorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilStorageStatusColumn",
                HeaderText = "Cклад\r\nПрофиль",
                DataPropertyName = "ProfilStorageStatusID",
                DataSource = new DataView(StorageStatusesDataTable),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilExpeditionStatusColumn",
                HeaderText = "Экспедиция\r\nПрофиль",
                DataPropertyName = "ProfilExpeditionStatusID",
                DataSource = new DataView(ExpeditionStatusesDataTable),
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nПрофиль",
                DataPropertyName = "ProfilDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSOrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSOrderStatusColumn",
                HeaderText = "Статус\n\rзаказа ТПС",
                DataPropertyName = "TPSOrderStatusID",
                DataSource = new DataView(FirmOrderStatusesDataTable),
                ValueMember = "FirmOrderStatusID",
                DisplayMember = "FirmOrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSProductionStatusColumn",
                HeaderText = "Пр-во\n\rТПС",
                DataPropertyName = "TPSProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSStorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSStorageStatusColumn",
                HeaderText = "Склад\r\nТПС",
                DataPropertyName = "TPSStorageStatusID",
                DataSource = new DataView(StorageStatusesDataTable),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSExpeditionStatusColumn",
                HeaderText = "Экспедиция\r\nТПС",
                DataPropertyName = "TPSExpeditionStatusID",
                DataSource = new DataView(ExpeditionStatusesDataTable),
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nТПС",
                DataPropertyName = "TPSDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            FactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryTypeColumn",
                HeaderText = "Тип\n\rпр-ва",
                DataPropertyName = "FactoryID",
                DataSource = new DataView(FactoryTypesDataTable),
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MegaFactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "MegaFactoryTypeColumn",
                HeaderText = "Тип\n\rпр-ва",
                DataPropertyName = "FactoryID",
                DataSource = new DataView(FactoryTypesDataTable),
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MegaOrdersDataGrid.Columns.Add(ProfilOrderStatusColumn);
            MegaOrdersDataGrid.Columns.Add(TPSOrderStatusColumn);

            MegaOrdersDataGrid.Columns.Add(OrderStatusColumn);
            MegaOrdersDataGrid.Columns.Add(AgreementStatusColumn);
            MegaOrdersDataGrid.Columns.Add(MegaFactoryTypeColumn);

            MainOrdersDataGrid.Columns.Add(ProfilProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(FactoryTypeColumn);
        }

        private void CreateBatchColumns()
        {
            BatchProfilOrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilOrderStatusColumn",
                HeaderText = "Статус заказа\n\rПрофиль",
                DataPropertyName = "ProfilOrderStatusID",
                DataSource = new DataView(FirmOrderStatusesDataTable),
                ValueMember = "FirmOrderStatusID",
                DisplayMember = "FirmOrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchOrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "OrderStatusColumn",
                HeaderText = "Статус заказа",
                DataPropertyName = "OrderStatusID",
                DataSource = new DataView(OrderStatusesDataTable),
                ValueMember = "OrderStatusID",
                DisplayMember = "OrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchAgreementStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "AgreementStatusColumn",
                HeaderText = "Статус\r\nсогласования",
                DataPropertyName = "AgreementStatusID",
                DataSource = new DataView(AgreementStatusesDataTable),
                ValueMember = "AgreementStatusID",
                DisplayMember = "AgreementStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchProfilProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilProductionStatusColumn",
                HeaderText = "Пр-во\n\rПрофиль",
                DataPropertyName = "ProfilProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchProfilStorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilStorageStatusColumn",
                HeaderText = "Cклад\r\nПрофиль",
                DataPropertyName = "ProfilStorageStatusID",
                DataSource = new DataView(StorageStatusesDataTable),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchProfilExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilExpeditionStatusColumn",
                HeaderText = "Экспедиция\r\nПрофиль",
                DataPropertyName = "ProfilExpeditionStatusID",
                DataSource = new DataView(ExpeditionStatusesDataTable),
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchProfilDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nПрофиль",
                DataPropertyName = "ProfilDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchTPSOrderStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSOrderStatusColumn",
                HeaderText = "Статус\n\rзаказа ТПС",
                DataPropertyName = "TPSOrderStatusID",
                DataSource = new DataView(FirmOrderStatusesDataTable),
                ValueMember = "FirmOrderStatusID",
                DisplayMember = "FirmOrderStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchTPSProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSProductionStatusColumn",
                HeaderText = "Пр-во\n\rТПС",
                DataPropertyName = "TPSProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchTPSStorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSStorageStatusColumn",
                HeaderText = "Склад\r\nТПС",
                DataPropertyName = "TPSStorageStatusID",
                DataSource = new DataView(StorageStatusesDataTable),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchTPSExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSExpeditionStatusColumn",
                HeaderText = "Экспедиция\r\nТПС",
                DataPropertyName = "TPSExpeditionStatusID",
                DataSource = new DataView(ExpeditionStatusesDataTable),
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchTPSDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nТПС",
                DataPropertyName = "TPSDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchFactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryTypeColumn",
                HeaderText = "Тип\n\rпр-ва",
                DataPropertyName = "FactoryID",
                DataSource = new DataView(FactoryTypesDataTable),
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchMegaFactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "MegaFactoryTypeColumn",
                HeaderText = "Тип\n\rпр-ва",
                DataPropertyName = "FactoryID",
                DataSource = new DataView(FactoryTypesDataTable),
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            BatchMegaOrdersDataGrid.Columns.Add(BatchMegaFactoryTypeColumn);
            BatchMegaOrdersDataGrid.Columns.Add(BatchProfilOrderStatusColumn);
            BatchMegaOrdersDataGrid.Columns.Add(BatchTPSOrderStatusColumn);
            BatchMegaOrdersDataGrid.Columns.Add(BatchOrderStatusColumn);
            BatchMegaOrdersDataGrid.Columns.Add(BatchAgreementStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchProfilProductionStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchProfilStorageStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchProfilExpeditionStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchProfilDispatchStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchTPSProductionStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchTPSStorageStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchTPSExpeditionStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchTPSDispatchStatusColumn);
            BatchMainOrdersDataGrid.Columns.Add(BatchFactoryTypeColumn);
        }

        private void ShowMainOrdersColumns(ref PercentageDataGrid tPercentageDataGrid, int FactoryID)
        {
            if (FactoryID == 0)
            {
                tPercentageDataGrid.Columns["ProfilProductionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilStorageStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilExpeditionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilDispatchStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilProductionDate"].Visible = true;
                tPercentageDataGrid.Columns["ProfilOnProductionDate"].Visible = true;

                tPercentageDataGrid.Columns["TPSProductionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSStorageStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSExpeditionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSDispatchStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSProductionDate"].Visible = true;
                tPercentageDataGrid.Columns["TPSOnProductionDate"].Visible = true;
            }

            if (FactoryID == 1)
            {
                tPercentageDataGrid.Columns["ProfilProductionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilStorageStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilExpeditionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilDispatchStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["ProfilProductionDate"].Visible = true;
                tPercentageDataGrid.Columns["ProfilOnProductionDate"].Visible = true;

                tPercentageDataGrid.Columns["TPSProductionStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["TPSStorageStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["TPSExpeditionStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["TPSDispatchStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["TPSProductionDate"].Visible = false;
                tPercentageDataGrid.Columns["TPSOnProductionDate"].Visible = false;
            }

            if (FactoryID == 2)
            {
                tPercentageDataGrid.Columns["ProfilProductionStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["ProfilStorageStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["ProfilExpeditionStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["ProfilDispatchStatusColumn"].Visible = false;
                tPercentageDataGrid.Columns["ProfilProductionDate"].Visible = false;
                tPercentageDataGrid.Columns["ProfilOnProductionDate"].Visible = false;

                tPercentageDataGrid.Columns["TPSProductionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSStorageStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSExpeditionStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSDispatchStatusColumn"].Visible = true;
                tPercentageDataGrid.Columns["TPSProductionDate"].Visible = true;
                tPercentageDataGrid.Columns["TPSOnProductionDate"].Visible = true;
            }
        }

        private void ShowMegaOrdersColumns(ref PercentageDataGrid tPercentageDataGrid, int FactoryID)
        {
            if (FactoryID == 0)
            {
                tPercentageDataGrid.Columns["ProfilDispatchDate"].Visible = true;
                tPercentageDataGrid.Columns["ProfilOrderStatusColumn"].Visible = true;

                tPercentageDataGrid.Columns["TPSDispatchDate"].Visible = true;
                tPercentageDataGrid.Columns["TPSOrderStatusColumn"].Visible = true;
            }

            if (FactoryID == 1)
            {
                tPercentageDataGrid.Columns["ProfilDispatchDate"].Visible = true;
                tPercentageDataGrid.Columns["ProfilOrderStatusColumn"].Visible = true;

                tPercentageDataGrid.Columns["TPSDispatchDate"].Visible = false;
                tPercentageDataGrid.Columns["TPSOrderStatusColumn"].Visible = false;
            }

            if (FactoryID == 2)
            {
                tPercentageDataGrid.Columns["ProfilDispatchDate"].Visible = false;
                tPercentageDataGrid.Columns["ProfilOrderStatusColumn"].Visible = false;

                tPercentageDataGrid.Columns["TPSDispatchDate"].Visible = true;
                tPercentageDataGrid.Columns["TPSOrderStatusColumn"].Visible = true;
            }
        }

        private void ProductsGridSetting(
            ref PercentageDataGrid tFrontsDataGrid,
            ref PercentageDataGrid FrameColorsDataGrid,
            ref PercentageDataGrid TechnoColorsDataGrid,
            ref PercentageDataGrid InsetTypesDataGrid,
            ref PercentageDataGrid InsetColorsDataGrid,
            ref PercentageDataGrid TechnoInsetTypesDataGrid,
            ref PercentageDataGrid TechnoInsetColorsDataGrid,
            ref PercentageDataGrid SizesDataGrid,
            ref PercentageDataGrid tDecorProductsDataGrid,
            ref PercentageDataGrid tDecorItemsDataGrid,
            ref PercentageDataGrid tDecorColorsDataGrid,
            ref PercentageDataGrid tDecorSizesDataGrid)
        {
            tFrontsDataGrid.ColumnHeadersHeight = 38;
            FrameColorsDataGrid.ColumnHeadersHeight = 38;
            TechnoColorsDataGrid.ColumnHeadersHeight = 38;
            InsetTypesDataGrid.ColumnHeadersHeight = 38;
            InsetColorsDataGrid.ColumnHeadersHeight = 38;
            TechnoInsetTypesDataGrid.ColumnHeadersHeight = 38;
            TechnoInsetColorsDataGrid.ColumnHeadersHeight = 38;
            SizesDataGrid.ColumnHeadersHeight = 38;

            tDecorProductsDataGrid.ColumnHeadersHeight = 38;
            tDecorItemsDataGrid.ColumnHeadersHeight = 38;
            tDecorColorsDataGrid.ColumnHeadersHeight = 38;
            tDecorSizesDataGrid.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in tFrontsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FrameColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in TechnoColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in InsetTypesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in InsetColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in TechnoInsetTypesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in TechnoInsetColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in SizesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }


            foreach (DataGridViewColumn Column in tDecorProductsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in tDecorItemsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in tDecorColorsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in tDecorSizesDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            tFrontsDataGrid.Columns["FrontID"].Visible = false;

            FrameColorsDataGrid.Columns["FrontID"].Visible = false;
            FrameColorsDataGrid.Columns["ColorID"].Visible = false;
            FrameColorsDataGrid.Columns["PatinaID"].Visible = false;

            TechnoColorsDataGrid.Columns["FrontID"].Visible = false;
            TechnoColorsDataGrid.Columns["ColorID"].Visible = false;
            TechnoColorsDataGrid.Columns["TechnoColorID"].Visible = false;
            TechnoColorsDataGrid.Columns["PatinaID"].Visible = false;

            InsetTypesDataGrid.Columns["FrontID"].Visible = false;
            InsetTypesDataGrid.Columns["PatinaID"].Visible = false;
            InsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            InsetTypesDataGrid.Columns["ColorID"].Visible = false;
            InsetTypesDataGrid.Columns["TechnoColorID"].Visible = false;

            InsetColorsDataGrid.Columns["FrontID"].Visible = false;
            InsetColorsDataGrid.Columns["InsetTypeID"].Visible = false;
            InsetColorsDataGrid.Columns["PatinaID"].Visible = false;
            InsetColorsDataGrid.Columns["ColorID"].Visible = false;
            InsetColorsDataGrid.Columns["TechnoColorID"].Visible = false;
            InsetColorsDataGrid.Columns["InsetColorID"].Visible = false;

            TechnoInsetTypesDataGrid.Columns["FrontID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["PatinaID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["InsetTypeID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["InsetColorID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["ColorID"].Visible = false;
            TechnoInsetTypesDataGrid.Columns["TechnoColorID"].Visible = false;

            TechnoInsetColorsDataGrid.Columns["FrontID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["PatinaID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["ColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["TechnoColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["InsetTypeID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["InsetColorID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            TechnoInsetColorsDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            SizesDataGrid.Columns["FrontID"].Visible = false;
            SizesDataGrid.Columns["PatinaID"].Visible = false;
            SizesDataGrid.Columns["Height"].Visible = false;
            SizesDataGrid.Columns["Width"].Visible = false;
            SizesDataGrid.Columns["ColorID"].Visible = false;
            SizesDataGrid.Columns["TechnoColorID"].Visible = false;
            SizesDataGrid.Columns["InsetColorID"].Visible = false;
            SizesDataGrid.Columns["InsetTypeID"].Visible = false;
            SizesDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            SizesDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            tFrontsDataGrid.Columns["Front"].HeaderText = "Фасад";
            tFrontsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            tFrontsDataGrid.Columns["Count"].HeaderText = "шт.";

            FrameColorsDataGrid.Columns["FrameColor"].HeaderText = "Цвет профиля";
            FrameColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            FrameColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            TechnoColorsDataGrid.Columns["TechnoColor"].HeaderText = "Цвет профиля-2";
            TechnoColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            TechnoColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            SizesDataGrid.Columns["Size"].HeaderText = "Размер";
            SizesDataGrid.Columns["Square"].HeaderText = "м.кв.";
            SizesDataGrid.Columns["Count"].HeaderText = "шт.";

            InsetTypesDataGrid.Columns["InsetType"].HeaderText = "Тип наполнителя";
            InsetTypesDataGrid.Columns["Square"].HeaderText = "м.кв.";
            InsetTypesDataGrid.Columns["Count"].HeaderText = "шт.";

            InsetColorsDataGrid.Columns["InsetColor"].HeaderText = "Цвет наполнителя";
            InsetColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            InsetColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            TechnoInsetTypesDataGrid.Columns["TechnoInsetType"].HeaderText = "Тип наполнителя-2";
            TechnoInsetTypesDataGrid.Columns["Square"].HeaderText = "м.кв.";
            TechnoInsetTypesDataGrid.Columns["Count"].HeaderText = "шт.";

            TechnoInsetColorsDataGrid.Columns["TechnoInsetColor"].HeaderText = "Цвет наполнителя-2";
            TechnoInsetColorsDataGrid.Columns["Square"].HeaderText = "м.кв.";
            TechnoInsetColorsDataGrid.Columns["Count"].HeaderText = "шт.";

            tFrontsDataGrid.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tFrontsDataGrid.Columns["Front"].MinimumWidth = 110;
            tFrontsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tFrontsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrontsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrontsDataGrid.Columns["Square"].Width = 100;
            //FrontsDataGrid.Columns["Cost"].Width = 100;
            //FrontsDataGrid.Columns["Count"].Width = 90;

            FrameColorsDataGrid.Columns["FrameColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrameColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrameColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //FrameColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrameColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrameColorsDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //FrameColorsDataGrid.Columns["Square"].Width = 100;
            //FrameColorsDataGrid.Columns["Cost"].Width = 100;
            //FrameColorsDataGrid.Columns["Count"].Width = 90;

            TechnoColorsDataGrid.Columns["TechnoColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechnoColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechnoColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoColorsDataGrid.Columns["Square"].Width = 100;
            //TechnoColorsDataGrid.Columns["Count"].Width = 90;

            SizesDataGrid.Columns["Size"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SizesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //SizesDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //SizesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //SizesDataGrid.Columns["Square"].Width = 100;
            //SizesDataGrid.Columns["Count"].Width = 90;

            InsetTypesDataGrid.Columns["InsetType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetTypesDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            InsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //InsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //InsetTypesDataGrid.Columns["Square"].Width = 100;
            //InsetTypesDataGrid.Columns["Count"].Width = 90;

            InsetColorsDataGrid.Columns["InsetColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            InsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            InsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //InsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //InsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //InsetColorsDataGrid.Columns["Square"].Width = 100;
            //InsetColorsDataGrid.Columns["Count"].Width = 90;

            TechnoInsetTypesDataGrid.Columns["TechnoInsetType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetTypesDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechnoInsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechnoInsetTypesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoInsetTypesDataGrid.Columns["Square"].Width = 100;
            //TechnoInsetTypesDataGrid.Columns["Count"].Width = 90;

            TechnoInsetColorsDataGrid.Columns["TechnoInsetColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechnoInsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechnoInsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechnoInsetColorsDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoInsetColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechnoInsetColorsDataGrid.Columns["Square"].Width = 100;
            //TechnoInsetColorsDataGrid.Columns["Count"].Width = 90;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            FrameColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            FrameColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;
            TechnoColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            TechnoColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;
            tFrontsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            tFrontsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            SizesDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            SizesDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            InsetTypesDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            InsetTypesDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            InsetColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            InsetColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            TechnoInsetTypesDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            TechnoInsetTypesDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            TechnoInsetColorsDataGrid.Columns["Square"].DefaultCellStyle.Format = "N";
            TechnoInsetColorsDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            if (tDecorProductsDataGrid.Columns.Contains("ProductID"))
                tDecorProductsDataGrid.Columns["ProductID"].Visible = false;
            if (tDecorProductsDataGrid.Columns.Contains("MeasureID"))
                tDecorProductsDataGrid.Columns["MeasureID"].Visible = false;

            if (tDecorItemsDataGrid.Columns.Contains("ProductID"))
                tDecorItemsDataGrid.Columns["ProductID"].Visible = false;
            if (tDecorItemsDataGrid.Columns.Contains("DecorID"))
                tDecorItemsDataGrid.Columns["DecorID"].Visible = false;
            if (tDecorItemsDataGrid.Columns.Contains("MeasureID"))
                tDecorItemsDataGrid.Columns["MeasureID"].Visible = false;

            if (tDecorColorsDataGrid.Columns.Contains("ProductID"))
                tDecorColorsDataGrid.Columns["ProductID"].Visible = false;
            if (tDecorColorsDataGrid.Columns.Contains("DecorID"))
                tDecorColorsDataGrid.Columns["DecorID"].Visible = false;
            if (tDecorColorsDataGrid.Columns.Contains("ColorID"))
                tDecorColorsDataGrid.Columns["ColorID"].Visible = false;
            if (tDecorColorsDataGrid.Columns.Contains("MeasureID"))
                tDecorColorsDataGrid.Columns["MeasureID"].Visible = false;

            if (tDecorSizesDataGrid.Columns.Contains("ProductID"))
                tDecorSizesDataGrid.Columns["ProductID"].Visible = false;
            if (tDecorSizesDataGrid.Columns.Contains("DecorID"))
                tDecorSizesDataGrid.Columns["DecorID"].Visible = false;
            if (tDecorSizesDataGrid.Columns.Contains("ColorID"))
                tDecorSizesDataGrid.Columns["ColorID"].Visible = false;
            if (tDecorSizesDataGrid.Columns.Contains("MeasureID"))
                tDecorSizesDataGrid.Columns["MeasureID"].Visible = false;
            if (tDecorSizesDataGrid.Columns.Contains("Height"))
                tDecorSizesDataGrid.Columns["Height"].Visible = false;
            if (tDecorSizesDataGrid.Columns.Contains("Length"))
                tDecorSizesDataGrid.Columns["Length"].Visible = false;
            if (tDecorSizesDataGrid.Columns.Contains("Width"))
                tDecorSizesDataGrid.Columns["Width"].Visible = false;

            tDecorProductsDataGrid.Columns["DecorProduct"].HeaderText = "Продукт";
            tDecorProductsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            tDecorProductsDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            tDecorItemsDataGrid.Columns["DecorItem"].HeaderText = "Наименование";
            tDecorItemsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            tDecorItemsDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            tDecorColorsDataGrid.Columns["Color"].HeaderText = "Цвет";
            tDecorColorsDataGrid.Columns["Count"].HeaderText = "Кол-во";
            tDecorColorsDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            tDecorSizesDataGrid.Columns["Size"].HeaderText = "Размер";
            tDecorSizesDataGrid.Columns["Count"].HeaderText = "Кол-во";
            tDecorSizesDataGrid.Columns["Measure"].HeaderText = "Ед.изм.";

            tDecorProductsDataGrid.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tDecorProductsDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            tDecorProductsDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;
            tDecorProductsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorProductsDataGrid.Columns["Count"].Width = 90;
            tDecorProductsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorProductsDataGrid.Columns["Measure"].Width = 80;

            tDecorItemsDataGrid.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tDecorItemsDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            tDecorItemsDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;
            tDecorItemsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorItemsDataGrid.Columns["Count"].Width = 90;
            tDecorItemsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorItemsDataGrid.Columns["Measure"].Width = 80;

            tDecorColorsDataGrid.Columns["Color"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tDecorColorsDataGrid.Columns["Color"].MinimumWidth = 100;
            tDecorColorsDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            tDecorColorsDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;
            tDecorColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorColorsDataGrid.Columns["Count"].Width = 90;
            tDecorColorsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorColorsDataGrid.Columns["Measure"].Width = 80;

            tDecorSizesDataGrid.Columns["Size"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tDecorSizesDataGrid.Columns["Size"].MinimumWidth = 100;
            tDecorSizesDataGrid.Columns["Count"].DefaultCellStyle.Format = "N";
            tDecorSizesDataGrid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;
            tDecorSizesDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorSizesDataGrid.Columns["Count"].Width = 90;
            tDecorSizesDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tDecorSizesDataGrid.Columns["Measure"].Width = 80;

            tFrontsDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tFrontsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrameColorsDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrameColorsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            SizesDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            SizesDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetTypesDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetTypesDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetColorsDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InsetColorsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            tDecorProductsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tDecorItemsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tDecorColorsDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tDecorSizesDataGrid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void BatchGridSetting()
        {
            BatchDataGrid.Columns.Add(ProfilEnabledColumn);
            BatchDataGrid.Columns.Add(TPSEnabledColumn);
            BatchDataGrid.Columns.Add(ProfilConfirmUserColumn);
            BatchDataGrid.Columns.Add(TPSConfirmUserColumn);
            BatchDataGrid.Columns.Add(ProfilCloseUserColumn);
            BatchDataGrid.Columns.Add(TPSCloseUserColumn);

            BatchDataGrid.Columns["ProfilBatchClose"].Visible = false;
            BatchDataGrid.Columns["TPSBatchClose"].Visible = false;
            BatchDataGrid.Columns["MegaBatchID"].Visible = false;
            BatchDataGrid.Columns["CreateUserID"].Visible = false;
            BatchDataGrid.Columns["CreateDateTime"].Visible = false;

            if (BatchDataGrid.Columns.Contains("ProfilCloseUserID"))
                BatchDataGrid.Columns["ProfilCloseUserID"].Visible = false;
            if (BatchDataGrid.Columns.Contains("TPSCloseUserID"))
                BatchDataGrid.Columns["TPSCloseUserID"].Visible = false;
            if (BatchDataGrid.Columns.Contains("ProfilConfirmUserID"))
                BatchDataGrid.Columns["ProfilConfirmUserID"].Visible = false;
            if (BatchDataGrid.Columns.Contains("TPSConfirmUserID"))
                BatchDataGrid.Columns["TPSConfirmUserID"].Visible = false;
            if (BatchDataGrid.Columns.Contains("ProfilWorkAssignmentID"))
                BatchDataGrid.Columns["ProfilWorkAssignmentID"].Visible = false;
            if (BatchDataGrid.Columns.Contains("TPSWorkAssignmentID"))
                BatchDataGrid.Columns["TPSWorkAssignmentID"].Visible = false;
            if (BatchDataGrid.Columns.Contains("ProfilBatchClose"))
                BatchDataGrid.Columns["ProfilBatchClose"].Visible = false;
            if (BatchDataGrid.Columns.Contains("TPSBatchClose"))
                BatchDataGrid.Columns["TPSBatchClose"].Visible = false;
            foreach (DataGridViewColumn Column in BatchDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            BatchDataGrid.Columns["BatchID"].HeaderText = "№ п\\п";
            BatchDataGrid.Columns["ProfilConfirm"].HeaderText = "Утвер.";
            BatchDataGrid.Columns["TPSConfirm"].HeaderText = "Утвер.";
            BatchDataGrid.Columns["ProfilBatchClose"].HeaderText = "";
            BatchDataGrid.Columns["TPSBatchClose"].HeaderText = "";
            BatchDataGrid.Columns["ProfilName"].HeaderText = "Имя";
            BatchDataGrid.Columns["TPSName"].HeaderText = "Имя";
            BatchDataGrid.Columns["ProfilCloseDateTime"].HeaderText = "Закрыта";
            BatchDataGrid.Columns["TPSCloseDateTime"].HeaderText = "Закрыта";
            BatchDataGrid.Columns["ProfilConfirmDateTime"].HeaderText = "Утверждена";
            BatchDataGrid.Columns["TPSConfirmDateTime"].HeaderText = "Утверждена";
            BatchDataGrid.Columns["CreateDateTime"].HeaderText = "Дата\n\rсоздания";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            BatchDataGrid.Columns["ProfilEnabledColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            BatchDataGrid.Columns["ProfilEnabledColumn"].Width = 40;
            BatchDataGrid.Columns["TPSEnabledColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            BatchDataGrid.Columns["TPSEnabledColumn"].Width = 40;

            BatchDataGrid.Columns["ProfilConfirmUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["ProfilConfirmUserColumn"].MinimumWidth = 40;
            BatchDataGrid.Columns["TPSConfirmUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["TPSConfirmUserColumn"].MinimumWidth = 40;
            BatchDataGrid.Columns["ProfilCloseUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["ProfilCloseUserColumn"].MinimumWidth = 40;
            BatchDataGrid.Columns["TPSCloseUserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["TPSCloseUserColumn"].MinimumWidth = 40;

            BatchDataGrid.Columns["ProfilCloseDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["ProfilCloseDateTime"].MinimumWidth = 40;
            BatchDataGrid.Columns["TPSCloseDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["TPSCloseDateTime"].MinimumWidth = 40;
            BatchDataGrid.Columns["ProfilConfirmDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["ProfilConfirmDateTime"].MinimumWidth = 40;
            BatchDataGrid.Columns["TPSConfirmDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["TPSConfirmDateTime"].MinimumWidth = 40;
            BatchDataGrid.Columns["ProfilBatchClose"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            BatchDataGrid.Columns["ProfilBatchClose"].Width = 40;
            BatchDataGrid.Columns["TPSBatchClose"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            BatchDataGrid.Columns["TPSBatchClose"].Width = 40;
            BatchDataGrid.Columns["ProfilConfirm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["ProfilConfirm"].MinimumWidth = 40;
            BatchDataGrid.Columns["TPSConfirm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["TPSConfirm"].MinimumWidth = 40;
            BatchDataGrid.Columns["BatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            BatchDataGrid.Columns["BatchID"].Width = 60;
            BatchDataGrid.Columns["ProfilName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["ProfilName"].MinimumWidth = 130;
            BatchDataGrid.Columns["TPSName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            BatchDataGrid.Columns["TPSName"].MinimumWidth = 130;

            BatchDataGrid.Columns["ProfilName"].ReadOnly = false;
            BatchDataGrid.Columns["TPSName"].ReadOnly = false;

            BatchDataGrid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            BatchDataGrid.Columns["BatchID"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilConfirm"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSConfirm"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilEnabledColumn"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSEnabledColumn"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilName"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSName"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilConfirmDateTime"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilConfirmUserColumn"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSConfirmDateTime"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSConfirmUserColumn"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilCloseDateTime"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["ProfilCloseUserColumn"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSCloseDateTime"].DisplayIndex = DisplayIndex++;
            BatchDataGrid.Columns["TPSCloseUserColumn"].DisplayIndex = DisplayIndex++;

            BatchDataGrid.Columns["ProfilConfirm"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            BatchDataGrid.Columns["TPSConfirm"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            BatchDataGrid.Columns["ProfilBatchClose"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            BatchDataGrid.Columns["TPSBatchClose"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void MegaBatchGridSetting()
        {
            MegaBatchDataGrid.Columns.Add(ProfilEnabledColumn);
            MegaBatchDataGrid.Columns.Add(TPSEnabledColumn);

            MegaBatchDataGrid.Columns["ProfilBatchClose"].Visible = false;
            MegaBatchDataGrid.Columns["TPSBatchClose"].Visible = false;
            MegaBatchDataGrid.Columns["CreateUserID"].Visible = false;
            MegaBatchDataGrid.Columns["ProfilCloseUserID"].Visible = false;
            MegaBatchDataGrid.Columns["TPSCloseUserID"].Visible = false;
            MegaBatchDataGrid.Columns["ProfilBatchClose"].Visible = false;
            MegaBatchDataGrid.Columns["TPSBatchClose"].Visible = false;

            MegaBatchDataGrid.Columns["ProfilAgreedUserID"].Visible = false;
            MegaBatchDataGrid.Columns["ProfilAgreedDateTime"].Visible = false;
            MegaBatchDataGrid.Columns["TPSAgreedUserID"].Visible = false;
            MegaBatchDataGrid.Columns["TPSAgreedDateTime"].Visible = false;

            foreach (DataGridViewColumn Column in MegaBatchDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            MegaBatchDataGrid.Columns["WeekNumber"].HeaderText = "Неделя";
            MegaBatchDataGrid.Columns["ProfilEntryDateTime"].HeaderText = "Вошло в пр-во";
            MegaBatchDataGrid.Columns["TPSEntryDateTime"].HeaderText = "Вошло в пр-во";
            MegaBatchDataGrid.Columns["ProfilBatchClose"].HeaderText = "";
            MegaBatchDataGrid.Columns["TPSBatchClose"].HeaderText = "";
            MegaBatchDataGrid.Columns["Notes"].HeaderText = "Примечание";
            MegaBatchDataGrid.Columns["MegaBatchID"].HeaderText = "№ n\\п";
            MegaBatchDataGrid.Columns["CreateDateTime"].HeaderText = "Дата\n\rсоздания";
            //MegaBatchDataGrid.Columns["ProfilInputDateTime"].HeaderText = "Дата входа\n\rна пр-во";

            MegaBatchDataGrid.Columns["WeekNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaBatchDataGrid.Columns["WeekNumber"].Width = 70;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MegaBatchDataGrid.Columns["ProfilEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MegaBatchDataGrid.Columns["TPSEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MegaBatchDataGrid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            //MegaBatchDataGrid.Columns["ProfilInputDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy";

            MegaBatchDataGrid.Columns["ProfilEnabledColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaBatchDataGrid.Columns["ProfilEnabledColumn"].Width = 40;
            MegaBatchDataGrid.Columns["TPSEnabledColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaBatchDataGrid.Columns["TPSEnabledColumn"].Width = 40;

            MegaBatchDataGrid.Columns["MegaBatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaBatchDataGrid.Columns["MegaBatchID"].Width = 60;
            MegaBatchDataGrid.Columns["ProfilEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaBatchDataGrid.Columns["ProfilEntryDateTime"].MinimumWidth = 130;
            MegaBatchDataGrid.Columns["TPSEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaBatchDataGrid.Columns["TPSEntryDateTime"].MinimumWidth = 130;
            MegaBatchDataGrid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaBatchDataGrid.Columns["CreateDateTime"].MinimumWidth = 130;
            MegaBatchDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaBatchDataGrid.Columns["Notes"].MinimumWidth = 80;
            MegaBatchDataGrid.Columns["ProfilBatchClose"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaBatchDataGrid.Columns["ProfilBatchClose"].MinimumWidth = 40;
            MegaBatchDataGrid.Columns["TPSBatchClose"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaBatchDataGrid.Columns["TPSBatchClose"].MinimumWidth = 40;
            //MegaBatchDataGrid.Columns["ProfilInputDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MegaBatchDataGrid.Columns["ProfilInputDateTime"].MinimumWidth = 130;

            MegaBatchDataGrid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            MegaBatchDataGrid.Columns["MegaBatchID"].DisplayIndex = DisplayIndex++;
            MegaBatchDataGrid.Columns["WeekNumber"].DisplayIndex = DisplayIndex++;
            MegaBatchDataGrid.Columns["ProfilEnabledColumn"].DisplayIndex = DisplayIndex++;
            MegaBatchDataGrid.Columns["TPSEnabledColumn"].DisplayIndex = DisplayIndex++;
            MegaBatchDataGrid.Columns["CreateDateTime"].DisplayIndex = DisplayIndex++;
            MegaBatchDataGrid.Columns["Notes"].DisplayIndex = DisplayIndex++;

            MegaBatchDataGrid.Columns["Notes"].ReadOnly = false;
        }

        private void MainGridSetting(ref PercentageDataGrid tPercentageDataGrid)
        {
            foreach (DataGridViewColumn Column in tPercentageDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (tPercentageDataGrid.Columns.Contains("IsSample"))
                tPercentageDataGrid.Columns["IsSample"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("WillPercentID"))
                tPercentageDataGrid.Columns["WillPercentID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("FactoryID"))
                tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilProductionStatusID"))
                tPercentageDataGrid.Columns["ProfilProductionStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilStorageStatusID"))
                tPercentageDataGrid.Columns["ProfilStorageStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilDispatchStatusID"))
                tPercentageDataGrid.Columns["ProfilDispatchStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSProductionStatusID"))
                tPercentageDataGrid.Columns["TPSProductionStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSStorageStatusID"))
                tPercentageDataGrid.Columns["TPSStorageStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSDispatchStatusID"))
                tPercentageDataGrid.Columns["TPSDispatchStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("MegaOrderID"))
                tPercentageDataGrid.Columns["MegaOrderID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("AllocPackDateTime"))
                tPercentageDataGrid.Columns["AllocPackDateTime"].Visible = false;

            if (tPercentageDataGrid.Columns.Contains("FrontsCost"))
                tPercentageDataGrid.Columns["FrontsCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("DecorCost"))
                tPercentageDataGrid.Columns["DecorCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("OrderCost"))
                tPercentageDataGrid.Columns["OrderCost"].Visible = false;

            if (tPercentageDataGrid.Columns.Contains("ProfilPackAllocStatusID"))
                tPercentageDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSPackAllocStatusID"))
                tPercentageDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSPackCount"))
                tPercentageDataGrid.Columns["TPSPackCount"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilPackCount"))
                tPercentageDataGrid.Columns["ProfilPackCount"].Visible = false;

            tPercentageDataGrid.Columns["ProfilProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            tPercentageDataGrid.Columns["TPSProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            tPercentageDataGrid.Columns["MainOrderID"].HeaderText = "№ п\\п";
            tPercentageDataGrid.Columns["FrontsCost"].HeaderText = "Стоимость\r\nфасадов, евро";
            tPercentageDataGrid.Columns["DecorCost"].HeaderText = "Стоимость\r\nдекора, евро";
            tPercentageDataGrid.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            tPercentageDataGrid.Columns["DocDateTime"].HeaderText = "Дата\r\nсоздания";
            tPercentageDataGrid.Columns["Notes"].HeaderText = "Примечание";
            tPercentageDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            tPercentageDataGrid.Columns["ProfilProductionDate"].HeaderText = "Дата входа\r\nв пр-во, Профиль";
            tPercentageDataGrid.Columns["TPSProductionDate"].HeaderText = " Дата входа\r\nв пр-во, ТПС";
            tPercentageDataGrid.Columns["ProfilOnProductionDate"].HeaderText = "Дата входа\r\nна пр-во, Профиль";
            tPercentageDataGrid.Columns["TPSOnProductionDate"].HeaderText = " Дата входа\r\nна пр-во, ТПС";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            tPercentageDataGrid.Columns["FrontsCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["FrontsCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["DecorCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["DecorCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["ProfilOnProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilOnProductionDate"].MinimumWidth = 165;
            tPercentageDataGrid.Columns["TPSOnProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSOnProductionDate"].MinimumWidth = 140;
            tPercentageDataGrid.Columns["ProfilProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilProductionDate"].MinimumWidth = 165;
            tPercentageDataGrid.Columns["TPSProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSProductionDate"].MinimumWidth = 140;
            tPercentageDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["MainOrderID"].Width = 80;
            tPercentageDataGrid.Columns["FrontsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["FrontsCost"].Width = 130;
            tPercentageDataGrid.Columns["DecorCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["DecorCost"].Width = 130;
            tPercentageDataGrid.Columns["OrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["OrderCost"].Width = 130;
            tPercentageDataGrid.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["DocDateTime"].MinimumWidth = 130;
            tPercentageDataGrid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["FrontsSquare"].Width = 110;
            tPercentageDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["Notes"].MinimumWidth = 60;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["Weight"].Width = 90;
            tPercentageDataGrid.Columns["ProfilProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilProductionStatusColumn"].Width = 150;
            tPercentageDataGrid.Columns["ProfilStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilStorageStatusColumn"].Width = 110;
            tPercentageDataGrid.Columns["ProfilExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilExpeditionStatusColumn"].Width = 150;
            tPercentageDataGrid.Columns["ProfilDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilDispatchStatusColumn"].Width = 110;
            tPercentageDataGrid.Columns["TPSProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSProductionStatusColumn"].Width = 150;
            tPercentageDataGrid.Columns["TPSStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSStorageStatusColumn"].Width = 110;
            tPercentageDataGrid.Columns["TPSExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSExpeditionStatusColumn"].Width = 150;
            tPercentageDataGrid.Columns["TPSDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSDispatchStatusColumn"].Width = 110;
            tPercentageDataGrid.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["FactoryTypeColumn"].Width = 130;

            tPercentageDataGrid.AutoGenerateColumns = false;

            tPercentageDataGrid.Columns["MainOrderID"].DisplayIndex = 0;
            tPercentageDataGrid.Columns["FactoryTypeColumn"].DisplayIndex = 1;
            tPercentageDataGrid.Columns["ProfilProductionDate"].DisplayIndex = 2;
            tPercentageDataGrid.Columns["ProfilProductionStatusColumn"].DisplayIndex = 3;
            tPercentageDataGrid.Columns["ProfilStorageStatusColumn"].DisplayIndex = 4;
            tPercentageDataGrid.Columns["ProfilExpeditionStatusColumn"].DisplayIndex = 5;
            tPercentageDataGrid.Columns["ProfilDispatchStatusColumn"].DisplayIndex = 6;
            tPercentageDataGrid.Columns["TPSProductionDate"].DisplayIndex = 7;
            tPercentageDataGrid.Columns["TPSProductionStatusColumn"].DisplayIndex = 8;
            tPercentageDataGrid.Columns["TPSStorageStatusColumn"].DisplayIndex = 9;
            tPercentageDataGrid.Columns["TPSExpeditionStatusColumn"].DisplayIndex = 10;
            tPercentageDataGrid.Columns["TPSDispatchStatusColumn"].DisplayIndex = 11;
            tPercentageDataGrid.Columns["FrontsSquare"].DisplayIndex = 12;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = 13;
            tPercentageDataGrid.Columns["DocDateTime"].DisplayIndex = 14;
            tPercentageDataGrid.Columns["Notes"].DisplayIndex = 15;

            if (tPercentageDataGrid.Columns.Contains("BatchID"))
            {
                tPercentageDataGrid.Columns["BatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                tPercentageDataGrid.Columns["BatchID"].Width = 80;
                tPercentageDataGrid.Columns["BatchID"].DisplayIndex = 1;
                tPercentageDataGrid.Columns["BatchID"].HeaderText = "    №\n\rпартии";
            }

            if (tPercentageDataGrid.Columns.Contains("BatchNumber"))
            {
                tPercentageDataGrid.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                tPercentageDataGrid.Columns["BatchNumber"].Width = 80;
                tPercentageDataGrid.Columns["BatchNumber"].HeaderText = "    №\n\rпартии";
                tPercentageDataGrid.Columns["BatchNumber"].DisplayIndex = 1;
            }

            tPercentageDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tPercentageDataGrid.Columns["FrontsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tPercentageDataGrid.Columns["DecorCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tPercentageDataGrid.Columns["OrderCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tPercentageDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void MegaGridSetting(ref PercentageDataGrid tPercentageDataGrid)
        {
            foreach (DataGridViewColumn Column in tPercentageDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (tPercentageDataGrid.Columns.Contains("TransportType"))
                tPercentageDataGrid.Columns["TransportType"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ClientID"))
                tPercentageDataGrid.Columns["ClientID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("OrderStatusID"))
                tPercentageDataGrid.Columns["OrderStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilOrderStatusID"))
                tPercentageDataGrid.Columns["ProfilOrderStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSOrderStatusID"))
                tPercentageDataGrid.Columns["TPSOrderStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("AgreementStatusID"))
                tPercentageDataGrid.Columns["AgreementStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTypeID"))
                tPercentageDataGrid.Columns["CurrencyTypeID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("FactoryID"))
                tPercentageDataGrid.Columns["FactoryID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("OrderCost"))
                tPercentageDataGrid.Columns["OrderCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TransportCost"))
                tPercentageDataGrid.Columns["TransportCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("AdditionalCost"))
                tPercentageDataGrid.Columns["AdditionalCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyOrderCost"))
                tPercentageDataGrid.Columns["CurrencyOrderCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTotalCost"))
                tPercentageDataGrid.Columns["CurrencyTotalCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyTransportCost"))
                tPercentageDataGrid.Columns["CurrencyTransportCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyAdditionalCost"))
                tPercentageDataGrid.Columns["CurrencyAdditionalCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ComplaintProfilCost"))
                tPercentageDataGrid.Columns["ComplaintProfilCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ComplaintTPSCost"))
                tPercentageDataGrid.Columns["ComplaintTPSCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ComplaintNotes"))
                tPercentageDataGrid.Columns["ComplaintNotes"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyComplaintProfilCost"))
                tPercentageDataGrid.Columns["CurrencyComplaintProfilCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("CurrencyComplaintTPSCost"))
                tPercentageDataGrid.Columns["CurrencyComplaintTPSCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("DelayOfPayment"))
                tPercentageDataGrid.Columns["DelayOfPayment"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TotalCost"))
                tPercentageDataGrid.Columns["TotalCost"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("Rate"))
                tPercentageDataGrid.Columns["Rate"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("PaymentRate"))
                tPercentageDataGrid.Columns["PaymentRate"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilPackAllocStatusID"))
                tPercentageDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSPackAllocStatusID"))
                tPercentageDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSPackCount"))
                tPercentageDataGrid.Columns["TPSPackCount"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilPackCount"))
                tPercentageDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("DesireDate"))
                tPercentageDataGrid.Columns["DesireDate"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("LastCalcDate"))
                tPercentageDataGrid.Columns["LastCalcDate"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("LastCalcUserID"))
                tPercentageDataGrid.Columns["LastCalcUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilConfirmProduction"))
                tPercentageDataGrid.Columns["ProfilConfirmProduction"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSConfirmProduction"))
                tPercentageDataGrid.Columns["TPSConfirmProduction"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilAllowDispatch"))
                tPercentageDataGrid.Columns["ProfilAllowDispatch"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSAllowDispatch"))
                tPercentageDataGrid.Columns["TPSAllowDispatch"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilConfirmDispatch"))
                tPercentageDataGrid.Columns["ProfilConfirmDispatch"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSConfirmDispatch"))
                tPercentageDataGrid.Columns["TPSConfirmDispatch"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilProductionDate"))
                tPercentageDataGrid.Columns["ProfilProductionDate"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("ProfilProductionUserID"))
                tPercentageDataGrid.Columns["ProfilProductionUserID"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSProductionDate"))
                tPercentageDataGrid.Columns["TPSProductionDate"].Visible = false;
            if (tPercentageDataGrid.Columns.Contains("TPSProductionUserID"))
                tPercentageDataGrid.Columns["TPSProductionUserID"].Visible = false;

            tPercentageDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            tPercentageDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            tPercentageDataGrid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            tPercentageDataGrid.Columns["OrderDate"].HeaderText = "Дата\r\nсоздания";
            tPercentageDataGrid.Columns["ProfilDispatchDate"].HeaderText = "Дата отгрузки\r\nПрофиль";
            tPercentageDataGrid.Columns["TPSDispatchDate"].HeaderText = "Дата отгрузки\r\nТПС";
            tPercentageDataGrid.Columns["ConfirmDateTime"].HeaderText = "Дата\r\nсогласования";
            tPercentageDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            tPercentageDataGrid.Columns["Square"].HeaderText = "Квадратура";
            tPercentageDataGrid.Columns["IsComplaint"].HeaderText = "Рекл.";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 4
            };
            NumberFormatInfo nfi3 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 2
            };
            tPercentageDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["TransportCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["TransportCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["AdditionalCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["AdditionalCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["TotalCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["TotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["Rate"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["Rate"].DefaultCellStyle.FormatProvider = nfi2;
            tPercentageDataGrid.Columns["PaymentRate"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["PaymentRate"].DefaultCellStyle.FormatProvider = nfi2;

            tPercentageDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            tPercentageDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            tPercentageDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi3;

            tPercentageDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tPercentageDataGrid.Columns["ClientName"].MinimumWidth = 240;
            tPercentageDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["MegaOrderID"].Width = 70;
            tPercentageDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["OrderNumber"].Width = 70;
            tPercentageDataGrid.Columns["OrderDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["OrderDate"].Width = 150;
            tPercentageDataGrid.Columns["ProfilDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilDispatchDate"].Width = 140;
            tPercentageDataGrid.Columns["TPSDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSDispatchDate"].Width = 140;
            tPercentageDataGrid.Columns["ConfirmDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ConfirmDateTime"].Width = 130;
            tPercentageDataGrid.Columns["AgreementStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["AgreementStatusColumn"].Width = 160;
            tPercentageDataGrid.Columns["OrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["OrderStatusColumn"].Width = 160;
            tPercentageDataGrid.Columns["ProfilOrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["ProfilOrderStatusColumn"].Width = 160;
            tPercentageDataGrid.Columns["TPSOrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["TPSOrderStatusColumn"].Width = 160;
            tPercentageDataGrid.Columns["MegaFactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["MegaFactoryTypeColumn"].Width = 130;
            tPercentageDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["Weight"].Width = 70;
            tPercentageDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["Square"].Width = 105;
            tPercentageDataGrid.Columns["IsComplaint"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tPercentageDataGrid.Columns["IsComplaint"].MinimumWidth = 55;

            tPercentageDataGrid.AutoGenerateColumns = false;

            tPercentageDataGrid.Columns["OrderDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            tPercentageDataGrid.Columns["ClientName"].DisplayIndex = 0;
            tPercentageDataGrid.Columns["OrderNumber"].DisplayIndex = 1;
            tPercentageDataGrid.Columns["MegaOrderID"].DisplayIndex = 2;
            tPercentageDataGrid.Columns["ProfilOrderStatusColumn"].DisplayIndex = 3;
            tPercentageDataGrid.Columns["TPSOrderStatusColumn"].DisplayIndex = 4;
            tPercentageDataGrid.Columns["OrderStatusColumn"].DisplayIndex = 5;
            tPercentageDataGrid.Columns["MegaFactoryTypeColumn"].DisplayIndex = 6;
            tPercentageDataGrid.Columns["AgreementStatusColumn"].DisplayIndex = 7;
            tPercentageDataGrid.Columns["ProfilDispatchDate"].DisplayIndex = 8;
            tPercentageDataGrid.Columns["TPSDispatchDate"].DisplayIndex = 9;
            tPercentageDataGrid.Columns["Weight"].DisplayIndex = 10;
            tPercentageDataGrid.Columns["Square"].DisplayIndex = 11;
            tPercentageDataGrid.Columns["OrderDate"].DisplayIndex = 12;
            tPercentageDataGrid.Columns["IsComplaint"].DisplayIndex = 13;
            tPercentageDataGrid.Columns["ConfirmDateTime"].DisplayIndex = 14;

            tPercentageDataGrid.Columns["ClientName"].Frozen = true;
            tPercentageDataGrid.Columns["OrderNumber"].Frozen = true;
            tPercentageDataGrid.RightToLeft = RightToLeft.No;

            tPercentageDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tPercentageDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            tPercentageDataGrid.Columns["IsComplaint"].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        #endregion

        #region Filter functions

        public void MoveToMegaOrder(int MegaOrderID)
        {
            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        public void FilterBatch(int MegaBatchID, int FactoryID)
        {
            if (CurrentMegaBatchID == MegaBatchID)
                return;

            string BatchFactoryFilter = string.Empty;
            string SelectCommand = string.Empty;

            if (FactoryID != 0)
                BatchFactoryFilter = " AND (BatchID IN (SELECT BatchID FROM BatchDetails WHERE BatchDetails.FactoryID = " + FactoryID +
                    ") OR BatchID NOT IN (SELECT BatchID FROM BatchDetails))";

            SelectCommand = @"SELECT * FROM Batch WHERE MegaBatchID = " + MegaBatchID + BatchFactoryFilter + " ORDER BY BatchID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                BatchDataTable.Clear();
                DA.Fill(BatchDataTable);
            }
        }

        private void FillBatchNumber()
        {
            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                MainOrdersDataTable.Rows[i]["BatchNumber"] = GetBatchNumber(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
            }
        }

        public bool Filter_MegaBatches_ByFactory(int FactoryID)
        {
            int MegaBatchID = CurrentMegaBatchID;

            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string SelectCommand = string.Empty;

            if (FactoryID != 0)
                BatchFactoryFilter = " WHERE MegaBatchID IN (SELECT MegaBatchID FROM Batch WHERE BatchID IN (SELECT BatchID FROM BatchDetails WHERE BatchDetails.FactoryID = " + FactoryID +
                    ") OR BatchID NOT IN (SELECT BatchID FROM BatchDetails)) OR MegaBatchID NOT IN (SELECT MegaBatchID FROM Batch)";

            SelectCommand = "SELECT * FROM MegaBatch ORDER BY MegaBatchID DESC";

            try
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        MegaBatchDataTable.Clear();
                        DA.Fill(MegaBatchDataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MegaBatchBindingSource.Position = MegaBatchBindingSource.Find("MegaBatchID", MegaBatchID);
            FillWeekNumber();
            SetMegaBatchEnable();
            ShowMegaOrdersColumns(ref BatchMegaOrdersDataGrid, FactoryID);

            return MegaBatchDataTable.Rows.Count > 0;
        }

        public void Filter(
            int FactoryID,
            bool NotConfirm,
            bool Confirm,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool PickUpFronts)
        {
            ArrayList array = new ArrayList();

            int MegaOrderID = -1;

            string FactoryFilter = string.Empty;
            string AgreementStatus = string.Empty;
            string OrdersProductionStatus = string.Empty;

            if (NotConfirm)
                AgreementStatus = "AgreementStatusID=1";

            if (Confirm)
            {
                if (AgreementStatus.Length > 0)
                    AgreementStatus += " OR AgreementStatusID=2";
                else
                    AgreementStatus = "AgreementStatusID=2";
            }

            if (AgreementStatus.Length > 0)
                AgreementStatus = " AND (" + AgreementStatus + ")";

            #region Orders

            if (NotProduction)
            {
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OrdersProductionStatus.Length > 0)
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";

            #endregion

            if (FactoryID > 0)
            {
                FactoryFilter = " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            if (FactoryID == -1)
            {
                FactoryFilter = " WHERE (FactoryID = -1)";
            }

            if (MegaOrdersBindingSource.Count > 0)
                MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBindingSource.Current).Row["MegaOrderID"]);

            CurrentMegaOrderID = -1;

            if (!OnProduction && !NotProduction && !InProduction)
                AgreementStatus = " AND AgreementStatusID=-1";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" + FactoryFilter + OrdersProductionStatus + ")" + AgreementStatus +
                " ORDER BY ClientName, OrderNumber", ConnectionStrings.MarketingOrdersConnectionString))
            {
                MegaOrdersDataTable.Clear();
                DA.Fill(MegaOrdersDataTable);
            }

            MoveToMegaOrder(MegaOrderID);

            ShowMegaOrdersColumns(ref MegaOrdersDataGrid, FactoryID);
        }

        public void FilterMainOrdersByMegaOrder(
            int MegaOrderID,
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            if (MegaOrdersBindingSource.Count == 0)
                return;

            if (CurrentMegaOrderID == MegaOrderID)
                return;

            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (FactoryID > 0)
            {
                FactoryFilter = " AND (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")";
            }

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            MainOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + MainProductionStatus,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            CurrentMegaOrderID = MegaOrderID;
            ShowMainOrdersColumns(ref MainOrdersDataGrid, FactoryID);
            FillBatchNumber();
        }

        public void FilterProductsByMainOrder(int MainOrderID, int FactoryID)
        {
            OrdersTabControl.TabPages[0].PageVisible = MainOrdersFrontsOrders.Filter(MainOrderID, FactoryID);
            OrdersTabControl.TabPages[1].PageVisible = MainOrdersDecorOrders.Filter(MainOrderID, FactoryID);
        }

        public ArrayList FilterBatchMegaOrders(
            int BatchID, int FactoryID,
            bool BatchOnProduction,
            bool BatchNotProduction,
            bool BatchInProduction,
            bool BatchOnStorage,
            bool BatchOnExp,
            bool BatchDispatched,
            bool PickUpFronts)
        {
            ArrayList array = new ArrayList();

            if (BatchBindingSource.Count == 0)
                return array;

            int MegaOrderID = 0;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (BatchNotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (BatchInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (BatchOnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (BatchOnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (BatchOnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (BatchDispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (BatchMegaOrdersBindingSource.Count > 0)
                MegaOrderID = Convert.ToInt32(((DataRowView)BatchMegaOrdersBindingSource.Current).Row["MegaOrderID"]);


            if (FactoryID != 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";

            if (!BatchNotProduction && !BatchOnProduction && !BatchInProduction && !BatchOnStorage && !BatchDispatched)
            {
                MainProductionStatus = " AND MainOrders.MainOrderID = -1";
            }

            BatchMegaOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")" + FactoryFilter + MainProductionStatus + ")" +
                " ORDER BY ClientName, OrderNumber",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchMegaOrdersDataTable);

                for (int i = 0; i < BatchMegaOrdersDataTable.Rows.Count; i++)
                {
                    array.Add(Convert.ToInt32(BatchMegaOrdersDataTable.Rows[i]["MegaOrderID"]));
                }
            }

            BatchMegaOrdersBindingSource.Position = BatchMegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
            GetMainOrdersNotInProduction(FactoryID, PickUpFronts);

            return array;
        }

        public ArrayList FilterBatchMainOrdersByMegaOrder(
            int BatchID, int MegaOrderID, int FactoryID,
            bool BatchOnProduction,
            bool BatchNotProduction,
            bool BatchInProduction,
            bool BatchOnStorage,
            bool BatchOnExp,
            bool BatchDispatched)
        {
            ArrayList array = new ArrayList();

            if (BatchMegaOrdersBindingSource.Count == 0)
                return array;

            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")";

            if (BatchNotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (BatchInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (BatchOnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (BatchOnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (BatchOnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (BatchDispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            BatchMainOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrders.*, BatchDetails.BatchID FROM MainOrders" +
                " LEFT JOIN BatchDetails ON (MainOrders.MainOrderID = BatchDetails.MainOrderID AND BatchDetails.FactoryID = " + FactoryID + ")" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM BatchDetails" +
                " WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")" + MainProductionStatus,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchMainOrdersDataTable);

                for (int i = 0; i < BatchMainOrdersDataTable.Rows.Count; i++)
                {
                    array.Add(Convert.ToInt32(BatchMainOrdersDataTable.Rows[i]["MainOrderID"]));
                }
            }

            string SelectionCommand = string.Empty;

            if (FactoryID == 1)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ") AND ProfilProductionStatusID = 3 AND MegaOrderID = " + MegaOrderID;

            if (FactoryID == 2)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ") AND TPSProductionStatusID = 3 AND MegaOrderID = " + MegaOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < BatchMainOrdersDataGrid.Rows.Count; i++)
                    {
                        int MainOrderID = Convert.ToInt32(BatchMainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                        if (DT.Select("MainOrderID = " + MainOrderID).Count() > 0)
                            BatchMainOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                    }
                }
            }

            ShowMainOrdersColumns(ref BatchMainOrdersDataGrid, FactoryID);

            return array;
        }

        public ArrayList FilterBatchMainOrdersByFront(int BatchID,
            int MegaOrderID, int FactoryID,
            bool BatchOnProduction,
            bool BatchNotProduction,
            bool BatchInProduction,
            bool BatchOnStorage,
            bool BatchOnExp,
            bool BatchDispatched)
        {
            ArrayList array = new ArrayList();

            if (BatchMegaOrdersBindingSource.Count == 0)
                return array;

            string MainOrdersFilter = "SELECT MainOrderID FROM FrontsOrders WHERE FrontID = " + CurrentFrontID + " AND ColorID = " + CurrentFrameColorID + " AND PatinaID = " + CurrentPatinaID;

            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")";

            if (BatchNotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (BatchInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (BatchOnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (BatchOnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (BatchOnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (BatchDispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrders.*, BatchDetails.BatchID FROM MainOrders" +
                " LEFT JOIN BatchDetails ON (MainOrders.MainOrderID = BatchDetails.MainOrderID AND BatchDetails.FactoryID = " + FactoryID + ")" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM BatchDetails" +
                " WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")" + MainProductionStatus +
                " AND MainOrders.MainOrderID IN (" + MainOrdersFilter + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                BatchMainOrdersDataTable.Clear();
                DA.Fill(BatchMainOrdersDataTable);

                for (int i = 0; i < BatchMainOrdersDataTable.Rows.Count; i++)
                {
                    array.Add(Convert.ToInt32(BatchMainOrdersDataTable.Rows[i]["MainOrderID"]));
                }
            }

            string SelectionCommand = string.Empty;

            if (FactoryID == 1)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID=" + BatchID + ") AND ProfilProductionStatusID = 3 AND MegaOrderID = " + MegaOrderID;

            if (FactoryID == 2)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID=" + BatchID + ") AND TPSProductionStatusID = 3 AND MegaOrderID = " + MegaOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < BatchMainOrdersDataGrid.Rows.Count; i++)
                    {
                        int MainOrderID = Convert.ToInt32(BatchMainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                        if (DT.Select("MainOrderID = " + MainOrderID).Count() > 0)
                            BatchMainOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                    }
                }
            }

            ShowMainOrdersColumns(ref BatchMainOrdersDataGrid, FactoryID);

            return array;
        }

        public ArrayList FilterBatchMainOrdersByFront(int BatchID,
            int MegaOrderID, int FactoryID,
            bool BatchOnProduction,
            bool BatchNotProduction,
            bool BatchInProduction,
            bool BatchOnStorage,
            bool BatchOnExp,
            bool BatchDispatched,
            ArrayList Fronts)
        {
            ArrayList array = new ArrayList();

            if (BatchMegaOrdersBindingSource.Count == 0)
                return array;

            string MainOrdersFilter = "SELECT MainOrderID FROM FrontsOrders WHERE FrontID IN (" + string.Join(",", Fronts.OfType<int>().ToArray()) + ")";

            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")";

            if (BatchNotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (BatchInProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (BatchOnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (BatchOnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (BatchOnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (BatchDispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrders.*, BatchDetails.BatchID FROM MainOrders" +
                " LEFT JOIN BatchDetails ON (MainOrders.MainOrderID = BatchDetails.MainOrderID AND BatchDetails.FactoryID = " + FactoryID + ")" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM BatchDetails" +
                " WHERE FactoryID = " + FactoryID + " AND BatchID=" + BatchID + ")" + MainProductionStatus +
                " AND MainOrders.MainOrderID IN (" + MainOrdersFilter + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                BatchMainOrdersDataTable.Clear();
                DA.Fill(BatchMainOrdersDataTable);

                for (int i = 0; i < BatchMainOrdersDataTable.Rows.Count; i++)
                {
                    array.Add(Convert.ToInt32(BatchMainOrdersDataTable.Rows[i]["MainOrderID"]));
                }
            }

            string SelectionCommand = string.Empty;

            if (FactoryID == 1)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID=" + BatchID + ") AND ProfilProductionStatusID = 3 AND MegaOrderID = " + MegaOrderID;

            if (FactoryID == 2)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID=" + BatchID + ") AND TPSProductionStatusID = 3 AND MegaOrderID = " + MegaOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < BatchMainOrdersDataGrid.Rows.Count; i++)
                    {
                        int MainOrderID = Convert.ToInt32(BatchMainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                        if (DT.Select("MainOrderID = " + MainOrderID).Count() > 0)
                            BatchMainOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                    }
                }
            }

            ShowMainOrdersColumns(ref BatchMainOrdersDataGrid, FactoryID);

            return array;
        }

        public void FilterBatchProductsByMainOrder(int MainOrderID, int FactoryID)
        {
            BatchOrdersTabControl.TabPages[0].PageVisible = BatchMainOrdersFrontsOrders.Filter(MainOrderID, FactoryID);
            BatchOrdersTabControl.TabPages[1].PageVisible = BatchMainOrdersDecorOrders.Filter(MainOrderID, FactoryID);
        }

        public void FilterFrameColors(int FrontID)
        {
            FrameColorsSummaryBindingSource.Filter = "FrontID=" + FrontID;
            FrameColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterTechnoColors(int FrontID, int ColorID, int PatinaID)
        {
            TechnoColorsSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND ColorID=" + ColorID + " AND PatinaID=" + PatinaID;
            TechnoColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterInsetTypes(int FrontID, int ColorID, int PatinaID, int TechnoColorID)
        {
            InsetTypesSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND ColorID=" + ColorID + " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID;
            InsetTypesSummaryBindingSource.MoveFirst();
        }

        public void FilterInsetColors(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID)
        {
            InsetColorsSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" +
                PatinaID + " AND InsetTypeID=" + InsetTypeID + " AND ColorID=" + ColorID;
            InsetColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterTechnoInsetTypes(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID, int InsetColorID)
        {
            TechnoInsetTypesSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND TechnoColorID=" + TechnoColorID + " AND InsetColorID=" + InsetColorID + " AND InsetTypeID=" + InsetTypeID +
                " AND ColorID=" + ColorID;
            TechnoInsetTypesSummaryBindingSource.MoveFirst();
        }

        public void FilterTechnoInsetColors(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID)
        {
            TechnoInsetColorsSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND InsetTypeID=" + InsetTypeID +
                " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID +
                " AND InsetColorID=" + InsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
            TechnoInsetColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterSizes(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            SizesSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID +
                " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID + " AND TechnoInsetColorID=" + TechnoInsetColorID;
            SizesSummaryBindingSource.MoveFirst();
        }

        public void FilterPreFrameColors(int FrontID)
        {
            PreFrameColorsSummaryBindingSource.Filter = "FrontID=" + FrontID;
            PreFrameColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterPreTechnoColors(int FrontID, int ColorID, int PatinaID)
        {
            PreTechnoColorsSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND ColorID=" + ColorID + " AND PatinaID=" + PatinaID;
            PreTechnoColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterPreInsetTypes(int FrontID, int ColorID, int PatinaID, int TechnoColorID)
        {
            PreInsetTypesSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND ColorID=" + ColorID + " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID;
            PreInsetTypesSummaryBindingSource.MoveFirst();
        }

        public void FilterPreInsetColors(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID)
        {
            PreInsetColorsSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" +
                PatinaID + " AND InsetTypeID=" + InsetTypeID + " AND ColorID=" + ColorID;
            PreInsetColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterPreTechnoInsetTypes(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID, int InsetColorID)
        {
            PreTechnoInsetTypesSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND TechnoColorID=" + TechnoColorID + " AND InsetColorID=" + InsetColorID + " AND InsetTypeID=" + InsetTypeID +
                " AND ColorID=" + ColorID;
            PreTechnoInsetTypesSummaryBindingSource.MoveFirst();
        }

        public void FilterPreTechnoInsetColors(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID)
        {
            PreTechnoInsetColorsSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND InsetTypeID=" + InsetTypeID +
                " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID +
                " AND InsetColorID=" + InsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID;
            PreTechnoInsetColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterPreSizes(int FrontID, int ColorID, int PatinaID, int TechnoColorID, int InsetTypeID, int InsetColorID, int TechnoInsetTypeID, int TechnoInsetColorID)
        {
            PreSizesSummaryBindingSource.Filter = "FrontID=" + FrontID + " AND PatinaID=" + PatinaID +
                " AND ColorID=" + ColorID + " AND TechnoColorID=" + TechnoColorID +
                " AND InsetTypeID=" + InsetTypeID + " AND InsetColorID=" + InsetColorID + " AND TechnoInsetTypeID=" + TechnoInsetTypeID + " AND TechnoInsetColorID=" + TechnoInsetColorID;
            PreSizesSummaryBindingSource.MoveFirst();
        }

        public void FilterDecorProducts(int ProductID, int MeasureID)
        {
            DecorItemsSummaryBindingSource.Filter = "ProductID=" + ProductID + " AND MeasureID=" + MeasureID;
            DecorItemsSummaryBindingSource.MoveFirst();
        }

        public void FilterDecorItems(int ProductID, int DecorID, int MeasureID)
        {
            DecorColorsSummaryBindingSource.Filter = "ProductID=" + ProductID + " AND DecorID="
                + DecorID + " AND MeasureID=" + MeasureID;
            DecorColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterDecorSizes(int ProductID, int DecorID, int ColorID, int MeasureID)
        {
            DecorSizesSummaryBindingSource.Filter = "ProductID=" + ProductID +
                " AND DecorID=" + DecorID + " AND ColorID=" + ColorID + " AND MeasureID=" + MeasureID;
            DecorSizesSummaryBindingSource.MoveFirst();
        }

        public void FilterPreDecorProducts(int ProductID, int MeasureID)
        {
            PreDecorItemsSummaryBindingSource.Filter = "ProductID=" + ProductID + " AND MeasureID=" + MeasureID;
            PreDecorItemsSummaryBindingSource.MoveFirst();
        }

        public void FilterPreDecorItems(int ProductID, int DecorID, int MeasureID)
        {
            PreDecorColorsSummaryBindingSource.Filter = "ProductID=" + ProductID + " AND DecorID="
                + DecorID + " AND MeasureID=" + MeasureID;
            PreDecorColorsSummaryBindingSource.MoveFirst();
        }

        public void FilterPreDecorSizes(int ProductID, int DecorID, int ColorID, int MeasureID)
        {
            PreDecorSizesSummaryBindingSource.Filter = "ProductID=" + ProductID +
                " AND DecorID=" + DecorID + " AND ColorID=" + ColorID + " AND MeasureID=" + MeasureID;
            PreDecorSizesSummaryBindingSource.MoveFirst();
        }

        #endregion

        #region Clear functions

        public void ClearBatch()
        {
            BatchDataTable.Clear();
            CurrentMegaBatchID = 0;
        }

        public void ClearMegaOrders()
        {
            BatchMegaOrdersDataTable.Clear();
            CurrentMainOrderID = -1;
        }

        public void ClearGrids()
        {
            BatchMainOrdersDataTable.Clear();
            BatchMainOrdersFrontsOrders.ClearOrders();
            BatchMainOrdersDecorOrders.ClearOrders();
            CurrentBatchMegaOrderID = -1;
        }

        public void ClearProductsGrids()
        {
            FrontsSummaryDataTable.Clear();
            FrameColorsSummaryDataTable.Clear();
            TechnoColorsSummaryDataTable.Clear();
            InsetTypesSummaryDataTable.Clear();
            InsetColorsSummaryDataTable.Clear();
            TechnoInsetTypesSummaryDataTable.Clear();
            TechnoInsetColorsSummaryDataTable.Clear();
            SizesSummaryDataTable.Clear();
            DecorProductsSummaryDataTable.Clear();
            DecorItemsSummaryDataTable.Clear();
            DecorColorsSummaryDataTable.Clear();
            DecorSizesSummaryDataTable.Clear();
        }

        public void ClearPreProductsGrids()
        {
            PreFrontsSummaryDataTable.Clear();
            PreFrameColorsSummaryDataTable.Clear();
            PreTechnoColorsSummaryDataTable.Clear();
            PreInsetTypesSummaryDataTable.Clear();
            PreInsetColorsSummaryDataTable.Clear();
            PreTechnoInsetTypesSummaryDataTable.Clear();
            PreTechnoInsetColorsSummaryDataTable.Clear();
            PreSizesSummaryDataTable.Clear();
            PreDecorProductsSummaryDataTable.Clear();
            PreDecorItemsSummaryDataTable.Clear();
            PreDecorColorsSummaryDataTable.Clear();
            PreDecorSizesSummaryDataTable.Clear();
        }

        #endregion

        #region Batch functions

        /// <summary>
        /// Создает новую группу партий
        /// </summary>
        public void AddMegaBatch(int FactoryID)
        {
            DataRow NewRow = MegaBatchDataTable.NewRow();
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = Security.GetCurrentDate();
            MegaBatchDataTable.Rows.Add(NewRow);

            SaveMegaBatch(FactoryID);
        }

        /// <summary>
        /// Создает новую партию
        /// </summary>
        public void AddBatch(int MegaBatchID, int FactoryID)
        {
            DataRow NewRow = BatchDataTable.NewRow();
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = Security.GetCurrentDate();
            NewRow["MegaBatchID"] = MegaBatchID;
            BatchDataTable.Rows.Add(NewRow);

            SaveBatch(FactoryID);
        }

        /// <summary>
        /// Добавляет подзаказы в новую партию
        /// </summary>
        public bool AddToBatch(int BatchID, int[] MainOrders, int FactoryID)
        {
            if (MainOrders.Count() < 1)
                return false;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM BatchDetails",
                   ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (IsAlreadyInBatch(MainOrders[i], FactoryID))
                                continue;

                            DataRow NewRow = DT.NewRow();
                            NewRow["BatchID"] = BatchID;
                            NewRow["MainOrderID"] = MainOrders[i];
                            NewRow["FactoryID"] = FactoryID;
                            DT.Rows.Add(NewRow);

                            DataRow[] Rows = MainOrdersDataTable.Select("MainOrderID = " + MainOrders[i]);

                            //if (Rows.Count() > 0)
                            //    Rows[0]["BatchID"] = BatchID;
                        }
                        DA.Update(DT);
                    }
                }
            }

            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE BatchID = " + BatchID,
            //       ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        using (DataTable DT = new DataTable())
            //        {
            //            DA.Fill(DT);

            //            if (DT.Rows.Count > 0)
            //            {
            //                DT.Rows[0]["Name"] = BatchName(MainOrders, FactoryID);
            //                DA.Update(DT);
            //            }
            //        }
            //    }
            //}

            SaveBatch(FactoryID);

            return true;
        }

        public void CloseMegaBatch(int MegaBatchID, int FactoryID, bool Enabled)
        {
            DataRow[] rows = MegaBatchDataTable.Select("MegaBatchID=" + MegaBatchID);
            if (rows.Count() > 0)
            {
                if (FactoryID == 1)
                {
                    if (!Enabled)
                    {
                        rows[0]["ProfilBatchClose"] = false;
                        rows[0]["ProfilCloseUserID"] = DBNull.Value;
                        rows[0]["ProfilEntryDateTime"] = DBNull.Value;
                    }
                    else
                    {
                        rows[0]["ProfilBatchClose"] = true;
                        rows[0]["ProfilCloseUserID"] = Security.CurrentUserID;
                        rows[0]["ProfilEntryDateTime"] = Security.GetCurrentDate();
                    }
                }
                if (FactoryID == 2)
                {
                    if (!Enabled)
                    {
                        rows[0]["TPSBatchClose"] = false;
                        rows[0]["TPSCloseUserID"] = DBNull.Value;
                        rows[0]["TPSEntryDateTime"] = DBNull.Value;
                    }
                    else
                    {
                        rows[0]["TPSBatchClose"] = true;
                        rows[0]["TPSCloseUserID"] = Security.CurrentUserID;
                        rows[0]["TPSEntryDateTime"] = Security.GetCurrentDate();
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaBatch" +
                " WHERE MegaBatchID = " + MegaBatchID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            if (FactoryID == 1)
                            {
                                if (!Enabled)
                                {
                                    DT.Rows[0]["ProfilBatchClose"] = false;
                                    DT.Rows[0]["ProfilCloseUserID"] = DBNull.Value;
                                    DT.Rows[0]["ProfilEntryDateTime"] = DBNull.Value;
                                }
                                else
                                {
                                    DT.Rows[0]["ProfilBatchClose"] = true;
                                    DT.Rows[0]["ProfilCloseUserID"] = Security.CurrentUserID;
                                    DT.Rows[0]["ProfilEntryDateTime"] = Security.GetCurrentDate();
                                }
                            }
                            if (FactoryID == 2)
                            {
                                if (!Enabled)
                                {
                                    DT.Rows[0]["TPSBatchClose"] = false;
                                    DT.Rows[0]["TPSCloseUserID"] = DBNull.Value;
                                    DT.Rows[0]["TPSEntryDateTime"] = DBNull.Value;
                                }
                                else
                                {
                                    DT.Rows[0]["TPSBatchClose"] = true;
                                    DT.Rows[0]["TPSCloseUserID"] = Security.CurrentUserID;
                                    DT.Rows[0]["TPSEntryDateTime"] = Security.GetCurrentDate();
                                }
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void MoveBatch(int[] Batch, int DestMegaBatchID, int FactoryID)
        {
            if (Batch.Count() < 1)
                return;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch" +
                " WHERE BatchID IN (" + string.Join(",", Batch) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            DT.Rows[i]["MegaBatchID"] = DestMegaBatchID;
                        }
                        DA.Update(DT);
                    }
                }
            }
            SaveMegaBatch(FactoryID);
        }

        /// <summary>
        /// Переносит выбранные кухни в новую партию
        /// </summary>
        /// <param name="MainOrders"></param>
        /// <param name="FactoryID"></param>
        public void MoveOrdersToNewPart(int[] MainOrders, int FactoryID)
        {
            int MegaBatchID = Convert.ToInt32(((DataRowView)MegaBatchBindingSource.Current).Row["MegaBatchID"]);
            AddBatch(MegaBatchID, FactoryID);

            int NewBatchID = 0;

            //находим BatchID новой созданной партии
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 BatchID FROM Batch ORDER BY BatchID DESC",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            NewBatchID = Convert.ToInt32(DT.Rows[0]["BatchID"]);
                        }
                    }
                }
            }

            //меняем BatchID для заказов, которые переносим
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM BatchDetails" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ") AND FactoryID = " + FactoryID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            DT.Rows[i]["BatchID"] = NewBatchID;
                        }
                        DA.Update(DT);
                    }
                }
            }
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE BatchID = " + NewBatchID,
            //       ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        using (DataTable DT = new DataTable())
            //        {
            //            DA.Fill(DT);

            //            if (DT.Rows.Count > 0)
            //            {
            //                DT.Rows[0]["Name"] = BatchName(MainOrders, FactoryID);
            //                DA.Update(DT);
            //            }
            //        }
            //    }
            //}
            SaveMegaBatch(FactoryID);
        }

        /// <summary>
        /// Переносит выбранные кухни в уже созданную партию
        /// </summary>
        /// <param name="BatchID"></param>
        /// <param name="MainOrders"></param>
        /// <param name="FactoryID"></param>
        public void MoveOrdersToSelectedBatch(int BatchID, int[] MainOrders, int FactoryID)
        {
            //меняем BatchID для заказов, которые переносим
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM BatchDetails" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ") AND FactoryID = " + FactoryID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            DT.Rows[i]["BatchID"] = BatchID;
                        }
                        DA.Update(DT);
                    }
                }
            }
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE BatchID = " + BatchID,
            //       ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        using (DataTable DT = new DataTable())
            //        {
            //            DA.Fill(DT);

            //            if (DT.Rows.Count > 0)
            //            {
            //                DT.Rows[0]["Name"] = BatchName(MainOrders, FactoryID);
            //                DA.Update(DT);
            //            }
            //        }
            //    }
            //}
            SaveMegaBatch(FactoryID);
        }

        /// <summary>
        /// Удаляет выбранную партию
        /// Для тех заказов, которые были в этой партии, нужно выставить статусы согласования, 
        /// поэтому сначала нужно вызвать функцию SetMegaOrderOnProduction(int)
        /// </summary>
        /// <param name="BatchID"></param>
        public void RemoveBatch(int BatchID, int FactoryID)
        {
            if (BatchBindingSource == null || BatchBindingSource.Count < 1)
                return;

            int[] MegaOrders = GetBatchMegaOrders().OfType<int>().ToArray();
            if (MegaOrders.Count() > 0)
            {
                int[] MainOrders = GetBatchMainOrders(BatchID, MegaOrders, FactoryID, false).OfType<int>().ToArray();
                Array.Sort(MainOrders);

                if (MainOrders.Count() < 1)
                    return;

                if (!CanBeRemove(MainOrders, FactoryID))
                {
                    MessageBox.Show("Один или несколько подзаказов уже отправлены в производство", "Ошибка");

                    return;
                }
                SetMainOrdersNotInProduction(MainOrders, FactoryID);
                SetMegaOrdersNotInProduction(MegaOrders, FactoryID);
            }
            //удаляем выбранную партию
            DataRow[] Rows = BatchDataTable.Select("BatchID = " + BatchID);

            if (Rows.Count() > 0)
            {
                foreach (DataRow Row in Rows)
                {
                    Row.Delete();
                }
            }

            ClearBatchDetails(BatchID);
            SaveBatch(FactoryID);

            //остается на позиции удаленного
            if (BatchBindingSource.Count > 0)
            {
                BatchID = Convert.ToInt32(((DataRowView)BatchBindingSource.Current).Row["BatchID"]);
            }
        }

        public void RemoveMegaBatch(int MegaBatchID, int FactoryID)
        {
            if (MegaBatchBindingSource == null || MegaBatchBindingSource.Count < 1)
                return;

            //удаляем выбранную партию
            DataRow[] Rows = MegaBatchDataTable.Select("MegaBatchID = " + MegaBatchID);

            if (Rows.Count() > 0)
            {
                foreach (DataRow Row in Rows)
                {
                    Row.Delete();
                }
            }

            //ClearBatchDetails(MegaBatchID);
            SaveMegaBatch(FactoryID);
        }

        /// <summary>
        /// Удаление выбранного заказа из партии
        /// </summary>
        /// <param name="BatchID"></param>
        public bool RemoveMegaOrderFromBatch(int BatchID, int FactoryID)
        {
            int Pos = BatchMegaOrdersBindingSource.Position;

            int[] MainOrders = GetBatchMainOrders().OfType<int>().ToArray();
            Array.Sort(MainOrders);

            if (MainOrders.Count() < 1)
                return false;

            if (!CanBeRemove(MainOrders, FactoryID))
            {
                MessageBox.Show("Один или несколько подзаказов уже отправлены в производство", "Ошибка");

                return false;
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM BatchDetails WHERE BatchID = " + BatchID +
                " AND FactoryID = " + FactoryID + " AND MainOrderID IN (" + string.Join(",", MainOrders) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            foreach (DataRow Row in DT.Rows)
                            {
                                Row.Delete();
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            //остается на позиции удаленного
            if (BatchMegaOrdersBindingSource.Count > 0)
                if (Pos >= BatchMegaOrdersBindingSource.Count)
                {
                    BatchMegaOrdersBindingSource.MoveLast();
                    BatchMegaOrdersDataGrid.Rows[BatchMegaOrdersDataGrid.Rows.Count - 1].Selected = true;
                }
                else
                    BatchMegaOrdersBindingSource.Position = Pos;

            return true;
        }

        /// <summary>
        /// Очищает таблицу BatchDetails
        /// </summary>
        /// <param name="BatchID">номер партии, которая будет удалена</param>
        public void ClearBatchDetails(int BatchID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM BatchDetails WHERE BatchID = " + BatchID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row.Delete();
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveBatch(int FactoryID)
        {
            int BatchID = CurrentBatchID;

            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string SelectCommand = string.Empty;

            if (FactoryID != 0)
                BatchFactoryFilter = " AND (BatchID IN (SELECT BatchID FROM BatchDetails WHERE BatchDetails.FactoryID = " + FactoryID +
                    ") OR BatchID NOT IN (SELECT BatchID FROM BatchDetails))";

            SelectCommand = @"SELECT * FROM Batch WHERE MegaBatchID = " + CurrentMegaBatchID + BatchFactoryFilter + " ORDER BY BatchID DESC";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM Batch",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(BatchDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    BatchDataTable.Clear();
                    DA.Fill(BatchDataTable);
                }
            }

            //остается на позиции удаленного
            BatchBindingSource.Position = BatchBindingSource.Find("BatchID", BatchID);
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID," +
                " BatchDetails.MainOrderID, MainOrders.MegaOrderID FROM BatchDetails" +
                " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID" +
                " ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                BatchDetailsDataTable.Clear();
                DA.Fill(BatchDetailsDataTable);
            }
        }

        public void SaveMegaBatch(int FactoryID)
        {
            int MegaBatchID = CurrentMegaBatchID;
            string BatchFactoryFilter = string.Empty;
            string SelectCommand = string.Empty;

            if (FactoryID != 0)
                BatchFactoryFilter = " WHERE MegaBatchID IN (SELECT MegaBatchID FROM Batch WHERE BatchID IN (SELECT BatchID FROM BatchDetails WHERE BatchDetails.FactoryID = " + FactoryID +
                    ") OR BatchID NOT IN (SELECT BatchID FROM BatchDetails)) OR MegaBatchID NOT IN (SELECT MegaBatchID FROM Batch)";

            SelectCommand = "SELECT * FROM MegaBatch" + BatchFactoryFilter + " ORDER BY MegaBatchID DESC";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MegaBatchDataTable);
                    MegaBatchDataTable.Clear();
                    DA.Fill(MegaBatchDataTable);
                    FillWeekNumber();
                    SetMegaBatchEnable();
                }
            }
            MegaBatchBindingSource.Position = MegaBatchBindingSource.Find("MegaBatchID", MegaBatchID);
        }
        #endregion

        #region Get functions

        public void PrintedCountUp(int PackageID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PrintedCount, PrintDateTime FROM Packages WHERE PackageID=" + PackageID,
                    ConnectionStrings.MarketingOrdersConnectionString))
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

        public DataTable TempPackages(int MainOrderID, int FactoryID)
        {
            DataTable DT = new DataTable();

            string SelectionCommand = "SELECT * FROM Packages WHERE FactoryID = " + FactoryID + " AND MainOrderID = " + MainOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DT);
            }
            return (DataTable)DT;
        }

        public void GetDecorInfo(ref decimal Pogon, ref int Count)
        {
            for (int i = 0; i < DecorProductsDataGrid.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(DecorProductsDataGrid.SelectedRows[i].Cells["MeasureID"].Value) != 2)
                    Count += Convert.ToInt32(DecorProductsDataGrid.SelectedRows[i].Cells["Count"].Value);
                else
                {
                    Pogon += Convert.ToDecimal(DecorProductsDataGrid.SelectedRows[i].Cells["Count"].Value);
                }
            }

            Pogon = Decimal.Round(Pogon, 2, MidpointRounding.AwayFromZero);
        }

        public void GetFrontsInfo(ref decimal Square, ref int Count)
        {
            for (int i = 0; i < FrontsDataGrid.SelectedRows.Count; i++)
            {
                Square += Convert.ToDecimal(FrontsDataGrid.SelectedRows[i].Cells["Square"].Value);
                Count += Convert.ToInt32(FrontsDataGrid.SelectedRows[i].Cells["Count"].Value);
            }

            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
        }

        public void GetPreDecorInfo(ref decimal Pogon, ref int Count)
        {
            for (int i = 0; i < PreDecorProductsDataGrid.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(PreDecorProductsDataGrid.SelectedRows[i].Cells["MeasureID"].Value) != 2)
                    Count += Convert.ToInt32(PreDecorProductsDataGrid.SelectedRows[i].Cells["Count"].Value);
                else
                {
                    Pogon += Convert.ToDecimal(PreDecorProductsDataGrid.SelectedRows[i].Cells["Count"].Value);
                }
            }

            Pogon = Decimal.Round(Pogon, 2, MidpointRounding.AwayFromZero);
        }

        public void GetPreFrontsInfo(ref decimal Square, ref int Count)
        {
            for (int i = 0; i < PreFrontsDataGrid.SelectedRows.Count; i++)
            {
                Square += Convert.ToDecimal(PreFrontsDataGrid.SelectedRows[i].Cells["Square"].Value);
                Count += Convert.ToInt32(PreFrontsDataGrid.SelectedRows[i].Cells["Count"].Value);
            }

            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Если подзаказ не в партии, то он выделяется зеленоватым цветом
        /// </summary>
        /// <returns></returns>
        /// 
        public int GetMainOrdersNotInBatch(int FactoryID)
        {
            string SelectionCommand = string.Empty;

            if (FactoryID == 1)
                SelectionCommand = "SELECT MainOrders.MainOrderID, MegaOrderID FROM MainOrders" +
                    " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ") AND MainOrderID NOT IN" +
                    " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ")";

            if (FactoryID == 2)
                SelectionCommand = "SELECT MainOrders.MainOrderID, MegaOrderID FROM MainOrders" +
                    " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ") AND MainOrderID NOT IN" +
                    " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                InBatchDataTable.Clear();
                DA.Fill(InBatchDataTable);

                if (MegaOrdersDataGrid.Rows.Count == 0)
                    return 0;

                for (int i = 0; i < MegaOrdersDataGrid.Rows.Count; i++)
                {
                    int MegaOrderID = Convert.ToInt32(MegaOrdersDataGrid.Rows[i].Cells["MegaOrderID"].Value);

                    if (InBatchDataTable.Select("MegaOrderID = " + MegaOrderID).Count() > 0)
                        MegaOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                    else
                        MegaOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }

            SelectionCommand = "SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                InBatchDataTable.Clear();
                DA.Fill(InBatchDataTable);

                if (MainOrdersDataGrid.Rows.Count == 0)
                    return 0;

                for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
                {
                    int MainOrderID = Convert.ToInt32(MainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                    if (InBatchDataTable.Select("MainOrderID = " + MainOrderID).Count() < 1)
                        MainOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                    else
                        MainOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }

                return InBatchDataTable.Rows.Count;
            }
        }

        public bool GetMainOrdersNotInProduction(int FactoryID, bool PickUpFronts)
        {
            NotInProductionDataTable.Clear();
            NotInProductionDataTable.AcceptChanges();

            int OrderNotInProduction = 0;

            string MainOrdersFilter = string.Empty;
            string SelectionCommand = string.Empty;

            if (PickUpFronts)
                MainOrdersFilter = " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM FrontsOrders WHERE FrontID = " + CurrentFrontID + " AND ColorID = " + CurrentFrameColorID + " AND PatinaID = " + CurrentPatinaID + ")";

            if (FactoryID == 1)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ") AND ProfilProductionStatusID = 3" + MainOrdersFilter;

            if (FactoryID == 2)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ") AND TPSProductionStatusID = 3" + MainOrdersFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < BatchMainOrdersDataGrid.Rows.Count; i++)
                    {
                        int MainOrderID = Convert.ToInt32(BatchMainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                        if (DT.Select("MainOrderID = " + MainOrderID).Count() > 0)
                        {
                            OrderNotInProduction++;
                            BatchMainOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                        }
                    }
                }
            }

            return OrderNotInProduction > 0;
        }

        public bool GetMegaOrdersNotInProduction(int BatchID, int[] MegaOrders, int FactoryID, bool PickUpFronts)
        {
            if (MegaOrders.Count() < 1)
                return false;

            NotInProductionDataTable.Clear();
            NotInProductionDataTable.AcceptChanges();

            int OrderNotInProduction = 0;

            string MainOrdersFilter = string.Empty;
            string SelectionCommand = string.Empty;

            if (PickUpFronts)
                MainOrdersFilter = " AND MainOrderID IN" +
                    " (SELECT MainOrderID FROM FrontsOrders WHERE FrontID = " + CurrentFrontID + " AND ColorID = " + CurrentFrameColorID + " AND PatinaID = " + CurrentPatinaID + ")";

            if (FactoryID == 1)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE BatchID = " + BatchID + " AND FactoryID = " + FactoryID + ")" +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ") AND MegaOrderID IN (" + string.Join(",", MegaOrders) + "))" +
                " AND ProfilProductionStatusID = 3" + MainOrdersFilter;

            if (FactoryID == 2)
                SelectionCommand = "SELECT * FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE BatchID = " + BatchID + " AND FactoryID = " + FactoryID + ")" +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ") AND MegaOrderID IN (" + string.Join(",", MegaOrders) + "))" +
                " AND TPSProductionStatusID = 3" + MainOrdersFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DataTable Table = new DataTable();

                    using (DataView DV = new DataView(DT))
                    {
                        Table = DV.ToTable(true, new string[] { "MegaOrderID" });
                    }

                    foreach (DataRow Row in Table.Rows)
                        NotInProductionDataTable.ImportRow(Row);

                    Table.Dispose();

                    for (int i = 0; i < BatchMegaOrdersDataGrid.Rows.Count; i++)
                    {
                        int MegaOrderID = Convert.ToInt32(BatchMegaOrdersDataGrid.Rows[i].Cells["MegaOrderID"].Value);

                        if (NotInProductionDataTable.Select("MegaOrderID = " + MegaOrderID).Count() > 0)
                            BatchMegaOrdersDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(211, 249, 211);
                    }
                }
            }

            return OrderNotInProduction > 0;
        }

        public ArrayList GetSelectedMainOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value));

            return array;
        }

        public ArrayList GetSelectedMegaOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MegaOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(MegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value));

            return array;
        }

        public ArrayList GetMainOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < MainOrdersDataGrid.Rows.Count; i++)
                array.Add(Convert.ToInt32(MainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value));

            return array;
        }

        public ArrayList GetBatchMegaOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < BatchMegaOrdersDataGrid.Rows.Count; i++)
                array.Add(Convert.ToInt32(BatchMegaOrdersDataGrid.Rows[i].Cells["MegaOrderID"].Value));

            return array;
        }

        public ArrayList GetBatchMainOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < BatchMainOrdersDataGrid.Rows.Count; i++)
                array.Add(Convert.ToInt32(BatchMainOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value));

            return array;
        }

        public ArrayList GetSelectedBatchMainOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < BatchMainOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(BatchMainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value));

            return array;
        }

        public ArrayList GetSelectedBatchMegaOrders()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < BatchMegaOrdersDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(BatchMegaOrdersDataGrid.SelectedRows[i].Cells["MegaOrderID"].Value));

            return array;
        }

        public ArrayList GetBatchMainOrders(int BatchID, int[] MegaOrders, int FactoryID, bool PickUpFronts)
        {
            ArrayList array = new ArrayList();

            string MainOrdersFilter = string.Empty;

            if (PickUpFronts)
                MainOrdersFilter = " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM FrontsOrders" +
                        " WHERE FrontsOrders.FrontID = " + CurrentFrontID + " AND FrontsOrders.ColorID = " + CurrentFrameColorID + " AND FrontsOrders.PatinaID = " + CurrentPatinaID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT BatchDetails.MainOrderID, MegaOrders.MegaOrderID," +
                " infiniu2_marketingreference.dbo.Clients.ClientName AS ClientName FROM BatchDetails" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE BatchID = " + BatchID + " AND BatchDetails.FactoryID = " + FactoryID + " AND BatchDetails.MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")" +
                " AND MainOrders.MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainOrdersFilter + ")" +
                " ORDER BY ClientName, MegaOrders.MegaOrderID, BatchDetails.MainOrderID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                    }
                }
            }

            return array;
        }

        public ArrayList GetSelectedBatch()
        {
            ArrayList array = new ArrayList();

            for (int i = 0; i < BatchDataGrid.SelectedRows.Count; i++)
                array.Add(Convert.ToInt32(BatchDataGrid.SelectedRows[i].Cells["BatchID"].Value));

            return array;
        }

        //номера подказаков для переноса
        public ArrayList GetBatchMainOrders(
            int[] MegaOrders, int BatchID, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            bool PickUpFronts)
        {
            ArrayList array = new ArrayList();

            string MainOrdersFilter = string.Empty;

            if (PickUpFronts)
                MainOrdersFilter = " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM FrontsOrders" +
                        " WHERE FrontID = " + CurrentFrontID + " AND ColorID = " + CurrentFrameColorID + " AND PatinaID = " + CurrentPatinaID + ")";

            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")";

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + FactoryFilter +
                " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM BatchDetails" +
                " WHERE BatchID = " + BatchID + " AND FactoryID = " + FactoryID + ")" + MainProductionStatus + MainOrdersFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                }
            }

            return array;
        }

        //номера подказаков для переноса
        public ArrayList GetBatchMainOrders(
            int[] MegaOrders, int BatchID, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            bool PickUpFronts,
            ArrayList Fronts)
        {
            ArrayList array = new ArrayList();

            string MainOrdersFilter = string.Empty;

            if (PickUpFronts)
                MainOrdersFilter = " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM FrontsOrders" +
                        " WHERE FrontID IN (" + string.Join(",", Fronts.OfType<int>().ToArray()) + "))";

            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (MainOrders.FactoryID = 0 OR MainOrders.FactoryID = " + FactoryID + ")";

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + FactoryFilter +
                " AND MainOrders.MainOrderID IN (SELECT MainOrderID FROM BatchDetails" +
                " WHERE BatchID = " + BatchID + " AND FactoryID = " + FactoryID + ")" + MainProductionStatus + MainOrdersFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                }
            }

            return array;
        }

        private bool GetFronts(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            decimal FrontSquare = 0;
            int FrontCount = 0;

            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            FrontsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrontSquare += Convert.ToDecimal(row["Square"]);
                        FrontCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = FrontsSummaryDataTable.NewRow();
                    NewRow["Front"] = GetFrontName(Convert.ToInt32(Table.Rows[i]["FrontID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["Square"] = Decimal.Round(FrontSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrontCount;
                    FrontsSummaryDataTable.Rows.Add(NewRow);

                    FrontSquare = 0;
                    FrontCount = 0;
                }
            }

            Table.Dispose();
            FrontsSummaryDataTable.DefaultView.Sort = "Front";
            FrontsSummaryBindingSource.MoveFirst();
            FrontsOrdersDataTable.Dispose();

            return FrontsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetFronts(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            decimal FrontSquare = 0;
            int FrontCount = 0;

            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            FrontsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrontSquare += Convert.ToDecimal(row["Square"]);
                        FrontCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = FrontsSummaryDataTable.NewRow();
                    NewRow["Front"] = GetFrontName(Convert.ToInt32(Table.Rows[i]["FrontID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["Square"] = Decimal.Round(FrontSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrontCount;
                    FrontsSummaryDataTable.Rows.Add(NewRow);

                    FrontSquare = 0;
                    FrontCount = 0;
                }
            }

            Table.Dispose();
            FrontsSummaryDataTable.DefaultView.Sort = "Front";
            FrontsSummaryBindingSource.MoveFirst();
            FrontsOrdersDataTable.Dispose();

            return FrontsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetFrameColors(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal FrameColorSquare = 0;
            int FrameColorCount = 0;
            FrameColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrameColorSquare += Convert.ToDecimal(row["Square"]);
                        FrameColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = FrameColorsSummaryDataTable.NewRow();
                    if (Convert.ToInt32(Table.Rows[i]["PatinaID"]) == -1)
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));
                    else
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"])) + " + " + GetPatinaName(Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["Square"] = Decimal.Round(FrameColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrameColorCount;
                    FrameColorsSummaryDataTable.Rows.Add(NewRow);

                    FrameColorSquare = 0;
                    FrameColorCount = 0;
                }
            }
            Table.Dispose();
            FrameColorsSummaryDataTable.DefaultView.Sort = "FrameColor";
            FrameColorsSummaryBindingSource.MoveFirst();

            return FrameColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetFrameColors(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal FrameColorSquare = 0;
            int FrameColorCount = 0;
            FrameColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrameColorSquare += Convert.ToDecimal(row["Square"]);
                        FrameColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = FrameColorsSummaryDataTable.NewRow();
                    if (Convert.ToInt32(Table.Rows[i]["PatinaID"]) == -1)
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));
                    else
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"])) + " + " + GetPatinaName(Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["Square"] = Decimal.Round(FrameColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrameColorCount;
                    FrameColorsSummaryDataTable.Rows.Add(NewRow);

                    FrameColorSquare = 0;
                    FrameColorCount = 0;
                }
            }
            Table.Dispose();
            FrameColorsSummaryDataTable.DefaultView.Sort = "FrameColor";
            FrameColorsSummaryBindingSource.MoveFirst();

            return FrameColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetTechnoColors(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal Square = 0;
            int Count = 0;

            TechnoColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " ");
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        Square += Convert.ToDecimal(row["Square"]);
                        Count += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = TechnoColorsSummaryDataTable.NewRow();
                    NewRow["TechnoColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["TechnoColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["Square"] = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = Count;
                    TechnoColorsSummaryDataTable.Rows.Add(NewRow);

                    Square = 0;
                    Count = 0;
                }
            }
            Table.Dispose();
            TechnoColorsSummaryDataTable.DefaultView.Sort = "TechnoColor";
            TechnoColorsSummaryBindingSource.MoveFirst();

            return TechnoColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetTechnoColors(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal Square = 0;
            int Count = 0;

            TechnoColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " ");
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        Square += Convert.ToDecimal(row["Square"]);
                        Count += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = TechnoColorsSummaryDataTable.NewRow();
                    NewRow["TechnoColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["TechnoColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["Square"] = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = Count;
                    TechnoColorsSummaryDataTable.Rows.Add(NewRow);

                    Square = 0;
                    Count = 0;
                }
            }
            Table.Dispose();
            TechnoColorsSummaryDataTable.DefaultView.Sort = "TechnoColor";
            TechnoColorsSummaryBindingSource.MoveFirst();

            return TechnoColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetInsetTypes(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetTypeSquare = 0;
            int InsetTypeCount = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;
            decimal GridHeight = 0;
            decimal GridWidth = 0;

            InsetTypesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        GetGridMargins(Convert.ToInt32(row["FrontID"]), ref MarginHeight, ref MarginWidth);
                        GridHeight = Convert.ToInt32(Convert.ToInt32(row["Height"]) - MarginHeight);
                        GridWidth = Convert.ToInt32(Convert.ToInt32(row["Width"]) - MarginWidth);
                        if (GridHeight < 0 || GridWidth < 0)
                        {
                            GridHeight = 0;
                            GridWidth = 0;
                        }
                        InsetTypeSquare += Decimal.Round(GridHeight * GridWidth * Convert.ToInt32(row["Count"]) / 1000000, 2, MidpointRounding.AwayFromZero);
                        InsetTypeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = InsetTypesSummaryDataTable.NewRow();
                    NewRow["InsetType"] = GetInsetTypeName(Convert.ToInt32(Table.Rows[i]["InsetTypeID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Table.Rows[i]["InsetTypeID"];
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetTypeSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetTypeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetTypeCount;
                    InsetTypesSummaryDataTable.Rows.Add(NewRow);

                    InsetTypeSquare = 0;
                    InsetTypeCount = 0;
                }
            }
            Table.Dispose();
            InsetTypesSummaryDataTable.DefaultView.Sort = "InsetType";
            InsetTypesSummaryBindingSource.MoveFirst();

            return InsetTypesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetInsetTypes(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetTypeSquare = 0;
            int InsetTypeCount = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;
            decimal GridHeight = 0;
            decimal GridWidth = 0;

            InsetTypesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        GetGridMargins(Convert.ToInt32(row["FrontID"]), ref MarginHeight, ref MarginWidth);
                        GridHeight = Convert.ToInt32(Convert.ToInt32(row["Height"]) - MarginHeight);
                        GridWidth = Convert.ToInt32(Convert.ToInt32(row["Width"]) - MarginWidth);
                        if (GridHeight < 0 || GridWidth < 0)
                        {
                            GridHeight = 0;
                            GridWidth = 0;
                        }
                        InsetTypeSquare += Decimal.Round(GridHeight * GridWidth * Convert.ToInt32(row["Count"]) / 1000000, 2, MidpointRounding.AwayFromZero);
                        InsetTypeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = InsetTypesSummaryDataTable.NewRow();
                    NewRow["InsetType"] = GetInsetTypeName(Convert.ToInt32(Table.Rows[i]["InsetTypeID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Table.Rows[i]["InsetTypeID"];
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetTypeSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetTypeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetTypeCount;
                    InsetTypesSummaryDataTable.Rows.Add(NewRow);

                    InsetTypeSquare = 0;
                    InsetTypeCount = 0;
                }
            }
            Table.Dispose();
            InsetTypesSummaryDataTable.DefaultView.Sort = "InsetType";
            InsetTypesSummaryBindingSource.MoveFirst();

            return InsetTypesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetInsetColors(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetColorSquare = 0;
            int InsetColorCount = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;
            decimal GridHeight = 0;
            decimal GridWidth = 0;

            InsetColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        GetGridMargins(Convert.ToInt32(row["FrontID"]), ref MarginHeight, ref MarginWidth);
                        GridHeight = Convert.ToInt32(Convert.ToInt32(row["Height"]) - MarginHeight);
                        GridWidth = Convert.ToInt32(Convert.ToInt32(row["Width"]) - MarginWidth);
                        if (GridHeight < 0 || GridWidth < 0)
                        {
                            GridHeight = 0;
                            GridWidth = 0;
                        }
                        InsetColorSquare += Decimal.Round(GridHeight * GridWidth * Convert.ToInt32(row["Count"]) / 1000000, 2, MidpointRounding.AwayFromZero);
                        InsetColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = InsetColorsSummaryDataTable.NewRow();
                    NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Table.Rows[i]["InsetColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetColorSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetColorCount;
                    InsetColorsSummaryDataTable.Rows.Add(NewRow);

                    InsetColorSquare = 0;
                    InsetColorCount = 0;
                }
            }
            Table.Dispose();
            InsetColorsSummaryDataTable.DefaultView.Sort = "InsetColor";
            InsetColorsSummaryBindingSource.MoveFirst();
            return InsetColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetInsetColors(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetColorSquare = 0;
            int InsetColorCount = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;
            decimal GridHeight = 0;
            decimal GridWidth = 0;

            InsetColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        GetGridMargins(Convert.ToInt32(row["FrontID"]), ref MarginHeight, ref MarginWidth);
                        GridHeight = Convert.ToInt32(Convert.ToInt32(row["Height"]) - MarginHeight);
                        GridWidth = Convert.ToInt32(Convert.ToInt32(row["Width"]) - MarginWidth);
                        if (GridHeight < 0 || GridWidth < 0)
                        {
                            GridHeight = 0;
                            GridWidth = 0;
                        }
                        InsetColorSquare += Decimal.Round(GridHeight * GridWidth * Convert.ToInt32(row["Count"]) / 1000000, 2, MidpointRounding.AwayFromZero);
                        InsetColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = InsetColorsSummaryDataTable.NewRow();
                    NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Table.Rows[i]["InsetColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetColorSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetColorCount;
                    InsetColorsSummaryDataTable.Rows.Add(NewRow);

                    InsetColorSquare = 0;
                    InsetColorCount = 0;
                }
            }
            Table.Dispose();
            InsetColorsSummaryDataTable.DefaultView.Sort = "InsetColor";
            InsetColorsSummaryBindingSource.MoveFirst();

            return InsetColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetTechnoInsetTypes(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetTypeSquare = 0;
            int InsetTypeCount = 0;

            TechnoInsetTypesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        InsetTypeSquare += Convert.ToDecimal(row["Square"]);
                        InsetTypeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = TechnoInsetTypesSummaryDataTable.NewRow();
                    NewRow["TechnoInsetType"] = GetInsetTypeName(Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Table.Rows[i]["InsetTypeID"];
                    NewRow["InsetColorID"] = Table.Rows[i]["InsetColorID"];
                    NewRow["TechnoInsetTypeID"] = Table.Rows[i]["TechnoInsetTypeID"];
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetTypeSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetTypeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetTypeCount;
                    TechnoInsetTypesSummaryDataTable.Rows.Add(NewRow);

                    InsetTypeSquare = 0;
                    InsetTypeCount = 0;
                }
            }
            Table.Dispose();
            TechnoInsetTypesSummaryDataTable.DefaultView.Sort = "TechnoInsetType";
            TechnoInsetTypesSummaryBindingSource.MoveFirst();

            return TechnoInsetTypesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetTechnoInsetTypes(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetTypeSquare = 0;
            int InsetTypeCount = 0;

            TechnoInsetTypesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        InsetTypeSquare += Convert.ToDecimal(row["Square"]);
                        InsetTypeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = TechnoInsetTypesSummaryDataTable.NewRow();
                    NewRow["TechnoInsetType"] = GetInsetTypeName(Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Table.Rows[i]["InsetTypeID"];
                    NewRow["InsetColorID"] = Table.Rows[i]["InsetColorID"];
                    NewRow["TechnoInsetTypeID"] = Table.Rows[i]["TechnoInsetTypeID"];
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetTypeSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetTypeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetTypeCount;
                    TechnoInsetTypesSummaryDataTable.Rows.Add(NewRow);

                    InsetTypeSquare = 0;
                    InsetTypeCount = 0;
                }
            }
            Table.Dispose();
            TechnoInsetTypesSummaryDataTable.DefaultView.Sort = "TechnoInsetType";
            TechnoInsetTypesSummaryBindingSource.MoveFirst();

            return TechnoInsetTypesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetTechnoInsetColors(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetColorSquare = 0;
            int InsetColorCount = 0;

            TechnoInsetColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]) +
                    " AND TechnoInsetColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        InsetColorSquare += Convert.ToDecimal(row["Square"]);
                        InsetColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = TechnoInsetColorsSummaryDataTable.NewRow();
                    NewRow["TechnoInsetColor"] = GetInsetColorName(Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    NewRow["TechnoInsetTypeID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]);
                    NewRow["TechnoInsetColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]);
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetColorSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetColorCount;
                    TechnoInsetColorsSummaryDataTable.Rows.Add(NewRow);

                    InsetColorSquare = 0;
                    InsetColorCount = 0;
                }
            }
            Table.Dispose();
            TechnoInsetColorsSummaryDataTable.DefaultView.Sort = "TechnoInsetColor";
            TechnoInsetColorsSummaryBindingSource.MoveFirst();

            return TechnoInsetColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetTechnoInsetColors(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetColorSquare = 0;
            int InsetColorCount = 0;

            TechnoInsetColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]) +
                    " AND TechnoInsetColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        InsetColorSquare += Convert.ToDecimal(row["Square"]);
                        InsetColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = TechnoInsetColorsSummaryDataTable.NewRow();
                    NewRow["TechnoInsetColor"] = GetInsetColorName(Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    NewRow["TechnoInsetTypeID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]);
                    NewRow["TechnoInsetColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]);
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetColorSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetColorCount;
                    TechnoInsetColorsSummaryDataTable.Rows.Add(NewRow);

                    InsetColorSquare = 0;
                    InsetColorCount = 0;
                }
            }
            Table.Dispose();
            TechnoInsetColorsSummaryDataTable.DefaultView.Sort = "TechnoInsetColor";
            TechnoInsetColorsSummaryBindingSource.MoveFirst();

            return TechnoInsetColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetSizes(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND FrontsOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal SizeSquare = 0;
            int SizeCount = 0;
            SizesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID", "Height", "Width" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]) +
                    " AND TechnoInsetColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]) +
                    " AND Height=" + Convert.ToInt32(Table.Rows[i]["Height"]) +
                    " AND Width=" + Convert.ToInt32(Table.Rows[i]["Width"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        SizeSquare += Convert.ToDecimal(row["Square"]);
                        SizeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = SizesSummaryDataTable.NewRow();
                    NewRow["Size"] = Convert.ToInt32(Table.Rows[i]["Height"]) + " x " + Convert.ToInt32(Table.Rows[i]["Width"]);
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["TechnoInsetColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]);
                    NewRow["TechnoInsetTypeID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]);
                    NewRow["Height"] = Convert.ToInt32(Table.Rows[i]["Height"]);
                    NewRow["Width"] = Convert.ToInt32(Table.Rows[i]["Width"]);
                    NewRow["Square"] = Decimal.Round(SizeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = SizeCount;
                    SizesSummaryDataTable.Rows.Add(NewRow);

                    SizeSquare = 0;
                    SizeCount = 0;
                }
            }
            Table.Dispose();
            SizesSummaryDataTable.DefaultView.Sort = "Square DESC";
            SizesSummaryBindingSource.MoveFirst();

            return SizesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetSizes(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DataTable FrontsOrdersDataTable = new DataTable();

            SelectCommand = "SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal SizeSquare = 0;
            int SizeCount = 0;
            SizesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID", "Height", "Width" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]) +
                    " AND TechnoInsetColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]) +
                    " AND Height=" + Convert.ToInt32(Table.Rows[i]["Height"]) +
                    " AND Width=" + Convert.ToInt32(Table.Rows[i]["Width"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        SizeSquare += Convert.ToDecimal(row["Square"]);
                        SizeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = SizesSummaryDataTable.NewRow();
                    NewRow["Size"] = Convert.ToInt32(Table.Rows[i]["Height"]) + " x " + Convert.ToInt32(Table.Rows[i]["Width"]);
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["TechnoInsetColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]);
                    NewRow["TechnoInsetTypeID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]);
                    NewRow["Height"] = Convert.ToInt32(Table.Rows[i]["Height"]);
                    NewRow["Width"] = Convert.ToInt32(Table.Rows[i]["Width"]);
                    NewRow["Square"] = Decimal.Round(SizeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = SizeCount;
                    SizesSummaryDataTable.Rows.Add(NewRow);

                    SizeSquare = 0;
                    SizeCount = 0;
                }
            }
            Table.Dispose();
            SizesSummaryDataTable.DefaultView.Sort = "Square DESC";
            SizesSummaryBindingSource.MoveFirst();

            return SizesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorProducts(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            decimal DecorProductCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorProductsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorProductCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorProductCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorProductsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorProduct"] = GetProductName(Convert.ToInt32(Table.Rows[i]["ProductID"]));
                if (DecorProductCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorProductCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorProductsSummaryDataTable.Rows.Add(NewRow);

                Measure = "";
                DecorProductCount = 0;
            }
            DecorProductsSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorProductsSummaryBindingSource.MoveFirst();

            return DecorProductsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorProducts(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            decimal DecorProductCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorProductsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorProductCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorProductCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorProductsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorProduct"] = GetProductName(Convert.ToInt32(Table.Rows[i]["ProductID"]));
                if (DecorProductCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorProductCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorProductsSummaryDataTable.Rows.Add(NewRow);

                Measure = "";
                DecorProductCount = 0;
            }
            DecorProductsSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorProductsSummaryBindingSource.MoveFirst();

            return DecorProductsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorItems(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            decimal DecorItemCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorItemsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorItemCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorItemCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorItemsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorItem"] = GetDecorName(Convert.ToInt32(Table.Rows[i]["DecorID"]));
                if (DecorItemCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorItemCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorItemsSummaryDataTable.Rows.Add(NewRow);

                Measure = "";
                DecorItemCount = 0;
            }
            Table.Dispose();
            DecorItemsSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorItemsSummaryBindingSource.MoveFirst();

            return DecorItemsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorItems(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            decimal DecorItemCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorItemsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorItemCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorItemCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorItemsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorItem"] = GetDecorName(Convert.ToInt32(Table.Rows[i]["DecorID"]));
                if (DecorItemCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorItemCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorItemsSummaryDataTable.Rows.Add(NewRow);

                Measure = "";
                DecorItemCount = 0;
            }
            Table.Dispose();
            DecorItemsSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorItemsSummaryBindingSource.MoveFirst();

            return DecorItemsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorColors(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            decimal DecorColorCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        if (row["Height"].ToString() == "-1")
                            DecorColorCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorColorCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorColorCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorColorCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorColorCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorColorCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorColorsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["Color"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));
                if (DecorColorCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorColorCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorColorsSummaryDataTable.Rows.Add(NewRow);

                Measure = string.Empty;
                DecorColorCount = 0;
            }
            Table.Dispose();
            DecorColorsSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorColorsSummaryBindingSource.MoveFirst();

            return DecorItemsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorColors(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            decimal DecorColorCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        if (row["Height"].ToString() == "-1")
                            DecorColorCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorColorCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorColorCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorColorCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorColorCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorColorCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorColorsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["Color"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));
                if (DecorColorCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorColorCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorColorsSummaryDataTable.Rows.Add(NewRow);

                Measure = string.Empty;
                DecorColorCount = 0;
            }
            Table.Dispose();
            DecorColorsSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorColorsSummaryBindingSource.MoveFirst();

            return DecorItemsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorSizes(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            decimal DecorSizeCount = 0;
            int decimals = 2;
            int Height = 0;
            int Length = 0;
            int Width = 0;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string Sizes = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorSizesSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            if (ClientGroups.Count() > 0)
                GroupsFilter = " AND MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID IN (SELECT ClientID FROM infiniu2_marketingreference.dbo.Clients" +
                    " WHERE ClientGroupID IN (" + string.Join(",", ClientGroups) + ")))";
            else
                GroupsFilter = " AND MainOrders.MegaOrderID = -1";

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID" +
                " FROM BatchDetails WHERE BatchID IN (" + string.Join(",", Batches) + ")" + BatchFactoryFilter + ")" +
                MainProductionStatus + GroupsFilter + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "MeasureID", "Length", "Height", "Width" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]) +
                    " AND Length=" + Convert.ToInt32(Table.Rows[i]["Length"]) +
                    " AND Height=" + Convert.ToInt32(Table.Rows[i]["Height"]) +
                    " AND Width=" + Convert.ToInt32(Table.Rows[i]["Width"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        if (row["Height"].ToString() == "-1")
                            DecorSizeCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorSizeCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorSizeCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorSizeCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorSizeCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorSizeCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorSizesSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                if (DecorSizeCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorSizeCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;

                Height = Convert.ToInt32(Table.Rows[i]["Height"]);
                Length = Convert.ToInt32(Table.Rows[i]["Length"]);
                Width = Convert.ToInt32(Table.Rows[i]["Width"]);

                if (Height > -1)
                    Sizes = Height.ToString();

                if (Sizes != string.Empty)
                {
                    if (Width > -1)
                        Sizes += " x " + Width.ToString();
                }
                else
                {
                    if (Length > -1)
                    {
                        Sizes = Length.ToString();
                        if (Width > -1)
                            Sizes += " x " + Width.ToString();
                    }
                    else
                    {
                        if (Width > -1)
                            Sizes = Width.ToString();
                    }
                }

                DecorSizesSummaryDataTable.Rows.Add(NewRow);
                NewRow["Size"] = Sizes;
                Sizes = string.Empty;
                Measure = string.Empty;
                DecorSizeCount = 0;
            }
            Table.Dispose();
            DecorSizesSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorSizesSummaryBindingSource.MoveFirst();

            return DecorSizesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetDecorSizes(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            decimal DecorSizeCount = 0;
            int decimals = 2;
            int Height = 0;
            int Length = 0;
            int Width = 0;
            string Measure = string.Empty;
            string GroupsFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string FactoryFilter = string.Empty;
            string MainProductionStatus = string.Empty;
            string Sizes = string.Empty;
            string SelectCommand = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (OnExp)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (MainProductionStatus.Length > 0)
                {
                    MainProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    MainProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            if (FactoryID != 0)
            {
                BatchFactoryFilter = " AND BatchDetails.FactoryID = " + FactoryID;
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorSizesSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            SelectCommand = "SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")" +
                FactoryFilter + MainProductionStatus + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "MeasureID", "Length", "Height", "Width" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]) +
                    " AND Length=" + Convert.ToInt32(Table.Rows[i]["Length"]) +
                    " AND Height=" + Convert.ToInt32(Table.Rows[i]["Height"]) +
                    " AND Width=" + Convert.ToInt32(Table.Rows[i]["Width"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        if (row["Height"].ToString() == "-1")
                            DecorSizeCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorSizeCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorSizeCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorSizeCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorSizeCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorSizeCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorSizesSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                if (DecorSizeCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorSizeCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;

                Height = Convert.ToInt32(Table.Rows[i]["Height"]);
                Length = Convert.ToInt32(Table.Rows[i]["Length"]);
                Width = Convert.ToInt32(Table.Rows[i]["Width"]);

                if (Height > -1)
                    Sizes = Height.ToString();

                if (Sizes != string.Empty)
                {
                    if (Width > -1)
                        Sizes += " x " + Width.ToString();
                }
                else
                {
                    if (Length > -1)
                    {
                        Sizes = Length.ToString();
                        if (Width > -1)
                            Sizes += " x " + Width.ToString();
                    }
                    else
                    {
                        if (Width > -1)
                            Sizes = Width.ToString();
                    }
                }

                DecorSizesSummaryDataTable.Rows.Add(NewRow);
                NewRow["Size"] = Sizes;
                Sizes = string.Empty;
                Measure = string.Empty;
                DecorSizeCount = 0;
            }
            Table.Dispose();
            DecorSizesSummaryDataTable.DefaultView.Sort = "Count DESC";
            DecorSizesSummaryBindingSource.MoveFirst();

            return DecorSizesSummaryDataTable.Rows.Count > 0;
        }

        public void GetProductInfo(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched)
        {
            GetFronts(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetFrameColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetTechnoColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetInsetTypes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetInsetColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetTechnoInsetTypes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetTechnoInsetColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetSizes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetDecorProducts(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetDecorItems(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetDecorColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
            GetDecorSizes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched);
        }

        public void GetProductInfo(
            int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExp,
            bool Dispatched,
            int[] Batches,
            int[] ClientGroups)
        {
            GetFronts(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetFrameColors(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetTechnoColors(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetInsetTypes(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetInsetColors(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetTechnoInsetTypes(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetTechnoInsetColors(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetSizes(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetDecorProducts(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetDecorItems(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetDecorColors(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
            GetDecorSizes(FactoryID, OnProduction, NotProduction, InProduction, OnStorage, OnExp, Dispatched, Batches, ClientGroups);
        }

        private bool GetPreFronts(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal FrontSquare = 0;
            int FrontCount = 0;

            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            PreFrontsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" +
                MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrontSquare += Convert.ToDecimal(row["Square"]);
                        FrontCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreFrontsSummaryDataTable.NewRow();
                    NewRow["Front"] = GetFrontName(Convert.ToInt32(Table.Rows[i]["FrontID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["Square"] = Decimal.Round(FrontSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrontCount;
                    PreFrontsSummaryDataTable.Rows.Add(NewRow);

                    FrontSquare = 0;
                    FrontCount = 0;
                }
            }

            Table.Dispose();
            PreFrontsSummaryDataTable.DefaultView.Sort = "Square DESC";
            PreFrontsSummaryBindingSource.MoveFirst();

            return PreFrontsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreFrameColors(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal FrameColorSquare = 0;
            int FrameColorCount = 0;

            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            PreFrameColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrameColorSquare += Convert.ToDecimal(row["Square"]);
                        FrameColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreFrameColorsSummaryDataTable.NewRow();
                    if (Convert.ToInt32(Table.Rows[i]["PatinaID"]) == -1)
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));
                    else
                        NewRow["FrameColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"])) + " + " + GetPatinaName(Convert.ToInt32(Table.Rows[i]["PatinaID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["Square"] = Decimal.Round(FrameColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrameColorCount;
                    PreFrameColorsSummaryDataTable.Rows.Add(NewRow);

                    FrameColorSquare = 0;
                    FrameColorCount = 0;
                }
            }
            Table.Dispose();
            PreFrameColorsSummaryDataTable.DefaultView.Sort = "FrameColor";
            PreFrameColorsSummaryBindingSource.MoveFirst();

            return PreFrameColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreTechnoColors(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal Square = 0;
            int Count = 0;

            PreTechnoColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " ");
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        Square += Convert.ToDecimal(row["Square"]);
                        Count += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreTechnoColorsSummaryDataTable.NewRow();
                    NewRow["TechnoColor"] = GetColorName(Convert.ToInt32(Table.Rows[i]["TechnoColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["Square"] = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = Count;
                    PreTechnoColorsSummaryDataTable.Rows.Add(NewRow);

                    Square = 0;
                    Count = 0;
                }
            }
            Table.Dispose();
            PreTechnoColorsSummaryDataTable.DefaultView.Sort = "TechnoColor";
            PreTechnoColorsSummaryBindingSource.MoveFirst();

            return PreTechnoColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreInsetTypes(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" +
                MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetTypeSquare = 0;
            int InsetTypeCount = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;
            decimal GridHeight = 0;
            decimal GridWidth = 0;

            PreInsetTypesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        GetGridMargins(Convert.ToInt32(row["FrontID"]), ref MarginHeight, ref MarginWidth);
                        GridHeight = Convert.ToInt32(Convert.ToInt32(row["Height"]) - MarginHeight);
                        GridWidth = Convert.ToInt32(Convert.ToInt32(row["Width"]) - MarginWidth);
                        if (GridHeight < 0 || GridWidth < 0)
                        {
                            GridHeight = 0;
                            GridWidth = 0;
                        }
                        InsetTypeSquare += Decimal.Round(GridHeight * GridWidth * Convert.ToInt32(row["Count"]) / 1000000, 2, MidpointRounding.AwayFromZero);
                        InsetTypeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreInsetTypesSummaryDataTable.NewRow();
                    NewRow["InsetType"] = GetInsetTypeName(Convert.ToInt32(Table.Rows[i]["InsetTypeID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Table.Rows[i]["InsetTypeID"];
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetTypeSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetTypeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetTypeCount;
                    PreInsetTypesSummaryDataTable.Rows.Add(NewRow);

                    InsetTypeSquare = 0;
                    InsetTypeCount = 0;
                }
            }
            Table.Dispose();
            PreInsetTypesSummaryDataTable.DefaultView.Sort = "InsetType";
            PreInsetTypesSummaryBindingSource.MoveFirst();

            return PreInsetTypesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreInsetColors(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal InsetColorSquare = 0;
            int InsetColorCount = 0;

            int MarginHeight = 0;
            int MarginWidth = 0;
            decimal GridHeight = 0;
            decimal GridWidth = 0;

            PreInsetColorsSummaryDataTable.Clear();

            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable Table = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID", "InsetColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) + " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        GetGridMargins(Convert.ToInt32(row["FrontID"]), ref MarginHeight, ref MarginWidth);
                        GridHeight = Convert.ToInt32(Convert.ToInt32(row["Height"]) - MarginHeight);
                        GridWidth = Convert.ToInt32(Convert.ToInt32(row["Width"]) - MarginWidth);
                        if (GridHeight < 0 || GridWidth < 0)
                        {
                            GridHeight = 0;
                            GridWidth = 0;
                        }
                        InsetColorSquare += Decimal.Round(GridHeight * GridWidth * Convert.ToInt32(row["Count"]) / 1000000, 2, MidpointRounding.AwayFromZero);
                        InsetColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreInsetColorsSummaryDataTable.NewRow();
                    NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Table.Rows[i]["InsetColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetColorSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetColorCount;
                    PreInsetColorsSummaryDataTable.Rows.Add(NewRow);

                    InsetColorSquare = 0;
                    InsetColorCount = 0;
                }
            }
            Table.Dispose();
            PreInsetColorsSummaryDataTable.DefaultView.Sort = "InsetColor";
            PreInsetColorsSummaryBindingSource.MoveFirst();

            return PreInsetColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreTechnoInsetTypes(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" +
                MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetTypeSquare = 0;
            int InsetTypeCount = 0;

            PreTechnoInsetTypesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID", "TechnoColorID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        InsetTypeSquare += Convert.ToDecimal(row["Square"]);
                        InsetTypeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreTechnoInsetTypesSummaryDataTable.NewRow();
                    NewRow["TechnoInsetType"] = GetInsetTypeName(Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Table.Rows[i]["InsetTypeID"];
                    NewRow["InsetColorID"] = Table.Rows[i]["InsetColorID"];
                    NewRow["TechnoInsetTypeID"] = Table.Rows[i]["TechnoInsetTypeID"];
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetTypeSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetTypeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetTypeCount;
                    PreTechnoInsetTypesSummaryDataTable.Rows.Add(NewRow);

                    InsetTypeSquare = 0;
                    InsetTypeCount = 0;
                }
            }
            Table.Dispose();
            PreTechnoInsetTypesSummaryDataTable.DefaultView.Sort = "TechnoInsetType";
            PreTechnoInsetTypesSummaryBindingSource.MoveFirst();

            return PreTechnoInsetTypesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreTechnoInsetColors(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal InsetColorSquare = 0;
            int InsetColorCount = 0;

            PreTechnoInsetColorsSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]) +
                    " AND TechnoInsetColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        InsetColorSquare += Convert.ToDecimal(row["Square"]);
                        InsetColorCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreTechnoInsetColorsSummaryDataTable.NewRow();
                    NewRow["TechnoInsetColor"] = GetInsetColorName(Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    NewRow["TechnoInsetTypeID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]);
                    NewRow["TechnoInsetColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]);
                    if (Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) == 1)
                        InsetColorSquare = 0;
                    NewRow["Square"] = Decimal.Round(InsetColorSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = InsetColorCount;
                    PreTechnoInsetColorsSummaryDataTable.Rows.Add(NewRow);

                    InsetColorSquare = 0;
                    InsetColorCount = 0;
                }
            }
            Table.Dispose();
            PreTechnoInsetColorsSummaryDataTable.DefaultView.Sort = "TechnoInsetColor";
            PreTechnoInsetColorsSummaryBindingSource.MoveFirst();

            return PreTechnoInsetColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreSizes(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable FrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE FrontsOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            decimal SizeSquare = 0;
            int SizeCount = 0;
            PreSizesSummaryDataTable.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "FrontID", "ColorID", "TechnoColorID", "PatinaID", "InsetTypeID", "InsetColorID", "TechnoInsetTypeID", "TechnoInsetColorID", "Height", "Width" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDataTable.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND PatinaID=" + Convert.ToInt32(Table.Rows[i]["PatinaID"]) +
                    " AND InsetTypeID=" + Convert.ToInt32(Table.Rows[i]["InsetTypeID"]) +
                    " AND InsetColorID=" + Convert.ToInt32(Table.Rows[i]["InsetColorID"]) +
                    " AND TechnoColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoColorID"]) +
                    " AND TechnoInsetTypeID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]) +
                    " AND TechnoInsetColorID=" + Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]) +
                    " AND Height=" + Convert.ToInt32(Table.Rows[i]["Height"]) +
                    " AND Width=" + Convert.ToInt32(Table.Rows[i]["Width"]));
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        SizeSquare += Convert.ToDecimal(row["Square"]);
                        SizeCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = PreSizesSummaryDataTable.NewRow();
                    NewRow["Size"] = Convert.ToInt32(Table.Rows[i]["Height"]) + " x " + Convert.ToInt32(Table.Rows[i]["Width"]);
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                    NewRow["PatinaID"] = Convert.ToInt32(Table.Rows[i]["PatinaID"]);
                    NewRow["InsetColorID"] = Convert.ToInt32(Table.Rows[i]["InsetColorID"]);
                    NewRow["InsetTypeID"] = Convert.ToInt32(Table.Rows[i]["InsetTypeID"]);
                    NewRow["TechnoColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoColorID"]);
                    NewRow["TechnoInsetColorID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetColorID"]);
                    NewRow["TechnoInsetTypeID"] = Convert.ToInt32(Table.Rows[i]["TechnoInsetTypeID"]);
                    NewRow["Height"] = Convert.ToInt32(Table.Rows[i]["Height"]);
                    NewRow["Width"] = Convert.ToInt32(Table.Rows[i]["Width"]);
                    NewRow["Square"] = Decimal.Round(SizeSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = SizeCount;
                    PreSizesSummaryDataTable.Rows.Add(NewRow);

                    SizeSquare = 0;
                    SizeCount = 0;
                }
            }
            Table.Dispose();
            PreSizesSummaryDataTable.DefaultView.Sort = "Square DESC";
            PreSizesSummaryBindingSource.MoveFirst();

            return PreSizesSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreDecorProducts(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal DecorProductCount = 0;
            int decimals = 2;
            string Measure = string.Empty;

            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            PreDecorProductsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorProductCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorProductCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = PreDecorProductsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorProduct"] = GetProductName(Convert.ToInt32(Table.Rows[i]["ProductID"]));
                if (DecorProductCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorProductCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                PreDecorProductsSummaryDataTable.Rows.Add(NewRow);

                Measure = "";
                DecorProductCount = 0;
            }
            PreDecorProductsSummaryDataTable.DefaultView.Sort = "Count DESC";
            PreDecorProductsSummaryBindingSource.MoveFirst();

            return PreDecorProductsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreDecorItems(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal DecorItemCount = 0;
            int decimals = 2;
            string Measure = string.Empty;

            string MainProductionStatus = string.Empty;

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            PreDecorItemsSummaryDataTable.Clear();

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" + MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorItemCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorItemCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = PreDecorItemsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorItem"] = GetDecorName(Convert.ToInt32(Table.Rows[i]["DecorID"]));
                if (DecorItemCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorItemCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                PreDecorItemsSummaryDataTable.Rows.Add(NewRow);

                Measure = "";
                DecorItemCount = 0;
            }
            Table.Dispose();
            PreDecorItemsSummaryDataTable.DefaultView.Sort = "Count DESC";
            PreDecorItemsSummaryBindingSource.MoveFirst();

            return PreDecorItemsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreDecorColors(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal DecorColorCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            string MainProductionStatus = string.Empty;

            PreDecorColorsSummaryDataTable.Clear();

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" +
                MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        if (row["Height"].ToString() == "-1")
                            DecorColorCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorColorCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorColorCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorColorCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorColorCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorColorCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = PreDecorColorsSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["Color"] = GetColorName(Convert.ToInt32(Table.Rows[i]["ColorID"]));
                if (DecorColorCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorColorCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                PreDecorColorsSummaryDataTable.Rows.Add(NewRow);

                Measure = string.Empty;
                DecorColorCount = 0;
            }
            Table.Dispose();
            PreDecorColorsSummaryDataTable.DefaultView.Sort = "Count DESC";
            PreDecorColorsSummaryBindingSource.MoveFirst();

            return PreDecorColorsSummaryDataTable.Rows.Count > 0;
        }

        private bool GetPreDecorSizes(
            int[] MegaOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            decimal DecorSizeCount = 0;
            int decimals = 2;
            int Height = 0;
            int Length = 0;
            int Width = 0;
            string Measure = string.Empty;
            string Sizes = string.Empty;

            string MainProductionStatus = string.Empty;

            PreDecorSizesSummaryDataTable.Clear();

            if (NotProduction)
            {
                if (FactoryID == 1)
                    MainProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    MainProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (MainProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        MainProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        MainProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        MainProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (MainProductionStatus.Length > 0)
                MainProductionStatus = " AND (" + MainProductionStatus + ")";

            DataTable Table = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrders.*, MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DecorOrders.FactoryID = " + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" +
                MainProductionStatus + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (DataView DV = new DataView(DecorOrdersDataTable))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "ColorID", "MeasureID", "Length", "Height", "Width" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) +
                    " AND ColorID=" + Convert.ToInt32(Table.Rows[i]["ColorID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]) +
                    " AND Length=" + Convert.ToInt32(Table.Rows[i]["Length"]) +
                    " AND Height=" + Convert.ToInt32(Table.Rows[i]["Height"]) +
                    " AND Width=" + Convert.ToInt32(Table.Rows[i]["Width"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        if (row["Height"].ToString() == "-1")
                            DecorSizeCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorSizeCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorSizeCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorSizeCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorSizeCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorSizeCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = PreDecorSizesSummaryDataTable.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["ColorID"] = Convert.ToInt32(Table.Rows[i]["ColorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                if (DecorSizeCount < 3)
                    decimals = 1;
                NewRow["Count"] = Decimal.Round(DecorSizeCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;

                Height = Convert.ToInt32(Table.Rows[i]["Height"]);
                Length = Convert.ToInt32(Table.Rows[i]["Length"]);
                Width = Convert.ToInt32(Table.Rows[i]["Width"]);

                if (Height > -1)
                    Sizes = Height.ToString();

                if (Sizes != string.Empty)
                {
                    if (Width > -1)
                        Sizes += " x " + Width.ToString();
                }
                else
                {
                    if (Length > -1)
                    {
                        Sizes = Length.ToString();
                        if (Width > -1)
                            Sizes += " x " + Width.ToString();
                    }
                    else
                    {
                        if (Width > -1)
                            Sizes = Width.ToString();
                    }
                }

                PreDecorSizesSummaryDataTable.Rows.Add(NewRow);
                NewRow["Size"] = Sizes;
                Sizes = string.Empty;
                Measure = string.Empty;
                DecorSizeCount = 0;
            }
            Table.Dispose();
            PreDecorSizesSummaryDataTable.DefaultView.Sort = "Count DESC";
            PreDecorSizesSummaryBindingSource.MoveFirst();

            return PreDecorSizesSummaryDataTable.Rows.Count > 0;
        }

        public void GetPreProductInfo(
            int[] MainOrders, int FactoryID,
            bool OnProduction,
            bool NotProduction,
            bool InProduction)
        {
            GetPreFronts(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreFrameColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreTechnoColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreInsetTypes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreInsetColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreTechnoInsetTypes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreTechnoInsetColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreSizes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreDecorProducts(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreDecorItems(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreDecorColors(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
            GetPreDecorSizes(MainOrders, FactoryID, OnProduction, NotProduction, InProduction);
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
                DataRow[] Rows = ColorsDataTable.Select("ColorID = " + ColorID);
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

        /// <summary>
        /// Возвращает название продукта
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        private string GetProductName(int ProductID)
        {
            string ProductName = "";
            try
            {
                DataRow[] Rows = DecorProductsDataTable.Select("ProductID = " + ProductID);
                ProductName = Rows[0]["ProductName"].ToString();
            }
            catch
            {
                return "";
            }
            return ProductName;
        }

        /// <summary>
        /// Возвращает название наименования
        /// </summary>
        /// <param name="DecorID"></param>
        /// <returns></returns>
        private string GetDecorName(int DecorID)
        {
            string DecorName = "";
            try
            {
                DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
                DecorName = Rows[0]["Name"].ToString();
            }
            catch
            {
                return "";
            }
            return DecorName;
        }

        private void GetGridMargins(int FrontID, ref int MarginHeight, ref int MarginWidth)
        {
            DataRow[] Rows = TechStoreDataTable.Select("TechStoreID = " + FrontID);
            if (Rows.Count() > 0)
            {
                if (Rows[0]["InsetHeightAdmission"] != DBNull.Value)
                    MarginHeight = Convert.ToInt32(Rows[0]["InsetHeightAdmission"]);
                if (Rows[0]["InsetWidthAdmission"] != DBNull.Value)
                    MarginWidth = Convert.ToInt32(Rows[0]["InsetWidthAdmission"]);
            }
        }

        public object BatchProfilCloseUser
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return 0;
                return Convert.ToInt32(((DataRowView)BatchBindingSource.Current).Row["ProfilCloseUserID"]);
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["ProfilCloseUserID"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["ProfilCloseUserID"] = value;
            }
        }

        public object BatchTPSCloseUser
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return 0;
                return Convert.ToInt32(((DataRowView)BatchBindingSource.Current).Row["TPSCloseUserID"]);
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["TPSCloseUserID"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["TPSCloseUserID"] = value;
            }
        }

        public object BatchProfilCloseDateTime
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return Convert.ToDateTime(((DataRowView)BatchBindingSource.Current).Row["ProfilCloseDateTime"]);
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["ProfilCloseDateTime"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["ProfilCloseDateTime"] = value;
            }
        }

        public object BatchTPSCloseDateTime
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return Convert.ToDateTime(((DataRowView)BatchBindingSource.Current).Row["TPSCloseDateTime"]);
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["TPSCloseDateTime"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["TPSCloseDateTime"] = value;
            }
        }

        public object BatchProfilConfirmUser
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return 0;
                return Convert.ToInt32(((DataRowView)BatchBindingSource.Current).Row["ProfilConfirmUserID"]);
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["ProfilConfirmUserID"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["ProfilConfirmUserID"] = value;
            }
        }

        public object BatchTPSConfirmUser
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return 0;
                return Convert.ToInt32(((DataRowView)BatchBindingSource.Current).Row["TPSConfirmUserID"]);
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["TPSConfirmUserID"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["TPSConfirmUserID"] = value;
            }
        }

        public bool BatchProfilConfirm
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return Convert.ToBoolean(((DataRowView)BatchBindingSource.Current).Row["ProfilConfirm"]);
            }
            set { ((DataRowView)BatchBindingSource.Current).Row["ProfilConfirm"] = value; }
        }

        public bool BatchTPSConfirm
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return Convert.ToBoolean(((DataRowView)BatchBindingSource.Current).Row["TPSConfirm"]);
            }
            set { ((DataRowView)BatchBindingSource.Current).Row["TPSConfirm"] = value; }
        }

        public object BatchProfilConfirmDateTime
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return ((DataRowView)BatchBindingSource.Current).Row["ProfilConfirmDateTime"];
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["ProfilConfirmDateTime"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["ProfilConfirmDateTime"] = value;
            }
        }

        public object BatchTPSConfirmDateTime
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return ((DataRowView)BatchBindingSource.Current).Row["TPSConfirmDateTime"];
            }
            set
            {
                if (((DataRowView)BatchBindingSource.Current).Row["TPSConfirmDateTime"] == DBNull.Value)
                    ((DataRowView)BatchBindingSource.Current).Row["TPSConfirmDateTime"] = value;
            }
        }

        public bool BatchProfilAccess
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return Convert.ToBoolean(((DataRowView)BatchBindingSource.Current).Row["ProfilBatchClose"]);
            }
            set { ((DataRowView)BatchBindingSource.Current).Row["ProfilBatchClose"] = value; }
        }

        public bool BatchTPSAccess
        {
            get
            {
                if (BatchBindingSource.Count < 1)
                    return false;
                return Convert.ToBoolean(((DataRowView)BatchBindingSource.Current).Row["TPSBatchClose"]);
            }
            set { ((DataRowView)BatchBindingSource.Current).Row["TPSBatchClose"] = value; }
        }

        public bool MegaBatchProfilAccess
        {
            get
            {
                if (MegaBatchBindingSource.Count < 1)
                    return false;
                return Convert.ToBoolean(((DataRowView)MegaBatchBindingSource.Current).Row["ProfilBatchClose"]);
            }
            set { ((DataRowView)MegaBatchBindingSource.Current).Row["ProfilBatchClose"] = value; }
        }

        public bool MegaBatchTPSAccess
        {
            get
            {
                if (MegaBatchBindingSource.Count < 1)
                    return false;
                return Convert.ToBoolean(((DataRowView)MegaBatchBindingSource.Current).Row["TPSBatchClose"]);
            }
            set { ((DataRowView)MegaBatchBindingSource.Current).Row["TPSBatchClose"] = value; }
        }

        public void GetCurrentFrontID()
        {
            if (BatchMainOrdersFrontsOrders.FrontsOrdersBindingSource.Count < 1)
                return;
            CurrentFrontID = Convert.ToInt32(((DataRowView)BatchMainOrdersFrontsOrders.FrontsOrdersBindingSource.Current).Row["FrontID"]);
        }

        public void GetCurrentFrameColorID()
        {
            if (BatchMainOrdersFrontsOrders.FrontsOrdersBindingSource.Count < 1)
                return;
            CurrentFrameColorID = Convert.ToInt32(((DataRowView)BatchMainOrdersFrontsOrders.FrontsOrdersBindingSource.Current).Row["ColorID"]);
            CurrentPatinaID = Convert.ToInt32(((DataRowView)BatchMainOrdersFrontsOrders.FrontsOrdersBindingSource.Current).Row["PatinaID"]);
        }

        #endregion

        #region Set functions

        public void SetMainOrderInProduction(int BatchID, int FactoryID)
        {
            string SelectCommand = "SELECT MainOrderID," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN " +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")";

            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM MainOrders WHERE MainOrderID IN " +
                    " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 2;
                                //DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                //DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                                DT.Rows[i]["ProfilProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["ProfilProductionUserID"] = Security.CurrentUserID;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 2;
                                //DT.Rows[i]["TPSStorageStatusID"] = 1;
                                //DT.Rows[i]["TPSDispatchStatusID"] = 1;
                                DT.Rows[i]["TPSProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["TPSProductionUserID"] = Security.CurrentUserID;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
            SelectCommand = "SELECT MainOrderID," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM NewMainOrders WHERE MainOrderID IN " +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")";

            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM NewMainOrders WHERE MainOrderID IN " +
                    " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 2;
                                //DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                //DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                                DT.Rows[i]["ProfilProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["ProfilProductionUserID"] = Security.CurrentUserID;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 2;
                                //DT.Rows[i]["TPSStorageStatusID"] = 1;
                                //DT.Rows[i]["TPSDispatchStatusID"] = 1;
                                DT.Rows[i]["TPSProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["TPSProductionUserID"] = Security.CurrentUserID;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetMainOrdersNotInProduction(int[] MainOrders, int FactoryID)
        {
            string SelectCommand = "SELECT MainOrderID," +
                " ProfilOnProductionDate, ProfilOnProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSOnProductionDate, TPSOnProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 1;
                                DT.Rows[i]["ProfilOnProductionDate"] = DBNull.Value;
                                DT.Rows[i]["ProfilOnProductionUserID"] = DBNull.Value;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 1;
                                DT.Rows[i]["TPSOnProductionDate"] = DBNull.Value;
                                DT.Rows[i]["TPSOnProductionUserID"] = DBNull.Value;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
            SelectCommand = "SELECT MainOrderID," +
                " ProfilOnProductionDate, ProfilOnProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSOnProductionDate, TPSOnProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 1;
                                DT.Rows[i]["ProfilOnProductionDate"] = DBNull.Value;
                                DT.Rows[i]["ProfilOnProductionUserID"] = DBNull.Value;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 1;
                                DT.Rows[i]["TPSOnProductionDate"] = DBNull.Value;
                                DT.Rows[i]["TPSOnProductionUserID"] = DBNull.Value;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetMegaOrdersNotInProduction(int[] MegaOrders, int FactoryID)
        {
            string SelectCommand = "SELECT MegaOrderID, OrderStatusID, ProfilOrderStatusID, TPSOrderStatusID FROM MegaOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")";
            int ProfilOrderStatusID = 1;
            int TPSOrderStatusID = 1;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < MegaOrders.Count(); i++)
                        {
                            DT.Rows[i]["OrderStatusID"] = 0;
                            if (FactoryID == 1)
                                DT.Rows[i]["ProfilOrderStatusID"] = ProfilOrderStatusID;
                            if (FactoryID == 2)
                                DT.Rows[i]["TPSOrderStatusID"] = TPSOrderStatusID;
                        }
                        DA.Update(DT);
                    }
                }
            }
            SelectCommand = "SELECT MegaOrderID, OrderStatusID, ProfilOrderStatusID, TPSOrderStatusID FROM NewMegaOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")";
            ProfilOrderStatusID = 1;
            TPSOrderStatusID = 1;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < MegaOrders.Count(); i++)
                        {
                            DT.Rows[i]["OrderStatusID"] = 0;
                            if (FactoryID == 1)
                                DT.Rows[i]["ProfilOrderStatusID"] = ProfilOrderStatusID;
                            if (FactoryID == 2)
                                DT.Rows[i]["TPSOrderStatusID"] = TPSOrderStatusID;
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetMainOrderInProduction(int[] MainOrders, int FactoryID)
        {
            string SelectCommand = "SELECT MainOrderID," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 2;
                                //DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                //DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                                DT.Rows[i]["ProfilProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["ProfilProductionUserID"] = Security.CurrentUserID;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 2;
                                //DT.Rows[i]["TPSStorageStatusID"] = 1;
                                //DT.Rows[i]["TPSDispatchStatusID"] = 1;
                                DT.Rows[i]["TPSProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["TPSProductionUserID"] = Security.CurrentUserID;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }

            SelectCommand = "SELECT MainOrderID," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 2;
                                //DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                //DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                                DT.Rows[i]["ProfilProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["ProfilProductionUserID"] = Security.CurrentUserID;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 2;
                                //DT.Rows[i]["TPSStorageStatusID"] = 1;
                                //DT.Rows[i]["TPSDispatchStatusID"] = 1;
                                DT.Rows[i]["TPSProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["TPSProductionUserID"] = Security.CurrentUserID;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }

            SelectCommand = "SELECT MainOrderID," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 2;
                                //DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                //DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                                DT.Rows[i]["ProfilProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["ProfilProductionUserID"] = Security.CurrentUserID;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 2;
                                //DT.Rows[i]["TPSStorageStatusID"] = 1;
                                //DT.Rows[i]["TPSDispatchStatusID"] = 1;
                                DT.Rows[i]["TPSProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["TPSProductionUserID"] = Security.CurrentUserID;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }

            SelectCommand = "SELECT MainOrderID," +
                " ProfilProductionDate, ProfilProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilDispatchStatusID" +
                " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            if (FactoryID == 2)
                SelectCommand = "SELECT MainOrderID," +
                    " TPSProductionDate, TPSProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSDispatchStatusID" +
                    " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilProductionStatusID"] = 2;
                                //DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                //DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                                DT.Rows[i]["ProfilProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["ProfilProductionUserID"] = Security.CurrentUserID;
                            }

                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSProductionStatusID"] = 2;
                                //DT.Rows[i]["TPSStorageStatusID"] = 1;
                                //DT.Rows[i]["TPSDispatchStatusID"] = 1;
                                DT.Rows[i]["TPSProductionDate"] = GetCurrentDate;
                                DT.Rows[i]["TPSProductionUserID"] = Security.CurrentUserID;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetMainOrderOnProduction(int[] MainOrders, int FactoryID, bool IsBatchAdd)
        {
            int ProductionStatusID = 1;

            DataTable BatchDetailsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM BatchDetails" +
                " WHERE FactoryID = " + FactoryID + " AND MainOrderID IN(" + string.Join(",", MainOrders) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDataTable);
            }

            string SelectionCommand = "SELECT MainOrderID, FactoryID," +
                " ProfilOnProductionDate, ProfilOnProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSOnProductionDate, TPSOnProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            DataRow[] rows = BatchDetailsDataTable.Select("MainOrderID = " + MainOrders[i]);

                            if (rows.Count() > 0)
                                ProductionStatusID = 3;
                            else
                                ProductionStatusID = 1;

                            if (FactoryID == 1)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["ProfilOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["ProfilOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["ProfilProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                DT.Rows[i]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                            }

                            if (FactoryID == 2)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["TPSOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["TPSOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["TPSProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["TPSStorageStatusID"] = 1;
                                DT.Rows[i]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[i]["TPSDispatchStatusID"] = 1;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
            SelectionCommand = "SELECT MainOrderID, FactoryID," +
                " ProfilOnProductionDate, ProfilOnProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSOnProductionDate, TPSOnProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            DataRow[] rows = BatchDetailsDataTable.Select("MainOrderID = " + MainOrders[i]);

                            if (rows.Count() > 0)
                                ProductionStatusID = 3;
                            else
                                ProductionStatusID = 1;

                            if (FactoryID == 1)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["ProfilOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["ProfilOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["ProfilProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                DT.Rows[i]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                            }

                            if (FactoryID == 2)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["TPSOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["TPSOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["TPSProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["TPSStorageStatusID"] = 1;
                                DT.Rows[i]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[i]["TPSDispatchStatusID"] = 1;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
            SelectionCommand = "SELECT MainOrderID, FactoryID," +
                " ProfilOnProductionDate, ProfilOnProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSOnProductionDate, TPSOnProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            DataRow[] rows = BatchDetailsDataTable.Select("MainOrderID = " + MainOrders[i]);

                            if (rows.Count() > 0)
                                ProductionStatusID = 3;
                            else
                                ProductionStatusID = 1;

                            if (FactoryID == 1)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["ProfilOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["ProfilOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["ProfilProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                DT.Rows[i]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                            }

                            if (FactoryID == 2)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["TPSOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["TPSOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["TPSProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["TPSStorageStatusID"] = 1;
                                DT.Rows[i]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[i]["TPSDispatchStatusID"] = 1;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
            SelectionCommand = "SELECT MainOrderID, FactoryID," +
                " ProfilOnProductionDate, ProfilOnProductionUserID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSOnProductionDate, TPSOnProductionUserID, TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM NewMainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < MainOrders.Count(); i++)
                        {
                            DataRow[] rows = BatchDetailsDataTable.Select("MainOrderID = " + MainOrders[i]);

                            if (rows.Count() > 0)
                                ProductionStatusID = 3;
                            else
                                ProductionStatusID = 1;

                            if (FactoryID == 1)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["ProfilOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["ProfilOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["ProfilOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["ProfilProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["ProfilStorageStatusID"] = 1;
                                DT.Rows[i]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[i]["ProfilDispatchStatusID"] = 1;
                            }

                            if (FactoryID == 2)
                            {
                                if (IsBatchAdd)
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = Security.GetCurrentDate();
                                    DT.Rows[i]["TPSOnProductionUserID"] = Security.CurrentUserID;
                                }
                                else
                                {
                                    DT.Rows[i]["TPSOnProductionDate"] = DBNull.Value;
                                    DT.Rows[i]["TPSOnProductionUserID"] = DBNull.Value;
                                }
                                DT.Rows[i]["TPSProductionStatusID"] = ProductionStatusID;
                                DT.Rows[i]["TPSStorageStatusID"] = 1;
                                DT.Rows[i]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[i]["TPSDispatchStatusID"] = 1;
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        /// <summary>
        /// Выставляет статус заказа:
        /// "Не в производстве", если в данном заказе ни один подзаказ не добавлен в партию
        /// "На производстве", если хотя бы один подзаказ добавлен в партию
        /// </summary>
        /// <param name="MegaOrderID"></param>
        public void SetMegaOrderOnProduction(int MegaOrderID, int FactoryID)
        {
            string SelectCommand = "SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND MainOrderID IN" +
                " (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")";

            int ProfilOrderStatusID = 1;
            int TPSOrderStatusID = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        ProfilOrderStatusID = 5;
                        TPSOrderStatusID = 5;
                    }
                }
            }

            SelectCommand = "SELECT MainOrderID FROM BatchDetails WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")";
            int OrderStatusID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    //если хотя бы один подзаказ добавлен в партию, то статус заказа "На производстве"
                    //если нет, остается статус "Не в производстве"
                    if (DT.Rows.Count > 0)
                    {
                        OrderStatusID = 4;
                    }
                }
            }

            SelectCommand = "SELECT MegaOrderID, OrderStatusID, ProfilOrderStatusID, TPSOrderStatusID FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0 && DT.Rows[0]["OrderStatusID"] != DBNull.Value)
                        {
                            if (Convert.ToInt32(DT.Rows[0]["OrderStatusID"]) != OrderStatusID)
                            {
                                DT.Rows[0]["OrderStatusID"] = OrderStatusID;
                            }

                            if (FactoryID == 1)
                                if (Convert.ToInt32(DT.Rows[0]["ProfilOrderStatusID"]) != ProfilOrderStatusID)
                                {
                                    DT.Rows[0]["ProfilOrderStatusID"] = ProfilOrderStatusID;
                                }

                            if (FactoryID == 2)
                                if (Convert.ToInt32(DT.Rows[0]["TPSOrderStatusID"]) != TPSOrderStatusID)
                                {
                                    DT.Rows[0]["TPSOrderStatusID"] = TPSOrderStatusID;
                                }

                            DA.Update(DT);
                        }
                    }
                }
            }
            SelectCommand = "SELECT MegaOrderID, OrderStatusID, ProfilOrderStatusID, TPSOrderStatusID FROM NewMegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0 && DT.Rows[0]["OrderStatusID"] != DBNull.Value)
                        {
                            if (Convert.ToInt32(DT.Rows[0]["OrderStatusID"]) != OrderStatusID)
                            {
                                DT.Rows[0]["OrderStatusID"] = OrderStatusID;
                            }

                            if (FactoryID == 1)
                                if (Convert.ToInt32(DT.Rows[0]["ProfilOrderStatusID"]) != ProfilOrderStatusID)
                                {
                                    DT.Rows[0]["ProfilOrderStatusID"] = ProfilOrderStatusID;
                                }

                            if (FactoryID == 2)
                                if (Convert.ToInt32(DT.Rows[0]["TPSOrderStatusID"]) != TPSOrderStatusID)
                                {
                                    DT.Rows[0]["TPSOrderStatusID"] = TPSOrderStatusID;
                                }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SetBatchEnabled(int MegaBatchID, int FactoryID, bool Enabled)
        {
            for (int i = 0; i < BatchDataTable.Rows.Count; i++)
            {
                if (FactoryID == 1)
                {
                    if (Enabled)
                    {
                        BatchDataTable.Rows[i]["ProfilConfirm"] = true;
                        if (BatchDataTable.Rows[i]["ProfilConfirmUserID"] == DBNull.Value)
                            BatchDataTable.Rows[i]["ProfilConfirmUserID"] = Security.CurrentUserID;
                        if (BatchDataTable.Rows[i]["ProfilConfirmDateTime"] == DBNull.Value)
                            BatchDataTable.Rows[i]["ProfilConfirmDateTime"] = Security.GetCurrentDate();

                        BatchDataTable.Rows[i]["ProfilBatchClose"] = Enabled;
                        if (BatchDataTable.Rows[i]["ProfilCloseUserID"] == DBNull.Value)
                            BatchDataTable.Rows[i]["ProfilCloseUserID"] = Security.CurrentUserID;
                        if (BatchDataTable.Rows[i]["ProfilCloseDateTime"] == DBNull.Value)
                            BatchDataTable.Rows[i]["ProfilCloseDateTime"] = Security.GetCurrentDate();
                    }
                    else
                    {
                        BatchDataTable.Rows[i]["ProfilBatchClose"] = Enabled;
                        //BatchDataTable.Rows[i]["ProfilCloseUserID"] = DBNull.Value;
                        //BatchDataTable.Rows[i]["ProfilCloseDateTime"] = DBNull.Value;
                    }
                }
                if (FactoryID == 2)
                {
                    if (Enabled)
                    {
                        BatchDataTable.Rows[i]["TPSConfirm"] = true;
                        if (BatchDataTable.Rows[i]["TPSConfirmUserID"] == DBNull.Value)
                            BatchDataTable.Rows[i]["TPSConfirmUserID"] = Security.CurrentUserID;
                        if (BatchDataTable.Rows[i]["TPSConfirmDateTime"] == DBNull.Value)
                            BatchDataTable.Rows[i]["TPSConfirmDateTime"] = Security.GetCurrentDate();

                        BatchDataTable.Rows[i]["TPSBatchClose"] = Enabled;
                        if (BatchDataTable.Rows[i]["TPSCloseUserID"] == DBNull.Value)
                            BatchDataTable.Rows[i]["TPSCloseUserID"] = Security.CurrentUserID;
                        if (BatchDataTable.Rows[i]["TPSCloseDateTime"] == DBNull.Value)
                            BatchDataTable.Rows[i]["TPSCloseDateTime"] = Security.GetCurrentDate();
                    }
                    else
                    {
                        BatchDataTable.Rows[i]["TPSBatchClose"] = Enabled;
                        //BatchDataTable.Rows[i]["TPSCloseUserID"] = DBNull.Value;
                        //BatchDataTable.Rows[i]["TPSCloseDateTime"] = DBNull.Value;
                    }
                }
            }

            try
            {
                string BatchFactoryFilter = string.Empty;

                if (FactoryID != 0)
                    BatchFactoryFilter = " AND (BatchID IN (SELECT BatchID FROM BatchDetails WHERE BatchDetails.FactoryID = " + FactoryID + "))";

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE MegaBatchID = " + MegaBatchID + BatchFactoryFilter + " ORDER BY BatchID DESC",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            for (int i = 0; i < BatchDataTable.Rows.Count; i++)
                            {
                                int BatchID = Convert.ToInt32(BatchDataTable.Rows[i]["BatchID"]);
                                if (FactoryID == 1)
                                {
                                    DataRow[] rows = DT.Select("BatchID=" + BatchID);
                                    if (rows.Count() == 0)
                                        continue;
                                    if (!Enabled)
                                    {
                                        rows[0]["ProfilConfirm"] = true;
                                        if (rows[0]["ProfilConfirmUserID"] == DBNull.Value)
                                            rows[0]["ProfilConfirmUserID"] = Security.CurrentUserID;
                                        if (rows[0]["ProfilConfirmDateTime"] == DBNull.Value)
                                            rows[0]["ProfilConfirmDateTime"] = Security.GetCurrentDate();

                                        rows[0]["ProfilBatchClose"] = Enabled;
                                        if (rows[0]["ProfilCloseUserID"] == DBNull.Value)
                                            rows[0]["ProfilCloseUserID"] = Security.CurrentUserID;
                                        if (rows[0]["ProfilCloseDateTime"] == DBNull.Value)
                                            rows[0]["ProfilCloseDateTime"] = Security.GetCurrentDate();
                                    }
                                    else
                                    {
                                        rows[0]["ProfilBatchClose"] = Enabled;
                                        //rows[0]["ProfilCloseUserID"] = DBNull.Value;
                                        //rows[0]["ProfilCloseDateTime"] = DBNull.Value;
                                    }
                                }
                                if (FactoryID == 2)
                                {
                                    DataRow[] rows = DT.Select("BatchID=" + BatchID);
                                    if (rows.Count() == 0)
                                        continue;
                                    if (!Enabled)
                                    {
                                        rows[0]["TPSConfirm"] = true;
                                        if (rows[0]["TPSConfirmUserID"] == DBNull.Value)
                                            rows[0]["TPSConfirmUserID"] = Security.CurrentUserID;
                                        if (rows[0]["TPSConfirmDateTime"] == DBNull.Value)
                                            rows[0]["TPSConfirmDateTime"] = Security.GetCurrentDate();

                                        rows[0]["TPSBatchClose"] = Enabled;
                                        if (rows[0]["TPSCloseUserID"] == DBNull.Value)
                                            rows[0]["TPSCloseUserID"] = Security.CurrentUserID;
                                        if (rows[0]["TPSCloseDateTime"] == DBNull.Value)
                                            rows[0]["TPSCloseDateTime"] = Security.GetCurrentDate();
                                    }
                                    else
                                    {
                                        rows[0]["TPSBatchClose"] = Enabled;
                                        //rows[0]["TPSCloseUserID"] = DBNull.Value;
                                        //rows[0]["TPSCloseDateTime"] = DBNull.Value;
                                    }
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SetMegaBatchEnable();
        }

        public void SetMegaBatchEnable()
        {
            int MegaBatchID = 0;
            int ProfilBatchClose = 0;
            int TPSBatchClose = 0;

            string BatchFactoryFilter = @"SELECT Batch.*, BatchDetails.FactoryID FROM Batch
                INNER JOIN BatchDetails ON Batch.BatchID = BatchDetails.BatchID";

            using (SqlDataAdapter DA = new SqlDataAdapter(BatchFactoryFilter, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < MegaBatchDataTable.Rows.Count; i++)
                    {
                        MegaBatchID = Convert.ToInt32(MegaBatchDataTable.Rows[i]["MegaBatchID"]);
                        ProfilBatchClose = 0;
                        TPSBatchClose = 0;
                        DataRow[] Rows = DT.Select("MegaBatchID = " + MegaBatchID + " AND FactoryID=1");

                        foreach (DataRow item in Rows)
                        {
                            if (Convert.ToInt32(item["ProfilBatchClose"]) == 1)
                            {
                                ProfilBatchClose = 1;
                                break;
                            }
                        }
                        Rows = DT.Select("MegaBatchID = " + MegaBatchID + " AND FactoryID=2");
                        foreach (DataRow item in Rows)
                        {
                            if (Convert.ToInt32(item["TPSBatchClose"]) == 1)
                            {
                                TPSBatchClose = 1;
                                break;
                            }
                        }
                        MegaBatchDataTable.Rows[i]["ProfilBatchClose"] = ProfilBatchClose;
                        MegaBatchDataTable.Rows[i]["TPSBatchClose"] = TPSBatchClose;
                    }
                }
            }
        }

        public void SetMegaBatchAgreement(int MegaBatchID, int FactoryID)
        {
            string BatchFactoryFilter = @"SELECT * FROM MegaBatch WHERE MegaBatchID=" + MegaBatchID;

            using (
                SqlDataAdapter DA = new SqlDataAdapter(BatchFactoryFilter,
                    ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[0]["ProfilAgreedUserID"] = Security.CurrentUserID;
                                DT.Rows[0]["ProfilAgreedDateTime"] = Security.GetCurrentDate();
                            }
                            if (FactoryID == 2)
                            {
                                DT.Rows[0]["TPSAgreedUserID"] = Security.CurrentUserID;
                                DT.Rows[0]["TPSAgreedDateTime"] = Security.GetCurrentDate();
                            }
                            DA.Update(DT);
                        }
                    }

                }
            }
            BatchFactoryFilter = @"SELECT * FROM Batch WHERE MegaBatchID=" + MegaBatchID;

            using (
                SqlDataAdapter DA = new SqlDataAdapter(BatchFactoryFilter,
                    ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (FactoryID == 1)
                            {
                                DT.Rows[i]["ProfilConfirm"] = true;
                                DT.Rows[i]["ProfilConfirmUserID"] = Security.CurrentUserID;
                                DT.Rows[i]["ProfilConfirmDateTime"] = Security.GetCurrentDate();
                            }
                            if (FactoryID == 2)
                            {
                                DT.Rows[i]["TPSConfirm"] = true;
                                DT.Rows[i]["TPSConfirmUserID"] = Security.CurrentUserID;
                                DT.Rows[i]["TPSConfirmDateTime"] = Security.GetCurrentDate();
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        #endregion

        public string GetMegaBatchAgreement(int MegaBatchID)
        {
            string s = string.Empty;
            DataRow[] rows = MegaBatchDataTable.Select("MegaBatchID = " + MegaBatchID);
            if (rows.Count() > 0)
            {
                if (rows[0]["ProfilAgreedUserID"] != DBNull.Value)
                    s = "Утверждение (Профиль): " + UserName(Convert.ToInt32(rows[0]["ProfilAgreedUserID"])) + " " + Convert.ToDateTime(rows[0]["ProfilAgreedDateTime"]).ToString("dd.MM.yyyy");
                if (rows[0]["TPSAgreedUserID"] != DBNull.Value)
                    if (s.Length == 0)
                        s = "Утверждение (ТПС): " + UserName(Convert.ToInt32(rows[0]["TPSAgreedUserID"])) + " " + Convert.ToDateTime(rows[0]["TPSAgreedDateTime"]).ToString("dd.MM.yyyy");
                    else
                        s += "\r\nУтверждение (ТПС): " + UserName(Convert.ToInt32(rows[0]["TPSAgreedUserID"])) + " " + Convert.ToDateTime(rows[0]["TPSAgreedDateTime"]).ToString("dd.MM.yyyy");
            }
            if (s.Length == 0)
                s = "Нет утверждений";
            return s;
        }

        public string UserName(int UserID)
        {
            DataRow[] rows = UsersDT.Select("UserID = " + UserID);
            if (rows.Count() > 0)
                return rows[0]["ShortName"].ToString();
            return string.Empty;
        }


        #region Check functions

        private bool CanBeRemove(int[] MainOrders, int FactoryID)
        {
            string SelectionCommand = "SELECT ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM MainOrders WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (FactoryID == 1)
                        {
                            if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) == 2
                                || Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) == 2
                                || Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) == 2
                                || Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) == 2)
                                return false;
                        }

                        if (FactoryID == 2)
                        {
                            if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) == 2
                                || Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) == 2
                                || Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) == 2
                                || Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) == 2)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// возвращает true, если уже создана пустая партия
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public bool IsBatchEmpty()
        {
            int BatchID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM Batch ORDER BY BatchID DESC",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        BatchID = Convert.ToInt32(DT.Rows[0]["BatchID"]);

                        using (SqlDataAdapter DA1 = new SqlDataAdapter("SELECT BatchID FROM BatchDetails" +
                            " WHERE BatchID = " + BatchID,
                            ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            using (DataTable DT1 = new DataTable())
                            {
                                DA1.Fill(DT1);
                                return DT1.Rows.Count < 1;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool IsBatchEnabled(int BatchID, int FactoryID)
        {
            bool BatchClose = false;
            //проверяем, можно ли добавлять заказы в эту партию
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE BatchID = " + BatchID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (FactoryID == 1)
                            BatchClose = Convert.ToBoolean(DT.Rows[0]["ProfilBatchClose"]);
                        if (FactoryID == 2)
                            BatchClose = Convert.ToBoolean(DT.Rows[0]["TPSBatchClose"]);
                        if (BatchClose)
                        {
                            return false;
                        }
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// возвращает true, если подзаказ уже находится в партии
        /// </summary>
        /// <param name="BatchID"></param>
        /// <param name="MainOrderID"></param>
        /// <returns></returns>
        private bool IsAlreadyInBatch(int MainOrderID, int FactoryID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT BatchID FROM BatchDetails" +
                " WHERE MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DT.Rows.Count > 0;
                }
            }
        }

        private string GetBatchNumber(int MainOrderID)
        {
            string MegaBatch = string.Empty;
            string Batch = string.Empty;
            string BatchNumber = string.Empty;

            DataRow[] Rows = BatchDetailsDataTable.Select("MainOrderID = " + MainOrderID);

            if (Rows.Count() > 0)
            {
                MegaBatch = Rows[0]["MegaBatchID"].ToString();
                Batch = Rows[0]["BatchID"].ToString();
                BatchNumber = MegaBatch + ", " + Batch;
            }

            return BatchNumber;
        }

        public bool HasOrders(int[] MegaOrders, int FactoryID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM FrontsOrders  WHERE FrontConfigID IN " +
                "(SELECT FrontConfigID FROM infiniu2_catalog.dbo.FrontsConfig WHERE FactoryID=" +
                FactoryID + ") AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ") AND MainOrderID IN" +
                " (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DA.Fill(DT) > 0)
                        return true;
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE DecorConfigID IN " +
                "(SELECT DecorConfigID FROM infiniu2_catalog.dbo.DecorConfig WHERE FactoryID=" +
                FactoryID + ") AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + ") AND MainOrderID IN" +
                " (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + "))",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return true;
                }
            }

            return false;
        }
        #endregion

        private int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void FillWeekNumber()
        {
            for (int i = 0; i < MegaBatchDataTable.Rows.Count; i++)
            {
                MegaBatchDataTable.Rows[i]["WeekNumber"] = GetWeekNumber(Convert.ToDateTime(MegaBatchDataTable.Rows[i]["CreateDateTime"])) + " к.н.";
            }
        }

        public ArrayList PickUpFrontsProfil(int FactoryID, int BatchID,
            bool BatchOnProduction,
            bool BatchNotProduction,
            bool BatchInProduction,
            bool BatchOnStorage,
            bool BatchOnExp,
            bool BatchDispatched,
            ArrayList Fronts)
        {
            ArrayList array = new ArrayList();

            string MainOrdersFilter = " AND MainOrderID IN" +
                " (SELECT MainOrderID FROM FrontsOrders WHERE FrontID IN (" + string.Join(",", Fronts.OfType<int>().ToArray()) + "))";

            string FactoryFilter = string.Empty;
            string OrdersProductionStatus = string.Empty;
            string ProductionStatus = string.Empty;

            if (BatchNotProduction)
            {
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (BatchInProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (BatchOnProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (BatchOnStorage)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (BatchDispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (BatchOnExp)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (OrdersProductionStatus.Length > 0)
            {
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
            }

            if (FactoryID > 0)
            {
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            if (FactoryID == -1)
            {
                FactoryFilter = " AND (FactoryID = -1)";
            }

            CurrentMegaOrderID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + CurrentBatchID + ")" +
                FactoryFilter + OrdersProductionStatus + MainOrdersFilter + ")" +
                " ORDER BY ClientName, OrderNumber",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                BatchMegaOrdersDataTable.Clear();
                DA.Fill(BatchMegaOrdersDataTable);

                for (int i = 0; i < BatchMegaOrdersDataTable.Rows.Count; i++)
                {
                    array.Add(Convert.ToInt32(BatchMegaOrdersDataTable.Rows[i]["MegaOrderID"]));
                }
            }

            return array;
        }

        public ArrayList PickUpFrontsTPS(int FactoryID, int BatchID,
            bool BatchOnProduction,
            bool BatchNotProduction,
            bool BatchInProduction,
            bool BatchOnStorage,
            bool BatchOnExp,
            bool BatchDispatched)
        {
            ArrayList array = new ArrayList();

            if (BatchMainOrdersFrontsOrders.FrontsOrdersDataTable.Rows.Count < 1)
                return array;

            string MainOrdersFilter = " AND MainOrderID IN" +
                " (SELECT MainOrderID FROM FrontsOrders WHERE FrontID = " + CurrentFrontID + " AND ColorID = " + CurrentFrameColorID + " AND PatinaID = " + CurrentPatinaID + ")";

            string FactoryFilter = string.Empty;
            string OrdersProductionStatus = string.Empty;
            string ProductionStatus = string.Empty;

            if (BatchNotProduction)
            {
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (BatchInProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (BatchOnProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (BatchOnStorage)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                }
            }

            if (BatchOnExp)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                }
            }

            if (BatchDispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                }
            }

            if (OrdersProductionStatus.Length > 0)
            {
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
            }

            if (FactoryID > 0)
            {
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            if (FactoryID == -1)
            {
                FactoryFilter = " AND (FactoryID = -1)";
            }

            CurrentMegaOrderID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients" +
                " ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders WHERE MainOrderID IN" +
                " (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID + " AND BatchID = " + CurrentBatchID + ")" +
                FactoryFilter + OrdersProductionStatus + MainOrdersFilter + ")" +
                " ORDER BY ClientName, OrderNumber",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                BatchMegaOrdersDataTable.Clear();
                DA.Fill(BatchMegaOrdersDataTable);

                for (int i = 0; i < BatchMegaOrdersDataTable.Rows.Count; i++)
                {
                    array.Add(Convert.ToInt32(BatchMegaOrdersDataTable.Rows[i]["MegaOrderID"]));
                }
            }

            return array;
        }
    }




    public class BatchReport : IAllFrontParameterName, IIsMarsel
    {
        object ResponsibleDateTime = DBNull.Value;
        object TechnologyDateTime = DBNull.Value;
        object ConfirmDateTime = DBNull.Value;
        object CloseDateTime = DBNull.Value;
        object PrintDateTime = DBNull.Value;
        object ResponsibleUserID = DBNull.Value;
        object TechnologyUserID = DBNull.Value;
        object ConfirmUserID = DBNull.Value;
        object CloseUserID = DBNull.Value;
        object PrintUserID = DBNull.Value;

        FileManager FM = new FileManager();
        DataTable ClientsDataTable = null;

        DataTable FrontsResultDataTable = null;
        DataTable[] DecorResultDataTable = null;

        public DataTable[] ClientReportTables = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;
        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable TechnoInsetTypesDataTable = null;
        private DataTable TechnoInsetColorsDataTable = null;

        Infinium.Modules.Marketing.NewOrders.DecorCatalogOrder DecorCatalog = null;

        public BatchReport(ref Infinium.Modules.Marketing.NewOrders.DecorCatalogOrder tDecorCatalog)
        {
            DecorCatalog = tDecorCatalog;

            Create();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.MarketingReferenceConnectionString))
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
            TechnoInsetTypesDataTable = InsetTypesDataTable.Copy();
            TechnoInsetColorsDataTable = InsetColorsDataTable.Copy();

            CreateFrontsDataTable();
            CreateDecorDataTable();
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
            FrontsOrdersDataTable = new DataTable();
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            TechnoInsetTypesDataTable = new DataTable();
            TechnoInsetColorsDataTable = new DataTable();
            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable[DecorCatalog.DecorProductsCount];
            DecorOrdersDataTable = new DataTable();

            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
                DecorOrdersDataTable = new DataTable();
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Patina"), System.Type.GetType("System.String")));
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
            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
            {
                DecorResultDataTable[i] = new DataTable();

                DecorResultDataTable[i].Columns.Add("Product", Type.GetType("System.String"));
                DecorResultDataTable[i].Columns.Add("Color", Type.GetType("System.String"));
                DecorResultDataTable[i].Columns.Add("Height", Type.GetType("System.Int32"));
                DecorResultDataTable[i].Columns.Add("Width", Type.GetType("System.Int32"));
                DecorResultDataTable[i].Columns.Add("Count", Type.GetType("System.Int32"));
                DecorResultDataTable[i].Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            }
        }

        private string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        public string GetClientName(int MainOrderID)
        {
            string ClientName = "";

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientName FROM Clients WHERE ClientID = " +
                    " (SELECT ClientID FROM infiniu2_marketingorders.dbo.MegaOrders" +
                    " WHERE MegaOrderID=(SELECT TOP 1 MegaOrderID FROM infiniu2_marketingorders.dbo.MainOrders WHERE MainOrderID = " + MainOrderID + "))",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientName = DT.Rows[0]["ClientName"].ToString();
                }
            }

            return ClientName;
        }

        public int GetOrderNumber(int MainOrderID)
        {
            int OrderNumber = 0;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OrderNumber FROM MegaOrders" +
                    " WHERE MegaOrderID=(SELECT TOP 1 MegaOrderID FROM MainOrders WHERE MainOrderID = " + MainOrderID + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        OrderNumber = Convert.ToInt32(DT.Rows[0]["OrderNumber"]);
                }
            }

            return OrderNumber;
        }

        private string GetClientID(int MainOrderID)
        {
            string ClientID = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE MegaOrderID=" +
                    "(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientID = DT.Rows[0]["ClientID"].ToString();
                }
            }
            return ClientID;
        }

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

        public void GetBatchInfoByMainOrder(int MainOrderID, int FactoryID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE BatchID = " +
                    " (SELECT BatchID FROM BatchDetails WHERE MainOrderID=" + MainOrderID + " AND FactoryID=" + FactoryID + ")", ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        if (FactoryID == 1)
                        {
                            ConfirmDateTime = DT.Rows[0]["ProfilConfirmDateTime"];
                            ConfirmUserID = DT.Rows[0]["ProfilConfirmUserID"];
                            CloseDateTime = DT.Rows[0]["ProfilCloseDateTime"];
                            CloseUserID = DT.Rows[0]["ProfilCloseUserID"];
                        }
                        if (FactoryID == 2)
                        {
                            ConfirmDateTime = DT.Rows[0]["TPSConfirmDateTime"];
                            ConfirmUserID = DT.Rows[0]["TPSConfirmUserID"];
                            CloseDateTime = DT.Rows[0]["TPSCloseDateTime"];
                            CloseUserID = DT.Rows[0]["TPSCloseUserID"];
                        }
                    }
                }
            }
        }

        public void GetBatchInfoByBatch(int BatchID, int FactoryID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Batch WHERE BatchID=" + BatchID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        if (FactoryID == 1)
                        {
                            ConfirmDateTime = DT.Rows[0]["ProfilConfirmDateTime"];
                            ConfirmUserID = DT.Rows[0]["ProfilConfirmUserID"];
                            CloseDateTime = DT.Rows[0]["ProfilCloseDateTime"];
                            CloseUserID = DT.Rows[0]["ProfilCloseUserID"];
                        }
                        if (FactoryID == 2)
                        {
                            ConfirmDateTime = DT.Rows[0]["TPSConfirmDateTime"];
                            ConfirmUserID = DT.Rows[0]["TPSConfirmUserID"];
                            CloseDateTime = DT.Rows[0]["TPSCloseDateTime"];
                            CloseUserID = DT.Rows[0]["TPSCloseUserID"];
                        }
                    }
                }
            }
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ShortName FROM Users " +
                    " WHERE UserID=" + UserID, ConnectionStrings.UsersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Name = DT.Rows[0]["ShortName"].ToString();
                }
            }

            return Name;
        }

        //public string GetUserName(int UserID)
        //{
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ShortName FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
        //    {
        //        using (DataTable DT = new DataTable())
        //        {
        //            DA.Fill(DT);
        //            return DT.Rows[0]["ShortName"].ToString();
        //        }
        //    }
        //}

        private ArrayList GetMainOrders(int BatchID, int FactoryID)
        {
            ArrayList array = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM BatchDetails" +
                " WHERE FactoryID = " + FactoryID + " AND BatchID = " + BatchID + " ORDER BY MainOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        array.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                    }
                }
            }
            return array;
        }

        private void FillFronts()
        {
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["TechnoColorID"]) != -1)
                    FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"])) + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
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
                NewRow["Front"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrameColor"] = FrameColor;
                NewRow["Patina"] = GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                NewRow["InsetType"] = InsetType;
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Square"] = Row["Square"];
                NewRow["Notes"] = Row["Notes"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
        }

        private void FillDecor()
        {
            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
            {
                DecorResultDataTable[i].Clear();
                DecorResultDataTable[i].AcceptChanges();

                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow Row in Rows)
                {
                    DataRow NewRow2 = DecorResultDataTable[i].NewRow();

                    NewRow2["Product"] = DecorCatalog.DecorProductsDataTable.Rows[i]["ProductName"].ToString() + " " +
                                     DecorCatalog.GetItemName(Convert.ToInt32(Row["DecorID"]));

                    if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "Height"))
                    {
                        if (Convert.ToInt32(Row["Height"]) != -1)
                            NewRow2["Height"] = Row["Height"];
                        if (Convert.ToInt32(Row["Length"]) != -1)
                            NewRow2["Height"] = Row["Length"];
                    }
                    if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "Length"))
                    {
                        if (Convert.ToInt32(Row["Length"]) != -1)
                            NewRow2["Height"] = Row["Length"];
                        if (Convert.ToInt32(Row["Height"]) != -1)
                            NewRow2["Height"] = Row["Height"];
                    }
                    if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "Width") && Convert.ToInt32(Row["Width"]) != -1)
                        NewRow2["Width"] = Row["Width"];


                    string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                    if (Convert.ToInt32(Row["PatinaID"]) != -1)
                        Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                    //if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "ColorID"))
                        NewRow2["Color"] = Color;

                    NewRow2["Count"] = Row["Count"];
                    NewRow2["Notes"] = Row["Notes"];
                    DecorResultDataTable[i].Rows.Add(NewRow2);
                }
            }
        }

        private void FillFrontsByBatch()
        {
            FrontsResultDataTable.Clear();

            string Front = string.Empty;
            string FrameColor = string.Empty;
            string Patina = string.Empty;
            string TechnoInset = string.Empty;
            string InsetColor = string.Empty;
            string Notes = string.Empty;

            int Height = 0;
            int Width = 0;
            int Count = 0;
            decimal Square = 0;

            DataTable DT1 = FrontsOrdersDataTable.Clone();
            DataTable DT2 = FrontsOrdersDataTable.Clone();
            DataTable DT3 = FrontsResultDataTable.Clone();
            DataTable DT = new DataTable();

            //сначала витрины
            string filter = "InsetTypeID IN (1)";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            //решетки
            filter = "InsetTypeID IN (685,686,687,688,29470,29471)";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            //глухие
            DataRow[] rows = InsetTypesDataTable.Select("GroupID = 3 OR GroupID = 4");
            filter = string.Empty;
            foreach (DataRow item in rows)
                filter += item["InsetTypeID"].ToString() + ",";
            if (filter.Length > 0)
                filter = "(InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            //все остальные
            rows = InsetTypesDataTable.Select("GroupID <> 3 AND GroupID <> 4 AND InsetTypeID NOT IN (1,685,686,687,688,29470,29471)");
            filter = string.Empty;
            foreach (DataRow item in rows)
                filter += item["InsetTypeID"].ToString() + ",";
            if (filter.Length > 0)
                filter = "(InsetTypeID IN (" + filter.Substring(0, filter.Length - 1) + "))";
            using (DataView DV = new DataView(FrontsOrdersDataTable, filter, "FrontID, ColorID, PatinaID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "FrontID", "ColorID", "PatinaID" });
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "FrontID = " + Convert.ToInt32(DT.Rows[i]["FrontID"]) + " AND PatinaID = " + Convert.ToInt32(DT.Rows[i]["PatinaID"]) +
                        " AND ColorID = " + Convert.ToInt32(DT.Rows[i]["ColorID"]);
                    DV.Sort = "FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width";
                    DT1 = DV.ToTable();
                    foreach (DataRow Row in DT1.Rows)
                        DT2.ImportRow(Row);
                    DT1.Clear();
                }
            }
            foreach (DataRow Row in DT2.Rows)
            {
                Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                Patina = GetPatinaName(Convert.ToInt32(Row["PatinaID"]));

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
                TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));

                Height = Convert.ToInt32(Row["Height"]);
                Width = Convert.ToInt32(Row["Width"]);
                Count = Convert.ToInt32(Row["Count"]);
                Square = Convert.ToDecimal(Row["Square"]);

                if (Row["Notes"] == DBNull.Value)
                {
                    DataRow[] fRow = FrontsResultDataTable.Select("Front = '" + Front + "' AND Patina = '" + Patina +
                        "' AND InsetType = '" + InsetType + "' AND FrameColor = '" + FrameColor + "' AND InsetColor = '" + InsetColor +
                        "' AND TechnoInset='" + TechnoInset + "' AND Height = '" + Height.ToString() + "' AND Width = '" + Width.ToString() +
                        "' AND LEN(Notes) = 0");
                    if (fRow.Count() == 0)
                    {
                        DataRow NewRow = FrontsResultDataTable.NewRow();

                        NewRow["Front"] = Front;
                        NewRow["FrameColor"] = FrameColor;
                        NewRow["Patina"] = Patina;
                        NewRow["InsetType"] = InsetType;
                        NewRow["InsetColor"] = InsetColor;
                        NewRow["TechnoInset"] = TechnoInset;
                        NewRow["Height"] = Height;
                        NewRow["Width"] = Width;
                        NewRow["Square"] = Row["Square"];
                        NewRow["Count"] = Count;
                        NewRow["Notes"] = string.Empty;

                        FrontsResultDataTable.Rows.Add(NewRow);
                    }
                    else
                    {
                        fRow[0]["Square"] = Convert.ToDecimal(fRow[0]["Square"]) + Square;
                        fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Count;
                    }
                }
                else
                {
                    Notes = Row["Notes"].ToString();

                    DataRow[] fRow = FrontsResultDataTable.Select("Front = '" + Front + "' AND Patina = '" + Patina +
                        "' AND InsetType = '" + InsetType + "' AND FrameColor = '" + FrameColor + "' AND InsetColor = '" + InsetColor +
                        "' AND TechnoInset = '" + TechnoInset + "' AND Height = '" + Height.ToString() + "' AND Width = '" + Width.ToString() + "'" +
                        " AND Notes = '" + Notes + "'");
                    if (fRow.Count() == 0)
                    {
                        DataRow NewRow = FrontsResultDataTable.NewRow();

                        NewRow["Patina"] = Patina;
                        NewRow["TechnoInset"] = TechnoInset;
                        NewRow["InsetType"] = InsetType;
                        NewRow["FrameColor"] = FrameColor;
                        NewRow["InsetColor"] = InsetColor;
                        NewRow["Front"] = Front;
                        NewRow["Front"] = Front;
                        NewRow["Height"] = Height;
                        NewRow["Width"] = Width;
                        NewRow["Square"] = Row["Square"];
                        NewRow["Count"] = Count;

                        NewRow["Notes"] = Notes;

                        FrontsResultDataTable.Rows.Add(NewRow);
                    }
                    else
                    {
                        fRow[0]["Square"] = Convert.ToDecimal(fRow[0]["Square"]) + Square;
                        fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Count;
                    }
                }
            }

            DT.Dispose();
            DT1.Dispose();
            DT2.Dispose();
            DT3.Dispose();
        }

        private void FillDecorByBatch()
        {
            for (int i = 0; i < DecorCatalog.DecorProductsCount; i++)
            {
                DecorResultDataTable[i].Clear();
                DecorResultDataTable[i].AcceptChanges();

                DataRow[] Rows = DecorOrdersDataTable.Select("ProductID = " + DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"].ToString());

                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow Row in Rows)
                {
                    DataRow NewRow2 = DecorResultDataTable[i].NewRow();

                    NewRow2["Product"] = DecorCatalog.DecorProductsDataTable.Rows[i]["ProductName"].ToString() + " " +
                                     DecorCatalog.GetItemName(Convert.ToInt32(Row["DecorID"]));

                    if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "Height"))
                    {
                        if (Convert.ToInt32(Row["Height"]) != -1)
                            NewRow2["Height"] = Row["Height"];
                        if (Convert.ToInt32(Row["Length"]) != -1)
                            NewRow2["Height"] = Row["Length"];
                    }
                    if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "Length"))
                    {
                        if (Convert.ToInt32(Row["Length"]) != -1)
                            NewRow2["Height"] = Row["Length"];
                        if (Convert.ToInt32(Row["Height"]) != -1)
                            NewRow2["Height"] = Row["Height"];
                    }
                    if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "Width") && Convert.ToInt32(Row["Width"]) != -1)
                        NewRow2["Width"] = Row["Width"];

                    string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                    if (Convert.ToInt32(Row["PatinaID"]) != -1)
                        Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                    //if (DecorCatalog.HasParameter(Convert.ToInt32(DecorCatalog.DecorProductsDataTable.Rows[i]["ProductID"]), "ColorID"))
                        NewRow2["Color"] = Color;

                    NewRow2["Count"] = Row["Count"];
                    NewRow2["Notes"] = Row["Notes"];
                    DecorResultDataTable[i].Rows.Add(NewRow2);
                }
            }
        }

        private bool FilterFrontsOrdersByBatch(int BatchID, int FactoryID)
        {
            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders  WHERE FrontConfigID IN " +
                "(SELECT FrontConfigID FROM infiniu2_catalog.dbo.FrontsConfig WHERE FactoryID=" +
                FactoryID + ") AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return (FrontsOrdersDataTable.Rows.Count > 0);
        }

        private bool FilterDecorOrdersByBatch(int BatchID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE DecorConfigID IN " +
                "(SELECT DecorConfigID FROM infiniu2_catalog.dbo.DecorConfig WHERE FactoryID=" +
                FactoryID + ") AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetFronts(int MainOrderID, int FactoryID)
        {
            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE Width<>-1 AND FactoryID=" +
                FactoryID + " AND MainOrderID = " + MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return (FrontsOrdersDataTable.Rows.Count > 0);
        }

        private bool GetCurvedFronts(int MainOrderID, int FactoryID)
        {
            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE Width=-1 AND FactoryID=" + FactoryID + " AND MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            return (FrontsOrdersDataTable.Rows.Count > 0);
        }

        private bool GetAllDecor(int MainOrderID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetAllDecor(int MainOrderID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE FactoryID=" + FactoryID + " AND MainOrderID=" + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetNotArchDecor(int MainOrderID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE ProductID NOT IN (31, 4, 18, 32, 10, 11, 12)" +
                " AND FactoryID=" + FactoryID + " AND MainOrderID=" + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetArchDecor(int MainOrderID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE ProductID IN (31, 4, 18, 32)" +
                " AND FactoryID=" + FactoryID + " AND MainOrderID=" + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private bool GetGrids(int MainOrderID, int FactoryID)
        {
            DecorOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders WHERE ProductID IN (10, 11, 12)" +
                " AND FactoryID=" + FactoryID + " AND MainOrderID=" + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            return (DecorOrdersDataTable.Rows.Count > 0);
        }

        private int GetCount(DataTable DT)
        {
            int S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                S += Convert.ToInt32(Row["Count"]);
            }

            return S;
        }

        private decimal GetSquare(DataTable DT)
        {
            decimal S = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    S += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return S;
        }

        public void CreateReportForMaketing(int[] MainOrders)
        {
            string ClientName = string.Empty;
            int OrderNumber = -1;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("ДЕКОР");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            int RowIndex = 0;

            string ExcelName = string.Empty;

            #region Create fonts and styles

            HSSFFont ClientNameFont = hssfworkbook.CreateFont();
            ClientNameFont.FontHeightInPoints = 14;
            ClientNameFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            ClientNameFont.FontName = "Calibri";

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 13;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle ClientNameStyle = hssfworkbook.CreateCellStyle();
            ClientNameStyle.SetFont(ClientNameFont);

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

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

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

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

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

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


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                #region Decor

                if (GetAllDecor(MainOrders[i]))
                {
                    FillDecor();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                    ClientCell.CellStyle = ClientNameStyle;

                    HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                    cell1.CellStyle = MainStyle;

                    //HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate.ToString("dd.MM.yyyy"));
                    //cell2.CellStyle = MainStyle;

                    HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                    cell3.CellStyle = MainStyle;

                    //декор
                    int DisplayIndex = 0;
                    HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                    cell15.CellStyle = HeaderStyle;
                    HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                    cell17.CellStyle = HeaderStyle;
                    HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                    cell18.CellStyle = HeaderStyle;
                    HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                    cell19.CellStyle = HeaderStyle;
                    HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                    cell20.CellStyle = HeaderStyle;
                    HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                    cell21.CellStyle = HeaderStyle;

                    RowIndex++;

                    for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                    {
                        if (DecorResultDataTable[c].Rows.Count == 0)
                            continue;

                        //вывод заказов декора в excel
                        for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                        {
                            for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                            {
                                int ColumnIndex = y;

                                //if (y == 0)
                                //{
                                //    ColumnIndex = y;
                                //}
                                //else
                                //{
                                //    ColumnIndex = y + 1;
                                //}

                                Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                            }

                            RowIndex++;
                        }
                        RowIndex++;

                    }
                }
                #endregion
            }

            ExcelName = ClientName + ", №" + OrderNumber + ", декор в работу";

            //string ReportFilePath = string.Empty;

            //ReportFilePath = ReadReportFilePath("MarketingClientReportPath.config");
            //FileInfo file = new FileInfo(ReportFilePath + ExcelName + ".xls");

            //if (!(Directory.Exists(ReportFilePath)))
            //{
            //    Directory.CreateDirectory(ReportFilePath);
            //}

            //int j = 1;
            //while (file.Exists == true)
            //{
            //    file = new FileInfo(ReportFilePath + ExcelName + "(" + j++ + ").xls");
            //}

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            ExcelName = ExcelName.Replace('\"', '\'');
            FileInfo file = new FileInfo(tempFolder + @"\" + ExcelName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + ExcelName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        private int GetFolderID(string FolderPath)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FolderID FROM Folders WHERE FolderPath = '" + FolderPath + "'",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["FolderID"]);
                    else
                        return -1;
                }
            }
        }

        private string GetFileName(string sDestFolder, string ExcelName)
        {
            string sExtension = ".xls";
            string sFileName = ExcelName;

            int j = 1;
            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
            {
                sFileName = ExcelName + "(" + j++ + ")";
            }
            sFileName = sFileName + sExtension;
            return sFileName;
        }

        public bool UploadFile(string sSourceFileName, string sDestFileName, int FolderID, ref Int64 iFileSize)
        {
            FileInfo fi;

            //get file size
            try
            {
                fi = new FileInfo(sSourceFileName);
            }
            catch
            {
                return false;
            }

            iFileSize = fi.Length;

            //load file to ftp
            FM.UploadFile(sSourceFileName, sDestFileName, Configs.FTPType);

            //add file to database
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM Files", ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["FileName"] = Path.GetFileName(sSourceFileName);
                        NewRow["FolderID"] = FolderID;
                        if (Path.GetExtension(sSourceFileName).Length > 0)
                            NewRow["FileExtension"] = Path.GetExtension(sSourceFileName).Substring(1, Path.GetExtension(sSourceFileName).Length - 1);
                        else
                            NewRow["FileExtension"] = "";
                        NewRow["FileSize"] = iFileSize;
                        NewRow["Author"] = Security.CurrentUserID;

                        DateTime Date = Security.GetCurrentDate();

                        NewRow["CreationDateTime"] = Date;
                        NewRow["LastModifiedDateTime"] = Date;
                        NewRow["LastModifiedUserID"] = Security.CurrentUserID;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }

            return true;
        }

        public void CreateReport(int MegaBatchID, int BatchID, int[] MainOrders, int FactoryID)
        {
            ResponsibleDateTime = DBNull.Value;
            TechnologyDateTime = DBNull.Value;
            ConfirmDateTime = DBNull.Value;
            CloseDateTime = DBNull.Value;
            PrintDateTime = DBNull.Value;
            ResponsibleUserID = DBNull.Value;
            TechnologyUserID = DBNull.Value;
            ConfirmUserID = DBNull.Value;
            CloseUserID = DBNull.Value;
            PrintUserID = DBNull.Value;

            string ClientName = string.Empty;
            int OrderNumber = -1;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Все заказы");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            int RowIndex = 0;

            string ExcelName = string.Empty;

            ExcelName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль (по подзаказам)";

            if (FactoryID == 2)
                ExcelName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС (по подзаказам)";

            #region Create fonts and styles

            HSSFFont ClientNameFont = hssfworkbook.CreateFont();
            ClientNameFont.FontHeightInPoints = 14;
            ClientNameFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            ClientNameFont.FontName = "Calibri";

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 13;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle ClientNameStyle = hssfworkbook.CreateCellStyle();
            ClientNameStyle.SetFont(ClientNameFont);

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

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

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

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

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

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


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            FrontsExcelSheet(ref hssfworkbook, ClientNameStyle, HeaderStyle, MainStyle,
                SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, FactoryID, MegaBatchID, BatchID);
            CurvedExcelSheet(ref hssfworkbook, ClientNameStyle, HeaderStyle, MainStyle,
                SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, FactoryID, MegaBatchID, BatchID);
            NotArchDecorExcelSheet(ref hssfworkbook, ClientNameStyle, HeaderStyle, MainStyle,
                SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, FactoryID, MegaBatchID, BatchID);
            ArchDecorExcelSheet(ref hssfworkbook, ClientNameStyle, HeaderStyle, MainStyle,
                SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, FactoryID, MegaBatchID, BatchID);
            GridsExcelSheet(ref hssfworkbook, ClientNameStyle, HeaderStyle, MainStyle,
                SimpleFont, SimpleCellStyle, PackNumberFont, TempStyle, MainOrders, FactoryID, MegaBatchID, BatchID);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                decimal Square = 0;
                int FrontsCount = 0;

                #region Fronts

                if (GetFronts(MainOrders[i], FactoryID))
                {
                    FillFronts();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    OneKitchen(ref hssfworkbook, FrontsResultDataTable, ClientName,
                        MegaBatchID, BatchID, MainOrders[i], FactoryID);

                    HSSFCell cell0 = null;
                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    if (ConfirmDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    if (CloseDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }

                    cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                        "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                    cell0.CellStyle = TempStyle;

                    string BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

                    if (FactoryID == 2)
                        BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";

                    HSSFCell BatchNamecell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
                    BatchNamecell1.CellStyle = ClientNameStyle;

                    HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                    ClientCell.CellStyle = ClientNameStyle;

                    HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                    cell1.CellStyle = MainStyle;

                    HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                    cell3.CellStyle = MainStyle;

                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        int DisplayIndex = 0;
                        HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                        cell4.CellStyle = HeaderStyle;
                        HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                        cell5.CellStyle = HeaderStyle;
                        HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                        cell6.CellStyle = HeaderStyle;
                        HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                        cell7.CellStyle = HeaderStyle;
                        HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                        cell8.CellStyle = HeaderStyle;

                        HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                        cell9.CellStyle = HeaderStyle;
                        HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                        cell10.CellStyle = HeaderStyle;
                        HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell11.CellStyle = HeaderStyle;
                        HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell12.CellStyle = HeaderStyle;
                        HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                        cell13.CellStyle = HeaderStyle;
                        HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell14.CellStyle = HeaderStyle;

                        RowIndex++;

                        Square = GetSquare(FrontsResultDataTable);
                        FrontsCount = GetCount(FrontsResultDataTable);
                    }

                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                        {
                            Type t = FrontsResultDataTable.Rows[x][y].GetType();

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }
                        }
                        RowIndex++;
                    }

                    HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                    cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                    cellStyle1.SetFont(PackNumberFont);

                    HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                    cell15.CellStyle = cellStyle1;
                    HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                    cell16.CellStyle = cellStyle1;

                    if (Square > 0)
                    {
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                            Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                        cell17.CellStyle = cellStyle1;
                    }

                    RowIndex++;
                }

                #endregion

                #region Decor

                if (GetAllDecor(MainOrders[i], FactoryID))
                {
                    FillDecor();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    HSSFCell cell0 = null;
                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    if (ConfirmDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    if (CloseDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    //if (PrintDateTime != DBNull.Value)
                    //{
                    cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                        "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                    cell0.CellStyle = TempStyle;
                    //}
                    HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                    ClientCell.CellStyle = ClientNameStyle;

                    HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                    cell1.CellStyle = MainStyle;

                    //HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate.ToString("dd.MM.yyyy"));
                    //cell2.CellStyle = MainStyle;

                    HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                    cell3.CellStyle = MainStyle;

                    //декор
                    int DisplayIndex = 0;
                    HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                    cell15.CellStyle = HeaderStyle;
                    HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                    cell17.CellStyle = HeaderStyle;
                    HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                    cell18.CellStyle = HeaderStyle;
                    HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                    cell19.CellStyle = HeaderStyle;
                    HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                    cell20.CellStyle = HeaderStyle;
                    HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                    cell21.CellStyle = HeaderStyle;

                    RowIndex++;

                    for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                    {
                        if (DecorResultDataTable[c].Rows.Count == 0)
                            continue;

                        //вывод заказов декора в excel
                        for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                        {
                            for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                            {
                                int ColumnIndex = y;

                                //if (y == 0)
                                //{
                                //    ColumnIndex = y;
                                //}
                                //else
                                //{
                                //    ColumnIndex = y + 1;
                                //}

                                Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                            }

                            RowIndex++;
                        }
                        RowIndex++;

                    }
                }
                #endregion
            }

            Int64 iFileSize = 0;
            string sSourceFolder = System.Environment.GetEnvironmentVariable("TEMP");
            string sFolderPath = "Общие файлы/Производство/Недельное планирование (маркетинг)";
            string sDestFolder = Configs.DocumentsPath + sFolderPath;
            string sSourceFileName = GetFileName(sDestFolder, ExcelName);

            FileInfo file = new FileInfo(sSourceFolder + @"\" + sSourceFileName);
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            try
            {
                int FolderID = GetFolderID(sFolderPath);
                if (FolderID != -1)
                    UploadFile(sSourceFolder + @"\" + sSourceFileName, sDestFolder + "/" + sSourceFileName, FolderID, ref iFileSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            System.Threading.Thread T = new System.Threading.Thread(delegate ()
            {
                FM.DownloadFile(sDestFolder + "/" + sSourceFileName, sSourceFolder + @"\" + sSourceFileName, iFileSize, Configs.FTPType);
            });
            T.Start();

            while (T.IsAlive)
            {
                T.Join(50);
                Application.DoEvents();
            }
            System.Diagnostics.Process.Start(sSourceFolder + @"\" + sSourceFileName);
        }

        public void CreateReport(int MegaBatchID, int BatchID, int FactoryID)
        {
            ResponsibleDateTime = DBNull.Value;
            TechnologyDateTime = DBNull.Value;
            ConfirmDateTime = DBNull.Value;
            CloseDateTime = DBNull.Value;
            PrintDateTime = DBNull.Value;
            ResponsibleUserID = DBNull.Value;
            TechnologyUserID = DBNull.Value;
            ConfirmUserID = DBNull.Value;
            CloseUserID = DBNull.Value;
            PrintUserID = DBNull.Value;

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Партия");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            int RowIndex = 1;

            int TopRowFront = 1;
            int BottomRowFront = 1;

            string BatchName = string.Empty;
            string ExcelName = string.Empty;
            string MainOrdersList = string.Empty;

            #region Create fonts and styles

            HSSFFont ClientNameFont = hssfworkbook.CreateFont();
            ClientNameFont.FontHeightInPoints = 14;
            ClientNameFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            ClientNameFont.FontName = "Calibri";

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 13;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle ClientNameStyle = hssfworkbook.CreateCellStyle();
            ClientNameStyle.SetFont(ClientNameFont);

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

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

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

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

            HSSFCellStyle GreyCellStyle = hssfworkbook.CreateCellStyle();
            GreyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyCellStyle.SetFont(SimpleFont);

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

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


            HSSFCellStyle GreyBottomMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyBottomMediumLeftBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumLeftBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumLeftBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumLeftBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumLeftBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyBottomMediumLeftBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyBottomMediumLeftBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyBottomMediumRightBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyBottomMediumRightBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyBottomMediumRightBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyBottomMediumRightBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyBottomMediumRightBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyBottomMediumRightBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyBottomMediumRightBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyBottomMediumRightBorderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle GreyLeftMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyLeftMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            GreyLeftMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyLeftMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyLeftMediumBorderCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            GreyLeftMediumBorderCellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            GreyLeftMediumBorderCellStyle.SetFont(PackNumberFont);

            HSSFCellStyle GreyRightMediumBorderCellStyle = hssfworkbook.CreateCellStyle();
            GreyRightMediumBorderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            GreyRightMediumBorderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyRightMediumBorderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyRightMediumBorderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyRightMediumBorderCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyRightMediumBorderCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyRightMediumBorderCellStyle.SetFont(SimpleFont);
            #endregion

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";
            ExcelName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
                ExcelName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell cell0 = null;
            GetBatchInfoByBatch(BatchID, FactoryID);
            if (ConfirmDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            if (CloseDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }

            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
            cell0.CellStyle = TempStyle;

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            cell1.CellStyle = MainStyle;

            ArrayList array = GetMainOrders(BatchID, FactoryID);

            int RowCount = 1;

            for (int i = 0; i < array.Count; i++)
            {
                MainOrdersList += array[i].ToString() + ", ";

                if (i > 0 && i % 9 == 0)
                {
                    if (MainOrdersList.Length > 3)
                        MainOrdersList = MainOrdersList.Substring(0, MainOrdersList.Length - 2);
                    cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, MainOrdersList);
                    cell0.CellStyle = MainStyle;
                    MainOrdersList = string.Empty;
                    RowCount++;
                }
                if (i == array.Count - 1)
                {
                    if (MainOrdersList.Length > 3)
                        MainOrdersList = MainOrdersList.Substring(0, MainOrdersList.Length - 2);
                    cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, MainOrdersList);
                    cell0.CellStyle = MainStyle;
                    MainOrdersList = string.Empty;
                    RowCount++;
                }
            }

            decimal Square = 0;
            int FrontsCount = 0;

            #region Fronts

            if (FilterFrontsOrdersByBatch(BatchID, FactoryID))
            {
                FillFrontsByBatch();

                if (FrontsResultDataTable.Rows.Count != 0)
                {
                    int DisplayIndex = 0;
                    HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                    cell4.CellStyle = HeaderStyle;
                    HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                    cell5.CellStyle = HeaderStyle;
                    HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                    cell6.CellStyle = HeaderStyle;
                    HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                    cell7.CellStyle = HeaderStyle;
                    HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                    cell8.CellStyle = HeaderStyle;

                    HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                    cell9.CellStyle = HeaderStyle;
                    HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                    cell10.CellStyle = HeaderStyle;
                    HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                    cell11.CellStyle = HeaderStyle;
                    HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                    cell12.CellStyle = HeaderStyle;
                    HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                    cell13.CellStyle = HeaderStyle;
                    HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                    cell14.CellStyle = HeaderStyle;

                    RowIndex++;

                    Square = GetSquare(FrontsResultDataTable);
                    FrontsCount = GetCount(FrontsResultDataTable);
                }

                TopRowFront = RowIndex;
                BottomRowFront = FrontsResultDataTable.Rows.Count + RowIndex;

                //вывод заказов фасадов
                for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                {
                    if (FrontsResultDataTable.Rows.Count == 0)
                        break;

                    for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                    {
                        Type t = FrontsResultDataTable.Rows[x][y].GetType();

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

                            HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                            continue;
                        }
                        if (t.Name == "Int32")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                            cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }
                    }
                    RowIndex++;
                }

                HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                cellStyle1.SetFont(PackNumberFont);

                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                cell15.CellStyle = cellStyle1;
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                cell16.CellStyle = cellStyle1;

                if (Square > 0)
                {
                    HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                        Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                    cell17.CellStyle = cellStyle1;
                }

                if (FrontsResultDataTable.Rows.Count != 0)
                    RowIndex++;

                RowIndex++;
            }

            #endregion

            #region Decor

            if (FilterDecorOrdersByBatch(BatchID, FactoryID))
            {
                FillDecorByBatch();

                //декор
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;

                for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                {
                    if (DecorResultDataTable[c].Rows.Count == 0)
                        continue;

                    //вывод заказов декора в excel
                    for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                    {
                        for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                            //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                        }

                        RowIndex++;
                    }
                    RowIndex++;

                }


                RowIndex++;
                RowIndex++;
            }
            #endregion

            CurvedDepersonalized(ref hssfworkbook, MainStyle, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MegaBatchID, BatchID, FactoryID);

            GridsDepersonalized(ref hssfworkbook, MainStyle, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MegaBatchID, BatchID, FactoryID);

            CabinetDepersonalized(ref hssfworkbook, MainStyle, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MegaBatchID, BatchID, FactoryID);

            ArchDepersonalized(ref hssfworkbook, MainStyle, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MegaBatchID, BatchID, FactoryID);

            RestDecorDepersonalized(ref hssfworkbook, MainStyle, HeaderStyle, SimpleFont, SimpleCellStyle,
                PackNumberFont, TempStyle, MegaBatchID, BatchID, FactoryID);

            string ReportFilePath = string.Empty;

            Int64 iFileSize = 0;
            string sSourceFolder = System.Environment.GetEnvironmentVariable("TEMP");
            string sFolderPath = "Общие файлы/Производство/Недельное планирование (маркетинг)";
            string sDestFolder = Configs.DocumentsPath + sFolderPath;
            string sSourceFileName = GetFileName(sDestFolder, ExcelName);

            FileInfo file = new FileInfo(sSourceFolder + @"\" + sSourceFileName);
            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            try
            {
                int FolderID = GetFolderID(sFolderPath);
                if (FolderID != -1)
                    UploadFile(sSourceFolder + @"\" + sSourceFileName, sDestFolder + "/" + sSourceFileName, FolderID, ref iFileSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            System.Threading.Thread T = new System.Threading.Thread(delegate ()
            {
                FM.DownloadFile(sDestFolder + "/" + sSourceFileName, sSourceFolder + @"\" + sSourceFileName, iFileSize, Configs.FTPType);
            });
            T.Start();

            while (T.IsAlive)
            {
                T.Join(50);
                Application.DoEvents();
            }
            System.Diagnostics.Process.Start(sSourceFolder + @"\" + sSourceFileName);
        }

        private void OneKitchen(ref HSSFWorkbook hssfworkbook, DataTable FrontsResultDataTable, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet(MegaBatchID.ToString() + ", " + BatchID.ToString() + ", " + MainOrderID.ToString());
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            #region Create fonts and styles

            HSSFFont ClientNameFont = hssfworkbook.CreateFont();
            ClientNameFont.FontHeightInPoints = 14;
            ClientNameFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            ClientNameFont.FontName = "Calibri";

            HSSFCellStyle ClientNameStyle = hssfworkbook.CreateCellStyle();
            ClientNameStyle.SetFont(ClientNameFont);

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 13;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            HSSFCellStyle MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            HSSFFont PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

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

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);
            #endregion

            int RowIndex = 0;

            HSSFCell ConfirmCell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, "Утверждаю...............");
            ConfirmCell2.CellStyle = TempStyle;
            HSSFCell CreateDateCell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2,
                "Жен1. (" + GetUserName(Security.CurrentUserID) + " от " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm") + ")");
            CreateDateCell2.CellStyle = TempStyle;

            RowIndex = WomenExcelSheet1(ref hssfworkbook, ref sheet1, ClientNameStyle, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont,
                RowIndex, ClientName, MegaBatchID, BatchID, MainOrderID, FactoryID);

            HSSFCell ConfirmCell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, "Утверждаю...............");
            ConfirmCell1.CellStyle = TempStyle;
            HSSFCell CreateDateCell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2,
                "Муж1. (" + GetUserName(Security.CurrentUserID) + " от " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm") + ")");
            CreateDateCell1.CellStyle = TempStyle;

            RowIndex = MenExcelSheet1(ref hssfworkbook, ref sheet1, ClientNameStyle, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont,
                RowIndex, ClientName, MegaBatchID, BatchID, MainOrderID, FactoryID);

            HSSFCell ConfirmCell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, "Утверждаю...............");
            ConfirmCell6.CellStyle = TempStyle;
            HSSFCell CreateDateCell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2,
                "Жен2. (" + GetUserName(Security.CurrentUserID) + " от " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm") + ")");
            CreateDateCell6.CellStyle = TempStyle;

            RowIndex = WomenExcelSheet2(ref hssfworkbook, ref sheet1, ClientNameStyle, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont,
                RowIndex, ClientName, MegaBatchID, BatchID, MainOrderID, FactoryID);

            HSSFCell ConfirmCell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, "Утверждаю...............");
            ConfirmCell5.CellStyle = TempStyle;
            HSSFCell CreateDateCell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2,
                "Муж2. (" + GetUserName(Security.CurrentUserID) + " от " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm") + ")");
            CreateDateCell5.CellStyle = TempStyle;

            RowIndex = MenExcelSheet2(ref hssfworkbook, ref sheet1, ClientNameStyle, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont,
                RowIndex, ClientName, MegaBatchID, BatchID, MainOrderID, FactoryID);

            HSSFCell ConfirmCell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, "Утверждаю...............");
            ConfirmCell3.CellStyle = TempStyle;
            HSSFCell CreateDateCell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2,
                "Упаковка. (" + GetUserName(Security.CurrentUserID) + " от " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm") + ")");
            CreateDateCell3.CellStyle = TempStyle;

            RowIndex = PackingExcelSheet(ref hssfworkbook, ref sheet1, ClientNameStyle, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont,
                RowIndex, ClientName, MegaBatchID, BatchID, MainOrderID, FactoryID);

            HSSFCell ConfirmCell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, "Утверждаю...............");
            ConfirmCell4.CellStyle = TempStyle;
            HSSFCell CreateDateCell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2,
                "Сверление. (" + GetUserName(Security.CurrentUserID) + " от " + GetCurrentDate.ToString("dd.MM.yyyy HH:mm") + ")");
            CreateDateCell4.CellStyle = TempStyle;

            RowIndex = BoringExcelSheet(ref hssfworkbook, ref sheet1, ClientNameStyle, HeaderStyle, SimpleFont, SimpleCellStyle, PackNumberFont,
                RowIndex, ClientName, MegaBatchID, BatchID, MainOrderID, FactoryID);
        }

        #region Обезличено
        private int CurvedDepersonalized(ref HSSFWorkbook hssfworkbook, HSSFCellStyle MainStyle,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int MegaBatchID, int BatchID, int FactoryID)
        {
            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE Width=-1 AND FactoryID=" + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ") ORDER BY FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoInsetTypeID, TechnoInsetColorID, Height", ConnectionStrings.MarketingOrdersConnectionString))
            {
                FrontsOrdersDataTable.Clear();
                DA.Fill(FrontsOrdersDataTable);
            }

            if (FrontsOrdersDataTable.Rows.Count < 1)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Гнутые");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            string BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";

            HSSFCell cell0 = null;
            GetBatchInfoByBatch(BatchID, FactoryID);
            if (ConfirmDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            if (CloseDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            //if (PrintDateTime != DBNull.Value)
            //{
            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
            cell0.CellStyle = TempStyle;
            //}
            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            cell1.CellStyle = MainStyle;

            FillFrontsByBatch();

            int FrontsCount = 0;

            if (FrontsResultDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                RowIndex++;

                FrontsCount = GetCount(FrontsResultDataTable);
            }

            //вывод заказов фасадов
            for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
            {
                for (int y = 0; y < FrontsResultDataTable.Columns.Count - 2; y++)
                {
                    Type t = FrontsResultDataTable.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            return ++RowIndex;
        }

        private int GridsDepersonalized(ref HSSFWorkbook hssfworkbook, HSSFCellStyle MainStyle,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int MegaBatchID, int BatchID, int FactoryID)
        {
            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders" +
                " WHERE ProductID IN (10, 11, 12) AND FactoryID=" + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ") ORDER BY ProductID, DecorID, ColorID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Решетки");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            string BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";

            HSSFCell cell0 = null;
            GetBatchInfoByBatch(BatchID, FactoryID);
            if (ConfirmDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            if (CloseDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            //if (PrintDateTime != DBNull.Value)
            //{
            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
            cell0.CellStyle = TempStyle;
            //}
            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            cell1.CellStyle = MainStyle;

            FillDecorByBatch();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

                            HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                            continue;
                        }
                        if (t.Name == "Int32")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }
                HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                cellStyle1.SetFont(PackNumberFont);

                HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                cell22.CellStyle = cellStyle1;
                HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
                cell23.CellStyle = cellStyle1;

                RowIndex++;

            }

            return ++RowIndex;
        }

        private int CabinetDepersonalized(ref HSSFWorkbook hssfworkbook, HSSFCellStyle MainStyle,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int MegaBatchID, int BatchID, int FactoryID)
        {
            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders" +
                " WHERE ProductID IN (24) AND FactoryID=" + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ") ORDER BY ProductID, DecorID, ColorID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Полки");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            string BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";

            HSSFCell cell0 = null;
            GetBatchInfoByBatch(BatchID, FactoryID);
            if (ConfirmDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            if (CloseDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            //if (PrintDateTime != DBNull.Value)
            //{
            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
            cell0.CellStyle = TempStyle;
            //}
            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            cell1.CellStyle = MainStyle;

            FillDecorByBatch();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

                            HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                            continue;
                        }
                        if (t.Name == "Int32")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }
                HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                cellStyle1.SetFont(PackNumberFont);

                HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                cell22.CellStyle = cellStyle1;
                HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
                cell23.CellStyle = cellStyle1;

                RowIndex++;

            }

            return ++RowIndex;
        }

        private int ArchDepersonalized(ref HSSFWorkbook hssfworkbook, HSSFCellStyle MainStyle,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int MegaBatchID, int BatchID, int FactoryID)
        {
            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders" +
                " WHERE ProductID IN (31, 4, 18, 32) AND FactoryID=" + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ") ORDER BY ProductID, DecorID, ColorID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Арки");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            string BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";

            HSSFCell cell0 = null;
            GetBatchInfoByBatch(BatchID, FactoryID);
            if (ConfirmDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            if (CloseDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            //if (PrintDateTime != DBNull.Value)
            //{
            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
            cell0.CellStyle = TempStyle;
            //}
            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            cell1.CellStyle = MainStyle;

            FillDecorByBatch();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

                            HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                            continue;
                        }
                        if (t.Name == "Int32")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }
                HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                cellStyle1.SetFont(PackNumberFont);

                HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                cell22.CellStyle = cellStyle1;
                HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
                cell23.CellStyle = cellStyle1;

                RowIndex++;

            }

            return ++RowIndex;
        }

        private int RestDecorDepersonalized(ref HSSFWorkbook hssfworkbook, HSSFCellStyle MainStyle,
            HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle,
            HSSFFont PackNumberFont, HSSFCellStyle TempStyle, int MegaBatchID, int BatchID, int FactoryID)
        {
            int RowIndex = 1;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT * FROM DecorOrders" +
                " WHERE ProductID NOT IN (31, 4, 18, 32, 10, 11, 12, 31, 4, 18, 32, 24) AND FactoryID=" + FactoryID +
                " AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE FactoryID = " + FactoryID +
                " AND BatchID = " + BatchID + ") ORDER BY ProductID, DecorID, ColorID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DecorOrdersDataTable.Clear();
                DA.Fill(DecorOrdersDataTable);
            }

            if (DecorOrdersDataTable.Rows.Count == 0)
                return RowIndex;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Остальное");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            string BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";

            HSSFCell cell0 = null;
            GetBatchInfoByBatch(BatchID, FactoryID);
            if (ConfirmDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            if (CloseDateTime != DBNull.Value)
            {
                cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell0.CellStyle = TempStyle;
            }
            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
            cell0.CellStyle = TempStyle;

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            cell1.CellStyle = MainStyle;

            FillDecorByBatch();

            int DecorCount = 0;

            if (DecorOrdersDataTable.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell15.CellStyle = HeaderStyle;
                HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell17.CellStyle = HeaderStyle;
                HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell18.CellStyle = HeaderStyle;
                HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell19.CellStyle = HeaderStyle;
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell20.CellStyle = HeaderStyle;
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell21.CellStyle = HeaderStyle;

                RowIndex++;
            }

            //вывод заказов фасадов
            for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
            {
                if (DecorResultDataTable[c].Rows.Count == 0)
                    continue;

                DecorCount = GetCount(DecorResultDataTable[c]);

                //вывод заказов декора в excel
                for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                {
                    for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                    {
                        int ColumnIndex = y;

                        //if (y == 0)
                        //{
                        //    ColumnIndex = y;
                        //}
                        //else
                        //{
                        //    ColumnIndex = y + 1;
                        //}

                        Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                        //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                        if (t.Name == "Decimal")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

                            HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                            continue;
                        }
                        if (t.Name == "Int32")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                        if (t.Name == "String" || t.Name == "DBNull")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                            cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                            cell.CellStyle = SimpleCellStyle;
                            continue;
                        }

                    }

                    RowIndex++;
                }
                HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                cellStyle1.SetFont(PackNumberFont);

                HSSFCell cell22 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                cell22.CellStyle = cellStyle1;
                HSSFCell cell23 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, DecorCount + " шт.");
                cell23.CellStyle = cellStyle1;

                RowIndex++;

            }

            return ++RowIndex;
        }

        #endregion

        #region По подзаказам
        private void FrontsExcelSheet(ref HSSFWorkbook hssfworkbook,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFCellStyle MainStyle,
            HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont, HSSFCellStyle TempStyle,
            int[] MainOrders, int FactoryID, int MegaBatchID, int BatchID)
        {
            bool AreFronts = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetFronts(MainOrders[i], FactoryID))
                {
                    AreFronts = true;
                    break;
                }
            }

            if (!AreFronts)
                return;

            string ClientName = string.Empty;
            string BatchName = string.Empty;
            int OrderNumber = -1;
            int RowIndex = 0;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Фасады");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                decimal Square = 0;
                int FrontsCount = 0;

                if (GetFronts(MainOrders[i], FactoryID))
                {
                    FillFronts();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    HSSFCell cell0 = null;
                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    if (ConfirmDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    if (CloseDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    //if (PrintDateTime != DBNull.Value)
                    //{
                    cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                        "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                    cell0.CellStyle = TempStyle;
                    //}
                    BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

                    if (FactoryID == 2)
                    {
                        BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
                    }

                    HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
                    Batchcell1.CellStyle = MainStyle;

                    HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                    ClientCell.CellStyle = ClientNameStyle;

                    HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                    cell1.CellStyle = MainStyle;

                    HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                    cell3.CellStyle = MainStyle;

                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        int DisplayIndex = 0;
                        HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                        cell4.CellStyle = HeaderStyle;
                        HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                        cell5.CellStyle = HeaderStyle;
                        HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                        cell6.CellStyle = HeaderStyle;
                        HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                        cell7.CellStyle = HeaderStyle;
                        HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                        cell8.CellStyle = HeaderStyle;

                        HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                        cell9.CellStyle = HeaderStyle;
                        HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                        cell10.CellStyle = HeaderStyle;
                        HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell11.CellStyle = HeaderStyle;
                        HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell12.CellStyle = HeaderStyle;
                        HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                        cell13.CellStyle = HeaderStyle;
                        HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell14.CellStyle = HeaderStyle;

                        RowIndex++;

                        Square = GetSquare(FrontsResultDataTable);
                        FrontsCount = GetCount(FrontsResultDataTable);
                    }

                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                        {
                            Type t = FrontsResultDataTable.Rows[x][y].GetType();

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }
                        }
                        RowIndex++;
                    }

                    HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                    cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                    cellStyle1.SetFont(PackNumberFont);

                    HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                    cell15.CellStyle = cellStyle1;
                    HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                    cell16.CellStyle = cellStyle1;

                    if (Square > 0)
                    {
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                            Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                        cell17.CellStyle = cellStyle1;
                    }

                    RowIndex++;
                }
            }
        }

        private void CurvedExcelSheet(ref HSSFWorkbook hssfworkbook,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFCellStyle MainStyle,
            HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont, HSSFCellStyle TempStyle,
            int[] MainOrders, int FactoryID, int MegaBatchID, int BatchID)
        {
            bool AreFronts = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetCurvedFronts(MainOrders[i], FactoryID))
                {
                    AreFronts = true;
                    break;
                }
            }

            if (!AreFronts)
                return;

            string ClientName = string.Empty;
            string BatchName = string.Empty;
            int OrderNumber = -1;
            int RowIndex = 0;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Гнутые");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 18 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 9 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 5 * 256);
            sheet1.SetColumnWidth(7, 5 * 256);
            sheet1.SetColumnWidth(8, 5 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            sheet1.SetColumnWidth(11, 7 * 256);
            sheet1.SetColumnWidth(12, 7 * 256);
            sheet1.SetColumnWidth(13, 7 * 256);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                decimal Square = 0;
                int FrontsCount = 0;

                if (GetCurvedFronts(MainOrders[i], FactoryID))
                {
                    FillFronts();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    HSSFCell cell0 = null;
                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    if (ConfirmDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    if (CloseDateTime != DBNull.Value)
                    {
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                    }
                    //if (PrintDateTime != DBNull.Value)
                    //{
                    cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                        "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                    cell0.CellStyle = TempStyle;
                    //}
                    BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

                    if (FactoryID == 2)
                    {
                        BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
                    }

                    HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
                    Batchcell1.CellStyle = MainStyle;

                    HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                    ClientCell.CellStyle = ClientNameStyle;

                    HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                    cell1.CellStyle = MainStyle;

                    HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                    cell3.CellStyle = MainStyle;

                    if (FrontsResultDataTable.Rows.Count != 0)
                    {
                        int DisplayIndex = 0;
                        HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                        cell4.CellStyle = HeaderStyle;
                        HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                        cell5.CellStyle = HeaderStyle;
                        HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                        cell6.CellStyle = HeaderStyle;
                        HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                        cell7.CellStyle = HeaderStyle;
                        HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                        cell8.CellStyle = HeaderStyle;

                        HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                        cell9.CellStyle = HeaderStyle;
                        HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                        cell10.CellStyle = HeaderStyle;
                        HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell11.CellStyle = HeaderStyle;
                        HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell12.CellStyle = HeaderStyle;
                        HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                        cell13.CellStyle = HeaderStyle;
                        HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell14.CellStyle = HeaderStyle;

                        RowIndex++;

                        Square = GetSquare(FrontsResultDataTable);
                        FrontsCount = GetCount(FrontsResultDataTable);
                    }

                    //вывод заказов фасадов
                    for (int x = 0; x < FrontsResultDataTable.Rows.Count; x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count; y++)
                        {
                            Type t = FrontsResultDataTable.Rows[x][y].GetType();

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(Convert.ToDouble(FrontsResultDataTable.Rows[x][y]));

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
                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FrontsResultDataTable.Rows[x][y]));
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                                cell.SetCellValue(FrontsResultDataTable.Rows[x][y].ToString());
                                cell.CellStyle = SimpleCellStyle;
                                continue;
                            }
                        }
                        RowIndex++;
                    }

                    HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
                    cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                    cellStyle1.SetFont(PackNumberFont);

                    HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
                    cell15.CellStyle = cellStyle1;
                    HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
                    cell16.CellStyle = cellStyle1;

                    if (Square > 0)
                    {
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                            Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                        cell17.CellStyle = cellStyle1;
                    }

                    RowIndex++;
                }
            }
        }

        private void NotArchDecorExcelSheet(ref HSSFWorkbook hssfworkbook,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFCellStyle MainStyle,
            HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont, HSSFCellStyle TempStyle,
            int[] MainOrders, int FactoryID, int MegaBatchID, int BatchID)
        {
            #region Декор без Арки, Бутылочницы, Полочницы, Накладки ящика

            bool NotArchDecor = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetNotArchDecor(MainOrders[i], FactoryID))
                {
                    NotArchDecor = true;
                    break;
                }
            }

            if (!NotArchDecor)
                return;

            string ClientName = string.Empty;
            string BatchName = string.Empty;

            int OrderNumber = -1;
            int RowIndex = 0;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Декор");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetNotArchDecor(MainOrders[i], FactoryID))
                {
                    FillDecor();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    for (int j = 0; j < 2; j++)
                    {
                        HSSFCell cell0 = null;
                        if (ConfirmDateTime != DBNull.Value)
                        {
                            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                            cell0.CellStyle = TempStyle;
                        }
                        if (CloseDateTime != DBNull.Value)
                        {
                            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                            cell0.CellStyle = TempStyle;
                        }
                        //if (PrintDateTime != DBNull.Value)
                        //{
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                        //}
                        BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

                        if (FactoryID == 2)
                        {
                            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
                        }

                        HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 4, "УТВЕРЖДАЮ...............");
                        cell.CellStyle = MainStyle;

                        HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
                        Batchcell1.CellStyle = MainStyle;

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        if (j == 1)
                        {
                            HSSFCell RepeatCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex - 1), 5, "ДУБЛЬ");
                            RepeatCell.CellStyle = ClientNameStyle;
                        }

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                        cell1.CellStyle = MainStyle;

                        HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                        cell3.CellStyle = MainStyle;

                        //декор
                        int DisplayIndex = 0;
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                        cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Ответственный:");
                        cell.CellStyle = MainStyle;
                        cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Выполнил:");
                        cell.CellStyle = MainStyle;
                    }
                }
            }
            #endregion
        }

        private void ArchDecorExcelSheet(ref HSSFWorkbook hssfworkbook,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFCellStyle MainStyle,
            HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont, HSSFCellStyle TempStyle,
            int[] MainOrders, int FactoryID, int MegaBatchID, int BatchID)
        {
            #region Арки, Бутылочницы, Полочницы, Накладки ящика

            bool ArchDecor = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetArchDecor(MainOrders[i], FactoryID))
                {
                    ArchDecor = true;
                    break;
                }
            }

            if (!ArchDecor)
                return;

            string ClientName = string.Empty;
            string BatchName = string.Empty;
            int OrderNumber = -1;
            int RowIndex = 0;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Арки");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetArchDecor(MainOrders[i], FactoryID))
                {
                    FillDecor();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    for (int j = 0; j < 2; j++)
                    {
                        HSSFCell cell0 = null;
                        if (ConfirmDateTime != DBNull.Value)
                        {
                            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                            cell0.CellStyle = TempStyle;
                        }
                        if (CloseDateTime != DBNull.Value)
                        {
                            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                            cell0.CellStyle = TempStyle;
                        }
                        //if (PrintDateTime != DBNull.Value)
                        //{
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                        //}
                        BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

                        if (FactoryID == 2)
                        {
                            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
                        }

                        HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 4, "УТВЕРЖДАЮ...............");
                        cell.CellStyle = MainStyle;

                        HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
                        Batchcell1.CellStyle = MainStyle;

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        if (j == 1)
                        {
                            HSSFCell RepeatCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex - 1), 5, "ДУБЛЬ");
                            RepeatCell.CellStyle = ClientNameStyle;
                        }

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                        cell1.CellStyle = MainStyle;

                        HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                        cell3.CellStyle = MainStyle;

                        //декор
                        int DisplayIndex = 0;
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                        cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Ответственный:");
                        cell.CellStyle = MainStyle;
                        cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Выполнил:");
                        cell.CellStyle = MainStyle;

                    }
                }
            }
            #endregion
        }

        private void GridsExcelSheet(ref HSSFWorkbook hssfworkbook,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFCellStyle MainStyle,
            HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont, HSSFCellStyle TempStyle,
            int[] MainOrders, int FactoryID, int MegaBatchID, int BatchID)
        {
            #region Решетки

            bool Grids = false;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetGrids(MainOrders[i], FactoryID))
                {
                    Grids = true;
                    break;
                }
            }

            if (!Grids)
                return;

            string ClientName = string.Empty;
            string BatchName = string.Empty;

            int OrderNumber = -1;
            int RowIndex = 0;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Решетки");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 10 * 256);
            sheet1.SetColumnWidth(4, 6 * 256);
            sheet1.SetColumnWidth(5, 10 * 256);

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                if (GetGrids(MainOrders[i], FactoryID))
                {
                    FillDecor();

                    ClientName = GetClientName(MainOrders[i]);
                    OrderNumber = GetOrderNumber(MainOrders[i]);

                    GetBatchInfoByMainOrder(MainOrders[i], FactoryID);
                    for (int j = 0; j < 2; j++)
                    {
                        HSSFCell cell0 = null;
                        if (ConfirmDateTime != DBNull.Value)
                        {
                            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                "Утвердил партию: " + GetUserName(Convert.ToInt32(ConfirmUserID)) + " " + Convert.ToDateTime(ConfirmDateTime).ToString("dd.MM.yyyy HH:mm"));
                            cell0.CellStyle = TempStyle;
                        }
                        if (CloseDateTime != DBNull.Value)
                        {
                            cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                                "Закрыл партию: " + GetUserName(Convert.ToInt32(CloseUserID)) + " " + Convert.ToDateTime(CloseDateTime).ToString("dd.MM.yyyy HH:mm"));
                            cell0.CellStyle = TempStyle;
                        }
                        //if (PrintDateTime != DBNull.Value)
                        //{
                        cell0 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                            "Распечатал: " + GetUserName(Convert.ToInt32(Security.CurrentUserID)) + " " + Convert.ToDateTime(GetCurrentDate).ToString("dd.MM.yyyy HH:mm"));
                        cell0.CellStyle = TempStyle;
                        //}
                        BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

                        if (FactoryID == 2)
                        {
                            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
                        }

                        HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 4, "УТВЕРЖДАЮ...............");
                        cell.CellStyle = MainStyle;

                        HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
                        Batchcell1.CellStyle = MainStyle;

                        HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, ClientName);
                        ClientCell.CellStyle = ClientNameStyle;

                        if (j == 1)
                        {
                            HSSFCell RepeatCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex - 1), 5, "ДУБЛЬ");
                            RepeatCell.CellStyle = ClientNameStyle;
                        }

                        HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "№" + OrderNumber + "-" + MainOrders[i].ToString());
                        cell1.CellStyle = MainStyle;

                        HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание к заказу: " + GetMainOrderNotes(MainOrders[i]));
                        cell3.CellStyle = MainStyle;

                        //декор
                        int DisplayIndex = 0;
                        HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Наименование");
                        cell15.CellStyle = HeaderStyle;
                        HSSFCell cell17 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                        cell17.CellStyle = HeaderStyle;
                        HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                        cell18.CellStyle = HeaderStyle;
                        HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                        cell19.CellStyle = HeaderStyle;
                        HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                        cell20.CellStyle = HeaderStyle;
                        HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                        cell21.CellStyle = HeaderStyle;

                        RowIndex++;

                        for (int c = 0; c < DecorCatalog.DecorProductsCount; c++)
                        {
                            if (DecorResultDataTable[c].Rows.Count == 0)
                                continue;

                            //вывод заказов декора в excel
                            for (int x = 0; x < DecorResultDataTable[c].Rows.Count; x++)
                            {
                                for (int y = 0; y < DecorResultDataTable[c].Columns.Count; y++)
                                {
                                    int ColumnIndex = y;

                                    //if (y == 0)
                                    //{
                                    //    ColumnIndex = y;
                                    //}
                                    //else
                                    //{
                                    //    ColumnIndex = y + 1;
                                    //}

                                    Type t = DecorResultDataTable[c].Rows[x][y].GetType();

                                    //sheet1.CreateRow(RowIndex).CreateCell(1).CellStyle = SimpleCellStyle;

                                    if (t.Name == "Decimal")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToDouble(DecorResultDataTable[c].Rows[x][y]));

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
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }
                                    if (t.Name == "Int32")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(Convert.ToInt32(DecorResultDataTable[c].Rows[x][y]));
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                    if (t.Name == "String" || t.Name == "DBNull")
                                    {
                                        cell = sheet1.CreateRow(RowIndex).CreateCell(ColumnIndex);
                                        cell.SetCellValue(DecorResultDataTable[c].Rows[x][y].ToString());
                                        cell.CellStyle = SimpleCellStyle;
                                        continue;
                                    }

                                }

                                RowIndex++;
                            }
                            RowIndex++;

                        }
                        cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Ответственный:");
                        cell.CellStyle = MainStyle;
                        cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Выполнил:");
                        cell.CellStyle = MainStyle;

                    }
                }
            }
            #endregion
        }
        #endregion

        private int MenExcelSheet1(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont,
            int RowIndex, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {
            string BatchName = string.Empty;

            DataTable DT = FrontsResultDataTable.Copy();

            DataColumn GroundUpCol = DT.Columns.Add("GroundUp", System.Type.GetType("System.Decimal"));
            DataColumn GroundDownCol = DT.Columns.Add("GroundDown", System.Type.GetType("System.Decimal"));
            DataColumn PatinaCol = DT.Columns.Add("PatinaColumn", System.Type.GetType("System.Decimal"));
            GroundUpCol.SetOrdinal(9);
            GroundDownCol.SetOrdinal(10);
            PatinaCol.SetOrdinal(11);
            DT.Columns["Square"].SetOrdinal(12);

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            Batchcell1.CellStyle = ClientNameStyle;

            HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ClientCell.CellStyle = ClientNameStyle;

            HSSFCell OrderCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказ " + MegaBatchID + ", " + BatchID + ", " + MainOrderID);
            OrderCell.CellStyle = ClientNameStyle;

            decimal Square = 0;
            int FrontsCount = 0;

            if (DT.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Гр.в.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Гр.н");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell4.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(DT);
                FrontsCount = GetCount(DT);
            }

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell20.CellStyle = cellStyle1;
            }

            return ++RowIndex;
        }

        private int MenExcelSheet2(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont,
            int RowIndex, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {
            string BatchName = string.Empty;

            DataTable DT = FrontsResultDataTable.Copy();

            DataColumn VarnishCol = DT.Columns.Add("Varnish", System.Type.GetType("System.Decimal"));
            VarnishCol.SetOrdinal(9);
            DT.Columns["Square"].SetOrdinal(10);

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            Batchcell1.CellStyle = ClientNameStyle;

            HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ClientCell.CellStyle = ClientNameStyle;

            HSSFCell OrderCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказ " + MegaBatchID + ", " + BatchID + ", " + MainOrderID);
            OrderCell.CellStyle = ClientNameStyle;

            decimal Square = 0;
            int FrontsCount = 0;

            if (DT.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Лак");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell4.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(DT);
                FrontsCount = GetCount(DT);
            }

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell20.CellStyle = cellStyle1;
            }

            return ++RowIndex;
        }

        private int WomenExcelSheet1(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont,
            int RowIndex, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {

            string BatchName = string.Empty;

            DataTable DT = FrontsResultDataTable.Copy();

            DataColumn DegreasingCol = DT.Columns.Add("Degreasing", System.Type.GetType("System.Decimal"));
            DegreasingCol.SetOrdinal(9);
            DT.Columns["Square"].SetOrdinal(10);

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            Batchcell1.CellStyle = ClientNameStyle;

            HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ClientCell.CellStyle = ClientNameStyle;

            HSSFCell OrderCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказ " + MegaBatchID + ", " + BatchID + ", " + MainOrderID);
            OrderCell.CellStyle = ClientNameStyle;

            decimal Square = 0;
            int FrontsCount = 0;

            if (DT.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Обезжиривание");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell4.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(DT);
                FrontsCount = GetCount(DT);
            }

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell20.CellStyle = cellStyle1;
            }

            return ++RowIndex;
        }

        private int WomenExcelSheet2(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont,
            int RowIndex, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {

            string BatchName = string.Empty;

            DataTable DT = FrontsResultDataTable.Copy();

            DataColumn PatinaCol = DT.Columns.Add("PatinaCol", System.Type.GetType("System.Decimal"));
            PatinaCol.SetOrdinal(9);
            DT.Columns["Square"].SetOrdinal(10);

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            Batchcell1.CellStyle = ClientNameStyle;

            HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ClientCell.CellStyle = ClientNameStyle;

            HSSFCell OrderCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказ " + MegaBatchID + ", " + BatchID + ", " + MainOrderID);
            OrderCell.CellStyle = ClientNameStyle;

            decimal Square = 0;
            int FrontsCount = 0;

            if (DT.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Протирка Патины");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell4.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(DT);
                FrontsCount = GetCount(DT);
            }

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell20.CellStyle = cellStyle1;
            }

            return ++RowIndex;
        }

        private int PackingExcelSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont,
            int RowIndex, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {
            string BatchName = string.Empty;

            DataTable DT = FrontsResultDataTable.Copy();

            DataColumn FilmCol = DT.Columns.Add("Film", System.Type.GetType("System.Decimal"));
            DataColumn PackingCol = DT.Columns.Add("Packing", System.Type.GetType("System.Decimal"));
            FilmCol.SetOrdinal(9);
            PackingCol.SetOrdinal(10);
            DT.Columns["Square"].SetOrdinal(11);

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            Batchcell1.CellStyle = ClientNameStyle;

            HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ClientCell.CellStyle = ClientNameStyle;

            HSSFCell OrderCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказ " + MegaBatchID + ", " + BatchID + ", " + MainOrderID);
            OrderCell.CellStyle = ClientNameStyle;

            decimal Square = 0;
            int FrontsCount = 0;

            if (DT.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Пленка");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Упаковка");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell4.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(DT);
                FrontsCount = GetCount(DT);
            }

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell18 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell18.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell20.CellStyle = cellStyle1;
            }

            return ++RowIndex;
        }

        private int BoringExcelSheet(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1,
            HSSFCellStyle ClientNameStyle, HSSFCellStyle HeaderStyle, HSSFFont SimpleFont, HSSFCellStyle SimpleCellStyle, HSSFFont PackNumberFont,
            int RowIndex, string ClientName,
            int MegaBatchID, int BatchID, int MainOrderID, int FactoryID)
        {
            string BatchName = string.Empty;

            DataTable DT = FrontsResultDataTable.Copy();

            DataColumn BoringCol = DT.Columns.Add("Boring", System.Type.GetType("System.Decimal"));
            BoringCol.SetOrdinal(9);
            DT.Columns["Square"].SetOrdinal(10);

            BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", Профиль";

            if (FactoryID == 2)
            {
                BatchName = "Группа №" + MegaBatchID + ", партия №" + BatchID + ", ТПС";
            }

            HSSFCell Batchcell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, BatchName);
            Batchcell1.CellStyle = ClientNameStyle;

            HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ClientCell.CellStyle = ClientNameStyle;

            HSSFCell OrderCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказ " + MegaBatchID + ", " + BatchID + ", " + MainOrderID);
            OrderCell.CellStyle = ClientNameStyle;

            decimal Square = 0;
            int FrontsCount = 0;

            if (DT.Rows.Count != 0)
            {
                int DisplayIndex = 0;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Патина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell8.CellStyle = HeaderStyle;

                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell11 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell11.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell12.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Сверление");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Квадр.");
                cell4.CellStyle = HeaderStyle;
                cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell4.CellStyle = HeaderStyle;

                RowIndex++;

                Square = GetSquare(DT);
                FrontsCount = GetCount(DT);
            }

            //вывод заказов фасадов
            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));

                        HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
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
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = SimpleCellStyle;
                        continue;
                    }
                }

                RowIndex++;

            }

            HSSFCellStyle cellStyle1 = hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            cellStyle1.SetFont(PackNumberFont);

            HSSFCell cell20 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell20.CellStyle = cellStyle1;
            HSSFCell cell19 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, FrontsCount + " шт.");
            cell19.CellStyle = cellStyle1;

            if (Square > 0)
            {
                HSSFCell cell21 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2,
                    Decimal.Round(Square, 3, MidpointRounding.AwayFromZero) + " м.кв.");

                cell21.CellStyle = cellStyle1;
            }

            return ++RowIndex;
        }

        //private string ReadReportFilePath(string FileName)
        //{
        //    string ReportFilePath = string.Empty;

        //    using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName, Encoding.Default))
        //    {
        //        ReportFilePath = sr.ReadToEnd();
        //    }
        //    return ReportFilePath;
        //}
    }




}
