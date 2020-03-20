using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.Clients
{
    public enum MachineFileTypes
    {
        MachineFoto = 1,
        MachineAspiration = 2,
        MachineMechanics = 3,
        MachineTechnical = 4,
        MachineElectrics = 5,
        MachinePneumatics = 6,
        MachineHydraulics = 7,
        EquipmentTools = 8,
        AspirationDetails = 9,
        MechanicsDetails = 10,
        ElectricsDetails = 11,
        HydraulicsDetails = 12,
        PneumaticsDetails = 13,
        ExploitationTools = 14,
        RepairTools = 15,
        ServiceTools = 16,
        Lubricant = 17,
        OperatingInstructions = 18,
        ServiceInstructions = 19,
        LaborProtInstructions = 20,
        Admissions = 21,
        Journal = 22
    }

    //public class MachineFileTypes
    //{
    //    public int MachineFoto = 1;
    //    public int MachineAspiration = 2;
    //    public int MachineMechanics = 3;
    //    public int MachineTechnical = 4;
    //    public int MachineElectrics = 5;
    //    public int MachinePneumatics = 6;
    //    public int MachineHydraulics = 7;
    //    public int EquipmentTools = 8;
    //    public int AspirationDetails = 9;
    //    public int MechanicsDetails = 10;
    //    public int ElectricsDetails = 11;
    //    public int HydraulicsDetails = 12;
    //    public int PneumaticsDetails = 13;
    //    public int ExploitationTools = 14;
    //    public int RepairTools = 15;
    //    public int ServiceTools = 16;
    //    public int Lubricant = 17;

    //    public MachineFileTypes()
    //    {
    //        using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineFileTypes", ConnectionStrings.CatalogConnectionString))
    //        {
    //            using (DataTable DT = new DataTable())
    //            {
    //                MachineFileType.AspirationDetails;
    //                DA.Fill(DT);
    //                MachineFoto = Convert.ToInt32(DT.Rows[0]["Name"]);
    //                MachineAspiration = Convert.ToInt32(DT.Rows[1]["Name"]);
    //                MachineMechanics = Convert.ToInt32(DT.Rows[2]["Name"]);
    //                MachineTechnical = Convert.ToInt32(DT.Rows[3]["Name"]);
    //                MachineElectrics = Convert.ToInt32(DT.Rows[4]["Name"]);
    //                MachinePneumatics = Convert.ToInt32(DT.Rows[5]["Name"]);
    //                MachineHydraulics = Convert.ToInt32(DT.Rows[6]["Name"]);
    //                EquipmentTools = Convert.ToInt32(DT.Rows[7]["Name"]);
    //                AspirationDetails = Convert.ToInt32(DT.Rows[8]["Name"]);
    //                MechanicsDetails = Convert.ToInt32(DT.Rows[9]["Name"]);
    //                ElectricsDetails = Convert.ToInt32(DT.Rows[10]["Name"]);
    //                HydraulicsDetails = Convert.ToInt32(DT.Rows[11]["Name"]);
    //                PneumaticsDetails = Convert.ToInt32(DT.Rows[12]["Name"]);
    //                ExploitationTools = Convert.ToInt32(DT.Rows[13]["Name"]);
    //                RepairTools = Convert.ToInt32(DT.Rows[14]["Name"]);
    //                ServiceTools = Convert.ToInt32(DT.Rows[15]["Name"]);
    //                Lubricant = Convert.ToInt32(DT.Rows[16]["Name"]);
    //            }
    //        }
    //    }
    //}

    public class MachinesCatalog
    {
        public FileManager FM = new FileManager();

        DataTable FindMachinesDT = new DataTable();
        BindingSource FindMachinesBS = new BindingSource();
        DataTable MachineDocumentsDT;
        DataTable TempMachineDocumentsDT;

        DataTable AspirationDT;
        DataTable MechanicsDT;
        DataTable ElectricsDT;
        DataTable HydraulicsDT;
        DataTable PneumaticsDT;

        DataTable AspirationDetailsDT;
        DataTable MechanicsDetailsDT;
        DataTable ElectricsDetailsDT;
        DataTable HydraulicsDetailsDT;
        DataTable PneumaticsDetailsDT;

        DataTable ExploitationToolsDT;
        DataTable RepairToolsDT;
        DataTable ServiceToolsDT;
        DataTable LubricantDT;
        DataTable EquipmentToolsDT;

        DataTable AspirationUnitsDT;
        DataTable MechanicsUnitsDT;
        DataTable ElectricsUnitsDT;
        DataTable HydraulicsUnitsDT;
        DataTable PneumaticsUnitsDT;

        DataTable TechnicalSpecificationDT;
        DataTable TempTechnicalSpecificationDT;

        DataTable SpareGroupsDT;
        DataTable SparesDT;
        DataTable SparesOnStockDT;
        DataTable MachinesDT;
        DataTable MachinesSummaryDT;
        DataTable MainParametersDT;
        DataTable MeasuresDT;
        DataTable TempMachinesDT;
        DataTable UnitsDT;

        DataTable FactoryDT;
        DataTable SectorsDT;
        DataTable SubSectorsDT;

        BindingSource FactoryBS;
        BindingSource SectorsBS;
        BindingSource SubSectorsBS;

        BindingSource AspirationBS;
        BindingSource MechanicsBS;
        BindingSource ElectricsBS;
        BindingSource HydraulicsBS;
        BindingSource PneumaticsBS;

        BindingSource AspirationDetailsBS;
        BindingSource MechanicsDetailsBS;
        BindingSource ElectricsDetailsBS;
        BindingSource HydraulicsDetailsBS;
        BindingSource PneumaticsDetailsBS;

        BindingSource ExploitationToolsBS;
        BindingSource RepairToolsBS;
        BindingSource ServiceToolsBS;
        BindingSource LubricantBS;
        BindingSource EquipmentBS;

        BindingSource AspirationUnitsBS;
        BindingSource MechanicsUnitsBS;
        BindingSource ElectricsUnitsBS;
        BindingSource HydraulicsUnitsBS;
        BindingSource PneumaticsUnitsBS;

        BindingSource TechnicalSpecificationBS;
        BindingSource TempTechnicalSpecificationBS;

        BindingSource AspirationSpareGroupsBS;
        BindingSource MechanicsSpareGroupsBS;
        BindingSource ElectricsSpareGroupsBS;
        BindingSource HydraulicsSpareGroupsBS;
        BindingSource PneumaticsSpareGroupsBS;

        BindingSource AspirationSparesBS;
        BindingSource MechanicsSparesBS;
        BindingSource ElectricsSparesBS;
        BindingSource HydraulicsSparesBS;
        BindingSource PneumaticsSparesBS;

        BindingSource AspirationSparesOnStockBS;
        BindingSource MechanicsSparesOnStockBS;
        BindingSource ElectricsSparesOnStockBS;
        BindingSource HydraulicsSparesOnStockBS;
        BindingSource PneumaticsSparesOnStockBS;

        BindingSource MachinesBS;
        BindingSource SparesOnStockBS;

        BindingSource AspirationFilesBS;
        BindingSource MechanicsFilesBS;
        BindingSource ElectricsFilesBS;
        BindingSource HydraulicsFilesBS;
        BindingSource PneumaticsFilesBS;

        BindingSource AspirationDetailFilesBS;
        BindingSource MechanicsDetailFilesBS;
        BindingSource ElectricsDetailFilesBS;
        BindingSource HydraulicsDetailFilesBS;
        BindingSource PneumaticsDetailFilesBS;

        BindingSource ExploitationToolsFilesBS;
        BindingSource RepairToolsFilesBS;
        BindingSource ServiceToolsFilesBS;
        BindingSource LubricantFilesBS;
        BindingSource EquipmentFilesBS;

        BindingSource OperatingInstructionsBS;
        BindingSource ServiceInstructionsBS;
        BindingSource LaborProtInstructionsBS;
        BindingSource JournalBS;
        BindingSource AdmissionsBS;
        BindingSource TechnicalFilesBS;

        BindingSource MachinesSummaryBS;
        BindingSource MainParametersBS;
        BindingSource TempMachinesBS;

        public MachinesCatalog()
        {

        }

        public void CreateTempMachines()
        {
            if (TempMachinesDT != null)
                TempMachinesDT.Dispose();
            using (DataView DV = new DataView(MachinesDT))
            {
                DV.Sort = "MachineName";
                TempMachinesDT = DV.ToTable(false, "MachineID", "MachineName");
                TempMachinesDT.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                foreach (DataRow item in TempMachinesDT.Rows)
                {
                    item["Check"] = false;
                }
                TempMachinesBS.DataSource = TempMachinesDT;
                TempMachinesBS.MoveFirst();
            }
        }

        public void F()
        {
            DataTable DT = TechnicalSpecificationDT.Clone();
            using (DataView DV = new DataView(TechnicalSpecificationDT))
            {
                DV.Sort = "Group";
                DT = DV.ToTable(true, "Group", "GroupNumber");
            }
            TempTechnicalSpecificationDT.Clear();
            foreach (DataRow item in DT.Rows)
            {
                TempTechnicalSpecificationDT.Rows.Add(item.ItemArray);
            }
        }

        public void MachinesSumming(int[] Machines)
        {
            decimal ConsumedPower = 0;
            decimal CompressedAirConsumption = 0;
            decimal AspirationCapacity = 0;

            DataRow[] Rows = MachinesDT.Select("MachineID IN (" + string.Join(",", Machines) + ") AND Firm='ЗОВ-Профиль'");
            if (Rows.Count() > 0)
            {
                foreach (DataRow item in Rows)
                {
                    int MachineID = Convert.ToInt32(item["MachineID"]);
                    decimal.TryParse(item["ConsumedPower"].ToString(), out decimal d);
                    ConsumedPower += d;
                    d = 0;
                    decimal.TryParse(item["CompressedAirConsumption"].ToString(), out d);
                    CompressedAirConsumption += d;
                    d = 0;
                    decimal.TryParse(item["AspirationCapacity"].ToString(), out d);
                    AspirationCapacity += d;
                    d = 0;
                }
            }
            MachinesSummaryDT.Clear();
            AddMachinesSummaryNewRow("ЗОВ-Профиль", string.Empty);
            AddMachinesSummaryNewRow("  Мощность, кВт", ConsumedPower.ToString());
            AddMachinesSummaryNewRow("  Расход, л/мин", CompressedAirConsumption.ToString());
            AddMachinesSummaryNewRow("  Объём, м3/час", AspirationCapacity.ToString());

            ConsumedPower = 0;
            CompressedAirConsumption = 0;
            AspirationCapacity = 0;

            Rows = MachinesDT.Select("MachineID IN (" + string.Join(",", Machines) + ") AND Firm='ЗОВ-ТПС'");
            if (Rows.Count() > 0)
            {
                foreach (DataRow item in Rows)
                {
                    int MachineID = Convert.ToInt32(item["MachineID"]);
                    decimal.TryParse(item["ConsumedPower"].ToString(), out decimal d);
                    ConsumedPower += d;
                    d = 0;
                    decimal.TryParse(item["CompressedAirConsumption"].ToString(), out d);
                    CompressedAirConsumption += d;
                    d = 0;
                    decimal.TryParse(item["AspirationCapacity"].ToString(), out d);
                    AspirationCapacity += d;
                    d = 0;
                }
            }
            AddMachinesSummaryNewRow("ЗОВ-ТПС", string.Empty);
            AddMachinesSummaryNewRow("  Мощность, кВт", ConsumedPower.ToString());
            AddMachinesSummaryNewRow("  Расход, л/мин", CompressedAirConsumption.ToString());
            AddMachinesSummaryNewRow("  Объём, м3/час", AspirationCapacity.ToString());
        }

        private void Create()
        {
            FactoryDT = new DataTable();
            SectorsDT = new DataTable();
            SubSectorsDT = new DataTable();

            FactoryBS = new BindingSource();
            SectorsBS = new BindingSource();
            SubSectorsBS = new BindingSource();

            MachineDocumentsDT = new DataTable();
            MachineDocumentsDT.Columns.Add(new DataColumn("FileType", Type.GetType("System.Int32")));
            MachineDocumentsDT.Columns.Add(new DataColumn("MachineDocumentID", Type.GetType("System.Int32")));
            MachineDocumentsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            MachineDocumentsDT.Columns.Add(new DataColumn("FileSize", Type.GetType("System.Int64")));
            MachineDocumentsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            MainParametersDT = new DataTable();
            MainParametersDT.Columns.Add(new DataColumn("ColumnName", Type.GetType("System.String")));
            MainParametersDT.Columns.Add(new DataColumn("Number", Type.GetType("System.String")));
            MainParametersDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            MainParametersDT.Columns.Add(new DataColumn("Value", Type.GetType("System.String")));
            CreateMainParameters();

            MachinesSummaryDT = new DataTable();
            MachinesSummaryDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            MachinesSummaryDT.Columns.Add(new DataColumn("Value", Type.GetType("System.String")));

            AspirationDT = new DataTable()
            {
                TableName = "AspirationDT"
            };
            AspirationDT.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));//Общее описание
            AspirationDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));//Заметки

            MechanicsDT = new DataTable()
            {
                TableName = "MechanicsDT"
            };
            MechanicsDT.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));//Общее описание
            MechanicsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));//Заметки

            ElectricsDT = new DataTable()
            {
                TableName = "ElectricsDT"
            };
            ElectricsDT.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));//Общее описание
            ElectricsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));//Заметки

            HydraulicsDT = new DataTable()
            {
                TableName = "HydraulicsDT"
            };
            HydraulicsDT.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));//Общее описание
            HydraulicsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));//Заметки

            PneumaticsDT = new DataTable()
            {
                TableName = "PneumaticsDT"
            };
            PneumaticsDT.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));//Общее описание
            PneumaticsDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));//Заметки

            TechnicalSpecificationDT = new DataTable()
            {
                TableName = "TechnicalSpecificationDT"
            };
            TechnicalSpecificationDT.Columns.Add(new DataColumn("GroupDisplay", Type.GetType("System.String")));
            TechnicalSpecificationDT.Columns.Add(new DataColumn("Group", Type.GetType("System.String")));
            TechnicalSpecificationDT.Columns.Add(new DataColumn("GroupNumber", Type.GetType("System.Int32")));
            TechnicalSpecificationDT.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            TechnicalSpecificationDT.Columns.Add(new DataColumn("Value", Type.GetType("System.String")));
            TechnicalSpecificationDT.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
            TechnicalSpecificationDT.Columns.Add(new DataColumn("Notes", Type.GetType("System.String")));

            TempTechnicalSpecificationDT = new DataTable();
            TempTechnicalSpecificationDT.Columns.Add(new DataColumn("Group", Type.GetType("System.String")));
            TempTechnicalSpecificationDT.Columns.Add(new DataColumn("GroupNumber", Type.GetType("System.Int32")));

            MachinesDT = new DataTable();
            FindMachinesDT = new DataTable();

            SpareGroupsDT = new DataTable();
            SparesDT = new DataTable();
            MeasuresDT = new DataTable();
            SparesOnStockDT = new DataTable();
            TempMachinesDT = new DataTable();
            TempMachineDocumentsDT = new DataTable();
            UnitsDT = new DataTable();

            AspirationDetailsDT = new DataTable();
            MechanicsDetailsDT = new DataTable();
            ElectricsDetailsDT = new DataTable();
            HydraulicsDetailsDT = new DataTable();
            PneumaticsDetailsDT = new DataTable();

            ExploitationToolsDT = new DataTable();
            RepairToolsDT = new DataTable();
            ServiceToolsDT = new DataTable();
            LubricantDT = new DataTable();
            EquipmentToolsDT = new DataTable();

            AspirationUnitsDT = new DataTable();
            MechanicsUnitsDT = new DataTable();
            ElectricsUnitsDT = new DataTable();
            HydraulicsUnitsDT = new DataTable();
            PneumaticsUnitsDT = new DataTable();

            AspirationBS = new BindingSource();
            MechanicsBS = new BindingSource();
            ElectricsBS = new BindingSource();
            HydraulicsBS = new BindingSource();
            PneumaticsBS = new BindingSource();

            AspirationDetailsBS = new BindingSource();
            MechanicsDetailsBS = new BindingSource();
            ElectricsDetailsBS = new BindingSource();
            HydraulicsDetailsBS = new BindingSource();
            PneumaticsDetailsBS = new BindingSource();

            ExploitationToolsBS = new BindingSource();
            RepairToolsBS = new BindingSource();
            ServiceToolsBS = new BindingSource();
            LubricantBS = new BindingSource();
            EquipmentBS = new BindingSource();

            AspirationUnitsBS = new BindingSource();
            MechanicsUnitsBS = new BindingSource();
            ElectricsUnitsBS = new BindingSource();
            HydraulicsUnitsBS = new BindingSource();
            PneumaticsUnitsBS = new BindingSource();

            TechnicalSpecificationBS = new BindingSource();

            AspirationSpareGroupsBS = new BindingSource();
            MechanicsSpareGroupsBS = new BindingSource();
            ElectricsSpareGroupsBS = new BindingSource();
            HydraulicsSpareGroupsBS = new BindingSource();
            PneumaticsSpareGroupsBS = new BindingSource();

            AspirationSparesBS = new BindingSource();
            MechanicsSparesBS = new BindingSource();
            ElectricsSparesBS = new BindingSource();
            HydraulicsSparesBS = new BindingSource();
            PneumaticsSparesBS = new BindingSource();

            AspirationSparesOnStockBS = new BindingSource();
            MechanicsSparesOnStockBS = new BindingSource();
            ElectricsSparesOnStockBS = new BindingSource();
            HydraulicsSparesOnStockBS = new BindingSource();
            PneumaticsSparesOnStockBS = new BindingSource();

            MachinesBS = new BindingSource();
            FindMachinesBS = new BindingSource();
            SparesOnStockBS = new BindingSource();

            AspirationFilesBS = new BindingSource();
            MechanicsFilesBS = new BindingSource();
            ElectricsFilesBS = new BindingSource();
            HydraulicsFilesBS = new BindingSource();
            PneumaticsFilesBS = new BindingSource();

            AspirationDetailFilesBS = new BindingSource();
            MechanicsDetailFilesBS = new BindingSource();
            ElectricsDetailFilesBS = new BindingSource();
            HydraulicsDetailFilesBS = new BindingSource();
            PneumaticsDetailFilesBS = new BindingSource();

            ExploitationToolsFilesBS = new BindingSource();
            RepairToolsFilesBS = new BindingSource();
            ServiceToolsFilesBS = new BindingSource();
            LubricantFilesBS = new BindingSource();
            EquipmentFilesBS = new BindingSource();

            TechnicalFilesBS = new BindingSource();

            OperatingInstructionsBS = new BindingSource();
            ServiceInstructionsBS = new BindingSource();
            LaborProtInstructionsBS = new BindingSource();
            JournalBS = new BindingSource();
            AdmissionsBS = new BindingSource();

            MainParametersBS = new BindingSource();
            MachinesSummaryBS = new BindingSource();
            TempMachinesBS = new BindingSource();
            TempTechnicalSpecificationBS = new BindingSource();
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 Machines.MachineName FROM MachineDetails
                INNER JOIN Machines ON MachineDetails.MachineID = Machines.MachineID", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FindMachinesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FactoryID, FactoryName FROM Factory WHERE FactoryID <> 0", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SectorID, FactoryID, SectorName FROM Sectors", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SectorsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT SubSectorID, SectorID, SubSectorName FROM SubSectors", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SubSectorsDT);
            }
            //DetailGroupsDT
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineSpareGroups", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SpareGroupsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineSpares.*, MachineSpareGroups.Type FROM MachineSpares
                INNER JOIN MachineSpareGroups ON MachineSpares.MachineSpareGroupID=MachineSpareGroups.MachineSpareGroupID", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SparesDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 MachineSparesOnStock.*,
                MachineSpareGroups.MachineSpareGroupID, MachineSpares.MachineSpareID, MachineSpares.Description, MachineDetails.Type FROM MachineSparesOnStock
                INNER JOIN MachineDetails ON MachineSparesOnStock.MachineDetailID = MachineDetails.MachineDetailID
                INNER JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                INNER JOIN MachineSpareGroups ON MachineSpares.MachineSpareGroupID = MachineSpareGroups.MachineSpareGroupID", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SparesOnStockDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineID, SubSectorID, MachineName, TechPasportName, MachineFunction, Producer, ProducersContacts, 
                         ProducersRepresentative, RepresentativesContacts, SerialNumber, Year, Commissioning, Productivity, ConsumedPower, Voltage, OperatingPressure, 
                         CompressedAirConsumption, AspirationCapacity, AspirationFlowRate, Firm, InventoryNumber, MachineOperator, Technician, Sizes, Weight, EquipmentNotes, TechnicalNotes, 
                         TechnicalSpecification, MachineElectrics, MachineHydraulics, MachinePneumatics, MachineAspiration, MachineMechanics, PermanentWorks FROM Machines", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MachinesDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Measures", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MeasuresDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(UnitsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 MachineDetails.*, MachineSpares.Description FROM MachineDetails
                LEFT JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(AspirationDetailsDT);
                DA.Fill(MechanicsDetailsDT);
                DA.Fill(ElectricsDetailsDT);
                DA.Fill(HydraulicsDetailsDT);
                DA.Fill(PneumaticsDetailsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM MachineTools", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ExploitationToolsDT);
                DA.Fill(RepairToolsDT);
                DA.Fill(ServiceToolsDT);
                DA.Fill(LubricantDT);
                DA.Fill(EquipmentToolsDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM MachineUnits", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(AspirationUnitsDT);
                DA.Fill(MechanicsUnitsDT);
                DA.Fill(ElectricsUnitsDT);
                DA.Fill(HydraulicsUnitsDT);
                DA.Fill(PneumaticsUnitsDT);
            }
        }

        public void Binding()
        {
            FindMachinesBS.DataSource = FindMachinesDT;

            FactoryBS.DataSource = FactoryDT;
            SectorsBS.DataSource = SectorsDT;
            SubSectorsBS.DataSource = SubSectorsDT;

            AspirationDetailsBS.DataSource = AspirationDetailsDT;
            MechanicsDetailsBS.DataSource = MechanicsDetailsDT;
            ElectricsDetailsBS.DataSource = ElectricsDetailsDT;
            HydraulicsDetailsBS.DataSource = HydraulicsDetailsDT;
            PneumaticsDetailsBS.DataSource = PneumaticsDetailsDT;

            ExploitationToolsBS.DataSource = ExploitationToolsDT;
            RepairToolsBS.DataSource = RepairToolsDT;
            ServiceToolsBS.DataSource = ServiceToolsDT;
            LubricantBS.DataSource = LubricantDT;
            EquipmentBS.DataSource = EquipmentToolsDT;

            AspirationUnitsBS.DataSource = AspirationUnitsDT;
            MechanicsUnitsBS.DataSource = MechanicsUnitsDT;
            ElectricsUnitsBS.DataSource = ElectricsUnitsDT;
            HydraulicsUnitsBS.DataSource = HydraulicsUnitsDT;
            PneumaticsUnitsBS.DataSource = PneumaticsUnitsDT;

            TechnicalSpecificationBS.DataSource = TechnicalSpecificationDT;
            TempTechnicalSpecificationBS.DataSource = TempTechnicalSpecificationDT;
            //TechnicalSpecificationBS.Sort = "Name";

            AspirationBS.DataSource = AspirationDT;
            MechanicsBS.DataSource = MechanicsDT;
            ElectricsBS.DataSource = ElectricsDT;
            HydraulicsBS.DataSource = HydraulicsDT;
            PneumaticsBS.DataSource = PneumaticsDT;

            MachinesBS.DataSource = MachinesDT;
            MachinesBS.Sort = "MachineName";
            SparesOnStockBS.DataSource = SparesOnStockDT;

            AspirationSpareGroupsBS.DataSource = new DataView(SpareGroupsDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineAspiration),
                string.Empty, DataViewRowState.CurrentRows);

            MechanicsSpareGroupsBS.DataSource = new DataView(SpareGroupsDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineMechanics),
                string.Empty, DataViewRowState.CurrentRows);

            ElectricsSpareGroupsBS.DataSource = new DataView(SpareGroupsDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineElectrics),
                string.Empty, DataViewRowState.CurrentRows);

            HydraulicsSpareGroupsBS.DataSource = new DataView(SpareGroupsDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineHydraulics),
                string.Empty, DataViewRowState.CurrentRows);

            PneumaticsSpareGroupsBS.DataSource = new DataView(SpareGroupsDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachinePneumatics),
                string.Empty, DataViewRowState.CurrentRows);

            AspirationSparesBS.DataSource = new DataView(SparesDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineAspiration),
                string.Empty, DataViewRowState.CurrentRows);

            MechanicsSparesBS.DataSource = new DataView(SparesDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineMechanics),
                string.Empty, DataViewRowState.CurrentRows);

            ElectricsSparesBS.DataSource = new DataView(SparesDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineElectrics),
                string.Empty, DataViewRowState.CurrentRows);

            HydraulicsSparesBS.DataSource = new DataView(SparesDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineHydraulics),
                string.Empty, DataViewRowState.CurrentRows);

            PneumaticsSparesBS.DataSource = new DataView(SparesDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachinePneumatics),
                string.Empty, DataViewRowState.CurrentRows);

            AspirationSparesOnStockBS.DataSource = new DataView(SparesOnStockDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineAspiration),
                string.Empty, DataViewRowState.CurrentRows);

            MechanicsSparesOnStockBS.DataSource = new DataView(SparesOnStockDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineMechanics),
                string.Empty, DataViewRowState.CurrentRows);

            ElectricsSparesOnStockBS.DataSource = new DataView(SparesOnStockDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineElectrics),
                string.Empty, DataViewRowState.CurrentRows);

            HydraulicsSparesOnStockBS.DataSource = new DataView(SparesOnStockDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachineHydraulics),
                string.Empty, DataViewRowState.CurrentRows);

            PneumaticsSparesOnStockBS.DataSource = new DataView(SparesOnStockDT,
                "Type = " + Convert.ToInt32(MachineFileTypes.MachinePneumatics),
                string.Empty, DataViewRowState.CurrentRows);

            AspirationFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MachineAspiration),
                string.Empty, DataViewRowState.CurrentRows);

            MechanicsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MachineMechanics),
                string.Empty, DataViewRowState.CurrentRows);

            ElectricsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MachineElectrics),
                string.Empty, DataViewRowState.CurrentRows);

            HydraulicsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MachineHydraulics),
                string.Empty, DataViewRowState.CurrentRows);

            PneumaticsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MachinePneumatics),
                string.Empty, DataViewRowState.CurrentRows);

            AspirationDetailFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.AspirationDetails),
                string.Empty, DataViewRowState.CurrentRows);

            MechanicsDetailFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MechanicsDetails),
                string.Empty, DataViewRowState.CurrentRows);

            ElectricsDetailFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.ElectricsDetails),
                string.Empty, DataViewRowState.CurrentRows);

            HydraulicsDetailFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.HydraulicsDetails),
                string.Empty, DataViewRowState.CurrentRows);

            PneumaticsDetailFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.PneumaticsDetails),
                string.Empty, DataViewRowState.CurrentRows);

            ExploitationToolsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.ExploitationTools),
                string.Empty, DataViewRowState.CurrentRows);

            RepairToolsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.RepairTools),
                string.Empty, DataViewRowState.CurrentRows);

            ServiceToolsFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.ServiceTools),
                string.Empty, DataViewRowState.CurrentRows);

            LubricantFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.Lubricant),
                string.Empty, DataViewRowState.CurrentRows);

            EquipmentFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.EquipmentTools),
                string.Empty, DataViewRowState.CurrentRows);

            TechnicalFilesBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.MachineTechnical),
                string.Empty, DataViewRowState.CurrentRows);

            OperatingInstructionsBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.OperatingInstructions),
                string.Empty, DataViewRowState.CurrentRows);

            ServiceInstructionsBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.ServiceInstructions),
                string.Empty, DataViewRowState.CurrentRows);

            LaborProtInstructionsBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.LaborProtInstructions),
                string.Empty, DataViewRowState.CurrentRows);

            JournalBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.Journal),
                string.Empty, DataViewRowState.CurrentRows);

            AdmissionsBS.DataSource = new DataView(MachineDocumentsDT,
                "FileType = " + Convert.ToInt32(MachineFileTypes.Admissions),
                string.Empty, DataViewRowState.CurrentRows);

            MainParametersBS.DataSource = MainParametersDT;
            MachinesSummaryBS.DataSource = MachinesSummaryDT;
        }

        //public void GetMachinePhoto()
        //{
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MachineID, MachineName, MachinePhoto FROM Machines WHERE MachinePhoto IS NOT NULL",
        //        ConnectionStrings.CatalogConnectionString))
        //    {
        //        using (DataTable DT = new DataTable())
        //        {
        //            DA.Fill(DT);
        //            for (int i = 0; i < DT.Rows.Count; i++)
        //            {
        //                byte[] b = (byte[])DT.Rows[i]["MachinePhoto"];
        //                using (MemoryStream ms = new MemoryStream(b))
        //                {
        //                    PictureBox pb = new PictureBox();
        //                    pb.Image = Image.FromStream(ms);
        //                    pb.Image.Save(@"D:\Images\" + DT.Rows[i]["MachineID"].ToString() + ".jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
        //                }
        //            }
        //        }
        //    }
        //}

        public void AddMachine(int SubSectorID, string Name)
        {
            DataRow NewRow = MachinesDT.NewRow();
            NewRow["MachineName"] = Name;
            NewRow["SubSectorID"] = SubSectorID;
            MachinesDT.Rows.Add(NewRow);
        }

        public void EditMachine(int MachineID, int SubSectorID, string Name)
        {
            DataRow[] EditRows = MachinesDT.Select("MachineID = " + MachineID);
            if (EditRows.Count() > 0)
            {
                EditRows[0]["MachineName"] = Name;
                EditRows[0]["SubSectorID"] = SubSectorID;
            }
        }

        public int GetSubSectorID(int MachineID)
        {
            DataRow[] rows = MachinesDT.Select("MachineID = " + MachineID);
            if (rows.Count() > 0)
                return Convert.ToInt32(rows[0]["SubSectorID"]);
            else
                return -1;
        }

        public int GetSectorID(int SubSectorID)
        {
            DataRow[] rows = SubSectorsDT.Select("SubSectorID = " + SubSectorID);
            if (rows.Count() > 0)
                return Convert.ToInt32(rows[0]["SectorID"]);
            else
                return -1;
        }

        public int GetFactoryID(int SectorID)
        {
            DataRow[] rows = SectorsDT.Select("SectorID = " + SectorID);
            if (rows.Count() > 0)
                return Convert.ToInt32(rows[0]["FactoryID"]);
            else
                return -1;
        }

        private void AddMainParameterNewRow(string ColumnName, string Number, string Name)
        {
            DataRow NewRow = MainParametersDT.NewRow();
            NewRow["ColumnName"] = ColumnName;
            NewRow["Number"] = Number;
            NewRow["Name"] = Name;
            MainParametersDT.Rows.Add(NewRow);
        }

        private void AddMachinesSummaryNewRow(string Name, string Value)
        {
            DataRow NewRow = MachinesSummaryDT.NewRow();
            NewRow["Name"] = Name;
            NewRow["Value"] = Value;
            MachinesSummaryDT.Rows.Add(NewRow);
        }

        private void FillMainParameter(string ColumnName, object Value)
        {
            DataRow[] rows = MainParametersDT.Select("ColumnName = '" + ColumnName + "'");
            if (rows.Count() > 0)
            {
                rows[0]["Value"] = Value;
            }
        }

        private void CreateMainParameters()
        {
            AddMainParameterNewRow("TechPasportName", "1", "Наименование");
            AddMainParameterNewRow("MachineFunction", "2", "Назначение");
            AddMainParameterNewRow("Producer", "3", "Производитель");
            AddMainParameterNewRow("ProducersContacts", "4", "Контакты");
            AddMainParameterNewRow("ProducersRepresentative", "5", "Представитель произв-ля");
            AddMainParameterNewRow("RepresentativesContacts", "6", "Контакты представителя");
            AddMainParameterNewRow("SerialNumber", "7", "Сер.№");
            AddMainParameterNewRow("Year", "8", "Год выпуска");
            AddMainParameterNewRow("Commissioning", "9", "Дата ввода в экспл-цию");
            AddMainParameterNewRow("Productivity", "10", "Производительность");
            AddMainParameterNewRow("ConsumedPower", "11", "Потребляемая мощность");
            AddMainParameterNewRow("Voltage", "12", "Напряжение подключения, В");
            AddMainParameterNewRow("CompressedAir", "13", "Сжатый воздух:");
            AddMainParameterNewRow("OperatingPressure", string.Empty, "рабочее давление, bar");
            AddMainParameterNewRow("CompressedAirConsumption", string.Empty, "расход, л/мин");
            AddMainParameterNewRow("Aspiration", "14", "Аспирация:");
            AddMainParameterNewRow("AspirationCapacity", string.Empty, "объём, м3/час");
            AddMainParameterNewRow("AspirationFlowRate", string.Empty, "скорость потока, м/с");
            AddMainParameterNewRow("Firm", "15", "Участок");
            AddMainParameterNewRow("InventoryNumber", "16", "Инв.№");
            AddMainParameterNewRow("MachineOperator", "17", "Ответственный станочник");
            AddMainParameterNewRow("Technician", "18", "Ответственный техник");
        }

        public void GetMainParameters()
        {
            int MachineID = Convert.ToInt32(((DataRowView)MachinesBS.Current).Row["MachineID"]);
            FillMainParameter("TechPasportName", ((DataRowView)MachinesBS.Current).Row["TechPasportName"]);
            FillMainParameter("MachineFunction", ((DataRowView)MachinesBS.Current).Row["MachineFunction"]);
            FillMainParameter("Producer", ((DataRowView)MachinesBS.Current).Row["Producer"]);
            FillMainParameter("ProducersContacts", ((DataRowView)MachinesBS.Current).Row["ProducersContacts"]);
            FillMainParameter("ProducersRepresentative", ((DataRowView)MachinesBS.Current).Row["ProducersRepresentative"]);
            FillMainParameter("RepresentativesContacts", ((DataRowView)MachinesBS.Current).Row["RepresentativesContacts"]);
            FillMainParameter("SerialNumber", ((DataRowView)MachinesBS.Current).Row["SerialNumber"]);
            FillMainParameter("Year", ((DataRowView)MachinesBS.Current).Row["Year"]);
            FillMainParameter("Commissioning", ((DataRowView)MachinesBS.Current).Row["Commissioning"]);
            FillMainParameter("Productivity", ((DataRowView)MachinesBS.Current).Row["Productivity"]);
            FillMainParameter("ConsumedPower", ((DataRowView)MachinesBS.Current).Row["ConsumedPower"]);
            FillMainParameter("Voltage", ((DataRowView)MachinesBS.Current).Row["Voltage"]);
            bool b1 = int.TryParse(((DataRowView)MachinesBS.Current).Row["OperatingPressure"].ToString(), out int i1);
            bool b2 = int.TryParse(((DataRowView)MachinesBS.Current).Row["CompressedAirConsumption"].ToString(), out int i2);

            if (((DataRowView)MachinesBS.Current).Row["OperatingPressure"] != null
                && ((DataRowView)MachinesBS.Current).Row["OperatingPressure"] != DBNull.Value
                && b1 && Convert.ToInt32(i1) == -1
                && ((DataRowView)MachinesBS.Current).Row["CompressedAirConsumption"] != null
                && ((DataRowView)MachinesBS.Current).Row["CompressedAirConsumption"] != DBNull.Value
                && b2 && Convert.ToInt32(i2) == -1)
                FillMainParameter("CompressedAir", "Нет");

            else
            {
                FillMainParameter("OperatingPressure", ((DataRowView)MachinesBS.Current).Row["OperatingPressure"]);
                FillMainParameter("CompressedAirConsumption", ((DataRowView)MachinesBS.Current).Row["CompressedAirConsumption"]);
            }

            i1 = 0;
            i2 = 0;
            b1 = int.TryParse(((DataRowView)MachinesBS.Current).Row["AspirationCapacity"].ToString(), out i1);
            b2 = int.TryParse(((DataRowView)MachinesBS.Current).Row["AspirationFlowRate"].ToString(), out i2);

            if (((DataRowView)MachinesBS.Current).Row["AspirationCapacity"] != null
                && ((DataRowView)MachinesBS.Current).Row["AspirationCapacity"] != DBNull.Value
                && b1 && Convert.ToInt32(i1) == -1
                && ((DataRowView)MachinesBS.Current).Row["AspirationFlowRate"] != null
                && ((DataRowView)MachinesBS.Current).Row["AspirationFlowRate"] != DBNull.Value
                && b2 && Convert.ToInt32(i2) == -1)
                FillMainParameter("Aspiration", "Нет");

            else
            {
                FillMainParameter("AspirationCapacity", ((DataRowView)MachinesBS.Current).Row["AspirationCapacity"]);
                FillMainParameter("AspirationFlowRate", ((DataRowView)MachinesBS.Current).Row["AspirationFlowRate"]);
            }
            FillMainParameter("Firm", ((DataRowView)MachinesBS.Current).Row["Firm"]);
            FillMainParameter("InventoryNumber", ((DataRowView)MachinesBS.Current).Row["InventoryNumber"]);
            FillMainParameter("MachineOperator", ((DataRowView)MachinesBS.Current).Row["MachineOperator"]);
            FillMainParameter("Technician", ((DataRowView)MachinesBS.Current).Row["Technician"]);
            MainParametersBS.MoveFirst();
        }

        public void PreSaveMainParameters()
        {
            bool NeedSetAspiration = false;
            bool NeedSetCompressedAir = false;
            string ColumnName = string.Empty;

            for (int i = 0; i < MainParametersDT.Rows.Count; i++)
            {
                ColumnName = MainParametersDT.Rows[i]["ColumnName"].ToString();
                if (ColumnName == "Aspiration")
                {
                    if (MainParametersDT.Rows[i]["Value"] != DBNull.Value
                        && MainParametersDT.Rows[i]["Value"].ToString().Equals("Нет", StringComparison.InvariantCultureIgnoreCase))
                    {
                        NeedSetAspiration = true;
                        ((DataRowView)MachinesBS.Current).Row["AspirationCapacity"] = -1;
                        ((DataRowView)MachinesBS.Current).Row["AspirationFlowRate"] = -1;
                    }
                    continue;
                }
                if (ColumnName == "CompressedAir")
                {
                    if (MainParametersDT.Rows[i]["Value"] != DBNull.Value
                        && MainParametersDT.Rows[i]["Value"].ToString().Equals("Нет", StringComparison.InvariantCultureIgnoreCase))
                    {
                        NeedSetCompressedAir = true;
                        ((DataRowView)MachinesBS.Current).Row["OperatingPressure"] = -1;
                        ((DataRowView)MachinesBS.Current).Row["CompressedAirConsumption"] = -1;
                    }
                    continue;
                }
                if (ColumnName != string.Empty && MainParametersDT.Rows[i]["Value"] != ((DataRowView)MachinesBS.Current).Row[ColumnName])
                {
                    if (NeedSetAspiration && (ColumnName == "AspirationCapacity" || ColumnName == "AspirationFlowRate"))
                        continue;
                    if (NeedSetCompressedAir && (ColumnName == "OperatingPressure" || ColumnName == "CompressedAirConsumption"))
                        continue;
                    ((DataRowView)MachinesBS.Current).Row[ColumnName] = MainParametersDT.Rows[i]["Value"];
                }
                if (ColumnName == "ConsumedPower")
                    ((DataRowView)MachinesBS.Current).Row["ConsumedPower"] = MainParametersDT.Rows[i]["Value"].ToString().Replace(".", ",");
                if (ColumnName == "CompressedAirConsumption")
                    ((DataRowView)MachinesBS.Current).Row["CompressedAirConsumption"] = MainParametersDT.Rows[i]["Value"].ToString().Replace(".", ",");
                if (ColumnName == "AspirationCapacity")
                    ((DataRowView)MachinesBS.Current).Row["AspirationCapacity"] = MainParametersDT.Rows[i]["Value"].ToString().Replace(".", ",");
            }
        }

        public void PreSaveTechnicalSpecification(bool bEdit, string Group)
        {
            if (bEdit)
            {
                for (int i = 0; i < TempTechnicalSpecificationDT.Rows.Count; i++)
                {
                    int GroupNumber = Convert.ToInt32(TempTechnicalSpecificationDT.Rows[i]["GroupNumber"]);
                    Group = TempTechnicalSpecificationDT.Rows[i]["Group"].ToString();
                    DataRow[] rows = TechnicalSpecificationDT.Select("GroupNumber=" + GroupNumber);
                    foreach (DataRow item in rows)
                    {
                        string Group1 = item["Group"].ToString();
                        if (!item["Group"].Equals(Group))
                            item["Group"] = Group;
                    }
                }
            }
            else
            {
                DataRow NewRow = TechnicalSpecificationDT.NewRow();
                NewRow["GroupDisplay"] = Group;
                NewRow["Group"] = Group;
                TechnicalSpecificationDT.Rows.Add(NewRow);
            }
        }

        public DataView S(int MachineSpareGroupID, MachineFileTypes Type)
        {
            DataView DV = new DataView(SparesDT, "MachineSpareGroupID = " + MachineSpareGroupID + " AND Type = " + Convert.ToInt32(Type), "Name", DataViewRowState.CurrentRows);
            return DV;
        }

        public DataGridViewComboBoxColumn MeasureColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "MeasureColumn",
                    HeaderText = "Ед.изм.",
                    DataPropertyName = "MeasureID",
                    DataSource = new DataView(MeasuresDT, string.Empty, string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "MeasureID",
                    DisplayMember = "Measure",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    Width = 75,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn SpareGroupColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "SpareGroupColumn",
                HeaderText = "Группа",
                DataPropertyName = "MachineSpareGroupID",
                DataSource = new DataView(SpareGroupsDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "MachineSpareGroupID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public DataGridViewComboBoxColumn SpareColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "SpareColumn",
                HeaderText = "Модель",
                DataPropertyName = "MachineSpareID",
                DataSource = new DataView(SparesDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "MachineSpareID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public DataGridViewComboBoxColumn AspirationUnitColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "UnitColumn",
                HeaderText = "Установлено",
                DataPropertyName = "UnitID",
                DataSource = new DataView(AspirationUnitsDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "UnitID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public DataGridViewComboBoxColumn MechanicsUnitColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "UnitColumn",
                HeaderText = "Установлено",
                DataPropertyName = "UnitID",
                DataSource = new DataView(MechanicsUnitsDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "UnitID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public DataGridViewComboBoxColumn ElectricsUnitColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "UnitColumn",
                HeaderText = "Установлено",
                DataPropertyName = "UnitID",
                DataSource = new DataView(ElectricsUnitsDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "UnitID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public DataGridViewComboBoxColumn HydraulicsUnitColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "UnitColumn",
                HeaderText = "Установлено",
                DataPropertyName = "UnitID",
                DataSource = new DataView(HydraulicsUnitsDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "UnitID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public DataGridViewComboBoxColumn PneumaticsUnitColumn(MachineFileTypes Type)
        {
            DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
            {
                Name = "UnitColumn",
                HeaderText = "Установлено",
                DataPropertyName = "UnitID",
                DataSource = new DataView(PneumaticsUnitsDT, "Type = " + Convert.ToInt32(Type), string.Empty, DataViewRowState.CurrentRows),
                ValueMember = "UnitID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            return Column;
        }

        public void FilterDetailFiles(int MachineDetailID, MachineFileTypes Type)
        {
            string filter = string.Empty;
            DataRow[] rows = TempMachineDocumentsDT.Select("MachineDetailID=" + MachineDetailID + " AND FileType = " + Convert.ToInt32(Type));

            if (rows.Count() > 0)
            {
                foreach (DataRow item in rows)
                {
                    filter += item["MachineDocumentID"] + ",";
                }
                filter = filter.Substring(0, filter.Length - 1) + ")";
            }
            else
                filter = "0)";
            switch (Type)
            {
                case MachineFileTypes.MachineFoto:
                    break;
                case MachineFileTypes.MachineAspiration:
                    break;
                case MachineFileTypes.MachineMechanics:
                    break;
                case MachineFileTypes.MachineTechnical:
                    break;
                case MachineFileTypes.MachineElectrics:
                    break;
                case MachineFileTypes.MachinePneumatics:
                    break;
                case MachineFileTypes.MachineHydraulics:
                    break;
                case MachineFileTypes.EquipmentTools:
                    EquipmentFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.AspirationDetails:
                    AspirationDetailFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.MechanicsDetails:
                    MechanicsDetailFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.ElectricsDetails:
                    ElectricsDetailFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.HydraulicsDetails:
                    HydraulicsDetailFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.PneumaticsDetails:
                    PneumaticsDetailFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.ExploitationTools:
                    ExploitationToolsFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.RepairTools:
                    RepairToolsFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.ServiceTools:
                    ServiceToolsFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                case MachineFileTypes.Lubricant:
                    LubricantFilesBS.Filter = "MachineDocumentID IN (" + filter;
                    break;
                default:
                    break;
            }
        }

        public void FilterSpares(int MachineSpareGroupID, MachineFileTypes Type)
        {
            switch (Type)
            {
                case MachineFileTypes.MachineAspiration:
                    AspirationSparesBS.Filter = "MachineSpareGroupID=" + MachineSpareGroupID;
                    break;
                case MachineFileTypes.MachineMechanics:
                    MechanicsSparesBS.Filter = "MachineSpareGroupID=" + MachineSpareGroupID;
                    break;
                case MachineFileTypes.MachineTechnical:
                    AspirationSparesBS.Filter = "MachineSpareGroupID=" + MachineSpareGroupID;
                    break;
                case MachineFileTypes.MachineElectrics:
                    ElectricsSparesBS.Filter = "MachineSpareGroupID=" + MachineSpareGroupID;
                    break;
                case MachineFileTypes.MachinePneumatics:
                    PneumaticsSparesBS.Filter = "MachineSpareGroupID=" + MachineSpareGroupID;
                    break;
                case MachineFileTypes.MachineHydraulics:
                    HydraulicsSparesBS.Filter = "MachineSpareGroupID=" + MachineSpareGroupID;
                    break;
                default:
                    break;
            }
        }

        public void FilterSectors(int FactoryID)
        {
            SectorsBS.Filter = "FactoryID = " + FactoryID;
            SectorsBS.MoveFirst();
        }

        public void FilterSubSectors(int SectorID)
        {
            SubSectorsBS.Filter = "SectorID = " + SectorID;
            SubSectorsBS.MoveFirst();
        }

        #region Check data

        public bool HasAspiration
        {
            get { return AspirationBS.Count > 0; }
        }

        public bool HasMechanics
        {
            get { return MechanicsBS.Count > 0; }
        }

        public bool HasElectrics
        {
            get { return ElectricsBS.Count > 0; }
        }

        public bool HasHydraulics
        {
            get { return HydraulicsBS.Count > 0; }
        }

        public bool HasPneumatics
        {
            get { return PneumaticsBS.Count > 0; }
        }

        public bool HasMachines
        {
            get { return MachinesBS.Count > 0; }
        }

        public bool HasMachineFoto
        {
            get
            {
                DataRow[] rows = MachineDocumentsDT.Select("FileType = " + Convert.ToInt32(MachineFileTypes.MachineFoto));
                return rows.Count() > 0;
            }
        }

        public bool HasAspirationFiles
        {
            get
            {
                return AspirationFilesBS.Count > 0;
            }
        }

        public bool HasMechanicsFiles
        {
            get
            {
                return MechanicsFilesBS.Count > 0;
            }
        }

        public bool HasElectricsFiles
        {
            get
            {
                return ElectricsFilesBS.Count > 0;
            }
        }

        public bool HasHydraulicsFiles
        {
            get
            {
                return HydraulicsFilesBS.Count > 0;
            }
        }

        public bool HasPneumaticsFiles
        {
            get
            {
                return PneumaticsFilesBS.Count > 0;
            }
        }

        public bool HasAspirationDetailFiles
        {
            get
            {
                return AspirationDetailFilesBS.Count > 0;
            }
        }

        public bool HasMechanicsDetailFiles
        {
            get
            {
                return MechanicsDetailFilesBS.Count > 0;
            }
        }

        public bool HasElectricsDetailFiles
        {
            get
            {
                return ElectricsDetailFilesBS.Count > 0;
            }
        }

        public bool HasHydraulicsDetailFiles
        {
            get
            {
                return HydraulicsDetailFilesBS.Count > 0;
            }
        }

        public bool HasPneumaticsDetailFiles
        {
            get
            {
                return PneumaticsDetailFilesBS.Count > 0;
            }
        }

        public bool HasExploitationToolsFiles
        {
            get
            {
                return ExploitationToolsFilesBS.Count > 0;
            }
        }

        public bool HasRepairToolsFiles
        {
            get
            {
                return RepairToolsFilesBS.Count > 0;
            }
        }

        public bool HasServiceToolsFiles
        {
            get
            {
                return ServiceToolsFilesBS.Count > 0;
            }
        }

        public bool HasLubricantFiles
        {
            get
            {
                return LubricantFilesBS.Count > 0;
            }
        }

        public bool HasEquipmentFiles
        {
            get
            {
                return EquipmentFilesBS.Count > 0;
            }
        }

        public bool HasMachinesStructure
        {
            get
            {
                return TechnicalFilesBS.Count > 0;
            }
        }

        public bool HasAspirationDetails
        {
            get
            {
                return AspirationDetailsBS.Count > 0;
            }
        }

        public bool HasMechanicsDetails
        {
            get
            {
                return MechanicsDetailsBS.Count > 0;
            }
        }

        public bool HasElectricsDetails
        {
            get
            {
                return ElectricsDetailsBS.Count > 0;
            }
        }

        public bool HasHydraulicsDetails
        {
            get
            {
                return HydraulicsDetailsBS.Count > 0;
            }
        }

        public bool HasPneumaticsDetails
        {
            get
            {
                return PneumaticsDetailsBS.Count > 0;
            }
        }

        public bool HasExploitationTools
        {
            get
            {
                return ExploitationToolsBS.Count > 0;
            }
        }

        public bool HasRepairTools
        {
            get
            {
                return RepairToolsBS.Count > 0;
            }
        }

        public bool HasServiceTools
        {
            get
            {
                return ServiceToolsBS.Count > 0;
            }
        }

        public bool HasLubricant
        {
            get
            {
                return LubricantBS.Count > 0;
            }
        }

        public bool HasEquipment
        {
            get
            {
                return EquipmentBS.Count > 0;
            }
        }

        public bool HasAspirationUnits
        {
            get
            {
                return AspirationUnitsBS.Count > 0;
            }
        }

        public bool HasMechanicsUnits
        {
            get
            {
                return MechanicsUnitsBS.Count > 0;
            }
        }

        public bool HasElectricsUnits
        {
            get
            {
                return ElectricsUnitsBS.Count > 0;
            }
        }

        public bool HasHydraulicsUnits
        {
            get
            {
                return HydraulicsUnitsBS.Count > 0;
            }
        }

        public bool HasPneumaticsUnits
        {
            get
            {
                return PneumaticsUnitsBS.Count > 0;
            }
        }

        public bool HasTechnicalSpecification
        {
            get
            {
                return TechnicalSpecificationBS.Count > 0;
            }
        }

        #endregion

        #region Current values

        public int CurrentMachineFoto
        {
            get
            {
                DataRow[] rows = MachineDocumentsDT.Select("FileType = " + Convert.ToInt32(MachineFileTypes.MachineFoto));
                if (rows.Count() > 0)
                {
                    return Convert.ToInt32(rows[0]["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public int CurrentAspirationFile
        {
            get
            {
                if (AspirationFilesBS.Current != null && ((DataRowView)AspirationFilesBS.Current).Row["MachineDocumentID"] != DBNull.Value)
                {
                    return Convert.ToInt32(((DataRowView)AspirationFilesBS.Current).Row["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public int CurrentMechanicsFile
        {
            get
            {
                if (MechanicsFilesBS.Current != null && ((DataRowView)MechanicsFilesBS.Current).Row["MachineDocumentID"] != DBNull.Value)
                {
                    return Convert.ToInt32(((DataRowView)MechanicsFilesBS.Current).Row["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public int CurrentElectricsFile
        {
            get
            {
                if (ElectricsFilesBS.Current != null && ((DataRowView)ElectricsFilesBS.Current).Row["MachineDocumentID"] != DBNull.Value)
                {
                    return Convert.ToInt32(((DataRowView)ElectricsFilesBS.Current).Row["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public int CurrentHydraulicsFile
        {
            get
            {
                if (HydraulicsFilesBS.Current != null && ((DataRowView)HydraulicsFilesBS.Current).Row["MachineDocumentID"] != DBNull.Value)
                {
                    return Convert.ToInt32(((DataRowView)HydraulicsFilesBS.Current).Row["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public int CurrentPneumaticsFile
        {
            get
            {
                if (PneumaticsFilesBS.Current != null && ((DataRowView)PneumaticsFilesBS.Current).Row["MachineDocumentID"] != DBNull.Value)
                {
                    return Convert.ToInt32(((DataRowView)PneumaticsFilesBS.Current).Row["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public int CurrentMachinesStructure
        {
            get
            {
                if (TechnicalFilesBS.Current != null && ((DataRowView)TechnicalFilesBS.Current).Row["MachineDocumentID"] != DBNull.Value)
                {
                    return Convert.ToInt32(((DataRowView)TechnicalFilesBS.Current).Row["MachineDocumentID"]);
                }
                return -1;
            }
        }

        public string CurrentAspiration
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["MachineAspiration"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["MachineAspiration"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["MachineAspiration"] = value;
            }
        }

        public string CurrentMechanics
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["MachineMechanics"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["MachineMechanics"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["MachineMechanics"] = value;
            }
        }

        public string CurrentElectrics
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["MachineElectrics"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["MachineElectrics"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["MachineElectrics"] = value;
            }
        }

        public string CurrentHydraulics
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["MachineHydraulics"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["MachineHydraulics"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["MachineHydraulics"] = value;
            }
        }

        public string CurrentPneumatics
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["MachinePneumatics"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["MachinePneumatics"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["MachinePneumatics"] = value;
            }
        }

        public string CurrentAspirationNotes
        {
            get
            {
                if (AspirationBS.Current != null && ((DataRowView)AspirationBS.Current).Row["Notes"] != DBNull.Value)
                {
                    return ((DataRowView)AspirationBS.Current).Row["Notes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)AspirationBS.Current).Row["Notes"] = value;
            }
        }

        public string CurrentMechanicsNotes
        {
            get
            {
                if (MechanicsBS.Current != null && ((DataRowView)MechanicsBS.Current).Row["Notes"] != DBNull.Value)
                {
                    return ((DataRowView)MechanicsBS.Current).Row["Notes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MechanicsBS.Current).Row["Notes"] = value;
            }
        }

        public string CurrentElectricsNotes
        {
            get
            {
                if (ElectricsBS.Current != null && ((DataRowView)ElectricsBS.Current).Row["Notes"] != DBNull.Value)
                {
                    return ((DataRowView)ElectricsBS.Current).Row["Notes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)ElectricsBS.Current).Row["Notes"] = value;
            }
        }

        public string CurrentHydraulicsNotes
        {
            get
            {
                if (HydraulicsBS.Current != null && ((DataRowView)HydraulicsBS.Current).Row["Notes"] != DBNull.Value)
                {
                    return ((DataRowView)HydraulicsBS.Current).Row["Notes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)HydraulicsBS.Current).Row["Notes"] = value;
            }
        }

        public string CurrentPneumaticsNotes
        {
            get
            {
                if (PneumaticsBS.Current != null && ((DataRowView)PneumaticsBS.Current).Row["Notes"] != DBNull.Value)
                {
                    return ((DataRowView)PneumaticsBS.Current).Row["Notes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)PneumaticsBS.Current).Row["Notes"] = value;
            }
        }

        public string CurrentAspirationDescription
        {
            get
            {
                if (AspirationBS.Current != null && ((DataRowView)AspirationBS.Current).Row["Description"] != DBNull.Value)
                {
                    return ((DataRowView)AspirationBS.Current).Row["Description"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)AspirationBS.Current).Row["Description"] = value;
            }
        }

        public string CurrentMechanicsDescription
        {
            get
            {
                if (MechanicsBS.Current != null && ((DataRowView)MechanicsBS.Current).Row["Description"] != DBNull.Value)
                {
                    return ((DataRowView)MechanicsBS.Current).Row["Description"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MechanicsBS.Current).Row["Description"] = value;
            }
        }

        public string CurrentElectricsDescription
        {
            get
            {
                if (ElectricsBS.Current != null && ((DataRowView)ElectricsBS.Current).Row["Description"] != DBNull.Value)
                {
                    return ((DataRowView)ElectricsBS.Current).Row["Description"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)ElectricsBS.Current).Row["Description"] = value;
            }
        }

        public string CurrentHydraulicsDescription
        {
            get
            {
                if (HydraulicsBS.Current != null && ((DataRowView)HydraulicsBS.Current).Row["Description"] != DBNull.Value)
                {
                    return ((DataRowView)HydraulicsBS.Current).Row["Description"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)HydraulicsBS.Current).Row["Description"] = value;
            }
        }

        public string CurrentPneumaticsDescription
        {
            get
            {
                if (PneumaticsBS.Current != null && ((DataRowView)PneumaticsBS.Current).Row["Description"] != DBNull.Value)
                {
                    return ((DataRowView)PneumaticsBS.Current).Row["Description"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)PneumaticsBS.Current).Row["Description"] = value;
            }
        }

        public string CurrentTechnicalSpecification
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["TechnicalSpecification"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["TechnicalSpecification"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["TechnicalSpecification"] = value;
            }
        }

        public string CurrentSizes
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["Sizes"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["Sizes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["Sizes"] = value;
            }
        }

        public string CurrentEquipmentNotes
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["EquipmentNotes"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["EquipmentNotes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["EquipmentNotes"] = value;
            }
        }

        public string CurrentTechnicalNotes
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["TechnicalNotes"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["TechnicalNotes"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["TechnicalNotes"] = value;
            }
        }

        public string CurrentPermanentWorks
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["PermanentWorks"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["PermanentWorks"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["PermanentWorks"] = value;
            }
        }

        public string CurrentWeight
        {
            get
            {
                if (MachinesBS.Current != null && ((DataRowView)MachinesBS.Current).Row["Weight"] != DBNull.Value)
                {
                    return ((DataRowView)MachinesBS.Current).Row["Weight"].ToString();
                }
                return string.Empty;
            }
            set
            {
                ((DataRowView)MachinesBS.Current).Row["Weight"] = value;
            }
        }

        #endregion

        #region Lists

        public BindingSource FindMachinesList
        {
            get { return FindMachinesBS; }
        }

        public BindingSource FactoryList
        {
            get { return FactoryBS; }
        }

        public BindingSource SectorsList
        {
            get { return SectorsBS; }
        }

        public BindingSource SubSectorsList
        {
            get { return SubSectorsBS; }
        }

        public BindingSource MachinesList
        {
            get { return MachinesBS; }
        }

        public BindingSource MainParametersList
        {
            get { return MainParametersBS; }
        }

        public BindingSource MachinesStructureList
        {
            get { return TechnicalFilesBS; }
        }

        public BindingSource OperatingInstructionsList
        {
            get { return OperatingInstructionsBS; }
        }

        public BindingSource ServiceInstructionsList
        {
            get { return ServiceInstructionsBS; }
        }

        public BindingSource LaborProtInstructionsList
        {
            get { return LaborProtInstructionsBS; }
        }

        public BindingSource JournalList
        {
            get { return JournalBS; }
        }

        public BindingSource AdmissionsList
        {
            get { return AdmissionsBS; }
        }

        public BindingSource MachinesSummaryList
        {
            get { return MachinesSummaryBS; }
        }

        public BindingSource TempMachinesList
        {
            get { return TempMachinesBS; }
        }

        public BindingSource AspirationFilesList
        {
            get { return AspirationFilesBS; }
        }

        public BindingSource MechanicsFilesList
        {
            get { return MechanicsFilesBS; }
        }

        public BindingSource ElectricsFilesList
        {
            get { return ElectricsFilesBS; }
        }

        public BindingSource HydraulicsFilesList
        {
            get { return HydraulicsFilesBS; }
        }

        public BindingSource PneumaticsFilesList
        {
            get { return PneumaticsFilesBS; }
        }

        public BindingSource AspirationDetailFilesList
        {
            get { return AspirationDetailFilesBS; }
        }

        public BindingSource MechanicsDetailFilesList
        {
            get { return MechanicsDetailFilesBS; }
        }

        public BindingSource ElectricsDetailFilesList
        {
            get { return ElectricsDetailFilesBS; }
        }

        public BindingSource HydraulicsDetailFilesList
        {
            get { return HydraulicsDetailFilesBS; }
        }

        public BindingSource PneumaticsDetailFilesList
        {
            get { return PneumaticsDetailFilesBS; }
        }

        public BindingSource ExploitationToolsFilesList
        {
            get { return ExploitationToolsFilesBS; }
        }

        public BindingSource RepairToolsFilesList
        {
            get { return RepairToolsFilesBS; }
        }

        public BindingSource ServiceToolsFilesList
        {
            get { return ServiceToolsFilesBS; }
        }

        public BindingSource LubricantFilesList
        {
            get { return LubricantFilesBS; }
        }

        public BindingSource EquipmentFilesList
        {
            get { return EquipmentFilesBS; }
        }

        public BindingSource AspirationSpareGroupsList
        {
            get { return AspirationSpareGroupsBS; }
        }

        public BindingSource MechanicsSpareGroupsList
        {
            get { return MechanicsSpareGroupsBS; }
        }

        public BindingSource ElectricsSpareGroupsList
        {
            get { return ElectricsSpareGroupsBS; }
        }

        public BindingSource HydraulicsSpareGroupsList
        {
            get { return HydraulicsSpareGroupsBS; }
        }

        public BindingSource PneumaticsSpareGroupsList
        {
            get { return PneumaticsSpareGroupsBS; }
        }

        public BindingSource AspirationSparesList
        {
            get { return AspirationSparesBS; }
        }

        public BindingSource MechanicsSparesList
        {
            get { return MechanicsSparesBS; }
        }

        public BindingSource ElectricsSparesList
        {
            get { return ElectricsSparesBS; }
        }

        public BindingSource HydraulicsSparesList
        {
            get { return HydraulicsSparesBS; }
        }

        public BindingSource PneumaticsSparesList
        {
            get { return PneumaticsSparesBS; }
        }

        public BindingSource AspirationSparesOnStockList
        {
            get { return AspirationSparesOnStockBS; }
        }

        public BindingSource MechanicsSparesOnStockList
        {
            get { return MechanicsSparesOnStockBS; }
        }

        public BindingSource ElectricsSparesOnStockList
        {
            get { return ElectricsSparesOnStockBS; }
        }

        public BindingSource HydraulicsSparesOnStockList
        {
            get { return HydraulicsSparesOnStockBS; }
        }

        public BindingSource PneumaticsSparesOnStockList
        {
            get { return PneumaticsSparesOnStockBS; }
        }

        public BindingSource AspirationDetailsList
        {
            get { return AspirationDetailsBS; }
        }

        public BindingSource MechanicsDetailsList
        {
            get { return MechanicsDetailsBS; }
        }

        public BindingSource ElectricsDetailsList
        {
            get { return ElectricsDetailsBS; }
        }

        public BindingSource HydraulicsDetailsList
        {
            get { return HydraulicsDetailsBS; }
        }

        public BindingSource PneumaticsDetailsList
        {
            get { return PneumaticsDetailsBS; }
        }

        public BindingSource ExploitationToolsList
        {
            get { return ExploitationToolsBS; }
        }

        public BindingSource RepairToolsList
        {
            get { return RepairToolsBS; }
        }

        public BindingSource ServiceToolsList
        {
            get { return ServiceToolsBS; }
        }

        public BindingSource LubricantList
        {
            get { return LubricantBS; }
        }

        public BindingSource EquipmentList
        {
            get { return EquipmentBS; }
        }

        public BindingSource AspirationUnitsList
        {
            get { return AspirationUnitsBS; }
        }

        public BindingSource MechanicsUnitsList
        {
            get { return MechanicsUnitsBS; }
        }

        public BindingSource ElectricsUnitsList
        {
            get { return ElectricsUnitsBS; }
        }

        public BindingSource HydraulicsUnitsList
        {
            get { return HydraulicsUnitsBS; }
        }

        public BindingSource PneumaticsUnitsList
        {
            get { return PneumaticsUnitsBS; }
        }

        public BindingSource TechnicalSpecificationList
        {
            get { return TechnicalSpecificationBS; }
        }

        public BindingSource TempTechnicalSpecificationList
        {
            get { return TempTechnicalSpecificationBS; }
        }

        #endregion

        #region Get xml

        public string GetAspiration()
        {
            StringWriter SW = new StringWriter();
            AspirationDT.WriteXml(SW);

            return SW.ToString();
        }

        public string GetMechanics()
        {
            StringWriter SW = new StringWriter();
            MechanicsDT.WriteXml(SW);

            return SW.ToString();
        }

        public string GetElectrics()
        {
            StringWriter SW = new StringWriter();
            ElectricsDT.WriteXml(SW);

            return SW.ToString();
        }

        public string GetHydraulics()
        {
            StringWriter SW = new StringWriter();
            HydraulicsDT.WriteXml(SW);

            return SW.ToString();
        }

        public string GetPneumatics()
        {
            StringWriter SW = new StringWriter();
            PneumaticsDT.WriteXml(SW);

            return SW.ToString();
        }

        public string GetTechnicalSpecification()
        {
            DataTable DT = TechnicalSpecificationDT.Copy();
            DT.Columns.Remove("GroupDisplay");
            StringWriter SW = new StringWriter();
            DT.WriteXml(SW);

            return SW.ToString();
        }

        #endregion

        #region Read xml

        public void ReadAspiration(string Aspiration)
        {
            AspirationDT.Clear();

            using (StringReader SR = new StringReader(Aspiration))
            {
                AspirationDT.ReadXml(SR);
            }
        }

        public void ReadMechanics(string Mechanics)
        {
            MechanicsDT.Clear();

            using (StringReader SR = new StringReader(Mechanics))
            {
                MechanicsDT.ReadXml(SR);
            }
        }

        public void ReadElectrics(string Electrics)
        {
            ElectricsDT.Clear();

            using (StringReader SR = new StringReader(Electrics))
            {
                ElectricsDT.ReadXml(SR);
            }
        }

        public void ReadHydraulics(string Hydraulics)
        {
            HydraulicsDT.Clear();

            using (StringReader SR = new StringReader(Hydraulics))
            {
                HydraulicsDT.ReadXml(SR);
            }
        }

        public void ReadPneumatics(string PneumaticsUnits)
        {
            PneumaticsDT.Clear();

            using (StringReader SR = new StringReader(PneumaticsUnits))
            {
                PneumaticsDT.ReadXml(SR);
            }
        }

        public void ReadTechnicalSpecification(string TechnicalSpecification)
        {
            TechnicalSpecificationDT.Clear();

            using (StringReader SR = new StringReader(TechnicalSpecification))
            {
                TechnicalSpecificationDT.ReadXml(SR);
            }

            DataTable DT = new DataTable();
            using (DataView DV = new DataView(TechnicalSpecificationDT.Copy()))
            {
                DV.Sort = "Group, Name";
                DT = DV.ToTable();
            }
            TechnicalSpecificationDT.Clear();
            //TechnicalSpecificationDT = DT.Copy();
            foreach (DataRow item in DT.Rows)
                TechnicalSpecificationDT.Rows.Add(item.ItemArray);

            int GroupNumber = 0;
            if (TechnicalSpecificationDT.Rows.Count > 0)
            {
                TechnicalSpecificationDT.Rows[0]["GroupDisplay"] = TechnicalSpecificationDT.Rows[0]["Group"];
                TechnicalSpecificationDT.Rows[0]["GroupNumber"] = GroupNumber;
                for (int i = 1; i < TechnicalSpecificationDT.Rows.Count; i++)
                {
                    if (TechnicalSpecificationDT.Rows[i]["Group"].ToString() != TechnicalSpecificationDT.Rows[i - 1]["Group"].ToString())
                    {
                        GroupNumber++;
                        TechnicalSpecificationDT.Rows[i]["GroupDisplay"] = TechnicalSpecificationDT.Rows[i]["Group"];
                    }
                    TechnicalSpecificationDT.Rows[i]["GroupNumber"] = GroupNumber;
                }
            }
            TechnicalSpecificationBS.Sort = "";
        }

        #endregion

        #region Clear

        public void ClearAspiration()
        {
            AspirationDT.Clear();
        }

        public void ClearMechanics()
        {
            MechanicsDT.Clear();
        }

        public void ClearElectrics()
        {
            ElectricsDT.Clear();
        }

        public void ClearHydraulics()
        {
            HydraulicsDT.Clear();
        }

        public void ClearPneumatics()
        {
            PneumaticsDT.Clear();
        }

        public void ClearSpareGroups()
        {
            SpareGroupsDT.Clear();
        }

        public void ClearSpares()
        {
            SparesDT.Clear();
        }

        public void ClearSparesOnStock()
        {
            SparesOnStockDT.Clear();
        }

        public void ClearMachines()
        {
            MachinesDT.Clear();
        }

        public void ClearUnits()
        {
            UnitsDT.Clear();
        }

        public void ClearElectricsDetails()
        {
            ElectricsDetailsDT.Clear();
        }

        public void ClearPneumaticsDetails()
        {
            PneumaticsDetailsDT.Clear();
        }

        public void ClearHydraulicsDetails()
        {
            HydraulicsDetailsDT.Clear();
        }

        public void ClearAspirationDetails()
        {
            AspirationDetailsDT.Clear();
        }

        public void ClearMechanicsDetails()
        {
            MechanicsDetailsDT.Clear();
        }

        public void ClearServiceTools()
        {
            ServiceToolsDT.Clear();
        }

        public void ClearEquipmentTools()
        {
            EquipmentToolsDT.Clear();
        }

        public void ClearLubricant()
        {
            LubricantDT.Clear();
        }

        public void ClearExploitationTools()
        {
            ExploitationToolsDT.Clear();
        }

        public void ClearRepairTools()
        {
            RepairToolsDT.Clear();
        }

        public void ClearElectricsUnits()
        {
            ElectricsUnitsDT.Clear();
        }

        public void ClearPneumaticsUnits()
        {
            PneumaticsUnitsDT.Clear();
        }

        public void ClearHydraulicsUnits()
        {
            HydraulicsUnitsDT.Clear();
        }

        public void ClearAspirationUnits()
        {
            AspirationUnitsDT.Clear();
        }

        public void ClearMechanicsUnits()
        {
            MechanicsUnitsDT.Clear();
        }

        public void ClearMainParameters()
        {
            for (int i = 0; i < MainParametersDT.Rows.Count; i++)
            {
                MainParametersDT.Rows[i]["Value"] = DBNull.Value;
            }
        }

        public void ClearMachineDocuments()
        {
            MachineDocumentsDT.Clear();
        }

        public void ClearMachinesSummary()
        {
            MachinesSummaryDT.Clear();
        }

        public void ClearTechnicalSpecification()
        {
            TechnicalSpecificationDT.Clear();
        }

        #endregion

        #region MachineDocuments

        public void CopyMachineDocuments(int MachineID)
        {
            GetMachineDocuments(MachineID);

            foreach (DataRow Row in TempMachineDocumentsDT.Rows)
            {
                DataRow NewRow = MachineDocumentsDT.NewRow();
                NewRow["MachineDocumentID"] = Row["MachineDocumentID"];
                NewRow["FileType"] = Row["FileType"];
                NewRow["FileName"] = Row["FileName"];
                NewRow["FileSize"] = Row["FileSize"];
                NewRow["Path"] = "server";
                MachineDocumentsDT.Rows.Add(NewRow);
            }
        }

        private void GetMachineDocuments(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachineDocuments WHERE MachineID = " + MachineID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    TempMachineDocumentsDT.Clear();
                    DA.Fill(TempMachineDocumentsDT);
                }
            }
        }

        public bool AttachMachineDocument(DataTable AttachsDT, int MachineID, MachineFileTypes Type, int MachineDetailID = 0)
        {
            if (AttachsDT.Rows.Count == 0)
                return true;

            bool Ok = true;

            //write to ftp
            foreach (DataRow Row in AttachsDT.Rows)
            {
                try
                {
                    string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("MachineDocuments");
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

            if (!Ok)
                return Ok;

            //save to base
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineDocuments",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in AttachsDT.Rows)
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
                            NewRow["FileType"] = Type;
                            NewRow["MachineID"] = MachineID;
                            NewRow["MachineDetailID"] = MachineDetailID;
                            NewRow["FileName"] = Row["FileName"];
                            NewRow["FileSize"] = fi.Length;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            return Ok;
        }

        public void GetMachineDocumentInfo(int MachineDocumentID, ref long FileSize, ref string sFileName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachineDocuments WHERE MachineDocumentID = " + MachineDocumentID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["FileSize"] != DBNull.Value)
                            FileSize = (long)DT.Rows[0]["FileSize"];
                        if (DT.Rows[0]["FileName"] != DBNull.Value)
                            sFileName = DT.Rows[0]["FileName"].ToString();
                    }
                }
            }
        }

        public bool SaveFile(long FileSize, string sFileName, string SaveToPath)
        {
            bool bOk = false;

            string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("MachineDocuments");

            bOk = FM.DownloadFile(sDestFolder + "/" + sFileName, SaveToPath, FileSize, Configs.FTPType);

            return bOk;
        }

        public bool RemoveMachineDocuments(int MachineDocumentID)
        {
            bool Ok = true;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachineDocuments WHERE MachineDocumentID = " + MachineDocumentID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        try
                        {
                            FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("MachineDocuments") + "/" + Row["FileName"].ToString(), Configs.FTPType);
                        }
                        catch
                        {
                            Ok = false;
                            break;
                        }
                    }
                }
            }
            if (!Ok)
                return Ok;
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM MachineDocuments WHERE MachineDocumentID = " + MachineDocumentID,
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

            return Ok;
        }

        public Image GetMachineImage(int MachineDocumentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachineDocuments" +
                " WHERE MachineDocumentID = " + MachineDocumentID, ConnectionStrings.CatalogConnectionString))
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
                            FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("MachineDocuments") + "/" + FileName,
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

        #endregion

        #region Refresh data

        public void RefreshSpareGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineSpareGroups", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SpareGroupsDT);
            }
        }

        public void RefreshSpares()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineSpares.*, MachineSpareGroups.Type FROM MachineSpares
                INNER JOIN MachineSpareGroups ON MachineSpares.MachineSpareGroupID=MachineSpareGroups.MachineSpareGroupID", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SparesDT);
            }
        }

        public void RefreshSparesOnStock(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineSparesOnStock.*,
                MachineSpareGroups.MachineSpareGroupID, MachineSpares.MachineSpareID, MachineSpares.Description, MachineDetails.Type FROM MachineSparesOnStock
                INNER JOIN MachineDetails ON MachineSparesOnStock.MachineDetailID = MachineDetails.MachineDetailID AND MachineDetails.MachineID=" + MachineID +
                @" INNER JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                 INNER JOIN MachineSpareGroups ON MachineSpares.MachineSpareGroupID = MachineSpareGroups.MachineSpareGroupID",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(SparesOnStockDT);
            }
        }

        public void RefreshMachines()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineID, SubSectorID, MachineName, TechPasportName, MachineFunction, Producer, ProducersContacts, 
                         ProducersRepresentative, RepresentativesContacts, SerialNumber, Year, Commissioning, Productivity, ConsumedPower, Voltage, OperatingPressure, 
                         CompressedAirConsumption, AspirationCapacity, AspirationFlowRate, Firm, InventoryNumber, MachineOperator, Technician, Sizes, Weight, EquipmentNotes, TechnicalNotes, 
                         TechnicalSpecification, MachineElectrics, MachineHydraulics, MachinePneumatics, MachineAspiration, MachineMechanics, PermanentWorks FROM Machines", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MachinesDT);
            }
        }

        public void RefreshUnits()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(UnitsDT);
            }
        }

        public void RefreshElectricsDetails(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineDetails.*, MachineSpares.Description FROM MachineDetails
                LEFT JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                WHERE MachineID = " + MachineID + " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineElectrics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ElectricsDetailsDT);
            }
        }

        public void RefreshPneumaticsDetails(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineDetails.*, MachineSpares.Description FROM MachineDetails
                LEFT JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                WHERE MachineID = " + MachineID + " AND Type=" + Convert.ToInt32(MachineFileTypes.MachinePneumatics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PneumaticsDetailsDT);
            }
        }

        public void RefreshHydraulicsDetails(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineDetails.*, MachineSpares.Description FROM MachineDetails
                LEFT JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                WHERE MachineID = " + MachineID + " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineHydraulics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(HydraulicsDetailsDT);
            }
        }

        public void RefreshAspirationDetails(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineDetails.*, MachineSpares.Description FROM MachineDetails
                LEFT JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                WHERE MachineID = " + MachineID + " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineAspiration), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(AspirationDetailsDT);
            }
        }

        public void RefreshMechanicsDetails(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MachineDetails.*, MachineSpares.Description FROM MachineDetails
                LEFT JOIN MachineSpares ON MachineDetails.MachineSpareID = MachineSpares.MachineSpareID
                WHERE MachineID = " + MachineID + " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineMechanics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MechanicsDetailsDT);
            }
        }

        public void RefreshServiceTools(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineTools WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.ServiceTools), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ServiceToolsDT);
            }
        }

        public void RefreshEquipmentTools(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineTools WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.EquipmentTools), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(EquipmentToolsDT);
            }
        }

        public void RefreshLubricant(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineTools WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.Lubricant), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(LubricantDT);
            }
        }

        public void RefreshExploitationTools(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineTools WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.ExploitationTools), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ExploitationToolsDT);
            }
        }

        public void RefreshRepairTools(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineTools WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.RepairTools), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(RepairToolsDT);
            }
        }

        public void RefreshElectricsUnits(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineElectrics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ElectricsUnitsDT);
            }
        }

        public void RefreshPneumaticsUnits(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.MachinePneumatics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(PneumaticsUnitsDT);
            }
        }

        public void RefreshHydraulicsUnits(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineHydraulics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(HydraulicsUnitsDT);
            }
        }

        public void RefreshAspirationUnits(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineAspiration), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(AspirationUnitsDT);
            }
        }

        public void RefreshMechanicsUnits(int MachineID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MachineUnits WHERE MachineID = " + MachineID +
                " AND Type=" + Convert.ToInt32(MachineFileTypes.MachineMechanics), ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(MechanicsUnitsDT);
            }
        }

        #endregion

        #region Save

        public string SaveMachineDocuments(int MachineDocumentID)
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MachineDocuments WHERE MachineDocumentID = " + MachineDocumentID,
                ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("MachineDocuments") + "/" + FileName,
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

        public void SaveSpareGroups()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM MachineSpareGroups",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(SpareGroupsDT);
                }
            }
        }

        public void SaveSpares()
        {
            DataTable DT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM MachineSpares",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (SparesDT.GetChanges(DataRowState.Added) != null)
                    {
                        DT = SparesDT.GetChanges(DataRowState.Added).Copy();
                        DT.Columns.Remove("Type");
                        DA.Update(DT.Select("MachineSpareGroupID IS NOT NULL AND Name IS NOT NULL"));
                    }
                    if (SparesDT.GetChanges(DataRowState.Deleted) != null)
                    {
                        DT = SparesDT.GetChanges(DataRowState.Deleted).Copy();
                        DT.Columns.Remove("Type");
                        DA.Update(DT);
                    }
                    if (SparesDT.GetChanges(DataRowState.Modified) != null)
                    {
                        DT = SparesDT.GetChanges(DataRowState.Modified).Copy();
                        DT.Columns.Remove("Type");
                        DA.Update(DT);
                    }
                }
            }
            DT.Dispose();
        }

        public void SaveSparesOnStock()
        {
            DataTable DT = SparesOnStockDT.Copy();
            DT.Columns.Remove("Type");
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM MachineSparesOnStock",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DT);
                }
            }
            DT.Dispose();
        }

        public void SaveMachines()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 MachineID, SubSectorID, MachineName, TechPasportName, MachineFunction, Producer, ProducersContacts, 
                         ProducersRepresentative, RepresentativesContacts, SerialNumber, Year, Commissioning, Productivity, ConsumedPower, Voltage, OperatingPressure, 
                         CompressedAirConsumption, AspirationCapacity, AspirationFlowRate, Firm, InventoryNumber, MachineOperator, Technician, Sizes, Weight, EquipmentNotes, TechnicalNotes, 
                         TechnicalSpecification, MachineElectrics, MachineHydraulics, MachinePneumatics, MachineAspiration, MachineMechanics, PermanentWorks FROM Machines",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MachinesDT);
                }
            }
        }

        public void SaveAspirationDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineDetails",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(AspirationDetailsDT);
                }
            }
        }

        public void SaveMechanicsDetails()
        {
            DataTable DT = MechanicsDetailsDT.Copy();
            DT.Columns.Remove("Description");
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineDetails",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DT);
                }
            }
            DT.Dispose();
        }

        public void SaveElectricsDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineDetails",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ElectricsDetailsDT);
                }
            }
        }

        public void SaveHydraulicsDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineDetails",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(HydraulicsDetailsDT);
                }
            }
        }

        public void SavePneumaticsDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineDetails",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(PneumaticsDetailsDT);
                }
            }
        }

        public void SaveExploitationTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineTools",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ExploitationToolsDT);
                }
            }
        }

        public void SaveRepairTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineTools",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(RepairToolsDT);
                }
            }
        }

        public void SaveServiceTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineTools",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ServiceToolsDT);
                }
            }
        }

        public void SaveLubricant()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineTools",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(LubricantDT);
                }
            }
        }

        public void SaveEquipmentTools()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineTools",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(EquipmentToolsDT);
                }
            }
        }

        public void SaveAspirationUnits()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineUnits",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(AspirationUnitsDT);
                }
            }
        }

        public void SaveMechanicsUnits()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineUnits",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(MechanicsUnitsDT);
                }
            }
        }

        public void SaveElectricsUnits()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineUnits",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ElectricsUnitsDT);
                }
            }
        }

        public void SaveHydraulicsUnits()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineUnits",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(HydraulicsUnitsDT);
                }
            }
        }

        public void SavePneumaticsUnits()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM MachineUnits",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(PneumaticsUnitsDT);
                }
            }
        }

        public void SaveAndRefreshMachines(int MachineID)
        {
            SaveMachines();
            ClearMachines();
            RefreshMachines();
            MoveToMachine(MachineID);
        }

        #endregion

        #region MoveTo

        public void MoveToMachine(int MachineID)
        {
            MachinesBS.Position = MachinesBS.Find("MachineID", MachineID);
        }

        public void MoveFirstSparesOnStock()
        {
            SparesOnStockBS.MoveFirst();
        }

        public void MoveFirstAspirationSpareGroups()
        {
            AspirationSpareGroupsBS.MoveFirst();
        }

        public void MoveFirstMechanicsSpareGroups()
        {
            MechanicsSpareGroupsBS.MoveFirst();
        }

        public void MoveFirstElectricsSpareGroups()
        {
            ElectricsSpareGroupsBS.MoveFirst();
        }

        public void MoveFirstHydraulicsSpareGroups()
        {
            HydraulicsSpareGroupsBS.MoveFirst();
        }

        public void MoveFirstPneumaticsSpareGroups()
        {
            PneumaticsSpareGroupsBS.MoveFirst();
        }

        public void MoveFirstAspirationSpares()
        {
            AspirationSparesBS.MoveFirst();
        }

        public void MoveFirstMechanicsSpares()
        {
            MechanicsSparesBS.MoveFirst();
        }

        public void MoveFirstElectricsSpares()
        {
            ElectricsSparesBS.MoveFirst();
        }

        public void MoveFirstHydraulicsSpares()
        {
            HydraulicsSparesBS.MoveFirst();
        }

        public void MoveFirstPneumaticsSpares()
        {
            PneumaticsSparesBS.MoveFirst();
        }

        public void MoveFirstAspirationSparesOnStock()
        {
            AspirationSparesOnStockBS.MoveFirst();
        }

        public void MoveFirstMechanicsSparesOnStock()
        {
            MechanicsSparesOnStockBS.MoveFirst();
        }

        public void MoveFirstElectricsSparesOnStock()
        {
            ElectricsSparesOnStockBS.MoveFirst();
        }

        public void MoveFirstHydraulicsSparesOnStock()
        {
            HydraulicsSparesOnStockBS.MoveFirst();
        }

        public void MoveFirstPneumaticsSparesOnStock()
        {
            PneumaticsSparesOnStockBS.MoveFirst();
        }

        public void MoveToMachineDetail(int MachineDetailID, MachineFileTypes Type)
        {
            switch (Type)
            {
                case MachineFileTypes.AspirationDetails:
                    AspirationDetailsBS.Position = AspirationDetailsBS.Find("MachineDetailID", MachineDetailID);
                    break;
                case MachineFileTypes.MechanicsDetails:
                    MechanicsDetailsBS.Position = MechanicsDetailsBS.Find("MachineDetailID", MachineDetailID);
                    break;
                case MachineFileTypes.ElectricsDetails:
                    ElectricsDetailsBS.Position = ElectricsDetailsBS.Find("MachineDetailID", MachineDetailID);
                    break;
                case MachineFileTypes.HydraulicsDetails:
                    HydraulicsDetailsBS.Position = HydraulicsDetailsBS.Find("MachineDetailID", MachineDetailID);
                    break;
                case MachineFileTypes.PneumaticsDetails:
                    PneumaticsDetailsBS.Position = PneumaticsDetailsBS.Find("MachineDetailID", MachineDetailID);
                    break;
                default:
                    break;
            }
        }

        public void MoveToFactory(int FactoryID)
        {
            FactoryBS.Position = FactoryBS.Find("FactoryID", FactoryID);
        }

        public void MoveToSubSector(int SubSectorID)
        {
            SubSectorsBS.Position = SubSectorsBS.Find("SubSectorID", SubSectorID);
        }

        public void MoveToSector(int SectorID)
        {
            SectorsBS.Position = SectorsBS.Find("SectorID", SectorID);
        }

        public void MoveFirstAspirationDetails()
        {
            AspirationDetailsBS.MoveFirst();
        }

        public void MoveFirstMechanicsDetails()
        {
            MechanicsDetailsBS.MoveFirst();
        }

        public void MoveFirstElectricsDetails()
        {
            ElectricsDetailsBS.MoveFirst();
        }

        public void MoveFirstHydraulicsDetails()
        {
            HydraulicsDetailsBS.MoveFirst();
        }

        public void MoveFirstPneumaticsDetails()
        {
            PneumaticsDetailsBS.MoveFirst();
        }

        public void MoveFirstExploitationTools()
        {
            ExploitationToolsBS.MoveFirst();
        }

        public void MoveFirstRepairTools()
        {
            RepairToolsBS.MoveFirst();
        }

        public void MoveFirstServiceTools()
        {
            ServiceToolsBS.MoveFirst();
        }

        public void MoveFirstLubricant()
        {
            LubricantBS.MoveFirst();
        }

        public void MoveFirstEquipment()
        {
            EquipmentBS.MoveFirst();
        }

        public void MoveFirstAspirationUnits()
        {
            AspirationUnitsBS.MoveFirst();
        }

        public void MoveFirstMechanicsUnits()
        {
            MechanicsUnitsBS.MoveFirst();
        }

        public void MoveFirstElectricsUnits()
        {
            ElectricsUnitsBS.MoveFirst();
        }

        public void MoveFirstHydraulicsUnits()
        {
            HydraulicsUnitsBS.MoveFirst();
        }

        public void MoveFirstPneumaticsUnits()
        {
            PneumaticsUnitsBS.MoveFirst();
        }

        public void MoveFirstTechnicalSpecification()
        {
            TechnicalSpecificationBS.MoveFirst();
        }

        #endregion

        public void AddSpareOnStock(int MachineDetailID, MachineFileTypes Type)
        {
            DataRow NewRow = SparesOnStockDT.NewRow();
            NewRow["MachineDetailID"] = MachineDetailID;
            NewRow["Type"] = Convert.ToInt32(Type);
            SparesOnStockDT.Rows.Add(NewRow);
        }

        #region Create empty rows in xml tables

        public void CreateEmptyTecnhicalRow(int pos)
        {
            DataRow NewRow = TechnicalSpecificationDT.NewRow();
            TechnicalSpecificationDT.Rows.InsertAt(NewRow, pos);
        }

        public void AddTecnhicalRow(string Group)
        {
            DataRow NewRow = TempTechnicalSpecificationDT.NewRow();
            NewRow["Group"] = Group;
            TempTechnicalSpecificationDT.Rows.Add(NewRow);
        }

        public void CreateEmptyAspirationRow()
        {
            DataRow NewRow = AspirationDT.NewRow();
            AspirationDT.Rows.Add(NewRow);
        }

        public void CreateEmptyMechanicsRow()
        {
            DataRow NewRow = MechanicsDT.NewRow();
            MechanicsDT.Rows.Add(NewRow);
        }

        public void CreateEmptyElectricsRow()
        {
            DataRow NewRow = ElectricsDT.NewRow();
            ElectricsDT.Rows.Add(NewRow);
        }

        public void CreateEmptyHydraulicsRow()
        {
            DataRow NewRow = HydraulicsDT.NewRow();
            HydraulicsDT.Rows.Add(NewRow);
        }

        public void CreateEmptyPneumaticsRow()
        {
            DataRow NewRow = PneumaticsDT.NewRow();
            PneumaticsDT.Rows.Add(NewRow);
        }

        #endregion

        public bool IsDetailUsed(int MachineDetailID, MachineFileTypes Type)
        {
            DataRow[] rows = TempMachineDocumentsDT.Select("MachineDetailID=" + MachineDetailID + " AND FileType = " + Convert.ToInt32(Type));
            return rows.Count() > 0;
        }

        public void RemoveAttachedFiles(int MachineDetailID, MachineFileTypes Type)
        {
            DataRow[] rows = TempMachineDocumentsDT.Select("MachineDetailID=" + MachineDetailID + " AND FileType = " + Convert.ToInt32(Type));
            for (int i = 0; i < rows.Count(); i++)
            {
                RemoveMachineDocuments(Convert.ToInt32(rows[i]["MachineDocumentID"]));
            }
        }

        public void DeleteNullSpares()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"DELETE MachineSpares
                WHERE MachineSpareGroupID NOT IN (SELECT MachineSpareGroupID FROM MachineSpareGroups)",
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
        }

        public void DeleteNullSparesOnStock()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"DELETE MachineSparesOnStock
                WHERE MachineDetailID NOT IN (SELECT MachineDetailID FROM MachineDetails)",
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
        }

        public void DeleteNullDetails()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"DELETE MachineDetails
                WHERE MachineSpareGroupID NOT IN (SELECT MachineSpareGroupID FROM MachineSpareGroups)
                OR MachineSpareID NOT IN (SELECT MachineSpareID FROM MachineSpares)",
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
        }

        public void FindMachines(int MachineSpareID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT Machines.MachineName FROM MachineDetails
                INNER JOIN Machines ON MachineDetails.MachineID = Machines.MachineID
                WHERE MachineSpareID = " + MachineSpareID + " ORDER BY MachineName", ConnectionStrings.CatalogConnectionString))
            {
                FindMachinesDT.Clear();
                DA.Fill(FindMachinesDT);
            }
        }

    }
}
