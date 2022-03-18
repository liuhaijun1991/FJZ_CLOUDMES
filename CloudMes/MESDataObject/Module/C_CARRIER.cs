using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_CARRIER : DataObjectTable
    {
        public T_C_CARRIER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CARRIER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CARRIER);
            TableName = "C_CARRIER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool isCarrierNoExist(string CARRIERNO,OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER>().Where(cc => cc.CARRIERNO == CARRIERNO&& cc.VALID_FLAG=="1").Any();
        }
        public C_CARRIER GET_byID(string id, OleExec DB)
        {
            List<C_CARRIER> Sds = DB.ORM.Queryable<C_CARRIER>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }
        public C_CARRIER GET_byCarrierNo(string CARRIERNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER>().Where(cc => cc.CARRIERNO == CARRIERNO && cc.VALID_FLAG == "1").First();
        }

        public int AddUsetimes(string CARRIERNO, OleExec DB)
        {
            int usetimes = DB.ORM.Queryable<C_CARRIER>()
                .Where(it => it.CARRIERNO == CARRIERNO && it.VALID_FLAG == "1")
                .Select(it=>it.USETIMES)
                .First()+1;

            return DB.ORM.Updateable<C_CARRIER>()
                .UpdateColumns(it => 
                new C_CARRIER {
                    USETIMES= usetimes
                })
                .Where(it => it.CARRIERNO == CARRIERNO && it.VALID_FLAG == "1")
                .ExecuteCommand();
        }
        public List<C_CARRIER_MENU_LIST> GetSnSkunoByCarrierNo(string CARRIERNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER,R_CARRIER_SKUNO_LINK>
                ((a, b) => a.CARRIER_SKUNO == b.CARRIER_SKUNO)
                .Where((a, b) => a.VALID_FLAG == "1"
                && a.CARRIERNO == CARRIERNO)
                .Select((a,b) => new C_CARRIER_MENU_LIST
                {
                    ID = a.ID,
                    CARRIER_SKUNO = a.CARRIER_SKUNO,
                    SKUNO=b.SKUNO,
                    EDITTIME = a.EDITTIME,
                    EDITBY = a.EDITBY
                })
                .ToList();
        }
        public List<C_CARRIER> GetValidData(OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER>()
                .Where(it => it.VALID_FLAG == "1")
                .OrderBy(it => it.EDITTIME, SqlSugar.OrderByType.Desc)
                .ToList();
        }
        public List<C_CARRIER_MENU_LIST> GetCCarrierMenu(OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER, C_CARRIER_SKUNO_INFO>
                ((A, B) => A.CARRIER_SKUNO == B.CARRIER_SKUNO)
                .Where((A, B) => A.VALID_FLAG == "1" && B.VALID_FLAG == "1")
                .Select((A, B) => new C_CARRIER_MENU_LIST {
                    ID = A.ID,
                    CARRIERNO = A.CARRIERNO,
                    CARRIER_SKUNO = A.CARRIER_SKUNO,
                    USETIMES = A.USETIMES,
                    SUSELIMIT = B.SUSELIMIT,
                    EDITBY = A.EDITBY,
                    EDITTIME = A.EDITTIME
                })
                .OrderBy(a=>a.EDITTIME,SqlSugar.OrderByType.Desc)
                .ToList();
        }
        public int setValidFlag0byID(string ID,string emp,DateTime? time, OleExec DB)
        {
            return DB.ORM.Updateable<C_CARRIER>()
                .Where(it => it.ID == ID)
                .UpdateColumns(it => new C_CARRIER { VALID_FLAG = "0",EDITBY= emp, EDITTIME= time }).ExecuteCommand();
        }

        public int resetCarrierUsetimes(string ID, string emp, DateTime? time, OleExec DB)
        {
            return DB.ORM.Updateable<C_CARRIER>()
                .Where(it => it.ID == ID)
                .UpdateColumns(it => new C_CARRIER { USETIMES = 0, EDITBY = emp, EDITTIME = time }).ExecuteCommand();
        }
        public bool checkExistByCARRIERNO(string CARRIERNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_CARRIER>().Where(it => it.CARRIERNO == CARRIERNO && it.VALID_FLAG == "1").Any();
        }
        public int AddOrUpdateCCARRIER(string Operation, C_CARRIER CC, OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result = DB.ORM.Insertable<C_CARRIER>(CC).ExecuteCommand();
                    break;
                case "UPDATE":
                    result = DB.ORM.Updateable<C_CARRIER>(CC).Where(sd => sd.ID == CC.ID).ExecuteCommand();
                    break;
            }
            return result;
        }
    }
    public class Row_C_CARRIER : DataObjectBase
    {
        public Row_C_CARRIER(DataObjectInfo info) : base(info)
        {

        }
        public C_CARRIER GetDataObject()
        {
            C_CARRIER DataObject = new C_CARRIER();
            DataObject.ID = this.ID;
            DataObject.CARRIERNO = this.CARRIERNO;
            DataObject.CARRIER_SKUNO = this.CARRIER_SKUNO;
            DataObject.USETIMES = this.USETIMES;
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
        public string CARRIERNO
        {
            get
            {
                return (string)this["CARRIERNO"];
            }
            set
            {
                this["CARRIERNO"] = value;
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
        public int USETIMES
        {
            get
            {
                return (int)this["USETIMES"];
            }
            set
            {
                this["USETIMES"] = value;
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
    public class C_CARRIER
    {
        public string ID { get; set; }
        public string CARRIERNO{ get; set; }
        public string CARRIER_SKUNO { get; set; }
        public int USETIMES { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }

    public class C_CARRIER_MENU_LIST
    {
        public string ID { get; set; }
        public string CARRIERNO { get; set; }

        public string SKUNO { get; set; }
        public string CARRIER_SKUNO { get; set; }
        public int USETIMES { get; set; }
        public int SUSELIMIT { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        public DateTime? enableTime { get; set; }
        public int timeLimitFlag { get; set; }

    }

}