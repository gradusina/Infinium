using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ReturnedRollersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        public bool OkReturned = true;

        Form MainForm = null;
        Form TopForm = null;

        int CoverID = 0;
        int DecorAssignmentID = 0;
        int FormEvent = 0;

        ProfileAssignments ProfileAssignmentsManager;

        public ReturnedRollersForm(Form tMainForm, ProfileAssignments tProfileAssignmentsManager, int iCoverID, int iDecorAssignmentID)
        {
            InitializeComponent();
            MainForm = tMainForm;
            ProfileAssignmentsManager = tProfileAssignmentsManager;
            CoverID = iCoverID;
            DecorAssignmentID = iDecorAssignmentID;

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

        private void ReturnedRollersForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ReturnedRollersForm_Load(object sender, EventArgs e)
        {
            ProfileAssignmentsManager.SaveTransferredRollers();
            ProfileAssignmentsManager.GetTransferredRollers(DecorAssignmentID);
            ProfileAssignmentsManager.SaveTransferredRollers();
            ProfileAssignmentsManager.GetTransferredRollers(DecorAssignmentID);
            ProfileAssignmentsManager.GetRollersInColorOnStorage(CoverID);
            dgvRollersInColor.DataSource = ProfileAssignmentsManager.RollersInColorOnStorageList;
            dgvReturnedRollers.DataSource = ProfileAssignmentsManager.ReturnedRollersList;
            dgvTransferredRollers.DataSource = ProfileAssignmentsManager.TransferredRollersList;
            dgvRollersInColorSetting(ref dgvRollersInColor);
            dgvReturnedRollersSetting(ref dgvReturnedRollers);
            dgvTransferredRollersSetting(ref dgvTransferredRollers);
        }

        private void dgvRollersInColorSetting(ref PercentageDataGrid grid)
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn Column1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn()
            {
                HeaderText = string.Empty,
                Name = "Column1",
                Text = "Передать",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("ManufactureStoreID"))
                grid.Columns["ManufactureStoreID"].Visible = false;
            if (grid.Columns.Contains("StoreItemID"))
                grid.Columns["StoreItemID"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Название";
            grid.Columns["Thickness"].HeaderText = "Толщина";
            grid.Columns["Diameter"].HeaderText = "Диаметр";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["CurrentCount"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Thickness"].Width = 120;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Diameter"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["CurrentCount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["CurrentCount"].Width = 120;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["CurrentCount"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;
        }

        private void dgvReturnedRollersSetting(ref PercentageDataGrid grid)
        {
            //ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn Column1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn();
            //Column1.HeaderText = string.Empty;
            //Column1.Name = "Column1";
            //Column1.Text = "Вернуть";
            //Column1.UseColumnTextForButtonValue = true;

            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn Column2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn()
            {
                HeaderText = string.Empty,
                Name = "Column2",
                Text = "Удалить",
                UseColumnTextForButtonValue = true
            };

            //grid.Columns.Add(Column1);
            grid.Columns.Add(Column2);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("ManufactureStoreID"))
                grid.Columns["ManufactureStoreID"].Visible = false;
            if (grid.Columns.Contains("ReturnedRollerID"))
                grid.Columns["ReturnedRollerID"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("StoreItemID"))
                grid.Columns["StoreItemID"].Visible = false;
            if (grid.Columns.Contains("ReturnType"))
                grid.Columns["ReturnType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.Columns["Count"].ReadOnly = false;
            grid.Columns["TechStoreName"].HeaderText = "Название";
            grid.Columns["Thickness"].HeaderText = "Толщина";
            grid.Columns["Diameter"].HeaderText = "Диаметр";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Thickness"].Width = 120;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Diameter"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Count"].Width = 120;
            //grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //grid.Columns["Column1"].Width = 100;
            grid.Columns["Column2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column2"].Width = 100;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Column1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column2"].DisplayIndex = DisplayIndex++;
        }

        private void dgvTransferredRollersSetting(ref PercentageDataGrid grid)
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn Column1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn()
            {
                HeaderText = string.Empty,
                Name = "Column1",
                Text = "Вернуть",
                UseColumnTextForButtonValue = true
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn Column2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewButtonColumn()
            {
                HeaderText = string.Empty,
                Name = "Column2",
                Text = "Удалить",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(Column1);
            grid.Columns.Add(Column2);
            grid.AutoGenerateColumns = false;

            if (grid.Columns.Contains("ManufactureStoreID"))
                grid.Columns["ManufactureStoreID"].Visible = false;
            if (grid.Columns.Contains("ReturnedRollerID"))
                grid.Columns["ReturnedRollerID"].Visible = false;
            if (grid.Columns.Contains("DecorAssignmentID"))
                grid.Columns["DecorAssignmentID"].Visible = false;
            if (grid.Columns.Contains("CreationUserID"))
                grid.Columns["CreationUserID"].Visible = false;
            if (grid.Columns.Contains("CreationDateTime"))
                grid.Columns["CreationDateTime"].Visible = false;
            if (grid.Columns.Contains("StoreItemID"))
                grid.Columns["StoreItemID"].Visible = false;
            if (grid.Columns.Contains("ReturnType"))
                grid.Columns["ReturnType"].Visible = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["TechStoreName"].HeaderText = "Название";
            grid.Columns["Thickness"].HeaderText = "Толщина";
            grid.Columns["Diameter"].HeaderText = "Диаметр";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";

            grid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["TechStoreName"].MinimumWidth = 100;
            grid.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Thickness"].Width = 120;
            grid.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Diameter"].Width = 120;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Width"].Width = 120;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Count"].Width = 120;
            grid.Columns["Column1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column1"].Width = 100;
            grid.Columns["Column2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            grid.Columns["Column2"].Width = 100;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 100;

            int DisplayIndex = 0;
            grid.Columns["TechStoreName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
            grid.Columns["Thickness"].DisplayIndex = DisplayIndex++;
            grid.Columns["Diameter"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column1"].DisplayIndex = DisplayIndex++;
            grid.Columns["Column2"].DisplayIndex = DisplayIndex++;
        }

        private void btnOkReturned_Click(object sender, EventArgs e)
        {
            //ProfileAssignmentsManager.SaveReturnedRollers();
            //ProfileAssignmentsManager.SaveTransferredRollers();
            OkReturned = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelReturned_Click(object sender, EventArgs e)
        {
            OkReturned = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void dgvRollersInColor_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                bool OKSplit = false;
                int ManufactureStoreID = 0;
                int NewCount = 0;
                int OldCount = 0;

                if (senderGrid.SelectedRows[0].Cells["ManufactureStoreID"].Value != DBNull.Value)
                    ManufactureStoreID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["ManufactureStoreID"].Value);
                if (senderGrid.SelectedRows[0].Cells["CurrentCount"].Value != DBNull.Value)
                    OldCount = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["CurrentCount"].Value);
                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(true, OldCount);
                TopForm = SplitAssignmentRequestMenu;
                SplitAssignmentRequestMenu.ShowDialog();

                OKSplit = SplitAssignmentRequestMenu.OKSplit;
                NewCount = SplitAssignmentRequestMenu.Count;

                PhantomForm.Close();
                PhantomForm.Dispose();
                SplitAssignmentRequestMenu.Dispose();
                TopForm = null;
                if (OKSplit)
                {
                    ProfileAssignmentsManager.AddTransferredRoller(DecorAssignmentID, ManufactureStoreID, NewCount);
                    //ProfileAssignmentsManager.WriteOffFromManufactureStoreByManufactureStoreID(ManufactureStoreID, NewCount);
                    ProfileAssignmentsManager.SaveTransferredRollers();
                    ProfileAssignmentsManager.GetTransferredRollers(DecorAssignmentID);
                    ProfileAssignmentsManager.GetRollersInColorOnStorage(CoverID);
                    InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                }
            }
        }

        private void dgvTransferredRollers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            {
                bool OKSplit = false;
                decimal NewDiameter = 0;
                decimal NewWidth = 0;
                int NewCount = 0;

                PhantomForm PhantomForm = new Infinium.PhantomForm();
                PhantomForm.Show();

                SplitAssignmentRequestMenu SplitAssignmentRequestMenu = new SplitAssignmentRequestMenu(false, 0);
                TopForm = SplitAssignmentRequestMenu;
                SplitAssignmentRequestMenu.ShowDialog();

                OKSplit = SplitAssignmentRequestMenu.OKSplit;
                NewDiameter = SplitAssignmentRequestMenu.Diameter;
                NewWidth = SplitAssignmentRequestMenu.iWidth;
                NewCount = SplitAssignmentRequestMenu.Count;

                PhantomForm.Close();
                PhantomForm.Dispose();
                SplitAssignmentRequestMenu.Dispose();
                TopForm = null;

                //int StoreItemID = -1;
                //decimal Length = -1;
                //decimal Width = -1;
                //decimal Height = -1;
                //decimal Thickness = -1;
                //decimal Diameter = -1;
                //decimal Admission = -1;
                //decimal Capacity = -1;
                //decimal Weight = -1;
                //int ColorID = -1;
                //int PatinaID = -1;
                ////int CoverID = -1;
                //int FactoryID = 1;
                //string Notes = string.Empty;

                //if (senderGrid.SelectedRows[0].Cells["Width"].Value != DBNull.Value)
                //    Width = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Width"].Value);
                //if (senderGrid.SelectedRows[0].Cells["Thickness"].Value != DBNull.Value)
                //    Thickness = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Thickness"].Value);
                //if (senderGrid.SelectedRows[0].Cells["Diameter"].Value != DBNull.Value)
                //    Diameter = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["Diameter"].Value);
                //if (senderGrid.SelectedRows[0].Cells["StoreItemID"].Value != DBNull.Value)
                //    StoreItemID = Convert.ToInt32(senderGrid.SelectedRows[0].Cells["StoreItemID"].Value);

                if (OKSplit)
                {
                    ProfileAssignmentsManager.AddReturnRoller(DecorAssignmentID, NewDiameter, NewWidth, NewCount);
                    ProfileAssignmentsManager.SaveTransferredRollers();
                    int ReturnedRollerID = ProfileAssignmentsManager.GetTransferredRollers(DecorAssignmentID);

                    //ProfileAssignmentsManager.AssignmentsStoreManager.AddToManufactureStore(DecorAssignmentID, StoreItemID, Length, NewWidth, Height, Thickness,
                    //    NewDiameter, Admission, Capacity, Weight, ColorID, PatinaID, -1, NewCount, FactoryID, Notes, ReturnedRollerID);
                    ProfileAssignmentsManager.GetRollersInColorOnStorage(CoverID);
                    InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                }
            }
            if (senderGrid.Columns[e.ColumnIndex].Name == "Column2" && e.RowIndex >= 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Продолжить?",
                        "Удаление");

                if (!OKCancel)
                    return;
                ProfileAssignmentsManager.RemoveTransferredRoller();
                ProfileAssignmentsManager.SaveTransferredRollers();
                ProfileAssignmentsManager.GetTransferredRollers(DecorAssignmentID);
                ProfileAssignmentsManager.GetRollersInColorOnStorage(CoverID);
                InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
            }
        }

        private void dgvReturnedRollers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid senderGrid = (PercentageDataGrid)sender;

            if (senderGrid.SelectedRows.Count == 0)
                return;

            //if (senderGrid.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0)
            //{
            //    ProfileAssignmentsManager.ReturnRoller();
            //}
            if (senderGrid.Columns[e.ColumnIndex].Name == "Column2" && e.RowIndex >= 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Продолжить?",
                        "Удаление");

                if (!OKCancel)
                    return;
                ProfileAssignmentsManager.RemoveReturnedRoller();
                ProfileAssignmentsManager.SaveTransferredRollers();
                ProfileAssignmentsManager.GetTransferredRollers(DecorAssignmentID);
                ProfileAssignmentsManager.GetRollersInColorOnStorage(CoverID);
                InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
            }
        }
    }
}
