using ComponentFactory.Krypton.Toolkit;

using Infinium.Modules.Admin;

using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class WorkTimeRegisterForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        const int iAdminRole = 97;
        LightStartForm LightStartForm;

        Form TopForm = null;

        WorkTimeRegister WorkTimeRegister;
        WorkTimeRegister.DayStatus DayStatus;

        RoleTypes RoleType = RoleTypes.OrdinaryRole;
        public enum RoleTypes
        {
            OrdinaryRole = 0,
            AdminRole = 1
        }

        int absenceTypeId = 1;

        //----------------------------------------------
        DateTime Date;
        string Year;

        WorkTimeSheet WorkTimeSheet;
        DataTable DayStartDate;

        ProductionShedule _productionShedule;
        ResultTimesheet resultTimesheet;
        AbsenceJournal _absenceJournal;
        bool NeedRefresh = false;

        public WorkTimeRegisterForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            //Connection = new Connection();
            //ConnectionStrings.CatalogConnectionString = Connection.GetConnectionString(CommonVariables.CatalogConnectionString);
            //ConnectionStrings.LightConnectionString = Connection.GetConnectionString(CommonVariables.LightConnectionString);
            //ConnectionStrings.MarketingOrdersConnectionString = Connection.GetConnectionString(CommonVariables.MarketingOrdersConnectionString);
            //ConnectionStrings.MarketingReferenceConnectionString = Connection.GetConnectionString(CommonVariables.MarketingReferenceConnectionString);
            //ConnectionStrings.StorageConnectionString = Connection.GetConnectionString(CommonVariables.StorageConnectionString);
            //ConnectionStrings.UsersConnectionString = Connection.GetConnectionString(CommonVariables.UsersConnectionString);
            //ConnectionStrings.ZOVOrdersConnectionString = Connection.GetConnectionString(CommonVariables.ZOVOrdersConnectionString);
            //ConnectionStrings.ZOVReferenceConnectionString = Connection.GetConnectionString(CommonVariables.ZOVReferenceConnectionString);

            //Security = new Infinium.Security();
            //if (!Security.Initialize())
            //{
            //    MessageBox.Show("Не удалось подключится к базе данных. Возможные причины: не работает сервер баз данных, нет доступа к сети или интернет. Обратитесь к системному администратору");
            //    this.Close();
            //    Application.Exit();
            //    return;
            //}

            //Security.Enter(322, "gradus");

            WorkTimeRegister = new WorkTimeRegister();
            resultTimesheet = new ResultTimesheet();

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        public void SetOverduedColor()
        {
            for (int i = 0; i < WorkDaysGrid.Rows.Count; i++)
            {
                if (WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Style.BackColor = Color.FromArgb(85, 200, 85);
                        WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Style.BackColor = Color.FromArgb(85, 200, 85);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Style.BackColor = Color.White;
                }

                if (WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Style.BackColor = Color.FromArgb(222, 222, 65);
                        WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Style.BackColor = Color.FromArgb(222, 222, 65);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Style.BackColor = Color.White;
                }

                if (WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Value != DBNull.Value &&
                    WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Style.BackColor = Color.FromArgb(75, 120, 210);
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Style.BackColor = Color.FromArgb(75, 120, 210);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Style.BackColor = Color.White;
                }

                if (WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Value != DBNull.Value &&
                    WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Style.BackColor = Color.FromArgb(85, 190, 190);
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Style.BackColor = Color.FromArgb(85, 190, 190);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Style.BackColor = Color.White;
                }
            }
        }
        private void WorkTimeRegisterForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            SetOverduedColor();
            WorkDaysGrid.Focus();

            //----------------------------------------------
            WorkTimeSheet = new WorkTimeSheet();
            _productionShedule = new ProductionShedule();

            _absenceJournal = new AbsenceJournal();

            ProdSheduleDataGrid.DataSource = _productionShedule.HoursBindingSource;
            ProdSheduleDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (_productionShedule.PermissionGranted(Security.CurrentUserID, this.Name, iAdminRole))
                RoleType = RoleTypes.AdminRole;

            for (int i = 1; i < DateTime.DaysInMonth(2020, 1) + 1; i++)
            {
                ProdSheduleDataGrid.Columns[i.ToString()].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                ProdSheduleDataGrid.Columns[i.ToString()].Width = 50;
                ProdSheduleDataGrid.Columns[i.ToString()].ReadOnly = true;
                if (RoleType == RoleTypes.AdminRole)
                    ProdSheduleDataGrid.Columns[i.ToString()].ReadOnly = false;
            }

            ProdSheduleDataGrid.Columns["MonthName"].HeaderText = "Дата";
            ProdSheduleDataGrid.Columns["MonthName"].ReadOnly = true;
            ProdSheduleDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ProdSheduleDataGrid.Columns["MonthName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ProdSheduleDataGrid.Columns["MonthName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            absencesDataGrid.DataSource = _absenceJournal.AbsencesJournalBindingSource;
            absencesDataGrid.Columns.Add(_absenceJournal.PositionColumn);
            absencesDataGrid.Columns.Add(_absenceJournal.UserColumn);
            absencesDataGrid.Columns.Add(_absenceJournal.DateStartColumn);
            absencesDataGrid.Columns.Add(_absenceJournal.DateFinishColumn);

            absencesDataGrid.Columns["AbsenceID"].Visible = false;
            absencesDataGrid.Columns["PositionID"].Visible = false;
            absencesDataGrid.Columns["UserID"].Visible = false;
            absencesDataGrid.Columns["DateStart"].Visible = false;
            absencesDataGrid.Columns["DateFinish"].Visible = false;
            absencesDataGrid.Columns["AbsenceTypeID"].Visible = false;

            absencesDataGrid.Columns["Rate"].HeaderText = "Ставка";
            absencesDataGrid.Columns["Hours"].HeaderText = "Часы";

            absencesDataGrid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            absencesDataGrid.Columns["UserColumn"].DisplayIndex = DisplayIndex++;
            absencesDataGrid.Columns["PositionColumn"].DisplayIndex = DisplayIndex++;
            absencesDataGrid.Columns["Rate"].DisplayIndex = DisplayIndex++;
            absencesDataGrid.Columns["DateStartColumn"].DisplayIndex = DisplayIndex++;
            absencesDataGrid.Columns["DateFinishColumn"].DisplayIndex = DisplayIndex++;
            absencesDataGrid.Columns["Hours"].DisplayIndex = DisplayIndex++;

            absencesDataGrid.Columns["Rate"].ReadOnly = true;
            DayStartDate = WorkTimeSheet.DayStartDate();

            for (int i = 0; i < DayStartDate.Rows.Count; i++)
            {
                Date = (DateTime)DayStartDate.Rows[i]["DayStartDateTime"];
                Year = Date.ToString("yyyy");

                if (YearComboBox.Items.Count == 0 | YearComboBox.Items.IndexOf(Year) == -1)
                {
                    YearComboBox.Items.Add(Year);
                    YearComboBox1.Items.Add(Year);
                    YearComboBox2.Items.Add(Year);
                }
            }
            YearComboBox.Text = YearComboBox.Items[YearComboBox.Items.Count - 1].ToString();
            YearComboBox1.Text = YearComboBox1.Items[YearComboBox1.Items.Count - 1].ToString();
            YearComboBox2.Text = YearComboBox2.Items[YearComboBox2.Items.Count - 1].ToString();

            _productionShedule.GetShedule(YearComboBox1.SelectedItem.ToString());
            _productionShedule.FillHoursDataTable();

            AbsenceTypesRadioButtons_CheckedChanged(null, null);
            NeedRefresh = true;
            //----------------------------------------------
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
            WorkDaysGrid.DataSource = WorkTimeRegister.WorkDaysBindingSource;
            GridSettings();

            DateTime D = WorkDateTimePicker.Value;
            StatusToControls(D);
            DayLengthLabel.Text = GetDayLength();
        }

        private void GridSettings()
        {
            WorkDaysGrid.AutoGenerateColumns = false;
            foreach (DataGridViewColumn Column in WorkDaysGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            WorkDaysGrid.Columns["WorkDayID"].Visible = false;
            WorkDaysGrid.Columns["UserID"].Visible = false;
            //WorkDaysGrid.Columns["DayStartDateTime"].Visible = false;
            //WorkDaysGrid.Columns["DayEndDateTime"].Visible = false;
            //WorkDaysGrid.Columns["DayBreakStartDateTime"].Visible = false;
            //WorkDaysGrid.Columns["DayBreakEndDateTime"].Visible = false;
            //WorkDaysGrid.Columns["DayBreakStartFactDateTime"].Visible = false;
            //WorkDaysGrid.Columns["DayBreakEndFactDateTime"].Visible = false;

            WorkDaysGrid.Columns["DayStartNotes"].Visible = false;
            WorkDaysGrid.Columns["DayBreakStartNotes"].Visible = false;
            WorkDaysGrid.Columns["DayContinueNotes"].Visible = false;
            WorkDaysGrid.Columns["DayEndNotes"].Visible = false;
            WorkDaysGrid.Columns["Saved"].Visible = false;

            WorkDaysGrid.Columns["Name"].HeaderText = "Сотрудник";
            WorkDaysGrid.Columns["FactHours"].HeaderText = "Фактически";
            WorkDaysGrid.Columns["TimesheetHours"].HeaderText = "В табель";
            WorkDaysGrid.Columns["DayStartDateTime"].HeaderText = "Начало";
            WorkDaysGrid.Columns["DayEndDateTime"].HeaderText = "Завершение";
            WorkDaysGrid.Columns["DayBreakStartDateTime"].HeaderText = "Начало\r\nперерыва";
            WorkDaysGrid.Columns["DayBreakEndDateTime"].HeaderText = "Завершение\r\nперерыва";
            WorkDaysGrid.Columns["DayStartFactDateTime"].HeaderText = "Начало\r\n(фактическое)";
            WorkDaysGrid.Columns["DayEndFactDateTime"].HeaderText = "Завершение\r\n(фактическое)";
            WorkDaysGrid.Columns["DayBreakStartFactDateTime"].HeaderText = "Начало перерыва\r\n(фактическое)";
            WorkDaysGrid.Columns["DayBreakEndFactDateTime"].HeaderText = "Завершение перерыва\r\n(фактическое)";

            WorkDaysGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WorkDaysGrid.Columns["Name"].MinimumWidth = 290;
            WorkDaysGrid.Columns["TimesheetHours"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["TimesheetHours"].Width = 100;
            WorkDaysGrid.Columns["DayStartDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayStartDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayStartFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayStartFactDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayEndDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayEndDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayEndFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayEndFactDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayBreakStartDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayBreakStartDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayBreakStartFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayBreakStartFactDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayBreakEndDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayBreakEndDateTime"].Width = 140;
            WorkDaysGrid.Columns["DayBreakEndFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["DayBreakEndFactDateTime"].Width = 140;
            WorkDaysGrid.Columns["FactHours"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            WorkDaysGrid.Columns["FactHours"].Width = 100;

            int DisplayIndex = 0;
            WorkDaysGrid.Columns["Name"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayStartDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayStartFactDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayBreakStartDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayBreakStartFactDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayBreakEndDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayBreakEndFactDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayEndDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["DayEndFactDateTime"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["FactHours"].DisplayIndex = DisplayIndex++;
            WorkDaysGrid.Columns["TimesheetHours"].DisplayIndex = DisplayIndex++;
        }

        private string MinToHHmm(int Minutes)
        {
            return Convert.ToInt32(Minutes / 60).ToString() + " : " + (Minutes - Convert.ToInt32(Minutes / 60) * 60).ToString();
        }

        private string GetDayLength()
        {
            if (DayStatus.iDayStatus == 0)//day not started
                return "-- : --";

            string L = "";
            int Minutes = 0;

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayStarted)
            {
                Minutes = (Convert.ToInt32((Security.GetCurrentDate() - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes));
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayEnded || DayStatus.iDayStatus == WorkTimeRegister.sDaySaved)
            {
                int Break = 0;

                if (DayStatus.bBreak)
                    Break = (Convert.ToInt32((Convert.ToDateTime(DayStatus.BreakEnded) - Convert.ToDateTime(DayStatus.BreakStarted)).TotalMinutes));

                Minutes = (Convert.ToInt32((Convert.ToDateTime(DayStatus.DayEnded) - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes) - Break);
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sBreakStarted)
                Minutes = Convert.ToInt32((Convert.ToDateTime(DayStatus.BreakStarted) - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes);

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayContinued)
            {
                int Break = 0;

                Break = (Convert.ToInt32((Convert.ToDateTime(DayStatus.BreakEnded) - Convert.ToDateTime(DayStatus.BreakStarted)).TotalMinutes));

                Minutes = (Convert.ToInt32((Security.GetCurrentDate() - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes) - Break);
            }

            L = MinToHHmm(Minutes);

            string res = "";

            int c = 0;

            for (int i = 0; i < L.Length; i++)
            {
                if (L[i] != ':')
                    res += L[i];
                else
                {
                    if (c == 0)
                    {
                        res += " ч : ";
                        c++;
                    }
                    else
                        break;
                }
            }

            return res + " м";
        }

        private void StatusToControls(DateTime D)
        {
            DayStatus = WorkTimeRegister.GetDayStatus(D);

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayNotStarted)
            {
                DayLengthLabel.Text = "-- : --";
                DayStartLabel.Text = "-- : --";
                BreakStartLabel.Text = "-- : --";
                BreakEndLabel.Text = "-- : --";
                DayEndLabel.Text = "-- : --";
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayStarted)
                StatusLabel.Text = "Рабочий день начат";

            if (DayStatus.iDayStatus == WorkTimeRegister.sBreakStarted)
                StatusLabel.Text = "Перерыв";

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayContinued)
                StatusLabel.Text = "Рабочий день продолжается";

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayEnded)
                StatusLabel.Text = "Рабочий день завершен";

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayNotStarted)
                StatusLabel.Text = "Рабочий день не начат";

            if (DayStatus.iDayStatus == WorkTimeRegister.sDaySaved)
                StatusLabel.Text = "Рабочий день сохранен";

            if (DayStatus.iDayStatus != WorkTimeRegister.sDayEnded)
            {
                DayLengthLabel.Text = "-- : --";
                DayStartLabel.Text = "-- : --";
                BreakStartLabel.Text = "-- : --";
                BreakEndLabel.Text = "-- : --";
                DayEndLabel.Text = "-- : --";
            }



            if (DayStatus.iDayStatus == WorkTimeRegister.sDaySaved)
            {
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
                if (DayStatus.bBreak)
                {
                    BreakStartLabel.Text = DayStatus.BreakStarted.ToString("HH:mm");
                    BreakEndLabel.Text = DayStatus.BreakEnded.ToString("HH:mm");
                }
                else
                {
                    BreakStartLabel.Text = "-- : --";
                    BreakEndLabel.Text = "-- : --";
                }

                DayEndLabel.Text = DayStatus.DayEnded.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayStarted)
            {
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sBreakStarted)
            {
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
                BreakStartLabel.Text = DayStatus.BreakStarted.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayContinued)
            {
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
                BreakStartLabel.Text = DayStatus.BreakStarted.ToString("HH:mm");
                BreakEndLabel.Text = DayStatus.BreakEnded.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == WorkTimeRegister.sDayEnded)
            {
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
                BreakStartLabel.Text = DayStatus.BreakStarted.ToString("HH:mm");
                BreakEndLabel.Text = DayStatus.BreakEnded.ToString("HH:mm");
                DayEndLabel.Text = DayStatus.DayEnded.ToString("HH:mm");
            }

            DayLengthLabel.Text = GetDayLength();
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

        private void DayTimer_Tick(object sender, EventArgs e)
        {
            if (DayStatus.iDayStatus == WorkTimeRegister.sBreakStarted ||
                DayStatus.iDayStatus == WorkTimeRegister.sDayEnded || DayStatus.iDayStatus == WorkTimeRegister.sDaySaved)
                return;

            DayLengthLabel.Text = GetDayLength();
        }

        private void WorkDaysGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (WorkTimeRegister == null || WorkTimeRegister.WorkDaysBindingSource.Count < 1)
                return;

            WorkTimeRegister.UserID = Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["UserID"]);
            WorkTimeRegister.WorkDayID = Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["WorkDayID"]);
            DateTime D = WorkDateTimePicker.Value;
            StatusToControls(D);
            CreateNotes();
            SetOverduedColor();
        }

        private void WorkDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);
            CreateNotes();
            SetOverduedColor();
        }

        private void CreateNotes()
        {
            Font fDateFont = new Font("Segoe UI", 14.0f, FontStyle.Bold);
            Font fNotesFont = new Font("Segoe UI", 14.0f, FontStyle.Regular);
            Font fTextFont = new Font("Segoe UI", 14.0f, FontStyle.Italic);

            Color cNotesColor = Color.Black;
            Color cTextFontColor = Color.Gray;

            DateTime D = WorkDateTimePicker.Value;

            string DayStartNotes = WorkTimeRegister.GetDayStartNotes;
            string DayBreakStartNotes = WorkTimeRegister.GetDayBreakStartNotes;
            string DayContinueNotes = WorkTimeRegister.GetDayContinueNotes;
            string DayEndNotes = WorkTimeRegister.GetDayEndNotes;

            //string str = WorkTimeRegister.GetDayBreakStartDateTime;

            NotesRichTextBox.Clear();
            NotesRichTextBox.SelectionStart = NotesRichTextBox.TextLength;
            NotesRichTextBox.SelectionLength = 0;

            if (DayStartNotes.Length > 0)
            {
                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.SelectionColor = cNotesColor;

                string notes = "";
                if (NotesRichTextBox.TextLength > 0)
                    notes += "\n\n";
                notes += "Начало рабочего дня было перенесено с ";
                NotesRichTextBox.AppendText(notes);

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayStartFactDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" на ");

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayStartDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" по причине:\n");

                NotesRichTextBox.SelectionFont = fTextFont;
                NotesRichTextBox.SelectionColor = cTextFontColor;
                NotesRichTextBox.AppendText(DayStartNotes);
            }

            if (DayBreakStartNotes.Length > 0)
            {
                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.SelectionColor = cNotesColor;

                string notes = "";
                if (NotesRichTextBox.TextLength > 0)
                    notes += "\n\n";
                notes += "Начало перерыва было перенесено с ";
                NotesRichTextBox.AppendText(notes);

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayBreakStartFactDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" на ");

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayBreakStartDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" по причине:\n");

                NotesRichTextBox.SelectionFont = fTextFont;
                NotesRichTextBox.SelectionColor = cTextFontColor;
                NotesRichTextBox.AppendText(DayBreakStartNotes);
            }

            if (DayContinueNotes.Length > 0)
            {
                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.SelectionColor = cNotesColor;

                string notes = "";
                if (NotesRichTextBox.TextLength > 0)
                    notes += "\n\n";
                notes += "Продложение рабочего дня было перенесено с ";
                NotesRichTextBox.AppendText(notes);

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayBreakEndFactDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" на ");

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayBreakEndDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" по причине:\n");

                NotesRichTextBox.SelectionFont = fTextFont;
                NotesRichTextBox.SelectionColor = cTextFontColor;
                NotesRichTextBox.AppendText(DayContinueNotes);
            }

            if (DayEndNotes.Length > 0)
            {
                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.SelectionColor = cNotesColor;

                string notes = "";
                if (NotesRichTextBox.TextLength > 0)
                    notes += "\n\n";
                notes += "Завершение рабочего дня было перенесено с ";
                NotesRichTextBox.AppendText(notes);

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayEndFactDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" на ");

                NotesRichTextBox.SelectionFont = fDateFont;
                NotesRichTextBox.AppendText(WorkTimeRegister.GetDayEndDateTime);

                NotesRichTextBox.SelectionFont = fNotesFont;
                NotesRichTextBox.AppendText(" по причине:\n");

                NotesRichTextBox.SelectionFont = fTextFont;
                NotesRichTextBox.SelectionColor = cTextFontColor;
                NotesRichTextBox.AppendText(DayEndNotes);
            }
        }

        //----------------------------------------------
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            WorkTimeSheet.GetTimeSheet(TimeSheetDataGrid, YearComboBox.SelectedItem.ToString(), MonthComboBox.SelectedItem.ToString());
            int monthInt = Convert.ToDateTime(MonthComboBox.SelectedItem.ToString() + " " + YearComboBox.SelectedItem.ToString()).Month;
            int yearInt = int.Parse(YearComboBox.SelectedItem.ToString());


            resultTimesheet.CreateUsersList(yearInt, monthInt, DateTime.Now);
            TimesheetReport timesheetReport = new TimesheetReport();
            timesheetReport.CreateReport(yearInt, monthInt, resultTimesheet);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

        }

        private void YearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonthComboBox.Items.Clear();
            MonthComboBox.Items.AddRange(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" });
            //ComboBox Month_mass = new ComboBox();
            //Month_mass.Items.AddRange(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" });

            //for (int i = 0; i < DayStartDate.Rows.Count; i++)
            //{
            //    Date = (DateTime)DayStartDate.Rows[i]["DayStartDateTime"];
            //    Year = Date.ToString("yyyy");
            //    Month = Date.ToString("MMMM");

            //    if (Year == YearComboBox.SelectedItem.ToString() && Month_mass.Items.IndexOf(Month) != -1)
            //        Month_mass.Items.Remove(Month);
            //}
            //for (int i = 0; i < Month_mass.Items.Count; i++)
            //{
            //    if (MonthComboBox.Items.IndexOf(Month_mass.Items[i]) != -1)
            //        MonthComboBox.Items.Remove(Month_mass.Items[i]);
            //}
            //Month_mass.Dispose();
            if (MonthComboBox.Items.Count > 0)
                MonthComboBox.Text = MonthComboBox.Items[0].ToString();
        }

        private void YearComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonthComboBox2.Items.Clear();
            MonthComboBox2.Items.AddRange(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" });
            //ComboBox Month_mass = new ComboBox();
            //Month_mass.Items.AddRange(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" });

            //for (int i = 0; i < DayStartDate.Rows.Count; i++)
            //{
            //    Date = (DateTime)DayStartDate.Rows[i]["DayStartDateTime"];
            //    Year = Date.ToString("yyyy");
            //    Month = Date.ToString("MMMM");

            //    if (Year == YearComboBox2.SelectedItem.ToString() && Month_mass.Items.IndexOf(Month) != -1)
            //        Month_mass.Items.Remove(Month);
            //}
            //for (int i = 0; i < Month_mass.Items.Count; i++)
            //{
            //    if (MonthComboBox2.Items.IndexOf(Month_mass.Items[i]) != -1)
            //        MonthComboBox2.Items.Remove(Month_mass.Items[i]);
            //}
            //Month_mass.Dispose();
            if (MonthComboBox2.Items.Count > 0)
                MonthComboBox2.Text = MonthComboBox2.Items[0].ToString();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            //Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            //T.Start();

            //while (!SplashWindow.bSmallCreated) ;

            ////int monthInt = Convert.ToDateTime(MonthComboBox.SelectedItem.ToString() + " " + YearComboBox.SelectedItem.ToString()).Month;
            ////int yearInt = int.Parse(YearComboBox.SelectedItem.ToString());

            //if (TimeSheetDataGrid.ColumnCount != 0)
            //    WorkTimeSheet.ExportToExcel(TimeSheetDataGrid);
            ////resultTimesheet.CreateUsersList(yearInt, monthInt, DateTime.Now);

            //while (SplashWindow.bSmallCreated)
            //    SmallWaitForm.CloseS = true;
        }

        private void WorkDaysGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            //if (((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayStartDateTime"] == DBNull.Value)
            //    return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm1 EditTimeForm = new EditTimeForm1(ref TopForm, DateCorrectForm.dStartDay,
                Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["WorkDayID"]),
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayStartDateTime"],
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayStartDateTime"]);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);
            CreateNotes();
            SetOverduedColor();
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            //if (((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayEndDateTime"] == DBNull.Value)
            //    return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm1 EditTimeForm = new EditTimeForm1(ref TopForm, DateCorrectForm.dEndDay,
                Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["WorkDayID"]),
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayEndDateTime"],
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayStartDateTime"]);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);
            CreateNotes();
            SetOverduedColor();
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            //if (((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayBreakStartDateTime"] == DBNull.Value)
            //    return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm1 EditTimeForm = new EditTimeForm1(ref TopForm, DateCorrectForm.dBreakDay,
                Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["WorkDayID"]),
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayBreakStartDateTime"],
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayStartDateTime"]);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);
            CreateNotes();
            SetOverduedColor();
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            //if (((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayBreakEndDateTime"] == DBNull.Value)
            //    return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm1 EditTimeForm = new EditTimeForm1(ref TopForm, DateCorrectForm.dContinueDay,
                Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["WorkDayID"]),
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayBreakEndDateTime"],
                ((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["DayStartDateTime"]);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);
            CreateNotes();
            SetOverduedColor();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            _productionShedule.GetShedule(YearComboBox1.SelectedItem.ToString());
            _productionShedule.FillHoursDataTable();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Данные обновлены", 1700);
        }

        private void btnSaveShedule_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            _productionShedule.FillSourceDataTable(YearComboBox1.SelectedItem.ToString());
            _productionShedule.SaveShedule();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Календарь сохраненён", 1700);
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Подождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            _absenceJournal.GetJournal(YearComboBox2.SelectedItem.ToString(), MonthComboBox2.SelectedItem.ToString());
            AbsenceTypesRadioButtons_CheckedChanged(null, null);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Данные обновлены", 1700);
        }

        private void AbsenceTypesRadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            KryptonRadioButton radioButton = (KryptonRadioButton)sender;
            if (radioButton != null && radioButton.Tag != null && radioButton.Tag != DBNull.Value)
                absenceTypeId = Convert.ToInt32(radioButton.Tag);

            _absenceJournal.FilterAbsenceJournal(absenceTypeId);

            //if (kryptonRadioButton0.Checked)
            //    absenceTypeId = 1;
            //else if (kryptonRadioButton1.Checked)
            //    absenceTypeId = 2;
            //else if (kryptonRadioButton2.Checked)
            //    absenceTypeId = 3;
            //else if (kryptonRadioButton3.Checked)
            //    absenceTypeId = 4;
            //else if (kryptonRadioButton4.Checked)
            //    absenceTypeId = 5;
            //else if (kryptonRadioButton5.Checked)
            //    absenceTypeId = 6;
            //else if (kryptonRadioButton6.Checked)
            //    absenceTypeId = 7;
            //else if (kryptonRadioButton7.Checked)
            //    absenceTypeId = 8;
            //else if (kryptonRadioButton8.Checked)
            //    absenceTypeId = 9;
            //else if (kryptonRadioButton9.Checked)
            //    absenceTypeId = 10;
            //else if (kryptonRadioButton10.Checked)
            //    absenceTypeId = 11;
            //else if (kryptonRadioButton11.Checked)
            //    absenceTypeId = 12;
            //else if (kryptonRadioButton12.Checked)
            //    absenceTypeId = 13;
            //else if (kryptonRadioButton13.Checked)
            //    absenceTypeId = 14;
        }

        private void absencesDataGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["AbsenceTypeID"].Value = absenceTypeId;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            if (!_absenceJournal.CheckCorrectData)
            {
                MessageBox.Show("Введены не все даты", "Ошибка сохранения", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            _absenceJournal.SaveJournal();
            _absenceJournal.GetJournal(YearComboBox2.SelectedItem.ToString(), MonthComboBox2.SelectedItem.ToString());
            AbsenceTypesRadioButtons_CheckedChanged(null, null);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Журнал сохраненён", 1700);
        }

        private void MonthComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NeedRefresh == true)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Подождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                _absenceJournal.GetJournal(YearComboBox2.SelectedItem.ToString(), MonthComboBox2.SelectedItem.ToString());
                AbsenceTypesRadioButtons_CheckedChanged(null, null);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Данные обновлены", 1700);
            }
            else
            {
                _absenceJournal.GetJournal(YearComboBox2.SelectedItem.ToString(), MonthComboBox2.SelectedItem.ToString());
                AbsenceTypesRadioButtons_CheckedChanged(null, null);
            }
        }

        private void absencesDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                absencesDataGrid.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu8.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem12_Click(object sender, EventArgs e)
        {
            int AbsenceID = -1;
            if (absencesDataGrid.SelectedCells.Count > 0 && absencesDataGrid.CurrentRow.Cells["AbsenceID"].Value != DBNull.Value)
            {
                AbsenceID = Convert.ToInt32(absencesDataGrid.CurrentRow.Cells["AbsenceID"].Value);
                _absenceJournal.RemoveAbsenceRecord(AbsenceID);
            }
            else
            {
                int Index = absencesDataGrid.CurrentRow.Index;

                absencesDataGrid.Rows.RemoveAt(Index);
            }
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimesheetForm editTimesheetForm = new EditTimesheetForm(ref TopForm,
                Convert.ToInt32(((DataRowView)WorkTimeRegister.WorkDaysBindingSource.Current).Row["WorkDayID"]));

            TopForm = editTimesheetForm;

            editTimesheetForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);

            CreateNotes();
            SetOverduedColor();
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            int UserID = -1;
            object DateStart = DBNull.Value;
            object DateFinish = DBNull.Value;
            int AbsenceTypeID = -1;
            if (absencesDataGrid.SelectedCells.Count > 0)
            {
                if (absencesDataGrid.CurrentRow.Cells["UserID"].Value != DBNull.Value)
                    UserID = Convert.ToInt32(absencesDataGrid.CurrentRow.Cells["UserID"].Value);
                if (absencesDataGrid.CurrentRow.Cells["DateStart"].Value != DBNull.Value)
                    DateStart = Convert.ToDateTime(absencesDataGrid.CurrentRow.Cells["DateStart"].Value);
                if (absencesDataGrid.CurrentRow.Cells["DateFinish"].Value != DBNull.Value)
                    DateFinish = Convert.ToDateTime(absencesDataGrid.CurrentRow.Cells["DateFinish"].Value);
                if (absencesDataGrid.CurrentRow.Cells["AbsenceTypeID"].Value != DBNull.Value)
                    AbsenceTypeID = Convert.ToInt32(absencesDataGrid.CurrentRow.Cells["AbsenceTypeID"].Value);

                _absenceJournal.CopyAbsenceRecord(UserID, DateStart, DateFinish, AbsenceTypeID);
            }
        }


        //----------------------------------------------
    }
}
