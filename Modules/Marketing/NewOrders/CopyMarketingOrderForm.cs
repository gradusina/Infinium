using Infinium.Modules.Marketing.NewOrders;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CopyMarketingOrderForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public int ClientID = 1;
        public int MegaOrderID = 1;
        public int MainOrderID = 1;
        int FormEvent = 0;

        bool MoveMegaOrder = true;
        Form MainForm = null;

        CopyMarketingOrders CopyMarketingOrders;

        public CopyMarketingOrderForm(Form tMainForm, bool bMoveMegaOrder, int iClientID, int iMegaOrderID, int iMainOrderID)
        {
            MainForm = tMainForm;
            MoveMegaOrder = bMoveMegaOrder;
            ClientID = iClientID;
            MegaOrderID = iMegaOrderID;
            MainOrderID = iMainOrderID;
            InitializeComponent();

            if (!bMoveMegaOrder)
                panel4.Enabled = true;
            Initialize();
        }

        private void NewOrderButton_Click(object sender, EventArgs e)
        {
            if (rbOtherClient.Checked)
                ClientID = Convert.ToInt32(cbClients.SelectedValue);

            if (rbNewOrder.Checked)
            {
                if (rbCopy.Checked)
                {
                    if (!MoveMegaOrder)
                        CopyMarketingOrders.CopyMainOrderToNew(ClientID, MainOrderID);
                    else
                        CopyMarketingOrders.CopyMegaOrderToNew(ClientID, MegaOrderID);
                }
                if (rbMove.Checked)
                {
                    if (!MoveMegaOrder)
                        CopyMarketingOrders.MoveMainOrderToNew(ClientID, MainOrderID);
                    else
                        CopyMarketingOrders.MoveMegaOrderToNew(ClientID, MegaOrderID);
                }
            }
            if (rbOtherOrder.Checked)
            {
                int iMegaOrderID = 0;
                if (cbOrders.SelectedItem != null && ((DataRowView)cbOrders.SelectedItem).Row["MegaOrderID"] != DBNull.Value)
                    iMegaOrderID = Convert.ToInt32(((DataRowView)cbOrders.SelectedItem).Row["MegaOrderID"]);
                if (rbCopy.Checked)
                    CopyMarketingOrders.CopyMainOrderToExist(ClientID, iMegaOrderID, MainOrderID);
                if (rbMove.Checked)
                    CopyMarketingOrders.MoveMainOrderToExist(ClientID, iMegaOrderID, MainOrderID);
            }

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelOrderButton_Click(object sender, EventArgs e)
        {
            ClientID = 0;
            MegaOrderID = 0;
            MainOrderID = 0;
            FormEvent = eClose;
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

        private void Initialize()
        {
            CopyMarketingOrders = new CopyMarketingOrders();

            cbClients.DataSource = CopyMarketingOrders.ClientsBS;
            cbClients.DisplayMember = "ClientName";
            cbClients.ValueMember = "ClientID";

            cbOrders.DataSource = CopyMarketingOrders.OrdersBS;
            cbOrders.DisplayMember = "OrderNumber";
            cbOrders.ValueMember = "MegaOrderID";

            if (ClientID > 0)
                cbClients.SelectedValue = ClientID;

            CopyMarketingOrders.GetOrdersDT(ClientID);
            if (CopyMarketingOrders.GetOrdersDT(ClientID))
                cbOrders.SelectedValue = MegaOrderID;
        }

        private void rbThisClient_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbThisClient.Checked)
                return;

            cbClients.Enabled = false;
        }

        private void rbOtherClient_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbOtherClient.Checked)
                return;

            cbClients.Enabled = true;
        }

        private void rbNewOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbNewOrder.Checked)
                return;

            cbOrders.Enabled = false;
        }

        private void rbOtherOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbOtherOrder.Checked)
                return;

            cbOrders.Enabled = true;
        }

        private void cbClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CopyMarketingOrders == null)
                return;
            int iClientID = 0;
            if (cbClients.SelectedItem != null && ((DataRowView)cbClients.SelectedItem).Row["ClientID"] != DBNull.Value)
                iClientID = Convert.ToInt32(((DataRowView)cbClients.SelectedItem).Row["ClientID"]);
            CopyMarketingOrders.GetOrdersDT(iClientID);
        }
    }
}
