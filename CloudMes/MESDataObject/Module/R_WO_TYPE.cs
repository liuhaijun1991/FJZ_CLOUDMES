using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    // <copyright file="R_WO_TYPE.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-03-16 </date>
    /// <summary>
    /// 映射數據庫中的R_WO_TYPE表
    /// </summary>
    public class T_R_WO_TYPE : DataObjectTable
    {
        public T_R_WO_TYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_TYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_TYPE);
            TableName = "R_WO_TYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// Get r_wo_type object by wo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="wo"></param>
        /// <returns></returns>
        public R_WO_TYPE GetWOTypeByWO(OleExec db,string order_type)
        {
            R_WO_TYPE woType = new R_WO_TYPE();
            string sql = "";
            try
            {                
                sql = $@"select * from R_WO_TYPE where order_type='{order_type}'";
                DataSet ds = db.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Row_R_WO_TYPE rowWOType = (Row_R_WO_TYPE)this.NewRow();
                    rowWOType.loadData(ds.Tables[0].Rows[0]);
                    woType = rowWOType.GetDataObject();
                }
                else
                {
                    woType = null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return woType;
        }

        public R_WO_TYPE GetWoTypeByWo(OleExec DB, string Wo)
        {
            string sql = $@"SELECT * FROM R_WO_TYPE WHERE '{Wo}' LIKE PREFIX||'%'";
            try
            {
                return DB.ORM.Ado.SqlQuery<R_WO_TYPE>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public R_WO_TYPE GetWOTypeByWO_HWT(OleExec db, string WO)
        {
            R_WO_TYPE woType = new R_WO_TYPE();
            string sql = "";
            try
            {
                sql = $@"SELECT *
                          FROM sfcruntime.r_wo_type
                         WHERE prefix = substr('{WO}', 1, 6)
                           AND rownum = 1";
                DataSet ds = db.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Row_R_WO_TYPE rowWOType = (Row_R_WO_TYPE)this.NewRow();
                    rowWOType.loadData(ds.Tables[0].Rows[0]);
                    woType = rowWOType.GetDataObject();
                }
                else
                {
                    woType = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return woType;
        }

        public string GetProductTypeByWO_HWT(OleExec db, string WO)
        {
            //R_WO_TYPE woType = new R_WO_TYPE();
            string productType;
            string sql = "";
            try
            {
                sql = $@"SELECT *
                          FROM sfcruntime.r_wo_type
                         WHERE prefix = substr('{WO}', 1, 6)
                           AND rownum = 1";
                DataSet ds = db.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    productType = ds.Tables[0].Rows[0]["PRODUCT_TYPE"].ToString();
                }
                else
                {
                    productType = "NORMAL";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productType;
        }

        public List<string> GetAllType(OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            List<string> types = new List<string>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select distinct(workorder_type) wotype from {TableName} ";
                try
                {
                    dt = db.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        types.Add(dr[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                    
                }
                return types;
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }
            
        }

        /// <summary>
        /// WZW 檢查工單前六位與工單類型是否存在表中R_WO_TYPE
        /// </summary>
        /// <param name="db"></param>
        /// <param name="order_type"></param>
        /// <returns></returns>
        public bool GetWOTypeByPREFIX(string Wo_type, string prefix, OleExec db)
        {
            bool res = false;
            try
            {
                string sql = $@"select * from r_wo_type where workorder_type='{Wo_type}' and prefix='{prefix}'";
                DataTable dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        /// <summary>
        /// 輸入工單前六位和類型，判斷工單是否屬於該類型，R_WO_TYPE
        /// add by HGB  2019.05.23
        /// </summary>
        /// <param name="db"></param>
        /// <param name="order_type"></param>
        /// <returns></returns>
        public bool IsTypeInput(string Wo_type, string prefix, OleExec db)
        {
            bool res = false;
            try
            {
                string sql = $@"select * from r_wo_type where PRODUCT_TYPE='{Wo_type}' and prefix='{prefix}'";
               
                 DataTable   dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        public List<R_WO_TYPE> GetRowsListByCategory(string category, OleExec db)
        {
            string strSql = $@" select * from r_wo_type where category in ('{category}') ";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            List<R_WO_TYPE> rWoTypeList = new List<R_WO_TYPE>();
            if (table.Rows.Count > 0)
            {
                R_WO_TYPE rWoType = new R_WO_TYPE();
                foreach (DataRow row in table.Rows)
                {
                    Row_R_WO_TYPE ret = (Row_R_WO_TYPE)this.NewRow();
                    ret.loadData(row);
                    rWoType = ret.GetDataObject();
                    rWoTypeList.Add(rWoType);
                }
            }
            else
            {
                rWoTypeList = null;
            }
            return rWoTypeList;
        }
        public List<R_WO_TYPE> GetWOType(List<string> WOType, string WoPrefix, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_TYPE>().Where(t => WOType.Contains(t.WORKORDER_TYPE) && t.PREFIX == WoPrefix).ToList();
        }
        public List<R_WO_TYPE> GetWONOType(string WO, string WOType, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_TYPE, R_WO_BASE>((p1, p2) => p2.WORKORDERNO.Contains(p1.PREFIX)).Where((p1, p2) => p2.WORKORDERNO == WO && p1.WORKORDER_TYPE.Contains(WOType)).ToList();
        }

        public string GetProductTypeByWO(OleExec sfcdb, string wo)
        {
            return sfcdb.ORM.Queryable<R_WO_TYPE>().Where(r => SqlSugar.SqlFunc.StartsWith(wo, r.PREFIX)).Select(r => r.PRODUCT_TYPE).ToList().FirstOrDefault();
        }

    }
    public class Row_R_WO_TYPE : DataObjectBase
    {
        public Row_R_WO_TYPE(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_TYPE GetDataObject()
        {
            R_WO_TYPE DataObject = new R_WO_TYPE();
            DataObject.ID = this.ID;
            DataObject.WORKORDER_TYPE = this.WORKORDER_TYPE;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.PREFIX = this.PREFIX;
            DataObject.ORDER_TYPE = this.ORDER_TYPE;
            DataObject.PRODUCT_TYPE = this.PRODUCT_TYPE;
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
        public string WORKORDER_TYPE
        {
            get
            {
                return (string)this["WORKORDER_TYPE"];
            }
            set
            {
                this["WORKORDER_TYPE"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string PREFIX
        {
            get
            {
                return (string)this["PREFIX"];
            }
            set
            {
                this["PREFIX"] = value;
            }
        }
        public string ORDER_TYPE
        {
            get
            {
                return (string)this["ORDER_TYPE"];
            }
            set
            {
                this["ORDER_TYPE"] = value;
            }
        }
        public string PRODUCT_TYPE
        {
            get
            {
                return (string)this["PRODUCT_TYPE"];
            }
            set
            {
                this["PRODUCT_TYPE"] = value;
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
    public class R_WO_TYPE
    {
        public string ID{get;set;}
        public string WORKORDER_TYPE{get;set;}
        public string CATEGORY{get;set;}
        public string PREFIX{get;set;}
        public string ORDER_TYPE{get;set;}
        public string PRODUCT_TYPE{get;set;}
        public string DESCRIPTION{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}