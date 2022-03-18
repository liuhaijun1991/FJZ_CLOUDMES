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
    public class H_CycleCount_His
    {
        public H_CycleCount_His()
        {
			
        }
 
       #region  H_CycleCount_His实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string P_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SAP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SFC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int REALCOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int DIFF {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Container_TIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastSAPDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastSFCDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int AnalyseCount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AnalyseType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AnalyseOrder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AnalyseOwer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RealDiff {get; set;}
	
	   
	   #endregion
    }
}
 
 