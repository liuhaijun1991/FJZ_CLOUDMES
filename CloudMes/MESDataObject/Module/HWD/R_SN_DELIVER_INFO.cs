using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
 
    public class T_R_SN_DELIVER_INFO : DataObjectTable
    {
        public T_R_SN_DELIVER_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {


        }
        public T_R_SN_DELIVER_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_DELIVER_INFO);
            TableName = "R_SN_DELIVER_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public void ChangeSnStatus(R_SN_DELIVER_INFO DELIVER, string StationName, string EmpNo, OleExec DB)
        {
            if (DELIVER != null)
            {
                if (StationName.Contains("OQC_CHECK"))
                {
                    DELIVER.CHECK_FLAG = "1";
                    DELIVER.CHECK_TIME = GetDBDateTime(DB);
                    DELIVER.CURRENT_STATION = StationName;
                    DELIVER.NEXT_STATION = "DELIVER_CHECK";
                    DELIVER.CHECK_EMP = EmpNo;
                }
                else if (StationName.Contains("DELIVER_CHECK"))
                {
                    DELIVER.DELIVER_FLAG = "1";
                    DELIVER.DELIVER_TIME = GetDBDateTime(DB);
                    DELIVER.CURRENT_STATION = StationName;
                    DELIVER.NEXT_STATION = "JOBFINISH";
                    DELIVER.DELIVER_EMP = EmpNo;
                }
                 
                DELIVER.EDIT_TIME = GetDBDateTime(DB);
                DELIVER.EDIT_EMP = EmpNo;
                DB.ORM.Updateable(DELIVER).Where(t => t.ID == DELIVER.ID).ExecuteCommand();
            }
        }

    }
    public class Row_R_SN_DELIVER_INFO : DataObjectBase
    {
        public Row_R_SN_DELIVER_INFO(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_DELIVER_INFO GetDataObject()
        {
            R_SN_DELIVER_INFO DataObject = new R_SN_DELIVER_INFO();
            DataObject.ID = this.ID;
            DataObject.ORDERNO = this.ORDERNO;
            DataObject.IMEI = this.IMEI;
            DataObject.CARTON_SN = this.CARTON_SN;
            DataObject.PALLET_SN = this.PALLET_SN;
            DataObject.CHECK_FLAG = this.CHECK_FLAG;
            DataObject.CHECK_EMP = this.CHECK_EMP;
            DataObject.CHECK_TIME = this.CHECK_TIME;
            DataObject.DELIVER_FLAG = this.DELIVER_FLAG;
            DataObject.DELIVER_EMP = this.DELIVER_EMP;
            DataObject.DELIVER_TIME = this.DELIVER_TIME;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.REMARK = this.REMARK;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.DATALOAD_EMP = this.DATALOAD_EMP;
            DataObject.DATALOAD_TIME = this.DATALOAD_TIME;
            DataObject.PLANT = this.PLANT;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
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
        public string ORDERNO
        {
            get
            {
                return (string)this["ORDERNO"];
            }
            set
            {
                this["ORDERNO"] = value;
            }
        }
        public string IMEI
        {
            get
            {
                return (string)this["IMEI"];
            }
            set
            {
                this["IMEI"] = value;
            }
        }
        public string CARTON_SN
        {
            get
            {
                return (string)this["CARTON_SN"];
            }
            set
            {
                this["CARTON_SN"] = value;
            }
        }
        public string PALLET_SN
        {
            get
            {
                return (string)this["PALLET_SN"];
            }
            set
            {
                this["PALLET_SN"] = value;
            }
        }
        public string CHECK_FLAG
        {
            get
            {
                return (string)this["CHECK_FLAG"];
            }
            set
            {
                this["CHECK_FLAG"] = value;
            }
        }
        public string CHECK_EMP
        {
            get
            {
                return (string)this["CHECK_EMP"];
            }
            set
            {
                this["CHECK_EMP"] = value;
            }
        }
        public DateTime? CHECK_TIME
        {
            get
            {
                return (DateTime?)this["CHECK_TIME"];
            }
            set
            {
                this["CHECK_TIME"] = value;
            }
        }
        public string DELIVER_FLAG
        {
            get
            {
                return (string)this["DELIVER_FLAG"];
            }
            set
            {
                this["DELIVER_FLAG"] = value;
            }
        }
        public string DELIVER_EMP
        {
            get
            {
                return (string)this["DELIVER_EMP"];
            }
            set
            {
                this["DELIVER_EMP"] = value;
            }
        }
        public DateTime? DELIVER_TIME
        {
            get
            {
                return (DateTime?)this["DELIVER_TIME"];
            }
            set
            {
                this["DELIVER_TIME"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
        public string DATALOAD_EMP
        {
            get
            {
                return (string)this["DATALOAD_EMP"];
            }
            set
            {
                this["DATALOAD_EMP"] = value;
            }
        }
        public DateTime? DATALOAD_TIME
        {
            get
            {
                return (DateTime?)this["DATALOAD_TIME"];
            }
            set
            {
                this["DATALOAD_TIME"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
    }
    public class R_SN_DELIVER_INFO
    {
        public string ID { get; set; }
        public string ORDERNO { get; set; }
        public string IMEI { get; set; }
        public string CARTON_SN { get; set; }
        public string PALLET_SN { get; set; }
        public string CHECK_FLAG { get; set; }
        public string CHECK_EMP { get; set; }
        public DateTime? CHECK_TIME { get; set; }
        public string DELIVER_FLAG { get; set; }
        public string DELIVER_EMP { get; set; }
        public DateTime? DELIVER_TIME { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string VALID_FLAG { get; set; }
        public string REMARK { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string DATALOAD_EMP { get; set; }
        public DateTime? DATALOAD_TIME { get; set; }
        public string PLANT { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
    }
}
