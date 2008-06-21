using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace wqNotes_frm
{
    class wqRichEdit : RichTextBox
    {
        private static IntPtr RichEditModuleHandle;
        private const string RichEditDllV3 = "RichEd20.dll";
        private const string RichEditDllV41 = "Msftedit.dll";

        private const string RichEditClassV3A = "RichEdit20A";
        private const string RichEditClassV3W = "RichEdit20W";
        private const string RichEditClassV41W = "RICHEDIT50W";

        protected override CreateParams CreateParams
        {
            [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, UnmanagedCode = true)]
            get
            {
                if (RichEditModuleHandle == IntPtr.Zero)
                {
                    //попытаемс€ загрузить реализацию RichEdit v4.1 (Msftedit.dll, WinXP + SP1) 
                    RichEditModuleHandle = Program.LoadLibrary(RichEditDllV41);
                    if (RichEditModuleHandle == IntPtr.Zero)
                    {
                        //нет такой dll, используем стандартную реализацию (Riched20.dll) 
                        return base.CreateParams;
                    }
                }

                //используем более новую реализацию richedit'а 
                CreateParams theParams = base.CreateParams;
                theParams.ClassName = RichEditClassV41W;
                return theParams;
            }
        }

        //‘ункции работы с таблицами и т.д. и парсинг заметок
        //================================================================
        
        /// <summary>
        /// First - id
        /// Second - size (in lines)
        /// </summary>
        private Pair<Int32, Int32> NowNode = null;

        public Dictionary<Int32, Int32> wqNodes = new Dictionary<Int32, Int32>();

        private Int32 SelectionLine
        {
            set
            {
                Int32 f = 0;
                for (Int32 i = 0; i < value; ++i)
                    f += this.Lines[i].Length;
                this.SelectionStart = f + value;
            }
            get
            {
                Int32 t = this.SelectionStart, a = -1, x = -1, ret = 0;
                while ((a = this.Find(new char[] { '\r' }, a + 1, t)) != -1)
                {
                    if (a <= x) break;
                    x = Math.Max(a, x);
                    ret++;
                }
                return ret;
            }
        }
        private Int32 NodeOfLine(Int32 line)
        {
            return line;
        }
        private void RemoveNonChangedNode()
        {
            //
        }
        
        private string wqRtfToText(string rtf)
        {
            return rtf;
        }
        private string wqTextToRtf(string text)
        {
            return text;
        }

        public void wqAdd(Int32 id, string text)
        {
            this.RemoveNonChangedNode();
            if (this.wqNodes.Count == 0)
            {
                this.Text = this.wqTextToRtf(text);
                this.wqNodes.Add(id, this.Lines.Length);
                // ƒл€ получени€ количества строк в измененной заметке,
                // мы вычисл€ем разницу количества строк всего рича до и
                // после внесени€ изменений в заметку.
            }
            else
            {
                //
            }
        }
        public void wqRemove(Int32 id)
        {
            //
        }
        public void wqClear()
        {
            this.wqNodes = new Dictionary<Int32, Int32>();
            this.Text = "";
        }
        public string wqTextOfNode(Int32 id)
        {
            return "";
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            //
            // “екст неактивных заметок блокируетс€ (вместе с разделител€ми)
            // и курсор над ними обычного стил€ (не italic). –азблокировать
            // можно двойным щелчком, тем самым происходит переход к другой
            // заметке и вызов событи€ wqNodeSwitched.
            if (this.Text == "test")
            {
                wqNodeChanged(this, new wqNodeEventArgs());
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //ћен€ть курсор
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            //–азблокировать заметку и перейти к ней
        }

        public delegate void wqNodesEventHandler(object sender, wqNodeEventArgs e);
        public event wqNodesEventHandler wqNodeChanged;
        public event wqNodesEventHandler wqNodeSwitched;
    }

    public class wqNodeEventArgs : EventArgs
    {
        public Int32 wqNodeId;
    }
}
