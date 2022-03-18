using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace MESDataObject.Module
{
    public class T_C_WT_CONFIG : DataObjectTable
    {
        public T_C_WT_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_WT_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_WT_CONFIG);
            TableName = "C_WT_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 獲取稱重修改刪除Approve權限
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> Get_WTPrivilegeList(OleExec DB,string LoginUserEmp)
        {
            List<string> List = new List<string>();
            string sql = string.Empty;
            sql = $@" SELECT A.ID,A.SYSTEM_NAME, A.PRIVILEGE_NAME,A.PRIVILEGE_DESC
                      FROM C_PRIVILEGE A, C_USER B, C_USER_PRIVILEGE C
                      WHERE  C.USER_ID = B.ID AND C.PRIVILEGE_ID = A.ID  AND 
                             B.EMP_NO = '{LoginUserEmp}' AND A.PRIVILEGE_Name IN ('WTConfigModify','WTConfigApprove')
                      UNION
                      SELECT A.ID,A.SYSTEM_NAME, A.PRIVILEGE_NAME, A.PRIVILEGE_DESC
                      FROM C_PRIVILEGE A, C_USER_ROLE B, C_ROLE_PRIVILEGE C, C_USER D
                      WHERE  C.ROLE_ID = B.ROLE_ID  AND A.ID = C.PRIVILEGE_ID   AND D.EMP_NO = '{LoginUserEmp}' AND 
                             D.ID = B.USER_ID AND A.PRIVILEGE_Name IN ('WTConfigModify','WTConfigApprove')  ";
            DataSet ds = DB.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach(DataRow item in ds.Tables[0].Rows)
                {
                    List.Add(item["PRIVILEGE_NAME"].ToString());
                }
            }
            return List;
        }
        /// <summary>
        /// 返回所有料號的稱重參數
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<TVC_WT_CONFIG> GetAllWTList(OleExec DB)
        {
            List<TVC_WT_CONFIG> WTList = new List<TVC_WT_CONFIG>();
            string sql = $@"Select ID,SKUNO,BMinValue,BMaxValue,CMinValue,CMaxValue,
                                           Case when online_flag=0 then 'NO' else 'YES' end as OnLine_Flag,
                                           Edit_Emp,Edit_Time,Approve_Emp,Approve_Time 
                                           From C_WT_CONFIG 
                                    Order by case when approve_emp is null then 0 else 1 end asc, 
                                             case when approve_time is null then edit_time 
                                                  when approve_time>edit_time then approve_time else edit_time end  desc";
            DataTable dt = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        //這種方式可增加速度
                        if (dr["APPROVE_TIME"] is DBNull)
                        {
                            WTList.Add(new TVC_WT_CONFIG
                            {
                                ID = dr["ID"].ToString(),
                                SKUNO = dr["SKUNO"].ToString(),
                                BMINVALUE = dr["BMINVALUE"].ToString(),
                                BMAXVALUE = dr["BMAXVALUE"].ToString(),
                                CMINVALUE = dr["CMINVALUE"].ToString(),
                                CMAXVALUE = dr["CMAXVALUE"].ToString(),
                                ONLINE_FLAG = dr["ONLINE_FLAG"].ToString(),
                                EDIT_EMP = dr["EDIT_EMP"].ToString(),
                                EDIT_TIME = (DateTime)dr["EDIT_TIME"],
                                APPROVE_EMP = dr["APPROVE_EMP"].ToString(),
                                APPROVE_TIME = null                                
                            });
                        }
                        else { 

                            WTList.Add(new TVC_WT_CONFIG
                            {
                                ID = dr["ID"].ToString(),
                                SKUNO = dr["SKUNO"].ToString(),
                                BMINVALUE = dr["BMINVALUE"].ToString(),
                                BMAXVALUE = dr["BMAXVALUE"].ToString(),
                                CMINVALUE = dr["CMINVALUE"].ToString(),
                                CMAXVALUE = dr["CMAXVALUE"].ToString(),
                                ONLINE_FLAG = dr["ONLINE_FLAG"].ToString(),
                                EDIT_EMP = dr["EDIT_EMP"].ToString(),
                                EDIT_TIME = (DateTime)dr["EDIT_TIME"],
                                APPROVE_EMP = dr["APPROVE_EMP"].ToString(),
                                APPROVE_TIME =  (DateTime)dr["APPROVE_TIME"]
                            });
                        }
                        //速度太慢
                        //Row_C_WT_CONFIG RowCWT = (Row_C_WT_CONFIG)this.NewRow();
                        //RowCWT.loadData(dr);
                        //WTList.Add(RowCWT.GetDataObject());

                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return WTList;
        }
    }
    public class Row_C_WT_CONFIG : DataObjectBase
    {
        public Row_C_WT_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_WT_CONFIG GetDataObject()
        {
            C_WT_CONFIG DataObject = new C_WT_CONFIG();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.BMINVALUE = this.BMINVALUE;
            DataObject.BMAXVALUE = this.BMAXVALUE;
            DataObject.CMINVALUE = this.CMINVALUE;
            DataObject.CMAXVALUE = this.CMAXVALUE;
            DataObject.ONLINE_FLAG = this.ONLINE_FLAG;
            DataObject.APPROVE_EMP = this.APPROVE_EMP;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.APPROVE_TIME = this.APPROVE_TIME;
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
        public string BMINVALUE
        {
            get
            {
                return (string)this["BMINVALUE"];
            }
            set
            {
                this["BMINVALUE"] = value;
            }
        }
        public string BMAXVALUE
        {
            get
            {
                return (string)this["BMAXVALUE"];
            }
            set
            {
                this["BMAXVALUE"] = value;
            }
        }
        public string CMINVALUE
        {
            get
            {
                return (string)this["CMINVALUE"];
            }
            set
            {
                this["CMINVALUE"] = value;
            }
        }
        public string CMAXVALUE
        {
            get
            {
                return (string)this["CMAXVALUE"];
            }
            set
            {
                this["CMAXVALUE"] = value;
            }
        }
        public double? ONLINE_FLAG
        {
            get
            {
                return (double?)this["ONLINE_FLAG"];
            }
            set
            {
                this["ONLINE_FLAG"] = value;
            }
        }
        public string APPROVE_EMP
        {
            get
            {
                return (string)this["APPROVE_EMP"];
            }
            set
            {
                this["APPROVE_EMP"] = value;
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
        public DateTime? APPROVE_TIME
        {
            get
            {
                return (DateTime?)this["APPROVE_TIME"];
            }
            set
            {
                this["APPROVE_TIME"] = value;
            }
        }
    }
    public class C_WT_CONFIG
    {
        public string ID;
        public string SKUNO;
        public string BMINVALUE;
        public string BMAXVALUE;
        public string CMINVALUE;
        public string CMAXVALUE;
        public double? ONLINE_FLAG;
        public string APPROVE_EMP;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
        public DateTime? APPROVE_TIME;
    }

    public class TVC_WT_CONFIG
    {
        public string ID;
        public string SKUNO;
        public string BMINVALUE;
        public string BMAXVALUE;
        public string CMINVALUE;
        public string CMAXVALUE;
        public string ONLINE_FLAG;
        public string APPROVE_EMP;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
        public DateTime? APPROVE_TIME;
    }

}
