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
    public class sfc5DXchartSet
    {
        public sfc5DXchartSet()
        {
			
        }
 
       #region  sfc5DXchartSet实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int year {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int week {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TesterNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal falsecalls {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int testpcs {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal totalpins {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int failpcs {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int passpcs {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 