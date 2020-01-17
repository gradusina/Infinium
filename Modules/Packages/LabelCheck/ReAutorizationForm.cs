using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ReAutorizationForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public bool PressOK = false;
        public int UserID = 0;
        int FormEvent = 0;

        Form MainForm;

        DataTable UsersDataTable = null;
        BindingSource UsersBindingSource = null;

        public ReAutorizationForm(Form tMainForm)
        {
            MainForm = tMainForm;
            InitializeComponent();
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

        private void ReAutorizationForm_Load(object sender, EventArgs e)
        {
            UsersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, Coding, PriceAccess, Password, AuthorizationCode FROM Users WHERE Fired <> 1 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }
            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            LoginComboBox.DataSource = UsersBindingSource;
            LoginComboBox.DisplayMember = "Name";
            LoginComboBox.ValueMember = "UserID";

            try
            {
                LoginComboBox.SelectedIndex = UsersBindingSource.Find("UserID", Security.CurrentUserID);
            }
            catch
            {
                LoginComboBox.SelectedIndex = 0;
            }
        }

        private string GetMD5(string text)
        {
            using (MD5 Hasher = MD5.Create())
            {
                byte[] data = Hasher.ComputeHash(Encoding.Default.GetBytes(text));

                StringBuilder sBuilder = new StringBuilder();

                //преобразование в HEX
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public int AccessToScaning(int UserID, string Password)
        {
            DataRow[] Rows = UsersDataTable.Select("UserID = " + UserID);
            string passMD5 = GetMD5(Password);

            if (Rows[0]["Password"].ToString() == passMD5)
            {
                return 1;
            }

            return 0;
        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            if (AccessToScaning(Convert.ToInt32(LoginComboBox.SelectedValue), PasswordComboBox.Text) == 0)
            {
                PasswordComboBox.ResetText();
                return;
            }
            UserID = Convert.ToInt32(LoginComboBox.SelectedValue);
            PressOK = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            PressOK = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void PasswordComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LogInButton_Click(null, null);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
