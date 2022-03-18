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
    public class T_C_STORAGE_ITEM : DataObjectTable
    {
        public T_C_STORAGE_ITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_STORAGE_ITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_STORAGE_ITEM);
            TableName = "C_STORAGE_ITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<string> GetByItemNameAndItemSon(string BU,string user,string ItemName, string ItemSon, OleExec DB)
        {
            string strSql = "";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":ItemName", ItemName),
                new OleDbParameter(":ItemSon", ItemSon)
            };
            List<string> result = new List<string>();
            //HWD TO_STOREGE的權限管控，單獨加這麼一段
            if (BU == "HWD")
            {
                if (ItemSon == "009T")
                {
                    strSql = $@"select * from c_storage_item where item_name=:ItemName and item_son=:ItemSon";
                }
                else
                {
                    strSql = $@"select b.STORAGE_CODE From c_control a,c_storage_item b where a.CONTROL_NAME ='MRB_TO_STORAGE_LIMIT' and instr(a.CONTROL_TYPE,b.STORAGE_CODE)>0 and instr( a.CONTROL_VALUE,'{user}')>0 
                                and b.item_name='{ItemName}' and b.item_son ='{ItemSon}'";
                }
            }
            else if (BU == "VNDCN" || BU == "VNJUNIPER")
            {
                strSql = $@"
                select distinct storage_code from c_storage_item where item_name='{ItemName}' and item_son='{ItemSon}'
                union all
                select distinct control_type from c_control where CONTROL_NAME ='MRB_TO_STORAGE_LIMIT' and instr(control_value,'{user}')>0";
            }
            else
            {
                strSql = $@"select * from c_storage_item where item_name=:ItemName and item_son=:ItemSon";
            }


            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STORAGE_ITEM ret = (Row_C_STORAGE_ITEM)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject().STORAGE_CODE);
                }
            }
            return result;
        }
        public List<string> GetByItemName(string ItemName, OleExec DB)
        {
            string strSql = $@"select * from c_storage_item where item_name=:ItemName ";
            OleDbParameter[] paramet = new OleDbParameter[] {new OleDbParameter(":ItemName", ItemName)};
            List<string> result = new List<string>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STORAGE_ITEM ret = (Row_C_STORAGE_ITEM)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject().STORAGE_CODE);
                }
            }
            return result;
        }
        public C_STORAGE_ITEM GetByItemNameAndItemSonAndStorageCode(string ItemName, string ItemSon,string StorageCode, OleExec DB)
        {
            string strSql = $@"select * from c_storage_item where item_name=:ItemName and item_son=:ItemSon and storage_code=:StorageCode";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":ItemName", ItemName),
                new OleDbParameter(":ItemSon", ItemSon),
                new OleDbParameter(":StorageCode", StorageCode)
            };          
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_STORAGE_ITEM ret = (Row_C_STORAGE_ITEM)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public C_STORAGE_ITEM GetById(string id, OleExec DB)
        {
            string strSql = $@"select * from c_storage_item where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id", id)
            };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_STORAGE_ITEM ret = (Row_C_STORAGE_ITEM)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public int Add(C_STORAGE_ITEM NewStorageItem, OleExec DB)
        {
            Row_C_STORAGE_ITEM NewStorageItemRow = (Row_C_STORAGE_ITEM)NewRow();
            NewStorageItemRow.ID = NewStorageItem.ID;
            NewStorageItemRow.ITEM_NAME = NewStorageItem.ITEM_NAME;
            NewStorageItemRow.ITEM_SON = NewStorageItem.ITEM_SON;
            NewStorageItemRow.STORAGE_CODE = NewStorageItem.STORAGE_CODE;
            NewStorageItemRow.EDIT_EMP = NewStorageItem.EDIT_EMP;
            NewStorageItemRow.EDIT_TIME = NewStorageItem.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewStorageItemRow.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        public int DeleteById(string id, OleExec DB)
        {
            string strSql = $@"delete c_storage_item  where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
    }
    public class Row_C_STORAGE_ITEM : DataObjectBase
    {
        public Row_C_STORAGE_ITEM(DataObjectInfo info) : base(info)
        {

        }
        public C_STORAGE_ITEM GetDataObject()
        {
            C_STORAGE_ITEM DataObject = new C_STORAGE_ITEM();
            DataObject.ID = this.ID;
            DataObject.ITEM_NAME = this.ITEM_NAME;
            DataObject.ITEM_SON = this.ITEM_SON;
            DataObject.STORAGE_CODE = this.STORAGE_CODE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string ITEM_NAME
        {
            get
            {
                return (string)this["ITEM_NAME"];
            }
            set
            {
                this["ITEM_NAME"] = value;
            }
        }
        public string ITEM_SON
        {
            get
            {
                return (string)this["ITEM_SON"];
            }
            set
            {
                this["ITEM_SON"] = value;
            }
        }
        public string STORAGE_CODE
        {
            get
            {
                return (string)this["STORAGE_CODE"];
            }
            set
            {
                this["STORAGE_CODE"] = value;
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
    }
    public class C_STORAGE_ITEM
    {
        public string ID{get;set;}
        public string ITEM_NAME{get;set;}
        public string ITEM_SON{get;set;}
        public string STORAGE_CODE{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}