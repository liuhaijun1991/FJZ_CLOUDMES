using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{      
    public class T_C_FACTORY : DataObjectTable
    {
        public T_C_FACTORY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_FACTORY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_FACTORY);
            TableName = "C_FACTORY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }


        public C_FACTORY ConstructFactory(DataRow dr)  //convert row to object (as one row ) to send to client Json
        {
            C_FACTORY factory = new C_FACTORY();
            Row_C_FACTORY row = (Row_C_FACTORY)NewRow();
            row.loadData(dr);
            factory = row.GetDataObject();
            return factory;
        }

        public List<C_FACTORY> GetAllFactory(string FACTORY_CODE, OleExec DB)
        {
            List<C_FACTORY> FactoryList = new List<C_FACTORY>();
            string sql = "";
            DataTable dt = null;
            try
            {
                if (string.IsNullOrEmpty(FACTORY_CODE.Trim()))
                {     //get all Factory
                    sql = "SELECT * FROM  C_FACTORY ORDER BY FACTORY_CODE";
                }
                else
                {     // get info follow factorycode
                    sql = $@"SELECT * FROM  C_FACTORY WHERE FACTORY_CODE='{FACTORY_CODE.ToUpper()}' ORDER BY FACTORY_CODE";
                }
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        FactoryList.Add(ConstructFactory(dr));
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }   
            return FactoryList;
        }
        public int Delete(string ID, OleExec DB)
        {
            try
            {
                if (!string.IsNullOrEmpty(ID.Trim()))
                {
                    string sql = $@"DELETE FROM C_FACTORY WHERE ID='{ID.Trim()}'";
                    int result = DB.ExecuteNonQuery(sql, CommandType.Text);
                    if (result > 0)
                    {
                        return 0;  //delete success
                    }
                    else
                        return 1;  // delete fail
                }
                else return 2; //delete fail id is empty

            }
            catch (Exception e)
            {
                return 3;  //exception
                throw e;

            }

        }
        public int Insert(string FACTORY_CODE, string FACTORY_NAME, string FACTORY_ADDRESS, string DESCRIPTION, string EDIT_EMP, string BU, DB_TYPE_ENUM dbt, OleExec DB)
        {
            OleDbParameter[] param = null;
            string sql;
            int result;
            string ID;
            T_C_FACTORY factory = null;
            DataTable table;
            try
            {
                sql = $@"SELECT * FROM C_FACTORY WHERE UPPER(FACTORY_CODE)='{FACTORY_CODE}' AND UPPER(FACTORY_NAME)='{FACTORY_NAME}' AND UPPER(FACTORY_ADDRESS)='{FACTORY_ADDRESS}'";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count > 0)
                {   // factorycode+factoryname+factoryaddress existed fail primary key
                    return 0;
                }
                factory = new T_C_FACTORY(DB, dbt);
                ID = factory.GetNewID(BU, DB, dbt);
                param = new OleDbParameter[]
                                             {
                                             new OleDbParameter("ID",ID),
                                             new OleDbParameter("FACTORY_CODE",FACTORY_CODE.ToUpper()),
                                             new OleDbParameter("FACTORY_NAME",FACTORY_NAME.ToUpper()),
                                             new OleDbParameter("FACTORY_ADDRESS",FACTORY_ADDRESS.ToUpper()),
                                             new OleDbParameter("DESCRIPTION",DESCRIPTION),
                                             new OleDbParameter("EDIT_EMP",EDIT_EMP)
                                              };
                sql = "INSERT INTO C_FACTORY VALUES(:ID,:FACTORY_CODE,:FACTORY_NAME,:FACTORY_ADDRESS,:DESCRIPTION, SYSDATE,:EDIT_EMP)";
                result = DB.ExecuteNonQuery(sql, CommandType.Text, param);
                if (result > 0)
                    return 1;   //Insert success
                else
                    return 2;   //Insert fail
            }


            catch (Exception e)
            {
                return 3; //exception
                throw e;

            }


        }
        public int Update(string ID, string FACTORY_CODE, string FACTORY_NAME, string FACTORY_ADDRESS, string DESCRIPTION, string EDIT_EMP, OleExec DB)
        {
            OleDbParameter[] param = null;
            int result;
            string sql;
            DataTable table;
            try
            {

                sql = $@"SELECT * FROM C_FACTORY WHERE ID='{ID}'";
                table = DB.ExecSelect(sql).Tables[0];  // check id exist ?
                if (table.Rows.Count <= 0)
                {
                    return 0; // id not exist
                }
                sql = $@"SELECT * FROM C_FACTORY WHERE UPPER(FACTORY_CODE)='{FACTORY_CODE}' AND UPPER(FACTORY_NAME)='{FACTORY_NAME}' AND UPPER(FACTORY_ADDRESS)='{FACTORY_ADDRESS}'";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count > 0)
                {
                    return 1; // factorycode+factoryname+factoryaddress existed fail primary key
                }
                sql = $@"UPDATE C_FACTORY SET FACTORY_CODE=:FACTORY_CODE,FACTORY_NAME=:FACTORY_NAME,FACTORY_ADDRESS=:FACTORY_ADDRESS,DESCRIPTION=:DESCRIPTION,EDIT_TIME=SYSDATE,EDIT_EMP=:EDIT_EMP WHERE ID='{ID}'";
                param = new OleDbParameter[]
                                                {
                                             new OleDbParameter("FACTORY_CODE",FACTORY_CODE.ToUpper()),
                                             new OleDbParameter("FACTORY_NAME",FACTORY_NAME.ToUpper()),
                                             new OleDbParameter("FACTORY_ADDRESS",FACTORY_ADDRESS.ToUpper()),
                                             new OleDbParameter("DESCRIPTION",DESCRIPTION),
                                             new OleDbParameter("EDIT_EMP",EDIT_EMP)
                                                 };

                result = DB.ExecuteNonQuery(sql, CommandType.Text, param);
                if (result > 0)
                    return 2;   //Update success
                else
                    return 3;   //Update fail
            }
            catch (Exception e)
            {
                return 4; //exception
                throw e;
            }

        }


    }
    public class Row_C_FACTORY : DataObjectBase
    {
        public Row_C_FACTORY(DataObjectInfo info) : base(info)
        {

        }
        public C_FACTORY GetDataObject()
        {
            C_FACTORY DataObject = new C_FACTORY();
            DataObject.ID = this.ID;
            DataObject.FACTORY_CODE = this.FACTORY_CODE;
            DataObject.FACTORY_NAME = this.FACTORY_NAME;
            DataObject.FACTORY_ADDRESS = this.FACTORY_ADDRESS;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string FACTORY_CODE
        {
            get
            {
                return (string)this["FACTORY_CODE"];
            }
            set
            {
                this["FACTORY_CODE"] = value;
            }
        }
        public string FACTORY_NAME
        {
            get
            {
                return (string)this["FACTORY_NAME"];
            }
            set
            {
                this["FACTORY_NAME"] = value;
            }
        }
        public string FACTORY_ADDRESS
        {
            get
            {
                return (string)this["FACTORY_ADDRESS"];
            }
            set
            {
                this["FACTORY_ADDRESS"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
    }
    public class C_FACTORY
    {
        public string ID{get;set;}
        public string FACTORY_CODE{get;set;}
        public string FACTORY_NAME{get;set;}
        public string FACTORY_ADDRESS{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}