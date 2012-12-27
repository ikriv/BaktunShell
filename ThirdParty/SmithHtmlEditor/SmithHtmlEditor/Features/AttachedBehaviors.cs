using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Smith.WPF.HtmlEditor
{
    internal static class PositiveIntegerInput
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable",
                typeof(bool), typeof(PositiveIntegerInput), new FrameworkPropertyMetadata(false, OnEnableChanged));

        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }

        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        static KeyEventHandler keyDownEventHandler = new KeyEventHandler(HandleKeyDown);
        static TextCompositionEventHandler textInputEventHandler = new TextCompositionEventHandler(HandleTextInput);
        static RoutedEventHandler lostFocusEventHandler = new RoutedEventHandler(HandleLostFocus);

        static void OnEnableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null && e.NewValue is bool)
            {
                if ((bool)e.NewValue)
                {
                    textbox.AddHandler(TextBox.PreviewKeyDownEvent, keyDownEventHandler);
                    textbox.AddHandler(TextBox.PreviewTextInputEvent, textInputEventHandler);
                    textbox.AddHandler(TextBox.LostFocusEvent, lostFocusEventHandler);
                    DataObject.AddPastingHandler(textbox, HandlePasting);
                }
                else
                {
                    textbox.RemoveHandler(TextBox.PreviewKeyDownEvent, keyDownEventHandler);
                    textbox.RemoveHandler(TextBox.PreviewTextInputEvent, textInputEventHandler);
                    textbox.RemoveHandler(TextBox.LostFocusEvent, lostFocusEventHandler);
                    DataObject.RemovePastingHandler(textbox, HandlePasting);
                }
            }
        }

        /// <summary>
        /// 响应粘贴事件
        /// </summary>
        static void HandlePasting(object sender, DataObjectPastingEventArgs e)
        {
            string content = e.DataObject.GetData(typeof(string)) as string;
            HandleInput((TextBox)sender, content);
            e.CancelCommand();
            e.Handled = true;
        }

        /// <summary>
        /// 响应失去焦点事件
        /// </summary>
        static void HandleLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null) tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }        

        /// <summary>
        /// 响应文本输入事件
        /// </summary>
        static void HandleTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            HandleInput((TextBox)sender, e.Text);
        }

        /// <summary>
        /// 响应键盘按键事件
        /// </summary>
        static void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                e.Handled = true;
                HandleBackspaceKeyInput((TextBox)sender);
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                HandleDeleteKeyInput((TextBox)sender);
            }
        }

        /// <summary>
        /// 处理文本输入
        /// </summary>
        static void HandleInput(TextBox textBox, string inputValue)
        {
            inputValue = inputValue.Trim();
            if (string.IsNullOrEmpty(inputValue)) return;

            int caret = textBox.CaretIndex;
            StringBuilder xb = new StringBuilder(textBox.Text);
            if (textBox.SelectionLength > 0)
            {
                xb.Remove(textBox.SelectionStart, textBox.SelectionLength);
                caret = textBox.SelectionStart;
            }
            xb.Insert(caret, inputValue);
            string sign = xb.ToString();
            if (ValidChanges(sign))
            {
                textBox.Text = sign;
                textBox.CaretIndex = caret + inputValue.Length;
            }
        }

        /// <summary>
        /// 处理退格键输入
        /// </summary>
        static void HandleBackspaceKeyInput(TextBox textBox)
        {
            if (textBox.SelectionLength > 0)
            {
                int caret = textBox.SelectionStart;
                string sign = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                if (ValidChanges(sign))
                {
                    textBox.Text = sign;
                    textBox.CaretIndex = caret;
                }
            }
            else if (textBox.CaretIndex > 0)
            {
                int caret = textBox.CaretIndex;
                string sign = textBox.Text.Remove(caret - 1, 1);
                if (ValidChanges(sign))
                {
                    textBox.Text = sign;
                    textBox.CaretIndex = caret - 1;
                }
            }
        }

        /// <summary>
        /// 处理删除键操作
        /// </summary>
        static void HandleDeleteKeyInput(TextBox textBox)
        {
            if (textBox.SelectionLength > 0)
            {
                int caret = textBox.SelectionStart;
                string sign = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                if (ValidChanges(sign))
                {
                    textBox.Text = sign;
                    textBox.CaretIndex = caret;
                }
            }
            else
            {
                int caret = textBox.CaretIndex;
                string sign = textBox.Text.Remove(caret, 1);
                if (ValidChanges(sign))
                {
                    textBox.Text = sign;
                    textBox.CaretIndex = caret;
                }
            }
        }

        /// <summary>
        /// 检验输入是否有效
        /// </summary>
        static bool ValidChanges(string value)
        {
            int val;
            return Int32.TryParse(value, out val);
        }
    }

    internal static class ScrollViewContentDragable
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable",
                typeof(bool), typeof(ScrollViewContentDragable), new FrameworkPropertyMetadata(false, OnEnableChanged));

        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }

        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        static readonly DependencyProperty StartPointProperty =
            DependencyProperty.RegisterAttached("StartPoint",
                typeof(Point), typeof(ScrollViewContentDragable), new FrameworkPropertyMetadata(new Point(0, 0)));

        static Point GetStartPoint(DependencyObject obj)
        {
            return (Point)obj.GetValue(StartPointProperty);
        }

        static void SetStartPoint(DependencyObject obj, Point value)
        {
            obj.SetValue(StartPointProperty, value);
        }

        static void OnEnableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (scrollviewer != null && e.NewValue is bool && 
                scrollviewer.Content != null && scrollviewer.Content is FrameworkElement)
            {
                FrameworkElement fe = scrollviewer.Content as FrameworkElement;
                if ((bool)e.NewValue)
                {
                    SetStartPoint(scrollviewer, new Point(-1, -1));
                    fe.AddHandler(FrameworkElement.PreviewMouseDownEvent, mouseButtonDownHandle);
                    fe.AddHandler(FrameworkElement.PreviewMouseUpEvent, mouseButtonUpHandle);
                    fe.AddHandler(FrameworkElement.PreviewMouseMoveEvent, mouseMoveHandle);
                }
                else
                {
                    fe.RemoveHandler(FrameworkElement.PreviewMouseDownEvent, mouseButtonDownHandle);
                    fe.RemoveHandler(FrameworkElement.PreviewMouseUpEvent, mouseButtonUpHandle);
                    fe.RemoveHandler(FrameworkElement.PreviewMouseMoveEvent, mouseMoveHandle);
                }
            }
        }

        static void HandleContentMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;

            ScrollViewer sc = fe.Parent as ScrollViewer;
            if (sc == null) return;

            Point sp = GetStartPoint(sc);
            if (sp.X > 0 && sp.Y > 0)
            {
                Point p = e.GetPosition(sc);
                double dtX = p.X - sp.X;
                double dtY = p.Y - sp.Y;
                sc.ScrollToHorizontalOffset(sc.ContentHorizontalOffset - dtX);
                sc.ScrollToVerticalOffset(sc.ContentVerticalOffset - dtY);
                SetStartPoint(sc, new Point(p.X, p.Y));
            }
        }

        static void HandleContentMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;

            ScrollViewer sc = fe.Parent as ScrollViewer;
            if (sc == null) return;

            SetStartPoint(sc, e.GetPosition(sc));
        }

        static void HandleContentMouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;

            ScrollViewer sc = fe.Parent as ScrollViewer;
            if (sc == null) return;

            SetStartPoint(sc, new Point(-1, -1));
        }

        static MouseButtonEventHandler mouseButtonDownHandle = new MouseButtonEventHandler(HandleContentMouseDown);
        static MouseButtonEventHandler mouseButtonUpHandle = new MouseButtonEventHandler(HandleContentMouseUp);
        static MouseEventHandler mouseMoveHandle = new MouseEventHandler(HandleContentMouseMove);
    }
}
