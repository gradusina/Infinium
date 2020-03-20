using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium
{
    public enum TechStoreGroups
    {
        FacingMaterial = 1,
        FacingRollers = 11
    }

    public enum TechStoreSubGroups
    {
        Kashir = 9,
        MdfPlate = 5,
        PVA = 17,
        MilledProfile = 22,
        MilledAssembledProfile = 91,
        ShroudedAssembledProfile = 232,
        ShroudedProfile = 30,
        SawDSTP = 45,
        SawStrips = 44
    }


    public class ProfileAssignmentsLabels
    {
        private int iDecorAssignmentID;
        private int iFactTotalAmount = 0;
        private int iFactLabelsAmount = 0;
        private int iDisprepancyTotalAmount = 0;
        private int iDisprepancyLabelsAmount = 0;
        private int iDefectTotalAmount = 0;
        private int iDefectLabelsAmount = 0;

        DataTable DecorAssignmentsDT;
        DataTable FactLabelsDT;
        DataTable DisprepancyLabelsDT;
        DataTable DefectLabelsDT;
        DataTable ClientsDT;
        DataTable CoversDT;
        DataTable ResultDT;
        DataTable TechStoreDT;
        DataTable UsersDT;

        BindingSource FactLabelsBS;
        BindingSource DisprepancyLabelsBS;
        BindingSource DefectLabelsBS;

        public ProfileAssignmentsLabels()
        {

        }

        public void Initialize()
        {
            CreateFrontsDataTable();
            CreateCoversDT();
            Create();
            Fill();
            Binding();
            CalcFactLabelsAmount();
            CalcDisprepancyLabelsAmount();
            CalcDefectLabelsAmount();
        }

        private void CreateFrontsDataTable()
        {
            ResultDT = new DataTable();

            ResultDT.Columns.Add(new DataColumn(("Product"), System.Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn(("Color"), System.Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn(("Length"), System.Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.String")));
        }

        private void CreateCoversDT()
        {
            CoversDT = new DataTable();
            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore" +
                " WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)" +
                " ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DataRow EmptyRow = CoversDT.NewRow();
                    EmptyRow["CoverID"] = -1;
                    EmptyRow["CoverName"] = "-";
                    CoversDT.Rows.Add(EmptyRow);

                    DataRow ChoiceRow = CoversDT.NewRow();
                    ChoiceRow["CoverID"] = 0;
                    ChoiceRow["CoverName"] = "на выбор";
                    CoversDT.Rows.Add(ChoiceRow);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = CoversDT.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["CoverName"] = DT.Rows[i]["TechStoreName"].ToString();
                        CoversDT.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void Create()
        {
            ClientsDT = new DataTable();
            DecorAssignmentsDT = new DataTable();
            FactLabelsDT = new DataTable();
            DisprepancyLabelsDT = new DataTable();
            DefectLabelsDT = new DataTable();
            TechStoreDT = new DataTable();
            UsersDT = new DataTable();

            FactLabelsBS = new BindingSource();
            DisprepancyLabelsBS = new BindingSource();
            DefectLabelsBS = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT DecorAssignmentsLabels.*, BatchAssignmentID, ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID2, Height2, Length2, 
                Width2, Thickness2, Diameter2, CoverID2 FROM DecorAssignmentsLabels
                INNER JOIN DecorAssignments ON DecorAssignmentsLabels.DecorAssignmentID=DecorAssignments.DecorAssignmentID


WHERE LabelType=0 AND DecorAssignmentsLabels.DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FactLabelsDT);
            }
            SelectCommand = @"SELECT DecorAssignmentsLabels.*, BatchAssignmentID, ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID2, Height2, Length2, 
                Width2, Thickness2, Diameter2, CoverID2 FROM DecorAssignmentsLabels
                INNER JOIN DecorAssignments ON DecorAssignmentsLabels.DecorAssignmentID=DecorAssignments.DecorAssignmentID


WHERE LabelType=1 AND DecorAssignmentsLabels.DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DisprepancyLabelsDT);
            }
            SelectCommand = @"SELECT DecorAssignmentsLabels.*, BatchAssignmentID, ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID2, Height2, Length2, 
                Width2, Thickness2, Diameter2, CoverID2 FROM DecorAssignmentsLabels
                INNER JOIN DecorAssignments ON DecorAssignmentsLabels.DecorAssignmentID=DecorAssignments.DecorAssignmentID


WHERE LabelType=2 AND DecorAssignmentsLabels.DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DefectLabelsDT);
            }

            SelectCommand = @"SELECT * FROM DecorAssignments WHERE DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DecorAssignmentsDT);
            }
            if (DecorAssignmentsDT.Rows.Count > 0 && DecorAssignmentsDT.Rows[0]["FactCount"] != DBNull.Value)
                iFactTotalAmount = Convert.ToInt32(DecorAssignmentsDT.Rows[0]["FactCount"]);
            if (DecorAssignmentsDT.Rows.Count > 0 && DecorAssignmentsDT.Rows[0]["DisprepancyCount"] != DBNull.Value)
                iDisprepancyTotalAmount = Convert.ToInt32(DecorAssignmentsDT.Rows[0]["DisprepancyCount"]);
            if (DecorAssignmentsDT.Rows.Count > 0 && DecorAssignmentsDT.Rows[0]["DefectCount"] != DBNull.Value)
                iDefectTotalAmount = Convert.ToInt32(DecorAssignmentsDT.Rows[0]["DefectCount"]);
            SelectCommand = @"SELECT UserID, Name, ShortName FROM Users";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            SelectCommand = @"SELECT ClientID, ClientName FROM Clients";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                DataRow NewRow = ClientsDT.NewRow();
                NewRow["ClientID"] = 0;
                NewRow["ClientName"] = "СКЛАД";
                ClientsDT.Rows.Add(NewRow);
            }
            SelectCommand = @"SELECT TechStoreID, TechStore.TechStoreSubGroupID, TechStoreSubGroups.TechStoreGroupID, TechStoreName FROM TechStore
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID=TechStoreSubGroups.TechStoreSubGroupID
                INNER JOIN TechStoreGroups ON TechStoreSubGroups.TechStoreGroupID=TechStoreGroups.TechStoreGroupID
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDT);
            }
        }

        public int CurrentDecorAssignmentID
        {
            get { return iDecorAssignmentID; }
            set { iDecorAssignmentID = value; }
        }

        public int FactTotalAmount
        {
            get { return iFactTotalAmount; }
        }

        public int FactRestAmount
        {
            get { return iFactTotalAmount - iFactLabelsAmount; }
        }

        public int DisprepancyTotalAmount
        {
            get { return iDisprepancyTotalAmount; }
        }

        public int DisprepancyRestAmount
        {
            get { return iDisprepancyTotalAmount - iDisprepancyLabelsAmount; }
        }

        public int DefectTotalAmount
        {
            get { return iDefectTotalAmount; }
        }

        public int DefectRestAmount
        {
            get { return iDefectTotalAmount - iDefectLabelsAmount; }
        }

        public BindingSource FactLabelsList
        {
            get { return FactLabelsBS; }
        }

        public BindingSource DisprepancyLabelsList
        {
            get { return DisprepancyLabelsBS; }
        }

        public BindingSource DefectLabelsList
        {
            get { return DefectLabelsBS; }
        }

        public DataGridViewComboBoxColumn ClientNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientNameColumn",
                    HeaderText = "Клиент",
                    DataPropertyName = "ClientID",
                    DataSource = new DataView(ClientsDT),
                    ValueMember = "ClientID",
                    DisplayMember = "ClientName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CoversColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CoversColumn",
                    HeaderText = "Цвет",
                    DataPropertyName = "CoverID2",
                    DataSource = new DataView(CoversDT),
                    ValueMember = "CoverID",
                    DisplayMember = "CoverName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechStoreColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechStoreColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        private void Binding()
        {
            FactLabelsBS.DataSource = FactLabelsDT;
            DisprepancyLabelsBS.DataSource = DisprepancyLabelsDT;
            DefectLabelsBS.DataSource = DefectLabelsDT;
        }

        private void CalcFactLabelsAmount()
        {
            iFactLabelsAmount = 0;
            for (int i = 0; i < FactLabelsDT.Rows.Count; i++)
            {
                if (FactLabelsDT.Rows[i].RowState != DataRowState.Deleted && FactLabelsDT.Rows[i]["Count"] != DBNull.Value)
                    iFactLabelsAmount += Convert.ToInt32(FactLabelsDT.Rows[i]["Count"]);
            }
        }

        private void CalcDisprepancyLabelsAmount()
        {
            iDisprepancyLabelsAmount = 0;
            for (int i = 0; i < DisprepancyLabelsDT.Rows.Count; i++)
            {
                if (DisprepancyLabelsDT.Rows[i].RowState != DataRowState.Deleted && DisprepancyLabelsDT.Rows[i]["Count"] != DBNull.Value)
                    iDisprepancyLabelsAmount += Convert.ToInt32(DisprepancyLabelsDT.Rows[i]["Count"]);
            }
        }

        private void CalcDefectLabelsAmount()
        {
            iDefectLabelsAmount = 0;
            for (int i = 0; i < DefectLabelsDT.Rows.Count; i++)
            {
                if (DefectLabelsDT.Rows[i].RowState != DataRowState.Deleted && DefectLabelsDT.Rows[i]["Count"] != DBNull.Value)
                    iDefectLabelsAmount += Convert.ToInt32(DefectLabelsDT.Rows[i]["Count"]);
            }
        }

        public void FillFactLabelsTable()
        {
            for (int i = 0; i < FactLabelsDT.Rows.Count; i++)
            {
                FactLabelsDT.Rows[i]["BatchAssignmentID"] = DecorAssignmentsDT.Rows[0]["BatchAssignmentID"];
                FactLabelsDT.Rows[i]["ClientID"] = DecorAssignmentsDT.Rows[0]["ClientID"];
                FactLabelsDT.Rows[i]["MegaOrderID"] = DecorAssignmentsDT.Rows[0]["MegaOrderID"];
                FactLabelsDT.Rows[i]["MainOrderID"] = DecorAssignmentsDT.Rows[0]["MainOrderID"];
                FactLabelsDT.Rows[i]["DecorOrderID"] = DecorAssignmentsDT.Rows[0]["DecorOrderID"];
                FactLabelsDT.Rows[i]["TechStoreID2"] = DecorAssignmentsDT.Rows[0]["TechStoreID2"];
                FactLabelsDT.Rows[i]["Height2"] = DecorAssignmentsDT.Rows[0]["Height2"];
                FactLabelsDT.Rows[i]["Length2"] = DecorAssignmentsDT.Rows[0]["Length2"];
                FactLabelsDT.Rows[i]["Width2"] = DecorAssignmentsDT.Rows[0]["Width2"];
                FactLabelsDT.Rows[i]["Thickness2"] = DecorAssignmentsDT.Rows[0]["Thickness2"];
                FactLabelsDT.Rows[i]["Diameter2"] = DecorAssignmentsDT.Rows[0]["Diameter2"];
                FactLabelsDT.Rows[i]["CoverID2"] = DecorAssignmentsDT.Rows[0]["CoverID2"];
                //FactLabelsDT.Rows[i]["Notes"] = DecorAssignmentsDT.Rows[0]["Notes"];
            }
        }

        public void FillDisprepancyLabelsTable()
        {
            for (int i = 0; i < DisprepancyLabelsDT.Rows.Count; i++)
            {
                DisprepancyLabelsDT.Rows[i]["BatchAssignmentID"] = DecorAssignmentsDT.Rows[0]["BatchAssignmentID"];
                DisprepancyLabelsDT.Rows[i]["ClientID"] = DecorAssignmentsDT.Rows[0]["ClientID"];
                DisprepancyLabelsDT.Rows[i]["MegaOrderID"] = DecorAssignmentsDT.Rows[0]["MegaOrderID"];
                DisprepancyLabelsDT.Rows[i]["MainOrderID"] = DecorAssignmentsDT.Rows[0]["MainOrderID"];
                DisprepancyLabelsDT.Rows[i]["DecorOrderID"] = DecorAssignmentsDT.Rows[0]["DecorOrderID"];
                DisprepancyLabelsDT.Rows[i]["TechStoreID2"] = DecorAssignmentsDT.Rows[0]["TechStoreID2"];
                DisprepancyLabelsDT.Rows[i]["Height2"] = DecorAssignmentsDT.Rows[0]["Height2"];
                DisprepancyLabelsDT.Rows[i]["Length2"] = DecorAssignmentsDT.Rows[0]["Length2"];
                DisprepancyLabelsDT.Rows[i]["Width2"] = DecorAssignmentsDT.Rows[0]["Width2"];
                DisprepancyLabelsDT.Rows[i]["Thickness2"] = DecorAssignmentsDT.Rows[0]["Thickness2"];
                DisprepancyLabelsDT.Rows[i]["Diameter2"] = DecorAssignmentsDT.Rows[0]["Diameter2"];
                DisprepancyLabelsDT.Rows[i]["CoverID2"] = DecorAssignmentsDT.Rows[0]["CoverID2"];
                //DisprepancyLabelsDT.Rows[i]["Notes"] = DecorAssignmentsDT.Rows[0]["Notes"];
            }
        }

        public void FillDefectLabelsTable()
        {
            for (int i = 0; i < DefectLabelsDT.Rows.Count; i++)
            {
                DefectLabelsDT.Rows[i]["BatchAssignmentID"] = DecorAssignmentsDT.Rows[0]["BatchAssignmentID"];
                DefectLabelsDT.Rows[i]["ClientID"] = DecorAssignmentsDT.Rows[0]["ClientID"];
                DefectLabelsDT.Rows[i]["MegaOrderID"] = DecorAssignmentsDT.Rows[0]["MegaOrderID"];
                DefectLabelsDT.Rows[i]["MainOrderID"] = DecorAssignmentsDT.Rows[0]["MainOrderID"];
                DefectLabelsDT.Rows[i]["DecorOrderID"] = DecorAssignmentsDT.Rows[0]["DecorOrderID"];
                DefectLabelsDT.Rows[i]["TechStoreID2"] = DecorAssignmentsDT.Rows[0]["TechStoreID2"];
                DefectLabelsDT.Rows[i]["Height2"] = DecorAssignmentsDT.Rows[0]["Height2"];
                DefectLabelsDT.Rows[i]["Length2"] = DecorAssignmentsDT.Rows[0]["Length2"];
                DefectLabelsDT.Rows[i]["Width2"] = DecorAssignmentsDT.Rows[0]["Width2"];
                DefectLabelsDT.Rows[i]["Thickness2"] = DecorAssignmentsDT.Rows[0]["Thickness2"];
                DefectLabelsDT.Rows[i]["Diameter2"] = DecorAssignmentsDT.Rows[0]["Diameter2"];
                DefectLabelsDT.Rows[i]["CoverID2"] = DecorAssignmentsDT.Rows[0]["CoverID2"];
                //DefectLabelsDT.Rows[i]["Notes"] = DecorAssignmentsDT.Rows[0]["Notes"];
            }
        }

        public void UpdateFactLabels()
        {
            string SelectCommand = @"SELECT DecorAssignmentsLabels.*, BatchAssignmentID, ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID2, Height2, Length2, 
                Width2, Thickness2, Diameter2, CoverID2 FROM DecorAssignmentsLabels
                INNER JOIN DecorAssignments ON DecorAssignmentsLabels.DecorAssignmentID=DecorAssignments.DecorAssignmentID
WHERE LabelType=0 AND DecorAssignmentsLabels.DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                FactLabelsDT.Clear();
                DA.Fill(FactLabelsDT);
            }
        }

        public void UpdateDisprepancyLabels()
        {
            string SelectCommand = @"SELECT DecorAssignmentsLabels.*, BatchAssignmentID, ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID2, Height2, Length2, 
                Width2, Thickness2, Diameter2, CoverID2 FROM DecorAssignmentsLabels
                INNER JOIN DecorAssignments ON DecorAssignmentsLabels.DecorAssignmentID=DecorAssignments.DecorAssignmentID
WHERE LabelType=1 AND DecorAssignmentsLabels.DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DisprepancyLabelsDT.Clear();
                DA.Fill(DisprepancyLabelsDT);
            }
        }

        public void UpdateDefectLabels()
        {
            string SelectCommand = @"SELECT DecorAssignmentsLabels.*, BatchAssignmentID, ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID2, Height2, Length2, 
                Width2, Thickness2, Diameter2, CoverID2 FROM DecorAssignmentsLabels
                INNER JOIN DecorAssignments ON DecorAssignmentsLabels.DecorAssignmentID=DecorAssignments.DecorAssignmentID
WHERE LabelType=2 AND DecorAssignmentsLabels.DecorAssignmentID=" + iDecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DefectLabelsDT.Clear();
                DA.Fill(DefectLabelsDT);
            }
        }

        public void SaveFactLabels()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM DecorAssignmentsLabels";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(FactLabelsDT);
                }
            }
        }

        public void SaveDisprepancyLabels()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM DecorAssignmentsLabels";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DisprepancyLabelsDT);
                }
            }
        }

        public void SaveDefectLabels()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM DecorAssignmentsLabels";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DefectLabelsDT);
                }
            }
        }

        public void CreateFactLabels(int LabelsAmount, int PositionsAmount, string Notes)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            for (int i = 0; i < LabelsAmount; i++)
            {
                CalcFactLabelsAmount();
                if (iFactTotalAmount < iFactLabelsAmount + PositionsAmount)
                {
                    CalcFactLabelsAmount();
                    return;
                }
                DataRow NewRow = FactLabelsDT.NewRow();
                NewRow["BatchAssignmentID"] = DecorAssignmentsDT.Rows[0]["BatchAssignmentID"];
                NewRow["ClientID"] = DecorAssignmentsDT.Rows[0]["ClientID"];
                NewRow["MegaOrderID"] = DecorAssignmentsDT.Rows[0]["MegaOrderID"];
                NewRow["MainOrderID"] = DecorAssignmentsDT.Rows[0]["MainOrderID"];
                NewRow["DecorOrderID"] = DecorAssignmentsDT.Rows[0]["DecorOrderID"];
                NewRow["TechStoreID2"] = DecorAssignmentsDT.Rows[0]["TechStoreID2"];
                NewRow["Height2"] = DecorAssignmentsDT.Rows[0]["Height2"];
                NewRow["Length2"] = DecorAssignmentsDT.Rows[0]["Length2"];
                NewRow["Width2"] = DecorAssignmentsDT.Rows[0]["Width2"];
                NewRow["Thickness2"] = DecorAssignmentsDT.Rows[0]["Thickness2"];
                NewRow["Diameter2"] = DecorAssignmentsDT.Rows[0]["Diameter2"];
                NewRow["CoverID2"] = DecorAssignmentsDT.Rows[0]["CoverID2"];
                NewRow["DecorAssignmentID"] = iDecorAssignmentID;
                NewRow["CreationDateTime"] = CurrentDate;
                NewRow["CreationUserID"] = Security.CurrentUserID;
                NewRow["Count"] = PositionsAmount;
                NewRow["LabelType"] = 0;
                NewRow["Notes"] = Notes;
                FactLabelsDT.Rows.Add(NewRow);
            }
            CalcFactLabelsAmount();
        }

        public void CreateDisprepancyLabels(int LabelsAmount, int PositionsAmount, string Notes)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            for (int i = 0; i < LabelsAmount; i++)
            {
                CalcDisprepancyLabelsAmount();
                if (iDisprepancyTotalAmount < iDisprepancyLabelsAmount + PositionsAmount)
                {
                    CalcDisprepancyLabelsAmount();
                    return;
                }
                DataRow NewRow = DisprepancyLabelsDT.NewRow();
                NewRow["BatchAssignmentID"] = DecorAssignmentsDT.Rows[0]["BatchAssignmentID"];
                NewRow["ClientID"] = DecorAssignmentsDT.Rows[0]["ClientID"];
                NewRow["MegaOrderID"] = DecorAssignmentsDT.Rows[0]["MegaOrderID"];
                NewRow["MainOrderID"] = DecorAssignmentsDT.Rows[0]["MainOrderID"];
                NewRow["DecorOrderID"] = DecorAssignmentsDT.Rows[0]["DecorOrderID"];
                NewRow["TechStoreID2"] = DecorAssignmentsDT.Rows[0]["TechStoreID2"];
                NewRow["Height2"] = DecorAssignmentsDT.Rows[0]["Height2"];
                NewRow["Length2"] = DecorAssignmentsDT.Rows[0]["Length2"];
                NewRow["Width2"] = DecorAssignmentsDT.Rows[0]["Width2"];
                NewRow["Thickness2"] = DecorAssignmentsDT.Rows[0]["Thickness2"];
                NewRow["Diameter2"] = DecorAssignmentsDT.Rows[0]["Diameter2"];
                NewRow["CoverID2"] = DecorAssignmentsDT.Rows[0]["CoverID2"];
                NewRow["DecorAssignmentID"] = iDecorAssignmentID;
                NewRow["CreationDateTime"] = CurrentDate;
                NewRow["CreationUserID"] = Security.CurrentUserID;
                NewRow["Count"] = PositionsAmount;
                NewRow["LabelType"] = 1;
                NewRow["Notes"] = Notes;
                DisprepancyLabelsDT.Rows.Add(NewRow);
            }
            CalcDisprepancyLabelsAmount();
        }

        public void CreateDefectLabels(int LabelsAmount, int PositionsAmount, string Notes)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            for (int i = 0; i < LabelsAmount; i++)
            {
                CalcDefectLabelsAmount();
                if (iDefectTotalAmount < iDefectLabelsAmount + PositionsAmount)
                {
                    CalcDefectLabelsAmount();
                    return;
                }
                DataRow NewRow = DefectLabelsDT.NewRow();
                NewRow["BatchAssignmentID"] = DecorAssignmentsDT.Rows[0]["BatchAssignmentID"];
                NewRow["ClientID"] = DecorAssignmentsDT.Rows[0]["ClientID"];
                NewRow["MegaOrderID"] = DecorAssignmentsDT.Rows[0]["MegaOrderID"];
                NewRow["MainOrderID"] = DecorAssignmentsDT.Rows[0]["MainOrderID"];
                NewRow["DecorOrderID"] = DecorAssignmentsDT.Rows[0]["DecorOrderID"];
                NewRow["TechStoreID2"] = DecorAssignmentsDT.Rows[0]["TechStoreID2"];
                NewRow["Height2"] = DecorAssignmentsDT.Rows[0]["Height2"];
                NewRow["Length2"] = DecorAssignmentsDT.Rows[0]["Length2"];
                NewRow["Width2"] = DecorAssignmentsDT.Rows[0]["Width2"];
                NewRow["Thickness2"] = DecorAssignmentsDT.Rows[0]["Thickness2"];
                NewRow["Diameter2"] = DecorAssignmentsDT.Rows[0]["Diameter2"];
                NewRow["CoverID2"] = DecorAssignmentsDT.Rows[0]["CoverID2"];
                NewRow["DecorAssignmentID"] = iDecorAssignmentID;
                NewRow["CreationDateTime"] = CurrentDate;
                NewRow["CreationUserID"] = Security.CurrentUserID;
                NewRow["Count"] = PositionsAmount;
                NewRow["LabelType"] = 2;
                NewRow["Notes"] = Notes;
                DefectLabelsDT.Rows.Add(NewRow);
            }
            CalcDefectLabelsAmount();
        }

        public void RemoveFactLabel()
        {
            if (FactLabelsBS.Current != null)
                FactLabelsBS.RemoveCurrent();
            CalcFactLabelsAmount();
        }

        public void RemoveDisprepancyLabel()
        {
            if (DisprepancyLabelsBS.Current != null)
                DisprepancyLabelsBS.RemoveCurrent();
            CalcDisprepancyLabelsAmount();
        }

        public void RemoveDefectLabel()
        {
            if (DefectLabelsBS.Current != null)
                DefectLabelsBS.RemoveCurrent();
            CalcDefectLabelsAmount();
        }

        public void RemoveFactLabels(int[] DecorAssignmentsLabelID)
        {
            DataRow[] rows = FactLabelsDT.Select("DecorAssignmentsLabelID IN (" + string.Join(",", DecorAssignmentsLabelID) + ")");
            for (int i = rows.Count() - 1; i >= 0; i--)
            {
                rows[i].Delete();
            }
            CalcFactLabelsAmount();
        }

        public void RemoveDisprepancyLabels(int[] DecorAssignmentsLabelID)
        {
            DataRow[] rows = DisprepancyLabelsDT.Select("DecorAssignmentsLabelID IN (" + string.Join(",", DecorAssignmentsLabelID) + ")");
            for (int i = rows.Count() - 1; i >= 0; i--)
            {
                rows[i].Delete();
            }
            CalcDisprepancyLabelsAmount();
        }

        public void RemoveDefectLabels(int[] DecorAssignmentsLabelID)
        {
            DataRow[] rows = DefectLabelsDT.Select("DecorAssignmentsLabelID IN (" + string.Join(",", DecorAssignmentsLabelID) + ")");
            for (int i = rows.Count() - 1; i >= 0; i--)
            {
                rows[i].Delete();
            }
            CalcDefectLabelsAmount();
        }

        public void PrintFactLabels(int[] DecorAssignmentsLabelID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = FactLabelsDT.Select("DecorAssignmentsLabelID IN (" + string.Join(",", DecorAssignmentsLabelID) + ")");
            foreach (DataRow item in rows)
            {
                if (item["PrintUserID"] == DBNull.Value)
                    item["PrintUserID"] = Security.CurrentUserID;
                if (item["PrintDateTime"] == DBNull.Value)
                    item["PrintDateTime"] = CurrentDate;
            }
        }

        public void PrintDisprepancyLabels(int[] DecorAssignmentsLabelID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = DisprepancyLabelsDT.Select("DecorAssignmentsLabelID IN (" + string.Join(",", DecorAssignmentsLabelID) + ")");
            foreach (DataRow item in rows)
            {
                if (item["PrintUserID"] == DBNull.Value)
                    item["PrintUserID"] = Security.CurrentUserID;
                if (item["PrintDateTime"] == DBNull.Value)
                    item["PrintDateTime"] = CurrentDate;
            }
        }

        public void PrintDefectLabels(int[] DecorAssignmentsLabelID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = DefectLabelsDT.Select("DecorAssignmentsLabelID IN (" + string.Join(",", DecorAssignmentsLabelID) + ")");
            foreach (DataRow item in rows)
            {
                if (item["PrintUserID"] == DBNull.Value)
                    item["PrintUserID"] = Security.CurrentUserID;
                if (item["PrintDateTime"] == DBNull.Value)
                    item["PrintDateTime"] = CurrentDate;
            }
        }

        public DataTable FillLabelsTable(string Product, string Color, int Length = 0, int Height = 0, int Width = 0, int Count = 0)
        {
            ResultDT.Clear();
            DataRow NewRow = ResultDT.NewRow();
            NewRow["Product"] = Product;
            NewRow["Color"] = Color;
            NewRow["Length"] = Length;
            NewRow["Height"] = Height;
            NewRow["Width"] = Width;
            NewRow["Count"] = Count;
            ResultDT.Rows.Add(NewRow);
            return ResultDT.Copy();
        }

    }

    public struct ProfileAssignmentsLabelInfo
    {
        public string ClientName;
        public int ClientID;
        public int MegaOrderID;
        public int MainOrderID;
        public int PackNumber;
        public int TotalPackCount;
        public string DocDateTime;
        public string BarcodeNumber;
        public string Notes;
        public DataTable LabelsDT;
        public int FactoryType;
        public int LabelType;
        public string GroupType;
        public bool NeedUserName;
        public string UserName;
        public int BatchID;
        public int NumberOfChange;
        public int WeekNumber;
    }

    public class PrintDefectProfileAssignmentsLabels
    {
        Infinium.Modules.Packages.Marketing.Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        //public int PaperHeight = 394;
        //public int PaperWidth = 488;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font DefectFont;
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

        public PrintDefectProfileAssignmentsLabels()
        {
            Barcode = new Modules.Packages.Marketing.Barcode();

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
            DefectFont = new Font("Arial", 15.0f, FontStyle.Regular);
            ClientFont = new Font("Arial", 18.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 12.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

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

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        private void DrawDefectLabelsTable(PrintPageEventArgs ev, int indent, int LabelNumber)
        {
            float HeaderTopY = indent + 37;
            float OrderTopY = indent + 50;
            float TopLineY = indent + 30;
            float BottomLineY = TopLineY + 48;

            ev.Graphics.DrawLine(Pen, 11, indent + 30, 467, indent + 30);
            string Product = ((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows[0]["Product"].ToString();
            //Product = Product.Substring(0, Product.IndexOf(' '));
            ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, indent);
            //header
            //ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Длин.", HeaderFont, FontBrush, 313, HeaderTopY);
            ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, 353, HeaderTopY);
            ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, 393, HeaderTopY);
            ev.Graphics.DrawString("Кол.", HeaderFont, FontBrush, 430, HeaderTopY);


            ev.Graphics.DrawLine(Pen, 11, TopLineY, 11, BottomLineY);
            //ev.Graphics.DrawLine(Pen, 205, TopLineY, 205, BottomLineY);
            ev.Graphics.DrawLine(Pen, 311, TopLineY, 311, BottomLineY);
            ev.Graphics.DrawLine(Pen, 352, TopLineY, 352, BottomLineY);
            ev.Graphics.DrawLine(Pen, 392, TopLineY, 392, BottomLineY);
            ev.Graphics.DrawLine(Pen, 429, TopLineY, 429, BottomLineY);
            ev.Graphics.DrawLine(Pen, 467, TopLineY, 467, BottomLineY);

            for (int i = 0, p = indent + 54; i <= ((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                if (i != 6)
                    ev.Graphics.DrawLine(Pen, 11, p, 467, p);
            }

            for (int i = 0, p = 10; i < ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                //ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Product"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, 313, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, 353, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, 393, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[LabelNumber]).LabelsDT.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, 430, OrderTopY + p);
            }
        }

        private void DrawFactLabelsTable(PrintPageEventArgs ev, int indent)
        {
            float HeaderTopY = indent + 151;
            float OrderTopY = indent + 164;
            float TopLineY = indent + 144;
            float BottomLineY = 315;

            ev.Graphics.DrawLine(Pen, 11, indent + 113, 467, indent + 113);
            string Product = ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[0]["Product"].ToString();
            //Product = Product.Substring(0, Product.IndexOf(' '));
            ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, indent + 114);
            //header
            //ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Длин.", HeaderFont, FontBrush, 313, HeaderTopY);
            ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, 353, HeaderTopY);
            ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, 393, HeaderTopY);
            ev.Graphics.DrawString("Кол.", HeaderFont, FontBrush, 430, HeaderTopY);


            ev.Graphics.DrawLine(Pen, 11, TopLineY, 11, BottomLineY);
            //ev.Graphics.DrawLine(Pen, 205, TopLineY, 205, BottomLineY);
            ev.Graphics.DrawLine(Pen, 311, TopLineY, 311, BottomLineY);
            ev.Graphics.DrawLine(Pen, 352, TopLineY, 352, BottomLineY);
            ev.Graphics.DrawLine(Pen, 392, TopLineY, 392, BottomLineY);
            ev.Graphics.DrawLine(Pen, 429, TopLineY, 429, BottomLineY);
            ev.Graphics.DrawLine(Pen, 467, TopLineY, 467, BottomLineY);

            for (int i = 0, p = indent + 168; i <= ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                if (i != 6)
                    ev.Graphics.DrawLine(Pen, 11, p, 467, p);
            }

            for (int i = 0, p = 10; i < ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                //ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Product"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, 313, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, 353, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, 393, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, 430, OrderTopY + p);
            }
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref ProfileAssignmentsLabelInfo LabelInfoItem)
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
            int indent = 0;
            //if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).NeedUserName)
            //{
            //    ev.Graphics.DrawString("Партия №" + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BatchID + ", " +
            //        ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).WeekNumber + " к.н.", ClientFont, FontBrush, 8, 6);
            //    ev.Graphics.DrawLine(Pen, 11, 33, 467, 33);
            //    ev.Graphics.DrawString("Изгот: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).DocDateTime + "    " +
            //        "Смена: 0" + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).NumberOfChange.ToString() + "    " +
            //        "Отв.: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).UserName, DecorOrderFont, FontBrush, 8, 37);

            //    indent = 0;
            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
            //        ev.Graphics.DrawString("НЕКОНДИЦИЯ", ClientFont, FontBrush, 289, indent + 6);
            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
            //        ev.Graphics.DrawString("БРАК", ClientFont, FontBrush, 371, indent + 6);
            //    ev.Graphics.DrawLine(Pen, 11, indent + 33, 467, indent + 33);
            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 0)
            //    {
            //        //ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 70);

            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 37)
            //            ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, indent + 8);
            //        else
            //            ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes, NotesFont, FontBrush, 7, indent + 8);
            //    }
            //    ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 15, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 342, indent + 37, 130, 15);
            //    indent = indent + 34;
            //    //ev.Graphics.DrawLine(Pen, 11, indent, 467, indent);
            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
            //        DrawDefectLabelsTable(ev, indent - 1);
            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
            //        DrawDefectLabelsTable(ev, indent - 1);
            //    indent = indent + 80;

            //    ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, 46, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, indent);
            //    Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, ev.Graphics, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, indent + 49);
            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            //        ev.Graphics.DrawImage(ZTTPS, 249, indent + 3, 37, 45);
            //    else
            //        ev.Graphics.DrawImage(ZTProfil, 249, indent + 3, 37, 45);
            //    ev.Graphics.DrawImage(STB, 418, indent + 2, 39, 27);
            //    ev.Graphics.DrawImage(RST, 423, indent + 40, 34, 27);
            //    //ev.Graphics.DrawLine(Pen, 11, indent - 2, 467, indent - 2);
            //    ev.Graphics.DrawLine(Pen, 235, indent - 2, 235, indent + 68);
            //    ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, indent + 3);

            //    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            //        ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, indent + 15);
            //    else
            //        ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, indent + 15);
            //    ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, indent + 27);
            //    ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, indent + 39);
            //    ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, indent + 51);
            //    ev.Graphics.DrawString("Изготовлено: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, InfoFont, FontBrush, 305, indent + 63);
            //    ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).GroupType, ClientFont, FontBrush, 252, indent + 47);

            //    if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            //    {
            //        indent = 184;
            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 1)
            //            ev.Graphics.DrawString("НЕКОНДИЦИЯ", ClientFont, FontBrush, 289, indent + 6);
            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 2)
            //            ev.Graphics.DrawString("БРАК", ClientFont, FontBrush, 371, indent + 6);
            //        ev.Graphics.DrawLine(Pen, 11, indent + 33, 467, indent + 33);
            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes.Length > 0)
            //        {
            //            //ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 70);

            //            if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes.Length > 37)
            //                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, indent + 8);
            //            else
            //                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes, NotesFont, FontBrush, 7, indent + 8);
            //        }
            //        ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 15, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber), 342, indent + 37, 130, 15);
            //        indent = indent + 34;
            //        //ev.Graphics.DrawLine(Pen, 11, indent, 467, indent);
            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 1)
            //            DrawDefectLabelsTable(ev, indent - 1);
            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 2)
            //            DrawDefectLabelsTable(ev, indent - 1);
            //        indent = indent + 80;

            //        ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, 46, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber), 10, indent);
            //        Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, ev.Graphics, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber, 9, indent + 49);
            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).FactoryType == 2)
            //            ev.Graphics.DrawImage(ZTTPS, 249, indent + 3, 37, 45);
            //        else
            //            ev.Graphics.DrawImage(ZTProfil, 249, indent + 3, 37, 45);
            //        ev.Graphics.DrawImage(STB, 418, indent + 2, 39, 27);
            //        ev.Graphics.DrawImage(RST, 423, indent + 40, 34, 27);
            //        //ev.Graphics.DrawLine(Pen, 11, indent - 2, 467, indent - 2);
            //        ev.Graphics.DrawLine(Pen, 235, indent - 2, 235, indent + 68);
            //        ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, indent + 3);

            //        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).FactoryType == 2)
            //            ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, indent + 15);
            //        else
            //            ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, indent + 15);
            //        ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, indent + 27);
            //        ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, indent + 39);
            //        ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, indent + 51);
            //        ev.Graphics.DrawString("Изготовлено: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).DocDateTime, InfoFont, FontBrush, 305, indent + 63);
            //        ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).GroupType, ClientFont, FontBrush, 252, indent + 47);

            //        CurrentLabelNumber++;
            //    }
            //}

            {
                indent = 8;
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
                    ev.Graphics.DrawString("НЕКОНДИЦИЯ", ClientFont, FontBrush, 289, indent + 6);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
                    ev.Graphics.DrawString("БРАК", ClientFont, FontBrush, 371, indent + 6);
                ev.Graphics.DrawLine(Pen, 11, indent + 33, 467, indent + 33);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 0)
                {
                    //ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 70);

                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 37)
                        ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, indent + 8);
                    else
                        ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes, NotesFont, FontBrush, 7, indent + 8);
                }
                ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 15, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 342, indent + 37, 130, 15);
                indent = indent + 34;
                //ev.Graphics.DrawLine(Pen, 11, indent, 467, indent);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
                    DrawDefectLabelsTable(ev, indent - 1, CurrentLabelNumber);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
                    DrawDefectLabelsTable(ev, indent - 1, CurrentLabelNumber);
                indent = indent + 80;

                ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, 46, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, indent);
                Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, ev.Graphics, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, indent + 49);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                    ev.Graphics.DrawImage(ZTTPS, 249, indent + 3, 37, 45);
                else
                    ev.Graphics.DrawImage(ZTProfil, 249, indent + 3, 37, 45);
                ev.Graphics.DrawImage(STB, 418, indent + 2, 39, 27);
                ev.Graphics.DrawImage(RST, 423, indent + 40, 34, 27);
                //ev.Graphics.DrawLine(Pen, 11, indent - 2, 467, indent - 2);
                ev.Graphics.DrawLine(Pen, 235, indent - 2, 235, indent + 68);
                ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, indent + 3);

                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                    ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, indent + 15);
                else
                    ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, indent + 15);
                ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, indent + 27);
                ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, indent + 39);
                ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, indent + 51);
                ev.Graphics.DrawString("Изготовлено: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, InfoFont, FontBrush, 305, indent + 63);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).GroupType, ClientFont, FontBrush, 252, indent + 47);

                ev.Graphics.DrawLine(Pen, 11, indent + 78, 467, indent + 78);

                if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
                {
                    indent = 197;
                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 1)
                        ev.Graphics.DrawString("НЕКОНДИЦИЯ", ClientFont, FontBrush, 289, indent + 6);
                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 2)
                        ev.Graphics.DrawString("БРАК", ClientFont, FontBrush, 371, indent + 6);
                    ev.Graphics.DrawLine(Pen, 11, indent + 33, 467, indent + 33);
                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes.Length > 0)
                    {
                        //ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 70);

                        if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes.Length > 37)
                            ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, indent + 8);
                        else
                            ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).Notes, NotesFont, FontBrush, 7, indent + 8);
                    }
                    ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 15, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber), 342, indent + 37, 130, 15);
                    indent = indent + 34;
                    //ev.Graphics.DrawLine(Pen, 11, indent, 467, indent);
                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 1)
                        DrawDefectLabelsTable(ev, indent - 1, CurrentLabelNumber + 1);
                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).LabelType == 2)
                        DrawDefectLabelsTable(ev, indent - 1, CurrentLabelNumber + 1);
                    indent = indent + 80;

                    ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, 46, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber), 10, indent);
                    Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, ev.Graphics, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).BarcodeNumber, 9, indent + 49);
                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).FactoryType == 2)
                        ev.Graphics.DrawImage(ZTTPS, 249, indent + 3, 37, 45);
                    else
                        ev.Graphics.DrawImage(ZTProfil, 249, indent + 3, 37, 45);
                    ev.Graphics.DrawImage(STB, 418, indent + 2, 39, 27);
                    ev.Graphics.DrawImage(RST, 423, indent + 40, 34, 27);
                    //ev.Graphics.DrawLine(Pen, 11, indent - 2, 467, indent - 2);
                    ev.Graphics.DrawLine(Pen, 235, indent - 2, 235, indent + 68);
                    ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, indent + 3);

                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).FactoryType == 2)
                        ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, indent + 15);
                    else
                        ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, indent + 15);
                    ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, indent + 27);
                    ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, indent + 39);
                    ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, indent + 51);
                    ev.Graphics.DrawString("Изготовлено: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).DocDateTime, InfoFont, FontBrush, 305, indent + 63);
                    ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber + 1]).GroupType, ClientFont, FontBrush, 252, indent + 47);

                    CurrentLabelNumber++;
                }
            }

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

    public class PrintProfileAssignmentsLabels
    {
        Infinium.Modules.Packages.Marketing.Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        //public int PaperHeight = 794;
        //public int PaperWidth = 488;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font DefectFont;
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

        public PrintProfileAssignmentsLabels()
        {
            Barcode = new Modules.Packages.Marketing.Barcode();

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
            DefectFont = new Font("Arial", 15.0f, FontStyle.Regular);
            ClientFont = new Font("Arial", 18.0f, FontStyle.Regular);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 12.0f, FontStyle.Regular);
            HeaderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

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

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        private void DrawDefectLabelsTable(PrintPageEventArgs ev, int indent)
        {
            float HeaderTopY = indent + 151;
            float OrderTopY = indent + 164;
            float TopLineY = indent + 144;
            float BottomLineY = TopLineY + 48;

            ev.Graphics.DrawLine(Pen, 11, indent + 144, 467, indent + 144);
            string Product = ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[0]["Product"].ToString();
            //Product = Product.Substring(0, Product.IndexOf(' '));
            ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, indent + 114);
            //header
            //ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Длин.", HeaderFont, FontBrush, 313, HeaderTopY);
            ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, 353, HeaderTopY);
            ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, 393, HeaderTopY);
            ev.Graphics.DrawString("Кол.", HeaderFont, FontBrush, 430, HeaderTopY);


            ev.Graphics.DrawLine(Pen, 11, TopLineY, 11, BottomLineY);
            //ev.Graphics.DrawLine(Pen, 205, TopLineY, 205, BottomLineY);
            ev.Graphics.DrawLine(Pen, 311, TopLineY, 311, BottomLineY);
            ev.Graphics.DrawLine(Pen, 352, TopLineY, 352, BottomLineY);
            ev.Graphics.DrawLine(Pen, 392, TopLineY, 392, BottomLineY);
            ev.Graphics.DrawLine(Pen, 429, TopLineY, 429, BottomLineY);
            ev.Graphics.DrawLine(Pen, 467, TopLineY, 467, BottomLineY);

            for (int i = 0, p = indent + 168; i <= ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                if (i != 6)
                    ev.Graphics.DrawLine(Pen, 11, p, 467, p);
            }

            for (int i = 0, p = 10; i < ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                //ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Product"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, 313, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, 353, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, 393, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, 430, OrderTopY + p);
            }
        }

        private void DrawFactLabelsTable(PrintPageEventArgs ev, int indent)
        {
            float HeaderTopY = indent + 151;
            float OrderTopY = indent + 164;
            float TopLineY = indent + 144;
            float BottomLineY = 315;

            ev.Graphics.DrawLine(Pen, 11, indent + 113, 467, indent + 113);
            string Product = ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[0]["Product"].ToString();
            //Product = Product.Substring(0, Product.IndexOf(' '));
            ev.Graphics.DrawString(Product, DocFont, FontBrush, 7, indent + 114);
            //header
            //ev.Graphics.DrawString("Продукт", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Цвет", HeaderFont, FontBrush, 12, HeaderTopY);
            ev.Graphics.DrawString("Длин.", HeaderFont, FontBrush, 313, HeaderTopY);
            ev.Graphics.DrawString("Выс.", HeaderFont, FontBrush, 353, HeaderTopY);
            ev.Graphics.DrawString("Шир.", HeaderFont, FontBrush, 393, HeaderTopY);
            ev.Graphics.DrawString("Кол.", HeaderFont, FontBrush, 430, HeaderTopY);


            ev.Graphics.DrawLine(Pen, 11, TopLineY, 11, BottomLineY);
            //ev.Graphics.DrawLine(Pen, 205, TopLineY, 205, BottomLineY);
            ev.Graphics.DrawLine(Pen, 311, TopLineY, 311, BottomLineY);
            ev.Graphics.DrawLine(Pen, 352, TopLineY, 352, BottomLineY);
            ev.Graphics.DrawLine(Pen, 392, TopLineY, 392, BottomLineY);
            ev.Graphics.DrawLine(Pen, 429, TopLineY, 429, BottomLineY);
            ev.Graphics.DrawLine(Pen, 467, TopLineY, 467, BottomLineY);

            for (int i = 0, p = indent + 168; i <= ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                if (i != 6)
                    ev.Graphics.DrawLine(Pen, 11, p, 467, p);
            }

            for (int i = 0, p = 10; i < ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows.Count; i++, p += 24)
            {
                //ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Product"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Color"].ToString(), DecorOrderFont, FontBrush, 12, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, 313, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, 353, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, 393, OrderTopY + p);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelsDT.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, 430, OrderTopY + p);
            }
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref ProfileAssignmentsLabelInfo LabelInfoItem)
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
            int indent = 0;
            if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).NeedUserName)
            {
                indent = 15;
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
                    ev.Graphics.DrawString("НЕКОНДИЦИЯ", ClientFont, FontBrush, 289, 6);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
                    ev.Graphics.DrawString("БРАК", ClientFont, FontBrush, 371, 6);
                ev.Graphics.DrawLine(Pen, 11, 33, 467, 33);
                ev.Graphics.DrawString("Партия №" + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BatchID + ", " +
                    ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).WeekNumber + " к.н.", ClientFont, FontBrush, 8, 6);
                ev.Graphics.DrawLine(Pen, 11, 33, 467, 33);
                ev.Graphics.DrawString("Изгот: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).DocDateTime + "    " +
                    "Смена: 0" + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).NumberOfChange.ToString() + "    " +
                    "Отв.: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).UserName, DecorOrderFont, FontBrush, 8, 37);
                ev.Graphics.DrawLine(Pen, 11, 55, 467, 55);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 0)
                {
                    ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 60);
                    RectangleF rectF1 = new RectangleF(7, 74, 467, 125);
                    ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes, NotesFont, FontBrush, rectF1);
                    ev.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rectF1));
                    ev.Graphics.DrawLine(Pen, 11, 125, 467, 125);
                }
                else
                    indent = -50;
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 0)
                    DrawFactLabelsTable(ev, indent);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
                    DrawDefectLabelsTable(ev, indent);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
                    DrawDefectLabelsTable(ev, indent);
                ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, 46, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);
                Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, ev.Graphics, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                    ev.Graphics.DrawImage(ZTTPS, 249, 320, 37, 45);
                else
                    ev.Graphics.DrawImage(ZTProfil, 249, 320, 37, 45);

                ev.Graphics.DrawImage(STB, 418, 319, 39, 27);
                ev.Graphics.DrawImage(RST, 423, 357, 34, 27);
                ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
                ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);
                ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, 320);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                    ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 332);
                else
                    ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 332);
                ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 344);
                ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 356);
                ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 368);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).GroupType, ClientFont, FontBrush, 252, 364);
            }
            else
            {
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName.Length > 20)
                    ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName.Substring(0, 20), ClientFont, FontBrush, 9, 6);
                else
                    ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName, ClientFont, FontBrush, 9, 6);

                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
                    ev.Graphics.DrawString("НЕКОНДИЦИЯ", ClientFont, FontBrush, 289, 6);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
                    ev.Graphics.DrawString("БРАК", ClientFont, FontBrush, 371, 6);
                ev.Graphics.DrawLine(Pen, 11, 33, 467, 33);
                ev.Graphics.DrawString("№" + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).MegaOrderID.ToString() + "-" + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).MainOrderID.ToString(),
                    DocFont, FontBrush, 8, 37);
                ev.Graphics.DrawLine(Pen, 11, 67, 467, 67);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes != null)
                {
                    ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, 70);

                    if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 37)
                        ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, 84);
                    else
                        ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).Notes, NotesFont, FontBrush, 7, 84);
                }

                ev.Graphics.DrawLine(Pen, 11, 144, 467, 144);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 0)
                    DrawFactLabelsTable(ev, 0);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 1)
                    DrawDefectLabelsTable(ev, 0);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).LabelType == 2)
                    DrawDefectLabelsTable(ev, 0);
                ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, 46, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);
                Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Medium, ev.Graphics, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);
                ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 15, ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 342, 69, 130, 15);
                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                    ev.Graphics.DrawImage(ZTTPS, 249, 320, 37, 45);
                else
                    ev.Graphics.DrawImage(ZTProfil, 249, 320, 37, 45);
                ev.Graphics.DrawImage(STB, 418, 319, 39, 27);
                ev.Graphics.DrawImage(RST, 423, 357, 34, 27);
                ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
                ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);
                ev.Graphics.DrawString("ТУ РБ 100135477.422-2005", InfoFont, FontBrush, 305, 320);

                if (((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                    ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 332);
                else
                    ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 332);
                ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 344);
                ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 356);
                ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 368);
                ev.Graphics.DrawString("Изготовлено: " + ((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, InfoFont, FontBrush, 305, 380);
                ev.Graphics.DrawString(((ProfileAssignmentsLabelInfo)LabelInfo[CurrentLabelNumber]).GroupType, ClientFont, FontBrush, 252, 364);
            }

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

    public class ProfileAssignments
    {
        public bool bBatchEnable;
        public int iBatchAssignmentID;
        public object oBatchDateTime;

        DataTable ComplexSawingDT;
        //DataTable ReturnedRollersDT;
        DataTable TransferredRollersDT;

        DataTable FacingMaterialOnStorageDT;
        DataTable FacingRollersOnStorageDT;
        DataTable RollersInColorOnStorageDT;
        DataTable KashirOnStorageDT;
        DataTable MdfPlateOnStorageDT;
        DataTable MilledProfilesOnStorageDT;
        DataTable SawnStripsOnStorageDT;
        DataTable ShroudedProfilesOnStorageDT;
        DataTable AssembledProfilesOnStorageDT;

        DataTable FilterFacingMaterialDT;
        DataTable FilterFacingRollersDT;
        DataTable FacingMaterialDT;
        DataTable FacingRollersDT1;
        DataTable FacingRollersDT2;

        DataTable AssignmentsDT;
        DataTable AssignmentsBeforeUpdateDT;

        DataTable BatchAssignmentsDT;
        DataTable DecorAssignmentsDT;
        DataTable CoversDT;
        DataTable TechStoreGroupsDT;
        DataTable TechStoreSubGroupsDT;
        DataTable TechStoreDT;
        DataTable TechCatalogStoreDetailDT;
        DataTable TechCatalogOperationsDetailDT;
        DataTable ClientsDT;
        DataTable OrderStatusesDT;
        DataTable UsersDT;

        BindingSource FilterFacingMaterialBS;
        BindingSource FilterFacingRollersBS;
        BindingSource ComplexSawingBS;
        BindingSource ReturnedRollersBS;
        BindingSource TransferredRollersBS;
        BindingSource BatchAssignmentsBS;
        BindingSource TechStoreGroupsBS;
        BindingSource TechStoreSubGroupsBS;
        BindingSource TechStoreBS;

        BindingSource FacingMaterialRequestsBS;
        BindingSource FacingRollersRequestsBS;
        BindingSource KashirRequestsBS;
        BindingSource SawnStripsRequestsBS;
        BindingSource MilledProfileRequestsBS;
        BindingSource ShroudedProfileRequestsBS;
        BindingSource AssembledProfileRequestsBS;

        BindingSource FacingMaterialAssignmentsBS;
        BindingSource FacingRollersAssignmentsBS;
        BindingSource KashirAssignmentsBS;
        BindingSource MilledProfileAssignmentsBS1;
        BindingSource SawnStripsAssignmentsBS;
        BindingSource ShroudedProfileAssignmentsBS1;
        BindingSource AssembledProfileAssignmentsBS;

        BindingSource MilledProfileAssignmentsBS2;
        BindingSource MilledProfileAssignmentsBS3;
        BindingSource ShroudedProfileAssignmentsBS2;

        BindingSource FacingMaterialOnStorageBS;
        BindingSource FacingRollersOnStorageBS;
        BindingSource RollersInColorOnStorageBS;
        BindingSource KashirOnStorageBS;
        BindingSource MdfPlateOnStorageBS;
        BindingSource MilledProfilesOnStorageBS;
        BindingSource SawnStripsOnStorageBS;
        BindingSource ShroudedProfilesOnStorageBS;
        BindingSource AssembledProfilesOnStorageBS;

        public AssignmentsStoreManager AssignmentsStoreManager;

        public ProfileAssignments()
        {
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            GetFacingMaterialOnStorage();
            GetFacingRollersOnStorage();
            GetFilterFacingMaterial();
            GetFilterFacingRollers();
            GetKashirOnStorage();
            GetMdfPlateOnStorage();
            GetMilledProfilesOnStorage();
            GetSawnStripsOnStorage();
            GetShroudedProfilesOnStorage();
            GetAssembledProfilesOnStorage();

            GroupKashir();
            GroupMdfPlate();
            GroupMilledProfiles();
            //GroupSawnStrips();
            GroupShroudedProfiles();
            GroupAssembledProfiles();
            GetAssignmentReadyTable();
        }

        private void Create()
        {
            AssignmentsStoreManager = new AssignmentsStoreManager();

            FilterFacingMaterialDT = new DataTable();
            FilterFacingRollersDT = new DataTable();
            ComplexSawingDT = new DataTable();
            //ReturnedRollersDT = new DataTable();
            TransferredRollersDT = new DataTable();
            FacingMaterialDT = new DataTable();
            FacingRollersDT1 = new DataTable();
            FacingRollersDT2 = new DataTable();
            //AssignmentsDT = new DataTable();
            //AssignmentsDT = new DataTable();
            //AssignmentsDT = new DataTable();
            //AssignmentsDT = new DataTable();
            AssignmentsDT = new DataTable();
            //PackingDT = new DataTable();
            //MilledProfilesBeforeUpdateDT = new DataTable();
            //SawnStripsDBeforeUpdateDT = new DataTable();
            //ShroudedProfilesBeforeUpdateDT = new DataTable();
            AssignmentsBeforeUpdateDT = new DataTable();

            FacingMaterialOnStorageDT = new DataTable();
            FacingRollersOnStorageDT = new DataTable();
            RollersInColorOnStorageDT = new DataTable();
            KashirOnStorageDT = new DataTable();
            MdfPlateOnStorageDT = new DataTable();
            SawnStripsOnStorageDT = new DataTable();
            MilledProfilesOnStorageDT = new DataTable();
            ShroudedProfilesOnStorageDT = new DataTable();
            AssembledProfilesOnStorageDT = new DataTable();

            BatchAssignmentsDT = new DataTable();
            DecorAssignmentsDT = new DataTable();
            TechStoreGroupsDT = new DataTable();
            TechStoreSubGroupsDT = new DataTable();
            TechStoreDT = new DataTable();
            TechCatalogStoreDetailDT = new DataTable();
            TechCatalogOperationsDetailDT = new DataTable();
            ClientsDT = new DataTable();
            OrderStatusesDT = new DataTable();
            UsersDT = new DataTable();

            ComplexSawingBS = new BindingSource();
            ReturnedRollersBS = new BindingSource();
            TransferredRollersBS = new BindingSource();
            BatchAssignmentsBS = new BindingSource();
            TechStoreGroupsBS = new BindingSource();
            TechStoreSubGroupsBS = new BindingSource();
            TechStoreBS = new BindingSource();

            FilterFacingMaterialBS = new BindingSource();
            FilterFacingRollersBS = new BindingSource();
            FacingMaterialRequestsBS = new BindingSource();
            FacingRollersRequestsBS = new BindingSource();
            FacingMaterialOnStorageBS = new BindingSource();
            FacingRollersOnStorageBS = new BindingSource();
            RollersInColorOnStorageBS = new BindingSource();
            KashirOnStorageBS = new BindingSource();
            MdfPlateOnStorageBS = new BindingSource();
            MilledProfilesOnStorageBS = new BindingSource();
            SawnStripsOnStorageBS = new BindingSource();
            ShroudedProfilesOnStorageBS = new BindingSource();
            AssembledProfilesOnStorageBS = new BindingSource();

            KashirRequestsBS = new BindingSource();
            MilledProfileRequestsBS = new BindingSource();
            SawnStripsRequestsBS = new BindingSource();
            ShroudedProfileRequestsBS = new BindingSource();
            AssembledProfileRequestsBS = new BindingSource();
            //PackingRequestsBS = new BindingSource();

            FacingMaterialAssignmentsBS = new BindingSource();
            FacingRollersAssignmentsBS = new BindingSource();
            KashirAssignmentsBS = new BindingSource();
            MilledProfileAssignmentsBS1 = new BindingSource();
            SawnStripsAssignmentsBS = new BindingSource();
            ShroudedProfileAssignmentsBS1 = new BindingSource();
            AssembledProfileAssignmentsBS = new BindingSource();
            //PackingAssignmentsBS = new BindingSource();

            MilledProfileAssignmentsBS2 = new BindingSource();
            MilledProfileAssignmentsBS3 = new BindingSource();
            ShroudedProfileAssignmentsBS2 = new BindingSource();
        }

        private void Fill()
        {
            CreateCoversDT();
            string SelectCommand = @"SELECT TechStoreGroupID, TechStoreGroupName FROM TechStoreGroups ORDER BY TechStoreGroupName";

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreGroupsDT);
            }
            SelectCommand = @"SELECT TechStoreSubGroupID, TechStoreGroupID, TechStoreSubGroupName FROM TechStoreSubGroups ORDER BY TechStoreSubGroupName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreSubGroupsDT);
            }
            SelectCommand = @"SELECT TechStoreID, TechStore.TechStoreSubGroupID, TechStoreSubGroups.TechStoreGroupID, TechStoreName FROM TechStore
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID=TechStoreSubGroups.TechStoreSubGroupID
                INNER JOIN TechStoreGroups ON TechStoreSubGroups.TechStoreGroupID=TechStoreGroups.TechStoreGroupID
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDT);
            }
            SelectCommand = @"SELECT TechCatalogOperationsGroups.GroupName, TechCatalogOperationsGroups.GroupNumber, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsDetail.TechCatalogOperationsDetailID, TechCatalogOperationsDetail.TechCatalogOperationsGroupID, TechCatalogOperationsDetail.SerialNumber, MachinesOperationName FROM TechCatalogOperationsDetail 
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                ORDER BY TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupNumber, TechCatalogOperationsDetail.SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogOperationsDetailDT);
            }
            SelectCommand = @"SELECT TechCatalogStoreDetail.*, TechStore.TechStoreName, TechStore.TechStoreSubGroupID, Measures.Measure FROM TechCatalogStoreDetail
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID ORDER BY GroupA, GroupB, GroupC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogStoreDetailDT);
            }
            SelectCommand = @"SELECT TOP 0 Store.StoreID, Store.StoreItemID, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, Store.Length, Store.Width, Store.CurrentCount, infiniu2_catalog.dbo.TechStore.Notes FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups ON infiniu2_catalog.dbo.TechStore.TechStoreSubGroupID=infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreSubGroupID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FacingMaterialOnStorageDT);
                using (DataView DV = new DataView(FacingMaterialOnStorageDT))
                {
                    FilterFacingMaterialDT = DV.ToTable(true, new string[] { "TechStoreName" });
                }
            }
            SelectCommand = @"SELECT TOP 0 ManufactureStore.ManufactureStoreID, ManufactureStore.StoreItemID, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, ManufactureStore.Diameter, ManufactureStore.Width, ManufactureStore.CurrentCount, infiniu2_catalog.dbo.TechStore.Notes FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups ON infiniu2_catalog.dbo.TechStore.TechStoreSubGroupID=infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreSubGroupID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FacingRollersOnStorageDT);
                using (DataView DV = new DataView(FacingRollersOnStorageDT, string.Empty, "TechStoreName", DataViewRowState.CurrentRows))
                {
                    FilterFacingRollersDT = DV.ToTable(true, "TechStoreName");
                }
            }
            SelectCommand = @"SELECT TOP 0 ManufactureStore.ManufactureStoreID, ManufactureStore.StoreItemID, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, ManufactureStore.Diameter, ManufactureStore.Width, ManufactureStore.CurrentCount, infiniu2_catalog.dbo.TechStore.Notes FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups ON infiniu2_catalog.dbo.TechStore.TechStoreSubGroupID=infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreSubGroupID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(RollersInColorOnStorageDT);
            }
            SelectCommand = @"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, Store.CurrentCount FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(KashirOnStorageDT);
            }
            SelectCommand = @"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, Store.CurrentCount FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(MdfPlateOnStorageDT);
            }
            SelectCommand = @"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, Store.CurrentCount FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(SawnStripsOnStorageDT);
            }
            SelectCommand = @"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, Store.CurrentCount FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(MilledProfilesOnStorageDT);
            }
            SelectCommand = @"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreID, Store.CoverID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, Store.CurrentCount FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(ShroudedProfilesOnStorageDT);
                ShroudedProfilesOnStorageDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));
            }
            SelectCommand = @"SELECT TOP 0 infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, Store.CurrentCount FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(AssembledProfilesOnStorageDT);
            }
            SelectCommand = @"SELECT TOP 0 ReturnedRollers.*, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Notes FROM ReturnedRollers
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ReturnedRollers.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                ORDER BY TechStoreName, ReturnedRollers.Diameter, ReturnedRollers.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                //DA.Fill(ReturnedRollersDT);
                DA.Fill(TransferredRollersDT);
            }

            SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(ComplexSawingDT);
                ComplexSawingDT.Columns.Add(new DataColumn(("ProfilOrderStatusID"), System.Type.GetType("System.Int64")));
            }
            SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(AssignmentsDT);
                AssignmentsDT.Columns.Add(new DataColumn(("ProfilOrderStatusID"), System.Type.GetType("System.Int64")));
            }
            //SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=2";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(AssignmentsDT);
            //    AssignmentsDT.Columns.Add(new DataColumn(("ProfilOrderStatusID"), System.Type.GetType("System.Int64")));
            //}
            //SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=4";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(AssignmentsDT);
            //    AssignmentsDT.Columns.Add(new DataColumn(("ProfilOrderStatusID"), System.Type.GetType("System.Int64")));
            //}

            //SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=1";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(AssignmentsDT);
            //    AssignmentsDT.Columns.Add(new DataColumn(("ProfilOrderStatusID"), System.Type.GetType("System.Int64")));
            //}
            //SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=3";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(AssignmentsDT);
            //    AssignmentsDT.Columns.Add(new DataColumn(("ProfilOrderStatusID"), System.Type.GetType("System.Int64")));
            //}
            //SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=6";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(PackingDT);
            //}
            SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=8";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FacingMaterialDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM DecorAssignments WHERE ProductType=7";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FacingRollersDT1);
                DA.Fill(FacingRollersDT2);
            }
            SelectCommand = @"SELECT * FROM BatchAssignments";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(BatchAssignmentsDT);
                BatchAssignmentsDT.Columns.Add(new DataColumn("ReadyPerc", Type.GetType("System.Decimal")));
            }
            SelectCommand = @"SELECT UserID, Name, ShortName FROM Users";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            SelectCommand = @"SELECT ClientID, ClientName FROM Clients";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                DataRow NewRow = ClientsDT.NewRow();
                NewRow["ClientID"] = 0;
                NewRow["ClientName"] = "СКЛАД";
                ClientsDT.Rows.Add(NewRow);
            }
            SelectCommand = @"SELECT * FROM FirmOrderStatuses";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(OrderStatusesDT);
            }
        }

        private void CreateCoversDT()
        {
            CoversDT = new DataTable();
            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore" +
                " WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)" +
                " ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DataRow EmptyRow = CoversDT.NewRow();
                    EmptyRow["CoverID"] = -1;
                    EmptyRow["CoverName"] = "-";
                    CoversDT.Rows.Add(EmptyRow);

                    DataRow ChoiceRow = CoversDT.NewRow();
                    ChoiceRow["CoverID"] = 0;
                    ChoiceRow["CoverName"] = "на выбор";
                    CoversDT.Rows.Add(ChoiceRow);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = CoversDT.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["CoverName"] = DT.Rows[i]["TechStoreName"].ToString();
                        CoversDT.Rows.Add(NewRow);
                    }
                }
            }
        }

        public bool BatchEnable
        {
            get { return bBatchEnable; }
            set { bBatchEnable = value; }
        }

        public int CurrentBatchAssignmentID
        {
            get { return iBatchAssignmentID; }
            set { iBatchAssignmentID = value; }
        }

        public object CurrentBatchDateTime
        {
            get { return oBatchDateTime; }
            set { oBatchDateTime = value; }
        }

        public DataTable CopyMilledProfileAssignments1
        {
            get
            {
                DataTable dt = ((DataView)MilledProfileAssignmentsBS1.DataSource).ToTable();

                return dt;
            }
        }

        public DataTable CopyMilledProfileAssignments2
        {
            get
            {
                DataTable dt = ((DataView)MilledProfileAssignmentsBS2.DataSource).ToTable();

                return dt;
            }
        }

        public DataTable CopyMilledProfileAssignments3
        {
            get
            {
                DataTable dt = ((DataView)MilledProfileAssignmentsBS2.DataSource).ToTable();

                return dt;
            }
        }

        public DataTable CopyShroudedProfileAssignments1
        {
            get
            {
                DataTable dt = ((DataView)ShroudedProfileAssignmentsBS1.DataSource).ToTable();

                return dt;
            }
        }

        public DataTable CopyShroudedProfileAssignments2
        {
            get
            {
                DataTable dt = ((DataView)ShroudedProfileAssignmentsBS2.DataSource).ToTable();

                return dt;
            }
        }

        public DataTable CopySawnStripsAssignments
        {
            get
            {
                DataTable dt = ((DataView)SawnStripsAssignmentsBS.DataSource).ToTable();

                return dt;
            }
        }

        public DataTable CopyKashirAssignments
        {
            get
            {
                DataTable dt = ((DataView)KashirAssignmentsBS.DataSource).ToTable();

                return dt;
            }
        }

        //public DataTable CopyPackingAssignments
        //{
        //    get
        //    {
        //        DataTable dt = ((DataView)PackingAssignmentsBS.DataSource).ToTable();

        //        return dt;
        //    }
        //}

        public BindingSource FilterFacingMaterialList
        {
            get { return FilterFacingMaterialBS; }
        }

        public BindingSource FilterFacingRollersList
        {
            get { return FilterFacingRollersBS; }
        }

        public BindingSource BatchAssignmentsList
        {
            get { return BatchAssignmentsBS; }
        }

        public BindingSource TechStoreGroupsList
        {
            get { return TechStoreGroupsBS; }
        }

        public BindingSource TechStoreSubGroupsList
        {
            get { return TechStoreSubGroupsBS; }
        }

        public BindingSource TechStoreList
        {
            get { return TechStoreBS; }
        }

        public BindingSource FacingMaterialAssignmentsList
        {
            get { return FacingMaterialAssignmentsBS; }
        }

        public BindingSource FacingRollersAssignmentsList
        {
            get { return FacingRollersAssignmentsBS; }
        }

        public BindingSource FacingMaterialOnStorageList
        {
            get { return FacingMaterialOnStorageBS; }
        }

        public BindingSource FacingRollersOnStorageList
        {
            get { return FacingRollersOnStorageBS; }
        }

        public BindingSource RollersInColorOnStorageList
        {
            get { return RollersInColorOnStorageBS; }
        }

        public BindingSource KashirOnStorageList
        {
            get { return KashirOnStorageBS; }
        }

        public BindingSource MdfPlateOnStorageList
        {
            get { return MdfPlateOnStorageBS; }
        }

        public BindingSource MilledProfilesOnStorageList
        {
            get { return MilledProfilesOnStorageBS; }
        }

        public BindingSource SawnStripsOnStorageList
        {
            get { return SawnStripsOnStorageBS; }
        }

        public BindingSource ShroudedProfilesOnStorageList
        {
            get { return ShroudedProfilesOnStorageBS; }
        }

        public BindingSource AssembledProfilesOnStorageList
        {
            get { return AssembledProfilesOnStorageBS; }
        }

        public BindingSource KashirRequestsList
        {
            get { return KashirRequestsBS; }
        }

        public BindingSource MilledProfileRequestsList
        {
            get { return MilledProfileRequestsBS; }
        }

        public BindingSource SawStripsRequestsList
        {
            get { return SawnStripsRequestsBS; }
        }

        public BindingSource ShroudedProfileRequestsList
        {
            get { return ShroudedProfileRequestsBS; }
        }

        public BindingSource AssembledProfileRequestsList
        {
            get { return AssembledProfileRequestsBS; }
        }

        //public BindingSource PackingRequestsList
        //{
        //    get { return PackingRequestsBS; }
        //}

        public BindingSource KashirAssignmentsList
        {
            get { return KashirAssignmentsBS; }
        }

        public BindingSource MilledProfileAssignmentsList1
        {
            get { return MilledProfileAssignmentsBS1; }
        }

        public BindingSource MilledProfileAssignmentsList2
        {
            get { return MilledProfileAssignmentsBS2; }
        }

        public BindingSource MilledProfileAssignmentsList3
        {
            get { return MilledProfileAssignmentsBS3; }
        }

        public BindingSource SawnStripsAssignmentsList
        {
            get { return SawnStripsAssignmentsBS; }
        }

        public BindingSource ShroudedProfileAssignmentsList1
        {
            get { return ShroudedProfileAssignmentsBS1; }
        }

        public BindingSource ShroudedProfileAssignmentsList2
        {
            get { return ShroudedProfileAssignmentsBS2; }
        }

        public BindingSource AssembledProfileAssignmentsList
        {
            get { return AssembledProfileAssignmentsBS; }
        }

        //public BindingSource PackingAssignmentsList
        //{
        //    get { return PackingAssignmentsBS; }
        //}

        public BindingSource FacingRollersRequestsList
        {
            get { return FacingRollersRequestsBS; }
        }

        public BindingSource FacingMaterialRequestsList
        {
            get { return FacingMaterialRequestsBS; }
        }

        public BindingSource ComplexSawingList
        {
            get { return ComplexSawingBS; }
        }

        public BindingSource ReturnedRollersList
        {
            get { return ReturnedRollersBS; }
        }

        public BindingSource TransferredRollersList
        {
            get { return TransferredRollersBS; }
        }

        public DataGridViewComboBoxColumn ClientNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientNameColumn",
                    HeaderText = "Клиент",
                    DataPropertyName = "ClientID",
                    DataSource = new DataView(ClientsDT),
                    ValueMember = "ClientID",
                    DisplayMember = "ClientName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn OrderStatusColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "OrderStatusColumn",
                    HeaderText = "Статус заказа",
                    DataPropertyName = "ProfilOrderStatusID",
                    DataSource = new DataView(OrderStatusesDT),
                    ValueMember = "FirmOrderStatusID",
                    DisplayMember = "FirmOrderStatus",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CloseUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CloseUserColumn",
                    HeaderText = "Кто закрыл",
                    DataPropertyName = "CloseUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ToolsConfirmUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ToolsConfirmUserColumn",
                    HeaderText = "Инструмент:\r\nкто утвердил",
                    DataPropertyName = "ToolsConfirmUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnologyConfirmUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnologyConfirmUserColumn",
                    HeaderText = "Технология:\r\nкто утвердил",
                    DataPropertyName = "TechnologyConfirmUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn MaterialConfirmUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MaterialConfirmUserColumn",
                    HeaderText = "Материал:\r\nкто утвердил",
                    DataPropertyName = "MaterialConfirmUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechnicalConfirmUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechnicalConfirmUserColumn",
                    HeaderText = "Техническое состояние:\r\nкто утвердил",
                    DataPropertyName = "TechnicalConfirmUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn FacingRollersColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "FacingRollersColumn",
                    HeaderText = "Название",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreGroupID=" + Convert.ToInt32(TechStoreGroups.FacingRollers), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn FacingMaterialColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "FacingMaterialColumn",
                    HeaderText = "Название",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreGroupID=" + Convert.ToInt32(TechStoreGroups.FacingMaterial), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PVAColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PVAColumn",
                    HeaderText = "Клей",
                    DataPropertyName = "TechColorID",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.PVA), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CoversColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CoversColumn",
                    HeaderText = "Цвет",
                    DataPropertyName = "CoverID2",
                    DataSource = new DataView(CoversDT),
                    ValueMember = "CoverID",
                    DisplayMember = "CoverName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn MdfPlateColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MdfPlateColumn",
                    HeaderText = "МДФ",
                    DataPropertyName = "TechStoreID1",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MdfPlate), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn KashirColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "KashirColumn",
                    HeaderText = "Вставка",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.Kashir), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PackingProfileColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PackingProfileColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledAssembledProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedAssembledProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ShroudedProfileColumn1
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ShroudedProfileColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID1",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledAssembledProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedAssembledProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn ShroudedProfileColumn2
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ShroudedProfileColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn AssembledProfileColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "AssembledProfileColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedAssembledProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledAssembledProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn MilledProfileColumn1
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MilledProfileColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID1",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn MilledProfileColumn2
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MilledProfileColumn",
                    HeaderText = "Профиль",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn SawStripsColumn1
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "SawStripsColumn",
                    HeaderText = "Наименование",
                    DataPropertyName = "TechStoreID1",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.SawStrips) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledAssembledProfile) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedAssembledProfile), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn SawStripsColumn2
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "SawStripsColumn",
                    HeaderText = "Наименование",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.SawStrips) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.SawDSTP), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn SawStripsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "SawStripsColumn",
                    HeaderText = "Наименование",
                    DataPropertyName = "TechStoreID2",
                    DataSource = new DataView(TechStoreDT, "TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.SawStrips) +
                    " OR TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.SawDSTP), string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public DataGridViewImageColumn BatchEnabledColumn
        {
            get
            {
                DataGridViewImageColumn Column = new DataGridViewImageColumn()
                {
                    Name = "BatchEnabledColumn",
                    HeaderText = string.Empty,
                    DataPropertyName = "EnabledImage",
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        private string GetTechStoreName(int TechStoreID)
        {
            string Name = string.Empty;
            try
            {
                DataRow[] Rows = TechStoreDT.Select("TechStoreID = " + TechStoreID);
                Name = Rows[0]["TechStoreName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return Name;
        }

        private string GetOperationName(int TechStoreID)
        {
            string Name = string.Empty;
            try
            {
                DataRow[] Rows = TechStoreDT.Select("TechStoreID = " + TechStoreID);
                Name = Rows[0]["TechStore"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return Name;
        }


        private void Binding()
        {
            FilterFacingMaterialBS.DataSource = FilterFacingMaterialDT;
            FilterFacingRollersBS.DataSource = FilterFacingRollersDT;

            ComplexSawingBS.DataSource = ComplexSawingDT;
            //ReturnedRollersBS.DataSource = ReturnedRollersDT;
            //TransferredRollersBS.DataSource = TransferredRollersDT;
            ReturnedRollersBS.DataSource = new DataView(TransferredRollersDT, "ReturnType=1", string.Empty, DataViewRowState.CurrentRows);
            TransferredRollersBS.DataSource = new DataView(TransferredRollersDT, "ReturnType=0", string.Empty, DataViewRowState.CurrentRows);
            BatchAssignmentsBS.DataSource = BatchAssignmentsDT;
            BatchAssignmentsBS.Sort = "BatchAssignmentID DESC";
            TechStoreGroupsBS.DataSource = TechStoreGroupsDT;
            TechStoreSubGroupsBS.DataSource = TechStoreSubGroupsDT;
            TechStoreBS.DataSource = TechStoreDT;

            FacingMaterialOnStorageBS.DataSource = FacingMaterialOnStorageDT;
            FacingRollersOnStorageBS.DataSource = FacingRollersOnStorageDT;
            RollersInColorOnStorageBS.DataSource = RollersInColorOnStorageDT;
            KashirOnStorageBS.DataSource = KashirOnStorageDT;
            MdfPlateOnStorageBS.DataSource = MdfPlateOnStorageDT;
            MilledProfilesOnStorageBS.DataSource = MilledProfilesOnStorageDT;
            SawnStripsOnStorageBS.DataSource = SawnStripsOnStorageDT;
            ShroudedProfilesOnStorageBS.DataSource = ShroudedProfilesOnStorageDT;
            AssembledProfilesOnStorageBS.DataSource = AssembledProfilesOnStorageDT;

            FacingRollersRequestsBS.DataSource = new DataView(FacingRollersDT2, "BatchAssignmentID IS NOT NULL AND DecorAssignmentStatusID=1", string.Empty, DataViewRowState.CurrentRows);
            FacingMaterialRequestsBS.DataSource = new DataView(FacingMaterialDT, string.Empty, string.Empty, DataViewRowState.CurrentRows);
            KashirRequestsBS.DataSource = new DataView(AssignmentsDT, "ProductType=5 AND InPlan=0", string.Empty, DataViewRowState.CurrentRows);
            MilledProfileRequestsBS.DataSource = new DataView(AssignmentsDT, "ProductType=2 AND InPlan=False", string.Empty, DataViewRowState.CurrentRows);
            SawnStripsRequestsBS.DataSource = new DataView(AssignmentsDT, "ProductType=4 AND InPlan=0", string.Empty, DataViewRowState.CurrentRows);
            ShroudedProfileRequestsBS.DataSource = new DataView(AssignmentsDT, "ProductType=1 AND InPlan=0", string.Empty, DataViewRowState.CurrentRows);
            AssembledProfileRequestsBS.DataSource = new DataView(AssignmentsDT, "ProductType=3 AND InPlan=0", string.Empty, DataViewRowState.CurrentRows);

            FacingMaterialAssignmentsBS.DataSource = new DataView(FacingRollersDT1, "BatchAssignmentID IS NOT NULL", string.Empty, DataViewRowState.CurrentRows);
            FacingRollersAssignmentsBS.DataSource = new DataView(FacingRollersDT2, "BatchAssignmentID IS NOT NULL AND DecorAssignmentStatusID=2", string.Empty, DataViewRowState.CurrentRows);

            KashirAssignmentsBS.DataSource = new DataView(AssignmentsDT, "ProductType=5 AND BatchAssignmentID IS NOT NULL AND InPlan=1", string.Empty, DataViewRowState.CurrentRows);
            MilledProfileAssignmentsBS1.DataSource = new DataView(AssignmentsDT, "ProductType=2 AND BatchAssignmentID IS NOT NULL AND InPlan=1 AND FrezerNumber=1", string.Empty, DataViewRowState.CurrentRows);
            SawnStripsAssignmentsBS.DataSource = new DataView(AssignmentsDT, "ProductType=4 AND BatchAssignmentID IS NOT NULL AND InPlan=1", string.Empty, DataViewRowState.CurrentRows);
            ShroudedProfileAssignmentsBS1.DataSource = new DataView(AssignmentsDT, "ProductType=1 AND BatchAssignmentID IS NOT NULL AND InPlan=1 AND BarberanNumber=1", string.Empty, DataViewRowState.CurrentRows);
            AssembledProfileAssignmentsBS.DataSource = new DataView(AssignmentsDT, "ProductType=3 AND BatchAssignmentID IS NOT NULL AND InPlan=1", string.Empty, DataViewRowState.CurrentRows);

            MilledProfileAssignmentsBS2.DataSource = new DataView(AssignmentsDT, "ProductType=2 AND BatchAssignmentID IS NOT NULL AND InPlan=1 AND FrezerNumber=2", string.Empty, DataViewRowState.CurrentRows);
            MilledProfileAssignmentsBS3.DataSource = new DataView(AssignmentsDT, "ProductType=2 AND BatchAssignmentID IS NOT NULL AND InPlan=1 AND FrezerNumber=3", string.Empty, DataViewRowState.CurrentRows);
            ShroudedProfileAssignmentsBS2.DataSource = new DataView(AssignmentsDT, "ProductType=1 AND BatchAssignmentID IS NOT NULL AND InPlan=1 AND BarberanNumber=2", string.Empty, DataViewRowState.CurrentRows);
        }

        public void SetPrevLinks()
        {
            for (int i = 0; i < AssignmentsDT.Rows.Count; i++)
            {
                int BarberanNumber = 0;
                int FrezerNumber = 0;
                int ProductType = 0;
                int TechStoreID1Old = 0;
                int TechStoreID1New = 0;
                int TechStoreID2 = 0;
                decimal Width1 = 0;
                if (AssignmentsDT.Rows[i]["ProductType"] != DBNull.Value)
                    ProductType = Convert.ToInt32(AssignmentsDT.Rows[i]["ProductType"]);
                if (AssignmentsDT.Rows[i]["BarberanNumber"] != DBNull.Value)
                    BarberanNumber = Convert.ToInt32(AssignmentsDT.Rows[i]["BarberanNumber"]);
                if (AssignmentsDT.Rows[i]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(AssignmentsDT.Rows[i]["FrezerNumber"]);
                if (AssignmentsDT.Rows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1Old = Convert.ToInt32(AssignmentsDT.Rows[i]["TechStoreID1"]);
                if (AssignmentsDT.Rows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(AssignmentsDT.Rows[i]["TechStoreID2"]);

                if (ProductType == 1)
                {
                    TechStoreID1New = FindMilledProfileID(TechStoreID2, BarberanNumber);
                    if (TechStoreID1New == 0)
                    {
                        TechStoreID1New = FindShroudedAssembledProfileID(TechStoreID2, BarberanNumber);
                        if (TechStoreID1New == 0)
                        {
                            TechStoreID1New = FindMilledAssembledProfileID(TechStoreID2, BarberanNumber);
                            if (TechStoreID1New == 0)
                            {
                                TechStoreID1New = FindSawStripID(TechStoreID2, BarberanNumber, ref Width1);
                            }
                        }
                    }

                }
                if (ProductType == 2)
                {
                    TechStoreID1New = FindSawStripID(TechStoreID2, FrezerNumber, ref Width1);
                    if (TechStoreID1New == 0)
                    {
                        TechStoreID1New = FindShroudedAssembledProfileID(TechStoreID2, FrezerNumber);
                    }
                }
                if (ProductType == 3)
                {
                    ArrayList TechStoreIDs = new ArrayList();
                    bool b = FindShroudedProfileID(TechStoreID2, 1, TechStoreIDs);
                    if (!b)
                    {
                        FindMilledProfileID(TechStoreID2, 1, TechStoreIDs);
                    }
                }
                if (ProductType == 4)
                {
                    TechStoreID1New = FindMdfID(TechStoreID2, ref Width1);
                }
                if (ProductType == 5)
                {
                    TechStoreID1New = FindSawStripID(TechStoreID2, 1, ref Width1);
                }
                if (TechStoreID1New != 0 && TechStoreID1New != TechStoreID1Old)
                    AssignmentsDT.Rows[i]["TechStoreID1"] = TechStoreID1New;

                if (AssignmentsDT.Rows[i]["PrevLinkAssignmentID"] == DBNull.Value)
                    continue;
                int DecorAssignmentID = Convert.ToInt32(AssignmentsDT.Rows[i]["DecorAssignmentID"]);
                int PrevLinkAssignmentID = Convert.ToInt32(AssignmentsDT.Rows[i]["PrevLinkAssignmentID"]);
                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + PrevLinkAssignmentID);
                if (rows.Count() != 0)
                {
                    if (rows[0]["NextLinkAssignmentID"] != DBNull.Value)
                        continue;
                    rows[0]["NextLinkAssignmentID"] = DecorAssignmentID;

                    if (AssignmentsDT.Rows[i]["Length2"] != DBNull.Value)
                        rows[0]["Length1"] = AssignmentsDT.Rows[i]["Length2"];
                    if (AssignmentsDT.Rows[i]["Width2"] != DBNull.Value)
                        rows[0]["Width1"] = AssignmentsDT.Rows[i]["Width2"];
                }
            }
        }

        public decimal EditFrezerWidth(int PrevLinkAssignmentID, int SawNumber)
        {
            int TechStoreID1 = 0;
            decimal Width1 = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + PrevLinkAssignmentID);
            if (rows.Count() != 0)
            {
                int FrezerNumber = 0;
                if (rows[0]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(rows[0]["FrezerNumber"]);
                if (rows[0]["TechStoreID2"] != DBNull.Value)
                    TechStoreID1 = Convert.ToInt32(rows[0]["TechStoreID2"]);
                FindSawStripID(TechStoreID1, FrezerNumber, SawNumber, ref Width1);
                if (Width1 != 0)
                    rows[0]["Width1"] = Width1;
            }
            return Width1;
        }

        public void GetAssignmentReadyTable()
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyPerc = 0;
            decimal ReadyProgressVal = 0;
            decimal d1 = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT BatchAssignmentID, DecorAssignmentID, PrevLinkAssignmentID, NextLinkAssignmentID, ProductType, DecorAssignmentStatusID, BarberanNumber, FrezerNumber, FacingMachine FROM DecorAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DecorAssignmentsDT.Clear();
                    DA.Fill(DecorAssignmentsDT);
                }
            }

            for (int i = 0; i < BatchAssignmentsDT.Rows.Count; i++)
            {
                int BatchAssignmentID = Convert.ToInt32(BatchAssignmentsDT.Rows[i]["BatchAssignmentID"]);

                DataRow[] rows = DecorAssignmentsDT.Select("BatchAssignmentID=" + BatchAssignmentID);
                if (rows.Count() == 0)
                {
                    BatchAssignmentsDT.Rows[i]["ReadyPerc"] = 0;
                    continue;
                }

                ReadyCount = 0;
                AllCount = 0;
                ReadyPerc = 0;
                ReadyProgressVal = 0;
                d1 = 0;

                foreach (DataRow item in rows)
                {
                    if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                        ReadyCount++;
                    AllCount++;

                }

                if (AllCount > 0)
                    ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
                d1 = ReadyProgressVal * 100;
                ReadyPerc = Decimal.Round(d1, 1, MidpointRounding.AwayFromZero);

                BatchAssignmentsDT.Rows[i]["ReadyPerc"] = ReadyPerc;
            }
        }

        public bool GetBarberanReady(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("ProductType=1 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetBarberan1Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("BarberanNumber=1 AND ProductType=1 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetBarberan2Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("BarberanNumber=2 AND ProductType=1 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFrezerReady(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("ProductType=2 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFrezer1Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("FrezerNumber=1 AND ProductType=2 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFrezer2Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("FrezerNumber=2 AND ProductType=2 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFrezer3Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("FrezerNumber=3 AND ProductType=2 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetAssemblyReady(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("ProductType=3 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetSawReady(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("ProductType=4 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFacingReady(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("(ProductType=7 OR ProductType=8) AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFacing2Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("(FacingMachine='Пила' AND ProductType=8) OR (ProductType=7) AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFacing1Ready(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("(FacingMachine='TF-1300' AND ProductType=8) OR (FacingMachine IS NULL AND ProductType=7) AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetKashirReady(int BatchAssignmentID, ref decimal ReadyPerc)
        {
            int ReadyCount = 0;
            int AllCount = 0;

            decimal ReadyProgressVal = 0;

            DataRow[] rows = DecorAssignmentsDT.Select("ProductType=5 AND BatchAssignmentID=" + BatchAssignmentID);
            if (rows.Count() == 0)
                return false;

            foreach (DataRow item in rows)
            {
                if (Convert.ToInt32(item["DecorAssignmentStatusID"]) == 3)
                    ReadyCount++;
                AllCount++;
            }

            if (AllCount > 0)
                ReadyProgressVal = Convert.ToDecimal(Convert.ToDecimal(ReadyCount) / Convert.ToDecimal(AllCount));
            ReadyPerc = Decimal.Round(ReadyProgressVal * 100, 1, MidpointRounding.AwayFromZero);

            return true;
        }

        public bool GetFacingMaterialOnStorage()
        {
            FacingMaterialOnStorageDT.Clear();
            string SelectCommand = @"SELECT Store.StoreID, Store.StoreItemID, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, Store.Length, Store.Width, Store.CurrentCount, infiniu2_catalog.dbo.TechStore.Notes, Store.CreateDateTime FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups ON infiniu2_catalog.dbo.TechStore.TechStoreSubGroupID=infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreSubGroupID 
                AND infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreGroupID=" + Convert.ToInt32(TechStoreGroups.FacingMaterial) + " WHERE Store.CurrentCount<>0 ORDER BY TechStoreName, Store.Length, Store.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FacingMaterialOnStorageDT);
            }
            return FacingMaterialOnStorageDT.Rows.Count > 0;
        }

        public bool GetFacingRollersOnStorage()
        {
            FacingRollersOnStorageDT.Clear();
            string SelectCommand = @"SELECT ManufactureStore.ManufactureStoreID, ManufactureStore.StoreItemID, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, ManufactureStore.Diameter, ManufactureStore.Width, ManufactureStore.CurrentCount, infiniu2_catalog.dbo.TechStore.Notes, ManufactureStore.CreateDateTime FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups ON infiniu2_catalog.dbo.TechStore.TechStoreSubGroupID=infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreSubGroupID 
                AND infiniu2_catalog.dbo.TechStoreSubGroups.TechStoreGroupID=" + Convert.ToInt32(TechStoreGroups.FacingRollers) + " WHERE ManufactureStore.CurrentCount<>0 ORDER BY TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(FacingRollersOnStorageDT);
            }
            return FacingRollersOnStorageDT.Rows.Count > 0;
        }

        public void GetRollersInColorOnStorage(int CoverID)
        {
            RollersInColorOnStorageDT.Clear();
            string SelectCommand = @"SELECT ManufactureStore.ManufactureStoreID, ManufactureStore.StoreItemID, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, ManufactureStore.Diameter, ManufactureStore.Width, ManufactureStore.CurrentCount, infiniu2_catalog.dbo.TechStore.Notes, ManufactureStore.CreateDateTime FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE ManufactureStore.StoreItemID=" + CoverID + " AND ManufactureStore.CurrentCount<>0 ORDER BY TechStoreName, ManufactureStore.Diameter, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(RollersInColorOnStorageDT);
            }
        }

        //        public void GetReturnedRollers(int DecorAssignmentID)
        //        {
        //            ReturnedRollersDT.Clear();
        //            string SelectCommand = @"SELECT ReturnedRollers.*, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Thickness, infiniu2_catalog.dbo.TechStore.Notes FROM ReturnedRollers
        //                INNER JOIN infiniu2_catalog.dbo.TechStore ON ReturnedRollers.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
        //                WHERE ReturnType=1 AND ReturnedRollers.DecorAssignmentID=" + DecorAssignmentID + " ORDER BY TechStoreName, ReturnedRollers.Diameter, ReturnedRollers.Height";
        //            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
        //            {
        //                DA.Fill(ReturnedRollersDT);
        //            }
        //        }

        public int GetTransferredRollers(int DecorAssignmentID)
        {
            int LastReturnedRollerID = 0;
            TransferredRollersDT.Clear();
            string SelectCommand = @"SELECT ReturnedRollers.*, infiniu2_catalog.dbo.TechStore.TechStoreName, infiniu2_catalog.dbo.TechStore.Notes FROM ReturnedRollers
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ReturnedRollers.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE ReturnedRollers.DecorAssignmentID=" + DecorAssignmentID + " ORDER BY ReturnedRollerID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(TransferredRollersDT);
                if (TransferredRollersDT.Rows.Count > 0)
                {
                    LastReturnedRollerID = Convert.ToInt32(TransferredRollersDT.Rows[TransferredRollersDT.Rows.Count - 1]["ReturnedRollerID"]);
                }
            }
            return LastReturnedRollerID;
        }

        public void GetFilterFacingMaterial()
        {
            DataTable DT = FilterFacingMaterialDT.Clone();
            FilterFacingMaterialDT.Clear();
            using (DataView DV = new DataView(FacingMaterialOnStorageDT, string.Empty, "TechStoreName", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, "TechStoreName");
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                DataRow NewRow = FilterFacingMaterialDT.NewRow();
                NewRow.ItemArray = DT.Rows[i].ItemArray;
                FilterFacingMaterialDT.Rows.Add(NewRow);
            }
        }

        public void GetFilterFacingRollers()
        {
            DataTable DT = FilterFacingRollersDT.Clone();
            FilterFacingRollersDT.Clear();
            using (DataView DV = new DataView(FacingRollersOnStorageDT, string.Empty, "TechStoreName", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, "TechStoreName");
            }
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                DataRow NewRow = FilterFacingRollersDT.NewRow();
                NewRow.ItemArray = DT.Rows[i].ItemArray;
                FilterFacingRollersDT.Rows.Add(NewRow);
            }
        }

        public bool GetKashirOnStorage()
        {
            KashirOnStorageDT.Clear();
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, SUM(Store.CurrentCount) FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.Kashir) + ") AND Store.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = KashirOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                KashirOnStorageDT.Rows.Add(NewRow);
            }
            SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width, SUM(ManufactureStore.CurrentCount) FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore 
                WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.Kashir) + ") AND ManufactureStore.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = KashirOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                KashirOnStorageDT.Rows.Add(NewRow);
            }
            DT.Dispose();
            return KashirOnStorageDT.Rows.Count > 0;
        }

        public bool GetMdfPlateOnStorage()
        {
            MdfPlateOnStorageDT.Clear();
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width, SUM(ManufactureStore.CurrentCount) FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore 
                WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MdfPlate) + ") AND ManufactureStore.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = MdfPlateOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                MdfPlateOnStorageDT.Rows.Add(NewRow);
            }
            DT.Dispose();
            return MdfPlateOnStorageDT.Rows.Count > 0;
        }

        public bool GetMilledProfilesOnStorage()
        {
            MilledProfilesOnStorageDT.Clear();
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width, SUM(ManufactureStore.CurrentCount) FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore 
                WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.MilledProfile) + ") AND ManufactureStore.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = MilledProfilesOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                MilledProfilesOnStorageDT.Rows.Add(NewRow);
            }
            DT.Dispose();
            return MilledProfilesOnStorageDT.Rows.Count > 0;
        }

        public bool GetSawnStripsOnStorage()
        {
            SawnStripsOnStorageDT.Clear();
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width, SUM(ManufactureStore.CurrentCount) FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore 
                WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.SawStrips) + @") AND ManufactureStore.CurrentCount<>0 AND FactoryID=1
                GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = SawnStripsOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                SawnStripsOnStorageDT.Rows.Add(NewRow);
            }
            DT.Dispose();
            return SawnStripsOnStorageDT.Rows.Count > 0;
        }

        public bool GetShroudedProfilesOnStorage()
        {
            //DecorAssignmentID=0 AND 
            ShroudedProfilesOnStorageDT.Clear();
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, Store.CoverID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width, SUM(Store.CurrentCount) FROM Store
                INNER JOIN infiniu2_catalog.dbo.TechStore ON Store.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedProfile) + ") AND Store.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, Store.CoverID, infiniu2_catalog.dbo.TechStore.TechStoreName, Store.Length, Store.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = ShroudedProfilesOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                ShroudedProfilesOnStorageDT.Rows.Add(NewRow);
            }
            SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, ManufactureStore.CoverID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width, SUM(ManufactureStore.CurrentCount) FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedProfile) + ") AND ManufactureStore.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, ManufactureStore.CoverID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = ShroudedProfilesOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                ShroudedProfilesOnStorageDT.Rows.Add(NewRow);
            }
            for (int i = 0; i < ShroudedProfilesOnStorageDT.Rows.Count; i++)
            {
                int CoverID = -1;
                if (ShroudedProfilesOnStorageDT.Rows[i]["CoverID"] == DBNull.Value)
                    continue;
                CoverID = Convert.ToInt32(ShroudedProfilesOnStorageDT.Rows[i]["CoverID"]);
                DataRow[] rows = CoversDT.Select("CoverID=" + CoverID);
                if (rows.Count() > 0)
                    ShroudedProfilesOnStorageDT.Rows[i]["CoverName"] = rows[0]["CoverName"].ToString();
            }
            DT.Dispose();
            return ShroudedProfilesOnStorageDT.Rows.Count > 0;
        }

        public bool GetAssembledProfilesOnStorage()
        {
            AssembledProfilesOnStorageDT.Clear();
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width, SUM(ManufactureStore.CurrentCount) FROM ManufactureStore
                INNER JOIN infiniu2_catalog.dbo.TechStore ON ManufactureStore.StoreItemID=infiniu2_catalog.dbo.TechStore.TechStoreID
                WHERE TechStoreID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore
                WHERE TechStoreSubGroupID=" + Convert.ToInt32(TechStoreSubGroups.ShroudedAssembledProfile) + ") AND ManufactureStore.CurrentCount<>0 AND FactoryID=1 GROUP BY infiniu2_catalog.dbo.TechStore.TechStoreID, infiniu2_catalog.dbo.TechStore.TechStoreName, ManufactureStore.Length, ManufactureStore.Width";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DT.Clear();
                DA.Fill(DT);
            }
            foreach (DataRow item in DT.Rows)
            {
                DataRow NewRow = AssembledProfilesOnStorageDT.NewRow();
                NewRow.ItemArray = item.ItemArray;
                AssembledProfilesOnStorageDT.Rows.Add(NewRow);
            }
            DT.Dispose();
            return AssembledProfilesOnStorageDT.Rows.Count > 0;
        }

        public void GetComplexSawing(int DecorAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM DecorAssignments WHERE PrevLinkAssignmentID=" + DecorAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                ComplexSawingDT.Clear();
                DA.Fill(ComplexSawingDT);
            }
        }

        public void GroupFacingMaterialOnStorage()
        {
            DataTable DistinctTable = new DataTable();
            DataTable TempDT = FacingMaterialOnStorageDT.Copy();

            FacingMaterialOnStorageDT.Clear();

            using (DataView DV = new DataView(TempDT, "Thickness IS NOT NULL AND Length IS NOT NULL AND Width IS NOT NULL AND CurrentCount IS NOT NULL", "TechStoreName, Thickness, Length, Width", DataViewRowState.CurrentRows))
            {
                DistinctTable = DV.ToTable(true, "StoreItemID", "Thickness", "Length", "Width", "Notes");
            }

            for (int i = 0; i < DistinctTable.Rows.Count; i++)
            {
                string s =
                    String.Format(new System.Globalization.CultureInfo("en-US"), "StoreItemID=" + Convert.ToInt32(DistinctTable.Rows[i]["StoreItemID"]) +
                       " AND Thickness={0} AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) +
                       " AND Width=" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]), Convert.ToDecimal(DistinctTable.Rows[i]["Thickness"]));
                DataRow[] Srows = TempDT.Select(s);
                if (Srows.Count() == 0)
                    continue;

                int TotalCount = 0;
                foreach (DataRow row in Srows)
                    TotalCount += Convert.ToInt32(row["CurrentCount"]);

                DataRow[] Rows = FacingMaterialOnStorageDT.Select(s);

                if (Rows.Count() == 0)
                {
                    DataRow NewRow = FacingMaterialOnStorageDT.NewRow();
                    NewRow["StoreItemID"] = Srows[0]["StoreItemID"];
                    NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
                    NewRow["Thickness"] = Srows[0]["Thickness"];
                    NewRow["Length"] = Srows[0]["Length"];
                    NewRow["Width"] = Srows[0]["Width"];
                    NewRow["CurrentCount"] = TotalCount;
                    FacingMaterialOnStorageDT.Rows.Add(NewRow);
                }
                else
                    Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            }
        }

        public void GroupKashir()
        {
            //DataTable DistinctTable = new DataTable();
            //DataTable TempDT = KashirOnStorageDT.Copy();

            //KashirOnStorageDT.Clear();

            //using (DataView DV = new DataView(TempDT, string.Empty, "TechStoreName, Length, Width", DataViewRowState.CurrentRows))
            //{
            //    DistinctTable = DV.ToTable(true, "TechStoreID", "Length", "Width");
            //}

            //for (int i = 0; i < DistinctTable.Rows.Count; i++)
            //{
            //    DataRow[] Srows = TempDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND Width=" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]));
            //    if (Srows.Count() == 0)
            //        continue;

            //    int TotalCount = 0;
            //    foreach (DataRow row in Srows)
            //        TotalCount += Convert.ToInt32(row["CurrentCount"]);

            //    DataRow[] Rows = KashirOnStorageDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND Width=" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]));

            //    if (Rows.Count() == 0)
            //    {
            //        DataRow NewRow = KashirOnStorageDT.NewRow();
            //        NewRow["TechStoreID"] = Srows[0]["TechStoreID"];
            //        NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
            //        NewRow["Length"] = Srows[0]["Length"];
            //        NewRow["Width"] = Srows[0]["Width"];
            //        NewRow["CurrentCount"] = TotalCount;
            //        KashirOnStorageDT.Rows.Add(NewRow);
            //    }
            //    else
            //        Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            //}
        }

        public void GroupMdfPlate()
        {
            //DataTable DistinctTable = new DataTable();
            //DataTable TempDT = MdfPlateOnStorageDT.Copy();

            //MdfPlateOnStorageDT.Clear();

            //using (DataView DV = new DataView(TempDT, string.Empty, "TechStoreName, Length, Width", DataViewRowState.CurrentRows))
            //{
            //    DistinctTable = DV.ToTable(true, "TechStoreID", "Length", "Width");
            //}

            //for (int i = 0; i < DistinctTable.Rows.Count; i++)
            //{
            //    DataRow[] Srows = TempDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND Width=" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]));
            //    if (Srows.Count() == 0)
            //        continue;

            //    int TotalCount = 0;
            //    foreach (DataRow row in Srows)
            //        TotalCount += Convert.ToInt32(row["CurrentCount"]);

            //    DataRow[] Rows = MdfPlateOnStorageDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND Width=" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]));

            //    if (Rows.Count() == 0)
            //    {
            //        DataRow NewRow = MdfPlateOnStorageDT.NewRow();
            //        NewRow["TechStoreID"] = Srows[0]["TechStoreID"];
            //        NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
            //        NewRow["Length"] = Srows[0]["Length"];
            //        NewRow["Width"] = Srows[0]["Width"];
            //        NewRow["CurrentCount"] = TotalCount;
            //        MdfPlateOnStorageDT.Rows.Add(NewRow);
            //    }
            //    else
            //        Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            //}
        }

        public void GroupMilledProfiles()
        {
            //DataTable DistinctTable = new DataTable();
            //DataTable TempDT = MilledProfilesOnStorageDT.Copy();

            //MilledProfilesOnStorageDT.Clear();

            //using (DataView DV = new DataView(TempDT, string.Empty, "TechStoreName, Length", DataViewRowState.CurrentRows))
            //{
            //    DistinctTable = DV.ToTable(true, "TechStoreID", "Length");
            //}

            //for (int i = 0; i < DistinctTable.Rows.Count; i++)
            //{
            //    DataRow[] Srows = TempDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]));
            //    if (Srows.Count() == 0)
            //        continue;

            //    int TotalCount = 0;
            //    foreach (DataRow row in Srows)
            //        TotalCount += Convert.ToInt32(row["CurrentCount"]);

            //    DataRow[] Rows = MilledProfilesOnStorageDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]));

            //    if (Rows.Count() == 0)
            //    {
            //        DataRow NewRow = MilledProfilesOnStorageDT.NewRow();
            //        NewRow["TechStoreID"] = Srows[0]["TechStoreID"];
            //        NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
            //        NewRow["Length"] = Srows[0]["Length"];
            //        NewRow["CurrentCount"] = TotalCount;
            //        MilledProfilesOnStorageDT.Rows.Add(NewRow);
            //    }
            //    else
            //        Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            //}
        }

        public void GroupSawnStrips()
        {
            //DataTable DistinctTable = new DataTable();
            //DataTable TempDT = SawnStripsOnStorageDT.Copy();

            //SawnStripsOnStorageDT.Clear();

            //using (DataView DV = new DataView(TempDT, string.Empty, "TechStoreName, Length, Width", DataViewRowState.CurrentRows))
            //{
            //    DistinctTable = DV.ToTable(true, "TechStoreID", "Length", "Width");
            //}

            //for (int i = 0; i < DistinctTable.Rows.Count; i++)
            //{
            //    DataRow[] Srows = TempDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length='" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]).ToString() + "' AND Width='" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]).ToString() + "'");
            //    if (Srows.Count() == 0)
            //        continue;

            //    int TotalCount = 0;
            //    foreach (DataRow row in Srows)
            //        TotalCount += Convert.ToInt32(row["CurrentCount"]);

            //    DataRow[] Rows = SawnStripsOnStorageDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND Width=" + Convert.ToInt32(DistinctTable.Rows[i]["Width"]));

            //    if (Rows.Count() == 0)
            //    {
            //        DataRow NewRow = SawnStripsOnStorageDT.NewRow();
            //        NewRow["TechStoreID"] = Srows[0]["TechStoreID"];
            //        NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
            //        NewRow["Length"] = Srows[0]["Length"];
            //        NewRow["Width"] = Srows[0]["Width"];
            //        NewRow["CurrentCount"] = TotalCount;
            //        SawnStripsOnStorageDT.Rows.Add(NewRow);
            //    }
            //    else
            //        Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            //}
        }

        public void GroupShroudedProfiles()
        {
            //DataTable DistinctTable = new DataTable();
            //DataTable TempDT = ShroudedProfilesOnStorageDT.Copy();

            //ShroudedProfilesOnStorageDT.Clear();

            //using (DataView DV = new DataView(TempDT, string.Empty, "TechStoreName, CoverName, Length", DataViewRowState.CurrentRows))
            //{
            //    DistinctTable = DV.ToTable(true, "TechStoreID", "Length", "CoverID");
            //}

            //for (int i = 0; i < DistinctTable.Rows.Count; i++)
            //{
            //    var v1 = DistinctTable.Rows[i]["TechStoreID"];
            //    var v2 = DistinctTable.Rows[i]["Length"];
            //    var v3 = DistinctTable.Rows[i]["CoverID"];
            //    DataRow[] Srows = TempDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND CoverID=" + Convert.ToInt32(DistinctTable.Rows[i]["CoverID"]));
            //    if (Srows.Count() == 0)
            //        continue;

            //    int TotalCount = 0;
            //    foreach (DataRow row in Srows)
            //        TotalCount += Convert.ToInt32(row["CurrentCount"]);

            //    DataRow[] Rows = ShroudedProfilesOnStorageDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]) + " AND CoverID=" + Convert.ToInt32(DistinctTable.Rows[i]["CoverID"]));

            //    if (Rows.Count() == 0)
            //    {
            //        DataRow NewRow = ShroudedProfilesOnStorageDT.NewRow();
            //        NewRow["TechStoreID"] = Srows[0]["TechStoreID"];
            //        NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
            //        NewRow["Length"] = Srows[0]["Length"];
            //        NewRow["CoverName"] = Srows[0]["CoverName"];
            //        NewRow["CurrentCount"] = TotalCount;
            //        ShroudedProfilesOnStorageDT.Rows.Add(NewRow);
            //    }
            //    else
            //        Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            //}
        }

        public void GroupAssembledProfiles()
        {
            //DataTable DistinctTable = new DataTable();
            //DataTable TempDT = AssembledProfilesOnStorageDT.Copy();

            //AssembledProfilesOnStorageDT.Clear();

            //using (DataView DV = new DataView(TempDT, string.Empty, "TechStoreName, Length", DataViewRowState.CurrentRows))
            //{
            //    DistinctTable = DV.ToTable(true, "TechStoreID", "Length");
            //}

            //for (int i = 0; i < DistinctTable.Rows.Count; i++)
            //{
            //    DataRow[] Srows = TempDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]));
            //    if (Srows.Count() == 0)
            //        continue;

            //    int TotalCount = 0;
            //    foreach (DataRow row in Srows)
            //        TotalCount += Convert.ToInt32(row["CurrentCount"]);

            //    DataRow[] Rows = AssembledProfilesOnStorageDT.Select("TechStoreID=" + Convert.ToInt32(DistinctTable.Rows[i]["TechStoreID"]) +
            //           " AND Length=" + Convert.ToInt32(DistinctTable.Rows[i]["Length"]));

            //    if (Rows.Count() == 0)
            //    {
            //        DataRow NewRow = AssembledProfilesOnStorageDT.NewRow();
            //        NewRow["TechStoreID"] = Srows[0]["TechStoreID"];
            //        NewRow["TechStoreName"] = Srows[0]["TechStoreName"];
            //        NewRow["Length"] = Srows[0]["Length"];
            //        NewRow["CurrentCount"] = TotalCount;
            //        AssembledProfilesOnStorageDT.Rows.Add(NewRow);
            //    }
            //    else
            //        Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + TotalCount;
            //}
        }

        public void CompareAssignments()
        {
            for (int i = 0; i < AssignmentsDT.Rows.Count; i++)
            {
                if (AssignmentsDT.Rows[i]["DecorAssignmentID"] == DBNull.Value)
                    continue;
                int DecorAssignmentID = Convert.ToInt32(AssignmentsDT.Rows[i]["DecorAssignmentID"]);
                DataRow[] rows = AssignmentsBeforeUpdateDT.Select("DecorAssignmentID=" + DecorAssignmentID);
                if (rows.Count() == 0)
                {
                    if (AssignmentsDT.Rows[i]["InPlan"] != DBNull.Value && !Convert.ToBoolean(AssignmentsDT.Rows[i]["InPlan"]))
                        continue;
                    int ProductType = 0;
                    int ClientID = 0;
                    int MegaOrderID = 0;
                    int MainOrderID = 0;
                    int DecorOrderID = 0;
                    int TechStoreID2 = 0;
                    int TechStoreID1 = 0;
                    int CoverID2 = 0;
                    int Length2 = 0;
                    int PlanCount = 0;
                    int BarberanNumber = 0;
                    int FrezerNumber = 0;
                    decimal Width2 = 0;
                    bool AlreadyAdded = false;

                    if (AssignmentsDT.Rows[i]["ProductType"] != DBNull.Value)
                        ProductType = Convert.ToInt32(AssignmentsDT.Rows[i]["ProductType"]);
                    if (AssignmentsDT.Rows[i]["BarberanNumber"] != DBNull.Value)
                        BarberanNumber = Convert.ToInt32(AssignmentsDT.Rows[i]["BarberanNumber"]);
                    if (AssignmentsDT.Rows[i]["FrezerNumber"] != DBNull.Value)
                        FrezerNumber = Convert.ToInt32(AssignmentsDT.Rows[i]["FrezerNumber"]);
                    if (AssignmentsDT.Rows[i]["ClientID"] != DBNull.Value)
                        ClientID = Convert.ToInt32(AssignmentsDT.Rows[i]["ClientID"]);
                    if (AssignmentsDT.Rows[i]["MegaOrderID"] != DBNull.Value)
                        MegaOrderID = Convert.ToInt32(AssignmentsDT.Rows[i]["MegaOrderID"]);
                    if (AssignmentsDT.Rows[i]["MainOrderID"] != DBNull.Value)
                        MainOrderID = Convert.ToInt32(AssignmentsDT.Rows[i]["MainOrderID"]);
                    if (AssignmentsDT.Rows[i]["DecorOrderID"] != DBNull.Value)
                        DecorOrderID = Convert.ToInt32(AssignmentsDT.Rows[i]["DecorOrderID"]);
                    if (AssignmentsDT.Rows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(AssignmentsDT.Rows[i]["DecorAssignmentID"]);
                    if (AssignmentsDT.Rows[i]["TechStoreID2"] != DBNull.Value)
                        TechStoreID2 = Convert.ToInt32(AssignmentsDT.Rows[i]["TechStoreID2"]);
                    if (AssignmentsDT.Rows[i]["Length2"] != DBNull.Value)
                        Length2 = Convert.ToInt32(AssignmentsDT.Rows[i]["Length2"]);
                    if (AssignmentsDT.Rows[i]["CoverID2"] != DBNull.Value)
                        CoverID2 = Convert.ToInt32(AssignmentsDT.Rows[i]["CoverID2"]);
                    if (AssignmentsDT.Rows[i]["PlanCount"] != DBNull.Value)
                        PlanCount = Convert.ToInt32(AssignmentsDT.Rows[i]["PlanCount"]);

                    if (ProductType == 1)
                    {
                        TechStoreID1 = FindMilledProfileID(TechStoreID2, BarberanNumber);
                        if (TechStoreID1 != 0)
                        {
                            AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                            if (!AlreadyAdded)
                                AddMilledProfileToRequestFromEnv(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                        }
                        else
                        {
                            TechStoreID1 = FindShroudedAssembledProfileID(TechStoreID2, BarberanNumber);
                            if (TechStoreID1 != 0)
                            {
                                AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                                if (!AlreadyAdded)
                                    AddAssembledProfileToRequestFromEnv(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                            }
                            else
                            {
                                TechStoreID1 = FindMilledAssembledProfileID(TechStoreID2, BarberanNumber);
                                if (TechStoreID1 != 0)
                                {
                                    AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                                    if (!AlreadyAdded)
                                        AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                                }
                                else
                                {
                                    TechStoreID1 = FindSawStripID(TechStoreID2, BarberanNumber, ref Width2);
                                    if (TechStoreID1 != 0)
                                    {
                                        AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                                        if (!AlreadyAdded)
                                            AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                                    }
                                }
                            }
                        }
                    }
                    if (ProductType == 2)
                    {
                        TechStoreID1 = FindSawStripID(TechStoreID2, FrezerNumber, ref Width2);
                        if (TechStoreID1 != 0)
                        {
                            AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                            if (!AlreadyAdded)
                                AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                        }
                        else
                        {
                            TechStoreID1 = FindShroudedAssembledProfileID(TechStoreID2, FrezerNumber);
                            if (TechStoreID1 != 0)
                            {
                                AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                                if (!AlreadyAdded)
                                    AddAssembledProfileToRequestFromMil(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, PlanCount, DecorAssignmentID);
                            }
                        }
                    }
                    if (ProductType == 4)
                    {
                        ArrayList TechStoreIDs = new ArrayList();
                        bool b = FindShroudedProfileID(TechStoreID2, 1, TechStoreIDs);
                        if (b)
                        {
                            for (int j = 0; j < TechStoreIDs.Count; j++)
                            {
                                AddShroudedProfileToRequestFromAssembly(ClientID, MegaOrderID, MainOrderID, DecorOrderID, Convert.ToInt32(TechStoreIDs[j]), Length2, PlanCount, DecorAssignmentID);
                            }
                        }
                        else
                        {
                            FindMilledProfileID(TechStoreID2, 1, TechStoreIDs);
                            for (int j = 0; j < TechStoreIDs.Count; j++)
                            {
                                AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                                if (!AlreadyAdded)
                                    AddMilledProfileToRequestFromAssembly(ClientID, MegaOrderID, MainOrderID, DecorOrderID, Convert.ToInt32(TechStoreIDs[j]), Length2, PlanCount, DecorAssignmentID);
                            }
                        }
                    }
                    if (ProductType == 5)
                    {
                        TechStoreID1 = FindSawStripID(TechStoreID2, 1, ref Width2);
                        if (TechStoreID1 != 0)
                        {
                            AlreadyAdded = IsAssignmentAdded(DecorAssignmentID);
                            if (!AlreadyAdded)
                                AddSawStripsToRequest(ClientID, MegaOrderID, MainOrderID, DecorOrderID, TechStoreID1, Length2, Width2, PlanCount, DecorAssignmentID);
                        }
                        AddKashirToPlan(DecorAssignmentID);
                    }
                }
            }
        }

        public void UpdateDecorAssignments()
        {
            UpdateAssignments();
            SetOrderStatus();
            UpdateFacingMaterialAssignments();
            UpdateFacingRollersAssignments();
        }

        public void UpdateFacingMaterialAssignments()
        {
            string SelectCommand = @"SELECT * FROM DecorAssignments WHERE ProductType=8 AND (BatchAssignmentID IS NULL OR BatchAssignmentID=" + iBatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                FacingMaterialDT.Clear();
                DA.Fill(FacingMaterialDT);
            }
        }

        public void UpdateFacingRollersAssignments()
        {
            string SelectCommand = @"SELECT * FROM DecorAssignments WHERE ProductType=7 AND FacingMachine IS NOT NULL AND (BatchAssignmentID IS NULL OR BatchAssignmentID=" + iBatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                FacingRollersDT1.Clear();
                DA.Fill(FacingRollersDT1);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE ProductType=7 AND FacingMachine IS NULL AND (BatchAssignmentID IS NULL OR BatchAssignmentID=" + iBatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                FacingRollersDT2.Clear();
                DA.Fill(FacingRollersDT2);
            }
        }

        public void SetOrderStatus()
        {
            DataTable DT = new DataTable();
            using (DataView DV = new DataView(AssignmentsDT, "InPlan=0", string.Empty, DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "MegaOrderID" });
            }

            string filter = string.Empty; foreach (DataRow item in DT.Rows)
                filter += item["MegaOrderID"].ToString() + ",";
            if (filter.Length > 0)
                filter = " WHERE MegaOrderID IN (" + filter.Substring(0, filter.Length - 1) + ")";
            else
            {
                DT.Dispose();
                return;
            }
            string SelectCommand = @"SELECT ProfilOrderStatusID FROM MegaOrders" + filter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT2 = new DataTable())
                {
                    if (DA.Fill(DT2) > 0)
                    {
                        int MegaOrderID = 0;
                        int ProfilOrderStatusID = 0;

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            MegaOrderID = Convert.ToInt32(DT.Rows[i]["MegaOrderID"]);
                            ProfilOrderStatusID = 0;
                            DataRow[] rows = AssignmentsDT.Select("MegaOrderID=" + MegaOrderID);
                            for (int j = 0; j < rows.Count(); j++)
                            {
                                rows[j]["ProfilOrderStatusID"] = ProfilOrderStatusID;
                            }
                        }
                    }
                }
            }

            DT.Dispose();
        }

        public void UpdateAssignments()
        {
            string SelectCommand = @"SELECT * FROM DecorAssignments WHERE (BatchAssignmentID IS NULL OR BatchAssignmentID=" + iBatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                AssignmentsDT.Clear();
                DA.Fill(AssignmentsDT);
            }
        }

        public void FilterByProductionStatus(
            bool bsNotInProduction,
            bool bsOnProduction,
            bool bsInProduction,
            bool bsInStorage,
            bool bsOnExpedition,
            bool bsIsDispatched)
        {
            string Status = string.Empty;

            Status = "(ProfilOrderStatusID=0)";

            if (bsNotInProduction)
            {
                if (Status.Length > 0)
                {
                    Status += " OR (ProfilOrderStatusID=1)";
                }
                else
                {
                    Status = "(ProfilOrderStatusID=1)";
                }
            }

            if (bsInProduction)
            {
                if (Status.Length > 0)
                {
                    Status += " OR (ProfilOrderStatusID=2)";
                }
                else
                {
                    Status = "(ProfilOrderStatusID=2)";
                }
            }

            if (bsOnProduction)
            {
                if (Status.Length > 0)
                {
                    Status += " OR (ProfilOrderStatusID=5)";
                }
                else
                {
                    Status = "(ProfilOrderStatusID=5)";
                }
            }

            if (bsInStorage)
            {
                if (Status.Length > 0)
                {
                    Status += " OR (ProfilOrderStatusID=3)";
                }
                else
                {
                    Status = "(ProfilOrderStatusID=3)";
                }
            }
            if (bsOnExpedition)
            {
                if (Status.Length > 0)
                {
                    Status += " OR (ProfilOrderStatusID=6)";
                }
                else
                {
                    Status = "(ProfilOrderStatusID=6)";
                }
            }
            if (bsIsDispatched)
            {
                if (Status.Length > 0)
                {
                    Status += " OR (ProfilOrderStatusID=4)";
                }
                else
                {
                    Status = "(ProfilOrderStatusID=4)";
                }
            }
            if (Status.Length > 0)
            {
                Status = " InPlan=0 AND (" + Status + ")";
            }

            AssembledProfileRequestsBS.Filter = "ProductType=3 AND " + Status;
            ShroudedProfileRequestsBS.Filter = "ProductType=1 AND " + Status;
            KashirRequestsBS.Filter = "ProductType=5 AND " + Status;
            MilledProfileRequestsBS.Filter = "ProductType=2 AND " + Status;
            SawnStripsRequestsBS.Filter = "ProductType=4 AND " + Status;
        }

        public void FilterTechStoreSubGroups(int TechStoreGroupID)
        {
            TechStoreSubGroupsBS.Filter = "TechStoreGroupID = " + TechStoreGroupID;
            TechStoreSubGroupsBS.MoveFirst();
        }

        public void FilterFacingMaterialOnStorage(bool bShowAll, string Name)
        {
            if (bShowAll)
            {
                FacingMaterialOnStorageBS.RemoveFilter();

            }
            else
            {
                DataTable TempItemsDataTable = FacingMaterialOnStorageDT;
                using (DataView DV = new DataView(TempItemsDataTable))
                {
                    DV.RowFilter = "TechStoreName='" + Name + "'";

                    TempItemsDataTable = DV.ToTable();
                }
                string filter = string.Empty;
                for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                    filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["StoreItemID"]) + ",";
                if (filter.Length > 0)
                {
                    filter = filter.Substring(0, filter.Length - 1);
                    filter = "StoreItemID IN (" + filter + ")";
                }
                else
                    filter = "StoreItemID <> - 1";

                FacingMaterialOnStorageBS.Filter = filter;
            }
            FacingMaterialOnStorageBS.MoveFirst();
        }

        public void FilterFacingRollersOnStorage(bool bShowAll, string Name)
        {
            if (bShowAll)
            {
                FacingRollersOnStorageBS.RemoveFilter();

            }
            else
            {
                DataTable TempItemsDataTable = FacingRollersOnStorageDT;
                using (DataView DV = new DataView(TempItemsDataTable))
                {
                    DV.RowFilter = "TechStoreName='" + Name + "'";

                    TempItemsDataTable = DV.ToTable();
                }
                string filter = string.Empty;
                for (int i = 0; i < TempItemsDataTable.Rows.Count; i++)
                    filter += Convert.ToInt32(TempItemsDataTable.Rows[i]["StoreItemID"]) + ",";
                if (filter.Length > 0)
                {
                    filter = filter.Substring(0, filter.Length - 1);
                    filter = "StoreItemID IN (" + filter + ")";
                }
                else
                    filter = "StoreItemID <> - 1";
                FacingRollersOnStorageBS.Filter = filter;
            }
            FacingRollersOnStorageBS.MoveFirst();
        }

        public void FilterTechStore(int TechStoreSubGroupID)
        {
            TechStoreBS.Filter = "TechStoreSubGroupID = " + TechStoreSubGroupID;
            TechStoreBS.MoveFirst();
        }

        public void FilterFacingMaterialAssignments(bool bOnProd, bool bInProd, bool bOnStorage, int DecorAssignmentID)
        {
            string sFilter1 = "(PrevLinkAssignmentID=" + DecorAssignmentID + ")";
            sFilter1 = "BatchAssignmentID IS NOT NULL AND (" + sFilter1 + ")";
            FacingMaterialAssignmentsBS.Filter = sFilter1;
            FacingMaterialAssignmentsBS.MoveFirst();
        }

        public void FilterFacingRollersAssignments(bool bOnProd, bool bInProd, bool bOnStorage, int DecorAssignmentID)
        {
            string sFilter1 = "(PrevLinkAssignmentID=" + DecorAssignmentID + ")";
            FacingRollersAssignmentsBS.Filter = sFilter1;
            FacingRollersAssignmentsBS.MoveFirst();
        }

        public void SummarizeEqualKashir()
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(AssignmentsDT, "ProductType=5 AND BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "TechStoreID2", "CoverID2", "Length2", "Width2" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int Count = 0;
                DataRow[] rows = AssignmentsDT.Select("BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1 AND TechStoreID2=" + Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]) +
                    " AND CoverID2=" + Convert.ToInt32(DT1.Rows[i]["CoverID2"]) +
                    " AND Length2='" + Convert.ToDecimal(DT1.Rows[i]["Length2"]).ToString() + "'" +
                    " AND Width2='" + Convert.ToDecimal(DT1.Rows[i]["Width2"]).ToString() + "'");
                if (rows.Count() < 1)
                    continue;
                int NewLinkAssignmentID = 0;
                if (rows[0]["DecorAssignmentID"] != DBNull.Value)
                    NewLinkAssignmentID = Convert.ToInt32(rows[0]["DecorAssignmentID"]);
                foreach (DataRow item in rows)
                    Count += Convert.ToInt32(item["PlanCount"]);
                rows[0]["PlanCount"] = Count;
                for (int j = 1; j < rows.Count(); j++)
                {
                    int OldLinkAssignmentID = 0;
                    if (rows[j]["DecorAssignmentID"] != DBNull.Value)
                        OldLinkAssignmentID = Convert.ToInt32(rows[j]["DecorAssignmentID"]);
                    if (OldLinkAssignmentID != 0)
                    {
                        DataRow[] srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                    }
                    rows[j].Delete();
                }
            }
        }

        public void SummarizeEqualMilledProfiles()
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(AssignmentsDT, "ProductType=2 AND BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "TechStoreID2", "Length2", "FrezerNumber" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int Count = 0;
                DataRow[] rows = AssignmentsDT.Select("BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1 AND TechStoreID2=" + Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]) +
                    " AND Length2='" + Convert.ToDecimal(DT1.Rows[i]["Length2"]).ToString() + "'" +
                    " AND FrezerNumber=" + Convert.ToInt32(DT1.Rows[i]["FrezerNumber"]));
                if (rows.Count() < 1)
                    continue;
                int NewLinkAssignmentID = 0;
                if (rows[0]["DecorAssignmentID"] != DBNull.Value)
                    NewLinkAssignmentID = Convert.ToInt32(rows[0]["DecorAssignmentID"]);
                foreach (DataRow item in rows)
                    Count += Convert.ToInt32(item["PlanCount"]);
                rows[0]["PlanCount"] = Count;
                for (int j = 1; j < rows.Count(); j++)
                {
                    int OldLinkAssignmentID = 0;
                    if (rows[j]["DecorAssignmentID"] != DBNull.Value)
                        OldLinkAssignmentID = Convert.ToInt32(rows[j]["DecorAssignmentID"]);
                    if (OldLinkAssignmentID != 0)
                    {
                        DataRow[] srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                    }
                    rows[j].Delete();
                }
            }
        }

        public void SummarizeEqualAssembledProfiles()
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(AssignmentsDT, "ProductType=3 AND BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "TechStoreID2", "Length2" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int Count = 0;
                DataRow[] rows = AssignmentsDT.Select("BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1 AND TechStoreID2=" + Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]) +
                    " AND Length2='" + Convert.ToDecimal(DT1.Rows[i]["Length2"]).ToString() + "'");
                if (rows.Count() < 1)
                    continue;
                int NewLinkAssignmentID = 0;
                if (rows[0]["DecorAssignmentID"] != DBNull.Value)
                    NewLinkAssignmentID = Convert.ToInt32(rows[0]["DecorAssignmentID"]);
                foreach (DataRow item in rows)
                    Count += Convert.ToInt32(item["PlanCount"]);
                rows[0]["PlanCount"] = Count;
                for (int j = 1; j < rows.Count(); j++)
                {
                    int OldLinkAssignmentID = 0;
                    if (rows[j]["DecorAssignmentID"] != DBNull.Value)
                        OldLinkAssignmentID = Convert.ToInt32(rows[j]["DecorAssignmentID"]);
                    if (OldLinkAssignmentID != 0)
                    {
                        DataRow[] srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                    }
                    rows[j].Delete();
                }
            }

        }

        public void SummarizeEqualSawnStrips()
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(AssignmentsDT, "ProductType=4 AND BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "TechStoreID2", "Length2", "Width2" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int Count = 0;
                DataRow[] rows = AssignmentsDT.Select("BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1 AND TechStoreID2=" + Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]) +
                    " AND Length2='" + Convert.ToDecimal(DT1.Rows[i]["Length2"]).ToString() + "'" +
                    " AND Width2='" + Convert.ToDecimal(DT1.Rows[i]["Width2"]).ToString() + "'");
                if (rows.Count() < 1)
                    continue;
                int NewLinkAssignmentID = 0;
                if (rows[0]["DecorAssignmentID"] != DBNull.Value)
                    NewLinkAssignmentID = Convert.ToInt32(rows[0]["DecorAssignmentID"]);
                foreach (DataRow item in rows)
                    Count += Convert.ToInt32(item["PlanCount"]);
                rows[0]["PlanCount"] = Count;
                for (int j = 1; j < rows.Count(); j++)
                {
                    int OldLinkAssignmentID = 0;
                    if (rows[j]["DecorAssignmentID"] != DBNull.Value)
                        OldLinkAssignmentID = Convert.ToInt32(rows[j]["DecorAssignmentID"]);
                    if (OldLinkAssignmentID != 0)
                    {
                        DataRow[] srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                    }
                    rows[j].Delete();
                }
            }
        }

        public void SummarizeEqualShroudedProfiles()
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(AssignmentsDT, "ProductType=1 AND BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1", string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "TechStoreID2", "Length2", "CoverID2", "BarberanNumber" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int Count = 0;
                DataRow[] rows = AssignmentsDT.Select("BatchAssignmentID=" + iBatchAssignmentID + " AND InPlan=1 AND TechStoreID2=" + Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]) +
                    " AND Length2='" + Convert.ToDecimal(DT1.Rows[i]["Length2"]).ToString() + "'" +
                    " AND CoverID2=" + Convert.ToInt32(DT1.Rows[i]["CoverID2"]) + " AND BarberanNumber=" + Convert.ToInt32(DT1.Rows[i]["BarberanNumber"]));
                if (rows.Count() < 1)
                    continue;
                int NewLinkAssignmentID = 0;
                if (rows[0]["DecorAssignmentID"] != DBNull.Value)
                    NewLinkAssignmentID = Convert.ToInt32(rows[0]["DecorAssignmentID"]);
                foreach (DataRow item in rows)
                    Count += Convert.ToInt32(item["PlanCount"]);
                rows[0]["PlanCount"] = Count;
                for (int j = 1; j < rows.Count(); j++)
                {
                    int OldLinkAssignmentID = 0;
                    if (rows[j]["DecorAssignmentID"] != DBNull.Value)
                        OldLinkAssignmentID = Convert.ToInt32(rows[j]["DecorAssignmentID"]);
                    if (OldLinkAssignmentID != 0)
                    {
                        DataRow[] srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                        srows = AssignmentsDT.Select("PrevLinkAssignmentID=" + OldLinkAssignmentID);
                        if (srows.Count() > 0)
                            srows[0]["PrevLinkAssignmentID"] = NewLinkAssignmentID;
                    }
                    rows[j].Delete();
                }
            }
        }

        public bool IsAssignmentAdded(int DecorAssignmentID)
        {
            DataRow[] rows = AssignmentsDT.Select("PrevLinkAssignmentID=" + DecorAssignmentID);
            return rows.Count() > 0;
        }

        public bool IsAssigmentInProduction(int DecorAssignmentID)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return false;
            return Convert.ToInt32(rows[0]["DecorAssignmentStatusID"]) != 1;
        }

        public void MilledProfileToFrezer(int DecorAssignmentID, int FrezerNumber)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["FrezerNumber"] = FrezerNumber;
        }

        public void ShroudedProfileToBarberan(int DecorAssignmentID, int BarberanNumber)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["BarberanNumber"] = BarberanNumber;
        }

        public void ExcludeFromAssignments(int[] DecorAssignmentID)
        {
            for (int i = 0; i < DecorAssignmentID.Count(); i++)
            {
                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID[i]);
                if (rows.Count() == 0)
                    return;
                rows[0]["InPlan"] = false;
                rows[0]["AddToPlanUserID"] = DBNull.Value;
                rows[0]["AddToPlanDateTime"] = DBNull.Value;
                rows[0]["BatchAssignmentID"] = DBNull.Value;
                rows[0]["DecorAssignmentStatusID"] = 1;
            }
        }

        public void SplitRequest(int DecorAssignmentID, int Count)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["PlanCount"] = Convert.ToInt32(rows[0]["PlanCount"]) - Count;
            DataRow NewRow = AssignmentsDT.NewRow();
            NewRow.ItemArray = rows[0].ItemArray;
            NewRow["PlanCount"] = Count;
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void RemoveAssigments(int[] DecorAssignmentID)
        {
            for (int i = 0; i < DecorAssignmentID.Count(); i++)
            {
                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID[i]);
                if (rows.Count() == 0)
                    return;
                rows[0].Delete();
            }
        }

        public void CloseFacingMaterialRequest(int DecorAssignmentID)
        {
            DataRow[] rows = FacingMaterialDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["DecorAssignmentStatusID"] = 3;
            rows[0]["WriteOffFromStore"] = true;

            int StoreID = Convert.ToInt32(rows[0]["PrevLinkAssignmentID"]);
            int Count = Convert.ToInt32(rows[0]["PlanCount"]);
            int WriteOffMovementInvoiceID = 0;
            int RecipientStoreAllocID = 3;
            int SellerStoreAllocID = 1;
            DateTime CreateDateTime = Security.GetCurrentDate();
            WriteOffMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Списание обл. материала при резке на обл. ролики", 2);
            WriteOffFromStoreByStoreID(WriteOffMovementInvoiceID, CreateDateTime, StoreID, Count);

            rows = FacingRollersDT1.Select("PrevLinkAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            int ArrivalMovementInvoiceID = 0;
            ArrivalMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Приход обл. ролики после резки обл. материала", 1);
            for (int i = 0; i < rows.Count(); i++)
            {
                if (!Convert.ToBoolean(rows[i]["SaveToStore"]) && rows[i]["FactCount"] != DBNull.Value)
                {
                    rows[i]["DecorAssignmentStatusID"] = 3;
                    int StoreItemID = -1;
                    decimal Length = -1;
                    decimal Width = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (rows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(rows[i]["DecorAssignmentID"]);
                    if (rows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(rows[i]["TechStoreID2"]);
                    if (rows[i]["Thickness2"] != DBNull.Value)
                        Thickness = Convert.ToDecimal(rows[i]["Thickness2"]);
                    if (rows[i]["Diameter2"] != DBNull.Value)
                        Diameter = Convert.ToDecimal(rows[i]["Diameter2"]);
                    if (rows[i]["Width2"] != DBNull.Value)
                        Width = Convert.ToDecimal(rows[i]["Width2"]);
                    if (rows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(rows[i]["FactCount"]);
                    if (rows[i]["Notes"] != DBNull.Value)
                        Notes = rows[i]["Notes"].ToString();
                    int ManufactureStoreID = -1;
                    if (AssignmentsStoreManager.AddToManufactureStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length, Width, Height, Thickness,
                        Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref ManufactureStoreID))
                        rows[i]["SaveToStore"] = true;
                    AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                }
            }
            AssignmentsStoreManager.SaveMovementInvoiceDetails();
        }

        public void CloseFacingRollersRequest(int DecorAssignmentID)
        {
            DataRow[] rows = FacingRollersDT2.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["WriteOffFromStore"] = true;

            int StoreID = Convert.ToInt32(rows[0]["PrevLinkAssignmentID"]);
            int Count = Convert.ToInt32(rows[0]["PlanCount"]);
            int WriteOffMovementInvoiceID = 0;
            int RecipientStoreAllocID = 3;
            int SellerStoreAllocID = 3;
            DateTime CreateDateTime = Security.GetCurrentDate();
            WriteOffMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Списание обл. ролики при резке на обл. ролики", 2);
            WriteOffFromManufactureStoreByManufactureStoreID(WriteOffMovementInvoiceID, CreateDateTime, StoreID, Count);

            rows = FacingRollersDT2.Select("PrevLinkAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            int ArrivalMovementInvoiceID = 0;
            ArrivalMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Приход обл. ролики после резки обл. ролики", 1);
            for (int i = 0; i < rows.Count(); i++)
            {
                if (!Convert.ToBoolean(rows[i]["SaveToStore"]) && rows[i]["FactCount"] != DBNull.Value)
                {
                    rows[i]["DecorAssignmentStatusID"] = 3;
                    int StoreItemID = -1;
                    decimal Length = -1;
                    decimal Width = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (rows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(rows[i]["DecorAssignmentID"]);
                    if (rows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(rows[i]["TechStoreID2"]);
                    if (rows[i]["Thickness2"] != DBNull.Value)
                        Thickness = Convert.ToDecimal(rows[i]["Thickness2"]);
                    if (rows[i]["Diameter2"] != DBNull.Value)
                        Diameter = Convert.ToDecimal(rows[i]["Diameter2"]);
                    if (rows[i]["Width2"] != DBNull.Value)
                        Width = Convert.ToDecimal(rows[i]["Width2"]);
                    if (rows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(rows[i]["FactCount"]);
                    if (rows[i]["Notes"] != DBNull.Value)
                        Notes = rows[i]["Notes"].ToString();
                    int ManufactureStoreID = -1;
                    if (AssignmentsStoreManager.AddToManufactureStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length, Width, Height, Thickness,
                        Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref ManufactureStoreID))
                        rows[i]["SaveToStore"] = true;
                    AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                }
            }
            AssignmentsStoreManager.SaveMovementInvoiceDetails();
        }

        public bool AddFacingMaterialToRequest(int TechStoreID2, decimal Thickness2, int Length2, decimal Width2, int PlanCount, int StoreID, string FacingMachine, string Notes)
        {
            DataRow[] rows = FacingMaterialDT.Select("InPlan=0 AND PrevLinkAssignmentID=" + StoreID);
            if (rows.Count() > 0)
                return false;

            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = FacingMaterialDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["Notes"] = Notes;
            NewRow["Thickness2"] = Thickness2;
            NewRow["Length2"] = Length2;
            NewRow["Width2"] = Width2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = true;
            NewRow["PrevLinkAssignmentID"] = StoreID;
            NewRow["BatchAssignmentID"] = iBatchAssignmentID;
            NewRow["DecorAssignmentStatusID"] = 1;
            NewRow["ProductType"] = 8;
            NewRow["ClientID"] = 0;
            NewRow["MegaOrderID"] = 0;
            NewRow["MainOrderID"] = 0;
            NewRow["DecorOrderID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            NewRow["AddToPlanUserID"] = Security.CurrentUserID;
            NewRow["AddToPlanDateTime"] = Security.GetCurrentDate();
            NewRow["FacingMachine"] = FacingMachine;
            FacingMaterialDT.Rows.Add(NewRow);

            return true;
        }

        public bool AddFacingRollersToRequest(int TechStoreID2, decimal Thickness2, decimal Diameter2, decimal Width2, int PlanCount, int StoreID, string Notes)
        {
            DataRow[] rows = FacingRollersDT2.Select("InPlan=0 AND PrevLinkAssignmentID=" + StoreID);
            if (rows.Count() > 0)
                return false;
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = FacingRollersDT2.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["Notes"] = Notes;
            NewRow["Thickness2"] = Thickness2;
            NewRow["Diameter2"] = Diameter2;
            NewRow["Width2"] = Width2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = true;
            NewRow["PrevLinkAssignmentID"] = StoreID;
            NewRow["BatchAssignmentID"] = iBatchAssignmentID;
            NewRow["DecorAssignmentStatusID"] = 1;
            NewRow["ProductType"] = 7;
            NewRow["ClientID"] = 0;
            NewRow["MegaOrderID"] = 0;
            NewRow["MainOrderID"] = 0;
            NewRow["DecorOrderID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = CurrentDate;
            NewRow["AddToPlanUserID"] = Security.CurrentUserID;
            NewRow["AddToPlanDateTime"] = CurrentDate;
            FacingRollersDT2.Rows.Add(NewRow);

            return true;
        }

        public bool AddFacingMaterialToAssignment(int StoreID, decimal Thickness2, string FacingMachine)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = FacingRollersDT1.NewRow();
            NewRow["InPlan"] = true;
            NewRow["PrevLinkAssignmentID"] = StoreID;
            NewRow["Thickness2"] = Thickness2;
            NewRow["BatchAssignmentID"] = iBatchAssignmentID;
            NewRow["DecorAssignmentStatusID"] = 2;
            NewRow["ProductType"] = 7;
            NewRow["PrevLinkAssignmentID"] = StoreID;
            NewRow["FacingMachine"] = FacingMachine;
            NewRow["ClientID"] = 0;
            NewRow["MegaOrderID"] = 0;
            NewRow["MainOrderID"] = 0;
            NewRow["DecorOrderID"] = 0;
            NewRow["DisprepancyCount"] = 0;
            NewRow["DefectCount"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            NewRow["AddToPlanUserID"] = Security.CurrentUserID;
            NewRow["AddToPlanDateTime"] = Security.GetCurrentDate();
            FacingRollersDT1.Rows.Add(NewRow);

            return true;
        }

        public bool AddFacingRollersToAssignment(int StoreID, int TechStoreID2, decimal Thickness2, decimal Diameter2, decimal Width2)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = FacingRollersDT2.NewRow();
            NewRow["InPlan"] = true;
            NewRow["PrevLinkAssignmentID"] = StoreID;
            NewRow["BatchAssignmentID"] = iBatchAssignmentID;
            NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["DecorAssignmentStatusID"] = 2;
            NewRow["Thickness2"] = Thickness2;
            NewRow["Diameter2"] = Diameter2;
            NewRow["Width2"] = Width2;
            NewRow["ProductType"] = 7;
            NewRow["ClientID"] = 0;
            NewRow["MegaOrderID"] = 0;
            NewRow["MainOrderID"] = 0;
            NewRow["DecorOrderID"] = 0;
            NewRow["DisprepancyCount"] = 0;
            NewRow["DefectCount"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = CurrentDate;
            NewRow["AddToPlanUserID"] = Security.CurrentUserID;
            NewRow["AddToPlanDateTime"] = CurrentDate;
            FacingRollersDT2.Rows.Add(NewRow);

            return true;
        }

        public void CopyPasteFacingMaterial()
        {
            DataRow row = ((DataRowView)FacingMaterialAssignmentsBS.Current).Row;

            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = FacingRollersDT1.NewRow();
            NewRow.ItemArray = row.ItemArray;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = CurrentDate;
            NewRow["AddToPlanUserID"] = Security.CurrentUserID;
            NewRow["AddToPlanDateTime"] = CurrentDate;
            FacingRollersDT1.Rows.Add(NewRow);
        }

        public void CopyPasteFacingRollers()
        {
            DataRow row = ((DataRowView)FacingRollersAssignmentsBS.Current).Row;

            DataRow NewRow = FacingRollersDT2.NewRow();
            NewRow.ItemArray = row.ItemArray;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            NewRow["AddToPlanUserID"] = Security.CurrentUserID;
            NewRow["AddToPlanDateTime"] = Security.GetCurrentDate();
            FacingRollersDT2.Rows.Add(NewRow);
        }

        public void AddTransferredRoller(int DecorAssignmentID, int ManufactureStoreID, int Count)
        {
            int WriteOffMovementInvoiceID = 0;
            int RecipientStoreAllocID = 3;
            int SellerStoreAllocID = 3;
            DateTime CreateDateTime = Security.GetCurrentDate();
            WriteOffMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Передано в пр-во обл. ролики", 2);
            WriteOffFromManufactureStoreByManufactureStoreID(WriteOffMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);

            DataRow row = ((DataRowView)RollersInColorOnStorageBS.Current).Row;

            DataRow NewRow = TransferredRollersDT.NewRow();
            NewRow["TechStoreName"] = row["TechStoreName"];
            NewRow["Notes"] = row["Notes"];
            NewRow["Thickness"] = row["Thickness"];
            NewRow["StoreItemID"] = row["StoreItemID"];
            NewRow["Diameter"] = row["Diameter"];
            NewRow["Width"] = row["Width"];
            NewRow["Count"] = Count;
            NewRow["ReturnType"] = 0;
            NewRow["DecorAssignmentID"] = DecorAssignmentID;
            NewRow["ManufactureStoreID"] = ManufactureStoreID;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = CreateDateTime;
            TransferredRollersDT.Rows.Add(NewRow);

            AssignmentsStoreManager.SaveMovementInvoiceDetails();
        }

        public void AddReturnRoller(int DecorAssignmentID, decimal Diameter, decimal Width, int Count)
        {
            DateTime CreateDateTime = Security.GetCurrentDate();
            DataRow row = ((DataRowView)TransferredRollersBS.Current).Row;

            DataRow NewRow = TransferredRollersDT.NewRow();
            NewRow["TechStoreName"] = row["TechStoreName"];
            NewRow["Notes"] = row["Notes"];
            NewRow["Thickness"] = row["Thickness"];
            NewRow["StoreItemID"] = row["StoreItemID"];
            NewRow["Diameter"] = Diameter;
            NewRow["Width"] = Width;
            NewRow["Count"] = Count;
            NewRow["ReturnType"] = 1;
            NewRow["DecorAssignmentID"] = DecorAssignmentID;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = CreateDateTime;

            int StoreItemID = -1;
            decimal Length = -1;
            decimal Height = -1;
            decimal Thickness = -1;
            decimal Admission = -1;
            decimal Capacity = -1;
            decimal Weight = -1;
            int ColorID = -1;
            int PatinaID = -1;
            int CoverID = -1;
            int FactoryID = 1;
            string Notes = string.Empty;

            int RecipientStoreAllocID = 3;
            int SellerStoreAllocID = 3;
            int ArrivalMovementInvoiceID = 0;
            ArrivalMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Возвращено из пр-ва обл. ролики", 1);

            if (row["StoreItemID"] != DBNull.Value)
                StoreItemID = Convert.ToInt32(row["StoreItemID"]);
            if (row["Thickness"] != DBNull.Value)
                Thickness = Convert.ToDecimal(row["Thickness"]);

            if (row["Notes"] != DBNull.Value)
                Notes = row["Notes"].ToString();
            int ManufactureStoreID = -1;
            if (AssignmentsStoreManager.AddToManufactureStore(CreateDateTime, ArrivalMovementInvoiceID, StoreItemID, Length, Width, Height, Thickness,
                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref ManufactureStoreID))
                AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
            NewRow["ManufactureStoreID"] = ManufactureStoreID;
            TransferredRollersDT.Rows.Add(NewRow);
        }

        public void RemoveReturnedRoller()
        {
            if (ReturnedRollersBS.Current != null)
            {
                DataRow row = ((DataRowView)ReturnedRollersBS.Current).Row;
                int ManufactureStoreID = Convert.ToInt32(row["ManufactureStoreID"]);
                decimal Count = Convert.ToDecimal(row["Count"]);
                int WriteOffMovementInvoiceID = 0;
                int RecipientStoreAllocID = 3;
                int SellerStoreAllocID = 3;
                DateTime CreateDateTime = Security.GetCurrentDate();
                WriteOffMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                    Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Возвращено из пр-ва обл. ролики: откатить", 2);
                ReturnToManufactureStoreByManufactureStoreID(WriteOffMovementInvoiceID, CreateDateTime, ManufactureStoreID, -Count);

                ReturnedRollersBS.RemoveCurrent();
            }
        }

        public void RemoveTransferredRoller()
        {
            if (TransferredRollersBS.Current != null)
            {
                DataRow row = ((DataRowView)TransferredRollersBS.Current).Row;
                int ManufactureStoreID = Convert.ToInt32(row["ManufactureStoreID"]);
                decimal Count = Convert.ToDecimal(row["Count"]);

                int RecipientStoreAllocID = 3;
                int SellerStoreAllocID = 3;
                int ArrivalMovementInvoiceID = 0;
                DateTime CreateDateTime = Security.GetCurrentDate();
                ArrivalMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                    Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Передано в пр-во обл. ролики: откатить", 1);

                ReturnToManufactureStoreByManufactureStoreID(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);

                TransferredRollersBS.RemoveCurrent();
            }
        }

        public void AddKashirToPlan(int DecorAssignmentID)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;

            rows[0]["ProductType"] = 5;
            rows[0]["InPlan"] = true;
            rows[0]["BatchAssignmentID"] = iBatchAssignmentID;
        }

        public void AddMilledProfileToPlan(int DecorAssignmentID, int FrezerNumber)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;

            rows[0]["ProductType"] = 2;
            rows[0]["FrezerNumber"] = FrezerNumber;
            rows[0]["InPlan"] = true;
            rows[0]["BatchAssignmentID"] = iBatchAssignmentID;
        }

        public void AddSawnStripsToPlan(int DecorAssignmentID, int SawNumber, string SawName, decimal Width2)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["Width2"] = Width2;
            rows[0]["ProductType"] = 4;
            rows[0]["SawName"] = SawName;
            rows[0]["SawNumber"] = SawNumber;
            rows[0]["InPlan"] = true;
            rows[0]["BatchAssignmentID"] = iBatchAssignmentID;
        }

        public void AddShroudedProfileToPlan(int DecorAssignmentID, int BarberanNumber)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            rows[0]["ProductType"] = 1;
            rows[0]["BarberanNumber"] = BarberanNumber;
            rows[0]["InPlan"] = true;
            rows[0]["BatchAssignmentID"] = iBatchAssignmentID;
        }

        public void AddAssembledProfileToPlan(int DecorAssignmentID, ArrayList TechStoreIDs)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;
            int PrevLinkAssignmentID = 0;
            if (rows[0]["PrevLinkAssignmentID"] != DBNull.Value)
                PrevLinkAssignmentID = Convert.ToInt32(rows[0]["PrevLinkAssignmentID"]);
            for (int i = 1; i < TechStoreIDs.Count; i++)
            {
                DataRow NewRow = AssignmentsDT.NewRow();
                NewRow.ItemArray = rows[0].ItemArray;
                if (PrevLinkAssignmentID != 0)
                    NewRow["PrevLinkAssignmentID"] = PrevLinkAssignmentID;
                NewRow["BatchAssignmentID"] = iBatchAssignmentID;
                NewRow["InPlan"] = true;
                NewRow["CreationUserID"] = Security.CurrentUserID;
                NewRow["CreationDateTime"] = Security.GetCurrentDate();
                AssignmentsDT.Rows.Add(NewRow);
            }
            rows[0]["ProductType"] = 3;
            rows[0]["BatchAssignmentID"] = iBatchAssignmentID;
            rows[0]["InPlan"] = true;
        }

        public void AddShroudedProfileToRequestFromAssembly(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int TechStoreID2, int Length2, int PlanCount, int DecorAssignmentID)
        {
            DataRow NewRow = AssignmentsDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["ClientID"] = ClientID;
            NewRow["MegaOrderID"] = MegaOrderID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DecorOrderID"] = DecorOrderID;
            NewRow["Length2"] = Length2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = false;
            NewRow["PrevLinkAssignmentID"] = DecorAssignmentID;
            NewRow["ProductType"] = 1;
            NewRow["ProfilOrderStatusID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void AddAssembledProfileToRequestFromMil(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int TechStoreID2, int Length2, int PlanCount, int DecorAssignmentID)
        {
            DataRow NewRow = AssignmentsDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["ClientID"] = ClientID;
            NewRow["MegaOrderID"] = MegaOrderID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DecorOrderID"] = DecorOrderID;
            NewRow["Length2"] = Length2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = false;
            NewRow["PrevLinkAssignmentID"] = DecorAssignmentID;
            NewRow["ProductType"] = 3;
            NewRow["ProfilOrderStatusID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void AddAssembledProfileToRequestFromEnv(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int TechStoreID2, int Length2, int PlanCount, int DecorAssignmentID)
        {
            DataRow NewRow = AssignmentsDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["ClientID"] = ClientID;
            NewRow["MegaOrderID"] = MegaOrderID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DecorOrderID"] = DecorOrderID;
            NewRow["Length2"] = Length2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = false;
            NewRow["PrevLinkAssignmentID"] = DecorAssignmentID;
            NewRow["ProductType"] = 3;
            NewRow["ProfilOrderStatusID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void AddMilledProfileToRequestFromEnv(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int TechStoreID2, int Length2, int PlanCount, int DecorAssignmentID)
        {
            DataRow NewRow = AssignmentsDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["ClientID"] = ClientID;
            NewRow["MegaOrderID"] = MegaOrderID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DecorOrderID"] = DecorOrderID;
            NewRow["Length2"] = Length2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = false;
            NewRow["PrevLinkAssignmentID"] = DecorAssignmentID;
            NewRow["ProductType"] = 2;
            NewRow["ProfilOrderStatusID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void AddMilledProfileToRequestFromAssembly(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int TechStoreID2, int Length2, int PlanCount, int DecorAssignmentID)
        {
            DataRow NewRow = AssignmentsDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["ClientID"] = ClientID;
            NewRow["MegaOrderID"] = MegaOrderID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DecorOrderID"] = DecorOrderID;
            NewRow["Length2"] = Length2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = false;
            NewRow["PrevLinkAssignmentID"] = DecorAssignmentID;
            NewRow["ProductType"] = 2;
            NewRow["ProfilOrderStatusID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void AddSawStripsToRequest(int ClientID, int MegaOrderID, int MainOrderID, int DecorOrderID, int TechStoreID2, int Length2, decimal Width2, int PlanCount, int DecorAssignmentID)
        {
            DataRow NewRow = AssignmentsDT.NewRow();
            if (TechStoreID2 != 0)
                NewRow["TechStoreID2"] = TechStoreID2;
            NewRow["ClientID"] = ClientID;
            NewRow["MegaOrderID"] = MegaOrderID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DecorOrderID"] = DecorOrderID;
            NewRow["Length2"] = Length2;
            NewRow["Width2"] = Width2;
            NewRow["PlanCount"] = PlanCount;
            NewRow["InPlan"] = false;
            NewRow["PrevLinkAssignmentID"] = DecorAssignmentID;
            NewRow["ProductType"] = 4;
            NewRow["ProfilOrderStatusID"] = 0;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            AssignmentsDT.Rows.Add(NewRow);
        }

        public void EditParametersInPrevLink(int PrevLinkAssignmentID, string Parameter, decimal NewValue)
        {
            while (PrevLinkAssignmentID != 0)
            {
                DataRow[] rows = DecorAssignmentsDT.Select("DecorAssignmentID=" + PrevLinkAssignmentID);
                if (rows.Count() > 0)
                {
                    int ProductType = Convert.ToInt32(rows[0]["ProductType"]);
                    //PrevLinkAssignmentID = Convert.ToInt32(rows[0]["PrevLinkAssignmentID"]);
                    switch (ProductType)
                    {
                        case 1:
                            {
                                PrevLinkAssignmentID = EditPrevLinkShroudedProfile(PrevLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 2:
                            {
                                PrevLinkAssignmentID = EditPrevLinkMilledProfile(PrevLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 3:
                            {
                                PrevLinkAssignmentID = EditPrevLinkAssembledProfile(PrevLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 4:
                            {
                                PrevLinkAssignmentID = EditPrevLinkSawnStrip(PrevLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 5:
                            {
                                PrevLinkAssignmentID = EditPrevLinkKashir(PrevLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                else
                    break;
            }
        }

        public void EditParametersInNextLink(int NextLinkAssignmentID, string Parameter, decimal NewValue)
        {
            while (NextLinkAssignmentID != 0)
            {
                DataRow[] rows = DecorAssignmentsDT.Select("DecorAssignmentID=" + NextLinkAssignmentID);
                if (rows.Count() > 0)
                {
                    int ProductType = Convert.ToInt32(rows[0]["ProductType"]);
                    //PrevLinkAssignmentID = Convert.ToInt32(rows[0]["PrevLinkAssignmentID"]);
                    switch (ProductType)
                    {
                        case 1:
                            {
                                NextLinkAssignmentID = EditNextLinkShroudedProfile(NextLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 2:
                            {
                                NextLinkAssignmentID = EditNextLinkMilledProfile(NextLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 3:
                            {
                                NextLinkAssignmentID = EditNextLinkAssembledProfile(NextLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 4:
                            {
                                NextLinkAssignmentID = EditNextLinkSawnStrip(NextLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        case 5:
                            {
                                NextLinkAssignmentID = EditNextLinkKashir(NextLinkAssignmentID, Parameter, NewValue);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                else
                    break;
            }
        }

        public int EditPrevLinkAssembledProfile(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int PrevLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["PrevLinkAssignmentID"] != DBNull.Value)
                        PrevLinkAssignmentID = Convert.ToInt32(item["PrevLinkAssignmentID"]);
                }
            }
            return PrevLinkAssignmentID;
        }

        public int EditPrevLinkShroudedProfile(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int PrevLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["PrevLinkAssignmentID"] != DBNull.Value)
                        PrevLinkAssignmentID = Convert.ToInt32(item["PrevLinkAssignmentID"]);
                }
            }
            return PrevLinkAssignmentID;
        }

        public int EditPrevLinkSawnStrip(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int PrevLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["PrevLinkAssignmentID"] != DBNull.Value)
                        PrevLinkAssignmentID = Convert.ToInt32(item["PrevLinkAssignmentID"]);
                }
            }
            return PrevLinkAssignmentID;
        }

        public int EditPrevLinkKashir(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int PrevLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["PrevLinkAssignmentID"] != DBNull.Value)
                        PrevLinkAssignmentID = Convert.ToInt32(item["PrevLinkAssignmentID"]);
                }
            }
            return PrevLinkAssignmentID;
        }

        public int EditPrevLinkMilledProfile(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int PrevLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["PrevLinkAssignmentID"] != DBNull.Value)
                        PrevLinkAssignmentID = Convert.ToInt32(item["PrevLinkAssignmentID"]);
                }
            }
            return PrevLinkAssignmentID;
        }

        public int EditNextLinkAssembledProfile(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int NextLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["NextLinkAssignmentID"] != DBNull.Value)
                        NextLinkAssignmentID = Convert.ToInt32(item["NextLinkAssignmentID"]);
                }
            }
            return NextLinkAssignmentID;
        }

        public int EditNextLinkShroudedProfile(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int NextLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["NextLinkAssignmentID"] != DBNull.Value)
                        NextLinkAssignmentID = Convert.ToInt32(item["NextLinkAssignmentID"]);
                }
            }
            return NextLinkAssignmentID;
        }

        public int EditNextLinkSawnStrip(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int NextLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["NextLinkAssignmentID"] != DBNull.Value)
                        NextLinkAssignmentID = Convert.ToInt32(item["NextLinkAssignmentID"]);
                }
            }
            return NextLinkAssignmentID;
        }

        public int EditNextLinkKashir(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int NextLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["NextLinkAssignmentID"] != DBNull.Value)
                        NextLinkAssignmentID = Convert.ToInt32(item["NextLinkAssignmentID"]);
                }
            }
            return NextLinkAssignmentID;
        }

        public int EditNextLinkMilledProfile(int DecorAssignmentID, string Parameter, decimal NewValue)
        {
            int NextLinkAssignmentID = 0;
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() > 0 && rows[0]["PrintUserID"] == DBNull.Value)
            {
                foreach (var item in rows)
                {
                    if (item[Parameter] != DBNull.Value)
                        item[Parameter] = NewValue;
                    if (item["NextLinkAssignmentID"] != DBNull.Value)
                        NextLinkAssignmentID = Convert.ToInt32(item["NextLinkAssignmentID"]);
                }
            }
            return NextLinkAssignmentID;
        }

        public void PreSaveAssigments()
        {
            SummarizeEqualKashir();
            SummarizeEqualShroudedProfiles();
            SummarizeEqualMilledProfiles();
            SummarizeEqualSawnStrips();
            SummarizeEqualAssembledProfiles();

            bool CreateMovementInvoice = false;
            int ArrivalMovementInvoiceID = 0;
            int WriteOffMovementInvoiceID = 0;
            int RecipientStoreAllocID = 3;
            int SellerStoreAllocID = 3;
            DateTime CreateDateTime = Security.GetCurrentDate();
            DataRow[] rows = AssignmentsDT.Select("FactCount IS NOT NULL AND DefectCount IS NOT NULL AND DisprepancyCount IS NOT NULL AND SaveToStore=0");
            if (rows.Count() > 0)
            {
                CreateMovementInvoice = true;
                ArrivalMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                    Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Приход в рабочую зону", 1);
            }
            rows = AssignmentsDT.Select("FactCount IS NOT NULL AND DefectCount IS NOT NULL AND DisprepancyCount IS NOT NULL AND WriteOffFromStore=0");
            if (rows.Count() > 0)
            {
                CreateMovementInvoice = true;
                WriteOffMovementInvoiceID = AssignmentsStoreManager.SaveMovementInvoices(CreateDateTime, SellerStoreAllocID, RecipientStoreAllocID, 0,
                    Security.CurrentUserID, Security.CurrentUserShortName, Security.CurrentUserID, 0, 0, string.Empty, "Списание в рабочую зону", 2);
            }

            DataTable DT = AssignmentsDT.Clone();
            rows = AssignmentsDT.Select("ProductType=4");
            PreSaveSawnStrips(ArrivalMovementInvoiceID, WriteOffMovementInvoiceID, CreateDateTime, ref rows);

            DT.Clear();
            rows = AssignmentsDT.Select("ProductType=5");
            PreSaveKashir(ArrivalMovementInvoiceID, WriteOffMovementInvoiceID, CreateDateTime, ref rows);

            DT.Clear();
            rows = AssignmentsDT.Select("ProductType=2");
            PreSaveMilledProfile(ArrivalMovementInvoiceID, WriteOffMovementInvoiceID, CreateDateTime, ref rows);

            DT.Clear();
            rows = AssignmentsDT.Select("ProductType=3");
            PreSaveAssembledProfile(ArrivalMovementInvoiceID, WriteOffMovementInvoiceID, CreateDateTime, ref rows);

            DT.Clear();
            rows = AssignmentsDT.Select("ProductType=1");
            PreSaveShroudedProfile(ArrivalMovementInvoiceID, WriteOffMovementInvoiceID, CreateDateTime, ref rows);

            if (CreateMovementInvoice)
                AssignmentsStoreManager.SaveMovementInvoiceDetails();
        }

        private void PreSaveSawnStrips(int ArrivalMovementInvoiceID, int WriteOffMovementInvoiceID, DateTime CreateDateTime, ref DataRow[] SawnStripsRows)
        {
            for (int i = 0; i < SawnStripsRows.Count(); i++)
            {
                if (SawnStripsRows[i].RowState == DataRowState.Deleted || Convert.ToInt32(SawnStripsRows[i]["InPlan"]) == 0)
                    continue;
                int BarberanNumber = 0;
                int FrezerNumber = 0;
                int ProductType = 0;
                int TechStoreID1Old = 0;
                int TechStoreID1New = 0;
                int TechStoreID2 = 0;
                decimal Width1 = 0;
                if (SawnStripsRows[i]["ProductType"] != DBNull.Value)
                    ProductType = Convert.ToInt32(SawnStripsRows[i]["ProductType"]);
                if (SawnStripsRows[i]["BarberanNumber"] != DBNull.Value)
                    BarberanNumber = Convert.ToInt32(SawnStripsRows[i]["BarberanNumber"]);
                if (SawnStripsRows[i]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(SawnStripsRows[i]["FrezerNumber"]);
                if (SawnStripsRows[i]["ClientID"] == DBNull.Value)
                    SawnStripsRows[i]["ClientID"] = 0;
                if (SawnStripsRows[i]["MegaOrderID"] == DBNull.Value)
                    SawnStripsRows[i]["MegaOrderID"] = 0;
                if (SawnStripsRows[i]["MainOrderID"] == DBNull.Value)
                    SawnStripsRows[i]["MainOrderID"] = 0;
                if (SawnStripsRows[i]["DecorOrderID"] == DBNull.Value)
                    SawnStripsRows[i]["DecorOrderID"] = 0;
                if (SawnStripsRows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1Old = Convert.ToInt32(SawnStripsRows[i]["TechStoreID1"]);
                if (SawnStripsRows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(SawnStripsRows[i]["TechStoreID2"]);
                if (SawnStripsRows[i]["CreationUserID"] == DBNull.Value)
                    SawnStripsRows[i]["CreationUserID"] = Security.CurrentUserID;
                if (SawnStripsRows[i]["CreationDateTime"] == DBNull.Value)
                    SawnStripsRows[i]["CreationDateTime"] = CreateDateTime;
                if (SawnStripsRows[i]["AddToPlanUserID"] == DBNull.Value)
                    SawnStripsRows[i]["AddToPlanUserID"] = Security.CurrentUserID;
                if (SawnStripsRows[i]["AddToPlanDateTime"] == DBNull.Value)
                    SawnStripsRows[i]["AddToPlanDateTime"] = CreateDateTime;
                if (SawnStripsRows[i]["FactCount"] != DBNull.Value && SawnStripsRows[i]["DisprepancyCount"] != DBNull.Value && SawnStripsRows[i]["DefectCount"] != DBNull.Value)
                    SawnStripsRows[i]["DecorAssignmentStatusID"] = 3;
                if (SawnStripsRows[i]["DecorAssignmentID"] != DBNull.Value && Convert.ToInt32(SawnStripsRows[i]["DecorAssignmentStatusID"]) == 3)
                {
                    int DecorAssignmentID = 0;
                    int StoreItemID = -1;
                    decimal Length2 = -1;
                    decimal Width2 = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    int Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (SawnStripsRows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(SawnStripsRows[i]["DecorAssignmentID"]);
                    if (SawnStripsRows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(SawnStripsRows[i]["TechStoreID2"]);
                    if (SawnStripsRows[i]["Length2"] != DBNull.Value)
                        Length2 = Convert.ToDecimal(SawnStripsRows[i]["Length2"]);
                    if (SawnStripsRows[i]["Width2"] != DBNull.Value)
                        Width2 = Convert.ToDecimal(SawnStripsRows[i]["Width2"]);
                    if (SawnStripsRows[i]["CoverID2"] != DBNull.Value)
                        CoverID = Convert.ToInt32(SawnStripsRows[i]["CoverID2"]);
                    if (SawnStripsRows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(SawnStripsRows[i]["FactCount"]);
                    if (SawnStripsRows[i]["Notes"] != DBNull.Value)
                        Notes = SawnStripsRows[i]["Notes"].ToString();

                    if (ProductType == 4)
                    {
                        if (!Convert.ToBoolean(SawnStripsRows[i]["SaveToStore"]))
                        {
                            int ManufactureStoreID = -1;
                            if (AssignmentsStoreManager.AddToManufactureStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length2, Width2, Height, Thickness,
                                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref ManufactureStoreID))
                                SawnStripsRows[i]["SaveToStore"] = true;
                            AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                        }
                        if (!Convert.ToBoolean(SawnStripsRows[i]["WriteOffFromStore"]))
                        {
                            if (SawnStripsRows[i]["TechStoreID1"] != DBNull.Value && SawnStripsRows[i]["Length1"] != DBNull.Value && SawnStripsRows[i]["Width1"] != DBNull.Value)
                            {
                                if (WriteOffMDF(WriteOffMovementInvoiceID, CreateDateTime, Convert.ToInt32(SawnStripsRows[i]["TechStoreID1"]), Convert.ToInt32(SawnStripsRows[i]["Length1"]), Convert.ToInt32(SawnStripsRows[i]["Width1"]), Convert.ToInt32(SawnStripsRows[i]["Count1"])))
                                    SawnStripsRows[i]["WriteOffFromStore"] = true;
                            }
                        }
                    }
                }
                if (ProductType == 4)
                {
                    TechStoreID1New = FindMdfID(TechStoreID2, ref Width1);
                }
                if (TechStoreID1New != 0 && TechStoreID1New != TechStoreID1Old)
                    SawnStripsRows[i]["TechStoreID1"] = TechStoreID1New;
            }
        }

        private void PreSaveKashir(int ArrivalMovementInvoiceID, int WriteOffMovementInvoiceID, DateTime CreateDateTime, ref DataRow[] ShroudedProfileRows)
        {

            for (int i = 0; i < ShroudedProfileRows.Count(); i++)
            {
                if (ShroudedProfileRows[i].RowState == DataRowState.Deleted || Convert.ToInt32(ShroudedProfileRows[i]["InPlan"]) == 0)
                    continue;
                int BarberanNumber = 0;
                int FrezerNumber = 0;
                int ProductType = 0;
                int TechStoreID1Old = 0;
                int TechStoreID1New = 0;
                int TechStoreID2 = 0;
                decimal Width1 = 0;
                if (ShroudedProfileRows[i]["ProductType"] != DBNull.Value)
                    ProductType = Convert.ToInt32(ShroudedProfileRows[i]["ProductType"]);
                if (ShroudedProfileRows[i]["BarberanNumber"] != DBNull.Value)
                    BarberanNumber = Convert.ToInt32(ShroudedProfileRows[i]["BarberanNumber"]);
                if (ShroudedProfileRows[i]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(ShroudedProfileRows[i]["FrezerNumber"]);
                if (ShroudedProfileRows[i]["ClientID"] == DBNull.Value)
                    ShroudedProfileRows[i]["ClientID"] = 0;
                if (ShroudedProfileRows[i]["MegaOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MegaOrderID"] = 0;
                if (ShroudedProfileRows[i]["MainOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MainOrderID"] = 0;
                if (ShroudedProfileRows[i]["DecorOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["DecorOrderID"] = 0;
                if (ShroudedProfileRows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1Old = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID1"]);
                if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                if (ShroudedProfileRows[i]["CreationUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["CreationDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["AddToPlanUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["AddToPlanDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value && ShroudedProfileRows[i]["DisprepancyCount"] != DBNull.Value && ShroudedProfileRows[i]["DefectCount"] != DBNull.Value)
                    ShroudedProfileRows[i]["DecorAssignmentStatusID"] = 3;
                if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value && Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentStatusID"]) == 3)
                {
                    int DecorAssignmentID = 0;
                    int StoreItemID = -1;
                    decimal Length2 = -1;
                    decimal Width2 = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    int Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentID"]);
                    if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                    if (ShroudedProfileRows[i]["Length2"] != DBNull.Value)
                        Length2 = Convert.ToDecimal(ShroudedProfileRows[i]["Length2"]);
                    if (ShroudedProfileRows[i]["Width2"] != DBNull.Value)
                        Width2 = Convert.ToDecimal(ShroudedProfileRows[i]["Width2"]);
                    if (ShroudedProfileRows[i]["CoverID2"] != DBNull.Value)
                        CoverID = Convert.ToInt32(ShroudedProfileRows[i]["CoverID2"]);
                    if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(ShroudedProfileRows[i]["FactCount"]);
                    if (ShroudedProfileRows[i]["Notes"] != DBNull.Value)
                        Notes = ShroudedProfileRows[i]["Notes"].ToString();

                    if (ProductType == 5)
                    {
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["SaveToStore"]))
                        {
                            int StoreID = -1;
                            if (AssignmentsStoreManager.AddToMainStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length2, Width2, Height, Thickness,
                                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref StoreID))
                                ShroudedProfileRows[i]["SaveToStore"] = true;
                            AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, StoreID, Count);
                        }
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["WriteOffFromStore"]))
                        {
                            if (ShroudedProfileRows[i]["NextLinkAssignmentID"] != DBNull.Value)
                            {
                                int NextLinkAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["NextLinkAssignmentID"]);
                                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + NextLinkAssignmentID);
                                if (rows.Count() > 0 && rows[0]["PlanCount"] != DBNull.Value)
                                {
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 1 || Convert.ToInt32(rows[0]["ProductType"]) == 2 || Convert.ToInt32(rows[0]["ProductType"]) == 3)
                                    {
                                        if (WriteOffProfile(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 4 || Convert.ToInt32(rows[0]["ProductType"]) == 5)
                                    {
                                        if (WriteOffMDF(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]), Convert.ToInt32(rows[0]["Width2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (ProductType == 5)
                {
                    TechStoreID1New = FindSawStripID(TechStoreID2, 1, ref Width1);
                }
                if (TechStoreID1New != 0 && TechStoreID1New != TechStoreID1Old)
                    ShroudedProfileRows[i]["TechStoreID1"] = TechStoreID1New;
            }
        }

        private void PreSaveMilledProfile(int ArrivalMovementInvoiceID, int WriteOffMovementInvoiceID, DateTime CreateDateTime, ref DataRow[] ShroudedProfileRows)
        {

            for (int i = 0; i < ShroudedProfileRows.Count(); i++)
            {
                if (ShroudedProfileRows[i].RowState == DataRowState.Deleted || Convert.ToInt32(ShroudedProfileRows[i]["InPlan"]) == 0)
                    continue;
                int BarberanNumber = 0;
                int FrezerNumber = 0;
                int ProductType = 0;
                int TechStoreID1Old = 0;
                int TechStoreID1New = 0;
                int TechStoreID2 = 0;
                decimal Width1 = 0;
                if (ShroudedProfileRows[i]["ProductType"] != DBNull.Value)
                    ProductType = Convert.ToInt32(ShroudedProfileRows[i]["ProductType"]);
                if (ShroudedProfileRows[i]["BarberanNumber"] != DBNull.Value)
                    BarberanNumber = Convert.ToInt32(ShroudedProfileRows[i]["BarberanNumber"]);
                if (ShroudedProfileRows[i]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(ShroudedProfileRows[i]["FrezerNumber"]);
                if (ShroudedProfileRows[i]["ClientID"] == DBNull.Value)
                    ShroudedProfileRows[i]["ClientID"] = 0;
                if (ShroudedProfileRows[i]["MegaOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MegaOrderID"] = 0;
                if (ShroudedProfileRows[i]["MainOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MainOrderID"] = 0;
                if (ShroudedProfileRows[i]["DecorOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["DecorOrderID"] = 0;
                if (ShroudedProfileRows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1Old = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID1"]);
                if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                if (ShroudedProfileRows[i]["CreationUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["CreationDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["AddToPlanUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["AddToPlanDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value && ShroudedProfileRows[i]["DisprepancyCount"] != DBNull.Value && ShroudedProfileRows[i]["DefectCount"] != DBNull.Value)
                    ShroudedProfileRows[i]["DecorAssignmentStatusID"] = 3;
                if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value && Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentStatusID"]) == 3)
                {
                    int DecorAssignmentID = 0;
                    int StoreItemID = -1;
                    decimal Length2 = -1;
                    decimal Width2 = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    int Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentID"]);
                    if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                    if (ShroudedProfileRows[i]["Length2"] != DBNull.Value)
                        Length2 = Convert.ToDecimal(ShroudedProfileRows[i]["Length2"]);
                    if (ShroudedProfileRows[i]["Width2"] != DBNull.Value)
                        Width2 = Convert.ToDecimal(ShroudedProfileRows[i]["Width2"]);
                    if (ShroudedProfileRows[i]["CoverID2"] != DBNull.Value)
                        CoverID = Convert.ToInt32(ShroudedProfileRows[i]["CoverID2"]);
                    if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(ShroudedProfileRows[i]["FactCount"]);
                    if (ShroudedProfileRows[i]["Notes"] != DBNull.Value)
                        Notes = ShroudedProfileRows[i]["Notes"].ToString();

                    if (ProductType == 2)
                    {
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["SaveToStore"]))
                        {
                            int ManufactureStoreID = -1;
                            if (AssignmentsStoreManager.AddToManufactureStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length2, Width2, Height, Thickness,
                                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref ManufactureStoreID))
                                ShroudedProfileRows[i]["SaveToStore"] = true;
                            AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                        }
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["WriteOffFromStore"]))
                        {
                            if (ShroudedProfileRows[i]["NextLinkAssignmentID"] != DBNull.Value)
                            {
                                int NextLinkAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["NextLinkAssignmentID"]);
                                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + NextLinkAssignmentID);
                                if (rows.Count() > 0 && rows[0]["PlanCount"] != DBNull.Value)
                                {
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 1 || Convert.ToInt32(rows[0]["ProductType"]) == 2 || Convert.ToInt32(rows[0]["ProductType"]) == 3)
                                    {
                                        if (WriteOffProfile(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 4 || Convert.ToInt32(rows[0]["ProductType"]) == 5)
                                    {
                                        if (WriteOffMDF(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]), Convert.ToDouble(rows[0]["Width2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (ProductType == 2)
                {
                    TechStoreID1New = FindSawStripID(TechStoreID2, FrezerNumber, ref Width1);
                    if (TechStoreID1New == 0)
                    {
                        TechStoreID1New = FindShroudedAssembledProfileID(TechStoreID2, FrezerNumber);
                    }
                }
                if (TechStoreID1New != 0 && TechStoreID1New != TechStoreID1Old)
                    ShroudedProfileRows[i]["TechStoreID1"] = TechStoreID1New;
            }
        }

        private void PreSaveAssembledProfile(int ArrivalMovementInvoiceID, int WriteOffMovementInvoiceID, DateTime CreateDateTime, ref DataRow[] ShroudedProfileRows)
        {

            for (int i = 0; i < ShroudedProfileRows.Count(); i++)
            {
                if (ShroudedProfileRows[i].RowState == DataRowState.Deleted || Convert.ToInt32(ShroudedProfileRows[i]["InPlan"]) == 0)
                    continue;
                int BarberanNumber = 0;
                int FrezerNumber = 0;
                int ProductType = 0;
                int TechStoreID1Old = 0;
                int TechStoreID1New = 0;
                int TechStoreID2 = 0;
                //decimal Width1 = 0;
                if (ShroudedProfileRows[i]["ProductType"] != DBNull.Value)
                    ProductType = Convert.ToInt32(ShroudedProfileRows[i]["ProductType"]);
                if (ShroudedProfileRows[i]["BarberanNumber"] != DBNull.Value)
                    BarberanNumber = Convert.ToInt32(ShroudedProfileRows[i]["BarberanNumber"]);
                if (ShroudedProfileRows[i]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(ShroudedProfileRows[i]["FrezerNumber"]);
                if (ShroudedProfileRows[i]["ClientID"] == DBNull.Value)
                    ShroudedProfileRows[i]["ClientID"] = 0;
                if (ShroudedProfileRows[i]["MegaOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MegaOrderID"] = 0;
                if (ShroudedProfileRows[i]["MainOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MainOrderID"] = 0;
                if (ShroudedProfileRows[i]["DecorOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["DecorOrderID"] = 0;
                if (ShroudedProfileRows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1Old = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID1"]);
                if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                if (ShroudedProfileRows[i]["CreationUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["CreationDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["AddToPlanUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["AddToPlanDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value && ShroudedProfileRows[i]["DisprepancyCount"] != DBNull.Value && ShroudedProfileRows[i]["DefectCount"] != DBNull.Value)
                    ShroudedProfileRows[i]["DecorAssignmentStatusID"] = 3;
                if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value && Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentStatusID"]) == 3)
                {
                    int DecorAssignmentID = 0;
                    int StoreItemID = -1;
                    decimal Length2 = -1;
                    decimal Width2 = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    int Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentID"]);
                    if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                    if (ShroudedProfileRows[i]["Length2"] != DBNull.Value)
                        Length2 = Convert.ToDecimal(ShroudedProfileRows[i]["Length2"]);
                    if (ShroudedProfileRows[i]["Width2"] != DBNull.Value)
                        Width2 = Convert.ToDecimal(ShroudedProfileRows[i]["Width2"]);
                    if (ShroudedProfileRows[i]["CoverID2"] != DBNull.Value)
                        CoverID = Convert.ToInt32(ShroudedProfileRows[i]["CoverID2"]);
                    if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(ShroudedProfileRows[i]["FactCount"]);
                    if (ShroudedProfileRows[i]["Notes"] != DBNull.Value)
                        Notes = ShroudedProfileRows[i]["Notes"].ToString();

                    if (ProductType == 3)
                    {
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["SaveToStore"]))
                        {
                            int ManufactureStoreID = -1;
                            if (AssignmentsStoreManager.AddToManufactureStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length2, Width2, Height, Thickness,
                                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref ManufactureStoreID))
                                ShroudedProfileRows[i]["SaveToStore"] = true;
                            AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                        }
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["WriteOffFromStore"]))
                        {
                            if (ShroudedProfileRows[i]["NextLinkAssignmentID"] != DBNull.Value)
                            {
                                int NextLinkAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["NextLinkAssignmentID"]);
                                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + NextLinkAssignmentID);
                                if (rows.Count() > 0 && rows[0]["PlanCount"] != DBNull.Value)
                                {
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 1 || Convert.ToInt32(rows[0]["ProductType"]) == 2 || Convert.ToInt32(rows[0]["ProductType"]) == 3)
                                    {
                                        if (WriteOffProfile(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 4 || Convert.ToInt32(rows[0]["ProductType"]) == 5)
                                    {
                                        if (WriteOffMDF(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]), Convert.ToInt32(rows[0]["Width2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (ProductType == 3)
                {
                    ArrayList TechStoreIDs = new ArrayList();
                    bool b = FindShroudedProfileID(TechStoreID2, 1, TechStoreIDs);
                    if (!b)
                    {
                        FindMilledProfileID(TechStoreID2, 1, TechStoreIDs);
                    }
                }
                if (TechStoreID1New != 0 && TechStoreID1New != TechStoreID1Old)
                    ShroudedProfileRows[i]["TechStoreID1"] = TechStoreID1New;
            }
        }

        private void PreSaveShroudedProfile(int ArrivalMovementInvoiceID, int WriteOffMovementInvoiceID, DateTime CreateDateTime, ref DataRow[] ShroudedProfileRows)
        {
            for (int i = 0; i < ShroudedProfileRows.Count(); i++)
            {
                if (ShroudedProfileRows[i].RowState == DataRowState.Deleted || Convert.ToInt32(ShroudedProfileRows[i]["InPlan"]) == 0)
                    continue;
                int BarberanNumber = 0;
                int FrezerNumber = 0;
                int ProductType = 0;
                int TechStoreID1Old = 0;
                int TechStoreID1New = 0;
                int TechStoreID2 = 0;
                decimal Width1 = 0;
                if (ShroudedProfileRows[i]["ProductType"] != DBNull.Value)
                    ProductType = Convert.ToInt32(ShroudedProfileRows[i]["ProductType"]);
                if (ShroudedProfileRows[i]["BarberanNumber"] != DBNull.Value)
                    BarberanNumber = Convert.ToInt32(ShroudedProfileRows[i]["BarberanNumber"]);
                if (ShroudedProfileRows[i]["FrezerNumber"] != DBNull.Value)
                    FrezerNumber = Convert.ToInt32(ShroudedProfileRows[i]["FrezerNumber"]);
                if (ShroudedProfileRows[i]["ClientID"] == DBNull.Value)
                    ShroudedProfileRows[i]["ClientID"] = 0;
                if (ShroudedProfileRows[i]["MegaOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MegaOrderID"] = 0;
                if (ShroudedProfileRows[i]["MainOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["MainOrderID"] = 0;
                if (ShroudedProfileRows[i]["DecorOrderID"] == DBNull.Value)
                    ShroudedProfileRows[i]["DecorOrderID"] = 0;
                if (ShroudedProfileRows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1Old = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID1"]);
                if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                if (ShroudedProfileRows[i]["CreationUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["CreationDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["CreationDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["AddToPlanUserID"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanUserID"] = Security.CurrentUserID;
                if (ShroudedProfileRows[i]["AddToPlanDateTime"] == DBNull.Value)
                    ShroudedProfileRows[i]["AddToPlanDateTime"] = CreateDateTime;
                if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value && ShroudedProfileRows[i]["DisprepancyCount"] != DBNull.Value && ShroudedProfileRows[i]["DefectCount"] != DBNull.Value)
                    ShroudedProfileRows[i]["DecorAssignmentStatusID"] = 3;
                if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value && Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentStatusID"]) == 3)
                {
                    int DecorAssignmentID = 0;
                    int StoreItemID = -1;
                    decimal Length2 = -1;
                    decimal Width2 = -1;
                    decimal Height = -1;
                    decimal Thickness = -1;
                    decimal Diameter = -1;
                    decimal Admission = -1;
                    decimal Capacity = -1;
                    decimal Weight = -1;
                    int ColorID = -1;
                    int PatinaID = -1;
                    int CoverID = -1;
                    int Count = 0;
                    int FactoryID = 1;
                    string Notes = string.Empty;

                    if (ShroudedProfileRows[i]["DecorAssignmentID"] != DBNull.Value)
                        DecorAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["DecorAssignmentID"]);
                    if (ShroudedProfileRows[i]["TechStoreID2"] != DBNull.Value)
                        StoreItemID = Convert.ToInt32(ShroudedProfileRows[i]["TechStoreID2"]);
                    if (ShroudedProfileRows[i]["Length2"] != DBNull.Value)
                        Length2 = Convert.ToDecimal(ShroudedProfileRows[i]["Length2"]);
                    if (ShroudedProfileRows[i]["Width2"] != DBNull.Value)
                        Width2 = Convert.ToDecimal(ShroudedProfileRows[i]["Width2"]);
                    if (ShroudedProfileRows[i]["CoverID2"] != DBNull.Value)
                        CoverID = Convert.ToInt32(ShroudedProfileRows[i]["CoverID2"]);
                    if (ShroudedProfileRows[i]["FactCount"] != DBNull.Value)
                        Count = Convert.ToInt32(ShroudedProfileRows[i]["FactCount"]);
                    if (ShroudedProfileRows[i]["Notes"] != DBNull.Value)
                        Notes = ShroudedProfileRows[i]["Notes"].ToString();

                    if (ProductType == 1)
                    {
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["SaveToStore"]))
                        {
                            int StoreID = -1;
                            if (AssignmentsStoreManager.AddToMainStore(CreateDateTime, ArrivalMovementInvoiceID, DecorAssignmentID, StoreItemID, Length2, Width2, Height, Thickness,
                                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes, ref StoreID))
                                ShroudedProfileRows[i]["SaveToStore"] = true;
                            AssignmentsStoreManager.AddMovementInvoiceDetail(ArrivalMovementInvoiceID, CreateDateTime, StoreID, Count);
                        }
                        if (!Convert.ToBoolean(ShroudedProfileRows[i]["WriteOffFromStore"]))
                        {
                            if (ShroudedProfileRows[i]["NextLinkAssignmentID"] != DBNull.Value)
                            {
                                int NextLinkAssignmentID = Convert.ToInt32(ShroudedProfileRows[i]["NextLinkAssignmentID"]);
                                DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + NextLinkAssignmentID);
                                if (rows.Count() > 0 && rows[0]["PlanCount"] != DBNull.Value)
                                {
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 1 || Convert.ToInt32(rows[0]["ProductType"]) == 2 || Convert.ToInt32(rows[0]["ProductType"]) == 3)
                                    {
                                        if (WriteOffProfile(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                    if (Convert.ToInt32(rows[0]["ProductType"]) == 4 || Convert.ToInt32(rows[0]["ProductType"]) == 5)
                                    {
                                        if (WriteOffMDF(WriteOffMovementInvoiceID, CreateDateTime,
                                            Convert.ToInt32(rows[0]["TechStoreID2"]), Convert.ToInt32(rows[0]["Length2"]), Convert.ToInt32(rows[0]["Width2"]),
                                            Convert.ToInt32(rows[0]["PlanCount"])))
                                            rows[0]["WriteOffFromStore"] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (ProductType == 1)
                {
                    TechStoreID1New = FindMilledProfileID(TechStoreID2, BarberanNumber);
                    if (TechStoreID1New == 0)
                    {
                        TechStoreID1New = FindShroudedAssembledProfileID(TechStoreID2, BarberanNumber);
                        if (TechStoreID1New == 0)
                        {
                            TechStoreID1New = FindMilledAssembledProfileID(TechStoreID2, BarberanNumber);
                            if (TechStoreID1New == 0)
                            {
                                TechStoreID1New = FindSawStripID(TechStoreID2, BarberanNumber, ref Width1);
                            }
                        }
                    }

                }
                if (TechStoreID1New != 0 && TechStoreID1New != TechStoreID1Old)
                    ShroudedProfileRows[i]["TechStoreID1"] = TechStoreID1New;
            }
        }

        public void SaveComplexSawing(int DecorAssignmentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DecorAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        for (int i = 0; i < ComplexSawingDT.Rows.Count; i++)
                        {
                            int TechStoreID2 = 0;
                            int Length2 = 0;
                            int Width2 = 0;
                            int PlanCount = 0;

                            if (ComplexSawingDT.Rows[i]["TechStoreID2"] != DBNull.Value)
                                TechStoreID2 = Convert.ToInt32(ComplexSawingDT.Rows[i]["TechStoreID2"]);
                            if (ComplexSawingDT.Rows[i]["Length2"] != DBNull.Value)
                                Length2 = Convert.ToInt32(ComplexSawingDT.Rows[i]["Length2"]);
                            if (ComplexSawingDT.Rows[i]["Width2"] != DBNull.Value)
                                Width2 = Convert.ToInt32(ComplexSawingDT.Rows[i]["Width2"]);
                            if (ComplexSawingDT.Rows[i]["PlanCount"] != DBNull.Value)
                                PlanCount = Convert.ToInt32(ComplexSawingDT.Rows[i]["PlanCount"]);

                            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
                            if (rows.Count() == 0)
                                continue;
                            ComplexSawingDT.Rows[i].ItemArray = rows[0].ItemArray;

                            //if (TechStoreID2 != 0)
                            ComplexSawingDT.Rows[i]["TechStoreID2"] = TechStoreID2;
                            //if (Length2 != 0)
                            ComplexSawingDT.Rows[i]["Length2"] = Length2;
                            //if (Width2 != 0)
                            ComplexSawingDT.Rows[i]["Width2"] = Width2;
                            //if (PlanCount != 0)
                            ComplexSawingDT.Rows[i]["PlanCount"] = PlanCount;
                            ComplexSawingDT.Rows[i]["InPlan"] = true;
                            ComplexSawingDT.Rows[i]["PrevLinkAssignmentID"] = DecorAssignmentID;
                            ComplexSawingDT.Rows[i]["ProductType"] = 4;
                        }
                        DA.Update(ComplexSawingDT);
                    }
                }
            }
        }

        public void SaveTransferredRollers()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ReturnedRollers",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TransferredRollersDT);
                }
            }
        }

        public void SaveAssignments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DecorAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    AssignmentsBeforeUpdateDT.Clear();
                    AssignmentsBeforeUpdateDT = AssignmentsDT.Copy();
                    DA.Update(AssignmentsDT);
                }
            }
        }

        public void SaveFacingRollersAssignments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DecorAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(FacingRollersDT1);
                    DA.Update(FacingRollersDT2);
                }
            }
        }

        public void SaveFacingMaterialAssignments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DecorAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(FacingMaterialDT);
                }
            }
        }

        public void SetComplexSawing(int DecorAssignmentID)
        {
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID=" + DecorAssignmentID);
            if (rows.Count() == 0)
                return;

            rows[0]["ComplexSawing"] = true;
        }

        public void PrintAssignmnents(int[] DecorAssignmentID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = AssignmentsDT.Select("DecorAssignmentID IN (" + string.Join(",", DecorAssignmentID) + ")");
            foreach (DataRow item in rows)
            {
                if (item["PrintUserID"] == DBNull.Value)
                    item["PrintUserID"] = Security.CurrentUserID;
                if (item["PrintDateTime"] == DBNull.Value)
                    item["PrintDateTime"] = CurrentDate;
            }
        }

        public bool FindShroudedProfileID(int TechStoreID, int GroupNumber, ArrayList TechStoreIDs)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.ShroudedProfile))
                    {
                        TechStoreIDs.Add(TechStoreID1);
                    }
                    FindShroudedProfileID(TechStoreID1, GroupNumber, TechStoreIDs);
                }
            }
            return TechStoreIDs.Count > 0;
        }

        public bool FindMilledProfileID(int TechStoreID, int GroupNumber, ArrayList TechStoreIDs)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.MilledProfile))
                    {
                        TechStoreIDs.Add(TechStoreID1);
                    }
                    FindMilledProfileID(TechStoreID1, GroupNumber, TechStoreIDs);
                }
            }
            return TechStoreIDs.Count > 0;
        }

        public int FindMilledProfileID(int TechStoreID, int GroupNumber)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.MilledProfile))
                    {
                        return TechStoreID1;
                    }
                    FindMilledProfileID(TechStoreID1, GroupNumber);
                }
            }
            return 0;
        }

        public int FindMilledAssembledProfileID(int TechStoreID, int GroupNumber)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.MilledAssembledProfile))
                    {
                        return TechStoreID1;
                    }
                    FindMilledAssembledProfileID(TechStoreID1, GroupNumber);
                }
            }
            return 0;
        }

        public int FindShroudedAssembledProfileID(int TechStoreID, int GroupNumber)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.ShroudedAssembledProfile))
                    {
                        return TechStoreID1;
                    }
                    FindShroudedAssembledProfileID(TechStoreID1, GroupNumber);
                }
            }
            return 0;
        }

        public int FindSawStripID(int TechStoreID, int GroupNumber, ref decimal Width1)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    //if (TechStoreID == TechStoreID1)
                    //{
                    //    DialogResult dialogResult = MessageBox.Show("Материал ссылается сам на себя. Нужно поправить тех. каталог:\r\n" +
                    //        GetTechStoreName(TechStoreID), "Зацикливание операции", MessageBoxButtons.OK);
                    //    return -1;
                    //}

                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.SawStrips) ||
                        TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.SawDSTP))
                    {
                        if (DT2.Rows[j]["Width"] != DBNull.Value)
                            Width1 = Convert.ToDecimal(DT2.Rows[j]["Width"]);
                        return TechStoreID1;
                    }
                    FindSawStripID(TechStoreID1, GroupNumber, ref Width1);
                }
            }
            return 0;
        }

        public int FindSawStripID(int TechStoreID, int GroupNumber, int SawNumber, ref decimal Width1)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID + " AND GroupNumber=" + GroupNumber, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.SawStrips) ||
                        TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.SawDSTP))
                    {
                        if (SawNumber == 1)
                        {
                            if (DT2.Rows[j]["Width"] != DBNull.Value)
                                Width1 = Convert.ToDecimal(DT2.Rows[j]["Width"]);
                        }
                        if (SawNumber == 2)
                        {
                            if (DT2.Rows[j]["Width1"] != DBNull.Value)
                                Width1 = Convert.ToDecimal(DT2.Rows[j]["Width1"]);
                        }
                        return TechStoreID1;
                    }
                    FindSawStripID(TechStoreID1, GroupNumber, SawNumber, ref Width1);
                }
            }
            return 0;
        }

        public int FindMdfID(int TechStoreID, ref decimal Width1)
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetailDT, "TechStoreID=" + TechStoreID, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable();
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsDetailID"]);
                using (DataView DV = new DataView(TechCatalogStoreDetailDT, "TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2 = DV.ToTable();
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechStoreID1 = Convert.ToInt32(DT2.Rows[j]["TechStoreID"]);
                    int TechStoreSubGroupID = Convert.ToInt32(DT2.Rows[j]["TechStoreSubGroupID"]);
                    if (TechStoreSubGroupID == Convert.ToInt32(TechStoreSubGroups.MdfPlate))
                    {
                        if (DT2.Rows[j]["Width"] != DBNull.Value)
                            Width1 = Convert.ToDecimal(DT2.Rows[j]["Width"]);
                        return TechStoreID1;
                    }
                    FindMdfID(TechStoreID1, ref Width1);
                }
            }
            return 0;
        }

        public void CreateBatchAssignment()
        {
            DataRow NewRow = BatchAssignmentsDT.NewRow();
            NewRow["CreateDate"] = Security.GetCurrentDate();
            NewRow["CreateUserID"] = Security.CurrentUserID;
            BatchAssignmentsDT.Rows.Add(NewRow);
        }

        public void ConfirmTools(int BatchAssignmentID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = BatchAssignmentsDT.Select("BatchAssignmentID = " + BatchAssignmentID);
            if (rows.Count() > 0)
            {
                if (rows[0]["ToolsConfirmUserID"] == DBNull.Value)
                    rows[0]["ToolsConfirmUserID"] = Security.CurrentUserID;
                if (rows[0]["ToolsConfirmDate"] == DBNull.Value)
                    rows[0]["ToolsConfirmDate"] = CurrentDate;
            }
        }

        public void ConfirmTechnology(int BatchAssignmentID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = BatchAssignmentsDT.Select("BatchAssignmentID = " + BatchAssignmentID);
            if (rows.Count() > 0)
            {
                if (rows[0]["TechnologyConfirmUserID"] == DBNull.Value)
                    rows[0]["TechnologyConfirmUserID"] = Security.CurrentUserID;
                if (rows[0]["TechnologyConfirmDate"] == DBNull.Value)
                    rows[0]["TechnologyConfirmDate"] = CurrentDate;
            }
        }

        public void ConfirmMaterial(int BatchAssignmentID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = BatchAssignmentsDT.Select("BatchAssignmentID = " + BatchAssignmentID);
            if (rows.Count() > 0)
            {
                if (rows[0]["MaterialConfirmUserID"] == DBNull.Value)
                    rows[0]["MaterialConfirmUserID"] = Security.CurrentUserID;
                if (rows[0]["MaterialConfirmDate"] == DBNull.Value)
                    rows[0]["MaterialConfirmDate"] = CurrentDate;
            }
        }

        public void ConfirmTechnical(int BatchAssignmentID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = BatchAssignmentsDT.Select("BatchAssignmentID = " + BatchAssignmentID);
            if (rows.Count() > 0)
            {
                if (rows[0]["TechnicalConfirmUserID"] == DBNull.Value)
                    rows[0]["TechnicalConfirmUserID"] = Security.CurrentUserID;
                if (rows[0]["TechnicalConfirmDate"] == DBNull.Value)
                    rows[0]["TechnicalConfirmDate"] = CurrentDate;
            }
        }

        public void SetBatchEnable(int BatchAssignmentID, bool Enabled)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = BatchAssignmentsDT.Select("BatchAssignmentID = " + BatchAssignmentID);
            if (rows.Count() > 0)
            {
                rows[0]["BatchEnable"] = Enabled;
                if (!Enabled)
                {
                    if (rows[0]["CloseUserID"] == DBNull.Value)
                        rows[0]["CloseUserID"] = Security.CurrentUserID;
                    if (rows[0]["CloseDate"] == DBNull.Value)
                        rows[0]["CloseDate"] = CurrentDate;
                }
                else
                {
                    rows[0]["CloseUserID"] = DBNull.Value;
                    rows[0]["CloseDate"] = DBNull.Value;
                }
            }
        }

        public void BatchAssigmentToProduction(int BatchAssignmentID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow[] rows = BatchAssignmentsDT.Select("BatchAssignmentID = " + BatchAssignmentID);
            if (rows.Count() > 0)
            {
                if (rows[0]["ProductionUserID"] == DBNull.Value)
                    rows[0]["ProductionUserID"] = Security.CurrentUserID;
                if (rows[0]["ProductionDate"] == DBNull.Value)
                    rows[0]["ProductionDate"] = CurrentDate;
            }
        }

        public void PrintBatchAssignments(int BatchAssignmentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DecorAssignments WHERE BatchAssignmentID=" + BatchAssignmentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DateTime CurrentDate = Security.GetCurrentDate();
                        foreach (DataRow item in DT.Rows)
                        {
                            if (Convert.ToInt32(item["DecorAssignmentStatusID"]) != 3)
                                item["DecorAssignmentStatusID"] = 2;
                            if (item["PrintUserID"] == DBNull.Value)
                                item["PrintUserID"] = Security.CurrentUserID;
                            if (item["PrintDateTime"] == DBNull.Value)
                                item["PrintDateTime"] = CurrentDate;
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveBatchAssignments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM BatchAssignments",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(BatchAssignmentsDT);
                }
            }
        }

        public void UpdateBatchAssignments()
        {
            string SelectCommand = @"SELECT * FROM BatchAssignments";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                BatchAssignmentsDT.Clear();
                DA.Fill(BatchAssignmentsDT);
            }
        }

        public void MoveToFacingMaterialRequest(int DecorAssignmentID)
        {
            FacingMaterialRequestsBS.Position = FacingMaterialRequestsBS.Find("DecorAssignmentID", DecorAssignmentID);
        }

        public void MoveToFacingMaterialOnStorage(string TechStoreName)
        {
            FacingMaterialOnStorageBS.Position = FacingMaterialOnStorageBS.Find("TechStoreName", TechStoreName);
        }

        public void MoveToFacingRollersOnStorage(string TechStoreName)
        {
            FacingRollersOnStorageBS.Position = FacingRollersOnStorageBS.Find("TechStoreName", TechStoreName);
        }

        public void MoveToFacingRollersRequest(int DecorAssignmentID)
        {
            FacingRollersRequestsBS.Position = FacingRollersRequestsBS.Find("DecorAssignmentID", DecorAssignmentID);
        }

        public void MoveToBatchAssignmentID(int BatchAssignmentID)
        {
            BatchAssignmentsBS.Position = BatchAssignmentsBS.Find("BatchAssignmentID", BatchAssignmentID);
        }

        public void MoveToBatchAssignmentPos(int Pos)
        {
            BatchAssignmentsBS.Position = Pos;
        }

        public void MoveToFirstBatchAssignmentID()
        {
            BatchAssignmentsBS.MoveFirst();
        }

        public bool WriteOffFromMainStore(int StoreID, decimal Count)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Store WHERE StoreID=" + StoreID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        try
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                decimal CurrentCount = Convert.ToDecimal(DT.Rows[0]["CurrentCount"]);
                                if (CurrentCount >= Count)
                                    DT.Rows[0]["CurrentCount"] = CurrentCount - Count;
                                DA.Update(DT);
                                return true;
                            }
                            return false;
                        }
                        catch (ArgumentNullException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (DBConcurrencyException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (SystemException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция AddToManufactureStore)");
                            return false;
                        }
                    }
                }
            }
        }

        public bool WriteOffMDF(int MovementInvoiceID, DateTime CreateDateTime, int TechStoreID, int Length, double Width, decimal Count)
        {
            string withComma = Width.ToString();
            string withDot = Width.ToString(System.Globalization.CultureInfo.InvariantCulture);
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ManufactureStore WHERE StoreItemID=" + TechStoreID + " AND Length=" + Length + " AND Width=" + withDot,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        try
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                decimal InvoiceCount = Count;
                                decimal WriteOffCount = 0;
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    int ManufactureStoreID = Convert.ToInt32(DT.Rows[i]["ManufactureStoreID"]);
                                    decimal CurrentCount = Convert.ToDecimal(DT.Rows[i]["CurrentCount"]);

                                    if (CurrentCount - InvoiceCount < 0)
                                    {
                                        WriteOffCount = CurrentCount;
                                        DT.Rows[i]["CurrentCount"] = 0;
                                        InvoiceCount = InvoiceCount - CurrentCount;
                                    }
                                    else
                                    {
                                        WriteOffCount = InvoiceCount;
                                        DT.Rows[i]["CurrentCount"] = CurrentCount - InvoiceCount;
                                        InvoiceCount = InvoiceCount - CurrentCount;
                                    }
                                    AssignmentsStoreManager.AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, ManufactureStoreID, WriteOffCount);
                                    if (InvoiceCount <= 0)
                                        break;
                                }
                                DA.Update(DT);
                                return true;
                            }
                            return false;
                        }
                        catch (ArgumentNullException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (DBConcurrencyException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (SystemException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция WriteOffMDF)");
                            return false;
                        }
                    }
                }
            }
        }

        public bool WriteOffProfile(int MovementInvoiceID, DateTime CreateDateTime, int TechStoreID, int Length, decimal Count)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ManufactureStore WHERE StoreItemID=" + TechStoreID + " AND Length=" + Length,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        try
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                decimal InvoiceCount = Count;
                                decimal WriteOffCount = 0;
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    int ManufactureStoreID = Convert.ToInt32(DT.Rows[i]["ManufactureStoreID"]);
                                    decimal CurrentCount = Convert.ToDecimal(DT.Rows[i]["CurrentCount"]);

                                    if (CurrentCount - InvoiceCount < 0)
                                    {
                                        WriteOffCount = CurrentCount;
                                        DT.Rows[i]["CurrentCount"] = 0;
                                        InvoiceCount = InvoiceCount - CurrentCount;
                                    }
                                    else
                                    {
                                        WriteOffCount = InvoiceCount;
                                        DT.Rows[i]["CurrentCount"] = CurrentCount - InvoiceCount;
                                        InvoiceCount = InvoiceCount - CurrentCount;
                                    }
                                    if (WriteOffCount != 0)
                                        AssignmentsStoreManager.AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, ManufactureStoreID, WriteOffCount);
                                    if (InvoiceCount <= 0)
                                        break;
                                }
                                DA.Update(DT);
                                return true;
                            }
                            return false;
                        }
                        catch (ArgumentNullException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (DBConcurrencyException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (SystemException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция WriteOffMDF)");
                            return false;
                        }
                    }
                }
            }
        }

        public bool WriteOffFromStoreByStoreID(int MovementInvoiceID, DateTime CreateDateTime, int StoreID, decimal Count)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Store WHERE StoreID=" + StoreID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        try
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                decimal CurrentCount = Convert.ToDecimal(DT.Rows[0]["CurrentCount"]);
                                if (CurrentCount >= Count)
                                    DT.Rows[0]["CurrentCount"] = CurrentCount - Count;
                                AssignmentsStoreManager.AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, StoreID, Count);
                                DA.Update(DT);
                                return true;
                            }
                            return false;
                        }
                        catch (ArgumentNullException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (DBConcurrencyException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (SystemException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция WriteOffFromManufactureStoreByManufactureStoreID)");
                            return false;
                        }
                    }
                }
            }
        }

        public bool WriteOffFromManufactureStoreByManufactureStoreID(int MovementInvoiceID, DateTime CreateDateTime, int ManufactureStoreID, decimal Count)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ManufactureStore WHERE ManufactureStoreID=" + ManufactureStoreID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        try
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                decimal CurrentCount = Convert.ToDecimal(DT.Rows[0]["CurrentCount"]);
                                if (CurrentCount >= Count)
                                    DT.Rows[0]["CurrentCount"] = CurrentCount - Count;
                                AssignmentsStoreManager.AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                                DA.Update(DT);
                                return true;
                            }
                            return false;
                        }
                        catch (ArgumentNullException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (DBConcurrencyException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (SystemException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция WriteOffFromManufactureStoreByManufactureStoreID)");
                            return false;
                        }
                    }
                }
            }
        }

        public bool ReturnToManufactureStoreByManufactureStoreID(int MovementInvoiceID, DateTime CreateDateTime, int ManufactureStoreID, decimal Count)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ManufactureStore WHERE ManufactureStoreID=" + ManufactureStoreID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        try
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                decimal CurrentCount = Convert.ToDecimal(DT.Rows[0]["CurrentCount"]);
                                DT.Rows[0]["CurrentCount"] = CurrentCount + Count;
                                AssignmentsStoreManager.AddMovementInvoiceDetail(MovementInvoiceID, CreateDateTime, ManufactureStoreID, Count);
                                DA.Update(DT);
                                return true;
                            }
                            return false;
                        }
                        catch (ArgumentNullException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (DBConcurrencyException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (SystemException ex)
                        {
                            MessageBox.Show(ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция WriteOffFromManufactureStoreByReturnRollerID)");
                            return false;
                        }
                    }
                }
            }
        }

        public decimal GetCost(int StoreItemID, decimal Thickness,
            decimal Length, decimal Width, decimal Height,
            decimal Diameter, decimal Capacity, decimal Weight,
            decimal Price, decimal Count)
        {
            int MeasureID = 0;
            decimal Sum = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Thickness, Length, Width, Height," +
                " Diameter, Capacity, Weight, MeasureID FROM TechStore WHERE TechStoreID = " + StoreItemID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        MeasureID = Convert.ToInt32(DT.Rows[0]["MeasureID"]);

                        //м.кв.
                        if (MeasureID == 1)
                        {
                            ///ВСТАВИТЬ АЛГОРИТМ ДЛЯ "ОБЛИЦОВКА РОЛИКИ"

                            if (Length < 0 && DT.Rows[0]["Length"] != DBNull.Value)
                                Length = Convert.ToDecimal(DT.Rows[0]["Length"]);
                            if (Width < 0 && DT.Rows[0]["Width"] != DBNull.Value)
                                Width = Convert.ToDecimal(DT.Rows[0]["Width"]);
                            if (Height < 0 && DT.Rows[0]["Height"] != DBNull.Value)
                                Height = Convert.ToDecimal(DT.Rows[0]["Height"]);
                            if (Length > 0)
                            {
                                if (Width > 0)
                                { }
                                else
                                {
                                    if (Height > 0)
                                        Width = Height;
                                }
                            }
                            else
                            {
                                if (Height > 0)
                                    Length = Height;
                            }
                            Sum = Length * Width * Count * Price / 1000000;
                        }

                        //м.п.
                        if (MeasureID == 2)
                        {
                            Sum = Length * Count * Price / 1000;
                        }

                        //шт.
                        if (MeasureID == 3)
                        {
                            Sum = Count * Price;
                        }

                        //кг.
                        if (MeasureID == 4)
                        {
                            if (Weight < 0)
                            {
                                Weight = Convert.ToDecimal(DT.Rows[0]["Weight"]);
                            }
                            Sum = Weight * Count * Price;
                        }

                        //л.
                        if (MeasureID == 5)
                        {
                            if (Capacity < 0)
                            {
                                Capacity = Convert.ToDecimal(DT.Rows[0]["Capacity"]);
                            }
                            Sum = Capacity * Count * Price;
                        }

                        //м.куб.
                        if (MeasureID == 6)
                        {
                            if (Thickness < 0)
                                Sum = Length * Width * Height * Count * Price / 1000000000;
                            if (Height < 0)
                                Sum = Length * Width * Thickness * Count * Price / 1000000000;
                        }

                        //тыс.шт.
                        if (MeasureID == 7)
                        {
                            Sum = Count * Price;
                        }


                        Decimal.Round(Sum, 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            return Sum;
        }

        public DataTable GetPermissions(int UserID, string FormName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UserRoles WHERE UserID = " + UserID +
                " AND RoleID IN (SELECT RoleID FROM Roles WHERE ModuleID IN " +
                " (SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'))", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return (DataTable)DT;
                }
            }
        }
    }






    public class AssignmentsStoreManager
    {
        DataTable MainStoreDT;
        DataTable ManufactureStoreDT;
        DataTable ReadyStoreDT;
        DataTable MovementInvoicesDT;
        DataTable MovementInvoiceDetailsDT;

        SqlDataAdapter MainStoreDA;
        SqlCommandBuilder MainStoreCB;
        SqlDataAdapter ManufactureStoreDA;
        SqlCommandBuilder ManufactureStoreCB;
        SqlDataAdapter ReadyStoreDA;
        SqlCommandBuilder ReadyStoreCB;
        SqlDataAdapter MovementInvoicesDA;
        SqlCommandBuilder MovementInvoicesCB;
        SqlDataAdapter MovementInvoiceDetailsDA;
        SqlCommandBuilder MovementInvoiceDetailsCB;

        public AssignmentsStoreManager()
        {
            MainStoreDT = new DataTable();
            MainStoreDA = new SqlDataAdapter("SELECT TOP 0 * FROM Store",
                ConnectionStrings.StorageConnectionString);
            MainStoreCB = new SqlCommandBuilder(MainStoreDA);
            MainStoreDA.Fill(MainStoreDT);

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

        public void ClearMainStore()
        {
            MovementInvoiceDetailsDT.Clear();
            MainStoreDT.Clear();
        }

        public void ClearManufactureStore()
        {
            MovementInvoiceDetailsDT.Clear();
            ManufactureStoreDT.Clear();
        }

        public void ClearReadyStore()
        {
            MovementInvoiceDetailsDT.Clear();
            ReadyStoreDT.Clear();
        }

        public bool AddToMainStore(DateTime CreateDateTime, int MovementInvoiceID, int DecorAssignmentID, int StoreItemID, decimal Length, decimal Width, decimal Height, decimal Thickness,
            decimal Diameter, decimal Admission, decimal Capacity, decimal Weight,
            int ColorID, int PatinaID, int CoverID, decimal Count, int FactoryID, string Notes, ref int StoreID)
        {
            MainStoreCB.Dispose();
            MainStoreDA.Dispose();
            MainStoreDA = new SqlDataAdapter(@"SELECT * FROM Store WHERE DecorAssignmentID=" + DecorAssignmentID,
                ConnectionStrings.StorageConnectionString);
            MainStoreCB = new SqlCommandBuilder(MainStoreDA);
            try
            {
                if (MainStoreDA.Fill(MainStoreDT) == 0)
                {
                    DataRow NewRow = MainStoreDT.NewRow();
                    if (MainStoreDT.Columns.Contains("CreateDateTime"))
                        NewRow["CreateDateTime"] = CreateDateTime;
                    NewRow["ManufacturerID"] = 145;
                    NewRow["Produced"] = CreateDateTime;
                    NewRow["MovementInvoiceID"] = MovementInvoiceID;
                    NewRow["DecorAssignmentID"] = DecorAssignmentID;
                    NewRow["StoreItemID"] = StoreItemID;
                    if (Length > -1)
                        NewRow["Length"] = Length;
                    if (Width > -1)
                        NewRow["Width"] = Width;
                    if (Height > -1)
                        NewRow["Height"] = Height;
                    if (Thickness > -1)
                        NewRow["Thickness"] = Thickness;
                    if (Diameter > -1)
                        NewRow["Diameter"] = Diameter;
                    if (Admission > -1)
                        NewRow["Admission"] = Admission;
                    if (Capacity > -1)
                        NewRow["Capacity"] = Capacity;
                    if (Weight > -1)
                        NewRow["Weight"] = Weight;
                    if (ColorID > -1)
                        NewRow["ColorID"] = ColorID;
                    if (CoverID > -1)
                        NewRow["CoverID"] = CoverID;
                    if (PatinaID > -1)
                        NewRow["PatinaID"] = PatinaID;
                    NewRow["InvoiceCount"] = Count;
                    NewRow["CurrentCount"] = Count;
                    NewRow["FactoryID"] = FactoryID;
                    NewRow["PriceEUR"] = 1;
                    NewRow["Price"] = 1;
                    NewRow["CurrencyTypeID"] = 1;
                    NewRow["Cost"] = 1;
                    NewRow["VAT"] = 1;
                    NewRow["VATCost"] = 1;
                    if (Notes.Length > 0)
                        NewRow["Notes"] = Notes;
                    MainStoreDT.Rows.Add(NewRow);
                }
                else
                {
                    MainStoreDT.Rows[0]["StoreItemID"] = StoreItemID;
                    if (Length > -1)
                        MainStoreDT.Rows[0]["Length"] = Length;
                    if (Width > -1)
                        MainStoreDT.Rows[0]["Width"] = Width;
                    if (Height > -1)
                        MainStoreDT.Rows[0]["Height"] = Height;
                    if (Thickness > -1)
                        MainStoreDT.Rows[0]["Thickness"] = Thickness;
                    if (Diameter > -1)
                        MainStoreDT.Rows[0]["Diameter"] = Diameter;
                    if (Admission > -1)
                        MainStoreDT.Rows[0]["Admission"] = Admission;
                    if (Capacity > -1)
                        MainStoreDT.Rows[0]["Capacity"] = Capacity;
                    if (Weight > -1)
                        MainStoreDT.Rows[0]["Weight"] = Weight;
                    if (ColorID > -1)
                        MainStoreDT.Rows[0]["ColorID"] = ColorID;
                    if (CoverID > -1)
                        MainStoreDT.Rows[0]["CoverID"] = CoverID;
                    if (PatinaID > -1)
                        MainStoreDT.Rows[0]["PatinaID"] = PatinaID;
                    if (MainStoreDT.Rows[0]["InvoiceCount"] != DBNull.Value)
                        MainStoreDT.Rows[0]["InvoiceCount"] = Convert.ToDecimal(MainStoreDT.Rows[0]["InvoiceCount"]) + Count;
                    if (MainStoreDT.Rows[0]["CurrentCount"] != DBNull.Value)
                        MainStoreDT.Rows[0]["CurrentCount"] = Convert.ToDecimal(MainStoreDT.Rows[0]["CurrentCount"]) + Count;
                    MainStoreDT.Rows[0]["FactoryID"] = FactoryID;
                    if (Notes.Length > 0)
                        MainStoreDT.Rows[0]["Notes"] = Notes;
                }
                MainStoreDA.Update(MainStoreDT);
                MainStoreDT.Clear();
                MainStoreDA.Fill(MainStoreDT);
                if (MainStoreDT.Rows.Count > 0)
                    StoreID = Convert.ToInt32(MainStoreDT.Rows[MainStoreDT.Rows.Count - 1]["StoreID"]);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция AddToMainStore)");
                return false;
            }
        }

        public bool AddToManufactureStore(int DecorAssignmentID, int StoreItemID, decimal Length, decimal Width, decimal Height, decimal Thickness,
            decimal Diameter, decimal Admission, decimal Capacity, decimal Weight,
            int ColorID, int PatinaID, int CoverID, decimal Count, int FactoryID, string Notes, int ReturnedRollerID = 0)
        {
            ManufactureStoreDT.Clear();
            ManufactureStoreCB.Dispose();
            ManufactureStoreDA.Dispose();
            ManufactureStoreDA = new SqlDataAdapter(@"SELECT * FROM ManufactureStore WHERE DecorAssignmentID=" + DecorAssignmentID,
                ConnectionStrings.StorageConnectionString);
            ManufactureStoreCB = new SqlCommandBuilder(ManufactureStoreDA);
            try
            {
                if (ManufactureStoreDA.Fill(ManufactureStoreDT) == 0)
                {
                    DateTime CreateDateTime = Security.GetCurrentDate();
                    DataRow NewRow = ManufactureStoreDT.NewRow();
                    if (ManufactureStoreDT.Columns.Contains("CreateDateTime"))
                        NewRow["CreateDateTime"] = CreateDateTime;
                    NewRow["Produced"] = CreateDateTime;
                    NewRow["DecorAssignmentID"] = DecorAssignmentID;
                    NewRow["StoreItemID"] = StoreItemID;
                    NewRow["ReturnedRollerID"] = ReturnedRollerID;
                    if (Length > -1)
                        NewRow["Length"] = Length;
                    if (Width > -1)
                        NewRow["Width"] = Width;
                    if (Height > -1)
                        NewRow["Height"] = Height;
                    if (Thickness > -1)
                        NewRow["Thickness"] = Thickness;
                    if (Diameter > -1)
                        NewRow["Diameter"] = Diameter;
                    if (Admission > -1)
                        NewRow["Admission"] = Admission;
                    if (Capacity > -1)
                        NewRow["Capacity"] = Capacity;
                    if (Weight > -1)
                        NewRow["Weight"] = Weight;
                    if (ColorID > -1)
                        NewRow["ColorID"] = ColorID;
                    if (CoverID > -1)
                        NewRow["CoverID"] = CoverID;
                    if (PatinaID > -1)
                        NewRow["PatinaID"] = PatinaID;
                    NewRow["InvoiceCount"] = Count;
                    NewRow["CurrentCount"] = Count;
                    NewRow["FactoryID"] = FactoryID;
                    if (Notes.Length > 0)
                        NewRow["Notes"] = Notes;
                    ManufactureStoreDT.Rows.Add(NewRow);
                }
                else
                {
                    ManufactureStoreDT.Rows[0]["ReturnedRollerID"] = ReturnedRollerID;
                    ManufactureStoreDT.Rows[0]["StoreItemID"] = StoreItemID;
                    if (Length > -1)
                        ManufactureStoreDT.Rows[0]["Length"] = Length;
                    if (Width > -1)
                        ManufactureStoreDT.Rows[0]["Width"] = Width;
                    if (Height > -1)
                        ManufactureStoreDT.Rows[0]["Height"] = Height;
                    if (Thickness > -1)
                        ManufactureStoreDT.Rows[0]["Thickness"] = Thickness;
                    if (Diameter > -1)
                        ManufactureStoreDT.Rows[0]["Diameter"] = Diameter;
                    if (Admission > -1)
                        ManufactureStoreDT.Rows[0]["Admission"] = Admission;
                    if (Capacity > -1)
                        ManufactureStoreDT.Rows[0]["Capacity"] = Capacity;
                    if (Weight > -1)
                        ManufactureStoreDT.Rows[0]["Weight"] = Weight;
                    if (ColorID > -1)
                        ManufactureStoreDT.Rows[0]["ColorID"] = ColorID;
                    if (CoverID > -1)
                        ManufactureStoreDT.Rows[0]["CoverID"] = CoverID;
                    if (PatinaID > -1)
                        ManufactureStoreDT.Rows[0]["PatinaID"] = PatinaID;
                    if (ManufactureStoreDT.Rows[0]["InvoiceCount"] != DBNull.Value)
                        ManufactureStoreDT.Rows[0]["InvoiceCount"] = Count;
                    if (ManufactureStoreDT.Rows[0]["CurrentCount"] != DBNull.Value)
                        ManufactureStoreDT.Rows[0]["CurrentCount"] = Count;
                    ManufactureStoreDT.Rows[0]["FactoryID"] = FactoryID;
                    if (Notes.Length > 0)
                        ManufactureStoreDT.Rows[0]["Notes"] = Notes;
                }
                ManufactureStoreDA.Update(ManufactureStoreDT);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция AddToManufactureStore)");
                return false;
            }
        }

        public bool AddToManufactureStore(DateTime CreateDateTime, int MovementInvoiceID, int StoreItemID, decimal Length, decimal Width, decimal Height, decimal Thickness,
            decimal Diameter, decimal Admission, decimal Capacity, decimal Weight,
            int ColorID, int PatinaID, int CoverID, decimal Count, int FactoryID, string Notes, ref int ManufactureStoreID, int ReturnedRollerID = 0)
        {
            ManufactureStoreDT.Clear();
            ManufactureStoreCB.Dispose();
            ManufactureStoreDA.Dispose();
            ManufactureStoreDA = new SqlDataAdapter(@"SELECT TOP 1 * FROM ManufactureStore ORDER BY ManufactureStoreID DESC",
                ConnectionStrings.StorageConnectionString);
            ManufactureStoreCB = new SqlCommandBuilder(ManufactureStoreDA);
            try
            {
                DataRow NewRow = ManufactureStoreDT.NewRow();
                if (ManufactureStoreDT.Columns.Contains("CreateDateTime"))
                    NewRow["CreateDateTime"] = CreateDateTime;
                NewRow["Produced"] = CreateDateTime;
                NewRow["MovementInvoiceID"] = MovementInvoiceID;
                NewRow["StoreItemID"] = StoreItemID;
                NewRow["ReturnedRollerID"] = ReturnedRollerID;
                if (Length > -1)
                    NewRow["Length"] = Length;
                if (Width > -1)
                    NewRow["Width"] = Width;
                if (Height > -1)
                    NewRow["Height"] = Height;
                if (Thickness > -1)
                    NewRow["Thickness"] = Thickness;
                if (Diameter > -1)
                    NewRow["Diameter"] = Diameter;
                if (Admission > -1)
                    NewRow["Admission"] = Admission;
                if (Capacity > -1)
                    NewRow["Capacity"] = Capacity;
                if (Weight > -1)
                    NewRow["Weight"] = Weight;
                if (ColorID > -1)
                    NewRow["ColorID"] = ColorID;
                if (CoverID > -1)
                    NewRow["CoverID"] = CoverID;
                if (PatinaID > -1)
                    NewRow["PatinaID"] = PatinaID;
                NewRow["InvoiceCount"] = Count;
                NewRow["CurrentCount"] = Count;
                NewRow["FactoryID"] = FactoryID;
                if (Notes.Length > 0)
                    NewRow["Notes"] = Notes;
                ManufactureStoreDT.Rows.Add(NewRow);
                ManufactureStoreDA.Update(ManufactureStoreDT);
                ManufactureStoreDT.Clear();
                ManufactureStoreDA.Fill(ManufactureStoreDT);
                if (ManufactureStoreDT.Rows.Count > 0)
                    ManufactureStoreID = Convert.ToInt32(ManufactureStoreDT.Rows[ManufactureStoreDT.Rows.Count - 1]["ManufactureStoreID"]);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция AddToManufactureStore)");
                return false;
            }
        }

        public bool AddToManufactureStore(DateTime CreateDateTime, int MovementInvoiceID, int DecorAssignmentID, int StoreItemID, decimal Length, decimal Width, decimal Height, decimal Thickness,
            decimal Diameter, decimal Admission, decimal Capacity, decimal Weight,
            int ColorID, int PatinaID, int CoverID, decimal Count, int FactoryID, string Notes, ref int ManufactureStoreID, int ReturnedRollerID = 0)
        {
            ManufactureStoreDT.Clear();
            ManufactureStoreCB.Dispose();
            ManufactureStoreDA.Dispose();
            ManufactureStoreDA = new SqlDataAdapter(@"SELECT * FROM ManufactureStore WHERE DecorAssignmentID=" + DecorAssignmentID,
                ConnectionStrings.StorageConnectionString);
            ManufactureStoreCB = new SqlCommandBuilder(ManufactureStoreDA);
            try
            {
                if (ManufactureStoreDA.Fill(ManufactureStoreDT) == 0)
                {
                    DataRow NewRow = ManufactureStoreDT.NewRow();
                    if (ManufactureStoreDT.Columns.Contains("CreateDateTime"))
                        NewRow["CreateDateTime"] = CreateDateTime;
                    NewRow["Produced"] = CreateDateTime;
                    NewRow["MovementInvoiceID"] = MovementInvoiceID;
                    NewRow["DecorAssignmentID"] = DecorAssignmentID;
                    NewRow["StoreItemID"] = StoreItemID;
                    NewRow["ReturnedRollerID"] = ReturnedRollerID;
                    if (Length > -1)
                        NewRow["Length"] = Length;
                    if (Width > -1)
                        NewRow["Width"] = Width;
                    if (Height > -1)
                        NewRow["Height"] = Height;
                    if (Thickness > -1)
                        NewRow["Thickness"] = Thickness;
                    if (Diameter > -1)
                        NewRow["Diameter"] = Diameter;
                    if (Admission > -1)
                        NewRow["Admission"] = Admission;
                    if (Capacity > -1)
                        NewRow["Capacity"] = Capacity;
                    if (Weight > -1)
                        NewRow["Weight"] = Weight;
                    if (ColorID > -1)
                        NewRow["ColorID"] = ColorID;
                    if (CoverID > -1)
                        NewRow["CoverID"] = CoverID;
                    if (PatinaID > -1)
                        NewRow["PatinaID"] = PatinaID;
                    NewRow["InvoiceCount"] = Count;
                    NewRow["CurrentCount"] = Count;
                    NewRow["FactoryID"] = FactoryID;
                    if (Notes.Length > 0)
                        NewRow["Notes"] = Notes;
                    ManufactureStoreDT.Rows.Add(NewRow);
                }
                else
                {
                    ManufactureStoreDT.Rows[0]["ReturnedRollerID"] = ReturnedRollerID;
                    ManufactureStoreDT.Rows[0]["StoreItemID"] = StoreItemID;
                    if (Length > -1)
                        ManufactureStoreDT.Rows[0]["Length"] = Length;
                    if (Width > -1)
                        ManufactureStoreDT.Rows[0]["Width"] = Width;
                    if (Height > -1)
                        ManufactureStoreDT.Rows[0]["Height"] = Height;
                    if (Thickness > -1)
                        ManufactureStoreDT.Rows[0]["Thickness"] = Thickness;
                    if (Diameter > -1)
                        ManufactureStoreDT.Rows[0]["Diameter"] = Diameter;
                    if (Admission > -1)
                        ManufactureStoreDT.Rows[0]["Admission"] = Admission;
                    if (Capacity > -1)
                        ManufactureStoreDT.Rows[0]["Capacity"] = Capacity;
                    if (Weight > -1)
                        ManufactureStoreDT.Rows[0]["Weight"] = Weight;
                    if (ColorID > -1)
                        ManufactureStoreDT.Rows[0]["ColorID"] = ColorID;
                    if (CoverID > -1)
                        ManufactureStoreDT.Rows[0]["CoverID"] = CoverID;
                    if (PatinaID > -1)
                        ManufactureStoreDT.Rows[0]["PatinaID"] = PatinaID;
                    if (ManufactureStoreDT.Rows[0]["InvoiceCount"] != DBNull.Value)
                        ManufactureStoreDT.Rows[0]["InvoiceCount"] = Convert.ToDecimal(ManufactureStoreDT.Rows[0]["InvoiceCount"]) + Count;
                    if (ManufactureStoreDT.Rows[0]["CurrentCount"] != DBNull.Value)
                        ManufactureStoreDT.Rows[0]["CurrentCount"] = Convert.ToDecimal(ManufactureStoreDT.Rows[0]["CurrentCount"]) + Count;
                    ManufactureStoreDT.Rows[0]["FactoryID"] = FactoryID;
                    if (Notes.Length > 0)
                        ManufactureStoreDT.Rows[0]["Notes"] = Notes;
                }
                ManufactureStoreDA.Update(ManufactureStoreDT);
                ManufactureStoreDT.Clear();
                ManufactureStoreDA.Fill(ManufactureStoreDT);
                if (ManufactureStoreDT.Rows.Count > 0)
                    ManufactureStoreID = Convert.ToInt32(ManufactureStoreDT.Rows[ManufactureStoreDT.Rows.Count - 1]["ManufactureStoreID"]);
                return true;
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nНИЧОСИ! КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ! (функция AddToManufactureStore)");
                return false;
            }
        }

        public void SaveMovementInvoiceDetails()
        {
            MovementInvoiceDetailsDA.Update(MovementInvoiceDetailsDT);
            MovementInvoiceDetailsDT.Clear();
            MovementInvoiceDetailsDA.Fill(MovementInvoiceDetailsDT);
        }

        public void AddMovementInvoiceDetail(int MovementInvoiceID, DateTime CreateDateTime, int StoreIDFrom, decimal Count)
        {
            //DataRow[] Rows = MovementInvoiceDetailsDT.Select("StoreIDFrom = " + StoreIDFrom);
            //if (Rows.Count() > 0)
            //{
            //    Rows[0]["Count"] = Convert.ToDecimal(Rows[0]["Count"]) + Count;
            //}
            //else
            //{
            //    DataRow NewRow = MovementInvoiceDetailsDT.NewRow();
            //    if (MovementInvoiceDetailsDT.Columns.Contains("CreateDateTime"))
            //        NewRow["CreateDateTime"] = CreateDateTime;
            //    NewRow["MovementInvoiceID"] = MovementInvoiceID;
            //    NewRow["StoreIDFrom"] = StoreIDFrom;
            //    NewRow["StoreIDTo"] = 0;
            //    NewRow["Count"] = Count;
            //    MovementInvoiceDetailsDT.Rows.Add(NewRow);
            //}
            DataRow NewRow = MovementInvoiceDetailsDT.NewRow();
            if (MovementInvoiceDetailsDT.Columns.Contains("CreateDateTime"))
                NewRow["CreateDateTime"] = CreateDateTime;
            NewRow["MovementInvoiceID"] = MovementInvoiceID;
            NewRow["StoreIDFrom"] = StoreIDFrom;
            NewRow["StoreIDTo"] = 0;
            NewRow["Count"] = Count;
            MovementInvoiceDetailsDT.Rows.Add(NewRow);
        }

        public void FillMainStoreMovementInvoiceDetails(int MovementInvoiceID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Store" +
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
                            MovementInvoiceDetailsDT.Rows[i]["StoreIDTo"] = DT.Rows[j++]["StoreID"];
                        }
                        else
                        {
                            DataRow[] Rows = DT.Select("StoreID = " + StoreID);
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

        public void FillManufactureStoreMovementInvoiceDetails(int MovementInvoiceID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ManufactureStore" +
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
                            MovementInvoiceDetailsDT.Rows[i]["StoreIDTo"] = DT.Rows[j++]["ManufactureStoreID"];
                        }
                        else
                        {
                            DataRow[] Rows = DT.Select("ManufactureStoreID = " + StoreID);
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

        public int SaveMovementInvoices(DateTime CreateDateTime,
            int SellerStoreAllocID,
            int RecipientStoreAllocID, int RecipientSectorID,
            int PersonID, string PersonName, int StoreKeeperID,
            int ClientID, int SellerID,
            string ClientName, string Notes, int TypeCreation)
        {
            int LastMovementInvoiceID = 0;

            DataRow NewRow = MovementInvoicesDT.NewRow();

            NewRow["DateTime"] = CreateDateTime;
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
            NewRow["TypeCreation"] = TypeCreation;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = CreateDateTime;

            MovementInvoicesDT.Rows.Add(NewRow);

            MovementInvoicesDA.Update(MovementInvoicesDT);
            MovementInvoicesDT.Clear();
            MovementInvoicesDA.Fill(MovementInvoicesDT);
            if (MovementInvoicesDT.Rows.Count > 0)
                LastMovementInvoiceID = Convert.ToInt32(MovementInvoicesDT.Rows[MovementInvoicesDT.Rows.Count - 1]["MovementInvoiceID"]);

            return LastMovementInvoiceID;
        }

        public decimal AddToReadyStore(int MovementInvoiceID, DateTime CreateDateTime, int ManufactureStoreID, ref decimal Count)
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
                NewRow["CurrentCount"] = CurrentCount + Count;
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




    public class ProfileAssignmentsToExcel
    {
        object ToolsConfirmDate = DBNull.Value;
        object TechnologyConfirmDate = DBNull.Value;
        object MaterialConfirmDate = DBNull.Value;
        object TechnicalConfirmDate = DBNull.Value;
        object PrintDateTime = DBNull.Value;
        object ToolsConfirmUserID = DBNull.Value;
        object TechnologyConfirmUserID = DBNull.Value;
        object MaterialConfirmUserID = DBNull.Value;
        object TechnicalConfirmUserID = DBNull.Value;
        object PrintUserID = DBNull.Value;

        HSSFWorkbook hssfworkbook;

        HSSFFont Calibri10F;
        HSSFFont CalibriBold10F;
        HSSFFont Calibri11F;
        HSSFFont CalibriBold11F;
        HSSFCellStyle Calibri11CS;
        HSSFCellStyle CalibriBold11CS;
        HSSFCellStyle TableNameCS;
        HSSFCellStyle TableHeaderCS;
        HSSFCellStyle TableContentCS;
        HSSFCellStyle TableDecContentCS;

        DateTime CurrentDate;

        DataTable CuttingAssignmentsDT;
        DataTable MillingAssignmentsDT;
        DataTable EnvelopingAssignmentsDT;
        DataTable KashirAssignmentsDT;
        //DataTable PackingAssignmentsDT;

        DataTable Barberan1DT;
        DataTable Barberan2DT;
        DataTable Frezer1DT;
        DataTable Frezer2DT;
        DataTable Frezer3DT;
        DataTable HolzmaDT;
        DataTable KashirDT;
        DataTable PackingDT;

        DataTable CoversDT;
        DataTable TechStoreDT;

        public ProfileAssignmentsToExcel()
        {

        }

        public void GetCurrentDate()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    CurrentDate = Convert.ToDateTime(DT.Rows[0][0]);
                }
            }
        }

        public void Initialize()
        {
            Create();
            Fill();
        }

        private void Create()
        {
            hssfworkbook = new HSSFWorkbook();

            CoversDT = new DataTable();
            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));

            Barberan1DT = new DataTable();
            Barberan2DT = new DataTable();
            Frezer1DT = new DataTable();
            Frezer2DT = new DataTable();
            Frezer3DT = new DataTable();
            HolzmaDT = new DataTable();
            KashirDT = new DataTable();
            PackingDT = new DataTable();

            CuttingAssignmentsDT = new DataTable();
            CuttingAssignmentsDT.Columns.Add(new DataColumn("PalleteNumber", Type.GetType("System.String")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Name1", Type.GetType("System.String")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Length1", Type.GetType("System.Int32")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Width1", Type.GetType("System.Decimal")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Count1", Type.GetType("System.Int32")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Name2", Type.GetType("System.String")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Width2", Type.GetType("System.Decimal")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Length2", Type.GetType("System.Int32")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("PlanCount", Type.GetType("System.Int32")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Norm", Type.GetType("System.Decimal")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("SawName", Type.GetType("System.String")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("FactCount", Type.GetType("System.Int32")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("DefectCount", Type.GetType("System.Int32")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("User", Type.GetType("System.String")));
            CuttingAssignmentsDT.Columns.Add(new DataColumn("DisprepancyCount", Type.GetType("System.Int32")));

            MillingAssignmentsDT = new DataTable();
            MillingAssignmentsDT.Columns.Add(new DataColumn("Name1", Type.GetType("System.String")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("Name2", Type.GetType("System.String")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("Width1", Type.GetType("System.Int32")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("PlanCount", Type.GetType("System.Int32")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("Length2", Type.GetType("System.Int32")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("Amount", Type.GetType("System.Decimal")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("Norm", Type.GetType("System.Decimal")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("FactCount", Type.GetType("System.Int32")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("DefectCount", Type.GetType("System.Int32")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("User", Type.GetType("System.String")));
            MillingAssignmentsDT.Columns.Add(new DataColumn("DisprepancyCount", Type.GetType("System.Int32")));

            EnvelopingAssignmentsDT = new DataTable();
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("Name2", Type.GetType("System.String")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("PlanCount", Type.GetType("System.Int32")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("Length2", Type.GetType("System.Int32")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("Amount", Type.GetType("System.Decimal")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("Norm", Type.GetType("System.Decimal")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("FactCount", Type.GetType("System.Int32")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("DefectCount", Type.GetType("System.Int32")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("User", Type.GetType("System.String")));
            EnvelopingAssignmentsDT.Columns.Add(new DataColumn("DisprepancyCount", Type.GetType("System.Int32")));

            KashirAssignmentsDT = new DataTable();
            KashirAssignmentsDT.Columns.Add(new DataColumn("Name1", Type.GetType("System.String")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("Name2", Type.GetType("System.String")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("Length2", Type.GetType("System.Int32")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("Width2", Type.GetType("System.Int32")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("PlanCount", Type.GetType("System.Int32")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("Amount", Type.GetType("System.Decimal")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("Norm", Type.GetType("System.Decimal")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("FactCount", Type.GetType("System.Int32")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("DefectCount", Type.GetType("System.Int32")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("User", Type.GetType("System.String")));
            KashirAssignmentsDT.Columns.Add(new DataColumn("DisprepancyCount", Type.GetType("System.Int32")));

            //PackingAssignmentsDT = new DataTable();
            //PackingAssignmentsDT.Columns.Add(new DataColumn("Name2", Type.GetType("System.String")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("Length2", Type.GetType("System.Int32")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("PlanCount", Type.GetType("System.Int32")));

            //PackingAssignmentsDT.Columns.Add(new DataColumn("Amount", Type.GetType("System.Decimal")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("Norm", Type.GetType("System.Decimal")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("FactCount", Type.GetType("System.Int32")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("DefectCount", Type.GetType("System.Int32")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("User", Type.GetType("System.String")));
            //PackingAssignmentsDT.Columns.Add(new DataColumn("DisprepancyCount", Type.GetType("System.Int32")));

            TechStoreDT = new DataTable();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore 
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11) 
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = CoversDT.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["CoverName"] = DT.Rows[i]["TechStoreName"].ToString();
                        CoversDT.Rows.Add(NewRow);
                    }
                }
            }

            SelectCommand = @"SELECT TechStoreID, TechStoreSubGroupID, TechStoreName FROM TechStore ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDT);
            }
        }

        private string GetCoverName(int ID)
        {
            DataRow[] rows = CoversDT.Select("CoverID=" + ID);
            if (rows.Count() > 0)
                return rows[0]["CoverName"].ToString();
            else
                return string.Empty;
        }

        private string GetTechStoreName(int ID)
        {
            DataRow[] rows = TechStoreDT.Select("TechStoreID=" + ID);
            if (rows.Count() > 0)
                return rows[0]["TechStoreName"].ToString();
            else
                return string.Empty;
        }

        public void GetDecorAssignments(int BatchAssignmentID)
        {
            string SelectCommand = @"SELECT * FROM DecorAssignments WHERE BarberanNumber=1 AND ProductType=1 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                Barberan1DT.Clear();
                DA.Fill(Barberan1DT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE BarberanNumber=2 AND ProductType=1 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                Barberan2DT.Clear();
                DA.Fill(Barberan2DT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE FrezerNumber=1 AND ProductType=2 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                Frezer1DT.Clear();
                DA.Fill(Frezer1DT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE FrezerNumber=2 AND ProductType=2 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                Frezer2DT.Clear();
                DA.Fill(Frezer2DT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE FrezerNumber=3 AND ProductType=2 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                Frezer3DT.Clear();
                DA.Fill(Frezer3DT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE ProductType=4 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                HolzmaDT.Clear();
                DA.Fill(HolzmaDT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE ProductType=5 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                KashirDT.Clear();
                DA.Fill(KashirDT);
            }
            SelectCommand = @"SELECT * FROM DecorAssignments WHERE ProductType=6 AND (BatchAssignmentID=" + BatchAssignmentID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                PackingDT.Clear();
                DA.Fill(PackingDT);
            }
        }

        private void FillCuttingAssignmentsDT(DataTable DT1)
        {
            CuttingAssignmentsDT.Clear();
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                string Name1 = string.Empty;
                string Name2 = string.Empty;
                string Notes = string.Empty;
                string SawName = string.Empty;
                int Length1 = 0;
                int Length2 = 0;
                decimal Width1 = 0;
                decimal Width2 = 0;
                int Count1 = 0;
                int PlanCount = 0;
                int FactCount = 0;
                int DisprepancyCount = 0;
                int DefectCount = 0;

                int TechStoreID1 = 0;
                int TechStoreID2 = 0;

                if (DT1.Rows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1 = Convert.ToInt32(DT1.Rows[i]["TechStoreID1"]);
                if (DT1.Rows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]);
                if (DT1.Rows[i]["Length1"] != DBNull.Value)
                    Length1 = Convert.ToInt32(DT1.Rows[i]["Length1"]);
                if (DT1.Rows[i]["Length2"] != DBNull.Value)
                    Length2 = Convert.ToInt32(DT1.Rows[i]["Length2"]);
                if (DT1.Rows[i]["Width1"] != DBNull.Value)
                    Width1 = Convert.ToDecimal(DT1.Rows[i]["Width1"]);
                if (DT1.Rows[i]["Width2"] != DBNull.Value)
                    Width2 = Convert.ToDecimal(DT1.Rows[i]["Width2"]);
                if (DT1.Rows[i]["Count1"] != DBNull.Value)
                    Count1 = Convert.ToInt32(DT1.Rows[i]["Count1"]);
                if (DT1.Rows[i]["PlanCount"] != DBNull.Value)
                    PlanCount = Convert.ToInt32(DT1.Rows[i]["PlanCount"]);
                if (DT1.Rows[i]["FactCount"] != DBNull.Value)
                    FactCount = Convert.ToInt32(DT1.Rows[i]["FactCount"]);
                if (DT1.Rows[i]["DisprepancyCount"] != DBNull.Value)
                    DisprepancyCount = Convert.ToInt32(DT1.Rows[i]["DisprepancyCount"]);
                if (DT1.Rows[i]["DefectCount"] != DBNull.Value)
                    DefectCount = Convert.ToInt32(DT1.Rows[i]["DefectCount"]);

                Name1 = GetTechStoreName(TechStoreID1);
                Name2 = GetTechStoreName(TechStoreID2);
                Notes = DT1.Rows[i]["Notes"].ToString();
                SawName = DT1.Rows[i]["SawName"].ToString();

                DataRow NewRow = CuttingAssignmentsDT.NewRow();
                NewRow["Name1"] = Name1;
                NewRow["Name2"] = Name2;
                if (Length1 != 0)
                    NewRow["Length1"] = Length1;
                if (Length2 != 0)
                    NewRow["Length2"] = Length2;
                if (Width1 != 0)
                    NewRow["Width1"] = Width1;
                if (Width2 != 0)
                    NewRow["Width2"] = Width2;
                NewRow["Count1"] = Count1;
                NewRow["PlanCount"] = PlanCount;
                NewRow["Notes"] = Notes;
                NewRow["SawName"] = SawName;
                if (FactCount != 0)
                    NewRow["FactCount"] = FactCount;
                if (DisprepancyCount != 0)
                    NewRow["DisprepancyCount"] = DisprepancyCount;
                if (DefectCount != 0)
                    NewRow["DefectCount"] = DefectCount;
                CuttingAssignmentsDT.Rows.Add(NewRow);
            }
        }

        private void FillMillingAssignmentsDT(DataTable DT1)
        {
            MillingAssignmentsDT.Clear();
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                string Name1 = string.Empty;
                string Name2 = string.Empty;
                string Notes = string.Empty;
                decimal Amount = 0;
                int Length2 = 0;
                int Width1 = 0;
                int PlanCount = 0;
                int FactCount = 0;
                int DisprepancyCount = 0;
                int DefectCount = 0;

                int TechStoreID1 = 0;
                int TechStoreID2 = 0;

                if (DT1.Rows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1 = Convert.ToInt32(DT1.Rows[i]["TechStoreID1"]);
                if (DT1.Rows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]);
                if (DT1.Rows[i]["Length2"] != DBNull.Value)
                    Length2 = Convert.ToInt32(DT1.Rows[i]["Length2"]);
                if (DT1.Rows[i]["Width1"] != DBNull.Value)
                    Width1 = Convert.ToInt32(DT1.Rows[i]["Width1"]);
                if (DT1.Rows[i]["PlanCount"] != DBNull.Value)
                    PlanCount = Convert.ToInt32(DT1.Rows[i]["PlanCount"]);
                if (DT1.Rows[i]["FactCount"] != DBNull.Value)
                    FactCount = Convert.ToInt32(DT1.Rows[i]["FactCount"]);
                if (DT1.Rows[i]["DisprepancyCount"] != DBNull.Value)
                    DisprepancyCount = Convert.ToInt32(DT1.Rows[i]["DisprepancyCount"]);
                if (DT1.Rows[i]["DefectCount"] != DBNull.Value)
                    DefectCount = Convert.ToInt32(DT1.Rows[i]["DefectCount"]);

                Name1 = GetTechStoreName(TechStoreID1);
                Name2 = GetTechStoreName(TechStoreID2);
                Notes = DT1.Rows[i]["Notes"].ToString();
                Amount = Convert.ToDecimal(PlanCount) * Convert.ToDecimal(Length2) / 1000;
                Amount = Decimal.Round(Amount, 1, MidpointRounding.AwayFromZero);

                DataRow NewRow = MillingAssignmentsDT.NewRow();
                NewRow["Name1"] = Name1;
                NewRow["Name2"] = Name2;
                if (Amount != 0)
                    NewRow["Amount"] = Amount;
                if (Length2 != 0)
                    NewRow["Length2"] = Length2;
                if (Width1 != 0)
                    NewRow["Width1"] = Width1;
                NewRow["PlanCount"] = PlanCount;
                NewRow["Notes"] = Notes;
                if (FactCount != 0)
                    NewRow["FactCount"] = FactCount;
                if (DisprepancyCount != 0)
                    NewRow["DisprepancyCount"] = DisprepancyCount;
                if (DefectCount != 0)
                    NewRow["DefectCount"] = DefectCount;
                MillingAssignmentsDT.Rows.Add(NewRow);
            }
        }

        private void FillEnvelopingAssignmentsDT(DataTable DT1)
        {
            EnvelopingAssignmentsDT.Clear();
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                string Name2 = string.Empty;
                string CoverName = string.Empty;
                string Notes = string.Empty;
                decimal Amount = 0;
                int Length2 = 0;
                int PlanCount = 0;
                int FactCount = 0;
                int DisprepancyCount = 0;
                int DefectCount = 0;

                int TechStoreID1 = 0;
                int TechStoreID2 = 0;
                int CoverID2 = 0;

                if (DT1.Rows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1 = Convert.ToInt32(DT1.Rows[i]["TechStoreID1"]);
                if (DT1.Rows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]);
                if (DT1.Rows[i]["CoverID2"] != DBNull.Value)
                    CoverID2 = Convert.ToInt32(DT1.Rows[i]["CoverID2"]);
                if (DT1.Rows[i]["Length2"] != DBNull.Value)
                    Length2 = Convert.ToInt32(DT1.Rows[i]["Length2"]);
                if (DT1.Rows[i]["PlanCount"] != DBNull.Value)
                    PlanCount = Convert.ToInt32(DT1.Rows[i]["PlanCount"]);
                if (DT1.Rows[i]["FactCount"] != DBNull.Value)
                    FactCount = Convert.ToInt32(DT1.Rows[i]["FactCount"]);
                if (DT1.Rows[i]["DisprepancyCount"] != DBNull.Value)
                    DisprepancyCount = Convert.ToInt32(DT1.Rows[i]["DisprepancyCount"]);
                if (DT1.Rows[i]["DefectCount"] != DBNull.Value)
                    DefectCount = Convert.ToInt32(DT1.Rows[i]["DefectCount"]);

                Notes = DT1.Rows[i]["Notes"].ToString();
                Name2 = GetTechStoreName(TechStoreID2);
                CoverName = GetCoverName(CoverID2);
                Amount = Convert.ToDecimal(PlanCount) * Convert.ToDecimal(Length2) / 1000;
                Amount = Decimal.Round(Amount, 1, MidpointRounding.AwayFromZero);

                DataRow NewRow = EnvelopingAssignmentsDT.NewRow();
                NewRow["Name2"] = Name2;
                NewRow["CoverName"] = CoverName;
                NewRow["PlanCount"] = PlanCount;
                NewRow["Notes"] = Notes;
                if (Length2 != 0)
                    NewRow["Length2"] = Length2;
                if (Amount != 0)
                    NewRow["Amount"] = Amount;
                if (FactCount != 0)
                    NewRow["FactCount"] = FactCount;
                if (DisprepancyCount != 0)
                    NewRow["DisprepancyCount"] = DisprepancyCount;
                if (DefectCount != 0)
                    NewRow["DefectCount"] = DefectCount;
                EnvelopingAssignmentsDT.Rows.Add(NewRow);
            }
        }

        private void FillKashirAssignmentsDT(DataTable DT1)
        {
            KashirAssignmentsDT.Clear();
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                string Name1 = string.Empty;
                string Name2 = string.Empty;
                string CoverName = string.Empty;
                string Notes = string.Empty;
                decimal Amount = 0;
                int Length2 = 0;
                decimal Width2 = 0;
                int PlanCount = 0;
                int FactCount = 0;
                int DisprepancyCount = 0;
                int DefectCount = 0;

                int TechStoreID1 = 0;
                int TechStoreID2 = 0;
                int CoverID2 = 0;

                if (DT1.Rows[i]["TechStoreID1"] != DBNull.Value)
                    TechStoreID1 = Convert.ToInt32(DT1.Rows[i]["TechStoreID1"]);
                if (DT1.Rows[i]["TechStoreID2"] != DBNull.Value)
                    TechStoreID2 = Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]);
                if (DT1.Rows[i]["CoverID2"] != DBNull.Value)
                    CoverID2 = Convert.ToInt32(DT1.Rows[i]["CoverID2"]);
                if (DT1.Rows[i]["Length2"] != DBNull.Value)
                    Length2 = Convert.ToInt32(DT1.Rows[i]["Length2"]);
                if (DT1.Rows[i]["Width2"] != DBNull.Value)
                    Width2 = Convert.ToDecimal(DT1.Rows[i]["Width2"]);
                if (DT1.Rows[i]["PlanCount"] != DBNull.Value)
                    PlanCount = Convert.ToInt32(DT1.Rows[i]["PlanCount"]);
                if (DT1.Rows[i]["FactCount"] != DBNull.Value)
                    FactCount = Convert.ToInt32(DT1.Rows[i]["FactCount"]);
                if (DT1.Rows[i]["DisprepancyCount"] != DBNull.Value)
                    DisprepancyCount = Convert.ToInt32(DT1.Rows[i]["DisprepancyCount"]);
                if (DT1.Rows[i]["DefectCount"] != DBNull.Value)
                    DefectCount = Convert.ToInt32(DT1.Rows[i]["DefectCount"]);

                Notes = DT1.Rows[i]["Notes"].ToString();
                Name1 = GetTechStoreName(TechStoreID1);
                Name2 = GetTechStoreName(TechStoreID2);
                CoverName = GetCoverName(CoverID2);
                Amount = Convert.ToDecimal(Width2) * Convert.ToDecimal(Length2) / 1000000;
                Amount = Decimal.Round(Amount, 2, MidpointRounding.AwayFromZero);

                DataRow NewRow = KashirAssignmentsDT.NewRow();
                NewRow["Name1"] = Name1;
                NewRow["Name2"] = Name2;
                NewRow["CoverName"] = CoverName;
                NewRow["PlanCount"] = PlanCount;
                NewRow["Notes"] = Notes;
                if (Length2 != 0)
                    NewRow["Length2"] = Length2;
                if (Width2 != 0)
                    NewRow["Width2"] = Width2;
                if (Amount != 0)
                    NewRow["Amount"] = Amount;
                if (FactCount != 0)
                    NewRow["FactCount"] = FactCount;
                if (DisprepancyCount != 0)
                    NewRow["DisprepancyCount"] = DisprepancyCount;
                if (DefectCount != 0)
                    NewRow["DefectCount"] = DefectCount;
                KashirAssignmentsDT.Rows.Add(NewRow);
            }
        }

        //private void FillPackingAssignmentsDT(DataTable DT1)
        //{
        //    PackingAssignmentsDT.Clear();
        //    for (int i = 0; i < DT1.Rows.Count; i++)
        //    {
        //        string CoverName = string.Empty;
        //        string Name2 = string.Empty;
        //        string Notes = string.Empty;
        //        decimal Amount = 0;
        //        int Length2 = 0;
        //        int PlanCount = 0;
        //        int FactCount = 0;
        //        int DisprepancyCount = 0;
        //        int DefectCount = 0;

        //        int TechStoreID1 = 0;
        //        int TechStoreID2 = 0;
        //        int CoverID2 = 0;

        //        if (DT1.Rows[i]["TechStoreID1"] != DBNull.Value)
        //            TechStoreID1 = Convert.ToInt32(DT1.Rows[i]["TechStoreID1"]);
        //        if (DT1.Rows[i]["TechStoreID2"] != DBNull.Value)
        //            TechStoreID2 = Convert.ToInt32(DT1.Rows[i]["TechStoreID2"]);
        //        if (DT1.Rows[i]["CoverID2"] != DBNull.Value)
        //            CoverID2 = Convert.ToInt32(DT1.Rows[i]["CoverID2"]);
        //        if (DT1.Rows[i]["Length2"] != DBNull.Value)
        //            Length2 = Convert.ToInt32(DT1.Rows[i]["Length2"]);
        //        if (DT1.Rows[i]["PlanCount"] != DBNull.Value)
        //            PlanCount = Convert.ToInt32(DT1.Rows[i]["PlanCount"]);
        //        if (DT1.Rows[i]["FactCount"] != DBNull.Value)
        //            FactCount = Convert.ToInt32(DT1.Rows[i]["FactCount"]);
        //        if (DT1.Rows[i]["DisprepancyCount"] != DBNull.Value)
        //            DisprepancyCount = Convert.ToInt32(DT1.Rows[i]["DisprepancyCount"]);
        //        if (DT1.Rows[i]["DefectCount"] != DBNull.Value)
        //            DefectCount = Convert.ToInt32(DT1.Rows[i]["DefectCount"]);

        //        CoverName = GetCoverName(CoverID2);
        //        Notes = DT1.Rows[i]["Notes"].ToString();
        //        Name2 = GetTechStoreName(TechStoreID2);
        //        Amount = Convert.ToDecimal(PlanCount) * Convert.ToDecimal(Length2) / 1000;
        //        Amount = Decimal.Round(Amount, 1, MidpointRounding.AwayFromZero);

        //        DataRow NewRow = PackingAssignmentsDT.NewRow();
        //        NewRow["Name2"] = Name2;
        //        NewRow["CoverName"] = CoverName;
        //        NewRow["PlanCount"] = PlanCount;
        //        //NewRow["Notes"] = Notes;
        //        if (Length2 != 0)
        //            NewRow["Length2"] = Length2;
        //        //if (Amount != 0)
        //        //    NewRow["Amount"] = Amount;
        //        //if (FactCount != 0)
        //        //    NewRow["FactCount"] = FactCount;
        //        //if (DisprepancyCount != 0)
        //        //    NewRow["DisprepancyCount"] = DisprepancyCount;
        //        //if (DefectCount != 0)
        //        //    NewRow["DefectCount"] = DefectCount;
        //        PackingAssignmentsDT.Rows.Add(NewRow);
        //    }
        //}

        public void CreateExcel(int BatchAssignmentID, DateTime date)
        {
            GetCurrentDate();
            hssfworkbook = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            Calibri10F = hssfworkbook.CreateFont();
            Calibri10F.FontHeightInPoints = 9;
            Calibri10F.FontName = "Calibri";

            CalibriBold10F = hssfworkbook.CreateFont();
            CalibriBold10F.FontHeightInPoints = 10;
            CalibriBold10F.FontName = "Calibri";
            CalibriBold10F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

            Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 9;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            TableNameCS = hssfworkbook.CreateCellStyle();
            TableNameCS.SetFont(CalibriBold11F);

            TableHeaderCS = hssfworkbook.CreateCellStyle();
            TableHeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            TableHeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TableHeaderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.WrapText = true;
            TableHeaderCS.SetFont(CalibriBold11F);

            TableContentCS = hssfworkbook.CreateCellStyle();
            TableContentCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            TableContentCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TableContentCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableContentCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableContentCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableContentCS.RightBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableContentCS.TopBorderColor = HSSFColor.BLACK.index;
            TableContentCS.WrapText = true;
            TableContentCS.SetFont(Calibri10F);

            TableDecContentCS = hssfworkbook.CreateCellStyle();
            TableDecContentCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            TableDecContentCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            TableDecContentCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableDecContentCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableDecContentCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableDecContentCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableDecContentCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableDecContentCS.RightBorderColor = HSSFColor.BLACK.index;
            TableDecContentCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableDecContentCS.TopBorderColor = HSSFColor.BLACK.index;
            TableDecContentCS.WrapText = true;
            TableDecContentCS.SetFont(Calibri10F);

            FillEnvelopingAssignmentsDT(Barberan1DT);
            if (EnvelopingAssignmentsDT.Rows.Count > 0)
                Barberan1AssignmentToExcel(BatchAssignmentID, EnvelopingAssignmentsDT, date);
            FillEnvelopingAssignmentsDT(Barberan2DT);
            if (EnvelopingAssignmentsDT.Rows.Count > 0)
                Barberan2AssignmentToExcel(BatchAssignmentID, EnvelopingAssignmentsDT, date);

            FillMillingAssignmentsDT(Frezer1DT);
            if (MillingAssignmentsDT.Rows.Count > 0)
                Frezer1AssignmentToExcel(BatchAssignmentID, MillingAssignmentsDT, date);
            FillMillingAssignmentsDT(Frezer2DT);
            if (MillingAssignmentsDT.Rows.Count > 0)
                Frezer2AssignmentToExcel(BatchAssignmentID, MillingAssignmentsDT, date);
            FillMillingAssignmentsDT(Frezer3DT);
            if (MillingAssignmentsDT.Rows.Count > 0)
                Frezer3AssignmentToExcel(BatchAssignmentID, MillingAssignmentsDT, date);

            FillCuttingAssignmentsDT(HolzmaDT);
            if (CuttingAssignmentsDT.Rows.Count > 0)
                HolzmaAssignmentToExcel(BatchAssignmentID, CuttingAssignmentsDT, date);

            FillKashirAssignmentsDT(KashirDT);
            if (KashirAssignmentsDT.Rows.Count > 0)
                KashirAssignmentToExcel(BatchAssignmentID, KashirAssignmentsDT, date);

            //FillPackingAssignmentsDT(PackingDT);
            //if (PackingDT.Rows.Count > 0)
            //    PackingAssignmentToExcel(PackingAssignmentsDT, date);

            string FileName = "Партия №" + BatchAssignmentID;
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            //string FileName = "№" + WorkAssignmentID + " " + BatchName;
            //string tempFolder = @"\\192.168.1.6\Public\ТПС\Infinium\Задания\";
            string CurrentMonthName = DateTime.Now.ToString("MMMM");
            tempFolder = Path.Combine(tempFolder, CurrentMonthName);
            if (!(Directory.Exists(tempFolder)))
            {
                Directory.CreateDirectory(tempFolder);
            }
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
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

        public void GetAssignmentInfo(ref object date1, ref object date2, ref object date3, ref object date4,
            ref object User1, ref object User2, ref object User3, ref object User4)
        {
            ToolsConfirmDate = date1;
            TechnologyConfirmDate = date2;
            MaterialConfirmDate = date3;
            TechnicalConfirmDate = date4;
            PrintDateTime = Security.GetCurrentDate();
            ToolsConfirmUserID = User1;
            TechnologyConfirmUserID = User2;
            MaterialConfirmUserID = User3;
            TechnicalConfirmUserID = User4;
            PrintUserID = Security.CurrentUserID;
        }

        private void Barberan1AssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Barberan RP-30");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 20 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 8 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 8 * 256);
            sheet1.SetColumnWidth(5, 11 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 8 * 256);
            sheet1.SetColumnWidth(8, 12 * 256);
            sheet1.SetColumnWidth(9, 12 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 6, "Утверждаю...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "ОКУТЫВАНИЕ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Barberan RP-30");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Профиль");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол-во, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "мп");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Факт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void Barberan2AssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Barberan PUR-33");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 20 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 8 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 8 * 256);
            sheet1.SetColumnWidth(5, 11 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 8 * 256);
            sheet1.SetColumnWidth(8, 12 * 256);
            sheet1.SetColumnWidth(9, 12 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 6, "Утверждаю...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "ОКУТЫВАНИЕ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Barberan PUR-33");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Профиль");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол-во, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "мп");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Факт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void HolzmaAssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("HOLZMA HPP 350");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 8 * 256);
            sheet1.SetColumnWidth(1, 10 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 12 * 256);
            sheet1.SetColumnWidth(4, 8 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 8 * 256);
            sheet1.SetColumnWidth(7, 8 * 256);
            sheet1.SetColumnWidth(8, 15 * 256);
            sheet1.SetColumnWidth(9, 8 * 256);
            sheet1.SetColumnWidth(10, 18 * 256);
            sheet1.SetColumnWidth(11, 12 * 256);
            sheet1.SetColumnWidth(12, 12 * 256);
            sheet1.SetColumnWidth(13, 12 * 256);
            sheet1.SetColumnWidth(14, 12 * 256);

            HSSFCell cell = null;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 9, "УТВЕРЖДАЮ...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "РАСКРОЙ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 3, "HOLZMA HPP 350");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "№ паллеты");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "МДФ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Размер");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, 2, RowIndex, 3));

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), 2, "длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), 3, "ширина, мм");
            cell.CellStyle = TableHeaderCS;
            DisplayIndex++;
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Лист, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Наименование");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Полоса, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "План, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Толщина пильного диска");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Факт, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            cell.CellStyle = TableHeaderCS;
            sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            DisplayIndex++;

            RowIndex++;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void Frezer1AssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("SCM Superset XL");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 8 * 256);
            sheet1.SetColumnWidth(5, 8 * 256);
            sheet1.SetColumnWidth(6, 11 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 8 * 256);
            sheet1.SetColumnWidth(9, 12 * 256);
            sheet1.SetColumnWidth(10, 15 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 9, "УТВЕРЖДАЮ...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "ФРЕЗЕРОВАНИЕ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "SCM Superset XL");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "МДФ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "ПРОФИЛЬ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина черновой полосы, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол-во, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "мп");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Факт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void Frezer2AssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Weinig Powermat 1200");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 8 * 256);
            sheet1.SetColumnWidth(5, 8 * 256);
            sheet1.SetColumnWidth(6, 11 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 8 * 256);
            sheet1.SetColumnWidth(9, 12 * 256);
            sheet1.SetColumnWidth(10, 15 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 9, "УТВЕРЖДАЮ...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "ФРЕЗЕРОВАНИЕ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Weinig Powermat 1200");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "МДФ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "ПРОФИЛЬ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина черновой полосы, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол-во, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, м");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "мп");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Факт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void Frezer3AssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Casolin F45");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 8 * 256);
            sheet1.SetColumnWidth(5, 8 * 256);
            sheet1.SetColumnWidth(6, 11 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 8 * 256);
            sheet1.SetColumnWidth(9, 12 * 256);
            sheet1.SetColumnWidth(10, 15 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 9, "УТВЕРЖДАЮ...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "ФРЕЗЕРОВАНИЕ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Casolin F45");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "МДФ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "ПРОФИЛЬ");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина черновой полосы, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол-во, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, м");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "мп");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Факт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void KashirAssignmentToExcel(int BatchAssignmentID, DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Кашир");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 8 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 8 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);
            sheet1.SetColumnWidth(4, 20 * 256);
            sheet1.SetColumnWidth(5, 8 * 256);
            sheet1.SetColumnWidth(6, 8 * 256);
            sheet1.SetColumnWidth(7, 8 * 256);
            sheet1.SetColumnWidth(8, 15 * 256);
            sheet1.SetColumnWidth(9, 8 * 256);
            sheet1.SetColumnWidth(10, 12 * 256);
            sheet1.SetColumnWidth(11, 12 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 6, "Утверждаю...............");
            cell.CellStyle = TableNameCS;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ №" + BatchAssignmentID);
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "КАШИР");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Barberan PUR-33");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Клей");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Вставка");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "План, шт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "м.кв.");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Плановое время, ч");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Примечание");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Факт");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Брак/причина");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Исполнитель");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
            RowIndex++;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "СОГЛАСОВАНО:");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Инструмент:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(ToolsConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Технология:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnologyConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Материалы:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(MaterialConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnicalConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Техническое состояние:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(TechnicalConfirmUserID)));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Распечатал:");
                cell.CellStyle = Calibri11CS;
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 5, GetUserName(Convert.ToInt32(PrintUserID)));
                cell.CellStyle = Calibri11CS;
            }
        }

        private void PackingAssignmentToExcel(DataTable DT, DateTime date)
        {
            int RowIndex = 0;
            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Упаковка");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 20 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 8 * 256);
            sheet1.SetColumnWidth(3, 8 * 256);

            HSSFCell cell = null;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, date.ToString("dd MMMM yyyy"));
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 6, "Утверждаю...............");
            cell.CellStyle = TableNameCS;
            if (ToolsConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Инструмент: " + GetUserName(Convert.ToInt32(ToolsConfirmUserID)) + " " + Convert.ToDateTime(ToolsConfirmDate).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (TechnologyConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Технология: " + GetUserName(Convert.ToInt32(TechnologyConfirmUserID)) + " " + Convert.ToDateTime(TechnologyConfirmDate).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (MaterialConfirmDate != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Материалы: " + GetUserName(Convert.ToInt32(MaterialConfirmUserID)) + " " + Convert.ToDateTime(MaterialConfirmDate).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }
            if (PrintDateTime != DBNull.Value)
            {
                cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Распечатал: " + GetUserName(Convert.ToInt32(PrintUserID)) + " " + Convert.ToDateTime(PrintDateTime).ToString("dd.MM.yyyy HH:mm"));
                cell.CellStyle = Calibri11CS;
            }

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ЗАДАНИЕ НА");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "УПАКОВКУ");
            cell.CellStyle = TableNameCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 2, "Участок упаковки");
            cell.CellStyle = TableNameCS;

            int DisplayIndex = 0;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Профиль");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина, мм");
            cell.CellStyle = TableHeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол-во, шт");
            cell.CellStyle = TableHeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    if (DT.Columns[y].ColumnName == "DisprepancyCount")
                        continue;
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = TableDecContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = TableContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = TableContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }
        }
    }
}
