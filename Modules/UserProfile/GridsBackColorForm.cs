using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class GridsBackColorForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        Form MainForm = null;

        public GridsBackColorForm()
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        private void GridsBackColorForm_Shown(object sender, EventArgs e)
        {
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
            DataTable DT = new DataTable();

            DT.Columns.Add(new DataColumn("№ заказа", Type.GetType("System.Int32")));
            DT.Columns.Add(new DataColumn("Клиент", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Фасад", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Цвет профиля", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Вставка", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Цвет наполнителя", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Высота", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Ширина", Type.GetType("System.String")));
            DT.Columns.Add(new DataColumn("Кол-во", Type.GetType("System.String")));

            percentageDataGrid1.DataSource = DT;

            for (int i = 0; i < 15; i++)
            {
                DataRow NewRow = DT.NewRow();
                NewRow["№ заказа"] = 5;
                NewRow["Клиент"] = "ИП Иванов";
                NewRow["Фасад"] = "София";
                NewRow["Цвет профиля"] = "ПП Вишня тёмная";
                NewRow["Вставка"] = "филенка";
                NewRow["Цвет наполнителя"] = "ПП Вишня";
                NewRow["Высота"] = 713;
                NewRow["Ширина"] = 396;
                NewRow["Кол-во"] = 1;
                DT.Rows.Add(NewRow);
            }

            percentageDataGrid1.Columns["№ заказа"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            percentageDataGrid1.Columns["№ заказа"].Width = 90;
            percentageDataGrid1.Columns["Высота"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            percentageDataGrid1.Columns["Высота"].Width = 80;
            percentageDataGrid1.Columns["Ширина"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            percentageDataGrid1.Columns["Ширина"].Width = 80;
            percentageDataGrid1.Columns["Кол-во"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            percentageDataGrid1.Columns["Кол-во"].Width = 80;

            colorPickEdit1.Color = Security.GridsBackColor;

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

        private void colorPickEdit1_EditValueChanged(object sender, EventArgs e)
        {
            percentageDataGrid1.StateCommon.Background.Color1 = colorPickEdit1.Color;
            percentageDataGrid1.StateCommon.DataCell.Back.Color1 = colorPickEdit1.Color;
        }

        private void OKMessageButton_Click(object sender, EventArgs e)
        {
            Security.SetStandardFormBackColor(colorPickEdit1.Color);
        }
    }
}
