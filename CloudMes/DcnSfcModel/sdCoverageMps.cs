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
    public class sdCoverageMps
    {
        public sdCoverageMps()
        {
			
        }
 
       #region  sdCoverageMps实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mps_version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mrp_version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime reqtodate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal reqqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime reqfromdate {get; set;}
	
	   
	   #endregion
    }
}
 
 