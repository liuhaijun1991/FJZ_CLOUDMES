using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class T_O_PO_STATUS : DataObjectTable
    {
        public T_O_PO_STATUS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_PO_STATUS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_PO_STATUS);
            TableName = "O_PO_STATUS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_PO_STATUS : DataObjectBase
    {
        public Row_O_PO_STATUS(DataObjectInfo info) : base(info)
        {

        }
        public O_PO_STATUS GetDataObject()
        {
            O_PO_STATUS DataObject = new O_PO_STATUS();
            DataObject.ID = this.ID;
            DataObject.POID = this.POID;
            DataObject.STATUSID = this.STATUSID;
            DataObject.LEV = this.LEV;
            DataObject.VALIDFLAG = this.VALIDFLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string POID
        {
            get
            {
                return (string)this["POID"];
            }
            set
            {
                this["POID"] = value;
            }
        }
        public string STATUSID
        {
            get
            {
                return (string)this["STATUSID"];
            }
            set
            {
                this["STATUSID"] = value;
            }
        }
        public string LEV
        {
            get
            {
                return (string)this["LEV"];
            }
            set
            {
                this["LEV"] = value;
            }
        }
        public string VALIDFLAG
        {
            get
            {
                return (string)this["VALIDFLAG"];
            }
            set
            {
                this["VALIDFLAG"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
    }
    public class O_PO_STATUS
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string POID { get; set; }
        public string STATUSID { get; set; }
        public string LEV { get; set; }
        public string VALIDFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }

    public enum ENUM_O_PO_STATUS
    {
        [EnumValue("0")]
        ValidationI137,
        [EnumValue("1")]
        WaitCreatePreWo,
        [EnumValue("2")]
        WaitDismantle,
        [EnumValue("3")]
        OnePreUploadBom,
        [EnumValue("4")]
        AddNonBom,
        [EnumValue("5")]
        ReceiveGroupId,
        [EnumValue("6")]
        SecPreUploadBom,
        [EnumValue("7")]
        CreateWo,
        [EnumValue("8")]
        DownloadWo,
        [EnumValue("9")]
        Production,
        [EnumValue("10")]
        CBS,
        [EnumValue("11")]
        PreAsn,
        [EnumValue("11")]
        DOAShipment,
        [EnumValue("12")]
        PrintLabelAndList,
        [EnumValue("13")]
        RmqEnd,
        [EnumValue("14")]
        NotProduce,
        [EnumValue("23")]
        OnHold,
        [EnumValue("28")]
        ShipOut,
        [EnumValue("29")]
        FinalAsn,
        [EnumValue("30")]
        CancelPreAsn,
        [EnumValue("30")]
        CancelDOAShipment,
        [EnumValue("31")]
        Finish,
        [EnumValue("32")]
        Closed,
    }

}