using System;
using System.Linq;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CreateFolderForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        public bool Canceled = false;

        public string FolderName = "";

        Form TopForm;

        Infinium.InfiniumFiles InfiniumDocuments;

        public int ProjectID = -1;

        public CreateFolderForm(ref Infinium.InfiniumFiles tInfiniumDocuments, ref Form tTopForm)
        {
            InitializeComponent();

            TopForm = tTopForm;
            InfiniumDocuments = tInfiniumDocuments;
        }

        public CreateFolderForm(ref Infinium.InfiniumProjects tInfiniumProjects, ref Form tTopForm, int ProjectID)//edit
        {
            //InitializeComponent();

            //TopForm = tTopForm;
            //InfiniumProjects = tInfiniumProjects;

            //bEdit = true;

            //ProjectNameTextEdit.Text = InfiniumProjects.ProjectsDataTable.Select("ProjectID = " + ProjectID)[0]["ProjectName"].ToString();
            //ProjectDescriptionRichTextBox.Text = InfiniumProjects.ProjectsDataTable.Select("ProjectID = " + ProjectID)[0]["ProjectDescription"].ToString();

            //AllUsersNotifyCheckBox.Enabled = false;
            //AllUsersNotifyCheckBox.StateCommon.ShortText.Color1 = Color.LightGray;

            //SetMembersTree();
            //CheckMembersTree();
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

        private void CreateFolderForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void CancelFolderButton_Click(object sender, EventArgs e)
        {
            Canceled = true;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void OKFolderButton_Click(object sender, EventArgs e)
        {
            string res = FolderNameTextBox.Text.Trim();

            //check symbols
            {
                if (res.IndexOf(@"*") > -1 || res.IndexOf(@"/") > -1 ||
                   res.IndexOf(@"\") > -1 || res.IndexOf(@"|") > -1 ||
                   res.IndexOf(@"?") > -1 ||
                   res.IndexOf(@":") > -1 || res.IndexOf(@"<") > -1 ||
                   res.IndexOf(@">") > -1 || res.IndexOf(@"[") > -1 || res.IndexOf(@"]") > -1)
                {
                    InfiniumTips.ShowTip(this, 50, 85, "Название папки содержит недопустимые символы", 2600);
                    return;
                }
            }

            if (res.Length == 0)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Название папки не может быть пустым", 2600);
                return;
            }

            //check exists 
            if (InfiniumDocuments.CurrentItemsDataTable.Select("Extension = 'folder'" + " AND ItemName = '" + res + "'").Count() > 0)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Папка с таким именем уже существует", 2600);
                return;
            }

            FolderName = res;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void FolderNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKFolderButton_Click(null, null);
        }
    }
}
