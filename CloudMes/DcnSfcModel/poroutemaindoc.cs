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
    public class poroutemaindoc
    {
        public poroutemaindoc()
        {
			
        }
 
       #region  poroutemaindoc实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routepoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string documentname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string documentno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string note {get; set;}
	
	
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
 
 