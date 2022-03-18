using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.KeyPart
{
    public class SN_KP
    {

        public List<R_SN_KP> KPS;

        public List< C_SKU_MPN> MPNs = new List<C_SKU_MPN>();
        public Dictionary<string, List< R_WO_KP_Repalce>> Repalce = new Dictionary<string, List<R_WO_KP_Repalce>>();
        /// <summary>
        /// key =RepalcePn+scantype
        /// </summary>
        public List<C_KP_Rule> KPRule = new List< C_KP_Rule>();

        /// <summary>
        /// Add by James Zhu 12/17/2019 for keypart trim function

        public List<C_KP_TRIM> KPTrims = new List<C_KP_TRIM>();
        /// </summary>

        public SN_KP()
        {

        }

        public SN_KP(List<R_SN_KP> _Value ,string Wo,string Skuno, OleExec SFCDB , string BU)
        {
            KPS = _Value;

            T_C_SKU_MPN TCSM = new T_C_SKU_MPN(SFCDB, DB_TYPE_ENUM.Oracle);
            MPNs = TCSM.GetMpnBySku(SFCDB, Skuno);
            T_R_WO_KP_Repalce TRWKR = new T_R_WO_KP_Repalce(SFCDB, DB_TYPE_ENUM.Oracle);

            if (BU=="ORACLE")
            {
                T_C_KP_TRIM TKPTRIM = new T_C_KP_TRIM(SFCDB, DB_TYPE_ENUM.Oracle);
                KPTrims = TKPTRIM.GetKPTrimBySkuWo("", Skuno, SFCDB);
            }

            List< R_WO_KP_Repalce> Repalce1 = TRWKR.GetWoRepalceKP(Wo, SFCDB);

            T_C_KP_Rule TCKR = new T_C_KP_Rule(SFCDB, DB_TYPE_ENUM.Oracle);
            KPRule = TCKR.GetDataBySkuWo(Wo, Skuno, SFCDB);
            for (int i = 0; i < Repalce1.Count; i++)
            {
                if (!Repalce.ContainsKey(Repalce1[i].PARTNO))
                {
                    Repalce.Add(Repalce1[i].PARTNO, new List<R_WO_KP_Repalce>());
                }
                Repalce[Repalce1[i].PARTNO].Add(Repalce1[i]);

            }

            for (int i = 0; i < KPS.Count; i++)
            {
                if (KPS[i].VALUE == "" || KPS[i].VALUE == null)
                {
                    C_KP_Rule rule = KPRule.Find(T => T.PARTNO == KPS[i].PARTNO && T.SCANTYPE == KPS[i].SCANTYPE);
                    if (BU == "HWD" && rule == null)
                    {
                        if (KPS[i].SCANTYPE.ToUpper() == "SYSTEMSN")
                        {
                            rule = new C_KP_Rule() { MPN = "HWD", PARTNO = KPS[i].PARTNO, SCANTYPE = KPS[i].SCANTYPE, REGEX = $@"[\w\W]*" };
                            KPRule.Add(rule);
                           
                        }
                    }
                    if (rule == null)
                    {
                        
                        List<R_WO_KP_Repalce> rep = null;
                        try
                        {
                            rep = Repalce[KPS[i].PARTNO];
                        } catch
                        { }
                        if (rep != null)
                        {
                            for (int j = 0; j < rep.Count; j++)
                            {
                                rule = KPRule.Find(T => T.PARTNO == rep[j].REPALCEPARTNO && T.SCANTYPE == KPS[i].SCANTYPE);
                                if (rule == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            
                            if (rule == null)
                            {
                                //throw new Exception(KPS[i].PARTNO+"的替代料"+ rep[i].REPALCEPARTNO + "," + KPS[i].SCANTYPE + "未配置規則!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141601", new string[] { KPS[i].PARTNO, rep[i].REPALCEPARTNO, KPS[i].SCANTYPE }));
                            }
                        }
                        
                        //throw new Exception(KPS[i].PARTNO + "," + KPS[i].SCANTYPE + "未配置規則!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141816", new string[] { KPS[i].PARTNO, KPS[i].SCANTYPE }));
                    }

                }
            }

        }


    }
}
