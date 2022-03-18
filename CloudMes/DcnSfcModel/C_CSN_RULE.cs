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
    public class C_CSN_RULE
    {
        public C_CSN_RULE()
        {
			
        }
 
       #region  C_CSN_RULE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int csnlen {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prefix {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int prefixlen {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sitecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sitecodelen {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mfrcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mfrname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string editby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime edittime {get; set;}
	
	   
	   #endregion
    }
}
 
 