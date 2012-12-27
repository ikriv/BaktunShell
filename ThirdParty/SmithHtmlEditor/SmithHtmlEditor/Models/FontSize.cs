namespace Smith.WPF.HtmlEditor
{
    public sealed class FontSize
    {
        public int Key { get; set; }
        public double Size { get; set; }
        public string Text { get; set; }

        public static readonly FontSize NO = new FontSize { Size = 0 };
        public static readonly FontSize XXSmall = new FontSize { Key = 1, Size = 8.5, Text = "8pt" };
        public static readonly FontSize XSmall = new FontSize { Key = 2, Size = 10.5, Text = "10pt" };
        public static readonly FontSize Small = new FontSize { Key = 3, Size = 12, Text = "12pt" };
        public static readonly FontSize Middle = new FontSize { Key = 4, Size = 14, Text = "14pt" };
        public static readonly FontSize Large = new FontSize { Key = 5, Size = 18, Text = "18pt" };
        public static readonly FontSize XLarge = new FontSize { Key = 6, Size = 24, Text = "24pt" };
        public static readonly FontSize XXLarge = new FontSize { Key = 7, Size = 36, Text = "36pt" };
    }
}
