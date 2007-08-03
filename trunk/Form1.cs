using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes_frm
{
    public partial class Form1 : Form
    {
       Form4 findwindow; //так надо
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
            toolStripComboBox1.SelectedIndex = 0;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }


        // Меню Вид, показ и скрытие 6 основных элементов интерфейса

        private void панельИнструментовToolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip1.Visible = панельИнструментовToolStripMenuItem1.Checked;
        }

        private void панельНавигацииToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !панельНавигацииToolStripMenuItem.Checked;
        }

        private void заголовокToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = !заголовокToolStripMenuItem.Checked;
        }

        private void результатыПоискаToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            заметкаToolStripMenuItem.Enabled = результатыПоискаToolStripMenuItem.Checked;
            splitContainer2.Panel1Collapsed = !результатыПоискаToolStripMenuItem.Checked;
        }

        private void заметкаToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            присоединенныеФайлыToolStripMenuItem.Enabled = заметкаToolStripMenuItem.Checked;
            if (заметкаToolStripMenuItem.Checked)
            {
                присоединенныеФайлыToolStripMenuItem.Checked = (bool)присоединенныеФайлыToolStripMenuItem.Tag;
            }
            else
            {
                присоединенныеФайлыToolStripMenuItem.Tag = присоединенныеФайлыToolStripMenuItem.Checked;
                присоединенныеФайлыToolStripMenuItem.Checked = false;
            }
            результатыПоискаToolStripMenuItem.Enabled = заметкаToolStripMenuItem.Checked;
            splitContainer2.Panel2Collapsed = !заметкаToolStripMenuItem.Checked;
        }


        private void присоединенныеФайлыToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip2.Visible = присоединенныеФайлыToolStripMenuItem.Checked;
        }

        private void строкаСостоянияToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            statusStrip1.Visible = строкаСостоянияToolStripMenuItem.Checked;
        }


        // Меню Вид - Быстрые конфигурации
        private void полнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            панельИнструментовToolStripMenuItem1.Checked = true;
            панельНавигацииToolStripMenuItem.Checked = true;
            заголовокToolStripMenuItem.Checked = true;
            результатыПоискаToolStripMenuItem.Checked = true;
            заметкаToolStripMenuItem.Checked = true;
            присоединенныеФайлыToolStripMenuItem.Checked = true;
            строкаСостоянияToolStripMenuItem.Checked = true;
        }

        private void минимальнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            панельИнструментовToolStripMenuItem1.Checked = true;
            панельНавигацииToolStripMenuItem.Checked = false;
            заголовокToolStripMenuItem.Checked = true;
            результатыПоискаToolStripMenuItem.Checked = false;
            заметкаToolStripMenuItem.Checked = true;
            присоединенныеФайлыToolStripMenuItem.Checked = false;
            строкаСостоянияToolStripMenuItem.Checked = true;
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "Ln " +
                richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart).ToString() +
                " Pos " +
                (richTextBox1.GetCharIndexFromPosition(
                richTextBox1.GetPositionFromCharIndex(richTextBox1.SelectionStart)
                ) - richTextBox1.GetFirstCharIndexOfCurrentLine()).ToString();
        }

        private void toolStripStatusLabel2_DoubleClick(object sender, EventArgs e)
        {
            перейтиКПозицииToolStripMenuItem_Click(sender, e);
        }

        private void перейтиКПозицииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.Show(Form1.ActiveForm);
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            поискToolStripMenuItem_Click(sender, e);
        }

        private void поискToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if (findwindow == null || findwindow.IsDisposed)
           {
              findwindow = new Form4();
              findwindow.Show(Form1.ActiveForm);
           }
           else
           {
              findwindow.Activate();
           }
        }
    }
}