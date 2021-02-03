using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DayPlannerForm : Form
    {
        public int CurrentNotesID;
        public string CurrentNotesName;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;
        Form TopForm = null;

        InfiniumFunctionsContainer[] ProfilFunctionsContainers;
        InfiniumFunctionsContainer[] TPSFunctionsContainers;
        UsersResponsibilities AdminResponsibilities;

        LightWorkDay LightWorkDay;
        DayStatus DayStatus;
        DayFactStatus DayFactStatus;

        DayPlannerTimesheet DayPlannerWorkTimeSheet;
        InfiniumProjects InfiniumProjects;

        int TimesheetMonth = 1;
        int TimesheetYear = 1;

        bool bC = false;
        int ProfilUserFunctionsContainerCount = 0;
        int TPSUserFunctionsContainerCount = 0;
        int ProfilUserFunctionsContainerCount1 = 0;
        int TPSUserFunctionsContainerCount1 = 0;
        bool bNeedSplash = false;
        bool bNeedNewsSplash = false;

        static TimeSpan DeltaTime;

        CultureInfo CI = new CultureInfo("ru-RU");

        private void AddProfilInfiniumFunctionsContainer(ref InfiniumFunctionsContainer FunctionsContainer, string Position)
        {
            int yMargin = 20;
            int yMargin1 = 0;
            if (ProfilUserFunctionsContainerCount1 != 0)
            {
                this.pnlProfilFunctions1.Size = new Size(this.pnlProfilFunctions1.Size.Width, panel26.Height);
                yMargin = 45;
                yMargin1 = 20;
            }

            Label label1 = new Label()
            {
                Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right))),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Pixel),
                ForeColor = Color.Black,
                Location = new Point(0, ProfilUserFunctionsContainerCount1 * panel26.Height + yMargin1),
                Name = "ProfilPositionLabel" + ProfilUserFunctionsContainerCount1,
                Size = new Size(336, 19),
                Text = Position,
                TextAlign = ContentAlignment.MiddleLeft
            };
            FunctionsContainer = new InfiniumFunctionsContainer()
            {
                Anchor = ((AnchorStyles)((((AnchorStyles.Top)
            | AnchorStyles.Left)
            | AnchorStyles.Right))),
                FunctionsDataTable = null,
                Location = new Point(0, ProfilUserFunctionsContainerCount1++ * panel26.Height + yMargin),
                Name = "InfiniumFunctionsContainer" + ProfilUserFunctionsContainerCount1,
                ReadOnly = false,
                Size = new Size(771, panel26.Height),
                TabIndex = 0,
                TimePickersFont = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Pixel)
            };
            FunctionsContainer.TimeChanged += new InfiniumFunctionsContainer.TimeChangedEventHandler(FunctionsContainer_TimeChanged);
            this.pnlProfilFunctions1.Controls.Add(label1);
            this.pnlProfilFunctions1.Controls.Add(FunctionsContainer);
        }

        private void AddTPSInfiniumFunctionsContainer(ref InfiniumFunctionsContainer FunctionsContainer, string Position)
        {
            int yMargin = 20;
            int yMargin1 = 0;
            if (TPSUserFunctionsContainerCount1 != 0)
            {
                this.pnlTPSFunctions1.Size = new Size(this.pnlTPSFunctions1.Size.Width, panel26.Height);
                yMargin = 45;
                yMargin1 = 20;
            }
            Label label1 = new Label()
            {
                Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right))),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Pixel),
                ForeColor = Color.Black,
                Location = new Point(0, TPSUserFunctionsContainerCount1 * panel26.Height + yMargin1),
                Name = "TPSPositionLabel" + TPSUserFunctionsContainerCount1,
                Size = new Size(336, 19),
                Text = Position,
                TextAlign = ContentAlignment.MiddleLeft
            };
            FunctionsContainer = new InfiniumFunctionsContainer()
            {
                Anchor = ((AnchorStyles)((((AnchorStyles.Top)
            | AnchorStyles.Left)
            | AnchorStyles.Right))),
                FunctionsDataTable = null,
                Location = new Point(0, TPSUserFunctionsContainerCount1++ * panel26.Height + yMargin),
                Name = "InfiniumFunctionsContainer" + TPSUserFunctionsContainerCount1,
                ReadOnly = false,
                Size = new Size(771, panel26.Height),
                TabIndex = 0,
                TimePickersFont = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Pixel)
            };
            FunctionsContainer.TimeChanged += new InfiniumFunctionsContainer.TimeChangedEventHandler(FunctionsContainer_TimeChanged);
            this.pnlTPSFunctions1.Controls.Add(label1);
            this.pnlTPSFunctions1.Controls.Add(FunctionsContainer);
        }

        private void AddProfilUserFunctionsContainer(int DepartmentID, int PositionID, string Position)
        {
            if (ProfilUserFunctionsContainerCount != 0)
                this.panel12.Size = new Size(this.panel12.Size.Width, this.panel12.Size.Height + 347);
            UserFunctionsContainer userFunctionsContainer = new UserFunctionsContainer()
            {
                Location = new Point(0, ProfilUserFunctionsContainerCount++ * 347),
                Name = "ProfilUserFunctionsContainer" + ProfilUserFunctionsContainerCount,
                FactoryID = 1,
                DepartmentID = DepartmentID,
                PositionID = PositionID,
                Position = Position,
                UsersFunctionsDataTable = AdminResponsibilities.GetProfilPositionFunctions(PositionID),
                AdminResponsibilities = AdminResponsibilities
            };
            userFunctionsContainer.UsersFunctionsSetting();
            //userFunctionsContainer.btnDeleteUserFunction_Click += EventArgs(btnDeleteUserFunction_Click);
            this.panel12.Controls.Add(userFunctionsContainer);
        }

        public void btnDeleteUserFunction_Click(object sender, EventArgs e)
        {
        }

        private void AddTPSUserFunctionsContainer(int DepartmentID, int PositionID, string Position)
        {
            if (TPSUserFunctionsContainerCount != 0)
                this.panel27.Size = new Size(this.panel27.Size.Width, this.panel27.Size.Height + 347);
            UserFunctionsContainer userFunctionsContainer = new UserFunctionsContainer()
            {
                Location = new Point(0, TPSUserFunctionsContainerCount++ * 347),
                Name = "TPSUserFunctionsContainer" + TPSUserFunctionsContainerCount,
                FactoryID = 2,
                DepartmentID = DepartmentID,
                PositionID = PositionID,
                Position = Position,
                UsersFunctionsDataTable = AdminResponsibilities.GetTPSPositionFunctions(PositionID),
                AdminResponsibilities = AdminResponsibilities
            };
            userFunctionsContainer.UsersFunctionsSetting();
            this.panel27.Controls.Add(userFunctionsContainer);
        }

        public DayPlannerForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();


            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            InfiniumProjects = new InfiniumProjects();
            InfiniumProjects.Fill();

            LightWorkDay = new LightWorkDay();
            LightWorkDay.GetDayStatus(Security.CurrentUserID);

            AdminResponsibilities = new UsersResponsibilities(0, 0, 0);

            if (!AdminResponsibilities.HasProfilFunctions)
            {
                cbtnProfilFunctions.Visible = false;
                cbtnProfilFunctions1.Visible = false;
                pnlProfilFunctions.Visible = false;
                pnlProfilFunctions1.Visible = false;
                if (!AdminResponsibilities.HasTPSFunctions)
                {
                    cbtnTPSFunctions.Visible = false;
                    cbtnTPSFunctions1.Visible = false;
                    pnlTPSFunctions.Visible = false;
                    pnlTPSFunctions1.Visible = false;
                }
                else
                {
                    cbtnTPSFunctions.Checked = true;
                    cbtnTPSFunctions1.Checked = true;
                }
            }
            else
            {
                ProfilFunctionsContainers = new InfiniumFunctionsContainer[AdminResponsibilities.ProfilPositionsCount];
                AdminResponsibilities.UpdateProfilFunctions();
                for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                {
                    int DepartmentID = -1;
                    int PositionID = -1;
                    decimal Rate = 1;
                    string Position = string.Empty;
                    AdminResponsibilities.GetProfilPositionsInfo(i, ref DepartmentID, ref PositionID, ref Rate, ref Position);
                    AddProfilUserFunctionsContainer(DepartmentID, PositionID, Position);

                    AddProfilInfiniumFunctionsContainer(ref ProfilFunctionsContainers[i], Position + " (" + Rate.ToString("G29") + ")");
                    LightWorkDay.UpdateFunctions(1, PositionID);
                    LightWorkDay.FillMyFunctionDataTable(1);
                    ProfilFunctionsContainers[i].FunctionsDataTable = LightWorkDay.MyFunctionDataTableDT;
                }
            }
            if (!AdminResponsibilities.HasTPSFunctions)
            {
                cbtnTPSFunctions.Visible = false;
                cbtnTPSFunctions1.Visible = false;
                pnlTPSFunctions.Visible = false;
                pnlTPSFunctions1.Visible = false;
                if (!AdminResponsibilities.HasProfilFunctions)
                {
                    cbtnProfilFunctions.Visible = false;
                    cbtnProfilFunctions1.Visible = false;
                    pnlProfilFunctions.Visible = false;
                    pnlProfilFunctions1.Visible = false;
                }
                else
                {
                    cbtnProfilFunctions.Checked = true;
                    cbtnProfilFunctions1.Checked = true;
                }
            }
            else
            {
                TPSFunctionsContainers = new InfiniumFunctionsContainer[AdminResponsibilities.TPSPositionsCount];
                AdminResponsibilities.UpdateTPSFunctions();
                for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                {
                    int DepartmentID = -1;
                    int PositionID = -1;
                    decimal Rate = 1;
                    string Position = string.Empty;
                    AdminResponsibilities.GetTPSPositionsInfo(i, ref DepartmentID, ref PositionID, ref Rate, ref Position);
                    AddTPSUserFunctionsContainer(DepartmentID, PositionID, Position);

                    AddTPSInfiniumFunctionsContainer(ref TPSFunctionsContainers[i], Position + " (" + Rate.ToString("G29") + ")");
                    LightWorkDay.UpdateFunctions(2, PositionID);
                    LightWorkDay.FillMyFunctionDataTable(2);
                    TPSFunctionsContainers[i].FunctionsDataTable = LightWorkDay.MyFunctionDataTableDT;
                }
            }

            Initialize();

            DayPlannerWorkTimeSheet = new DayPlannerTimesheet();

            DeltaTime = Security.GetCurrentDate() - DateTime.Now;

            GetProjects();

            CalendarFrom.SelectionStart = DateTime.Today;

            while (!SplashForm.bCreated) ;
        }

        private void GetProjects()
        {
            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            infiniumProjectsList1.ProjectsDataTable = InfiniumProjects.ProjectsDataTable;
            infiniumProjectsList1.UsersDataTable = InfiniumProjects.UsersDataTable;
            InfiniumProjects.FillProjects(0, 0, -1, -1);
            InfiniumProjects.FillMembers(0);
            infiniumProjectsList1.Filter();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;
        }

        private void DayPlannerForm1_Shown(object sender, EventArgs e)
        {
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
                    bNeedSplash = true;
                    bNeedNewsSplash = true;
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


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    bNeedSplash = true;
                    bNeedNewsSplash = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            if (DayStatus.iDayStatus == LightWorkDay.sDayNotSaved)
            {
                if (LightMessageBox.Show(ref TopForm, true, "Вы не сохранили рабочий день! Все равно закрыть модуль?", "Рабочий день") == false)
                    return;
            }

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

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton.Checked)
                kryptonCheckSet1.CheckedButton.BringToFront();

            if (kryptonCheckSet1.CheckedButton.Name == "TodayMenuButton")
            {
                TodayPanel.BringToFront();
                return;
            }

            if (kryptonCheckSet1.CheckedButton.Name == "WorkDayMenuButton")
            {
                WorkDayPanel.BringToFront();
                return;
            }

            if (kryptonCheckSet1.CheckedButton.Name == "ProjectsMenuButton")
            {
                ProjectsPanel.BringToFront();
                return;
            }

            if (kryptonCheckSet1.CheckedButton.Name == "MyFunctionsMenuButton")
            {
                MyFunctionsPanel.BringToFront();
                return;
            }

            if (kryptonCheckSet1.CheckedButton.Name == "TimeSheetMenuButton")
            {
                TimeSheetPanel.BringToFront();
                return;
            }
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet2.CheckedButton.Name == "cbtnProfilFunctions")
            {
                pnlProfilFunctions.BringToFront();
            }
            if (kryptonCheckSet2.CheckedButton.Name == "cbtnTPSFunctions")
            {
                pnlTPSFunctions.BringToFront();
            }
        }

        private void CurrentTimeTimer_Tick(object sender, EventArgs e)
        {
            infiniumWorkDayClock.CurrentTime = DateTime.Now + DeltaTime;

            infiniumClock1.iMinutes = (DateTime.Now + DeltaTime).Minute;
            infiniumClock1.iSeconds = (DateTime.Now + DeltaTime).Second;
            infiniumClock1.iHours = (DateTime.Now + DeltaTime).Hour;
            infiniumClock1.Refresh();

            CurrentTimeLabel.Text = (DateTime.Now + DeltaTime).ToString("HH:mm:ss");
            CurrentDateLabel.Text = (DateTime.Now + DeltaTime).ToString("dddd") + ", " + DateTime.Now.ToString("d MMMM");
            //string tempTime = CurrentTimeLabel.Text;

            //CurrentTimeLabel.Text = DateTime.Now.ToString("HH:mm");
            //CurrentDayOfWeekLabel.Text = DateTime.Now.ToString("dddd");
            //CurrentDayMonthLabel.Text = DateTime.Now.ToString("dd MMMM");

            //if (CurrentTimeLabel.Text != tempTime)
            //{
            //    CurrentDayOfWeekLabel.Left = GetTimeLabelLength(CurrentTimeLabel.Text) + CurrentTimeLabel.Left;
            //    CurrentDayMonthLabel.Left = GetTimeLabelLength(CurrentTimeLabel.Text) + CurrentTimeLabel.Left;
            //}
        }

        private int GetTimeLabelLength(string Text)
        {
            int W = 0;

            Graphics G = this.CreateGraphics();
            Font F = new Font("Segoe UI Semilight", 90, FontStyle.Regular, GraphicsUnit.Pixel);

            W = Convert.ToInt32(G.MeasureString(Text, F).Width);

            F.Dispose();
            G.Dispose();

            return W;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            //if (TimeSheetDataGrid.ColumnCount != 0)
            //    DayPlannerWorkTimeSheet.ExportToExcel(TimeSheetDataGrid);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DateCorrectForm DateCorrectForm = new DateCorrectForm(ref TopForm, DateCorrectForm.dEndDay);

            TopForm = DateCorrectForm;

            DateCorrectForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();

            if (DayStatus.iDayStatus == LightWorkDay.sDayNotSaved)
                TodayMenuButton.Checked = true;
            //  TodayPanel.BringToFront();
        }

        private void StatusToControls()
        {
            DayStatus = LightWorkDay.GetDayStatus(Security.CurrentUserID);

            StartButton.Visible = true;
            BreakButton.Visible = true;
            ContinueButton.Visible = true;
            StopButton.Visible = true;
            btnCancelEnd.Visible = true;
            SaveWorkDayButton.Visible = true;

            if (DayStatus.iDayStatus != LightWorkDay.sDayNotStarted)
            {
                for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                {
                    int DepartmentID = -1;
                    int PositionID = -1;
                    decimal Rate = 1;
                    string Position = string.Empty;
                    AdminResponsibilities.GetProfilPositionsInfo(i, ref DepartmentID, ref PositionID, ref Rate, ref Position);
                    LightWorkDay.UpdateFunctions(1, PositionID);
                    LightWorkDay.FillMyFunctionDataTable(1);
                    ProfilFunctionsContainers[i].FunctionsDataTable = LightWorkDay.MyFunctionDataTableDT;
                }
                for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                {
                    int DepartmentID = -1;
                    int PositionID = -1;
                    decimal Rate = 1;
                    string Position = string.Empty;
                    AdminResponsibilities.GetTPSPositionsInfo(i, ref DepartmentID, ref PositionID, ref Rate, ref Position);
                    LightWorkDay.UpdateFunctions(2, PositionID);
                    LightWorkDay.FillMyFunctionDataTable(2);
                    TPSFunctionsContainers[i].FunctionsDataTable = LightWorkDay.MyFunctionDataTableDT;
                }
                CommentsRichTextBoxProfil.Text = LightWorkDay.ProfilComments;
                CommentsRichTextBoxTPS.Text = LightWorkDay.TPSComments;
                //if (CommentsRichTextBoxProfil.Text.Length > 0)
                //    bComments = true;
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayNotStarted)
            {
                DayLengthLabel.Text = "-- : --";
                DayStartLabel.Text = "-- : --";
                BreakStartLabel.Text = "-- : --";
                BreakEndLabel.Text = "-- : --";
                DayEndLabel.Text = "-- : --";

                BreakButton.Visible = false;
                ContinueButton.Visible = false;
                StopButton.Visible = false;
                btnCancelEnd.Visible = false;
                SaveWorkDayButton.Visible = false;
                infiniumWorkDayClock.bStarted = false;
                infiniumWorkDayClock.bBreakStart = false;
                infiniumWorkDayClock.bBreakEnd = false;
                infiniumWorkDayClock.bEnded = false;
                infiniumWorkDayClock.Refresh();

                CommentsLabelProfil.Visible = false;
                CommentsRichTextBoxProfil.Visible = false;
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayStarted)
            {
                //infiniumFunctionsContainer1.ReadOnly = false;
                StatusLabel.Text = "Рабочий день начат";
                WorkDayStatusLabel.Text = StatusLabel.Text;
                DayStatusHelpLabel.Text = "Не забудьте отметить обеденный перерыв";
            }

            if (DayStatus.iDayStatus == LightWorkDay.sBreakStarted)
            {
                StatusLabel.Text = "Перерыв";
                WorkDayStatusLabel.Text = StatusLabel.Text;
                DayStatusHelpLabel.Text = "Продолжите рабочий день на вкладке «Рабочий день»";
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayContinued)
            {
                StatusLabel.Text = "Рабочий день продолжается";
                WorkDayStatusLabel.Text = StatusLabel.Text;
                DayStatusHelpLabel.Text = "В конце рабочего дня укажите время его завершения";
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayEnded || DayStatus.iDayStatus == LightWorkDay.sDayNotSaved)
            {
                //if(DayStatus.iDayStatus != LightWorkDay.sDayNotSaved)
                //    infiniumFunctionsContainer1.ReadOnly = true;
                StatusLabel.Text = "Рабочий день завершен";

                WorkDayStatusLabel.Text = StatusLabel.Text;

                if (DayStatus.DayEnded.DayOfYear != DateTime.Now.DayOfYear)
                    WorkDayStatusLabel.Text += " (" + DayStatus.DayEnded.ToShortDateString() + ")";

                DayStatusHelpLabel.Text = "Укажите время выполнения обязанностей и сохраните\nрабочий день";
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayNotStarted)
            {
                StatusLabel.Text = "Рабочий день не начат";
                WorkDayStatusLabel.Text = StatusLabel.Text;
                DayStatusHelpLabel.Text = "Начните рабочий день на вкладке «Рабочий день»";
            }

            if (DayStatus.iDayStatus != LightWorkDay.sDayEnded)
            {
                DayLengthLabel.Text = "-- : --";
                DayStartLabel.Text = "-- : --";
                BreakStartLabel.Text = "-- : --";
                BreakEndLabel.Text = "-- : --";
                DayEndLabel.Text = "-- : --";
            }
            if (DayStatus.iDayStatus == LightWorkDay.sDayEnded)
                SaveWorkDayButton.Visible = false;

            if (DayStatus.iDayStatus == LightWorkDay.sDayStarted)
            {
                StartButton.Visible = false;
                BreakButton.BringToFront();
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == LightWorkDay.sBreakStarted)
            {
                StartButton.Visible = false;
                ContinueButton.BringToFront();
                StopButton.Visible = false;
                btnCancelEnd.Visible = false;
                //SaveButton.Visible = false;
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
                BreakStartLabel.Text = DayStatus.BreakStarted.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayContinued)
            {
                StartButton.Visible = false;
                BreakButton.Visible = false;
                ContinueButton.Visible = false;
                DayStartLabel.Text = DayStatus.DayStarted.ToString("HH:mm");
                BreakStartLabel.Text = DayStatus.BreakStarted.ToString("HH:mm");
                BreakEndLabel.Text = DayStatus.BreakEnded.ToString("HH:mm");
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayEnded || DayStatus.iDayStatus == LightWorkDay.sDayNotSaved)
            {
                StartButton.Visible = false;
                BreakButton.Visible = false;
                ContinueButton.Visible = false;
                StopButton.Visible = false;
                btnCancelEnd.Visible = true;

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

            DayLengthLabel.Text = GetDayLength();

            ChangeBreakEndLabel.Click -= ChangeBreakEndLabel_Click;
            ChangeBreakStartLabel.Click -= ChangeBreakStartLabel_Click;
            ChangeDayEndLabel.Click -= ChangeDayEndLabel_Click;
            ChangeDayStartLabel.Click -= ChangeDayStartLabel_Click;

            if (DayStatus.DayStarted.ToString("dd.MM.yyyy") != "01.01.0001" && DayStatus.iDayStatus != LightWorkDay.sDayEnded)
            {
                ChangeDayStartLabel.Click += ChangeDayStartLabel_Click;
                ChangeDayStartLabel.Cursor = Cursors.Hand;
            }
            else
                ChangeDayStartLabel.Cursor = Cursors.Default;

            if (DayStatus.DayEnded.ToString("dd.MM.yyyy") != "01.01.0001" && DayStatus.iDayStatus != LightWorkDay.sDayEnded)
            {
                ChangeDayEndLabel.Click += ChangeDayEndLabel_Click;
                ChangeDayEndLabel.Cursor = Cursors.Hand;
            }
            else
                ChangeDayEndLabel.Cursor = Cursors.Default;

            if (DayStatus.BreakStarted.ToString("dd.MM.yyyy") != "01.01.0001" && DayStatus.iDayStatus != LightWorkDay.sDayEnded)
            {
                ChangeBreakStartLabel.Click += ChangeBreakStartLabel_Click;
                ChangeBreakStartLabel.Cursor = Cursors.Hand;
            }
            else
                ChangeBreakStartLabel.Cursor = Cursors.Default;

            if (DayStatus.BreakEnded.ToString("dd.MM.yyyy") != "01.01.0001" && DayStatus.iDayStatus != LightWorkDay.sDayEnded)
            {
                ChangeBreakEndLabel.Click += ChangeBreakEndLabel_Click;
                ChangeBreakEndLabel.Cursor = Cursors.Hand;
            }
            else
                ChangeBreakEndLabel.Cursor = Cursors.Default;

            DayFactStatus = LightWorkDay.GetStatusFactTime(ref ChangeDayStartLabel, ref ChangeBreakStartLabel, ref ChangeBreakEndLabel, ref ChangeDayEndLabel);

            if (DayLengthLabel.Text == "-- : --")
                DayLengthLabel.ForeColor = Color.Gray;
            else
                DayLengthLabel.ForeColor = Color.FromArgb(0, 102, 204);

            if (DayStartLabel.Text == "-- : --")
                DayStartLabel.ForeColor = Color.Gray;
            else
                DayStartLabel.ForeColor = Color.FromArgb(0, 102, 204);

            if (DayEndLabel.Text == "-- : --")
                DayEndLabel.ForeColor = Color.Gray;
            else
                DayEndLabel.ForeColor = Color.FromArgb(0, 102, 204);

            if (BreakStartLabel.Text == "-- : --")
                BreakStartLabel.ForeColor = Color.Gray;
            else
                BreakStartLabel.ForeColor = Color.FromArgb(0, 102, 204);

            if (BreakEndLabel.Text == "-- : --")
                BreakEndLabel.ForeColor = Color.Gray;
            else
                BreakEndLabel.ForeColor = Color.FromArgb(0, 102, 204);

            LightWorkDay.GetWeekTimeSheet(ref MonTimeSheetlabel, ref TueTimeSheetlabel, ref WedTimeSheetlabel, ref ThuTimeSheetlabel, ref FriTimeSheetlabel, ref SatTimeSheetlabel, ref SunTimeSheetlabel);

        }

        private void Initialize()
        {
            StatusToControls();

            DayLengthLabel.Text = GetDayLength();

            if (DayLengthLabel.Text == "-- : --")
                DayLengthLabel.ForeColor = Color.Gray;
            else
                DayLengthLabel.ForeColor = Color.FromArgb(0, 102, 204);
        }

        private string GetDayLength()
        {

            if (DayStatus.iDayStatus == 0)//day not started
            {
                LightWorkDay.TotalMin = 0;
                return "-- : --";
            }

            int Minutes = 0;

            string L = "";

            if (DayStatus.iDayStatus == LightWorkDay.sDayStarted)
            {
                Minutes = (Convert.ToInt32((Security.GetCurrentDate() - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes));
                infiniumWorkDayClock.dtStartDate = DayStatus.DayStarted;
                infiniumWorkDayClock.bStarted = true;
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayEnded || DayStatus.iDayStatus == LightWorkDay.sDayNotSaved)
            {
                int Break = 0;

                if (DayStatus.bBreak)
                    Break = (Convert.ToInt32((Convert.ToDateTime(DayStatus.BreakEnded) - Convert.ToDateTime(DayStatus.BreakStarted)).TotalMinutes));

                Minutes = (Convert.ToInt32((Convert.ToDateTime(DayStatus.DayEnded) - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes) - Break);

                infiniumWorkDayClock.bStarted = true;

                if (DayStatus.bBreak)
                {
                    infiniumWorkDayClock.bBreakStart = true;
                    infiniumWorkDayClock.bBreakEnd = true;
                    infiniumWorkDayClock.dtBreakStartDate = DayStatus.BreakStarted;
                    infiniumWorkDayClock.dtBreakEndDate = DayStatus.BreakEnded;
                }
                infiniumWorkDayClock.bEnded = true;

                infiniumWorkDayClock.dtStartDate = DayStatus.DayStarted;
                infiniumWorkDayClock.dtEndDate = DayStatus.DayEnded;
            }

            if (DayStatus.iDayStatus == LightWorkDay.sBreakStarted)
            {
                Minutes = Convert.ToInt32((Convert.ToDateTime(DayStatus.BreakStarted) - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes);

                infiniumWorkDayClock.bStarted = true;
                infiniumWorkDayClock.bBreakStart = true;
                infiniumWorkDayClock.dtStartDate = DayStatus.DayStarted;
                infiniumWorkDayClock.dtBreakStartDate = DayStatus.BreakStarted;
            }

            if (DayStatus.iDayStatus == LightWorkDay.sDayContinued)
            {
                int Break = 0;

                Break = (Convert.ToInt32((Convert.ToDateTime(DayStatus.BreakEnded) - Convert.ToDateTime(DayStatus.BreakStarted)).TotalMinutes));

                Minutes = (Convert.ToInt32((Security.GetCurrentDate() - Convert.ToDateTime(DayStatus.DayStarted)).TotalMinutes) - Break);

                infiniumWorkDayClock.bStarted = true;
                infiniumWorkDayClock.bBreakStart = true;
                infiniumWorkDayClock.bBreakEnd = true;
                infiniumWorkDayClock.dtStartDate = DayStatus.DayStarted;
                infiniumWorkDayClock.dtBreakStartDate = DayStatus.BreakStarted;
                infiniumWorkDayClock.dtBreakEndDate = DayStatus.BreakEnded;
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
            LightWorkDay.TotalMin = Minutes;
            for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
            {
                ProfilFunctionsContainers[i].TotalMin = Minutes;
                FunctionsContainer_TimeChanged(ProfilFunctionsContainers[i], Minutes);
            }
            for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
            {
                TPSFunctionsContainers[i].TotalMin = Minutes;
                FunctionsContainer_TimeChanged(TPSFunctionsContainers[i], Minutes);
            }

            infiniumWorkDayClock.Refresh();

            return res + " м";
        }

        private string MinToHHmm(int Minutes)
        {
            return Convert.ToInt32(Minutes / 60).ToString() + " : " + (Minutes - Convert.ToInt32(Minutes / 60) * 60).ToString();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DateCorrectForm DateCorrectForm = new DateCorrectForm(ref TopForm, DateCorrectForm.dStartDay);

            TopForm = DateCorrectForm;

            DateCorrectForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void BreakButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DateCorrectForm DateCorrectForm = new DateCorrectForm(ref TopForm, DateCorrectForm.dBreakDay);

            TopForm = DateCorrectForm;

            DateCorrectForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DateCorrectForm DateCorrectForm = new DateCorrectForm(ref TopForm, DateCorrectForm.dContinueDay);

            TopForm = DateCorrectForm;

            DateCorrectForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void DayTimer_Tick(object sender, EventArgs e)
        {
            //if (DayStatus.iDayStatus == LightWorkDay.sDayEnded)
            //return;

            DayLengthLabel.Text = GetDayLength();

            if (DayLengthLabel.Text == "-- : --")
                DayLengthLabel.ForeColor = Color.Gray;
            else
                DayLengthLabel.ForeColor = Color.FromArgb(0, 102, 204);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ChangeDayStartLabel_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm EditTimeForm = new EditTimeForm(ref TopForm, DateCorrectForm.dStartDay, ref DayFactStatus, ref DayStatus);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void ChangeBreakStartLabel_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm EditTimeForm = new EditTimeForm(ref TopForm, DateCorrectForm.dBreakDay, ref DayFactStatus, ref DayStatus);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void ChangeBreakEndLabel_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm EditTimeForm = new EditTimeForm(ref TopForm, DateCorrectForm.dContinueDay, ref DayFactStatus, ref DayStatus);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void ChangeDayEndLabel_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            EditTimeForm EditTimeForm = new EditTimeForm(ref TopForm, DateCorrectForm.dEndDay, ref DayFactStatus, ref DayStatus);

            TopForm = EditTimeForm;

            EditTimeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            StatusToControls();
        }

        private void WorkDayStatusLabel_Click(object sender, EventArgs e)
        {
            WorkDayMenuButton.Checked = true;
        }

        private void FunctionsContainer_TimeChanged(object sender, int Minutes)
        {
            InfiniumFunctionsContainer infiniumFunctionsContainer1 = (InfiniumFunctionsContainer)sender;
            int iNA = 0;
            int TotalAllocMin = 0;

            for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                TotalAllocMin += ProfilFunctionsContainers[i].TotalAllocMin;
            for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                TotalAllocMin += TPSFunctionsContainers[i].TotalAllocMin;
            iNA = LightWorkDay.TotalMin - TotalAllocMin;

            int H = Convert.ToInt32(iNA / 60);
            int M = iNA - H * 60;

            if (H < 0 || M < 0)
            {
                if (H < 0)
                    H *= -1;
                if (M < 0)
                    M *= -1;

                if (M > 9)
                    NotAllocLabel.Text = " - " + H.ToString() + " : " + M.ToString();
                else
                    NotAllocLabel.Text = " - " + H.ToString() + " : 0" + M.ToString();
            }
            else
            {
                if (M > 9)
                    NotAllocLabel.Text = H.ToString() + " : " + M.ToString();
                else
                    NotAllocLabel.Text = H.ToString() + " : 0" + M.ToString();

                if (M > 9)
                    NotAllocHelpLabel.Text = "Нераспределённое рабочее время (" + H.ToString() + " ч : " + M.ToString() + " м)";
                else
                    NotAllocHelpLabel.Text = "Нераспределённое рабочее время (" + H.ToString() + " ч : 0" + M.ToString() + " м)";
            }

            DataRow[] Rows = infiniumFunctionsContainer1.dtFunctionsDataTable.Select("FunctionID1 = 0");

            if (Rows.Count() > 0 && Convert.ToInt32(Rows[0]["FactoryID"]) == 1)
            {
                if (cbtnProfilFunctions1.Checked && Rows.Count() > 0)
                {
                    if (Convert.ToInt32(Rows[0]["Hours"]) > 0 || Convert.ToInt32(Rows[0]["Minutes"]) > 0)
                    {
                        CommentsLabelProfil.Visible = true;
                        CommentsRichTextBoxProfil.Visible = true;
                    }
                    else
                    {
                        CommentsLabelProfil.Visible = false;
                        CommentsRichTextBoxProfil.Visible = false;
                    }
                    //CommentsLabelTPS.Visible = false;
                    //CommentsRichTextBoxTPS.Visible = false;
                }
                else
                {
                    CommentsLabelProfil.Visible = false;
                    CommentsRichTextBoxProfil.Visible = false;
                }
            }
            if (Rows.Count() > 0 && Convert.ToInt32(Rows[0]["FactoryID"]) == 2)
            {
                if (cbtnTPSFunctions1.Checked && Rows.Count() > 0)
                {
                    if (Convert.ToInt32(Rows[0]["Hours"]) > 0 || Convert.ToInt32(Rows[0]["Minutes"]) > 0)
                    {
                        CommentsLabelTPS.Visible = true;
                        CommentsRichTextBoxTPS.Visible = true;
                    }
                    else
                    {
                        CommentsLabelTPS.Visible = false;
                        CommentsRichTextBoxTPS.Visible = false;
                    }
                    //CommentsLabelProfil.Visible = false;
                    //CommentsRichTextBoxProfil.Visible = false;
                }
                else
                {
                    CommentsLabelTPS.Visible = false;
                    CommentsRichTextBoxTPS.Visible = false;
                }
            }
        }

        private void SaveWorkDayButton_Click(object sender, EventArgs e)
        {
            int TotalAllocMin = 0;
            for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                TotalAllocMin += ProfilFunctionsContainers[i].TotalAllocMin;
            for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                TotalAllocMin += TPSFunctionsContainers[i].TotalAllocMin;

            if (LightWorkDay.TotalMin - TotalAllocMin < 0)
            {
                ErrorSaveLabel.Visible = true;
                ErrorSaveLabel.ForeColor = Color.Red;
                ErrorSaveLabel.Text = "У вас распределено лишнее время.";
                return;
            }

            LightWorkDay.ProfilComments = CommentsRichTextBoxProfil.Text;
            LightWorkDay.TPSComments = CommentsRichTextBoxTPS.Text;

            if (DayStatus.iDayStatus == LightWorkDay.sDayNotSaved)
            {
                if (LightWorkDay.TotalMin - TotalAllocMin > 0)
                {
                    ErrorSaveLabel.Visible = true;
                    ErrorSaveLabel.ForeColor = Color.Red;
                    ErrorSaveLabel.Text = "У вас осталось нераспределенное время.";
                    return;
                }

                for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                {
                    string Error = LightWorkDay.SaveMyFunction(true, ProfilFunctionsContainers[i]);

                    if (Error != "")
                    {
                        ErrorSaveLabel.Visible = true;
                        ErrorSaveLabel.ForeColor = Color.Red;
                        ErrorSaveLabel.Text = Error;
                        return;
                    }
                }
                for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                {
                    string Error = LightWorkDay.SaveMyFunction(true, TPSFunctionsContainers[i]);

                    if (Error != "")
                    {
                        ErrorSaveLabel.Visible = true;
                        ErrorSaveLabel.ForeColor = Color.Red;
                        ErrorSaveLabel.Text = Error;
                        return;
                    }
                }

                SaveWorkDayButton.Visible = false;
                LightWorkDay.SaveCurrentWorkDay();

                LightWorkDay.ProfilComments = "";
                CommentsRichTextBoxProfil.Text = "";
                LightWorkDay.TPSComments = "";
                CommentsRichTextBoxTPS.Text = "";

                for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                {
                    ProfilFunctionsContainers[i].ReadOnly = true;
                    ProfilFunctionsContainers[i].Refresh();
                }
                for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                {
                    TPSFunctionsContainers[i].ReadOnly = true;
                    TPSFunctionsContainers[i].Refresh();
                }
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено...", 1700);
                LightWorkDay.SaveWorkDayDetails();
                StatusToControls();

                if (ErrorSaveLabel.Visible)
                    ErrorSaveLabel.Visible = false;
            }
            else
            {
                for (int i = 0; i < AdminResponsibilities.ProfilPositionsCount; i++)
                {
                    string Error = LightWorkDay.SaveMyFunction(true, ProfilFunctionsContainers[i]);

                    if (Error != "")
                    {
                        ErrorSaveLabel.Visible = true;
                        ErrorSaveLabel.ForeColor = Color.Red;
                        ErrorSaveLabel.Text = Error;
                        return;
                    }
                }
                for (int i = 0; i < AdminResponsibilities.TPSPositionsCount; i++)
                {
                    string Error = LightWorkDay.SaveMyFunction(true, TPSFunctionsContainers[i]);

                    if (Error != "")
                    {
                        ErrorSaveLabel.Visible = true;
                        ErrorSaveLabel.ForeColor = Color.Red;
                        ErrorSaveLabel.Text = Error;
                        return;
                    }
                }
            }

        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void WorkDayMenuButton_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void infiniumProjectsList1_SelectedChanged(object sender, EventArgs e)
        {
            if (bNeedNewsSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(ProjectsUpdatePanel.Top + UpdatePanel.Top + ProjectsPanel.Top, ProjectsPanel.Left + ProjectsUpdatePanel.Left + UpdatePanel.Left + 1,
                                                   ProjectsUpdatePanel.Height, ProjectsUpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (infiniumProjectsList1.Selected != -1)
            {
                panel4.Visible = true;
                ProjectsSplitContainer.Visible = true;


                ProjectCaptionLabel.Text = InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectName"].ToString();

                AuthorLabel.Text =
                     InfiniumProjects.GetUserName(Convert.ToInt32(
                           InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["AuthorID"])) + ", " +
                     Convert.ToDateTime(
                         InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["CreationDate"]).ToString("dd MMMM yyyy HH:mm");

                AuthorPhotoBox.Image = InfiniumProjects.GetUserPhoto(
                    Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["AuthorID"]));

                infiniumProjectsDescriptionBox1.DescriptionItem.DescriptionText =
                    InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectDescription"].ToString();

                InfiniumProjects.FillProjectNews(Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));
                InfiniumProjects.FillComments();
                InfiniumProjects.FillLikes();
                InfiniumProjects.FillAttachments();


                NewsContainer.Clear();
                NewsContainer.CurrentUserID = Security.CurrentUserID;
                NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
                NewsContainer.UsersDataTable = InfiniumProjects.UsersDataTable.Copy();
                NewsContainer.AttachsDT = InfiniumProjects.ProjectNewsAttachmentsDataTable.Copy();
                NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
                NewsContainer.NewsLikesDT = InfiniumProjects.ProjectNewsLikesDataTable.Copy();
                NewsContainer.CommentsLikesDT = InfiniumProjects.ProjectNewsCommentsLikesDataTable.Copy();
                NewsContainer.CreateNews();
                //InfiniumProjects.ClearAllSubs(Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));
                NewsContainer.Visible = true;
                NewsContainer.Focus();

                if (InfiniumProjects.ProjectNewsDataTable.Rows.Count > 0)
                {
                    NoNewsLabel.SendToBack();
                    NoNewsPicture.SendToBack();
                }
                else
                {
                    NoNewsPicture.BringToFront();
                    NoNewsLabel.BringToFront();
                }
            }
            else
            {
                NewsContainer.Clear();
                ProjectCaptionLabel.Text = "";
                AuthorLabel.Text = "";
                AuthorPhotoBox.Image = null;

                panel4.Visible = false;
                ProjectsSplitContainer.Visible = false;

                InfiniumProjects.FillProjectMembers(infiniumProjectsList1.ProjectID);

                infiniumProjectsDescriptionBox1.DescriptionItem.DescriptionText = "";
                NoNewsLabel.BringToFront();
                NoNewsPicture.BringToFront();
            }


            InfiniumProjects.FillProjectMembers(infiniumProjectsList1.ProjectID);
            ProjectMembersList.UsersDataTable = InfiniumProjects.CurrentProjectUsersDataTable;
            ProjectMembersList.DepartmentsDataTable = InfiniumProjects.CurrentProjectDepartmentsDataTable;
            ProjectMembersList.CreateItems();



            if (bNeedNewsSplash)
                bC = true;
        }

        private void AddNewsPicture_Click(object sender, EventArgs e)
        {
            AddNewsLabel_Click(null, null);
        }

        private void AddNewsLabel_Click(object sender, EventArgs e)
        {
            if (infiniumProjectsList1.ProjectID == -1)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddProjectNewsForm AddProjectNewsForm = new AddProjectNewsForm(ref InfiniumProjects, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]), ref TopForm);

            TopForm = AddProjectNewsForm;

            AddProjectNewsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddProjectNewsForm.Canceled)
                return;

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;



            InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
            InfiniumProjects.ReloadComments();
            InfiniumProjects.ReloadAttachments();
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
            NewsContainer.AttachsDT = InfiniumProjects.ProjectNewsAttachmentsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToTop();
            NewsContainer.Focus();


            if (InfiniumProjects.ProjectNewsDataTable.Rows.Count > 0)
            {
                NoNewsLabel.SendToBack();
                NoNewsPicture.SendToBack();
            }
            else
            {
                NoNewsPicture.BringToFront();
                NoNewsLabel.BringToFront();
            }


            bC = true;
        }

        private void NewsContainer_CommentLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            InfiniumProjects.LikeComments(Security.CurrentUserID, NewsID, NewsCommentID, infiniumProjectsList1.ProjectID);

            InfiniumProjects.FillLikes();
            NewsContainer.CommentsLikesDT = InfiniumProjects.ProjectNewsCommentsLikesDataTable.Copy();
            NewsContainer.ReloadLikes(NewsID);
            NewsContainer.Focus();
        }

        private void NewsContainer_AttachClicked(object sender, int NewsAttachID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ProjectAttachDownloadForm ProjectAttachDownloadForm = new ProjectAttachDownloadForm(NewsAttachID, ref InfiniumProjects.FM, ref InfiniumProjects);

            TopForm = ProjectAttachDownloadForm;

            ProjectAttachDownloadForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            TopForm = null;

            ProjectAttachDownloadForm.Dispose();
        }

        private void NewsContainer_CommentSendButtonClicked(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID, bool bNoNotify)
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (bEdit)
                InfiniumProjects.EditComments(SenderID, NewsCommentID, Text);
            else
                InfiniumProjects.AddComments(SenderID,
                                             Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]),
                                             NewsID, Text, bNoNotify);


            InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
            InfiniumProjects.ReloadComments();
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToTop();
            NewsContainer.ReloadNewsItem(NewsID, true);
            NewsContainer.Refresh();
            NewsContainer.Focus();


            bC = true;
        }

        private void NewsContainer_CommentsQuoteLabelClicked(object sender)
        {
            InfiniumTips.ShowTip(this, 50, 85, "Комментарий скопирован в буфер обмена...", 2200);
        }

        private void NewsContainer_EditCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            NewsContainer.Refresh();
        }

        private void NewsContainer_EditNewsClicked(object sender, int NewsID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddProjectNewsForm AddProjectNewsForm = new AddProjectNewsForm(ref InfiniumProjects,
                             Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]),
                                                      InfiniumProjects.GetThisNewsBodyText(NewsID),
                                                      NewsID, InfiniumProjects.GetThisNewsDateTime(NewsID), ref TopForm);

            TopForm = AddProjectNewsForm;

            AddProjectNewsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddProjectNewsForm.Canceled)
                return;

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;


            InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
            InfiniumProjects.ReloadComments();
            InfiniumProjects.ReloadAttachments();
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
            NewsContainer.AttachsDT = InfiniumProjects.ProjectNewsAttachmentsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToTop();
            NewsContainer.Focus();

            bC = true;
        }

        private void NewsContainer_LikeClicked(object sender, int NewsID)
        {
            InfiniumProjects.LikeNews(Security.CurrentUserID, NewsID, infiniumProjectsList1.ProjectID);

            InfiniumProjects.FillLikes();
            NewsContainer.NewsLikesDT = InfiniumProjects.ProjectNewsLikesDataTable;
            NewsContainer.ReloadLikes(NewsID);
            NewsContainer.Focus();
        }

        private void NewsContainer_NeedMoreNews(object sender, EventArgs e)
        {
            if (InfiniumProjects.IsMoreNews(NewsContainer.NewsCount, infiniumProjectsList1.ProjectID))
                MoreNewsLabel.Visible = true;
            else
                MoreNewsLabel.Visible = false;
        }

        private void NewsContainer_NewsQuoteLabelClicked(object sender)
        {
            InfiniumTips.ShowTip(this, 50, 85, "Текст скопирован в буфер обмена...", 2200);
        }

        private void NewsContainer_NoNeedMoreNews(object sender, EventArgs e)
        {
            if (MoreNewsLabel.Visible)
                MoreNewsLabel.Visible = false;
        }

        private void NewsContainer_RemoveCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            LightMessageBoxForm LightMessageBoxForm = new LightMessageBoxForm(true, "Комментарий будет удален.\nПродолжить?",
                                                                                    "Удаление комментария");

            TopForm = LightMessageBoxForm;

            LightMessageBoxForm.ShowDialog();

            TopForm = null;

            PhantomForm.Close();
            PhantomForm.Dispose();


            if (LightMessageBoxForm.OKCancel)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                InfiniumProjects.RemoveComment(NewsCommentID);

                InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
                InfiniumProjects.ReloadComments();
                NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
                NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
                NewsContainer.CreateNews();
                NewsContainer.ScrollToTop();
                NewsContainer.ReloadNewsItem(NewsID, true);
                NewsContainer.Refresh();
                NewsContainer.Focus();

                bC = true;
            }
        }

        private void NewsContainer_RemoveNewsClicked(object sender, int NewsID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            LightMessageBoxForm LightMessageBoxForm = new LightMessageBoxForm(true, "Сообщение будет удалено безвозвратно.\nПродолжить?",
                                                                                    "Удаление сообщения");

            TopForm = LightMessageBoxForm;

            LightMessageBoxForm.ShowDialog();

            TopForm = null;

            PhantomForm.Close();
            PhantomForm.Dispose();


            if (LightMessageBoxForm.OKCancel)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumProjects.RemoveNews(NewsID);
                InfiniumProjects.ReloadNews(NewsContainer.NewsCount, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));
                InfiniumProjects.ReloadComments();
                InfiniumProjects.ReloadAttachments();
                NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
                NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
                NewsContainer.CreateNews();
                NewsContainer.SetNewsPositions();
                NewsContainer.Refresh();
            }

            LightMessageBoxForm.Dispose();

            if (InfiniumProjects.ProjectNewsDataTable.Rows.Count > 0)
            {
                NoNewsLabel.SendToBack();
                NoNewsPicture.SendToBack();
            }
            else
            {
                NoNewsPicture.BringToFront();
                NoNewsLabel.BringToFront();
            }


            bC = true;
        }

        private void btnCancelEnd_Click(object sender, EventArgs e)
        {
            LightWorkDay.CancelEndWorkDay(Security.CurrentUserID);
            StatusToControls();
        }

        private void infiniumTimeLabel1_Click(object sender, EventArgs e)
        {

        }

        private void CalendarFrom_DateChanged(object sender, DateRangeEventArgs e)
        {
            int userId = 313;
            //int userId = Security.CurrentUserID;

                TimesheetMonth = e.Start.Month;
                TimesheetYear = e.Start.Year;

                DayPlannerWorkTimeSheet.GetAbsJournal(userId, TimesheetYear, TimesheetMonth);
                DayPlannerWorkTimeSheet.GetProdShedule(TimesheetYear, TimesheetMonth);
                DayPlannerWorkTimeSheet.GetTimesheet(userId, TimesheetYear, TimesheetMonth);
                DayPlannerWorkTimeSheet.GetRate(userId);

            DayStatus dayStatus = LightWorkDay.GetDayStatus(userId, e.Start.Date);

            if (dayStatus.iDayStatus != LightWorkDay.sDayNotStarted)
            {
                label32.Text = "День завершен";

                if (dayStatus.bDayStarted)
                    timeEdit1.EditValue = dayStatus.DayStarted.ToString("HH:mm");

                if (dayStatus.bBreakStarted)
                    timeEdit2.EditValue = dayStatus.BreakStarted.ToString("HH:mm");

                if (dayStatus.bBreakEnded)
                    timeEdit3.EditValue = dayStatus.BreakEnded.ToString("HH:mm");

                if (dayStatus.bDayEnded)
                    timeEdit4.EditValue = dayStatus.DayEnded.ToString("HH:mm");
            }
            else
            {
                label32.Text = "День не начат";

                timeEdit1.EditValue = new System.DateTime(e.Start.Date.Year, e.Start.Date.Month, e.Start.Date.Day, 8, 0, 0, 0);
                timeEdit2.EditValue = new System.DateTime(e.Start.Date.Year, e.Start.Date.Month, e.Start.Date.Day, 12, 0, 0, 0);
                timeEdit3.EditValue = new System.DateTime(e.Start.Date.Year, e.Start.Date.Month, e.Start.Date.Day, 13, 0, 0, 0);
                timeEdit4.EditValue = new System.DateTime(e.Start.Date.Year, e.Start.Date.Month, e.Start.Date.Day, 17, 0, 0, 0);
            }

            DayPlannerWorkTimeSheet.CalcOverwork(TimesheetYear, TimesheetMonth, e.Start.Date);

            TimesheetInfo dayInfo = DayPlannerWorkTimeSheet.GetDayInfo(e.Start.Date);
            if (dayInfo.IsAbsence)
            {
                lbAbsenceHours.Text = string.Format("{0}/({1})", dayInfo.AbsenceShortName, dayInfo.AbsenceHours.ToString());
            }
            else
            {
                lbAbsenceHours.Text = "-";
            }
            lbPlanHours.Text = dayInfo.PlanHours.ToString();
            lbRate.Text = dayInfo.StrRate.ToString();
            lbOverworkHours.Text = dayInfo.OverworkHours.ToString();
            if (dayInfo.AbsenceTypeID != 14)
            {
                lbFactHours.Text = dayInfo.FactHours.ToString();
                lbBreakHours.Text = dayInfo.BreakHours.ToString();
            }
            else
            {
                lbFactHours.Text = dayInfo.AbsenceHours.ToString();
                lbBreakHours.Text = "-";
            }
        }

        private void kryptonCheckSet3_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet3.CheckedButton.Name == "cbtnProfilFunctions1")
            {
                pnlProfilFunctions1.BringToFront();
                pnlProfilFunctions1.Visible = true;
                pnlTPSFunctions1.Visible = false;
            }
            if (kryptonCheckSet3.CheckedButton.Name == "cbtnTPSFunctions1")
            {
                pnlTPSFunctions1.BringToFront();
                pnlProfilFunctions1.Visible = false;
                pnlTPSFunctions1.Visible = true;
            }
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            int userId = 313;
            //int userId = Security.CurrentUserID;


            DayStatus dayStatus = LightWorkDay.GetDayStatus(userId, CalendarFrom.SelectionStart);

            dayStatus.DayStarted = Convert.ToDateTime(CalendarFrom.SelectionStart.ToShortDateString() + " " + timeEdit1.Time.ToString("HH:mm:ss.fff"));
            dayStatus.BreakStarted = Convert.ToDateTime(CalendarFrom.SelectionStart.ToShortDateString() + " " + timeEdit2.Time.ToString("HH:mm:ss.fff"));
            dayStatus.BreakEnded = Convert.ToDateTime(CalendarFrom.SelectionStart.ToShortDateString() + " " + timeEdit3.Time.ToString("HH:mm:ss.fff"));
            dayStatus.DayEnded = Convert.ToDateTime(CalendarFrom.SelectionStart.ToShortDateString() + " " + timeEdit4.Time.ToString("HH:mm:ss.fff"));

            dayStatus.iDayStatus = LightWorkDay.sDayEnded;

            LightWorkDay.SaveDay(userId, dayStatus);

            InfiniumTips.ShowTip(this, 50, 85, "Рабочий день сохранен", 1700);

            TimesheetMonth = CalendarFrom.SelectionStart.Month;
            TimesheetYear = CalendarFrom.SelectionStart.Year;

            DayPlannerWorkTimeSheet.GetAbsJournal(userId, TimesheetYear, TimesheetMonth);
            DayPlannerWorkTimeSheet.GetProdShedule(TimesheetYear, TimesheetMonth);
            DayPlannerWorkTimeSheet.GetTimesheet(userId, TimesheetYear, TimesheetMonth);
            DayPlannerWorkTimeSheet.GetRate(userId);

            dayStatus = LightWorkDay.GetDayStatus(userId, CalendarFrom.SelectionStart);
            if (dayStatus.iDayStatus != LightWorkDay.sDayNotStarted)
            {
                label32.Text = "День завершен";

                if (dayStatus.bDayStarted)
                    timeEdit1.EditValue = dayStatus.DayStarted.ToString("HH:mm");

                if (dayStatus.bBreakStarted)
                    timeEdit2.EditValue = dayStatus.BreakStarted.ToString("HH:mm");

                if (dayStatus.bBreakEnded)
                    timeEdit3.EditValue = dayStatus.BreakEnded.ToString("HH:mm");

                if (dayStatus.bDayEnded)
                    timeEdit4.EditValue = dayStatus.DayEnded.ToString("HH:mm");
            }
            else
            {
                label32.Text = "День не начат";

                timeEdit1.EditValue = new System.DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, CalendarFrom.SelectionStart.Day, 8, 0, 0, 0);
                timeEdit2.EditValue = new System.DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, CalendarFrom.SelectionStart.Day, 12, 0, 0, 0);
                timeEdit3.EditValue = new System.DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, CalendarFrom.SelectionStart.Day, 13, 0, 0, 0);
                timeEdit4.EditValue = new System.DateTime(CalendarFrom.SelectionStart.Year, CalendarFrom.SelectionStart.Month, CalendarFrom.SelectionStart.Day, 17, 0, 0, 0);
            }

            DayPlannerWorkTimeSheet.CalcOverwork(TimesheetYear, TimesheetMonth, CalendarFrom.SelectionStart);

            TimesheetInfo dayInfo = DayPlannerWorkTimeSheet.GetDayInfo(CalendarFrom.SelectionStart);
            if (dayInfo.IsAbsence)
            {
                lbAbsenceHours.Text = string.Format("{0}/({1})", dayInfo.AbsenceShortName, dayInfo.AbsenceHours.ToString());
            }
            else
            {
                lbAbsenceHours.Text = "-";
            }
            lbPlanHours.Text = dayInfo.PlanHours.ToString();
            lbRate.Text = dayInfo.StrRate.ToString();
            lbOverworkHours.Text = dayInfo.OverworkHours.ToString();
            if (dayInfo.AbsenceTypeID != 14)
            {
                lbFactHours.Text = dayInfo.FactHours.ToString();
                lbBreakHours.Text = dayInfo.BreakHours.ToString();
            }
            else
            {
                lbFactHours.Text = dayInfo.AbsenceHours.ToString();
                lbBreakHours.Text = "-";
            }
        }
    }
}
