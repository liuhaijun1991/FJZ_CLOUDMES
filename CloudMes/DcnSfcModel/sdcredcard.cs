using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
 
namespace DcnSfcModel
{
    /// <summary>
    /// 功能: 实体类 ()
    /// 创建人：Eden     
    /// 创建日期：2019/12/31    
    /// </summary>
    [Serializable]
    public class sdcredcard
    {
        public sdcredcard()
        {
			
        }
 
       #region  sdcredcard实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cardno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cardtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string expiredate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal amount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cardholder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string addr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string zip {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool adrverified {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string authno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool authorized {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool deposited {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool credited {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasttran {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool payconfirmed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime payconfirmdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string avscode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transactionid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transactiontime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string acicode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string responsecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string validcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string traceno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string processor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime aprvdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime dpstdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string adtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool submited {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string submitid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal authamount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool paycleared {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 