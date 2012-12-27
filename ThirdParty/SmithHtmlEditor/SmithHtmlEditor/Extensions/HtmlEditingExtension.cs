using System;
using System.Text;
using System.Windows.Media;
using mshtml;

namespace Smith.WPF.HtmlEditor
{
    internal static class HtmlEditingExtension
    {
        /// <summary>
        /// 撤销命令是否可执行
        /// </summary>
        public static bool CanUndo(this HtmlDocument document)
        {
            return document.QueryCommandEnabled("Undo");
        }

        /// <summary>
        /// 重做命令是否可执行
        /// </summary>
        public static bool CanRedo(this HtmlDocument document)
        {
            return document.QueryCommandEnabled("Redo");
        }

        /// <summary>
        /// 剪切命令是否可执行
        /// </summary>
        public static bool CanCut(this HtmlDocument document)
        {
            return document.QueryCommandEnabled("Cut");
        }

        /// <summary>
        /// 复制命令是否可执行
        /// </summary>
        public static bool CanCopy(this HtmlDocument document)
        {
            return document.QueryCommandEnabled("Copy");
        }

        /// <summary>
        /// 粘贴命令是否可执行
        /// </summary>
        public static bool CanPaste(this HtmlDocument document)
        {
            return document.QueryCommandEnabled("Paste");
        }

        /// <summary>
        /// 删除命令是否可执行
        /// </summary>
        public static bool CanDelete(this HtmlDocument document)
        {
            return document.QueryCommandEnabled("Delete");
        }

        /// <summary>
        /// 设置下标命令是否可执行
        /// </summary>
        public static bool CanSubscript(this HtmlDocument document)
        {
            return document.QueryCommandSupported("Subscript") && document.QueryCommandEnabled("Subscript");
        }

        /// <summary>
        /// 设置上标命令是否可执行
        /// </summary>
        public static bool CanSuperscript(this HtmlDocument document)
        {
            return document.QueryCommandSupported("Superscript") && document.QueryCommandEnabled("Superscript");
        }

        /// <summary>
        /// 是否是左对齐
        /// </summary>
        public static bool IsJustifyLeft(this HtmlDocument document)
        {
            return document.QueryCommandState("JustifyLeft");
        }

        /// <summary>
        /// 是否是右对齐
        /// </summary>
        public static bool IsJustifyRight(this HtmlDocument document)
        {
            return document.QueryCommandState("JustifyRight");
        }

        /// <summary>
        /// 是否是中间对齐
        /// </summary>
        public static bool IsJustifyCenter(this HtmlDocument document)
        {
            return document.QueryCommandState("JustifyCenter");
        }

        /// <summary>
        /// 是否是两端对齐
        /// </summary>
        public static bool IsJustifyFull(this HtmlDocument document)
        {
            return document.QueryCommandState("JustifyFull");
        }

        /// <summary>
        /// 是否是粗体
        /// </summary>
        public static bool IsBold(this HtmlDocument document)
        {
            return document.QueryCommandState("Bold");
        }

        /// <summary>
        /// 是否是斜体
        /// </summary>
        public static bool IsItalic(this HtmlDocument document)
        {
            return document.QueryCommandState("Italic");
        }

        /// <summary>
        /// 是否有下划线
        /// </summary>
        public static bool IsUnderline(this HtmlDocument document)
        {
            return document.QueryCommandState("Underline");
        }

        /// <summary>
        /// 是否是下标
        /// </summary>
        public static bool IsSubscript(this HtmlDocument document)
        {
            return document.QueryCommandSupported("Subscript") && 
                   document.QueryCommandState("Subscript");
        }

        /// <summary>
        /// 是否是上标
        /// </summary>
        public static bool IsSuperscript(this HtmlDocument document)
        {
            return document.QueryCommandSupported("Superscript") && 
                   document.QueryCommandState("Superscript");
        }

        /// <summary>
        /// 是否是无序列表
        /// </summary>
        public static bool IsBulletsList(this HtmlDocument document)
        {
            return document.QueryCommandState("InsertUnorderedList");
        }

        /// <summary>
        /// 是否是有序列表
        /// </summary>
        public static bool IsNumberedList(this HtmlDocument document)
        {
            return document.QueryCommandState("InsertOrderedList");
        }

        /// <summary>
        /// 执行撤销命令
        /// </summary>
        public static void Undo(this HtmlDocument document)
        {
            document.ExecuteCommand("Undo", false, null);
        }

        /// <summary>
        /// 执行重做命令
        /// </summary>
        public static void Redo(this HtmlDocument document)
        {
            document.ExecuteCommand("Redo", false, null);
        }

        /// <summary>
        /// 执行剪切命令
        /// </summary>
        public static void Cut(this HtmlDocument document)
        {
            document.ExecuteCommand("Cut", false, null);
        }

        /// <summary>
        /// 执行复制命令
        /// </summary>
        public static void Copy(this HtmlDocument document)
        {
            document.ExecuteCommand("Copy", false, null);
        }

        /// <summary>
        /// 执行粘贴命令
        /// </summary>
        public static void Paste(this HtmlDocument document)
        {
            document.ExecuteCommand("Paste", false, null);
        }

