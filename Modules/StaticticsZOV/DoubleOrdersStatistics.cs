using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Infinium.Modules.StatisticsMarketing
{
    public class DoubleOrdersStatistics
    {
        DataTable FirstOperatorStatisticsDT;
        DataTable SecondOperatorStatisticsDT;
        DataTable FirstOperatorsDT;
        DataTable SecondOperatorsDT;

        BindingSource FirstOperatorStatisticsBS;
        BindingSource SecondOperatorStatisticsBS;
        BindingSource FirstOperatorsBS;
        BindingSource SecondOperatorsBS;

        public DoubleOrdersStatistics()
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
            FirstOperatorStatisticsDT = new DataTable();
            SecondOperatorStatisticsDT = new DataTable();
            FirstOperatorsDT = new DataTable();
            SecondOperatorsDT = new DataTable();

            FirstOperatorStatisticsBS = new BindingSource();
            SecondOperatorStatisticsBS = new BindingSource();
            FirstOperatorsBS = new BindingSource();
            SecondOperatorsBS = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = "SELECT TOP 0 ClientName, DoubleOrders.DocNumber," +
                " FirstDocDateTime, FirstSaveDateTime, FirstErrors FROM DoubleOrders" +
                " INNER JOIN MainOrders ON DoubleOrders.DocNumber = MainOrders.DocNumber" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FirstOperatorStatisticsDT);
            }
            SelectCommand = "SELECT TOP 0 ClientName, DoubleOrders.DocNumber," +
                " SecondDocDateTime, SecondSaveDateTime, SecondErrors FROM DoubleOrders" +
                " INNER JOIN MainOrders ON DoubleOrders.DocNumber = MainOrders.DocNumber" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SecondOperatorStatisticsDT);
            }
            SelectCommand = "SELECT UserID, ShortName FROM Users" +
                " WHERE UserID IN (SELECT DISTINCT FirstOperatorID FROM infiniu2_zovorders.dbo.DoubleOrders)" +
                " ORDER BY ShortName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(FirstOperatorsDT);
            }
            SelectCommand = "SELECT UserID, ShortName FROM Users" +
                " WHERE UserID IN (SELECT DISTINCT SecondOperatorID FROM infiniu2_zovorders.dbo.DoubleOrders)" +
                " ORDER BY ShortName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(SecondOperatorsDT);
            }
        }

        private void Binding()
        {
            FirstOperatorStatisticsBS.DataSource = FirstOperatorStatisticsDT;
            SecondOperatorStatisticsBS.DataSource = SecondOperatorStatisticsDT;
            FirstOperatorsBS.DataSource = FirstOperatorsDT;
            SecondOperatorsBS.DataSource = SecondOperatorsDT;
        }

        public bool HasFirstOperators
        {
            get
            {
                return FirstOperatorsBS.Count > 0;
            }
        }

        public bool HasSecondOperators
        {
            get
            {
                return SecondOperatorsBS.Count > 0;
            }
        }

        public BindingSource FirstOperatorStatisticsList
        {
            get { return FirstOperatorStatisticsBS; }
        }

        public BindingSource SecondOperatorStatisticsList
        {
            get { return SecondOperatorStatisticsBS; }
        }

        public BindingSource FirstOperatorsList
        {
            get { return FirstOperatorsBS; }
        }

        public BindingSource SecondOperatorsList
        {
            get { return SecondOperatorsBS; }
        }

        public void ClearFirstOperatorStatisticsDT()
        {
            FirstOperatorStatisticsDT.Clear();
        }

        public void ClearSecondOperatorStatisticsDT()
        {
            SecondOperatorStatisticsDT.Clear();
        }

        //public void ClearFirstOperatorsDT()
        //{
        //    FirstOperatorsDT.Clear();
        //}

        //public void ClearSecondOperatorsDT()
        //{
        //    SecondOperatorsDT.Clear();
        //}

        //public void UpdateFirstOperators(string OperatorID)
        //{
        //    string SelectCommand = "SELECT UserID, ShortName FROM Users" +
        //        " WHERE UserID IN (SELECT DISTINCT " + OperatorID + " FROM infiniu2_zovorders.dbo.DoubleOrders)" +
        //        " ORDER BY ShortName";
        //    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
        //    {
        //        DA.Fill(FirstOperatorsDT);
        //    }
        //}

        //public void UpdateSecondOperators(string OperatorID)
        //{
        //    string SelectCommand = "SELECT UserID, ShortName FROM Users" +
        //        " WHERE UserID IN (SELECT DISTINCT " + OperatorID + " FROM infiniu2_zovorders.dbo.DoubleOrders)" +
        //        " ORDER BY ShortName";
        //    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
        //    {
        //        DA.Fill(SecondOperatorsDT);
        //    }
        //}

        public void FirstOperatorOrders(int FirstOperatorID, DateTime FirstDate, DateTime SecondDate, bool bShowCorrectOrders)
        {
            string sDate = "CAST(FirstSaveDateTime AS DATE) >= '" + FirstDate.ToString("yyyy-MM-dd") + "'" +
                " AND CAST(FirstSaveDateTime AS DATE) <= '" + SecondDate.ToString("yyyy-MM-dd") + "'";
            string sFirstOperator = " AND DoubleOrders.FirstOperatorID = " + FirstOperatorID;
            string sShowCorrectOrders = string.Empty;

            if (bShowCorrectOrders)
                sShowCorrectOrders = " AND FirstErrors <> 0";

            string SelectCommand = "SELECT ClientName, DoubleOrders.DocNumber," +
                " FirstDocDateTime, FirstSaveDateTime, FirstErrors FROM DoubleOrders" +
                " INNER JOIN MainOrders ON DoubleOrders.DocNumber = MainOrders.DocNumber" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE " + sDate + sFirstOperator + sShowCorrectOrders +
                " ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FirstOperatorStatisticsDT);
            }
        }

        public void SecondOperatorOrders(int SecondOperatorID, DateTime FirstDate, DateTime SecondDate, bool bShowCorrectOrders)
        {
            string sDate = "CAST(SecondSaveDateTime AS DATE) >= '" + FirstDate.ToString("yyyy-MM-dd") + "'" +
                " AND CAST(SecondSaveDateTime AS DATE) <= '" + SecondDate.ToString("yyyy-MM-dd") + "'";
            string sSecondOperator = " AND DoubleOrders.SecondOperatorID = " + SecondOperatorID;
            string sShowCorrectOrders = string.Empty;

            if (bShowCorrectOrders)
                sShowCorrectOrders = " AND SecondErrors <> 0";

            string SelectCommand = "SELECT ClientName, DoubleOrders.DocNumber," +
                " SecondDocDateTime, SecondSaveDateTime, SecondErrors FROM DoubleOrders" +
                " INNER JOIN MainOrders ON DoubleOrders.DocNumber = MainOrders.DocNumber" +
                " INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID" +
                " WHERE " + sDate + sSecondOperator + sShowCorrectOrders +
                " ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SecondOperatorStatisticsDT);
            }
        }

        public void CalcFirstOperatorTotal(ref int Errors, ref TimeSpan t)
        {
            DateTime d1 = new DateTime();
            DateTime d2 = new DateTime();

            for (int i = 0; i < FirstOperatorStatisticsDT.Rows.Count; i++)
            {
                Errors += Convert.ToInt32(FirstOperatorStatisticsDT.Rows[i]["FirstErrors"]);
                d1 = Convert.ToDateTime(FirstOperatorStatisticsDT.Rows[i]["FirstDocDateTime"]);
                d2 = Convert.ToDateTime(FirstOperatorStatisticsDT.Rows[i]["FirstSaveDateTime"]);
                t += d2 - d1;
            }
        }

        public void CalcSecondOperatorTotal(ref int Errors, ref TimeSpan t)
        {
            DateTime d1 = new DateTime();
            DateTime d2 = new DateTime();

            for (int i = 0; i < SecondOperatorStatisticsDT.Rows.Count; i++)
            {
                Errors += Convert.ToInt32(SecondOperatorStatisticsDT.Rows[i]["SecondErrors"]);
                d1 = Convert.ToDateTime(SecondOperatorStatisticsDT.Rows[i]["SecondDocDateTime"]);
                d2 = Convert.ToDateTime(SecondOperatorStatisticsDT.Rows[i]["SecondSaveDateTime"]);
                t += d2 - d1;
            }
        }
    }
}
