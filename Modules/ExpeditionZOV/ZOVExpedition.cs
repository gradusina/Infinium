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
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.ZOV.Expedition
{
    public class ZOVExpeditionFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentMainOrder = 1;
        int CurrentPackNumber = 1;
        //48215 47516
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

        public ZOVExpeditionFrontsOrders(
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM FrontsOrders", ConnectionStrings.ZOVOrdersConnectionString))
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
            //MainOrdersFrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
            //MainOrdersFrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
            FrontsOrdersDataGrid.Columns["Debt"].Visible = false;
            //MainOrdersFrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Visible = false;
            FrontsOrdersDataGrid.Columns["PackNumber"].Visible = false;

            if (FrontsOrdersDataGrid.Columns.Contains("AlHandsSize"))
                FrontsOrdersDataGrid.Columns["AlHandsSize"].Visible = false;
            if (FrontsOrdersDataGrid.Columns.Contains("FrontDrillTypeID"))
                FrontsOrdersDataGrid.Columns["FrontDrillTypeID"].Visible = false;

            if (!Security.PriceAccess)
            {
                FrontsOrdersDataGrid.Columns["FrontPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["InsetPrice"].Visible = false;
                FrontsOrdersDataGrid.Columns["Cost"].Visible = false;
            }

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
            FrontsOrdersDataGrid.Columns["FrontPrice"].HeaderText = "Цена за\r\nфасад";
            FrontsOrdersDataGrid.Columns["InsetPrice"].HeaderText = "Цена за\r\nвставку";
            FrontsOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";

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
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["FrontPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["InsetPrice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            FrontsOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["Notes"].MinimumWidth = 175;

            FrontsOrdersDataGrid.Columns["CupboardString"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsOrdersDataGrid.Columns["CupboardString"].MinimumWidth = 165;
            FrontsOrdersDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["PackNumber"].Width = 110;
            FrontsOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["MainOrderID"].Width = 110;
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

        public bool FilterByMainOrder(int MainOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return FrontsOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            OriginalFrontsOrdersDataTable = FrontsOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0 " + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                            " WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(FrontsOrdersDataTable);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();
            FrontsOrdersBindingSource.MoveFirst();
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public bool FilterByMegaOrder(int MegaOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return FrontsOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;
            string FactoryFilter = string.Empty;
            string MainOrdersFactoryFilter = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter = " AND FactoryID = " + FactoryID;
                MainOrdersFactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            OriginalFrontsOrdersDataTable = FrontsOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" +
                " AND ProductType = 0 " + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter(
                            "SELECT * FROM FrontsOrders" +
                            " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                            " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" + FactoryFilter,
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            fDA.Fill(FrontsOrdersDataTable);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();
            FrontsOrdersDataTable.DefaultView.Sort = "MainOrderID, PackNumber";
            FrontsOrdersBindingSource.MoveFirst();
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        public void SetColor(int MainOrder, int PackNum)
        {
            for (int i = 0; i < FrontsOrdersDataGrid.Rows.Count; i++)
            {
                if (FrontsOrdersDataGrid.Rows[i].Cells["PackNumber"].Value != DBNull.Value &&
                    FrontsOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value != DBNull.Value)
                {
                    int FrontOrderPackNumber = Convert.ToInt32(FrontsOrdersDataGrid.Rows[i].Cells["PackNumber"].Value);
                    int MainOrderID = Convert.ToInt32(FrontsOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                    CurrentMainOrder = MainOrder;
                    CurrentPackNumber = PackNum;

                    if (FrontOrderPackNumber == CurrentPackNumber && MainOrderID == CurrentMainOrder)
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
        }

        public void MoveToFrontOrder(int MainOrderID, int PackNumber)
        {
            DataRow[] Rows = FrontsOrdersDataTable.Select("PackNumber = " + PackNumber + " AND MainOrderID = " + MainOrderID);
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

            if (Convert.ToInt32(FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) == CurrentPackNumber &&
                Convert.ToInt32(FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["MainOrderID"].Value) == CurrentMainOrder)
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
            if (Convert.ToInt32(FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) != CurrentPackNumber ||
                Convert.ToInt32(FrontsOrdersDataGrid.Rows[e.RowIndex].Cells["MainOrderID"].Value) != CurrentMainOrder)
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







    public class ZOVExpeditionDecorOrders
    {
        int CurrentPackNumber = 1;
        int CurrentMainOrder = 1;

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
        public ZOVExpeditionDecorOrders(ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid)
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

            //ItemColumn
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
            MainOrdersDecorOrdersDataGrid.Columns["DecorConfigID"].Visible = false;
            //MainOrdersDecorOrdersDataGrid.Columns["Cost"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Debt"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ItemWeight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["Weight"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].Visible = false;

            //русские названия полей

            MainOrdersDecorOrdersDataGrid.Columns["Price"].HeaderText = "Цена";
            MainOrdersDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
            //MainOrdersDecorOrdersDataGrid.Columns["PackNumber"].HeaderText = "  Номер\r\nупаковки";

            if (!Security.PriceAccess)
            {
                MainOrdersDecorOrdersDataGrid.Columns["Price"].Visible = false;
                MainOrdersDecorOrdersDataGrid.Columns["Cost"].Visible = false;
            }

            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ProductColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ItemColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["ColorsColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Length"].Width = 110;
            MainOrdersDecorOrdersDataGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Height"].Width = 110;
            MainOrdersDecorOrdersDataGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Width"].Width = 110;
            MainOrdersDecorOrdersDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDecorOrdersDataGrid.Columns["Count"].Width = 110;
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].MinimumWidth = 110;


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

            //MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].Width = 120;

            //MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].DisplayIndex = 0;
        }

        public bool FilterByMainOrder(int MainOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return DecorOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = DecorOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1) AND PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1 " + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                            ConnectionStrings.ZOVOrdersConnectionString))
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
            DecorOrdersBindingSource.MoveFirst();
            return DecorOrdersDataTable.Rows.Count > 0;
        }

        public bool FilterByMegaOrder(int MegaOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return DecorOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;
            string FactoryFilter = string.Empty;
            string MainOrdersFactoryFilter = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter = " AND FactoryID = " + FactoryID;
                MainOrdersFactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = DecorOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" +
                " AND ProductType = 1 " + FactoryFilter + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                            " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" + FactoryFilter,
                            ConnectionStrings.ZOVOrdersConnectionString))
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
            DecorOrdersDataTable.DefaultView.Sort = "MainOrderID, PackNumber";
            DecorOrdersBindingSource.MoveFirst();
            return DecorOrdersDataTable.Rows.Count > 0;
        }

        public void SetColor(int MainOrder, int PackNumber)
        {
            for (int i = 0; i < MainOrdersDecorOrdersDataGrid.Rows.Count; i++)
            {
                if (MainOrdersDecorOrdersDataGrid.Rows[i].Cells["PackNumber"].Value != DBNull.Value &&
                    MainOrdersDecorOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value != DBNull.Value)
                {
                    int DecorOrderPackNumber = Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[i].Cells["PackNumber"].Value);
                    int MainOrderID = Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[i].Cells["MainOrderID"].Value);

                    CurrentMainOrder = MainOrder;
                    CurrentPackNumber = PackNumber;

                    if (DecorOrderPackNumber == CurrentPackNumber && MainOrderID == CurrentMainOrder)
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
        }

        public void MoveToDecorOrder(int MainOrderID, int PackNumber)
        {
            DataRow[] Rows = DecorOrdersDataTable.Select("PackNumber = " + PackNumber + " AND MainOrderID = " + MainOrderID);
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

            if (Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) == CurrentPackNumber &&
                Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["MainOrderID"].Value) == CurrentMainOrder)
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
            if (Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["PackNumber"].Value) != CurrentPackNumber ||
                Convert.ToInt32(MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells["MainOrderID"].Value) != CurrentMainOrder)
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







    public class ZOVExpeditionManager
    {
        private int CurrentMainOrderID = -1;
        private int CurrentMegaOrderID = -1;

        public ZOVExpeditionFrontsOrders PackedMainOrdersFrontsOrders = null;
        public ZOVExpeditionDecorOrders PackedMainOrdersDecorOrders = null;

        public PercentageDataGrid MainOrdersDGV = null;
        public PercentageDataGrid MegaOrdersDGV = null;
        public PercentageDataGrid PackagesDGV = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        private DataTable DocNumbersDT = null;
        private DataTable ClientsDataTable = null;
        public DataTable MainOrdersDT = null;
        private DataTable FactoryTypesDT = null;
        private DataTable DebtTypesFullDT = null;
        private DataTable PriceTypesDT = null;
        private DataTable PackAllocStatusesDT = null;
        private DataTable ProductionStatusesDT = null;
        private DataTable StorageStatusesDT = null;
        private DataTable DispatchStatusesDT = null;
        public DataTable AllPackagesDT = null;
        public DataTable PackagesDT = null;
        private DataTable PackageStatusesDT = null;
        private DataTable PackMainOrdersDT = null;
        private DataTable PackMegaOrdersDT = null;
        private DataTable StoreMainOrdersDT = null;
        private DataTable StoreMegaOrdersDT = null;
        private DataTable ExpMainOrdersDT = null;
        private DataTable ExpMegaOrdersDT = null;
        private DataTable DispMainOrdersDT = null;
        private DataTable DispMegaOrdersDT = null;

        public DataTable MegaOrdersDT = null;
        private DataTable BatchDetailsDT = null;
        private DataTable SearchMainOrdersDT = null;
        private DataTable UsersDataTable = null;

        public BindingSource DocNumbersBS = null;
        public BindingSource ClientsBS = null;
        public BindingSource MainOrdersBS = null;
        public BindingSource FactoryTypesBS = null;
        public BindingSource MegaOrdersBS = null;
        public BindingSource PackagesBS = null;
        public BindingSource PackageStatusesBS = null;
        public BindingSource SearchDocNumberBS = null;
        public BindingSource SearchPartDocNumberBS = null;

        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn PriceTypeColumn = null;
        private DataGridViewComboBoxColumn DebtTypeColumn = null;
        private DataGridViewComboBoxColumn MainProfilPackAllocStatusColumn = null;
        private DataGridViewComboBoxColumn MainTPSPackAllocStatusColumn = null;
        private DataGridViewComboBoxColumn MegaProfilPackAllocStatusColumn = null;
        private DataGridViewComboBoxColumn MegaTPSPackAllocStatusColumn = null;
        private DataGridViewComboBoxColumn PackageStatusesColumn = null;
        private DataGridViewComboBoxColumn DispatchStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilProductionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilStorageStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn TPSProductionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSStorageStatusColumn = null;
        private DataGridViewComboBoxColumn TPSDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn PackUsersColumn = null;
        private DataGridViewComboBoxColumn StoreUsersColumn = null;
        private DataGridViewComboBoxColumn ExpUsersColumn = null;
        private DataGridViewComboBoxColumn DispUsersColumn = null;

        public ZOVExpeditionManager(
            ref PercentageDataGrid tMegaOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDataGrid,
            ref PercentageDataGrid tPackagesDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl)
        {
            MainOrdersDGV = tMainOrdersDataGrid;
            MegaOrdersDGV = tMegaOrdersDataGrid;
            PackagesDGV = tPackagesDataGrid;
            OrdersTabControl = tOrdersTabControl;

            PackedMainOrdersFrontsOrders = new ZOVExpeditionFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);

            PackedMainOrdersDecorOrders = new ZOVExpeditionDecorOrders(ref tMainOrdersDecorOrdersDataGrid);

            Initialize();
        }

        #region Initialize
        private void Create()
        {
            AllPackagesDT = new DataTable();
            DocNumbersDT = new DataTable();
            MainOrdersDT = new DataTable();
            MegaOrdersDT = new DataTable();
            PackagesDT = new DataTable();
            FactoryTypesDT = new DataTable();
            PackageStatusesDT = new DataTable();
            PackMegaOrdersDT = new DataTable();
            PackMainOrdersDT = new DataTable();
            StoreMainOrdersDT = new DataTable();
            StoreMegaOrdersDT = new DataTable();
            ExpMainOrdersDT = new DataTable();
            ExpMegaOrdersDT = new DataTable();
            DispMegaOrdersDT = new DataTable();
            DispMainOrdersDT = new DataTable();
            BatchDetailsDT = new DataTable();
            SearchMainOrdersDT = new DataTable();

            ProductionStatusesDT = new DataTable();
            StorageStatusesDT = new DataTable();
            DispatchStatusesDT = new DataTable();

            DocNumbersBS = new BindingSource();
            ClientsBS = new BindingSource();
            MainOrdersBS = new BindingSource();
            MegaOrdersBS = new BindingSource();

            PackagesBS = new BindingSource();
            FactoryTypesBS = new BindingSource();
            PackageStatusesBS = new BindingSource();
            SearchDocNumberBS = new BindingSource();
            SearchPartDocNumberBS = new BindingSource();
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

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT(DocNumber) FROM MainOrders" +
                   " ORDER BY DocNumber",
                   ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 MainOrders.*, ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients" +
                " ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " ORDER BY ClientName",
               ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDT);
            }
            MainOrdersDT.Columns.Add(new DataColumn("CheckBoxColumn", Type.GetType("System.Boolean")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilPackedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilPackPercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilStoreCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilStorePercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilExpCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilExpPercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilDispatchedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilDispPercentage", Type.GetType("System.Int32")));

            MainOrdersDT.Columns.Add(new DataColumn("TPSPackedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSPackPercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSStoreCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSStorePercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSExpCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSExpPercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSDispatchedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSDispPercentage", Type.GetType("System.Int32")));
            MainOrdersDT.Columns.Add(new DataColumn("BatchNumber", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaOrders WHERE MegaOrderID > 11025 OR MegaOrderID = 0 ORDER BY DispatchDate",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MegaOrdersDT);
            }
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilPackedCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilPackPercentage", Type.GetType("System.Int32")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilStoreCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilStorePercentage", Type.GetType("System.Int32")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilExpCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilExpPercentage", Type.GetType("System.Int32")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilDispatchedCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("ProfilDispPercentage", Type.GetType("System.Int32")));

            MegaOrdersDT.Columns.Add(new DataColumn("TPSPackedCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSPackPercentage", Type.GetType("System.Int32")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSStoreCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSStorePercentage", Type.GetType("System.Int32")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSExpCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSExpPercentage", Type.GetType("System.Int32")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSDispatchedCount", Type.GetType("System.String")));
            MegaOrdersDT.Columns.Add(new DataColumn("TPSDispPercentage", Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 Packages.*, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(AllPackagesDT);
                DA.Fill(PackagesDT);
                PackagesDT.Columns.Add(new DataColumn("CheckBoxColumn", Type.GetType("System.Boolean")));
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Factory", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryTypesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDT);
            }
            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID <> 0)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PackMegaOrdersDT.Clear();
                DA.Fill(PackMegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages" +
                " WHERE PackageStatusID <> 0" +
                " GROUP BY MainOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PackMainOrdersDT.Clear();
                DA.Fill(PackMainOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 2)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMegaOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =2 " +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMainOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 4)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                ExpMegaOrdersDT.Clear();
                DA.Fill(ExpMegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =4 " +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                ExpMainOrdersDT.Clear();
                DA.Fill(ExpMainOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 3)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DispMegaOrdersDT.Clear();
                DA.Fill(DispMegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages" +
                " WHERE PackageStatusID = 3" +
                " GROUP BY MainOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DispMainOrdersDT.Clear();
                DA.Fill(DispMainOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID FROM BatchDetails" +
                   " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID",
                   ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDT);
            }

            PriceTypesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PriceTypes",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(PriceTypesDT);
            }

            DebtTypesFullDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtTypesFullDT);
            }
            PackAllocStatusesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackAllocStatuses",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackAllocStatusesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, DocNumber FROM MainOrders",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SearchMainOrdersDT);
            }

            UsersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM Users WHERE Fired <> 1 ", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }
        }

        private void FillMainPercentageColumn()
        {
            int MainOrderProfilPackCount = 0;
            int MainOrderProfilStoreCount = 0;
            int MainOrderProfilDispCount = 0;
            int MainOrderProfilExpCount = 0;
            int MainOrderProfilAllCount = 0;

            int ProfilPackPercentage = 0;
            int ProfilStorePercentage = 0;
            int ProfilExpPercentage = 0;
            int ProfilDispPercentage = 0;

            decimal ProfilPackProgressVal = 0;
            decimal ProfilStoreProgressVal = 0;
            decimal ProfilExpProgressVal = 0;
            decimal ProfilDispProgressVal = 0;

            int MainOrderTPSPackCount = 0;
            int MainOrderTPSStoreCount = 0;
            int MainOrderTPSDispCount = 0;
            int MainOrderTPSExpCount = 0;
            int MainOrderTPSAllCount = 0;

            int TPSPackPercentage = 0;
            int TPSStorePercentage = 0;
            int TPSExpPercentage = 0;
            int TPSDispPercentage = 0;

            decimal TPSPackProgressVal = 0;
            decimal TPSStoreProgressVal = 0;
            decimal TPSExpProgressVal = 0;
            decimal TPSDispProgressVal = 0;

            decimal d1 = 0;
            decimal d2 = 0;
            decimal d3 = 0;
            decimal d4 = 0;
            decimal d5 = 0;
            decimal d6 = 0;
            decimal d7 = 0;
            decimal d8 = 0;

            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                int MainOrderID = Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]);

                MainOrderProfilPackCount = GetMainOrderPackCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilStoreCount = GetMainOrderStoreCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilExpCount = GetMainOrderExpCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilDispCount = GetMainOrderDispCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilAllCount = Convert.ToInt32(MainOrdersDT.Rows[i]["ProfilPackCount"]);

                MainOrderTPSPackCount = GetMainOrderPackCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSStoreCount = GetMainOrderStoreCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSExpCount = GetMainOrderExpCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSDispCount = GetMainOrderDispCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSAllCount = Convert.ToInt32(MainOrdersDT.Rows[i]["TPSPackCount"]);

                if (MainOrderTPSAllCount == 0 && MainOrderTPSDispCount > 0)
                    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                if (MainOrderProfilAllCount == 0 && MainOrderProfilDispCount > 0)
                    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                ProfilPackProgressVal = 0;
                ProfilStoreProgressVal = 0;
                ProfilExpProgressVal = 0;
                ProfilDispProgressVal = 0;

                TPSPackProgressVal = 0;
                TPSStoreProgressVal = 0;
                TPSExpProgressVal = 0;
                TPSDispProgressVal = 0;

                if (MainOrderProfilAllCount > 0)
                    ProfilPackProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderProfilPackCount) / Convert.ToDecimal(MainOrderProfilAllCount));

                if (MainOrderProfilAllCount > 0)
                    ProfilStoreProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderProfilStoreCount) / Convert.ToDecimal(MainOrderProfilAllCount));

                if (MainOrderProfilAllCount > 0)
                    ProfilExpProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderProfilExpCount) / Convert.ToDecimal(MainOrderProfilAllCount));

                if (MainOrderProfilAllCount > 0)
                    ProfilDispProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderProfilDispCount) / Convert.ToDecimal(MainOrderProfilAllCount));

                if (MainOrderTPSAllCount > 0)
                    TPSPackProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderTPSPackCount) / Convert.ToDecimal(MainOrderTPSAllCount));

                if (MainOrderTPSAllCount > 0)
                    TPSStoreProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderTPSStoreCount) / Convert.ToDecimal(MainOrderTPSAllCount));

                if (MainOrderTPSAllCount > 0)
                    TPSExpProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderTPSExpCount) / Convert.ToDecimal(MainOrderTPSAllCount));

                if (MainOrderTPSAllCount > 0)
                    TPSDispProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderTPSDispCount) / Convert.ToDecimal(MainOrderTPSAllCount));

                d1 = ProfilPackProgressVal * 100;
                d2 = ProfilStoreProgressVal * 100;
                d7 = ProfilExpProgressVal * 100;
                d3 = ProfilDispProgressVal * 100;
                d4 = TPSPackProgressVal * 100;
                d5 = TPSStoreProgressVal * 100;
                d8 = TPSExpProgressVal * 100;
                d6 = TPSDispProgressVal * 100;

                ProfilPackPercentage = Convert.ToInt32(Math.Truncate(d1));
                ProfilStorePercentage = Convert.ToInt32(Math.Truncate(d2));
                ProfilExpPercentage = Convert.ToInt32(Math.Truncate(d7));
                ProfilDispPercentage = Convert.ToInt32(Math.Truncate(d3));

                TPSPackPercentage = Convert.ToInt32(Math.Truncate(d4));
                TPSStorePercentage = Convert.ToInt32(Math.Truncate(d5));
                TPSExpPercentage = Convert.ToInt32(Math.Truncate(d8));
                TPSDispPercentage = Convert.ToInt32(Math.Truncate(d6));

                MainOrdersDT.Rows[i]["ProfilPackedCount"] = MainOrderProfilPackCount + " / " + MainOrderProfilAllCount;
                MainOrdersDT.Rows[i]["ProfilPackPercentage"] = ProfilPackPercentage;
                MainOrdersDT.Rows[i]["ProfilStoreCount"] = MainOrderProfilStoreCount + " / " + MainOrderProfilAllCount;
                MainOrdersDT.Rows[i]["ProfilStorePercentage"] = ProfilStorePercentage;
                MainOrdersDT.Rows[i]["ProfilExpCount"] = MainOrderProfilExpCount + " / " + MainOrderProfilAllCount;
                MainOrdersDT.Rows[i]["ProfilExpPercentage"] = ProfilExpPercentage;
                MainOrdersDT.Rows[i]["ProfilDispatchedCount"] = MainOrderProfilDispCount + " / " + MainOrderProfilAllCount;
                MainOrdersDT.Rows[i]["ProfilDispPercentage"] = ProfilDispPercentage;

                MainOrdersDT.Rows[i]["TPSPackedCount"] = MainOrderTPSPackCount + " / " + MainOrderTPSAllCount;
                MainOrdersDT.Rows[i]["TPSPackPercentage"] = TPSPackPercentage;
                MainOrdersDT.Rows[i]["TPSStoreCount"] = MainOrderTPSStoreCount + " / " + MainOrderTPSAllCount;
                MainOrdersDT.Rows[i]["TPSStorePercentage"] = TPSStorePercentage;
                MainOrdersDT.Rows[i]["TPSExpCount"] = MainOrderTPSExpCount + " / " + MainOrderTPSAllCount;
                MainOrdersDT.Rows[i]["TPSExpPercentage"] = TPSExpPercentage;
                MainOrdersDT.Rows[i]["TPSDispatchedCount"] = MainOrderTPSDispCount + " / " + MainOrderTPSAllCount;
                MainOrdersDT.Rows[i]["TPSDispPercentage"] = TPSDispPercentage;
            }
        }

        private void FillMegaPercentageColumn()
        {
            int MegaOrderProfilPackCount = 0;
            int MegaOrderProfilStoreCount = 0;
            int MegaOrderProfilExpCount = 0;
            int MegaOrderProfilDispCount = 0;
            int MegaOrderProfilAllCount = 0;

            int ProfilPackPercentage = 0;
            int ProfilStorePercentage = 0;
            int ProfilExpPercentage = 0;
            int ProfilDispPercentage = 0;

            decimal ProfilPackProgressVal = 0;
            decimal ProfilStoreProgressVal = 0;
            decimal ProfilExpProgressVal = 0;
            decimal ProfilDispProgressVal = 0;

            int MegaOrderTPSPackCount = 0;
            int MegaOrderTPSStoreCount = 0;
            int MegaOrderTPSExpCount = 0;
            int MegaOrderTPSDispCount = 0;
            int MegaOrderTPSAllCount = 0;

            int TPSPackPercentage = 0;
            int TPSStorePercentage = 0;
            int TPSExpPercentage = 0;
            int TPSDispPercentage = 0;

            decimal TPSPackProgressVal = 0;
            decimal TPSStoreProgressVal = 0;
            decimal TPSExpProgressVal = 0;
            decimal TPSDispProgressVal = 0;

            decimal d1 = 0;
            decimal d2 = 0;
            decimal d3 = 0;
            decimal d4 = 0;
            decimal d5 = 0;
            decimal d6 = 0;
            decimal d7 = 0;
            decimal d8 = 0;

            for (int i = 0; i < MegaOrdersDT.Rows.Count; i++)
            {
                int MegaOrderID = Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]);

                MegaOrderProfilPackCount = GetMegaOrderPackCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilStoreCount = GetMegaOrderStoreCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilExpCount = GetMegaOrderExpCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilDispCount = GetMegaOrderDispCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilAllCount = Convert.ToInt32(MegaOrdersDT.Rows[i]["ProfilPackCount"]);

                MegaOrderTPSPackCount = GetMegaOrderPackCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSStoreCount = GetMegaOrderStoreCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSExpCount = GetMegaOrderExpCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSDispCount = GetMegaOrderDispCount(Convert.ToInt32(MegaOrdersDT.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSAllCount = Convert.ToInt32(MegaOrdersDT.Rows[i]["TPSPackCount"]);

                //if (MegaOrderTPSAllCount == 0 && MegaOrderTPSDispCount > 0)
                //    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                //if (MegaOrderProfilAllCount == 0 && MegaOrderProfilDispCount > 0)
                //    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                ProfilPackProgressVal = 0;
                ProfilStoreProgressVal = 0;
                ProfilExpProgressVal = 0;
                ProfilDispProgressVal = 0;

                TPSPackProgressVal = 0;
                TPSStoreProgressVal = 0;
                TPSExpProgressVal = 0;
                TPSDispProgressVal = 0;

                if (MegaOrderProfilAllCount > 0)
                    ProfilPackProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderProfilPackCount) / Convert.ToDecimal(MegaOrderProfilAllCount));

                if (MegaOrderProfilAllCount > 0)
                    ProfilStoreProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderProfilStoreCount) / Convert.ToDecimal(MegaOrderProfilAllCount));

                if (MegaOrderProfilAllCount > 0)
                    ProfilExpProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderProfilExpCount) / Convert.ToDecimal(MegaOrderProfilAllCount));

                if (MegaOrderProfilAllCount > 0)
                    ProfilDispProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderProfilDispCount) / Convert.ToDecimal(MegaOrderProfilAllCount));

                if (MegaOrderTPSAllCount > 0)
                    TPSPackProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderTPSPackCount) / Convert.ToDecimal(MegaOrderTPSAllCount));

                if (MegaOrderTPSAllCount > 0)
                    TPSStoreProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderTPSStoreCount) / Convert.ToDecimal(MegaOrderTPSAllCount));

                if (MegaOrderTPSAllCount > 0)
                    TPSExpProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderTPSExpCount) / Convert.ToDecimal(MegaOrderTPSAllCount));

                if (MegaOrderTPSAllCount > 0)
                    TPSDispProgressVal = Convert.ToDecimal(Convert.ToDecimal(MegaOrderTPSDispCount) / Convert.ToDecimal(MegaOrderTPSAllCount));

                d1 = ProfilPackProgressVal * 100;
                d2 = ProfilStoreProgressVal * 100;
                d3 = ProfilDispProgressVal * 100;
                d4 = TPSPackProgressVal * 100;
                d5 = TPSStoreProgressVal * 100;
                d6 = TPSDispProgressVal * 100;
                d7 = ProfilExpProgressVal * 100;
                d8 = TPSExpProgressVal * 100;

                ProfilPackPercentage = Convert.ToInt32(Math.Truncate(d1));
                ProfilStorePercentage = Convert.ToInt32(Math.Truncate(d2));
                ProfilExpPercentage = Convert.ToInt32(Math.Truncate(d7));
                ProfilDispPercentage = Convert.ToInt32(Math.Truncate(d3));

                TPSPackPercentage = Convert.ToInt32(Math.Truncate(d4));
                TPSStorePercentage = Convert.ToInt32(Math.Truncate(d5));
                TPSExpPercentage = Convert.ToInt32(Math.Truncate(d8));
                TPSDispPercentage = Convert.ToInt32(Math.Truncate(d6));

                MegaOrdersDT.Rows[i]["ProfilPackedCount"] = MegaOrderProfilPackCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDT.Rows[i]["ProfilPackPercentage"] = ProfilPackPercentage;
                MegaOrdersDT.Rows[i]["ProfilStoreCount"] = MegaOrderProfilStoreCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDT.Rows[i]["ProfilStorePercentage"] = ProfilStorePercentage;
                MegaOrdersDT.Rows[i]["ProfilExpCount"] = MegaOrderProfilExpCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDT.Rows[i]["ProfilExpPercentage"] = ProfilExpPercentage;
                MegaOrdersDT.Rows[i]["ProfilDispatchedCount"] = MegaOrderProfilDispCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDT.Rows[i]["ProfilDispPercentage"] = ProfilDispPercentage;

                MegaOrdersDT.Rows[i]["TPSPackedCount"] = MegaOrderTPSPackCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDT.Rows[i]["TPSPackPercentage"] = TPSPackPercentage;
                MegaOrdersDT.Rows[i]["TPSStoreCount"] = MegaOrderTPSStoreCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDT.Rows[i]["TPSStorePercentage"] = TPSStorePercentage;
                MegaOrdersDT.Rows[i]["TPSExpCount"] = MegaOrderTPSExpCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDT.Rows[i]["TPSExpPercentage"] = TPSExpPercentage;
                MegaOrdersDT.Rows[i]["TPSDispatchedCount"] = MegaOrderTPSDispCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDT.Rows[i]["TPSDispPercentage"] = TPSDispPercentage;
            }
            //MegaOrdersBindingSource.MoveLast();
        }

        private void FillBatchNumber()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int MainOrderID = -1;
            string BatchNumber = string.Empty;
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                MainOrderID = Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]);
                BatchNumber = GetBatchNumber(MainOrderID);
                if (BatchNumber.Length < 1)
                {
                    if (Convert.ToInt32(MainOrdersDT.Rows[i]["DebtTypeID"]) != 0)
                    {
                        MainOrderID = -1;
                        string DebtDocNumber = MainOrdersDT.Rows[i]["DebtDocNumber"].ToString();
                        string DocNumber = MainOrdersDT.Rows[i]["DocNumber"].ToString();

                        using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                            " WHERE DocNumber = '" + DebtDocNumber + "'",
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            using (DataTable DT = new DataTable())
                            {
                                DA.Fill(DT);

                                if (DT.Rows.Count > 0)
                                {
                                    MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                                    MainOrdersDT.Rows[i]["BatchNumber"] = GetBatchNumber(MainOrderID);
                                    continue;
                                }
                            }
                        }

                    }
                }
                MainOrdersDT.Rows[i]["BatchNumber"] = BatchNumber;
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        private void Binding()
        {
            ClientsBS.DataSource = ClientsDataTable;

            MegaOrdersBS.DataSource = MegaOrdersDT;
            MainOrdersBS.DataSource = MainOrdersDT;
            PackagesBS.DataSource = PackagesDT;
            FactoryTypesBS.DataSource = FactoryTypesDT;
            MainOrdersDGV.DataSource = MainOrdersBS;
            MegaOrdersDGV.DataSource = MegaOrdersBS;
            PackagesDGV.DataSource = PackagesBS;

            PackageStatusesBS.DataSource = PackageStatusesDT;

            DocNumbersBS.DataSource = DocNumbersDT;

            SearchDocNumberBS.DataSource = SearchMainOrdersDT;
            SearchPartDocNumberBS.DataSource = new DataView(SearchMainOrdersDT);
        }

        private void CreateColumns()
        {
            DispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DispatchStatusColumn",
                HeaderText = "Статус\r\nотгрузки",
                DataPropertyName = "DispatchStatusID",
                DataSource = new DataView(DispatchStatusesDT),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilProductionStatusColumn",
                HeaderText = "Пр-во\n\rПрофиль",
                DataPropertyName = "ProfilProductionStatusID",
                DataSource = new DataView(ProductionStatusesDT),
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
                DataSource = new DataView(StorageStatusesDT),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ProfilDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nПрофиль",
                DataPropertyName = "ProfilDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDT),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSProductionStatusColumn",
                HeaderText = "Пр-во\n\rТПС",
                DataPropertyName = "TPSProductionStatusID",
                DataSource = new DataView(ProductionStatusesDT),
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
                DataSource = new DataView(StorageStatusesDT),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            TPSDispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSDispatchStatusColumn",
                HeaderText = "Отгрузка\r\nТПС",
                DataPropertyName = "TPSDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDT),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            PackageStatusesColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PackageStatusesColumn",
                HeaderText = "Статус",
                DataPropertyName = "PackageStatusID",
                DataSource = PackageStatusesBS,
                ValueMember = "PackageStatusID",
                DisplayMember = "PackageStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            PackUsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PackUsersColumn",
                HeaderText = "Упаковал",
                DataPropertyName = "PackUserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "ShortName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40,
                Visible = false
            };
            StoreUsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "StoreUsersColumn",
                HeaderText = "Принял\r\nна склад",
                DataPropertyName = "StoreUserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "ShortName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40,
                Visible = false
            };
            ExpUsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ExpUsersColumn",
                HeaderText = "Принял\r\nна экс-цию",
                DataPropertyName = "ExpUserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "ShortName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40,
                Visible = false
            };
            DispUsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DispUsersColumn",
                HeaderText = "Отгрузил",
                DataPropertyName = "DispUserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "ShortName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40,
                Visible = false
            };
            FactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryTypeColumn",
                HeaderText = "Тип\r\nпроизводства",
                DataPropertyName = "FactoryID",
                DataSource = FactoryTypesBS,
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            PriceTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PriceTypeColumn",
                HeaderText = "Тип\r\nпрайса",
                DataPropertyName = "PriceTypeID",
                DataSource = new DataView(PriceTypesDT),
                ValueMember = "PriceTypeID",
                DisplayMember = "PriceTypeRus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            DebtTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DebtTypeColumn",
                HeaderText = "Долг",
                DataPropertyName = "DebtTypeID",
                DataSource = new DataView(DebtTypesFullDT),
                ValueMember = "DebtTypeID",
                DisplayMember = "DebtType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MainProfilPackAllocStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilPackAllocStatusColumn",
                HeaderText = "Упаковано\r\nПрофиль",
                DataPropertyName = "ProfilPackAllocStatusID",
                DataSource = new DataView(PackAllocStatusesDT),
                ValueMember = "PackAllocStatusID",
                DisplayMember = "PackAllocStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MainTPSPackAllocStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSPackAllocStatusColumn",
                HeaderText = "Упаковано\r\nТПС",
                DataPropertyName = "TPSPackAllocStatusID",
                DataSource = new DataView(PackAllocStatusesDT),
                ValueMember = "PackAllocStatusID",
                DisplayMember = "PackAllocStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MegaProfilPackAllocStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProfilPackAllocStatusColumn",
                HeaderText = "Упаковано\r\nПрофиль",
                DataPropertyName = "ProfilPackAllocStatusID",
                DataSource = new DataView(PackAllocStatusesDT),
                ValueMember = "PackAllocStatusID",
                DisplayMember = "PackAllocStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MegaTPSPackAllocStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TPSPackAllocStatusColumn",
                HeaderText = "Упаковано\r\nТПС",
                DataPropertyName = "TPSPackAllocStatusID",
                DataSource = new DataView(PackAllocStatusesDT),
                ValueMember = "PackAllocStatusID",
                DisplayMember = "PackAllocStatus",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            MegaOrdersDGV.Columns.Add(DispatchStatusColumn);
            MegaOrdersDGV.Columns.Add(MegaProfilPackAllocStatusColumn);
            MegaOrdersDGV.Columns.Add(MegaTPSPackAllocStatusColumn);

            MainOrdersDGV.Columns.Add(ProfilProductionStatusColumn);
            MainOrdersDGV.Columns.Add(ProfilStorageStatusColumn);
            MainOrdersDGV.Columns.Add(ProfilDispatchStatusColumn);
            MainOrdersDGV.Columns.Add(TPSProductionStatusColumn);
            MainOrdersDGV.Columns.Add(TPSStorageStatusColumn);
            MainOrdersDGV.Columns.Add(TPSDispatchStatusColumn);
            MainOrdersDGV.Columns.Add(FactoryTypeColumn);
            MainOrdersDGV.Columns.Add(PriceTypeColumn);
            MainOrdersDGV.Columns.Add(DebtTypeColumn);
            MainOrdersDGV.Columns.Add(MainProfilPackAllocStatusColumn);
            MainOrdersDGV.Columns.Add(MainTPSPackAllocStatusColumn);

            PackagesDGV.Columns.Add(PackageStatusesColumn);
            PackagesDGV.Columns.Add(PackUsersColumn);
            PackagesDGV.Columns.Add(StoreUsersColumn);
            PackagesDGV.Columns.Add(ExpUsersColumn);
            PackagesDGV.Columns.Add(DispUsersColumn);
        }

        public void ShowPackUsersColumn(bool bShow)
        {
            if (PackagesDGV.Columns.Contains("PackUsersColumn"))
                PackagesDGV.Columns["PackUsersColumn"].Visible = bShow;
        }

        public void ShowStoreUsersColumn(bool bShow)
        {
            if (PackagesDGV.Columns.Contains("StoreUsersColumn"))
                PackagesDGV.Columns["StoreUsersColumn"].Visible = bShow;
        }

        public void ShowExpUsersColumn(bool bShow)
        {
            if (PackagesDGV.Columns.Contains("ExpUsersColumn"))
                PackagesDGV.Columns["ExpUsersColumn"].Visible = bShow;
        }

        public void ShowDispUsersColumn(bool bShow)
        {
            if (PackagesDGV.Columns.Contains("DispUsersColumn"))
                PackagesDGV.Columns["DispUsersColumn"].Visible = bShow;
        }

        private void ShowColumns(int FactoryID)
        {
            if (FactoryID == 0)
            {
                MainOrdersDGV.Columns["ProfilProductionStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["ProfilStorageStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["ProfilDispatchStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["ProfilPackPercentage"].Visible = true;
                MainOrdersDGV.Columns["ProfilPackedCount"].Visible = true;
                MainOrdersDGV.Columns["ProfilExpPercentage"].Visible = true;
                MainOrdersDGV.Columns["ProfilExpCount"].Visible = true;
                MainOrdersDGV.Columns["ProfilDispPercentage"].Visible = true;
                MainOrdersDGV.Columns["ProfilDispatchedCount"].Visible = true;

                MainOrdersDGV.Columns["TPSProductionStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["TPSStorageStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["TPSDispatchStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["TPSPackPercentage"].Visible = true;
                MainOrdersDGV.Columns["TPSPackedCount"].Visible = true;
                MainOrdersDGV.Columns["TPSExpPercentage"].Visible = true;
                MainOrdersDGV.Columns["TPSExpCount"].Visible = true;
                MainOrdersDGV.Columns["TPSDispPercentage"].Visible = true;
                MainOrdersDGV.Columns["TPSDispatchedCount"].Visible = true;


                MegaOrdersDGV.Columns["ProfilPackPercentage"].Visible = true;
                MegaOrdersDGV.Columns["ProfilPackedCount"].Visible = true;
                MegaOrdersDGV.Columns["ProfilExpPercentage"].Visible = true;
                MegaOrdersDGV.Columns["ProfilExpCount"].Visible = true;
                MegaOrdersDGV.Columns["ProfilDispPercentage"].Visible = true;
                MegaOrdersDGV.Columns["ProfilDispatchedCount"].Visible = true;

                MegaOrdersDGV.Columns["TPSPackPercentage"].Visible = true;
                MegaOrdersDGV.Columns["TPSPackedCount"].Visible = true;
                MegaOrdersDGV.Columns["TPSExpPercentage"].Visible = true;
                MegaOrdersDGV.Columns["TPSExpCount"].Visible = true;
                MegaOrdersDGV.Columns["TPSDispPercentage"].Visible = true;
                MegaOrdersDGV.Columns["TPSDispatchedCount"].Visible = true;
            }

            if (FactoryID == 1)
            {
                MainOrdersDGV.Columns["ProfilProductionStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["ProfilStorageStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["ProfilDispatchStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["ProfilPackPercentage"].Visible = true;
                MainOrdersDGV.Columns["ProfilExpPercentage"].Visible = true;
                MainOrdersDGV.Columns["ProfilExpCount"].Visible = true;
                MainOrdersDGV.Columns["ProfilPackedCount"].Visible = true;
                MainOrdersDGV.Columns["ProfilDispPercentage"].Visible = true;
                MainOrdersDGV.Columns["ProfilDispatchedCount"].Visible = true;

                MainOrdersDGV.Columns["TPSProductionStatusColumn"].Visible = false;
                MainOrdersDGV.Columns["TPSStorageStatusColumn"].Visible = false;
                MainOrdersDGV.Columns["TPSDispatchStatusColumn"].Visible = false;
                MainOrdersDGV.Columns["TPSPackPercentage"].Visible = false;
                MainOrdersDGV.Columns["TPSPackedCount"].Visible = false;
                MainOrdersDGV.Columns["TPSExpPercentage"].Visible = false;
                MainOrdersDGV.Columns["TPSExpCount"].Visible = false;
                MainOrdersDGV.Columns["TPSDispPercentage"].Visible = false;
                MainOrdersDGV.Columns["TPSDispatchedCount"].Visible = false;

                MegaOrdersDGV.Columns["ProfilPackPercentage"].Visible = true;
                MegaOrdersDGV.Columns["ProfilPackedCount"].Visible = true;
                MegaOrdersDGV.Columns["ProfilExpPercentage"].Visible = true;
                MegaOrdersDGV.Columns["ProfilExpCount"].Visible = true;
                MegaOrdersDGV.Columns["ProfilDispPercentage"].Visible = true;
                MegaOrdersDGV.Columns["ProfilDispatchedCount"].Visible = true;

                MegaOrdersDGV.Columns["TPSPackPercentage"].Visible = false;
                MegaOrdersDGV.Columns["TPSPackedCount"].Visible = false;
                MegaOrdersDGV.Columns["TPSExpPercentage"].Visible = false;
                MegaOrdersDGV.Columns["TPSExpCount"].Visible = false;
                MegaOrdersDGV.Columns["TPSDispPercentage"].Visible = false;
                MegaOrdersDGV.Columns["TPSDispatchedCount"].Visible = false;
            }

            if (FactoryID == 2)
            {
                MainOrdersDGV.Columns["ProfilProductionStatusColumn"].Visible = false;
                MainOrdersDGV.Columns["ProfilStorageStatusColumn"].Visible = false;
                MainOrdersDGV.Columns["ProfilDispatchStatusColumn"].Visible = false;
                MainOrdersDGV.Columns["ProfilPackPercentage"].Visible = false;
                MainOrdersDGV.Columns["ProfilPackedCount"].Visible = false;
                MainOrdersDGV.Columns["ProfilExpPercentage"].Visible = false;
                MainOrdersDGV.Columns["ProfilExpCount"].Visible = false;
                MainOrdersDGV.Columns["ProfilDispPercentage"].Visible = false;
                MainOrdersDGV.Columns["ProfilDispatchedCount"].Visible = false;

                MainOrdersDGV.Columns["TPSProductionStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["TPSStorageStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["TPSDispatchStatusColumn"].Visible = true;
                MainOrdersDGV.Columns["TPSPackPercentage"].Visible = true;
                MainOrdersDGV.Columns["TPSPackedCount"].Visible = true;
                MainOrdersDGV.Columns["TPSExpPercentage"].Visible = true;
                MainOrdersDGV.Columns["TPSExpCount"].Visible = true;
                MainOrdersDGV.Columns["TPSDispPercentage"].Visible = true;
                MainOrdersDGV.Columns["TPSDispatchedCount"].Visible = true;

                MegaOrdersDGV.Columns["ProfilPackPercentage"].Visible = false;
                MegaOrdersDGV.Columns["ProfilPackedCount"].Visible = false;
                MegaOrdersDGV.Columns["ProfilExpPercentage"].Visible = false;
                MegaOrdersDGV.Columns["ProfilExpCount"].Visible = false;
                MegaOrdersDGV.Columns["ProfilDispPercentage"].Visible = false;
                MegaOrdersDGV.Columns["ProfilDispatchedCount"].Visible = false;

                MegaOrdersDGV.Columns["TPSPackPercentage"].Visible = true;
                MegaOrdersDGV.Columns["TPSPackedCount"].Visible = true;
                MegaOrdersDGV.Columns["TPSExpPercentage"].Visible = true;
                MegaOrdersDGV.Columns["TPSExpCount"].Visible = true;
                MegaOrdersDGV.Columns["TPSDispPercentage"].Visible = true;
                MegaOrdersDGV.Columns["TPSDispatchedCount"].Visible = true;
            }
        }

        private void MainGridSetting()
        {
            MainOrdersDGV.Columns["CheckBoxColumn"].SortMode = DataGridViewColumnSortMode.Programmatic;
            foreach (DataGridViewColumn Column in MainOrdersDGV.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
                Column.MinimumWidth = 150;
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
            //MainOrdersDataGrid.Columns["ClientID"].Visible = false;
            //MainOrdersDataGrid.Columns["MegaOrderID"].Visible = false;
            //MainOrdersDataGrid.Columns["DoNotDispatch"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffDebtCost"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffDefectsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffProductionErrorsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["WriteOffZOVErrorsCost"].Visible = false;
            //MainOrdersDataGrid.Columns["TotalWriteOffCost"].Visible = false;
            //MainOrdersDataGrid.Columns["IncomeCost"].Visible = false;
            //MainOrdersDataGrid.Columns["ProfitCost"].Visible = false;
            //MainOrdersDataGrid.Columns["NeedCalculate"].Visible = false;
            //MainOrdersDataGrid.Columns["DocDateTime"].Visible = false;
            //MainOrdersDataGrid.Columns["AllocPackDateTime"].Visible = false;
            //MainOrdersDataGrid.Columns["DoNotDispatch"].Visible = false;
            MainOrdersDGV.Columns["WillPercentID"].Visible = false;
            MainOrdersDGV.Columns["DebtTypeID"].Visible = false;
            MainOrdersDGV.Columns["PriceTypeID"].Visible = false;
            MainOrdersDGV.Columns["IsPrepared"].Visible = false;
            MainOrdersDGV.Columns["FactoryID"].Visible = false;
            MainOrdersDGV.Columns["DispatchStatusID"].Visible = false;
            //MainOrdersDGV.Columns["AllocPackDateTime"].Visible = false;
            MainOrdersDGV.Columns["ProfilPackAllocStatusID"].Visible = false;
            MainOrdersDGV.Columns["TPSPackAllocStatusID"].Visible = false;
            //MainOrdersDGV.Columns["TPSPackCount"].Visible = false;
            MainOrdersDGV.Columns["ProfilPackAllocStatusID"].Visible = false;
            //MainOrdersDGV.Columns["ProfilPackCount"].Visible = false;
            MainOrdersDGV.Columns["ClientID"].Visible = false;
            MainOrdersDGV.Columns["MegaOrderID"].Visible = false;

            if (MainOrdersDGV.Columns.Contains("DispatchID"))
                MainOrdersDGV.Columns["DispatchID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("DoubleOrder"))
                MainOrdersDGV.Columns["DoubleOrder"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("FirstOperatorID"))
                MainOrdersDGV.Columns["FirstOperatorID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("SecondOperatorID"))
                MainOrdersDGV.Columns["SecondOperatorID"].Visible = false;

            if (MainOrdersDGV.Columns.Contains("ProfilExpeditionStatusID"))
                MainOrdersDGV.Columns["ProfilExpeditionStatusID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("TPSExpeditionStatusID"))
                MainOrdersDGV.Columns["TPSExpeditionStatusID"].Visible = false;

            if (MainOrdersDGV.Columns.Contains("ProfilProductionStatusID"))
                MainOrdersDGV.Columns["ProfilProductionStatusID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("ProfilStorageStatusID"))
                MainOrdersDGV.Columns["ProfilStorageStatusID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("ProfilDispatchStatusID"))
                MainOrdersDGV.Columns["ProfilDispatchStatusID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("TPSProductionStatusID"))
                MainOrdersDGV.Columns["TPSProductionStatusID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("TPSStorageStatusID"))
                MainOrdersDGV.Columns["TPSStorageStatusID"].Visible = false;
            if (MainOrdersDGV.Columns.Contains("TPSDispatchStatusID"))
                MainOrdersDGV.Columns["TPSDispatchStatusID"].Visible = false;

            if (MainOrdersDGV.Columns.Contains("CheckBoxColumn"))
                MainOrdersDGV.Columns["CheckBoxColumn"].Visible = false;

            MainOrdersDGV.Columns["ProfilProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MainOrdersDGV.Columns["TPSProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MainOrdersDGV.Columns["MovePrepareDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            MainOrdersDGV.Columns["MainOrderID"].HeaderText = "№ п\\п";
            MainOrdersDGV.Columns["NeedCalculate"].HeaderText = "Включено в\r\nрасчет";
            MainOrdersDGV.Columns["DocNumber"].HeaderText = "№ документа";
            MainOrdersDGV.Columns["DebtDocNumber"].HeaderText = "№ документа\r\nдолга";
            MainOrdersDGV.Columns["ReorderDocNumber"].HeaderText = "№ документа\r\nперезаказа";
            MainOrdersDGV.Columns["IsSample"].HeaderText = "Образцы";
            MainOrdersDGV.Columns["FrontsCost"].HeaderText = "Стоимость\r\nфасадов, евро";
            MainOrdersDGV.Columns["DecorCost"].HeaderText = "Стоимость\r\nдекора, евро";
            MainOrdersDGV.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            MainOrdersDGV.Columns["DocDateTime"].HeaderText = "Дата\r\nсоздания";
            MainOrdersDGV.Columns["Notes"].HeaderText = "Примечание к заказу";
            MainOrdersDGV.Columns["FrontsSquare"].HeaderText = "Квадратура";
            MainOrdersDGV.Columns["Weight"].HeaderText = "Вес, кг.";
            MainOrdersDGV.Columns["ClientName"].HeaderText = "Клиент";
            MainOrdersDGV.Columns["AllocPackDateTime"].HeaderText = "Дата\r\nраспределения";
            MainOrdersDGV.Columns["MovePrepareDate"].HeaderText = "Дата переноса";
            MainOrdersDGV.Columns["ProfilProductionDate"].HeaderText = "Дата входа\r\nв пр-во, Профиль";
            MainOrdersDGV.Columns["TPSProductionDate"].HeaderText = "Дата входа\r\nв пр-во, ТПС";
            MainOrdersDGV.Columns["ProfilOnProductionDate"].HeaderText = "Дата входа\r\nна пр-во, Профиль";
            MainOrdersDGV.Columns["TPSOnProductionDate"].HeaderText = "Дата входа\r\nна пр-во, ТПС";

            MainOrdersDGV.Columns["DispatchedCost"].HeaderText = "Отгружено,\r\nевро";
            MainOrdersDGV.Columns["DispatchedDebtCost"].HeaderText = "Не отгружено,\r\nевро";
            MainOrdersDGV.Columns["CalcDebtCost"].HeaderText = "Долги в расчете,\r\nевро";
            MainOrdersDGV.Columns["SamplesWriteOffCost"].HeaderText = "Списание по\r\nобразцам, евро";
            MainOrdersDGV.Columns["WriteOffDebtCost"].HeaderText = "Долги по отгрузке,\r\nевро";
            MainOrdersDGV.Columns["WriteOffDefectsCost"].HeaderText = "Брак по возврату,\r\nевро";
            MainOrdersDGV.Columns["WriteOffProductionErrorsCost"].HeaderText = "Ошибки пр-ва\r\nпо возврату, евро";
            MainOrdersDGV.Columns["WriteOffZOVErrorsCost"].HeaderText = "Ошибки ЗОВа\r\nпо возврату, евро";
            MainOrdersDGV.Columns["TotalWriteOffCost"].HeaderText = "Списано по возвратам,\r\nевро";
            MainOrdersDGV.Columns["IncomeCost"].HeaderText = "Итого по расчету,\r\nевро";
            MainOrdersDGV.Columns["ProfitCost"].HeaderText = "Итого,\r\n евро";

            MainOrdersDGV.Columns["DoNotDispatch"].HeaderText = "Отгрузка\r\nбез фасадов";
            MainOrdersDGV.Columns["TechDrilling"].HeaderText = "Сверление";
            MainOrdersDGV.Columns["QuicklyOrder"].HeaderText = "Срочно";
            MainOrdersDGV.Columns["ToAssembly"].HeaderText = "На сборку";
            MainOrdersDGV.Columns["FromAssembly"].HeaderText = "Со сборки";
            MainOrdersDGV.Columns["IsNotPaid"].HeaderText = "Не оплачено";


            MainOrdersDGV.Columns["ProfilPackPercentage"].HeaderText = "Упаковано\r\nПрофиль, %";
            MainOrdersDGV.Columns["ProfilPackedCount"].HeaderText = "Упаковано\r\n Профиль, кол-во";
            MainOrdersDGV.Columns["ProfilStoreCount"].HeaderText = "Склад,\r\nПрофиль, кол-во";
            MainOrdersDGV.Columns["ProfilStorePercentage"].HeaderText = "Склад,\r\nПрофиль, %";
            MainOrdersDGV.Columns["ProfilExpCount"].HeaderText = "Экспедиция,\r\nПрофиль, кол-во";
            MainOrdersDGV.Columns["ProfilExpPercentage"].HeaderText = "Экспедиция,\r\nПрофиль, %";
            MainOrdersDGV.Columns["ProfilDispPercentage"].HeaderText = "Отгружено\r\nПрофиль, %";
            MainOrdersDGV.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            MainOrdersDGV.Columns["TPSPackPercentage"].HeaderText = "Упаковано\r\n    ТПС, %";
            MainOrdersDGV.Columns["TPSPackedCount"].HeaderText = "Упаковано\r\nТПС, кол-во";
            MainOrdersDGV.Columns["TPSStoreCount"].HeaderText = "Склад,\r\nТПС, кол-во";
            MainOrdersDGV.Columns["TPSStorePercentage"].HeaderText = "Склад,\r\nТПС, %";
            MainOrdersDGV.Columns["TPSExpCount"].HeaderText = "Экспедиция,\r\nТПС, кол-во";
            MainOrdersDGV.Columns["TPSExpPercentage"].HeaderText = "Экспедиция,\r\nТПС, %";
            MainOrdersDGV.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            MainOrdersDGV.Columns["TPSDispatchedCount"].HeaderText = " Отгружено\r\nТПС, кол-во";
            MainOrdersDGV.Columns["BatchNumber"].HeaderText = "Партия";
            MainOrdersDGV.Columns["CheckBoxColumn"].HeaderText = string.Empty;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 1
            };
            MainOrdersDGV.Columns["CheckBoxColumn"].ReadOnly = false;

            MainOrdersDGV.Columns["Weight"].DefaultCellStyle.Format = "C";
            MainOrdersDGV.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDGV.Columns["CheckBoxColumn"].MinimumWidth = 50;
            MainOrdersDGV.Columns["CheckBoxColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["CheckBoxColumn"].Width = 50;
            MainOrdersDGV.Columns["MovePrepareDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["MovePrepareDate"].MinimumWidth = 140;
            MainOrdersDGV.Columns["ProfilProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilProductionDate"].MinimumWidth = 165;
            MainOrdersDGV.Columns["TPSProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSProductionDate"].MinimumWidth = 140;
            MainOrdersDGV.Columns["ProfilOnProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilOnProductionDate"].MinimumWidth = 165;
            MainOrdersDGV.Columns["TPSOnProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSOnProductionDate"].MinimumWidth = 140;

            MainOrdersDGV.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["MainOrderID"].Width = 65;
            MainOrdersDGV.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MainOrdersDGV.Columns["ClientName"].MinimumWidth = 160;
            MainOrdersDGV.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["DocNumber"].MinimumWidth = 190;
            MainOrdersDGV.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["FrontsSquare"].Width = 120;
            MainOrdersDGV.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["IsSample"].Width = 100;
            MainOrdersDGV.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            MainOrdersDGV.Columns["Notes"].MinimumWidth = 110;
            MainOrdersDGV.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["Weight"].Width = 70;
            MainOrdersDGV.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["FactoryTypeColumn"].Width = 140;

            MainOrdersDGV.Columns["ProfilPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilPackPercentage"].Width = 155;
            MainOrdersDGV.Columns["ProfilPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilPackedCount"].Width = 155;
            MainOrdersDGV.Columns["TPSPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSPackPercentage"].Width = 125;
            MainOrdersDGV.Columns["TPSPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSPackedCount"].Width = 125;

            MainOrdersDGV.Columns["ProfilStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilStorePercentage"].Width = 155;
            MainOrdersDGV.Columns["ProfilStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilStoreCount"].Width = 155;
            MainOrdersDGV.Columns["TPSStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSStorePercentage"].Width = 125;
            MainOrdersDGV.Columns["TPSStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSStoreCount"].Width = 125;

            MainOrdersDGV.Columns["ProfilExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilExpPercentage"].Width = 155;
            MainOrdersDGV.Columns["ProfilExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilExpCount"].Width = 155;
            MainOrdersDGV.Columns["TPSExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSExpPercentage"].Width = 125;
            MainOrdersDGV.Columns["TPSExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSExpCount"].Width = 125;

            MainOrdersDGV.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilDispPercentage"].Width = 155;
            MainOrdersDGV.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["ProfilDispatchedCount"].Width = 155;
            MainOrdersDGV.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSDispPercentage"].Width = 125;
            MainOrdersDGV.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["TPSDispatchedCount"].Width = 125;

            MainOrdersDGV.Columns["ProfilProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["ProfilProductionStatusColumn"].MinimumWidth = 110;
            MainOrdersDGV.Columns["ProfilStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["ProfilStorageStatusColumn"].MinimumWidth = 110;
            MainOrdersDGV.Columns["ProfilDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["ProfilDispatchStatusColumn"].MinimumWidth = 110;
            MainOrdersDGV.Columns["TPSProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["TPSProductionStatusColumn"].MinimumWidth = 110;
            MainOrdersDGV.Columns["TPSStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["TPSStorageStatusColumn"].MinimumWidth = 110;
            MainOrdersDGV.Columns["TPSDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["TPSDispatchStatusColumn"].MinimumWidth = 110;

            MainOrdersDGV.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDGV.Columns["BatchNumber"].Width = 125;

            MainOrdersDGV.Columns["DebtDocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["DoNotDispatch"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["TechDrilling"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDGV.Columns["QuicklyOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            MainOrdersDGV.AutoGenerateColumns = false;

            int DisplayIndex = 0;

            MainOrdersDGV.Columns["CheckBoxColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["DocNumber"].DisplayIndex = DisplayIndex++;

            MainOrdersDGV.Columns["ProfilPackPercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilPackedCount"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilStorePercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilStoreCount"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilExpPercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilExpCount"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilDispPercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilDispatchedCount"].DisplayIndex = DisplayIndex++;

            MainOrdersDGV.Columns["TPSPackPercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSPackedCount"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSStorePercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSStoreCount"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSExpPercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSExpCount"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSDispPercentage"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSDispatchedCount"].DisplayIndex = DisplayIndex++;

            MainOrdersDGV.Columns["FactoryTypeColumn"].DisplayIndex = DisplayIndex++;
            //MainOrdersDataGrid.Columns["ProfilProductionDate"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilProductionStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilStorageStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ProfilDispatchStatusColumn"].DisplayIndex = DisplayIndex++;
            //MainOrdersDataGrid.Columns["TPSProductionDate"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSProductionStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSStorageStatusColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["TPSDispatchStatusColumn"].DisplayIndex = DisplayIndex++;

            MainOrdersDGV.Columns["DebtDocNumber"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["ReorderDocNumber"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["DoNotDispatch"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["IsSample"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["FrontsSquare"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["Weight"].DisplayIndex = DisplayIndex++;
            MainOrdersDGV.Columns["Notes"].DisplayIndex = DisplayIndex++;

            MainOrdersDGV.Columns["BatchNumber"].DisplayIndex = 4;

            MainOrdersDGV.Columns["IsSample"].SortMode = DataGridViewColumnSortMode.Automatic;
            MainOrdersDGV.Columns["DoNotDispatch"].SortMode = DataGridViewColumnSortMode.Automatic;
            MainOrdersDGV.Columns["TechDrilling"].SortMode = DataGridViewColumnSortMode.Automatic;
            MainOrdersDGV.Columns["QuicklyOrder"].SortMode = DataGridViewColumnSortMode.Automatic;

            MainOrdersDGV.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDGV.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDGV.Columns["IsSample"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            MainOrdersDGV.Columns["ProfilPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["ProfilPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("ProfilPackPercentage");
            MainOrdersDGV.Columns["ProfilStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["ProfilStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("ProfilStorePercentage");
            MainOrdersDGV.Columns["ProfilExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["ProfilExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("ProfilExpPercentage");
            MainOrdersDGV.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("ProfilDispPercentage");

            MainOrdersDGV.Columns["TPSPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["TPSPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("TPSPackPercentage");
            MainOrdersDGV.Columns["TPSStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["TPSStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("TPSStorePercentage");
            MainOrdersDGV.Columns["TPSExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["TPSExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("TPSExpPercentage");
            MainOrdersDGV.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDGV.AddPercentageColumn("TPSDispPercentage");
        }

        private void MegaGridSetting()
        {
            foreach (DataGridViewColumn Column in MegaOrdersDGV.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 150;
            }

            if (!Security.PriceAccess)
            {
                MegaOrdersDGV.Columns["TotalCost"].Visible = false;
                MegaOrdersDGV.Columns["DispatchedCost"].Visible = false;
                MegaOrdersDGV.Columns["DispatchedDebtCost"].Visible = false;
                MegaOrdersDGV.Columns["CalcDebtCost"].Visible = false;
                MegaOrdersDGV.Columns["CalcDefectsCost"].Visible = false;
                MegaOrdersDGV.Columns["CalcProductionErrorsCost"].Visible = false;
                MegaOrdersDGV.Columns["CalcZOVErrorsCost"].Visible = false;
                MegaOrdersDGV.Columns["WriteOffDebtCost"].Visible = false;
                MegaOrdersDGV.Columns["WriteOffDefectsCost"].Visible = false;
                MegaOrdersDGV.Columns["WriteOffProductionErrorsCost"].Visible = false;
                MegaOrdersDGV.Columns["WriteOffZOVErrorsCost"].Visible = false;
                MegaOrdersDGV.Columns["SamplesWriteOffCost"].Visible = false;
                MegaOrdersDGV.Columns["TotalWriteOffCost"].Visible = false;
                MegaOrdersDGV.Columns["TotalCalcWriteOffCost"].Visible = false;
                MegaOrdersDGV.Columns["IncomeCost"].Visible = false;
                MegaOrdersDGV.Columns["ProfitCost"].Visible = false;
            }

            if (MegaOrdersDGV.Columns.Contains("ProfilPackAllocStatusID"))
                MegaOrdersDGV.Columns["ProfilPackAllocStatusID"].Visible = false;
            if (MegaOrdersDGV.Columns.Contains("TPSPackAllocStatusID"))
                MegaOrdersDGV.Columns["TPSPackAllocStatusID"].Visible = false;

            MegaOrdersDGV.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDGV.Columns["DispatchDate"].HeaderText = "    Дата\r\nотгрузки";

            MegaOrdersDGV.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDGV.Columns["Square"].HeaderText = "Площадь\r\nфасадов, м.кв.";
            MegaOrdersDGV.Columns["TotalCost"].HeaderText = "Расчет,\r\n  евро";
            MegaOrdersDGV.Columns["DispatchedCost"].HeaderText = "Отгружено,\r\nевро";
            MegaOrdersDGV.Columns["DispatchedDebtCost"].HeaderText = "Не отгружено,\r\nевро";
            MegaOrdersDGV.Columns["CalcDebtCost"].HeaderText = "Долги в расчете,\r\nевро";
            MegaOrdersDGV.Columns["CalcDefectsCost"].HeaderText = "Брак в расчете,\r\nевро";
            MegaOrdersDGV.Columns["CalcProductionErrorsCost"].HeaderText = " Ошибки пр-ва\r\nв расчете, евро";
            MegaOrdersDGV.Columns["CalcZOVErrorsCost"].HeaderText = "Ошибки ЗОВа\r\nв расчете, евро";
            MegaOrdersDGV.Columns["WriteOffDebtCost"].HeaderText = "Долги по возвратам,\r\nевро";
            MegaOrdersDGV.Columns["WriteOffDefectsCost"].HeaderText = "Брак по возвратам,\r\nевро";
            MegaOrdersDGV.Columns["WriteOffProductionErrorsCost"].HeaderText = "Ошибки пр-ва\r\nпо возвратам, евро";
            MegaOrdersDGV.Columns["WriteOffZOVErrorsCost"].HeaderText = "Ошибки ЗОВа\r\nпо возвратам, евро";
            MegaOrdersDGV.Columns["SamplesWriteOffCost"].HeaderText = "Списание за образцы,\r\nевро";
            MegaOrdersDGV.Columns["TotalWriteOffCost"].HeaderText = "Списано по возвратам,\r\nевро";
            MegaOrdersDGV.Columns["TotalCalcWriteOffCost"].HeaderText = "Списано по расчету,\r\nевро";
            MegaOrdersDGV.Columns["IncomeCost"].HeaderText = "Итого по расчету,\r\nевро";
            MegaOrdersDGV.Columns["ProfitCost"].HeaderText = "Итого,\r\n евро";

            MegaOrdersDGV.Columns["ProfilPackCount"].HeaderText = "Кол-во упаковок\r\nПрофиль";
            MegaOrdersDGV.Columns["TPSPackCount"].HeaderText = "Кол-во упаковок\r\nТПС";
            MegaOrdersDGV.Columns["ProfilPackPercentage"].HeaderText = "Упаковано\r\nПрофиль, %";
            MegaOrdersDGV.Columns["ProfilPackedCount"].HeaderText = "Упаковано\r\n Профиль, кол-во";
            MegaOrdersDGV.Columns["ProfilStoreCount"].HeaderText = "Склад,\r\nПрофиль, кол-во";
            MegaOrdersDGV.Columns["ProfilStorePercentage"].HeaderText = "Склад,\r\nПрофиль, %";
            MegaOrdersDGV.Columns["ProfilExpCount"].HeaderText = "Экспедиция,\r\nПрофиль, кол-во";
            MegaOrdersDGV.Columns["ProfilExpPercentage"].HeaderText = "Экспедиция,\r\nПрофиль, %";
            MegaOrdersDGV.Columns["ProfilDispPercentage"].HeaderText = " Отгружено\r\nПрофиль, %";
            MegaOrdersDGV.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            MegaOrdersDGV.Columns["TPSPackPercentage"].HeaderText = "Упаковано\r\nТПС, %";
            MegaOrdersDGV.Columns["TPSPackedCount"].HeaderText = "Упаковано\r\nТПС, кол-во";
            MegaOrdersDGV.Columns["TPSStoreCount"].HeaderText = "Склад,\r\nТПС, кол-во";
            MegaOrdersDGV.Columns["TPSStorePercentage"].HeaderText = "Склад,\r\nТПС, %";
            MegaOrdersDGV.Columns["TPSExpCount"].HeaderText = "Экспедиция,\r\nТПС, кол-во";
            MegaOrdersDGV.Columns["TPSExpPercentage"].HeaderText = "Экспедиция,\r\nТПС, %";
            MegaOrdersDGV.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            MegaOrdersDGV.Columns["TPSDispatchedCount"].HeaderText = "Отгружено\r\nТПС, кол-во";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 1
            };
            NumberFormatInfo nfi3 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2
            };
            MegaOrdersDGV.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDGV.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDGV.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDGV.Columns["Square"].DefaultCellStyle.FormatProvider = nfi3;

            MegaOrdersDGV.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            MegaOrdersDGV.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["DispatchDate"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilPackPercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilPackedCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilStorePercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilStoreCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilExpPercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilExpCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilDispPercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["ProfilDispatchedCount"].DisplayIndex = DisplayIndex++;

            MegaOrdersDGV.Columns["TPSPackPercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSPackedCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSStorePercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSStoreCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSExpPercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSExpCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSDispPercentage"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["TPSDispatchedCount"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["Square"].DisplayIndex = DisplayIndex++;
            MegaOrdersDGV.Columns["Weight"].DisplayIndex = DisplayIndex++;

            MegaOrdersDGV.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDGV.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //MegaOrdersDataGrid.Columns["Percentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            MegaOrdersDGV.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["Square"].Width = 130;
            MegaOrdersDGV.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MegaOrdersDGV.Columns["DispatchDate"].MinimumWidth = 120;
            MegaOrdersDGV.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["Weight"].Width = 70;
            MegaOrdersDGV.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["MegaOrderID"].Width = 80;

            MegaOrdersDGV.Columns["ProfilPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilPackPercentage"].Width = 155;
            MegaOrdersDGV.Columns["ProfilPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilPackedCount"].Width = 155;
            MegaOrdersDGV.Columns["TPSPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSPackPercentage"].Width = 125;
            MegaOrdersDGV.Columns["TPSPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSPackedCount"].Width = 125;

            MegaOrdersDGV.Columns["ProfilStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilStorePercentage"].Width = 155;
            MegaOrdersDGV.Columns["ProfilStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilStoreCount"].Width = 155;
            MegaOrdersDGV.Columns["TPSStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSStorePercentage"].Width = 125;
            MegaOrdersDGV.Columns["TPSStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSStoreCount"].Width = 125;

            MegaOrdersDGV.Columns["ProfilExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilExpPercentage"].Width = 155;
            MegaOrdersDGV.Columns["ProfilExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilExpCount"].Width = 155;
            MegaOrdersDGV.Columns["TPSExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSExpPercentage"].Width = 125;
            MegaOrdersDGV.Columns["TPSExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSExpCount"].Width = 125;

            MegaOrdersDGV.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilDispPercentage"].Width = 155;
            MegaOrdersDGV.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["ProfilDispatchedCount"].Width = 155;
            MegaOrdersDGV.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSDispPercentage"].Width = 125;
            MegaOrdersDGV.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDGV.Columns["TPSDispatchedCount"].Width = 125;

            MegaOrdersDGV.Columns["DispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDGV.Columns["DispatchStatusColumn"].MinimumWidth = 120;

            MegaOrdersDGV.Columns["ProfilPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["ProfilPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("ProfilPackPercentage");
            MegaOrdersDGV.Columns["ProfilStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["ProfilStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("ProfilStorePercentage");
            MegaOrdersDGV.Columns["ProfilExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["ProfilExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("ProfilExpPercentage");
            MegaOrdersDGV.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("ProfilDispPercentage");

            MegaOrdersDGV.Columns["TPSPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["TPSPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("TPSPackPercentage");
            MegaOrdersDGV.Columns["TPSStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["TPSStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("TPSStorePercentage");
            MegaOrdersDGV.Columns["TPSExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["TPSExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("TPSExpPercentage");
            MegaOrdersDGV.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDGV.AddPercentageColumn("TPSDispPercentage");
        }

        private void PackagesGridSetting()
        {
            PackagesDGV.Columns["CheckBoxColumn"].SortMode = DataGridViewColumnSortMode.Programmatic;
            foreach (DataGridViewColumn Column in PackagesDGV.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            //PackagesDataGrid.Columns["PackageID"].Visible = false;
            PackagesDGV.Columns["PackageStatusID"].Visible = false;
            PackagesDGV.Columns["MainOrderID"].Visible = false;
            PackagesDGV.Columns["PrintedCount"].Visible = false;

            if (PackagesDGV.Columns.Contains("CheckBoxColumn"))
                PackagesDGV.Columns["CheckBoxColumn"].Visible = false;
            if (PackagesDGV.Columns.Contains("PalleteID"))
                PackagesDGV.Columns["PalleteID"].Visible = false;
            if (PackagesDGV.Columns.Contains("DispatchID"))
                PackagesDGV.Columns["DispatchID"].Visible = false;
            if (PackagesDGV.Columns.Contains("PackUserID"))
                PackagesDGV.Columns["PackUserID"].Visible = false;
            if (PackagesDGV.Columns.Contains("StoreUserID"))
                PackagesDGV.Columns["StoreUserID"].Visible = false;
            if (PackagesDGV.Columns.Contains("ExpUserID"))
                PackagesDGV.Columns["ExpUserID"].Visible = false;
            if (PackagesDGV.Columns.Contains("DispUserID"))
                PackagesDGV.Columns["DispUserID"].Visible = false;

            if (PackagesDGV.Columns.Contains("ProductType"))
                PackagesDGV.Columns["ProductType"].Visible = false;
            if (PackagesDGV.Columns.Contains("FactoryID"))
                PackagesDGV.Columns["FactoryID"].Visible = false;

            PackagesDGV.Columns["PrintDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDGV.Columns["PackingDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDGV.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDGV.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDGV.Columns["DispatchDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDGV.Columns["PackNumber"].HeaderText = "  №\r\nупак.";
            PackagesDGV.Columns["PrintedCount"].HeaderText = "    Кол-во\r\nраспечаток";
            PackagesDGV.Columns["PrintDateTime"].HeaderText = "  Дата\r\nпечати";
            PackagesDGV.Columns["PackingDateTime"].HeaderText = "   Дата\r\nупаковки";
            PackagesDGV.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            PackagesDGV.Columns["ExpeditionDateTime"].HeaderText = "      Дата\r\nэкспедиции";
            PackagesDGV.Columns["DispatchDateTime"].HeaderText = "    Дата\r\nотгрузки";
            PackagesDGV.Columns["PackageID"].HeaderText = "ID";
            PackagesDGV.Columns["FactoryName"].HeaderText = "Участок";
            PackagesDGV.Columns["TrayID"].HeaderText = " №\r\nпод.";
            PackagesDGV.Columns["CheckBoxColumn"].HeaderText = string.Empty;

            PackagesDGV.Columns["CheckBoxColumn"].ReadOnly = false;

            PackagesDGV.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["PackNumber"].Width = 70;
            PackagesDGV.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["PackageStatusesColumn"].Width = 140;
            //PackagesDataGrid.Columns["PrintedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //PackagesDataGrid.Columns["PrintedCount"].Width = 140;
            PackagesDGV.Columns["PrintDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["PrintDateTime"].Width = 150;
            PackagesDGV.Columns["PackingDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["PackingDateTime"].Width = 150;
            PackagesDGV.Columns["StorageDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["StorageDateTime"].Width = 150;
            PackagesDGV.Columns["ExpeditionDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["ExpeditionDateTime"].Width = 150;
            PackagesDGV.Columns["DispatchDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["DispatchDateTime"].Width = 150;
            PackagesDGV.Columns["PackageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["PackageID"].Width = 100;
            PackagesDGV.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["TrayID"].Width = 100;
            PackagesDGV.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["FactoryName"].Width = 100;
            PackagesDGV.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["MainOrderID"].Width = 100;
            PackagesDGV.Columns["CheckBoxColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDGV.Columns["CheckBoxColumn"].Width = 50;

            PackagesDGV.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            PackagesDGV.Columns["CheckBoxColumn"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["PackNumber"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["PackageStatusesColumn"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["FactoryName"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["TrayID"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["PrintDateTime"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["PackingDateTime"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["StorageDateTime"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["ExpeditionDateTime"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["DispatchDateTime"].DisplayIndex = DisplayIndex++;
            PackagesDGV.Columns["PackageID"].DisplayIndex = DisplayIndex++;
            //PackagesDataGrid.Columns["PrintedCount"].DisplayIndex = 8;
            //PackagesDataGrid.Columns["MainOrderID"].DisplayIndex = 1;

            //for (int i = 0; i < PackagesDataGrid.Rows.Count; i++)
            //{
            //    int PackageStatusID = Convert.ToInt32(PackagesDataGrid.Rows[i].Cells["PackageStatusID"].Value);

            //    if (PackageStatusID == 1)
            //    {
            //        PackagesDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
            //    }
            //}
        }

        public void ShowMainOrdersCheckBoxColumn(bool bShow)
        {
            if (MainOrdersDGV.Columns.Contains("CheckBoxColumn"))
                MainOrdersDGV.Columns["CheckBoxColumn"].Visible = bShow;
        }

        public void ShowPackagesCheckBoxColumn(bool bShow)
        {
            if (PackagesDGV.Columns.Contains("CheckBoxColumn"))
                PackagesDGV.Columns["CheckBoxColumn"].Visible = bShow;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            FillMegaPercentageColumn();
            CreateColumns();
            MainGridSetting();
            MegaGridSetting();
            PackagesGridSetting();
        }
        #endregion


        #region Filter

        public void Filter(bool NotProduction,
            bool InProduction,
            bool OnProduction,
            bool OnStorage,
            bool Dispatched, int FactoryID)
        {
            string FactoryFilter = string.Empty;
            string OrdersProductionStatus = string.Empty;

            int Pos = MegaOrdersBS.Position;
            int MegaOrderID = -1;
            //остается на позиции удаленного
            if (MegaOrdersBS.Count > 0)
            {
                MegaOrderID = Convert.ToInt32(((DataRowView)MegaOrdersBS.Current)["MegaOrderID"]);
            }

            if (FactoryID > 0)
                FactoryFilter = " WHERE (FactoryID = 0 OR FactoryID = " + FactoryID + ")";

            if (FactoryID == -1)
                FactoryFilter = " WHERE (FactoryID = -1)";

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1))";

                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";

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
                    OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }
            }

            if (OrdersProductionStatus.Length > 0)
            {
                if (FactoryFilter.Length > 0)
                    OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
                else
                    OrdersProductionStatus = " WHERE (" + OrdersProductionStatus + ")";
            }

            #endregion

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MegaOrders" +
                " WHERE (MegaOrderID = 0 OR MegaOrderID > 11025) AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders " + FactoryFilter + OrdersProductionStatus + ") ORDER BY DispatchDate",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                MegaOrdersDT.Clear();
                DA.Fill(MegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID <> 0)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PackMegaOrdersDT.Clear();
                DA.Fill(PackMegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages" +
                " WHERE PackageStatusID <> 0" +
                " GROUP BY MainOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PackMainOrdersDT.Clear();
                DA.Fill(PackMainOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 2)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMegaOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =2 " +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMainOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 4)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                ExpMegaOrdersDT.Clear();
                DA.Fill(ExpMegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =4 " +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                ExpMainOrdersDT.Clear();
                DA.Fill(ExpMainOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 3)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DispMegaOrdersDT.Clear();
                DA.Fill(DispMegaOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages" +
                " WHERE PackageStatusID = 3" +
                " GROUP BY MainOrderID, Packages.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DispMainOrdersDT.Clear();
                DA.Fill(DispMainOrdersDT);
            }

            FillMegaPercentageColumn();
            ShowColumns(FactoryID);

            MegaOrdersBS.Position = MegaOrdersBS.Find("MegaOrderID", MegaOrderID);
            //MegaOrdersBindingSource.MoveFirst();
        }

        public void FilterMainOrdersByMegaOrder(int MegaOrderID,
            bool NotProduction,
            bool InProduction,
            bool OnProduction,
            bool OnStorage,
            bool Dispatched,
            int FactoryID)
        {
            //if (CurrentMegaOrderID == MegaOrderID)
            //    return;

            if (MegaOrdersBS.Count == 0)
                return;

            string FactoryFilter = string.Empty;
            string OrdersProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1))";

                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";

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
                    OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }
            }

            if (OrdersProductionStatus.Length > 0)
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";

            #endregion

            MainOrdersDT.Clear();

            string SelectionCommand = "SELECT MainOrders.*, ClientName FROM MainOrders" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients" +
                " ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + OrdersProductionStatus + " ORDER BY ClientName";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDT);
            }

            FillMainPercentageColumn();
            FillBatchNumber();

            CurrentMegaOrderID = MegaOrderID;
            //MainOrdersDataTable.DefaultView.Sort = "PackPercentage DESC";
            MainOrdersBS.MoveFirst();
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                MainOrdersDT.Rows[i]["CheckBoxColumn"] = false;
            }
        }

        public void GetAllPackages(int MegaOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;
            string MainOrdersFactoryFilter = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter = " AND Packages.FactoryID = " + FactoryID;
                MainOrdersFactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }

            AllPackagesDT.Clear();
            string SelectionCommand = "SELECT PackageID, MainOrderID, DispatchID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ") " + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(AllPackagesDT);
            }
        }

        public void FilterPackagesByMainOrder(int MainOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return;

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND Packages.FactoryID = " + FactoryID;

            PackagesDT.Clear();

            string SelectionCommand = "SELECT Packages.*, FactoryName FROM Packages " +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " WHERE MainOrderID = " + MainOrderID + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }
            PackagesDT.DefaultView.Sort = "PackNumber ASC";
            PackagesBS.MoveFirst();
            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                PackagesDT.Rows[i]["CheckBoxColumn"] = false;
            }
        }

        public void FilterPackagesByMegaOrder(int MegaOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;
            string MainOrdersFactoryFilter = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter = " AND Packages.FactoryID = " + FactoryID;
                MainOrdersFactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }


            OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.FilterByMegaOrder(MegaOrderID, FactoryID);
            OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.FilterByMegaOrder(MegaOrderID, FactoryID);

            PackagesDT.Clear();

            string SelectionCommand = "SELECT * FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ") " + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }
            PackagesDT.DefaultView.Sort = "MainOrderID, PackNumber ASC";
            PackagesBS.MoveFirst();
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                MainOrdersDT.Rows[i]["CheckBoxColumn"] = false;
            }
        }

        public void FilterProductsByPackage(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return;
            CurrentMainOrderID = MainOrderID;
            OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.FilterByMainOrder(MainOrderID, FactoryID);
            OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.FilterByMainOrder(MainOrderID, FactoryID);
        }
        #endregion

        public DataTable ClientsDT
        {
            get
            {
                using (DataView DV = new DataView(ClientsDataTable))
                {
                    DV.Sort = "ClientName";
                    return DV.ToTable();
                }
            }
        }

        public int[] GetSelectedMainOrders()
        {
            int[] rows = new int[MainOrdersDGV.SelectedRows.Count];

            for (int i = 0; i < MainOrdersDGV.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDGV.SelectedRows[i].Cells["MainOrderID"].Value);
            Array.Sort(rows);

            return rows;
        }

        public ArrayList GetMainOrders(int MegaOrderID)
        {
            ArrayList array = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID, ConnectionStrings.ZOVOrdersConnectionString))
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

        public int[] GetMainOrders()
        {
            int[] rows = new int[MainOrdersDT.Rows.Count];

            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]);

            return rows;
        }

        private int GetMainOrderPackCount(int MainOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = PackMainOrdersDT.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMainOrderDispCount(int MainOrderID, int FactoryID)
        {
            int DispCount = 0;
            DataRow[] Rows = DispMainOrdersDT.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                DispCount = Convert.ToInt32(Rows[0]["Count"]);

            return DispCount;
        }

        private int GetMegaOrderPackCount(int MegaOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = PackMegaOrdersDT.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMainOrderStoreCount(int MainOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = StoreMainOrdersDT.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMegaOrderStoreCount(int MegaOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = StoreMegaOrdersDT.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMegaOrderDispCount(int MegaOrderID, int FactoryID)
        {
            int DispCount = 0;
            DataRow[] Rows = DispMegaOrdersDT.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                DispCount = Convert.ToInt32(Rows[0]["Count"]);

            return DispCount;
        }

        private int GetMainOrderExpCount(int MainOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = ExpMainOrdersDT.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMegaOrderExpCount(int MegaOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = ExpMegaOrdersDT.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private string GetBatchNumber(int MainOrderID)
        {
            string MegaBatch = string.Empty;
            string Batch = string.Empty;
            string BatchNumber = string.Empty;

            DataRow[] Rows = BatchDetailsDT.Select("MainOrderID = " + MainOrderID);

            if (Rows.Count() > 0)
            {
                MegaBatch = Rows[0]["MegaBatchID"].ToString();
                Batch = Rows[0]["BatchID"].ToString();
                BatchNumber = MegaBatch + ", " + Batch + ", " + MainOrderID.ToString();
            }

            return BatchNumber;
        }

        public void SearchDocNumber(int MainOrderID)
        {
            DataRow[] Rows = SearchMainOrdersDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() < 1)
                return;
            int MegaOrderID = Convert.ToInt32(Rows[0]["MegaOrderID"]);

            int index = MegaOrdersBS.Find("MegaOrderID",
                                        SearchMainOrdersDT.Select("MainOrderID = " + MainOrderID)[0]["MegaOrderID"]);
            if (index == -1)
                return;

            MegaOrdersBS.Position = index;

            while (MainOrdersBS.Count == 0) ;

            object MOID = ((DataRowView)MegaOrdersBS.Current)["MegaOrderID"].ToString();

            MainOrdersBS.Position = MainOrdersBS.Find("MainOrderID", MainOrderID);

            CurrentMainOrderID = MainOrderID;
        }

        public void SearchPartDocNumber(string DocText)
        {
            string Search = string.Format("[DocNumber] LIKE '%" + DocText + "%'");

            SearchPartDocNumberBS.Filter = Search;
        }

        public void FindDocNumber(string DocNumber)
        {
            int MainOrderID = -1;
            int MegaOrderID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, MegaOrderID FROM MainOrders" +
                " WHERE DocNumber = '" + DocNumber + "'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    MainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                    MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                }
            }


            int MegaIndex = MegaOrdersBS.Find("MegaOrderID", MegaOrderID);

            if (MegaIndex == -1)
                return;

            MegaOrdersBS.Position = MegaIndex;
            FillMegaPercentageColumn();
            int MainIndex = MainOrdersBS.Find("MainOrderID", MainOrderID);

            if (MainIndex == -1)
                return;

            MainOrdersBS.Position = MainIndex;
            FillMainPercentageColumn();
            FillBatchNumber();
        }

        public void SetDispatched(int MegaOrderID)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int AllPackCount = 0;
            int AllDispPackCount = 0;
            int MainOrderPackCount = 0;
            int MainOrderDispPackCount = 0;
            int DispStatus = 0;
            int MainOrderID = 0;

            decimal FrontsDebtCost = 0;
            decimal DecorDebtCost = 0;
            decimal DispatchedCost = 0;
            decimal DispatchedDebtCost = 0;

            DataTable PackageDetailsDataTable = new DataTable();
            DataTable FrontsOrdersDataTable = new DataTable();
            DataTable DecorOrdersDataTable = new DataTable();
            DataTable PackOrdersDataTable = new DataTable();
            DataTable MainOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ProductType, PackageStatusID, PackageDetails.PackageID, OrderID, Count FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + "))",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackageDetailsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontsOrdersID, Cost, Count FROM FrontsOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorOrderID, Cost, Count FROM DecorOrders" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, MainOrderID, PackageStatusID FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ClientID, DocNumber, DispatchStatusID, ProfilPackCount, TPSPackCount," +
                " FrontsCost, DecorCost, OrderCost, DispatchedCost, DispatchedDebtCost FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(MainOrdersDataTable);

                    for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
                    {
                        MainOrderID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);

                        //кол-во всех упаковок в подзаказе
                        MainOrderPackCount = Convert.ToInt32(MainOrdersDataTable.Rows[i]["ProfilPackCount"]) + Convert.ToInt32(MainOrdersDataTable.Rows[i]["TPSPackCount"]);
                        AllPackCount += MainOrderPackCount;

                        DataRow[] Rows = PackOrdersDataTable.Select("PackageStatusID = 3 AND MainOrderID = " + MainOrderID);

                        MainOrderDispPackCount = Rows.Count();//кол-во отгруженных упаковок
                        AllDispPackCount += MainOrderDispPackCount;

                        DispStatus = 0;
                        //если хоть что-то отгружалось
                        if (MainOrderDispPackCount > 0)
                        {
                            //все упаковки отгружены
                            if (MainOrderPackCount == MainOrderDispPackCount)
                                DispStatus = 2;//отгружено
                            else
                                DispStatus = 3;//отгружено частично
                        }
                        else
                        {
                            //если машина уехала, но упаковки не были отгружены по сканеру
                            if (MainOrderPackCount > MainOrderDispPackCount)
                                DispStatus = 1;//статус не отгружено
                        }

                        MainOrdersDataTable.Rows[i]["DispatchStatusID"] = DispStatus;

                        //debts table
                        using (SqlDataAdapter dDA = new SqlDataAdapter("SELECT * FROM Debts",
                            ConnectionStrings.ZOVOrdersConnectionString))
                        {
                            using (SqlCommandBuilder dCB = new SqlCommandBuilder(dDA))
                            {
                                using (DataTable DT = new DataTable())
                                {
                                    dDA.Fill(DT);

                                    DataRow[] dRows = DT.Select("MainOrderID = " + MainOrderID);

                                    if (dRows.Count() > 0)
                                    {
                                        if (DispStatus == 1)
                                        {
                                            dRows[0].Delete();
                                            dDA.Update(dRows);
                                        }
                                    }
                                    else
                                    {
                                        if (DispStatus != 1)
                                        {
                                            DataRow NewRow = DT.NewRow();
                                            NewRow["DispatchDate"] = ((DataRowView)MegaOrdersBS.Current).Row["DispatchDate"];
                                            NewRow["ClientID"] = MainOrdersDataTable.Rows[i]["ClientID"];
                                            NewRow["MainOrderID"] = MainOrdersDataTable.Rows[i]["MainOrderID"];
                                            NewRow["DocNumber"] = MainOrdersDataTable.Rows[i]["DocNumber"];
                                            DT.Rows.Add(NewRow);

                                            dDA.Update(DT);
                                        }
                                    }
                                }
                            }
                        }

                        FrontsDebtCost = 0;
                        DecorDebtCost = 0;

                        //частично отгружено
                        if (DispStatus == 2)
                        {
                            //вычисление долгов по фасадам
                            DataRow[] PFRows = PackageDetailsDataTable.Select("PackageStatusID <> 3 AND ProductType = 0 AND MainOrderID = " + MainOrderID);

                            foreach (DataRow PFRow in PFRows)
                            {
                                DataRow[] FRows = FrontsOrdersDataTable.Select("FrontsOrdersID = " + Convert.ToInt32(PFRow["OrderID"]));

                                foreach (DataRow Row in FRows)
                                {
                                    FrontsDebtCost += Decimal.Round(Convert.ToDecimal(Row["Cost"]) / Convert.ToDecimal(Row["Count"]) * Convert.ToDecimal(PFRow["Count"]),
                                        1,
                                        MidpointRounding.AwayFromZero);
                                }
                            }

                            //вычисление долгов по декору
                            DataRow[] PDRows = PackageDetailsDataTable.Select("PackageStatusID <> 3 AND ProductType = 1 AND MainOrderID = " + MainOrderID);

                            foreach (DataRow DFRow in PDRows)
                            {
                                DataRow[] DRows = DecorOrdersDataTable.Select("DecorOrderID = " + Convert.ToInt32(DFRow["OrderID"]));

                                foreach (DataRow Row in DRows)
                                {
                                    DecorDebtCost += Decimal.Round(Convert.ToDecimal(Row["Cost"]) / Convert.ToDecimal(Row["Count"]) * Convert.ToDecimal(DFRow["Count"]),
                                        1,
                                        MidpointRounding.AwayFromZero);
                                }
                            }
                        }

                        //не отгружено
                        if (DispStatus == 3)
                        {
                            FrontsDebtCost = Convert.ToDecimal(MainOrdersDataTable.Rows[i]["FrontsCost"]);
                            DecorDebtCost = Convert.ToDecimal(MainOrdersDataTable.Rows[i]["DecorCost"]);
                        }

                        //вычисление долгов для каждого подзаказа
                        MainOrdersDataTable.Rows[i]["DispatchedCost"] =
                            Convert.ToDecimal(MainOrdersDataTable.Rows[i]["OrderCost"]) - (FrontsDebtCost + DecorDebtCost);
                        MainOrdersDataTable.Rows[i]["DispatchedDebtCost"] = FrontsDebtCost + DecorDebtCost;

                        //вычисление долгов для целого заказа
                        DispatchedCost += Convert.ToDecimal(MainOrdersDataTable.Rows[i]["DispatchedCost"]);
                        DispatchedDebtCost += Convert.ToDecimal(MainOrdersDataTable.Rows[i]["DispatchedDebtCost"]);
                    }

                    DA.Update(MainOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, DispatchStatusID, DispatchedCost, DispatchedDebtCost FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DispStatus = 0;
                        //если хоть что-то отгружалось
                        if (AllDispPackCount > 0)
                        {
                            if (AllPackCount == AllDispPackCount)
                                DispStatus = 2;//отгружено
                            else
                                DispStatus = 3;//отгружено частично
                        }
                        else
                        {
                            //если машина уехала, но упаковки не были отгружены по сканеру
                            if (AllPackCount > AllDispPackCount)
                                DispStatus = 1;
                        }

                        DT.Rows[0]["DispatchStatusID"] = DispStatus;
                        DT.Rows[0]["DispatchedCost"] = DispatchedCost;
                        DT.Rows[0]["DispatchedDebtCost"] = DispatchedDebtCost;

                        DA.Update(DT);
                    }
                }
            }


            PackOrdersDataTable.Dispose();
            MainOrdersDataTable.Dispose();
            PackageDetailsDataTable.Dispose();
            FrontsOrdersDataTable.Dispose();
            DecorOrdersDataTable.Dispose();

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        public int SearchPackedOrders()
        {
            bool OrderPacked = false;
            int MainOrderID = 0;
            int PackedOrdersCount = 0;

            using (SqlDataAdapter pDA = new SqlDataAdapter(
                " SELECT PackageID, Packages.MainOrderID, MainOrders.MegaOrderID, PackageStatusID FROM Packages" +
                " INNER JOIN MainOrders ON (Packages.MainOrderID = MainOrders.MainOrderID AND MainOrders.MegaOrderID = 0)",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable pDT = new DataTable())
                {
                    pDA.Fill(pDT);

                    using (SqlDataAdapter mDA = new SqlDataAdapter(
                        " SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = 0",
                        ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        using (DataTable mDT = new DataTable())
                        {
                            mDA.Fill(mDT);

                            for (int i = 0; i < mDT.Rows.Count; i++)
                            {
                                OrderPacked = true;
                                MainOrderID = Convert.ToInt32(mDT.Rows[i]["MainOrderID"]);

                                DataRow[] pRows = pDT.Select("MainOrderID = " + MainOrderID);

                                if (pRows.Count() < 1)
                                    continue;

                                foreach (DataRow item in pRows)
                                {
                                    if (Convert.ToInt32(item["PackageStatusID"]) == 0 || Convert.ToInt32(item["PackageStatusID"]) == 3)
                                    {
                                        OrderPacked = false;
                                        break;
                                    }
                                }

                                if (OrderPacked)
                                {
                                    PackedOrdersCount++;
                                }
                            }
                        }
                    }
                }
            }

            return PackedOrdersCount;
        }

        public string DebtDocNumber
        {
            get
            {
                if (((DataRowView)MainOrdersBS.Current != null
                    && ((DataRowView)MainOrdersBS.Current).Row["DebtDocNumber"] != DBNull.Value))
                    return ((DataRowView)MainOrdersBS.Current).Row["DebtDocNumber"].ToString();
                return string.Empty;
            }
        }

        public string ReorderDocNumber
        {
            get
            {
                if (((DataRowView)MainOrdersBS.Current != null
                    && ((DataRowView)MainOrdersBS.Current).Row["ReorderDocNumber"] != DBNull.Value))
                    return ((DataRowView)MainOrdersBS.Current).Row["ReorderDocNumber"].ToString();
                return string.Empty;
            }
        }

        public int FindMainOrder(string DocNumber)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID FROM MainOrders" +
                " WHERE DocNumber = '" + DocNumber + "'",
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

        public void SavePackages(int DispatchID)
        {
            DataTable DT = PackagesDT.Copy();
            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                if (Convert.ToBoolean(PackagesDT.Rows[i]["CheckBoxColumn"]))
                    DT.Rows[i]["DispatchID"] = DispatchID;
                else
                    DT.Rows[i]["DispatchID"] = DBNull.Value;
            }

            //DT.Columns.Remove("CheckBoxColumn");
            string SelectCommand = "SELECT TOP 0 PackageID, DispatchID FROM Packages";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DT);
                }
            }
        }

        public void SavePackages(int MainOrderID, int DispatchID)
        {
            //DT.Columns.Remove("CheckBoxColumn");
            string SelectCommand = @"SELECT PackageID, DispatchID FROM Packages WHERE MainOrderID=" + MainOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["DispatchID"] = DispatchID;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }

        }

        public bool SetMainOrderDispatchStatus()
        {
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                DataRow[] DRows = AllPackagesDT.Select("DispatchID IS NOT NULL AND MainOrderID=" + MainOrdersDT.Rows[i]["MainOrderID"]);
                DataRow[] MRows = AllPackagesDT.Select("MainOrderID=" + MainOrdersDT.Rows[i]["MainOrderID"]);
                if (DRows.Count() > 0 && DRows.Count() == MRows.Count())
                    MainOrdersDT.Rows[i]["CheckBoxColumn"] = true;
            }
            return true;
        }

        public bool SetPackageDispatchStatus()
        {
            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                if (PackagesDT.Rows[i]["DispatchID"] != DBNull.Value)
                    PackagesDT.Rows[i]["CheckBoxColumn"] = true;
            }
            return true;
        }

        public bool IsPackagesCheck()
        {
            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                if (Convert.ToBoolean(PackagesDT.Rows[i]["CheckBoxColumn"]))
                    return true;
            }
            return false;
        }

        public void CheckPackages()
        {
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                FlagPackages(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]),
                    Convert.ToBoolean(MainOrdersDT.Rows[i]["CheckBoxColumn"]));
            }
        }

        public void FlagPackages(int MainOrderID, bool Checked)
        {
            DataRow[] Rows = PackagesDT.Select("MainOrderID=" + MainOrderID);
            foreach (DataRow item in Rows)
            {
                item["CheckBoxColumn"] = Checked;
            }
        }

        public void MoveToMegaOrder(int MegaOrderID)
        {
            MegaOrdersBS.Position = MegaOrdersBS.Find("MegaOrderID", MegaOrderID);
        }

        public void MoveToMainOrder(int MainOrderID)
        {
            MainOrdersBS.Position = MainOrdersBS.Find("MainOrderID", MainOrderID);
        }

        public bool IsMainOrderDispatched(int MainOrderID)
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

        public void DispatchDebts(int MegaOrderID)
        {
            DataTable DebtsDT = new DataTable();
            DataTable ReOrdersDT = new DataTable();
            SqlDataAdapter DebtsDA;
            SqlDataAdapter ReOrdersDA;
            SqlCommandBuilder DebtsCB;
            string SelectCommand = string.Empty;

            SelectCommand = @"SELECT MainOrderID, DocNumber, ReorderDocNumber FROM MainOrders
                WHERE ReorderDocNumber IS NOT NULL AND MegaOrderID = " + MegaOrderID;
            DebtsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString);
            DebtsCB = new SqlCommandBuilder(DebtsDA);
            DebtsDA.Fill(DebtsDT);

            SelectCommand = @"SELECT MainOrderID, DocNumber FROM MainOrders
                WHERE DocNumber IN (SELECT ReorderDocNumber FROM MainOrders
                WHERE ReorderDocNumber IS NOT NULL AND MegaOrderID = " + MegaOrderID + ")";
            ReOrdersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString);
            ReOrdersDA.Fill(ReOrdersDT);

            for (int i = 0; i < DebtsDT.Rows.Count; i++)
            {
                int DebtMainOrderID = Convert.ToInt32(DebtsDT.Rows[i]["MainOrderID"]);
                string ReorderDocNumber = DebtsDT.Rows[i]["ReorderDocNumber"].ToString();
                DataRow[] rows = ReOrdersDT.Select("DocNumber='" + ReorderDocNumber + "'");
                if (rows.Count() == 0)
                    continue;
                int ReOrderMainOrderID = Convert.ToInt32(rows[0]["MainOrderID"]);
                if (IsMainOrderDispatched(ReOrderMainOrderID))
                    SetMainOrderDispatch(DebtMainOrderID);
            }
            FillMegaPercentageColumn();
        }

        public void SetMainOrderDispatch(int MainOrderID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpeditionDateTime, DispatchDateTime," +
                " PackUserID, StoreUserID, ExpUserID, DispUserID FROM Packages" +
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
                                if (Convert.ToInt32(DT.Rows[i]["PackageStatusID"]) == 3)
                                    continue;
                                DT.Rows[i]["PackageStatusID"] = 3;
                                if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PackingDateTime"] = CurrentDate;
                                if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                    DT.Rows[i]["StorageDateTime"] = CurrentDate;
                                if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    DT.Rows[i]["ExpeditionDateTime"] = CurrentDate;
                                if (DT.Rows[i]["DispatchDateTime"] == DBNull.Value)
                                    DT.Rows[i]["DispatchDateTime"] = CurrentDate;

                                if (DT.Rows[i]["PackUserID"] == DBNull.Value)
                                    DT.Rows[i]["PackUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["StoreUserID"] == DBNull.Value)
                                    DT.Rows[i]["StoreUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["ExpUserID"] == DBNull.Value)
                                    DT.Rows[i]["ExpUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["DispUserID"] == DBNull.Value)
                                    DT.Rows[i]["DispUserID"] = Security.CurrentUserID;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
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
                                DT.Rows[0]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 2;
                            }

                            if (FactoryID == 2)
                            {
                                if (Convert.ToInt32(DT.Rows[0]["TPSDispatchStatusID"]) == 2)
                                    return;
                                DT.Rows[0]["TPSProductionStatusID"] = 1;
                                DT.Rows[0]["TPSStorageStatusID"] = 1;
                                DT.Rows[0]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[0]["TPSDispatchStatusID"] = 2;
                            }

                            if (FactoryID == 0)
                            {
                                if (Convert.ToInt32(DT.Rows[0]["ProfilDispatchStatusID"]) == 2
                                    && Convert.ToInt32(DT.Rows[0]["TPSDispatchStatusID"]) == 2)
                                    return;
                                DT.Rows[0]["ProfilProductionStatusID"] = 1;
                                DT.Rows[0]["ProfilStorageStatusID"] = 1;
                                DT.Rows[0]["ProfilExpeditionStatusID"] = 1;
                                DT.Rows[0]["ProfilDispatchStatusID"] = 2;
                                DT.Rows[0]["TPSProductionStatusID"] = 1;
                                DT.Rows[0]["TPSStorageStatusID"] = 1;
                                DT.Rows[0]["TPSExpeditionStatusID"] = 1;
                                DT.Rows[0]["TPSDispatchStatusID"] = 2;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public bool FindDispatchInPermits(int DispatchID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Permits WHERE ZOVDispatchID = " + DispatchID,
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return true;
                }
            }
            return false;
        }

        public void ChangeDispatchDate(int[] Dispatches, DateTime PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT * FROM Dispatch WHERE DispatchID IN (" + string.Join(",", Dispatches) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < DT.Rows.Count; i++)
                            DT.Rows[i]["PrepareDispatchDateTime"] = PrepareDispatchDateTime;
                        DA.Update(DT);
                    }
                }
            }
        }
    }










    public class ZOVDebtsDispatchManager
    {
        int CurrentMainOrderID = -1;

        public ZOVExpeditionFrontsOrders PackedMainOrdersFrontsOrders = null;
        public ZOVExpeditionDecorOrders PackedMainOrdersDecorOrders = null;

        public PercentageDataGrid PackagesDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        public DataTable PackagesDataTable = null;
        private DataTable PackageStatusesDataTable = null;

        public BindingSource PackagesBindingSource = null;
        public BindingSource PackageStatusesBindingSource = null;

        private DataGridViewComboBoxColumn PackageStatusesColumn = null;

        private SqlDataAdapter PackagesDA = null;
        private SqlCommandBuilder PackagesCB = null;

        public ZOVDebtsDispatchManager(
            ref PercentageDataGrid tPackagesDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl)
        {
            PackagesDataGrid = tPackagesDataGrid;
            OrdersTabControl = tOrdersTabControl;

            PackedMainOrdersFrontsOrders = new ZOVExpeditionFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);

            PackedMainOrdersDecorOrders = new ZOVExpeditionDecorOrders(ref tMainOrdersDecorOrdersDataGrid);

            Initialize();
        }

        #region Initialize
        private void Create()
        {
            PackagesDataTable = new DataTable();
            PackageStatusesDataTable = new DataTable();

            PackagesBindingSource = new BindingSource();
            PackageStatusesBindingSource = new BindingSource();
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

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }

            PackagesDA = new SqlDataAdapter("SELECT TOP 0 * FROM Packages",
                ConnectionStrings.ZOVOrdersConnectionString);
            PackagesCB = new SqlCommandBuilder(PackagesDA);
            PackagesDA.Fill(PackagesDataTable);
        }

        private void Binding()
        {
            PackagesBindingSource.DataSource = PackagesDataTable;
            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackageStatusesBindingSource.DataSource = PackageStatusesDataTable;
        }

        private void CreateColumns()
        {
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
            PackagesDataGrid.Columns["PrintedCount"].Visible = false;

            if (PackagesDataGrid.Columns.Contains("ProductType"))
                PackagesDataGrid.Columns["ProductType"].Visible = false;
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
            PackagesDataGrid.Columns["DispatchDateTime"].HeaderText = "    Дата\r\nотгрузки";
            PackagesDataGrid.Columns["PackageID"].HeaderText = "ID";
            PackagesDataGrid.Columns["TrayID"].HeaderText = " №\r\nпод.";

            PackagesDataGrid.Columns["PackNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackNumber"].Width = 70;
            PackagesDataGrid.Columns["PackageStatusesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["PackageStatusesColumn"].Width = 140;
            //PackagesDataGrid.Columns["PrintedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //PackagesDataGrid.Columns["PrintedCount"].Width = 140;
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
            PackagesDataGrid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["TrayID"].Width = 100;
            PackagesDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["MainOrderID"].Width = 100;

            PackagesDataGrid.AutoGenerateColumns = false;

            PackagesDataGrid.Columns["PackNumber"].DisplayIndex = 0;
            PackagesDataGrid.Columns["PackageStatusesColumn"].DisplayIndex = 1;
            PackagesDataGrid.Columns["TrayID"].DisplayIndex = 3;
            PackagesDataGrid.Columns["PrintDateTime"].DisplayIndex = 4;
            PackagesDataGrid.Columns["PackingDateTime"].DisplayIndex = 5;
            PackagesDataGrid.Columns["StorageDateTime"].DisplayIndex = 6;
            PackagesDataGrid.Columns["ExpeditionDateTime"].DisplayIndex = 7;
            PackagesDataGrid.Columns["DispatchDateTime"].DisplayIndex = 8;
            PackagesDataGrid.Columns["PackageID"].DisplayIndex = 9;
            //PackagesDataGrid.Columns["PrintedCount"].DisplayIndex = 8;
            //PackagesDataGrid.Columns["MainOrderID"].DisplayIndex = 1;

            //for (int i = 0; i < PackagesDataGrid.Rows.Count; i++)
            //{
            //    int PackageStatusID = Convert.ToInt32(PackagesDataGrid.Rows[i].Cells["PackageStatusID"].Value);

            //    if (PackageStatusID == 1)
            //    {
            //        PackagesDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
            //    }
            //}
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            PackagesGridSetting();
        }
        #endregion


        #region Filter

        public void FilterPackagesByMainOrder(int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND Packages.FactoryID = " + FactoryID;

            PackagesDA.Dispose();
            PackagesCB.Dispose();
            PackagesDataTable.Clear();

            string SelectionCommand = "SELECT * FROM Packages " +
                " WHERE MainOrderID = " + MainOrderID + FactoryFilter;

            PackagesDA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString);
            PackagesCB = new SqlCommandBuilder(PackagesDA);
            PackagesDA.Fill(PackagesDataTable);

            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
            //    ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(PackagesDataTable);
            //}
            PackagesDataTable.DefaultView.Sort = "PackNumber ASC";
            PackagesBindingSource.MoveFirst();
        }

        public void FilterProductsByPackage(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return;
            CurrentMainOrderID = MainOrderID;
            OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.FilterByMainOrder(MainOrderID, FactoryID);
            OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.FilterByMainOrder(MainOrderID, FactoryID);
        }
        #endregion

        public void DispPackage(int[] Packages, DateTime DispatchDateTime)
        {
            DataRow[] Rows = PackagesDataTable.Select("PackageID IN (" + string.Join(",", Packages) + ")");
            if (Rows.Count() < 1)
                return;

            foreach (DataRow row in Rows)
            {
                row["PackageStatusID"] = 3;
                row["DispatchDateTime"] = DispatchDateTime;
                if (row["PrintDateTime"] == DBNull.Value)
                    row["PrintDateTime"] = DispatchDateTime;
                if (row["PackingDateTime"] == DBNull.Value)
                    row["PackingDateTime"] = DispatchDateTime;
                if (row["StorageDateTime"] == DBNull.Value)
                    row["StorageDateTime"] = DispatchDateTime;
                if (row["ExpeditionDateTime"] == DBNull.Value)
                    row["ExpeditionDateTime"] = DispatchDateTime;
            }
        }

        public void SavePackages()
        {
            PackagesDA.Update(PackagesDataTable);
        }
    }







    public class ZOVDispatch
    {
        DataTable AllDispatchFrontsWeightDT;
        DataTable AllDispatchDecorWeightDT;
        DataTable AllMainOrdersSquareDT;
        DataTable AllMainOrdersFrontsWeightDT;
        DataTable AllMainOrdersDecorWeightDT;
        DataTable AllMegaBatchNumbersDT;
        DataTable DispatchInfoDT;
        DataTable RealDispTimeDT;
        DataTable DispatchDT;
        DataTable DispatchDatesDT;
        DataTable DispatchContentDT;

        BindingSource DispatchBS;
        BindingSource DispatchDatesBS;
        BindingSource DispatchContentBS;

        public ZOVDispatch()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            AllDispatchFrontsWeightDT = new DataTable();
            AllDispatchDecorWeightDT = new DataTable();
            AllMainOrdersSquareDT = new DataTable();
            AllMainOrdersFrontsWeightDT = new DataTable();
            AllMainOrdersDecorWeightDT = new DataTable();
            AllMegaBatchNumbersDT = new DataTable();
            DispatchInfoDT = new DataTable();
            RealDispTimeDT = new DataTable();
            DispatchContentDT = new DataTable();
            DispatchDT = new DataTable();
            DispatchDT.Columns.Add(new DataColumn(("DispPackagesCount"), System.Type.GetType("System.String")));
            DispatchDT.Columns.Add(new DataColumn(("Weight"), System.Type.GetType("System.Decimal")));
            DispatchDT.Columns.Add(new DataColumn(("DispatchStatus"), System.Type.GetType("System.String")));
            DispatchDT.Columns.Add(new DataColumn(("RealDispDateTime"), System.Type.GetType("System.DateTime")));
            DispatchDatesDT = new DataTable();
            DispatchDatesDT.Columns.Add(new DataColumn(("WeekNumber"), System.Type.GetType("System.String")));

            DispatchBS = new BindingSource();
            DispatchDatesBS = new BindingSource();
            DispatchContentBS = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TOP 0 Dispatch.*, infiniu2_zovreference.dbo.Clients.ClientName, 
                ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
                INNER JOIN infiniu2_zovreference.dbo.Clients ON Dispatch.ClientID = infiniu2_zovreference.dbo.Clients.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchDT);
            }
            SelectCommand = "SELECT TOP 0 PrepareDispatchDateTime FROM Dispatch ORDER BY PrepareDispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchDatesDT);
            }
            //SelectCommand = "SELECT TOP 0 MegaOrderID, MegaOrders.ClientID, OrderNumber FROM MegaOrders" +
            //    " INNER JOIN infiniu2_zovreference.dbo.Clients ON MegaOrders.ClientID=infiniu2_zovreference.dbo.Clients.ClientID";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    DA.Fill(DispatchContentDT);
            //}

            SelectCommand = "SELECT TOP 0 MainOrders.MegaOrderID, MainOrders.ClientID, MainOrders.DocNumber, MainOrders.DoNotDispatch, MainOrders.MainOrderID FROM MainOrders" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID=MegaOrders.MegaOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchContentDT);
            }
            DispatchContentDT.Columns.Add(new DataColumn("MegaBatchID", Type.GetType("System.Int32")));
            DispatchContentDT.Columns.Add(new DataColumn("Weight", Type.GetType("System.Decimal")));
            DispatchContentDT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            DispatchContentDT.Columns.Add(new DataColumn("AllPackCount", Type.GetType("System.Int32")));
            DispatchContentDT.Columns.Add(new DataColumn("PackPercentage", Type.GetType("System.Decimal")));
            DispatchContentDT.Columns.Add(new DataColumn("StorePercentage", Type.GetType("System.Decimal")));
            DispatchContentDT.Columns.Add(new DataColumn("ExpPercentage", Type.GetType("System.Decimal")));
            DispatchContentDT.Columns.Add(new DataColumn("DispPercentage", Type.GetType("System.Decimal")));
        }

        public void GetMainOrdersSquareAndWeight(DateTime PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT Packages.DispatchID, FrontsOrders.MainOrderID, (FrontsOrders.Square*PackageDetails.Count/FrontsOrders.Count) AS Square
                    FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllMainOrdersSquareDT.Clear();
                DA.Fill(AllMainOrdersSquareDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, FrontsOrders.MainOrderID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllMainOrdersFrontsWeightDT.Clear();
                DA.Fill(AllMainOrdersFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, DecorOrders.MainOrderID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllMainOrdersDecorWeightDT.Clear();
                DA.Fill(AllMainOrdersDecorWeightDT);
            }

            SelectCommand = @"SELECT Packages.DispatchID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllDispatchFrontsWeightDT.Clear();
                DA.Fill(AllDispatchFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllDispatchDecorWeightDT.Clear();
                DA.Fill(AllDispatchDecorWeightDT);
            }
            SelectCommand = @"SELECT PackageID, PackageStatusID, DispatchID
                    FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DispatchInfoDT.Clear();
                DA.Fill(DispatchInfoDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, MAX(DispatchDateTime) AS DispatchDateTime
                    FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "') GROUP By DispatchID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                RealDispTimeDT.Clear();
                DA.Fill(RealDispTimeDT);
            }
        }

        public void GetMegaBatchNumbers(DateTime PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT Batch.MegaBatchID, BatchDetails.MainOrderID FROM BatchDetails 
                INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID 
                WHERE BatchDetails.MainOrderID IN (SELECT MainOrderID FROM Packages WHERE Packages.DispatchID IN 
                (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "'))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllMegaBatchNumbersDT.Clear();
                DA.Fill(AllMegaBatchNumbersDT);
            }
        }

        private decimal GetSquare(int DispatchID, int MainOrderID)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            decimal Square = 0;
            DataRow[] rows = AllMainOrdersSquareDT.Select("DispatchID=" + DispatchID + " AND MainOrderID=" + MainOrderID);
            foreach (DataRow item in rows)
            {
                Square += Convert.ToDecimal(item["Square"]);
            }
            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
            sw.Stop();
            double G = sw.Elapsed.Milliseconds;

            return Square;
        }

        private decimal GetWeight(int DispatchID, int MainOrderID)
        {
            decimal Weight = 0;
            DataRow[] rows = AllMainOrdersFrontsWeightDT.Select("DispatchID=" + DispatchID + " AND MainOrderID=" + MainOrderID);

            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]) + Convert.ToDecimal(item["Square"]) * Convert.ToDecimal(0.7);
            }
            rows = AllMainOrdersDecorWeightDT.Select("DispatchID=" + DispatchID + " AND MainOrderID=" + MainOrderID);
            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]);
            }
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        private decimal GetWeight(int DispatchID)
        {
            decimal Weight = 0;
            DataRow[] rows = AllDispatchFrontsWeightDT.Select("DispatchID=" + DispatchID);
            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]) + Convert.ToDecimal(item["Square"]) * Convert.ToDecimal(0.7);
            }
            rows = AllDispatchDecorWeightDT.Select("DispatchID=" + DispatchID);
            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]);
            }
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        private int GetMegaBatchID(int MainOrderID)
        {
            int MegaBatchID = 0;
            DataRow[] rows = AllMegaBatchNumbersDT.Select("MainOrderID=" + MainOrderID);
            if (rows.Count() > 0)
                MegaBatchID = Convert.ToInt32(rows[0]["MegaBatchID"]);

            return MegaBatchID;
        }

        public void FillPercColumns(int DispatchID)
        {
            DataTable DT = new DataTable();

            int PackedCount = 0;
            int StoreCount = 0;
            int DispCount = 0;
            int ExpCount = 0;
            int AllCount = 0;

            decimal PackPercentage = 0;
            decimal StorePercentage = 0;
            decimal ExpPercentage = 0;
            decimal DispPercentage = 0;

            decimal PackProgressVal = 0;
            decimal StoreProgressVal = 0;
            decimal ExpProgressVal = 0;
            decimal DispProgressVal = 0;

            decimal d1 = 0;
            decimal d2 = 0;
            decimal d3 = 0;
            decimal d4 = 0;

            for (int i = 0; i < DispatchContentDT.Rows.Count; i++)
            {
                int MainOrderID = Convert.ToInt32(DispatchContentDT.Rows[i]["MainOrderID"]);

                PackedCount = 0;
                StoreCount = 0;
                DispCount = 0;
                ExpCount = 0;
                AllCount = 0;

                PackPercentage = 0;
                StorePercentage = 0;
                ExpPercentage = 0;
                DispPercentage = 0;

                PackProgressVal = 0;
                StoreProgressVal = 0;
                ExpProgressVal = 0;
                DispProgressVal = 0;

                d1 = 0;
                d2 = 0;
                d3 = 0;
                d4 = 0;

                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageID, PackageStatusID, FactoryID, DispatchID FROM Packages
                    WHERE MainOrderID=" + MainOrderID + " AND DispatchID=" + DispatchID, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);
                }
                foreach (DataRow item in DT.Rows)
                {
                    if (Convert.ToInt32(item["PackageStatusID"]) == 1)
                        PackedCount++;
                    if (Convert.ToInt32(item["PackageStatusID"]) == 2)
                        StoreCount++;
                    if (Convert.ToInt32(item["PackageStatusID"]) == 4)
                        ExpCount++;
                    if (Convert.ToInt32(item["PackageStatusID"]) == 3)
                        DispCount++;
                    AllCount++;
                }

                if (AllCount == 0 && DispCount > 0)
                    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                PackProgressVal = 0;
                StoreProgressVal = 0;
                ExpProgressVal = 0;
                DispProgressVal = 0;

                if (AllCount > 0)
                    PackProgressVal = Convert.ToDecimal(Convert.ToDecimal(PackedCount) / Convert.ToDecimal(AllCount));

                if (AllCount > 0)
                    StoreProgressVal = Convert.ToDecimal(Convert.ToDecimal(StoreCount) / Convert.ToDecimal(AllCount));

                if (AllCount > 0)
                    ExpProgressVal = Convert.ToDecimal(Convert.ToDecimal(ExpCount) / Convert.ToDecimal(AllCount));

                if (AllCount > 0)
                    DispProgressVal = Convert.ToDecimal(Convert.ToDecimal(DispCount) / Convert.ToDecimal(AllCount));

                d1 = PackProgressVal * 100;
                d2 = StoreProgressVal * 100;
                d4 = ExpProgressVal * 100;
                d3 = DispProgressVal * 100;

                //PackPercentage = Convert.ToInt32(Math.Truncate(d1));
                //StorePercentage = Convert.ToInt32(Math.Truncate(d2));
                //ExpPercentage = Convert.ToInt32(Math.Truncate(d4));
                //DispPercentage = Convert.ToInt32(Math.Truncate(d3));

                PackPercentage = Decimal.Round(d1, 1, MidpointRounding.AwayFromZero);
                StorePercentage = Decimal.Round(d2, 1, MidpointRounding.AwayFromZero);
                ExpPercentage = Decimal.Round(d4, 1, MidpointRounding.AwayFromZero);
                DispPercentage = Decimal.Round(d3, 1, MidpointRounding.AwayFromZero);

                DispatchContentDT.Rows[i]["Square"] = GetSquare(DispatchID, MainOrderID);
                DispatchContentDT.Rows[i]["Weight"] = GetWeight(DispatchID, MainOrderID);
                DispatchContentDT.Rows[i]["MegaBatchID"] = GetMegaBatchID(MainOrderID);
                DispatchContentDT.Rows[i]["AllPackCount"] = AllCount;
                DispatchContentDT.Rows[i]["PackPercentage"] = PackPercentage;
                DispatchContentDT.Rows[i]["StorePercentage"] = StorePercentage;
                DispatchContentDT.Rows[i]["ExpPercentage"] = ExpPercentage;
                DispatchContentDT.Rows[i]["DispPercentage"] = DispPercentage;
            }
            DT.Dispose();
        }

        private void Binding()
        {
            DispatchBS.DataSource = DispatchDT;
            DispatchDatesBS.DataSource = DispatchDatesDT;
            DispatchContentBS.DataSource = DispatchContentDT;
        }

        public int CurrentDispatchID
        {
            get
            {
                if (DispatchBS.Count == 0 || ((DataRowView)DispatchBS.Current).Row["DispatchID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)DispatchBS.Current).Row["DispatchID"]);
            }
        }

        public object CurrentDispatchDate
        {
            get
            {
                if (DispatchDatesBS.Count == 0 || ((DataRowView)DispatchDatesBS.Current).Row["PrepareDispatchDateTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)DispatchDatesBS.Current).Row["PrepareDispatchDateTime"];
            }
        }

        public bool HasDispatchDates
        {
            get
            {
                return DispatchDatesBS.Count > 0;
            }
        }

        public bool HasDispatch
        {
            get
            {
                return DispatchBS.Count > 0;
            }
        }

        public bool HasPackages(int DispatchID)
        {
            string SelectCommand = @"SELECT PackageID, PackageStatusID FROM Packages WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    return (DA.Fill(DT) > 0);
                }
            }
        }

        public BindingSource DispatchList
        {
            get { return DispatchBS; }
        }

        public BindingSource DispatchDatesList
        {
            get { return DispatchDatesBS; }
        }

        public BindingSource DispatchContentList
        {
            get { return DispatchContentBS; }
        }

        public object GetPrepareDispatchDateTime(int DispatchID)
        {
            object PrepareDispatchDateTime = DBNull.Value;
            string SelectCommand = @"SELECT PrepareDispatchDateTime
                FROM Dispatch WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["PrepareDispatchDateTime"] != DBNull.Value)
                            PrepareDispatchDateTime = DT.Rows[0]["PrepareDispatchDateTime"];
                    }
                }
            }
            return PrepareDispatchDateTime;
        }

        public void GetRealDispDateTime(int DispatchID, ref object RealDispDateTime, ref object DispUserID)
        {
            string SelectCommand = @"SELECT MAX(DispatchDateTime) AS DispatchDateTime, DispUserID
                FROM Packages WHERE DispatchID = " + DispatchID + " GROUP BY DispUserID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["DispatchDateTime"] != DBNull.Value)
                            RealDispDateTime = DT.Rows[0]["DispatchDateTime"];
                        if (DT.Rows[0]["DispUserID"] != DBNull.Value)
                            DispUserID = DT.Rows[0]["DispUserID"];
                    }
                }
            }
        }

        public object GetRealDispDateTime(int DispatchID)
        {
            object RealDispDateTime = DBNull.Value;
            DataRow[] rows = RealDispTimeDT.Select("DispatchID=" + DispatchID);

            if (rows.Count() > 0 && rows[0]["DispatchDateTime"] != DBNull.Value)
                RealDispDateTime = rows[0]["DispatchDateTime"];
            //            string SelectCommand = @"SELECT MAX(DispatchDateTime) AS DispatchDateTime
            //				FROM Packages WHERE DispatchID = " + DispatchID;
            //            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            //            {
            //                using (DataTable DT = new DataTable())
            //                {
            //                    if (DA.Fill(DT) > 0)
            //                    {
            //                        if (DT.Rows[0]["DispatchDateTime"] != DBNull.Value)
            //                            RealDispDateTime = DT.Rows[0]["DispatchDateTime"];
            //                    }
            //                }
            //            }
            return RealDispDateTime;
        }

        private void GetDispPackagesInfo(int DispatchID, ref int DispPackagesCount, ref int PackagesCount)
        {
            DataRow[] rows1 = DispatchInfoDT.Select("DispatchID=" + DispatchID);
            DataRow[] rows2 = DispatchInfoDT.Select("DispatchID=" + DispatchID + " AND PackageStatusID=3");

            if (rows2.Count() > 0)
            {
                DispPackagesCount = rows2.Count();
            }
            if (rows1.Count() > 0)
            {
                PackagesCount = rows1.Count();
            }
            //string SelectCommand = @"SELECT PackageID, PackageStatusID FROM Packages WHERE DispatchID = " + DispatchID;
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        if (DA.Fill(DT) > 0)
            //        {
            //            DispPackagesCount = DT.Select("PackageStatusID = 3").Count();
            //            PackagesCount = DT.Rows.Count;
            //        }
            //    }
            //}
        }

        public bool IsDispatchCanExp(int DispatchID)
        {
            string SelectCommand = @"SELECT PackageID, PackageStatusID FROM Packages WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int StorePackagesCount = DT.Select("PackageStatusID = 2 OR PackageStatusID = 3 OR PackageStatusID = 4").Count();
                        int AllPackagesCoutnt = DT.Rows.Count;
                        return AllPackagesCoutnt == StorePackagesCount;
                    }
                    else
                        return false;
                }
            }
        }

        public bool IsDispatchEmpty(int DispatchID)
        {
            string SelectCommand = @"SELECT PackageID FROM Packages WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return false;
                    else
                        return true;
                }
            }
        }

        public bool IsMainOrderInDispatch(int MainOrderID, ref int DispatchID)
        {
            string SelectCommand = @"SELECT * FROM Packages WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (DT.Rows[i]["DispatchID"] != DBNull.Value)
                            {
                                DispatchID = Convert.ToInt32(DT.Rows[i]["DispatchID"]);
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool IsDoNotDispatch(int MainOrderID)
        {
            string SelectCommand = @"SELECT DoNotDispatch FROM MainOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {

                        if (DT.Rows[0]["DoNotDispatch"] != DBNull.Value && Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]))
                            return true;
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool IsPackageInDispatch(int PackageID, ref int DispatchID)
        {
            string SelectCommand = @"SELECT * FROM Packages WHERE PackageID = " + PackageID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["DispatchID"] != DBNull.Value)
                        {
                            DispatchID = Convert.ToInt32(DT.Rows[0]["DispatchID"]);
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private void FillDispPackagesInfo()
        {
            bool IsDispConfirm = false;
            bool IsExpConfirm = false;
            int DispPackagesCount = 0;
            int PackagesCount = 0;
            string Status = string.Empty;
            object RealDispDateTime = DBNull.Value;

            for (int i = 0; i < DispatchDT.Rows.Count; i++)
            {
                DispPackagesCount = 0;
                PackagesCount = 0;
                if (DispatchDT.Rows[i]["ConfirmExpDateTime"] != DBNull.Value)
                    IsExpConfirm = true;
                else
                    IsExpConfirm = false;
                if (DispatchDT.Rows[i]["ConfirmDispDateTime"] != DBNull.Value)
                    IsDispConfirm = true;
                else
                    IsDispConfirm = false;
                GetDispPackagesInfo(Convert.ToInt32(DispatchDT.Rows[i]["DispatchID"]), ref DispPackagesCount, ref PackagesCount);
                if (PackagesCount > 0)
                {
                    if (IsExpConfirm)
                    {
                        Status = "Утверждена к эксп-ции";
                        if (IsDispConfirm)
                        {
                            Status = "Утверждена к отгрузке";
                            if (DispPackagesCount > 0 && PackagesCount == DispPackagesCount)
                                Status = "Отгружена";
                        }
                    }
                    else
                        Status = "Ожидает утверждения к эксп-ции";

                }
                else
                {
                    Status = "Отгрузка пуста";
                }

                if (PackagesCount == DispPackagesCount)
                {
                    RealDispDateTime = GetRealDispDateTime(Convert.ToInt32(DispatchDT.Rows[i]["DispatchID"]));
                    DispatchDT.Rows[i]["RealDispDateTime"] = RealDispDateTime;
                }
                DispatchDT.Rows[i]["DispatchStatus"] = Status;
                DispatchDT.Rows[i]["DispPackagesCount"] = DispPackagesCount + " / " + PackagesCount;
                DispatchDT.Rows[i]["Weight"] = GetWeight(Convert.ToInt32(DispatchDT.Rows[i]["DispatchID"]));
            }
        }

        private int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void FillWeekNumber()
        {
            for (int i = 0; i < DispatchDatesDT.Rows.Count; i++)
            {
                DispatchDatesDT.Rows[i]["WeekNumber"] = GetWeekNumber(Convert.ToDateTime(DispatchDatesDT.Rows[i]["PrepareDispatchDateTime"])) + " к.н.";
            }
        }

        public void RemoveDispatch(int DispatchID)
        {
            string SelectCommand = @"SELECT * FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ClearDispatch()
        {
            DispatchDT.Clear();
        }

        public void ClearDispatchDates()
        {
            DispatchDatesDT.Clear();
        }

        public void ClearDispatchContent()
        {
            DispatchContentDT.Clear();
        }

        public void ChangeDispatchDate(int DispatchID, object PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT DispatchID, PrepareDispatchDateTime, 
                ConfirmExpDateTime, ConfirmExpUserID, ConfirmDispDateTime, ConfirmDispUserID FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (PrepareDispatchDateTime != null)
                            {
                                DT.Rows[0]["PrepareDispatchDateTime"] = Convert.ToDateTime(PrepareDispatchDateTime);
                                DT.Rows[0]["ConfirmDispDateTime"] = DBNull.Value;
                                DT.Rows[0]["ConfirmDispUserID"] = DBNull.Value;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SaveConfirmExpInfo(int[] DispatchID, bool Confirm)
        {
            //WHERE CAST(PrepareDispatchDateTime AS Date) = 
            //        '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')"
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT PackageID, PackageStatusID, DispatchID FROM Packages WHERE DispatchID IN (" + string.Join(",", DispatchID) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            DateTime CurrentDate = Security.GetCurrentDate();
            SelectCommand = "SELECT DispatchID, ConfirmExpUserID, ConfirmExpDateTime FROM Dispatch WHERE DispatchID IN (" + string.Join(",", DispatchID) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DispatchID.Count(); i++)
                            {
                                if (Confirm)
                                {
                                    int D = DispatchID[i];
                                    int StorePackagesCount = TempDT.Select("DispatchID=" + DispatchID[i] + " AND (PackageStatusID = 2 OR PackageStatusID = 3 OR PackageStatusID = 4)").Count();
                                    int AllPackagesCount = TempDT.Select("DispatchID=" + DispatchID[i]).Count();
                                    if (AllPackagesCount > 0 && AllPackagesCount == StorePackagesCount)
                                    {
                                        DataRow[] rows1 = DT.Select("DispatchID=" + DispatchID[i]);
                                        if (rows1.Count() > 0)
                                        {
                                            rows1[0]["ConfirmExpDateTime"] = CurrentDate;
                                            rows1[0]["ConfirmExpUserID"] = Security.CurrentUserID;
                                        }
                                    }

                                }
                                else
                                {
                                    DataRow[] rows1 = DT.Select("DispatchID=" + DispatchID[i]);
                                    if (rows1.Count() > 0)
                                    {
                                        rows1[0]["ConfirmExpDateTime"] = DBNull.Value;
                                        rows1[0]["ConfirmExpUserID"] = DBNull.Value;
                                    }
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            TempDT.Dispose();
        }

        public void SaveConfirmExpInfo(DateTime PrepareDispatchDateTime, bool Confirm)
        {
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT PackageID, PackageStatusID, DispatchID FROM Packages WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            DateTime CurrentDate = Security.GetCurrentDate();
            SelectCommand = @"SELECT DispatchID, ConfirmExpUserID, ConfirmExpDateTime FROM Dispatch WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                int D = Convert.ToInt32(DT.Rows[i]["DispatchID"]);
                                if (Confirm)
                                {
                                    int StorePackagesCount = TempDT.Select("DispatchID=" + D + " AND (PackageStatusID = 2 OR PackageStatusID = 3 OR PackageStatusID = 4)").Count();
                                    int AllPackagesCount = TempDT.Select("DispatchID=" + D).Count();
                                    if (AllPackagesCount > 0 && AllPackagesCount == StorePackagesCount)
                                    {
                                        DataRow[] rows1 = DT.Select("DispatchID=" + D);
                                        if (rows1.Count() > 0)
                                        {
                                            rows1[0]["ConfirmExpDateTime"] = CurrentDate;
                                            rows1[0]["ConfirmExpUserID"] = Security.CurrentUserID;
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow[] rows1 = DT.Select("DispatchID=" + D);
                                    if (rows1.Count() > 0)
                                    {
                                        rows1[0]["ConfirmExpDateTime"] = DBNull.Value;
                                        rows1[0]["ConfirmExpUserID"] = DBNull.Value;
                                    }
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            TempDT.Dispose();
        }

        public void SaveConfirmDispInfo(int[] DispatchID, bool Confirm)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            string SelectCommand = "SELECT DispatchID, ConfirmDispUserID, ConfirmExpDateTime, ConfirmDispDateTime FROM Dispatch WHERE DispatchID IN (" + string.Join(",", DispatchID) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DispatchID.Count(); i++)
                            {
                                if (Confirm)
                                {
                                    DataRow[] rows = DT.Select("DispatchID=" + DispatchID[i]);
                                    if (rows.Count() > 0)
                                    {
                                        if (rows[0]["ConfirmExpDateTime"] != DBNull.Value)
                                        {
                                            rows[0]["ConfirmDispDateTime"] = CurrentDate;
                                            rows[0]["ConfirmDispUserID"] = Security.CurrentUserID;
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow[] rows = DT.Select("DispatchID=" + DispatchID[i]);
                                    if (rows.Count() > 0)
                                    {
                                        rows[0]["ConfirmDispDateTime"] = DBNull.Value;
                                        rows[0]["ConfirmDispUserID"] = DBNull.Value;
                                    }
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SaveConfirmDispInfo(DateTime PrepareDispatchDateTime, bool Confirm)
        {
            DateTime CurrentDate = Security.GetCurrentDate();

            string SelectCommand = @"SELECT DispatchID, ConfirmExpDateTime, ConfirmDispUserID, ConfirmDispDateTime FROM Dispatch WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                int D = Convert.ToInt32(DT.Rows[i]["DispatchID"]);
                                if (Confirm)
                                {
                                    DataRow[] rows = DT.Select("DispatchID=" + D);
                                    if (rows.Count() > 0)
                                    {
                                        if (rows[0]["ConfirmExpDateTime"] != DBNull.Value)
                                        {
                                            rows[0]["ConfirmDispDateTime"] = CurrentDate;
                                            rows[0]["ConfirmDispUserID"] = Security.CurrentUserID;
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow[] rows = DT.Select("DispatchID=" + D);
                                    if (rows.Count() > 0)
                                    {
                                        rows[0]["ConfirmDispDateTime"] = DBNull.Value;
                                        rows[0]["ConfirmDispUserID"] = DBNull.Value;
                                    }
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void UpdateDispatchDates(DateTime Date)
        {
            string SelectCommand = "SELECT DISTINCT PrepareDispatchDateTime FROM Dispatch" +
                " WHERE DATEPART(month, PrepareDispatchDateTime) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                "') AND DATEPART(year, PrepareDispatchDateTime) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                " ORDER BY PrepareDispatchDateTime DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchDatesDT);
            }
            FillWeekNumber();
        }

        public void FilterDispatchByDate(DateTime Date, bool NotPacked, bool Packed, bool Store, bool Exp, bool Disp)
        {
            string Filter = string.Empty;
            if (NotPacked)
            {
                DataRow[] rows = DispatchInfoDT.Select("PackageStatusID=0");
                for (int i = 0; i < rows.Count(); i++)
                {
                    Filter += rows[i]["DispatchID"].ToString() + ",";
                }
            }
            if (Packed)
            {
                DataRow[] rows = DispatchInfoDT.Select("PackageStatusID=1");
                for (int i = 0; i < rows.Count(); i++)
                {
                    Filter += rows[i]["DispatchID"].ToString() + ",";
                }
            }
            if (Store)
            {
                DataRow[] rows = DispatchInfoDT.Select("PackageStatusID=2");
                for (int i = 0; i < rows.Count(); i++)
                {
                    Filter += rows[i]["DispatchID"].ToString() + ",";
                }
            }
            if (Exp)
            {
                DataRow[] rows = DispatchInfoDT.Select("PackageStatusID=4");
                for (int i = 0; i < rows.Count(); i++)
                {
                    Filter += rows[i]["DispatchID"].ToString() + ",";
                }
            }
            if (Disp)
            {
                DataRow[] rows = DispatchInfoDT.Select("PackageStatusID=3");
                for (int i = 0; i < rows.Count(); i++)
                {
                    Filter += rows[i]["DispatchID"].ToString() + ",";
                }
            }
            if (Filter.Length > 0)
            {
                Filter = Filter.Substring(0, Filter.Length - 1);
                Filter = " AND DispatchID IN (" + Filter + ")";
            }

            if ((NotPacked || Packed || Store || Exp || Disp) && Filter.Length == 0)
                Filter = " AND DispatchID IN (-1)";
            string SelectCommand = @"SELECT Dispatch.*, infiniu2_zovreference.dbo.Clients.ClientName, 
                ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
                INNER JOIN infiniu2_zovreference.dbo.Clients ON Dispatch.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
                WHERE CAST(PrepareDispatchDateTime AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'" + Filter + " ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchDT);
                //for (int i = 0; i < DispatchDT.Rows.Count; i++)
                //    DispatchDT.Rows[i]["Weight"] = GetWeight(Convert.ToInt32(DispatchDT.Rows[i]["DispatchID"]));
            }
            FillDispPackagesInfo();
        }

        public void FilterDispatchContent(int DispatchID)
        {
            string SelectCommand = "SELECT MainOrders.MegaOrderID, MainOrders.ClientID, MainOrders.DocNumber, MainOrders.MainOrderID, MainOrders.DoNotDispatch FROM MainOrders" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID=MegaOrders.MegaOrderID" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE DispatchID = " + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchContentDT);
            }
        }

        public void MoveToDispatchDate(DateTime DispatchDate)
        {
            DispatchDatesBS.Position = DispatchDatesBS.Find("PrepareDispatchDateTime", DispatchDate);
        }

        public void MoveToDispatch(int DispatchID)
        {
            DispatchBS.Position = DispatchBS.Find("DispatchID", DispatchID);
        }

        public void MoveToMainOrder(int MainOrderID)
        {
            DispatchContentBS.Position = DispatchContentBS.Find("MainOrderID", MainOrderID);
        }

        public void AddDispatch(int ClientID, object PrepareDispatchDateTime)
        {
            string SelectCommand = "SELECT TOP 0 * FROM Dispatch";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["CreationDateTime"] = Security.GetCurrentDate();
                        if (PrepareDispatchDateTime != DBNull.Value)
                            NewRow["PrepareDispatchDateTime"] = Convert.ToDateTime(PrepareDispatchDateTime);
                        NewRow["ClientID"] = ClientID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public int MaxDispatchID()
        {
            string SelectCommand = "SELECT MAX(DispatchID) AS DispatchID FROM Dispatch";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["DispatchID"]);
                }
            }
            return 0;
        }

        public bool ExcludeMainOrderFromDispatch(int MainOrderID)
        {
            string SelectCommand = @"SELECT PackageID, MainOrderID, DispatchID FROM Packages WHERE MainOrderID=" + MainOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["DispatchID"] = DBNull.Value;
                            }
                            DA.Update(DT);
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public bool ExcludePackageFromDispatch(int PackageID)
        {
            string SelectCommand = @"SELECT PackageID, MainOrderID, DispatchID FROM Packages WHERE PackageID=" + PackageID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["DispatchID"] = DBNull.Value;
                            DA.Update(DT);
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public bool IsDispatchBindToPermit(DateTime ZOVDispatchDate)
        {
            string SelectCommand = @"SELECT * FROM Permits WHERE CAST (ZOVDispatchDate AS Date) = '" + ZOVDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    }








    public struct PackagesCount
    {
        public int ProfilPackedPackages;
        public int TPSPackedPackages;
        public int AllPackedPackages;

        public int ProfilPackages;
        public int TPSPackages;
        public int AllPackages;
    }
    public class PackingReportZOV : IAllFrontParameterName, IIsMarsel
    {
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public DataTable FrontsDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public PackingReportZOV()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
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
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("AccountingName"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Product"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Count"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("AccountingName"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
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

                string FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                string TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
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
                NewRow["InsetType"] = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                NewRow["InsetColor"] = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["AccountingName"] = Row["AccountingName"];
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
                    NewRow["Width"] = Convert.ToInt32(Row["Width"]);

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["AccountingName"] = Row["AccountingName"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(object PrepareDispatchDateTime, int MainOrderID, int FactoryID)
        {
            string FactoryFilter1 = string.Empty;
            string FactoryFilter2 = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter1 = " AND FrontsOrders.FactoryID = " + FactoryID;
                FactoryFilter2 = " AND FactoryID = " + FactoryID;
            }

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName FROM FrontsOrders 
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE MainOrderID = " + MainOrderID + FactoryFilter1,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "')" + " AND MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter2 + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
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
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(object PrepareDispatchDateTime, int MainOrderID, int FactoryID)
        {
            string FactoryFilter1 = string.Empty;
            string FactoryFilter2 = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter1 = " AND DecorOrders.FactoryID = " + FactoryID;
                FactoryFilter2 = " AND FactoryID = " + FactoryID;
            }

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();


            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName FROM DecorOrders
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID WHERE MainOrderID = " + MainOrderID + FactoryFilter1,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "')" + " AND MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter2 + ")",
                ConnectionStrings.ZOVOrdersConnectionString))
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

                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];

                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool HasFronts(object PrepareDispatchDateTime, int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "')" + " AND MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        private bool HasDecor(object PrepareDispatchDateTime, int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "')" + " AND MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        return true;
                }
            }

            return false;
        }

        public void CreateReport(ref HSSFWorkbook thssfworkbook, DataTable OrderDT, object PrepareDispatchDateTime,
            string DispatchDate, int FactoryID)
        {
            string SheetName = "Ведомость Профиль+ТПС";

            if (FactoryID == 1)
                SheetName = "Ведомость Профиль";
            if (FactoryID == 2)
                SheetName = "Ведомость ТПС";

            HSSFSheet sheet1 = thssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            DataTable DT = new DataTable();

            using (DataView DV = new DataView(OrderDT))
            {
                DT = DV.ToTable(true, new string[] { "MainOrderID" });
            }

            int[] MainOrderIDs = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                MainOrderIDs[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            DT.Dispose();

            if (HasFronts(PrepareDispatchDateTime, MainOrderIDs, FactoryID))
            {
                CreateFrontsExcel(ref thssfworkbook, OrderDT, PrepareDispatchDateTime, DispatchDate, ref RowIndex, FactoryID, SheetName);
            }

            if (HasDecor(PrepareDispatchDateTime, MainOrderIDs, FactoryID))
            {
                CreateDecorExcel(ref thssfworkbook, OrderDT, PrepareDispatchDateTime, DispatchDate, RowIndex, FactoryID, SheetName);
            }
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, DataTable OrdersDT, object PrepareDispatchDateTime,
            string DispatchDate, ref int RowIndex, int FactoryID, string SheetName)
        {

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;
            bool DoNotDispatch = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            #region
            int DisplayIndex = 0;
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 4 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 18 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 12 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 5 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 7 * 256);
            hssfworkbook.GetSheet(SheetName).SetColumnWidth(DisplayIndex++, 9 * 256);

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
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

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 6, "Утверждаю...............");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterFrontsOrders(PrepareDispatchDateTime, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Клиент: " +
                    OrdersDT.Rows[i]["ClientName"].ToString() + " " + OrdersDT.Rows[i]["DocNumber"].ToString() + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                DoNotDispatch = IsDoNotDispatch(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell1.CellStyle = HeaderStyle;

                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Бухг. наим.");
                cell1.CellStyle = HeaderStyle;

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

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3 || DoNotDispatch)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
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
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
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
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(++RowIndex), 8, Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
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

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, DataTable OrdersDT, object PrepareDispatchDateTime,
            string DispatchDate, int RowIndex, int FactoryID, string SheetName)
        {
            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;
            bool DoNotDispatch = false;

            #region

            HSSFFont MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
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

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);
            #endregion

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterDecorOrders(PrepareDispatchDateTime, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                DoNotDispatch = IsDoNotDispatch(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0,
                    "Клиент: " + OrdersDT.Rows[i]["ClientName"].ToString() + " " + OrdersDT.Rows[i]["DocNumber"].ToString() + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote + "  ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0 && !DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length < 1 && DoNotDispatch)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex++), 0, "ОТГРУЗКА БЕЗ ФАСАДОВ");
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Наименование");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), DisplayIndex++, "Бухг. наим.");
                cell1.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3 || DoNotDispatch)
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
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
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    hssfworkbook.GetSheet(SheetName).AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
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
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(hssfworkbook.GetSheet(SheetName).CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }
    }

    public class DispatchReportZOV
    {
        PackingReportZOV PackingReport;

        PackagesCount PackagesCount;

        object CreationDateTime = DBNull.Value;
        object ConfirmExpDateTime = DBNull.Value;
        object ConfirmDispDateTime = DBNull.Value;
        object PrepareDispatchDateTime = DBNull.Value;
        object RealDispDateTime = DBNull.Value;
        object ConfirmExpUserID = DBNull.Value;
        object ConfirmDispUserID = DBNull.Value;
        object RealDispUserID = DBNull.Value;
        object MachineName = DBNull.Value;
        object PermitNumber = DBNull.Value;
        object SealNumber = DBNull.Value;

        DataTable SimpleResultDT = null;
        DataTable AttachResultDT = null;
        DataTable PackagesDT = null;

        public DispatchReportZOV()
        {
            Create();
        }

        public void Initialize()
        {
            ClearPackages();
            FillPackages();
        }

        private void ClearPackages()
        {
            PackagesDT.Clear();
        }

        public object CurrentDispatchDateTime
        {
            get { return PrepareDispatchDateTime; }
            set { PrepareDispatchDateTime = value; }
        }

        private void Create()
        {
            SimpleResultDT = new DataTable();
            AttachResultDT = new DataTable();
            PackagesDT = new DataTable();

            SimpleResultDT.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            SimpleResultDT.Columns.Add(new DataColumn(("FrontsPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDT.Columns.Add(new DataColumn(("DecorPackagesCount"), System.Type.GetType("System.String")));
            SimpleResultDT.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));

            AttachResultDT.Columns.Add(new DataColumn(("MainOrder"), System.Type.GetType("System.String")));
            AttachResultDT.Columns.Add(new DataColumn(("ProfilFrontsPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDT.Columns.Add(new DataColumn(("ProfilDecorPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDT.Columns.Add(new DataColumn(("TPSFrontsPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDT.Columns.Add(new DataColumn(("TPSDecorPackagesCount"), System.Type.GetType("System.String")));
            AttachResultDT.Columns.Add(new DataColumn(("AllPackagesCount"), System.Type.GetType("System.String")));
        }

        private void FillPackages()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(
                @"SELECT MainOrders.MegaOrderID, MainOrders.DocNumber, infiniu2_zovreference.dbo.Clients.ClientName, Packages.ProductType, Packages.FactoryID, Packages.MainOrderID, Packages.PackageID, Packages.PackageStatusID
                FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
                WHERE DispatchID IN (SELECT DispatchID FROM Dispatch 
                WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "')",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }
        }

        public string GetClientName(int ClientID)
        {
            string ClientName = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients" +
                    " WHERE ClientID=" + ClientID, ConnectionStrings.ZOVReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientName = DT.Rows[0]["ClientName"].ToString();
                }
            }

            return ClientName;
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ShortName FROM Users" +
                    " WHERE UserID=" + UserID, ConnectionStrings.UsersConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        Name = DT.Rows[0]["ShortName"].ToString();
                }
            }

            return Name;
        }

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

        private string[] GetOrderNumbers()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DT = DV.ToTable(true, new string[] { "DocNumber" });
            }

            string[] rows = new string[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = DT.Rows[i]["DocNumber"].ToString();
            DT.Dispose();
            return rows;
        }

        private string[] GetOrderNumbers(int FactoryID)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DV.RowFilter = "FactoryID = " + FactoryID;
                DT = DV.ToTable(true, new string[] { "DocNumber" });
            }

            string[] rows = new string[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = DT.Rows[i]["DocNumber"].ToString();
            DT.Dispose();
            return rows;
        }

        private DataTable GetOrdersInfo()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DT = DV.ToTable(true, new string[] { "MegaOrderID", "DocNumber", "MainOrderID", "ClientName" });
            }

            return DT;
        }

        public DataTable GetMainOrders(int FactoryID)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DV.RowFilter = "FactoryID=" + FactoryID;
                DT = DV.ToTable(true, new string[] { "MainOrderID", "ClientName" });
            }

            //int[] rows = new int[DT.Rows.Count];

            //for (int i = 0; i < DT.Rows.Count; i++)
            //    rows[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            //DT.Dispose();
            return DT;
        }

        private int GetDispPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            DataRow[] rows = PackagesDT.Select("MainOrderID = " + MainOrderID + " AND PackageStatusID = 3 AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID);
            return rows.Count();
        }

        private int GetPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            DataRow[] rows = PackagesDT.Select("MainOrderID = " + MainOrderID + " AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID);
            return rows.Count();
        }

        private decimal GetSquare(int FactoryID, DateTime DispatchDate)
        {
            decimal Square = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, FrontsOrders.Count AS FrontsOrdersCount, FrontsOrders.Square, FrontsOrders.Weight FROM PackageDetails 
                    INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND CAST(DispatchDateTime AS Date)='" + DispatchDate.ToString("yyyy-MM-dd") + "' AND FactoryID = " + FactoryID + " AND DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "'))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["Square"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["FrontsOrdersCount"] != DBNull.Value)
                            Square += Convert.ToDecimal(DT.Rows[i]["Square"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["FrontsOrdersCount"]);
                    }
                    Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                }
            }
            return Square;
        }

        private decimal GetWeight(int FactoryID, DateTime DispatchDate)
        {
            decimal PackWeight = 0;
            decimal Weight = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, FrontsOrders.Count AS FrontsOrdersCount, FrontsOrders.Square, FrontsOrders.Weight FROM PackageDetails 
                    INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND CAST(DispatchDateTime AS Date)='" + DispatchDate.ToString("yyyy-MM-dd") + "' AND FactoryID = " + FactoryID + " AND DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "'))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["Square"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["FrontsOrdersCount"] != DBNull.Value)
                            PackWeight += Convert.ToDecimal(0.7) * Convert.ToDecimal(DT.Rows[i]["Square"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["FrontsOrdersCount"]);
                        if (DT.Rows[i]["Weight"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["FrontsOrdersCount"] != DBNull.Value)
                            Weight += Convert.ToDecimal(DT.Rows[i]["Weight"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["FrontsOrdersCount"]);
                    }
                    Weight = PackWeight + Weight;
                }
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, DecorOrders.Count AS DecorOrdersCount, DecorOrders.Weight FROM PackageDetails 
                    INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND CAST(DispatchDateTime AS Date)='" + DispatchDate.ToString("yyyy-MM-dd") + "' AND FactoryID = " + FactoryID + " AND DispatchID IN (SELECT DispatchID FROM Dispatch WHERE PrepareDispatchDateTime = '" + Convert.ToDateTime(PrepareDispatchDateTime).ToString("yyyy-MM-dd") + "'))",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["Weight"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["DecorOrdersCount"] != DBNull.Value)
                            Weight += Convert.ToDecimal(DT.Rows[i]["Weight"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["DecorOrdersCount"]);
                    }
                }
                Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);
            }

            return Weight;
        }

        private bool Fill(DataTable MainOrdersInfo, int FactoryID)
        {
            SimpleResultDT.Clear();

            PackagesCount.AllPackages = 0;
            PackagesCount.AllPackedPackages = 0;
            PackagesCount.ProfilPackages = 0;
            PackagesCount.ProfilPackedPackages = 0;
            PackagesCount.TPSPackages = 0;
            PackagesCount.TPSPackedPackages = 0;

            int MainOrderID = 0;
            for (int i = 0; i < MainOrdersInfo.Rows.Count; i++)
            {
                int FrontsPackedPackagesCount = 0;
                int DecorPackedPackagesCount = 0;
                int AllPackedPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                MainOrderID = Convert.ToInt32(MainOrdersInfo.Rows[i]["MainOrderID"]);

                FrontsPackedPackagesCount = GetDispPackagesCount(MainOrderID, FactoryID, 0);
                DecorPackedPackagesCount = GetDispPackagesCount(MainOrderID, FactoryID, 1);
                AllPackedPackagesCount = FrontsPackedPackagesCount + DecorPackedPackagesCount;

                PackagesCount.AllPackedPackages += AllPackedPackagesCount;
                PackagesCount.ProfilPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;
                PackagesCount.TPSPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrderID, FactoryID, 0);
                DecorPackagesCount = GetPackagesCount(MainOrderID, FactoryID, 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                PackagesCount.AllPackages += AllPackagesCount;
                PackagesCount.ProfilPackages += FrontsPackagesCount + DecorPackagesCount;
                PackagesCount.TPSPackages += FrontsPackagesCount + DecorPackagesCount;

                DataRow NewRow = SimpleResultDT.NewRow();
                NewRow["MainOrder"] = MainOrdersInfo.Rows[i]["ClientName"].ToString() + " Подзаказ №" + MainOrderID;
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsPackedPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorPackedPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllPackedPackagesCount.ToString() + " / " + AllPackagesCount.ToString();
                SimpleResultDT.Rows.Add(NewRow);
            }

            return SimpleResultDT.Rows.Count > 0;
        }

        public void CreateReport(bool bNeedProfilList, bool bNeedTPSList, bool Attach, object PrepareDispDateTime)
        {
            DataTable OrdersInfo = GetOrdersInfo();

            DataTable ProfilMainOrdersInfo = GetMainOrders(1);
            DataTable TPSMainOrdersInfo = GetMainOrders(2);

            if (bNeedProfilList && bNeedTPSList)
            {
                if (ProfilMainOrdersInfo.Rows.Count == 0 && TPSMainOrdersInfo.Rows.Count == 0)
                    return;
            }
            if (bNeedProfilList && !bNeedTPSList)
            {
                if (ProfilMainOrdersInfo.Rows.Count == 0)
                    return;
            }
            if (!bNeedProfilList && bNeedTPSList)
            {
                if (TPSMainOrdersInfo.Rows.Count == 0)
                    return;
            }

            int FactoryID = 1;

            string Firm = "(Profil+TPS)";
            if (bNeedProfilList && !bNeedTPSList)
                Firm = "(Profil)";
            if (!bNeedProfilList && bNeedTPSList)
                Firm = "(TPS)";

            //ClientName = ClientName.Replace('/', '-');

            if (ProfilMainOrdersInfo.Rows.Count == 0 && TPSMainOrdersInfo.Rows.Count == 0)
            {
                MessageBox.Show("Выбранный заказ пуст");
                return;
            }

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            if (Attach)
            {
                PackingReport = new PackingReportZOV();

                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrdersInfo.Rows.Count > 0 && TPSMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrdersInfo, 1, "ЗОВ-Профиль");
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrdersInfo, 2, "ЗОВ-ТПС");
                        }

                        PackingReport.CreateReport(ref hssfworkbook, OrdersInfo, PrepareDispatchDateTime, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), 0);
                    }
                    if (ProfilMainOrdersInfo.Rows.Count > 0 && TPSMainOrdersInfo.Rows.Count < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrdersInfo, 1, "ЗОВ-Профиль");
                            PackingReport.CreateReport(ref hssfworkbook, OrdersInfo, PrepareDispatchDateTime, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), FactoryID);
                        }
                    }
                    if (ProfilMainOrdersInfo.Rows.Count < 1 && TPSMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrdersInfo, 2, "ЗОВ-ТПС");
                            PackingReport.CreateReport(ref hssfworkbook, OrdersInfo, PrepareDispatchDateTime, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), FactoryID);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrdersInfo, 1, "ЗОВ-Профиль");
                            PackingReport.CreateReport(ref hssfworkbook, OrdersInfo, PrepareDispatchDateTime, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), FactoryID);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrdersInfo, 2, "ЗОВ-ТПС");
                            PackingReport.CreateReport(ref hssfworkbook, OrdersInfo, PrepareDispatchDateTime, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), FactoryID);
                        }
                    }
                }

                string FileName = "Dispatch " + Convert.ToDateTime(PrepareDispDateTime).ToString("dd.MM.yyyy") + " " + Firm;

                FileName = FileName.Replace("*", " ");
                FileName = FileName.Replace("|", " ");
                FileName = FileName.Replace(@"\", " ");
                FileName = FileName.Replace(":", " ");
                FileName = FileName.Replace("<", " ");
                FileName = FileName.Replace(">", " ");
                FileName = FileName.Replace("?", " ");
                FileName = FileName.Replace("/", " ");

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
                Send(file.FullName, "horuz9@list.ru");
                //Send(file.FullName, "zovvozvrat@mail.ru");
                //Send(file.FullName, "romanchukgrad@gmail.com");
                System.Diagnostics.Process.Start(file.FullName);
            }


            else
            {
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrdersInfo.Rows.Count > 0 && TPSMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrdersInfo, 1, "Профиль");
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrdersInfo, 2, "ТПС");
                        }
                    }
                    if (ProfilMainOrdersInfo.Rows.Count > 0 && TPSMainOrdersInfo.Rows.Count < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrdersInfo, 1, "Профиль");
                        }
                    }
                    if (ProfilMainOrdersInfo.Rows.Count < 1 && TPSMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrdersInfo, 2, "ТПС");
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 1;
                        Firm = "Профиль";
                        if (Fill(ProfilMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrdersInfo, 1, "Профиль");
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrdersInfo.Rows.Count > 0)
                    {
                        FactoryID = 2;
                        Firm = "ТПС";
                        if (Fill(TPSMainOrdersInfo, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrdersInfo, 2, "ТПС");
                        }
                    }
                }

                string FileName = "Dispatch " + Convert.ToDateTime(PrepareDispDateTime).ToString("dd.MM.yyyy") + " " + Firm;

                FileName = FileName.Replace("*", " ");
                FileName = FileName.Replace("|", " ");
                FileName = FileName.Replace(@"\", " ");
                FileName = FileName.Replace(":", " ");
                FileName = FileName.Replace("<", " ");
                FileName = FileName.Replace(">", " ");
                FileName = FileName.Replace("?", " ");
                FileName = FileName.Replace("/", " ");
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
                Send(file.FullName, "horuz9@list.ru");
                //Send(file.FullName, "zovvozvrat@mail.ru");
                //Send(file.FullName, "romanchukgrad@gmail.com");
                System.Diagnostics.Process.Start(file.FullName);
            }
        }

        public void GetDispatchInfo(ref object date1, ref object date2, ref object date3, ref object date4, ref object date5,
            ref object User1, ref object User2, ref object User3, ref object sMachineName, ref object sPermitNumber, ref object sSealNumber)
        {
            CreationDateTime = date1;
            ConfirmExpDateTime = date2;
            ConfirmDispDateTime = date3;
            RealDispDateTime = date4;
            PrepareDispatchDateTime = date5;
            ConfirmExpUserID = User1;
            ConfirmDispUserID = User2;
            RealDispUserID = User3;
            MachineName = sMachineName;
            PermitNumber = sPermitNumber;
            SealNumber = sSealNumber;
        }

        private void CreateExcel(ref HSSFWorkbook hssfworkbook, DataTable MainOrdersInfo, int FactoryID, string Firm)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet(Firm);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            #region Create fonts and styles

            HSSFFont HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.FontHeightInPoints = 13;
            HeaderFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont.FontName = "Calibri";

            HSSFFont NotesFont = hssfworkbook.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            HSSFFont SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 12;
            SimpleFont.FontName = "Calibri";

            HSSFFont TempFont = hssfworkbook.CreateFont();
            TempFont.FontHeightInPoints = 12;
            TempFont.FontName = "Calibri";

            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle EmptyCellStyle = hssfworkbook.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            HSSFCellStyle ConfirmStyle = hssfworkbook.CreateCellStyle();
            ConfirmStyle.SetFont(HeaderFont);

            HSSFCellStyle HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle.WrapText = true;
            HeaderStyle.SetFont(HeaderFont);

            HSSFCellStyle MainOrderCellStyle = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont);

            HSSFCellStyle MainOrderCellStyle1 = hssfworkbook.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle NotesCellStyle = hssfworkbook.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            HSSFCellStyle SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont);

            HSSFCellStyle SimpleCellStyle1 = hssfworkbook.CreateCellStyle();
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle1.SetFont(SimpleFont);

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TempFont);

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            int RowIndex = 0;
            HSSFCell ConfirmCell = null;

            //HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, "«УТВЕРЖДАЮ»");
            //ConfirmCell.CellStyle = ConfirmStyle;
            if (ConfirmDispDateTime != DBNull.Value)
            {
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 1, "«УТВЕРЖДАЮ»  " + GetUserName(Convert.ToInt32(ConfirmDispUserID)));
                ConfirmCell.CellStyle = ConfirmStyle;
            }
            if (CreationDateTime != DBNull.Value)
            {
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата создания отгрузки: " + Convert.ToDateTime(CreationDateTime).ToString("dd.MM.yyyy HH:mm"));
                ConfirmCell.CellStyle = TempStyle;
            }
            if (ConfirmExpDateTime != DBNull.Value)
            {
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Экспедиция разрешена: " + GetUserName(Convert.ToInt32(ConfirmExpUserID)) + " " + Convert.ToDateTime(ConfirmExpDateTime).ToString("dd.MM.yyyy HH:mm"));
                ConfirmCell.CellStyle = TempStyle;
            }
            if (ConfirmDispDateTime != DBNull.Value)
            {
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Отгрузка разрешена: " + GetUserName(Convert.ToInt32(ConfirmDispUserID)) + " " + Convert.ToDateTime(ConfirmDispDateTime).ToString("dd.MM.yyyy HH:mm"));
                ConfirmCell.CellStyle = TempStyle;
            }
            if (RealDispDateTime != DBNull.Value)
            {
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Отгрузку произвел: " + GetUserName(Convert.ToInt32(RealDispUserID)) + " " + Convert.ToDateTime(RealDispDateTime).ToString("dd.MM.yyyy HH:mm"));
                ConfirmCell.CellStyle = TempStyle;
            }
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Ведомость создана: " + Security.CurrentUserShortName + " " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            ConfirmCell.CellStyle = TempStyle;

            //ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), 0, "Дата отгрузки: " + PrepareDispatchDateTime.ToString("dd.MM.yyyy"));
            //ConfirmCell.CellStyle = TempStyle;
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Участок: " + Firm);
            ConfirmCell.CellStyle = TempStyle;
            //ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            //ConfirmCell.CellStyle = TempStyle;
            //ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказы: " + OrderNumber);
            //ConfirmCell.CellStyle = TempStyle;

            int TopRowFront = 1;
            int BottomRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            string Notes = string.Empty;

            sheet1.SetColumnWidth(0, 55 * 256);
            sheet1.SetColumnWidth(1, 12 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);

            HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle;

            TopRowFront = RowIndex;
            BottomRowFront = SimpleResultDT.Rows.Count + RowIndex;

            for (int i = 0; i < MainOrdersInfo.Rows.Count; i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrdersInfo.Rows[i]["MainOrderID"]));

                for (int y = 0; y < SimpleResultDT.Columns.Count; y++)
                {
                    if (Notes.Length > 0)
                    {
                        if (AttachResultDT.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (SimpleResultDT.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = sheet1.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = sheet1.CreateRow(RowIndex + 2).CreateCell(0);
                        cell.SetCellValue("Примечание: " + Notes);
                        cell.CellStyle = NotesCellStyle;
                    }
                }

                if (Notes.Length > 0)
                {
                    RowIndex++;
                    BottomRowFront++;
                }


                RowIndex++;
            }

            RowIndex++;


            RowIndex++;

            Weight = GetWeight(FactoryID, Convert.ToDateTime(PrepareDispatchDateTime));
            TotalFrontsSquare = GetSquare(FactoryID, Convert.ToDateTime(PrepareDispatchDateTime));

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle;

            if (FactoryID == 1)
            {
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Упаковок: " + PackagesCount.ProfilPackedPackages + "/" + PackagesCount.ProfilPackages);
                cell16.CellStyle = TempStyle;
            }
            if (FactoryID == 2)
            {
                HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Упаковок: " + PackagesCount.TPSPackedPackages + "/" + PackagesCount.TPSPackages);
                cell16.CellStyle = TempStyle;
            }
            RowIndex++;

            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Начальник логистики: Скоморошко Е.В.");
            ConfirmCell.CellStyle = TempStyle;
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Организатор отгрузки: ");
            ConfirmCell.CellStyle = TempStyle;
            if (RealDispDateTime != DBNull.Value)
            {
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Номер пропуска: " + PermitNumber.ToString());
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Номер пломбы: " + SealNumber.ToString());
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Машина: " + MachineName.ToString());
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "На вынос: ");
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Ответственный за материальную отгрузку: " + GetUserName(Convert.ToInt32(RealDispUserID)));
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Участники отгрузки: ");
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, " - механическая:");
                ConfirmCell.CellStyle = TempStyle;
                ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, " - ручная:");
                ConfirmCell.CellStyle = TempStyle;
            }
        }

        public void Send(string FileName, string MailAddressTo)
        {
            string AccountPassword = "1290qpalzm";
            string SenderEmail = "zovprofilreport@mail.ru";

            string from = SenderEmail;

            if (MailAddressTo.Length == 0)
            {
                MessageBox.Show("У клиента не указан Email. Отправка отчета невозможна");
                return;
            }

            using (MailMessage message = new MailMessage(from, MailAddressTo))
            {
                message.Subject = "Отчет по отгрузке " + Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy");
                message.Body = "Отчет сгенерирован автоматически системой Infinium. Не надо отвечать на это письмо. По всем вопросам обращайтесь " +
                    "infiniumdevelopers@gmail.com";
                SmtpClient client = new SmtpClient("smtp.mail.ru")
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SenderEmail, AccountPassword)
                };
                string S = new UTF8Encoding().GetString(Encoding.Convert(Encoding.GetEncoding("UTF-16"), Encoding.UTF8, Encoding.GetEncoding("UTF-16").GetBytes(FileName)));

                Attachment attach = new Attachment(S,
                                                    MediaTypeNames.Application.Octet);

                ContentDisposition disposition = attach.ContentDisposition;

                message.Attachments.Add(attach);

                try
                {
                    client.Send(message);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                attach.Dispose();
                client.Dispose();
            }
        }

    }
}