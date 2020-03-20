using Infinium.Modules.DyeingAssignments;

using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DyeingAssignmentsMainForm : Form
    {
        const int iResponsibleRole = 58;
        const int iTechnologyRole = 59;
        const int iControlRole = 60;
        const int iAgreementRole = 61;
        const int iAdminRole = 62;
        const int iWorkerRole = 63;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        DataTable RolesDataTable;
        Form TopForm = null;
        LightStartForm LightStartForm;

        ControlAssignments ControlAssignmentsManager;
        PrintDyeingAssignments PrintDyeingAssignmentsManager;

        RoleTypes RoleType = RoleTypes.OrdinaryRole;

        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1,
            ResponsibleRole = 2,
            TechnologyRole = 3,
            ControlRole = 4,
            AgreementRole = 5,
            WorkerRole = 6
        }

        public DyeingAssignmentsMainForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            RolesDataTable = ControlAssignmentsManager.GetPermissions(Security.CurrentUserID, this.Name);

            if (PermissionGranted(iResponsibleRole))
            {
                RoleType = RoleTypes.ResponsibleRole;
                cmiOpenAssingnment.Visible = true;
                cmiSetResponsible.Visible = true;
                cmiSetTechnology.Visible = false;
                cmiSetControl.Visible = false;
                cmiSetAgreement.Visible = false;
                cmiSaveAssignments.Visible = true;
                kryptonContextMenuItem5.Visible = true;
                kryptonContextMenuItem10.Visible = true;
                kryptonContextMenuItem11.Visible = false;
                kryptonContextMenuItem12.Visible = false;
                kryptonContextMenuItem13.Visible = false;
            }
            if (PermissionGranted(iTechnologyRole))
            {
                RoleType = RoleTypes.TechnologyRole;
                cmiOpenAssingnment.Visible = false;
                cmiSetResponsible.Visible = false;
                cmiSetTechnology.Visible = true;
                cmiSetControl.Visible = false;
                cmiSetAgreement.Visible = false;
                cmiSaveAssignments.Visible = false;
                kryptonContextMenuItem5.Visible = false;
                kryptonContextMenuItem10.Visible = false;
                kryptonContextMenuItem11.Visible = true;
                kryptonContextMenuItem12.Visible = false;
                kryptonContextMenuItem13.Visible = false;
            }
            if (PermissionGranted(iControlRole))
            {
                RoleType = RoleTypes.ControlRole;
                cmiOpenAssingnment.Visible = false;
                cmiSetResponsible.Visible = false;
                cmiSetTechnology.Visible = false;
                cmiSetControl.Visible = true;
                cmiSetAgreement.Visible = false;
                cmiSaveAssignments.Visible = false;
                kryptonContextMenuItem5.Visible = false;
                kryptonContextMenuItem10.Visible = false;
                kryptonContextMenuItem11.Visible = false;
                kryptonContextMenuItem12.Visible = true;
                kryptonContextMenuItem13.Visible = false;
            }
            if (PermissionGranted(iAgreementRole))
            {
                RoleType = RoleTypes.AgreementRole;
                cmiOpenAssingnment.Visible = false;
                cmiSetResponsible.Visible = false;
                cmiSetTechnology.Visible = false;
                cmiSetControl.Visible = false;
                cmiSetAgreement.Visible = true;
                cmiSaveAssignments.Visible = false;
                kryptonContextMenuItem5.Visible = false;
                kryptonContextMenuItem10.Visible = false;
                kryptonContextMenuItem11.Visible = false;
                kryptonContextMenuItem12.Visible = false;
                kryptonContextMenuItem13.Visible = true;
            }
            if (PermissionGranted(iAdminRole))
            {
                RoleType = RoleTypes.AdminRole;
                cmiOpenAssingnment.Visible = true;
                cmiSetResponsible.Visible = true;
                cmiSetTechnology.Visible = true;
                cmiSetControl.Visible = true;
                cmiSetAgreement.Visible = true;
                cmiSaveAssignments.Visible = true;
                kryptonContextMenuItem5.Visible = true;
                kryptonContextMenuItem10.Visible = true;
                kryptonContextMenuItem11.Visible = true;
                kryptonContextMenuItem12.Visible = true;
                kryptonContextMenuItem13.Visible = true;
            }
            if (PermissionGranted(iWorkerRole))
            {
                RoleType = RoleTypes.WorkerRole;
                cmiOpenAssingnment.Visible = true;
                cmiSetResponsible.Visible = false;
                cmiSetTechnology.Visible = false;
                cmiSetControl.Visible = false;
                cmiSetAgreement.Visible = false;
                cmiSaveAssignments.Visible = true;
                kryptonContextMenuItem5.Visible = true;
                kryptonContextMenuItem10.Visible = false;
                kryptonContextMenuItem11.Visible = false;
                kryptonContextMenuItem12.Visible = false;
                kryptonContextMenuItem13.Visible = false;
            }

            while (!SplashForm.bCreated) ;
        }

        private bool PermissionGranted(int RoleID)
        {
            DataRow[] Rows = RolesDataTable.Select("RoleID = " + RoleID);

            return Rows.Count() > 0;
        }

        private void DyeingAssignmentsForm_Shown(object sender, EventArgs e)
        {
            pnlDyeingAssignments.BringToFront();
            MenuPanel.BringToFront();

            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
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
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            ControlAssignmentsManager = new ControlAssignments();
            ControlAssignmentsManager.Initialize();

            PrintDyeingAssignmentsManager = new PrintDyeingAssignments(ControlAssignmentsManager);
            PrintDyeingAssignmentsManager.Initialize();

            dgvWorkAssignments.DataSource = ControlAssignmentsManager.WorkAssignmentsList;
            dgvDyeingAssignments.DataSource = ControlAssignmentsManager.DyeingAssignmentsList;

            dgvWorkAssignmentsSetting(ref dgvWorkAssignments);
            dgvDyeingAssignmentsSetting(ref dgvDyeingAssignments);

            DateTime FirstDay = DateTime.Now.AddDays(-30);
            CalendarFrom.SelectionEnd = FirstDay;
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            ControlAssignmentsManager.UpdateWorkAssignments(2);
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.IsWorkAssignmentPrinted();
        }

        private void dgvDyeingAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ControlAssignmentsManager.CreationUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.ResponsibleUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.TechnologyUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.ControlUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.AgreementUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.PrintUserColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("ResponsibleUserID"))
                grid.Columns["ResponsibleUserID"].Visible = false;
            if (grid.Columns.Contains("TechnologyUserID"))
                grid.Columns["TechnologyUserID"].Visible = false;
            if (grid.Columns.Contains("ControlUserID"))
                grid.Columns["ControlUserID"].Visible = false;
            if (grid.Columns.Contains("AgreementUserID"))
                grid.Columns["AgreementUserID"].Visible = false;
            if (grid.Columns.Contains("GroupType"))
                grid.Columns["GroupType"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("TechCatalogOperationsGroupID"))
                grid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            if (grid.Columns.Contains("DyeingAssignmentStatusID"))
                grid.Columns["DyeingAssignmentStatusID"].Visible = false;

            //grid.Columns["CreationDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            //grid.Columns["ResponsibleDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            //grid.Columns["TechnologyDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            //grid.Columns["ControlDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            //grid.Columns["AgreementDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            //grid.Columns["PrintDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            grid.Columns["GroupName"].HeaderText = "Название";
            grid.Columns["DyeingAssignmentID"].HeaderText = "№";
            grid.Columns["DyeingAssignmentStatusID"].HeaderText = "Статус";
            grid.Columns["CreationDateTime"].HeaderText = "Дата создания";
            grid.Columns["ResponsibleDateTime"].HeaderText = "Время";
            grid.Columns["TechnologyDateTime"].HeaderText = "Время";
            grid.Columns["ControlDateTime"].HeaderText = "Время";
            grid.Columns["AgreementDateTime"].HeaderText = "Время";

            grid.Columns["PrintDateTime"].HeaderText = "Дата печати";
            grid.Columns["Notes"].HeaderText = "Примечание";

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
            }

            grid.Columns["DyeingAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DyeingAssignmentID"].Width = 55;
            grid.Columns["DyeingAssignmentStatusID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DyeingAssignmentStatusID"].Width = 100;
            grid.Columns["TechCatalogOperationsGroupID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechCatalogOperationsGroupID"].MinimumWidth = 55;
            grid.Columns["CreationDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CreationDateTime"].MinimumWidth = 55;
            grid.Columns["AgreementDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AgreementDateTime"].MinimumWidth = 55;
            grid.Columns["PrintDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PrintDateTime"].MinimumWidth = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 55;
            grid.Columns["GroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["GroupName"].MinimumWidth = 55;

            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["DyeingAssignmentID"].DisplayIndex = DisplayIndex++;
            grid.Columns["GroupName"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreationDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreationUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ResponsibleDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["ResponsibleUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnologyDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnologyUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ControlDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["ControlUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["AgreementDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["AgreementUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PrintDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["PrintUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        private void dgvWorkAssignmentsSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ControlAssignmentsManager.ResponsibleUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.TechnologyUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.ControlUserColumn);
            grid.Columns.Add(ControlAssignmentsManager.AgreementUserColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("ResponsibleUserID"))
                grid.Columns["ResponsibleUserID"].Visible = false;
            if (grid.Columns.Contains("TechnologyUserID"))
                grid.Columns["TechnologyUserID"].Visible = false;
            if (grid.Columns.Contains("ControlUserID"))
                grid.Columns["ControlUserID"].Visible = false;
            if (grid.Columns.Contains("AgreementUserID"))
                grid.Columns["AgreementUserID"].Visible = false;

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
            if (grid.Columns.Contains("TPS45PrintingStatus"))
                grid.Columns["TPS45PrintingStatus"].Visible = false;
            if (grid.Columns.Contains("RALUserID"))
                grid.Columns["RALUserID"].Visible = false;
            if (grid.Columns.Contains("GenevaPrintingStatus"))
                grid.Columns["GenevaPrintingStatus"].Visible = false;
            if (grid.Columns.Contains("TafelPrintingStatus"))
                grid.Columns["TafelPrintingStatus"].Visible = false;
            if (grid.Columns.Contains("Profil90PrintingStatus"))
                grid.Columns["Profil90PrintingStatus"].Visible = false;
            if (grid.Columns.Contains("Profil45PrintingStatus"))
                grid.Columns["Profil45PrintingStatus"].Visible = false;
            if (grid.Columns.Contains("DominoPrintingStatus"))
                grid.Columns["DominoPrintingStatus"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("TPS45DateTime"))
                grid.Columns["TPS45DateTime"].Visible = false;
            if (grid.Columns.Contains("GenevaDateTime"))
                grid.Columns["GenevaDateTime"].Visible = false;
            if (grid.Columns.Contains("TafelDateTime"))
                grid.Columns["TafelDateTime"].Visible = false;
            if (grid.Columns.Contains("Profil90DateTime"))
                grid.Columns["Profil90DateTime"].Visible = false;
            if (grid.Columns.Contains("Profil45DateTime"))
                grid.Columns["Profil45DateTime"].Visible = false;
            if (grid.Columns.Contains("DominoDateTime"))
                grid.Columns["DominoDateTime"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;

            grid.Columns["ResponsibleDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["TechnologyDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["ControlDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            grid.Columns["AgreementDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

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

            grid.Columns["ResponsibleDateTime"].HeaderText = "Время";
            grid.Columns["TechnologyDateTime"].HeaderText = "Время";
            grid.Columns["ControlDateTime"].HeaderText = "Время";
            grid.Columns["AgreementDateTime"].HeaderText = "Время";
            grid.Columns["Printed"].HeaderText = "Распечатано";

            grid.Columns["CreationDateTime"].HeaderText = "Дата создания";
            grid.Columns["TPS45DateTime"].HeaderText = "   Угол 45\r\nдата печати";
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

            grid.Columns["Printed"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Printed"].Width = 115;
            grid.Columns["WorkAssignmentID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["WorkAssignmentID"].Width = 55;
            grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Square"].Width = 100;
            grid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Name"].MinimumWidth = 55;
            grid.Columns["CreationDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CreationDateTime"].MinimumWidth = 55;
            grid.Columns["TPS45DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TPS45DateTime"].MinimumWidth = 55;
            grid.Columns["GenevaDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["GenevaDateTime"].MinimumWidth = 55;
            grid.Columns["TafelDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TafelDateTime"].MinimumWidth = 55;
            grid.Columns["Profil90DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Profil90DateTime"].MinimumWidth = 55;
            grid.Columns["Profil45DateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Profil45DateTime"].MinimumWidth = 55;
            grid.Columns["DominoDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["DominoDateTime"].MinimumWidth = 55;
            grid.Columns["PrintDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PrintDateTime"].MinimumWidth = 55;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 55;
            grid.Columns["RALDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["RALDateTime"].Width = 95;
            grid.Columns["ResponsibleDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ResponsibleDateTime"].MinimumWidth = 55;
            grid.Columns["TechnologyDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnologyDateTime"].MinimumWidth = 55;
            grid.Columns["ControlDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ControlDateTime"].MinimumWidth = 55;
            grid.Columns["AgreementDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["AgreementDateTime"].MinimumWidth = 55;

            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["WorkAssignmentID"].DisplayIndex = DisplayIndex++;
            grid.Columns["Name"].DisplayIndex = DisplayIndex++;
            grid.Columns["CreationDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Printed"].DisplayIndex = DisplayIndex++;
            grid.Columns["ResponsibleDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["ResponsibleUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnologyDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnologyUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["ControlDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["ControlUserColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["AgreementDateTime"].DisplayIndex = DisplayIndex++;
            grid.Columns["AgreementUserColumn"].DisplayIndex = DisplayIndex++;
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

        private void dgvWorkAssignments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ControlAssignmentsManager == null || (RoleType != RoleTypes.AdminRole && RoleType != RoleTypes.ResponsibleRole && RoleType != RoleTypes.WorkerRole))
                return;
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            ControlAssignmentsManager.NewAssignment = true;
            ControlAssignmentsManager.DyeingAssignmentID = 0;
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;
            ControlAssignmentsManager.ClearDyeingTables();
            ControlAssignmentsManager.FactoryID = FactoryID;
            ControlAssignmentsManager.WorkAssignmentID = WorkAssignmentID;

            ControlAssignmentsManager.UpdateMegaBatches(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.UpdateBatches(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.UpdateBatchMainOrders(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SumBatchFrontsSquare();

            DyeingAssignmentsCreateForm DyeingAssignmentsForm1 = new DyeingAssignmentsCreateForm(this, ref ControlAssignmentsManager, ref PrintDyeingAssignmentsManager, From, To);

            TopForm = DyeingAssignmentsForm1;

            DyeingAssignmentsForm1.ShowDialog();

            DyeingAssignmentsForm1.Close();
            DyeingAssignmentsForm1.Dispose();

            TopForm = null;
        }

        private void dgvWorkAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType != RoleTypes.OrdinaryRole && e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiOpenAssingnment_Click(object sender, EventArgs e)
        {
            if (ControlAssignmentsManager == null)
                return;
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int FactoryID = 0;
            int WorkAssignmentID = 0;
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                FactoryID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["FactoryID"].Value);

            ControlAssignmentsManager.NewAssignment = true;
            ControlAssignmentsManager.DyeingAssignmentID = 0;
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ControlAssignmentsManager.ClearDyeingTables();
            ControlAssignmentsManager.FactoryID = FactoryID;
            ControlAssignmentsManager.WorkAssignmentID = WorkAssignmentID;

            ControlAssignmentsManager.UpdateMegaBatches(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.UpdateBatches(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.UpdateBatchMainOrders(WorkAssignmentID, FactoryID);
            ControlAssignmentsManager.SumBatchFrontsSquare();

            DyeingAssignmentsCreateForm DyeingAssignmentsForm1 = new DyeingAssignmentsCreateForm(this, ref ControlAssignmentsManager, ref PrintDyeingAssignmentsManager, From, To);

            TopForm = DyeingAssignmentsForm1;

            DyeingAssignmentsForm1.ShowDialog();

            DyeingAssignmentsForm1.Close();
            DyeingAssignmentsForm1.Dispose();

            TopForm = null;
        }

        private void kryptonCheckSet3_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ControlAssignmentsManager == null)
                return;
            if (kryptonCheckSet3.CheckedButton == cbtnWorkAssignments)
            {
                ControlAssignmentsManager.NewAssignment = true;
                ControlAssignmentsManager.DyeingAssignmentID = 0;
                ControlAssignmentsManager.TechCatalogOperationsGroupID = 0;
                pnlWorkAssignments.BringToFront();
                if (MenuPanel.Visible)
                    MenuPanel.BringToFront();
            }
            if (kryptonCheckSet3.CheckedButton == cbtnDyeingAssignments)
            {
                int DyeingAssignmentID = ControlAssignmentsManager.DyeingAssignmentID;
                DateTime From = CalendarFrom.SelectionEnd;
                DateTime To = CalendarTo.SelectionEnd;
                ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
                ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
                pnlDyeingAssignments.BringToFront();
                if (MenuPanel.Visible)
                    MenuPanel.BringToFront();
            }
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вы собираетесь создать новое задание. Продолжить?",
                    "Новое задание");

            if (!OKCancel)
                return;
            int DyeingAssignmentID = 0;
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.CreateDyeingAssignment(0);
            ControlAssignmentsManager.SaveDyeingAssignments();
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToFirstDyeingAssignmentID();

            if (dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            ControlAssignmentsManager.DyeingAssignmentID = DyeingAssignmentID;
            ControlAssignmentsManager.NewAssignment = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvDyeingAssignments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ControlAssignmentsManager == null)
                return;
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            int TechCatalogOperationsGroupID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                TechCatalogOperationsGroupID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            ControlAssignmentsManager.DyeingAssignmentID = DyeingAssignmentID;
            ControlAssignmentsManager.TechCatalogOperationsGroupID = TechCatalogOperationsGroupID;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ControlAssignmentsManager.NewAssignment = false;
            ControlAssignmentsManager.ClearBatchTables();

            ControlAssignmentsManager.GetDyeingAssignmentsInfo(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateDyeingCarts(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateMegaBatches(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateBatches(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateBatchMainOrders(DyeingAssignmentID);
            ControlAssignmentsManager.SumBatchFrontsSquare();

            DyeingAssignmentsCreateForm DyeingAssignmentsForm1 = new DyeingAssignmentsCreateForm(this, ref ControlAssignmentsManager, ref PrintDyeingAssignmentsManager, From, To);

            TopForm = DyeingAssignmentsForm1;

            DyeingAssignmentsForm1.ShowDialog();

            DyeingAssignmentsForm1.Close();
            DyeingAssignmentsForm1.Dispose();

            TopForm = null;

            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
        }

        private void dgvDyeingAssignments_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (RoleType != RoleTypes.OrdinaryRole && e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                //ControlAssignmentsManager.MoveToDyeingAssignmentPos(e.RowIndex);
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            if (ControlAssignmentsManager == null)
                return;
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            int TechCatalogOperationsGroupID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                TechCatalogOperationsGroupID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            ControlAssignmentsManager.DyeingAssignmentID = DyeingAssignmentID;
            ControlAssignmentsManager.TechCatalogOperationsGroupID = TechCatalogOperationsGroupID;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ControlAssignmentsManager.NewAssignment = false;
            ControlAssignmentsManager.ClearBatchTables();

            ControlAssignmentsManager.GetDyeingAssignmentsInfo(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateDyeingCarts(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateMegaBatches(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateBatches(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateBatchMainOrders(DyeingAssignmentID);
            ControlAssignmentsManager.SumBatchFrontsSquare();

            DyeingAssignmentsCreateForm DyeingAssignmentsForm1 = new DyeingAssignmentsCreateForm(this, ref ControlAssignmentsManager, ref PrintDyeingAssignmentsManager, From, To);

            TopForm = DyeingAssignmentsForm1;

            DyeingAssignmentsForm1.ShowDialog();

            DyeingAssignmentsForm1.Close();
            DyeingAssignmentsForm1.Dispose();

            TopForm = null;
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            int DyeingAssignmentID = 0;
            int TechCatalogOperationsGroupID = 0;
            int RowIndex = 0;
            PrintDyeingAssignmentsManager.CreateWorkBook();
            for (int i = 0; i < dgvDyeingAssignments.SelectedRows.Count; i++)
            {
                if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[i].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                    DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[i].Cells["DyeingAssignmentID"].Value);
                if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[i].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                    TechCatalogOperationsGroupID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[i].Cells["TechCatalogOperationsGroupID"].Value);

                //if (!ControlAssignmentsManager.GetDyeingAssignmentStatus(DyeingAssignmentID))
                //{
                //    bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, false,
                //            "Задание не согласовано",
                //            "Печать запрещена");

                //    return;
                //}

                object ResponsibleDateTime = dgvDyeingAssignments.SelectedRows[i].Cells["ResponsibleDateTime"].Value;
                object TechnologyDateTime = dgvDyeingAssignments.SelectedRows[i].Cells["TechnologyDateTime"].Value;
                object ControlDateTime = dgvDyeingAssignments.SelectedRows[i].Cells["ControlDateTime"].Value;
                object AgreementDateTime = dgvDyeingAssignments.SelectedRows[i].Cells["AgreementDateTime"].Value;
                object PrintDateTime = dgvDyeingAssignments.SelectedRows[i].Cells["PrintDateTime"].Value;
                object ResponsibleUserID = dgvDyeingAssignments.SelectedRows[i].Cells["ResponsibleUserID"].Value;
                object TechnologyUserID = dgvDyeingAssignments.SelectedRows[i].Cells["TechnologyUserID"].Value;
                object ControlUserID = dgvDyeingAssignments.SelectedRows[i].Cells["ControlUserID"].Value;
                object AgreementUserID = dgvDyeingAssignments.SelectedRows[i].Cells["AgreementUserID"].Value;
                object PrintUserID = dgvDyeingAssignments.SelectedRows[i].Cells["PrintUserID"].Value;

                PrintDyeingAssignmentsManager.GetDyeingAssignmentInfo(ref ResponsibleDateTime, ref TechnologyDateTime, ref ControlDateTime, ref AgreementDateTime, ref PrintDateTime,
                    ref ResponsibleUserID, ref TechnologyUserID, ref ControlUserID, ref AgreementUserID, ref PrintUserID);

                ControlAssignmentsManager.PrintDyeingAssignment(DyeingAssignmentID);
                ControlAssignmentsManager.SaveDyeingAssignments();
                CreateBarcodes(DyeingAssignmentID, TechCatalogOperationsGroupID);
                PrintDyeingAssignmentsManager.CreateDyeingAssignmentReport(DyeingAssignmentID, ref RowIndex);
            }
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            PrintDyeingAssignmentsManager.SaveOpenDyeingAssignmentReport(DyeingAssignmentID);
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void SaveDyeingAssignments()
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SaveDyeingAssignments();
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvDyeingAssignments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                SaveDyeingAssignments();
        }

        private void cmiSaveAssignments_Click(object sender, EventArgs e)
        {
            SaveDyeingAssignments();
        }

        private void CreateBarcodes(int DyeingAssignmentID, int TechCatalogOperationsGroupID)
        {
            //ControlAssignmentsManager.UpdateTechCatalogOperationsDetail(TechCatalogOperationsGroupID);
            //ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(DyeingAssignmentID);
            //ControlAssignmentsManager.UpdateDyeingCarts(DyeingAssignmentID);
            //ControlAssignmentsManager.RemoveDyeingAssignmentBarcode(DyeingAssignmentID, TechCatalogOperationsGroupID);
            //ControlAssignmentsManager.CreateDyeingAssignmentBarcode(DyeingAssignmentID, TechCatalogOperationsGroupID);
            //ControlAssignmentsManager.SaveDyeingAssignmentBarcodes();
            ControlAssignmentsManager.UpdateDyeingAssignmentBarcodes(DyeingAssignmentID);
            ControlAssignmentsManager.CreateBarcodes();
        }

        private void kryptonContextMenuItem9_Click(object sender, EventArgs e)
        {
            if (ControlAssignmentsManager == null)
                return;
            int DyeingAssignmentID = 0;
            int TechCatalogOperationsGroupID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                TechCatalogOperationsGroupID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            ControlAssignmentsManager.DyeingAssignmentID = DyeingAssignmentID;
            ControlAssignmentsManager.TechCatalogOperationsGroupID = TechCatalogOperationsGroupID;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;
            CreateBarcodes(DyeingAssignmentID, TechCatalogOperationsGroupID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void cmiSetResponsible_Click(object sender, EventArgs e)
        {
            int DyeingAssignmentID = 0;
            int WorkAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetWorkAssignmentAgreementPermissions(WorkAssignmentID, 0);
            ControlAssignmentsManager.SaveWorkAssignments();
            ControlAssignmentsManager.UpdateWorkAssignments(2);
            ControlAssignmentsManager.MoveToWorkAssignmentID(WorkAssignmentID);
            ControlAssignmentsManager.SetDyeingAssignmentPermissions(WorkAssignmentID, 0);
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
            ControlAssignmentsManager.IsWorkAssignmentPrinted();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem10_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetDyeingAssignmentPermissions2(DyeingAssignmentID, 0);
            ControlAssignmentsManager.SaveDyeingAssignments();
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cmiSetAgreement_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            int WorkAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetWorkAssignmentAgreementPermissions(WorkAssignmentID, 3);
            ControlAssignmentsManager.SaveWorkAssignments();
            ControlAssignmentsManager.UpdateWorkAssignments(2);
            ControlAssignmentsManager.MoveToWorkAssignmentID(WorkAssignmentID);
            ControlAssignmentsManager.SetDyeingAssignmentPermissions(WorkAssignmentID, 3);
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
            ControlAssignmentsManager.IsWorkAssignmentPrinted();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cmiSetTechnology_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            int WorkAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetWorkAssignmentAgreementPermissions(WorkAssignmentID, 1);
            ControlAssignmentsManager.SaveWorkAssignments();
            ControlAssignmentsManager.UpdateWorkAssignments(2);
            ControlAssignmentsManager.MoveToWorkAssignmentID(WorkAssignmentID);
            ControlAssignmentsManager.SetDyeingAssignmentPermissions(WorkAssignmentID, 1);
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
            ControlAssignmentsManager.IsWorkAssignmentPrinted();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cmiSetControl_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            int WorkAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);
            if (dgvWorkAssignments.SelectedRows.Count != 0 && dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value != DBNull.Value)
                WorkAssignmentID = Convert.ToInt32(dgvWorkAssignments.SelectedRows[0].Cells["WorkAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetWorkAssignmentAgreementPermissions(WorkAssignmentID, 2);
            ControlAssignmentsManager.SaveWorkAssignments();
            ControlAssignmentsManager.UpdateWorkAssignments(2);
            ControlAssignmentsManager.MoveToWorkAssignmentID(WorkAssignmentID);
            ControlAssignmentsManager.SetDyeingAssignmentPermissions(WorkAssignmentID, 2);
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);
            ControlAssignmentsManager.IsWorkAssignmentPrinted();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem11_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetDyeingAssignmentPermissions2(DyeingAssignmentID, 1);
            ControlAssignmentsManager.SaveDyeingAssignments();
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem12_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetDyeingAssignmentPermissions2(DyeingAssignmentID, 2);
            ControlAssignmentsManager.SaveDyeingAssignments();
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonContextMenuItem13_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.SetDyeingAssignmentPermissions2(DyeingAssignmentID, 3);
            ControlAssignmentsManager.SaveDyeingAssignments();
            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            panel62.Visible = false;
        }

        private void kryptonContextMenuItem14_Click(object sender, EventArgs e)
        {
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            percentageDataGrid1.DataSource = ControlAssignmentsManager.GetPercentageTable(DyeingAssignmentID);
            percentageDataGrid1.Columns["MachinesOperationName"].HeaderText = "Операция";
            percentageDataGrid1.Columns["ReadyPerc"].HeaderText = "Готовность, %";
            percentageDataGrid1.Columns["MachinesOperationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            percentageDataGrid1.Columns["MachinesOperationName"].MinimumWidth = 55;
            percentageDataGrid1.Columns["ReadyPerc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            percentageDataGrid1.Columns["ReadyPerc"].MinimumWidth = 55;
            percentageDataGrid1.Columns["ReadyPerc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            percentageDataGrid1.AddPercentageColumn("ReadyPerc");

            label40.Text = "Готовность задания №" + DyeingAssignmentID;
            panel62.Visible = true;
            panel62.Location = new Point(Cursor.Position.X - 10, Cursor.Position.Y - 10);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            DateTime From = CalendarFrom.SelectionEnd;
            DateTime To = CalendarTo.SelectionEnd;
            int DyeingAssignmentID = 0;
            if (dgvDyeingAssignments.SelectedRows.Count != 0 && dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value != DBNull.Value)
                DyeingAssignmentID = Convert.ToInt32(dgvDyeingAssignments.SelectedRows[0].Cells["DyeingAssignmentID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ControlAssignmentsManager.UpdateDyeingAssignments(From, To);
            ControlAssignmentsManager.MoveToDyeingAssignmentID(DyeingAssignmentID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
            if (MenuPanel.Visible)
                MenuPanel.BringToFront();
        }
    }
}
