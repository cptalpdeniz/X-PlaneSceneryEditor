using System;
using System.Threading;
using System.Windows.Forms;
using XPlane_Scenery_Editor.Properties;

namespace XPlane_Scenery_Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static public bool closefromInternal = false;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Settings.Default.FirstRun)
            {
                Welcome.showSplashScreen();
                Thread.Sleep(3500);
                Welcome.ShowFirstRunBox();
                Welcome.closeForm();
                
            }
            if (closefromInternal)
            {
                //Application.Exit();
                Application.ExitThread();
            }
            else
            {
                Application.Run(new Forms());
            }
        }
    }
}

