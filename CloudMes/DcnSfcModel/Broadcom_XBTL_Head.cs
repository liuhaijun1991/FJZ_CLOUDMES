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
    public class Broadcom_XBTL_Head
    {
        public Broadcom_XBTL_Head()
        {
			
        }
 
       #region  Broadcom_XBTL_Head实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte[] AppName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string StratTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EndTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TimeZone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Site {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string StationHostName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductionName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TestPhase {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte[] TestEnvironment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CreatDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STD_FLAG {get; set;}
	
	   
	   #endregion
    }
}
 
 