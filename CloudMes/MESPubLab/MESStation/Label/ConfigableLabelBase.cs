using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MESDBHelper;

namespace MESPubLab.MESStation.Label
{
    public class ConfigableLabelBase:LabelBase
    {
        
        public Dictionary<string, object> Keys = new Dictionary<string, object>();
        public List<GetDataFunctionConfig> Functions = new List<GetDataFunctionConfig>();
        public List<string> NotOutputKeys = new List<string>();
        public ConfigableLabelBase()
        {
            
        }
        public override void MakeLabel(OleExec DB)
        {
            var keys = Keys.Keys.ToList<string>();
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (keys.Contains(Inputs[i].Name))
                {
                    if (Inputs[i].Value.GetType() == typeof(string) || Inputs[i].Value.GetType() == typeof(string[]))
                    {
                        Keys[Inputs[i].Name] = Inputs[i].Value;
                    }
                    else
                    {
                        try
                        {
                            I_LabelValue V = (I_LabelValue)Inputs[i].Value;
                            Keys[Inputs[i].Name] = V.GetLabelValue();
                        }
                        catch
                        {
                            Keys[Inputs[i].Name] = Inputs[i].Value.ToString();
                        }
                    }
                    
                    
                }
                else
                {
                    Keys.Add(Inputs[i].Name, "");
                    if (Inputs[i].Value.GetType() == typeof(string) || Inputs[i].Value.GetType() == typeof(string[]))
                    {
                        Keys[Inputs[i].Name] = Inputs[i].Value;
                    }
                    else
                    {
                        try
                        {
                            I_LabelValue V = (I_LabelValue)Inputs[i].Value;
                            Keys[Inputs[i].Name] = V.GetLabelValue();
                        }
                        catch
                        {
                            Keys[Inputs[i].Name] = Inputs[i].Value.ToString();
                        }
                    }
                }
            }

            for (int i = 0; i < Functions.Count; i++)
            {
                try
                {
                    Functions[i].Run(DB, this);
                    if (Functions[i].OutPutKey == "NotPrint")
                    {
                        var value = this.Keys["NotPrint"];
                        if (value == "TRUE")
                        {
                            break;
                        }
                    }
                }
                catch (Exception ee)
                {
                    if (ee.InnerException != null)
                    {
                        throw new Exception($@"{ Functions[i].OutPutKey} : {ee.InnerException.Message}");
                    }
                    else
                    {
                        throw new Exception($@"{ Functions[i].OutPutKey} : {ee.Message}");
                    }                    
                }
            }

