using System.ComponentModel;
using System.Windows;

namespace WpfDepth
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel mainViewModel = new MainViewModel();

        public MainWindow()
        {
            DataContext = mainViewModel;
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
            => await mainViewModel.Initialize();

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
            => await mainViewModel.DisposeAsync();

    }
}
