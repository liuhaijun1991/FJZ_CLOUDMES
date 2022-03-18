using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_REPAIR_ITEMS_SON : DataObjectTable
    {
        public T_C_REPAIR_ITEMS_SON(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REPAIR_ITEMS_SON(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REPAIR_ITEMS_SON);
            TableName = "C_REPAIR_ITEMS_SON".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<string> GetRepairItemsSonList(string ItemsID,OleExec DB)
        {
            string strSql = string.Empty;
            //if (string.IsNullOrEmpty(ItemsID))
            //{
            //    strSql = $@" select *from c_repair_items_son where items_id in (select min(id) from c_repair_items where 1=1 )";
            //}
            //else
            //{
                strSql = $@" select *from c_repair_items_son where items_id ='{ItemsID}'";
          //  }
                
            List<string> result = new List<string>();
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            result.Add("");  //BY SDL  加載頁面默認賦予空值,操作員必須點選其他有內容選項
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_REPAIR_ITEMS_SON ret = (Row_C_REPAIR_ITEMS_SON)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject().ITEMS_SON);
                }
            }
            return result;
        }

        public Row_C_REPAIR_ITEMS_SON GetIDByItemsSon(string _ItemSon, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from c_repair_items_son where items_son='{_ItemSon.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ItemSon:" + _ItemSon });
                    throw new MESReturnMessage(errMsg);
                }
                Row_C_REPAIR_ITEMS_SON R = (Row_C_REPAIR_ITEMS_SON)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_C_REPAIR_ITEMS_SON : DataObjectBase
    {
        public Row_C_REPAIR_ITEMS_SON(DataObjectInfo info) : base(info)
        {

        }
        public C_REPAIR_ITEMS_SON GetDataObject()
        {
            C_REPAIR_ITEMS_SON DataObject = new C_REPAIR_ITEMS_SON();
            DataObject.ID = this.ID;
            DataObject.ITEMS_ID = this.ITEMS_ID;
            DataObject.ITEMS_SON = this.ITEMS_SON;
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
        public string ITEMS_ID
        {
            get

            {
                return (string)this["ITEMS_ID"];
            }
            set
            {
                this["ITEMS_ID"] = value;
            }
        }
        public string ITEMS_SON
        {
            get

            {
                return (string)this["ITEMS_SON"];
            }
            set
            {
                this["ITEMS_SON"] = value;
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
    public class C_REPAIR_ITEMS_SON
    {
        public string ID{get;set;}
        public string ITEMS_ID{get;set;}
        public string ITEMS_SON{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}