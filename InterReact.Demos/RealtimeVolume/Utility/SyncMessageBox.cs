using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

#nullable enable

namespace RealtimeVolume.Utility
{
    public static class SyncMessageBox
    {
        public static async Task Show(string message, bool terminate = false)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (dispatcher.HasThreadAccess)
                await ShowInternal(message);
            else
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await ShowInternal(message));
            if (terminate)
                Application.Current.Exit();
        }

        private static async Task ShowInternal(string message)
            => await new MessageDialog(message).ShowAsync();

    }
}
