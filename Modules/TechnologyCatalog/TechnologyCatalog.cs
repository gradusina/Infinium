using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.TechnologyCatalog
{
    public class PatinaManager
    {
        public BindingSource PatinaBS;
        public BindingSource PatinaRalBS;
        DataTable PatinaDT;
        DataTable PatinaRalDT;
        SqlCommandBuilder PatinaRalSCB;
        SqlDataAdapter PatinaRalSDA;
        SqlCommandBuilder PatinaSCB;
        SqlDataAdapter PatinaSDA;

        public PatinaManager()
        {
            PatinaDT = new DataTable();
            PatinaRalDT = new DataTable();
            PatinaBS = new BindingSource();
            PatinaRalBS = new BindingSource();
            string SelectCommand = @"SELECT * FROM Patina WHERE PatinaID NOT IN (-1,0) ORDER BY PatinaName";
            PatinaSDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString);
            PatinaSCB = new SqlCommandBuilder(PatinaSDA);
            PatinaSDA.Fill(PatinaDT);

            SelectCommand = @"SELECT * FROM PatinaRAL ORDER BY PatinaRAL";
            PatinaRalSDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString);
            PatinaRalSCB = new SqlCommandBuilder(PatinaRalSDA);
            PatinaRalSDA.Fill(PatinaRalDT);

            PatinaBS.DataSource = PatinaDT;
            PatinaRalBS.DataSource = PatinaRalDT;
        }

        public void ChangePatinaGroup(int PatinaRALID, int PatinaID)
        {
            DataRow[] Rows = PatinaRalDT.Select("PatinaRALID = " + PatinaRALID);
            if (Rows.Count() > 0)
            {
                Rows[0]["PatinaID"] = PatinaID;
            }
        }

        public void RemoveCurrentPatina()
        {
            if (PatinaBS.Count == 0)
                return;
            int Pos = PatinaBS.Position;
            PatinaBS.RemoveCurrent();
            if (PatinaBS.Count > 0)
                if (Pos >= PatinaBS.Count)
                    PatinaBS.MoveLast();
                else
                    PatinaBS.Position = Pos;
        }

        public void RemoveCurrentPatinaRal()
        {
            if (PatinaRalBS.Count == 0)
                return;
            int Pos = PatinaRalBS.Position;
            PatinaRalBS.RemoveCurrent();
            if (PatinaRalBS.Count > 0)
                if (Pos >= PatinaRalBS.Count)
                    PatinaRalBS.MoveLast();
                else
                    PatinaRalBS.Position = Pos;
        }

        public void SavePatina()
        {
            PatinaSDA.Update(PatinaDT);
            PatinaDT.Clear();
            PatinaSDA.Fill(PatinaDT);
        }

        public void SavePatinaRal()
        {
            PatinaRalSDA.Update(PatinaRalDT);
            PatinaRalDT.Clear();
            PatinaRalSDA.Fill(PatinaRalDT);
        }

        public void FilterPatinaRAL(int PatinaID)
        {
            PatinaRalBS.Filter = "PatinaID=" + PatinaID;
        }
    }

    public class TechCatalogEvents
    {
        public static void SaveEvents(string Event)
        {
            DataTable EventsDataTable = new DataTable();

            TimeSpan DeltaTime = Security.GetCurrentDate() - DateTime.Now;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechCatalogEventsJournal",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(EventsDataTable);
                    DataRow NewRow = EventsDataTable.NewRow();
                    NewRow["UserID"] = Security.CurrentUserID;
                    NewRow["LoginJournalID"] = Security.CurrentLoginJournalID;
                    NewRow["Event"] = Event;
                    NewRow["EventDateTime"] = DateTime.Now + DeltaTime;
                    EventsDataTable.Rows.Add(NewRow);
                    DA.Update(EventsDataTable);
                }
            }
        }
    }

    public class TechCatalogOperationsTerms
    {
        BindingSource ColorsBS;
        DataTable ColorsDT;
        BindingSource CoversBS;
        DataTable CoversDT;
        BindingSource InsetColorsBS;
        DataTable InsetColorsDT;
        BindingSource InsetTypesBS;
        DataTable InsetTypesDT;
        int iTechCatalogOperationsDetailID = 0;
        BindingSource MathSymbolsBS;
        BindingSource LogicOperationsBS;
        DataTable MathSymbolsDT;
        DataTable LogicOperationsDT;
        BindingSource ParametersBS;
        DataTable ParametersDT;
        BindingSource PatinaBS;
        DataTable PatinaDT;
        BindingSource TermsBS;
        DataTable TermsDT;
        public TechCatalogOperationsTerms()
        {

        }

        public DataGridViewComboBoxColumn ParameterColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ParameterColumn",
                    HeaderText = "Параметр",
                    DataPropertyName = "Parameter",
                    DataSource = new DataView(ParametersDT),
                    ValueMember = "Parameter",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                return Column;
            }
        }

        public DataGridViewComboBoxColumn LogicOperationsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "LogicOperationsColumn",
                    HeaderText = "Лог. операция",
                    DataPropertyName = "LogicOperation",
                    DataSource = new DataView(LogicOperationsDT),
                    ValueMember = "LogicOperation",
                    DisplayMember = "LogicOperation",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                return Column;
            }
        }

        public int TechCatalogOperationsDetailID
        {
            set { iTechCatalogOperationsDetailID = value; }
        }

        #region DataSources

        public BindingSource ColorsList
        {
            get { return ColorsBS; }
        }

        public BindingSource CoversList
        {
            get { return CoversBS; }
        }

        public BindingSource InsetColorsList
        {
            get { return InsetColorsBS; }
        }

        public BindingSource InsetTypesList
        {
            get { return InsetTypesBS; }
        }

        public BindingSource MathSymbolsList
        {
            get { return MathSymbolsBS; }
        }
        public BindingSource LogicOperationsList
        {
            get { return LogicOperationsBS; }
        }

        public BindingSource ParametersList
        {
            get { return ParametersBS; }
        }

        public BindingSource PatinaList
        {
            get { return PatinaBS; }
        }

        public BindingSource TermsList
        {
            get { return TermsBS; }
        }

        #endregion
        public void AddMathSymbol(string MathSymbol)
        {
            DataRow NewRow = MathSymbolsDT.NewRow();
            NewRow["MathSymbol"] = MathSymbol;
            MathSymbolsDT.Rows.Add(NewRow);
        }

        public void AddLogicOperation(string LogicOperation)
        {
            DataRow NewRow = LogicOperationsDT.NewRow();
            NewRow["LogicOperation"] = LogicOperation;
            LogicOperationsDT.Rows.Add(NewRow);
        }

        public void AddTerm(string Parameter, string MathSymbol, decimal Term, string LogicOperation)
        {
            DataRow NewRow = TermsDT.NewRow();
            NewRow["TechCatalogOperationsDetailID"] = iTechCatalogOperationsDetailID;
            NewRow["Parameter"] = Parameter;
            NewRow["MathSymbol"] = MathSymbol;
            NewRow["Term"] = Term;
            NewRow["LogicOperation"] = LogicOperation;
            TermsDT.Rows.Add(NewRow);
        }

        public void ClearTerms()
        {
            TermsDT.Clear();
        }

        public void FillParameterNames()
        {
            int Term = 0;
            string Parameter = string.Empty;
            string ParameterName = string.Empty;
            for (int i = 0; i < TermsDT.Rows.Count; i++)
            {
                Term = Convert.ToInt32(TermsDT.Rows[i]["Term"]);
                Parameter = TermsDT.Rows[i]["Parameter"].ToString();
                switch (Parameter)
                {
                    case "CoverID":
                        DataRow[] rows1 = CoversDT.Select("CoverID = " + Term);
                        if (rows1.Count() > 0)
                            ParameterName = rows1[0]["CoverName"].ToString();
                        break;
                    case "InsetTypeID":
                        DataRow[] rows2 = InsetTypesDT.Select("InsetTypeID = " + Term);
                        if (rows2.Count() > 0)
                            ParameterName = rows2[0]["InsetTypeName"].ToString();
                        break;
                    case "InsetColorID":
                        DataRow[] rows3 = InsetColorsDT.Select("InsetColorID = " + Term);
                        if (rows3.Count() > 0)
                            ParameterName = rows3[0]["InsetColorName"].ToString();
                        break;
                    case "ColorID":
                        DataRow[] rows4 = ColorsDT.Select("ColorID = " + Term);
                        if (rows4.Count() > 0)
                            ParameterName = rows4[0]["ColorName"].ToString();
                        break;
                    case "PatinaID":
                        DataRow[] rows5 = PatinaDT.Select("PatinaID = " + Term);
                        if (rows5.Count() > 0)
                            ParameterName = rows5[0]["PatinaName"].ToString();
                        break;
                    default:
                        ParameterName = Term.ToString();
                        break;
                }
                TermsDT.Rows[i]["TermDisplayName"] = ParameterName;
            }
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            FillParameterNames();
        }

        public bool RemoveTerm()
        {
            if (TermsBS.Current == null)
            {
                return false;
            }
            TermsBS.RemoveCurrent();
            return true;
        }

        public void SaveChangesToTechCatalogOperations()
        {
            string SelectCommand = "SELECT TechCatalogOperationsDetailID, IsPerform FROM TechCatalogOperationsDetail" +
                " WHERE TechCatalogOperationsDetailID = " + iTechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (TermsDT.Rows.Count > 0)
                                DT.Rows[0]["IsPerform"] = true;
                            else
                                DT.Rows[0]["IsPerform"] = false;
                            DA.Update(DT);
                        }

                    }
                }
            }
        }

        public void SaveTerms()
        {
            string SelectCommand = "SELECT TOP 0 * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TermsDT);
                }
            }
        }

        public void UpdateTerms()
        {
            string SelectCommand = "SELECT * FROM TechCatalogOperationsTerms WHERE TechCatalogOperationsDetailID = " + iTechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TermsDT);
            }
        }

        private void AddParameter(string Parameter, string Name)
        {
            DataRow NewRow = ParametersDT.NewRow();
            NewRow["Parameter"] = Parameter;
            NewRow["Name"] = Name;
            ParametersDT.Rows.Add(NewRow);
        }

        private void Binding()
        {
            ColorsBS.DataSource = ColorsDT;
            CoversBS.DataSource = CoversDT;
            InsetColorsBS.DataSource = InsetColorsDT;
            InsetTypesBS.DataSource = InsetTypesDT;
            MathSymbolsBS.DataSource = MathSymbolsDT;
            LogicOperationsBS.DataSource = LogicOperationsDT;
            PatinaBS.DataSource = PatinaDT;
            ParametersBS.DataSource = ParametersDT;
            TermsBS.DataSource = TermsDT;
        }

        private void Create()
        {
            ColorsDT = new DataTable();
            CoversDT = new DataTable();
            InsetColorsDT = new DataTable();
            InsetTypesDT = new DataTable();
            MathSymbolsDT = new DataTable();
            LogicOperationsDT = new DataTable();
            PatinaDT = new DataTable();
            ParametersDT = new DataTable();
            TermsDT = new DataTable();

            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));
            MathSymbolsDT.Columns.Add(new DataColumn("MathSymbol", Type.GetType("System.String")));
            LogicOperationsDT.Columns.Add(new DataColumn("LogicOperation", Type.GetType("System.String")));
            ParametersDT.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            ParametersDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));

            ColorsBS = new BindingSource();
            CoversBS = new BindingSource();
            InsetColorsBS = new BindingSource();
            InsetTypesBS = new BindingSource();
            MathSymbolsBS = new BindingSource();
            LogicOperationsBS = new BindingSource();
            PatinaBS = new BindingSource();
            ParametersBS = new BindingSource();
            TermsBS = new BindingSource();
        }

        private void CreateCoversDT()
        {
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

        private void Fill()
        {
            CreateCoversDT();

            string SelectCommand = "SELECT Colors.*, TechStoreName FROM Colors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore" +
                " ON Colors.ColorID = infiniu2_catalog.dbo.TechStore.TechStoreID" +
                " ORDER BY ColorsGroupID, TechStoreName";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(ColorsDT);
            //    {
            //        DataRow EmptyRow1 = ColorsDT.NewRow();
            //        EmptyRow1["ColorID"] = -1;
            //        EmptyRow1["TechStoreName"] = "-";
            //        ColorsDT.Rows.InsertAt(EmptyRow1, 0);

            //        DataRow ChoiceRow1 = ColorsDT.NewRow();
            //        ChoiceRow1["ColorID"] = 0;
            //        ChoiceRow1["TechStoreName"] = "на выбор";
            //        ColorsDT.Rows.InsertAt(ChoiceRow1, 1);
            //    }
            //}
            GetColorsDT();
            GetInsetColorsDT();
            //SelectCommand = "SELECT * FROM InsetColors ORDER BY InsetColorName";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(InsetColorsDT);
            //}
            SelectCommand = "SELECT * FROM InsetTypes ORDER BY InsetTypeName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDT);
            }
            SelectCommand = "SELECT * FROM Patina ORDER BY PatinaName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDT);
            }
            SelectCommand = "SELECT * FROM TechCatalogOperationsTerms WHERE TechCatalogOperationsDetailID = " + iTechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TermsDT);
                TermsDT.Columns.Add(new DataColumn("TermDisplayName", Type.GetType("System.String")));
            }
            AddMathSymbol("=");
            AddMathSymbol("!=");
            AddMathSymbol("<=");
            AddMathSymbol(">=");
            AddLogicOperation("&&");
            AddLogicOperation("||");
            AddParameter("CoverID", "Облицовка");
            AddParameter("InsetTypeID", "Тип наполнителя");
            AddParameter("InsetColorID", "Цвет наполнителя");
            AddParameter("ColorID", "Цвет");
            AddParameter("PatinaID", "Патина");
            AddParameter("Diameter", "Диаметр");
            AddParameter("Thickness", "Толщина");
            AddParameter("Length", "Длина");
            AddParameter("Height", "Высота");
            AddParameter("Width", "Ширина");
            AddParameter("Admission", "Допуск");
            AddParameter("InsetHeightAdmission", "Допуск на вставку по высоте");
            AddParameter("InsetWidthAdmission", "Допуск на вставку по ширине");
            AddParameter("Capacity", "Емкость");
            AddParameter("Weight", "Вес");
        }

        public void RefreshTerms()
        {
            string SelectCommand = "SELECT * FROM TechCatalogOperationsTerms WHERE TechCatalogOperationsDetailID = " + iTechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TermsDT.Clear();
                DA.Fill(TermsDT);
            }
            FillParameterNames();
        }

        public string GetColorName(int TechStoreID)
        {
            string ColorName = string.Empty;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore WHERE TechStoreID=" + TechStoreID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    try
                    {
                        DataRow[] Rows = DT.Select("TechStoreID = " + TechStoreID);
                        ColorName = Rows[0]["TechStoreName"].ToString();
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
            return ColorName;
        }

        private void GetColorsDT()
        {
            ColorsDT = new DataTable();
            ColorsDT.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("GroupID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
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
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["GroupID"] = 1;
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["GroupID"] = Convert.ToInt64(DT.Rows[i]["GroupID"]);
                        NewRow["ColorName"] = GetColorName(Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = -1;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "-";
                ColorsDT.Rows.Add(NewRow);
            }
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(ColorsDT))
            {
                DV.Sort = "GroupID, ColorName";
                Table = DV.ToTable();
            }
            ColorsDT.Clear();
            ColorsDT = Table.Copy();
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDT);
                {
                    DataRow NewRow = InsetColorsDT.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDT.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDT.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDT.Rows.Add(NewRow);
                }

            }

        }
    }

    public class TechCatalogStoreDetailTerms
    {
        BindingSource ColorsBS;
        DataTable ColorsDT;
        BindingSource CoversBS;
        DataTable CoversDT;
        BindingSource InsetColorsBS;
        DataTable InsetColorsDT;
        BindingSource InsetTypesBS;
        DataTable InsetTypesDT;
        int iTechCatalogStoreDetailID = 0;
        BindingSource MathSymbolsBS;
        BindingSource LogicOperationsBS;
        DataTable MathSymbolsDT;
        DataTable LogicOperationsDT;
        BindingSource ParametersBS;
        DataTable ParametersDT;
        BindingSource PatinaBS;
        DataTable PatinaDT;
        BindingSource TermsBS;
        DataTable TermsDT;
        public TechCatalogStoreDetailTerms()
        {

        }

        public void RefreshTerms()
        {
            string SelectCommand = "SELECT * FROM TechCatalogStoreDetailTerms WHERE TechCatalogStoreDetailID = " + iTechCatalogStoreDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TermsDT.Clear();
                DA.Fill(TermsDT);
            }
            FillParameterNames();
        }

        public DataGridViewComboBoxColumn ParameterColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ParameterColumn",
                    HeaderText = "Параметр",
                    DataPropertyName = "Parameter",
                    DataSource = new DataView(ParametersDT),
                    ValueMember = "Parameter",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                return Column;
            }
        }

        public DataGridViewComboBoxColumn LogicOperationsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "LogicOperationsColumn",
                    HeaderText = "Лог. операция",
                    DataPropertyName = "LogicOperation",
                    DataSource = new DataView(LogicOperationsDT),
                    ValueMember = "LogicOperation",
                    DisplayMember = "LogicOperation",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                return Column;
            }
        }

        public int TechCatalogStoreDetailID
        {
            set { iTechCatalogStoreDetailID = value; }
        }

        #region DataSources

        public BindingSource ColorsList
        {
            get { return ColorsBS; }
        }

        public BindingSource CoversList
        {
            get { return CoversBS; }
        }

        public BindingSource InsetColorsList
        {
            get { return InsetColorsBS; }
        }

        public BindingSource InsetTypesList
        {
            get { return InsetTypesBS; }
        }

        public BindingSource MathSymbolsList
        {
            get { return MathSymbolsBS; }
        }

        public BindingSource LogicOperationsList
        {
            get { return LogicOperationsBS; }
        }

        public BindingSource ParametersList
        {
            get { return ParametersBS; }
        }

        public BindingSource PatinaList
        {
            get { return PatinaBS; }
        }

        public BindingSource TermsList
        {
            get { return TermsBS; }
        }

        #endregion
        public void AddMathSymbol(string MathSymbol)
        {
            DataRow NewRow = MathSymbolsDT.NewRow();
            NewRow["MathSymbol"] = MathSymbol;
            MathSymbolsDT.Rows.Add(NewRow);
        }

        public void AddLogicOperation(string LogicOperation)
        {
            DataRow NewRow = LogicOperationsDT.NewRow();
            NewRow["LogicOperation"] = LogicOperation;
            LogicOperationsDT.Rows.Add(NewRow);
        }

        public void AddTerm(string Parameter, string MathSymbol, decimal Term, string LogicOperation)
        {
            DataRow NewRow = TermsDT.NewRow();
            NewRow["TechCatalogStoreDetailID"] = iTechCatalogStoreDetailID;
            NewRow["Parameter"] = Parameter;
            NewRow["MathSymbol"] = MathSymbol;
            NewRow["Term"] = Term;
            NewRow["LogicOperation"] = LogicOperation;
            TermsDT.Rows.Add(NewRow);
        }

        public void ClearTerms()
        {
            TermsDT.Clear();
        }

        public void FillParameterNames()
        {
            int Term = 0;
            string Parameter = string.Empty;
            string ParameterName = string.Empty;
            for (int i = 0; i < TermsDT.Rows.Count; i++)
            {
                Term = Convert.ToInt32(TermsDT.Rows[i]["Term"]);
                Parameter = TermsDT.Rows[i]["Parameter"].ToString();
                switch (Parameter)
                {
                    case "CoverID":
                        DataRow[] rows1 = CoversDT.Select("CoverID = " + Term);
                        if (rows1.Count() > 0)
                            ParameterName = rows1[0]["CoverName"].ToString();
                        break;
                    case "InsetTypeID":
                        DataRow[] rows2 = InsetTypesDT.Select("InsetTypeID = " + Term);
                        if (rows2.Count() > 0)
                            ParameterName = rows2[0]["InsetTypeName"].ToString();
                        break;
                    case "InsetColorID":
                        DataRow[] rows3 = InsetColorsDT.Select("InsetColorID = " + Term);
                        if (rows3.Count() > 0)
                            ParameterName = rows3[0]["InsetColorName"].ToString();
                        break;
                    case "ColorID":
                        DataRow[] rows4 = ColorsDT.Select("ColorID = " + Term);
                        if (rows4.Count() > 0)
                            ParameterName = rows4[0]["ColorName"].ToString();
                        break;
                    case "PatinaID":
                        DataRow[] rows5 = PatinaDT.Select("PatinaID = " + Term);
                        if (rows5.Count() > 0)
                            ParameterName = rows5[0]["PatinaName"].ToString();
                        break;
                    default:
                        ParameterName = Term.ToString();
                        break;
                }
                TermsDT.Rows[i]["TermDisplayName"] = ParameterName;
            }
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            FillParameterNames();
        }

        public bool RemoveTerm()
        {
            if (TermsBS.Current == null)
            {
                return false;
            }
            TermsBS.RemoveCurrent();
            return true;
        }

        public void SaveChangesToTechCatalogOperations()
        {
            string SelectCommand = "SELECT TechCatalogStoreDetailID, IsPerform FROM TechCatalogStoreDetail" +
                " WHERE TechCatalogStoreDetailID = " + iTechCatalogStoreDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (TermsDT.Rows.Count > 0)
                                DT.Rows[0]["IsPerform"] = true;
                            else
                                DT.Rows[0]["IsPerform"] = false;
                            DA.Update(DT);
                        }

                    }
                }
            }
        }

        public void SaveTerms()
        {
            string SelectCommand = "SELECT TOP 0 * FROM TechCatalogStoreDetailTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TermsDT);
                }
            }
        }

        public void UpdateTerms()
        {
            string SelectCommand = "SELECT * FROM TechCatalogStoreDetailTerms WHERE TechCatalogStoreDetailID = " + iTechCatalogStoreDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TermsDT);
            }
        }

        private void AddParameter(string Parameter, string Name)
        {
            DataRow NewRow = ParametersDT.NewRow();
            NewRow["Parameter"] = Parameter;
            NewRow["Name"] = Name;
            ParametersDT.Rows.Add(NewRow);
        }

        private void Binding()
        {
            ColorsBS.DataSource = ColorsDT;
            CoversBS.DataSource = CoversDT;
            InsetColorsBS.DataSource = InsetColorsDT;
            InsetTypesBS.DataSource = InsetTypesDT;
            MathSymbolsBS.DataSource = MathSymbolsDT;
            LogicOperationsBS.DataSource = LogicOperationsDT;
            PatinaBS.DataSource = PatinaDT;
            ParametersBS.DataSource = ParametersDT;
            TermsBS.DataSource = TermsDT;
        }

        private void Create()
        {
            ColorsDT = new DataTable();
            CoversDT = new DataTable();
            InsetColorsDT = new DataTable();
            InsetTypesDT = new DataTable();
            MathSymbolsDT = new DataTable();
            LogicOperationsDT = new DataTable();
            PatinaDT = new DataTable();
            ParametersDT = new DataTable();
            TermsDT = new DataTable();

            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));
            MathSymbolsDT.Columns.Add(new DataColumn("MathSymbol", Type.GetType("System.String")));
            LogicOperationsDT.Columns.Add(new DataColumn("LogicOperation", Type.GetType("System.String")));
            ParametersDT.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            ParametersDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));

            ColorsBS = new BindingSource();
            CoversBS = new BindingSource();
            InsetColorsBS = new BindingSource();
            InsetTypesBS = new BindingSource();
            MathSymbolsBS = new BindingSource();
            LogicOperationsBS = new BindingSource();
            PatinaBS = new BindingSource();
            ParametersBS = new BindingSource();
            TermsBS = new BindingSource();
        }

        private void CreateCoversDT()
        {
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

        private void Fill()
        {
            CreateCoversDT();

            string SelectCommand = "SELECT Colors.*, TechStoreName FROM Colors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore" +
                " ON Colors.ColorID = infiniu2_catalog.dbo.TechStore.TechStoreID" +
                " ORDER BY ColorsGroupID, TechStoreName";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(ColorsDT);
            //    {
            //        DataRow EmptyRow1 = ColorsDT.NewRow();
            //        EmptyRow1["ColorID"] = -1;
            //        EmptyRow1["TechStoreName"] = "-";
            //        ColorsDT.Rows.InsertAt(EmptyRow1, 0);

            //        DataRow ChoiceRow1 = ColorsDT.NewRow();
            //        ChoiceRow1["ColorID"] = 0;
            //        ChoiceRow1["TechStoreName"] = "на выбор";
            //        ColorsDT.Rows.InsertAt(ChoiceRow1, 1);
            //    }
            //}
            GetColorsDT();
            GetInsetColorsDT();
            //SelectCommand = "SELECT * FROM InsetColors ORDER BY InsetColorName";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(InsetColorsDT);
            //}
            SelectCommand = "SELECT * FROM InsetTypes ORDER BY InsetTypeName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDT);
            }
            SelectCommand = "SELECT * FROM Patina ORDER BY PatinaName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDT);
            }
            SelectCommand = "SELECT * FROM TechCatalogStoreDetailTerms WHERE TechCatalogStoreDetailID = " + iTechCatalogStoreDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TermsDT);
                TermsDT.Columns.Add(new DataColumn("TermDisplayName", Type.GetType("System.String")));
            }
            AddMathSymbol("=");
            AddMathSymbol("!=");
            AddMathSymbol("<=");
            AddMathSymbol(">=");
            AddLogicOperation("&&");
            AddLogicOperation("||");
            AddParameter("CoverID", "Облицовка");
            AddParameter("InsetTypeID", "Тип наполнителя");
            AddParameter("InsetColorID", "Цвет наполнителя");
            AddParameter("ColorID", "Цвет");
            AddParameter("PatinaID", "Патина");
            AddParameter("Diameter", "Диаметр");
            AddParameter("Thickness", "Толщина");
            AddParameter("Length", "Длина");
            AddParameter("Height", "Высота");
            AddParameter("Width", "Ширина");
            AddParameter("Admission", "Допуск");
            AddParameter("InsetHeightAdmission", "Допуск на вставку по высоте");
            AddParameter("InsetWidthAdmission", "Допуск на вставку по ширине");
            AddParameter("Capacity", "Емкость");
            AddParameter("Weight", "Вес");
        }

        public string GetColorName(int TechStoreID)
        {
            string ColorName = string.Empty;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore WHERE TechStoreID=" + TechStoreID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    try
                    {
                        DataRow[] Rows = DT.Select("TechStoreID = " + TechStoreID);
                        ColorName = Rows[0]["TechStoreName"].ToString();
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
            return ColorName;
        }

        private void GetColorsDT()
        {
            ColorsDT = new DataTable();
            ColorsDT.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("GroupID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
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
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["GroupID"] = 1;
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["GroupID"] = Convert.ToInt64(DT.Rows[i]["GroupID"]);
                        NewRow["ColorName"] = GetColorName(Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = -1;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "-";
                ColorsDT.Rows.Add(NewRow);
            }
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(ColorsDT))
            {
                DV.Sort = "GroupID, ColorName";
                Table = DV.ToTable();
            }
            ColorsDT.Clear();
            ColorsDT = Table.Copy();
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.InsetColorID, InsetColors.GroupID, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDT);
                {
                    DataRow NewRow = InsetColorsDT.NewRow();
                    NewRow["InsetColorID"] = -1;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "-";
                    InsetColorsDT.Rows.Add(NewRow);
                }
                {
                    DataRow NewRow = InsetColorsDT.NewRow();
                    NewRow["InsetColorID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetColorName"] = "на выбор";
                    InsetColorsDT.Rows.Add(NewRow);
                }

            }

        }
    }

    public class TechStoreItemsManager
    {
        public BindingSource ColorsBS;
        public int CopyTechStoreID = -1;

        public BindingSource CoversBS;
        public BindingSource GroupsBS;
        public BindingSource InsetColorsBS;
        public BindingSource InsetTypesBS;
        public BindingSource ItemsBS;
        public BindingSource NewItemsBS;
        public BindingSource PatinaBS;
        public BindingSource StoreColorsGroupsBS;
        public BindingSource SubGroupsBS;
        DataGridViewComboBoxColumn ColorColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn ColorColumn1 = new DataGridViewComboBoxColumn();
        DataTable ColorsDT;
        DataGridViewComboBoxColumn CoverColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn CoverColumn1 = new DataGridViewComboBoxColumn();
        DataTable CoversDT;
        SqlCommandBuilder GroupsCB;
        SqlDataAdapter GroupsDA;
        PercentageDataGrid GroupsDG;
        DataTable GroupsDT;
        DataGridViewComboBoxColumn InsetColorColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn InsetColorColumn1 = new DataGridViewComboBoxColumn();
        DataTable InsetColorsDT;
        DataGridViewComboBoxColumn InsetTypeColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn InsetTypeColumn1 = new DataGridViewComboBoxColumn();
        DataTable InsetTypesDT;
        SqlCommandBuilder ItemsCB;
        SqlDataAdapter ItemsDA;
        PercentageDataGrid ItemsDG;
        DataTable ItemsDT;
        DataGridViewComboBoxColumn MeasureColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn MeasureColumn1 = new DataGridViewComboBoxColumn();
        DataTable MeasuresDT;
        PercentageDataGrid NewItemsDG;
        DataTable NewItemsDT;
        DataGridViewComboBoxColumn PatinaColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn PatinaColumn1 = new DataGridViewComboBoxColumn();
        DataTable PatinaDT;
        //SqlCommandBuilder StoreColorsGroupsCB;
        //SqlDataAdapter StoreColorsGroupsDA;
        //DataTable StoreColorsGroupsDT;
        SqlCommandBuilder SubGroupsCB;
        SqlDataAdapter SubGroupsDA;
        PercentageDataGrid SubGroupsDG;
        DataTable SubGroupsDT;
        DataGridViewComboBoxColumn TechnoInsetColorColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn TechnoInsetColorColumn1 = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn TechnoInsetTypeColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn TechnoInsetTypeColumn1 = new DataGridViewComboBoxColumn();
        public TechStoreItemsManager(ref PercentageDataGrid tGroupsDataGrid,
            ref PercentageDataGrid tSubGroupsDataGrid,
            ref PercentageDataGrid tItemsDataGrid)
        {
            GroupsDG = tGroupsDataGrid;
            SubGroupsDG = tSubGroupsDataGrid;
            ItemsDG = tItemsDataGrid;


            CreateAndFill();
            Binding();
            GridSettings();
        }

        public int CurrentGroup
        {
            get
            {
                if (GroupsBS.Count == 0 || ((DataRowView)GroupsBS.Current).Row["TechStoreGroupID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)GroupsBS.Current).Row["TechStoreGroupID"]);
            }
        }

        public int CurrentInsetTypeGroup
        {
            get
            {
                if (InsetTypesBS.Count == 0 || ((DataRowView)InsetTypesBS.Current).Row["GroupID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)InsetTypesBS.Current).Row["GroupID"]);
            }
        }

        public int CurrentStoreItem
        {
            get
            {
                if (ItemsBS.Count == 0 || ((DataRowView)ItemsBS.Current).Row["TechStoreID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)ItemsBS.Current).Row["TechStoreID"]);
            }
        }

        public int CurrentSubGroup
        {
            get
            {
                if (SubGroupsBS.Count == 0 || ((DataRowView)SubGroupsBS.Current).Row["TechStoreSubGroupID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)SubGroupsBS.Current).Row["TechStoreSubGroupID"]);
            }
        }
        public string CurrentSubGroupNotes
        {
            get
            {
                if (SubGroupsBS.Count == 0 || ((DataRowView)SubGroupsBS.Current).Row["Notes"] == DBNull.Value)
                    return "";
                else
                    return ((DataRowView)SubGroupsBS.Current).Row["Notes"].ToString();
            }
        }
        public string CurrentSubGroupNotes1
        {
            get
            {
                if (SubGroupsBS.Count == 0 || ((DataRowView)SubGroupsBS.Current).Row["Notes1"] == DBNull.Value)
                    return "";
                else
                    return ((DataRowView)SubGroupsBS.Current).Row["Notes1"].ToString();
            }
        }
        public string CurrentSubGroupNotes2
        {
            get
            {
                if (SubGroupsBS.Count == 0 || ((DataRowView)SubGroupsBS.Current).Row["Notes2"] == DBNull.Value)
                    return "";
                else
                    return ((DataRowView)SubGroupsBS.Current).Row["Notes2"].ToString();
            }
        }

        public void AddInsetColor(int TechStoreID, int GroupID, string TechStoreName)
        {
            DataRow NewRow = InsetColorsDT.NewRow();
            NewRow["InsetColorID"] = TechStoreID;
            NewRow["GroupID"] = GroupID;
            NewRow["InsetColorName"] = TechStoreName;
            InsetColorsDT.Rows.Add(NewRow);
        }

        public void AddInsetType(int TechStoreID, int GroupID, string TechStoreName)
        {
            DataRow NewRow = InsetTypesDT.NewRow();
            NewRow["InsetTypeID"] = TechStoreID;
            NewRow["GroupID"] = GroupID;
            NewRow["InsetTypeName"] = TechStoreName;
            InsetTypesDT.Rows.Add(NewRow);
        }

        public void CopyTechStoreToSubGroup(int TechStoreSubGroupID)
        {
            string SelectCommand = "SELECT * FROM TechStore" +
                " WHERE TechStoreID = " + CopyTechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow.ItemArray = DT.Rows[0].ItemArray;
                            NewRow["TechStoreSubGroupID"] = TechStoreSubGroupID;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void DeleteInsetColor(int TechStoreID, int GroupID)
        {
            DataRow[] Rows = InsetColorsDT.Select("InsetColorID = " + TechStoreID + " AND GroupID = " + GroupID);
            if (Rows.Count() > 0)
            {
                Rows[0].Delete();
            }
        }

        public void DeleteInsetType(int TechStoreID, int GroupID)
        {
            DataRow[] Rows = InsetTypesDT.Select("InsetTypeID = " + TechStoreID + " AND GroupID = " + GroupID);
            if (Rows.Count() > 0)
            {
                Rows[0].Delete();
            }
        }

        public bool EditItem()
        {
            NewItemsDT.Clear();

            if (ItemsBS.Count == 0)
                return false;

            DataRow NewRow = NewItemsDT.NewRow();

            DataRow[] Row = ItemsDT.Select("TechStoreID = " + ((DataRowView)ItemsBS.Current).Row["TechStoreID"]);

            foreach (DataColumn DC in ItemsDT.Columns)
            {
                NewRow[DC.ColumnName] = Row[0][DC.ColumnName];
            }

            NewItemsDT.Rows.Add(NewRow);

            return true;
        }

        public void FilterInsetColors(int GroupID)
        {
            InsetColorsBS.RemoveFilter();
            InsetColorsBS.Filter = "GroupID = -1 OR GroupID = " + GroupID;
        }

        public void FilterStoreItems(int TechStoreSubGroupID)
        {
            ItemsDA.Dispose();
            ItemsCB.Dispose();
            ItemsDT.Clear();
            ItemsDA = new SqlDataAdapter("SELECT * FROM TechStore WHERE TechStoreSubGroupID=" + TechStoreSubGroupID + " ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString);
            ItemsCB = new SqlCommandBuilder(ItemsDA);
            ItemsDA.Fill(ItemsDT);
        }

        public bool IsInsetColorAlreadyExist(int TechStoreID, int GroupID)
        {
            DataRow[] Rows = InsetColorsDT.Select("InsetColorID = " + TechStoreID + " AND GroupID = " + GroupID);
            if (Rows.Count() > 0)
                return true;
            else
                return false;
        }

        public bool IsInsetTypeAlreadyExist(int TechStoreID, int GroupID)
        {
            DataRow[] Rows = InsetTypesDT.Select("InsetTypeID = " + TechStoreID + " AND GroupID = " + GroupID);
            if (Rows.Count() > 0)
                return true;
            else
                return false;
        }

        public void MoveTechStoreToSubGroup(int TechStoreSubGroupID)
        {
            string SelectCommand = "SELECT TechStoreID, TechStoreSubGroupID FROM TechStore" +
                " WHERE TechStoreID = " + CopyTechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["TechStoreSubGroupID"] = TechStoreSubGroupID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }
        public void MoveToStore(int TechStoreID)
        {
            ItemsBS.Position = ItemsBS.Find("TechStoreID", TechStoreID);
        }

        public void MoveToStoreGroup(int TechStoreGroupID)
        {
            GroupsBS.Position = GroupsBS.Find("TechStoreGroupID", TechStoreGroupID);
        }

        public void MoveToStoreSubGroup(int TechStoreSubGroupID)
        {
            SubGroupsBS.Position = SubGroupsBS.Find("TechStoreSubGroupID", TechStoreSubGroupID);
        }

        public void RefreshGroups()
        {
            GroupsDT.Clear();
            GroupsDA.Fill(GroupsDT);
        }

        //public void RefreshStoreColorsGroups()
        //{
        //    StoreColorsGroupsDT.Clear();
        //    StoreColorsGroupsDA.Fill(StoreColorsGroupsDT);
        //}
        public void RefreshStoreItems()
        {
            ItemsDT.Clear();
            ItemsDA.Fill(ItemsDT);
        }

        public void RefreshSubGroups()
        {
            SubGroupsDT.Clear();
            SubGroupsDA.Fill(SubGroupsDT);
        }

        public void SaveEditItems()
        {
            DataRow[] row = ItemsDT.Select("TechStoreID = " + NewItemsDT.Rows[0]["TechStoreID"]);

            foreach (DataColumn dc in ItemsDT.Columns)
            {
                if (dc.ColumnName == "ColorID" || dc.ColumnName == "PatinaID" || dc.ColumnName == "CoverID" || dc.ColumnName == "InsetTypeID" || dc.ColumnName == "InsetColorID")
                {
                    if (NewItemsDT.Rows[0][dc.ColumnName] != DBNull.Value && Convert.ToInt32(NewItemsDT.Rows[0][dc.ColumnName]) == -1)
                    {
                        row[0][dc.ColumnName] = DBNull.Value;
                        continue;
                    }
                }
                row[0][dc.ColumnName] = NewItemsDT.Rows[0][dc.ColumnName];
            }

            ItemsDA.Update(ItemsDT);
        }

        public void SaveChangesToStoreDetail()
        {
            foreach (DataRow row in NewItemsDT.Rows)
            {
                using (var da = new SqlDataAdapter("SELECT TechCatalogStoreDetailID, Length, Height, Width, IsHalfStuff1, IsHalfStuff2 FROM TechCatalogStoreDetail WHERE TechStoreID=" + Convert.ToInt32(row["TechStoreID"]),
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (new SqlCommandBuilder(da))
                    {
                        using (var dt = new DataTable())
                        {
                            da.Fill(dt);
                            for (var i = 0; i < dt.Rows.Count; i++)
                            {
                                if (row["IsHalfStuff"] != DBNull.Value)
                                    dt.Rows[i]["IsHalfStuff1"] = row["IsHalfStuff"];
                                if (row["IsHalfStuff"] != DBNull.Value)
                                    dt.Rows[i]["IsHalfStuff2"] = row["IsHalfStuff"];
                                if (row["Length"] != DBNull.Value)
                                    dt.Rows[i]["Length"] = row["Length"];
                                if (row["Height"] != DBNull.Value)
                                    dt.Rows[i]["Height"] = row["Height"];
                                if (row["Width"] != DBNull.Value)
                                    dt.Rows[i]["Width"] = row["Width"];
                            }
                            da.Update(dt);
                        }
                    }
                }
            }
        }
        public void SaveInsetColors()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {

                    if (InsetColorsDT.GetChanges() != null)
                    {
                        DataTable D = InsetColorsDT.GetChanges();
                        DA.Update(InsetColorsDT);
                    }
                    TechCatalogEvents.SaveEvents("Склад: цвета вставок сохранены");
                    InsetColorsDT.Clear();
                    DA.Fill(InsetColorsDT);
                }
            }
        }

        public void SaveInsetTypes()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(InsetTypesDT);
                    TechCatalogEvents.SaveEvents("Склад: типы вставок сохранены");
                    InsetTypesDT.Clear();
                    DA.Fill(InsetTypesDT);
                }
            }
        }

        public void SaveNewItems()
        {
            foreach (DataRow Row in NewItemsDT.Rows)
            {
                Row["TechStoreSubGroupID"] = ((DataRowView)SubGroupsBS.Current).Row["TechStoreSubGroupID"];
                ItemsDT.ImportRow(Row);
            }

            ItemsDA.Update(ItemsDT);
            ItemsDT.Clear();
            ItemsDA.Fill(ItemsDT);

            NewItemsDT.Clear();
        }

        public void SetNewItemGrid(ref PercentageDataGrid tNewItemsDataGrid)
        {
            NewItemsDG = tNewItemsDataGrid;
            NewItemsDG.DataSource = NewItemsBS;

            MeasureColumn = new DataGridViewComboBoxColumn()
            {
                Name = "MeasureColumn",
                HeaderText = "Ед. измерения",
                DataPropertyName = "MeasureID",
                DataSource = new DataView(MeasuresDT),
                ValueMember = "MeasureID",
                DisplayMember = "Measure",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ColorColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ColorColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",
                DataSource = ColorsBS,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            CoverColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CoverColumn",
                HeaderText = "Облицовка",
                DataPropertyName = "CoverID",
                DataSource = CoversBS,
                ValueMember = "CoverID",
                DisplayMember = "CoverName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = PatinaBS,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypeColumn",
                HeaderText = "Тип наполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = new DataView(InsetTypesDT),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorColumn = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorColumn",
                HeaderText = "Цвет наполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = new DataView(InsetColorsDT),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypeColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypeColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(InsetTypesDT),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorColumn = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(InsetColorsDT),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            NewItemsDG.Columns.Add(MeasureColumn);
            NewItemsDG.Columns.Add(ColorColumn);
            NewItemsDG.Columns.Add(PatinaColumn);
            NewItemsDG.Columns.Add(CoverColumn);
            NewItemsDG.Columns.Add(InsetTypeColumn);
            NewItemsDG.Columns.Add(InsetColorColumn);
            NewItemsDG.Columns.Add(TechnoInsetTypeColumn);
            NewItemsDG.Columns.Add(TechnoInsetColorColumn);

            NewItemsDG.Columns["TechStoreSubGroupID"].Visible = false;
            NewItemsDG.Columns["TechStoreID"].Visible = false;
            NewItemsDG.Columns["MeasureID"].Visible = false;
            NewItemsDG.Columns["ColorID"].Visible = false;
            NewItemsDG.Columns["CoverID"].Visible = false;
            NewItemsDG.Columns["PatinaID"].Visible = false;
            NewItemsDG.Columns["InsetTypeID"].Visible = false;
            NewItemsDG.Columns["InsetColorID"].Visible = false;
            NewItemsDG.Columns["TechnoInsetTypeID"].Visible = false;
            NewItemsDG.Columns["TechnoInsetColorID"].Visible = false;

            NewItemsDG.Columns["IsHalfStuff"].HeaderText = "п/ф";
            NewItemsDG.Columns["InvNumber1S"].HeaderText = "Инв.№ 1С";
            NewItemsDG.Columns["TechStoreName"].HeaderText = "Название";
            NewItemsDG.Columns["SellerCode"].HeaderText = "Кодировка поставщика";
            NewItemsDG.Columns["LeftAngle"].HeaderText = "ᵒ∠ л";
            NewItemsDG.Columns["RightAngle"].HeaderText = "ᵒ∠ пр";
            NewItemsDG.Columns["Length"].HeaderText = "Длина, мм";
            NewItemsDG.Columns["Width"].HeaderText = "Ширина, мм";
            NewItemsDG.Columns["WidthMin"].HeaderText = "Ширина min, мм";
            NewItemsDG.Columns["WidthMax"].HeaderText = "Ширина max, мм";
            NewItemsDG.Columns["Height"].HeaderText = "Высота, мм";
            NewItemsDG.Columns["HeightMin"].HeaderText = "Высота min, мм";
            NewItemsDG.Columns["HeightMax"].HeaderText = "Высота max, мм";
            NewItemsDG.Columns["Thickness"].HeaderText = "Толщина, мм";
            NewItemsDG.Columns["Diameter"].HeaderText = "Диаметр, мм";
            NewItemsDG.Columns["Admission"].HeaderText = "Допуск, мм";
            NewItemsDG.Columns["InsetHeightAdmission"].HeaderText = "Допуск на вставку\r\n   по высоте, мм";
            NewItemsDG.Columns["InsetWidthAdmission"].HeaderText = "Допуск на вставку\r\n   по ширине, мм";
            NewItemsDG.Columns["Weight"].HeaderText = "Вес, кг";
            NewItemsDG.Columns["Capacity"].HeaderText = "Емкость, л";
            NewItemsDG.Columns["Notes"].HeaderText = "Примечание";
            //65f2a94c8c2d56d5b43a1a3d9d811102
            NewItemsDG.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            NewItemsDG.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["MeasureColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["IsHalfStuff"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["InvNumber1S"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["CoverColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["InsetTypeColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["InsetColorColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["TechnoInsetTypeColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["TechnoInsetColorColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["ColorColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["LeftAngle"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Length"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["RightAngle"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Height"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["HeightMin"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["HeightMax"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Width"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["WidthMin"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["WidthMax"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Admission"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["InsetHeightAdmission"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["InsetWidthAdmission"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Weight"].DisplayIndex = DisplayIndex++;
            NewItemsDG.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        private void Binding()
        {
            //StoreColorsGroupsBS = new BindingSource();
            //StoreColorsGroupsBS.DataSource = StoreColorsGroupsDT;

            GroupsBS = new BindingSource()
            {
                DataSource = GroupsDT
            };
            GroupsDG.DataSource = GroupsBS;

            SubGroupsBS = new BindingSource()
            {
                DataSource = SubGroupsDT
            };
            SubGroupsDG.DataSource = SubGroupsBS;

            ItemsBS = new BindingSource()
            {
                DataSource = ItemsDT
            };
            ItemsDG.DataSource = ItemsBS;

            NewItemsBS = new BindingSource()
            {
                DataSource = NewItemsDT
            };
            ColorsBS = new BindingSource()
            {
                DataSource = ColorsDT
            };
            CoversBS = new BindingSource()
            {
                DataSource = CoversDT
            };
            PatinaBS = new BindingSource()
            {
                DataSource = PatinaDT
            };
            InsetTypesBS = new BindingSource()
            {
                DataSource = InsetTypesDT
            };
            InsetColorsBS = new BindingSource()
            {
                DataSource = InsetColorsDT
            };
        }

        private void CreateAndFill()
        {
            //StoreColorsGroupsDT = new DataTable();
            //StoreColorsGroupsDA = new SqlDataAdapter("SELECT * FROM ColorsGroups ORDER BY Name", ConnectionStrings.CatalogConnectionString);
            //StoreColorsGroupsCB = new SqlCommandBuilder(StoreColorsGroupsDA);
            //StoreColorsGroupsDA.Fill(StoreColorsGroupsDT);

            GroupsDT = new DataTable();
            GroupsDA = new SqlDataAdapter("SELECT * FROM TechStoreGroups ORDER BY TechStoreGroupName", ConnectionStrings.CatalogConnectionString);
            GroupsCB = new SqlCommandBuilder(GroupsDA);
            GroupsDA.Fill(GroupsDT);

            SubGroupsDT = new DataTable();
            SubGroupsDA = new SqlDataAdapter("SELECT * FROM TechStoreSubGroups ORDER BY TechStoreSubGroupName", ConnectionStrings.CatalogConnectionString);
            SubGroupsCB = new SqlCommandBuilder(SubGroupsDA);
            SubGroupsDA.Fill(SubGroupsDT);

            ItemsDT = new DataTable();
            ItemsDA = new SqlDataAdapter("SELECT TOP 0 * FROM TechStore ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString);
            ItemsCB = new SqlCommandBuilder(ItemsDA);
            ItemsDA.Fill(ItemsDT);

            MeasuresDT = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Measures", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDT);
            }

            //ColorsDT = new DataTable();
            //StoreColorsDT = new DataTable();
            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Colors.*, TechStoreName FROM Colors" +
            //    " INNER JOIN infiniu2_catalog.dbo.TechStore" +
            //    " ON Colors.ColorID = infiniu2_catalog.dbo.TechStore.TechStoreID" +
            //    " ORDER BY ColorsGroupID, TechStoreName", ConnectionStrings.StorageConnectionString))
            //{
            //    DA.Fill(StoreColorsDT);
            //    DA.Fill(ColorsDT);
            //    {
            //        DataRow EmptyRow1 = ColorsDT.NewRow();
            //        EmptyRow1["ColorID"] = -1;
            //        EmptyRow1["TechStoreName"] = "-";
            //        ColorsDT.Rows.InsertAt(EmptyRow1, 0);

            //        DataRow ChoiceRow1 = ColorsDT.NewRow();
            //        ChoiceRow1["ColorID"] = 0;
            //        ChoiceRow1["TechStoreName"] = "на выбор";
            //        ColorsDT.Rows.InsertAt(ChoiceRow1, 1);
            //    }
            //}

            PatinaDT = new DataTable();
            //InsetColorsDT = new DataTable();
            InsetTypesDT = new DataTable();

            GetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDT);
            }
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes ORDER BY InsetTypeName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDT);
            }

            NewItemsDT = new DataTable();
            NewItemsDT = ItemsDT.Clone();

            CreateCoversDT();
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

        public string GetColorName(int TechStoreID)
        {
            string ColorName = string.Empty;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore WHERE TechStoreID=" + TechStoreID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    try
                    {
                        DataRow[] Rows = DT.Select("TechStoreID = " + TechStoreID);
                        ColorName = Rows[0]["TechStoreName"].ToString();
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
            return ColorName;
        }

        private void GetColorsDT()
        {
            ColorsDT = new DataTable();
            ColorsDT.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("GroupID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
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
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["GroupID"] = 1;
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE InsetColorID <> 0 AND InsetColorID <> -1", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["GroupID"] = Convert.ToInt64(DT.Rows[i]["GroupID"]);
                        NewRow["ColorName"] = GetColorName(Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = -1;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "-";
                ColorsDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = 0;
                NewRow["ColorName"] = "на выбор";
                NewRow["GroupID"] = 0;
                ColorsDT.Rows.InsertAt(NewRow, 0);
            }
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(ColorsDT))
            {
                DV.Sort = "GroupID, ColorName";
                Table = DV.ToTable();
            }
            ColorsDT.Clear();
            ColorsDT = Table.Copy();
        }

        private void GetInsetColorsDT()
        {
            InsetColorsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT InsetColors.*, infiniu2_catalog.dbo.TechStore.TechStoreName AS InsetColorName FROM InsetColors" +
                " INNER JOIN infiniu2_catalog.dbo.TechStore ON InsetColors.InsetColorID = infiniu2_catalog.dbo.TechStore.TechStoreID ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetColorsDT);
                //{
                //    DataRow NewRow = InsetColorsDT.NewRow();
                //    NewRow["InsetColorID"] = -1;
                //    NewRow["GroupID"] = -1;
                //    NewRow["InsetColorName"] = "-";
                //    InsetColorsDT.Rows.Add(NewRow);
                //}
                //{
                //    DataRow NewRow = InsetColorsDT.NewRow();
                //    NewRow["InsetColorID"] = 0;
                //    NewRow["GroupID"] = -1;
                //    NewRow["InsetColorName"] = "на выбор";
                //    InsetColorsDT.Rows.Add(NewRow);
                //}
            }

        }
        private void GridSettings()
        {
            GroupsDG.Columns["TechStoreGroupID"].Visible = false;
            GroupsDG.Columns["TechStoreGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            SubGroupsDG.Columns["TechStoreSubGroupID"].Visible = false;
            SubGroupsDG.Columns["TechStoreGroupID"].Visible = false;
            SubGroupsDG.Columns["TechStoreSubGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SubGroupsDG.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SubGroupsDG.Columns["Notes1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SubGroupsDG.Columns["Notes2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            MeasureColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "MeasureColumn",
                HeaderText = "Ед. измерения",
                DataPropertyName = "MeasureID",
                DataSource = new DataView(MeasuresDT),
                ValueMember = "MeasureID",
                DisplayMember = "Measure",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ColorColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "ColorColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",
                DataSource = new DataView(ColorsDT),
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            CoverColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "CoverColumn",
                HeaderText = "Облицовка",
                DataPropertyName = "CoverID",
                DataSource = new DataView(CoversDT),
                ValueMember = "CoverID",
                DisplayMember = "CoverName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = new DataView(PatinaDT),
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetTypeColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "InsetTypeColumn",
                HeaderText = "Тип наполнителя",
                DataPropertyName = "InsetTypeID",
                DataSource = InsetTypesBS,
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            InsetColorColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "InsetColorColumn",
                HeaderText = "Цвет наполнителя",
                DataPropertyName = "InsetColorID",
                DataSource = InsetColorsBS,
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetTypeColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetTypeColumn",
                HeaderText = "Тип наполнителя-2",
                DataPropertyName = "TechnoInsetTypeID",
                DataSource = new DataView(InsetTypesDT),
                ValueMember = "InsetTypeID",
                DisplayMember = "InsetTypeName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            TechnoInsetColorColumn1 = new DataGridViewComboBoxColumn()
            {
                Name = "TechnoInsetColorColumn",
                HeaderText = "Цвет наполнителя-2",
                DataPropertyName = "TechnoInsetColorID",
                DataSource = new DataView(InsetColorsDT),
                ValueMember = "InsetColorID",
                DisplayMember = "InsetColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ItemsDG.Columns.Add(MeasureColumn1);
            ItemsDG.Columns.Add(ColorColumn1);
            ItemsDG.Columns.Add(PatinaColumn1);
            ItemsDG.Columns.Add(CoverColumn1);
            ItemsDG.Columns.Add(InsetTypeColumn1);
            ItemsDG.Columns.Add(InsetColorColumn1);
            ItemsDG.Columns.Add(TechnoInsetTypeColumn1);
            ItemsDG.Columns.Add(TechnoInsetColorColumn1);

            foreach (DataGridViewColumn Column in ItemsDG.Columns)
            {
                Column.ReadOnly = true;
            }

            ItemsDG.Columns["TechStoreSubGroupID"].Visible = false;
            //ItemsDG.Columns["TechStoreID"].Visible = false;
            ItemsDG.Columns["MeasureID"].Visible = false;
            ItemsDG.Columns["ColorID"].Visible = false;
            ItemsDG.Columns["CoverID"].Visible = false;
            ItemsDG.Columns["PatinaID"].Visible = false;
            ItemsDG.Columns["InsetTypeID"].Visible = false;
            ItemsDG.Columns["InsetColorID"].Visible = false;
            ItemsDG.Columns["TechnoInsetTypeID"].Visible = false;
            ItemsDG.Columns["TechnoInsetColorID"].Visible = false;

            ItemsDG.Columns["IsHalfStuff"].HeaderText = "п/ф";
            ItemsDG.Columns["InvNumber1S"].HeaderText = "Инв.№ 1С";
            ItemsDG.Columns["TechStoreName"].HeaderText = "Название";
            ItemsDG.Columns["SellerCode"].HeaderText = "Кодировка поставщика";
            ItemsDG.Columns["LeftAngle"].HeaderText = "ᵒ∠ л";
            ItemsDG.Columns["RightAngle"].HeaderText = "ᵒ∠ пр";
            ItemsDG.Columns["Length"].HeaderText = "Длина, мм";
            ItemsDG.Columns["Width"].HeaderText = "Ширина, мм";
            ItemsDG.Columns["WidthMin"].HeaderText = "Ширина min, мм";
            ItemsDG.Columns["WidthMax"].HeaderText = "Ширина max, мм";
            ItemsDG.Columns["Height"].HeaderText = "Высота, мм";
            ItemsDG.Columns["HeightMin"].HeaderText = "Высота min, мм";
            ItemsDG.Columns["HeightMax"].HeaderText = "Высота max, мм";
            ItemsDG.Columns["Thickness"].HeaderText = "Толщина, мм";
            ItemsDG.Columns["Diameter"].HeaderText = "Диаметр, мм";
            ItemsDG.Columns["Admission"].HeaderText = "Допуск, мм";
            ItemsDG.Columns["InsetHeightAdmission"].HeaderText = "Допуск на вставку\r\n   по высоте, мм";
            ItemsDG.Columns["InsetWidthAdmission"].HeaderText = "Допуск на вставку\r\n   по ширине, мм";
            ItemsDG.Columns["Weight"].HeaderText = "Вес, кг";
            ItemsDG.Columns["Capacity"].HeaderText = "Емкость, л";
            ItemsDG.Columns["Notes"].HeaderText = "Примечание";

            ItemsDG.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            ItemsDG.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["SellerCode"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["MeasureColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["IsHalfStuff"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["InvNumber1S"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["CoverColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["InsetTypeColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["InsetColorColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["TechnoInsetTypeColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["TechnoInsetColorColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["ColorColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["LeftAngle"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Length"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["RightAngle"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Height"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["HeightMin"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["HeightMax"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Width"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["WidthMin"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["WidthMax"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Admission"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["InsetHeightAdmission"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["InsetWidthAdmission"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Capacity"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Weight"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["Notes"].DisplayIndex = DisplayIndex++;
            ItemsDG.Columns["TechStoreID"].DisplayIndex = DisplayIndex++;
        }
    }

    public class TechStoreManager
    {
        public BindingSource AllMachinesBS;
        public DataTable CurrentMachineParametrsDT;
        //public DataTable TechStoreNamesDT;
        //public DataTable TechSubGroupsNamesDT;
        public DataTable CurrentParametrsDT;

        public FileManager FM = new FileManager();
        public BindingSource MachinesBS;
        public DataTable MachinesDT;
        public BindingSource MachinesOperationsBS;
        public DataTable MachinesOperationsDT;
        public DataTable MeasuresDT;
        public DataTable CabFurnitureDocumentTypesDT;
        public DataTable CabFurnitureAlgorithmsDT;
        public DataTable OperationsDocumentsDT;

        public BindingSource CabFurnitureDocumentTypesBS;
        public BindingSource OperationsOnMachineBS;
        public BindingSource PositionsBS;
        public DataTable PositionsDT;
        public BindingSource SectorsBS;
        public DataTable SectorsDT;
        public BindingSource SubSectorsBS;
        public DataTable SubSectorsDT;
        public BindingSource TechCatalogOperationsDetailBS;
        public DataTable TechCatalogOperationsDetailDT;
        public BindingSource TechCatalogOperationsDetailGroupsBS;
        public DataTable TechCatalogOperationsDetailGroupsDT;
        public BindingSource TechCatalogStoreDetailBS;
        public DataTable TechCatalogStoreDetailDT;
        public BindingSource TechCatalogToolsBS;
        public DataTable TechCatalogToolsDT;
        public DataTable TechSectorNamesDT;
        public BindingSource TechStoreBS;
        public DataTable TechStoreDocumentsDT;
        public DataTable TechStoreDT;
        public BindingSource TechStoreGroupsBS;
        public DataTable TechStoreGroupsDT;
        public DataTable TechStoreNamesDT;
        public BindingSource TechStoreSubGroupsBS;
        public DataTable TechStoreSubGroupsDT;
        public BindingSource ToolsBS;
        public DataTable ToolsDocumentsDT;
        public DataTable ToolsDT;
        public DataTable ToolsGroupDT;
        public BindingSource ToolsGroupsBS;
        public DataTable ToolsSubTypeDT;
        public BindingSource ToolsSubTypesBS;
        public DataTable ToolsTypeDT;
        public BindingSource ToolsTypesBS;
        static DataTable DecorConfigDT = null;
        static DataTable FrontsConfigDT = null;
        DataTable ManufactureDT;
        int techStoreGroupID = 0;
        int techStoreID = 0;
        int techStoreSubGroupID = 0;

        DataTable dtRolePermissions;

        public void GetPermissions(int UserID, string FormName)
        {
            if (dtRolePermissions == null)
                dtRolePermissions = new DataTable();
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

        public TechStoreManager()
        {
        }

        public BindingSource AllMachinesList
        {
            get { return AllMachinesBS; }
        }

        public DataTable CurrentMachineParametrsList
        {
            get { return CurrentMachineParametrsDT; }
        }

        public DataTable CurrentParametrsList
        {
            get { return CurrentParametrsDT; }
        }

        public int CurrentTechStoreGroupID
        {
            get { return techStoreGroupID; }
            set { techStoreGroupID = value; }
        }

        public int CurrentTechStoreID
        {
            get { return techStoreID; }
            set { techStoreID = value; }
        }

        public int CurrentTechStoreSubGroupID
        {
            get { return techStoreSubGroupID; }
            set { techStoreSubGroupID = value; }
        }

        public bool HasTechCatalogOperationsDetails
        {
            get { return TechCatalogOperationsDetailBS.Count > 0; }
        }

        public bool HasTechStoreGroups
        {
            get { return TechStoreGroupsBS.Count > 0; }
        }

        public bool HasTechStoreSubGroups
        {
            get { return TechStoreSubGroupsBS.Count > 0; }
        }

        public DataGridViewComboBoxColumn MachineNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MachineName",
                    HeaderText = "Станок",
                    DataSource = new DataView(MachinesDT),
                    DisplayMember = "MachineName",
                    ValueMember = "MachineID",
                    DataPropertyName = "MachineID",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public BindingSource MachinesList
        {
            get { return MachinesBS; }
        }

        public DataGridViewComboBoxColumn MachinesOperationArticleColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MachinesOperationArticle",
                    HeaderText = "Артикул",
                    DataSource = new DataView(MachinesOperationsDT),
                    DisplayMember = "Article",
                    ValueMember = "MachinesOperationID",
                    DataPropertyName = "MachinesOperationID",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 80
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn MachinesOperationNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MachinesOperationName",
                    MinimumWidth = 250,
                    HeaderText = "Операция",
                    DataSource = new DataView(MachinesOperationsDT),
                    DisplayMember = "MachinesOperationName",
                    ValueMember = "MachinesOperationID",
                    DataPropertyName = "MachinesOperationID",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public BindingSource MachinesOperationsList
        {
            get { return MachinesOperationsBS; }
        }

        public DataGridViewComboBoxColumn MeasureColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Ед. изм.",
                    DataSource = MeasuresDT,
                    DisplayMember = "Measure",
                    ValueMember = "MeasureID",
                    DataPropertyName = "MeasureID",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 80,
                    DisplayIndex = 3
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn OperationsOnMachineMeasureColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Ед. изм.",
                    DataSource = MeasuresDT,
                    DisplayMember = "Measure",
                    ValueMember = "MeasureID",
                    DataPropertyName = "MeasureID",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 80,
                    DisplayIndex = 3
                };
                return Column;
            }
        }

        public BindingSource PositionsList
        {
            get
            {
                return new BindingSource
                {
                    DataSource = PositionsDT,
                    Sort = "Position, TariffRank"
                };
            }
        }

        public BindingSource DocTypesList
        {
            get
            {
                BindingSource bs = new BindingSource()
                {
                    DataSource = CabFurnitureDocumentTypesDT
                };
                return bs;
            }
        }

        public BindingSource AlgorithmsList
        {
            get
            {
                BindingSource bs = new BindingSource()
                {
                    DataSource = CabFurnitureAlgorithmsDT
                };
                return bs;
            }
        }

        public DataGridViewComboBoxColumn SectorNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "SectorName",
                    HeaderText = "Участок",
                    DataSource = new DataView(SectorsDT),
                    DisplayMember = "SectorName",
                    ValueMember = "SectorID",
                    DataPropertyName = "SectorID",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public BindingSource SectorsList
        {
            get { return SectorsBS; }
        }

        public DataGridViewComboBoxColumn DocTypesColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "DocTypesColumn",
                    HeaderText = "Тип задания",
                    DataPropertyName = "CabFurDocTypeID",
                    DataSource = new DataView(CabFurnitureDocumentTypesDT),
                    ValueMember = "CabFurDocTypeID",
                    DisplayMember = "DocName",
                    ReadOnly = true,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn AlgorithmsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "AlgorithmsColumn",
                    HeaderText = "Алгоритм",
                    DataPropertyName = "CabFurAlgorithmID",
                    DataSource = new DataView(CabFurnitureAlgorithmsDT),
                    ValueMember = "CabFurAlgorithmID",
                    DisplayMember = "Algorithm",
                    ReadOnly = true,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn SubSectorNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "SubSectorName",
                    HeaderText = "Подучасток",
                    DataSource = new DataView(SubSectorsDT),
                    DisplayMember = "SubSectorName",
                    ValueMember = "SubSectorID",
                    DataPropertyName = "SubSectorID",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public BindingSource CabFurnitureDocumentTypesList
        {
            get { return CabFurnitureDocumentTypesBS; }
        }

        public BindingSource SubSectorsList
        {
            get { return SubSectorsBS; }
        }

        public BindingSource TechCatalogOperationsDetailGroupsList
        {
            get { return TechCatalogOperationsDetailGroupsBS; }
        }

        public BindingSource TechCatalogOperationsDetailList
        {
            get { return TechCatalogOperationsDetailBS; }
        }

        public BindingSource TechCatalogStoreDetailList
        {
            get { return TechCatalogStoreDetailBS; }
        }

        public BindingSource TechCatalogToolsList
        {
            get { return TechCatalogToolsBS; }
        }

        public BindingSource TechStoreGroupsList
        {
            get { return TechStoreGroupsBS; }
        }

        public BindingSource TechStoreList
        {
            get { return TechStoreBS; }
        }

        public BindingSource TechStoreSubGroupsList
        {
            get { return TechStoreSubGroupsBS; }
        }

        public BindingSource ToolsGroupsList
        {
            get { return ToolsGroupsBS; }
        }

        public BindingSource ToolsList
        {
            get { return ToolsBS; }
        }

        public DataGridViewComboBoxColumn ToolsNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ToolsName",
                    HeaderText = "Операция",
                    DataSource = new DataView(ToolsDT),
                    DisplayMember = "ToolsName",
                    ValueMember = "ToolsID",
                    DataPropertyName = "ToolsID",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public BindingSource ToolsSubTypesList
        {
            get { return ToolsSubTypesBS; }
        }

        public BindingSource ToolsTypesList
        {
            get { return ToolsTypesBS; }
        }

        public static void DataSetIntoDBF()
        {
            //            string ssssss = @"SELECT        dbo.TechStoreGroups.TechStoreGroupName, dbo.TechStoreSubGroups.TechStoreSubGroupName, dbo.TechStore.TechStoreName, dbo.Measures.Measure
            //FROM            dbo.TechStore INNER JOIN
            //                         dbo.Measures ON dbo.TechStore.MeasureID = dbo.Measures.MeasureID INNER JOIN
            //                         dbo.TechStoreSubGroups ON dbo.TechStore.TechStoreSubGroupID = dbo.TechStoreSubGroups.TechStoreSubGroupID INNER JOIN
            //                         dbo.TechStoreGroups ON dbo.TechStoreSubGroups.TechStoreGroupID = dbo.TechStoreGroups.TechStoreGroupID
            //ORDER BY dbo.TechStoreGroups.TechStoreGroupName, dbo.TechStoreSubGroups.TechStoreSubGroupName, dbo.TechStore.TechStoreName, dbo.Measures.Measure";
            string fileName = "Catalog";
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium";
            FileInfo f = new FileInfo(filePath + @"\" + fileName + ".DBF");
            //int x = 1;
            //while (f.Exists == true)
            //    f = new FileInfo(filePath + @"\" + fileName + "(" + x++ + ").DBF");
            fileName = f.FullName;

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("TechStoreGroupName"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("TechStoreSubGroupName"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("TechStoreName"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i]["TechStoreGroupName"] = convertDefaultToDos(table.Rows[i]["TechStoreGroupName"].ToString());
                table.Rows[i]["TechStoreSubGroupName"] = convertDefaultToDos(table.Rows[i]["TechStoreSubGroupName"].ToString());
                table.Rows[i]["TechStoreName"] = convertDefaultToDos(table.Rows[i]["TechStoreName"].ToString());
                table.Rows[i]["Measure"] = convertDefaultToDos(table.Rows[i]["Measure"].ToString());
            }

            if (File.Exists(filePath + fileName + ".dbf"))
            {
                File.Delete(filePath + fileName + ".dbf");
            }

            string createSql = "create table " + fileName +
                " ([TechStoreGroupName] varchar(50), [TechStoreSubGroupName] varchar(50), [TechStoreName] varchar(100), [Measure] varchar(20))";

            OleDbConnection con = new OleDbConnection(GetConnection(filePath));

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con
            };
            con.Open();

            cmd.CommandText = createSql;

            cmd.ExecuteNonQuery();

            foreach (DataRow row in table.Rows)
            {
                string insertSql = "insert into " + fileName +
                    " (TechStoreGroupName, TechStoreSubGroupName, TechStoreName, Measure) values(TechStoreGroupName, TechStoreSubGroupName, TechStoreName, Measure)";
                string TechStoreGroupName = row["TechStoreGroupName"].ToString();
                string TechStoreSubGroupName = row["TechStoreSubGroupName"].ToString();
                string TechStoreName = row["TechStoreName"].ToString();
                string Measure = row["Measure"].ToString();
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("TechStoreGroupName", OleDbType.VarChar).Value = TechStoreGroupName;
                cmd.Parameters.Add("TechStoreSubGroupName", OleDbType.VarChar).Value = TechStoreSubGroupName;
                cmd.Parameters.Add("TechStoreName", OleDbType.VarChar).Value = TechStoreName;
                cmd.Parameters.Add("Measure", OleDbType.VarChar).Value = Measure;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        public static void DataSetIntoDBF11(DataTable table, string filePath, string fileName)
        {

            if (File.Exists(filePath + fileName + ".dbf"))
            {
                File.Delete(filePath + fileName + ".dbf");
            }

            string createSql = "create table " + fileName + " ([UNN] varchar(20), [CurrencyCode] varchar(20), [TPSCurCode] varchar(20), [InvNumber] varchar(20), [Amount] Double, [Price] Double, [Weight] Double, [PackageCount] Integer)";

            OleDbConnection con = new OleDbConnection(GetConnection(filePath));

            OleDbCommand cmd = new OleDbCommand()
            {
                Connection = con
            };
            con.Open();

            cmd.CommandText = createSql;

            cmd.ExecuteNonQuery();

            foreach (DataRow row in table.Rows)
            {
                string insertSql = "insert into " + fileName +
                    " (UNN, CurrencyCode, TPSCurCode, InvNumber, Amount, Price, Weight, PackageCount) values(UNN, CurrencyCode, TPSCurCode, InvNumber, Amount, Price, Weight, PackageCount)";

                double Amount = Convert.ToDouble(row["Amount"]);
                double Price = Convert.ToDouble(row["Price"]);
                string InvNumber = row["InvNumber"].ToString();
                string CurrencyCode = row["CurrencyCode"].ToString();
                //string TPSCurCode = row["TPSCurCode"].ToString();
                string UNN = row["UNN"].ToString();
                cmd.CommandText = insertSql;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("UNN", OleDbType.VarChar).Value = UNN;
                cmd.Parameters.Add("CurrencyCode", OleDbType.VarChar).Value = CurrencyCode;
                cmd.Parameters.Add("TPSCurCode", OleDbType.VarChar).Value = DBNull.Value;
                cmd.Parameters.Add("InvNumber", OleDbType.VarChar).Value = InvNumber;
                cmd.Parameters.Add("Amount", OleDbType.Double).Value = Amount;
                cmd.Parameters.Add("Price", OleDbType.Double).Value = Price;
                cmd.Parameters.Add("Weight", OleDbType.Double).Value = DBNull.Value;
                cmd.Parameters.Add("PackageCount", OleDbType.Integer).Value = DBNull.Value;
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }

        static public void SSS()
        {
            DecorConfigDT = new DataTable();
            FrontsConfigDT = new DataTable();
            string SelectCommand = "SELECT DecorConfigID, InvNumber1, InvNumber FROM DecorConfig";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorConfigDT);
            }
            SelectCommand = "SELECT FrontConfigID, InvNumber1, InvNumber FROM FrontsConfig";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsConfigDT);
            }

            string fileName = "Catalog";
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium";
            FileInfo f = new FileInfo(filePath + @"\" + fileName + ".DBF");
            fileName = f.FullName;

            DataTable table = new DataTable();
            DataTable table1 = new DataTable();
            table.Columns.Add(new DataColumn(("ID"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("UNN"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("CurrencyCode"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("TPSCurCode"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("InvNumber"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Amount"), System.Type.GetType("System.Decimal")));
            table.Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.Decimal")));
            table.Columns.Add(new DataColumn(("Weight"), System.Type.GetType("System.Decimal")));
            table.Columns.Add(new DataColumn(("PackageCount"), System.Type.GetType("System.Int32")));
            table.TableName = "ImportedTable";

            table1.Columns.Add(new DataColumn(("UNN"), System.Type.GetType("System.String")));
            table1.Columns.Add(new DataColumn(("CurrencyCode"), System.Type.GetType("System.String")));
            table1.Columns.Add(new DataColumn(("TPSCurCode"), System.Type.GetType("System.String")));
            table1.Columns.Add(new DataColumn(("InvNumber"), System.Type.GetType("System.String")));
            table1.Columns.Add(new DataColumn(("Amount"), System.Type.GetType("System.Decimal")));
            table1.Columns.Add(new DataColumn(("Price"), System.Type.GetType("System.Decimal")));
            table1.Columns.Add(new DataColumn(("Weight"), System.Type.GetType("System.Decimal")));
            table1.Columns.Add(new DataColumn(("PackageCount"), System.Type.GetType("System.Int32")));
            table1.TableName = "ImportedTable1";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }
            fileName = table.Rows[0]["ID"].ToString();
            DataRow NewRow = null;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (i < table.Rows.Count - 1)
                {
                    if (table.Rows[i]["UNN"] == DBNull.Value)
                    {
                        fileName = table.Rows[i]["ID"].ToString();
                        continue;
                    }
                    if (table.Rows[i + 1]["UNN"] == DBNull.Value)
                    {
                        NewRow = table1.NewRow();
                        DataRow[] rows = DecorConfigDT.Select("InvNumber='" + table.Rows[i]["InvNumber"].ToString() + "'");
                        if (rows.Count() > 0)
                            NewRow["InvNumber"] = rows[0]["InvNumber1"];
                        else
                        {
                            rows = FrontsConfigDT.Select("InvNumber='" + table.Rows[i]["InvNumber"].ToString() + "'");
                            if (rows.Count() > 0)
                                NewRow["InvNumber"] = rows[0]["InvNumber1"];
                        }
                        NewRow["UNN"] = table.Rows[i]["UNN"];
                        if (table.Rows[i]["CurrencyCode"].ToString() == "974")
                            NewRow["CurrencyCode"] = "933";
                        else
                            NewRow["CurrencyCode"] = table.Rows[i]["CurrencyCode"];
                        NewRow["Amount"] = table.Rows[i]["Amount"];
                        NewRow["Price"] = table.Rows[i]["Price"];
                        table1.Rows.Add(NewRow);
                        DataSetIntoDBF11(table1, filePath, fileName);
                        table1.Clear();
                        continue;
                    }

                    NewRow = table1.NewRow();
                    DataRow[] rows1 = DecorConfigDT.Select("InvNumber='" + table.Rows[i]["InvNumber"].ToString() + "'");
                    if (rows1.Count() > 0)
                        NewRow["InvNumber"] = rows1[0]["InvNumber1"];
                    else
                    {
                        rows1 = FrontsConfigDT.Select("InvNumber='" + table.Rows[i]["InvNumber"].ToString() + "'");
                        if (rows1.Count() > 0)
                            NewRow["InvNumber"] = rows1[0]["InvNumber1"];
                    }
                    NewRow["UNN"] = table.Rows[i]["UNN"];
                    if (table.Rows[i]["CurrencyCode"].ToString() == "974")
                        NewRow["CurrencyCode"] = "933";
                    else
                        NewRow["CurrencyCode"] = table.Rows[i]["CurrencyCode"];
                    NewRow["Amount"] = table.Rows[i]["Amount"];
                    NewRow["Price"] = table.Rows[i]["Price"];
                    table1.Rows.Add(NewRow);
                }
                else
                {
                    NewRow = table1.NewRow();
                    DataRow[] rows1 = DecorConfigDT.Select("InvNumber='" + table.Rows[i]["InvNumber"].ToString() + "'");
                    if (rows1.Count() > 0)
                        NewRow["InvNumber"] = rows1[0]["InvNumber1"];
                    else
                    {
                        rows1 = FrontsConfigDT.Select("InvNumber='" + table.Rows[i]["InvNumber"].ToString() + "'");
                        if (rows1.Count() > 0)
                            NewRow["InvNumber"] = rows1[0]["InvNumber1"];
                    }
                    NewRow["UNN"] = table.Rows[i]["UNN"];
                    if (table.Rows[i]["CurrencyCode"].ToString() == "974")
                        NewRow["CurrencyCode"] = "933";
                    else
                        NewRow["CurrencyCode"] = table.Rows[i]["CurrencyCode"];
                    NewRow["Amount"] = table.Rows[i]["Amount"];
                    NewRow["Price"] = table.Rows[i]["Price"];
                    table1.Rows.Add(NewRow);
                    DataSetIntoDBF11(table1, filePath, fileName);
                }
            }
        }

        public void AddNewInvNumber()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("InvNumber"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("NewInvNumber"), System.Type.GetType("System.String")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }
            DataTable DecorConfigDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM FrontsConfig",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(DecorConfigDT);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string InvNumber = table.Rows[i]["InvNumber"].ToString();
                        string NewInvNumber = table.Rows[i]["NewInvNumber"].ToString();
                        if (InvNumber.Length == 0 || NewInvNumber.Length == 0)
                            continue;
                        DataRow[] rows = DecorConfigDT.Select("InvNumber1='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            foreach (DataRow item in rows)
                            {
                                item["InvNumber"] = "00" + NewInvNumber;
                            }
                    }
                    if (DecorConfigDT.GetChanges() != null)
                    {
                        DataTable D = DecorConfigDT.GetChanges();
                        DA.Update(DecorConfigDT);
                    }
                }
            }
            DecorConfigDT.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DecorConfig",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(DecorConfigDT);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string InvNumber = table.Rows[i]["InvNumber"].ToString();
                        string NewInvNumber = table.Rows[i]["NewInvNumber"].ToString();
                        if (InvNumber.Length == 0 || NewInvNumber.Length == 0)
                            continue;
                        DataRow[] rows = DecorConfigDT.Select("InvNumber1='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            foreach (DataRow item in rows)
                            {
                                item["InvNumber"] = "00" + NewInvNumber;
                            }
                    }
                    if (DecorConfigDT.GetChanges() != null)
                    {
                        DataTable D = DecorConfigDT.GetChanges();
                        DA.Update(DecorConfigDT);
                    }
                }
            }
        }

        public void AddToManufactureStore(int StoreItemID, decimal Length, decimal Width, decimal Height, decimal Thickness,
            decimal Diameter, decimal Admission, decimal Capacity, decimal Weight,
            int ColorID, int PatinaID, int CoverID, decimal Count, int FactoryID, string Notes, int DecorAssignmentID = 0, int ReturnedRollerID = 0)
        {
            DataRow[] Rows = ManufactureDT.Select(@"StoreItemID=" + StoreItemID + " AND Diameter=" + Diameter + " AND Width=" + Width);
            if (Rows.Count() == 0)
            {
                DataRow NewRow = ManufactureDT.NewRow();
                if (ManufactureDT.Columns.Contains("CreateDateTime"))
                    NewRow["CreateDateTime"] = Security.GetCurrentDate();
                NewRow["DecorAssignmentID"] = DecorAssignmentID;
                NewRow["ReturnedRollerID"] = ReturnedRollerID;
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

                if (Notes.Length > 0)
                    NewRow["Notes"] = Notes;
                ManufactureDT.Rows.Add(NewRow);
            }
            else
            {
                Rows[0]["DecorAssignmentID"] = DecorAssignmentID;
                Rows[0]["ReturnedRollerID"] = ReturnedRollerID;
                Rows[0]["StoreItemID"] = StoreItemID;
                if (Length > -1)
                    Rows[0]["Length"] = Length;
                if (Width > -1)
                    Rows[0]["Width"] = Width;
                if (Height > -1)
                    Rows[0]["Height"] = Height;
                if (Thickness > -1)
                    Rows[0]["Thickness"] = Thickness;
                if (Diameter > -1)
                    Rows[0]["Diameter"] = Diameter;
                if (Admission > -1)
                    Rows[0]["Admission"] = Admission;
                if (Capacity > -1)
                    Rows[0]["Capacity"] = Capacity;
                if (Weight > -1)
                    Rows[0]["Weight"] = Weight;
                if (ColorID > -1)
                    Rows[0]["ColorID"] = ColorID;
                if (CoverID > -1)
                    Rows[0]["CoverID"] = CoverID;
                if (PatinaID > -1)
                    Rows[0]["PatinaID"] = PatinaID;
                Rows[0]["InvoiceCount"] = Convert.ToInt32(Rows[0]["InvoiceCount"]) + Count;
                Rows[0]["CurrentCount"] = Convert.ToInt32(Rows[0]["CurrentCount"]) + Count;
                Rows[0]["FactoryID"] = FactoryID;

                if (Notes.Length > 0)
                    Rows[0]["Notes"] = Notes;
            }
        }

        public bool RemoveNotExistedRecords()
        {
            DataTable newsDt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM NewsAttachs", ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                {
                    da.Fill(newsDt);

                    string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("LightNews");

                    for (int i = newsDt.Rows.Count - 1; i >= 0; i--)
                    {
                        int newsAttachId = Convert.ToInt32(newsDt.Rows[i]["NewsAttachID"]);

                        string sExtension = ".idf";
                        string sFileName = newsDt.Rows[i]["FileName"].ToString();

                        if (!FM.FileExist(sDestFolder + "/" + newsAttachId + sExtension, Configs.FTPType))
                        {
                            newsDt.Rows[i].Delete();
                        }
                    }

                    da.Update(newsDt);
                }
            }
            return true;
        }

        public bool SaveRenamedLightNews()
        {
            DataTable newsDt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM NewsAttachs", ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                {
                    da.Fill(newsDt);

                    string sDestFolder = Configs.DocumentsZOVTPSPath + FileManager.GetPath("LightNews");
                    string sSourceFolder = @"C:\\LightNews";

                    for (int i = newsDt.Rows.Count - 1; i >= 0; i--)
                    {
                        int newsAttachId = Convert.ToInt32(newsDt.Rows[i]["NewsAttachID"]);
                        FileInfo fi = new FileInfo(newsDt.Rows[i]["FileName"].ToString());
                        string sExtension = fi.Extension;
                        string sFileName = Path.GetFileNameWithoutExtension(fi.Name);
                        string sFileName1 = sFileName;

                        int j = 1;
                        while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                        {
                            sFileName = sFileName1 + "(" + j++ + ")";
                        }
                        newsDt.Rows[i]["FileName"] = sFileName + sExtension;
                        if (!FM.UploadFile(sSourceFolder + "/" + newsAttachId + ".idf", sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            continue;
                    }

                    da.Update(newsDt);
                }
            }
            return true;
        }

        public bool AttachOperationsDocument(DataTable AttachmentsDataTable, string TechID)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM OperationsDocuments", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["TechID"] = TechID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);

                    }
                }
            }

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OperationsDocuments WHERE TechID = " + TechID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            if (FM.UploadFile(Row["Path"].ToString(), Configs.DocumentsPath +
                                          FileManager.GetPath("TechCatalogOperations") + "/" +
                                          DT.Select("FileName = '" + Row["FileName"].ToString() + "'")[0]["OperationsDocumentID"] + ".idf", Configs.FTPType) == false)
                                break;
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }
            RefreshOperationsDocuments();

            return Ok;
        }

        public bool AttachTechStoreDocument(DataTable AttachmentsDataTable, string TechID, int DocType)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments WHERE TechID = " + TechID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments");
                            string sExtension = Row["Extension"].ToString();
                            string sFileName = Row["FileName"].ToString();

                            int j = 1;
                            while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                            {
                                sFileName = Row["FileName"].ToString() + "(" + j++ + ")";
                            }
                            Row["FileName"] = sFileName + sExtension;
                            if (FM.UploadFile(Row["Path"].ToString(), sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                                break;
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechStoreDocuments", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["DocType"] = DocType;
                            NewRow["TechID"] = TechID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            RefreshTechStoreDocuments();

            return Ok;
        }

        public bool AttachToolsDocument(DataTable AttachmentsDataTable, string TechID)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ToolsDocuments", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachmentsDataTable.Rows)
                        {
                            FileInfo fi;

                            try
                            {
                                fi = new FileInfo(Row["Path"].ToString());

                            }
                            catch
                            {
                                Ok = false;
                                continue;
                            }

                            DataRow NewRow = DT.NewRow();
                            NewRow["TechID"] = TechID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);

                    }
                }
            }

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ToolsDocuments WHERE TechID = " + TechID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in AttachmentsDataTable.Rows)
                    {
                        try
                        {
                            if (FM.UploadFile(Row["Path"].ToString(), Configs.DocumentsPath +
                                          FileManager.GetPath("TechCatalogTools") + "/" +
                                          DT.Select("FileName = '" + Row["FileName"].ToString() + "'")[0]["ToolsDocumentID"] + ".idf", Configs.FTPType) == false)
                                break;
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }

                    DA.Update(DT);
                }
            }
            RefreshToolsDocuments();

            return Ok;
        }

        public void Binding()
        {
            CabFurnitureDocumentTypesBS = new BindingSource()
            {
                DataSource = CabFurnitureDocumentTypesDT,
                Sort = "DocName"
            };
            PositionsBS = new BindingSource
            {
                DataSource = PositionsDT,
                Sort = "Position, TariffRank"
            };

            TechStoreGroupsBS = new BindingSource()
            {
                DataSource = TechStoreGroupsDT,
                Sort = "TechStoreGroupName"
            };
            TechStoreSubGroupsBS = new BindingSource()
            {
                DataSource = TechStoreSubGroupsDT,
                Sort = "TechStoreSubGroupName"
            };
            TechStoreBS = new BindingSource()
            {
                DataSource = TechStoreDT,
                Sort = "TechStoreName"
            };
            SectorsBS = new BindingSource()
            {
                DataSource = SectorsDT
            };
            SubSectorsBS = new BindingSource()
            {
                DataSource = SubSectorsDT
            };
            MachinesBS = new BindingSource()
            {
                DataSource = MachinesDT
            };
            AllMachinesBS = new BindingSource()
            {
                DataSource = MachinesDT.Copy(),
                Sort = "MachineName"
            };
            MachinesOperationsBS = new BindingSource()
            {
                DataSource = MachinesOperationsDT,
                Sort = "MachinesOperationName"
            };
            TechCatalogOperationsDetailBS = new BindingSource()
            {
                DataSource = TechCatalogOperationsDetailDT
            };
            TechCatalogOperationsDetailGroupsBS = new BindingSource()
            {
                DataSource = TechCatalogOperationsDetailGroupsDT
            };
            TechCatalogStoreDetailBS = new BindingSource()
            {
                DataSource = TechCatalogStoreDetailDT
            };
            ToolsGroupsBS = new BindingSource()
            {
                DataSource = ToolsGroupDT
            };
            ToolsTypesBS = new BindingSource()
            {
                DataSource = ToolsTypeDT
            };
            ToolsSubTypesBS = new BindingSource()
            {
                DataSource = ToolsSubTypeDT
            };
            ToolsBS = new BindingSource()
            {
                DataSource = ToolsDT
            };
            TechCatalogToolsBS = new BindingSource()
            {
                DataSource = TechCatalogToolsDT
            };
        }

        public void CreateOperationsGroups()
        {
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            string SelectCommand = @"SELECT DISTINCT TechStoreID, GroupNumber FROM TechCatalogOperationsDetail WHERE TechCatalogOperationsGroupID IS NULL ORDER BY TechStoreID, GroupNumber";
            //using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //{
            //    using (SqlCommandBuilder CB1 = new SqlCommandBuilder(DA1))
            //    {
            //        if (DA1.Fill(DT1) > 0)
            //        {
            //            SelectCommand = "SELECT TechCatalogOperationsGroupID, TechStoreID, GroupNumber FROM TechCatalogOperationsGroups";
            //            using (SqlDataAdapter DA2 = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            //            {
            //                using (SqlCommandBuilder CB2 = new SqlCommandBuilder(DA2))
            //                {
            //                    DA2.Fill(DT2);
            //                    for (int i = 0; i < DT1.Rows.Count; i++)
            //                    {
            //                        DataRow NewRow = DT2.NewRow();
            //                        NewRow["TechStoreID"] = DT1.Rows[i]["TechStoreID"];
            //                        NewRow["GroupNumber"] = DT1.Rows[i]["GroupNumber"];
            //                        DT2.Rows.Add(NewRow);
            //                    }
            //                    DA2.Update(DT2);
            //                    DT2.Clear();
            //                    DA2.Fill(DT2);
            //                }
            //            }
            //        }
            //    }
            //}

            SelectCommand = "SELECT TechCatalogOperationsGroupID, TechStoreID, GroupNumber FROM TechCatalogOperationsGroups";
            using (SqlDataAdapter DA2 = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB2 = new SqlCommandBuilder(DA2))
                {
                    DA2.Fill(DT2);
                }
            }

            SelectCommand = @"SELECT * FROM TechCatalogOperationsDetail WHERE TechCatalogOperationsGroupID IS NULL";
            using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB1 = new SqlCommandBuilder(DA1))
                {
                    if (DA1.Fill(DT1) > 0)
                    {
                        for (int i = 0; i < DT2.Rows.Count; i++)
                        {
                            DataRow[] rows = DT1.Select("TechStoreID=" + DT2.Rows[i]["TechStoreID"] + " AND GroupNumber=" + DT2.Rows[i]["GroupNumber"]);
                            foreach (DataRow item in rows)
                                item["TechCatalogOperationsGroupID"] = DT2.Rows[i]["TechCatalogOperationsGroupID"];
                        }
                        DA1.Update(DT1);
                    }
                }
            }
        }

        public DataTable CreateTableParametrs()
        {
            using (DataTable tDataTable = new DataTable("Parametrs"))
            {
                tDataTable.Columns.Add("ParametrID", System.Type.GetType("System.Int32"));
                tDataTable.Columns.Add("ParametrName");
                tDataTable.Columns.Add("Type");
                tDataTable.Columns["ParametrID"].AutoIncrement = true;
                tDataTable.Columns["ParametrID"].AutoIncrementStep = 1;
                tDataTable.Columns["ParametrID"].AutoIncrementSeed = 1;
                return tDataTable;
            }
        }

        public DataTable CreateTableValueParametrs()
        {
            using (DataTable tDataTable = new DataTable("ValueParametrs"))
            {
                tDataTable.Columns.Add("ParametrID", System.Type.GetType("System.Int32"));
                tDataTable.Columns.Add("Value");
                return tDataTable;
            }
        }

        public void FilterMachines(int SubSectorID)
        {
            MachinesBS.Filter = "SubSectorID = " + SubSectorID;
            MachinesBS.MoveFirst();
        }

        public void FilterMachinesOperations(int MachineID)
        {
            MachinesOperationsBS.Filter = "MachineID = " + MachineID;
            MachinesOperationsBS.MoveFirst();
        }

        public void FilterOperationsDetails(int TechCatalogOperationsGroupID)
        {
            TechCatalogOperationsDetailBS.Filter = "TechCatalogOperationsGroupID = " + TechCatalogOperationsGroupID;
            TechCatalogOperationsDetailBS.MoveFirst();
        }

        public void FilterOperationsGroups(int TechStoreID)
        {
            TechCatalogOperationsDetailGroupsBS.Filter = "TechStoreID = " + TechStoreID;
            TechCatalogOperationsDetailGroupsBS.MoveFirst();
        }

        public void FilterSectors(int FactoryID)
        {
            SectorsBS.Filter = "FactoryID = " + FactoryID;
            SectorsBS.MoveFirst();
        }

        public void FilterStoreDetails(int TechCatalogOperationsDetailID)
        {
            TechCatalogStoreDetailBS.Filter = "TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID;
            TechCatalogStoreDetailBS.MoveFirst();

            TechCatalogToolsBS.Filter = "TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID;
            TechCatalogToolsBS.MoveFirst();
        }

        public void FilterSubSectors(int SectorID)
        {
            SubSectorsBS.Filter = "SectorID = " + SectorID;
            SubSectorsBS.MoveFirst();
        }

        public void FilterTechStore(int TechStoreSubGroupID)
        {
            TechStoreBS.Filter = "TechStoreSubGroupID = " + TechStoreSubGroupID;
            TechStoreBS.MoveFirst();
        }

        public void FilterTechStoreSubGroups(int TechStoreGroupID)
        {
            TechStoreSubGroupsBS.Filter = "TechStoreGroupID = " + TechStoreGroupID;
            TechStoreSubGroupsBS.MoveFirst();
        }

        public void FilterTools(int ToolsSubTypeID)
        {
            ToolsBS.Filter = "ToolsSubTypeID = " + ToolsSubTypeID;
            ToolsBS.MoveFirst();
        }

        public void FilterToolsGroups(int FactoryID)
        {
            ToolsGroupsBS.Filter = "FactoryID = " + FactoryID;
            ToolsGroupsBS.MoveFirst();
        }

        public void FilterToolsSubTypes(int ToolsTypeID)
        {
            ToolsSubTypesBS.Filter = "ToolsTypeID = " + ToolsTypeID;
            ToolsSubTypesBS.MoveFirst();
        }

        public void FilterToolsTypes(int ToolsGroupID)
        {
            ToolsTypesBS.Filter = "ToolsGroupID = " + ToolsGroupID;
            ToolsTypesBS.MoveFirst();
        }

        public Image GetMachinePhoto(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachinePhoto FROM Machines WHERE MachineID = " + MachineID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["MachinePhoto"] == DBNull.Value)
                        return null;

                    byte[] b = (byte[])DT.Rows[0]["MachinePhoto"];
                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }
        }

        public DataTable GetOperationsDocuments(string TechID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OperationsDocumentID, FileName FROM OperationsDocuments WHERE TechID = " + TechID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public DataTable GetTechStoreDocuments(string TechID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments WHERE TechID = " + TechID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public Image GetTechStoreImage(int TechStoreDocumentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments" +
                " WHERE TechStoreDocumentID = " + TechStoreDocumentID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return null;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return null;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        using (MemoryStream ms = new MemoryStream(
                            FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName,
                            Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType)))
                        {
                            return Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return null;
                    }
                }
            }
        }

        public DataTable GetToolsDocuments(string TechID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsDocumentID, FileName FROM ToolsDocuments WHERE TechID = " + TechID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public void Initialize()
        {
            CreateAndFill();
            Binding();
        }

        public bool MachineParametrCanBeApply(int ParametrID, string NewValue)
        {
            if (CurrentMachineParametrsDT.Select("ParametrID = " + ParametrID)[0]["type"].ToString() == "Числовой")
            {
                try
                {
                    Convert.ToDecimal(NewValue);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public DataTable MachineReadValueTable(int MachineID)
        {
            using (DataTable DT = CreateTableValueParametrs())
            {
                using (DataTable ValueParametrsDT = CreateTableValueParametrs())
                {
                    string ParametrsXML = MachinesDT.Select("MachineID = " + MachineID)[0]["Parametrs"].ToString();
                    if (ParametrsXML.Length == 0)
                        return DT;

                    if (MachineID != 0)
                    {
                        string ValueParametrsXML = MachinesDT.Select("MachineID = " + MachineID)[0]["ValueParametrs"].ToString();
                        if (ValueParametrsXML.Length != 0)
                        {
                            using (StringReader SR = new StringReader(ValueParametrsXML))
                            {
                                ValueParametrsDT.ReadXml(SR);
                            }
                        }
                    }

                    using (StringReader SR = new StringReader(ParametrsXML))
                    {
                        using (DataTable ParametrsDT = CreateTableParametrs())
                        {
                            ParametrsDT.ReadXml(SR);
                            CurrentMachineParametrsDT = ParametrsDT.Copy();

                            foreach (DataRow Row in ParametrsDT.Rows)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow["ParametrID"] = Row["ParametrID"];

                                DataRow[] ValueRow = ValueParametrsDT.Select("ParametrID = " + Row["ParametrID"]);
                                if (ValueRow.Count() != 0)
                                    NewRow["Value"] = ValueRow[0]["Value"];

                                DT.Rows.Add(NewRow);
                            }
                        }
                    }

                    return DT;
                }
            }
        }

        public void MoveToGroupNumber(int GroupNumber)
        {
            TechCatalogOperationsDetailGroupsBS.Position = TechCatalogOperationsDetailGroupsBS.Find("TechCatalogOperationsGroupID", GroupNumber);
        }

        public void MoveToOperationsDetail(int TechCatalogOperationsDetailID)
        {
            TechCatalogOperationsDetailBS.Position = TechCatalogOperationsDetailBS.Find("TechCatalogOperationsDetailID", TechCatalogOperationsDetailID);
        }

        public void MoveToStoreDetail(int TechCatalogStoreDetailID)
        {
            TechCatalogStoreDetailBS.Position = TechCatalogStoreDetailBS.Find("TechCatalogStoreDetailID", TechCatalogStoreDetailID);
        }

        public void MoveToMachinesOperation(int MachinesOperationID)
        {
            MachinesOperationsBS.Position = MachinesOperationsBS.Find("MachinesOperationID", MachinesOperationID);
        }

        public void MoveToSerialNumber(int TechCatalogOperationsDetailID)
        {
            TechCatalogOperationsDetailBS.Position = TechCatalogOperationsDetailBS.Find("TechCatalogOperationsDetailID", TechCatalogOperationsDetailID);
        }

        public void MoveToTechStore(int TechStoreID)
        {
            TechStoreBS.Position = TechStoreBS.Find("TechStoreID", TechStoreID);
        }

        public void MoveToTechStoreGroup(int TechStoreGroupID)
        {
            TechStoreGroupsBS.Position = TechStoreGroupsBS.Find("TechStoreGroupID", TechStoreGroupID);
        }

        public void MoveToTechStoreSubGroup(int TechStoreSubGroupID)
        {
            TechStoreSubGroupsBS.Position = TechStoreSubGroupsBS.Find("TechStoreSubGroupID", TechStoreSubGroupID);
        }

        public bool OperationsUseInCatalog(int MachinesOperationID)
        {
            return TechCatalogOperationsDetailDT.Select("MachinesOperationID = " + MachinesOperationID).Count() > 0;
        }

        public bool ParametrCanBeApply(int ParametrID, string NewValue)
        {
            if (CurrentParametrsDT.Select("ParametrID = " + ParametrID)[0]["type"].ToString() == "Числовой")
            {
                try
                {
                    Convert.ToDecimal(NewValue);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public DataTable ReadValueTable(string ValueParametrsXML, string ParametrsXML)
        {
            using (DataTable DT = CreateTableValueParametrs())
            {
                using (DataTable ValueParametrsDT = CreateTableValueParametrs())
                {
                    if (ParametrsXML.Length == 0)
                        return DT;

                    if (ValueParametrsXML.Length != 0)
                    {
                        using (StringReader SR = new StringReader(ValueParametrsXML))
                        {
                            ValueParametrsDT.ReadXml(SR);
                        }
                    }

                    using (StringReader SR = new StringReader(ParametrsXML))
                    {
                        using (DataTable ParametrsDT = CreateTableParametrs())
                        {
                            ParametrsDT.ReadXml(SR);
                            CurrentParametrsDT = ParametrsDT.Copy();

                            foreach (DataRow Row in ParametrsDT.Rows)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow["ParametrID"] = Row["ParametrID"];

                                DataRow[] ValueRow = ValueParametrsDT.Select("ParametrID = " + Row["ParametrID"]);
                                if (ValueRow.Count() != 0)
                                    NewRow["Value"] = ValueRow[0]["Value"];

                                DT.Rows.Add(NewRow);
                            }
                        }
                    }

                    return DT;
                }
            }
        }

        public void RefreshOperationsDocuments()
        {
            OperationsDocumentsDT.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OperationsDocuments", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsDocumentsDT);
            }
        }

        public void RefreshTechStoreDocuments()
        {
            TechStoreDocumentsDT.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDocumentsDT);
            }
        }

        public void RefreshToolsDocuments()
        {
            ToolsDocumentsDT.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ToolsDocuments", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsDocumentsDT);
            }
        }

        public void RemoveOperationsDocuments(string TechID, string FileName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OperationsDocuments WHERE TechID = " + TechID +
                " AND FileName = '" + FileName + "'", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("TechCatalogOperations") + "/" +
                                      DT.Select("FileName = '" + Row["FileName"].ToString() + "'")[0]["OperationsDocumentID"] + ".idf", Configs.FTPType);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM OperationsDocuments WHERE TechID = " + TechID +
                " AND FileName = '" + FileName + "'", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }

            RefreshOperationsDocuments();
        }

        public void RemoveOperationsDocuments(int TechID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OperationsDocuments WHERE TechID = " + TechID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("TechCatalogOperations") + "/" +
                                      DT.Select("FileName = '" + Row["FileName"].ToString() + "'")[0]["OperationsDocumentID"] + ".idf", Configs.FTPType);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM OperationsDocuments WHERE TechID = " + TechID, ConnectionStrings.CatalogConnectionString))
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

        public void RemoveTechStoreDocuments(int TechStoreDocumentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments WHERE TechStoreDocumentID = " + TechStoreDocumentID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM TechStoreDocuments WHERE TechStoreDocumentID = " + TechStoreDocumentID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }

            RefreshTechStoreDocuments();
        }

        public void RemoveToolsDocuments(string TechID, string FileName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ToolsDocuments WHERE TechID = " + TechID +
                " AND FileName = '" + FileName + "'", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("TechCatalogTools") + "/" +
                                      DT.Select("FileName = '" + Row["FileName"].ToString() + "'")[0]["ToolsDocumentID"] + ".idf", Configs.FTPType);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM ToolsDocuments WHERE TechID = " + TechID +
                " AND FileName = '" + FileName + "'", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }

            RefreshToolsDocuments();
        }

        public void ReplaceAccountingName()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("InvNumber"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("AccountingName"), System.Type.GetType("System.String")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }
            DataTable DecorConfigDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM FrontsConfig",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(DecorConfigDT);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string InvNumber = table.Rows[i]["InvNumber"].ToString();
                        if (InvNumber.Length == 0)
                            continue;
                        string AccountingName = table.Rows[i]["AccountingName"].ToString();
                        DataRow[] rows = DecorConfigDT.Select("InvNumber='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            foreach (DataRow item in rows)
                            {
                                s = item["AccountingName"].ToString();
                                item["AccountingName"] = AccountingName;
                            }
                    }
                    if (DecorConfigDT.GetChanges() != null)
                    {
                        DataTable D = DecorConfigDT.GetChanges();
                        DA.Update(DecorConfigDT);
                    }
                }
            }
            DecorConfigDT.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM DecorConfig",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(DecorConfigDT);
                    if (DecorConfigDT.GetChanges() != null)
                    {
                        DataTable D = DecorConfigDT.GetChanges();
                        DA.Update(DecorConfigDT);
                    }
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string InvNumber = table.Rows[i]["InvNumber"].ToString();
                        if (InvNumber.Length == 0)
                            continue;
                        string AccountingName = table.Rows[i]["AccountingName"].ToString();
                        DataRow[] rows = DecorConfigDT.Select("InvNumber='" + InvNumber + "'");
                        if (rows.Count() > 0)
                            foreach (DataRow item in rows)
                            {
                                s = item["AccountingName"].ToString();
                                item["AccountingName"] = AccountingName;
                            }
                    }
                    if (DecorConfigDT.GetChanges() != null)
                    {
                        DataTable D = DecorConfigDT.GetChanges();
                        DA.Update(DecorConfigDT);
                    }
                }
            }
        }

        public void SaveClientsCatalogImagesFromExcel()
        {
            ManufactureDT = new DataTable();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("ImageID"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("TempName"), System.Type.GetType("System.String")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM ClientsCatalogImages",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            int ImageID = 0;
                            string TempName = string.Empty;
                            ImageID = Convert.ToInt32(table.Rows[i]["ImageID"]);
                            TempName = table.Rows[i]["TempName"].ToString();
                            DataRow[] rows = DT.Select("ImageID=" + ImageID);
                            if (rows.Count() > 0)
                                rows[0]["TempName"] = TempName;
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveCabFurnitureDocumentTypesFromExcel()
        {
            ManufactureDT = new DataTable();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("MachinesOperationID"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("CabFurDocTypeID"), System.Type.GetType("System.Int32")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachinesOperations",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            int MachinesOperationID = 0;
                            int CabFurDocTypeID = 0;
                            MachinesOperationID = Convert.ToInt32(table.Rows[i]["MachinesOperationID"]);
                            CabFurDocTypeID = Convert.ToInt32(table.Rows[i]["CabFurDocTypeID"]);
                            DataRow[] rows = DT.Select("MachinesOperationID=" + MachinesOperationID);
                            if (rows.Count() > 0)
                                rows[0]["CabFurDocTypeID"] = CabFurDocTypeID;
                        }
                        DA.Update(DT);
                    }
                }
            }
        }

        public void SaveCoatingMaterialFromExcel()
        {
            ManufactureDT = new DataTable();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("TechStoreID"), System.Type.GetType("System.Int32")));
            //table.Columns.Add(new DataColumn(("TechStoreName"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Diameter1"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter2"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter3"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter4"), System.Type.GetType("System.Int32")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Store
