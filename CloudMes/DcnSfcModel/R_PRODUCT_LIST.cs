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
    public class R_PRODUCT_LIST
    {
        public R_PRODUCT_LIST()
        {
			
        }
 
       #region  R_PRODUCT_LIST实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VERSION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SET_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WORKTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 