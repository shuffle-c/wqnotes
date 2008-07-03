using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
   public partial class AboutForm : Form
   {
      public AboutForm()
      {
         InitializeComponent();
         label3.Text = label3.Text.Replace("%ver%", Program.wqVersion);
      }

      private void pictureBox1_Click(object sender, EventArgs e)
      {
         System.Diagnostics.Process.Start("http://webmoney.ru");
      }

      private void pictureBox2_Click(object sender, EventArgs e)
      {
         System.Diagnostics.Process.Start("http://money.yandex.ru");
      }

      private void button1_Click(object sender, EventArgs e)
      {
         ActiveForm.Close();
      }

      private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         System.Diagnostics.Process.Start("mailto:sharp-c@yandex.ru");
      }

      private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         System.Diagnostics.Process.Start("mailto:shuffle@vbnet.ru");
      }

      private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         System.Diagnostics.Process.Start("http://code.google.com/p/wqnotes/");
      }
   }
}