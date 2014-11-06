using System.Text.RegularExpressions;
namespace Smith.WPF.HtmlEditor
{
    internal static class HtmlExtension
    {
        /// <summary>
        /// Perform html encoding.
        /// </summary>
        public static string HtmlEncoding(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("<", "&lt;");
                value = value.Replace(">", "&gt;");
                value = value.Replace(" ", "&nbsp;");
                value = value.Replace("\"", "&quot;");
                value = value.Replace("\'", "&#39;");
                value = value.Replace("&", "&amp;");
                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Perform html decoding.
        /// </summary>
        public static string HtmlDecoding(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("&lt;", "<");
                value = value.Replace("&gt;", ">");
                value = value.Replace("&nbsp;", " ");
                value = value.Replace("&quot;", "\"");
                value = value.Replace("&#39;", "\'");
                value = value.Replace("&amp;", "&");
                return value;
            } 
            return string.Empty;
        }

        /// <summary>
        /// Filter all html tags.
        /// </summary>
        public static string FilterAllTags(this string value)
        {
            Regex match = new Regex("<[^>]+>");
            return match.Replace(value, string.Empty);
        }
    }
}
