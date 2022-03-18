using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.DCN
{
    public class T_R_TEST_BRCD : DataObjectTable
    {
        public T_R_TEST_BRCD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_BRCD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_BRCD);
            TableName = "R_TEST_BRCD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TEST_BRCD : DataObjectBase
    {
        public Row_R_TEST_BRCD(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_BRCD GetDataObject()
        {
            R_TEST_BRCD DataObject = new R_TEST_BRCD();
            DataObject.ID = this.ID;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.TESTDATE = this.TESTDATE;
            DataObject.EVENTNAME = this.EVENTNAME;
            DataObject.STATUS = this.STATUS;
            DataObject.PARTNO = this.PARTNO;
            DataObject.SFPMPN = this.SFPMPN;
            DataObject.LOCATION = this.LOCATION;
            DataObject.SYMPTOM = this.SYMPTOM;
            DataObject.PCBASN = this.PCBASN;
            DataObject.PCBAPN = this.PCBAPN;
            DataObject.VSN = this.VSN;
            DataObject.VPN = this.VPN;
            DataObject.CSN = this.CSN;
            DataObject.CPN = this.CPN;
            DataObject.FRUPCBASN = this.FRUPCBASN;
            DataObject.TESTREPORTNAME = this.TESTREPORTNAME;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.FAILURECODE = this.FAILURECODE;
            DataObject.TRAY_SN = this.TRAY_SN;
            DataObject.TESTERNO = this.TESTERNO;
            DataObject.TEMP4 = this.TEMP4;
            DataObject.TEMP5 = this.TEMP5;
            DataObject.TATIME = this.TATIME;
            DataObject.BK1_KEY = this.BK1_KEY;
            DataObject.BK1_VALUE = this.BK1_VALUE;
            DataObject.BK2_KEY = this.BK2_KEY;
            DataObject.BK2_VALUE = this.BK2_VALUE;
            DataObject.BK3_KEY = this.BK3_KEY;
            DataObject.BK3_VALUE = this.BK3_VALUE;
            DataObject.BK4_KEY = this.BK4_KEY;
            DataObject.BK4_VALUE = this.BK4_VALUE;
            DataObject.R_TEST_RECORD_ID = this.R_TEST_RECORD_ID;
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
        public string SYSSERIALNO
        {
            get
            {
                return (string)this["SYSSERIALNO"];
            }
            set
            {
                this["SYSSERIALNO"] = value;
            }
        }
        public DateTime? TESTDATE
        {
            get
            {
                return (DateTime?)this["TESTDATE"];
            }
            set
            {
                this["TESTDATE"] = value;
            }
        }
        public string EVENTNAME
        {
            get
            {
                return (string)this["EVENTNAME"];
            }
            set
            {
                this["EVENTNAME"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string SFPMPN
        {
            get
            {
                return (string)this["SFPMPN"];
            }
            set
            {
                this["SFPMPN"] = value;
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
        public string SYMPTOM
        {
            get
            {
                return (string)this["SYMPTOM"];
            }
            set
            {
                this["SYMPTOM"] = value;
            }
        }
        public string PCBASN
        {
            get
            {
                return (string)this["PCBASN"];
            }
            set
            {
                this["PCBASN"] = value;
            }
        }
        public string PCBAPN
        {
            get
            {
                return (string)this["PCBAPN"];
            }
            set
            {
                this["PCBAPN"] = value;
            }
        }
        public string VSN
        {
            get
            {
                return (string)this["VSN"];
            }
            set
            {
                this["VSN"] = value;
            }
        }
        public string VPN
        {
            get
            {
                return (string)this["VPN"];
            }
            set
            {
                this["VPN"] = value;
            }
        }
        public string CSN
        {
            get
            {
                return (string)this["CSN"];
            }
            set
            {
                this["CSN"] = value;
            }
        }
        public string CPN
        {
            get
            {
                return (string)this["CPN"];
            }
            set
            {
                this["CPN"] = value;
            }
        }
        public string FRUPCBASN
        {
            get
            {
                return (string)this["FRUPCBASN"];
            }
            set
            {
                this["FRUPCBASN"] = value;
            }
        }
        public string TESTREPORTNAME
        {
            get
            {
                return (string)this["TESTREPORTNAME"];
            }
            set
            {
                this["TESTREPORTNAME"] = value;
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
        public string FAILURECODE
        {
            get
            {
                return (string)this["FAILURECODE"];
            }
            set
            {
                this["FAILURECODE"] = value;
            }
        }
        public string TRAY_SN
        {
            get
            {
                return (string)this["TRAY_SN"];
            }
            set
            {
                this["TRAY_SN"] = value;
            }
        }
        public string TESTERNO
        {
            get
            {
                return (string)this["TESTERNO"];
            }
            set
            {
                this["TESTERNO"] = value;
            }
        }
        public double? TEMP4
        {
            get
            {
                return (double?)this["TEMP4"];
            }
            set
            {
                this["TEMP4"] = value;
            }
        }
        public double? TEMP5
        {
            get
            {
                return (double?)this["TEMP5"];
            }
            set
            {
                this["TEMP5"] = value;
            }
        }
        public DateTime? TATIME
        {
            get
            {
                return (DateTime?)this["TATIME"];
            }
            set
            {
                this["TATIME"] = value;
            }
        }
        public string BK1_KEY
        {
            get
            {
                return (string)this["BK1_KEY"];
            }
            set
            {
                this["BK1_KEY"] = value;
            }
        }
        public string BK1_VALUE
        {
            get
            {
                return (string)this["BK1_VALUE"];
            }
            set
            {
                this["BK1_VALUE"] = value;
            }
        }
        public string BK2_KEY
        {
            get
            {
                return (string)this["BK2_KEY"];
            }
            set
            {
                this["BK2_KEY"] = value;
            }
        }
        public string BK2_VALUE
        {
            get
            {
                return (string)this["BK2_VALUE"];
            }
            set
            {
                this["BK2_VALUE"] = value;
            }
        }
        public string BK3_KEY
        {
            get
            {
                return (string)this["BK3_KEY"];
            }
            set
            {
                this["BK3_KEY"] = value;
            }
        }
        public string BK3_VALUE
        {
            get
            {
                return (string)this["BK3_VALUE"];
            }
            set
            {
                this["BK3_VALUE"] = value;
            }
        }
        public string BK4_KEY
        {
            get
            {
                return (string)this["BK4_KEY"];
            }
            set
            {
                this["BK4_KEY"] = value;
            }
        }
        public string BK4_VALUE
        {
            get
            {
                return (string)this["BK4_VALUE"];
            }
            set
            {
                this["BK4_VALUE"] = value;
            }
        }
        public string R_TEST_RECORD_ID
        {
            get
            {
                return (string)this["R_TEST_RECORD_ID"];
            }
            set
            {
                this["R_TEST_RECORD_ID"] = value;
            }
        }
    }
    public class R_TEST_BRCD
    {
        public string ID { get; set; }
        public string SYSSERIALNO { get; set; }
        public DateTime? TESTDATE { get; set; }
        public string EVENTNAME { get; set; }
        public string STATUS { get; set; }
        public string PARTNO { get; set; }
        public string SFPMPN { get; set; }
        public string LOCATION { get; set; }
        public string SYMPTOM { get; set; }
        public string PCBASN { get; set; }
        public string PCBAPN { get; set; }
        public string VSN { get; set; }
        public string VPN { get; set; }
        public string CSN { get; set; }
        public string CPN { get; set; }
        public string FRUPCBASN { get; set; }
        public string TESTREPORTNAME { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public string FAILURECODE { get; set; }
        public string TRAY_SN { get; set; }
        public string TESTERNO { get; set; }
        public double? TEMP4 { get; set; }
        public double? TEMP5 { get; set; }
        public DateTime? TATIME { get; set; }
        public string BK1_KEY { get; set; }
        public string BK1_VALUE { get; set; }
        public string BK2_KEY { get; set; }
        public string BK2_VALUE { get; set; }
        public string BK3_KEY { get; set; }
        public string BK3_VALUE { get; set; }
        public string BK4_KEY { get; set; }
        public string BK4_VALUE { get; set; }
        public string R_TEST_RECORD_ID { get; set; }
    }
}