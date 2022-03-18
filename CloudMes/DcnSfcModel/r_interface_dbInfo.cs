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
    public class r_interface_dbInfo
    {
        public r_interface_dbInfo()
        {
			
        }
 
       #region  r_interface_dbInfo实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Factory_Code {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DB_Note {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Svr_IP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DB_Name {get; set;}
	
	   
	   #endregion
    }
}
 
 