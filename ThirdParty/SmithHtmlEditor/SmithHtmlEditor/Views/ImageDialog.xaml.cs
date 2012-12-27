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
using System.Windows.Shapes;
using System.Net;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace Smith.WPF.HtmlEditor
{
    /// <summary>
    /// ImageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDialog : Window
    {
        ImageObject bindingContext;

        public ImageDialog()
        {
            InitializeComponent();
            InitAlignmentItems();
            InitBindingContext();
            InitEvents();
        }

        public ImageObject Model
        {
            get { return bindingContext; }
            private set
            {
                bindingContext = value;
                DataContext = bindingContext;
            }
        }

        void InitBindingContext()
        {
            Model = new ImageObject
            {
                ImageUrl = "http://",
                Alignment = ImageAlignment.Default
            };
        }

        void InitAlignmentItems()
        {
            List<ImageAlignment> ls = new List<ImageAlignment>();
            ls.Add(ImageAlignment.Default);
            ls.Add(ImageAlignment.Left);
            ls.Add(ImageAlignment.Right);
            ls.Add(ImageAlignment.Top);
            ls.Add(ImageAlignment.Center);
            ls.Add(ImageAlignment.Bottom);
            ImageAlignmentSelection.ItemsSource = new ReadOnlyCollection<ImageAlignment>(ls);
            ImageAlignmentSelection.DisplayMemberPath = "Text";
        } 

        void InitEvents()
        {
            RefreshButton.Click += new RoutedEventHandler(RefreshButton_Click);
            BrowseButton.Click += new RoutedEventHandler(BrowseButton_Click);
            ResizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(ResizeSlider_ValueChanged);
            ZoomInButton.Click += new RoutedEventHandler(ZoomInButton_Click);
            ZoomOutButton.Click += new RoutedEventHandler(ZoomOutButton_Click);
            OkayButton.Click += new RoutedEventHandler(OkayButton_Click);
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);

            ScrollViewContentDragable.SetEnable(PreviewScroll, true);
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal) this.DialogResult = true;
            this.Close();
        }

        void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            double val = ResizeSlider.Value - 10;
            if (val < 0) val = 1;
            ResizeSlider.Value = val;
        }

        void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            double val = ResizeSlider.Value + 10;
            if (val > 200) val = 200;
            ResizeSlider.Value = val;
        }

        void ResizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double p = e.NewValue / 100;
            bindingContext.Width = Convert.ToInt32(Math.Round(p * bindingContext.OriginalWidth));
            bindingContext.Height = Convert.ToInt32(Math.Round(p * bindingContext.OriginalHeight));
        }

        void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dialog =
                 new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "所有格式|*.jpg;*.jpeg;*.png;*gif|JPEG|*.jpg;*.jpeg|PNG|*.png|GIF|*.gif";
                dialog.FilterIndex = 0;
                if (System.Windows.Forms.DialogResult.OK == dialog.ShowDialog())
                {
                    UrlText.Text = dialog.FileName;
                    LoadImage(dialog.FileName);
                }
            }
        }

        void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(UrlText.Text)) LoadImageAsyn(UrlText.Text);
        }

        /// <summary>
        /// 加载图像，用于加载本地图像
        /// </summary>
        void LoadImage(string uri)
        {
            StatusPrompt.Content = "正在加载";
            PreviewImage.Source = null;
            bindingContext.Image = null; 

            // 加载图像
            Uri u = new Uri(uri, UriKind.RelativeOrAbsolute);
            BitmapImage img = new BitmapImage(u);
            PreviewImage.Source = img;

            // 更新绑定上下文
            bindingContext.ImageUrl = u.ToString();
            bindingContext.Image = img;
            bindingContext.OriginalWidth = img.PixelWidth;
            bindingContext.OriginalHeight = img.PixelHeight;

            ResizeSlider.Value = 100;
            ScrollToCenter();
        }

        /// <summary>
        /// 异步加载图像，用于加载网络上的图像
        /// </summary>
        void LoadImageAsyn(string uri)
        {
            StatusPrompt.Content = "正在下载";
            PreviewImage.Source = null;
            bindingContext.Image = null;
            TopContentArea.IsEnabled = false;

            // 异步下载图像
            Uri u = new Uri(uri, UriKind.RelativeOrAbsolute);
            BitmapImage img = new BitmapImage(u);
            img.DownloadCompleted += new EventHandler(ImageDownloadCompleted);
            img.DownloadFailed += new EventHandler<ExceptionEventArgs>(ImageDownloadFailed);
        }

        /// <summary>
        /// 图像下载完成时调用
        /// </summary>
        void ImageDownloadCompleted(object sender, EventArgs e)
        {
            // 异步下载图像完成
            StatusPrompt.Content = "下载完成";
            TopContentArea.IsEnabled = true;
            BitmapImage img = (BitmapImage)sender;
            PreviewImage.Source = img;

            // 更新绑定上下文
            bindingContext.ImageUrl = img.UriSource.ToString();
            bindingContext.Image = img;
            bindingContext.OriginalWidth = img.PixelWidth;
            bindingContext.OriginalHeight = img.PixelHeight;

            ResizeSlider.Value = 100;
            ScrollToCenter();
        }

        /// <summary>
        /// 图像下载失败时调用
        /// </summary>
        void ImageDownloadFailed(object sender, ExceptionEventArgs e)
        {
            // 异步下载图像失败
            StatusPrompt.Content = "无法加载图像";
            TopContentArea.IsEnabled = true;

            // 更新绑定上下文
            bindingContext.Image = null;
            bindingContext.Width = 0;
            bindingContext.Height = 0;
            bindingContext.OriginalWidth = 0;
            bindingContext.OriginalHeight = 0;
        }

        void ScrollToCenter()
        {
            if (PreviewImage.Width > PreviewScroll.ViewportWidth)
            {
                PreviewScroll.ScrollToHorizontalOffset((PreviewImage.Width - PreviewScroll.ViewportWidth) / 2);
            }

            if (PreviewImage.Height > PreviewScroll.ViewportHeight)
            {
                PreviewScroll.ScrollToVerticalOffset((PreviewImage.Height - PreviewScroll.ViewportHeight) / 2);
            }
        }
    }
}
