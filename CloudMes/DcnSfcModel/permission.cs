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
    public class permission
    {
        public permission()
        {
			
        }
 
       #region  permission实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string permissiontype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string permissionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string functionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool modification {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lupby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lupdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public Guid rowguid {get; set;}
	
	   
	   #endregion
    }
}
 
 