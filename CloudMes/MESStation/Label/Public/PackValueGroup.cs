using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MESStation.Label.Public
{
    public class PackValueGroup : LabelValueGroup
    {
        public PackValueGroup()
        {
            ConfigGroup = "PackValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSNCartion", Description = "获取CartionNO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCartionSN", Description = "获取Cartion下SN列表", Paras = new List<string>() { "CARTON" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSNPallet", Description = "获取SN的棧板號", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCartonPallet", Description = "获取CARTON的棧板號", Paras = new List<string>() { "CARTON" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPalletCarton", Description = "获取棧板下CARTON列表", Paras = new List<string>() { "PALLET" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPalletSN", Description = "获取棧板下SN列表", Paras = new List<string>() { "PALLET" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPalletKpSN", Description = "获取棧板下下階的SN列表", Paras = new List<string>() { "PALLET" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetR_PACKING_VALUE", Description = "获取R_PACKING表的字段值", Paras = new List<string>() { "PACKNO","KEY" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPACK_QTY", Description = "获取R_PACKING表的QTY字段值", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPACK_TIME", Description = "获取R_PACKING表的EDIT_TIME字段值", Paras = new List<string>() { "PACKNO","FORMAT" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPL_SN_QTY", Description = "获取栈板内SN的数量", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPL_QTY_FORSN", Description = "获取栈板内包裝的SN数量", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWOByPallet", Description = "获取栈板内SN的工單", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMaxSnByPallet", Description = "獲取棧板號或者卡通號最大SN", Paras = new List<string>() { "PackNo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMixSnByPallet", Description = "獲取棧板號或者卡通號最小SN", Paras = new List<string>() { "PackNo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetYYWW", Description = "獲取年份周別", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWeightByPallet", Description = "獲取棧板內條碼對應的重量", Paras = new List<string>() { "PackNo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDNByPallet", Description = "Get DN From Shiping Pallet", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWOByCarton", Description = "Get WO From Carton Number", Paras = new List<string>() { "CARTON" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetOneSNByCarton", Description = "Get One SN From Carton Number", Paras = new List<string>() { "CARTON" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "NotPrintCtrlBySnLen", Description = "In range return False", Paras = new List<string>() { "SNs" , "FROM_LEN","TO_LEN"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSNinCarton", Description = "GetSNinCarton", Paras = new List<string>() { "CARTON" } });

        }

        public string NotPrintCtrlBySnLen(OleExec SFCDB, List<string> SNs,string FROM_LEN , string TO_LEN)
        {
            var OutPut = "FALSE";
            int fl = int.Parse(FROM_LEN);
            int tl = int.Parse(TO_LEN);
            for (int i = 0; i < SNs.Count; i++)
            {
                var L = SNs[i].Length;
                if ( !(L >= fl && L <= tl))
                {
                    OutPut = "TRUE";
                    break;
                }
            }

            return OutPut;
        }


        public string GetPL_SN_QTY(OleExec SFCDB, string PACKNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return "PACK NO FOUND";
            }
            var ctns = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == pack.ID).ToList();
            if (ctns.Count == 0)
            {
                return pack.QTY.ToString();
            }

            int c = 0;
            for (int i = 0; i < ctns.Count; i++)
            {
                c += (int)ctns[i].QTY;
            }

            return c.ToString();
        }
        public string GetPL_QTY_FORSN(OleExec SFCDB, string PACKNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            string Pallet = "No data record";
            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return Pallet;
            }

            var plsn = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((P0, P1, SP, S) => P0.ID == P1.PARENT_PACK_ID && P1.ID == SP.PACK_ID && SP.SN_ID == S.ID)
                .Where((P0, P1, SP, S) => S.VALID_FLAG == "1" && P0.PACK_NO == PACKNO).Select((P0, P1, SP, S) => S).ToList();
            if (plsn.Count() > 0)
            {
                Pallet = plsn.Count().ToString();
            }
            return Pallet;
        }

        public string GetPACK_QTY(OleExec SFCDB, string PACKNO)
        {
            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return "PACK NO FOUND";
            }
            //Juniper 發現有時pack.QTY!=R_SN_PACKING裏的總數的bug,故先暫時按以下方式取數量
            if (pack.PACK_TYPE == "CARTON")
            {
                return SFCDB.ORM.Queryable<R_SN_PACKING>().Where(r => r.PACK_ID == pack.ID).ToList().Count().ToString();
            }
            return pack.QTY.ToString();
        }
        public string GetPACK_TIME(OleExec SFCDB, string PACKNO,string FORMAT)
        {
            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return "PACK NO FOUND";
            }
            try
            {
                DateTime t = (DateTime)pack.EDIT_TIME;

                //var xx = t.ToString("MMM,dd,yyyy", CultureInfo.CreateSpecificCulture("en-GB"));//Aug,19,2020
                //var yy = t.ToString("yyyy-MM-dd", CultureInfo.CreateSpecificCulture("en-GB"));//2020-08-19

                return t.ToString(FORMAT, CultureInfo.CreateSpecificCulture("en-GB"));

            } catch
            {
                return "NULL";
            }
            
        }

        public string GetSNCartion(OleExec SFCDB, string SN)
        {
            var Carton = "No data record";
            var OSN = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").First();
            var P_SN = SFCDB.ORM.Queryable<R_SN_PACKING>().Where(t => t.SN_ID == OSN.ID).First();
            var R_PACKING = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.ID == P_SN.PACK_ID).First();
            return R_PACKING.PACK_NO;
            var s= SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((P, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P, SP, S) => S.SN == SN && S.VALID_FLAG == "1")
            .Select((P, SP, S) => P.PACK_NO)
            .ToList();
            if (s.Count>0)
            {
                Carton = s[0];
            }
            return Carton;
        }

        public List<string> GetCartionSN(OleExec SFCDB, string CARTON)
        {
            var s = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((P, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P, SP, S) => P.PACK_NO == CARTON && S.VALID_FLAG == "1")
            .Select((P, SP, S) => S.SN)
            .ToList();
            return s;
        }

        public string GetSNinCarton(OleExec SFCDB, string CARTON)
        {
            var s = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((P, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P, SP, S) => P.PACK_NO == CARTON && S.VALID_FLAG == "1")
            .Select((P, SP, S) => S.SN)
            .ToList();
            return s[0];
        }

        public string GetSNPallet(OleExec SFCDB, string SN)
        {
            var Pallet = "No data record"; 
            var s1 = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((P, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P, SP, S) => S.SN == SN && P.PACK_TYPE== "PALLET" && S.VALID_FLAG == "1")
            .Select((P, SP, S) => P.PACK_NO)
            .ToList();

            var s2 = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((P1,P2, SP, S) => new object[] {
                SqlSugar.JoinType.Left,P1.ID==P2.PARENT_PACK_ID,
                SqlSugar.JoinType.Left, P2.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P1, P2, SP, S) => S.SN == SN && P1.PACK_TYPE == "PALLET" && S.VALID_FLAG == "1")
            .Select((P1, P2, SP, S) => P1.PACK_NO)
            .ToList();

            s1.AddRange(s2);
            if (s1.Count > 0)
            {
                Pallet = s1[0];
            }
            return Pallet;
        }

        public string GetCartonPallet(OleExec SFCDB, string CARTON)
        {
            var Carton = "No data record";
            var s = SFCDB.ORM.Queryable<R_PACKING, R_PACKING>((P1, P2) => new object[] {
                SqlSugar.JoinType.Left, P1.ID == P2.PARENT_PACK_ID
            })
            .Where((P1, P2) => P1.PACK_TYPE=="PALLET" && P2.PACK_NO==CARTON)
            .Select((P1,P2) => P1.PACK_NO)
            .ToList();            
            if (s.Count > 0)
            {
                Carton = s[0];
            }
            return Carton;
        }

        public List<string> GetPalletCarton(OleExec SFCDB, string PALLET)
        {
            var s = SFCDB.ORM.Queryable<R_PACKING, R_PACKING>((P1, P2) => new object[] {
                SqlSugar.JoinType.Left, P1.PARENT_PACK_ID == P2.ID
            })
            .Where((P1, P2) => P1.PACK_TYPE == "CARTON" && P2.PACK_NO == PALLET)
            .Select((P1, P2) => P1.PACK_NO)
            .ToList();
            return s;
        }

        public List<string> GetPalletSN(OleExec SFCDB, string PALLET)
        {
            PALLET = PALLET.Replace("MBO", "").ToString();
            var s = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((P, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P, SP, S) => P.PACK_NO == PALLET && S.VALID_FLAG == "1")
            .Select((P, SP, S) => S.SN)
            .ToList();

            var s2 = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((P1,P2, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P1.ID == P2.PARENT_PACK_ID,
                SqlSugar.JoinType.Left, P2.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P1, P2, SP, S) => P1.PACK_NO == PALLET && S.VALID_FLAG == "1")
            .Select((P1, P2, SP, S) => S.SN)
            .ToList();
            s.AddRange(s2);
            return s;
        }

        public List<string> GetPalletKpSN(OleExec SFCDB, string PALLET)
        {
            PALLET = PALLET.Replace("MBO", "").ToString();
            var s = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((P, SP, S) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID
            })
            .Where((P, SP, S) => P.PACK_NO == PALLET && S.VALID_FLAG == "1")
            .Select((P, SP, S) => S.SN)
            .ToList();

            var s2 = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN,R_SN_KP>((P1, P2, SP, S,SK) => new object[] {
                SqlSugar.JoinType.Left, P1.ID == P2.PARENT_PACK_ID,
                SqlSugar.JoinType.Left, P2.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID,
                SqlSugar.JoinType.Left, S.SN==SK.SN
            })
            .Where((P1, P2, SP, S) => P1.PACK_NO == PALLET && S.VALID_FLAG == "1")
            .Select((P1, P2, SP, S,SK) => SK.VALUE)
            .ToList();

            s.AddRange(s2);
            return s;
        }

        public string GetR_PACKING_VALUE(OleExec SFCDB, string PACKNO,string KEY)
        {
            var value = "No data record";
            var s = SFCDB.ORM.Queryable<R_PACKING>()
            .Where(t=> t.PACK_NO == PACKNO)
            .Select<string>(KEY)
            .ToList();
            if (s.Count>0)
            {
                value = s[0];
            }
            return value;
        }

        /// <summary>
        /// 获取栈板内SN的工單
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="PACKNO"></param>
        /// <returns></returns>
        public string GetWOByPallet(OleExec SFCDB, string PACKNO)
        {
            string value = "No data record";
            var s = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rp1, rp2, rsp, rs) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID)
                .Where((rp1, rp2, rsp, rs) => rp1.PACK_NO == PACKNO).Select((rp1, rp2, rsp, rs) => rs.WORKORDERNO).ToList();
            if (s.Count > 0)
            {
                value = s[0];
            }
            return value;
        }

        public string GetMaxSnByPallet(OleExec SFCDB, string PACKNO)
        {

            string value = "No data record";
            var Packing = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).Select(t => t.ID).ToList();
            var LastPacking = Packing;
            while (Packing.Count > 0)
            {
                Packing = SFCDB.ORM.Queryable<R_PACKING>().Where(t => Packing.Contains(t.PARENT_PACK_ID)).Select(t => t.ID).ToList();
                if (Packing.Count > 0)
                {
                    LastPacking = Packing;
                }
            }
            var SNs = SFCDB.ORM.Queryable<R_SN_PACKING, R_SN>((rsp, rs) => rsp.SN_ID == rs.ID).Where((rsp, rs) => LastPacking.Contains(rsp.PACK_ID))
                .OrderBy((rsp, rs) => rs.SN, SqlSugar.OrderByType.Desc).Select((rsp, rs) => rs.SN).First();

            if (SNs.Length>0)
            {
                value = SNs;
            }

            return value;
        }

        public string GetMixSnByPallet(OleExec SFCDB, string PACKNO)
        {

            string value = "No data record";
            var Packing = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).Select(t => t.ID).ToList();
            var LastPacking = Packing;
            while (Packing.Count > 0)
            {
                Packing = SFCDB.ORM.Queryable<R_PACKING>().Where(t => Packing.Contains(t.PARENT_PACK_ID)).Select(t => t.ID).ToList();
                if (Packing.Count > 0)
                {
                    LastPacking = Packing;
                }
            }
            var SNs = SFCDB.ORM.Queryable<R_SN_PACKING, R_SN>((rsp, rs) => rsp.SN_ID == rs.ID).Where((rsp, rs) => LastPacking.Contains(rsp.PACK_ID))
                .OrderBy((rsp, rs) => rs.SN, SqlSugar.OrderByType.Asc).Select((rsp, rs) => rs.SN).First();

            if (SNs.Length > 0)
            {
                value = SNs;
            }

            return value;
        }

        public string GetYYWW(OleExec SFCDB, string PACKNO)
        {

            string yy = DateTime.Today.ToString("yy");
            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            string ww = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
            string yyww = yy + ww;

            return yyww;

        }

        public string GetWeightByPallet(OleExec SFCDB, string PACKNO)
        {
            var qty = GetPL_SN_QTY(SFCDB, PACKNO);
            var Total_weight=SFCDB.ORM.Queryable<C_SKU_DETAIL, R_PACKING>((csd,rp)=>csd.SKUNO==rp.SKUNO)
                .Where((csd, rp)=> rp.PACK_NO == PACKNO&&csd.CATEGORY== "Total_weight").Select((csd, rp) => csd.VALUE).First();
            var Net_weight = SFCDB.ORM.Queryable<C_SKU_DETAIL, R_PACKING>((csd, rp) => csd.SKUNO == rp.SKUNO)
                .Where((csd, rp) => rp.PACK_NO == PACKNO && csd.CATEGORY == "Net_weight").Select((csd, rp) => csd.VALUE).First();

            var PalletQty = SFCDB.ORM.Queryable<C_PACKING, R_PACKING>((cp, rp) => cp.SKUNO == rp.SKUNO && cp.PACK_TYPE == "PALLET").Where((cp, rp) => rp.PACK_NO == PACKNO)
                .Select((cp, rp) => cp.MAX_QTY).First();

            var CartonQty = SFCDB.ORM.Queryable<C_PACKING, R_PACKING>((cp, rp) => cp.SKUNO == rp.SKUNO && cp.PACK_TYPE == "CARTON").Where((cp, rp) => rp.PACK_NO == PACKNO)
                .Select((cp, rp) => cp.MAX_QTY).First();

            var _Total_weight = Total_weight.Replace("KG","").ToString();
            var _Net_weight = Net_weight.Replace("KG", "").ToString();
            var _PalletQty = PalletQty.ToString();
            var _CartonQty = CartonQty.ToString();

            var PalletWeight = double.Parse(_Total_weight) - ((double.Parse(_PalletQty) * double.Parse(_CartonQty)) - double.Parse(qty)) * double.Parse(_Net_weight);

            var _PalletWeight = PalletWeight.ToString() + "KG";

            return _PalletWeight;
        }

        public string GetDNByPallet(OleExec SFCDB, string PALLETNO) {
            PALLETNO = PALLETNO.Trim().ToUpper();
            List<R_SHIP_DETAIL> listShipping = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_SHIP_DETAIL>
                ((pallet, carton, rsp, sn, rsd) => pallet.ID == carton.PARENT_PACK_ID && carton.ID == rsp.PACK_ID && rsp.SN_ID == sn.ID && sn.SN == rsd.SN)
                .Where((pallet, carton, rsp, sn, rsd) => pallet.PACK_NO == PALLETNO && sn.VALID_FLAG == "1")
                .Select((pallet, carton, rsp, sn, rsd) => rsd)
                .Distinct().ToList();

            if (listShipping.Count == 0)
            {                
                return "";
            }
            else
            {
                return listShipping.FirstOrDefault().DN_NO;
            }
        }

        public string GetWOByCarton(OleExec SFCDB, string CARTON)
        {
            string output = "";
            CARTON = CARTON.Trim().ToUpper();
            var carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PACK_NO == CARTON).ToList().First();

            if (carton_obj.QTY == 0)
            {
                carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PARENT_PACK_ID == carton_obj.PARENT_PACK_ID && r.QTY > 0).ToList().FirstOrDefault();
            }

            if (carton_obj != null)
            {
                var list_sn = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING>((sn, rp) => sn.ID == rp.SN_ID)
                    .Where((sn, rp) => sn.VALID_FLAG == "1" && rp.PACK_ID == carton_obj.ID).Select((sn, rp) => sn).ToList();
                if (list_sn.Count != 0)
                {
                    output = list_sn.FirstOrDefault().WORKORDERNO;
                }
            }
            return output; 
        }
        public string GetOneSNByCarton(OleExec SFCDB, string CARTON)
        {
            string output = "";
            CARTON = CARTON.Trim().ToUpper();
            var carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PACK_NO == CARTON).ToList().FirstOrDefault();

            if (carton_obj != null)
            {
                var list_sn = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING>((sn, rp) => sn.ID == rp.SN_ID)
                    .Where((sn, rp) => sn.VALID_FLAG == "1" && rp.PACK_ID == carton_obj.ID).Select((sn, rp) => sn).ToList();
                if (list_sn.Count != 0)
                {
                    output = list_sn.FirstOrDefault().SN;
                }
            }
            return output;
        }
    }
}
