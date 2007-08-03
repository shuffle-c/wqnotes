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
       Form4 findwindow; //��� ����
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
            toolStripComboBox1.SelectedIndex = 0;
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }


        // ���� ���, ����� � ������� 6 �������� ��������� ����������

        private void ������������������ToolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip1.Visible = ������������������ToolStripMenuItem1.Checked;
        }

        private void ���������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !���������������ToolStripMenuItem.Checked;
        }

        private void ���������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = !���������ToolStripMenuItem.Checked;
        }

        private void ����������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            �������ToolStripMenuItem.Enabled = ����������������ToolStripMenuItem.Checked;
            splitContainer2.Panel1Collapsed = !����������������ToolStripMenuItem.Checked;
        }

        private void �������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            �������������������ToolStripMenuItem.Enabled = �������ToolStripMenuItem.Checked;
            if (�������ToolStripMenuItem.Checked)
            {
                �������������������ToolStripMenuItem.Checked = (bool)�������������������ToolStripMenuItem.Tag;
            }
            else
            {
                �������������������ToolStripMenuItem.Tag = �������������������ToolStripMenuItem.Checked;
                �������������������ToolStripMenuItem.Checked = false;
            }
            ����������������ToolStripMenuItem.Enabled = �������ToolStripMenuItem.Checked;
            splitContainer2.Panel2Collapsed = !�������ToolStripMenuItem.Checked;
        }


        private void �������������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip2.Visible = �������������������ToolStripMenuItem.Checked;
        }

        private void ���������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            statusStrip1.Visible = ���������������ToolStripMenuItem.Checked;
        }


        // ���� ��� - ������� ������������
        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ������������������ToolStripMenuItem1.Checked = true;
            ���������������ToolStripMenuItem.Checked = true;
            ���������ToolStripMenuItem.Checked = true;
            ����������������ToolStripMenuItem.Checked = true;
            �������ToolStripMenuItem.Checked = true;
            �������������������ToolStripMenuItem.Checked = true;
            ���������������ToolStripMenuItem.Checked = true;
        }

        private void �����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ������������������ToolStripMenuItem1.Checked = true;
            ���������������ToolStripMenuItem.Checked = false;
            ���������ToolStripMenuItem.Checked = true;
            ����������������ToolStripMenuItem.Checked = false;
            �������ToolStripMenuItem.Checked = true;
            �������������������ToolStripMenuItem.Checked = false;
            ���������������ToolStripMenuItem.Checked = true;
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
            ���������������ToolStripMenuItem_Click(sender, e);
        }

        private void ���������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.Show(Form1.ActiveForm);
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            �����ToolStripMenuItem_Click(sender, e);
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
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