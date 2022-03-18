using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ACTION_PARA : DataObjectTable
    {
        public T_C_ACTION_PARA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ACTION_PARA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ACTION_PARA);
            TableName = "C_ACTION_PARA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckDataExist(string StationActionID,String Name ,OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where C_STATION_ACTION_ID ='{StationActionID}' and NAME='{Name}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

   

        public List<C_ACTION_PARA> QueryActionPara(string ID,string StationActionID, string Name, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_ACTION_PARA> LanguageList = new List<C_ACTION_PARA>();
            sql = $@"SELECT * FROM C_ACTION_PARA where 1=1 ";
            if (ID != "")
            {
                sql = sql + $@" AND ID = '{ID}'";
            }
            if (StationActionID != "")
            {
                sql = sql + $@" AND C_STATION_ACTION_ID = '{StationActionID}'";
            }
            if (Name != "")
            {
                sql = sql + $@" AND NAME = '{Name}' ";
            }
            sql = sql + " ORDER BY C_STATION_action_id,seq_no";

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateActionParaClass(dr));
            }
            return LanguageList;
        }

        public void DeleteByActionID( string StationActionID,  OleExec DB)
        {
            string sql = string.Empty;
            sql = $@"DELETE  C_ACTION_PARA where C_STATION_ACTION_ID='{StationActionID}' ";
            DB.ExecSQL(sql);
        }
        public C_ACTION_PARA CreateActionParaClass(DataRow dr)
        {
            Row_C_ACTION_PARA row = (Row_C_ACTION_PARA)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        public List<C_ACTION_PARA> QueryActionPara(string InputID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_ACTION_PARA> paralist = new List<C_ACTION_PARA>();
            sql = $@"select b.* from  R_Input_Action a, C_Action_Para  b where a.C_STATION_ACTION_ID= b.C_STATION_ACTION_ID and  A.input_id='{InputID}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                paralist.Add(CreateActionParaClass(dr));
            }
          
            return paralist;
        }

        public List<C_ACTION_PARA> QueryActionParaByStation(string ActionID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_ACTION_PARA> paralist = new List<C_ACTION_PARA>();
            //  sql = $@"select b.* from  R_Station_Action a, C_Action_Para  b where a.C_STATION_ACTION_ID= b.C_STATION_ACTION_ID and  A.R_STATION_INPUT_ID='{InputID}' ";
            sql = $@"select * from c_action_para where c_station_action_id='{ActionID}' order by seq_no";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                paralist.Add(CreateActionParaClass(dr));
            }

            return paralist;
        }

    }
    public class Row_C_ACTION_PARA : DataObjectBase
    {
        public Row_C_ACTION_PARA(DataObjectInfo info) : base(info)
        {

        }
        public C_ACTION_PARA GetDataObject()
        {
            C_ACTION_PARA DataObject = new C_ACTION_PARA();
            DataObject.ID = this.ID;
            DataObject.C_STATION_ACTION_ID = this.C_STATION_ACTION_ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string C_STATION_ACTION_ID
        {
            get
            {
                return (string)this["C_STATION_ACTION_ID"];
            }
            set
            {
                this["C_STATION_ACTION_ID"] = value;
            }
        }
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
    }
    public class C_ACTION_PARA
    {
        public string ID{get;set;}
        public string C_STATION_ACTION_ID{get;set;}
        public double? SEQ_NO{get;set;}
        public string NAME{get;set;}
        public string DESCRIPTION{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}