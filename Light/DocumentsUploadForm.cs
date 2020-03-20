using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DocumentsUploadForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm;

        InfiniumDocuments InfiniumDocuments;

        public bool bStopTransfer = false;

        public int InnerDocumentID = -1;

        public long TotalSize = 0;
        public long Position = 0;

        public int DocumentID = -1;
        public int DocumentCategoryID = -1;
        DataTable FilesDataTable = null;

        int CurrentFile = 0;
        int DocumentFileID = -1;
        public string sCommentsText = "";
        int DocumentCommentFileID = -1;
        public DataTable CurrentFilesDataTable = null;

        public static bool bOK = true;

        public DocumentsUploadForm(ref Form tTopForm, ref InfiniumDocuments tInfiniumDocuments, string sText, DataTable tFilesDataTable, int iDocumentID, int iDocumentCategoryID)
        {
            InitializeComponent();

            DocumentID = iDocumentID;
            DocumentCategoryID = iDocumentCategoryID;
            FilesDataTable = tFilesDataTable.Copy();

            InfiniumDocuments = tInfiniumDocuments;
            sCommentsText = sText;
            TopForm = tTopForm;

            StartUpload();
        }

        public DocumentsUploadForm(ref Form tTopForm, ref InfiniumDocuments tInfiniumDocuments, string sText, DataTable tFilesDataTable, int iDocumentCommentID)
        {
            InitializeComponent();

            FilesDataTable = tFilesDataTable.Copy();

            InfiniumDocuments = tInfiniumDocuments;
            sCommentsText = sText;
            TopForm = tTopForm;

            StartUpload(iDocumentCommentID);
        }

        public DocumentsUploadForm(ref Form tTopForm, ref InfiniumDocuments tInfiniumDocuments, int iDocumentFileID)
        {
            InitializeComponent();


            InfiniumDocuments = tInfiniumDocuments;
            TopForm = tTopForm;

            DocumentFileID = iDocumentFileID;

            StartDownload(iDocumentFileID);
        }

        public DocumentsUploadForm(ref Form tTopForm, ref InfiniumDocuments tInfiniumDocuments, int iDocumentCommentFileID, bool bComment)
        {
            InitializeComponent();


            InfiniumDocuments = tInfiniumDocuments;
            TopForm = tTopForm;

            DocumentCommentFileID = iDocumentCommentFileID;

            StartDownloadComment(iDocumentCommentFileID);
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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        Thread LoadThread;


        public void StartUpload()
        {
            UploadTimer.Enabled = true;

            InfiniumDocuments.FM.bStopTransfer = false;

            foreach (DataRow Row in FilesDataTable.Select("IsNew = 1"))
            {
                TotalSize += Convert.ToInt64(Row["FileSize"]);
            }


            LoadThread = new Thread(delegate ()
            {
                InfiniumDocuments.AddComment(Security.CurrentUserID, sCommentsText, DocumentID, DocumentCategoryID, FilesDataTable, ref CurrentFile);
            });
            LoadThread.Start();
        }

        public void StartDownload(int DocumentFileID)
        {
            StatusLabel.Text = "Загрузка файла...";

            DownloadTimer.Enabled = true;

            InfiniumDocuments.FM.bStopTransfer = false;


            LoadThread = new Thread(delegate ()
            {
                InfiniumDocuments.OpenFile(DocumentFileID);
            });
            LoadThread.Start();
        }

        public void StartDownloadComment(int DocumentCommentFileID)
        {
            StatusLabel.Text = "Загрузка файла...";

            DownloadTimer.Enabled = true;

            InfiniumDocuments.FM.bStopTransfer = false;


            LoadThread = new Thread(delegate ()
            {
                InfiniumDocuments.OpenCommentFile(DocumentCommentFileID);
            });
            LoadThread.Start();
        }

        public void StartUpload(int iDocumentCommentID)
        {
            UploadTimer.Enabled = true;

            InfiniumDocuments.FM.bStopTransfer = false;

            foreach (DataRow Row in FilesDataTable.Select("IsNew = 1"))
            {
                TotalSize += Convert.ToInt64(Row["FileSize"]);
            }

            LoadThread = new Thread(delegate ()
            {
                InfiniumDocuments.EditComment(iDocumentCommentID, sCommentsText, FilesDataTable, ref CurrentFile);
            });
            LoadThread.Start();
        }


        long CurPosition = 0;
        int iCur = 0;

        private void UploadTimer_Tick(object sender, EventArgs e)
        {
            if (bStopTransfer)
            {
                InfiniumDocuments.FM.bStopTransfer = true;
                bStopTransfer = false;
                UploadTimer.Enabled = false;

                StatusLabel.Text = "Отмена...";

                Application.DoEvents();

                //remove files
                Thread T = new Thread(delegate ()
                {
                    InfiniumDocuments.RemoveUploadedFiles();
                });
                T.Start();

                while (T.IsAlive) ;

                LoadThread.Abort();

                Application.DoEvents();

                FormEvent = eClose;
                AnimateTimer.Enabled = true;
            }



            if (TotalSize == 0)
            {
                UploadTimer.Enabled = false;
                this.Close();
                return;
            }

            if (CurrentFile != iCur)
            {
                CurPosition = 0;
                iCur = CurrentFile;
            }

            CurPosition = InfiniumDocuments.FM.Position - CurPosition;
            Position += CurPosition;
            CurPosition = InfiniumDocuments.FM.Position;

            if (Position == TotalSize || Position > TotalSize)
            {
                ProgressBar.Value = 100;
                UploadTimer.Enabled = false;
                this.Close();
                return;
            }

            ProgressBar.Value = Convert.ToInt32(100 * Position / TotalSize);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            bOK = false;
            bStopTransfer = true;
        }

        private void DownloadTimer_Tick(object sender, EventArgs e)
        {
            if (bStopTransfer)
            {
                InfiniumDocuments.FM.bStopTransfer = true;
                bStopTransfer = false;
                DownloadTimer.Enabled = false;

                StatusLabel.Text = "Отмена...";

                Application.DoEvents();

                LoadThread.Abort();

                Application.DoEvents();

                FormEvent = eClose;
                AnimateTimer.Enabled = true;
            }


            if (InfiniumDocuments.FM.TotalFileSize == 0)
            {
                DownloadTimer.Enabled = false;
                this.Close();
                return;
            }

            if (InfiniumDocuments.FM.Position == InfiniumDocuments.FM.TotalFileSize || InfiniumDocuments.FM.Position > InfiniumDocuments.FM.TotalFileSize)
            {
                ProgressBar.Value = 100;
                DownloadTimer.Enabled = false;
                this.Close();
                return;
            }

            ProgressBar.Value = Convert.ToInt32(100 * InfiniumDocuments.FM.Position / InfiniumDocuments.FM.TotalFileSize);
        }

    }
}
