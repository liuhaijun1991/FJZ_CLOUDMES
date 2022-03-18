using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;

namespace MESStation.Test
{
   public class CreateClass : MesAPIBase
    {
        protected APIInfo FCreateClassAPI = new APIInfo()
        {
            FunctionName = "CreateClassAPI",
            Description = "根據表名創建對應的類",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Table_Name", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CreateClass()
        {
            this.Apis.Add(FCreateClassAPI.FunctionName, FCreateClassAPI);
        }
        /// <summary>
        /// 根據傳過來的表名生成相應的類
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CreateClassAPI(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            //
            string ClassACode = "";
            string ClassBCode = "";
            string ClassCCode = "";
            string Table_Name = Data["Table_Name"].ToString();
            string UsingRef0 = "using System;\n";
            string UsingRef1 = "using System.Collections.Generic;\n";
            string UsingRef2 = "using System.Linq;\n";
            string UsingRef3 = "using System.Text;\n";
            string UsingRef4 = "using System.Data;\n";
            string UsingRef5 = "using System.Threading.Tasks;\n";
            string UsingRef6 = "using MESDBHelper; \n \n ";

            string UsingRef = UsingRef0 + UsingRef1 + UsingRef2 + UsingRef3 + UsingRef4 + UsingRef5 + UsingRef6;
            string NameSpace = "namespace MESDataObject.Module\n{\n";
            string DeclarationClassA = " public class T_" + Table_Name + " : DataObjectTable \n{\n ";
            string AclassMakeFunctionA = "public T_" + Table_Name + "(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)\n{\n\n}\n";
            string AclassMakeFunctionB = "public T_" + Table_Name + "(OleExec DB, DB_TYPE_ENUM DBType)\n{\n" +
                                                 " RowType = typeof(Row_" + Table_Name + ");\n" +
                                                 " TableName = \"" + Table_Name + "\".ToUpper();\n" +
                                                 " DataInfo = GetDataObjectInfo(TableName, DB, DBType);\n}\n}\n";
            ClassACode = DeclarationClassA + AclassMakeFunctionA + AclassMakeFunctionB;
            string DeclarationClassB = " public class Row_" + Table_Name + " : DataObjectBase\n{";
            string BclassMakeFunctionB = "public Row_" + Table_Name + "(DataObjectInfo info) : base(info)\n{\n\n }";
            String DeclarationClassC = "public class  " + Table_Name +"\n{\n";

            try
            {
                DataObjectInfo Table_Info = new DataObjectInfo();
                Table_Info = GetTableInfo(Table_Name);
                string StrField = CreateField(Table_Name, Table_Info);

                String BclassFunctionA = CreateObjectFunction(Table_Name,Table_Info);

                ClassBCode = DeclarationClassB + BclassMakeFunctionB+ BclassFunctionA + StrField+ "}\n";

                string DField = DeclarationField(Table_Name, Table_Info);
                ClassCCode = DeclarationClassC+DField+"}";

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "";
                StationReturn.Data = UsingRef + NameSpace + ClassACode + ClassBCode+ClassCCode + "}";
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                //StationReturn.Message = "類生成發生錯誤" + e.Message.ToString();
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145907", new string[] { e.Message.ToString() });
                StationReturn.Data = "";
            }
        }
        /// <summary>
        /// 根據表名生成C#字段代碼
        /// </summary>
        /// <param name="Table_Name">表名</param>
        /// <returns>C#字段</returns>
        private string CreateField(string Table_Name, DataObjectInfo Table_Info)
        {
            string StrReturn = "";
            int Count = Table_Info.BaseColsInfo.Count;
            string FieldName = "";
            string TempType = "";
            string FieldType = "";
            for (int i = 0; i < Count; i++)
            {
                FieldName = ((DataObjectColInfo)Table_Info.BaseColsInfo[i]).name.ToString();
                TempType = ((DataObjectColInfo)Table_Info.BaseColsInfo[i]).DataType.ToString();
                FieldType = GetFieldType(((DataObjectColInfo)Table_Info.BaseColsInfo[i]).DataType);

                StrReturn = StrReturn + "public " + FieldType + " " + FieldName + "\n{\n" +
                            " get\n{\nreturn (" + FieldType + ")this[\"" + FieldName + "\"]; \n}set  \n{\n    this[\"" + FieldName + "\"] = value; }\n}" + "\n";
            }
            return StrReturn;

        }
        /// <summary>
        /// 字義類所有字段
        /// </summary>
        /// <param name="Table_Name"></param>
        /// <returns></returns>
        private string DeclarationField(string Table_Name, DataObjectInfo Table_Info)
        {
            string StrReturn = "";
            int Count = Table_Info.BaseColsInfo.Count;
            string FieldName = "";
            string TempType = "";
            string FieldType = "";
            for (int i = 0; i < Count; i++)
            {
                FieldName = ((DataObjectColInfo)Table_Info.BaseColsInfo[i]).name.ToString();
                TempType = ((DataObjectColInfo)Table_Info.BaseColsInfo[i]).DataType.ToString();
                FieldType = GetFieldType(((DataObjectColInfo)Table_Info.BaseColsInfo[i]).DataType);
                StrReturn = StrReturn + "public " + FieldType + " " + FieldName + "{get;set;}\n" ;
            }
            return StrReturn;

        }

        /// <summary>
        /// 根據C#類型獲取實際類型
        /// </summary>
        /// <param name="Datatype">類型</param>
        /// <returns></returns>
        private string GetFieldType(Type Datatype)
        {
            string typename = "";

            if (Datatype == typeof(string))
            {
                typename = "string";
            }
            else if (Datatype == typeof(DateTime))
            {
                typename = "DateTime?";
            }
            else if (Datatype == typeof(long))
            {
                typename = "long?";
            }
            else if (Datatype == typeof(double))
            {

                typename = "double?";
            }
            else if (Datatype == typeof(object))
            {
                typename = "string";
            }
            else
            {
                //throw new Exception("數據類型:" + Datatype.ToString() + "存在問題，不能轉換！");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145914", new string[] { Datatype.ToString() }));
            }


            return typename;
        }
        /// <summary>
        /// 生成GetDataObject函數
        /// </summary>
        /// <param name="Table_Name"></param>
        /// <returns></returns>
        private String CreateObjectFunction(string Table_Name,DataObjectInfo Table_Info)
        {
            string StrReturn = "";
            int Count = Table_Info.BaseColsInfo.Count;
            string FieldName = "";
            string TempType = "";
            string FieldType = "";
            string FunctionA = "public "+Table_Name+" GetDataObject()\n{\n " + Table_Name+" DataObject = new "+Table_Name+"();\n";
            for (int i = 0; i < Count; i++)
            {
                FieldName = ((DataObjectColInfo)Table_Info.BaseColsInfo[i]).name.ToString();
                TempType = ((DataObjectColInfo)Table_Info.BaseColsInfo[i]).DataType.ToString();
                FieldType = GetFieldType(((DataObjectColInfo)Table_Info.BaseColsInfo[i]).DataType);
                StrReturn = StrReturn+ "DataObject." +FieldName+"="+"this."+FieldName+";\n";
            }
            return FunctionA +StrReturn+ "return DataObject;\n}";
        }
        /// <summary>
        /// 獲取表類型
        /// </summary>
        /// <param name="Table_Name"></param>
        /// <returns></returns>
        private DataObjectInfo GetTableInfo(string Table_Name)
        {
            try
            {
                OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
                DataObjectInfo Table_Info = new DataObjectInfo();
                Table_Info = DataObjectTable.GetDataObjectInfo(Table_Name.ToUpper(), sfcdb, DB_TYPE_ENUM.Oracle);
                this.DBPools["SFCDB"].Return(sfcdb);
                return Table_Info;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message); 

            }
        }

    }
}
