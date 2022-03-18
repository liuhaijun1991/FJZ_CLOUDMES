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
    public class FAI_Config
    {
        public FAI_Config()
        {
			
        }
 
       #region  FAI_Config实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Remark {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDT {get; set;}
	
	   
	   #endregion
    }
}
 
 