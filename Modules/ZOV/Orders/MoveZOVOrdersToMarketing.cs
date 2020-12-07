using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Xml;

using MInvoiceReportToDBF = Infinium.Modules.Marketing.NewOrders.InvoiceReportToDBF;
using MOrdersCalculate = Infinium.Modules.Marketing.NewOrders.OrdersCalculate;

namespace Infinium.Modules.ZOV.Orders
{
    public class MoveZOVOrdersToMarketing
    {
        private decimal EURBYRCurrency = 1000000;
        private string zovClientName = string.Empty;
        private string zovDocNumber = string.Empty;
        private int marketingClientID = 0;
        private int marketingDelayOfPayment = 0;
        private DataTable zovMainOrdersInfoDT;
        private object zovDocDateTime = DBNull.Value;

        DataTable marketingFrontsOrdersDT = null;
        DataTable marketingDecorOrdersDT = null;
        DataTable marketingPackagesDT = null;
        DataTable marketingPackageDetailsDT = null;
        DataTable zovFrontsOrdersDT = null;
        DataTable zovDecorOrdersDT = null;
        DataTable zovMainOrdersDT = null;
        DataTable zovPackagesDT = null;
        DataTable zovPackageDetailsDT = null;

        MInvoiceReportToDBF DBFReport = null;
        MOrdersCalculate OrdersCalculate = null;
        public MoveZOVOrdersToMarketing()
        {
            zovMainOrdersInfoDT = new DataTable();
            zovMainOrdersInfoDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.String")));
            zovMainOrdersInfoDT.Columns.Add(new DataColumn("MainOrderID", Type.GetType("System.String")));
            zovMainOrdersInfoDT.Columns.Add(new DataColumn("DocDateTime", Type.GetType("System.Object")));
            zovMainOrdersInfoDT.Columns.Add(new DataColumn("ClienName", Type.GetType("System.String")));
            zovMainOrdersInfoDT.Columns.Add(new DataColumn("DocNumber", Type.GetType("System.String")));
            marketingFrontsOrdersDT = new DataTable();
            marketingDecorOrdersDT = new DataTable();
            marketingPackagesDT = new DataTable();
            marketingPackageDetailsDT = new DataTable();
            zovFrontsOrdersDT = new DataTable();
            zovDecorOrdersDT = new DataTable();
            zovMainOrdersDT = new DataTable();
            zovPackagesDT = new DataTable();
            zovPackageDetailsDT = new DataTable();

            DBFReport = new MInvoiceReportToDBF();
            OrdersCalculate = new MOrdersCalculate();
        }

        public int MarketingClientID
        {
            get { return marketingClientID; }
            set { marketingClientID = value; }
        }

        public void AddZOVMainOrder(int ClientID, int MainOrderID, object DocDateTime, string ClienName, string DocNumber)
        {
            DataRow NewRow = zovMainOrdersInfoDT.NewRow();
            NewRow["ClientID"] = ClientID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DocDateTime"] = DocDateTime;
            NewRow["ClienName"] = ClienName;
            NewRow["DocNumber"] = DocNumber;
            zovMainOrdersInfoDT.Rows.Add(NewRow);
        }

        public void ClearZOVMainOrdersInfo()
        {
            zovMainOrdersInfoDT.Clear();
        }

