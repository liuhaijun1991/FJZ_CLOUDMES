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
    public class R_REPAIR_AGINGREPORT
    {
        public R_REPAIR_AGINGREPORT()
        {
			
        }
 
       #region  R_REPAIR_AGINGREPORT实体
 
	
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
	   public int day1within {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int days1_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int days4_7 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int days8_14 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int days15_30 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int over30days {get; set;}
	
	
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
	
	   
	   #endregion
    }
}
 
 