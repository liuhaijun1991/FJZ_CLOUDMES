using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CUSTSN_T : DataObjectTable
    {
        public bool NotCheck(string name)
        {
            bool flag = true;
            string[] list = new string[] { "MO_NUMBER", "GROUP_NAME", "IN_STATION_TIME", "CARTON_NO", "EMPNO", "SSN21", "SSN22" };
            foreach (string item in list)
            {
                if (item == name)
                {
                    flag = false;
                }
            }
            return flag;
        }
        public T_R_CUSTSN_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CUSTSN_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CUSTSN_T);
            TableName = "R_CUSTSN_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_CUSTSN_T GetTestDataBySn(string SN, OleExec DB, DB_TYPE_ENUM DBType)
        {
            return DB.ORM.Queryable<R_CUSTSN_T>().Where(t => t.SERIAL_NUMBER == SN).ToList().FirstOrDefault();
        }

        public int CheckSSNorMAC(string SN, FtControl ftControl, OleExec DB)
        {
            string sql = $@" SELECT COUNT (serial_number)
                                     FROM r_custsn_t
                                    WHERE serial_number <> '{SN}'  AND {ftControl.CheckType} = '{ftControl.Value}'  ";
            DataTable res = DB.ExecSelect(sql).Tables[0];
            return Convert.ToInt16(res.Rows[0][0].ToString());
            //return DB.ORM.Queryable<R_CUSTSN_T>().Where(t => t.SERIAL_NUMBER == SN).ToList().FirstOrDefault();
        }
    }
    public class Row_R_CUSTSN_T : DataObjectBase
    {
        public Row_R_CUSTSN_T(DataObjectInfo info) : base(info)
        {

        }
        public R_CUSTSN_T GetDataObject()
        {
            R_CUSTSN_T DataObject = new R_CUSTSN_T();
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.MO_NUMBER = this.MO_NUMBER;
            DataObject.GROUP_NAME = this.GROUP_NAME;
            DataObject.SSN1 = this.SSN1;
            DataObject.SSN2 = this.SSN2;
            DataObject.SSN3 = this.SSN3;
            DataObject.SSN4 = this.SSN4;
            DataObject.SSN5 = this.SSN5;
            DataObject.SSN6 = this.SSN6;
            DataObject.SSN7 = this.SSN7;
            DataObject.SSN8 = this.SSN8;
            DataObject.SSN9 = this.SSN9;
            DataObject.SSN10 = this.SSN10;
            DataObject.SSN11 = this.SSN11;
            DataObject.SSN12 = this.SSN12;
            DataObject.MAC1 = this.MAC1;
            DataObject.MAC2 = this.MAC2;
            DataObject.MAC3 = this.MAC3;
            DataObject.MAC4 = this.MAC4;
            DataObject.MAC5 = this.MAC5;
            DataObject.MAC6 = this.MAC6;
            DataObject.IN_STATION_TIME = this.IN_STATION_TIME;
            DataObject.CARTON_NO = this.CARTON_NO;
            DataObject.EMP_NO = this.EMP_NO;
            DataObject.MAC7 = this.MAC7;
            DataObject.MAC8 = this.MAC8;
            DataObject.MAC9 = this.MAC9;
            DataObject.MAC10 = this.MAC10;
            DataObject.MAC11 = this.MAC11;
            DataObject.MAC12 = this.MAC12;
            DataObject.MAC13 = this.MAC13;
            DataObject.MAC14 = this.MAC14;
            DataObject.MAC15 = this.MAC15;
            DataObject.SSN13 = this.SSN13;
            DataObject.SSN14 = this.SSN14;
            DataObject.SSN15 = this.SSN15;
            DataObject.SSN16 = this.SSN16;
            DataObject.MAC16 = this.MAC16;
            DataObject.SSN17 = this.SSN17;
            DataObject.MAC17 = this.MAC17;
            DataObject.SSN18 = this.SSN18;
            DataObject.MAC18 = this.MAC18;
            DataObject.SSN19 = this.SSN19;
            DataObject.MAC19 = this.MAC19;
            DataObject.SSN20 = this.SSN20;
            DataObject.MAC20 = this.MAC20;
            DataObject.SSN21 = this.SSN21;
            DataObject.SSN22 = this.SSN22;
            return DataObject;
        }
        public string SERIAL_NUMBER
        {
            get
            {
                return (string)this["SERIAL_NUMBER"];
            }
            set
            {
                this["SERIAL_NUMBER"] = value;
            }
        }
        public string MO_NUMBER
        {
            get
            {
                return (string)this["MO_NUMBER"];
            }
            set
            {
                this["MO_NUMBER"] = value;
            }
        }
        public string GROUP_NAME
        {
            get
            {
                return (string)this["GROUP_NAME"];
            }
            set
            {
                this["GROUP_NAME"] = value;
            }
        }
        public string SSN1
        {
            get
            {
                return (string)this["SSN1"];
            }
            set
            {
                this["SSN1"] = value;
            }
        }
        public string SSN2
        {
            get
            {
                return (string)this["SSN2"];
            }
            set
            {
                this["SSN2"] = value;
            }
        }
        public string SSN3
        {
            get
            {
                return (string)this["SSN3"];
            }
            set
            {
                this["SSN3"] = value;
            }
        }
        public string SSN4
        {
            get
            {
                return (string)this["SSN4"];
            }
            set
            {
                this["SSN4"] = value;
            }
        }
        public string SSN5
        {
            get
            {
                return (string)this["SSN5"];
            }
            set
            {
                this["SSN5"] = value;
            }
        }
        public string SSN6
        {
            get
            {
                return (string)this["SSN6"];
            }
            set
            {
                this["SSN6"] = value;
            }
        }
        public string SSN7
        {
            get
            {
                return (string)this["SSN7"];
            }
            set
            {
                this["SSN7"] = value;
            }
        }
        public string SSN8
        {
            get
            {
                return (string)this["SSN8"];
            }
            set
            {
                this["SSN8"] = value;
            }
        }
        public string SSN9
        {
            get
            {
                return (string)this["SSN9"];
            }
            set
            {
                this["SSN9"] = value;
            }
        }
        public string SSN10
        {
            get
            {
                return (string)this["SSN10"];
            }
            set
            {
                this["SSN10"] = value;
            }
        }
        public string SSN11
        {
            get
            {
                return (string)this["SSN11"];
            }
            set
            {
                this["SSN11"] = value;
            }
        }
        public string SSN12
        {
            get
            {
                return (string)this["SSN12"];
            }
            set
            {
                this["SSN12"] = value;
            }
        }
        public string MAC1
        {
            get
            {
                return (string)this["MAC1"];
            }
            set
            {
                this["MAC1"] = value;
            }
        }
        public string MAC2
        {
            get
            {
                return (string)this["MAC2"];
            }
            set
            {
                this["MAC2"] = value;
            }
        }
        public string MAC3
        {
            get
            {
                return (string)this["MAC3"];
            }
            set
            {
                this["MAC3"] = value;
            }
        }
        public string MAC4
        {
            get
            {
                return (string)this["MAC4"];
            }
            set
            {
                this["MAC4"] = value;
            }
        }
        public string MAC5
        {
            get
            {
                return (string)this["MAC5"];
            }
            set
            {
                this["MAC5"] = value;
            }
        }
        public string MAC6
        {
            get
            {
                return (string)this["MAC6"];
            }
            set
            {
                this["MAC6"] = value;
            }
        }
        public DateTime? IN_STATION_TIME
        {
            get
            {
                return (DateTime?)this["IN_STATION_TIME"];
            }
            set
            {
                this["IN_STATION_TIME"] = value;
            }
        }
        public string CARTON_NO
        {
            get
            {
                return (string)this["CARTON_NO"];
            }
            set
            {
                this["CARTON_NO"] = value;
            }
        }
        public string EMP_NO
        {
            get
            {
                return (string)this["EMP_NO"];
            }
            set
            {
                this["EMP_NO"] = value;
            }
        }
        public string MAC7
        {
            get
            {
                return (string)this["MAC7"];
            }
            set
            {
                this["MAC7"] = value;
            }
        }
        public string MAC8
        {
            get
            {
                return (string)this["MAC8"];
            }
            set
            {
                this["MAC8"] = value;
            }
        }
        public string MAC9
        {
            get
            {
                return (string)this["MAC9"];
            }
            set
            {
                this["MAC9"] = value;
            }
        }
        public string MAC10
        {
            get
            {
                return (string)this["MAC10"];
            }
            set
            {
                this["MAC10"] = value;
            }
        }
        public string MAC11
        {
            get
            {
                return (string)this["MAC11"];
            }
            set
            {
                this["MAC11"] = value;
            }
        }
        public string MAC12
        {
            get
            {
                return (string)this["MAC12"];
            }
            set
            {
                this["MAC12"] = value;
            }
        }
        public string MAC13
        {
            get
            {
                return (string)this["MAC13"];
            }
            set
            {
                this["MAC13"] = value;
            }
        }
        public string MAC14
        {
            get
            {
                return (string)this["MAC14"];
            }
            set
            {
                this["MAC14"] = value;
            }
        }
        public string MAC15
        {
            get
            {
                return (string)this["MAC15"];
            }
            set
            {
                this["MAC15"] = value;
            }
        }
        public string SSN13
        {
            get
            {
                return (string)this["SSN13"];
            }
            set
            {
                this["SSN13"] = value;
            }
        }
        public string SSN14
        {
            get
            {
                return (string)this["SSN14"];
            }
            set
            {
                this["SSN14"] = value;
            }
        }
        public string SSN15
        {
            get
            {
                return (string)this["SSN15"];
            }
            set
            {
                this["SSN15"] = value;
            }
        }
        public string SSN16
        {
            get
            {
                return (string)this["SSN16"];
            }
            set
            {
                this["SSN16"] = value;
            }
        }
        public string MAC16
        {
            get
            {
                return (string)this["MAC16"];
            }
            set
            {
                this["MAC16"] = value;
            }
        }
        public string SSN17
        {
            get
            {
                return (string)this["SSN17"];
            }
            set
            {
                this["SSN17"] = value;
            }
        }
        public string MAC17
        {
            get
            {
                return (string)this["MAC17"];
            }
            set
            {
                this["MAC17"] = value;
            }
        }
        public string SSN18
        {
            get
            {
                return (string)this["SSN18"];
            }
            set
            {
                this["SSN18"] = value;
            }
        }
        public string MAC18
        {
            get
            {
                return (string)this["MAC18"];
            }
            set
            {
                this["MAC18"] = value;
            }
        }
        public string SSN19
        {
            get
            {
                return (string)this["SSN19"];
            }
            set
            {
                this["SSN19"] = value;
            }
        }
        public string MAC19
        {
            get
            {
                return (string)this["MAC19"];
            }
            set
            {
                this["MAC19"] = value;
            }
        }
        public string SSN20
        {
            get
            {
                return (string)this["SSN20"];
            }
            set
            {
                this["SSN20"] = value;
            }
        }
        public string MAC20
        {
            get
            {
                return (string)this["MAC20"];
            }
            set
            {
                this["MAC20"] = value;
            }
        }
        public string SSN21
        {
            get
            {
                return (string)this["SSN21"];
            }
            set
            {
                this["SSN21"] = value;
            }
        }
        public string SSN22
        {
            get
            {
                return (string)this["SSN22"];
            }
            set
            {
                this["SSN22"] = value;
            }
        }
    }
    public class R_CUSTSN_T
    {
        public string SERIAL_NUMBER { get; set; }
        public string MO_NUMBER { get; set; }
        public string GROUP_NAME { get; set; }
        public string SSN1 { get; set; }
        public string SSN2 { get; set; }
        public string SSN3 { get; set; }
        public string SSN4 { get; set; }
        public string SSN5 { get; set; }
        public string SSN6 { get; set; }
        public string SSN7 { get; set; }
        public string SSN8 { get; set; }
        public string SSN9 { get; set; }
        public string SSN10 { get; set; }
        public string SSN11 { get; set; }
        public string SSN12 { get; set; }
        public string MAC1 { get; set; }
        public string MAC2 { get; set; }
        public string MAC3 { get; set; }
        public string MAC4 { get; set; }
        public string MAC5 { get; set; }
        public string MAC6 { get; set; }
        public DateTime? IN_STATION_TIME { get; set; }
        public string CARTON_NO { get; set; }
        public string EMP_NO { get; set; }
        public string MAC7 { get; set; }
        public string MAC8 { get; set; }
        public string MAC9 { get; set; }
        public string MAC10 { get; set; }
        public string MAC11 { get; set; }
        public string MAC12 { get; set; }
        public string MAC13 { get; set; }
        public string MAC14 { get; set; }
        public string MAC15 { get; set; }
        public string SSN13 { get; set; }
        public string SSN14 { get; set; }
        public string SSN15 { get; set; }
        public string SSN16 { get; set; }
        public string MAC16 { get; set; }
        public string SSN17 { get; set; }
        public string MAC17 { get; set; }
        public string SSN18 { get; set; }
        public string MAC18 { get; set; }
        public string SSN19 { get; set; }
        public string MAC19 { get; set; }
        public string SSN20 { get; set; }
        public string MAC20 { get; set; }
        public string SSN21 { get; set; }
        public string SSN22 { get; set; }
    }
}