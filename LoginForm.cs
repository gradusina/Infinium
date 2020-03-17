using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Infinium
{
    public partial class LoginForm : InfiniumLoginForm
    {
        Security Security = null;
        Form TopForm = null;
        bool WaitForEnter = false;
        Connection Connection;

        public void CloseJournalRec()
        {
            Security.CloseJournalRecord();
        }

        public LoginForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Connection = new Connection();
            //ConnectionStrings.CatalogConnectionString = Connection.GetConnectionString(CommonVariables.CatalogConnectionString);
            //ConnectionStrings.LightConnectionString = Connection.GetConnectionString(CommonVariables.LightConnectionString);
            //ConnectionStrings.MarketingOrdersConnectionString = Connection.GetConnectionString(CommonVariables.MarketingOrdersConnectionString);
            //ConnectionStrings.MarketingReferenceConnectionString = Connection.GetConnectionString(CommonVariables.MarketingReferenceConnectionString);
            //ConnectionStrings.StorageConnectionString = Connection.GetConnectionString(CommonVariables.StorageConnectionString);
            //ConnectionStrings.UsersConnectionString = Connection.GetConnectionString(CommonVariables.UsersConnectionString);
            //ConnectionStrings.ZOVOrdersConnectionString = Connection.GetConnectionString(CommonVariables.ZOVOrdersConnectionString);
            //ConnectionStrings.ZOVReferenceConnectionString = Connection.GetConnectionString(CommonVariables.ZOVReferenceConnectionString);

            ConnectionStrings.UsersConnectionString = Connection.GetConnectionString("ConnectionUsers.config");
            ConnectionStrings.CatalogConnectionString = Connection.GetConnectionString("ConnectionCatalog.config");
            ConnectionStrings.LightConnectionString = Connection.GetConnectionString("ConnectionLight.config");
            ConnectionStrings.MarketingOrdersConnectionString = Connection.GetConnectionString("ConnectionMarketingOrders.config");
            ConnectionStrings.MarketingReferenceConnectionString = Connection.GetConnectionString("ConnectionMarketingReference.config");
            ConnectionStrings.StorageConnectionString = Connection.GetConnectionString("ConnectionStorage.config");
            ConnectionStrings.ZOVOrdersConnectionString = Connection.GetConnectionString("ConnectionZOVOrders.config");
            ConnectionStrings.ZOVReferenceConnectionString = Connection.GetConnectionString("ConnectionZOVReference.config");

            //ConnectionStrings.CatalogConnectionString = Connection.GetConnectionString("ConnectionCatalog.config");
            //ConnectionStrings.LightConnectionString = Connection.GetConnectionString("ConnectionLight.config");
            //ConnectionStrings.MarketingOrdersConnectionString = Connection.GetConnectionString("ConnectionMarketingOrders.config");
            //ConnectionStrings.MarketingReferenceConnectionString = Connection.GetConnectionString("ConnectionMarketingReference.config");
            //ConnectionStrings.StorageConnectionString = Connection.GetConnectionString("ConnectionStorage.config");
            //ConnectionStrings.UsersConnectionString = Connection.GetConnectionString("ConnectionUsers.config");
            //ConnectionStrings.ZOVOrdersConnectionString = Connection.GetConnectionString("ConnectionZOVOrders.config");
            //ConnectionStrings.ZOVReferenceConnectionString = Connection.GetConnectionString("ConnectionZOVReference.config");

            DatabaseConfigsManager DatabaseConfigsManager = new DatabaseConfigsManager();
            DatabaseConfigsManager.ReadAnimationFlag("Animation.config");
            Configs.DocumentsPath = DatabaseConfigsManager.ReadConfig("DocumentsPath.config");
            Configs.DocumentsZOVTPSPath = DatabaseConfigsManager.ReadConfig("DocumentsZOVTPSPath.config");
            Configs.DocumentsPathHost = DatabaseConfigsManager.ReadConfig("DocumentsPathHost.config");
            Configs.FTPType = Convert.ToInt32(DatabaseConfigsManager.ReadConfig("FTP.config", 1, 0));

            AnimateTimer.Enabled = true;
        }


        private void Binding()
        {
            LoginComboBox.DataSource = Security.UsersBindingSource;
            LoginComboBox.DisplayMember = Security.UsersBindingSourceDisplayMember;
            LoginComboBox.ValueMember = Security.UsersBindingSourceValueMember;

            int CurID = Security.GetCurrentUserLogin();

            if (CurID == -1)
                return;
            else
                try
                {
                    LoginComboBox.SelectedIndex = Security.UsersBindingSource.Find("UserID", CurID);
                    //Security.CheckConding(CurID);
                }
                catch 
                {
                    LoginComboBox.SelectedIndex = 0;
                }
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (DatabaseConfigsManager.Animation == false)
            {
                this.Opacity = 1;
                return;
            }

            if (this.Opacity != 1)
                this.Opacity += 0.05;
            else
                AnimateTimer.Enabled = false;
        }

        private void LoginComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                PasswordComboBox.Focus();
        }



        private void LogInButton_Click(object sender, EventArgs e)
        {

            if (Security.Enter(Convert.ToInt32(LoginComboBox.SelectedValue), PasswordComboBox.Text) == 0)
            {
                PasswordComboBox.ResetText();
                return;
            }

            if (Security.Enter(Convert.ToInt32(LoginComboBox.SelectedValue), PasswordComboBox.Text) == -1)
            {
                DialogResult result = MessageBox.Show("Хотите закрыть уже запущенную версию Инфиниума?", "Закрытие", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    Security.SetForciblyOffline(Convert.ToInt32(LoginComboBox.SelectedValue), true);
                    MessageBox.Show("Подождите 5 секунд и нажмите Войти заново", "Вход");
                    //LogInButton.Enabled = false;
                    return;
                }
                else
                {
                    NoAccessPanel.Visible = true;
                    PasswordComboBox.ResetText();
                    return;
                }
            }

            Security.SetCurrentUserLogin(Convert.ToInt32(LoginComboBox.SelectedValue));

            Security.CreateJournalRecord();

            Thread T = new Thread(delegate() { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            LightStartForm LightStartForm = new LightStartForm(this);
            LightStartForm.Show();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {           
            this.Close();
        }

        private void PasswordComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void LoginComboBox_SelectionChanged(object sender, EventArgs e)
        {
            PasswordComboBox.Focus();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (Control.FromHandle(m.LParam) == null)
                {
                    if (TopForm != null)
                        TopForm.Activate();
                }
            }
        }

        private void CurrentTimeTimer_Tick(object sender, EventArgs e)
        {
            string tempTime = CurrentTimeLabel.Text;

            CurrentTimeLabel.Text = DateTime.Now.ToString("HH:mm");
            CurrentDayOfWeekLabel.Text = DateTime.Now.ToString("dddd");
            CurrentDayMonthLabel.Text = DateTime.Now.ToString("dd MMMM");

            if (CurrentTimeLabel.Text != tempTime)
            {
                CurrentDayOfWeekLabel.Left = GetTimeLabelLength(CurrentTimeLabel.Text) + CurrentTimeLabel.Left;
                CurrentDayMonthLabel.Left = GetTimeLabelLength(CurrentTimeLabel.Text) + CurrentTimeLabel.Left;
            }
            if (WaitForEnter)
            {

            }
        }

        private int GetTimeLabelLength(string Text)
        {
            int W = 0;
            
            Graphics G = this.CreateGraphics();
            Font F = new System.Drawing.Font("Segoe UI Semilight", 90, FontStyle.Regular, GraphicsUnit.Pixel);

            W = Convert.ToInt32(G.MeasureString(Text, F).Width);

            F.Dispose();
            G.Dispose();

            return W;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

            Security = new Infinium.Security();

            if (!Security.Initialize())
            {
                MessageBox.Show("Не удалось подключится к базе данных. Возможные причины: не работает сервер баз данных, нет доступа к сети или интернет. Обратитесь к системному администратору");
                this.Close();
                Application.Exit();
                return;
            }

            Binding();

            int CurID = Security.GetCurrentUserLogin();

            if (CurID == 322 && ConnectionStrings.StorageConnectionString == @"Data Source=v02.bizneshost.by, 32433;Initial Catalog=Storage;Persist Security Info=True;User ID=hercules;Password=1q2w3e4r")
            {
                LoginComboBox.SelectedIndex = Security.UsersBindingSource.Find("UserID", 322);
                PasswordComboBox.Text = "gradus";
                LogInButton_Click(null, null);
            }
            //if (Security.IsConding)
            //{
            //    LoginComboBox.SelectedIndex = Security.UsersBindingSource.Find("UserID", 322);
            //    PasswordComboBox.Text = "gradus";
            //    LogInButton_Click(null, null);
            //}
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LogInButton_Click(null, null);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
