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
    public class r_interface_euserlog
    {
        public r_interface_euserlog()
        {
			
        }
 
       #region  r_interface_euserlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGONNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LOGDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACTIONTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACTIONMEMO {get; set;}
	
	   
	   #endregion
    }
}
 
 