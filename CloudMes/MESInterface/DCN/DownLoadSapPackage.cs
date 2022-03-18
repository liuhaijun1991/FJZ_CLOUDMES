using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.SAP_RFC;
using System.Data;
using MESPubLab.MesBase;

namespace MESInterface.DCN
{
    public class DownLoadSapPackage : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, apdbstr, bustr, seriesname, Series, filepath, filebackpath, remotepath;

        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
                bustr = ConfigGet("BU");

            }
            catch (Exception e)
            {
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                using (var mesdb = OleExec.GetSqlSugarClient(this.mesdbstr, false))
                {
                    var waittartget = mesdb.Queryable<C_SKU>().ToList();
                    var rfc = new ZCSD_NSBG_0008(this.bustr);
                    var dt = rfc.GetInitINA();
                    foreach (var skuobj in waittartget)
                    {
                        var dr = dt.NewRow();
                        dr["PN"] = skuobj.SKUNO;
                        dt.Rows.Add(dr);
                    }
                    rfc.SetValue(dt);
                    rfc.CallRFC();
                    var res = rfc.GetTableValue("OUTB");
                    mesdb.Deleteable<R_SAP_PACKAGE>().ExecuteCommand();
                    var indblist =new List<R_SAP_PACKAGE>();
                    foreach (DataRow item in res.Rows)                    
                        indblist.Add(new R_SAP_PACKAGE()
                        {
                            ID = MesDbBase.GetNewID<R_SAP_PACKAGE>(mesdb, this.bustr),
                            BU = item["BU"].ToString(),
                            PN = item["PN"].ToString(),
                            DESCRIPTION = item["DESCRIPTION"].ToString(),
                            BOX_NT = item["BOX_NT"].ToString(),
                            BOX_GW = item["BOX_GW"].ToString(),
                            PCS_NT = item["PCS_NT"].ToString(),
                            PCS_GW = item["PCS_GW"].ToString(),
                            P_NULLWG = item["P_NULLWG"].ToString(),
                            PCS_B = item["PCS_B"].ToString(),
                            PCS_P = item["PCS_P"].ToString(),
                            BOX_P = item["BOX_P"].ToString(),
                            P_GW = item["P_GW"].ToString(),
                            P_GROES = item["P_GROES"].ToString(),
                            C_GROES = item["C_GROES"].ToString(),
                            CREATETIME = DateTime.Now,
                            CREATEBY = MesConst.MesNullStr
                        });
                    mesdb.Insertable(indblist).ExecuteCommand();
                }
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }
    }
}
