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
    public class R_REPAIR_DAILYREPORT_SN
    {
        public R_REPAIR_DAILYREPORT_SN()
        {
			
        }
 
       #region  R_REPAIR_DAILYREPORT_SN实体
 
	
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
	   public string sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sn_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkin_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkout_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime failtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime first_failtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal sn_agile {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime editdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string p_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string final_status {get; set;}
	
	   
	   #endregion
    }
}
 
 