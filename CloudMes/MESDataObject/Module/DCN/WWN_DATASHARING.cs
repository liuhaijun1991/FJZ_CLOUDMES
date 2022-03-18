using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module.DCN
{
    public class T_WWN_DATASHARING : DataObjectTable
    {
        public T_WWN_DATASHARING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_WWN_DATASHARING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_WWN_DATASHARING);
            TableName = "WWN_DATASHARING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public WWN_DATASHARING GetWsnSku(string StrSN, OleExec DB)
        {
            return DB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == StrSN).ToList().FirstOrDefault();
        }
        public List<WWN_DATASHARING> GetVssnByWsn(string StrWsn,OleExec db)
        {
            return db.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == StrWsn&& t.VSSN=="N/A").ToList();
        }
        public List<WWN_DATASHARING> GetCssnByVssn(string StrWsn, OleExec db)
        {
            return db.ORM.Queryable<WWN_DATASHARING>().Where(t => t.VSSN == StrWsn && t.CSSN == "N/A").ToList();
        }

        /// <summary>
        /// 替換SN時更新WWN表
        /// </summary>
        /// <param name="new_sn"></param>
        /// <param name="old_sn"></param>
        /// <param name="SFCDB"></param>
        /// <returns></returns>
        public void ReplaceSnWWN(string new_sn, string old_sn, OleExec SFCDB)
        {
            var result = 0;
            var w = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == old_sn).Any();
            if (w)
            {
                result = SFCDB.ORM.Updateable<WWN_DATASHARING>().SetColumns(r => new WWN_DATASHARING { WSN = new_sn }).Where(r => r.WSN == old_sn).ExecuteCommand();
            }
            var v = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.VSSN == old_sn).Any();
            if (v)
            {
                result = SFCDB.ORM.Updateable<WWN_DATASHARING>().SetColumns(r => new WWN_DATASHARING { VSSN = new_sn }).Where(r => r.VSSN == old_sn).ExecuteCommand();
            }
            var c = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.CSSN == old_sn).Any();
            if (c)
            {
                result = SFCDB.ORM.Updateable<WWN_DATASHARING>().SetColumns(r => new WWN_DATASHARING { CSSN = new_sn }).Where(r => r.CSSN == old_sn).ExecuteCommand();
            }
        }
    }

    public class Row_WWN_DATASHARING : DataObjectBase
    {
        public Row_WWN_DATASHARING(DataObjectInfo info) : base(info)
        {

        }
        public WWN_DATASHARING GetDataObject()
        {
            WWN_DATASHARING DataObject = new WWN_DATASHARING();
            DataObject.ID = this.ID;
            DataObject.WSN = this.WSN;
            DataObject.SKU = this.SKU;
            DataObject.VSSN = this.VSSN;
            DataObject.VSKU = this.VSKU;
            DataObject.CSSN = this.CSSN;
            DataObject.CSKU = this.CSKU;
            DataObject.MAC = this.MAC;
            DataObject.WWN = this.WWN;
            DataObject.MAC_BLOCK_SIZE = this.MAC_BLOCK_SIZE;
            DataObject.WWN_BLOCK_SIZE = this.WWN_BLOCK_SIZE;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.MACTB0 = this.MACTB0;
            DataObject.MACTB1 = this.MACTB1;
            DataObject.MACTB2 = this.MACTB2;
            DataObject.MACTB3 = this.MACTB3;
            DataObject.MACTB4 = this.MACTB4;
            DataObject.WWNTB0 = this.WWNTB0;
            DataObject.WWNTB1 = this.WWNTB1;
            DataObject.WWNTB2 = this.WWNTB2;
            DataObject.WWNTB3 = this.WWNTB3;
            DataObject.WWNTB4 = this.WWNTB4;
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
        public string WSN
        {
            get
            {
                return (string)this["WSN"];
            }
            set
            {
                this["WSN"] = value;
            }
        }
        public string SKU
        {
            get
            {
                return (string)this["SKU"];
            }
            set
            {
                this["SKU"] = value;
            }
        }
        public string VSSN
        {
            get
            {
                return (string)this["VSSN"];
            }
            set
            {
                this["VSSN"] = value;
            }
        }
        public string VSKU
        {
            get
            {
                return (string)this["VSKU"];
            }
            set
            {
                this["VSKU"] = value;
            }
        }
        public string CSSN
        {
            get
            {
                return (string)this["CSSN"];
            }
            set
            {
                this["CSSN"] = value;
            }
        }
        public string CSKU
        {
            get
            {
                return (string)this["CSKU"];
            }
            set
            {
                this["CSKU"] = value;
            }
        }
        public string MAC
        {
            get
            {
                return (string)this["MAC"];
            }
            set
            {
                this["MAC"] = value;
            }
        }
        public string WWN
        {
            get
            {
                return (string)this["WWN"];
            }
            set
            {
                this["WWN"] = value;
            }
        }
        public double? MAC_BLOCK_SIZE
        {
            get
            {
                return (double?)this["MAC_BLOCK_SIZE"];
            }
            set
            {
                this["MAC_BLOCK_SIZE"] = value;
            }
        }
        public double? WWN_BLOCK_SIZE
        {
            get
            {
                return (double?)this["WWN_BLOCK_SIZE"];
            }
            set
            {
                this["WWN_BLOCK_SIZE"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string MACTB0
        {
            get
            {
                return (string)this["MACTB0"];
            }
            set
            {
                this["MACTB0"] = value;
            }
        }
        public string MACTB1
        {
            get
            {
                return (string)this["MACTB1"];
            }
            set
            {
                this["MACTB1"] = value;
            }
        }
        public string MACTB2
        {
            get
            {
                return (string)this["MACTB2"];
            }
            set
            {
                this["MACTB2"] = value;
            }
        }
        public string MACTB3
        {
            get
            {
                return (string)this["MACTB3"];
            }
            set
            {
                this["MACTB3"] = value;
            }
        }
        public string MACTB4
        {
            get
            {
                return (string)this["MACTB4"];
            }
            set
            {
                this["MACTB4"] = value;
            }
        }
        public string WWNTB0
        {
            get
            {
                return (string)this["WWNTB0"];
            }
            set
            {
                this["WWNTB0"] = value;
            }
        }
        public string WWNTB1
        {
            get
            {
                return (string)this["WWNTB1"];
            }
            set
            {
                this["WWNTB1"] = value;
            }
        }
        public string WWNTB2
        {
            get
            {
                return (string)this["WWNTB2"];
            }
            set
            {
                this["WWNTB2"] = value;
            }
        }
        public string WWNTB3
        {
            get
            {
                return (string)this["WWNTB3"];
            }
            set
            {
                this["WWNTB3"] = value;
            }
        }
        public string WWNTB4
        {
            get
            {
                return (string)this["WWNTB4"];
            }
            set
            {
                this["WWNTB4"] = value;
            }
        }
    }
    public class WWN_DATASHARING
    {
        public string ID { get; set; }
        public string WSN { get; set; }
        public string SKU { get; set; }
        public string VSSN { get; set; }
        public string VSKU { get; set; }
        public string CSSN { get; set; }
        public string CSKU { get; set; }
        public string MAC { get; set; }
        public string WWN { get; set; }
        public double? MAC_BLOCK_SIZE { get; set; }
        public double? WWN_BLOCK_SIZE { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string MACTB0 { get; set; }
        public string MACTB1 { get; set; }
        public string MACTB2 { get; set; }
        public string MACTB3 { get; set; }
        public string MACTB4 { get; set; }
        public string WWNTB0 { get; set; }
        public string WWNTB1 { get; set; }
        public string WWNTB2 { get; set; }
        public string WWNTB3 { get; set; }
        public string WWNTB4 { get; set; }
    }
}
