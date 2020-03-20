using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ComplexSawingForm : Form
    {
        public bool OkComplexSawing = true;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int DecorAssignmentID = 0;
        int FormEvent = 0;

        ProfileAssignments ProfileAssignmentsManager;

        public ComplexSawingForm(ProfileAssignments tProfileAssignmentsManager, int iDecorAssignmentID)
        {
            InitializeComponent();
            ProfileAssignmentsManager = tProfileAssignmentsManager;
            DecorAssignmentID = iDecorAssignmentID;
            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            ProfileAssignmentsManager.GetComplexSawing(DecorAssignmentID);
            dgvComplexSawing.DataSource = ProfileAssignmentsManager.ComplexSawingList;
            dgvComplexSawingSetting(ref dgvComplexSawing);
        }

        private void dgvComplexSawingSetting(ref PercentageDataGrid grid)
        {
            grid.Columns.Add(ProfileAssignmentsManager.SawStripsColumn);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("ProfilOrderStatusID"))
                grid.Columns["ProfilOrderStatusID"].Visible = false;
            if (grid.Columns.Contains("BalanceStatus"))
                grid.Columns["BalanceStatus"].Visible = false;
            if (grid.Columns.Contains("MegaOrderID"))
                grid.Columns["MegaOrderID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentStatusID"))
                grid.Columns["DecorAssignmentStatusID"].Visible = false;
            if (grid.Columns.Contains("ComplexSawing"))
                grid.Columns["ComplexSawing"].Visible = false;
            if (grid.Columns.Contains("Height2"))
                grid.Columns["Height2"].Visible = false;
            if (grid.Columns.Contains("Thickness2"))
                grid.Columns["Thickness2"].Visible = false;
            if (grid.Columns.Contains("Diameter2"))
                grid.Columns["Diameter2"].Visible = false;
            if (grid.Columns.Contains("BatchAssignmentID"))
                grid.Columns["BatchAssignmentID"].Visible = false;
            if (grid.Columns.Contains("Notes"))
                grid.Columns["Notes"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
            if (grid.Columns.Contains("MainOrderID"))
                grid.Columns["MainOrderID"].Visible = false;
            if (grid.Columns.Contains("DecorOrderID"))
                grid.Columns["DecorOrderID"].Visible = false;
            if (grid.Columns.Contains("TechStoreID1"))
                grid.Columns["TechStoreID1"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("Count1"))
                grid.Columns["Count1"].Visible = false;
            if (grid.Columns.Contains("TechStoreID2"))
                grid.Columns["TechStoreID2"].Visible = false;
            if (grid.Columns.Contains("CoverID2"))
                grid.Columns["CoverID2"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("AddToPlanUserID"))
                grid.Columns["AddToPlanUserID"].Visible = false;
            if (grid.Columns.Contains("AddToPlanDateTime"))
                grid.Columns["AddToPlanDateTime"].Visible = false;
            if (grid.Columns.Contains("InPlan"))
                grid.Columns["InPlan"].Visible = false;
            if (grid.Columns.Contains("FactCount"))
                grid.Columns["FactCount"].Visible = false;
            if (grid.Columns.Contains("DisprepancyCount"))
                grid.Columns["DisprepancyCount"].Visible = false;
            if (grid.Columns.Contains("DefectCount"))
                grid.Columns["DefectCount"].Visible = false;
            if (grid.Columns.Contains("Length1"))
                grid.Columns["Length1"].Visible = false;
            if (grid.Columns.Contains("Width1"))
                grid.Columns["Width1"].Visible = false;
            if (grid.Columns.Contains("BarberanNumber"))
                grid.Columns["BarberanNumber"].Visible = false;
            if (grid.Columns.Contains("FrezerNumber"))
                grid.Columns["FrezerNumber"].Visible = false;
            if (grid.Columns.Contains("SawNumber"))
                grid.Columns["SawNumber"].Visible = false;
            if (grid.Columns.Contains("LinkAssignmentID"))
                grid.Columns["LinkAssignmentID"].Visible = false;
            if (grid.Columns.Contains("PrintUserID"))
                grid.Columns["PrintUserID"].Visible = false;
            if (grid.Columns.Contains("PrintDateTime"))
                grid.Columns["PrintDateTime"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["PlanCount"].HeaderText = "Кол-во";

            int DisplayIndex = 0;
            grid.Columns["SawStripsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["PlanCount"].DisplayIndex = DisplayIndex++;

            grid.Columns["SawStripsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["SawStripsColumn"].MinimumWidth = 150;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["PlanCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PlanCount"].MinimumWidth = 150;
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        this.Hide();
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        this.Hide();
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

        private void ComplexSawingForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void ComplexSawingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void OkSawingButton_Click(object sender, EventArgs e)
        {
            //if (Count < Convert.ToInt32(CountTextBox.Text))
            //{
            //    Infinium.LightMessageBox.Show(ref TopForm, false,
            //        "Недопустимое значение",
            //        "Ошибка");
            //    return;
            //}

            OkComplexSawing = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelSawingButton_Click(object sender, EventArgs e)
        {
            OkComplexSawing = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
