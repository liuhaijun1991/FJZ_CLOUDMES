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
    public class MDSYieldLog
    {
        public MDSYieldLog()
        {
			
        }
 
       #region  MDSYieldLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DataPoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SysSerialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventPoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RecordType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PostDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrderNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RMAReg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Operater {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SignCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comments {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RefLocation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailureCPartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailureCSerialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ReplaceCSerialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Correction {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDT {get; set;}
	
	   
	   #endregion
    }
}
 
 