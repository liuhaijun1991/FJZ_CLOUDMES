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
    public class R_PRODUCT_MDS
    {
        public R_PRODUCT_MDS()
        {
			
        }
 
       #region  R_PRODUCT_MDS实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type {get; set;}
	
	
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
	   public string location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool MDS_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool VALID_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REPLACE_KP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime SCANDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITDT {get; set;}
	
	   
	   #endregion
    }
}
 
 