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
    public class s_cqa_control
    {
        public s_cqa_control()
        {
			
        }
 
       #region  s_cqa_control实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shiporder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int packageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cust {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string so {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalPackages {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pallet {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool doc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool sfc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool package {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool label {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool truckNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool loadTruck {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rejectReason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sampleSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cqaResult {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool firstResult {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string firsteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime firsteditdt {get; set;}
	
	
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
 
 