using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using System.Reflection;
using MESPubLab.MESStation.MESReturnView;

namespace MESStation
{
    public class ApiHelper:MesAPIBase
    { 
        protected APIInfo FGetApiClassList = new APIInfo()
        {
            FunctionName = "GetApiClassList",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()  
        };
        protected APIInfo FGetApiFunctionsList = new APIInfo()
        {
            FunctionName = "GetApiFunctionsList",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName="CLASSNAME" } },
            Permissions = new List<MESPermission>()
        };
        public ApiHelper()
        {
            _MastLogin = false;
            this.Apis.Add(FGetApiClassList.FunctionName,FGetApiClassList);
            this.Apis.Add(FGetApiFunctionsList.FunctionName, FGetApiFunctionsList);

        }

        public void GetApiClassList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            //MESPubLab.MESStation.MESPubLab.MESStation.MESReturnView.Public.GetApiClassListReturncs
            MESPubLab.MESStation.MESReturnView.Public.GetApiClassListReturncs ret = new MESPubLab.MESStation.MESReturnView.Public.GetApiClassListReturncs();
            var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            List<string> dllfiles = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].ToUpper().EndsWith(".DLL"))
                {
                    dllfiles.Add(files[i].ToUpper());
                }
            }


            Assembly assenbly = Assembly.Load("MESStation");
            Type tagType = typeof(MesAPIBase);
            
            for (int j = 0; j < dllfiles.Count; j++)
            {
                try
                {
                    assenbly = Assembly.LoadFile(dllfiles[j]);
                    Type[] t = assenbly.GetTypes();
                    for (int i = 0; i < t.Length; i++)
                    {
                        TypeInfo ti = t[i].GetTypeInfo();
                        Type baseType = ti.BaseType;
                        if (baseType == tagType)
                        {
                            ret.ClassName.Add(ti.FullName);
                        }
                    }
                }
                catch
                { }
            }
            StationReturn.Data = ret;
            StationReturn.Status = "Pass";
            StationReturn.Message =  "Success!";
        }

        public void GetApiFunctionsList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["CLASSNAME"].ToString();
            var sp = ClassName.Split(new char[] { '.' });

            Assembly assemby = Assembly.Load(sp[0]);
            Type t = assemby.GetType(ClassName);
            object obj = assemby.CreateInstance(ClassName);
            MesAPIBase API = (MesAPIBase)obj;
            MESPubLab.MESStation.MESReturnView.Public.GetApiFunctionsListReturn ret = new MESPubLab.MESStation.MESReturnView.Public.GetApiFunctionsListReturn();
            ret.APIS = API.Apis;
            StationReturn.Data = ret;
            StationReturn.Status = "Pass";

        }
    }
}
