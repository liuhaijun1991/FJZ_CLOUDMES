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
    public class T_R_2DX5DX_SAMPLING_SN : DataObjectTable
    {
        public T_R_2DX5DX_SAMPLING_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DX5DX_SAMPLING_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DX5DX_SAMPLING_SN);
            TableName = "R_2DX5DX_SAMPLING_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public void CheckSampling(string StrSN, string StrTest_flag, string StrSampling_type, OleExec DB)
        {
            string StrSql = "";
            DataTable Dt = new DataTable();
            StrSql = $@"
        SELECT * from r_2dx5dx_sampling_sn b
       WHERE b.SN = '{StrSN}'  
         AND b.test_flag <> '{StrTest_flag}'       
         AND b.sampling_type = '{StrSampling_type}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                if (StrSampling_type == "NORMAL_STATION")
                {
                    string errMsg = $@"SN在{Dt.Rows[0]["station"].ToString()}工站被制程巡檢自動抽測系統抽中，請先將產品送IPQC檢驗！";
                    throw new MESReturnMessage(errMsg);
                }
                else
                {
                    string errMsg = $@"SN在{Dt.Rows[0]["station"].ToString()}工站被抽中，請先將產品進行[{StrSampling_type}]測試或檢驗！";
                    throw new MESReturnMessage(errMsg);
                }
            }

        }


        public class Row_R_2DX5DX_SAMPLING_SN : DataObjectBase
        {
            public Row_R_2DX5DX_SAMPLING_SN(DataObjectInfo info) : base(info)
            {

            }
            public R_2DX5DX_SAMPLING_SN GetDataObject()
            {
                R_2DX5DX_SAMPLING_SN DataObject = new R_2DX5DX_SAMPLING_SN();
                DataObject.ID = this.ID;
                DataObject.WO = this.WO;
                DataObject.LINE_NAME = this.LINE_NAME;
                DataObject.STATION = this.STATION;
                DataObject.SN = this.SN;
                DataObject.SAMPLING_TIME = this.SAMPLING_TIME;
                DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
                DataObject.DATA2 = this.DATA2;
                DataObject.DATA3 = this.DATA3;
                DataObject.TEST_FLAG = this.TEST_FLAG;
                DataObject.TEST_TIME = this.TEST_TIME;
                DataObject.STATE = this.STATE;
                DataObject.TRANSPCB_TIME = this.TRANSPCB_TIME;
                DataObject.TRANSPCB_EMP = this.TRANSPCB_EMP;
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
            public DateTime? SAMPLING_TIME
            {
                get
                {
                    return (DateTime?)this["SAMPLING_TIME"];
                }
                set
                {
                    this["SAMPLING_TIME"] = value;
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
            public string DATA3
            {
                get
                {
                    return (string)this["DATA3"];
                }
                set
                {
                    this["DATA3"] = value;
                }
            }
            public string TEST_FLAG
            {
                get
                {
                    return (string)this["TEST_FLAG"];
                }
                set
                {
                    this["TEST_FLAG"] = value;
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
            public string STATE
            {
                get
                {
                    return (string)this["STATE"];
                }
                set
                {
                    this["STATE"] = value;
                }
            }
            public DateTime? TRANSPCB_TIME
            {
                get
                {
                    return (DateTime?)this["TRANSPCB_TIME"];
                }
                set
                {
                    this["TRANSPCB_TIME"] = value;
                }
            }
            public string TRANSPCB_EMP
            {
                get
                {
                    return (string)this["TRANSPCB_EMP"];
                }
                set
                {
                    this["TRANSPCB_EMP"] = value;
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
        public class R_2DX5DX_SAMPLING_SN
        {
            public string ID { get; set; }
            public string WO { get; set; }
            public string LINE_NAME { get; set; }
            public string STATION { get; set; }
            public string SN { get; set; }
            public DateTime? SAMPLING_TIME { get; set; }
            public string SAMPLING_TYPE { get; set; }
            public string DATA2 { get; set; }
            public string DATA3 { get; set; }
            public string TEST_FLAG { get; set; }
            public DateTime? TEST_TIME { get; set; }
            public string STATE { get; set; }
            public DateTime? TRANSPCB_TIME { get; set; }
            public string TRANSPCB_EMP { get; set; }
            public string EDIT_EMP { get; set; }
            public DateTime? EDIT_TIME { get; set; }
        }
    }
}