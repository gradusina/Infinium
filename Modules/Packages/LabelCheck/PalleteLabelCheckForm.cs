using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PalleteLabelCheckForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        PalleteTypeAction TypeAction;
        LightStartForm LightStartForm;


        public PalleteCheckLabel CheckLabel;

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public PalleteLabelCheckForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            while (!SplashForm.bCreated) ;
        }

        private void PalleteLabelCheckForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            BarcodeTextBox.Focus();
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

        private int GetChar(KeyEventArgs e)
        {
            int c = -1;

            if (e.KeyCode == Keys.NumPad1 || e.KeyCode == Keys.NumPad2 || e.KeyCode == Keys.NumPad3 || e.KeyCode == Keys.NumPad4 ||
                e.KeyCode == Keys.NumPad5 || e.KeyCode == Keys.NumPad6 || e.KeyCode == Keys.NumPad7 || e.KeyCode == Keys.NumPad8 ||
                e.KeyCode == Keys.NumPad9 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.D0 || e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 || e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 || e.KeyCode == Keys.D9 || e.KeyCode == Keys.D0)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad1:
                        { c = 1; }
                        break;
                    case Keys.NumPad2:
                        { c = 2; }
                        break;
                    case Keys.NumPad3:
                        { c = 3; }
                        break;
                    case Keys.NumPad4:
                        { c = 4; }
                        break;
                    case Keys.NumPad5:
                        { c = 5; }
                        break;
                    case Keys.NumPad6:
                        { c = 6; }
                        break;
                    case Keys.NumPad7:
                        { c = 7; }
                        break;
                    case Keys.NumPad8:
                        { c = 8; }
                        break;
                    case Keys.NumPad9:
                        { c = 9; }
                        break;
                    case Keys.NumPad0:
                        { c = 0; }
                        break;


                    case Keys.D1:
                        { c = 1; }
                        break;
                    case Keys.D2:
                        { c = 2; }
                        break;
                    case Keys.D3:
                        { c = 3; }
                        break;
                    case Keys.D4:
                        { c = 4; }
                        break;
                    case Keys.D5:
                        { c = 5; }
                        break;
                    case Keys.D6:
                        { c = 6; }
                        break;
                    case Keys.D7:
                        { c = 7; }
                        break;
                    case Keys.D8:
                        { c = 8; }
                        break;
                    case Keys.D9:
                        { c = 9; }
                        break;
                    case Keys.D0:
                        { c = 0; }
                        break;
                }


            }
            return c;
        }


        private void Initialize()
        {
            CheckLabel = new PalleteCheckLabel();

            lbMegaBatch.Text = string.Empty;
            lbUserName.Text = string.Empty;
            lbWeekNumber.Text = string.Empty;
            lbNumberOfChange.Text = string.Empty;
            lbPalleteNumber.Text = string.Empty;
            lbDocDateTime.Text = string.Empty;
            lbFactory.Text = string.Empty;
            lbGroup.Text = string.Empty;
            dgvDecorSettings();
        }

        private void dgvDecorSettings()
        {
            dgvDecorContent.DataSource = CheckLabel.DecorPackContentBindingSource;
            dgvDecorContent.ReadOnly = true;

            dgvDecorContent.Columns["ProductName"].HeaderText = "Продукт";
            dgvDecorContent.Columns["Name"].HeaderText = "Наименование";
            dgvDecorContent.Columns["ColorName"].HeaderText = "Цвет";
            dgvDecorContent.Columns["Height"].HeaderText = "Высота";
            dgvDecorContent.Columns["Length"].HeaderText = "Длина";
            dgvDecorContent.Columns["Width"].HeaderText = "Ширина";
            dgvDecorContent.Columns["Count"].HeaderText = "Кол-во";

            dgvDecorContent.Columns["ProductName"].DisplayIndex = 0;
            dgvDecorContent.Columns["Name"].DisplayIndex = 1;
            dgvDecorContent.Columns["ColorName"].DisplayIndex = 2;
            dgvDecorContent.Columns["Height"].DisplayIndex = 3;
            dgvDecorContent.Columns["Length"].DisplayIndex = 4;
            dgvDecorContent.Columns["Width"].DisplayIndex = 5;
            dgvDecorContent.Columns["Count"].DisplayIndex = 6;

            dgvDecorContent.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecorContent.Columns["ProductName"].MinimumWidth = 120;
            dgvDecorContent.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecorContent.Columns["Name"].MinimumWidth = 120;
            dgvDecorContent.Columns["ColorName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDecorContent.Columns["ColorName"].MinimumWidth = 145;
            dgvDecorContent.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorContent.Columns["Height"].Width = 85;
            dgvDecorContent.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorContent.Columns["Length"].Width = 85;
            dgvDecorContent.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorContent.Columns["Width"].Width = 85;
            dgvDecorContent.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDecorContent.Columns["Count"].Width = 85;

            foreach (DataGridViewColumn Column in dgvDecorContent.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void BarcodeTextBox_Leave(object sender, EventArgs e)
        {
            CheckTimer.Enabled = true;
        }

        private void SaveButton_MouseUp(object sender, MouseEventArgs e)
        {
            BarcodeTextBox.Focus();
        }

        private void NavigateMenuCloseButton_MouseUp(object sender, MouseEventArgs e)
        {
            BarcodeTextBox.Focus();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (!BarcodeTextBox.Focused)
            {
                BarcodeTextBox.Focus();
            }
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double G = 0;

            if (e.KeyCode == Keys.Enter)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                BarcodeLabel.Text = string.Empty;
                CheckPicture.Visible = false;

                CheckLabel.Clear();

                if (BarcodeTextBox.Text.Length < 12)
                {
                    BarcodeTextBox.Clear();

                    lbMegaBatch.Text = string.Empty;
                    lbUserName.Text = string.Empty;
                    lbWeekNumber.Text = string.Empty;
                    lbNumberOfChange.Text = string.Empty;
                    lbPalleteNumber.Text = string.Empty;
                    lbDocDateTime.Text = string.Empty;
                    lbFactory.Text = string.Empty;
                    lbGroup.Text = string.Empty;

                    return;
                }

                BarcodeLabel.Text = BarcodeTextBox.Text;

                BarcodeTextBox.Clear();

                int PalleteID = Convert.ToInt32(BarcodeLabel.Text.Substring(3, 9));

                if (CheckLabel.CheckBarcode(BarcodeLabel.Text))
                {
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.OK;
                    BarcodeLabel.ForeColor = Color.FromArgb(82, 169, 24);
                    CheckLabel.GetPalleteParameters(BarcodeLabel.Text);
                    CheckLabel.FillDecorPackContent(BarcodeLabel.Text);
                    if (TypeAction == PalleteTypeAction.PackingPallete)
                        CheckLabel.SetPacked(BarcodeLabel.Text);
                    if (TypeAction == PalleteTypeAction.DispatchPallete)
                        CheckLabel.SetDispatched(BarcodeLabel.Text);

                    lbMegaBatch.Text = CheckLabel.LabelInfo.MegaBatchID.ToString();
                    lbNumberOfChange.Text = CheckLabel.LabelInfo.NumberOfChange.ToString();
                    lbPalleteNumber.Text = PalleteID.ToString();
                    lbUserName.Text = CheckLabel.LabelInfo.UserName;
                    lbWeekNumber.Text = CheckLabel.LabelInfo.WeekNumber.ToString();
                    lbDocDateTime.Text = CheckLabel.LabelInfo.DocDateTime;
                    if (CheckLabel.LabelInfo.FactoryType == 1)
                        lbFactory.Text = "ЗОВ-Профиль";
                    if (CheckLabel.LabelInfo.FactoryType == 2)
                        lbFactory.Text = "ЗОВ-ТПС";
                    lbGroup.Text = CheckLabel.LabelInfo.GroupType;
                }
                else
                {
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    BarcodeLabel.ForeColor = Color.FromArgb(240, 0, 0);

                    lbMegaBatch.Text = string.Empty;
                    lbUserName.Text = string.Empty;
                    lbWeekNumber.Text = string.Empty;
                    lbNumberOfChange.Text = string.Empty;
                    lbPalleteNumber.Text = string.Empty;
                    lbDocDateTime.Text = string.Empty;
                    lbFactory.Text = string.Empty;
                    lbGroup.Text = string.Empty;
                }

                sw.Stop();
                G = sw.Elapsed.TotalMilliseconds;
            }
        }

        private void BarcodeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                if (BarcodeTextBox.Text.Length >= 12 && e.KeyChar != (char)8)
                    e.Handled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetActiveWindow() != this.Handle)
            {
                this.Activate();
            }
        }

        private void PalleteLabelCheckForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet2.CheckedButton == ckbtnPacking)
            {
                TypeAction = PalleteTypeAction.PackingPallete;
            }

            if (kryptonCheckSet2.CheckedButton == ckbtnDispatch)
            {
                TypeAction = PalleteTypeAction.DispatchPallete;
            }
        }

    }
}
