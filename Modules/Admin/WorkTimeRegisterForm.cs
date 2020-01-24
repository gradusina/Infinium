using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Infinium.Modules.Admin;

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

        //----------------------------------------------
        DateTime Date;
        string Year;
        string Month;
        WorkTimeSheet WorkTimeSheet;
        DataTable DayStartDate;

        ProductionShedule _productionShedule;
        //----------------------------------------------

        public WorkTimeRegisterForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            WorkTimeRegister = new WorkTimeRegister(ref WorkDaysGrid);

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        private void WorkTimeRegisterForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;

            WorkTimeRegister.SetOverduedColor();
            WorkDaysGrid.Focus();

            //----------------------------------------------
            WorkTimeSheet = new WorkTimeSheet();
            _productionShedule = new ProductionShedule();

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

            DayStartDate = WorkTimeSheet.DayStartDate();

            for (int i = 0; i < DayStartDate.Rows.Count; i++)
            {
                Date = (DateTime)DayStartDate.Rows[i]["DayStartDateTime"];
                Year = Date.ToString("yyyy");

                if (YearComboBox.Items.Count == 0 | YearComboBox.Items.IndexOf(Year) == -1)
                    YearComboBox.Items.Add(Year);
            }
            YearComboBox.Text = YearComboBox.Items[YearComboBox.Items.Count - 1].ToString();


            YearComboBox1.Items.Add(2019);
            YearComboBox1.Items.Add(2020);
            YearComboBox1.Items.Add(2021);
            YearComboBox1.Text = YearComboBox.Items[YearComboBox.Items.Count - 1].ToString();
            
            _productionShedule.GetCalendar(YearComboBox1.SelectedItem.ToString());
            _productionShedule.FillHoursDataTable();
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
            DateTime D = WorkDateTimePicker.Value;
            StatusToControls(D);
            DayLengthLabel.Text = GetDayLength();
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
            WorkTimeRegister.SetOverduedColor();
        }

        private void WorkDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime D = WorkDateTimePicker.Value;
            WorkTimeRegister.FilterWorkDays(D);
            CreateNotes();
            WorkTimeRegister.SetOverduedColor();
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
            WorkTimeSheet.GetTimeSheet(TimeSheetDataGrid, YearComboBox.SelectedItem.ToString(), MonthComboBox.SelectedItem.ToString());
        }

        private void YearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonthComboBox.Items.Clear();
            MonthComboBox.Items.AddRange(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" });
            ComboBox Month_mass = new ComboBox();
            Month_mass.Items.AddRange(new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" });

            for (int i = 0; i < DayStartDate.Rows.Count; i++)
            {
                Date = (DateTime)DayStartDate.Rows[i]["DayStartDateTime"];
                Year = Date.ToString("yyyy");
                Month = Date.ToString("MMMM");

                if (Year == YearComboBox.SelectedItem.ToString() && Month_mass.Items.IndexOf(Month) != -1)
                    Month_mass.Items.Remove(Month);
            }
            for (int i = 0; i < Month_mass.Items.Count; i++)
            {
                if (MonthComboBox.Items.IndexOf(Month_mass.Items[i]) != -1)
                    MonthComboBox.Items.Remove(Month_mass.Items[i]);
            }
            Month_mass.Dispose();
            MonthComboBox.Text = MonthComboBox.Items[MonthComboBox.Items.Count - 1].ToString();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate() { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (TimeSheetDataGrid.ColumnCount != 0)
                WorkTimeSheet.ExportToExcel(TimeSheetDataGrid);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;            
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
            WorkTimeRegister.SetOverduedColor();
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
            WorkTimeRegister.SetOverduedColor();
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
            WorkTimeRegister.SetOverduedColor();
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
            WorkTimeRegister.SetOverduedColor();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            _productionShedule.GetCalendar(YearComboBox1.SelectedItem.ToString());
            _productionShedule.FillHoursDataTable();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Данные обновлены", 1700);
        }

        private void btnSaveCalendar_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            _productionShedule.FillSourceDataTable(YearComboBox1.SelectedItem.ToString());
            _productionShedule.SaveCalendar();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Календарь сохраненён", 1700);
        }
        //----------------------------------------------
    }
}
