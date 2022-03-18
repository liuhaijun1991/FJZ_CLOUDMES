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
    public class sfcfailurecodeinfo
    {
        public sfcfailurecodeinfo()
        {
			
        }
 
       #region  sfcfailurecodeinfo实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string codename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string codedesc1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string codedesc2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replace_flag {get; set;}
	
	
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
 
 