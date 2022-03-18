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
    public class R_OBA_REPORT
    {
        public R_OBA_REPORT()
        {
			
        }
 
       #region  R_OBA_REPORT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Lot_size {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Sampie_QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string First_OBA_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Failed_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RE_OBA_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OBA_analysis_report {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime inspection_date {get; set;}
	
	   
	   #endregion
    }
}
 
 