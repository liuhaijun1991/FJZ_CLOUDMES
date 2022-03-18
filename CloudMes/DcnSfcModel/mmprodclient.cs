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
    public class mmprodclient
    {
        public mmprodclient()
        {
			
        }
 
       #region  mmprodclient实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartdesc {get; set;}
	
	
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
	   public DateTime lastpricedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string specialnote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ClientVersion {get; set;}
	
	
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
 
 