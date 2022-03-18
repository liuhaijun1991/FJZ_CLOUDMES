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
    public class s_loading_qty_alert
    {
        public s_loading_qty_alert()
        {
			
        }
 
       #region  s_loading_qty_alert实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string functionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime last_run_time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int total_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int used_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int model_num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int alert_max {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int alert_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool alert_flag {get; set;}
	
	
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
 
 