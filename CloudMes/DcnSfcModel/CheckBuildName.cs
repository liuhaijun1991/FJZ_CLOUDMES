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
    public class CheckBuildName
    {
        public CheckBuildName()
        {
			
        }
 
       #region  CheckBuildName实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int checkId {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductionType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BuildName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 