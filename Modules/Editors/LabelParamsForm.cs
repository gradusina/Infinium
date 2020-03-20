using Infinium.Modules.Editors;

using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class LabelParamsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        LabelParamsManager ParamsManager;

        public LabelParamsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


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

        private void ClientsManagersForm_Shown(object sender, EventArgs e)
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

        private void ClientsManagersForm_Load(object sender, EventArgs e)
        {
            ParamsManager = new LabelParamsManager();
            dgvParams.DataSource = ParamsManager.ParamsBS;
            dgvGridsSettings();
        }

        private void dgvGridsSettings()
        {
            dgvParams.Columns["LabelParamID"].Visible = false;

            dgvParams.Columns["Param"].HeaderText = "Параметр";
            dgvParams.Columns["RussianParam"].HeaderText = "Русский";
            dgvParams.Columns["PolishParam"].HeaderText = "Польский";
            dgvParams.Columns["EnglishParam"].HeaderText = "Английский";

            dgvParams.Columns["Param"].ReadOnly = true;

            dgvParams.Columns["Param"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvParams.Columns["Param"].MinimumWidth = 70;

            dgvParams.Columns["RussianParam"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvParams.Columns["RussianParam"].MinimumWidth = 150;
            dgvParams.Columns["PolishParam"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvParams.Columns["PolishParam"].MinimumWidth = 150;
            dgvParams.Columns["EnglishParam"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvParams.Columns["EnglishParam"].MinimumWidth = 150;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                ParamsManager.Save();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
            }
        }
    }
}
