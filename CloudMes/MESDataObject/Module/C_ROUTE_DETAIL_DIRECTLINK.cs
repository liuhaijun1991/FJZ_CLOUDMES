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
    public class T_C_ROUTE_DETAIL_DIRECTLINK : DataObjectTable
    {
        public T_C_ROUTE_DETAIL_DIRECTLINK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_C_ROUTE_DETAIL_DIRECTLINK);
            TableName = "c_route_detail_directlink".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_C_ROUTE_DETAIL_DIRECTLINK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROUTE_DETAIL_DIRECTLINK);
            TableName = "c_route_detail_directlink".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_ROUTE_DETAIL_DIRECTLINK> GetByDetailId(string DetailId, OleExec DB)
        {
            return DB.ORM.Queryable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.C_ROUTE_DETAIL_ID == DetailId).ToList();
            //string strSql = $@"select * from c_route_detail_directlink where c_route_detail_id=:DetailId";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":DetailId", DetailId) };
            //DataSet res = DB.ExecSelect(strSql, paramet);
            //if (res.Tables[0].Rows.Count > 0)
            //{
            //    List<C_ROUTE_DETAIL_DIRECTLINK> retlist = new List<C_ROUTE_DETAIL_DIRECTLINK>();
            //    for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //    {
            //        Row_C_ROUTE_DETAIL_DIRECTLINK ret = (Row_C_ROUTE_DETAIL_DIRECTLINK)NewRow();
            //        ret.loadData(res.Tables[0].Rows[i]);
            //        retlist.Add(ret.GetDataObject());
            //    }
            //    return retlist;
            //}
            //else
            //{
            //    return null;
            //}
        }
        public int DeleteById(string Id, OleExec DB)
        {
            return DB.ORM.Deleteable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.ID == Id).ExecuteCommand();
            //string strSql = $@"delete c_route_detail_directlink where id=:Id";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            //int res = DB.ExecSqlNoReturn(strSql, paramet);
            //return res;
        }
        public int GetCountByDetailId(string detailId, OleExec DB)
        {
            return DB.ORM.Queryable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.C_ROUTE_DETAIL_ID == detailId).ToList().Count();
            //string strSql = $@"select count(*) from c_route_detail_directlink where c_route_detail_id=:detailId";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":detailId", detailId) };
            //int res = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            //return res;
        }
        public int GetCountByDirectlinkId(string directlinkId, OleExec DB)
        {
            return DB.ORM.Queryable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.DIRECTLINK_ROUTE_DETAIL_ID == directlinkId).ToList().Count();
            //string strSql = $@"select count(*) from c_route_detail_directlink where directlink_route_detail_id=:directlink_route_detail_id";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":directlink_route_detail_id", directlinkId) };
            //int res = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            //return res;
        }
        public int Add(C_ROUTE_DETAIL_DIRECTLINK newdirectlink, OleExec DB)
        {

            return DB.ORM.Insertable<C_ROUTE_DETAIL_DIRECTLINK>(newdirectlink).ExecuteCommand();
            //string strSql = $@"insert into c_route_detail_directlink(id,c_route_detail_id,directlink_route_detail_id)";
            //strSql = strSql + $@"values(:id,:c_route_detail_id,:directlink_route_detail_id)";
            //OleDbParameter[] paramet = new OleDbParameter[3];
            //paramet[0] = new OleDbParameter(":id", newdirectlink.ID);
            //paramet[1] = new OleDbParameter(":c_route_detail_id", newdirectlink.C_ROUTE_DETAIL_ID);
            //paramet[2] = new OleDbParameter(":directlink_route_detail_id", newdirectlink.DIRECTLINK_ROUTE_DETAIL_ID);
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
        }
        public int DeleteByDetailIdAndDirectlinkId(string detailId, string directlinkId, OleExec DB)
        {
            return DB.ORM.Deleteable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.C_ROUTE_DETAIL_ID == detailId && t.DIRECTLINK_ROUTE_DETAIL_ID == directlinkId).ExecuteCommand();
            //string strSql = $@"delete c_route_detail_directlink where c_route_detail_id=:c_route_detail_id and directlink_route_detail_id=:directlink_route_detail_id";
            //OleDbParameter[] paramet = new OleDbParameter[2];
            //paramet[0] = new OleDbParameter(":c_route_detail_id", detailId);
            //paramet[1] = new OleDbParameter(":directlink_route_detail_id", directlinkId);
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
        }
        public int DeleteByDirectlinkId(string directlinkId, OleExec DB)
        {
            return DB.ORM.Deleteable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.DIRECTLINK_ROUTE_DETAIL_ID == directlinkId).ExecuteCommand();
            //string strSql = $@"delete c_route_detail_directlink where  directlink_route_detail_id=:directlink_route_detail_id";
            //OleDbParameter[] paramet = new OleDbParameter[1];         
            //paramet[0] = new OleDbParameter(":directlink_route_detail_id", directlinkId);
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
        }
        public int DeleteByDetailId(string detailId, OleExec DB)
        {
            return DB.ORM.Deleteable<C_ROUTE_DETAIL_DIRECTLINK>().Where(t => t.C_ROUTE_DETAIL_ID == detailId).ExecuteCommand();
            //string strSql = $@"delete c_route_detail_directlink where c_route_detail_id=:c_route_detail_id";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            //paramet[0] = new OleDbParameter(":c_route_detail_id", detailId);
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
        }
    }
    public class Row_C_ROUTE_DETAIL_DIRECTLINK : DataObjectBase
    {
        public Row_C_ROUTE_DETAIL_DIRECTLINK(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE_DETAIL_DIRECTLINK GetDataObject()
        {
            C_ROUTE_DETAIL_DIRECTLINK DataObject = new C_ROUTE_DETAIL_DIRECTLINK();
            DataObject.ID = this.ID;
            DataObject.C_ROUTE_DETAIL_ID = this.C_ROUTE_DETAIL_ID;
            DataObject.DIRECTLINK_ROUTE_DETAIL_ID = this.DIRECTLINK_ROUTE_DETAIL_ID;
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
        public string C_ROUTE_DETAIL_ID
        {
            get
            {
                return (string)this["C_ROUTE_DETAIL_ID"];
            }
            set
            {
                this["C_ROUTE_DETAIL_ID"] = value;
            }
        }
        public string DIRECTLINK_ROUTE_DETAIL_ID
        {
            get
            {
                return (string)this["DIRECTLINK_ROUTE_DETAIL_ID"];
            }
            set
            {
                this["DIRECTLINK_ROUTE_DETAIL_ID"] = value;
            }
        }

    }
    public class C_ROUTE_DETAIL_DIRECTLINK
    {
        public string ID{get;set;}
        public string C_ROUTE_DETAIL_ID{get;set;}
        public string DIRECTLINK_ROUTE_DETAIL_ID{get;set;}
    }
}