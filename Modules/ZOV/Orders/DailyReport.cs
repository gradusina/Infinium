using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace Infinium.Modules.ZOV.DailyReport
{

    public class DailyReportFrontsOrdersReport
    {
        public DataTable MainTable = null;
        public DataTable ResultTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable FrontsDataTable = null;
        private DataTable FrontTypesDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable ColorsDataTable = null;
        private DataTable MeasuresDataTable = null;
        private DataTable InsetMarginsDataTable = null;

        public DailyReportFrontsOrdersReport(ref MainOrdersFrontsOrders MainOrdersFrontsOrders)
        {
            Create();

            FrontsDataTable = MainOrdersFrontsOrders.FrontsDataTable.Copy();
            FrontTypesDataTable = MainOrdersFrontsOrders.PatinaDataTable.Copy();
            InsetTypesDataTable = MainOrdersFrontsOrders.InsetTypesDataTable.Copy();
            ColorsDataTable = MainOrdersFrontsOrders.FrameColorsDataTable.Copy();
            MeasuresDataTable = MainOrdersFrontsOrders.MeasuresDataTable.Copy();
            InsetMarginsDataTable = MainOrdersFrontsOrders.InsetMarginsDataTable.Copy();

            CreateMainTable();
            CreateResultTable();
        }

        private void Create()
        {
            FrontsOrdersDataTable = new DataTable();
            FrontsDataTable = new DataTable();
            FrontTypesDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            ColorsDataTable = new DataTable();
            MeasuresDataTable = new DataTable();
            InsetMarginsDataTable = new DataTable();
        }

        private void CreateMainTable()
        {
            MainTable = new DataTable();
            MainTable.Columns.Add(new DataColumn(("FrontName"), System.Type.GetType("System.String")));
            MainTable.Columns.Add(new DataColumn(("FrontType"), System.Type.GetType("System.String")));
            MainTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            MainTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            MainTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            MainTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            MainTable.Columns.Add(new DataColumn(("NonStandard"), System.Type.GetType("System.Boolean")));
            MainTable.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            MainTable.Columns.Add(new DataColumn(("FrontPrice"), System.Type.GetType("System.Decimal")));
            MainTable.Columns.Add(new DataColumn(("InsetPrice"), System.Type.GetType("System.Decimal")));
            MainTable.Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.Decimal")));
            MainTable.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            MainTable.Columns.Add(new DataColumn(("InsetTypeID"), System.Type.GetType("System.Int32")));
            MainTable.Columns.Add(new DataColumn(("IsSample"), System.Type.GetType("System.Boolean")));
        }

        private void CreateResultTable()
        {
            ResultTable = new DataTable();
            ResultTable.Columns.Add(new DataColumn(("ItemName"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.String")));
        }

        public string GetFrontName(int FrontID)
        {
            DataRow[] Rows = FrontsDataTable.Select("FrontID = " + FrontID);
            return Rows[0]["FrontName"].ToString();
        }

        private string GetFrontTypeName(int FrontTypeID)
        {
            DataRow[] Rows = FrontTypesDataTable.Select("FrontTypeID = " + FrontTypeID);
            return Rows[0]["FrontType"].ToString();
        }

        public string GetInsetTypeName(int InsetTypeID)
        {
            DataRow[] Rows = InsetTypesDataTable.Select("InsetTypeID = " + InsetTypeID);
            return Rows[0]["InsetTypeName"].ToString();
        }

        public string GetColorName(int ColorID)
        {
            DataRow[] Rows = ColorsDataTable.Select("ColorID = " + ColorID);
            return Rows[0]["ColorName"].ToString();
        }

        private void CopyMainTable()
        {
            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = MainTable.NewRow();
                string FrontName = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                NewRow["FrontName"] = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                if (Convert.ToInt32(Row["Width"]) != -1)
                    NewRow["FrontType"] = "прямой";
                else
                    NewRow["FrontType"] = "гнутый";
                NewRow["InsetType"] = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                NewRow["FrontID"] = Convert.ToInt32(Row["FrontID"]);
                NewRow["InsetTypeID"] = Convert.ToInt32(Row["InsetTypeID"]);
                NewRow["Height"] = Convert.ToInt32(Row["Height"]);
                NewRow["Width"] = Convert.ToInt32(Row["Width"]);
                NewRow["Count"] = Convert.ToInt32(Row["Count"]);
                NewRow["NonStandard"] = Convert.ToBoolean(Row["IsNonStandard"]);
                NewRow["IsSample"] = Convert.ToBoolean(Row["IsSample"]);
                NewRow["Square"] = Convert.ToDecimal(Row["Square"]);
                NewRow["FrontPrice"] = Convert.ToDecimal(Row["FrontPrice"]);
                NewRow["InsetPrice"] = Convert.ToDecimal(Row["InsetPrice"]);
                NewRow["Cost"] = Convert.ToDecimal(Row["Cost"]);
                MainTable.Rows.Add(NewRow);
            }
        }

        private void GetCurvedFronts()
        {
            DataTable DT = new DataTable();

            DT = MainTable.Clone();

            foreach (DataRow Row in MainTable.Select("FrontType = 'гнутый'"))
            {
                DT.ImportRow(Row);
            }

            foreach (DataRow Row in DT.Rows)
            {
                DataRow NewRow = ResultTable.NewRow();
                NewRow["ItemName"] = Row["FrontName"].ToString() + " гнут. " + Row["Height"].ToString();
                NewRow["Count"] = Row["Count"];
                NewRow["Measure"] = "шт.";
                NewRow["Price"] = Row["FrontPrice"];
                NewRow["Cost"] = Row["Cost"];
                ResultTable.Rows.Add(NewRow);
            }

            DT.Dispose();
        }

        private void GetStandard()
        {
            DataTable DT = new DataTable();

            DT = MainTable.Clone();

            foreach (DataRow Row in MainTable.Select("FrontType <> 'гнутый'"))
            {
                DT.ImportRow(Row);
            }

            foreach (DataRow Row in DT.Rows)
            {
                DataRow NewRow = ResultTable.NewRow();
                if (Row["NonStandard"].ToString() == "True")
                    NewRow["ItemName"] = Row["FrontName"].ToString() + " нестандарт";
                else
                    NewRow["ItemName"] = Row["FrontName"].ToString() + " стандарт";
                NewRow["Count"] = Row["Square"];
                NewRow["Measure"] = "м.кв.";

                int FrontID = Convert.ToInt32(Row["FrontID"]);
                int InsetTypeID = Convert.ToInt32(Row["InsetTypeID"]);
                bool IsSample = Convert.ToBoolean(Row["IsSample"]);

                if (FrontID == 3728 || FrontID == 3731 ||
                    FrontID == 3732 || FrontID == 3739 ||
                    FrontID == 3740 || FrontID == 3741 ||
                    FrontID == 3744 || FrontID == 3745 ||
                    FrontID == 3746 ||
                    InsetTypeID == 28961 || InsetTypeID == 3653 || InsetTypeID == 3654 || InsetTypeID == 3655
                    )
                {
                    decimal FrontPrice = Convert.ToDecimal(Row["FrontPrice"]);
                    decimal Cost = Convert.ToDecimal(Row["Cost"]);
                    decimal Square = Convert.ToDecimal(Row["Square"]);
                    int Count = Convert.ToInt32(Row["Count"]);
                    Cost = ((FrontPrice * Square / Count) + 5) * Count;
                    if (IsSample)
                        Cost = ((FrontPrice * Square / Count) + 2.5m) * Count;
                    NewRow["Price"] = FrontPrice;
                    NewRow["Cost"] = Cost;
                }
                else
                {
                    NewRow["Price"] = Row["FrontPrice"];
                    if (Row["FrontName"].ToString()[0] != 'Z')
                        NewRow["Cost"] = Convert.ToDecimal(Row["Square"]) * Convert.ToDecimal(Row["FrontPrice"]);
                    else
                        NewRow["Cost"] = Row["Cost"];
                }



                ResultTable.Rows.Add(NewRow);
            }

            DT.Dispose();
        }

        private void GetInsets()
        {
            DataTable DT = new DataTable();

            DT = MainTable.Clone();

            foreach (DataRow Row in MainTable.Select(
@"FrontType <> 'гнутый' AND (InsetTypeID IN (2,685,687,688,29470,29471,686))"))
            {
                DT.ImportRow(Row);
            }



            foreach (DataRow Row in DT.Rows)
            {
                string FrontName = Row["FrontName"].ToString();
                if (Row["FrontName"].ToString() == "Z-1 AxBxC" || Row["FrontName"].ToString() == "Z-3 AxBxC" || Row["FrontName"].ToString() == "Z-4 AxBxC" ||
                   Row["FrontName"].ToString() == "Z-7 AxBxC" || Row["FrontName"].ToString() == "Z-8 AxBxC" || Row["FrontName"].ToString() == "Z-10 AxBxC")
                    continue;


                int FrontID = Convert.ToInt32(Row["FrontID"]);
                int InsetTypeID = Convert.ToInt32(Row["InsetTypeID"]);
                int MID = Convert.ToInt32(InsetTypesDataTable.Select("InsetTypeName = '" + Row["InsetType"] + "'")[0]["MeasureID"]);

                if (FrontID == 3728 || FrontID == 3731 ||
                    FrontID == 3732 || FrontID == 3739 ||
                    FrontID == 3740 || FrontID == 3741 ||
                    FrontID == 3744 || FrontID == 3745 ||
                    FrontID == 3746 ||
                    InsetTypeID == 28961 || InsetTypeID == 3653 || InsetTypeID == 3654 || InsetTypeID == 3655
                    )
                {
                    MID = 3;
                }
                DataRow NewRow = ResultTable.NewRow();
                NewRow["ItemName"] = Row["InsetType"].ToString();
                if (MID == 3)
                    NewRow["Count"] = Row["Count"];
                if (MID == 1)
                {
                    decimal H = Convert.ToDecimal(Row["Height"]);
                    decimal W = Convert.ToDecimal(Row["Width"]);

                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStore WHERE TechStoreID = " + Convert.ToInt32(Row["FrontID"]),
                        ConnectionStrings.CatalogConnectionString))
                    {
                        using (DataTable DT1 = new DataTable())
                        {
                            if (DA.Fill(DT1) > 0)
                            {
                                //decimal InsetHeightAdmission = Convert.ToInt32(DT1.Rows[0]["InsetHeightAdmission"]);
                                //decimal InsetWidthAdmission = Convert.ToInt32(DT1.Rows[0]["InsetWidthAdmission"]);
                                //decimal d = (H - Convert.ToInt32(DT1.Rows[0]["InsetHeightAdmission"])) * (W - Convert.ToInt32(DT1.Rows[0]["InsetWidthAdmission"]));
                                //decimal h1 = H - Convert.ToInt32(DT1.Rows[0]["InsetHeightAdmission"]);
                                //decimal w1 = W - Convert.ToInt32(DT1.Rows[0]["InsetWidthAdmission"]);
                                //d = d / 1000000;
                                //d = Decimal.Round(d, 3, MidpointRounding.AwayFromZero);
                                //d = d * Convert.ToDecimal(Row["Count"]);

                                if (FrontID == 3729)
                                    NewRow["Count"] = Convert.ToDecimal(Row["Count"]) * Decimal.Round(Convert.ToInt32(DT1.Rows[0]["InsetHeightAdmission"]) * (W - Convert.ToInt32(DT1.Rows[0]["InsetWidthAdmission"])) / 1000000, 3, MidpointRounding.AwayFromZero);
                                else
                                    NewRow["Count"] = Convert.ToDecimal(Row["Count"]) * Decimal.Round((H - Convert.ToInt32(DT1.Rows[0]["InsetHeightAdmission"])) * (W - Convert.ToInt32(DT1.Rows[0]["InsetWidthAdmission"])) / 1000000, 3, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }


                NewRow["Measure"] = MeasuresDataTable.Select("MeasureID = " + MID)[0]["Measure"].ToString();
                NewRow["Price"] = Row["InsetPrice"];
                if (MID == 3)
                    NewRow["Cost"] = Convert.ToDecimal(Row["InsetPrice"]) * Convert.ToDecimal(Row["Count"]);
                decimal Count = Convert.ToDecimal(NewRow["Count"]);
                Count = Convert.ToDecimal(Row["Count"]);
                if (MID == 1)
                {
                    NewRow["Cost"] = Convert.ToDecimal(Row["InsetPrice"]) * Convert.ToDecimal(NewRow["Count"]);
                    Count = Convert.ToDecimal(Row["InsetPrice"]) * Convert.ToDecimal(NewRow["Count"]);
                }
                ResultTable.Rows.Add(NewRow);
            }

            DT.Dispose();
        }

        private void Group()
        {
            DataTable DT = new DataTable();

            DT = ResultTable.Clone();

            for (int r = 0; r < ResultTable.Rows.Count; r++)
            {
                if (DT.Rows.Count == 0)
                {
                    DT.ImportRow(ResultTable.Rows[r]);
                    continue;
                }

                bool Ex = false;

                for (int d = 0; d < DT.Rows.Count; d++)
                {
                    if (DT.Rows[d]["ItemName"].ToString() == ResultTable.Rows[r]["ItemName"].ToString() &&
                        DT.Rows[d]["Price"].ToString() == ResultTable.Rows[r]["Price"].ToString())
                    {
                        DT.Rows[d]["Count"] = Convert.ToDecimal(DT.Rows[d]["Count"]) + Convert.ToDecimal(ResultTable.Rows[r]["Count"]);
                        DT.Rows[d]["Cost"] = Convert.ToDecimal(DT.Rows[d]["Cost"]) + Convert.ToDecimal(ResultTable.Rows[r]["Cost"]);
                        Ex = true;
                        break;
                    }

                    if (d == DT.Rows.Count - 1 && Ex == false)
                    {
                        DT.ImportRow(ResultTable.Rows[r]);
                        break;
                    }
                }
            }

            ResultTable.Clear();
            ResultTable = DT.Copy();

            DT.Dispose();
        }

        private bool FillFrontsOrders(int MainOrderID)
        {
            FrontsOrdersDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                if (DA.Fill(FrontsOrdersDataTable) == 0)
                    return false;
            }

            return true;
        }

        public bool Collect(int MainOrderID)
        {
            MainTable.Clear();
            ResultTable.Clear();

            if (FillFrontsOrders(MainOrderID) == false)
                return false;

            CopyMainTable();
            GetStandard();
            GetCurvedFronts();
            GetInsets();
            Group();

            return (ResultTable.Rows.Count > 0);
        }

    }





    public class DailyReportDecorOrdersReport
    {
        DecorCatalogOrder DecorCatalogOrder = null;

        public DataTable ResultTable = null;

        public DailyReportDecorOrdersReport(ref DecorCatalogOrder tDecorCatalogOrder)
        {
            DecorCatalogOrder = tDecorCatalogOrder;

            CreateResultTable();
        }

        private string GetMeasure(int DecorConfigID)
        {
            int MeasureID = -1;

            MeasureID = DecorCatalogOrder.GetMeasureID(DecorConfigID);

            DataRow[] Rows = DecorCatalogOrder.MeasuresDataTable.Select("MeasureID = " + MeasureID);

            return Rows[0]["Measure"].ToString();
        }

        private bool GetReportParams(ref StringCollection Params, int ProductID)
        {
            string Prms = DecorCatalogOrder.DecorProductsDataTable.Select("ProductID = " + ProductID)[0]["ReportParam"].ToString();

            if (Prms.Length == 0)
                return false;

            string temp = "";

            for (int i = 0; i < Prms.Length; i++)
            {
                if (Prms[i] != ';')
                {
                    temp += Prms[i];
                }
                else
                {
                    Params.Add(temp);
                    temp = "";
                }
            }

            Params.Add(temp);

            return true;
        }

        private void CreateResultTable()
        {
            ResultTable = new DataTable();
            ResultTable.Columns.Add(new DataColumn(("ItemName"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.String")));
            ResultTable.Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.String")));
        }

        private void WriteResultTable(DataTable Table)
        {
            foreach (DataRow Row in Table.Rows)
            {
                string TableName = DecorCatalogOrder.DecorProductsDataTable.Select("ProductID = " + Row["ProductID"])[0]["ProductName"].ToString();

                StringCollection Params = new StringCollection();
                DataRow NewRow = ResultTable.NewRow();

                string ReportParams = null;

                if (GetReportParams(ref Params, Convert.ToInt32(Row["ProductID"])) == true)
                {
                    for (int i = 0; i < Params.Count; i++)
                    {
                        if (ReportParams != null)
                            ReportParams += "x" + Row[Params[i]];
                        else
                            ReportParams += ", " + Row[Params[i]];
                    }
                }
                NewRow["ItemName"] = TableName + " " + DecorCatalogOrder.GetItemName(Convert.ToInt32(Row["DecorID"])) + ReportParams;

                string Measure = GetMeasure(Convert.ToInt32(Row["DecorConfigID"]));

                if (Measure == "м.кв.")
                {

                    NewRow["Count"] = Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
                }
                else
                    NewRow["Count"] = Row["Count"];

                NewRow["Measure"] = Measure;

                NewRow["Price"] = Row["Price"];

                NewRow["Cost"] = Row["Cost"];

                ResultTable.Rows.Add(NewRow);
            }
        }

        private void Group()
        {
            DataTable DT = new DataTable();

            DT = ResultTable.Clone();

            for (int r = 0; r < ResultTable.Rows.Count; r++)
            {
                if (DT.Rows.Count == 0)
                {
                    DT.ImportRow(ResultTable.Rows[r]);
                    continue;
                }

                bool Ex = false;

                for (int d = 0; d < DT.Rows.Count; d++)
                {
                    if (DT.Rows[d]["ItemName"].ToString() == ResultTable.Rows[r]["ItemName"].ToString() &&
                        DT.Rows[d]["Price"].ToString() == ResultTable.Rows[r]["Price"].ToString())
                    {
                        DT.Rows[d]["Count"] = Convert.ToDecimal(DT.Rows[d]["Count"]) + Convert.ToDecimal(ResultTable.Rows[r]["Count"]);
                        DT.Rows[d]["Cost"] = Convert.ToDecimal(DT.Rows[d]["Cost"]) + Convert.ToDecimal(ResultTable.Rows[r]["Cost"]);

                        Ex = true;
                        break;
                    }

                    if (d == DT.Rows.Count - 1 && Ex == false)
                    {
                        DT.ImportRow(ResultTable.Rows[r]);
                        break;
                    }
                }
            }

            ResultTable.Clear();
            ResultTable = DT.Copy();

            DT.Dispose();
        }

        public bool Collect(int MainOrderID)
        {
            ResultTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("Select * FROM DecorOrders WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    WriteResultTable(DT);
                    Group();
                }
            }

            return (ResultTable.Rows.Count > 0);
        }
    }





    public class DailyReport
    {
        Excel Ex = null;

        DataTable ClientsDataTable = null;

        public DataTable[] ClientReportTables = null;

        public DataTable ReportTable = null;
        private DataTable MainOrdersDataTable = null;
        DataTable ClientErrorsDT = null;

        int[] Clients = null;

        DailyReportFrontsOrdersReport FrontsReport = null;
        DailyReportDecorOrdersReport DecorReport = null;

        MainOrdersFrontsOrders MainOrdersFrontsOrders = null;
        DecorCatalogOrder DecorCatalogOrder = null;


        public DailyReport(ref MainOrdersFrontsOrders tMainOrdersFrontsOrders,
                           ref DecorCatalogOrder tDecorCatalogOrder,
                           ref DataTable tClientsDataTable)
        {
            MainOrdersFrontsOrders = tMainOrdersFrontsOrders;
            DecorCatalogOrder = tDecorCatalogOrder;
            ClientsDataTable = tClientsDataTable;

            FrontsReport = new DailyReportFrontsOrdersReport(ref MainOrdersFrontsOrders);

            DecorReport = new DailyReportDecorOrdersReport(ref DecorCatalogOrder);

            CreateReportTable();
            MainOrdersDataTable = new DataTable();
            ClientErrorsDT = new DataTable();
        }

        private string GetClientName(int ClientID)
        {
            DataRow[] Rows = ClientsDataTable.Select("ClientID = " + ClientID);
            if (Rows.Count() == 0)
                return string.Empty;
            return Rows[0]["ClientName"].ToString();
        }

        private decimal CopyFrontsResultTable(DataTable ResultTable, DataTable ClientsTable, string DocNumber)
        {
            bool FirstRow = false;
            decimal Cost = 0;

            bool Calc = Convert.ToBoolean(MainOrdersDataTable.Select("DocNumber = '" + DocNumber + "'")[0]["NeedCalculate"]);
            bool IsSample = Convert.ToBoolean(MainOrdersDataTable.Select("DocNumber = '" + DocNumber + "'")[0]["IsSample"]);

            foreach (DataRow Row in ResultTable.Rows)
            {
                DataRow NewRow = ClientsTable.NewRow();

                if (FirstRow == false)
                    if (!Calc)
                        NewRow["DocNumber"] = DocNumber + " ДОЛГ";
                    else
                        NewRow["DocNumber"] = DocNumber;

                NewRow["IsSample"] = IsSample;
                NewRow["ItemName"] = Row["ItemName"];
                NewRow["Count"] = Decimal.Round(Convert.ToDecimal(Row["Count"]), 3, MidpointRounding.AwayFromZero).ToString();
                NewRow["Measure"] = Row["Measure"];
                NewRow["Price"] = Row["Price"];
                if (Calc)
                    NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(Row["Cost"]), 1, MidpointRounding.AwayFromZero);
                else
                    NewRow["Cost"] = 0;
                if (Calc)
                    Cost += Convert.ToDecimal(Row["Cost"]);
                NewRow["IsError"] = false;
                ClientsTable.Rows.Add(NewRow);

                FirstRow = true;
            }

            return Cost;
        }

        private decimal AddClientErrorWriteOff(DataTable ClientsTable, int ClientID, string DispatchDate)
        {
            string DocNumber = string.Empty;
            string Product = string.Empty;
            string Reason = string.Empty;
            decimal Cost = 0;
            decimal TotalCost = 0;
            DataRow[] Rows = ClientErrorsDT.Select("ClientID=" + ClientID + " AND OrderDate='" + Convert.ToDateTime(DispatchDate).ToString("dd.MM.yyyy") + "'");
            for (int i = 0; i < Rows.Count(); i++)
            {
                if (Rows[i]["DocNumber"] != DBNull.Value)
                    DocNumber = Rows[i]["DocNumber"].ToString();
                if (Rows[i]["Product"] != DBNull.Value)
                    Product = Rows[i]["Product"].ToString();
                if (Rows[i]["Reason"] != DBNull.Value)
                    Reason = Rows[i]["Reason"].ToString();
                if (Rows[i]["Cost"] != DBNull.Value)
                {
                    Cost = Convert.ToDecimal(Rows[i]["Cost"]);
                    TotalCost += Convert.ToDecimal(Rows[i]["Cost"]);
                }
                DataRow NewRow = ClientsTable.NewRow();

                NewRow["DocNumber"] = DocNumber;
                NewRow["ItemName"] = Product;
                NewRow["Count"] = Reason;
                NewRow["IsError"] = true;
                NewRow["Cost"] = Decimal.Round(Cost, 1, MidpointRounding.AwayFromZero);
                ClientsTable.Rows.Add(NewRow);
            }

            return TotalCost;
        }

        private decimal CopyDecorResultTable(DataTable ResultTable, DataTable ClientsTable, string DocNumber, bool IsFronts)
        {
            decimal Cost = 0;

            bool Calc = Convert.ToBoolean(MainOrdersDataTable.Select("DocNumber = '" + DocNumber + "'")[0]["NeedCalculate"]);
            bool IsSample = Convert.ToBoolean(MainOrdersDataTable.Select("DocNumber = '" + DocNumber + "'")[0]["IsSample"]);

            DataRow NRow = ClientsTable.NewRow();
            NRow["ItemName"] = "декор";
            NRow["IsSample"] = IsSample;
            if (IsFronts == false)
                if (!Calc)
                    NRow["DocNumber"] = DocNumber + " ДОЛГ";
                else
                    NRow["DocNumber"] = DocNumber;
            ClientsTable.Rows.Add(NRow);

            foreach (DataRow Row in ResultTable.Rows)
            {
                DataRow NewRow = ClientsTable.NewRow();

                NewRow["ItemName"] = Row["ItemName"];
                NewRow["IsSample"] = IsSample;
                NewRow["Count"] = Decimal.Round(Convert.ToDecimal(Row["Count"]), 3, MidpointRounding.AwayFromZero).ToString();
                NewRow["Measure"] = Row["Measure"];
                NewRow["Price"] = Row["Price"];
                if (Calc)
                    NewRow["Cost"] = Decimal.Round(Convert.ToDecimal(Row["Cost"]), 1, MidpointRounding.AwayFromZero);
                else
                    NewRow["Cost"] = 0;
                if (Calc)
                    Cost += Convert.ToDecimal(Row["Cost"]);
                NewRow["IsError"] = false;
                ClientsTable.Rows.Add(NewRow);
            }

            return Cost;
        }

        public void CreateClientReport(int ClientID, DataRow[] MainRows, int CurrentClient, string DispatchDate)
        {
            string ClientName = GetClientName(ClientID);

            ClientReportTables[CurrentClient].TableName = ClientName;

            decimal Cost = 0;

            foreach (DataRow Row in MainRows)
            {
                bool IsFronts = false;

                if (FrontsReport.Collect(Convert.ToInt32(Row["MainOrderID"])) == true)
                {
                    Cost += CopyFrontsResultTable(FrontsReport.ResultTable, ClientReportTables[CurrentClient], Row["DocNumber"].ToString());
                    IsFronts = true;
                }

                if (DecorReport.Collect(Convert.ToInt32(Row["MainOrderID"])) == true)
                {
                    Cost += CopyDecorResultTable(DecorReport.ResultTable, ClientReportTables[CurrentClient], Row["DocNumber"].ToString(), IsFronts);
                }
            }
            Cost += AddClientErrorWriteOff(ClientReportTables[CurrentClient], ClientID, DispatchDate);

            DataRow NewRow = ClientReportTables[CurrentClient].NewRow();
            NewRow["Cost"] = Decimal.Round(Cost, 1, MidpointRounding.AwayFromZero);
            ClientReportTables[CurrentClient].Rows.Add(NewRow);
        }

        private int GetClients(DataTable Table)
        {
            int ClientsCount = 0;

            using (DataView DV = new DataView(Table))
            {
                DataTable DT = new DataTable();
                DT = DV.ToTable(true, new string[] { "ClientID" });

                ClientsCount = DT.Rows.Count;

                Clients = new int[ClientsCount];

                for (int i = 0; i < ClientsCount; i++)
                {
                    Clients[i] = Convert.ToInt32(DT.Rows[i]["ClientID"]);
                }
            }

            return ClientsCount;
        }

        private void CreateClientReportTables(int ClientsCount)
        {
            ClientReportTables = new DataTable[ClientsCount];

            for (int i = 0; i < ClientsCount; i++)
            {
                ClientReportTables[i] = new DataTable();
                ClientReportTables[i].Columns.Add(new DataColumn(("DocNumber"), System.Type.GetType("System.String")));
                ClientReportTables[i].Columns.Add(new DataColumn(("ItemName"), System.Type.GetType("System.String")));
                ClientReportTables[i].Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.String")));
                ClientReportTables[i].Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
                ClientReportTables[i].Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.Decimal")));
                ClientReportTables[i].Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.Decimal")));
                ClientReportTables[i].Columns.Add(new DataColumn(("IsError"), System.Type.GetType("System.Boolean")));
                ClientReportTables[i].Columns.Add(new DataColumn(("IsSample"), System.Type.GetType("System.Boolean")));
            }
        }

        private void CreateReportTable()
        {
            ReportTable = new DataTable();

            ReportTable.Columns.Add(new DataColumn(("DocNumber"), System.Type.GetType("System.String")));
            ReportTable.Columns.Add(new DataColumn(("ItemName"), System.Type.GetType("System.String")));
            ReportTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.String")));
            ReportTable.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            ReportTable.Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.Decimal")));
            ReportTable.Columns.Add(new DataColumn(("Cost"), System.Type.GetType("System.Decimal")));
            ReportTable.Columns.Add(new DataColumn(("IsError"), System.Type.GetType("System.Boolean")));
            ReportTable.Columns.Add(new DataColumn(("IsSample"), System.Type.GetType("System.Boolean")));
        }

        private void CollectClientsReports(int ClientsCount, string DispatchDate)
        {
            Decimal Cost = 0;

            for (int i = 0; i < ClientsCount; i++)
            {


                DataRow NewRow = ReportTable.NewRow();
                NewRow["DocNumber"] = ClientReportTables[i].TableName;
                NewRow["ItemName"] = "отгр. " + Convert.ToDateTime(DispatchDate).ToString("dd.MM.yyyy");
                ReportTable.Rows.Add(NewRow);

                foreach (DataRow Row in ClientReportTables[i].Rows)
                {
                    ReportTable.ImportRow(Row);
                    try
                    {
                        Cost = Convert.ToDecimal(Row["Cost"]);
                        if (Row["Price"].ToString().Length == 0)
                            Cost += Convert.ToDecimal(Row["Cost"]);
                    }
                    catch { }

                }
            }
        }

        public void Clear()
        {
            if (ReportTable != null)
                ReportTable.Clear();

            if (Clients == null)
                return;

            for (int i = 0; i < Clients.Count(); i++)
                if (ClientReportTables[i] != null)
                    ClientReportTables[i].Dispose();
        }

        public bool IsDispatch(int MegaOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders" +
                  " WHERE MegaOrderID = " + MegaOrderID + " AND IsPrepared <> 1" +
                  " ORDER BY MainOrderID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT.Rows.Count > 0;
                }
            }
        }

        public bool CreateReport(int MegaOrderID, string Day)
        {
            Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ClientErrorsWriteOffID, ClientID, MainOrderID, DocNumber, Product, CAST(OrderDate AS Date) AS OrderDate, Cost, Reason, Created
                FROM ClientErrorsWriteOffs ORDER BY MainOrderID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientErrorsDT.Clear();
                    DA.Fill(ClientErrorsDT);
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + " AND IsPrepared <> 1" +
                " ORDER BY MainOrderID", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    MainOrdersDataTable.Clear();
                    MainOrdersDataTable = DT.Copy();

                    int ClientsCount = GetClients(DT);

                    CreateClientReportTables(ClientsCount);

                    for (int i = 0; i < ClientsCount; i++)
                    {
                        CreateClientReport(Clients[i], DT.Select("ClientID = " + Clients[i]), i, Day);
                    }

                    CollectClientsReports(ClientsCount, Day);
                }
            }

            return true;
        }

        public void ReportToExcel()
        {
            int RowIndex = 1;
            int TopRowIndex = RowIndex;
            int BottomRowIndex = ReportTable.Rows.Count + 1;

            bool NeedBorder = false;
            bool NeedPaint = false;

            if (Ex != null)
            {
                Ex.Dispose();
                Ex = null;
            }
            Ex = new Excel();
            Ex.NewDocument(1);
            //Ex.SetMargins(1, 20, 0, 50, 0);

            //Ex.WriteCell(1, "Клиент\\№ кухни", RowIndex, 1, 11, true);
            //Ex.WriteCell(1, "Наименование", RowIndex, 2, 11, true);
            //Ex.WriteCell(1, "Кол-во", RowIndex, 3, 11, true);
            //Ex.WriteCell(1, "Единицы\r\nизмерения", RowIndex, 4, 11, true);
            //Ex.SetWrapText(1, RowIndex, 4, true);
            //Ex.WriteCell(1, "Цена", RowIndex, 5, 11, true);
            //Ex.WriteCell(1, "Стоимость", RowIndex, 6, 11, true);

            //RowIndex++;
            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                string doc = ReportTable.Rows[i][1].ToString();
                doc = ReportTable.Rows[i][5].ToString();

                NeedPaint = false;
                if (
                    (ReportTable.Rows[i][1].ToString().Length > 4
                    && ReportTable.Rows[i][1].ToString().Substring(0, 4) == "отгр")
                    ||
                    (ReportTable.Rows[i][5].ToString().Length > 0
                    && ReportTable.Rows[i][5 - 1].ToString().Length == 0)
                    )
                    NeedBorder = false;
                else
                    NeedBorder = true;
                if (ReportTable.Rows[i][6] != DBNull.Value && Convert.ToBoolean(ReportTable.Rows[i][6]))
                {
                    NeedPaint = true;
                    NeedBorder = true;
                }

                for (int j = 0; j < ReportTable.Columns.Count; j++)
                {
                    if (ReportTable.Columns[j].ColumnName == "IsError" || ReportTable.Columns[j].ColumnName == "IsSample")
                        continue;
                    if (NeedBorder)
                        Ex.SetBorderStyle(1, RowIndex, j + 1, true, true, true, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);

                    doc = ReportTable.Rows[i][j].ToString();
                    bool IsSample = false;
                    if (ReportTable.Rows[i]["IsSample"] != DBNull.Value)
                        IsSample = Convert.ToBoolean(ReportTable.Rows[i]["IsSample"]);
                    if (j == 0)
                    {
                        if (IsSample)
                        {
                            if (ReportTable.Rows[i][1].ToString().Length > 4 && ReportTable.Rows[i][1].ToString().Substring(0, 4) == "отгр")
                                Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, true, Excel.Color.Red);
                            else
                                Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, false, Excel.Color.Red);
                            if (NeedPaint)
                                Ex.SetColor(1, RowIndex, j + 1, Excel.Color.Tan);
                        }
                        else
                        {
                            if (ReportTable.Rows[i][1].ToString().Length > 4 && ReportTable.Rows[i][1].ToString().Substring(0, 4) == "отгр")
                                Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, true);
                            else
                                Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, false);
                            if (NeedPaint)
                                Ex.SetColor(1, RowIndex, j + 1, Excel.Color.Tan);
                        }
                        continue;
                    }
                    if (j == 1 && ReportTable.Rows[i][j].ToString() == "декор")
                    {
                        if (IsSample)
                            Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, false, Excel.Color.Red);
                        else
                            Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, false);
                        continue;
                    }
                    if (j == 1 && ReportTable.Rows[i][j].ToString().Length > 4)
                    {
                        if (ReportTable.Rows[i][j].ToString().Substring(0, 4) == "отгр")
                        {
                            if (IsSample)
                                Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, false, Excel.Color.Red);
                            else
                                Ex.WriteCell(1, ReportTable.Rows[i][j].ToString(), RowIndex, j + 1, 11, false);
                            continue;
                        }
                    }
                    doc = ReportTable.Rows[i][j - 1].ToString();
                    doc = ReportTable.Rows[i][j].ToString();
                    if (j == 5 && ReportTable.Rows[i][j].ToString().Length > 0 && ReportTable.Rows[i][j - 1].ToString().Length == 0)
                    {
                        if (IsSample)
                            Ex.WriteCell(1, Convert.ToDecimal(ReportTable.Rows[i][j]), RowIndex, j + 1, 11, true, Excel.Color.Red);
                        else
                            Ex.WriteCell(1, Convert.ToDecimal(ReportTable.Rows[i][j]), RowIndex, j + 1, 11, true);

                        if (NeedPaint)
                            Ex.SetColor(1, RowIndex, j + 1, Excel.Color.Tan);
                        //RowIndex++;
                        continue;
                    }

                    Type t = ReportTable.Rows[i][j].GetType();

                    if (t.Name == "Decimal")
                    {
                        if (IsSample)
                            Ex.WriteCell(1, Convert.ToDecimal(ReportTable.Rows[i][j]), RowIndex, j + 1, 11, false, Excel.Color.Red);
                        else
                            Ex.WriteCell(1, Convert.ToDecimal(ReportTable.Rows[i][j]), RowIndex, j + 1, 11, false);
                    }
                    if (t.Name == "Int32")
                    {
                        if (IsSample)
                            Ex.WriteCell(1, ReportTable.Rows[i][j], RowIndex, j + 1, 11, false, Excel.Color.Red);
                        else
                            Ex.WriteCell(1, ReportTable.Rows[i][j], RowIndex, j + 1, 11, false);
                    }
                    if (t.Name == "String")
                    {
                        if (!NeedPaint)
                        {
                            if (decimal.TryParse(ReportTable.Rows[i][j].ToString(), out decimal d))
                            {
                                if (IsSample)
                                    Ex.WriteCell(1, d, RowIndex, j + 1, 11, false, Excel.Color.Red);
                                else
                                    Ex.WriteCell(1, d, RowIndex, j + 1, 11, false);
                            }
                            else
                            {
                                if (IsSample)
                                    Ex.WriteCell(1, ReportTable.Rows[i][j], RowIndex, j + 1, 11, false, Excel.Color.Red);
                                else
                                    Ex.WriteCell(1, ReportTable.Rows[i][j], RowIndex, j + 1, 11, false);
                            }
                        }
                        else
                        {
                            if (IsSample)
                                Ex.WriteCell(1, ReportTable.Rows[i][j], RowIndex, j + 1, 11, false, Excel.Color.Red);
                            else
                                Ex.WriteCell(1, ReportTable.Rows[i][j], RowIndex, j + 1, 11, false);
                        }
                    }
                    if (NeedPaint)
                        Ex.SetColor(1, RowIndex, j + 1, Excel.Color.Tan);
                }
                RowIndex++;
            }
            BottomRowIndex++;
            Ex.AutoFit(1, 1, 1, ReportTable.Rows.Count, ReportTable.Columns.Count);
            Ex.SetColumnWidth(1, 3, 9);
            Ex.SetColumnWidth(1, 4, 9);
            Ex.SetColumnWidth(1, 5, 9);
            Ex.SetColumnWidth(1, 6, 9);
            //Ex.SetRowHeight(1, 1, 34);

            //for (int i = 1; i <= ReportTable.Columns.Count; i++)
            //{
            //    Ex.SetHorisontalAlignment(1, TopRowIndex, i, Excel.AlignHorizontal.xlCenter);
            //    Ex.SetVerticalAlignment(1, TopRowIndex, i, Excel.AlignVertical.xlCenter);
            //    Ex.SetBorderStyle(1, TopRowIndex, i, true, true, true, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlMedium);
            //}
            //for (int i = 1; i <= ReportTable.Columns.Count; i++)
            //{
            //    Ex.SetBorderStyle(1, TopRowIndex, i, false, false, true, false, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlMedium);
            //}
            //for (int i = 1; i <= ReportTable.Columns.Count; i++)
            //{
            //    Ex.SetBorderStyle(1, BottomRowIndex, i, false, false, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlMedium);
            //}
            //for (int i = TopRowIndex; i <= BottomRowIndex; i++)
            //{
            //    Ex.SetBorderStyle(1, i, 1, true, false, false, false, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlMedium);
            //}
            //for (int i = TopRowIndex; i <= BottomRowIndex; i++)
            //{
            //    Ex.SetBorderStyle(1, i, ReportTable.Columns.Count, false, true, false, false, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlMedium);
            //}
            Ex.Visible = true;
        }
    }




}
