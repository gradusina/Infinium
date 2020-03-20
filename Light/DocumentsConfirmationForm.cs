using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DocumentsConfirmationForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm;

        InfiniumDocuments InfiniumDocuments;

        public bool bStopTransfer = false;

        public bool bCanceled = false;

        public int DocumentID = -1;
        public int DocumentCategoryID = -1;
        public int DocumentConfirmationID = -1;

        bool bEdit = false;

        public DocumentsConfirmationForm(ref Form tTopForm, ref InfiniumDocuments tInfiniumDocuments, int iDocumentID, int iDocumentCategoryID)
        {
            InitializeComponent();

            InfiniumDocuments = tInfiniumDocuments;

            DocumentCategoryID = iDocumentCategoryID;
            DocumentID = iDocumentID;

            RecipientsList.ItemsDataTable = InfiniumDocuments.UsersDataTable;
            RecipientsList.InitializeItems();

            TopForm = tTopForm;
        }

        public DocumentsConfirmationForm(ref Form tTopForm, ref InfiniumDocuments tInfiniumDocuments, int iDocumentConfirmationID)
        {
            InitializeComponent();

            InfiniumDocuments = tInfiniumDocuments;

            DocumentConfirmationID = iDocumentConfirmationID;

            DataTable DT = InfiniumDocuments.GetConfirmationRecipients(iDocumentConfirmationID);

            RecipientsList.ItemsDataTable = InfiniumDocuments.UsersDataTable;
            RecipientsList.InitializeItems();

            bEdit = true;

            foreach (DataRow Row in DT.Rows)
            {
                RecipientsList.SelectItem(Convert.ToInt32(Row["UserID"]));
            }

            TopForm = tTopForm;
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
            bCanceled = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (RecipientsList.GetSelectedDataTable().Rows.Count == 0)
            {
                bCanceled = true;

                FormEvent = eClose;
                AnimateTimer.Enabled = true;
            }

            if (!bEdit)
                InfiniumDocuments.AddConfirm(RecipientsList.GetSelectedDataTable(), Security.CurrentUserID, DocumentID, DocumentCategoryID);
            else
                InfiniumDocuments.EditConfirm(DocumentConfirmationID, RecipientsList.GetSelectedDataTable());

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

    }
}
