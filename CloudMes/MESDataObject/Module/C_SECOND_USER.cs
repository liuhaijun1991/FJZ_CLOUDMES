using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_SECOND_USER : DataObjectTable
    {
        public T_C_SECOND_USER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SECOND_USER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SECOND_USER);
            TableName = "C_SECOND_USER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_SECOND_USER> GetEmpPwdSelect(string EMP, string PWD, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == EMP && t.EMP_PASSWORD == PWD).ToList();
        }
        public List<C_SECOND_USER> CheckIsNO(string EMP_NO, string STATION_NAME, string STATION_ITEM, string MAC_ADDRESS, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == EMP_NO && t.STATION_NAME == STATION_NAME && t.STATION_ITEM == STATION_ITEM && t.MAC_ADDRESS == MAC_ADDRESS).ToList();
        }
        public List<C_SECOND_USER> GetByEMPNO(string eMPNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == eMPNO).ToList();
        }
        public List<C_SECOND_USER> GetById(string Id, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.ID == Id).ToList();
        }

        public List<C_SECOND_USER> GetAllUser(OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Asc).ToList();
        }
        /// <summary>
        /// 添加二級權限賬號
        /// </summary>
        /// <param name="newRework"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public int AddEMPNO(C_SECOND_USER NewMPNO, OleExec DB)
        {
            Row_C_SECOND_USER NewEMPNO = (Row_C_SECOND_USER)NewRow();
            NewEMPNO.ID = NewMPNO.ID;
            NewEMPNO.EMP_NO = NewMPNO.EMP_NO;
            NewEMPNO.EMP_PASSWORD = NewMPNO.EMP_PASSWORD;
            NewEMPNO.STATION_NAME = NewMPNO.STATION_NAME;
            NewEMPNO.STATION_ITEM = NewMPNO.STATION_ITEM;
            NewEMPNO.MAC_ADDRESS = NewMPNO.MAC_ADDRESS;
            NewEMPNO.EDIT_TIME = NewMPNO.EDIT_TIME;
            NewEMPNO.EDIT_EMP = NewMPNO.EDIT_EMP;
            int result = DB.ExecuteNonQuery(NewEMPNO.GetInsertString(DBType), CommandType.Text);
            return result;
        }

        /// <summary>
        /// 修改信息通過賬號
        /// </summary>
        /// <param name="newReworkWo"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public int UpdateById(C_SECOND_USER NewMPNO, OleExec DB)
        {
            Row_C_SECOND_USER NewEMPNO = (Row_C_SECOND_USER)NewRow();
            NewEMPNO.ID = NewMPNO.ID;
            NewEMPNO.EMP_NO = NewMPNO.EMP_NO;
            NewEMPNO.EMP_PASSWORD = NewMPNO.EMP_PASSWORD;
            NewEMPNO.STATION_NAME = NewMPNO.STATION_NAME;
            NewEMPNO.STATION_ITEM = NewMPNO.STATION_ITEM;
            NewEMPNO.MAC_ADDRESS = NewMPNO.MAC_ADDRESS;
            NewEMPNO.EDIT_TIME = NewMPNO.EDIT_TIME;
            NewEMPNO.EDIT_EMP = NewMPNO.EDIT_EMP;
            int result = DB.ExecuteNonQuery(NewEMPNO.GetUpdateString(DBType, NewEMPNO.ID), CommandType.Text);
            return result;
        }

        /// <summary>
        /// 刪除信息通過ID
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"delete c_second_user where ID=:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

        public List<C_SECOND_USER> GetEmpStationItemSelect(string EMP, string StationItem, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == EMP && t.STATION_ITEM == StationItem).ToList();
        }

        public List<string> GetEMPByStation(string StationName, string StationItem, OleExec DB, DB_TYPE_ENUM DBType)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.STATION_NAME == StationName && t.STATION_ITEM == StationItem).Select(t => t.EMP_NO).ToList();
            }
            else
            {
                string sql = $@"select '' as UserID union SELECT distinct UserID from qstationset(nolock) where station = '{StationName}'AND stationitem = '{StationItem}' order by UserID";

                DataSet dsReceiveEMP = DB.ExecSelect(sql);
                List<string> EMPList = new List<string>();

                foreach (DataRow row in dsReceiveEMP.Tables[0].Rows)
                {
                    EMPList.Add(row["UserID"].ToString());
                }
                return EMPList;
            }
        }
        public List<C_SECOND_USER> GetEmpStationNameSelect(string EMP, string StationName, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == EMP && t.STATION_NAME == StationName).ToList();
        }

        public List<C_SECOND_USER> GetEmpPWDStationNameItemSelect(string EMP, string PWD, string StationName, string StationItem, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == EMP && t.EMP_PASSWORD == PWD && t.STATION_NAME == StationName && (t.STATION_ITEM == "ALL" || t.STATION_ITEM == StationItem)).ToList();
        }

        public List<C_SECOND_USER> GetEmpStationNameItemSelect(string EMP, string StationName, string StationItem, OleExec DB)
        {
            return DB.ORM.Queryable<C_SECOND_USER>().Where(t => t.EMP_NO == EMP && t.STATION_NAME == StationName && (t.STATION_ITEM == "ALL" || t.STATION_ITEM == StationItem)).ToList();
        }
    }
    public class Row_C_SECOND_USER : DataObjectBase
    {
        public Row_C_SECOND_USER(DataObjectInfo info) : base(info)
        {

        }
        public C_SECOND_USER GetDataObject()
        {
            C_SECOND_USER DataObject = new C_SECOND_USER();
            DataObject.ID = this.ID;
            DataObject.EMP_NO = this.EMP_NO;
            DataObject.EMP_PASSWORD = this.EMP_PASSWORD;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.STATION_ITEM = this.STATION_ITEM;
            DataObject.MAC_ADDRESS = this.MAC_ADDRESS;
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
        public string EMP_NO
        {
            get
            {
                return (string)this["EMP_NO"];
            }
            set
            {
                this["EMP_NO"] = value;
            }
        }
        public string EMP_PASSWORD
        {
            get
            {
                return (string)this["EMP_PASSWORD"];
            }
            set
            {
                this["EMP_PASSWORD"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string STATION_ITEM
        {
            get
            {
                return (string)this["STATION_ITEM"];
            }
            set
            {
                this["STATION_ITEM"] = value;
            }
        }
        public string MAC_ADDRESS
        {
            get
            {
                return (string)this["MAC_ADDRESS"];
            }
            set
            {
                this["MAC_ADDRESS"] = value;
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
    public class C_SECOND_USER
    {
        public string ID { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_PASSWORD { get; set; }
        public string STATION_NAME { get; set; }
        public string STATION_ITEM { get; set; }
        public string MAC_ADDRESS { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}