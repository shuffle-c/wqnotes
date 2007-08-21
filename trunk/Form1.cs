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
        Journal MainJrn;
        bool NodeChange;
        NodeInfoTag NowNode;

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {

                if (treeView1.SelectedNode.Parent == null)
                    ��������������ToolStripMenuItem.Enabled = false;
                else
                    ��������������ToolStripMenuItem.Enabled = true;
                if (treeView1.SelectedNode.FirstNode == null)
                    �������������ToolStripMenuItem.Enabled = false;
                else
                    �������������ToolStripMenuItem.Enabled = true;

                NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
                if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
                {
                    ��������ToolStripMenuItem.Visible = false;
                    ����������ToolStripMenuItem.Visible = false;
                    �����������������ToolStripMenuItem.Visible = false;
                    �����ToolStripMenuItem.Visible = false;
                    �������ToolStripMenuItem.Visible = true;
                    ����������ToolStripMenuItem.Visible = true;
                }
                else
                {
                    ��������ToolStripMenuItem.Visible = true;
                    ����������ToolStripMenuItem.Visible = true;
                    �����������������ToolStripMenuItem.Visible = true;
                    �����ToolStripMenuItem.Visible = true;
                    �������ToolStripMenuItem.Visible = false;
                    ����������ToolStripMenuItem.Visible = false;
                }

            }
            else e.Cancel = true;
        }

        private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
            toolStripComboBox1.SelectedIndex = 0;
            treeView1.Nodes.Clear();
            NodeChange = false;
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
                wqRichEdit1.GetLineFromCharIndex(wqRichEdit1.SelectionStart).ToString() +
                " Pos " +
                (wqRichEdit1.GetCharIndexFromPosition(
                wqRichEdit1.GetPositionFromCharIndex(wqRichEdit1.SelectionStart)
                ) - wqRichEdit1.GetFirstCharIndexOfCurrentLine()).ToString();
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

        private void �����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //����
            MainJrn = new Journal("default.xml", true);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(MainJrn.LoadTreeView());
            treeView1.ExpandAll();
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MainJrn = new Journal(openFileDialog1.FileName, false);
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(MainJrn.LoadTreeView());
                treeView1.ExpandAll();
            }
        }

        private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MainJrn.SaveDB(saveFileDialog1.FileName);
            }
        }

        private void wqRichEdit1_TextChanged(object sender, EventArgs e)
        {
            NodeChange = true;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (NowNode != null && NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
            {
                if (NodeChange == true)
                {
                    MainJrn.SetNodeContent(NowNode.wqId, wqRichEdit1.Text);
                    NodeChange = false;
                }
            }
            NowNode = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
            {
                wqRichEdit1.Text = MainJrn.GetNode(NowNode.wqId);
                NodeChange = false;
            }
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            Form5 frm = new Form5();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                MainJrn.CreateDir(nit.wqId, frm.wqResult);

                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(MainJrn.LoadTreeView());
                treeView1.ExpandAll();
            }
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            Form5 frm = new Form5();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                MainJrn.CreateNode(nit.wqId, frm.wqResult);

                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(MainJrn.LoadTreeView());
                treeView1.ExpandAll();
            }
        }
    }
}