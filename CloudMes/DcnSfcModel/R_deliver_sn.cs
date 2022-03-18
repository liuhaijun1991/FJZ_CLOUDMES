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
    public class R_deliver_sn
    {
        public R_deliver_sn()
        {
			
        }
 
       #region  R_deliver_sn实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DeliverNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DeliverLine {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sendflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Suppliercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 