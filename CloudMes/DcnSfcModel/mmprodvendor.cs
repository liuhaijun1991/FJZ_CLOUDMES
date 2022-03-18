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
    public class mmprodvendor
    {
        public mmprodvendor()
        {
			
        }
 
       #region  mmprodvendor实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string originate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal leadtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string unitleadtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string specialnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorversion {get; set;}
	
	
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
 
 