        public void MoveToOthersOrders()
        {
            if (zovMainOrdersInfoDT.Rows.Count == 0)
                return;
            marketingDelayOfPayment = GetDelayOfPayment();
            foreach (DataRow zRow in zovMainOrdersInfoDT.Rows)
            {
                int MegaOrderID = CreateMegaOrder(zRow["DocDateTime"]);
                if (MegaOrderID != -1)
                {
                    int zMainOrderID = Convert.ToInt32(zRow["MainOrderID"]);
                    object DocDateTime = zRow["DocDateTime"];
                    string ClienName = zRow["ClienName"].ToString();
                    string DocNumber = zRow["DocNumber"].ToString();
                    GetZOVMainOrdersDT(zMainOrderID);
                    if (zovMainOrdersDT.Rows.Count > 0)
                    {
                        int mMainOrderID = CreateMainOrders(MegaOrderID, ClienName, DocNumber);
                        if (mMainOrderID != -1)
                        {
                            GetZOVFrontOrdersDT(zMainOrderID);
                            GetZOVDecorOrdersDT(zMainOrderID);
                            if (zovFrontsOrdersDT.Rows.Count > 0)
                                CreateFrontsOrders(mMainOrderID);
                            if (zovDecorOrdersDT.Rows.Count > 0)
                                CreateDecorOrders(mMainOrderID);
                            GetZOVPackagesDT(zMainOrderID);
                            CreatePackage(mMainOrderID);

                            if (marketingClientID == 145)
                            {
                                OrdersCalculate.Recalculate(MegaOrderID, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 5, EURBYRCurrency, DocDateTime);
                                SetCurrencyCost(MegaOrderID, 0, 0, string.Empty, false, marketingDelayOfPayment, 0, 0, 5, EURBYRCurrency, EURBYRCurrency, 1, 1, 0, 0, 0, 0,
                                    DBFReport.CalcCurrencyCost(MegaOrderID, marketingClientID, 5, EURBYRCurrency));
                            }
                            else
                            {
                                OrdersCalculate.Recalculate(MegaOrderID, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, DocDateTime);
                                SetCurrencyCost(MegaOrderID, 0, 0, string.Empty, false, marketingDelayOfPayment, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0,
                                    DBFReport.CalcCurrencyCost(MegaOrderID, marketingClientID, 1, 1));
                            }
                            AcceptOrder(MegaOrderID);
                            MoveOrdersTo(MegaOrderID);
                            CheckOrdersStatus.SetMegaOrderStatus(MegaOrderID);
                        }
                    }
                }
            }
        }

        public void MoveToOneOrder()
        {
            if (zovMainOrdersInfoDT.Rows.Count == 0)
                return;
            marketingDelayOfPayment = GetDelayOfPayment();
            int MegaOrderID = CreateMegaOrder(zovMainOrdersInfoDT.Rows[0]["DocDateTime"]);
            if (MegaOrderID != -1)
            {
                foreach (DataRow zRow in zovMainOrdersInfoDT.Rows)
                {
                    int zClientID = Convert.ToInt32(zRow["ClientID"]);
                    int zMainOrderID = Convert.ToInt32(zRow["MainOrderID"]);
                    object DocDateTime = zRow["DocDateTime"];
                    string ClienName = zRow["ClienName"].ToString();
                    string DocNumber = zRow["DocNumber"].ToString();
                    GetZOVMainOrdersDT(zMainOrderID);
                    if (zovMainOrdersDT.Rows.Count > 0)
                    {
                        int mMainOrderID = CreateMainOrders(MegaOrderID, ClienName, DocNumber);
                        if (mMainOrderID != -1)
                        {
                            CreateJoinMainOrder(mMainOrderID, zMainOrderID, zClientID, ClienName, DocNumber);
                            GetZOVFrontOrdersDT(zMainOrderID);
                            GetZOVDecorOrdersDT(zMainOrderID);
                            if (zovFrontsOrdersDT.Rows.Count > 0)
                                CreateFrontsOrders(mMainOrderID);
                            if (zovDecorOrdersDT.Rows.Count > 0)
                                CreateDecorOrders(mMainOrderID);
                            GetZOVPackagesDT(zMainOrderID);
                            CreatePackage(mMainOrderID);

                            if (marketingClientID == 145)
                            {
                                OrdersCalculate.Recalculate(MegaOrderID, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 5, EURBYRCurrency, DocDateTime);
                                SetCurrencyCost(MegaOrderID, 0, 0, string.Empty, false, marketingDelayOfPayment, 0, 0, 5, EURBYRCurrency, EURBYRCurrency, 1, 1, 0, 0, 0, 0,
                                    DBFReport.CalcCurrencyCost(MegaOrderID, marketingClientID, 5, EURBYRCurrency));
                            }
                            else
                            {
                                OrdersCalculate.Recalculate(MegaOrderID, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, DocDateTime);
                                SetCurrencyCost(MegaOrderID, 0, 0, string.Empty, false, marketingDelayOfPayment, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0,
                                    DBFReport.CalcCurrencyCost(MegaOrderID, marketingClientID, 1, 1));
                            }
                            AcceptOrder(MegaOrderID);
                            MoveOrdersTo(MegaOrderID);
                            CheckOrdersStatus.SetMegaOrderStatus(MegaOrderID);
                        }
                    }
                }
            }
        }

