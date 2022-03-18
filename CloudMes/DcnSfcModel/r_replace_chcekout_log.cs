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
    public class r_replace_chcekout_log
    {
        public r_replace_chcekout_log()
        {
			
        }
 
       #region  r_replace_chcekout_log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string action {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string userid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scantime {get; set;}
	
	   
	   #endregion
    }
}
 
 