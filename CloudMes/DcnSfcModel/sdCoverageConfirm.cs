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
    public class sdCoverageConfirm
    {
        public sdCoverageConfirm()
        {
			
        }
 
       #region  sdCoverageConfirm实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mrp_version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DB {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool confirmed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string confirmby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime confirmdt {get; set;}
	
	   
	   #endregion
    }
}
 
 