WHERE StoreItemID IN (SELECT TechStoreID FROM infiniu2_catalog.dbo.TechStore WHERE TechStoreSubGroupID IN 
(SELECT TechStoreSubGroupID FROM infiniu2_catalog.dbo.TechStoreSubGroups WHERE TechStoreGroupID = 1))",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(ManufactureDT);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        int Length = 0;
                        int Width = 0;
                        int CurrentCount = 0;
                        int TechStoreID = 0;
                        if (table.Rows[i]["TechStoreID"] == DBNull.Value)
                            continue;
                        TechStoreID = Convert.ToInt32(table.Rows[i]["TechStoreID"]);
                        Length = Convert.ToInt32(table.Rows[i][1]);
                        Width = Convert.ToInt32(table.Rows[i][2]);
                        CurrentCount = Convert.ToInt32(table.Rows[i][3]);
                        DataRow[] rows = ManufactureDT.Select("StoreItemID=" + TechStoreID + " AND Length=" + Length + " AND Width=" + Width);
                        if (rows.Count() > 0)
                            rows[0]["CurrentCount"] = Convert.ToInt32(rows[0]["CurrentCount"]) + CurrentCount;
                    }
                    DA.Update(ManufactureDT);
                }
            }
        }

        public void SaveCoatingRollersFromExcel()
        {
            ManufactureDT = new DataTable();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("TechStoreID"), System.Type.GetType("System.Int32")));
            //table.Columns.Add(new DataColumn(("TechStoreName"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Diameter1"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter2"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter3"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter4"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter5"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter6"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter7"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter8"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter9"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter10"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter11"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter12"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter13"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter14"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter15"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter16"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter17"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter18"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter19"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter20"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter21"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter22"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter23"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter24"), System.Type.GetType("System.Int32")));
            table.Columns.Add(new DataColumn(("Diameter25"), System.Type.GetType("System.Int32")));
            //table.Columns.Add(new DataColumn(("Diameter26"), System.Type.GetType("System.Int32")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ManufactureStore",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(ManufactureDT);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        int Diameter = 0;
                        int Width = 0;
                        int TechStoreID = 0;
                        if (table.Rows[i]["TechStoreID"] == DBNull.Value)
                            continue;
                        TechStoreID = Convert.ToInt32(table.Rows[i]["TechStoreID"]);
                        for (int j = 1; j < table.Columns.Count; j++)
                        {
                            if (j > 18)
                                continue;
                            string ColumnName = "Diameter" + j;
                            if (table.Rows[i][ColumnName] == DBNull.Value)
                                break;
                            Diameter = Convert.ToInt32(table.Rows[i][ColumnName]);
                            Width = Convert.ToInt32(table.Rows[i + 1][ColumnName]);

                            int StoreItemID = TechStoreID;
                            decimal Length = -1;
                            decimal Height = -1;
                            decimal Thickness = -1;
                            decimal Admission = -1;
                            decimal Capacity = -1;
                            decimal Weight = -1;
                            int ColorID = -1;
                            int PatinaID = -1;
                            int CoverID = -1;
                            int Count = 1;
                            int FactoryID = 1;
                            string Notes = string.Empty;
                            AddToManufactureStore(StoreItemID, Length, Width, Height, Thickness,
                                Diameter, Admission, Capacity, Weight, ColorID, PatinaID, CoverID, Count, FactoryID, Notes);
                        }
                        i++;
                    }
                    DA.Update(ManufactureDT);
                }
            }
        }

        public string SaveOperationsDocuments(string FileNameDoc)//temp folder
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";
            int OperationsDocumentID;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OperationsDocuments WHERE FileName = '" + FileNameDoc + "'", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    OperationsDocumentID = Convert.ToInt32(DT.Rows[0]["OperationsDocumentID"]);
                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("TechCatalogOperations") + "/" + OperationsDocumentID + ".idf",
                                            tempFolder + "\\" + FileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return tempFolder + "\\" + FileName;
        }

        public void SaveSellersFromExcel()
        {
            DataTable SellersDT = new DataTable();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(("SellerName"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Address"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("ContractDocNumber"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Email"), System.Type.GetType("System.String")));
            table.Columns.Add(new DataColumn(("Site"), System.Type.GetType("System.String")));
            table.TableName = "ImportedTable";

            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>(lines);

            if (data.Count > 0 && string.IsNullOrWhiteSpace(data[data.Count - 1]))
            {
                data.RemoveAt(data.Count - 1);
            }

            foreach (string iterationRow in data)
            {
                string row = iterationRow;
                if (row.EndsWith("\r"))
                {
                    row = row.Substring(0, row.Length - "\r".Length);
                }

                string[] rowData = row.Split(new char[] { '\r', '\x09' });
                DataRow newRow = table.NewRow();

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= table.Columns.Count) break;
                    if (rowData[i].Length > 0)
                        newRow[i] = rowData[i];
                }
                table.Rows.Add(newRow);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ToolsSellers",
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(SellersDT);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string SellerName = table.Rows[i]["SellerName"].ToString();
                        string Country = "РБ";
                        string Address = table.Rows[i]["Address"].ToString();
                        string ContractDocNumber = table.Rows[i]["ContractDocNumber"].ToString();
                        string Email = table.Rows[i]["Email"].ToString();
                        string Site = table.Rows[i]["Site"].ToString();
                        int SellerSubGroupID = 58;

                        DataRow NewRow = SellersDT.NewRow();
                        NewRow["ToolsSellerName"] = SellerName;
                        NewRow["Country"] = Country;
                        NewRow["Address"] = Address;
                        NewRow["ContractDocNumber"] = ContractDocNumber;
                        NewRow["Email"] = Email;
                        NewRow["Site"] = Site;
                        NewRow["ToolsSellerSubGroupID"] = SellerSubGroupID;
                        SellersDT.Rows.Add(NewRow);
                    }
                    DA.Update(SellersDT);
                }
            }
        }

        public string SaveTechStoreDocuments(int TechStoreDocumentID)//temp folder
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments WHERE TechStoreDocumentID = " + TechStoreDocumentID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName,
                            tempFolder + "\\" + FileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return tempFolder + "\\" + FileName;
        }

        public string SaveToolsDocuments(string FileNameDoc)//temp folder
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";
            int ToolsDocumentID;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ToolsDocuments WHERE FileName = '" + FileNameDoc + "'", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    ToolsDocumentID = Convert.ToInt32(DT.Rows[0]["ToolsDocumentID"]);
                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("TechCatalogTools") + "/" + ToolsDocumentID + ".idf",
                                            tempFolder + "\\" + FileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return tempFolder + "\\" + FileName;
        }

        public void SearchStoreDetail(int TechStoreID)
        {
            int TechStoreGroupID = 0;
            int TechStoreSubGroupID = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TechStoreGroups.TechStoreGroupID, TechStoreSubGroups.TechStoreSubGroupID FROM TechStore
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID=TechStoreSubGroups.TechStoreSubGroupID  
                INNER JOIN TechStoreGroups ON TechStoreSubGroups.TechStoreGroupID=TechStoreGroups.TechStoreGroupID  
                WHERE TechStoreID = " + TechStoreID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        TechStoreGroupID = Convert.ToInt32(DT.Rows[0]["TechStoreGroupID"]);
                        TechStoreSubGroupID = Convert.ToInt32(DT.Rows[0]["TechStoreSubGroupID"]);
                    }
                }
            }
            MoveToTechStoreGroup(TechStoreGroupID);
            MoveToTechStoreSubGroup(TechStoreSubGroupID);
            MoveToTechStore(TechStoreID);
        }

        public string SectorName(int MachineID)
        {
            DataRow[] rows = TechSectorNamesDT.Select("MachineID = " + MachineID);
            if (rows.Count() > 0)
            {
                return rows[0]["SectorName"].ToString() + "; " + rows[0]["SubSectorName"].ToString();
            }
            return string.Empty;
        }

        public void SetMachinePhoto(int MachineID, Image MachinePhoto)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachineID, MachinePhoto FROM Machines WHERE MachineID = " + MachineID, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (MachinePhoto != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ImageCodecInfo ImageCodecInfo = UserProfile.GetEncoderInfo("image/jpeg");
                                System.Drawing.Imaging.Encoder eEncoder1 = System.Drawing.Imaging.Encoder.Quality;

                                EncoderParameter myEncoderParameter1 = new EncoderParameter(eEncoder1, 100L);
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                                myEncoderParameters.Param[0] = myEncoderParameter1;
                                MachinePhoto.Save(ms, ImageCodecInfo, myEncoderParameters);
                                DT.Rows[0]["MachinePhoto"] = ms.ToArray();
                            }
                        }
                        else
                            DT.Rows[0]["MachinePhoto"] = null;

                        DA.Update(DT);
                    }
                }
            }
        }

        public string StoreName(int TechStoreID, bool IsSubGroup)
        {
            if (IsSubGroup)
            {
                DataRow[] rows = TechStoreNamesDT.Select("TechStoreSubGroupID = " + TechStoreID);
                if (rows.Count() > 0)
                {
                    return rows[0]["TechStoreGroupName"].ToString() + "; " + rows[0]["TechStoreSubGroupName"].ToString();
                }
            }
            else
            {
                DataRow[] rows = TechStoreNamesDT.Select("TechStoreID = " + TechStoreID);
                if (rows.Count() > 0)
                {
                    return rows[0]["TechStoreGroupName"].ToString() + "; " + rows[0]["TechStoreSubGroupName"].ToString();
                }
            }
            return string.Empty;
        }

        public bool ToolsUseInCatalog(int ToolsID)
        {
            return TechCatalogToolsDT.Select("ToolsID = " + ToolsID).Count() > 0;
        }

        public bool ToolsUseInCatalogByToolsGroupID(int ToolsGroupID)
        {
            foreach (DataRow Row in ToolsTypeDT.Select("ToolsGroupID = " + ToolsGroupID))
            {
                foreach (DataRow RowToolsSubType in ToolsSubTypeDT.Select("ToolsTypeID = " + Row["ToolsTypeID"]))
                {
                    foreach (DataRow RowTools in ToolsDT.Select("ToolsSubTypeID = " + RowToolsSubType["ToolsSubTypeID"]))
                    {
                        if (TechCatalogToolsDT.Select("ToolsID = " + RowTools["ToolsID"]).Count() > 0)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool ToolsUseInCatalogByToolsSubTypeID(int ToolsSubTypeID)
        {
            foreach (DataRow Row in ToolsDT.Select("ToolsSubTypeID = " + ToolsSubTypeID))
            {
                if (TechCatalogToolsDT.Select("ToolsID = " + Row["ToolsID"]).Count() > 0)
                    return true;
            }

            return false;
        }

        public bool ToolsUseInCatalogByToolsTypeID(int ToolsTypeID)
        {
            foreach (DataRow Row in ToolsSubTypeDT.Select("ToolsTypeID = " + ToolsTypeID))
            {
                foreach (DataRow RowTools in ToolsDT.Select("ToolsSubTypeID = " + Row["ToolsSubTypeID"]))
                {
                    if (TechCatalogToolsDT.Select("ToolsID = " + RowTools["ToolsID"]).Count() > 0)
                        return true;
                }
            }

            return false;
        }

        public void UpdateTechCatalogOperations(string TechCatalogOperationsDetailID, string TechCatalogImageID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail WHERE TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID, ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["TechCatalogImageID"] = TechCatalogImageID;

                        DA.Update(DT);
                    }
                }
            }
        }

        private static string convertDefaultToDos(string src)
        {
            byte[] buffer;
            buffer = Encoding.Default.GetBytes(src);
            Encoding.Convert(Encoding.Default, Encoding.GetEncoding("windows-1251"), buffer);
            return Encoding.Default.GetString(buffer);
        }

        private static string GetConnection(string path)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=dBASE IV;";
        }

        private void CreateAndFill()
        {
            string SelectCommand = @"SELECT * FROM CabFurnitureDocumentTypes";
            CabFurnitureDocumentTypesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(CabFurnitureDocumentTypesDT);
            }
            SelectCommand = @"SELECT * FROM CabFurnitureAlgorithms";
            CabFurnitureAlgorithmsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(CabFurnitureAlgorithmsDT);
            }
            PositionsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Positions ORDER BY Position, TariffRank", ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PositionsDT);
            }
            //for (int i = 0; i < PositionsDT.Rows.Count; i++)
            //{
            //    if (Convert.ToInt32(PositionsDT.Rows[i]["PositionID"]) == 0)
            //        continue;
            //PositionsDT.Rows[i]["Position"] = PositionsDT.Rows[i]["Position"].ToString() + " " + PositionsDT.Rows[i]["TariffRank"].ToString() + " разряд";
            //}

            TechCatalogOperationsDetailGroupsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsGroups  ORDER BY GroupNumber", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogOperationsDetailGroupsDT);
            }

            TechStoreGroupsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreGroups", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreGroupsDT);
            }

            TechStoreSubGroupsDT = new DataTable();
            //TechSubGroupsNamesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreSubGroups", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreSubGroupsDT);
                //DA.Fill(TechSubGroupsNamesDT);
            }

            TechStoreDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreSubGroupID, TechStoreName, TechStore.MeasureID, Notes FROM TechStore ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDT);
            }

            TechStoreNamesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreSubGroups.TechStoreGroupID, TechStore.TechStoreSubGroupID, TechStoreName, TechStoreSubGroupName, TechStoreGroupName" +
                " FROM TechStore" +
                " INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID = TechStoreSubGroups.TechStoreSubGroupID" +
                " INNER JOIN TechStoreGroups ON TechStoreSubGroups.TechStoreGroupID = TechStoreGroups.TechStoreGroupID" +
                " ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreNamesDT);
                DataRow NewRow = TechStoreNamesDT.NewRow();
                NewRow["TechStoreID"] = -1;
                NewRow["TechStoreGroupID"] = -1;
                NewRow["TechStoreSubGroupID"] = -1;
                NewRow["TechStoreName"] = "-";
                TechStoreNamesDT.Rows.InsertAt(NewRow, 0);
            }

            TechSectorNamesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachineID, Machines.SubSectorID, SubSectors.SectorID, SubSectors.SubSectorName, Sectors.SectorName" +
                " FROM Machines" +
                " INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID" +
                " INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechSectorNamesDT);
            }

            SectorsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SectorID, FactoryID, SectorName FROM Sectors", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SectorsDT);
            }

            SubSectorsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SubSectorID, SectorID, SubSectorName FROM SubSectors", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SubSectorsDT);
            }

            MachinesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachineID, SubSectorID, MachineName, Parametrs, ValueParametrs FROM Machines", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MachinesDT);
            }

            MachinesOperationsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachinesOperations", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MachinesOperationsDT);
            }

            MeasuresDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MeasureID, Measure FROM Measures", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDT);
            }

            TechCatalogOperationsDetailDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail ORDER BY SerialNumber", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogOperationsDetailDT);
            }
            TechCatalogOperationsDetailDT.Columns.Add("MachineID", System.Type.GetType("System.Int64"));
            //TechCatalogOperationsDetailDT.Columns.Add("SubSectorID", System.Type.GetType("System.Int64"));
            //TechCatalogOperationsDetailDT.Columns.Add("SectorID", System.Type.GetType("System.Int64"));
            RefreshTechCatalogOperationsDetail();

            TechCatalogStoreDetailDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetail" +
                " ORDER BY GroupA, GroupB, GroupC", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogStoreDetailDT);
            }
            TechCatalogStoreDetailDT.Columns.Add("TechName", System.Type.GetType("System.String"));
            //TechCatalogStoreDetailDT.Columns.Add("SubGroupName", System.Type.GetType("System.String"));
            //TechCatalogStoreDetailDT.Columns.Add("GroupName", System.Type.GetType("System.String"));
            TechCatalogStoreDetailDT.Columns.Add("Measure", System.Type.GetType("System.String"));
            TechCatalogStoreDetailDT.Columns.Add("Notes", System.Type.GetType("System.String"));
            RefreshTechCatalogStoreDetails();

            ToolsGroupDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsGroupID, FactoryID, ToolsGroupName FROM ToolsGroup" +
                " ORDER BY ToolsGroupName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsGroupDT);
            }

            ToolsTypeDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsTypeID, ToolsGroupID, ToolsTypeName FROM ToolsType" +
                " ORDER BY ToolsTypeName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsTypeDT);
            }

            ToolsSubTypeDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsSubTypeID, ToolsTypeID, ToolsSubTypeName, Parametrs FROM ToolsSubType" +
                " ORDER BY ToolsSubTypeName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsSubTypeDT);
            }

            ToolsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsID, ToolsSubTypeID, ToolsName, ValueParametrs FROM Tools" +
                " ORDER BY ToolsName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsDT);
            }

            TechCatalogToolsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogTools", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechCatalogToolsDT);
            }

            OperationsDocumentsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM OperationsDocuments", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsDocumentsDT);
            }

            TechStoreDocumentsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreDocuments", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDocumentsDT);
            }

            ToolsDocumentsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ToolsDocuments", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ToolsDocumentsDT);
            }
        }

        #region ToolsGroup
        public void AddToolsGroup(int FactoryID, string Name)
        {
            DataRow NewRow = ToolsGroupDT.NewRow();
            NewRow["FactoryID"] = FactoryID;
            NewRow["ToolsGroupName"] = Name;
            ToolsGroupDT.Rows.Add(NewRow);
            TechCatalogEvents.SaveEvents("Инструмент: группа ToolsGroupName=" + Name + " добавлена");

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsGroupID, FactoryID, ToolsGroupName FROM ToolsGroup", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsGroupDT);
                    ToolsGroupDT.Clear();
                    DA.Fill(ToolsGroupDT);
                }
            }
        }

        public void EditToolsGroup(int FactoryID, int ToolsGroupID, string NewName)
        {
            DataRow[] EditRows = ToolsGroupDT.Select("ToolsGroupID = " + ToolsGroupID);
            if (EditRows.Count() > 0)
            {
                string ToolsGroupName = EditRows[0]["ToolsGroupName"].ToString();
                EditRows[0]["FactoryID"] = FactoryID;
                EditRows[0]["ToolsGroupName"] = NewName;
                TechCatalogEvents.SaveEvents("Инструмент: группа " + ToolsGroupName + " отредактирована");
            }

            UpdateToolsGroups();
        }

        public void RemoveToolsGroup(int ToolsGroupID)
        {
            foreach (DataRow ToolsTypeRow in ToolsTypeDT.Select("ToolsGroupID = " + ToolsGroupID))
            {
                foreach (DataRow ToolsSubTypeRow in ToolsSubTypeDT.Select("ToolsTypeID = " + ToolsTypeRow["ToolsTypeID"]))
                {
                    foreach (DataRow ToolsRow in ToolsDT.Select("ToolsSubTypeID = " + ToolsSubTypeRow["ToolsSubTypeID"]))
                    {
                        foreach (DataRow TechCatalogToolsRow in TechCatalogToolsDT.Select("ToolsID = " + ToolsRow["ToolsID"]))
                            TechCatalogToolsRow.Delete();
                        ToolsRow.Delete();
                    }
                    ToolsSubTypeRow.Delete();
                }
                ToolsTypeRow.Delete();
            }

            UpdateTechCatalogTools();
            UpdateTools();
            UpdateToolsSubTypes();
            UpdateToolsTypes();

            DataRow[] Rows = ToolsGroupDT.Select("ToolsGroupID = " + ToolsGroupID);
            if (Rows.Count() > 0)
            {
                string ToolsGroupName = Rows[0]["ToolsGroupName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Инструмент: группа " + ToolsGroupName + " ToolsGroupID=" + ToolsGroupID + " удалена");
            }

            UpdateToolsGroups();
        }

        private void UpdateToolsGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ToolsGroupID, FactoryID, ToolsGroupName FROM ToolsGroup", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsGroupDT);
                }
            }
        }

        #endregion

        #region Tools
        public void AddTools(int ToolsSubTypeID, string Name, string ValueParametrs)
        {
            DataRow NewRow = ToolsDT.NewRow();

            NewRow["ToolsSubTypeID"] = ToolsSubTypeID;
            NewRow["ToolsName"] = Name;
            NewRow["ValueParametrs"] = ValueParametrs;

            ToolsDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Инструмент: инструмент ToolsName=" + Name + " добавлен");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsID, ToolsSubTypeID, ToolsName, ValueParametrs FROM Tools", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsDT);
                    ToolsDT.Clear();
                    DA.Fill(ToolsDT);
                }
            }
        }

        public void EditTools(int ToolsID, string NewName, string NewValueParametrs)
        {
            DataRow[] EditRows = ToolsDT.Select("ToolsID = " + ToolsID);
            if (EditRows.Count() > 0)
            {
                string ToolsName = EditRows[0]["ToolsName"].ToString();
                EditRows[0]["ToolsName"] = NewName;
                EditRows[0]["ValueParametrs"] = NewValueParametrs;
                TechCatalogEvents.SaveEvents("Инструмент: инструмент " + ToolsName + " отредактирован");
            }

            UpdateTools();
        }

        public void RemoveTools(int ToolsID)
        {
            foreach (DataRow TechCatalogToolsRow in TechCatalogToolsDT.Select("ToolsID = " + ToolsID))
                TechCatalogToolsRow.Delete();

            DataRow[] Rows = ToolsDT.Select("ToolsID = " + ToolsID);
            if (Rows.Count() > 0)
            {
                string ToolsName = Rows[0]["ToolsName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Инструмент: инструмент " + ToolsName + " ToolsID=" + ToolsID + " удален");
            }

            UpdateTechCatalogTools();
            UpdateTools();
        }

        public void UpdateToolsNameColumn(ref PercentageDataGrid TechCatalogToolsGrid)
        {
            ((DataGridViewComboBoxColumn)TechCatalogToolsGrid.Columns["ToolsName"]).DataSource = ToolsDT.Copy();
        }
        private void UpdateTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ToolsID, ToolsSubTypeID, ToolsName, ValueParametrs FROM Tools", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsDT);
                }
            }
        }

        #endregion

        #region ToolsType
        public void AddToolsType(int ToolsGroupID, string Name)
        {
            DataRow NewRow = ToolsTypeDT.NewRow();

            NewRow["ToolsGroupID"] = ToolsGroupID;
            NewRow["ToolsTypeName"] = Name;

            ToolsTypeDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Инструмент: тип ToolsTypeName=" + Name + " добавлен");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsTypeID, ToolsGroupID, ToolsTypeName FROM ToolsType", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsTypeDT);
                    ToolsTypeDT.Clear();
                    DA.Fill(ToolsTypeDT);
                }
            }
        }

        public void EditToolsType(int ToolsTypeID, int NewToolsGroupID, string NewName)
        {
            DataRow[] EditRows = ToolsTypeDT.Select("ToolsTypeID = " + ToolsTypeID);
            if (EditRows.Count() > 0)
            {
                string ToolsTypeName = EditRows[0]["ToolsTypeName"].ToString();
                EditRows[0]["ToolsGroupID"] = NewToolsGroupID;
                EditRows[0]["ToolsTypeName"] = NewName;
                TechCatalogEvents.SaveEvents("Инструмент: тип " + ToolsTypeName + " отредактирован");
            }

            UpdateToolsTypes();
        }

        public void RemoveToolsType(int ToolsTypeID)
        {
            foreach (DataRow ToolsSubTypeRow in ToolsSubTypeDT.Select("ToolsTypeID = " + ToolsTypeID))
            {
                foreach (DataRow ToolsRow in ToolsDT.Select("ToolsSubTypeID = " + ToolsSubTypeRow["ToolsSubTypeID"]))
                {
                    foreach (DataRow TechCatalogToolsRow in TechCatalogToolsDT.Select("ToolsID = " + ToolsRow["ToolsID"]))
                        TechCatalogToolsRow.Delete();
                    ToolsRow.Delete();
                }
                ToolsSubTypeRow.Delete();
            }

            UpdateTechCatalogTools();
            UpdateTools();
            UpdateToolsSubTypes();

            DataRow[] Rows = ToolsTypeDT.Select("ToolsTypeID = " + ToolsTypeID);
            if (Rows.Count() > 0)
            {
                string ToolsTypeName = Rows[0]["ToolsTypeName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Инструмент: тип " + ToolsTypeName + " ToolsTypeID=" + ToolsTypeID + " удален");
            }

            UpdateToolsTypes();
        }

        private void UpdateToolsTypes()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ToolsTypeID, ToolsGroupID, ToolsTypeName FROM ToolsType", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsTypeDT);
                }
            }
        }

        #endregion

        public void DeleteCabFurDocType(int CabFurDocTypeID)
        {
            foreach (DataRow row in CabFurnitureDocumentTypesDT.Select("CabFurDocTypeID = " + CabFurDocTypeID))
                row.Delete();
        }

        public void SaveCabFurDocTypes()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurnitureDocumentTypes",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(CabFurnitureDocumentTypesDT);
                    CabFurnitureDocumentTypesDT.Clear();
                    DA.Fill(CabFurnitureDocumentTypesDT);
                }
            }
        }

        #region ToolsSubType
        public void AddToolsSubType(int ToolsTypeID, string Name, string Parametrs)
        {
            DataRow NewRow = ToolsSubTypeDT.NewRow();

            NewRow["ToolsTypeID"] = ToolsTypeID;
            NewRow["ToolsSubTypeName"] = Name;
            NewRow["Parametrs"] = Parametrs;

            ToolsSubTypeDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Инструмент: подтип ToolsSubTypeName=" + Name + " добавлен");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ToolsSubTypeID, ToolsTypeID, ToolsSubTypeName, Parametrs FROM ToolsSubType", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsSubTypeDT);
                    ToolsSubTypeDT.Clear();
                    DA.Fill(ToolsSubTypeDT);
                }
            }
        }

        public void EditToolsSubType(int ToolsSubTypeID, int NewToolsTypeID, string NewName, string NewParametrs)
        {
            DataRow[] EditRows = ToolsSubTypeDT.Select("ToolsSubTypeID = " + ToolsSubTypeID);
            if (EditRows.Count() > 0)
            {
                string ToolsSubTypeName = EditRows[0]["ToolsSubTypeName"].ToString();
                EditRows[0]["ToolsTypeID"] = NewToolsTypeID;
                EditRows[0]["ToolsSubTypeName"] = NewName;
                EditRows[0]["Parametrs"] = NewParametrs;
                TechCatalogEvents.SaveEvents("Инструмент: подтип " + ToolsSubTypeName + " отредактирован");
            }

            UpdateToolsSubTypes();
        }

        public void RemoveToolsSubType(int ToolsSubTypeID)
        {
            foreach (DataRow ToolsRow in ToolsDT.Select("ToolsSubTypeID = " + ToolsSubTypeID))
            {
                foreach (DataRow TechCatalogToolsRow in TechCatalogToolsDT.Select("ToolsID = " + ToolsRow["ToolsID"]))
                    TechCatalogToolsRow.Delete();
                ToolsRow.Delete();
            }

            UpdateTechCatalogTools();
            UpdateTools();

            DataRow[] Rows = ToolsSubTypeDT.Select("ToolsSubTypeID = " + ToolsSubTypeID);
            if (Rows.Count() > 0)
            {
                string ToolsSubTypeName = Rows[0]["ToolsSubTypeName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Инструмент: подтип " + ToolsSubTypeName + " ToolsSubTypeID=" + ToolsSubTypeID + " удален");
            }

            UpdateToolsSubTypes();
        }

        private void UpdateToolsSubTypes()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ToolsSubTypeID, ToolsTypeID, ToolsSubTypeName, Parametrs FROM ToolsSubType",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ToolsSubTypeDT);
                }
            }
        }

        #endregion

        #region TechStoreGroup
        public void AddTechStoreGroup(string Name)
        {
            DataRow NewRow = TechStoreGroupsDT.NewRow();
            NewRow["TechStoreGroupName"] = Name;
            TechStoreGroupsDT.Rows.Add(NewRow);
            TechCatalogEvents.SaveEvents("Склад: группа TechStoreGroupName=" + Name + " добавлена");
            UpdateTechStoreGroups();
            RefreshTechStoreGroups();
        }

        public void EditTechStoreGroup(int TechStoreGroupID, string Name)
        {
            DataRow[] EditRows = TechStoreGroupsDT.Select("TechStoreGroupID = " + TechStoreGroupID);
            if (EditRows.Count() > 0)
            {
                string TechStoreGroupName = EditRows[0]["TechStoreGroupName"].ToString();
                EditRows[0]["TechStoreGroupName"] = Name;
                TechCatalogEvents.SaveEvents("Склад: группа " + TechStoreGroupName + " TechStoreGroupID=" + TechStoreGroupID + " отредактирована");

                UpdateTechStoreGroups();
                RefreshTechStoreGroups();
            }
        }

        public void RefreshTechStoreGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreGroupID, TechStoreGroupName FROM TechStoreGroups",
                ConnectionStrings.CatalogConnectionString))
            {
                TechStoreGroupsDT.Clear();
                DA.Fill(TechStoreGroupsDT);
            }
        }

        public void RemoveTechStoreGroup(int TechStoreGroupID)
        {
            foreach (DataRow RowSubGroup in TechStoreSubGroupsDT.Select("TechStoreGroupID = " + TechStoreGroupID))
            {
                foreach (DataRow RowTechStore in TechStoreDT.Select("TechStoreSubGroupID = " + RowSubGroup["TechStoreSubGroupID"]))
                {
                    foreach (DataRow RowOperation in TechCatalogOperationsDetailDT.Select("TechStoreID = " + RowTechStore["TechStoreID"]))
                    {
                        foreach (DataRow RowStoreDetail in TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID = " + RowOperation["TechCatalogOperationsDetailID"]))
                            RowStoreDetail.Delete();
                        RemoveTechCatalogOperationsTerms(Convert.ToInt32(RowOperation["TechCatalogOperationsDetailID"]));
                        RowOperation.Delete();
                    }
                    RowTechStore.Delete();
                }
                RowSubGroup.Delete();
            }
            DataRow[] Rows = TechStoreGroupsDT.Select("TechStoreGroupID = " + TechStoreGroupID);
            if (Rows.Count() > 0)
            {
                string TechStoreGroupName = Rows[0]["TechStoreGroupName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Склад: группа " + TechStoreGroupName + " TechStoreGroupID=" + TechStoreGroupID + " удалена");
            }

            UpdateTechCatalogStoreDetails();
            UpdateTechCatalogOperationsDetails();
            UpdateTechStore();
            UpdateTechStoreSubGroups();
            UpdateTechStoreGroups();

            RefreshTechCatalogStoreDetails();
            RefreshTechCatalogOperationsDetail();
            RefreshTechStore();
            RefreshTechStoreSubGroups();
            RefreshTechStoreGroups();
        }

        private void UpdateTechStoreGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechStoreGroups",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechStoreGroupsDT);
                }
            }
        }
        #endregion

        #region TechStoreSubGroup

        public void AddTechStoreSubGroup(string Name, int TechStoreGroupID)
        {
            DataRow NewRow = TechStoreSubGroupsDT.NewRow();
            NewRow["TechStoreSubGroupName"] = Name;
            NewRow["TechStoreGroupID"] = TechStoreGroupID;
            TechStoreSubGroupsDT.Rows.Add(NewRow);
            TechCatalogEvents.SaveEvents("Склад: подгруппа TechStoreSubGroupName=" + Name + " добавлена");
            UpdateTechStoreSubGroups();
            RefreshTechStoreSubGroups();
        }

        public void EditTechStoreSubGroup(int TechStoreSubGroupID, string Name, string Notes, string Notes1, string Notes2)
        {
            DataRow[] EditRows = TechStoreSubGroupsDT.Select("TechStoreSubGroupID = " + TechStoreSubGroupID);
            if (EditRows.Count() > 0)
            {
                string TechStoreSubGroupName = EditRows[0]["TechStoreSubGroupName"].ToString();
                EditRows[0]["TechStoreSubGroupName"] = Name;
                EditRows[0]["Notes"] = Notes;
                EditRows[0]["Notes1"] = Notes1;
                EditRows[0]["Notes2"] = Notes2;
                TechCatalogEvents.SaveEvents("Склад: подгруппа " + TechStoreSubGroupName + " TechStoreSubGroupID=" + TechStoreSubGroupID + " отредактирована");

                UpdateTechStoreSubGroups();
                RefreshTechStoreSubGroups();
            }
        }

        public void RefreshTechStoreSubGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStoreSubGroups",
                ConnectionStrings.CatalogConnectionString))
            {
                TechStoreSubGroupsDT.Clear();
                //TechSubGroupsNamesDT.Clear();
                DA.Fill(TechStoreSubGroupsDT);
                //DA.Fill(TechSubGroupsNamesDT);
            }
        }

        public void RemoveTechStoreSubGroup(int TechStoreSubGroupID)
        {
            foreach (DataRow RowTechStore in TechStoreDT.Select("TechStoreSubGroupID = " + TechStoreSubGroupID))
            {
                foreach (DataRow RowOperation in TechCatalogOperationsDetailDT.Select("TechStoreID = " + RowTechStore["TechStoreID"]))
                {
                    foreach (DataRow RowStoreDetail in TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID = " + RowOperation["TechCatalogOperationsDetailID"]))
                        RowStoreDetail.Delete();
                    RemoveTechCatalogOperationsTerms(Convert.ToInt32(RowOperation["TechCatalogOperationsDetailID"]));
                    RowOperation.Delete();
                }
                RowTechStore.Delete();
            }
            DataRow[] Rows = TechStoreSubGroupsDT.Select("TechStoreSubGroupID = " + TechStoreSubGroupID);
            if (Rows.Count() > 0)
            {
                string TechStoreSubGroupName = Rows[0]["TechStoreSubGroupName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Склад: подгруппа " + TechStoreSubGroupName + " TechStoreSubGroupID=" + TechStoreSubGroupID + " удалена");
            }

            UpdateTechCatalogStoreDetails();
            UpdateTechCatalogOperationsDetails();
            UpdateTechStore();
            UpdateTechStoreSubGroups();

            RefreshTechCatalogStoreDetails();
            RefreshTechCatalogOperationsDetail();
            RefreshTechStore();
            RefreshTechStoreSubGroups();
        }

        private void UpdateTechStoreSubGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechStoreSubGroups",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechStoreSubGroupsDT);
                }
            }
        }
        #endregion

        #region TechStore

        public void AddTechStore(string Name, int TechStoreSubGroupID)
        {
            DataRow NewRow = TechStoreDT.NewRow();
            NewRow["TechStoreName"] = Name;
            NewRow["TechStoreSubGroupID"] = TechStoreSubGroupID;
            TechStoreDT.Rows.Add(NewRow);
            TechCatalogEvents.SaveEvents("Склад: наименование TechStoreName=" + Name + " добавлено");
            UpdateTechStore();
            RefreshTechStore();
        }

        public void EditTechStore(int TechStoreID, string Name)
        {
            DataRow[] EditRows = TechStoreDT.Select("TechStoreID = " + TechStoreID);
            if (EditRows.Count() > 0)
            {
                string TechStoreName = EditRows[0]["TechStoreName"].ToString();
                EditRows[0]["TechStoreName"] = Name;
                TechCatalogEvents.SaveEvents("Склад: наименование " + TechStoreName + " TechStoreID=" + TechStoreID + " отредактировано");

                UpdateTechStore();
                RefreshTechStore();
            }
        }

        public void RefreshTechStore()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreSubGroupID, TechStoreName, MeasureID, Notes FROM TechStore",
                ConnectionStrings.CatalogConnectionString))
            {
                TechStoreDT.Clear();
                DA.Fill(TechStoreDT);
            }
        }

        public void RemoveTechStore(int TechStoreID)
        {
            foreach (DataRow RowOperationGroup in TechCatalogOperationsDetailGroupsDT.Select("TechStoreID = " + TechStoreID))
            {
                foreach (DataRow RowOperation in TechCatalogOperationsDetailDT.Select("TechCatalogOperationsGroupID = " + RowOperationGroup["TechCatalogOperationsGroupID"]))
                {
                    RemoveOperationsDocuments(Convert.ToInt32(RowOperation["TechCatalogOperationsDetailID"]));
                    foreach (DataRow RowStoreDetail in TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID = " + RowOperation["TechCatalogOperationsDetailID"]))
                        RowStoreDetail.Delete();
                    RemoveTechCatalogOperationsTerms(Convert.ToInt32(RowOperation["TechCatalogOperationsDetailID"]));
                    RowOperation.Delete();
                }
                RowOperationGroup.Delete();
            }
            DataRow[] Rows = TechStoreDT.Select("TechStoreID = " + TechStoreID);
            if (Rows.Count() > 0)
            {
                string TechStoreName = Rows[0]["TechStoreName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Склад: наименование " + TechStoreName + " TechStoreID=" + TechStoreID + " удалено");
            }
            UpdateTechCatalogOperationsDetailGroups();
            UpdateTechCatalogStoreDetails();
            UpdateTechCatalogOperationsDetails();
            UpdateTechStore();

            RefreshTechCatalogOperationsDetailGroups();
            RefreshTechCatalogStoreDetails();
            RefreshTechCatalogOperationsDetail();
            RefreshTechStore();
            RefreshOperationsDocuments();
        }

        private void UpdateTechStore()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 TechStoreID, TechStoreSubGroupID, TechStoreName FROM TechStore",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechStoreDT);
                }
            }
        }
        #endregion

        #region TechCatalogOperationsDetail

        public void AddTechCatalogOperationsDetail(int TechStoreID, int[] MachinesOperationID, int TechCatalogOperationsGroupID)
        {
            int SerialNumber = 0;
            if (TechCatalogOperationsDetailDT.DefaultView.Count > 0)
                SerialNumber = Convert.ToInt32(TechCatalogOperationsDetailDT.Compute("MAX(SerialNumber)", "TechCatalogOperationsGroupID = " + TechCatalogOperationsGroupID));
            for (int i = 0; i < MachinesOperationID.Count(); i++)
            {
                DataRow NewRow = TechCatalogOperationsDetailDT.NewRow();

                NewRow["MachinesOperationID"] = MachinesOperationID[i];
                NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                //NewRow["GroupName"] = GroupName;
                NewRow["SerialNumber"] = SerialNumber + 1;

                TechCatalogOperationsDetailDT.Rows.Add(NewRow);
                SerialNumber = Convert.ToInt32(TechCatalogOperationsDetailDT.Compute("MAX(SerialNumber)", "TechCatalogOperationsGroupID = " + TechCatalogOperationsGroupID));
                TechCatalogEvents.SaveEvents("Склад: операция MachinesOperationID=" + MachinesOperationID[i] + " прикреплена к наименованию TechStoreID=" + TechStoreID);
            }

            UpdateTechCatalogOperationsDetails();
            RefreshTechCatalogOperationsDetail();
        }

        public void AddTechCatalogOperationsDetailGroup(int TechStoreID)
        {
            int GroupNumber = 0;
            if (TechCatalogOperationsDetailGroupsDT.Select("TechStoreID=" + TechStoreID).Count() > 0)
                GroupNumber = Convert.ToInt32(TechCatalogOperationsDetailGroupsDT.Compute("MAX(GroupNumber)", "TechStoreID = " + TechStoreID));
            DataRow NewRow = TechCatalogOperationsDetailGroupsDT.NewRow();
            NewRow["GroupNumber"] = GroupNumber + 1;
            NewRow["TechStoreID"] = TechStoreID;
            TechCatalogOperationsDetailGroupsDT.Rows.Add(NewRow);
            UpdateTechCatalogOperationsDetailGroups();
            RefreshTechCatalogOperationsDetailGroups();
        }

        public bool CanDeleteOperationsGroup(int GroupNumber)
        {
            return TechCatalogOperationsDetailGroupsDT.Select("GroupNumber = " + GroupNumber).Count() > 0;
        }

        public void ChangeGroupNumber(int OldOperationsGroupID, int NewOperationsGroupID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsGroups" +
                " WHERE TechCatalogOperationsGroupID = " + OldOperationsGroupID + " OR TechCatalogOperationsGroupID = " + NewOperationsGroupID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            object GroupNumber1 = null;
                            object GroupNumber2 = null;

                            DataRow[] rows = DT.Select("TechCatalogOperationsGroupID=" + OldOperationsGroupID);
                            if (rows.Count() > 0)
                            {
                                GroupNumber1 = rows[0]["GroupNumber"];
                                //GroupName1 = rows[0]["GroupName"];
                            }
                            rows = DT.Select("TechCatalogOperationsGroupID=" + NewOperationsGroupID);
                            if (rows.Count() > 0)
                            {
                                GroupNumber2 = rows[0]["GroupNumber"];
                                //GroupName2 = rows[0]["GroupName"];
                            }

                            rows = DT.Select("TechCatalogOperationsGroupID=" + OldOperationsGroupID);
                            if (rows.Count() > 0)
                            {
                                rows[0]["GroupNumber"] = GroupNumber2;
                                //rows[0]["GroupName"] = GroupName2;
                            }
                            rows = DT.Select("TechCatalogOperationsGroupID=" + NewOperationsGroupID);
                            if (rows.Count() > 0)
                            {
                                rows[0]["GroupNumber"] = GroupNumber1;
                                //rows[0]["GroupName"] = GroupName1;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ChangeSerialNumber(int FirstTechCatalogOperationsDetailID, int SecondTechCatalogOperationsDetailID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail" +
                " WHERE (TechCatalogOperationsDetailID = " + FirstTechCatalogOperationsDetailID + " OR TechCatalogOperationsDetailID = " + SecondTechCatalogOperationsDetailID + ")",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            int FirstSerialNumber = 0;
                            int SecondSerialNumber = 0;
                            DataRow[] rows1 = DT.Select("TechCatalogOperationsDetailID = " + FirstTechCatalogOperationsDetailID);
                            if (rows1.Count() > 0)
                            {
                                FirstSerialNumber = Convert.ToInt32(rows1[0]["SerialNumber"]);
                            }
                            DataRow[] rows2 = DT.Select("TechCatalogOperationsDetailID = " + SecondTechCatalogOperationsDetailID);
                            if (rows2.Count() > 0)
                            {
                                SecondSerialNumber = Convert.ToInt32(rows2[0]["SerialNumber"]);
                            }

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (Convert.ToInt32(DT.Rows[i]["TechCatalogOperationsDetailID"]) == SecondTechCatalogOperationsDetailID)
                                {
                                    DT.Rows[i]["SerialNumber"] = FirstSerialNumber;
                                    continue;
                                }
                                if (Convert.ToInt32(DT.Rows[i]["TechCatalogOperationsDetailID"]) == FirstTechCatalogOperationsDetailID)
                                    DT.Rows[i]["SerialNumber"] = SecondSerialNumber;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void CopyGroupOperationsDetail(int TechStoreID, int TechCatalogOperationsGroupID, int NewGroupNumber)
        {
            DataTable DT1 = new DataTable();
            SqlDataAdapter DA1 = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail" +
                " WHERE TechCatalogOperationsGroupID = " + TechCatalogOperationsGroupID,
                ConnectionStrings.CatalogConnectionString);
            SqlCommandBuilder CB1 = new SqlCommandBuilder(DA1);
            DA1.Fill(DT1);

            DataTable DT2 = new DataTable();
            SqlDataAdapter DA2 = new SqlDataAdapter("SELECT TOP 1 * FROM TechCatalogOperationsGroups" +
                " ORDER BY TechCatalogOperationsGroupID DESC",
                ConnectionStrings.CatalogConnectionString);
            SqlCommandBuilder CB2 = new SqlCommandBuilder(DA2);
            DA2.Fill(DT2);
            DataRow NewRow0 = DT2.NewRow();
            NewRow0["TechStoreID"] = TechStoreID;
            NewRow0["GroupNumber"] = NewGroupNumber;
            DT2.Rows.Add(NewRow0);
            DA2.Update(DT2);
            DT2.Clear();
            DA2.Fill(DT2);

            int NewTechCatalogOperationsGroupID = 0;
            if (DT2.Rows.Count > 0)
                NewTechCatalogOperationsGroupID = Convert.ToInt32(DT2.Rows[0]["TechCatalogOperationsGroupID"]);

            DataTable DT3 = new DataTable();
            SqlDataAdapter DA3 = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail" +
                " WHERE TechCatalogOperationsGroupID = " + NewTechCatalogOperationsGroupID,
                ConnectionStrings.CatalogConnectionString);
            SqlCommandBuilder CB3 = new SqlCommandBuilder(DA3);
            DA3.Fill(DT3);

            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DataRow NewRow = DT3.NewRow();
                NewRow["MachinesOperationID"] = DT1.Rows[i]["MachinesOperationID"];
                NewRow["SerialNumber"] = DT1.Rows[i]["SerialNumber"];
                NewRow["TechCatalogOperationsGroupID"] = NewTechCatalogOperationsGroupID;
                NewRow["IsPerform"] = DT1.Rows[i]["IsPerform"];
                DT3.Rows.Add(NewRow);
                DA3.Update(DT3);
                DT3.Clear();
                DA3.Fill(DT3);
                int TechCatalogOperationsDetailID = Convert.ToInt32(DT3.Rows[DT3.Rows.Count - 1]["TechCatalogOperationsDetailID"]);

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsTerms" +
                    " WHERE TechCatalogOperationsDetailID = " + DT1.Rows[i]["TechCatalogOperationsDetailID"],
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                int RowsCount1 = DT.Rows.Count;
                                for (int j = 0; j < RowsCount1; j++)
                                {
                                    DataRow NewRow1 = DT.NewRow();
                                    NewRow1["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                                    NewRow1["Parameter"] = DT.Rows[j]["Parameter"];
                                    NewRow1["MathSymbol"] = DT.Rows[j]["MathSymbol"];
                                    NewRow1["Term"] = DT.Rows[j]["Term"];
                                    DT.Rows.Add(NewRow1);
                                }
                                DA.Update(DT);
                            }
                        }
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogTools" +
                    " WHERE TechCatalogOperationsDetailID = " + DT1.Rows[i]["TechCatalogOperationsDetailID"],
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                int RowsCount1 = DT.Rows.Count;
                                for (int j = 0; j < RowsCount1; j++)
                                {
                                    DataRow NewRow1 = DT.NewRow();
                                    NewRow1["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                                    NewRow1["ToolsID"] = DT.Rows[j]["ToolsID"];
                                    NewRow1["Count"] = DT.Rows[j]["Count"];
                                    NewRow1["GroupNumber"] = DT.Rows[j]["GroupNumber"];
                                    //NewRow1["GroupName"] = DT.Rows[j]["GroupName"];
                                    DT.Rows.Add(NewRow1);
                                }
                                DA.Update(DT);
                            }
                        }
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetail" +
                                                              " WHERE TechCatalogOperationsDetailID = " +
                                                              DT1.Rows[i]["TechCatalogOperationsDetailID"],
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            int RowsCount1 = DT.Rows.Count;
                            for (int j = 0; j < RowsCount1; j++)
                            {
                                CopyTechStoreDetail(Convert.ToInt32(DT.Rows[j]["TechCatalogStoreDetailID"]),
                                    TechCatalogOperationsDetailID);
                            }
                        }
                    }
                }
            }
            DT1.Dispose();
            DT2.Dispose();
            DT3.Dispose();

            DA1.Dispose();
            DA2.Dispose();
            DA3.Dispose();

            CB1.Dispose();
            CB2.Dispose();
            CB3.Dispose();
        }

        public void CopyOperationsDetail(List<int> TechCatalogOperationsIDs, int NewGroupOperationsDetailID)
        {
            for (int i = 0; i < TechCatalogOperationsIDs.Count(); i++)
            {
                int TechCatalogOperationsDetailID = TechCatalogOperationsIDs[i];
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail" +
                    " WHERE TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = DT.Rows[0].ItemArray;
                                NewRow["TechCatalogOperationsGroupID"] = NewGroupOperationsDetailID;
                                DT.Rows.Add(NewRow);
                                DA.Update(DT);
                            }
                        }
                    }
                }
                int NewTechCatalogOperationsDetailID = 0;

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechCatalogOperationsDetailID FROM TechCatalogOperationsDetail ORDER BY TechCatalogOperationsDetailID DESC",
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                            NewTechCatalogOperationsDetailID = Convert.ToInt32(DT.Rows[0]["TechCatalogOperationsDetailID"]);
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetail" +
                                                              " WHERE TechCatalogOperationsDetailID = " +
                                                              TechCatalogOperationsDetailID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int j = 0; j < DT.Rows.Count; j++)
                                CopyTechStoreDetail(Convert.ToInt32(DT.Rows[j]["TechCatalogStoreDetailID"]),
                                    NewTechCatalogOperationsDetailID);
                        }
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsTerms" +
                    " WHERE TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                int RowsCount1 = DT.Rows.Count;
                                for (int j = 0; j < RowsCount1; j++)
                                {
                                    DataRow NewRow = DT.NewRow();
                                    NewRow.ItemArray = DT.Rows[j].ItemArray;
                                    NewRow["TechCatalogOperationsDetailID"] = NewTechCatalogOperationsDetailID;
                                    DT.Rows.Add(NewRow);
                                }
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        public void CopyTechStoreDetail(int TechCatalogStoreDetailID, int NewTechCatalogOperationsDetailID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetail" +
                                                          " WHERE TechCatalogStoreDetailID = " +
                                                          TechCatalogStoreDetailID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DataRow NewRow = DT.NewRow();
                            NewRow.ItemArray = DT.Rows[0].ItemArray;
                            NewRow["TechCatalogOperationsDetailID"] = NewTechCatalogOperationsDetailID;
                            DT.Rows.Add(NewRow);
                            DA.Update(DT);
                        }
                    }
                }
            }
            int NewTechCatalogStoreDetailID = 0;

            using (
                SqlDataAdapter DA =
                    new SqlDataAdapter(
                        "SELECT TechCatalogStoreDetailID FROM TechCatalogStoreDetail ORDER BY TechCatalogStoreDetailID DESC",
                        ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        NewTechCatalogStoreDetailID = Convert.ToInt32(DT.Rows[0]["TechCatalogStoreDetailID"]);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetailTerms" +
                                                          " WHERE TechCatalogStoreDetailID = " +
                                                          TechCatalogStoreDetailID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            int RowsCount1 = DT.Rows.Count;
                            for (int j = 0; j < RowsCount1; j++)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = DT.Rows[j].ItemArray;
                                NewRow["TechCatalogStoreDetailID"] = NewTechCatalogStoreDetailID;
                                DT.Rows.Add(NewRow);
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void CopyTechStoreDetail(List<int> TechCatalogStoreDetailIDs, int NewTechCatalogOperationsDetailID)
        {
            for (int i = 0; i < TechCatalogStoreDetailIDs.Count(); i++)
            {
                int TechCatalogStoreDetailID = TechCatalogStoreDetailIDs[i];

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetail" +
                    " WHERE TechCatalogStoreDetailID = " + TechCatalogStoreDetailID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = DT.Rows[0].ItemArray;
                                NewRow["TechCatalogOperationsDetailID"] = NewTechCatalogOperationsDetailID;
                                DT.Rows.Add(NewRow);
                                DA.Update(DT);
                            }
                        }
                    }
                }
                int NewTechCatalogStoreDetailID = 0;

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechCatalogStoreDetailID FROM TechCatalogStoreDetail ORDER BY TechCatalogStoreDetailID DESC",
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                            NewTechCatalogStoreDetailID = Convert.ToInt32(DT.Rows[0]["TechCatalogStoreDetailID"]);
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetailTerms" +
                    " WHERE TechCatalogStoreDetailID = " + TechCatalogStoreDetailID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                int RowsCount1 = DT.Rows.Count;
                                for (int j = 0; j < RowsCount1; j++)
                                {
                                    DataRow NewRow = DT.NewRow();
                                    NewRow.ItemArray = DT.Rows[j].ItemArray;
                                    NewRow["TechCatalogStoreDetailID"] = NewTechCatalogStoreDetailID;
                                    DT.Rows.Add(NewRow);
                                }
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        public void MoveTechStore(List<int> TechStoreIDs, int NewTechStoreSubGroupID)
        {
            for (int i = 0; i < TechStoreIDs.Count(); i++)
            {
                var TechStoreID = TechStoreIDs[i];

                using (var da = new SqlDataAdapter("SELECT * FROM TechStore" +
                    " WHERE TechStoreID = " + TechStoreID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (new SqlCommandBuilder(da))
                    {
                        using (var dt = new DataTable())
                        {
                            if (da.Fill(dt) <= 0) continue;
                            dt.Rows[0]["TechStoreSubGroupID"] = NewTechStoreSubGroupID;
                            da.Update(dt);
                        }
                    }
                }
            }
        }

        public void CopyTechStore(List<int> TechStoreIDs, int NewTechStoreSubGroupID)
        {
            for (int i = 0; i < TechStoreIDs.Count(); i++)
            {
                int TechStoreID = TechStoreIDs[i];

                using (var da = new SqlDataAdapter("SELECT * FROM TechStore" +
                    " WHERE TechStoreID = " + TechStoreID,
                    ConnectionStrings.CatalogConnectionString))
                {
                    using (new SqlCommandBuilder(da))
                    {
                        using (var dt = new DataTable())
                        {
                            if (da.Fill(dt) > 0)
                            {
                                DataRow NewRow = dt.NewRow();
                                NewRow.ItemArray = dt.Rows[0].ItemArray;
                                NewRow["TechStoreSubGroupID"] = NewTechStoreSubGroupID;
                                dt.Rows.Add(NewRow);
                                da.Update(dt);
                            }
                        }
                    }
                }
            }
        }

        public void RefreshTechCatalogOperationsDetail()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail ORDER BY SerialNumber", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    TechCatalogOperationsDetailDT.Clear();
                    DA.Fill(TechCatalogOperationsDetailDT);
                }
            }
            foreach (DataRow Row in TechCatalogOperationsDetailDT.Rows)
            {
                DataRow[] SelectRows = MachinesOperationsDT.Select("MachinesOperationID = " + Row["MachinesOperationID"]);
                if (SelectRows.Count() > 0)
                    Row["MachineID"] = SelectRows[0]["MachineID"];
                else
                    Row["MachineID"] = DBNull.Value;
            }
        }

        public bool RefreshTechCatalogOperationsDetailGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsGroups ORDER BY GroupNumber", ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogOperationsDetailGroupsDT.Clear();
                DA.Fill(TechCatalogOperationsDetailGroupsDT);
            }

            return TechCatalogOperationsDetailGroupsDT.Rows.Count > 0;
        }

        public void RemoveTechCatalogOperationsDetail(int TechCatalogOperationsDetailID)
        {
            RemoveTechCatalogOperationsTerms(TechCatalogOperationsDetailID);

            foreach (DataRow RowStoreDetail in TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID))
                RowStoreDetail.Delete();

            foreach (DataRow TechCatalogToolsRow in TechCatalogToolsDT.Select("TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID))
                TechCatalogToolsRow.Delete();

            DataRow[] Rows = TechCatalogOperationsDetailDT.Select("TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID);
            if (Rows.Count() > 0)
            {
                int MachinesOperationID = Convert.ToInt32(Rows[0]["MachinesOperationID"]);
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Склад: операция MachinesOperationID=" + MachinesOperationID + " откреплена от наименования");
            }

            UpdateTechCatalogTools();
            UpdateTechCatalogOperationsDetails();
            UpdateTechCatalogStoreDetails();

            RefreshTechCatalogTools();
            RefreshTechCatalogOperationsDetail();
            RefreshTechCatalogStoreDetails();
        }

        public void RemoveTechCatalogOperationsGroup(int TechCatalogOperationsGroupID)
        {
            foreach (DataRow RowDetailGroup in TechCatalogOperationsDetailGroupsDT.Select("TechCatalogOperationsGroupID = " + TechCatalogOperationsGroupID))
            {
                DataRow[] GRows = TechCatalogOperationsDetailDT.Select("TechCatalogOperationsGroupID = " + TechCatalogOperationsGroupID);
                foreach (DataRow item in GRows)
                {
                    int TechCatalogOperationsDetailID = Convert.ToInt32(item["TechCatalogOperationsDetailID"]);

                    RemoveTechCatalogOperationsTerms(TechCatalogOperationsDetailID);

                    foreach (DataRow RowStoreDetail in TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID))
                        RowStoreDetail.Delete();

                    foreach (DataRow TechCatalogToolsRow in TechCatalogToolsDT.Select("TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID))
                        TechCatalogToolsRow.Delete();

                    RemoveOperationsDocuments(TechCatalogOperationsDetailID);

                    DataRow[] Rows = TechCatalogOperationsDetailDT.Select("TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID);
                    if (Rows.Count() > 0)
                    {
                        int MachinesOperationID = Convert.ToInt32(Rows[0]["MachinesOperationID"]);
                        Rows[0].Delete();
                        TechCatalogEvents.SaveEvents("Склад: операция MachinesOperationID=" + MachinesOperationID + " откреплена от наименования");
                    }

                }

                RowDetailGroup.Delete();
            }
            UpdateTechCatalogOperationsDetailGroups();
            UpdateTechCatalogTools();
            UpdateTechCatalogOperationsDetails();
            UpdateTechCatalogStoreDetails();

            RefreshTechCatalogOperationsDetailGroups();
            RefreshTechCatalogTools();
            RefreshTechCatalogOperationsDetail();
            RefreshTechCatalogStoreDetails();
            RefreshOperationsDocuments();
        }

        public void RemoveTechCatalogOperationsTerms(int TechCatalogOperationsDetailID)
        {
            string SelectCommand = "DELETE FROM TechCatalogOperationsTerms" +
                " WHERE TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
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

        //public string GetGroupName(int GroupNumber)
        //{
        //    DataRow[] rows = TechCatalogOperationsDetailGroupsDT.Select("GroupNumber = " + GroupNumber);
        //    if (rows.Count() > 0)
        //        return rows[0]["GroupName"].ToString();
        //    return string.Empty;
        //}
        public void UpdateTechCatalogOperationsDetailGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsGroups ORDER BY GroupNumber", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechCatalogOperationsDetailGroupsDT);
                    TechCatalogOperationsDetailGroupsDT.Clear();
                    DA.Fill(TechCatalogOperationsDetailGroupsDT);
                }
            }
        }
        public void UpdateTechCatalogOperationsDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogOperationsDetail ORDER BY SerialNumber", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechCatalogOperationsDetailDT);
                    TechCatalogOperationsDetailDT.Clear();
                    DA.Fill(TechCatalogOperationsDetailDT);
                }
            }
            RefreshTechCatalogOperationsDetail();
        }
        #endregion

        #region TechCatalogStoreDetail
        public void AddTechCatalogStoreDetail(int TechCatalogOperationsDetailID, int[] TechStoreID, object[] IsHalfStuff1, object[] Length, object[] Height, object[] Width)
        {
            for (int i = 0; i < TechStoreID.Count(); i++)
            {
                DataRow NewRow = TechCatalogStoreDetailDT.NewRow();

                NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                NewRow["TechStoreID"] = TechStoreID[i];
                NewRow["IsSubGroup"] = false;
                NewRow["IsHalfStuff1"] = IsHalfStuff1[i];
                NewRow["IsHalfStuff2"] = IsHalfStuff1[i];
                NewRow["Length"] = Length[i];
                NewRow["Height"] = Height[i];
                NewRow["Width"] = Width[i];

                TechCatalogStoreDetailDT.Rows.Add(NewRow);
                TechCatalogEvents.SaveEvents("Склад: материал TechStoreID=" + TechStoreID[i] + " прикреплен к операции TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            }

            UpdateTechCatalogStoreDetails();
            RefreshTechCatalogStoreDetails();
        }

        public void AddTechCatalogStoreDetail(int TechCatalogOperationsDetailID, int TechStoreSubGroupID)
        {
            DataRow NewRow = TechCatalogStoreDetailDT.NewRow();

            NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
            NewRow["TechStoreID"] = TechStoreSubGroupID;
            NewRow["IsSubGroup"] = true;
            //NewRow["Length"] = DBNull.Value;
            //NewRow["Height"] = DBNull.Value;
            //NewRow["Width"] = DBNull.Value;

            TechCatalogStoreDetailDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Склад: подгруппа TechStoreID=" + TechStoreSubGroupID + " прикреплена к операции TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            UpdateTechCatalogStoreDetails();
            RefreshTechCatalogStoreDetails();
        }

        public void RefreshTechCatalogStoreDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogStoreDetail" +
                " ORDER BY GroupA, GroupB, GroupC", ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogStoreDetailDT.Clear();
                DA.Fill(TechCatalogStoreDetailDT);
            }
            foreach (DataRow Row in TechCatalogStoreDetailDT.Rows)
            {
                if (Convert.ToBoolean(Row["IsSubGroup"]))
                {
                    Row["TechName"] = " - ";
                    DataRow[] Rows3 = TechStoreDT.Select("TechStoreSubGroupID = " + Row["TechStoreID"]);
                    if (Rows3.Count() > 0)
                    {
                        DataRow[] Rows4 = MeasuresDT.Select("MeasureID = " + Rows3[0]["MeasureID"]);
                        if (Rows4.Count() > 0)
                        {
                            Row["Measure"] = Rows4[0]["Measure"];
                        }
                    }
                }
                else
                {
                    int TechStoreID = Convert.ToInt32(Row["TechStoreID"]);
                    DataRow[] Rows1 = TechStoreDT.Select("TechStoreID = " + Row["TechStoreID"]);
                    if (Rows1.Count() > 0)
                    {
                        DataRow[] Rows4 = MeasuresDT.Select("MeasureID = " + Rows1[0]["MeasureID"]);
                        if (Rows4.Count() > 0)
                        {
                            Row["Measure"] = Rows4[0]["Measure"];
                        }
                        Row["TechName"] = Rows1[0]["TechStoreName"];
                        Row["Notes"] = Rows1[0]["Notes"];
                    }
                }
            }
        }

        public void RemoveTechCatalogStoreDetail(int TechCatalogStoreDetailID)
        {
            DataRow[] Rows = TechCatalogStoreDetailDT.Select("TechCatalogStoreDetailID = " + TechCatalogStoreDetailID);
            if (Rows.Count() > 0)
            {
                int TechStoreID = Convert.ToInt32(Rows[0]["TechStoreID"]);
                int TechCatalogOperationsDetailID = Convert.ToInt32(Rows[0]["TechCatalogOperationsDetailID"]);

                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Склад: материал TechStoreID=" + TechStoreID + " откреплен от операции TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            }
            UpdateTechCatalogStoreDetails();
            RefreshTechCatalogStoreDetails();
        }

        public void UpdateTechCatalogStoreDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechCatalogStoreDetail", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechCatalogStoreDetailDT);
                }
            }
        }
        #endregion

        #region Sector
        public void AddSector(int FactoryID, string Name)
        {
            DataRow NewRow = SectorsDT.NewRow();

            NewRow["SectorName"] = Name;
            NewRow["FactoryID"] = FactoryID;

            SectorsDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Операции: участок SectorName=" + Name + " добавлен");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SectorID, FactoryID, SectorName FROM Sectors", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(SectorsDT);
                    SectorsDT.Clear();
                    DA.Fill(SectorsDT);
                }
            }
        }

        public void EditSector(int SectorID, int FactoryID, string NewName)
        {
            DataRow[] Rows = SectorsDT.Select("SectorID = " + SectorID);
            if (Rows.Count() > 0)
            {
                string SectorName = Rows[0]["SectorName"].ToString();
                Rows[0]["SectorName"] = NewName;
                Rows[0]["SectorID"] = SectorID;
                TechCatalogEvents.SaveEvents("Инструмент: участок " + SectorName + " отредактирован");
            }

            UpdateSectors();
        }

        public void RemoveSector(int SectorID)
        {
            foreach (DataRow SubSectorRow in SubSectorsDT.Select("SectorID = " + SectorID))
            {
                foreach (DataRow MachineRow in MachinesDT.Select("SubSectorID = " + SubSectorRow["SubSectorID"]))
                {
                    foreach (DataRow OperationRow in MachinesOperationsDT.Select("MachineID = " + MachineRow["MachineID"]))
                    {
                        OperationRow["MachineID"] = DBNull.Value;
                    }
                    MachineRow.Delete();
                }
                SubSectorRow.Delete();
            }

            UpdateMachinesOperations();

            UpdateMachines();

            UpdateSubSectors();

            DataRow[] Rows = SectorsDT.Select("SectorID = " + SectorID);
            if (Rows.Count() > 0)
            {
                string SectorName = Rows[0]["SectorName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Операции: участок " + SectorName + " SectorID=" + SectorID + " удален");
            }
            UpdateSectors();
        }

        public void UpdateSectorNameColumn(ref PercentageDataGrid TechCatalogOperationsDetailGrid)
        {
            ((DataGridViewComboBoxColumn)TechCatalogOperationsDetailGrid.Columns["SectorName"]).DataSource = SectorsDT.Copy();
        }
        private void UpdateSectors()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SectorID, FactoryID, SectorName FROM Sectors", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(SectorsDT);
                }
            }
        }

        #endregion

        #region SubSector
        public void AddSubSector(int SectorID, string Name)
        {
            DataRow NewRow = SubSectorsDT.NewRow();

            NewRow["SubSectorName"] = Name;
            NewRow["SectorID"] = SectorID;

            SubSectorsDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Операции: подучасток SubSectorName=" + Name + " добавлен");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SubSectorID, SectorID, SubSectorName FROM SubSectors", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(SubSectorsDT);
                    SubSectorsDT.Clear();
                    DA.Fill(SubSectorsDT);
                }
            }
        }

        public void EditSubSector(int SubSectorID, int SectorID, string NewName)
        {
            DataRow[] Rows = SubSectorsDT.Select("SubSectorID = " + SubSectorID);
            if (Rows.Count() > 0)
            {
                string SubSectorName = Rows[0]["SubSectorName"].ToString();
                Rows[0]["SubSectorName"] = NewName;
                Rows[0]["SectorID"] = SectorID;
                TechCatalogEvents.SaveEvents("Инструмент: подучасток " + SubSectorName + " отредактирован");
            }

            UpdateSubSectors();
        }

        public void RemoveSubSector(int SubSectorID)
        {
            foreach (DataRow Row in MachinesDT.Select("SubSectorID = " + SubSectorID))
            {
                foreach (DataRow OperationRow in MachinesOperationsDT.Select("MachineID = " + Row["MachineID"]))
                {
                    OperationRow["MachineID"] = DBNull.Value;
                }
                Row.Delete();
            }

            UpdateMachinesOperations();

            UpdateMachines();

            DataRow[] Rows = SubSectorsDT.Select("SubSectorID = " + SubSectorID);
            if (Rows.Count() > 0)
            {
                string SubSectorName = Rows[0]["SubSectorName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Операции: подучасток " + SubSectorName + " SubSectorID=" + SubSectorID + " удален");
            }
            UpdateSubSectors();
        }

        public void UpdateSubSectorNameColumn(ref PercentageDataGrid TechCatalogOperationsDetailGrid)
        {
            ((DataGridViewComboBoxColumn)TechCatalogOperationsDetailGrid.Columns["SubSectorName"]).DataSource = SubSectorsDT.Copy();
        }
        private void UpdateSubSectors()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SubSectorID, SectorID, SubSectorName FROM SubSectors", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(SubSectorsDT);
                }
            }
            RefreshTechCatalogOperationsDetail();
        }
        #endregion

        #region Machine
        public void AddMachine(int SubSectorID, string Name)
        {
            DataRow NewRow = MachinesDT.NewRow();

            NewRow["MachineName"] = Name;
            NewRow["SubSectorID"] = SubSectorID;

            MachinesDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Станки: станок MachineName=" + Name + " добавлен");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachineID, SubSectorID, MachineName FROM Machines", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MachinesDT);
                    MachinesDT.Clear();
                    DA.Fill(MachinesDT);
                }
                AllMachinesBS.DataSource = MachinesDT.Copy();
            }
        }

        public void EditMachine(int MachineID, int SubSectorID, string NewName)
        {
            DataRow[] Rows = MachinesDT.Select("MachineID = " + MachineID);
            if (Rows.Count() > 0)
            {
                string MachineName = Rows[0]["MachineName"].ToString();
                Rows[0]["MachineName"] = NewName;
                Rows[0]["SubSectorID"] = SubSectorID;
                TechCatalogEvents.SaveEvents("Станки: станок " + MachineName + " отредактирован");
            }

            UpdateMachines();
        }

        public void EditMachineValueParametrs(int MachineID, string ValueParametrs)
        {
            DataRow EditRow = MachinesDT.Select("MachineID = " + MachineID)[0];

            EditRow["ValueParametrs"] = ValueParametrs;

            UpdateMachines();
        }

        public void RemoveMachine(int MachineID)
        {
            foreach (DataRow Row in MachinesOperationsDT.Select("MachineID = " + MachineID))
            {
                Row["MachineID"] = DBNull.Value;
            }

            UpdateMachinesOperations();

            DataRow[] Rows = MachinesDT.Select("MachineID = " + MachineID);
            if (Rows.Count() > 0)
            {
                string MachineName = Rows[0]["MachineName"].ToString();
                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Станки: станок " + MachineName + " MachineID=" + MachineID + " удален");
            }
            UpdateMachines();
        }

        public void UpdateMachineNameColumn(ref PercentageDataGrid TechCatalogOperationsDetailGrid)
        {
            ((DataGridViewComboBoxColumn)TechCatalogOperationsDetailGrid.Columns["MachineName"]).DataSource = MachinesDT.Copy();
        }
        private void UpdateMachines()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachineID, SubSectorID, MachineName, Parametrs, ValueParametrs FROM Machines", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MachinesDT);
                }
                AllMachinesBS.DataSource = MachinesDT.Copy();
            }
            RefreshTechCatalogOperationsDetail();
        }

        #endregion

        #region MachinesOperation
        public void AddMachinesOperation(string Name, decimal Norm, decimal PreparatoryNorm, int MeasureID, string Article, int PositionID, int Rank,
            int PositionID2, int Rank2, int CabFurDocTypeID, int CabFurAlgorithmID)
        {
            DataRow NewRow = MachinesOperationsDT.NewRow();

            NewRow["MachinesOperationName"] = Name;
            NewRow["MachineID"] = DBNull.Value;
            NewRow["Norm"] = Norm;
            NewRow["PreparatoryNorm"] = PreparatoryNorm;
            NewRow["MeasureID"] = MeasureID;
            NewRow["Article"] = Article;
            NewRow["PositionID"] = PositionID;
            NewRow["Rank"] = Rank;
            NewRow["PositionID2"] = PositionID2;
            NewRow["Rank2"] = Rank2;
            NewRow["CabFurDocTypeID"] = CabFurDocTypeID;
            NewRow["CabFurAlgorithmID"] = CabFurAlgorithmID;

            MachinesOperationsDT.Rows.Add(NewRow);

            TechCatalogEvents.SaveEvents("Станки: операция MachinesOperationName=" + Name + " добавлена");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachinesOperations", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MachinesOperationsDT);
                    MachinesOperationsDT.Clear();
                    DA.Fill(MachinesOperationsDT);
                }
            }
            RefreshTechCatalogOperationsDetail();
        }

        public void EditCabFurDocType(int MachinesOperationID, int CabFurDocTypeID)
        {
            DataRow[] EditRows = MachinesOperationsDT.Select("MachinesOperationID = " + MachinesOperationID);
            if (EditRows.Count() > 0)
            {
                EditRows[0]["CabFurDocTypeID"] = CabFurDocTypeID;
            }

        }

        public void AddMachinesOperationToMachine(int MachineID, int MachinesOperationID)
        {
            DataRow[] Rows = MachinesOperationsDT.Select("MachinesOperationID = " + MachinesOperationID);
            if (Rows.Count() > 0)
            {
                string MachinesOperationName = Rows[0]["MachinesOperationName"].ToString();
                Rows[0]["MachineID"] = MachineID;
                TechCatalogEvents.SaveEvents("Станки: операция " + MachinesOperationName + " прикреплена к станку MachineID=" + MachineID);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachinesOperationID, MachineID, MachinesOperationName, Norm, PreparatoryNorm, MeasureID, Article FROM MachinesOperations", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MachinesOperationsDT);
                }
            }
            RefreshTechCatalogOperationsDetail();
        }

        public void EditMachinesOperation(int MachinesOperationID, string NewName, decimal NewNorm, decimal NewPreparatoryNorm, int NewMeasureID, string NewArticle,
            int PositionID, int Rank, int PositionID2, int Rank2, int CabFurDocTypeID, int CabFurAlgorithmID)
        {
            DataRow[] Rows = MachinesOperationsDT.Select("MachinesOperationID = " + MachinesOperationID);
            if (Rows.Count() > 0)
            {
                string MachinesOperationName = Rows[0]["MachinesOperationName"].ToString();
                Rows[0]["MachinesOperationName"] = NewName;
                Rows[0]["Norm"] = NewNorm;
                Rows[0]["PreparatoryNorm"] = NewPreparatoryNorm;
                Rows[0]["MeasureID"] = NewMeasureID;
                Rows[0]["Article"] = NewArticle;
                Rows[0]["PositionID"] = PositionID;
                Rows[0]["Rank"] = Rank;
                Rows[0]["PositionID2"] = PositionID2;
                Rows[0]["Rank2"] = Rank2;
                Rows[0]["CabFurDocTypeID"] = CabFurDocTypeID;
                Rows[0]["CabFurAlgorithmID"] = CabFurAlgorithmID;
                TechCatalogEvents.SaveEvents("Станки: операция " + MachinesOperationName + " отредактирована");
            }

            UpdateMachinesOperations();
            RefreshTechCatalogOperationsDetail();
        }

        public void RefreshMachinesOperations()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachinesOperations", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    MachinesOperationsDT.Clear();
                    DA.Fill(MachinesOperationsDT);
                }
            }
        }

        public void RemoveMachinesOperation(int MachinesOperationID)
        {
            foreach (DataRow RowOperation in TechCatalogOperationsDetailDT.Select("MachinesOperationID = " + MachinesOperationID))
            {
                foreach (DataRow RowStoreDetail in TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID = " + RowOperation["TechCatalogOperationsDetailID"]))
                    RowStoreDetail.Delete();
                RowOperation.Delete();
            }

            UpdateTechCatalogOperationsDetails();

            UpdateTechCatalogStoreDetails();

            DataRow[] Rows = MachinesOperationsDT.Select("MachinesOperationID = " + MachinesOperationID);
            if (Rows.Count() > 0)
            {
                string MachinesOperationName = Rows[0]["MachinesOperationName"].ToString();
                int MachineID = 0;
                if (Rows[0]["MachineID"] != DBNull.Value)
                    int.TryParse(Rows[0]["MachineID"].ToString(), out MachineID);

                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Станки: операция" + MachinesOperationName + " удалена со станка MachineID=" + MachineID);
            }
            UpdateMachinesOperations();
        }

        public void UpdateMachinesOperationNameColumn(ref PercentageDataGrid TechCatalogOperationsDetailGrid)
        {
            ((DataGridViewComboBoxColumn)TechCatalogOperationsDetailGrid.Columns["MachinesOperationName"]).DataSource = MachinesOperationsDT.Copy();
            ((DataGridViewComboBoxColumn)TechCatalogOperationsDetailGrid.Columns["MachinesOperationArticle"]).DataSource = MachinesOperationsDT.Copy();
        }
        public void UpdateMachinesOperations()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachinesOperations", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MachinesOperationsDT);
                }
            }
            RefreshTechCatalogOperationsDetail();
        }

        #endregion

        #region TechCatalogTools
        public void AddTechCatalogTools(int TechCatalogOperationsDetailID, int ToolsID, int Count)
        {
            DataRow NewRow = TechCatalogToolsDT.NewRow();

            NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
            NewRow["ToolsID"] = ToolsID;
            NewRow["Count"] = Count;

            TechCatalogToolsDT.Rows.Add(NewRow);
            TechCatalogEvents.SaveEvents("Склад: инструмент ToolsID=" + ToolsID + " прикреплен к операции TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            UpdateTechCatalogTools();
            RefreshTechCatalogTools();
        }

        public void RefreshTechCatalogTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechCatalogTools", ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogToolsDT.Clear();
                DA.Fill(TechCatalogToolsDT);
            }
        }

        public void RemoveTechCatalogTools(int TechCatalogToolsID)
        {
            DataRow[] Rows = TechCatalogToolsDT.Select("TechCatalogToolsID = " + TechCatalogToolsID);
            if (Rows.Count() > 0)
            {
                int ToolsID = 0;
                int TechCatalogOperationsDetailID = 0;
                if (Rows[0]["ToolsID"] != DBNull.Value)
                    int.TryParse(Rows[0]["ToolsID"].ToString(), out ToolsID);
                if (Rows[0]["TechCatalogOperationsDetailID"] != DBNull.Value)
                    int.TryParse(Rows[0]["TechCatalogOperationsDetailID"].ToString(), out TechCatalogOperationsDetailID);

                Rows[0].Delete();
                TechCatalogEvents.SaveEvents("Склад: инструмент ToolsID=" + ToolsID + " откреплен от операции TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            }
            UpdateTechCatalogTools();
            RefreshTechCatalogTools();
        }

        public void UpdateTechCatalogTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM TechCatalogTools", ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(TechCatalogToolsDT);
                }
            }
        }
        #endregion

        public TechStoreGroupInfo GetSubGroupInfo(int TechStoreID)
        {
            TechStoreGroupInfo techStoreGroupInfo = new TechStoreGroupInfo();
            string SelectCommand = @"SELECT TOP 1 TSG.*, TechStore.* FROM TechStore                
                INNER JOIN TechStoreSubGroups as TSG ON TechStore.TechStoreSubGroupID = TSG.TechStoreSubGroupID
                WHERE TechStoreID = " + TechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        techStoreGroupInfo.TechStoreName = DT.Rows[0]["TechStoreName"].ToString();
                        techStoreGroupInfo.TechStoreSubGroupName = DT.Rows[0]["TechStoreSubGroupName"].ToString();
                        techStoreGroupInfo.SubGroupNotes = DT.Rows[0]["Notes"].ToString();
                        techStoreGroupInfo.SubGroupNotes1 = DT.Rows[0]["Notes1"].ToString();
                        techStoreGroupInfo.SubGroupNotes2 = DT.Rows[0]["Notes2"].ToString();
                        if (DT.Rows[0]["Length"] != DBNull.Value)
                            techStoreGroupInfo.Length = Convert.ToInt32(DT.Rows[0]["Length"]);
                        else
                            techStoreGroupInfo.Length = -1;
                        if (DT.Rows[0]["Width"] != DBNull.Value)
                            techStoreGroupInfo.Width = Convert.ToInt32(DT.Rows[0]["Width"]);
                        else
                            techStoreGroupInfo.Width = -1;
                        if (DT.Rows[0]["Height"] != DBNull.Value)
                            techStoreGroupInfo.Height = Convert.ToInt32(DT.Rows[0]["Height"]);
                        else
                            techStoreGroupInfo.Height = -1;
                    }
                }
            }
            return techStoreGroupInfo;
        }

    }





    public class TestTechCatalog
    {
        public BindingSource ResultBS;
        public BindingSource SummaryBS;
        DataTable DecorConfigDT;
        DataTable DecorOrdersDT;
        DataTable DTTTTTT;
        DataTable FrontsConfigDT;
        DataTable FrontsOrdersDT;
        DataTable ResultDT;
        DataTable SummaryDT;
        DataTable TechCatalogOperationsDetail;
        DataTable TechCatalogOperationsTerms;
        DataTable TechCatalogStoreDetailDT;
        DataTable TechStore;
        public TestTechCatalog()
        {

        }

        public DataView ResultDV
        {
            get { return DTTTTTT.AsDataView(); }
        }

        public void CreateDecorExcel(int TechCatalogOperationsGroupID)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont Serif8F = hssfworkbook.CreateFont();
            Serif8F.FontHeightInPoints = 8;
            Serif8F.FontName = "MS Sans Serif";

            HSSFFont Serif10F = hssfworkbook.CreateFont();
            Serif10F.FontHeightInPoints = 10;
            Serif10F.FontName = "MS Sans Serif";

            HSSFFont SerifBold10F = hssfworkbook.CreateFont();
            SerifBold10F.FontHeightInPoints = 10;
            SerifBold10F.FontName = "MS Sans Serif";
            SerifBold10F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle TableHeaderCS = hssfworkbook.CreateCellStyle();
            //TableHeaderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            //TableHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            //TableHeaderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            //TableHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            //TableHeaderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            //TableHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            //TableHeaderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            //TableHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            TableHeaderCS.SetFont(Serif8F);

            HSSFCellStyle TableHeaderDecCS = hssfworkbook.CreateCellStyle();
            TableHeaderDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.RightBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.TopBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.SetFont(Serif8F);

            HSSFCellStyle WorkerColumnCS = hssfworkbook.CreateCellStyle();
            WorkerColumnCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            WorkerColumnCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.BottomBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.LeftBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.RightBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.TopBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.SetFont(Serif10F);

            HSSFCellStyle TableContentCS = hssfworkbook.CreateCellStyle();
            TableContentCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableContentCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableContentCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableContentCS.RightBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableContentCS.TopBorderColor = HSSFColor.BLACK.index;
            TableContentCS.SetFont(SerifBold10F);

            HSSFCellStyle TotamAmountCS = hssfworkbook.CreateCellStyle();
            TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.SetFont(SerifBold10F);

            HSSFCellStyle TotamAmountDecCS = hssfworkbook.CreateCellStyle();
            TotamAmountDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.SetFont(SerifBold10F);

            #endregion

            //DataTable DT = new DataTable();
            //GridsDecorAssemblyToExcelSingly(ref hssfworkbook, ref sheet1,
            //    Calibri11CS, CalibriBold11CS, CalibriBold11F, TableHeaderCS, TableHeaderDecCS, DT);

            GetDecor(TechCatalogOperationsGroupID);

            string FileName = "тест1";
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

        public void CreateFrontsExcel(string TechStoreName, int TechCatalogOperationsGroupID)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFFont Serif8F = hssfworkbook.CreateFont();
            Serif8F.FontHeightInPoints = 8;
            Serif8F.FontName = "MS Sans Serif";

            HSSFFont Serif10F = hssfworkbook.CreateFont();
            Serif10F.FontHeightInPoints = 10;
            Serif10F.FontName = "MS Sans Serif";

            HSSFFont SerifBold10F = hssfworkbook.CreateFont();
            SerifBold10F.FontHeightInPoints = 10;
            SerifBold10F.FontName = "MS Sans Serif";
            SerifBold10F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            SimpleCS.SetFont(Serif8F);

            HSSFCellStyle SimpleMergedCS = hssfworkbook.CreateCellStyle();
            SimpleMergedCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleMergedCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            SimpleMergedCS.SetFont(Serif8F);

            HSSFCellStyle TableHeaderDecCS = hssfworkbook.CreateCellStyle();
            TableHeaderDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.RightBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableHeaderDecCS.TopBorderColor = HSSFColor.BLACK.index;
            TableHeaderDecCS.SetFont(Serif8F);

            HSSFCellStyle WorkerColumnCS = hssfworkbook.CreateCellStyle();
            WorkerColumnCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            WorkerColumnCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.BottomBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.LeftBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.RightBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            WorkerColumnCS.TopBorderColor = HSSFColor.BLACK.index;
            WorkerColumnCS.SetFont(Serif10F);

            HSSFCellStyle TableContentCS = hssfworkbook.CreateCellStyle();
            TableContentCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            TableContentCS.BottomBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TableContentCS.LeftBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TableContentCS.RightBorderColor = HSSFColor.BLACK.index;
            TableContentCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TableContentCS.TopBorderColor = HSSFColor.BLACK.index;
            TableContentCS.SetFont(SerifBold10F);

            HSSFCellStyle TotamAmountCS = hssfworkbook.CreateCellStyle();
            TotamAmountCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotamAmountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.LeftBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.RightBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            TotamAmountCS.TopBorderColor = HSSFColor.BLACK.index;
            TotamAmountCS.SetFont(SerifBold10F);

            HSSFCellStyle SimpleDecCS = hssfworkbook.CreateCellStyle();
            SimpleDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.0000");
            SimpleDecCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            SimpleDecCS.SetFont(Serif8F);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet(TechStoreName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 20 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 20 * 256);
            sheet1.SetColumnWidth(3, 20 * 256);
            sheet1.SetColumnWidth(4, 20 * 256);

            //HSSFCell cell = null;

            int ColumnIndex = 1;
            int RowIndex = 0;
            for (int i = 0; i < SummaryDT.Rows.Count; i++)
            {
                ColumnIndex = Convert.ToInt32(SummaryDT.Rows[i]["SerialNumber"]);

                HSSFRow row = sheet1.CreateRow(RowIndex);
                HSSFCell cell = row.CreateCell(0);
                cell.SetCellValue(Convert.ToInt32(SummaryDT.Rows[i]["SerialNumber"]));
                cell.CellStyle = SimpleCS;

                cell = row.CreateCell(ColumnIndex + 1);
                cell.SetCellValue(SummaryDT.Rows[i]["GroupName"].ToString());
                cell.CellStyle = SimpleCS;

                cell = row.CreateCell(ColumnIndex + 2);
                if (SummaryDT.Rows[i]["MachinesOperationName"] != DBNull.Value)
                    cell.SetCellValue(SummaryDT.Rows[i]["MachinesOperationName"].ToString());
                cell.CellStyle = SimpleCS;

                cell = row.CreateCell(ColumnIndex + 3);
                if (SummaryDT.Rows[i]["TechStoreName"] != DBNull.Value)
                    cell.SetCellValue(SummaryDT.Rows[i]["TechStoreName"].ToString());
                cell.CellStyle = SimpleCS;

                cell = row.CreateCell(ColumnIndex + 4);
                if (SummaryDT.Rows[i]["Cost"] != DBNull.Value)
                    cell.SetCellValue(Convert.ToDouble(SummaryDT.Rows[i]["Cost"]));
                cell.CellStyle = SimpleDecCS;

                cell = sheet1.CreateRow(RowIndex++).CreateCell(ColumnIndex + 5);
                if (SummaryDT.Rows[i]["Measure"] != DBNull.Value)
                    cell.SetCellValue(SummaryDT.Rows[i]["Measure"].ToString());
                cell.CellStyle = SimpleCS;

                if (SummaryDT.Rows.Count - 1 > i && Convert.ToInt32(SummaryDT.Rows[i + 1]["SerialNumber"]) > Convert.ToInt32(SummaryDT.Rows[i]["SerialNumber"]))
                {
                    int SerialNumber1 = Convert.ToInt32(SummaryDT.Rows[i + 1]["SerialNumber"]);
                    int SerialNumber2 = Convert.ToInt32(SummaryDT.Rows[i]["SerialNumber"]);
                    ColumnIndex++;
                }
                if (SummaryDT.Rows.Count - 1 > i && Convert.ToInt32(SummaryDT.Rows[i + 1]["SerialNumber"]) < Convert.ToInt32(SummaryDT.Rows[i]["SerialNumber"]))
                {
                    int SerialNumber1 = Convert.ToInt32(SummaryDT.Rows[i + 1]["SerialNumber"]);
                    int SerialNumber2 = Convert.ToInt32(SummaryDT.Rows[i]["SerialNumber"]);
                    ColumnIndex--;
                }
            }
            int firstRowMergeIndex = -1;
            int secondRowMergeIndex = 0;

            for (int x = 0; x < SummaryDT.Rows.Count; x++)
            {
                int SerialNumber = Convert.ToInt32(SummaryDT.Rows[x]["SerialNumber"]);
                string CurrentGroupName = SummaryDT.Rows[x]["GroupName"].ToString();
                if (x == SummaryDT.Rows.Count - 1)
                {
                    if (firstRowMergeIndex != 0 && firstRowMergeIndex != -1)
                    {
                        secondRowMergeIndex = x;
                        sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(firstRowMergeIndex, SerialNumber + 1, secondRowMergeIndex, SerialNumber + 1));
                        firstRowMergeIndex = 0;
                        secondRowMergeIndex = 0;
                        continue;
                    }
                }
                else
                {
                    string GroupName = SummaryDT.Rows[x + 1]["GroupName"].ToString();
                    if (SummaryDT.Rows[x + 1]["GroupName"].ToString() == CurrentGroupName)
                    {
                        if (firstRowMergeIndex == -1)
                            firstRowMergeIndex = x;
                    }
                    else
                    {
                        if (firstRowMergeIndex == -1)
                        {

                        }
                        else
                            secondRowMergeIndex = x;
                    }

                    if (firstRowMergeIndex != -1 && secondRowMergeIndex != 0)
                    {
                        HSSFRow mergedRow = sheet1.CreateRow(firstRowMergeIndex);
                        sheet1.AddMergedRegion(new CellRangeAddress(firstRowMergeIndex, secondRowMergeIndex, SerialNumber + 1, SerialNumber + 1));
                        HSSFCell c = mergedRow.GetCell(0);
                        firstRowMergeIndex = -1;
                        secondRowMergeIndex = 0;
                    }
                }
            }

            firstRowMergeIndex = -1;
            secondRowMergeIndex = 0;

            for (int x = 0; x < SummaryDT.Rows.Count; x++)
            {
                int SerialNumber = Convert.ToInt32(SummaryDT.Rows[x]["SerialNumber"]);
                string CurrentMachinesOperationName = SummaryDT.Rows[x]["MachinesOperationName"].ToString();
                if (x == SummaryDT.Rows.Count - 1)
                {
                    if (firstRowMergeIndex != 0 && firstRowMergeIndex != -1)
                    {
                        secondRowMergeIndex = x;
                        sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(firstRowMergeIndex, SerialNumber + 1, secondRowMergeIndex, SerialNumber + 1));
                        firstRowMergeIndex = 0;
                        secondRowMergeIndex = 0;
                        continue;
                    }
                }
                else
                {
                    string MachinesOperationName = SummaryDT.Rows[x + 1]["MachinesOperationName"].ToString();
                    if (SummaryDT.Rows[x + 1]["MachinesOperationName"].ToString() == CurrentMachinesOperationName)
                    {
                        if (firstRowMergeIndex == -1)
                            firstRowMergeIndex = x;
                    }
                    else
                    {
                        if (firstRowMergeIndex == -1)
                        {

                        }
                        else
                            secondRowMergeIndex = x;
                    }

                    if (firstRowMergeIndex != -1 && secondRowMergeIndex != 0)
                    {
                        HSSFRow mergedRow = sheet1.CreateRow(firstRowMergeIndex);
                        sheet1.AddMergedRegion(new CellRangeAddress(firstRowMergeIndex, secondRowMergeIndex, SerialNumber + 2, SerialNumber + 2));
                        HSSFCell c = mergedRow.GetCell(0);
                        firstRowMergeIndex = -1;
                        secondRowMergeIndex = 0;
                    }
                }
            }

            string FileName = "тест1";
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

        public bool Func1(DataTable DT1, int TechCatalogOperationsDetailID)
        {
            string filter = "PrevTechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID;
            DataRow[] rows1 = DT1.Select(filter);
            if (rows1.Count() == 0)
                return false;

            DataTable DT = DT1.Clone();
            foreach (DataRow item in rows1)
                DT.Rows.Add(item.ItemArray);

            Func2(DT1, DT, TechCatalogOperationsDetailID);
            return true;
        }

        public void Func2(DataTable DT1, DataTable DT2, int TechCatalogOperationsDetailID)
        {
            for (int j = 0; j < DT2.Rows.Count; j++)
            {
                int SerialNumber = Convert.ToInt32(DT2.Rows[j]["SerialNumber"]);
                int TechCatalogOperationsGroupID = Convert.ToInt32(DT2.Rows[j]["TechCatalogOperationsGroupID"]);
                int PrevTechCatalogOperationsDetailID = Convert.ToInt32(DT2.Rows[j]["PrevTechCatalogOperationsDetailID"]);
                TechCatalogOperationsDetailID = Convert.ToInt32(DT2.Rows[j]["TechCatalogOperationsDetailID"]);
                string GroupName = DT2.Rows[j]["GroupName"].ToString();

                if (!DTTTTTT.Columns.Contains("GroupName" + SerialNumber))
                    DTTTTTT.Columns.Add(new DataColumn("GroupName" + SerialNumber, Type.GetType("System.String")));

                DataRow NewRow = DTTTTTT.NewRow();
                NewRow["GroupName" + SerialNumber] = GroupName;
                NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                NewRow["PrevTechCatalogOperationsDetailID"] = PrevTechCatalogOperationsDetailID;
                NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                DTTTTTT.Rows.Add(NewRow);

                //if (Func1(DT1, TechCatalogOperationsDetailID))
                //{

                //}
            }
        }

        public void GetDecorOrders(int DecorOrderID, int TechCatalogOperationsGroupID)
        {
            string SelectCommand = @"SELECT * FROM NewDecorOrders
                WHERE DecorOrderID=" + DecorOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DecorOrdersDT.Clear();
                DA.Fill(DecorOrdersDT);
            }
            SelectCommand = @"SELECT * FROM DecorConfig
                WHERE DecorConfigID IN (SELECT DecorConfigID FROM infiniu2_marketingorders.dbo.NewDecorOrders WHERE DecorOrderID=" + DecorOrderID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DecorConfigDT.Clear();
                DA.Fill(DecorConfigDT);
            }

            SummaryDT.Clear();
            GetDecor(TechCatalogOperationsGroupID);
        }

        public void GetFrontsOrders(int FrontsOrdersID, int TechCatalogOperationsGroupID)
        {
            string SelectCommand = @"SELECT * FROM NewFrontsOrders
                WHERE FrontsOrdersID=" + FrontsOrdersID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                FrontsOrdersDT.Clear();
                DA.Fill(FrontsOrdersDT);
            }
            SelectCommand = @"SELECT * FROM FrontsConfig
                WHERE FrontConfigID IN (SELECT FrontConfigID FROM infiniu2_marketingorders.dbo.NewFrontsOrders WHERE FrontsOrdersID=" + FrontsOrdersID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                FrontsConfigDT.Clear();
                DA.Fill(FrontsConfigDT);
            }

            SummaryDT.Clear();
            GetFronts(TechCatalogOperationsGroupID);
        }

        public DataTable GetOperationsGroups(int TechStoreID)
        {
            DataTable DT = new DataTable();
            string SelectCommand = @"SELECT * FROM TechCatalogOperationsGroups WHERE TechStoreID=" + TechStoreID + " ORDER BY GroupNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DT);
            }
            return DT;
        }

        public void Initialize()
        {
            Create();

            string SelectCommand = @"SELECT * FROM TechStore
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechStore.Clear();
                DA.Fill(TechStore);
            }
            SelectCommand = @"SELECT TechCatalogOperationsGroups.GroupName, TechCatalogOperationsGroups.GroupNumber, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsDetail.TechCatalogOperationsDetailID, TechCatalogOperationsDetail.TechCatalogOperationsGroupID, TechCatalogOperationsDetail.SerialNumber, MachinesOperationName FROM TechCatalogOperationsDetail 
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                ORDER BY TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupNumber, TechCatalogOperationsDetail.SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogOperationsDetail.Clear();
                DA.Fill(TechCatalogOperationsDetail);
            }
            SelectCommand = @"SELECT * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogOperationsTerms.Clear();
                DA.Fill(TechCatalogOperationsTerms);
            }
            SelectCommand = @"SELECT TechCatalogStoreDetail.*, TechStore.TechStoreName, Measures.Measure FROM TechCatalogStoreDetail
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID 
                ORDER BY GroupA, GroupB, GroupC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                TechCatalogStoreDetailDT.Clear();
                DA.Fill(TechCatalogStoreDetailDT);
            }
        }

        public void ReturnResultTable()
        {
            ResultDT.Clear();
            DTTTTTT.Clear();
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(SummaryDT, string.Empty, "SerialNumber", DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "SerialNumber" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int SerialNumber = Convert.ToInt32(DT1.Rows[i]["SerialNumber"]);
                string filter = "SerialNumber=" + SerialNumber;
                DataRow[] rows1 = SummaryDT.Select(filter);
                if (rows1.Count() == 0)
                    continue;

                DataTable DT2 = SummaryDT.Clone();
                foreach (DataRow item in rows1)
                    DT2.Rows.Add(item.ItemArray);

                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int TechCatalogOperationsGroupID = Convert.ToInt32(DT2.Rows[j]["TechCatalogOperationsGroupID"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(DT2.Rows[j]["TechCatalogOperationsDetailID"]);
                    int PrevTechCatalogOperationsDetailID = Convert.ToInt32(DT2.Rows[j]["PrevTechCatalogOperationsDetailID"]);
                    string GroupName = DT2.Rows[j]["GroupName"].ToString();

                    filter = "SerialNumber=" + SerialNumber + " AND TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID + " AND PrevTechCatalogOperationsDetailID=" + PrevTechCatalogOperationsDetailID;
                    rows1 = ResultDT.Select(filter);
                    if (rows1.Count() == 0)
                    {
                        DataRow NewRow = ResultDT.NewRow();
                        NewRow["SerialNumber"] = SerialNumber;
                        NewRow["GroupName"] = DT2.Rows[j]["GroupName"].ToString();
                        NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                        NewRow["PrevTechCatalogOperationsDetailID"] = PrevTechCatalogOperationsDetailID;
                        NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                        ResultDT.Rows.Add(NewRow);
                    }
                }
            }
            for (int i = ResultDT.Rows.Count - 1; i >= 0; i--)
            {
                int PrevTechCatalogOperationsDetailID = Convert.ToInt32(ResultDT.Rows[i]["PrevTechCatalogOperationsDetailID"]);
                int TechCatalogOperationsDetailID = Convert.ToInt32(ResultDT.Rows[i]["TechCatalogOperationsDetailID"]);
                string filter = "PrevTechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID;
                DataRow[] rows1 = ResultDT.Select(filter);
                if (rows1.Count() == 0 && PrevTechCatalogOperationsDetailID == 0)
                    ResultDT.Rows[i].Delete();
            }

            for (int i = 0; i < ResultDT.Rows.Count; i++)
            {
                int SerialNumber = Convert.ToInt32(ResultDT.Rows[i]["SerialNumber"]);
                if (i != 0)
                    continue;
                int TechCatalogOperationsDetailID = Convert.ToInt32(ResultDT.Rows[i]["TechCatalogOperationsDetailID"]);

                string filter = "PrevTechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID;
                DataRow[] rows1 = ResultDT.Select(filter);
                if (rows1.Count() == 0)
                    continue;

                DataTable DT2 = ResultDT.Clone();
                foreach (DataRow item in rows1)
                    DT2.Rows.Add(item.ItemArray);

                Func2(ResultDT, DT2, TechCatalogOperationsDetailID);
            }
            for (int i = 0; i < DTTTTTT.Rows.Count; i++)
            {
                DTTTTTT.Rows[i]["check"] = false;
                int TechCatalogOperationsGroupID = Convert.ToInt32(DTTTTTT.Rows[i]["TechCatalogOperationsGroupID"]);
                int PrevTechCatalogOperationsDetailID = Convert.ToInt32(DTTTTTT.Rows[i]["PrevTechCatalogOperationsDetailID"]);
                int TechCatalogOperationsDetailID = Convert.ToInt32(DTTTTTT.Rows[i]["TechCatalogOperationsDetailID"]);
            }
        }

        private void Create()
        {
            DTTTTTT = new DataTable();
            DTTTTTT.Columns.Add(new DataColumn("TechCatalogOperationsGroupID", Type.GetType("System.Int32")));
            DTTTTTT.Columns.Add(new DataColumn("PrevTechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            DTTTTTT.Columns.Add(new DataColumn("TechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            DTTTTTT.Columns.Add(new DataColumn("check", Type.GetType("System.Boolean")));
            DTTTTTT.Columns.Add(new DataColumn("hide", Type.GetType("System.Boolean")));

            ResultDT = new DataTable();
            ResultDT.Columns.Add(new DataColumn("SerialNumber", Type.GetType("System.Int32")));
            ResultDT.Columns.Add(new DataColumn("GroupName", Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn("TechCatalogOperationsGroupID", Type.GetType("System.Int32")));
            ResultDT.Columns.Add(new DataColumn("PrevTechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            ResultDT.Columns.Add(new DataColumn("TechCatalogOperationsDetailID", Type.GetType("System.Int32")));

            SummaryDT = new DataTable();
            SummaryDT.Columns.Add(new DataColumn("TechCatalogOperationsGroupID", Type.GetType("System.Int32")));
            SummaryDT.Columns.Add(new DataColumn("PrevTechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            SummaryDT.Columns.Add(new DataColumn("TechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            SummaryDT.Columns.Add(new DataColumn("TechStoreID", Type.GetType("System.Int32")));
            SummaryDT.Columns.Add(new DataColumn("SerialNumber", Type.GetType("System.Int32")));
            SummaryDT.Columns.Add(new DataColumn("GroupName", Type.GetType("System.String")));
            SummaryDT.Columns.Add(new DataColumn("MachinesOperationName", Type.GetType("System.String")));
            SummaryDT.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
            SummaryDT.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            SummaryDT.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            SummaryDT.Columns.Add(new DataColumn("check", Type.GetType("System.Boolean")));

            FrontsOrdersDT = new DataTable();
            DecorOrdersDT = new DataTable();
            DecorConfigDT = new DataTable();
            FrontsConfigDT = new DataTable();
            TechStore = new DataTable();
            TechCatalogStoreDetailDT = new DataTable();
            TechCatalogOperationsDetail = new DataTable();
            TechCatalogOperationsTerms = new DataTable();

            ResultBS = new BindingSource()
            {
                DataSource = ResultDT
            };
            SummaryBS = new BindingSource()
            {
                DataSource = SummaryDT
            };
        }
        private void GetDecor(int TechCatalogOperationsGroupID)
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(DecorOrdersDT))
            {
                DT1 = DV.ToTable(true, new string[] { "DecorConfigID", "Height", "Length", "Width" });
            }

            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int DecorID = 0;
                int ColorID = 0;
                int PatinaID = 0;
                int Height = 0;
                int Length = 0;
                int Width = 0;
                int Count = 0;

                DataRow[] rows1 = DecorConfigDT.Select("DecorConfigID=" + DT1.Rows[i]["DecorConfigID"]);
                if (rows1.Count() == 0)
                    continue;

                DecorID = Convert.ToInt32(rows1[0]["DecorID"]);
                ColorID = Convert.ToInt32(rows1[0]["ColorID"]);
                PatinaID = Convert.ToInt32(rows1[0]["PatinaID"]);
                Height = Convert.ToInt32(DT1.Rows[i]["Height"]);
                Length = Convert.ToInt32(DT1.Rows[i]["Length"]);
                Width = Convert.ToInt32(DT1.Rows[i]["Width"]);

                DataRow[] rows2 = DecorOrdersDT.Select("Height=" + Height + " AND Length=" + Length + " AND Width=" + Width + " AND DecorConfigID=" + DT1.Rows[i]["DecorConfigID"]);
                foreach (DataRow item in rows2)
                {
                    Count += Convert.ToInt32(item["Count"]);
                }

                DataRow[] rows3 = TechStore.Select("TechStoreID=" + DecorID);
                if (rows3.Count() == 0)
                    continue;

                string TechStoreName = rows3[0]["TechStoreName"].ToString().Replace("/", " ");

                string filter = "TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID;
                DataRow[] rows4 = TechCatalogOperationsDetail.Select("TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID);
                if (rows4.Count() == 0)
                    continue;

                foreach (DataRow row1 in rows4)
                {
                    int GroupNumber = Convert.ToInt32(row1["GroupNumber"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(row1["TechCatalogOperationsDetailID"]);
                    string GroupName = row1["GroupName"].ToString();
                    string MachinesOperationName = row1["MachinesOperationName"].ToString();

                    if (!GetTerms(TechCatalogOperationsDetailID, 0, PatinaID, Height, Width))
                    {
                        continue;
                    }
                    GetTechStoreDetail(TechCatalogOperationsGroupID, TechCatalogOperationsDetailID, TechCatalogOperationsDetailID, 0, GroupName, MachinesOperationName,
                        0, ColorID, PatinaID, Height, Width, Count, Count, GroupNumber);
                }
            }
        }

        private void GetFronts(int TechCatalogOperationsGroupID)
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(FrontsOrdersDT))
            {
                DT1 = DV.ToTable(true, new string[] { "FrontConfigID", "Height", "Width" });
            }

            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                decimal Square = 0;
                int FrontID = 0;
                int ColorID = 0;
                int InsetTypeID = 0;
                int PatinaID = 0;
                int Height = 0;
                int Width = 0;
                int Count = 0;

                DataRow[] rows1 = FrontsConfigDT.Select("FrontConfigID=" + DT1.Rows[i]["FrontConfigID"]);
                if (rows1.Count() == 0)
                    continue;

                FrontID = Convert.ToInt32(rows1[0]["FrontID"]);
                ColorID = Convert.ToInt32(rows1[0]["ColorID"]);
                InsetTypeID = Convert.ToInt32(rows1[0]["InsetTypeID"]);
                PatinaID = Convert.ToInt32(rows1[0]["PatinaID"]);
                Height = Convert.ToInt32(DT1.Rows[i]["Height"]);
                Width = Convert.ToInt32(DT1.Rows[i]["Width"]);

                DataRow[] rows2 = FrontsOrdersDT.Select("Height=" + Height + " AND Width=" + Width + " AND FrontConfigID=" + DT1.Rows[i]["FrontConfigID"]);
                foreach (DataRow item in rows2)
                {
                    Count += Convert.ToInt32(item["Count"]);
                    Square += Convert.ToDecimal(Height) * Convert.ToDecimal(Width) * Convert.ToDecimal(item["Count"]) / 1000000;
                }

                DataRow[] rows3 = TechStore.Select("TechStoreID=" + FrontID);
                if (rows3.Count() == 0)
                    continue;

                string TechStoreName = rows3[0]["TechStoreName"].ToString().Replace("/", " ");

                string filter = "TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID;
                DataRow[] rows4 = TechCatalogOperationsDetail.Select("TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID);
                if (rows4.Count() == 0)
                    continue;

                foreach (DataRow row1 in rows4)
                {
                    int GroupNumber = Convert.ToInt32(row1["GroupNumber"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(row1["TechCatalogOperationsDetailID"]);
                    string GroupName = row1["GroupName"].ToString();
                    string MachinesOperationName = row1["MachinesOperationName"].ToString();
                    if (GroupName.Length == 0)
                        GroupName = "НЕТ НАЗВАНИЯ_" + TechCatalogOperationsGroupID;
                    if (!GetTerms(TechCatalogOperationsDetailID, InsetTypeID, PatinaID, Height, Width))
                    {
                        continue;
                    }
                    GetTechStoreDetail(TechCatalogOperationsGroupID, 0, TechCatalogOperationsDetailID, 0, GroupName, MachinesOperationName,
                        InsetTypeID, ColorID, PatinaID, Height, Width, Count, Square, GroupNumber);
                }
            }
            foreach (DataRow row in SummaryDT.Rows)
                row["check"] = false;
        }
        private void GetOperations(int PrevTechCatalogOperationsDetailID, int TechStoreID, int ColumnOffset,
            int InsetTypeID, int ColorID, int PatinaID, int Height, int Width, int Count, decimal Square, int iGroupNumber = 0)
        {
            DataTable DT1 = new DataTable();
            using (DataView DV = new DataView(TechCatalogOperationsDetail, "TechStoreID=" + TechStoreID, string.Empty, DataViewRowState.CurrentRows))
            {
                DT1 = DV.ToTable(true, new string[] { "TechCatalogOperationsGroupID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int TechCatalogOperationsGroupID = Convert.ToInt32(DT1.Rows[i]["TechCatalogOperationsGroupID"]);

                string filter = "TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID + " AND (GroupNumber=" + iGroupNumber + " OR GroupNumber=0)";
                DataRow[] rows1 = TechCatalogOperationsDetail.Select(filter);
                if (rows1.Count() == 0)
                {
                    filter = "TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID;
                    rows1 = TechCatalogOperationsDetail.Select(filter);
                    if (rows1.Count() == 0)
                        continue;
                }
                DataTable TempDT = TechCatalogOperationsDetail.Clone();
                foreach (DataRow item in rows1)
                    TempDT.Rows.Add(item.ItemArray);
                foreach (DataRow row1 in rows1)
                {
                    int GroupNumber = Convert.ToInt32(row1["GroupNumber"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(row1["TechCatalogOperationsDetailID"]);
                    string GroupName = row1["GroupName"].ToString();
                    string MachinesOperationName = row1["MachinesOperationName"].ToString();
                    if (GroupName.Length == 0)
                        GroupName = "НЕТ НАЗВАНИЯ_" + TechCatalogOperationsGroupID;
                    if (!GetTerms(TechCatalogOperationsDetailID, InsetTypeID, PatinaID, Height, Width))
                    {
                        continue;
                    }
                    ColumnOffset++;
                    GetTechStoreDetail(TechCatalogOperationsGroupID, PrevTechCatalogOperationsDetailID, TechCatalogOperationsDetailID, ColumnOffset, GroupName, MachinesOperationName,
                        InsetTypeID, ColorID, PatinaID, Height, Width, Count, Square, GroupNumber);
                    ColumnOffset--;
                }
            }
            //GroupNameCounter++;
        }

        private void GetTechStoreDetail(
            int TechCatalogOperationsGroupID, int PrevTechCatalogOperationsDetailID, int TechCatalogOperationsDetailID, int ColumnOffset, string GroupName, string MachinesOperationName,
            int InsetTypeID, int ColorID, int PatinaID, int Height, int Width, int Count, decimal Square, int GroupNumber)
        {
            DataRow[] rows = TechCatalogStoreDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            if (rows.Count() == 0)
            {
                DataRow NewRow = SummaryDT.NewRow();
                NewRow["SerialNumber"] = ColumnOffset;
                NewRow["GroupName"] = GroupName;
                NewRow["MachinesOperationName"] = MachinesOperationName;
                NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                NewRow["PrevTechCatalogOperationsDetailID"] = PrevTechCatalogOperationsDetailID;
                NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                SummaryDT.Rows.Add(NewRow);

                return;
            }
            DataTable TempDT = TechCatalogStoreDetailDT.Clone();
            foreach (DataRow item in rows)
                TempDT.Rows.Add(item.ItemArray);
            foreach (DataRow row in rows)
            {
                bool BreakChain = false;
                decimal Expense = 0;
                int TechStoreID = 0;
                string Measure = string.Empty;
                string TechStoreName = string.Empty;
                if (Convert.ToBoolean(row["IsSubGroup"]))
                {
                    DataRow[] rows0 = TechStore.Select("TechStoreID=" + ColorID);
                    if (rows0.Count() > 0)
                    {
                        TechStoreID = ColorID;
                        Measure = rows0[0]["Measure"].ToString();
                        TechStoreName = rows0[0]["TechStoreName"].ToString();
                    }
                }
                else
                {
                    TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                    Measure = row["Measure"].ToString();
                    TechStoreName = row["TechStoreName"].ToString();
                }
                BreakChain = Convert.ToBoolean(row["BreakChain"]);
                if (BreakChain)
                    continue;
                if (row["Count"] != DBNull.Value)
                    Expense = Convert.ToDecimal(row["Count"]);
                decimal Cost = Expense * Square;

                DataRow NewRow = SummaryDT.NewRow();
                NewRow["TechCatalogOperationsGroupID"] = TechCatalogOperationsGroupID;
                NewRow["PrevTechCatalogOperationsDetailID"] = PrevTechCatalogOperationsDetailID;
                NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                NewRow["TechStoreID"] = TechStoreID;
                NewRow["SerialNumber"] = ColumnOffset;
                NewRow["GroupName"] = GroupName;
                NewRow["MachinesOperationName"] = MachinesOperationName;
                NewRow["TechStoreName"] = TechStoreName;
                NewRow["Cost"] = Cost;
                NewRow["Measure"] = Measure;
                NewRow["check"] = false;
                SummaryDT.Rows.Add(NewRow);

                GetOperations(TechCatalogOperationsDetailID, TechStoreID, ColumnOffset,
                    InsetTypeID, ColorID, PatinaID, Height, Width, Count, Square, GroupNumber);
            }
        }

        private bool GetTerms(int TechCatalogOperationsDetailID, int InsetTypeID, int PatinaID, int Height, int Width)
        {
            DataRow[] rows = TechCatalogOperationsTerms.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
            if (rows.Count() == 0)
            {
                return true;
            }
            foreach (DataRow row in rows)
            {
                int Term = Convert.ToInt32(row["Term"]);
                string Parameter = row["Parameter"].ToString();
                switch (Parameter)
                {
                    case "CoverID":
                        break;
                    case "InsetTypeID":
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (InsetTypeID == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (InsetTypeID != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (InsetTypeID >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (InsetTypeID <= Term)
                            { }
                            else
                                return false;
                        }
                        break;
                    case "InsetColorID":
                        break;
                    case "ColorID":
                        break;
                    case "PatinaID":
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (PatinaID == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (PatinaID != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (PatinaID >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (PatinaID <= Term)
                            { }
                            else
                                return false;
                        }
                        break;
                    case "Diameter":
                        break;
                    case "Thickness":
                        break;
                    case "Length":
                        break;
                    case "Height":
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (Height == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (Height != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (Height >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (Height <= Term)
                            { }
                            else
                                return false;
                        }
                        break;
                    case "Width":
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (Width == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (Width != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (Width >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (Width <= Term)
                            { }
                            else
                                return false;
                        }
                        break;
                    case "Admission":
                        break;
                    case "InsetHeightAdmission":
                        break;
                    case "InsetWidthAdmission":
                        break;
                    case "Capacity":
                        break;
                    case "Weight":
                        break;
                }
            }
            return true;
        }

        private bool SearchRow(int RowIndex, int TechCatalogOperationsGroupID, int PrevTechCatalogOperationsDetailID, int TechCatalogOperationsDetailID)
        {
            for (int i = RowIndex + 1; i < DTTTTTT.Rows.Count; i++)
            {
                if (Convert.ToInt32(DTTTTTT.Rows[i]["TechCatalogOperationsGroupID"]) == TechCatalogOperationsGroupID
                    || Convert.ToInt32(DTTTTTT.Rows[i]["PrevTechCatalogOperationsDetailID"]) == PrevTechCatalogOperationsDetailID
                    || Convert.ToInt32(DTTTTTT.Rows[i]["TechCatalogOperationsDetailID"]) == TechCatalogOperationsDetailID)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public struct TechStoreGroupInfo
    {
        public int Length;
        public int Width;
        public int Height;
        public string TechStoreName;
        public string TechStoreSubGroupName;
        public string SubGroupNotes;
        public string SubGroupNotes1;
        public string SubGroupNotes2;
    }

    public struct TechLabelInfo
    {
        public int Length;
        public int Width;
        public int Height;
        public int PositionsCount;
        public int LabelsCount;
        public int Factory;
        public string Color;
        public string DocDateTime;
    }
}
