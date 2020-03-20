using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Collections.Generic;
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

namespace Infinium.Modules.Marketing.Expedition
{
    public class MarketingExpeditionFrontsOrders
    {
        private PercentageDataGrid FrontsOrdersDataGrid = null;

        int CurrentPackNumber = 1;
        int CurrentMainOrder = 1;

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

        public MarketingExpeditionFrontsOrders(ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid)
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
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig)
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL",
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
            if (FrontsOrdersDataGrid.Columns.Contains("OriginalInsetPrice"))
                FrontsOrdersDataGrid.Columns["OriginalInsetPrice"].Visible = false;


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


            FrontsOrdersDataGrid.ScrollBars = ScrollBars.Both;

            foreach (DataGridViewColumn Column in FrontsOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            //названия столбцов
            FrontsOrdersDataGrid.Columns["IsSample"].HeaderText = "Образец";
            FrontsOrdersDataGrid.Columns["Height"].HeaderText = "Высота";
            FrontsOrdersDataGrid.Columns["Width"].HeaderText = "Ширина";
            FrontsOrdersDataGrid.Columns["Count"].HeaderText = "Кол-во";
            FrontsOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            FrontsOrdersDataGrid.Columns["Square"].HeaderText = "Площадь";
            //MainOrdersFrontsOrdersDataGrid.Columns["PackNumber"].HeaderText = "  Номер\r\nупаковки";

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
            FrontsOrdersDataGrid.Columns["Count"].Width = 65;
            FrontsOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["Square"].Width = 100;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsNonStandard"].Width = 85;
            FrontsOrdersDataGrid.Columns["IsSample"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsOrdersDataGrid.Columns["IsSample"].Width = 85;
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

        public bool FilterByMainOrder(int MainOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return FrontsOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;
            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();
            OriginalFrontsOrdersDataTable = FrontsOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 0 " + FactoryFilter + ")",
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                            " WHERE MainOrderID = " + MainOrderID + FactoryFilter, ConnectionStrings.MarketingOrdersConnectionString))
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
            string FactoryFilter = "";
            string MainOrdersFactoryFilter = "";

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
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" +
                " AND ProductType = 0 " + FactoryFilter + ")",
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM FrontsOrders" +
                            " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                            " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" + FactoryFilter,
                            ConnectionStrings.MarketingOrdersConnectionString))
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







    public class MarketingExpeditionDecorOrders
    {
        int CurrentPackNumber = 1;
        int CurrentMainOrder = 1;

        private PercentageDataGrid MainOrdersDecorOrdersDataGrid = null;

        private DataTable ColorsDataTable = null;
        public DataTable PatinaDataTable = null;
        public DataTable PatinaRALDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable InsetColorsDataTable = null;

        public DataTable DecorOrdersDataTable = null;

        public BindingSource DecorOrdersBindingSource = null;

        public SqlDataAdapter DecorOrdersDataAdapter = null;
        public SqlCommandBuilder DecorOrdersCommandBuilder = null;

        //конструктор
        public MarketingExpeditionDecorOrders(ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid)
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
            DecorOrdersDataAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM DecorOrders", ConnectionStrings.MarketingOrdersConnectionString);
            DecorOrdersCommandBuilder = new SqlCommandBuilder(DecorOrdersDataAdapter);
            DecorOrdersDataAdapter.Fill(DecorOrdersDataTable);
            DecorOrdersDataTable.Columns.Add(new DataColumn("PackNumber", Type.GetType("System.Int32")));

            string SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig)) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            GetColorsDT();
            GetInsetColorsDT();
            InsetTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDataTable);
            }
            PatinaDataTable = new DataTable();
            SelectCommand = @"SELECT * FROM Patina ORDER BY PatinaName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDataTable);
            }
            PatinaRALDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL ",
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
        private void GridSettings()
        {
            MainOrdersDecorOrdersDataGrid.Columns.Add(ProductColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(ItemColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(ColorColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(PatinaColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(InsetTypesColumn);
            MainOrdersDecorOrdersDataGrid.Columns.Add(InsetColorsColumn);
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
            MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["MainOrderID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ProductID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["DecorID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["ColorID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["PatinaID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["InsetTypeID"].Visible = false;
            MainOrdersDecorOrdersDataGrid.Columns["InsetColorID"].Visible = false;
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
            MainOrdersDecorOrdersDataGrid.Columns["IsSample"].HeaderText = "Образец";
            MainOrdersDecorOrdersDataGrid.Columns["Cost"].HeaderText = "Стоимость";
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
            MainOrdersDecorOrdersDataGrid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            MainOrdersDecorOrdersDataGrid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
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
            MainOrdersDecorOrdersDataGrid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["InsetTypesColumn"].MinimumWidth = 110;
            MainOrdersDecorOrdersDataGrid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["InsetColorsColumn"].MinimumWidth = 110;
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

            MainOrdersDecorOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDecorOrdersDataGrid.Columns["Notes"].MinimumWidth = 110;

            //MainOrdersDecorOrdersDataGrid.Columns["DecorOrderID"].DisplayIndex = 0;

            foreach (DataGridViewColumn Column in MainOrdersDecorOrdersDataGrid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public bool FilterByMainOrder(int MainOrderID, int FactoryID)
        {
            //if (CurrentMainOrderID == MainOrderID)
            //    return DecorOrdersDataTable.Rows.Count > 0;

            //CurrentMainOrderID = MainOrderID;
            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();
            OriginalDecorOrdersDataTable = DecorOrdersDataTable.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1) AND PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID = " + MainOrderID +
                " AND ProductType = 1 " + FactoryFilter + ")",
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID = " + MainOrderID + FactoryFilter, ConnectionStrings.MarketingOrdersConnectionString))
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
            string FactoryFilter = "";
            string MainOrdersFactoryFilter = "";

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
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails" +
                " WHERE PackageID IN (" +
                " SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" +
                " AND ProductType = 1 " + FactoryFilter + ")",
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
                    else
                    {
                        using (SqlDataAdapter fDA = new SqlDataAdapter("SELECT * FROM DecorOrders" +
                            " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                            " WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ")" + FactoryFilter, ConnectionStrings.MarketingOrdersConnectionString))
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





    public class MarketingExpeditionManager
    {
        private int CurrentMainOrderID = -1;
        private int CurrentMegaOrderID = -1;

        public MarketingExpeditionFrontsOrders PackedMainOrdersFrontsOrders = null;
        public MarketingExpeditionDecorOrders PackedMainOrdersDecorOrders = null;

        //string PackCount = "ProfilPackCount";
        //string ProductionStatus = "ProfilProductionStatusID";
        //string StorageStatus = "ProfilStorageStatusID";
        //string DispatchStatus = "ProfilDispatchStatusID";
        //string FirmOrderStatus = "ProfilOrderStatusID";
        //string PackAllocStatusID = "ProfilPackAllocStatusID";

        public PercentageDataGrid MainOrdersDataGrid = null;
        public PercentageDataGrid MegaOrdersDataGrid = null;
        public PercentageDataGrid PackagesDataGrid = null;
        private DevExpress.XtraTab.XtraTabControl OrdersTabControl = null;

        private DataTable dtRolePermissions = null;
        private DataTable ClientsDataTable = null;
        private DataTable FilterClientsDataTable = null;
        public DataTable MainOrdersDataTable = null;
        private DataTable FactoryTypesDataTable = null;
        public DataTable PackagesDataTable = null;
        private DataTable PackageStatusesDataTable = null;

        private DataTable FirmOrderStatusesDataTable = null;
        private DataTable ProductionStatusesDataTable = null;
        private DataTable StorageStatusesDataTable = null;
        private DataTable ExpeditionStatusesDataTable = null;
        private DataTable DispatchStatusesDataTable = null;
        private DataTable PackMainOrdersDataTable = null;
        private DataTable PackMegaOrdersDataTable = null;
        private DataTable StoreMainOrdersDataTable = null;
        private DataTable StoreMegaOrdersDataTable = null;
        private DataTable ExpMainOrdersDataTable = null;
        private DataTable ExpMegaOrdersDataTable = null;
        private DataTable DispMainOrdersDataTable = null;
        private DataTable DispMegaOrdersDataTable = null;
        public DataTable MegaOrdersDataTable = null;
        private DataTable BatchDetailsDataTable = null;
        private DataTable TempBatchDetailsDataTable = null;

        private DataTable UsersDataTable = null;
        private DataTable CurrencyTypesDataTable = null;

        private BindingSource CurrencyTypesBindingSource = null;
        public BindingSource ClientsBindingSource = null;
        public BindingSource FilterClientsBindingSource = null;
        public BindingSource MainOrdersBindingSource = null;
        public BindingSource FactoryTypesBindingSource = null;
        public BindingSource MegaOrdersBindingSource = null;
        public BindingSource PackagesBindingSource = null;
        public BindingSource PackageStatusesBindingSource = null;
        public BindingSource BatchDetailsBindingSource = null;

        private BindingSource FirmOrderStatusesBindingSource = null;
        private BindingSource ProductionStatusesBindingSource = null;
        private BindingSource StorageStatusesBindingSource = null;
        private BindingSource ExpeditionStatusesBindingSource = null;
        private BindingSource DispatchStatusesBindingSource = null;

        private DataGridViewComboBoxColumn ClientsColumn = null;
        private DataGridViewComboBoxColumn FactoryTypeColumn = null;
        private DataGridViewComboBoxColumn PackageStatusesColumn = null;

        private DataGridViewComboBoxColumn ProfilProductionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilStorageStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn ProfilDispatchStatusColumn = null;
        private DataGridViewComboBoxColumn TPSProductionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSStorageStatusColumn = null;
        private DataGridViewComboBoxColumn TPSExpeditionStatusColumn = null;
        private DataGridViewComboBoxColumn TPSDispatchStatusColumn = null;

        private DataGridViewComboBoxColumn MegaFactoryTypeColumn = null;
        private DataGridViewComboBoxColumn ProfilOrderStatusColumn = null;
        private DataGridViewComboBoxColumn TPSOrderStatusColumn = null;
        private DataGridViewComboBoxColumn CurrencyTypeColumn = null;
        private DataGridViewComboBoxColumn PackUsersColumn = null;
        private DataGridViewComboBoxColumn StoreUsersColumn = null;
        private DataGridViewComboBoxColumn ExpUsersColumn = null;
        private DataGridViewComboBoxColumn DispUsersColumn = null;

        public MarketingExpeditionManager(ref PercentageDataGrid tMegaOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDataGrid,
            ref PercentageDataGrid tPackagesDataGrid,
            ref PercentageDataGrid tMainOrdersFrontsOrdersDataGrid,
            ref PercentageDataGrid tMainOrdersDecorOrdersDataGrid,
            ref DevExpress.XtraTab.XtraTabControl tOrdersTabControl)
        {
            MainOrdersDataGrid = tMainOrdersDataGrid;
            MegaOrdersDataGrid = tMegaOrdersDataGrid;
            PackagesDataGrid = tPackagesDataGrid;
            OrdersTabControl = tOrdersTabControl;

            PackedMainOrdersFrontsOrders = new MarketingExpeditionFrontsOrders(ref tMainOrdersFrontsOrdersDataGrid);

            PackedMainOrdersDecorOrders = new MarketingExpeditionDecorOrders(ref tMainOrdersDecorOrdersDataGrid);

            Initialize();
        }

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

        #region Initialize
        private void Create()
        {
            dtRolePermissions = new DataTable();
            FilterClientsDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();
            MegaOrdersDataTable = new DataTable();
            PackagesDataTable = new DataTable();
            PackageStatusesDataTable = new DataTable();

            FirmOrderStatusesDataTable = new DataTable();
            ProductionStatusesDataTable = new DataTable();
            StorageStatusesDataTable = new DataTable();
            ExpeditionStatusesDataTable = new DataTable();
            DispatchStatusesDataTable = new DataTable();
            PackMegaOrdersDataTable = new DataTable();
            PackMainOrdersDataTable = new DataTable();
            StoreMainOrdersDataTable = new DataTable();
            StoreMegaOrdersDataTable = new DataTable();
            ExpMainOrdersDataTable = new DataTable();
            ExpMegaOrdersDataTable = new DataTable();
            DispMegaOrdersDataTable = new DataTable();
            DispMainOrdersDataTable = new DataTable();
            BatchDetailsDataTable = new DataTable();
            TempBatchDetailsDataTable = new DataTable();

            ClientsBindingSource = new BindingSource();
            FilterClientsBindingSource = new BindingSource();
            MainOrdersBindingSource = new BindingSource();
            MegaOrdersBindingSource = new BindingSource();
            FactoryTypesBindingSource = new BindingSource();
            PackagesBindingSource = new BindingSource();
            PackageStatusesBindingSource = new BindingSource();
            BatchDetailsBindingSource = new BindingSource();

            FirmOrderStatusesBindingSource = new BindingSource();
            ProductionStatusesBindingSource = new BindingSource();
            StorageStatusesBindingSource = new BindingSource();
            ExpeditionStatusesBindingSource = new BindingSource();
            DispatchStatusesBindingSource = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(FilterClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MainOrders",
               ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilPackedCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilPackPercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilStoreCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilStorePercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilExpCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilExpPercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilDispatchedCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("ProfilDispPercentage", Type.GetType("System.Int32")));

            MainOrdersDataTable.Columns.Add(new DataColumn("TPSPackedCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSPackPercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSStoreCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSStorePercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSExpCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSExpPercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSDispatchedCount", Type.GetType("System.String")));
            MainOrdersDataTable.Columns.Add(new DataColumn("TPSDispPercentage", Type.GetType("System.Int32")));
            MainOrdersDataTable.Columns.Add(new DataColumn("BatchNumber", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE MegaOrders.MegaOrderID IN (SELECT MegaOrderID FROM MainOrders" +
                " WHERE ((ProfilProductionStatusID=2 OR TPSProductionStatusID=2) OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2) OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)))" +
                " ORDER BY ClientName, OrderNumber",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MegaOrdersDataTable);
            }
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilPackedCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilPackPercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilStoreCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilStorePercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilExpCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilExpPercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilDispatchedCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("ProfilDispPercentage", Type.GetType("System.Int32")));

            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSPackedCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSPackPercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSStoreCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSStorePercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSExpCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSExpPercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSDispatchedCount", Type.GetType("System.String")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("TPSDispPercentage", Type.GetType("System.Int32")));
            MegaOrdersDataTable.Columns.Add(new DataColumn("MegaBatchNumber", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageStatuses", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PackageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 Packages.*, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.AreaID = infiniu2_catalog.dbo.Factory.FactoryID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FirmOrderStatuses", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(FirmOrderStatusesDataTable);
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID <> 0)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(PackMegaOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID <> 0" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(PackMainOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 2)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMegaOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =2 " +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMainOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 4)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(ExpMegaOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =4 " +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(ExpMainOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE (Packages.PackageStatusID = 3)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DispMegaOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID = 3" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DispMainOrdersDataTable);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID," +
                " BatchDetails.MainOrderID, MainOrders.MegaOrderID, ClientID," +
                " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID, MainOrders.FactoryID AS MFactoryID, BatchDetails.FactoryID AS BFactoryID" +
                " FROM BatchDetails" +
                " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID AND ((ProfilProductionStatusID=2 OR TPSProductionStatusID=2) OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2) OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2))" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID, ClientID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDataTable);
            }

            using (DataView DV = new DataView(BatchDetailsDataTable))
            {
                DV.Sort = "MegaBatchID";
                TempBatchDetailsDataTable = DV.ToTable(true, "MegaBatchID", "BatchID", "MegaOrderID", "ClientID");
            }

            CurrencyTypesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CurrencyTypes", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDataTable);
            }

            UsersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM Users", ConnectionStrings.UsersConnectionString))
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

            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                int MainOrderID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]);
                int FactoryID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["FactoryID"]);
                int ProfilPackAllocStatusID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["ProfilPackAllocStatusID"]);
                int TPSPackAllocStatusID = Convert.ToInt32(MainOrdersDataTable.Rows[i]["TPSPackAllocStatusID"]);

                MainOrderProfilPackCount = GetMainOrderPackCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilStoreCount = GetMainOrderStoreCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilExpCount = GetMainOrderExpCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilDispCount = GetMainOrderDispCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilAllCount = Convert.ToInt32(MainOrdersDataTable.Rows[i]["ProfilPackCount"]);

                MainOrderTPSPackCount = GetMainOrderPackCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSStoreCount = GetMainOrderStoreCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSExpCount = GetMainOrderExpCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSDispCount = GetMainOrderDispCount(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSAllCount = Convert.ToInt32(MainOrdersDataTable.Rows[i]["TPSPackCount"]);

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

                if (FactoryID != 2 && ProfilPackAllocStatusID != 2)
                {
                    MainOrderProfilPackCount = 0;
                    MainOrderProfilStoreCount = 0;
                    MainOrderProfilExpCount = 0;
                    MainOrderProfilDispCount = 0;
                }
                if (FactoryID != 1 && TPSPackAllocStatusID != 2)
                {
                    MainOrderTPSPackCount = 0;
                    MainOrderTPSStoreCount = 0;
                    MainOrderTPSExpCount = 0;
                    MainOrderTPSDispCount = 0;
                }

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

                MainOrdersDataTable.Rows[i]["ProfilPackedCount"] = MainOrderProfilPackCount + " / " + MainOrderProfilAllCount;
                MainOrdersDataTable.Rows[i]["ProfilPackPercentage"] = ProfilPackPercentage;
                MainOrdersDataTable.Rows[i]["ProfilStoreCount"] = MainOrderProfilStoreCount + " / " + MainOrderProfilAllCount;
                MainOrdersDataTable.Rows[i]["ProfilStorePercentage"] = ProfilStorePercentage;
                MainOrdersDataTable.Rows[i]["ProfilExpCount"] = MainOrderProfilExpCount + " / " + MainOrderProfilAllCount;
                MainOrdersDataTable.Rows[i]["ProfilExpPercentage"] = ProfilExpPercentage;
                MainOrdersDataTable.Rows[i]["ProfilDispatchedCount"] = MainOrderProfilDispCount + " / " + MainOrderProfilAllCount;
                MainOrdersDataTable.Rows[i]["ProfilDispPercentage"] = ProfilDispPercentage;

                MainOrdersDataTable.Rows[i]["TPSPackedCount"] = MainOrderTPSPackCount + " / " + MainOrderTPSAllCount;
                MainOrdersDataTable.Rows[i]["TPSPackPercentage"] = TPSPackPercentage;
                MainOrdersDataTable.Rows[i]["TPSStoreCount"] = MainOrderTPSStoreCount + " / " + MainOrderTPSAllCount;
                MainOrdersDataTable.Rows[i]["TPSStorePercentage"] = TPSStorePercentage;
                MainOrdersDataTable.Rows[i]["TPSExpCount"] = MainOrderTPSExpCount + " / " + MainOrderTPSAllCount;
                MainOrdersDataTable.Rows[i]["TPSExpPercentage"] = TPSExpPercentage;
                MainOrdersDataTable.Rows[i]["TPSDispatchedCount"] = MainOrderTPSDispCount + " / " + MainOrderTPSAllCount;
                MainOrdersDataTable.Rows[i]["TPSDispPercentage"] = TPSDispPercentage;
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

            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                int MegaOrderID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]);
                int FactoryID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["FactoryID"]);
                int ProfilPackAllocStatusID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["ProfilPackAllocStatusID"]);
                int TPSPackAllocStatusID = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["TPSPackAllocStatusID"]);

                MegaOrderProfilPackCount = GetMegaOrderPackCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilStoreCount = GetMegaOrderStoreCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilExpCount = GetMegaOrderExpCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilDispCount = GetMegaOrderDispCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 1);
                MegaOrderProfilAllCount = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["ProfilPackCount"]);

                MegaOrderTPSPackCount = GetMegaOrderPackCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSStoreCount = GetMegaOrderStoreCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSExpCount = GetMegaOrderExpCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSDispCount = GetMegaOrderDispCount(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]), 2);
                MegaOrderTPSAllCount = Convert.ToInt32(MegaOrdersDataTable.Rows[i]["TPSPackCount"]);

                ProfilPackProgressVal = 0;
                ProfilStoreProgressVal = 0;
                ProfilExpProgressVal = 0;
                ProfilDispProgressVal = 0;

                TPSPackProgressVal = 0;
                TPSStoreProgressVal = 0;
                TPSExpProgressVal = 0;
                TPSDispProgressVal = 0;

                if (FactoryID != 2 && ProfilPackAllocStatusID != 2)
                {
                    MegaOrderProfilPackCount = 0;
                    MegaOrderProfilStoreCount = 0;
                    MegaOrderProfilExpCount = 0;
                    MegaOrderProfilDispCount = 0;
                }
                if (FactoryID != 1 && TPSPackAllocStatusID != 2)
                {
                    MegaOrderTPSPackCount = 0;
                    MegaOrderTPSStoreCount = 0;
                    MegaOrderTPSExpCount = 0;
                    MegaOrderTPSDispCount = 0;
                }

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

                MegaOrdersDataTable.Rows[i]["ProfilPackedCount"] = MegaOrderProfilPackCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDataTable.Rows[i]["ProfilPackPercentage"] = ProfilPackPercentage;
                MegaOrdersDataTable.Rows[i]["ProfilStoreCount"] = MegaOrderProfilStoreCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDataTable.Rows[i]["ProfilStorePercentage"] = ProfilStorePercentage;
                MegaOrdersDataTable.Rows[i]["ProfilExpCount"] = MegaOrderProfilExpCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDataTable.Rows[i]["ProfilExpPercentage"] = ProfilExpPercentage;
                MegaOrdersDataTable.Rows[i]["ProfilDispatchedCount"] = MegaOrderProfilDispCount + " / " + MegaOrderProfilAllCount;
                MegaOrdersDataTable.Rows[i]["ProfilDispPercentage"] = ProfilDispPercentage;

                MegaOrdersDataTable.Rows[i]["TPSPackedCount"] = MegaOrderTPSPackCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDataTable.Rows[i]["TPSPackPercentage"] = TPSPackPercentage;
                MegaOrdersDataTable.Rows[i]["TPSStoreCount"] = MegaOrderTPSStoreCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDataTable.Rows[i]["TPSStorePercentage"] = TPSStorePercentage;
                MegaOrdersDataTable.Rows[i]["TPSExpCount"] = MegaOrderTPSExpCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDataTable.Rows[i]["TPSExpPercentage"] = TPSExpPercentage;
                MegaOrdersDataTable.Rows[i]["TPSDispatchedCount"] = MegaOrderTPSDispCount + " / " + MegaOrderTPSAllCount;
                MegaOrdersDataTable.Rows[i]["TPSDispPercentage"] = TPSDispPercentage;
            }
            //MegaOrdersBindingSource.MoveLast();
        }

        private void FillBatchNumber()
        {
            for (int i = 0; i < MainOrdersDataTable.Rows.Count; i++)
            {
                MainOrdersDataTable.Rows[i]["BatchNumber"] = GetBatchNumber(Convert.ToInt32(MainOrdersDataTable.Rows[i]["MainOrderID"]));
            }
        }

        private void FillMegaBatchNumber()
        {
            for (int i = 0; i < MegaOrdersDataTable.Rows.Count; i++)
            {
                MegaOrdersDataTable.Rows[i]["MegaBatchNumber"] = GetMegaBatchNumber(Convert.ToInt32(MegaOrdersDataTable.Rows[i]["MegaOrderID"]));
            }

        }

        private void Binding()
        {
            CurrencyTypesBindingSource = new BindingSource()
            {
                DataSource = CurrencyTypesDataTable
            };
            ClientsBindingSource.DataSource = ClientsDataTable;
            FilterClientsBindingSource.DataSource = FilterClientsDataTable;
            FactoryTypesBindingSource.DataSource = FactoryTypesDataTable;

            ProductionStatusesBindingSource.DataSource = ProductionStatusesDataTable;
            StorageStatusesBindingSource.DataSource = StorageStatusesDataTable;
            ExpeditionStatusesBindingSource.DataSource = ExpeditionStatusesDataTable;
            DispatchStatusesBindingSource.DataSource = DispatchStatusesDataTable;
            FirmOrderStatusesBindingSource.DataSource = FirmOrderStatusesDataTable;

            MegaOrdersBindingSource.DataSource = MegaOrdersDataTable;
            MainOrdersBindingSource.DataSource = MainOrdersDataTable;

            MainOrdersDataGrid.DataSource = MainOrdersBindingSource;
            MegaOrdersDataGrid.DataSource = MegaOrdersBindingSource;
            PackagesDataGrid.DataSource = PackagesBindingSource;

            PackagesBindingSource.DataSource = PackagesDataTable;
            using (DataView DV = new DataView(BatchDetailsDataTable))
            {
                DV.Sort = "MegaBatchID";
                BatchDetailsBindingSource.DataSource = DV.ToTable(true, "MegaBatchID");
            }

            PackageStatusesBindingSource.DataSource = PackageStatusesDataTable;
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
                HeaderText = "Производство\r\nПрофиль",
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
                HeaderText = "Производство\r\nТПС",
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
                HeaderText = "Тип\r\nпроизводства",
                DataPropertyName = "FactoryID",
                DataSource = FactoryTypesBindingSource,
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
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
            MegaFactoryTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "MegaFactoryTypeColumn",
                HeaderText = "  Тип\n\rпр-ва",
                DataPropertyName = "FactoryID",
                DataSource = new DataView(FactoryTypesDataTable),
                ValueMember = "FactoryID",
                DisplayMember = "FactoryName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            CurrencyTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CurrencyTypeColumn",
                HeaderText = "Валюта",
                DataPropertyName = "CurrencyTypeID",
                DataSource = CurrencyTypesBindingSource,
                ValueMember = "CurrencyTypeID",
                DisplayMember = "CurrencyType",
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
                MinimumWidth = 40
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
                MinimumWidth = 40
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
                MinimumWidth = 40
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
                MinimumWidth = 40
            };
            MegaOrdersDataGrid.Columns.Add(ProfilOrderStatusColumn);
            MegaOrdersDataGrid.Columns.Add(TPSOrderStatusColumn);
            MegaOrdersDataGrid.Columns.Add(MegaFactoryTypeColumn);
            MegaOrdersDataGrid.Columns.Add(CurrencyTypeColumn);

            MainOrdersDataGrid.Columns.Add(ProfilProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(ProfilDispatchStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSProductionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSStorageStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSExpeditionStatusColumn);
            MainOrdersDataGrid.Columns.Add(TPSDispatchStatusColumn);

            MainOrdersDataGrid.Columns.Add(FactoryTypeColumn);

            PackagesDataGrid.Columns.Add(PackageStatusesColumn);
            PackagesDataGrid.Columns.Add(PackUsersColumn);
            PackagesDataGrid.Columns.Add(StoreUsersColumn);
            PackagesDataGrid.Columns.Add(ExpUsersColumn);
            PackagesDataGrid.Columns.Add(DispUsersColumn);
        }

        private void ShowColumns(int FactoryID)
        {
            if (FactoryID == 0)
            {
                MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilPackPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilPackedCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilStorePercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilStoreCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilExpPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilExpCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilDispPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilDispatchedCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilProductionDate"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilOnProductionDate"].Visible = true;

                MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["TPSPackPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSPackedCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSStorePercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSStoreCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSExpPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSExpCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSDispPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSDispatchedCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSProductionDate"].Visible = true;
                MainOrdersDataGrid.Columns["TPSOnProductionDate"].Visible = true;


                MegaOrdersDataGrid.Columns["ProfilPackPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilPackedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilStorePercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilStoreCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilExpPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilExpCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilDispPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilDispatchDate"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilOrderStatusColumn"].Visible = true;

                MegaOrdersDataGrid.Columns["TPSPackPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSPackedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSStorePercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSStoreCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSExpPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSExpCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSDispPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSDispatchedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSDispatchDate"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSOrderStatusColumn"].Visible = true;
            }

            if (FactoryID == 1)
            {
                MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilPackPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilStorePercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilStoreCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilExpPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilExpCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilPackedCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilDispPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilDispatchedCount"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilProductionDate"].Visible = true;
                MainOrdersDataGrid.Columns["ProfilOnProductionDate"].Visible = true;

                MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].Visible = false;
                MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].Visible = false;
                MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].Visible = false;
                MainOrdersDataGrid.Columns["TPSPackPercentage"].Visible = false;
                MainOrdersDataGrid.Columns["TPSPackedCount"].Visible = false;
                MainOrdersDataGrid.Columns["TPSStorePercentage"].Visible = false;
                MainOrdersDataGrid.Columns["TPSStoreCount"].Visible = false;
                MainOrdersDataGrid.Columns["TPSExpPercentage"].Visible = false;
                MainOrdersDataGrid.Columns["TPSExpCount"].Visible = false;
                MainOrdersDataGrid.Columns["TPSDispPercentage"].Visible = false;
                MainOrdersDataGrid.Columns["TPSDispatchedCount"].Visible = false;
                MainOrdersDataGrid.Columns["TPSProductionDate"].Visible = false;
                MainOrdersDataGrid.Columns["TPSOnProductionDate"].Visible = false;


                MegaOrdersDataGrid.Columns["ProfilPackPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilPackedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilStorePercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilStoreCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilExpPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilExpCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilDispPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilDispatchDate"].Visible = true;
                MegaOrdersDataGrid.Columns["ProfilOrderStatusColumn"].Visible = true;

                MegaOrdersDataGrid.Columns["TPSPackPercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSPackedCount"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSStorePercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSStoreCount"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSExpPercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSExpCount"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSDispPercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSDispatchedCount"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSDispatchDate"].Visible = false;
                MegaOrdersDataGrid.Columns["TPSOrderStatusColumn"].Visible = false;
            }

            if (FactoryID == 2)
            {
                MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilPackPercentage"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilPackedCount"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilStorePercentage"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilStoreCount"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilExpPercentage"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilExpCount"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilDispPercentage"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilDispatchedCount"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilProductionDate"].Visible = false;
                MainOrdersDataGrid.Columns["ProfilOnProductionDate"].Visible = false;

                MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].Visible = true;
                MainOrdersDataGrid.Columns["TPSPackPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSPackedCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSStorePercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSStoreCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSExpPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSExpCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSDispPercentage"].Visible = true;
                MainOrdersDataGrid.Columns["TPSDispatchedCount"].Visible = true;
                MainOrdersDataGrid.Columns["TPSProductionDate"].Visible = true;
                MainOrdersDataGrid.Columns["TPSOnProductionDate"].Visible = true;


                MegaOrdersDataGrid.Columns["ProfilPackPercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilPackedCount"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilStorePercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilStoreCount"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilExpPercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilExpCount"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilDispPercentage"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilDispatchDate"].Visible = false;
                MegaOrdersDataGrid.Columns["ProfilOrderStatusColumn"].Visible = false;

                MegaOrdersDataGrid.Columns["TPSPackPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSPackedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSStorePercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSStoreCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSExpPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSExpCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSDispPercentage"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSDispatchedCount"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSDispatchDate"].Visible = true;
                MegaOrdersDataGrid.Columns["TPSOrderStatusColumn"].Visible = true;
            }
        }

        private void MainGridSetting()
        {
            foreach (DataGridViewColumn Column in MainOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            MainOrdersDataGrid.Columns["IsSample"].Visible = false;
            MainOrdersDataGrid.Columns["WillPercentID"].Visible = false;
            MainOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilProductionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilStorageStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSProductionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSStorageStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSDispatchStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["MegaOrderID"].Visible = false;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].Visible = false;
            MainOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            MainOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            MainOrdersDataGrid.Columns["AllocPackDateTime"].Visible = false;

            if (!Security.PriceAccess)
            {
                MainOrdersDataGrid.Columns["FrontsCost"].Visible = false;
                MainOrdersDataGrid.Columns["DecorCost"].Visible = false;
                MainOrdersDataGrid.Columns["OrderCost"].Visible = false;
            }

            //if (MainOrdersDataGrid.Columns.Contains("ProfilProductionDate"))
            //    MainOrdersDataGrid.Columns["ProfilProductionDate"].Visible = false;
            //if (MainOrdersDataGrid.Columns.Contains("TPSProductionDate"))
            //    MainOrdersDataGrid.Columns["TPSProductionDate"].Visible = false;
            //if (MainOrdersDataGrid.Columns.Contains("ProfilOnProductionDate"))
            //    MainOrdersDataGrid.Columns["ProfilOnProductionDate"].Visible = false;
            //if (MainOrdersDataGrid.Columns.Contains("TPSOnProductionDate"))
            //    MainOrdersDataGrid.Columns["TPSOnProductionDate"].Visible = false;

            MainOrdersDataGrid.Columns["ProfilProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MainOrdersDataGrid.Columns["TPSProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MainOrdersDataGrid.Columns["ProfilOnProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            MainOrdersDataGrid.Columns["TPSOnProductionDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            MainOrdersDataGrid.Columns["FrontsCost"].HeaderText = "Стоимость\r\nфасадов, евро";
            MainOrdersDataGrid.Columns["DecorCost"].HeaderText = "Стоимость\r\nдекора, евро";
            MainOrdersDataGrid.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            MainOrdersDataGrid.Columns["MainOrderID"].HeaderText = "№ п\\п";
            MainOrdersDataGrid.Columns["DocDateTime"].HeaderText = "Дата\r\nсоздания";
            MainOrdersDataGrid.Columns["Notes"].HeaderText = "Примечание";
            MainOrdersDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            MainOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MainOrdersDataGrid.Columns["ProfilPackPercentage"].HeaderText = "Упаковано\r\nПрофиль, %";
            MainOrdersDataGrid.Columns["ProfilPackedCount"].HeaderText = "Упаковано\r\n Профиль, кол-во";
            MainOrdersDataGrid.Columns["ProfilStoreCount"].HeaderText = "Склад,\r\nПрофиль, кол-во";
            MainOrdersDataGrid.Columns["ProfilExpCount"].HeaderText = "Экспедиция,\r\nПрофиль, кол-во";
            MainOrdersDataGrid.Columns["ProfilStorePercentage"].HeaderText = "Склад,\r\nПрофиль, %";
            MainOrdersDataGrid.Columns["ProfilExpPercentage"].HeaderText = "Экспедиция,\r\nПрофиль, %";
            MainOrdersDataGrid.Columns["ProfilDispPercentage"].HeaderText = " Отгружено\r\nПрофиль, %";
            MainOrdersDataGrid.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            MainOrdersDataGrid.Columns["TPSPackPercentage"].HeaderText = "Упаковано\r\nТПС, %";
            MainOrdersDataGrid.Columns["TPSPackedCount"].HeaderText = " Упаковано\r\nТПС, кол-во";
            MainOrdersDataGrid.Columns["TPSExpCount"].HeaderText = "Экспедиция,\r\nТПС, кол-во";
            MainOrdersDataGrid.Columns["TPSStoreCount"].HeaderText = "Склад,\r\nТПС, кол-во";
            MainOrdersDataGrid.Columns["TPSExpPercentage"].HeaderText = "Экспедиция,\r\nТПС, %";
            MainOrdersDataGrid.Columns["TPSStorePercentage"].HeaderText = "Склад,\r\nТПС, %";
            MainOrdersDataGrid.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            MainOrdersDataGrid.Columns["TPSDispatchedCount"].HeaderText = "Отгружено\r\nТПС, кол-во";
            MainOrdersDataGrid.Columns["BatchNumber"].HeaderText = "Партия";
            MainOrdersDataGrid.Columns["ProfilProductionDate"].HeaderText = "Дата входа\r\nв пр-во, Профиль";
            MainOrdersDataGrid.Columns["TPSProductionDate"].HeaderText = "Дата входа\r\nв пр-во, ТПС";
            MainOrdersDataGrid.Columns["ProfilOnProductionDate"].HeaderText = "Дата входа\r\nна пр-во, Профиль";
            MainOrdersDataGrid.Columns["TPSOnProductionDate"].HeaderText = " Дата входа\r\nна пр-во, ТПС";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["FrontsCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["DecorCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MainOrdersDataGrid.Columns["FrontsCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["FrontsCost"].Width = 130;
            MainOrdersDataGrid.Columns["DecorCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["DecorCost"].Width = 130;
            MainOrdersDataGrid.Columns["OrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //MainOrdersDataGrid.Columns["OrderCost"].Width = 130;

            MainOrdersDataGrid.Columns["ProfilProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilProductionDate"].MinimumWidth = 165;
            MainOrdersDataGrid.Columns["TPSProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSProductionDate"].MinimumWidth = 140;
            MainOrdersDataGrid.Columns["ProfilOnProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilOnProductionDate"].MinimumWidth = 165;
            MainOrdersDataGrid.Columns["TPSOnProductionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSOnProductionDate"].MinimumWidth = 140;
            MainOrdersDataGrid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["MainOrderID"].Width = 80;
            MainOrdersDataGrid.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["DocDateTime"].Width = 130;
            MainOrdersDataGrid.Columns["FrontsSquare"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["FrontsSquare"].Width = 110;
            MainOrdersDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MainOrdersDataGrid.Columns["Notes"].MinimumWidth = 60;
            MainOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["Weight"].Width = 90;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].Width = 140;

            MainOrdersDataGrid.Columns["ProfilPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilPackPercentage"].Width = 155;
            MainOrdersDataGrid.Columns["ProfilPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilPackedCount"].Width = 155;
            MainOrdersDataGrid.Columns["TPSPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSPackPercentage"].Width = 125;
            MainOrdersDataGrid.Columns["TPSPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSPackedCount"].Width = 125;

            MainOrdersDataGrid.Columns["ProfilStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilStorePercentage"].Width = 155;
            MainOrdersDataGrid.Columns["ProfilStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilStoreCount"].Width = 155;
            MainOrdersDataGrid.Columns["TPSStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSStorePercentage"].Width = 125;
            MainOrdersDataGrid.Columns["TPSStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSStoreCount"].Width = 125;

            MainOrdersDataGrid.Columns["ProfilExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilExpPercentage"].Width = 155;
            MainOrdersDataGrid.Columns["ProfilExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilExpCount"].Width = 155;
            MainOrdersDataGrid.Columns["TPSExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSExpPercentage"].Width = 125;
            MainOrdersDataGrid.Columns["TPSExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSExpCount"].Width = 125;

            MainOrdersDataGrid.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilDispPercentage"].Width = 155;
            MainOrdersDataGrid.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilDispatchedCount"].Width = 155;
            MainOrdersDataGrid.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSDispPercentage"].Width = 125;
            MainOrdersDataGrid.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSDispatchedCount"].Width = 125;

            MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilProductionStatusColumn"].Width = 150;
            MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilStorageStatusColumn"].Width = 140;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilExpeditionStatusColumn"].Width = 140;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["ProfilDispatchStatusColumn"].Width = 140;

            MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSProductionStatusColumn"].Width = 150;
            MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSStorageStatusColumn"].Width = 140;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSExpeditionStatusColumn"].Width = 140;
            MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["TPSDispatchStatusColumn"].Width = 140;

            MainOrdersDataGrid.Columns["BatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MainOrdersDataGrid.Columns["BatchNumber"].Width = 125;

            MainOrdersDataGrid.AutoGenerateColumns = false;

            MainOrdersDataGrid.Columns["MainOrderID"].DisplayIndex = 0;
            MainOrdersDataGrid.Columns["FactoryTypeColumn"].DisplayIndex = 1;

            MainOrdersDataGrid.Columns["ProfilPackPercentage"].DisplayIndex = 2;
            MainOrdersDataGrid.Columns["ProfilPackedCount"].DisplayIndex = 3;
            MainOrdersDataGrid.Columns["ProfilStorePercentage"].DisplayIndex = 4;
            MainOrdersDataGrid.Columns["ProfilStoreCount"].DisplayIndex = 5;
            MainOrdersDataGrid.Columns["ProfilDispPercentage"].DisplayIndex = 6;
            MainOrdersDataGrid.Columns["ProfilDispatchedCount"].DisplayIndex = 7;

            MainOrdersDataGrid.Columns["TPSPackPercentage"].DisplayIndex = 8;
            MainOrdersDataGrid.Columns["TPSPackedCount"].DisplayIndex = 9;
            MainOrdersDataGrid.Columns["TPSStorePercentage"].DisplayIndex = 10;
            MainOrdersDataGrid.Columns["TPSStoreCount"].DisplayIndex = 11;
            MainOrdersDataGrid.Columns["TPSDispPercentage"].DisplayIndex = 12;
            MainOrdersDataGrid.Columns["TPSDispatchedCount"].DisplayIndex = 13;

            MainOrdersDataGrid.Columns["FrontsSquare"].DisplayIndex = 14;
            MainOrdersDataGrid.Columns["Weight"].DisplayIndex = 15;
            MainOrdersDataGrid.Columns["FrontsCost"].DisplayIndex = 16;
            MainOrdersDataGrid.Columns["DecorCost"].DisplayIndex = 17;
            MainOrdersDataGrid.Columns["OrderCost"].DisplayIndex = 18;
            MainOrdersDataGrid.Columns["Notes"].DisplayIndex = 19;
            MainOrdersDataGrid.Columns["DocDateTime"].DisplayIndex = 20;

            MainOrdersDataGrid.Columns["BatchNumber"].DisplayIndex = 2;

            MainOrdersDataGrid.Columns["TPSExpPercentage"].DisplayIndex = 13;
            MainOrdersDataGrid.Columns["TPSExpCount"].DisplayIndex = 13;
            MainOrdersDataGrid.Columns["ProfilExpCount"].DisplayIndex = 7;
            MainOrdersDataGrid.Columns["ProfilExpPercentage"].DisplayIndex = 7;

            MainOrdersDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MainOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MainOrdersDataGrid.Columns["ProfilPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["ProfilPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("ProfilPackPercentage");
            MainOrdersDataGrid.Columns["ProfilStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["ProfilStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("ProfilStorePercentage");
            MainOrdersDataGrid.Columns["ProfilExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["ProfilExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("ProfilExpPercentage");
            MainOrdersDataGrid.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("ProfilDispPercentage");

            MainOrdersDataGrid.Columns["TPSPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["TPSPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("TPSPackPercentage");
            MainOrdersDataGrid.Columns["TPSStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["TPSStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("TPSStorePercentage");
            MainOrdersDataGrid.Columns["TPSExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["TPSExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("TPSExpPercentage");
            MainOrdersDataGrid.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MainOrdersDataGrid.AddPercentageColumn("TPSDispPercentage");
        }

        private void MegaGridSetting()
        {
            foreach (DataGridViewColumn Column in MegaOrdersDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (MegaOrdersDataGrid.Columns.Contains("DelayOfPayment"))
                MegaOrdersDataGrid.Columns["DelayOfPayment"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("CreatedByClient"))
                MegaOrdersDataGrid.Columns["CreatedByClient"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("FixedPaymentRate"))
                MegaOrdersDataGrid.Columns["FixedPaymentRate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TransportType"))
                MegaOrdersDataGrid.Columns["TransportType"].Visible = false;
            MegaOrdersDataGrid.Columns["ClientID"].Visible = false;
            MegaOrdersDataGrid.Columns["OrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilOrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSOrderStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["AgreementStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["CurrencyTypeID"].Visible = false;
            MegaOrdersDataGrid.Columns["FactoryID"].Visible = false;
            MegaOrdersDataGrid.Columns["DesireDate"].Visible = false;
            MegaOrdersDataGrid.Columns["LastCalcDate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("LastCalcUserID"))
                MegaOrdersDataGrid.Columns["LastCalcUserID"].Visible = false;
            MegaOrdersDataGrid.Columns["OrderDate"].Visible = false;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackAllocStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["TPSPackCount"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilPackAllocStatusID"].Visible = false;
            MegaOrdersDataGrid.Columns["ProfilPackCount"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("IsComplaint"))
                MegaOrdersDataGrid.Columns["IsComplaint"].Visible = false;

            if (!Security.PriceAccess)
            {
                if (MegaOrdersDataGrid.Columns.Contains("ComplaintProfilCost"))
                    MegaOrdersDataGrid.Columns["ComplaintProfilCost"].Visible = false;
                if (MegaOrdersDataGrid.Columns.Contains("ComplaintTPSCost"))
                    MegaOrdersDataGrid.Columns["ComplaintTPSCost"].Visible = false;
                if (MegaOrdersDataGrid.Columns.Contains("ComplaintNotes"))
                    MegaOrdersDataGrid.Columns["ComplaintNotes"].Visible = false;
                if (MegaOrdersDataGrid.Columns.Contains("CurrencyComplaintProfilCost"))
                    MegaOrdersDataGrid.Columns["CurrencyComplaintProfilCost"].Visible = false;
                if (MegaOrdersDataGrid.Columns.Contains("CurrencyComplaintTPSCost"))
                    MegaOrdersDataGrid.Columns["CurrencyComplaintTPSCost"].Visible = false;
                if (MegaOrdersDataGrid.Columns.Contains("DelayOfPayment"))
                    MegaOrdersDataGrid.Columns["DelayOfPayment"].Visible = false;
                MegaOrdersDataGrid.Columns["OrderCost"].Visible = false;
                MegaOrdersDataGrid.Columns["TransportCost"].Visible = false;
                MegaOrdersDataGrid.Columns["AdditionalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyOrderCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyTotalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyTransportCost"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["TotalCost"].Visible = false;
                MegaOrdersDataGrid.Columns["Rate"].Visible = false;
                MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].Visible = false;
            }

            if (MegaOrdersDataGrid.Columns.Contains("ProfilConfirmProduction"))
                MegaOrdersDataGrid.Columns["ProfilConfirmProduction"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSConfirmProduction"))
                MegaOrdersDataGrid.Columns["TPSConfirmProduction"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ProfilAllowDispatch"))
                MegaOrdersDataGrid.Columns["ProfilAllowDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSAllowDispatch"))
                MegaOrdersDataGrid.Columns["TPSAllowDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("ProfilConfirmDispatch"))
                MegaOrdersDataGrid.Columns["ProfilConfirmDispatch"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSConfirmDispatch"))
                MegaOrdersDataGrid.Columns["TPSConfirmDispatch"].Visible = false;

            if (MegaOrdersDataGrid.Columns.Contains("ProfilProductionDate"))
                MegaOrdersDataGrid.Columns["ProfilProductionDate"].Visible = false;
            if (MegaOrdersDataGrid.Columns.Contains("TPSProductionDate"))
                MegaOrdersDataGrid.Columns["TPSProductionDate"].Visible = false;

            MegaOrdersDataGrid.Columns["OrderCost"].HeaderText = "Стоимость\r\nзаказа, евро";
            MegaOrdersDataGrid.Columns["TransportCost"].HeaderText = "Стоимость\r\nтранспорта, евро";
            MegaOrdersDataGrid.Columns["AdditionalCost"].HeaderText = "Дополнительная\r\nстоимость, евро";
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].HeaderText = "Дата\r\nсогласования";
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].HeaderText = "Стоимость\r\nзаказа, расчет";
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].HeaderText = "Итого, расчет";
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].HeaderText = "Стоимость \r\nтранспорта, расчет";
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].HeaderText = "Дополнительная\r\nстоимость, расчет";
            MegaOrdersDataGrid.Columns["TotalCost"].HeaderText = "Итого, евро";
            MegaOrdersDataGrid.Columns["Rate"].HeaderText = "Курс";

            MegaOrdersDataGrid.Columns["MegaOrderID"].HeaderText = "№ п\\п";
            MegaOrdersDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            MegaOrdersDataGrid.Columns["OrderNumber"].HeaderText = "№\r\nзаказа";
            MegaOrdersDataGrid.Columns["OrderDate"].HeaderText = "Дата\r\nсоздания";
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].HeaderText = "Дата отгрузки\r\nПрофиль";
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].HeaderText = "Дата отгрузки\r\nТПС";
            MegaOrdersDataGrid.Columns["AdditionalCost"].HeaderText = "Дополнительная\r\nстоимость, евро";
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].HeaderText = "Дата\r\nсогласования";
            MegaOrdersDataGrid.Columns["Weight"].HeaderText = "Вес, кг.";
            MegaOrdersDataGrid.Columns["Square"].HeaderText = "Площадь\r\nфасадов, м.кв.";
            MegaOrdersDataGrid.Columns["DesireDate"].HeaderText = "Предварит.\r\nдата отгрузки";

            MegaOrdersDataGrid.Columns["ProfilPackPercentage"].HeaderText = "Упаковано\r\nПрофиль, %";
            MegaOrdersDataGrid.Columns["ProfilPackedCount"].HeaderText = "Упаковано\r\n Профиль, кол-во";
            MegaOrdersDataGrid.Columns["ProfilStoreCount"].HeaderText = "Склад,\r\nПрофиль, кол-во";
            MegaOrdersDataGrid.Columns["ProfilStorePercentage"].HeaderText = "Склад,\r\nПрофиль, %";
            MegaOrdersDataGrid.Columns["ProfilExpCount"].HeaderText = "Экспедиция,\r\nПрофиль, кол-во";
            MegaOrdersDataGrid.Columns["ProfilExpPercentage"].HeaderText = "Экспедиция,\r\nПрофиль, %";
            MegaOrdersDataGrid.Columns["ProfilDispPercentage"].HeaderText = "Отгружено\r\nПрофиль, %";
            MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].HeaderText = "Отгружено\r\n Профиль, кол-во";
            MegaOrdersDataGrid.Columns["TPSPackPercentage"].HeaderText = "Упаковано\r\nТПС, %";
            MegaOrdersDataGrid.Columns["TPSPackedCount"].HeaderText = "Упаковано\r\nТПС, кол-во";
            MegaOrdersDataGrid.Columns["TPSStoreCount"].HeaderText = "Склад,\r\nТПС, кол-во";
            MegaOrdersDataGrid.Columns["TPSStorePercentage"].HeaderText = "Склад,\r\nТПС, %";
            MegaOrdersDataGrid.Columns["TPSExpCount"].HeaderText = "Экспедиция,\r\nТПС, кол-во";
            MegaOrdersDataGrid.Columns["TPSExpPercentage"].HeaderText = "Экспедиция,\r\nТПС, %";
            MegaOrdersDataGrid.Columns["TPSDispPercentage"].HeaderText = "Отгружено\r\nТПС, %";
            MegaOrdersDataGrid.Columns["TPSDispatchedCount"].HeaderText = " Отгружено\r\nТПС, кол-во";
            MegaOrdersDataGrid.Columns["IsComplaint"].Visible = false;
            MegaOrdersDataGrid.Columns["MegaBatchNumber"].HeaderText = "Группа\n\rпартий";

            MegaOrdersDataGrid.Columns["PlanDispDate"].HeaderText = "Плановая\r\nдата отгрузки";
            MegaOrdersDataGrid.Columns["PlanDispDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["PlanDispDate"].Width = 110;
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
            MegaOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["OrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["TransportCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["TransportCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["AdditionalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["AdditionalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["TotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Rate"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Rate"].DefaultCellStyle.FormatProvider = nfi2;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi3;

            MegaOrdersDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MegaOrdersDataGrid.Columns["ClientName"].MinimumWidth = 150;
            MegaOrdersDataGrid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaOrderID"].Width = 70;
            MegaOrdersDataGrid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderNumber"].Width = 70;
            MegaOrdersDataGrid.Columns["OrderDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderDate"].Width = 150;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].Width = 140;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].Width = 140;
            MegaOrdersDataGrid.Columns["DesireDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["DesireDate"].Width = 130;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ConfirmDateTime"].Width = 130;
            MegaOrdersDataGrid.Columns["ProfilOrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilOrderStatusColumn"].Width = 160;
            MegaOrdersDataGrid.Columns["TPSOrderStatusColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSOrderStatusColumn"].Width = 160;
            MegaOrdersDataGrid.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Weight"].Width = 110;
            MegaOrdersDataGrid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Square"].Width = 140;

            MegaOrdersDataGrid.Columns["OrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["OrderCost"].Width = 130;
            MegaOrdersDataGrid.Columns["TotalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TotalCost"].Width = 130;
            MegaOrdersDataGrid.Columns["TransportCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TransportCost"].Width = 150;
            MegaOrdersDataGrid.Columns["AdditionalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["AdditionalCost"].Width = 150;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].Width = 130;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].Width = 130;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].Width = 190;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].Width = 170;
            MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].Width = 90;
            MegaOrdersDataGrid.Columns["Rate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["Rate"].Width = 100;

            MegaOrdersDataGrid.Columns["ProfilPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilPackPercentage"].Width = 155;
            MegaOrdersDataGrid.Columns["ProfilPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilPackedCount"].Width = 155;
            MegaOrdersDataGrid.Columns["TPSPackPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSPackPercentage"].Width = 125;
            MegaOrdersDataGrid.Columns["TPSPackedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSPackedCount"].Width = 125;

            MegaOrdersDataGrid.Columns["ProfilStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilStorePercentage"].Width = 155;
            MegaOrdersDataGrid.Columns["ProfilStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilStoreCount"].Width = 155;
            MegaOrdersDataGrid.Columns["TPSStorePercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSStorePercentage"].Width = 125;
            MegaOrdersDataGrid.Columns["TPSStoreCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSStoreCount"].Width = 125;

            MegaOrdersDataGrid.Columns["ProfilExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilExpPercentage"].Width = 155;
            MegaOrdersDataGrid.Columns["ProfilExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilExpCount"].Width = 155;
            MegaOrdersDataGrid.Columns["TPSExpPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSExpPercentage"].Width = 125;
            MegaOrdersDataGrid.Columns["TPSExpCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSExpCount"].Width = 125;

            MegaOrdersDataGrid.Columns["ProfilDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilDispPercentage"].Width = 155;
            MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].Width = 155;
            MegaOrdersDataGrid.Columns["TPSDispPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSDispPercentage"].Width = 125;
            MegaOrdersDataGrid.Columns["TPSDispatchedCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["TPSDispatchedCount"].Width = 125;

            MegaOrdersDataGrid.Columns["MegaFactoryTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaFactoryTypeColumn"].Width = 130;

            MegaOrdersDataGrid.Columns["MegaBatchNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MegaOrdersDataGrid.Columns["MegaBatchNumber"].Width = 80;

            MegaOrdersDataGrid.AutoGenerateColumns = false;

            MegaOrdersDataGrid.Columns["ClientName"].DisplayIndex = 0;
            MegaOrdersDataGrid.Columns["OrderNumber"].DisplayIndex = 1;
            MegaOrdersDataGrid.Columns["MegaOrderID"].DisplayIndex = 2;

            MegaOrdersDataGrid.Columns["ProfilPackPercentage"].DisplayIndex = 3;
            MegaOrdersDataGrid.Columns["ProfilPackedCount"].DisplayIndex = 4;
            MegaOrdersDataGrid.Columns["ProfilStorePercentage"].DisplayIndex = 5;
            MegaOrdersDataGrid.Columns["ProfilStoreCount"].DisplayIndex = 6;
            MegaOrdersDataGrid.Columns["ProfilDispPercentage"].DisplayIndex = 7;
            MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].DisplayIndex = 8;

            MegaOrdersDataGrid.Columns["TPSPackPercentage"].DisplayIndex = 9;
            MegaOrdersDataGrid.Columns["TPSPackedCount"].DisplayIndex = 10;
            MegaOrdersDataGrid.Columns["TPSStorePercentage"].DisplayIndex = 11;
            MegaOrdersDataGrid.Columns["TPSStoreCount"].DisplayIndex = 12;
            MegaOrdersDataGrid.Columns["TPSDispPercentage"].DisplayIndex = 13;
            MegaOrdersDataGrid.Columns["TPSDispatchedCount"].DisplayIndex = 14;

            MegaOrdersDataGrid.Columns["ProfilDispatchDate"].DisplayIndex = 15;
            MegaOrdersDataGrid.Columns["TPSDispatchDate"].DisplayIndex = 16;

            MegaOrdersDataGrid.Columns["MegaFactoryTypeColumn"].DisplayIndex = 17;
            MegaOrdersDataGrid.Columns["ProfilOrderStatusColumn"].DisplayIndex = 18;
            MegaOrdersDataGrid.Columns["TPSOrderStatusColumn"].DisplayIndex = 19;
            MegaOrdersDataGrid.Columns["Square"].DisplayIndex = 20;
            MegaOrdersDataGrid.Columns["Weight"].DisplayIndex = 21;
            MegaOrdersDataGrid.Columns["OrderCost"].DisplayIndex = 22;
            MegaOrdersDataGrid.Columns["TransportCost"].DisplayIndex = 23;
            MegaOrdersDataGrid.Columns["AdditionalCost"].DisplayIndex = 24;
            MegaOrdersDataGrid.Columns["TotalCost"].DisplayIndex = 25;
            MegaOrdersDataGrid.Columns["CurrencyOrderCost"].DisplayIndex = 26;
            MegaOrdersDataGrid.Columns["CurrencyTransportCost"].DisplayIndex = 27;
            MegaOrdersDataGrid.Columns["CurrencyAdditionalCost"].DisplayIndex = 28;
            MegaOrdersDataGrid.Columns["CurrencyTotalCost"].DisplayIndex = 29;
            MegaOrdersDataGrid.Columns["CurrencyTypeColumn"].DisplayIndex = 30;
            MegaOrdersDataGrid.Columns["Rate"].DisplayIndex = 31;

            MegaOrdersDataGrid.Columns["MegaBatchNumber"].DisplayIndex = 3;

            MegaOrdersDataGrid.Columns["ProfilExpPercentage"].DisplayIndex = 8;
            MegaOrdersDataGrid.Columns["ProfilExpCount"].DisplayIndex = 8;
            MegaOrdersDataGrid.Columns["TPSExpCount"].DisplayIndex = 16;
            MegaOrdersDataGrid.Columns["TPSExpPercentage"].DisplayIndex = 16;

            MegaOrdersDataGrid.Columns["ClientName"].Frozen = true;
            MegaOrdersDataGrid.Columns["OrderNumber"].Frozen = true;
            MegaOrdersDataGrid.RightToLeft = RightToLeft.No;

            MegaOrdersDataGrid.Columns["Weight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            MegaOrdersDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            MegaOrdersDataGrid.Columns["ProfilPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["ProfilPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("ProfilPackPercentage");
            MegaOrdersDataGrid.Columns["ProfilStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["ProfilStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("ProfilStorePercentage");
            MegaOrdersDataGrid.Columns["ProfilExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["ProfilExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("ProfilExpPercentage");
            MegaOrdersDataGrid.Columns["ProfilDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["ProfilDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("ProfilDispPercentage");

            MegaOrdersDataGrid.Columns["TPSPackedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["TPSPackPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("TPSPackPercentage");
            MegaOrdersDataGrid.Columns["TPSStoreCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["TPSStorePercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("TPSStorePercentage");
            MegaOrdersDataGrid.Columns["TPSExpCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["TPSExpPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("TPSExpPercentage");
            MegaOrdersDataGrid.Columns["TPSDispatchedCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.Columns["TPSDispPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            MegaOrdersDataGrid.AddPercentageColumn("TPSDispPercentage");
        }

        public void ShowPackUsersColumn(bool bShow)
        {
            if (PackagesDataGrid.Columns.Contains("PackUsersColumn"))
                PackagesDataGrid.Columns["PackUsersColumn"].Visible = bShow;
        }

        public void ShowStoreUsersColumn(bool bShow)
        {
            if (PackagesDataGrid.Columns.Contains("StoreUsersColumn"))
                PackagesDataGrid.Columns["StoreUsersColumn"].Visible = bShow;
        }

        public void ShowExpUsersColumn(bool bShow)
        {
            if (PackagesDataGrid.Columns.Contains("ExpUsersColumn"))
                PackagesDataGrid.Columns["ExpUsersColumn"].Visible = bShow;
        }

        public void ShowDispUsersColumn(bool bShow)
        {
            if (PackagesDataGrid.Columns.Contains("DispUsersColumn"))
                PackagesDataGrid.Columns["DispUsersColumn"].Visible = bShow;
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
            if (PackagesDataGrid.Columns.Contains("PackUsersColumn"))
                PackagesDataGrid.Columns["PackUsersColumn"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("StoreUsersColumn"))
                PackagesDataGrid.Columns["StoreUsersColumn"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("ExpUsersColumn"))
                PackagesDataGrid.Columns["ExpUsersColumn"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("DispUsersColumn"))
                PackagesDataGrid.Columns["DispUsersColumn"].Visible = false;

            if (PackagesDataGrid.Columns.Contains("ProductType"))
                PackagesDataGrid.Columns["ProductType"].Visible = false;
            if (PackagesDataGrid.Columns.Contains("FactoryID"))
                PackagesDataGrid.Columns["FactoryID"].Visible = false;

            PackagesDataGrid.Columns["PrintDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["PackingDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["StorageDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["ExpeditionDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["DispatchDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            PackagesDataGrid.Columns["PackNumber"].HeaderText = "№\r\nупак.";
            PackagesDataGrid.Columns["PrintedCount"].HeaderText = "Кол-во\r\nраспечаток";
            PackagesDataGrid.Columns["PrintDateTime"].HeaderText = "Дата\r\nпечати";
            PackagesDataGrid.Columns["PackingDateTime"].HeaderText = "Дата\r\nупаковки";
            PackagesDataGrid.Columns["StorageDateTime"].HeaderText = "Принято\r\nна склад";
            PackagesDataGrid.Columns["ExpeditionDateTime"].HeaderText = "Дата\r\nэкспедиции";
            PackagesDataGrid.Columns["DispatchDateTime"].HeaderText = "Дата\r\nотгрузки";
            PackagesDataGrid.Columns["PackageID"].HeaderText = "ID";
            PackagesDataGrid.Columns["FactoryName"].HeaderText = "Участок";
            PackagesDataGrid.Columns["TrayID"].HeaderText = "№\r\nпод.";

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
            PackagesDataGrid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["FactoryName"].Width = 100;
            PackagesDataGrid.Columns["TrayID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PackagesDataGrid.Columns["TrayID"].Width = 100;

            PackagesDataGrid.AutoGenerateColumns = false;

            PackagesDataGrid.Columns["PackNumber"].DisplayIndex = 0;
            PackagesDataGrid.Columns["PackageStatusesColumn"].DisplayIndex = 1;
            PackagesDataGrid.Columns["FactoryName"].DisplayIndex = 2;
            PackagesDataGrid.Columns["TrayID"].DisplayIndex = 3;
            PackagesDataGrid.Columns["PrintDateTime"].DisplayIndex = 4;
            PackagesDataGrid.Columns["PackingDateTime"].DisplayIndex = 5;
            PackagesDataGrid.Columns["StorageDateTime"].DisplayIndex = 6;
            PackagesDataGrid.Columns["ExpeditionDateTime"].DisplayIndex = 7;
            PackagesDataGrid.Columns["DispatchDateTime"].DisplayIndex = 8;
            PackagesDataGrid.Columns["PackageID"].DisplayIndex = 9;
            //PackagesDataGrid.Columns["PrintedCount"].DisplayIndex = 8;

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
            FillMegaPercentageColumn();
            FillMegaBatchNumber();
            CreateColumns();
            MainGridSetting();
            MegaGridSetting();
            PackagesGridSetting();
        }
        #endregion

        #region Filter

        public void Clear()
        {
            MainOrdersDataTable.Clear();
            PackagesDataTable.Clear();
            PackedMainOrdersFrontsOrders.FrontsOrdersDataTable.Clear();
            PackedMainOrdersDecorOrders.DecorOrdersDataTable.Clear();
        }

        public void FilterMegaBatch(
            bool ByClient,
            bool NotProduction,
            bool OnProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExpedition,
            bool Dispatched,
            int ClientID,
            int FactoryID)
        {
            string BatchFilter = string.Empty;
            string FilterClient = string.Empty;
            string FilterFactory = string.Empty;
            string FilterMegaBatchFactory = string.Empty;
            string FilterOrdersStatus = string.Empty;

            if (ByClient)
                FilterClient = "ClientID = " + ClientID;

            if (FactoryID != 0)
                FilterMegaBatchFactory = "BFactoryID = " + FactoryID;

            if (FactoryID > 0)
                FilterFactory = "MFactoryID = 0 OR MFactoryID = " + FactoryID;

            if (FactoryID == -1)
                FilterFactory = "MFactoryID = -1";

            #region Orders Filter

            if (NotProduction)
            {
                FilterOrdersStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID=1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1))";

                if (FactoryID == 1)
                    FilterOrdersStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    FilterOrdersStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (FilterOrdersStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus = "(ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (FilterOrdersStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                    if (FactoryID == 1)
                        FilterOrdersStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        FilterOrdersStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus = "(ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                    if (FactoryID == 1)
                        FilterOrdersStatus = "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        FilterOrdersStatus = "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (FilterOrdersStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus = "(TPSStorageStatusID=2)";
                }
            }

            if (OnExpedition)
            {
                if (FilterOrdersStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus += " OR (ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus += " OR (TPSExpeditionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus = "(ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus = "(TPSExpeditionStatusID=2)";
                }
            }
            if (Dispatched)
            {
                if (FilterOrdersStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        FilterOrdersStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        FilterOrdersStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        FilterOrdersStatus = "(TPSDispatchStatusID=2)";
                }

            }

            #endregion

            if (FilterClient.Length > 0)
                BatchFilter = "(" + FilterClient + ")";

            if (FilterMegaBatchFactory.Length > 0)
            {
                if (BatchFilter.Length > 0)
                    BatchFilter = BatchFilter + " AND (" + FilterMegaBatchFactory + ")";
                else
                    BatchFilter = "(" + FilterMegaBatchFactory + ")";
            }

            if (FilterFactory.Length > 0)
            {
                if (BatchFilter.Length > 0)
                    BatchFilter = BatchFilter + " AND (" + FilterFactory + ")";
                else
                    BatchFilter = "(" + FilterFactory + ")";
            }

            if (FilterOrdersStatus.Length > 0)
            {
                if (BatchFilter.Length > 0)
                    BatchFilter = BatchFilter + " AND (" + FilterOrdersStatus + ")";
                else
                    BatchFilter = "(" + FilterOrdersStatus + ")";
            }

            if (!NotProduction && !OnProduction && !InProduction && !OnStorage && !OnExpedition && !Dispatched)
                BatchFilter = "MegaBatchID = -1";

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(BatchDetailsDataTable))
            {
                DV.RowFilter = BatchFilter;
                DT = DV.ToTable();
            }
            using (DataView DV = new DataView(DT))
            {
                DV.Sort = "MegaBatchID";
                BatchDetailsBindingSource.DataSource = DV.ToTable(true, "MegaBatchID");
            }

            DT.Dispose();
            BatchDetailsBindingSource.MoveFirst();
        }

        public void Filter(bool ByClient,
            bool ByMegaBatch,
            bool NotProduction,
            bool OnProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExpedition,
            bool Dispatched,
            int ClientID, int MegaBatchID, int FactoryID)
        {
            string OrdersProductionStatus = string.Empty;
            string BatchOrdersProductionStatus = string.Empty;
            string MegaBatchFilter = string.Empty;
            string MegaBatchFactoryFilter = string.Empty;
            string FilterClient = string.Empty;
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                MegaBatchFactoryFilter = " AND (FactoryID = " + FactoryID + ")";

            if (FactoryID > 0)
                FactoryFilter = " (FactoryID = 0 OR FactoryID = " + FactoryID + ")";

            if (FactoryID == -1)
                FactoryFilter = " (FactoryID = -1)";

            if (ByClient && ClientID != -1)
                FilterClient = " MegaOrders.ClientID = " + ClientID + " AND ";

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1))";
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }
            if (OnExpedition)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSExpeditionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSExpeditionStatusID=2)";
                }
            }
            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }

            }
            #endregion

            if (FactoryFilter.Length > 0)
                FactoryFilter = " WHERE (" + FactoryFilter + ")";
            BatchOrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
            if (OrdersProductionStatus.Length > 0)
            {
                if (FactoryFilter.Length > 0)
                    OrdersProductionStatus = FactoryFilter + " AND (" + OrdersProductionStatus + ")";
                else
                    OrdersProductionStatus = " WHERE (" + OrdersProductionStatus + ")";
            }
            else
            {
                OrdersProductionStatus = FactoryFilter;
            }
            if (ByMegaBatch)
            {
                MegaBatchFilter = " MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE BatchID IN (SELECT BatchID FROM Batch WHERE MegaBatchID = " + MegaBatchID + ")" + MegaBatchFactoryFilter + ")";
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " AND (" + MegaBatchFilter + ")";
                }
                else
                {
                    OrdersProductionStatus = " WHERE (" + MegaBatchFilter + ")";
                }
            }

            if (!NotProduction && !OnProduction && !InProduction && !OnStorage && !OnExpedition && !Dispatched)
                OrdersProductionStatus = " WHERE MainOrderID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT MegaOrders.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MegaOrders" +
                " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID" +
                " WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND " + FilterClient + " MegaOrders.MegaOrderID IN (SELECT MegaOrderID FROM MainOrders " + OrdersProductionStatus +
                ") ORDER BY ClientName, OrderNumber",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                MegaOrdersDataTable.Clear();
                DA.Fill(MegaOrdersDataTable);
            }

            BatchDetailsDataTable.Clear();
            TempBatchDetailsDataTable.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Batch.MegaBatchID, BatchDetails.BatchID," +
                " BatchDetails.MainOrderID, MainOrders.MegaOrderID, ClientID," +
                " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID, MainOrders.FactoryID AS MFactoryID, BatchDetails.FactoryID AS BFactoryID" +
                " FROM BatchDetails" +
                " INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID" +
                " INNER JOIN MainOrders ON BatchDetails.MainOrderID = MainOrders.MainOrderID" + BatchOrdersProductionStatus +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " ORDER BY Batch.MegaBatchID, BatchDetails.BatchID, BatchDetails.MainOrderID, ClientID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(BatchDetailsDataTable);
            }

            using (DataView DV = new DataView(BatchDetailsDataTable))
            {
                DV.Sort = "MegaBatchID";
                TempBatchDetailsDataTable = DV.ToTable(true, "MegaBatchID", "BatchID", "MegaOrderID", "ClientID");
            }

            FillMegaPercentageColumn();
            if (!ByMegaBatch)
                FillMegaBatchNumber();
            ShowColumns(FactoryID);

            MegaOrdersBindingSource.MoveFirst();
        }

        public void FilterMainOrdersByMegaOrder(
            bool ByBatch,
            bool NotProduction,
            bool OnProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExpedition,
            bool Dispatched,
            int MegaOrderID,
            int MegaBatchID,
            int FactoryID)
        {
            if (MegaOrdersBindingSource.Count == 0)
                return;

            if (CurrentMainOrderID == MegaOrderID)
                return;
            CurrentMegaOrderID = MegaOrderID;

            string BatchFilter = string.Empty;
            string BatchFactoryFilter = string.Empty;
            string OrdersProductionStatus = string.Empty;
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                BatchFactoryFilter = " AND FactoryID = " + FactoryID;

            if (FactoryID != 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";

            #region Orders
            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1))";
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1)";
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
            if (OnExpedition)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSExpeditionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSExpeditionStatusID=2)";
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
            #endregion

            if (OrdersProductionStatus.Length > 0)
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";

            if (ByBatch)
                BatchFilter = " AND MainOrderID IN (SELECT MainOrderID FROM BatchDetails WHERE BatchID IN (SELECT BatchID FROM Batch WHERE MegaBatchID = " + MegaBatchID + ")" + BatchFactoryFilter + ")";

            if (!NotProduction && !OnProduction && !InProduction && !OnStorage && !OnExpedition && !Dispatched)
                OrdersProductionStatus = " WHERE MainOrderID = -1";

            MainOrdersDataTable.Clear();

            string SelectionCommand = "SELECT * FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + OrdersProductionStatus + BatchFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);
            }

            FillMainPercentageColumn();
            FillBatchNumber();

            MainOrdersBindingSource.MoveFirst();
        }

        public bool FilterPackagesByMainOrder(int MainOrderID, int FactoryID)
        {
            PackagesDataTable.Clear();

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND Packages.FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT Packages.*, FactoryName FROM Packages" +
                " INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.AreaID = infiniu2_catalog.dbo.Factory.FactoryID" +
                " WHERE MainOrderID = " + MainOrderID + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }
            PackagesDataTable.DefaultView.Sort = "PackNumber ASC";
            PackagesBindingSource.MoveFirst();
            return PackagesDataTable.Rows.Count > 0;
        }

        public void FilterPackagesByMegaOrder(int MegaOrderID, int FactoryID)
        {
            string FactoryFilter = "";
            string MainOrdersFactoryFilter = "";

            if (FactoryID != 0)
            {
                FactoryFilter = " AND Packages.FactoryID = " + FactoryID;
                MainOrdersFactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";
            }


            OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.FilterByMegaOrder(MegaOrderID, FactoryID);
            OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.FilterByMegaOrder(MegaOrderID, FactoryID);

            PackagesDataTable.Clear();

            string SelectionCommand = "SELECT * FROM Packages" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + MainOrdersFactoryFilter + ") " + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDataTable);
            }
            PackagesDataTable.DefaultView.Sort = "MainOrderID, PackNumber ASC";
            PackagesBindingSource.MoveFirst();

        }

        public void FilterProductsByPackage()
        {
            CurrentMainOrderID = -1;
        }
        #endregion

        public void FilterProductsByPackage(int MainOrderID, int FactoryID)
        {
            if (CurrentMainOrderID == MainOrderID)
                return;
            CurrentMainOrderID = MainOrderID;
            OrdersTabControl.TabPages[0].PageVisible = PackedMainOrdersFrontsOrders.FilterByMainOrder(MainOrderID, FactoryID);
            OrdersTabControl.TabPages[1].PageVisible = PackedMainOrdersDecorOrders.FilterByMainOrder(MainOrderID, FactoryID);
        }

        public int[] GetSelectedMainOrders()
        {
            int[] rows = new int[MainOrdersDataGrid.SelectedRows.Count];

            for (int i = 0; i < MainOrdersDataGrid.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(MainOrdersDataGrid.SelectedRows[i].Cells["MainOrderID"].Value);
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

        private int GetMainOrderPackCount(int MainOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = PackMainOrdersDataTable.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMainOrderDispCount(int MainOrderID, int FactoryID)
        {
            int DispCount = 0;
            DataRow[] Rows = DispMainOrdersDataTable.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                DispCount = Convert.ToInt32(Rows[0]["Count"]);

            return DispCount;
        }

        private int GetMegaOrderPackCount(int MegaOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = PackMegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMegaOrderDispCount(int MegaOrderID, int FactoryID)
        {
            int DispCount = 0;
            DataRow[] Rows = DispMegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                DispCount = Convert.ToInt32(Rows[0]["Count"]);

            return DispCount;
        }

        private int GetMainOrderStoreCount(int MainOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = StoreMainOrdersDataTable.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMegaOrderStoreCount(int MegaOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = StoreMegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMainOrderExpCount(int MainOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = ExpMainOrdersDataTable.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
        }

        private int GetMegaOrderExpCount(int MegaOrderID, int FactoryID)
        {
            int PackCount = 0;
            DataRow[] Rows = ExpMegaOrdersDataTable.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                PackCount = Convert.ToInt32(Rows[0]["Count"]);

            return PackCount;
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
                BatchNumber = MegaBatch + ", " + Batch + ", " + MainOrderID.ToString();
            }

            return BatchNumber;
        }

        private string GetMegaBatchNumber(int MegaOrderID)
        {
            string MegaBatch = string.Empty;

            DataRow[] Rows = TempBatchDetailsDataTable.Select("MegaOrderID = " + MegaOrderID);

            if (Rows.Count() > 0)
            {
                MegaBatch = Rows[0]["MegaBatchID"].ToString();
            }

            return MegaBatch;
        }

        public void Fuck(int[] MegaOrders)
        {
            DateTime DispDate = DateTime.Now;
            string Date = string.Empty;

            using (SqlDataAdapter mDA = new SqlDataAdapter(
                " SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrders) + ")" +
                " ORDER BY MainOrdeID",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable mDT = new DataTable())
                {
                    mDA.Fill(mDT);

                    for (int i = 0; i < mDT.Rows.Count; i++)
                    {
                        using (SqlDataAdapter pDA = new SqlDataAdapter(
                            " SELECT * FROM Packages" +
                            " WHERE MainOrderID = " + Convert.ToInt32(mDT.Rows[i]["MainOrderID"]),
                            ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            using (DataTable pDT = new DataTable())
                            {
                                pDA.Fill(pDT);

                                if (pDT.Rows.Count < 1)
                                    continue;

                                if (pDT.Rows[0]["DispatchDateTime"] != DBNull.Value)
                                    DispDate = Convert.ToDateTime(pDT.Rows[0]["DispatchDateTime"]);

                                for (int j = 1; j < pDT.Rows.Count; j++)
                                {
                                    if (pDT.Rows[j]["DispatchDateTime"] != DBNull.Value)
                                    {
                                        DispDate = Convert.ToDateTime(pDT.Rows[j]["DispatchDateTime"]);
                                        Date = DispDate.ToShortDateString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public int SearchPackedOrders(int ClientID)
        {
            bool OrderPacked = false;
            int PackedOrdersCount = 0;

            using (SqlDataAdapter pDA = new SqlDataAdapter(
                " SELECT PackageID, Packages.MainOrderID, MainOrders.MegaOrderID, PackageStatusID FROM Packages" +
                " INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON (MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrderID = 0)",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable pDT = new DataTable())
                {
                    pDA.Fill(pDT);

                    using (SqlDataAdapter mDA = new SqlDataAdapter(
                        " SELECT MegaOrderID, OrderNumber FROM MegaOrders WHERE ClientID = " + ClientID + " ORDER BY OrderNumber",
                        ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        using (DataTable mDT = new DataTable())
                        {
                            mDA.Fill(mDT);

                            for (int i = 0; i < mDT.Rows.Count; i++)
                            {
                                OrderPacked = true;

                                DataRow[] pRows = pDT.Select("MegaOrderID = " + Convert.ToInt32(mDT.Rows[i]["MegaOrderID"]));

                                if (pRows.Count() < 1)
                                    continue;

                                foreach (DataRow item in pRows)
                                {
                                    if (Convert.ToInt32(item["PackageStatusID"]) == 1 || Convert.ToInt32(item["PackageStatusID"]) == 2)
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

        public void MoveToMegaOrder(int MegaOrderID)
        {
            MegaOrdersBindingSource.Position = MegaOrdersBindingSource.Find("MegaOrderID", MegaOrderID);
        }

        public void MoveToPackage(int PackageID)
        {
            PackagesBindingSource.Position = PackagesBindingSource.Find("PackageID", PackageID);
        }

        public void SetDispatchDate(int FactoryID, int MegaOrderID, DateTime DispatchDate)
        {
            string SqlCommandText = "SELECT MegaOrderID, ProfilDispatchDate FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;

            if (FactoryID == 2)
                SqlCommandText = "SELECT MegaOrderID, TPSDispatchDate FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;

            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
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
                                DT.Rows[0]["ProfilDispatchDate"] = DispatchDate;
                            }
                            if (FactoryID == 2)
                            {
                                DT.Rows[0]["TPSDispatchDate"] = DispatchDate;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void GetPermissions(int UserID, string FormName)
        {
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

        public void EditDecorCount(int PackageID, int DecorOrderID, int Count)
        {
            int AddCount = 0;
            string SqlCommandText = "SELECT * FROM PackageDetails WHERE PackageID = " + PackageID + " AND OrderID=" + DecorOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            AddCount = Convert.ToInt32(DT.Rows[0]["Count"]) - Count;
                            DT.Rows[0]["Count"] = Count;
                            DA.Update(DT);
                        }
                    }
                }
            }
            SqlCommandText = "SELECT DecorOrderID, Count FROM DecorOrders WHERE DecorOrderID=" + DecorOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            //int OldCount = Convert.ToInt32(DT.Rows[0]["Count"]);
                            //if (OldCount > Count)
                            DT.Rows[0]["Count"] = Convert.ToInt32(DT.Rows[0]["Count"]) - AddCount;
                            //DT.Rows[0]["Count"] = Count;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void EditDecorLength(int DecorOrderID, int Length)
        {
            string SqlCommandText = "SELECT DecorOrderID, Length FROM DecorOrders WHERE DecorOrderID=" + DecorOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0]["Length"] = Length;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void MoveMainOrdersToAnotherDispatch(int[] MainOrders, int DispatchID)
        {
            DataTable TempDT = new DataTable();
            int NewDispatchID = 0;
            string SelectCommand = @"SELECT * FROM Dispatch WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT TOP 1 * FROM Dispatch ORDER BY DispatchID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (TempDT.Rows.Count > 0)
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow.ItemArray = TempDT.Rows[0].ItemArray;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                            DT.Clear();
                            DA.Fill(DT);
                            if (DT.Rows.Count > 0)
                                NewDispatchID = Convert.ToInt32(DT.Rows[0]["DispatchID"]);
                        }
                    }
                }
            }
            SelectCommand = "SELECT PackageID, MainOrderID, DispatchID FROM Packages " +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrders) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (DT.Rows[i]["DispatchID"] != DBNull.Value)
                                DT.Rows[i]["DispatchID"] = NewDispatchID;
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

    }




    public class MarketingDispatch
    {
        //DataTable PaidDispatchesDT;
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
        DataTable ClientsDispatchesDT;
        DataTable ClientsIncomesDT;

        BindingSource DispatchBS;
        BindingSource DispatchDatesBS;
        BindingSource DispatchContentBS;

        public MarketingDispatch()
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
            string SelectCommand = @"SELECT TOP 0 Dispatch.*, Clients.ClientName, NewClients.ClientName AS NewClientName, 
                ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
                LEFT JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON Dispatch.ClientID = Clients.ClientID
                LEFT JOIN infiniu2_marketingreference.dbo.Clients AS NewClients ON Dispatch.NewClientID = NewClients.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispatchDT);
                DispatchDT.Columns.Add(new DataColumn("PaidStatus", Type.GetType("System.Int32")));
            }
            SelectCommand = "SELECT TOP 0 PrepareDispatchDateTime FROM Dispatch ORDER BY PrepareDispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispatchDatesDT);
            }
            //SelectCommand = "SELECT TOP 0 MegaOrderID, MegaOrders.ClientID, OrderNumber FROM MegaOrders" +
            //    " INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            //{
            //    DA.Fill(DispatchContentDT);
            //}

            SelectCommand = "SELECT TOP 0 MainOrders.MegaOrderID, MegaOrders.ClientID, MegaOrders.OrderNumber, MainOrders.FactoryID, MainOrders.MainOrderID, MainOrders.ProfilPackAllocStatusID, MainOrders.TPSPackAllocStatusID FROM MainOrders" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID=MegaOrders.MegaOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
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

            ClientsDispatchesDT = new DataTable();
            ClientsDispatchesDT.Columns.Add(new DataColumn("MutualSettlementID", Type.GetType("System.Int32")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("FactoryID", Type.GetType("System.Int32")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("DispatchDateTime", Type.GetType("System.String")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("WayBill", Type.GetType("System.String")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("DispatchSum", Type.GetType("System.Decimal")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("TotalInvoiceSum", Type.GetType("System.Decimal")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("IncomeSum", Type.GetType("System.Decimal")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("DebtSum", Type.GetType("System.Decimal")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("CurrencyTypeID", Type.GetType("System.Int32")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("Deadline", Type.GetType("System.String")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("Overdue", Type.GetType("System.Boolean")));
            ClientsDispatchesDT.Columns.Add(new DataColumn("TotalDays", Type.GetType("System.Int32")));
        }

        public void GetMainOrdersSquareAndWeight(DateTime PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT Packages.DispatchID, FrontsOrders.MainOrderID, (FrontsOrders.Square*PackageDetails.Count/FrontsOrders.Count) AS Square
                    FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllMainOrdersSquareDT.Clear();
                DA.Fill(AllMainOrdersSquareDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, FrontsOrders.MainOrderID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllMainOrdersFrontsWeightDT.Clear();
                DA.Fill(AllMainOrdersFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, DecorOrders.MainOrderID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllMainOrdersDecorWeightDT.Clear();
                DA.Fill(AllMainOrdersDecorWeightDT);
            }

            SelectCommand = @"SELECT Packages.DispatchID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllDispatchFrontsWeightDT.Clear();
                DA.Fill(AllDispatchFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
                    FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
                    AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllDispatchDecorWeightDT.Clear();
                DA.Fill(AllDispatchDecorWeightDT);
            }
            SelectCommand = @"SELECT PackageID, PackageStatusID, DispatchID
                    FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DispatchInfoDT.Clear();
                DA.Fill(DispatchInfoDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, MAX(DispatchDateTime) AS DispatchDateTime
                    FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
                    '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "') GROUP By DispatchID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                RealDispTimeDT.Clear();
                DA.Fill(RealDispTimeDT);
            }
        }

        private decimal GetSquare(int DispatchID, int MainOrderID)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            decimal Square = 0;
            DataRow[] rows = AllMainOrdersSquareDT.Select("DispatchID=" + DispatchID + " AND MainOrderID=" + MainOrderID);

            //if (rows.Count() > 0 && rows[0]["Square"] != DBNull.Value)
            //{
            //    Square += Convert.ToDecimal(rows[0]["Square"]);
            //}
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

            //if (rows.Count() > 0 && rows[0]["Square"] != DBNull.Value)
            //{
            //    if (Convert.ToDecimal(rows[0]["Square"]) > 0)
            //        PackWeight = Convert.ToDecimal(rows[0]["Square"]) * Convert.ToDecimal(0.7);
            //}
            //if (rows.Count() > 0 && rows[0]["Weight"] != DBNull.Value)
            //{
            //    Weight = PackWeight + Convert.ToDecimal(rows[0]["Weight"]);
            //}
            rows = AllMainOrdersDecorWeightDT.Select("DispatchID=" + DispatchID + " AND MainOrderID=" + MainOrderID);
            //if (rows.Count() > 0 && rows[0]["Weight"] != DBNull.Value)
            //{
            //    Weight += Convert.ToDecimal(rows[0]["Weight"]);
            //}
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
            //if (rows.Count() > 0 && rows[0]["Square"] != DBNull.Value)
            //{
            //    if (Convert.ToDecimal(rows[0]["Square"]) > 0)
            //        PackWeight = Convert.ToDecimal(rows[0]["Square"]) * Convert.ToDecimal(0.7);
            //}
            //if (rows.Count() > 0 && rows[0]["Weight"] != DBNull.Value)
            //{
            //    Weight = PackWeight + Convert.ToDecimal(rows[0]["Weight"]);
            //}
            rows = AllDispatchDecorWeightDT.Select("DispatchID=" + DispatchID);
            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]);
            }
            //if (rows.Count() > 0 && rows[0]["Weight"] != DBNull.Value)
            //{
            //    Weight += Convert.ToDecimal(rows[0]["Weight"]);
            //}
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        public void GetMegaBatchNumbers(DateTime PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT Batch.MegaBatchID, BatchDetails.MainOrderID FROM BatchDetails 
                INNER JOIN Batch ON BatchDetails.BatchID = Batch.BatchID 
                WHERE BatchDetails.MainOrderID IN (SELECT MainOrderID FROM Packages WHERE Packages.DispatchID IN 
                (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = '" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "'))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllMegaBatchNumbersDT.Clear();
                DA.Fill(AllMegaBatchNumbersDT);
            }
        }

        public decimal GetDispatchSum(int MutualSettlementID)
        {
            decimal DispatchSum = 0;
            string SelectCommand = @"SELECT * FROM MutualSettlements WHERE MutualSettlementID=" + MutualSettlementID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DispatchSum = Convert.ToDecimal(DT.Rows[0]["TotalInvoiceSum"]);
                        return DispatchSum;
                    }
                }
            }

            return DispatchSum;
        }

        public void DelayOfPayment(List<int> Clients)
        {
            if (ClientsIncomesDT == null)
                ClientsIncomesDT = new DataTable();
            ClientsIncomesDT.Clear();
            string SelectCommand = @"SELECT DiscountPaymentConditionID, ClientsIncomes.CurrencyTypeID, ClientsIncomes.MutualSettlementID, IncomeSum, MutualSettlements.TotalInvoiceSum FROM ClientsIncomes
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID
                WHERE MutualSettlements.ClientID IN (" + string.Join(",", Clients) + ") ORDER BY IncomeDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(ClientsIncomesDT);
            }


            ClientsDispatchesDT.Clear();
            DataTable DispDT = new DataTable();

            SelectCommand = @"SELECT DiscountPaymentConditionID, ClientsDispatches.*, MutualSettlements.TotalInvoiceSum, MutualSettlements.FactoryID, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID IN (" + string.Join(",", Clients) + ") ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(DispDT.Rows[i]["ClientID"]);
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                decimal AllDispatchSum = 0;
                decimal AllIncomeSum = 0;
                DataRow[] dRows = DispDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (dRows.Count() > 0)
                {
                    foreach (DataRow item in dRows)
                    {
                        AllDispatchSum += Convert.ToDecimal(item["DispatchSum"]);
                    }
                }
                DataRow[] cRows = ClientsIncomesDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (cRows.Any())
                {
                    AllIncomeSum += cRows.Sum(item => Convert.ToDecimal(item["IncomeSum"]));
                }
                DataRow[] Rows = ClientsIncomesDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (Rows.Count() > 0)
                {
                    if (AllIncomeSum - AllDispatchSum < 0)
                    {
                        decimal d = DispatchSum;
                        foreach (DataRow dr in ClientsIncomesDT.Select("MutualSettlementID = " + MutualSettlementID))
                        {
                            decimal c = Convert.ToDecimal(dr["IncomeSum"]);
                            if (d <= 0)
                                break;
                            if (c == 0)
                                continue;
                            if (Convert.ToDecimal(dr["IncomeSum"]) >= d)
                            {
                                dr["IncomeSum"] = Convert.ToDecimal(dr["IncomeSum"]) - d;
                                d = 0;
                                break;
                            }
                            d -= Convert.ToDecimal(dr["IncomeSum"]);
                            dr["IncomeSum"] = 0;
                        }

                        if (d <= 0)
                            continue;
                        DataRow NewRow = ClientsDispatchesDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["MutualSettlementID"] = DispDT.Rows[i]["MutualSettlementID"];
                        NewRow["FactoryID"] = DispDT.Rows[i]["FactoryID"];
                        NewRow["CurrencyTypeID"] = DispDT.Rows[i]["CurrencyTypeID"];
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["ClientID"] = DispDT.Rows[i]["ClientID"];
                        NewRow["TotalInvoiceSum"] = DispDT.Rows[i]["TotalInvoiceSum"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["DebtSum"] = d;
                        //if ((IncomeSum - DispatchSum) * -1 > DispatchSum)
                        //    NewRow["DebtSum"] = DispatchSum;
                        //else
                        //    NewRow["DebtSum"] = (IncomeSum - DispatchSum) * -1;
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ClientsDispatchesDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ClientsDispatchesDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["MutualSettlementID"] = DispDT.Rows[i]["MutualSettlementID"];
                    NewRow["FactoryID"] = DispDT.Rows[i]["FactoryID"];
                    NewRow["CurrencyTypeID"] = DispDT.Rows[i]["CurrencyTypeID"];
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["ClientID"] = DispDT.Rows[i]["ClientID"];
                    NewRow["TotalInvoiceSum"] = DispDT.Rows[i]["TotalInvoiceSum"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["DebtSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ClientsDispatchesDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ClientsDispatchesDT.Copy(), string.Empty, "ClientName, MutualSettlementID", DataViewRowState.CurrentRows))
            {
                ClientsDispatchesDT.Clear();
                ClientsDispatchesDT = DV.ToTable();
            }

            DispDT.Dispose();
        }

        public int GetPaidStatus(int ClientID, int MutualSettlementID)
        {
            int PaidStatus = -1;
            bool NotPaid = true;
            bool Overdue = false;
            decimal TotalIncomeSum = 0;
            if (MutualSettlementID > 0)
            {
                DataRow[] Rows1 = ClientsIncomesDT.Select("DiscountPaymentConditionID IN (2,3) AND MutualSettlementID = " + MutualSettlementID);
                if (Rows1.Count() > 0)
                {
                    TotalIncomeSum += Rows1.Sum(item => Convert.ToDecimal(item["IncomeSum"]));
                    decimal TotalInvoiceSum = Convert.ToDecimal(Rows1[0]["TotalInvoiceSum"]);
                    TotalInvoiceSum = Decimal.Round(TotalInvoiceSum, 2, MidpointRounding.AwayFromZero);
                    if (TotalInvoiceSum > Convert.ToDecimal(TotalIncomeSum))
                        NotPaid = true;
                    else
                        NotPaid = false;
                }
            }
            else
                NotPaid = false;
            DataRow[] Rows = ClientsDispatchesDT.Select("ClientID = " + ClientID);
            if (Rows.Count() > 0)
            {
                var OverdueCount = Rows.Count(row => row.Field<bool>("Overdue") == true);
                if (OverdueCount > 0)
                    Overdue = true;
            }
            if (Overdue && NotPaid)
                PaidStatus = 2;
            if (!Overdue && NotPaid)
                PaidStatus = 1;
            if (Overdue && !NotPaid)
                PaidStatus = 0;
            return PaidStatus;
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
                int FactoryID = Convert.ToInt32(DispatchContentDT.Rows[i]["FactoryID"]);
                int MainOrderID = Convert.ToInt32(DispatchContentDT.Rows[i]["MainOrderID"]);
                int ProfilPackAllocStatusID = Convert.ToInt32(DispatchContentDT.Rows[i]["ProfilPackAllocStatusID"]);
                int TPSPackAllocStatusID = Convert.ToInt32(DispatchContentDT.Rows[i]["TPSPackAllocStatusID"]);

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
                    WHERE MainOrderID=" + MainOrderID + " AND DispatchID=" + DispatchID, ConnectionStrings.MarketingOrdersConnectionString))
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

                if ((FactoryID != 2 && ProfilPackAllocStatusID != 2) || (FactoryID != 1 && TPSPackAllocStatusID != 2))
                {
                    PackedCount = 0;
                    StoreCount = 0;
                    DispCount = 0;
                    ExpCount = 0;
                }

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
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
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

        public void GetRealDispDateTime(int DispatchID, ref object RealDispDateTime, ref object DispUserID)
        {
            string SelectCommand = @"SELECT MAX(DispatchDateTime) AS DispatchDateTime, DispUserID
                FROM Packages WHERE DispatchID = " + DispatchID + " GROUP BY DispUserID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
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
            string SelectCommand = @"SELECT MAX(DispatchDateTime) AS DispatchDateTime
                FROM Packages WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["DispatchDateTime"] != DBNull.Value)
                            RealDispDateTime = DT.Rows[0]["DispatchDateTime"];
                    }
                }
            }
            return RealDispDateTime;
        }

        private void GetDispPackagesInfo(int DispatchID, ref int DispPackagesCount, ref int PackagesCount)
        {
            string SelectCommand = @"SELECT PackageID, PackageStatusID FROM Packages WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DispPackagesCount = DT.Select("PackageStatusID = 3").Count();
                        PackagesCount = DT.Rows.Count;
                    }
                }
            }
        }

        public bool IsDispatchCanExp(int DispatchID)
        {
            string SelectCommand = @"SELECT PackageID, PackageStatusID FROM Packages WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int StorePackagesCount = DT.Select("PackageStatusID = 2").Count();
                        int AllPackagesCoutnt = DT.Rows.Count;
                        return AllPackagesCoutnt == StorePackagesCount;
                    }
                    else
                        return false;
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

        public void RemoveOrder()
        {
            if (DispatchBS.Current != null)
            {
                DispatchBS.RemoveCurrent();
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
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
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

        public void SetDispatchDate(int DispatchID, DateTime DispatchDate)
        {
            string SqlCommandText = @"SELECT MegaOrderID, ProfilDispatchDate, TPSDispatchDate, FactoryID FROM MegaOrders
                WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE (DispatchID = " + DispatchID + ")))";

            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                if (FactoryID == 1)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 2)
                                {
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 0)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
            SqlCommandText = @"SELECT MegaOrderID, ProfilDispatchDate, TPSDispatchDate, FactoryID FROM NewMegaOrders
                WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE (DispatchID = " + DispatchID + ")))";

            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                if (FactoryID == 1)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 2)
                                {
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 0)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        public void SaveConfirmExpInfo(int DispatchID, bool Confirm)
        {
            string SelectCommand = "SELECT DispatchID, ConfirmExpUserID, ConfirmExpDateTime FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (Confirm)
                            {
                                DT.Rows[0]["ConfirmExpDateTime"] = Security.GetCurrentDate();
                                DT.Rows[0]["ConfirmExpUserID"] = Security.CurrentUserID;
                            }
                            else
                            {
                                DT.Rows[0]["ConfirmExpDateTime"] = DBNull.Value;
                                DT.Rows[0]["ConfirmExpUserID"] = DBNull.Value;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void IncludeInMutualSettlement(int DispatchID, int ProfilMutualSettlementID, int TPSMutualSettlementID)
        {
            string SelectCommand = "SELECT DispatchID, InMutualSettlement, ProfilMutualSettlementID, TPSMutualSettlementID FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["InMutualSettlement"] = true;
                            DT.Rows[0]["ProfilMutualSettlementID"] = ProfilMutualSettlementID;
                            DT.Rows[0]["TPSMutualSettlementID"] = TPSMutualSettlementID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void IncludeInMutualSettlement(int[] Dispatches, int ProfilMutualSettlementID, int TPSMutualSettlementID)
        {
            string SelectCommand = @"SELECT DispatchID, InMutualSettlement, ProfilMutualSettlementID, TPSMutualSettlementID FROM Dispatch 
                WHERE DispatchID IN (" + string.Join(",", Dispatches) + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["InMutualSettlement"] = true;
                                DT.Rows[i]["ProfilMutualSettlementID"] = ProfilMutualSettlementID;
                                DT.Rows[i]["TPSMutualSettlementID"] = TPSMutualSettlementID;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SaveConfirmDispInfo(int DispatchID, bool Confirm)
        {
            string SelectCommand = "SELECT DispatchID, ConfirmDispUserID, ConfirmExpDateTime, ConfirmDispDateTime FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (Confirm)
                            {
                                if (DT.Rows[0]["ConfirmExpDateTime"] != DBNull.Value)
                                {
                                    if (DT.Rows[0]["ConfirmExpDateTime"] != DBNull.Value)
                                    {
                                        DT.Rows[0]["ConfirmDispDateTime"] = Security.GetCurrentDate();
                                        DT.Rows[0]["ConfirmDispUserID"] = Security.CurrentUserID;
                                    }
                                }
                            }
                            else
                            {
                                DT.Rows[0]["ConfirmDispDateTime"] = DBNull.Value;
                                DT.Rows[0]["ConfirmDispUserID"] = DBNull.Value;
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
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispatchDatesDT);
            }
            FillWeekNumber();
        }

        public void FilterDispatchByDate(DateTime Date)
        {
            string SelectCommand = @"SELECT Dispatch.*, Clients.ClientName, NewClients.ClientName AS NewClientName, 
                ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
                LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
                LEFT JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON Dispatch.ClientID = Clients.ClientID
                LEFT JOIN infiniu2_marketingreference.dbo.Clients AS NewClients ON Dispatch.NewClientID = NewClients.ClientID
                WHERE CAST(PrepareDispatchDateTime AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispatchDT);
                for (int i = 0; i < DispatchDT.Rows.Count; i++)
                    DispatchDT.Rows[i]["Weight"] = GetWeight(Convert.ToInt32(DispatchDT.Rows[i]["DispatchID"]));
            }
            FillDispPackagesInfo();
        }

        public void FilterDispatchContent(int DispatchID)
        {
            string SelectCommand = "SELECT MainOrders.MegaOrderID, MegaOrders.ClientID, MegaOrders.OrderNumber, MainOrders.FactoryID, MainOrders.MainOrderID, MainOrders.ProfilPackAllocStatusID, MainOrders.TPSPackAllocStatusID FROM MainOrders" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID=MegaOrders.MegaOrderID" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE DispatchID = " + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispatchContentDT);
            }
        }

        public int[] GetMainOrdersInDispatch(int DispatchID)
        {
            ArrayList MainOrders = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT MainOrderID FROM Packages" +
                " WHERE DispatchID =" + DispatchID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);


                    for (int i = 0; i < DT.Rows.Count; i++)
                        MainOrders.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                }
            }

            return MainOrders.OfType<Int32>().ToArray();
        }

        public int[] GetMainOrders(int[] Dispatches)
        {
            ArrayList MainOrders = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT MainOrderID FROM Packages" +
                " WHERE DispatchID IN (" + string.Join(",", Dispatches) + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);


                    for (int i = 0; i < DT.Rows.Count; i++)
                        MainOrders.Add(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                }
            }

            return MainOrders.OfType<Int32>().ToArray();
        }

        //        public int GetCurrencyType(int[] Dispatches)
        //        {
        //            int CurrencyTypeID = 1;

        //            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CurrencyTypeID FROM MegaOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
        //                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages" +
        //                " WHERE DispatchID IN (" + string.Join(",", Dispatches) + ")))", ConnectionStrings.MarketingOrdersConnectionString))
        //            {
        //                using (DataTable DT = new DataTable())
        //                {
        //                    if (DA.Fill(DT) > 0 )
        //                        CurrencyTypeID = Convert.ToInt32(DT.Rows[0]["CurrencyTypeID"]);
        //                }
        //            }

        //            return CurrencyTypeID;
        //        }

        public DataTable GetMegaOrdersInDispatch(int DispatchID)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FactoryID, MegaOrderID, OrderNumber, DiscountPaymentConditionID,
                ProfilDiscountOrderSum, TPSDiscountOrderSum, ProfilDiscountDirector, TPSDiscountDirector,
                Rate, ConfirmDateTime, CurrencyTypeID FROM MegaOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE DispatchID=" + DispatchID + "))", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(dt);
            }
            return dt;
        }

        public DataTable GetMegaOrders(int[] Dispatches)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MegaOrderID, OrderNumber, DiscountPaymentConditionID,
                ProfilDiscountOrderSum, TPSDiscountOrderSum, ProfilDiscountDirector, TPSDiscountDirector,
                Rate, ConfirmDateTime, CurrencyTypeID FROM MegaOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE DispatchID IN (" + string.Join(",", Dispatches) + ")))", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(dt);
            }
            return dt;
        }

        private decimal GetDiscountPaymentCondition(int DiscountPaymentConditionID)
        {
            decimal Discount = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DiscountPaymentConditions WHERE DiscountPaymentConditionID=" + DiscountPaymentConditionID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        Discount = Convert.ToInt32(DT.Rows[0]["Discount"]);
                    }
                }
            }
            return Discount;
        }

        public decimal GetDiscountFactoring(int DiscountFactoringID)
        {
            decimal Discount = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DiscountFactoring WHERE DiscountFactoringID=" + DiscountFactoringID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        Discount = Convert.ToInt32(DT.Rows[0]["Discount"]);
                    }
                }
            }
            return (6 - Discount);
        }

        public MegaOrderInfo GetMegaOrders(int MegaOrderID)
        {
            DataTable dt = new DataTable();
            MegaOrderInfo m = new MegaOrderInfo();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MegaOrderID, OrderNumber, DiscountPaymentConditionID, DiscountFactoringID,
                ProfilDiscountOrderSum, TPSDiscountOrderSum, ProfilDiscountDirector, TPSDiscountDirector, ProfilTotalDiscount, TPSTotalDiscount,
                Rate, ConfirmDateTime, CurrencyTypeID FROM MegaOrders WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                if (DA.Fill(dt) > 0)
                {
                    m.DiscountPaymentConditionID = Convert.ToInt32(dt.Rows[0]["DiscountPaymentConditionID"]);
                    m.DiscountFactoringID = Convert.ToInt32(dt.Rows[0]["DiscountFactoringID"]);
                    m.ProfilDiscountOrderSum = Convert.ToDecimal(dt.Rows[0]["ProfilDiscountOrderSum"]);
                    m.TPSDiscountOrderSum = Convert.ToDecimal(dt.Rows[0]["TPSDiscountOrderSum"]);
                    m.ProfilDiscountDirector = Convert.ToDecimal(dt.Rows[0]["ProfilDiscountDirector"]);
                    m.TPSDiscountDirector = Convert.ToDecimal(dt.Rows[0]["TPSDiscountDirector"]);
                    m.ProfilTotalDiscount = Convert.ToDecimal(dt.Rows[0]["ProfilTotalDiscount"]);
                    m.TPSTotalDiscount = Convert.ToDecimal(dt.Rows[0]["TPSTotalDiscount"]);
                    m.OriginalRate = Convert.ToDecimal(dt.Rows[0]["Rate"]);
                    m.PaymentRate = Convert.ToDecimal(dt.Rows[0]["Rate"]);
                    m.ConfirmDateTime = dt.Rows[0]["ConfirmDateTime"];
                    int DiscountPaymentConditionID = Convert.ToInt32(dt.Rows[0]["DiscountPaymentConditionID"]);
                    int DiscountFactoringID = Convert.ToInt32(dt.Rows[0]["DiscountFactoringID"]);

                    m.DiscountPaymentCondition = GetDiscountPaymentCondition(DiscountPaymentConditionID);
                    if (DiscountPaymentConditionID == 4)
                    {
                        m.DiscountPaymentCondition = GetDiscountFactoring(DiscountFactoringID);
                    }
                }
            }
            return m;
        }

        public bool IsPackageInDispatch(int PackageID, ref int DispatchID)
        {
            string SelectCommand = @"SELECT * FROM Packages WHERE PackageID = " + PackageID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
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

        public object GetPrepareDispatchDateTime(int DispatchID)
        {
            object PrepareDispatchDateTime = DBNull.Value;
            string SelectCommand = @"SELECT PrepareDispatchDateTime
                FROM Dispatch WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
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

        public void MoveToDispatchDate(DateTime DispatchDate)
        {
            DispatchDatesBS.Position = DispatchDatesBS.Find("PrepareDispatchDateTime", DispatchDate);
        }

        public void MoveToDispatch(int DispatchID)
        {
            DispatchBS.Position = DispatchBS.Find("DispatchID", DispatchID);
        }

        public bool IsDispatchBindToPermit(int DispatchID)
        {
            string SelectCommand = @"SELECT * FROM PermitDetails WHERE DispatchID = " + DispatchID;
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

        public void ReaddressDispatch(int DispatchID, int NewClientID)
        {
            string SelectCommand = @"SELECT * FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["NewClientID"] = NewClientID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public bool HasOrders(int[] Dispatches, bool IsSample)
        {
            bool hasFronts = false;
            bool hasDecor = false;
            string SelectCommand = @"SELECT PackageDetails.* FROM PackageDetails 
                    INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    WHERE FrontsOrders.IsSample=1 AND PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + "))";
            if (!IsSample)
                SelectCommand = @"SELECT PackageDetails.* FROM PackageDetails 
                    INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    WHERE FrontsOrders.IsSample=0 AND PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    hasFronts = DA.Fill(DT) > 0 ? true : false;
                }
            }

            SelectCommand = SelectCommand = @"SELECT PackageDetails.* FROM PackageDetails 
                    INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    WHERE DecorOrders.IsSample=1 AND PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + "))";
            if (!IsSample)
                SelectCommand = SelectCommand = @"SELECT PackageDetails.* FROM PackageDetails 
                    INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
                    WHERE DecorOrders.IsSample=0 AND PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    hasDecor = DA.Fill(DT) > 0 ? true : false;
                }
            }
            if (!hasFronts && !hasDecor)
                return false;
            return true;
        }
    }

    public struct MegaOrderInfo
    {
        public decimal DiscountPaymentCondition;
        public int DiscountPaymentConditionID;
        public int DiscountFactoringID;
        public decimal ProfilDiscountOrderSum;
        public decimal TPSDiscountOrderSum;
        public decimal ProfilDiscountDirector;
        public decimal TPSDiscountDirector;
        public decimal ProfilTotalDiscount;
        public decimal TPSTotalDiscount;
        public decimal OriginalRate;
        public decimal PaymentRate;
        public object ConfirmDateTime;
    }




    public class NewMarketingDispatch
    {
        int ClientID = 0;
        int DispatchID = 0;

        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;

        DataTable PackMainOrdersDT = null;
        DataTable PackMegaOrdersDT = null;
        DataTable StoreMainOrdersDT = null;
        DataTable StoreMegaOrdersDT = null;
        DataTable ExpMainOrdersDT = null;
        DataTable ExpMegaOrdersDT = null;
        DataTable DispMainOrdersDT = null;
        DataTable DispMegaOrdersDT = null;

        DataTable MegaOrdersDT;
        DataTable MainOrdersDT;
        DataTable PackagesDT;
        DataTable FrontsOrdersDT;
        DataTable DecorOrdersDT;

        BindingSource MegaOrdersBS;
        BindingSource MainOrdersBS;
        BindingSource PackagesBS;
        BindingSource FrontsOrdersBS;
        BindingSource DecorOrdersBS;

        public NewMarketingDispatch()
        {

        }

        public int CurrentClient
        {
            set { ClientID = value; }
        }

        public int CurrentDispatch
        {
            get { return DispatchID; }
            set { DispatchID = value; }
        }

        public void Initialize(int DispatchID, bool CanEditDispatch)
        {
            Create();

            UpdateMegaOrders(DispatchID, CanEditDispatch, false, false, true, true, true, !CanEditDispatch);
            UpdateMainOrders(DispatchID, CanEditDispatch, false, false, true, true, true, !CanEditDispatch);
            UpdatePackages(DispatchID, CanEditDispatch, false, false, true, true, true, !CanEditDispatch);
            Fill();
            SetDefaultValueToCheckBoxColumn();
            SetPackageDispatchStatus();
            SetMainOrderDispatchStatus();
            SetMegaOrderDispatchStatus();
            Binding();
        }

        public void Initialize(int MegaOrderID)
        {
            Create();
            UpdateMegaOrders(MegaOrderID);
            UpdateMainOrders(MegaOrderID);
            UpdatePackages(MegaOrderID);
            Fill();
            SetDefaultValueToCheckBoxColumn();
            SetPackageDispatchStatus();
            SetMainOrderDispatchStatus();
            SetMegaOrderDispatchStatus();
            Binding();
        }

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();

            PackMainOrdersDT = new DataTable();
            PackMegaOrdersDT = new DataTable();
            StoreMainOrdersDT = new DataTable();
            StoreMegaOrdersDT = new DataTable();
            ExpMainOrdersDT = new DataTable();
            ExpMegaOrdersDT = new DataTable();
            DispMainOrdersDT = new DataTable();
            DispMegaOrdersDT = new DataTable();

            MegaOrdersDT = new DataTable();
            MainOrdersDT = new DataTable();
            PackagesDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            DecorOrdersDT = new DataTable();

            MegaOrdersBS = new BindingSource();
            MainOrdersBS = new BindingSource();
            PackagesBS = new BindingSource();
            FrontsOrdersBS = new BindingSource();
            DecorOrdersBS = new BindingSource();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND ClientID = " + ClientID +
                " WHERE (Packages.PackageStatusID <> 0)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(PackMegaOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID <> 0" +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + "))" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(PackMainOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND ClientID = " + ClientID +
                " WHERE (Packages.PackageStatusID = 2)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMegaOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =2 " +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + "))" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(StoreMainOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND ClientID = " + ClientID +
                " WHERE (Packages.PackageStatusID = 4)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(ExpMegaOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID =4 " +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + "))" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(ExpMainOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(Packages.PackageID) AS Count, MegaOrders.MegaOrderID, Packages.FactoryID" +
                " FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND ClientID = " + ClientID +
                " WHERE (Packages.PackageStatusID = 3)" +
                " GROUP BY MegaOrders.MegaOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DispMegaOrdersDT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID = 3" +
                " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + "))" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DispMainOrdersDT);
                }
            }

            MegaOrdersDT.Columns.Add(new DataColumn("CheckBoxColumn", Type.GetType("System.Boolean")));

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

            PackagesDT.Columns.Add(new DataColumn("CheckBoxColumn", Type.GetType("System.Boolean")));

            string SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig)) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig )
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT DISTINCT TechStoreID AS TechnoProfileID, TechStoreName AS TechnoProfileName FROM TechStore 
                WHERE TechStoreID IN (SELECT TechnoProfileID FROM FrontsConfig )
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL",
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

            SelectCommand = @"SELECT TOP 0  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, Square, Notes FROM FrontsOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDT);
            }
            SelectCommand = @"SELECT TOP 0 DecorOrderID, ProductID, DecorID, ColorID, PatinaID, InsetTypeID, InsetColorID, Height, Length, Width, Count, Notes
                FROM DecorOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDT);
            }
        }

        private void Binding()
        {
            MegaOrdersBS.DataSource = MegaOrdersDT;
            MainOrdersBS.DataSource = MainOrdersDT;
            PackagesBS.DataSource = PackagesDT;
            FrontsOrdersBS.DataSource = FrontsOrdersDT;
            DecorOrdersBS.DataSource = DecorOrdersDT;
        }

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
                    DataSource = new DataView(InsetTypesDataTable),
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
                    DataSource = new DataView(InsetColorsDataTable),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
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

                    DataSource = new DataView(ProductsDataTable),
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

        public string PatinaDisplayName(int PatinaID)
        {
            DataRow[] rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
        }

        public void FillMainPercentageColumn()
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
                int FactoryID = Convert.ToInt32(MainOrdersDT.Rows[i]["FactoryID"]);
                int ProfilPackAllocStatusID = Convert.ToInt32(MainOrdersDT.Rows[i]["ProfilPackAllocStatusID"]);
                int TPSPackAllocStatusID = Convert.ToInt32(MainOrdersDT.Rows[i]["TPSPackAllocStatusID"]);

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

                if (FactoryID != 2 && ProfilPackAllocStatusID != 2)
                {
                    MainOrderProfilPackCount = 0;
                    MainOrderProfilStoreCount = 0;
                    MainOrderProfilExpCount = 0;
                    MainOrderProfilDispCount = 0;
                }
                if (FactoryID != 1 && TPSPackAllocStatusID != 2)
                {
                    MainOrderTPSPackCount = 0;
                    MainOrderTPSStoreCount = 0;
                    MainOrderTPSExpCount = 0;
                    MainOrderTPSDispCount = 0;
                }

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

        public void FillMegaPercentageColumn()
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
                int FactoryID = Convert.ToInt32(MegaOrdersDT.Rows[i]["FactoryID"]);
                int ProfilPackAllocStatusID = Convert.ToInt32(MegaOrdersDT.Rows[i]["ProfilPackAllocStatusID"]);
                int TPSPackAllocStatusID = Convert.ToInt32(MegaOrdersDT.Rows[i]["TPSPackAllocStatusID"]);

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

                if (FactoryID != 2 && ProfilPackAllocStatusID != 2)
                {
                    MegaOrderProfilPackCount = 0;
                    MegaOrderProfilStoreCount = 0;
                    MegaOrderProfilExpCount = 0;
                    MegaOrderProfilDispCount = 0;
                }
                if (FactoryID != 1 && TPSPackAllocStatusID != 2)
                {
                    MegaOrderTPSPackCount = 0;
                    MegaOrderTPSStoreCount = 0;
                    MegaOrderTPSExpCount = 0;
                    MegaOrderTPSDispCount = 0;
                }

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

        private int GetMegaOrderDispCount(int MegaOrderID, int FactoryID)
        {
            int DispCount = 0;
            DataRow[] Rows = DispMegaOrdersDT.Select("MegaOrderID = " + MegaOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                DispCount = Convert.ToInt32(Rows[0]["Count"]);

            return DispCount;
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

        public bool IsEmptyDispath
        {
            get
            {
                return PackagesDT.Select("DispatchID IS NOT NULL").Count() == 0;
            }
        }

        public BindingSource MegaOrdersList
        {
            get { return MegaOrdersBS; }
        }

        public BindingSource MainOrdersList
        {
            get { return MainOrdersBS; }
        }

        public BindingSource PackagesList
        {
            get { return PackagesBS; }
        }

        public BindingSource FrontsOrdersList
        {
            get { return FrontsOrdersBS; }
        }

        public BindingSource DecorOrdersList
        {
            get { return DecorOrdersBS; }
        }

        public void ClearMegaOrders()
        {
            MegaOrdersDT.Clear();
        }

        public void ClearMainOrders()
        {
            MainOrdersDT.Clear();
        }

        public void ClearPackages()
        {
            PackagesDT.Clear();
        }

        public void ClearFrontsOrders()
        {
            FrontsOrdersDT.Clear();
        }

        public void ClearDecorOrders()
        {
            DecorOrdersDT.Clear();
        }

        public void ClearConfirmDispInfo()
        {
            string SelectCommand = "SELECT DispatchID, ConfirmDispDateTime, ConfirmDispUserID FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ConfirmDispDateTime"] = DBNull.Value;
                            DT.Rows[0]["ConfirmDispUserID"] = DBNull.Value;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ClearConfirmExpInfo()
        {
            string SelectCommand = "SELECT DispatchID, ConfirmExpDateTime, ConfirmExpUserID FROM Dispatch WHERE DispatchID=" + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ConfirmExpDateTime"] = DBNull.Value;
                            DT.Rows[0]["ConfirmExpUserID"] = DBNull.Value;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SetDispatchDate(DateTime DispatchDate)
        {
            int[] MegaOrderID = DistMegaOrders();
            string SqlCommandText = "SELECT MegaOrderID, ProfilDispatchDate, TPSDispatchDate, FactoryID FROM MegaOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrderID) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                if (FactoryID == 1)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 2)
                                {
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 0)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }

            SqlCommandText = "SELECT MegaOrderID, ProfilDispatchDate, TPSDispatchDate, FactoryID FROM NewMegaOrders" +
                " WHERE MegaOrderID IN (" + string.Join(",", MegaOrderID) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(SqlCommandText, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                if (FactoryID == 1)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 2)
                                {
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }
                                if (FactoryID == 0)
                                {
                                    DT.Rows[i]["ProfilDispatchDate"] = DispatchDate;
                                    DT.Rows[i]["TPSDispatchDate"] = DispatchDate;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        public void SavePackages()
        {
            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                if (Convert.ToBoolean(PackagesDT.Rows[i]["CheckBoxColumn"]))
                    PackagesDT.Rows[i]["DispatchID"] = DispatchID;
                else
                    PackagesDT.Rows[i]["DispatchID"] = DBNull.Value;
            }

            string SelectCommand = "SELECT TOP 0 PackageID, DispatchID FROM Packages";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(PackagesDT);
                }
            }
        }

        private int[] DistMegaOrders()
        {
            DataTable DT = new DataTable();
            DT.Columns.Add(new DataColumn("MegaOrderID", Type.GetType("System.Int32")));

            using (DataView DV = new DataView(PackagesDT))
            {
                DT = DV.ToTable(true, new string[] { "MegaOrderID" });
            }
            int[] MegaOrders = new int[DT.Rows.Count];
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                MegaOrders[i] = Convert.ToInt32(DT.Rows[i]["MegaOrderID"]);
            }
            return MegaOrders;
        }

        public void SetDefaultValueToCheckBoxColumn()
        {
            for (int i = 0; i < MegaOrdersDT.Rows.Count; i++)
            {
                MegaOrdersDT.Rows[i]["CheckBoxColumn"] = false;
            }
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                MainOrdersDT.Rows[i]["CheckBoxColumn"] = false;
            }
            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                PackagesDT.Rows[i]["CheckBoxColumn"] = false;
            }
        }

        public void UpdateMegaOrders(int DispatchID, bool CanEditDispatch,
            bool NotProduction,
            bool OnProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExpedition,
            bool Dispatched)
        {
            int FactoryID = 0;
            string OrdersProductionStatus = string.Empty;

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1))";
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }
            if (OnExpedition)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSExpeditionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSExpeditionStatusID=2)";
                }
            }
            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }

            }
            #endregion

            if (OrdersProductionStatus.Length > 0)
            {
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
            }

            string SelectCommand = @"SELECT MegaOrderID, OrderNumber, Weight, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount FROM MegaOrders
                WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND ClientID = " + ClientID + @" AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE (DispatchID IS NULL OR DispatchID = " + DispatchID + @")) " + OrdersProductionStatus + ") ORDER BY OrderNumber";
            if (!CanEditDispatch)
                SelectCommand = @"SELECT MegaOrderID, OrderNumber, Weight, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount FROM MegaOrders
                WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND ClientID = " + ClientID + @" AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE (DispatchID = " + DispatchID + @")) " + OrdersProductionStatus + ") ORDER BY OrderNumber";
            //            string SelectCommand = @"SELECT MegaOrderID, OrderNumber, Weight, ProfilPackCount, TPSPackCount FROM MegaOrders
            //				WHERE ClientID = " + ClientID + @" AND MegaOrderID IN (SELECT MegaOrderID FROM MainOrders)
            //				ORDER BY OrderNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MegaOrdersDT.Clear();
                DA.Fill(MegaOrdersDT);
            }
        }

        public void UpdateMainOrders(int DispatchID, bool CanEditDispatch,
            bool NotProduction,
            bool OnProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExpedition,
            bool Dispatched)
        {
            int FactoryID = 0;
            string OrdersProductionStatus = string.Empty;

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1))";
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }
            if (OnExpedition)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSExpeditionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSExpeditionStatusID=2)";
                }
            }
            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }

            }
            #endregion

            if (OrdersProductionStatus.Length > 0)
            {
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
            }

            string SelectCommand = @"SELECT MainOrderID, MegaOrderID, Weight, MainOrders.FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount, FactoryName, Notes FROM MainOrders
                INNER JOIN infiniu2_catalog.dbo.Factory ON MainOrders.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE (DispatchID IS NULL OR DispatchID = " + DispatchID + @")) AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND ClientID = " + ClientID + @")" +
                OrdersProductionStatus;
            if (!CanEditDispatch)
                SelectCommand = @"SELECT MainOrderID, MegaOrderID, Weight, MainOrders.FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount, FactoryName, Notes FROM MainOrders
                INNER JOIN infiniu2_catalog.dbo.Factory ON MainOrders.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
                WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE (DispatchID = " + DispatchID + @")) AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE NOT (AgreementStatusID=0 AND CreatedByClient=1) AND ClientID = " + ClientID + @")" +
                    OrdersProductionStatus;
            //            string SelectCommand = @"SELECT MainOrderID, MegaOrderID, Weight, ProfilPackCount, TPSPackCount, FactoryName, Notes FROM MainOrders
            //				INNER JOIN infiniu2_catalog.dbo.Factory ON MainOrders.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
            //				WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + @")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MainOrdersDT.Clear();
                DA.Fill(MainOrdersDT);
            }
        }

        public void UpdatePackages(int DispatchID, bool CanEditDispatch,
            bool NotProduction,
            bool OnProduction,
            bool InProduction,
            bool OnStorage,
            bool OnExpedition,
            bool Dispatched)
        {
            int FactoryID = 0;
            string OrdersProductionStatus = string.Empty;

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus = "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)" +
                       " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1))";
                if (FactoryID == 1)
                    OrdersProductionStatus = "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilExpeditionStatusID = 1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus = "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID = 1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    if (FactoryID == 0)
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
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }
            if (OnExpedition)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSExpeditionStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2 OR TPSExpeditionStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilExpeditionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSExpeditionStatusID=2)";
                }
            }
            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    if (FactoryID == 0)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";
                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }

            }
            #endregion

            if (OrdersProductionStatus.Length > 0)
            {
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";
            }

            string SelectCommand = @"SELECT Packages.PackNumber, Packages.ProductType, Packages.MainOrderID, MainOrders.MegaOrderID, PackageStatus, FactoryName, Packages.PackingDateTime,
                Packages.StorageDateTime, Packages.ExpeditionDateTime, Packages.DispatchDateTime, Packages.PackageID, Packages.DispatchID, Packages.TrayID FROM Packages
                INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
                INNER JOIN infiniu2_catalog.dbo.PackageStatuses ON Packages.PackageStatusID = infiniu2_catalog.dbo.PackageStatuses.PackageStatusID
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" + OrdersProductionStatus +
                @"INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND NOT (AgreementStatusID=0 AND CreatedByClient=1)
                AND ClientID = " + ClientID + @" WHERE (DispatchID IS NULL OR DispatchID = " + DispatchID + @")";
            if (!CanEditDispatch)
                SelectCommand = @"SELECT Packages.PackNumber, Packages.ProductType, Packages.MainOrderID, MainOrders.MegaOrderID, PackageStatus, FactoryName, Packages.PackingDateTime,
                Packages.StorageDateTime, Packages.ExpeditionDateTime, Packages.DispatchDateTime, Packages.PackageID, Packages.DispatchID, Packages.TrayID FROM Packages
                INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
                INNER JOIN infiniu2_catalog.dbo.PackageStatuses ON Packages.PackageStatusID = infiniu2_catalog.dbo.PackageStatuses.PackageStatusID
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID" + OrdersProductionStatus +
                @"INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND NOT (AgreementStatusID=0 AND CreatedByClient=1)
                AND ClientID = " + ClientID + @" WHERE (DispatchID = " + DispatchID + @")";
            //WHERE (DispatchID IS NULL OR DispatchID = " + DispatchID + @") AND MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + @")" + OrdersProductionStatus + ")";
            //            string SelectCommand = @"SELECT PackNumber, ProductType, MainOrderID, PackageStatus, FactoryName, PackingDateTime,
            //				StorageDateTime, ExpeditionDateTime, DispatchDateTime, PackageID, DispatchID FROM Packages
            //				INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
            //				INNER JOIN infiniu2_catalog.dbo.PackageStatuses ON Packages.PackageStatusID = infiniu2_catalog.dbo.PackageStatuses.PackageStatusID
            //				WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE ClientID = " + ClientID + @"))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                PackagesDT.Clear();
                DA.Fill(PackagesDT);
            }
        }

        public void UpdateMegaOrders(int MegaOrderID)
        {
            string SelectCommand = @"SELECT MegaOrderID, OrderNumber, Weight, MegaOrders.FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MegaOrdersDT);
            }
        }

        public void UpdateMainOrders(int MegaOrderID)
        {
            string SelectCommand = @"SELECT MainOrderID, MegaOrderID, Weight, MainOrders.FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID, ProfilPackCount, TPSPackCount, FactoryName, Notes FROM MainOrders
                INNER JOIN infiniu2_catalog.dbo.Factory ON MainOrders.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
                WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDT);
            }
        }

        public void UpdatePackages(int MegaOrderID)
        {
            string SelectCommand = @"SELECT Packages.PackNumber, Packages.ProductType, Packages.MainOrderID, MainOrders.MegaOrderID, PackageStatus, FactoryName, Packages.PackingDateTime,
                Packages.StorageDateTime, Packages.ExpeditionDateTime, Packages.DispatchDateTime, Packages.PackageID, Packages.DispatchID, Packages.TrayID FROM Packages
                INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
                INNER JOIN infiniu2_catalog.dbo.PackageStatuses ON Packages.PackageStatusID = infiniu2_catalog.dbo.PackageStatuses.PackageStatusID
                INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID AND Packages.MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ") ORDER BY PackNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }
        }

        public void FilterMainOrders(int MegaOrderID)
        {
            MainOrdersBS.Filter = "MegaOrderID = " + MegaOrderID;
            MainOrdersBS.MoveFirst();
        }

        public void FilterPackages(int MainOrderID)
        {
            PackagesBS.Filter = "MainOrderID = " + MainOrderID;
            PackagesBS.MoveFirst();
        }

        public bool FilterFrontsOrders(int PackageID, int MainOrderID)
        {
            DataTable OriginalFrontsOrdersDT = new DataTable();
            OriginalFrontsOrdersDT = FrontsOrdersDT.Clone();

            string SelectCommand = @"SELECT  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, Square, Notes FROM FrontsOrders
                WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID = " + PackageID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDT.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDT.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            FrontsOrdersDT.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow Row in OriginalFrontsOrdersDT.Rows)
                        {
                            DataRow NewRow = FrontsOrdersDT.NewRow();
                            NewRow.ItemArray = Row.ItemArray;
                            FrontsOrdersDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDT.Dispose();

            return FrontsOrdersDT.Rows.Count > 0;
        }

        public bool FilterDecorOrders(int PackageID, int MainOrderID)
        {
            DataTable OriginalDecorOrdersDT = new DataTable();
            OriginalDecorOrdersDT = DecorOrdersDT.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrderID, ProductID, DecorID, ColorID, PatinaID, InsetTypeID, InsetColorID, Height, Length, Width, Count, Notes FROM DecorOrders
                WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID = " + PackageID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDT.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDT.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            DecorOrdersDT.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow Row in OriginalDecorOrdersDT.Rows)
                        {
                            DataRow NewRow = DecorOrdersDT.NewRow();
                            NewRow.ItemArray = Row.ItemArray;
                            DecorOrdersDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDT.Dispose();

            return DecorOrdersDT.Rows.Count > 0;
        }

        public bool SetMegaOrderDispatchStatus()
        {
            for (int i = 0; i < MegaOrdersDT.Rows.Count; i++)
            {
                DataRow[] DRows = MainOrdersDT.Select("CheckBoxColumn=TRUE AND MegaOrderID=" + MegaOrdersDT.Rows[i]["MegaOrderID"]);
                DataRow[] MRows = MainOrdersDT.Select("MegaOrderID=" + MegaOrdersDT.Rows[i]["MegaOrderID"]);
                if (DRows.Count() > 0 && DRows.Count() == MRows.Count())
                    MegaOrdersDT.Rows[i]["CheckBoxColumn"] = true;
            }
            return true;
        }

        public bool SetMainOrderDispatchStatus()
        {
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                DataRow[] DRows = PackagesDT.Select("DispatchID IS NOT NULL AND MainOrderID=" + MainOrdersDT.Rows[i]["MainOrderID"]);
                DataRow[] MRows = PackagesDT.Select("MainOrderID=" + MainOrdersDT.Rows[i]["MainOrderID"]);
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

        public void FlagMainOrders(int MegaOrderID, bool Checked)
        {
            DataRow[] Rows = MainOrdersDT.Select("MegaOrderID=" + MegaOrderID);
            foreach (DataRow item in Rows)
            {
                item["CheckBoxColumn"] = Checked;
                FlagPackages(Convert.ToInt32(item["MainOrderID"]), Checked);
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

        public void AddDispatch(object PrepareDispatchDateTime)
        {
            string SelectCommand = "SELECT TOP 0 * FROM Dispatch";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["CreationDateTime"] = Security.GetCurrentDate();
                        if (PrepareDispatchDateTime != null)
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
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["DispatchID"]);
                }
            }
            return 0;
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

    public class PackingReport : IAllFrontParameterName, IIsMarsel
    {
        int ClientID = 0;
        public bool ColorFullName = false;
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public DataTable FrontsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        public PackingReport()
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig )
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL",
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
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig ) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID ORDER BY TechStoreName";
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
            FrontsResultDataTable.Columns.Add(new DataColumn(("ConfirmDateTime"), System.Type.GetType("System.String")));
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
            DecorResultDataTable.Columns.Add(new DataColumn(("ConfirmDateTime"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        public bool IsMegaComplaint(int MegaOrderID)
        {
            bool IsComplaint = false;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsComplaint FROM MegaOrders WHERE MegaOrderID=" +
                    MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return IsComplaint;

                    IsComplaint = Convert.ToBoolean(DT.Rows[0]["IsComplaint"]);
                }
            }
            return IsComplaint;
        }

        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
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

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            //if (ClientID == 101)
            //    return Rows[0]["OldName"].ToString();
            //else
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }

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
            string Front = "";
            string FrameColor = "";
            string InsetColor = "";
            string TechnoInset = "";
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));
                TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

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

                NewRow["InsetType"] = InsetType;
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetColor"] = InsetColor;
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Front"] = Front;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["AccountingName"] = Row["AccountingName"];
                NewRow["ConfirmDateTime"] = Row["ConfirmDateTime"];
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
                NewRow["ConfirmDateTime"] = Row["ConfirmDateTime"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(int[] Dispatches, int MainOrderID, int FactoryID)
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

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, MegaOrders.ConfirmDateTime FROM FrontsOrders 
                INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE FrontsOrders.MainOrderID = " + MainOrderID + FactoryFilter1,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE Packages.DispatchID IN (" + string.Join(",", Dispatches) + ") AND MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter2 + ")",
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

        private bool FilterDecorOrders(int[] Dispatches, int MainOrderID, int FactoryID)
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

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, MegaOrders.ConfirmDateTime FROM DecorOrders
                INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID WHERE DecorOrders.MainOrderID = " + MainOrderID + FactoryFilter1,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE Packages.DispatchID IN (" + string.Join(",", Dispatches) + ") AND MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter2 + ")",
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

        private bool HasFronts(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
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

        private bool HasDecor(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
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

        private string GetCellValue(ref HSSFSheet sheet1, int row, int col)
        {
            String value = "";

            try
            {
                HSSFCell cell = sheet1.GetRow(row - 1).GetCell(col - 1);

                if (cell.CellType != HSSFCell.CELL_TYPE_BLANK)
                {
                    switch (cell.CellType)
                    {
                        case HSSFCell.CELL_TYPE_NUMERIC:
                            // ********* Date comes here ************

                            // Numeric type
                            value = cell.NumericCellValue.ToString();

                            break;

                        case HSSFCell.CELL_TYPE_BOOLEAN:
                            // Boolean type
                            value = cell.BooleanCellValue.ToString();
                            break;

                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }

            }
            catch (Exception)
            {
                value = "";
            }

            return value.Trim();
        }

        public void CreateReport(ref HSSFWorkbook thssfworkbook, DataTable OrderDT, int[] Dispatches,
            string DispatchDate, string OrderNumber, string ClientName, int iClientID, int FactoryID)
        {
            string SheetName = "Ведомость Профиль+ТПС";
            ClientID = iClientID;
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

            if (HasFronts(MainOrderIDs, FactoryID))
            {
                CreateFrontsExcel(ref thssfworkbook, ref sheet1, OrderDT, Dispatches, DispatchDate, ClientName, ref RowIndex, FactoryID);
            }

            if (HasDecor(MainOrderIDs, FactoryID))
            {
                CreateDecorExcel(ref thssfworkbook, ref sheet1, OrderDT, Dispatches, DispatchDate, ClientName, RowIndex, FactoryID);
            }
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, DataTable OrdersDT, int[] Dispatches,
            string DispatchDate, string ClientName, ref int RowIndex, int FactoryID)
        {
            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

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
            //if (ColorFullName)
            //{
            //    sheet1.SetColumnWidth(3, 18 * 256);
            //    sheet1.SetColumnWidth(4, 15 * 256);
            //    sheet1.SetColumnWidth(5, 18 * 256);
            //}
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

            HSSFFont ComplaintFont = hssfworkbook.CreateFont();
            ComplaintFont.Boldweight = 8 * 256;
            ComplaintFont.FontName = "Calibri";
            ComplaintFont.IsItalic = true;

            HSSFCellStyle ComplaintCellStyle = hssfworkbook.CreateCellStyle();
            ComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.SetFont(ComplaintFont);

            HSSFCellStyle GreyComplaintCellStyle = hssfworkbook.CreateCellStyle();
            GreyComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyComplaintCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyComplaintCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyComplaintCellStyle.SetFont(ComplaintFont);

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

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 6, "Утверждаю...............");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterFrontsOrders(Dispatches, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                bool IsComplaint = IsMegaComplaint(Convert.ToInt32(OrdersDT.Rows[i]["MegaOrderID"]));
                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " +
                    ClientName + " № " + Convert.ToInt32(OrdersDT.Rows[i]["OrderNumber"]) + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
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
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Бухг. наим.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Согласовано");
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
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
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
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
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
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
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

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
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

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, DataTable OrdersDT, int[] Dispatches,
            string DispatchDate, string ClientName, int RowIndex, int FactoryID)
        {
            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;

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

            HSSFFont ComplaintFont = hssfworkbook.CreateFont();
            ComplaintFont.Boldweight = 8 * 256;
            ComplaintFont.FontName = "Calibri";
            ComplaintFont.IsItalic = true;

            HSSFCellStyle ComplaintCellStyle = hssfworkbook.CreateCellStyle();
            ComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.SetFont(ComplaintFont);

            HSSFCellStyle GreyComplaintCellStyle = hssfworkbook.CreateCellStyle();
            GreyComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyComplaintCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyComplaintCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyComplaintCellStyle.SetFont(ComplaintFont);

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
                FilterDecorOrders(Dispatches, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                bool IsComplaint = IsMegaComplaint(Convert.ToInt32(OrdersDT.Rows[i]["MegaOrderID"]));
                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Клиент: " + ClientName + " № " + Convert.ToInt32(OrdersDT.Rows[i]["OrderNumber"]) + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }
                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Название");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell8.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Бухг. наим.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Согласовано");
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

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3)
                            {
                                //sheet1.CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

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
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //sheet1.CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

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
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
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
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }
    }

    public class DispatchReport
    {
        PackingReport PackingReport;

        PackagesCount PackagesCount;

        int ClientID = 0;
        int PermitID = -1;
        int[] Dispatches;

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

        public DispatchReport()
        {
            Create();
        }

        public void Initialize()
        {
            ClearPackages();
            if (Dispatches.Count() > 0)
                PermitID = GetPermitID(Dispatches[0]);
            else
                PermitID = -1;
            FillPackages();
        }

        private void ClearPackages()
        {
            PackagesDT.Clear();
        }

        public int CurrentClient
        {
            get { return ClientID; }
            set { ClientID = value; }
        }

        public int[] CurrentDispatches
        {
            get { return Dispatches; }
            set { Dispatches = value; }
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
                @"SELECT MainOrders.MegaOrderID, MegaOrders.OrderNumber, Packages.ProductType, Packages.FactoryID, Packages.MainOrderID, Packages.PackageID, Packages.PackageStatusID
                FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                WHERE Packages.DispatchID IN (" + string.Join(",", Dispatches) + ")", ConnectionStrings.MarketingOrdersConnectionString))
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
                    " WHERE ClientID=" + ClientID, ConnectionStrings.MarketingReferenceConnectionString))
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
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        private int[] GetOrderNumbers()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DT = DV.ToTable(true, new string[] { "OrderNumber" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["OrderNumber"]);
            DT.Dispose();
            return rows;
        }

        private int[] GetOrderNumbers(int FactoryID)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DV.RowFilter = "FactoryID = " + FactoryID;
                DT = DV.ToTable(true, new string[] { "OrderNumber" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["OrderNumber"]);
            DT.Dispose();
            return rows;
        }

        private DataTable GetOrdersID()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, string.Empty, "MainOrderID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "MegaOrderID", "OrderNumber", "MainOrderID" });
            }

            return DT;
        }

        public int[] GetMainOrders(int FactoryID)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, string.Empty, "MainOrderID", DataViewRowState.CurrentRows))
            {
                DV.RowFilter = "FactoryID=" + FactoryID;
                DT = DV.ToTable(true, new string[] { "MainOrderID" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            DT.Dispose();
            return rows;
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
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND FactoryID = " + FactoryID + " AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + @"))",
                    ConnectionStrings.MarketingOrdersConnectionString))
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
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND FactoryID = " + FactoryID + " AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
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
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND FactoryID = " + FactoryID + " AND Packages.DispatchID IN (" + string.Join(",", Dispatches) + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
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

        private bool Fill(int[] MainOrders, int FactoryID)
        {
            SimpleResultDT.Clear();

            PackagesCount.AllPackages = 0;
            PackagesCount.AllPackedPackages = 0;
            PackagesCount.ProfilPackages = 0;
            PackagesCount.ProfilPackedPackages = 0;
            PackagesCount.TPSPackages = 0;
            PackagesCount.TPSPackedPackages = 0;

            int MainOrderID = 0;
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                int FrontsPackedPackagesCount = 0;
                int DecorPackedPackagesCount = 0;
                int AllPackedPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                MainOrderID = MainOrders[i];

                FrontsPackedPackagesCount = GetDispPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackedPackagesCount = GetDispPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackedPackagesCount = FrontsPackedPackagesCount + DecorPackedPackagesCount;

                PackagesCount.AllPackedPackages += AllPackedPackagesCount;
                PackagesCount.ProfilPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;
                PackagesCount.TPSPackedPackages += FrontsPackedPackagesCount + DecorPackedPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                PackagesCount.AllPackages += AllPackagesCount;
                PackagesCount.ProfilPackages += FrontsPackagesCount + DecorPackagesCount;
                PackagesCount.TPSPackages += FrontsPackagesCount + DecorPackagesCount;

                DataRow NewRow = SimpleResultDT.NewRow();
                NewRow["MainOrder"] = "Подзаказ №" + MainOrders[i];
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsPackedPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorPackedPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllPackedPackagesCount.ToString() + " / " + AllPackagesCount.ToString();
                SimpleResultDT.Rows.Add(NewRow);
            }

            return SimpleResultDT.Rows.Count > 0;
        }

        private string GetCellValue(ref HSSFSheet sheet1, int row, int col)
        {
            String value = "";

            try
            {
                HSSFCell cell = sheet1.GetRow(row - 1).GetCell(col - 1);

                if (cell.CellType != HSSFCell.CELL_TYPE_BLANK)
                {
                    switch (cell.CellType)
                    {
                        case HSSFCell.CELL_TYPE_NUMERIC:
                            // ********* Date comes here ************

                            // Numeric type
                            value = cell.NumericCellValue.ToString();

                            break;

                        case HSSFCell.CELL_TYPE_BOOLEAN:
                            // Boolean type
                            value = cell.BooleanCellValue.ToString();
                            break;

                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }

            }
            catch (Exception)
            {
                value = "";
            }

            return value.Trim();
        }

        public int GetPermitID(int DispatchID)
        {
            string SelectCommand = @"SELECT * FROM PermitDetails WHERE DispatchID = " + DispatchID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        return Convert.ToInt32(DT.Rows[0]["PermitID"]);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        public void CreateReport(bool bNeedProfilList, bool bNeedTPSList, bool Attach, bool ColorFullName, bool NeedOpen, ref string PackagesReportName)
        {
            DataTable OrdersID = GetOrdersID();

            int[] ProfilMainOrders = GetMainOrders(1);
            int[] TPSMainOrders = GetMainOrders(2);

            if (bNeedProfilList && bNeedTPSList)
            {
                if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
                    return;
            }
            if (bNeedProfilList && !bNeedTPSList)
            {
                if (ProfilMainOrders.Count() == 0)
                    return;
            }
            if (!bNeedProfilList && bNeedTPSList)
            {
                if (TPSMainOrders.Count() == 0)
                    return;
            }

            int FactoryID = 1;
            int[] OrderNumbers = GetOrderNumbers();
            int[] ProfilOrderNumbers = GetOrderNumbers(1);
            int[] TPSOrderNumbers = GetOrderNumbers(2);

            string ClientName = GetClientName(ClientID);
            string OrderNumber = string.Empty;
            string ProfilOrderNumber = string.Empty;
            string TPSOrderNumber = string.Empty;

            if (OrderNumbers.Count() > 0)
            {
                for (int i = 0; i < OrderNumbers.Count(); i++)
                    OrderNumber += OrderNumbers[i] + ", ";
                OrderNumber = OrderNumber.Substring(0, OrderNumber.Length - 2);
            }
            if (ProfilOrderNumbers.Count() > 0)
            {
                for (int i = 0; i < ProfilOrderNumbers.Count(); i++)
                    ProfilOrderNumber += ProfilOrderNumbers[i] + ", ";
                ProfilOrderNumber = ProfilOrderNumber.Substring(0, ProfilOrderNumber.Length - 2);
            }
            if (TPSOrderNumbers.Count() > 0)
            {
                for (int i = 0; i < TPSOrderNumbers.Count(); i++)
                    TPSOrderNumber += TPSOrderNumbers[i] + ", ";
                TPSOrderNumber = TPSOrderNumber.Substring(0, TPSOrderNumber.Length - 2);
            }

            string Firm = "ЗОВ-Профиль+ЗОВ-ТПС";

            ClientName = ClientName.Replace('/', '-');

            if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
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
                PackingReport = new PackingReport()
                {
                    ColorFullName = ColorFullName
                };
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "ЗОВ-Профиль", ClientName, ProfilOrderNumber);
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ЗОВ-ТПС", ClientName, TPSOrderNumber);
                        }

                        PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), OrderNumber, ClientName, ClientID, 0);
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "ЗОВ-Профиль", ClientName, ProfilOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), ProfilOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ЗОВ-ТПС", ClientName, TPSOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), TPSOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "ЗОВ-Профиль", ClientName, ProfilOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), ProfilOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ЗОВ-ТПС", ClientName, TPSOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), TPSOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                }

                ClientName = ClientName.Replace('\"', '\'');

                string FileName = ClientName + " № " + OrderNumber + " " + Firm;
                if (ClientID == 145)
                {
                    Firm = "(Profil+TPS)";
                    if (bNeedProfilList && !bNeedTPSList)
                        Firm = "(Profil)";
                    if (!bNeedProfilList && bNeedTPSList)
                        Firm = "(TPS)";
                    FileName = "Dispatch " + Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy") + " " + Firm;
                }
                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                PackagesReportName = file.FullName;
                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                if (ClientID == 145)
                {
                    Send(file.FullName, "horuz9@list.ru");
                    //Send(file.FullName, "zovvozvrat@mail.ru");
                    //Send(file.FullName, "romanchukgrad@gmail.com");
                    //Send(file.FullName, "nsq@tut.by");
                }
                if (NeedOpen)
                    System.Diagnostics.Process.Start(file.FullName);
            }


            else
            {
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "Профиль", ClientName, ProfilOrderNumber);
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ТПС", ClientName, TPSOrderNumber);
                        }
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "Профиль", ClientName, ProfilOrderNumber);
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ТПС", ClientName, TPSOrderNumber);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        Firm = "Профиль";
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "Профиль", ClientName, ProfilOrderNumber);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        Firm = "ТПС";
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ТПС", ClientName, TPSOrderNumber);
                        }
                    }
                }

                ClientName = ClientName.Replace('\"', '\'');

                string FileName = ClientName + " № " + OrderNumber + " " + Firm;
                if (ClientID == 145)
                {
                    Firm = "(Profil+TPS)";
                    if (bNeedProfilList && !bNeedTPSList)
                        Firm = "(Profil)";
                    if (!bNeedProfilList && bNeedTPSList)
                        Firm = "(TPS)";
                    FileName = "Dispatch " + Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy") + " " + Firm;
                }

                string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                int j = 1;
                while (file.Exists == true)
                {
                    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                }

                PackagesReportName = file.FullName;
                FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                hssfworkbook.Write(NewFile);
                NewFile.Close();

                if (ClientID == 145)
                {
                    Send(file.FullName, "horuz9@list.ru");
                    //Send(file.FullName, "zovvozvrat@mail.ru");
                    //Send(file.FullName, "romanchukgrad@gmail.com");
                    //Send(file.FullName, "nsq@tut.by");
                }
                if (NeedOpen)
                    System.Diagnostics.Process.Start(file.FullName);
            }
        }

        public void CreateReport(ref HSSFWorkbook hssfworkbook, bool bNeedProfilList, bool bNeedTPSList, bool Attach, bool ColorFullName)
        {
            DataTable OrdersID = GetOrdersID();

            int[] ProfilMainOrders = GetMainOrders(1);
            int[] TPSMainOrders = GetMainOrders(2);

            if (bNeedProfilList && bNeedTPSList)
            {
                if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
                    return;
            }
            if (bNeedProfilList && !bNeedTPSList)
            {
                if (ProfilMainOrders.Count() == 0)
                    return;
            }
            if (!bNeedProfilList && bNeedTPSList)
            {
                if (TPSMainOrders.Count() == 0)
                    return;
            }

            int FactoryID = 1;
            int[] OrderNumbers = GetOrderNumbers();
            int[] ProfilOrderNumbers = GetOrderNumbers(1);
            int[] TPSOrderNumbers = GetOrderNumbers(2);

            string ClientName = GetClientName(ClientID);
            string OrderNumber = string.Empty;
            string ProfilOrderNumber = string.Empty;
            string TPSOrderNumber = string.Empty;

            if (OrderNumbers.Count() > 0)
            {
                for (int i = 0; i < OrderNumbers.Count(); i++)
                    OrderNumber += OrderNumbers[i] + ", ";
                OrderNumber = OrderNumber.Substring(0, OrderNumber.Length - 2);
            }
            if (ProfilOrderNumbers.Count() > 0)
            {
                for (int i = 0; i < ProfilOrderNumbers.Count(); i++)
                    ProfilOrderNumber += ProfilOrderNumbers[i] + ", ";
                ProfilOrderNumber = ProfilOrderNumber.Substring(0, ProfilOrderNumber.Length - 2);
            }
            if (TPSOrderNumbers.Count() > 0)
            {
                for (int i = 0; i < TPSOrderNumbers.Count(); i++)
                    TPSOrderNumber += TPSOrderNumbers[i] + ", ";
                TPSOrderNumber = TPSOrderNumber.Substring(0, TPSOrderNumber.Length - 2);
            }

            //string Firm = "ЗОВ-Профиль+ЗОВ-ТПС";

            ClientName = ClientName.Replace('/', '-');

            if (ProfilMainOrders.Count() == 0 && TPSMainOrders.Count() == 0)
            {
                MessageBox.Show("Выбранный заказ пуст");
                return;
            }

            if (Attach)
            {
                PackingReport = new PackingReport()
                {
                    ColorFullName = ColorFullName
                };
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "ЗОВ-Профиль", ClientName, ProfilOrderNumber);
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ЗОВ-ТПС", ClientName, TPSOrderNumber);
                        }

                        PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), OrderNumber, ClientName, ClientID, 0);
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "ЗОВ-Профиль", ClientName, ProfilOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), ProfilOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ЗОВ-ТПС", ClientName, TPSOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), TPSOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "ЗОВ-Профиль", ClientName, ProfilOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), ProfilOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ЗОВ-ТПС", ClientName, TPSOrderNumber);
                            PackingReport.CreateReport(ref hssfworkbook, OrdersID, Dispatches, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"), TPSOrderNumber, ClientName, ClientID, FactoryID);
                        }
                    }
                }

                //ClientName = ClientName.Replace('\"', '\'');

                //string FileName = ClientName + " № " + OrderNumber + " " + Firm;
                //if (ClientID == 145)
                //{
                //    Firm = "(Profil+TPS)";
                //    if (bNeedProfilList && !bNeedTPSList)
                //        Firm = "(Profil)";
                //    if (!bNeedProfilList && bNeedTPSList)
                //        Firm = "(TPS)";
                //    FileName = "Dispatch " + Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy") + " " + Firm;
                //}
                //string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                //FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                //int j = 1;
                //while (file.Exists == true)
                //{
                //    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                //}

                //PackagesReportName = file.FullName;
                //FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                //hssfworkbook.Write(NewFile);
                //NewFile.Close();

                //if (ClientID == 145)
                //{
                //    Send(file.FullName, "horuz9@list.ru");
                //    Send(file.FullName, "zovvozvrat@mail.ru");
                //    //Send(file.FullName, "romanchukgrad@gmail.com");
                //    //Send(file.FullName, "nsq@tut.by");
                //}
                //if (NeedOpen)
                //    System.Diagnostics.Process.Start(file.FullName);
            }


            else
            {
                if (bNeedProfilList && bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "Профиль", ClientName, ProfilOrderNumber);
                        }

                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ТПС", ClientName, TPSOrderNumber);
                        }
                    }
                    if (ProfilMainOrders.Count() > 0 && TPSMainOrders.Count() < 1)
                    {
                        FactoryID = 1;
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "Профиль", ClientName, ProfilOrderNumber);
                        }
                    }
                    if (ProfilMainOrders.Count() < 1 && TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ТПС", ClientName, TPSOrderNumber);
                        }
                    }
                }

                if (bNeedProfilList && !bNeedTPSList)
                {
                    if (ProfilMainOrders.Count() > 0)
                    {
                        FactoryID = 1;
                        //Firm = "Профиль";
                        if (Fill(ProfilMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, ProfilMainOrders, 1, "Профиль", ClientName, ProfilOrderNumber);
                        }
                    }
                }

                if (!bNeedProfilList && bNeedTPSList)
                {
                    if (TPSMainOrders.Count() > 0)
                    {
                        FactoryID = 2;
                        //Firm = "ТПС";
                        if (Fill(TPSMainOrders, FactoryID))
                        {
                            CreateExcel(ref hssfworkbook, TPSMainOrders, 2, "ТПС", ClientName, TPSOrderNumber);
                        }
                    }
                }

                //ClientName = ClientName.Replace('\"', '\'');

                //string FileName = ClientName + " № " + OrderNumber + " " + Firm;
                //if (ClientID == 145)
                //{
                //    Firm = "(Profil+TPS)";
                //    if (bNeedProfilList && !bNeedTPSList)
                //        Firm = "(Profil)";
                //    if (!bNeedProfilList && bNeedTPSList)
                //        Firm = "(TPS)";
                //    FileName = "Dispatch " + Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy") + " " + Firm;
                //}

                //string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                //FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
                //int j = 1;
                //while (file.Exists == true)
                //{
                //    file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
                //}

                //PackagesReportName = file.FullName;
                //FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
                //hssfworkbook.Write(NewFile);
                //NewFile.Close();

                //if (ClientID == 145)
                //{
                //    Send(file.FullName, "horuz9@list.ru");
                //    Send(file.FullName, "zovvozvrat@mail.ru");
                //    //Send(file.FullName, "romanchukgrad@gmail.com");
                //    //Send(file.FullName, "nsq@tut.by");
                //}
                //if (NeedOpen)
                //    System.Diagnostics.Process.Start(file.FullName);
            }
            //hssfworkbook.RemoveSheetAt(0);
        }

        public void Send(string FileName, string MailAddressTo)
        {
            string AccountPassword = "1290qpalzm";
            string SenderEmail = "zovprofilreport@mail.ru";

            string from = SenderEmail;

            if (MailAddressTo.Length == 0)
            {
                MessageBox.Show("У клиента не указан Email. Отправка отчета не возможна");
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

        public void GetDispatchInfo(ref object date1, ref object date2, ref object date3, ref object date4, ref object date5,
            ref object User1, ref object User2, ref object User3)
        {
            CreationDateTime = date1;
            ConfirmExpDateTime = date2;
            ConfirmDispDateTime = date3;
            RealDispDateTime = date4;
            PrepareDispatchDateTime = date5;
            ConfirmExpUserID = User1;
            ConfirmDispUserID = User2;
            RealDispUserID = User3;
        }

        public string GetBarcodeNumber(int BarcodeType, int iNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string sNumber = "";
            if (iNumber.ToString().Length == 1)
                sNumber = "00000000" + iNumber.ToString();
            if (iNumber.ToString().Length == 2)
                sNumber = "0000000" + iNumber.ToString();
            if (iNumber.ToString().Length == 3)
                sNumber = "000000" + iNumber.ToString();
            if (iNumber.ToString().Length == 4)
                sNumber = "00000" + iNumber.ToString();
            if (iNumber.ToString().Length == 5)
                sNumber = "0000" + iNumber.ToString();
            if (iNumber.ToString().Length == 6)
                sNumber = "000" + iNumber.ToString();
            if (iNumber.ToString().Length == 7)
                sNumber = "00" + iNumber.ToString();
            if (iNumber.ToString().Length == 8)
                sNumber = "0" + iNumber.ToString();

            StringBuilder BarcodeNumber = new StringBuilder(Type);
            BarcodeNumber.Append(sNumber);

            return BarcodeNumber.ToString();
        }

        public void CreateBarcode(int PermitID)
        {
            Barcode Barcode = new Barcode();
            string BarcodeNumber = GetBarcodeNumber(18, PermitID);
            string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
            Image img = Barcode.GetBarcode(Barcode.BarcodeLength.Short, 45, BarcodeNumber);
            if (!File.Exists(FileName))
                img.Save(FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        public static int LoadImage(string path, HSSFWorkbook wb)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, (int)file.Length);
            return wb.AddPicture(buffer, HSSFWorkbook.PICTURE_TYPE_JPEG);

        }

        private void CreateExcel(ref HSSFWorkbook hssfworkbook, int[] MainOrders, int FactoryID, string Firm,
            string ClientName, string OrderNumber)
        {
            HSSFSheet sheet1 = hssfworkbook.CreateSheet(Firm);
            HSSFPatriarch patriarch = sheet1.CreateDrawingPatriarch();
            HSSFClientAnchor anchor;
            HSSFPicture picture;
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

            HSSFFont BarcodeFont = hssfworkbook.CreateFont();
            BarcodeFont.FontHeightInPoints = 12;
            BarcodeFont.FontName = "Calibri";

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

            HSSFCellStyle BarcodeStyle = hssfworkbook.CreateCellStyle();
            BarcodeStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            BarcodeStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            BarcodeStyle.SetFont(BarcodeFont);
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
                    "Отгрузка произведена: " + GetUserName(Convert.ToInt32(RealDispUserID)) + " " + Convert.ToDateTime(RealDispDateTime).ToString("dd.MM.yyyy HH:mm"));
                ConfirmCell.CellStyle = TempStyle;
            }
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                "Ведомость создана: " + Security.CurrentUserShortName + " " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            ConfirmCell.CellStyle = TempStyle;

            //ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), 0, "Дата отгрузки: " + PrepareDispatchDateTime.ToString("dd.MM.yyyy"));
            //ConfirmCell.CellStyle = TempStyle;
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Участок: " + Firm);
            ConfirmCell.CellStyle = TempStyle;
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " + ClientName);
            ConfirmCell.CellStyle = TempStyle;
            ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Заказы: " + OrderNumber);
            ConfirmCell.CellStyle = TempStyle;

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

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));

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

            if (PermitID != -1)
            {
                CreateBarcode(PermitID);
                anchor = new HSSFClientAnchor(0, 1, 1000, 8, 2, RowIndex, 3, RowIndex + 4)
                {
                    AnchorType = 2
                };
                string BarcodeNumber = GetBarcodeNumber(18, PermitID);
                string FileName = System.Environment.GetEnvironmentVariable("TEMP") + @"/" + BarcodeNumber + ".jpg";
                picture = patriarch.CreatePicture(anchor, LoadImage(FileName, hssfworkbook));
                ConfirmCell = sheet1.CreateRow(RowIndex + 4).CreateCell(2);
                ConfirmCell.SetCellValue(BarcodeNumber);
                ConfirmCell.CellStyle = BarcodeStyle;
                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex + 4, 2, RowIndex + 4, 3));
            }
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
    }


    public class NotReadyPackagesCount
    {
        public int ProfilNotReadyPackages;
        public int TPSNotReadyPackages;
        public int AllNotReadyPackages;

        public int ProfilPackages;
        public int TPSPackages;
        public int AllPackages;
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

            Font F = new Font("Arial", FontSize, FontStyle.Regular);

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
            //DrawBarcodeText(Barcode.BarcodeLength.Short, G, Text, 0, 23);


            //create text
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;


            Bar.Dispose();
            G.Dispose();

            GC.Collect();

            return B;
        }
    }

    public class NotReadyProductsDetails : IAllFrontParameterName, IIsMarsel
    {
        int ClientID = 0;
        string ClientName = string.Empty;

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

        public NotReadyProductsDetails()
        {
            Create();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }
            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig )
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PatinaRAL ",
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
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig ) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID  ORDER BY TechStoreName";
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
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        #region Реализация интерфейса IReference

        public bool IsMegaComplaint(int MegaOrderID)
        {
            bool IsComplaint = false;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsComplaint FROM MegaOrders WHERE MegaOrderID=" +
                    MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return IsComplaint;

                    IsComplaint = Convert.ToBoolean(DT.Rows[0]["IsComplaint"]);
                }
            }
            return IsComplaint;
        }

        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }
        #endregion

        //public void GetClientName(int MainOrderID)
        //{
        //    ClientID = 0;
        //    ClientName = string.Empty;

        //    using (DataTable DT = new DataTable())
        //    {
        //        using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM MegaOrders" +
        //            " WHERE MegaOrderID=(SELECT MegaOrderID FROM MainOrders WHERE MainOrderID=" + MainOrderID + ")", ConnectionStrings.MarketingOrdersConnectionString))
        //        {
        //            DA.Fill(DT);
        //            if (DT.Rows.Count > 0)
        //                ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
        //        }
        //    }

        //    DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
        //    if (Rows.Count() > 0)
        //        ClientName = Rows[0]["ClientName"].ToString();
        //}

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
            //if (ClientID == 101)
            //    return Rows[0]["OldName"].ToString();
            //else
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

            string Front = "";
            string FrameColor = "";
            string InsetColor = "";
            string TechnoInset = "";

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));
                TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

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

                NewRow["InsetType"] = InsetType;
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetColor"] = InsetColor;
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Front"] = Front;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
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

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterFrontsOrders(int DispatchID, int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE PackageStatusID=0 AND DispatchID=" + DispatchID + " AND MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter + ")",
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

        private bool FilterDecorOrders(int DispatchID, int MainOrderID, int FactoryID)
        {
            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID + FactoryFilter,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE PackageStatusID=0 AND DispatchID=" + DispatchID + " AND MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter + ")",
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

        private bool HasFronts(int DispatchID, int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE PackageStatusID=0 AND DispatchID = " + DispatchID + " AND MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
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

        private bool HasDecor(int DispatchID, int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE PackageStatusID=0 AND DispatchID = " + DispatchID + " AND MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
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

        private string GetCellValue(ref HSSFSheet sheet1, int row, int col)
        {
            String value = "";

            try
            {
                HSSFCell cell = sheet1.GetRow(row - 1).GetCell(col - 1);

                if (cell.CellType != HSSFCell.CELL_TYPE_BLANK)
                {
                    switch (cell.CellType)
                    {
                        case HSSFCell.CELL_TYPE_NUMERIC:
                            // ********* Date comes here ************

                            // Numeric type
                            value = cell.NumericCellValue.ToString();

                            break;

                        case HSSFCell.CELL_TYPE_BOOLEAN:
                            // Boolean type
                            value = cell.BooleanCellValue.ToString();
                            break;

                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }

            }
            catch (Exception)
            {
                value = "";
            }

            return value.Trim();
        }

        public void CreateReport(ref HSSFWorkbook thssfworkbook, ref HSSFSheet sheet1, ref HSSFSheet sheet2, ref int RowIndex1, ref int RowIndex2,
            DataTable OrderDT, int iClientID, int DispatchID, int FactoryID, string sClientName, string DispatchDate)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(OrderDT))
            {
                DT = DV.ToTable(true, new string[] { "MainOrderID" });
            }

            ClientID = iClientID;
            ClientName = sClientName;
            int[] MainOrderIDs = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                MainOrderIDs[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            DT.Dispose();

            if (FactoryID == 1)
            {
                if (HasFronts(DispatchID, MainOrderIDs, FactoryID))
                    CreateFrontsExcel(ref thssfworkbook, ref sheet1, ref RowIndex1, OrderDT, DispatchID, FactoryID, DispatchDate);
                if (HasDecor(DispatchID, MainOrderIDs, FactoryID))
                    CreateDecorExcel(ref thssfworkbook, ref sheet1, ref RowIndex1, OrderDT, DispatchID, FactoryID, DispatchDate);
            }
            if (FactoryID == 2)
            {
                if (HasFronts(DispatchID, MainOrderIDs, FactoryID))
                    CreateFrontsExcel(ref thssfworkbook, ref sheet2, ref RowIndex2, OrderDT, DispatchID, FactoryID, DispatchDate);
                if (HasDecor(DispatchID, MainOrderIDs, FactoryID))
                    CreateDecorExcel(ref thssfworkbook, ref sheet2, ref RowIndex2, OrderDT, DispatchID, FactoryID, DispatchDate);
            }
        }

        private void CreateFrontsExcel(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, ref int RowIndex, DataTable OrdersDT,
            int DispatchID, int FactoryID, string DispatchDate)
        {
            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

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

            HSSFFont ComplaintFont = hssfworkbook.CreateFont();
            ComplaintFont.Boldweight = 8 * 256;
            ComplaintFont.FontName = "Calibri";
            ComplaintFont.IsItalic = true;

            HSSFCellStyle ComplaintCellStyle = hssfworkbook.CreateCellStyle();
            ComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.SetFont(ComplaintFont);

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

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            #endregion

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterFrontsOrders(DispatchID, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                //GetClientName(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                bool IsComplaint = IsMegaComplaint(Convert.ToInt32(OrdersDT.Rows[i]["MegaOrderID"]));
                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " +
                    ClientName + " № " + Convert.ToInt32(OrdersDT.Rows[i]["OrderNumber"]) + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                int DisplayIndex = 0;
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
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;

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
                            Type t = FrontsResultDataTable.Rows[x][y].GetType();

                            if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                cell.CellStyle = PackNumberStyle;
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
                                if (IsComplaint)
                                    cell.CellStyle = ComplaintCellStyle;
                                else
                                    cell.CellStyle = SimpleCellStyle;
                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                if (IsComplaint)
                                    cell.CellStyle = ComplaintCellStyle;
                                else
                                    cell.CellStyle = SimpleCellStyle;
                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                cell.SetCellValue(FRows[x][y].ToString());
                                if (IsComplaint)
                                    cell.CellStyle = ComplaintCellStyle;
                                else
                                    cell.CellStyle = SimpleCellStyle;
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

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
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

            RowIndex++;
            RowIndex++;
        }

        private void CreateDecorExcel(ref HSSFWorkbook hssfworkbook, ref HSSFSheet sheet1, ref int RowIndex, DataTable OrdersDT,
            int DispatchID, int FactoryID, string DispatchDate)
        {
            HSSFFont TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            HSSFCellStyle TempStyle = hssfworkbook.CreateCellStyle();

            TempStyle.SetFont(TotalFont);

            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;

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

            HSSFFont ComplaintFont = hssfworkbook.CreateFont();
            ComplaintFont.Boldweight = 8 * 256;
            ComplaintFont.FontName = "Calibri";
            ComplaintFont.IsItalic = true;

            HSSFCellStyle ComplaintCellStyle = hssfworkbook.CreateCellStyle();
            ComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.SetFont(ComplaintFont);

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

            HSSFCellStyle TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);
            #endregion

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterDecorOrders(DispatchID, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                //GetClientName(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                bool IsComplaint = IsMegaComplaint(Convert.ToInt32(OrdersDT.Rows[i]["MegaOrderID"]));
                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Клиент: " + ClientName + " № " + Convert.ToInt32(OrdersDT.Rows[i]["OrderNumber"]) + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }
                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Название");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell8.CellStyle = HeaderStyle;

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

                            //sheet1.CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                            if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                cell.CellStyle = PackNumberStyle;
                                sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                continue;
                            }

                            if (t.Name == "Decimal")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

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
                                if (IsComplaint)
                                    cell.CellStyle = ComplaintCellStyle;
                                else
                                    cell.CellStyle = SimpleCellStyle;

                                continue;
                            }
                            if (t.Name == "Int32")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                if (DRows[x][y] == DBNull.Value)
                                    cell.SetCellValue(DRows[x][y].ToString());
                                else
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                if (IsComplaint)
                                    cell.CellStyle = ComplaintCellStyle;
                                else
                                    cell.CellStyle = SimpleCellStyle;

                                continue;
                            }

                            if (t.Name == "String" || t.Name == "DBNull")
                            {
                                HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                cell.SetCellValue(DRows[x][y].ToString());
                                if (IsComplaint)
                                    cell.CellStyle = ComplaintCellStyle;
                                else
                                    cell.CellStyle = SimpleCellStyle;
                                continue;
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
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
            RowIndex++;
            RowIndex++;
        }
    }

    public static class NotReadyProducts
    {
        static HSSFWorkbook hssfworkbook1;
        static HSSFWorkbook hssfworkbook2;
        static HSSFSheet OrdersNumbersSheet;
        static HSSFSheet ZOVProfilCommonSheet;
        static HSSFSheet ZOVTPSCommonSheet;
        static HSSFSheet ZOVProfilDetailsSheet;
        static HSSFSheet ZOVTPSDetailsSheet;

        static HSSFFont HeaderFont1;
        static HSSFFont HeaderFont2;
        static HSSFFont NotesFont;
        static HSSFFont SimpleFont1;
        static HSSFFont SimpleFont2;
        static HSSFFont TempFont1;
        static HSSFFont TempFont2;
        static HSSFFont TotalFont;

        static HSSFCellStyle EmptyCellStyle;
        static HSSFCellStyle ConfirmStyle;
        static HSSFCellStyle HeaderStyle1;
        static HSSFCellStyle HeaderStyle2;
        static HSSFCellStyle MainOrderCellStyle;
        static HSSFCellStyle MainOrderCellStyle1;
        static HSSFCellStyle MainOrderCellStyle2;
        static HSSFCellStyle NotesCellStyle;
        static HSSFCellStyle SimpleCellStyle;
        static HSSFCellStyle SimpleCellStyle1;
        static HSSFCellStyle TempStyle1;
        static HSSFCellStyle TempStyle2;
        static HSSFCellStyle TotalStyle;

        static NotReadyProductsDetails PackingReport;

        static NotReadyPackagesCount PackagesCount;

        static ArrayList Dispatches;
        static int CurrentDispatchID = 0;
        static string sFileName = string.Empty;

        static object PrepareDispatchDateTime = DBNull.Value;

        static DataTable SimpleResultDT = null;
        static DataTable AttachResultDT = null;
        static DataTable PackagesDT = null;
        static DataTable OrdersDT = null;

        public static void Initialize1()
        {
            Create();
            hssfworkbook1 = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook1.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook1.SummaryInformation = si;

            ZOVProfilCommonSheet = hssfworkbook1.CreateSheet("ЗОВ-Профиль");
            ZOVProfilCommonSheet.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            ZOVProfilCommonSheet.SetMargin(HSSFSheet.LeftMargin, (double).12);
            ZOVProfilCommonSheet.SetMargin(HSSFSheet.RightMargin, (double).07);
            ZOVProfilCommonSheet.SetMargin(HSSFSheet.TopMargin, (double).20);
            ZOVProfilCommonSheet.SetMargin(HSSFSheet.BottomMargin, (double).20);
            ZOVProfilCommonSheet.SetColumnWidth(0, 55 * 256);
            ZOVProfilCommonSheet.SetColumnWidth(1, 12 * 256);
            ZOVProfilCommonSheet.SetColumnWidth(2, 12 * 256);
            ZOVProfilCommonSheet.SetColumnWidth(3, 12 * 256);

            ZOVProfilDetailsSheet = hssfworkbook1.CreateSheet("Ведомость ЗОВ-Профиль");
            ZOVProfilDetailsSheet.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            ZOVProfilDetailsSheet.SetMargin(HSSFSheet.LeftMargin, (double).12);
            ZOVProfilDetailsSheet.SetMargin(HSSFSheet.RightMargin, (double).07);
            ZOVProfilDetailsSheet.SetMargin(HSSFSheet.TopMargin, (double).20);
            ZOVProfilDetailsSheet.SetMargin(HSSFSheet.BottomMargin, (double).20);
            int DisplayIndex = 0;
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 4 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 18 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 5 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 5 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 5 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 7 * 256);
            ZOVProfilDetailsSheet.SetColumnWidth(DisplayIndex++, 9 * 256);

            HeaderFont1 = hssfworkbook1.CreateFont();
            HeaderFont1.FontHeightInPoints = 13;
            HeaderFont1.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont1.FontName = "Calibri";

            NotesFont = hssfworkbook1.CreateFont();
            NotesFont.FontHeightInPoints = 12;
            NotesFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            NotesFont.FontName = "Calibri";

            SimpleFont1 = hssfworkbook1.CreateFont();
            SimpleFont1.FontHeightInPoints = 12;
            SimpleFont1.FontName = "Calibri";

            TempFont1 = hssfworkbook1.CreateFont();
            TempFont1.FontHeightInPoints = 12;
            TempFont1.FontName = "Calibri";

            TotalFont = hssfworkbook1.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            TotalFont.FontName = "Calibri";

            EmptyCellStyle = hssfworkbook1.CreateCellStyle();
            EmptyCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            EmptyCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            EmptyCellStyle.RightBorderColor = HSSFColor.BLACK.index;

            ConfirmStyle = hssfworkbook1.CreateCellStyle();
            ConfirmStyle.SetFont(HeaderFont1);

            HeaderStyle1 = hssfworkbook1.CreateCellStyle();
            HeaderStyle1.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle1.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle1.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle1.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle1.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle1.WrapText = true;
            HeaderStyle1.SetFont(HeaderFont1);

            MainOrderCellStyle = hssfworkbook1.CreateCellStyle();
            MainOrderCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle.SetFont(SimpleFont1);

            MainOrderCellStyle1 = hssfworkbook1.CreateCellStyle();
            MainOrderCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle1.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle1.SetFont(SimpleFont1);

            NotesCellStyle = hssfworkbook1.CreateCellStyle();
            NotesCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            NotesCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            NotesCellStyle.SetFont(NotesFont);

            SimpleCellStyle = hssfworkbook1.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle.SetFont(SimpleFont1);

            SimpleCellStyle1 = hssfworkbook1.CreateCellStyle();
            SimpleCellStyle1.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle1.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle1.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleCellStyle1.SetFont(SimpleFont1);

            TempStyle1 = hssfworkbook1.CreateCellStyle();
            TempStyle1.SetFont(TempFont1);

            TotalStyle = hssfworkbook1.CreateCellStyle();
            TotalStyle.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.BottomBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);
        }

        public static void Initialize2()
        {
            Create();
            hssfworkbook2 = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook2.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook2.SummaryInformation = si;

            OrdersNumbersSheet = hssfworkbook2.CreateSheet("Номера заказов");
            OrdersNumbersSheet.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            OrdersNumbersSheet.SetMargin(HSSFSheet.LeftMargin, (double).12);
            OrdersNumbersSheet.SetMargin(HSSFSheet.RightMargin, (double).07);
            OrdersNumbersSheet.SetMargin(HSSFSheet.TopMargin, (double).20);
            OrdersNumbersSheet.SetMargin(HSSFSheet.BottomMargin, (double).20);
            OrdersNumbersSheet.SetColumnWidth(0, 12 * 256);
            OrdersNumbersSheet.SetColumnWidth(1, 55 * 256);
            OrdersNumbersSheet.SetColumnWidth(2, 12 * 256);
            OrdersNumbersSheet.SetColumnWidth(3, 12 * 256);

            HeaderFont2 = hssfworkbook2.CreateFont();
            HeaderFont2.FontHeightInPoints = 13;
            HeaderFont2.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            HeaderFont2.FontName = "Calibri";

            SimpleFont2 = hssfworkbook2.CreateFont();
            SimpleFont2.FontHeightInPoints = 12;
            SimpleFont2.FontName = "Calibri";

            TempFont2 = hssfworkbook2.CreateFont();
            TempFont2.FontHeightInPoints = 12;
            TempFont2.FontName = "Calibri";

            HeaderStyle2 = hssfworkbook2.CreateCellStyle();
            HeaderStyle2.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle2.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle2.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle2.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle2.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle2.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle2.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle2.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle2.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderStyle2.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderStyle2.WrapText = true;
            HeaderStyle2.SetFont(HeaderFont2);

            MainOrderCellStyle2 = hssfworkbook2.CreateCellStyle();
            MainOrderCellStyle2.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle2.BottomBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle2.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle2.LeftBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle2.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle2.RightBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle2.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainOrderCellStyle2.TopBorderColor = HSSFColor.BLACK.index;
            MainOrderCellStyle2.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainOrderCellStyle2.SetFont(SimpleFont2);

            TempStyle2 = hssfworkbook2.CreateCellStyle();
            TempStyle2.SetFont(TempFont2);
        }

        private static void ClearPackages()
        {
            PackagesDT.Clear();
        }

        private static void ClearOrdersDT()
        {
            OrdersDT.Clear();
        }

        public static ArrayList CurrentDispatches
        {
            get { return Dispatches; }
            set { Dispatches = value; }
        }

        public static string FileName
        {
            get { return sFileName; }
            set { sFileName = value; }
        }

        private static void Create()
        {
            SimpleResultDT = new DataTable();
            AttachResultDT = new DataTable();
            PackagesDT = new DataTable();
            OrdersDT = new DataTable();

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

        private static void FillPackages(int FactoryID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(
                @"SELECT MainOrders.MegaOrderID, MegaOrders.OrderNumber, Packages.ProductType, Packages.FactoryID, Packages.MainOrderID, Packages.PackageID, Packages.PackageStatusID
                FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                WHERE Packages.FactoryID=" + FactoryID + " AND DispatchID = " + CurrentDispatchID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }
        }

        private static void FillOrdersDT()
        {
            OrdersDT.Clear();
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(
                @"SELECT DISTINCT ClientName, MainOrders.MegaOrderID, MegaOrders.OrderNumber, Packages.FactoryID, Packages.MainOrderID
                FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE DispatchID = " + CurrentDispatchID + " ORDER BY ClientName, OrderNumber, MainOrderID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OrdersDT);
            }
        }

        public static void GetClientName(int DispatchID, ref int ClientID, ref string ClientName)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients" +
                    " WHERE ClientID=(SELECT ClientID FROM infiniu2_marketingorders.dbo.Dispatch WHERE DispatchID=" + DispatchID + ")",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        ClientID = Convert.ToInt32(DT.Rows[0]["ClientID"]);
                        ClientName = DT.Rows[0]["ClientName"].ToString();
                    }
                }
            }
        }

        public static string GetUserName(int UserID)
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

        public static string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
        }

        private static int[] GetOrderNumbers()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, "PackageStatusID=0", string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "OrderNumber" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["OrderNumber"]);
            DT.Dispose();
            return rows;
        }

        private static int[] GetOrderNumbers(int FactoryID)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, "PackageStatusID=0 AND FactoryID = " + FactoryID, string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "OrderNumber" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["OrderNumber"]);
            DT.Dispose();
            return rows;
        }

        private static DataTable GetOrdersID()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, "PackageStatusID=0", "MainOrderID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "MegaOrderID", "OrderNumber", "MainOrderID" });
            }

            return DT;
        }

        public static int[] GetMainOrders(int FactoryID)
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, "PackageStatusID=0 AND FactoryID=" + FactoryID, "MainOrderID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "MainOrderID" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            DT.Dispose();
            return rows;
        }

        private static int GetNotReadyPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            DataRow[] rows = PackagesDT.Select("MainOrderID = " + MainOrderID + " AND PackageStatusID = 0 AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID);
            return rows.Count();
        }

        private static int GetPackagesCount(int MainOrderID, int FactoryID, int ProductType)
        {
            DataRow[] rows = PackagesDT.Select("MainOrderID = " + MainOrderID + " AND ProductType = " + ProductType + " AND FactoryID = " + FactoryID);
            return rows.Count();
        }

        private static decimal GetSquare(int FactoryID, DateTime DispatchDate)
        {
            decimal Square = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, FrontsOrders.Count AS FrontsOrdersCount, FrontsOrders.Square, FrontsOrders.Weight FROM PackageDetails 
                    INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE PackageStatusID=0 AND ProductType = 0 AND FactoryID = " + FactoryID + " AND DispatchID = " + CurrentDispatchID + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
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

        private static decimal GetWeight(int FactoryID, DateTime DispatchDate)
        {
            decimal PackWeight = 0;
            decimal Weight = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, FrontsOrders.Count AS FrontsOrdersCount, FrontsOrders.Square, FrontsOrders.Weight FROM PackageDetails 
                    INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE PackageStatusID=0 AND ProductType = 0 AND FactoryID = " + FactoryID + " AND DispatchID = " + CurrentDispatchID + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
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
                    WHERE PackageID IN (SELECT PackageID FROM Packages WHERE PackageStatusID=0 AND ProductType = 1 AND FactoryID = " + FactoryID + " AND DispatchID = " + CurrentDispatchID + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
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

        private static bool Fill(int[] MainOrders, int FactoryID)
        {
            SimpleResultDT.Clear();

            PackagesCount.AllPackages = 0;
            PackagesCount.AllNotReadyPackages = 0;
            PackagesCount.ProfilPackages = 0;
            PackagesCount.ProfilNotReadyPackages = 0;
            PackagesCount.TPSPackages = 0;
            PackagesCount.TPSNotReadyPackages = 0;

            int MainOrderID = 0;
            for (int i = 0; i < MainOrders.Count(); i++)
            {
                int FrontsNotPackagesCount = 0;
                int DecorNotReadyPackagesCount = 0;
                int AllNotReadyPackagesCount = 0;

                int FrontsPackagesCount = 0;
                int DecorPackagesCount = 0;
                int AllPackagesCount = 0;

                MainOrderID = MainOrders[i];

                FrontsNotPackagesCount = GetNotReadyPackagesCount(MainOrders[i], FactoryID, 0);
                DecorNotReadyPackagesCount = GetNotReadyPackagesCount(MainOrders[i], FactoryID, 1);
                AllNotReadyPackagesCount = FrontsNotPackagesCount + DecorNotReadyPackagesCount;

                PackagesCount.AllNotReadyPackages += AllNotReadyPackagesCount;
                PackagesCount.ProfilNotReadyPackages += FrontsNotPackagesCount + DecorNotReadyPackagesCount;
                PackagesCount.TPSNotReadyPackages += FrontsNotPackagesCount + DecorNotReadyPackagesCount;

                FrontsPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 0);
                DecorPackagesCount = GetPackagesCount(MainOrders[i], FactoryID, 1);
                AllPackagesCount = FrontsPackagesCount + DecorPackagesCount;

                PackagesCount.AllPackages += AllPackagesCount;
                PackagesCount.ProfilPackages += FrontsPackagesCount + DecorPackagesCount;
                PackagesCount.TPSPackages += FrontsPackagesCount + DecorPackagesCount;

                DataRow NewRow = SimpleResultDT.NewRow();
                NewRow["MainOrder"] = "Подзаказ №" + MainOrders[i];
                if (FrontsPackagesCount > 0)
                    NewRow["FrontsPackagesCount"] = FrontsNotPackagesCount.ToString() + " / " + FrontsPackagesCount.ToString();
                if (DecorPackagesCount > 0)
                    NewRow["DecorPackagesCount"] = DecorNotReadyPackagesCount.ToString() + " / " + DecorPackagesCount.ToString();

                NewRow["AllPackagesCount"] = AllNotReadyPackagesCount.ToString() + " / " + AllPackagesCount.ToString();
                SimpleResultDT.Rows.Add(NewRow);
            }

            return SimpleResultDT.Rows.Count > 0;
        }

        public static void CreateReport()
        {
            int ZOVProfilCommonSheetRowIndex = 0;
            int ZOVTPSCommonSheetRowIndex = 0;
            int ZOVProfilDetailsSheetRowIndex = 0;
            int ZOVTPSDetailsSheetRowIndex = 0;

            HSSFCell Cell = null;

            Cell = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(ZOVProfilCommonSheetRowIndex++), 1, "«УТВЕРЖДАЮ»");
            Cell.CellStyle = ConfirmStyle;
            Cell = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(ZOVProfilCommonSheetRowIndex++), 0,
                "Ведомость создана: " + Security.CurrentUserShortName + " " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            Cell.CellStyle = TempStyle1;
            Cell = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(ZOVProfilCommonSheetRowIndex++), 0, "Участок: ЗОВ-Профиль");
            Cell.CellStyle = TempStyle1;

            Cell = HSSFCellUtil.CreateCell(ZOVProfilDetailsSheet.CreateRow(ZOVProfilDetailsSheetRowIndex++), 3, "«УТВЕРЖДАЮ»");
            Cell.CellStyle = ConfirmStyle;
            Cell = HSSFCellUtil.CreateCell(ZOVProfilDetailsSheet.CreateRow(ZOVProfilDetailsSheetRowIndex++), 0,
                "Ведомость создана: " + Security.CurrentUserShortName + " " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            Cell.CellStyle = TempStyle1;
            Cell = HSSFCellUtil.CreateCell(ZOVProfilDetailsSheet.CreateRow(ZOVProfilDetailsSheetRowIndex++), 0, "Участок: ЗОВ-Профиль");
            Cell.CellStyle = TempStyle1;


            for (int i = 0; i < Dispatches.Count; i++)
            {
                CurrentDispatchID = Convert.ToInt32(Dispatches[i]);
                int ClientID = 0;
                string ClientName = string.Empty;
                GetClientName(Convert.ToInt32(Dispatches[i]), ref ClientID, ref ClientName);

                {
                    ClearPackages();
                    FillPackages(1);
                    DataTable OrdersID = GetOrdersID();

                    int[] ProfilMainOrders = GetMainOrders(1);

                    if (ProfilMainOrders.Count() > 0)
                    {
                        int[] OrderNumbers = GetOrderNumbers();
                        int[] ProfilOrderNumbers = GetOrderNumbers(1);

                        PackingReport = new NotReadyProductsDetails();
                        PackagesCount = new NotReadyPackagesCount();

                        if (Fill(ProfilMainOrders, 1))
                        {
                            Cell = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(ZOVProfilCommonSheetRowIndex++), 0, "Клиент: " + ClientName);
                            Cell.CellStyle = TempStyle1;

                            CreateZOVProfilCommonSheet(ref ZOVProfilCommonSheetRowIndex, ProfilMainOrders);
                            PackingReport.CreateReport(ref hssfworkbook1, ref ZOVProfilDetailsSheet, ref ZOVTPSDetailsSheet, ref ZOVProfilDetailsSheetRowIndex, ref ZOVTPSDetailsSheetRowIndex,
                                OrdersID, ClientID, CurrentDispatchID, 1, ClientName, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"));
                        }
                    }
                }

                {
                    ClearPackages();
                    FillPackages(2);
                    DataTable OrdersID = GetOrdersID();

                    int[] TPSMainOrders = GetMainOrders(2);


                    if (TPSMainOrders.Count() > 0)
                    {
                        int[] OrderNumbers = GetOrderNumbers();
                        int[] TPSOrderNumbers = GetOrderNumbers(2);

                        PackingReport = new NotReadyProductsDetails();
                        PackagesCount = new NotReadyPackagesCount();


                        ZOVTPSCommonSheet = hssfworkbook1.CreateSheet("ЗОВ-ТПС");
                        ZOVTPSCommonSheet.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                        ZOVTPSCommonSheet.SetMargin(HSSFSheet.LeftMargin, (double).12);
                        ZOVTPSCommonSheet.SetMargin(HSSFSheet.RightMargin, (double).07);
                        ZOVTPSCommonSheet.SetMargin(HSSFSheet.TopMargin, (double).20);
                        ZOVTPSCommonSheet.SetMargin(HSSFSheet.BottomMargin, (double).20);
                        ZOVTPSCommonSheet.SetColumnWidth(0, 55 * 256);
                        ZOVTPSCommonSheet.SetColumnWidth(1, 12 * 256);
                        ZOVTPSCommonSheet.SetColumnWidth(2, 12 * 256);
                        ZOVTPSCommonSheet.SetColumnWidth(3, 12 * 256);

                        ZOVTPSDetailsSheet = hssfworkbook1.CreateSheet("Ведомость ЗОВ-ТПС");
                        ZOVTPSDetailsSheet.PrintSetup.PaperSize = (short)PaperSizeType.A4;
                        ZOVTPSDetailsSheet.SetMargin(HSSFSheet.LeftMargin, (double).12);
                        ZOVTPSDetailsSheet.SetMargin(HSSFSheet.RightMargin, (double).07);
                        ZOVTPSDetailsSheet.SetMargin(HSSFSheet.TopMargin, (double).20);
                        ZOVTPSDetailsSheet.SetMargin(HSSFSheet.BottomMargin, (double).20);
                        int DisplayIndex = 0;
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 4 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 18 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 12 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 5 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 5 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 5 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 7 * 256);
                        ZOVTPSDetailsSheet.SetColumnWidth(DisplayIndex++, 9 * 256);

                        Cell = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(ZOVTPSCommonSheetRowIndex++), 1, "«УТВЕРЖДАЮ»");
                        Cell.CellStyle = ConfirmStyle;
                        Cell = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(ZOVTPSCommonSheetRowIndex++), 0,
                            "Ведомость создана: " + Security.CurrentUserShortName + " " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
                        Cell.CellStyle = TempStyle1;
                        Cell = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(ZOVTPSCommonSheetRowIndex++), 0, "Участок: ЗОВ-ТПС");
                        Cell.CellStyle = TempStyle1;


                        Cell = HSSFCellUtil.CreateCell(ZOVTPSDetailsSheet.CreateRow(ZOVTPSDetailsSheetRowIndex++), 3, "«УТВЕРЖДАЮ»");
                        Cell.CellStyle = ConfirmStyle;
                        Cell = HSSFCellUtil.CreateCell(ZOVTPSDetailsSheet.CreateRow(ZOVTPSDetailsSheetRowIndex++), 0,
                            "Ведомость создана: " + Security.CurrentUserShortName + " " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
                        Cell.CellStyle = TempStyle1;
                        Cell = HSSFCellUtil.CreateCell(ZOVTPSDetailsSheet.CreateRow(ZOVTPSDetailsSheetRowIndex++), 0, "Участок: ЗОВ-ТПС");
                        Cell.CellStyle = TempStyle1;


                        if (Fill(TPSMainOrders, 2))
                        {
                            Cell = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(ZOVTPSCommonSheetRowIndex++), 0, "Клиент: " + ClientName);
                            Cell.CellStyle = TempStyle1;

                            CreateZOVTPSCommonSheet(ref ZOVTPSCommonSheetRowIndex, TPSMainOrders);
                            PackingReport.CreateReport(ref hssfworkbook1, ref ZOVProfilDetailsSheet, ref ZOVTPSDetailsSheet, ref ZOVProfilDetailsSheetRowIndex, ref ZOVTPSDetailsSheetRowIndex,
                                OrdersID, ClientID, CurrentDispatchID, 2, ClientName, Convert.ToDateTime(PrepareDispatchDateTime).ToString("dd.MM.yyyy"));
                        }
                    }

                }
            }
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook1.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public static void CreateOrdersNumbersReport()
        {
            int RowIndex = 0;
            for (int i = 0; i < Dispatches.Count; i++)
            {
                CurrentDispatchID = Convert.ToInt32(Dispatches[i]);
                ClearOrdersDT();
                FillOrdersDT();
                HSSFCell cell = HSSFCellUtil.CreateCell(OrdersNumbersSheet.CreateRow(0), 0, "№ отгр.");
                cell.CellStyle = HeaderStyle2;
                cell = HSSFCellUtil.CreateCell(OrdersNumbersSheet.CreateRow(0), 1, "Клиент");
                cell.CellStyle = HeaderStyle2;
                cell = HSSFCellUtil.CreateCell(OrdersNumbersSheet.CreateRow(0), 2, "№ заказа");
                cell.CellStyle = HeaderStyle2;
                cell = HSSFCellUtil.CreateCell(OrdersNumbersSheet.CreateRow(0), 3, "№ подзаказа");
                cell.CellStyle = HeaderStyle2;
                RowIndex++;
                CreateOrdersNumbersSheet(ref RowIndex);
            }
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook2.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public static void GetDispatchInfo(ref object date1)
        {
            PrepareDispatchDateTime = date1;
        }

        private static void CreateZOVTPSCommonSheet(ref int RowIndex, int[] MainOrders)
        {
            int TopRowFront = 1;
            int BottomRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            string Notes = string.Empty;

            HSSFCell cell1 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle1;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle1;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle1;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle1;

            TopRowFront = RowIndex;
            BottomRowFront = SimpleResultDT.Rows.Count + RowIndex;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));

                for (int y = 0; y < SimpleResultDT.Columns.Count; y++)
                {
                    if (Notes.Length > 0)
                    {
                        if (AttachResultDT.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = ZOVTPSCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = ZOVTPSCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = ZOVTPSCommonSheet.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = ZOVTPSCommonSheet.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (SimpleResultDT.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = ZOVTPSCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = ZOVTPSCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = ZOVTPSCommonSheet.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = ZOVTPSCommonSheet.CreateRow(RowIndex + 2).CreateCell(0);
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

            Weight = GetWeight(2, Convert.ToDateTime(PrepareDispatchDateTime));
            TotalFrontsSquare = GetSquare(2, Convert.ToDateTime(PrepareDispatchDateTime));

            HSSFCell cell13 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle1;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle1;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(ZOVTPSCommonSheet.CreateRow(RowIndex++), 0, "Упаковок: " + PackagesCount.TPSNotReadyPackages + "/" + PackagesCount.TPSPackages);
            cell16.CellStyle = TempStyle1;
            RowIndex++;
            RowIndex++;
        }

        private static void CreateZOVProfilCommonSheet(ref int RowIndex, int[] MainOrders)
        {
            int TopRowFront = 1;
            int BottomRowFront = 1;

            decimal Weight = 0;
            decimal TotalFrontsSquare = 0;

            string Notes = string.Empty;

            HSSFCell cell1 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex), 0, "Клиент/Заказ");
            cell1.CellStyle = HeaderStyle1;
            HSSFCell cell2 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex), 1, "Кол-во упаковок, фасады");
            cell2.CellStyle = HeaderStyle1;
            HSSFCell cell3 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex), 2, "Кол-во упаковок, декор");
            cell3.CellStyle = HeaderStyle1;
            HSSFCell cell4 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex), 3, "Кол-во упаковок, общее");
            cell4.CellStyle = HeaderStyle1;

            TopRowFront = RowIndex;
            BottomRowFront = SimpleResultDT.Rows.Count + RowIndex;

            for (int i = 0; i < MainOrders.Count(); i++)
            {
                Notes = GetMainOrderNotes(Convert.ToInt32(MainOrders[i]));

                for (int y = 0; y < SimpleResultDT.Columns.Count; y++)
                {
                    if (Notes.Length > 0)
                    {
                        if (AttachResultDT.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = ZOVProfilCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle1;//нижняя линия не рисуется
                        }
                        else
                        {
                            HSSFCell cell = ZOVProfilCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle1;
                        }


                        if (Notes.Length > 0)
                        {
                            HSSFCell EmptyCell = ZOVProfilCommonSheet.CreateRow(RowIndex + 2).CreateCell(y);
                            EmptyCell.CellStyle = EmptyCellStyle;

                            HSSFCell cell = ZOVProfilCommonSheet.CreateRow(RowIndex + 2).CreateCell(0);
                            cell.SetCellValue("Примечание: " + Notes);
                            cell.CellStyle = NotesCellStyle;
                        }
                    }

                    else
                    {
                        if (SimpleResultDT.Columns[y].ColumnName == "MainOrder")
                        {
                            HSSFCell cell = ZOVProfilCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = MainOrderCellStyle;
                        }
                        else
                        {
                            HSSFCell cell = ZOVProfilCommonSheet.CreateRow(RowIndex + 1).CreateCell(y);
                            cell.SetCellValue(SimpleResultDT.Rows[i][y].ToString());

                            cell.CellStyle = SimpleCellStyle;
                        }
                    }

                    if (Notes.Length > 0)
                    {
                        HSSFCell EmptyCell = ZOVProfilCommonSheet.CreateRow(RowIndex + 2).CreateCell(y);
                        EmptyCell.CellStyle = EmptyCellStyle;

                        HSSFCell cell = ZOVProfilCommonSheet.CreateRow(RowIndex + 2).CreateCell(0);
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

            Weight = GetWeight(1, Convert.ToDateTime(PrepareDispatchDateTime));
            TotalFrontsSquare = GetSquare(1, Convert.ToDateTime(PrepareDispatchDateTime));

            HSSFCell cell13 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex++), 0, "Итого:");
            cell13.CellStyle = TotalStyle;

            HSSFCell cell14 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex++), 0, "Площадь: " + TotalFrontsSquare + " м.кв.");
            cell14.CellStyle = TempStyle1;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex++), 0, "Вес: " + Weight + " кг");
            cell15.CellStyle = TempStyle1;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(ZOVProfilCommonSheet.CreateRow(RowIndex++), 0, "Упаковок: " + PackagesCount.ProfilNotReadyPackages + "/" + PackagesCount.ProfilPackages);
            cell16.CellStyle = TempStyle1;
            RowIndex++;
            RowIndex++;
        }

        private static void CreateOrdersNumbersSheet(ref int RowIndex)
        {
            HSSFRow r = OrdersNumbersSheet.CreateRow(RowIndex);
            HSSFCell cell = r.CreateCell(0);
            cell.SetCellValue(CurrentDispatchID);
            cell.CellStyle = TempStyle2;
            int DisplayIndex = 0;
            for (int y = 0; y < OrdersDT.Rows.Count; y++)
            {
                r = OrdersNumbersSheet.CreateRow(RowIndex);
                DisplayIndex = 1;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(OrdersDT.Rows[y]["ClientName"].ToString());
                cell.CellStyle = MainOrderCellStyle2;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(Convert.ToInt32(OrdersDT.Rows[y]["OrderNumber"]));
                cell.CellStyle = MainOrderCellStyle2;
                cell = r.CreateCell(DisplayIndex++);
                cell.SetCellValue(Convert.ToInt32(OrdersDT.Rows[y]["MainOrderID"]));
                cell.CellStyle = MainOrderCellStyle2;
                RowIndex++;
            }
            RowIndex++;
        }
    }
}