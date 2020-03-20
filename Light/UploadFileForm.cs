using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class UploadFileForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        public bool Canceled = false;

        Form TopForm;

        Infinium.InfiniumFiles InfiniumFiles;

        FileManager FM;

        //InfiniumDocuments InfiniumDocuments;

        int LastUploadedFile = 0;
        int CurrentUploadedFile = 0;
        int TotalFilesCount = 0;

        bool bStopTransfer = false;

        string[] FileNames;
        string FileName = "";

        int FolderID = -1;
        int FileID = -1;

        bool bUpload = false;
        bool bSave = false;
        bool bDownload = false;
        bool bMultiple = false;
        bool bReplace = false;

        string SaveToPath = "";

        public int bOk = 5;

        DataTable ItemsDataTable;



        public UploadFileForm(ref FileManager tFM, ref Infinium.InfiniumFiles tInfiniumDocuments, string[] sFileNames, int iFolderID, ref Form tTopForm)//upload
        {
            InitializeComponent();

            FM = tFM;

            FileNames = sFileNames;

            TotalFilesCount = sFileNames.Count();

            FolderID = iFolderID;

            TopForm = tTopForm;
            InfiniumFiles = tInfiniumDocuments;

            bUpload = true;
        }

        public UploadFileForm(ref FileManager tFM, ref Infinium.InfiniumFiles tInfiniumDocuments, string sFileName, int iFolderID, int iFileID,
                              bool Save, string sSaveToPath, ref Form tTopForm)//download
        {
            InitializeComponent();

            FM = tFM;

            FileName = sFileName;

            TotalFilesCount = 1;

            FolderID = iFolderID;
            FileID = iFileID;

            TopForm = tTopForm;
            InfiniumFiles = tInfiniumDocuments;

            bDownload = true;

            bSave = Save;
            SaveToPath = sSaveToPath;
            bUpload = false;
        }

        public UploadFileForm(ref FileManager tFM, ref Infinium.InfiniumFiles tInfiniumDocuments, string sSaveToPath, DataTable dtItemsDataTable, ref Form tTopForm)//download multiple
        {
            InitializeComponent();

            FM = tFM;

            TotalFilesCount = 1;

            TopForm = tTopForm;

            InfiniumFiles = tInfiniumDocuments;

            SaveToPath = sSaveToPath;

            ItemsDataTable = dtItemsDataTable;

            bUpload = false;
            bMultiple = true;
        }

        public UploadFileForm(ref FileManager tFM, ref Infinium.InfiniumFiles tInfiniumDocuments, int iFileID, string sFileName, ref Form tTopForm)//replace upload
        {
            InitializeComponent();

            FM = tFM;

            FileID = iFileID;

            TotalFilesCount = 1;

            FileName = sFileName;

            TopForm = tTopForm;
            InfiniumFiles = tInfiniumDocuments;

            bReplace = true;
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

                    if (bUpload)
                        bOk = StartLoading();

                    if (bMultiple)
                        StartDownloadMulti();

                    if (bDownload)
                        bOk = StartDownload();

                    if (bReplace)
                        StartReplace();

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


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;

                    if (bUpload)
                        bOk = StartLoading();

                    if (bMultiple)
                        StartDownloadMulti();

                    if (bDownload)
                        bOk = StartDownload();

                    if (bReplace)
                        StartReplace();
                }

                return;
            }
        }

        private void CreateFolderForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void CancelFolderButton_Click(object sender, EventArgs e)
        {
            bStopTransfer = true;
            Canceled = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            if (FM.TotalFileSize == 0)
                return;

            if (FM.Position == FM.TotalFileSize ||
                FM.Position > FM.TotalFileSize)
            {
                ProgressBar.Value = 100;
                return;
            }


            DownloadLabel.Text = FileManager.GetIntegerWithThousands(Convert.ToInt32(FM.Position / 1024)) + " / " +
                                   FileManager.GetIntegerWithThousands(Convert.ToInt32((FM.TotalFileSize / 1024))) + " КБайт";

            ProgressBar.Value = Convert.ToInt32(100 * FM.Position / FM.TotalFileSize);
            PercentsLabel.Text = ProgressBar.Value.ToString() + " %";
        }

        private int StartLoading()
        {
            bStopTransfer = false;
            FM.bStopTransfer = false;

            LoadLabel.Visible = true;
            ProgressBar.Visible = true;
            LoadTimer.Enabled = true;

            bool bOk = false;

            Thread T = new Thread(delegate () { bOk = InfiniumFiles.UploadFile(FileNames, FolderID, ref CurrentUploadedFile); });

            T.Start();

            this.Activate();
            Application.DoEvents();

            while (T.IsAlive)
            {
                T.Join(50);
                Application.DoEvents();

                if (CurrentUploadedFile != LastUploadedFile)
                {
                    LoadLabel.Text = "Загрузка файлов (" + CurrentUploadedFile.ToString() + " из " + TotalFilesCount.ToString() + ")";
                    LastUploadedFile = CurrentUploadedFile;
                }

                if (bStopTransfer)
                {
                    FM.bStopTransfer = true;
                    bStopTransfer = false;
                    LoadTimer.Enabled = false;
                    ProgressBar.Visible = false;
                    LoadLabel.Text = "Отмена загрузки файлов...";
                    DownloadLabel.Text = "";
                    SpeedLabel.Text = "";
                    PercentsLabel.Text = "";
                    CancelFilesButton.Visible = false;

                    Application.DoEvents();

                    while (T.IsAlive)
                        Thread.Sleep(50);

                    FormEvent = eClose;
                    AnimateTimer.Enabled = true;

                    return -1;
                }
            }

            LoadTimer.Enabled = false;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;

            return Convert.ToInt32(bOk);
        }

        private int StartDownload()
        {
            bStopTransfer = false;
            FM.bStopTransfer = false;

            LoadLabel.Visible = true;
            ProgressBar.Visible = true;
            LoadTimer.Enabled = true;

            Thread T;

            bool bOk = false;

            if (bSave)
                T = new Thread(delegate () { bOk = InfiniumFiles.SaveFile(FileID, SaveToPath); });
            else
                T = new Thread(delegate () { bOk = InfiniumFiles.OpenFile(FileID); });

            T.Start();

            this.Activate();
            Application.DoEvents();

            while (T.IsAlive)
            {
                T.Join(50);
                Application.DoEvents();

                if (CurrentUploadedFile != LastUploadedFile)
                {
                    LoadLabel.Text = "Загрузка файлов (" + CurrentUploadedFile.ToString() + " из " + TotalFilesCount.ToString() + ")";
                    LastUploadedFile = CurrentUploadedFile;
                }

                if (bStopTransfer)
                {
                    FM.bStopTransfer = true;
                    bStopTransfer = false;
                    LoadTimer.Enabled = false;
                    ProgressBar.Visible = false;
                    LoadLabel.Text = "Отмена загрузки файлов...";
                    DownloadLabel.Text = "";
                    SpeedLabel.Text = "";
                    PercentsLabel.Text = "";
                    CancelFilesButton.Visible = false;

                    Application.DoEvents();

                    while (T.IsAlive)
                        Thread.Sleep(50);

                    FormEvent = eClose;
                    AnimateTimer.Enabled = true;

                    return -1;
                }
            }

            LoadTimer.Enabled = false;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;

            return Convert.ToInt32(bOk);
        }

        private void StartDownloadMulti()
        {
            bStopTransfer = false;
            FM.bStopTransfer = false;

            LoadLabel.Visible = true;
            ProgressBar.Visible = true;
            LoadTimer.Enabled = true;

            Thread T;

            T = new Thread(delegate () { InfiniumFiles.SaveFiles(ItemsDataTable, SaveToPath, ref CurrentUploadedFile); });

            T.Start();

            this.Activate();
            Application.DoEvents();

            while (T.IsAlive)
            {
                T.Join(50);
                Application.DoEvents();

                if (CurrentUploadedFile != LastUploadedFile)
                {
                    LoadLabel.Text = "Загрузка файлов (" + CurrentUploadedFile.ToString() + " из " + TotalFilesCount.ToString() + ")";
                    LastUploadedFile = CurrentUploadedFile;
                }

                if (bStopTransfer)
                {
                    FM.bStopTransfer = true;
                    bStopTransfer = false;
                    LoadTimer.Enabled = false;
                    ProgressBar.Visible = false;
                    LoadLabel.Text = "Отмена загрузки файлов...";
                    DownloadLabel.Text = "";
                    SpeedLabel.Text = "";
                    PercentsLabel.Text = "";
                    CancelFilesButton.Visible = false;

                    Application.DoEvents();

                    while (T.IsAlive)
                        Thread.Sleep(50);

                    FormEvent = eClose;
                    AnimateTimer.Enabled = true;

                    return;
                }
            }

            LoadTimer.Enabled = false;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void StartReplace()
        {
            bStopTransfer = false;
            FM.bStopTransfer = false;

            LoadLabel.Visible = true;
            ProgressBar.Visible = true;
            LoadTimer.Enabled = true;

            bool Ok = false;

            Thread T = new Thread(delegate () { Ok = InfiniumFiles.ReplaceFile(FileID, FileName); });

            T.Start();

            this.Activate();
            Application.DoEvents();

            while (T.IsAlive)
            {
                T.Join(50);
                Application.DoEvents();

                if (CurrentUploadedFile != LastUploadedFile)
                {
                    LoadLabel.Text = "Загрузка файлов (" + CurrentUploadedFile.ToString() + " из " + TotalFilesCount.ToString() + ")";
                    LastUploadedFile = CurrentUploadedFile;
                }

                if (bStopTransfer)
                {
                    FM.bStopTransfer = true;
                    bStopTransfer = false;
                    LoadTimer.Enabled = false;
                    ProgressBar.Visible = false;
                    LoadLabel.Text = "Отмена загрузки файлов...";
                    DownloadLabel.Text = "";
                    SpeedLabel.Text = "";
                    PercentsLabel.Text = "";
                    CancelFilesButton.Visible = false;

                    Application.DoEvents();

                    while (T.IsAlive)
                        Thread.Sleep(50);

                    FormEvent = eClose;
                    AnimateTimer.Enabled = true;

                    return;
                }
            }

            LoadTimer.Enabled = false;

            if (!Ok)
            {
                MessageBox.Show("Один или несколько файлов не были найдены или отсутствовал доступ");
                return;
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

    }
}
