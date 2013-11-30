using System.Windows;
using System.Windows.Controls;

namespace SolarSystem
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        OrbitsCalculator _data = new OrbitsCalculator();
        public MainUserControl()
        {
            DataContext = _data;
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _data.StartTimer();
        }

        private void Pause_Checked(object sender, RoutedEventArgs e)
        {
            _data.Pause(true);
        }

        private void Pause_Unchecked(object sender, RoutedEventArgs e)
        {
            _data.Pause(false);
        }
    }
}
