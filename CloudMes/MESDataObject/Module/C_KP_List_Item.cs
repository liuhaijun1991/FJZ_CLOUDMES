using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_List_Item : DataObjectTable
    {
        public T_C_KP_List_Item(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_List_Item(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_List_Item);
            TableName = "C_KP_List_Item".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<string> GetItemDetailID(string ID, OleExec DB)
        {
            List<string> ret = new List<string>();
            string strSql = $@"select ID from c_kp_list_item_detail c where c.item_id='{ID}' order by c.seq ";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            }
            return ret;
        }

        public List<C_KP_List_Item> GetItemObjectByListId(string listID,OleExec sfcdb)
        {
            List<C_KP_List_Item> itemList = new List<C_KP_List_Item>();
            string sql = $@" select * from c_kp_list_item where list_id='{listID}' order by seq ";
            DataSet ds = sfcdb.RunSelect(sql);
            Row_C_KP_List_Item rowItem;
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    rowItem = (Row_C_KP_List_Item)this.NewRow();
                    rowItem.loadData(row);
                    itemList.Add(rowItem.GetDataObject());
                }
            }
            catch(Exception)
            {
                itemList = null;
            }
            return itemList;
        }

        public DataRow GetItemAndDetailByListId(string listId, string station_name, OleExec db)
        {
            DataRow JoinRow;
            string sql = $@"select * from (select * from c_kp_list_item where list_id = '{listId}' and station = '{station_name}') A left join c_kp_list_item_detail B on A.id = B.Item_Id";
            DataSet ds = db.RunSelect(sql);
            try
            {
                JoinRow = ds.Tables[0].Rows[0];
            }
            catch (Exception)
            {
                JoinRow = null;
            }
            return JoinRow;
        }

        public C_KP_List_Item GetListItemByListId(string listId, string station_name, OleExec db)
        {
            string sql = $@"select * from c_kp_list_item where list_id = '{listId}' and station = '{station_name}'";
            DataTable table = db.ExecSelect(sql).Tables[0];
            C_KP_List_Item result = new C_KP_List_Item();
            if (table.Rows.Count > 0)
            {
                Row_C_KP_List_Item ret = (Row_C_KP_List_Item)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public List<C_KP_List_Item> GetListItemByListIdStation(string listId, string station_name, OleExec db)
        {
            string sql = $@"select * from c_kp_list_item where list_id = '{listId}' and station = '{station_name}'";
            DataTable table = db.ExecSelect(sql).Tables[0];
            List<C_KP_List_Item> result = new List<C_KP_List_Item>();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow Dr in table.Rows)
                {
                    Row_C_KP_List_Item ret = (Row_C_KP_List_Item)this.NewRow();
                    ret.loadData(Dr);
                    result.Add(ret.GetDataObject());
                }
            }

            return result;
        }

        /// <summary>
        /// add by LLF 2018-10-17
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="station_name"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<DataRow> GetKPListItemByListId(string listId, string station_name, OleExec db)
        {
            List<DataRow> ListKPItem = new List<DataRow>();
            string sql = $@"select a.ID, a.SKUNO, B.KP_PARTNO, B.STATION, B.QTY, B.SEQ, SILOADING_FLAG, PACKING_FLAG, SN_FLAG, CHECKPCBAVER_FALG, CHECKPCBMODELVER_FLAG, AUTOSTATION, AUTOSTATION2, CHECKBOXSKUNO_FLAG, TESTSTATION from c_kp_list a left join c_kp_list_item b on a.id = b.list_id
                 left join c_kp_list_item_check c
                    on b.id = c.c_kp_list_item_id where b.station = '{station_name}' and a.id='{listId}' and a.flag='1' ORDER BY b.SEQ";
            DataTable table = db.ExecSelect(sql).Tables[0];
            if (table.Rows.Count > 0)
            {
                foreach (DataRow DR in table.Rows)
                {
                    ListKPItem.Add(DR);
                }
            }
            else
            {
                ListKPItem = null;
            }
            return ListKPItem;
        }
        public int InsertKPListItem(C_KP_List_Item CKPList, OleExec DB)
        {
            return DB.ORM.Insertable(CKPList).ExecuteCommand();
        }
        public int UpdateKPListItem(C_KP_List_Item CKPList, string ListID, OleExec DB)
        {
            return DB.ORM.Updateable(CKPList).Where(t => t.ID == ListID).ExecuteCommand();
        }

        /// <summary>
        /// add by LLF 2018-10-17
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="station_name"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<DataRow> GetKPListPackFlagItemByListId(string ListId, string Station_name, OleExec db)
        {
            List<DataRow> ListKPItem = new List<DataRow>();
            string sql = $@"select a.ID, a.SKUNO, B.KP_PARTNO, B.STATION, B.QTY, B.SEQ, SILOADING_FLAG, PACKING_FLAG, SN_FLAG, CHECKPCBAVER_FALG, CHECKPCBMODELVER_FLAG, AUTOSTATION, AUTOSTATION2, CHECKBOXSKUNO_FLAG, TESTSTATION from c_kp_list a left join c_kp_list_item b on a.id = b.list_id
                 left join c_kp_list_item_check c
                    on b.id = c.c_kp_list_item_id where b.station = '{Station_name}' and a.id='{ListId}' and c.packing_flag='1' ORDER BY b.SEQ desc";
            DataTable table = db.ExecSelect(sql).Tables[0];
            if (table.Rows.Count > 0)
            {
                foreach (DataRow DR in table.Rows)
                {
                    ListKPItem.Add(DR);
                }
            }
            else
            {
                ListKPItem = null;
            }
            return ListKPItem;
        }
    }
    public class Row_C_KP_List_Item : DataObjectBase
    {
        public Row_C_KP_List_Item(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_List_Item GetDataObject()
        {
            C_KP_List_Item DataObject = new C_KP_List_Item();
            DataObject.ID = this.ID;
            DataObject.LIST_ID = this.LIST_ID;
            DataObject.KP_NAME = this.KP_NAME;
            DataObject.KP_PARTNO = this.KP_PARTNO;
            DataObject.STATION = this.STATION;
            DataObject.QTY = this.QTY;
            DataObject.SEQ = this.SEQ;
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
        public string LIST_ID
        {
            get
            {
                return (string)this["LIST_ID"];
            }
            set
            {
                this["LIST_ID"] = value;
            }
        }
        public string KP_NAME
        {
            get
            {
                return (string)this["KP_NAME"];
            }
            set
            {
                this["KP_NAME"] = value;
            }
        }
        public string KP_PARTNO
        {
            get
            {
                return (string)this["KP_PARTNO"];
            }
            set
            {
                this["KP_PARTNO"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
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
    public class C_KP_List_Item
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string LIST_ID{get;set;}
        public string KP_NAME{get;set;}
        public string KP_PARTNO{get;set;}
        public string STATION{get;set;}
        public double? QTY{get;set;}
        public double? SEQ{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}