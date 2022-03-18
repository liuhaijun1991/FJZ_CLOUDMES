using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESStation.Label;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Api
{
    public class BundlePackAPI : MesAPIBase
    {
        protected APIInfo FGetBundleInfo = new APIInfo()
        {
            FunctionName = "GetBundleInfo",
            Description = "",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SN", InputType = "string" } },
            Permissions = new List<MESPermission>() { }
        };

        public BundlePackAPI()
        {
            this.Apis.Add(FGetBundleInfo.FunctionName, FGetBundleInfo);
        }

        public void GetBundleInfo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            var sn = Data["SN"].ToString();
            OleExec _DB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = new ResData();
                var snObj = _DB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
                if (snObj == null)
                {
                    var rsn = _DB.ORM.Queryable<R_SN, O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((S, O, I, H) => S.WORKORDERNO == O.PREWO && O.ITEMID == I.ID && I.TRANID == H.TRANID)
                        .Where((S, O, I, H) => S.SN == sn && S.VALID_FLAG == "1")
                        .Select((S, O, I, H) => new { S.SN,S.SKUNO, I.SALESORDERLINEITEM, H.SALESORDERNUMBER })
                        .First();
                    if (rsn == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { "SN:" + sn }));
                    }
                    //var rcsn = _DB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "2" && t.SKUNO.StartsWith("750-")).First();
                    //if (rcsn == null)
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { "SN:" + sn }));
                    //}
                    //var clei = _DB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == rcsn.ID && t.KP_NAME == "CLEI LABEL" && t.VALID_FLAG == 1).First();
                    //if (clei == null)
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { "750's CLEI KP", " SN:", sn }));
                    //}
                    //res.SNData = new List<SNCLEI>() { new SNCLEI() { SN = rsn.S.SN, SKUNO = rsn.S.SKUNO, CLEI = clei.VALUE } };

                    var clei = _DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == rsn.SN && t.KP_NAME == "AutoKP" && t.VALID_FLAG == 1).First();
                    res.SNData = new List<SNCLEI>() { new SNCLEI() { SN = rsn.SN, SKUNO = rsn.SKUNO, CLEI = "[)>0625SLBJPNW" + rsn.SN +"11P" + clei.EXVALUE2 } };
                    //res.SOLine = rsn.H.SALESORDERNUMBER.TrimStart('0') + "." + rsn.I.SALESORDERLINEITEM.TrimStart('0').TrimEnd('1').PadRight(4, '0');
                    if (rsn.SALESORDERLINEITEM.TrimStart('0').Length == 4)
                    {
                        res.SOLine = rsn.SALESORDERNUMBER.TrimStart('0') + "." + rsn.SALESORDERLINEITEM.TrimStart('0').TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9').PadRight(4, '0');
                    }
                    else
                    {
                        res.SOLine = rsn.SALESORDERNUMBER.TrimStart('0') + "." + rsn.SALESORDERLINEITEM.TrimStart('0').TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9').PadRight(5, '0');
                    }
                }
                else
                {
                    var bndldata = _DB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.BNDL_NO == snObj.BNDL_NO && t.VALID_FLAG == "1").ToList();
                    var soinfoList = _DB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                    .Where((O, I, H) => I.SOID == snObj.SOID && H.SALESORDERNUMBER == snObj.SALEORDER)
                    .Select((O, I, H) => new { O.PREWO, O.PID, I.QUANTITY,I.SOQTY })
                    .Distinct()
                    .ToList();
                    for (int i = 0; i < soinfoList.Count; i++)
                    {
                        var bCount = bndldata.Where(t => t.WORKORDERNO == soinfoList[i].PREWO).ToList().Count;
                        var NeedQty = Convert.ToInt32(double.Parse(soinfoList[i].QUANTITY) / double.Parse(soinfoList[i].SOQTY));
                        if (bCount != NeedQty)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_SNQTYMEET", new string[] { soinfoList[i].PID }));
                        }
                    }
                    var snid = bndldata.Select(t => t.R_SN_ID).ToList();
                    var snkps = _DB.ORM.Queryable<R_SN_KP>().Where(t => snid.Contains(t.R_SN_ID) && t.KP_NAME == "AutoKP" && t.VALID_FLAG == 1).ToList();
                    var r = bndldata.Select(t => new SNCLEI { SN = t.SN, SKUNO = t.SKUNO, CLEI = "" }).ToList();
                    for (int i = 0; i < snkps.Count; i++)
                    {
                        var SkuInfo = _DB.ORM.Queryable<C_SKU, C_SERIES, R_SN>((S, C, R) => S.C_SERIES_ID == C.ID && S.SKUNO == R.SKUNO)
                            .Where((S, C, R) => R.ID == snkps[i].R_SN_ID && R.VALID_FLAG == "1" && S.SKU_TYPE.Contains( "OPTICS") && C.SERIES_NAME.Contains("Juniper-Optics"))
                            .Select((S, C, R) => S)
                            .First();
                        if (SkuInfo == null)
                        {
                            r.Find(t => t.SN == snkps[i].SN).CLEI = "[)>0625SLBJPNW" + snkps[i].SN + "11P" + snkps[i].EXVALUE2;

                        }
                        else
                        {
                            r.Find(t => t.SN == snkps[i].SN).CLEI = "[)>0611P" + snkps[i].EXVALUE2;
                        }
                    }
                    //REMOVE BY LHJ 2021年11月22日 ，request BY POHHONE
                    //var sns = bndldata.Select(t => t.SN).ToList();
                    //var csnkp = _DB.ORM.Queryable<R_SN_KP>().Where(t => sns.Contains(t.SN) && t.KP_NAME == "CLEI LABEL" && t.PARTNO == "CLEI LABEL" && t.VALID_FLAG == 1).ToList();
                    //for (int i = 0; i < csnkp.Count; i++)
                    //{
                    //    r.Find(t => t.SN == csnkp[i].SN).CLEI = csnkp[i].VALUE;
                    //}
                    res.SNData = r.OrderBy(t => t.SN).ToList();
                    res.SOLine = snObj.SALEORDER.TrimStart('0') + "." + snObj.SOID.TrimStart('0');
                }
                
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(_DB);
            }
        }
        class SNCLEI
        {
            public string SKUNO { get; set; }
            public string SN { get; set; }
            public string CLEI { get; set; }
        }
        class ResData
        {
            public List<SNCLEI> SNData { get; set; }
            public string SOLine { get; set; }
        }
    }

}
