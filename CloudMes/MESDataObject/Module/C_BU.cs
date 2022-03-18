using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Common;

namespace MESDataObject.Module
{
    // <copyright file="C_BU.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2017-11-27 </date>
    /// <summary>
    /// 映射數據庫中的C_BU表
    /// </summary>
    public class T_C_BU : DataObjectTable
    {
        public T_C_BU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
        }
        public T_C_BU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_BU);
            TableName = "C_BU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool BUIsExist(OleExec oleDB, string bu)
        {
            return oleDB.ORM.Queryable<C_BU>().Any(t => t.BU == bu);
        }
        public bool BUIsExistByID(OleExec oleDB, string strid)
        {
            return oleDB.ORM.Queryable<C_BU>().Any(t => t.ID == strid);
        }
        /// <summary>
        /// Get BU ID
        /// </summary>
        /// <param name="DB">DB</param>
        /// <returns></returns>
        public string GetBUID(string bu, OleExec DB)
        {
            return DB.ORM.Queryable<C_BU>().Where(t => t.BU == bu).Select(t => t.ID).First();
        }
        /// <summary>
        /// Get All BU
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> GetAllBU(OleExec DB)
        {
            return DB.ORM.Queryable<C_BU>().Select(bu => bu.BU).ToList();
        }

        public List<C_BU> GetBUList(OleExec oleDB, string bu)
        {
            return oleDB.ORM.Queryable<C_BU>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(bu), t => t.BU.Contains(bu))
                .OrderBy(t => t.ID).ToList();
        }

        public DataTable GetAllBu(OleExec DB)
        {
            return DB.ORM.Queryable<C_BU>().GroupBy(t => t.BU).OrderBy(t => t.BU).Select(t => t.BU).ToDataTable();
        }
    }

    public class Row_C_BU : DataObjectBase
    {
        public Row_C_BU(DataObjectInfo info) : base(info)
        {

        }
        public C_BU GetDataObject()
        {
            C_BU DataObject = new C_BU();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
    }
    public class C_BU
    {
        public string ID{get;set;}
        public string BU{get;set;}
    }
}