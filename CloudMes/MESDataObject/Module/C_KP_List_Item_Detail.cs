using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_C_KP_List_Item_Detail : DataObjectTable
    {
        public T_C_KP_List_Item_Detail(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_List_Item_Detail(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_List_Item_Detail);
            TableName = "C_KP_List_Item_Detail".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_KP_List_Item_Detail> GetItemDetailObjectByItemId(string itemID, OleExec sfcdb)
        {
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            string sql = $@" select * from c_kp_list_item_detail where item_id='{itemID}' order by seq";
            DataSet ds = sfcdb.RunSelect(sql);
            Row_C_KP_List_Item_Detail rowItemDetail;
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    rowItemDetail = (Row_C_KP_List_Item_Detail)this.NewRow();
                    rowItemDetail.loadData(row);
                    itemDetailList.Add(rowItemDetail.GetDataObject());
                }
            }
            catch (Exception)
            {
                itemDetailList = null;
            }
            return itemDetailList;
        }
    }
    public class Row_C_KP_List_Item_Detail : DataObjectBase
    {
        public Row_C_KP_List_Item_Detail(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_List_Item_Detail GetDataObject()
        {
            C_KP_List_Item_Detail DataObject = new C_KP_List_Item_Detail();
            DataObject.ID = this.ID;
            DataObject.ITEM_ID = this.ITEM_ID;
            DataObject.SCANTYPE = this.SCANTYPE;
            DataObject.SEQ = this.SEQ;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.LOCATION = this.LOCATION;
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
        public string ITEM_ID
        {
            get
            {
                return (string)this["ITEM_ID"];
            }
            set
            {
                this["ITEM_ID"] = value;
            }
        }
        public string SCANTYPE
        {
            get
            {
                return (string)this["SCANTYPE"];
            }
            set
            {
                this["SCANTYPE"] = value;
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
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
    }
    public class C_KP_List_Item_Detail
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string ITEM_ID { get; set; }
        public string SCANTYPE { get; set; }
        public double? SEQ { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string LOCATION { get; set; }
    }
    [SugarTable("C_KP_List_Item_Detail")]
    public class C_KP_LIST_I_D
    {
        public string ID { get; set; }
        public string ITEM_ID { get; set; }
        public string SCANTYPE { get; set; }
        public double? SEQ { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string LOCATION { get; set; }
    }
}
