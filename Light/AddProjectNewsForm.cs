using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddProjectNewsForm : Form
    {
        Infinium.InfiniumProjects InfiniumProjects;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        bool bStopTransfer = false;

        int FormEvent = 0;

        public bool Canceled = false;

        public int iNewsIDEdit = -1;

        public int AttachsCount = 0;

        Form TopForm;

        public int ProjectID = -1;

        public DateTime DateTime;

        public DataTable AttachmentsDataTable;
        public BindingSource AttachmentsBindingSource;

        private void CreateAttachments()
        {
            AttachmentsDataTable = new DataTable();
            AttachmentsDataTable.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachmentsDataTable.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            AttachmentsBindingSource = new BindingSource()
            {
                DataSource = AttachmentsDataTable
            };
            AttachmentsGrid.DataSource = AttachmentsBindingSource;

            AttachmentsGrid.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            AttachmentsGrid.Columns["FileName"].Width = 250;
        }

        private void CopyAttachs(int NewsID)
        {
            using (DataTable DT = InfiniumProjects.GetAttachments(NewsID))
            {
                if (DT.Rows.Count == 0)
                    return;

                AttachsCount = DT.Rows.Count;

                foreach (DataRow Row in DT.Rows)
                {
                    DataRow NewRow = AttachmentsDataTable.NewRow();
                    NewRow["FileName"] = Row["FileName"];
                    NewRow["Path"] = "server";
                    AttachmentsDataTable.Rows.Add(NewRow);
                }
            }
        }

        public AddProjectNewsForm(ref Infinium.InfiniumProjects tInfiniumProjects, int iProjectID, ref Form tTopForm)
        {
            InitializeComponent();

            ProjectID = iProjectID;

            TopForm = tTopForm;
            InfiniumProjects = tInfiniumProjects;

            CreateAttachments();
        }

        public AddProjectNewsForm(ref Infinium.InfiniumProjects tInfiniumProjects, int iProjectID, string BodyText, int iNewsID, DateTime dDateTime, ref Form tTopForm)
        {
            InitializeComponent();

            InfiniumProjects = tInfiniumProjects;

            TopForm = tTopForm;

            BodyTextEdit.Text = BodyText;

            ProjectID = iProjectID;
            iNewsIDEdit = iNewsID;
            DateTime = dDateTime;

            AllUsersNotifyCheckBox.Enabled = false;
            AllUsersNotifyCheckBox.StateCommon.ShortText.Color1 = Color.LightGray;
            NoNotifyCheckBox.Enabled = false;
            NoNotifyCheckBox.StateCommon.ShortText.Color1 = Color.LightGray;


            CreateAttachments();
            CopyAttachs(iNewsID);
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            if (BodyTextEdit.Text.Length < 1)
                return;

            int DepartmentID = Convert.ToInt32(InfiniumProjects.UsersDataTable.Select("UserID = " + Security.CurrentUserID)[0]["DepartmentID"]);


            if (iNewsIDEdit == -1)
            {
                DateTime Date;

                LoadLabel.Text = "Создание новости...";
                OKNewsButton.Enabled = false;
                CancelNewsButton.Enabled = false;
                AttachmentsGrid.ReadOnly = true;
                BodyTextEdit.Enabled = false;
                AttachButton.Enabled = false;
                DetachButton.Enabled = false;

                Application.DoEvents();

                Date = InfiniumProjects.AddNews(Security.CurrentUserID, ProjectID, BodyTextEdit.Text);

                bool Ok = false;

                if (AttachmentsBindingSource.Count > 0)
                {
                    LoadLabel.Visible = true;
                    ProgressBar.Visible = true;
                    //LoadLabel.Text = "Загрузка прикрепленых файлов...";
                    LoadTimer.Enabled = true;
                    CancelLoadingFilesButton.Visible = true;

                    Application.DoEvents();

                    int LastUploadedFile = 0;
                    int CurrentUploadedFile = 0;
                    int TotalFilesCount = AttachmentsDataTable.Rows.Count;

                    Thread T = new Thread(delegate () { Ok = InfiniumProjects.Attach(AttachmentsDataTable, Date, ref CurrentUploadedFile, ProjectID); });
                    T.Start();

                    this.Activate();
                    Application.DoEvents();

                    while (T.IsAlive)
                    {
                        T.Join(50);
                        Application.DoEvents();

                        if (CurrentUploadedFile != LastUploadedFile)
                        {
                            LoadLabel.Text = "Загрузка прикрепленых файлов(" + CurrentUploadedFile.ToString() + " из " + TotalFilesCount.ToString() + ")";
                            LastUploadedFile = CurrentUploadedFile;
                        }

                        if (bStopTransfer)
                        {
                            InfiniumProjects.FM.bStopTransfer = true;
                            bStopTransfer = false;
                            LoadTimer.Enabled = false;
                            ProgressBar.Visible = false;
                            LoadLabel.Text = "Отмена загрузки файлов...";
                            DownloadLabel.Text = "";
                            SpeedLabel.Text = "";
                            PercentsLabel.Text = "";
                            CancelLoadingFilesButton.Visible = false;

                            Application.DoEvents();

                            while (T.IsAlive)
                                Thread.Sleep(50);

                            InfiniumProjects.RemoveAttachments(InfiniumProjects.GetNewsIDByDateTime(Date));

                            FormEvent = eClose;
                            AnimateTimer.Enabled = true;
                        }
                    }

                    LoadTimer.Enabled = false;

                    if (!Ok)
                    {
                        MessageBox.Show("Один или несколько файлов не были найдены или отсутствовал доступ");
                        return;
                    }
                }

                InfiniumProjects.ClearNewsPending(Date);

                if (!NoNotifyCheckBox.Checked)
                    InfiniumProjects.AddNewsSubscribe(Date, ProjectID, AllUsersNotifyCheckBox.Checked);
            }
            else
            {
                LoadLabel.Text = "Обновление новости...";
                OKNewsButton.Enabled = false;
                CancelNewsButton.Enabled = false;
                AttachmentsGrid.ReadOnly = true;
                BodyTextEdit.Enabled = false;
                AttachButton.Enabled = false;
                DetachButton.Enabled = false;

                Application.DoEvents();

                InfiniumProjects.EditNews(iNewsIDEdit, BodyTextEdit.Text);
                InfiniumProjects.FillProjectNews(ProjectID);

                if (AttachsCount > 0 || AttachmentsBindingSource.Count > 0)
                {
                    bool Ok = false;


                    LoadLabel.Visible = true;
                    ProgressBar.Visible = true;
                    //LoadLabel.Text = "Загрузка прикрепленых файлов...";
                    LoadTimer.Enabled = true;
                    CancelLoadingFilesButton.Visible = true;

                    Application.DoEvents();

                    int LastUploadedFile = 0;
                    int CurrentUploadedFile = 0;
                    int TotalFilesCount = 0;

                    Thread T = new Thread(delegate () { Ok = InfiniumProjects.EditAttachments(iNewsIDEdit, ProjectID, AttachmentsDataTable, ref CurrentUploadedFile, ref TotalFilesCount); });
                    T.Start();

                    this.Activate();
                    Application.DoEvents();


                    while (T.IsAlive)
                    {
                        T.Join(50);
                        Application.DoEvents();

                        if (CurrentUploadedFile != LastUploadedFile)
                        {
                            LoadLabel.Text = "Загрузка прикрепленых файлов(" + CurrentUploadedFile.ToString() + " из " + TotalFilesCount.ToString() + ")";
                            LastUploadedFile = CurrentUploadedFile;
                        }

                        if (bStopTransfer)
                        {
                            InfiniumProjects.FM.bStopTransfer = true;
                            bStopTransfer = false;
                            LoadTimer.Enabled = false;
                            ProgressBar.Visible = false;
                            LoadLabel.Text = "Отмена загрузки файлов...";
                            DownloadLabel.Text = "";
                            SpeedLabel.Text = "";
                            PercentsLabel.Text = "";
                            CancelLoadingFilesButton.Visible = false;

                            Application.DoEvents();

                            while (T.IsAlive)
                                Thread.Sleep(50);

                            InfiniumProjects.RemoveCurrentAttachments(InfiniumProjects.GetNewsIDByDateTime(DateTime), AttachmentsDataTable);

                            FormEvent = eClose;
                            AnimateTimer.Enabled = true;
                        }
                    }

                    LoadTimer.Enabled = false;



                    if (!Ok)
                    {
                        MessageBox.Show("Один или несколько файлов не были найдены или отсутствовал доступ");
                    }
                }
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelDateButton_Click(object sender, EventArgs e)
        {
            bStopTransfer = true;

            Canceled = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void AttachButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void DetachButton_Click(object sender, EventArgs e)
        {
            if (AttachmentsBindingSource.Count == 0)
                return;

            AttachmentsBindingSource.RemoveCurrent();
            AttachmentsDataTable.AcceptChanges();
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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            foreach (string FileName in openFileDialog1.FileNames)
            {
                var fileInfo = new System.IO.FileInfo(FileName);

                DataRow NewRow = AttachmentsDataTable.NewRow();
                NewRow["FileName"] = System.IO.Path.GetFileName(FileName);
                NewRow["Path"] = FileName;
                AttachmentsDataTable.Rows.Add(NewRow);
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

        private void AddNewsForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            if (InfiniumProjects.FM.TotalFileSize == 0)
                return;

            if (InfiniumProjects.FM.Position == InfiniumProjects.FM.TotalFileSize || InfiniumProjects.FM.Position > InfiniumProjects.FM.TotalFileSize)
            {
                ProgressBar.Value = 100;
                return;
            }



            DownloadLabel.Text = FileManager.GetIntegerWithThousands(Convert.ToInt32(InfiniumProjects.FM.Position / 1024)) + " / " +
                                   FileManager.GetIntegerWithThousands(Convert.ToInt32((InfiniumProjects.FM.TotalFileSize / 1024))) + " КБайт";

            SpeedLabel.Text = FileManager.GetIntegerWithThousands(Convert.ToInt32(InfiniumProjects.FM.CurrentSpeed)) + " КБайт/c";

            ProgressBar.Value = Convert.ToInt32(100 * InfiniumProjects.FM.Position / InfiniumProjects.FM.TotalFileSize);
            PercentsLabel.Text = ProgressBar.Value.ToString() + " %";
        }

        private void CancelLoadingFilesButton_Click(object sender, EventArgs e)
        {
            bStopTransfer = true;
        }

        private void AllUsersNotifyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AllUsersNotifyCheckBox.Checked)
                NoNotifyCheckBox.Checked = false;
        }

        private void NoNotifyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (NoNotifyCheckBox.Checked)
                AllUsersNotifyCheckBox.Checked = false;
        }
    }
}
