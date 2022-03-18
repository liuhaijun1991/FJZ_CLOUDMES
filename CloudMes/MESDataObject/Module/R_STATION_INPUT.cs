using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_Station_Input : DataObjectTable
    {
        public T_R_Station_Input(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_Station_Input(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_Station_Input);
            TableName = "R_Station_Input".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<Row_R_Station_Input> GetRowsByStationID(string _StationID, OleExec DB)
        {
            string strsql = "";
            List<Row_R_Station_Input> RET = new List<Row_R_Station_Input>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from r_station_input where station_id = '{_StationID}' order by SEQ_NO";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Input R =(Row_R_Station_Input) this.NewRow();
                    R.loadData(res1.Tables[0].Rows[i]);
                    RET.Add(R);
                }

                return RET;

                //Row_R_Station R = (Row_R_Station)this.GetObjByID(ID, DB);
                //return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<R_Station_Input> GetRowsByStationID(string _StationID, OleExec DB ,bool Retr)
        {
            string strsql = "";
            List<R_Station_Input> RET = new List<R_Station_Input>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from r_station_input where station_id = '{_StationID}' order by SEQ_NO";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Input R = (Row_R_Station_Input)this.NewRow();
                    R.loadData(res1.Tables[0].Rows[i]);
                    RET.Add(R.GetDataObject());
                }

                return RET;

                //Row_R_Station R = (Row_R_Station)this.GetObjByID(ID, DB);
                //return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public bool CheckDataExistByID(string ID, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where ID ='{ID}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }

        public List<R_Station_Input> GetFstInput(string station_name, OleExec DB)
        {
            string strsql = "";
            List<R_Station_Input> RET = new List<R_Station_Input>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select a.* from r_station_input a,c_input b,r_station c
                                  where A.STATION_ID=c.id and A.INPUT_ID=b.id and 
                                        b.DISPLAY_TYPE='TXT' and  c.DISPLAY_STATION_NAME='{station_name}' order by SEQ_NO ";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Input R = (Row_R_Station_Input)this.NewRow();
                    R.loadData(res1.Tables[0].Rows[i]);
                    RET.Add(R.GetDataObject());
                }

                return RET;

                //Row_R_Station R = (Row_R_Station)this.GetObjByID(ID, DB);
                //return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        //public void DeleteByRStationID(string RStationID,OleExec DB)
        //{
        //    string sql = string.Empty;
        //    sql = $@"delete r_station_input where station_id='{RStationID}'";
        //}

    }
    public class Row_R_Station_Input : DataObjectBase
    {
        public Row_R_Station_Input(DataObjectInfo info) : base(info)
        {

        }
        public R_Station_Input GetDataObject()
        {
            R_Station_Input DataObject = new R_Station_Input();
            DataObject.ID = this.ID;
            DataObject.STATION_ID = this.STATION_ID;
            DataObject.INPUT_ID = this.INPUT_ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.REMEMBER_LAST_INPUT = this.REMEMBER_LAST_INPUT;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.SCAN_FLAG = this.SCAN_FLAG;
            DataObject.DISPLAY_NAME = this.DISPLAY_NAME;
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
        public string STATION_ID
        {
            get
            {
                return (string)this["STATION_ID"];
            }
            set
            {
                this["STATION_ID"] = value;
            }
        }
        public string INPUT_ID
        {
            get
            {
                return (string)this["INPUT_ID"];
            }
            set
            {
                this["INPUT_ID"] = value;
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
        public string REMEMBER_LAST_INPUT
        {
            get
            {
                return (string)this["REMEMBER_LAST_INPUT"];
            }
            set
            {
                this["REMEMBER_LAST_INPUT"] = value;
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
        public double? SCAN_FLAG
        {
            get
            {
                return (double?)this["SCAN_FLAG"];
            }
            set
            {
                this["SCAN_FLAG"] = value;
            }
        }
        public string DISPLAY_NAME
        {
            get
            {
                return (string) this["DISPLAY_NAME"];
            }
            set
            {
                this["DISPLAY_NAME"] = value;
            }
        }
    }
    public class R_Station_Input
    {
        public string ID{get;set;}
        public string STATION_ID{get;set;}
        public string INPUT_ID{get;set;}
        public double? SEQ_NO{get;set;}
        public string REMEMBER_LAST_INPUT{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public double? SCAN_FLAG{get;set;}
        public string DISPLAY_NAME{get;set;}
    }
}