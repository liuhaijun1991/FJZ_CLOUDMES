using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_LOT_PACK : DataObjectTable
    {
        public T_R_LOT_PACK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LOT_PACK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LOT_PACK);
            TableName = "R_LOT_PACK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// ByPackNo加載該PackNo所在未關閉Lot內Pack列表
        /// </summary>
        /// <param name="packNo"></param>
        /// <returns></returns>
        public List<R_LOT_PACK> GetRLotPackWithWaitClose(OleExec DB, string packNo)
        {
            List<R_LOT_PACK> res = new List<R_LOT_PACK>();
            string strSql = $@" select A.* from R_LOT_PACK A,R_LOT_STATUS B  where A.LOTNO=B.LOT_NO AND B.CLOSED_FLAG='0'  AND A.PACKNO='{packNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_LOT_PACK r =(Row_R_LOT_PACK)this.NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        /// <summary>
        /// ByPackNo加載該PackNo所在未關閉Lot內Pack列表
        /// </summary>
        /// <param name="packNo"></param>
        /// <returns></returns>
        public List<R_LOT_PACK> GetRLotPackByLotNo(OleExec DB, string lotNo)
        {
            List<R_LOT_PACK> res = new List<R_LOT_PACK>();
            string strSql = $@" select * from R_LOT_PACK where LOTNO='{lotNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_LOT_PACK r = (Row_R_LOT_PACK)this.NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        /// <summary>
        /// ByPackNo加載該PackNo所在未關閉Lot信息
        /// </summary>
        /// <param name="packNo"></param>
        /// <returns></returns>
        public List<R_LOT_STATUS> GetRLotStatusWithWaitClose(OleExec DB, string packNo)
        {
            string strSql = $@" select DISTINCT B.* from R_LOT_PACK A,R_LOT_STATUS B  where A.LOTNO=B.LOT_NO AND B.CLOSED_FLAG<>'2'  AND A.PACKNO='{packNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            List<R_LOT_STATUS> res = new List<R_LOT_STATUS>();
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(DB,this.DBType);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_LOT_STATUS r = (Row_R_LOT_STATUS)tRLotStatus.NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        public bool PackNoIsOnOBASampling(string packNo,OleExec DB)
        {
            string sql = $@"select * from r_lot_pack pack,r_lot_status status where pack.lotno=status.lot_no and status.closed_flag  in ('0','1') and pack.packno='{packNo}'";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if(dt.Rows.Count>0)
            {
                return true;
            }
            else
            {
                sql = $@"select * from r_lot_pack pack,r_lot_status status,(select b.* from r_packing a,r_packing b  where a.pack_no='{packNo}' 
                        and a.parent_pack_id=b.id) p where pack.lotno=status.lot_no and status.closed_flag in ('0','1') and pack.packno=p.pack_no ";
                dt= DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public class Row_R_LOT_PACK : DataObjectBase
    {
        public Row_R_LOT_PACK(DataObjectInfo info) : base(info)
        {

        }
        public R_LOT_PACK GetDataObject()
        {
            R_LOT_PACK DataObject = new R_LOT_PACK();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.PACKNO = this.PACKNO;
            DataObject.LOTNO = this.LOTNO;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string PACKNO
        {
            get
            {
                return (string)this["PACKNO"];
            }
            set
            {
                this["PACKNO"] = value;
            }
        }
        public string LOTNO
        {
            get
            {
                return (string)this["LOTNO"];
            }
            set
            {
                this["LOTNO"] = value;
            }
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
    }
    public class R_LOT_PACK
    {
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string PACKNO{get;set;}
        public string LOTNO{get;set;}
        public string ID{get;set;}
    }
}