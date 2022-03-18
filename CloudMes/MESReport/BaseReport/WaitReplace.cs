using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class WaitReplace : ReportBase
    {
        ReportInput Sn = new ReportInput() { Name = "SN", InputType = "TextArea", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public WaitReplace()
        {
            Inputs.Add(Sn);
        }
        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public override void Run()
        {
            string sn = Sn.Value.ToString().Trim();
            sn = $@"'{sn.Replace("\n", "',\n'")}'";
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sql = $@"SELECT linktype,newsn,oldsn,station,remark,edittime,editby FROM r_sn_replace ";
                if (Sn.Value.ToString() != "ALL")
                {
                    sql = sql + $@" where newsn  in ({sn} )  or oldsn in ({sn} )  ";
                }
                sql = sql + $@" order by edittime DESC";
                RunSqls.Add(sql);
                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Replace SN Report";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }

    }
}
