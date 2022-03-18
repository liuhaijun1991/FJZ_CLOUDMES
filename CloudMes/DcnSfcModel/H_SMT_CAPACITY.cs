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
    public class H_SMT_CAPACITY
    {
        public H_SMT_CAPACITY()
        {
			
        }
 
       #region  H_SMT_CAPACITY实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BUILDING {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FLOOR {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SMTCODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SEQUENCE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QTYBYHOUR {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime START_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime STOP_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double UTILIZATION_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ACTUAL_QUANTITY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int YIELD_QUANTITY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FIRST_PASS_QUANTITY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int REWORK_PASS_QUANTITY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int UPLOAD_STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int UPLOAD_COUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UPLOAD_ERROR_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UPLOAD_ERROR_MESSAGE {get; set;}
	
	   
	   #endregion
    }
}
 
 