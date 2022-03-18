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
    public class R_Autoship_status
    {
        public R_Autoship_status()
        {
			
        }
 
       #region  R_Autoship_status实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string solineno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Promisedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int orderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DNflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DNmes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ABflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ABmes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Shipflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Shipmes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PGIflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PGIMes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string InvoiceFlag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string InvoiceMes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 