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
    public class C_USER_ROLE
    {
        public C_USER_ROLE()
        {
			
        }
 
       #region  C_USER_ROLE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ROLE_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EMP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int PERMISSION_FLAG {get; set;}
	
	
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
 
 