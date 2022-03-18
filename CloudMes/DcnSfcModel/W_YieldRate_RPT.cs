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
    public class W_YieldRate_RPT
    {
        public W_YieldRate_RPT()
        {
			
        }
 
       #region  W_YieldRate_RPT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datatype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int yearFrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int yearTo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string weekList {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int total_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int pass_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int total_qty2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int pass_qty2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rate2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 