using System;
using System.Windows.Forms;

namespace WinFormsTicks
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*
            AppDomain.CurrentDomain.UnhandledException += (source, ex) =>
                ObserveError("AppDomain.CurrentDomain.UnhandledException:\n\n" + ex.ExceptionObject.ToString());

            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
            System.Windows.Forms.Application.ThreadException += (source, ex) =>
                ObserveError("Application.ThreadException:\n\n" + ex.Exception.ToString());

            TaskScheduler.UnobservedTaskException += (source, ex) => 
                ObserveError("UnobservedTaskException:\n\n" + ex.Exception.InnerExceptions.Single().ToString());
            */

            try
            {
                Application.Run(new Form1());
            }
            catch (Exception exception)
            {
                ObserveError("Application.Run Exception:\n\n" + exception);
            }
        }
        private static void ObserveError(string str) => MessageBox.Show(str);
    }
}

