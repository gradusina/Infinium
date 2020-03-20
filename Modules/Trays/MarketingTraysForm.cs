using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingTraysForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int BarcodeType = 6; // Маркетинг

        int FormEvent = 0;
        int CurrentRowIndex = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;


        Infinium.Modules.Packages.Trays.MarketingTraysManager MarketingTraysManager;
        Infinium.Modules.Packages.Trays.TrayLabel TrayLabel;
        Infinium.Modules.Dispatch.MarketingTrayList MarketingTrayList;

        public MarketingTraysForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void ZOVTraysForm_Shown(object sender, EventArgs e)
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


        private void Initialize()
        {
            MarketingTraysManager = new Modules.Packages.Trays.MarketingTraysManager(ref PackagesDataGrid, ref TraysDataGrid,
                ref MainOrdersFrontsOrdersDataGrid, ref MainOrdersDecorOrdersDataGrid, ref MainOrdersTabControl);

            TrayLabel = new Modules.Packages.Trays.TrayLabel();
            MarketingTraysManager.Initialize();

            MarketingTrayList = new Modules.Dispatch.MarketingTrayList();
        }

        private void PackagesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null)
                return;

            if (MarketingTraysManager.PackagesBindingSource.Count < 1)
            {
                MarketingTraysManager.ClearOrdersGrid();
                return;
            }

            if (((DataRowView)MarketingTraysManager.PackagesBindingSource.Current)["PackNumber"] != DBNull.Value)
            {
                int ProductType = Convert.ToInt32(((DataRowView)MarketingTraysManager.PackagesBindingSource.Current)["ProductType"]);

                int PackageID = Convert.ToInt32(((DataRowView)MarketingTraysManager.PackagesBindingSource.Current)["PackageID"]);
                MarketingTraysManager.Filter(PackageID, ProductType);

                if (ProductType == 0)
                {
                    MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[0];
                }
                if (ProductType == 1)
                {
                    MainOrdersTabControl.SelectedTabPage = MainOrdersTabControl.TabPages[1];
                }
            }
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

        private void TraysDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null || MarketingTraysManager.TraysBindingSource.Count < 1)
                return;

            if (((DataRowView)MarketingTraysManager.TraysBindingSource.Current)["TrayID"] != DBNull.Value)
            {
                int TrayID = Convert.ToInt32(((DataRowView)MarketingTraysManager.TraysBindingSource.Current)["TrayID"]);
                MarketingTraysManager.FilterPackages(TrayID);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (MarketingTraysManager != null)
                MarketingTraysManager.AddTray();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null || MarketingTraysManager.TraysBindingSource.Count < 1)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Поддон будет расформирован. Продолжить?",
                    "Удаление поддона");

            if (OKCancel)
            {
                int TrayID = Convert.ToInt32(((DataRowView)MarketingTraysManager.TraysBindingSource.Current).Row["TrayID"]);
                MarketingTraysManager.RemoveTray(TrayID);
            }
        }

        private void PrintTrayLabelButton_Click(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null || MarketingTraysManager.TraysBindingSource.Count < 1)
                return;

            int[] Trays = MarketingTraysManager.GetTrayIDs();

            TrayLabel.ClearLabelInfo();
            Infinium.Modules.Packages.Trays.Info LabelInfo = new Modules.Packages.Trays.Info();

            for (int j = 0; j < Trays.Count(); j++)
            {
                LabelInfo.GroupType = "Маркетинг";

                LabelInfo.TrayID = Trays[j];
                LabelInfo.BarcodeNumber = TrayLabel.GetBarcodeNumber(BarcodeType, Trays[j]);
                TrayLabel.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = TrayLabel.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                TrayLabel.Print();
            }
        }

        private bool Filter()
        {
            bool NotFormed = NotFormedCheckBox.Checked;
            bool Formed = FormedCheckBox.Checked;
            bool Dispatched = DispatchedCheckBox.Checked;

            return MarketingTraysManager.FilterTrays(NotFormed, Formed, Dispatched);

        }

        private void NotFormedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null)
                return;

            if (!Filter())
            {
                MarketingTraysManager.ClearPackagesGrid();
                MarketingTraysManager.ClearOrdersGrid();
            }
        }

        private void FormedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null)
                return;

            if (!Filter())
            {
                MarketingTraysManager.ClearPackagesGrid();
                MarketingTraysManager.ClearOrdersGrid();
            }
        }

        private void DispatchedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null)
                return;

            if (!Filter())
            {
                MarketingTraysManager.ClearPackagesGrid();
                MarketingTraysManager.ClearOrdersGrid();
            }
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            MarketingTraysManager.TraysBindingSource.Position = CurrentRowIndex;
            int TrayID = Convert.ToInt32(((DataRowView)MarketingTraysManager.TraysBindingSource.Current)["TrayID"]);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            MarketingTrayList.CreateReport(TrayID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TraysDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int[] Trays = MarketingTraysManager.GetTrayIDs();

            MarketingTrayList.SummaryTrayReport(Trays);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ExpeditionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MarketingTraysManager == null)
                return;

            if (!Filter())
            {
                MarketingTraysManager.ClearPackagesGrid();
                MarketingTraysManager.ClearOrdersGrid();
            }
        }
    }
}