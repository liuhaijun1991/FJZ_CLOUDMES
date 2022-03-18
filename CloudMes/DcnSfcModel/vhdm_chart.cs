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
    public class vhdm_chart
    {
        public vhdm_chart()
        {
			
        }
 
       #region  vhdm_chart实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string route {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int years {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int week {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalfreshbuild {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalfreshfail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int damage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double fresh {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double rate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalreworkbuild {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalreworkfail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double rework {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string editby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime editdt {get; set;}
	
	   
	   #endregion
    }
}
 
 