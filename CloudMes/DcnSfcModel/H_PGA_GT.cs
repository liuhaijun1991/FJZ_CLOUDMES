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
    public class H_PGA_GT
    {
        public H_PGA_GT()
        {
			
        }
 
       #region  H_PGA_GT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SYSSERIALNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STORAGE_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PGATIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAP_FLAG {get; set;}
	
	   
	   #endregion
    }
}
 
 