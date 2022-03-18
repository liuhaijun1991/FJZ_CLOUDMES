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
    public class mmendinv
    {
        public mmendinv()
        {
			
        }
 
       #region  mmendinv实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string endinvdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string whid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal commitqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal intransitqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal poqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal currentqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal hiqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal safeqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal loqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal outtransqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal woqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal woreleaseqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal mrqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string abccode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string taxcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ood {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ooddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currencytype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitcost {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lastauditdt {get; set;}
	
	
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
 
 