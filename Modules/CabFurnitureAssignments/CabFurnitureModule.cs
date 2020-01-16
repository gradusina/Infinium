using System;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using System.Text;
using System.Drawing.Printing;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;

namespace Infinium.Modules.CabFurnitureModule
{
    public class CoversManager
    {
        DataTable CoversDT;
        public BindingSource CoversBS;
        SqlDataAdapter CoversDA;
        SqlCommandBuilder CoversCB;

        public CoversManager()
        {
            string SelectCommand = @"SELECT * FROM CabFurnitureCovers";
            CoversDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            CoversCB = new SqlCommandBuilder(CoversDA);
            CoversDT = new DataTable();
            CoversDA.Fill(CoversDT);

            CoversBS = new BindingSource()
            {
                DataSource = CoversDT
            };
        }

        public void AddCover(int CoverID1, int PatinaID1, int InsetColorID, int TechStoreID, int CoverID2, int PatinaID2)
        {
            DataRow NewRow = CoversDT.NewRow();
            NewRow["CoverID1"] = CoverID1;
            NewRow["PatinaID1"] = PatinaID1;
            NewRow["InsetColorID"] = InsetColorID;
            NewRow["TechStoreID"] = TechStoreID;
            NewRow["CoverID2"] = CoverID2;
            NewRow["PatinaID2"] = PatinaID2;
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            CoversDT.Rows.Add(NewRow);
        }

        public void AddCovers(int CoverID1, int PatinaID1, int InsetColorID, int TechStoreSubGroupID, int CoverID2, int PatinaID2)
        {
            DataTable techStoredt = new DataTable();
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore WHERE TechStoreSubGroupID=" + TechStoreSubGroupID + " ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(techStoredt);
            }

