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
    public class mfshifttime
    {
        public mfshifttime()
        {
			
        }
 
       #region  mfshifttime实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string hourperiod {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal uphratio {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string firsthour {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasthour {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fromtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string totime {get; set;}
	
	
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
 
 