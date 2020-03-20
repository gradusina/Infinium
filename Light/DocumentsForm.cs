using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DocumentsForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        InfiniumDocuments InfiniumDocuments;

        bool bC = false;

        bool bNeedSplash = false;

        public int CurrentDocumenID = -1;
        public int CurrentDocumentCategoryID = -1;

        public DocumentsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void LightNewsForm_Shown(object sender, EventArgs e)
        {
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
                    bNeedSplash = true;

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
            InfiniumDocuments = new InfiniumDocuments();

            InfiniumDocumentsMenu.ItemsDataTable = InfiniumDocuments.DocumentsMenuDataTable;
            InfiniumDocumentsMenu.InitializeItems();


            InnerDocumentsList.FactoryDataTable = InfiniumDocuments.FactoryTypesDataTable;
            InnerDocumentsList.DocumentsTypesDataTable = InfiniumDocuments.DocumentTypesDataTable;
            InnerDocumentsList.UsersDataTable = InfiniumDocuments.UsersDataTable;
            InnerDocumentsList.CorrespondentsDataTable = InfiniumDocuments.CorrespondentsDataTable;

            IncomeDocumentsList.FactoryDataTable = InfiniumDocuments.FactoryTypesDataTable;
            IncomeDocumentsList.DocumentsTypesDataTable = InfiniumDocuments.DocumentTypesDataTable;
            IncomeDocumentsList.UsersDataTable = InfiniumDocuments.UsersDataTable;
            IncomeDocumentsList.CorrespondentsDataTable = InfiniumDocuments.CorrespondentsDataTable;

            OuterDocumentsList.FactoryDataTable = InfiniumDocuments.FactoryTypesDataTable;
            OuterDocumentsList.DocumentsTypesDataTable = InfiniumDocuments.DocumentTypesDataTable;
            OuterDocumentsList.UsersDataTable = InfiniumDocuments.UsersDataTable;
            OuterDocumentsList.CorrespondentsDataTable = InfiniumDocuments.CorrespondentsDataTable;

            InnerDocumentsList.ItemsDataTable = InfiniumDocuments.InnerDocumentsDataTable;
            IncomeDocumentsList.ItemsDataTable = InfiniumDocuments.IncomeDocumentsDataTable;
            OuterDocumentsList.ItemsDataTable = InfiniumDocuments.OuterDocumentsDataTable;

            DocTypeComboBox.DataSource = InfiniumDocuments.DocumentTypesDataTable;
            DocTypeComboBox.DisplayMember = "DocumentType";
            DocTypeComboBox.ValueMember = "DocumentTypeID";

            CorrespondentComboBox.DataSource = InfiniumDocuments.CorrespondentsDataTable;
            CorrespondentComboBox.DisplayMember = "CorrespondentName";
            CorrespondentComboBox.ValueMember = "CorrespondentID";

            FactoryComboBox.DataSource = InfiniumDocuments.FactoryTypesDataTable;
            FactoryComboBox.DisplayMember = "Factory";
            FactoryComboBox.ValueMember = "FactoryID";

            CategoriesComboBox.DataSource = InfiniumDocuments.DocumentsCategoriesDataTable;
            CategoriesComboBox.DisplayMember = "DocumentCategory";
            CategoriesComboBox.ValueMember = "DocumentCategoryID";

            DocumentsUpdatesList.ItemsDataTable = InfiniumDocuments.UpdatesDocumentsDataTable;
            DocumentsUpdatesList.UsersDataTable = InfiniumDocuments.UsersDataTable;
            DocumentsUpdatesList.CommentsDataTable = InfiniumDocuments.UpdatesCommentsDataTable;
            DocumentsUpdatesList.FilesDataTable = InfiniumDocuments.UpdatesFilesDataTable;
            DocumentsUpdatesList.DocumentsTypesDataTable = InfiniumDocuments.DocumentTypesDataTable;
            DocumentsUpdatesList.RecipientsDataTable = InfiniumDocuments.UpdatesRecipientsDataTable;
            DocumentsUpdatesList.CorrespondentsDataTable = InfiniumDocuments.CorrespondentsDataTable;
            DocumentsUpdatesList.CommentsFilesDataTable = InfiniumDocuments.UpdatesCommentsFilesDataTable;
            DocumentsUpdatesList.DocumentsCategoriesDataTable = InfiniumDocuments.DocumentsCategoriesDataTable;
            DocumentsUpdatesList.FactoryDataTable = InfiniumDocuments.FactoryTypesDataTable;
            DocumentsUpdatesList.ConfirmsDataTable = InfiniumDocuments.UpdatesConfirmsDataTable;
            DocumentsUpdatesList.ConfirmsRecipientsDataTable = InfiniumDocuments.UpdatesConfirmsRecipientsDataTable;

            InfiniumDocumentsMenu.Selected = 0;
            InfiniumDocumentsMenu.Items[3].Count = InfiniumDocuments.GetNotSigned(Security.CurrentUserID);
        }


        private void CoverUpdatesList()
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top + DateTypePanel.Height, UpdatePanel.Left,
                                                   UpdatePanel.Height - DateTypePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }
        }

        private void CoverDocumentsList()
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InnerDocumentsList.Top + UpdatePanel.Top, UpdatePanel.Left,
                                                   InnerDocumentsList.Height - InnerDocumentsList.Top, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }
        }

        private void CoverUpdatePanel()
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }
        }


        private int GetDateType()
        {
            if (NewUpdatesButton.Checked)
                return 0;
            if (TodayUpdatesButton.Checked)
                return 1;
            if (WeekUpdatesButton.Checked)
                return 2;
            if (MonthUpdatesButton.Checked)
                return 3;

            return -1;
        }

        private int GetConfirmType()
        {
            if (NotConfirmedButton.Checked)
                return 0;
            if (ConfirmedButton.Checked)
                return 1;
            if (CanceledButton.Checked)
                return 2;
            if (YourSignButton.Checked)
                return 3;

            return -1;
        }

        private int GetMyDocsType()
        {
            if (MyDocCreatorButton.Checked)
                return 0;
            if (MyDocRecipientButton.Checked)
                return 1;

            return -1;
        }

        private string GetFilter()
        {
            string res = "";

            if (DayCheckBox.Checked)
                res += "(CAST(DateTime AS DATE) = '" + DayPicker.Value.ToString("yyyy-MM-dd") + "')";

            if (DocTypeCheckBox.Checked)
            {
                if (res.Length != 0)
                    res += " AND ";

                res += "(DocumentTypeID = " + Convert.ToInt32(DocTypeComboBox.SelectedValue) + ")";
            }

            if (CorrespondentCheckBox.Checked)
            {
                if (res.Length != 0)
                    res += " AND ";

                res += "(CorrespondentID = " + Convert.ToInt32(CorrespondentComboBox.SelectedValue) + ")";
            }

            if (FactoryCheckBox.Checked)
            {
                if (res.Length != 0)
                    res += " AND ";

                res += "(FactoryID = " + Convert.ToInt32(FactoryComboBox.SelectedValue) + ")";
            }

            return res;
        }

        private void FillUpdates()
        {
            if (InfiniumDocumentsMenu.SelectedName == "Лента")
                InfiniumDocuments.FillUpdates(GetDateType());

            if (InfiniumDocumentsMenu.SelectedName == "Все документы")
                InfiniumDocuments.FillItemUpdate(CurrentDocumenID, CurrentDocumentCategoryID);

            if (InfiniumDocumentsMenu.SelectedName == "Мои документы")
                InfiniumDocuments.FillMyUpdates(Security.CurrentUserID, GetMyDocsType());

            if (InfiniumDocumentsMenu.SelectedName == "Согласование")
                InfiniumDocuments.FillConfirmUpdates(GetConfirmType());

            InfiniumDocumentsMenu.Items[3].Count = InfiniumDocuments.GetNotSigned(Security.CurrentUserID);
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
            InfiniumDocumentsMenu.Items[0].Count = InfiniumDocuments.GetUpdatesCount(Security.CurrentUserID);
            InfiniumDocumentsMenu.Items[3].Count = InfiniumDocuments.GetNotSigned(Security.CurrentUserID);
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

        private void CreateDocumentButton_Click(object sender, EventArgs e)
        {
            if (CategoriesComboBox.GetItemText(CategoriesComboBox.SelectedItem) == "Исходящий документ")
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                CreateOuterDocumentForm CreateDocumentForm = new CreateOuterDocumentForm(ref TopForm, ref InfiniumDocuments);

                TopForm = CreateDocumentForm;

                CreateDocumentForm.ShowDialog();

                if (CreateDocumentForm.bCanceled == false)
                {
                    InfiniumDocuments.FillOuterDocuments(GetFilter());
                    OuterDocumentsList.InitializeItems();
                    OuterDocumentsList.BringToFront();
                }

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
            }

            if (CategoriesComboBox.GetItemText(CategoriesComboBox.SelectedItem) == "Внутренний документ")
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                CreateInnerDocumentForm CreateDocumentForm = new CreateInnerDocumentForm(ref TopForm, ref InfiniumDocuments);

                TopForm = CreateDocumentForm;

                CreateDocumentForm.ShowDialog();

                if (CreateDocumentForm.bCanceled == false)
                {
                    InfiniumDocuments.FillInnerDocuments(GetFilter());
                    InnerDocumentsList.InitializeItems();
                    InnerDocumentsList.BringToFront();
                }

                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
            }

            if (CategoriesComboBox.GetItemText(CategoriesComboBox.SelectedItem) == "Входящий документ")
            {
                PhantomForm PhantomForm = new PhantomForm();
                PhantomForm.Show();

                CreateIncomeDocumentForm CreateDocumentForm = new CreateIncomeDocumentForm(ref TopForm, ref InfiniumDocuments);

                TopForm = CreateDocumentForm;

                CreateDocumentForm.ShowDialog();

                if (CreateDocumentForm.bCanceled == false)
                {
                    InfiniumDocuments.FillIncomeDocuments(GetFilter());
                    IncomeDocumentsList.InitializeItems();
                    IncomeDocumentsList.BringToFront();
                }
                PhantomForm.Close();
                PhantomForm.Dispose();

                TopForm = null;
            }
        }

        private void InfiniumDocumentsMenu_SelectedChanged(object sender, string Name, int index)
        {
            CoverUpdatePanel();

            if (Name == "Лента")
            {
                UpdatesPanel.BringToFront();
                DateTypePanel.BringToFront();
                EditButtonsPanel.Hide();

                if (InfiniumDocumentsMenu.Items[0].Count > 0)
                {
                    NewUpdatesButton.Checked = true;
                    TodayUpdatesButton.Checked = false;
                    WeekUpdatesButton.Checked = false;
                    MonthUpdatesButton.Checked = false;
                }
                else
                {
                    NewUpdatesButton.Checked = false;
                    TodayUpdatesButton.Checked = true;
                    WeekUpdatesButton.Checked = false;
                    MonthUpdatesButton.Checked = false;
                }

                FillUpdates();
                DocumentsUpdatesList.InitializeItems();

                ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                InfiniumDocumentsMenu.Items[0].Count = 0;
            }

            if (Name == "Все документы")
            {
                AllDocumentsPanel.BringToFront();
                FilterPanel.BringToFront();
                EditButtonsPanel.Show();
            }

            if (Name == "Согласование")
            {
                UpdatesPanel.BringToFront();
                StatusPanel.BringToFront();
                EditButtonsPanel.Hide();

                FillUpdates();
                DocumentsUpdatesList.InitializeItems();
            }

            if (Name == "Мои документы")
            {
                UpdatesPanel.BringToFront();
                MyDocumentsPanel.BringToFront();
                EditButtonsPanel.Hide();

                FillUpdates();
                DocumentsUpdatesList.InitializeItems();
            }

            if (bNeedSplash)
                bC = true;
        }

        private void MenuFilterButton_Click(object sender, EventArgs e)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InnerDocumentsList.Top + UpdatePanel.Top, InnerDocumentsList.Left + UpdatePanel.Left,
                                                   InnerDocumentsList.Height, InnerDocumentsList.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (CategoriesComboBox.GetItemText(CategoriesComboBox.SelectedItem) == "Внутренний документ")
            {
                InfiniumDocuments.FillInnerDocuments(GetFilter());
                InnerDocumentsList.InitializeItems();
                InnerDocumentsList.BringToFront();
            }

            if (CategoriesComboBox.GetItemText(CategoriesComboBox.SelectedItem) == "Входящий документ")
            {
                InfiniumDocuments.FillIncomeDocuments(GetFilter());
                IncomeDocumentsList.InitializeItems();
                IncomeDocumentsList.BringToFront();
            }

            if (CategoriesComboBox.GetItemText(CategoriesComboBox.SelectedItem) == "Исходящий документ")
            {
                InfiniumDocuments.FillOuterDocuments(GetFilter());
                OuterDocumentsList.InitializeItems();
                OuterDocumentsList.BringToFront();
            }


            if (bNeedSplash)
                bC = true;
        }

        private void DocumentsUpdatesList_CommentsTextBoxFileLabelClicked(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DocumentsCommentsFilesForm DocumentsCommentsFilesForm = new DocumentsCommentsFilesForm(ref TopForm, ref InfiniumDocuments,
                                            ((InfiniumDocumentsUpdatesItem)sender).CurrentFilesDataTable);

            TopForm = DocumentsCommentsFilesForm;

            DocumentsCommentsFilesForm.ShowDialog();

            if (((InfiniumDocumentsUpdatesItem)sender).CurrentFilesDataTable.Rows.Count > 0)
                ((InfiniumDocumentsUpdatesItem)sender).ControlPanel.CommentsTextBox.FilesLabel.Text =
                    ((InfiniumDocumentsUpdatesItem)sender).CurrentFilesDataTable.Rows.Count.ToString() + " файлов";
            else
                ((InfiniumDocumentsUpdatesItem)sender).ControlPanel.CommentsTextBox.FilesLabel.Text = "Прикрепить файлы";

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void DocumentsUpdatesList_CommentsSendButtonClicked(object sender, int DocumentID, int DocumentCommentID, int DocumentCategoryID, string sText, bool bIsNew, DataTable FilesDataTable)
        {
            InfiniumDocuments.FM.bStopTransfer = false;

            int iCurrentFile = 0;

            if (sText.Length == 0 && FilesDataTable.Rows.Count == 0)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Добавьте текст и/или файлы", 3000);

                return;
            }


            if (DocumentCommentID != -1)//edit
            {
                if (FilesDataTable.Select("IsNew = 1").Count() == 0)
                {
                    CoverUpdatesList();

                    InfiniumDocuments.EditComment(DocumentCommentID, sText, FilesDataTable, ref iCurrentFile);

                    ((InfiniumDocumentsUpdatesItem)sender).CloseCommentsTextBox();


                    FillUpdates();

                    DocumentsUpdatesList.InitializeItems();
                    GC.Collect();

                    if (bNeedSplash)
                        bC = true;
                }
                else
                {

                    PhantomForm PhantomForm = new PhantomForm();
                    PhantomForm.Show();

                    DocumentsUploadForm DocumentsUploadForm = new DocumentsUploadForm(ref TopForm, ref InfiniumDocuments, sText, FilesDataTable, DocumentCommentID);

                    TopForm = DocumentsUploadForm;

                    DocumentsUploadForm.ShowDialog();

                    if (DocumentsUploadForm.bOK)
                        ((InfiniumDocumentsUpdatesItem)sender).CloseCommentsTextBox();

                    PhantomForm.Close();
                    PhantomForm.Dispose();

                    TopForm = null;

                    CoverUpdatesList();

                    FillUpdates();

                    DocumentsUpdatesList.InitializeItems();
                    GC.Collect();

                    if (bNeedSplash)
                        bC = true;
                }
            }
            else
            {
                if (FilesDataTable.Select("IsNew = 1").Count() == 0)
                {
                    CoverUpdatesList();

                    InfiniumDocuments.AddComment(Security.CurrentUserID, sText, DocumentID, DocumentCategoryID, FilesDataTable, ref iCurrentFile);

                    ((InfiniumDocumentsUpdatesItem)sender).CloseCommentsTextBox();

                    FillUpdates();

                    DocumentsUpdatesList.InitializeItems();
                    GC.Collect();

                    if (bNeedSplash)
                        bC = true;
                }
                else
                {
                    PhantomForm PhantomForm = new PhantomForm();
                    PhantomForm.Show();

                    DocumentsUploadForm DocumentsUploadForm = new DocumentsUploadForm(ref TopForm, ref InfiniumDocuments, sText, FilesDataTable, DocumentID, DocumentCategoryID);

                    TopForm = DocumentsUploadForm;

                    DocumentsUploadForm.ShowDialog();

                    if (DocumentsUploadForm.bOK)
                        ((InfiniumDocumentsUpdatesItem)sender).CloseCommentsTextBox();

                    PhantomForm.Close();
                    PhantomForm.Dispose();

                    TopForm = null;

                    CoverUpdatesList();

                    FillUpdates();

                    DocumentsUpdatesList.InitializeItems();
                    GC.Collect();

                    if (bNeedSplash)
                        bC = true;
                }
            }




        }

        private void DocumentsUpdatesList_EditCommentClicked(object sender, int DocumentCommentID)
        {
            DocumentsUpdatesList.Items[((InfiniumDocumentsUpdatesItem)sender).iItemIndex].CurrentFilesDataTable =
                InfiniumDocuments.GetCommentFiles(DocumentCommentID);

            if (((InfiniumDocumentsUpdatesItem)sender).CurrentFilesDataTable.Rows.Count > 0)
                ((InfiniumDocumentsUpdatesItem)sender).ControlPanel.CommentsTextBox.FilesLabel.Text =
                    ((InfiniumDocumentsUpdatesItem)sender).CurrentFilesDataTable.Rows.Count.ToString() + " файлов";
            else
                ((InfiniumDocumentsUpdatesItem)sender).ControlPanel.CommentsTextBox.FilesLabel.Text = "Прикрепить файлы";
        }

        private void DocumentsUpdatesList_DeleteCommentClicked(object sender, int DocumentCommentID)
        {
            bool OK = LightMessageBox.Show(ref TopForm, true,
                                   "Удалить комментарий?", "Удаление");

            if (!OK)
                return;

            CoverUpdatesList();

            InfiniumDocuments.RemoveComment(DocumentCommentID);
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();
            GC.Collect();


            if (bNeedSplash)
                bC = true;
        }


        private void NewUpdatesButton_Click(object sender, EventArgs e)
        {
            TodayUpdatesButton.Checked = false;
            WeekUpdatesButton.Checked = false;
            MonthUpdatesButton.Checked = false;

            CoverUpdatesList();

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void TodayUpdatesButton_Click(object sender, EventArgs e)
        {
            NewUpdatesButton.Checked = false;
            WeekUpdatesButton.Checked = false;
            MonthUpdatesButton.Checked = false;

            CoverUpdatesList();

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void WeekUpdatesButton_Click(object sender, EventArgs e)
        {
            NewUpdatesButton.Checked = false;
            TodayUpdatesButton.Checked = false;
            MonthUpdatesButton.Checked = false;

            CoverUpdatesList();

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void MonthUpdatesButton_Click(object sender, EventArgs e)
        {
            NewUpdatesButton.Checked = false;
            TodayUpdatesButton.Checked = false;
            WeekUpdatesButton.Checked = false;

            CoverUpdatesList();

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void DocumentsUpdatesList_FileClicked(object sender, int DocumentFileID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            try
            {

                DocumentsUploadForm DocumentsUploadForm = new DocumentsUploadForm(ref TopForm, ref InfiniumDocuments, DocumentFileID);

                TopForm = DocumentsUploadForm;

                DocumentsUploadForm.ShowDialog();
            }
            catch (Exception ex)
            {
                InfiniumMessages.SendMessage("Ошибка скачивания файла документов UserID = " + Security.CurrentUserID + ", ID = " + DocumentFileID + " exception = " + ex.Message, 321);
            }

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void DocumentsUpdatesList_CommentFileClicked(object sender, int DocumentCommentFileID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DocumentsUploadForm DocumentsUploadForm = new DocumentsUploadForm(ref TopForm, ref InfiniumDocuments, DocumentCommentFileID, true);

            TopForm = DocumentsUploadForm;

            DocumentsUploadForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }


        private void InnerDocumentsList_EditClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            if (InfiniumDocuments.IsAccessGrantedInner(Security.CurrentUserID, DocumentID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав для изменения документа", 3600);
                return;
            }

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            CreateInnerDocumentForm CreateDocumentForm = new CreateInnerDocumentForm(ref TopForm, ref InfiniumDocuments, DocumentID);

            TopForm = CreateDocumentForm;

            CreateDocumentForm.ShowDialog();

            InfiniumDocuments.FillInnerDocuments(GetFilter());
            InnerDocumentsList.InitializeItems();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void OuterDocumentsList_EditClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            if (InfiniumDocuments.IsAccessGrantedOuter(Security.CurrentUserID, DocumentID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав для изменения документа", 3600);
                return;
            }

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            CreateOuterDocumentForm CreateDocumentForm = new CreateOuterDocumentForm(ref TopForm, ref InfiniumDocuments, DocumentID);

            TopForm = CreateDocumentForm;

            CreateDocumentForm.ShowDialog();

            InfiniumDocuments.FillOuterDocuments(GetFilter());
            OuterDocumentsList.InitializeItems();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void IncomeDocumentsList_EditClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            if (InfiniumDocuments.IsAccessGrantedIncome(Security.CurrentUserID, DocumentID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав для изменения документа", 3600);
                return;
            }

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            CreateIncomeDocumentForm CreateDocumentForm = new CreateIncomeDocumentForm(ref TopForm, ref InfiniumDocuments, DocumentID);

            TopForm = CreateDocumentForm;

            CreateDocumentForm.ShowDialog();

            InfiniumDocuments.FillIncomeDocuments(GetFilter());
            IncomeDocumentsList.InitializeItems();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;
        }

        private void InnerDocumentsList_DeleteClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            if (InfiniumDocuments.IsAccessGrantedInner(Security.CurrentUserID, DocumentID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав для изменения документа", 3600);
                return;
            }

            bool OK = LightMessageBox.Show(ref TopForm, true,
                                    "Удалить выбранный документ?", "Удаление");

            if (!OK)
                return;

            CoverDocumentsList();

            InfiniumDocuments.RemoveInnerDocument(DocumentID);
            InfiniumDocuments.FillInnerDocuments(GetFilter());
            InnerDocumentsList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void OuterDocumentsList_DeleteClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            if (InfiniumDocuments.IsAccessGrantedOuter(Security.CurrentUserID, DocumentID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав для изменения документа", 3600);
                return;
            }

            bool OK = LightMessageBox.Show(ref TopForm, true,
                                    "Удалить выбранный документ?", "Удаление");

            if (!OK)
                return;

            CoverDocumentsList();

            InfiniumDocuments.RemoveOuterDocument(DocumentID);
            InfiniumDocuments.FillOuterDocuments(GetFilter());
            OuterDocumentsList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void IncomeDocumentsList_DeleteClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            if (InfiniumDocuments.IsAccessGrantedIncome(Security.CurrentUserID, DocumentID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Недостаточно прав для изменения документа", 3600);
                return;
            }

            bool OK = LightMessageBox.Show(ref TopForm, true,
                                    "Удалить выбранный документ?", "Удаление");

            if (!OK)
                return;

            CoverDocumentsList();

            InfiniumDocuments.RemoveIncomeDocument(DocumentID);
            InfiniumDocuments.FillIncomeDocuments(GetFilter());
            IncomeDocumentsList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void DocumentsUpdatesList_AddConfirmClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DocumentsConfirmationForm DocumentsConfirmationForm = new DocumentsConfirmationForm(ref TopForm, ref InfiniumDocuments, DocumentID, DocumentCategoryID);

            TopForm = DocumentsConfirmationForm;

            DocumentsConfirmationForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (!DocumentsConfirmationForm.bCanceled)
            {
                CoverUpdatesList();

                FillUpdates();

                DocumentsUpdatesList.InitializeItems();

                if (bNeedSplash)
                    bC = true;
            }
        }

        private void DocumentsUpdatesList_ConfirmItemConfirmClicked(object sender, int DocumentConfirmationRecipientID)
        {
            InfiniumDocuments.SetConfirmStatus(1, DocumentConfirmationRecipientID);
        }

        private void DocumentsUpdatesList_ConfirmDeleteClicked(object sender, int DocumentConfirmationID)
        {
            bool OK = LightMessageBox.Show(ref TopForm, true,
                                   "Удалить запрос подтверждения?", "Удаление");

            if (!OK)
                return;

            CoverUpdatesList();

            InfiniumDocuments.DeleteConfirm(DocumentConfirmationID);

            FillUpdates();

            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void DocumentsUpdatesList_ConfirmItemCancelClicked(object sender, int DocumentConfirmationRecipientID)
        {
            InfiniumDocuments.SetConfirmStatus(2, DocumentConfirmationRecipientID);
        }

        private void DocumentsUpdatesList_ConfirmEditClicked(object sender, int DocumentConfirmationID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            DocumentsConfirmationForm DocumentsConfirmationForm = new DocumentsConfirmationForm(ref TopForm, ref InfiniumDocuments, DocumentConfirmationID);

            TopForm = DocumentsConfirmationForm;

            DocumentsConfirmationForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (!DocumentsConfirmationForm.bCanceled)
            {
                CoverUpdatesList();

                FillUpdates();
                DocumentsUpdatesList.InitializeItems();

                if (bNeedSplash)
                    bC = true;
            }
        }

        private void DocumentsUpdatesList_ConfirmItemEditClicked(object sender, int DocumentConfirmationRecipientID)
        {
            InfiniumDocuments.SetConfirmStatus(0, DocumentConfirmationRecipientID);
        }

        private void YourSignButton_Click(object sender, EventArgs e)
        {
            NotConfirmedButton.Checked = false;
            ConfirmedButton.Checked = false;
            CanceledButton.Checked = false;

            CoverUpdatesList();
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void NotConfirmedButton_Click(object sender, EventArgs e)
        {
            YourSignButton.Checked = false;
            ConfirmedButton.Checked = false;
            CanceledButton.Checked = false;

            CoverUpdatesList();
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void ConfirmedButton_Click(object sender, EventArgs e)
        {
            NotConfirmedButton.Checked = false;
            YourSignButton.Checked = false;
            CanceledButton.Checked = false;

            CoverUpdatesList();
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void CanceledButton_Click(object sender, EventArgs e)
        {
            NotConfirmedButton.Checked = false;
            ConfirmedButton.Checked = false;
            YourSignButton.Checked = false;

            CoverUpdatesList();
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void InnerDocumentsList_ItemClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            CoverUpdatePanel();

            UpdatesPanel.BringToFront();
            CurrentDocumentPanel.BringToFront();
            EditButtonsPanel.Hide();

            CurrentDocumenID = DocumentID;
            CurrentDocumentCategoryID = DocumentCategoryID;

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void OuterDocumentsList_ItemClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            CoverUpdatePanel();

            UpdatesPanel.BringToFront();
            CurrentDocumentPanel.BringToFront();
            EditButtonsPanel.Hide();

            CurrentDocumenID = DocumentID;
            CurrentDocumentCategoryID = DocumentCategoryID;

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void IncomeDocumentsList_ItemClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            CoverUpdatePanel();

            UpdatesPanel.BringToFront();
            CurrentDocumentPanel.BringToFront();
            EditButtonsPanel.Hide();

            CurrentDocumenID = DocumentID;
            CurrentDocumentCategoryID = DocumentCategoryID;

            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void CurrentDocumentBackButton_Click(object sender, EventArgs e)
        {
            CoverUpdatePanel();

            AllDocumentsPanel.BringToFront();
            EditButtonsPanel.Show();

            if (bNeedSplash)
                bC = true;
        }

        private void MyDocCreatorButton_Click(object sender, EventArgs e)
        {
            MyDocRecipientButton.Checked = false;

            CoverUpdatesList();
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void MyDocRecipientButton_Click(object sender, EventArgs e)
        {
            MyDocCreatorButton.Checked = false;

            CoverUpdatesList();
            FillUpdates();
            DocumentsUpdatesList.InitializeItems();

            if (bNeedSplash)
                bC = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void DocumentsUpdatesList_AddUserClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddDocumentRecipientsForm AddDocumentRecipientsForm = new AddDocumentRecipientsForm(ref TopForm, ref InfiniumDocuments, DocumentID, DocumentCategoryID);

            TopForm = AddDocumentRecipientsForm;

            AddDocumentRecipientsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (!AddDocumentRecipientsForm.bCanceled)
            {
                CoverUpdatesList();

                FillUpdates();

                DocumentsUpdatesList.InitializeItems();

                if (bNeedSplash)
                    bC = true;
            }
        }

    }
}