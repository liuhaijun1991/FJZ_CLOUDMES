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
    public class sfc_ta_station_mapping
    {
        public sfc_ta_station_mapping()
        {
			
        }
 
       #region  sfc_ta_station_mapping实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFC_station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TA_station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string valid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	   
	   #endregion
    }
}
 
 