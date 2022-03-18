using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_C_REPAIR_USER_CONTROL : DataObjectTable
    {
        public T_C_REPAIR_USER_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REPAIR_USER_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REPAIR_USER_CONTROL);
            TableName = "C_REPAIR_USER_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_REPAIR_USER_CONTROL> GetEMPByExists(string EMP, OleExec DB)
        {
            return DB.ORM.Queryable<C_REPAIR_USER_CONTROL>().Where(t => t.CONTROL_EMP_NO == EMP).ToList();
        }
        public List<C_REPAIR_USER_CONTROL> GetByTYPE(string TYPE, OleExec DB)
        {
            return DB.ORM.Queryable<C_REPAIR_USER_CONTROL>().Where(t => t.CONTROL_TYPE == TYPE).ToList();
        }
        public List<C_REPAIR_USER_CONTROL> GetByName(string Name, OleExec DB)
        {
            return DB.ORM.Queryable<C_REPAIR_USER_CONTROL>().Where(t => t.CONTROL_EMP_NO == Name).ToList();
        }
        public List<C_REPAIR_USER_CONTROL> GetBValidControid(string Valid, OleExec DB)
        {
            return DB.ORM.Queryable<C_REPAIR_USER_CONTROL>().Where(t => t.VALID_FLAG == Valid).OrderBy(t => t.CONTROL_ID, OrderByType.Desc).ToList();
        }
        public List<C_REPAIR_USER_CONTROL> GetEMPByExistsANDQTYTYPE(string EMP, double? QTY, string TYPE, OleExec DB)
        {
            return DB.ORM.Queryable<C_REPAIR_USER_CONTROL>().Where(t => t.CONTROL_EMP_NO == EMP && t.CONTROL_QTY > QTY && t.CONTROL_TYPE == TYPE).ToList();
        }
        public List<C_REPAIR_USER_CONTROL> GetBySNCheckIN(string RepairId, OleExec DB)
        {
            string StrSQL = $@"SELECT A.CONTROL_ID FROM 
    (SELECT * FROM C_REPAIR_USER_CONTROL WHERE ROWNUM=1) A
    LEFT JOIN (
    SELECT A.IN_RECEIVE_EMP AS IN_RECEIVE_EMP,SUM(1) AS QTY  FROM R_REPAIR_TRANSFER A  
    WHERE NOT EXISTS(SELECT * FROM R_REPAIR_ACTION WHERE SN=A.SN AND  EDIT_TIME>=A.IN_TIME )  
    AND NOT EXISTS(SELECT * FROM R_REPAIR_OFFLINE WHERE SN=A.SN AND EDIT_TIME>=A.IN_TIME )  
    AND SUBSTRC(A.SN,1)<>'#' AND A.CLOSED_FLAG='1' AND A.DESCRIPTION NOT LIKE 'is sub_unit;main_unit:%'  
    GROUP BY A.IN_RECEIVE_EMP 
    ) B   
    ON A.CONTROL_EMP_NO=B.IN_RECEIVE_EMP     
    WHERE  NVL(B.QTY,'0')<A.CONTROL_QTY  AND A.CONTROL_ID>'{RepairId}' AND A.VALID_FLAG='1'   
    ORDER BY  A.CONTROL_ID ASC ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<C_REPAIR_USER_CONTROL> listSn = new List<C_REPAIR_USER_CONTROL>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_C_REPAIR_USER_CONTROL ret = (Row_C_REPAIR_USER_CONTROL)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
        public List<C_REPAIR_USER_CONTROL> GetBySNCheckINSmall(string RepairId, OleExec DB)
        {
            string StrSQL = $@"SELECT A.CONTROL_ID FROM 
    (SELECT * FROM C_REPAIR_USER_CONTROL WHERE ROWNUM=1) A
    LEFT JOIN (
    SELECT A.IN_RECEIVE_EMP AS IN_RECEIVE_EMP,SUM(1) AS QTY  FROM R_REPAIR_TRANSFER A  
    WHERE NOT EXISTS(SELECT * FROM R_REPAIR_ACTION WHERE SN=A.SN AND  EDIT_TIME>=A.IN_TIME )  
    AND NOT EXISTS(SELECT * FROM R_REPAIR_OFFLINE WHERE SN=A.SN AND EDIT_TIME>=A.IN_TIME )  
    AND SUBSTRC(A.SN,1)<>'#' AND A.CLOSED_FLAG='1' AND A.DESCRIPTION NOT LIKE 'is sub_unit;main_unit:%'  
    GROUP BY A.IN_RECEIVE_EMP 
    ) B   
    ON A.CONTROL_EMP_NO=B.IN_RECEIVE_EMP     
    WHERE  NVL(B.QTY,'0')<A.CONTROL_QTY  AND A.CONTROL_ID<'{RepairId}' AND A.VALID_FLAG='1'   
    ORDER BY  A.CONTROL_ID ASC ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<C_REPAIR_USER_CONTROL> listSn = new List<C_REPAIR_USER_CONTROL>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_C_REPAIR_USER_CONTROL ret = (Row_C_REPAIR_USER_CONTROL)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
        public List<C_REPAIR_USER_CONTROL> GetBySNCheckINOutStrike2(string EMP, OleExec DB)
        {
            string StrSQL = $@"SELECT * FROM C_REPAIR_USER_CONTROL 
   WHERE  CONTROL_EMP_NO NOT LIKE '#%' AND CONTROL_TYPE='AcceptRepairID' AND CONTROL_EMP_NO='{EMP}' ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<C_REPAIR_USER_CONTROL> listSn = new List<C_REPAIR_USER_CONTROL>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_C_REPAIR_USER_CONTROL ret = (Row_C_REPAIR_USER_CONTROL)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
        public List<C_REPAIR_USER_CONTROL> GetBySNCheckINOutStrike(string EMP, OleExec DB)
        {
            string StrSQL = $@"SELECT * FROM C_REPAIR_USER_CONTROL 
   WHERE  CONTROL_EMP_NO NOT LIKE '#%' AND CONTROL_TYPE='ACCEPTREPAIRID' AND CONTROL_EMP_NO='{EMP}' ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<C_REPAIR_USER_CONTROL> listSn = new List<C_REPAIR_USER_CONTROL>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_C_REPAIR_USER_CONTROL ret = (Row_C_REPAIR_USER_CONTROL)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
        public int UpdateCRepairUserControl(C_REPAIR_USER_CONTROL CRepairUserControl, OleExec DB)
        {
            return DB.ORM.Updateable<C_REPAIR_USER_CONTROL>(CRepairUserControl).Where(t => t.ID == CRepairUserControl.ID).ExecuteCommand();
        }
        public int InsertCRepairUserControl(C_REPAIR_USER_CONTROL CRepairUserControl, OleExec DB)
        {
            return DB.ORM.Insertable<C_REPAIR_USER_CONTROL>(CRepairUserControl).ExecuteCommand();
        }
        public List<C_REPAIR_USER_CONTROL> GetSNBYRepairmanManagement(string AcceptRepairID, OleExec DB)
        {
            string StrSQL = $@"SELECT  CONTROL_EMP_NO,CONTROL_ID,CONTROL_QTY,
 CASE VALID_FLAG  WHEN '1' THEN '上班' 
  ELSE '休假' END AS VALID_FLAG,LOCATION 
FROM C_REPAIR_USER_CONTROL WHERE CONTROL_TYPE='{AcceptRepairID}' AND CONTROL_EMP_NO NOT LIKE '#%' ORDER BY CONTROL_ID ASC ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<C_REPAIR_USER_CONTROL> listSn = new List<C_REPAIR_USER_CONTROL>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_C_REPAIR_USER_CONTROL ret = (Row_C_REPAIR_USER_CONTROL)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            return listSn;
        }
        public int DeleteCRepairUserControl(string CONTROL_EMP_NO, OleExec DB)
        {
            return DB.ORM.Deleteable<C_REPAIR_USER_CONTROL>(t => t.CONTROL_EMP_NO == CONTROL_EMP_NO).ExecuteCommand();
        }
    }
    public class Row_C_REPAIR_USER_CONTROL : DataObjectBase
    {
        public Row_C_REPAIR_USER_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public C_REPAIR_USER_CONTROL GetDataObject()
        {
            C_REPAIR_USER_CONTROL DataObject = new C_REPAIR_USER_CONTROL();
            DataObject.ID = this.ID;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_EMP_NO = this.CONTROL_EMP_NO;
            DataObject.CONTROL_EMP_NAME = this.CONTROL_EMP_NAME;
            DataObject.CONTROL_QTY = this.CONTROL_QTY;
            DataObject.CONTROL_ID = this.CONTROL_ID;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.LOCATION = this.LOCATION;
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
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_EMP_NO
        {
            get
            {
                return (string)this["CONTROL_EMP_NO"];
            }
            set
            {
                this["CONTROL_EMP_NO"] = value;
            }
        }
        public string CONTROL_EMP_NAME
        {
            get
            {
                return (string)this["CONTROL_EMP_NAME"];
            }
            set
            {
                this["CONTROL_EMP_NAME"] = value;
            }
        }
        public double? CONTROL_QTY
        {
            get
            {
                return (double?)this["CONTROL_QTY"];
            }
            set
            {
                this["CONTROL_QTY"] = value;
            }
        }
        public string CONTROL_ID
        {
            get
            {
                return (string)this["CONTROL_ID"];
            }
            set
            {
                this["CONTROL_ID"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
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
    public class C_REPAIR_USER_CONTROL
    {
        public string ID { get; set; }
        public string CONTROL_TYPE { get; set; }
        public string CONTROL_EMP_NO { get; set; }
        public string CONTROL_EMP_NAME { get; set; }
        public double? CONTROL_QTY { get; set; }
        public string CONTROL_ID { get; set; }
        public string VALID_FLAG { get; set; }
        public string LOCATION { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}