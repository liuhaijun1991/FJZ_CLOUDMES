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
    public class C_AP_ROLE
    {
        public C_AP_ROLE()
        {
			
        }
 
       #region  C_AP_ROLE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ROLE_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AP_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTION_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool AP_FLAG {get; set;}
	
	
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
 
 