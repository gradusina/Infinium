using Infinium.Modules.WorkAssignments;

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class WorkAssignmentsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;

        int FactoryID = 1;
        int FormEvent = 0;

        ArrayList FrontsID;

        LightStartForm LightStartForm;
        Form TopForm = null;

        FileManager FM = new FileManager();
        CreationAssignments CreationAssignmentsManager;
        ControlAssignments ControlAssignmentsManager;

        FrontsOrdersManager MarketFrontsOrdersManager;
        FrontsOrdersManager ZOVFrontsOrdersManager;
        FrontsOrdersManager WFrontsOrdersManager;

        DecorOrdersManager MarketDecorOrdersManager;
        DecorOrdersManager ZOVDecorOrdersManager;
        DecorOrdersManager WDecorOrdersManager;

        //Techno4DominoAssignments Techno4DominoAssignments;
        ProfilAngle45Assignments ProfilAngle45Assignments;
        ProfilAngle90Assignments ProfilAngle90Assignments;
        ImpostAssignments ImpostAssignments;
        DecorAssignments DecorAssignments;
        TPSAngle45Assignments TPSAngle45Assignments;
        GenevaAssignments GenevaAssignments;
        TafelAssignments TafelAssignments;
        Tafel1Assignments Tafel1Assignments;

        public WorkAssignmentsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void WorkAssignmentsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            NeedSplash = true;
        }

        private void WorkAssignmentsForm_Load(object sender, EventArgs e)
        {
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
                        this.Close();
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
                    NeedSplash = true;
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
                        this.Close();
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
                    NeedSplash = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
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
            MarketFrontsOrdersManager = new FrontsOrdersManager();
            MarketFrontsOrdersManager.Initialize();
            ZOVFrontsOrdersManager = new FrontsOrdersManager();
            ZOVFrontsOrdersManager.Initialize();
            WFrontsOrdersManager = new FrontsOrdersManager();
            WFrontsOrdersManager.Initialize();

            MarketDecorOrdersManager = new DecorOrdersManager();
            MarketDecorOrdersManager.Initialize();
            ZOVDecorOrdersManager = new DecorOrdersManager();
            ZOVDecorOrdersManager.Initialize();
            WDecorOrdersManager = new DecorOrdersManager();
            WDecorOrdersManager.Initialize();

            FrontsID = new ArrayList();
            //Techno4DominoAssignments = new Techno4DominoAssignments();
            //Techno4DominoAssignments.Initialize();
            ProfilAngle45Assignments = new ProfilAngle45Assignments();
            ProfilAngle45Assignments.Initialize();
            ProfilAngle90Assignments = new ProfilAngle90Assignments();
            ProfilAngle90Assignments.Initialize();
            ImpostAssignments = new ImpostAssignments();
            ImpostAssignments.Initialize();
            DecorAssignments = new DecorAssignments();
            DecorAssignments.Initialize();
            TPSAngle45Assignments = new TPSAngle45Assignments();
            TPSAngle45Assignments.Initialize();
            GenevaAssignments = new GenevaAssignments();
            GenevaAssignments.Initialize();
            Tafel1Assignments = new Tafel1Assignments();
            Tafel1Assignments.Initialize();
            TafelAssignments = new TafelAssignments();
            TafelAssignments.Initialize();

            CreationAssignmentsManager = new CreationAssignments();
            CreationAssignmentsManager.Initialize();
            ControlAssignmentsManager = new ControlAssignments();
            ControlAssignmentsManager.Initialize();

            DataBinding();

            FrontsSummaryGridSetting(ref dgvFrontsSummary);
            FrontsSummaryGridSetting(ref dgvFrameColorsSummary);
            FrontsSummaryGridSetting(ref dgvInsetTypesSummary);
            FrontsSummaryGridSetting(ref dgvInsetColorsSummary);
            FrontsSummaryGridSetting(ref dgvSizesSummary);

            DecorSummaryGridSetting(ref dgvDecorProducts);
            DecorSummaryGridSetting(ref dgvDecorItems);
            DecorSummaryGridSetting(ref dgvDecorColors);
            DecorSummaryGridSetting(ref dgvDecorSizes);

            FrontsGridSetting(ref dgvMarketBatchFronts);
            FrontsGridSetting(ref dgvZOVBatchFronts);
            FrontsGridSetting(ref dgvWBatchFronts);

            DecorGridSetting(ref dgvMarketBatchDecor, MarketDecorOrdersManager);
            DecorGridSetting(ref dgvZOVBatchDecor, ZOVDecorOrdersManager);
            DecorGridSetting(ref dgvWBatchDecor, WDecorOrdersManager);

            BatchesGridSetting(ref dgvMarketBatches);
            BatchesGridSetting(ref dgvZOVBatches);
            BatchesGridSetting(ref dgvWBatches);

            MegaBatchesGridSetting(ref dgvMarketMegaBatches);
            MegaBatchesGridSetting(ref dgvZOVMegaBatches);
            MegaBatchesGridSetting(ref dgvWMegaBatches);

            dgvWorkAssignments.Columns.Add(ControlAssignmentsManager.MachineColumn);
            dgvWorkAssignmentsSetting(ref dgvWorkAssignments);

            AddCheckColumn(ref dgvMarketBatches);
            AddCheckColumn(ref dgvZOVBatches);

            CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);

            CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);

            FilterMegaBatch();

            ShowColumns(FactoryID, ref dgvMarketMegaBatches, ref dgvMarketBatches);
            ShowColumns(FactoryID, ref dgvZOVMegaBatches, ref dgvZOVBatches);

            DateTimePicker1.ValueNullable = DateTime.Today.AddDays(-60);
            DateTimePicker2.ValueNullable = DateTime.Today;

            DateTime date1 = DateTime.Today.AddDays(-60);
            DateTime date2 = DateTime.Today;

            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);
        }

        private void DataBinding()
        {
            dgvFrontsSummary.DataSource = WFrontsOrdersManager.FrontsSummaryList;
            dgvFrameColorsSummary.DataSource = WFrontsOrdersManager.FrameColorsSummaryList;
            dgvInsetTypesSummary.DataSource = WFrontsOrdersManager.InsetTypesSummaryList;
            dgvInsetColorsSummary.DataSource = WFrontsOrdersManager.InsetColorsSummaryList;
            dgvSizesSummary.DataSource = WFrontsOrdersManager.SizesSummaryList;

            dgvDecorProducts.DataSource = WDecorOrdersManager.DecorProductsSummaryList;
            dgvDecorItems.DataSource = WDecorOrdersManager.DecorItemsSummaryList;
            dgvDecorColors.DataSource = WDecorOrdersManager.DecorColorsSummaryList;
            dgvDecorSizes.DataSource = WDecorOrdersManager.DecorSizesSummaryList;

            dgvMarketBatchFronts.DataSource = MarketFrontsOrdersManager.BatchFrontsList;
            dgvMarketBatchDecor.DataSource = MarketDecorOrdersManager.BatchDecorList;
            dgvMarketBatches.DataSource = CreationAssignmentsManager.MarketBatchesList;
            dgvMarketMegaBatches.DataSource = CreationAssignmentsManager.MarketMegaBatchesList;

            dgvZOVBatchFronts.DataSource = ZOVFrontsOrdersManager.BatchFrontsList;
            dgvZOVBatchDecor.DataSource = ZOVDecorOrdersManager.BatchDecorList;
            dgvZOVBatches.DataSource = CreationAssignmentsManager.ZOVBatchesList;
            dgvZOVMegaBatches.DataSource = CreationAssignmentsManager.ZOVMegaBatchesList;

            dgvWBatchFronts.DataSource = WFrontsOrdersManager.BatchFrontsList;
            dgvWBatchDecor.DataSource = WDecorOrdersManager.BatchDecorList;
            dgvWBatches.DataSource = ControlAssignmentsManager.BatchesList;
            dgvWMegaBatches.DataSource = ControlAssignmentsManager.MegaBatchesList;

            dgvWorkAssignments.DataSource = ControlAssignmentsManager.WorkAssignmentsList;
        }

        #region GridSettings

        private void ShowColumns(int FactoryID, ref PercentageDataGrid grid1, ref PercentageDataGrid grid2)
        {
            if (FactoryID == 1)
            {
                grid1.Columns["ProfilEntryDateTime"].Visible = true;
                grid1.Columns["TPSEntryDateTime"].Visible = false;

                grid2.Columns["ProfilName"].Visible = true;
                grid2.Columns["TPSName"].Visible = false;

                //dgvWorkAssignments.Columns["TPS45DateTime"].Visible = false;
                //dgvWorkAssignments.Columns["GenevaDateTime"].Visible = false;
                //dgvWorkAssignments.Columns["TafelDateTime"].Visible = false;
                dgvWorkAssignments.Columns["Profil90DateTime"].Visible = true;
                dgvWorkAssignments.Columns["Profil45DateTime"].Visible = true;
                dgvWorkAssignments.Columns["DominoDateTime"].Visible = true;

                //if (dgvWorkAssignments.Columns.Contains("TPS45UserColumn"))
                //    dgvWorkAssignments.Columns["TPS45UserColumn"].Visible = false;
                //if (dgvWorkAssignments.Columns.Contains("GenevaUserColumn"))
                //    dgvWorkAssignments.Columns["GenevaUserColumn"].Visible = false;
                //if (dgvWorkAssignments.Columns.Contains("TafelUserColumn"))
                //    dgvWorkAssignments.Columns["TafelUserColumn"].Visible = false;
                if (dgvWorkAssignments.Columns.Contains("Profil90UserColumn"))
                    dgvWorkAssignments.Columns["Profil90UserColumn"].Visible = true;
                if (dgvWorkAssignments.Columns.Contains("Profil45UserColumn"))
                    dgvWorkAssignments.Columns["Profil45UserColumn"].Visible = true;
                if (dgvWorkAssignments.Columns.Contains("DominoUserColumn"))
                    dgvWorkAssignments.Columns["DominoUserColumn"].Visible = true;
            }
            if (FactoryID == 2)
            {
                grid1.Columns["ProfilEntryDateTime"].Visible = false;
                grid1.Columns["TPSEntryDateTime"].Visible = true;

                grid2.Columns["ProfilName"].Visible = false;
                grid2.Columns["TPSName"].Visible = true;

                dgvWorkAssignments.Columns["TPS45DateTime"].Visible = true;
                dgvWorkAssignments.Columns["GenevaDateTime"].Visible = true;
                dgvWorkAssignments.Columns["TafelDateTime"].Visible = true;
                dgvWorkAssignments.Columns["Profil90DateTime"].Visible = false;
                dgvWorkAssignments.Columns["Profil45DateTime"].Visible = false;
                dgvWorkAssignments.Columns["DominoDateTime"].Visible = false;

                if (dgvWorkAssignments.Columns.Contains("TPS45UserColumn"))
                    dgvWorkAssignments.Columns["TPS45UserColumn"].Visible = true;
                if (dgvWorkAssignments.Columns.Contains("GenevaUserColumn"))
                    dgvWorkAssignments.Columns["GenevaUserColumn"].Visible = true;
                if (dgvWorkAssignments.Columns.Contains("TafelUserColumn"))
                    dgvWorkAssignments.Columns["TafelUserColumn"].Visible = true;
                if (dgvWorkAssignments.Columns.Contains("Profil90UserColumn"))
                    dgvWorkAssignments.Columns["Profil90UserColumn"].Visible = false;
                if (dgvWorkAssignments.Columns.Contains("Profil45UserColumn"))
                    dgvWorkAssignments.Columns["Profil45UserColumn"].Visible = false;
                if (dgvWorkAssignments.Columns.Contains("DominoUserColumn"))
                    dgvWorkAssignments.Columns["DominoUserColumn"].Visible = false;
            }
        }

        private void dgvWorkAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn FilenkaDateTimeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                //FilenkaDateTimeColumn.CalendarTodayDate = new System.DateTime(2013, 4, 10, 0, 0, 0, 0);
                Checked = false,
                DataPropertyName = "FilenkaDateTime",
                HeaderText = "Филенка",
                Name = "FilenkaDateTimeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn TrimmingDateTimeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                //TrimmingDateTimeColumn.CalendarTodayDate = DateTime.Now;
                Checked = false,
                DataPropertyName = "TrimmingDateTime",
                HeaderText = "Торцовка",
                Name = "TrimmingDateTimeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn AssemblyDateTimeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                //AssemblyDateTimeColumn.CalendarTodayDate = new System.DateTime(2013, 4, 10, 0, 0, 0, 0);
                Checked = false,
                DataPropertyName = "AssemblyDateTime",
                HeaderText = "Сборка",
                Name = "AssemblyDateTimeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn DeyingDateTimeColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewDateTimePickerColumn()
            {
                //DeyingDateTimeColumn.CalendarTodayDate = new System.DateTime(2013, 4, 10, 0, 0, 0, 0);
                Checked = false,
                DataPropertyName = "DeyingDateTime",
                HeaderText = "Покраска",
                Name = "DeyingDateTimeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            grid.Columns.Add(FilenkaDateTimeColumn);
            grid.Columns.Add(TrimmingDateTimeColumn);
            grid.Columns.Add(AssemblyDateTimeColumn);
            grid.Columns.Add(DeyingDateTimeColumn);
            grid.Columns.Add(ControlAssignmentsManager.TPS45UserColumn);
            grid.Columns.Add(ControlAssignmentsManager.GenevaUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.TafelUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.RALUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.Profil90UserColumn);
            grid.Columns.Add(ControlAssignmentsManager.Profil45UserColumn);
            grid.Columns.Add(ControlAssignmentsManager.DominoUserColumn);

            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("TrimmingDateTime"))
                grid.Columns["TrimmingDateTime"].Visible = false;
            if (grid.Columns.Contains("FilenkaDateTime"))
                grid.Columns["FilenkaDateTime"].Visible = false;
            if (grid.Columns.Contains("AssemblyDateTime"))
                grid.Columns["AssemblyDateTime"].Visible = false;
            if (grid.Columns.Contains("DeyingDateTime"))
                grid.Columns["DeyingDateTime"].Visible = false;
            if (grid.Columns.Contains("Machine"))
                grid.Columns["Machine"].Visible = false;
            if (grid.Columns.Contains("FactoryID"))
                grid.Columns["FactoryID"].Visible = false;
            if (grid.Columns.Contains("Changed"))
                grid.Columns["Changed"].Visible = false;
            if (grid.Columns.Contains("TPS45UserID"))
                grid.Columns["TPS45UserID"].Visible = false;
            if (grid.Columns.Contains("GenevaUserID"))
                grid.Columns["GenevaUserID"].Visible = false;
            if (grid.Columns.Contains("TafelUserID"))
                grid.Columns["TafelUserID"].Visible = false;
            if (grid.Columns.Contains("Profil90UserID"))
                grid.Columns["Profil90UserID"].Visible = false;
            if (grid.Columns.Contains("Profil45UserID"))
                grid.Columns["Profil45UserID"].Visible = false;
            if (grid.Columns.Contains("DominoUserID"))
                grid.Columns["DominoUserID"].Visible = false;
            if (grid.Columns.Contains("RALUserID"))
                grid.Columns["RALUserID"].Visible = false;
            if (grid.Columns.Contains("TPS45PrintingStatus"))
                grid.Columns["TPS45PrintingStatus"].Visible = false;
            if (grid.Columns.Contains("GenevaPrintingStatus"))
                grid.Columns["GenevaPrintingStatus"].Visible = false;
            if (grid.Columns.Contains("TafelPrintingStatus"))
                grid.Columns["TafelPrintingStatus"].Visible = false;
            if (grid.Columns.Contains("RALPrintingStatus"))
                grid.Columns["RALPrintingStatus"].Visible = false;
            if (grid.Columns.Contains("Profil90PrintingStatus"))
                grid.Columns["Profil90PrintingStatus"].Visible = false;
            if (grid.Columns.Contains("Profil45PrintingStatus"))
                grid.Columns["Profil45PrintingStatus"].Visible = false;
            if (grid.Columns.Contains("DominoPrintingStatus"))
                grid.Columns["DominoPrintingStatus"].Visible = false;

            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("ResponsibleDateTime"))
                grid.Columns["ResponsibleDateTime"].Visible = false;
            if (grid.Columns.Contains("ResponsibleUserID"))
                grid.Columns["ResponsibleUserID"].Visible = false;
            if (grid.Columns.Contains("TechnologyDateTime"))
                grid.Columns["TechnologyDateTime"].Visible = false;
            if (grid.Columns.Contains("TechnologyUserID"))
                grid.Columns["TechnologyUserID"].Visible = false;
            if (grid.Columns.Contains("ControlDateTime"))
                grid.Columns["ControlDateTime"].Visible = false;
            if (grid.Columns.Contains("ControlUserID"))
                grid.Columns["ControlUserID"].Visible = false;
            if (grid.Columns.Contains("AgreementDateTime"))
                grid.Columns["AgreementDateTime"].Visible = false;
            if (grid.Columns.Contains("AgreementUserID"))
                grid.Columns["AgreementUserID"].Visible = false;

            grid.Columns["CreationDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["TPS45DateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["GenevaDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["TafelDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["Profil90DateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["Profil45DateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["DominoDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["PrintDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["RALDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["WorkAssignmentID"].HeaderText = "№";
            grid.Columns["Name"].HeaderText = "Название";
            grid.Columns["CreationDateTime"].HeaderText = "Дата создания";
            grid.Columns["TPS45DateTime"].HeaderText = "Угол 45 ТПС\r\nдата печати";
            grid.Columns["GenevaDateTime"].HeaderText = "   Женева\r\nдата печати";
            grid.Columns["TafelDateTime"].HeaderText = "   Тафель\r\nдата печати";
            grid.Columns["Profil90DateTime"].HeaderText = "   Угол 90\r\nдата печати";
            grid.Columns["Profil45DateTime"].HeaderText = "   Угол 45\r\nдата печати";
            grid.Columns["DominoDateTime"].HeaderText = "   Домино\r\nдата печати";
            grid.Columns["RALDateTime"].HeaderText = "     RAL\r\nдата печати";
            grid.Columns["PrintDateTime"].HeaderText = "Дата печати";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Square"].HeaderText = "Квадратура";

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //Column.HeaderCell.Style.WrapMode = DataGridViewTriState.True;
            }

            grid.Columns["TrimmingDateTimeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TrimmingDateTimeColumn"].Width = 100;
            grid.Columns["FilenkaDateTimeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["FilenkaDateTimeColumn"].Width = 100;
            grid.Columns["AssemblyDateTimeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["AssemblyDateTimeColumn"].Width = 100;
            grid.Columns["DeyingDateTimeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DeyingDateTimeColumn"].Width = 100;
            grid.Columns["WorkAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["WorkAssignmentID"].Width = 55;
            grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Square"].Width = 100;
            grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Name"].Width = 155;

            grid.Columns["CreationDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CreationDateTime"].Width = 115;
            grid.Columns["TPS45DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TPS45DateTime"].Width = 115;
            grid.Columns["GenevaDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["GenevaDateTime"].Width = 115;
            grid.Columns["TafelDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["TafelDateTime"].Width = 115;
            grid.Columns["Profil90DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Profil90DateTime"].Width = 115;
            grid.Columns["Profil45DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Profil45DateTime"].Width = 115;
            grid.Columns["DominoDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DominoDateTime"].Width = 115;
            grid.Columns["PrintDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["PrintDateTime"].Width = 115;
            grid.Columns["RALDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["RALDateTime"].Width = 115;

            //grid.Columns["CreationDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["CreationDateTime"].MinimumWidth = 55;
            //grid.Columns["TPS45DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["TPS45DateTime"].MinimumWidth = 55;
            //grid.Columns["GenevaDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["GenevaDateTime"].MinimumWidth = 55;
            //grid.Columns["TafelDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["TafelDateTime"].MinimumWidth = 55;
            //grid.Columns["Profil90DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Profil90DateTime"].MinimumWidth = 55;
            //grid.Columns["Profil45DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["Profil45DateTime"].MinimumWidth = 55;
            //grid.Columns["DominoDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["DominoDateTime"].MinimumWidth = 55;
            //grid.Columns["PrintDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //grid.Columns["PrintDateTime"].MinimumWidth = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Notes"].Width = 155;

            grid.Columns["MachineColumn"].ReadOnly = false;
            grid.Columns["Notes"].ReadOnly = false;
            grid.Columns["TrimmingDateTimeColumn"].ReadOnly = false;
            grid.Columns["FilenkaDateTimeColumn"].ReadOnly = false;
            grid.Columns["AssemblyDateTimeColumn"].ReadOnly = false;
            grid.Columns["DeyingDateTimeColumn"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["WorkAssignmentID"].DisplayIndex = DisplayIndex++;
            grid.Columns["Name"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreationDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["TPS45DateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["TPS45UserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["GenevaDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["GenevaUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TafelDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["TafelUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["RALDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["RALUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Profil90DateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["Profil90UserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Profil45DateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["Profil45UserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["DominoDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["DominoUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PrintDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["MachineColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["FilenkaDateTimeColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TrimmingDateTimeColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["AssemblyDateTimeColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["DeyingDateTimeColumn"].DisplayIndex = DisplayIndex++;
        }

        private void BatchesGridSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;
            if (grid.Columns.Contains("CreateDateTime"))
                grid.Columns["CreateDateTime"].Visible = false;
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;

            if (grid.Columns.Contains("ProfilConfirmDateTime"))
                grid.Columns["ProfilConfirmDateTime"].Visible = false;
            if (grid.Columns.Contains("TPSConfirmDateTime"))
                grid.Columns["TPSConfirmDateTime"].Visible = false;
            if (grid.Columns.Contains("ProfilCloseDateTime"))
                grid.Columns["ProfilCloseDateTime"].Visible = false;
            if (grid.Columns.Contains("TPSCloseDateTime"))
                grid.Columns["TPSCloseDateTime"].Visible = false;
            grid.Columns["ProfilEnabled"].Visible = false;
            grid.Columns["TPSEnabled"].Visible = false;
            grid.Columns["MegaBatchID"].Visible = false;
            if (grid.Columns.Contains("ProfilConfirm"))
                grid.Columns["ProfilConfirm"].Visible = false;
            if (grid.Columns.Contains("TPSConfirm"))
                grid.Columns["TPSConfirm"].Visible = false;
            if (grid.Columns.Contains("ProfilCloseUserID"))
                grid.Columns["ProfilCloseUserID"].Visible = false;
            if (grid.Columns.Contains("TPSCloseUserID"))
                grid.Columns["TPSCloseUserID"].Visible = false;
            if (grid.Columns.Contains("ProfilConfirmUserID"))
                grid.Columns["ProfilConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("TPSConfirmUserID"))
                grid.Columns["TPSConfirmUserID"].Visible = false;
            if (grid.Columns.Contains("ProfilWorkAssignmentID"))
                grid.Columns["ProfilWorkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("TPSWorkAssignmentID"))
                grid.Columns["TPSWorkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["BatchID"].HeaderText = "№";
            grid.Columns["ProfilName"].HeaderText = "Название";
            grid.Columns["TPSName"].HeaderText = "Название";

            grid.Columns["BatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["BatchID"].Width = 55;
            grid.Columns["ProfilName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["ProfilName"].MinimumWidth = 55;
            grid.Columns["TPSName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TPSName"].MinimumWidth = 55;
        }

        private void MegaBatchesGridSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            grid.Columns["TPSEntryDateTime"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;
            if (grid.Columns.Contains("ProfilCloseUserID"))
                grid.Columns["ProfilCloseUserID"].Visible = false;
            if (grid.Columns.Contains("TPSCloseUserID"))
                grid.Columns["TPSCloseUserID"].Visible = false;
            if (grid.Columns.Contains("CreateDateTime"))
                grid.Columns["CreateDateTime"].Visible = false;
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;

            if (grid.Columns.Contains("Group"))
            {
                grid.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["Group"].Width = 45;
                grid.Columns["Group"].DisplayIndex = 0;
                grid.Columns["Group"].HeaderText = string.Empty;
            }

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TPSEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ProfilEntryDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            grid.Columns["MegaBatchID"].HeaderText = "№";
            grid.Columns["CreateDateTime"].HeaderText = "Дата создания";
            grid.Columns["ProfilEntryDateTime"].HeaderText = "Вход на пр-во";
            grid.Columns["TPSEntryDateTime"].HeaderText = "Вход на пр-во";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["MegaBatchID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaBatchID"].Width = 55;
            grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CreateDateTime"].MinimumWidth = 55;
            grid.Columns["ProfilEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ProfilEntryDateTime"].MinimumWidth = 55;
            grid.Columns["TPSEntryDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TPSEntryDateTime"].MinimumWidth = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["Notes"].MinimumWidth = 155;
        }

        private void FrontsGridSetting(ref PercentageDataGrid grid)
        {
            if (!grid.Columns.Contains("FrontsColumn"))
                grid.Columns.Add(WFrontsOrdersManager.FrontsColumn);
            if (!grid.Columns.Contains("FrameColorsColumn"))
                grid.Columns.Add(WFrontsOrdersManager.FrameColorsColumn);
            if (!grid.Columns.Contains("PatinaColumn"))
                grid.Columns.Add(WFrontsOrdersManager.PatinaColumn);
            if (!grid.Columns.Contains("InsetTypesColumn"))
                grid.Columns.Add(WFrontsOrdersManager.InsetTypesColumn);
            if (!grid.Columns.Contains("InsetColorsColumn"))
                grid.Columns.Add(WFrontsOrdersManager.InsetColorsColumn);
            if (!grid.Columns.Contains("TechnoProfilesColumn"))
                grid.Columns.Add(WFrontsOrdersManager.TechnoProfilesColumn);
            if (!grid.Columns.Contains("TechnoFrameColorsColumn"))
                grid.Columns.Add(WFrontsOrdersManager.TechnoFrameColorsColumn);
            if (!grid.Columns.Contains("TechnoInsetTypesColumn"))
                grid.Columns.Add(WFrontsOrdersManager.TechnoInsetTypesColumn);
            if (!grid.Columns.Contains("TechnoInsetColorsColumn"))
                grid.Columns.Add(WFrontsOrdersManager.TechnoInsetColorsColumn);
            if (grid.Columns.Contains("ImpostMargin"))
                grid.Columns["ImpostMargin"].Visible = false;
            if (grid.Columns.Contains("CreateDateTime"))
            {
                grid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                grid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["CreateDateTime"].Width = 100;
            }
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            grid.Columns["FrontsOrdersID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["FrontID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["InsetColorID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["InsetTypeID"].Visible = false;
            grid.Columns["TechnoProfileID"].Visible = false;
            grid.Columns["TechnoColorID"].Visible = false;
            grid.Columns["TechnoInsetTypeID"].Visible = false;
            grid.Columns["TechnoInsetColorID"].Visible = false;
            if (grid.Columns.Contains("TotalDiscount"))
                grid.Columns["TotalDiscount"].Visible = false;
            if (grid.Columns.Contains("OriginalCost"))
                grid.Columns["OriginalCost"].Visible = false;
            if (grid.Columns.Contains("CurrencyCost"))
                grid.Columns["CurrencyCost"].Visible = false;
            if (grid.Columns.Contains("CurrencyTypeID"))
                grid.Columns["CurrencyTypeID"].Visible = false;

            grid.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["Height"].HeaderText = "Высота";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Square"].HeaderText = "Площадь";
            grid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            if (grid.Columns.Contains("ClientName"))
                grid.Columns["ClientName"].HeaderText = "Клиент";

            grid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].Width = 85;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].Width = 85;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count"].Width = 85;
            grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Square"].Width = 85;
            grid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["IsNonStandard"].MinimumWidth = 55;
            if (grid.Columns.Contains("ClientName"))
            {
                grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid.Columns["ClientName"].MinimumWidth = 185;
            }
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 85;

            grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["IsNonStandard"].DisplayIndex = DisplayIndex++;
            if (grid.Columns.Contains("ClientName"))
            {
                grid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            }
        }

        private void DecorGridSetting(ref PercentageDataGrid grid, DecorOrdersManager tDecorOrdersManager)
        {
            grid.Columns.Add(tDecorOrdersManager.ProductColumn);
            grid.Columns.Add(tDecorOrdersManager.ItemColumn);
            grid.Columns.Add(tDecorOrdersManager.ColorColumn);
            grid.Columns.Add(tDecorOrdersManager.PatinaColumn);
            if (grid.Columns.Contains("CreateDateTime"))
            {
                grid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                grid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["CreateDateTime"].Width = 100;
            }
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            grid.Columns["ProductID"].Visible = false;
            grid.Columns["DecorID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["DecorConfigID"].Visible = false;
            grid.Columns["DecorOrderID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["FactoryID"].Visible = false;
            if (grid.Columns.Contains("CurrencyCost"))
                grid.Columns["CurrencyCost"].Visible = false;
            if (grid.Columns.Contains("CurrencyTypeID"))
                grid.Columns["CurrencyTypeID"].Visible = false;
            if (grid.Columns.Contains("OriginalCost"))
                grid.Columns["OriginalCost"].Visible = false;
            if (grid.Columns.Contains("CurrencyCost"))
                grid.Columns["CurrencyCost"].Visible = false;
            if (grid.Columns.Contains("TotalDiscount"))
                grid.Columns["TotalDiscount"].Visible = false;
            if (grid.Columns.Contains("DiscountVolume"))
                grid.Columns["DiscountVolume"].Visible = false;
            if (grid.Columns.Contains("OriginalPrice"))
                grid.Columns["OriginalPrice"].Visible = false;
            if (grid.Columns.Contains("MeasureID"))
                grid.Columns["MeasureID"].Visible = false;
            if (grid.Columns.Contains("PriceWithTransport"))
                grid.Columns["PriceWithTransport"].Visible = false;
            if (grid.Columns.Contains("CostWithTransport"))
                grid.Columns["CostWithTransport"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
            }

            grid.Columns["Length"].HeaderText = "Длина";
            grid.Columns["Height"].HeaderText = "Высота";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["ProductColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length"].MinimumWidth = 85;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].Width = 85;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].Width = 85;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count"].Width = 85;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 185;

            grid.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            grid.Columns["ProductColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ItemColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        private void FrontsSummaryGridSetting(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
                Column.ReadOnly = true;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            if (grid.Columns.Contains("FrontID"))
                grid.Columns["FrontID"].Visible = false;
            if (grid.Columns.Contains("FrontTypeID"))
                grid.Columns["FrontTypeID"].Visible = false;
            if (grid.Columns.Contains("ColorID"))
                grid.Columns["ColorID"].Visible = false;
            if (grid.Columns.Contains("PatinaID"))
                grid.Columns["PatinaID"].Visible = false;
            if (grid.Columns.Contains("InsetTypeID"))
                grid.Columns["InsetTypeID"].Visible = false;
            if (grid.Columns.Contains("InsetColorID"))
                grid.Columns["InsetColorID"].Visible = false;
            if (grid.Columns.Contains("Height"))
                grid.Columns["Height"].Visible = false;
            if (grid.Columns.Contains("Width"))
                grid.Columns["Width"].Visible = false;
            if (grid.Columns.Contains("PrintingStatus"))
                grid.Columns["PrintingStatus"].Visible = false;

            if (grid.Columns.Contains("Front"))
                grid.Columns["Front"].HeaderText = "Фасад";
            if (grid.Columns.Contains("Square"))
                grid.Columns["Square"].HeaderText = "м.кв.";
            if (grid.Columns.Contains("Count"))
                grid.Columns["Count"].HeaderText = "шт.";
            if (grid.Columns.Contains("FrameColor"))
                grid.Columns["FrameColor"].HeaderText = "Цвет профиля";
            if (grid.Columns.Contains("Size"))
                grid.Columns["Size"].HeaderText = "Размеры";
            if (grid.Columns.Contains("InsetType"))
                grid.Columns["InsetType"].HeaderText = "Вставка";
            if (grid.Columns.Contains("InsetColor"))
                grid.Columns["InsetColor"].HeaderText = "Цвет наполнителя";

            if (grid.Columns.Contains("InsetColor"))
            {
                grid.Columns["Square"].DefaultCellStyle.Format = "N";
                grid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;
            }
            if (grid.Columns.Contains("Square"))
                grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            if (grid.Columns.Contains("Count"))
                grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            if (grid.Columns.Contains("Square"))
                grid.Columns["Square"].Width = 90;
            if (grid.Columns.Contains("Count"))
                grid.Columns["Count"].Width = 60;
            if (grid.Columns.Contains("Front"))
                grid.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (grid.Columns.Contains("FrameColor"))
                grid.Columns["FrameColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (grid.Columns.Contains("InsetType"))
                grid.Columns["InsetType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (grid.Columns.Contains("InsetColor"))
                grid.Columns["InsetColor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (grid.Columns.Contains("Size"))
                grid.Columns["Size"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //if (grid.Columns.Contains("Front"))
            //    grid.Columns["Front"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (grid.Columns.Contains("Size"))
                grid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //if (grid.Columns.Contains("InsetColor"))
            //    grid.Columns["InsetColor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //if (grid.Columns.Contains("InsetType"))
            //    grid.Columns["InsetType"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (grid.Columns.Contains("Count"))
                grid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //if (grid.Columns.Contains("FrameColor"))
            //    grid.Columns["FrameColor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void DecorSummaryGridSetting(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
                Column.ReadOnly = true;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            if (grid.Columns.Contains("ProductID"))
                grid.Columns["ProductID"].Visible = false;
            if (grid.Columns.Contains("DecorID"))
                grid.Columns["DecorID"].Visible = false;
            if (grid.Columns.Contains("ColorID"))
                grid.Columns["ColorID"].Visible = false;
            if (grid.Columns.Contains("MeasureID"))
                grid.Columns["MeasureID"].Visible = false;
            if (grid.Columns.Contains("Height"))
                grid.Columns["Height"].Visible = false;
            if (grid.Columns.Contains("Length"))
                grid.Columns["Length"].Visible = false;
            if (grid.Columns.Contains("Width"))
                grid.Columns["Width"].Visible = false;

            if (grid.Columns.Contains("DecorProduct"))
            {
                grid.Columns["DecorProduct"].HeaderText = "Продукт";
                grid.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns["DecorProduct"].MinimumWidth = 100;
            }
            if (grid.Columns.Contains("DecorItem"))
            {
                grid.Columns["DecorItem"].HeaderText = "Наименование";
                grid.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns["DecorItem"].MinimumWidth = 100;
            }
            if (grid.Columns.Contains("Color"))
            {
                grid.Columns["Color"].HeaderText = "Цвет";
                grid.Columns["Color"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns["Color"].MinimumWidth = 100;
            }
            if (grid.Columns.Contains("Count"))
            {
                grid.Columns["Count"].HeaderText = "Кол-во";
                grid.Columns["Count"].DefaultCellStyle.Format = "N";
                grid.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;
                grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["Count"].Width = 90;
                grid.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (grid.Columns.Contains("Measure"))
            {
                grid.Columns["Measure"].HeaderText = "Ед.изм.";
                grid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["Measure"].Width = 80;
            }
            if (grid.Columns.Contains("Size"))
            {
                grid.Columns["Size"].HeaderText = "Размер";
                grid.Columns["Size"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns["Size"].MinimumWidth = 100;
            }
        }
        #endregion

        private void dgvMarketMegaBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;
            int MegaBatchID = 0;
            if (dgvMarketMegaBatches.SelectedRows.Count != 0 && dgvMarketMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMarketMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                CreationAssignmentsManager.FilterBatchesByMegaBatch(false, MegaBatchID, FactoryID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                CreationAssignmentsManager.FilterBatchesByMegaBatch(false, MegaBatchID, FactoryID);
        }

        private void dgvZOVMegaBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;
            int MegaBatchID = 0;
            if (dgvZOVMegaBatches.SelectedRows.Count != 0 && dgvZOVMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvZOVMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                CreationAssignmentsManager.FilterBatchesByMegaBatch(true, MegaBatchID, FactoryID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                CreationAssignmentsManager.FilterBatchesByMegaBatch(true, MegaBatchID, FactoryID);
        }

        private void FilterMegaBatch()
        {
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                CreationAssignmentsManager.FilterMegaBatchesByFactory(false, FactoryID);
                CreationAssignmentsManager.FilterMegaBatchesByFactory(true, FactoryID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                CreationAssignmentsManager.FilterMegaBatchesByFactory(false, FactoryID);
                CreationAssignmentsManager.FilterMegaBatchesByFactory(true, FactoryID);
            }
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;

            if (ZOVProfilCheckButton.Checked && !ZOVTPSCheckButton.Checked)
            {
                FactoryID = 1;
            }
            if (!ZOVProfilCheckButton.Checked && ZOVTPSCheckButton.Checked)
            {
                FactoryID = 2;
            }
            kryptonCheckSet3_CheckedButtonChanged(null, null);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            //CreationAssignmentsManager.GetMegaBatchFronts(false, FactoryID);
            //CreationAssignmentsManager.GetBatchFronts(false, FactoryID);
            //CreationAssignmentsManager.GetMegaBatchFronts(true, FactoryID);
            //CreationAssignmentsManager.GetBatchFronts(true, FactoryID);

            CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);
            CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);

            FilterMegaBatch();
            ShowColumns(FactoryID, ref dgvMarketMegaBatches, ref dgvMarketBatches);
            ShowColumns(FactoryID, ref dgvZOVMegaBatches, ref dgvZOVBatches);

            DateTime date1 = DateTimePicker1.Value.Date;
            DateTime date2 = DateTimePicker2.Value.Date;

            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvMarketBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (MarketFrontsOrdersManager == null || MarketDecorOrdersManager == null)
                return;
            int BatchID = 0;
            int FilterType = 0;
            if (dgvMarketBatches.SelectedRows.Count != 0 && dgvMarketBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvMarketBatches.SelectedRows[0].Cells["BatchID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                MarketFrontsOrdersManager.FilterFrontsByBatch(false, BatchID, FactoryID, FilterType);
                MarketDecorOrdersManager.FilterDecorByBatch(false, BatchID, FactoryID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                MarketFrontsOrdersManager.FilterFrontsByBatch(false, BatchID, FactoryID, FilterType);
                MarketDecorOrdersManager.FilterDecorByBatch(false, BatchID, FactoryID);
            }
        }

        private void dgvZOVBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (ZOVFrontsOrdersManager == null || ZOVDecorOrdersManager == null)
                return;
            int BatchID = 0;
            int FilterType = 0;
            if (dgvZOVBatches.SelectedRows.Count != 0 && dgvZOVBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvZOVBatches.SelectedRows[0].Cells["BatchID"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ZOVFrontsOrdersManager.FilterFrontsByBatch(true, BatchID, FactoryID, FilterType);
                ZOVDecorOrdersManager.FilterDecorByBatch(true, BatchID, FactoryID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                ZOVFrontsOrdersManager.FilterFrontsByBatch(true, BatchID, FactoryID, FilterType);
                ZOVDecorOrdersManager.FilterDecorByBatch(true, BatchID, FactoryID);
            }
        }

        private void AddCheckColumn(ref PercentageDataGrid grid)
        {
            DataGridViewCheckBoxColumn CheckColumn = new DataGridViewCheckBoxColumn()
            {
                Name = "CheckColumn",
                HeaderText = string.Empty,
                DataPropertyName = "Check",
                DisplayIndex = 0,
                SortMode = DataGridViewColumnSortMode.Automatic,
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40
            };
            grid.Columns.Add(CheckColumn);
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;

            if (cbtnMarketing.Checked)
            {
                pnlMarketBatch.BringToFront();
                if (cbtnFronts.Checked)
                    pnlMarketFronts.BringToFront();
                if (cbtnDecor.Checked)
                    pnlMarketDecor.BringToFront();
            }
            else
            {
                pnlZOVBatch.BringToFront();
                if (cbtnFronts.Checked)
                    pnlZOVFronts.BringToFront();
                if (cbtnDecor.Checked)
                    pnlZOVDecor.BringToFront();
            }
        }

        private void btnTPSAngle45_Click(object sender, EventArgs e)
        {
            kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
        }

        private void btnGeneva_Click(object sender, EventArgs e)
        {
            kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void kryptonCheckSet3_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;

            if (cbtnAllAssignments.Checked)
            {
                panel5.BringToFront();
                if (FactoryID == 1)
                {
                    flowLayoutPanel9.Visible = true;
                    btnPrintProfilWorkAssigment.Visible = true;
                    flowLayoutPanel8.Visible = false;
                    btnPrintTPSWorkAssigment.Visible = false;
                }
                if (FactoryID == 2)
                {
                    flowLayoutPanel9.Visible = false;
                    btnPrintProfilWorkAssigment.Visible = false;
                    flowLayoutPanel8.Visible = true;
                    btnPrintTPSWorkAssigment.Visible = true;
                }
                flowLayoutPanel1.Visible = false;
                flowLayoutPanel3.Visible = false;
            }
            if (cbtnNewAssignment.Checked)
            {
                panel2.BringToFront();
                flowLayoutPanel9.Visible = false;
                flowLayoutPanel8.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel3.Visible = true;
                btnPrintProfilWorkAssigment.Visible = false;
                btnPrintTPSWorkAssigment.Visible = false;
            }
            if (cbtnAssignmentsSummary.Checked)
            {
                panel20.BringToFront();
                if (FactoryID == 1)
                {
                    flowLayoutPanel9.Visible = true;
                    btnPrintProfilWorkAssigment.Visible = false;
                    flowLayoutPanel8.Visible = false;
                    btnPrintTPSWorkAssigment.Visible = false;
                }
                if (FactoryID == 2)
                {
                    flowLayoutPanel9.Visible = false;
                    btnPrintProfilWorkAssigment.Visible = false;
                    flowLayoutPanel8.Visible = true;
                    btnPrintTPSWorkAssigment.Visible = false;
                }
                flowLayoutPanel1.Visible = false;
                flowLayoutPanel3.Visible = false;
            }
        }

        private void dgvWBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (WFrontsOrdersManager == null || WDecorOrdersManager == null)
                return;
            bool ZOV = true;
            int BatchID = 0;
            int GroupType = 0;
            int FactoryID = 0;
            int FilterType = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWBatches.SelectedRows.Count != 0 && dgvWBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvWBatches.SelectedRows[0].Cells["BatchID"].Value);
            if (dgvWBatches.SelectedRows.Count != 0 && dgvWBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvWBatches.SelectedRows[0].Cells["GroupType"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);
            if (GroupType == 1)
                ZOV = false;
            //flowLayoutPanel2.Visible = false;
            if (FactoryID == 1)
            {
                if (cbtnAllProfil.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 0;
                }
                if (cbtnAngle90Profil.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 4;
                }
                if (cbtnAngle45Profil.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 5;
                }
                if (cbtnDecorProfil.Checked)
                {
                    pnlWDecor.BringToFront();
                    pnlDecorSummary.BringToFront();
                }
                if (cbtnGeneva.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 1;
                }
                if (cbtnTafel.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 3;
                }
                if (cbtnRAL.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 6;
                }
            }
            if (FactoryID == 2)
            {
                if (cbtnAllTPS.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 0;
                }
                if (cbtnAngle45TPS.Checked)
                {
                    pnlWFronts.BringToFront();
                    pnlFrontsSummary.BringToFront();
                    FilterType = 2;
                }
                if (cbtnDecorTPS.Checked)
                {
                    pnlWDecor.BringToFront();
                    pnlDecorSummary.BringToFront();
                }
            }
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                WFrontsOrdersManager.GetPrintedFronts(WorkAssignmentID);
                WFrontsOrdersManager.FilterFrontsByWorkAssignment(WorkAssignmentID, FactoryID, FilterType);
                WDecorOrdersManager.FilterDecorByWorkAssignment(WorkAssignmentID, FactoryID);
                WFrontsOrdersManager.FilterFrontsByBatch(ZOV, BatchID, FactoryID, FilterType);
                WDecorOrdersManager.FilterDecorByBatch(ZOV, BatchID, FactoryID);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                WFrontsOrdersManager.GetPrintedFronts(WorkAssignmentID);
                WFrontsOrdersManager.FilterFrontsByWorkAssignment(WorkAssignmentID, FactoryID, FilterType);
                WDecorOrdersManager.FilterDecorByWorkAssignment(WorkAssignmentID, FactoryID);
                WFrontsOrdersManager.FilterFrontsByBatch(ZOV, BatchID, FactoryID, FilterType);
                WDecorOrdersManager.FilterDecorByBatch(ZOV, BatchID, FactoryID);
            }
        }

        private void dgvWMegaBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;
            int GroupType = 0;
            int MegaBatchID = 0;
            if (dgvWMegaBatches.SelectedRows.Count != 0 && dgvWMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvWMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvWMegaBatches.SelectedRows.Count != 0 && dgvWMegaBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvWMegaBatches.SelectedRows[0].Cells["GroupType"].Value);

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ControlAssignmentsManager.FilterBatchesByMegaBatch(GroupType, MegaBatchID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                ControlAssignmentsManager.FilterBatchesByMegaBatch(GroupType, MegaBatchID);
        }

        private void dgvWorkAssignments_SelectionChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;
            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);
            //if (WorkAssignmentID == 0 || CurrentWorkAssignmentID == WorkAssignmentID)
            //    return;
            //CurrentWorkAssignmentID = WorkAssignmentID;
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;
                ControlAssignmentsManager.RefreshMegaBatches(WorkAssignmentID, FactoryID);
                ControlAssignmentsManager.RefreshBatches(WorkAssignmentID, FactoryID);
                //WFrontsOrdersManager.FilterFrontsByWorkAssignment(WorkAssignmentID, FactoryID, FilterType);
                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                ControlAssignmentsManager.RefreshMegaBatches(WorkAssignmentID, FactoryID);
                ControlAssignmentsManager.RefreshBatches(WorkAssignmentID, FactoryID);
                //WFrontsOrdersManager.FilterFrontsByWorkAssignment(WorkAssignmentID, FactoryID, FilterType);
            }
            ShowColumns(FactoryID, ref dgvWMegaBatches, ref dgvWBatches);
        }

        private void btnCreateWorkAssignments_Click(object sender, EventArgs e)
        {
            string BatchName = string.Empty;

            ArrayList MRows = new ArrayList();
            ArrayList ZRows = new ArrayList();

            for (int i = 0; i < dgvMarketBatches.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvMarketBatches.Rows[i].Cells["CheckColumn"].Value))
                {
                    MRows.Add(Convert.ToInt32(dgvMarketBatches.Rows[i].Cells["BatchID"].Value));
                    BatchName += "М(" + dgvMarketBatches.Rows[i].Cells["MegaBatchID"].Value.ToString() + "," + dgvMarketBatches.Rows[i].Cells["BatchID"].Value + ")+";
                }
            }

            if (dgvZOVBatches.Columns.Contains("CheckColumn"))
            {
                for (int i = 0; i < dgvZOVBatches.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dgvZOVBatches.Rows[i].Cells["CheckColumn"].Value))
                    {
                        ZRows.Add(Convert.ToInt32(dgvZOVBatches.Rows[i].Cells["BatchID"].Value));
                        BatchName += "З(" + dgvZOVBatches.Rows[i].Cells["MegaBatchID"].Value.ToString() + "," + dgvZOVBatches.Rows[i].Cells["BatchID"].Value + ")+";
                    }
                }
            }

            if (MRows.Count == 0 && ZRows.Count == 0)
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;

                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбрана ни одна партия", "Внимание");
                return;
            }

            if (BatchName.Length > 0)
                BatchName = BatchName.Substring(0, BatchName.Length - 1);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            ControlAssignmentsManager.CreateWorkAssignment(BatchName, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            DateTime date1 = DateTimePicker1.Value.Date;
            DateTime date2 = DateTimePicker2.Value.Date;

            int WorkAssignmentID = ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            if (MRows.Count > 0)
                ControlAssignmentsManager.SaveBatches(false, MRows.OfType<int>().ToArray(), WorkAssignmentID, FactoryID);
            if (ZRows.Count > 0)
                ControlAssignmentsManager.SaveBatches(true, ZRows.OfType<int>().ToArray(), WorkAssignmentID, FactoryID);


            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            ControlAssignmentsManager.MoveToWorkAssignment(WorkAssignmentID);

            if (MRows.Count > 0)
            {
                //CreationAssignmentsManager.GetMegaBatchFronts(false, FactoryID);
                //CreationAssignmentsManager.GetBatchFronts(false, FactoryID);
                CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);
                dgvMarketMegaBatches_SelectionChanged(null, null);
            }
            if (ZRows.Count > 0)
            {
                //CreationAssignmentsManager.GetMegaBatchFronts(true, FactoryID);
                //CreationAssignmentsManager.GetBatchFronts(true, FactoryID);
                CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);
                dgvZOVMegaBatches_SelectionChanged(null, null);
            }

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            Infinium.LightMessageBox.Show(ref TopForm, false,
                "Задание №" + WorkAssignmentID + " создано", "Новое задание");
        }

        private void btnSaveWorkAssignments_Click(object sender, EventArgs e)
        {
            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            NeedSplash = true;
            ControlAssignmentsManager.SetName(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.CalculateSquare(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void cmiRemoveBatch_Click(object sender, EventArgs e)
        {
            bool ZOV = false;
            int GroupType = 0;
            int BatchID = 0;
            int MegaBatchID = 0;
            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWBatches.SelectedRows.Count != 0 && dgvWBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvWBatches.SelectedRows[0].Cells["BatchID"].Value);
            if (dgvWBatches.SelectedRows.Count != 0 && dgvWBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvWBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvWBatches.SelectedRows.Count != 0 && dgvWBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvWBatches.SelectedRows[0].Cells["GroupType"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);
            if (GroupType == 0)
                ZOV = true;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            ControlAssignmentsManager.RemoveBatchFromAssignment(ZOV, BatchID, WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SetName(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            //CreationAssignmentsManager.GetMegaBatchFronts(false, FactoryID);
            //CreationAssignmentsManager.GetBatchFronts(false, FactoryID);
            CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);

            //CreationAssignmentsManager.GetMegaBatchFronts(true, FactoryID);
            //CreationAssignmentsManager.GetBatchFronts(true, FactoryID);
            CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);

            dgvWorkAssignments_SelectionChanged(null, null);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            if (GroupType == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Партия З(" + MegaBatchID + "," + BatchID + ") исключена из задания №" + WorkAssignmentID, "Редактирование задания");
            }
            else
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Партия М(" + MegaBatchID + "," + BatchID + ") исключена из задания №" + WorkAssignmentID, "Редактирование задания");
            }
        }

        private void dgvWBatches_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                dgvWBatches.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiRemoveMegaBatch_Click(object sender, EventArgs e)
        {
            //bool ZOV = false;
            //int GroupType = 0;
            //int BatchID = 0;
            //int MegaBatchID = 0;
            //int FactoryID = 0;
            //int WorkAssignmentID = 0;
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
            //    WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            //if (dgvWMegaBatches.SelectedRows.Count != 0 && dgvWMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
            //    MegaBatchID = Convert.ToInt32(dgvWMegaBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            //if (dgvWBatches.SelectedRows.Count != 0 && dgvWBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
            //    BatchID = Convert.ToInt32(dgvWBatches.SelectedRows[0].Cells["BatchID"].Value);
            //if (dgvWMegaBatches.SelectedRows.Count != 0 && dgvWMegaBatches.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
            //    GroupType = Convert.ToInt32(dgvWMegaBatches.SelectedRows[0].Cells["GroupType"].Value);
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
            //    FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);
            //if (GroupType == 0)
            //    ZOV = true;

            //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            //T.Start();
            //while (!SplashWindow.bSmallCreated) ;
            //NeedSplash = false;
            //ControlAssignmentsManager.RemoveMegaBatchFromAssignment(ZOV, MegaBatchID, WorkAssignmentID, FactoryID);
            //ControlAssignmentsManager.SetName(WorkAssignmentID, FactoryID);
            //ControlAssignmentsManager.SaveWorkAssignments();

            //if (GroupType == 1)
            //{
            //    CreationAssignmentsManager.GetMegaBatchFronts(false, FactoryID);
            //    CreationAssignmentsManager.GetBatchFronts(false, FactoryID);
            //    CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);
            //}
            //if (GroupType == 0)
            //{
            //    CreationAssignmentsManager.GetMegaBatchFronts(true, FactoryID);
            //    CreationAssignmentsManager.GetBatchFronts(true, FactoryID);
            //    CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);
            //}

            //dgvWorkAssignments_SelectionChanged(null, null);
            //NeedSplash = true;
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
            //if (GroupType == 0)
            //{
            //    InfiniumTips.ShowTip(this, 50, 85,
            //        "Группа партий З(" + MegaBatchID + ") исключена из задания №" + WorkAssignmentID, 1700);
            //}
            //else
            //{
            //    InfiniumTips.ShowTip(this, 50, 85,
            //        "Группа партий М(" + MegaBatchID + ") исключена из задания №" + WorkAssignmentID, 1700);
            //}
        }

        private void dgvWMegaBatches_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void cmiAddMarketBatch_Click(object sender, EventArgs e)
        {
            if (dgvMarketBatches.SelectedRows.Count == 0 || dgvMarketBatches.SelectedRows[0].Cells["BatchID"].Value == DBNull.Value)
                return;
            if (dgvWorkAssignments.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбрано задание", "Внимание");
                return;
            }
            ArrayList Rows = new ArrayList();
            Rows.Add(Convert.ToInt32(dgvMarketBatches.SelectedRows[0].Cells["BatchID"].Value));

            if (Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбрана ни одна партия", "Внимание");
                return;
            }

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            if (Rows.Count > 0)
                ControlAssignmentsManager.SaveBatches(false, Rows.OfType<int>().ToArray(), WorkAssignmentID, FactoryID);

            ControlAssignmentsManager.SetName(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            DateTime date1 = DateTimePicker1.Value.Date;
            DateTime date2 = DateTimePicker2.Value.Date;

            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            ControlAssignmentsManager.MoveToWorkAssignment(WorkAssignmentID);
            //CreationAssignmentsManager.GetMegaBatchFronts(false, FactoryID);
            //CreationAssignmentsManager.GetBatchFronts(false, FactoryID);
            CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);

            dgvMarketMegaBatches_SelectionChanged(null, null);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            int BatchID = 0;
            int MegaBatchID = 0;
            if (dgvMarketBatches.SelectedRows.Count != 0 && dgvMarketBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvMarketBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvMarketBatches.SelectedRows.Count != 0 && dgvMarketBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvMarketBatches.SelectedRows[0].Cells["BatchID"].Value);

            Infinium.LightMessageBox.Show(ref TopForm, false,
                "Партия М(" + MegaBatchID + "," + BatchID + ") добавлена в задание №" + WorkAssignmentID, "Редактирование задания");
        }

        private void dgvMarketBatches_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                dgvMarketBatches.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiAddZOVBatch_Click(object sender, EventArgs e)
        {
            if (dgvZOVBatches.SelectedRows.Count == 0 || dgvZOVBatches.SelectedRows[0].Cells["BatchID"].Value == DBNull.Value)
                return;
            if (dgvWorkAssignments.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбрано задание", "Внимание");
                return;
            }

            ArrayList Rows = new ArrayList();
            Rows.Add(Convert.ToInt32(dgvZOVBatches.SelectedRows[0].Cells["BatchID"].Value));

            if (Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбрана ни одна партия", "Внимание");
                return;
            }

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            if (Rows.Count > 0)
                ControlAssignmentsManager.SaveBatches(true, Rows.OfType<int>().ToArray(), WorkAssignmentID, FactoryID);

            ControlAssignmentsManager.SetName(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            DateTime date1 = DateTimePicker1.Value.Date;
            DateTime date2 = DateTimePicker2.Value.Date;

            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            ControlAssignmentsManager.MoveToWorkAssignment(WorkAssignmentID);

            //CreationAssignmentsManager.GetMegaBatchFronts(true, FactoryID);
            //CreationAssignmentsManager.GetBatchFronts(true, FactoryID);
            CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);

            dgvZOVMegaBatches_SelectionChanged(null, null);
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            int BatchID = 0;
            int MegaBatchID = 0;
            if (dgvZOVBatches.SelectedRows.Count != 0 && dgvZOVBatches.SelectedRows[0].Cells["MegaBatchID"].Value != DBNull.Value)
                MegaBatchID = Convert.ToInt32(dgvZOVBatches.SelectedRows[0].Cells["MegaBatchID"].Value);
            if (dgvZOVBatches.SelectedRows.Count != 0 && dgvZOVBatches.SelectedRows[0].Cells["BatchID"].Value != DBNull.Value)
                BatchID = Convert.ToInt32(dgvZOVBatches.SelectedRows[0].Cells["BatchID"].Value);

            Infinium.LightMessageBox.Show(ref TopForm, false,
                "Партия З(" + MegaBatchID + "," + BatchID + ") добавлена в задание №" + WorkAssignmentID, "Редактирование задания");
        }

        private void dgvZOVBatches_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                dgvZOVBatches.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvMarketMegaBatches_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int Status = 0;
            CreationAssignmentsManager.IsMarketMegaBatchInAssignment(Convert.ToInt32(grid.Rows[e.RowIndex].Cells["MegaBatchID"].Value), FactoryID, ref Status);

            if (Status == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (Status == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(253, 164, 61);
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (Status == 2)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void dgvMarketBatches_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            bool NeedPaintRow = CreationAssignmentsManager.IsMarketBatchInAssignment(FactoryID, Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BatchID"].Value));

            if (NeedPaintRow)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void dgvZOVMegaBatches_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int Status = 0;
            CreationAssignmentsManager.IsZOVMegaBatchInAssignment(Convert.ToInt32(grid.Rows[e.RowIndex].Cells["MegaBatchID"].Value), FactoryID, ref Status);

            if (Status == 0)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (Status == 1)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(253, 164, 61);
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            if (Status == 2)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void dgvZOVBatches_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            bool NeedPaintRow = CreationAssignmentsManager.IsZOVBatchInAssignment(FactoryID, Convert.ToInt32(grid.Rows[e.RowIndex].Cells["BatchID"].Value));

            if (NeedPaintRow)
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgvMarketMegaBatches.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else
            {
                int rowHeaderWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
                Rectangle rowBounds = new Rectangle(rowHeaderWidth, e.RowBounds.Top,
                    grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - grid.HorizontalScrollingOffset + 1, e.RowBounds.Height);

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Security.GridsBackColor;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void btnProfilAngle90_Click(object sender, EventArgs e)
        {
            kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
        }

        private int GetFolderID(string FolderPath)
        {
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT FolderID FROM Folders WHERE FolderPath = '" + FolderPath + "'",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["FolderID"]);
                    else
                        return -1;
                }
            }
        }

        public bool UploadFile(string sSourceFileName, string sDestFileName, int FolderID, ref Int64 iFileSize)
        {
            FileInfo fi;

            //get file size
            try
            {
                fi = new FileInfo(sSourceFileName);
            }
            catch
            {
                return false;
            }

            iFileSize = fi.Length;

            //load file to ftp
            FM.UploadFile(sSourceFileName, sDestFileName, Configs.FTPType);

            //add file to database
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT TOP 0 * FROM Files", ConnectionStrings.LightConnectionString))
            {
                using (System.Data.SqlClient.SqlCommandBuilder CB = new System.Data.SqlClient.SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["FileName"] = Path.GetFileName(sSourceFileName);
                        NewRow["FolderID"] = FolderID;
                        if (Path.GetExtension(sSourceFileName).Length > 0)
                            NewRow["FileExtension"] = Path.GetExtension(sSourceFileName).Substring(1, Path.GetExtension(sSourceFileName).Length - 1);
                        else
                            NewRow["FileExtension"] = "";
                        NewRow["FileSize"] = iFileSize;
                        NewRow["Author"] = Security.CurrentUserID;

                        DateTime Date = Security.GetCurrentDate();

                        NewRow["CreationDateTime"] = Date;
                        NewRow["LastModifiedDateTime"] = Date;
                        NewRow["LastModifiedUserID"] = Security.CurrentUserID;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }

            return true;
        }

        private void kryptonCheckSet4_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (CreationAssignmentsManager == null)
                return;

            if (cbtnFronts.Checked && !cbtnDecor.Checked)
            {
                pnlFronts.BringToFront();
            }
            if (!cbtnFronts.Checked && cbtnDecor.Checked)
            {
                pnlDecor.BringToFront();
            }

            if (cbtnMarketing.Checked)
            {
                pnlMarketBatch.BringToFront();
                if (cbtnFronts.Checked)
                    pnlMarketFronts.BringToFront();
                if (cbtnDecor.Checked)
                    pnlMarketDecor.BringToFront();
            }
            else
            {
                pnlZOVBatch.BringToFront();
                if (cbtnFronts.Checked)
                    pnlZOVFronts.BringToFront();
                if (cbtnDecor.Checked)
                    pnlZOVDecor.BringToFront();
            }
        }

        private void btnTafel_Click(object sender, EventArgs e)
        {
            kryptonContextMenu6.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
        }

        private void cmiRemoveWorkAssigment_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Задание будет удалено. Продолжить?",
                    "Удаление задания");

            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление задания.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            ControlAssignmentsManager.RemoveWorkAssignment(WorkAssignmentID, FactoryID);


            DateTime date1 = DateTimePicker1.Value.Date;
            DateTime date2 = DateTimePicker2.Value.Date;

            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            CreationAssignmentsManager.GetMarketBatchesInAssignment(FactoryID);
            CreationAssignmentsManager.GetZOVBatchesInAssignment(FactoryID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            Infinium.LightMessageBox.Show(ref TopForm, false,
                "Задание №" + WorkAssignmentID + " было удалено", "Удаление задания");
        }

        private void dgvWorkAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                if (FactoryID == 1)
                {
                    kryptonContextMenuItem9.Visible = true;
                    kryptonContextMenuItem10.Visible = true;
                    kryptonContextMenuItem15.Visible = true;
                    kryptonContextMenuItem11.Visible = true;
                    kryptonContextMenuItem12.Visible = true;
                    //kryptonContextMenuItem11.Visible = false;
                    //kryptonContextMenuItem12.Visible = false;
                    //kryptonContextMenuItem13.Visible = false;
                }
                if (FactoryID == 2)
                {
                    kryptonContextMenuItem9.Visible = false;
                    kryptonContextMenuItem10.Visible = false;
                    kryptonContextMenuItem15.Visible = false;
                    kryptonContextMenuItem11.Visible = true;
                    kryptonContextMenuItem12.Visible = true;
                    //kryptonContextMenuItem13.Visible = true;
                }
                dgvWorkAssignments.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu5.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonCheckSet6_CheckedButtonChanged(object sender, EventArgs e)
        {
            dgvWBatches_SelectionChanged(null, null);
        }

        private void kryptonCheckSet7_CheckedButtonChanged(object sender, EventArgs e)
        {
            dgvWBatches_SelectionChanged(null, null);
        }

        private void dgvWBatchFronts_SelectionChanged(object sender, EventArgs e)
        {
            if (WFrontsOrdersManager == null)
                return;
            decimal TotalSquare1 = 0;
            decimal TotalSquare2 = 0;
            decimal TotalSquare3 = 0;
            decimal TotalSquare4 = 0;
            decimal TotalSquare5 = 0;
            int TotalCount1 = 0;
            int TotalCount2 = 0;
            int TotalCount3 = 0;
            int TotalCount4 = 0;
            int TotalCount5 = 0;
            int TotalCurvedCount1 = 0;
            int TotalCurvedCount2 = 0;
            int TotalCurvedCount3 = 0;
            int TotalCurvedCount4 = 0;
            int TotalCurvedCount5 = 0;

            WFrontsOrdersManager.FrontsSummary(ref TotalSquare1, ref TotalCount1, ref TotalCurvedCount1);
            WFrontsOrdersManager.FrameColorsSummary(ref TotalSquare2, ref TotalCount2, ref TotalCurvedCount2);
            WFrontsOrdersManager.InsetTypesSummary(ref TotalSquare3, ref TotalCount3, ref TotalCurvedCount3);
            WFrontsOrdersManager.InsetColorsSummary(ref TotalSquare4, ref TotalCount4, ref TotalCurvedCount4);
            WFrontsOrdersManager.SizesSummary(ref TotalSquare5, ref TotalCount5, ref TotalCurvedCount5);

            WFrontsOrdersManager.SetFrontPrintingStatus();
            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            decimal TotalPogon = 0;
            int TotalCount = 0;

            WDecorOrdersManager.GetDecorProducts(ref TotalPogon, ref TotalCount);
            WDecorOrdersManager.GetDecorItems();
            WDecorOrdersManager.GetDecorColors();
            WDecorOrdersManager.GetDecorSizes();
        }

        public void GetFrontsInfo(ref decimal Square, ref int Count, ref int CurvedCount)
        {
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontTypeID"].Value) == 1)
                    CurvedCount += Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["Count"].Value);
                else
                {
                    Square += Convert.ToDecimal(dgvFrontsSummary.SelectedRows[i].Cells["Square"].Value);
                    Count += Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["Count"].Value);
                }
            }

            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
        }

        private void dgvFrontsSummary_SelectionChanged(object sender, EventArgs e)
        {
            if (WFrontsOrdersManager == null)
                return;
            int FrontID = 0;
            int FrontTypeID = 0;
            if (dgvFrontsSummary.SelectedRows.Count != 0 && dgvFrontsSummary.SelectedRows[0].Cells["FrontID"].Value != DBNull.Value)
                FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[0].Cells["FrontID"].Value);
            if (dgvFrontsSummary.SelectedRows.Count != 0 && dgvFrontsSummary.SelectedRows[0].Cells["FrontTypeID"].Value != DBNull.Value)
                FrontTypeID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[0].Cells["FrontTypeID"].Value);
            WFrontsOrdersManager.FilterFrameColors(FrontID, FrontTypeID);

            decimal FrontSquare = 0;
            int Count = 0;
            int CurvedCount = 0;

            label3.Text = "0";
            label15.Text = "0";
            label16.Text = "0";
            GetFrontsInfo(ref FrontSquare, ref Count, ref CurvedCount);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            label3.Text = FrontSquare.ToString("N", nfi1);
            label15.Text = Count.ToString();
            label16.Text = CurvedCount.ToString();

            //if (WFrontsOrdersManager == null)
            //    return;
            //decimal TotalSquare1 = 0;
            //decimal TotalSquare2 = 0;
            //decimal TotalSquare3 = 0;
            //decimal TotalSquare4 = 0;
            //decimal TotalSquare5 = 0;
            //int TotalCount1 = 0;
            //int TotalCount2 = 0;
            //int TotalCount3 = 0;
            //int TotalCount4 = 0;
            //int TotalCount5 = 0;
            //int TotalCurvedCount1 = 0;
            //int TotalCurvedCount2 = 0;
            //int TotalCurvedCount3 = 0;
            //int TotalCurvedCount4 = 0;
            //int TotalCurvedCount5 = 0;

            //label3.Text = "0";
            //label15.Text = "0";
            //label16.Text = "0";

            //WFrontsOrdersManager.FrontsSummary(ref TotalSquare1, ref TotalCount1, ref TotalCurvedCount1);
            //WFrontsOrdersManager.FrameColorsSummary(ref TotalSquare2, ref TotalCount2, ref TotalCurvedCount2);
            //WFrontsOrdersManager.InsetTypesSummary(ref TotalSquare3, ref TotalCount3, ref TotalCurvedCount3);
            //WFrontsOrdersManager.InsetColorsSummary(ref TotalSquare4, ref TotalCount4, ref TotalCurvedCount4);
            //WFrontsOrdersManager.SizesSummary(ref TotalSquare5, ref TotalCount5, ref TotalCurvedCount5);

            //NumberFormatInfo nfi1 = new NumberFormatInfo();

            //nfi1.NumberGroupSeparator = " ";
            //nfi1.NumberDecimalDigits = 2;
            //nfi1.NumberDecimalSeparator = ",";

            //label3.Text = TotalSquare1.ToString("N", nfi1);
            //label15.Text = TotalCount1.ToString();
            //label16.Text = TotalCurvedCount1.ToString();
        }

        private void dgvFrameColorsSummary_SelectionChanged(object sender, EventArgs e)
        {
            if (WFrontsOrdersManager == null)
                return;
            int FrontID = 0;
            int FrontTypeID = 0;
            int ColorID = 0;
            int PatinaID = 0;
            if (dgvFrameColorsSummary.SelectedRows.Count != 0 && dgvFrameColorsSummary.SelectedRows[0].Cells["FrontID"].Value != DBNull.Value)
                FrontID = Convert.ToInt32(dgvFrameColorsSummary.SelectedRows[0].Cells["FrontID"].Value);
            if (dgvFrameColorsSummary.SelectedRows.Count != 0 && dgvFrameColorsSummary.SelectedRows[0].Cells["FrontTypeID"].Value != DBNull.Value)
                FrontTypeID = Convert.ToInt32(dgvFrameColorsSummary.SelectedRows[0].Cells["FrontTypeID"].Value);
            if (dgvFrameColorsSummary.SelectedRows.Count != 0 && dgvFrameColorsSummary.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(dgvFrameColorsSummary.SelectedRows[0].Cells["ColorID"].Value);
            if (dgvFrameColorsSummary.SelectedRows.Count != 0 && dgvFrameColorsSummary.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(dgvFrameColorsSummary.SelectedRows[0].Cells["PatinaID"].Value);
            WFrontsOrdersManager.FilterInsetTypes(FrontID, FrontTypeID, ColorID, PatinaID);
        }

        private void dgvInsetTypesSummary_SelectionChanged(object sender, EventArgs e)
        {
            if (WFrontsOrdersManager == null)
                return;
            int FrontID = 0;
            int FrontTypeID = 0;
            int ColorID = 0;
            int PatinaID = 0;
            int InsetTypeID = 0;
            if (dgvInsetTypesSummary.SelectedRows.Count != 0 && dgvInsetTypesSummary.SelectedRows[0].Cells["FrontID"].Value != DBNull.Value)
                FrontID = Convert.ToInt32(dgvInsetTypesSummary.SelectedRows[0].Cells["FrontID"].Value);
            if (dgvInsetTypesSummary.SelectedRows.Count != 0 && dgvInsetTypesSummary.SelectedRows[0].Cells["FrontTypeID"].Value != DBNull.Value)
                FrontTypeID = Convert.ToInt32(dgvInsetTypesSummary.SelectedRows[0].Cells["FrontTypeID"].Value);
            if (dgvInsetTypesSummary.SelectedRows.Count != 0 && dgvInsetTypesSummary.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(dgvInsetTypesSummary.SelectedRows[0].Cells["ColorID"].Value);
            if (dgvInsetTypesSummary.SelectedRows.Count != 0 && dgvInsetTypesSummary.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(dgvInsetTypesSummary.SelectedRows[0].Cells["PatinaID"].Value);
            if (dgvInsetTypesSummary.SelectedRows.Count != 0 && dgvInsetTypesSummary.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(dgvInsetTypesSummary.SelectedRows[0].Cells["InsetTypeID"].Value);
            WFrontsOrdersManager.FilterInsetColors(FrontID, FrontTypeID, ColorID, PatinaID, InsetTypeID);
        }

        private void dgvInsetColorsSummary_SelectionChanged(object sender, EventArgs e)
        {
            if (WFrontsOrdersManager == null)
                return;
            int FrontID = 0;
            int FrontTypeID = 0;
            int ColorID = 0;
            int PatinaID = 0;
            int InsetTypeID = 0;
            int InsetColorID = 0;
            if (dgvInsetColorsSummary.SelectedRows.Count != 0 && dgvInsetColorsSummary.SelectedRows[0].Cells["FrontID"].Value != DBNull.Value)
                FrontID = Convert.ToInt32(dgvInsetColorsSummary.SelectedRows[0].Cells["FrontID"].Value);
            if (dgvInsetColorsSummary.SelectedRows.Count != 0 && dgvInsetColorsSummary.SelectedRows[0].Cells["FrontTypeID"].Value != DBNull.Value)
                FrontTypeID = Convert.ToInt32(dgvInsetColorsSummary.SelectedRows[0].Cells["FrontTypeID"].Value);
            if (dgvInsetColorsSummary.SelectedRows.Count != 0 && dgvInsetColorsSummary.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(dgvInsetColorsSummary.SelectedRows[0].Cells["ColorID"].Value);
            if (dgvInsetColorsSummary.SelectedRows.Count != 0 && dgvInsetColorsSummary.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(dgvInsetColorsSummary.SelectedRows[0].Cells["PatinaID"].Value);
            if (dgvInsetColorsSummary.SelectedRows.Count != 0 && dgvInsetColorsSummary.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(dgvInsetColorsSummary.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (dgvInsetColorsSummary.SelectedRows.Count != 0 && dgvInsetColorsSummary.SelectedRows[0].Cells["InsetColorID"].Value != DBNull.Value)
                InsetColorID = Convert.ToInt32(dgvInsetColorsSummary.SelectedRows[0].Cells["InsetColorID"].Value);
            WFrontsOrdersManager.FilterSizes(FrontID, FrontTypeID, ColorID, PatinaID, InsetTypeID, InsetColorID);
        }

        public void GetDecorInfo(ref decimal Pogon, ref int Count)
        {
            for (int i = 0; i < dgvDecorProducts.SelectedRows.Count; i++)
            {
                if (Convert.ToInt32(dgvDecorProducts.SelectedRows[i].Cells["MeasureID"].Value) != 2)
                    Count += Convert.ToInt32(dgvDecorProducts.SelectedRows[i].Cells["Count"].Value);
                else
                {
                    Pogon += Convert.ToDecimal(dgvDecorProducts.SelectedRows[i].Cells["Count"].Value);
                }
            }

            Pogon = Decimal.Round(Pogon, 2, MidpointRounding.AwayFromZero);
        }

        private void dgvDecorProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (WDecorOrdersManager == null)
                return;
            int ProductID = 0;
            int MeasureID = 0;
            if (dgvDecorProducts.SelectedRows.Count != 0 && dgvDecorProducts.SelectedRows[0].Cells["ProductID"].Value != DBNull.Value)
                ProductID = Convert.ToInt32(dgvDecorProducts.SelectedRows[0].Cells["ProductID"].Value);
            if (dgvDecorProducts.SelectedRows.Count != 0 && dgvDecorProducts.SelectedRows[0].Cells["MeasureID"].Value != DBNull.Value)
                MeasureID = Convert.ToInt32(dgvDecorProducts.SelectedRows[0].Cells["MeasureID"].Value);
            WDecorOrdersManager.FilterDecorItems(ProductID, MeasureID);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            label10.Text = "0";
            label14.Text = "0";

            decimal Pogon = 0;
            int Count = 0;

            GetDecorInfo(ref Pogon, ref Count);

            label10.Text = Pogon.ToString("N", nfi1);
            label14.Text = Count.ToString();
        }

        private void dgvDecorItems_SelectionChanged(object sender, EventArgs e)
        {
            if (WDecorOrdersManager == null)
                return;
            int ProductID = 0;
            int DecorID = 0;
            int MeasureID = 0;
            if (dgvDecorItems.SelectedRows.Count != 0 && dgvDecorItems.SelectedRows[0].Cells["ProductID"].Value != DBNull.Value)
                ProductID = Convert.ToInt32(dgvDecorItems.SelectedRows[0].Cells["ProductID"].Value);
            if (dgvDecorItems.SelectedRows.Count != 0 && dgvDecorItems.SelectedRows[0].Cells["DecorID"].Value != DBNull.Value)
                DecorID = Convert.ToInt32(dgvDecorItems.SelectedRows[0].Cells["DecorID"].Value);
            if (dgvDecorItems.SelectedRows.Count != 0 && dgvDecorItems.SelectedRows[0].Cells["MeasureID"].Value != DBNull.Value)
                MeasureID = Convert.ToInt32(dgvDecorItems.SelectedRows[0].Cells["MeasureID"].Value);
            WDecorOrdersManager.FilterDecorColors(ProductID, DecorID, MeasureID);
        }

        private void dgvDecorColors_SelectionChanged(object sender, EventArgs e)
        {
            if (WDecorOrdersManager == null)
                return;
            int ProductID = 0;
            int DecorID = 0;
            int ColorID = 0;
            int MeasureID = 0;
            if (dgvDecorColors.SelectedRows.Count != 0 && dgvDecorColors.SelectedRows[0].Cells["ProductID"].Value != DBNull.Value)
                ProductID = Convert.ToInt32(dgvDecorColors.SelectedRows[0].Cells["ProductID"].Value);
            if (dgvDecorColors.SelectedRows.Count != 0 && dgvDecorColors.SelectedRows[0].Cells["DecorID"].Value != DBNull.Value)
                DecorID = Convert.ToInt32(dgvDecorColors.SelectedRows[0].Cells["DecorID"].Value);
            if (dgvDecorColors.SelectedRows.Count != 0 && dgvDecorColors.SelectedRows[0].Cells["ColorID"].Value != DBNull.Value)
                ColorID = Convert.ToInt32(dgvDecorColors.SelectedRows[0].Cells["ColorID"].Value);
            if (dgvDecorColors.SelectedRows.Count != 0 && dgvDecorColors.SelectedRows[0].Cells["MeasureID"].Value != DBNull.Value)
                MeasureID = Convert.ToInt32(dgvDecorColors.SelectedRows[0].Cells["MeasureID"].Value);
            WDecorOrdersManager.FilterDecorSizes(ProductID, DecorID, ColorID, MeasureID);
        }

        private void dgvWBatchDecor_SelectionChanged(object sender, EventArgs e)
        {
            if (WDecorOrdersManager == null)
                return;

            decimal TotalPogon = 0;
            int TotalCount = 0;

            WDecorOrdersManager.GetDecorProducts(ref TotalPogon, ref TotalCount);
            WDecorOrdersManager.GetDecorItems();
            WDecorOrdersManager.GetDecorColors();
            WDecorOrdersManager.GetDecorSizes();
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ProfilAngle90Assignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Marsel1) || FrontID == Convert.ToInt32(Fronts.Marsel5) || FrontID == Convert.ToInt32(Fronts.Marsel3) ||
                    FrontID == Convert.ToInt32(Fronts.Marsel4) || FrontID == Convert.ToInt32(Fronts.Jersy110) ||
                    FrontID == Convert.ToInt32(Fronts.Porto) || FrontID == Convert.ToInt32(Fronts.Monte) ||
                    FrontID == Convert.ToInt32(Fronts.Techno1) || FrontID == Convert.ToInt32(Fronts.Shervud) ||
                    FrontID == Convert.ToInt32(Fronts.Techno2) || FrontID == Convert.ToInt32(Fronts.Techno4) || FrontID == Convert.ToInt32(Fronts.pFox) ||
                    FrontID == Convert.ToInt32(Fronts.pFlorenc) ||
                    FrontID == Convert.ToInt32(Fronts.Techno5) || FrontID == Convert.ToInt32(Fronts.PRU8))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            ProfilAngle90Assignments.GetFrontsID = FrontsID;
            bool HasOrders = ProfilAngle90Assignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcel(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
            }
            ProfilAngle90Assignments.ClearOrders();
            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.PR1) || FrontID == Convert.ToInt32(Fronts.PR2))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            HasOrders = ProfilAngle90Assignments.GetPR1Orders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcelPR1(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
                //dgvWBatches_SelectionChanged(null, null);
            }
            ProfilAngle90Assignments.ClearOrders();
            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.PR3))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            HasOrders = ProfilAngle90Assignments.GetPR3Orders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcelPR3(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            ProfilAngle90Assignments.ClearOrders();
            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.PRU8))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            HasOrders = ProfilAngle90Assignments.GetPRU8Orders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcelPRU8(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            Machines Machine = Machines.No_machine;
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            TPSAngle45Assignments.ClearOrders();

            int FactoryID = 0;
            int MachineID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Machine"].Value != DBNull.Value)
                MachineID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["Machine"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (MachineID == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбран станок", "Внимание");
                return;
            }

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            switch (MachineID)
            {
                case 0:
                    Machine = Machines.No_machine;
                    break;
                case 1:
                    Machine = Machines.ELME;
                    break;
                case 2:
                    Machine = Machines.Balistrini;
                    break;
                case 3:
                    Machine = Machines.Rapid;
                    break;
                default:
                    break;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Lorenzo) || FrontID == Convert.ToInt32(Fronts.Elegant) || FrontID == Convert.ToInt32(Fronts.ElegantPat) || FrontID == Convert.ToInt32(Fronts.KansasPat) || FrontID == Convert.ToInt32(Fronts.Kansas) || FrontID == Convert.ToInt32(Fronts.Infiniti) ||
                    FrontID == Convert.ToInt32(Fronts.Dakota) || FrontID == Convert.ToInt32(Fronts.DakotaPat) || FrontID == Convert.ToInt32(Fronts.Sofia) || FrontID == Convert.ToInt32(Fronts.Turin1) || FrontID == Convert.ToInt32(Fronts.Turin1_1) || FrontID == Convert.ToInt32(Fronts.Turin3) ||
                    FrontID == Convert.ToInt32(Fronts.LeonTPS) || FrontID == Convert.ToInt32(Fronts.InfinitiPat))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            TPSAngle45Assignments.GetFrontsID = FrontsID;
            bool HasOrders = TPSAngle45Assignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                TPSAngle45Assignments.CreateExcel(WorkAssignmentID, Machine, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(1, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            GenevaAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            string sSourceFileName = string.Empty;
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            bool HasOrders = GenevaAssignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                GenevaAssignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.SetPrintDateTime(2, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            //Int64 iFileSize = 0;
            //string sSourceFolder = System.Environment.GetEnvironmentVariable("TEMP");
            //string sFolderPath = "Общие файлы/Производство/Задания в работу";
            //string sDestFolder = Configs.DocumentsPath + sFolderPath;
            //try
            //{
            //    int FolderID = GetFolderID(sFolderPath);
            //    if (FolderID != -1)
            //        UploadFile(sSourceFolder + @"\" + sSourceFileName, sDestFolder + "/" + sSourceFileName, FolderID, ref iFileSize);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            //System.Threading.Thread T1 = new System.Threading.Thread(delegate()
            //{
            //    FM.DownloadFile(sDestFolder + "/" + sSourceFileName, sSourceFolder + @"\" + sSourceFileName, iFileSize, Configs.FTPType);
            //});
            //T1.Start();

            //while (T1.IsAlive)
            //{
            //    T1.Join(50);
            //    Application.DoEvents();
            //}
            //System.Diagnostics.Process.Start(sSourceFolder + @"\" + sSourceFileName);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            TafelAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            string sSourceFileName = string.Empty;
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            //bool HasOrders = TafelAssignments.GetOrders(WorkAssignmentID, FactoryID);
            //if (HasOrders)
            //{
            //    TafelAssignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
            //    ControlAssignmentsManager.SetPrintDateTime(3, WorkAssignmentID);
            //    ControlAssignmentsManager.SaveWorkAssignments();
            //    ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            //}
            //ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            //ControlAssignmentsManager.SaveWorkAssignments();

            Int64 iFileSize = 0;
            string sSourceFolder = System.Environment.GetEnvironmentVariable("TEMP");
            string sFolderPath = "Общие файлы/Производство/Задания в работу";
            string sDestFolder = Configs.DocumentsPath + sFolderPath;
            try
            {
                int FolderID = GetFolderID(sFolderPath);
                if (FolderID != -1)
                    UploadFile(sSourceFolder + @"\" + sSourceFileName, sDestFolder + "/" + sSourceFileName, FolderID, ref iFileSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            System.Threading.Thread T1 = new System.Threading.Thread(delegate ()
            {
                FM.DownloadFile(sDestFolder + "/" + sSourceFileName, sSourceFolder + @"\" + sSourceFileName, iFileSize, Configs.FTPType);
            });
            T1.Start();

            while (T1.IsAlive)
            {
                T1.Join(50);
                Application.DoEvents();
            }
            System.Diagnostics.Process.Start(sSourceFolder + @"\" + sSourceFileName);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ProfilAngle45Assignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            //ProfilAngle45Assignments.GetOrders(WorkAssignmentID);

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.TechnoN) ||
                    FrontID == Convert.ToInt32(Fronts.Antalia) || FrontID == Convert.ToInt32(Fronts.Nord95) || FrontID == Convert.ToInt32(Fronts.epFox) ||
                    FrontID == Convert.ToInt32(Fronts.Venecia) || FrontID == Convert.ToInt32(Fronts.Bergamo) ||
                    FrontID == Convert.ToInt32(Fronts.ep206) || FrontID == Convert.ToInt32(Fronts.ep216) || FrontID == Convert.ToInt32(Fronts.ep111) || FrontID == Convert.ToInt32(Fronts.ep041) || FrontID == Convert.ToInt32(Fronts.ep071) ||
                    FrontID == Convert.ToInt32(Fronts.Boston) ||
                    FrontID == Convert.ToInt32(Fronts.Leon) || FrontID == Convert.ToInt32(Fronts.Limog) ||
                    FrontID == Convert.ToInt32(Fronts.ep018Marsel1) || FrontID == Convert.ToInt32(Fronts.ep043Shervud) ||
                    FrontID == Convert.ToInt32(Fronts.ep112) ||
                    FrontID == Convert.ToInt32(Fronts.Urban) || FrontID == Convert.ToInt32(Fronts.Alby) || FrontID == Convert.ToInt32(Fronts.Bruno) ||
                    FrontID == Convert.ToInt32(Fronts.ep066Marsel4) || FrontID == Convert.ToInt32(Fronts.ep110Jersy) ||
                    FrontID == Convert.ToInt32(Fronts.epsh406Techno4) ||
                    FrontID == Convert.ToInt32(Fronts.Luk) || FrontID == Convert.ToInt32(Fronts.LukPVH) || FrontID == Convert.ToInt32(Fronts.Milano) ||
                    FrontID == Convert.ToInt32(Fronts.Praga) ||
                    FrontID == Convert.ToInt32(Fronts.Sigma) || FrontID == Convert.ToInt32(Fronts.Fat))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            ProfilAngle45Assignments.GetFrontsID = FrontsID;
            bool HasOrders = ProfilAngle45Assignments.GetOrders(WorkAssignmentID);
            if (HasOrders)
            {
                ProfilAngle45Assignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(5, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
                //dgvWBatches_SelectionChanged(null, null);
            }
            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            //ProfilAngle45Assignments.GetFrontsID = FrontsID;
            //ProfilAngle45Assignments.GetOrders(WorkAssignmentID);
            //ProfilAngle45Assignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
            //ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
            //ControlAssignmentsManager.SetPrintDateTime(5, WorkAssignmentID);
            //ControlAssignmentsManager.SaveWorkAssignments();
            //ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            //dgvWBatches_SelectionChanged(null, null);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvFrontsSummary_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                FrontsID.Clear();
                for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
                //if (FactoryID == 1)
                kryptonContextMenu8.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
                //if (FactoryID == 2)
                //    kryptonContextMenu9.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ProfilAngle45Assignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.TechnoN) ||
                    FrontID == Convert.ToInt32(Fronts.Antalia) || FrontID == Convert.ToInt32(Fronts.Nord95) || FrontID == Convert.ToInt32(Fronts.epFox) ||
                    FrontID == Convert.ToInt32(Fronts.Venecia) || FrontID == Convert.ToInt32(Fronts.Bergamo) ||
                    FrontID == Convert.ToInt32(Fronts.ep206) || FrontID == Convert.ToInt32(Fronts.ep216) || FrontID == Convert.ToInt32(Fronts.ep111) || FrontID == Convert.ToInt32(Fronts.ep041) || FrontID == Convert.ToInt32(Fronts.ep071) ||
                    FrontID == Convert.ToInt32(Fronts.Boston) ||
                    FrontID == Convert.ToInt32(Fronts.Leon) || FrontID == Convert.ToInt32(Fronts.Limog) ||
                    FrontID == Convert.ToInt32(Fronts.ep018Marsel1) || FrontID == Convert.ToInt32(Fronts.ep043Shervud) ||
                    FrontID == Convert.ToInt32(Fronts.ep112) ||
                    FrontID == Convert.ToInt32(Fronts.Urban) || FrontID == Convert.ToInt32(Fronts.Alby) || FrontID == Convert.ToInt32(Fronts.Bruno) ||
                    FrontID == Convert.ToInt32(Fronts.ep066Marsel4) || FrontID == Convert.ToInt32(Fronts.ep110Jersy) ||
                    FrontID == Convert.ToInt32(Fronts.epsh406Techno4) ||
                    FrontID == Convert.ToInt32(Fronts.Luk) || FrontID == Convert.ToInt32(Fronts.LukPVH) || FrontID == Convert.ToInt32(Fronts.Milano) ||
                    FrontID == Convert.ToInt32(Fronts.Praga) ||
                    FrontID == Convert.ToInt32(Fronts.Sigma) || FrontID == Convert.ToInt32(Fronts.Fat))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }
            ProfilAngle45Assignments.GetFrontsID = FrontsID;
            bool HasOrders = ProfilAngle45Assignments.GetOrders(WorkAssignmentID);
            if (HasOrders)
            {
                ProfilAngle45Assignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(5, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
                //dgvWBatches_SelectionChanged(null, null);
            }
            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem16_Click(object sender, EventArgs e)
        {
            //string ClientName = string.Empty;
            //string BatchName = string.Empty;

            //Techno4DominoAssignments.ClearOrders();

            //int FactoryID = 0;
            //int WorkAssignmentID = 0;
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
            //    WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
            //    BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
            //    FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            //if (BatchName.Length == 0)
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
            //    return;
            //}

            //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            //T.Start();
            //while (!SplashWindow.bSmallCreated) ;
            //NeedSplash = false;

            //bool M = BatchName.Contains('М');
            //bool Z = BatchName.Contains('З');
            //if (M && Z)
            //    ClientName = "Маркетинг + ЗОВ";
            //if (M && !Z)
            //    ClientName = "Маркетинг";
            //if (!M && Z)
            //    ClientName = "ЗОВ";
            //if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
            //    ClientName = "Москва-1";
            //string sSourceFileName = string.Empty;

            //FrontsID.Clear();
            //for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            //{
            //    int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
            //    if (FrontID == Convert.ToInt32(Fronts.Techno4Domino))
            //        FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            //}
            //Techno4DominoAssignments.GetFrontsID = FrontsID;
            //bool HasOrders = Techno4DominoAssignments.GetOrders(WorkAssignmentID, FactoryID, true);
            //if (HasOrders)
            //{
            //    Techno4DominoAssignments.CreateExcel(WorkAssignmentID, BatchName, ClientName, ref sSourceFileName);
            //    ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
            //    ControlAssignmentsManager.SetPrintDateTime(6, WorkAssignmentID);
            //    ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            //    //dgvWBatches_SelectionChanged(null, null);
            //}
            //Techno4DominoAssignments.ClearOrders();
            //HasOrders = Techno4DominoAssignments.GetOrders(WorkAssignmentID, FactoryID, false);
            //if (HasOrders)
            //{
            //    Techno4DominoAssignments.CreateExcelCurvedDomino(WorkAssignmentID, BatchName, ClientName, ref sSourceFileName);
            //    ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
            //    ControlAssignmentsManager.SetPrintDateTime(6, WorkAssignmentID);
            //    ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            //    //dgvWBatches_SelectionChanged(null, null);
            //}
            //dgvWBatches_SelectionChanged(null, null);
            //ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            //ControlAssignmentsManager.SaveWorkAssignments();
            //NeedSplash = true;
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void dgvFrontsSummary_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int PrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["PrintingStatus"].Value);
            if (grid.Columns["Front"].Index == e.ColumnIndex && e.RowIndex >= 0)
            {
                if (PrintingStatus == 2)
                {
                    e.CellStyle.BackColor = Color.Green;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.Green;
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (PrintingStatus == 1)
                {
                    e.CellStyle.BackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (PrintingStatus == 0)
                {
                    e.CellStyle.BackColor = Security.GridsBackColor;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            }
        }

        private void kryptonContextMenuItem17_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ProfilAngle90Assignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Marsel1) || FrontID == Convert.ToInt32(Fronts.Marsel5) || FrontID == Convert.ToInt32(Fronts.Marsel3) ||
                    FrontID == Convert.ToInt32(Fronts.Marsel4) ||
                    FrontID == Convert.ToInt32(Fronts.Jersy110) || FrontID == Convert.ToInt32(Fronts.Porto) || FrontID == Convert.ToInt32(Fronts.Monte) ||
                    FrontID == Convert.ToInt32(Fronts.Techno1) || FrontID == Convert.ToInt32(Fronts.Shervud) ||
                    FrontID == Convert.ToInt32(Fronts.Techno2) || FrontID == Convert.ToInt32(Fronts.Techno4) || FrontID == Convert.ToInt32(Fronts.pFox) ||
                    FrontID == Convert.ToInt32(Fronts.pFlorenc) ||
                    FrontID == Convert.ToInt32(Fronts.Techno5) || FrontID == Convert.ToInt32(Fronts.PRU8))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }

            ProfilAngle90Assignments.GetFrontsID = FrontsID;
            bool HasOrders = ProfilAngle90Assignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcel(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
                //dgvWBatches_SelectionChanged(null, null);
            }
            ProfilAngle90Assignments.ClearOrders();
            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.PR1) || FrontID == Convert.ToInt32(Fronts.PR2))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }
            HasOrders = ProfilAngle90Assignments.GetPR1Orders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcelPR1(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
                //dgvWBatches_SelectionChanged(null, null);
            }
            ProfilAngle90Assignments.ClearOrders();
            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.PR3))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }
            HasOrders = ProfilAngle90Assignments.GetPR3Orders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcelPR3(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            ProfilAngle90Assignments.ClearOrders();
            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.PRU8))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }
            HasOrders = ProfilAngle90Assignments.GetPRU8Orders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ProfilAngle90Assignments.CreateExcelPRU8(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(4, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem18_Click(object sender, EventArgs e)
        {
            //string ClientName = string.Empty;
            //string BatchName = string.Empty;

            //Techno4DominoAssignments.ClearOrders();

            //int FactoryID = 0;
            //int WorkAssignmentID = 0;
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
            //    WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
            //    BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            //if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
            //    FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            //if (BatchName.Length == 0)
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
            //    return;
            //}

            //Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            //T.Start();
            //while (!SplashWindow.bSmallCreated) ;
            //NeedSplash = false;

            //bool M = BatchName.Contains('М');
            //bool Z = BatchName.Contains('З');
            //if (M && Z)
            //    ClientName = "Маркетинг + ЗОВ";
            //if (M && !Z)
            //    ClientName = "Маркетинг";
            //if (!M && Z)
            //    ClientName = "ЗОВ";
            //if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
            //    ClientName = "Москва-1";
            //string sSourceFileName = string.Empty;

            //FrontsID.Clear();
            //for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            //{
            //    int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
            //    if (FrontID == Convert.ToInt32(Fronts.Techno4Domino))
            //        FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            //}
            //Techno4DominoAssignments.GetFrontsID = FrontsID;
            //bool HasOrders = Techno4DominoAssignments.GetOrders(WorkAssignmentID, FactoryID, true);
            //if (HasOrders)
            //{
            //    Techno4DominoAssignments.CreateExcel(WorkAssignmentID, BatchName, ClientName, ref sSourceFileName);
            //    ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
            //    ControlAssignmentsManager.SetPrintDateTime(6, WorkAssignmentID);
            //    ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            //    //dgvWBatches_SelectionChanged(null, null);
            //}
            //Techno4DominoAssignments.ClearOrders();
            //HasOrders = Techno4DominoAssignments.GetOrders(WorkAssignmentID, FactoryID, false);
            //if (HasOrders)
            //{
            //    Techno4DominoAssignments.CreateExcelCurvedDomino(WorkAssignmentID, BatchName, ClientName, ref sSourceFileName);
            //    ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
            //    ControlAssignmentsManager.SetPrintDateTime(6, WorkAssignmentID);
            //    ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            //    //dgvWBatches_SelectionChanged(null, null);
            //}
            //dgvWBatches_SelectionChanged(null, null);
            //ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            //ControlAssignmentsManager.SaveWorkAssignments();
            //NeedSplash = true;
            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void dgvWorkAssignments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int TPS45PrintingStatus = 0;
            int GenevaPrintingStatus = 0;
            int TafelPrintingStatus = 0;
            int Profil90PrintingStatus = 0;
            int Profil45PrintingStatus = 0;
            int DominoPrintingStatus = 0;

            if (grid.Rows[e.RowIndex].Cells["TPS45PrintingStatus"].Value != DBNull.Value)
                TPS45PrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["TPS45PrintingStatus"].Value);
            if (grid.Rows[e.RowIndex].Cells["GenevaPrintingStatus"].Value != DBNull.Value)
                GenevaPrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["GenevaPrintingStatus"].Value);
            if (grid.Rows[e.RowIndex].Cells["TafelPrintingStatus"].Value != DBNull.Value)
                TafelPrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["TafelPrintingStatus"].Value);
            if (grid.Rows[e.RowIndex].Cells["Profil90PrintingStatus"].Value != DBNull.Value)
                Profil90PrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Profil90PrintingStatus"].Value);
            if (grid.Rows[e.RowIndex].Cells["Profil45PrintingStatus"].Value != DBNull.Value)
                Profil45PrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["Profil45PrintingStatus"].Value);
            if (grid.Rows[e.RowIndex].Cells["DominoPrintingStatus"].Value != DBNull.Value)
                DominoPrintingStatus = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["DominoPrintingStatus"].Value);

            if ((grid.Columns["Profil45DateTime"].Index == e.ColumnIndex || grid.Columns["Profil45UserColumn"].Index == e.ColumnIndex) && e.RowIndex >= 0)
            {
                if (Profil45PrintingStatus == 2 || Profil45PrintingStatus == -1)
                {
                    e.CellStyle.BackColor = Color.Green;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.Green;
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (Profil45PrintingStatus == 1)
                {
                    e.CellStyle.BackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (Profil45PrintingStatus == 0)
                {
                    e.CellStyle.BackColor = Security.GridsBackColor;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            }
            if ((grid.Columns["Profil90DateTime"].Index == e.ColumnIndex || grid.Columns["Profil90UserColumn"].Index == e.ColumnIndex) && e.RowIndex >= 0)
            {
                if (Profil90PrintingStatus == 2 || Profil90PrintingStatus == -1)
                {
                    e.CellStyle.BackColor = Color.Green;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.Green;
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (Profil90PrintingStatus == 1)
                {
                    e.CellStyle.BackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (Profil90PrintingStatus == 0)
                {
                    e.CellStyle.BackColor = Security.GridsBackColor;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            }
            if ((grid.Columns["DominoDateTime"].Index == e.ColumnIndex || grid.Columns["DominoUserColumn"].Index == e.ColumnIndex) && e.RowIndex >= 0)
            {
                if (DominoPrintingStatus == 2 || DominoPrintingStatus == -1)
                {
                    e.CellStyle.BackColor = Color.Green;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.Green;
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (DominoPrintingStatus == 1)
                {
                    e.CellStyle.BackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(253, 164, 61);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
                if (DominoPrintingStatus == 0)
                {
                    e.CellStyle.BackColor = Security.GridsBackColor;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(121, 177, 229);
                    e.CellStyle.SelectionForeColor = Color.White;
                }
            }
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Thread T1 = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T1.Start();

            while (!SplashForm.bCreated) ;

            bool bPrint = false;
            DataTable EditFrontOrdersDT = new DataTable();
            TafelManager TafelManager = new TafelManager(WFrontsOrdersManager);
            TafelManager.FillTables();

            EditTafelForm EditTafelForm = new EditTafelForm(this, ref TafelManager);

            TopForm = EditTafelForm;

            EditTafelForm.ShowDialog();
            bPrint = EditTafelForm.bPrint;
            EditFrontOrdersDT = TafelManager.EditFrontOrdersDT;

            EditTafelForm.Close();
            EditTafelForm.Dispose();

            TopForm = null;
            if (!bPrint)
                return;

            string ClientName = string.Empty;
            string BatchName = string.Empty;

            TafelAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            string sSourceFileName = string.Empty;
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            bool HasOrders = TafelAssignments.GetOrders(EditFrontOrdersDT, WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                TafelAssignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.SetPrintDateTime(3, WorkAssignmentID);
                ControlAssignmentsManager.SaveWorkAssignments();
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();
            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            int TechStoreID = 0;
            int InsetTypeID = 0;
            int PatinaID = 0;
            int Height = 0;
            int Width = 0;
            if (dgvWBatchFronts.SelectedRows[0].Cells["FrontID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(dgvWBatchFronts.SelectedRows[0].Cells["FrontID"].Value);
            if (dgvWBatchFronts.SelectedRows[0].Cells["InsetTypeID"].Value != DBNull.Value)
                InsetTypeID = Convert.ToInt32(dgvWBatchFronts.SelectedRows[0].Cells["InsetTypeID"].Value);
            if (dgvWBatchFronts.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(dgvWBatchFronts.SelectedRows[0].Cells["PatinaID"].Value);
            if (dgvWBatchFronts.SelectedRows[0].Cells["Height"].Value != DBNull.Value)
                Height = Convert.ToInt32(dgvWBatchFronts.SelectedRows[0].Cells["Height"].Value);
            if (dgvWBatchFronts.SelectedRows[0].Cells["Width"].Value != DBNull.Value)
                Width = Convert.ToInt32(dgvWBatchFronts.SelectedRows[0].Cells["Width"].Value);
            FrontsProdCapacityForm FrontsProdCapacityForm = new FrontsProdCapacityForm(this, TechStoreID, InsetTypeID, PatinaID, Height, Width);

            TopForm = FrontsProdCapacityForm;

            FrontsProdCapacityForm.ShowDialog();

            FrontsProdCapacityForm.Close();
            FrontsProdCapacityForm.Dispose();
        }

        private void cbtnTafel_Click(object sender, EventArgs e)
        {

        }

        private void kryptonContextMenuItem20_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            Tafel1Assignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            string sSourceFileName = string.Empty;
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            bool HasOrders = Tafel1Assignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                Tafel1Assignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.SetPrintDateTime(7, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem22_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ImpostAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Marsel3) || FrontID == Convert.ToInt32(Fronts.Marsel4) || FrontID == Convert.ToInt32(Fronts.Jersy110) ||
                    FrontID == Convert.ToInt32(Fronts.Porto) || FrontID == Convert.ToInt32(Fronts.Monte) ||
                    FrontID == Convert.ToInt32(Fronts.PR1) || FrontID == Convert.ToInt32(Fronts.PR2))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            ImpostAssignments.GetFrontsID = FrontsID;
            bool HasOrders = ImpostAssignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ImpostAssignments.CreateExcel(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
            }
            ImpostAssignments.ClearOrders();

            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem21_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ImpostAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.Rows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Marsel3) || FrontID == Convert.ToInt32(Fronts.Marsel4) || FrontID == Convert.ToInt32(Fronts.Jersy110) ||
                    FrontID == Convert.ToInt32(Fronts.Porto) || FrontID == Convert.ToInt32(Fronts.Monte) ||
                    FrontID == Convert.ToInt32(Fronts.PR1) || FrontID == Convert.ToInt32(Fronts.PR2))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.Rows[i].Cells["FrontID"].Value));
            }
            ImpostAssignments.GetFrontsID = FrontsID;
            bool HasOrders = ImpostAssignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ImpostAssignments.CreateExcel(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
            }
            ImpostAssignments.ClearOrders();

            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem23_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            ImpostAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Marsel3) || FrontID == Convert.ToInt32(Fronts.Marsel4) || FrontID == Convert.ToInt32(Fronts.Jersy110) ||
                    FrontID == Convert.ToInt32(Fronts.Porto) || FrontID == Convert.ToInt32(Fronts.Monte) ||
                    FrontID == Convert.ToInt32(Fronts.PR1)
                     || FrontID == Convert.ToInt32(Fronts.PR2))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }
            ImpostAssignments.GetFrontsID = FrontsID;
            bool HasOrders = ImpostAssignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                ImpostAssignments.CreateExcel(WorkAssignmentID, FactoryID, BatchName, ClientName, ref sSourceFileName);
            }
            ImpostAssignments.ClearOrders();

            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem24_Click(object sender, EventArgs e)
        {
            Machines Machine = Machines.No_machine;
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            TPSAngle45Assignments.ClearOrders();

            int FactoryID = 0;
            int MachineID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Machine"].Value != DBNull.Value)
                MachineID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["Machine"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (MachineID == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Не выбран станок", "Внимание");
                return;
            }

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            switch (MachineID)
            {
                case 0:
                    Machine = Machines.No_machine;
                    break;
                case 1:
                    Machine = Machines.ELME;
                    break;
                case 2:
                    Machine = Machines.Balistrini;
                    break;
                case 3:
                    Machine = Machines.Rapid;
                    break;
                default:
                    break;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            FrontsID.Clear();
            for (int i = 0; i < dgvFrontsSummary.SelectedRows.Count; i++)
            {
                int FrontID = Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value);
                if (FrontID == Convert.ToInt32(Fronts.Lorenzo) || FrontID == Convert.ToInt32(Fronts.Elegant) || FrontID == Convert.ToInt32(Fronts.ElegantPat) || FrontID == Convert.ToInt32(Fronts.KansasPat) || FrontID == Convert.ToInt32(Fronts.Kansas) || FrontID == Convert.ToInt32(Fronts.Infiniti) ||
                    FrontID == Convert.ToInt32(Fronts.Dakota) || FrontID == Convert.ToInt32(Fronts.DakotaPat) || FrontID == Convert.ToInt32(Fronts.Sofia) || FrontID == Convert.ToInt32(Fronts.Turin1) || FrontID == Convert.ToInt32(Fronts.Turin1_1) || FrontID == Convert.ToInt32(Fronts.Turin3) ||
                    FrontID == Convert.ToInt32(Fronts.LeonTPS) || FrontID == Convert.ToInt32(Fronts.InfinitiPat))
                    FrontsID.Add(Convert.ToInt32(dgvFrontsSummary.SelectedRows[i].Cells["FrontID"].Value));
            }
            TPSAngle45Assignments.GetFrontsID = FrontsID;
            bool HasOrders = TPSAngle45Assignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                TPSAngle45Assignments.CreateExcel(WorkAssignmentID, Machine, ClientName, BatchName, ref sSourceFileName);
                ControlAssignmentsManager.PrintAssignment(WorkAssignmentID, WFrontsOrdersManager.DinstinctFrontsDT(FrontsID.OfType<int>().ToArray()));
                ControlAssignmentsManager.SetPrintDateTime(1, WorkAssignmentID);
                ControlAssignmentsManager.SetInProduction(WorkAssignmentID, FactoryID);
            }
            dgvWBatches_SelectionChanged(null, null);
            ControlAssignmentsManager.SetPrintingStatus(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SaveWorkAssignments();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem25_Click(object sender, EventArgs e)
        {

        }

        private void kryptonContextMenuItem27_Click(object sender, EventArgs e)
        {
            string ClientName = string.Empty;
            string BatchName = string.Empty;

            DecorAssignments.ClearOrders();

            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value != DBNull.Value)
                BatchName = dgvWorkAssignments.SelectedRows[0].Cells["Name"].Value.ToString();
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            if (BatchName.Length == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "В задании нет партий", "Внимание");
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            bool M = BatchName.Contains('М');
            bool Z = BatchName.Contains('З');
            if (M && Z)
                ClientName = "Маркетинг + ЗОВ";
            if (M && !Z)
                ClientName = "Маркетинг";
            if (!M && Z)
                ClientName = "ЗОВ";
            if (ControlAssignmentsManager.IsM1(WorkAssignmentID, FactoryID))
                ClientName = "Москва-1";
            string sSourceFileName = string.Empty;

            bool HasOrders = DecorAssignments.GetOrders(WorkAssignmentID, FactoryID);
            if (HasOrders)
            {
                DecorAssignments.CreateExcel(WorkAssignmentID, ClientName, BatchName, ref sSourceFileName);
            }
            dgvWBatches_SelectionChanged(null, null);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.BringToFront();
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void btnUpdateComplements_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Обновление.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;

            DateTime date1 = DateTimePicker1.Value.Date;
            DateTime date2 = DateTimePicker2.Value.Date;

            ControlAssignmentsManager.UpdateWorkAssignments(FactoryID, date1, date2);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }
    }
}
