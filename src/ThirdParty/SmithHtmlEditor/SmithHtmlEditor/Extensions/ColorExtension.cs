using System;

namespace Smith.WPF.HtmlEditor
{
    /// <summary>
    /// 提供 System.Drawing.Color 和 System.Windows.Media.Color 之间的转换方法。
    /// To provide methods to handle convertion between System.Drawing.Color and System.Windows.Media.Color.
    /// </summary>
    internal static class ColorExtension
    {
        /// <summary>
        /// 将 System.Drawing.Color 转换到 System.Windows.Media.Color。
        /// Convert System.Drawing.Color to System.Windows.Media.Color.
        /// </summary>
        public static System.Windows.Media.Color ColorConvert(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// 将 System.Windows.Media.Color 转换到 System.Drawing.Color。
        /// Convert System.Windows.Media.Color to System.Drawing.Color.
        /// </summary>
        public static System.Drawing.Color ColorConvert(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// 判断 System.Drawing.Color 和 System.Windows.Media.Color 是否表示相同的颜色。
        /// </summary>
        public static bool ColorEqual(this System.Drawing.Color drawingColor, System.Windows.Media.Color mediaColor)
        {
            return (drawingColor.A == mediaColor.A &&
                    drawingColor.R == mediaColor.R &&
                    drawingColor.G == mediaColor.G &&
                    drawingColor.B == mediaColor.B);
        }

        /// <summary>
        /// 判断 System.Windows.Media.Color 和 System.Drawing.Color 是否表示相同的颜色。
        /// </summary>
        public static bool ColorEqual(this System.Windows.Media.Color mediaColor, System.Drawing.Color drawingColor)
        {
            return (drawingColor.A == mediaColor.A &&
                    drawingColor.R == mediaColor.R &&
                    drawingColor.G == mediaColor.G &&
                    drawingColor.B == mediaColor.B);
        }

        /// <summary>
        /// 将表达式转换到 System.Windows.Media.Color。
        /// Convert an expression to System.Windows.Media.Color.
        /// </summary>
        public static System.Windows.Media.Color ConvertToColor(string value)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value);
            //int r = 0, g = 0, b = 0;
            //if (value.StartsWith("#"))
            //{
            //    int v = Convert.ToInt32(value.Substring(1), 16);
            //    r = (v >> 16) & 255; g = (v >> 8) & 255; b = v & 255;
            //}
            //return System.Windows.Media.Color.FromRgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }

        public static readonly System.Windows.Media.Color DefaultBackColor = System.Windows.SystemColors.WindowColor;

        public static readonly System.Windows.Media.Color DefaultForeColor = System.Windows.SystemColors.WindowTextColor;
    }
}
