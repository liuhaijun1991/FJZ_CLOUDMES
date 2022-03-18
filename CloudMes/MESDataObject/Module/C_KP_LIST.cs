using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_C_KP_LIST : DataObjectTable
    {
        public T_C_KP_LIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public bool CheckKPListName(string KPListName, OleExec DB)
        {
            //string strSql = $@"select count(1) from c_kp_list where LISTNAME='{KPListName}'";
            //string strRet = DB.ExecSelectOneValue(strSql).ToString();
            //try
            //{
            //    if (Int32.Parse(strRet) > 0)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //catch(Exception ee)
            //{
            //    throw new Exception(strSql +":" +strRet);
            //}
            return DB.ORM.Queryable<C_KP_LIST>().Any(t => t.LISTNAME == KPListName);


        }

        public T_C_KP_LIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_LIST);
            TableName = "C_KP_LIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<string> GetListIDBySkuno(string Skuno, OleExec DB)
        {
            //List<string> ret = new List<string>();
            //string strSql = $@"select ID from c_kp_list where skuno='{Skuno}'";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            //}
            //return ret;
            return DB.ORM.Queryable<C_KP_LIST>().Where(t => t.SKUNO == Skuno && t.FLAG=="1").Select(t => t.ID).ToList();
        }
        public List<string> GetListNameBySkuno(string Skuno, OleExec DB)
        {
            //List<string> ret = new List<string>();
            //string strSql = $@"select ID from c_kp_list where skuno='{Skuno}'";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            //}
            //return ret;
            return DB.ORM.Queryable<C_KP_LIST>().Where(t => t.SKUNO == Skuno).OrderBy(t=>t.EDIT_TIME,SqlSugar.OrderByType.Desc).Select(t => t.LISTNAME).ToList();
        }
        public List<string> GetListIDBySkUNAME(string SKNAME, OleExec DB)
        {
           
            return DB.ORM.Queryable<C_KP_LIST>().Where(t => t.SKUNO == SKNAME && t.FLAG == "1").Select(t => t.ID).ToList();
        }

        public List<string> GetItemID(string ID, OleExec DB)
        {
            //List<string> ret = new List<string>();
            //string strSql = $@"select ID from c_kp_list_item c where c.list_id='{ID}' order by c.seq";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            //}
            //return ret;
            return DB.ORM.Queryable<C_KP_List_Item>().Where(t => t.LIST_ID==ID).OrderBy(t=>t.SEQ).Select(t => t.ID).ToList();
        }

        public bool KpIDIsExist(string kpID, OleExec sfcdb)
        {
            //string strSql = $@"select ID from c_kp_list where id='{kpID}'";
            //DataSet ds = sfcdb.RunSelect(strSql);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return sfcdb.ORM.Queryable<C_KP_LIST>().Any(t => t.ID == kpID);
        }

        /// <summary>
        /// WZW 查詢LISTNAME
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> GetKPLISTID(OleExec DB, string ListName)
        {
            List<string> ret = new List<string>();
            string strSql = $@"select ID from c_kp_list where LISTNAME='{ListName}'";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            }
            return ret;
        }
        /// <summary>
        /// WZW 查詢LISTNAME
        /// </summary>
        /// <param name="kpID"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<C_KP_LIST> GetListNOBySkuno(string SKU, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_KP_LIST> LanguageList = new List<C_KP_LIST>();
            sql = $@"select * from c_kp_list where skuno='{SKU}' && t.FLAG=='1'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
            //List<string> ret = new List<string>();
            //string strSql = $@"select * from c_kp_list where skuno='{SKU}'";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            //}
            //return ret;
        }

        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="kpID"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public C_KP_LIST CreateLanguageClass(DataRow dr)
        {
            Row_C_KP_LIST row = (Row_C_KP_LIST)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        public int InsertKPList(C_KP_LIST CKPList, OleExec DB)
        {
            return DB.ORM.Insertable(CKPList).ExecuteCommand();
        }
        public int UpdateKPList(C_KP_LIST CKPList, string ID, OleExec DB)
        {
            return DB.ORM.Updateable(CKPList).Where(t => t.ID == ID).ExecuteCommand();
        }
        public string GetListIDbySkuno( string skuno, OleExec DB)
        {
            return DB.ORM.Queryable<C_KP_LIST>().Where(t => t.SKUNO == skuno && t.FLAG == "1").Select(t => t.ID).First();
        }
    }
    public class Row_C_KP_LIST : DataObjectBase
    {
        public Row_C_KP_LIST(DataObjectInfo info) : base(info)
        {

        }

        //public DataTable GetBySkuNO(string Skuno, OleExec sfcdb)
        //{
        //    string strSql = "";
        //    throw new Exception();
            
        //}

        public C_KP_LIST GetDataObject()
        {
            C_KP_LIST DataObject = new C_KP_LIST();
            DataObject.ID = this.ID;
            DataObject.LISTNAME = this.LISTNAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.CUSTVERSION = this.CUSTVERSION;
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
        public string LISTNAME
        {
            get
            {
                return (string)this["LISTNAME"];
            }
            set
            {
                this["LISTNAME"] = value;
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
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
        public string CUSTVERSION
        {
            get
            {
                return (string)this["CUSTVERSION"];
            }
            set
            {
                this["CUSTVERSION"] = value;
            }
        }
    }
    public class C_KP_LIST
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string LISTNAME{get;set;}
        public string SKUNO{get;set; }
        public string FLAG { get; set; }
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set; }
        public string CUSTVERSION { get; set; }
    }

    public enum ENUM_C_KP_LIST
    {
        /// <summary>
        /// 默認優先使用
        /// </summary>
        [EnumValue("1")]
        VALID_FLAG_TRUE,
        /// <summary>
        /// 默認不優先使用
        /// </summary>
        [EnumValue("1")]
        VALID_FLAG_FALSE
    }

}