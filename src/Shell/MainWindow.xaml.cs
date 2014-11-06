using System;
using System.Windows;

namespace Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _viewModel = new MainViewModel();
            this.Closed += new EventHandler(MainWindow_Closed);
            Title = String.Format("Baktun Shell {0} bit", IntPtr.Size * 8);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _viewModel.OnWindowClosed();
        }

        private void TabClose_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null) return;
            var plugin = element.DataContext as Plugin;
            if (plugin == null) return;
            _viewModel.CloseTab(plugin);
        }
    }
}
