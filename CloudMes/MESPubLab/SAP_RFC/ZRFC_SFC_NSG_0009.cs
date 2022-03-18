using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZRFC_SFC_NSG_0009 : SAP_RFC_BASE
    {
        /*
                O_ERROR	CHAR		Variable part of a message
                O_ERRORMESSAGE	CHAR		Variable part of a message
                O_FLAG	CHAR		Flag Material for Deletion at Client Level
                O_MESSAGE	CHAR		Variable part of a message
                I_AUFNR	CHAR		Order Number
                I_BUDAT	DATE	SY-DATUM	Posting date
                I_LMNGA	BCD		Yield currently to be confirmed
                I_STATION	CHAR		Operation Number
             */
        /// <summary>
        /// 
        /// </summary>
        public ZRFC_SFC_NSG_0009(string BuName): base(BuName)
        {
        SetRFC_NAME("ZRFC_SFC_NSG_0009");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Wo"></param>
    /// <param name="SAPStationCode"></param>
    /// <param name="QTY"></param>
    /// <param name="PostDate">"mm/dd/yyyy"</param>
    public void SetValue(string Wo, string SAPStationCode, string QTY, string PostDate)
    {
        this.ClearValues();
        SetValue("I_AUFNR", Wo);
        SetValue("I_STATION", SAPStationCode);
        SetValue("I_LMNGA", QTY);
        SetValue("I_BUDAT", PostDate);

    }
}
}
