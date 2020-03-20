using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DocumentAttributesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm;

        InfiniumFiles InfiniumDocuments;

        bool bC = false;

        public bool bCanceled = false;

        public bool bFirstSign = false;

        public DocumentAttributesForm(ref Infinium.InfiniumFiles tInfiniumDocuments, ref Form tTopForm)
        {
            InitializeComponent();

            InfiniumDocuments = tInfiniumDocuments;

            UsersComboBox.DataSource = InfiniumDocuments.UsersDataTable;
            UsersComboBox.DisplayMember = "Name";
            UsersComboBox.ValueMember = "UserID";

            InfiniumDocuments.CurrentSignsDataTable.Clear();
            InfiniumDocumentsUsersList.UsersDataTable = InfiniumDocuments.UsersDataTable;
            InfiniumDocumentsUsersList.ItemsDataTable = InfiniumDocuments.CurrentSignsDataTable;

            InfiniumDocuments.ClearAttributes();

            InfiniumDocumentsAttributesList.ItemsDataTable = InfiniumDocuments.DocumentAttributesDataTable;

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


            if (FormEvent == eShow)
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

        private void PictureViewForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
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

        private void CancButton_Click(object sender, EventArgs e)
        {
            bCanceled = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void infiniumDocumentsAttributesList_ItemClicked(object sender, string AttributeName)
        {
            if (AttributeName == "Подписи")
            {
                UsersComboBox.Visible = true;
                AddUserButton.Visible = true;
                InfiniumDocumentsUsersList.Visible = true;
                FirstSignCheckBox.Visible = true;

                InfiniumDocumentsUsersList.ItemsDataTable = InfiniumDocuments.CurrentSignsDataTable;
                InfiniumDocumentsUsersList.InitializeItems();
            }
            else
                if (AttributeName == "Ознакомлен")
            {
                UsersComboBox.Visible = true;
                AddUserButton.Visible = true;
                InfiniumDocumentsUsersList.Visible = true;
                FirstSignCheckBox.Visible = true;

                InfiniumDocumentsUsersList.ItemsDataTable = InfiniumDocuments.CurrentReadDataTable;
                InfiniumDocumentsUsersList.InitializeItems();
            }
            else
            {
                UsersComboBox.Visible = false;
                AddUserButton.Visible = false;
                InfiniumDocumentsUsersList.Visible = false;
                FirstSignCheckBox.Visible = false;
            }
        }

        private void DocumentSamlesForm_Load(object sender, EventArgs e)
        {

        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            if (InfiniumDocumentsAttributesList.Items[InfiniumDocumentsAttributesList.Selected].Caption == "Подписи")
            {
                if (InfiniumDocuments.CurrentSignsDataTable.Select("UserID = " + UsersComboBox.SelectedValue).Count() > 0)
                    return;

                DataRow NewRow = InfiniumDocuments.CurrentSignsDataTable.NewRow();
                NewRow["UserID"] = UsersComboBox.SelectedValue;
                InfiniumDocuments.CurrentSignsDataTable.Rows.Add(NewRow);

                InfiniumDocumentsUsersList.ItemsDataTable = InfiniumDocuments.CurrentSignsDataTable;
                InfiniumDocumentsUsersList.InitializeItems();
            }

            if (InfiniumDocumentsAttributesList.Items[InfiniumDocumentsAttributesList.Selected].Caption == "Ознакомлен")
            {
                if (InfiniumDocuments.CurrentReadDataTable.Select("UserID = " + UsersComboBox.SelectedValue).Count() > 0)
                    return;

                DataRow NewRow = InfiniumDocuments.CurrentReadDataTable.NewRow();
                NewRow["UserID"] = UsersComboBox.SelectedValue;
                InfiniumDocuments.CurrentReadDataTable.Rows.Add(NewRow);

                InfiniumDocumentsUsersList.ItemsDataTable = InfiniumDocuments.CurrentReadDataTable;

                InfiniumDocumentsUsersList.InitializeItems();
            }
        }

        private void infiniumDocumentsUsersList1_ItemRemoveClicked(object sender, int UserID)
        {
            DataRow[] Row = InfiniumDocuments.CurrentSignsDataTable.Select("UserID = " + UserID);

            Row[0].Delete();

            InfiniumDocumentsUsersList.InitializeItems();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            foreach (InfiniumFilesAttributeItem Item in InfiniumDocumentsAttributesList.Items)
            {
                InfiniumDocuments.DocumentAttributesDataTable.Select("AttributeName = '" + Item.Caption + "'")[0]["Value"] = Item.Checked;

                if (Item.Caption == "Подписи")
                    bFirstSign = FirstSignCheckBox.Checked;
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
