using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace wqNotes_frm
{
    class wqRichEdit : RichTextBox
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string libname);

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
                    //попытаемся загрузить реализацию RichEdit v4.1 (Msftedit.dll, WinXP + SP1) 
                    RichEditModuleHandle = LoadLibrary(RichEditDllV41);
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

        //Функции работы с таблицами и т.д. и парсинг заметок
    }
}
