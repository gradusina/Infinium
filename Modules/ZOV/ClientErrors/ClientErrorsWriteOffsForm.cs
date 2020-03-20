using Infinium.Modules.ZOV.ClientErrors;

using System;
using System.Globalization;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientErrorsWriteOffsForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm;

        ClientErrorsWriteOffs ClientErrorsWriteOffs;
        AssemblyOrders AssemblyOrders;
        NotPaidOrders NotPaidOrders;

        public ClientErrorsWriteOffsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        private void ClientErrorsWriteOffsForm_Shown(object sender, EventArgs e)
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

        private void Initialize()
        {
            ClientErrorsWriteOffs = new ClientErrorsWriteOffs();
            ClientErrorsWriteOffs.Initialize();
            ClientErrorsGridSetting();

            AssemblyOrders = new AssemblyOrders();
            AssemblyOrders.Initialize();
            AssemblyOrdersGridSetting();

            NotPaidOrders = new NotPaidOrders();
            NotPaidOrders.Initialize();
            NotPaidOrdersGridSetting();

            SearchPartDocNumberComboBox.DataSource = ClientErrorsWriteOffs.SearchPartDocNumberList;
            SearchPartDocNumberComboBox.DisplayMember = "DocNumber";
            SearchPartDocNumberComboBox.ValueMember = "MainOrderID";

            decimal ErrorsSumCost = 0;
            for (int i = 0; i < dgvClientErrors.Rows.Count; i++)
            {
                if (dgvClientErrors.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvClientErrors.Rows[i].Cells["Cost"].Value);
            }
            label2.Text = "Общая сумма: " + ErrorsSumCost + " €";

            ErrorsSumCost = 0;
            for (int i = 0; i < dgvAssemblyOrders.Rows.Count; i++)
            {
                if (dgvAssemblyOrders.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvAssemblyOrders.Rows[i].Cells["Cost"].Value);
            }
            label3.Text = "Общая сумма: " + ErrorsSumCost + " €";

            for (int i = 0; i < dgvNotPaidOrders.Rows.Count; i++)
            {
                if (dgvNotPaidOrders.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvNotPaidOrders.Rows[i].Cells["Cost"].Value);
            }
            label4.Text = "Общая сумма: " + ErrorsSumCost + " €";
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

        private void ClientErrorsGridSetting()
        {
            dgvClientErrors.DataSource = ClientErrorsWriteOffs.ClientErrorsList;
            dgvClientErrors.Columns.Add(ClientErrorsWriteOffs.ClientColumn);
            dgvClientErrors.Columns.Add(ClientErrorsWriteOffs.DateTimeColumn);

            if (dgvClientErrors.Columns.Contains("MainOrderID"))
                dgvClientErrors.Columns["MainOrderID"].Visible = false;
            if (dgvClientErrors.Columns.Contains("ClientErrorsWriteOffID"))
                dgvClientErrors.Columns["ClientErrorsWriteOffID"].Visible = false;
            if (dgvClientErrors.Columns.Contains("ClientID"))
                dgvClientErrors.Columns["ClientID"].Visible = false;
            if (dgvClientErrors.Columns.Contains("Created"))
                dgvClientErrors.Columns["Created"].Visible = false;
            if (dgvClientErrors.Columns.Contains("OrderDate"))
                dgvClientErrors.Columns["OrderDate"].Visible = false;

            dgvClientErrors.Columns["DateTimeColumn"].DefaultCellStyle.Format = "dd.MM.yyyy";

            dgvClientErrors.Columns["Product"].HeaderText = "Продукт";
            dgvClientErrors.Columns["DateTimeColumn"].HeaderText = "Дата";
            dgvClientErrors.Columns["DocNumber"].HeaderText = "№ кухни";
            dgvClientErrors.Columns["Cost"].HeaderText = "Сумма";
            dgvClientErrors.Columns["Reason"].HeaderText = "Причина";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 1
            };
            dgvClientErrors.Columns["Cost"].DefaultCellStyle.Format = "C";
            dgvClientErrors.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            dgvClientErrors.Columns["ClientColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientErrors.Columns["ClientColumn"].MinimumWidth = 250;
            dgvClientErrors.Columns["DateTimeColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientErrors.Columns["DateTimeColumn"].MinimumWidth = 250;
            dgvClientErrors.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientErrors.Columns["DocNumber"].MinimumWidth = 190;
            dgvClientErrors.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientErrors.Columns["Cost"].MinimumWidth = 190;
            dgvClientErrors.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvClientErrors.Columns["Reason"].MinimumWidth = 160;
            dgvClientErrors.Columns["Product"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvClientErrors.Columns["Product"].MinimumWidth = 250;

            dgvClientErrors.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvClientErrors.Columns["ClientColumn"].DisplayIndex = DisplayIndex++;
            dgvClientErrors.Columns["DocNumber"].DisplayIndex = DisplayIndex++;
            dgvClientErrors.Columns["DateTimeColumn"].DisplayIndex = DisplayIndex++;
            dgvClientErrors.Columns["Product"].DisplayIndex = DisplayIndex++;
            dgvClientErrors.Columns["Cost"].DisplayIndex = DisplayIndex++;
            dgvClientErrors.Columns["Reason"].DisplayIndex = DisplayIndex++;

            dgvClientErrors.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void AssemblyOrdersGridSetting()
        {
            dgvAssemblyOrders.DataSource = AssemblyOrders.AssemblyOrdersList;
            dgvAssemblyOrders.Columns.Add(AssemblyOrders.ClientColumn);
            dgvAssemblyOrders.Columns.Add(AssemblyOrders.DispatchDateColumn);
            dgvAssemblyOrders.Columns.Add(AssemblyOrders.PaymentDateColumn);

            if (dgvAssemblyOrders.Columns.Contains("AssemblyOrderID"))
                dgvAssemblyOrders.Columns["AssemblyOrderID"].Visible = false;
            if (dgvAssemblyOrders.Columns.Contains("ClientID"))
                dgvAssemblyOrders.Columns["ClientID"].Visible = false;
            if (dgvAssemblyOrders.Columns.Contains("Created"))
                dgvAssemblyOrders.Columns["Created"].Visible = false;
            if (dgvAssemblyOrders.Columns.Contains("DispatchDate"))
                dgvAssemblyOrders.Columns["DispatchDate"].Visible = false;
            if (dgvAssemblyOrders.Columns.Contains("PaymentDate"))
                dgvAssemblyOrders.Columns["PaymentDate"].Visible = false;
            if (dgvAssemblyOrders.Columns.Contains("MainOrderID"))
                dgvAssemblyOrders.Columns["MainOrderID"].Visible = false;
            if (dgvAssemblyOrders.Columns.Contains("Active"))
                dgvAssemblyOrders.Columns["Active"].Visible = false;

            dgvAssemblyOrders.Columns["DocNumber"].HeaderText = "№ кухни";
            dgvAssemblyOrders.Columns["AssemblyNumber"].HeaderText = "№ сборки";
            dgvAssemblyOrders.Columns["Cost"].HeaderText = "Сумма";
            dgvAssemblyOrders.Columns["Notes"].HeaderText = "Примечание";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 1
            };
            dgvAssemblyOrders.Columns["Cost"].DefaultCellStyle.Format = "C";
            dgvAssemblyOrders.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            dgvAssemblyOrders.Columns["ClientColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvAssemblyOrders.Columns["ClientColumn"].MinimumWidth = 250;
            dgvAssemblyOrders.Columns["DispatchDateColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvAssemblyOrders.Columns["DispatchDateColumn"].MinimumWidth = 150;
            dgvAssemblyOrders.Columns["PaymentDateColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvAssemblyOrders.Columns["PaymentDateColumn"].MinimumWidth = 150;
            dgvAssemblyOrders.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvAssemblyOrders.Columns["DocNumber"].MinimumWidth = 90;
            dgvAssemblyOrders.Columns["AssemblyNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvAssemblyOrders.Columns["AssemblyNumber"].MinimumWidth = 90;
            dgvAssemblyOrders.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvAssemblyOrders.Columns["Cost"].MinimumWidth = 190;
            dgvAssemblyOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvAssemblyOrders.Columns["Notes"].MinimumWidth = 160;

            dgvAssemblyOrders.Columns["ClientColumn"].ReadOnly = true;
            dgvAssemblyOrders.Columns["DispatchDateColumn"].ReadOnly = true;
            dgvAssemblyOrders.Columns["DocNumber"].ReadOnly = true;

            dgvAssemblyOrders.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvAssemblyOrders.Columns["ClientColumn"].DisplayIndex = DisplayIndex++;
            dgvAssemblyOrders.Columns["DocNumber"].DisplayIndex = DisplayIndex++;
            dgvAssemblyOrders.Columns["DispatchDateColumn"].DisplayIndex = DisplayIndex++;
            dgvAssemblyOrders.Columns["AssemblyNumber"].DisplayIndex = DisplayIndex++;
            dgvAssemblyOrders.Columns["PaymentDateColumn"].DisplayIndex = DisplayIndex++;
            dgvAssemblyOrders.Columns["Cost"].DisplayIndex = DisplayIndex++;
            dgvAssemblyOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;

            dgvAssemblyOrders.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void NotPaidOrdersGridSetting()
        {
            dgvNotPaidOrders.DataSource = NotPaidOrders.NotPaidOrdersList;
            dgvNotPaidOrders.Columns.Add(NotPaidOrders.ClientColumn);
            dgvNotPaidOrders.Columns.Add(NotPaidOrders.DispatchDateColumn);
            dgvNotPaidOrders.Columns.Add(NotPaidOrders.PaymentDateColumn);

            if (dgvNotPaidOrders.Columns.Contains("NotPaidOrderID"))
                dgvNotPaidOrders.Columns["NotPaidOrderID"].Visible = false;
            if (dgvNotPaidOrders.Columns.Contains("ClientID"))
                dgvNotPaidOrders.Columns["ClientID"].Visible = false;
            if (dgvNotPaidOrders.Columns.Contains("Created"))
                dgvNotPaidOrders.Columns["Created"].Visible = false;
            if (dgvNotPaidOrders.Columns.Contains("DispatchDate"))
                dgvNotPaidOrders.Columns["DispatchDate"].Visible = false;
            if (dgvNotPaidOrders.Columns.Contains("PaymentDate"))
                dgvNotPaidOrders.Columns["PaymentDate"].Visible = false;
            if (dgvNotPaidOrders.Columns.Contains("MainOrderID"))
                dgvNotPaidOrders.Columns["MainOrderID"].Visible = false;
            if (dgvNotPaidOrders.Columns.Contains("Active"))
                dgvNotPaidOrders.Columns["Active"].Visible = false;

            dgvNotPaidOrders.Columns["DocNumber"].HeaderText = "№ кухни";
            dgvNotPaidOrders.Columns["Cost"].HeaderText = "Сумма";
            dgvNotPaidOrders.Columns["Notes"].HeaderText = "Примечание";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 1
            };
            dgvNotPaidOrders.Columns["Cost"].DefaultCellStyle.Format = "C";
            dgvNotPaidOrders.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            dgvNotPaidOrders.Columns["ClientColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvNotPaidOrders.Columns["ClientColumn"].MinimumWidth = 250;
            dgvNotPaidOrders.Columns["DispatchDateColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvNotPaidOrders.Columns["DispatchDateColumn"].MinimumWidth = 150;
            dgvNotPaidOrders.Columns["PaymentDateColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvNotPaidOrders.Columns["PaymentDateColumn"].MinimumWidth = 150;
            dgvNotPaidOrders.Columns["DocNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvNotPaidOrders.Columns["DocNumber"].MinimumWidth = 90;
            dgvNotPaidOrders.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvNotPaidOrders.Columns["Cost"].MinimumWidth = 190;
            dgvNotPaidOrders.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvNotPaidOrders.Columns["Notes"].MinimumWidth = 160;

            dgvNotPaidOrders.Columns["ClientColumn"].ReadOnly = true;
            dgvNotPaidOrders.Columns["DispatchDateColumn"].ReadOnly = true;
            dgvNotPaidOrders.Columns["DocNumber"].ReadOnly = true;

            dgvNotPaidOrders.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvNotPaidOrders.Columns["ClientColumn"].DisplayIndex = DisplayIndex++;
            dgvNotPaidOrders.Columns["DocNumber"].DisplayIndex = DisplayIndex++;
            dgvNotPaidOrders.Columns["DispatchDateColumn"].DisplayIndex = DisplayIndex++;
            dgvNotPaidOrders.Columns["PaymentDateColumn"].DisplayIndex = DisplayIndex++;
            dgvNotPaidOrders.Columns["Cost"].DisplayIndex = DisplayIndex++;
            dgvNotPaidOrders.Columns["Notes"].DisplayIndex = DisplayIndex++;

            dgvNotPaidOrders.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void btnAddError_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewClientErrorForm AddNewClientErrorForm = new NewClientErrorForm(this, ref ClientErrorsWriteOffs);

            TopForm = AddNewClientErrorForm;
            AddNewClientErrorForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            AddNewClientErrorForm.Dispose();
            TopForm = null;
        }

        private void btnAddAssemblyOrder_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewAssemblyOrderForm NewAssemblyOrderForm = new NewAssemblyOrderForm(this, ref AssemblyOrders);

            TopForm = NewAssemblyOrderForm;
            NewAssemblyOrderForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewAssemblyOrderForm.Dispose();
            TopForm = null;
        }

        private void btnSaveError_Click(object sender, EventArgs e)
        {
            int ClientErrorsWriteOffID = 0;
            if (dgvClientErrors.SelectedRows.Count != 0 && dgvClientErrors.SelectedRows[0].Cells["ClientErrorsWriteOffID"].Value != DBNull.Value)
                ClientErrorsWriteOffID = Convert.ToInt32(dgvClientErrors.SelectedRows[0].Cells["ClientErrorsWriteOffID"].Value);
            bool bShowAllErrors = cbShowAllErrors.Checked;
            object DateFrom = CalendarFrom.SelectionEnd;
            object DateTo = CalendarTo.SelectionEnd;
            ClientErrorsWriteOffs.SaveClientErrors();
            ClientErrorsWriteOffs.UpdateClientErrors(bShowAllErrors, DateFrom, DateTo);
            ClientErrorsWriteOffs.MoveToClientError(ClientErrorsWriteOffID);
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);

            decimal ErrorsSumCost = 0;
            for (int i = 0; i < dgvClientErrors.Rows.Count; i++)
            {
                if (dgvClientErrors.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvClientErrors.Rows[i].Cells["Cost"].Value);
            }
            label2.Text = "Общая сумма: " + ErrorsSumCost + " €";
        }

        private void btnSaveAssemblyOrders_Click(object sender, EventArgs e)
        {
            int AssemblyOrderID = 0;
            if (dgvAssemblyOrders.SelectedRows.Count != 0 && dgvAssemblyOrders.SelectedRows[0].Cells["AssemblyOrderID"].Value != DBNull.Value)
                AssemblyOrderID = Convert.ToInt32(dgvAssemblyOrders.SelectedRows[0].Cells["AssemblyOrderID"].Value);
            AssemblyOrders.SaveAssemblyOrders();
            AssemblyOrders.UpdateAssemblyOrders();
            AssemblyOrders.MoveToAssemblyOrder(AssemblyOrderID);
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);

            decimal ErrorsSumCost = 0;
            for (int i = 0; i < dgvAssemblyOrders.Rows.Count; i++)
            {
                if (dgvAssemblyOrders.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvAssemblyOrders.Rows[i].Cells["Cost"].Value);
            }
            label3.Text = "Общая сумма: " + ErrorsSumCost + " €";
        }

        private void btnDeleteError_Click(object sender, EventArgs e)
        {
            ClientErrorsWriteOffs.DeleteClientError();
        }

        private void btnDeleteAssemblyOrder_Click(object sender, EventArgs e)
        {
            AssemblyOrders.DeleteAssemblyOrder();
        }

        private void gridClientErrors_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewRow r in dgvClientErrors.SelectedRows)
                {
                    if (!r.IsNewRow)
                    {
                        dgvClientErrors.Rows.RemoveAt(r.Index);
                    }
                }
            }
        }

        private void gridAssemblyOrders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewRow r in dgvAssemblyOrders.SelectedRows)
                {
                    if (!r.IsNewRow)
                    {
                        dgvAssemblyOrders.Rows.RemoveAt(r.Index);
                    }
                }
            }
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (ClientErrorsWriteOffs == null)
                return;

            if (kryptonCheckSet1.CheckedButton == cbtnErrors)
            {
                ErrorsToolsPanel.BringToFront();
                ClientErrorsPanel.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnAssembly)
            {
                AssemblyToolsPanel.BringToFront();
                AssemblyOrdersPanel.BringToFront();
            }
            if (kryptonCheckSet1.CheckedButton == cbtnNotPaidOrders)
            {
                NotPaidToolsPanel.BringToFront();
                NotPaidOrdersPanel.BringToFront();
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && SearchTextBox.Text.Length > 0)
            {
                ClientErrorsWriteOffs.SearchPartDocNumber(SearchTextBox.Text);
                ClientErrorsWriteOffs.SearchDocNumber(Convert.ToInt32(SearchPartDocNumberComboBox.SelectedValue));
            }
        }

        private void SearchPartDocNumberComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (SearchPartDocNumberComboBox.Items.Count > 0)
                ClientErrorsWriteOffs.SearchDocNumber(Convert.ToInt32(SearchPartDocNumberComboBox.SelectedValue));
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            bool bShowAllErrors = cbShowAllErrors.Checked;
            object DateFrom = CalendarFrom.SelectionEnd;
            object DateTo = CalendarTo.SelectionEnd;
            decimal ErrorsSumCost = 0;
            ClientErrorsWriteOffs.UpdateClientErrors(bShowAllErrors, DateFrom, DateTo);
            for (int i = 0; i < dgvClientErrors.Rows.Count; i++)
            {
                if (dgvClientErrors.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvClientErrors.Rows[i].Cells["Cost"].Value);
            }
            label2.Text = "Общая сумма: " + ErrorsSumCost + " €";
        }

        private void btnAddNotPaidOrder_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewNotPaidOrderForm NewNotPaidOrderForm = new NewNotPaidOrderForm(this, ref NotPaidOrders);

            TopForm = NewNotPaidOrderForm;
            NewNotPaidOrderForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewNotPaidOrderForm.Dispose();
            TopForm = null;
        }

        private void btnSaveNotPaidOrders_Click(object sender, EventArgs e)
        {
            int NotPaidOrderID = 0;
            if (dgvNotPaidOrders.SelectedRows.Count != 0 && dgvNotPaidOrders.SelectedRows[0].Cells["NotPaidOrderID"].Value != DBNull.Value)
                NotPaidOrderID = Convert.ToInt32(dgvNotPaidOrders.SelectedRows[0].Cells["NotPaidOrderID"].Value);
            NotPaidOrders.SaveNotPaidOrders();
            NotPaidOrders.UpdateNotPaidOrders();
            NotPaidOrders.MoveToNotPaidOrder(NotPaidOrderID);
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);

            decimal ErrorsSumCost = 0;
            for (int i = 0; i < dgvNotPaidOrders.Rows.Count; i++)
            {
                if (dgvNotPaidOrders.Rows[i].Cells["Cost"].Value != DBNull.Value)
                    ErrorsSumCost += Convert.ToDecimal(dgvNotPaidOrders.Rows[i].Cells["Cost"].Value);
            }
            label4.Text = "Общая сумма: " + ErrorsSumCost + " €";
        }
    }
}
