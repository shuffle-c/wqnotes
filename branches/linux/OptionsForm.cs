using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
   public partial class OptionsForm : Form
   {
      Options NewOptions;

      public OptionsForm()
      {
         InitializeComponent();
      }

      private void OptionsForm_Load(object sender, EventArgs e)
      {
         NewOptions = Program.Opt.Clone();
         propertyGrid1.SelectedObject = NewOptions;
      }

      private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
      {
         propertyGrid1.Refresh();
      }

      private void button2_Click(object sender, EventArgs e)
      {
         Program.Opt = NewOptions;
      }
   }
}
