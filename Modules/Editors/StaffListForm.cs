using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class StaffListForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        NumberFormatInfo nfi1;
        StaffListManager StaffListManager;

        public StaffListForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            StaffListManager = new StaffListManager();

            while (!SplashForm.bCreated) ;
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

        private void AdminResponsibilitiesForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void StaffListForm_Load(object sender, EventArgs e)
        {
            nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 2
            };
            dgvStaffListSettings();
            dgvStaffListGroupByFullNameSettings();

            cbFactory.DataSource = StaffListManager.FactoryBS;
            cbFactory.DisplayMember = "FactoryName";
            cbFactory.ValueMember = "FactoryID";

            cbDepartments.DataSource = StaffListManager.DepartmentsBS;
            cbDepartments.DisplayMember = "DepartmentName";
            cbDepartments.ValueMember = "DepartmentID";

            FilterStaffList();
        }

        private void dgvStaffListGroupByFullNameSettings()
        {
            dgvStaffListGroupByFullName.DataSource = StaffListManager.StaffListGroupByFullNameBS;
            dgvStaffListGroupByFullName.Columns.Add(StaffListManager.DepartmentNameColumn);
            dgvStaffListGroupByFullName.Columns.Add(StaffListManager.PositionColumn);
            if (dgvStaffListGroupByFullName.Columns.Contains("StaffListID"))
                dgvStaffListGroupByFullName.Columns["StaffListID"].Visible = false;
            if (dgvStaffListGroupByFullName.Columns.Contains("FactoryID"))
                dgvStaffListGroupByFullName.Columns["FactoryID"].Visible = false;
            if (dgvStaffListGroupByFullName.Columns.Contains("DepartmentID"))
                dgvStaffListGroupByFullName.Columns["DepartmentID"].Visible = false;
            if (dgvStaffListGroupByFullName.Columns.Contains("PositionID"))
                dgvStaffListGroupByFullName.Columns["PositionID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvStaffListGroupByFullName.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }
            dgvStaffListGroupByFullName.Columns["TariffRateFirstRank"].HeaderText = "ТСт1";
            dgvStaffListGroupByFullName.Columns["IncreasingContract"].HeaderText = "ПК";
            dgvStaffListGroupByFullName.Columns["IndividualIncrease"].HeaderText = "ИП";
            dgvStaffListGroupByFullName.Columns["IncreaseCoefCategorization"].HeaderText = "ПДК";
            dgvStaffListGroupByFullName.Columns["TechnologyIncrease"].HeaderText = "ПТВР";
            dgvStaffListGroupByFullName.Columns["SurchargeContract"].HeaderText = "НК";
            dgvStaffListGroupByFullName.Columns["Salary"].HeaderText = "Оклад";
            dgvStaffListGroupByFullName.Columns["Rank"].HeaderText = "Разряд";
            dgvStaffListGroupByFullName.Columns["TariffCoef"].HeaderText = "ТК";
            dgvStaffListGroupByFullName.Columns["Rate"].HeaderText = "Ст";
            int DisplayIndex = 0;
            dgvStaffListGroupByFullName.Columns["DepartmentName"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["Position"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["Rank"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["TariffCoef"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["Rate"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["TariffRateFirstRank"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["IncreasingContract"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["IndividualIncrease"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["IncreaseCoefCategorization"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["TechnologyIncrease"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["SurchargeContract"].DisplayIndex = DisplayIndex++;
            dgvStaffListGroupByFullName.Columns["Salary"].DisplayIndex = DisplayIndex++;
        }

        private void dgvStaffListSettings()
        {
            dgvStaffList.DataSource = StaffListManager.StaffListBS;
            dgvStaffList.Columns.Add(StaffListManager.DepartmentNameColumn);
            dgvStaffList.Columns.Add(StaffListManager.PositionColumn);
            dgvStaffList.Columns.Add(StaffListManager.UserNameColumn);
            dgvStaffList.Columns.Add(StaffListManager.RanksColumn);
            dgvStaffList.Columns.Add(StaffListManager.TariffCoefsColumn);
            dgvStaffList.Columns.Add(StaffListManager.RatesColumn);
            if (dgvStaffList.Columns.Contains("BaseSalary"))
                dgvStaffList.Columns["BaseSalary"].Visible = false;
            if (dgvStaffList.Columns.Contains("StaffListID"))
                dgvStaffList.Columns["StaffListID"].Visible = false;
            if (dgvStaffList.Columns.Contains("FactoryID"))
                dgvStaffList.Columns["FactoryID"].Visible = false;
            if (dgvStaffList.Columns.Contains("DepartmentID"))
                dgvStaffList.Columns["DepartmentID"].Visible = false;
            if (dgvStaffList.Columns.Contains("PositionID"))
                dgvStaffList.Columns["PositionID"].Visible = false;
            if (dgvStaffList.Columns.Contains("UserID"))
                dgvStaffList.Columns["UserID"].Visible = false;
            if (dgvStaffList.Columns.Contains("Rank"))
                dgvStaffList.Columns["Rank"].Visible = false;
            if (dgvStaffList.Columns.Contains("TariffCoef"))
                dgvStaffList.Columns["TariffCoef"].Visible = false;
            if (dgvStaffList.Columns.Contains("Rate"))
                dgvStaffList.Columns["Rate"].Visible = false;

            foreach (DataGridViewColumn Column in dgvStaffList.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            dgvStaffList.Columns["TariffRateFirstRank"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["TariffRateFirstRank"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStaffList.Columns["IncreasingContractSum"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["IncreasingContractSum"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStaffList.Columns["IndividualIncreaseSum"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["IndividualIncreaseSum"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStaffList.Columns["IncreaseCoefCategorizationSum"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["IncreaseCoefCategorizationSum"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStaffList.Columns["TechnologyIncreaseSum"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["TechnologyIncreaseSum"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStaffList.Columns["SurchargeContract"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["SurchargeContract"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStaffList.Columns["Salary"].DefaultCellStyle.Format = "C";
            dgvStaffList.Columns["Salary"].DefaultCellStyle.FormatProvider = nfi1;

            dgvStaffList.Columns["TariffRateFirstRank"].HeaderText = "ТСт1";
            dgvStaffList.Columns["IncreasingContract"].HeaderText = "ПК, %";
            dgvStaffList.Columns["IncreasingContractSum"].HeaderText = "ПК, руб";
            dgvStaffList.Columns["IndividualIncrease"].HeaderText = "ИП, %";
            dgvStaffList.Columns["IndividualIncreaseSum"].HeaderText = "ИП, руб";
            dgvStaffList.Columns["IncreaseCoefCategorization"].HeaderText = "ПДК";
            dgvStaffList.Columns["IncreaseCoefCategorizationSum"].HeaderText = "ПДК, руб";
            dgvStaffList.Columns["TechnologyIncrease"].HeaderText = "ПТВР, %";
            dgvStaffList.Columns["TechnologyIncreaseSum"].HeaderText = "ПТВР, руб";
            dgvStaffList.Columns["SurchargeContract"].HeaderText = "НК";
            dgvStaffList.Columns["Salary"].HeaderText = "Оклад";
            int DisplayIndex = 0;
            dgvStaffList.Columns["DepartmentName"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["Name"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["Position"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["RanksColumn"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["TariffCoefsColumn"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["RatesColumn"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["TariffRateFirstRank"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["IncreasingContract"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["IncreasingContractSum"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["IndividualIncrease"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["IndividualIncreaseSum"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["IncreaseCoefCategorization"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["IncreaseCoefCategorizationSum"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["TechnologyIncrease"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["TechnologyIncreaseSum"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["SurchargeContract"].DisplayIndex = DisplayIndex++;
            dgvStaffList.Columns["Salary"].DisplayIndex = DisplayIndex++;

        }

        private void FilterStaffList()
        {
            int DepartmentID = 0;
            int FactoryID = 0;

            if (cbDepartments.SelectedItem != null)
                DepartmentID = Convert.ToInt32(cbDepartments.SelectedValue);
            if (cbFactory.SelectedItem != null)
                FactoryID = Convert.ToInt32(cbFactory.SelectedValue);
            StaffListManager.FilterStaffList(cbGroupByFullName.Checked, DepartmentID, FactoryID);

            decimal Total = 0;
            if (!cbGroupByFullName.Checked)
            {
                for (int i = 0; i < dgvStaffList.Rows.Count; i++)
                {
                    if (dgvStaffList.Rows[i].Cells["Salary"].Value != DBNull.Value)
                        Total += Convert.ToDecimal(dgvStaffList.Rows[i].Cells["Salary"].Value);
                }
            }
            else
            {
                for (int i = 0; i < dgvStaffListGroupByFullName.Rows.Count; i++)
                {
                    if (dgvStaffListGroupByFullName.Rows[i].Cells["Salary"].Value != DBNull.Value)
                        Total += Convert.ToDecimal(dgvStaffListGroupByFullName.Rows[i].Cells["Salary"].Value);
                }
            }
            lbTotal.Text = Total.ToString("C", nfi1) + " руб";
        }

        private void cbFactory_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilterStaffList();
        }

        private void btnSaveStaffList_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvStaffList.Rows.Count; i++)
            {
                if (dgvStaffList.Rows[i].Cells["DepartmentID"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Служба", "Ошибка сохранения");
                    return;
                }
                if (dgvStaffList.Rows[i].Cells["PositionID"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: Должность", "Ошибка сохранения");
                    return;
                }
                if (dgvStaffList.Rows[i].Cells["UserID"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Облицовочный материал. Не введен параметр: ФИО", "Ошибка сохранения");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            StaffListManager.Calculation();
            StaffListManager.SaveStaffList();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
            StaffListManager.UpdateStaffList(cbGroupByFullName.Checked);
        }

        private void cbDepartments_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilterStaffList();
        }

        private void dgvStaffList_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["FactoryID"].Value = Convert.ToInt32(cbFactory.SelectedValue);
        }

        private void cbShowAllFunctions_CheckedChanged(object sender, EventArgs e)
        {
            StaffListManager.UpdateStaffList(cbGroupByFullName.Checked);
            if (cbGroupByFullName.Checked)
            {
                dgvStaffList.Visible = false;
                dgvStaffListGroupByFullName.Visible = true;
            }
            else
            {
                dgvStaffList.Visible = true;
                dgvStaffListGroupByFullName.Visible = false;
            }
            btnSaveStaffList.Visible = !cbGroupByFullName.Checked;
            FilterStaffList();
        }

        private void dgvStaffList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string colName = dgvStaffList.Columns[e.ColumnIndex].Name;
        }

        private void btnDeleteStaff_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Вместе с этой будут удалены и связанные с ней обязанности. Продолжить удаление?",
                    "Удаление позиции");
            if (!OKCancel)
                return;

            Thread T =
                new Thread(
                    delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            StaffListManager.DeleteStaffFunctions(Convert.ToInt32((dgvStaffList.SelectedRows[0].Cells["StaffListID"].Value)));
            StaffListManager.DeleteStaff(Convert.ToInt32((dgvStaffList.SelectedRows[0].Cells["StaffListID"].Value)));

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
        }
    }
}
