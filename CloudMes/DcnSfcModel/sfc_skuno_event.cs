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
    public class sfc_skuno_event
    {
        public sfc_skuno_event()
        {
			
        }
 
       #region  sfc_skuno_event实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductClass {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventPoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FY_Goal {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 