            keys = Keys.Keys.ToList<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (NotOutputKeys.Contains(keys[i]))
                {
                    continue;
                }
                var output = Outputs.Find(t => t.Name == keys[i]);
                if (output == null)
                {
                    output = new LabelOutput() { Name = keys[i], Value = Keys[keys[i]] };
                    Outputs.Add(output);
                }
                else
                {
                    output.Value = Keys[keys[i]];
                }
                if (output.Value == null)
                {
                    output.Value = "";
                    output.Type = LabelOutPutTypeEnum.String;
                }
                else if (output.Value.GetType() == typeof(string))
                {
                    output.Type = LabelOutPutTypeEnum.String;
                }
                else
                {
                    output.Type = LabelOutPutTypeEnum.StringArry;
                }
            }
        }
        public static List<string> GetLabelValueGroupNameList()
        {
            List<string> retList = new List<string>();
            Assembly[] Assenblys = new Assembly[] { Assembly.Load("MESStation"), Assembly.Load("MESPubLab") };
            Type tagType = typeof(LabelValueGroup);
            for (int i = 0; i < Assenblys.Count(); i++)
            {
                Type[] t = Assenblys[i].GetTypes();

                for (int j = 0; j < t.Length; j++)
                {
                    TypeInfo tj = t[j].GetTypeInfo();
                    Type baseType = tj.BaseType;
                    if (baseType == tagType)
                    {
                        object obj = Assenblys[i].CreateInstance(tj.FullName);
                        LabelValueGroup labVale = (LabelValueGroup)obj;
                        retList.Add(labVale.ConfigGroup); 
                    }
                }
            }
            return retList;
        }


        public static List<object> GetLabelValueGroup(string GroupName )
        {
            List<object> retList = new List<object>();
            Assembly[] Assenblys = new Assembly[] { Assembly.Load("MESStation"), Assembly.Load("MESPubLab") };
            Type tagType = typeof(LabelValueGroup);
            for (int i = 0; i < Assenblys.Count(); i++)
            {
                Type[] t = Assenblys[i].GetTypes();
                
                for (int j = 0; j < t.Length; j++)
                {
                    TypeInfo tj = t[j].GetTypeInfo();
                    Type baseType = tj.BaseType;
                    if (baseType == tagType)
                    {
                        object obj = Assenblys[i].CreateInstance(tj.FullName);
                        LabelValueGroup labVale = (LabelValueGroup)obj;
                        if (GroupName != "" && labVale.ConfigGroup != GroupName)
                        {
                            continue;
                        }
                        var Functions = labVale.Functions;
                        foreach (var F in Functions)
                        {
                            retList.Add(new { ConfigGroup = labVale.ConfigGroup,  ClassName = tj.FullName, FunctionName = F.FunctionName, Description = F.Description, Paras = F.Paras });
                        }
                    }
                }
            }
            return retList;

        }

    }

    public class GetDataFunctionConfig
    {
        public string Class;
        public string FunctionName;
        public List<string> ParaNames;
        public string OutPutKey;

        public GetDataFunctionConfig()
        {

        }
        public GetDataFunctionConfig(string _Class , string _Function )
        {
            Class = _Class;
            FunctionName = _Function;
        }
        public void Run(OleExec SFCDB, ConfigableLabelBase label)
        {
            var nsps = Class.Split(new char[] { '.' });
            string NameSpace = "";
            if (nsps.Length > 0)
            {
                NameSpace = nsps[0];
            }
            Type APIType;
            //加載類庫
            Assembly assembly = Assembly.Load(NameSpace);
            var CallApiClass = Class;
            APIType = assembly.GetType(Class);
            object API_CLASS = assembly.CreateInstance(Class);
            MethodInfo Function = APIType.GetMethod(FunctionName);

            object[] args = new object[ParaNames.Count + 1];
            args[0] = SFCDB;

            object _outPut = null;
            if (ParaNames.Count >= 1 && label.Keys[ParaNames[0]].GetType() == typeof(string))
            {

                for (int i = 0; i < ParaNames.Count; i++)
                {
                    args[i + 1] = label.Keys[ParaNames[i]];
                }
                _outPut = Function.Invoke(API_CLASS, args);


            }
            else if (ParaNames.Count >= 1 && label.Keys[ParaNames[0]].GetType() == typeof(List<string>))
            {
                var values = (List<string>)label.Keys[ParaNames[0]];
                _outPut = new string[values.Count()];
                for (int j = 0; j < values.Count(); j++)
                {
                    for (int i = 0; i < ParaNames.Count; i++)
                    {
                        if (label.Keys[ParaNames[i]].GetType() == typeof(string))
                        {
                            args[i + 1] = label.Keys[ParaNames[i]];
                        }
                        else
                        {
                            args[i + 1] = ((List<string>)label.Keys[ParaNames[i]])[j];
                        }
                        ((string[])_outPut)[j] = Function.Invoke(API_CLASS, args).ToString();
                    }
                }

            }
            else if (ParaNames.Count >= 1 && label.Keys[ParaNames[0]].GetType() == typeof(string[]))
            {
                var values = (string[])label.Keys[ParaNames[0]];
                _outPut = new string[values.Count()];
                for (int j = 0; j < values.Count(); j++)
                {
                    for (int i = 0; i < ParaNames.Count; i++)
                    {
                        if (label.Keys[ParaNames[i]].GetType() == typeof(string))
                        {
                            args[i + 1] = label.Keys[ParaNames[i]];
                        }
                        else
                        {
                            args[i + 1] = ((string[])label.Keys[ParaNames[i]])[j];
                        }
                        ((string[])_outPut)[j] = Function.Invoke(API_CLASS, args).ToString();
                    }
                }

            }
            else
            {
                _outPut = Function.Invoke(API_CLASS, args);
            }

            if (label.Keys.ContainsKey(OutPutKey))
            {
                label.Keys[OutPutKey] = _outPut;
            }
            else
            {
                label.Keys.Add(OutPutKey, _outPut);
            }
        }

    }
    public class LabelValueGroup
    {
        public string ConfigGroup { get; set; }
        public string Description { get; set; }
        public List<LabelValueFunctionConfig> Functions = new List<LabelValueFunctionConfig>();


    }

    public class LabelValueFunctionConfig
    {
        public string FunctionName;
        public string Description { get; set; }
        public List<string> Paras ;
    }

    public class TestLabelValueGroup: LabelValueGroup
    {
        public TestLabelValueGroup()
        {
            ConfigGroup = "TESTLabe";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSKU", Description = "获取SKU", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSNCartion", Description = "获取CartionNO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMAC", Description = "获取MAC地址", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetC_SKU_VALUE", Description = "获取C_SKU表的字段值", Paras = new List<string>() { "SN" , "KEY" } });
            
        }

        public string GetSKU(OleExec SFCDB, string SN)
        {
            return "TESTSKU";
        }
        public string GetSNCartion(OleExec SFCDB, string SN)
        {
            return "C-000000001";
        }

        public string GetMAC(OleExec SFCDB, string SN)
        {
            return "12:ab:77:21:21";
        }
        public string GetC_SKU_VALUE(OleExec SFCDB, string SKU , string KEY)
        {
            return KEY;
        }
    }

    public class TestConfigLabel:ConfigableLabelBase
    {
        public TestConfigLabel()
        {
            this.Inputs.Add(new LabelInputValue() { Name = "SN", StationSessionKey = "SN", StationSessionType = "1" });
            TestLabelValueGroup TG = new TestLabelValueGroup();

            Functions.Add(new GetDataFunctionConfig() { Class = "MESPubLab.MESStation.Label.TestLabelValueGroup", FunctionName = TG.Functions[0].FunctionName, ParaNames = TG.Functions[0].Paras, OutPutKey = "SKUNO" });
            Functions.Add(new GetDataFunctionConfig() { Class = "MESPubLab.MESStation.Label.TestLabelValueGroup", FunctionName = TG.Functions[1].FunctionName, ParaNames = TG.Functions[1].Paras, OutPutKey = "CARTIONNO" });
            Functions.Add(new GetDataFunctionConfig() { Class = "MESPubLab.MESStation.Label.TestLabelValueGroup", FunctionName = TG.Functions[2].FunctionName, ParaNames = TG.Functions[2].Paras, OutPutKey = "MAC" });
        }
    }
}
