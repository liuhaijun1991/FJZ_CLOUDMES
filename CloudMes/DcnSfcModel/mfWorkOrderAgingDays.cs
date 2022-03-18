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
    public class mfWorkOrderAgingDays
    {
        public mfWorkOrderAgingDays()
        {
			
        }
 
       #region  mfWorkOrderAgingDays实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rev {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OrderType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReleaseDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int OpenQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short Days1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Days12 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short Days2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Days22 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short AllDays {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int OpenDays {get; set;}
	
	
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
 
 