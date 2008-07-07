using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
   public partial class PosForm : Form
   {
      public wqRichEdit wqre;

      public PosForm()
      {
         InitializeComponent();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         wqre.SelectionStart = wqre.GetFirstCharIndexFromLine((int)
            numericUpDown2.Value - 1) + (int)numericUpDown1.Value - 1;
         this.Close();
      }

      private void PosForm_Activated(object sender, EventArgs e)
      {
         Decimal maxl = (Decimal)wqre.GetLineFromCharIndex(wqre.Text.Length) + 1;
         if (numericUpDown2.Value > maxl) numericUpDown2.Value = maxl;
         numericUpDown2.Maximum = maxl;
         numericUpDown2_ValueChanged(sender, e);
      }

      private void numericUpDown2_ValueChanged(object sender, EventArgs e)
      {
         Int32 mmx = wqre.GetFirstCharIndexFromLine((int)numericUpDown2.Value);
         if (mmx < 0) mmx = wqre.TextLength;
         Decimal maxc = mmx - wqre.GetFirstCharIndexFromLine((int)numericUpDown2.Value - 1);
         if (maxc == 0) maxc++;
         if (numericUpDown1.Value > maxc) numericUpDown1.Value = maxc;
         numericUpDown1.Maximum = maxc;
      }
   }
}