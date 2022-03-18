using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_C_CUSTOMER : DataObjectTable
    {
        public T_C_CUSTOMER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CUSTOMER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CUSTOMER);
            TableName = "C_CUSTOMER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable GetCustomer(Dictionary<string, string> parameters, OleExec DB)
        {
            List<IConditionalModel> CMs = new List<IConditionalModel>();
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> paras in parameters)
                {
                    if (paras.Key.Equals("CUSTOMER_NAME"))
                    {
                        CMs.Add(new ConditionalModel { FieldName = paras.Key, FieldValue = paras.Value, ConditionalType = ConditionalType.Like });
                    }
                    else
                    {
                        CMs.Add(new ConditionalModel { FieldName = paras.Key, FieldValue = paras.Value, ConditionalType = ConditionalType.Equal });
                    }
                }
            }
            return DB.ORM.Queryable<C_CUSTOMER>().Where(CMs).ToDataTable();


            //string sql = $@"select * from c_customer  where 1=1 ";
            //string tempSql = "";
            //if (parameters != null)
            //{
            //    foreach (KeyValuePair<string, string> paras in parameters)
            //    {
            //        if (paras.Value != "")
            //        {
            //            tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
            //        }
            //    }
            //}
            //sql = sql + tempSql;
            //return DB.ExecSelect(sql).Tables[0];
        }
        public string GetTypeSkuno(string skuno, OleExec oleDB)
        {
            DataTable dt = new DataTable();
            string sql = $@"SELECT * FROM SFCBASE.C_CUSTOMER WHERE ID IN(
                        SELECT CUSTOMER_ID FROM SFCBASE.C_SERIES WHERE ID IN (
                        SELECT C_SERIES_ID FROM SFCBASE.C_SKU WHERE SKUNO ='{skuno}')) ";
            dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string CUSTOMER_NAME = dt.Rows[0]["CUSTOMER_NAME"].ToString();
                return CUSTOMER_NAME;
            }
            else
            {
                return "";
            }
          
        }
        public List<C_CUSTOMER> GetCustomerList(Dictionary<string, string> parameters, OleExec oleDB)
        {
            List<IConditionalModel> CMs = new List<IConditionalModel>();
            foreach (KeyValuePair<string, string> paras in parameters)
            {
                if (paras.Value != "")
                {
                    if (paras.Key.Equals("CUSTOMER_NAME"))
                    {
                        CMs.Add(new ConditionalModel { FieldName=paras.Key,FieldValue=paras.Value, ConditionalType=ConditionalType.Like });
                    }
                    else
                    {
                        CMs.Add(new ConditionalModel { FieldName = paras.Key, FieldValue = paras.Value, ConditionalType = ConditionalType.Equal });
                    }
                }
            }
            return oleDB.ORM.Queryable<C_CUSTOMER>().Where(CMs).ToList();

            //string sql = $@"select * from c_customer  where 1=1 ";
            //string tempSql = "";
            //DataTable dtCustomer = new DataTable();
            //foreach (KeyValuePair<string, string> paras in parameters)
            //{
            //    if (paras.Value != "")
            //    {
            //        if (paras.Key.Equals("CUSTOMER_NAME"))
            //        {
            //            tempSql = tempSql + $@" and {paras.Key} like '%{paras.Value}%' ";
            //        }
            //        else
            //        {
            //            tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
            //        }
            //    }
            //}
            //sql = sql + tempSql;
            //dtCustomer = oleDB.ExecSelect(sql).Tables[0];
            //List<C_CUSTOMER> costomerList = new List<C_CUSTOMER>();
            //Row_C_CUSTOMER customerRow;
            //foreach (DataRow row in dtCustomer.Rows)
            //{
            //    customerRow = (Row_C_CUSTOMER)this.NewRow();
            //    customerRow.loadData(row);
            //    costomerList.Add(customerRow.GetDataObject());
            //}
            //return costomerList;
        }
        public List<Dictionary<string, string>> GetCustomerDetail(Dictionary<string, string> parameters, OleExec oleDB)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> detail;

            List<C_CUSTOMER> Cs = GetCustomerList(parameters, oleDB);
            foreach (C_CUSTOMER C in Cs)
            {
                detail = new Dictionary<string, string>();
                detail.Add("BU", C.BU);
                detail.Add("CUSTOMER_NAME", C.CUSTOMER_NAME);
                detail.Add("DESCRIPTION", C.DESCRIPTION);
                List<C_CUSTOMER_EX> CEs = oleDB.ORM.Queryable<C_CUSTOMER_EX>().Where(t => t.ID == C.ID).ToList();
                foreach (C_CUSTOMER_EX CE in CEs)
                {
                    detail.Add(CE.NAME, CE.VALUE);
                }
                list.Add(detail);
            }

            //string sql = $@"select * from c_customer  where 1=1 ";
            //string tempSql = "";
            //string sqlExpand = "";
            //List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            //Dictionary<string, string> detail;
            //DataTable temp;
            //if (parameters != null)
            //{
            //    foreach (KeyValuePair<string, string> paras in parameters)
            //    {
            //        if (paras.Value != "")
            //        {
            //            tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
            //        }
            //    }
            //}
            //sql = sql + tempSql;
            //DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            //foreach (DataRow row in dt.Rows)
            //{
            //    detail = new Dictionary<string, string>();
            //    //detail.Add("ID", row["ID"].ToString());
            //    detail.Add("BU", row["BU"].ToString());
            //    detail.Add("CUSTOMER_NAME", row["CUSTOMER_NAME"].ToString());
            //    detail.Add("DESCRIPTION", row["DESCRIPTION"].ToString());
            //    sqlExpand = $@"select * from c_customer_ex where id='{row["ID"].ToString()}'";
            //    temp = oleDB.ExecSelect(sqlExpand).Tables[0];
            //    foreach (DataRow rowExpand in temp.Rows)
            //    {
            //        detail.Add(rowExpand["NAME"].ToString(), rowExpand["VALUE"].ToString());
            //    }
            //    list.Add(detail);
            //}
            return list;
        }
        public bool CustomerIsExist(OleExec oleDB, string strbu, string customerName)
        {
            return oleDB.ORM.Queryable<C_CUSTOMER>().Any(t => t.BU==strbu && t.CUSTOMER_NAME==customerName);
        }

        public bool CustomerIsExist(OleExec oleDB, string strID)
        {
            return oleDB.ORM.Queryable<C_CUSTOMER>().Any(t => t.ID == strID);
        }
        public string GetCustomerID(OleExec oleDB, string strbu, string customerName)
        {
            List<C_CUSTOMER> Cs = oleDB.ORM.Queryable<C_CUSTOMER>().Where(t => t.BU == strbu && t.CUSTOMER_NAME == customerName).ToList();
            if (Cs.Count > 0)
            {
                return Cs.First().ID;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetCustomerName(OleExec oleDB, string strID)
        {
            List<C_CUSTOMER> Cs = oleDB.ORM.Queryable<C_CUSTOMER>().Where(t =>t.ID== strID).ToList();
            if (Cs.Count > 0)
            {
                return Cs.First().CUSTOMER_NAME;
            }
            else
            {
                return string.Empty;
            }
        }
    }
    public class Row_C_CUSTOMER : DataObjectBase
    {
        public Row_C_CUSTOMER(DataObjectInfo info) : base(info)
        {

        }
        public C_CUSTOMER GetDataObject()
        {
            C_CUSTOMER DataObject = new C_CUSTOMER();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.CUSTOMER_NAME = this.CUSTOMER_NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
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
    }
    public class C_CUSTOMER
    {
        public string ID{get;set;}
        public string BU{get;set;}
        public string CUSTOMER_NAME{get;set;}
        public string DESCRIPTION{get;set;}

        public override string ToString()
        {
            return CUSTOMER_NAME == null ? "" : CUSTOMER_NAME;
        }
    }

    public class C_CUSTOMER_EX
    {
        public string ID { get; set; }
        public string SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }

    }
}