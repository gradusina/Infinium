using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddMarketingNewsForm : Form
    {
        Infinium.MarketingNews LightNews;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        bool bStopTransfer = false;

        int FormEvent = 0;

        public bool Canceled = false;

        public int iNewsIDEdit = -1;

        public int AttachsCount = 0;

        Form TopForm;

        public DateTime DateTime;

        public DataTable AttachmentsDataTable;
        public BindingSource AttachmentsBindingSource;



        public AddMarketingNewsForm(ref Infinium.MarketingNews tLightNews, ref Form tTopForm)
        {
            InitializeComponent();

            TopForm = tTopForm;
            LightNews = tLightNews;

            CreateAttachments();

            foreach (DataRow Row in LightNews.ClientsSelectDataTable.Rows)
            {
                Row["Check"] = false;
            }

            ClientsDataGrid.DataSource = LightNews.ClientsSelectDataTable;
            ClientsDataGrid.Columns["UserTypeID"].Visible = false;
            ClientsDataGrid.Columns["ID"].Visible = false;
            ClientsDataGrid.Columns["Check"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsDataGrid.Columns["Check"].Width = 30;
            ClientsDataGrid.Columns["Check"].DisplayIndex = 0;
            ClientsDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public AddMarketingNewsForm(ref Infinium.MarketingNews tLightNews, int SenderTypeID, string HeaderText, string BodyText, int iNewsID, DateTime dDateTime, ref Form tTopForm)
        {
            InitializeComponent();

            LightNews = tLightNews;

            TopForm = tTopForm;

            HeaderTextEdit.Text = HeaderText;
            BodyTextEdit.Text = BodyText;

            iNewsIDEdit = iNewsID;
            DateTime = dDateTime;

            CreateAttachments();
            CopyAttachs(iNewsID);
        }

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
            using (DataTable DT = LightNews.GetAttachments(NewsID))
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

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            //if (HeaderTextEdit.Text.Length < 1 || HeaderTextEdit.Text.Length > 1024)
            //    return;

            bool N = false;

            foreach (DataRow Row in LightNews.ClientsSelectDataTable.Rows)
            {
                if (Convert.ToBoolean(Row["Check"]) == true)
                {
                    N = true;
                    break;
                }
            }

            if (!cbNewsToSite.Checked)
                if (N == false)
                {
                    InfiniumTips.ShowTip(this, 50, 93, "Не выбраны клиенты", 3000);
                    return;
                }

            if (BodyTextEdit.Text.Length < 1)
            {
                InfiniumTips.ShowTip(this, 50, 93, "Не введено содержание", 3000);
                return;
            }

            if (iNewsIDEdit == -1)
            {
                if (!cbNewsToSite.Checked)
                {
                    foreach (DataRow Row in LightNews.ClientsSelectDataTable.Rows)
                    {
                        if (Convert.ToBoolean(Row["Check"]) == false)
                            continue;

                        DateTime Date = new DateTime();

                        LoadLabel.Text = "Создание новости...";
                        OKNewsButton.Enabled = false;
                        CancelNewsButton.Enabled = false;
                        AttachmentsGrid.ReadOnly = true;
                        BodyTextEdit.Enabled = false;
                        AttachButton.Enabled = false;
                        DetachButton.Enabled = false;
                        HeaderTextEdit.Enabled = false;

                        Application.DoEvents();

                        if (Convert.ToInt32(Row["UserTypeID"]) == 0) //новость для клиентов
                            Date = LightNews.AddNews(Security.CurrentUserID, 0, HeaderTextEdit.Text, BodyTextEdit.Text, Convert.ToInt32(Row["ID"]), 2);
                        if (Convert.ToInt32(Row["UserTypeID"]) == 1) //новость для менеджеров
                            Date = LightNews.AddNews(Security.CurrentUserID, 0, HeaderTextEdit.Text, BodyTextEdit.Text, Convert.ToInt32(Row["ID"]), 3);

                        bool Ok = false;

                        bStopTransfer = false;
                        LightNews.FM.bStopTransfer = false;

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

                            Thread T = new Thread(delegate () { Ok = LightNews.Attach(AttachmentsDataTable, Date, ref CurrentUploadedFile, Convert.ToInt32(Row["ID"])); });
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
                                    LightNews.FM.bStopTransfer = true;
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

                                    LightNews.RemoveAttachments(LightNews.GetNewsIDByDateTime(Date));

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

                        LightNews.ClearPending(Date, Convert.ToInt32(Row["ID"]));

                        if (Convert.ToInt32(Row["UserTypeID"]) == 0) //для клиентов
                            LightNews.AddSubscribeForNews(Date, 2, Convert.ToInt32(Row["ID"]));
                        if (Convert.ToInt32(Row["UserTypeID"]) == 1) //для менеджеров
                            LightNews.AddSubscribeForNews(Date, 3, Convert.ToInt32(Row["ID"]));

                    }
                }
                else
                {
                    DateTime Date = new DateTime();

                    LoadLabel.Text = "Создание новости...";
                    OKNewsButton.Enabled = false;
                    CancelNewsButton.Enabled = false;
                    AttachmentsGrid.ReadOnly = true;
                    BodyTextEdit.Enabled = false;
                    AttachButton.Enabled = false;
                    DetachButton.Enabled = false;
                    HeaderTextEdit.Enabled = false;

                    Application.DoEvents();

                    Date = LightNews.AddNews(Security.CurrentUserID, 0, HeaderTextEdit.Text, BodyTextEdit.Text, -1, -1);

                    LightNews.ClearPending(Date, -1);

                    LightNews.AddSubscribeForNews(Date, -1, -1);
                }
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
                HeaderTextEdit.Enabled = false;

                Application.DoEvents();

                LightNews.EditNews(iNewsIDEdit, Security.CurrentUserID, 0, HeaderTextEdit.Text, BodyTextEdit.Text, -1, DateTime);


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

                    Thread T = new Thread(delegate () { Ok = LightNews.EditAttachments(iNewsIDEdit, AttachmentsDataTable, ref CurrentUploadedFile, ref TotalFilesCount); });
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
                            LightNews.FM.bStopTransfer = true;
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

                            LightNews.RemoveCurrentAttachments(LightNews.GetNewsIDByDateTime(DateTime), AttachmentsDataTable);

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
            if (LightNews.FM.TotalFileSize == 0)
                return;

            if (LightNews.FM.Position == LightNews.FM.TotalFileSize || LightNews.FM.Position > LightNews.FM.TotalFileSize)
            {
                ProgressBar.Value = 100;
                return;
            }



            DownloadLabel.Text = FileManager.GetIntegerWithThousands(Convert.ToInt32(LightNews.FM.Position / 1024)) + " / " +
                                   FileManager.GetIntegerWithThousands(Convert.ToInt32((LightNews.FM.TotalFileSize / 1024))) + " КБайт";

            SpeedLabel.Text = FileManager.GetIntegerWithThousands(Convert.ToInt32(LightNews.FM.CurrentSpeed)) + " КБайт/c";

            ProgressBar.Value = Convert.ToInt32(100 * LightNews.FM.Position / LightNews.FM.TotalFileSize);
            PercentsLabel.Text = ProgressBar.Value.ToString() + " %";
        }

        private void CancelLoadingFilesButton_Click(object sender, EventArgs e)
        {
            bStopTransfer = true;
        }

        private void AllClientsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataRow Row in LightNews.ClientsSelectDataTable.Rows)
            {
                if (AllClientsCheckBox.Checked)
                    Row["Check"] = true;
                else
                    Row["Check"] = false;
            }
        }

        private void cbNewsToSite_CheckedChanged(object sender, EventArgs e)
        {
            AttachButton.Enabled = !cbNewsToSite.Checked;
            ClientsDataGrid.Enabled = !cbNewsToSite.Checked;
            AttachmentsGrid.Enabled = !cbNewsToSite.Checked;
            DetachButton.Enabled = !cbNewsToSite.Checked;
            AllClientsCheckBox.Enabled = !cbNewsToSite.Checked;

            foreach (DataRow Row in LightNews.ClientsSelectDataTable.Rows)
            {
                Row["Check"] = false;
            }
        }
    }
}
