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
    public class R_CHANGE_KP
    {
        public R_CHANGE_KP()
        {
			
        }
 
       #region  R_CHANGE_KP实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATION_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATE_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOT_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VENDER_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal CHANGE_QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WORKTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 