using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESReport;
using System.Reflection;
using MESDBHelper;

namespace MESStation.Report
{
    public class CallReport : MESPubLab.MESStation.MesAPIBase
    {
        static Dictionary<string, ReportSession> Session = new Dictionary<string, ReportSession>();
        protected APIInfo _GetReport = new APIInfo()
        {
            FunctionName = "GetReport",
            Description = "獲取報表對象",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ClassName", InputType = "string", DefaultValue = "MESReport.Test.TEST1" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _RunReport = new APIInfo()
        {
            FunctionName = "RunReport",
            Description = "計算報表",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ClassName", InputType = "string", DefaultValue = "MESReport.Test.TEST1" },
                new APIInputInfo() {InputName = "Report", InputType = "string", DefaultValue = "MESReport.Test.TEST1" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo _DownFile = new APIInfo()
        {
            FunctionName = "DownFile",
            Description = "Download Report File From Server",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ClassName", InputType = "string", DefaultValue = "MESReport.Test.TEST1" },
                new APIInputInfo() {InputName = "Report", InputType = "string", DefaultValue = "MESReport.Test.TEST1" }
            },
            Permissions = new List<MESPermission>() { }
        };
        
        protected APIInfo _InputChange = new APIInfo()
        {
            FunctionName = "InputChange",
            Description = "InputChange事件",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ClassName", InputType = "string", DefaultValue = "MESReport.Test.TEST1" },
                new APIInputInfo() {InputName = "Report", InputType = "string", DefaultValue = "MESReport.Test.TEST1" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CallReport()
        {
            Apis.Add(_GetReport.FunctionName, _GetReport);
            Apis.Add(_RunReport.FunctionName, _RunReport);
            Apis.Add(_InputChange.FunctionName, _InputChange);
            Apis.Add(_DownFile.FunctionName, _DownFile);
        }

        public void GetReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["ClassName"].ToString();
            if (ClassName == "")
            {
                throw new Exception("ClassName Not Set!");
            }
            Assembly assembly = Assembly.Load("MESReport");
            Type ReportType = assembly.GetType(ClassName);
            if(ReportType == null)
            {
                throw new Exception($@"Can Not Create {ClassName}!");
            }
            ReportBase Report = (MESReport.ReportBase)assembly.CreateInstance(ClassName);
            Report.DBPools = this.DBPools;
            Report.Init();
            StationReturn.Data = Report;
            StationReturn.Message = "";
            StationReturn.Status = StationReturnStatusValue.Pass;
        }

        public void InputChange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["ClassName"].ToString();
            if (ClassName == "")
            {
                throw new Exception("ClassName Not Set!");
            }
            Assembly assembly = Assembly.Load("MESReport");
            Type ReportType = assembly.GetType(ClassName);
            if (ReportType == null)
            {
                throw new Exception($@"Can Not Create {ClassName}!");
            }
            ReportBase Report = (MESReport.ReportBase)assembly.CreateInstance(ClassName);
            Report.DBPools = this.DBPools;
            Report.isCallBack = true;
            Report.Init();
            //Report.Inputs.Clear();

            List<ReportInput> notMatch = new List<ReportInput>();
            for (int i = 0; i < Report.Inputs.Count; i++)
            {
                ReportInput input = Report.Inputs[i];
                bool match = false;
                int mIndex = 0;
                for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
                {
                    if (Data["Report"]["Inputs"][j]["Name"].ToString() == input.Name)
                    {
                        match = true;
                        mIndex = j;
                        break;
                    }
                }
                if (!match)
                {
                    notMatch.Add(input);
                    continue;
                }
                try
                {

                    if (input.InputType == "DateTime")
                    {
                        //input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<DateTime>();
                        input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToString().Replace("%20", " ");
                        if (input.Value != null)
                            input.Value = Convert.ToDateTime(input.Value);
                        input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<DateTime>>();
                    }
                    else
                    {
                        input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<string>();
                        input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<string>>();
                    }
                    input.Enable = Data["Report"]["Inputs"][mIndex]["Enable"]?.ToObject<bool>();
                }
                catch
                { }
            }
            for (int i = 0; i < notMatch.Count; i++)
            {
                Report.Inputs.Remove(notMatch[i]);
            }
            List<ReportInput> AddInput = new List<ReportInput>();
            for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
            {
                var input = Data["Report"]["Inputs"][j];
                bool match = false;
                int mIndex = 0;
                for (int i = 0; i < Report.Inputs.Count; i++)
                {
                    if (input["Name"].ToString() == Report.Inputs[i].Name)
                    {
                        match = true;
                        mIndex = i;
                        break;
                    }
                }
                if (!match)
                {
                    ReportInput inputN = new ReportInput()
                    {
                        Name = input["Name"].ToString(),
                        Value = input["Value"].ToString(),
                        InputType = input["InputType"].ToString(),
                        Enable = input.Value<bool?>("Enable")
                    };
                    AddInput.Add(inputN);
                    try
                    {

                        if (input["InputType"].ToString() == "DateTime")
                        {
                            inputN.Value = input.Value<DateTime?>("Value");

                            inputN.ValueForUse = input["ValueForUse"]?.ToObject<List<DateTime>>();
                        }
                        else
                        {
                            inputN.Value = input["Value"]?.ToObject<string>();
                            inputN.ValueForUse = input["ValueForUse"]?.ToObject<List<string>>();
                        }
                        inputN.Enable = input["Enable"]?.ToObject<bool>();
                    }
                    catch
                    { }
                }
            }
            for (int i = 0; i < AddInput.Count; i++)
            {
                Report.Inputs.Add(AddInput[i]);
            }

            Report.InputChangeEvent();

            StationReturn.Data = Report;
            StationReturn.Message = "";
            StationReturn.Status = StationReturnStatusValue.Pass;
        }



        public void RunReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["ClassName"].ToString();
            if (ClassName == "")
            {
                throw new Exception("ClassName Not Set!");
            }
            Assembly assembly = Assembly.Load("MESReport");
            Type ReportType = assembly.GetType(ClassName);
            if (ReportType == null)
            {
                throw new Exception($@"Can Not Create {ClassName}!");
            }
            ReportBase Report = (MESReport.ReportBase)assembly.CreateInstance(ClassName);
            try
            {
                Report.PageSize = Data["PageSize"] == null ? 0 : Convert.ToInt32(Data["PageSize"].ToString());
                Report.PageNumber = Data["PageNumber"] == null ? 0 : Convert.ToInt32(Data["PageNumber"].ToString());
            }
            catch
            {
                Report.PageSize = 0;
                Report.PageNumber = 0;
            }
            Report.DBPools = this.DBPools;
            Report.LoginBU = BU;
            Report.Init();
            ////循環加載input
            //for (int i = 0; i < Report.Inputs.Count; i++)
            //{
            //    ReportInput input = Report.Inputs[i];
            //    bool match = false;
            //    int mIndex = 0;
            //    for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
            //    {
            //        if (Data["Report"]["Inputs"][j]["Name"].ToString() == input.Name)
            //        {
            //            match = true;
            //            mIndex = j;
            //            break;
            //        }
            //    }
            //    if (!match)
            //    {
            //        //input.Value = null;
            //        //input.ValueForUse = null;
            //        continue;
            //    }
            //    try
            //    {

            //        if (input.InputType == "DateTime")
            //        {
            //            //input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<DateTime>();
            //            input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToString().Replace("%20", " ");
            //            if(input.Value!=null)
            //                input.Value =Convert.ToDateTime(input.Value);
            //            input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<DateTime>>();
            //        }
            //        else
            //        {
            //            input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<string>();
            //            input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<string>>();
            //        }
            //        input.Enable = Data["Report"]["Inputs"][mIndex]["Enable"]?.ToObject<bool>();
            //    }
            //    catch
            //    { }
            //}

            List<ReportInput> notMatch = new List<ReportInput>();
            for (int i = 0; i < Report.Inputs.Count; i++)
            {
                ReportInput input = Report.Inputs[i];
                bool match = false;
                int mIndex = 0;
                for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
                {
                    if (Data["Report"]["Inputs"][j]["Name"].ToString() == input.Name)
                    {
                        match = true;
                        mIndex = j;
                        break;
                    }
                }
                if (!match)
                {
                    notMatch.Add(input);
                    continue;
                }
                try
                {

                    if (input.InputType == "DateTime")
                    {
                        //input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<DateTime>();
                        input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToString().Replace("%20", " ");
                        if (input.Value != null)
                            input.Value = Convert.ToDateTime(input.Value);
                        input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<DateTime>>();
                    }
                    else
                    {
                        input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<string>();
                        input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<string>>();
                    }
                    input.Enable = Data["Report"]["Inputs"][mIndex]["Enable"]?.ToObject<bool>();
                }
                catch
                { }
            }
            for (int i = 0; i < notMatch.Count; i++)
            {
                Report.Inputs.Remove(notMatch[i]);
            }
            List<ReportInput> AddInput = new List<ReportInput>();
            for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
            {
                var input = Data["Report"]["Inputs"][j];
                bool match = false;
                int mIndex = 0;
                for (int i = 0; i < Report.Inputs.Count; i++)
                {
                    if (input["Name"].ToString() == Report.Inputs[i].Name)
                    {
                        match = true;
                        mIndex = i;
                        break;
                    }
                }
                if (!match)
                {
                    ReportInput inputN = new ReportInput()
                    {
                        Name = input["Name"].ToString(),
                        Value = input["Value"].ToString(),
                        InputType = input["InputType"].ToString(),
                        Enable = input.Value<bool?>("Enable")
                    };
                    AddInput.Add(inputN);
                    try
                    {

                        if (input["InputType"].ToString() == "DateTime")
                        {
                            inputN.Value = input.Value<DateTime?>("Value");

                            inputN.ValueForUse = input["ValueForUse"]?.ToObject<List<DateTime>>();
                        }
                        else
                        {
                            inputN.Value = input["Value"]?.ToObject<string>();
                            inputN.ValueForUse = input["ValueForUse"]?.ToObject<List<string>>();
                        }
                        inputN.Enable = input["Enable"]?.ToObject<bool>();
                    }
                    catch
                    { }
                }
            }
            for (int i = 0; i < AddInput.Count; i++)
            {
                Report.Inputs.Add(AddInput[i]);
            }

            Report.Run();

            StationReturn.Data = Report;
            StationReturn.Message = "";
            StationReturn.Status = StationReturnStatusValue.Pass;

        }

        public void DownFile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            try
            {

                string ClassName = Data["ClassName"].ToString();
                if (ClassName == "")
                {
                    throw new Exception("ClassName Not Set!");
                }
                Assembly assembly = Assembly.Load("MESReport");
                Type ReportType = assembly.GetType(ClassName);
                if (ReportType == null)
                {
                    throw new Exception($@"Can Not Create {ClassName}!");
                }
                ReportBase Report = (MESReport.ReportBase)assembly.CreateInstance(ClassName);
                Report.DBPools = this.DBPools;
                Report.Init();

                List<ReportInput> notMatch = new List<ReportInput>();
                for (int i = 0; i < Report.Inputs.Count; i++)
                {
                    ReportInput input = Report.Inputs[i];
                    bool match = false;
                    int mIndex = 0;
                    for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
                    {
                        if (Data["Report"]["Inputs"][j]["Name"].ToString() == input.Name)
                        {
                            match = true;
                            mIndex = j;
                            break;
                        }
                    }
                    if (!match)
                    {
                        notMatch.Add(input);
                        continue;
                    }
                    try
                    {

                        if (input.InputType == "DateTime")
                        {
                            input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToString().Replace("%20", " ");
                            if (input.Value != null)
                                input.Value = Convert.ToDateTime(input.Value);
                            input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<DateTime>>();
                        }
                        else
                        {
                            input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<string>();
                            input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<string>>();
                        }
                        input.Enable = Data["Report"]["Inputs"][mIndex]["Enable"]?.ToObject<bool>();
                    }
                    catch
                    { }
                }
                for (int i = 0; i < notMatch.Count; i++)
                {
                    Report.Inputs.Remove(notMatch[i]);
                }
                List<ReportInput> AddInput = new List<ReportInput>();
                for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
                {
                    var input = Data["Report"]["Inputs"][j];
                    bool match = false;
                    int mIndex = 0;
                    for (int i = 0; i < Report.Inputs.Count; i++)
                    {
                        if (input["Name"].ToString() == Report.Inputs[i].Name)
                        {
                            match = true;
                            mIndex = i;
                            break;
                        }
                    }
                    if (!match)
                    {
                        ReportInput inputN = new ReportInput()
                        {
                            Name = input["Name"].ToString(),
                            Value = input["Value"].ToString(),
                            InputType = input["InputType"].ToString(),
                            Enable = input.Value<bool?>("Enable")
                        };
                        AddInput.Add(inputN);
                        try
                        {

                            if (input["InputType"].ToString() == "DateTime")
                            {
                                inputN.Value = input.Value<DateTime?>("Value");

                                inputN.ValueForUse = input["ValueForUse"]?.ToObject<List<DateTime>>();
                            }
                            else
                            {
                                inputN.Value = input["Value"]?.ToObject<string>();
                                inputN.ValueForUse = input["ValueForUse"]?.ToObject<List<string>>();
                            }
                            inputN.Enable = input["Enable"]?.ToObject<bool>();
                        }
                        catch
                        { }
                    }
                }
                for (int i = 0; i < AddInput.Count; i++)
                {
                    Report.Inputs.Add(AddInput[i]);
                }
                Report.DownFile();
                StationReturn.Data = Report;
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        class ReportSession
        {
            public string Token;
            public ReportBase Report;
            public DateTime LastEditTime;
        }

       
    }
}
