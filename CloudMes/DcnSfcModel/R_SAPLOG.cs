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
    public class R_SAPLOG
    {
        public R_SAPLOG()
        {
			
        }
 
       #region  R_SAPLOG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string errortype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string errormessage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	   
	   #endregion
    }
}
 
 