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
    public class CheckPermission
    {
        public CheckPermission()
        {
			
        }
 
       #region  CheckPermission实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TABLE_QUALIFIER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TABLE_OWNER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TABLE_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GRANTOR {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GRANTEE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRIVILEGE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IS_GRANTABLE {get; set;}
	
	   
	   #endregion
    }
}
 
 