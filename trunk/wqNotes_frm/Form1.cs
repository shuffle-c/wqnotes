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
        //==================================
        Journal MainJrn;
        wqFile mDB;
        //==================================

        public bool TryClose()
        {
            String FilePath = "";
            if (mDB.FileState == wqFile.wqFileState.wqNew ||
                mDB.FileState == wqFile.wqFileState.wqOpened
                && MainJrn.IsChanged == true)
            {
                DialogResult iDr;
                List<Int32> ires = null;
                Boolean isl = false;
                if ((Properties.Settings.Default.SaveMode == 1 ||
                    Properties.Settings.Default.SaveMode == 3) &&
                    mDB.NodeList.Count > 0)
                {
                    Form6 ifrm = new Form6();
                    foreach (Int32 u in mDB.NodeList)
                    {
                        ifrm.wqInput.Add(u, treeView1.Nodes.
                            Find(u.ToString(), true)[0].FullPath);
                    }
                    iDr = ifrm.ShowDialog();
                    ires = ifrm.wqResult;
                    isl = true;
                }
                else
                    iDr = MessageBox.Show("��������� ������?", "wqNotes",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                switch (iDr)
                {
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.Yes:
                        if (mDB.FileState == wqFile.wqFileState.wqOpened)
                            FilePath = mDB.FileName;
                        else
                        {
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                                FilePath = saveFileDialog1.FileName;
                            else
                            {
                                if (mDB.FileState == wqFile.wqFileState.wqNew)
                                    mDB.Delete();
                                mDB = new wqFile();
                                break;
                            }
                        }
                        try
                        {
                            if (isl == true)
                            {
                                foreach (Int32 u in ires)
                                    MainJrn.RemoveChange(u);
                            }
                            MainJrn.SaveDB(FilePath);
                            if (mDB.FileState == wqFile.wqFileState.wqNew)
                                mDB.Delete();
                            mDB = new wqFile();
                            mDB.FileName = FilePath;
                            mDB.FileState = wqFile.wqFileState.wqOpened;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "������!");
                        }
                        break;
                    case DialogResult.No:
                        if (mDB.FileState == wqFile.wqFileState.wqNew)
                            mDB.Delete();
                        mDB = new wqFile();
                        break;
                }
            }
            return true;
        }

        public void DeleteElem(TreeNode tnow, bool iLast)
        {
            NodeInfoTag nit = (NodeInfoTag)tnow.Tag;
            if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
            {
                //������!!! ����� �� ������ ��� ������!
                MainJrn.DeleteNode(nit.wqId);
                if (mDB.NodeList.Contains(nit.wqId))
                    mDB.NodeList.Remove(nit.wqId);
            }
            else
            {
                foreach (TreeNode tn in tnow.Nodes)
                    DeleteElem(tn, false);
                MainJrn.DeleteDir(nit.wqId);
            }
            if (iLast == true)
            {
                this.RefreshTop(tnow);
                TreeNode tn = tnow.Parent;
                if (tnow.PrevNode != null) tn = tnow.PrevNode;
                if (tnow.NextNode != null) tn = tnow.NextNode;
                tnow.Remove();
                treeView1.SelectedNode = tn;
            }
        }

        public TreeNode FullTreeNode(NodeInfoTag mID)
        {
            TreeNode ret = new TreeNode();
            ret.Name = mID.wqId.ToString();
            ret.Text = mID.wqName;
            if (mID.wqType == NodeInfoTag.wqTypes.wqDir)
                ret.Text += " (" + mID.wqAddInfo + ")";
            else
            {
                ret.ImageIndex = mID.wqAddInfo + 2;
                ret.SelectedImageIndex = mID.wqAddInfo + 2;

            }
            ret.NodeFont = new Font(
                treeView1.Font, mID.wqPriority == 0 ? FontStyle.Regular :
                mID.wqPriority == 1 ? FontStyle.Bold : FontStyle.Italic);
            ret.Tag = mID;
            return ret;
        }

        public void RefreshTop(TreeNode child)
        {
            this.RefreshTop(child, -1);
        }

        public void RefreshTop(TreeNode child, Int32 c)
        {
            TreeNode par = child.Parent, tn = null;
            NodeInfoTag res = null; Int32 C = c;
            treeView1.BeginUpdate();
            while (par != null)
            {
                if (c-- == 0 && C != -1) break;
                res = (NodeInfoTag)par.Tag;
                tn = FullTreeNode(MainJrn.GetInfoElem(res.wqId, res.wqType));
                par.Text = tn.Text;
                par.Tag = tn.Tag;
                par.ForeColor = tn.ForeColor; // �� �������
                par.ToolTipText = tn.ToolTipText;
                par = par.Parent;
            }
            treeView1.EndUpdate();
            this.SetLabelStatus();
        }

        public void SetNodeSchema(Int32 num)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (MainJrn.SetSchema(nit.wqId, num))
            {
                nit.wqAddInfo = num;
                treeView1.SelectedNode.Tag = nit;
                treeView1.SelectedNode.ImageIndex = 2 + num;
                treeView1.SelectedNode.SelectedImageIndex = 2 + num;
            }
        }

        public void SetElemPriority(Int32 num)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (MainJrn.SetPriority(nit.wqId, nit.wqType, num))
            {
                nit.wqPriority = num;
                treeView1.SelectedNode.Tag = nit;
                treeView1.SelectedNode.NodeFont = new Font(
                    treeView1.Font, num == 0 ? FontStyle.Regular :
                    num == 1 ? FontStyle.Bold : FontStyle.Italic);
            }
        }

        public string GetShortSize(Int32 qw) //UInt64
        {
            if (qw < 1024) return qw.ToString() + " ����";
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
            String[] use = { "��", "��", "��", "��" };
            return ret + " " + use[c];
        }

        public void SetLabelStatus()
        {
            if (mDB.FileState == wqFile.wqFileState.wqNone)
                toolStripStatusLabel1.Text = "������ �� ������";
            else
            {
                NodeInfoTag nit = (NodeInfoTag)treeView1.Nodes["1"].Tag;
                String res = " (" + nit.wqAddInfo + " �������, " +
                    GetShortSize(nit.wqSize) + ")";
                if (mDB.FileState == wqFile.wqFileState.wqNew)
                    toolStripStatusLabel1.Text = "����� ������";
                else toolStripStatusLabel1.Text = mDB.FileName;
                toolStripStatusLabel1.Text += res;
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                NodeInfoTag np = null;
                if (treeView1.SelectedNode.Parent == null)
                    ��������������ToolStripMenuItem.Enabled = false;
                else ��������������ToolStripMenuItem.Enabled = true;
                if (treeView1.SelectedNode.PrevNode == null)
                    ����������������ToolStripMenuItem.Enabled = false;
                else
                {
                    ����������������ToolStripMenuItem.Enabled = true;
                    np = (NodeInfoTag)treeView1.SelectedNode.PrevNode.Tag;
                }
                if (treeView1.SelectedNode.NextNode == null)
                    ���������������ToolStripMenuItem.Enabled = false;
                else ���������������ToolStripMenuItem.Enabled = true;
                if (np == null || np.wqType != NodeInfoTag.wqTypes.wqDir)
                    �������������ToolStripMenuItem.Enabled = false;
                else �������������ToolStripMenuItem.Enabled = true;

                NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
                if (nit.wqParent_id == 1)
                    ��������������ToolStripMenuItem.Enabled = false;
                �������ToolStripMenuItem3.Visible = true;
                ����������ToolStripMenuItem.Visible = true;
                ��������������ToolStripMenuItem.Visible = true;
                �������������ToolStripMenuItem.Visible = true;
                ����������������ToolStripMenuItem.Visible = true;
                ���������������ToolStripMenuItem.Visible = true;
                if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
                {
                    ��������ToolStripMenuItem1.Visible = false;
                    ����������ToolStripMenuItem1.Visible = false;
                    �����������������ToolStripMenuItem.Visible = false;
                    �����ToolStripMenuItem.Visible = false;
                    �������ToolStripMenuItem.Visible = true;
                    ����������ToolStripMenuItem.Visible = true;
                    if (nit.wqId == 1)
                    {
                        �������ToolStripMenuItem3.Visible = false;
                        ����������ToolStripMenuItem.Visible = false;
                        ��������������ToolStripMenuItem.Visible = false;
                        �������������ToolStripMenuItem.Visible = false;
                        ����������������ToolStripMenuItem.Visible = false;
                        ���������������ToolStripMenuItem.Visible = false;
                    }
                }
                else
                {
                    ��������ToolStripMenuItem1.Visible = true;
                    ����������ToolStripMenuItem1.Visible = true;
                    �����������������ToolStripMenuItem.Visible = true;
                    �����ToolStripMenuItem.Visible = true;
                    �������ToolStripMenuItem.Visible = false;
                    ����������ToolStripMenuItem.Visible = false;

                    �������ToolStripMenuItem1.Checked = false;
                    ���ToolStripMenuItem1.Checked = false;
                    ������ToolStripMenuItem.Checked = false;
                    ���ToolStripMenuItem.Checked = false;
                    ������ToolStripMenuItem.Checked = false;
                    �����ToolStripMenuItem.Checked = false;
                    switch (nit.wqAddInfo)
                    {
                        case 0: �������ToolStripMenuItem1.Checked = true; break;
                        case 1: ���ToolStripMenuItem1.Checked = true; break;
                        case 2: ������ToolStripMenuItem.Checked = true; break;
                        case 3: ���ToolStripMenuItem.Checked = true; break;
                        case 4: ������ToolStripMenuItem.Checked = true; break;
                        case 5: �����ToolStripMenuItem.Checked = true; break;
                    }
                }
                �������ToolStripMenuItem.Checked = false;
                �������ToolStripMenuItem.Checked = false;
                ������ToolStripMenuItem.Checked = false;
                switch (nit.wqPriority)
                {
                    case 0: �������ToolStripMenuItem.Checked = true; break;
                    case 1: �������ToolStripMenuItem.Checked = true; break;
                    case 2: ������ToolStripMenuItem.Checked = true; break;
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
            Application.Idle += new EventHandler(Application_Idle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // �������������
            treeView1.ExpandAll();
            toolStripComboBox1.SelectedIndex = 0;
            treeView1.Nodes.Clear();
            label10.Text = "<�� ������ �������>";
            ////////////////////////////
            mDB = new wqFile();
            mDB.NodeChange = false;

            // ��������� ��������
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Title = "��������� ������";
            saveFileDialog1.Filter = "������ wqNotes (*.wqn)|*.wqn|��� �����|*.*";
            saveFileDialog1.FilterIndex = 0;

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Title = "������� ������";
            openFileDialog1.Filter = "������ wqNotes (*.wqn)|*.wqn|��� �����|*.*";
            openFileDialog1.FilterIndex = 0;

            this.Location = Properties.Settings.Default.Location;
            this.Size = Properties.Settings.Default.Size;
            this.WindowState = Properties.Settings.Default.WinState;
            if (Properties.Settings.Default.LoadLastFile == true)
                mDB.FileName = Properties.Settings.Default.LastFile;
            string RecentFiles = Properties.Settings.Default.RecentFiles;

            openFileDialog1.FileName = mDB.FileName;
            //
            //1. ���������� ������ �����
            //2. ��������� ������ ���������
            //

            if (mDB.FileName.Length == 0) mDB.FileState = wqFile.wqFileState.wqNone;
            else mDB.FileState = wqFile.wqFileState.wqOpened;

            if (mDB.FileState == wqFile.wqFileState.wqOpened)
            {
                try
                {
                    MainJrn = new Journal(mDB.FileName, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    MainJrn = null;
                    mDB.FileState = wqFile.wqFileState.wqNone;
                    mDB.FileName = "";
                }
            }
            this.SetLabelStatus();
            //
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

        private void wqRichEdit1_SelectionChanged(object sender, EventArgs e)
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
            if (!TryClose()) return;
            mDB.FileName = System.IO.Path.GetTempFileName();
            mDB.FileState = wqFile.wqFileState.wqNew;
            MainJrn = new Journal(mDB.FileName, true);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
            treeView1.ExpandAll();
            this.SetLabelStatus();
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!TryClose()) return;
                treeView1.Nodes.Clear();
                try
                {
                    String res = openFileDialog1.FileName;
                    MainJrn = new Journal(res, false);
                    mDB.FileName = res;
                    mDB.FileState = wqFile.wqFileState.wqOpened;
                    treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
                    treeView1.ExpandAll();
                }
                catch
                {
                    mDB = new wqFile();
                }
                this.SetLabelStatus();
            }
        }

        private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mDB.FileState == wqFile.wqFileState.wqNew)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mDB.FileName = saveFileDialog1.FileName;
                    mDB.FileState = wqFile.wqFileState.wqOpened;
                }
                else
                    return;
            }
            if (MainJrn.IsChanged == false) return;
            try
            {
                String res = mDB.FileName;
                MainJrn.SaveDB(mDB.FileName);
                mDB = new wqFile();
                mDB.FileName = res;
                mDB.FileState = wqFile.wqFileState.wqOpened;

                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
                treeView1.ExpandAll();
            }
            catch
            {
                MessageBox.Show("������!");
            }
            this.SetLabelStatus();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            //if (Properties.Settings.Default.SaveMode != 0 &&
            //    Properties.Settings.Default.SaveMode != 1)
            //{
            //    //
            //}
            //else
            //{
            //    if(nit.wqType == NodeInfoTag.wqTypes.wqDir) {
            //        // ������ ���, �������� ���������� � �����
            //        if(mDB.NowNode != null && !mDB.NodeChange) {
            //            /* -----------------------------------------
            //             * // ������ ��� ������� �� ����
            //             * wqRichEdit1.wqRemove(mDB.NowNode.wqId);
            //             * -----------------------------------------
            //            */
            //        }
            //    } else {
            //        if(mDB.NowNode != null && !mDB.NodeChange

            //        ////��������� ���� ������?
            //        //if(mDB.NodeListPos.Count == 0) {
            //        //    //wqRichEdit1.Text = MainJrn.GetNode(nit.wqId);
            //        //    /* -----------------------------------------
            //        //     * // �������� ������� � ���
            //}

            //����� ���������� ����� �������, �� ������ ��� ClearUndo!
            //��������. ����������.
            if (mDB.NowNode != null && mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
            {
                if (mDB.NodeChange == true)
                {
                    MainJrn.SetNodeContent(mDB.NowNode.wqId, wqRichEdit1.Text);
                    mDB.NodeChange = false;
                    if (!mDB.NodeList.Contains(mDB.NowNode.wqId))
                        mDB.NodeList.Add(mDB.NowNode.wqId);
                }
            }
            mDB.NowNode = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
            {
                wqRichEdit1.Text = MainJrn.GetNode(mDB.NowNode.wqId);
                mDB.NodeChange = false;
                label10.Text = mDB.NowNode.wqName;
            }
            else label10.Text = "[" + mDB.NowNode.wqName + "]";
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            String name = "����� ����� #" + Properties.Settings.
                Default.LastNumberElem.ToString();
            Properties.Settings.Default.LastNumberElem++;
            TreeNode res = FullTreeNode(MainJrn.CreateDir(nit.wqId, name));
            treeView1.SelectedNode.Nodes.Add(res);
            this.RefreshTop(res, 1);
            treeView1.SelectedNode.ExpandAll();
            res.Text = name;
            treeView1.LabelEdit = true;
            res.BeginEdit();
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            String name = "����� ������� #" + Properties.Settings.
                Default.LastNumberElem.ToString();
            Properties.Settings.Default.LastNumberElem++;
            TreeNode res = FullTreeNode(MainJrn.CreateNode(nit.wqId, name));
            treeView1.SelectedNode.Nodes.Add(res);
            //mDB.NodeList.Add(Int32.Parse(res.Name));
            this.RefreshTop(res);
            treeView1.SelectedNode.ExpandAll();
            treeView1.LabelEdit = true;
            res.BeginEdit();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)e.Node.Tag;
            if (e.Label == null || e.Label.Contains("/") || e.Label.Contains("\\")
                || e.Label == nit.wqName)
            {
                e.CancelEdit = true;
                treeView1.SelectedNode = e.Node;
                treeView1.LabelEdit = false;
                if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
                    e.Node.Text += " (" + nit.wqAddInfo + ")";
                return;
            }
            MainJrn.Rename(nit, e.Label);
            e.Node.Tag = MainJrn.GetInfoElem(nit.wqId, nit.wqType);
            this.RefreshTop(e.Node, 1);
            treeView1.SelectedNode = e.Node;
            treeView1.LabelEdit = false;
            if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
            {
                e.Node.Text = e.Label + " (" + nit.wqAddInfo + ")";
                e.CancelEdit = true;
            }
        }

        private void wqRichEdit1_TextChanged(object sender, EventArgs e)
        {
            mDB.NodeChange = true;
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //wqRichEdit1.Paste(new DataFormats.Format("text", 0));
            wqRichEdit1.Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!TryClose()) e.Cancel = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //��������� ��������� ������
            Properties.Settings.Default.WinState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Location = this.Location;
                Properties.Settings.Default.Size = this.Size;
            }
            else
            {
                Properties.Settings.Default.Location = this.RestoreBounds.Location;
                Properties.Settings.Default.Size = this.RestoreBounds.Size;
            }
            Properties.Settings.Default.Save();
        }

        private void �������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            treeView1.SelectedNode.Text = nit.wqName;
            treeView1.LabelEdit = true;
            treeView1.SelectedNode.BeginEdit();
        }

        private void �������ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.SetNodeSchema(0);
        }

        private void ���ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.SetNodeSchema(1);
        }

        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetNodeSchema(2);
        }

        private void ���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetNodeSchema(3);
        }

        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetNodeSchema(4);
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetNodeSchema(5);
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetElemPriority(1);
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetElemPriority(0);
        }

        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetElemPriority(2);
        }

        private void �������ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            string res = "�� �������, ��� ������ ������� ";
            if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
                res += "����� \"" + nit.wqName + "\" � ��� �� ����������?";
            else res += "������� \"" + nit.wqName + "\"?";
            if (MessageBox.Show(res, "wqNotes", MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                == DialogResult.Yes)
            {
                this.DeleteElem(treeView1.SelectedNode, true);
            }
        }

        private void ������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mDB.FileName = saveFileDialog1.FileName;
                mDB.FileState = wqFile.wqFileState.wqOpened;
                MainJrn.IsChanged = true;
                ���������ToolStripMenuItem_Click(sender, e);
            }
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
            {
                if (MessageBox.Show("�� �������, ��� ������ �������� "
                    + "���������� ������� \"" + nit.wqName + "\"?", "wqNotes",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    wqRichEdit1.Text = "";
                }
            }
            else
            {
                if (MessageBox.Show("�� �������, ��� ������ ������� ��� "
                    + "���������� ����� \"" + nit.wqName + "\"?", "wqNotes",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    TreeNode tnow = treeView1.SelectedNode;
                    treeView1.BeginUpdate();
                    while (tnow.Nodes.Count > 0)
                        this.DeleteElem(tnow.Nodes[0], true);
                    treeView1.EndUpdate();
                }
            }
        }

        private void ��������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnow = treeView1.SelectedNode;
            TreeNode tpar = tnow.Parent;
            NodeInfoTag nit = (NodeInfoTag)tnow.Tag;
            MainJrn.BringUp(nit);
            RefreshTop(tnow);
            tnow.Remove();
            tpar.Parent.Nodes.Insert(tpar.Index + 1, tnow);
            treeView1.SelectedNode = tnow;
        }

        private void �������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnow = treeView1.SelectedNode;
            TreeNode tpar = tnow.PrevNode;
            NodeInfoTag n1 = (NodeInfoTag)tpar.Tag;
            NodeInfoTag n2 = (NodeInfoTag)tnow.Tag;
            MainJrn.BringDown(n2, n1);
            tnow.Remove();
            tpar.Nodes.Add(tnow);
            RefreshTop(tnow);
            treeView1.SelectedNode = tnow;
        }

        private void ����������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnow = treeView1.SelectedNode;
            TreeNode tpar = tnow.Parent;
            Int32 inx = tnow.PrevNode.Index;
            tnow.Remove();
            tpar.Nodes.Insert(inx, tnow);
            treeView1.SelectedNode = tnow;
        }

        private void ���������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnow = treeView1.SelectedNode;
            TreeNode tpar = tnow.Parent;
            Int32 inx = tnow.NextNode.Index;
            tnow.Remove();
            tpar.Nodes.Insert(inx, tnow);
            treeView1.SelectedNode = tnow;
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnow = treeView1.SelectedNode;
            treeView1.BeginUpdate();
            while (tnow.Nodes.Count > 0)
            {
                treeView1.SelectedNode = tnow.Nodes[0];
                ��������������ToolStripMenuItem_Click(sender, e);
            }
            treeView1.SelectedNode = tnow;
            treeView1.EndUpdate();
        }

        private void wqRichEdit1_wqNodeChanged(object sender, wqNodeEventArgs e)
        {
            //MessageBox.Show("Xa!");
        }

        private void wqRichEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            toolStripStatusLabel4.Text = Control.IsKeyLocked(Keys.Insert) ? "INS" : "OVR";
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            toolStripStatusLabel3.ForeColor = Control.IsKeyLocked(Keys.CapsLock) ? Color.Black : Color.Gray;
        }

        private void Form1_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            toolStripStatusLabel5.Text = e.Culture.TwoLetterISOLanguageName.ToUpper();
            toolStripStatusLabel5.ToolTipText = e.Culture.NativeName;
        }
    }
}