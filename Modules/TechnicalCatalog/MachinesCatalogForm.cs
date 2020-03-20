using ComponentFactory.Krypton.Toolkit;

using Infinium.Modules.Marketing.Clients;

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MachinesCatalogForm : Form
    {
        const int iEditCatalog = 60;
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool CanEditCatalog = false;
        bool bStopTransfer = false;
        bool NeedSplash = false;
        bool NeedAddColumns = true;

        int CurrentTechRowIndex = 0;
        int CurrentMachineDetailID = 0;
        int CurrentMachineDocumentID = 0;
        int CurrentMachineSpareID = 0;
        int FormEvent = 0;

        string CurrentPanelName = "pnlMainPage";
        DataTable PanelsNamesDT;
        PercentageDataGrid CurrentGrid;
        MachineFileTypes CurrentType;

        string MachineName = string.Empty;

        System.Threading.Thread T;

        Form TopForm = null;
        LightStartForm LightStartForm;

        DataTable AttachsDT;
        DataTable RolePermissionsDataTable;

        MachinesCatalog MachinesCatalogManager;

        RoleTypes RoleType = RoleTypes.OrdinaryRole;

        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            TechRole = 2
        }

        public MachinesCatalogForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, this.Name);
            if (PermissionGranted(iEditCatalog))
            {
                CanEditCatalog = true;
            }

            if (CanEditCatalog)
                RoleType = RoleTypes.AdminRole;

            if (RoleType == RoleTypes.OrdinaryRole)
            {
                btnAddExploitationToolsFile.Enabled = false;
                btnRemoveExploitationToolsFile.Enabled = false;
                btnAddRepairToolsFile.Enabled = false;
                btnRemoveRepairToolsFile.Enabled = false;
                btnAddServiceToolsFile.Enabled = false;
                btnRemoveServiceToolsFile.Enabled = false;
                btnAddLubricantToolsFile.Enabled = false;
                btnRemoveLubricantToolsFile.Enabled = false;
                btnSaveTools.Enabled = false;
                btnSaveAspiration.Enabled = false;
                btnAddAspirationSсhema.Enabled = false;
                btnRemoveAspirationSсhema.Enabled = false;
                btnSavePneumatics.Enabled = false;
                btnAddPneumaticsSchema.Enabled = false;
                btnRemovePneumaticsSchema.Enabled = false;
                btnSaveHydraulics.Enabled = false;
                btnAddHydraulicsSchema.Enabled = false;
                btnRemoveHydraulicsSchema.Enabled = false;
                btnSaveMechanics.Enabled = false;
                btnAddMechanicsSchema.Enabled = false;
                btnRemoveMechanicsSchema.Enabled = false;
                btnSaveElectrics.Enabled = false;
                btnAddElectricsSchema.Enabled = false;
                btnRemoveElectricsSchema.Enabled = false;
                btnSaveTechnics.Enabled = false;
                btnAddMachinesStructure.Enabled = false;
                btnRemoveMachinesStructure.Enabled = false;
                btnSaveEquipment.Enabled = false;
                btnAddMachineFoto.Enabled = false;
                btnRemoveMachineFoto.Enabled = false;
                btnSaveMainParameters.Enabled = false;
                btnAddMechanicsDetailFile.Enabled = false;
                btnRemoveMechanicsDetailFile.Enabled = false;
                btnAddElectricsDetailFile.Enabled = false;
                btnRemoveElectricsDetailFile.Enabled = false;
                btnAddPneumaticsDetailFile.Enabled = false;
                btnRemovePneumaticsDetailFile.Enabled = false;
                btnAddHydraulicsDetailFile.Enabled = false;
                btnRemoveHydraulicsDetailFile.Enabled = false;
                btnAddAspirationDetailFile.Enabled = false;
                btnRemoveAspirationDetailFile.Enabled = false;
                btnSaveDetails.Enabled = false;
            }

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            dgvMainParameters.ReadOnly = false;
            while (!SplashForm.bCreated) ;
        }

        private bool PermissionGranted(int PermissionID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

            if (Rows.Count() > 0)
            {
                return Convert.ToBoolean(Rows[0]["Granted"]);
            }

            return false;
        }

        private void MachinesCatalogForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (FormEvent == eClose || FormEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
                    }


                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    return;
                }

            }

            if (FormEvent == eClose || FormEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
                    }

                }

                return;
            }


            if (FormEvent == eShow || FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            bool OkCancel = Infinium.LightMessageBox.Show(ref TopForm, true, "Точно выйти?", "Внимание");
            if (!OkCancel)
                return;
            GC.Collect();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void Initialize()
        {
            AttachsDT = new DataTable();
            AttachsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachsDT.Columns.Add(new DataColumn("Extension", Type.GetType("System.String")));
            AttachsDT.Columns.Add(new DataColumn("FileSize", Type.GetType("System.Int64")));
            AttachsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            MachinesCatalogManager = new MachinesCatalog();
            MachinesCatalogManager.Initialize();

            DataBinding();

            GridSettings();
        }

        private void DataBinding()
        {
            percentageDataGrid1.DataSource = MachinesCatalogManager.FindMachinesList;

            dgvAspirationSpareGroups.DataSource = MachinesCatalogManager.AspirationSpareGroupsList;
            dgvHydraulicsSpareGroups.DataSource = MachinesCatalogManager.HydraulicsSpareGroupsList;
            dgvPneumaticsSpareGroups.DataSource = MachinesCatalogManager.PneumaticsSpareGroupsList;
            dgvElectricsSpareGroups.DataSource = MachinesCatalogManager.ElectricsSpareGroupsList;
            dgvMechanicsSpareGroups.DataSource = MachinesCatalogManager.MechanicsSpareGroupsList;

            dgvAspirationSpares.DataSource = MachinesCatalogManager.AspirationSparesList;
            dgvHydraulicsSpares.DataSource = MachinesCatalogManager.HydraulicsSparesList;
            dgvPneumaticsSpares.DataSource = MachinesCatalogManager.PneumaticsSparesList;
            dgvElectricsSpares.DataSource = MachinesCatalogManager.ElectricsSparesList;
            dgvMechanicsSpares.DataSource = MachinesCatalogManager.MechanicsSparesList;

            dgvAspirationSparesOnStock.DataSource = MachinesCatalogManager.AspirationSparesOnStockList;
            dgvHydraulicsSparesOnStock.DataSource = MachinesCatalogManager.HydraulicsSparesOnStockList;
            dgvPneumaticsSparesOnStock.DataSource = MachinesCatalogManager.PneumaticsSparesOnStockList;
            dgvElectricsSparesOnStock.DataSource = MachinesCatalogManager.ElectricsSparesOnStockList;
            dgvMechanicsSparesOnStock.DataSource = MachinesCatalogManager.MechanicsSparesOnStockList;

            dgvMachines.DataSource = MachinesCatalogManager.MachinesList;

            dgvMainParameters.DataSource = MachinesCatalogManager.MainParametersList;
            dgvAspirationFiles.DataSource = MachinesCatalogManager.AspirationFilesList;
            dgvMechanicsFiles.DataSource = MachinesCatalogManager.MechanicsFilesList;
            dgvElectricsFiles.DataSource = MachinesCatalogManager.ElectricsFilesList;
            dgvHydraulicsFiles.DataSource = MachinesCatalogManager.HydraulicsFilesList;
            dgvPneumaticsFiles.DataSource = MachinesCatalogManager.PneumaticsFilesList;
            dgvAspirationDetailFiles.DataSource = MachinesCatalogManager.AspirationDetailFilesList;
            dgvMechanicsDetailFiles.DataSource = MachinesCatalogManager.MechanicsDetailFilesList;
            dgvElectricsDetailFiles.DataSource = MachinesCatalogManager.ElectricsDetailFilesList;
            dgvHydraulicsDetailFiles.DataSource = MachinesCatalogManager.HydraulicsDetailFilesList;
            dgvPneumaticsDetailFiles.DataSource = MachinesCatalogManager.PneumaticsDetailFilesList;

            dgvExploitationToolsFiles.DataSource = MachinesCatalogManager.ExploitationToolsFilesList;
            dgvRepairToolsFiles.DataSource = MachinesCatalogManager.RepairToolsFilesList;
            dgvServiceToolsFiles.DataSource = MachinesCatalogManager.ServiceToolsFilesList;
            dgvLubricantFiles.DataSource = MachinesCatalogManager.LubricantFilesList;
            dgvEquipmentFiles.DataSource = MachinesCatalogManager.EquipmentFilesList;

            dgvMachinesStructure.DataSource = MachinesCatalogManager.MachinesStructureList;

            dgvExploitationTools.DataSource = MachinesCatalogManager.ExploitationToolsList;
            dgvRepairTools.DataSource = MachinesCatalogManager.RepairToolsList;
            dgvServiceTools.DataSource = MachinesCatalogManager.ServiceToolsList;
            dgvLubricant.DataSource = MachinesCatalogManager.LubricantList;
            dgvEquipment.DataSource = MachinesCatalogManager.EquipmentList;

            dgvAspirationDetails.DataSource = MachinesCatalogManager.AspirationDetailsList;
            dgvMechanicsDetails.DataSource = MachinesCatalogManager.MechanicsDetailsList;
            dgvElectricsDetails.DataSource = MachinesCatalogManager.ElectricsDetailsList;
            dgvHydraulicsDetails.DataSource = MachinesCatalogManager.HydraulicsDetailsList;
            dgvPneumaticsDetails.DataSource = MachinesCatalogManager.PneumaticsDetailsList;

            dgvAspirationUnits.DataSource = MachinesCatalogManager.AspirationUnitsList;
            dgvMechanicsUnits.DataSource = MachinesCatalogManager.MechanicsUnitsList;
            dgvElectricsUnits.DataSource = MachinesCatalogManager.ElectricsUnitsList;
            dgvHydraulicsUnits.DataSource = MachinesCatalogManager.HydraulicsUnitsList;
            dgvPneumaticsUnits.DataSource = MachinesCatalogManager.PneumaticsUnitsList;

            dgvOperatingInstructions.DataSource = MachinesCatalogManager.OperatingInstructionsList;
            dgvServiceInstructions.DataSource = MachinesCatalogManager.ServiceInstructionsList;
            dgvLaborProtInstructions.DataSource = MachinesCatalogManager.LaborProtInstructionsList;
            dgvJournal.DataSource = MachinesCatalogManager.JournalList;
            dgvAdmissions.DataSource = MachinesCatalogManager.AdmissionsList;

            dgvTechnicalSpecification.DataSource = MachinesCatalogManager.TechnicalSpecificationList;
            if (NeedAddColumns)
            {
                dgvAspirationDetails.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvMechanicsDetails.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvElectricsDetails.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvHydraulicsDetails.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvPneumaticsDetails.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvAspirationDetails.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineAspiration));
                dgvMechanicsDetails.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineMechanics));
                dgvElectricsDetails.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineElectrics));
                dgvHydraulicsDetails.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineHydraulics));
                dgvPneumaticsDetails.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachinePneumatics));
                dgvAspirationDetails.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineAspiration));
                dgvMechanicsDetails.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineMechanics));
                dgvElectricsDetails.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineElectrics));
                dgvHydraulicsDetails.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineHydraulics));
                dgvPneumaticsDetails.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachinePneumatics));
                dgvAspirationDetails.Columns.Add(MachinesCatalogManager.AspirationUnitColumn(MachineFileTypes.MachineAspiration));
                dgvMechanicsDetails.Columns.Add(MachinesCatalogManager.MechanicsUnitColumn(MachineFileTypes.MachineMechanics));
                dgvElectricsDetails.Columns.Add(MachinesCatalogManager.ElectricsUnitColumn(MachineFileTypes.MachineElectrics));
                dgvHydraulicsDetails.Columns.Add(MachinesCatalogManager.HydraulicsUnitColumn(MachineFileTypes.MachineHydraulics));
                dgvPneumaticsDetails.Columns.Add(MachinesCatalogManager.PneumaticsUnitColumn(MachineFileTypes.MachinePneumatics));

                dgvAspirationSparesOnStock.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineAspiration));
                dgvMechanicsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineMechanics));
                dgvElectricsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineElectrics));
                dgvHydraulicsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachineHydraulics));
                dgvPneumaticsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareGroupColumn(MachineFileTypes.MachinePneumatics));
                dgvAspirationSparesOnStock.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineAspiration));
                dgvMechanicsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineMechanics));
                dgvElectricsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineElectrics));
                dgvHydraulicsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachineHydraulics));
                dgvPneumaticsSparesOnStock.Columns.Add(MachinesCatalogManager.SpareColumn(MachineFileTypes.MachinePneumatics));

                dgvExploitationTools.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvRepairTools.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvServiceTools.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvLubricant.Columns.Add(MachinesCatalogManager.MeasureColumn);
                dgvEquipment.Columns.Add(MachinesCatalogManager.MeasureColumn);
            }
        }

        private void GridSettings()
        {
            DataGridViewTextBoxColumn IndexNumber = new DataGridViewTextBoxColumn()
            {
                HeaderText = "IndexNumber",
                Name = "IndexNumber",
                ReadOnly = true
            };
            dgvMachines.Columns.Add(IndexNumber);

            foreach (DataGridViewColumn item in dgvMachines.Columns)
            {
                item.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                item.Visible = false;
            }

            dgvMachines.Columns["IndexNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMachines.Columns["IndexNumber"].Width = 40;
            dgvMachines.Columns["IndexNumber"].Visible = true;
            dgvMachines.Columns["MachineName"].Visible = true;
            dgvMachines.Columns["MachineName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMachines.AutoGenerateColumns = false;
            dgvMachines.Columns["IndexNumber"].DisplayIndex = 0;
            dgvMachines.Columns["MachineName"].DisplayIndex = 1;

            dgvMainParameters.Columns["Name"].ReadOnly = true;
            dgvMainParameters.Columns["Number"].ReadOnly = true;
            dgvMainParameters.Columns["ColumnName"].Visible = false;
            dgvMainParameters.Columns["Number"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainParameters.Columns["Number"].Width = 40;
            dgvMainParameters.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMainParameters.Columns["Name"].Width = 220;
            dgvMainParameters.Columns["Value"].MinimumWidth = 200;
            dgvMainParameters.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (DataGridViewColumn item in dgvTechnicalSpecification.Columns)
                item.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (dgvTechnicalSpecification.Columns.Contains("Group"))
                dgvTechnicalSpecification.Columns["Group"].Visible = false;
            if (dgvTechnicalSpecification.Columns.Contains("GroupNumber"))
                dgvTechnicalSpecification.Columns["GroupNumber"].Visible = false;
            dgvTechnicalSpecification.Columns["GroupDisplay"].HeaderText = "Подраздел";
            dgvTechnicalSpecification.Columns["Name"].HeaderText = "Наименование";
            dgvTechnicalSpecification.Columns["Value"].HeaderText = "Значение";
            dgvTechnicalSpecification.Columns["Measure"].HeaderText = "Ед.изм.";
            dgvTechnicalSpecification.Columns["Notes"].HeaderText = "Прим.";
            dgvTechnicalSpecification.Columns["GroupDisplay"].MinimumWidth = 100;
            dgvTechnicalSpecification.Columns["GroupDisplay"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTechnicalSpecification.Columns["Name"].MinimumWidth = 100;
            dgvTechnicalSpecification.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTechnicalSpecification.Columns["Value"].MinimumWidth = 100;
            dgvTechnicalSpecification.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTechnicalSpecification.Columns["Measure"].MinimumWidth = 100;
            dgvTechnicalSpecification.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTechnicalSpecification.Columns["Notes"].MinimumWidth = 100;
            dgvTechnicalSpecification.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvTechnicalSpecification.Columns["GroupDisplay"].ReadOnly = true;

            dgvSpareGroupsSetting(ref dgvAspirationSpareGroups);
            dgvSpareGroupsSetting(ref dgvMechanicsSpareGroups);
            dgvSpareGroupsSetting(ref dgvElectricsSpareGroups);
            dgvSpareGroupsSetting(ref dgvHydraulicsSpareGroups);
            dgvSpareGroupsSetting(ref dgvPneumaticsSpareGroups);

            dgvSparesSetting(ref dgvAspirationSpares);
            dgvSparesSetting(ref dgvMechanicsSpares);
            dgvSparesSetting(ref dgvElectricsSpares);
            dgvSparesSetting(ref dgvHydraulicsSpares);
            dgvSparesSetting(ref dgvPneumaticsSpares);

            dgvSparesOnStockSetting(ref dgvAspirationSparesOnStock);
            dgvSparesOnStockSetting(ref dgvMechanicsSparesOnStock);
            dgvSparesOnStockSetting(ref dgvElectricsSparesOnStock);
            dgvSparesOnStockSetting(ref dgvHydraulicsSparesOnStock);
            dgvSparesOnStockSetting(ref dgvPneumaticsSparesOnStock);

            dgvToolsSetting(ref dgvExploitationTools);
            dgvToolsSetting(ref dgvRepairTools);
            dgvToolsSetting(ref dgvServiceTools);
            dgvToolsSetting(ref dgvLubricant);
            dgvToolsSetting(ref dgvEquipment);

            dgvFilesSetting(ref dgvMachinesStructure);
            dgvFilesSetting(ref dgvEquipmentFiles);
            dgvFilesSetting(ref dgvMechanicsFiles);
            dgvFilesSetting(ref dgvAspirationFiles);
            dgvFilesSetting(ref dgvElectricsFiles);
            dgvFilesSetting(ref dgvHydraulicsFiles);
            dgvFilesSetting(ref dgvPneumaticsFiles);
            dgvFilesSetting(ref dgvAspirationDetailFiles);
            dgvFilesSetting(ref dgvMechanicsDetailFiles);
            dgvFilesSetting(ref dgvElectricsDetailFiles);
            dgvFilesSetting(ref dgvHydraulicsDetailFiles);
            dgvFilesSetting(ref dgvPneumaticsDetailFiles);
            dgvFilesSetting(ref dgvExploitationToolsFiles);
            dgvFilesSetting(ref dgvRepairToolsFiles);
            dgvFilesSetting(ref dgvServiceToolsFiles);
            dgvFilesSetting(ref dgvLubricantFiles);
            dgvFilesSetting(ref dgvEquipmentFiles);
            dgvFilesSetting(ref dgvOperatingInstructions);
            dgvFilesSetting(ref dgvServiceInstructions);
            dgvFilesSetting(ref dgvLaborProtInstructions);
            dgvFilesSetting(ref dgvJournal);
            dgvFilesSetting(ref dgvAdmissions);

            dgvDetailsSetting(ref dgvAspirationDetails);
            dgvDetailsSetting(ref dgvMechanicsDetails);
            dgvDetailsSetting(ref dgvPneumaticsDetails);
            dgvDetailsSetting(ref dgvHydraulicsDetails);
            dgvDetailsSetting(ref dgvElectricsDetails);

            dgvUnitsSetting(ref dgvAspirationUnits);
            dgvUnitsSetting(ref dgvElectricsUnits);
            dgvUnitsSetting(ref dgvMechanicsUnits);
            dgvUnitsSetting(ref dgvPneumaticsUnits);
            dgvUnitsSetting(ref dgvHydraulicsUnits);

        }

        private void dgvToolsSetting(ref PercentageDataGrid grid)
        {
            DataGridViewTextBoxColumn IndexNumber = new DataGridViewTextBoxColumn()
            {
                HeaderText = "IndexNumber",
                Name = "IndexNumber",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            grid.Columns.Add(IndexNumber);

            foreach (DataGridViewColumn item in grid.Columns)
                item.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            grid.Columns["MachineID"].Visible = false;
            grid.Columns["MeasureID"].Visible = false;
            grid.Columns["Type"].Visible = false;
            grid.Columns["IndexNumber"].HeaderText = @"№п/п";
            grid.Columns["Name"].HeaderText = "Наименование";
            grid.Columns["Description"].HeaderText = "Характеристика";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["IndexNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["IndexNumber"].Width = 50;
            grid.Columns["Count"].Width = 75;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MachineToolsID"].Width = 1;
            grid.Columns["MachineToolsID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.AutoGenerateColumns = false;
            grid.Columns["IndexNumber"].DisplayIndex = 0;
            grid.Columns["Name"].DisplayIndex = 1;
            grid.Columns["Description"].DisplayIndex = 2;
            grid.Columns["Count"].DisplayIndex = 3;
            grid.Columns["MeasureColumn"].DisplayIndex = 4;
            grid.Columns["MachineToolsID"].DisplayIndex = 5;
        }

        private void dgvDetailsSetting(ref PercentageDataGrid grid)
        {
            DataGridViewTextBoxColumn IndexNumber = new DataGridViewTextBoxColumn()
            {
                HeaderText = "IndexNumber",
                Name = "IndexNumber",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            grid.Columns.Add(IndexNumber);

            foreach (DataGridViewColumn item in grid.Columns)
                item.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            grid.Columns["MachineSpareGroupID"].Visible = false;
            grid.Columns["MachineSpareID"].Visible = false;
            grid.Columns["MachineID"].Visible = false;
            grid.Columns["UnitID"].Visible = false;
            grid.Columns["MeasureID"].Visible = false;
            grid.Columns["Type"].Visible = false;
            grid.Columns["IndexNumber"].HeaderText = @"№п/п";
            //grid.Columns["Name"].HeaderText = "Модель";
            grid.Columns["Description"].HeaderText = "Краткая хар-ка";
            grid.Columns["DetailDescription"].HeaderText = "Подробная хар-ка";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["OrderCode"].HeaderText = "Код для заказа";
            grid.Columns["OrderPlaces"].HeaderText = "Места для заказа";
            grid.Columns["IndexNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["IndexNumber"].Width = 50;
            //grid.Columns["Name"].MinimumWidth = 50;
            //grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Description"].MinimumWidth = 50;
            grid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["DetailDescription"].MinimumWidth = 50;
            grid.Columns["DetailDescription"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count"].Width = 75;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["OrderCode"].MinimumWidth = 50;
            grid.Columns["OrderCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["OrderPlaces"].MinimumWidth = 50;
            grid.Columns["OrderPlaces"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MachineDetailID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MachineDetailID"].Width = 1;
            grid.AutoGenerateColumns = false;
            grid.Columns["IndexNumber"].DisplayIndex = 0;
            grid.Columns["SpareGroupColumn"].DisplayIndex = 1;
            grid.Columns["SpareColumn"].DisplayIndex = 2;
            //grid.Columns["Name"].DisplayIndex = 2;
            grid.Columns["Description"].DisplayIndex = 3;
            grid.Columns["Count"].DisplayIndex = 4;
            grid.Columns["MeasureColumn"].DisplayIndex = 5;
            grid.Columns["UnitColumn"].DisplayIndex = 6;
            grid.Columns["OrderCode"].DisplayIndex = 7;
            grid.Columns["OrderPlaces"].DisplayIndex = 8;
            grid.Columns["MachineDetailID"].DisplayIndex = 9;
            grid.Columns["DetailDescription"].DisplayIndex = 4;
        }

        private void dgvUnitsSetting(ref PercentageDataGrid grid)
        {
            DataGridViewTextBoxColumn IndexNumber = new DataGridViewTextBoxColumn()
            {
                HeaderText = "IndexNumber",
                Name = "IndexNumber",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            grid.Columns.Add(IndexNumber);

            foreach (DataGridViewColumn item in grid.Columns)
                item.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //grid.Columns["UnitID"].Visible = false;
            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            grid.Columns["MachineID"].Visible = false;
            grid.Columns["Type"].Visible = false;
            grid.Columns["IndexNumber"].HeaderText = @"№п/п";
            grid.Columns["Name"].HeaderText = "Наименование";
            grid.Columns["Description"].HeaderText = "Описание";
            grid.Columns["IndexNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["IndexNumber"].Width = 50;
            grid.Columns["Name"].MinimumWidth = 150;
            grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            grid.Columns["Description"].MinimumWidth = grid.Width / 2;
            grid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["UnitID"].Width = 1;
            grid.Columns["UnitID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.AutoGenerateColumns = false;
            grid.Columns["IndexNumber"].DisplayIndex = 0;
            grid.Columns["Name"].DisplayIndex = 1;
            grid.Columns["Description"].DisplayIndex = 2;
            grid.Columns["UnitID"].DisplayIndex = 3;
        }

        private void dgvFilesSetting(ref PercentageDataGrid grid)
        {
            grid.Columns["FileSize"].Visible = false;
            grid.Columns["Path"].Visible = false;
            grid.Columns["FileType"].Visible = false;
            grid.Columns["MachineDocumentID"].Visible = false;
            grid.Columns["FileName"].MinimumWidth = grid.Width - 1;
            grid.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void dgvSpareGroupsSetting(ref PercentageDataGrid grid)
        {
            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            //grid.Columns["MachineSpareGroupID"].Visible = false;
            grid.Columns["Type"].Visible = false;
            grid.Columns["Name"].HeaderText = "Группа";
            grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["MachineSpareGroupID"].Width = 1;
            grid.Columns["MachineSpareGroupID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.AutoGenerateColumns = false;
            grid.Columns["Name"].DisplayIndex = 0;
            grid.Columns["MachineSpareGroupID"].DisplayIndex = 1;
        }

        private void dgvSparesSetting(ref PercentageDataGrid grid)
        {
            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            grid.Columns["MachineSpareGroupID"].Visible = false;
            grid.Columns["Type"].Visible = false;
            grid.Columns["Name"].HeaderText = "Модель";
            grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["Name"].MinimumWidth = 140;
            grid.Columns["Description"].HeaderText = "Описание";
            grid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MachineSpareID"].Width = 1;
            grid.Columns["MachineSpareID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.AutoGenerateColumns = false;
            grid.Columns["Name"].DisplayIndex = 0;
            grid.Columns["Description"].DisplayIndex = 1;
            grid.Columns["MachineSpareID"].DisplayIndex = 2;
        }

        private void dgvSparesOnStockSetting(ref PercentageDataGrid grid)
        {
            if (grid.Columns.Contains("SerialNumber"))
                grid.Columns["SerialNumber"].Visible = false;
            grid.Columns["MachineDetailID"].Visible = false;
            grid.Columns["MachineSpareGroupID"].Visible = false;
            grid.Columns["MachineSpareID"].Visible = false;
            grid.Columns["Type"].Visible = false;
            //grid.Columns["Type"].Visible = false;
            //grid.Columns["Name"].HeaderText = "Группа";
            //grid.Columns["Name1"].HeaderText = "Модель";
            grid.Columns["Description"].HeaderText = "Характеристика";
            grid.Columns["Count"].HeaderText = "Кол-во";
            //grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["SpareGroupColumn"].ReadOnly = true;
            grid.Columns["SpareColumn"].ReadOnly = true;
            grid.Columns["Description"].ReadOnly = true;
            grid.Columns["MachineSpareStockID"].Width = 1;
            grid.Columns["MachineSpareStockID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.AutoGenerateColumns = false;
            grid.Columns["SpareGroupColumn"].DisplayIndex = 0;
            grid.Columns["SpareColumn"].DisplayIndex = 1;
            grid.Columns["Description"].DisplayIndex = 2;
            grid.Columns["Count"].DisplayIndex = 3;
            grid.Columns["MachineSpareStockID"].DisplayIndex = 4;
        }

        public bool ContainsText(string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        private void PanelNameNewRow(string ValueMember, string DisplayMember)
        {
            DataRow NewRow = PanelsNamesDT.NewRow();
            NewRow["ValueMember"] = ValueMember;
            NewRow["DisplayMember"] = DisplayMember;
            PanelsNamesDT.Rows.Add(NewRow);
        }

        private void MachinesCatalogForm_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(kryptonButton16, "На Главную");
            toolTip1.SetToolTip(kryptonButton17, "На Главную");
            toolTip1.SetToolTip(kryptonButton18, "На Главную");
            toolTip1.SetToolTip(kryptonButton19, "На Главную");
            toolTip1.SetToolTip(kryptonButton20, "На Главную");
            toolTip1.SetToolTip(kryptonButton21, "На Главную");
            toolTip1.SetToolTip(kryptonButton22, "На Главную");
            toolTip1.SetToolTip(kryptonButton23, "На Главную");
            toolTip1.SetToolTip(kryptonButton24, "На Главную");
            toolTip1.SetToolTip(kryptonButton25, "На Главную");
            toolTip1.SetToolTip(kryptonButton26, "На Главную");
            toolTip1.SetToolTip(kryptonButton27, "На Главную");
            toolTip1.SetToolTip(kryptonButton28, "На Главную");
            toolTip1.SetToolTip(kryptonButton29, "На Главную");
            toolTip1.SetToolTip(kryptonButton35, "На Главную");

            PanelsNamesDT = new DataTable();
            PanelsNamesDT.Columns.Add(new DataColumn("ValueMember", Type.GetType("System.String")));
            PanelsNamesDT.Columns.Add(new DataColumn("DisplayMember", Type.GetType("System.String")));
            PanelNameNewRow("pnlMainPage", "Главная страница");
            PanelNameNewRow("pnlTechnicsPage", "Технические характеристики");
            PanelNameNewRow("pnlMechanicsPage", "Механика");
            PanelNameNewRow("pnlElectricsPage", "Электрика");
            PanelNameNewRow("pnlPneumaticsPage", "Пневматика");
            PanelNameNewRow("pnlHydraulicsPage", "Гидравлика");
            PanelNameNewRow("pnlAspirationPage", "Аспирация");
            PanelNameNewRow("pnlEquipmentPage", "Приспособления и оснастка");
            PanelNameNewRow("pnlToolsPage", "Инструмент и материалы");
            PanelNameNewRow("pnlSparesCatalog", "Каталог запчастей");
            PanelNameNewRow("pnlDetailsPage", "Запчасти");
            PanelNameNewRow("pnlInstructions", "Документация");

            cbPagesNavigation1.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation1.ValueMember = "ValueMember";
            cbPagesNavigation1.DisplayMember = "DisplayMember";

            cbPagesNavigation2.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation2.ValueMember = "ValueMember";
            cbPagesNavigation2.DisplayMember = "DisplayMember";

            cbPagesNavigation3.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation3.ValueMember = "ValueMember";
            cbPagesNavigation3.DisplayMember = "DisplayMember";

            cbPagesNavigation4.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation4.ValueMember = "ValueMember";
            cbPagesNavigation4.DisplayMember = "DisplayMember";

            cbPagesNavigation5.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation5.ValueMember = "ValueMember";
            cbPagesNavigation5.DisplayMember = "DisplayMember";

            cbPagesNavigation6.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation6.ValueMember = "ValueMember";
            cbPagesNavigation6.DisplayMember = "DisplayMember";

            cbPagesNavigation7.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation7.ValueMember = "ValueMember";
            cbPagesNavigation7.DisplayMember = "DisplayMember";

            cbPagesNavigation8.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation8.ValueMember = "ValueMember";
            cbPagesNavigation8.DisplayMember = "DisplayMember";

            cbPagesNavigation9.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation9.ValueMember = "ValueMember";
            cbPagesNavigation9.DisplayMember = "DisplayMember";

            cbPagesNavigation10.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation10.ValueMember = "ValueMember";
            cbPagesNavigation10.DisplayMember = "DisplayMember";

            cbPagesNavigation11.DataSource = new DataView(PanelsNamesDT);
            cbPagesNavigation11.ValueMember = "ValueMember";
            cbPagesNavigation11.DisplayMember = "DisplayMember";

            Initialize();
            NeedAddColumns = false;

            lblMachineName.Text = MachineName;
            DefinePageName(CurrentPanelName);

            foreach (Panel p0 in tableLayoutPanel6.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                Panel p2 = (Panel)p0.Controls[2];
                p1.Visible = false;
                p2.Visible = false;
                p0.Height = 40;
            }
            foreach (Panel p0 in tableLayoutPanel9.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                Panel p2 = (Panel)p0.Controls[2];
                p1.Visible = false;
                p2.Visible = false;
                p0.Height = 40;
            }
            foreach (Panel p0 in tableLayoutPanel7.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                p1.Visible = false;
                p0.Height = 40;
            }
            foreach (Panel p0 in tableLayoutPanel8.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                p1.Visible = false;
                p0.Height = 40;
            }
            dgvMechanicsSpareGroups_SelectionChanged(null, null);
            dgvElectricsSpareGroups_SelectionChanged(null, null);
            dgvPneumaticsSpareGroups_SelectionChanged(null, null);
            dgvHydraulicsSpareGroups_SelectionChanged(null, null);
            dgvAspirationSpareGroups_SelectionChanged(null, null);
        }

        private void dgvMachines_SelectionChanged(object sender, EventArgs e)
        {
            ClearRichTextBoxes();

            MachinesCatalogManager.ClearEquipmentTools();
            MachinesCatalogManager.ClearAspirationDetails();
            MachinesCatalogManager.ClearMechanicsDetails();
            MachinesCatalogManager.ClearElectricsDetails();
            MachinesCatalogManager.ClearHydraulicsDetails();
            MachinesCatalogManager.ClearPneumaticsDetails();
            MachinesCatalogManager.ClearExploitationTools();
            MachinesCatalogManager.ClearServiceTools();
            MachinesCatalogManager.ClearRepairTools();
            MachinesCatalogManager.ClearLubricant();
            MachinesCatalogManager.ClearEquipmentTools();
            MachinesCatalogManager.ClearAspirationUnits();
            MachinesCatalogManager.ClearMechanicsUnits();
            MachinesCatalogManager.ClearElectricsUnits();
            MachinesCatalogManager.ClearHydraulicsUnits();
            MachinesCatalogManager.ClearPneumaticsUnits();
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.ClearMainParameters();
            MachinesCatalogManager.ClearAspiration();
            MachinesCatalogManager.ClearMechanics();
            MachinesCatalogManager.ClearElectrics();
            MachinesCatalogManager.ClearHydraulics();
            MachinesCatalogManager.ClearPneumatics();
            MachinesCatalogManager.ClearTechnicalSpecification();
            MachinesCatalogManager.ClearSparesOnStock();

            if (MachinesCatalogManager == null || !MachinesCatalogManager.HasMachines
                || dgvMachines.SelectedCells.Count == 0
                || dgvMachines.CurrentRow.Cells["MachineID"].Value == DBNull.Value)
            {
                return;
            }

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                GetMachineData();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                GetMachineData();
            MachineName = dgvMachines.CurrentRow.Cells["MachineName"].Value.ToString();
            lblMachineName.Text = MachineName;
        }

        private void GetMachineData()
        {
            tbMachineSizes.Text = MachinesCatalogManager.CurrentSizes;
            rtbEquipmentNotes.Text = MachinesCatalogManager.CurrentEquipmentNotes;
            rtbTechnicalNotes.Text = MachinesCatalogManager.CurrentTechnicalNotes;
            rtbPermanentWorks.Text = MachinesCatalogManager.CurrentPermanentWorks;
            tbMachineWeight.Text = MachinesCatalogManager.CurrentWeight;

            if (MachinesCatalogManager.CurrentAspiration.Length > 0)
                MachinesCatalogManager.ReadAspiration(MachinesCatalogManager.CurrentAspiration);
            if (MachinesCatalogManager.CurrentMechanics.Length > 0)
                MachinesCatalogManager.ReadMechanics(MachinesCatalogManager.CurrentMechanics);
            if (MachinesCatalogManager.CurrentElectrics.Length > 0)
                MachinesCatalogManager.ReadElectrics(MachinesCatalogManager.CurrentElectrics);
            if (MachinesCatalogManager.CurrentHydraulics.Length > 0)
                MachinesCatalogManager.ReadHydraulics(MachinesCatalogManager.CurrentHydraulics);
            if (MachinesCatalogManager.CurrentPneumatics.Length > 0)
                MachinesCatalogManager.ReadPneumatics(MachinesCatalogManager.CurrentPneumatics);

            rtbAspirationDescription.Text = MachinesCatalogManager.CurrentAspirationDescription;
            rtbAspirationNotes.Text = MachinesCatalogManager.CurrentAspirationNotes;
            rtbMechanicsDescription.Text = MachinesCatalogManager.CurrentMechanicsDescription;
            rtbMechanicsNotes.Text = MachinesCatalogManager.CurrentMechanicsNotes;
            rtbElectricsDescription.Text = MachinesCatalogManager.CurrentElectricsDescription;
            rtbElectricsNotes.Text = MachinesCatalogManager.CurrentElectricsNotes;
            rtbHydraulicsDescription.Text = MachinesCatalogManager.CurrentHydraulicsDescription;
            rtbHydraulicsNotes.Text = MachinesCatalogManager.CurrentHydraulicsNotes;
            rtbPneumaticsDescription.Text = MachinesCatalogManager.CurrentPneumaticsDescription;
            rtbPneumaticsNotes.Text = MachinesCatalogManager.CurrentPneumaticsNotes;

            if (MachinesCatalogManager.CurrentTechnicalSpecification.Length > 0)
                MachinesCatalogManager.ReadTechnicalSpecification(MachinesCatalogManager.CurrentTechnicalSpecification);

            MachinesCatalogManager.GetMainParameters();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));

            MachinesCatalogManager.RefreshAspirationUnits(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshMechanicsUnits(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshElectricsUnits(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshPneumaticsUnits(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshHydraulicsUnits(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));

            MachinesCatalogManager.RefreshAspirationDetails(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshMechanicsDetails(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshElectricsDetails(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshPneumaticsDetails(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshHydraulicsDetails(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));

            MachinesCatalogManager.RefreshExploitationTools(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshRepairTools(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshServiceTools(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshLubricant(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            MachinesCatalogManager.RefreshEquipmentTools(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));

            MachinesCatalogManager.RefreshSparesOnStock(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));

            pbMachinePhoto.Image = null;
            pbMachinePhoto.Cursor = Cursors.Default;

            if (cbShowMachineFoto.Checked)
            {
                NeedSplash = true;
            }
            if (cbShowMachineFoto.Checked)
            {
                ShowMachineFoto();
            }
            NeedSplash = false;

            MachinesCatalogManager.MoveFirstSparesOnStock();

            MachinesCatalogManager.MoveFirstAspirationSpareGroups();
            MachinesCatalogManager.MoveFirstMechanicsSpareGroups();
            MachinesCatalogManager.MoveFirstElectricsSpareGroups();
            MachinesCatalogManager.MoveFirstHydraulicsSpareGroups();
            MachinesCatalogManager.MoveFirstPneumaticsSpareGroups();

            MachinesCatalogManager.MoveFirstAspirationSpares();
            MachinesCatalogManager.MoveFirstMechanicsSpares();
            MachinesCatalogManager.MoveFirstElectricsSpares();
            MachinesCatalogManager.MoveFirstHydraulicsSpares();
            MachinesCatalogManager.MoveFirstPneumaticsSpares();

            MachinesCatalogManager.MoveFirstAspirationSparesOnStock();
            MachinesCatalogManager.MoveFirstMechanicsSparesOnStock();
            MachinesCatalogManager.MoveFirstElectricsSparesOnStock();
            MachinesCatalogManager.MoveFirstHydraulicsSparesOnStock();
            MachinesCatalogManager.MoveFirstPneumaticsSparesOnStock();

            MachinesCatalogManager.MoveFirstAspirationDetails();
            MachinesCatalogManager.MoveFirstMechanicsDetails();
            MachinesCatalogManager.MoveFirstElectricsDetails();
            MachinesCatalogManager.MoveFirstHydraulicsDetails();
            MachinesCatalogManager.MoveFirstPneumaticsDetails();

            MachinesCatalogManager.MoveFirstExploitationTools();
            MachinesCatalogManager.MoveFirstLubricant();
            MachinesCatalogManager.MoveFirstRepairTools();
            MachinesCatalogManager.MoveFirstServiceTools();
            MachinesCatalogManager.MoveFirstEquipment();

            MachinesCatalogManager.MoveFirstAspirationUnits();
            MachinesCatalogManager.MoveFirstMechanicsUnits();
            MachinesCatalogManager.MoveFirstElectricsUnits();
            MachinesCatalogManager.MoveFirstHydraulicsUnits();
            MachinesCatalogManager.MoveFirstPneumaticsUnits();
            MachinesCatalogManager.MoveFirstTechnicalSpecification();
        }

        #region Add documents

        private void btnAddMachineFoto_Click(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog1.ShowDialog();
        }

        private void btnAddMachinesStructure_Click(object sender, EventArgs e)
        {
            int i = dgvMachines.SelectedCells[0].RowIndex;
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog4.ShowDialog();
        }

        private void btnAddMechanicsSchema_Click(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog2.ShowDialog();
        }

        private void btnAddElectricsSchema_Click(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog5.ShowDialog();
        }

        private void btnAddHydraulicsSchema_Click(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog6.ShowDialog();
        }

        private void btnAddPneumaticsSchema_Click(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog7.ShowDialog();
        }

        private void btnAddEquipmentSchema_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте наименование, к которому будет прикреплен файл",
                   "Добавление изображения");
                return;
            }

            openFileDialog3.ShowDialog();
        }

        private void btnAddAspirationSchema_Click(object sender, EventArgs e)
        {
            if (dgvMachines.SelectedCells.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog8.ShowDialog();
        }

        #endregion

        #region Zoom images

        private void btnZoomMachineFoto_Click(object sender, EventArgs e)
        {
            if (pbMachinePhoto.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pbMachinePhoto.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        #endregion

        #region openFileDialog_FileOk

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog1.FileName);

            //if (fileInfo.Length > 6291000)
            //{
            //    MessageBox.Show("Файл больше 6 МБ и не может быть загружен");
            //    return;
            //}
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog1.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineFoto);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineFoto);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            ShowMachineFoto();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog2.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog2.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog2.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineMechanics);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineMechanics);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog3.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog3.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog3.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1 && dgvEquipment.SelectedCells.Count == 1 && dgvEquipment.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    int MachineToolsID = Convert.ToInt32(dgvEquipment.CurrentRow.Cells["MachineToolsID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.EquipmentTools, MachineToolsID);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.EquipmentTools, MachineToolsID);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            dgvEquipmentTools_SelectionChanged(null, null);
        }

        private void openFileDialog4_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog4.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog4.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineTechnical);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineTechnical);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        private void openFileDialog5_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog5.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog5.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog5.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineElectrics);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineElectrics);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        private void openFileDialog6_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog6.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog6.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog6.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineHydraulics);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineHydraulics);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        private void openFileDialog7_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog7.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog7.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog7.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachinePneumatics);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachinePneumatics);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        private void openFileDialog8_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog8.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog8.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog8.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineAspiration);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, MachineFileTypes.MachineAspiration);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        #endregion

        #region Show images

        private void ShowMachineFoto()
        {
            if (MachinesCatalogManager.HasMachineFoto)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentMachineFoto;

                if (MachineDocumentID > -1)
                {
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        pbMachinePhoto.Image = MachinesCatalogManager.GetMachineImage(MachineDocumentID);

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                    }
                    else
                        pbMachinePhoto.Image = MachinesCatalogManager.GetMachineImage(MachineDocumentID);
                }
            }

            if (pbMachinePhoto.Image == null)
            {
                pbMachinePhoto.Cursor = Cursors.Default;
            }
            else
            {
                pbMachinePhoto.Cursor = Cursors.Hand;
            }
        }

        #endregion

        #region Remove documents

        private void btnRemoveMachineFoto_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasMachineFoto)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentMachineFoto;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
                pbMachinePhoto.Image = null;
            }
        }

        private void btnRemoveMechanicsSchema_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasMechanicsFiles)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentMechanicsFile;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }

        private void btnRemoveElectricsSchema_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasElectricsFiles)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentElectricsFile;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }

        private void btnRemoveHydraulicsSchema_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasHydraulicsFiles)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentHydraulicsFile;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }

        private void btnRemovePneumaticsSchema_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasPneumaticsFiles)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentPneumaticsFile;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }

        private void btnRemoveMachinesStructure_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasMachinesStructure)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentMachinesStructure;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }

        private void btnRemoveEquipmentFile_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasEquipmentFiles)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = Convert.ToInt32(dgvEquipmentFiles.CurrentRow.Cells["MachineDocumentID"].Value);
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }

        private void btnRemoveAspirationSchema_Click(object sender, EventArgs e)
        {
            if (!MachinesCatalogManager.HasAspirationFiles)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                int MachineDocumentID = MachinesCatalogManager.CurrentAspirationFile;
                if (MachineDocumentID > -1)
                {
                    MachinesCatalogManager.RemoveMachineDocuments(MachineDocumentID);
                    MachinesCatalogManager.ClearMachineDocuments();
                    MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                }
            }
        }


        #endregion

        private void ClearRichTextBoxes()
        {
            rtbMechanicsDescription.Clear();
            rtbMechanicsNotes.Clear();
            rtbElectricsDescription.Clear();
            rtbElectricsNotes.Clear();
            rtbHydraulicsDescription.Clear();
            rtbHydraulicsNotes.Clear();
            rtbHydraulicsDescription.Clear();
            rtbPneumaticsDescription.Clear();
            rtbPneumaticsNotes.Clear();
            tbMachineSizes.Clear();
            rtbTechnicalNotes.Clear();
            tbMachineWeight.Clear();
        }

        private void dgvMachinesStructure_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string temppath = string.Empty;
            int MachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).CurrentRow.Cells["MachineDocumentID"].Value);
            {
                T = new System.Threading.Thread(delegate ()
                {
                    Thread T1 = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Открытие файла.\r\nПодождите..."); });
                    T1.Start();

                    while (!SplashWindow.bSmallCreated) ;
                    temppath = MachinesCatalogManager.SaveMachineDocuments(MachineDocumentID);
                    while (SplashWindow.bSmallCreated)
                        SmallWaitForm.CloseS = true;
                });
                T.Start();

                while (T.IsAlive)
                {
                    T.Join(50);
                    Application.DoEvents();

                    if (bStopTransfer)
                    {
                        bStopTransfer = false;
                        timer1.Enabled = false;
                        return;
                    }
                }

                if (!bStopTransfer && temppath != null)
                    System.Diagnostics.Process.Start(temppath);
            }
        }

        private void btnSaveTechnics_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            MachinesCatalogManager.CurrentTechnicalSpecification = MachinesCatalogManager.GetTechnicalSpecification();
            MachinesCatalogManager.CurrentSizes = tbMachineSizes.Text;
            MachinesCatalogManager.CurrentTechnicalNotes = rtbTechnicalNotes.Text;
            MachinesCatalogManager.CurrentWeight = tbMachineWeight.Text;
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void btnSaveMainParameters_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = 0;
            if (dgvMachines.SelectedCells.Count != 0 && dgvMachines.CurrentRow.Cells["MachineID"].Value != DBNull.Value)
                MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            MachinesCatalogManager.PreSaveMainParameters();
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvMainParameters_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvMainParameters.CurrentCell.ColumnIndex == 3)
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!((e.KeyChar >= (char)48 && e.KeyChar <= (char)57) || (e.KeyChar == (char)8) || (e.KeyChar == (char)45) || (e.KeyChar == (char)44)))
            //    e.Handled = true;
        }

        private void DefinePageName(string PanelName)
        {
            string PageName = "Главная страница";
            switch (PanelName)
            {
                case "pnlInstructions":
                    pnlInstructions.BringToFront();
                    PageName = "Документация";
                    break;
                case "pnlSparesCatalog":
                    pnlSparesCatalog.BringToFront();
                    PageName = "Каталог запчастей";
                    break;
                case "pnlAspirationPage":
                    pnlAspirationPage.BringToFront();
                    PageName = "Аспирация";
                    CurrentType = MachineFileTypes.MachineAspiration;
                    break;
                case "pnlMechanicsPage":
                    pnlMechanicsPage.BringToFront();
                    PageName = "Механика";
                    CurrentType = MachineFileTypes.MachineMechanics;
                    break;
                case "pnlTechnicsPage":
                    PageName = "Технические характеристики";
                    pnlTechnicsPage.BringToFront();
                    break;
                case "pnlElectricsPage":
                    pnlElectricsPage.BringToFront();
                    PageName = "Электрика";
                    CurrentType = MachineFileTypes.MachineElectrics;
                    break;
                case "pnlMainPage":
                    pnlMainPage.BringToFront();
                    PageName = "Главная страница";
                    break;
                case "pnlPneumaticsPage":
                    pnlPneumaticsPage.BringToFront();
                    PageName = "Пневматика";
                    CurrentType = MachineFileTypes.MachinePneumatics;
                    break;
                case "pnlHydraulicsPage":
                    pnlHydraulicsPage.BringToFront();
                    PageName = "Гидравлика";
                    CurrentType = MachineFileTypes.MachineHydraulics;
                    break;
                case "pnlEquipmentPage":
                    pnlEquipmentPage.BringToFront();
                    PageName = "Приспособления и оснастка";
                    CurrentType = MachineFileTypes.EquipmentTools;
                    break;
                case "pnlDetailsPage":
                    pnlDetailsPage.BringToFront();
                    PageName = "Запчасти";
                    break;
                case "pnlToolsPage":
                    pnlToolsPage.BringToFront();
                    PageName = "Инструмент и материалы";
                    break;
            }
            CurrentPanelName = PanelName;
            lblPageName.Text = PageName;
            lblPageName.Left = panel4.Width / 2 - lblPageName.Width / 2;
        }

        private void lbtnTechnics_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlTechnicsPage.Name);
        }

        private void lbtnElectrics_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlElectricsPage.Name);
        }

        private void lbtnPneumatics_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlPneumaticsPage.Name);
        }

        private void lbtnHydraulics_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlHydraulicsPage.Name);
        }

        private void lbtnEquipment_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlEquipmentPage.Name);
        }

        private void lbtnAspiration_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlAspirationPage.Name);
        }

        private void lbtnDetails_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlDetailsPage.Name);
        }

        private void lbtnToolsPage_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlToolsPage.Name);
        }

        private void lbtnBackToMainPage1_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlMainPage.Name);
        }

        private void btnCompareMachines_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MachinesSummaryForm MachinesSummaryForm = new MachinesSummaryForm(this, ref MachinesCatalogManager);
            TopForm = MachinesSummaryForm;
            MachinesSummaryForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            MachinesSummaryForm.Dispose();
            TopForm = null;
        }

        private void cmiInsertNewRow_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;

            MachinesCatalogManager.CreateEmptyTecnhicalRow(CurrentTechRowIndex);
        }

        private void dgvTechnicalSpecification_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                CurrentTechRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void lbtnMechanics_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlMechanicsPage.Name);
        }

        private void dgvEquipmentTools_SelectionChanged(object sender, EventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (MachinesCatalogManager == null
                || dgvEquipment.SelectedCells.Count == 0)
            {
                return;
            }
            int MachineDetailID = 0;
            if (dgvEquipment.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(dgvEquipment.CurrentRow.Cells["MachineToolsID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, MachineFileTypes.EquipmentTools);
        }

        private void lblHover_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void lblHover_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void label32_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            Panel pnl = (Panel)((Panel)lbl.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel6.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                Panel p2 = (Panel)p0.Controls[2];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel6.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p2.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p2.Visible = true;
                        p0.Height = tableLayoutPanel6.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p2.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void label45_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            Panel pnl = (Panel)((Panel)lbl.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel7.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel7.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p0.Height = tableLayoutPanel7.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void label64_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            Panel pnl = (Panel)((Panel)lbl.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel9.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                Panel p2 = (Panel)p0.Controls[2];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel9.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p2.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p2.Visible = true;
                        p0.Height = tableLayoutPanel9.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p2.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void lblDetailGroups_Clicked(object sender, EventArgs e)
        {
            pnl1.BringToFront();
        }

        private void lblDetailsList_Clicked(object sender, EventArgs e)
        {
            pnlDetailsList.BringToFront();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            ComponentFactory.Krypton.Toolkit.KryptonButton btn = (ComponentFactory.Krypton.Toolkit.KryptonButton)sender;
            Panel pnl = (Panel)((Panel)btn.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel6.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                Panel p2 = (Panel)p0.Controls[2];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel6.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p2.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p2.Visible = true;
                        p0.Height = tableLayoutPanel6.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p2.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void kryptonButton10_Click(object sender, EventArgs e)
        {
            ComponentFactory.Krypton.Toolkit.KryptonButton btn = (ComponentFactory.Krypton.Toolkit.KryptonButton)sender;
            Panel pnl = (Panel)((Panel)btn.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel7.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel7.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p0.Height = tableLayoutPanel7.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void kryptonButton21_Click(object sender, EventArgs e)
        {
            ComponentFactory.Krypton.Toolkit.KryptonButton btn = (ComponentFactory.Krypton.Toolkit.KryptonButton)sender;
            Panel pnl = (Panel)((Panel)btn.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel9.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                Panel p2 = (Panel)p0.Controls[2];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel9.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p2.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p2.Visible = true;
                        p0.Height = tableLayoutPanel9.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p2.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void dgvMechanicsSpareGroups_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineMechanics);
        }

        private void dgvElectricsSpareGroups_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineElectrics);
        }

        private void dgvPneumaticsSpareGroups_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachinePneumatics);
        }

        private void dgvHydraulicsSpareGroups_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineHydraulics);
        }

        private void dgvAspirationSpareGroups_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineAspiration);
        }

        private void dgvMechanicsDetails_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineMechanics);
        }

        private void dgvElectricsDetails_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineElectrics);
        }

        private void dgvPneumaticsDetails_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachinePneumatics);
        }

        private void dgvHydraulicsDetails_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineHydraulics);
        }

        private void dgvAspirationDetails_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineAspiration);
        }

        private void dgvEquipmentTools_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.EquipmentTools);
        }

        private void dgvHydraulicsUnits_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineHydraulics);
        }

        private void dgvMechanicsUnits_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineMechanics);
        }

        private void dgvPneumaticsUnits_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachinePneumatics);
        }

        private void dgvElectricsUnits_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineElectrics);
        }

        private void dgvAspirationUnits_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.MachineAspiration);
        }

        private void dgvMechanicsSpareGroups_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu7.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiRemoveRow_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int Index = CurrentGrid.CurrentRow.Index;

            if (CurrentGrid.CurrentRow.IsNewRow)
                return;

            CurrentGrid.Rows.RemoveAt(Index);

            if (CurrentGrid.Rows.Count - 2 == Index)
                Index--;
            if (Index == -1)
                Index = 0;

            if (CurrentGrid.Rows.Count == 1)
                return;
            CurrentGrid.CurrentCell = CurrentGrid.Rows[Index].Cells[0];
            CurrentGrid.Rows[Index].Selected = true;
        }

        private bool CheckDetailsData(MachineFileTypes Type)
        {
            bool flag = true;
            switch (Type)
            {
                case MachineFileTypes.MachineFoto:
                    break;
                case MachineFileTypes.MachineAspiration:
                    foreach (DataGridViewRow item in dgvAspirationDetails.Rows)
                    {
                        if (item.IsNewRow)
                            continue;
                        if (item.Cells["MachineSpareGroupID"].Value == DBNull.Value
                            || item.Cells["MachineSpareID"].Value == DBNull.Value)
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;
                case MachineFileTypes.MachineMechanics:
                    foreach (DataGridViewRow item in dgvMechanicsDetails.Rows)
                    {
                        if (item.IsNewRow)
                            continue;
                        if (item.Cells["MachineSpareGroupID"].Value == DBNull.Value
                            || item.Cells["MachineSpareID"].Value == DBNull.Value)
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;
                case MachineFileTypes.MachineTechnical:
                    break;
                case MachineFileTypes.MachineElectrics:
                    foreach (DataGridViewRow item in dgvElectricsDetails.Rows)
                    {
                        if (item.IsNewRow)
                            continue;
                        if (item.Cells["MachineSpareGroupID"].Value == DBNull.Value
                            || item.Cells["MachineSpareID"].Value == DBNull.Value)
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;
                case MachineFileTypes.MachinePneumatics:
                    foreach (DataGridViewRow item in dgvPneumaticsDetails.Rows)
                    {
                        if (item.IsNewRow)
                            continue;
                        if (item.Cells["MachineSpareGroupID"].Value == DBNull.Value
                            || item.Cells["MachineSpareID"].Value == DBNull.Value)
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;
                case MachineFileTypes.MachineHydraulics:
                    foreach (DataGridViewRow item in dgvHydraulicsDetails.Rows)
                    {
                        if (item.IsNewRow)
                            continue;
                        if (item.Cells["MachineSpareGroupID"].Value == DBNull.Value
                            || item.Cells["MachineSpareID"].Value == DBNull.Value)
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;
                case MachineFileTypes.EquipmentTools:
                    break;
                default:
                    break;
            }
            return flag;
        }

        private void cmiSaveDetails_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            if (CheckDetailsData(MachineFileTypes.MachineAspiration))
            {
                MachinesCatalogManager.SaveAspirationDetails();
                MachinesCatalogManager.ClearAspirationDetails();
                MachinesCatalogManager.RefreshAspirationDetails(MachineID);
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Запчасти Аспирация: не выбран один из параметров \"Группа запчастей\" или \"Модель\". Сохранение прервано",
                   "Ошибка сохранения");
                return;
            }
            if (CheckDetailsData(MachineFileTypes.MachineMechanics))
            {
                MachinesCatalogManager.SaveMechanicsDetails();
                MachinesCatalogManager.ClearMechanicsDetails();
                MachinesCatalogManager.RefreshMechanicsDetails(MachineID);
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Запчасти Механика: не выбран один из параметров \"Группа запчастей\" или \"Модель\". Сохранение прервано",
                   "Ошибка сохранения");
                return;
            }
            if (CheckDetailsData(MachineFileTypes.MachinePneumatics))
            {
                MachinesCatalogManager.SavePneumaticsDetails();
                MachinesCatalogManager.ClearPneumaticsDetails();
                MachinesCatalogManager.RefreshPneumaticsDetails(MachineID);
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Запчасти Пневматика: не выбран один из параметров \"Группа запчастей\" или \"Модель\". Сохранение прервано",
                   "Ошибка сохранения");
                return;
            }
            if (CheckDetailsData(MachineFileTypes.MachineHydraulics))
            {
                MachinesCatalogManager.SaveHydraulicsDetails();
                MachinesCatalogManager.ClearHydraulicsDetails();
                MachinesCatalogManager.RefreshHydraulicsDetails(MachineID);
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Запчасти Гидравлика: не выбран один из параметров \"Группа запчастей\" или \"Модель\". Сохранение прервано",
                   "Ошибка сохранения");
                return;
            }
            if (CheckDetailsData(MachineFileTypes.MachineElectrics))
            {
                MachinesCatalogManager.SaveElectricsDetails();
                MachinesCatalogManager.ClearElectricsDetails();
                MachinesCatalogManager.RefreshElectricsDetails(MachineID);
            }
            else
            {
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Запчасти Электрика: не выбран один из параметров \"Группа запчастей\" или \"Модель\". Сохранение прервано",
                   "Ошибка сохранения");
                return;
            }

            MachinesCatalogManager.SaveSparesOnStock();
            MachinesCatalogManager.ClearSparesOnStock();

            MachinesCatalogManager.DeleteNullSparesOnStock();

            MachinesCatalogManager.RefreshSparesOnStock(MachineID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvMechanicsDetailFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvMechanicsDetails;
                CurrentType = MachineFileTypes.MechanicsDetails;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(dgvMechanicsDetails.CurrentRow.Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvElectricsDetailFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvElectricsDetails;
                CurrentType = MachineFileTypes.ElectricsDetails;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvPneumaticsDetailFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvPneumaticsDetails;
                CurrentType = MachineFileTypes.PneumaticsDetails;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvHydraulicsDetailFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvHydraulicsDetails;
                CurrentType = MachineFileTypes.HydraulicsDetails;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAspirationDetailFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvAspirationDetails;
                CurrentType = MachineFileTypes.AspirationDetails;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;

                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMechanicsDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentType = MachineFileTypes.MechanicsDetails;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvElectricsDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentType = MachineFileTypes.ElectricsDetails;
                CurrentGrid.Rows[e.RowIndex].Selected = true;

                if (CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvPneumaticsDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentType = MachineFileTypes.PneumaticsDetails;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvHydraulicsDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentType = MachineFileTypes.HydraulicsDetails;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAspirationDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentType = MachineFileTypes.AspirationDetails;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.Rows[e.RowIndex].Cells["MachineDetailID"].Value);
                else
                    CurrentMachineDetailID = 0;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMechanicsDetails_SelectionChanged(object sender, EventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (MachinesCatalogManager == null
                || dgvMechanicsDetails.SelectedCells.Count == 0)
            {
                return;
            }
            int MachineDetailID = 0;
            if (dgvMechanicsDetails.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(dgvMechanicsDetails.CurrentRow.Cells["MachineDetailID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, MachineFileTypes.MechanicsDetails);
        }

        private void dgvElectricsDetails_SelectionChanged(object sender, EventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (MachinesCatalogManager == null
                || dgvElectricsDetails.SelectedCells.Count == 0)
            {
                return;
            }
            int MachineDetailID = 0;
            if (dgvElectricsDetails.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(dgvElectricsDetails.CurrentRow.Cells["MachineDetailID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, MachineFileTypes.ElectricsDetails);
        }

        private void dgvPneumaticsDetails_SelectionChanged(object sender, EventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (MachinesCatalogManager == null
                || dgvPneumaticsDetails.SelectedCells.Count == 0)
            {
                return;
            }
            int MachineDetailID = 0;
            if (dgvPneumaticsDetails.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(dgvPneumaticsDetails.CurrentRow.Cells["MachineDetailID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, MachineFileTypes.PneumaticsDetails);
        }

        private void dgvHydraulicsDetails_SelectionChanged(object sender, EventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (MachinesCatalogManager == null
                || dgvHydraulicsDetails.SelectedCells.Count == 0)
            {
                return;
            }
            int MachineDetailID = 0;
            if (dgvHydraulicsDetails.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(dgvHydraulicsDetails.CurrentRow.Cells["MachineDetailID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, MachineFileTypes.HydraulicsDetails);
        }

        private void dgvAspirationDetails_SelectionChanged(object sender, EventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (MachinesCatalogManager == null
                || dgvAspirationDetails.SelectedCells.Count == 0)
            {
                return;
            }
            int MachineDetailID = 0;
            if (dgvAspirationDetails.CurrentRow.Cells["MachineDetailID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(dgvAspirationDetails.CurrentRow.Cells["MachineDetailID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, MachineFileTypes.AspirationDetails);
        }

        private void openFileDialog9_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog9.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 16 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog9.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog9.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1 && CurrentMachineDetailID != 0)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, CurrentType, CurrentMachineDetailID);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, CurrentType, CurrentMachineDetailID);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
            switch (CurrentType)
            {
                case MachineFileTypes.EquipmentTools:
                    dgvEquipmentTools_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.AspirationDetails:
                    dgvAspirationDetails_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.MechanicsDetails:
                    dgvMechanicsDetails_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.ElectricsDetails:
                    dgvElectricsDetails_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.HydraulicsDetails:
                    dgvHydraulicsDetails_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.PneumaticsDetails:
                    dgvPneumaticsDetails_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.ExploitationTools:
                    dgvExploitationTools_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.RepairTools:
                    dgvRepairTools_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.ServiceTools:
                    dgvServiceTools_SelectionChanged(null, null);
                    break;
                case MachineFileTypes.Lubricant:
                    dgvLubricant_SelectionChanged(null, null);
                    break;
                default:
                    break;
            }
        }

        private void cmiAttachDetailFile_Click(object sender, EventArgs e)
        {
            if (CurrentGrid.SelectedCells.Count == 0
                || CurrentGrid.CurrentRow.IsNewRow || CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте запчасть, к которой будет прикреплен файл",
                   "Добавление файла");
                return;
            }

            openFileDialog9.ShowDialog();
        }

        private void cmiRemoveDetailFile_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void dgvEquipmentFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvEquipment;
                CurrentType = MachineFileTypes.EquipmentTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;

                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvEquipmentTools_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvEquipment;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.EquipmentTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;
                CurrentMachineDocumentID = 0;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiAttachToolsFile_Click(object sender, EventArgs e)
        {
            if (CurrentGrid.SelectedCells.Count == 0
                || CurrentGrid.CurrentRow.IsNewRow || CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте запчасть, к которой будет прикреплен файл",
                   "Добавление файла");
                return;
            }

            openFileDialog9.ShowDialog();
        }

        private void dgvExploitationTools_SelectionChanged(object sender, EventArgs e)
        {
            FilterDetailFiles(dgvExploitationTools, MachineFileTypes.ExploitationTools);
        }

        private void dgvRepairTools_SelectionChanged(object sender, EventArgs e)
        {
            FilterDetailFiles(dgvRepairTools, MachineFileTypes.RepairTools);
        }

        private void dgvServiceTools_SelectionChanged(object sender, EventArgs e)
        {
            FilterDetailFiles(dgvServiceTools, MachineFileTypes.ServiceTools);
        }

        private void dgvLubricant_SelectionChanged(object sender, EventArgs e)
        {
            FilterDetailFiles(dgvLubricant, MachineFileTypes.Lubricant);
        }

        private void FilterDetailFiles(PercentageDataGrid grid, MachineFileTypes Type)
        {
            if (MachinesCatalogManager == null)
            {
                return;
            }
            int MachineDetailID = 0;
            if (grid.SelectedCells.Count != 0 && grid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                MachineDetailID = Convert.ToInt32(grid.CurrentRow.Cells["MachineToolsID"].Value);
            MachinesCatalogManager.FilterDetailFiles(MachineDetailID, Type);
        }

        private void dgvExploitationTools_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.ExploitationTools);
        }

        private void dgvRepairTools_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.RepairTools);
        }

        private void dgvServiceTools_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.ServiceTools);
        }

        private void dgvLubricant_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["MachineID"].Value = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            e.Row.Cells["Type"].Value = Convert.ToInt32(MachineFileTypes.Lubricant);
        }

        private void dgvExploitationTools_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvExploitationTools;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.ExploitationTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;
                CurrentMachineDocumentID = 0;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvRepairTools_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvRepairTools;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.RepairTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;
                CurrentMachineDocumentID = 0;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvServiceTools_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvServiceTools;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.ServiceTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;
                CurrentMachineDocumentID = 0;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvLubricant_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvLubricant;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.Lubricant;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;

                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;
                CurrentMachineDocumentID = 0;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvExploitationToolsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvExploitationTools;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.ExploitationTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;

                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvRepairToolsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvRepairTools;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.RepairTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;

                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvServiceToolsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvServiceTools;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.ServiceTools;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;

                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvLubricantFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvLubricant;
                if (CurrentGrid.CurrentRow == null)
                    return;
                CurrentType = MachineFileTypes.Lubricant;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value != DBNull.Value)
                    CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
                else
                    CurrentMachineDetailID = 0;

                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnSaveTools_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            MachinesCatalogManager.SaveExploitationTools();
            MachinesCatalogManager.ClearExploitationTools();
            MachinesCatalogManager.RefreshExploitationTools(MachineID);

            MachinesCatalogManager.SaveRepairTools();
            MachinesCatalogManager.ClearRepairTools();
            MachinesCatalogManager.RefreshRepairTools(MachineID);

            MachinesCatalogManager.SaveServiceTools();
            MachinesCatalogManager.ClearServiceTools();
            MachinesCatalogManager.RefreshServiceTools(MachineID);

            MachinesCatalogManager.SaveLubricant();
            MachinesCatalogManager.ClearLubricant();
            MachinesCatalogManager.RefreshLubricant(MachineID);

            MachinesCatalogManager.SaveEquipmentTools();
            MachinesCatalogManager.ClearEquipmentTools();
            MachinesCatalogManager.RefreshEquipmentTools(MachineID);

            MachinesCatalogManager.CurrentEquipmentNotes = rtbEquipmentNotes.Text;
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void cmiSaveUnits_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            switch (CurrentType)
            {
                case MachineFileTypes.MachineAspiration:
                    MachinesCatalogManager.SaveAspirationUnits();
                    MachinesCatalogManager.ClearAspirationUnits();
                    MachinesCatalogManager.RefreshAspirationUnits(MachineID);
                    break;
                case MachineFileTypes.MachineMechanics:
                    MachinesCatalogManager.SaveMechanicsUnits();
                    MachinesCatalogManager.ClearMechanicsUnits();
                    MachinesCatalogManager.RefreshMechanicsUnits(MachineID);
                    break;
                case MachineFileTypes.MachineElectrics:
                    MachinesCatalogManager.SaveElectricsUnits();
                    MachinesCatalogManager.ClearElectricsUnits();
                    MachinesCatalogManager.RefreshElectricsUnits(MachineID);
                    break;
                case MachineFileTypes.MachinePneumatics:
                    MachinesCatalogManager.SavePneumaticsUnits();
                    MachinesCatalogManager.ClearPneumaticsUnits();
                    MachinesCatalogManager.RefreshPneumaticsUnits(MachineID);
                    break;
                case MachineFileTypes.MachineHydraulics:
                    MachinesCatalogManager.SaveHydraulicsUnits();
                    MachinesCatalogManager.ClearHydraulicsUnits();
                    MachinesCatalogManager.RefreshHydraulicsUnits(MachineID);
                    break;
                default:
                    break;
            }
            MachinesCatalogManager.ClearUnits();
            MachinesCatalogManager.RefreshUnits();
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvAspirationUnits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvAspirationUnits;
                CurrentType = MachineFileTypes.MachineAspiration;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;

                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvElectricsUnits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvElectricsUnits;
                CurrentType = MachineFileTypes.MachineElectrics;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;

                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvPneumaticsUnits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvPneumaticsUnits;
                CurrentType = MachineFileTypes.MachinePneumatics;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;

                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMechanicsUnits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvMechanicsUnits;
                CurrentType = MachineFileTypes.MachineMechanics;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvHydraulicsUnits_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = dgvHydraulicsUnits;
                CurrentType = MachineFileTypes.MachineHydraulics;
                ((PercentageDataGrid)sender).Rows[e.RowIndex].Selected = true;

                kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void tbMachineSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string searchText = tbMachineSearch.Text;

                try
                {
                    foreach (DataGridViewRow row in dgvMachines.Rows)
                    {
                        if (ContainsText(row.Cells["MachineName"].Value.ToString(), searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            int MachineID = Convert.ToInt32(row.Cells["MachineID"].Value);
                            MachinesCatalogManager.MoveToMachine(MachineID);
                            tbMachineSearch.Clear();
                            break;
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void dgvMechanicsSpares_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (dgvMechanicsSpareGroups.SelectedCells.Count > 0 && !dgvMechanicsSpareGroups.CurrentRow.IsNewRow && dgvMechanicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                e.Row.Cells["MachineSpareGroupID"].Value = Convert.ToInt32(dgvMechanicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
        }

        private void dgvElectricsSpares_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (dgvElectricsSpareGroups.SelectedCells.Count > 0 && !dgvElectricsSpareGroups.CurrentRow.IsNewRow && dgvElectricsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                e.Row.Cells["MachineSpareGroupID"].Value = Convert.ToInt32(dgvElectricsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
        }

        private void dgvPneumaticsSpares_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (dgvPneumaticsSpareGroups.SelectedCells.Count > 0 && !dgvPneumaticsSpareGroups.CurrentRow.IsNewRow && dgvPneumaticsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                e.Row.Cells["MachineSpareGroupID"].Value = Convert.ToInt32(dgvPneumaticsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
        }

        private void dgvHydraulicsSpares_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (dgvHydraulicsSpareGroups.SelectedCells.Count > 0 && !dgvHydraulicsSpareGroups.CurrentRow.IsNewRow && dgvHydraulicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                e.Row.Cells["MachineSpareGroupID"].Value = Convert.ToInt32(dgvHydraulicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
        }

        private void dgvAspirationSpares_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (dgvAspirationSpareGroups.SelectedCells.Count > 0 && !dgvAspirationSpareGroups.CurrentRow.IsNewRow && dgvAspirationSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                e.Row.Cells["MachineSpareGroupID"].Value = Convert.ToInt32(dgvAspirationSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
        }

        private void dgvMechanicsSpareGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;
            int MachineSpareGroupID = 0;
            if (dgvMechanicsSpareGroups.SelectedCells.Count != 0 && !dgvMechanicsSpareGroups.CurrentRow.IsNewRow && dgvMechanicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                MachineSpareGroupID = Convert.ToInt32(dgvMechanicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
            MachinesCatalogManager.FilterSpares(MachineSpareGroupID, MachineFileTypes.MachineMechanics);
        }

        private void dgvElectricsSpareGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;
            int MachineSpareGroupID = 0;
            if (dgvElectricsSpareGroups.SelectedCells.Count != 0 && !dgvElectricsSpareGroups.CurrentRow.IsNewRow && dgvElectricsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                MachineSpareGroupID = Convert.ToInt32(dgvElectricsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
            MachinesCatalogManager.FilterSpares(MachineSpareGroupID, MachineFileTypes.MachineElectrics);
        }

        private void dgvPneumaticsSpareGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;
            int MachineSpareGroupID = 0;
            if (dgvPneumaticsSpareGroups.SelectedCells.Count != 0 && dgvPneumaticsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                MachineSpareGroupID = Convert.ToInt32(dgvPneumaticsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
            MachinesCatalogManager.FilterSpares(MachineSpareGroupID, MachineFileTypes.MachinePneumatics);
        }

        private void dgvHydraulicsSpareGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;
            int MachineSpareGroupID = 0;
            if (dgvHydraulicsSpareGroups.SelectedCells.Count != 0 && dgvHydraulicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                MachineSpareGroupID = Convert.ToInt32(dgvHydraulicsSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
            MachinesCatalogManager.FilterSpares(MachineSpareGroupID, MachineFileTypes.MachineHydraulics);
        }

        private void dgvAspirationSpareGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (MachinesCatalogManager == null)
                return;
            int MachineSpareGroupID = 0;
            if (dgvAspirationSpareGroups.SelectedCells.Count != 0 && dgvAspirationSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value != DBNull.Value)
                MachineSpareGroupID = Convert.ToInt32(dgvAspirationSpareGroups.CurrentRow.Cells["MachineSpareGroupID"].Value);
            MachinesCatalogManager.FilterSpares(MachineSpareGroupID, MachineFileTypes.MachineAspiration);
        }

        private void AddDetailFile(PercentageDataGrid grid, MachineFileTypes Type)
        {
            CurrentGrid = grid;
            CurrentType = Type;

            if (CurrentGrid.SelectedCells.Count == 0
                || CurrentGrid.CurrentRow.IsNewRow || CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте запчасть, к которой будет прикреплен файл",
                   "Добавление файла");
                return;
            }

            CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineDetailID"].Value);
            openFileDialog9.ShowDialog();
        }

        private void btnAddMechanicsDetailFile_Click(object sender, EventArgs e)
        {
            AddDetailFile(dgvMechanicsDetails, MachineFileTypes.MechanicsDetails);
        }

        private void btnAddElectricsDetailFile_Click(object sender, EventArgs e)
        {
            AddDetailFile(dgvElectricsDetails, MachineFileTypes.ElectricsDetails);
        }

        private void btnAddPneumaticsDetailFile_Click(object sender, EventArgs e)
        {
            AddDetailFile(dgvPneumaticsDetails, MachineFileTypes.PneumaticsDetails);
        }

        private void btnAddHydraulicsDetailFile_Click(object sender, EventArgs e)
        {
            AddDetailFile(dgvHydraulicsDetails, MachineFileTypes.HydraulicsDetails);
        }

        private void btnAddAspirationDetailFile_Click(object sender, EventArgs e)
        {
            AddDetailFile(dgvAspirationDetails, MachineFileTypes.AspirationDetails);
        }

        private void RemoveDetailFile(PercentageDataGrid grid)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (grid.SelectedCells.Count > 0 && grid.CurrentRow.Cells["MachineDocumentID"].Value != DBNull.Value)
                CurrentMachineDocumentID = Convert.ToInt32(grid.CurrentRow.Cells["MachineDocumentID"].Value);
            else
                CurrentMachineDocumentID = 0;

            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void btnRemoveMechanicsDetailFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvMechanicsDetailFiles);
        }

        private void btnRemoveElectricsDetailFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvElectricsDetailFiles);
        }

        private void btnRemovePneumaticsDetailFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvPneumaticsDetailFiles);
        }

        private void btnRemoveHydraulicsDetailFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvHydraulicsDetailFiles);
        }

        private void btnRemoveAspirationDetailFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvAspirationDetailFiles);
        }

        private void kryptonButton15_Click(object sender, EventArgs e)
        {
            ComponentFactory.Krypton.Toolkit.KryptonButton btn = (ComponentFactory.Krypton.Toolkit.KryptonButton)sender;
            Panel pnl = (Panel)((Panel)btn.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel8.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel8.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p0.Height = tableLayoutPanel8.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void label49_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            Panel pnl = (Panel)((Panel)lbl.Parent).Parent;
            foreach (Panel p0 in tableLayoutPanel8.Controls)
            {
                Panel p1 = (Panel)p0.Controls[0];
                ComponentFactory.Krypton.Toolkit.KryptonButton btn1 = (ComponentFactory.Krypton.Toolkit.KryptonButton)((Panel)p0.Controls[1]).Controls[1];
                if (p0.Equals(pnl))
                {
                    if (p0.Height == tableLayoutPanel8.Height - 40 - 40 - 40 - 40 - 42)
                    {
                        btn1.Values.Image = Properties.Resources.Collapsed;
                        p1.Visible = false;
                        p0.Height = 40;
                    }
                    else
                    {
                        btn1.Values.Image = Properties.Resources.Explanded;
                        p1.Visible = true;
                        p0.Height = tableLayoutPanel8.Height - 40 - 40 - 40 - 40 - 42;
                    }
                }
                else
                {
                    btn1.Values.Image = Properties.Resources.Collapsed;
                    p1.Visible = false;
                    p0.Height = 40;
                }
            }
        }

        private void cmiSaveSpareGroups_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            MachinesCatalogManager.SaveSpareGroups();
            MachinesCatalogManager.ClearSpareGroups();

            MachinesCatalogManager.DeleteNullSpares();
            MachinesCatalogManager.DeleteNullDetails();
            MachinesCatalogManager.DeleteNullSparesOnStock();

            MachinesCatalogManager.RefreshSpareGroups();

            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void cmiSaveSpares_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            MachinesCatalogManager.SaveSpares();
            MachinesCatalogManager.ClearSpares();

            MachinesCatalogManager.DeleteNullDetails();
            MachinesCatalogManager.DeleteNullSparesOnStock();

            MachinesCatalogManager.RefreshSpares();

            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvMechanicsSpares_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;
                if (CurrentGrid.Rows[e.RowIndex].Cells["MachineSpareID"].Value != DBNull.Value)
                    CurrentMachineSpareID = Convert.ToInt32(CurrentGrid.Rows[e.RowIndex].Cells["MachineSpareID"].Value);
                else
                    CurrentMachineSpareID = 0;
                kryptonContextMenu8.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiAddSpareOnStock_Click(object sender, EventArgs e)
        {
            if (CurrentMachineDetailID != 0)
            {
                MachinesCatalogManager.AddSpareOnStock(CurrentMachineDetailID, MachineFileTypes.MachineMechanics);
                cmiSaveSparesOnStock_Click(null, null);
            }
        }

        private void cmiSaveSparesOnStock_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            MachinesCatalogManager.SaveSparesOnStock();
            MachinesCatalogManager.ClearSparesOnStock();
            MachinesCatalogManager.RefreshSparesOnStock(MachineID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Добавлено", 1700);
        }

        private void dgvMechanicsSparesOnStock_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;

                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem11_Click(object sender, EventArgs e)
        {
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            cmiRemoveRow_Click(null, null);
            MachinesCatalogManager.SaveSparesOnStock();
            MachinesCatalogManager.ClearSparesOnStock();
            MachinesCatalogManager.RefreshSparesOnStock(MachineID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
        }

        private void dgvElectricsSparesOnStock_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;

                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvPneumaticsSparesOnStock_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;

                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvHydraulicsSparesOnStock_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;

                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAspirationSparesOnStock_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                CurrentGrid = (PercentageDataGrid)sender;
                CurrentGrid.Rows[e.RowIndex].Selected = true;

                kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMechanicsSpareGroups_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                cmiSaveSpareGroups_Click(null, null);
        }

        private void dgvMechanicsSpares_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                cmiSaveSpares_Click(null, null);
        }

        private void dgvMechanicsDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                cmiSaveDetails_Click(null, null);
        }

        private void dgvExploitationTools_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                btnSaveTools_Click(null, null);
        }

        private void dgvTechnicalSpecification_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                btnSaveTechnics_Click(null, null);
        }

        private void dgvMachines_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                btnSaveMainParameters_Click(null, null);
        }

        private void dgvElectricsUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                CurrentGrid = dgvElectricsUnits;
                CurrentType = MachineFileTypes.MachineElectrics;
                cmiSaveUnits_Click(null, null);
            }
        }

        private void dgvMechanicsUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                CurrentGrid = dgvMechanicsUnits;
                CurrentType = MachineFileTypes.MachineMechanics;
                cmiSaveUnits_Click(null, null);
            }
        }

        private void dgvHydraulicsUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                CurrentGrid = dgvHydraulicsUnits;
                CurrentType = MachineFileTypes.MachineHydraulics;
                cmiSaveUnits_Click(null, null);
            }
        }

        private void dgvPneumaticsUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                CurrentGrid = dgvPneumaticsUnits;
                CurrentType = MachineFileTypes.MachineElectrics;
                cmiSaveUnits_Click(null, null);
            }
        }

        private void dgvAspirationUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                CurrentGrid = dgvAspirationUnits;
                CurrentType = MachineFileTypes.MachineAspiration;
                cmiSaveUnits_Click(null, null);
            }
        }

        private void dgvMechanicsSparesOnStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                cmiSaveSparesOnStock_Click(null, null);
        }

        private void SearchDetail(KryptonTextBox tb, PercentageDataGrid grid, MachineFileTypes Type)
        {
            string searchText = tb.Text;

            try
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;
                    if (ContainsText(row.Cells["SpareColumn"].FormattedValue.ToString(), searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        int MachineDetailID = Convert.ToInt32(row.Cells["MachineDetailID"].Value);
                        MachinesCatalogManager.MoveToMachineDetail(MachineDetailID, Type);
                        tb.Clear();
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void tbDetailSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KryptonTextBox tb = (KryptonTextBox)sender;

                SearchDetail(tb, dgvMechanicsDetails, MachineFileTypes.MechanicsDetails);
            }
        }

        private void kryptonTextBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KryptonTextBox tb = (KryptonTextBox)sender;

                SearchDetail(tb, dgvAspirationDetails, MachineFileTypes.AspirationDetails);
            }
        }

        private void kryptonTextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KryptonTextBox tb = (KryptonTextBox)sender;

                SearchDetail(tb, dgvHydraulicsDetails, MachineFileTypes.HydraulicsDetails);
            }
        }

        private void kryptonTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KryptonTextBox tb = (KryptonTextBox)sender;

                SearchDetail(tb, dgvPneumaticsDetails, MachineFileTypes.PneumaticsDetails);
            }
        }

        private void kryptonTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KryptonTextBox tb = (KryptonTextBox)sender;

                SearchDetail(tb, dgvElectricsDetails, MachineFileTypes.ElectricsDetails);
            }
        }

        private void AddToolsFile(PercentageDataGrid grid, MachineFileTypes Type)
        {
            CurrentGrid = grid;
            CurrentType = Type;
            if (CurrentGrid.SelectedCells.Count == 0
                || CurrentGrid.CurrentRow.IsNewRow || CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте инструмент, к которому будет прикреплен файл",
                   "Добавление файла");
                return;
            }

            CurrentMachineDetailID = Convert.ToInt32(CurrentGrid.CurrentRow.Cells["MachineToolsID"].Value);
            openFileDialog9.ShowDialog();
        }

        private void btnAddExploitationToolsFile_Click(object sender, EventArgs e)
        {
            AddToolsFile(dgvExploitationTools, MachineFileTypes.ExploitationTools);
        }

        private void btnAddRepairToolsFile_Click(object sender, EventArgs e)
        {
            AddToolsFile(dgvRepairTools, MachineFileTypes.RepairTools);
        }

        private void btnAddServiceToolsFile_Click(object sender, EventArgs e)
        {
            AddToolsFile(dgvServiceTools, MachineFileTypes.ServiceTools);
        }

        private void btnAddLubricantToolsFile_Click(object sender, EventArgs e)
        {
            AddToolsFile(dgvLubricant, MachineFileTypes.Lubricant);
        }

        private void btnRemoveExploitationToolsFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvExploitationToolsFiles);
        }

        private void btnRemoveRepairToolsFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvRepairToolsFiles);
        }

        private void btnRemoveServiceToolsFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvServiceToolsFiles);
        }

        private void btnRemoveLubricantToolsFile_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvLubricantFiles);
        }

        private void dgvMachines_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMachines.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void cbPagesNavigation_SelectionChangeCommitted(object sender, EventArgs e)
        {
            KryptonComboBox cb = (KryptonComboBox)sender;
            DefinePageName(((DataRowView)cb.SelectedItem).Row["ValueMember"].ToString());
        }

        private void dgvMachines_KeyUp(object sender, KeyEventArgs e)
        {
            //if ((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))
            //{
            //    char[] rowSplitter = { '\r', '\n' };
            //    char[] columnSplitter = { '\t' };
            //    //get the text from clipboard
            //    IDataObject dataInClipboard = Clipboard.GetDataObject();
            //    string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);
            //    //split it into lines
            //    string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);
            //    //get the row and column of selected cell in grid
            //    int r = dgvMachines.SelectedCells[0].RowIndex;
            //    int c = dgvMachines.SelectedCells[0].ColumnIndex;
            //    //add rows into grid to fit clipboard lines
            //    if (dgvMachines.Rows.Count < (r + rowsInClipboard.Length))
            //    {
            //        dgvMachines.Rows.Add(r + rowsInClipboard.Length - dgvMachines.Rows.Count);
            //    }
            //    // loop through the lines, split them into cells and place the values in the corresponding cell.
            //    for (int iRow = 0; iRow < rowsInClipboard.Length; iRow++)
            //    {
            //        //split row into cell values
            //        string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);
            //        //cycle through cell values
            //        for (int iCol = 0; iCol < valuesInRow.Length; iCol++)
            //        {
            //            //assign cell value, only if it within columns of the grid
            //            if (dgvMachines.ColumnCount - 1 >= c + iCol)
            //            {
            //                dgvMachines.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol];
            //            }
            //        }
            //    }
            //}
            //if ((e.Control && e.KeyCode == Keys.C))
            //{
            //    CopyDataGridViewToClipboard(ref dgvMachines);
            //}
        }

        private void CopyDataGridViewToClipboard(ref PercentageDataGrid dgv)
        {
            string s = string.Empty;

            if (dgv.CurrentCell != null && dgv.CurrentCell.Value != DBNull.Value)
            {
                s = dgv.CurrentCell.Value.ToString();
                Clipboard.GetData(s);
            }
        }

        private void kryptonLinkLabel1_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlSparesCatalog.Name);
            lblMachineName.Text = string.Empty;
        }

        private void btnSaveSpares_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;

            MachinesCatalogManager.SaveSpareGroups();
            MachinesCatalogManager.ClearSpareGroups();
            MachinesCatalogManager.RefreshSpareGroups();

            MachinesCatalogManager.SaveSpares();
            MachinesCatalogManager.ClearSpares();

            MachinesCatalogManager.DeleteNullSpares();
            MachinesCatalogManager.DeleteNullDetails();
            MachinesCatalogManager.DeleteNullSparesOnStock();

            MachinesCatalogManager.RefreshSpares();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void kryptonButton16_Click(object sender, EventArgs e)
        {
            DefinePageName(pnlMainPage.Name);
            MachineName = dgvMachines.CurrentRow.Cells["MachineName"].Value.ToString();
            lblMachineName.Text = MachineName;
        }

        private void btnAddOperatingInstruction_Click(object sender, EventArgs e)
        {
            CurrentType = MachineFileTypes.OperatingInstructions;

            if (dgvMachines.SelectedCells.Count == 0
                || dgvMachines.CurrentRow.IsNewRow || dgvMachines.CurrentRow.Cells["MachineID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog10.ShowDialog();
        }

        private void btnRemoveOperatingInstruction_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (dgvOperatingInstructions.SelectedCells.Count > 0 && dgvOperatingInstructions.CurrentRow.Cells["MachineDocumentID"].Value != DBNull.Value)
                CurrentMachineDocumentID = Convert.ToInt32(dgvOperatingInstructions.CurrentRow.Cells["MachineDocumentID"].Value);
            else
                CurrentMachineDocumentID = 0;

            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void openFileDialog10_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog10.FileName);

            if (fileInfo.Length > 16291000)
            {
                MessageBox.Show("Файл больше 15 МБ и не может быть загружен");
                return;
            }
            AttachsDT.Clear();

            DataRow NewRow = AttachsDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog10.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog10.FileName;
            AttachsDT.Rows.Add(NewRow);

            if (AttachsDT.Rows.Count != 0)
            {
                if (dgvMachines.SelectedCells.Count == 1)
                {
                    int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
                    if (NeedSplash)
                    {
                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, CurrentType);
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                    else
                    {
                        MachinesCatalogManager.AttachMachineDocument(AttachsDT, MachineID, CurrentType);
                    }
                }
            }
            MachinesCatalogManager.ClearMachineDocuments();
            MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
        }

        private void btnAddServiceInstruction_Click(object sender, EventArgs e)
        {
            CurrentType = MachineFileTypes.ServiceInstructions;

            if (dgvMachines.SelectedCells.Count == 0
                || dgvMachines.CurrentRow.IsNewRow || dgvMachines.CurrentRow.Cells["MachineID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog10.ShowDialog();
        }

        private void btnRemoveServiceInstruction_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (dgvServiceInstructions.SelectedCells.Count > 0 && dgvServiceInstructions.CurrentRow.Cells["MachineDocumentID"].Value != DBNull.Value)
                CurrentMachineDocumentID = Convert.ToInt32(dgvServiceInstructions.CurrentRow.Cells["MachineDocumentID"].Value);
            else
                CurrentMachineDocumentID = 0;

            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void btnAddLaborProtInstruction_Click(object sender, EventArgs e)
        {
            CurrentType = MachineFileTypes.LaborProtInstructions;

            if (dgvMachines.SelectedCells.Count == 0
                || dgvMachines.CurrentRow.IsNewRow || dgvMachines.CurrentRow.Cells["MachineID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog10.ShowDialog();
        }

        private void btnRemoveLaborProtInstruction_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (dgvLaborProtInstructions.SelectedCells.Count > 0 && dgvLaborProtInstructions.CurrentRow.Cells["MachineDocumentID"].Value != DBNull.Value)
                CurrentMachineDocumentID = Convert.ToInt32(dgvLaborProtInstructions.CurrentRow.Cells["MachineDocumentID"].Value);
            else
                CurrentMachineDocumentID = 0;

            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void btnAddAdmission_Click(object sender, EventArgs e)
        {
            CurrentType = MachineFileTypes.Admissions;

            if (dgvMachines.SelectedCells.Count == 0
                || dgvMachines.CurrentRow.IsNewRow || dgvMachines.CurrentRow.Cells["MachineID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog10.ShowDialog();
        }

        private void btnRemoveAdmission_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (dgvAdmissions.SelectedCells.Count > 0 && dgvAdmissions.CurrentRow.Cells["MachineDocumentID"].Value != DBNull.Value)
                CurrentMachineDocumentID = Convert.ToInt32(dgvAdmissions.CurrentRow.Cells["MachineDocumentID"].Value);
            else
                CurrentMachineDocumentID = 0;

            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void lbtnpnlInstructions_LinkClicked(object sender, EventArgs e)
        {
            DefinePageName(pnlInstructions.Name);
        }

        private void btnAddMachine_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewMachineForm MachinesSummaryForm = new NewMachineForm(this, ref MachinesCatalogManager);
            TopForm = MachinesSummaryForm;
            MachinesSummaryForm.ShowDialog();

            PressOK = MachinesSummaryForm.PressOK;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MachinesSummaryForm.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
        }

        private void btnAddJournal_Click(object sender, EventArgs e)
        {
            CurrentType = MachineFileTypes.Journal;

            if (dgvMachines.SelectedCells.Count == 0
                || dgvMachines.CurrentRow.IsNewRow || dgvMachines.CurrentRow.Cells["MachineID"].Value == DBNull.Value)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет станка",
                   "Добавление файла");
                return;
            }

            openFileDialog10.ShowDialog();
        }

        private void btnRemoveJournal_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            if (dgvJournal.SelectedCells.Count > 0 && dgvJournal.CurrentRow.Cells["MachineDocumentID"].Value != DBNull.Value)
                CurrentMachineDocumentID = Convert.ToInt32(dgvJournal.CurrentRow.Cells["MachineDocumentID"].Value);
            else
                CurrentMachineDocumentID = 0;

            if (CurrentMachineDocumentID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                MachinesCatalogManager.RemoveMachineDocuments(CurrentMachineDocumentID);
                MachinesCatalogManager.ClearMachineDocuments();
                MachinesCatalogManager.CopyMachineDocuments(Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value));
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void dgvElectricsDetails_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvAspirationDetails_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvHydraulicsDetails_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvPneumaticsDetails_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvMechanicsDetails_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvLubricant_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvServiceTools_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvRepairTools_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvExploitationTools_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvEquipment_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvMechanicsUnits_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvAspirationUnits_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvElectricsUnits_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvHydraulicsUnits_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void dgvPneumaticsUnits_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "IndexNumber")
                e.Value = e.RowIndex + 1;
        }

        private void MenuFileSaveFile_Click(object sender, EventArgs e)
        {

        }

        private void SaveFileToDisk()
        {
            if (CurrentMachineDocumentID == 0)
                return;

            long FileSize = 0;
            string sFileName = string.Empty;

            MachinesCatalogManager.GetMachineDocumentInfo(CurrentMachineDocumentID, ref FileSize, ref sFileName);
            var fileInfo = new System.IO.FileInfo(sFileName);
            SaveFileDialog.FileName = Path.GetFileNameWithoutExtension(sFileName);
            SaveFileDialog.DefaultExt = fileInfo.Extension;
            SaveFileDialog.Filter = "(*" + fileInfo.Extension + ")|*" + fileInfo.Extension + "|All files (*.*)|*.*";

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bool bOk = MachinesCatalogManager.SaveFile(FileSize, sFileName, SaveFileDialog.FileName);
                if (bOk)
                    InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
            }
        }

        private void cmiSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileToDisk();
        }

        private void dgvOperatingInstructions_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvServiceInstructions_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvLaborProtInstructions_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAdmissions_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvJournal_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            SaveFileToDisk();
        }

        private void dgvPneumaticsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMechanicsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvHydraulicsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvElectricsFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvAspirationFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMachinesStructure_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value != DBNull.Value)
                    CurrentMachineDocumentID = Convert.ToInt32(((PercentageDataGrid)sender).Rows[e.RowIndex].Cells["MachineDocumentID"].Value);
                else
                    CurrentMachineDocumentID = 0;
                kryptonContextMenu10.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonButton30_Click(object sender, EventArgs e)
        {
            AddToolsFile(dgvEquipment, MachineFileTypes.EquipmentTools);
        }

        private void kryptonButton31_Click(object sender, EventArgs e)
        {
            RemoveDetailFile(dgvEquipmentFiles);
        }

        DataView dv;
        private void dgvMechanicsDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 14)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineMechanics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvMechanicsDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineMechanics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvElectricsDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 14)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineElectrics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvElectricsDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineElectrics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvPneumaticsDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 14)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachinePneumatics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvPneumaticsDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachinePneumatics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvHydraulicsDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 14)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineHydraulics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvHydraulicsDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineHydraulics);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvAspirationDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 14)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineAspiration);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void dgvAspirationDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 13)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                DataGridViewComboBoxCell dgvCbo = grid[14, e.RowIndex] as DataGridViewComboBoxCell;
                if (dgvCbo != null)
                {
                    string str = string.Empty;
                    if (grid[13, grid.CurrentRow.Index].Value != null)
                    {
                        str = grid[13, grid.CurrentRow.Index].Value.ToString();
                    }
                    if (string.IsNullOrEmpty(str))
                        return;
                    dv = MachinesCatalogManager.S(Convert.ToInt32(str), MachineFileTypes.MachineAspiration);
                    grid[14, grid.CurrentRow.Index].Value = DBNull.Value;
                    dgvCbo.DataSource = dv;
                }
            }
        }

        private void btnAddTechnicalSpecification_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            TechnicalSpecificationForm TechnicalSpecificationForm = new TechnicalSpecificationForm(false, this, ref MachinesCatalogManager);
            TopForm = TechnicalSpecificationForm;
            TechnicalSpecificationForm.ShowDialog();

            PressOK = TechnicalSpecificationForm.PressOK;

            PhantomForm.Close();
            PhantomForm.Dispose();
            TechnicalSpecificationForm.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
            MachinesCatalogManager.CurrentTechnicalSpecification = MachinesCatalogManager.GetTechnicalSpecification();
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
        }

        private void btnRemoveTechnicalSpecification_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole || dgvTechnicalSpecification.CurrentRow == null)
                return;
            int Index = dgvTechnicalSpecification.CurrentRow.Index;

            if (dgvTechnicalSpecification.CurrentRow.IsNewRow)
                return;

            dgvTechnicalSpecification.Rows.RemoveAt(Index);

            if (dgvTechnicalSpecification.Rows.Count - 2 == Index)
                Index--;
            if (Index == -1)
                Index = 0;

            if (dgvTechnicalSpecification.Rows.Count == 1 || dgvTechnicalSpecification.Rows.Count == 0)
                return;
            dgvTechnicalSpecification.CurrentCell = dgvTechnicalSpecification.Rows[Index].Cells[0];
            dgvTechnicalSpecification.Rows[Index].Selected = true;
        }

        private void btnEditTechnicalSpecification_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            TechnicalSpecificationForm TechnicalSpecificationForm = new TechnicalSpecificationForm(true, this, ref MachinesCatalogManager);
            TopForm = TechnicalSpecificationForm;
            TechnicalSpecificationForm.ShowDialog();

            PressOK = TechnicalSpecificationForm.PressOK;

            PhantomForm.Close();
            PhantomForm.Dispose();
            TechnicalSpecificationForm.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
            MachinesCatalogManager.CurrentTechnicalSpecification = MachinesCatalogManager.GetTechnicalSpecification();
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
        }

        private void btnSavePermanentWorks_Click(object sender, EventArgs e)
        {
            if (RoleType == RoleTypes.OrdinaryRole)
                return;
            int MachineID = 0;
            if (dgvMachines.SelectedCells.Count != 0 && dgvMachines.CurrentRow.Cells["MachineID"].Value != DBNull.Value)
                MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            MachinesCatalogManager.CurrentPermanentWorks = rtbPermanentWorks.Text;
            MachinesCatalogManager.SaveAndRefreshMachines(MachineID);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvMechanicsDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string ColName = dgvMechanicsDetails.Columns[e.ColumnIndex].Name;
        }

        private void dgvMachines_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            bool PressOK = false;
            int MachineID = Convert.ToInt32(dgvMachines.CurrentRow.Cells["MachineID"].Value);
            string MachineName = dgvMachines.CurrentRow.Cells["MachineName"].FormattedValue.ToString();

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewMachineForm MachinesSummaryForm = new NewMachineForm(this, ref MachinesCatalogManager, MachineID, MachineName);
            TopForm = MachinesSummaryForm;
            MachinesSummaryForm.ShowDialog();

            PressOK = MachinesSummaryForm.PressOK;

            PhantomForm.Close();
            PhantomForm.Dispose();
            MachinesSummaryForm.Dispose();
            TopForm = null;

            if (!PressOK)
                return;
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            MachinesCatalogManager.FindMachines(CurrentMachineSpareID);
            panel163.Location = new Point(pnl1.Width / 2 - panel163.Size.Width / 2, pnl1.Height / 2 - panel163.Size.Height / 2);
            panel163.Anchor = AnchorStyles.None;
            panel163.Visible = true;
        }

        private void kryptonButton32_Click(object sender, EventArgs e)
        {
            panel163.Visible = false;
        }
    }
}
