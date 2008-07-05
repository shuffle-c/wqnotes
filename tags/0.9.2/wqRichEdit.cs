using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;

namespace wqNotes
{
   public class wqRichEdit : RichTextBox
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
               //попытаемся загрузить реализацию RichEdit v4.1 (Msftedit.dll, WinXP + SP1) 
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

      public void wqClear()
      {
         this.Text = "";
      }

      protected override void OnTextChanged(EventArgs e)
      {
         if (m_bPaint) base.OnTextChanged(e);
      }

      private SyntaxSettings m_settings = new SyntaxSettings();
      private static bool m_bPaint = true;
      private string m_strLine = "";
      private int m_nLineLength = 0;
      private int m_nLineStart = 0;
      private int m_nLineEnd = 0;
      private string m_strKeywords = "";
      private int m_nCurSelection = 0;

      public SyntaxSettings Settings
      {
         get { return m_settings; }
      }

      protected override void WndProc(ref System.Windows.Forms.Message m)
      {
         if (m.Msg == 0x00f)
         {
            if (m_bPaint)
               base.WndProc(ref m);
            else
               m.Result = IntPtr.Zero;
         }
         else
            base.WndProc(ref m);
      }

      private void ProcessLine()
      {
         int nPosition = SelectionStart;
         SelectionStart = m_nLineStart;
         SelectionLength = m_nLineLength;
         SelectionColor = Color.Black;

         ProcessRegex(m_strKeywords, Settings.KeywordColor);
         if (Settings.EnableIntegers)
            ProcessRegex("\\b(?:[0-9]*\\.)?[0-9]+\\b", Settings.IntegerColor);
         if (Settings.EnableStrings)
            ProcessRegex("\"[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\"", Settings.StringColor);
         if (Settings.EnableComments && !string.IsNullOrEmpty(Settings.Comment))
            ProcessRegex(Settings.Comment + ".*$", Settings.CommentColor);

         SelectionStart = nPosition;
         SelectionLength = 0;
         SelectionColor = Color.Black;

         m_nCurSelection = nPosition;
      }

      private void ProcessRegex(string strRegex, Color color)
      {
         Regex regKeywords = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
         Match regMatch;

         for (regMatch = regKeywords.Match(m_strLine); regMatch.Success; regMatch = regMatch.NextMatch())
         {
            int nStart = m_nLineStart + regMatch.Index;
            int nLenght = regMatch.Length;
            SelectionStart = nStart;
            SelectionLength = nLenght;
            SelectionColor = color;
         }
      }

      public void CompileKeywords()
      {
         for (int i = 0; i < Settings.Keywords.Count; i++)
         {
            string strKeyword = Settings.Keywords[i];

            if (i == Settings.Keywords.Count - 1)
               m_strKeywords += "\\b" + strKeyword + "\\b";
            else
               m_strKeywords += "\\b" + strKeyword + "\\b|";
         }
      }

      public void ProcessSelectedLines()
      {
         m_bPaint = false;

         int nStartPos = 0;
         int i = 0;
         int nOriginalPos = SelectionStart;
         int iend = GetLineFromCharIndex(SelectionStart + SelectionLength) + 1;
         i = GetLineFromCharIndex(SelectionStart);
         nStartPos = SelectionStart;
         m_nLineStart = SelectionStart;
         while (i < iend && i < Lines.Length)
         {
            m_strLine = Lines[i];
            m_nLineStart = nStartPos;
            m_nLineEnd = m_nLineStart + m_strLine.Length;

            ProcessLine();
            i++;

            nStartPos += m_strLine.Length + 1;
         }

         m_bPaint = true;
      }
   }

   public class SyntaxList
   {
      public List<string> m_rgList = new List<string>();
      public Color m_color = new Color();
   }

   public class SyntaxSettings
   {
      SyntaxList m_rgKeywords = new SyntaxList();
      string m_strComment = "";
      Color m_colorComment = Color.Green;
      Color m_colorString = Color.Gray;
      Color m_colorInteger = Color.Red;
      bool m_bEnableComments = true;
      bool m_bEnableIntegers = true;
      bool m_bEnableStrings = true;

      #region Свойства
      public List<string> Keywords
      {
         get { return m_rgKeywords.m_rgList; }
      }

      public Color KeywordColor
      {
         get { return m_rgKeywords.m_color; }
         set { m_rgKeywords.m_color = value; }
      }

      public string Comment
      {
         get { return m_strComment; }
         set { m_strComment = value; }
      }

      public Color CommentColor
      {
         get { return m_colorComment; }
         set { m_colorComment = value; }
      }

      public bool EnableComments
      {
         get { return m_bEnableComments; }
         set { m_bEnableComments = value; }
      }

      public bool EnableIntegers
      {
         get { return m_bEnableIntegers; }
         set { m_bEnableIntegers = value; }
      }

      public bool EnableStrings
      {
         get { return m_bEnableStrings; }
         set { m_bEnableStrings = value; }
      }

      public Color StringColor
      {
         get { return m_colorString; }
         set { m_colorString = value; }
      }

      public Color IntegerColor
      {
         get { return m_colorInteger; }
         set { m_colorInteger = value; }
      }
      #endregion
   }
}