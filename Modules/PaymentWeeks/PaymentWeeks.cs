using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace Infinium.Modules.PaymentWeeks
{



    public class ZOVDebts
    {
        DataTable DebtsDetailsDT = null;
        DataTable DoNotDispatchDetailsDT = null;

        public ZOVDebts()
        {

        }


        private void Create()
        {
            DebtsDetailsDT = new DataTable();
            DoNotDispatchDetailsDT = new DataTable();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 
            dbo.MegaOrders.DispatchDate, ClientName, DocNumber, SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Cost,
            dbo.MainOrders.DebtTypeID, dbo.FrontsOrders.MainOrderID
            FROM dbo.PackageDetails INNER JOIN
            dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
            dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND Packages.PackageStatusID <> 3 AND
            dbo.Packages.ProductType = 0 INNER JOIN
            dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DebtTypeID <> 0 INNER JOIN
            infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
            dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID AND dbo.MainOrders.MegaOrderID <> 0
            GROUP BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber, dbo.FrontsOrders.MainOrderID, dbo.MainOrders.DebtTypeID, Packages.PackageStatusID
            ORDER BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DebtsDetailsDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 
            dbo.MegaOrders.DispatchDate, ClientName, DocNumber, SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Cost,
            dbo.MainOrders.DebtTypeID, dbo.FrontsOrders.MainOrderID
            FROM dbo.PackageDetails INNER JOIN
            dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
            dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND Packages.PackageStatusID <> 3 AND
            dbo.Packages.ProductType = 0 INNER JOIN
            dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DoNotDispatch = 1 INNER JOIN
            infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
            dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID AND dbo.MainOrders.MegaOrderID <> 0
            GROUP BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber, dbo.FrontsOrders.MainOrderID, dbo.MainOrders.DebtTypeID
            ORDER BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DoNotDispatchDetailsDT);
            }
        }

        public void Initialize()
        {
            Create();
            Fill();
        }

        public void Load(ref decimal DoNotDispatchCost, ref decimal DoNotDispatchCostAll, ref decimal DebtsCost, ref decimal DebtsCostAll,
            DateTime date1, DateTime date2)
        {
            DebtsDetailsDT.Clear();
            DoNotDispatchDetailsDT.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT 
            dbo.MegaOrders.DispatchDate, ClientName, DocNumber, SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Cost,
            dbo.MainOrders.DebtTypeID, dbo.FrontsOrders.MainOrderID, Packages.PackageStatusID
            FROM dbo.PackageDetails INNER JOIN
            dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
            dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND
            dbo.Packages.ProductType = 0 INNER JOIN
            dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DebtTypeID <> 0 INNER JOIN
            infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
            dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID AND dbo.MainOrders.MegaOrderID <> 0
            AND CAST(MegaOrders.DispatchDate AS date) > '" + date1.ToString("yyyy-MM-dd") + "' AND CAST(MegaOrders.DispatchDate AS date) < '" + date2.ToString("yyyy-MM-dd") + @"'
            GROUP BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber, dbo.FrontsOrders.MainOrderID, dbo.MainOrders.DebtTypeID, Packages.PackageStatusID
            ORDER BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DebtsCostAll += Convert.ToDecimal(DT.Rows[i]["Cost"]);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT 
            dbo.MegaOrders.DispatchDate, ClientName, DocNumber, SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Cost,
            dbo.MainOrders.DebtTypeID, dbo.FrontsOrders.MainOrderID, Packages.PackageStatusID
            FROM dbo.PackageDetails INNER JOIN
            dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
            dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND
            dbo.Packages.ProductType = 0 INNER JOIN
            dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DoNotDispatch = 1 INNER JOIN
            infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
            dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID AND dbo.MainOrders.MegaOrderID <> 0
            AND CAST(MegaOrders.DispatchDate AS date) > '" + date1.ToString("yyyy-MM-dd") + "' AND CAST(MegaOrders.DispatchDate AS date) < '" + date2.ToString("yyyy-MM-dd") + @"'
            GROUP BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber, dbo.FrontsOrders.MainOrderID, dbo.MainOrders.DebtTypeID, Packages.PackageStatusID
            ORDER BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DoNotDispatchCostAll += Convert.ToDecimal(DT.Rows[i]["Cost"]);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT 
            dbo.MegaOrders.DispatchDate, ClientName, DocNumber, SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Cost,
            dbo.MainOrders.DebtTypeID, dbo.FrontsOrders.MainOrderID
            FROM dbo.PackageDetails INNER JOIN
            dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
            dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND Packages.PackageStatusID <> 3 AND
            dbo.Packages.ProductType = 0 INNER JOIN
            dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DebtTypeID <> 0 INNER JOIN
            infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
            dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID AND dbo.MainOrders.MegaOrderID <> 0
            AND CAST(MegaOrders.DispatchDate AS date) > '" + date1.ToString("yyyy-MM-dd") + "' AND CAST(MegaOrders.DispatchDate AS date) < '" + date2.ToString("yyyy-MM-dd") + @"'
            GROUP BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber, dbo.FrontsOrders.MainOrderID, dbo.MainOrders.DebtTypeID
            ORDER BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DebtsDetailsDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT 
            dbo.MegaOrders.DispatchDate, ClientName, DocNumber, SUM(dbo.PackageDetails.Count * dbo.FrontsOrders.Cost / dbo.FrontsOrders.Count) AS Cost,
            dbo.MainOrders.DebtTypeID, dbo.FrontsOrders.MainOrderID
            FROM dbo.PackageDetails INNER JOIN
            dbo.FrontsOrders ON dbo.PackageDetails.OrderID = dbo.FrontsOrders.FrontsOrdersID INNER JOIN
            dbo.Packages ON dbo.PackageDetails.PackageID = dbo.Packages.PackageID AND Packages.PackageStatusID <> 3 AND
            dbo.Packages.ProductType = 0 INNER JOIN
            dbo.MainOrders ON dbo.Packages.MainOrderID = dbo.MainOrders.MainOrderID AND dbo.MainOrders.DoNotDispatch = 1 INNER JOIN
            infiniu2_zovreference.dbo.Clients ON dbo.MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID INNER JOIN
            dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID AND dbo.MainOrders.MegaOrderID <> 0
            AND CAST(MegaOrders.DispatchDate AS date) > '" + date1.ToString("yyyy-MM-dd") + "' AND CAST(MegaOrders.DispatchDate AS date) < '" + date2.ToString("yyyy-MM-dd") + @"'
            GROUP BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber, dbo.FrontsOrders.MainOrderID, dbo.MainOrders.DebtTypeID
            ORDER BY dbo.MegaOrders.DispatchDate, ClientName, DocNumber", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DoNotDispatchDetailsDT);
            }

            int DebtTypeID = 0;

            for (int i = 0; i < DebtsDetailsDT.Rows.Count; i++)
            {
                DebtTypeID = Convert.ToInt32(DebtsDetailsDT.Rows[i]["DebtTypeID"]);
                if (DebtTypeID == 1 || DebtTypeID == 2 || DebtTypeID == 3)
                {
                    DebtsCost += Convert.ToDecimal(DebtsDetailsDT.Rows[i]["Cost"]);
                }
            }
            for (int i = 0; i < DoNotDispatchDetailsDT.Rows.Count; i++)
            {
                DoNotDispatchCost += Convert.ToDecimal(DoNotDispatchDetailsDT.Rows[i]["Cost"]);
            }
        }

        public DataView DebtsList
        {
            get
            {
                DataView DV = new DataView(DebtsDetailsDT);
                //DV.RowFilter = "PackageStatusID <> 3";
                return DV;
            }
        }

        public DataView DoNotDispatchDetailsList
        {
            get
            {
                DataView DV = new DataView(DoNotDispatchDetailsDT);
                //DV.RowFilter = "PackageStatusID <> 3";
                return DV;
            }
        }
    }





    public class PaymentWeeks
    {
        DataTable PaymentWeeksDataTable = null;
        DataTable PaymentDetailDataTable = null;

        DataTable ResultTotalDataTable = null;
        DataTable WriteOffDataTable = null;
        DataTable CalcWriteOffDataTable = null;
        DataTable DebtTypesDataTable = null;

        public BindingSource ResultTotalBindingSource = null;
        public BindingSource WriteOffBindingSource = null;
        public BindingSource CalcWriteOffBindingSource = null;
        public BindingSource PaymentDetailBindingSource = null;
        public BindingSource DebtTypesBindingSource = null;

        PercentageDataGrid ResultTotalDataGrid = null;
        PercentageDataGrid WriteOffDataGrid = null;
        PercentageDataGrid CalcWriteOffDataGrid = null;
        PercentageDataGrid WriteOffDetailDataGrid = null;

        Label ResultLabel;
        Label CalcWriteOffResultLabel;
        Label WriteOffResultLabel;

        private DataGridViewComboBoxColumn DebtTypeColumn = null;

        public PaymentWeeks(ref PercentageDataGrid tResultTotalDataGrid, ref PercentageDataGrid tWriteOffDataGrid,
                            ref PercentageDataGrid tCalcWriteOffDataGrid, ref PercentageDataGrid tWriteOffDetailDataGrid, ref Label tResultLabel,
                            ref Label tCalcWriteOffResultLabel, ref Label tWriteOffResultLabel)
        {
            ResultTotalDataGrid = tResultTotalDataGrid;
            CalcWriteOffDataGrid = tCalcWriteOffDataGrid;
            WriteOffDataGrid = tWriteOffDataGrid;
            WriteOffDetailDataGrid = tWriteOffDetailDataGrid;

            ResultLabel = tResultLabel;
            CalcWriteOffResultLabel = tCalcWriteOffResultLabel;
            WriteOffResultLabel = tWriteOffResultLabel;

            Initialize();
        }


        private void Create()
        {
            PaymentWeeksDataTable = new DataTable();
            PaymentDetailDataTable = new DataTable();

            ResultTotalDataTable = new DataTable();
            ResultTotalDataTable.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            ResultTotalDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            WriteOffDataTable = new DataTable();
            WriteOffDataTable.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            WriteOffDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            CalcWriteOffDataTable = new DataTable();
            CalcWriteOffDataTable.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            CalcWriteOffDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            DebtTypesDataTable = new DataTable();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DebtTypes", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(DebtTypesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM PaymentDetail", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PaymentDetailDataTable);
            }
        }

        private void Binding()
        {
            ResultTotalBindingSource = new BindingSource()
            {
                DataSource = ResultTotalDataTable
            };
            ResultTotalDataGrid.DataSource = ResultTotalBindingSource;

            WriteOffBindingSource = new BindingSource()
            {
                DataSource = WriteOffDataTable
            };
            WriteOffDataGrid.DataSource = WriteOffBindingSource;

            CalcWriteOffBindingSource = new BindingSource()
            {
                DataSource = CalcWriteOffDataTable
            };
            CalcWriteOffDataGrid.DataSource = CalcWriteOffBindingSource;

            PaymentDetailBindingSource = new BindingSource()
            {
                DataSource = PaymentDetailDataTable
            };
            WriteOffDetailDataGrid.DataSource = PaymentDetailBindingSource;

            DebtTypesBindingSource = new BindingSource()
            {
                DataSource = DebtTypesDataTable
            };
        }

        private void CreateColumns()
        {
            //создание столбцов
            DebtTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DebtTypeColumn",
                HeaderText = "Тип долга",
                DataPropertyName = "DebtTypeID",
                DataSource = DebtTypesBindingSource,
                ValueMember = "DebtTypeID",
                DisplayMember = "DebtType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            //добавление столбцов

            WriteOffDetailDataGrid.Columns.Add(DebtTypeColumn);
        }

        private void SetGrids()
        {
            //SelectDateDataGrid.Columns["PaymentWeekID"].Visible = false;
            //SelectDateDataGrid.Columns["Period"].HeaderText = "Расчетная неделя";

            //SelectDateDataGrid.Columns["Period"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //SelectDateDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            foreach (DataGridViewColumn Column in WriteOffDetailDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            ResultTotalDataGrid.Columns["Cost"].DefaultCellStyle.Format = "C";
            ResultTotalDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            ResultTotalDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ResultTotalDataGrid.Columns["Cost"].Width = 150;

            ResultTotalDataGrid.Columns["Parameter"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
            ResultTotalDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;


            WriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Format = "C";
            WriteOffDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            WriteOffDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WriteOffDataGrid.Columns["Cost"].Width = 150;

            WriteOffDataGrid.Columns["Parameter"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
            WriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;



            CalcWriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Format = "C";
            CalcWriteOffDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            CalcWriteOffDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            CalcWriteOffDataGrid.Columns["Cost"].Width = 150;

            CalcWriteOffDataGrid.Columns["Parameter"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
            CalcWriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;


            WriteOffDetailDataGrid.Columns["PaymentDetailID"].Visible = false;
            WriteOffDetailDataGrid.Columns["PaymentWeekID"].Visible = false;
            WriteOffDetailDataGrid.Columns["DebtTypeID"].Visible = false;

            WriteOffDetailDataGrid.Columns["DispatchDate"].HeaderText = "Дата отгрузки";
            WriteOffDetailDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            WriteOffDetailDataGrid.Columns["DocNumber"].HeaderText = "№ документа";
            WriteOffDetailDataGrid.Columns["SamplesWriteOffCost"].HeaderText = "Образцы";
            WriteOffDetailDataGrid.Columns["DebtCost"].HeaderText = "Долги";

            WriteOffDetailDataGrid.Columns["DispatchDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WriteOffDetailDataGrid.Columns["DispatchDate"].Width = 150;
            WriteOffDetailDataGrid.Columns["SamplesWriteOffCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WriteOffDetailDataGrid.Columns["SamplesWriteOffCost"].Width = 150;
            WriteOffDetailDataGrid.Columns["DebtCost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WriteOffDetailDataGrid.Columns["DebtCost"].Width = 150;
            WriteOffDetailDataGrid.Columns["DebtTypeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WriteOffDetailDataGrid.Columns["DebtTypeColumn"].Width = 190;

            WriteOffDetailDataGrid.AutoGenerateColumns = false;

            WriteOffDetailDataGrid.Columns["DispatchDate"].DisplayIndex = 0;
            WriteOffDetailDataGrid.Columns["ClientName"].DisplayIndex = 1;
            WriteOffDetailDataGrid.Columns["DocNumber"].DisplayIndex = 2;
            WriteOffDetailDataGrid.Columns["SamplesWriteOffCost"].DisplayIndex = 3;
            WriteOffDetailDataGrid.Columns["DebtCost"].DisplayIndex = 4;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            SetGrids();
        }


        private void AddResultRow(String Parameter, object Cost)
        {
            {
                DataRow NewRow = ResultTotalDataTable.NewRow();
                NewRow["Parameter"] = Parameter;
                NewRow["Cost"] = Cost;
                ResultTotalDataTable.Rows.Add(NewRow);
            }
        }

        private void AddWriteOffRow(String Parameter, object Cost)
        {
            {
                DataRow NewRow = WriteOffDataTable.NewRow();
                NewRow["Parameter"] = Parameter;
                NewRow["Cost"] = Cost;
                WriteOffDataTable.Rows.Add(NewRow);
            }
        }

        private void AddCalcWriteOffRow(String Parameter, object Cost)
        {
            {
                DataRow NewRow = CalcWriteOffDataTable.NewRow();
                NewRow["Parameter"] = Parameter;
                NewRow["Cost"] = Cost;
                CalcWriteOffDataTable.Rows.Add(NewRow);
            }
        }

        public void Load(int PaymentWeekID)
        {
            PaymentDetailDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PaymentDetail WHERE PaymentWeekID = " + PaymentWeekID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PaymentDetailDataTable);
            }

            PaymentWeeksDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PaymentWeeks WHERE PaymentWeekID = " + PaymentWeekID, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PaymentWeeksDataTable);

                decimal TotalWriteOff = Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["TotalCalcWriteOffCost"]) +
                                              Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["TotalWriteOffCost"]) +
                                              ((Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["ErrorWriteOffCost"]) -
                                                Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["CompensationCost"])));

                AddResultRow("Стоимость заказов", PaymentWeeksDataTable.Rows[0]["TotalCost"]);
                AddResultRow("Отгружено", PaymentWeeksDataTable.Rows[0]["DispatchedCost"]);
                AddResultRow("Не отгружено", PaymentWeeksDataTable.Rows[0]["DispatchedDebtCost"]);
                AddResultRow("Списано", TotalWriteOff);
                ResultLabel.Text = "Итого: " + PaymentWeeksDataTable.Rows[0]["ProfitCost"].ToString();



                AddCalcWriteOffRow("Долги", PaymentWeeksDataTable.Rows[0]["CalcDebtCost"]);
                AddCalcWriteOffRow("Браки", PaymentWeeksDataTable.Rows[0]["CalcDefectsCost"]);
                AddCalcWriteOffRow("Ошибки пр-ва", PaymentWeeksDataTable.Rows[0]["CalcProductionErrorsCost"]);
                AddCalcWriteOffRow("Ошибки ЗОВа", PaymentWeeksDataTable.Rows[0]["CalcZOVErrorsCost"]);
                CalcWriteOffResultLabel.Text = "Итого списано: " + PaymentWeeksDataTable.Rows[0]["TotalCalcWriteOffCost"].ToString();

                AddWriteOffRow("Долги", PaymentWeeksDataTable.Rows[0]["WriteOffDebtCost"]);
                AddWriteOffRow("Браки", PaymentWeeksDataTable.Rows[0]["WriteOffDefectsCost"]);
                AddWriteOffRow("Ошибки пр-ва", PaymentWeeksDataTable.Rows[0]["WriteOffProductionErrorsCost"]);
                AddWriteOffRow("Ошибки ЗОВа", PaymentWeeksDataTable.Rows[0]["WriteOffZOVErrorsCost"]);
                AddWriteOffRow("Образцы", PaymentWeeksDataTable.Rows[0]["SamplesWriteOffCost"]);
                AddWriteOffRow("Ошибка списания", PaymentWeeksDataTable.Rows[0]["ErrorWriteOffCost"]);
                AddWriteOffRow("Компенсация", PaymentWeeksDataTable.Rows[0]["CompensationCost"]);

                WriteOffResultLabel.Text = "Итого списано: " + (Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["TotalWriteOffCost"]) +
                    (Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["ErrorWriteOffCost"]) -
                            Convert.ToDecimal(PaymentWeeksDataTable.Rows[0]["CompensationCost"]))).ToString();
            }
        }
    }






    public class SelectPaymentWeek
    {
        public DataTable SelectPaymentWeekDataTable = null;

        public BindingSource SelectPaymentWeekBindingSource = null;

        PercentageDataGrid SelectDateDataGrid = null;

        public SelectPaymentWeek(ref PercentageDataGrid tSelectDateDataGrid)
        {
            SelectDateDataGrid = tSelectDateDataGrid;

            Initialize();
        }

        private void Create()
        {
            SelectPaymentWeekDataTable = new DataTable();
            SelectPaymentWeekDataTable.Columns.Add(new DataColumn("Period", Type.GetType("System.String")));
            SelectPaymentWeekDataTable.Columns.Add(new DataColumn("PaymentWeekID", Type.GetType("System.Int32")));
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PaymentWeekID, DateFrom, DateTo FROM PaymentWeeks", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        DataRow NewRow = SelectPaymentWeekDataTable.NewRow();
                        NewRow["Period"] = Convert.ToDateTime(Row["DateFrom"]).ToString("dd.MM.yyyy") + " - " + Convert.ToDateTime(Row["DateTo"]).ToString("dd.MM.yyyy");
                        NewRow["PaymentWeekID"] = Row["PaymentWeekID"];
                        SelectPaymentWeekDataTable.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void Binding()
        {
            SelectPaymentWeekBindingSource = new BindingSource()
            {
                DataSource = SelectPaymentWeekDataTable
            };
            SelectDateDataGrid.DataSource = SelectPaymentWeekBindingSource;
        }

        private void SetGrids()
        {
            SelectDateDataGrid.Columns["PaymentWeekID"].Visible = false;
            SelectDateDataGrid.Columns["Period"].HeaderText = "Расчетная неделя";

            SelectDateDataGrid.Columns["Period"].SortMode = DataGridViewColumnSortMode.NotSortable;
            SelectDateDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            SetGrids();
        }

        public int GetCurrentPaymentWeekID()
        {
            return Convert.ToInt32(((DataRowView)SelectPaymentWeekBindingSource.Current)["PaymentWeekID"]);
        }

        public string GetCurrentPeriod()
        {
            return ((DataRowView)SelectPaymentWeekBindingSource.Current)["Period"].ToString();
        }
    }
}
