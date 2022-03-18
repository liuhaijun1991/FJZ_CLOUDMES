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
    public class R_REPAIR_DAILYREPORT
    {
        public R_REPAIR_DAILYREPORT()
        {
			
        }
 
       #region  R_REPAIR_DAILYREPORT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string report_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bu {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string series {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string p_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int balance_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int checkin_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int checkout_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int overtime4h_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int overtime8h_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int send_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime send_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int last_balance_qty {get; set;}
	
	   
	   #endregion
    }
}
 
 