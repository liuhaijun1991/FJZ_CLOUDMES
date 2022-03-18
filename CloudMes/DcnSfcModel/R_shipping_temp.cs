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
    public class R_shipping_temp
    {
        public R_shipping_temp()
        {
			
        }
 
       #region  R_shipping_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TO_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN_ITEM_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int P_NO_QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int REAL_QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WORKTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 