        public void GetFixedPaymentRate(int ClientID, DateTime ConfirmDateTime, ref bool FixedPaymentRate)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ClientRates WHERE CAST(Date AS Date) <= 
                    '" + ConfirmDateTime.ToString("yyyy-MM-dd") + "' AND ClientID = " + ClientID + " ORDER BY Date DESC",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        if (DT.Rows[0]["USD"] == DBNull.Value || DT.Rows[0]["RUB"] == DBNull.Value || DT.Rows[0]["BYN"] == DBNull.Value)
                            FixedPaymentRate = false;
                        else
                        {
                            FixedPaymentRate = true;
                            EURBYRCurrency = Convert.ToDecimal(DT.Rows[0]["BYN"]);

                            //BYN = 2.54m;
                        }
                    }
                    else
                        FixedPaymentRate = false;
                }
            }
            return;
        }

        public void GetDateRates(DateTime DateTime, ref bool RateExist)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DateRates WHERE CAST(Date AS Date) = 
                    '" + DateTime.ToString("yyyy-MM-dd") + "'",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        RateExist = true;
                        EURBYRCurrency = Convert.ToDecimal(DT.Rows[0]["BYN"]);
                    }
                    else
                        RateExist = false;
                }
            }
            return;
        }

        public bool NBRBDailyRates(DateTime date)
        {
            string error = "";
            string EuroXML = "";
            string url = "http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM/dd/yyyy");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.AllowAutoRedirect = true;
                CookieContainer cookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer = cookieContainer;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            XmlTextReader reader = new XmlTextReader(myHttpWebResponse.GetResponseStream());
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Currency")
                            {
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "Id")
                                        {
                                            if (reader.Value == "292")
                                            {
                                                reader.MoveToElement();
                                                EuroXML = reader.ReadOuterXml();
                                            }
                                        }
                                        if (reader.Name == "Id")
                                        {
                                            if (reader.Value == "19")
                                            {
                                                reader.MoveToElement();
                                                EuroXML = reader.ReadOuterXml();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Currency/Rate");
                EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
            }
            catch (WebException ex)
            {
                error = ex.Message;
                return false;
            }
            return true;
        }

        public void SetCurrencyCost(int MegaOrderID, decimal ComplaintProfilCost, decimal ComplaintTPSCost, string ComplaintNotes, bool IsComplaint, int DelayOfPayment,
            decimal TransportCost, decimal AdditionalCost, int CurrencyTypeID, decimal OriginalCurrency, decimal PaymentCurrency,
            int DiscountPaymentConditionID, int DiscountFactoringID, decimal ProfilDiscountOrderSum, decimal TPSDiscountOrderSum, decimal ProfilDiscountDirector,
            decimal TPSDiscountDirector, decimal CurrencyTotalCost)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MegaOrderID, OrderStatusID, AgreementStatusID, OrderCost,
                ComplaintProfilCost, ComplaintTPSCost, CurrencyComplaintProfilCost, CurrencyComplaintTPSCost, ComplaintNotes, IsComplaint,DelayOfPayment,
                TransportCost, AdditionalCost, TotalCost, CurrencyOrderCost," +
                " CurrencyTransportCost, CurrencyAdditionalCost, CurrencyTotalCost, CurrencyTypeID, Rate, PaymentRate, ConfirmDateTime, AgreementStatusID, DiscountPaymentConditionID, DiscountFactoringID, ProfilDiscountOrderSum, TPSDiscountOrderSum, ProfilDiscountDirector, TPSDiscountDirector" +
                " FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        int OrderStatusID = Convert.ToInt32(DT.Rows[0]["OrderStatusID"]);
                        int AgreementStatusID = Convert.ToInt32(DT.Rows[0]["AgreementStatusID"]);
                        decimal OrderCost = Convert.ToDecimal(DT.Rows[0]["OrderCost"]);
                        //decimal CurrencyOrderCost = Convert.ToDecimal(DT.Rows[0]["CurrencyOrderCost"]);
                        decimal TotalCost = OrderCost + TransportCost + AdditionalCost;
                        decimal CurrencyTransportCost = Decimal.Round(TransportCost * PaymentCurrency, 2, MidpointRounding.AwayFromZero);
                        decimal CurrencyAdditionalCost = Decimal.Round(AdditionalCost * PaymentCurrency, 2, MidpointRounding.AwayFromZero);
                        decimal CurrencyComplaintProfilCost = Decimal.Round(ComplaintProfilCost * PaymentCurrency, 2, MidpointRounding.AwayFromZero);
                        decimal CurrencyComplaintTPSCost = Decimal.Round(ComplaintTPSCost * PaymentCurrency, 2, MidpointRounding.AwayFromZero);
                        decimal CurrencyOrderCost = Decimal.Round((CurrencyTotalCost - CurrencyTransportCost - CurrencyAdditionalCost), 2, MidpointRounding.AwayFromZero);
                        OriginalCurrency = Decimal.Round(OriginalCurrency, 4, MidpointRounding.AwayFromZero);
                        PaymentCurrency = Decimal.Round(PaymentCurrency, 4, MidpointRounding.AwayFromZero);


                        DT.Rows[0]["TransportCost"] = TransportCost;
                        DT.Rows[0]["AdditionalCost"] = AdditionalCost;
                        DT.Rows[0]["TotalCost"] = TotalCost;

                        if (AgreementStatusID != 2 && OrderStatusID != 3)
                            DT.Rows[0]["AgreementStatusID"] = 1;

                        DT.Rows[0]["DelayOfPayment"] = DelayOfPayment;
                        DT.Rows[0]["DiscountPaymentConditionID"] = DiscountPaymentConditionID;
                        DT.Rows[0]["DiscountFactoringID"] = DiscountFactoringID;
                        DT.Rows[0]["ProfilDiscountOrderSum"] = ProfilDiscountOrderSum;
                        DT.Rows[0]["TPSDiscountOrderSum"] = TPSDiscountOrderSum;
                        DT.Rows[0]["ProfilDiscountDirector"] = ProfilDiscountDirector;
                        DT.Rows[0]["TPSDiscountDirector"] = TPSDiscountDirector;
                        DT.Rows[0]["CurrencyTypeID"] = CurrencyTypeID;
                        DT.Rows[0]["CurrencyTransportCost"] = CurrencyTransportCost;
                        DT.Rows[0]["CurrencyAdditionalCost"] = CurrencyAdditionalCost;
                        DT.Rows[0]["CurrencyTotalCost"] = CurrencyTotalCost;
                        DT.Rows[0]["CurrencyOrderCost"] = CurrencyOrderCost;
                        DT.Rows[0]["Rate"] = OriginalCurrency;
                        DT.Rows[0]["PaymentRate"] = PaymentCurrency;
                        DT.Rows[0]["ComplaintProfilCost"] = ComplaintProfilCost;
                        DT.Rows[0]["ComplaintTPSCost"] = ComplaintTPSCost;
                        DT.Rows[0]["CurrencyComplaintProfilCost"] = CurrencyComplaintProfilCost;
                        DT.Rows[0]["CurrencyComplaintTPSCost"] = CurrencyComplaintTPSCost;
                        DT.Rows[0]["ComplaintNotes"] = ComplaintNotes;
                        DT.Rows[0]["IsComplaint"] = IsComplaint;

                        DA.Update(DT);
                    }
                }
            }
        }

        public void AcceptOrder(int MegaOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM NewMegaOrders WHERE MegaOrderID=" + MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["AgreementStatusID"] = 2;
                            DT.Rows[0]["ConfirmDateTime"] = Security.GetCurrentDate();
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void MoveOrdersTo(int MegaOrderID)
        {
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT * FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT * FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (TempDT.Rows.Count > 0)
                        {
                            for (int i = DT.Rows.Count - 1; i >= 0; i--)
                                DT.Rows[i].Delete();
                            DataRow NewRow = DT.NewRow();
                            NewRow.ItemArray = TempDT.Rows[0].ItemArray;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                    }
                }
            }

            TempDT.Dispose();
            TempDT = new DataTable();
            SelectCommand = @"SELECT * FROM NewMainOrders WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT * FROM MainOrders WHERE MegaOrderID = " + MegaOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (TempDT.Rows.Count > 0)
                        {
                            for (int i = DT.Rows.Count - 1; i >= 0; i--)
                                DT.Rows[i].Delete();
                            for (int i = 0; i < TempDT.Rows.Count; i++)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                DT.Rows.Add(NewRow);
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            //if (AgreementStatusID != 2)
            //{
            TempDT.Dispose();
            TempDT = new DataTable();
            SelectCommand = @"SELECT * FROM NewFrontsOrders WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT * FROM FrontsOrders WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = DT.Rows.Count - 1; i >= 0; i--)
                            DT.Rows[i].Delete();
                        DA.Update(DT);
                        DT.Clear();
                        if (TempDT.Rows.Count > 0)
                        {
                            for (int i = 0; i < TempDT.Rows.Count; i++)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                DT.Rows.Add(NewRow);
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }

            TempDT.Dispose();
            TempDT = new DataTable();
            SelectCommand = @"SELECT * FROM NewDecorOrders WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(TempDT);
            }
            SelectCommand = @"SELECT * FROM DecorOrders WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = DT.Rows.Count - 1; i >= 0; i--)
                            DT.Rows[i].Delete();
                        DA.Update(DT);
                        DT.Clear();
                        if (TempDT.Rows.Count > 0)
                        {
                            DA.Update(DT);
                            for (int i = 0; i < TempDT.Rows.Count; i++)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                DT.Rows.Add(NewRow);
                            }
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        private bool GetZOVPackagesDT(int MainOrderID)
        {
            zovPackagesDT.Clear();
            zovPackageDetailsDT.Clear();
            string SelectCommand = @"SELECT * FROM Packages WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(zovPackagesDT);
            }
            SelectCommand = @"SELECT * FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID=" + MainOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(zovPackageDetailsDT);
            }
            return zovPackagesDT.Rows.Count > 0;
        }

        private void GetZOVMainOrdersDT(int MainOrderID)
        {
            zovMainOrdersDT.Clear();
            string SelectCommand = @"SELECT * FROM MainOrders WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(zovMainOrdersDT);
            }
        }

        private void GetZOVFrontOrdersDT(int MainOrderID)
        {
            zovFrontsOrdersDT.Clear();
            string SelectCommand = @"SELECT * FROM FrontsOrders WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(zovFrontsOrdersDT);
            }
        }

        private void GetZOVDecorOrdersDT(int MainOrderID)
        {
            zovDecorOrdersDT.Clear();
            string SelectCommand = @"SELECT * FROM DecorOrders WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(zovDecorOrdersDT);
            }
        }

        private int GetDelayOfPayment()
        {
            int DelayOfPayment = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, DelayOfPayment FROM Clients WHERE ClientID=" + marketingClientID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                    {
                        DelayOfPayment = Convert.ToInt32(DT.Rows[0]["DelayOfPayment"]);
                    }
                }
            }
            return DelayOfPayment;
        }

        private int GetNextMegaNumber()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MAX(OrderNumber) AS NEXT FROM NewMegaOrders WHERE ClientID = " + marketingClientID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    if (DT.Rows[0]["Next"] == DBNull.Value)
                        return 1;
                    return Convert.ToInt32(DT.Rows[0]["Next"]) + 1;
                }
            }
        }

        private int CreateMegaOrder(object zovDocDateTime)
        {
            int MegaOrderID = -1;
            DataTable mDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 1 * FROM NewMegaOrders WHERE ClientID = " + marketingClientID +
               " ORDER BY MegaOrderID DESC", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(mDT);

                    int OrderNumber = 1;
                    if (mDT.Rows.Count > 0)
                        OrderNumber = GetNextMegaNumber();

                    DataRow NewRow = mDT.NewRow();
                    NewRow["ClientID"] = marketingClientID;
                    NewRow["OrderNumber"] = OrderNumber;
                    NewRow["DelayOfPayment"] = marketingDelayOfPayment;
                    NewRow["OrderDate"] = zovDocDateTime;
                    mDT.Rows.Add(NewRow);

                    DA.Update(mDT);
                    mDT.Clear();
                    if (DA.Fill(mDT) > 0)
                        MegaOrderID = Convert.ToInt32(mDT.Rows[0]["MegaOrderID"]);
                }
            }
            return MegaOrderID;
        }

        private int CreateMainOrders(int MegaOrderID, string ClientName, string DocNumber)
        {
            int MainOrderID = -1;
            DataTable mDT = new DataTable();
            string SelectCommand = @"SELECT TOP 1 * FROM NewMainOrders WHERE MegaOrderID=" + MegaOrderID + " ORDER BY MainOrderID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(mDT);
                    for (int i = 0; i < zovMainOrdersDT.Rows.Count; i++)
                    {
                        DataRow NewRow = mDT.NewRow();
                        for (int j = 0; j < zovMainOrdersDT.Columns.Count; j++)
                        {
                            string ColumnName = zovMainOrdersDT.Columns[j].ColumnName;
                            if (mDT.Columns.Contains(ColumnName))
                                NewRow[zovMainOrdersDT.Columns[j].ColumnName] = zovMainOrdersDT.Rows[i][ColumnName];
                        }
                        NewRow["DocDateTime"] = Security.GetCurrentDate();
                        NewRow["MegaOrderID"] = MegaOrderID;
                        NewRow["WillPercentID"] = 1;
                        NewRow["Notes"] = ClientName + ", " + DocNumber;
                        mDT.Rows.Add(NewRow);
                    }
                    DA.Update(mDT);
                    mDT.Clear();
                    if (DA.Fill(mDT) > 0)
                        MainOrderID = Convert.ToInt32(mDT.Rows[0]["MainOrderID"]);
                }
            }
            mDT.Dispose();
            return MainOrderID;
        }

        private void CreateJoinMainOrder(int MarketMainOrderID, int ZOVMainOrderID, int ZOVClientID, string ClientName, string DocNumber)
        {
            DataTable mDT = new DataTable();
            string SelectCommand = @"SELECT TOP 0 * FROM JoinMainOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["DateTime"] = Security.GetCurrentDate();
                        NewRow["MarketMainOrderID"] = MarketMainOrderID;
                        NewRow["ZOVMainOrderID"] = ZOVMainOrderID;
                        NewRow["ClientName"] = ClientName;
                        NewRow["DocNumber"] = DocNumber;
                        NewRow["ZOVClientID"] = ZOVClientID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        private bool CreateFrontsOrders(int MainOrderID)
        {
            marketingFrontsOrdersDT.Clear();
            string SelectCommand = @"SELECT * FROM NewFrontsOrders WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(marketingFrontsOrdersDT);
                    for (int i = 0; i < zovFrontsOrdersDT.Rows.Count; i++)
                    {
                        DataRow NewRow = marketingFrontsOrdersDT.NewRow();
                        for (int j = 0; j < zovFrontsOrdersDT.Columns.Count; j++)
                        {
                            string ColumnName = zovFrontsOrdersDT.Columns[j].ColumnName;
                            if (marketingFrontsOrdersDT.Columns.Contains(ColumnName))
                                NewRow[zovFrontsOrdersDT.Columns[j].ColumnName] = zovFrontsOrdersDT.Rows[i][ColumnName];
                        }
                        NewRow["MainOrderID"] = MainOrderID;
                        marketingFrontsOrdersDT.Rows.Add(NewRow);
                    }
                    DA.Update(marketingFrontsOrdersDT);
                    marketingFrontsOrdersDT.Clear();
                    DA.Fill(marketingFrontsOrdersDT);
                }
            }
            return true;
        }

        private bool CreateDecorOrders(int MainOrderID)
        {
            marketingDecorOrdersDT.Clear();
            string SelectCommand = @"SELECT * FROM NewDecorOrders WHERE MainOrderID=" + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(marketingDecorOrdersDT);
                    for (int i = 0; i < zovDecorOrdersDT.Rows.Count; i++)
                    {
                        DataRow NewRow = marketingDecorOrdersDT.NewRow();
                        for (int j = 0; j < zovDecorOrdersDT.Columns.Count; j++)
                        {
                            string ColumnName = zovDecorOrdersDT.Columns[j].ColumnName;
                            if (marketingDecorOrdersDT.Columns.Contains(ColumnName))
                                NewRow[zovDecorOrdersDT.Columns[j].ColumnName] = zovDecorOrdersDT.Rows[i][ColumnName];
                        }
                        NewRow["MainOrderID"] = MainOrderID;
                        marketingDecorOrdersDT.Rows.Add(NewRow);
                    }
                    DA.Update(marketingDecorOrdersDT);
                    marketingDecorOrdersDT.Clear();
                    DA.Fill(marketingDecorOrdersDT);
                }
            }
            return true;
        }

        private void CreatePackage(int MainOrderID)
        {
            if (zovPackagesDT.Rows.Count == 0)
                return;

            for (int i = 0; i < zovPackagesDT.Rows.Count; i++)
            {
                int zovPackageID = Convert.ToInt32(zovPackagesDT.Rows[i]["PackageID"]);
                int PackNumber = Convert.ToInt32(zovPackagesDT.Rows[i]["PackNumber"]);
                int ProductType = Convert.ToInt32(zovPackagesDT.Rows[i]["ProductType"]);
                string SelectCommand = @"SELECT * FROM Packages WHERE PackNumber=" + PackNumber + " AND MainOrderID=" + MainOrderID;
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        marketingPackagesDT.Clear();
                        DA.Fill(marketingPackagesDT);

                        DataRow NewRow = marketingPackagesDT.NewRow();
                        for (int j = 0; j < zovPackagesDT.Columns.Count; j++)
                        {
                            string ColumnName = zovPackagesDT.Columns[j].ColumnName;
                            if (marketingPackagesDT.Columns.Contains(ColumnName))
                                NewRow[zovPackagesDT.Columns[j].ColumnName] = zovPackagesDT.Rows[i][ColumnName];
                        }
                        NewRow["MainOrderID"] = MainOrderID;
                        NewRow["TrayID"] = DBNull.Value;
                        NewRow["PalleteID"] = DBNull.Value;
                        NewRow["DispatchID"] = DBNull.Value;
                        marketingPackagesDT.Rows.Add(NewRow);

                        DA.Update(marketingPackagesDT);
                        marketingPackagesDT.Clear();
                        if (DA.Fill(marketingPackagesDT) > 0)
                        {
                            int marketingPackageID = Convert.ToInt32(marketingPackagesDT.Rows[0]["PackageID"]);
                            SelectCommand = @"SELECT TOP 0 * FROM PackageDetails";
                            using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                            {
                                using (SqlCommandBuilder CB1 = new SqlCommandBuilder(DA1))
                                {
                                    marketingPackageDetailsDT.Clear();
                                    DA1.Fill(marketingPackageDetailsDT);
                                    if (ProductType == 0)
                                    {
                                        DataRow[] Rows = zovPackageDetailsDT.Select("PackageID = " + zovPackageID);
                                        foreach (DataRow row in Rows)
                                        {
                                            int OrderID = Convert.ToInt32(row["OrderID"]);
                                            for (int j = 0; j < zovFrontsOrdersDT.Rows.Count; j++)
                                            {
                                                if (OrderID != Convert.ToInt32(zovFrontsOrdersDT.Rows[j]["FrontsOrdersID"]))
                                                    continue;
                                                DataRow mRow = marketingFrontsOrdersDT.Rows[j];
                                                CreatePackageDetail(marketingPackageID, PackNumber, Convert.ToInt32(mRow["FrontsOrdersID"]), Convert.ToInt32(mRow["Count"]));
                                            }
                                        }
                                    }
                                    if (ProductType == 1)
                                    {
                                        DataRow[] Rows = zovPackageDetailsDT.Select("PackageID = " + zovPackageID);
                                        foreach (DataRow row in Rows)
                                        {
                                            int OrderID = Convert.ToInt32(row["OrderID"]);
                                            for (int j = 0; j < zovDecorOrdersDT.Rows.Count; j++)
                                            {
                                                if (OrderID != Convert.ToInt32(zovDecorOrdersDT.Rows[j]["DecorOrderID"]))
                                                    continue;
                                                DataRow mRow = marketingDecorOrdersDT.Rows[j];
                                                CreatePackageDetail(marketingPackageID, PackNumber, Convert.ToInt32(mRow["DecorOrderID"]), Convert.ToInt32(mRow["Count"]));
                                            }
                                        }
                                    }
                                    DA1.Update(marketingPackageDetailsDT);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreatePackageDetail(int PackageID, int PackNumber, int OrderID, int Count)
        {
            DataRow NewRow = marketingPackageDetailsDT.NewRow();
            NewRow["PackageID"] = PackageID;
            NewRow["PackNumber"] = PackNumber;
            NewRow["OrderID"] = OrderID;
            NewRow["Count"] = Count;
            marketingPackageDetailsDT.Rows.Add(NewRow);
        }
    }

}
