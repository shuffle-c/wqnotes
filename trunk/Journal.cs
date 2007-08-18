using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace wqNotes_frm
{
    public class NodeInfoTag : Object
    {
        public enum wqTypes { wqDir, wqNode, wqAttach }
        public wqTypes wqType;
        public Int32 wqId;
        public Int32 wqParent_id;
        public string wqName;
        public DateTime wqDtc;
        public DateTime wqDtm;
        public Int32 wqSchema;
        public string wqCrypto;
        public string wqHash;
        public Int32 wqFlag;
        public Int32 wqSize;
        //priority for node & count for dir
        public Int32 wqAddInfo;
    }

    public class Journal
    {
        private DataSet wqMainDts;
        private Int32 wqLastId;
        private List<Int32> DelId = new List<Int32>();
        private Dictionary<Int32, Int32> cID = new Dictionary<Int32, Int32>();
        private FileStream fxmltmps; //Временный файл ~DBPath.wqds
        public string DBPath;
        public bool IsChanged;

        //Если при загрузки файла св-во IsChanged устновлено,
        //скорее всего программа завершилась аварийно, поэтому следует
        //вызвать метод RecoveryDB.
        public Journal(string path, bool IsNew)
        {
            this.DBPath = path;
            //~DBPath.wqds - временный файл данных
            //~DBPath.wqdx - временный файл структуры
            FileInfo fis = new FileInfo(this.wqGetTmpName()[0]);
            FileInfo fix = new FileInfo(this.wqGetTmpName()[1]);
            if (fis.Exists || fix.Exists)
                this.IsChanged = true;
            else
                this.IsChanged = false;
            this.wqMainDts = new DataSet();
            if (!IsNew) this.LoadDB(this.DBPath);
            else this.CreateNewDB(this.DBPath);
        }

        ~Journal()
        {
            FileInfo fis = new FileInfo(this.wqGetTmpName()[0]);
            FileInfo fix = new FileInfo(this.wqGetTmpName()[1]);
            if(fis.Exists) fis.Delete();
            if(fix.Exists) fix.Delete();
        }

        public bool CreateNewDB(string path)
        {
            //Здесь нас не ебет, сохранен ли текущий файл
            if (this.fxmltmps != null)
            {
                this.fxmltmps.Close();
                this.fxmltmps = null;
            }
            FileInfo fid = new FileInfo(this.wqGetTmpName()[0]);
            if (fid.Exists) fid.Delete();
            fid =new FileInfo(this.wqGetTmpName()[1]);
            if(fid.Exists) fid.Delete();
            this.wqLastId = 2;
            this.DelId.Clear();
            this.cID.Clear();
            this.wqMainDts.Reset();
            this.DBPath = path;
            this.wqMainDts.DataSetName = "wqStructure";
            DataTable dtb = this.wqMainDts.Tables.Add("dir");
            dtb.Columns.Add("id", typeof(Int32));
            dtb.Columns.Add("parent_id", typeof(Int32));
            dtb.Columns.Add("name", typeof(string));
            dtb.Columns.Add("dtc", typeof(DateTime));
            dtb.Columns.Add("dtm", typeof(DateTime));
            dtb.Columns.Add("schema", typeof(Int32));
            dtb.Columns.Add("crypto", typeof(string));
            dtb.Columns.Add("flag", typeof(Int32));
            dtb.Columns.Add("count", typeof(Int32));
            dtb.Columns.Add("size", typeof(Int32));
            dtb = this.wqMainDts.Tables.Add("node");
            dtb.Columns.Add("id", typeof(Int32));
            dtb.Columns.Add("parent_id", typeof(Int32));
            dtb.Columns.Add("name", typeof(string));
            dtb.Columns.Add("dtc", typeof(DateTime));
            dtb.Columns.Add("dtm", typeof(DateTime));
            dtb.Columns.Add("schema", typeof(Int32));
            dtb.Columns.Add("priority", typeof(Int32));
            dtb.Columns.Add("crypto", typeof(string));
            dtb.Columns.Add("flag", typeof(Int32));
            dtb.Columns.Add("size", typeof(Int32));
            dtb = this.wqMainDts.Tables.Add("attach");
            dtb.Columns.Add("id", typeof(Int32));
            dtb.Columns.Add("parent_id", typeof(Int32));
            dtb.Columns.Add("name", typeof(string));
            dtb.Columns.Add("dtc", typeof(DateTime));
            dtb.Columns.Add("dtm", typeof(DateTime));
            dtb.Columns.Add("hash", typeof(string));
            dtb.Columns.Add("crypto", typeof(string));
            dtb.Columns.Add("flag", typeof(Int32));
            dtb.Columns.Add("size", typeof(Int32));

            this.wqMainDts.Tables["dir"].Rows.Add(new object[] {
                1,null,"root",DateTime.Now,DateTime.Now,0,null,0,1,12});
            this.wqMainDts.Tables["node"].Rows.Add(new object[] {
                2,1,"default",DateTime.Now,DateTime.Now,0,0,null,0,12});

            this.SetNodeContent(2, "Кревед");

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
            string[,] w = { { "id", "name", "dtc", "dtm", "schema", 
                "crypto", "flag", "count", "size" }, 
                { "id", "name", "dtc", "dtm", "schema", "priority", 
                 "crypto", "flag", "size" }, { "id", "name", "dtc", 
                     "dtm", "hash", "crypto", "flag", "size", "parent_id" } };
            for (Int32 i = 0; i < 3; ++i) for (Int32 j = 0; j < 9; ++j)
                this.wqMainDts.Tables[l[i]].Columns[w[i, j]].ColumnMapping = MappingType.Attribute;
            this.wqMainDts.Tables["dir"].Columns["parent_id"].ColumnMapping = MappingType.Hidden;
            this.wqMainDts.Tables["node"].Columns["parent_id"].ColumnMapping = MappingType.Hidden;
            this.wqMainDts.Tables["attach"].Columns["parent_id"].ColumnMapping = MappingType.Hidden;

            this.SaveDB(this.DBPath);
            return true;
        }

        // Не готово
        public NodeInfoTag[] GetAttachList(Int32 id)
        {
            NodeInfoTag[] ret = new NodeInfoTag[2];
            //return new int[]{a, b};
            return ret;
        }

        public bool LoadDB(string path)
        {
            bool ret = true;
            try
            {
                this.wqMainDts.Reset();
                this.cID.Clear();
                this.DelId.Clear();
                this.fxmltmps = null;
                this.IsChanged = false;
                this.wqMainDts.ReadXml(path, XmlReadMode.ReadSchema);
                this.wqGetFreeId();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ret = false;
            }
            return ret;
        }

        // Не готово
        public bool RecoverDB(string path)
        {
            return true;
        }

        public bool SaveDB(string path)
        {
            if (!this.IsChanged) return true;

            FileStream fsm = new FileStream(this.DBPath + "s", FileMode.Create);
            XmlTextWriter xmldb = new XmlTextWriter(fsm, System.Text.Encoding.UTF8);

            string res = "";
            xmldb.WriteStartDocument(true);
            xmldb.WriteStartElement("wqMain");
            /*xmldb.WriteAttributeString("", "");*/
            wqMainDts.WriteXml(xmldb, XmlWriteMode.WriteSchema);
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
            }
            foreach (DataRow dr in this.wqMainDts.Tables["attach"].Rows)
            {
                Int32 nId = Int32.Parse(dr["id"].ToString());
                xmldb.WriteStartElement("wqAttach");
                xmldb.WriteAttributeString("id", nId.ToString());
                xmldb.WriteString(this.GetAttach(nId, true).ToString());
                xmldb.WriteEndElement();
            }
            xmldb.WriteEndElement(); //wqCtructure
            xmldb.WriteEndElement(); //wqMain
            xmldb.Close();

            FileInfo fid = new FileInfo(this.DBPath + "s");
            fid.CopyTo(path, true);
            fid.Delete();
            if (this.fxmltmps != null) this.fxmltmps.Close();
            fid = new FileInfo(this.wqGetTmpName()[0]);
            if(fid.Exists) fid.Delete();
            fid = new FileInfo(this.wqGetTmpName()[1]);
            if(fid.Exists) fid.Delete();

            this.IsChanged = false;
            this.cID.Clear();
            return true;
        }

        public string GetNode(Int32 id)
        {
            string ret;
            if (this.cID.ContainsKey(id))
            {
                DataRow dr;
                dr = this.wqMainDts.Tables["node"].Select("id=" + id.ToString())[0];
                Int32 sz = Int32.Parse(dr["size"].ToString());
                byte[] res = new byte[sz];
                this.fxmltmps.Seek(this.cID[id], SeekOrigin.Begin);
                this.fxmltmps.Read(res, 0, sz);
                UTF8Encoding utf8 = new UTF8Encoding();
                ret = utf8.GetString(res);
            }
            else
            {
                XmlDocument xdoc = new XmlDocument();
                string res = "/wqMain/wqContent/wqNode[@id=\"_id\"]";
                res = res.Replace("_id", id.ToString());
                xdoc.Load(this.DBPath);
                ret = xdoc.SelectSingleNode(res).InnerText;
            }
            return ret;
        }

        public Int32 SetNodeContent(Int32 id, string text)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Int32 ret = utf8.GetByteCount(text), osz = 0;
            DataRow dr = wqMainDts.Tables["node"].Select("id=" + id.ToString())[0];
            osz = Int32.Parse(dr["size"].ToString());
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
            dr["size"] = ret;
            dr["dtm"] = DateTime.Now;
            dr = dr.GetParentRow("dir-node");
            while (dr != null)
            {
                dr["size"] = Int32.Parse(dr["size"].ToString()) + ret - osz;
                dr = dr.GetParentRow("dir-dir");
            }
            this.wqSaveStructure();
            return ret;
        }

        // Внимание! Сначала нужно удалить все аттачи, если они есть
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
                this.wqSaveStructure();
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
            Int32 nId = 0;
            if (DelId.Count > 0)
            {
                nId = DelId[0];
                DelId.RemoveAt(0);
            }
            else nId = ++wqLastId;

            DataRow dr = this.wqMainDts.Tables["node"].Rows.Add(new object[] { 
                nId,parent,name,DateTime.Now,DateTime.Now,0,0,null,0,0 });
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
            this.wqSaveStructure();
            return ret;
        }

        public NodeInfoTag CreateDir(Int32 parent, string name)
        {
            Int32 nId = 0;
            if (DelId.Count > 0)
            {
                nId = DelId[0];
                DelId.RemoveAt(0);
            }
            else nId = ++wqLastId;

            DataRow dr = this.wqMainDts.Tables["dir"].Rows.Add(new object[] { 
                nId,parent,name,DateTime.Now,DateTime.Now,0,0,0,0,0 });
            NodeInfoTag ret = this.wqSetNit(dr, 0);
            dr.GetParentRow("dir-dir")["dtm"] = DateTime.Now;
            this.IsChanged = true;
            this.wqSaveStructure();
            return ret;
        }

        //Внимание! Эту функцию следует вызывать только для пустых папок!
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
                this.wqSaveStructure();
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
            this.wqSaveStructure();
        }

        // Не готово
        public bool SetAtttachContent(string path)
        {
            //неважно
            return true;
        }

        // Частично готово
        public object GetAttach(Int32 id, bool IsBase64)
        {
            string ret;
            // Не base64 вернуть в виде byte[];
            if (this.cID.ContainsKey(id))
            {
                DataRow dr;
                dr = this.wqMainDts.Tables["attach"].Select("id=" + id.ToString())[0];
                Int32 sz = Int32.Parse(dr["attach"].ToString());
                byte[] res = new byte[sz];
                this.fxmltmps.Seek(this.cID[id], SeekOrigin.Begin);
                this.fxmltmps.Read(res, 0, sz);
                UTF8Encoding utf8 = new UTF8Encoding();
                ret = utf8.GetString(res);
            }
            else
            {
                XmlDocument xdoc = new XmlDocument();
                string res = "/wqMain/wqContent/wqAttach[@id=\"_id\"]";
                res = res.Replace("_id", id.ToString());
                xdoc.Load(this.DBPath);
                ret = xdoc.SelectSingleNode(res).InnerText;
            }
            return ret;
        }

        public bool BringUpAll(NodeInfoTag parent)
        {
            return true;
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
            this.wqSaveStructure();
            return true;
        }

        // Не готово
        public bool BringDown(NodeInfoTag ThisNit, NodeInfoTag ParentNit)
        {
            return false;
        }

        public TreeNode LoadTreeView()
        {
            TreeNode tnd = new TreeNode();
            DataRow[] dr = this.wqMainDts.Tables["dir"].Select("id=1");
            if (dr.Length == 1)
            {
                tnd.Tag = wqSetNit(dr[0], 0);
                tnd.Text = dr[0]["name"].ToString();
                this.wqLoadTreeView(dr[0], tnd, "dir-dir");
                this.wqLoadTreeView(dr[0], tnd, "dir-node");
            }
            return tnd;
        }

        // Возможно тут нужно добавить аргумент-функцию, которая будет
        // вызываться для каждого тринода
        private void wqLoadTreeView(DataRow dr, TreeNode tnd, string relation)
        {
            foreach (DataRow drc in dr.GetChildRows(relation))
            {
                TreeNode tnc = new TreeNode();
                tnc.Text = drc["name"].ToString();
                tnc.Tag = wqSetNit(drc, relation == "dir-dir" ? 0 : 1);
                tnd.Nodes.Add(tnc);
                if (relation == "dir-dir")
                    wqLoadTreeView(drc, tnc, relation);
                wqLoadTreeView(drc, tnc, "dir-node");
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
            nit.wqName = dr["name"].ToString();
            nit.wqDtc = DateTime.Parse(dr["dtc"].ToString());
            nit.wqDtm = DateTime.Parse(dr["dtm"].ToString());
            nit.wqSchema = Int32.Parse(dr["schema"].ToString());
            nit.wqFlag = Int32.Parse(dr["flag"].ToString());
            nit.wqSize = Int32.Parse(dr["size"].ToString());
            switch (nt)
            {
                case 0: //dir
                    nit.wqType = NodeInfoTag.wqTypes.wqDir;
                    nit.wqAddInfo = Int32.Parse(dr["count"].ToString());
                    break;
                case 1: //node
                    nit.wqType = NodeInfoTag.wqTypes.wqNode;
                    nit.wqAddInfo = Int32.Parse(dr["priority"].ToString());
                    break;
                case 2: // attach
                    nit.wqType = NodeInfoTag.wqTypes.wqAttach;
                    nit.wqHash = dr["hash"].ToString();
                    break;
            }
            return nit;
        }

        private string[] wqGetTmpName() //[0] - данные, [1] - схема
        {
            string[] res = this.DBPath.Split('\\');
            res[res.Length - 1] = "~" + res[res.Length - 1] + ".wqd";
            string ret = String.Join("\\", res);
            return new string[] { ret + "s", ret + "x" };
        }

        private bool wqGetFreeId()
        {
            XmlDocument xdoc = new XmlDocument();
            bool ret = true;
            try
            {
                xdoc.Load(this.DBPath);
                string res = "/wqMain/wqContent/infoid";
                this.wqLastId = Int32.Parse(xdoc.SelectSingleNode(res)
                    .Attributes["lastid"].Value);
                res = xdoc.SelectSingleNode(res).InnerText;
                string[] em = res.Split('|');
                foreach (string u in em) if (u != "") 
                    DelId.Add(Int32.Parse(u));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ret = false;
            }
            return ret;
        }

        private void wqSaveStructure()
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

    }

}
