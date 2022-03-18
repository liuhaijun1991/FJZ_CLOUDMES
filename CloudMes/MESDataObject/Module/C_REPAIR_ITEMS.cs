using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_REPAIR_ITEMS : DataObjectTable
    {
        public T_C_REPAIR_ITEMS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REPAIR_ITEMS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REPAIR_ITEMS);
            TableName = "C_REPAIR_ITEMS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<string> GetRepairItemsList(string ITEM_NAME, OleExec DB)
        {
            string strSql = string.Empty;
            //if (string.IsNullOrEmpty(ITEM_NAME))
            //{
                strSql = $@" select * from c_repair_items where 1=1 order by id ";
            //}
            //else
            //{
            //    strSql = $@" select * from c_repair_items where item_name='{ITEM_NAME}' ";
            //}
           
            List<string> result = new List<string>();
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            result.Add("");  //BY SDL  加載頁面默認賦予空值,操作員必須點選其他有內容選項
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_REPAIR_ITEMS ret = (Row_C_REPAIR_ITEMS)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject().ITEM_NAME);
                }
            }
            return result;
        }

        public Row_C_REPAIR_ITEMS GetIDByItemName(string _ItemName, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from c_repair_items where item_name='{_ItemName.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ItemName:" + _ItemName });
                    throw new MESReturnMessage(errMsg);
                }
                Row_C_REPAIR_ITEMS R = (Row_C_REPAIR_ITEMS)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_C_REPAIR_ITEMS : DataObjectBase
    {
        public Row_C_REPAIR_ITEMS(DataObjectInfo info) : base(info)
        {

        }
        public C_REPAIR_ITEMS GetDataObject()
        {
            C_REPAIR_ITEMS DataObject = new C_REPAIR_ITEMS();
            DataObject.ID = this.ID;
            DataObject.ITEM_NAME = this.ITEM_NAME;
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
    public class C_REPAIR_ITEMS
    {
        public string ID{get;set;}
        public string ITEM_NAME{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}