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

            Application.Run(new LoginForm());
        }
    }
}
