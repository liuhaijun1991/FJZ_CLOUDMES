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
    public class H_BACKFLUSH_TEMP
    {
        public H_BACKFLUSH_TEMP()
        {
			
        }
 
       #region  H_BACKFLUSH_TEMP实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sapstationcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rec_type {get; set;}
	
	   
	   #endregion
    }
}
 
 