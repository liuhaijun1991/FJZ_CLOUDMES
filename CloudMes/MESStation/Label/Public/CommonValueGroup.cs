using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System.Collections.Generic;

namespace MESStation.Label.Public
{
    public class CommonValueGroup : LabelValueGroup
    {
        public CommonValueGroup()
        {
            ConfigGroup = "CommonValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetC_Control_Value", Description = "Get country of origin", Paras = new List<string>() { "CONTROL_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDate", Description = "Get date on database system", Paras = new List<string>() { } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPages", Description = "Get date on database system", Paras = new List<string>() { "QTY" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDateFromDB", Description = "Get date with sql format on database system", Paras = new List<string>() { "Format" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDateByFormat", Description = "Get date on database system by format", Paras = new List<string>() { "Format" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDateByFormatEnglish", Description = "Get date on database system by format", Paras = new List<string>() { "Format" } });            
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "ReturnInput", Description = "Return Input Value", Paras = new List<string>() { "InputValue" } });
        }

        public string GetC_Control_Value(OleExec SFCDB, string CONTROL_NAME)
        {
            var Value = "No data record";
            var s = SFCDB.ORM.Queryable<C_CONTROL>()
            .Where(t => t.CONTROL_NAME == CONTROL_NAME)
            .Select(t => t.CONTROL_VALUE)
            .ToList();
            if (s.Count > 0)
            {
                Value = s[0];
            }
            return Value;
        }
        
        public string GetDate(OleExec SFCDB)
        {
            var date = SFCDB.ORM.GetDate().ToString("yyyy-MM-dd");
            return date;
        }

        public string GetDateFromDB(OleExec SFCDB, string Format) 
        {
            var sql = $@"SELECT TO_CHAR(SYSDATE,'{Format}') FROM DUAL";
            var date = SFCDB.ORM.Ado.SqlQuerySingle<string>(sql);
            return date;
        }

        public string GetDateByFormat(OleExec SFCDB,string Format)
        {
            var date = SFCDB.ORM.GetDate().ToString(Format);
            return date;
        }
        public string GetDateByFormatEnglish(OleExec SFCDB, string Format)
        {
            var date = SFCDB.ORM.GetDate().ToString($@"{Format}",new System.Globalization.CultureInfo("en-us"));
            return date;
        }
        public List<string> GetPages(OleExec SFCDB, string QTY)
        {
            List<string> res = new List<string>();
            int q = int.Parse(QTY);
            for (int i = 0; i < q; i++)
            {
                res.Add((i + 1).ToString());
            }
            return res;
        }
        public string ReturnInput(OleExec SFCDB, string input_value)
        {
            return input_value;
        }

    }
}
