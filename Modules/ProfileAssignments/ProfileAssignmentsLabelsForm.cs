using ComponentFactory.Krypton.Toolkit;

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ProfileAssignmentsLabelsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int BatchAssignmentID = 0;
        int DecorAssignmentID = 0;
        object BatchDateTime = DBNull.Value;

        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        ProfileAssignmentsLabels ProfileAssignmentsLabelsManager;
        PrintDefectProfileAssignmentsLabels PrintDefectLabelsManager;
        PrintProfileAssignmentsLabels PrintLabelsManager;

        public ProfileAssignmentsLabelsForm(Form tMainForm, int iBatchAssignmentID, int iDecorAssignmentID, object oBatchDateTime)
        {
            InitializeComponent();
            BatchAssignmentID = iBatchAssignmentID;
            DecorAssignmentID = iDecorAssignmentID;
            BatchDateTime = oBatchDateTime;
            MainForm = tMainForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
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
                        MainForm.Activate();
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

        private void ProfileAssignmentsLabelsForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ProfileAssignmentsLabelsForm_Load(object sender, EventArgs e)
        {
            ProfileAssignmentsLabelsManager = new ProfileAssignmentsLabels()
            {
                CurrentDecorAssignmentID = DecorAssignmentID
            };
            ProfileAssignmentsLabelsManager.Initialize();
            dgvFactLabels.DataSource = ProfileAssignmentsLabelsManager.FactLabelsList;
            dgvDisprepancyLabels.DataSource = ProfileAssignmentsLabelsManager.DisprepancyLabelsList;
            dgvDefectLabels.DataSource = ProfileAssignmentsLabelsManager.DefectLabelsList;
            dgvLabelsSetting(ref dgvFactLabels);
            HideEmptyColumns(ref dgvFactLabels);
            dgvLabelsSetting(ref dgvDisprepancyLabels);
            HideEmptyColumns(ref dgvDisprepancyLabels);
            dgvLabelsSetting(ref dgvDefectLabels);
            HideEmptyColumns(ref dgvDefectLabels);
            ProfileAssignmentsLabelsManager.FillFactLabelsTable();
            ProfileAssignmentsLabelsManager.FillDisprepancyLabelsTable();
            ProfileAssignmentsLabelsManager.FillDefectLabelsTable();
            lbFactLabelsTotalAmount.Text = ProfileAssignmentsLabelsManager.FactTotalAmount.ToString();
            lbFactLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.FactRestAmount.ToString();
            lbDisprepancyLabelsTotalAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyTotalAmount.ToString();
            lbDisprepancyLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyRestAmount.ToString();
            lbDefectLabelsTotalAmount.Text = ProfileAssignmentsLabelsManager.DefectTotalAmount.ToString();
            lbDefectLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DefectRestAmount.ToString();

            PrintLabelsManager = new PrintProfileAssignmentsLabels();
            PrintDefectLabelsManager = new PrintDefectProfileAssignmentsLabels();
            kryptonCheckSet1.CheckedButton = cbtnFactLabels;
        }

        private void dgvLabelsSetting(ref PercentageDataGrid grid)
        {
            KryptonDataGridViewButtonColumn DeleteColumn = new KryptonDataGridViewButtonColumn()
            {
                HeaderText = string.Empty,
                Name = "DeleteColumn",
                Text = "Удалить",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(DeleteColumn);
            grid.Columns.Add(ProfileAssignmentsLabelsManager.ClientNameColumn);
            grid.Columns.Add(ProfileAssignmentsLabelsManager.CoversColumn);
            grid.Columns.Add(ProfileAssignmentsLabelsManager.TechStoreColumn);

            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("LabelType"))
                grid.Columns["LabelType"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentsLabelID"))
                grid.Columns["DecorAssignmentsLabelID"].Visible = false;
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
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
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
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.MinimumWidth = 50;
            }

            grid.Columns["MegaOrderID"].HeaderText = "Заказ";
            grid.Columns["MainOrderID"].HeaderText = "Подзаказ";
            grid.Columns["Height2"].HeaderText = "Высота";
            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["Width2"].HeaderText = "Ширина";
            grid.Columns["Length2"].HeaderText = "Длина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Notes"].ReadOnly = false;

            int DisplayIndex = 0;
            grid.Columns["ClientNameColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["MegaOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["MainOrderID"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechStoreColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["CoversColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Length2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width2"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["DeleteColumn"].DisplayIndex = DisplayIndex++;

            grid.Columns["MegaOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MegaOrderID"].Width = 100;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["MainOrderID"].Width = 100;
            grid.Columns["DeleteColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["DeleteColumn"].Width = 100;

            grid.Columns["ClientNameColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ClientNameColumn"].MinimumWidth = 80;
            grid.Columns["TechStoreColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechStoreColumn"].MinimumWidth = 80;
            grid.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["CoversColumn"].MinimumWidth = 80;
            grid.Columns["Height2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height2"].MinimumWidth = 80;
            grid.Columns["Length2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Length2"].MinimumWidth = 80;
            grid.Columns["Width2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width2"].MinimumWidth = 80;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count"].MinimumWidth = 80;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 80;
        }

        private void HideEmptyColumns(ref PercentageDataGrid grid)
        {
            foreach (DataGridViewColumn Column in grid.Columns)
            {
                foreach (DataGridViewRow Row in grid.Rows)
                {
                    if (Column.Name == "DeleteColumn"
                        || Column.Name == "Notes")
                        continue;
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        grid.Columns[Column.Index].Visible = false;
                }
            }

            if (grid.Columns.Contains("LabelType"))
                grid.Columns["LabelType"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentsLabelID"))
                grid.Columns["DecorAssignmentsLabelID"].Visible = false;
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
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("ClientID"))
                grid.Columns["ClientID"].Visible = false;
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
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void dgvLabels_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.SaveFactLabels();
            ProfileAssignmentsLabelsManager.UpdateFactLabels();
            ProfileAssignmentsLabelsManager.FillFactLabelsTable();
            lbFactLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.FactRestAmount.ToString();
            HideEmptyColumns(ref dgvFactLabels);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void btnAddLabels_Click(object sender, EventArgs e)
        {
            if (tbFactLabelsAmount.Text.Length == 0 || tbFactPositionsAmount.Text.Length == 0)
                return;
            int LabelsAmount = Convert.ToInt32(tbFactLabelsAmount.Text);
            int PositionsAmount = Convert.ToInt32(tbFactPositionsAmount.Text);
            string Notes = rtbFactLabelsNotes.Text;
            ProfileAssignmentsLabelsManager.CreateFactLabels(LabelsAmount, PositionsAmount, Notes);
            tbFactLabelsAmount.Text = "1";
            tbFactPositionsAmount.Text = "1";
            lbFactLabelsTotalAmount.Text = ProfileAssignmentsLabelsManager.FactTotalAmount.ToString();
            lbFactLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.FactRestAmount.ToString();
            HideEmptyColumns(ref dgvFactLabels);
        }

        private void dgvLabels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "DeleteColumn" && e.RowIndex >= 0)
            {
                ProfileAssignmentsLabelsManager.RemoveFactLabel();
                lbFactLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.FactRestAmount.ToString();
                HideEmptyColumns(ref dgvFactLabels);
            }
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            if (dgvFactLabels.SelectedRows.Count == 0)
                return;

            int[] DecorAssignmentsLabelID = new int[dgvFactLabels.SelectedRows.Count];
            for (int i = 0; i < dgvFactLabels.SelectedRows.Count; i++)
            {
                if (dgvFactLabels.SelectedRows[i].Cells["DecorAssignmentsLabelID"].Value != DBNull.Value)
                    DecorAssignmentsLabelID[i] = Convert.ToInt32(dgvFactLabels.SelectedRows[i].Cells["DecorAssignmentsLabelID"].Value);
            }
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.RemoveFactLabels(DecorAssignmentsLabelID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            lbFactLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.FactRestAmount.ToString();
            HideEmptyColumns(ref dgvFactLabels);
        }

        private void btnSaveLabels_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.SaveFactLabels();
            ProfileAssignmentsLabelsManager.UpdateFactLabels();
            ProfileAssignmentsLabelsManager.FillFactLabelsTable();
            lbFactLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.FactRestAmount.ToString();
            HideEmptyColumns(ref dgvFactLabels);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void kryptonContextMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (dgvFactLabels.SelectedRows.Count == 0)
                return;

            bool NeedUserName = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Ввести номер смены?",
                    "Ась?");

            bool PressOK = false;
            object DocDateTime = DBNull.Value;
            int NumberOfChange = 0;
            int WeekNumber = 0;
            string UserName = string.Empty;

            if (NeedUserName)
            {
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();
                InputPackageInfoMenu InputPackageInfoMenu = new InputPackageInfoMenu(this);
                TopForm = InputPackageInfoMenu;
                InputPackageInfoMenu.ShowDialog();
                PressOK = InputPackageInfoMenu.PressOK;
                DocDateTime = InputPackageInfoMenu.DocDateTime;
                NumberOfChange = InputPackageInfoMenu.NumberOfChange;
                UserName = InputPackageInfoMenu.UserName;
                PhantomForm.Close();
                PhantomForm.Dispose();
                InputPackageInfoMenu.Dispose();
                TopForm = null;

                if (!PressOK)
                    return;
            }

            PrintLabelsManager.ClearLabelInfo();
            int[] Labels = new int[dgvFactLabels.SelectedRows.Count];
            for (int j = 0; j < Labels.Count(); j++)
            {
                ProfileAssignmentsLabelInfo LabelInfo = new ProfileAssignmentsLabelInfo();
                int LabelID = 0;
                int ClientID = 0;
                int MegaOrderID = 0;
                int MainOrderID = 0;
                int Length = 0;
                int Height = 0;
                int Width = 0;
                int Count = 0;
                string ClientName = string.Empty;
                string Notes = string.Empty;
                string Product = string.Empty;
                string Color = string.Empty;

                if (dgvFactLabels.SelectedRows[j].Cells["DecorAssignmentsLabelID"].Value != DBNull.Value)
                    LabelID = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["DecorAssignmentsLabelID"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["ClientID"].Value != DBNull.Value)
                    ClientID = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["ClientID"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["MegaOrderID"].Value != DBNull.Value)
                    MegaOrderID = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["MegaOrderID"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["MainOrderID"].Value != DBNull.Value)
                    MainOrderID = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["MainOrderID"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["Length2"].Value != DBNull.Value)
                    Length = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["Length2"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["Height2"].Value != DBNull.Value)
                    Height = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["Height2"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["Width2"].Value != DBNull.Value)
                    Width = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["Width2"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["Count"].Value != DBNull.Value)
                    Count = Convert.ToInt32(dgvFactLabels.SelectedRows[j].Cells["Count"].Value);
                if (dgvFactLabels.SelectedRows[j].Cells["ClientNameColumn"].FormattedValue != DBNull.Value)
                    ClientName = dgvFactLabels.SelectedRows[j].Cells["ClientNameColumn"].FormattedValue.ToString();
                if (dgvFactLabels.SelectedRows[j].Cells["TechStoreColumn"].FormattedValue != DBNull.Value)
                    Product = dgvFactLabels.SelectedRows[j].Cells["TechStoreColumn"].FormattedValue.ToString();
                if (dgvFactLabels.SelectedRows[j].Cells["CoversColumn"].FormattedValue != DBNull.Value)
                    Color = dgvFactLabels.SelectedRows[j].Cells["CoversColumn"].FormattedValue.ToString();
                if (dgvFactLabels.SelectedRows[j].Cells["Notes"].FormattedValue != DBNull.Value)
                    Notes = dgvFactLabels.SelectedRows[j].Cells["Notes"].FormattedValue.ToString();
                LabelInfo.LabelsDT = ProfileAssignmentsLabelsManager.FillLabelsTable(Product, Color, Length, Height, Width, Count);
                LabelInfo.ClientName = ClientName;
                LabelInfo.ClientID = ClientID;
                LabelInfo.MegaOrderID = MegaOrderID;
                LabelInfo.MainOrderID = MainOrderID;
                LabelInfo.Notes = Notes;
                //LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                LabelInfo.PackNumber = LabelID;
                LabelInfo.TotalPackCount = ProfileAssignmentsLabelsManager.FactTotalAmount;
                LabelInfo.FactoryType = 1;
                LabelInfo.LabelType = 0;

                LabelInfo.BarcodeNumber = PrintLabelsManager.GetBarcodeNumber(16, LabelID);

                LabelInfo.GroupType = "М";
                LabelInfo.BatchID = BatchAssignmentID;
                LabelInfo.NeedUserName = NeedUserName;
                if (NeedUserName)
                {
                    WeekNumber = GetWeekNumber(Convert.ToDateTime(BatchDateTime));
                    LabelInfo.DocDateTime = Convert.ToDateTime(DocDateTime).ToString();
                    LabelInfo.NumberOfChange = NumberOfChange;
                    LabelInfo.UserName = UserName;
                    LabelInfo.WeekNumber = WeekNumber;
                }
                PrintLabelsManager.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = PrintLabelsManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                PrintLabelsManager.Print();
                ProfileAssignmentsLabelsManager.PrintFactLabels(Labels);
            }

        }

        public int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton == cbtnFactLabels)
            {
                pnlFactLabels.Visible = true;
                pnlDisprepancyLabels.Visible = false;
                pnlDefectLabels.Visible = false;
            }
            if (kryptonCheckSet1.CheckedButton == cbtnDisprepancyLabels)
            {
                pnlFactLabels.Visible = false;
                pnlDisprepancyLabels.Visible = true;
                pnlDefectLabels.Visible = false;
            }
            if (kryptonCheckSet1.CheckedButton == cbtnDefectLabels)
            {
                pnlFactLabels.Visible = false;
                pnlDisprepancyLabels.Visible = false;
                pnlDefectLabels.Visible = true;
            }
        }

        private void btnDefectAddLabels_Click(object sender, EventArgs e)
        {
            if (tbDefectLabelsAmount.Text.Length == 0 || tbDefectPositionsAmount.Text.Length == 0)
                return;
            int LabelsAmount = Convert.ToInt32(tbDefectLabelsAmount.Text);
            int PositionsAmount = Convert.ToInt32(tbDefectPositionsAmount.Text);
            string Notes = rtbDefectLabelsNotes.Text;
            ProfileAssignmentsLabelsManager.CreateDefectLabels(LabelsAmount, PositionsAmount, Notes);
            tbDefectLabelsAmount.Text = "1";
            tbDefectPositionsAmount.Text = "1";
            lbDefectLabelsTotalAmount.Text = ProfileAssignmentsLabelsManager.DefectTotalAmount.ToString();
            lbDefectLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DefectRestAmount.ToString();
            HideEmptyColumns(ref dgvDefectLabels);
        }

        private void btnDefectSaveLabels_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.SaveDefectLabels();
            ProfileAssignmentsLabelsManager.UpdateDefectLabels();
            ProfileAssignmentsLabelsManager.FillDefectLabelsTable();
            lbDefectLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DefectRestAmount.ToString();
            HideEmptyColumns(ref dgvDefectLabels);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvDefectLabels_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnDisprepancyAddLabels_Click(object sender, EventArgs e)
        {
            if (tbDisprepancyLabelsAmount.Text.Length == 0 || tbDisprepancyPositionsAmount.Text.Length == 0)
                return;
            int LabelsAmount = Convert.ToInt32(tbDisprepancyLabelsAmount.Text);
            int PositionsAmount = Convert.ToInt32(tbDisprepancyPositionsAmount.Text);
            string Notes = rtbDisprepancyLabelsNotes.Text;
            ProfileAssignmentsLabelsManager.CreateDisprepancyLabels(LabelsAmount, PositionsAmount, Notes);
            tbDisprepancyLabelsAmount.Text = "1";
            tbDisprepancyPositionsAmount.Text = "1";
            lbDisprepancyLabelsTotalAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyTotalAmount.ToString();
            lbDisprepancyLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyRestAmount.ToString();
            HideEmptyColumns(ref dgvDisprepancyLabels);
        }

        private void btnDisprepancySaveLabels_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.SaveDisprepancyLabels();
            ProfileAssignmentsLabelsManager.UpdateDisprepancyLabels();
            ProfileAssignmentsLabelsManager.FillDisprepancyLabelsTable();
            lbDisprepancyLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyRestAmount.ToString();
            HideEmptyColumns(ref dgvDisprepancyLabels);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void dgvDisprepancyLabels_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.SaveDisprepancyLabels();
            ProfileAssignmentsLabelsManager.UpdateDisprepancyLabels();
            ProfileAssignmentsLabelsManager.FillDisprepancyLabelsTable();
            lbDisprepancyLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyRestAmount.ToString();
            HideEmptyColumns(ref dgvDisprepancyLabels);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void kryptonContextMenuItem7_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.SaveDefectLabels();
            ProfileAssignmentsLabelsManager.UpdateDefectLabels();
            ProfileAssignmentsLabelsManager.FillDefectLabelsTable();
            lbDefectLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DefectRestAmount.ToString();
            HideEmptyColumns(ref dgvDefectLabels);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            if (dgvDisprepancyLabels.SelectedRows.Count == 0)
                return;
            bool NeedUserName = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Ввести номер смены?",
                    "Ась?");

            bool PressOK = false;
            object DocDateTime = DBNull.Value;
            int NumberOfChange = 0;
            int WeekNumber = 0;
            string UserName = string.Empty;

            if (NeedUserName)
            {
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();
                InputPackageInfoMenu InputPackageInfoMenu = new InputPackageInfoMenu(this);
                TopForm = InputPackageInfoMenu;
                InputPackageInfoMenu.ShowDialog();
                PressOK = InputPackageInfoMenu.PressOK;
                DocDateTime = InputPackageInfoMenu.DocDateTime;
                NumberOfChange = InputPackageInfoMenu.NumberOfChange;
                UserName = InputPackageInfoMenu.UserName;
                PhantomForm.Close();
                PhantomForm.Dispose();
                InputPackageInfoMenu.Dispose();
                TopForm = null;

                if (!PressOK)
                    return;
            }

            PrintDefectLabelsManager.ClearLabelInfo();
            int[] Labels = new int[dgvDisprepancyLabels.SelectedRows.Count];
            for (int j = 0; j < Labels.Count(); j++)
            {
                ProfileAssignmentsLabelInfo LabelInfo = new ProfileAssignmentsLabelInfo();
                int LabelID = 0;
                int ClientID = 0;
                int MegaOrderID = 0;
                int MainOrderID = 0;
                int Length = 0;
                int Height = 0;
                int Width = 0;
                int Count = 0;
                string ClientName = string.Empty;
                string DocNumber = string.Empty;
                string Notes = string.Empty;
                string Product = string.Empty;
                string Color = string.Empty;

                if (dgvDisprepancyLabels.SelectedRows[j].Cells["DecorAssignmentsLabelID"].Value != DBNull.Value)
                    LabelID = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["DecorAssignmentsLabelID"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["ClientID"].Value != DBNull.Value)
                    ClientID = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["ClientID"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["MegaOrderID"].Value != DBNull.Value)
                    MegaOrderID = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["MegaOrderID"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["MainOrderID"].Value != DBNull.Value)
                    MainOrderID = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["MainOrderID"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["Length2"].Value != DBNull.Value)
                    Length = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["Length2"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["Height2"].Value != DBNull.Value)
                    Height = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["Height2"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["Width2"].Value != DBNull.Value)
                    Width = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["Width2"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["Count"].Value != DBNull.Value)
                    Count = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[j].Cells["Count"].Value);
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["ClientNameColumn"].FormattedValue != DBNull.Value)
                    ClientName = dgvDisprepancyLabels.SelectedRows[j].Cells["ClientNameColumn"].FormattedValue.ToString();
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["TechStoreColumn"].FormattedValue != DBNull.Value)
                    Product = dgvDisprepancyLabels.SelectedRows[j].Cells["TechStoreColumn"].FormattedValue.ToString();
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["CoversColumn"].FormattedValue != DBNull.Value)
                    Color = dgvDisprepancyLabels.SelectedRows[j].Cells["CoversColumn"].FormattedValue.ToString();
                if (dgvDisprepancyLabels.SelectedRows[j].Cells["Notes"].FormattedValue != DBNull.Value)
                    Notes = dgvDisprepancyLabels.SelectedRows[j].Cells["Notes"].FormattedValue.ToString();
                LabelInfo.LabelsDT = ProfileAssignmentsLabelsManager.FillLabelsTable(Product, Color, Length, Height, Width, Count);
                LabelInfo.ClientName = ClientName;
                LabelInfo.ClientID = ClientID;
                LabelInfo.MegaOrderID = MegaOrderID;
                LabelInfo.MainOrderID = MainOrderID;
                LabelInfo.Notes = Notes;
                //LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                LabelInfo.PackNumber = LabelID;
                LabelInfo.TotalPackCount = ProfileAssignmentsLabelsManager.DisprepancyTotalAmount;
                LabelInfo.FactoryType = 1;
                LabelInfo.LabelType = 1;

                LabelInfo.BarcodeNumber = PrintDefectLabelsManager.GetBarcodeNumber(16, LabelID);

                LabelInfo.GroupType = "М";
                LabelInfo.BatchID = BatchAssignmentID;
                LabelInfo.NeedUserName = NeedUserName;
                if (NeedUserName)
                {
                    WeekNumber = GetWeekNumber(Convert.ToDateTime(BatchDateTime));
                    LabelInfo.DocDateTime = Convert.ToDateTime(DocDateTime).ToString();
                    LabelInfo.NumberOfChange = NumberOfChange;
                    LabelInfo.UserName = UserName;
                    LabelInfo.WeekNumber = WeekNumber;
                }

                PrintDefectLabelsManager.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = PrintDefectLabelsManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                PrintDefectLabelsManager.Print();
                ProfileAssignmentsLabelsManager.PrintDisprepancyLabels(Labels);
            }
        }

        private void kryptonContextMenuItem8_Click(object sender, EventArgs e)
        {
            if (dgvDefectLabels.SelectedRows.Count == 0)
                return;
            bool NeedUserName = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Ввести номер смены?",
                    "Ась?");

            bool PressOK = false;
            object DocDateTime = DBNull.Value;
            int NumberOfChange = 0;
            int WeekNumber = 0;
            string UserName = string.Empty;

            if (NeedUserName)
            {
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();
                InputPackageInfoMenu InputPackageInfoMenu = new InputPackageInfoMenu(this);
                TopForm = InputPackageInfoMenu;
                InputPackageInfoMenu.ShowDialog();
                PressOK = InputPackageInfoMenu.PressOK;
                DocDateTime = InputPackageInfoMenu.DocDateTime;
                NumberOfChange = InputPackageInfoMenu.NumberOfChange;
                UserName = InputPackageInfoMenu.UserName;
                PhantomForm.Close();
                PhantomForm.Dispose();
                InputPackageInfoMenu.Dispose();
                TopForm = null;

                if (!PressOK)
                    return;
            }

            PrintDefectLabelsManager.ClearLabelInfo();
            int[] Labels = new int[dgvDefectLabels.SelectedRows.Count];
            for (int j = 0; j < Labels.Count(); j++)
            {
                ProfileAssignmentsLabelInfo LabelInfo = new ProfileAssignmentsLabelInfo();
                int LabelID = 0;
                int ClientID = 0;
                int MegaOrderID = 0;
                int MainOrderID = 0;
                int Length = 0;
                int Height = 0;
                int Width = 0;
                int Count = 0;
                string ClientName = string.Empty;
                string Notes = string.Empty;
                string Product = string.Empty;
                string Color = string.Empty;

                if (dgvDefectLabels.SelectedRows[j].Cells["DecorAssignmentsLabelID"].Value != DBNull.Value)
                    LabelID = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["DecorAssignmentsLabelID"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["ClientID"].Value != DBNull.Value)
                    ClientID = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["ClientID"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["MegaOrderID"].Value != DBNull.Value)
                    MegaOrderID = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["MegaOrderID"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["MainOrderID"].Value != DBNull.Value)
                    MainOrderID = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["MainOrderID"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["Length2"].Value != DBNull.Value)
                    Length = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["Length2"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["Height2"].Value != DBNull.Value)
                    Height = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["Height2"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["Width2"].Value != DBNull.Value)
                    Width = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["Width2"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["Count"].Value != DBNull.Value)
                    Count = Convert.ToInt32(dgvDefectLabels.SelectedRows[j].Cells["Count"].Value);
                if (dgvDefectLabels.SelectedRows[j].Cells["ClientNameColumn"].FormattedValue != DBNull.Value)
                    ClientName = dgvDefectLabels.SelectedRows[j].Cells["ClientNameColumn"].FormattedValue.ToString();
                if (dgvDefectLabels.SelectedRows[j].Cells["TechStoreColumn"].FormattedValue != DBNull.Value)
                    Product = dgvDefectLabels.SelectedRows[j].Cells["TechStoreColumn"].FormattedValue.ToString();
                if (dgvDefectLabels.SelectedRows[j].Cells["CoversColumn"].FormattedValue != DBNull.Value)
                    Color = dgvDefectLabels.SelectedRows[j].Cells["CoversColumn"].FormattedValue.ToString();
                if (dgvDefectLabels.SelectedRows[j].Cells["Notes"].FormattedValue != DBNull.Value)
                    Notes = dgvDefectLabels.SelectedRows[j].Cells["Notes"].FormattedValue.ToString();
                LabelInfo.LabelsDT = ProfileAssignmentsLabelsManager.FillLabelsTable(Product, Color, Length, Height, Width, Count);
                LabelInfo.ClientName = ClientName;
                LabelInfo.ClientID = ClientID;
                LabelInfo.MegaOrderID = MegaOrderID;
                LabelInfo.MainOrderID = MainOrderID;
                LabelInfo.Notes = Notes;
                //LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                LabelInfo.PackNumber = LabelID;
                LabelInfo.TotalPackCount = ProfileAssignmentsLabelsManager.DefectTotalAmount;
                LabelInfo.FactoryType = 1;
                LabelInfo.LabelType = 2;

                LabelInfo.BarcodeNumber = PrintDefectLabelsManager.GetBarcodeNumber(16, LabelID);

                LabelInfo.GroupType = "М";
                LabelInfo.BatchID = BatchAssignmentID;
                LabelInfo.NeedUserName = NeedUserName;
                if (NeedUserName)
                {
                    WeekNumber = GetWeekNumber(Convert.ToDateTime(BatchDateTime));
                    LabelInfo.DocDateTime = Convert.ToDateTime(DocDateTime).ToString();
                    LabelInfo.NumberOfChange = NumberOfChange;
                    LabelInfo.UserName = UserName;
                    LabelInfo.WeekNumber = WeekNumber;
                }

                PrintDefectLabelsManager.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = PrintDefectLabelsManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                PrintDefectLabelsManager.Print();
                ProfileAssignmentsLabelsManager.PrintDefectLabels(Labels);
            }
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            if (dgvDisprepancyLabels.SelectedRows.Count == 0)
                return;

            int[] DecorAssignmentsLabelID = new int[dgvDisprepancyLabels.SelectedRows.Count];
            for (int i = 0; i < dgvDisprepancyLabels.SelectedRows.Count; i++)
            {
                if (dgvDisprepancyLabels.SelectedRows[i].Cells["DecorAssignmentsLabelID"].Value != DBNull.Value)
                    DecorAssignmentsLabelID[i] = Convert.ToInt32(dgvDisprepancyLabels.SelectedRows[i].Cells["DecorAssignmentsLabelID"].Value);
            }
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.RemoveDisprepancyLabels(DecorAssignmentsLabelID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            lbDisprepancyLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyRestAmount.ToString();
            HideEmptyColumns(ref dgvDisprepancyLabels);
        }

        private void kryptonContextMenuItem9_Click(object sender, EventArgs e)
        {
            if (dgvDefectLabels.SelectedRows.Count == 0)
                return;

            int[] DecorAssignmentsLabelID = new int[dgvDefectLabels.SelectedRows.Count];
            for (int i = 0; i < dgvDefectLabels.SelectedRows.Count; i++)
            {
                if (dgvDefectLabels.SelectedRows[i].Cells["DecorAssignmentsLabelID"].Value != DBNull.Value)
                    DecorAssignmentsLabelID[i] = Convert.ToInt32(dgvDefectLabels.SelectedRows[i].Cells["DecorAssignmentsLabelID"].Value);
            }
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Удаление данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            ProfileAssignmentsLabelsManager.RemoveDefectLabels(DecorAssignmentsLabelID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            lbDefectLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DefectRestAmount.ToString();
            HideEmptyColumns(ref dgvDefectLabels);
        }

        private void dgvDisprepancyLabels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "DeleteColumn" && e.RowIndex >= 0)
            {
                ProfileAssignmentsLabelsManager.RemoveDisprepancyLabel();
                lbDisprepancyLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DisprepancyRestAmount.ToString();
                HideEmptyColumns(ref senderGrid);
            }
        }

        private void dgvDefectLabels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "DeleteColumn" && e.RowIndex >= 0)
            {
                ProfileAssignmentsLabelsManager.RemoveDefectLabel();
                lbDefectLabelsRestAmount.Text = ProfileAssignmentsLabelsManager.DefectRestAmount.ToString();
                HideEmptyColumns(ref senderGrid);
            }
        }
    }
}
