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
    public class scmeventlog
    {
        public scmeventlog()
        {
			
        }
 
       #region  scmeventlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime logdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string keycode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventdetail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string warehouseid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mypartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal qty {get; set;}
	
	   
	   #endregion
    }
}
 
 