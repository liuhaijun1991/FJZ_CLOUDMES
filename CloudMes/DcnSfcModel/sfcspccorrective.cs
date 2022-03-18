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
    public class sfcspccorrective
    {
        public sfcspccorrective()
        {
			
        }
 
       #region  sfcspccorrective实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string hourperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string problem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int DPPM {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string corrective {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool corrected {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rootcause {get; set;}
	
	   
	   #endregion
    }
}
 
 