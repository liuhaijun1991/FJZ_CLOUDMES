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
    public class mmwarehouseuser
    {
        public mmwarehouseuser()
        {
			
        }
 
       #region  mmwarehouseuser实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string whid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string userid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string username {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime activedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disable {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime disabledate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool externaluser {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string companyname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field3 {get; set;}
	
	
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
 
 