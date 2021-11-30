using System;
using System.Windows;
using System.Windows.Threading;

namespace TicksWpf;

public static class Utilities
{
    public static void MessageBoxShow(string message, string caption, bool terminate = false)
    {
        RunOnDispatcher(() =>
        {
            MessageBox.Show(message, caption);
            if (terminate)
                    // Application.Current.Shutdown() may only be called from the thread that created the Application object.
                    Application.Current.Shutdown();
        });
    }

    public static void RunOnDispatcher(Action action)
    {
        Dispatcher dispatcher = Application.Current.Dispatcher;
        if (dispatcher.CheckAccess())
            action();
        else
            dispatcher.Invoke(action);
    }

}
