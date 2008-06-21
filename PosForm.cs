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
            numericUpDown2.Value - 1) + (int)numericUpDown1.Value;
         this.Close();
      }

      private void PosForm_Activated(object sender, EventArgs e)
      {
         Decimal maxl = (Decimal)wqre.GetLineFromCharIndex(wqre.Text.Length);
         if (numericUpDown2.Value > maxl) numericUpDown2.Value = maxl;
         numericUpDown2.Maximum = maxl;
      }

      private void numericUpDown2_ValueChanged(object sender, EventArgs e)
      {
         Decimal maxc = wqre.GetFirstCharIndexFromLine((int)numericUpDown2.Value) -
            wqre.GetFirstCharIndexFromLine((int)numericUpDown2.Value - 1) - 1;
         if (maxc == 0) maxc++;
         if (numericUpDown1.Value > maxc) numericUpDown1.Value = maxc;
         numericUpDown1.Maximum = maxc;
      }
   }
}