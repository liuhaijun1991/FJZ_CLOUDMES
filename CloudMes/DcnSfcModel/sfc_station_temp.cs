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
    public class sfc_station_temp
    {
        public sfc_station_temp()
        {
			
        }
 
       #region  sfc_station_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkinby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkindate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkoutby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkoutdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int flag {get; set;}
	
	   
	   #endregion
    }
}
 
 