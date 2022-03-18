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
    public class R_STATION_AUTOSCAN
    {
        public R_STATION_AUTOSCAN()
        {
			
        }
 
       #region  R_STATION_AUTOSCAN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pack_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string label_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int palletqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ppid_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string label_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 