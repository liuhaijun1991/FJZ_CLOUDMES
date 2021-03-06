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
    public class MDSStructureLog
    {
        public MDSStructureLog()
        {
			
        }
 
       #region  MDSStructureLog实体
 
	
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
	   public DateTime AssemblyDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CPartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CSerialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CVendorID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CVendorName {get; set;}
	
	
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
 
 