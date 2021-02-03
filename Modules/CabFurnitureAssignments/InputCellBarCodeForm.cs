using Infinium.Modules.CabFurnitureAssignments;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class InputCellBarCodeForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public int cellId = -1;

        int FormEvent = 0;

        Form MainForm = null;
        StorePackagesManager storagePackagesManager;

        public InputCellBarCodeForm(Form tMainForm, StorePackagesManager SM)
        {
            MainForm = tMainForm;
            storagePackagesManager = SM;
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cellId == -1)
            {
                lbBarcode.Text = "Ячейка не выбрана";
                CheckPicture.Visible = true;
                CheckPicture.Image = Properties.Resources.cancel;
                lbBarcode.ForeColor = Color.FromArgb(240, 0, 0);
                return;
            }
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cellId = -1;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void tbBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckPicture.Visible = false;
                cellId = -1;

                if (tbBarcode.Text.Length < 12)
                {
                    tbBarcode.Clear();
                    lbBarcode.Text = "Неверный штрихкод";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    lbBarcode.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }

                string Prefix = tbBarcode.Text.Substring(0, 3);

                if (Prefix != "023")
                {
                    tbBarcode.Clear();
                    lbBarcode.Text = "Это не штрихкод ячейки";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    lbBarcode.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }

                cellId = Convert.ToInt32(tbBarcode.Text.Substring(3, 9));
                if (!storagePackagesManager.IsCellExist(cellId))
                {
                    cellId = -1;
                    tbBarcode.Clear();
                    lbBarcode.Text = "Ячейки не существует";
                    CheckPicture.Visible = true;
                    CheckPicture.Image = Properties.Resources.cancel;
                    lbBarcode.ForeColor = Color.FromArgb(240, 0, 0);
                    return;
                }

                lbBarcode.Text = tbBarcode.Text;
                tbBarcode.Clear();

                //привязка к ячейке
                CheckPicture.Visible = true;
                CheckPicture.Image = Properties.Resources.OK;
                lbBarcode.ForeColor = Color.FromArgb(82, 169, 24);
            }

        }

        private void tbBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                if (tbBarcode.Text.Length >= 12 && e.KeyChar != (char)8)
                    e.Handled = true;
            }
        }
    }
}
