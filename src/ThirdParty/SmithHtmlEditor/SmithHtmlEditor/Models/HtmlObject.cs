using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Smith.WPF.HtmlEditor
{
    public class HtmlObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class HyperlinkObject : HtmlObject
    {
        public string URL
        {
            get { return fdURL; }
            set
            {
                fdURL = value;
                RaisePropertyChanged("URL");
            }
        }

        public string Text
        {
            get { return fdText; }
            set
            {
                fdText = value;
                RaisePropertyChanged("Text");
            }
        }

        string fdText;
        string fdURL;
    }

    public class ImageObject : HtmlObject
    {
        public int Width
        {
            get { return fdWidth; }
            set
            {
                fdWidth = value;
                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get { return fdHeight; }
            set
            {
                fdHeight = value;
                RaisePropertyChanged("Height");
            }
        }

        public int OriginalWidth
        {
            get { return fdOriginalWidth; }
            set
            {
                fdOriginalWidth = value;
                RaisePropertyChanged("OriginalWidth");
            }
        }

        public int OriginalHeight
        {
            get { return fdOriginalHeight; }
            set
            {
                fdOriginalHeight = value;
                RaisePropertyChanged("OriginalHeight");
            }
        }

        public int HorizontalSpace
        {
            get { return fdHorizontalSpace; }
            set
            {
                fdHorizontalSpace = value;
                RaisePropertyChanged("HorizontalSpace");
            }
        }

        public int VerticalSpace
        {
            get { return fdVerticalSpace; }
            set
            {
                fdVerticalSpace = value;
                RaisePropertyChanged("VerticalSpace");
            }
        }

        public int BorderSize
        {
            get { return fdBorderSize; }
            set
            {
                fdBorderSize = value;
                RaisePropertyChanged("BorderSize");
            }
        }

        public ImageAlignment Alignment
        {
            get { return fdAlignment; }
            set
            {
                fdAlignment = value;
                RaisePropertyChanged("Alignment");
            }
        }

        public string TitleText
        {
            get { return fdTitleText; }
            set
            {
                fdTitleText = value;
                RaisePropertyChanged("Title");
            }
        }

        public string AltText
        {
            get { return fdAltText; }
            set
            {
                fdAltText = value;
                RaisePropertyChanged("AlternativeText");
            }
        }

        public string LinkUrl
        {
            get { return fdLinkUrl; }
            set
            {
                fdLinkUrl = value;
                RaisePropertyChanged("LinkUrl");
            }
        }

        public string ImageUrl
        {
            get { return fdImageUrl; }
            set
            {
                fdImageUrl = value;
                RaisePropertyChanged("ImageUrl");
            }
        }

        public BitmapImage Image
        {
            get { return fdImage; }
            set
            {
                fdImage = value;
                RaisePropertyChanged("Image");
            }
        }

        #region 字段

        BitmapImage fdImage;
        ImageAlignment fdAlignment;
        string fdImageUrl;
        string fdLinkUrl;
        string fdAltText;
        string fdTitleText;
        int fdBorderSize;
        int fdVerticalSpace;
        int fdHorizontalSpace;
        int fdOriginalHeight;
        int fdOriginalWidth;
        int fdHeight;
        int fdWidth; 

        #endregion
    }

    public class TableObject : HtmlObject
    {
        public int Columns
        {
            get { return fdColumns; }
            set
            {
                if (value <= 0) value = 1;
                fdColumns = value;
                RaisePropertyChanged("Columns");
            }
        }

        public int Rows
        {
            get { return fdRows; }
            set
            {
                if (value <= 0) value = 1;
                fdRows = value;
                RaisePropertyChanged("Rows");
            }
        }

        public int Width
        {
            get { return fdWidth; }
            set
            {
                fdWidth = value;
                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get { return fdHeight; }
            set
            {
                fdHeight = value;
                RaisePropertyChanged("Height");
            }
        }

        public int Spacing
        {
            get { return fdSpacing; }
            set
            {
                fdSpacing = value;
                RaisePropertyChanged("Spacing");
            }
        }

        public int Padding
        {
            get { return fdPadding; }
            set
            {
                fdPadding = value;
                RaisePropertyChanged("Padding");
            }
        }

        public int Border
        {
            get { return fdBorder; }
            set
            {
                fdBorder = value;
                RaisePropertyChanged("Border");
            }
        }

        public string Title
        {
            get { return fdTitle; }
            set
            {
                fdTitle = value;
                RaisePropertyChanged("Title");
            }
        }

        public Unit WidthUnit
        {
            get { return fdWidthUnit; }
            set
            {
                fdWidthUnit = value;
                RaisePropertyChanged("WidthUnit");
            }
        }

        public Unit HeightUnit
        {
            get { return fdHeightUnit; }
            set
            {
                fdHeightUnit = value;
                RaisePropertyChanged("HeightUnit");
            }
        }

        public Unit SpacingUnit
        {
            get { return fdSpacingUnit; }
            set
            {
                fdSpacingUnit = value;
                RaisePropertyChanged("SpacingUnit");
            }
        }

        public Unit PaddingUnit
        {
            get { return fdPaddingUnit; }
            set
            {
                fdPaddingUnit = value;
                RaisePropertyChanged("PaddingUnit");
            }
        }

        public TableHeaderOption HeaderOption
        {
            get { return fdHeaderOption; }
            set
            {
                fdHeaderOption = value;
                RaisePropertyChanged("HeaderOption");
            }
        }

        public TableAlignment Alignment
        {
            get { return fdAlignment; }
            set
            {
                fdAlignment = value;
                RaisePropertyChanged("Alignment");
            }
        }

        TableAlignment fdAlignment;
        TableHeaderOption fdHeaderOption;
        Unit fdPaddingUnit;
        Unit fdSpacingUnit;
        Unit fdHeightUnit;
        Unit fdWidthUnit;
        string fdTitle;
        int fdBorder;
        int fdPadding;
        int fdSpacing;
        int fdHeight;
        int fdWidth;
        int fdRows;
        int fdColumns;
    }
}
