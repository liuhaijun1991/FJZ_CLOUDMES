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
    public class sfc_OracleReport
    {
        public sfc_OracleReport()
        {
			
        }
 
       #region  sfc_OracleReport实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string percent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandatetime {get; set;}
	
	   
	   #endregion
    }
}
 
 