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
    public class R_STATION_KP_SET
    {
        public R_STATION_KP_SET()
        {
			
        }
 
       #region  R_STATION_KP_SET实体
 
	
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
	   public string KP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REPLACE_KP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int KP_STANDARDQTY {get; set;}
	
	
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
	   public string AUTOSCAN_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SETCOUNT {get; set;}
	
	   
	   #endregion
    }
}
 
 