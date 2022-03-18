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
    public class R_scrap
    {
        public R_scrap()
        {
			
        }
 
       #region  R_scrap实体
 
	
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
	   public string from_storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string to_storage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scraptime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sap_flag {get; set;}
	
	   
	   #endregion
    }
}
 
 