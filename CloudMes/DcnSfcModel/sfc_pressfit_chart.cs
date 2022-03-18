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
    public class sfc_pressfit_chart
    {
        public sfc_pressfit_chart()
        {
			
        }
 
       #region  sfc_pressfit_chart实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pn {get; set;}
	
	
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
	   public double avrg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double sigma {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double cp {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double ca {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double cpk {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string editor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime edate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string des {get; set;}
	
	   
	   #endregion
    }
}
 
 