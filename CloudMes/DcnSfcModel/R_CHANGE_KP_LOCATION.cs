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
    public class R_CHANGE_KP_LOCATION
    {
        public R_CHANGE_KP_LOCATION()
        {
			
        }
 
       #region  R_CHANGE_KP_LOCATION实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATION_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOCATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WORKTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 