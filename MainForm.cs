using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
   public partial class MainForm : Form
   {
      #region ����������

      Form4 findwindow;
      //==================================
      Journal MainJrn;
      wqFile mDB;
      //==================================
      #endregion

      #region ������� � ������

      public bool TryClose()
      {
         String FilePath = "";
         if (mDB.NodeChange) MainJrn.IsChanged = true;
         if (mDB.FileState == wqFile.wqFileState.wqNew ||
             mDB.FileState == wqFile.wqFileState.wqOpened
             && MainJrn.IsChanged == true)
         {
            DialogResult iDr;
            List<Int32> ires = null;
            Boolean isl = false;
            if ((false) && //xz
                mDB.NodeList.Count > 0)
            {
               SaveForm ifrm = new SaveForm();
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
                     this.DoProcess(true);
                     //Thread
                     MainJrn.SaveDB(FilePath);
                     this.DoProcess(false);

                     //MainJrn = new Journal(FilePath, false);
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

      public void DoProcess(bool start)
      {
         toolStripProgressBar1.Visible = start;
         toolStripStatusLabel2.Visible = !start;
         wqRichEdit1.Enabled = !start;
         treeView1.Enabled = !start;
      }

      public void DeleteElem(TreeNode tnow, bool iLast)
      {
         NodeInfoTag nit = (NodeInfoTag)tnow.Tag;
         if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            NodeInfoTag[] att = MainJrn.GetAttachList(nit.wqId);
            foreach (NodeInfoTag it in att)
               MainJrn.DeleteAttach(it.wqId);
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
            ret.Text += " (" + mID.wqCount + ")";
         else
         {
            ret.ImageIndex = mID.wqSchema + 2;
            ret.SelectedImageIndex = mID.wqSchema + 2;

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
            nit.wqSchema = num;
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

      public void SetLabelStatus()
      {
         if (mDB.FileState == wqFile.wqFileState.wqNone)
            toolStripStatusLabel1.Text = "������ �� ������";
         else
         {
            System.IO.FileInfo fi = new System.IO.FileInfo(mDB.FileName);
            NodeInfoTag nit = (NodeInfoTag)treeView1.Nodes["1"].Tag;
            String res = " (" + nit.wqCount + " �������, " +
                Program.GetShortSize((Int32)fi.Length) + ")";
            if (mDB.FileState == wqFile.wqFileState.wqNew)
               toolStripStatusLabel1.Text = "����� ������";
            else toolStripStatusLabel1.Text = mDB.FileName;
            toolStripStatusLabel1.Text += res;
         }
      }

      public void AddAttachInTool(NodeInfoTag nId, String FileName)
      {
         ToolStripSplitButton newf = new ToolStripSplitButton();
         ToolStripItem iOpen = new ToolStripMenuItem();
         iOpen.Click += new EventHandler(�������ToolStripMenuItem1_Click);
         iOpen.Text = "�������";
         iOpen.Font = new Font(iOpen.Font, iOpen.Font.Style ^ FontStyle.Bold);
         newf.DropDownItems.Add(iOpen);
         ToolStripItem iOpenIn = new ToolStripMenuItem();
         iOpenIn.Click += new EventHandler(��������ToolStripMenuItem_Click);
         iOpenIn.Text = "������� �...";
         newf.DropDownItems.Add(iOpenIn);
         ToolStripItem iSave = new ToolStripMenuItem();
         iSave.Click += new EventHandler(���������ToolStripMenuItem2_Click);
         iSave.Text = "���������";
         iSave.Visible = nId.wqFlag != "shortcut";
         newf.DropDownItems.Add(iSave);
         ToolStripItem iDelete = new ToolStripMenuItem();
         iDelete.Click += new EventHandler(�������ToolStripMenuItem1_Click);
         iDelete.Text = "�������";
         newf.DropDownItems.Add(iDelete);
         newf.DropDownItems.Add(new ToolStripSeparator());
         ToolStripItem iProperty = new ToolStripMenuItem();
         iProperty.Click += new EventHandler(��������ToolStripMenuItem2_Click);
         iProperty.Text = "��������";
         newf.DropDownItems.Add(iProperty);

         string res = nId.wqName;
         newf.Tag = nId;
         newf.ForeColor = nId.wqFlag == "shortcut" ? Color.Blue : Color.Black;
         newf.Text = System.IO.Path.GetFileName(nId.wqName) + " (" +
             Program.GetShortSize(Int32.Parse(nId.wqHash.Split(new
             string[] { "::" }, StringSplitOptions.None)[1])) + ")";
         newf.Image = Program.GetFileIcon(res, res.EndsWith(".exe"), true);
         newf.ButtonClick += new EventHandler(toolStripButton10_ButtonClick);
         toolStrip2.Items.Insert(toolStrip2.Items.Count - 1, newf);
      }

      public void LoadAllAttachs(Int32 id)
      {
         NodeInfoTag[] nits = MainJrn.GetAttachList(id);
         foreach (NodeInfoTag it in nits) AddAttachInTool(it, "");
         if (nits.Length > 0) toolStripLabel1.Text = "�������������� �����:";
      }

      public void ClearAttachTool()
      {
         while (toolStrip2.Items.Count > 2)
         {
            ToolStripItem tsi = null; Int32 i = 0;
            while ((tsi = toolStrip2.Items[i++]) != null)
               if (tsi != toolStripLabel1 && tsi != toolStripButton12) break;
            toolStrip2.Items.Remove(tsi);
         }
         toolStripLabel1.Text = "��� �������������� ������";
      }

      public void StartSearch()
      {
         if (MainJrn == null)
         {
            findwindow.Close();
            return;
         }
         List<NodeInfoTag> res;
         findwindow.Hide();
         this.DoProcess(true);
         res = MainJrn.Search(findwindow.comboBox5.Text,
            findwindow.checkBox8.CheckState == CheckState.Checked,
            findwindow.checkBox10.CheckState == CheckState.Checked,
            findwindow.checkBox9.CheckState == CheckState.Checked,
            findwindow.checkBox13.CheckState == CheckState.Checked,
            findwindow.checkBox1.CheckState == CheckState.Checked,
            findwindow.checkBox2.CheckState == CheckState.Checked,
            findwindow.checkBox11.CheckState == CheckState.Checked,
            findwindow.DateFrom, findwindow.DateTo,
            findwindow.SizeFrom, findwindow.SizeTo);
         this.DoProcess(false);

         listView2.Items.Clear();
         foreach (NodeInfoTag it in res)
            this.AddToMarkslist(it);
         findwindow.Close();
      }

      public void AddToMarkslist(NodeInfoTag nit)
      {
         ListViewItem lvi = new ListViewItem(nit.wqName);
         lvi.Tag = nit; String res = "";
         if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
            res = Program.GetTextFromRtf(MainJrn.GetNode(nit.wqId));
         if (res.Length > 100) res = res.Substring(0, 97) + "...";
         lvi.ToolTipText = res;
         lvi.SubItems.Add(Program.GetShortSize(nit.wqSize));
         lvi.SubItems.Add(nit.wqDtc.ToShortDateString() + " " + nit.wqDtc.ToShortTimeString());
         lvi.SubItems.Add(nit.wqDtm.ToShortDateString() + " " + nit.wqDtm.ToShortTimeString());
         TreeNode[] tvf = treeView1.Nodes.Find(nit.wqId.ToString(), true);
         res = tvf[0].FullPath;
         if (nit.wqType == NodeInfoTag.wqTypes.wqDir) res += "/";
         res = "/" + System.Text.RegularExpressions.Regex.Replace
             (res, @" \([0-9]+\)\/", "/");
         if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
            res = res.Substring(0, res.Length - 1);
         res = res.Substring(0, res.Length - nit.wqName.Length);
         lvi.SubItems.Add(res).Name = "chPath";
         if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            switch (nit.wqSchema)
            {
               case 0: res = "�������"; break;
               case 1: res = "���"; break;
               case 2: res = "������"; break;
               case 3: res = "���"; break;
               case 4: res = "������"; break;
               case 5: res = "�����"; break;
            }
         }
         else res = "[�����]";
         lvi.SubItems.Add(res);
         listView2.Items.Add(lvi);
      }
      public void LoadCodeMenu()
      {
         System.IO.DirectoryInfo diri = new System.IO.DirectoryInfo
            (Application.StartupPath  + "\\syntax");
         contextMenuStrip3.Items.Clear();
         foreach (System.IO.FileInfo it in diri.GetFiles())
         {
            //System.IO.FileStream fs = it.OpenRead();
            string res = it.OpenText().ReadLine();
            ToolStripItem tsi = contextMenuStrip3.Items.Add(res);
            tsi.Tag = it.FullName;
            tsi.Click += new EventHandler(CodehighlightMenuItem_Click);
         }
         contextMenuStrip3.Show(toolStripComboBox1.Control, 0, 0);
      }
      public void CodehighlightMenuItem_Click(object sender, EventArgs e)
      {
         String path = ((ToolStripItem)sender).Tag.ToString();
         System.IO.FileInfo fi = new System.IO.FileInfo(path);
         System.IO.StreamReader sr = fi.OpenText();
         sr.ReadLine();
         wqRichEdit1.Settings.Comment = sr.ReadLine();
         wqRichEdit1.Settings.Keywords.Clear();
         while (!sr.EndOfStream)
         {
            wqRichEdit1.Settings.Keywords.Add(sr.ReadLine());
         }
         sr.Close();
         wqRichEdit1.Settings.KeywordColor = Color.Blue;
         wqRichEdit1.Settings.CommentColor = Color.Green;
         wqRichEdit1.Settings.StringColor = Color.Gray;
         wqRichEdit1.Settings.IntegerColor = Color.Red;
         wqRichEdit1.Settings.EnableStrings = true;
         wqRichEdit1.Settings.EnableIntegers = true;
         wqRichEdit1.CompileKeywords();

         wqRichEdit1.SelectionFont = new Font("Courier New", 10, FontStyle.Regular);
         //wqRichEdit1.SelectionBackColor = Color.LightBlue;
         wqRichEdit1.ProcessSelectedLines();
      }
      public void InitSettings(bool isload)
      {
         if (isload) // ���� ��� ��������
         {
            this.wqRichEdit1.Font = Program.Opt.FontRichEdit;
         }
         //...
      }
      #endregion

      #region ������� �������� ����

      #region ���� "����"

      private void �����������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (!TryClose()) return;
         mDB.FileName = System.IO.Path.GetTempFileName();
         mDB.FileState = wqFile.wqFileState.wqNew;
         if (MainJrn != null) MainJrn.CloseDB();
         MainJrn = new Journal(mDB.FileName, toolStripProgressBar1);
         MainJrn.CreateNewDB();
         ClearAttachTool();
         treeView1.Nodes.Clear();
         treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
         treeView1.ExpandAll();
         this.SetLabelStatus();
      }

      private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (openFileDialog1.ShowDialog() == DialogResult.OK)
         {
            if (mDB.FileName == openFileDialog1.FileName) return;
            if (!TryClose()) return;
            treeView1.Nodes.Clear();
            ClearAttachTool();
            try
            {
               String res = openFileDialog1.FileName;
               if (MainJrn != null) MainJrn.CloseDB();
               MainJrn = new Journal(res, toolStripProgressBar1);

               this.DoProcess(true);
               //Thread
               MainJrn.LoadDB();

               mDB.FileName = res;
               mDB.FileState = wqFile.wqFileState.wqOpened;
               treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
               treeView1.ExpandAll();
               this.DoProcess(false);
            }
            catch
            {
               toolStripProgressBar1.Visible = false;
               toolStripStatusLabel2.Visible = true;
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
         if (mDB.FileState == wqFile.wqFileState.wqNone) return;
         if (MainJrn.IsChanged == false) return;
         try
         {
            String res = mDB.FileName;

            this.DoProcess(true);
            //Thread
            MainJrn.SaveDB(mDB.FileName);
            this.DoProcess(false);

            //MainJrn = new Journal(res, false);
            mDB = new wqFile();
            mDB.FileName = res;
            mDB.FileState = wqFile.wqFileState.wqOpened;

            //treeView1.Nodes.Clear();
            //treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
            //treeView1.ExpandAll();
         }
         catch
         {
            MessageBox.Show("������!");
         }
         this.SetLabelStatus();
      }

      private void ������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (mDB.FileState == wqFile.wqFileState.wqNone) return;
         if (saveFileDialog1.ShowDialog() == DialogResult.OK)
         {
            mDB.FileName = saveFileDialog1.FileName;
            mDB.FileState = wqFile.wqFileState.wqOpened;
            MainJrn.IsChanged = true;
            ���������ToolStripMenuItem_Click(sender, e);
         }
      }
      #endregion

      #region ���� "������"

      private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         wqRichEdit1.Undo();
      }

      private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         wqRichEdit1.Redo();
      }

      private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         wqRichEdit1.Copy();
      }

      private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         wqRichEdit1.Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
      }

      private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (findwindow == null || findwindow.IsDisposed)
         {
            findwindow = new Form4();
            findwindow.pStartSearch = this.StartSearch;
            findwindow.Show(MainForm.ActiveForm);
         }
         else
         {
            findwindow.Activate();
         }
      }

      private void ���������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (wqRichEdit1.Text.Length > 0)
         {
            PosForm frm = new PosForm();
            frm.wqre = wqRichEdit1;
            frm.Show(MainForm.ActiveForm);
         }
      }
      #endregion

      #region ���� "���"

      // ����� � ������� 6 �������� ��������� ���������� 
      private void ������������������ToolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
      {
         toolStrip1.Visible = toolBarToolStripMenuItem1.Checked;
      }
      private void ���������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         splitContainer1.Panel1Collapsed = !navigateBarToolStripMenuItem.Checked;
      }
      private void ���������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         splitContainer3.Panel1Collapsed = !headToolStripMenuItem.Checked;
      }
      private void ����������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         noteToolStripMenuItem.Enabled = searchResultToolStripMenuItem.Checked;
         splitContainer2.Panel1Collapsed = !searchResultToolStripMenuItem.Checked;
      }
      private void �������������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         toolStrip2.Visible = attachedFilesToolStripMenuItem.Checked;
      }
      private void ���������������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         statusStrip1.Visible = statusBarToolStripMenuItem.Checked;
      }
      private void �������ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         attachedFilesToolStripMenuItem.Enabled = noteToolStripMenuItem.Checked;
         if (noteToolStripMenuItem.Checked)
         {
            attachedFilesToolStripMenuItem.Checked = (bool)attachedFilesToolStripMenuItem.Tag;
         }
         else
         {
            attachedFilesToolStripMenuItem.Tag = attachedFilesToolStripMenuItem.Checked;
            attachedFilesToolStripMenuItem.Checked = false;
         }
         searchResultToolStripMenuItem.Enabled = noteToolStripMenuItem.Checked;
         splitContainer2.Panel2Collapsed = !noteToolStripMenuItem.Checked;
      }

      // ���� ��� - ������� ������������
      private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         toolBarToolStripMenuItem1.Checked = true;
         navigateBarToolStripMenuItem.Checked = true;
         headToolStripMenuItem.Checked = true;
         searchResultToolStripMenuItem.Checked = true;
         noteToolStripMenuItem.Checked = true;
         attachedFilesToolStripMenuItem.Checked = true;
         statusBarToolStripMenuItem.Checked = true;
      }

      private void �����������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         toolBarToolStripMenuItem1.Checked = true;
         navigateBarToolStripMenuItem.Checked = false;
         headToolStripMenuItem.Checked = true;
         searchResultToolStripMenuItem.Checked = false;
         noteToolStripMenuItem.Checked = true;
         attachedFilesToolStripMenuItem.Checked = false;
         statusBarToolStripMenuItem.Checked = true;
      }
      #endregion


      #region ���� "�������"

      private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         AboutForm frm2 = new AboutForm();
         frm2.ShowDialog();
      }
      #endregion

      #endregion

      #region ������� ����������� ����

      #region ������ "�����", "���������"

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
      #endregion

      private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
      {
         if (treeView1.SelectedNode != null)
         {
            NodeInfoTag np = null;
            if (treeView1.SelectedNode.Parent == null)
               toLevelTopToolStripMenuItem.Enabled = false;
            else toLevelTopToolStripMenuItem.Enabled = true;
            if (treeView1.SelectedNode.PrevNode == null)
               movetoTopToolStripMenuItem.Enabled = false;
            else
            {
               movetoTopToolStripMenuItem.Enabled = true;
               np = (NodeInfoTag)treeView1.SelectedNode.PrevNode.Tag;
            }
            if (treeView1.SelectedNode.NextNode == null)
               movetoBottomToolStripMenuItem.Enabled = false;
            else movetoBottomToolStripMenuItem.Enabled = true;
            if (np == null || np.wqType != NodeInfoTag.wqTypes.wqDir)
               toLevelBottomToolStripMenuItem.Enabled = false;
            else toLevelBottomToolStripMenuItem.Enabled = true;

            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (nit.wqParent_id == 1)
               toLevelTopToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem3.Visible = true;
            extractAllToolStripMenuItem.Visible = true;
            toLevelTopToolStripMenuItem.Visible = true;
            toLevelBottomToolStripMenuItem.Visible = true;
            movetoTopToolStripMenuItem.Visible = true;
            movetoBottomToolStripMenuItem.Visible = true;
            if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
            {
               cutToolStripMenuItem1.Visible = false;
               copyToolStripMenuItem1.Visible = false;
               addToMarksToolStripMenuItem.Visible = false;
               styleToolStripMenuItem.Visible = false;
               createToolStripMenuItem.Visible = true;
               extractAllToolStripMenuItem.Visible = true;
               if (nit.wqId == 1)
               {
                  deleteToolStripMenuItem3.Visible = false;
                  extractAllToolStripMenuItem.Visible = false;
                  toLevelTopToolStripMenuItem.Visible = false;
                  toLevelBottomToolStripMenuItem.Visible = false;
                  movetoTopToolStripMenuItem.Visible = false;
                  movetoBottomToolStripMenuItem.Visible = false;
               }
            }
            else
            {
               cutToolStripMenuItem1.Visible = true;
               copyToolStripMenuItem1.Visible = true;
               addToMarksToolStripMenuItem.Visible = true;
               styleToolStripMenuItem.Visible = true;
               createToolStripMenuItem.Visible = false;
               extractAllToolStripMenuItem.Visible = false;

               noteToolStripMenuItem1.Checked = false;
               codeToolStripMenuItem1.Checked = false;
               quoteToolStripMenuItem.Checked = false;
               logToolStripMenuItem.Checked = false;
               mailToolStripMenuItem.Checked = false;
               reportToolStripMenuItem.Checked = false;
               switch (nit.wqSchema)
               {
                  case 0: noteToolStripMenuItem1.Checked = true; break;
                  case 1: codeToolStripMenuItem1.Checked = true; break;
                  case 2: quoteToolStripMenuItem.Checked = true; break;
                  case 3: logToolStripMenuItem.Checked = true; break;
                  case 4: mailToolStripMenuItem.Checked = true; break;
                  case 5: reportToolStripMenuItem.Checked = true; break;
               }
            }
            highToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            lowToolStripMenuItem.Checked = false;
            switch (nit.wqPriority)
            {
               case 0: mediumToolStripMenuItem.Checked = true; break;
               case 1: highToolStripMenuItem.Checked = true; break;
               case 2: lowToolStripMenuItem.Checked = true; break;
            }
         }
         else e.Cancel = true;
      }

      private void �����������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.AddToMarkslist((NodeInfoTag)treeView1.SelectedNode.Tag);
      }

      private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         String name = "����� ����� #" + Program.LastNumberElem.ToString();
         Program.LastNumberElem++;
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
         String name = "����� ������� #" + Program.LastNumberElem.ToString();
         Program.LastNumberElem++;
         TreeNode res = FullTreeNode(MainJrn.CreateNode(nit.wqId, name));
         treeView1.SelectedNode.Nodes.Add(res);
         //mDB.NodeList.Add(Int32.Parse(res.Name));
         this.RefreshTop(res);
         treeView1.SelectedNode.ExpandAll();
         treeView1.LabelEdit = true;
         res.BeginEdit();
      }

      private void �������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         treeView1.SelectedNode.Text = nit.wqName;
         treeView1.LabelEdit = true;
         treeView1.SelectedNode.BeginEdit();
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
            if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
            {
               if (MainJrn.GetAttachList(nit.wqId).Length > 0)
                  if (MessageBox.Show("������� �������� �������������� " +
                      "�����, ������� ����� ����� �������. ����������?",
                      "wqNotes", MessageBoxButtons.YesNo,
                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                      == DialogResult.No)
                     return;
            }
            this.DeleteElem(treeView1.SelectedNode, true);
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

      #region ���� �������

      private void �������ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).OwnerItem.Tag;
         String start = "";
         try
         {
            if (nit.wqFlag == "shortcut") start = nit.wqName;
            else
            {
               try
               {
                  start = System.IO.Path.GetTempFileName();
                  start += System.IO.Path.GetExtension(nit.wqName);
                  MainJrn.SaveAttach(nit.wqId, start);
               }
               catch (Exception ex) { throw ex; }
            }
            System.Diagnostics.Process.Start(start);
         }
         catch (Exception ex)
         {
            MessageBox.Show("���������� ������� ����. �����������:\n" +
                ex.Message, "wqNotes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (nit.wqFlag != "shortcut")
               try { System.IO.File.Delete(start); }
               catch { }
         }
      }

      private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         MessageBox.Show("�� ������!");
      }

      private void ���������ToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).OwnerItem.Tag;
         saveFileDialog2.FileName = System.IO.Path.GetFileName(nit.wqName);
         if (saveFileDialog2.ShowDialog() == DialogResult.OK)
         {
            MainJrn.SaveAttach(nit.wqId, saveFileDialog2.FileName);
         }
      }

      private void �������ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).OwnerItem.Tag;
         if (MessageBox.Show("�� �������, ��� ������ ������� �������������� " +
             "���� \"" + System.IO.Path.GetFileName(nit.wqName) + "\"",
             "wqNotes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
             MessageBoxDefaultButton.Button2) == DialogResult.Yes)
         {
            MainJrn.DeleteAttach(nit.wqId);
            toolStrip2.Items.Remove(((ToolStripItem)sender).OwnerItem);
            if (toolStrip2.Items.Count == 2)
               toolStripLabel1.Text = "��� �������������� ������";
            NodeInfoTag nt = (NodeInfoTag)treeView1.SelectedNode.Tag;
            treeView1.SelectedNode.Tag = MainJrn.GetInfoElem(nt.wqId, nt.wqType);
            RefreshTop(treeView1.SelectedNode);
         }
      }

      private void ��������ToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         MessageBox.Show("�� ������!");
      }

      private void toolStripButton10_ButtonClick(object sender, EventArgs e)
      {
         ToolStripSplitButton tsi = (ToolStripSplitButton)sender;
         �������ToolStripMenuItem1_Click(tsi.DropDownItems[0], e);
      }

      private void toolStripButton12_Click(object sender, EventArgs e)
      {
         if (openFileDialog2.ShowDialog() == DialogResult.OK)
         {
            AttachForm frm7 = new AttachForm();
            frm7.wqInput = openFileDialog2.FileName;
            if (frm7.ShowDialog() == DialogResult.OK)
            {
               try
               {
                  System.IO.FileInfo fi = new
                      System.IO.FileInfo(openFileDialog2.FileName);
                  if (!fi.Exists) throw new System.IO.FileNotFoundException();
               }
               catch (UnauthorizedAccessException ex)
               {
                  MessageBox.Show("��� ������� � �����. �����������:\n" +
                      ex.Message, "wqNotes", MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                  return;
               }
               catch (System.IO.FileNotFoundException ex)
               {
                  MessageBox.Show("���� �� ������. �����������:\n" +
                    ex.Message, "wqNotes", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                  return;
               }
               catch (Exception ex)
               {
                  MessageBox.Show("������������ ������. �����������:\n" +
                      ex.Message, "wqNotes", MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                  return;
               }
               toolStripLabel1.Text = "�������������� �����:";
               AddAttachInTool(MainJrn.CreateAttach(mDB.NowNode.wqId,
                   openFileDialog2.FileName,
                   frm7.wqOut_IsLink, frm7.wqOut_Dtc, frm7.wqOut_Dtm),
                   openFileDialog2.FileName);
               //
               this.RefreshTop(treeView1.SelectedNode);
            }
         }
      }
      #endregion

      #region ���� wqRichText
      private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
      {
         if (wqRichEdit1.CanUndo)
            undoToolStripMenuItem1.Enabled = true;
         else undoToolStripMenuItem1.Enabled = false;
         if (wqRichEdit1.CanRedo)
            redoToolStripMenuItem1.Enabled = true;
         else redoToolStripMenuItem1.Enabled = false;
         if (wqRichEdit1.SelectionLength > 0)
            cutToolStripMenuItem2.Enabled = true;
         else cutToolStripMenuItem2.Enabled = false;
         if (wqRichEdit1.SelectionLength > 0)
            copyToolStripMenuItem2.Enabled = true;
         else copyToolStripMenuItem2.Enabled = false;
         if (wqRichEdit1.CanPaste(DataFormats.GetFormat(DataFormats.Text)))
            pasteToolStripMenuItem2.Enabled = true;
         else pasteToolStripMenuItem2.Enabled = false;
         if (wqRichEdit1.Text.Length > 0)
            deleteToolStripMenuItem4.Enabled = true;
         else deleteToolStripMenuItem4.Enabled = false;
         if (wqRichEdit1.Text.Length > 0)
            selectAllToolStripMenuItem1.Enabled = true;
         else selectAllToolStripMenuItem1.Enabled = false;

         if (mDB == null || mDB.NowNode == null) e.Cancel = true;
      }

      private void ��������ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         ��������ToolStripMenuItem_Click(sender, e);
      }

      private void ���������ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         ���������ToolStripMenuItem_Click(sender, e);
      }

      private void ����������ToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         ����������ToolStripMenuItem_Click(sender, e);
      }

      private void ��������ToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         ��������ToolStripMenuItem_Click(sender, e);
      }

      private void �����������ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         �����������ToolStripMenuItem1_Click(sender, e);
      }
      #endregion

      #endregion

      #region ������� �������

      private void toolStripStatusLabel2_DoubleClick(object sender, EventArgs e)
      {
         ���������������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton1_Click(object sender, EventArgs e)
      {
         �����������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton2_ButtonClick(object sender, EventArgs e)
      {
         �������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton3_Click(object sender, EventArgs e)
      {
         ���������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton6_Click(object sender, EventArgs e)
      {
         ����������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton7_Click(object sender, EventArgs e)
      {
         ��������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton9_ButtonClick(object sender, EventArgs e)
      {
         ��������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
      {
         ���������ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton13_Click(object sender, EventArgs e)
      {
         �����ToolStripMenuItem_Click(sender, e);
      }

      private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (wqRichEdit1.SelectedText == "") return;
         switch (toolStripComboBox1.SelectedItem.ToString())
         {
            case "������� �����":
               wqRichEdit1.SelectionFont = new Font("Lucida Console", 8, FontStyle.Regular);
               wqRichEdit1.SelectionColor = Color.FromKnownColor(KnownColor.WindowText);
               //wqRichEdit1.SelectionBackColor = Color.FromKnownColor(KnownColor.Window);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Left;
               wqRichEdit1.SelectionCharOffset = 0;
               wqRichEdit1.SelectionBullet = false;
               break;
            case "��������� 1":
               wqRichEdit1.SelectionFont = new Font("Arial", 13, FontStyle.Bold);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Center;
               break;
            case "��������� 2":
               wqRichEdit1.SelectionFont = new Font("Times New Roman", 14, FontStyle.Bold);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Center;
               break;
            case "��������� 3":
               wqRichEdit1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Left;
               break;
            case "������":
               wqRichEdit1.SelectionBullet = !wqRichEdit1.SelectionBullet;
               //
               break;
            case "���":
               LoadCodeMenu();
               break;
            case "�������":
               wqRichEdit1.SelectionFont = new Font("Times New Roman", 11, FontStyle.Italic);
               break;
            case "����������":
               wqRichEdit1.SelectionFont = new Font("Microsoft Sans Serif", 9, FontStyle.Italic);
               wqRichEdit1.SelectionColor = Color.Gray;
               break;
            case "���������":
               //wqRichEdit1.SelectionBackColor = Color.Yellow;
               break;
            case "������� ������":
               wqRichEdit1.SelectionCharOffset = 2;
               break;
            case "������ ������":
               wqRichEdit1.SelectionCharOffset = -2;
               break;
            default:
               wqRichEdit1.SelectionFont = new Font("Lucida Console", 8, FontStyle.Regular);
               wqRichEdit1.SelectionColor = Color.FromKnownColor(KnownColor.WindowText);
               //wqRichEdit1.SelectionBackColor = Color.FromKnownColor(KnownColor.Window);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Left;
               wqRichEdit1.SelectionCharOffset = 0;
               wqRichEdit1.SelectionBullet = false;
               break;
         }
      }
      #endregion

      #region ������� ���������

      private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
      }

      public MainForm()
      {
         InitializeComponent();
         Application.Idle += new EventHandler(Application_Idle);
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         // ������������� (������� �������� � ������)
         treeView1.ExpandAll();
         toolStripComboBox1.SelectedIndex = 0;
         treeView1.Nodes.Clear();
         listView2.Items.Clear();
         label10.Text = "<�� ������ �������>";
         toolStripLabel1.Text = "��� �������������� ������";
         toolStripStatusLabel2.Text = "Ln 0 Col 0";
         toolStrip2.Items.Remove(toolStripButton10);
         toolStrip2.Items.Remove(toolStripButton11);
         toolStripButton12.Enabled = false;
         wqRichEdit1.wqClear();
         wqRichEdit1.ClearUndo();
         listView1.Items.Clear();
         listView1.Groups.Clear();
         ////////////////////////////
         InitSettings(true);

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

         //this.Location = Properties.Settings.Default.Location;
         //this.Size = Properties.Settings.Default.Size;
         //this.WindowState = Properties.Settings.Default.WinState;
         //if (Properties.Settings.Default.LoadLastFile == true)
         //   mDB.FileName = Properties.Settings.Default.LastFile;
         //string RecentFiles = Properties.Settings.Default.RecentFiles;

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
               MainJrn = new Journal(mDB.FileName, toolStripProgressBar1);
               this.DoProcess(true);
               //Thread
               MainJrn.LoadDB();
               this.DoProcess(false);
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

      private void wqRichEdit1_SelectionChanged(object sender, EventArgs e)
      {
         toolStripStatusLabel2.Text = "Ln " +
             (wqRichEdit1.GetLineFromCharIndex(wqRichEdit1.SelectionStart) + 1).ToString() +
             " Pos " +
             (wqRichEdit1.GetCharIndexFromPosition(
             wqRichEdit1.GetPositionFromCharIndex(wqRichEdit1.SelectionStart)
             ) - wqRichEdit1.GetFirstCharIndexOfCurrentLine() + 1).ToString();
      }

      private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
      {
         //����� ���������� ����� �������, �� ������ ��� ClearUndo!
         if (mDB.NowNode != null && mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            if (mDB.NodeChange == true)
            {
               MainJrn.SetNodeContent(mDB.NowNode.wqId, wqRichEdit1.Rtf);
               mDB.NodeChange = false;
               if (!mDB.NodeList.Contains(mDB.NowNode.wqId))
                  mDB.NodeList.Add(mDB.NowNode.wqId);
            }
         }
         mDB.NowNode = (NodeInfoTag)treeView1.SelectedNode.Tag;
         if (mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            //MainJrn.SetNodeContent(1, "{\\rtf1}\0");
            wqRichEdit1.Rtf = MainJrn.GetNode(mDB.NowNode.wqId);
            mDB.NodeChange = false;
            label10.Text = mDB.NowNode.wqName;
            ClearAttachTool();
            LoadAllAttachs(mDB.NowNode.wqId);
         }
         else label10.Text = "[" + mDB.NowNode.wqName + "]";
         //toolStripLabel1.Text = "�������������� �����:";
         toolStripButton12.Enabled = true;
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
               e.Node.Text += " (" + nit.wqCount + ")";
            return;
         }
         MainJrn.Rename(nit, e.Label);
         e.Node.Tag = MainJrn.GetInfoElem(nit.wqId, nit.wqType);
         this.RefreshTop(e.Node, 1);
         treeView1.SelectedNode = e.Node;
         treeView1.LabelEdit = false;
         if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
         {
            e.Node.Text = e.Label + " (" + nit.wqCount + ")";
            e.CancelEdit = true;
         }
      }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (!TryClose()) e.Cancel = true;
      }

      private void Form1_FormClosed(object sender, FormClosedEventArgs e)
      {
         //��������� ��������� ������
         //Properties.Settings.Default.WinState = this.WindowState;
         //if (this.WindowState == FormWindowState.Normal)
         //{
         //   Properties.Settings.Default.Location = this.Location;
         //   Properties.Settings.Default.Size = this.Size;
         //}
         //else
         //{
         //   Properties.Settings.Default.Location = this.RestoreBounds.Location;
         //   Properties.Settings.Default.Size = this.RestoreBounds.Size;
         //}

         //Program.Opt.Save();
         //Properties.Settings.Default.Save();
      }

      private void wqRichEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
      {
         //toolStripStatusLabel4.Text = Control.IsKeyLocked(Keys.Insert) ? "OVR" : "INS";
      }

      private void Application_Idle(object sender, EventArgs e)
      {
         //toolStripStatusLabel3.ForeColor = Control.IsKeyLocked(Keys.CapsLock) ? Color.Black : Color.Gray;
      }

      private void Form1_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
      {
         toolStripStatusLabel5.Text = e.Culture.TwoLetterISOLanguageName.ToUpper();
         toolStripStatusLabel5.ToolTipText = e.Culture.NativeName;
      }

      private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
            TreeNode[] res = treeView1.Nodes.Find(nit.wqId.ToString(), true);
            NodeInfoTag f = null;
            if (res.Length == 1) f = (NodeInfoTag)res[0].Tag;
            if (f != null && f.wqName == nit.wqName && f.wqType == nit.wqType)
               treeView1.SelectedNode = res[0];
         }
      }

      private void treeView1_MouseClick(object sender, MouseEventArgs e)
      {
         if (treeView1.SelectedNode == null) return;
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         if (nit.wqType == NodeInfoTag.wqTypes.wqDir) return;
         if (Control.ModifierKeys == Keys.Control)
            �����������������ToolStripMenuItem_Click(sender, new EventArgs());
      }

      private void wqRichEdit1_TextChanged(object sender, EventArgs e)
      {
         if (mDB != null)
            mDB.NodeChange = true;
      }
      #endregion

      private void button1_Click(object sender, EventArgs e)
      {
         if (MainJrn == null) return;
         DateTime DateFrom = DateTime.MinValue, DateTo = DateTime.MaxValue;
         int SizeFrom = int.MinValue, SizeTo = int.MaxValue;
         ////////////////////////////////////////////////
         if (textBox1.Text == "") return;
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
               case "�����":
                  DateFrom -= new TimeSpan(0, (int)numericUpDown1.Value, 0, 0);
                  break;
               case "�����":
                  DateFrom -= new TimeSpan((int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "������":
                  DateFrom -= new TimeSpan(7 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "�������":
                  DateFrom -= new TimeSpan(30 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "���":
                  DateFrom -= new TimeSpan(365 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
            }
         }
         if (checkBox5.Checked && comboBox3.Text != "")
         {
            switch (comboBox3.SelectedItem.ToString())
            {
               case "����": SizeFrom = (int)numericUpDown2.Value; break;
               case "�����": SizeFrom = (int)numericUpDown2.Value * 1024; break;
               case "�����": SizeFrom = (int)numericUpDown2.Value * 1024 * 1004; break;
            }
         }
         if (checkBox7.Checked && comboBox4.Text != "")
         {
            switch (comboBox4.SelectedItem.ToString())
            {
               case "����": SizeTo = (int)numericUpDown3.Value; break;
               case "�����": SizeTo = (int)numericUpDown3.Value * 1024; break;
               case "�����": SizeTo = (int)numericUpDown3.Value * 1024 * 1004; break;
            }
         }
         ////////////////////////////////////////////////
         List<NodeInfoTag> res;
         button1.Enabled = false;
         this.DoProcess(true);
         res = MainJrn.Search(textBox1.Text,
            checkBox8.CheckState == CheckState.Checked,
            checkBox10.CheckState == CheckState.Checked,
            checkBox9.CheckState == CheckState.Checked,
            checkBox12.CheckState == CheckState.Checked,
            checkBox1.CheckState == CheckState.Checked,
            checkBox2.CheckState == CheckState.Checked,
            checkBox11.CheckState == CheckState.Checked,
            DateFrom, DateTo, SizeFrom, SizeTo);
         this.DoProcess(false);

         listView2.Items.Clear();
         foreach (NodeInfoTag it in res)
            this.AddToMarkslist(it);
         button1.Enabled = true;
      }

      private void tabControl1_MouseClick(object sender, MouseEventArgs e)
      {
         if (tabControl1.SelectedTab.Name == "tabPage3")
         {
            if(MainJrn == null) return;
            listView1.Items.Clear();
            listView1.Groups.Clear();
            List<NodeInfoTag> res = MainJrn.GetHistory(
               new TimeSpan(1, 0, 0, 0), new TimeSpan(1, 0, 0, 0));
            ListViewGroup lvg = listView1.Groups.Add("today", "������� (" +
               res.Count.ToString() + ")");
            foreach (NodeInfoTag nit in res)
            {
               ListViewItem lvi = new ListViewItem(new String[] { nit.wqName, 
                  Program.GetShortSize(nit.wqSize),
                  (nit.wqDtm.ToShortDateString() + " " +
                     nit.wqDtm.ToShortTimeString()),
                  ("/" + System.Text.RegularExpressions.Regex.Replace
                     (treeView1.Nodes.Find(nit.wqId.ToString(), true)[0].FullPath,
                     @" \([0-9]+\)\/", "/")),
                  (new String[] { "�������", "���", "������", "���", 
                     "������", "�����" }[nit.wqSchema]) }, lvg);
               lvi.Tag = nit;
               listView1.Items.Add(lvi);
            }
            //
            res = MainJrn.GetHistory(
               new TimeSpan(2, 0, 0, 0), new TimeSpan(1, 0, 0, 0));
            lvg = listView1.Groups.Add("yesterday", "����� (" +
               res.Count.ToString() + ")");
            foreach (NodeInfoTag nit in res)
            {
               ListViewItem lvi = new ListViewItem(new String[] { nit.wqName, 
                  Program.GetShortSize(nit.wqSize),
                  (nit.wqDtm.ToShortDateString() + " " +
                     nit.wqDtm.ToShortTimeString()),
                  ("/" + System.Text.RegularExpressions.Regex.Replace
                     (treeView1.Nodes.Find(nit.wqId.ToString(), true)[0].FullPath,
                     @" \([0-9]+\)\/", "/")),
                  (new String[] { "�������", "���", "������", "���", 
                     "������", "�����" }[nit.wqSchema]) }, lvg);
               lvi.Tag = nit;
               listView1.Items.Add(lvi);
            }
            //
            res = MainJrn.GetHistory(
               new TimeSpan(7, 0, 0, 0), new TimeSpan(5, 0, 0, 0));
            lvg = listView1.Groups.Add("backweek", "������ ����� (" +
               res.Count.ToString() + ")");
            foreach (NodeInfoTag nit in res)
            {
               ListViewItem lvi = new ListViewItem(new String[] { nit.wqName, 
                  Program.GetShortSize(nit.wqSize),
                  (nit.wqDtm.ToShortDateString() + " " +
                     nit.wqDtm.ToShortTimeString()),
                  ("/" + System.Text.RegularExpressions.Regex.Replace
                     (treeView1.Nodes.Find(nit.wqId.ToString(), true)[0].FullPath,
                     @" \([0-9]+\)\/", "/")),
                  (new String[] { "�������", "���", "������", "���", 
                     "������", "�����" }[nit.wqSchema]) }, lvg);
               lvi.Tag = nit;
               listView1.Items.Add(lvi);
            }
         }
         // end function
      }

      private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (listView1.SelectedItems.Count > 0)
         {
            NodeInfoTag nit = (NodeInfoTag)listView1.SelectedItems[0].Tag;
            TreeNode[] res = treeView1.Nodes.Find(nit.wqId.ToString(), true);
            NodeInfoTag f = null;
            if (res.Length == 1) f = (NodeInfoTag)res[0].Tag;
            if (f != null && f.wqName == nit.wqName && f.wqType == nit.wqType)
               treeView1.SelectedNode = res[0];
         }
      }

      private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
      {
         e.Cancel = listView2.SelectedItems.Count == 0;
         if (!e.Cancel)
         {
            NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
            if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
               copyContentToolStripMenuItem.Visible = false;
            else
               copyContentToolStripMenuItem.Visible = true;
         }
      }

      private void ��������������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            try
            {
               NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
               Clipboard.SetText(MainJrn.GetNode(nit.wqId), TextDataFormat.Rtf);
            }
            catch (Exception)
            {
               MessageBox.Show("�� ������� ���������� �����������. ��������, " +
                  "������� ���� �������.", "wqNotes", MessageBoxButtons.OK,
                  MessageBoxIcon.Exclamation);
            }
         }
         return;
      }

      private void ������������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
            Clipboard.SetText(nit.wqName, TextDataFormat.Text);
         }
      }

      private void ��������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            Clipboard.SetText(listView2.SelectedItems[0].SubItems["chPath"].Text, TextDataFormat.Text);
         }
      }

      private void ���������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count == 0) return;
         foreach (ListViewItem lvi in listView2.SelectedItems)
            listView2.Items.Remove(lvi);
      }

      private void �������ToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         if (Program.Opt.IsHideOnMinimize) this.Show();
         this.WindowState = FormWindowState.Normal;
         this.Activate();
      }

      private void ����������ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         ����������ToolStripMenuItem_Click(sender, e);
      }

      private void �����ToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         �����ToolStripMenuItem_Click(sender, e);
      }

      private void ����������������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // �������� ����� �� ������ � ����� �������
         //String res = wqRichEdit1.Rtf; Int32 i = res.Length;
         //while (res[--i] != '}') ; res = res.Substring(0, i);
         String clp = Clipboard.GetText(TextDataFormat.Text);

         clp = "\r\n==================================\r\n" + clp;
         wqRichEdit1.AppendText(clp);
         wqRichEdit1.SelectionStart = wqRichEdit1.TextLength;
         //return;
         // ��� ������ ������ ������ � rtf
         //clp = clp.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\\par ");
         //res += "==================================\\par " + clp + "\\par}";
         //wqRichEdit1.Rtf = res;
      }

      private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
      {
         if (mDB == null || mDB.NowNode == null ||
            mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqDir ||
            !Clipboard.ContainsText(TextDataFormat.Text))
            addFromClipboardToolStripMenuItem.Enabled = false;
         else
            addFromClipboardToolStripMenuItem.Enabled = true;
      }

      private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (addFromClipboardToolStripMenuItem.Enabled)
         {
            ����������������ToolStripMenuItem_Click(sender, (EventArgs)e);
            timer1.Enabled = false;
         }
      }

      private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left) timer1.Enabled = true;
      }

      private void timer1_Tick(object sender, EventArgs e)
      {
         �������ToolStripMenuItem2_Click(sender, (EventArgs)e);
         timer1.Enabled = false;
      }

      private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         wqRichEdit1.Focus();
         SendKeys.Send("{DEL}");
      }

      private void �������ToolStripMenuItem4_Click(object sender, EventArgs e)
      {
         �������ToolStripMenuItem_Click(sender, e);
      }

      private void MainForm_Move(object sender, EventArgs e)
      {
         if (this.WindowState == FormWindowState.Minimized)
         {
            if (Program.Opt.IsHideOnMinimize == true)
            {
               this.Hide();
            }
         }
      }

      private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         OptionsForm opt = new OptionsForm();
         if (opt.ShowDialog() == DialogResult.Yes)
         {
            InitSettings(false);
         }
      }
   }
}