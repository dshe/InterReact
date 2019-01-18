using System;

namespace WinFormsTicks
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

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
                System.Windows.Forms.Application.Run(new Form1());
            }
            catch (Exception exception)
            {
                ObserveError("Application.Run Exception:\n\n" + exception);
            }
        }

        private static void ObserveError(string str) => System.Windows.Forms.MessageBox.Show(str);
    }
}
