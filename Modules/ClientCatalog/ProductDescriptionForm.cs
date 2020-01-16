using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ProductDescriptionForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public bool PressOK = false;

        public bool ToSite = false;
        public bool Latest = false;
        public string Category = string.Empty;
        public string NameProd = string.Empty;
        public string Description = string.Empty;
        public string Sizes = string.Empty;
        public string Material = string.Empty;
        int FormEvent = 0;

        Form MainForm = null;

        public ProductDescriptionForm(Form tMainForm, bool bToSite, bool bLatest, string sCategory, string sName, string sDescription, string sSizes, string sMaterial)
        {
            MainForm = tMainForm;
            Category = sCategory;
            NameProd = sName;
            Description = sDescription;
            Sizes = sSizes;
            Material = sMaterial;

            InitializeComponent();

            kryptonRichTextBox4.Text = Category;
            kryptonRichTextBox5.Text = NameProd;
            kryptonRichTextBox1.Text = Description;
            kryptonRichTextBox2.Text = Sizes;
            kryptonRichTextBox3.Text = Material;
            cbToSite.Checked = bToSite;
            cbLatest.Checked = bLatest;
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
                        MainForm.Activate();
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
                        MainForm.Activate();
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

        private void btnOKInput_Click(object sender, EventArgs e)
        {
            PressOK = true;
            Category = kryptonRichTextBox4.Text;
            NameProd = kryptonRichTextBox5.Text;
            Description = kryptonRichTextBox1.Text;
            Sizes = kryptonRichTextBox2.Text;
            Material = kryptonRichTextBox3.Text;
            ToSite = cbToSite.Checked;
            Latest = cbLatest.Checked;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelInput_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ProductDescriptionForm_Load(object sender, EventArgs e)
        {

        }
    }
}
