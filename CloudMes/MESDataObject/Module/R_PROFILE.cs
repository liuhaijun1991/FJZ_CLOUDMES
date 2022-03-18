using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_r_profile : DataObjectTable
    {
        public T_r_profile(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_profile(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_profile);
            TableName = "r_profile".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public r_profile GetProfile(string profilename, string profilecategory,string profiletype,string profilevalue, OleExec DB)
        {
            return DB.ORM.Queryable<r_profile>().Where(r => r.PROFILENAME == profilename && r.PROFILECATEGORY == profilecategory && r.PROFILETYPE == profiletype && r.PROFILEVALUE== profilevalue).ToList().FirstOrDefault();
        }

        public int SaveNewProfile(r_profile profile, OleExec DB)
        {
            return DB.ORM.Insertable<r_profile>(profile).ExecuteCommand();
        }

        public int Update(r_profile profile, OleExec DB)
        {
            return DB.ORM.Updateable<r_profile>(profile).Where(r => r.ID == profile.ID).ExecuteCommand();
        }
    }
    public class Row_r_profile : DataObjectBase
    {
        public Row_r_profile(DataObjectInfo info) : base(info)
        {

        }
        public r_profile GetDataObject()
        {
            r_profile DataObject = new r_profile();
            DataObject.ID = this.ID;
            DataObject.PROFILENAME = this.PROFILENAME;
            DataObject.PROFILECATEGORY = this.PROFILECATEGORY;
            DataObject.PROFILETYPE = this.PROFILETYPE;
            DataObject.PROFILEVALUE = this.PROFILEVALUE;
            DataObject.PROFILEDESC = this.PROFILEDESC;
            DataObject.PROFILESORT = this.PROFILESORT;
            DataObject.PROFILELEVEL = this.PROFILELEVEL;
            DataObject.NOTE1 = this.NOTE1;
            DataObject.NOTE2 = this.NOTE2;
            DataObject.NOTE3 = this.NOTE3;
            DataObject.NOTE4 = this.NOTE4;
            DataObject.NOTE5 = this.NOTE5;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string PROFILENAME
        {
            get
            {
                return (string)this["PROFILENAME"];
            }
            set
            {
                this["PROFILENAME"] = value;
            }
        }
        public string PROFILECATEGORY
        {
            get
            {
                return (string)this["PROFILECATEGORY"];
            }
            set
            {
                this["PROFILECATEGORY"] = value;
            }
        }
        public string PROFILETYPE
        {
            get
            {
                return (string)this["PROFILETYPE"];
            }
            set
            {
                this["PROFILETYPE"] = value;
            }
        }
        public string PROFILEVALUE
        {
            get
            {
                return (string)this["PROFILEVALUE"];
            }
            set
            {
                this["PROFILEVALUE"] = value;
            }
        }
        public string PROFILEDESC
        {
            get
            {
                return (string)this["PROFILEDESC"];
            }
            set
            {
                this["PROFILEDESC"] = value;
            }
        }
        public double? PROFILESORT
        {
            get
            {
                return (double?)this["PROFILESORT"];
            }
            set
            {
                this["PROFILESORT"] = value;
            }
        }
        public double? PROFILELEVEL
        {
            get
            {
                return (double?)this["PROFILELEVEL"];
            }
            set
            {
                this["PROFILELEVEL"] = value;
            }
        }
        public string NOTE1
        {
            get
            {
                return (string)this["NOTE1"];
            }
            set
            {
                this["NOTE1"] = value;
            }
        }
        public string NOTE2
        {
            get
            {
                return (string)this["NOTE2"];
            }
            set
            {
                this["NOTE2"] = value;
            }
        }
        public string NOTE3
        {
            get
            {
                return (string)this["NOTE3"];
            }
            set
            {
                this["NOTE3"] = value;
            }
        }
        public string NOTE4
        {
            get
            {
                return (string)this["NOTE4"];
            }
            set
            {
                this["NOTE4"] = value;
            }
        }
        public string NOTE5
        {
            get
            {
                return (string)this["NOTE5"];
            }
            set
            {
                this["NOTE5"] = value;
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
    }
    public class r_profile
    {
        public string ID { get; set; }
        public string PROFILENAME { get; set; }
        public string PROFILECATEGORY { get; set; }
        public string PROFILETYPE { get; set; }
        public string PROFILEVALUE { get; set; }
        public string PROFILEDESC { get; set; }
        public double? PROFILESORT { get; set; }
        public double? PROFILELEVEL { get; set; }
        public string NOTE1 { get; set; }
        public string NOTE2 { get; set; }
        public string NOTE3 { get; set; }
        public string NOTE4 { get; set; }
        public string NOTE5 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}