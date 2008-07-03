using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace wqNotes
{
   [Serializable]
   public class Pair<T, K>
   {
      public Pair() { }
      public Pair(T first, K second)
      {
         this.First = first;
         this.Second = second;
      }
      public T First;
      public K Second;
   }

   public class wqFile
   {
      public enum wqFileState { wqNew, wqOpened, wqNone };
      public wqFileState FileState = wqFileState.wqNone;
      public String FileName = "";

      public NodeInfoTag NowNode = null;
      public Boolean NodeChange = false;
      public List<Int32> NodeList = new List<Int32>();
      public Dictionary<Int32, Int32> NodeListPos =
          new Dictionary<Int32, Int32>();

      public void Delete()
      {
         if (this.FileState == wqFileState.wqNone) return;
         try
         {
            FileInfo fi = new FileInfo(FileName);
            if (fi.Exists) fi.Delete();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }
      }
   }

   [Serializable]
   public class NodeInfoTag : Object
   {
      public enum wqTypes { wqDir, wqNode, wqAttach }
      public wqTypes wqType;
      public Int32 wqId;
      public Int32 wqParent_id;
      public String wqName;
      public DateTime wqDtc;
      public DateTime wqDtm;
      public Int32 wqPriority;
      public String wqCrypto;
      public String wqHash;
      public String wqFlag;
      public Int32 wqSize;
      public Int32 wqExSize;
      public Int32 wqSchema;
      public Int32 wqCount;
   }

   public class Journal
   {
      private DataSet wqMainDts;
      private Int32 wqLastId;
      private List<Int32> DelId = new List<Int32>();
      private Dictionary<Int32, Int32> cID = new Dictionary<Int32, Int32>();
      private Dictionary<Int32, Pair<Int32, Int32>> wqIndex =
          new Dictionary<Int32, Pair<Int32, Int32>>();
      private FileStream fxmltmps, wqfs; //Временный файл ~DBPath.wqds
      private String DBPath;
      public ToolStripProgressBar DBProcess = null;
      public Boolean IsChanged;

      #region Public members
      /// <summary>
      /// Если при загрузке файла св-во IsChanged устновлено,
      /// скорее всего программа завершилась аварийно, поэтому следует
      /// вызвать метод RecoveryDB.
      /// </summary>
      /// <param name="path"></param>
      /// <param name="dbproc"></param>
      public Journal(string path, ToolStripProgressBar dbproc)
      {
         this.DBProcess = dbproc;
         this.DBPath = path;
         FileInfo fis = new FileInfo(this.wqGetTmpName()[0]);
         FileInfo fix = new FileInfo(this.wqGetTmpName()[1]);
         if (fis.Exists || fix.Exists)
            this.IsChanged = true;
         else
            this.IsChanged = false;
      }

      ~Journal()
      {
         this.CloseDB();
      }

      public string FilePath
      {
         get { return this.DBPath; }
      }

      public void CloseDB()
      {
         if (DBPath == "") return;
         if (wqfs != null) { wqfs.Close(); wqfs = null; }
         if (fxmltmps != null) { fxmltmps.Close(); fxmltmps = null; }
         FileInfo fis = new FileInfo(this.wqGetTmpName()[0]);
         FileInfo fix = new FileInfo(this.wqGetTmpName()[1]);
         if (fis.Exists) fis.Delete();
         if (fix.Exists) fix.Delete();
      }

      public bool CreateNewDB()
      {
         this.CloseDB();
         FileInfo fid = new FileInfo(this.wqGetTmpName()[0]);
         if (fid.Exists) fid.Delete();
         fid = new FileInfo(this.wqGetTmpName()[1]);
         if (fid.Exists) fid.Delete();
         this.wqLastId = 2;
         this.DelId.Clear();
         this.cID.Clear();
         this.wqMainDts = new DataSet();
         this.wqMainDts.DataSetName = "wqStructure";
         DataTable dtb = this.wqMainDts.Tables.Add("dir");
         dtb.Columns.Add("id", typeof(Int32));
         dtb.Columns.Add("parent_id", typeof(Int32));
         dtb.Columns.Add("name", typeof(String));
         dtb.Columns.Add("dtc", typeof(DateTime));
         dtb.Columns.Add("dtm", typeof(DateTime));
         dtb.Columns.Add("priority", typeof(Int32));
         dtb.Columns.Add("crypto", typeof(String));
         dtb.Columns.Add("flag", typeof(String));
         dtb.Columns.Add("count", typeof(Int32));
         dtb.Columns.Add("size", typeof(Int32));
         dtb = this.wqMainDts.Tables.Add("node");
         dtb.Columns.Add("id", typeof(Int32));
         dtb.Columns.Add("parent_id", typeof(Int32));
         dtb.Columns.Add("name", typeof(String));
         dtb.Columns.Add("dtc", typeof(DateTime));
         dtb.Columns.Add("dtm", typeof(DateTime));
         dtb.Columns.Add("schema", typeof(Int32));
         dtb.Columns.Add("priority", typeof(Int32));
         dtb.Columns.Add("crypto", typeof(String));
         dtb.Columns.Add("flag", typeof(String));
         dtb.Columns.Add("size", typeof(Int32));
         dtb.Columns.Add("exsize", typeof(Int32));
         dtb = this.wqMainDts.Tables.Add("attach");
         dtb.Columns.Add("id", typeof(Int32));
         dtb.Columns.Add("parent_id", typeof(Int32));
         dtb.Columns.Add("name", typeof(String));
         dtb.Columns.Add("dtc", typeof(DateTime));
         dtb.Columns.Add("dtm", typeof(DateTime));
         dtb.Columns.Add("hash", typeof(String));
         dtb.Columns.Add("crypto", typeof(String));
         dtb.Columns.Add("flag", typeof(String));
         dtb.Columns.Add("size", typeof(Int32));

         this.wqMainDts.Tables["dir"].Rows.Add(new object[] {
                1,null,"root",DateTime.Now,DateTime.Now,0,null,"",1,0});
         this.wqMainDts.Tables["node"].Rows.Add(new object[] {
                2,1,"default",DateTime.Now,DateTime.Now,0,0,null,"",0,0});

         DataRelation drln = this.wqMainDts.Relations.Add("dir-dir",
             this.wqMainDts.Tables["dir"].Columns["id"],
             this.wqMainDts.Tables["dir"].Columns["parent_id"]);
         drln.Nested = true;

         drln = this.wqMainDts.Relations.Add("dir-node",
             this.wqMainDts.Tables["dir"].Columns["id"],
             this.wqMainDts.Tables["node"].Columns["parent_id"]);
         drln.Nested = true;

         drln = this.wqMainDts.Relations.Add("node-attach",
             this.wqMainDts.Tables["node"].Columns["id"],
             this.wqMainDts.Tables["attach"].Columns["parent_id"]);
         drln.Nested = true;

         string[] l = { "dir", "node", "attach" };
         string[,] w = { { "id", "name", "dtc", "dtm", "priority", 
                "crypto", "flag", "count", "size" }, 
                { "id", "name", "dtc", "dtm", "schema", "priority", 
                 "crypto", "flag", "size" }, { "id", "name", "dtc", 
                     "dtm", "hash", "crypto", "flag", "size", "parent_id" } };
         for (Int32 i = 0; i < 3; ++i) for (Int32 j = 0; j < 9; ++j)
               this.wqMainDts.Tables[l[i]].Columns[w[i, j]].ColumnMapping = MappingType.Attribute;
         this.wqMainDts.Tables["node"].Columns["exsize"].ColumnMapping = MappingType.Attribute;
         this.wqMainDts.Tables["dir"].Columns["parent_id"].ColumnMapping = MappingType.Hidden;
         this.wqMainDts.Tables["node"].Columns["parent_id"].ColumnMapping = MappingType.Hidden;
         this.wqMainDts.Tables["attach"].Columns["parent_id"].ColumnMapping = MappingType.Hidden;

         this.SetNodeContent(2, "{\\rtf1}\0");

         this.SaveDB(this.DBPath);
         return true;
      }

      public bool LoadDB()
      {
         if (!File.Exists(DBPath)) return false;
         this.cID.Clear();
         this.IsChanged = false;
         this.wqMainDts = new DataSet();
         if (fxmltmps != null) { fxmltmps.Close(); fxmltmps = null; }
         FileStream fs = new FileStream(DBPath, FileMode.Open);
         MemoryStream ms = this.wqLoadStructure(fs);
         this.wqMainDts.ReadXml(ms, XmlReadMode.ReadSchema);
         this.wqLoadIndexs(fs);
         fs.Close();
         this.wqfs = new FileStream(DBPath, FileMode.Open, FileAccess.ReadWrite);
         return true;
      }

      public bool SaveDB(string path)
      {
         if (!this.IsChanged) return true;

         Int32 count = wqMainDts.Tables["node"].Rows.Count;
         count += wqMainDts.Tables["attach"].Rows.Count;
         try { DBProcess.Value = 0; }
         catch { }
         try { DBProcess.Maximum = count + count / 20; }
         catch { }
         FileStream fsm = new FileStream(this.DBPath + "s", FileMode.Create);
         XmlTextWriter xmldb = new XmlTextWriter(fsm, System.Text.Encoding.UTF8);

         string res = "";
         xmldb.WriteStartDocument(true);
         xmldb.WriteStartElement("wqMain");
         /*xmldb.WriteAttributeString("", "");*/
         wqMainDts.WriteXml(xmldb, XmlWriteMode.WriteSchema);
         try { DBProcess.Value += count / 20; }
         catch { }
         Int32 pos = (Int32)fsm.Position;
         xmldb.WriteStartElement("wqContent");
         xmldb.WriteStartElement("infoid");
         xmldb.WriteAttributeString("lastid", this.wqLastId.ToString());
         foreach (Int32 u in this.DelId) res += u.ToString() + "|";
         xmldb.WriteString(res.TrimEnd('|'));
         xmldb.WriteEndElement();
         foreach (DataRow dr in this.wqMainDts.Tables["node"].Rows)
         {
            Int32 nId = Int32.Parse(dr["id"].ToString());
            xmldb.WriteStartElement("wqNode");
            xmldb.WriteAttributeString("id", nId.ToString());
            xmldb.WriteString(this.GetNode(nId));
            xmldb.WriteEndElement();
            try { DBProcess.Value++; }
            catch { }
         }
         foreach (DataRow dr in this.wqMainDts.Tables["attach"].Rows)
         {
            Int32 nId = Int32.Parse(dr["id"].ToString());
            xmldb.WriteStartElement("wqAttach");
            xmldb.WriteAttributeString("id", nId.ToString());
            xmldb.WriteString(this.GetAttach(nId));
            //this.SaveAttach(nId, xmldb.BaseStream);
            xmldb.WriteEndElement();
            try { DBProcess.Value++; }
            catch { }
         }
         xmldb.WriteEndElement(); //wqCtructure
         xmldb.WriteEndElement(); //wqMain
         xmldb.Close();

         if (wqfs != null) wqfs.Close();
         FileInfo fid = new FileInfo(this.DBPath + "s");
         try { File.Delete(path); }
         catch { }
         fid.MoveTo(path);
         if (fxmltmps != null) { fxmltmps.Close(); fxmltmps = null; }
         fid = new FileInfo(this.wqGetTmpName()[0]);
         if (fid.Exists) fid.Delete();
         fid = new FileInfo(this.wqGetTmpName()[1]);
         if (fid.Exists) fid.Delete();

         this.DBPath = path;
         this.wqfs = new FileStream(DBPath, FileMode.Open, FileAccess.ReadWrite);
         this.cID.Clear();
         this.IsChanged = false;
         this.wqfs.Seek(pos, SeekOrigin.Begin);
         this.wqLoadIndexs(this.wqfs);
         return true;
      }

      public NodeInfoTag[] GetAttachList(Int32 id)
      {
         DataRow[] drs = wqMainDts.Tables["node"].Select(
             "id=" + id.ToString())[0].GetChildRows("node-attach");
         NodeInfoTag[] ret = new NodeInfoTag[drs.Length];
         for (int i = 0; i < ret.Length; ++i)
            ret[i] = wqSetNit(drs[i], 2);
         return ret;
      }

      public NodeInfoTag CreateAttach(Int32 parent, string FileName, bool IsLink, DateTime dtc, DateTime dtm)
      {
         Int32 nId = this.wqGetFreeId();

         FileStream fs = new FileStream(FileName, FileMode.Open,
             FileAccess.Read, FileShare.Read, 8192);
         Int32 sz = (Int32)fs.Length;
         CRC32 crc = new CRC32();
         String hash = crc.GetCrc32(fs).ToString() + "::" + sz.ToString();
         String stype = IsLink ? "shortcut" : "base64";

         try
         {
            if (this.fxmltmps == null)
            {
               this.fxmltmps = new FileStream(this.wqGetTmpName()[0],
                   FileMode.Create, FileAccess.ReadWrite);
            }
            if (this.fxmltmps.CanSeek) this.fxmltmps.Seek(0, SeekOrigin.End);
            if (fs.CanSeek) fs.Seek(0, SeekOrigin.Begin);
            this.cID[nId] = (Int32)this.fxmltmps.Length;
            if (IsLink) { fs.Close(); throw new MethodAccessException(); }

            byte[] buf = new byte[sz];
            { fs.Read(buf, 0, sz); fs.Close(); }
            MemoryStream ms = new MemoryStream(); // DeflateStream Algorithm
            GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true);
            { gzip.Write(buf, 0, sz); gzip.Close(); }
            sz = (Int32)ms.Length;
            buf = new byte[sz];
            ms.Seek(0, SeekOrigin.Begin);
            { ms.Read(buf, 0, sz); ms.Close(); }
            String b64 = Convert.ToBase64String(buf);
            UTF8Encoding utf8 = new UTF8Encoding();
            buf = utf8.GetBytes(b64);
            sz = buf.Length;
            this.fxmltmps.Write(buf, 0, sz);
            this.fxmltmps.Flush();
         }
         catch (MethodAccessException ex) { ex.Source = ""; sz = 0; }
         catch { /*MessageBox.Show("bag");*/ }

         DataRow dr = this.wqMainDts.Tables["attach"].Rows.Add(new object[] {
                nId,parent,FileName,dtc,dtm,hash,null,stype,sz });
         NodeInfoTag ret = this.wqSetNit(dr, 2);
         dr = dr.GetParentRow("node-attach");
         dr["dtm"] = DateTime.Now;
         dr["size"] = Int32.Parse(dr["size"].ToString()) + sz;
         dr = dr.GetParentRow("dir-node");
         while (dr != null)
         {
            dr["size"] = Int32.Parse(dr["size"].ToString()) + sz;
            dr = dr.GetParentRow("dir-dir");
         }
         this.IsChanged = true;
         this.wqBackupStructure();
         return ret;
      }

      public bool SaveAttach(Int32 id, string FileName)
      {
         FileInfo fi = new FileInfo(FileName);
         FileStream fs = fi.Open(FileMode.Create);
         bool ret = this.SaveAttach(id, fs); fs.Close();

         DataRow dr = wqMainDts.Tables["attach"].Select("id=" + id.ToString())[0];
         fi.CreationTimeUtc = DateTime.Parse(dr["dtc"].ToString());
         fi.LastWriteTimeUtc = DateTime.Parse(dr["dtm"].ToString());
         return ret;
      }

      public bool SaveAttach(Int32 id, Stream fs)
      {
         byte[] buf = Convert.FromBase64String(this.GetAttach(id));
         MemoryStream ms = new MemoryStream();
         ms.Write(buf, 0, buf.Length);
         ms.Seek(0, SeekOrigin.Begin);
         GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);

         const Int32 _size = 8192;
         buf = new byte[_size];
         Int32 count = gzip.Read(buf, 0, _size);
         if (count == 0) return false;
         while (count > 0)
         {
            fs.Write(buf, 0, count);
            count = gzip.Read(buf, 0, _size);
         }
         { gzip.Close(); ms.Close(); }
         return true;
      }

      public bool DeleteAttach(Int32 id)
      {
         DataRow dr;
         bool ret = true;
         try
         {
            dr = this.wqMainDts.Tables["attach"].Select("id=" + id.ToString())[0];
            Int32 sz = Int32.Parse(dr["size"].ToString());
            DataRow dw = dr.GetParentRow("node-attach");
            dw["dtm"] = DateTime.Now;
            dw["size"] = Int32.Parse(dw["size"].ToString()) - sz;
            dw = dr.GetParentRow("dir-node");
            while (dw != null)
            {
               dw["size"] = Int32.Parse(dw["size"].ToString()) - sz;
               dw = dw.GetParentRow("dir-dir");
            }
            dr.Delete();
            this.IsChanged = true;
            this.DelId.Add(id);
            if (this.cID.ContainsKey(id))
               cID.Remove(id);
            this.wqBackupStructure();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
            ret = false;
         }
         return ret;
      }

      public string GetNode(Int32 id)
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<wq>" + wqGetContent(id, NodeInfoTag.wqTypes.wqNode) + "</wq>");
         return doc.DocumentElement.InnerText;
      }

      public bool SetSchema(Int32 id, Int32 schema)
      {
         DataRow[] dr;
         dr = wqMainDts.Tables["node"].Select("id=" + id.ToString());
         if (dr.Length != 1) return false;
         dr[0]["schema"] = schema;
         this.wqBackupStructure();
         this.IsChanged = true;
         return true;
      }

      public bool SetPriority(Int32 id, NodeInfoTag.wqTypes itype, Int32 priory)
      {
         string res = itype == NodeInfoTag.wqTypes.wqDir ? "dir" : "node";
         DataRow[] dr = wqMainDts.Tables[res].Select("id=" + id.ToString());
         if (dr.Length != 1) return false;
         dr[0]["priority"] = priory;
         this.wqBackupStructure();
         this.IsChanged = true;
         return true;
      }

      public Int32 SetNodeContent(Int32 id, string text)
      {
         UTF8Encoding utf8 = new UTF8Encoding();
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<wq></wq>");
         doc.DocumentElement.InnerText = text;
         text = doc.DocumentElement.InnerXml;
         Int32 ret = utf8.GetByteCount(text), osz = 0;
         DataRow dr = wqMainDts.Tables["node"].Select("id=" + id.ToString())[0];
         osz = Int32.Parse(dr["exsize"].ToString());
         try
         {
            if (this.fxmltmps == null)
            {
               this.fxmltmps = new FileStream(this.wqGetTmpName()[0],
                   FileMode.Create, FileAccess.ReadWrite);
            }
            Int32 seek = (Int32)this.fxmltmps.Length;
            if (this.cID.ContainsKey(id) && ret <= osz) seek = this.cID[id];
            if (this.fxmltmps.CanSeek) this.fxmltmps.Seek(seek, SeekOrigin.Begin);
            this.fxmltmps.Write(utf8.GetBytes(text), 0, ret);
            this.fxmltmps.Flush();
            this.cID[id] = seek;
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
            return -1;
         }

         this.IsChanged = true;
         dr["size"] = Int32.Parse(dr["size"].ToString()) + ret -
             Int32.Parse(dr["exsize"].ToString());
         dr["exsize"] = ret;
         dr["dtm"] = DateTime.Now;
         dr = dr.GetParentRow("dir-node");
         while (dr != null)
         {
            dr["size"] = Int32.Parse(dr["size"].ToString()) + ret - osz;
            dr = dr.GetParentRow("dir-dir");
         }
         this.wqBackupStructure();
         return ret;
      }

      /// <summary>
      /// Внимание! Сначала нужно удалить все аттачи, если они есть
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public bool DeleteNode(Int32 id)
      {
         DataRow dr;
         bool ret = true;
         try
         {
            dr = this.wqMainDts.Tables["node"].Select("id=" + id.ToString())[0];
            Int32 sz = Int32.Parse(dr["size"].ToString());
            DataRow dw = dr.GetParentRow("dir-node");
            dw["dtm"] = DateTime.Now;
            while (dw != null)
            {
               dw["count"] = Int32.Parse(dw["count"].ToString()) - 1;
               dw["size"] = Int32.Parse(dw["size"].ToString()) - sz;
               dw = dw.GetParentRow("dir-dir");
            }
            dr.Delete();
            this.IsChanged = true;
            this.DelId.Add(id);
            if (this.cID.ContainsKey(id))
               cID.Remove(id);
            this.wqBackupStructure();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
            ret = false;
         }
         return ret;
      }

      public NodeInfoTag CreateNode(Int32 parent, string name)
      {
         Int32 nId = this.wqGetFreeId();

         DataRow dr = this.wqMainDts.Tables["node"].Rows.Add(new object[] { 
                nId,parent,name,DateTime.Now,DateTime.Now,0,0,null,"",0,0 });
         if (this.SetNodeContent(nId, "") == -1) return null;
         NodeInfoTag ret = this.wqSetNit(dr, 1);
         dr = dr.GetParentRow("dir-node");
         dr["dtm"] = DateTime.Now;
         while (dr != null)
         {
            dr["count"] = Int32.Parse(dr["count"].ToString()) + 1;
            dr = dr.GetParentRow("dir-dir");
         }
         this.IsChanged = true;
         this.wqBackupStructure();
         return ret;
      }

      public bool RemoveChange(Int32 id)
      {
         // Если заметка новая, приводим ее в исходное состояние
         if (!this.wqIndex.ContainsKey(id))
            return this.SetNodeContent(id, "") != -1;

         if (!this.cID.ContainsKey(id)) return false;
         return this.cID.Remove(id);
      }

      public NodeInfoTag CreateDir(Int32 parent, string name)
      {
         Int32 nId = this.wqGetFreeId();

         DataRow dr = this.wqMainDts.Tables["dir"].Rows.Add(new object[] { 
                nId,parent,name,DateTime.Now,DateTime.Now,0,0,"",0,0 });
         NodeInfoTag ret = this.wqSetNit(dr, 0);
         dr.GetParentRow("dir-dir")["dtm"] = DateTime.Now;
         this.IsChanged = true;
         this.wqBackupStructure();
         return ret;
      }

      /// <summary>
      /// Внимание! Эту функцию следует вызывать только для пустых папок
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public bool DeleteDir(Int32 id)
      {
         DataRow dr;
         bool ret = true;
         try
         {
            dr = this.wqMainDts.Tables["dir"].Select("id=" + id.ToString())[0];
            dr.GetParentRow("dir-dir")["dtm"] = DateTime.Now;
            dr.Delete();
            this.IsChanged = true;
            this.DelId.Add(id);
            this.wqBackupStructure();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
            ret = false;
         }
         return ret;
      }

      public void Rename(NodeInfoTag nit, string name)
      {
         DataRow dr;
         string res = nit.wqType == NodeInfoTag.wqTypes.wqDir ? "dir" :
             nit.wqType == NodeInfoTag.wqTypes.wqNode ? "node" : "attach";
         dr = this.wqMainDts.Tables[res].Select("id=" + nit.wqId.ToString())[0];
         dr["name"] = name;
         if (nit.wqType == NodeInfoTag.wqTypes.wqAttach)
            dr = dr.GetParentRow("node-attach");
         else if (nit.wqType == NodeInfoTag.wqTypes.wqNode)
            dr = dr.GetParentRow("dir-node");
         else dr = dr.GetParentRow("dir-dir");
         if (dr != null) dr["dtm"] = DateTime.Now;
         this.IsChanged = true;
         this.wqBackupStructure();
      }

      public string GetAttach(Int32 id)
      {
         return this.wqGetContent(id, NodeInfoTag.wqTypes.wqAttach);
      }

      public bool BringUp(NodeInfoTag child)
      {
         DataRow OldParent, NewParent, Child;
         if (child.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            Child = wqMainDts.Tables["node"].Select("id=" + child.wqId)[0];
            if (Child == null) return false;
            OldParent = Child.GetParentRow("dir-node");
            NewParent = OldParent.GetParentRow("dir-dir");
            if (NewParent == null) return false;
            OldParent["count"] = Int32.Parse(OldParent["count"].ToString()) - 1;
         }
         else
         {
            Child = wqMainDts.Tables["dir"].Select("id=" + child.wqId)[0];
            if (Child == null) return false;
            OldParent = Child.GetParentRow("dir-dir");
            if (OldParent == null) return false;
            NewParent = OldParent.GetParentRow("dir-dir");
            if (NewParent == null) return false;
            OldParent["count"] = Int32.Parse(OldParent["count"].ToString()) -
                Int32.Parse(Child["count"].ToString());
         }
         OldParent["size"] = Int32.Parse(OldParent["size"].ToString()) -
             Int32.Parse(Child["size"].ToString());
         Child["parent_id"] = NewParent["id"];
         OldParent["dtm"] = DateTime.Now;
         NewParent["dtm"] = DateTime.Now;
         this.IsChanged = true;
         this.wqBackupStructure();
         return true;
      }

      public bool BringDown(NodeInfoTag child, NodeInfoTag newpar)
      {
         DataRow Child, NewParent;
         Int32 count = 1;
         if (child.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            Child = wqMainDts.Tables["node"].Select("id=" + child.wqId)[0];
            Child.GetParentRow("dir-node")["dtm"] = DateTime.Now;
         }
         else
         {
            Child = wqMainDts.Tables["dir"].Select("id=" + child.wqId)[0];
            count = Int32.Parse(Child["count"].ToString());
            Child.GetParentRow("dir-dir")["dtm"] = DateTime.Now;
         }
         NewParent = wqMainDts.Tables["dir"].Select("id=" + newpar.wqId)[0];
         if (NewParent == null) return false;
         NewParent["count"] = Int32.Parse(NewParent["count"].ToString()) + count;
         NewParent["size"] = Int32.Parse(NewParent["size"].ToString()) +
             Int32.Parse(Child["size"].ToString());
         Child["parent_id"] = NewParent["id"];
         NewParent["dtm"] = DateTime.Now;
         this.IsChanged = true;
         this.wqBackupStructure();
         return true;
      }

      public bool BringRandom(NodeInfoTag child, NodeInfoTag newpar)
      {
         DataRow Child, NewParent, dr;
         Int32 count = 1;
         if (child.wqType == NodeInfoTag.wqTypes.wqNode)
         {
            Child = wqMainDts.Tables["node"].Select("id=" + child.wqId)[0];
            dr = Child.GetParentRow("dir-node");
         }
         else
         {
            Child = wqMainDts.Tables["dir"].Select("id=" + child.wqId)[0];
            count = Int32.Parse(Child["count"].ToString());
            dr = Child.GetParentRow("dir-dir");
         }
         NewParent = wqMainDts.Tables["dir"].Select("id=" + newpar.wqId)[0];
         if (NewParent == null) return false;
         Int32 sz = Int32.Parse(Child["size"].ToString());
         dr["dtm"] = DateTime.Now;
         while (dr != null)
         {
            dr["size"] = Int32.Parse(dr["size"].ToString()) - sz;
            dr["count"] = Int32.Parse(dr["count"].ToString()) - count;
            dr = dr.GetParentRow("dir-dir");
         }
         dr = NewParent;
         dr["dtm"] = DateTime.Now;
         while (dr != null)
         {
            dr["size"] = Int32.Parse(dr["size"].ToString()) + sz;
            dr["count"] = Int32.Parse(dr["count"].ToString()) + count;
            dr = dr.GetParentRow("dir-dir");
         }
         Child["parent_id"] = NewParent["id"];
         this.IsChanged = true;
         this.wqBackupStructure();
         return true;
      }

      public void CopyNode(NodeInfoTag id)
      {
         Pair<NodeInfoTag, String> data = new Pair<NodeInfoTag, String>();
         data.First = id;
         data.Second = this.GetNode(id.wqId);
         Clipboard.SetData("wqNotes_node", data);
      }

      public NodeInfoTag PasteNode(Int32 parentId)
      {
         Pair<NodeInfoTag, String> data = Clipboard.GetData("wqNotes_node")
            as Pair<NodeInfoTag, String>;
         NodeInfoTag nit = data.First;
         nit.wqId = this.wqGetFreeId();
         nit.wqParent_id = parentId;
         nit.wqSize = 0; nit.wqExSize = 0;

         DataRow dr = this.wqMainDts.Tables["node"].Rows.Add(new object[] { 
                nit.wqId,nit.wqParent_id,nit.wqName,nit.wqDtc,nit.wqDtm,nit.wqSchema,
                nit.wqPriority,nit.wqCrypto,nit.wqFlag,nit.wqSize,nit.wqExSize });
         int sz = this.SetNodeContent(nit.wqId, data.Second);
         if (sz == -1) return null;
         nit.wqSize = sz; nit.wqExSize = sz;
         dr = dr.GetParentRow("dir-node");
         dr["dtm"] = DateTime.Now;
         while (dr != null)
         {
            dr["count"] = Int32.Parse(dr["count"].ToString()) + 1;
            dr["size"] = Int32.Parse(dr["size"].ToString()) + sz;
            dr = dr.GetParentRow("dir-dir");
         }
         this.IsChanged = true;
         this.wqBackupStructure();
         return nit;
      }

      public NodeInfoTag GetInfoElem(Int32 id, NodeInfoTag.wqTypes itype)
      {
         String res = ""; Int32 r = 0;
         switch (itype)
         {
            case NodeInfoTag.wqTypes.wqAttach:
               res = "attach"; r = 2; break;
            case NodeInfoTag.wqTypes.wqDir:
               res = "dir"; r = 0; break;
            case NodeInfoTag.wqTypes.wqNode:
               res = "node"; r = 1; break;
         }
         DataRow dr = wqMainDts.Tables[res].Select("id=" + id.ToString())[0];
         return this.wqSetNit(dr, r);
      }

      public delegate TreeNode wqReturnNode(NodeInfoTag mID);

      public TreeNode LoadTreeView(wqReturnNode rtnd)
      {
         try { DBProcess.Style = ProgressBarStyle.Marquee; }
         catch { }
         TreeNode ret = null;
         DataRow[] dr = this.wqMainDts.Tables["dir"].Select("id=1");
         if (dr.Length == 1)
         {
            ret = rtnd(wqSetNit(dr[0], 0));
            this.wqLoadTreeView(dr[0], ret, "dir-dir", rtnd);
            this.wqLoadTreeView(dr[0], ret, "dir-node", rtnd);
         }
         try { DBProcess.Style = ProgressBarStyle.Blocks; }
         catch { }
         return ret;
      }

      public List<NodeInfoTag> GetHistory(TimeSpan diff, TimeSpan step)
      {
         List<NodeInfoTag> ret = new List<NodeInfoTag>();
         DateTime dtmin = DateTime.Now - diff;
         DateTime dtmax = dtmin + step;
         foreach (DataRow dr in this.wqMainDts.Tables["node"].Rows)
         {
            DateTime dtm = DateTime.Parse(dr["dtm"].ToString());
            if (dtm >= dtmin && dtm <= dtmax) ret.Add(wqSetNit(dr, 1));
         }
         return ret;
      }

      public List<NodeInfoTag> Search(string cmp, bool ndir, bool nnode, bool node, bool not, bool wholeword, bool register, bool regexp, DateTime datefrom, DateTime dateto, int sizefrom, int sizeto)
      {
         List<NodeInfoTag> ret = new List<NodeInfoTag>();
         List<int> uins = new List<int>();
         List<int> notl = new List<int>();
         if (ndir)
         {
            foreach (DataRow dr in this.wqMainDts.Tables["dir"].Rows)
            {
               string tcmp = dr["name"].ToString();
               if (tcmp == "") continue;
               if (not != wqCompare(cmp, tcmp, dr, wholeword, register,
                  regexp, datefrom, dateto, sizefrom, sizeto))
               {
                  if (uins.Contains(Int32.Parse(dr["id"].ToString())))
                     continue;
                  uins.Add(Int32.Parse(dr["id"].ToString()));
                  ret.Add(wqSetNit(dr, 0));
               }
            }
         }
         if (nnode || node)
         {
            foreach (DataRow dr in this.wqMainDts.Tables["node"].Rows)
            {
               if (nnode)
               {
                  string tcmp = dr["name"].ToString();
                  if (tcmp == "") continue;
                  if (not != wqCompare(cmp, tcmp, dr, wholeword, register,
                     regexp, datefrom, dateto, sizefrom, sizeto))
                  {
                     if (uins.Contains(Int32.Parse(dr["id"].ToString())))
                        continue;
                     uins.Add(Int32.Parse(dr["id"].ToString()));
                     ret.Add(wqSetNit(dr, 1));
                  }
                  else if (not)
                  {
                     if (!notl.Contains(Int32.Parse(dr["id"].ToString())))
                        notl.Add(Int32.Parse(dr["id"].ToString()));
                  }
               }
               if (node)
               {
                  string tcmp = Program.GetTextFromRtf(this.GetNode(
                     Int32.Parse(dr["id"].ToString())));
                  if (tcmp == "") continue;
                  if (not != wqCompare(cmp, tcmp, dr, wholeword, register,
                     regexp, datefrom, dateto, sizefrom, sizeto))
                  {
                     if (uins.Contains(Int32.Parse(dr["id"].ToString())))
                        continue;
                     if (not && notl.Contains(Int32.Parse(dr["id"].ToString())))
                        continue;
                     uins.Add(Int32.Parse(dr["id"].ToString()));
                     ret.Add(wqSetNit(dr, 1));
                  }
                  else if (not && uins.Contains(Int32.Parse(dr["id"].ToString())))
                  {
                     int uin = Int32.Parse(dr["id"].ToString());
                     uins.Remove(uin);
                     foreach (NodeInfoTag it in ret) if (it.wqId == uin)
                        {
                           ret.Remove(it); break;
                        }
                  }
               }
            }
         }
         return ret;
      }
      #endregion

      #region Private members

      private Int32 wqGetFreeId()
      {
         Int32 nId = 0;
         if (DelId.Count > 0)
         {
            nId = DelId[0];
            DelId.RemoveAt(0);
         }
         else nId = ++wqLastId;
         return nId;
      }

      private bool wqCompare(string s1, string s2, DataRow dr, bool wholeword, bool register, bool regexp, DateTime datefrom, DateTime dateto, int sizefrom, int sizeto)
      {
         bool istrue = false;
         if (!regexp)
         {
            if (!register) { s1 = s1.ToLower(); s2 = s2.ToLower(); }
            string tmp = " " + s2.Substring(0, Math.Min(s1.Length, s2.Length) - 1);
            for (int i = s1.Length; i < s2.Length + 1; ++i)
            {
               bool ww = true;
               char left = i - s1.Length > 0 ? s2[i - s1.Length - 1] : ' ';
               char right = i < s2.Length ? s2[i] : ' ';
               if (left >= 'a' && left <= 'z' || left >= 'A' && left <= 'Z' ||
                   left >= 'а' && left <= 'я' || left >= 'А' && left <= 'Я' ||
                   left >= '0' && left <= '9' || left == '_') ww = false;
               if (right >= 'a' && right <= 'z' || right >= 'A' && right <= 'Z' ||
                   right >= 'а' && right <= 'я' || right >= 'А' && right <= 'Я' ||
                   right >= '0' && right <= '9' || right == '_') ww = false;

               tmp = tmp.Substring(1) + s2[i - 1].ToString();
               if ((s1 == tmp) && (!wholeword || ww))
               {
                  istrue = true; break;
               }
            }
         }
         else
         {
            if (Regex.IsMatch(s2, s1)) istrue = true;
         }
         if (!istrue) return false;
         if (datefrom != DateTime.MinValue || dateto != DateTime.MaxValue)
         {
            DateTime odate = DateTime.Parse(dr["dtm"].ToString());
            if (!(datefrom <= odate && odate <= dateto)) return false;
         }
         if (sizefrom != int.MinValue || sizeto != int.MaxValue)
         {
            Int32 osize = Int32.Parse(dr["size"].ToString());
            if (!(sizefrom <= osize && osize <= sizeto)) return false;
         }
         return true;
      }

      private void wqLoadTreeView(DataRow dr, TreeNode tnd, string relation, wqReturnNode rtnd)
      {
         foreach (DataRow drc in dr.GetChildRows(relation))
         {
            TreeNode tnc = null;
            tnc = rtnd(wqSetNit(drc, relation == "dir-dir" ? 0 : 1));
            tnd.Nodes.Add(tnc);
            if (relation == "dir-dir")
               wqLoadTreeView(drc, tnc, relation, rtnd);
            wqLoadTreeView(drc, tnc, "dir-node", rtnd);
         }
      }

      private NodeInfoTag wqSetNit(DataRow dr, int nt)
      {
         NodeInfoTag nit = new NodeInfoTag();
         nit.wqId = Int32.Parse(dr["id"].ToString());
         if (dr["parent_id"].ToString() == "")
            nit.wqParent_id = -1;
         else
            nit.wqParent_id = Int32.Parse(dr["parent_id"].ToString());
         if (dr["crypto"].ToString() != "")
            nit.wqCrypto = dr["crypto"].ToString();
         if (nt != 2) nit.wqPriority = Int32.Parse(dr["priority"].ToString());
         nit.wqName = dr["name"].ToString();
         nit.wqDtc = DateTime.Parse(dr["dtc"].ToString());
         nit.wqDtm = DateTime.Parse(dr["dtm"].ToString());
         nit.wqFlag = dr["flag"].ToString();
         nit.wqSize = Int32.Parse(dr["size"].ToString());
         switch (nt)
         {
            case 0: //dir
               nit.wqType = NodeInfoTag.wqTypes.wqDir;
               nit.wqCount = Int32.Parse(dr["count"].ToString());
               break;
            case 1: //node
               nit.wqType = NodeInfoTag.wqTypes.wqNode;
               nit.wqSchema = Int32.Parse(dr["schema"].ToString());
               nit.wqExSize = Int32.Parse(dr["exsize"].ToString());
               break;
            case 2: // attach
               nit.wqType = NodeInfoTag.wqTypes.wqAttach;
               nit.wqHash = dr["hash"].ToString();
               break;
         }
         return nit;
      }

      private MemoryStream wqLoadStructure(FileStream fs)
      {
         UTF8Encoding utf8 = new UTF8Encoding();
         MemoryStream ms = new MemoryStream();
         String wqstruc = "", res = ""; Int32 c = 0;
         try { DBProcess.Style = ProgressBarStyle.Marquee; }
         catch { }
         fs.Seek(0, SeekOrigin.Begin);
         while (!wqstruc.EndsWith("<wqStructure>"))
         {
            c = fs.ReadByte();
            if (c != 32 && c != 13 && c != 10 && c != 9)
               wqstruc += ((char)c).ToString();
            if (fs.Position == fs.Length) throw new Exception("file corrupt");
         }
         wqstruc = "<wqStructure>";
         ms.Write(utf8.GetBytes(wqstruc), 0, utf8.GetByteCount(wqstruc));
         while (res != "</wqStructure")
         {
            c = fs.ReadByte();
            ms.WriteByte((byte)c);
            res += ((char)c).ToString();
            if (res.Length > 13) res = res.Substring(1);
            if (fs.Position == fs.Length) throw new Exception("file corrupt");
         }
         while ((c = fs.ReadByte()) != '>') ; ms.WriteByte((byte)c);
         ms.Seek(0, SeekOrigin.Begin); { res = ""; }
         while (!res.EndsWith("<infoidlastid=\""))
         {
            c = fs.ReadByte();
            if (c != 32 && c != 13 && c != 10 && c != 9)
               res += ((char)c).ToString();
            if (fs.Position == fs.Length) throw new Exception("file corrupt");
         }
         res = ""; bool flag = false;
         while (!res.EndsWith("\">"))
         {
            c = fs.ReadByte();
            if (c == '/') flag = true;
            if (c != 32 && c != 13 && c != 10 && c != 9 && c != '/')
               res += ((char)c).ToString();
            if (fs.Position == fs.Length) throw new Exception("file corrupt");
         }
         this.wqLastId = Int32.Parse(res.Substring(0, res.Length - 2));
         res = ""; this.DelId.Clear();
         try { DBProcess.Style = ProgressBarStyle.Blocks; }
         catch { }
         if (flag) return ms;
         while (!res.EndsWith("</infoid"))
         {
            c = fs.ReadByte();
            if (c != '|') res += ((char)c).ToString();
            else { this.DelId.Add(Int32.Parse(res)); res = ""; }
            if (fs.Position == fs.Length) throw new Exception("file corrupt");
         }
         return ms;
      }

      private void wqLoadIndexs(FileStream fs)
      {
         Int32 count = wqMainDts.Tables["node"].Rows.Count;
         count += wqMainDts.Tables["attach"].Rows.Count;
         try { DBProcess.Style = ProgressBarStyle.Blocks; }
         catch { }
         try { DBProcess.Maximum = count; }
         catch { }
         try { DBProcess.Value = 0; }
         catch { }

         String res = "", tab = "", tap = ""; Int32 c = 0;
         this.wqIndex.Clear();
         while (fs.CanRead)
         {
            string a = "<wqNodeid=\"", b = "<wqAttachid=\"";
            while ((!res.EndsWith(a) && !res.EndsWith(b)) && fs.Position != fs.Length)
            {
               c = fs.ReadByte();
               if (c != 32 && c != 13 && c != 10 && c != 9)
                  res += ((char)c).ToString();
            }
            tab = res.EndsWith(a) ? "node" : "attach";
            tap = res.EndsWith(b) ? "size" : "exsize";
            if (fs.Position == fs.Length) break; res = "";
            while (!res.EndsWith("\">"))
            {
               c = fs.ReadByte();
               if (c != 32 && c != 13 && c != 10 && c != 9 && c != '/')
                  res += ((char)c).ToString();
               if (fs.Position == fs.Length) throw new Exception("file corrupt");
            }
            Int32 id = Int32.Parse(res.Substring(0, res.Length - 2));
            Int32 begin = (Int32)fs.Position;
            Int32 end = Int32.Parse(this.wqMainDts.Tables[tab]
                .Select("id=" + id)[0][tap].ToString());
            fs.Seek(end, SeekOrigin.Current);
            this.wqIndex.Add(id, new Pair<Int32, Int32>(begin, end));
            try { DBProcess.Value++; }
            catch { }
         }

      }

      private string[] wqGetTmpName() //[0] - данные, [1] - схема
      {
         string[] res = this.DBPath.Split('\\');
         res[res.Length - 1] = "~" + res[res.Length - 1] + ".wqd";
         string ret = String.Join("\\", res);
         return new string[] { ret + "s", ret + "x" };
      }

      private void wqBackupStructure()
      {
         FileStream fstream = new FileStream(this.wqGetTmpName()[1],
             FileMode.Create);
         XmlTextWriter xmldb = new XmlTextWriter(
             fstream, System.Text.Encoding.UTF8);
         string res = "";

         xmldb.WriteStartDocument(true);
         xmldb.WriteStartElement("wqMain");
         this.wqMainDts.WriteXml(xmldb, XmlWriteMode.WriteSchema);
         xmldb.WriteStartElement("wqInfo");
         xmldb.WriteStartElement("infoid");
         xmldb.WriteAttributeString("lastid", this.wqLastId.ToString());
         foreach (Int32 u in this.DelId) res += u.ToString() + "|";
         xmldb.WriteString(res.TrimEnd('|'));
         xmldb.WriteEndElement();
         foreach (Int32 u in this.cID.Keys)
         {
            xmldb.WriteStartElement("wqElem");
            xmldb.WriteAttributeString("id", u.ToString());
            xmldb.WriteString(this.cID[u].ToString());
         }
         xmldb.WriteEndElement();
         xmldb.WriteEndElement();
         xmldb.Close();
      }

      private string wqGetContent(Int32 id, NodeInfoTag.wqTypes it)
      {
         byte[] buf;
         UTF8Encoding utf8 = new UTF8Encoding();
         String a = it == NodeInfoTag.wqTypes.wqNode ? "node" : "attach";
         String b = it == NodeInfoTag.wqTypes.wqNode ? "exsize" : "size";
         if (this.cID.ContainsKey(id))
         {
            DataRow dr = this.wqMainDts.Tables[a].Select("id=" + id)[0];
            Int32 sz = Int32.Parse(dr[b].ToString());
            buf = new byte[sz];
            this.fxmltmps.Seek(this.cID[id], SeekOrigin.Begin);
            this.fxmltmps.Read(buf, 0, sz);
         }
         else
         {
            this.wqfs.Seek(this.wqIndex[id].First, SeekOrigin.Begin);
            buf = new byte[this.wqIndex[id].Second];
            this.wqfs.Read(buf, 0, buf.Length);
         }
         return utf8.GetString(buf);
      }
      #endregion
   }

}
