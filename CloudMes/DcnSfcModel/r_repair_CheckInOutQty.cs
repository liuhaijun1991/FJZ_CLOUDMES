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
    public class r_repair_CheckInOutQty
    {
        public r_repair_CheckInOutQty()
        {
			
        }
 
       #region  r_repair_CheckInOutQty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logonname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dataType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdSeries {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool valid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime executeDt {get; set;}
	
	   
	   #endregion
    }
}
 
 