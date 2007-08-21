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
        Form4 findwindow; //òàê íàäî
        Journal MainJrn;
        bool NodeChange;
        NodeInfoTag NowNode;

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {

                if (treeView1.SelectedNode.Parent == null)
                    íàÓğîâåíüÂâåğõToolStripMenuItem.Enabled = false;
                else
                    íàÓğîâåíüÂâåğõToolStripMenuItem.Enabled = true;
                if (treeView1.SelectedNode.FirstNode == null)
                    íàÓğîâåíüÂíèçToolStripMenuItem.Enabled = false;
                else
                    íàÓğîâåíüÂíèçToolStripMenuItem.Enabled = true;

                NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
                if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
                {
                    âûğåçàòüToolStripMenuItem.Visible = false;
                    êîïèğîâàòüToolStripMenuItem.Visible = false;
                    äîáàâèòüÂÇàêëàäêèToolStripMenuItem.Visible = false;
                    ñòèëüToolStripMenuItem.Visible = false;
                    ñîçäàòüToolStripMenuItem.Visible = true;
                    èçâëå÷üÂñåToolStripMenuItem.Visible = true;
                }
                else
                {
                    âûğåçàòüToolStripMenuItem.Visible = true;
                    êîïèğîâàòüToolStripMenuItem.Visible = true;
                    äîáàâèòüÂÇàêëàäêèToolStripMenuItem.Visible = true;
                    ñòèëüToolStripMenuItem.Visible = true;
                    ñîçäàòüToolStripMenuItem.Visible = false;
                    èçâëå÷üÂñåToolStripMenuItem.Visible = false;
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

        private void îÏğîãğàììåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog();
        }


        // Ìåíş Âèä, ïîêàç è ñêğûòèå 6 îñíîâíûõ ıëåìåíòîâ èíòåğôåéñà

        private void ïàíåëüÈíñòğóìåíòîâToolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip1.Visible = ïàíåëüÈíñòğóìåíòîâToolStripMenuItem1.Checked;
        }

        private void ïàíåëüÍàâèãàöèèToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !ïàíåëüÍàâèãàöèèToolStripMenuItem.Checked;
        }

        private void çàãîëîâîêToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = !çàãîëîâîêToolStripMenuItem.Checked;
        }

        private void ğåçóëüòàòûÏîèñêàToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            çàìåòêàToolStripMenuItem.Enabled = ğåçóëüòàòûÏîèñêàToolStripMenuItem.Checked;
            splitContainer2.Panel1Collapsed = !ğåçóëüòàòûÏîèñêàToolStripMenuItem.Checked;
        }

        private void çàìåòêàToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Enabled = çàìåòêàToolStripMenuItem.Checked;
            if (çàìåòêàToolStripMenuItem.Checked)
            {
                ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Checked = (bool)ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Tag;
            }
            else
            {
                ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Tag = ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Checked;
                ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Checked = false;
            }
            ğåçóëüòàòûÏîèñêàToolStripMenuItem.Enabled = çàìåòêàToolStripMenuItem.Checked;
            splitContainer2.Panel2Collapsed = !çàìåòêàToolStripMenuItem.Checked;
        }


        private void ïğèñîåäèíåííûåÔàéëûToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip2.Visible = ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Checked;
        }

        private void ñòğîêàÑîñòîÿíèÿToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            statusStrip1.Visible = ñòğîêàÑîñòîÿíèÿToolStripMenuItem.Checked;
        }


        // Ìåíş Âèä - Áûñòğûå êîíôèãóğàöèè
        private void ïîëíàÿToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ïàíåëüÈíñòğóìåíòîâToolStripMenuItem1.Checked = true;
            ïàíåëüÍàâèãàöèèToolStripMenuItem.Checked = true;
            çàãîëîâîêToolStripMenuItem.Checked = true;
            ğåçóëüòàòûÏîèñêàToolStripMenuItem.Checked = true;
            çàìåòêàToolStripMenuItem.Checked = true;
            ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Checked = true;
            ñòğîêàÑîñòîÿíèÿToolStripMenuItem.Checked = true;
        }

        private void ìèíèìàëüíàÿToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ïàíåëüÈíñòğóìåíòîâToolStripMenuItem1.Checked = true;
            ïàíåëüÍàâèãàöèèToolStripMenuItem.Checked = false;
            çàãîëîâîêToolStripMenuItem.Checked = true;
            ğåçóëüòàòûÏîèñêàToolStripMenuItem.Checked = false;
            çàìåòêàToolStripMenuItem.Checked = true;
            ïğèñîåäèíåííûåÔàéëûToolStripMenuItem.Checked = false;
            ñòğîêàÑîñòîÿíèÿToolStripMenuItem.Checked = true;
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
            ïåğåéòèÊÏîçèöèèToolStripMenuItem_Click(sender, e);
        }

        private void ïåğåéòèÊÏîçèöèèToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.Show(Form1.ActiveForm);
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            ïîèñêToolStripMenuItem_Click(sender, e);
        }

        private void ïîèñêToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void íîâûéÆóğíàëToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //òåñò
            MainJrn = new Journal("default.xml", true);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(MainJrn.LoadTreeView());
            treeView1.ExpandAll();
        }

        private void îòêğûòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MainJrn = new Journal(openFileDialog1.FileName, false);
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(MainJrn.LoadTreeView());
                treeView1.ExpandAll();
            }
        }

        private void ñîõğàíèòüToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ïàïêóToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void çàìåòêóToolStripMenuItem_Click(object sender, EventArgs e)
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