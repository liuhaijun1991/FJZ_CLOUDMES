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
    public class NSG_WAREHOUSE_CODE
    {
        public NSG_WAREHOUSE_CODE()
        {
			
        }
 
       #region  NSG_WAREHOUSE_CODE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAPCODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFCCODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REMARK {get; set;}
	
	   
	   #endregion
    }
}
 
 