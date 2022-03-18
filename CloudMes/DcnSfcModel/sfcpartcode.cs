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
    public class sfcpartcode
    {
        public sfcpartcode()
        {
			
        }
 
       #region  sfcpartcode实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string category {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string keystring {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int length {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool valid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool cancel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
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
 
 