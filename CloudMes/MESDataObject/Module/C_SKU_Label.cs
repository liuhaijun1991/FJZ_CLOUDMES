using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_Label : DataObjectTable
    {
        public T_C_SKU_Label(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_Label(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_Label);
            TableName = "C_SKU_Label".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }



        public List<C_SKU_Label> GetLabelConfigBySkuStation(string Skuno,string Station,OleExec DB)
        {
            //List<Row_C_SKU_Label> ret = new List<Row_C_SKU_Label>();
            //string strSql = $@"select * from c_sku_label where skuno='{Skuno}' and station='{Station}'";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    Row_C_SKU_Label R =(Row_C_SKU_Label) NewRow();
            //    R.loadData(res.Tables[0].Rows[i]);
            //    ret.Add(R);
            //}

            //return ret;
            return DB.ORM.Queryable<C_SKU_Label>().Where(t => t.SKUNO == Skuno && t.STATION == Station).OrderBy(t=>t.EDIT_TIME,SqlSugar.OrderByType.Desc).ToList();
        }
        public List<C_SKU_Label> GetLabelConfigBySku(string Skuno,  OleExec DB)
        {
            //List<Row_C_SKU_Label> ret = new List<Row_C_SKU_Label>();
            //string strSql = $@"select * from c_sku_label where skuno='{Skuno}'";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    Row_C_SKU_Label R = (Row_C_SKU_Label)NewRow();
            //    R.loadData(res.Tables[0].Rows[i]);
            //    ret.Add(R);
            //}

            //return ret;
            return DB.ORM.Queryable<C_SKU_Label>().Where(t => t.SKUNO == Skuno).OrderBy(t => t.LABELTYPE, SqlSugar.OrderByType.Asc).ToList();
        }

        public List<C_SKU_Label> GetLabelConfigByWo(string WorkOrder,string Station, OleExec DB,bool IsPanel=false)
        {
            T_R_WO_BASE T_RWB = new T_R_WO_BASE(DB, DB_TYPE_ENUM.Oracle);
            R_WO_BASE WoBase = T_RWB.GetWoByWoNo(WorkOrder, DB);
            if (WoBase != null)
            {
                return GetLabelConfigBySkuStation(WoBase.SKUNO, IsPanel?"PANEL_"+Station:Station, DB);
            }
            else
            {
                return null;
            }
        }

        public C_SKU_Label GetLabelConfigBySN(string Sn, string Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_REGION_DETAIL, R_WO_BASE, C_SKU_Label>
                ((wrd, wb, sl) => wrd.WORKORDERNO == wb.WORKORDERNO && wb.SKUNO == sl.SKUNO)
                .Where((wrd, wb, sl) => wrd.SN == Sn && sl.STATION == Station)
                .Select((wrd, wb, sl) => sl).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 查詢該變更工號的配置信息
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU_Label> GetbyEditLab(string Editby, OleExec DB)
        {
            List<C_SKU_Label> CLabDet = new List<C_SKU_Label>();
            string sql = string.Empty;
            DataTable dt = new DataTable("GetbyEditLab");
            Row_C_SKU_Label LabRow = (Row_C_SKU_Label)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT * FROM C_SKU_LABEL where EDIT_EMP='{Editby}' ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    LabRow.loadData(dr);
                    CLabDet.Add(LabRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return CLabDet;
        }

        /// <summary>
        /// Update變更權限所屬工號,A工號配置的sku信息轉移到B工號
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="Editchage"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpLabEdit(string Editby, string Editchage, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (Editby.Length > 0)
                {
                    sql = $@" update C_SKU_LABEL set EDIT_EMP='{Editchage}' where EDIT_EMP='{Editby}'";
                    result = DB.ExecSqlNoReturn(sql, null);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

    }
    public class Row_C_SKU_Label : DataObjectBase
    {
        public Row_C_SKU_Label(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_Label GetDataObject()
        {
            C_SKU_Label DataObject = new C_SKU_Label();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION = this.STATION;
            DataObject.SEQ = this.SEQ;
            DataObject.QTY = this.QTY;
            DataObject.LABELNAME = this.LABELNAME;
            DataObject.LABELTYPE = this.LABELTYPE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string LABELNAME
        {
            get
            {
                return (string)this["LABELNAME"];
            }
            set
            {
                this["LABELNAME"] = value;
            }
        }
        public string LABELTYPE
        {
            get
            {
                return (string)this["LABELTYPE"];
            }
            set
            {
                this["LABELTYPE"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class C_SKU_Label
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string STATION{get;set;}
        public double? SEQ{get;set;}
        public double? QTY{get;set;}
        public string LABELNAME{get;set;}
        public string LABELTYPE{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}