        /// <summary>
        /// 执行删除命令
        /// </summary>
        public static void Delete(this HtmlDocument document)
        {
            document.ExecuteCommand("Delete", false, null);
        }

        /// <summary>
        /// 执行全选命令
        /// </summary>
        public static void SelectAll(this HtmlDocument document)
        {
            document.ExecuteCommand("SelectAll", false, null);
        }

        /// <summary>
        /// 执行设为粗体命令
        /// </summary>
        public static void Bold(this HtmlDocument document)
        {
            document.ExecuteCommand("Bold", false, null);
        }

        /// <summary>
        /// 执行设为斜体命令
        /// </summary>
        public static void Italic(this HtmlDocument document)
        {
            document.ExecuteCommand("Italic", false, null);
        }

        /// <summary>
        /// 执行添加下划线命令
        /// </summary>
        public static void Underline(this HtmlDocument document)
        {
            document.ExecuteCommand("Underline", false, null);
        }

        /// <summary>
        /// 执行设为下标命令
        /// </summary>
        public static void Subscript(this HtmlDocument document)
        {
            if (document.QueryCommandSupported("Subscript") &&
                document.QueryCommandEnabled("Subscript")) 
                document.ExecuteCommand("Subscript", false, null);
        }

        /// <summary>
        /// 执行设为上标命令
        /// </summary>
        public static void Superscript(this HtmlDocument document)
        {
            if (document.QueryCommandSupported("Superscript") &&
                document.QueryCommandEnabled("Superscript")) 
                document.ExecuteCommand("Superscript", false, null);
        }

        /// <summary>
        /// 执行清除样式命令
        /// </summary>
        public static void ClearStyle(this HtmlDocument document)
        {
            document.ExecuteCommand("RemoveFormat", false, null);
        }

        /// <summary>
        /// 执行增加缩进命令
        /// </summary>
        public static void Indent(this HtmlDocument document)
        {
            document.ExecuteCommand("Indent", false, null);
        }

        /// <summary>
        /// 执行减少缩进命令
        /// </summary>
        /// <param name="document"></param>
        public static void Outdent(this HtmlDocument document)
        {
            document.ExecuteCommand("Outdent", false, null);
        }

        /// <summary>
        /// 执行设为无序列表命令
        /// </summary>
        public static void BulletsList(this HtmlDocument document)
        {
            document.ExecuteCommand("InsertUnorderedList", false, null);
        }

        /// <summary>
        /// 执行设为有序列表命令
        /// </summary>
        public static void NumberedList(this HtmlDocument document)
        {
            document.ExecuteCommand("InsertOrderedList", false, null);
        }

        /// <summary>
        /// 执行设为左对齐命令
        /// </summary>
        public static void JustifyLeft(this HtmlDocument document)
        {
            document.ExecuteCommand("JustifyLeft", false, null);
        }

        /// <summary>
        /// 执行设为右对齐命令
        /// </summary>
        public static void JustifyRight(this HtmlDocument document)
        {
            document.ExecuteCommand("JustifyRight", false, null);
        }

        /// <summary>
        /// 执行设为中间对齐命令
        /// </summary>
        public static void JustifyCenter(this HtmlDocument document)
        {
            document.ExecuteCommand("JustifyCenter", false, null);
        }

        /// <summary>
        /// 执行设为两端对齐命令
        /// </summary>
        public static void JustifyFull(this HtmlDocument document)
        {
            document.ExecuteCommand("JustifyFull", false, null);
        }        

        /// <summary>
        /// 执行插入超链接命令
        /// </summary>
        public static void InsertHyperlick(this HtmlDocument document, HyperlinkObject hyperlink)
        {
            var url = hyperlink.URL.HtmlEncoding();
            var txt = hyperlink.Text.HtmlEncoding();
            if (string.IsNullOrEmpty(txt)) txt = url;
            string tx = string.Format("<a href=\"{0}\">{1}</a>", url, txt);
            document.InsertHTML(tx);
        }

        /// <summary>
        /// 执行插入图像命令
        /// </summary>
        public static void InsertImage(this HtmlDocument document, ImageObject image)
        {
            string hspace = (image.HorizontalSpace > 0 ? "hspace=\"" + image.HorizontalSpace + "\" " : string.Empty);
            string vspace = (image.VerticalSpace > 0 ? "vspace=\"" + image.VerticalSpace + "\" " : string.Empty);
            string border = (image.BorderSize > 0 ? "border=\"" + image.BorderSize + "\" " : string.Empty);
            string align = (image.Alignment != ImageAlignment.Default ? "align=\"" + image.Alignment.Value + "\" " : string.Empty);
            string title = (string.IsNullOrEmpty(image.TitleText) == false ? "title=\"" + image.TitleText + "\" " : string.Empty);
            string tx = string.Empty;
            if (string.IsNullOrEmpty(image.LinkUrl))
            {
                tx = string.Format("<img src=\"{0}\" alt=\"{1}\" width=\"{2}\" height=\"{3}\" {4}{5}{6}{7}{8} />",
                    image.ImageUrl, image.AltText, image.Width, image.Height, title, hspace, vspace, border, align);
            }
            else
            {
                string url = image.ImageUrl.HtmlEncoding();
                tx = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" width=\"{3}\" height=\"{4}\" {5}{6}{7}{8}{9} /></a>",
                    image.LinkUrl, image.ImageUrl, image.AltText, image.Width, image.Height, title, hspace, vspace, border, align);
            }
            document.InsertHTML(tx);
        }

