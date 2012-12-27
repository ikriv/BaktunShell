using System;
using System.Windows;
using System.Windows.Threading;

namespace Smith.WPF.HtmlEditor.Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer wordCountTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitEvents();
            InitTimer();
        }

        void InitEvents()
        {
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
            Editor.DocumentReady += new RoutedEventHandler(Editor_DocumentReady);
            GetHtmlButton.Click += new RoutedEventHandler(GetHtmlButton_Click);
            GetTextButton.Click += new RoutedEventHandler(GetTextButton_Click);
            BindingTestButton.Click += new RoutedEventHandler(BindingTestButton_Click);            
        }

        void BindingTestButton_Click(object sender, RoutedEventArgs e)
        {
            BindingTestWindow w = new BindingTestWindow();
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = this;
            w.ShowDialog();
        }

        void Editor_DocumentReady(object sender, RoutedEventArgs e)
        {
            // Always load initial content for editor in DocumentReady event
            Editor.ContentHtml = "<p><b>Smith Html Editor</b></p><p><a href=\"http://smithhtmleditor.codeplex.com\">http://smithhtmleditor.codeplex.com/</a></p>";
        }

        void GetTextButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Editor.ContentText);
        }

        void GetHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Editor.ContentHtml);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            wordCountTimer.Start();            
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            wordCountTimer.Stop();
        }

        void InitTimer()
        {
            wordCountTimer = new DispatcherTimer();
            wordCountTimer.Interval = TimeSpan.FromMilliseconds(500);
            wordCountTimer.Tick += new EventHandler(wordCountTimer_Tick);
        }

        void wordCountTimer_Tick(object sender, EventArgs e)
        {
            WordCountText.Content = Editor.WordCount;
        }
    }
}
