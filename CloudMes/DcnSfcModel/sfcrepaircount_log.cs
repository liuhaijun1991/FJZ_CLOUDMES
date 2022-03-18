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
    public class sfcrepaircount_log
    {
        public sfcrepaircount_log()
        {
			
        }
 
       #region  sfcrepaircount_log实体
 
	
	   /// <summary>
	   /// 序列號
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 維修工站
	   /// </summary>
	   public string failurestation {get; set;}
	
	
	   /// <summary>
	   /// 維修次數
	   /// </summary>
	   public int repaircount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int repairControlNum {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string repairControlBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime repairControlDt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Reason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Field5 {get; set;}
	
	   
	   #endregion
    }
}
 
 