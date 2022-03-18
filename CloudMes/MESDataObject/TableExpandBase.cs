using System;
using System.Collections.Generic;
using System.Linq;using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
namespace MESDataObject.Module
{
    // <copyright file="TableExpandBase.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2017-11-30 </date>
    /// <summary>
    /// 映射數據庫中的拓展表
    /// </summary>
    public class TableExpandBase : DataObjectTable
    {
        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="_TableName"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        public TableExpandBase(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_Table_Expand);
            TableName = _TableName;
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// Get Expand Detail By ID
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Table_Expand> GetExpandDetailByID(OleExec db,string id)
        {            
            string sql = $@"select * from {this.TableName} where id='{id}'";
            DataSet dsDetail = db.ExecSelect(sql);
            List<Table_Expand> tableExpandList = new List<Table_Expand>();
            Row_Table_Expand rowTableExpand;
            foreach (DataRow row in dsDetail.Tables[0].Rows)
            {
                rowTableExpand =(Row_Table_Expand)this.NewRow();
                rowTableExpand.loadData(row);
                tableExpandList.Add(rowTableExpand.GetDataObject());
            }
            return tableExpandList;
        }

        /// <summary>
        /// get expand list no id
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ExpandNoID> GetExpandDetailNoID(OleExec db, string id)
        {
            string sql = $@"select * from {this.TableName} where id='{id}'";
            DataSet dsDetail = db.ExecSelect(sql);
            List<ExpandNoID> tableExpandList = new List<ExpandNoID>();            
            foreach (DataRow row in dsDetail.Tables[0].Rows)
            {
                tableExpandList.Add(new ExpandNoID {
                    SEQ_NO = (double)Convert.ToInt32(row["SEQ_NO"].ToString()),
                    NAME = row["NAME"].ToString(),
                    VALUE=row["VALUE"].ToString()
                });
            }
            return tableExpandList;
        }
        
        /// <summary>
        /// Get One Expand  Row
        /// </summary>
        /// <param name="db"></param>      
        /// <param name="id"></param>
        /// <param name="seq_no"></param>
        /// <returns></returns>
        public Row_Table_Expand GetRowExpand(OleExec db, string id, int seq_no)
        {
            string sql = $@"select * from {this.TableName} where id='{id}' and seq_no={seq_no}";
            DataSet dsDetail = db.ExecSelect(sql);
            Row_Table_Expand rowTableExpand = (Row_Table_Expand)this.NewRow();
            rowTableExpand.loadData(dsDetail.Tables[0].Rows[0]);
            return rowTableExpand;
        }

        /// <summary>
        /// Delete Expand Info by id and seq_no
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="seq_no"></param>
        /// <returns></returns>
        public string DeleteExpandInfo(OleExec db,string id, int seq_no)
        {
            string sql = $@"delete {this.TableName} where id='{id}' and seq_no={seq_no}";
            return db.ExecSQL(sql);
        }

        /// <summary>
        /// Delete Expand Info by id 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="seq_no"></param>
        /// <returns></returns>
        public string DeleteExpandInfo(OleExec db, string id)
        {
            string sql = $@"delete {this.TableName} where id='{id}'";
            return db.ExecSQL(sql);
        }
        /// <summary>
        /// By ID Get Max Seq No
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSeqNo (OleExec db,string id)
        {
            string sql = $@"select max(seq_no) from {this.TableName} where id='{id}'";
            DataSet dsSeqNo = db.ExecSelect(sql);
            if (!string.IsNullOrEmpty(dsSeqNo.Tables[0].Rows[0][0].ToString()))
            {
                return Convert.ToInt32(dsSeqNo.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// check a expand name is exist or not
        /// </summary>
        /// <param name="oleDB"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ExpandNameIsExist(OleExec oleDB, string id, string name)
        {
            string sql = $@"select * from {this.TableName}  where id='{id}' and name='{name}'";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_Table_Expand : DataObjectBase
    {
        public Row_Table_Expand(DataObjectInfo info) : base(info) { }
        public Table_Expand GetDataObject()
        {
            Table_Expand DataObject = new Table_Expand();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
            return DataObject;
        }
        public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }
        public double? SEQ_NO
        {
            get { return (double?)this["SEQ_NO"]; }
            set { this["SEQ_NO"] = value; }
        }
        public string NAME
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }
        public string VALUE
        {
            get { return (string)this["VALUE"]; }
            set { this["VALUE"] = value; }
        }
    }
    public class Table_Expand
    {
        public string ID;
        public double? SEQ_NO;
        public string NAME;
        public string VALUE;
    }

    public class ExpandNoID
    {        
        public double? SEQ_NO;
        public string NAME;
        public string VALUE;
    }
}