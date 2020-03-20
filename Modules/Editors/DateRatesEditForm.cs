using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DateRatesEditForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        DateRatesEdit DateRatesEdit;

        public DateRatesEditForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            DateRatesEdit = new DateRatesEdit();

            while (!SplashForm.bCreated) ;
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

        private void DateRatesEditForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void DateRatesEditForm_Load(object sender, EventArgs e)
        {
            dgvDateRatesSettings();
        }

        private void dgvDateRatesSettings()
        {
            dgvDateRates.DataSource = DateRatesEdit.DateRatesBS;

            if (dgvDateRates.Columns.Contains("DateRateID"))
                dgvDateRates.Columns["DateRateID"].Visible = false;

            dgvDateRates.Columns["Date"].HeaderText = "Дата";
        }

        private void btnDeleteResponsibility_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Будут загружены новые курсы валют",
                    "Удаление курсов");

            if (OKCancel)
            {
                DateTime Date = Convert.ToDateTime(dgvDateRates.SelectedRows[0].Cells["Date"].Value);
                int DateRateID = Convert.ToInt32(dgvDateRates.SelectedRows[0].Cells["DateRateID"].Value);
                DateRatesEdit.DeleteDateRate(DateRateID);
                bool b = DateRatesEdit.GetDateRates(Date);
                DateRatesEdit.SaveDateRates();
                InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
                if (!b)
                    Infinium.LightMessageBox.Show(ref TopForm, false,
                         "Курс не взят. Возможная причина - нет связи с банком без авторизации в kerio control. Войдите в kerio и повторите попытку",
                         "Новые курсы валют");
            }
        }
    }
}
