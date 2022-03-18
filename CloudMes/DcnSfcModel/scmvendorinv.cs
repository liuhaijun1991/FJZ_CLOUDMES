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
    public class scmvendorinv
    {
        public scmvendorinv()
        {
			
        }
 
       #region  scmvendorinv实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string warehouseid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mypartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal currentqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal onwayqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal commitqty {get; set;}
	
	
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
 
 