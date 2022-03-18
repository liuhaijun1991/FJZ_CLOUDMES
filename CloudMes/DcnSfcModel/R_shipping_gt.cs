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
    public class R_shipping_gt
    {
        public R_shipping_gt()
        {
			
        }
 
       #region  R_shipping_gt实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pallet_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carton_no {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	   
	   #endregion
    }
}
 
 