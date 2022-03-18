using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_KP_Repalce : DataObjectTable
    {
        public T_R_WO_KP_Repalce(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_KP_Repalce(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_KP_Repalce);
            TableName = "R_WO_KP_Repalce".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public  List< R_WO_KP_Repalce> GetWoRepalceKP(string WO ,OleExec DB)
        {
            List<R_WO_KP_Repalce> ret = new List<R_WO_KP_Repalce>();
            string strSql = $@"select * from R_WO_KP_Repalce where wo = '{WO}'";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_WO_KP_Repalce R = new Row_R_WO_KP_Repalce(this.DataInfo);
                //(Row_R_WO_KP_Repalce)this.ConstructRow(res.Tables[0].Rows[i]);
                R.loadData(res.Tables[0].Rows[i]);

                ret.Add(R.GetDataObject());
            }
            return ret;
        }

        public List<R_WO_KP_Repalce> GetWoRepalceKpBySkuPartno(string Sku, string Partno, string ReplaceKp, OleExec DB)
        {
            List<R_WO_KP_Repalce> ret = new List<R_WO_KP_Repalce>();
            string strSql = $@" select a.* from R_WO_KP_Repalce a,r_wo_base b where a.wo=b.workorderno and b.skuno='{Sku}' and a.partno='{Partno}' and a.repalcepartno='{ReplaceKp}' ";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_WO_KP_Repalce R = new Row_R_WO_KP_Repalce(this.DataInfo);
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R.GetDataObject());
            }
            return ret;
        }

        public bool CheckDataExist(string Wo,string Partno,string ReplacePartno,OleExec DB)
        {
            string strSql = $@" select * from  R_WO_KP_Repalce where wo = {Wo} and partno='{Partno}' and repalcepartno='{ReplacePartno}' ";
            DataSet res = DB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

    }
    public class Row_R_WO_KP_Repalce : DataObjectBase
    {
        public Row_R_WO_KP_Repalce(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_KP_Repalce GetDataObject()
        {
            R_WO_KP_Repalce DataObject = new R_WO_KP_Repalce();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.REPALCEPARTNO = this.REPALCEPARTNO;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string REPALCEPARTNO
        {
            get
            {
                return (string)this["REPALCEPARTNO"];
            }
            set
            {
                this["REPALCEPARTNO"] = value;
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
    public class R_WO_KP_Repalce
    {
        public string ID{get;set;}
        public string WO{get;set;}
        public string PARTNO{get;set;}
        public string REPALCEPARTNO{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}