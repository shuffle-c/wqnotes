/*
* Copyright (c) 2007-2008 wqNotes Project
* License: BSD
* Windows: MainForm.cs, $Revision$
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
   public partial class MainForm : Form
   {
      #region Переменные

      Form4 findwindow;
      //==================================
      Journal MainJrn;
      wqFile mDB;
      //==================================
      #endregion

      #region Функции и методы

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
            if ((Properties.Settings.Default.SaveMode == 1 ||
                Properties.Settings.Default.SaveMode == 3) &&
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
               iDr = MessageBox.Show("Сохранить журнал?", "wqNotes",
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
                     MessageBox.Show(ex.Message, "Ошибка!");
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

      public void DoOpenFile(string name)
      {
         if (!TryClose()) return;
         treeView1.Nodes.Clear();
         ClearAttachTool();
         mDB = new wqFile();
         try
         {
            if (MainJrn != null) MainJrn.CloseDB();
            MainJrn = new Journal(name, toolStripProgressBar1);
            Boolean isrec = MainJrn.IsChanged;
            mDB.FileState = wqFile.wqFileState.wqOpened;
            mDB.FileName = name;

            this.DoProcess(true);
            //Thread
            MainJrn.LoadDB();
            if (isrec) mDB.NodeList = MainJrn.RecoveryDB();
            treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
            treeView1.ExpandAll(); //
            AddRecentFile(mDB.FileName);
            this.DoProcess(false);
         }
         catch
         {
            MainJrn = null;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel2.Visible = true;
            mDB = new wqFile();
         }
         this.SetLabelStatus();
      }

      public void DoProcess(bool start)
      {
         toolStripProgressBar1.Visible = start;
         toolStripStatusLabel2.Visible = !start;
         wqRichEdit1.Enabled = !start;
         treeView1.Enabled = !start;
      }

      public TreeNode SearchItem(NodeInfoTag nit)
      {
         TreeNode[] res = treeView1.Nodes.Find(nit.wqId.ToString(), true);
         NodeInfoTag f = null;
         if (res.Length == 1) f = (NodeInfoTag)res[0].Tag;
         if (f != null && f.wqName == nit.wqName && f.wqType == nit.wqType)
            return res[0];
         return null;
      }

      public void MovingLeftToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).Tag;
         TreeNode res = this.SearchItem(nit);
         try
         {
            if (res == null || mDB.NowNode == nit) // Удалить из списка
            {
               toolStripSplitButton2.DropDownItems.Remove((ToolStripItem)sender);
               mDB.MovingList.Remove(nit);
               throw new Exception();
            }
            mDB.MoveRet = true;
            treeView1.SelectedNode = res;
            mDB.MoveRet = false;

            toolStripSplitButton3.Enabled = true;
            ToolStripMenuItem tsi;
            Object bak = null;
            while (bak != sender)
            {
               tsi = new ToolStripMenuItem();
               tsi.Text = toolStripSplitButton2.DropDownItems[0].Text;
               tsi.Tag = toolStripSplitButton2.DropDownItems[0].Tag;
               tsi.Click += new EventHandler(MovingRightToolStripMenuItem_Click);
               toolStripSplitButton2.DropDownItems.RemoveAt(0);
               toolStripSplitButton3.DropDownItems.Insert(0, tsi);
               while (toolStripSplitButton3.DropDownItems.Count > 10)
                  toolStripSplitButton3.DropDownItems.RemoveAt(10);
               bak = toolStripSplitButton2.DropDownItems[0];
               mDB.MovePos++;
            }
            mDB.MovingList.RemoveAt(mDB.MovePos); // Удалить текущий
            toolStripSplitButton2.DropDownItems.RemoveAt(0);
         }
         catch { }
         finally
         {
            ToolStripMenuItem tsi;
            // Добавить недостающие
            while (toolStripSplitButton2.DropDownItems.Count < 10)
            {
               Int32 pos = mDB.MovePos + toolStripSplitButton2.DropDownItems.Count;
               if (pos >= mDB.MovingList.Count) break;
               tsi = new ToolStripMenuItem(mDB.MovingList[pos].wqName);
               tsi.Tag = mDB.MovingList[pos];
               tsi.Click += new EventHandler(MovingLeftToolStripMenuItem_Click);
               toolStripSplitButton2.DropDownItems.Add(tsi);
            }
            if (toolStripSplitButton2.DropDownItems.Count == 0)
               toolStripSplitButton2.Enabled = false;
         }
      }

      public void MovingRightToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).Tag;
         TreeNode res = this.SearchItem(nit);
         try
         {
            if (res == null || mDB.NowNode == nit) // Удалить из списка
            {
               toolStripSplitButton3.DropDownItems.Remove((ToolStripItem)sender);
               //if (mDB.MovingList.IndexOf(nit) <= mDB.MovePos)
               if (mDB.MovePos > 0) mDB.MovePos--;
               mDB.MovingList.Remove(nit);
               throw new Exception();
            }
            mDB.MoveRet = true;
            treeView1.SelectedNode = res;
            mDB.MoveRet = false;

            toolStripSplitButton2.Enabled = true;
            ToolStripMenuItem tsi;
            Object bak = toolStripSplitButton3.DropDownItems[0];
            mDB.MovePos--;
            while (bak != sender)
            {
               tsi = new ToolStripMenuItem();
               tsi.Text = toolStripSplitButton3.DropDownItems[0].Text;
               tsi.Tag = toolStripSplitButton3.DropDownItems[0].Tag;
               tsi.Click += new EventHandler(MovingLeftToolStripMenuItem_Click);
               toolStripSplitButton3.DropDownItems.RemoveAt(0);
               toolStripSplitButton2.DropDownItems.Insert(0, tsi);
               bak = toolStripSplitButton3.DropDownItems[0];
               mDB.MovePos--;
            }
            mDB.MovingList.RemoveAt(mDB.MovePos); // Удалить текущий
            toolStripSplitButton3.DropDownItems.RemoveAt(0);
            while (toolStripSplitButton2.DropDownItems.Count > 10)
               toolStripSplitButton2.DropDownItems.RemoveAt(10);
         }
         catch { }
         finally
         {
            ToolStripMenuItem tsi;
            // Добавить недостающие
            while (toolStripSplitButton3.DropDownItems.Count < 10)
            {
               Int32 pos = mDB.MovePos - toolStripSplitButton3.DropDownItems.Count - 1;
               if (pos < 0) break;
               tsi = new ToolStripMenuItem(mDB.MovingList[pos].wqName);
               tsi.Tag = mDB.MovingList[pos];
               tsi.Click += new EventHandler(MovingRightToolStripMenuItem_Click);
               toolStripSplitButton3.DropDownItems.Add(tsi);
            }
            if (toolStripSplitButton3.DropDownItems.Count == 0)
               toolStripSplitButton3.Enabled = false;
         }
      }

      public void RecentFileToolStripMenuItem_Click(object sender, EventArgs e)
      {
         string name = ((ToolStripItem)sender).Tag.ToString();
         if (name != mDB.FileName)
            this.DoOpenFile(name);
      }

      public void SetupRecentFiles()
      {
         toolStripButton2.DropDownItems.Clear();
         недавниеФайлыToolStripMenuItem.DropDownItems.Clear();
         foreach (object it in Properties.Settings.Default.RecentFiles)
         {
            ToolStripItem f = new ToolStripMenuItem(), g = new ToolStripMenuItem();
            f.Click += new EventHandler(RecentFileToolStripMenuItem_Click);
            g.Click += new EventHandler(RecentFileToolStripMenuItem_Click);
            f.Text = Program.GetShorterPath(it.ToString(), 50);
            g.Text = Program.GetShorterPath(it.ToString(), 50);
            f.Tag = it; g.Tag = it;
            toolStripButton2.DropDownItems.Add(g);
            недавниеФайлыToolStripMenuItem.DropDownItems.Add(f);
         }
      }

      public void AddRecentFile(string name)
      {
         if (Properties.Settings.Default.RecentFiles.Contains(name))
            Properties.Settings.Default.RecentFiles.Remove(name);
         if (Properties.Settings.Default.RecentFiles.Count >=
            Program.Opt.CountRecentFiles)
         {
            Properties.Settings.Default.RecentFiles.RemoveRange(
               Program.Opt.CountRecentFiles - 1,
               Properties.Settings.Default.RecentFiles.Count -
               Program.Opt.CountRecentFiles + 1);
         }
         Properties.Settings.Default.RecentFiles.Insert(0, name);
         SetupRecentFiles();
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
            par.ForeColor = tn.ForeColor; // на будущее
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

      public TreeNode GetHoveringNode(int x, int y)
      {
         Point pt = treeView1.PointToClient(new Point(x, y));
         TreeViewHitTestInfo hitInfo = treeView1.HitTest(pt);
         return hitInfo.Node;
      }

      public void SetLabelStatus()
      {
         if (mDB.FileState == wqFile.wqFileState.wqNone)
            toolStripStatusLabel1.Text = "Журнал не открыт";
         else
         {
            System.IO.FileInfo fi = new System.IO.FileInfo(mDB.FileName);
            NodeInfoTag nit = (NodeInfoTag)treeView1.Nodes["1"].Tag;
            String res = " (" + nit.wqCount + " записей, " +
                Program.GetShortSize((Int32)fi.Length) + ")";
            if (mDB.FileState == wqFile.wqFileState.wqNew)
               toolStripStatusLabel1.Text = "Новый журнал";
            else toolStripStatusLabel1.Text = mDB.FileName;
            toolStripStatusLabel1.Text += res;
         }
         // Navigation Panel
         if (mDB.MovingList.Count == 0)
         {
            toolStripSplitButton2.DropDownItems.Clear();
            toolStripSplitButton2.Enabled = false;
            toolStripSplitButton3.DropDownItems.Clear();
            toolStripSplitButton3.Enabled = false;
         }
      }

      public void AddAttachInTool(NodeInfoTag nId, String FileName)
      {
         ToolStripSplitButton newf = new ToolStripSplitButton();
         ToolStripItem iOpen = new ToolStripMenuItem();
         iOpen.Click += new EventHandler(открытьToolStripMenuItem1_Click);
         iOpen.Text = "Открыть";
         iOpen.Font = new Font(iOpen.Font, iOpen.Font.Style ^ FontStyle.Bold);
         newf.DropDownItems.Add(iOpen);
         ToolStripItem iOpenIn = new ToolStripMenuItem();
         iOpenIn.Click += new EventHandler(открытьВToolStripMenuItem_Click);
         iOpenIn.Text = "Открыть в...";
         newf.DropDownItems.Add(iOpenIn);
         ToolStripItem iSave = new ToolStripMenuItem();
         iSave.Click += new EventHandler(сохранитьToolStripMenuItem2_Click);
         iSave.Text = "Сохранить";
         iSave.Visible = nId.wqFlag != "shortcut";
         newf.DropDownItems.Add(iSave);
         ToolStripItem iDelete = new ToolStripMenuItem();
         iDelete.Click += new EventHandler(удалитьToolStripMenuItem1_Click);
         iDelete.Text = "Удалить";
         newf.DropDownItems.Add(iDelete);
         newf.DropDownItems.Add(new ToolStripSeparator());
         ToolStripItem iProperty = new ToolStripMenuItem();
         iProperty.Click += new EventHandler(свойстваToolStripMenuItem2_Click);
         iProperty.Text = "Свойства";
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
         if (nits.Length > 0) toolStripLabel1.Text = "Присоединенные файлы:";
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
         toolStripLabel1.Text = "Нет присоединенных файлов";
      }

      public void AddToMovingList(NodeInfoTag nit)
      {
         if (mDB.MovingList.Count > 0 && nit == mDB.MovingList[mDB.MovePos]) return;
         if (this.SearchItem(nit) == null) return;
         if (mDB.MovePos > 0 && !mDB.MoveRet)
         {
            mDB.MovingList.RemoveRange(0, mDB.MovePos);
            mDB.MovePos = 0;
         }
         mDB.MovingList.Insert(mDB.MovePos, nit);
         if (mDB.MovingList.Count > 50)
         {
            mDB.MovingList.RemoveRange(50,
               mDB.MovingList.Count - 50);
         }

         if (!mDB.MoveRet)
         {
            toolStripSplitButton3.DropDownItems.Clear();
            toolStripSplitButton3.Enabled = false;
         }
         toolStripSplitButton2.Enabled = true;
         ToolStripItem tsi = new ToolStripMenuItem(nit.wqName);
         tsi.Tag = nit;
         tsi.Click += new EventHandler(MovingLeftToolStripMenuItem_Click);
         toolStripSplitButton2.DropDownItems.Insert(0, tsi);
         while (toolStripSplitButton2.DropDownItems.Count > 10 && !mDB.MoveRet)
            toolStripSplitButton2.DropDownItems.RemoveAt(10);
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
         nit = MainJrn.GetInfoElem(nit.wqId, nit.wqType);
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
               case 0: res = "Заметка"; break;
               case 1: res = "Код"; break;
               case 2: res = "Цитата"; break;
               case 3: res = "Лог"; break;
               case 4: res = "Письмо"; break;
               case 5: res = "Отчет"; break;
            }
         }
         else res = "[Папка]";
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
         if (isload) // Если при загрузке
         {
            this.wqRichEdit1.Font = Program.Opt.FontRichEdit;
         }
         try
         {
            if (Properties.Settings.Default.RecentFiles.Count >=
               Program.Opt.CountRecentFiles)
            {
               Properties.Settings.Default.RecentFiles.RemoveRange(
                  Program.Opt.CountRecentFiles - 1,
                  Properties.Settings.Default.RecentFiles.Count -
                  Program.Opt.CountRecentFiles + 1);
            }
         }
         catch { }
         //...
      }
      #endregion

      #region События главного меню

      #region Меню "Файл"

      private void новыйЖурналToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (!TryClose()) return;
         mDB = null; wqRichEdit1.Clear();
         mDB = new wqFile();
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

      private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (openFileDialog1.ShowDialog() == DialogResult.OK)
         {
            if (mDB.FileName == openFileDialog1.FileName) return;
            this.DoOpenFile(openFileDialog1.FileName);
         }
      }

      private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
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
            NodeInfoTag bak0 = mDB.NowNode;
            List<NodeInfoTag> bak1 = mDB.MovingList;
            Int32 bak2 = mDB.MovePos;
            mDB = new wqFile();
            mDB.NowNode = bak0;
            mDB.MovingList = bak1;
            mDB.MovePos = bak2;
            mDB.FileName = res;
            mDB.FileState = wqFile.wqFileState.wqOpened;

            //treeView1.Nodes.Clear();
            //treeView1.Nodes.Add(MainJrn.LoadTreeView(FullTreeNode));
            //treeView1.ExpandAll();
         }
         catch
         {
            MessageBox.Show("Ошибка!");
         }
         this.SetLabelStatus();
      }

      private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (mDB.FileState == wqFile.wqFileState.wqNone) return;
         if (saveFileDialog1.ShowDialog() == DialogResult.OK)
         {
            mDB.FileName = saveFileDialog1.FileName;
            mDB.FileState = wqFile.wqFileState.wqOpened;
            MainJrn.IsChanged = true;
            сохранитьToolStripMenuItem_Click(sender, e);
         }
      }

      private void выходToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.Close();
      }
      #endregion

      #region Меню "Правка"

      private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null) wqRichEdit1.Undo();
      }

      private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null) wqRichEdit1.Redo();
      }

      private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null) wqRichEdit1.Cut();
      }

      private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null) wqRichEdit1.Copy();
      }

      private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null) wqRichEdit1.Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
      }

      private void вставитьКакНовуюЗаметкуToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn == null || treeView1.SelectedNode == null) return;
         if(!Clipboard.ContainsText()) return;
         TreeNode par;
         if (((NodeInfoTag)treeView1.SelectedNode.Tag).wqType ==
            NodeInfoTag.wqTypes.wqDir) par = treeView1.SelectedNode;
         else par = treeView1.SelectedNode.Parent;
         NodeInfoTag nit = (NodeInfoTag)par.Tag;

         String name = "Новая заметка #" + Properties.Settings.
             Default.LastNumberElem.ToString();
         Properties.Settings.Default.LastNumberElem++;
         nit = MainJrn.CreateNode(nit.wqId, name);
         MainJrn.SetNodeContent(nit.wqId, Program.GetRtfFromClipboard());
         TreeNode res = FullTreeNode(MainJrn.GetInfoElem(nit.wqId,
            NodeInfoTag.wqTypes.wqNode));
         par.Nodes.Add(res);
         this.RefreshTop(res);
         treeView1.SelectedNode.ExpandAll();
      }

      private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null)
         {
            wqRichEdit1.Focus();
            SendKeys.Send("{DEL}");
         }
      }

      private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (MainJrn != null) wqRichEdit1.SelectAll();
      }

      private void поискToolStripMenuItem_Click(object sender, EventArgs e)
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

      private void перейтиКПозицииToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (wqRichEdit1.Text.Length > 0)
         {
            PosForm frm = new PosForm();
            frm.wqre = wqRichEdit1;
            frm.Show(MainForm.ActiveForm);
         }
      }
      #endregion

      #region Меню "Вид"

      // показ и скрытие 6 основных элементов интерфейса 
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
      private void присоединенныеФайлыToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         toolStrip2.Visible = присоединенныеФайлыToolStripMenuItem.Checked;
      }
      private void строкаСостоянияToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
      {
         statusStrip1.Visible = строкаСостоянияToolStripMenuItem.Checked;
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
      #endregion

      #region Меню "Сервис"

      private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
      {
         OptionsForm opt = new OptionsForm();
         if (opt.ShowDialog() == DialogResult.Yes)
         {
            InitSettings(false);
         }
      }
      #endregion

      #region Меню "Справка"

      private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
      {
         AboutForm frm2 = new AboutForm();
         frm2.ShowDialog();
      }
      #endregion

      #endregion

      #region События контекстных меню

      #region Поменю "Стиль", "Приоритет"

      private void заметкаToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         this.SetNodeSchema(0);
      }

      private void кодToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         this.SetNodeSchema(1);
      }

      private void цитатаToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.SetNodeSchema(2);
      }

      private void логToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.SetNodeSchema(3);
      }

      private void письмоToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.SetNodeSchema(4);
      }

      private void отчетToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.SetNodeSchema(5);
      }

      private void высокаяToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.SetElemPriority(1);
      }

      private void средняяToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.SetElemPriority(0);
      }

      private void низкаяToolStripMenuItem_Click(object sender, EventArgs e)
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
               наУровеньВверхToolStripMenuItem.Enabled = false;
            else наУровеньВверхToolStripMenuItem.Enabled = true;
            if (treeView1.SelectedNode.PrevNode == null)
               переместитьВверхToolStripMenuItem.Enabled = false;
            else
            {
               переместитьВверхToolStripMenuItem.Enabled = true;
               np = (NodeInfoTag)treeView1.SelectedNode.PrevNode.Tag;
            }
            if (treeView1.SelectedNode.NextNode == null)
               переместитьВнизToolStripMenuItem.Enabled = false;
            else переместитьВнизToolStripMenuItem.Enabled = true;
            if (np == null || np.wqType != NodeInfoTag.wqTypes.wqDir)
               наУровеньВнизToolStripMenuItem.Enabled = false;
            else наУровеньВнизToolStripMenuItem.Enabled = true;

            if (Clipboard.ContainsData("wqNotes_node"))
               вставитьToolStripMenuItem1.Enabled = true;
            else вставитьToolStripMenuItem1.Enabled = false;

            NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
            if (nit.wqParent_id == 1)
               наУровеньВверхToolStripMenuItem.Enabled = false;
            удалитьToolStripMenuItem3.Visible = true;
            извлечьВсеToolStripMenuItem.Visible = true;
            наУровеньВверхToolStripMenuItem.Visible = true;
            наУровеньВнизToolStripMenuItem.Visible = true;
            переместитьВверхToolStripMenuItem.Visible = true;
            переместитьВнизToolStripMenuItem.Visible = true;
            if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
            {
               вырезатьToolStripMenuItem1.Visible = false;
               копироватьToolStripMenuItem1.Visible = false;
               вставитьToolStripMenuItem1.Visible = true;
               добавитьВЗакладкиToolStripMenuItem.Visible = false;
               стильToolStripMenuItem.Visible = false;
               создатьToolStripMenuItem.Visible = true;
               извлечьВсеToolStripMenuItem.Visible = true;
               if (nit.wqId == 1)
               {
                  удалитьToolStripMenuItem3.Visible = false;
                  извлечьВсеToolStripMenuItem.Visible = false;
                  наУровеньВверхToolStripMenuItem.Visible = false;
                  наУровеньВнизToolStripMenuItem.Visible = false;
                  переместитьВверхToolStripMenuItem.Visible = false;
                  переместитьВнизToolStripMenuItem.Visible = false;
               }
            }
            else
            {
               вырезатьToolStripMenuItem1.Visible = true;
               копироватьToolStripMenuItem1.Visible = true;
               вставитьToolStripMenuItem1.Visible = false;
               добавитьВЗакладкиToolStripMenuItem.Visible = true;
               стильToolStripMenuItem.Visible = true;
               создатьToolStripMenuItem.Visible = false;
               извлечьВсеToolStripMenuItem.Visible = false;

               заметкаToolStripMenuItem1.Checked = false;
               кодToolStripMenuItem1.Checked = false;
               цитатаToolStripMenuItem.Checked = false;
               логToolStripMenuItem.Checked = false;
               письмоToolStripMenuItem.Checked = false;
               отчетToolStripMenuItem.Checked = false;
               switch (nit.wqSchema)
               {
                  case 0: заметкаToolStripMenuItem1.Checked = true; break;
                  case 1: кодToolStripMenuItem1.Checked = true; break;
                  case 2: цитатаToolStripMenuItem.Checked = true; break;
                  case 3: логToolStripMenuItem.Checked = true; break;
                  case 4: письмоToolStripMenuItem.Checked = true; break;
                  case 5: отчетToolStripMenuItem.Checked = true; break;
               }
            }
            высокаяToolStripMenuItem.Checked = false;
            средняяToolStripMenuItem.Checked = false;
            низкаяToolStripMenuItem.Checked = false;
            switch (nit.wqPriority)
            {
               case 0: средняяToolStripMenuItem.Checked = true; break;
               case 1: высокаяToolStripMenuItem.Checked = true; break;
               case 2: низкаяToolStripMenuItem.Checked = true; break;
            }
         }
         else e.Cancel = true;
      }

      private void добавитьВЗакладкиToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.AddToMarkslist((NodeInfoTag)treeView1.SelectedNode.Tag);
      }

      private void папкуToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         String name = "Новая папка #" + Properties.Settings.
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

      private void заметкуToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         String name = "Новая заметка #" + Properties.Settings.
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

      private void переименоватьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         treeView1.SelectedNode.Text = nit.wqName;
         treeView1.LabelEdit = true;
         treeView1.SelectedNode.BeginEdit();
      }

      private void вырезатьToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         if (MainJrn.GetAttachList(nit.wqId).Length > 0)
         {
            if (MessageBox.Show("Внимание! У заметки есть прикрепленные файлы, " +
               "которые будут безвозвратно удалены. Продолжить?", "wqNotes",
               MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
               MessageBoxDefaultButton.Button2) == DialogResult.No)
               return;
         }
         MainJrn.CopyNode(nit);
         this.DeleteElem(treeView1.SelectedNode, true);
      }

      private void копироватьToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         MainJrn.CopyNode(nit);
      }

      private void вставитьToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         TreeNode res = FullTreeNode(MainJrn.PasteNode(nit.wqId));
         treeView1.SelectedNode.Nodes.Add(res);
         this.RefreshTop(res);
         treeView1.SelectedNode.Expand();
      }

      private void удалитьToolStripMenuItem3_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         string res = "Вы уверены, что хотите удалить ";
         if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
            res += "папку \"" + nit.wqName + "\" и все ее содержимое?";
         else res += "заметку \"" + nit.wqName + "\"?";
         if (MessageBox.Show(res, "wqNotes", MessageBoxButtons.YesNo,
             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
             == DialogResult.Yes)
         {
            if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
            {
               if (MainJrn.GetAttachList(nit.wqId).Length > 0)
                  if (MessageBox.Show("Заметка содержит присоединенные " +
                      "файлы, которые также будут удалены. Продолжить?",
                      "wqNotes", MessageBoxButtons.YesNo,
                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                      == DialogResult.No)
                     return;
            }
            this.DeleteElem(treeView1.SelectedNode, true);
         }
      }

      private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            if (MessageBox.Show("Вы уверены, что хотите очистить "
                + "содержимое заметки \"" + nit.wqName + "\"?", "wqNotes",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
               wqRichEdit1.Text = "";
            }
         }
         else
         {
            if (MessageBox.Show("Вы уверены, что хотите удалить все "
                + "содержимое папки \"" + nit.wqName + "\"?", "wqNotes",
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

      private void наУровеньВверхToolStripMenuItem_Click(object sender, EventArgs e)
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

      private void наУровеньВнизToolStripMenuItem_Click(object sender, EventArgs e)
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

      private void переместитьВверхToolStripMenuItem_Click(object sender, EventArgs e)
      {
         TreeNode tnow = treeView1.SelectedNode;
         TreeNode tpar = tnow.Parent;
         Int32 inx = tnow.PrevNode.Index;
         tnow.Remove();
         tpar.Nodes.Insert(inx, tnow);
         treeView1.SelectedNode = tnow;
      }

      private void переместитьВнизToolStripMenuItem_Click(object sender, EventArgs e)
      {
         TreeNode tnow = treeView1.SelectedNode;
         TreeNode tpar = tnow.Parent;
         Int32 inx = tnow.NextNode.Index;
         tnow.Remove();
         tpar.Nodes.Insert(inx, tnow);
         treeView1.SelectedNode = tnow;
      }

      private void извлечьВсеToolStripMenuItem_Click(object sender, EventArgs e)
      {
         TreeNode tnow = treeView1.SelectedNode;
         treeView1.BeginUpdate();
         while (tnow.Nodes.Count > 0)
         {
            treeView1.SelectedNode = tnow.Nodes[0];
            наУровеньВверхToolStripMenuItem_Click(sender, e);
         }
         treeView1.SelectedNode = tnow;
         treeView1.EndUpdate();
      }

      #region Меню аттачей

      private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
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
            MessageBox.Show("Невозможно открыть файл. Подробности:\n" +
                ex.Message, "wqNotes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (nit.wqFlag != "shortcut")
               try { System.IO.File.Delete(start); }
               catch { }
         }
      }

      private void открытьВToolStripMenuItem_Click(object sender, EventArgs e)
      {
         MessageBox.Show("Не готово!");
      }

      private void сохранитьToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).OwnerItem.Tag;
         saveFileDialog2.FileName = System.IO.Path.GetFileName(nit.wqName);
         if (saveFileDialog2.ShowDialog() == DialogResult.OK)
         {
            MainJrn.SaveAttach(nit.wqId, saveFileDialog2.FileName);
         }
      }

      private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         NodeInfoTag nit = (NodeInfoTag)((ToolStripItem)sender).OwnerItem.Tag;
         if (MessageBox.Show("Вы уверены, что хотите удалить присоединенный " +
             "файл \"" + System.IO.Path.GetFileName(nit.wqName) + "\"",
             "wqNotes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
             MessageBoxDefaultButton.Button2) == DialogResult.Yes)
         {
            MainJrn.DeleteAttach(nit.wqId);
            toolStrip2.Items.Remove(((ToolStripItem)sender).OwnerItem);
            if (toolStrip2.Items.Count == 2)
               toolStripLabel1.Text = "Нет присоединенных файлов";
            NodeInfoTag nt = (NodeInfoTag)treeView1.SelectedNode.Tag;
            treeView1.SelectedNode.Tag = MainJrn.GetInfoElem(nt.wqId, nt.wqType);
            RefreshTop(treeView1.SelectedNode);
         }
      }

      private void свойстваToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         MessageBox.Show("Не готово!");
      }

      private void toolStripButton10_ButtonClick(object sender, EventArgs e)
      {
         ToolStripSplitButton tsi = (ToolStripSplitButton)sender;
         открытьToolStripMenuItem1_Click(tsi.DropDownItems[0], e);
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
                  MessageBox.Show("Нет доступа к файлу. Подробности:\n" +
                      ex.Message, "wqNotes", MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                  return;
               }
               catch (System.IO.FileNotFoundException ex)
               {
                  MessageBox.Show("Файл не найден. Подробности:\n" +
                    ex.Message, "wqNotes", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                  return;
               }
               catch (Exception ex)
               {
                  MessageBox.Show("Неопознанная ошибка. Подробности:\n" +
                      ex.Message, "wqNotes", MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                  return;
               }
               toolStripLabel1.Text = "Присоединенные файлы:";
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

      #region Меню wqRichText
      private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
      {
         if (wqRichEdit1.CanUndo)
            отменитьToolStripMenuItem1.Enabled = true;
         else отменитьToolStripMenuItem1.Enabled = false;
         if (wqRichEdit1.CanRedo)
            повторитьToolStripMenuItem1.Enabled = true;
         else повторитьToolStripMenuItem1.Enabled = false;
         if (wqRichEdit1.SelectionLength > 0)
            вырезатьToolStripMenuItem2.Enabled = true;
         else вырезатьToolStripMenuItem2.Enabled = false;
         if (wqRichEdit1.SelectionLength > 0)
            копироватьToolStripMenuItem2.Enabled = true;
         else копироватьToolStripMenuItem2.Enabled = false;
         if (wqRichEdit1.CanPaste(DataFormats.GetFormat(DataFormats.Text)))
            вставитьToolStripMenuItem2.Enabled = true;
         else вставитьToolStripMenuItem2.Enabled = false;
         if (wqRichEdit1.Text.Length > 0)
            удалитьToolStripMenuItem4.Enabled = true;
         else удалитьToolStripMenuItem4.Enabled = false;
         if (wqRichEdit1.Text.Length > 0)
            выделитьВсеToolStripMenuItem1.Enabled = true;
         else выделитьВсеToolStripMenuItem1.Enabled = false;

         if (mDB == null || mDB.NowNode == null) e.Cancel = true;
      }

      private void отменитьToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         отменитьToolStripMenuItem_Click(sender, e);
      }

      private void повторитьToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         повторитьToolStripMenuItem_Click(sender, e);
      }

      private void копироватьToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         копироватьToolStripMenuItem_Click(sender, e);
      }

      private void вставитьToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         вставитьToolStripMenuItem_Click(sender, e);
      }

      private void выделитьВсеToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         выделитьВсеToolStripMenuItem_Click(sender, e);
      }
      #endregion

      #endregion

      #region События тулбара

      private void toolStripStatusLabel2_DoubleClick(object sender, EventArgs e)
      {
         перейтиКПозицииToolStripMenuItem_Click(sender, e);
      }

      private void toolStripSplitButton2_ButtonClick(object sender, EventArgs e)
      {
         MovingLeftToolStripMenuItem_Click(toolStripSplitButton2.DropDownItems[0], e);
      }

      private void toolStripSplitButton3_ButtonClick(object sender, EventArgs e)
      {
         MovingRightToolStripMenuItem_Click(toolStripSplitButton3.DropDownItems[0], e);
      }

      private void toolStripButton1_Click(object sender, EventArgs e)
      {
         новыйЖурналToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton2_ButtonClick(object sender, EventArgs e)
      {
         открытьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton3_Click(object sender, EventArgs e)
      {
         сохранитьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton6_Click(object sender, EventArgs e)
      {
         копироватьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton7_Click(object sender, EventArgs e)
      {
         вставитьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton9_ButtonClick(object sender, EventArgs e)
      {
         отменитьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
      {
         повторитьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton13_Click(object sender, EventArgs e)
      {
         поискToolStripMenuItem_Click(sender, e);
      }

      private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (wqRichEdit1.SelectedText == "") return;
         switch (toolStripComboBox1.SelectedItem.ToString())
         {
            case "Обычный текст":
               wqRichEdit1.SelectionFont = new Font("Lucida Console", 8, FontStyle.Regular);
               wqRichEdit1.SelectionColor = Color.FromKnownColor(KnownColor.WindowText);
               wqRichEdit1.SelectionBackColor = Color.FromKnownColor(KnownColor.Window);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Left;
               wqRichEdit1.SelectionCharOffset = 0;
               wqRichEdit1.SelectionBullet = false;
               break;
            case "Заголовок 1":
               wqRichEdit1.SelectionFont = new Font("Arial", 13, FontStyle.Bold);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Center;
               break;
            case "Заголовок 2":
               wqRichEdit1.SelectionFont = new Font("Times New Roman", 14, FontStyle.Bold);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Center;
               break;
            case "Заголовок 3":
               wqRichEdit1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Left;
               break;
            case "Список":
               wqRichEdit1.SelectionBullet = !wqRichEdit1.SelectionBullet;
               //
               break;
            case "Код":
               LoadCodeMenu();
               break;
            case "Формула":
               wqRichEdit1.SelectionFont = new Font("Times New Roman", 11, FontStyle.Italic);
               break;
            case "Примечание":
               wqRichEdit1.SelectionFont = new Font("Microsoft Sans Serif", 9, FontStyle.Italic);
               wqRichEdit1.SelectionColor = Color.Gray;
               break;
            case "Выделение":
               wqRichEdit1.SelectionBackColor = Color.Yellow;
               break;
            case "Верхний индекс":
               wqRichEdit1.SelectionCharOffset = 2;
               break;
            case "Нижний индекс":
               wqRichEdit1.SelectionCharOffset = -2;
               break;
            default:
               wqRichEdit1.SelectionFont = new Font("Lucida Console", 8, FontStyle.Regular);
               wqRichEdit1.SelectionColor = Color.FromKnownColor(KnownColor.WindowText);
               wqRichEdit1.SelectionBackColor = Color.FromKnownColor(KnownColor.Window);
               wqRichEdit1.SelectionAlignment = HorizontalAlignment.Left;
               wqRichEdit1.SelectionCharOffset = 0;
               wqRichEdit1.SelectionBullet = false;
               break;
         }
      }
      #endregion

      #region События контролов

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
         // Инициализация (убираем медведей и фубары)
         treeView1.ExpandAll();
         toolStripComboBox1.SelectedIndex = 0;
         treeView1.Nodes.Clear();
         listView2.Items.Clear();
         label10.Text = "<Не выбран элемент>";
         toolStripLabel1.Text = "Нет присоединенных файлов";
         toolStripStatusLabel1.Text = "Журнал не открыт";
         toolStripStatusLabel2.Text = "Ln 0 Col 0";
         toolStrip2.Items.Remove(toolStripButton10);
         toolStrip2.Items.Remove(toolStripButton11);
         toolStripButton12.Enabled = false;
         toolStripSplitButton2.Enabled = false;
         toolStripSplitButton3.Enabled = false;
         wqRichEdit1.wqClear();
         wqRichEdit1.ClearUndo();
         listView1.Items.Clear();
         listView1.Groups.Clear();
         ////////////////////////////
         this.Text = this.Text.Replace("%ver%", Program.wqVersion);

         if (Properties.Settings.Default.RecentFiles == null)
            Properties.Settings.Default.RecentFiles = new System.Collections.ArrayList();

         InitSettings(true);

         mDB = new wqFile();
         mDB.NodeChange = false;

         // Настройка диалогов
         saveFileDialog1.CheckFileExists = false;
         saveFileDialog1.CheckPathExists = true;
         saveFileDialog1.Title = "Сохранить журнал";
         saveFileDialog1.Filter = "Журнал wqNotes (*.wqn)|*.wqn|Все файлы|*.*";
         saveFileDialog1.FilterIndex = 0;

         openFileDialog1.CheckFileExists = true;
         openFileDialog1.CheckPathExists = true;
         openFileDialog1.Title = "Открыть журнал";
         openFileDialog1.Filter = "Журнал wqNotes (*.wqn)|*.wqn|Все файлы|*.*";
         openFileDialog1.FilterIndex = 0;

         this.Location = Properties.Settings.Default.Location;
         this.Size = Properties.Settings.Default.Size;
         this.WindowState = Properties.Settings.Default.WinState;
         //if (Properties.Settings.Default.LoadLastFile == true)
         if(Program.Opt.LoadLastFile == true)
            mDB.FileName = Properties.Settings.Default.LastFile;
         //string RecentFiles = Properties.Settings.Default.RecentFiles;

         openFileDialog1.FileName = mDB.FileName;
         //
         //1. обработать рекент файлы
         //2. Зазрузить другие настройки
         //
         SetupRecentFiles();


         if (mDB.FileName.Length > 0)
            this.DoOpenFile(mDB.FileName);
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
         //КОгда выберается новая заметка, не забыть про ClearUndo!
         if (mDB.NowNode != null && mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            if (mDB.NodeChange == true)
            {
               MainJrn.SetNodeContent(mDB.NowNode.wqId, wqRichEdit1.Rtf);
               mDB.NodeChange = false;
               if (!mDB.NodeList.Contains(mDB.NowNode.wqId))
                  mDB.NodeList.Add(mDB.NowNode.wqId);
            }
            //if (!mDB.MoveRet) 
            if (((NodeInfoTag)treeView1.SelectedNode.Tag).wqType != NodeInfoTag.wqTypes.wqDir)
               AddToMovingList(mDB.NowNode);
         }
         //mDB.MoveRet = false;
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
         //toolStripLabel1.Text = "Присоединенные файлы:";
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
         //Сохранить настройки всякие
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

         Program.Opt.Save();
         if (mDB != null) Properties.Settings.Default.LastFile = mDB.FileName;
         Properties.Settings.Default.Save();
      }

      private void wqRichEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
      {
         toolStripStatusLabel4.Text = Control.IsKeyLocked(Keys.Insert) ? "OVR" : "INS";
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

      private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
            TreeNode res = this.SearchItem(nit);
            if (res != null) treeView1.SelectedNode = res;
         }
      }

      private void treeView1_MouseClick(object sender, MouseEventArgs e)
      {
         if (treeView1.SelectedNode == null) return;
         NodeInfoTag nit = (NodeInfoTag)treeView1.SelectedNode.Tag;
         if (nit.wqType == NodeInfoTag.wqTypes.wqDir) return;
         if (Control.ModifierKeys == Keys.Control)
            добавитьВЗакладкиToolStripMenuItem_Click(sender, new EventArgs());
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
               case "часов":
                  DateFrom -= new TimeSpan(0, (int)numericUpDown1.Value, 0, 0);
                  break;
               case "суток":
                  DateFrom -= new TimeSpan((int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "недель":
                  DateFrom -= new TimeSpan(7 * (int)numericUpDown1.Value, 0, 0, 0);
                  break;
               case "месяцев":
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
               case "Кбайт": SizeFrom = (int)numericUpDown2.Value * 1024; break;
               case "Мбайт": SizeFrom = (int)numericUpDown2.Value * 1024 * 1004; break;
            }
         }
         if (checkBox7.Checked && comboBox4.Text != "")
         {
            switch (comboBox4.SelectedItem.ToString())
            {
               case "байт": SizeTo = (int)numericUpDown3.Value; break;
               case "Кбайт": SizeTo = (int)numericUpDown3.Value * 1024; break;
               case "Мбайт": SizeTo = (int)numericUpDown3.Value * 1024 * 1004; break;
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
            ListViewGroup lvg = listView1.Groups.Add("today", "Сегодня (" +
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
                  (new String[] { "Заметка", "Код", "Цитата", "Лог", 
                     "Письмо", "Отчет" }[nit.wqSchema]) }, lvg);
               lvi.Tag = nit;
               listView1.Items.Add(lvi);
            }
            //
            res = MainJrn.GetHistory(
               new TimeSpan(2, 0, 0, 0), new TimeSpan(1, 0, 0, 0));
            lvg = listView1.Groups.Add("yesterday", "Вчера (" +
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
                  (new String[] { "Заметка", "Код", "Цитата", "Лог", 
                     "Письмо", "Отчет" }[nit.wqSchema]) }, lvg);
               lvi.Tag = nit;
               listView1.Items.Add(lvi);
            }
            //
            res = MainJrn.GetHistory(
               new TimeSpan(7, 0, 0, 0), new TimeSpan(5, 0, 0, 0));
            lvg = listView1.Groups.Add("backweek", "Неделю назад (" +
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
                  (new String[] { "Заметка", "Код", "Цитата", "Лог", 
                     "Письмо", "Отчет" }[nit.wqSchema]) }, lvg);
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
            TreeNode res = this.SearchItem(nit);
            if (res != null) treeView1.SelectedNode = res;
         }
      }

      private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
      {
         e.Cancel = listView2.SelectedItems.Count == 0;
         if (!e.Cancel)
         {
            NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
            if (nit.wqType == NodeInfoTag.wqTypes.wqDir)
               копироватьСодержимоеToolStripMenuItem.Visible = false;
            else
               копироватьСодержимоеToolStripMenuItem.Visible = true;
         }
      }

      private void копироватьСодержимоеToolStripMenuItem_Click(object sender, EventArgs e)
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
               MessageBox.Show("Не удалось произвести копирование. Возможно, " +
                  "заметка была удалена.", "wqNotes", MessageBoxButtons.OK,
                  MessageBoxIcon.Exclamation);
            }
         }
         return;
      }

      private void копироватьНазваниеToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            NodeInfoTag nit = (NodeInfoTag)listView2.SelectedItems[0].Tag;
            Clipboard.SetText(nit.wqName, TextDataFormat.Text);
         }
      }

      private void копироватьПутьToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count > 0)
         {
            Clipboard.SetText(listView2.SelectedItems[0].SubItems["chPath"].Text, TextDataFormat.Text);
         }
      }

      private void удалитьИзСпискаToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (listView2.SelectedItems.Count == 0) return;
         foreach (ListViewItem lvi in listView2.SelectedItems)
            listView2.Items.Remove(lvi);
      }

      private void открытьToolStripMenuItem2_Click(object sender, EventArgs e)
      {
         if (Program.Opt.IsHideOnMinimize) this.Show();
         this.WindowState = FormWindowState.Normal;
         this.Activate();
      }

      private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         оПрограммеToolStripMenuItem_Click(sender, e);
      }

      private void выходToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         выходToolStripMenuItem_Click(sender, e);
      }

      private void добавитьИзБуфераToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // Добавить текст из буфера в конец заметки
         //String res = wqRichEdit1.Rtf; Int32 i = res.Length;
         //while (res[--i] != '}') ; res = res.Substring(0, i);
         String clp = Clipboard.GetText(TextDataFormat.Text);

         clp = "\r\n==================================\r\n" + clp;
         wqRichEdit1.AppendText(clp);
         wqRichEdit1.SelectionStart = wqRichEdit1.TextLength;
         //return;
         // Тут аццкий парсер текста в rtf
         //clp = clp.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\\par ");
         //res += "==================================\\par " + clp + "\\par}";
         //wqRichEdit1.Rtf = res;
      }

      private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
      {
         if (mDB == null || mDB.NowNode == null ||
            mDB.NowNode.wqType == NodeInfoTag.wqTypes.wqDir ||
            !Clipboard.ContainsText(TextDataFormat.Text))
            добавитьИзБуфераToolStripMenuItem.Enabled = false;
         else
            добавитьИзБуфераToolStripMenuItem.Enabled = true;
      }

      private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         if (добавитьИзБуфераToolStripMenuItem.Enabled)
         {
            добавитьИзБуфераToolStripMenuItem_Click(sender, (EventArgs)e);
            timer1.Enabled = false;
         }
      }

      private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left) timer1.Enabled = true;
      }

      private void timer1_Tick(object sender, EventArgs e)
      {
         открытьToolStripMenuItem2_Click(sender, (EventArgs)e);
         timer1.Enabled = false;
      }

      private void удалитьToolStripMenuItem4_Click(object sender, EventArgs e)
      {
         удалитьToolStripMenuItem_Click(sender, e);
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

      private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
      {
         if (((NodeInfoTag)((TreeNode)e.Item).Tag).wqId != 1)
         {
            treeView1.DoDragDrop(e.Item, DragDropEffects.All);
         }
      }

      private void treeView1_DragOver(object sender, DragEventArgs e)
      {
         TreeNode hoveringNode = GetHoveringNode(e.X, e.Y);
         TreeNode draggingNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
         e.Effect = DragDropEffects.None;

         if (hoveringNode == null) return;
         bool ok = ((NodeInfoTag)hoveringNode.Tag).wqType ==
            NodeInfoTag.wqTypes.wqDir;
         ok &= hoveringNode != draggingNode;
         ok &= draggingNode.Parent != hoveringNode;
         TreeNode tn = hoveringNode.Parent;
         while (tn != null)
         {
            ok &= tn != draggingNode;
            tn = tn.Parent;
         }
         if (!ok) return;
         e.Effect = DragDropEffects.Move;
         hoveringNode.TreeView.SelectedNode = hoveringNode;
      }

      private void treeView1_DragDrop(object sender, DragEventArgs e)
      {
         if (e.Effect == DragDropEffects.Move)
         {
            TreeNode hoveringNode = GetHoveringNode(e.X, e.Y);
            if (hoveringNode != null)
            {
               TreeNode draggingNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
               if (draggingNode != null)
               {
                  NodeInfoTag nChild = (NodeInfoTag)draggingNode.Tag;
                  NodeInfoTag nParent = (NodeInfoTag)hoveringNode.Tag;
                  MainJrn.BringRandom(nChild, nParent);
                  RefreshTop(draggingNode);
                  draggingNode.Remove();
                  hoveringNode.Nodes.Add(draggingNode);
                  RefreshTop(draggingNode);
                  treeView1.SelectedNode = draggingNode;
               }
            }
         }
      }

      private void toolStripButton5_Click(object sender, EventArgs e)
      {
         вырезатьToolStripMenuItem_Click(sender, e);
      }

      private void toolStripButton8_Click(object sender, EventArgs e)
      {
         удалитьToolStripMenuItem_Click(sender, e);
      }
   }
}