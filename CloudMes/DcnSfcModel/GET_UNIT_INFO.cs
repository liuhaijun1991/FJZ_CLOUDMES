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
    public class GET_UNIT_INFO
    {
        public GET_UNIT_INFO()
        {
			
        }
 
       #region  GET_UNIT_INFO实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrorCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrorString {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IMEI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WiFiMacAddr {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WiFiSSID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WiFiPassword {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WiFiGuestSSID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WiFiGuestPassword {get; set;}
	
	   
	   #endregion
    }
}
 
 