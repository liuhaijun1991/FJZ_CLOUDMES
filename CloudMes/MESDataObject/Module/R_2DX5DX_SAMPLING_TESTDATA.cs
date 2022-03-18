using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_2DX5DX_SAMPLING_TESTDATA : DataObjectTable
    {
        public T_R_2DX5DX_SAMPLING_TESTDATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DX5DX_SAMPLING_TESTDATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DX5DX_SAMPLING_TESTDATA);
            TableName = "R_2DX5DX_SAMPLING_TESTDATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_2DX5DX_SAMPLING_TESTDATA : DataObjectBase
    {
        public Row_R_2DX5DX_SAMPLING_TESTDATA(DataObjectInfo info) : base(info)
        {

        }
        public R_2DX5DX_SAMPLING_TESTDATA GetDataObject()
        {
            R_2DX5DX_SAMPLING_TESTDATA DataObject = new R_2DX5DX_SAMPLING_TESTDATA();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.LINE_NAME = this.LINE_NAME;
            DataObject.STATION = this.STATION;
            DataObject.RESULT = this.RESULT;
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.LOCATION = this.LOCATION;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.TEST_EMP = this.TEST_EMP;
            DataObject.TEST_TIME = this.TEST_TIME;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string LINE_NAME
        {
            get
            {
                return (string)this["LINE_NAME"];
            }
            set
            {
                this["LINE_NAME"] = value;
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
        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
        }
        public string ERROR_CODE
        {
            get
            {
                return (string)this["ERROR_CODE"];
            }
            set
            {
                this["ERROR_CODE"] = value;
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
        public string SAMPLING_TYPE
        {
            get
            {
                return (string)this["SAMPLING_TYPE"];
            }
            set
            {
                this["SAMPLING_TYPE"] = value;
            }
        }
        public string TEST_EMP
        {
            get
            {
                return (string)this["TEST_EMP"];
            }
            set
            {
                this["TEST_EMP"] = value;
            }
        }
        public DateTime? TEST_TIME
        {
            get
            {
                return (DateTime?)this["TEST_TIME"];
            }
            set
            {
                this["TEST_TIME"] = value;
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
    public class R_2DX5DX_SAMPLING_TESTDATA
    {
        public string ID;
        public string SN;
        public string WO;
        public string LINE_NAME;
        public string STATION;
        public string RESULT;
        public string ERROR_CODE;
        public string LOCATION;
        public string SAMPLING_TYPE;
        public string TEST_EMP;
        public DateTime? TEST_TIME;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}