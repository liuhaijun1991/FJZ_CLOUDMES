using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ROUTE_DETAIL_RETURN : DataObjectTable
    {
        public T_C_ROUTE_DETAIL_RETURN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_C_ROUTE_DETAIL_RETURN);
            TableName = "c_route_detail_return".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_C_ROUTE_DETAIL_RETURN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROUTE_DETAIL_RETURN);
            TableName = "c_route_detail_return".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_ROUTE_DETAIL_RETURN> GetByRoute_DetailId(string route_detailid, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail_return where route_detail_id=:route_detailid";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":route_detailid", route_detailid) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL_RETURN> retlist = new List<C_ROUTE_DETAIL_RETURN>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ROUTE_DETAIL_RETURN ret = (Row_C_ROUTE_DETAIL_RETURN)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }
        public List<C_ROUTE_DETAIL_RETURN> GetByRturn_Route_DetailId(string return_route_detailid, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail_return where return_route_detail_id=:return_route_detail_id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":return_route_detail_id", return_route_detailid) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL_RETURN> retlist = new List<C_ROUTE_DETAIL_RETURN>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ROUTE_DETAIL_RETURN ret = (Row_C_ROUTE_DETAIL_RETURN)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }
        public int GetCountByRoute_DetailId(string route_detailid, OleExec DB)
        {
            string strSql = $@"select count(*) from c_route_detail_return where route_detail_id=:route_detailid";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":route_detailid", route_detailid) };
            int res =Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));           
            return res;
        }
        public int GetCountByReturn_Route_Detail_Id(string return_route_detailid, OleExec DB)
        {
            string strSql = $@"select count(*) from c_route_detail_return where return_route_detail_id=:return_route_detail_id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":return_route_detail_id", return_route_detailid) };
            int res = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            return res;
        }
        public C_ROUTE_DETAIL_RETURN GetById(string id, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail_return where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_ROUTE_DETAIL_RETURN ret = (Row_C_ROUTE_DETAIL_RETURN)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public int Add(C_ROUTE_DETAIL_RETURN newdetailreturn, OleExec DB)
        {
            string strSql = $@"insert into c_route_detail_return(id,route_detail_id,return_route_detail_id)";
            strSql = strSql + $@" values(:id,:route_detail_id,:return_route_detail_id)";
            OleDbParameter[] paramet = new OleDbParameter[3];
            paramet[0] = new OleDbParameter(":id", newdetailreturn.ID);
            paramet[1] = new OleDbParameter(":route_detail_id", newdetailreturn.ROUTE_DETAIL_ID);
            paramet[2] = new OleDbParameter(":return_route_detail_id", newdetailreturn.RETURN_ROUTE_DETAIL_ID);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int DeleteByDetailIdAndReturnId(string detailid, string returnid, OleExec DB)
        {
            string strSql = $@"delete c_route_detail_return where route_detail_id=:route_detail_id and return_route_detail_id=:return_route_detail_id";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":route_detail_id", detailid);
            paramet[1] = new OleDbParameter(":return_route_detail_id", returnid);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int DeleteByDetailId(string detailid, OleExec DB)
        {
            string strSql = $@"delete c_route_detail_return where route_detail_id=:route_detail_id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":route_detail_id", detailid);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int DeleteByReturn_Route_DetailId(string returnid, OleExec DB)
        {
            string strSql = $@"delete c_route_detail_return where return_route_detail_id=:return_route_detail_id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":return_route_detail_id", returnid);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
    }
    public class Row_C_ROUTE_DETAIL_RETURN : DataObjectBase
    {
        public Row_C_ROUTE_DETAIL_RETURN(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE_DETAIL_RETURN GetDataObject()
        {
            C_ROUTE_DETAIL_RETURN DataObject = new C_ROUTE_DETAIL_RETURN();
            DataObject.ID = this.ID;
            DataObject.ROUTE_DETAIL_ID = this.ROUTE_DETAIL_ID;
            DataObject.RETURN_ROUTE_DETAIL_ID = this.RETURN_ROUTE_DETAIL_ID;
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
        public string ROUTE_DETAIL_ID
        {
            get
            {
                return (string)this["ROUTE_DETAIL_ID"];
            }
            set
            {
                this["ROUTE_DETAIL_ID"] = value;
            }
        }
        public string RETURN_ROUTE_DETAIL_ID
        {
            get
            {
                return (string)this["RETURN_ROUTE_DETAIL_ID"];
            }
            set
            {
                this["RETURN_ROUTE_DETAIL_ID"] = value;
            }
        }

    }
    public class C_ROUTE_DETAIL_RETURN
    {
        public string ID{get;set;}
        public string ROUTE_DETAIL_ID{get;set;}
        public string RETURN_ROUTE_DETAIL_ID{get;set;}
    }
}