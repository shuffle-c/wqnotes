/*
* Copyright (c) 2007-2008 wqNotes Project
* License: BSD
* Windows: FindForm.cs, $Revision$
* $HeadURL$
* $Date$
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
   public partial class Form4 : Form
   {
      public delegate void StartSearch();
      public StartSearch pStartSearch;

      public DateTime DateFrom = DateTime.MinValue, DateTo = DateTime.MaxValue;
      public int SizeFrom = int.MinValue, SizeTo = int.MaxValue;

      public Form4()
      {
         InitializeComponent();
      }

      private void button2_Click(object sender, EventArgs e)
      {
         Form.ActiveForm.Close();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         if (comboBox5.Text == "") return;
         if (checkBox3.Checked)
            DateFrom = dateTimePicker1.Value;
         if (checkBox4.Checked)
            DateTo = dateTimePicker2.Value;
         if (checkBox6.Checked && comboBox2.Text != "")
         {
            DateTo = DateTime.Now;
            DateFrom = DateTime.Now;
            switch (comboBox2.SelectedItem.ToString())
            {
               case "часов":
                  DateFrom -= new TimeSpan(0, (int)numericUpDown1.Value, 0, 0);
                  break;
               case "суток":
                  DateFrom -= new TimeSpan((int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "недель":
                  DateFrom -= new TimeSpan(7 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "мес€цев":
                  DateFrom -= new TimeSpan(30 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "лет":
                  DateFrom -= new TimeSpan(365 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
            }
         }
         if (checkBox5.Checked && comboBox3.Text != "")
         {
            switch (comboBox3.SelectedItem.ToString())
            {
               case "байт": SizeFrom = (int)numericUpDown2.Value; break;
               case " байт": SizeFrom = (int)numericUpDown2.Value * 1024; break;
               case "ћбайт": SizeFrom = (int)numericUpDown2.Value * 1024 * 1004; break;
            }
         }
         if (checkBox7.Checked && comboBox4.Text != "")
         {
            switch (comboBox4.SelectedItem.ToString())
            {
               case "байт": SizeTo = (int)numericUpDown3.Value; break;
               case " байт": SizeTo = (int)numericUpDown3.Value * 1024; break;
               case "ћбайт": SizeTo = (int)numericUpDown3.Value * 1024 * 1004; break;
            }
         }
         pStartSearch();
      }

      private void checkBox6_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox6.Checked)
         {
            checkBox3.Checked = !checkBox6.Checked;
            checkBox4.Checked = !checkBox6.Checked;
         }
      }

      private void checkBox3_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox3.Checked) checkBox6.Checked = false;
      }

      private void checkBox4_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox4.Checked) checkBox6.Checked = false;
      }

      private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
      {
         checkBox3.Checked = true;
      }

      private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
      {
         checkBox4.Checked = true;
      }

      private void numericUpDown1_ValueChanged(object sender, EventArgs e)
      {
         checkBox6.Checked = true;
      }

      private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
      {
         checkBox6.Checked = true;
      }

      private void numericUpDown2_ValueChanged(object sender, EventArgs e)
      {
         checkBox5.Checked = true;
      }

      private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
      {
         checkBox5.Checked = true;
      }

      private void numericUpDown3_ValueChanged(object sender, EventArgs e)
      {
         checkBox7.Checked = true;
      }

      private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
      {
         checkBox7.Checked = true;
      }

      private void checkBox11_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox11.Checked)
         {
            checkBox1.Checked = !checkBox11.Checked;
            checkBox2.Checked = !checkBox11.Checked;
            checkBox13.Checked = !checkBox11.Checked;
         }
      }

      private void checkBox13_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox13.Checked) checkBox11.Checked = false;
      }

      private void checkBox1_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox1.Checked) checkBox11.Checked = false;
      }

      private void checkBox2_CheckedChanged(object sender, EventArgs e)
      {
         if (checkBox2.Checked) checkBox11.Checked = false;
      }
   }
}