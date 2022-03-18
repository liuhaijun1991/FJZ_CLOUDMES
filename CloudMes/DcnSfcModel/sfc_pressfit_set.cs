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
    public class sfc_pressfit_set
    {
        public sfc_pressfit_set()
        {
			
        }
 
       #region  sfc_pressfit_set实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double usl {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double sl {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double lsl {get; set;}
	
	
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
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string qita {get; set;}
	
	   
	   #endregion
    }
}
 
 