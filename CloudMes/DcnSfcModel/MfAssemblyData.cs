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
    public class MfAssemblyData
    {
        public MfAssemblyData()
        {
			
        }
 
       #region  MfAssemblyData实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ConfigHeaderID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rev {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CustPartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditTime {get; set;}
	
	   
	   #endregion
    }
}
 
 