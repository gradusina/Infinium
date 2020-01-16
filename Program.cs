using System;
using System.Windows.Forms;

namespace Infinium
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Screen.PrimaryScreen.Bounds.Width < 1270 || Screen.PrimaryScreen.Bounds.Height < 720)
            {
                MessageBox.Show("Разрешение экрана не соответствует минимальным требованием приложения (1270:720)");
                return;
            }

            //using (Graphics g = Graphics.FromHwnd(Process.GetCurrentProcess().MainWindowHandle))
            //{
            //    if(g.DpiX > 96)
            //        MessageBox.Show("Масштабирование экрана не соответствует требованием приложения (более 96 точек на дюйм)");
            //    return;
            //}


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Connection Connection = new Connection();
            //ConnectionStrings.UsersConnectionString = Connection.GetConnectionString("ConnectionUsers.config");
            //ConnectionStrings.CatalogConnectionString = Connection.GetConnectionString("ConnectionCatalog.config");
            //ConnectionStrings.ZOVOrdersConnectionString = Connection.GetConnectionString("ConnectionZOVOrders.config");
            //ConnectionStrings.ZOVReferenceConnectionString = Connection.GetConnectionString("ConnectionZOVReference.config");
            //ConnectionStrings.MarketingOrdersConnectionString = Connection.GetConnectionString("ConnectionMarketingOrders.config");
            //ConnectionStrings.MarketingReferenceConnectionString = Connection.GetConnectionString("ConnectionMarketingReference.config");
            //ConnectionStrings.LightConnectionString = Connection.GetConnectionString("ConnectionLight.config");
            //ConnectionStrings.StorageConnectionString = Connection.GetConnectionString("ConnectionStorage.config");

            //DatabaseConfigsManager DatabaseConfigsManager = new DatabaseConfigsManager();
            //DatabaseConfigsManager.ReadAnimationFlag("Animation.config");
            //Configs.DocumentsPath = DatabaseConfigsManager.ReadConfig("DocumentsPath.config");
            //Configs.DocumentsZOVTPSPath = DatabaseConfigsManager.ReadConfig("DocumentsZOVTPSPath.config");
            //Configs.DocumentsPathHost = DatabaseConfigsManager.ReadConfig("DocumentsPathHost.config");
            //Configs.FTPType = Convert.ToInt32(DatabaseConfigsManager.ReadConfig("FTP.config", 1, 0));

            //Application.Run(new HistoryScanPermitsForm());
            Application.Run(new LoginForm());
        }
    }
}
