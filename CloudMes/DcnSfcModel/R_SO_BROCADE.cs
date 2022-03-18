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
    public class R_SO_BROCADE
    {
        public R_SO_BROCADE()
        {
			
        }
 
       #region  R_SO_BROCADE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_LINE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOCATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Transno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Commited {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Commitdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string foxaccept {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Scheduledate {get; set;}
	
	
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
 
 