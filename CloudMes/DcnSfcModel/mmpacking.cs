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
    public class mmpacking
    {
        public mmpacking()
        {
			
        }
 
       #region  mmpacking实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pkid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string subpkid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int capacity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note {get; set;}
	
	
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
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 