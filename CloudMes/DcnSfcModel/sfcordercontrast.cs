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
    public class sfcordercontrast
    {
        public sfcordercontrast()
        {
			
        }
 
       #region  sfcordercontrast实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string So_Num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Ship_date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Line_Num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Part_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 