using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MESDataObject.Module;
using Newtonsoft.Json;
using System.Reflection;

namespace MESPubLab.MESStation.Label
{
    public class LabelBase
    {
        public List<LabelInputValue> Inputs = new List<LabelInputValue>();
        public List<LabelOutput> Outputs = new List<LabelOutput>()
        {
            new LabelOutput() { Name="PAGE" , Type = LabelOutPutTypeEnum.String , Value = "1" },
            new LabelOutput() { Name="ALLPAGE" , Type = LabelOutPutTypeEnum.String , Value = "1" }
        };
        public string LabelName = "";
        public string FileName = "";
        public int PrintQTY = 1;
        public int PrinterIndex = 1;
        public virtual void MakeLabel(OleExec DB)
        {
            throw new NotImplementedException();
        }
        public int PAGE
        {
            get
            {
                try
                {
                    return int.Parse((string)Outputs.Find(T => T.Name == "PAGE").Value);
                }
                catch
                {
                    return 1;
                }
            }

            set
            {
                Outputs.Find(T => T.Name == "PAGE").Value = value.ToString();
            }
        }

        public int ALLPAGE
        {
            get
            {
                try
                {
                    return int.Parse((string)Outputs.Find(T => T.Name == "ALLPAGE").Value);
                }
                catch
                {
                    return 1;
                }
            }

            set
            {
                Outputs.Find(T => T.Name == "ALLPAGE").Value = value.ToString();
            }
        }
        /// <summary>
        /// 根據R_LABEL的配置將文檔分頁
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="RL"></param>
        /// <returns></returns>
        public static List<LabelBase> MakePrintPage(LabelBase lab, double? L)
        {
            List<LabelBase> RET = new List<LabelBase>();
            if (L == null || L == 0)
            {
                RET.Add(lab);
                return RET;
            }
            LabelOutput arry1 = lab.Outputs.Find(T => T.Type == LabelOutPutTypeEnum.StringArry);
            if (arry1 == null)
            {
                RET.Add(lab);
                return RET;
            }

            int l = 0;
            if (arry1.Value.GetType().Name == "String[]")
            {
                l = ((string[])arry1.Value).Length;
            }
            else
            {
                l = ((List<string>)arry1.Value).Count;
            }
            if (l == 0)
            {
                RET.Add(lab);
                return RET;
            }
            LabelBase p = null;

            List<LabelOutput> O = lab.Outputs.FindAll(T => T.Type == LabelOutPutTypeEnum.StringArry && T.Name != "PAGE" && T.Name != "ALLPAGE");

            int page = 0;
            for (int i = 0; i < l; i++)
            {

                if (i % L == 0)
                {
                    p = new LabelBase();
                    page++;
                    p.PAGE = page;
                    CopyLabString(lab, p);
                    RET.Add(p);
                }
                for (int j = 0; j < O.Count; j++)
                {
                    LabelOutput OP = p.Outputs.Find(T => T.Name == O[j].Name);
                    if (O[j].Value.GetType().Name == "String[]")
                    {
                        ((List<string>)OP.Value).Add(((string[])O[j].Value)[i]);
                    }
                    else
                    {
                        ((List<string>)OP.Value).Add(((List<string>)O[j].Value)[i]);
                    }
                }
            }

            return RET;

        }
        /// <summary>
        /// 拷貝2個輸出的固定部分
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        static void CopyLabString(LabelBase l1, LabelBase l2)
        {
            List<LabelOutput> O = l1.Outputs.FindAll(T => T.Type == LabelOutPutTypeEnum.String && T.Name != "PAGE" && T.Name != "ALLPAGE");
            for (int i = 0; i < O.Count; i++)
            {
                l2.Outputs.Add(new LabelOutput() { Name = O[i].Name, Type = O[i].Type, Value = O[i].Value == null ? "" : O[i].Value.ToString(), Description = O[i].Description });
            }
            O = l1.Outputs.FindAll(T => T.Type == LabelOutPutTypeEnum.StringArry && T.Name != "PAGE" && T.Name != "ALLPAGE");
            for (int i = 0; i < O.Count; i++)
            {
                l2.Outputs.Add(new LabelOutput() { Name = O[i].Name, Type = O[i].Type, Value = new List<string>(), Description = O[i].Description });
            }
            l2.LabelName = l1.LabelName;
            l2.FileName = l1.FileName;
            l2.PrintQTY = l1.PrintQTY;
            l2.PrinterIndex = l1.PrinterIndex;

        }

    }

    public class LabelInputValue
    {
        public string Name;
        [JsonIgnore]
        [ScriptIgnore]
        public object Value;
        public string Type;

        public string StationSessionType;
        public string StationSessionKey;
        public string StationSessionValue = "";
    }


    public class LabelOutput
    {
        public string Name = "";
        public string Description = "";
        public LabelOutPutTypeEnum Type = LabelOutPutTypeEnum.String;
        public object Value;
    }

    public enum LabelOutPutTypeEnum
    {
        String = 0,
        StringArry = 1
    }

}
