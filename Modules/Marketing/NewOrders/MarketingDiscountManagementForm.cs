using Infinium.Modules.Marketing.NewOrders;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingDiscountManagementForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        DiscountsManager DiscountsManager;

        public MarketingDiscountManagementForm(LightStartForm tLightStartForm)
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

        private void MarketingDiscountManagementForm_Shown(object sender, EventArgs e)
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

        private void MarketingDiscountManagementForm_Load(object sender, EventArgs e)
        {
            DiscountsManager = new DiscountsManager();
            dgvDiscountOrderSum.DataSource = DiscountsManager.DiscountOrderSumBS;
            dgvDiscountVolume.DataSource = DiscountsManager.DiscountVolumeBS;
            dgvDiscountPaymentConditions.DataSource = DiscountsManager.DiscountPaymentConditionsBS;
            dgvDiscountFactoring.DataSource = DiscountsManager.DiscountFactoringBS;
            dgvGridsSettings();
        }

        private void dgvGridsSettings()
        {
            dgvDiscountOrderSum.Columns["DiscountOrderSumID"].Visible = false;
            dgvDiscountOrderSum.Columns["MinSum"].HeaderText = "Min сумма";
            dgvDiscountOrderSum.Columns["MaxSum"].HeaderText = "Max сумма";
            dgvDiscountOrderSum.Columns["Discount"].HeaderText = "Скидка";

            dgvDiscountVolume.Columns["DiscountVolumeID"].Visible = false;
            dgvDiscountVolume.Columns["MinVolume"].HeaderText = "Min объем";
            dgvDiscountVolume.Columns["MaxVolume"].HeaderText = "Max объем";
            dgvDiscountVolume.Columns["Discount"].HeaderText = "Скидка";

            dgvDiscountPaymentConditions.Columns["DiscountPaymentConditionID"].Visible = false;
            dgvDiscountPaymentConditions.Columns["Name"].HeaderText = "Условие";
            dgvDiscountPaymentConditions.Columns["Discount"].HeaderText = "Скидка";

            dgvDiscountFactoring.Columns.Add(DiscountsManager.CurrencyTypeColumn);
            dgvDiscountFactoring.Columns["DiscountFactoringID"].Visible = false;
            dgvDiscountFactoring.Columns["CurrencyTypeID"].Visible = false;
            dgvDiscountFactoring.Columns["Name"].HeaderText = "Название";
            dgvDiscountFactoring.Columns["Discount"].HeaderText = "Скидка";
            dgvDiscountFactoring.Columns["DaysCount"].HeaderText = "Кол-во дней";
        }

        private void btnSaveDiscountOrderSum_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvDiscountOrderSum.Rows.Count; i++)
            {
                if (dgvDiscountOrderSum.Rows[i].IsNewRow)
                    continue;
                if (dgvDiscountOrderSum.Rows[i].Cells["Discount"].FormattedValue.ToString().Length == 0)
                {
                    LightMessageBox.Show(ref TopForm, false, "Поле \"Скидка\" должно быть заполнено", "Ошибка сохранения");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                DiscountsManager.SaveDiscountOrderSum();
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

        private void dgvDiscountOrderSum_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["DiscountOrderSumID"].Value = DiscountsManager.NextDiscountOrderSumID();
        }

        private void dgvDiscountOrderSum_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Удалить позицию?",
                    "Удаление");

            if (OKCancel)
            {
                DiscountsManager.RemoveDiscountOrderSum();
            }
        }

        private void dgvDiscountVolume_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvDiscountPaymentConditions_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu3.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvDiscountFactoring_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu4.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Удалить позицию?",
                    "Удаление");

            if (OKCancel)
            {
                DiscountsManager.RemoveDiscountVolume();
            }
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Удалить позицию?",
                    "Удаление");

            if (OKCancel)
            {
                DiscountsManager.RemoveDiscountPaymentCondition();
            }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                                "Удалить позицию?",
                                "Удаление");

            if (OKCancel)
            {
                DiscountsManager.RemoveDiscountFactoring();
            }
        }

        private void btnSaveDiscountVolume_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvDiscountVolume.Rows.Count; i++)
            {
                if (dgvDiscountVolume.Rows[i].IsNewRow)
                    continue;
                if (dgvDiscountVolume.Rows[i].Cells["Discount"].FormattedValue.ToString().Length == 0)
                {
                    LightMessageBox.Show(ref TopForm, false, "Поле \"Скидка\" должно быть заполнено", "Ошибка сохранения");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                DiscountsManager.SaveDiscountVolume();
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

        private void btnSaveDiscountPaymentConditions_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvDiscountPaymentConditions.Rows.Count; i++)
            {
                if (dgvDiscountPaymentConditions.Rows[i].IsNewRow)
                    continue;
                if (dgvDiscountPaymentConditions.Rows[i].Cells["Discount"].FormattedValue.ToString().Length == 0)
                {
                    LightMessageBox.Show(ref TopForm, false, "Поле \"Скидка\" должно быть заполнено", "Ошибка сохранения");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                DiscountsManager.SaveDiscountPaymentConditions();
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

        private void btnSavedgvDiscountFactoring_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvDiscountFactoring.Rows.Count; i++)
            {
                if (dgvDiscountFactoring.Rows[i].IsNewRow)
                    continue;
                if (dgvDiscountFactoring.Rows[i].Cells["Discount"].FormattedValue.ToString().Length == 0)
                {
                    LightMessageBox.Show(ref TopForm, false, "Поле \"Скидка\" должно быть заполнено", "Ошибка сохранения");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                DiscountsManager.SaveDiscountFactoring();
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

        private void dgvDiscountVolume_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["DiscountVolumeID"].Value = DiscountsManager.NextDiscountVolumeID();
        }

        private void dgvDiscountPaymentConditions_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["DiscountPaymentConditionID"].Value = DiscountsManager.NextDiscountPaymentConditionID();
        }

        private void dgvDiscountFactoring_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["DiscountFactoringID"].Value = DiscountsManager.NextDiscountFactoringID();
        }
    }
}
