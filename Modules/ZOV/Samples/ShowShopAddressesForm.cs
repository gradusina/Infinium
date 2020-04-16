using Infinium.Modules.ZOV.Samples;

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ShowShopAddressesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form MainForm = null;

        public ShowShopAddressesForm(Form tMainForm, OrdersManager tOrdersManager, int FirmType, int ClientID)
        {
            MainForm = tMainForm;
            InitializeComponent();
            ShopAddressesDataGrid.DataSource = tOrdersManager.FillShopAddressesDataTable(FirmType, ClientID);


            if (ShopAddressesDataGrid.Columns.Contains("Address"))
                ShopAddressesDataGrid.Columns["Address"].HeaderText = "Адрес";
            if (ShopAddressesDataGrid.Columns.Contains("City"))
                ShopAddressesDataGrid.Columns["City"].HeaderText = "Город";
            if (ShopAddressesDataGrid.Columns.Contains("Country"))
                ShopAddressesDataGrid.Columns["Country"].HeaderText = "Страна";
            if (ShopAddressesDataGrid.Columns.Contains("Lat"))
                ShopAddressesDataGrid.Columns["Lat"].HeaderText = "Широта";
            if (ShopAddressesDataGrid.Columns.Contains("Long"))
                ShopAddressesDataGrid.Columns["Long"].HeaderText = "Долгота";
            if (ShopAddressesDataGrid.Columns.Contains("Worktime"))
                ShopAddressesDataGrid.Columns["Worktime"].HeaderText = "Время работы";
            if (ShopAddressesDataGrid.Columns.Contains("Name"))
                ShopAddressesDataGrid.Columns["Name"].HeaderText = "Название";
            if (ShopAddressesDataGrid.Columns.Contains("Site"))
                ShopAddressesDataGrid.Columns["Site"].HeaderText = "Сайт";
            if (ShopAddressesDataGrid.Columns.Contains("IsFronts"))
                ShopAddressesDataGrid.Columns["IsFronts"].HeaderText = "Фасады";
            if (ShopAddressesDataGrid.Columns.Contains("IsProfile"))
                ShopAddressesDataGrid.Columns["IsProfile"].HeaderText = "Погонаж";
            if (ShopAddressesDataGrid.Columns.Contains("IsFurniture"))
                ShopAddressesDataGrid.Columns["IsFurniture"].HeaderText = "Мебель";
            if (ShopAddressesDataGrid.Columns.Contains("Email"))
                ShopAddressesDataGrid.Columns["Email"].HeaderText = "Email";
            if (ShopAddressesDataGrid.Columns.Contains("Phone"))
                ShopAddressesDataGrid.Columns["Phone"].HeaderText = "Телефон";
            if (ShopAddressesDataGrid.Columns.Contains("WebSite"))
                ShopAddressesDataGrid.Columns["WebSite"].HeaderText = "Сайт";

            if (ShopAddressesDataGrid.Columns.Contains("Lat"))
                ShopAddressesDataGrid.Columns["Lat"].Visible = false;
            if (ShopAddressesDataGrid.Columns.Contains("Long"))
                ShopAddressesDataGrid.Columns["Long"].Visible = false;
            if (ShopAddressesDataGrid.Columns.Contains("City"))
                ShopAddressesDataGrid.Columns["City"].Visible = false;
            if (ShopAddressesDataGrid.Columns.Contains("ShopAddressID"))
                ShopAddressesDataGrid.Columns["ShopAddressID"].Visible = false;
            if (ShopAddressesDataGrid.Columns.Contains("ClientID"))
                    ShopAddressesDataGrid.Columns["ClientID"].Visible = false;

            foreach (DataGridViewColumn Column in ShopAddressesDataGrid.Columns)
            {
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            ShopAddressesDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.ForeColor = Color.Blue;
            ShopAddressesDataGrid.Columns["Address"].DefaultCellStyle.Font = new System.Drawing.Font("SEGOE UI", 15.0F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
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

        private void ShopAddressesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string Address = string.Empty;
            if (ShopAddressesDataGrid.SelectedRows.Count != 0 && ShopAddressesDataGrid.SelectedRows[0].Cells["Address"].Value != DBNull.Value)
                Address = ShopAddressesDataGrid.SelectedRows[0].Cells["Address"].Value.ToString();
            if (Address.Length == 0)
                return;
            try
            {
                StringBuilder QuerryAddress = new StringBuilder();
                QuerryAddress.Append("http://www.google.com/maps?q=");
                QuerryAddress.Append(Address);
                System.Diagnostics.Process.Start(QuerryAddress.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void ShopAddressesDataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid dataGridView = (PercentageDataGrid)sender;
            if (e.ColumnIndex > -1 && e.RowIndex > -1 && dataGridView.Columns[e.ColumnIndex].Name == "Address" && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }
    }
}
