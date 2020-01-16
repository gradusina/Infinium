using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MoveToPersonMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public bool PressOK = false;
        public int PersonID = 0;
        public string PersonName = string.Empty;
        public object IncomeDateTime = null;

        int FormEvent = 0;

        DataTable UsersDataTable;
        Form MainForm = null;

        public MoveToPersonMenu(Form tMainForm)
        {
            MainForm = tMainForm;
            InitializeComponent();
            Initialize();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            IncomeDateTime = MovementDateTimePicker.Value;
            if (UnknownPersonButton.Checked)
            {
                PersonID = 0;
                if (PersonTextBox.Text.Length == 0)
                    return;

                PersonName = PersonTextBox.Text;
            }
            else
            {
                PersonID = Convert.ToInt32(((DataRowView)PersonsComboBox.SelectedItem).Row["UserID"]);
                PersonName = ((DataRowView)PersonsComboBox.SelectedItem).Row["ShortName"].ToString();
            }
            PressOK = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelButtonButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
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

        private void Initialize()
        {
            UsersDataTable = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter(
                "SELECT UserID, Name, ShortName FROM Users  WHERE Fired <> 1 ORDER BY Name",
                ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }

            PersonsComboBox.DataSource = new DataView(UsersDataTable);
            PersonsComboBox.DisplayMember = "Name";
            PersonsComboBox.ValueMember = "UserID";
        }

        private void KnownPersonButton_CheckedChanged(object sender, EventArgs e)
        {
            UnknownPersonButton.Checked = !KnownPersonButton.Checked;
            PersonsComboBox.Enabled = KnownPersonButton.Checked;
            PersonTextBox.Enabled = !KnownPersonButton.Checked;

            if (KnownPersonButton.Checked)
                PersonsComboBox.Focus();
        }

        private void UnknownPersonButton_CheckedChanged(object sender, EventArgs e)
        {
            KnownPersonButton.Checked = !UnknownPersonButton.Checked;
            PersonsComboBox.Enabled = !UnknownPersonButton.Checked;
            PersonTextBox.Enabled = UnknownPersonButton.Checked;

            if (UnknownPersonButton.Checked)
                PersonTextBox.Focus();
        }
    }
}
