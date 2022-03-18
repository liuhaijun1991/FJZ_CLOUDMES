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
    public class R_Purge
    {
        public R_Purge()
        {
			
        }
 
       #region  R_Purge实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rootcase {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ReturnFlag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Returntime {get; set;}
	
	   
	   #endregion
    }
}
 
 