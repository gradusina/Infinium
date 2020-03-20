using Infinium.Modules.DyeingAssignments;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ScaningDyeingAssignmentsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;
        string BarcodeText = string.Empty;
        LightStartForm LightStartForm;
        Form TopForm = null;

        ScaningDyeingAssignments ScaningDyeingAssignmentsmanager;
        RegistrationDyeingWorkMan RegistrationDyeingWorkM;
        RegistrationDyeingWorkWoman RegistrationDyeingWorkW;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public ScaningDyeingAssignmentsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void ScaningDyeingAssignmentsForm_Shown(object sender, EventArgs e)
        {
            panel2.BringToFront();
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

        private void Initialize()
        {
            RegistrationDyeingWorkW = new RegistrationDyeingWorkWoman();
            RegistrationDyeingWorkW.Initialize();

            RegistrationDyeingWorkM = new RegistrationDyeingWorkMan();
            RegistrationDyeingWorkM.Initialize();

            ScaningDyeingAssignmentsmanager = new ScaningDyeingAssignments();
            ScaningDyeingAssignmentsmanager.Initialize();
            ScaningDyeingAssignmentsmanager.ClearTables();
            GetOperationInfo();

            cbxMonths.DataSource = RegistrationDyeingWorkW.MonthsList;
            cbxMonths.ValueMember = "MonthID";
            cbxMonths.DisplayMember = "MonthName";

            cbxYears.DataSource = RegistrationDyeingWorkW.YearsList;
            cbxYears.ValueMember = "YearID";
            cbxYears.DisplayMember = "YearName";

            cbxMonths.SelectedValue = DateTime.Now.Month;
            cbxYears.SelectedValue = DateTime.Now.Year;

            dgvWorkDays.DataSource = RegistrationDyeingWorkM.UsersTimeWorkList;
            dgvWorkDaysSetting();

            DateTime DayStartDateTime = mcWorkDays.SelectionEnd;
            //RegistrationDyeingWorkW.UpdateWorkDays(DayStartDateTime);
            //RegistrationDyeingWorkW.FillUsersTimeWorkTable();

            RegistrationDyeingWorkM.UpdateWorkDays(DayStartDateTime);
            RegistrationDyeingWorkM.FillUsersTimeWorkTable();

            label6.Visible = false;
            label9.Visible = false;
            label2.Visible = false;
            label10.Visible = false;
            label3.Visible = false;
            label11.Visible = false;
            label4.Visible = false;
            label13.Visible = false;
            label5.Visible = false;
            label12.Visible = false;
            label16.Visible = false;
            label18.Visible = false;
            label7.Visible = false;
            label14.Visible = false;
            label8.Visible = false;
            label15.Visible = false;
            label17.Visible = false;
            label19.Visible = false;

        }

        private void dgvWorkDaysSetting()
        {
            foreach (DataGridViewColumn Column in dgvWorkDays.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }

            dgvWorkDays.AutoGenerateColumns = false;

            dgvWorkDays.Columns["UserID"].Visible = false;

            dgvWorkDays.Columns["TimeWork"].ReadOnly = false;
            //dgvWorkDays.Columns["ShortName"].HeaderText = "Работник";
            //dgvWorkDays.Columns["TimeWork"].HeaderText = "Отработано, ч";

            dgvWorkDays.Columns["ShortName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvWorkDays.Columns["ShortName"].MinimumWidth = 200;
            dgvWorkDays.Columns["TimeWork"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvWorkDays.Columns["TimeWork"].MinimumWidth = 30;
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

        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                pbCheckStatus.Visible = false;
                lbBarcode.Text = string.Empty;
                lbInfo.Text = string.Empty;
                lbInfo.ForeColor = Color.FromArgb(121, 177, 229);
                if (tbBarcode.Text.Length < 12)
                {
                    label6.Visible = false;
                    label9.Visible = false;
                    label2.Visible = false;
                    label10.Visible = false;
                    label3.Visible = false;
                    label11.Visible = false;
                    label4.Visible = false;
                    label13.Visible = false;
                    label5.Visible = false;
                    label12.Visible = false;
                    label16.Visible = false;
                    label18.Visible = false;
                    label7.Visible = false;
                    label14.Visible = false;
                    label8.Visible = false;
                    label15.Visible = false;
                    label17.Visible = false;
                    label19.Visible = false;

                    lbInfo.Text = "Неверный штрихкод.";
                    pbCheckStatus.Visible = true;
                    pbCheckStatus.Image = Properties.Resources.cancel;
                    lbInfo.ForeColor = Color.FromArgb(240, 0, 0);
                    tbBarcode.Clear();
                    return;
                }

                string Prefix = tbBarcode.Text.Substring(0, 3);
                if (Prefix != "014" && Prefix != "015")
                {
                    label6.Visible = false;
                    label9.Visible = false;
                    label2.Visible = false;
                    label10.Visible = false;
                    label3.Visible = false;
                    label11.Visible = false;
                    label4.Visible = false;
                    label13.Visible = false;
                    label5.Visible = false;
                    label12.Visible = false;
                    label16.Visible = false;
                    label18.Visible = false;
                    label7.Visible = false;
                    label14.Visible = false;
                    label8.Visible = false;
                    label15.Visible = false;
                    label17.Visible = false;
                    label19.Visible = false;

                    lbInfo.Text = "Неверный штрихкод.";
                    pbCheckStatus.Visible = true;
                    pbCheckStatus.Image = Properties.Resources.cancel;
                    lbInfo.ForeColor = Color.FromArgb(240, 0, 0);
                    tbBarcode.Clear();
                    return;
                }

                label6.Visible = true;
                label9.Visible = true;
                label2.Visible = true;
                label10.Visible = true;
                label3.Visible = true;
                label11.Visible = true;
                label4.Visible = true;
                label13.Visible = true;
                label5.Visible = true;
                label12.Visible = true;
                label16.Visible = true;
                label18.Visible = true;
                label7.Visible = true;
                label14.Visible = true;
                label8.Visible = true;
                label15.Visible = true;
                label17.Visible = true;
                label19.Visible = true;

                bool OkScaning = false;
                int BarcodeID = Convert.ToInt32(tbBarcode.Text.Substring(3, 9));
                string Message = string.Empty;

                lbBarcode.Text = tbBarcode.Text;
                if (Prefix == "014")
                {
                    bool HasBarcodes = ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodes(BarcodeID);
                    if (HasBarcodes)
                    {
                        ScaningDyeingAssignmentsmanager.GetDyeingAssignmentInfo(BarcodeID);
                        switch (ScaningDyeingAssignmentsmanager.OperationStatus)
                        {
                            case 0:
                                btnFinishOperation.Visible = false;
                                if (ScaningDyeingAssignmentsmanager.HasUserDyeingAssignmentUsers)
                                {
                                    ScaningDyeingAssignmentsmanager.AddBarcodeID(BarcodeID);
                                    ScaningDyeingAssignmentsmanager.StartOperation(BarcodeID);
                                    ScaningDyeingAssignmentsmanager.SetStatus(BarcodeID, 1);
                                    ScaningDyeingAssignmentsmanager.SaveDyeingAssignmentBarcodeDetails();
                                    ScaningDyeingAssignmentsmanager.SaveDyeingAssignmentBarcodes();
                                    ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodes(BarcodeID);
                                    ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodeDetails(BarcodeID);
                                    ScaningDyeingAssignmentsmanager.GetDyeingAssignmentInfo(BarcodeID);
                                    GetOperationInfo();
                                    ScaningDyeingAssignmentsmanager.ClearTables();
                                    ScaningDyeingAssignmentsmanager.ClearBarcodeDetails();

                                    Message = "Операция начата.";
                                    OkScaning = true;
                                }
                                else
                                {
                                    GetOperationInfo();
                                    Message = "Операция не начата. Отсканируйте бейджик работника.";
                                    OkScaning = true;
                                }
                                break;
                            case 1:
                                ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodeDetails(BarcodeID);
                                GetOperationInfo();
                                ScaningDyeingAssignmentsmanager.ClearTables();
                                ScaningDyeingAssignmentsmanager.ClearBarcodeDetails();

                                btnFinishOperation.Visible = true;
                                Message = "Подтвердите завершение операции.";
                                OkScaning = true;
                                if (BarcodeText == tbBarcode.Text)
                                {
                                    btnFinishOperation_Click(null, null);
                                    Message = "Операция завершена.";
                                }
                                break;
                            case 2:
                                ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodeDetails(BarcodeID);
                                GetOperationInfo();
                                ScaningDyeingAssignmentsmanager.ClearTables();
                                ScaningDyeingAssignmentsmanager.ClearBarcodeDetails();

                                btnFinishOperation.Visible = false;
                                Message = "Операция завершена.";
                                OkScaning = true;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Message = "Такой операции не существует";
                        btnFinishOperation.Visible = false;
                        label9.Text = string.Empty;
                        label10.Text = string.Empty;
                        label11.Text = string.Empty;
                        label13.Text = string.Empty;
                        label12.Text = string.Empty;
                        label18.Text = string.Empty;
                        label14.Text = string.Empty;
                        label15.Text = string.Empty;
                        label19.Text = string.Empty;
                        OkScaning = false;
                    }
                }

                if (Prefix == "015")
                {
                    ScaningDyeingAssignmentsmanager.CreateDyeingAssignmentBarcodeDetail(BarcodeID);
                    GetOperationInfo();
                    btnFinishOperation.Visible = false;
                    Message = "Задание формируется.";
                    label13.Text = ScaningDyeingAssignmentsmanager.DyeingAssignmentUsers();
                    OkScaning = true;
                }

                BarcodeText = tbBarcode.Text;
                lbInfo.Text = Message;
                tbBarcode.Clear();

                if (OkScaning)
                {
                    pbCheckStatus.Visible = true;
                    pbCheckStatus.Image = Properties.Resources.OK;
                    lbBarcode.ForeColor = Color.FromArgb(82, 169, 24);
                }
                else
                {
                    pbCheckStatus.Visible = true;
                    pbCheckStatus.Image = Properties.Resources.cancel;
                    lbBarcode.ForeColor = Color.FromArgb(240, 0, 0);
                }
            }
        }

        private void tbBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                if (tbBarcode.Text.Length >= 12 && e.KeyChar != (char)8)
                    e.Handled = true;
            }
        }

        private void GetOperationInfo()
        {
            if (ScaningDyeingAssignmentsmanager.DyeingAssignmentID > 0)
                label9.Text = ScaningDyeingAssignmentsmanager.BatchName;
            else
                label9.Text = string.Empty;

            if (ScaningDyeingAssignmentsmanager.GroupType == 0)
            {
                label2.Text = "Кухни";
                if (ScaningDyeingAssignmentsmanager.DocNumber.Length > 0)
                    label10.Text = ScaningDyeingAssignmentsmanager.DocNumber;
                else
                    label10.Text = string.Empty;
            }
            else
            {
                label2.Text = "Тележка";
                if (ScaningDyeingAssignmentsmanager.CartNumber > 0)
                    label10.Text = ScaningDyeingAssignmentsmanager.CartNumber.ToString();
                else
                    label10.Text = string.Empty;
            }

            label11.Text = ScaningDyeingAssignmentsmanager.OperationName.ToString();

            if (ScaningDyeingAssignmentsmanager.CartSquare > 0)
                label12.Text = ScaningDyeingAssignmentsmanager.CartSquare.ToString() + " м.кв.";
            else
                label12.Text = string.Empty;

            string DyeingAssignmentUsers = ScaningDyeingAssignmentsmanager.DyeingAssignmentUsers();
            if (DyeingAssignmentUsers.Length > 0)
                label13.Text = DyeingAssignmentUsers;
            else
            {
                label4.Visible = false;
                label13.Visible = false;
            }

            if (ScaningDyeingAssignmentsmanager.StartDateTime != DBNull.Value)
            {
                label7.Visible = true;
                label14.Visible = true;
                label14.Text = Convert.ToDateTime(ScaningDyeingAssignmentsmanager.StartDateTime).ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                label7.Visible = false;
                label14.Visible = false;
            }
            if (ScaningDyeingAssignmentsmanager.FinishDateTime != DBNull.Value)
            {
                label8.Visible = true;
                label15.Visible = true;
                label15.Text = Convert.ToDateTime(ScaningDyeingAssignmentsmanager.FinishDateTime).ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                label8.Visible = false;
                label15.Visible = false;
            }

            if (ScaningDyeingAssignmentsmanager.Norm > 0)
            {
                label16.Visible = true;
                label18.Visible = true;
                label18.Text = ScaningDyeingAssignmentsmanager.Norm.ToString() + " ч";
            }
            else
            {
                label16.Visible = false;
                label18.Visible = false;
            }

            if (ScaningDyeingAssignmentsmanager.StartDateTime != DBNull.Value && ScaningDyeingAssignmentsmanager.FinishDateTime != DBNull.Value)
            {
                label17.Visible = true;
                label19.Visible = true;
                label19.Text = ScaningDyeingAssignmentsmanager.OperationTime.ToString();

            }
            else
            {
                label17.Visible = false;
                label19.Visible = false;
            }
        }

        private void btnFinishOperation_Click(object sender, EventArgs e)
        {
            int BarcodeID = Convert.ToInt32(lbBarcode.Text.Substring(3, 9));
            string Message = string.Empty;
            bool HasBarcodes = ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodes(BarcodeID);

            ScaningDyeingAssignmentsmanager.FinishOperation(BarcodeID);
            ScaningDyeingAssignmentsmanager.SetStatus(BarcodeID, 2);
            ScaningDyeingAssignmentsmanager.SaveDyeingAssignmentBarcodes();
            ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodes(BarcodeID);
            ScaningDyeingAssignmentsmanager.UpdateDyeingAssignmentBarcodeDetails(BarcodeID);
            ScaningDyeingAssignmentsmanager.GetDyeingAssignmentInfo(BarcodeID);

            GetOperationInfo();
            ScaningDyeingAssignmentsmanager.ClearTables();
            ScaningDyeingAssignmentsmanager.ClearBarcodeDetails();

            Message = "Операция завершена.";
            lbInfo.Text = Message;
            pbCheckStatus.Visible = true;
            pbCheckStatus.Image = Properties.Resources.OK;
            lbBarcode.ForeColor = Color.FromArgb(82, 169, 24);
            btnFinishOperation.Visible = false;
        }

        private void tbBarcode_Leave(object sender, EventArgs e)
        {
            CheckTimer.Enabled = true;
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!tbBarcode.Focused)
            {
                tbBarcode.Focus();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (GetActiveWindow() != this.Handle)
            //{
            //    this.Activate();
            //}
        }

        private void btnCreateReport_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание отчета.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            //RegistrationDyeingWorkW.CreateReport(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue));

            RegistrationDyeingWorkM.CreateReport(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue));

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton == cbtnScanBarcodes)
            {
                CheckTimer.Enabled = true;
                panel2.BringToFront();
            }

            if (kryptonCheckSet1.CheckedButton == cbtnWorkDay)
            {
                CheckTimer.Enabled = false;
                panel1.BringToFront();
            }
        }

        private void mcWorkDays_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime DateTime = mcWorkDays.SelectionEnd;
            //RegistrationDyeingWorkW.UpdateWorkDays(DateTime);
            //RegistrationDyeingWorkW.FillUsersTimeWorkTable();

            RegistrationDyeingWorkM.UpdateWorkDays(DateTime);
            RegistrationDyeingWorkM.FillUsersTimeWorkTable();
        }

        private void btnSaveWorkDays_Click(object sender, EventArgs e)
        {
            DateTime DateTime = mcWorkDays.SelectionEnd;
            //RegistrationDyeingWorkW.SaveWorkDays(DateTime);
            //RegistrationDyeingWorkW.UpdateWorkDays(DateTime);
            //RegistrationDyeingWorkW.FillUsersTimeWorkTable();

            RegistrationDyeingWorkM.SaveWorkDays(DateTime);
            RegistrationDyeingWorkM.UpdateWorkDays(DateTime);
            RegistrationDyeingWorkM.FillUsersTimeWorkTable();
        }
    }
}
