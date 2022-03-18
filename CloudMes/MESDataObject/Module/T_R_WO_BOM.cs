using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_BOM : DataObjectTable
    {
        public T_R_WO_BOM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_BOM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_BOM);
            TableName = "R_WO_BOM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckExist(string WorkOrder, string PartNo, OleExec DB)
        {
            bool isExist = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"SELECT * FROM R_WO_BOM WHERE AUFNR='{WorkOrder}' AND MATNR='{PartNo}'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {
                isExist = true;
            }
            return isExist;
        }

        public bool CheckWOExist(string WorkOrder,OleExec DB)
        {
            bool isExist = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"SELECT * FROM R_WO_BOM WHERE AUFNR='{WorkOrder}'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {
                isExist = true;
            }
            return isExist;
        }

        public List<R_WO_BOM> GetWOBOM(string WorkOrder, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_BOM>().Where(t => t.AUFNR == WorkOrder).ToList();
        }

        public List<R_WO_BOM> ReturnNewPNfromBOM(string WorkOrder, OleExec DB)
        {
            string strSql = $@"select B.* from R_WO_BASE W  
                    inner join R_WO_BOM B on W.workorderno = B.AUFNR
                    where B.MATNR not in (select distinct PARTNO from C_PARTNO_EXCEPTION where EXCEPTIONTYPE = 'NoNeedScan') 
                    and B.MATNR not in (select distinct PARTNO from C_KP_GROUP_PARTNO) 
                    and W.workorderno = '{WorkOrder}' and B.MATNR not like '%-A' ";
            List<R_WO_BOM> WOBOM = new List<R_WO_BOM>();
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_WO_BOM rowbom = new Row_R_WO_BOM(this.DataInfo);
                rowbom.loadData(res.Tables[0].Rows[i]);
                WOBOM.Add(rowbom.GetDataObject());
            }
            return WOBOM;
        }
    }
    public class Row_R_WO_BOM : DataObjectBase
    {
        public Row_R_WO_BOM(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_BOM GetDataObject()
        {
            R_WO_BOM DataObject = new R_WO_BOM();
            DataObject.ID = this.ID;
            DataObject.AUFNR = this.AUFNR;
            DataObject.IDNRK = this.IDNRK;
            DataObject.MENGE = this.MENGE;
            DataObject.MATNR = this.MATNR;
            DataObject.REVLV = this.REVLV;
            DataObject.PIDREV = this.PIDREV;
            DataObject.PINDEX = this.PINDEX;
            DataObject.POSNR = this.POSNR;
            DataObject.STUFE = this.STUFE;
            DataObject.OPMATNR = this.OPMATNR;
            DataObject.SEQNO = this.SEQNO;
            DataObject.CMATNR = this.CMATNR;
            DataObject.CMENGE = this.CMENGE;
            DataObject.PIDNRK = this.PIDNRK;
            DataObject.PMATNR = this.PMATNR;
            DataObject.FINDEX = this.FINDEX;
            DataObject.POTX1 = this.POTX1;
            DataObject.POTX2 = this.POTX2;
            return DataObject;
        }

        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
            }
        }
        public string IDNRK
        {
            get
            {
                return (string)this["IDNRK"];
            }
            set
            {
                this["IDNRK"] = value;
            }
        }
        public string MENGE
        {
            get
            {
                return (string)this["MENGE"];
            }
            set
            {
                this["MENGE"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string REVLV
        {
            get
            {
                return (string)this["REVLV"];
            }
            set
            {
                this["REVLV"] = value;
            }
        }
        public string PIDREV
        {
            get
            {
                return (string)this["PIDREV"];
            }
            set
            {
                this["PIDREV"] = value;
            }
        }
        public string PINDEX
        {
            get
            {
                return (string)this["PINDEX"];
            }
            set
            {
                this["PINDEX"] = value;
            }
        }
        public string POSNR
        {
            get
            {
                return (string)this["POSNR"];
            }
            set
            {
                this["POSNR"] = value;
            }
        }
        public string STUFE
        {
            get
            {
                return (string)this["STUFE"];
            }
            set
            {
                this["STUFE"] = value;
            }
        }
        public string OPMATNR
        {
            get
            {
                return (string)this["OPMATNR"];
            }
            set
            {
                this["OPMATNR"] = value;
            }
        }
        public string SEQNO
        {
            get
            {
                return (string)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string CMATNR
        {
            get
            {
                return (string)this["CMATNR"];
            }
            set
            {
                this["CMATNR"] = value;
            }
        }
        public string CMENGE
        {
            get
            {
                return (string)this["CMENGE"];
            }
            set
            {
                this["CMENGE"] = value;
            }
        }
        public string PIDNRK
        {
            get
            {
                return (string)this["PIDNRK"];
            }
            set
            {
                this["PIDNRK"] = value;
            }
        }
        public string PMATNR
        {
            get
            {
                return (string)this["PMATNR"];
            }
            set
            {
                this["PMATNR"] = value;
            }
        }
        public string FINDEX
        {
            get
            {
                return (string)this["FINDEX"];
            }
            set
            {
                this["FINDEX"] = value;
            }
        }
        public string POTX1
        {
            get
            {
                return (string)this["POTX1"];
            }
            set
            {
                this["POTX1"] = value;
            }
        }
        public string ENMNG
        {
            get
            {
                return (string)this["ENMNG"];
            }
            set
            {
                this["ENMNG"] = value;
            }
        }
        public string POTX2
        {
            get
            {
                return (string)this["POTX2"];
            }
            set
            {
                this["POTX2"] = value;
            }
        }
       
    }
    public class R_WO_BOM
    {
        public string ID { get; set; }
        public string AUFNR { get; set; }
        public string IDNRK { get; set; }
        public string MENGE { get; set; }
        public string MATNR { get; set; }
        public string REVLV { get; set; }
        public string PIDREV { get; set; }
        public string PINDEX { get; set; }
        public string POSNR { get; set; }
        public string STUFE { get; set; }
        public string OPMATNR { get; set; }
        public string SEQNO { get; set; }
        public string CMATNR { get; set; }
        public string CMENGE { get; set; }
        public string PIDNRK { get; set; }
        public string PMATNR { get; set; }
        public string FINDEX { get; set; }
        public string POTX1 { get; set; }
        public string POTX2 { get; set; }        
    }
}
