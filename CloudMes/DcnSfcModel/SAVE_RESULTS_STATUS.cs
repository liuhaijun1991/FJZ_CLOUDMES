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
    public class SAVE_RESULTS_STATUS
    {
        public SAVE_RESULTS_STATUS()
        {
			
        }
 
       #region  SAVE_RESULTS_STATUS实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FactoryStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FactoryErrorCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FactoryErrorString {get; set;}
	
	   
	   #endregion
    }
}
 
 