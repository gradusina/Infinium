using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class FilesForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        InfiniumFiles InfiniumFiles;

        bool bC = false;

        bool bNeedSplash = false;

        //bool bNeedNewsSplash = false;

        //bool bNewProjectsSelected = false;
        //bool bNewMessagesSelected = false;

        public FilesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            InfiniumDocumentsMenu.ItemsDataTable = InfiniumFiles.CategoriesDataTable;
            InfiniumDocumentsMenu.InitializeItems();

            InfiniumDocumentsMenu.Selected = 0;
            InfiniumDocumentsMenu_ItemClicked(this, 2, "Общие файлы");

            ProjectsForm_ANSUpdate(this);

            ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);


            while (!SplashForm.bCreated) ;
        }

        private void LightNewsForm_Shown(object sender, EventArgs e)
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
            InfiniumFiles = new InfiniumFiles();

            InfiniumDocumentAttributesView.UsersDataTable = InfiniumFiles.UsersDataTable;
            InfiniumDocumentAttributesView.CurrentUserID = Security.CurrentUserID;

            DocumentsPermissionsUsersList.UsersDataTable = InfiniumFiles.UsersDataTable;
            DocumentsPermissionsUsersList.DepartmentsDataTable = InfiniumFiles.DepartmentsDataTable;
            DocumentsPermissionsUsersList.DepsItemsDT = InfiniumFiles.CurrentPermissionsDepsDataTable;
            DocumentsPermissionsUsersList.UsersItemsDT = InfiniumFiles.CurrentPermissionsUsersDataTable;
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


        private void ProjectsForm_ANSUpdate(object sender)
        {
            int iSign = InfiniumFiles.GetSignCount(Security.CurrentUserID);

            if (iSign > 0)
                InfiniumDocumentsMenu.GetItemByFolderID(-1).Count = iSign;
            else
                InfiniumDocumentsMenu.GetItemByFolderID(-1).Count = 0;


            int iRead = InfiniumFiles.GetReadCount(Security.CurrentUserID);

            if (iRead > 0)
                InfiniumDocumentsMenu.GetItemByFolderID(-2).Count = iSign;
            else
                InfiniumDocumentsMenu.GetItemByFolderID(-2).Count = 0;

            ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
        }


        private void InfiniumFileList_ItemDoubleClick(object sender, int FolderID, int FileID)
        {
            if (FileID > -1)//open file
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles,
                                                                   InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption,
                                                                   InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID,
                                                                   InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID,
                                                                   false, "", ref TopForm);

                TopForm = UploadFileForm;

                UploadFileForm.ShowDialog();

                if (UploadFileForm.bOk == 0)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Отсутствует файл либо нет доступа к интернет", 5000);
                }

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
                UploadFileDialog.Dispose();

                return;
            }

            if (FolderID != -1)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                                   InfiniumFileList.Height, InfiniumFileList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumFiles.EnterFolder(FolderID);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable.Copy();

                InfiniumFileList.EnterInFolder(FolderID);
                InfiniumFileList.Entered = FolderID;

                bC = true;
            }
        }

        private void InfiniumFileList_RootDoubleClick(object sender, int FolderID, int FileID)
        {
            if (FolderID != -1)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                                   InfiniumFileList.Height, InfiniumFileList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumFiles.EnterFolder(FolderID);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable.Copy();
                InfiniumFileList.RootOutFolder(FolderID);
                InfiniumFileList.Entered = FolderID;

                bC = true;
            }
        }

        private void AddFolderButton_Click(object sender, EventArgs e)
        {
            if (InfiniumFileList.Entered > -1)
            {
                if (InfiniumFileList.CheckVisible)
                {
                    InfiniumFileList.CheckVisible = false;
                    CheckMultipleButton.BringToFront();
                }

                if (InfiniumFiles.CheckInheritedPermission(Security.CurrentUserID, InfiniumFileList.Entered) == false)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав", 3600);
                    return;
                }

                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                CreateFolderForm CreateFolderForm = new CreateFolderForm(ref InfiniumFiles, ref TopForm);

                TopForm = CreateFolderForm;

                CreateFolderForm.ShowDialog();

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;

                if (CreateFolderForm.Canceled)
                    return;


                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                                   InfiniumFileList.Height, InfiniumFileList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumFiles.CreateFolder(InfiniumFileList.Entered, CreateFolderForm.FolderName);

                InfiniumFiles.EnterFolder(InfiniumFileList.Entered);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;
                InfiniumFileList.EnterInFolder(InfiniumFileList.Entered);

                CreateFolderForm.Dispose();

                bC = true;
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (InfiniumFileList.Selected == -1)
                return;

            if (InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption == "[...]")
                return;

            if (InfiniumFiles.CurrentItemsDataTable.Select("FolderID = " +
                        InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID)[0]["Extension"].ToString() == "folder")
            {
                if (InfiniumFiles.CheckFolderPermission(Security.CurrentUserID, InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID) == false)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав", 3600);
                    return;
                }
            }
            else
            {
                if (InfiniumFiles.CanEditFile(Security.CurrentUserID, InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID) == false)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав", 3600);
                    return;
                }
            }



            if (InfiniumFileList.CheckVisible)
            {
                int r = InfiniumFiles.CheckCheckedItems(InfiniumFileList.ItemsDataTable);

                if (r == 0)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Выберите один или несколько файлов", 5600);

                    return;
                }

                bool OK = LightMessageBox.Show(ref TopForm, true,
                                    "Удалить выбранные файлы\\папки?", "Удаление");

                if (!OK)
                    return;

                InfiniumFiles.RemoveFolder(InfiniumFileList.ItemsDataTable);
                InfiniumFiles.RemoveFile(InfiniumFileList.ItemsDataTable);
            }
            else
            {
                if (InfiniumFileList.Selected > -1)
                {
                    bool OK = LightMessageBox.Show(ref TopForm, true,
                        "Удалить?", "Удаление");

                    if (!OK)
                        return;

                    if (InfiniumFiles.CurrentItemsDataTable.Select("FolderID = " +
                        InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID)[0]["Extension"].ToString() == "folder")
                    {
                        if (InfiniumFiles.RemoveFolder(InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID) == false)
                        {
                            InfiniumTips.ShowTip(this, 50, 85, "Ошибка удаления папки с хостинга", 4000);
                        }
                    }
                    else
                    {
                        InfiniumFiles.RemoveFile(InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID,
                                                     InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID,
                                                     InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption);
                    }

                }
            }


            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                               InfiniumFileList.Height, InfiniumFileList.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;


            InfiniumFiles.EnterFolder(InfiniumFileList.Entered);

            InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;
            InfiniumFileList.EnterInFolder(InfiniumFileList.Entered);


            if (InfiniumFileList.CheckVisible)
            {
                InfiniumFileList.CheckVisible = false;
                CheckMultipleButton.BringToFront();
            }

            bC = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void UploadFileIButton_Click(object sender, EventArgs e)
        {
            if (InfiniumFileList.Selected == -1)
                return;

            if (InfiniumFiles.CheckInheritedPermission(Security.CurrentUserID, InfiniumFileList.Entered) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав", 3600);
                return;
            }

            if (InfiniumFileList.CheckVisible)
            {
                InfiniumFileList.CheckVisible = false;
                CheckMultipleButton.BringToFront();
            }

            UploadFileDialog.Multiselect = true;

            if (UploadFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (InfiniumFiles.CheckFileExist(UploadFileDialog.FileNames, InfiniumFileList.Entered) == true)//file exist
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Один или несколько файлов уже существуют в папке", 2600);
                    return;
                }

                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles, UploadFileDialog.FileNames, InfiniumFileList.Entered,
                                                                   ref TopForm);

                TopForm = UploadFileForm;

                UploadFileForm.ShowDialog();

                if (UploadFileForm.bOk == 0)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Отсутствует файл либо нет доступа к интернет", 5000);
                }

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
                UploadFileDialog.Dispose();


                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                                   InfiniumFileList.Height, InfiniumFileList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumFiles.EnterFolder(InfiniumFileList.Entered);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;

                bC = true;
            }
        }

        private void InfiniumFileList_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void InfiniumFileList_ItemRightClicked(object sender, int FolderID, int FileID)
        {
            if (FileID == -1)//folder
                return;

            FileContextMenu.Show(InfiniumFileList);
        }

        private void MenuFileOpenFile_Click(object sender, EventArgs e)
        {
            if (InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID != -1)
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles,
                                                                   InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption,
                                                                   InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID,
                                                                   InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID,
                                                                   false, "", ref TopForm);

                TopForm = UploadFileForm;

                UploadFileForm.ShowDialog();

                if (UploadFileForm.bOk == 0)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Отсутствует файл либо нет доступа к интернет", 5000);
                }

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
                UploadFileDialog.Dispose();
            }
        }

        private void MenuFileSaveFile_Click(object sender, EventArgs e)
        {
            if (InfiniumFileList.CheckVisible == false)
            {
                SaveFileDialog.Filter = "(*" + Path.GetExtension(InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption) + ")|*" +
                                               Path.GetExtension(InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption);
                SaveFileDialog.FileName = InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption;

                if (SaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    PhantomForm PhantomForm = new PhantomForm();
                    PhantomForm.Show();

                    UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles,
                                                                       InfiniumFileList.FileItems[InfiniumFileList.Selected].Caption,
                                                                       InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID,
                                                                       InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID,
                                                                       true, SaveFileDialog.FileName, ref TopForm);

                    TopForm = UploadFileForm;

                    UploadFileForm.ShowDialog();

                    if (UploadFileForm.bOk == 0)
                    {
                        InfiniumTips.ShowTip(this, 50, 85, "Отсутствует файл либо нет доступа к интернет", 5000);
                    }

                    PhantomForm.Close();
                    PhantomForm.Dispose();

                    TopForm = null;
                    UploadFileDialog.Dispose();
                }
            }
            else
            {
                int r = InfiniumFiles.CheckCheckedItems(InfiniumFileList.ItemsDataTable);

                if (r == -2)
                {
                    bool OK = LightMessageBox.Show(ref TopForm, true,
                                    "Копирование папок не поддерживается, скопируются только выбранные файлы. Продолжить?", "Копирование файла");

                    if (!OK)
                        return;
                }

                if (r == -1)
                {
                    LightMessageBox.Show(ref TopForm, false,
                                    "Копирование папок не поддерживается.", "Копирование файла");

                    return;
                }

                if (r == 0)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Выберите один или несколько файлов", 5600);

                    return;
                }



                if (FolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (InfiniumFiles.CheckFilesExists(InfiniumFileList.ItemsDataTable, FolderBrowserDialog.SelectedPath))
                    {
                        bool OK = LightMessageBox.Show(ref TopForm, true,
                                    "Файл с таким именем в указанной папке уже существует. Заменить?", "Копирование файла");

                        if (!OK)
                            return;
                    }

                    PhantomForm PhantomForm = new PhantomForm();
                    PhantomForm.Show();

                    UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles, FolderBrowserDialog.SelectedPath,
                                                                       InfiniumFileList.ItemsDataTable, ref TopForm);

                    TopForm = UploadFileForm;

                    UploadFileForm.ShowDialog();

                    PhantomForm.Close();
                    PhantomForm.Dispose();

                    TopForm = null;
                    UploadFileDialog.Dispose();


                    if (InfiniumFileList.CheckVisible)
                    {
                        InfiniumFileList.CheckVisible = false;
                        CheckMultipleButton.BringToFront();
                    }
                }
            }
        }

        private void MenuFileReplaceFile_Click(object sender, EventArgs e)
        {
            if (InfiniumFiles.CheckFileVersion(InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID, Security.CurrentUserID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Файл был кем-то изменен. Замена файла не возможна. Скачайте файл заново, \n" +
                                               "примените свои изменения, а затем попытайтесь заменить еще раз", 7600);
                return;
            }

            if (InfiniumFiles.CheckUploadPending(InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID))//someone uploads
            {
                InfiniumTips.ShowTip(this, 50, 85, "В настоящее время указанный файл уже обновляется кем-то. Замена файла не возможна. \n" +
                                                    "Скачайте файл заново, примените свои изменения, а затем попытайтесь заменить еще раз", 7600);
                return;
            }

            if (UploadFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles, InfiniumFileList.FileItems[InfiniumFileList.Selected].FileID,
                                                                   UploadFileDialog.FileName, ref TopForm);

                TopForm = UploadFileForm;

                UploadFileForm.ShowDialog();

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
                UploadFileDialog.Dispose();
            }


            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                               InfiniumFileList.Height, InfiniumFileList.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;


            InfiniumFiles.EnterFolder(InfiniumFileList.Entered);

            InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;

            bC = true;
        }

        private void CheckMultipleButton_Click(object sender, EventArgs e)
        {
            UncheckMultipleButton.BringToFront();
            InfiniumFileList.CheckVisible = true;
        }

        private void UncheckMultipleButton_Click(object sender, EventArgs e)
        {
            CheckMultipleButton.BringToFront();
            InfiniumFileList.CheckVisible = false;
        }

        private void MenuFileDeleteFile_Click(object sender, EventArgs e)
        {
            RemoveButton_Click(null, null);
        }

        private void SaveFileMenu_Opening(object sender, CancelEventArgs e)
        {
            if (InfiniumFileList.CheckVisible)
            {
                MenuFileOpenFile.Visible = false;
                MenuFileReplaceFile.Visible = false;
            }
        }

        private void InfiniumFileList_SizeChanged(object sender, EventArgs e)
        {
            if (InfiniumFileList.FileItems == null)
                return;

            if (InfiniumFileList.FileItems.Count() == 0)
                return;

            HeaderPanel.Width = InfiniumFileList.Width;
            HeaderPanel.Left = InfiniumFileList.Left;

            HeaderCreateLabel.Left = HeaderPanel.Width / 100 * (InfiniumFileList.FileItems[0].iMarginForAuthorPercents) - 2;
            HeaderLastModifiedLabel.Left = HeaderPanel.Width / 100 * (InfiniumFileList.FileItems[0].iMarginForLastModifiedPercents) - 2;
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

        private void LoadWithSampleButton_Click(object sender, EventArgs e)
        {
            if (InfiniumFileList.Selected == -1)
                return;

            if (InfiniumFiles.CheckInheritedPermission(Security.CurrentUserID, InfiniumFileList.Entered) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав", 3600);
                return;
            }

            if (InfiniumFileList.CheckVisible)
            {
                InfiniumFileList.CheckVisible = false;
                CheckMultipleButton.BringToFront();
            }

            UploadFileDialog.Multiselect = false;

            if (UploadFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (InfiniumFiles.CheckFileExist(UploadFileDialog.FileNames, InfiniumFileList.Entered) == true)//file exist
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Один или несколько файлов уже существуют в папке", 2600);
                    return;
                }

                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();


                //set attributes
                DocumentAttributesForm DocumentAttributesForm = new DocumentAttributesForm(ref InfiniumFiles, ref TopForm);

                TopForm = DocumentAttributesForm;

                DocumentAttributesForm.ShowDialog();

                bool bFirstSign = DocumentAttributesForm.bFirstSign;

                if (DocumentAttributesForm.bCanceled)
                {
                    PhantomForm.Close();
                    PhantomForm.Dispose();

                    TopForm = null;
                    return;
                }

                //upload
                UploadFileForm UploadFileForm = new UploadFileForm(ref InfiniumFiles.FM, ref InfiniumFiles, UploadFileDialog.FileNames, InfiniumFileList.Entered,
                                                                   ref TopForm);

                TopForm = UploadFileForm;

                UploadFileForm.ShowDialog();

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
                UploadFileDialog.Dispose();

                InfiniumFiles.SetAttributes(Path.GetFileName(UploadFileDialog.FileNames[0]), InfiniumFileList.Entered, bFirstSign);

                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                                   InfiniumFileList.Height, InfiniumFileList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumFiles.EnterFolder(InfiniumFileList.Entered);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;

                bC = true;
            }
        }

        private void InfiniumFileList_SelectedChanged(object sender, int FolderID, int FileID)
        {
            if (FileID > -1)
            {
                InfiniumFiles.FillAttributes(FileID);

                InfiniumDocumentAttributesView.AttributesDataTable = InfiniumFiles.CurrentAttributesDataTable;
                InfiniumDocumentAttributesView.SignsDT = InfiniumFiles.CurrentSignsDataTable;
                InfiniumDocumentAttributesView.ReadListDT = InfiniumFiles.CurrentReadDataTable;
                InfiniumDocumentAttributesView.FileID = FileID;
                InfiniumDocumentAttributesView.bFirstSign = InfiniumFiles.bFirstSign;
                InfiniumDocumentAttributesView.InitializeItems();
            }
            else
            {
                InfiniumFiles.FillAttributes(FileID);

                InfiniumDocumentAttributesView.AttributesDataTable = InfiniumFiles.CurrentAttributesDataTable;
                InfiniumDocumentAttributesView.SignsDT = InfiniumFiles.CurrentSignsDataTable;
                InfiniumDocumentAttributesView.ReadListDT = InfiniumFiles.CurrentReadDataTable;
                InfiniumDocumentAttributesView.FileID = -1;
                InfiniumDocumentAttributesView.InitializeItems();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void InfiniumDocumentAttributesView_SignButtonClicked(object sender, int UserID, int FileID)
        {
            if (Security.CheckAuthNull() == true)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Авторизационный код отсутствует. Создайте авторизационный\n" +
                                                    "код на вкладке \"Сменить пароль\" в настройках Infinium", 10000);
                return;
            }



            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AuthorizationForm AuthorizationForm = new AuthorizationForm(ref TopForm);

            TopForm = AuthorizationForm;

            AuthorizationForm.ShowDialog();

            if (AuthorizationForm.bCanceled)
            {
                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
            }
            else
            {
                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;


                InfiniumFiles.SignFile(FileID, UserID);

                InfiniumFiles.FillAttributes(FileID);

                InfiniumDocumentAttributesView.AttributesDataTable = InfiniumFiles.CurrentAttributesDataTable;
                InfiniumDocumentAttributesView.SignsDT = InfiniumFiles.CurrentSignsDataTable;
                InfiniumDocumentAttributesView.ReadListDT = InfiniumFiles.CurrentReadDataTable;
                InfiniumDocumentAttributesView.InitializeItems();

                ProjectsForm_ANSUpdate(null);

                InfiniumTips.ShowTip(this, 50, 85, "Подписано", 4000);
            }

        }

        private void InfiniumDocumentAttributesView_ReadButtonClicked(object sender, int UserID, int FileID)
        {
            if (Security.CheckAuthNull() == true)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Авторизационный код отсутствует. Создайте авторизационный\n" +
                                                    "код на вкладке \"Сменить пароль\" в настройках Infinium", 10000);
                return;
            }



            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AuthorizationForm AuthorizationForm = new AuthorizationForm(ref TopForm);

            TopForm = AuthorizationForm;

            AuthorizationForm.ShowDialog();

            if (AuthorizationForm.bCanceled)
            {
                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
            }
            else
            {
                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;


                InfiniumFiles.SignReadFile(FileID, UserID);

                InfiniumFiles.FillAttributes(FileID);

                InfiniumDocumentAttributesView.AttributesDataTable = InfiniumFiles.CurrentAttributesDataTable;
                InfiniumDocumentAttributesView.SignsDT = InfiniumFiles.CurrentSignsDataTable;
                InfiniumDocumentAttributesView.ReadListDT = InfiniumFiles.CurrentReadDataTable;
                InfiniumDocumentAttributesView.InitializeItems();

                //int iSC = InfiniumDocuments.GetSignCount(Security.CurrentUserID);

                //if (iSC > 0)
                //    SignFilesLabel.Text = "На подпись (" + iSC.ToString() + ")";
                //else
                //    SignFilesLabel.Text = "На подпись";

                InfiniumTips.ShowTip(this, 50, 85, "Подписано", 4000);
            }
        }

        private void InfiniumFileList_FolderEntered(object sender, int FolderID, int FileID)
        {
            int res = InfiniumFiles.FillPermissionsUsers(FolderID);

            if (res == -1)
            {
                DocumentsPermissionsUsersList.bAdminOnly = true;
                DocumentsPermissionsUsersList.bAllUsers = false;
            }

            if (res == -2)
            {
                DocumentsPermissionsUsersList.bAdminOnly = false;
                DocumentsPermissionsUsersList.bAllUsers = true;
            }

            if (res == 1)
            {
                DocumentsPermissionsUsersList.bAdminOnly = false;
                DocumentsPermissionsUsersList.bAllUsers = false;
            }

            DocumentsPermissionsUsersList.Refresh();
        }


        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void SignFilesLabel_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        private void InfiniumDocumentsMenu_ItemClicked(object sender, int FolderID, string Caption)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumFileList.Top + UpdatePanel.Top, InfiniumFileList.Left + UpdatePanel.Left,
                                                   InfiniumFileList.Height, InfiniumFileList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }


            if (FolderID == -3)
            {
                int iP = InfiniumFiles.CheckPersonalFolder(Security.CurrentUserID);

                if (iP == -1)
                {
                    InfiniumFiles.CreatePersonalFolder(Security.CurrentUserID);
                    iP = InfiniumFiles.CheckPersonalFolder(Security.CurrentUserID);
                }

                InfiniumFiles.EnterFolder(iP);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;

                InfiniumFileList.Entered = iP;
            }

            if (FolderID > 0)
            {
                InfiniumFiles.EnterFolder(FolderID);

                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;

                InfiniumFileList.Entered = FolderID;
            }

            if (FolderID == 4)//клиенты
            {
                if (InfiniumFiles.CheckInheritedPermission(Security.CurrentUserID, 4))
                    SendMailButton.Visible = true;
            }
            else
                SendMailButton.Visible = false;

            if (FolderID == -1)//на подпись
            {
                InfiniumFiles.FillSignsFiles(Security.CurrentUserID);
                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;
                InfiniumFileList.Entered = -1;
            }

            if (FolderID == -2)//на ознакомление
            {
                InfiniumFiles.FillReadFiles(Security.CurrentUserID);
                InfiniumFileList.ItemsDataTable = InfiniumFiles.CurrentItemsDataTable;
                InfiniumFileList.Entered = -1;
            }


            if (bNeedSplash)
                bC = true;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {


        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM Folders WHERE FolderID > 85", ConnectionStrings.LightConnectionString))
            {
                using (System.Data.SqlClient.SqlCommandBuilder CB = new System.Data.SqlClient.SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            DT.Rows[i]["FTPHost"] = true;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        private void button1_Click_4(object sender, EventArgs e)
        {
            //using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT ClientFolderName, ClientID FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        DA.Fill(DT);

            //        InfiniumDocuments InfiniumDocuments = new InfiniumDocuments();

            //        for (int i = 0; i < DT.Rows.Count; i++)
            //        {
            //            InfiniumDocuments.CreateClientFolders(DT.Rows[i]["ClientFolderName"].ToString());
            //            Thread.Sleep(1000);
            //        }
            //    }
            //}


        }

        private void SendMailButton_Click(object sender, EventArgs e)
        {
            int i = -1;

            if (InfiniumFileList.Selected == -1)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Не выбран клиент", 3000);
                return;
            }


            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            LightMessageBoxForm LightMessageBoxForm = new Infinium.LightMessageBoxForm(true, "Прикрепить к уведомлению файлы?",
                                                                                    "Уведомление клиенту");

            TopForm = LightMessageBoxForm;

            LightMessageBoxForm.ShowDialog();

            TopForm = null;

            PhantomForm.Close();
            PhantomForm.Dispose();

            if (LightMessageBoxForm.OKCancel)
            {
                openFileDialog1.ShowDialog();
            }

            if (InfiniumFileList.Entered == 4)
            {
                i = InfiniumFiles.GetCurrentClientFolder(InfiniumFileList.FileItems[InfiniumFileList.Selected].FolderID);
            }
            else
                i = InfiniumFiles.GetCurrentClientFolder(InfiniumFileList.Entered);

            if (i == -1)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Невозможно определить клиента", 4000);
                return;
            }

            string r = InfiniumFiles.SendEmailNotifyClient(i, openFileDialog1.FileNames);

            if (r == "-1")
            {
                LightMessageBox.Show(ref TopForm, false, "У клиента не указан Email", "Уведомление");
                //InfiniumTips.ShowTip(this, 50, 85, "У клиента не указан Email", 3000);
                return;
            }
            else
                if (r == "1")
            {
                LightMessageBox.Show(ref TopForm, false, "Уведомление успешно отправлено", "Уведомление");
                //InfiniumTips.ShowTip(this, 50, 85, "Уведомление успешно отправлено", 3000);
                return;
            }
            else
            {
                LightMessageBox.Show(ref TopForm, false, r, "Уведомление");
                //InfiniumTips.ShowTip(this, 50, 85, "Не подключения к интернет", 3000);
                return;
            }




        }

        private void button1_Click_5(object sender, EventArgs e)
        {
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT FolderID FROM Folders WHERE FolderID > 192 AND FolderID < 1080", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    using (System.Data.SqlClient.SqlDataAdapter sDA = new System.Data.SqlClient.SqlDataAdapter("SELECT TOP 0 * FROM DocumentsPermissions", ConnectionStrings.LightConnectionString))
                    {
                        using (System.Data.SqlClient.SqlCommandBuilder CB = new System.Data.SqlClient.SqlCommandBuilder(sDA))
                        {
                            using (DataTable sDT = new DataTable())
                            {
                                sDA.Fill(sDT);

                                foreach (DataRow Row in DT.Rows)
                                {
                                    DataRow NewRow = sDT.NewRow();
                                    NewRow["FolderID"] = Row["FolderID"];
                                    NewRow["UserID"] = 374;
                                    NewRow["UserTypeID"] = 0;
                                    sDT.Rows.Add(NewRow);
                                }

                                sDA.Update(sDT);
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click_6(object sender, EventArgs e)
        {
            //using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT ClientFolderName, ClientID FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            //{
            //    using (DataTable DT = new DataTable())
            //    {
            //        DA.Fill(DT);

            //        InfiniumDocuments InfiniumDocuments = new InfiniumDocuments();

            //        for (int i = 0; i < DT.Rows.Count; i++)
            //        {
            //            InfiniumDocuments.CreateFolderClient(DT.Rows[i]["ClientFolderName"].ToString(), "Спецификации");
            //            Thread.Sleep(1000);
            //        }
            //    }
            //}

            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM Folders", ConnectionStrings.LightConnectionString))
            {
                using (System.Data.SqlClient.SqlCommandBuilder CB = new System.Data.SqlClient.SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in DT.Rows)
                        {
                            if (Row["RootFolderID"].ToString() == "0")
                                Row["ParentFolderID"] = 0;
                            else
                            {
                                int iR = -1;
                                int iF = Convert.ToInt32(Row["RootFolderID"]);

                                for (int i = 0; i < 100; i++)
                                {
                                    iR = InfiniumFiles.GetRootFolderID(iF);

                                    if (iR != 0)
                                    {
                                        iF = iR;
                                        continue;
                                    }
                                    else
                                    {
                                        Row["ParentFolderID"] = iF;

                                        break;
                                    }
                                }
                            }
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

    }
}