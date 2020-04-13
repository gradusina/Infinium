using Infinium.Modules.TechnologyCatalog;

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PlannedWorkForm : Form
    {
        const int iAdmin = 89;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int _formEvent = 0;

        bool NeedSplash = false;

        Form _topForm = null;
        LightStartForm _lightStartForm;

        PlannedWork _plannedWorkManager;

        RoleTypes _roleType = RoleTypes.Ordinary;

        public enum RoleTypes
        {
            Ordinary = 0,
            Admin = 1
        }

        public PlannedWorkForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            _lightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            _plannedWorkManager = new PlannedWork();
            _plannedWorkManager.GetPermissions(Security.CurrentUserID, this.Name);
            if (_plannedWorkManager.PermissionGranted(iAdmin))
            {
                _roleType = RoleTypes.Admin;
            }

            while (!SplashForm.bCreated) ;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (_topForm != null)
                    _topForm.Activate();
            }
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (_formEvent == eClose || _formEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (_formEvent == eClose)
                    {

                        _lightStartForm.CloseForm(this);
                    }

                    if (_formEvent == eHide)
                    {

                        _lightStartForm.HideForm(this);
                    }


                    return;
                }

                if (_formEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    return;
                }

            }

            if (_formEvent == eClose || _formEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (_formEvent == eClose)
                    {

                        _lightStartForm.CloseForm(this);
                    }

                    if (_formEvent == eHide)
                    {

                        _lightStartForm.HideForm(this);
                    }

                }

                return;
            }


            if (_formEvent == eShow || _formEvent == eShow)
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

        private void PlannedWorkForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            _formEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            _formEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void PlannedWorkForm_Load(object sender, EventArgs e)
        {
            dgvGridSettings();
            UpdateWorks();
        }

        private void dgvGridSettings()
        {
            dgvUsers.DataSource = _plannedWorkManager.UsersBs;
            if (dgvUsers.Columns.Contains("UserID"))
                dgvUsers.Columns["UserID"].Visible = false;
            if (dgvUsers.Columns.Contains("ShortName"))
                dgvUsers.Columns["ShortName"].Visible = false;
            int DisplayIndex = 0;
            dgvUsers.Columns["Check"].DisplayIndex = DisplayIndex++;
            dgvUsers.Columns["Name"].DisplayIndex = DisplayIndex++;
            dgvUsers.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvUsers.Columns["Check"].Width = 45;
            dgvUsers.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvUsers.Columns["Check"].ReadOnly = false;
            dgvUsers.Columns["Name"].ReadOnly = true;

            dgvMachines.DataSource = _plannedWorkManager.MachinesBs;
            if (dgvMachines.Columns.Contains("MachineID"))
                dgvMachines.Columns["MachineID"].Visible = false;
            DisplayIndex = 0;
            dgvMachines.Columns["Check"].DisplayIndex = DisplayIndex++;
            dgvMachines.Columns["MachineName"].DisplayIndex = DisplayIndex++;
            dgvMachines.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMachines.Columns["Check"].Width = 45;
            dgvMachines.Columns["MachineName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMachines.Columns["Check"].ReadOnly = false;
            dgvMachines.Columns["MachineName"].ReadOnly = true;

            dgvExecutors.DataSource = _plannedWorkManager.ExecutorsBs;
            dgvExecutors.Columns.Add(_plannedWorkManager.ExecutorsColumn);
            if (dgvExecutors.Columns.Contains("PlannedWorkUserID"))
                dgvExecutors.Columns["PlannedWorkUserID"].Visible = false;
            if (dgvExecutors.Columns.Contains("UserID"))
                dgvExecutors.Columns["UserID"].Visible = false;
            if (dgvExecutors.Columns.Contains("PlannedWorkID"))
                dgvExecutors.Columns["PlannedWorkID"].Visible = false;
            if (dgvExecutors.Columns.Contains("CreateDate"))
                dgvExecutors.Columns["CreateDate"].Visible = false;
            if (dgvExecutors.Columns.Contains("CreateUserID"))
                dgvExecutors.Columns["CreateUserID"].Visible = false;
            dgvExecutors.Columns["ExecutorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvFiles.DataSource = _plannedWorkManager.FilesBs;
            if (dgvFiles.Columns.Contains("PlannedWorkFileID"))
                dgvFiles.Columns["PlannedWorkFileID"].Visible = false;
            if (dgvFiles.Columns.Contains("FileSize"))
                dgvFiles.Columns["FileSize"].Visible = false;
            if (dgvFiles.Columns.Contains("PlannedWorkID"))
                dgvFiles.Columns["PlannedWorkID"].Visible = false;
            if (dgvFiles.Columns.Contains("CreateDate"))
                dgvFiles.Columns["CreateDate"].Visible = false;
            if (dgvFiles.Columns.Contains("CreateUserID"))
                dgvFiles.Columns["CreateUserID"].Visible = false;
            dgvFiles.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvWorks.DataSource = _plannedWorkManager.PlannedWorksBs;
            dgvWorks.Columns.Add(_plannedWorkManager.MachinesColumn);
            dgvWorks.Columns.Add(_plannedWorkManager.StatusesColumn);
            if (dgvWorks.Columns.Contains("CreateUserID"))
                dgvWorks.Columns["CreateUserID"].Visible = false;
            if (dgvWorks.Columns.Contains("ConfirmUserID"))
                dgvWorks.Columns["ConfirmUserID"].Visible = false;
            if (dgvWorks.Columns.Contains("StartUserID"))
                dgvWorks.Columns["StartUserID"].Visible = false;
            if (dgvWorks.Columns.Contains("EndUserID"))
                dgvWorks.Columns["EndUserID"].Visible = false;
            if (dgvWorks.Columns.Contains("MachineID"))
                dgvWorks.Columns["MachineID"].Visible = false;
            if (dgvWorks.Columns.Contains("PlannedWorkStatusID"))
                dgvWorks.Columns["PlannedWorkStatusID"].Visible = false;

            foreach (DataGridViewColumn Column in dgvWorks.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.ReadOnly = true;
            }

            dgvWorks.Columns["PlannedWorkID"].HeaderText = "№п/п";
            dgvWorks.Columns["CreateDate"].HeaderText = "Дата оформления";
            dgvWorks.Columns["ConfirmDate"].HeaderText = "Дата утверждения";
            dgvWorks.Columns["StartDate"].HeaderText = "Дата начала";
            dgvWorks.Columns["Description"].HeaderText = "Описание";
            dgvWorks.Columns["PlannedTime"].HeaderText = "Плановое время";
            dgvWorks.Columns["EndDate"].HeaderText = "Время окончания";
            dgvWorks.Columns["Notes"].HeaderText = "Комментарий";
            DisplayIndex = 0;
            dgvWorks.Columns["PlannedWorkID"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["CreateDate"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["ConfirmDate"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["StartDate"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["Description"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["StatusesColumn"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["MachineColumn"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["PlannedTime"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["EndDate"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["Notes"].DisplayIndex = DisplayIndex++;
            dgvWorks.Columns["Description"].ReadOnly = false;
            dgvWorks.Columns["MachineColumn"].ReadOnly = false;
            dgvWorks.Columns["PlannedTime"].ReadOnly = false;
            dgvWorks.Columns["Notes"].ReadOnly = false;
        }

        private void UpdateWorks()
        {
            int dateType = 0;

            if (rbStartDate.Checked)
                dateType = 0;
            if (rbCreateDate.Checked)
                dateType = 1;
            if (rbEndDate.Checked)
                dateType = 2;
            _plannedWorkManager.UpdateWorks(cbMachines.Checked, cbUsers.Checked, cbNew.Checked, cbConfirm.Checked, cbInProduction.Checked, cbEnd.Checked,
                cbDate.Checked, dateType, CalendarFrom.SelectionStart, CalendarTo.SelectionStart);
        }

        private void btnSaveWorks_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvWorks.Rows.Count; i++)
            {
                if (dgvWorks.Rows[i].Cells["MachineID"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref _topForm, false,
                        "Не выбран станок", "Ошибка сохранения");
                    return;
                }
                if (dgvWorks.Rows[i].Cells["Description"].Value == DBNull.Value)
                {
                    Infinium.LightMessageBox.Show(ref _topForm, false,
                        "Нет описания", "Ошибка сохранения");
                    return;
                }
            }

            if (NeedSplash)
            {
                Thread T =
                new Thread(
                    delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                _plannedWorkManager.SaveExecutors();
                _plannedWorkManager.SaveWorks();
                UpdateWorks();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                _plannedWorkManager.SaveExecutors();
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
            }

            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);

        }

        private void dgvWorks_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["CreateDate"].Value = DateTime.Now;
            e.Row.Cells["CreateUserID"].Value = Security.CurrentUserID;
        }

        private void dgvWorks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string colName = dgvWorks.Columns[e.ColumnIndex].Name;
        }

        private void btnRemoveWork_Click(object sender, EventArgs e)
        {
            if (dgvWorks.SelectedRows.Count == 0)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref _topForm, true,
                "Продолжить удаление?",
                "Удаление позиции");
            if (!OKCancel)
                return;

            if (NeedSplash)
            {
                Thread T =
                new Thread(
                    delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                int PlannedWorkID = -1;
                if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                    PlannedWorkID = Convert.ToInt32((dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value));
                _plannedWorkManager.RemoveWork();

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                int PlannedWorkID = -1;
                if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                    PlannedWorkID = Convert.ToInt32((dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value));
                _plannedWorkManager.RemoveWork();
            }

            InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            CommonMenuPanel.Visible = !CommonMenuPanel.Visible;
        }

        private void cbMachines_CheckedChanged(object sender, EventArgs e)
        {
            cbSelectAllMachines.Visible = cbMachines.Checked;
            panel7.Enabled = cbMachines.Checked;
        }

        private void cbSelectAllMachines_CheckedChanged(object sender, EventArgs e)
        {
            _plannedWorkManager.SelectAllMachines(cbSelectAllMachines.Checked);
        }

        private void cbDate_CheckedChanged(object sender, EventArgs e)
        {
            panel12.Enabled = cbDate.Checked;
            CalendarFrom.Enabled = cbDate.Checked;
            CalendarTo.Enabled = cbDate.Checked;
        }

        private void cbSelectAllUsers_CheckedChanged(object sender, EventArgs e)
        {
            _plannedWorkManager.SelectAllUsers(cbSelectAllUsers.Checked);
        }

        private void cbUsers_CheckedChanged(object sender, EventArgs e)
        {
            cbSelectAllUsers.Visible = cbUsers.Checked;
            panel4.Enabled = cbUsers.Checked;
        }

        private void btnFilterWorks_Click(object sender, EventArgs e)
        {
            if (NeedSplash)
            {
                Thread T =
                new Thread(
                    delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Обновление данных.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                int PlannedWorkID = 0;

                if (dgvWorks.SelectedRows.Count > 0 && dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                    PlannedWorkID = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                int PlannedWorkID = 0;

                if (dgvWorks.SelectedRows.Count > 0 && dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                    PlannedWorkID = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);
            }
        }

        private void dgvWorks_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new System.Drawing.Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (dgvWorks.SelectedRows.Count == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref _topForm, true,
                "Вы собираетесь утвердить работу. Продолжить?",
                "Утверждение");
            if (!OKCancel)
                return;

            int PlannedWorkID = 0;

            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                PlannedWorkID = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            if (PlannedWorkID == 0)
                return;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                _plannedWorkManager.Confirm(PlannedWorkID);
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                _plannedWorkManager.Confirm(PlannedWorkID);
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);
            }
            InfiniumTips.ShowTip(this, 50, 85, "Работа утверждена", 1700);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (dgvWorks.SelectedRows.Count == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref _topForm, true,
                "Вы собираетесь начать работу. Продолжить?",
                "Выполнение");
            if (!OKCancel)
                return;

            int PlannedWorkID = 0;

            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                PlannedWorkID = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            if (PlannedWorkID == 0)
                return;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                _plannedWorkManager.Start(PlannedWorkID);
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                _plannedWorkManager.Start(PlannedWorkID);
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);
            }
            InfiniumTips.ShowTip(this, 50, 85, "Работа начата", 1700);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            if (dgvWorks.SelectedRows.Count == 0)
                return;
            bool OKCancel = Infinium.LightMessageBox.Show(ref _topForm, true,
                "Вы собираетесь завершить работу. Продолжить?",
                "Завершение");
            if (!OKCancel)
                return;

            int PlannedWorkID = 0;

            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                PlannedWorkID = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            if (PlannedWorkID == 0)
                return;

            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref _topForm, "Сохранение данных.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                NeedSplash = false;

                _plannedWorkManager.End(PlannedWorkID);
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);

                NeedSplash = true;
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                _plannedWorkManager.End(PlannedWorkID);
                _plannedWorkManager.SaveWorks();
                UpdateWorks();
                _plannedWorkManager.MoveToPosition(PlannedWorkID);
            }
            InfiniumTips.ShowTip(this, 50, 85, "Работа завершена", 1700);
        }

        private void dgvWorks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvWorks.SelectedRows.Count == 0)
            {
                _plannedWorkManager.UpdateExecutors(0);
                _plannedWorkManager.UpdateFiles(0);
                return;
            }
            int plannedWorkId = 0;
            int plannedWorkStatusId = 0;
            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                plannedWorkId = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkStatusID"].Value != DBNull.Value)
                plannedWorkStatusId = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkStatusID"].Value);
            if (NeedSplash)
            {
                Thread T =
                    new Thread(
                        delegate ()
                        {
                            SplashWindow.CreateSmallSplash(ref _topForm, "Обновление данных.\r\nПодождите...");
                        });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                _plannedWorkManager.UpdateExecutors(plannedWorkId);
                _plannedWorkManager.UpdateFiles(plannedWorkId);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                _plannedWorkManager.UpdateExecutors(plannedWorkId);
                _plannedWorkManager.UpdateFiles(plannedWorkId);
            }

            if (_roleType == RoleTypes.Ordinary)
            {
                btnConfirm.Enabled = false;
                btnStart.Enabled = false;
                btnEnd.Enabled = false;
            }
            else
            {
                switch (plannedWorkStatusId)
                {
                    case 1:
                        btnConfirm.Enabled = true;
                        btnStart.Enabled = false;
                        btnEnd.Enabled = false;
                        break;
                    case 2:
                        btnConfirm.Enabled = false;
                        btnStart.Enabled = true;
                        btnEnd.Enabled = false;
                        break;
                    case 3:
                        btnConfirm.Enabled = false;
                        btnStart.Enabled = false;
                        btnEnd.Enabled = true;
                        break;
                    case 4:
                        btnConfirm.Enabled = false;
                        btnStart.Enabled = false;
                        btnEnd.Enabled = false;
                        break;
                    default:
                        btnConfirm.Enabled = false;
                        btnStart.Enabled = false;
                        btnEnd.Enabled = false;
                        break;
                }
            }
        }

        private void btnRemoveExecutors_Click(object sender, EventArgs e)
        {
            if (dgvExecutors.SelectedRows.Count == 0)
                return;
            _plannedWorkManager.RemoveExecutor();
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (dgvFiles.SelectedRows.Count == 0 || dgvFiles.SelectedRows[0].Cells["PlannedWorkFileID"].Value == DBNull.Value)
                return;
            int PlannedWorkFileID = Convert.ToInt32(dgvFiles.SelectedRows[0].Cells["PlannedWorkFileID"].Value);
            int plannedWorkId = 0;
            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                plannedWorkId = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            bool bOk = _plannedWorkManager.RemoveFile(PlannedWorkFileID);
            _plannedWorkManager.UpdateFiles(plannedWorkId);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Файл удален", 1700);
        }

        private void dgvExecutors_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            int plannedWorkId = 0;
            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                plannedWorkId = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            if (plannedWorkId > 0)
            {
                e.Row.Cells["PlannedWorkID"].Value = plannedWorkId;
                e.Row.Cells["CreateDate"].Value = DateTime.Now;
                e.Row.Cells["CreateUserID"].Value = Security.CurrentUserID;
            }
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            if (dgvWorks.SelectedRows.Count == 0 || dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value == null)
                return;
            openFileDialog4.ShowDialog();
        }

        private void openFileDialog4_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int plannedWorkId = 0;
            if (dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value != DBNull.Value)
                plannedWorkId = Convert.ToInt32(dgvWorks.SelectedRows[0].Cells["PlannedWorkID"].Value);
            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            if (fileInfo.Length > 1500000)
            {
                MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
                return;
            }
            bool bOk = _plannedWorkManager.AttachFile(Path.GetFileNameWithoutExtension(openFileDialog4.FileName),
                fileInfo.Extension, openFileDialog4.FileName, plannedWorkId);
            _plannedWorkManager.UpdateFiles(plannedWorkId);
            if (bOk)
                InfiniumTips.ShowTip(this, 50, 85, "Файл прикреплен", 1700);
        }
    }
}
