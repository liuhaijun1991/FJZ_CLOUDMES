using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    class JuniperWeightReport : ReportBase
    {
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "WO/SN/PACKNO", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JuniperWeightReport()
        {
            Inputs.Add(inputSN);
        }

        public override void Init()
        {
            //base.Init();
        }
        public override void Run()
        {
            DataTable dt ;
            base.Run();
            string SqlWeight;
           string sn = inputSN.Value.ToString().Trim();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try

            {
                if (sn != "")
                {
                    SqlWeight = $@"select distinct a.workorderno,d.pack_no,c.WEIGHT,c.CREATETIME,c.CREATEBY From r_sn a,r_sn_packing b,r_weight c,r_packing d where a.id=b.sn_id 
                                        and c.snid=b.pack_id and d.id=b.pack_id and( a.workorderno='{sn}' or a.sn='{sn}' or d.pack_no='{sn}')";
                }
                else
                {
                    SqlWeight = $@"select distinct a.workorderno,d.pack_no,c.WEIGHT,c.CREATETIME,c.CREATEBY From r_sn a,r_sn_packing b,r_weight c,r_packing d where a.id=b.sn_id 
                                    and c.snid=b.pack_id and d.id=b.pack_id  order by c.CREATETIME desc ";

                }

               
                dt = SFCDB.RunSelect(SqlWeight).Tables[0];
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
   
                reportTable.Tittle = "WeightReport";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart rAlart = new ReportAlart(ex.Message);
                Outputs.Add(rAlart);
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
