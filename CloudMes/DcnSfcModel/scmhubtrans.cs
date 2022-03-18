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
    public class scmhubtrans
    {
        public scmhubtrans()
        {
			
        }
 
       #region  scmhubtrans实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pkeyno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime transdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pkeyno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string whid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string requestby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string refno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string refno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createdby {get; set;}
	
	   
	   #endregion
    }
}
 
 