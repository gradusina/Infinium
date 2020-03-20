using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminLoginJournalForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        public AdminLoginJournal AdminLoginJournal = null;


        public AdminLoginJournalForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();


            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void AdminLoginJournalForm_Shown(object sender, EventArgs e)
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
            AdminLoginJournal = new AdminLoginJournal(ref JournalDataGrid);

            UsersDataGrid.DataSource = AdminLoginJournal.UsersBingingSource;
            UsersDataGrid.Columns["UserID"].Visible = false;
            UsersDataGrid.Columns["ShortName"].Visible = false;
            UsersDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            UsersDataGrid.Columns["Name"].MinimumWidth = 150;

            //UsersListBox.DataSource = AdminLoginJournal.UsersBingingSource;
            //UsersListBox.DisplayMember = "Name";
            //UsersListBox.ValueMember = "UserID";

            int UserID = 0;

            if (!AllUsersCheckBox.Checked)
                UserID = Convert.ToInt32(UsersDataGrid.SelectedRows[0].Cells["UserID"].Value);


            AdminLoginJournal.Filter(UserID, CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, CodersCheckBox.Checked);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            int UserID = 0;

            if (!AllUsersCheckBox.Checked)
                UserID = Convert.ToInt32(UsersDataGrid.SelectedRows[0].Cells["UserID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            AdminLoginJournal.Filter(UserID, CalendarFrom.SelectionEnd, CalendarTo.SelectionEnd, CodersCheckBox.Checked);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
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
