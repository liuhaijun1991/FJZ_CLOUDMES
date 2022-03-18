using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_CARRIER_SKUNO_INFO : DataObjectTable
    {
        public T_C_CARRIER_SKUNO_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CARRIER_SKUNO_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CARRIER_SKUNO_INFO);
            TableName = "C_CARRIER_SKUNO_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_CARRIER_SKUNO_INFO GET_byID(string id, OleExec DB)
        {
            List<C_CARRIER_SKUNO_INFO> Sds = DB.ORM.Queryable<C_CARRIER_SKUNO_INFO>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }
        public C_CARRIER_SKUNO_INFO GET_byCarrierSkuno(string CARRIERSKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER_SKUNO_INFO>().Where(si => si.CARRIER_SKUNO == CARRIERSKUNO && si.VALID_FLAG == "1").First();
        }
    
        public List<C_CARRIER_SKUNO_INFO> GetValidData(OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER_SKUNO_INFO>()
                .Where(it => it.VALID_FLAG == "1")
                .OrderBy(it => it.EDITTIME, SqlSugar.OrderByType.Desc)
                .ToList();
        }

        public List<C_CARRIER_SKUNO_INFO> GetValidData(string SEARCHTEXT,OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER_SKUNO_INFO, R_CARRIER_SKUNO_LINK>((a, b) => a.CARRIER_SKUNO == b.CARRIER_SKUNO)
                .Where(
                (a, b) => a.VALID_FLAG == "1" && (
                    a.CARRIER_SKUNO.Contains(SEARCHTEXT)
                    || a.CARRIER_TYPE.Contains(SEARCHTEXT)
                    || a.CARRIER_NAME.Contains(SEARCHTEXT)
                    || a.CUSTOMER_NAME.Contains(SEARCHTEXT)
                    || a.CARRIER_MFR.Contains(SEARCHTEXT)
                    || a.LOCATION.Contains(SEARCHTEXT)
                    || a.EDITBY.Contains(SEARCHTEXT)
                    || b.SKUNO.Contains(SEARCHTEXT)
                    )
                )
                .GroupBy(a => new { a.ID, a.CARRIER_SKUNO, a.CARRIER_TYPE, a.CARRIER_NAME, a.CUSTOMER_NAME, a.CARRIER_MFR, a.LOCATION, a.SUSELIMIT, a.MAXUSELIMIT, a.LINKQTY, a.EDITTIME, a.EDITBY })
                .Select(a => new C_CARRIER_SKUNO_INFO {
                    ID = a.ID,
                    CARRIER_SKUNO = a.CARRIER_SKUNO,
                    CARRIER_TYPE = a.CARRIER_TYPE,
                    CARRIER_NAME = a.CARRIER_NAME,
                    CUSTOMER_NAME = a.CUSTOMER_NAME,
                    CARRIER_MFR = a.CARRIER_MFR,
                    LOCATION = a.LOCATION,
                    SUSELIMIT = a.SUSELIMIT,
                    MAXUSELIMIT = a.MAXUSELIMIT,
                    LINKQTY = a.LINKQTY,
                    EDITTIME = a.EDITTIME,
                    EDITBY = a.EDITBY
                })
                .OrderBy(a => a.EDITTIME, SqlSugar.OrderByType.Desc)
                .ToList();
        }

        public bool checkExistByCarrierskuno(string CARRIER_SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER_SKUNO_INFO>().Where(it => it.CARRIER_SKUNO == CARRIER_SKUNO && it.VALID_FLAG == "1")
                .Any();
        }
        public int AddOrUpdateCCSI(string Operation, C_CARRIER_SKUNO_INFO CCSI, OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result = DB.ORM.Insertable<C_CARRIER_SKUNO_INFO>(CCSI).ExecuteCommand();
                    break;
                case "UPDATE":
                    result = DB.ORM.Updateable<C_CARRIER_SKUNO_INFO>(CCSI).Where(sd => sd.ID == CCSI.ID).ExecuteCommand();
                    break;
            }
            return result;
        }
        public int setValidFlag0byID(string ID,string emp,DateTime? time ,OleExec DB)
        {
            return DB.ORM.Updateable<C_CARRIER_SKUNO_INFO>()
                .Where(it => it.ID == ID)
                .UpdateColumns(it => new C_CARRIER_SKUNO_INFO { VALID_FLAG="0",EDITBY=emp,EDITTIME= time }).ExecuteCommand();
        }

    }
    public class Row_C_CARRIER_SKUNO_INFO : DataObjectBase
    {
        public Row_C_CARRIER_SKUNO_INFO(DataObjectInfo info) : base(info)
        {

        }
        public C_CARRIER_SKUNO_INFO GetDataObject()
        {
            C_CARRIER_SKUNO_INFO DataObject = new C_CARRIER_SKUNO_INFO();
            DataObject.ID = this.ID;
            DataObject.CARRIER_SKUNO = this.CARRIER_SKUNO;
            DataObject.CARRIER_TYPE = this.CARRIER_TYPE;
            DataObject.CARRIER_NAME = this.CARRIER_NAME;
            DataObject.CUSTOMER_NAME = this.CUSTOMER_NAME;
            DataObject.CARRIER_MFR = this.CARRIER_MFR;
            DataObject.LOCATION = this.LOCATION;
            DataObject.SUSELIMIT = this.SUSELIMIT;
            DataObject.MAXUSELIMIT = this.MAXUSELIMIT;
            DataObject.LINKQTY = this.LINKQTY;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string CARRIER_SKUNO
        {
            get
            {
                return (string)this["CARRIER_SKUNO"];
            }
            set
            {
                this["CARRIER_SKUNO"] = value;
            }
        }
        public string CARRIER_TYPE
        {
            get
            {
                return (string)this["CARRIER_TYPE"];
            }
            set
            {
                this["CARRIER_TYPE"] = value;
            }
        }
        public string CARRIER_NAME
        {
            get
            {
                return (string)this["CARRIER_NAME"];
            }
            set
            {
                this["CARRIER_NAME"] = value;
            }
        }
        public string CUSTOMER_NAME
        {
            get
            {
                return (string)this["CUSTOMER_NAME"];
            }
            set
            {
                this["CUSTOMER_NAME"] = value;
            }
        }
        public string CARRIER_MFR
        {
            get
            {
                return (string)this["CARRIER_MFR"];
            }
            set
            {
                this["CARRIER_MFR"] = value;
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
        public int SUSELIMIT
        {
            get
            {
                return (int)this["SUSELIMIT"];
            }
            set
            {
                this["SUSELIMIT"] = value;
            }
        }
        public int MAXUSELIMIT
        {
            get
            {
                return (int)this["MAXUSELIMIT"];
            }
            set
            {
                this["MAXUSELIMIT"] = value;
            }
        }
        public int LINKQTY
        {
            get
            {
                return (int)this["LINKQTY"];
            }
            set
            {
                this["LINKQTY"] = value;
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
    public class C_CARRIER_SKUNO_INFO
    {
        public string ID { get; set; }
        public string CARRIER_SKUNO { get; set; }
        public string CARRIER_TYPE { get; set; }
        public string CARRIER_NAME { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CARRIER_MFR { get; set; }
        public string LOCATION { get; set; }
        public int SUSELIMIT { get; set; }
        public int MAXUSELIMIT { get; set; }
        public int LINKQTY { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }

    }
}