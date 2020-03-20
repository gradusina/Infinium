using Infinium.Modules.Marketing.NewOrders;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DecorSizesExportForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        AddMarketingNewOrdersForm MainForm = null;

        DecorOrders DecorOrders = null;

        public DecorSizesExportForm(AddMarketingNewOrdersForm tMainForm, ref DecorOrders cDecorOrders)
        {
            MainForm = tMainForm;
            DecorOrders = cDecorOrders;
            InitializeComponent();
            Initialize();
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

        }

        private void CupboardsExportForm_Load(object sender, EventArgs e)
        {
            DecorOrders.ImportFromSizeTable(ref SizeTableDataGrid);
        }

        private void SizeTableOKButton_Click(object sender, EventArgs e)
        {
            MainForm.AddDecorFromSizeTable(ref SizeTableDataGrid);
            SizeTableDataGrid.Rows.Clear();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void SizeTableDeleteButton_Click(object sender, EventArgs e)
        {
            if (SizeTableDataGrid.Rows.Count > 1)
                if (SizeTableDataGrid.CurrentRow.Index != SizeTableDataGrid.Rows.Count - 1)
                    SizeTableDataGrid.Rows.Remove(SizeTableDataGrid.CurrentRow);
        }

        private void SizeTableCancelButton_Click(object sender, EventArgs e)
        {
            SizeTableDataGrid.Rows.Clear();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
