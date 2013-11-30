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
using System.Collections.ObjectModel;

namespace Smith.WPF.HtmlEditor
{
    /// <summary>
    /// TableDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TableDialog : Window
    {
        TableObject bindingContext;
        ReadOnlyCollection<Unit> unitOptions;
        ReadOnlyCollection<TableHeaderOption> headerOptions;
        ReadOnlyCollection<TableAlignment> alignmentOptions;

        public TableDialog()
        {
            InitializeComponent();
            InitUnitOptions();
            InitHeaderOptions();
            InitAlignmentOptions();
            InitEvents();
            InitBindingContext();            
        }

        public TableObject Model
        {
            get { return bindingContext; }
            private set
            {
                bindingContext = value;
                this.DataContext = bindingContext;
            }
        }

        void InitUnitOptions()
        {
            List<Unit> ls = new List<Unit> { Unit.Pixel, Unit.Percentage };
            unitOptions = new ReadOnlyCollection<Unit>(ls);

            WidthUnitSelection.ItemsSource = ls;
            HeightUnitSelection.ItemsSource = ls;
            SpaceUnitSelection.ItemsSource = ls;
            PaddingUnitSelection.ItemsSource = ls;
        }

        void InitHeaderOptions()
        {
            List<TableHeaderOption> ls = new List<TableHeaderOption>
            {
                TableHeaderOption.Default, 
                TableHeaderOption.FirstRow, 
                TableHeaderOption.FirstColumn, 
                TableHeaderOption.FirstRowAndColumn 
            };
            headerOptions = new ReadOnlyCollection<TableHeaderOption>(ls);
            HeaderSelection.ItemsSource = headerOptions;
        }

        void InitAlignmentOptions()
        {
            List<TableAlignment> ls = new List<TableAlignment>()
            {
                TableAlignment.Default, 
                TableAlignment.Left, 
                TableAlignment.Right, 
                TableAlignment.Center
            };
            alignmentOptions = new ReadOnlyCollection<TableAlignment>(ls);
            AlignmentSelection.ItemsSource = alignmentOptions;
        }

        void InitBindingContext()
        {
            Model = new TableObject
            {
                Columns = 5,
                Rows = 3,
                Border = 1,
                Width = 100,
                Height = 100,
                WidthUnit = Unit.Percentage,
                HeightUnit = Unit.Pixel,
                SpacingUnit = Unit.Pixel,
                PaddingUnit = Unit.Pixel,
                HeaderOption = TableHeaderOption.Default,
                Alignment = TableAlignment.Default
            };
        }

        void InitEvents()
        {
            OkayButton.Click += new RoutedEventHandler(OkayButton_Click);
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
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
    }
}
