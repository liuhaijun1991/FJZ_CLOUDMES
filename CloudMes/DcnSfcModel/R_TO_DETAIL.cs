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
    public class R_TO_DETAIL
    {
        public R_TO_DETAIL()
        {
			
        }
 
       #region  R_TO_DETAIL实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TO_ITEM_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN_CUSTOMER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime DN_STARTTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime DN_ENDTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN_FLAG {get; set;}
	
	   
	   #endregion
    }
}
 
 