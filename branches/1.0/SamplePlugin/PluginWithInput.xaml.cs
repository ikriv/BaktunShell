using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace SamplePlugin
{
    /// <summary>
    /// Interaction logic for PluginWithInput.xaml
    /// </summary>
    public partial class PluginWithInput : UserControl
    {
        public PluginWithInput()
        {
            InitializeComponent();
            BlockUiThreadButton.Click += new RoutedEventHandler(BlockUiThreadButton_Click);
            MessageBoxButton.Click += new RoutedEventHandler(MessageBoxButton_Click);
        }

        void BlockUiThreadButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Blocking UI Thread...");
            Thread.Sleep(5000);
            Console.WriteLine("Finished");
        }

        void MessageBoxButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aha!");
        }

    }
}
