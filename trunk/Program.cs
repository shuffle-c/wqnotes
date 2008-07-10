/*
* Copyright (c) 2007-2008 wqNotes Project
* License: BSD
* Windows: Program.cs, $Revision$
* URL: $HeadURL$
* $Date$
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace wqNotes
{
   static class Program
   {
      [DllImport("shell32.dll")]
      private static extern UInt32 ExtractIconEx(String lpszFile, Int32 nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, UInt32 nIcons);
      [DllImport("user32.dll")]
      private static extern Int32 DestroyIcon(IntPtr hIcon);
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern IntPtr LoadLibrary(String lpFileName);

      public static string GetShortSize(Int32 qw) //UInt64
      {
         if (qw < 1024) return qw.ToString() + " áàéò";
         Int32 c = 0, t = 1024 * 1000 - 1;
         while (qw > t) { qw /= 1024; c++; }
         Int32 dw = qw / 1024;
         StringBuilder ret = new StringBuilder(dw.ToString());
         if (ret.Length < 3)
         {
            Int32 uDec = (qw - dw * 1024) * 1000 / 1024;
            uDec /= 10; ret.Append(".");
            if (ret.Length == 3) uDec /= 10;
            ret.Append('0', 4 - ret.Length - uDec.ToString().Length);
            ret.Append(uDec.ToString());
         }
         String[] use = { "ÊÁ", "ÌÁ", "ÃÁ", "ÒÁ" };
         return ret + " " + use[c];
      }

      public static Image GetFileIcon(string FileName, bool IsIconContains, bool _16x16)
      {
         Image ret = null; Int32 DefIndex = 0;
         try
         {
            if (IsIconContains == false)
            {
               RegistryKey hKey = null;
               String skey = Path.GetExtension(FileName);
               if ((hKey = Registry.ClassesRoot.OpenSubKey(skey)) != null)
               {
                  skey = hKey.GetValue(null).ToString();
                  skey += "\\DefaultIcon"; hKey.Close();
                  if ((hKey = Registry.ClassesRoot.OpenSubKey(skey)) != null)
                  {
                     skey = hKey.GetValue(null).ToString();
                     String[] df = skey.Split(',');
                     hKey.Close();
                     if (df.Length == 2)
                     {
                        FileName = df[0];
                        Int32.TryParse(df[1], out DefIndex);
                     }
                  }
               }
            }
         }
         catch { }
         finally
         {
            IntPtr hSmall = IntPtr.Zero, hLarge = IntPtr.Zero;
            ExtractIconEx(FileName, DefIndex, ref hLarge, ref hSmall, 1);
            if (hSmall == IntPtr.Zero)
               ExtractIconEx("shell32.dll", 0, ref hLarge, ref hSmall, 1);
            ret = (Image)Icon.FromHandle(_16x16 ? hSmall : hLarge).ToBitmap();
            DestroyIcon(hLarge); DestroyIcon(hSmall);
         }
         return ret;
      }

      public static Image MergeImages(Image ImageBack, Image ImageFore)
      {
         Image ret = (Image)ImageBack.Clone();
         Graphics res = Graphics.FromImage(ret);
         res.DrawImageUnscaled(ImageBack, 0, 0);
         res.DrawImageUnscaled(ImageFore, 0, 0);
         res.Save();
         return ret;
      }

       public static string GetTextFromRtf(string rtf)
       {
          RichTextBox rtb = new RichTextBox();
          rtb.Rtf = rtf;
          return rtb.Text;
       }

       public static string GetRtfFromClipboard()
       {
          RichTextBox rtb = new RichTextBox(); rtb.Text = "";
          rtb.Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
          return rtb.Rtf;
       }

       public static string GetShorterPath(string path, int MaxN)
       {
          // TODO: Implement ýòî
          return path;
       }

       public static Options Opt = Options.Load();

       public const string wqVersion = "0.9.3 SVN";

       /// <summary>
       /// The main entry point for the application.
       /// </summary>
       [STAThread]
       static void Main()
       {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);
          Application.Run(new MainForm());
       }
    }

    public class CRC32
    {
       private UInt32[] crc32Table;
       private const int BUFFER_SIZE = 1024;

       public UInt32 GetCrc32(Stream stream)
       {
          unchecked
          {
             UInt32 crc32Result;
             crc32Result = 0xFFFFFFFF;
             byte[] buffer = new byte[BUFFER_SIZE];
             int readSize = BUFFER_SIZE;

             int count = stream.Read(buffer, 0, readSize);
             while (count > 0)
             {
                for (int i = 0; i < count; i++)
                {
                   crc32Result = ((crc32Result) >> 8) ^
                       crc32Table[(buffer[i]) ^ ((crc32Result) & 0x000000FF)];
                }
                count = stream.Read(buffer, 0, readSize);
             }
             return ~crc32Result;
          }
       }

       public CRC32()
       {
          unchecked
          {
             UInt32 dwPolynomial = 0xEDB88320, dwCrc;
             crc32Table = new UInt32[256];

             for (UInt32 i = 0; i < 256; i++)
             {
                dwCrc = i;
                for (UInt32 j = 8; j > 0; j--)
                {
                   if ((dwCrc & 1) == 1)
                      dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                   else
                      dwCrc >>= 1;
                }
                crc32Table[i] = dwCrc;
             }
          }
       }
    }
 }