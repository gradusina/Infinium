using Infinium.Modules.ZOV.Samples;

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVShopAddressesForm : Form
    {
        private const int eClose = 3;
        private const int eHide = 2;
        private const int eMainMenu = 4;
        private const int eShow = 1;
        private int FormEvent = 0;
        private Form MainForm = null;
        private ZovSampleShops shopsManager;

        public ZOVShopAddressesForm(Form tMainForm, int iClientID, int iMainOrderID)
        {
            MainForm = tMainForm;
            InitializeComponent();

            shopsManager = new ZovSampleShops();
            shopsManager.ClientId = iClientID;
            shopsManager.MainOrderId = iMainOrderID;
            shopsManager.Fill();
            orderShopsDataGrid.DataSource = shopsManager.OrderShopsBindingSource;
            allShopsDataGrid.DataSource = shopsManager.AllShopsBindingSource;
            shopsManager.UpdatesTables();
            gridSettings(orderShopsDataGrid, false);
            gridSettings(allShopsDataGrid, true);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            shopsManager.SaveShopAddressOrders();
            shopsManager.SaveShopAddresses();
            shopsManager.UpdatesTables();
        }

        private void gridSettings(PercentageDataGrid percentageDataGrid, bool bAll)
        {
            if (percentageDataGrid.Columns.Contains("Address"))
                percentageDataGrid.Columns["Address"].HeaderText = "Адрес";
            if (percentageDataGrid.Columns.Contains("City"))
                percentageDataGrid.Columns["City"].HeaderText = "Город";
            if (percentageDataGrid.Columns.Contains("Country"))
                percentageDataGrid.Columns["Country"].HeaderText = "Страна";
            if (percentageDataGrid.Columns.Contains("Lat"))
                percentageDataGrid.Columns["Lat"].HeaderText = "Широта";
            if (percentageDataGrid.Columns.Contains("Long"))
                percentageDataGrid.Columns["Long"].HeaderText = "Долгота";
            if (percentageDataGrid.Columns.Contains("Worktime"))
                percentageDataGrid.Columns["Worktime"].HeaderText = "Время работы";
            if (percentageDataGrid.Columns.Contains("Name"))
                percentageDataGrid.Columns["Name"].HeaderText = "Название";
            if (percentageDataGrid.Columns.Contains("Site"))
                percentageDataGrid.Columns["Site"].HeaderText = "Сайт";
            if (percentageDataGrid.Columns.Contains("IsFronts"))
                percentageDataGrid.Columns["IsFronts"].HeaderText = "Фасады";
            if (percentageDataGrid.Columns.Contains("IsProfile"))
                percentageDataGrid.Columns["IsProfile"].HeaderText = "Погонаж";
            if (percentageDataGrid.Columns.Contains("IsFurniture"))
                percentageDataGrid.Columns["IsFurniture"].HeaderText = "Мебель";
            if (percentageDataGrid.Columns.Contains("Email"))
                percentageDataGrid.Columns["Email"].HeaderText = "Email";
            if (percentageDataGrid.Columns.Contains("Phone"))
                percentageDataGrid.Columns["Phone"].HeaderText = "Телефон";
            if (percentageDataGrid.Columns.Contains("WebSite"))
                percentageDataGrid.Columns["WebSite"].HeaderText = "Сайт";

            if (percentageDataGrid.Columns.Contains("ShopAddressOrderID"))
                percentageDataGrid.Columns["ShopAddressOrderID"].Visible = false;
            if (percentageDataGrid.Columns.Contains("ShopAddressID1"))
                percentageDataGrid.Columns["ShopAddressID1"].Visible = false;
            if (percentageDataGrid.Columns.Contains("MainOrderID"))
                percentageDataGrid.Columns["MainOrderID"].Visible = false;
            if (percentageDataGrid.Columns.Contains("ShopAddressID"))
                percentageDataGrid.Columns["ShopAddressID"].Visible = false;
            if (percentageDataGrid.Columns.Contains("ClientID"))
                percentageDataGrid.Columns["ClientID"].Visible = false;

            percentageDataGrid.Columns["Check"].DisplayIndex = 0;

            foreach (DataGridViewColumn Column in percentageDataGrid.Columns)
            {
                if (bAll)
                    Column.ReadOnly = false;
                else
                {
                    Column.ReadOnly = true;
                    if (Column.Name.Equals("Check"))
                        Column.ReadOnly = false;
                }

                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            percentageDataGrid.AllowUserToAddRows = false;
            if (bAll)
                percentageDataGrid.AllowUserToAddRows = true;
            percentageDataGrid.Columns["Address"].DefaultCellStyle.ForeColor = Color.Blue;
            percentageDataGrid.Columns["Address"].DefaultCellStyle.Font = new System.Drawing.Font("SEGOE UI", 13.0F,
                System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Pixel, 0);
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ShopAddressesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid dataGridView = (PercentageDataGrid) sender;
            if (e.ColumnIndex > -1 && e.RowIndex > -1 && dataGridView.Columns[e.ColumnIndex].Name == "Address"
                && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null &&
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
            {
                string Address = string.Empty;
                if (allShopsDataGrid.SelectedRows.Count != 0 &&
                    allShopsDataGrid.SelectedRows[0].Cells["Address"].Value != DBNull.Value)
                    Address = allShopsDataGrid.SelectedRows[0].Cells["Address"].Value.ToString();
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
        }

        private void ShopAddressesDataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid dataGridView = (PercentageDataGrid) sender;
            if (e.ColumnIndex > -1 && e.RowIndex > -1 && dataGridView.Columns[e.ColumnIndex].Name == "Address"
                && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null &&
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }

        private void ShopAddressesDataGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["ClientID"].Value = shopsManager.ClientId;
            //e.Row.Cells["Check"].Value = false;
        }

        private void allShopsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (allShopsDataGrid.Columns[e.ColumnIndex].Name == "Check" && e.RowIndex != -1)
            {
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell) allShopsDataGrid.Rows[e.RowIndex].Cells["Check"];

                int shopAddressId = -1;
                if (allShopsDataGrid.Rows[e.RowIndex].Cells["ShopAddressID"].Value != DBNull.Value)
                    shopAddressId = Convert.ToInt32(allShopsDataGrid.Rows[e.RowIndex].Cells["ShopAddressID"].Value);


                bool check = Convert.ToBoolean(checkCell.Value);
                if (check && shopAddressId != -1)
                    shopsManager.AddNewShopAddressOrder(shopAddressId);
                shopsManager.SaveShopAddressOrders();
                shopsManager.SaveShopAddresses();
                shopsManager.UpdatesTables();
            }
        }

        private void orderShopsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (orderShopsDataGrid.Columns[e.ColumnIndex].Name == "Check" && e.RowIndex != -1)
            {
                shopsManager.SaveShopAddressOrders();
                shopsManager.SaveShopAddresses();
                shopsManager.UpdatesTables();
            }
        }
    }
}