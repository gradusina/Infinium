using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminModulesAccessForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        public AdminModulesAccess AdminModulesAccess;


        public AdminModulesAccessForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void AdminModulesAccessForm_Shown(object sender, EventArgs e)
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

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }



        private void Initialize()
        {
            AdminModulesAccess = new AdminModulesAccess(ref ModulesDataGrid, ref UsersDataGrid);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            AdminModulesAccess.Save();
        }

        private void UsersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminModulesAccess == null)
                return;

            AdminModulesAccess.Filter();

            ModulesDataGrid.Refresh();
        }

        private void ModulesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminModulesAccess == null)
                return;
        }

        private void UsersDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void ModulesDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ModulesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ModulesDataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

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
    }
}
