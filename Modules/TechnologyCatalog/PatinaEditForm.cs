using Infinium.Modules.TechnologyCatalog;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PatinaEditForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        PatinaManager PManager;

        public PatinaEditForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            PManager = new PatinaManager();

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

        private void AdminResponsibilitiesForm_Shown(object sender, EventArgs e)
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

        private void StaffListForm_Load(object sender, EventArgs e)
        {
            dgvGridsSettings();
        }

        private void dgvGridsSettings()
        {
            dgvPatina.DataSource = PManager.PatinaBS;
            dgvPatinaRal.DataSource = PManager.PatinaRalBS;

            dgvPatina.Columns["PatinaID"].HeaderText = "ID";
            dgvPatina.Columns["PatinaName"].HeaderText = "Патина";
            dgvPatina.Columns["DisplayName"].HeaderText = "Отображаемое\r\nимя";

            dgvPatinaRal.Columns["PatinaID"].Visible = false;
            dgvPatinaRal.Columns["PatinaRALID"].HeaderText = "ID";
            dgvPatinaRal.Columns["PatinaRAL"].HeaderText = "Патина RAL";
            dgvPatinaRal.Columns["DisplayName"].HeaderText = "Отображаемое\r\nимя";
            dgvPatinaRal.Columns["Enabled"].HeaderText = "Используется";

            dgvPatina.Columns["PatinaID"].Width = 45;

            dgvPatinaRal.Columns["PatinaRALID"].Width = 75;
            dgvPatinaRal.Columns["DisplayName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void btnSavePatina_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvPatina.Rows.Count; i++)
            {
                if (dgvPatina.Rows[i].IsNewRow)
                    continue;
                if (dgvPatina.Rows[i].Cells["DisplayName"].FormattedValue.ToString().Length == 0)
                {
                    LightMessageBox.Show(ref TopForm, false, "Патина: Поле \"Отображаемое имя\" должно быть заполнено", "Ошибка сохранения");
                    return;
                }
            }
            for (int i = 0; i < dgvPatinaRal.Rows.Count; i++)
            {
                if (dgvPatinaRal.Rows[i].IsNewRow)
                    continue;
                if (dgvPatinaRal.Rows[i].Cells["DisplayName"].FormattedValue.ToString().Length == 0)
                {
                    LightMessageBox.Show(ref TopForm, false, "Патина RAL: Поле \"Отображаемое имя\" должно быть заполнено", "Ошибка сохранения");
                    return;
                }
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();
            while (!SplashWindow.bSmallCreated) ;

            try
            {
                PManager.SavePatina();
                PManager.SavePatinaRal();
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

        private void dgvPatina_SelectionChanged(object sender, EventArgs e)
        {
            int PatinaID = 0;
            if (dgvPatina.SelectedRows.Count > 0 && dgvPatina.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(dgvPatina.SelectedRows[0].Cells["PatinaID"].Value);
            PManager.FilterPatinaRAL(PatinaID);
        }

        private void dgvPatinaRal_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            int PatinaID = -1;
            if (dgvPatina.SelectedRows.Count > 0 && dgvPatina.SelectedRows[0].Cells["PatinaID"].Value != DBNull.Value)
                PatinaID = Convert.ToInt32(dgvPatina.SelectedRows[0].Cells["PatinaID"].Value);
            e.Row.Cells["PatinaID"].Value = PatinaID;
            e.Row.Cells["Enabled"].Value = true;
        }

        private void dgvPatina_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void dgvPatinaRal_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Удалить позицию?",
                    "Удаление");

            if (OKCancel)
            {
                PManager.RemoveCurrentPatina();
            }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Удалить позицию?",
                    "Удаление");

            if (OKCancel)
            {
                PManager.RemoveCurrentPatinaRal();
            }
        }

        int CurrentPatinaIDRALID = 0;
        private Rectangle dragBoxFromMouseDown;
        private object valueFromMouseDown;
        private void dgvPatinaRal_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgvPatinaRal.DoDragDrop(valueFromMouseDown, DragDropEffects.Move);
                }
            }
        }

        private void dgvPatina_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = dgvPatina.PointToClient(new Point(e.X, e.Y));
            if (e.Effect == DragDropEffects.Move)
            {
                //int DepartmentID = (int)e.Data.GetData(typeof(int));
                var hittest = dgvPatina.HitTest(clientPoint.X, clientPoint.Y);
                if (hittest.ColumnIndex != -1 && hittest.RowIndex != -1 && CurrentPatinaIDRALID != 0)
                {
                    PManager.ChangePatinaGroup(CurrentPatinaIDRALID, Convert.ToInt32(dgvPatina.Rows[hittest.RowIndex].Cells["PatinaID"].Value));
                    InfiniumTips.ShowTip(this, 50, 85, "Патина перенесена в группу \"" +
                        dgvPatina.Rows[hittest.RowIndex].Cells["PatinaName"].Value.ToString() + "\"", 1700);
                }
            }
        }

        private void dgvPatina_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dgvPatinaRal_MouseDown(object sender, MouseEventArgs e)
        {
            var hittestInfo = dgvPatinaRal.HitTest(e.X, e.Y);

            if (dgvPatinaRal.Rows[hittestInfo.RowIndex].IsNewRow)
            {
                dragBoxFromMouseDown = Rectangle.Empty;
                valueFromMouseDown = null;
                CurrentPatinaIDRALID = 0;
                return;
            }
            if (hittestInfo.RowIndex != -1 && hittestInfo.ColumnIndex != -1)
            {
                valueFromMouseDown = dgvPatinaRal.Rows[hittestInfo.RowIndex].Cells[hittestInfo.ColumnIndex].Value;
                if (dgvPatinaRal.Rows[hittestInfo.RowIndex].Cells["PatinaRALID"].Value != DBNull.Value)
                    CurrentPatinaIDRALID = Convert.ToInt32(dgvPatinaRal.Rows[hittestInfo.RowIndex].Cells["PatinaRALID"].Value);
                if (valueFromMouseDown != null)
                {
                    // Remember the point where the mouse down occurred. 
                    // The DragSize indicates the size that the mouse can move 
                    // before a drag event should be started.                
                    Size dragSize = SystemInformation.DragSize;

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }
    }
}
