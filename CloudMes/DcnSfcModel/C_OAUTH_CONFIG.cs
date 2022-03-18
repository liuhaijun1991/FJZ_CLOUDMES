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
    public class C_OAUTH_CONFIG
    {
        public C_OAUTH_CONFIG()
        {
			
        }
 
       #region  C_OAUTH_CONFIG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACCOUNTID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CERTIFICATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SCOPES {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOKEN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EDIT_EMP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EDIT_TIME {get; set;}
	
	   
	   #endregion
    }
}
 
 