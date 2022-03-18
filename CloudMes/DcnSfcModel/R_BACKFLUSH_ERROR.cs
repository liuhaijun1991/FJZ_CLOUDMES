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
    public class R_BACKFLUSH_ERROR
    {
        public R_BACKFLUSH_ERROR()
        {
			
        }
 
       #region  R_BACKFLUSH_ERROR实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WORKORDERNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAPSTATIONCODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ERRORMESSAGE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ERRTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 