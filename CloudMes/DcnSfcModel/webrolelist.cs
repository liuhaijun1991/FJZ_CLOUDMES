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
    public class webrolelist
    {
        public webrolelist()
        {
			
        }
 
       #region  webrolelist实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ROLENAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WEBCODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITDATE {get; set;}
	
	   
	   #endregion
    }
}
 
 