            SelectCommand = @"SELECT TOP 0 * FROM CabFurnitureCovers";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DateTime CreationDateTime = Security.GetCurrentDate();
                        for (int i = 0; i < techStoredt.Rows.Count; i++)
                        {
                            int TechStoreID = Convert.ToInt32(techStoredt.Rows[i]["TechStoreID"]);

                            if (IsCoverExist(CoverID1, PatinaID1, InsetColorID, TechStoreID, CoverID2, PatinaID2))
                                continue;

                            DataRow NewRow = DT.NewRow();
                            NewRow["CoverID1"] = CoverID1;
                            NewRow["PatinaID1"] = PatinaID1;
                            NewRow["InsetColorID"] = InsetColorID;
                            NewRow["TechStoreID"] = TechStoreID;
                            NewRow["CoverID2"] = CoverID2;
                            NewRow["PatinaID2"] = PatinaID2;
                            NewRow["CreationUserID"] = Security.CurrentUserID;
                            NewRow["CreationDateTime"] = CreationDateTime;
                            DT.Rows.Add(NewRow);
                        }
                        DA.Update(DT);
                    }
                }
            }
            techStoredt.Dispose();
        }

        public void DeleteCover()
        {
            if (CoversBS.Count == 0)
                return;
            int Pos = CoversBS.Position;
            CoversBS.RemoveCurrent();
            if (CoversBS.Count > 0)
                if (Pos >= CoversBS.Count)
                    CoversBS.MoveLast();
                else
                    CoversBS.Position = Pos;
        }


        public void DeleteCovers(int[] CabFurnitureCoverID)
        {
            DataRow[] rows = CoversDT.Select("CabFurnitureCoverID IN (" + string.Join(",", CabFurnitureCoverID) + ")");
            for (int i = rows.Count() - 1; i >= 0; i--)
            {
                rows[i].Delete();
            }
        }

        public void RefreshCovers()
        {
            CoversDT.Clear();
            CoversDA.Fill(CoversDT);
        }

        public void UpdateCovers()
        {
            CoversDA.Update(CoversDT);
        }

        public bool IsCoverExist(int CoverID1, int PatinaID1, int InsetColorID, int TechStoreID, int CoverID2, int PatinaID2)
        {
            DataRow[] rows = CoversDT.Select("CoverID1=" + CoverID1 + " AND PatinaID1=" + PatinaID1 + " AND InsetColorID=" + InsetColorID + " AND TechStoreID=" + TechStoreID
                 + " AND CoverID2=" + CoverID2 + " AND PatinaID2=" + PatinaID2);
            if (rows.Any())
                return true;
            return false;
        }

        public void MoveToLast()
        {
            CoversBS.MoveLast();
        }

    }


    public class AssignmentsManager
    {
        public FileManager FM = null;

        bool bNewAssignment = false;

        int iCabFurAssignmentID = 0;

        DataTable CabFurnitureDocumentTypesDT;
        DataTable ClientsDT;

        DataTable TechStoreGroupsDT;
        DataTable TechStoreSubGroupsDT;
        DataTable TechStoreDT;
        DataTable UsersDT;
        DataTable MeasuresDT;
        DataTable ColorsDT;
        DataTable CoversDT;
        DataTable PatinaDT;
        DataTable InsetTypesDT;
        DataTable InsetColorsDT;
        DataTable StoreItemsDT;
        DataTable StoreDetailTermsDT;

        DataTable TempCoversDT;
        DataTable TempPatinaDT;

        DataTable DocumentsDT = null;
        DataTable AllAssignmentsDT = null;
        DataTable NewAssignmentDetailsDT = null;

        DataTable NonAgreementDetailDT;
        DataTable AgreedDetailDT;
        DataTable OnProductionDetailDT;
        DataTable InProductionDetailDT;
        DataTable OnStorageDetailDT;
        DataTable OnExpeditionDetailDT;

        public BindingSource BasicInsetColorsBS;
        public BindingSource BasicCoversBS1;
        public BindingSource BasicCoversBS2;
        public BindingSource BasicPatinaBS1;
        public BindingSource BasicPatinaBS2;
        public BindingSource BasicTechStoreBS;
        public BindingSource BasicTechStoreGroupsBS;
        public BindingSource BasicTechStoreSubGroupsBS;

        public BindingSource NonAgreementDetailBS;
        public BindingSource AgreedDetailBS;
        public BindingSource OnProductionDetailBS;
        public BindingSource InProductionDetailBS;
        public BindingSource OnStorageDetailBS;
        public BindingSource OnExpeditionDetailBS;

        public BindingSource TechStoreGroupsBS;
        public BindingSource TechStoreSubGroupsBS;
        public BindingSource TechStoreBS;
        public BindingSource CoversBS;
        public BindingSource PatinaBS;

        public BindingSource AllAssignmentsBS = null;
        public BindingSource DocumentsBS = null;
        public BindingSource NewAssignmentDetailsBS = null;

        SqlCommandBuilder NewAssignmentCB;
        SqlCommandBuilder AllAssignmentsCB;

        SqlDataAdapter NewAssignmentDA;
        SqlDataAdapter AllAssignmentsDA;

        DataTable ComplementLabelDataDT = null;        
        DataTable PackageLabelDataDT = null;

        public bool NewAssignment
        {
            get { return bNewAssignment; }
            set { bNewAssignment = value; }
        }

        public int CabFurAssignmentID
        {
            get { return iCabFurAssignmentID; }
            set { iCabFurAssignmentID = value; }
        }

        public DataTable OrdersDT
        {
            get { return NewAssignmentDetailsDT; }
        }

        public AssignmentsManager()
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
            FM = new FileManager();

            ComplementLabelDataDT = new DataTable();
            ComplementLabelDataDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            ComplementLabelDataDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ComplementLabelDataDT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            ComplementLabelDataDT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            ComplementLabelDataDT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            ComplementLabelDataDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));

            PackageLabelDataDT = new DataTable();
            PackageLabelDataDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            PackageLabelDataDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            PackageLabelDataDT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            PackageLabelDataDT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            PackageLabelDataDT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            PackageLabelDataDT.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));

            CabFurnitureDocumentTypesDT = new DataTable();
            ClientsDT = new DataTable();

            NonAgreementDetailDT = new DataTable();
            AgreedDetailDT = new DataTable();
            OnProductionDetailDT = new DataTable();
            InProductionDetailDT = new DataTable();
            OnStorageDetailDT = new DataTable();
            OnExpeditionDetailDT = new DataTable();
            
            TechStoreGroupsDT = new DataTable();
            TechStoreSubGroupsDT = new DataTable();
            TechStoreDT = new DataTable();

            StoreDetailTermsDT = new DataTable();
            UsersDT = new DataTable();
            MeasuresDT = new DataTable();
            StoreItemsDT = new DataTable();
            DocumentsDT = new DataTable();
            AllAssignmentsDT = new DataTable();
            NewAssignmentDetailsDT = new DataTable();

            NonAgreementDetailBS = new BindingSource();
            AgreedDetailBS = new BindingSource();
            OnProductionDetailBS = new BindingSource();
            InProductionDetailBS = new BindingSource();
            OnStorageDetailBS = new BindingSource();
            OnExpeditionDetailBS = new BindingSource();
            
            AllAssignmentsBS = new BindingSource();
            DocumentsBS = new BindingSource();
            NewAssignmentDetailsBS = new BindingSource();
        }

        private void Fill()
        {
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
            SelectCommand = @"SELECT TS.TechStoreID, TS.CoverID, TS.PatinaID, TSG.TechStoreGroupID, TS.TechStoreSubGroupID, TSG.TechStoreSubGroupName, TS.TechStoreName, TS.MeasureID, TS.Notes FROM TechStore TS
                INNER JOIN TechStoreSubGroups TSG ON TS.TechStoreSubGroupID=TSG.TechStoreSubGroupID ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(TechStoreDT);
            }
            SelectCommand = @"SELECT UserID, Name, ShortName FROM Users";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            SelectCommand = @"SELECT * FROM Measures";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDT);
            }
            SelectCommand = @"SELECT * FROM CabFurnitureDocumentTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(CabFurnitureDocumentTypesDT);
            }
            SelectCommand = @"SELECT ClientID, ClientName FROM Clients";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
            }

            PatinaDT = new DataTable();
            InsetTypesDT = new DataTable();

            GetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PatinaDT);
            }
            TempPatinaDT = new DataTable();
            TempPatinaDT.Columns.Add(new DataColumn("PatinaID", Type.GetType("System.Int64")));
            GetInsetColorsDT();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetTypes ORDER BY InsetTypeName", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(InsetTypesDT);
                {
                    DataRow NewRow = InsetTypesDT.NewRow();
                    NewRow["InsetTypeID"] = 0;
                    NewRow["GroupID"] = -1;
                    NewRow["InsetTypeName"] = "на выбор";
                    NewRow["MeasureID"] = 1;
                    InsetTypesDT.Rows.Add(NewRow);
                }
            }
            CreateCoversDT();

            SelectCommand = @"SELECT * FROM TechCatalogStoreDetailTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(StoreDetailTermsDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM CabFurnitureDocuments", ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(DocumentsDT);
            }

            SelectCommand = @"SELECT * FROM CabFurnitureAssignments ORDER BY CabFurAssignmentID DESC";
            AllAssignmentsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            AllAssignmentsCB = new SqlCommandBuilder(AllAssignmentsDA);
            AllAssignmentsDA.Fill(AllAssignmentsDT);
            AllAssignmentsDT.Columns.Add(new DataColumn("DocsCount", Type.GetType("System.Int32")));
            AllAssignmentsDT.Columns.Add(new DataColumn("DocsPrintedCount", Type.GetType("System.Int32")));
            AllAssignmentsDT.Columns.Add(new DataColumn("PrintStatus", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 * FROM CabFurnitureAssignmentDetails";
            NewAssignmentDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            NewAssignmentCB = new SqlCommandBuilder(NewAssignmentDA);
            NewAssignmentDA.Fill(NewAssignmentDetailsDT);
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
                        if (Rows.Count() > 0)
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
                        NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
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
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "на выбор";
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

        private void CreateCoversDT()
        {
            TempCoversDT = new DataTable();
            TempCoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));

            CoversDT = new DataTable();
            CoversDT.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            CoversDT.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));

            DataRow EmptyRow = CoversDT.NewRow();
            EmptyRow["CoverID"] = -1;
            EmptyRow["CoverName"] = "-";
            CoversDT.Rows.Add(EmptyRow);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreID, TechStoreName FROM TechStore" +
                " WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)" +
                " ORDER BY TechStoreName",
                ConnectionStrings.CatalogConnectionString))
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

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = CoversDT.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["CoverName"] = DT.Rows[i]["InsetColorName"].ToString();
                        CoversDT.Rows.Add(NewRow);
                    }
                }
            }

        }

        private void Binding()
        {
            TechStoreGroupsBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreGroupsDT, "TechStoreGroupID IN (45,52,53,56,57,58)", string.Empty, DataViewRowState.CurrentRows),
                Sort = "TechStoreGroupName"
            };
            TechStoreSubGroupsBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreSubGroupsDT, "TechStoreGroupID IN (45,52,53,56,57,58)", string.Empty, DataViewRowState.CurrentRows),
                Sort = "TechStoreSubGroupName"
            };
            TechStoreBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreDT, "TechStoreGroupID IN (45,52,53,56,57,58)", string.Empty, DataViewRowState.CurrentRows),
                Sort = "TechStoreName"
            };
            CoversBS = new BindingSource()
            {
                DataSource = new DataView(CoversDT)
            };
            //CoversBS.Sort = "TechStoreName";

            PatinaBS = new BindingSource()
            {
                DataSource = new DataView(PatinaDT)
            };
            BasicTechStoreGroupsBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreGroupsDT),
                Sort = "TechStoreGroupName"
            };
            BasicTechStoreSubGroupsBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreSubGroupsDT),
                Sort = "TechStoreSubGroupName"
            };
            BasicTechStoreBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreDT),
                Sort = "TechStoreName"
            };
            BasicInsetColorsBS = new BindingSource()
            {
                DataSource = new DataView(InsetColorsDT),
                Sort = "InsetColorName"
            };
            BasicCoversBS1 = new BindingSource()
            {
                DataSource = new DataView(CoversDT)
            };
            BasicCoversBS2 = new BindingSource()
            {
                DataSource = new DataView(CoversDT)
            };
            BasicPatinaBS1 = new BindingSource()
            {
                DataSource = new DataView(PatinaDT)
            };
            BasicPatinaBS2 = new BindingSource()
            {
                DataSource = new DataView(PatinaDT)
            };
            //PatinaBS.Sort = "TechStoreName";

            NonAgreementDetailBS.DataSource = NonAgreementDetailDT;
            AgreedDetailBS.DataSource = AgreedDetailDT;
            OnProductionDetailBS.DataSource = OnProductionDetailDT;
            InProductionDetailBS.DataSource = InProductionDetailDT;
            OnStorageDetailBS.DataSource = OnStorageDetailDT;
            OnExpeditionDetailBS.DataSource = OnExpeditionDetailDT;

            AllAssignmentsBS.DataSource = AllAssignmentsDT;
            DocumentsBS.DataSource = DocumentsDT;
            NewAssignmentDetailsBS.DataSource = NewAssignmentDetailsDT;
        }

        public DataGridViewComboBoxColumn ClientColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientColumn",
                    HeaderText = "Клиент",
                    DataPropertyName = "ClientID",
                    DataSource = new DataView(ClientsDT),
                    ValueMember = "ClientID",
                    DisplayMember = "ClientName",
                    ReadOnly = true,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
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
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CreationUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CreationUserColumn",
                    HeaderText = "Создал",
                    DataPropertyName = "CreationUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    ReadOnly = true,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PrintUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PrintUserColumn",
                    HeaderText = "Распечатал",
                    DataPropertyName = "PrintUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn AgreementUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "AgreementUserColumn",
                    HeaderText = "Согласовал",
                    DataPropertyName = "AgreementUserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "ShortName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechStoreNameColumn1
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechStoreNameColumn",
                    HeaderText = "Материал",
                    DataPropertyName = "TechStoreID",
                    DataSource = new DataView(TechStoreDT),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechStoreSubGroupColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechStoreSubGroupColumn",
                    HeaderText = "Продукт",
                    DataPropertyName = "TechStoreSubGroupID",
                    DataSource = new DataView(TechStoreSubGroupsDT),
                    ValueMember = "TechStoreSubGroupID",
                    DisplayMember = "TechStoreSubGroupName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CTechStoreNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CTechStoreNameColumn",
                    HeaderText = "Наименование",
                    DataPropertyName = "CTechStoreID",
                    DataSource = new DataView(TechStoreDT),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn TechStoreNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "TechStoreNameColumn",
                    HeaderText = "Наименование",
                    DataPropertyName = "TechStoreID",
                    DataSource = new DataView(TechStoreDT),
                    ValueMember = "TechStoreID",
                    DisplayMember = "TechStoreName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn MeasuresColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MeasuresColumn",
                    HeaderText = "Ед.изм.",
                    DataPropertyName = "MeasureID",
                    DataSource = new DataView(MeasuresDT),
                    ValueMember = "MeasureID",
                    DisplayMember = "Measure",
                    ReadOnly = true,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 55
                };
                return Column;
            }
        }
        
        public DataGridViewComboBoxColumn ColorColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ColorColumn",
                    HeaderText = "Цвет",
                    DataPropertyName = "ColorID",
                    DataSource = new DataView(ColorsDT),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CoverColumn1
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CoverColumn1",
                    HeaderText = "Облицовка шкафа",
                    DataPropertyName = "CoverID1",
                    DataSource = new DataView(CoversDT),
                    ValueMember = "CoverID",
                    DisplayMember = "CoverName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CoverColumn2
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CoverColumn2",
                    HeaderText = "Облицовка материала",
                    DataPropertyName = "CoverID2",
                    DataSource = new DataView(CoversDT),
                    ValueMember = "CoverID",
                    DisplayMember = "CoverName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CoverColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CoverColumn",
                    HeaderText = "Облицовка",
                    DataPropertyName = "CoverID",
                    DataSource = new DataView(CoversDT),
                    ValueMember = "CoverID",
                    DisplayMember = "CoverName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PatinaColumn1
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn1",
                    HeaderText = "Патина шкафа",
                    DataPropertyName = "PatinaID1",
                    DataSource = new DataView(PatinaDT),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }
        
        public DataGridViewComboBoxColumn PatinaColumn2
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PatinaColumn2",
                    HeaderText = "Патина материала",
                    DataPropertyName = "PatinaID2",
                    DataSource = new DataView(PatinaDT),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
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
                    DataSource = new DataView(PatinaDT),
                    ValueMember = "PatinaID",
                    DisplayMember = "PatinaName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }
        
        public DataGridViewComboBoxColumn InsetTypeColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "InsetTypeColumn",
                    HeaderText = "Тип наполнителя",
                    DataPropertyName = "InsetTypeID",
                    DataSource = new DataView(InsetTypesDT),
                    ValueMember = "InsetTypeID",
                    DisplayMember = "InsetTypeName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }
        
        public DataGridViewComboBoxColumn InsetColorColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn();
                Column = new DataGridViewComboBoxColumn()
                {
                    Name = "InsetColorColumn",
                    HeaderText = "Цвет наполнителя",
                    DataPropertyName = "InsetColorID",
                    DataSource = new DataView(InsetColorsDT),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
        }

        public void FilterTechStoreSubGroups(int TechStoreGroupID)
        {
            TechStoreSubGroupsBS.Filter = "TechStoreGroupID = " + TechStoreGroupID;
            TechStoreSubGroupsBS.MoveFirst();
        }

        public void FilterTechStore(int TechStoreSubGroupID)
        {
            TechStoreBS.Filter = "TechStoreSubGroupID = " + TechStoreSubGroupID;
            TechStoreBS.MoveFirst();
        }

        public void FilterCovers(int TechStoreID)
        {
            TempCoversDT.Clear();
            using (DataView DV = new DataView(TechStoreDT))
            {
                DV.RowFilter = "TechStoreID=" + TechStoreID;
                TempCoversDT = DV.ToTable(true, new string[] { "CoverID" });
            }
            string filter = string.Empty;
            for (int i = 0; i < TempCoversDT.Rows.Count; i++)
                filter += Convert.ToInt32(TempCoversDT.Rows[i]["CoverID"]) + ",";

            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "CoverID IN (" + filter + ")";
            }
            else
                filter = "CoverID <> - 1";
            CoversBS.Filter = filter;
            CoversBS.MoveFirst();
        }

        public void FilterPatina(int TechStoreID, int CoverID)
        {
            TempPatinaDT.Clear();
            using (DataView DV = new DataView(TechStoreDT))
            {
                DV.RowFilter = "TechStoreID=" + TechStoreID;
                DV.RowFilter = "CoverID=" + CoverID;
                DV.RowFilter = "PatinaID IS NOT NULL";
                TempPatinaDT = DV.ToTable(true, new string[] { "PatinaID" });
            }
            string filter = string.Empty;
            for (int i = 0; i < TempPatinaDT.Rows.Count; i++)
                filter += Convert.ToInt32(TempPatinaDT.Rows[i]["PatinaID"]) + ",";

            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "PatinaID IN (" + filter + ")";
            }
            else
                filter = "PatinaID <> - 1";
            PatinaBS.Filter = filter;
            PatinaBS.MoveFirst();
        }

        public void FilterBasicTechStoreSubGroups(int TechStoreGroupID)
        {
            BasicTechStoreSubGroupsBS.Filter = "TechStoreGroupID = " + TechStoreGroupID;
            BasicTechStoreSubGroupsBS.MoveFirst();
        }

        public void FilterBasicTechStore(int TechStoreSubGroupID)
        {
            BasicTechStoreBS.Filter = "TechStoreSubGroupID = " + TechStoreSubGroupID;
            BasicTechStoreBS.MoveFirst();
        }

        public void SaveDocuments()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM CabFurnitureDocuments";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DocumentsDT);
                }
            }
        }
        
        public void SaveNewAssignment()
        {
            NewAssignmentDA.Update(NewAssignmentDetailsDT);
        }

        public void SaveAllAssignments()
        {
            AllAssignmentsDA.Update(AllAssignmentsDT);
        }

        public void UpdateDocuments()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurnitureDocuments",
                ConnectionStrings.StorageConnectionString))
            {
                DocumentsDT.Clear();
                DA.Fill(DocumentsDT);
            }
        }

        public void UpdateNewAssignment(int CabFurAssignmentID)
        {
            NewAssignmentDetailsDT.Clear();
            string SelectCommand = @"SELECT * FROM CabFurnitureAssignmentDetails WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            NewAssignmentDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            NewAssignmentCB = new SqlCommandBuilder(NewAssignmentDA);
            NewAssignmentDA.Fill(NewAssignmentDetailsDT);
        }

        public void UpdateAllAssignments()
        {
            AllAssignmentsDT.Clear();
            AllAssignmentsDA.Fill(AllAssignmentsDT);

            for (int i = 0; i < AllAssignmentsDT.Rows.Count; i++)
            {
                int PrintStatus = 0;
                int CabFurAssignmentID = Convert.ToInt32(AllAssignmentsDT.Rows[i]["CabFurAssignmentID"]);
                int DocsCount = 0;
                int DocsPrintedCount = 0;

                DataRow[] rows = DocumentsDT.Select("CabFurAssignmentID=" + CabFurAssignmentID);
                DocsCount = rows.Count();
                rows = DocumentsDT.Select("PrintUserID IS NOT NULL AND CabFurAssignmentID=" + CabFurAssignmentID);
                DocsPrintedCount = rows.Count();

                if (DocsCount == 0)
                    PrintStatus = 0;
                else
                {
                    if (DocsPrintedCount > 0)
                        PrintStatus = 2;
                    if (DocsCount == DocsPrintedCount)
                        PrintStatus = 1;
                }

                AllAssignmentsDT.Rows[i]["DocsCount"] = DocsCount;
                AllAssignmentsDT.Rows[i]["DocsPrintedCount"] = DocsPrintedCount;
                AllAssignmentsDT.Rows[i]["PrintStatus"] = PrintStatus;
            }
        }

        public void AddAssignment()
        {
            string SelectCommand = @"SELECT TOP 1 * FROM CabFurnitureAssignments ORDER BY CabFurAssignmentID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["CreationUserID"] = Security.CurrentUserID;
                        NewRow["CreationDateTime"] = Security.GetCurrentDate();
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                        DT.Clear();

                        if (DA.Fill(DT) > 0)
                            iCabFurAssignmentID = Convert.ToInt32(DT.Rows[0]["CabFurAssignmentID"]);
                    }
                }
            }
        }

        public void AddAssignmentDetail(int TechStoreID, int CoverID, int PatinaID, int InsetColorID, int Count)
        {
            DataRow NewRow = NewAssignmentDetailsDT.NewRow();
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();

            string SelectCommand = @"SELECT * FROM TechStore WHERE TechStoreID=" + TechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreItemsDT.Clear();
                DA.Fill(StoreItemsDT);
            }
            foreach (DataColumn Col in StoreItemsDT.Columns)
            {
                string str = Col.ColumnName;
                if (NewAssignmentDetailsDT.Columns.Contains(str))
                    NewRow[str] = StoreItemsDT.Rows[0][str];
            }
            NewRow["CabFurAssignmentID"] = CabFurAssignmentID;
            NewRow["CoverID"] = CoverID;
            NewRow["PatinaID"] = PatinaID;
            NewRow["InsetColorID"] = InsetColorID;
            NewRow["Count"] = Count;

            NewAssignmentDetailsDT.Rows.Add(NewRow);
        }

        public void AddCabFurnitureDocument()
        {
            DataRow NewRow = DocumentsDT.NewRow();
            NewRow["CreationUserID"] = Security.CurrentUserID;
            NewRow["CreationDateTime"] = Security.GetCurrentDate();
            DocumentsDT.Rows.Add(NewRow);
        }

        public void RemoveNewAssignmentDetail()
        {
            if (NewAssignmentDetailsBS.Count == 0)
                return;

            int Pos = NewAssignmentDetailsBS.Position;

            NewAssignmentDetailsBS.RemoveCurrent();

            if (NewAssignmentDetailsBS.Count > 0)
                if (Pos >= NewAssignmentDetailsBS.Count)
                    NewAssignmentDetailsBS.MoveLast();
                else
                    NewAssignmentDetailsBS.Position = Pos;
        }

        public void RemoveAssignment()
        {
            if (AllAssignmentsBS.Count == 0)
                return;

            int Pos = AllAssignmentsBS.Position;

            AllAssignmentsBS.RemoveCurrent();

            if (AllAssignmentsBS.Count > 0)
                if (Pos >= AllAssignmentsBS.Count)
                    AllAssignmentsBS.MoveLast();
                else
                    AllAssignmentsBS.Position = Pos;
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

        public void PrintAssignment(int CabFurAssignmentID)
        {
            DataRow[] rows = DocumentsDT.Select("CabFurAssignmentID=" + CabFurAssignmentID);
            if (rows.Count() == 0)
                return;
            DateTime PrintDateTime = Security.GetCurrentDate();
            for (int i = 0; i < rows.Count(); i++)
            {
                if (rows[i]["PrintUserID"] == DBNull.Value)
                    rows[i]["PrintUserID"] = Security.CurrentUserID;
                if (rows[i]["PrintDateTime"] == DBNull.Value)
                    rows[i]["PrintDateTime"] = PrintDateTime;
            }
        }

        public void PrintDocument(int CabFurDocumentID)
        {
            DataRow[] rows = DocumentsDT.Select("CabFurDocumentID=" + CabFurDocumentID);
            if (rows.Count() == 0)
                return;
            if (rows[0]["PrintUserID"] == DBNull.Value)
                rows[0]["PrintUserID"] = Security.CurrentUserID;
            if (rows[0]["PrintDateTime"] == DBNull.Value)
                rows[0]["PrintDateTime"] = Security.GetCurrentDate();
        }

        public string GetAssignmentName(int CabFurAssignmentID, int SubSectorID)
        {
            string name = string.Empty;



            return name;

        }

        public string GetDocNumber(int CabFurAssignmentID, int SubSectorID, int CabFurDocTypeID)
        {
            StringBuilder sb = new StringBuilder(CabFurAssignmentID);
            sb.Append(".");

            if (SubSectorID.ToString().Length == 1)
                sb.Append("00" + SubSectorID);
            if (SubSectorID.ToString().Length == 2)
                sb.Append("0" + SubSectorID);
            sb.Append(".");

            if (CabFurDocTypeID.ToString().Length == 1)
                sb.Append("00" + CabFurDocTypeID);
            if (CabFurDocTypeID.ToString().Length == 2)
                sb.Append("0" + CabFurDocTypeID);
            if (CabFurDocTypeID.ToString().Length == 3)
                sb.Append(CabFurDocTypeID);

            return sb.ToString();

        }

        public void AgreeAssignment(int CabFurAssignmentID)
        {
            DataRow[] rows = AllAssignmentsDT.Select("CabFurAssignmentID=" + CabFurAssignmentID);
            if (rows.Count() == 0)
                return;
            if (rows[0]["AgreementUserID"] == DBNull.Value)
                rows[0]["AgreementUserID"] = Security.CurrentUserID;
            if (rows[0]["AgreementDateTime"] == DBNull.Value)
                rows[0]["AgreementDateTime"] = Security.GetCurrentDate();
        }

        public void FillAssignmentID(int CabFurAssignmentID)
        {
            for (int i = 0; i < NewAssignmentDetailsDT.Rows.Count; i++)
            {
                if (NewAssignmentDetailsDT.Rows[i].RowState == DataRowState.Deleted)
                    continue;
                if (NewAssignmentDetailsDT.Rows[i]["CabFurAssignmentID"] == DBNull.Value || Convert.ToInt32(NewAssignmentDetailsDT.Rows[i]["CabFurAssignmentID"]) == 0)
                    NewAssignmentDetailsDT.Rows[i]["CabFurAssignmentID"] = CabFurAssignmentID;
            }
        }

        public void SetWorkAssignmentAgreementPermissions(int WorkAssignmentID, int Permission)
        {
            DataRow[] rows = NewAssignmentDetailsDT.Select("WorkAssignmentID=" + WorkAssignmentID);
            if (rows.Count() == 0)
                return;
            if (Permission == 0)
            {
                if (rows[0]["ResponsibleUserID"] == DBNull.Value)
                    rows[0]["ResponsibleUserID"] = Security.CurrentUserID;
                if (rows[0]["ResponsibleDateTime"] == DBNull.Value)
                    rows[0]["ResponsibleDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 1)
            {
                if (rows[0]["TechnologyUserID"] == DBNull.Value)
                    rows[0]["TechnologyUserID"] = Security.CurrentUserID;
                if (rows[0]["TechnologyDateTime"] == DBNull.Value)
                    rows[0]["TechnologyDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 2)
            {
                if (rows[0]["ControlUserID"] == DBNull.Value)
                    rows[0]["ControlUserID"] = Security.CurrentUserID;
                if (rows[0]["ControlDateTime"] == DBNull.Value)
                    rows[0]["ControlDateTime"] = Security.GetCurrentDate();
            }
            if (Permission == 3)
            {
                if (rows[0]["AgreementUserID"] == DBNull.Value)
                    rows[0]["AgreementUserID"] = Security.CurrentUserID;
                if (rows[0]["AgreementDateTime"] == DBNull.Value)
                    rows[0]["AgreementDateTime"] = Security.GetCurrentDate();
            }
        }

        public int NewProdAssignmentID
        {
            get
            {
                if (AllAssignmentsDT.Rows.Count > 0)
                    iCabFurAssignmentID = Convert.ToInt32(AllAssignmentsDT.Rows[0]["CabFurAssignmentID"]);
                else
                    iCabFurAssignmentID = 0;
                return iCabFurAssignmentID;
            }
        }

        public void MoveToAssignmentID(int CabFurAssignmentID)
        {
            AllAssignmentsBS.Position = AllAssignmentsBS.Find("CabFurAssignmentID", CabFurAssignmentID);
        }

        public void MoveToAssignmentPos(int Pos)
        {
            AllAssignmentsBS.Position = Pos;
        }

        public void MoveToFirstAssignmentID()
        {
            AllAssignmentsBS.MoveFirst();
        }

        public void MoveToLastAssignmentID()
        {
            AllAssignmentsBS.MoveLast();
            if (AllAssignmentsDT.Rows.Count > 0)
                iCabFurAssignmentID = Convert.ToInt32(AllAssignmentsDT.Rows[AllAssignmentsDT.Rows.Count - 1]["CabFurAssignmentID"]);
            else
                iCabFurAssignmentID = 0;
        }
        
        public void NonAgreementOrders(bool bClient, ref decimal TPSCount)
        {
            //НА СОГЛАСОВАНИИ
            string SelectCommand = @"SELECT DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(DecorOrders.Count) AS Count FROM DecorOrders
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    AND DecorOrders.ProductID=46 AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                    WHERE TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1
                    AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE (AgreementStatusID=0 AND CreatedByClient=0) OR AgreementStatusID=1))
                    GROUP BY DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";

            if (bClient)
                SelectCommand = @"SELECT ClientName, DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(DecorOrders.Count) AS Count FROM DecorOrders
                    INNER JOIN dbo.MainOrders ON dbo.DecorOrders.MainOrderID = dbo.MainOrders.MainOrderID 
                    INNER JOIN dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID 
                    INNER JOIN infiniu2_marketingreference.dbo.Clients ON dbo.MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    AND DecorOrders.ProductID=46 AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                    WHERE TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1
                    AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE (AgreementStatusID=0 AND CreatedByClient=0) OR AgreementStatusID=1))
                    GROUP BY ClientName, DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                NonAgreementDetailDT.Clear();
                DA.Fill(NonAgreementDetailDT);

                foreach (DataRow Row in NonAgreementDetailDT.Rows)
                    TPSCount += Convert.ToDecimal(Row["Count"]);
            }
        }

        public void AgreedOrders(bool bClient, ref decimal TPSCount)
        {
            //СОГЛАСОВАНЫ
            string SelectCommand = @"SELECT DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(DecorOrders.Count) AS Count FROM DecorOrders
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    AND DecorOrders.ProductID=46 AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                    WHERE TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1
                    AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE AgreementStatusID=2))
                    GROUP BY DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";

            if (bClient)
                SelectCommand = @"SELECT ClientName, DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(DecorOrders.Count) AS Count FROM DecorOrders
                    INNER JOIN dbo.MainOrders ON dbo.DecorOrders.MainOrderID = dbo.MainOrders.MainOrderID 
                    INNER JOIN dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID 
                    INNER JOIN infiniu2_marketingreference.dbo.Clients ON dbo.MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    AND DecorOrders.ProductID=46 AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                    WHERE TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1
                    AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE AgreementStatusID=2))
                    GROUP BY ClientName, DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AgreedDetailDT.Clear();
                DA.Fill(AgreedDetailDT);

                foreach (DataRow Row in AgreedDetailDT.Rows)
                    TPSCount += Convert.ToDecimal(Row["Count"]);
            }
        }

        public void OnProductionOrders(bool bClient, ref decimal TPSCount)
        {
            //НА ПРОИЗВОДСТВЕ
            string SelectCommand = @"SELECT DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(DecorOrders.Count) AS Count FROM DecorOrders
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    AND DecorOrders.ProductID=46 AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                    WHERE TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)
                    GROUP BY DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";

            if (bClient)
                SelectCommand = @"SELECT ClientName, DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(DecorOrders.Count) AS Count FROM DecorOrders
                    INNER JOIN dbo.MainOrders ON dbo.DecorOrders.MainOrderID = dbo.MainOrders.MainOrderID 
                    INNER JOIN dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID 
                    INNER JOIN infiniu2_marketingreference.dbo.Clients ON dbo.MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    AND DecorOrders.ProductID=46 AND DecorOrders.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                    WHERE TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSExpeditionStatusID=1 AND TPSDispatchStatusID=1)
                    GROUP BY ClientName, DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                OnProductionDetailDT.Clear();
                DA.Fill(OnProductionDetailDT);

                foreach (DataRow Row in OnProductionDetailDT.Rows)
                    TPSCount += Convert.ToDecimal(Row["Count"]);
            }
        }

        public void InProductionOrders(bool bClient, ref decimal TPSCount)
        {
            //В ПРОИЗВОДСТВЕ
            string SelectCommand = @"SELECT DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(PackageDetails.Count) AS Count FROM DecorOrders
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    INNER JOIN PackageDetails ON DecorOrders.DecorOrderID=PackageDetails.OrderID
                    INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.PackageStatusID IN (0) AND Packages.ProductType=1
                    WHERE DecorOrders.ProductID=46
                    GROUP BY DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";

            if (bClient)
                SelectCommand = @"SELECT ClientName, DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(PackageDetails.Count) AS Count FROM DecorOrders
                    INNER JOIN dbo.MainOrders ON dbo.DecorOrders.MainOrderID = dbo.MainOrders.MainOrderID 
                    INNER JOIN dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID 
                    INNER JOIN infiniu2_marketingreference.dbo.Clients ON dbo.MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    INNER JOIN PackageDetails ON DecorOrders.DecorOrderID=PackageDetails.OrderID
                    INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.PackageStatusID IN (0) AND Packages.ProductType=1
                    WHERE DecorOrders.ProductID=46
                    GROUP BY ClientName, DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                InProductionDetailDT.Clear();
                DA.Fill(InProductionDetailDT);

                foreach (DataRow Row in InProductionDetailDT.Rows)
                    TPSCount += Convert.ToDecimal(Row["Count"]);
            }
        }

        public void OnStorageOrders(bool bClient, ref decimal TPSCount)
        {
            string SelectCommand = @"SELECT DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(PackageDetails.Count) AS Count FROM DecorOrders
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    INNER JOIN PackageDetails ON DecorOrders.DecorOrderID=PackageDetails.OrderID
                    INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.PackageStatusID IN (1,2) AND Packages.ProductType=1
                    WHERE DecorOrders.ProductID=46
                    GROUP BY DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";

            if (bClient)
                SelectCommand = @"SELECT ClientName, DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(PackageDetails.Count) AS Count FROM DecorOrders
                    INNER JOIN dbo.MainOrders ON dbo.DecorOrders.MainOrderID = dbo.MainOrders.MainOrderID 
                    INNER JOIN dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID 
                    INNER JOIN infiniu2_marketingreference.dbo.Clients ON dbo.MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    INNER JOIN PackageDetails ON DecorOrders.DecorOrderID=PackageDetails.OrderID
                    INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.PackageStatusID IN (1,2) AND Packages.ProductType=1
                    WHERE DecorOrders.ProductID=46
                    GROUP BY ClientName, DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                OnStorageDetailDT.Clear();
                DA.Fill(OnStorageDetailDT);

                foreach (DataRow Row in OnStorageDetailDT.Rows)
                    TPSCount += Convert.ToDecimal(Row["Count"]);
            }
        }

        public void OnExpeditionOrders(bool bClient, ref decimal TPSCount)
        {
            string SelectCommand = @"SELECT DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(PackageDetails.Count) AS Count FROM DecorOrders
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    INNER JOIN PackageDetails ON DecorOrders.DecorOrderID=PackageDetails.OrderID
                    INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.PackageStatusID=4 AND Packages.ProductType=1
                    WHERE DecorOrders.ProductID=46
                    GROUP BY DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";

            if (bClient)
                SelectCommand = @"SELECT ClientName, DecorOrders.DecorID AS TechStoreID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID, SUM(PackageDetails.Count) AS Count FROM DecorOrders
                    INNER JOIN dbo.MainOrders ON dbo.DecorOrders.MainOrderID = dbo.MainOrders.MainOrderID 
                    INNER JOIN dbo.MegaOrders ON dbo.MainOrders.MegaOrderID = dbo.MegaOrders.MegaOrderID 
                    INNER JOIN infiniu2_marketingreference.dbo.Clients ON dbo.MegaOrders.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
                    INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID
                    INNER JOIN PackageDetails ON DecorOrders.DecorOrderID=PackageDetails.OrderID
                    INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID AND Packages.PackageStatusID=4 AND Packages.ProductType=1
                    WHERE DecorOrders.ProductID=46
                    GROUP BY ClientName, DecorOrders.DecorID, MeasureID, DecorOrders.ColorID, DecorOrders.PatinaID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                OnExpeditionDetailDT.Clear();
                DA.Fill(OnExpeditionDetailDT);

                foreach (DataRow Row in OnExpeditionDetailDT.Rows)
                    TPSCount += Convert.ToDecimal(Row["Count"]);
            }
        }

        public void DeleteAssignment(int CabFurAssignmentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM CabFurnitureAssignmentDetails WHERE CabFurAssignmentID = " + CabFurAssignmentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM CabFurnitureAssignments WHERE CabFurAssignmentID = " + CabFurAssignmentID,
                ConnectionStrings.StorageConnectionString))
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

        public void FilterAllAssignments(bool bAgreed, bool bNotAgreed, bool bPrinted, bool bNotPrinted, bool bPartPrinted)
        {
            string filtr1 = string.Empty;
            string filtr2 = string.Empty;
            if (bAgreed)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (AgreementUserID IS NOT NULL)";
                else
                    filtr1 = "(AgreementUserID IS NOT NULL)";
            }
            if (bNotAgreed)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (AgreementUserID IS NULL)";
                else
                    filtr1 = "(AgreementUserID IS NULL)";
            }
            if (bPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintStatus=1)";
                else
                    filtr2 = "(PrintStatus=1)";
            }
            if (bNotPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintStatus=0)";
                else
                    filtr2 = "(PrintStatus=0)";
            }
            if (bPartPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintStatus=2)";
                else
                    filtr2 = "(PrintStatus=2)";
            }

            if (filtr2.Length > 0)
                filtr2 = "(" + filtr2 + ")";
            else
                filtr2 = "(PrintStatus=-1)";
            if (filtr1.Length > 0)
            {
                filtr1 = "(" + filtr1 + ")";
                if (filtr2.Length > 0)
                    filtr1 += " AND (" + filtr2 + ")";
            }
            else
                filtr1 = "(AgreementUserID=-1)";

            AllAssignmentsBS.Filter = filtr1;
        }
        
        public void FilterDocuments(int CabFurAssignmentID)
        {
            DocumentsBS.Filter = "CabFurAssignmentID = " + CabFurAssignmentID;
        }

        public bool SaveDocToFTPAndBase(string FileName, string Extension, string Path, int CabFurDocTypeID, int CabFurAssignmentID)
        {
            //write to ftp
            try
            {
                string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("CabFurnitureDocuments");
                string sExtension = Extension;
                string sFileName = FileName;

                int j = 1;
                while (FM.FileExist(sDestFolder + "/" + sFileName + sExtension, Configs.FTPType))
                {
                    sFileName = FileName + "(" + j++ + ")";
                }
                FileName = sFileName + sExtension;
                if (FM.UploadFile(Path, sDestFolder + "/" + sFileName + sExtension, Configs.FTPType) == false)
                    return false;
            }
            catch
            {
                return false;
            }
            //write to database
            string SelectCommand = @"SELECT TOP 1 * FROM CabFurnitureDocuments ORDER BY CabFurAssignmentID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        FileInfo fi;

                        try
                        {
                            fi = new FileInfo(Path);
                        }
                        catch
                        {
                            return false;
                        }

                        DataRow NewRow = DT.NewRow();
                        NewRow["SectorID"] = CabFurDocTypeID;
                        NewRow["CabFurAssignmentID"] = CabFurAssignmentID;
                        NewRow["FileName"] = FileName;
                        NewRow["CreationUserID"] = Security.CurrentUserID;
                        NewRow["CreationDateTime"] = Security.GetCurrentDate();
                        NewRow["FileSize"] = fi.Length;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }
            return true;
        }

        public string OpenDocument(int CabFurDocumentID)
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurnitureDocuments WHERE CabFurDocumentID = " + CabFurDocumentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("CabFurnitureDocuments") + "/" + FileName,
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

        public void SaveDocumentOnComputer(int CabFurDocumentID, string sDistFileName)
        {
            string FileName = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurnitureDocuments WHERE CabFurDocumentID = " + CabFurDocumentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("CabFurnitureDocuments") + "/" + FileName,
                            sDistFileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return;
                    }

                }
            }
        }

        public string GetDocTypeName(int CabFurDocTypeID)
        {
            DataRow[] rows = CabFurnitureDocumentTypesDT.Select("CabFurDocTypeID = " + CabFurDocTypeID);
            if (rows.Count() > 0)
            {
                return rows[0]["DocName"].ToString();
            }
            return string.Empty;
        }

        public string GetCoverName(int CoverID)
        {
            DataRow[] rows = CoversDT.Select("CoverID = " + CoverID);
            if (rows.Count() > 0)
            {
                return rows[0]["CoverName"].ToString();
            }
            return string.Empty;
        }

        public string GetPatinaName(int PatinaID)
        {
            DataRow[] rows = PatinaDT.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
            {
                return rows[0]["PatinaName"].ToString();
            }
            return string.Empty;
        }

        public string GetInsetColorName(int ColorID)
        {
            string ColorName = string.Empty;
            try
            {
                DataRow[] Rows = InsetColorsDT.Select("InsetColorID = " + ColorID);
                ColorName = Rows[0]["InsetColorName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ColorName;
        }

        public string TechStoreName(int TechStoreID)
        {
            DataRow[] rows = TechStoreDT.Select("TechStoreID = " + TechStoreID);
            if (rows.Count() > 0)
            {
                return rows[0]["TechStoreName"].ToString();
            }
            return string.Empty;
        }

        public string StoreName(int TechStoreID)
        {
            DataRow[] rows = TechStoreDT.Select("TechStoreID = " + TechStoreID);
            if (rows.Count() > 0)
            {
                return rows[0]["TechStoreSubGroupName"].ToString();
            }
            return string.Empty;
        }
        
        public void SaveComplementsCount(int CabFurAssignmentID, int ComplementsCount)
        {
            string SelectCommand = @"SELECT * FROM CabFurnitureAssignments WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ComplementsCount"] = ComplementsCount;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void SavePackagesCount(int CabFurAssignmentID, int PackagesCount)
        {
            string SelectCommand = @"SELECT * FROM CabFurnitureAssignments WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["PackagesCount"] = PackagesCount;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        private string GetTechStore(int CabFurAssignmentID)
        {
            string TechStoreName = string.Empty;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TechStoreName FROM TechStore WHERE TechStoreID=(SELECT TechStoreID FROM infiniu2_storage.dbo.CabFurnitureAssignmentDetails WHERE CabFurAssignmentID=" + CabFurAssignmentID + ")",
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        TechStoreName = DT.Rows[0]["TechStoreName"].ToString();
                }
            }

            return TechStoreName;
        }

        private int GetTechStoreSubGroupID(int TechStoreID)
        {
            int TechStoreSubGroupID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 TechStoreSubGroupID FROM TechStore WHERE TechStoreID=" + TechStoreID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                        TechStoreSubGroupID = Convert.ToInt32(DT.Rows[0]["TechStoreSubGroupID"]);
                }
            }

            return TechStoreSubGroupID;
        }
        
        private void CreatePackageDetails(int CabFurniturePackageID, int CoverID, int PatinaID, int InsetColorID, DataTable dt1)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM CabFurniturePackageDetails ORDER BY CabFurniturePackageID DESC",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DateTime CreateDateTime = Security.GetCurrentDate();
                        int CreateUserID = Security.CurrentUserID;

                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            int TechCatalogStoreDetailID = Convert.ToInt32(dt1.Rows[j]["TechCatalogStoreDetailID"]);
                            if (!CheckStoreDetailConditions(TechCatalogStoreDetailID, CoverID, PatinaID, InsetColorID))
                                continue;
                            DataRow NewRow = DT.NewRow();
                            NewRow["TechStoreID"] = Convert.ToInt32(dt1.Rows[j]["TechStoreID"]);
                            NewRow["CabFurniturePackageID"] = CabFurniturePackageID;
                            NewRow["Notes"] = dt1.Rows[j]["Notes"];
                            NewRow["Length"] = dt1.Rows[j]["Length"];
                            NewRow["Height"] = dt1.Rows[j]["Height"];
                            NewRow["Width"] = dt1.Rows[j]["Width"];
                            NewRow["Count"] = dt1.Rows[j]["Count"];
                            NewRow["CreateDateTime"] = CreateDateTime;
                            NewRow["CreateUserID"] = CreateUserID;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void CreatePackages(int CabFurAssignmentID, int FactoryID)
        {
            DataTable CabFurnitureAssignmentDetailsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT CabFurnitureAssignmentDetails.*, T.TechStoreSubGroupID FROM CabFurnitureAssignmentDetails " +
                "INNER JOIN infiniu2_catalog.dbo.TechStore AS T ON CabFurnitureAssignmentDetails.TechStoreID=T.TechStoreID WHERE CabFurAssignmentID=" + CabFurAssignmentID,
                ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(CabFurnitureAssignmentDetailsDT);
            }
            int AllPackagesCount = 0;
            for (int i = 0; i < CabFurnitureAssignmentDetailsDT.Rows.Count; i++)
            {
                int TechStoreID = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["TechStoreID"]);
                int CoverID = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["CoverID"]);
                int PatinaID = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["PatinaID"]);
                int InsetColorID = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["InsetColorID"]);
                int Count = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["Count"]);
                int TechStoreSubGroupID = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["TechStoreSubGroupID"]);
                int CabFurAssignmentDetailID = Convert.ToInt32(CabFurnitureAssignmentDetailsDT.Rows[i]["CabFurAssignmentDetailID"]);
                DateTime CreateDateTime = Security.GetCurrentDate();

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM CabFurniturePackages ORDER BY CabFurniturePackageID DESC",
                    ConnectionStrings.StorageConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            DataTable dt = GetCabFurPackages(TechStoreID);
                            if (dt != null & dt.Rows.Count > 0)
                            {
                                for (int c = 0; c < Count; c++)
                                {
                                    int PackNumber = 1;
                                    for (int j = 0; j < dt.Rows.Count; j++)
                                    {
                                        int TechCatalogOperationsDetailID = Convert.ToInt32(dt.Rows[j]["TechCatalogOperationsDetailID"]);

                                        DataTable dt1 = GetTechStoreDetail(TechCatalogOperationsDetailID);
                                        if (dt1.Rows.Count == 0)
                                            continue;
                                        AllPackagesCount++;
                                        DataRow NewRow = DT.NewRow();
                                        NewRow["CabFurAssignmentID"] = CabFurAssignmentID;
                                        NewRow["CabFurAssignmentDetailID"] = CabFurAssignmentDetailID;
                                        NewRow["PackNumber"] = PackNumber++;
                                        NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
                                        NewRow["TechStoreSubGroupID"] = TechStoreSubGroupID;
                                        NewRow["TechStoreID"] = TechStoreID;
                                        NewRow["CoverID"] = CoverID;
                                        NewRow["PatinaID"] = PatinaID;
                                        NewRow["InsetColorID"] = InsetColorID;
                                        NewRow["FactoryID"] = FactoryID;
                                        NewRow["PackagesCount"] = 1;
                                        NewRow["CreateDateTime"] = CreateDateTime;
                                        NewRow["CreateUserID"] = Security.CurrentUserID;
                                        DT.Rows.Add(NewRow);

                                        DA.Update(DT);
                                        DT.Clear();
                                        if (DA.Fill(DT) > 0)
                                        {
                                            int CabFurniturePackageID = Convert.ToInt32(DT.Rows[0]["CabFurniturePackageID"]);
                                            if (CabFurniturePackageID > 0)
                                            {
                                                CreatePackageDetails(CabFurniturePackageID, CoverID, PatinaID, InsetColorID, dt1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (AllPackagesCount > 0)
                SavePackagesCount(CabFurAssignmentID, AllPackagesCount);
            CabFurnitureAssignmentDetailsDT.Dispose();
        }
        
        public void ClearCabFurnitureComplenents(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE CabFurnitureComplementDetails WHERE CabFurnitureComplementID IN" +
                " (SELECT CabFurnitureComplementID FROM CabFurnitureComplements WHERE MainOrderID = " + MainOrderID + ")",
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE CabFurnitureComplements WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
            SaveComplementsCount(CabFurAssignmentID, 0);
        }

        public void ClearCabFurniturePackages(int CabFurAssignmentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE CabFurniturePackageDetails WHERE CabFurniturePackageID IN" +
                " (SELECT CabFurniturePackageID FROM CabFurniturePackages WHERE CabFurAssignmentID = " + CabFurAssignmentID + ")",
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE CabFurniturePackages WHERE CabFurAssignmentID=" + CabFurAssignmentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
            SavePackagesCount(CabFurAssignmentID, 0);
        }
        
        private DataTable GetCabFurPackages(int TechStoreID)
        {
            DataTable dt = new DataTable();
            string SelectCommand = @"SELECT DISTINCT dbo.MachinesOperations.MachinesOperationName, TechCatalogOperationsDetailID, TechCatalogOperationsDetail.SerialNumber
FROM dbo.TechCatalogOperationsDetail INNER JOIN
                         dbo.MachinesOperations ON dbo.TechCatalogOperationsDetail.MachinesOperationID = dbo.MachinesOperations.MachinesOperationID INNER JOIN
                         infiniu2_storage.dbo.CabFurnitureDocumentTypes AS C ON dbo.MachinesOperations.CabFurDocTypeID = C.CabFurDocTypeID AND(C.DocName LIKE '%упак%' OR
                         C.DocName LIKE '%компл%')
WHERE dbo.TechCatalogOperationsDetail.TechCatalogOperationsGroupID IN
                             (SELECT TechCatalogOperationsGroupID
                               FROM dbo.TechCatalogOperationsGroups
                               WHERE TechStoreID = " + TechStoreID + ") ORDER BY TechCatalogOperationsDetail.SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(dt);
            }

            return dt;
        }

        private DataTable GetTechStoreDetail(int TechCatalogOperationsDetailID)
        {
            DataTable dt = new DataTable();
            string SelectCommand = @"SELECT TechCatalogStoreDetail.*, TechStore.Notes  FROM TechCatalogStoreDetail INNER JOIN
                         TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID WHERE (IsHalfStuff2=0) AND TechCatalogOperationsDetailID = " + TechCatalogOperationsDetailID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(dt);
            }

            return dt;
        }

        private bool CheckStoreDetailConditions(int TechCatalogStoreDetailID, int CoverID, int PatinaID, int InsetColorID)
        {
            DataRow[] rows = StoreDetailTermsDT.Select("TechCatalogStoreDetailID=" + TechCatalogStoreDetailID);
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
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (CoverID == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (CoverID != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (CoverID >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (CoverID <= Term)
                            { }
                            else
                                return false;
                        }
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
                    case "InsetColorID":
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (InsetColorID == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (InsetColorID != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (InsetColorID >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (InsetColorID <= Term)
                            { }
                            else
                                return false;
                        }
                        break;
                }
            }
            return true;
        }

        public bool IsAssignmentDetailsEmpty(int CabFurAssignmentID)
        {
            bool bEmpty = true;
            string SelectCommand = @"SELECT * FROM CabFurnitureAssignmentDetails WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        bEmpty = false;
                    }
                }
            }
            return bEmpty;
        }

        public bool IsAssignmentPackagesEmpty(int CabFurAssignmentID)
        {
            bool bEmpty = true;
            string SelectCommand = @"SELECT * FROM CabFurniturePackages WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        bEmpty = false;
                    }
                }
            }
            return bEmpty;
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

            StringBuilder BarcodeNumber = new StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        public List<ComplementLabelInfo> CreateComplementLabels(int[] CabFurnitureComplementID, int[] Index, int ComplementsCount)
        {
            string filter = string.Empty;
            foreach (int item in CabFurnitureComplementID)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = " WHERE CabFurnitureComplements.CabFurnitureComplementID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            List<ComplementLabelInfo> Labels = new List<ComplementLabelInfo>();
            string SelectCommand = @"SELECT CabFurnitureComplements.*, C.ClientName, T.TechStoreSubGroupName FROM CabFurnitureComplements 
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS C ON CabFurnitureComplements.ClientID=C.ClientID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups AS T ON CabFurnitureComplements.TechStoreSubGroupID=T.TechStoreSubGroupID" + filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            string ClientName = DT.Rows[i]["ClientName"].ToString();
                            string OrderNumber = DT.Rows[i]["OrderNumber"].ToString();
                            int MainOrder = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
                            int PackNumber = Index[i];
                            string DocDateTime = DT.Rows[i]["CreateDateTime"].ToString();
                            //string TechStoreSubGroup = DT.Rows[i]["TechStoreSubGroupName"].ToString();
                            string TechStoreSubGroup = TechStoreName(Convert.ToInt32(DT.Rows[i]["TechStoreID"]));
                            string Cover = GetCoverName(Convert.ToInt32(DT.Rows[i]["CoverID"]));
                            string DispatchDate = "";
                            string BarcodeNumber = GetBarcodeNumber(20, Convert.ToInt32(DT.Rows[i]["CabFurnitureComplementID"]));
                            string Notes = DT.Rows[i]["Notes"].ToString();
                            int FactoryType = Convert.ToInt32(DT.Rows[i]["FactoryID"]);

                            Notes = Notes.Replace("\n", " ");
                            ComplementLabelInfo LabelInfo = new ComplementLabelInfo();
                            LabelInfo.ClientName = ClientName;
                            LabelInfo.OrderNumber = OrderNumber;
                            LabelInfo.MainOrder = MainOrder;
                            LabelInfo.PackNumber = PackNumber;
                            LabelInfo.TotalPackCount = ComplementsCount;
                            LabelInfo.DocDateTime = DocDateTime;
                            LabelInfo.TechStoreSubGroup = TechStoreSubGroup;
                            LabelInfo.Cover = Cover;
                            LabelInfo.DispatchDate = DispatchDate;
                            LabelInfo.BarcodeNumber = BarcodeNumber;
                            LabelInfo.Notes = Notes;
                            LabelInfo.FactoryType = FactoryType;

                            ComplementLabelDataDT.Clear();
                            SelectCommand = @"SELECT T.TechStoreName, CabFurnitureComplementDetails.* FROM CabFurnitureComplementDetails 
                INNER JOIN infiniu2_catalog.dbo.TechStore AS T ON CabFurnitureComplementDetails.TechStoreID=T.TechStoreID WHERE CabFurnitureComplementID=" + Convert.ToInt32(DT.Rows[i]["CabFurnitureComplementID"]);
                            using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                            {
                                using (DataTable DT1 = new DataTable())
                                {
                                    if (DA1.Fill(DT1) > 0)
                                    {
                                        for (int j = 0; j < DT1.Rows.Count; j++)
                                        {
                                            DataRow NewRow = ComplementLabelDataDT.NewRow();
                                            NewRow["Notes"] = DT1.Rows[j]["Notes"].ToString();
                                            NewRow["Name"] = DT1.Rows[j]["TechStoreName"].ToString();
                                            NewRow["Length"] = DT1.Rows[j]["Length"];
                                            NewRow["Height"] = DT1.Rows[j]["Height"];
                                            NewRow["Width"] = DT1.Rows[j]["Width"];
                                            NewRow["Count"] = DT1.Rows[j]["Count"];
                                            ComplementLabelDataDT.Rows.Add(NewRow);
                                        }
                                    }
                                }
                            }
                            LabelInfo.OrderData = ComplementLabelDataDT.Copy();

                            Labels.Add(LabelInfo);
                        }
                    }
                }
            }

            return Labels;
        }

        public List<ComplementLabelInfo> CreateComplementLabels(int CabFurAssignmentID, int ComplementsCount)
        {
            List<ComplementLabelInfo> Labels = new List<ComplementLabelInfo>();
            string SelectCommand = @"SELECT CabFurnitureComplements.*, C.ClientName, T.TechStoreSubGroupName FROM CabFurnitureComplements 
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS C ON CabFurnitureComplements.ClientID=C.ClientID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups AS T ON CabFurnitureComplements.TechStoreSubGroupID=T.TechStoreSubGroupID 
                WHERE CabFurnitureComplements.CabFurAssignmentID=" + CabFurAssignmentID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            //int TotalPackCount = DT.Select("TechStoreSubGroupID = " + Convert.ToInt32(DT.Rows[i]["TechStoreSubGroupID"])).Count();

                            string ClientName = DT.Rows[i]["ClientName"].ToString();
                            string OrderNumber = DT.Rows[i]["OrderNumber"].ToString();
                            int MainOrder = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
                            int PackNumber = i + 1;
                            string DocDateTime = DT.Rows[i]["CreateDateTime"].ToString();
                            //string TechStoreSubGroup = DT.Rows[i]["TechStoreSubGroupName"].ToString();
                            string TechStoreSubGroup = TechStoreName(Convert.ToInt32(DT.Rows[i]["TechStoreID"]));
                            string Cover = GetCoverName(Convert.ToInt32(DT.Rows[i]["CoverID"]));
                            string DispatchDate = "";
                            string BarcodeNumber = GetBarcodeNumber(20, Convert.ToInt32(DT.Rows[i]["CabFurnitureComplementID"]));
                            string Notes = DT.Rows[i]["Notes"].ToString();
                            int FactoryType = Convert.ToInt32(DT.Rows[i]["FactoryID"]);

                            Notes = Notes.Replace("\n", " ");
                            ComplementLabelInfo LabelInfo = new ComplementLabelInfo();
                            LabelInfo.ClientName = ClientName;
                            LabelInfo.OrderNumber = OrderNumber;
                            LabelInfo.MainOrder = MainOrder;
                            LabelInfo.PackNumber = PackNumber;
                            LabelInfo.TotalPackCount = ComplementsCount;
                            LabelInfo.DocDateTime = DocDateTime;
                            LabelInfo.TechStoreSubGroup = TechStoreSubGroup;
                            LabelInfo.Cover = Cover;
                            LabelInfo.DispatchDate = DispatchDate;
                            LabelInfo.BarcodeNumber = BarcodeNumber;
                            LabelInfo.Notes = Notes;
                            LabelInfo.FactoryType = FactoryType;

                            ComplementLabelDataDT.Clear();
                            SelectCommand = @"SELECT T.TechStoreName, CabFurnitureComplementDetails.* FROM CabFurnitureComplementDetails 
                INNER JOIN infiniu2_catalog.dbo.TechStore AS T ON CabFurnitureComplementDetails.TechStoreID=T.TechStoreID WHERE CabFurnitureComplementID=" + Convert.ToInt32(DT.Rows[i]["CabFurnitureComplementID"]);
                            using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                            {
                                using (DataTable DT1 = new DataTable())
                                {
                                    if (DA1.Fill(DT1) > 0)
                                    {
                                        for (int j = 0; j < DT1.Rows.Count; j++)
                                        {
                                            DataRow NewRow = ComplementLabelDataDT.NewRow();
                                            NewRow["Notes"] = DT1.Rows[j]["Notes"].ToString();
                                            NewRow["Name"] = DT1.Rows[j]["TechStoreName"].ToString();
                                            NewRow["Length"] = DT1.Rows[j]["Length"];
                                            NewRow["Height"] = DT1.Rows[j]["Height"];
                                            NewRow["Width"] = DT1.Rows[j]["Width"];
                                            NewRow["Count"] = DT1.Rows[j]["Count"];
                                            ComplementLabelDataDT.Rows.Add(NewRow);
                                        }
                                    }
                                }
                            }
                            LabelInfo.OrderData = ComplementLabelDataDT.Copy();

                            Labels.Add(LabelInfo);
                        }
                    }
                }
            }

            return Labels;
        }

        private int TotalPackagesCount(int CabFurAssignmentDetailID)
        {
            int PackagesCount = 1;
            int Count = 1;
            string SelectCommand = @"SELECT dbo.CabFurniturePackages.CabFurAssignmentDetailID, C.Count, COUNT(dbo.CabFurniturePackages.CabFurniturePackageID) AS PackagesCount
FROM dbo.CabFurniturePackages 
INNER JOIN dbo.CabFurnitureAssignmentDetails AS C ON dbo.CabFurniturePackages.CabFurAssignmentDetailID = C.CabFurAssignmentDetailID
WHERE dbo.CabFurniturePackages.CabFurAssignmentDetailID = " + CabFurAssignmentDetailID + " GROUP BY dbo.CabFurniturePackages.CabFurAssignmentDetailID, C.Count";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        PackagesCount = Convert.ToInt32(DT.Rows[0]["PackagesCount"]);
                        Count = Convert.ToInt32(DT.Rows[0]["Count"]);
                    }
                }
            }
            return PackagesCount / Count;
        }

        public List<PackageLabelInfo> CreatePackageLabels(int[] CabFurniturePackageID, int CabFurAssignmentID)
        {
            string filter = string.Empty;
            foreach (int item in CabFurniturePackageID)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = " WHERE CabFurniturePackages.CabFurniturePackageID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            List<PackageLabelInfo> Labels = new List<PackageLabelInfo>();
            string SelectCommand = @"SELECT CabFurniturePackages.*, TC.GroupA,TC.GroupB,TC.GroupC, T.TechStoreSubGroupName, CA.CreationDateTime AS CACreationDateTime FROM CabFurniturePackages
                INNER JOIN CabFurnitureAssignments AS CA ON CabFurniturePackages.CabFurAssignmentID=CA.CabFurAssignmentID
                INNER JOIN infiniu2_catalog.dbo.TechCatalogOperationsDetail AS TC ON CabFurniturePackages.TechCatalogOperationsDetailID=TC.TechCatalogOperationsDetailID
                INNER JOIN infiniu2_catalog.dbo.TechStoreSubGroups AS T ON CabFurniturePackages.TechStoreSubGroupID=T.TechStoreSubGroupID" + filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            //int TotalPackCount = DT.Select("TechStoreSubGroupID = " + Convert.ToInt32(DT.Rows[i]["TechStoreSubGroupID"])).Count();

                            string ClientName = DT.Rows[i]["ClientName"].ToString();
                            int PackNumber = Convert.ToInt32(DT.Rows[i]["PackNumber"]);
                            string DocDateTime = Convert.ToDateTime(DT.Rows[i]["CACreationDateTime"]).ToString("dd.MM.yyyy");
                            string AddToStorageDateTime = DT.Rows[i]["PrintDateTime"].ToString();
                            if (DT.Rows[i]["AddToStorageDateTime"] != DBNull.Value)
                                AddToStorageDateTime = DT.Rows[i]["AddToStorageDateTime"].ToString();
                            string TechStoreSubGroup = TechStoreName(Convert.ToInt32(DT.Rows[i]["TechStoreID"]));
                            bool GroupA = Convert.ToBoolean(DT.Rows[i]["GroupA"]);
                            bool GroupB = Convert.ToBoolean(DT.Rows[i]["GroupB"]);
                            bool GroupC = Convert.ToBoolean(DT.Rows[i]["GroupC"]);

                            int CoverID = Convert.ToInt32(DT.Rows[i]["CoverID"]);
                            int PatinaID = Convert.ToInt32(DT.Rows[i]["PatinaID"]);
                            int InsetColorID = Convert.ToInt32(DT.Rows[i]["InsetColorID"]);
                            string Cover = GetCoverName(Convert.ToInt32(DT.Rows[i]["CoverID"]));
                            string Patina = GetPatinaName(Convert.ToInt32(DT.Rows[i]["PatinaID"]));
                            string InsetColor = GetInsetColorName(Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                            string DispatchDate = "";
                            string BarcodeNumber = GetBarcodeNumber(21, Convert.ToInt32(DT.Rows[i]["CabFurniturePackageID"]));
                            int FactoryType = Convert.ToInt32(DT.Rows[i]["FactoryID"]);


                            PackageLabelInfo LabelInfo = new PackageLabelInfo();
                            LabelInfo.ClientName = ClientName;
                            LabelInfo.CabFurAssignmentID = CabFurAssignmentID;
                            LabelInfo.PackNumber = PackNumber;
                            LabelInfo.TotalPackCount = TotalPackagesCount(Convert.ToInt32(DT.Rows[i]["CabFurAssignmentDetailID"]));
                            LabelInfo.AssignmentCreateDateTime = DocDateTime;
                            LabelInfo.AddToStorageDateTime = AddToStorageDateTime;
                            LabelInfo.TechStoreSubGroup = TechStoreSubGroup;

                            LabelInfo.Cover = Cover + Patina + InsetColor;

                            if (GroupA && GroupB && GroupC)
                            {
                                if (CoverID != -1 && PatinaID != -1 && InsetColorID != -1)
                                    LabelInfo.Cover = Cover + "+" + Patina + "+" + InsetColor;

                                if (CoverID != -1 && PatinaID == -1 && InsetColorID == -1)
                                    LabelInfo.Cover = Cover;
                                if (CoverID == -1 && PatinaID != -1 && InsetColorID == -1)
                                    LabelInfo.Cover = Patina;
                                if (CoverID == -1 && PatinaID == -1 && InsetColorID != -1)
                                    LabelInfo.Cover = InsetColor;

                                if (CoverID != -1 && PatinaID != -1 && InsetColorID == -1)
                                    LabelInfo.Cover = Cover + "+" + Patina;
                                if (CoverID != -1 && PatinaID == -1 && InsetColorID != -1)
                                    LabelInfo.Cover = Cover + "+" + InsetColor;
                                if (CoverID == -1 && PatinaID != -1 && InsetColorID != -1)
                                    LabelInfo.Cover = Patina + "+" + InsetColor;
                            }

                            if (GroupA && !GroupB && !GroupC)
                                LabelInfo.Cover = Cover;
                            if (!GroupA && GroupB && !GroupC)
                                LabelInfo.Cover = Patina;
                            if (!GroupA && !GroupB && GroupC)
                                LabelInfo.Cover = InsetColor;

                            if (GroupA && GroupB && !GroupC)
                                LabelInfo.Cover = Cover + "+" + Patina;
                            if (GroupA && !GroupB && GroupC)
                                LabelInfo.Cover = Cover + "+" + InsetColor;
                            if (!GroupA && GroupB && GroupC)
                                LabelInfo.Cover = Patina + "+" + InsetColor;

                            LabelInfo.DispatchDate = DispatchDate;
                            LabelInfo.BarcodeNumber = BarcodeNumber;
                            LabelInfo.FactoryType = FactoryType;

                            PackageLabelDataDT.Clear();
                            SelectCommand = @"SELECT T.TechStoreName, CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN infiniu2_catalog.dbo.TechStore AS T ON CabFurniturePackageDetails.TechStoreID=T.TechStoreID WHERE CabFurniturePackageID=" + Convert.ToInt32(DT.Rows[i]["CabFurniturePackageID"]);
                            using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                            {
                                using (DataTable DT1 = new DataTable())
                                {
                                    if (DA1.Fill(DT1) > 0)
                                    {
                                        for (int j = 0; j < DT1.Rows.Count; j++)
                                        {
                                            DataRow NewRow = PackageLabelDataDT.NewRow();
                                            NewRow["Notes"] = DT1.Rows[j]["Notes"].ToString();
                                            NewRow["Name"] = DT1.Rows[j]["TechStoreName"].ToString();
                                            NewRow["Length"] = DT1.Rows[j]["Length"];
                                            NewRow["Height"] = DT1.Rows[j]["Height"];
                                            NewRow["Width"] = DT1.Rows[j]["Width"];
                                            NewRow["Count"] = DT1.Rows[j]["Count"];
                                            PackageLabelDataDT.Rows.Add(NewRow);
                                        }
                                    }
                                }
                            }
                            LabelInfo.OrderData = PackageLabelDataDT.Copy();

                            Labels.Add(LabelInfo);
                        }
                    }
                }
            }

            return Labels;
        }
    }

    public class ComplementsManager
    {
        DataTable ComplementsDT = null;
        public BindingSource ComplementsBS = null;
        SqlDataAdapter ComplementsDA;

        DataTable MainOrdersDT = null;
        public BindingSource MainOrdersBS = null;
        SqlDataAdapter MainOrdersDA;

        DataTable ComplementLabelsDT = null;
        DataTable TempComplementLabelsDT = null;
        public BindingSource ComplementLabelsBS = null;
        SqlDataAdapter ComplementLabelsDA;

        DataTable ComplementDetailsDT = null;
        public BindingSource ComplementDetailsBS = null;
        SqlDataAdapter ComplementDetailsDA;

        private DataTable ProductionStatusesDataTable = null;
        private DataTable StorageStatusesDataTable = null;
        private DataTable ExpeditionStatusesDataTable = null;
        private DataTable DispatchStatusesDataTable = null;
        
        public DataGridViewComboBoxColumn ProductionStatusColumn = null;
        public DataGridViewComboBoxColumn StorageStatusColumn = null;
        public DataGridViewComboBoxColumn ExpeditionStatusColumn = null;
        public DataGridViewComboBoxColumn DispatchStatusColumn = null;

        public ComplementsManager()
        {
            ComplementsDT = new DataTable();
            MainOrdersDT = new DataTable();
            ComplementLabelsDT = new DataTable();
            ComplementDetailsDT = new DataTable();
            TempComplementLabelsDT = new DataTable();

            ComplementsBS = new BindingSource();
            MainOrdersBS = new BindingSource();
            ComplementLabelsBS = new BindingSource();
            ComplementDetailsBS = new BindingSource();
            
            ProductionStatusesDataTable = new DataTable();
            StorageStatusesDataTable = new DataTable();
            ExpeditionStatusesDataTable = new DataTable();
            DispatchStatusesDataTable = new DataTable();
            
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ProductionStatuses WHERE ProductionStatusID <> -1", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ProductionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StorageStatuses WHERE StorageStatusID <> -1", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(StorageStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ExpeditionStatuses WHERE ExpeditionStatusID <> -1", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ExpeditionStatusesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DispatchStatuses WHERE DispatchStatusID <> -1", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DispatchStatusesDataTable);
            }
            
            string SelectCommand = @"SELECT TOP 0 CabFurnitureComplementID, PackNumber, MainOrderID, TechStoreSubGroupID, PrintDateTime, PackingDateTime FROM CabFurnitureComplements";
            ComplementLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementLabelsDA.Fill(ComplementLabelsDT);
            ComplementLabelsDT.Columns.Add(new DataColumn("Index", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 C.MainOrderID, C.PackNumber, C.Notes AS CNotes, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID,
                CabFurnitureComplementDetails.* FROM CabFurnitureComplementDetails 
                INNER JOIN CabFurnitureComplements AS C ON CabFurnitureComplementDetails.CabFurnitureComplementID=C.CabFurnitureComplementID";
            ComplementDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementDetailsDA.Fill(ComplementDetailsDT);

            SelectCommand = @"SELECT TOP 0 ClientID, OrderNumber FROM CabFurnitureComplements";
            ComplementsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementsDA.Fill(ComplementsDT);
            ComplementsDT.Columns.Add(new DataColumn("ComplementsCount", Type.GetType("System.Int32")));
            ComplementsDT.Columns.Add(new DataColumn("PrintedPercentage", Type.GetType("System.Int32")));
            ComplementsDT.Columns.Add(new DataColumn("PackedCount", Type.GetType("System.String")));
            ComplementsDT.Columns.Add(new DataColumn("PackedPercentage", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 C.MainOrderID, M.Weight, M.AllocPackDateTime, M.DocDateTime, M.Notes, M.TPSProductionStatusID, M.TPSStorageStatusID, M.TPSExpeditionStatusID, M.TPSDispatchStatusID FROM CabFurnitureComplements AS C
INNER JOIN infiniu2_marketingorders.dbo.MainOrders AS M ON C.MainOrderID=M.MainOrderID";
            MainOrdersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            MainOrdersDA.Fill(MainOrdersDT);
            MainOrdersDT.Columns.Add(new DataColumn("PackedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("PackedPercentage", Type.GetType("System.Int32")));

            CreateColumns();

            ComplementsBS.DataSource = ComplementsDT;
            MainOrdersBS.DataSource = MainOrdersDT;
            ComplementLabelsBS.DataSource = ComplementLabelsDT;
            ComplementDetailsBS.DataSource = ComplementDetailsDT;
        }

        private void CreateColumns()
        {
            ProductionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ProductionStatusColumn",
                HeaderText = "Производство",
                DataPropertyName = "TPSProductionStatusID",
                DataSource = new DataView(ProductionStatusesDataTable),
                ValueMember = "ProductionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            StorageStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "StorageStatusColumn",
                HeaderText = "Склад",
                DataPropertyName = "TPSStorageStatusID",
                DataSource = new DataView(StorageStatusesDataTable),
                ValueMember = "StorageStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ExpeditionStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ExpeditionStatusColumn",
                HeaderText = "Экспедиция",
                DataPropertyName = "TPSExpeditionStatusID",
                DataSource = new DataView(ExpeditionStatusesDataTable),
                ValueMember = "ExpeditionStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            DispatchStatusColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DispatchStatusColumn",
                HeaderText = "Отгрузка",
                DataPropertyName = "TPSDispatchStatusID",
                DataSource = new DataView(DispatchStatusesDataTable),
                ValueMember = "DispatchStatusID",
                DisplayMember = "Status",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
        }
        
        public void FilterComplements(bool bPacked, bool bNotPacked, bool bPartPacked, bool bPrinted, bool bNotPrinted, bool bPartPrinted)
        {
            string filtr1 = string.Empty;
            string filtr2 = string.Empty;

            if (bPacked)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (PackedPercentage=100)";
                else
                    filtr1 = "(PackedPercentage=100)";
            }
            if (bNotPacked)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (PackedPercentage=0)";
                else
                    filtr1 = "(PackedPercentage=0)";
            }
            if (bPartPacked)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (PackedPercentage>0 AND PackedPercentage<100)";
                else
                    filtr1 = "(PackedPercentage>0 AND PackedPercentage<100)";
            }

            if (bPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintedPercentage=100)";
                else
                    filtr2 = "(PrintedPercentage=100)";
            }
            if (bNotPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintedPercentage=0)";
                else
                    filtr2 = "(PrintedPercentage=0)";
            }
            if (bPartPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintedPercentage>0 AND PrintedPercentage<100)";
                else
                    filtr2 = "(PrintedPercentage>0 AND PrintedPercentage<100)";
            }

            if (filtr2.Length > 0)
                filtr2 = "(" + filtr2 + ")";
            else
                filtr2 = "(PrintedPercentage=-1)";
            if (filtr1.Length > 0)
            {
                filtr1 = "(" + filtr1 + ")";
                if (filtr2.Length > 0)
                    filtr1 += " AND (" + filtr2 + ")";
            }
            else
                filtr1 = "(PackedPercentage=-1)";

            ComplementsBS.Filter = filtr1;
        }

        public void UpdateComplements(DateTime date1, DateTime date2)
        {
            string Filter = " WHERE CAST(CreateDateTime AS date) >= '" + date1.ToString("yyyy-MM-dd") +
                " 00:00' AND CAST(CreateDateTime AS date) <= '" + date2.ToString("yyyy-MM-dd") + " 23:59'";

            ComplementsDT.Clear();
            string SelectCommand = @"SELECT DISTINCT ClientID, OrderNumber FROM CabFurnitureComplements" + Filter;
            ComplementsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementsDA.Fill(ComplementsDT);

            TempComplementLabelsDT.Clear();

            SelectCommand = @"SELECT ClientID, OrderNumber, CabFurnitureComplementID, PackNumber, MainOrderID, TechStoreSubGroupID, PrintDateTime, PackingDateTime 
            FROM CabFurnitureComplements " + Filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(TempComplementLabelsDT);
            }

            for (int i = 0; i < ComplementsDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(ComplementsDT.Rows[i]["ClientID"]);
                int OrderNumber = Convert.ToInt32(ComplementsDT.Rows[i]["OrderNumber"]);
                int ComplementsCount = 0;

                DataRow[] rows = TempComplementLabelsDT.Select("ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
                if (rows.Count() > 0)
                    ComplementsCount = rows.Count();

                int PrintedCount = 0;
                int PrintedPercentage = 0;
                decimal PrintedProgressVal = 0;
                decimal d1 = 0;

                int PackedCount = 0;
                int PackedPercentage = 0;
                decimal PackedProgressVal = 0;
                decimal d2 = 0;

                rows = TempComplementLabelsDT.Select("PrintDateTime IS NOT NULL AND ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
                if (rows.Count() > 0)
                    PrintedCount = rows.Count();

                if (PrintedCount > 0)
                    PrintedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PrintedCount) / Convert.ToDecimal(ComplementsCount));
                d1 = PrintedProgressVal * 100;
                PrintedPercentage = Convert.ToInt32(Math.Truncate(d1));

                rows = TempComplementLabelsDT.Select("PackingDateTime IS NOT NULL AND ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
                if (rows.Count() > 0)
                    PackedCount = rows.Count();
                if (ComplementsCount > 0)
                    PackedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PackedCount) / Convert.ToDecimal(ComplementsCount));
                d2 = PackedProgressVal * 100;
                PackedPercentage = Convert.ToInt32(Math.Truncate(d2));

                ComplementsDT.Rows[i]["PrintedPercentage"] = PrintedPercentage;
                ComplementsDT.Rows[i]["ComplementsCount"] = ComplementsCount;
                ComplementsDT.Rows[i]["PackedCount"] = PackedCount + " / " + ComplementsCount;
                ComplementsDT.Rows[i]["PackedPercentage"] = PackedPercentage;
            }
        }

        public void UpdateComplements()
        {
            ComplementsDT.Clear();
            string SelectCommand = @"SELECT DISTINCT ClientID, OrderNumber FROM CabFurnitureComplements";
            ComplementsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementsDA.Fill(ComplementsDT);

            TempComplementLabelsDT.Clear();
            SelectCommand = @"SELECT ClientID, OrderNumber, CabFurnitureComplementID, PackNumber, MainOrderID, TechStoreSubGroupID, PrintDateTime, PackingDateTime FROM CabFurnitureComplements";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(TempComplementLabelsDT);
            }
            
            for (int i = 0; i < ComplementsDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(ComplementsDT.Rows[i]["ClientID"]);
                int OrderNumber = Convert.ToInt32(ComplementsDT.Rows[i]["OrderNumber"]);
                int ComplementsCount = 0;

                DataRow[] rows = TempComplementLabelsDT.Select("ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
                if (rows.Count() > 0)
                    ComplementsCount = rows.Count();

                int PrintedCount = 0;
                int PrintedPercentage = 0;
                decimal PrintedProgressVal = 0;
                decimal d1 = 0;

                int PackedCount = 0;
                int PackedPercentage = 0;
                decimal PackedProgressVal = 0;
                decimal d2 = 0;

                rows = TempComplementLabelsDT.Select("PrintDateTime IS NOT NULL AND ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
                if (rows.Count() > 0)
                    PrintedCount = rows.Count();

                if (PrintedCount > 0)
                    PrintedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PrintedCount) / Convert.ToDecimal(ComplementsCount));
                d1 = PrintedProgressVal * 100;
                PrintedPercentage = Convert.ToInt32(Math.Truncate(d1));

                rows = TempComplementLabelsDT.Select("PackingDateTime IS NOT NULL AND ClientID=" + ClientID + " AND OrderNumber=" + OrderNumber);
                if (rows.Count() > 0)
                    PackedCount = rows.Count();
                if (ComplementsCount > 0)
                    PackedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PackedCount) / Convert.ToDecimal(ComplementsCount));
                d2 = PackedProgressVal * 100;
                PackedPercentage = Convert.ToInt32(Math.Truncate(d2));

                ComplementsDT.Rows[i]["PrintedPercentage"] = PrintedPercentage;
                ComplementsDT.Rows[i]["ComplementsCount"] = ComplementsCount;
                ComplementsDT.Rows[i]["PackedCount"] = PackedCount + " / " + ComplementsCount;
                ComplementsDT.Rows[i]["PackedPercentage"] = PackedPercentage;
            }            
        }

        public void FilterMainOrders(int ClientID, int OrderNumber)
        {
            MainOrdersDT.Clear();
            string SelectCommand = @"SELECT DISTINCT C.MainOrderID, M.Weight, M.AllocPackDateTime, M.DocDateTime, M.Notes, M.TPSProductionStatusID, M.TPSStorageStatusID, M.TPSExpeditionStatusID, M.TPSDispatchStatusID FROM CabFurnitureComplements AS C
INNER JOIN infiniu2_marketingorders.dbo.MainOrders AS M ON C.MainOrderID=M.MainOrderID WHERE C.ClientID=" + ClientID + " AND C.OrderNumber=" + OrderNumber;
            MainOrdersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            MainOrdersDA.Fill(MainOrdersDT);

            TempComplementLabelsDT.Clear();
            SelectCommand = @"SELECT CabFurnitureComplementID, PackNumber, MainOrderID, TechStoreSubGroupID, PrintDateTime, PackingDateTime FROM CabFurnitureComplements AS C WHERE C.ClientID=" + ClientID + " AND C.OrderNumber=" + OrderNumber;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(TempComplementLabelsDT);
            }
            
            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                int MainOrderID = Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]);
                int ComplementsCount = 0;

                DataRow[] rows = TempComplementLabelsDT.Select("MainOrderID=" + MainOrderID);
                if (rows.Count() > 0)
                    ComplementsCount = rows.Count();

                int PackedCount = 0;
                int PackedPercentage = 0;
                decimal PackedProgressVal = 0;
                decimal d2 = 0;

                rows = TempComplementLabelsDT.Select("PackingDateTime IS NOT NULL AND MainOrderID=" + MainOrderID);
                if (rows.Count() > 0)
                    PackedCount = rows.Count();

                if (ComplementsCount > 0)
                    PackedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PackedCount) / Convert.ToDecimal(ComplementsCount));
                d2 = PackedProgressVal * 100;
                PackedPercentage = Convert.ToInt32(Math.Truncate(d2));
                
                MainOrdersDT.Rows[i]["PackedCount"] = PackedCount + " / " + ComplementsCount;
                MainOrdersDT.Rows[i]["PackedPercentage"] = PackedPercentage;
            }
        }

        public void FilterComplementsLabels(int MainOrderID)
        {
            ComplementDetailsDT.Clear();
            string SelectCommand = @"SELECT C.MainOrderID, C.PackNumber, C.Notes AS CNotes, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID,
                CabFurnitureComplementDetails.* FROM CabFurnitureComplementDetails 
                INNER JOIN CabFurnitureComplements AS C ON CabFurnitureComplementDetails.CabFurnitureComplementID=C.CabFurnitureComplementID
                WHERE MainOrderID=" + MainOrderID;
            ComplementDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementDetailsDA.Fill(ComplementDetailsDT);

            ComplementLabelsDT.Clear();
            SelectCommand = @"SELECT CabFurnitureComplementID, PackNumber, MainOrderID, TechStoreSubGroupID, PrintDateTime, PackingDateTime FROM CabFurnitureComplements WHERE MainOrderID=" + MainOrderID;
            ComplementLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            ComplementLabelsDA.Fill(ComplementLabelsDT);

            for (int i = 0; i < ComplementLabelsDT.Rows.Count; i++)
            {
                ComplementLabelsDT.Rows[i]["Index"] = i + 1;
            }
        }

        public void FilterComplementDetails(int CabFurnitureComplementID)
        {
            ComplementDetailsBS.Filter = "CabFurnitureComplementID =" + CabFurnitureComplementID;
            ComplementDetailsBS.MoveFirst();
        }

        public void ClearComplements()
        {
            ComplementLabelsDT.Clear();
            ComplementDetailsDT.Clear();
        }

        public void PrintComplements(int[] CabFurnitureComplementID)
        {
            string filter = string.Empty;
            foreach (int item in CabFurnitureComplementID)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = "SELECT * FROM CabFurnitureComplements WHERE CabFurnitureComplementID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(filter, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DateTime PrintDateTime = Security.GetCurrentDate();

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (DT.Rows[i]["PrintUserID"] == DBNull.Value)
                                    DT.Rows[i]["PrintUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["PrintDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PrintDateTime"] = PrintDateTime;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

    }

    public class PackagesManager
    {
        DataTable PackagesDT = null;
        public BindingSource PackagesBS = null;
        SqlDataAdapter PackagesDA;
        
        DataTable PackageLabelsDT = null;
        DataTable TempPackageLabelsDT = null;
        public BindingSource PackageLabelsBS = null;
        SqlDataAdapter PackageLabelsDA;

        DataTable PackageDetailsDT = null;
        public BindingSource PackageDetailsBS = null;
        SqlDataAdapter PackageDetailsDA;
        
        public PackagesManager()
        {
            PackagesDT = new DataTable();
            PackageLabelsDT = new DataTable();
            PackageDetailsDT = new DataTable();
            TempPackageLabelsDT = new DataTable();

            PackagesBS = new BindingSource();
            PackageLabelsBS = new BindingSource();
            PackageDetailsBS = new BindingSource();
            

            string SelectCommand = @"SELECT TOP 0 CabFurniturePackageID, CabFurAssignmentDetailID, PackNumber, PackagesCount, TechStoreSubGroupID, PrintDateTime, AddToStorageDateTime, RemoveFromStorageDateTime FROM CabFurniturePackages";
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(PackageLabelsDT);
            PackageLabelsDT.Columns.Add(new DataColumn("Index", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 CabFurniturePackages.*, C.Count FROM CabFurniturePackages
                INNER JOIN CabFurnitureAssignmentDetails AS C ON CabFurniturePackages.CabFurAssignmentDetailID = C.CabFurAssignmentDetailID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(TempPackageLabelsDT);
            }

            SelectCommand = @"SELECT TOP 0 C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID, 
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID";
            PackageDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageDetailsDA.Fill(PackageDetailsDT);

            SelectCommand = @"SELECT TOP 0 ClientName, CabFurAssignmentID FROM CabFurniturePackages";
            PackagesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackagesDA.Fill(PackagesDT);
            PackagesDT.Columns.Add(new DataColumn("PackagesCount", Type.GetType("System.Int32")));
            PackagesDT.Columns.Add(new DataColumn("PrintedPercentage", Type.GetType("System.Int32")));
            PackagesDT.Columns.Add(new DataColumn("PackedCount", Type.GetType("System.String")));
            PackagesDT.Columns.Add(new DataColumn("PackedPercentage", Type.GetType("System.Int32")));

            PackagesBS.DataSource = PackagesDT;
            PackageLabelsBS.DataSource = PackageLabelsDT;
            PackageDetailsBS.DataSource = PackageDetailsDT;
        }

        public void UpdatePackages(DateTime date1, DateTime date2)
        {
            string Filter = " WHERE CAST(CabFurniturePackages.CreateDateTime AS date) >= '" + date1.ToString("yyyy-MM-dd") +
                " 00:00' AND CAST(CabFurniturePackages.CreateDateTime AS date) <= '" + date2.ToString("yyyy-MM-dd") + " 23:59'";

            PackagesDT.Clear();
            string SelectCommand = @"SELECT DISTINCT ClientName, CabFurAssignmentID FROM CabFurniturePackages" + Filter + " ORDER BY CabFurAssignmentID DESC";
            PackagesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackagesDA.Fill(PackagesDT);

            TempPackageLabelsDT.Clear();
            SelectCommand = @"SELECT CabFurniturePackages.*, C.Count FROM CabFurniturePackages
                INNER JOIN CabFurnitureAssignmentDetails AS C ON CabFurniturePackages.CabFurAssignmentDetailID = C.CabFurAssignmentDetailID" + Filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(TempPackageLabelsDT);
            }

            for (int i = 0; i < PackageLabelsDT.Rows.Count; i++)
            {
                PackageLabelsDT.Rows[i]["Index"] = i + 1;
            }

            for (int i = 0; i < PackagesDT.Rows.Count; i++)
            {
                int PrintedCount = 0;
                int PrintedPercentage = 0;
                decimal PrintedProgressVal = 0;
                decimal d1 = 0;

                int PackedCount = 0;
                int PackedPercentage = 0;
                decimal PackedProgressVal = 0;
                decimal d2 = 0;

                int CabFurAssignmentID = Convert.ToInt32(PackagesDT.Rows[i]["CabFurAssignmentID"]);
                int PackagesCount = 0;

                DataRow[] rows = TempPackageLabelsDT.Select("CabFurAssignmentID=" + CabFurAssignmentID);
                if (rows.Count() > 0)
                {
                    PackagesCount = rows.Count();
                }

                rows = TempPackageLabelsDT.Select("PrintDateTime IS NOT NULL AND CabFurAssignmentID=" + CabFurAssignmentID);
                if (rows.Count() > 0)
                    PrintedCount = rows.Count();

                if (PrintedCount > 0)
                    PrintedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PrintedCount) / Convert.ToDecimal(PackagesCount));
                d1 = PrintedProgressVal * 100;
                PrintedPercentage = Convert.ToInt32(Math.Truncate(d1));

                rows = TempPackageLabelsDT.Select("AddToStorageDateTime IS NOT NULL AND CabFurAssignmentID=" + CabFurAssignmentID);
                if (rows.Count() > 0)
                    PackedCount = rows.Count();
                if (PackagesCount > 0)
                    PackedProgressVal = Convert.ToDecimal(Convert.ToDecimal(PackedCount) / Convert.ToDecimal(PackagesCount));
                d2 = PackedProgressVal * 100;
                PackedPercentage = Convert.ToInt32(Math.Truncate(d2));

                PackagesDT.Rows[i]["PrintedPercentage"] = PrintedPercentage;
                PackagesDT.Rows[i]["PackagesCount"] = PackagesCount;
                PackagesDT.Rows[i]["PackedCount"] = PackedCount + " / " + PackagesCount;
                PackagesDT.Rows[i]["PackedPercentage"] = PackedPercentage;
            }
        }

        public void FilterPackages(bool bPacked, bool bNotPacked, bool bPartPacked, bool bPrinted, bool bNotPrinted, bool bPartPrinted)
        {
            string filtr1 = string.Empty;
            string filtr2 = string.Empty;

            if (bPacked)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (PackedPercentage=100)";
                else
                    filtr1 = "(PackedPercentage=100)";
            }
            if (bNotPacked)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (PackedPercentage=0)";
                else
                    filtr1 = "(PackedPercentage=0)";
            }
            if (bPartPacked)
            {
                if (filtr1.Length > 0)
                    filtr1 += " OR (PackedPercentage>0 AND PackedPercentage<100)";
                else
                    filtr1 = "(PackedPercentage>0 AND PackedPercentage<100)";
            }

            if (bPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintedPercentage=100)";
                else
                    filtr2 = "(PrintedPercentage=100)";
            }
            if (bNotPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintedPercentage=0)";
                else
                    filtr2 = "(PrintedPercentage=0)";
            }
            if (bPartPrinted)
            {
                if (filtr2.Length > 0)
                    filtr2 += " OR (PrintedPercentage>0 AND PrintedPercentage<100)";
                else
                    filtr2 = "(PrintedPercentage>0 AND PrintedPercentage<100)";
            }

            if (filtr2.Length > 0)
                filtr2 = "(" + filtr2 + ")";
            else
                filtr2 = "(PrintedPercentage=-1)";
            if (filtr1.Length > 0)
            {
                filtr1 = "(" + filtr1 + ")";
                if (filtr2.Length > 0)
                    filtr1 += " AND (" + filtr2 + ")";
            }
            else
                filtr1 = "(PackedPercentage=-1)";

            PackagesBS.Filter = filtr1;
        }

        public void FilterPackagesLabels(int CabFurAssignmentID)
        {
            PackageDetailsDT.Clear();
            string SelectCommand = @"SELECT C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID,
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID
                WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            PackageDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageDetailsDA.Fill(PackageDetailsDT);

            PackageLabelsDT.Clear();
            SelectCommand = @"SELECT CabFurniturePackageID, CabFurAssignmentDetailID, PackNumber, PackagesCount, TechStoreSubGroupID, PrintDateTime, AddToStorageDateTime, RemoveFromStorageDateTime FROM CabFurniturePackages WHERE CabFurAssignmentID=" + CabFurAssignmentID;
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(PackageLabelsDT);

            for (int i = 0; i < PackageLabelsDT.Rows.Count; i++)
            {
                PackageLabelsDT.Rows[i]["Index"] = i + 1;
            }
        }

        public void FilterPackagesDetails(int CabFurniturePackageID)
        {
            PackageDetailsBS.Filter = "CabFurniturePackageID =" + CabFurniturePackageID;
            PackageDetailsBS.MoveFirst();
        }

        public void ClearPackges()
        {
            PackageLabelsDT.Clear();
            PackageDetailsDT.Clear();
        }

        public void PrintComplements(int[] CabFurniturePackageID)
        {
            string filter = string.Empty;
            foreach (int item in CabFurniturePackageID)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = "SELECT * FROM CabFurniturePackages WHERE CabFurniturePackageID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(filter, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DateTime PrintDateTime = Security.GetCurrentDate();

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                if (DT.Rows[i]["PrintUserID"] == DBNull.Value)
                                    DT.Rows[i]["PrintUserID"] = Security.CurrentUserID;
                                if (DT.Rows[i]["PrintDateTime"] == DBNull.Value)
                                    DT.Rows[i]["PrintDateTime"] = PrintDateTime;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

    }

    public class CheckLabel
    {
        int CurrentCabFurniturePackageID = -1;

        DataTable ScanContentDT = null;
        DataTable ScanComplementDetailsDT = null;
        DataTable ScanPackageDetailsDT = null;

        public BindingSource ScanContentBS = null;

        public DataTable StoreDT;
        SqlDataAdapter StoreDA;
        SqlCommandBuilder StoreCB;

        public struct LabelInfo
        {
            public string PackNumber;
            public string PackedToTotal;
            public string RemoveTotal;
            public string ClientName;
            public string OrderNumber;
            public string MainOrderNumber;
            public string AssignmentNumber;
            public string CreateDateTime;
            public string AddToStorageDateTime;
            public string RemoveFromStorageDateTime;
            public bool ComplementScan;
            public bool AddToStorage;
            public bool RemoveFromStorage;
            public Color TotalLabelColor;
            public Color RemoveTotalLabelColor;
        }
        
        public LabelInfo lInfo;

        public bool WaitScanComplement;
        public bool AlreadyComplementPack;

        public CheckLabel()
        {
            ScanContentDT = new DataTable();
            string SelectCommand = @"SELECT TOP 0 C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, 
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(ScanContentDT);
            }
            ScanContentBS = new BindingSource()
            {
                DataSource = ScanContentDT
            };


            StoreDT = new DataTable();

            StoreDA = new SqlDataAdapter("SELECT TOP 0 * FROM Store", ConnectionStrings.StorageConnectionString);
            StoreDA.Fill(StoreDT);
            StoreCB = new SqlCommandBuilder(StoreDA);

            ScanComplementDetailsDT = new DataTable();
            ScanPackageDetailsDT = new DataTable();
        }

        public void CancelPackScan()
        {
            WaitScanComplement = false;
            AlreadyComplementPack = false;
        }

        public void GetComplementContent(int CabFurnitureComplementID)
        {
            ScanContentDT.Clear();

            string SelectCommand = @"SELECT C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, 
                CabFurnitureComplementDetails.* FROM CabFurnitureComplementDetails 
                INNER JOIN CabFurnitureComplements AS C ON CabFurnitureComplementDetails.CabFurnitureComplementID=C.CabFurnitureComplementID
                WHERE CabFurnitureComplementDetails.CabFurnitureComplementID=" + CabFurnitureComplementID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(ScanContentDT);
            }
            ScanComplementDetails(CabFurnitureComplementID);
        }

        public void GetPackageContent(int CabFurniturePackageID)
        {
            ScanContentDT.Clear();

            string SelectCommand = @"SELECT C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, 
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID
                WHERE CabFurniturePackageDetails.CabFurniturePackageID=" + CabFurniturePackageID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(ScanContentDT);
            }
            ScanPackageDetails(CabFurniturePackageID);
        }

        public bool GetComplementInfo(int CabFurnitureComplementID)
        {
            bool PackageExist = false;
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CabFurnitureComplements.*, M.ClientName FROM CabFurnitureComplements 
                INNER JOIN infiniu2_marketingreference.dbo.Clients AS M ON CabFurnitureComplements.ClientID = M.ClientID
                WHERE CabFurnitureComplementID = " + CabFurnitureComplementID, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["PackingDateTime"] != DBNull.Value)
                            AlreadyComplementPack = true;
                        else
                            AlreadyComplementPack = false;

                        lInfo.ClientName = DT.Rows[0]["ClientName"].ToString();
                        lInfo.OrderNumber = Convert.ToInt32(DT.Rows[0]["OrderNumber"]).ToString();
                        lInfo.MainOrderNumber = Convert.ToInt32(DT.Rows[0]["MainOrderID"]).ToString();
                        lInfo.CreateDateTime = Convert.ToDateTime(DT.Rows[0]["CreateDateTime"]).ToString("dd.MM.yyyy");
                        lInfo.PackNumber = DT.Rows[0]["PackNumber"].ToString();
                        PackageExist = true;
                    }
                }
            }
            return PackageExist;
        }

        public bool GetPackageInfo(int CabFurniturePackageID)
        {
            bool PackageExist = false;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurniturePackages WHERE CabFurniturePackageID = " + CabFurniturePackageID, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        int CabFurAssignmentID = Convert.ToInt32(DT.Rows[0]["CabFurAssignmentID"]);
                        if (Convert.ToBoolean(DT.Rows[0]["AddToStorage"]))
                        {
                            CurrentCabFurniturePackageID = -1;
                            lInfo.AddToStorage = true;
                            lInfo.AddToStorageDateTime = Convert.ToDateTime(DT.Rows[0]["AddToStorageDateTime"]).ToString("dd.MM.yyyy");
                        }
                        if (Convert.ToBoolean(DT.Rows[0]["RemoveFromStorage"]))
                        {
                            lInfo.RemoveFromStorage = true;
                            lInfo.RemoveFromStorageDateTime = Convert.ToDateTime(DT.Rows[0]["RemoveFromStorageDateTime"]).ToString("dd.MM.yyyy");
                        }

                        //сканируется в первый раз
                        if (!lInfo.AddToStorage)
                        {
                            WaitScanComplement = false;
                            lInfo.AddToStorageDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                            AddToStorage(CabFurniturePackageID, DateTime.Now);
                        }

                        //сканируется во второй раз
                        if (lInfo.AddToStorage && !lInfo.RemoveFromStorage)
                        {
                            CurrentCabFurniturePackageID = CabFurniturePackageID;
                            WaitScanComplement = true;
                            lInfo.RemoveFromStorageDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                        }

                        int PackedCount = 0;
                        int TotalRemoveCount = 0;
                        int TotalPackCount = 0;

                        GetPackagesCount(CabFurAssignmentID, ref PackedCount, ref TotalRemoveCount, ref TotalPackCount);
                        
                        lInfo.PackedToTotal = PackedCount.ToString() + "/" + TotalPackCount.ToString();
                        lInfo.RemoveTotal = TotalRemoveCount.ToString() + "/" + TotalPackCount.ToString();

                        if (PackedCount == TotalPackCount)
                            lInfo.TotalLabelColor = Color.FromArgb(82, 169, 24);
                        else
                            lInfo.TotalLabelColor = Color.Black;

                        if (TotalRemoveCount == TotalPackCount)
                            lInfo.RemoveTotalLabelColor = Color.FromArgb(82, 169, 24);
                        else
                            lInfo.RemoveTotalLabelColor = Color.Black;

                        lInfo.ClientName = "СКЛАД";
                        lInfo.CreateDateTime = Convert.ToDateTime(DT.Rows[0]["CreateDateTime"]).ToString("dd.MM.yyyy");
                        lInfo.AssignmentNumber = CabFurAssignmentID.ToString();
                        lInfo.PackNumber = DT.Rows[0]["PackNumber"].ToString();
                        PackageExist = true;
                    }
                }
            }
            return PackageExist;
        }

        public void PackComplement(int CabFurnitureComplementID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT CabFurnitureComplementID, PackingUserID, PackingDateTime FROM CabFurnitureComplements WHERE CabFurnitureComplementID = " + CabFurnitureComplementID, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows[0]["PackingDateTime"] == DBNull.Value)
                                DT.Rows[0]["PackingDateTime"] = Security.GetCurrentDate();

                            if (DT.Rows[0]["PackingUserID"] == DBNull.Value)
                                DT.Rows[0]["PackingUserID"] = Security.CurrentUserID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        private void ScanComplementDetails(int CabFurnitureComplementID)
        {
            string SelectCommand = @"SELECT CD.TechStoreID, C.CoverID, C.PatinaID, CD.Length, CD.Height, CD.Width, CD.Count 
                FROM CabFurnitureComplementDetails AS CD
                INNER JOIN CabFurnitureComplements AS C ON CD.CabFurnitureComplementID=C.CabFurnitureComplementID
                WHERE CD.CabFurnitureComplementID=" + CabFurnitureComplementID + " ORDER BY TechStoreID, CoverID, PatinaID, Length, Height, Width, Count";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                ScanComplementDetailsDT.Clear();
                DA.Fill(ScanComplementDetailsDT);
            }
        }

        private void ScanPackageDetails(int CabFurniturePackageID)
        {
            string SelectCommand = @"SELECT StoreItemID AS TechStoreID, CoverID, PatinaID, Length, Height, Width, CurrentCount AS Count 
                FROM Store WHERE PurchaseInvoiceID = (SELECT PurchaseInvoiceID FROM CabFurniturePackages WHERE CabFurniturePackageID=" + CabFurniturePackageID + ") ORDER BY TechStoreID, CoverID, PatinaID, Length, Height, Width, Count";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                ScanPackageDetailsDT.Clear();
                DA.Fill(ScanPackageDetailsDT);
            }
            
        }

        private void AddToStorage(int CabFurniturePackageID, DateTime AddToStorageDateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurniturePackages WHERE CabFurniturePackageID = " + CabFurniturePackageID, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            if (Convert.ToBoolean(DT.Rows[0]["AddToStorage"]) || Convert.ToBoolean(DT.Rows[0]["RemoveFromStorage"]))
                                return;

                            int PurchaseInvoiceID = 0;
                            PurchaseInvoiceID = SavePackageDetailsToStorage(CabFurniturePackageID, AddToStorageDateTime);
                            if (PurchaseInvoiceID != 0)
                            {
                                DT.Rows[0]["PurchaseInvoiceID"] = PurchaseInvoiceID;
                                DT.Rows[0]["AddToStorage"] = 1;
                                if (DT.Rows[0]["AddToStorageDateTime"] == DBNull.Value)
                                    DT.Rows[0]["AddToStorageDateTime"] = AddToStorageDateTime;

                                if (DT.Rows[0]["AddToStorageUserID"] == DBNull.Value)
                                    DT.Rows[0]["AddToStorageUserID"] = Security.CurrentUserID;
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        public bool RemoveFromStorage()
        {
            if (CurrentCabFurniturePackageID == -1)
                return false;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM CabFurniturePackages WHERE CabFurniturePackageID = " + CurrentCabFurniturePackageID, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            if (!Convert.ToBoolean(DT.Rows[0]["AddToStorage"]) || Convert.ToBoolean(DT.Rows[0]["RemoveFromStorage"]))
                                return false;
                            int MovementInvoiceID = 0;
                            MovementInvoiceID = WriteOffPackageDetailsFromStorage(Convert.ToInt32(DT.Rows[0]["PurchaseInvoiceID"]));
                            if (MovementInvoiceID != 0)
                            {
                                DT.Rows[0]["MovementInvoiceID"] = MovementInvoiceID;
                                DT.Rows[0]["RemoveFromStorage"] = 1;
                                if (DT.Rows[0]["RemoveFromStorageDateTime"] == DBNull.Value)
                                    DT.Rows[0]["RemoveFromStorageDateTime"] = Security.GetCurrentDate();

                                if (DT.Rows[0]["RemoveFromStorageUserID"] == DBNull.Value)
                                    DT.Rows[0]["RemoveFromStorageUserID"] = Security.CurrentUserID;
                                DA.Update(DT);
                                WaitScanComplement = false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void GetPackagesCount(int CabFurAssignmentID, ref int PackedCount, ref int TotalRemoveCount, ref int TotalCount)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT CabFurniturePackageID, AddToStorageDateTime, RemoveFromStorageDateTime FROM CabFurniturePackages WHERE CabFurAssignmentID = " + CabFurAssignmentID,
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DataRow[] rows = DT.Select("AddToStorageDateTime IS NOT NULL");
                        if (rows.Count() > 0)
                            PackedCount = rows.Count();

                        rows = DT.Select("RemoveFromStorageDateTime IS NOT NULL");
                        if (rows.Count() > 0)
                            TotalRemoveCount = rows.Count();

                        TotalCount = DT.Rows.Count;
                    }
                }
            }
        }
        
        public void Clear()
        {
            ScanContentDT.Clear();

            lInfo.PackNumber = "";
            lInfo.PackedToTotal = "";
            lInfo.ClientName = "";
            lInfo.AssignmentNumber = "";
            lInfo.CreateDateTime = "";
            lInfo.AddToStorageDateTime = "";
            lInfo.RemoveFromStorageDateTime = "";

            lInfo.AddToStorage = false;
            lInfo.RemoveFromStorage = false;
            lInfo.TotalLabelColor = Color.White;
        }

        public bool ArePackagesEqual()
        {
            return AreTablesTheSame(ScanPackageDetailsDT, ScanComplementDetailsDT);
        }

        private bool AreTablesTheSame(DataTable dt1, DataTable dt2)
        {
            if (dt1.Rows.Count != dt2.Rows.Count || dt1.Columns.Count != dt2.Columns.Count)
                return false;


            //for (int i = 0; i < dt1.Rows.Count; i++)
            //{
            //    for (int c = 0; c < dt1.Columns.Count; c++)
            //    {
            //        if (!Equals(dt1.Rows[i][c], dt2.Rows[i][c]))
            //            return false;
            //    }
            //}
            return true;
        }

        private int SavePackageDetailsToStorage(int CabFurniturePackageID, DateTime AddToStorageDateTime)
        {
            int PurchaseInvoiceID = 0;
            string SelectCommand = @"SELECT CabFurniturePackageDetails.*, C.CoverID, C.PatinaID, C.AddToStorageDateTime, T.*
                FROM CabFurniturePackageDetails 
                INNER JOIN infiniu2_catalog.dbo.TechStore AS T ON CabFurniturePackageDetails.TechStoreID=T.TechStoreID
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID
                WHERE CabFurniturePackageDetails.CabFurniturePackageID=" + CabFurniturePackageID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DateTime IncomeDate = Security.GetCurrentDate();
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
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
                            decimal Count = -1;
                            decimal Price = 1;
                            int CurrencyTypeID = 1;
                            int ManufacturerID = 144;
                            int FactoryID = 2;
                            string Notes = DT.Rows[i]["Notes"].ToString();

                            if (DT.Rows[i]["Length"] != DBNull.Value)
                                Length = Convert.ToDecimal(DT.Rows[i]["Length"]);
                            if (DT.Rows[i]["Width"] != DBNull.Value)
                                Width = Convert.ToDecimal(DT.Rows[i]["Width"]);
                            if (DT.Rows[i]["Height"] != DBNull.Value)
                                Height = Convert.ToDecimal(DT.Rows[i]["Height"]);
                            if (DT.Rows[i]["Thickness"] != DBNull.Value)
                                Thickness = Convert.ToDecimal(DT.Rows[i]["Thickness"]);
                            if (DT.Rows[i]["Diameter"] != DBNull.Value)
                                Diameter = Convert.ToDecimal(DT.Rows[i]["Diameter"]);
                            if (DT.Rows[i]["Admission"] != DBNull.Value)
                                Admission = Convert.ToDecimal(DT.Rows[i]["Admission"]);
                            if (DT.Rows[i]["Capacity"] != DBNull.Value)
                                Capacity = Convert.ToDecimal(DT.Rows[i]["Capacity"]);
                            if (DT.Rows[i]["Weight"] != DBNull.Value)
                                Weight = Convert.ToDecimal(DT.Rows[i]["Weight"]);

                            if (DT.Rows[i]["TechStoreID"] != DBNull.Value)
                                StoreItemID = Convert.ToInt32(DT.Rows[i]["TechStoreID"]);
                            if (DT.Rows[i]["PatinaID"] != DBNull.Value)
                                PatinaID = Convert.ToInt32(DT.Rows[i]["PatinaID"]);
                            if (DT.Rows[i]["CoverID"] != DBNull.Value)
                                CoverID = Convert.ToInt32(DT.Rows[i]["CoverID"]);
                            if (DT.Rows[i]["ColorID"] != DBNull.Value)
                                ColorID = Convert.ToInt32(DT.Rows[i]["ColorID"]);

                            if (DT.Rows[i]["Count"] != DBNull.Value)
                                Count = Convert.ToDecimal(DT.Rows[i]["Count"]);

                            AddItemToStore(StoreItemID, Length, Width, Height, Thickness, Diameter, Admission,
                                Capacity, Weight, ColorID, PatinaID, CoverID,
                                Price, Count, CurrencyTypeID, FactoryID,
                                true, AddToStorageDateTime, ManufacturerID, Notes, IncomeDate);
                        }
                        PurchaseInvoiceID = SaveInvoice(IncomeDate, 144, string.Empty, 2, 1, string.Empty, "Упаковка корпусной мебели");
                    }
                }
            }
            return PurchaseInvoiceID;
        }

        private void AddItemToStore(int StoreItemID, decimal Length, decimal Width, decimal Height, decimal Thickness,
            decimal Diameter, decimal Admission, decimal Capacity, decimal Weight,
            int ColorID, int PatinaID, int CoverID, decimal Price, decimal Count, int CurrencyTypeID, int FactoryID,
            bool bProduced, DateTime ProducedDate, int ManufacturerID, string Notes, DateTime IncomeDate)
        {
            DataRow NewRow = StoreDT.NewRow();
            
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
            if (bProduced)
                NewRow["Produced"] = ProducedDate;
            if (Notes.Length > 0)
                NewRow["Notes"] = Notes;

            NewRow["InvoiceCount"] = Count;
            NewRow["CurrentCount"] = Count;

            decimal Cost = 1;
            decimal VAT = 1;
            decimal VATCost = 0;

            VAT = Cost * 20 / 100;
            VATCost = Cost + VAT;

            NewRow["Price"] = Price;
            NewRow["PriceEUR"] = 1;
            NewRow["CurrencyTypeID"] = CurrencyTypeID;

            NewRow["Cost"] = Cost;
            NewRow["VAT"] = VAT;
            NewRow["VATCost"] = VATCost;

            NewRow["FactoryID"] = FactoryID;
            NewRow["ManufacturerID"] = ManufacturerID;
            NewRow["CreateDateTime"] = IncomeDate;
            NewRow["CreateUserID"] = Security.CurrentUserID;

            StoreDT.Rows.Add(NewRow);
        }

        private int SaveInvoice(DateTime IncomeDate, int SellerID, string DocNumber, int FactoryID, int CurrencyTypeID, string Reason, string Notes)
        {
            int PurchaseInvoiceID = 0;
            try
            {                
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM PurchaseInvoices", ConnectionStrings.StorageConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            
                            DataRow NewRow = DT.NewRow();
                            NewRow["IncomeDate"] = IncomeDate;
                            NewRow["SellerID"] = SellerID;
                            NewRow["DocNumber"] = DocNumber;
                            NewRow["Reason"] = Reason;
                            NewRow["FactoryID"] = FactoryID;
                            NewRow["CurrencyTypeID"] = CurrencyTypeID;
                            NewRow["Rate"] = 1;
                            NewRow["Notes"] = Notes;
                            DT.Rows.Add(NewRow);

                            DA.Update(DT);
                        }
                    }
                }

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM PurchaseInvoices ORDER BY PurchaseInvoiceID DESC",
                   ConnectionStrings.StorageConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            PurchaseInvoiceID = Convert.ToInt32(DT.Rows[0]["PurchaseInvoiceID"]);
                        }
                    }
                }

                foreach (DataRow Row in StoreDT.Rows)
                    Row["PurchaseInvoiceID"] = PurchaseInvoiceID;

                StoreDA.Update(StoreDT);
                StoreDT.Clear();

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message + " \r\nSaveInvoice НЕТ СОЕДИНЕНИЯ С БАЗОЙ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nSaveInvoice КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ!");
            }
            return PurchaseInvoiceID;
        }

        private void SaveMovementInvoiceDetails(int StoreIDFrom, int StoreIDTo, decimal Count, 
            DateTime CreateDateTime, int MovementInvoiceID)
        {
            string SelectCommand = @"SELECT TOP 0 * FROM MovementInvoiceDetails";
            try
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);

                            DataRow NewRow = DT.NewRow();
                            NewRow["CreateUserID"] = Security.CurrentUserID;
                            NewRow["CreateDateTime"] = CreateDateTime;
                            NewRow["MovementInvoiceID"] = MovementInvoiceID;
                            NewRow["StoreIDFrom"] = StoreIDFrom;
                            NewRow["StoreIDTo"] = StoreIDTo;
                            NewRow["Count"] = Count;
                            DT.Rows.Add(NewRow);

                            DA.Update(DT);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message + " \r\nAddMovementInvoiceDetail НЕТ СОЕДИНЕНИЯ С БАЗОЙ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nAddMovementInvoiceDetail КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ");
            }
        }

        private int SaveMovementInvoices(DateTime DateTime,
            int SellerStoreAllocID,
            int RecipientStoreAllocID, int RecipientSectorID,
            int PersonID, string PersonName, int StoreKeeperID,
            int ClientID, int SellerID,
            string ClientName, string Notes, DateTime CreateDateTime)
        {
            int LastMovementInvoiceID = 0;
                        
            string SelectCommand = @"SELECT TOP 1 * FROM MovementInvoices ORDER BY MovementInvoiceID DESC";
            try
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            DataRow NewRow = DT.NewRow();

                            NewRow["DateTime"] = DateTime;
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
                            NewRow["CreateUserID"] = Security.CurrentUserID;
                            NewRow["CreateDateTime"] = CreateDateTime;

                            DT.Rows.Add(NewRow);

                            DA.Update(DT);
                            DT.Clear();
                            DA.Fill(DT);
                            if (DT.Rows.Count > 0)
                                LastMovementInvoiceID = Convert.ToInt32(DT.Rows[0]["MovementInvoiceID"]);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message + " \r\nSaveMovementInvoices НЕТ СОЕДИНЕНИЯ С БАЗОЙ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nSaveMovementInvoices КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ");
            }

            return LastMovementInvoiceID;
        }

        private void AddItemToWriteOffStore(DataRow row, DateTime CreateDateTime, int MovementInvoiceID)
        {
            string SelectCommand = @"SELECT TOP 1 * FROM WriteOffStore ORDER BY WriteOffStoreID DESC";
            try
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {

                            DA.Fill(DT);

                            DataRow NewRow = DT.NewRow();

                            NewRow["MovementInvoiceID"] = MovementInvoiceID;
                            NewRow["CreateUserID"] = Security.CurrentUserID;
                            NewRow["CreateDateTime"] = CreateDateTime;
                            NewRow["StoreItemID"] = row["StoreItemID"];
                            NewRow["Length"] = row["Length"];
                            NewRow["Width"] = row["Width"];
                            NewRow["Height"] = row["Height"];
                            NewRow["Thickness"] = row["Thickness"];
                            NewRow["Diameter"] = row["Diameter"];
                            NewRow["Admission"] = row["Admission"];
                            NewRow["Capacity"] = row["Capacity"];
                            NewRow["Weight"] = row["Weight"];
                            NewRow["ColorID"] = row["ColorID"];
                            NewRow["CoverID"] = row["CoverID"];
                            NewRow["PatinaID"] = row["PatinaID"];
                            NewRow["InvoiceCount"] = row["CurrentCount"];
                            NewRow["CurrentCount"] = row["CurrentCount"];
                            NewRow["FactoryID"] = 2;
                            NewRow["Notes"] = row["Notes"];
                            NewRow["DecorAssignmentID"] = row["DecorAssignmentID"];

                            decimal Price = 1;
                            decimal Cost = 1;
                            decimal VAT = 1;
                            decimal VATCost = 1;

                            Cost = Convert.ToDecimal(row["CurrentCount"]) * Price;
                            VAT = Cost * 20 / 100;
                            VATCost = Cost + VAT;

                            NewRow["Price"] = Price;
                            NewRow["CurrencyTypeID"] = 1;

                            NewRow["Cost"] = Cost;
                            NewRow["VAT"] = VAT;
                            NewRow["VATCost"] = VATCost;

                            DT.Rows.Add(NewRow);
                            DA.Update(DT);

                            DT.Clear();
                            DA.Fill(DT);
                            int WriteOffStoreID = 0;
                            if (DT.Rows.Count > 0)
                                WriteOffStoreID = Convert.ToInt32(DT.Rows[0]["WriteOffStoreID"]);
                            if (WriteOffStoreID != 0)
                            {
                                SaveMovementInvoiceDetails(Convert.ToInt32(row["StoreID"]), WriteOffStoreID, 
                                    Convert.ToDecimal(row["CurrentCount"]), CreateDateTime, MovementInvoiceID);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message + " \r\nMoveFromStoreToWriteOffStore НЕТ СОЕДИНЕНИЯ С БАЗОЙ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " \r\nMoveFromStoreToWriteOffStore КАКАЯ-ТО НЕВЕДОМАЯ ЁБАНАЯ ХУЙНЯ");
            }
        }

        public int WriteOffPackageDetailsFromStorage(int PurchaseInvoiceID)
        {
            string SelectCommand = @"SELECT * FROM Store WHERE PurchaseInvoiceID = " + PurchaseInvoiceID;
            DateTime CreateDateTime = Security.GetCurrentDate();
            int MovementInvoiceID = SaveMovementInvoices(CreateDateTime, 2, 13, 0, 0, string.Empty, Security.CurrentUserID, 
                144, 144, "ЗОВ-ТПС", "Списание корпусной мебели со склада", CreateDateTime);
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                AddItemToWriteOffStore(DT.Rows[i], CreateDateTime, MovementInvoiceID);
                                DT.Rows[i]["CurrentCount"] = 0;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
            return MovementInvoiceID;
        }

    }

    public class CalculateMaterial
    {
        int TechStoreID, CoverID, PatinaID, Length, Height, Width = 0;
        int ItemsCount = 0;
        string TechStoreName, CoverName, PatinaName = string.Empty;

        DataTable OperationsGroups;

        DataTable CubFurCoversDT;
        DataTable FrontsOrdersDT;
        DataTable FrontsConfigDT;
        DataTable OperationsDetailDT;
        DataTable StoreDetailDT;
        DataTable SumOperationsDetailDT;
        DataTable SumStoreDetailDT;
        DataTable OperationsTermsDT;
        DataTable StoreDetailTermsDT;

        DataTable Algorithm01DT;
        DataTable Algorithm02_1DT;
        DataTable Algorithm02_2DT;
        DataTable Algorithm03_1DT;
        DataTable Algorithm03_2DT;
        DataTable Algorithm04DT;
        DataTable Algorithm05DT;
        DataTable Algorithm06DT;
        DataTable Algorithm07DT;
        DataTable Algorithm08DT;
        DataTable Algorithm09DT;
        DataTable Algorithm10DT;
        DataTable Algorithm11DT;
        DataTable Algorithm12DT;
        DataTable Algorithm13_1DT;
        DataTable Algorithm13_2DT;
        DataTable Algorithm14_1DT;
        DataTable Algorithm14_2DT;
        DataTable Algorithm15DT;
        DataTable Algorithm16DT;
        DataTable Algorithm17DT;
        DataTable Algorithm18DT;

        DataTable Algorithm19DT;

        DataTable Algorithm20DT;

        DataTable Algorithm21DT;
        DataTable Algorithm22DT;
        DataTable Algorithm23DT;
        DataTable Algorithm24DT;

        DataTable Algorithm25DT;

        DataTable Algorithm26DT;

        DataTable Algorithm27DT;
        DataTable Algorithm28DT;
        DataTable Algorithm29DT;
        DataTable Algorithm30DT;

        DataTable Algorithm31DT;

        DataTable Algorithm32DT;

        DataTable Algorithm33DT;

        DataTable Algorithm34DT;

        DataTable Algorithm35DT;

        AssignmentsManager AssignmentsManager;

        public CalculateMaterial(AssignmentsManager tAssignmentsManager)
        {
            AssignmentsManager = tAssignmentsManager;
        }

        public void Initialize()
        {
            Create();

            string SelectCommand = @"SELECT TechCatalogOperationsDetail.*, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupName, 
                MachinesOperations.MachinesOperationName, MachinesOperations.Norm, MachinesOperations.PreparatoryNorm, MachinesOperations.MeasureID, MachinesOperations.CabFurDocTypeID, MachinesOperations.CabFurAlgorithmID, CabFurnitureDocumentTypes.DocName, 
                Machines.MachineID, Machines.MachineName, SubSectors.SubSectorID, SubSectors.SubSectorName, Sectors.SectorID, Sectors.SectorName, Measure FROM TechCatalogOperationsDetail
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID AND GroupNumber=1
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                INNER JOIN infiniu2_storage.dbo.CabFurnitureDocumentTypes AS CabFurnitureDocumentTypes ON MachinesOperations.CabFurDocTypeID = CabFurnitureDocumentTypes.CabFurDocTypeID
                INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
                INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID
                INNER JOIN Measures ON MachinesOperations.MeasureID = Measures.MeasureID ORDER BY SerialNumber";

            //SelectCommand = @"SELECT Sectors.SectorName, SubSectors.SubSectorName, Machines.MachineName, TechCatalogOperationsGroups.GroupName, TechCatalogOperationsGroups.TechStoreID, MachinesOperations.MachinesOperationName, TechCatalogOperationsDetail.*, 
            //    MachinesOperations.CabFurDocTypeID, MachinesOperations.CabFurAlgorithmID, CabFurnitureDocumentTypes.DocName, CabFurnitureDocumentTypes.AssignmentID, CabFurnitureAlgorithms.CabFurAlgorithmID FROM TechCatalogOperationsDetail
            //    INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
            //    INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
            //    INNER JOIN infiniu2_storage.dbo.CabFurnitureDocumentTypes AS CabFurnitureDocumentTypes ON MachinesOperations.CabFurDocTypeID = CabFurnitureDocumentTypes.CabFurDocTypeID
            //    INNER JOIN infiniu2_storage.dbo.CabFurnitureAlgorithms AS CabFurnitureAlgorithms ON MachinesOperations.CabFurAlgorithmID = CabFurnitureAlgorithms.CabFurAlgorithmID 
            //    INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
            //    INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
            //    INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID WHERE MachinesOperations.CabFurAlgorithmID<>-1";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsDetailDT.Clear();
                DA.Fill(OperationsDetailDT);
                SumOperationsDetailDT = OperationsDetailDT.Clone();
                SumOperationsDetailDT.Columns.Add(new DataColumn("NestedLevel", Type.GetType("System.Int32")));
                SumOperationsDetailDT.Columns.Add(new DataColumn("PrevOperationsDetailID", Type.GetType("System.Int32")));
                SumOperationsDetailDT.Columns.Add(new DataColumn("PrevStoreDetailID", Type.GetType("System.Int32")));
                SumOperationsDetailDT.Columns.Add(new DataColumn("OperationsDetail", Type.GetType("System.String")));
                SumOperationsDetailDT.Columns.Add(new DataColumn("Done", Type.GetType("System.Boolean")));
            }
            SelectCommand = @"SELECT Sectors.SectorName, SubSectors.SubSectorName, Machines.MachineID, Machines.MachineName, TechCatalogOperationsGroups.GroupName, TechCatalogOperationsGroups.TechCatalogOperationsGroupID, TechStore1.TechStoreName AS TechStoreName1, TechStore1.Notes AS Notes1, MachinesOperations.MachinesOperationName, TechCatalogStoreDetail.*, 
                TechStore.TechStoreName, TechStore.Notes, TechStore.IsHalfStuff, TechStore.CoverID, TechStore.PatinaID, Measure, 
                MachinesOperations.CabFurDocTypeID, MachinesOperations.CabFurAlgorithmID, CabFurnitureDocumentTypes.DocName, CabFurnitureDocumentTypes.AssignmentID, CabFurnitureAlgorithms.CabFurAlgorithmID, TechStoreSubGroups.TechStoreGroupID, TechStoreSubGroups.TechStoreSubGroupID, TechStoreSubGroups1.TechStoreGroupID, TechStoreSubGroups1.TechStoreSubGroupID FROM TechCatalogStoreDetail
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID = TechCatalogOperationsDetail.TechCatalogOperationsDetailID
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                INNER JOIN infiniu2_storage.dbo.CabFurnitureDocumentTypes AS CabFurnitureDocumentTypes ON MachinesOperations.CabFurDocTypeID = CabFurnitureDocumentTypes.CabFurDocTypeID
                INNER JOIN infiniu2_storage.dbo.CabFurnitureAlgorithms AS CabFurnitureAlgorithms ON MachinesOperations.CabFurAlgorithmID = CabFurnitureAlgorithms.CabFurAlgorithmID
                INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
                INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID
                INNER JOIN TechStoreSubGroups ON TechStore.TechStoreSubGroupID = TechStoreSubGroups.TechStoreSubGroupID
                INNER JOIN TechStore AS TechStore1 ON TechCatalogOperationsGroups.TechStoreID = TechStore1.TechStoreID
                INNER JOIN TechStoreSubGroups AS TechStoreSubGroups1 ON TechStore1.TechStoreSubGroupID = TechStoreSubGroups1.TechStoreSubGroupID
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID ORDER BY GroupA, GroupB, GroupC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreDetailDT.Clear();
                DA.Fill(StoreDetailDT);
                SumStoreDetailDT = StoreDetailDT.Clone();
                SumStoreDetailDT.Columns.Add(new DataColumn("NestedLevel", Type.GetType("System.Int32")));
                SumStoreDetailDT.Columns.Add(new DataColumn("Consumable", Type.GetType("System.Boolean")));
                SumStoreDetailDT.Columns.Add(new DataColumn("PrevStoreDetailID", Type.GetType("System.Int32")));
            }
            SelectCommand = @"SELECT * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsTermsDT.Clear();
                DA.Fill(OperationsTermsDT);
            }

            SelectCommand = @"SELECT * FROM TechCatalogStoreDetailTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                StoreDetailTermsDT.Clear();
                DA.Fill(StoreDetailTermsDT);
            }

            SelectCommand = @"SELECT * FROM TechCatalogOperationsGroups";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsGroups.Clear();
                DA.Fill(OperationsGroups);
            }

            SelectCommand = @"SELECT * FROM CabFurnitureCovers";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                CubFurCoversDT.Clear();
                DA.Fill(CubFurCoversDT);
            }
        }

        private void Create()
        {
            #region Объявление таблиц

            Algorithm01DT = new DataTable();
            Algorithm01DT.Columns.Add(new DataColumn("OperationName", Type.GetType("System.String")));
            Algorithm01DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm01DT.Columns.Add(new DataColumn("Patina", Type.GetType("System.String")));
            Algorithm01DT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            Algorithm01DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm01DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm01DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm01DT.Columns.Add(new DataColumn("PackCount", Type.GetType("System.Decimal")));
            Algorithm01DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm02_1DT = new DataTable();
            Algorithm02_1DT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            Algorithm02_1DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm02_1DT.Columns.Add(new DataColumn("Patina", Type.GetType("System.String")));
            Algorithm02_1DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm02_1DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm02_1DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm02_1DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));

            Algorithm02_2DT = new DataTable();
            Algorithm02_2DT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Patina", Type.GetType("System.String")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm02_2DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm03_1DT = new DataTable();
            Algorithm03_1DT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            Algorithm03_1DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm03_1DT.Columns.Add(new DataColumn("Patina", Type.GetType("System.String")));
            Algorithm03_1DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm03_1DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm03_1DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm03_1DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));

            Algorithm03_2DT = new DataTable();
            Algorithm03_2DT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Patina", Type.GetType("System.String")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm03_2DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm08DT = new DataTable();
            Algorithm08DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm08DT.Columns.Add(new DataColumn("NotesM", Type.GetType("System.String")));
            Algorithm08DT.Columns.Add(new DataColumn("NameM", Type.GetType("System.String")));
            Algorithm08DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm08DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm08DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm08DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));
            Algorithm08DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            
            Algorithm04DT = new DataTable();
            Algorithm04DT.Columns.Add(new DataColumn("NameMCover", Type.GetType("System.String")));
            Algorithm04DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm04DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm04DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm04DT.Columns.Add(new DataColumn("Notes1", Type.GetType("System.String")));
            Algorithm04DT.Columns.Add(new DataColumn("Name1", Type.GetType("System.String")));
            Algorithm04DT.Columns.Add(new DataColumn("NameMCover1", Type.GetType("System.String")));
            Algorithm04DT.Columns.Add(new DataColumn("Length1", Type.GetType("System.Int32")));
            Algorithm04DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            
            Algorithm05DT = new DataTable();
            Algorithm05DT.Columns.Add(new DataColumn("NameM", Type.GetType("System.String")));
            Algorithm05DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm05DT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            Algorithm05DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm05DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm05DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm05DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));
            Algorithm05DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            
            Algorithm16DT = new DataTable();
            Algorithm16DT.Columns.Add(new DataColumn("NameM", Type.GetType("System.String")));
            Algorithm16DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm16DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm16DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm16DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));
            Algorithm16DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));

            Algorithm10DT = new DataTable();
            Algorithm10DT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));
            Algorithm10DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm10DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm10DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm10DT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            Algorithm10DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm19DT = new DataTable();
            Algorithm19DT.Columns.Add(new DataColumn("NameM", Type.GetType("System.String")));
            Algorithm19DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm19DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm19DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm19DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm21DT = new DataTable();
            Algorithm21DT.Columns.Add(new DataColumn("NotesM", Type.GetType("System.String")));
            Algorithm21DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm21DT.Columns.Add(new DataColumn("Length", Type.GetType("System.Int32")));
            Algorithm21DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm21DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm21DT.Columns.Add(new DataColumn("NameM", Type.GetType("System.String")));
            Algorithm21DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm25DT = new DataTable();
            Algorithm25DT.Columns.Add(new DataColumn("NotesMNameM", Type.GetType("System.String")));
            Algorithm25DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm25DT.Columns.Add(new DataColumn("Filling", Type.GetType("System.String")));
            Algorithm25DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm25DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm25DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm25DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            Algorithm25DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm26DT = new DataTable();
            Algorithm26DT.Columns.Add(new DataColumn("NotesMNameM", Type.GetType("System.String")));
            Algorithm26DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm26DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm26DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm26DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm26DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            Algorithm26DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm27DT = new DataTable();
            Algorithm27DT.Columns.Add(new DataColumn("NotesMNameM", Type.GetType("System.String")));
            Algorithm27DT.Columns.Add(new DataColumn("Cover", Type.GetType("System.String")));
            Algorithm27DT.Columns.Add(new DataColumn("Patina", Type.GetType("System.String")));
            Algorithm27DT.Columns.Add(new DataColumn("Height", Type.GetType("System.Int32")));
            Algorithm27DT.Columns.Add(new DataColumn("Width", Type.GetType("System.Int32")));
            Algorithm27DT.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            Algorithm27DT.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            Algorithm27DT.Columns.Add(new DataColumn("Worker", Type.GetType("System.String")));

            Algorithm12DT = Algorithm19DT.Clone();

            Algorithm32DT = Algorithm25DT.Clone();

            Algorithm06DT = Algorithm05DT.Clone();
            Algorithm07DT = Algorithm05DT.Clone();
            Algorithm09DT = Algorithm05DT.Clone();
            Algorithm11DT = Algorithm05DT.Clone();

            Algorithm15DT = Algorithm10DT.Clone();
            Algorithm20DT = Algorithm10DT.Clone();
            Algorithm34DT = Algorithm10DT.Clone();

            Algorithm17DT = Algorithm16DT.Clone();
            Algorithm18DT = Algorithm16DT.Clone();

            Algorithm13_1DT = Algorithm02_1DT.Clone();
            Algorithm13_2DT = Algorithm02_2DT.Clone();
            Algorithm14_1DT = Algorithm02_1DT.Clone();
            Algorithm14_2DT = Algorithm02_2DT.Clone();

            Algorithm22DT = Algorithm21DT.Clone();
            Algorithm23DT = Algorithm21DT.Clone();
            Algorithm24DT = Algorithm21DT.Clone();

            Algorithm28DT = Algorithm27DT.Clone();
            Algorithm29DT = Algorithm27DT.Clone();
            Algorithm30DT = Algorithm27DT.Clone();
            Algorithm31DT = Algorithm27DT.Clone();
            Algorithm35DT = Algorithm27DT.Clone();

            Algorithm33DT = Algorithm26DT.Clone();

            #endregion

            OperationsGroups = new DataTable();

            CubFurCoversDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            FrontsConfigDT = new DataTable();
            OperationsDetailDT = new DataTable();
            StoreDetailDT = new DataTable();
            OperationsTermsDT = new DataTable();
            StoreDetailTermsDT = new DataTable();
        }

        //FORM 06

        private void Algorithm04(DataRow[] rows)
        {
            DataTable dt = Algorithm04DT.Clone();
            Algorithm04DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "PrevStoreDetailID" });
            }
            foreach (DataRow row in dt2.Rows)
            {
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);

                //«Эл-ты корпус. мебели» (рис.1 раздел1), подгруппы «Заготовки ЛДСтП с кромкой»
                DataRow[] pRows = dt1.Select("IsHalfStuff2=0 AND PrevStoreDetailID=" + PrevStoreDetailID);
                if (pRows.Count() > 0)
                {
                    DataTable dt3 = SumStoreDetailDT.Clone();
                    foreach (DataRow pRow in pRows)
                    {
                        dt3.ImportRow(pRow);
                    }
                    DataRow[] cRows = dt1.Select("IsHalfStuff=1 AND IsHalfStuff2=1 AND PrevStoreDetailID=" + PrevStoreDetailID);
                    DataTable dt4;

                    if (cRows.Count() > 0)
                    {
                        dt4 = cRows.CopyToDataTable();
                        for (int j = 0; j < cRows.Count(); j++)
                        {
                            string NameM = pRows[0]["TechStoreName"].ToString();
                            string NotesM = pRows[0]["Notes"].ToString();
                            string Name1 = pRows[0]["TechStoreName1"].ToString();
                            string Notes1 = pRows[0]["Notes1"].ToString();
                            int TechCatalogStoreDetailID = Convert.ToInt32(pRows[0]["TechCatalogStoreDetailID"]);
                            int TechStoreID = Convert.ToInt32(pRows[0]["TechStoreID"]);
                            int CoverID2 = 0;
                            int PatinaID2 = 0;
                            int CoverID1 = 0;
                            int PatinaID1 = 0;
                            GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                            
                            DataRow NewRow = dt.NewRow();
                            NewRow["NameMCover"] = NameM + " " + AssignmentsManager.GetCoverName(CoverID2);
                            NewRow["Notes1"] = Notes1;
                            NewRow["Name1"] = Name1;
                            NewRow["Length"] = pRows[0]["Length"];
                            NewRow["Width"] = pRows[0]["Width"];

                            //int i = 0;
                            //decimal count = 0;
                            //int PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                            //while (i != 3)
                            //{
                            //    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                            //    if (prevRows.Count() > 0)
                            //    {
                            //        if (prevRows[0]["Count"] != DBNull.Value)
                            //            count = Convert.ToDecimal(prevRows[0]["Count"]);
                            //        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                            //    }
                            //    i++;
                            //}

                            bool b = true;
                            decimal count = 1;
                            int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                            while (b)
                            {
                                DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                                if (prevRows.Count() > 0)
                                {
                                    if (prevRows[0]["Count"] != DBNull.Value)
                                    {
                                        decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                                        if (c % 1 == 0)
                                            count *= c;
                                    }
                                    PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                                }
                                else
                                    b = false;
                            }

                            if (cRows[j]["Length"] != DBNull.Value)
                                NewRow["Square"] = Convert.ToDecimal(cRows[j]["Length"]) * ItemsCount / 1000;
                            if (j > 0)
                                count = 0;
                            NewRow["Count"] = count * ItemsCount;
                            int TechStoreID1 = Convert.ToInt32(cRows[j]["TechStoreID"]);
                            GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID1, ref CoverID1, ref PatinaID1);
                            NewRow["NameMCover1"] = cRows[j]["TechStoreName"].ToString() + " " + AssignmentsManager.GetCoverName(CoverID1);

                            if (cRows[j]["Length"] != DBNull.Value)
                                NewRow["Length1"] = cRows[j]["Length"];
                            else
                                NewRow["Length1"] = -1;

                            dt.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                    }
                }
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "NameMCover, Length, Width, Count DESC, Notes1, Name1, Length1";
                dt.Clear();
                dt = DV.ToTable();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameMCover = row["NameMCover"].ToString();
                    string NameMCover1 = row["NameMCover1"].ToString();
                    string Notes1 = row["Notes1"].ToString();
                    string Name1 = row["Name1"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Length1 = Convert.ToInt32(row["Length1"]);
                    int Width = Convert.ToInt32(row["Width"]);
                    int Amount = Convert.ToInt32(row["Count"]);

                    DataRow[] drows = dt.Select("NameMCover='" + NameMCover + "' AND NameMCover1='" + NameMCover1 + "' AND Notes1='" + Notes1 + "' AND Name1='" + Name1 + "' AND Length=" + Length + " AND Length1=" + Length1 + " AND Width=" + Width + " AND Count=" + Amount);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }

                    DataRow[] frows = Algorithm04DT.Select("NameMCover='" + NameMCover + "' AND NameMCover1='" + NameMCover1 + "' AND Notes1='" + Notes1 + "' AND Name1='" + Name1 + "' AND Length=" + Length + " AND Length1=" + Length1 + " AND Width=" + Width + " AND Count=" + Amount);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm04DT.NewRow();
                        NewRow["NameMCover"] = NameMCover;
                        NewRow["NameMCover1"] = NameMCover1;
                        NewRow["Notes1"] = Notes1;
                        NewRow["Name1"] = Name1;
                        NewRow["Length"] = row["Length"];
                        NewRow["Length1"] = row["Length1"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        Algorithm04DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }

            dt.Dispose();
            dt1.Dispose();
            dt2.Dispose();
        }

        //FORM 01

        private void Algorithm02(DataRow[] rows)
        {
            DataTable dt = Algorithm02_2DT.Clone();
            Algorithm02_2DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                //группы складов «Эл-ты корпус. мебели» рис.1 раздел1), подгруппы «Эл-ты к.м. готовые» 
                if (Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 348
                    || Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 268)
                    continue;
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "PrevStoreDetailID" });
            }
            foreach (DataRow row in dt2.Rows)
            {
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);

                DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                if (prevRows.Count() > 0)
                {
                    int TechCatalogStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);
                    if (!CheckStoreDetailConditions(TechCatalogStoreDetailID, CoverID, PatinaID))
                        continue;
                    DataRow NewRow = dt.NewRow();

                    string NameM = prevRows[0]["TechStoreName"].ToString();
                    string NotesM = prevRows[0]["Notes"].ToString();
                    int TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                    int CoverID2 = 0;
                    int PatinaID2 = 0;
                    GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                    if (prevRows[0]["Length"] != DBNull.Value)
                        NewRow["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                        NewRow["Length"] = -1;

                    if (prevRows[0]["Height"] != DBNull.Value)
                        NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    else
                        NewRow["Height"] = -1;

                    if (prevRows[0]["Width"] != DBNull.Value)
                        NewRow["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                    else
                        NewRow["Width"] = -1;

                    NewRow["Name"] = NameM + " " + NotesM;
                    if (prevRows[0]["CoverID"] != DBNull.Value)
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                    else
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                    if (prevRows[0]["PatinaID"] != DBNull.Value)
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                    else
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                    //вычисление кол-ва элементов
                    bool b = true;
                    decimal count = 1;
                    int x = 1;
                    int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                    while (b)
                    {
                        DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                        if (pRows.Count() > 0)
                        {
                            if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                            {
                                decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                                if (c % 1 == 0)
                                    count *= c;
                            }
                            x++;
                            PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                        }
                        else
                            b = false;
                    }
                    NewRow["Count"] = count * ItemsCount;
                    dt.Rows.Add(NewRow);
                }
            }            
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }

                    DataRow[] frows = Algorithm02_2DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm02_2DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm02_2DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Clear();
            Algorithm02_1DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Эл-ты корпус. мебели», подгруппы «Эл-ты к.м. готовые»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 348
                    || Convert.ToInt32(row["TechStoreSubGroupID1"]) != 268)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["Name"] = NameM + " " + NotesM;
                if (row["CoverID"] != DBNull.Value)
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                else
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                if (row["PatinaID"] != DBNull.Value)
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                else
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                //вычисление кол-ва элементов
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (pRows.Count() > 0)
                    {
                        if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }

                    DataRow[] frows = Algorithm02_1DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm02_1DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm02_1DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Dispose();
            dt1.Dispose();
            dt2.Dispose();
        }

        private void Algorithm03(DataRow[] rows)
        {
            DataTable dt = Algorithm03_2DT.Clone();
            Algorithm03_2DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                //группы складов «Эл-ты корпус. мебели» рис.1 раздел1), подгруппы «Эл-ты к.м. собранные» 
                int TechStoreSubGroupID1 = Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]);
                if (Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 345
                    && Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 266)
                    continue;
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "PrevStoreDetailID" });
            }
            foreach (DataRow row in dt2.Rows)
            {
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);

                DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                if (prevRows.Count() > 0)
                {
                    int TechCatalogStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);
                    if (!CheckStoreDetailConditions(TechCatalogStoreDetailID, CoverID, PatinaID))
                        continue;
                    DataRow NewRow = dt.NewRow();

                    string NameM = prevRows[0]["TechStoreName"].ToString();
                    string NotesM = prevRows[0]["Notes"].ToString();
                    int TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                    int CoverID2 = 0;
                    int PatinaID2 = 0;
                    GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                    if (prevRows[0]["Length"] != DBNull.Value)
                        NewRow["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                        NewRow["Length"] = -1;

                    if (prevRows[0]["Height"] != DBNull.Value)
                        NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    else
                        NewRow["Height"] = -1;

                    if (prevRows[0]["Width"] != DBNull.Value)
                        NewRow["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                    else
                        NewRow["Width"] = -1;

                    NewRow["Name"] = NameM + " " + NotesM;
                    if (prevRows[0]["CoverID"] != DBNull.Value)
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                    else
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                    if (prevRows[0]["PatinaID"] != DBNull.Value)
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                    else
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                    //вычисление кол-ва элементов
                    bool b = true;
                    decimal count = 1;
                    int x = 1;
                    int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                    while (b)
                    {
                        DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                        if (pRows.Count() > 0)
                        {
                            if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                            {
                                decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                                if (c % 1 == 0)
                                    count *= c;
                            }
                            x++;
                            PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                        }
                        else
                            b = false;
                    }
                    NewRow["Count"] = count * ItemsCount;
                    dt.Rows.Add(NewRow);
                }
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }

                    DataRow[] frows = Algorithm03_2DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm03_2DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm03_2DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Clear();
            Algorithm03_1DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Эл-ты корпус. мебели», подгруппы «Эл-ты к.м. собранные»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 345
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 266)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["Name"] = NameM + " " + NotesM;
                if (row["CoverID"] != DBNull.Value)
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                else
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                if (row["PatinaID"] != DBNull.Value)
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                else
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                //вычисление кол-ва элементов
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (pRows.Count() > 0)
                    {
                        if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }

                    DataRow[] frows = Algorithm03_1DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm03_1DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm03_1DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Dispose();
            dt1.Dispose();
            dt2.Dispose();
        }

        private void Algorithm13(DataRow[] rows)
        {
            DataTable dt = Algorithm13_2DT.Clone();
            Algorithm13_2DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                //группы складов «Декорэлементы» (рис.1 раздел1), подгруппы «Декорэл-ты с декором» 
                if (Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 77)
                    continue;
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "PrevStoreDetailID" });
            }
            foreach (DataRow row in dt2.Rows)
            {
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);

                DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                if (prevRows.Count() > 0)
                {
                    int TechCatalogStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);
                    if (!CheckStoreDetailConditions(TechCatalogStoreDetailID, CoverID, PatinaID))
                        continue;
                    DataRow NewRow = dt.NewRow();

                    string NameM = prevRows[0]["TechStoreName"].ToString();
                    string NotesM = prevRows[0]["Notes"].ToString();
                    int TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                    int CoverID2 = 0;
                    int PatinaID2 = 0;
                    GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                    if (prevRows[0]["Length"] != DBNull.Value)
                        NewRow["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                        NewRow["Length"] = -1;

                    if (prevRows[0]["Height"] != DBNull.Value)
                        NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    else
                        NewRow["Height"] = -1;

                    if (prevRows[0]["Width"] != DBNull.Value)
                        NewRow["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                    else
                        NewRow["Width"] = -1;

                    NewRow["Name"] = NameM + " " + NotesM;
                    if (prevRows[0]["CoverID"] != DBNull.Value)
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                    else
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                    if (prevRows[0]["PatinaID"] != DBNull.Value)
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                    else
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                    //вычисление кол-ва элементов
                    bool b = true;
                    decimal count = 1;
                    int x = 1;
                    int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                    while (b)
                    {
                        DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                        if (pRows.Count() > 0)
                        {
                            if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                            {
                                decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                                if (c % 1 == 0)
                                    count *= c;
                            }
                            x++;
                            PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                        }
                        else
                            b = false;
                    }
                    NewRow["Count"] = count * ItemsCount;
                    dt.Rows.Add(NewRow);
                }
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }

                    DataRow[] frows = Algorithm13_2DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm13_2DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm13_2DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Clear();
            Algorithm13_1DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Декорэлементы» (рис.1 раздел1), подгруппы «Декорэл-ты с декором» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 77)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["Name"] = NameM + " " + NotesM;
                if (row["CoverID"] != DBNull.Value)
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                else
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                if (row["PatinaID"] != DBNull.Value)
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                else
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                //вычисление кол-ва элементов
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (pRows.Count() > 0)
                    {
                        if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm13_1DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm13_1DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm13_1DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Dispose();
            dt1.Dispose();
            dt2.Dispose();
        }

        private void f(DataRow[] drows)
        {
            //if (drows.Count() == 0)
            //    MessageBox.Show("пусто");
        }

        private void Algorithm14(DataRow[] rows)
        {
            DataTable dt = Algorithm14_2DT.Clone();
            Algorithm14_2DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                //группы складов «Декорэлементы» (рис.1 раздел1), подгруппы «Декорэл-ты собранные» 
                if (Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 78)
                    continue;
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "PrevStoreDetailID" });
            }
            foreach (DataRow row in dt2.Rows)
            {
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);

                DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                if (prevRows.Count() > 0)
                {
                    int TechCatalogStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);
                    if (!CheckStoreDetailConditions(TechCatalogStoreDetailID, CoverID, PatinaID))
                        continue;
                    DataRow NewRow = dt.NewRow();

                    string NameM = prevRows[0]["TechStoreName"].ToString();
                    string NotesM = prevRows[0]["Notes"].ToString();
                    int TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                    int CoverID2 = 0;
                    int PatinaID2 = 0;
                    GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                    if (prevRows[0]["Length"] != DBNull.Value)
                        NewRow["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                        NewRow["Length"] = -1;

                    if (prevRows[0]["Height"] != DBNull.Value)
                        NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    else
                        NewRow["Height"] = -1;

                    if (prevRows[0]["Width"] != DBNull.Value)
                        NewRow["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                    else
                        NewRow["Width"] = -1;

                    NewRow["Name"] = NameM + " " + NotesM;
                    if (prevRows[0]["CoverID"] != DBNull.Value)
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                    else
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                    if (prevRows[0]["PatinaID"] != DBNull.Value)
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                    else
                        NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                    //вычисление кол-ва элементов
                    bool b = true;
                    decimal count = 1;
                    int x = 1;
                    int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                    while (b)
                    {
                        DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                        if (pRows.Count() > 0)
                        {
                            if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                            {
                                decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                                if (c % 1 == 0)
                                    count *= c;
                            }
                            x++;
                            PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                        }
                        else
                            b = false;
                    }
                    NewRow["Count"] = count * ItemsCount;
                    dt.Rows.Add(NewRow);
                }
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm14_2DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm14_2DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm14_2DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Clear();
            Algorithm14_1DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Декорэлементы» (рис.1 раздел1), подгруппы «Декорэл-ты собранные» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 78)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["Name"] = NameM + " " + NotesM;
                if (row["CoverID"] != DBNull.Value)
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                else
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                if (row["PatinaID"] != DBNull.Value)
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                else
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                //вычисление кол-ва элементов
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (pRows.Count() > 0)
                    {
                        if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Name = row["Name"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm14_1DT.Select("Name='" + Name + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Length=" + Length + " AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm14_1DT.NewRow();
                        NewRow["Name"] = Name;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Length"] = row["Length"];
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        Algorithm14_1DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }

            dt.Dispose();
            dt1.Dispose();
            dt2.Dispose();
        }

        //FORM 02

        private void Algorithm08(DataRow[] rows)
        {
            DataTable dt = Algorithm08DT.Clone();
            Algorithm08DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Эл-ты корпус. мебели», подгруппы «Заготовки ЛДСтП с кромкой сверленные» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 341
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 278
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 332
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 333)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Height"] != DBNull.Value)
                        row["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    else
                    {
                        if (prevRows[0]["Length"] != DBNull.Value)
                            row["Height"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["NotesM"] = NotesM;
                NewRow["NameM"] = NameM;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "Length, Width, Count";
                dt.Clear();
                dt = DV.ToTable();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Cover = row["Cover"].ToString();
                    string NotesM = row["NotesM"].ToString();
                    string NameM = row["NameM"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("Cover='" + Cover + "' AND NotesM='" + NotesM + "' AND NameM='" + NameM + "' AND Length =" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm08DT.Select("Cover='" + Cover + "' AND NotesM='" + NotesM + "' AND NameM='" + NameM + "' AND Length =" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm08DT.NewRow();
                        NewRow["Cover"] = Cover;
                        NewRow["NotesM"] = NotesM;
                        NewRow["NameM"] = NameM;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        Algorithm08DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 08
        
        private void Algorithm16(DataRow[] rows)
        {
            DataTable dt = Algorithm16DT.Clone();
            Algorithm16DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Пиленые полосы», подгруппы «Пиленый МДФ»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 44)
                    continue;

                bool b = true;
                bool nextElement = true;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        int TechStoreSubGroupID = Convert.ToInt32(prevRows[0]["TechStoreSubGroupID1"]);
                        string name = prevRows[0]["TechStoreName"].ToString();
                        //группы складов «Фасады», подгруппы «Фрезерованные»
                        if (TechStoreSubGroupID == 210)
                        {
                            nextElement = false;
                            break;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                if (nextElement)
                    continue;

                string NameM = row["TechStoreName"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //длина и ширина вносятся по цепочке 2 шага вверх
                int i = 0;
                PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (i != 2)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Length"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                        else
                        {
                            if (prevRows[0]["Height"] != DBNull.Value)
                                row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                        }
                        if (prevRows[0]["Width"] != DBNull.Value)
                            row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    i++;
                }

                //вычисление кол-ва элементов. 2 шаг(-а) вверх
                b = true;
                decimal count = 1;
                int x = 1;
                PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 2)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm16DT.Select("NameM='" + NameM + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm16DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm16DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm17(DataRow[] rows)
        {
            DataTable dt = Algorithm17DT.Clone();
            Algorithm17DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Пиленые полосы», подгруппы «Пиленый щит»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 41)
                    continue;

                bool b = true;
                bool nextElement = true;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        int TechStoreSubGroupID = Convert.ToInt32(prevRows[0]["TechStoreSubGroupID1"]);
                        string name = prevRows[0]["TechStoreName"].ToString();
                        //группы складов «Фасады», подгруппы «Фрезерованные»
                        if (TechStoreSubGroupID == 210)
                        {
                            nextElement = false;
                            break;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                if (nextElement)
                    continue;

                string NameM = row["TechStoreName"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                //вычисление кол-ва элементов. 3 шаг(-а) вверх
                b = true;
                decimal count = 1;
                int x = 1;
                PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 3)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm17DT.Select("NameM='" + NameM + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm17DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm17DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm18(DataRow[] rows)
        {
            DataTable dt = Algorithm18DT.Clone();
            Algorithm18DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Пиленые полосы», подгруппы «Пиленый ЛМДФ»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 47)
                    continue;

                bool b = true;
                bool nextElement = true;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        int TechStoreSubGroupID = Convert.ToInt32(prevRows[0]["TechStoreSubGroupID1"]);
                        string name = prevRows[0]["TechStoreName"].ToString();
                        //группы складов «Фасады», подгруппы «Фрезерованные»
                        if (TechStoreSubGroupID == 210)
                        {
                            nextElement = false;
                            break;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                if (nextElement)
                    continue;

                string NameM = row["TechStoreName"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                //вычисление кол-ва элементов. 2 шаг(-а) вверх
                b = true;
                decimal count = 1;
                int x = 1;
                PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 2)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm18DT.Select("NameM='" + NameM + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm18DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm18DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 10

        private void Algorithm05(DataRow[] rows)
        {
            DataTable dt = Algorithm05DT.Clone();
            Algorithm05DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Пиленые полосы», подгруппы «Пиленый ЛДСтП»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 46)
                    continue;

                string NameM = row["TechStoreName"].ToString();
                string Notes = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        string name = prevRows[0]["TechStoreName"].ToString();
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);

                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Notes"] = Notes;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "NameM, Cover, Notes, Length, Width, Count";
                dt.Clear();
                dt = DV.ToTable();
            }


            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Notes = row["Notes"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm05DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm05DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Notes"] = Notes;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm05DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm06(DataRow[] rows)
        {
            DataTable dt = Algorithm06DT.Clone();
            Algorithm06DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Пиленые полосы», подгруппы «Пиленый ВП»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 235)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string Notes = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Notes"] = Notes;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Notes = row["Notes"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm06DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm06DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Notes"] = Notes;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm06DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm07(DataRow[] rows)
        {
            DataTable dt = Algorithm07DT.Clone();
            Algorithm07DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Пиленые полосы», подгруппы «Пиленый ВК»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 48)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string Notes = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Notes"] = Notes;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Notes = row["Notes"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm07DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm07DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Notes"] = Notes;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm07DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm09(DataRow[] rows)
        {
            DataTable dt = Algorithm09DT.Clone();
            Algorithm09DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Эл-ты корпус. мебели» (рис.1 раздел1), подгруппы «Стеклянные вставки»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 294
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 344)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string Notes = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Notes"] = Notes;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Notes = row["Notes"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm09DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm09DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Notes"] = Notes;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm09DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm11(DataRow[] rows)
        {
            DataTable dt = Algorithm11DT.Clone();
            Algorithm11DT.Clear();
            foreach (DataRow row in rows)
            {
                int TechStoreSubGroupID1 = Convert.ToInt32(row["TechStoreSubGroupID1"]);
                //группы складов «Эл-ты корпус. мебели» (рис.1 раздел1), подгруппы «Заготовки ХДФ» и «Заготовки ДВП»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 342
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 286
                    && Convert.ToInt32(row["TechStoreSubGroupID1"]) != 335)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string Notes = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Width"] != DBNull.Value)
                        row["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                }

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Notes"] = Notes;

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                if (NewRow["Length"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Length"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Notes = row["Notes"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm11DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Notes='" + Notes + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm11DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Notes"] = Notes;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm11DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }
        
        //FORM 03

        private void Algorithm12(DataRow[] rows)
        {
            DataTable dt = Algorithm12DT.Clone();
            Algorithm12DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Погонажные изделия» (рис.1 раздел1), подгруппы «Погонаж спиленный»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 92)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //вычисление кол-ва элементов. 2 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 2)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                int i = 0;
                PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (i != 2)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Length"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                        else
                        {
                            if (prevRows[0]["Height"] != DBNull.Value)
                                row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    i++;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "NameM, Cover, Length, Count DESC";
                dt.Clear();
                dt = DV.ToTable();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Length=" + Length);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm12DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Length=" + Length);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm12DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm12DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm19(DataRow[] rows)
        {
            DataTable dt = Algorithm19DT.Clone();
            Algorithm19DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Погонажные изделия» (рис.1 раздел1), подгруппы «Погонаж спиленный под угол » 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 220)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "NameM, Cover, Length, Count DESC";
                dt.Clear();
                dt = DV.ToTable();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Length=" + Length);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm19DT.Select("NameM='" + NameM + "' AND Cover='" + Cover + "' AND Length=" + Length);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm19DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm19DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 11

        private void Algorithm10(DataRow[] rows)
        {
            DataTable dt = Algorithm10DT.Clone();
            Algorithm10DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Погонажные изделия», подгруппы «Погонаж спиленный сверленный»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 237)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name1 = row["TechStoreName1"].ToString();
                string Notes1 = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0 && prevRows[0]["Length"] != DBNull.Value)
                        row["Length"] = Convert.ToDecimal(prevRows[0]["Length"]);
                    else
                    {
                        if (prevRows[0]["Height"] != DBNull.Value)
                            row["Length"] = Convert.ToDecimal(prevRows[0]["Height"]);
                    }
                }

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["Notes"] = Notes1;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                NewRow["Count"] = count * ItemsCount;
                NewRow["Name"] = Name1;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Notes = row["Notes"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Name = row["Name"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);

                    DataRow[] drows = dt.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm10DT.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm10DT.NewRow();
                        NewRow["Notes"] = Notes;
                        NewRow["Cover"] = Cover;
                        NewRow["Name"] = Name;
                        NewRow["Length"] = row["Length"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm10DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm15(DataRow[] rows)
        {
            DataTable dt = Algorithm15DT.Clone();
            Algorithm15DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Декорэлементы», подгруппы «Декорэлементы со сверлением» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 236)
                    continue;
                string Name1 = row["TechStoreName1"].ToString();
                string Notes1 = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["Notes"] = Notes1;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                NewRow["Count"] = count * ItemsCount;
                NewRow["Name"] = Name1;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Notes = row["Notes"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Name = row["Name"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);

                    DataRow[] drows = dt.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm15DT.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm15DT.NewRow();
                        NewRow["Notes"] = Notes;
                        NewRow["Cover"] = Cover;
                        NewRow["Name"] = Name;
                        NewRow["Length"] = row["Length"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm15DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm20(DataRow[] rows)
        {
            DataTable dt = Algorithm20DT.Clone();
            Algorithm20DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Погонажные изделия» (рис.1 раздел1), подгруппы «Погонаж спиленный сверленный» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 237)
                    continue;
                string Name1 = row["TechStoreName1"].ToString();
                string Notes1 = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["Notes"] = Notes1;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                NewRow["Count"] = count * ItemsCount;
                NewRow["Name"] = Name1;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Notes = row["Notes"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Name = row["Name"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);

                    DataRow[] drows = dt.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm20DT.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm20DT.NewRow();
                        NewRow["Notes"] = Notes;
                        NewRow["Cover"] = Cover;
                        NewRow["Name"] = Name;
                        NewRow["Length"] = row["Length"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm20DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm34(DataRow[] rows)
        {
            DataTable dt = Algorithm34DT.Clone();
            Algorithm34DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Погонажные изделия» (рис.1 раздел1), подгруппы «Погонаж спиленный сверленный» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 237)
                    continue;
                string Name1 = row["TechStoreName1"].ToString();
                string Notes1 = row["Notes1"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["Notes"] = Notes1;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                NewRow["Count"] = count * ItemsCount;
                NewRow["Name"] = Name1;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string Notes = row["Notes"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Name = row["Name"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);

                    DataRow[] drows = dt.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm34DT.Select("Notes='" + Notes + "' AND Cover='" + Cover + "' AND Name='" + Name + "' AND Length=" + Length);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm34DT.NewRow();
                        NewRow["Notes"] = Notes;
                        NewRow["Cover"] = Cover;
                        NewRow["Name"] = Name;
                        NewRow["Length"] = row["Length"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm34DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 12

        private void Algorithm01(DataRow[] rows)
        {
            Algorithm01DT.Clear();
            foreach (DataRow row in rows)
            {
                //группы складов «Корпусная мебель», все подгруппы складов» 
                if (Convert.ToInt32(row["TechStoreGroupID1"]) != 52
                    && Convert.ToInt32(row["TechStoreGroupID1"]) != 53
                    && Convert.ToInt32(row["TechStoreGroupID1"]) != 45
                    && Convert.ToInt32(row["TechStoreGroupID1"]) != 56
                    && Convert.ToInt32(row["TechStoreGroupID1"]) != 57
                    && Convert.ToInt32(row["TechStoreGroupID1"]) != 58)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string OperationName = row["MachinesOperationName"].ToString();
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;

                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = Algorithm01DT.NewRow();
                NewRow["OperationName"] = OperationName;
                NewRow["Name"] = NameM + " " + NotesM;
                if (row["CoverID"] != DBNull.Value)
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                else
                    NewRow["Cover"] = AssignmentsManager.GetCoverName(-1);
                if (row["PatinaID"] != DBNull.Value)
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                else
                    NewRow["Patina"] = AssignmentsManager.GetPatinaName(-1);

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                if (row["Count"] != DBNull.Value)
                    NewRow["Count"] = Convert.ToDecimal(row["Count"]) * ItemsCount;
                else
                    NewRow["Count"] = 0;
                if (row["Count"] != DBNull.Value)
                    NewRow["PackCount"] = Convert.ToDecimal(row["Count"]);
                else
                    NewRow["PackCount"] = 0;
                Algorithm01DT.Rows.Add(NewRow);
            }
        }

        //FORM 04

        private void Algorithm21(DataRow[] rows)
        {
            DataTable dt = Algorithm21DT.Clone();
            Algorithm21DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Фасады» (рис.1 раздел1), подгруппы «Фрезерованные» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 210)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                
                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["NotesM"] = NotesM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];


                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string NotesM = row["NotesM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm21DT.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm21DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["NotesM"] = NotesM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm21DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm22(DataRow[] rows)
        {
            DataTable dt = Algorithm22DT.Clone();
            Algorithm22DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Фасады» (рис.1 раздел1), подгруппы «Фрезерованные» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 210)
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["NotesM"] = NotesM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];


                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string NotesM = row["NotesM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm22DT.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm22DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["NotesM"] = NotesM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm22DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm23(DataRow[] rows)
        {
            DataTable dt = Algorithm23DT.Clone();
            Algorithm23DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Фасады» (рис.1 раздел1), подгруппы «Фрезерованные под клеем» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 212)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["NotesM"] = NotesM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];
                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string NotesM = row["NotesM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm23DT.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm23DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["NotesM"] = NotesM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm23DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm24(DataRow[] rows)
        {
            DataTable dt = Algorithm24DT.Clone();
            Algorithm24DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Фасады» (рис.1 раздел1), подгруппы «Фрезерованные пресс» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 213)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["NotesM"] = NotesM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string NotesM = row["NotesM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm24DT.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm24DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["NotesM"] = NotesM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm24DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 05

        private void Algorithm25(DataRow[] rows)
        {
            DataTable dt = Algorithm25DT.Clone();
            Algorithm25DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                //«Фасады» (рис.1 раздел1), подгруппы «Патриция»
                if (Convert.ToInt32(rows[i]["TechStoreSubGroupID1"]) != 265)
                    continue;
                if (Convert.ToBoolean(rows[i]["IsHalfStuff2"]))
                    continue;
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            DataTable dtWithV = new DataTable();
            DataTable dtWithoutV = new DataTable();

            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "TechCatalogOperationsDetailID" });
                DV.RowFilter = "TechStoreName LIKE '%(в%'";
                dtWithV = DV.ToTable();
                DV.RowFilter = "TechStoreName NOT LIKE '%(в%'";
                dtWithoutV = DV.ToTable();
            }

            foreach (DataRow row in dt2.Rows)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(row["TechCatalogOperationsDetailID"]);

                DataRow[] wVRows = dtWithoutV.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
                if (wVRows.Count() > 0)
                {
                    int PrevStoreDetailID = Convert.ToInt32(wVRows[0]["PrevStoreDetailID"]);
                    int CoverID2 = 0;
                    int PatinaID2 = 0;

                    DataRow NewRow = dt.NewRow();

                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0)
                    {
                        string Name = prevRows[0]["TechStoreName"].ToString();
                        string Notes = prevRows[0]["Notes"].ToString();
                        int TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                        int TechCatalogStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);

                        GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                        NewRow["NotesMNameM"] = Notes + " " + Name;
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                        
                        if (prevRows[0]["Height"] != DBNull.Value)
                            NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                        else
                            NewRow["Height"] = -1;
                        if (prevRows[0]["Length"] != DBNull.Value && prevRows[0]["Height"] == DBNull.Value)
                            NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Length"]);

                        if (prevRows[0]["Width"] != DBNull.Value)
                            NewRow["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                        else
                            NewRow["Width"] = -1;

                        //вычисление кол-ва элементов
                        bool b = true;
                        decimal count = 1;
                        int x = 1;
                        int PrevStoreDetailID1 = Convert.ToInt32(wVRows[0]["PrevStoreDetailID"]);
                        while (b)
                        {
                            DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                            if (pRows.Count() > 0)
                            {
                                if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                                {
                                    decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                                    if (c % 1 == 0)
                                        count *= c;
                                }
                                x++;
                                PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                            }
                            else
                                b = false;
                        }
                        NewRow["Count"] = count * ItemsCount;

                        if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                            NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                        else
                            NewRow["Square"] = 0;

                    }

                    string Filling = "empty";

                    DataRow[] vRows = dtWithV.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
                    if (vRows.Count() > 0)
                    {
                        Filling = "";
                        for (int j = 0; j < vRows.Count(); j++)
                        {
                            int TechCatalogStoreDetailID = Convert.ToInt32(vRows[j]["TechCatalogStoreDetailID"]);
                            DataRow[] pRows = SumStoreDetailDT.Select("PrevStoreDetailID=" + TechCatalogStoreDetailID);
                            if (pRows.Count() > 0)
                            {
                                if (j != vRows.Count() - 1)
                                    Filling += pRows[0]["TechStoreName"].ToString() + "+";
                                else
                                    Filling += pRows[0]["TechStoreName"].ToString();
                            }
                        }
                    }
                    NewRow["Filling"] = Filling;
                    NewRow["Worker"] = string.Empty;
                    dt.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "NotesMNameM, Cover, Filling, Height DESC, Width DESC";
                dt.Clear();
                dt = DV.ToTable();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Filling = row["Filling"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Filling='" + Filling + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm25DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Filling='" + Filling + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm25DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Filling"] = Filling;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm25DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm32(DataRow[] rows)
        {
            DataTable dt = Algorithm32DT.Clone();
            Algorithm32DT.Clear();

            DataTable dt1 = SumStoreDetailDT.Clone();
            for (int i = 0; i < rows.Length; i++)
            {
                //«Корпусная мебель Норман» (рис.1 раздел1)
                if (Convert.ToInt32(rows[i]["TechStoreGroupID1"]) != 52)
                    continue;
                if (Convert.ToBoolean(rows[i]["IsHalfStuff2"]))
                    continue;
                DataRow NewRow = dt1.NewRow();
                NewRow.ItemArray = rows[i].ItemArray;
                dt1.Rows.Add(NewRow);
            }
            DataTable dt2 = new DataTable();
            DataTable dtWithV = new DataTable();
            DataTable dtWithoutV = new DataTable();

            using (DataView DV = new DataView(dt1))
            {
                dt2 = DV.ToTable(true, new string[] { "TechCatalogOperationsDetailID" });
                DV.RowFilter = "TechStoreName LIKE '%(в%'";
                dtWithV = DV.ToTable();
                DV.RowFilter = "TechStoreName NOT LIKE '%(в%'";
                dtWithoutV = DV.ToTable();
            }

            foreach (DataRow row in dt2.Rows)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(row["TechCatalogOperationsDetailID"]);

                DataRow[] wVRows = dtWithoutV.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
                if (wVRows.Count() > 0)
                {
                    int PrevStoreDetailID = Convert.ToInt32(wVRows[0]["PrevStoreDetailID"]);
                    int CoverID2 = 0;
                    int PatinaID2 = 0;

                    DataRow NewRow = dt.NewRow();

                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID);
                    if (prevRows.Count() > 0)
                    {
                        string Name = prevRows[0]["TechStoreName"].ToString();
                        string Notes = prevRows[0]["Notes"].ToString();
                        int TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                        int TechCatalogStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);

                        GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                        NewRow["NotesMNameM"] = Notes + " " + Name;
                        NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                        if (prevRows[0]["Height"] != DBNull.Value)
                            NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Height"]);
                        else
                            NewRow["Height"] = -1;
                        if (prevRows[0]["Length"] != DBNull.Value && prevRows[0]["Height"] == DBNull.Value)
                            NewRow["Height"] = Convert.ToDecimal(prevRows[0]["Length"]);

                        if (prevRows[0]["Width"] != DBNull.Value)
                            NewRow["Width"] = Convert.ToDecimal(prevRows[0]["Width"]);
                        else
                            NewRow["Width"] = -1;
                        
                        //вычисление кол-ва элементов
                        bool b = true;
                        decimal count = 1;
                        int x = 1;
                        int PrevStoreDetailID1 = Convert.ToInt32(wVRows[0]["PrevStoreDetailID"]);
                        while (b)
                        {
                            DataRow[] pRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                            if (pRows.Count() > 0)
                            {
                                if (pRows[0]["Count"] != DBNull.Value && x >= 1)
                                {
                                    decimal c = Convert.ToDecimal(pRows[0]["Count"]);
                                    if (c % 1 == 0)
                                        count *= c;
                                }
                                x++;
                                PrevStoreDetailID1 = Convert.ToInt32(pRows[0]["PrevStoreDetailID"]);
                            }
                            else
                                b = false;
                        }
                        NewRow["Count"] = count * ItemsCount;

                        if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                            NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                        else
                            NewRow["Square"] = 0;

                    }

                    string Filling = "empty";

                    DataRow[] vRows = dtWithV.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
                    if (vRows.Count() > 0)
                    {
                        Filling = "";
                        for (int j = 0; j < vRows.Count(); j++)
                        {
                            int TechCatalogStoreDetailID = Convert.ToInt32(vRows[j]["TechCatalogStoreDetailID"]);
                            DataRow[] pRows = SumStoreDetailDT.Select("PrevStoreDetailID=" + TechCatalogStoreDetailID);
                            if (pRows.Count() > 0)
                            {
                                if (j != vRows.Count() - 1)
                                    Filling += pRows[0]["TechStoreName"].ToString() + "+";
                                else
                                    Filling += pRows[0]["TechStoreName"].ToString();
                            }
                        }
                    }
                    NewRow["Filling"] = Filling;
                    NewRow["Worker"] = string.Empty;
                    dt.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(dt.Copy()))
            {
                DV.Sort = "NotesMNameM, Cover, Filling, Height DESC, Width DESC";
                dt.Clear();
                dt = DV.ToTable();
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Filling = row["Filling"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Filling='" + Filling + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm32DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Filling='" + Filling + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm32DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Filling"] = Filling;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm32DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 07

        private void Algorithm26(DataRow[] rows)
        {
            DataTable dt = Algorithm26DT.Clone();
            Algorithm26DT.Clear();
            foreach (DataRow row in rows)
            {
                int TechStoreSubGroupID1 = Convert.ToInt32(row["TechStoreSubGroupID1"]);
                //«Фасады» (рис.1 раздел1), подгруппы «Патриция ЛКМ» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 281)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                
                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm26DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm26DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm26DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm33(DataRow[] rows)
        {
            DataTable dt = Algorithm33DT.Clone();
            Algorithm33DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Корпусная мебель Норман»
                if (Convert.ToInt32(row["TechStoreGroupID1"]) != 52)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm33DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm33DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm33DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        //FORM 09

        private void Algorithm27(DataRow[] rows)
        {
            DataTable dt = Algorithm27DT.Clone();
            Algorithm27DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Декорэлементы» (рис.1 раздел1), подгруппы «Декорэл-ты с лкм» (рис.1 раздел2)
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 73)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }

                    DataRow[] frows = Algorithm27DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm27DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm27DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm28(DataRow[] rows)
        {
            DataTable dt = Algorithm28DT.Clone();
            Algorithm28DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Эл-ты к.м. Патриция» (рис.1 раздел1), подгруппы «Эл-ты к.м. под ЛКМ» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 267)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;

                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }

                    DataRow[] frows = Algorithm28DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm28DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm28DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm29(DataRow[] rows)
        {
            DataTable dt = Algorithm29DT.Clone();
            Algorithm29DT.Clear();
            foreach (DataRow row in rows)
            {
                int TechStoreSubGroupID1 = Convert.ToInt32(row["TechStoreSubGroupID1"]);
                //«Фасады» (рис.1 раздел1), подгруппы «Патриция ЛКМ» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 281)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;
                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }

                    DataRow[] frows = Algorithm29DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm29DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm29DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm30(DataRow[] rows)
        {
            DataTable dt = Algorithm28DT.Clone();
            Algorithm28DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Фасады» (рис.1 раздел1), подгруппы «Фрезерованные с лкм» 
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 214)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);
                
                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;

                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }

                    DataRow[] frows = Algorithm28DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm28DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm28DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm31(DataRow[] rows)
        {
            DataTable dt = Algorithm31DT.Clone();
            Algorithm31DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Погонажные изделия» (рис.1 раздел1), подгруппы «Погонаж спиленный сверленный»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 237)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string Name = row["TechStoreName"].ToString();
                string Notes = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);
                
                //вычисление кол-ва элементов. 1 шаг(-а) вверх
                bool b = true;
                decimal count = 1;
                int x = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value && x >= 1)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        x++;
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }

                DataRow NewRow = dt.NewRow();
                NewRow["NotesMNameM"] = Notes + " " + Name;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);
                NewRow["Patina"] = AssignmentsManager.GetPatinaName(PatinaID2);

                if (row["Height"] != DBNull.Value)
                    NewRow["Height"] = Convert.ToDecimal(row["Height"]);
                else
                    NewRow["Height"] = -1;

                if (row["Length"] != DBNull.Value && row["Height"] == DBNull.Value)
                    NewRow["Height"] = row["Length"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;
                
                NewRow["Count"] = count * ItemsCount;

                if (NewRow["Height"] != DBNull.Value && NewRow["Width"] != DBNull.Value)
                    NewRow["Square"] = Convert.ToDecimal(NewRow["Count"]) * Convert.ToDecimal(NewRow["Height"]) * Convert.ToDecimal(NewRow["Width"]) / 1000000;
                else
                    NewRow["Square"] = 0;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NotesMNameM = row["NotesMNameM"].ToString();
                    string Cover = row["Cover"].ToString();
                    string Patina = row["Patina"].ToString();
                    int Height = Convert.ToInt32(row["Height"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    decimal Count = 0;
                    decimal Square = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                        if (item["Square"] != DBNull.Value)
                            Square += Convert.ToDecimal(item["Square"]);
                    }

                    DataRow[] frows = Algorithm31DT.Select("NotesMNameM='" + NotesMNameM + "' AND Cover='" + Cover + "' AND Patina='" + Patina + "' AND Height=" + Height + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm31DT.NewRow();
                        NewRow["NotesMNameM"] = NotesMNameM;
                        NewRow["Cover"] = Cover;
                        NewRow["Patina"] = Patina;
                        NewRow["Height"] = row["Height"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Square"] = row["Square"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm31DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                        frows[0]["Square"] = Square;
                    }
                }
            }
            dt.Dispose();
        }

        private void Algorithm35(DataRow[] rows)
        {
            DataTable dt = Algorithm35DT.Clone();
            Algorithm35DT.Clear();
            foreach (DataRow row in rows)
            {
                //«Эл-ты к.м. Куб» (рис.1 раздел1), подгруппы «Заготовки под отделку»
                if (Convert.ToInt32(row["TechStoreSubGroupID1"]) != 334)
                    continue;
                if (Convert.ToBoolean(row["IsHalfStuff2"]))
                    continue;
                string NameM = row["TechStoreName"].ToString();
                string NotesM = row["Notes"].ToString();
                int PrevStoreDetailID = Convert.ToInt32(row["PrevStoreDetailID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(row["TechCatalogStoreDetailID"]);
                int TechStoreID = Convert.ToInt32(row["TechStoreID"]);
                int CoverID2 = 0;
                int PatinaID2 = 0;
                GetCoverPatinaName(TechCatalogStoreDetailID, CoverID, PatinaID, TechStoreID, ref CoverID2, ref PatinaID2);

                DataRow NewRow = dt.NewRow();
                NewRow["NameM"] = NameM;
                NewRow["NotesM"] = NotesM;
                NewRow["Cover"] = AssignmentsManager.GetCoverName(CoverID2);

                if (row["Length"] != DBNull.Value)
                    NewRow["Length"] = Convert.ToDecimal(row["Length"]);
                else
                    NewRow["Length"] = -1;
                if (row["Height"] != DBNull.Value && row["Length"] == DBNull.Value)
                    NewRow["Length"] = row["Height"];

                if (row["Width"] != DBNull.Value)
                    NewRow["Width"] = Convert.ToDecimal(row["Width"]);
                else
                    NewRow["Width"] = -1;


                bool b = true;
                decimal count = 1;
                int PrevStoreDetailID1 = Convert.ToInt32(row["PrevStoreDetailID"]);
                while (b)
                {
                    DataRow[] prevRows = SumStoreDetailDT.Select("TechCatalogStoreDetailID=" + PrevStoreDetailID1);
                    if (prevRows.Count() > 0)
                    {
                        if (prevRows[0]["Count"] != DBNull.Value)
                        {
                            decimal c = Convert.ToDecimal(prevRows[0]["Count"]);
                            if (c % 1 == 0)
                                count *= c;
                        }
                        PrevStoreDetailID1 = Convert.ToInt32(prevRows[0]["PrevStoreDetailID"]);
                    }
                    else
                        b = false;
                }
                NewRow["Count"] = count * ItemsCount;
                NewRow["Worker"] = string.Empty;
                dt.Rows.Add(NewRow);
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string NameM = row["NameM"].ToString();
                    string NotesM = row["NotesM"].ToString();
                    string Cover = row["Cover"].ToString();
                    int Length = Convert.ToInt32(row["Length"]);
                    int Width = Convert.ToInt32(row["Width"]);

                    DataRow[] drows = dt.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    decimal Count = 0;
                    foreach (DataRow item in drows)
                    {
                        if (item["Count"] != DBNull.Value)
                            Count += Convert.ToDecimal(item["Count"]);
                    }
                    f(drows);

                    DataRow[] frows = Algorithm35DT.Select("NameM='" + NameM + "' AND NotesM='" + NotesM + "' AND Cover='" + Cover + "' AND Length=" + Length + " AND Width=" + Width);
                    if (frows.Count() == 0)
                    {
                        DataRow NewRow = Algorithm35DT.NewRow();
                        NewRow["NameM"] = NameM;
                        NewRow["NotesM"] = NotesM;
                        NewRow["Cover"] = Cover;
                        NewRow["Length"] = row["Length"];
                        NewRow["Width"] = row["Width"];
                        NewRow["Count"] = row["Count"];
                        NewRow["Worker"] = string.Empty;
                        Algorithm35DT.Rows.Add(NewRow);
                    }
                    else
                    {
                        frows[0]["Count"] = Count;
                    }
                }
            }
            dt.Dispose();
        }


        //public bool GetCoverPatinaName(int CoverID1, int PatinaID1, int TechStoreID, ref int CoverID2, ref int PatinaID2)
        //{
        //    DataRow[] rows = CubFurCoversDT.Select("TechStoreID = " + TechStoreID + " AND CoverID1 = " + CoverID1 + " AND PatinaID1 = " + PatinaID1);
        //    if (rows.Count() > 0)
        //    {
        //        CoverID2 = Convert.ToInt32(rows[0]["CoverID2"]);
        //        PatinaID2 = Convert.ToInt32(rows[0]["PatinaID2"]);
        //        return true;
        //    }
        //    CoverID2 = CoverID1;
        //    PatinaID2 = PatinaID1;
        //    return false;
        //}

        public bool GetCoverPatinaName(int PrevStoreDetailID, int CoverID1, int PatinaID1, int TechStoreID, ref int CoverID2, ref int PatinaID2)
        {
            DataRow[] pRows = CubFurCoversDT.Select("TechStoreID = " + TechStoreID + " AND CoverID1 = " + CoverID1 + " AND PatinaID1 = " + PatinaID1);
            if (pRows.Count() > 0)
            {
                CoverID2 = Convert.ToInt32(pRows[0]["CoverID2"]);
                PatinaID2 = Convert.ToInt32(pRows[0]["PatinaID2"]);
                return true;
            }

            bool b = true;
            while (b)
            {
                DataRow[] prevRows = SumStoreDetailDT.Select("PrevStoreDetailID=" + PrevStoreDetailID);
                if (prevRows.Count() > 0)
                {
                    TechStoreID = Convert.ToInt32(prevRows[0]["TechStoreID"]);
                    string NameM = prevRows[0]["TechStoreName"].ToString();
                    string NotesM = prevRows[0]["Notes"].ToString();
                    DataRow[] rows = CubFurCoversDT.Select("TechStoreID = " + TechStoreID + " AND CoverID1 = " + CoverID1 + " AND PatinaID1 = " + PatinaID1);
                    if (rows.Count() > 0)
                    {
                        CoverID2 = Convert.ToInt32(rows[0]["CoverID2"]);
                        PatinaID2 = Convert.ToInt32(rows[0]["PatinaID2"]);
                        return true;
                    }
                    PrevStoreDetailID = Convert.ToInt32(prevRows[0]["TechCatalogStoreDetailID"]);
                }
                else
                    b = false;
            }

            CoverID2 = CoverID1;
            PatinaID2 = PatinaID1;
            return false;
        }

        int NestedLevel = 1;

        public void MainFunction(int iCabFurAssignmentID, string sTechStoreName, int iTechStoreID, int iCoverID, int iPatinaID, int iLength, int iHeight, int iWidth, string sCoverName, string sPatinaName, int iItemsCount)
        {
            TechStoreName = sTechStoreName;
            TechStoreID = iTechStoreID;
            CoverID = iCoverID;
            PatinaID = iPatinaID;
            Length = iLength;
            Height = iHeight;
            Width = iWidth;
            CoverName = sCoverName;
            PatinaName = sPatinaName;
            ItemsCount = iItemsCount;
            SumOperationsDetailDT.Clear();
            SumStoreDetailDT.Clear();
            string SelectCommand = @"SELECT TechCatalogOperationsDetail.*, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupName, 
                MachinesOperations.MachinesOperationName, MachinesOperations.Norm, MachinesOperations.PreparatoryNorm, MachinesOperations.MeasureID, MachinesOperations.CabFurDocTypeID, MachinesOperations.CabFurAlgorithmID, CabFurnitureDocumentTypes.DocName, 
                Machines.MachineID, Machines.MachineName, SubSectors.SubSectorID, SubSectors.SubSectorName, Sectors.SectorID, Sectors.SectorName, Measure FROM TechCatalogOperationsDetail
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID AND GroupNumber=1
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                INNER JOIN infiniu2_storage.dbo.CabFurnitureDocumentTypes AS CabFurnitureDocumentTypes ON MachinesOperations.CabFurDocTypeID = CabFurnitureDocumentTypes.CabFurDocTypeID
                INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
                INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
                INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID
                INNER JOIN Measures ON MachinesOperations.MeasureID = Measures.MeasureID
                WHERE TechStoreID=" + TechStoreID + " ORDER BY SerialNumber";

            //SelectCommand = @"SELECT Sectors.SectorName, SubSectors.SubSectorName, Machines.MachineName, TechCatalogOperationsGroups.GroupName, TechCatalogOperationsGroups.TechStoreID, MachinesOperations.MachinesOperationName, TechCatalogOperationsDetail.*, 
            //    MachinesOperations.CabFurDocTypeID, MachinesOperations.CabFurAlgorithmID, CabFurnitureDocumentTypes.DocName, CabFurnitureDocumentTypes.AssignmentID, CabFurnitureAlgorithms.CabFurAlgorithmID FROM TechCatalogOperationsDetail
            //    INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
            //    INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
            //    INNER JOIN infiniu2_storage.dbo.CabFurnitureDocumentTypes AS CabFurnitureDocumentTypes ON MachinesOperations.CabFurDocTypeID = CabFurnitureDocumentTypes.CabFurDocTypeID
            //    INNER JOIN infiniu2_storage.dbo.CabFurnitureAlgorithms AS CabFurnitureAlgorithms ON MachinesOperations.CabFurAlgorithmID = CabFurnitureAlgorithms.CabFurAlgorithmID
            //    INNER JOIN Machines ON MachinesOperations.MachineID = Machines.MachineID
            //    INNER JOIN SubSectors ON Machines.SubSectorID = SubSectors.SubSectorID
            //    INNER JOIN Sectors ON SubSectors.SectorID = Sectors.SectorID WHERE TechStoreID=" + TechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = SumOperationsDetailDT.NewRow();
                        NewRow.ItemArray = DT.Rows[i].ItemArray;
                        NewRow["NestedLevel"] = NestedLevel;
                        NewRow["PrevStoreDetailID"] = TechStoreID;
                        NewRow["Done"] = false;
                        SumOperationsDetailDT.Rows.Add(NewRow);
                    }
                }
            }

            bool b = false;
            while (!b)
            {
                DataRow[] rows = SumOperationsDetailDT.Select("Done=0");
                if (rows.Count() == 0)
                    break;

                int j = SumOperationsDetailDT.Rows.Count;
                for (int i = 0; i < j; i++)
                {
                    int iNestedLevel = NestedLevel;
                    bool Done = Convert.ToBoolean(SumOperationsDetailDT.Rows[i]["Done"]);
                    if (Done)
                        continue;
                    int PrevStoreDetailID = Convert.ToInt32(SumOperationsDetailDT.Rows[i]["PrevStoreDetailID"]);
                    int TechCatalogOperationsDetailID = Convert.ToInt32(SumOperationsDetailDT.Rows[i]["TechCatalogOperationsDetailID"]);
                    string MachinesOperationName = SumOperationsDetailDT.Rows[i]["MachinesOperationName"].ToString();
                    if (!CheckConditions(TechCatalogOperationsDetailID, CoverID, PatinaID, Length, Height, Width))
                        continue;
                    GetStoreDetail(TechCatalogOperationsDetailID, PrevStoreDetailID,
                        Convert.ToInt32(SumOperationsDetailDT.Rows[i]["NestedLevel"]), SumOperationsDetailDT.Rows[i]);
                    SumOperationsDetailDT.Rows[i]["Done"] = true;
                }
            }

            foreach (DataRow item in SumStoreDetailDT.Rows)
            {
                int techStoreId = Convert.ToInt32(item["TechStoreID"]);
                DataRow[] rows = OperationsGroups.Select("TechStoreID=" + techStoreId);
                if (rows.Count() == 0)
                    item["Consumable"] = true;
                else
                    item["Consumable"] = false;
            }
            DataTable CabFurDocTypes = new DataTable();
            using (DataView DV = new DataView(SumStoreDetailDT))
            {
                DV.RowFilter = "CabFurDocTypeID<>-1";
                DV.Sort = "CabFurDocTypeID";
                CabFurDocTypes = DV.ToTable(true, new string[] { "CabFurDocTypeID", "DocName", "CabFurAlgorithmID", "MachineID" });
            }
            DataTable CabFurDocTypes1 = new DataTable();
            using (DataView DV = new DataView(SumOperationsDetailDT))
            {
                DV.RowFilter = "CabFurDocTypeID<>-1";
                DV.Sort = "CabFurDocTypeID";
                CabFurDocTypes1 = DV.ToTable(true, new string[] { "CabFurDocTypeID", "DocName", "CabFurAlgorithmID", "MachineID", "MachineName" });
            }
            ReportToExcel r = new ReportToExcel();
            //DataTable tSumStoreDetailDT = SumStoreDetailDT.Clone();
            foreach (DataRow item in CabFurDocTypes1.Rows)
            {
                int CabFurDocTypeID = Convert.ToInt32(item["CabFurDocTypeID"]);
                int CabFurAlgorithmID = Convert.ToInt32(item["CabFurAlgorithmID"]);
                int MachineID = Convert.ToInt32(item["MachineID"]);
                string DocName = item["DocName"].ToString();
                string MachineName = item["MachineName"].ToString();
                DataRow[] rows = SumStoreDetailDT.Select("CabFurDocTypeID=" + CabFurDocTypeID + " AND MachineID=" + MachineID + " AND CabFurAlgorithmID=" + CabFurAlgorithmID);
                //tSumStoreDetailDT.Clear();
                //foreach (DataRow row in rows)
                //{
                //    DataRow NewRow = tSumStoreDetailDT.NewRow();
                //    NewRow.ItemArray = row.ItemArray;
                //    tSumStoreDetailDT.Rows.Add(NewRow);
                //}
                if (rows.Count() == 0)
                {
                    DataRow[] rrows = SumOperationsDetailDT.Select("CabFurDocTypeID=" + CabFurDocTypeID + " AND MachineID=" + MachineID + " AND CabFurAlgorithmID=" + CabFurAlgorithmID);
                    if (rrows.Count() > 0)
                    {
                        string filter = string.Empty;
                        for (int i = 0; i < rrows.Count(); i++)
                            filter += Convert.ToInt32(rrows[i]["TechCatalogOperationsGroupID"]) + ",";
                        if (filter.Length > 0)
                        {
                            filter = filter.Substring(0, filter.Length - 1);
                            filter = " AND TechCatalogOperationsGroupID IN (" + filter + ")";
                        }
                        else
                            filter = " AND TechCatalogOperationsGroupID <> - 1";
                        
                        DataRow[] prevRows = SumStoreDetailDT.Select("CabFurDocTypeID<>-1 " + filter);
                        if (prevRows.Count() > 0)
                        {
                            fuck(r, prevRows, iCabFurAssignmentID, CabFurAlgorithmID, MachineName, DocName);
                        }
                    }
                    continue;

                }
                int FormNumber = Convert.ToInt32(rows[0]["AssignmentID"]);
                
                string GroupOperationName = rows[0]["GroupName"].ToString();
                string OperationName1 = rows[0]["MachinesOperationName"].ToString();
                string OperationName = DocName;
                string Sector = rows[0]["SectorName"].ToString();
                string SubSector = rows[0]["SubSectorName"].ToString();
                string CupboardsName = TechStoreName;
                string JobNumber = "№ " + iCabFurAssignmentID.ToString();

                SelectAlgorithm(CabFurAlgorithmID, FormNumber, r, rows, MachineName, OperationName, CupboardsName, JobNumber);
            }
            r.SaveFile("новое задание", true);
        }

        public void fuck(ReportToExcel r, DataRow[] rows, int iCabFurAssignmentID, int CabFurAlgorithmID, string MachineName, string DocName)
        {
            //for (int i = 0; i < rows.Count(); i++)
            //{
                int FormNumber = Convert.ToInt32(rows[0]["AssignmentID"]);
                
                string GroupOperationName = rows[0]["GroupName"].ToString();
                string OperationName1 = rows[0]["MachinesOperationName"].ToString();
                string OperationName = DocName;
                string Sector = rows[0]["SectorName"].ToString();
                string SubSector = rows[0]["SubSectorName"].ToString();
                string CupboardsName = TechStoreName;
                string JobNumber = "№ " + iCabFurAssignmentID.ToString();

                SelectAlgorithm(CabFurAlgorithmID, FormNumber, r, rows, MachineName, OperationName, CupboardsName, JobNumber);
            //}
        }

        private void SelectAlgorithm(int Algorithm, int FormNumber, ReportToExcel r, DataRow[] rows, string MachineName, string OperationName, string CupboardsName, string JobNumber)
        {
            switch (Algorithm)
            {
                case 1:
                    Algorithm01(rows);
                    if (Algorithm01DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm01DT);
                    break;
                case 2:
                    Algorithm02(rows);
                    if (Algorithm02_1DT.Rows.Count > 0 || Algorithm02_2DT.Rows.Count > 0)
                        r.Form01(MachineName, OperationName, CupboardsName, JobNumber, Algorithm02_1DT, Algorithm02_2DT);
                    break;
                case 3:
                    Algorithm03(rows);
                    if (Algorithm03_1DT.Rows.Count > 0 || Algorithm03_2DT.Rows.Count > 0)
                        r.Form01(MachineName, OperationName, CupboardsName, JobNumber, Algorithm03_1DT, Algorithm03_2DT);
                    break;
                case 4:
                    Algorithm04(rows);
                    if (Algorithm04DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm04DT);
                    break;
                case 5:
                    Algorithm05(rows);
                    if (Algorithm05DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm05DT);
                    break;
                case 6:
                    Algorithm06(rows);
                    if (Algorithm06DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm06DT);
                    break;
                case 7:
                    Algorithm07(rows);
                    if (Algorithm07DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm07DT);
                    break;
                case 8:
                    Algorithm08(rows);
                    if (Algorithm08DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm08DT);
                    break;
                case 9:
                    Algorithm09(rows);
                    if (Algorithm09DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm09DT);
                    break;
                case 10:
                    Algorithm10(rows);
                    if (Algorithm10DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm10DT);
                    break;
                case 11:
                    Algorithm11(rows);
                    if (Algorithm11DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm11DT);
                    break;
                case 12:
                    Algorithm12(rows);
                    if (Algorithm12DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm12DT);
                    break;
                case 13:
                    Algorithm13(rows);
                    if (Algorithm13_1DT.Rows.Count > 0 || Algorithm13_2DT.Rows.Count > 0)
                        r.Form01(MachineName, OperationName, CupboardsName, JobNumber, Algorithm13_1DT, Algorithm13_2DT);
                    break;
                case 14:
                    Algorithm14(rows);
                    if (Algorithm14_1DT.Rows.Count > 0 || Algorithm14_2DT.Rows.Count > 0)
                        r.Form01(MachineName, OperationName, CupboardsName, JobNumber, Algorithm14_1DT, Algorithm14_2DT);
                    break;
                case 15:
                    Algorithm15(rows);
                    if (Algorithm15DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm15DT);
                    break;
                case 16:
                    Algorithm16(rows);
                    if (Algorithm16DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm16DT);
                    break;
                case 17:
                    Algorithm17(rows);
                    if (Algorithm17DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm17DT);
                    break;
                case 18:
                    Algorithm18(rows);
                    if (Algorithm18DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm18DT);
                    break;
                case 19:
                    Algorithm19(rows);
                    if (Algorithm19DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm19DT);
                    break;
                case 20:
                    Algorithm20(rows);
                    if (Algorithm20DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm20DT);
                    break;
                case 21:
                    Algorithm21(rows);
                    if (Algorithm21DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm21DT);
                    break;
                case 22:
                    Algorithm22(rows);
                    if (Algorithm22DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm22DT);
                    break;
                case 23:
                    Algorithm23(rows);
                    if (Algorithm23DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm23DT);
                    break;
                case 24:
                    Algorithm24(rows);
                    if (Algorithm24DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm24DT);
                    break;
                case 25:
                    Algorithm25(rows);
                    if (Algorithm25DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm25DT);
                    break;
                case 26:
                    Algorithm26(rows);
                    if (Algorithm26DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm26DT);
                    break;
                case 27:
                    Algorithm27(rows);
                    if (Algorithm27DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm27DT);
                    break;
                case 28:
                    Algorithm28(rows);
                    if (Algorithm28DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm28DT);
                    break;
                case 29:
                    Algorithm29(rows);
                    if (Algorithm29DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm29DT);
                    break;
                case 30:
                    Algorithm30(rows);
                    if (Algorithm30DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm30DT);
                    break;
                case 31:
                    Algorithm31(rows);
                    if (Algorithm31DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm31DT);
                    break;
                case 32:
                    Algorithm32(rows);
                    if (Algorithm32DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm32DT);
                    break;
                case 33:
                    Algorithm33(rows);
                    if (Algorithm33DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm33DT);
                    break;
                case 34:
                    Algorithm34(rows);
                    if (Algorithm34DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm34DT);
                    break;
                case 35:
                    Algorithm35(rows);
                    if (Algorithm35DT.Rows.Count > 0)
                        SelectForm(FormNumber, r, MachineName, OperationName, CupboardsName, JobNumber, Algorithm35DT);
                    break;
                default:
                    break;
            }
        }

        private void SelectForm(int FormNumber, ReportToExcel r, string MachineName, string OperationName, string CupboardsName, string JobNumber, DataTable dt)
        {
            switch (FormNumber)
            {
                case 1:
                    break;
                case 2:
                    r.Form02(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 3:
                    r.Form03(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 4:
                    r.Form04(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 5:
                    r.Form05(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 6:
                    r.Form06(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 7:
                    r.Form07(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 8:
                    r.Form08(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 9:
                    r.Form09(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 10:
                    r.Form10(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 11:
                    r.Form11(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                case 12:
                    r.Form12(MachineName, OperationName, CupboardsName, JobNumber, dt);
                    break;
                default:
                    break;
            }
        }

        public bool GetOperationsDetail(int PrevOperationsDetailID, int PrevStoreDetailID, int NestedLevel, DataRow Row)
        {
            int TechStoreID = Convert.ToInt32(Row["TechStoreID"]);

            string TechStoreName = Row["TechStoreName"].ToString();
            bool BreakChain = Convert.ToBoolean(Row["BreakChain"]);
            if (BreakChain)
                return false;
            DataRow[] rows = OperationsDetailDT.Select("TechStoreID=" + Convert.ToInt32(Row["TechStoreID"]));
            if (rows.Count() == 0)
            {
            }
            else
            {
                foreach (DataRow item in rows)
                {
                    int TechCatalogOperationsDetailID = Convert.ToInt32(item["TechCatalogOperationsDetailID"]);
                    if (!CheckConditions(Convert.ToInt32(item["TechCatalogOperationsDetailID"]), CoverID, PatinaID, Length, Height, Width))
                        continue;
                    //if (SumOperationsDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID).Count() > 0)
                    //    continue;
                    if (TechCatalogOperationsDetailID == PrevOperationsDetailID)
                        continue;
                    DataRow NewRow = SumOperationsDetailDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["PrevOperationsDetailID"] = PrevOperationsDetailID;
                    NewRow["PrevStoreDetailID"] = PrevStoreDetailID;
                    NewRow["NestedLevel"] = NestedLevel;
                    NewRow["Done"] = false;
                    SumOperationsDetailDT.Rows.Add(NewRow);
                }
            }
            NestedLevel--;

            return rows.Count() > 0;
        }

        public bool GetStoreDetail(int PrevOperationsDetailID, int PrevStoreDetailID, int NestedLevel, DataRow Row)
        {
            int TechCatalogOperationsDetailID = Convert.ToInt32(Row["TechCatalogOperationsDetailID"]);
            int MachinesOperationID = Convert.ToInt32(Row["MachinesOperationID"]);
            string MachinesOperationName = Row["MachinesOperationName"].ToString();
            DataRow[] rows = StoreDetailDT.Select("TechCatalogOperationsDetailID=" + Convert.ToInt32(Row["TechCatalogOperationsDetailID"]));
            if (rows.Count() == 0)
            {
                //if (SumOperationsDetailDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID).Count() > 0)
                //    return false;
                //if (TechCatalogOperationsDetailID == PrevOperationsDetailID)
                //{
                //    //DataRow nRow = SumStoreDetailDT.NewRow();

                //    //for (int j = 0; j < SumOperationsDetailDT.Columns.Count; j++)
                //    //{
                //    //    string ColumnName = SumOperationsDetailDT.Columns[j].ColumnName;
                //    //    if (SumStoreDetailDT.Columns.Contains(ColumnName))
                //    //        nRow[SumOperationsDetailDT.Columns[j].ColumnName] = Row[ColumnName];
                //    //}
                //    //nRow["PrevStoreDetailID"] = PrevStoreDetailID;
                //    //SumStoreDetailDT.Rows.Add(nRow);
                //    return false;
                //}
                DataRow NewRow = SumOperationsDetailDT.NewRow();
                NewRow.ItemArray = Row.ItemArray;
                NewRow["PrevOperationsDetailID"] = PrevOperationsDetailID;
                NewRow["NestedLevel"] = NestedLevel;
                NewRow["Done"] = true;
                SumOperationsDetailDT.Rows.Add(NewRow);
            }
            else
            {
                NestedLevel++;
                int iNestedLevel = NestedLevel;
                foreach (DataRow item in rows)
                {
                    int StoreDetailID = Convert.ToInt32(item["TechCatalogStoreDetailID"]);
                    
                    if (!CheckStoreDetailConditions(StoreDetailID, CoverID, PatinaID))
                        continue;

                    DataRow NewRow = SumStoreDetailDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    NewRow["NestedLevel"] = iNestedLevel - 1;
                    NewRow["PrevStoreDetailID"] = PrevStoreDetailID;
                    SumStoreDetailDT.Rows.Add(NewRow);
                    GetOperationsDetail(PrevOperationsDetailID, StoreDetailID, iNestedLevel, item);
                }

                //foreach (DataRow item in rows)
                //{
                //    GetOperationsDetail(PrevTechCatalogOperationsDetailID, iNestedLevel, item);
                //}
            }
            NestedLevel--;

            return rows.Count() > 0;
        }

        private bool CheckConditions(int TechCatalogOperationsDetailID, int CoverID, int PatinaID, int Length, int Height, int Width)
        {
            DataRow[] rows = OperationsTermsDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
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
                    case "ColorID":
                        break;
                    case "InsetTypeID":
                        //if (row["MathSymbol"].ToString().Equals("="))
                        //{
                        //    if (InsetTypeID == Term)
                        //    { }
                        //    else
                        //        return false;
                        //}
                        //if (row["MathSymbol"].ToString().Equals("!="))
                        //{
                        //    if (InsetTypeID != Term)
                        //    { }
                        //    else
                        //        return false;
                        //}
                        //if (row["MathSymbol"].ToString().Equals(">="))
                        //{
                        //    if (InsetTypeID >= Term)
                        //    { }
                        //    else
                        //        return false;
                        //}
                        //if (row["MathSymbol"].ToString().Equals("<="))
                        //{
                        //    if (InsetTypeID <= Term)
                        //    { }
                        //    else
                        //        return false;
                        //}
                        break;
                    case "InsetColorID":
                        break;
                    case "CoverID":
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (CoverID == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (CoverID != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (CoverID >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (CoverID <= Term)
                            { }
                            else
                                return false;
                        }
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
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (Length == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (Length != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (Length >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (Length <= Term)
                            { }
                            else
                                return false;
                        }
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

        private bool CheckStoreDetailConditions(int TechCatalogStoreDetailID, int CoverID, int PatinaID)
        {
            DataRow[] rows = StoreDetailTermsDT.Select("TechCatalogStoreDetailID=" + TechCatalogStoreDetailID);
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
                        if (row["MathSymbol"].ToString().Equals("="))
                        {
                            if (CoverID == Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("!="))
                        {
                            if (CoverID != Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals(">="))
                        {
                            if (CoverID >= Term)
                            { }
                            else
                                return false;
                        }
                        if (row["MathSymbol"].ToString().Equals("<="))
                        {
                            if (CoverID <= Term)
                            { }
                            else
                                return false;
                        }
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
                }
            }
            return true;
        }

    }


    public class ReportToExcel
    {
        int pos01 = 0;
        int pos02 = 0;
        int pos03 = 0;
        int pos04 = 0;
        int pos05 = 0;
        int pos06 = 0;
        int pos07 = 0;
        int pos08 = 0;
        int pos09 = 0;
        int pos10 = 0;
        int pos11 = 0;
        int pos12 = 0;

        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet01;
        HSSFSheet sheet02;
        HSSFSheet sheet03;
        HSSFSheet sheet04;
        HSSFSheet sheet05;
        HSSFSheet sheet06;
        HSSFSheet sheet07;
        HSSFSheet sheet08;
        HSSFSheet sheet09;
        HSSFSheet sheet10;
        HSSFSheet sheet11;
        HSSFSheet sheet12;

        HSSFFont fConfirm;
        HSSFFont fHeader;
        HSSFFont fColumnName;
        HSSFFont fMainContent;
        HSSFFont fTotalInfo;
        HSSFFont fSignatures;

        HSSFCellStyle csConfirm;
        HSSFCellStyle csHeader;
        HSSFCellStyle csColumnName;
        HSSFCellStyle csMainContent;
        HSSFCellStyle csMainContentDec;
        HSSFCellStyle csMainContentWrap;
        HSSFCellStyle csTotalInfo;
        HSSFCellStyle csTotalValue;
        HSSFCellStyle csTotalValueDec;
        HSSFCellStyle csSignatures;

        public ReportToExcel()
        {
            hssfworkbook = new HSSFWorkbook();

            sheet01 = hssfworkbook.CreateSheet("1");
            sheet01.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet01.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet01.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet01.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet01.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet01.SetColumnWidth(0, 40 * 256);
            sheet01.SetColumnWidth(1, 15 * 256);
            sheet01.SetColumnWidth(2, 15 * 256);
            sheet01.SetColumnWidth(3, 8 * 256);
            sheet01.SetColumnWidth(4, 8 * 256);
            sheet01.SetColumnWidth(5, 8 * 256);
            sheet01.SetColumnWidth(6, 8 * 256);

            sheet02 = hssfworkbook.CreateSheet("2");
            sheet02.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet02.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet02.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet02.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet02.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet02.SetColumnWidth(0, 15 * 256);
            sheet02.SetColumnWidth(1, 25 * 256);
            sheet02.SetColumnWidth(2, 25 * 256);
            sheet02.SetColumnWidth(3, 8 * 256);
            sheet02.SetColumnWidth(4, 8 * 256);
            sheet02.SetColumnWidth(5, 8 * 256);
            sheet02.SetColumnWidth(6, 8 * 256);
            sheet02.SetColumnWidth(7, 10 * 256);

            sheet03 = hssfworkbook.CreateSheet("3");
            sheet03.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet03.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet03.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet03.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet03.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet03.SetColumnWidth(0, 41 * 256);
            sheet03.SetColumnWidth(1, 15 * 256);
            sheet03.SetColumnWidth(2, 8 * 256);
            sheet03.SetColumnWidth(3, 8 * 256);

            sheet04 = hssfworkbook.CreateSheet("4");
            sheet04.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet04.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet04.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet04.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet04.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet04.SetColumnWidth(0, 41 * 256);
            sheet04.SetColumnWidth(1, 15 * 256);
            sheet04.SetColumnWidth(2, 8 * 256);
            sheet04.SetColumnWidth(3, 8 * 256);
            sheet04.SetColumnWidth(4, 8 * 256);
            sheet04.SetColumnWidth(5, 20 * 256);

            sheet05 = hssfworkbook.CreateSheet("5");
            sheet05.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet05.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet05.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet05.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet05.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet05.SetColumnWidth(0, 35 * 256);
            sheet05.SetColumnWidth(1, 18 * 256);
            sheet05.SetColumnWidth(2, 25 * 256);
            sheet05.SetColumnWidth(3, 8 * 256);
            sheet05.SetColumnWidth(4, 8 * 256);
            sheet05.SetColumnWidth(5, 8 * 256);
            sheet05.SetColumnWidth(6, 10 * 256);

            sheet06 = hssfworkbook.CreateSheet("6");
            sheet06.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet06.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet06.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet06.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet06.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet06.SetColumnWidth(0, 30 * 256);
            sheet06.SetColumnWidth(1, 8 * 256);
            sheet06.SetColumnWidth(2, 8 * 256);
            sheet06.SetColumnWidth(3, 8 * 256);
            sheet06.SetColumnWidth(4, 25 * 256);
            sheet06.SetColumnWidth(5, 20 * 256);
            sheet06.SetColumnWidth(6, 30 * 256);
            sheet06.SetColumnWidth(7, 10 * 256);
            sheet06.SetColumnWidth(8, 10 * 256);

            sheet07 = hssfworkbook.CreateSheet("7");
            sheet07.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet07.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet07.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet07.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet07.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet07.SetColumnWidth(0, 41 * 256);
            sheet07.SetColumnWidth(1, 8 * 256);
            sheet07.SetColumnWidth(2, 8 * 256);
            sheet07.SetColumnWidth(3, 8 * 256);

            sheet08 = hssfworkbook.CreateSheet("8");
            sheet08.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet08.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet08.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet08.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet08.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet08.SetColumnWidth(0, 41 * 256);
            sheet08.SetColumnWidth(1, 18 * 256);
            sheet08.SetColumnWidth(2, 13 * 256);
            sheet08.SetColumnWidth(3, 14 * 256);

            sheet09 = hssfworkbook.CreateSheet("9");
            sheet09.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet09.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet09.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet09.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet09.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet09.SetColumnWidth(0, 41 * 256);
            sheet09.SetColumnWidth(1, 18 * 256);
            sheet09.SetColumnWidth(2, 13 * 256);
            sheet09.SetColumnWidth(3, 8 * 256);
            sheet09.SetColumnWidth(4, 8 * 256);
            sheet09.SetColumnWidth(5, 8 * 256);
            sheet09.SetColumnWidth(6, 10 * 256);

            sheet10 = hssfworkbook.CreateSheet("10");
            sheet10.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet10.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet10.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet10.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet10.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet10.SetColumnWidth(0, 41 * 256);
            sheet10.SetColumnWidth(1, 18 * 256);
            sheet10.SetColumnWidth(2, 13 * 256);
            sheet10.SetColumnWidth(3, 14 * 256);

            sheet11 = hssfworkbook.CreateSheet("11");
            sheet11.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet11.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet11.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet11.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet11.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet11.SetColumnWidth(0, 41 * 256);
            sheet11.SetColumnWidth(1, 18 * 256);
            sheet11.SetColumnWidth(2, 8 * 256);
            sheet11.SetColumnWidth(3, 8 * 256);
            sheet11.SetColumnWidth(4, 20 * 256);
            
            sheet12 = hssfworkbook.CreateSheet("12");
            sheet12.PrintSetup.PaperSize = (short)PaperSizeType.A4;
            sheet12.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet12.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet12.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet12.SetMargin(HSSFSheet.BottomMargin, (double).20);
            sheet12.SetColumnWidth(0, 41 * 256);
            sheet12.SetColumnWidth(1, 18 * 256);
            sheet12.SetColumnWidth(2, 13 * 256);
            sheet12.SetColumnWidth(3, 24 * 256);

            CreateFonts();
            CreateCellStyles();
        }

        private void CreateFonts()
        {
            fConfirm = hssfworkbook.CreateFont();
            fConfirm.FontHeightInPoints = 12;
            fConfirm.FontName = "Calibri";

            fHeader = hssfworkbook.CreateFont();
            fHeader.FontHeightInPoints = 12;
            fHeader.Boldweight = 12 * 256;
            fHeader.FontName = "Calibri";

            fColumnName = hssfworkbook.CreateFont();
            fColumnName.FontHeightInPoints = 12;
            fColumnName.FontName = "Calibri";

            fMainContent = hssfworkbook.CreateFont();
            fMainContent.FontHeightInPoints = 11;
            fMainContent.FontName = "Calibri";

            fTotalInfo = hssfworkbook.CreateFont();
            fTotalInfo.FontHeightInPoints = 12;
            fTotalInfo.Boldweight = 12 * 256;
            fTotalInfo.FontName = "Calibri";

            fSignatures = hssfworkbook.CreateFont();
            fSignatures.FontHeightInPoints = 12;
            fSignatures.Boldweight = 12 * 256;
            fSignatures.IsItalic = true;
            fSignatures.FontName = "Calibri";
        }

        private void CreateCellStyles()
        {
            csConfirm = hssfworkbook.CreateCellStyle();
            csConfirm.SetFont(fConfirm);

            csHeader = hssfworkbook.CreateCellStyle();
            csHeader.SetFont(fHeader);

            csColumnName = hssfworkbook.CreateCellStyle();
            csColumnName.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csColumnName.BottomBorderColor = HSSFColor.BLACK.index;
            csColumnName.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csColumnName.LeftBorderColor = HSSFColor.BLACK.index;
            csColumnName.BorderRight = HSSFCellStyle.BORDER_THIN;
            csColumnName.RightBorderColor = HSSFColor.BLACK.index;
            csColumnName.BorderTop = HSSFCellStyle.BORDER_THIN;
            csColumnName.TopBorderColor = HSSFColor.BLACK.index;
            csColumnName.Alignment = HSSFCellStyle.ALIGN_LEFT;
            csColumnName.WrapText = true;
            csColumnName.SetFont(fColumnName);

            csMainContent = hssfworkbook.CreateCellStyle();
            csMainContent.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContent.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContent.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContent.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContent.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContent.RightBorderColor = HSSFColor.BLACK.index;
            csMainContent.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContent.TopBorderColor = HSSFColor.BLACK.index;
            csMainContent.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContent.SetFont(fMainContent);

            csMainContentDec = hssfworkbook.CreateCellStyle();
            csMainContentDec.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.RightBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContentDec.TopBorderColor = HSSFColor.BLACK.index;
            csMainContentDec.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContentDec.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.000");
            csMainContentDec.SetFont(fMainContent);

            csMainContentWrap = hssfworkbook.CreateCellStyle();
            csMainContentWrap.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.BottomBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.LeftBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.BorderRight = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.RightBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.BorderTop = HSSFCellStyle.BORDER_THIN;
            csMainContentWrap.TopBorderColor = HSSFColor.BLACK.index;
            csMainContentWrap.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            csMainContentWrap.WrapText = true;
            csMainContentWrap.SetFont(fMainContent);

            csTotalInfo = hssfworkbook.CreateCellStyle();
            csTotalInfo.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csTotalInfo.BottomBorderColor = HSSFColor.BLACK.index;
            csTotalInfo.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csTotalInfo.LeftBorderColor = HSSFColor.BLACK.index;
            csTotalInfo.BorderRight = HSSFCellStyle.BORDER_THIN;
            csTotalInfo.RightBorderColor = HSSFColor.BLACK.index;
            csTotalInfo.BorderTop = HSSFCellStyle.BORDER_THIN;
            csTotalInfo.TopBorderColor = HSSFColor.BLACK.index;
            csTotalInfo.Alignment = HSSFCellStyle.ALIGN_LEFT;
            csTotalInfo.SetFont(fTotalInfo);

            csTotalValue = hssfworkbook.CreateCellStyle();
            csTotalValue.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csTotalValue.BottomBorderColor = HSSFColor.BLACK.index;
            csTotalValue.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csTotalValue.LeftBorderColor = HSSFColor.BLACK.index;
            csTotalValue.BorderRight = HSSFCellStyle.BORDER_THIN;
            csTotalValue.RightBorderColor = HSSFColor.BLACK.index;
            csTotalValue.BorderTop = HSSFCellStyle.BORDER_THIN;
            csTotalValue.TopBorderColor = HSSFColor.BLACK.index;
            csTotalValue.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            csTotalValue.SetFont(fTotalInfo);

            csTotalValueDec = hssfworkbook.CreateCellStyle();
            csTotalValueDec.BorderBottom = HSSFCellStyle.BORDER_THIN;
            csTotalValueDec.BottomBorderColor = HSSFColor.BLACK.index;
            csTotalValueDec.BorderLeft = HSSFCellStyle.BORDER_THIN;
            csTotalValueDec.LeftBorderColor = HSSFColor.BLACK.index;
            csTotalValueDec.BorderRight = HSSFCellStyle.BORDER_THIN;
            csTotalValueDec.RightBorderColor = HSSFColor.BLACK.index;
            csTotalValueDec.BorderTop = HSSFCellStyle.BORDER_THIN;
            csTotalValueDec.TopBorderColor = HSSFColor.BLACK.index;
            csTotalValueDec.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            csTotalValueDec.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.000");
            csTotalValueDec.SetFont(fTotalInfo);

            csSignatures = hssfworkbook.CreateCellStyle();
            csSignatures.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            csSignatures.SetFont(fSignatures);
        }
        
        public void ClearReport()
        {

        }
        
        public void Form01(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table1, DataTable table2)
        {
            HSSFCell Cell1;
            int DisplayIndex = 0;
            {
                //header таблицы
                Cell1 = sheet01.CreateRow(pos01++).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet01.CreateRow(pos01++).CreateCell(1);
                Cell1.SetCellValue("Материал");
                Cell1.CellStyle = csHeader;
                
                Cell1 = sheet01.CreateRow(pos01++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                //Названия столбцов
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Патина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Длина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                pos01++;

                //total
                double TotalCount = 0;
                //Содержимое таблицы
                for (int i = 0; i < table1.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table1.Rows[i]["Name"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table1.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table1.Rows[i]["Patina"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table1.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table1.Rows[i]["Length"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table1.Rows[i]["Length"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table1.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table1.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table1.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table1.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table1.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table1.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table1.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table1.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table1.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    pos01++;
                }

                for (int i = 0; i < table1.Columns.Count - 1; i++)
                {
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }


                Cell1 = sheet01.CreateRow(pos01).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(6);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                pos01++;
            }

            pos01++;
            pos01++;
            DisplayIndex = 0;
            {
                //header таблицы
                Cell1 = sheet01.CreateRow(pos01++).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet01.CreateRow(pos01++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet01.CreateRow(pos01++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet01.CreateRow(pos01++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet01.CreateRow(pos01++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                //Названия столбцов
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наименование");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Патина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Длина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos01++;

                //total
                double TotalCount = 0;
                //Содержимое таблицы
                for (int i = 0; i < table2.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table2.Rows[i]["Name"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table2.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table2.Rows[i]["Patina"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table2.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table2.Rows[i]["Length"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table2.Rows[i]["Length"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table2.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table2.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table2.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table2.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table2.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table2.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    if (table2.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table2.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table2.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(DisplayIndex++);
                    Cell1.CellStyle = csMainContent;
                    pos01++;
                }

                for (int i = 0; i < table2.Columns.Count; i++)
                {
                    Cell1 = sheet01.CreateRow(pos01).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet01.CreateRow(pos01).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet01.CreateRow(pos01).CreateCell(6);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                pos01++;
                pos01++;

                ////footer таблицы
                //double Plan = 0;

                //Cell1 = sheet01.CreateRow(pos01).CreateCell(5);
                //Cell1.SetCellValue(Plan);
                //Cell1.CellStyle = csSignatures;
                //Cell1 = sheet01.CreateRow(pos01++).CreateCell(6);
                //Cell1.SetCellValue("ч/ч план.");
                //Cell1.CellStyle = csSignatures;
                //pos01++;
                //Cell1 = sheet01.CreateRow(pos01++).CreateCell(6);
                //Cell1.SetCellValue("__________________ч/ч факт.");
                //Cell1.CellStyle = csSignatures;
                //pos01++;

                //Cell1 = sheet01.CreateRow(pos01++).CreateCell(6);
                //Cell1.SetCellValue("след. станок");
                //Cell1.CellStyle = csSignatures;
            }

            pos01++;
            pos01++;
        }

        public void Form02(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;
            {
                //header таблицы
                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;
                //Названия столбцов
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Чертеж");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Длина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                pos02++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    pos02++;
                }
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet02.CreateRow(pos02).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(7);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos02++;
                pos02++;

                ////footer таблицы
                //double Plan = 0;

                ////Cell1 = sheet2.CreateRow(pos2).CreateCell(7);
                ////Cell1.SetCellValue(Plan);
                ////Cell1.CellStyle = csSignatures;
                //Cell1 = sheet02.CreateRow(pos02++).CreateCell(5);
                //Cell1.SetCellValue("ч/ч план.");
                //Cell1.CellStyle = csSignatures;
                //pos02++;
                //Cell1 = sheet02.CreateRow(pos02++).CreateCell(5);
                //Cell1.SetCellValue("__________________ч/ч факт.");
                //Cell1.CellStyle = csSignatures;
                //pos02++;

                //Cell1 = sheet02.CreateRow(pos02++).CreateCell(5);
                //Cell1.SetCellValue("след. станок");
                //Cell1.CellStyle = csSignatures;
            }

            pos02++;
            pos02++;
            {
                //header таблицы
                Cell1 = sheet02.CreateRow(pos02).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(6);
                Cell1.SetCellValue("ДУБЛЬ");
                Cell1.CellStyle = csSignatures;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet02.CreateRow(pos02++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;
                //Названия столбцов
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Чертеж");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Длина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                pos02++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    pos02++;
                }
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet02.CreateRow(pos02).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet02.CreateRow(pos02).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet02.CreateRow(pos02).CreateCell(7);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos02++;
                pos02++;

                //footer таблицы
                //double Plan = 0;

                //Cell1 = sheet2.CreateRow(pos2).CreateCell(7);
                //Cell1.SetCellValue(Plan);
                //Cell1.CellStyle = csSignatures;
                //Cell1 = sheet02.CreateRow(pos02++).CreateCell(5);
                //Cell1.SetCellValue("ч/ч план.");
                //Cell1.CellStyle = csSignatures;
                //pos02++;
                //Cell1 = sheet02.CreateRow(pos02++).CreateCell(5);
                //Cell1.SetCellValue("__________________ч/ч факт.");
                //Cell1.CellStyle = csSignatures;
                //pos02++;

                //Cell1 = sheet02.CreateRow(pos02++).CreateCell(5);
                //Cell1.SetCellValue("след. станок");
                //Cell1.CellStyle = csSignatures;
            }
            pos02++;
            pos02++;
        }

        public void Form03(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet03.CreateRow(pos03++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet03.CreateRow(pos03++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet03.CreateRow(pos03++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet03.CreateRow(pos03++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet03.CreateRow(pos03++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Облицовка");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Длина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Работник");
            Cell1.CellStyle = csMainContent;
            pos03++;

            //total
            double TotalCount = 0;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["NameM"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet03.CreateRow(pos03).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                Cell1.CellStyle = csMainContent;
                pos03++;
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet03.CreateRow(pos03).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }


            Cell1 = sheet03.CreateRow(pos03).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet03.CreateRow(pos03).CreateCell(3);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            pos03++;
            pos03++;

            ////footer таблицы
            //double Plan1 = 0;
            //double Plan2 = 0;

            //Cell1 = sheet03.CreateRow(pos03).CreateCell(3);
            //Cell1.SetCellValue(Plan1);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet03.CreateRow(pos03++).CreateCell(4);
            //Cell1.SetCellValue("расход (м.п) план.");
            //Cell1.CellStyle = csSignatures;
            //pos03++;
            //Cell1 = sheet03.CreateRow(pos03++).CreateCell(4);
            //Cell1.SetCellValue("__________________расход (м.п) факт.");
            //Cell1.CellStyle = csSignatures;
            //pos03++;

            //Cell1 = sheet03.CreateRow(pos03).CreateCell(3);
            //Cell1.SetCellValue(Plan2);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet03.CreateRow(pos03++).CreateCell(4);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos03++;
            //Cell1 = sheet03.CreateRow(pos03++).CreateCell(4);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos03++;

            //Cell1 = sheet03.CreateRow(pos03++).CreateCell(4);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos03++;
            //pos03++;
        }

        public void Form04(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet04.CreateRow(pos04++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet04.CreateRow(pos04++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet04.CreateRow(pos04++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet04.CreateRow(pos04++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet04.CreateRow(pos04++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Деталь");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Облицовка");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Длина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Ширина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Чертеж");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Работник");
            Cell1.CellStyle = csMainContent;
            pos04++;

            //total
            double TotalCount = 0;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["NotesM"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["NameM"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet04.CreateRow(pos04).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                Cell1.CellStyle = csMainContent;
                pos04++;
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet04.CreateRow(pos04).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }


            Cell1 = sheet04.CreateRow(pos04).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet04.CreateRow(pos04).CreateCell(4);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            pos04++;
            pos04++;

            ////footer таблицы
            //double Plan1 = 0;

            //Cell1 = sheet04.CreateRow(pos04).CreateCell(4);
            //Cell1.SetCellValue(Plan1);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet04.CreateRow(pos04++).CreateCell(5);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos04++;
            //Cell1 = sheet04.CreateRow(pos04++).CreateCell(5);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos04++;

            //Cell1 = sheet04.CreateRow(pos04++).CreateCell(5);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos04++;
            //pos04++;
        }

        public void Form05(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;
            {
                //header таблицы
                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;
                //Названия столбцов
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наполнение");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos05++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesMNameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Filling"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    pos05++;
                }
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }


                Cell1 = sheet05.CreateRow(pos05).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(6);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos05++;
                pos05++;
            }
            pos05++;
            pos05++;
            {
                //header таблицы
                Cell1 = sheet05.CreateRow(pos05).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(6);
                Cell1.SetCellValue("ДУБЛЬ");
                Cell1.CellStyle = csSignatures;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet05.CreateRow(pos05++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;
                //Названия столбцов
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Наполнение");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos05++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesMNameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Filling"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    pos05++;
                }
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet05.CreateRow(pos05).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }


                Cell1 = sheet05.CreateRow(pos05).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet05.CreateRow(pos05).CreateCell(6);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos05++;
                pos05++;
            }
            ////footer таблицы
            //double Plan = 0;

            //Cell1 = sheet05.CreateRow(pos05).CreateCell(6);
            //Cell1.SetCellValue(Plan);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet05.CreateRow(pos05++).CreateCell(6);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos05++;
            //Cell1 = sheet05.CreateRow(pos05++).CreateCell(6);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos05++;

            //Cell1 = sheet05.CreateRow(pos05++).CreateCell(6);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos05++;
            //pos05++;
        }

        public void Form06(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet06.CreateRow(pos06++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet06.CreateRow(pos06++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;
            
            Cell1 = sheet06.CreateRow(pos06++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet06.CreateRow(pos06++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet06.CreateRow(pos06++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Цвет ЛДСтП");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Длина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Ширина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Примечание");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Вид Кромки");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("По Размеру");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("м.п.");
            Cell1.CellStyle = csMainContent;
            pos06++;

            //total
            double TotalCount = 0;
            double TotalSquare = 0;

            double TotalEqualCount = 0;
            bool EqualRow = false;

            int FirstRowIndex = 0;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (i > 0)
                {
                    if (table.Rows[i]["NameMCover"].ToString().Equals(table.Rows[i - 1]["NameMCover"].ToString())
                        && table.Rows[i]["Notes1"].ToString().Equals(table.Rows[i - 1]["Notes1"].ToString())
                        && table.Rows[i]["Name1"].ToString().Equals(table.Rows[i - 1]["Name1"].ToString())
                        && Convert.ToInt32(table.Rows[i]["Length"]) == Convert.ToInt32(table.Rows[i - 1]["Length"])
                        && Convert.ToInt32(table.Rows[i]["Width"]) == Convert.ToInt32(table.Rows[i - 1]["Width"]))
                        EqualRow = true;
                    else
                        EqualRow = false;
                }
                else
                {
                    FirstRowIndex = pos06;
                    EqualRow = false;
                }

                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    if (i == 0)
                        TotalEqualCount = Convert.ToDouble(table.Rows[i]["Count"]);
                    else
                    {
                        if (EqualRow && table.Rows.Count - 1 != i)
                            TotalEqualCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        else
                        {
                            Cell1 = sheet06.CreateRow(FirstRowIndex).CreateCell(3);
                            Cell1.SetCellValue(TotalEqualCount);
                            Cell1.CellStyle = csMainContent;
                            TotalEqualCount = Convert.ToDouble(table.Rows[i]["Count"]);
                            FirstRowIndex = pos06;
                        }
                    }
                }

                DisplayIndex = 0;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (!EqualRow)
                    Cell1.SetCellValue(table.Rows[i]["NameMCover"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1 && !EqualRow)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1 && !EqualRow)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value && !EqualRow)
                {
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (!EqualRow)
                    Cell1.SetCellValue(table.Rows[i]["Notes1"].ToString());
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (!EqualRow)
                    Cell1.SetCellValue(table.Rows[i]["Name1"].ToString());
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["NameMCover1"].ToString());
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length1"] != DBNull.Value)
                {
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length1"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet06.CreateRow(pos06).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Square"] != DBNull.Value)
                {
                    TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                }
                Cell1.CellStyle = csMainContentDec;
                pos06++;
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet06.CreateRow(pos06).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }


            Cell1 = sheet06.CreateRow(pos06).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(3);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            Cell1 = sheet06.CreateRow(pos06).CreateCell(8);
            Cell1.SetCellValue(TotalSquare);
            Cell1.CellStyle = csTotalValueDec;
            pos06++;
            pos06++;

            ////footer таблицы
            //double Plan1 = 0;
            //double Plan2 = 0;

            //Cell1 = sheet06.CreateRow(pos06).CreateCell(8);
            //Cell1.SetCellValue(Plan2);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet06.CreateRow(pos06++).CreateCell(9);
            //Cell1.SetCellValue("расход кромки (м.кв.) план.");
            //Cell1.CellStyle = csSignatures;
            //pos06++;
            //Cell1 = sheet06.CreateRow(pos06++).CreateCell(9);
            //Cell1.SetCellValue("__________________расход кромки (м.кв.) факт.");
            //Cell1.CellStyle = csSignatures;
            //pos06++;

            //Cell1 = sheet06.CreateRow(pos06).CreateCell(8);
            //Cell1.SetCellValue(Plan1);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet06.CreateRow(pos06++).CreateCell(9);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos06++;
            //Cell1 = sheet06.CreateRow(pos06++).CreateCell(9);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos06++;

            //Cell1 = sheet06.CreateRow(pos06++).CreateCell(9);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos06++;
            //pos06++;
        }

        public void Form07(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;
            {
                //header таблицы
                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                //Названия столбцов
                Cell1 = sheet07.CreateRow(pos07++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;

                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos07++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesMNameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    pos07++;
                }

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet07.CreateRow(pos07).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(7);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos07++;
                pos07++;
            }
            //pos02++;
            //pos02++;
            {
                //header таблицы
                Cell1 = sheet07.CreateRow(pos07).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(6);
                Cell1.SetCellValue("ДУБЛЬ");
                Cell1.CellStyle = csSignatures;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet07.CreateRow(pos07++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                //Названия столбцов
                Cell1 = sheet07.CreateRow(pos07++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;

                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos07++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesMNameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    pos07++;
                }

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet07.CreateRow(pos07).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet07.CreateRow(pos07).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet07.CreateRow(pos07).CreateCell(6);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos07++;
                pos07++;
            }
        }

        public void Form08(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet08.CreateRow(pos08++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet08.CreateRow(pos08++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet08.CreateRow(pos08++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;
            
            Cell1 = sheet08.CreateRow(pos08++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;
            
            Cell1 = sheet08.CreateRow(pos08++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Длина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Ширина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Работник");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("м.кв.");
            Cell1.CellStyle = csMainContent;
            pos08++;

            //total
            double TotalCount = 0;
            double TotalSquare = 0;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["NameM"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet08.CreateRow(pos08).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Square"] != DBNull.Value)
                {
                    TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                }
                Cell1.CellStyle = csMainContentDec;
                pos08++;
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet08.CreateRow(pos08).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }


            Cell1 = sheet08.CreateRow(pos08).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(3);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            Cell1 = sheet08.CreateRow(pos08).CreateCell(5);
            Cell1.SetCellValue(TotalSquare);
            Cell1.CellStyle = csTotalValueDec;
            pos08++;
            pos08++;

            ////footer таблицы
            //double Plan1 = 0;
            //double Plan2 = 0;

            //Cell1 = sheet08.CreateRow(pos08).CreateCell(4);
            //Cell1.SetCellValue(Plan2);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet08.CreateRow(pos08++).CreateCell(5);
            //Cell1.SetCellValue("расход (м.кв.) план.");
            //Cell1.CellStyle = csSignatures;
            //pos08++;
            //Cell1 = sheet08.CreateRow(pos08++).CreateCell(5);
            //Cell1.SetCellValue("__________________расход (м.кв.) факт.");
            //Cell1.CellStyle = csSignatures;
            //pos08++;

            //Cell1 = sheet08.CreateRow(pos08).CreateCell(4);
            //Cell1.SetCellValue(Plan1);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet08.CreateRow(pos08++).CreateCell(5);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos08++;
            //Cell1 = sheet08.CreateRow(pos08++).CreateCell(5);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos08++;

            //Cell1 = sheet08.CreateRow(pos08++).CreateCell(5);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos08++;
            //pos08++;
        }

        public void Form09(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;
            {
                //header таблицы
                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                //Названия столбцов
                Cell1 = sheet09.CreateRow(pos09++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;

                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Патина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos09++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesMNameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Patina"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    pos09++;
                }

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet09.CreateRow(pos09).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(6);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos09++;
                pos09++;
            }
            //pos02++;
            //pos02++;
            {
                //header таблицы
                Cell1 = sheet09.CreateRow(pos09).CreateCell(1);
                Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
                Cell1.CellStyle = csConfirm;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(6);
                Cell1.SetCellValue("ДУБЛЬ");
                Cell1.CellStyle = csSignatures;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue(MachineName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue(OperationName);
                Cell1.CellStyle = csHeader;

                Cell1 = sheet09.CreateRow(pos09++).CreateCell(1);
                Cell1.SetCellValue(CupboardsName);
                Cell1.CellStyle = csHeader;

                //Названия столбцов
                Cell1 = sheet09.CreateRow(pos09++).CreateCell(0);
                Cell1.SetCellValue(JobNumber);
                Cell1.CellStyle = csHeader;

                int DisplayIndex = 0;

                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Деталь");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Облицовка");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Патина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Высота");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Ширина");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Кол-во");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("м.кв.");
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                Cell1.SetCellValue("Работник");
                Cell1.CellStyle = csMainContent;
                pos09++;

                //total
                double TotalCount = 0;
                double TotalSquare = 0;
                //Содержимое таблицы
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["NotesMNameM"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Patina"].ToString());
                    Cell1.CellStyle = csMainContentWrap;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                        Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Count"] != DBNull.Value)
                    {
                        TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                    }
                    Cell1.CellStyle = csMainContent;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    if (table.Rows[i]["Square"] != DBNull.Value)
                    {
                        TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                        Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                    }
                    Cell1.CellStyle = csMainContentDec;
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                    Cell1.CellStyle = csMainContent;
                    pos09++;
                }

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Cell1 = sheet09.CreateRow(pos09).CreateCell(i);
                    Cell1.CellStyle = csMainContent;
                }

                Cell1 = sheet09.CreateRow(pos09).CreateCell(0);
                Cell1.SetCellValue("Всего");
                Cell1.CellStyle = csTotalInfo;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(5);
                Cell1.SetCellValue(TotalCount);
                Cell1.CellStyle = csTotalValue;
                Cell1 = sheet09.CreateRow(pos09).CreateCell(6);
                Cell1.SetCellValue(TotalSquare);
                Cell1.CellStyle = csTotalValueDec;
                pos09++;
                pos09++;
            }
        }

        public void Form10(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet10.CreateRow(pos10++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet10.CreateRow(pos10++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet10.CreateRow(pos10++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;
            
            Cell1 = sheet10.CreateRow(pos10++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;

            //Названия столбцов
            Cell1 = sheet10.CreateRow(pos10++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;

            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Облицовка");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Деталь");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Длина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Ширина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Работник");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("м.кв.");
            Cell1.CellStyle = csMainContent;
            pos10++;

            //total
            double TotalCount = 0;
            double TotalSquare = 0;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["NameM"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Notes"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet10.CreateRow(pos10).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Square"] != DBNull.Value)
                {
                    TotalSquare += Convert.ToDouble(table.Rows[i]["Square"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Square"]));
                }
                Cell1.CellStyle = csMainContentDec;
                pos10++;
            }

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet10.CreateRow(pos10).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }

            Cell1 = sheet10.CreateRow(pos10).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(5);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            Cell1 = sheet10.CreateRow(pos10).CreateCell(7);
            Cell1.SetCellValue(TotalSquare);
            Cell1.CellStyle = csTotalValueDec;
            pos10++;
            pos10++;

            ////footer таблицы
            //double Plan1 = 0;
            //double Plan2 = 0;

            //Cell1 = sheet10.CreateRow(pos10).CreateCell(4);
            //Cell1.SetCellValue(Plan2);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet10.CreateRow(pos10++).CreateCell(5);
            //Cell1.SetCellValue("расход (м.кв.) план.");
            //Cell1.CellStyle = csSignatures;
            //pos10++;
            //Cell1 = sheet10.CreateRow(pos10++).CreateCell(5);
            //Cell1.SetCellValue("__________________расход (м.кв.) факт.");
            //Cell1.CellStyle = csSignatures;
            //pos10++;

            //Cell1 = sheet10.CreateRow(pos10).CreateCell(4);
            //Cell1.SetCellValue(Plan1);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet10.CreateRow(pos10++).CreateCell(5);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos10++;
            //Cell1 = sheet10.CreateRow(pos10++).CreateCell(5);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos10++;

            //Cell1 = sheet10.CreateRow(pos10++).CreateCell(5);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos10++;
            //pos10++;
        }
        
        public void Form11(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet11.CreateRow(pos11++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet11.CreateRow(pos11++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet11.CreateRow(pos11++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet11.CreateRow(pos11++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet11.CreateRow(pos11++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Облицовка");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Длина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Чертеж");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Работник");
            Cell1.CellStyle = csMainContent;
            pos11++;

            //total
            double TotalCount = 0;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Notes"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Length"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Length"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Length"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Name"].ToString());
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet11.CreateRow(pos11).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                Cell1.CellStyle = csMainContent;
                pos11++;
            }

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet11.CreateRow(pos11).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }

            Cell1 = sheet11.CreateRow(pos11).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet11.CreateRow(pos11).CreateCell(3);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            pos11++;
            pos11++;

            ////footer таблицы
            //double Plan = 0;

            //Cell1 = sheet12.CreateRow(pos12).CreateCell(4);
            //Cell1.SetCellValue(Plan);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet12.CreateRow(pos12++).CreateCell(5);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos12++;
            //Cell1 = sheet12.CreateRow(pos12++).CreateCell(5);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos12++;

            //Cell1 = sheet12.CreateRow(pos12++).CreateCell(5);
            //Cell1.SetCellValue("след. станок");
            //Cell1.CellStyle = csSignatures;
            //pos12++;
            //pos12++;
        }
        
        public void Form12(string MachineName, string OperationName, string CupboardsName, string JobNumber,
            DataTable table)
        {
            HSSFCell Cell1;

            //header таблицы
            Cell1 = sheet12.CreateRow(pos12++).CreateCell(1);
            Cell1.SetCellValue("УТВЕРЖДАЮ_____________");
            Cell1.CellStyle = csConfirm;

            Cell1 = sheet12.CreateRow(pos12++).CreateCell(1);
            Cell1.SetCellValue(MachineName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet12.CreateRow(pos12++).CreateCell(1);
            Cell1.SetCellValue(OperationName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet12.CreateRow(pos12++).CreateCell(1);
            Cell1.SetCellValue(CupboardsName);
            Cell1.CellStyle = csHeader;

            Cell1 = sheet12.CreateRow(pos12++).CreateCell(0);
            Cell1.SetCellValue(JobNumber);
            Cell1.CellStyle = csHeader;

            int DisplayIndex = 0;
            //Названия столбцов
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Операция");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Облицовка");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Патина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Наименование");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Высота");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Ширина");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кол-во");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("в Уп-ке");
            Cell1.CellStyle = csMainContent;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Работник");
            Cell1.CellStyle = csMainContent;
            pos12++;

            //total
            double TotalCount = 0;
            double TotalPackCount = 0;
            bool EqualRow = false;
            //Содержимое таблицы
            for (int i = 0; i < table.Rows.Count; i++)
            {

                if (i > 0)
                {
                    if (table.Rows[i]["OperationName"].ToString().Equals(table.Rows[i - 1]["OperationName"].ToString()))
                        EqualRow = true;
                    else
                        EqualRow = false;
                }
                else
                {
                    EqualRow = false;
                }

                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                }

                DisplayIndex = 0;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                if (!EqualRow)
                    Cell1.SetCellValue(table.Rows[i]["OperationName"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Cover"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Patina"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Name"].ToString());
                Cell1.CellStyle = csMainContentWrap;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Height"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Height"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Height"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Width"] != DBNull.Value && Convert.ToInt32(table.Rows[i]["Width"]) != -1)
                    Cell1.SetCellValue(Convert.ToInt32(table.Rows[i]["Width"]));
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                if (table.Rows[i]["Count"] != DBNull.Value)
                {
                    TotalCount += Convert.ToDouble(table.Rows[i]["Count"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["Count"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                if (table.Rows[i]["PackCount"] != DBNull.Value)
                {
                    TotalPackCount += Convert.ToDouble(table.Rows[i]["PackCount"]);
                    Cell1.SetCellValue(Convert.ToDouble(table.Rows[i]["PackCount"]));
                }
                Cell1.CellStyle = csMainContent;
                Cell1 = sheet12.CreateRow(pos12).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(table.Rows[i]["Worker"].ToString());
                Cell1.CellStyle = csMainContent;
                pos12++;
            }

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Cell1 = sheet12.CreateRow(pos12).CreateCell(i);
                Cell1.CellStyle = csMainContent;
            }

            Cell1 = sheet12.CreateRow(pos12).CreateCell(0);
            Cell1.SetCellValue("Всего");
            Cell1.CellStyle = csTotalInfo;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(6);
            Cell1.SetCellValue(TotalCount);
            Cell1.CellStyle = csTotalValue;
            Cell1 = sheet12.CreateRow(pos12).CreateCell(7);
            Cell1.SetCellValue(TotalPackCount);
            Cell1.CellStyle = csTotalValue;
            pos12++;
            pos12++;

            ////footer таблицы
            //double Plan1 = 0;

            //Cell1 = sheet14.CreateRow(pos14).CreateCell(5);
            //Cell1.SetCellValue(Plan1);
            //Cell1.CellStyle = csSignatures;
            //Cell1 = sheet14.CreateRow(pos14++).CreateCell(6);
            //Cell1.SetCellValue("ч/ч план.");
            //Cell1.CellStyle = csSignatures;
            //pos14++;
            //Cell1 = sheet14.CreateRow(pos14++).CreateCell(6);
            //Cell1.SetCellValue("__________________ч/ч факт.");
            //Cell1.CellStyle = csSignatures;
            //pos14++;
            //pos14++;
        }

        public void SaveFile(string FileName, bool bOpenFile)
        {
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
            ClearReport();

            if (bOpenFile)
                System.Diagnostics.Process.Start(file.FullName);
        }
    }
    

    public class TestAssignments
    {
        int iPanelCounter = 0;
        int InsetTypeID = 0;
        int PatinaID = 0;
        int Height = 0;
        int Width = 0;

        DataTable DataTable1;
        DataTable ResultDT;
        DataTable SummaryDT;
        DataTable MaterialDT;
        DataTable FixedMaterialDT;

        DataTable FrontsOrdersDT;
        DataTable FrontsConfigDT;
        DataTable DecorConfigDT;
        DataTable TechStore;
        DataTable OperationsTermsDT;

        DataSet OperationsGroupsDS;
        DataSet OperationsDetailDS;
        DataSet StoreDetailDS;

        public BindingSource ResultBS;
        public BindingSource SummaryBS;
        public BindingSource MaterialBS;

        public TestAssignments(int iInsetTypeID, int iPatinaID, int iHeight, int iWidth)
        {
            InsetTypeID = iInsetTypeID;
            PatinaID = iPatinaID;
            Height = iHeight;
            Width = iWidth;
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
            SelectCommand = @"SELECT * FROM TechCatalogOperationsTerms";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                OperationsTermsDT.Clear();
                DA.Fill(OperationsTermsDT);
            }

            OperationsGroupsDS = new DataSet();
            OperationsDetailDS = new DataSet();
            StoreDetailDS = new DataSet();

        }

        private void Create()
        {
            ResultDT = new DataTable();
            ResultDT.Columns.Add(new DataColumn("SerialNumber", Type.GetType("System.Int32")));
            ResultDT.Columns.Add(new DataColumn("GroupName", Type.GetType("System.String")));
            ResultDT.Columns.Add(new DataColumn("TechCatalogOperationsGroupID", Type.GetType("System.Int32")));
            ResultDT.Columns.Add(new DataColumn("PrevTechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            ResultDT.Columns.Add(new DataColumn("TechCatalogOperationsDetailID", Type.GetType("System.Int32")));

            DataTable1 = new DataTable();
            DataTable1.Columns.Add(new DataColumn("TechCatalogOperationsGroupID", Type.GetType("System.Int32")));
            DataTable1.Columns.Add(new DataColumn("PrevTechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            DataTable1.Columns.Add(new DataColumn("TechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            DataTable1.Columns.Add(new DataColumn("TechStoreID", Type.GetType("System.Int32")));
            DataTable1.Columns.Add(new DataColumn("SerialNumber", Type.GetType("System.Int32")));
            DataTable1.Columns.Add(new DataColumn("GroupName", Type.GetType("System.String")));
            DataTable1.Columns.Add(new DataColumn("MachinesOperationName", Type.GetType("System.String")));
            DataTable1.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
            DataTable1.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            DataTable1.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            DataTable1.Columns.Add(new DataColumn("check", Type.GetType("System.Boolean")));

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

            MaterialDT = new DataTable();
            MaterialDT.Columns.Add(new DataColumn("TechCatalogStoreDetailID", Type.GetType("System.Int32")));
            MaterialDT.Columns.Add(new DataColumn("TechCatalogOperationsDetailID", Type.GetType("System.Int32")));
            MaterialDT.Columns.Add(new DataColumn("TechStoreID", Type.GetType("System.Int32")));
            MaterialDT.Columns.Add(new DataColumn("MachinesOperationName", Type.GetType("System.String")));
            MaterialDT.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
            MaterialDT.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));
            MaterialDT.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            MaterialDT.Columns.Add(new DataColumn("PanelCounter", Type.GetType("System.Int32")));
            FixedMaterialDT = MaterialDT.Clone();

            FrontsOrdersDT = new DataTable();
            DecorConfigDT = new DataTable();
            FrontsConfigDT = new DataTable();
            TechStore = new DataTable();
            OperationsTermsDT = new DataTable();

            ResultBS = new BindingSource()
            {
                DataSource = ResultDT
            };
            SummaryBS = new BindingSource()
            {
                DataSource = SummaryDT
            };
            MaterialBS = new BindingSource()
            {
                DataSource = MaterialDT
            };
        }

        public int CurrentPanelCounter
        {
            get { return iPanelCounter; }
            set { iPanelCounter = value; }
        }

        public int CurrentOperationsGroups
        {
            get
            {
                if (OperationsGroupsDS.Tables.Contains("OperationsGroups" + iPanelCounter))
                    return OperationsGroupsDS.Tables["OperationsGroups" + iPanelCounter].DefaultView.Count;
                return 0;
            }
        }

        public int CurrentOperationsDetail
        {
            get 
            {
                if (OperationsDetailDS.Tables.Contains("OperationsDetail" + iPanelCounter))
                    return OperationsDetailDS.Tables["OperationsDetail" + iPanelCounter].DefaultView.Count;
                return 0;
            }
        }

        public int CurrentStoreDetail
        {
            get
            {
                if (StoreDetailDS.Tables.Contains("StoreDetail" + iPanelCounter))
                    return StoreDetailDS.Tables["StoreDetail" + iPanelCounter].DefaultView.Count;
                return 0;
            }
        }

        public void AddMaterial(int TechCatalogStoreDetailID, int TechCatalogOperationsDetailID, int TechStoreID, string MachinesOperationName,
            string TechStoreName, decimal Cost, string Measure, int PanelCounter)
        {
            DataRow[] rows = MaterialDT.Select("TechCatalogStoreDetailID=" + TechCatalogStoreDetailID + " AND PanelCounter=" + PanelCounter);
            if (rows.Count() > 0)
                return;
            DataRow NewRow = MaterialDT.NewRow();
            NewRow["TechCatalogStoreDetailID"] = TechCatalogStoreDetailID;
            NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
            NewRow["TechStoreID"] = TechStoreID;
            NewRow["MachinesOperationName"] = MachinesOperationName;
            NewRow["TechStoreName"] = TechStoreName;
            NewRow["Cost"] = Cost;
            NewRow["Measure"] = Measure;
            NewRow["PanelCounter"] = PanelCounter;
            MaterialDT.Rows.Add(NewRow);
        }

        public void AddFixedMaterial(int TechCatalogStoreDetailID, int TechCatalogOperationsDetailID, int TechStoreID, string MachinesOperationName,
            string TechStoreName, decimal Cost, string Measure, int PanelCounter)
        {
            DataRow[] rows = FixedMaterialDT.Select("TechCatalogStoreDetailID=" + TechCatalogStoreDetailID);
            if (rows.Count() > 0)
                return;
            DataRow NewRow = FixedMaterialDT.NewRow();
            NewRow["TechCatalogStoreDetailID"] = TechCatalogStoreDetailID;
            NewRow["TechCatalogOperationsDetailID"] = TechCatalogOperationsDetailID;
            NewRow["TechStoreID"] = TechStoreID;
            NewRow["MachinesOperationName"] = MachinesOperationName;
            NewRow["TechStoreName"] = TechStoreName;
            NewRow["Cost"] = Cost;
            NewRow["Measure"] = Measure;
            NewRow["PanelCounter"] = PanelCounter;
            FixedMaterialDT.Rows.Add(NewRow);
        }

        public void DeleteMaterial(int PanelCounter)
        {
            DataRow[] rows = MaterialDT.Select("PanelCounter=" + PanelCounter);
            for (int i = rows.Count() - 1; i >= 0; i--)
                rows[i].Delete();
        }

        public void ClearOperationsGroups(int PanelCounter)
        {
            if (OperationsGroupsDS.Tables.Contains("OperationsGroups" + PanelCounter))
                OperationsGroupsDS.Tables["OperationsGroups" + PanelCounter].Clear();
        }

        public void ClearOperations(int PanelCounter)
        {
            if (OperationsDetailDS.Tables.Contains("OperationsDetail" + PanelCounter))
                OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Clear();
        }

        public void ClearStoreDetail(int PanelCounter)
        {
            if (StoreDetailDS.Tables.Contains("StoreDetail" + PanelCounter))
                StoreDetailDS.Tables["StoreDetail" + PanelCounter].Clear();
        }

        public DataView FillOperationsGroups(int TechStoreID, int PanelCounter)
        {
            string SelectCommand = @"SELECT * FROM TechCatalogOperationsGroups WHERE TechStoreID=" + TechStoreID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(OperationsGroupsDS, "OperationsGroups" + PanelCounter);
            }
            for (int i = 0; i < OperationsGroupsDS.Tables["OperationsGroups" + PanelCounter].Rows.Count; i++)
            {
                string GroupName = OperationsGroupsDS.Tables["OperationsGroups" + PanelCounter].Rows[i]["GroupName"].ToString();
                if (GroupName.Length == 0)
                    OperationsGroupsDS.Tables["OperationsGroups" + PanelCounter].Rows[i]["GroupName"] = "Нет названия группы";
            }
            return OperationsGroupsDS.Tables["OperationsGroups" + PanelCounter].AsDataView();
        }

        public DataTable FillOperations(int TechCatalogOperationsGroupID, int PanelCounter)
        {
            string SelectCommand = @"SELECT TechCatalogOperationsGroups.GroupName, TechCatalogOperationsGroups.GroupNumber, TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsDetail.TechCatalogOperationsDetailID, TechCatalogOperationsDetail.TechCatalogOperationsGroupID, TechCatalogOperationsDetail.SerialNumber, MachinesOperationName FROM TechCatalogOperationsDetail 
                INNER JOIN TechCatalogOperationsGroups ON TechCatalogOperationsDetail.TechCatalogOperationsGroupID=TechCatalogOperationsGroups.TechCatalogOperationsGroupID
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                WHERE TechCatalogOperationsDetail.TechCatalogOperationsGroupID=" + TechCatalogOperationsGroupID + @"
                ORDER BY TechCatalogOperationsGroups.TechStoreID, TechCatalogOperationsGroups.GroupNumber, TechCatalogOperationsDetail.SerialNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    DA.Fill(OperationsDetailDS, "OperationsDetail" + PanelCounter);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int TechCatalogOperationsDetailID = Convert.ToInt32(DT.Rows[i]["TechCatalogOperationsDetailID"]);
                        if (!GetTerms(TechCatalogOperationsDetailID, InsetTypeID, PatinaID, Height, Width))
                        {
                            DataRow[] rows = OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
                            for (int j = rows.Count() - 1; j >= 0; j--)
                                rows[j].Delete();
                        }

                    }
                }
                if (!OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Columns.Contains("CanClick"))
                    OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Columns.Add("CanClick", System.Type.GetType("System.Boolean"));
            }
            OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].AcceptChanges();
            for (int i = 0; i < OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Rows.Count; i++)
            {
                int TechCatalogOperationsDetailID = Convert.ToInt32(OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Rows[i]["TechCatalogOperationsDetailID"]);
                SelectCommand = @"SELECT TechCatalogStoreDetail.*, MachinesOperations.MachinesOperationName, TechStore.TechStoreName, Measures.Measure FROM TechCatalogStoreDetail
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID = TechCatalogOperationsDetail.TechCatalogOperationsDetailID 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID 
                WHERE TechCatalogStoreDetail.TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID + @"
                ORDER BY GroupA, GroupB, GroupC";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DT.Columns.Add("CanClick", System.Type.GetType("System.Boolean"));
                        for (int j = 0; j < DT.Rows.Count; j++)
                        {
                            int TechStoreID = Convert.ToInt32(DT.Rows[j]["TechStoreID"]);
                            SelectCommand = @"SELECT TechCatalogOperationsGroupID FROM TechCatalogOperationsGroups WHERE TechStoreID=" + TechStoreID;
                            using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
                            {
                                using (DataTable DT1 = new DataTable())
                                {
                                    if (DA1.Fill(DT1) > 0)
                                    {
                                        decimal Count = 0;
                                        if (DT.Rows[j]["Count"] != DBNull.Value)
                                            Count = Convert.ToDecimal(DT.Rows[j]["Count"]);
                                        AddMaterial(Convert.ToInt32(DT.Rows[j]["TechCatalogStoreDetailID"]), Convert.ToInt32(DT.Rows[j]["TechCatalogOperationsDetailID"]),
                                            Convert.ToInt32(DT.Rows[j]["TechStoreID"]), DT.Rows[j]["MachinesOperationName"].ToString(), DT.Rows[j]["TechStoreName"].ToString(),
                                            Count, DT.Rows[j]["Measure"].ToString(), PanelCounter);
                                        OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Rows[i]["CanClick"] = true;
                                    }
                                    else
                                    {
                                        decimal Count = 0;
                                        if (DT.Rows[j]["Count"] != DBNull.Value)
                                            Count = Convert.ToDecimal(DT.Rows[j]["Count"]);
                                        AddMaterial(Convert.ToInt32(DT.Rows[j]["TechCatalogStoreDetailID"]), Convert.ToInt32(DT.Rows[j]["TechCatalogOperationsDetailID"]), 
                                            Convert.ToInt32(DT.Rows[j]["TechStoreID"]), DT.Rows[j]["MachinesOperationName"].ToString(), DT.Rows[j]["TechStoreName"].ToString(),
                                            Count, DT.Rows[j]["Measure"].ToString(), PanelCounter);
                                        if (OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Rows[i]["CanClick"] != DBNull.Value && 
                                            !Convert.ToBoolean(OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Rows[i]["CanClick"]))
                                            OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].Rows[i]["CanClick"] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            OperationsDetailDS.Tables["OperationsDetail" + PanelCounter].DefaultView.RowFilter = "CanClick=1";
            return OperationsDetailDS.Tables["OperationsDetail" + PanelCounter];
        }

        public DataTable FillMaterial(int TechCatalogOperationsDetailID, int PanelCounter)
        {
            string SelectCommand = @"SELECT TechCatalogStoreDetail.*, MachinesOperations.MachinesOperationName, TechStore.TechStoreName, Measures.Measure FROM TechCatalogStoreDetail
                INNER JOIN TechCatalogOperationsDetail ON TechCatalogStoreDetail.TechCatalogOperationsDetailID = TechCatalogOperationsDetail.TechCatalogOperationsDetailID 
                INNER JOIN MachinesOperations ON TechCatalogOperationsDetail.MachinesOperationID = MachinesOperations.MachinesOperationID
                INNER JOIN TechStore ON TechCatalogStoreDetail.TechStoreID = TechStore.TechStoreID
                INNER JOIN Measures ON TechStore.MeasureID = Measures.MeasureID 
                WHERE TechCatalogStoreDetail.TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID + @"
                ORDER BY GroupA, GroupB, GroupC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(StoreDetailDS, "StoreDetail" + PanelCounter);
                if (!StoreDetailDS.Tables["StoreDetail" + PanelCounter].Columns.Contains("CanClick"))
                    StoreDetailDS.Tables["StoreDetail" + PanelCounter].Columns.Add("CanClick", System.Type.GetType("System.Boolean"));
                if (!StoreDetailDS.Tables["StoreDetail" + PanelCounter].Columns.Contains("Fixed"))
                    StoreDetailDS.Tables["StoreDetail" + PanelCounter].Columns.Add("Fixed", System.Type.GetType("System.Int32"));
            }
            for (int i = 0; i < StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows.Count; i++)
            {
                int TechStoreID = Convert.ToInt32(StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["TechStoreID"]);
                int TechCatalogStoreDetailID = Convert.ToInt32(StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["TechCatalogStoreDetailID"]);
                SelectCommand = @"SELECT TechCatalogOperationsGroupID FROM TechCatalogOperationsGroups WHERE TechStoreID=" + TechStoreID;
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
                {
                    using (DataTable  DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                            StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["CanClick"] = true;
                        else
                            StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["CanClick"] = false;
                        if (StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["Fixed"] == DBNull.Value)
                            StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["Fixed"] = 0;

                        DataRow[] rows = FixedMaterialDT.Select("TechCatalogStoreDetailID=" + TechCatalogStoreDetailID);
                        if (rows.Count() > 0)
                            StoreDetailDS.Tables["StoreDetail" + PanelCounter].Rows[i]["Fixed"] = 2;
                    }
                }
            }
            StoreDetailDS.Tables["StoreDetail" + PanelCounter].DefaultView.RowFilter = "CanClick=1";
            return StoreDetailDS.Tables["StoreDetail" + PanelCounter];
        }

        public void FixedMaterial(int PanelCounter)
        {
            for (int i = 0; i < StoreDetailDS.Tables.Count; i++)
            {
                if (!StoreDetailDS.Tables.Contains("StoreDetail" + i) || StoreDetailDS.Tables["StoreDetail" + i].Rows.Count == 0)
                    continue;
                for (int j = 0; j < StoreDetailDS.Tables["StoreDetail" + i].Rows.Count; j++)
                {
                    int TechStoreID = Convert.ToInt32(StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["TechStoreID"]);
                    bool CanClick = Convert.ToBoolean(StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["CanClick"]);
                    if (!CanClick)
                        continue;
                    decimal Count = 0;
                    if (StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["Count"] != DBNull.Value)
                        Count = Convert.ToDecimal(StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["Count"]);
                    AddFixedMaterial(Convert.ToInt32(StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["TechCatalogStoreDetailID"]),
                        Convert.ToInt32(StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["TechCatalogOperationsDetailID"]),
                        Convert.ToInt32(StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["TechStoreID"]), StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["MachinesOperationName"].ToString(),
                        StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["TechStoreName"].ToString(),
                        Count, StoreDetailDS.Tables["StoreDetail" + i].Rows[j]["Measure"].ToString(), PanelCounter);
                }
            }
        }

        private bool GetTerms(int TechCatalogOperationsDetailID, int InsetTypeID, int PatinaID, int Height, int Width)
        {
            DataRow[] rows = OperationsTermsDT.Select("TechCatalogOperationsDetailID=" + TechCatalogOperationsDetailID);
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
    }


    public class DetailsReport
    {
        List<ComplementLabelInfo> Labels;
        
        public DetailsReport()
        {

        }
        
        public void CreateReport(List<ComplementLabelInfo> lLabels)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            Labels = lLabels;

            #region Create fonts and styles

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 13;
            HeaderF1.Boldweight = 13 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFFont HeaderItalicF2 = hssfworkbook.CreateFont();
            HeaderItalicF2.Color = HSSFColor.RED.index;
            HeaderItalicF2.FontHeightInPoints = 12;
            HeaderItalicF2.Boldweight = 12 * 256;
            HeaderItalicF2.FontName = "Calibri";
            HeaderItalicF2.IsItalic = true;

            HSSFFont HeaderF2 = hssfworkbook.CreateFont();
            HeaderF2.FontHeightInPoints = 12;
            HeaderF2.Boldweight = 12 * 256;
            HeaderF2.FontName = "Calibri";

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 9;
            HeaderF3.Boldweight = 9 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 9;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle SimpleDecCS = hssfworkbook.CreateCellStyle();
            SimpleDecCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            SimpleDecCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleDecCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleDecCS.SetFont(SimpleF);

            HSSFCellStyle RateCS = hssfworkbook.CreateCellStyle();
            RateCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.0000");
            RateCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            RateCS.BottomBorderColor = HSSFColor.BLACK.index;
            RateCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            RateCS.LeftBorderColor = HSSFColor.BLACK.index;
            RateCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            RateCS.RightBorderColor = HSSFColor.BLACK.index;
            RateCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            RateCS.TopBorderColor = HSSFColor.BLACK.index;
            RateCS.SetFont(SimpleF);

            HSSFCellStyle ReportCS = hssfworkbook.CreateCellStyle();
            ReportCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS.SetFont(HeaderF1);

            HSSFCellStyle HeaderItalicWithoutBorderCS = hssfworkbook.CreateCellStyle();
            HeaderItalicWithoutBorderCS.SetFont(HeaderItalicF2);

            HSSFCellStyle HeaderWithoutBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithoutBorderCS.SetFont(HeaderF2);

            HSSFCellStyle HeaderWithBorderCS = hssfworkbook.CreateCellStyle();
            HeaderWithBorderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderWithBorderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderWithBorderCS.TopBorderColor = HSSFColor.BLACK.index;
            //HeaderWithBorderCS.WrapText = true;
            HeaderWithBorderCS.SetFont(HeaderF2);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            //HeaderStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            //HeaderStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            SimpleHeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleHeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            SimpleHeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            //SimpleHeaderCS.WrapText = true;
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Подробный отчет");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 25 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);

            int RowIndex = 0;

            HSSFCell Cell1;

            RowIndex++;
            for (int i = 0; i < Labels.Count; i++)
            {
                Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                Cell1.SetCellValue("Клиент:");
                Cell1.CellStyle = HeaderWithoutBorderCS;
                Cell1 = sheet1.CreateRow(RowIndex).CreateCell(1);
                Cell1.SetCellValue(((ComplementLabelInfo)Labels[i]).ClientName);
                Cell1.CellStyle = HeaderWithoutBorderCS;

                RowIndex++;

                Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                Cell1.SetCellValue("№" + ((ComplementLabelInfo)Labels[i]).OrderNumber + "-" + ((ComplementLabelInfo)Labels[i]).MainOrder);
                Cell1.CellStyle = HeaderWithoutBorderCS;
                Cell1 = sheet1.CreateRow(RowIndex).CreateCell(1);
                Cell1.SetCellValue("Примечание к заказу: " + ((ComplementLabelInfo)Labels[i]).TechStoreSubGroup);
                Cell1.CellStyle = HeaderWithoutBorderCS;
                RowIndex++;
                Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                Cell1.SetCellValue("№ упак: " + ((ComplementLabelInfo)Labels[i]).PackNumber);
                Cell1.CellStyle = HeaderWithoutBorderCS;

                int DisplayIndex = 0;
                if (((ComplementLabelInfo)Labels[i]).OrderData.Rows.Count != 0)
                {
                    Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue("Прим.");
                    Cell1.CellStyle = SimpleHeaderCS;
                    Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue("Наимен.");
                    Cell1.CellStyle = SimpleHeaderCS;
                    Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue("Длина");
                    Cell1.CellStyle = SimpleHeaderCS;
                    Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue("Высота");
                    Cell1.CellStyle = SimpleHeaderCS;
                    Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue("Ширина");
                    Cell1.CellStyle = SimpleHeaderCS;
                    Cell1 = sheet1.CreateRow(RowIndex + 1).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue("Кол-во");
                    Cell1.CellStyle = SimpleHeaderCS;

                    RowIndex++;
                }

                if (((ComplementLabelInfo)Labels[i]).OrderData.Rows.Count != 0)
                    RowIndex++;
                DisplayIndex = 0;
                for (int x = 0; x < ((ComplementLabelInfo)Labels[i]).OrderData.Rows.Count; x++)
                {
                    DisplayIndex = 0;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                    if (((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Notes"] != DBNull.Value)
                        Cell1.SetCellValue(((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Notes"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                    if (((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Name"] != DBNull.Value)
                        Cell1.SetCellValue(((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Name"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                    if (((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Length"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Length"]));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                    if (((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Height"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Height"]));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                    if (((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Width"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Width"]));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(RowIndex).CreateCell(DisplayIndex++);
                    if (((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Count"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDouble(((ComplementLabelInfo)Labels[i]).OrderData.Rows[x]["Count"]));
                    Cell1.CellStyle = SimpleCS;

                    RowIndex++;
                }
                RowIndex++;
                RowIndex++;


            }
            string FileName = "report";
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
    }


    public struct ComplementLabelInfo
    {
        public string ClientName;
        public string OrderNumber;
        public string TechStoreSubGroup;
        public string Cover;
        public int MainOrder;
        public int PackNumber;
        public int TotalPackCount;
        public string DocDateTime;
        public string DispatchDate;
        public string BarcodeNumber;
        public string Notes;
        public DataTable OrderData;
        public int FactoryType;
    }

    public struct PackageLabelInfo
    {
        public string ClientName;
        public string TechStoreSubGroup;
        public string Cover;
        public int CabFurAssignmentID;
        public int PackNumber;
        public int TotalPackCount;
        public string AssignmentCreateDateTime;
        public string AddToStorageDateTime;
        public string DispatchDate;
        public string BarcodeNumber;
        public DataTable OrderData;
        public int FactoryType;
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


    public class ComplementLabel
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

        public ComplementLabel()
        {

            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            ZTProfil = new Bitmap(Properties.Resources.ZOVPROFIL);
            STB = new Bitmap(Properties.Resources.eac);
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
            ClientFont = new Font("Arial", 14, FontStyle.Regular);
            DocFont = new Font("Arial", 14, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 12, FontStyle.Regular);
            HeaderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 8, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        private void DrawTable(PrintPageEventArgs ev)
        {
            int HorizLineBotNotes = 87;

            float TopLineY = HorizLineBotNotes + 28;
            float HeaderTopY = TopLineY + 3;
            float OrderTopY = HeaderTopY + 11;
            float BottomLineY = 315;

            float VertLine1 = 11;
            float VertLine3 = 138;
            float VertLine4 = 309;
            float VertLine6 = 349;
            float VertLine7 = 389;
            float VertLine8 = 429;
            float VertLine9 = 467;

            ev.Graphics.DrawLine(Pen, 11, HorizLineBotNotes, 467, HorizLineBotNotes);

            ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).TechStoreSubGroup + " " + ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).Cover, DocFont, FontBrush, 7, HorizLineBotNotes + 2);
            //header
            ev.Graphics.DrawString("Примечание", HeaderFont, FontBrush, VertLine1, HeaderTopY);
            ev.Graphics.DrawString("Наименование", HeaderFont, FontBrush, VertLine3, HeaderTopY);
            ev.Graphics.DrawString("Длин", HeaderFont, FontBrush, VertLine4, HeaderTopY);
            ev.Graphics.DrawString("Выс", HeaderFont, FontBrush, VertLine6, HeaderTopY);
            ev.Graphics.DrawString("Шир", HeaderFont, FontBrush, VertLine7, HeaderTopY);
            ev.Graphics.DrawString("Кол", HeaderFont, FontBrush, VertLine8, HeaderTopY);

            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine6, TopLineY, VertLine6, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine7, TopLineY, VertLine7, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine8, TopLineY, VertLine8, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

            for (int i = 0, p = (int)OrderTopY + 8; i <= ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 16)
            {
                if (i != 11)
                    ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                else
                    break;
            }

            for (int i = 0, p = 10; i < ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 16)
            {
                if (i == 11)
                    break;
                if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Notes"].ToString().Length > 26)
                    ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Notes"].ToString().Substring(0, 26), DecorOrderFont, FontBrush, VertLine1, OrderTopY + p);
                else
                    ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Notes"].ToString(), DecorOrderFont, FontBrush, VertLine1, OrderTopY + p);

                if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Name"].ToString().Length > 26)
                    ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Name"].ToString().Substring(0, 26), DecorOrderFont, FontBrush, VertLine3, OrderTopY + p);
                else
                    ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Name"].ToString(), DecorOrderFont, FontBrush, VertLine3, OrderTopY + p);

                ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, VertLine4, OrderTopY + p);
                ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, VertLine6, OrderTopY + p);
                ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, VertLine7, OrderTopY + p);
                ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, VertLine8, OrderTopY + p);
            }
        }
        
        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref ComplementLabelInfo LabelInfoItem)
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

            if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName.Length > 30)
                ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName.Substring(0, 30), ClientFont, FontBrush, 9, 6);
            else
                ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName, ClientFont, FontBrush, 9, 6);

            int HorizLineClientBot = 28;
            ev.Graphics.DrawLine(Pen, 11, HorizLineClientBot, 467, HorizLineClientBot);
            ev.Graphics.DrawString("№" + ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).OrderNumber + "-" + ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).MainOrder.ToString(),
                DocFont, FontBrush, 8, HorizLineClientBot + 2);
            int HorizLinOrderBot = 52;
            ev.Graphics.DrawLine(Pen, 11, HorizLinOrderBot, 467, HorizLinOrderBot);

            if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).Notes != null)
            {
                ev.Graphics.DrawString("Примечание: ", FrontOrderFont, FontBrush, 10, HorizLinOrderBot + 2);

                if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Length > 37)
                    ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).Notes.Substring(0, 37), NotesFont, FontBrush, 7, HorizLinOrderBot + 16);
                else
                    ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).Notes, NotesFont, FontBrush, 7, HorizLinOrderBot + 16);
            }

            int HorizLinTableTop = 115;
            ev.Graphics.DrawLine(Pen, 11, HorizLinTableTop, 467, HorizLinTableTop);

            ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).PackNumber.ToString() + "(" +
                ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).TotalPackCount.ToString() + ")", DocFont, FontBrush, 371, HorizLineClientBot + 2);
            ev.Graphics.DrawLine(Pen, 371, HorizLineClientBot, 371, HorizLinOrderBot);
            
            DrawTable(ev);

            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 46, ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);

            Barcode.DrawBarcodeText(Barcode.BarcodeLength.Medium, ev.Graphics, ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);



            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Short, 15, ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 342, 54, 130, 15);

            if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            {
                ev.Graphics.DrawImage(ZTTPS, 249, 320, 37, 45);
            }
            else
            {
                ev.Graphics.DrawImage(ZTProfil, 249, 320, 37, 45);
            }

            ev.Graphics.DrawImage(STB, 418, 339, 39, 27);
            //ev.Graphics.DrawImage(RST, 423, 357, 34, 27);

            ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
            ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);

            ev.Graphics.DrawString("ГОСТ 16371-2014", InfoFont, FontBrush, 305, 318);

            if (((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 328);
            else
                ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 328);

            ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 338);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 348);
            ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 358);
            ev.Graphics.DrawString("Изготовлено: " + ((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).DocDateTime, InfoFont, FontBrush, 305, 368);
            ev.Graphics.DrawString("Распечатено: " + Security.GetCurrentDate().ToString(), InfoFont, FontBrush, 305, 378);
            ev.Graphics.DrawString(((ComplementLabelInfo)LabelInfo[CurrentLabelNumber]).DispatchDate, DispatchFont, FontBrush, 237, 374);

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
            STB = new Bitmap(Properties.Resources.eac);
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
            ClientFont = new Font("Arial", 26, FontStyle.Regular);
            DocFont = new Font("Arial", 14, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 12, FontStyle.Regular);
            HeaderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 8, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        private void DrawTable(PrintPageEventArgs ev)
        {
            int HorizLineBotNotes = 87;

            float TopLineY = HorizLineBotNotes + 28;
            float HeaderTopY = TopLineY + 3;
            float OrderTopY = HeaderTopY + 11;
            float BottomLineY = 315;

            float VertLine1 = 11;
            float VertLine3 = 138;
            float VertLine4 = 309;
            float VertLine6 = 349;
            float VertLine7 = 389;
            float VertLine8 = 429;
            float VertLine9 = 467;

            ev.Graphics.DrawLine(Pen, 11, HorizLineBotNotes, 467, HorizLineBotNotes);

            ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).TechStoreSubGroup + " " + ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).Cover, DocFont, FontBrush, 7, HorizLineBotNotes + 2);
            //header
            ev.Graphics.DrawString("Примечание", HeaderFont, FontBrush, VertLine1, HeaderTopY);
            ev.Graphics.DrawString("Наименование", HeaderFont, FontBrush, VertLine3, HeaderTopY);
            ev.Graphics.DrawString("Длин", HeaderFont, FontBrush, VertLine4, HeaderTopY);
            ev.Graphics.DrawString("Выс", HeaderFont, FontBrush, VertLine6, HeaderTopY);
            ev.Graphics.DrawString("Шир", HeaderFont, FontBrush, VertLine7, HeaderTopY);
            ev.Graphics.DrawString("Кол", HeaderFont, FontBrush, VertLine8, HeaderTopY);

            ev.Graphics.DrawLine(Pen, VertLine1, TopLineY, VertLine1, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine3, TopLineY, VertLine3, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine4, TopLineY, VertLine4, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine6, TopLineY, VertLine6, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine7, TopLineY, VertLine7, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine8, TopLineY, VertLine8, BottomLineY);
            ev.Graphics.DrawLine(Pen, VertLine9, TopLineY, VertLine9, BottomLineY);

            for (int i = 0, p = (int)OrderTopY + 8; i <= ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 16)
            {
                if (i != 11)
                    ev.Graphics.DrawLine(Pen, 11, p, 467, p);
                else
                    break;
            }

            for (int i = 0, p = 10; i < ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows.Count; i++, p += 16)
            {
                if (i == 11)
                    break;
                if (((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Notes"].ToString().Length > 26)
                    ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Notes"].ToString().Substring(0, 26), DecorOrderFont, FontBrush, VertLine1, OrderTopY + p);
                else
                    ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Notes"].ToString(), DecorOrderFont, FontBrush, VertLine1, OrderTopY + p);

                if (((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Name"].ToString().Length > 26)
                    ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Name"].ToString().Substring(0, 26), DecorOrderFont, FontBrush, VertLine3, OrderTopY + p);
                else
                    ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Name"].ToString(), DecorOrderFont, FontBrush, VertLine3, OrderTopY + p);

                ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Length"].ToString(), DecorOrderFont, FontBrush, VertLine4, OrderTopY + p);
                ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Height"].ToString(), DecorOrderFont, FontBrush, VertLine6, OrderTopY + p);
                ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Width"].ToString(), DecorOrderFont, FontBrush, VertLine7, OrderTopY + p);
                ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).OrderData.Rows[i]["Count"].ToString(), DecorOrderFont, FontBrush, VertLine8, OrderTopY + p);
            }
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref PackageLabelInfo LabelInfoItem)
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


            int HorizLineClientBot = 18;
            ev.Graphics.DrawLine(Pen, 11, HorizLineClientBot, 467, HorizLineClientBot);

            if (((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName.Length > 30)
                ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName.Substring(0, 30), ClientFont, FontBrush, 79, HorizLineClientBot + 2);
            else
                ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).ClientName, ClientFont, FontBrush, 79, HorizLineClientBot + 2);

            int HorizLinOrderBot = 62;
            ev.Graphics.DrawLine(Pen, 11, HorizLinOrderBot, 467, HorizLinOrderBot);
            ev.Graphics.DrawString("№" + ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).CabFurAssignmentID + " " + ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).AssignmentCreateDateTime, DocFont, FontBrush, 8, HorizLinOrderBot + 2);
            
            int HorizLinTableTop = 115;
            ev.Graphics.DrawLine(Pen, 11, HorizLinTableTop, 467, HorizLinTableTop);

            ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).PackNumber.ToString() + "(" +
                ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).TotalPackCount.ToString() + ")", DocFont, FontBrush, 371, HorizLineClientBot + 2);
            ev.Graphics.DrawLine(Pen, 371, HorizLineClientBot, 371, HorizLinOrderBot);

            DrawTable(ev);

            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 46, ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);

            Barcode.DrawBarcodeText(Barcode.BarcodeLength.Medium, ev.Graphics, ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);
            
            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Short, 15, ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 342, 54, 130, 15);

            if (((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            {
                ev.Graphics.DrawImage(ZTTPS, 249, 320, 37, 45);
            }
            else
            {
                ev.Graphics.DrawImage(ZTProfil, 249, 320, 37, 45);
            }

            ev.Graphics.DrawImage(STB, 418, 319, 39, 27);
            //ev.Graphics.DrawImage(RST, 423, 357, 34, 27);

            ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
            ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);

            ev.Graphics.DrawString("ГОСТ 16371-2014", InfoFont, FontBrush, 305, 318);

            if (((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 328);
            else
                ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 328);

            ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 338);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 348);
            ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 358);
            ev.Graphics.DrawString("Изготовлено: " + ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).AddToStorageDateTime, InfoFont, FontBrush, 305, 368);
            ev.Graphics.DrawString("Распечатено: " + Security.GetCurrentDate().ToString(), InfoFont, FontBrush, 305, 378);
            ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).DispatchDate, DispatchFont, FontBrush, 237, 374);
            
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