        /// <summary>
        /// 执行插入表格命令
        /// </summary>
        public static void InsertTable(this HtmlDocument document, TableObject table)
        {
            int rows = table.Rows;
            int cols = table.Columns;
            string width = string.Format(" width=\"{0}{1}\"", table.Width, table.WidthUnit.Value);
            string height = string.Format(" height=\"{0}{1}\"", table.Height, table.HeightUnit.Value);
            string spacing = string.Format(" cellspacing=\"{0}{1}\"", table.Spacing, (table.Spacing > 0 ? table.SpacingUnit.Value : string.Empty));
            string padding = string.Format(" cellspacing=\"{0}{1}\"", table.Padding, (table.Padding > 0 ? table.PaddingUnit.Value : string.Empty));
            string border = string.Format(" border=\"{0}\"", table.Border);
            string title = (string.IsNullOrEmpty(table.Title) != false ? string.Format(" title=\"{0}\"", table.Title.HtmlEncoding()) : string.Empty);
            string align = (table.Alignment != TableAlignment.Default ? string.Format(" align=\"{0}\"", table.Alignment.Value) : string.Empty);

            StringBuilder bx = new StringBuilder();
            bx.AppendFormat("<table{0}{1}{2}{3}{4}{5}{6}>", width, height, spacing, padding, border, align, title);
            for (int i = 0; i < rows; i++)
            {
                bx.Append("<tr>");
                if (i == 0 && (table.HeaderOption == TableHeaderOption.FirstRow || table.HeaderOption == TableHeaderOption.FirstRowAndColumn))
                {
                    for (int j = 0; j < cols; j++) bx.Append("<th></th>");
                }
                else
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == 0 && (table.HeaderOption == TableHeaderOption.FirstColumn || table.HeaderOption == TableHeaderOption.FirstRowAndColumn))
                        {
                            bx.Append("<th></th>");
                        }
                        else bx.Append("<td></td>"); 
                    }
                }
                bx.Append("</tr>");
            }
            bx.Append("</table>");
            document.InsertHTML(bx.ToString());
        }

        /// <summary>
        /// 获取文档选区的字体
        /// </summary>
        public static FontFamily GetFontFamily(this HtmlDocument document)
        {
            if (document.State != HtmlDocumentState.Complete) return null;
            string name = document.QueryCommandValue("FontName") as string;
            if (name == null) return null;
            return new FontFamily(name);
        }

        /// <summary>
        /// 设置文档选区的字体
        /// </summary>
        public static void SetFontFamily(this HtmlDocument document, FontFamily value)
        {
            document.ExecuteCommand("FontName", false, value.ToString());
        }

        /// <summary>
        /// 获取文档选区的字体大小
        /// </summary>
        public static FontSize GetFontSize(this HtmlDocument document)
        {
            if (document.State != HtmlDocumentState.Complete) return FontSize.NO;
            switch (document.QueryCommandValue("FontSize").ToString())
            {
                case "1": return FontSize.XXSmall;
                case "2": return FontSize.XSmall;
                case "3": return FontSize.Small;
                case "4": return FontSize.Middle;
                case "5": return FontSize.Large;
                case "6": return FontSize.XLarge;
                case "7": return FontSize.XXLarge;
                default: return FontSize.NO;
            }
        }

        /// <summary>
        /// 设置文档选区的字体大小
        /// </summary>
        public static void SetFontSize(this HtmlDocument document, FontSize value)
        {
            if (value != null && value != FontSize.NO)
            {
                document.ExecuteCommand("FontSize", false, value.Key);
            }
        }

        /// <summary>
        /// 获取文档选区文字的前景色
        /// </summary>
        public static Color GetFontColor(this HtmlDocument document)
        {
            if (document.State != HtmlDocumentState.Complete) return Colors.Black;
            return ColorExtension.ConvertToColor(document.QueryCommandValue("ForeColor").ToString());
        }

        /// <summary>
        /// 设置文档选区文字的前景色
        /// </summary>
        public static void SetFontColor(this HtmlDocument document, Color value)
        {
            document.ExecuteCommand("ForeColor", false, string.Format("#{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B));
        }

        /// <summary>
        /// 获取文档选区文字的背景色
        /// </summary>
        public static Color GetLineColor(this HtmlDocument document)
        {
            if (document.State != HtmlDocumentState.Complete) return Colors.Black;
            return ColorExtension.ConvertToColor(document.QueryCommandValue("BackColor").ToString());
        }

        /// <summary>
        /// 设置文档选区文字的背景色
        /// </summary>
        public static void SetLineColor(this HtmlDocument document, Color value)
        {
            document.ExecuteCommand("BackColor", false, string.Format("#{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B));
        }
    }
}
