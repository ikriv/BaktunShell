using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Smith.WPF.HtmlEditor
{
    /// <summary>
    /// 用于封装 mshtml.IHTMLDocument2 接口。
    /// 避免直接使用 mshtml 命名空间下的组件，使用时就无需添加对 MSHTML 库的引用。
    /// </summary>
    public class HtmlDocument : HtmlObject
    {
        internal HtmlDocument(System.Windows.Forms.HtmlDocument htmlDocument)
        {
            sysWinFormHtmlDoc = htmlDocument;
            msHtmlDocInterface = (mshtml.IHTMLDocument2)htmlDocument.DomDocument;
        }

        /// <summary>
        /// 在当前HTML文档上执行命令
        /// </summary>
        /// <param name="commandID">命令名称</param>
        /// <param name="showUI">是否使用内置的对话框或消息框</param>
        /// <param name="value">命令参数</param>
        public void ExecuteCommand(string commandID, bool showUI, object value)
        {
            msHtmlDocInterface.execCommand(commandID, showUI, value);
        }

        /// <summary>
        /// 清空文档内容
        /// </summary>
        public void Clear()
        {
            sysWinFormHtmlDoc.Body.InnerHtml = string.Empty;
            RaiseContentChanged();
        }

        /// <summary>
        /// 查询命令是否可执行
        /// </summary>
        /// <param name="commandID">命令ID</param>
        public bool QueryCommandEnabled(string commandID)
        {
            return msHtmlDocInterface.queryCommandEnabled(commandID);
        }

        // 该方法的作用未知
        public bool QueryCommandIndeterm(string commandID)
        {
            return msHtmlDocInterface.queryCommandIndeterm(commandID);
        }

        /// <summary>
        /// 查询命令状态
        /// </summary>
        /// <param name="commandID">命令ID</param>
        public bool QueryCommandState(string commandID)
        {
            return msHtmlDocInterface.queryCommandState(commandID);
        }

        /// <summary>
        /// 查询命令是否被支持
        /// </summary>
        /// <param name="commandID">命令ID</param>
        public bool QueryCommandSupported(string commandID)
        {
            return msHtmlDocInterface.queryCommandSupported(commandID);
        }

        /// <summary>
        /// 查询命令文本
        /// </summary>
        /// <param name="commandID">命令ID</param>
        public string QueryCommandText(string commandID)
        {
            return msHtmlDocInterface.queryCommandText(commandID);
        }

        /// <summary>
        /// 查询命令值
        /// </summary>
        /// <param name="commandID">命令ID</param>
        public object QueryCommandValue(string commandID)
        {
            return msHtmlDocInterface.queryCommandValue(commandID);
        }

        /// <summary>
        /// 在文档选区插入HTML内容
        /// </summary>
        public void InsertHTML(string content)
        {
            var range = msHtmlDocInterface.selection.createRange() as IHTMLTxtRange;
            range.pasteHTML(content);
            RaiseContentChanged();
        }        

        /// <summary>
        /// 获取文档状态
        /// </summary>
        public HtmlDocumentState State
        {
            get
            {
                switch (msHtmlDocInterface.readyState.ToLower())
                {
                    case "loading": return HtmlDocumentState.Loading;
                    case "loaded": return HtmlDocumentState.Loaded;
                    case "interactive": return HtmlDocumentState.Interactive;
                    case "complete": return HtmlDocumentState.Complete;
                    default: return HtmlDocumentState.Uninitialized;
                }
            }
        }

        /// <summary>
        /// 获取文档默认编码方式
        /// </summary>
        public string DefaultEncoding
        {
            get { return sysWinFormHtmlDoc.DefaultEncoding; }
        }

        /// <summary>
        /// 获取或设置文档编码方式
        /// </summary>
        public string Encoding
        {
            get { return sysWinFormHtmlDoc.Encoding; }
            set
            {
                if (value == sysWinFormHtmlDoc.Title) return;
                sysWinFormHtmlDoc.Encoding = value;
                RaisePropertyChanged("Encoding");
            }
        }

        /// <summary>
        /// 获取或设置文档标题
        /// </summary>
        public string Title
        {
            get { return sysWinFormHtmlDoc.Title; }
            set
            {
                if (value == sysWinFormHtmlDoc.Encoding) return;
                sysWinFormHtmlDoc.Title = value;
                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// 获取或设置文档背景颜色
        /// </summary>
        public System.Windows.Media.Color BackColor
        {
            get { return sysWinFormHtmlDoc.BackColor.ColorConvert(); }
            set
            {
                if (sysWinFormHtmlDoc.BackColor.ColorEqual(value)) return;
                sysWinFormHtmlDoc.BackColor = value.ColorConvert();
                RaisePropertyChanged("BackColor");
            }
        }

        /// <summary>
        /// 获取或设置文档前景颜色
        /// </summary>
        public System.Windows.Media.Color ForeColor
        {
            get { return sysWinFormHtmlDoc.ForeColor.ColorConvert(); }
            set
            {
                if (sysWinFormHtmlDoc.ForeColor.ColorEqual(value)) return;
                sysWinFormHtmlDoc.ForeColor = value.ColorConvert();
                RaisePropertyChanged("ForeColor");
            }
        }

        /// <summary>
        /// 获取或设置超链接文本颜色
        /// </summary>
        public System.Windows.Media.Color LinkColor
        {
            get { return sysWinFormHtmlDoc.LinkColor.ColorConvert(); }
            set
            {
                if (sysWinFormHtmlDoc.LinkColor.ColorEqual(value)) return;
                sysWinFormHtmlDoc.LinkColor = value.ColorConvert();
                RaisePropertyChanged("LinkColor");
            }
        }

        /// <summary>
        /// 获取或设置活动的超链接文本颜色
        /// </summary>
        public System.Windows.Media.Color ActiveLinkColor
        {
            get { return sysWinFormHtmlDoc.ActiveLinkColor.ColorConvert(); }
            set
            {
                if (sysWinFormHtmlDoc.ActiveLinkColor.ColorEqual(value)) return;
                sysWinFormHtmlDoc.ActiveLinkColor = value.ColorConvert();
                RaisePropertyChanged("ActiveLinkColor");
            }
        }

        /// <summary>
        /// 获取或设置已访问的超链接文本颜色
        /// </summary>
        public System.Windows.Media.Color VisitedLinkColor
        {
            get { return sysWinFormHtmlDoc.VisitedLinkColor.ColorConvert(); }
            set
            {
                if (sysWinFormHtmlDoc.VisitedLinkColor.ColorEqual(value)) return;
                sysWinFormHtmlDoc.VisitedLinkColor = value.ColorConvert();
                RaisePropertyChanged("VisitedLinkColor");
            }
        }

        /// <summary>
        /// 获取或设置文档的HTML内容
        /// </summary>
        public string Content
        {
            get { return sysWinFormHtmlDoc.Body.InnerHtml; }
            set
            {
                sysWinFormHtmlDoc.Body.InnerHtml = value;
                RaiseContentChanged();
            }
        }

        /// <summary>
        /// 获取文档的纯文本内容
        /// </summary>
        public string Text
        {
            get { return sysWinFormHtmlDoc.Body.InnerText; }
        }

        /// <summary>
        /// 获取文档当前的选区
        /// </summary>
        public Range Selection
        {
            get { return new Range((IHTMLTxtRange)msHtmlDocInterface.selection.createRange()); }
        }

        private void RaiseContentChanged()
        {
            RaisePropertyChanged("HtmlContent");
            RaisePropertyChanged("TextContent");
        }

        // HtmlDocument 实例的引用
        private System.Windows.Forms.HtmlDocument sysWinFormHtmlDoc;

        // IHTMLDocument2 接口的引用
        private mshtml.IHTMLDocument2 msHtmlDocInterface;

        /// <summary>
        /// 用于封装 mshtml.IHTMLTxtRange 接口。
        /// 避免直接使用 mshtml 命名空间下的组件，使用时就无需添加对 MSHTML 库的引用。
        /// </summary>
        public class Range
        {
            internal Range(mshtml.IHTMLTxtRange range)
            {
                msHtmlTxRange = range;
            }

            /// <summary>
            /// 清楚选区内容
            /// </summary>
            public void Clear()
            {
                msHtmlTxRange.pasteHTML(string.Empty);
            }

            /// <summary>
            /// 复制当前选区
            /// </summary>
            public Range Duplicate()
            {
                return new Range(msHtmlTxRange.duplicate());
            }

            /// <summary>
            /// 在当前选区上执行命令
            /// </summary>
            /// <param name="commandID">命令名称</param>
            /// <param name="showUI">是否使用内置的对话框或消息框</param>
            /// <param name="value">命令参数</param>
            public void ExecuteCommand(string commandID, bool showUI, object value)
            {
                msHtmlTxRange.execCommand(commandID, showUI, value);
            }

            /// <summary>
            /// 判断当前选区是否在另一个选区之内
            /// </summary>
            public bool InRange(Range range)
            {
                return msHtmlTxRange.inRange(range.msHtmlTxRange);
            }

            /// <summary>
            /// 判断当前选区是否与另一个选区相同
            /// </summary>
            public bool IsEqual(Range range)
            {
                return msHtmlTxRange.isEqual(range.msHtmlTxRange);
            }

            /// <summary>
            /// 替换将当前选区的内容
            /// </summary>
            public void Replace(string content)
            {
                msHtmlTxRange.pasteHTML(content);
            }

            /// <summary>
            /// 查询命令是否可执行
            /// </summary>
            /// <param name="commandID">命令名称</param>
            public bool QueryCommandEnabled(string commandID)
            {
                return msHtmlTxRange.queryCommandEnabled(commandID);
            }

            public bool QueryCommandIndeterm(string commandID)
            {
                return msHtmlTxRange.queryCommandIndeterm(commandID);
            }

            /// <summary>
            /// 查询命令状态
            /// </summary>
            /// <param name="commandID">命令名称</param>
            public bool QueryCommandState(string commandID)
            {
                return msHtmlTxRange.queryCommandState(commandID);
            }

            /// <summary>
            /// 查询命令是否被支持
            /// </summary>
            /// <param name="commandID">命令名称</param>
            public bool QueryCommandSupported(string commandID)
            {
                return msHtmlTxRange.queryCommandSupported(commandID);
            }

            /// <summary>
            /// 查询命令文本
            /// </summary>
            /// <param name="commandID">命令名称</param>
            public string QueryCommandText(string commandID)
            {
                return msHtmlTxRange.queryCommandText(commandID);
            }

            /// <summary>
            /// 查询命令值
            /// </summary>
            /// <param name="commandID">命令名称</param>
            public object QueryCommandValue(string commandID)
            {
                return msHtmlTxRange.queryCommandValue(commandID);
            }

            /// <summary>
            /// 当前选区是否为空
            /// </summary>
            public bool IsEmpty
            {
                get { return string.IsNullOrEmpty(msHtmlTxRange.text); }
            }

            /// <summary>
            /// 获取当前选区中的HTML内容
            /// </summary>
            public string Content
            {
                get { return msHtmlTxRange.htmlText; }
            }

            /// <summary>
            /// 获取当前选区中的文本内容
            /// </summary>
            public string Text
            {
                get { return msHtmlTxRange.text; }
            }

            // IHTMLTxtRange 接口的引用
            private mshtml.IHTMLTxtRange msHtmlTxRange;
        }
    }
}
