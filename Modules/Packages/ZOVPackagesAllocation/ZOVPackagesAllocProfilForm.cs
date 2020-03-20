using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVPackagesAllocProfilForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;
        const int FactoryID = 1;

        const string PackCount = "ProfilPackCount";
        const string PackAllocStatusID = "ProfilPackAllocStatusID";

        int FormEvent = 0;
        int PackType = 0;

        bool NeedRefresh = false;
        bool NeedSplash = false;

        ZOVSplitPackagesForm SplitPackagesForm;
        Form TopForm = null;
        LightStartForm LightStartForm;


        private Modules.Packages.ZOV.ZOVPackagesAllocManager PackagesAllocManager;

        public ZOVPackagesAllocProfilForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();
            PackagesAllocManager.CheckPackages();
            PackedCheckButton.Checked = false;
            Filter();
            while (!SplashForm.bCreated) ;
        }

        private void ZOVPackagesAllocTPSForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            NeedSplash = true;
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
                    NeedSplash = true;
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
                    NeedSplash = true;
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
            NeedSplash = false;
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
            PackagesAllocManager = new Modules.Packages.ZOV.ZOVPackagesAllocManager(
                ref MainOrdersDataGrid, ref MainOrdersFrontsOrdersDataGrid, ref MegaOrdersDataGrid,
                ref MainOrdersDecorOrdersDataGrid, ref MainOrdersTabControl, FactoryID);

            DocNumbersComboBox.DataSource = PackagesAllocManager.DocNumbersBindingSource;
            DocNumbersComboBox.DisplayMember = "DocNumber";
            DocNumbersComboBox.ValueMember = "DocNumber";
        }

        private void MegaOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PackagesAllocManager != null)
                if (PackagesAllocManager.MegaOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)PackagesAllocManager.MegaOrdersBindingSource.Current)["MegaOrderID"] != DBNull.Value)
                    {
                        if (NeedSplash)
                        {
                            NeedSplash = false;
                            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                            T.Start();

                            while (!SplashWindow.bSmallCreated) ;

                            PackagesAllocManager.FilterMainOrders(Convert.ToInt32(((DataRowView)PackagesAllocManager.MegaOrdersBindingSource.Current)["MegaOrderID"]));

                            while (SplashWindow.bSmallCreated)
                                SmallWaitForm.CloseS = true;
                            NeedSplash = true;
                        }
                        else
                            PackagesAllocManager.FilterMainOrders(Convert.ToInt32(((DataRowView)PackagesAllocManager.MegaOrdersBindingSource.Current)["MegaOrderID"]));
                    }
                }
        }

        private void MainOrdersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (PackagesAllocManager != null)
                if (PackagesAllocManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)PackagesAllocManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        PackagesAllocManager.Filter(Convert.ToInt32(((DataRowView)PackagesAllocManager.MainOrdersBindingSource.Current)["MainOrderID"]));

                        if (MainOrdersTabControl.TabPages[0].Visible && MainOrdersTabControl.TabPages[1].Visible)
                            MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];

                        NextPackNumberLabel.Text = (PackagesAllocManager.MainOrderPackCount + PackagesAllocManager.GetPackCount + 1).ToString();

                        if (Convert.ToInt32(((DataRowView)PackagesAllocManager.MainOrdersBindingSource.Current)[PackAllocStatusID]) == 2)
                        {
                            SavePackagesButton.Enabled = false;
                            SplitPackagesButton.Enabled = false;
                        }
                        else
                        {
                            SavePackagesButton.Enabled = true;
                            SplitPackagesButton.Enabled = true;
                        }
                    }
                }
                else
                {
                    PackagesAllocManager.Filter(-1);
                    NextPackNumberLabel.Text = "";
                }
        }

        private void MegaOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MegaOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void MegaOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        private void MainOrdersDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (NeedRefresh == true)
            {
                MainOrdersDataGrid.Refresh();
                NeedRefresh = false;
            }
        }

        private void Filter()
        {
            PackagesAllocManager.FilterMegaOrders(PackedCheckButton.Checked,
                NonPackedCheckButton.Checked);
            PackagesAllocManager.FilterMainOrders(PackedCheckButton.Checked,
                NonPackedCheckButton.Checked);
        }

        private void MainOrdersFrontsOrdersDataGrid_KeyDown(object sender, KeyEventArgs e)
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

                string r = MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].FormattedValue.ToString();

                MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = r + c.ToString();
            }

            if (e.KeyCode == Keys.Delete)
                MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = DBNull.Value;

            if (e.KeyCode == Keys.Back)
            {
                string r = MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].FormattedValue.ToString();
                if (r.Length > 1)
                {
                    r = r.Substring(0, r.Length - 1);
                    MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = r;
                }
                else
                {
                    if (r.Length == 1)
                    {
                        MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = DBNull.Value;
                    }
                }
            }
        }

        private void MainOrdersDecorOrdersDataGrid_KeyDown(object sender, KeyEventArgs e)
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

                string r = MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].FormattedValue.ToString();

                MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = r + c.ToString();
            }

            if (e.KeyCode == Keys.Delete)
                MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = DBNull.Value;

            if (e.KeyCode == Keys.Back)
            {
                string r = MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].FormattedValue.ToString();
                if (r.Length > 1)
                {
                    r = r.Substring(0, r.Length - 1);
                    MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = r;
                }
                else
                {
                    if (r.Length == 1)
                    {
                        MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["PackNumber"].Value = DBNull.Value;
                    }
                }
            }
        }

        private void SavePackagesButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение упаковок.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (!PackagesAllocManager.IsSimpleAndCurved)
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;

                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "В одной упаковке не могут лежать гнутые и негнутые фасады. Перепакуйте",
                   "Ошибка сохранения упаковок");
                return;
            }

            if (PackagesAllocManager.IsOverflow == false)
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;

                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "В упаковке больше 6 позиций. Перепакуйте",
                   "Ошибка сохранения упаковок");
                return;
            }

            if (PackagesAllocManager.IsConsequence == false)
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;

                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Упаковки в заказе непоследовательны!",
                   "Ошибка сохранения упаковок");
                return;
            }

            if (PackagesAllocManager.IsDifferentPackNumbers == false)
            {
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;

                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Фасады и декор находятся в одной упаковке",
                   "Ошибка сохранения упаковок");
                return;
            }

            if (MainOrdersDataGrid.SelectedRows.Count > 0 &&
                Convert.ToInt32(MainOrdersDataGrid.SelectedRows[0].Cells["FactoryID"].Value) == 0)
            {
                if (PackagesAllocManager.PackagesMainOrdersFrontsOrders.FrontsOrdersBindingSource.Count > 0)
                {
                    if (!PackagesAllocManager.PackagesMainOrdersFrontsOrders.IsPacked)
                    {
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        Infinium.LightMessageBox.Show(ref TopForm, false,
                          "Заказ на обеих фирмах. Не все фасады запакованы",
                          "Ошибка сохранения упаковок");
                        return;
                    }
                }

                if (PackagesAllocManager.PackagesMainOrdersDecorOrders.DecorOrdersBindingSource.Count > 0)
                {
                    if (!PackagesAllocManager.PackagesMainOrdersDecorOrders.IsPacked)
                    {
                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;

                        Infinium.LightMessageBox.Show(ref TopForm, false,
                          "Заказ на обеих фирмах. Не весь декор запакован",
                          "Ошибка сохранения упаковок");
                        return;
                    }
                }
            }
            PackagesAllocManager.SavePackageDetails();
            PackagesAllocManager.SetPackStatus();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void PackedCheckButton_CheckedChanged(object sender, EventArgs e)
        {
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            Filter();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void NonPackedCheckButton_CheckedChanged(object sender, EventArgs e)
        {
            NeedSplash = false;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            Filter();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            NeedSplash = true;
        }

        private void SplitPackagesButton_Click(object sender, EventArgs e)
        {
            if (PackType == 0)//fronts
            {
                if (MainOrdersFrontsOrdersDataGrid.Rows.Count < 1 ||
                    Convert.ToInt32(MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["Count"].Value) < 2)
                    return;

                Infinium.Modules.Packages.ZOV.SplitStruct SS = new Modules.Packages.ZOV.SplitStruct();

                SplitPackagesForm = new ZOVSplitPackagesForm();
                SplitPackagesForm.SetSplitPositions(Convert.ToInt32(MainOrdersFrontsOrdersDataGrid.SelectedRows[0].Cells["Count"].Value),
                                                    ref SS);

                TopForm = SplitPackagesForm;
                SplitPackagesForm.ShowDialog();
                TopForm = null;

                if (SplitPackagesForm.AccessibleName == "true")
                {
                    SplitPackagesForm.Dispose();
                    SplitPackagesForm = null;
                    GC.Collect();
                }

                if (SS.IsSplit)
                    PackagesAllocManager.PackagesMainOrdersFrontsOrders.SplitCurrentItem(SS);
            }

            if (PackType == 1)//decor
            {
                if (MainOrdersDecorOrdersDataGrid.Rows.Count < 1 ||
                    Convert.ToInt32(MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["Count"].Value) < 2)
                    return;

                Infinium.Modules.Packages.ZOV.SplitStruct SS = new Modules.Packages.ZOV.SplitStruct();

                SplitPackagesForm = new ZOVSplitPackagesForm();
                SplitPackagesForm.SetSplitPositions(
                            Convert.ToInt32(MainOrdersDecorOrdersDataGrid.SelectedRows[0].Cells["Count"].Value),
                            ref SS);

                TopForm = SplitPackagesForm;
                SplitPackagesForm.ShowDialog();
                TopForm = null;

                if (SplitPackagesForm.AccessibleName == "true")
                {
                    SplitPackagesForm.Dispose();
                    SplitPackagesForm = null;
                    GC.Collect();
                }

                if (SS.IsSplit)
                    PackagesAllocManager.PackagesMainOrdersDecorOrders.SplitCurrentItem(SS);
            }
        }

        private void MainOrdersFrontsOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PackType = 0;
        }

        private void MainOrdersDecorOrdersDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            PackType = 1;
        }

        private void ClearPackageButton_Click(object sender, EventArgs e)
        {
            if (PackagesAllocManager != null)
                if (PackagesAllocManager.MainOrdersBindingSource.Count > 0)
                {
                    if (((DataRowView)PackagesAllocManager.MainOrdersBindingSource.Current)["MainOrderID"] != DBNull.Value)
                    {
                        bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                            "В данном подзаказе очистятся ВСЕ упаковки. Распечатанные этикетки будут недействительны. Продолжить?", "Внимание");

                        if (!OKCancel)
                        {
                            return;
                        }

                        NeedSplash = false;
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Очистка упаковок.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        PackagesAllocManager.FixOrderEvent(Convert.ToInt32(((DataRowView)PackagesAllocManager.MainOrdersBindingSource.Current)["MainOrderID"]),
                            "Упаковки очищены, UserID=" + Security.CurrentUserID);
                        PackagesAllocManager.ClearPackage();
                        PackagesAllocManager.SetPackStatus();

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                        NeedSplash = true;
                    }
                }
        }


        private void MainOrdersFrontsOrdersDataGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 24 && !string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {

                if (int.TryParse(e.FormattedValue.ToString(), out int NewPackNumber))
                {
                    if (NewPackNumber <= PackagesAllocManager.MainOrderPackCount)
                        MainOrdersFrontsOrdersDataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DBNull.Value;
                }
            }
            PackagesAllocManager.PackagesMainOrdersFrontsOrders.FrontsOrdersBindingSource.EndEdit();
            NextPackNumberLabel.Text = (PackagesAllocManager.MainOrderPackCount + PackagesAllocManager.GetPackCount + 1).ToString();
        }

        private void MainOrdersDecorOrdersDataGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 19 && !string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {

                if (int.TryParse(e.FormattedValue.ToString(), out int NewPackNumber))
                {
                    if (NewPackNumber <= PackagesAllocManager.MainOrderPackCount)
                        MainOrdersDecorOrdersDataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DBNull.Value;
                }
            }
            PackagesAllocManager.PackagesMainOrdersDecorOrders.DecorOrdersBindingSource.EndEdit();
            NextPackNumberLabel.Text = (PackagesAllocManager.MainOrderPackCount + PackagesAllocManager.GetPackCount + 1).ToString();
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

        private void DocNumbersComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && checkBox1.Checked)
            {
                string DocNumber = "-1";
                if (DocNumbersComboBox.FindStringExact(DocNumbersComboBox.Text) > -1)
                    DocNumber = DocNumbersComboBox.SelectedValue.ToString();
                PackagesAllocManager.FindDocNumber(DocNumber);
            }
        }

        private void DocNumbersComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (DocNumbersComboBox.Items.Count > 0 && checkBox1.Checked)
            {
                //PackedCheckButton.Checked = false;
                //NonPackedCheckButton.Checked = false;
                PackagesAllocManager.FindDocNumber(DocNumbersComboBox.SelectedValue.ToString());
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (DocNumbersComboBox.Items.Count > 0 && checkBox1.Checked)
            {
                NeedSplash = false;
                PackedCheckButton.Checked = true;
                NonPackedCheckButton.Checked = true;
                string DocNumber = "-1";
                if (DocNumbersComboBox.FindStringExact(DocNumbersComboBox.Text) > -1)
                    DocNumber = DocNumbersComboBox.SelectedValue.ToString();
                PackagesAllocManager.FindDocNumber(DocNumber);
                NeedSplash = true;
            }

            if (!checkBox1.Checked)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
                Filter();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }
    }
}