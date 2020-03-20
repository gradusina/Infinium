using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddInventoryRestForm : Form
    {
        public static bool IsOKPress = true;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        decimal CurrentCount = 0;
        public decimal FactCount = 0;
        public string Notes = string.Empty;

        Form TopForm = null;

        public AddInventoryRestForm(decimal iCurrentCount, string sInvNotes)
        {
            InitializeComponent();

            CurrentCount = iCurrentCount;
            Notes = sInvNotes;

            Initialize();

            NotesRichTextBox.Text = Notes.ToString();
            CountTextBox.Text = CurrentCount.ToString();
            CountTextBox.Focus();
            CountTextBox.SelectAll();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
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

        private void AddInventoryRestForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void AddInventoryRestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void OKInvButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CountTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введите остаток",
                    "Ошибка");
                CountTextBox.Focus();
                CountTextBox.SelectAll();
                return;
            }

            FactCount = Convert.ToDecimal(CountTextBox.Text);

            if ((FactCount != CurrentCount || CurrentCount < FactCount) && string.IsNullOrWhiteSpace(NotesRichTextBox.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Введите примечание",
                    "Ошибка");
                NotesRichTextBox.Focus();
                NotesRichTextBox.SelectAll();
                return;
            }

            Notes = NotesRichTextBox.Text;

            IsOKPress = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelInvButton_Click(object sender, EventArgs e)
        {
            IsOKPress = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CountTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
