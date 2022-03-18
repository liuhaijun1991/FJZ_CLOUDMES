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
    public class R_BACKFLUSH_BACKUP
    {
        public R_BACKFLUSH_BACKUP()
        {
			
        }
 
       #region  R_BACKFLUSH_BACKUP实体
 
	
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
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EMP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EMPTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 